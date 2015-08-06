using System;
using System.Windows.Forms;
using Maria_Radio.Misc;
using skProx = Maria_Radio.Misc.SettingKeys.Proxy;
using skGen = Maria_Radio.Misc.SettingKeys.General;
using skRec = Maria_Radio.Misc.SettingKeys.Record;

namespace Maria_Radio
{
    public partial class SettingsForm : Form
    {
        private Settings s;

        public SettingsForm()
        {
            s = Program.settings;

            InitializeComponent();
            initGeneral();
            initRecord();
            initProxy();
        }

        private void initGeneral()
        {
            chbRunAtStartUp.Checked = s.getValue(skGen.RunAtStartUp, false);
            chbAlwaysOnTop.Checked = s.getValue(skGen.AlwaysOnTop, false);
            chbMinimizeToTray.Checked = s.getValue(skGen.MinimizeToTray, true);
            chbAlwaysOnTop.Checked = s.getValue(skGen.AlwaysOnTop, false);
            chbShowPopup.Checked = s.getValue(skGen.ShowPopup, true);
            chbNotifyProgram.Checked = s.getValue(skGen.NotifyProgram, true);
            chbCheckUpdates.Checked = s.getValue(skGen.CheckUpdates, true);
        }

        private void initRecord()
        {
            tbRecordPath.Text = s.getValue(skRec.RecordPath, "");
            chbAfterRecordOpenFolder.Checked = s.getValue(skRec.AfterRecordOpenFolder, true);
        }

        private void initProxy()
        {
            
            bool proxyEnabled = s.getValue(skProx.UseProxy, false);

            chbUseProxy.Checked = proxyEnabled;

            tbHost.Text = s.getValue(skProx.Host, "127.0.0.1");
            nudPort.Value = s.getValue<Decimal>(skProx.Port, 8080);
            tbUser.Text = s.getValue(skProx.User, "");
            tbPass.Text = s.getValue(skProx.Pass, "");

            enableProxyComponents(proxyEnabled);
        }

        private void enableProxyComponents(bool enable)
        {
            tbHost.Enabled = enable;
            nudPort.Enabled = enable;
            tbUser.Enabled = enable;
            tbPass.Enabled = enable;
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void setGeneral()
        {
            s.setValue(skGen.RunAtStartUp, chbRunAtStartUp.Checked);
            s.setValue(skGen.AlwaysOnTop, chbRunAtStartUp.Checked);
            s.setValue(skGen.MinimizeToTray, chbMinimizeToTray.Checked);
            s.setValue(skGen.AlwaysOnTop, chbAlwaysOnTop.Checked);
            s.setValue(skGen.ShowPopup, chbShowPopup.Checked);
            s.setValue(skGen.NotifyProgram, chbNotifyProgram.Checked);
            s.setValue(skGen.CheckUpdates, chbCheckUpdates.Checked);
        }

        private void setRecord()
        {
            s.setValue(skRec.RecordPath, tbRecordPath.Text);
            s.setValue(skRec.AfterRecordOpenFolder, chbAfterRecordOpenFolder.Checked);
        }

        private void setProxy()
        {
            s.setValue(skProx.UseProxy, chbUseProxy.Checked);
            s.setValue(skProx.Host, tbHost.Text);
            s.setValue(skProx.Port, nudPort.Value);
            s.setValue(skProx.User, tbUser.Text);
            s.setValue(skProx.Pass, tbPass.Text);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            setGeneral();
            setRecord();
            setProxy();

            s.save();

            DialogResult = DialogResult.OK;
        }

        private void chbUseProxy_CheckedChanged(object sender, EventArgs e)
        {
            enableProxyComponents(chbUseProxy.Checked);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                btnCancel.PerformClick();
                return true;
            }
            if (keyData == Keys.Enter)
            {
                btnSave.PerformClick();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void setSelectRecordDir()
        {
            if (fbdRecordPath.ShowDialog(this) == DialogResult.OK)
            {
                tbRecordPath.Text = fbdRecordPath.SelectedPath;
            }
        }

        private void tbRecordPath_Click(object sender, EventArgs e)
        {
            setSelectRecordDir();
        }

        private void btnRecordPathBrowse_Click(object sender, EventArgs e)
        {
            setSelectRecordDir();
        }
    }
}
