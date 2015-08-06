using System.ComponentModel;
using System.Windows.Forms;
using Maria_Radio.Controls;

namespace Maria_Radio
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.tabSettings = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.chbCheckUpdates = new System.Windows.Forms.CheckBox();
            this.chbNotifyProgram = new System.Windows.Forms.CheckBox();
            this.chbAlwaysOnTop = new System.Windows.Forms.CheckBox();
            this.chbShowPopup = new System.Windows.Forms.CheckBox();
            this.chbMinimizeToTray = new System.Windows.Forms.CheckBox();
            this.chbRunAtStartUp = new System.Windows.Forms.CheckBox();
            this.tabRecord = new System.Windows.Forms.TabPage();
            this.tbRecordPath = new Maria_Radio.Controls.PlaceholderTextBox();
            this.btnRecordPathBrowse = new System.Windows.Forms.Button();
            this.chbAfterRecordOpenFolder = new System.Windows.Forms.CheckBox();
            this.tabProxy = new System.Windows.Forms.TabPage();
            this.chbUseProxy = new System.Windows.Forms.CheckBox();
            this.tbPass = new System.Windows.Forms.TextBox();
            this.lblPass = new System.Windows.Forms.Label();
            this.tbUser = new System.Windows.Forms.TextBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.tbHost = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.nudPort = new System.Windows.Forms.NumericUpDown();
            this.lblHost = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.fbdRecordPath = new System.Windows.Forms.FolderBrowserDialog();
            this.tabSettings.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabRecord.SuspendLayout();
            this.tabProxy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
            this.SuspendLayout();
            // 
            // tabSettings
            // 
            this.tabSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabSettings.Controls.Add(this.tabGeneral);
            this.tabSettings.Controls.Add(this.tabRecord);
            this.tabSettings.Controls.Add(this.tabProxy);
            this.tabSettings.Location = new System.Drawing.Point(12, 12);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.SelectedIndex = 0;
            this.tabSettings.Size = new System.Drawing.Size(380, 168);
            this.tabSettings.TabIndex = 0;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.chbCheckUpdates);
            this.tabGeneral.Controls.Add(this.chbNotifyProgram);
            this.tabGeneral.Controls.Add(this.chbAlwaysOnTop);
            this.tabGeneral.Controls.Add(this.chbShowPopup);
            this.tabGeneral.Controls.Add(this.chbMinimizeToTray);
            this.tabGeneral.Controls.Add(this.chbRunAtStartUp);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(372, 142);
            this.tabGeneral.TabIndex = 1;
            this.tabGeneral.Text = "Általános";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // chbCheckUpdates
            // 
            this.chbCheckUpdates.AutoSize = true;
            this.chbCheckUpdates.Location = new System.Drawing.Point(6, 121);
            this.chbCheckUpdates.Name = "chbCheckUpdates";
            this.chbCheckUpdates.Size = new System.Drawing.Size(208, 17);
            this.chbCheckUpdates.TabIndex = 6;
            this.chbCheckUpdates.Text = "Frissítés ellenőrzése minden indításkor";
            this.chbCheckUpdates.UseVisualStyleBackColor = true;
            // 
            // chbNotifyProgram
            // 
            this.chbNotifyProgram.AutoSize = true;
            this.chbNotifyProgram.Location = new System.Drawing.Point(6, 98);
            this.chbNotifyProgram.Name = "chbNotifyProgram";
            this.chbNotifyProgram.Size = new System.Drawing.Size(184, 17);
            this.chbNotifyProgram.TabIndex = 5;
            this.chbNotifyProgram.Text = "Buborék értesítés műsorváltáskor";
            this.chbNotifyProgram.UseVisualStyleBackColor = true;
            // 
            // chbAlwaysOnTop
            // 
            this.chbAlwaysOnTop.AutoSize = true;
            this.chbAlwaysOnTop.Location = new System.Drawing.Point(6, 52);
            this.chbAlwaysOnTop.Name = "chbAlwaysOnTop";
            this.chbAlwaysOnTop.Size = new System.Drawing.Size(95, 17);
            this.chbAlwaysOnTop.TabIndex = 3;
            this.chbAlwaysOnTop.Text = "Mindíg legfelül";
            this.chbAlwaysOnTop.UseVisualStyleBackColor = true;
            // 
            // chbShowPopup
            // 
            this.chbShowPopup.AutoSize = true;
            this.chbShowPopup.Location = new System.Drawing.Point(6, 75);
            this.chbShowPopup.Name = "chbShowPopup";
            this.chbShowPopup.Size = new System.Drawing.Size(205, 17);
            this.chbShowPopup.TabIndex = 4;
            this.chbShowPopup.Text = "Buborék megjeleítése lekicsinyítéskor";
            this.chbShowPopup.UseVisualStyleBackColor = true;
            // 
            // chbMinimizeToTray
            // 
            this.chbMinimizeToTray.AutoSize = true;
            this.chbMinimizeToTray.Location = new System.Drawing.Point(6, 29);
            this.chbMinimizeToTray.Name = "chbMinimizeToTray";
            this.chbMinimizeToTray.Size = new System.Drawing.Size(132, 17);
            this.chbMinimizeToTray.TabIndex = 2;
            this.chbMinimizeToTray.Text = "Lekicsinyítés a tálcára";
            this.chbMinimizeToTray.UseVisualStyleBackColor = true;
            // 
            // chbRunAtStartUp
            // 
            this.chbRunAtStartUp.AutoSize = true;
            this.chbRunAtStartUp.Location = new System.Drawing.Point(6, 6);
            this.chbRunAtStartUp.Name = "chbRunAtStartUp";
            this.chbRunAtStartUp.Size = new System.Drawing.Size(206, 17);
            this.chbRunAtStartUp.TabIndex = 1;
            this.chbRunAtStartUp.Text = "Program indítása Windows-szal együtt";
            this.chbRunAtStartUp.UseVisualStyleBackColor = true;
            // 
            // tabRecord
            // 
            this.tabRecord.Controls.Add(this.tbRecordPath);
            this.tabRecord.Controls.Add(this.btnRecordPathBrowse);
            this.tabRecord.Controls.Add(this.chbAfterRecordOpenFolder);
            this.tabRecord.Location = new System.Drawing.Point(4, 22);
            this.tabRecord.Name = "tabRecord";
            this.tabRecord.Padding = new System.Windows.Forms.Padding(3);
            this.tabRecord.Size = new System.Drawing.Size(372, 142);
            this.tabRecord.TabIndex = 2;
            this.tabRecord.Text = "Felvétel";
            this.tabRecord.UseVisualStyleBackColor = true;
            // 
            // tbRecordPath
            // 
            this.tbRecordPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRecordPath.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.tbRecordPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic);
            this.tbRecordPath.ForeColor = System.Drawing.Color.LightGray;
            this.tbRecordPath.Location = new System.Drawing.Point(6, 6);
            this.tbRecordPath.Name = "tbRecordPath";
            this.tbRecordPath.PlaceholderText = "Felvétel mentésének helye";
            this.tbRecordPath.ReadOnly = true;
            this.tbRecordPath.Size = new System.Drawing.Size(283, 20);
            this.tbRecordPath.TabIndex = 7;
            this.tbRecordPath.Click += new System.EventHandler(this.tbRecordPath_Click);
            // 
            // btnRecordPathBrowse
            // 
            this.btnRecordPathBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRecordPathBrowse.Location = new System.Drawing.Point(295, 4);
            this.btnRecordPathBrowse.Name = "btnRecordPathBrowse";
            this.btnRecordPathBrowse.Size = new System.Drawing.Size(71, 23);
            this.btnRecordPathBrowse.TabIndex = 8;
            this.btnRecordPathBrowse.Text = "Tallózás";
            this.btnRecordPathBrowse.UseVisualStyleBackColor = true;
            this.btnRecordPathBrowse.Click += new System.EventHandler(this.btnRecordPathBrowse_Click);
            // 
            // chbAfterRecordOpenFolder
            // 
            this.chbAfterRecordOpenFolder.AutoSize = true;
            this.chbAfterRecordOpenFolder.Location = new System.Drawing.Point(6, 32);
            this.chbAfterRecordOpenFolder.Name = "chbAfterRecordOpenFolder";
            this.chbAfterRecordOpenFolder.Size = new System.Drawing.Size(292, 17);
            this.chbAfterRecordOpenFolder.TabIndex = 9;
            this.chbAfterRecordOpenFolder.Text = "Felvétel befejezése után, a felvétel helyének megnyitása";
            this.chbAfterRecordOpenFolder.UseVisualStyleBackColor = true;
            // 
            // tabProxy
            // 
            this.tabProxy.Controls.Add(this.chbUseProxy);
            this.tabProxy.Controls.Add(this.tbPass);
            this.tabProxy.Controls.Add(this.lblPass);
            this.tabProxy.Controls.Add(this.tbUser);
            this.tabProxy.Controls.Add(this.lblUser);
            this.tabProxy.Controls.Add(this.tbHost);
            this.tabProxy.Controls.Add(this.lblPort);
            this.tabProxy.Controls.Add(this.nudPort);
            this.tabProxy.Controls.Add(this.lblHost);
            this.tabProxy.Location = new System.Drawing.Point(4, 22);
            this.tabProxy.Name = "tabProxy";
            this.tabProxy.Padding = new System.Windows.Forms.Padding(3);
            this.tabProxy.Size = new System.Drawing.Size(372, 142);
            this.tabProxy.TabIndex = 0;
            this.tabProxy.Text = "Proxy";
            this.tabProxy.UseVisualStyleBackColor = true;
            // 
            // chbUseProxy
            // 
            this.chbUseProxy.AutoSize = true;
            this.chbUseProxy.Location = new System.Drawing.Point(9, 6);
            this.chbUseProxy.Name = "chbUseProxy";
            this.chbUseProxy.Size = new System.Drawing.Size(106, 17);
            this.chbUseProxy.TabIndex = 10;
            this.chbUseProxy.Text = "Proxy használata";
            this.chbUseProxy.UseVisualStyleBackColor = true;
            this.chbUseProxy.CheckedChanged += new System.EventHandler(this.chbUseProxy_CheckedChanged);
            // 
            // tbPass
            // 
            this.tbPass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPass.Location = new System.Drawing.Point(78, 114);
            this.tbPass.Name = "tbPass";
            this.tbPass.PasswordChar = '●';
            this.tbPass.Size = new System.Drawing.Size(288, 20);
            this.tbPass.TabIndex = 14;
            // 
            // lblPass
            // 
            this.lblPass.AutoSize = true;
            this.lblPass.Location = new System.Drawing.Point(6, 117);
            this.lblPass.Name = "lblPass";
            this.lblPass.Size = new System.Drawing.Size(39, 13);
            this.lblPass.TabIndex = 6;
            this.lblPass.Text = "Jelszó:";
            // 
            // tbUser
            // 
            this.tbUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbUser.Location = new System.Drawing.Point(78, 88);
            this.tbUser.Name = "tbUser";
            this.tbUser.Size = new System.Drawing.Size(288, 20);
            this.tbUser.TabIndex = 13;
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(6, 91);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(66, 13);
            this.lblUser.TabIndex = 4;
            this.lblUser.Text = "Felhasználó:";
            // 
            // tbHost
            // 
            this.tbHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbHost.Location = new System.Drawing.Point(78, 36);
            this.tbHost.Name = "tbHost";
            this.tbHost.Size = new System.Drawing.Size(288, 20);
            this.tbHost.TabIndex = 11;
            this.tbHost.Text = "127.0.0.1";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(6, 64);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(29, 13);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "Port:";
            // 
            // nudPort
            // 
            this.nudPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nudPort.Location = new System.Drawing.Point(78, 62);
            this.nudPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nudPort.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudPort.Name = "nudPort";
            this.nudPort.Size = new System.Drawing.Size(288, 20);
            this.nudPort.TabIndex = 12;
            this.nudPort.Value = new decimal(new int[] {
            8080,
            0,
            0,
            0});
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(6, 39);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(57, 13);
            this.lblHost.TabIndex = 0;
            this.lblHost.Text = "Kiszolgáló:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(313, 186);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Mégse";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(232, 186);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "Mentés";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 221);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tabSettings);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(420, 260);
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Beállítások";
            this.tabSettings.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.tabRecord.ResumeLayout(false);
            this.tabRecord.PerformLayout();
            this.tabProxy.ResumeLayout(false);
            this.tabProxy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TabControl tabSettings;
        private TabPage tabProxy;
        private TextBox tbHost;
        private Label lblPort;
        private NumericUpDown nudPort;
        private Label lblHost;
        private Button btnCancel;
        private Button btnSave;
        private TextBox tbPass;
        private Label lblPass;
        private TextBox tbUser;
        private Label lblUser;
        private CheckBox chbUseProxy;
        private TabPage tabGeneral;
        private CheckBox chbAlwaysOnTop;
        private CheckBox chbShowPopup;
        private CheckBox chbMinimizeToTray;
        private CheckBox chbRunAtStartUp;
        private CheckBox chbCheckUpdates;
        private CheckBox chbNotifyProgram;
        private TabPage tabRecord;
        private PlaceholderTextBox tbRecordPath;
        private Button btnRecordPathBrowse;
        private CheckBox chbAfterRecordOpenFolder;
        private FolderBrowserDialog fbdRecordPath;
    }
}