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
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelBar1 = new System.Windows.Forms.Label();
            this.labelHorizontalBar1 = new System.Windows.Forms.Label();
            this.progressBarSteps = new System.Windows.Forms.ProgressBar();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.labelTo = new System.Windows.Forms.Label();
            this.labelFrom = new System.Windows.Forms.Label();
            this.linkLabelGenerateGraphs = new System.Windows.Forms.LinkLabel();
            this.buttonFind = new System.Windows.Forms.Button();
            this.comboBoxTill = new System.Windows.Forms.ComboBox();
            this.comboBoxFrom = new System.Windows.Forms.ComboBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.openFileDialogChooseFile = new System.Windows.Forms.OpenFileDialog();
            this.backgroundWorkerTable = new System.ComponentModel.BackgroundWorker();
            this.databaseMainDataSet1 = new PACS_Analyzer.DatabaseMainDataSet();
            this.labelWorkinProgress = new System.Windows.Forms.Label();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.progressBar3 = new System.Windows.Forms.ProgressBar();
            this.labelDone = new System.Windows.Forms.Label();
            this.groupBoxSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.databaseMainDataSet1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(227)))), ((int)(((byte)(227)))));
            this.groupBoxSettings.Controls.Add(this.label1);
            this.groupBoxSettings.Controls.Add(this.labelBar1);
            this.groupBoxSettings.Controls.Add(this.labelHorizontalBar1);
            this.groupBoxSettings.Controls.Add(this.progressBarSteps);
            this.groupBoxSettings.Controls.Add(this.buttonBrowse);
            this.groupBoxSettings.Controls.Add(this.labelTo);
            this.groupBoxSettings.Controls.Add(this.labelFrom);
            this.groupBoxSettings.Controls.Add(this.linkLabelGenerateGraphs);
            this.groupBoxSettings.Controls.Add(this.buttonFind);
            this.groupBoxSettings.Controls.Add(this.comboBoxTill);
            this.groupBoxSettings.Controls.Add(this.comboBoxFrom);
            this.groupBoxSettings.Font = new System.Drawing.Font("Lucida Sans", 12.25F);
            this.groupBoxSettings.ForeColor = System.Drawing.Color.Black;
            this.groupBoxSettings.Location = new System.Drawing.Point(12, 12);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(248, 343);
            this.groupBoxSettings.TabIndex = 2;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Enter += new System.EventHandler(this.groupBoxSettings_Enter);
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(6, 227);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(235, 2);
            this.label1.TabIndex = 23;
            this.label1.Text = "label1";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // labelBar1
            // 
            this.labelBar1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelBar1.Location = new System.Drawing.Point(6, 86);
            this.labelBar1.Name = "labelBar1";
            this.labelBar1.Size = new System.Drawing.Size(235, 2);
            this.labelBar1.TabIndex = 22;
            this.labelBar1.Text = "labelBar1";
            this.labelBar1.Click += new System.EventHandler(this.labelBar1_Click);
            // 
            // labelHorizontalBar1
            // 
            this.labelHorizontalBar1.AutoSize = true;
            this.labelHorizontalBar1.Location = new System.Drawing.Point(318, 79);
            this.labelHorizontalBar1.Name = "labelHorizontalBar1";
            this.labelHorizontalBar1.Size = new System.Drawing.Size(0, 19);
            this.labelHorizontalBar1.TabIndex = 21;
            this.labelHorizontalBar1.Click += new System.EventHandler(this.labelHorizontalBar1_Click);
            // 
            // progressBarSteps
            // 
            this.progressBarSteps.Location = new System.Drawing.Point(0, 1);
            this.progressBarSteps.Name = "progressBarSteps";
            this.progressBarSteps.Size = new System.Drawing.Size(248, 5);
            this.progressBarSteps.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBarSteps.TabIndex = 20;
            this.progressBarSteps.Click += new System.EventHandler(this.progressBarSteps_Click);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.BackColor = System.Drawing.Color.White;
            this.buttonBrowse.FlatAppearance.BorderColor = System.Drawing.Color.Red;
            this.buttonBrowse.Font = new System.Drawing.Font("Lucida Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBrowse.Location = new System.Drawing.Point(37, 26);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(173, 43);
            this.buttonBrowse.TabIndex = 19;
            this.buttonBrowse.Text = "Browse the log file...";
            this.buttonBrowse.UseVisualStyleBackColor = false;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // labelTo
            // 
            this.labelTo.Font = new System.Drawing.Font("Lucida Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTo.Location = new System.Drawing.Point(6, 165);
            this.labelTo.Name = "labelTo";
            this.labelTo.Size = new System.Drawing.Size(65, 15);
            this.labelTo.TabIndex = 17;
            this.labelTo.Text = "End time:";
            this.labelTo.Visible = false;
            this.labelTo.Click += new System.EventHandler(this.labelTo_Click);
            // 
            // labelFrom
            // 
            this.labelFrom.Font = new System.Drawing.Font("Lucida Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFrom.Location = new System.Drawing.Point(6, 109);
            this.labelFrom.Name = "labelFrom";
            this.labelFrom.Size = new System.Drawing.Size(72, 15);
            this.labelFrom.TabIndex = 16;
            this.labelFrom.Text = "Start time:";
            this.labelFrom.Visible = false;
            this.labelFrom.Click += new System.EventHandler(this.labelFrom_Click);
            // 
            // linkLabelGenerateGraphs
            // 
            this.linkLabelGenerateGraphs.Font = new System.Drawing.Font("Lucida Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelGenerateGraphs.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(187)))));
            this.linkLabelGenerateGraphs.Location = new System.Drawing.Point(6, 314);
            this.linkLabelGenerateGraphs.Name = "linkLabelGenerateGraphs";
            this.linkLabelGenerateGraphs.Size = new System.Drawing.Size(119, 15);
            this.linkLabelGenerateGraphs.TabIndex = 14;
            this.linkLabelGenerateGraphs.TabStop = true;
            this.linkLabelGenerateGraphs.Text = "Generate Vectors";
            this.linkLabelGenerateGraphs.Visible = false;
            this.linkLabelGenerateGraphs.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGenerateGraphs_LinkClicked);
            // 
            // buttonFind
            // 
            this.buttonFind.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.buttonFind.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonFind.Enabled = false;
            this.buttonFind.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(200)))), ((int)(((byte)(250)))));
            this.buttonFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFind.Font = new System.Drawing.Font("Lucida Sans", 12.2F);
            this.buttonFind.ForeColor = System.Drawing.Color.White;
            this.buttonFind.Location = new System.Drawing.Point(6, 249);
            this.buttonFind.Name = "buttonFind";
            this.buttonFind.Size = new System.Drawing.Size(235, 56);
            this.buttonFind.TabIndex = 9;
            this.buttonFind.Text = "Find anomalies";
            this.buttonFind.UseVisualStyleBackColor = false;
            this.buttonFind.Visible = false;
            this.buttonFind.Click += new System.EventHandler(this.buttonFind_Click);
            this.buttonFind.MouseEnter += new System.EventHandler(this.buttonFind_MouseEnter);
            this.buttonFind.MouseLeave += new System.EventHandler(this.buttonFind_MouseLeave);
            // 
            // comboBoxTill
            // 
            this.comboBoxTill.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTill.Enabled = false;
            this.comboBoxTill.Font = new System.Drawing.Font("Lucida Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxTill.FormattingEnabled = true;
            this.comboBoxTill.Location = new System.Drawing.Point(6, 183);
            this.comboBoxTill.Name = "comboBoxTill";
            this.comboBoxTill.Size = new System.Drawing.Size(235, 23);
            this.comboBoxTill.TabIndex = 5;
            this.comboBoxTill.Visible = false;
            this.comboBoxTill.SelectedIndexChanged += new System.EventHandler(this.comboBoxTill_SelectedIndexChanged);
            // 
            // comboBoxFrom
            // 
            this.comboBoxFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFrom.Enabled = false;
            this.comboBoxFrom.Font = new System.Drawing.Font("Lucida Sans", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxFrom.FormattingEnabled = true;
            this.comboBoxFrom.Location = new System.Drawing.Point(6, 127);
            this.comboBoxFrom.Name = "comboBoxFrom";
            this.comboBoxFrom.Size = new System.Drawing.Size(235, 23);
            this.comboBoxFrom.TabIndex = 3;
            this.comboBoxFrom.Visible = false;
            this.comboBoxFrom.SelectedIndexChanged += new System.EventHandler(this.comboBoxFrom_SelectedIndexChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.Color.Gainsboro;
            this.progressBar1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(213)))), ((int)(((byte)(84)))));
            this.progressBar1.Location = new System.Drawing.Point(12, 383);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(44, 23);
            this.progressBar1.TabIndex = 8;
            this.progressBar1.Visible = false;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // openFileDialogChooseFile
            // 
            this.openFileDialogChooseFile.FileName = "FixedProxy_EmpData.csv";
            this.openFileDialogChooseFile.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialogChooseFile_FileOk);
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
            // labelWorkinProgress
            // 
            this.labelWorkinProgress.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelWorkinProgress.Location = new System.Drawing.Point(12, 362);
            this.labelWorkinProgress.Name = "labelWorkinProgress";
            this.labelWorkinProgress.Size = new System.Drawing.Size(106, 19);
            this.labelWorkinProgress.TabIndex = 18;
            this.labelWorkinProgress.Text = "Work in progress...";
            this.labelWorkinProgress.Visible = false;
            this.labelWorkinProgress.Click += new System.EventHandler(this.labelWorkinProgress_Click);
            // 
            // progressBar2
            // 
            this.progressBar2.BackColor = System.Drawing.Color.Gainsboro;
            this.progressBar2.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.progressBar2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(213)))), ((int)(((byte)(84)))));
            this.progressBar2.Location = new System.Drawing.Point(62, 383);
            this.progressBar2.MarqueeAnimationSpeed = 50;
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(91, 23);
            this.progressBar2.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar2.TabIndex = 19;
            this.progressBar2.Visible = false;
            this.progressBar2.Click += new System.EventHandler(this.progressBar2_Click);
            // 
            // progressBar3
            // 
            this.progressBar3.BackColor = System.Drawing.Color.Gainsboro;
            this.progressBar3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(129)))), ((int)(((byte)(213)))), ((int)(((byte)(84)))));
            this.progressBar3.Location = new System.Drawing.Point(159, 383);
            this.progressBar3.Name = "progressBar3";
            this.progressBar3.Size = new System.Drawing.Size(101, 23);
            this.progressBar3.TabIndex = 20;
            this.progressBar3.Visible = false;
            this.progressBar3.Click += new System.EventHandler(this.progressBar3_Click);
            // 
            // labelDone
            // 
            this.labelDone.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelDone.Location = new System.Drawing.Point(124, 362);
            this.labelDone.Name = "labelDone";
            this.labelDone.Size = new System.Drawing.Size(136, 19);
            this.labelDone.TabIndex = 21;
            this.labelDone.Text = "0.25 seconds";
            this.labelDone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labelDone.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(236)))), ((int)(((byte)(236)))));
            this.ClientSize = new System.Drawing.Size(272, 415);
            this.Controls.Add(this.labelDone);
            this.Controls.Add(this.progressBar3);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.labelWorkinProgress);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.progressBar1);
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

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.ComboBox comboBoxTill;
        private System.Windows.Forms.ComboBox comboBoxFrom;
        private System.Windows.Forms.OpenFileDialog openFileDialogChooseFile;
        private System.Windows.Forms.Button buttonFind;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorkerTable;
        private System.Windows.Forms.LinkLabel linkLabelGenerateGraphs;
        private System.Windows.Forms.Label labelTo;
        private System.Windows.Forms.Label labelFrom;
        private DatabaseMainDataSet databaseMainDataSet1;
        private System.Windows.Forms.Label labelWorkinProgress;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.ProgressBar progressBar3;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.ProgressBar progressBarSteps;
        private System.Windows.Forms.Label labelBar1;
        private System.Windows.Forms.Label labelHorizontalBar1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelDone;
    }
}

