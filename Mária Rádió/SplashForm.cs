using System.Windows.Forms;

namespace Maria_Radio
{
    public partial class SplashForm : Form
    {
        private static SplashForm splashform;

        public static SplashForm GetInstance()
        {
            if (splashform == null)
            {
                splashform = new SplashForm();
            }

            return splashform;
        }

        private SplashForm()
        {
            InitializeComponent();
            Text = Program.NAME;
        }
    }
}