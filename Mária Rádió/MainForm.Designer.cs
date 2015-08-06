using System.ComponentModel;
using System.Windows.Forms;
using Maria_Radio.Controls;

namespace Maria_Radio
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblTimer = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.volume = new System.Windows.Forms.TrackBar();
            this.lblProgram = new System.Windows.Forms.Label();
            this.notify = new System.Windows.Forms.NotifyIcon(this.components);
            this.lblHeader = new System.Windows.Forms.Label();
            this.timerRecording = new System.Windows.Forms.Timer(this.components);
            this.lblRecording = new System.Windows.Forms.Label();
            this.menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.miAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProgram = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTime = new System.Windows.Forms.Label();
            this.timerTime = new System.Windows.Forms.Timer(this.components);
            this.timerNotify = new System.Windows.Forms.Timer(this.components);
            this.cbMountPoints = new System.Windows.Forms.ComboBox();
            this.btnShowHidePrograms = new Maria_Radio.Controls.ImageButton();
            this.ibtnConfig = new Maria_Radio.Controls.ImageButton();
            this.ibtnMinimize = new Maria_Radio.Controls.ImageButton();
            this.ibtnX = new Maria_Radio.Controls.ImageButton();
            this.ibtnRecord = new Maria_Radio.Controls.ImageButton();
            this.ibtnPlay = new Maria_Radio.Controls.ImageButton();
            ((System.ComponentModel.ISupportInitialize)(this.volume)).BeginInit();
            this.menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(240)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblTitle.Location = new System.Drawing.Point(92, 42);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(428, 18);
            this.lblTitle.TabIndex = 2;
            // 
            // lblTimer
            // 
            this.lblTimer.AutoSize = true;
            this.lblTimer.BackColor = System.Drawing.Color.Transparent;
            this.lblTimer.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(240)));
            this.lblTimer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblTimer.Location = new System.Drawing.Point(92, 98);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(86, 18);
            this.lblTimer.TabIndex = 4;
            this.lblTimer.Text = "00 : 00 : 00";
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Interval = 60000;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // volume
            // 
            this.volume.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.volume.Cursor = System.Windows.Forms.Cursors.Hand;
            this.volume.LargeChange = 10;
            this.volume.Location = new System.Drawing.Point(120, 125);
            this.volume.Maximum = 100;
            this.volume.Name = "volume";
            this.volume.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.volume.Size = new System.Drawing.Size(400, 45);
            this.volume.TabIndex = 6;
            this.volume.TickFrequency = 5;
            this.volume.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.volume.Scroll += new System.EventHandler(this.volume_Scroll);
            // 
            // lblProgram
            // 
            this.lblProgram.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProgram.BackColor = System.Drawing.Color.Transparent;
            this.lblProgram.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(240)));
            this.lblProgram.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblProgram.Location = new System.Drawing.Point(92, 69);
            this.lblProgram.Name = "lblProgram";
            this.lblProgram.Size = new System.Drawing.Size(428, 18);
            this.lblProgram.TabIndex = 7;
            // 
            // notify
            // 
            this.notify.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notify.BalloonTipText = "Kattintson a Mária Rádió ikonjára a megjelenítéshez!";
            this.notify.BalloonTipTitle = "Mária Rádió";
            this.notify.Icon = ((System.Drawing.Icon)(resources.GetObject("notify.Icon")));
            this.notify.BalloonTipClicked += new System.EventHandler(this.notify_BalloonTipClicked);
            this.notify.BalloonTipClosed += new System.EventHandler(this.notify_BalloonTipClosed);
            this.notify.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notify_MouseClick);
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.BackColor = System.Drawing.Color.Transparent;
            this.lblHeader.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblHeader.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(240)));
            this.lblHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblHeader.Location = new System.Drawing.Point(8, 9);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(266, 19);
            this.lblHeader.TabIndex = 10;
            this.lblHeader.Text = "Mária Rádió :: www.mariaradio.hu";
            this.lblHeader.Click += new System.EventHandler(this.lblHeader_Click);
            // 
            // timerRecording
            // 
            this.timerRecording.Interval = 1000;
            this.timerRecording.Tick += new System.EventHandler(this.timerRecording_Tick);
            // 
            // lblRecording
            // 
            this.lblRecording.AutoSize = true;
            this.lblRecording.BackColor = System.Drawing.Color.Transparent;
            this.lblRecording.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblRecording.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(240)));
            this.lblRecording.ForeColor = System.Drawing.Color.Red;
            this.lblRecording.Location = new System.Drawing.Point(194, 98);
            this.lblRecording.Name = "lblRecording";
            this.lblRecording.Size = new System.Drawing.Size(217, 18);
            this.lblRecording.TabIndex = 11;
            this.lblRecording.Text = "Felvétel:  00 : 00 : 00  |  0.00 B";
            this.lblRecording.Visible = false;
            this.lblRecording.Click += new System.EventHandler(this.lblRecording_Click);
            // 
            // menu
            // 
            this.menu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSettings,
            this.miAbout});
            this.menu.Name = "contextMenuStrip1";
            this.menu.Size = new System.Drawing.Size(203, 48);
            // 
            // miSettings
            // 
            this.miSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.miSettings.Image = global::Maria_Radio.Properties.Resources.cog;
            this.miSettings.Name = "miSettings";
            this.miSettings.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.miSettings.Size = new System.Drawing.Size(202, 22);
            this.miSettings.Text = "Beállítások";
            this.miSettings.Click += new System.EventHandler(this.miSettings_Click);
            // 
            // miAbout
            // 
            this.miAbout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.miAbout.Image = global::Maria_Radio.Properties.Resources.user_suit;
            this.miAbout.Name = "miAbout";
            this.miAbout.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.miAbout.Size = new System.Drawing.Size(202, 22);
            this.miAbout.Text = "Névjegy";
            this.miAbout.Click += new System.EventHandler(this.miAbout_Click);
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.Description = "Válassza ki azt a mappát, ahová szeretné menteni a Mária Rádióból felvett műsorok" +
    "at.";
            // 
            // pbLogo
            // 
            this.pbLogo.BackColor = System.Drawing.Color.Transparent;
            this.pbLogo.Image = ((System.Drawing.Image)(resources.GetObject("pbLogo.Image")));
            this.pbLogo.Location = new System.Drawing.Point(12, 42);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new System.Drawing.Size(74, 74);
            this.pbLogo.TabIndex = 1;
            this.pbLogo.TabStop = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Highlight;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.ForeColor = System.Drawing.SystemColors.Highlight;
            this.label1.Location = new System.Drawing.Point(10, 191);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(511, 2);
            this.label1.TabIndex = 13;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.ColumnHeadersVisible = false;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTime,
            this.colProgram});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.dataGridView.Location = new System.Drawing.Point(12, 208);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.dataGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.dataGridView.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.dataGridView.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dataGridView.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.dataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(508, 223);
            this.dataGridView.TabIndex = 14;
            // 
            // colTime
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colTime.DefaultCellStyle = dataGridViewCellStyle1;
            this.colTime.HeaderText = "Idő";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            this.colTime.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // colProgram
            // 
            this.colProgram.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.colProgram.DefaultCellStyle = dataGridViewCellStyle2;
            this.colProgram.HeaderText = "Műsor";
            this.colProgram.Name = "colProgram";
            this.colProgram.ReadOnly = true;
            this.colProgram.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.BackColor = System.Drawing.Color.Transparent;
            this.lblTime.Font = new System.Drawing.Font("Arial", 10F);
            this.lblTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.lblTime.Location = new System.Drawing.Point(323, 11);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(110, 16);
            this.lblTime.TabIndex = 15;
            this.lblTime.Text = "Jan. 1. 00:00:00";
            this.lblTime.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblTime_MouseDown);
            // 
            // timerTime
            // 
            this.timerTime.Enabled = true;
            this.timerTime.Interval = 1000;
            this.timerTime.Tick += new System.EventHandler(this.timerTime_Tick);
            // 
            // timerNotify
            // 
            this.timerNotify.Interval = 1000;
            this.timerNotify.Tick += new System.EventHandler(this.timerNotify_Tick);
            // 
            // cbMountPoints
            // 
            this.cbMountPoints.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.cbMountPoints.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMountPoints.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
            this.cbMountPoints.FormattingEnabled = true;
            this.cbMountPoints.Location = new System.Drawing.Point(197, 65);
            this.cbMountPoints.Name = "cbMountPoints";
            this.cbMountPoints.Size = new System.Drawing.Size(300, 21);
            this.cbMountPoints.TabIndex = 17;
            // 
            // btnShowHidePrograms
            // 
            this.btnShowHidePrograms.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowHidePrograms.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.btnShowHidePrograms.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShowHidePrograms.HoverImage = ((System.Drawing.Image)(resources.GetObject("btnShowHidePrograms.HoverImage")));
            this.btnShowHidePrograms.Location = new System.Drawing.Point(490, 167);
            this.btnShowHidePrograms.Name = "btnShowHidePrograms";
            this.btnShowHidePrograms.NormalImage = ((System.Drawing.Image)(resources.GetObject("btnShowHidePrograms.NormalImage")));
            this.btnShowHidePrograms.PushedImage = ((System.Drawing.Image)(resources.GetObject("btnShowHidePrograms.PushedImage")));
            this.btnShowHidePrograms.Size = new System.Drawing.Size(24, 16);
            this.btnShowHidePrograms.TabIndex = 16;
            this.btnShowHidePrograms.Click += new System.EventHandler(this.btnShowHidePrograms_Click);
            // 
            // ibtnConfig
            // 
            this.ibtnConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ibtnConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.ibtnConfig.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ibtnConfig.ForeColor = System.Drawing.Color.Transparent;
            this.ibtnConfig.HoverImage = ((System.Drawing.Image)(resources.GetObject("ibtnConfig.HoverImage")));
            this.ibtnConfig.Location = new System.Drawing.Point(448, 7);
            this.ibtnConfig.Name = "ibtnConfig";
            this.ibtnConfig.NormalImage = global::Maria_Radio.Properties.Resources.config;
            this.ibtnConfig.PushedImage = global::Maria_Radio.Properties.Resources.config_p;
            this.ibtnConfig.Size = new System.Drawing.Size(24, 24);
            this.ibtnConfig.TabIndex = 12;
            this.ibtnConfig.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ibtnConfig_MouseClick);
            // 
            // ibtnMinimize
            // 
            this.ibtnMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ibtnMinimize.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.ibtnMinimize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ibtnMinimize.ForeColor = System.Drawing.Color.Transparent;
            this.ibtnMinimize.HoverImage = global::Maria_Radio.Properties.Resources.minimize_h;
            this.ibtnMinimize.Location = new System.Drawing.Point(474, 6);
            this.ibtnMinimize.Name = "ibtnMinimize";
            this.ibtnMinimize.NormalImage = global::Maria_Radio.Properties.Resources.minimize;
            this.ibtnMinimize.PushedImage = global::Maria_Radio.Properties.Resources.minimize_p;
            this.ibtnMinimize.Size = new System.Drawing.Size(24, 24);
            this.ibtnMinimize.TabIndex = 9;
            this.ibtnMinimize.Click += new System.EventHandler(this.ibtnMinimize_Click);
            // 
            // ibtnX
            // 
            this.ibtnX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ibtnX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.ibtnX.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ibtnX.ForeColor = System.Drawing.Color.Transparent;
            this.ibtnX.HoverImage = global::Maria_Radio.Properties.Resources.x_h;
            this.ibtnX.Location = new System.Drawing.Point(500, 7);
            this.ibtnX.Name = "ibtnX";
            this.ibtnX.NormalImage = global::Maria_Radio.Properties.Resources.x;
            this.ibtnX.PushedImage = global::Maria_Radio.Properties.Resources.x_p;
            this.ibtnX.Size = new System.Drawing.Size(24, 24);
            this.ibtnX.TabIndex = 8;
            this.ibtnX.Click += new System.EventHandler(this.ibtnX_Click);
            // 
            // ibtnRecord
            // 
            this.ibtnRecord.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.ibtnRecord.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ibtnRecord.DisabledImage = global::Maria_Radio.Properties.Resources.record_d;
            this.ibtnRecord.ForeColor = System.Drawing.Color.Transparent;
            this.ibtnRecord.HoverImage = ((System.Drawing.Image)(resources.GetObject("ibtnRecord.HoverImage")));
            this.ibtnRecord.Location = new System.Drawing.Point(66, 125);
            this.ibtnRecord.Name = "ibtnRecord";
            this.ibtnRecord.NormalImage = global::Maria_Radio.Properties.Resources.record;
            this.ibtnRecord.PushedImage = ((System.Drawing.Image)(resources.GetObject("ibtnRecord.PushedImage")));
            this.ibtnRecord.Size = new System.Drawing.Size(48, 48);
            this.ibtnRecord.TabIndex = 5;
            this.ibtnRecord.Click += new System.EventHandler(this.ibtnRecord_Click);
            // 
            // ibtnPlay
            // 
            this.ibtnPlay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.ibtnPlay.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ibtnPlay.DisabledImage = global::Maria_Radio.Properties.Resources.play_d;
            this.ibtnPlay.ForeColor = System.Drawing.Color.Transparent;
            this.ibtnPlay.HoverImage = global::Maria_Radio.Properties.Resources.play_h;
            this.ibtnPlay.Location = new System.Drawing.Point(12, 125);
            this.ibtnPlay.Name = "ibtnPlay";
            this.ibtnPlay.NormalImage = global::Maria_Radio.Properties.Resources.play;
            this.ibtnPlay.PushedImage = global::Maria_Radio.Properties.Resources.play_p;
            this.ibtnPlay.Size = new System.Drawing.Size(48, 48);
            this.ibtnPlay.TabIndex = 0;
            this.ibtnPlay.Click += new System.EventHandler(this.ibtnPlay_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
            this.ClientSize = new System.Drawing.Size(532, 186);
            this.Controls.Add(this.cbMountPoints);
            this.Controls.Add(this.btnShowHidePrograms);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblRecording);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ibtnConfig);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.ibtnMinimize);
            this.Controls.Add(this.ibtnX);
            this.Controls.Add(this.lblProgram);
            this.Controls.Add(this.volume);
            this.Controls.Add(this.ibtnRecord);
            this.Controls.Add(this.lblTimer);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pbLogo);
            this.Controls.Add(this.ibtnPlay);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.ForeColor = System.Drawing.Color.Transparent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Opacity = 0.9D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mária Rádió";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.LocationChanged += new System.EventHandler(this.MainForm_LocationChanged);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.volume)).EndInit();
            this.menu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ImageButton ibtnPlay;
        private PictureBox pbLogo;
        private Label lblTitle;
        private Label lblTimer;
        private Timer timer;
        private Timer timerUpdate;
        private ImageButton ibtnRecord;
        private TrackBar volume;
        private Label lblProgram;
        private ImageButton ibtnX;
        private ImageButton ibtnMinimize;
        private NotifyIcon notify;
        private Label lblHeader;
        private Timer timerRecording;
        private Label lblRecording;
        private ImageButton ibtnConfig;
        private ContextMenuStrip menu;
        private ToolStripMenuItem miAbout;
        private FolderBrowserDialog folderBrowserDialog;
        private Label label1;
        private DataGridView dataGridView;
        private DataGridViewTextBoxColumn colTime;
        private DataGridViewTextBoxColumn colProgram;
        private Label lblTime;
        private Timer timerTime;
        private Timer timerNotify;
        private ToolStripMenuItem miSettings;
        private ImageButton btnShowHidePrograms;
        private ComboBox cbMountPoints;
    }
}

