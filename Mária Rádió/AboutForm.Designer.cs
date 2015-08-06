using System.ComponentModel;
using System.Windows.Forms;
using Maria_Radio.Controls;

namespace Maria_Radio
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblAuthor = new System.Windows.Forms.Label();
            this.lblAuthorVal = new System.Windows.Forms.Label();
            this.lblMailVal = new System.Windows.Forms.Label();
            this.lblMail = new System.Windows.Forms.Label();
            this.lblWebPageVal = new System.Windows.Forms.Label();
            this.lblWebPage = new System.Windows.Forms.Label();
            this.lblVersionVal = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.ibtnX = new Maria_Radio.Controls.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblTitle.Location = new System.Drawing.Point(127, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(170, 32);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Mária Rádió";
            this.lblTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblTitle_MouseDown);
            // 
            // lblAuthor
            // 
            this.lblAuthor.AutoSize = true;
            this.lblAuthor.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblAuthor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblAuthor.Location = new System.Drawing.Point(130, 56);
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Size = new System.Drawing.Size(82, 16);
            this.lblAuthor.TabIndex = 2;
            this.lblAuthor.Text = "Készítette:";
            // 
            // lblAuthorVal
            // 
            this.lblAuthorVal.AutoSize = true;
            this.lblAuthorVal.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblAuthorVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblAuthorVal.Location = new System.Drawing.Point(218, 56);
            this.lblAuthorVal.Name = "lblAuthorVal";
            this.lblAuthorVal.Size = new System.Drawing.Size(117, 16);
            this.lblAuthorVal.TabIndex = 3;
            this.lblAuthorVal.Text = "Gyurász Krisztián";
            // 
            // lblMailVal
            // 
            this.lblMailVal.AutoSize = true;
            this.lblMailVal.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblMailVal.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblMailVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblMailVal.Location = new System.Drawing.Point(218, 82);
            this.lblMailVal.Name = "lblMailVal";
            this.lblMailVal.Size = new System.Drawing.Size(119, 16);
            this.lblMailVal.TabIndex = 5;
            this.lblMailVal.Text = "sektor@sektor.hu";
            this.lblMailVal.Click += new System.EventHandler(this.lblMailVal_Click);
            // 
            // lblMail
            // 
            this.lblMail.AutoSize = true;
            this.lblMail.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblMail.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblMail.Location = new System.Drawing.Point(130, 82);
            this.lblMail.Name = "lblMail";
            this.lblMail.Size = new System.Drawing.Size(54, 16);
            this.lblMail.TabIndex = 4;
            this.lblMail.Text = "E-mail:";
            // 
            // lblWebPageVal
            // 
            this.lblWebPageVal.AutoSize = true;
            this.lblWebPageVal.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblWebPageVal.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblWebPageVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblWebPageVal.Location = new System.Drawing.Point(218, 108);
            this.lblWebPageVal.Name = "lblWebPageVal";
            this.lblWebPageVal.Size = new System.Drawing.Size(125, 16);
            this.lblWebPageVal.TabIndex = 7;
            this.lblWebPageVal.Text = "www.mariaradio.hu";
            this.lblWebPageVal.Click += new System.EventHandler(this.lblWebPageVal_Click);
            // 
            // lblWebPage
            // 
            this.lblWebPage.AutoSize = true;
            this.lblWebPage.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblWebPage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblWebPage.Location = new System.Drawing.Point(130, 108);
            this.lblWebPage.Name = "lblWebPage";
            this.lblWebPage.Size = new System.Drawing.Size(61, 16);
            this.lblWebPage.TabIndex = 6;
            this.lblWebPage.Text = "Honlap:";
            // 
            // lblVersionVal
            // 
            this.lblVersionVal.AutoSize = true;
            this.lblVersionVal.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblVersionVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblVersionVal.Location = new System.Drawing.Point(218, 134);
            this.lblVersionVal.Name = "lblVersionVal";
            this.lblVersionVal.Size = new System.Drawing.Size(52, 16);
            this.lblVersionVal.TabIndex = 9;
            this.lblVersionVal.Text = "1.0.0.0";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblVersion.Location = new System.Drawing.Point(130, 134);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(55, 16);
            this.lblVersion.TabIndex = 8;
            this.lblVersion.Text = "Verzió:";
            // 
            // pbLogo
            // 
            this.pbLogo.Image = global::Maria_Radio.Properties.Resources.maria_about;
            this.pbLogo.Location = new System.Drawing.Point(12, 12);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new System.Drawing.Size(109, 138);
            this.pbLogo.TabIndex = 0;
            this.pbLogo.TabStop = false;
            // 
            // ibtnX
            // 
            this.ibtnX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ibtnX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.ibtnX.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ibtnX.ForeColor = System.Drawing.Color.Transparent;
            this.ibtnX.HoverImage = global::Maria_Radio.Properties.Resources.x_h;
            this.ibtnX.Location = new System.Drawing.Point(334, 12);
            this.ibtnX.Name = "ibtnX";
            this.ibtnX.NormalImage = global::Maria_Radio.Properties.Resources.x;
            this.ibtnX.PushedImage = global::Maria_Radio.Properties.Resources.x_p;
            this.ibtnX.Size = new System.Drawing.Size(24, 24);
            this.ibtnX.TabIndex = 10;
            this.ibtnX.Click += new System.EventHandler(this.ibtnX_Click);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.ClientSize = new System.Drawing.Size(366, 166);
            this.Controls.Add(this.ibtnX);
            this.Controls.Add(this.lblVersionVal);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblWebPageVal);
            this.Controls.Add(this.lblWebPage);
            this.Controls.Add(this.lblMailVal);
            this.Controls.Add(this.lblMail);
            this.Controls.Add(this.lblAuthorVal);
            this.Controls.Add(this.lblAuthor);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pbLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AboutForm";
            this.Opacity = 0.9D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AboutForm";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AboutForm_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox pbLogo;
        private Label lblTitle;
        private Label lblAuthor;
        private Label lblAuthorVal;
        private Label lblMailVal;
        private Label lblMail;
        private Label lblWebPageVal;
        private Label lblWebPage;
        private Label lblVersionVal;
        private Label lblVersion;
        private ImageButton ibtnX;
    }
}