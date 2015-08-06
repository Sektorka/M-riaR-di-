using System.ComponentModel;
using System.Windows.Forms;
using Win7Progress;

namespace Updater
{
    partial class updaterForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(updaterForm));
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblFile = new System.Windows.Forms.Label();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblDownloaded = new System.Windows.Forms.Label();
            this.progressTotal = new Win7Progress.Windows7ProgressBar();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(13, 13);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(68, 13);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Fájl letöltése:";
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(79, 13);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(10, 13);
            this.lblFile.TabIndex = 1;
            this.lblFile.Text = ".";
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(16, 57);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(376, 23);
            this.progress.TabIndex = 2;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(13, 99);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(59, 13);
            this.lblTotal.TabIndex = 3;
            this.lblTotal.Text = "Állapot: 0%";
            // 
            // lblDownloaded
            // 
            this.lblDownloaded.AutoSize = true;
            this.lblDownloaded.Location = new System.Drawing.Point(13, 38);
            this.lblDownloaded.Name = "lblDownloaded";
            this.lblDownloaded.Size = new System.Drawing.Size(50, 13);
            this.lblDownloaded.TabIndex = 5;
            this.lblDownloaded.Text = "0 B / 0 B";
            // 
            // progressTotal
            // 
            this.progressTotal.ContainerControl = this;
            this.progressTotal.Location = new System.Drawing.Point(16, 115);
            this.progressTotal.Name = "progressTotal";
            this.progressTotal.ShowInTaskbar = true;
            this.progressTotal.Size = new System.Drawing.Size(376, 23);
            this.progressTotal.Step = 1;
            this.progressTotal.TabIndex = 4;
            // 
            // updaterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 150);
            this.Controls.Add(this.lblDownloaded);
            this.Controls.Add(this.progressTotal);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.lblFile);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "updaterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mária Rádió :: Program frissítő";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.updaterForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label lblStatus;
        private Label lblFile;
        private ProgressBar progress;
        private Label lblTotal;
        private Windows7ProgressBar progressTotal;
        private Label lblDownloaded;
    }
}