using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using HtmlAgilityPack;
using Maria_Radio.Properties;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Microsoft.WindowsAPICodePack.Taskbar;
using WMPLib;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using skProx = Maria_Radio.Misc.SettingKeys.Proxy;
using skGen = Maria_Radio.Misc.SettingKeys.General;
using skRec = Maria_Radio.Misc.SettingKeys.Record;
using skProg = Maria_Radio.Misc.SettingKeys.Program;
using Settings = Maria_Radio.Misc.Settings;

namespace Maria_Radio
{
    public partial class MainForm : Form
    {
        private WindowsMediaPlayer player;
        private Thread tRecord,tUpdate, tUpdateCheck;
        private bool recording;
        private bool listening;
        private bool lighted;
        private DateTime listen;
        private string streamURL;
        private string filename;
        private string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "\\";
        private static MainForm mainform;
        private bool initialized;
        private int bitrate = 128000;
        private int notifyCounter;
        private Settings settings = Program.settings;
        private List<MountPoint> mountPoints;

        private ToolTip ttRecording, ttVolume, ttTitle, ttProgram;
        private ThumbnailToolbarButton btnPlay, btnRecord;

        private const int WS_MINIMIZEBOX = 0x20000;
        private const int CS_DBLCLKS = 0x8;
        
        private delegate void RecordAbortedDelegate(string message);
        private delegate void UpdateStatSuccessDelegate(XmlDocument doc, bool bThread);
        private delegate void UpdateStatFailDelegate();
        private delegate void StartUpdaterDelegate(string PopupText);
        private delegate void UpdateInterfaceByNetworkDelegate(bool hasNetwork);
        private delegate void GotHttpContent(string content);

        private const int DEF_VOLUME = 100;

        public static MainForm GetInstance()
        {
            if (mainform == null)
            {
                mainform = new MainForm();
            }
            return mainform;
        }

        private MainForm()
        {

            InitializeComponent();
            SynchronizeSWC();

            settings.load();
            settings.onError += settings_onError;

            if (settings.getValue(skProx.UseProxy, false))
            {
                WebProxy proxy = new WebProxy();

                proxy.Address = new Uri(
                    "http://" + settings.getValue(skProx.Host, "127.0.0.1") +
                    ":" + settings.getValue<Decimal>(skProx.Port, 8080)
                );

                proxy.Credentials = new NetworkCredential(
                    settings.getValue(skProx.User,""),
                    settings.getValue(skProx.Pass, "")
                );

                HttpWebRequest.DefaultWebProxy = proxy;
            }

            int vol = settings.getValue(skProg.Volume, DEF_VOLUME);

            volume.Value = vol;

            ttVolume = new ToolTip();
            ttVolume.SetToolTip(volume, "Hang [" + vol + "%]");

            if (HasNetworkConnection())
                UpdateStatNoThread();
            else
            {
                //MessageBox.Show(SplashForm.GetInstance(), "Nincs internet kapcsolat!", Program.NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                notify.Visible = true;
                notify.BalloonTipIcon = ToolTipIcon.Error;
                notify.BalloonTipTitle = Program.NAME;
                notify.BalloonTipText = "Nincs internet kapcsolat!";
                notify.ShowBalloonTip(0);
                UpdateInterfaceByNetwork(false);
            }

            player = new WindowsMediaPlayer();
            player.settings.volume = vol;
            player.settings.autoStart = false;
            player.URL = streamURL;
            player.Buffering += player_Buffering;
            player.PlayStateChange += player_PlayStateChange;

            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;

            GetHttpContent("http://mariaradio.hu:8000/status.xsl", SetMountPointList);
        }

        private void settings_onError(Exception e)
        {
            MessageBox.Show(this, e.Message + "\r\n\r\n" + e.StackTrace, e.GetType().FullName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SynchronizeSWC()
        {
            lblHeader.Text = Program.NAME + " :: " + Program.WEBPAGE;
            Text = Program.NAME;

            (new ToolTip()).SetToolTip(ibtnRecord, "Felvétel / Felvétel leállítása (Ctrl + R)");
            (new ToolTip()).SetToolTip(ibtnPlay, "Hallgatás / Szüneteltetés (Space)");

            (new ToolTip()).SetToolTip(ibtnConfig, "Beállítások");
            (new ToolTip()).SetToolTip(ibtnMinimize, "Lekicsinyítés a tálcára");
            (new ToolTip()).SetToolTip(ibtnX, "Bezárás (Alt + F4)");

            (new ToolTip()).SetToolTip(lblHeader, "Mária Rádió honlapjának megnyitása");
            (new ToolTip()).SetToolTip(lblTime, "Jelenlegi idő");

            (new ToolTip()).SetToolTip(lblTimer, "Hallgatási idő");
            (new ToolTip()).SetToolTip(btnShowHidePrograms, "Programok megjelenítése/elrejtése");

            settings.load();

            string recordPath = settings.getValue(skRec.RecordPath, "");

            if (Directory.Exists(recordPath))
            {
                dir = (recordPath.Equals(String.Empty) ? Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "\\" : recordPath);
            }

            if (settings.getValue(skProg.ShowPrograms, false))
            {
                Size = new Size(Size.Width, 443);

                btnShowHidePrograms.HoverImage = Resources.up_16;
                btnShowHidePrograms.NormalImage = Resources.up_16;
                btnShowHidePrograms.PushedImage = Resources.up_16;
            }
            else
            {
                Size = new Size(Size.Width, 186);

                btnShowHidePrograms.HoverImage = Resources.down_16;
                btnShowHidePrograms.NormalImage = Resources.down_16;
                btnShowHidePrograms.PushedImage = Resources.down_16;
            }

            TopMost = settings.getValue(skGen.AlwaysOnTop, false);
        }

        private void SetPlayButtons()
        {
            ibtnPlay.NormalImage = Resources.pause;
            ibtnPlay.HoverImage = Resources.pause_h;
            ibtnPlay.PushedImage = Resources.pause_p;

            if (TaskbarManager.IsPlatformSupported)
            {
                btnPlay.Icon = Resources.ipause;
                btnPlay.Tooltip = "Szüneteltetés";
            }
        }

        private void SetPauseButtons()
        {
            ibtnPlay.NormalImage = Resources.play;
            ibtnPlay.HoverImage = Resources.play_h;
            ibtnPlay.PushedImage = Resources.play_p;
            if (TaskbarManager.IsPlatformSupported)
            {
                btnPlay.Icon = Resources.iplay;
                btnPlay.Tooltip = "Hallgatás";
            }
        }

        private void Play()
        {
            SetPlayButtons();

            lblTimer.Text = "Pufferelés... 0%"; ;
            player.controls.play();
        }

        private void Pause()
        {
            SetPauseButtons();
            listening = false;
            player.controls.stop();
            player.close();

            player = new WindowsMediaPlayer();
            player.settings.volume = settings.getValue(skProg.Volume, DEF_VOLUME);
            player.settings.autoStart = false;
            player.URL = streamURL;
            player.Buffering += player_Buffering;
            player.PlayStateChange += player_PlayStateChange;

            //player.newMedia(this.streamURL);
            timer.Enabled = false;
            lblTimer.Text = FormatSeconds(0);
            UpdateTaskBarIcon();
        }

        private void PlayPause()
        {
            if (player != null && listening)
                Pause();
            else
                Play();
        }

        private void Record()
        {
            if (!recording)
            {
                recording = true;
                if (TaskbarManager.IsPlatformSupported)
                {
                    btnRecord.Icon = Resources.irecord_e;
                    btnRecord.Tooltip = "Felvétel leállítása";
                }
                tRecord = new Thread(RecordThread);
                tRecord.Start();
                timerRecording.Enabled = true;
                lblRecording.Visible = true;
            }
            else
            {
                StopRecord();
                if (settings.getValue(skRec.AfterRecordOpenFolder, true))
                {
                    Process.Start("explorer.exe", @"/select, " + dir + filename + ".mp3");
                }
            }
            UpdateTaskBarIcon();
        }

        private void StopRecord()
        {
            recording = false;
            btnRecord.Icon = Resources.irecord;
            btnRecord.Tooltip = "Felvétel";
            ibtnRecord.NormalImage = Resources.record;
            timerRecording.Enabled = false;
            lblRecording.Visible = false;
            lblRecording.Text = "Felvétel:  00 : 00 : 00  |  0.00 B";
        }

        private void SetMountPointList(string content)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            //<div class="newscontent">
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='roundcont']");

            mountPoints = new List<MountPoint>();

            foreach (HtmlNode node in nodes)
            {

                MessageBox.Show(node.InnerHtml);

                HtmlNodeCollection dataNodes = node.SelectNodes(".//td[@class='streamdata']");

                if (dataNodes == null)
                {
                    MessageBox.Show("is null");
                }
                else
                {
                    MessageBox.Show(dataNodes.Count.ToString());
                }
                
                
                mountPoints.Add( 
                    new MountPoint(
                        dataNodes[MountPoint.INDEX_TITLE].InnerText,
                        dataNodes[MountPoint.INDEX_DESCRIPTION].InnerText,
                        dataNodes[MountPoint.INDEX_CONTENT_TYPE].InnerText,
                        DateTime.Parse(dataNodes[MountPoint.INDEX_UPTIME].InnerText),
                        int.Parse(dataNodes[MountPoint.INDEX_BITRATE].InnerText),
                        int.Parse(dataNodes[MountPoint.INDEX_CURRENTLISTENERS].InnerText),
                        int.Parse(dataNodes[MountPoint.INDEX_PEAKLISTENERS].InnerText),
                        dataNodes[MountPoint.INDEX_GENRE].InnerText,
                        dataNodes[MountPoint.INDEX_URL].InnerText,
                        dataNodes[MountPoint.INDEX_CURRENT_SONG].InnerText
                    )
                );
            }


            foreach (MountPoint mountPoint in mountPoints)
            {
                cbMountPoints.Items.Add(mountPoint.ToString());
            }
        }

        private void GetHttpContent(string url, GotHttpContent method)
        {
            new Thread(delegate ()
            {
                string response = null;
                    
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(res.GetResponseStream()))
                    {
                        response = reader.ReadToEnd();
                    }
                }

                Invoke(method, response);
            }
            ).Start();
        }

        //ide lehet proxy cucc
        private void RecordThread()
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            int metaInt = 0;
            int count = 0;
            int metadataLength = 0;

            byte[] buffer = new byte[512];

            Stream socketStream = null;
            Stream byteOut = null;

            request = (HttpWebRequest)WebRequest.Create(streamURL);

            request.Headers.Clear();
            request.Method = "GET";
            request.Accept = "*/*";
            request.Headers.Add("Icy-MetaData", "1");
            request.UserAgent = Program.USER_AGENT;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                Invoke(new RecordAbortedDelegate(RecordAborted), ex.Message);
                recording = false;
                return;
            }

            if (!response.StatusCode.ToString().Equals("OK"))
            {
                Invoke(new RecordAbortedDelegate(RecordAborted), "Hibás válasz a szerver felől! (" + response.StatusCode + ")");
                recording = false;
                return;
            }

            metaInt = Convert.ToInt32(response.GetResponseHeader("icy-metaint"));
            filename = "maria_radio_" + DateTime.Now.Year + "-" + (DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month : DateTime.Now.Month.ToString()) + "-" + (DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day : DateTime.Now.Day.ToString()) + "_" + (DateTime.Now.Hour < 10 ? "0" + DateTime.Now.Hour : DateTime.Now.Hour.ToString()) + "-" + (DateTime.Now.Minute < 10 ? "0" + DateTime.Now.Minute : DateTime.Now.Minute.ToString()) + "-" + (DateTime.Now.Second < 10 ? "0" + DateTime.Now.Second : DateTime.Now.Second.ToString());
            try
            {
                socketStream = response.GetResponseStream();
                byteOut = CreateFile(dir, filename);

                while (recording)
                {
                    int bufLen = socketStream.Read(buffer, 0, buffer.Length);
                    if (bufLen < 0)
                        return;

                    for (int i = 0; i < bufLen; i++)
                    {
                        if (metadataLength != 0)
                        {
                            metadataLength--;
                        }
                        else
                        {
                            if (count++ < metaInt)
                            {
                                if (byteOut != null)
                                {
                                    byteOut.Write(buffer, i, 1);
                                    if (count % 100 == 0)
                                        byteOut.Flush();
                                }
                            }
                            else
                            {
                                metadataLength = Convert.ToInt32(buffer[i]) * 16;
                                count = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Invoke(new RecordAbortedDelegate(RecordAborted), ex.Message);
                    recording = false;
                }
                catch (Exception) { }
            }
            finally
            {
                if (byteOut != null)
                    byteOut.Close();
                if (socketStream != null)
                    socketStream.Close();
            }
        }

        private void RecordAborted(string message)
        {
            StopRecord();
            MessageBox.Show(this, message, Program.NAME + " felvevő", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void UpdateTaskBarIcon()
        {
            if(!TaskbarManager.IsPlatformSupported) return;

            if(recording && listening)
                TaskbarManager.Instance.SetOverlayIcon(Resources.pr, "Hallgatás / Felvétel");
            else if (recording && !listening)
                TaskbarManager.Instance.SetOverlayIcon(Resources.irecord, "Felvétel");
            else if(!recording && listening)
                TaskbarManager.Instance.SetOverlayIcon(Resources.iplay, "Hallgatás");
            else
                TaskbarManager.Instance.SetOverlayIcon(null,"");
        }

        private void UpdateStatSuccess(XmlDocument XMLDoc, bool bThread)
        {
            try
            {
                XmlElement doc = XMLDoc.DocumentElement;
                bool notifyText = false;

                notify.BalloonTipIcon = ToolTipIcon.Info;
                notify.Text = Program.NAME;

                lblTitle.ForeColor = Color.FromArgb(40, 153, 255);

                bool notifyProgram = settings.getValue(skGen.NotifyProgram, true);

                foreach (XmlElement el in doc.ChildNodes)
                {
                    if (el.Name == "title")
                    {
                        if (notifyProgram && lblTitle.Text != el.InnerText)
                        {
                            notifyText = true;
                        }

                        lblTitle.Text = el.InnerText;
                        if (ttTitle == null)
                            ttTitle = new ToolTip();
                        ttTitle.SetToolTip(lblTitle, el.InnerText);
                    }
                    else if (el.Name == "program")
                    {
                        if (notifyProgram && lblProgram.Text != el.InnerText && !el.InnerText.Equals(string.Empty))
                        {
                            notifyText = true;
                        }

                        lblProgram.Text = el.InnerText;
                        if (ttProgram == null)
                            ttProgram = new ToolTip();
                        ttProgram.SetToolTip(lblProgram, el.InnerText);
                    }
                    else if (el.Name == "streamurl")
                    {
                        streamURL = el.InnerText;
                    }
                    else if (el.Name == "updatefreq")
                    {
                        try
                        {
                            timerUpdate.Interval = int.Parse(el.InnerText);
                        }
                        catch (Exception)
                        {
                            timerUpdate.Interval = 60000;
                        }
                    }
                    else if (el.Name == "bitrate")
                    {
                        try
                        {
                            bitrate = int.Parse(el.InnerText);
                        }
                        catch (Exception)
                        {
                            timerUpdate.Interval = 128000;
                        }
                    }
                    else if (el.Name == "programs")
                    {
                        dataGridView.Rows.Clear();
                        if (el.HasChildNodes)
                        {
                            foreach (XmlElement prog in el.ChildNodes)
                            {
                                int row = dataGridView.Rows.Add(prog.Attributes["time"].Value, prog.Attributes["title"].Value + (!prog.Attributes["program"].Value.Equals(string.Empty) ? " - " + prog.Attributes["program"].Value : string.Empty));
                                if (prog.HasAttribute("current"))
                                {
                                    dataGridView.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(8, 16, 8);
                                    dataGridView.Rows[row].DefaultCellStyle.ForeColor = Color.FromArgb(55, 194, 55);

                                    dataGridView.Rows[row].DefaultCellStyle.SelectionBackColor = Color.FromArgb(16, 32, 16);
                                    dataGridView.Rows[row].DefaultCellStyle.SelectionForeColor = Color.FromArgb(55, 194, 55);
                                }
                            }
                        }
                    }
                }

                ibtnPlay.Enabled = true;
                ibtnRecord.Enabled = true;
                timerUpdate.Enabled = true;

                string nt = Program.NAME;
                if (!lblTitle.Text.Equals(string.Empty))
                {
                    nt += " :: " + lblTitle.Text;
                    if (!lblProgram.Text.Equals(string.Empty))
                    {
                        nt += " - " + lblProgram.Text;
                    }
                }
                else if (!lblProgram.Text.Equals(string.Empty))
                {
                    nt += " :: " + lblProgram.Text;
                }

                if (nt.Length > 63)
                {
                    notify.Text = nt.Substring(0, 60) + "...";
                }
                else
                {
                    notify.Text = nt;
                }

                if (bThread && settings.getValue(skGen.NotifyProgram, true) && notifyText)
                {
                    notify.BalloonTipTitle = Program.NAME + " :: Műsor";
                    notify.BalloonTipText = lblTitle.Text + (!lblProgram.Text.Equals(string.Empty) ? "\r\n" + lblProgram.Text : "");
                    notify.Visible = true;
                    notify.ShowBalloonTip(0);
                }
            }
            catch (Exception)
            {
                UpdateStatFail();
            }
        }

        private void UpdateStatFail()
        {
            //MessageBox.Show(this, "Hiba történt az adatok lekérdezése közben!\r\nKérem ellenőrizze az internet kapcsolatát.", Program.NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);

            notify.Visible = true;
            notify.BalloonTipIcon = ToolTipIcon.Error;
            notify.BalloonTipTitle = Program.NAME;
            notify.BalloonTipText = "Hiba történt az adatok lekérdezése közben!";
            notify.ShowBalloonTip(0);
   
            if (this == null || IsDisposed || streamURL == null || streamURL.Equals(string.Empty))
                UpdateInterfaceByNetwork(false);
                //    Process.GetCurrentProcess().Kill();
        }

        private void UpdateStatNoThread(){
            try
            {
                XmlDocument XMLDoc = new XmlDocument();
                XMLDoc.Load(Program.STAT_URL);
                UpdateStatSuccess(XMLDoc, false);
            }
            catch (Exception)
            {
                UpdateStatFail();
            }
        }

        private void UpdateStatThread()
        {
            try
            {
                XmlDocument XMLDoc = new XmlDocument();
                XMLDoc.Load(Program.STAT_URL);
                Invoke(new UpdateStatSuccessDelegate(UpdateStatSuccess), XMLDoc, true);
            }
            catch (Exception) {
                try
                {
                    Invoke(new UpdateStatFailDelegate(UpdateStatFail));
                }
                catch (Exception) { }
            }
        }

        private void UpdateStat()
        {
            tUpdate = new Thread(UpdateStatThread);
            tUpdate.Start();
        }

        private void CheckForUpdate()
        {
            tUpdateCheck = new Thread(CheckForUpdateThread);
            tUpdateCheck.Start();
        }

        private void CheckForUpdateThread()
        {
            bool updatable = false;
            XmlDocument XMLDoc = new XmlDocument();
            try
            {
                XMLDoc.Load(Program.VERSION_CHECK_URL + Program.VERSION);
            }
            catch (Exception)
            {
                return;
            }
            XmlElement doc = XMLDoc.DocumentElement;

            string newVersion = "";

            foreach (XmlElement el in doc.ChildNodes)
            {
                if (el.Name == "latestversion")
                {
                    newVersion = el.InnerText;
                }
                if (el.Name == "updateavailable" && el.InnerText == "true")
                {
                    updatable = true;
                }
            }

            if (updatable)
            {
                Invoke(new StartUpdaterDelegate(StartUpdaterDeleg), "Újabb verziója jelent meg a " + Program.NAME + " programnak!\nÖn verziója: v" + Program.VERSION + "\nÚj verzió: v" + newVersion + "\n\nFrissítsük a " + Program.NAME + " programot?\n(A frissítés alatt a " + Program.NAME + " program bezárul.)");
                
            }
        }

        private void StartUpdaterDeleg(string PopupText)
        {
            if (MessageBox.Show(this, PopupText, Program.NAME + " :: Frissítő", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                StartUpdater();
            }
        }

        private void StartUpdater()
        {
            try
            {
                Process.Start("Updater.exe", Program.VERSION);
                Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(null, "A következő hiba lépett fel a frissítő program indításakor:\r\n" + e.Message, Program.NAME, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void ShowForm()
        {
            notify.Visible = false;
            timerNotify.Enabled = false;
            notifyCounter = 0;
            notify.Icon = ((Icon)((new ComponentResourceManager(typeof(MainForm))).GetObject("notify.Icon")));
            Show();
        }

        private void Minimize()
        {
            if (settings.getValue(skGen.MinimizeToTray, true))
            {
                Hide();
                notify.BalloonTipIcon = ToolTipIcon.Info;
                notify.BalloonTipTitle = Program.NAME;
                notify.BalloonTipText = "Kattintson a Mária Rádió ikonjára a megjelenítéshez!";
                notify.Visible = true;
                if (settings.getValue(skGen.ShowPopup, true))
                    notify.ShowBalloonTip(0);
                timerNotify.Enabled = true;
            }
            else
            {
                WinApi.ShowWindow(Handle, WinApi.SW_SHOWMINIMIZED);
            }
        }

        private void HideNotify()
        {
            if (Visible)
            {
                notify.Visible = false;
            }
        }

        private void UpdateInterfaceByNetwork(bool hasNetwork)
        {
            if (hasNetwork)
            {
                lblTitle.ForeColor = Color.FromArgb(40, 153, 255);
                lblTitle.Text = "Kapcsolódás...";
                UpdateStat();
            }
            else
            {
                if (listening)
                    Pause();
                if (recording)
                    StopRecord();

                lblTitle.ForeColor = Color.FromArgb(255, 0, 0); ;
                lblTitle.Text = "Nincs internet kapcsolat!";

                notify.Visible = true;
                notify.BalloonTipIcon = ToolTipIcon.Error;
                notify.BalloonTipTitle = Program.NAME;
                notify.BalloonTipText = "Nincs internet kapcsolat!";
                notify.ShowBalloonTip(0);

                lblProgram.Text = "";
                ibtnPlay.Enabled = false;
                ibtnRecord.Enabled = false;
                timerUpdate.Enabled = false;
                dataGridView.Rows.Clear();
            }
        }

        //virtual methods

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == SingleInstance.WM_SHOWFIRSTINSTANCE)
            {
                WinApi.ShowToFront(Handle);
                ShowForm();
            }
            base.WndProc(ref message);
        }

        protected override void SetVisibleCore(bool value)
        {
            if (!Program.startMinimized)
                base.SetVisibleCore(value);
            else
            {
                base.SetVisibleCore(true);
                base.SetVisibleCore(false);
            }
            try
            {
                Point pos = settings.getValue(skProg.Positon, new Point(-1000, -1000));

                if (pos.X != -1000 && pos.Y != -1000)
                {
                    Location = pos;
                }

                if (Program.startMinimized)
                {
                    notify.Visible = true;
                    notify.ShowBalloonTip(0);
                    Program.startMinimized = false;
                }

                if (!initialized && settings.getValue(skGen.CheckUpdates, true))
                    CheckForUpdate();

                if(!initialized)
                    initialized = true;
            }
            catch { }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Up)
            {
                volume.Value = (volume.Value + volume.TickFrequency + 1 > volume.Maximum ? volume.Maximum : volume.Value + volume.TickFrequency + 1);
            }
            else if (keyData == Keys.Down)
            {
                volume.Value = (volume.Value - volume.TickFrequency - 1 < volume.Minimum ? volume.Minimum : volume.Value - volume.TickFrequency - 1);
            }
            else if (keyData == (Keys.Control | Keys.U))
            {
                UpdateStat();
            }
            else if (keyData == Keys.Space)
            {
                PlayPause();
            }
            else if (keyData == (Keys.Control | Keys.R))
            {
                Record();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnShown(EventArgs e)
        {
            if (TaskbarManager.IsPlatformSupported)
            {

                btnPlay = new ThumbnailToolbarButton(Resources.iplay, "Hallgatás");
                btnPlay.Click += ibtnPlay_Click;

                btnRecord = new ThumbnailToolbarButton(Resources.irecord, "Felvétel");
                btnRecord.Click += ibtnRecord_Click;

                TaskbarManager.Instance.ThumbnailToolbars.AddButtons(Handle, btnPlay, btnRecord);
            }
            base.OnShown(e);            
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= WS_MINIMIZEBOX;
                cp.ClassStyle |= CS_DBLCLKS;
                return cp;
            }
        }

        //Static
        private static string FormatSeconds(long secs)
        {
            if (secs < 0) secs = 0;
            int hours = (int)(secs / 3600);
            secs %= 3600;
            int mins = (int)(secs / 60);
            secs %= 60;
            return (hours < 10 ? "0" + hours : hours.ToString()) + " : " + (mins < 10 ? "0" + mins : mins.ToString()) + " : " + (secs < 10 ? "0" + secs : secs.ToString());
        }

        private string FormatBytes(long bytes)
        {
            const int scale = 1024;
            string[] orders = { "TB", "GB", "MB", "KB", "B" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.00} {1}", decimal.Divide(bytes, max), order);

                max /= scale;
            }
            return "0.00 B";
        }

        private static Stream CreateFile(String destPath, String filename)
        {
            filename = filename.Replace(":", "");
            filename = filename.Replace("/", "");
            filename = filename.Replace("\\", "");
            filename = filename.Replace("<", "");
            filename = filename.Replace(">", "");
            filename = filename.Replace("|", "");
            filename = filename.Replace("?", "");
            filename = filename.Replace("*", "");
            filename = filename.Replace("\"", "");

            try
            {
                if (!Directory.Exists(destPath))
                    Directory.CreateDirectory(destPath);

                if (!File.Exists(destPath + filename + ".mp3"))
                {
                    return File.Create(destPath + filename + ".mp3");
                }
                for (int i = 1; ; i++)
                {
                    if (!File.Exists(destPath + filename + "(" + i + ").mp3"))
                    {
                        return File.Create(destPath + filename + "(" + i + ").mp3");
                    }
                }
            }
            catch (IOException)
            {
                return null;
            }
        }

        private static string GetMonthName(int month)
        {
            switch (month)
            {
                case 1: return "Jan.";
                case 2: return "Feb.";
                case 3: return "Már.";
                case 4: return "Ápr.";
                case 5: return "Máj.";
                case 6: return "Jún.";
                case 7: return "Júl.";
                case 8: return "Aug.";
                case 9: return "Szep.";
                case 10: return "Okt.";
                case 11: return "Nov.";
                case 12: return "Dec.";
                default: return "";
            }
        }

        private static bool HasNetworkConnection()
        {
            try
            {
                /*IPAddress[] addresslist = */
                Dns.GetHostAddresses(Program.HOST_PING);
                return true;
                /*Ping pingSender = new Ping();
                PingOptions options = new PingOptions();
                options.DontFragment = true;
                byte[] buffer = Encoding.ASCII.GetBytes("MariaRadioMariaRadioMariaRadio00");
                PingReply reply = pingSender.Send(addresslist[0], 1, buffer, options);
                return reply.Status == IPStatus.Success;*/
            }
            catch (Exception e)
            {
                MessageBox.Show(null, e.Message + "\r\n" + e.StackTrace, "Maria Radio", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }
        }

        //EVENTS

        private void ibtnPlay_Click(object sender, EventArgs e)
        {
            PlayPause();
        }

        private void ibtnRecord_Click(object sender, EventArgs e)
        {
            Record();
        }

        private void volume_Scroll(object sender, EventArgs e)
        {
            player.settings.volume = volume.Value;
            settings.setValue(skProg.Volume, volume.Value);
            ttVolume.SetToolTip(volume, "Hang [" + settings.getValue(skProg.Volume, DEF_VOLUME) + "%]");
            settings.save();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            lblTimer.Text = FormatSeconds((long)(DateTime.Now - listen).TotalSeconds);
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            UpdateStat();
        }

        private void ibtnX_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ibtnMinimize_Click(object sender, EventArgs e)
        {
            Minimize();
        }

        private void notify_MouseClick(object sender, MouseEventArgs e)
        {
            ShowForm();
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                WinApi.ReleaseCapture();
                WinApi.SendMessage(Handle, WinApi.WM_NCLBUTTONDOWN, WinApi.HT_CAPTION, 0);
            }
        }

        private void timerRecording_Tick(object sender, EventArgs e)
        {
            if (!lighted)
            {
                ibtnRecord.NormalImage = Resources.record_e;
                ibtnRecord.HoverImage = Resources.record_h_e;
                ibtnRecord.PushedImage = Resources.record_p_e;
                lighted = true;
            }
            else
            {
                ibtnRecord.NormalImage = Resources.record;
                ibtnRecord.HoverImage = Resources.record_h;
                ibtnRecord.PushedImage = Resources.record_p;
                lighted = false;
            }

            if (ttRecording == null)
            {
                ttRecording = new ToolTip();
            }

            ttRecording.SetToolTip(lblRecording, "Hang mentése ide: " + (dir + filename + ".mp3") + " (Kattins ide a mappa megnyitásához.)");
            if (File.Exists(dir + filename + ".mp3"))
            {
                long size = (new FileInfo(dir + filename + ".mp3")).Length;
                lblRecording.Text = "Felvétel:  " + FormatSeconds((long)(((double)size*8)/bitrate)) + "  |  " + FormatBytes(size);
            }
            else
                lblRecording.Text = "Felvétel:  00 : 00 : 00  |  0.00 B";
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (recording && MessageBox.Show(this, "Hang felvevés folyamatban van!\nBiztos, hogy bezárja a programot?", Program.NAME, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else {
                if (tRecord != null)
                    tRecord.Abort();
                NetworkChange.NetworkAddressChanged -= NetworkChange_NetworkAddressChanged;
            }
        }

        private void ibtnConfig_MouseClick(object sender, MouseEventArgs e)
        {
            menu.Show(ibtnConfig, e.Location);
        }

        private void miAbout_Click(object sender, EventArgs e)
        {
            (new AboutForm(this)).ShowDialog();
        }

        private void timerTime_Tick(object sender, EventArgs e)
        {
            lblTime.Text = GetMonthName(DateTime.Now.Month) + " " + DateTime.Now.Day + ". " + (DateTime.Now.Hour < 10 ? "0" + DateTime.Now.Hour : DateTime.Now.Hour.ToString()) + ":" + (DateTime.Now.Minute < 10 ? "0" + DateTime.Now.Minute : DateTime.Now.Minute.ToString()) + ":" + (DateTime.Now.Second < 10 ? "0" + DateTime.Now.Second : DateTime.Now.Second.ToString());
        }

        private void lblTime_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                WinApi.ReleaseCapture();
                WinApi.SendMessage(Handle, WinApi.WM_NCLBUTTONDOWN, WinApi.HT_CAPTION, 0);
            }
        }

        private void MainForm_LocationChanged(object sender, EventArgs e)
        {
            if (initialized)
            {
                settings.setValue(skProg.Positon, Location);
                settings.save();
            }
        }

        private void lblHeader_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://" + Program.WEBPAGE);
            }
            catch (Exception)
            {
                Clipboard.SetText("http://" + Program.WEBPAGE);
                MessageBox.Show(this, "Nincs webböngésző telepítve!\r\nA webcím vágólapra másolva.", "Folyamat indító", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblRecording_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", @"/select, " + dir + filename + ".mp3");
        }

        private void player_Buffering(bool Start)
        {
            if (!Start)
            {
                if (!listening)
                {
                    listen = DateTime.Now;
                    listening = true;
                }
                timer_Tick(null, null);
                timer.Enabled = true;
                UpdateTaskBarIcon();
            }
            lblTimer.Text = "Pufferelés... " + player.network.bufferingProgress + "%";
        }

        private void timerNotify_Tick(object sender, EventArgs e)
        {
            if (!listening && !recording) return;

            if (listening && recording)
            {
                switch (notifyCounter % 3)
                {
                    case 0:
                        notify.Icon = ((Icon)((new ComponentResourceManager(typeof(MainForm))).GetObject("notify.Icon")));
                        break;
                    case 1:
                        notify.Icon = Resources.iplay;
                        break;
                    case 2:
                        notify.Icon = Resources.irecord_e;
                        break;
                    default:
                        notify.Icon = ((Icon)((new ComponentResourceManager(typeof(MainForm))).GetObject("notify.Icon")));
                        break;
                }
            }
            else if (listening)
            {
                if (notifyCounter % 2 == 0)
                {
                    notify.Icon = ((Icon)((new ComponentResourceManager(typeof(MainForm))).GetObject("notify.Icon")));
                }
                else
                {
                    notify.Icon = Resources.iplay;
                }
            }
            else if (recording)
            {
                if (notifyCounter % 2 == 0)
                {
                    notify.Icon = ((Icon)((new ComponentResourceManager(typeof(MainForm))).GetObject("notify.Icon")));
                }
                else
                {
                    notify.Icon = Resources.irecord_e;
                }
            }

            notifyCounter++;
        }

        private void player_PlayStateChange(int NewState)
        {
            if (listening)
            {
                switch (NewState)
                {
                    case (int)WMPPlayState.wmppsPlaying:
                        //SetPlayButtons();
                        timer.Enabled = true;
                        break;
                    default:
                        //SetPauseButtons();
                        timer.Enabled = false;
                        lblTimer.Text = "Pufferelés... " + player.network.bufferingProgress + "%"; ;
                        break;
                }
            }
        }

        private void notify_BalloonTipClosed(object sender, EventArgs e)
        {
            HideNotify();
        }

        private void notify_BalloonTipClicked(object sender, EventArgs e)
        {
            HideNotify();
        }

        public void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            Invoke(new UpdateInterfaceByNetworkDelegate(UpdateInterfaceByNetwork), HasNetworkConnection());
        }

        private void miSettings_Click(object sender, EventArgs e)
        {
            SettingsForm form = new SettingsForm();
            form.ShowDialog(this);
        }

        private void btnShowHidePrograms_Click(object sender, EventArgs e)
        {
            if (settings.getValue(skProg.ShowPrograms, false))
            {
                Size = new Size(Size.Width, 186);
                settings.setValue(skProg.ShowPrograms, false);

                btnShowHidePrograms.HoverImage = Resources.down_16;
                btnShowHidePrograms.NormalImage = Resources.down_16;
                btnShowHidePrograms.PushedImage = Resources.down_16;
            }
            else
            {
                Size = new Size(Size.Width, 443);
                settings.setValue(skProg.ShowPrograms, true);

                btnShowHidePrograms.HoverImage = Resources.up_16;
                btnShowHidePrograms.NormalImage = Resources.up_16;
                btnShowHidePrograms.PushedImage = Resources.up_16;
            }

            settings.save();
        }

    }
}