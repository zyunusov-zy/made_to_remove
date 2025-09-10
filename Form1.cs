using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace LogJsonExtractor
{
    public partial class Form1 : Form
    {
        private BackgroundWorker worker;
        private ConcurrentBag<JsonData> extractedData;
        private List<string> logFiles;

        public Form1()
        {
            InitializeComponent();
            InitializeWorker();
            extractedData = new ConcurrentBag<JsonData>();
            logFiles = new List<string>();
        }

        private void InitializeWorker()
        {
            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void BtnSelectFiles_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Log files (*.log)|*.log|All files (*.*)|*.*";
                openFileDialog.Multiselect = true;
                openFileDialog.Title = "Выберите лог файлы";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    logFiles.AddRange(openFileDialog.FileNames);
                    UpdateFilesList();
                    btnProcess.Enabled = logFiles.Count > 0;
                }
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            logFiles.Clear();
            extractedData = new ConcurrentBag<JsonData>();
            UpdateFilesList();
            lblResults.Text = "";
            btnProcess.Enabled = false;
            btnExportExcel.Enabled = false;
            btnExportJson.Enabled = false;
            progressBar.Value = 0;
            lblStatus.Text = "Готов к работе";
        }

        private void UpdateFilesList()
        {
            listBoxFiles.Items.Clear();
            foreach (var file in logFiles)
            {
                listBoxFiles.Items.Add(Path.GetFileName(file));
            }
        }

        private void BtnProcess_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy) return;

            extractedData = new ConcurrentBag<JsonData>();
            btnProcess.Enabled = false;
            btnExportExcel.Enabled = false;
            btnExportJson.Enabled = false;
            progressBar.Value = 0;

            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var tasks = new List<Task>();
            var semaphore = new System.Threading.SemaphoreSlim(10, 10); // Ограничиваем 10 потоками

            for (int i = 0; i < logFiles.Count; i++)
            {
                var fileIndex = i;
                var filePath = logFiles[fileIndex];

                var task = Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        ProcessLogFile(filePath, fileIndex);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
        }

        private void ProcessLogFile(string filePath, int fileIndex)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);
                var jsonRegex = new Regex(@"REQUEST BODY:\s*(\{.*\})");

                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var match = jsonRegex.Match(line);

                    if (match.Success)
                    {
                        try
                        {
                            var jsonString = match.Groups[1].Value;
                            var jsonDoc = JsonDocument.Parse(jsonString);
                            var root = jsonDoc.RootElement;

                            // Проверяем method
                            if (root.TryGetProperty("method", out var methodProp) &&
                                methodProp.GetString() == "bo.trans.credit")
                            {
                                // Проверяем наличие inn
                                if (HasInnProperty(root))
                                {
                                    var data = new JsonData
                                    {
                                        FileName = Path.GetFileName(filePath),
                                        LineNumber = i + 1,
                                        JsonContent = jsonString,
                                        Method = methodProp.GetString(),
                                        Inn = ExtractInn(root)
                                    };

                                    extractedData.Add(data);
                                }
                            }
                        }
                        catch (JsonException)
                        {
                            // Игнорируем некорректный JSON
                        }
                    }
                }

                // Обновляем прогресс
                var progress = (fileIndex + 1) * 100 / logFiles.Count;
                worker.ReportProgress(progress, $"Обработан файл: {Path.GetFileName(filePath)}");
            }
            catch (Exception ex)
            {
                worker.ReportProgress(-1, $"Ошибка при обработке файла {Path.GetFileName(filePath)}: {ex.Message}");
            }
        }

        private bool HasInnProperty(JsonElement root)
        {
            if (root.TryGetProperty("params", out var paramsElement) &&
                paramsElement.TryGetProperty("tran", out var tranElement) &&
                tranElement.TryGetProperty("creditSource", out var creditSourceElement))
            {
                return creditSourceElement.TryGetProperty("inn", out _);
            }
            return false;
        }

        private string ExtractInn(JsonElement root)
        {
            try
            {
                return root.GetProperty("params")
                          .GetProperty("tran")
                          .GetProperty("creditSource")
                          .GetProperty("inn")
                          .GetString();
            }
            catch
            {
                return "";
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage >= 0)
            {
                progressBar.Value = e.ProgressPercentage;
            }
            lblStatus.Text = e.UserState?.ToString() ?? "";
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnProcess.Enabled = true;
            
            var count = extractedData.Count;
            lblResults.Text = $"Найдено записей: {count}";
            lblStatus.Text = "Обработка завершена";

            if (count > 0)
            {
                btnExportExcel.Enabled = true;
                btnExportJson.Enabled = true;
            }
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            if (extractedData.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                saveFileDialog.Title = "Сохранить Excel файл";
                saveFileDialog.FileName = $"extracted_data_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ExportToExcel(saveFileDialog.FileName);
                        MessageBox.Show($"Данные успешно экспортированы в {saveFileDialog.FileName}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при экспорте в Excel: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnExportJson_Click(object sender, EventArgs e)
        {
            if (extractedData.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JSON files (*.json)|*.json";
                saveFileDialog.Title = "Сохранить JSON файл";
                saveFileDialog.FileName = $"extracted_data_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        ExportToJson(saveFileDialog.FileName);
                        MessageBox.Show($"Данные успешно экспортированы в {saveFileDialog.FileName}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при экспорте в JSON: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportToExcel(string fileName)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Extracted Data");

                // Заголовки
                worksheet.Cell(1, 1).Value = "Файл";
                worksheet.Cell(1, 2).Value = "Строка";
                worksheet.Cell(1, 3).Value = "Method";
                worksheet.Cell(1, 4).Value = "INN";
                worksheet.Cell(1, 5).Value = "JSON Content";

                // Данные
                int row = 2;
                foreach (var item in extractedData.OrderBy(x => x.FileName).ThenBy(x => x.LineNumber))
                {
                    worksheet.Cell(row, 1).Value = item.FileName;
                    worksheet.Cell(row, 2).Value = item.LineNumber;
                    worksheet.Cell(row, 3).Value = item.Method;
                    worksheet.Cell(row, 4).Value = item.Inn;
                    worksheet.Cell(row, 5).Value = item.JsonContent;
                    row++;
                }

                // Автоподбор ширины столбцов
                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(fileName);
            }
        }

        private void ExportToJson(string fileName)
        {
            var dataToExport = extractedData.Select(x => new
            {
                fileName = x.FileName,
                lineNumber = x.LineNumber,
                method = x.Method,
                inn = x.Inn,
                jsonContent = x.JsonContent
            }).OrderBy(x => x.fileName).ThenBy(x => x.lineNumber).ToList();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            var jsonString = JsonSerializer.Serialize(dataToExport, options);
            File.WriteAllText(fileName, jsonString, System.Text.Encoding.UTF8);
        }
    }

    public class JsonData
    {
        public string FileName { get; set; }
        public int LineNumber { get; set; }
        public string JsonContent { get; set; }
        public string Method { get; set; }
        public string Inn { get; set; }
    }
}