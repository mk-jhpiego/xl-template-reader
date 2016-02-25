namespace template_reader
{
    partial class Form1
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
            this.btnSelectFileToImport = new System.Windows.Forms.Button();
            this.lblSelectedFile = new System.Windows.Forms.Label();
            this.btnUpdateProgramIndicatorsList = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnSaveToCsv = new System.Windows.Forms.Button();
            this.btnSaveToServer = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSelectFileToImport
            // 
            this.btnSelectFileToImport.BackColor = System.Drawing.Color.Transparent;
            this.btnSelectFileToImport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnSelectFileToImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectFileToImport.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelectFileToImport.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSelectFileToImport.Location = new System.Drawing.Point(12, 12);
            this.btnSelectFileToImport.Name = "btnSelectFileToImport";
            this.btnSelectFileToImport.Size = new System.Drawing.Size(117, 52);
            this.btnSelectFileToImport.TabIndex = 1;
            this.btnSelectFileToImport.Text = "Select File";
            this.btnSelectFileToImport.UseVisualStyleBackColor = false;
            this.btnSelectFileToImport.Click += new System.EventHandler(this.btnSelectFileToImport_Click);
            // 
            // lblSelectedFile
            // 
            this.lblSelectedFile.AutoSize = true;
            this.lblSelectedFile.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblSelectedFile.Location = new System.Drawing.Point(9, 71);
            this.lblSelectedFile.Name = "lblSelectedFile";
            this.lblSelectedFile.Size = new System.Drawing.Size(80, 13);
            this.lblSelectedFile.TabIndex = 1;
            this.lblSelectedFile.Text = "No file selected";
            this.lblSelectedFile.Click += new System.EventHandler(this.label1_Click);
            // 
            // btnUpdateProgramIndicatorsList
            // 
            this.btnUpdateProgramIndicatorsList.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnUpdateProgramIndicatorsList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateProgramIndicatorsList.Location = new System.Drawing.Point(638, 12);
            this.btnUpdateProgramIndicatorsList.Name = "btnUpdateProgramIndicatorsList";
            this.btnUpdateProgramIndicatorsList.Size = new System.Drawing.Size(109, 63);
            this.btnUpdateProgramIndicatorsList.TabIndex = 4;
            this.btnUpdateProgramIndicatorsList.Text = " Update Program Indicators List";
            this.btnUpdateProgramIndicatorsList.UseVisualStyleBackColor = false;
            this.btnUpdateProgramIndicatorsList.Visible = false;
            this.btnUpdateProgramIndicatorsList.Click += new System.EventHandler(this.btnUpdateProgramIndicatorsList_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(562, 345);
            this.dataGridView1.TabIndex = 5;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnSaveToCsv);
            this.splitContainer1.Panel1.Controls.Add(this.btnSaveToServer);
            this.splitContainer1.Panel1.Controls.Add(this.btnSelectFileToImport);
            this.splitContainer1.Panel1.Controls.Add(this.lblSelectedFile);
            this.splitContainer1.Panel1.Controls.Add(this.btnUpdateProgramIndicatorsList);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(759, 437);
            this.splitContainer1.SplitterDistance = 88;
            this.splitContainer1.TabIndex = 6;
            // 
            // btnSaveToCsv
            // 
            this.btnSaveToCsv.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnSaveToCsv.Enabled = false;
            this.btnSaveToCsv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveToCsv.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveToCsv.Location = new System.Drawing.Point(464, 12);
            this.btnSaveToCsv.Name = "btnSaveToCsv";
            this.btnSaveToCsv.Size = new System.Drawing.Size(95, 52);
            this.btnSaveToCsv.TabIndex = 3;
            this.btnSaveToCsv.Text = "Save as CSV";
            this.btnSaveToCsv.UseVisualStyleBackColor = false;
            this.btnSaveToCsv.Click += new System.EventHandler(this.btnSaveToCsv_Click);
            // 
            // btnSaveToServer
            // 
            this.btnSaveToServer.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnSaveToServer.Enabled = false;
            this.btnSaveToServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveToServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveToServer.Location = new System.Drawing.Point(149, 12);
            this.btnSaveToServer.Name = "btnSaveToServer";
            this.btnSaveToServer.Size = new System.Drawing.Size(90, 52);
            this.btnSaveToServer.TabIndex = 2;
            this.btnSaveToServer.Text = "Save to Server";
            this.btnSaveToServer.UseVisualStyleBackColor = false;
            this.btnSaveToServer.Click += new System.EventHandler(this.btnSaveToServer_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer2.Size = new System.Drawing.Size(759, 345);
            this.splitContainer2.SplitterDistance = 193;
            this.splitContainer2.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 437);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Data Importer";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSelectFileToImport;
        private System.Windows.Forms.Label lblSelectedFile;
        private System.Windows.Forms.Button btnUpdateProgramIndicatorsList;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnSaveToCsv;
        private System.Windows.Forms.Button btnSaveToServer;
        private System.Windows.Forms.SplitContainer splitContainer2;
    }
}

