namespace LogJsonExtractor
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSelectFiles = new System.Windows.Forms.Button();
            this.btnProcess = new System.Windows.Forms.Button();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnExportJson = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.listBoxFiles = new System.Windows.Forms.ListBox();
            this.lblResults = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSelectFiles
            // 
            this.btnSelectFiles.Location = new System.Drawing.Point(20, 20);
            this.btnSelectFiles.Name = "btnSelectFiles";
            this.btnSelectFiles.Size = new System.Drawing.Size(120, 30);
            this.btnSelectFiles.TabIndex = 0;
            this.btnSelectFiles.Text = "Выбрать файлы";
            this.btnSelectFiles.UseVisualStyleBackColor = true;
            this.btnSelectFiles.Click += new System.EventHandler(this.BtnSelectFiles_Click);
            // 
            // btnProcess
            // 
            this.btnProcess.Enabled = false;
            this.btnProcess.Location = new System.Drawing.Point(150, 20);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(120, 30);
            this.btnProcess.TabIndex = 1;
            this.btnProcess.Text = "Обработать";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.BtnProcess_Click);
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Enabled = false;
            this.btnExportExcel.Location = new System.Drawing.Point(20, 370);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(150, 30);
            this.btnExportExcel.TabIndex = 2;
            this.btnExportExcel.Text = "Экспорт в Excel";
            this.btnExportExcel.UseVisualStyleBackColor = true;
            this.btnExportExcel.Click += new System.EventHandler(this.BtnExportExcel_Click);
            // 
            // btnExportJson
            // 
            this.btnExportJson.Enabled = false;
            this.btnExportJson.Location = new System.Drawing.Point(180, 370);
            this.btnExportJson.Name = "btnExportJson";
            this.btnExportJson.Size = new System.Drawing.Size(150, 30);
            this.btnExportJson.TabIndex = 3;
            this.btnExportJson.Text = "Экспорт в JSON";
            this.btnExportJson.UseVisualStyleBackColor = true;
            this.btnExportJson.Click += new System.EventHandler(this.BtnExportJson_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(20, 240);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(95, 15);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Готов к работе";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(20, 270);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(750, 20);
            this.progressBar.TabIndex = 5;
            // 
            // listBoxFiles
            // 
            this.listBoxFiles.FormattingEnabled = true;
            this.listBoxFiles.ItemHeight = 15;
            this.listBoxFiles.Location = new System.Drawing.Point(20, 70);
            this.listBoxFiles.Name = "listBoxFiles";
            this.listBoxFiles.Size = new System.Drawing.Size(750, 154);
            this.listBoxFiles.TabIndex = 6;
            // 
            // lblResults
            // 
            this.lblResults.AutoSize = true;
            this.lblResults.Location = new System.Drawing.Point(20, 310);
            this.lblResults.Name = "lblResults";
            this.lblResults.Size = new System.Drawing.Size(0, 15);
            this.lblResults.TabIndex = 7;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(280, 20);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 30);
            this.btnClear.TabIndex = 8;
            this.btnClear.Text = "Очистить";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lblResults);
            this.Controls.Add(this.listBoxFiles);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnExportJson);
            this.Controls.Add(this.btnExportExcel);
            this.Controls.Add(this.btnProcess);
            this.Controls.Add(this.btnSelectFiles);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Log JSON Extractor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectFiles;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.Button btnExportJson;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.ListBox listBoxFiles;
        private System.Windows.Forms.Label lblResults;
        private System.Windows.Forms.Button btnClear;
    }
}