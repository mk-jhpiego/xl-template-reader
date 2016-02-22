namespace template_reader.views
{
    partial class ProgramSelector
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
            this.btnDod = new System.Windows.Forms.Button();
            this.btnVmmc = new System.Windows.Forms.Button();
            this.btnCapacityBuilding = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnDod
            // 
            this.btnDod.BackColor = System.Drawing.Color.Sienna;
            this.btnDod.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDod.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDod.Location = new System.Drawing.Point(16, 20);
            this.btnDod.Name = "btnDod";
            this.btnDod.Size = new System.Drawing.Size(180, 106);
            this.btnDod.TabIndex = 3;
            this.btnDod.Text = "DOD";
            this.btnDod.UseVisualStyleBackColor = false;
            this.btnDod.Click += new System.EventHandler(this.btnDod_Click);
            // 
            // btnVmmc
            // 
            this.btnVmmc.BackColor = System.Drawing.Color.DarkGreen;
            this.btnVmmc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVmmc.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVmmc.Location = new System.Drawing.Point(202, 20);
            this.btnVmmc.Name = "btnVmmc";
            this.btnVmmc.Size = new System.Drawing.Size(180, 106);
            this.btnVmmc.TabIndex = 4;
            this.btnVmmc.Text = "IHP - VMMC";
            this.btnVmmc.UseVisualStyleBackColor = false;
            this.btnVmmc.Click += new System.EventHandler(this.btnVmmc_Click);
            // 
            // btnCapacityBuilding
            // 
            this.btnCapacityBuilding.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.btnCapacityBuilding.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCapacityBuilding.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCapacityBuilding.Location = new System.Drawing.Point(388, 20);
            this.btnCapacityBuilding.Name = "btnCapacityBuilding";
            this.btnCapacityBuilding.Size = new System.Drawing.Size(198, 106);
            this.btnCapacityBuilding.TabIndex = 5;
            this.btnCapacityBuilding.Text = "IHP - Capacity Building and Training";
            this.btnCapacityBuilding.UseVisualStyleBackColor = false;
            this.btnCapacityBuilding.Click += new System.EventHandler(this.btnCapacityBuilding_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.btnCapacityBuilding);
            this.panel1.Controls.Add(this.btnVmmc);
            this.panel1.Controls.Add(this.btnDod);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(589, 229);
            this.panel1.TabIndex = 15;
            // 
            // btnCancel
            // 
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(468, 160);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 42);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ProgramSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(614, 250);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ProgramSelector";
            this.Text = "ProgramSelector";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDod;
        private System.Windows.Forms.Button btnVmmc;
        private System.Windows.Forms.Button btnCapacityBuilding;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
    }
}