using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LogExtractor
{
    public partial class Form1 : Form
    {
        private string selectedFilePath = "";
        private List<JObject> extractedJsons = new List<JObject>();
        private readonly object lockObject = new object();

        public Form1()
        {
            InitializeComponent();
            // Устанавливаем лицензию для EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Log files (*.log)|*.log|Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePath = openFileDialog.FileName;
                    txtFilePath.Text = selectedFilePath;
                    btnProcess.Enabled = true;
                    lblStatus.Text = "Файл выбран";
                }
            }
        }

        private async void btnProcess_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath) || !File.Exists(selectedFilePath))
            {
                MessageBox.Show("Пожалуйста, выберите корректный файл!", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnProcess.Enabled = false;
            btnSaveToExcel.Enabled = false;
            btnSaveToJson.Enabled = false;
            extractedJsons.Clear();
            txtOutput.Clear();
            
            lblStatus.Text = "Обработка файла...";
            progressBar.Style = ProgressBarStyle.Marquee;

            try
            {
                await ProcessLogFileAsync();
                
                lblStatus.Text = $"Обработка завершена! Найдено записей: {extractedJsons.Count}";
                
                if (extractedJsons.Count > 0)
                {
                    btnSaveToExcel.Enabled = true;
                    btnSaveToJson.Enabled = true;
                    DisplayResults();
                }
                else
                {
                    txtOutput.Text = "Не найдено записей с методом 'bo.trans.credit' и полем 'inn'";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке файла: {ex.Message}", "Ошибка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Ошибка обработки";
            }
            finally
            {
                progressBar.Style = ProgressBarStyle.Continuous;
                btnProcess.Enabled = true;
            }
        }

        private async Task ProcessLogFileAsync()
        {
            const int numberOfThreads = 10;
            var lines = await File.ReadAllLinesAsync(selectedFilePath);
            var chunks = SplitIntoChunks(lines, numberOfThreads);
            var tasks = new List<Task>();

            foreach (var chunk in chunks)
            {
                tasks.Add(Task.Run(() => ProcessChunk(chunk)));
            }

            await Task.WhenAll(tasks);
        }

        private void ProcessChunk(string[] lines)
        {
            var regex = new Regex(@"REQUEST BODY: (\{.*\})");
            var localResults = new List<JObject>();

            foreach (string line in lines)
            {
                try
                {
                    var match = regex.Match(line);
                    if (match.Success)
                    {
                        string jsonString = match.Groups[1].Value;
                        
                        if (JsonDocument.Parse(jsonString) != null)
                        {
                            var jsonObject = JObject.Parse(jsonString);
                            
                            // Проверяем метод
                            var method = jsonObject["method"]?.ToString();
                            if (method == "bo.trans.credit")
                            {
                                // Проверяем наличие поля inn
                                var inn = jsonObject.SelectToken("params.tran.creditSource.inn");
                                if (inn != null)
                                {
                                    localResults.Add(jsonObject);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Игнорируем строки с невалидным JSON
                    continue;
                }
            }

            // Добавляем результаты в общий список потокобезопасно
            lock (lockObject)
            {
                extractedJsons.AddRange(localResults);
            }
        }

        private string[][] SplitIntoChunks(string[] lines, int numberOfChunks)
        {
            int chunkSize = (int)Math.Ceiling((double)lines.Length / numberOfChunks);
            var chunks = new List<string[]>();

            for (int i = 0; i < lines.Length; i += chunkSize)
            {
                int actualChunkSize = Math.Min(chunkSize, lines.Length - i);
                var chunk = new string[actualChunkSize];
                Array.Copy(lines, i, chunk, 0, actualChunkSize);
                chunks.Add(chunk);
            }

            return chunks.ToArray();
        }

        private void DisplayResults()
        {
            if (extractedJsons.Count == 0)
            {
                txtOutput.Text = "Результатов не найдено";
                return;
            }

            var displayText = new System.Text.StringBuilder();
            displayText.AppendLine($"Найдено записей: {extractedJsons.Count}\n");

            // Показываем первые 5 записей для предварительного просмотра
            int displayCount = Math.Min(5, extractedJsons.Count);
            for (int i = 0; i < displayCount; i++)
            {
                displayText.AppendLine($"Запись {i + 1}:");
                displayText.AppendLine(extractedJsons[i].ToString(Formatting.Indented));
                displayText.AppendLine(new string('-', 80));
            }

            if (extractedJsons.Count > 5)
            {
                displayText.AppendLine($"... и еще {extractedJsons.Count - 5} записей");
            }

            txtOutput.Text = displayText.ToString();
        }

        private void btnSaveToExcel_Click(object sender, EventArgs e)
        {
            if (extractedJsons.Count == 0)
            {
                MessageBox.Show("Нет данных для сохранения!", "Информация", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                saveFileDialog.FileName = $"extracted_data_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        SaveToExcel(saveFileDialog.FileName);
                        MessageBox.Show($"Данные сохранены в файл: {saveFileDialog.FileName}", "Успех", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении в Excel: {ex.Message}", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnSaveToJson_Click(object sender, EventArgs e)
        {
            if (extractedJsons.Count == 0)
            {
                MessageBox.Show("Нет данных для сохранения!", "Информация", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JSON files (*.json)|*.json";
                saveFileDialog.FileName = $"extracted_data_{DateTime.Now:yyyyMMdd_HHmmss}.json";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        SaveToJson(saveFileDialog.FileName);
                        MessageBox.Show($"Данные сохранены в файл: {saveFileDialog.FileName}", "Успех", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при сохранении в JSON: {ex.Message}", "Ошибка", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SaveToExcel(string filePath)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Extracted Data");
                
                // Заголовки
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Method";
                worksheet.Cells[1, 3].Value = "PAN4Last";
                worksheet.Cells[1, 4].Value = "Account";
                worksheet.Cells[1, 5].Value = "Amount";
                worksheet.Cells[1, 6].Value = "MerchantId";
                worksheet.Cells[1, 7].Value = "TerminalId";
                worksheet.Cells[1, 8].Value = "INN";
                worksheet.Cells[1, 9].Value = "SenderName";
                worksheet.Cells[1, 10].Value = "Purpose";
                worksheet.Cells[1, 11].Value = "Full JSON";

                // Данные
                for (int i = 0; i < extractedJsons.Count; i++)
                {
                    var json = extractedJsons[i];
                    int row = i + 2;

                    worksheet.Cells[row, 1].Value = json["id"]?.ToString();
                    worksheet.Cells[row, 2].Value = json["method"]?.ToString();
                    worksheet.Cells[row, 3].Value = json.SelectToken("params.tran.card.pan4Last")?.ToString();
                    worksheet.Cells[row, 4].Value = json.SelectToken("params.tran.card.account")?.ToString();
                    worksheet.Cells[row, 5].Value = json.SelectToken("params.tran.amount")?.ToString();
                    worksheet.Cells[row, 6].Value = json.SelectToken("params.tran.merchantId")?.ToString();
                    worksheet.Cells[row, 7].Value = json.SelectToken("params.tran.terminalId")?.ToString();
                    worksheet.Cells[row, 8].Value = json.SelectToken("params.tran.creditSource.inn")?.ToString();
                    worksheet.Cells[row, 9].Value = json.SelectToken("params.tran.creditSource.senderName")?.ToString();
                    worksheet.Cells[row, 10].Value = json.SelectToken("params.tran.creditSource.purpose")?.ToString();
                    worksheet.Cells[row, 11].Value = json.ToString(Formatting.None);
                }

                // Автоматическая настройка ширины колонок
                worksheet.Cells.AutoFitColumns();
                
                package.SaveAs(new FileInfo(filePath));
            }
        }
    }
}