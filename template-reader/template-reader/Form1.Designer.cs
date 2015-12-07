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
            this.btnUpdateProgramAreasList = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSelectFileToImport
            // 
            this.btnSelectFileToImport.Location = new System.Drawing.Point(13, 13);
            this.btnSelectFileToImport.Name = "btnSelectFileToImport";
            this.btnSelectFileToImport.Size = new System.Drawing.Size(136, 43);
            this.btnSelectFileToImport.TabIndex = 0;
            this.btnSelectFileToImport.Text = "Select fiel to import data";
            this.btnSelectFileToImport.UseVisualStyleBackColor = true;
            this.btnSelectFileToImport.Click += new System.EventHandler(this.btnSelectFileToImport_Click);
            // 
            // lblSelectedFile
            // 
            this.lblSelectedFile.AutoSize = true;
            this.lblSelectedFile.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblSelectedFile.Location = new System.Drawing.Point(156, 28);
            this.lblSelectedFile.Name = "lblSelectedFile";
            this.lblSelectedFile.Size = new System.Drawing.Size(80, 13);
            this.lblSelectedFile.TabIndex = 1;
            this.lblSelectedFile.Text = "No file selected";
            this.lblSelectedFile.Click += new System.EventHandler(this.label1_Click);
            // 
            // btnUpdateProgramIndicatorsList
            // 
            this.btnUpdateProgramIndicatorsList.Location = new System.Drawing.Point(486, 13);
            this.btnUpdateProgramIndicatorsList.Name = "btnUpdateProgramIndicatorsList";
            this.btnUpdateProgramIndicatorsList.Size = new System.Drawing.Size(136, 43);
            this.btnUpdateProgramIndicatorsList.TabIndex = 3;
            this.btnUpdateProgramIndicatorsList.Text = " Update Program Indicators List";
            this.btnUpdateProgramIndicatorsList.UseVisualStyleBackColor = true;
            this.btnUpdateProgramIndicatorsList.Visible = false;
            this.btnUpdateProgramIndicatorsList.Click += new System.EventHandler(this.btnUpdateProgramIndicatorsList_Click);
            // 
            // btnUpdateProgramAreasList
            // 
            this.btnUpdateProgramAreasList.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnUpdateProgramAreasList.Location = new System.Drawing.Point(344, 13);
            this.btnUpdateProgramAreasList.Name = "btnUpdateProgramAreasList";
            this.btnUpdateProgramAreasList.Size = new System.Drawing.Size(136, 43);
            this.btnUpdateProgramAreasList.TabIndex = 4;
            this.btnUpdateProgramAreasList.Text = " Update Program Areas List";
            this.btnUpdateProgramAreasList.UseVisualStyleBackColor = false;
            this.btnUpdateProgramAreasList.Visible = false;
            this.btnUpdateProgramAreasList.Click += new System.EventHandler(this.btnUpdateProgramAreasList_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(1, 62);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(621, 363);
            this.dataGridView1.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 437);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnUpdateProgramAreasList);
            this.Controls.Add(this.btnUpdateProgramIndicatorsList);
            this.Controls.Add(this.lblSelectedFile);
            this.Controls.Add(this.btnSelectFileToImport);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSelectFileToImport;
        private System.Windows.Forms.Label lblSelectedFile;
        private System.Windows.Forms.Button btnUpdateProgramIndicatorsList;
        private System.Windows.Forms.Button btnUpdateProgramAreasList;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}

