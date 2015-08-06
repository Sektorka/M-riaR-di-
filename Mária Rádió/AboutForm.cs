using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Maria_Radio
{
    public partial class AboutForm : Form
    {
        public AboutForm(MainForm mf)
        {
            InitializeComponent();

            TopMost = mf.TopMost;

            Text = Program.NAME + " :: Névjegy";
            lblTitle.Text = Program.NAME;
            lblAuthorVal.Text = Program.AUTHOR;
            lblMailVal.Text = Program.EMAIL;
            lblWebPageVal.Text = Program.WEBPAGE;
            lblVersionVal.Text = Program.VERSION;
        }

        private void ibtnX_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AboutForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                WinApi.ReleaseCapture();
                WinApi.SendMessage(Handle, WinApi.WM_NCLBUTTONDOWN, WinApi.HT_CAPTION, 0);
            }
        }

        private void lblTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                WinApi.ReleaseCapture();
                WinApi.SendMessage(Handle, WinApi.WM_NCLBUTTONDOWN, WinApi.HT_CAPTION, 0);
            }
        }

        private void lblWebPageVal_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://" + Program.WEBPAGE);
            }
            catch (Exception)
            {
                Clipboard.SetText("http://" + Program.WEBPAGE);
                MessageBox.Show(this, "Nincs webböngészõ telepítve!\r\nA webcím vágólapra másolva.", "Folyamat indító", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblMailVal_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("mailto:"+Program.EMAIL);
            }
            catch (Exception)
            {
                Clipboard.SetText(Program.EMAIL);
                MessageBox.Show(this, "Nincs e-mail küldõ telepítve!\r\nAz e-mail cím vágólapra másolva!", "Folyamat indító!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}