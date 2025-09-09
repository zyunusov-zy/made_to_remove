namespace LogExtractor
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnSelectFile;
        private Button btnProcess;
        private TextBox txtFilePath;
        private TextBox txtOutput;
        private ProgressBar progressBar;
        private Label lblStatus;
        private Label lblFile;
        private Label lblResults;
        private Button btnSaveToExcel;
        private Button btnSaveToJson;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnSelectFiles = new Button();
            this.btnProcess = new Button();
            this.lstFiles = new ListBox();
            this.txtOutput = new TextBox();
            this.progressBar = new ProgressBar();
            this.lblStatus = new Label();
            this.lblFiles = new Label();
            this.lblResults = new Label();
            this.btnSaveToExcel = new Button();
            this.btnSaveToJson = new Button();
            this.btnClearFiles = new Button();
            this.lblThreadsInfo = new Label();
            this.SuspendLayout();
            
            // Form1
            this.AutoScaleDimensions = new SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(900, 700);
            this.Text = "Log JSON Extractor - Multi-File Processing";
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // lblFiles
            this.lblFiles.AutoSize = true;
            this.lblFiles.Location = new Point(20, 20);
            this.lblFiles.Size = new Size(200, 20);
            this.lblFiles.Text = "Выберите файлы логов (до 10):";
            
            // lstFiles
            this.lstFiles.Location = new Point(20, 45);
            this.lstFiles.Size = new Size(600, 120);
            this.lstFiles.HorizontalScrollbar = true;
            this.lstFiles.SelectionMode = SelectionMode.MultiExtended;
            
            // btnSelectFiles
            this.btnSelectFiles.Location = new Point(640, 45);
            this.btnSelectFiles.Size = new Size(120, 35);
            this.btnSelectFiles.Text = "Добавить файлы";
            this.btnSelectFiles.UseVisualStyleBackColor = true;
            this.btnSelectFiles.Click += new EventHandler(this.btnSelectFiles_Click);
            
            // btnClearFiles
            this.btnClearFiles.Location = new Point(640, 90);
            this.btnClearFiles.Size = new Size(120, 35);
            this.btnClearFiles.Text = "Очистить список";
            this.btnClearFiles.UseVisualStyleBackColor = true;
            this.btnClearFiles.Click += new EventHandler(this.btnClearFiles_Click);
            
            // btnProcess
            this.btnProcess.Location = new Point(640, 135);
            this.btnProcess.Size = new Size(120, 35);
            this.btnProcess.Text = "Обработать";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Enabled = false;
            this.btnProcess.Click += new EventHandler(this.btnProcess_Click);
            
            // lblThreadsInfo
            this.lblThreadsInfo.AutoSize = true;
            this.lblThreadsInfo.Location = new Point(780, 45);
            this.lblThreadsInfo.Size = new Size(100, 60);
            this.lblThreadsInfo.Text = "Режим работы:\n10 потоков\nпараллельно";
            this.lblThreadsInfo.ForeColor = Color.Blue;
            
            // progressBar
            this.progressBar.Location = new Point(20, 180);
            this.progressBar.Size = new Size(860, 25);
            this.progressBar.Style = ProgressBarStyle.Continuous;
            
            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new Point(20, 215);
            this.lblStatus.Size = new Size(60, 20);
            this.lblStatus.Text = "Готов";
            
            // lblResults
            this.lblResults.AutoSize = true;
            this.lblResults.Location = new Point(20, 245);
            this.lblResults.Size = new Size(90, 20);
            this.lblResults.Text = "Результаты:";
            
            // txtOutput
            this.txtOutput.Location = new Point(20, 270);
            this.txtOutput.Multiline = true;
            this.txtOutput.ScrollBars = ScrollBars.Both;
            this.txtOutput.Size = new Size(860, 350);
            this.txtOutput.ReadOnly = true;
            this.txtOutput.Font = new Font("Consolas", 9F);
            
            // btnSaveToExcel
            this.btnSaveToExcel.Location = new Point(20, 635);
            this.btnSaveToExcel.Size = new Size(150, 35);
            this.btnSaveToExcel.Text = "Сохранить в Excel";
            this.btnSaveToExcel.UseVisualStyleBackColor = true;
            this.btnSaveToExcel.Enabled = false;
            this.btnSaveToExcel.Click += new EventHandler(this.btnSaveToExcel_Click);
            
            // btnSaveToJson
            this.btnSaveToJson.Location = new Point(190, 635);
            this.btnSaveToJson.Size = new Size(150, 35);
            this.btnSaveToJson.Text = "Сохранить в JSON";
            this.btnSaveToJson.UseVisualStyleBackColor = true;
            this.btnSaveToJson.Enabled = false;
            this.btnSaveToJson.Click += new EventHandler(this.btnSaveToJson_Click);
            
            // Добавляем все контролы на форму
            this.Controls.Add(this.lblFiles);
            this.Controls.Add(this.lstFiles);
            this.Controls.Add(this.btnSelectFiles);
            this.Controls.Add(this.btnClearFiles);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.lblThreadsInfo);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblResults);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.btnSaveToExcel);
            this.Controls.Add(this.btnSaveToJson);
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}