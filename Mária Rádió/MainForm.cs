using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using HtmlAgilityPack;
using Maria_Radio.Properties;
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
        //consts
        private const int
            WS_MINIMIZEBOX = 0x20000,
            CS_DBLCLKS = 0x8,
            SELECTED_PROGRAM_BITRATE = 128,
            DEF_VOLUME = 100;

        private const string
            BUFFER_TEXT = "Pufferelés... {0:D0} %",
            SELECTED_PROGRAM_TITLE = "Maria Radio musor",
            VOLUME_TITLE = "Hang [{0:D}%]";

        //variables
        private bool recording, lighted, initialized;
        private string streamURL, dir, filename;
        private int notifyCounter;

        private WindowsMediaPlayer player;
        private Thread tRecord, tUpdate, tUpdateCheck;
        private Settings settings = Program.settings;
        private List<Data.MountPoint> mountPoints;
        private ProgramList<Data.Program> programs;
        private ThumbnailToolbarButton btnPlay, btnRecord;
        private ToolTip ttRecording, ttVolume, ttTitle, ttProgram;

        private delegate void RecordAbortedDelegate(string message);
        private delegate void UpdateStatSuccessDelegate(XmlDocument doc, bool bThread);
        private delegate void UpdateStatFailDelegate();
        private delegate void StartUpdaterDelegate(string PopupText);
        private delegate void UpdateInterfaceByNetworkDelegate(bool hasNetwork);
        private delegate void GotHttpContent(string content);

        private static MainForm instance;

        public static MainForm Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainForm();
                }

                return instance;
            }
        }

        private MainForm()
        {
            InitializeComponent();
            
            ProgramsForm.Instance.CreateControl();
            ProgramsForm.Instance.AddOwnedForm(this);

            settings.load();
            settings.onError += settings_onError;

            Initialize();
            InitializeMediaPlayer();

            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;

            GetHttpContent(Program.MOUNTPOINTS_URL, SetMountPointList);
            GetHttpContent(string.Format(Program.PROGRAMS_URL, DateTime.Now.ToString("yyyyMMdd")), SetProgramList);
        }

        #region private methods

        //CHECK it
        private void Initialize()
        {
            //Texts set
            lblHeader.Text = Program.NAME + " :: " + Program.WEBPAGE;
            Text = Program.NAME;

            //tool tips
            new ToolTip().SetToolTip(ibtnRecord, "Felvétel / Felvétel leállítása (Ctrl + R)");
            new ToolTip().SetToolTip(ibtnPlay, "Hallgatás / Szüneteltetés (Space)");

            new ToolTip().SetToolTip(ibtnConfig, "Beállítások");
            new ToolTip().SetToolTip(ibtnMinimize, "Lekicsinyítés a tálcára");
            new ToolTip().SetToolTip(ibtnX, "Bezárás (Alt + F4)");

            new ToolTip().SetToolTip(lblHeader, "Mária Rádió honlapjának megnyitása");
            new ToolTip().SetToolTip(lblTime, "Jelenlegi idő");

            new ToolTip().SetToolTip(lblTimer, "Hallgatási idő");
            new ToolTip().SetToolTip(btnShowHidePrograms, "Programok megjelenítése/elrejtése");
            
            //set proxy
            if (settings.getValue(skProx.UseProxy, false))
            {
                WebProxy proxy = new WebProxy();

                proxy.Address = new Uri(
                    "http://" + settings.getValue(skProx.Host, "127.0.0.1") +
                    ":" + settings.getValue<Decimal>(skProx.Port, 8080)
                );

                proxy.Credentials = new NetworkCredential(
                    settings.getValue(skProx.User, ""),
                    settings.getValue(skProx.Pass, "")
                );

                WebRequest.DefaultWebProxy = proxy;
            }
            
            // CHECK IT!!!
            if (HasNetworkConnection())
            {
                //UpdateStatNoThread();
            }
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
            // END OF CHECK IT

            //set record path
            dir = settings.getValue(skRec.RecordPath, "");

            if (Directory.Exists(dir))
            {
                dir = (dir.Equals(string.Empty) ? Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "\\" : dir);
            }

            //Show program form
            ShowProgramForm(settings.getValue(skProg.ShowPrograms, false));

            //load alwaysontop setting
            TopMost = settings.getValue(skGen.AlwaysOnTop, false);
        }

        private void InitializeMediaPlayer()
        {
            slVolume.Value = settings.getValue(skProg.Volume, DEF_VOLUME);
            ttVolume = ttVolume ?? new ToolTip();
            ttVolume.SetToolTip(slVolume, string.Format(VOLUME_TITLE, slVolume.Value));

            player = new WindowsMediaPlayer();
            player.settings.volume = slVolume.Value;
            player.network.bufferingTime = 10000;
            player.settings.autoStart = false;
            player.Buffering += player_Buffering;
            player.PlayStateChange += player_PlayStateChange;
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
            player.URL = (cbMountPoints.SelectedItem as Data.MountPoint).StreamUrl;
            player.controls.play();
        }

        private void Stop()
        {
            SetPauseButtons();
            player.controls.stop();
            player.close();

            player = new WindowsMediaPlayer();
            player.settings.volume = settings.getValue(skProg.Volume, DEF_VOLUME);
            player.settings.autoStart = false;
            player.URL = streamURL;
            player.Buffering += player_Buffering;
            player.PlayStateChange += player_PlayStateChange;

            //player.newMedia(this.streamURL);
            timerUpdatePrograms.Enabled = false;
            lblTimer.Text = TimeSpan.FromSeconds(0).ToString();
            UpdateTaskBarIcon();
        }

        private void PlayStop()
        {
            if (player.playState == WMPPlayState.wmppsPlaying)
                Stop();
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

        private void GetHttpContent(string url, GotHttpContent method)
        {
            new Thread(delegate ()
            {
                string response = null;

                try
                {
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
                catch (WebException ex)
                {
                    Invoke(new Action(() =>
                    {
                        if (method == SetMountPointList)
                        {
                            lblTitle.ForeColor = Color.Red;
                            lblTitle.Text = ex.Message;
                        }

                        if (
                            MessageBox.Show(this, ex.Message + "\r\n" + ex.StackTrace, Program.NAME,
                                MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                        {
                            GetHttpContent(url, method);
                        }
                    }));
                }
            }
            ).Start();
        }

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

        private void SetProgramsFormBounds()
        {
            ProgramsForm.Instance.Bounds = new Rectangle(
                    Location.X,
                    Location.Y + Height,
                    Width,
                    ProgramsForm.Instance.Height);
        }

        private void ShowProgramForm(bool show)
        {
            if (show)
            {
                SetProgramsFormBounds();
                ProgramsForm.Instance.Show();

                btnShowHidePrograms.HoverImage = Resources.up_16;
                btnShowHidePrograms.NormalImage = Resources.up_16;
                btnShowHidePrograms.PushedImage = Resources.up_16;
            }
            else
            {
                ProgramsForm.Instance.Hide();

                btnShowHidePrograms.HoverImage = Resources.down_16;
                btnShowHidePrograms.NormalImage = Resources.down_16;
                btnShowHidePrograms.PushedImage = Resources.down_16;
            }
        }

        private void RecordAborted(string message)
        {
            StopRecord();
            MessageBox.Show(this, message, Program.NAME + " felvevő", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void UpdateTaskBarIcon()
        {
            if (!TaskbarManager.IsPlatformSupported) return;

            if (recording && player.playState == WMPPlayState.wmppsPlaying)
                TaskbarManager.Instance.SetOverlayIcon(Resources.pr, "Hallgatás / Felvétel");
            else if (recording && player.playState != WMPPlayState.wmppsPlaying)
                TaskbarManager.Instance.SetOverlayIcon(Resources.irecord, "Felvétel");
            else if (!recording && player.playState == WMPPlayState.wmppsPlaying)
                TaskbarManager.Instance.SetOverlayIcon(Resources.iplay, "Hallgatás");
            else
                TaskbarManager.Instance.SetOverlayIcon(null, "");
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
                        ttTitle = ttTitle ?? new ToolTip();
                        ttTitle.SetToolTip(lblTitle, el.InnerText);
                    }
                    else if (el.Name == "program")
                    {
                        if (notifyProgram && lblProgram.Text != el.InnerText && !el.InnerText.Equals(string.Empty))
                        {
                            notifyText = true;
                        }

                        lblProgram.Text = el.InnerText;

                        ttProgram = ttProgram ?? new ToolTip();
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
                        /*try
                        {
                            bitrate = int.Parse(el.InnerText);
                        }
                        catch (Exception)
                        {
                            timerUpdate.Interval = 128000;
                        }*/
                    }
                    else if (el.Name == "programs")
                    {
                        /*dataGridView.Rows.Clear();
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
                        }*/
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

        private void UpdateStatThread()
        {
            try
            {
                XmlDocument XMLDoc = new XmlDocument();
                XMLDoc.Load(Program.STAT_URL);
                Invoke(new UpdateStatSuccessDelegate(UpdateStatSuccess), XMLDoc, true);
            }
            catch (Exception)
            {
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
                if (player.playState == WMPPlayState.wmppsPlaying)
                    Stop();
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
                //dataGridView.Rows.Clear();
            }
        }

        #endregion

        //OK
        #region http content parsers methods

        //OK
        private void SetMountPointList(string content)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='roundcont']");

            mountPoints = new List<Data.MountPoint>();

            foreach (HtmlNode node in nodes)
            {
                HtmlNodeCollection dataNodes = node.SelectNodes(".//td[@class='streamdata']");
                HtmlNodeCollection aNodes = node.SelectNodes(".//a");

                if (dataNodes != null && dataNodes.Count == 10)
                {
                    string m3u = aNodes[0].GetAttributeValue("href", null);

                    m3u = m3u?.Substring(0, m3u.LastIndexOf("."));
                    m3u = m3u.Insert(0, "http://mariaradio.hu:8000");

                    mountPoints.Add(
                        new Data.MountPoint(
                            m3u,
                            dataNodes[Data.MountPoint.INDEX_TITLE].InnerText,
                            dataNodes[Data.MountPoint.INDEX_DESCRIPTION].InnerText,
                            dataNodes[Data.MountPoint.INDEX_CONTENT_TYPE].InnerText,
                            DateTime.Parse(dataNodes[Data.MountPoint.INDEX_UPTIME].InnerText),
                            int.Parse(dataNodes[Data.MountPoint.INDEX_BITRATE].InnerText),
                            int.Parse(dataNodes[Data.MountPoint.INDEX_CURRENTLISTENERS].InnerText),
                            int.Parse(dataNodes[Data.MountPoint.INDEX_PEAKLISTENERS].InnerText),
                            dataNodes[Data.MountPoint.INDEX_GENRE].InnerText,
                            dataNodes[Data.MountPoint.INDEX_URL].InnerText,
                            dataNodes[Data.MountPoint.INDEX_CURRENT_SONG].InnerText
                        )
                    );
                }
            }


            foreach (Data.MountPoint mountPoint in mountPoints)
            {
                cbMountPoints.Items.Add(mountPoint);

                if (mountPoint.Title.Equals(SELECTED_PROGRAM_TITLE) && mountPoint.Bitrate == SELECTED_PROGRAM_BITRATE)
                {
                    cbMountPoints.SelectedItem = mountPoint;
                }
            }

            ibtnPlay.Enabled = true;
            ibtnRecord.Enabled = true;
        }

        //OK
        private void SetProgramList(string content)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);

            HtmlNodeCollection progDates = doc.DocumentNode.SelectNodes("//td[@class='mlistaido']");
            HtmlNodeCollection progNamesTitles = doc.DocumentNode.SelectNodes("//td[@class='mlistacim']");

            const String br = "|||";
            bool gotCurrent = false;
            programs = new ProgramList<Data.Program>();

            for (int i = 0; i < progDates.Count && i < progNamesTitles.Count; i++)
            {
                String time = progDates[i].InnerText;
                String txt = progNamesTitles[i].InnerHtml;

                txt = txt.Replace("<br>", br);
                txt = Regex.Replace(txt, "<.*?>", string.Empty);

                int hours = int.Parse(time.Substring(0, time.IndexOf(":")));
                int mins = int.Parse(time.Substring(time.IndexOf(":") + 1));

                DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours, mins, DateTime.Now.Second);

                Data.Program program = new Data.Program(
                    txt.Substring(0, txt.IndexOf(br)),
                    txt.Substring(txt.IndexOf(br) + br.Length),
                    dt, false
                );

                programs.Add(program);

                if (!gotCurrent && DateTime.Now.CompareTo(program.DateTime) < 0)
                {
                    if (programs.Count == 1)
                    {
                        programs[0].Current = true;
                    }
                    else
                    {
                        programs[programs.Count - 2].Current = true;
                    }

                    gotCurrent = true;
                }
            }

            for (int i = programs.Count - 1; i >= 0; i--)
            {
                if (DateTime.Now.CompareTo(programs[i].DateTime) > 0 && !programs[i].Current)
                {
                    programs.RemoveAt(i);
                }
            }

            if (programs.Count > 0)
            {
                lblTitle.Text = programs[0].Title;
                lblProgram.Text = programs[0].Description;
            }

            foreach (Data.Program program in programs)
            {
                int row = ProgramsForm.Instance.ProgramList.Rows.Add(
                    UppercaseFirst(program.DateTime.ToString("MMM dd. HH:mm")),
                    program.Title +
                    (!program.Description.Equals(string.Empty) ? " - " + program.Description : string.Empty)
                );

                if (program.Current)
                {
                    ProgramsForm.Instance.ProgramList.Rows[row].DefaultCellStyle.BackColor = Color.FromArgb(8, 16, 8);
                    ProgramsForm.Instance.ProgramList.Rows[row].DefaultCellStyle.ForeColor = Color.FromArgb(55, 194, 55);

                    ProgramsForm.Instance.ProgramList.Rows[row].DefaultCellStyle.SelectionBackColor = Color.FromArgb(16, 32, 16);
                    ProgramsForm.Instance.ProgramList.Rows[row].DefaultCellStyle.SelectionForeColor = Color.FromArgb(55, 194, 55);
                }
            }
        }

        #endregion

        //OK
        #region static methods
        //OK
        private static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        //OK
        private static string FormatBytes(long bytes)
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

        //OK
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

        //OK
        private static bool HasNetworkConnection()
        {
            try
            {
                Dns.GetHostAddresses(Program.HOST_PING);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(null, e.Message + "\r\n" + e.StackTrace, Program.NAME, MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }
        }

        #endregion

        #region override methods

        //OK
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

                if (!initialized)
                    initialized = true;
            }
            catch { }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Up)
            {
                slVolume.Value += slVolume.LargeChange;
            }
            else if (keyData == Keys.Down)
            {
                slVolume.Value -= slVolume.LargeChange;
            }
            else if (keyData == Keys.Right)
            {
                slVolume.Value += slVolume.SmallChange;
            }
            else if (keyData == Keys.Left)
            {
                slVolume.Value -= slVolume.SmallChange;
            }
            else if (keyData == (Keys.Control | Keys.U))
            {
                UpdateStat();
            }
            else if (keyData == Keys.Space)
            {
                PlayStop();
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

        #endregion
        
        #region events
        private void settings_onError(Exception e)
        {
            MessageBox.Show(this, e.Message + "\r\n\r\n" + e.StackTrace, e.GetType().FullName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ibtnPlay_Click(object sender, EventArgs e)
        {
            PlayStop();
        }

        //OK
        private void ibtnRecord_Click(object sender, EventArgs e)
        {
            Record();
        }

        //OK
        private void slVolume_ValueChanged(object sender, EventArgs e)
        {
            player.settings.volume = slVolume.Value;
            ttVolume.SetToolTip(slVolume, string.Format(VOLUME_TITLE, slVolume.Value));

            settings.setValue(skProg.Volume, slVolume.Value);
            settings.save();
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

            
            ttRecording = ttRecording ?? new ToolTip();
            ttRecording.SetToolTip(lblRecording, "Hang mentése ide: " + (dir + filename + ".mp3") + " (Kattins ide a mappa megnyitásához.)");

            if (File.Exists(dir + filename + ".mp3"))
            {
                long size = (new FileInfo(dir + filename + ".mp3")).Length;

                lblRecording.Text = string.Format("Felvétel:  {0}  |  {1}",
                    TimeSpan.FromSeconds((long)(((double)size * 8) / mountPoints[cbMountPoints.SelectedIndex].Bitrate * 1000)),
                    FormatBytes(size)
                    );
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

        //OK //Az idő megjelenítése az form tetején
        private void timerTime_Tick(object sender, EventArgs e)
        {
            lblTime.Text = UppercaseFirst(DateTime.Now.ToString("MMM dd. HH:mm:ss"));
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
            if (Start)
            {
                lblTimer.Text = string.Format(BUFFER_TEXT, 0);
                timerBuffer.Enabled = true;
                timerUpdatePlayTime.Enabled = false;
            }
            else
            {
                lblTimer.Text = string.Format(BUFFER_TEXT, 100);
                timerBuffer.Enabled = false;
                timerUpdatePlayTime.Enabled = true;
                UpdateTaskBarIcon();
            }
        }

        private void timerNotify_Tick(object sender, EventArgs e)
        {
            if (player.playState != WMPPlayState.wmppsPlaying && !recording) return;

            if (player.playState == WMPPlayState.wmppsPlaying && recording)
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
            else if (player.playState == WMPPlayState.wmppsPlaying)
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
            if (player.playState == WMPPlayState.wmppsPlaying)
            {
                switch (NewState)
                {
                    case (int)WMPPlayState.wmppsPlaying:
                        //SetPlayButtons();
                        timerUpdatePrograms.Enabled = true;
                        break;
                    default:
                        //SetPauseButtons();
                        timerUpdatePrograms.Enabled = false;
                        lblTimer.Text = "Pufferelés... " + player.network.bufferingProgress + "%"; ;
                        break;
                }
            }
        }

        private void notify_BalloonTipClosed(object sender, EventArgs e)
        {
            HideNotify();
        }

        private void MainForm_Move(object sender, EventArgs e)
        {
            SetProgramsFormBounds();
        }

        private void timerBuffer_Tick(object sender, EventArgs e)
        {
            lblTimer.Text = string.Format(BUFFER_TEXT, player.network.bufferingProgress);
        }

        //OK
        private void timerUpdatePlayTime_Tick(object sender, EventArgs e)
        {
            lblTimer.Text = TimeSpan.FromSeconds((long)player.controls.currentPosition).ToString();
        }

        private void cbMountPoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (player.playState == WMPPlayState.wmppsPlaying)
            {
                PlayStop();
            }
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
            bool showed = ProgramsForm.Instance.Visible;

            ShowProgramForm(!showed);
            settings.setValue(skProg.ShowPrograms, !showed);
            
            settings.save();
        }
        #endregion
    }
}