namespace PACS_Analyzer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.linkLabelChoose = new System.Windows.Forms.LinkLabel();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.labelTo = new System.Windows.Forms.Label();
            this.labelFrom = new System.Windows.Forms.Label();
            this.linkLabelGenerateGraphs = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonFind = new System.Windows.Forms.Button();
            this.comboBoxTill = new System.Windows.Forms.ComboBox();
            this.comboBoxFrom = new System.Windows.Forms.ComboBox();
            this.labelPeriodOfTime = new System.Windows.Forms.Label();
            this.labelProgress = new System.Windows.Forms.Label();
            this.progressBarMain = new System.Windows.Forms.ProgressBar();
            this.labelPACS = new System.Windows.Forms.Label();
            this.labelAnalyzer = new System.Windows.Forms.Label();
            this.openFileDialogChooseFile = new System.Windows.Forms.OpenFileDialog();
            this.backgroundWorkerFile = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerTable = new System.ComponentModel.BackgroundWorker();
            this.databaseMainDataSet1 = new PACS_Analyzer.DatabaseMainDataSet();
            this.richTextBoxDebug = new System.Windows.Forms.RichTextBox();
            this.groupBoxSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.databaseMainDataSet1)).BeginInit();
            this.SuspendLayout();
            // 
            // linkLabelChoose
            // 
            this.linkLabelChoose.ActiveLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.linkLabelChoose.AutoSize = true;
            this.linkLabelChoose.Font = new System.Drawing.Font("Lucida Sans", 12.25F);
            this.linkLabelChoose.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(187)))));
            this.linkLabelChoose.Location = new System.Drawing.Point(42, 38);
            this.linkLabelChoose.Name = "linkLabelChoose";
            this.linkLabelChoose.Size = new System.Drawing.Size(106, 19);
            this.linkLabelChoose.TabIndex = 0;
            this.linkLabelChoose.TabStop = true;
            this.linkLabelChoose.Text = "Pick the file";
            this.linkLabelChoose.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(187)))));
            this.linkLabelChoose.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelChoose_LinkClicked);
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(228)))), ((int)(((byte)(228)))));
            this.groupBoxSettings.Controls.Add(this.labelTo);
            this.groupBoxSettings.Controls.Add(this.labelFrom);
            this.groupBoxSettings.Controls.Add(this.linkLabelGenerateGraphs);
            this.groupBoxSettings.Controls.Add(this.label2);
            this.groupBoxSettings.Controls.Add(this.label1);
            this.groupBoxSettings.Controls.Add(this.buttonFind);
            this.groupBoxSettings.Controls.Add(this.comboBoxTill);
            this.groupBoxSettings.Controls.Add(this.comboBoxFrom);
            this.groupBoxSettings.Controls.Add(this.labelPeriodOfTime);
            this.groupBoxSettings.Controls.Add(this.linkLabelChoose);
            this.groupBoxSettings.Controls.Add(this.labelProgress);
            this.groupBoxSettings.Controls.Add(this.progressBarMain);
            this.groupBoxSettings.Font = new System.Drawing.Font("Lucida Sans", 12.25F);
            this.groupBoxSettings.ForeColor = System.Drawing.Color.Black;
            this.groupBoxSettings.Location = new System.Drawing.Point(12, 12);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(440, 312);
            this.groupBoxSettings.TabIndex = 2;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Settings";
            // 
            // labelTo
            // 
            this.labelTo.AutoSize = true;
            this.labelTo.Location = new System.Drawing.Point(42, 134);
            this.labelTo.Name = "labelTo";
            this.labelTo.Size = new System.Drawing.Size(32, 19);
            this.labelTo.TabIndex = 17;
            this.labelTo.Text = "to:";
            // 
            // labelFrom
            // 
            this.labelFrom.AutoSize = true;
            this.labelFrom.Location = new System.Drawing.Point(42, 105);
            this.labelFrom.Name = "labelFrom";
            this.labelFrom.Size = new System.Drawing.Size(55, 19);
            this.labelFrom.TabIndex = 16;
            this.labelFrom.Text = "from:";
            // 
            // linkLabelGenerateGraphs
            // 
            this.linkLabelGenerateGraphs.AutoSize = true;
            this.linkLabelGenerateGraphs.Font = new System.Drawing.Font("Lucida Sans", 12F, System.Drawing.FontStyle.Italic);
            this.linkLabelGenerateGraphs.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(187)))));
            this.linkLabelGenerateGraphs.Location = new System.Drawing.Point(163, 269);
            this.linkLabelGenerateGraphs.Name = "linkLabelGenerateGraphs";
            this.linkLabelGenerateGraphs.Size = new System.Drawing.Size(135, 18);
            this.linkLabelGenerateGraphs.TabIndex = 14;
            this.linkLabelGenerateGraphs.TabStop = true;
            this.linkLabelGenerateGraphs.Text = "Generate Vectors";
            this.linkLabelGenerateGraphs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGenerateGraphs_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Lucida Sans", 12F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(6, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 18);
            this.label2.TabIndex = 11;
            this.label2.Text = "2)";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Lucida Sans", 12F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(6, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 18);
            this.label1.TabIndex = 10;
            this.label1.Text = "1)";
            // 
            // buttonFind
            // 
            this.buttonFind.BackColor = System.Drawing.Color.White;
            this.buttonFind.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonFind.Enabled = false;
            this.buttonFind.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.buttonFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFind.Font = new System.Drawing.Font("Lucida Sans", 12.2F);
            this.buttonFind.ForeColor = System.Drawing.Color.Black;
            this.buttonFind.Location = new System.Drawing.Point(10, 190);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(419, 56);
            this.buttonFind.TabIndex = 9;
            this.buttonFind.Text = "Find anomalies";
            this.buttonFind.UseVisualStyleBackColor = false;
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            this.buttonFind.MouseEnter += new System.EventHandler(this.buttonFind_MouseEnter);
            this.buttonFind.MouseLeave += new System.EventHandler(this.buttonFind_MouseLeave);
            // 
            // comboBoxTill
            // 
            this.comboBoxTill.Enabled = false;
            this.comboBoxTill.Font = new System.Drawing.Font("Lucida Sans", 10.25F);
            this.comboBoxTill.FormattingEnabled = true;
            this.comboBoxTill.Location = new System.Drawing.Point(115, 135);
            this.comboBoxTill.Name = "comboBoxTill";
            this.comboBoxTill.Size = new System.Drawing.Size(235, 24);
            this.comboBoxTill.TabIndex = 5;
            // 
            // comboBoxFrom
            // 
            this.comboBoxFrom.Enabled = false;
            this.comboBoxFrom.Font = new System.Drawing.Font("Lucida Sans", 10.25F);
            this.comboBoxFrom.FormattingEnabled = true;
            this.comboBoxFrom.Location = new System.Drawing.Point(115, 102);
            this.comboBoxFrom.Name = "comboBoxFrom";
            this.comboBoxFrom.Size = new System.Drawing.Size(235, 24);
            this.comboBoxFrom.TabIndex = 3;
            // 
            // labelPeriodOfTime
            // 
            this.labelPeriodOfTime.AutoSize = true;
            this.labelPeriodOfTime.Font = new System.Drawing.Font("Lucida Sans", 12.25F);
            this.labelPeriodOfTime.Location = new System.Drawing.Point(40, 72);
            this.labelPeriodOfTime.Name = "labelPeriodOfTime";
            this.labelPeriodOfTime.Size = new System.Drawing.Size(196, 19);
            this.labelPeriodOfTime.TabIndex = 3;
            this.labelPeriodOfTime.Text = "Choose period of time";
            // 
            // labelProgress
            // 
            this.labelProgress.AutoSize = true;
            this.labelProgress.BackColor = System.Drawing.Color.Transparent;
            this.labelProgress.Location = new System.Drawing.Point(374, 0);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(39, 19);
            this.labelProgress.TabIndex = 12;
            this.labelProgress.Text = "0/0";
            this.labelProgress.Visible = false;
            // 
            // progressBarMain
            // 
            this.progressBarMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(241)))), ((int)(((byte)(237)))));
            this.progressBarMain.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(221)))), ((int)(((byte)(217)))));
            this.progressBarMain.Location = new System.Drawing.Point(0, 1);
            this.progressBarMain.Name = "progressBarMain";
            this.progressBarMain.Size = new System.Drawing.Size(469, 23);
            this.progressBarMain.TabIndex = 8;
            this.progressBarMain.Visible = false;
            // 
            // labelPACS
            // 
            this.labelPACS.AutoSize = true;
            this.labelPACS.BackColor = System.Drawing.Color.Transparent;
            this.labelPACS.Font = new System.Drawing.Font("Lucida Sans", 10.25F, System.Drawing.FontStyle.Bold);
            this.labelPACS.ForeColor = System.Drawing.Color.Black;
            this.labelPACS.Location = new System.Drawing.Point(312, 349);
            this.labelPACS.Name = "labelPACS";
            this.labelPACS.Size = new System.Drawing.Size(44, 16);
            this.labelPACS.TabIndex = 6;
            this.labelPACS.Text = "PACS";
            // 
            // labelAnalyzer
            // 
            this.labelAnalyzer.AutoSize = true;
            this.labelAnalyzer.BackColor = System.Drawing.Color.Transparent;
            this.labelAnalyzer.Font = new System.Drawing.Font("Lucida Sans", 10.25F, System.Drawing.FontStyle.Bold);
            this.labelAnalyzer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelAnalyzer.Location = new System.Drawing.Point(362, 349);
            this.labelAnalyzer.Name = "labelAnalyzer";
            this.labelAnalyzer.Size = new System.Drawing.Size(71, 16);
            this.labelAnalyzer.TabIndex = 7;
            this.labelAnalyzer.Text = "Analyzer";
            // 
            // openFileDialogChooseFile
            // 
            this.openFileDialogChooseFile.FileName = "FixedProxy_EmpData.csv";
            this.openFileDialogChooseFile.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialogChooseFile_FileOk);
            // 
            // backgroundWorkerFile
            // 
            this.backgroundWorkerFile.WorkerReportsProgress = true;
            this.backgroundWorkerFile.WorkerSupportsCancellation = true;
            this.backgroundWorkerFile.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerFile_DoWork);
            this.backgroundWorkerFile.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerFile_ProgressChanged);
            this.backgroundWorkerFile.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerFile_RunWorkerCompleted);
            // 
            // backgroundWorkerTable
            // 
            this.backgroundWorkerTable.WorkerReportsProgress = true;
            this.backgroundWorkerTable.WorkerSupportsCancellation = true;
            this.backgroundWorkerTable.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerTable_DoWork);
            this.backgroundWorkerTable.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerTable_ProgressChanged);
            this.backgroundWorkerTable.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerTable_RunWorkerCompleted);
            // 
            // databaseMainDataSet1
            // 
            this.databaseMainDataSet1.DataSetName = "DatabaseMainDataSet";
            this.databaseMainDataSet1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // richTextBoxDebug
            // 
            this.richTextBoxDebug.Location = new System.Drawing.Point(458, 13);
            this.richTextBoxDebug.Name = "richTextBoxDebug";
            this.richTextBoxDebug.Size = new System.Drawing.Size(258, 311);
            this.richTextBoxDebug.TabIndex = 8;
            this.richTextBoxDebug.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            this.ClientSize = new System.Drawing.Size(768, 379);
            this.Controls.Add(this.richTextBoxDebug);
            this.Controls.Add(this.labelAnalyzer);
            this.Controls.Add(this.labelPACS);
            this.Controls.Add(this.groupBoxSettings);
            this.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "PACS Analyzer";
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.databaseMainDataSet1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabelChoose;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.ComboBox comboBoxTill;
        private System.Windows.Forms.ComboBox comboBoxFrom;
        private System.Windows.Forms.Label labelPeriodOfTime;
        private System.Windows.Forms.Label labelPACS;
        private System.Windows.Forms.Label labelAnalyzer;
        private System.Windows.Forms.OpenFileDialog openFileDialogChooseFile;
        private System.Windows.Forms.Button buttonFind;
        private System.ComponentModel.BackgroundWorker backgroundWorkerFile;
        private System.Windows.Forms.ProgressBar progressBarMain;
        private System.ComponentModel.BackgroundWorker backgroundWorkerTable;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.LinkLabel linkLabelGenerateGraphs;
        private System.Windows.Forms.Label labelTo;
        private System.Windows.Forms.Label labelFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private DatabaseMainDataSet databaseMainDataSet1;
        private System.Windows.Forms.RichTextBox richTextBoxDebug;
    }
}

