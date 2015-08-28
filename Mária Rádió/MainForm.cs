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
using Maria_Radio.Data;
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
            SELECTED_PROGRAM_BITRATE = 128000,
            DEF_VOLUME = 100;

        private const string
            TITLE_BUFFERING = "Pufferelés... {0:D0}%",
            TITLE_SELECTED_PROGRAM = "Maria Radio musor",
            TITLE_VOLUME = "Hang: {0:D}%",
            TITLE_NO_INTERNET_CONNECTION = "Nincs internet kapcsolat!",
            TITLE_CONNECTING = "Kapcsolódás...",
            TITLE_RECORDING = "Felvétel:  {0}  |  {1}",
            TITLE_LISTENING = "Hallgatás",
            TITLE_PAUSING = "Szüneteltetés",
            TITLE_RECORD = "Felvétel",
            TITLE_LISTEN_RECORD = "Hallgatás / Felvétel",
            TITLE_STOP_RECORDING = "Felvétel leállítása",
            TITLE_RECORDER = Program.NAME + " felvevő";
            

        //variables
        private bool recording, lighted, initialized;
        private string recDir, recFilename;
        private int notifyCounter;

        private WindowsMediaPlayer player;
        private Thread tRecord;
        private Settings settings = Program.settings;
        private List<MountPoint> mountPoints;
        private ProgramList<Data.Program> programs;
        private ThumbnailToolbarButton btnPlay, btnRecord;
        private ToolTip ttRecording, ttVolume, ttTitle, ttProgram;

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

        //OK
        private string StreamURL
        {
            get
            {
                if (cbMountPoints.InvokeRequired)
                {
                    return (string)cbMountPoints.Invoke(
                        new Func<string>(() => StreamURL)
                    );
                }

                return (cbMountPoints.SelectedItem as MountPoint).StreamUrl;
            }
        }

        //OK
        private MainForm()
        {
            InitializeComponent();
            
            ProgramsForm.Instance.CreateControl();
            ProgramsForm.Instance.AddOwnedForm(this);

            settings.onError += settings_onError;
            settings.load();
            
            Initialize();
            InitializeMediaPlayer();

            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;

            UpdateStat();
        }

        #region private methods

        //CHECK IT
        private void Initialize()
        {
            //Texts set
            lblHeader.Text = string.Format("{0} :: {1}", Program.NAME, Program.WEBPAGE);
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
            recDir = settings.getValue(skRec.RecordPath, "");

            if (Directory.Exists(recDir))
            {
                recDir = (recDir.Equals(string.Empty) ? Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "\\" : recDir);
            }

            //Show program form
            ShowProgramForm(settings.getValue(skProg.ShowPrograms, false));

            //load alwaysontop setting
            TopMost = settings.getValue(skGen.AlwaysOnTop, false);
        }

        //OK
        private void InitializeMediaPlayer()
        {
            slVolume.Value = settings.getValue(skProg.Volume, DEF_VOLUME);
            ttVolume = ttVolume ?? new ToolTip();
            ttVolume.SetToolTip(slVolume, string.Format(TITLE_VOLUME, slVolume.Value));

            player = new WindowsMediaPlayer();
            player.settings.volume = slVolume.Value;
            player.network.bufferingTime = 10000;
            player.settings.autoStart = false;
            player.Buffering += player_Buffering;
            player.PlayStateChange += player_PlayStateChange;
        }

        //OK
        private void SetPlayButtons()
        {
            if (isPlayingBuffering())
            {
                ibtnPlay.NormalImage = Resources.pause;
                ibtnPlay.HoverImage = Resources.pause_h;
                ibtnPlay.PushedImage = Resources.pause_p;

                if (TaskbarManager.IsPlatformSupported)
                {
                    btnPlay.Icon = Resources.ipause;
                    btnPlay.Tooltip = TITLE_PAUSING;
                }
            }
            else
            {
                ibtnPlay.NormalImage = Resources.play;
                ibtnPlay.HoverImage = Resources.play_h;
                ibtnPlay.PushedImage = Resources.play_p;

                if (TaskbarManager.IsPlatformSupported)
                {
                    btnPlay.Icon = Resources.iplay;
                    btnPlay.Tooltip = TITLE_LISTENING;
                }
            }
            
        }
        
        //OK
        private void PlayStop()
        {
            if (isPlayingBuffering())
            {
                player.controls.stop();
            }
            else
            {
                player.URL = StreamURL;
                player.controls.play();
            }
        }

        //OK
        private void Record()
        {
            if (!recording)
            {
                recording = true;

                if (TaskbarManager.IsPlatformSupported)
                {
                    btnRecord.Icon = Resources.irecord_e;
                    btnRecord.Tooltip = TITLE_STOP_RECORDING;
                }

                tRecord = new Thread(delegate()
                {
                    HttpWebRequest request;
                    HttpWebResponse response;

                    int metaInt;
                    int count = 0;
                    int metadataLength = 0;

                    byte[] buffer = new byte[512];

                    Stream socketStream = null;
                    Stream byteOut = null;

                    request = (HttpWebRequest) WebRequest.Create(StreamURL);

                    request.Headers.Clear();
                    request.Method = "GET";
                    request.Accept = "*/*";
                    request.Headers.Add("Icy-MetaData", "1");
                    request.UserAgent = Program.USER_AGENT;

                    try
                    {
                        response = (HttpWebResponse) request.GetResponse();
                    }
                    catch (Exception ex)
                    {
                        StopRecording();

                        Invoke(new Action(() =>
                        {
                            MessageBox.Show(this, string.Format("{0}\r\n{1}", ex.Message, ex.StackTrace), TITLE_RECORDER,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));

                        return;
                    }

                    if (!response.StatusCode.ToString().Equals("OK"))
                    {
                        StopRecording();

                        Invoke(new Action(() =>
                        {
                            MessageBox.Show(this, $"Hibás válasz a szerver felől! ({response.StatusCode})",
                                TITLE_RECORDER, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));

                        return;
                    }

                    metaInt = Convert.ToInt32(response.GetResponseHeader("icy-metaint"));
                    recFilename = $"maria_radio_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
                    
                    try
                    {
                        socketStream = response.GetResponseStream();
                        byteOut = CreateFile(ref recDir, ref recFilename);

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
                                            if (count%100 == 0)
                                                byteOut.Flush();
                                        }
                                    }
                                    else
                                    {
                                        metadataLength = Convert.ToInt32(buffer[i])*16;
                                        count = 0;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        StopRecording();

                        Invoke(new Action(() =>
                        {
                            MessageBox.Show(this, string.Format("{0}\r\n{1}", ex.Message, ex.StackTrace), TITLE_RECORDER,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }
                    finally
                    {
                        byteOut?.Close();
                        socketStream?.Close();
                    }
                });

                tRecord.Start();

                timerRecording.Enabled = true;
                lblRecording.Visible = true;
            }
            else
            {
                StopRecording();

                if (settings.getValue(skRec.AfterRecordOpenFolder, true))
                {
                    Process.Start("explorer.exe", @"/select, " + recDir + recFilename);
                }
            }

            UpdateTaskBarIcon();
        }

        //OK
        private void StopRecording()
        {
            recording = false;

            btnRecord.Icon = Resources.irecord;
            btnRecord.Tooltip = "Felvétel";

            ibtnRecord.NormalImage = Resources.record;

            timerRecording.Enabled = false;

            lblRecording.Visible = false;
            lblRecording.Text = string.Format(TITLE_RECORDING, TimeSpan.FromSeconds(0L), FormatBytes(0));
        }

        //OK
        private void GetHttpContent(string url, GotHttpContent method)
        {
            new Thread(delegate ()
            {
                string response;

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
                            MessageBox.Show(this, string.Format("{0}\r\n{1}", ex.Message, ex.StackTrace), Program.NAME,
                                MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                        {
                            GetHttpContent(url, method);
                        }
                    }));
                }
            }
            ).Start();
        }

        //OK
        private void SetProgramsFormBounds()
        {
            ProgramsForm.Instance.Bounds = new Rectangle(
                    Location.X,
                    Location.Y + Height,
                    Width,
                    ProgramsForm.Instance.Height);
        }

        //OK
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

        //OK
        private void UpdateTaskBarIcon()
        {
            if (!TaskbarManager.IsPlatformSupported)
            {
                return;
            }

            if (recording && isPlayingBuffering())
            {
                TaskbarManager.Instance.SetOverlayIcon(Handle, Resources.pr, TITLE_LISTEN_RECORD);
            }   
            else if (recording && !isPlayingBuffering())
            {
                TaskbarManager.Instance.SetOverlayIcon(Handle, Resources.irecord, TITLE_RECORD);
            }   
            else if (!recording && isPlayingBuffering())
            {
                TaskbarManager.Instance.SetOverlayIcon(Handle, Resources.iplay, TITLE_LISTENING);
            }
            else
            {
                TaskbarManager.Instance.SetOverlayIcon(Handle, null, string.Empty);
            }
        }

        //OK
        private bool isPlayingBuffering()
        {
            return player.playState == WMPPlayState.wmppsPlaying || player.playState == WMPPlayState.wmppsBuffering;
        }

        //FULL FAIL
        private void UpdateStatSuccess(XmlDocument XMLDoc, bool bThread)
        {
            /*try
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
                        //StreamURL = el.InnerText;
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
            }*/
        }

        //FAIL
        private void UpdateStatFail()
        {
            //MessageBox.Show(this, "Hiba történt az adatok lekérdezése közben!\r\nKérem ellenőrizze az internet kapcsolatát.", Program.NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);

            /*notify.Visible = true;
            notify.BalloonTipIcon = ToolTipIcon.Error;
            notify.BalloonTipTitle = Program.NAME;
            notify.BalloonTipText = "Hiba történt az adatok lekérdezése közben!";
            notify.ShowBalloonTip(0);

            if (this == null || IsDisposed || StreamURL == null || StreamURL.Equals(string.Empty))
                UpdateInterfaceByNetwork(false);*/
            //    Process.GetCurrentProcess().Kill();
        }

        //FAIL
        private void UpdateStatThread()
        {
            /*try
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
            }*/
        }

        //OK
        private void UpdateStat()
        {
            GetHttpContent(Program.MOUNTPOINTS_URL, SetMountPointList);
            GetHttpContent(string.Format(Program.PROGRAMS_URL, DateTime.Now.ToString("yyyyMMdd")), SetProgramList);
        }

        //OK
        private void CheckForUpdate()
        {
            new Thread(delegate()
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
                    Invoke(new Action(() =>
                    {
                        if (MessageBox.Show(
                            this, 
                            string.Format("Újabb verziója jelent meg a {0} programnak!\r\nÖn verziója: v{1}\nÚj verzió: v{2}\r\n\r\nFrissítsük a {0} programot?\r\n(A frissítés alatt a {0} program bezárul.)", 
                                Program.NAME, 
                                Program.VERSION, 
                                newVersion), 
                            string.Format("{0} :: Frissítő", Program.NAME), 
                            MessageBoxButtons.YesNo, 
                            MessageBoxIcon.Question, 
                            MessageBoxDefaultButton.Button1
                        ) == DialogResult.Yes)
                        {
                            StartUpdater();
                        }
                    }));
                }
            }).Start();
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

        //OK
        private void ShowForm()
        {
            notify.Visible = false;
            timerNotify.Enabled = false;
            notifyCounter = 0;
            notify.Icon = ((Icon)((new ComponentResourceManager(typeof(MainForm))).GetObject("notify.Icon")));
            Show();

            if (settings.getValue(skProg.ShowPrograms, true))
            {
                ShowProgramForm(true);
            }
            
        }

        //OK
        private void Minimize()
        {
            ShowProgramForm(false);

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

        //OK
        private void HideNotify()
        {
            if (Visible)
            {
                notify.Visible = false;
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

            mountPoints = new List<MountPoint>();

            foreach (HtmlNode node in nodes)
            {
                HtmlNodeCollection dataNodes = node.SelectNodes(".//td[@class='streamdata']");
                HtmlNodeCollection aNodes = node.SelectNodes(".//a");

                if (dataNodes != null && dataNodes.Count == 10)
                {
                    string m3u = aNodes[0].GetAttributeValue("href", null);

                    m3u = m3u?.Substring(0, m3u.LastIndexOf("."));
                    m3u = m3u.Insert(0, Program.M3U_PRE_URL);

                    mountPoints.Add(
                        new MountPoint(
                            m3u,
                            dataNodes[MountPoint.INDEX_TITLE].InnerText,
                            dataNodes[MountPoint.INDEX_DESCRIPTION].InnerText,
                            dataNodes[MountPoint.INDEX_CONTENT_TYPE].InnerText,
                            DateTime.Parse(dataNodes[MountPoint.INDEX_UPTIME].InnerText),
                            int.Parse(dataNodes[MountPoint.INDEX_BITRATE].InnerText) * 1000,
                            int.Parse(dataNodes[MountPoint.INDEX_CURRENTLISTENERS].InnerText),
                            int.Parse(dataNodes[MountPoint.INDEX_PEAKLISTENERS].InnerText),
                            dataNodes[MountPoint.INDEX_GENRE].InnerText,
                            dataNodes[MountPoint.INDEX_URL].InnerText,
                            dataNodes[MountPoint.INDEX_CURRENT_SONG].InnerText
                        )
                    );
                }
            }


            foreach (MountPoint mountPoint in mountPoints)
            {
                cbMountPoints.Items.Add(mountPoint);

                if (mountPoint.Title.Equals(TITLE_SELECTED_PROGRAM) && mountPoint.Bitrate == SELECTED_PROGRAM_BITRATE)
                {
                    cbMountPoints.SelectedItem = mountPoint;
                }
            }

            if (cbMountPoints.SelectedIndex == -1 && cbMountPoints.Items.Count > 0)
            {
                cbMountPoints.SelectedIndex = 0;
            }
            
            ibtnRecord.Enabled = ibtnPlay.Enabled = cbMountPoints.SelectedIndex != -1;
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
        private static Stream CreateFile(ref string destPath, ref string filename)
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

            if (destPath[destPath.Length - 1] != '\\')
            {
                destPath += '\\';
            }

            try
            {
                if (!Directory.Exists(destPath))
                    Directory.CreateDirectory(destPath);

                if (!File.Exists(destPath + filename + ".mp3"))
                {
                    filename += ".mp3";

                    return File.Create(destPath + filename);
                }

                for (int i = 1; ; i++)
                {
                    if (!File.Exists(destPath + filename + "_(" + i + ").mp3"))
                    {
                        filename += "_(" + i + ").mp3";
                        return File.Create(destPath + filename);
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

        //OK
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

        //OK
        protected override void OnShown(EventArgs e)
        {
            if (TaskbarManager.IsPlatformSupported)
            {

                btnPlay = new ThumbnailToolbarButton(Resources.iplay, TITLE_LISTENING);
                btnPlay.Click += ibtnPlay_Click;

                btnRecord = new ThumbnailToolbarButton(Resources.irecord, TITLE_RECORD);
                btnRecord.Click += ibtnRecord_Click;

                TaskbarManager.Instance.ThumbnailToolbars.AddButtons(Handle, btnPlay, btnRecord);
            }
            base.OnShown(e);
        }

        //OK
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
        //OK
        private void settings_onError(Exception e)
        {
            MessageBox.Show(this, e.Message + "\r\n\r\n" + e.StackTrace, e.GetType().FullName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        //OK
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
            ttVolume.SetToolTip(slVolume, string.Format(TITLE_VOLUME, slVolume.Value));

            settings.setValue(skProg.Volume, slVolume.Value);
            settings.save();
        }

        //OK
        private void ibtnX_Click(object sender, EventArgs e)
        {
            Close();
        }

        //OK
        private void ibtnMinimize_Click(object sender, EventArgs e)
        {
            Minimize();
        }

        //OK
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

        //OK
        private void timerUpdateStats_Tick(object sender, EventArgs e)
        {
            UpdateStat();
        }

        //OK
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


            string filePath = recDir + recFilename;

            ttRecording = ttRecording ?? new ToolTip();
            ttRecording.SetToolTip(lblRecording, $"Hang mentése ide: {filePath} (Kattins ide a mappa megnyitásához.)");

            if (File.Exists(filePath))
            {
                long size = (new FileInfo(filePath)).Length;

                lblRecording.Text = string.Format(TITLE_RECORDING,
                    TimeSpan.FromSeconds((long) (((double) size*8)/(cbMountPoints.SelectedItem as MountPoint).Bitrate)),
                    FormatBytes(size)
                    );
            }
            else
            {
                lblRecording.Text = string.Format(TITLE_RECORDING, TimeSpan.FromSeconds(0L), FormatBytes(0));
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (recording && MessageBox.Show(this, "Hang felvevés folyamatban van!\nBiztos, hogy bezárja a programot?", 
                Program.NAME, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                StopRecording();
                tRecord.Join();

                //tRecord?.Abort();

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
            Process.Start("explorer.exe", @"/select, " + recDir + recFilename);
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

        //OK
        private void player_Buffering(bool Start)
        {
            if (Start)
            {
                lblTimer.Text = string.Format(TITLE_BUFFERING, 0);
                timerBuffer.Enabled = true;
                timerUpdatePlayTime.Enabled = false;

                UpdateTaskBarIcon();
                SetPlayButtons();
            }
            else
            {
                lblTimer.Text = string.Format(TITLE_BUFFERING, 100);
                timerBuffer.Enabled = false;
                timerUpdatePlayTime.Enabled = true;
            }
        }

        //OK
        private void player_PlayStateChange(int NewState)
        {
            switch (NewState)
            {
                case (int)WMPPlayState.wmppsPlaying:
                    SetPlayButtons();
                    break;
                case (int)WMPPlayState.wmppsStopped:
                    SetPlayButtons();
                    lblTimer.Text = TimeSpan.FromSeconds(0).ToString();
                    UpdateTaskBarIcon();
                    break;
                case (int)WMPPlayState.wmppsBuffering:
                    lblTimer.Text = string.Format(TITLE_BUFFERING, player.network.bufferingProgress);
                    break;
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

        //OK
        private void timerBuffer_Tick(object sender, EventArgs e)
        {
            if (player.playState != WMPPlayState.wmppsStopped)
            {
                lblTimer.Text = string.Format(TITLE_BUFFERING, player.network.bufferingProgress);
            }
        }

        //OK
        private void timerUpdatePlayTime_Tick(object sender, EventArgs e)
        {
            lblTimer.Text = TimeSpan.FromSeconds((long)player.controls.currentPosition).ToString();
        }

        //OK
        private void cbMountPoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isPlayingBuffering())
            {
                PlayStop();
                PlayStop();
            }
        }

        //OK
        private void notify_BalloonTipClicked(object sender, EventArgs e)
        {
            HideNotify();
        }

        private void UpdateInterfaceByNetwork(bool HasNetworkConnection)
        {
            if (HasNetworkConnection)
            {
                lblTitle.ForeColor = Color.FromArgb(40, 153, 255);
                lblTitle.Text = TITLE_CONNECTING;
                //UpdateStat();
            }
            else
            {
                if (isPlayingBuffering())
                {
                    player.controls.stop();
                }
                if (recording)
                {
                    StopRecording();
                }

                lblTitle.ForeColor = Color.FromArgb(255, 0, 0); ;
                lblTitle.Text = TITLE_NO_INTERNET_CONNECTION;

                notify.Visible = true;
                notify.BalloonTipIcon = ToolTipIcon.Error;
                notify.BalloonTipTitle = Program.NAME;
                notify.BalloonTipText = TITLE_NO_INTERNET_CONNECTION;
                notify.ShowBalloonTip(0);

                lblProgram.Text = string.Empty;
                ibtnPlay.Enabled = false;
                ibtnRecord.Enabled = false;
            }
        }

        //OK
        public void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                UpdateInterfaceByNetwork(HasNetworkConnection());
            }));
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