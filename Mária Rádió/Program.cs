using System;
using System.Windows.Forms;
using Maria_Radio.Misc;

namespace Maria_Radio
{
    static class Program
    {
        private static ApplicationContext context;	

        public const string NAME = "Mária Rádió";
        public const string AUTHOR = "Gyurász Krisztián";
        public const string EMAIL = "krisztian@gyurasz.eu";
        public const string WEBPAGE = "www.mariaradio.hu";
        public const string STAT_URL = "http://apps.gyurasz.eu/mariaradio/?xml";
        public const string VERSION_CHECK_URL = "http://apps.gyurasz.eu/mariaradio/?version=";

        public const string BASE_URL = "http://www.mariaradio.hu";
        public const string PROGRAMS_URL = BASE_URL + "/musorok/musornaptar/{0}";
        public const string MOUNTPOINTS_URL = BASE_URL + ":8000/status.xsl";
        public const string M3U_PRE_URL = BASE_URL + ":8000";

        public const string HOST_PING = "google.hu";
        public const string VERSION = "1.2.1.0";
        public const string USER_AGENT = "mariaradio_v" + VERSION;
        public const string SETTINGS_DIR = "MariaRadio";

        public static bool startMinimized;

        private const string APPDATA = "APPDATA";

        public static readonly Settings settings = new Settings(
            Environment.GetEnvironmentVariable(APPDATA) + "\\" + SETTINGS_DIR + "\\Settings.xml");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {


            if (!SingleInstance.Start())
            {
                SingleInstance.ShowFirstInstance();
                return;
            }

            if (args.Length > 0 && args[0].Equals("--minimized"))
            {
                startMinimized = true;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            context = new ApplicationContext();
            Application.Idle += OnAppIdle;

            SplashForm.GetInstance().Show();
            Application.Run(context);

            SingleInstance.Stop();
        }

        private static void OnAppIdle(object sender, EventArgs e)
        {
            if (context.MainForm == null)
            {
                Application.Idle -= OnAppIdle;
                context.MainForm = MainForm.Instance;
                context.MainForm.Show();
                SplashForm.GetInstance().Close();
            }
        }
    }
}