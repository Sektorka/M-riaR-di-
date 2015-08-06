using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace Updater
{
    public partial class updaterForm : Form
    {
        private readonly string VERSION;
        private Thread thread;
        private delegate void UpdateProgressDelegate(int percent);
        private delegate void SetProgressMaxDelegate(int max);
        private delegate void SetFileNameDelegate(string filename);
        private delegate void UpdateTotalProgressDelegate(int percent);
        private delegate void UpdateFailedDelegate(string msg);
        private delegate void SetTotalProgressMaxDelegate(int max);
        private int total;
        private int currFileSize;
        private AutoResetEvent evt = new AutoResetEvent(false);
        private bool Done;

        public updaterForm(string[] args)
        {
            VERSION = args[0];
            InitializeComponent();
            thread = new Thread(Update);
            thread.Start();
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            total += currFileSize;
            evt.Set();
        }

        private void UpdateProgress(int downloaded)
        {
            progress.Value = downloaded;
            lblDownloaded.Text = FormatBytes(downloaded) + " / " + FormatBytes(progress.Maximum);
        }

        private void SetProgressMax(int max)
        {
            progress.Maximum = max;
        }

        private void UpdateTotalProgress(int downloaded)
        {
            progressTotal.Value = total + downloaded;
            lblTotal.Text = "Állapot: " + (int)((double)progressTotal.Value / progressTotal.Maximum * 100) + " %";
        }

        private void SetTotalProgressMax(int max)
        {
            progressTotal.Maximum = max;
        }

        private void SetFileName(string filename)
        {
            lblFile.Text = filename;
        }

        private void OnDownload(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {
                Invoke(new UpdateProgressDelegate(UpdateProgress), (int)e.BytesReceived);
                Invoke(new UpdateTotalProgressDelegate(UpdateTotalProgress), (int)e.BytesReceived);
            }
            catch (Exception) { }
        }

        private void OnCopy(long TotalCopyed, ref bool Cancel)
        {
            try
            {
                Invoke(new UpdateProgressDelegate(UpdateProgress), (int)TotalCopyed);
                Invoke(new UpdateTotalProgressDelegate(UpdateTotalProgress), (int)TotalCopyed);
            }
            catch (Exception) { }
        }

        private void OnCopyCompleted()
        {
            total += currFileSize;
            evt.Set();
        }

        private void Update()
        {
            try
            {
                XmlDocument XMLDoc = new XmlDocument();
                XMLDoc.Load("http://mariaradio.sektor.hu/?version=" + VERSION);
                XmlElement doc = XMLDoc.DocumentElement;

                bool updatable = false;
                List<UFile> files = new List<UFile>();
                int totalFileSize = 0;

                foreach (XmlElement el in doc.ChildNodes)
                {
                    if (el.Name == "updateavailable" && el.InnerText == "true")
                    {
                        updatable = true;
                    }

                    if (el.Name == "files" && el.HasChildNodes)
                    {
                        foreach (XmlElement fe in el.ChildNodes)
                        {
                            UFile file = new UFile(fe.InnerText, int.Parse(fe.Attributes["size"].Value));
                            totalFileSize += int.Parse(fe.Attributes["size"].Value);
                            files.Add(file);
                        }
                    }
                }

                if (updatable)
                {
                    Invoke(new SetTotalProgressMaxDelegate(SetTotalProgressMax), totalFileSize * 2);

                    string dir = Path.GetTempPath() + "\\Maria_Radio\\";

                    if (!File.Exists(dir) || (File.GetAttributes(dir) & FileAttributes.Directory) != FileAttributes.Directory)
                    {
                        Directory.CreateDirectory(dir);
                    }

                    foreach (UFile file in files)
                    {
                        Invoke(new SetFileNameDelegate(SetFileName), file.fileName);
                        Invoke(new SetProgressMaxDelegate(SetProgressMax), file.fileSize);

                        currFileSize = file.fileSize;

                        WebClient webClient = new WebClient();
                        webClient.DownloadFileCompleted += Completed;
                        webClient.DownloadProgressChanged += OnDownload;
                        webClient.DownloadFileAsync(new Uri(file.filePath), dir + file.fileName);
                        evt.WaitOne();
                    }

                    foreach (UFile file in files)
                    {

                        Invoke(new SetFileNameDelegate(SetFileName), file.fileName);
                        Invoke(new SetProgressMaxDelegate(SetProgressMax), file.fileSize);

                        currFileSize = file.fileSize;

                        if (File.Exists(file.fileName))
                        {
                            File.Delete(file.fileName);
                        }

                        Copy copy = new Copy(dir + file.fileName, file.fileName);
                        copy.OnProgressChanged += OnCopy;
                        copy.OnComplete += OnCopyCompleted;
                        copy.StartCopy();
                        evt.WaitOne();
                    }

                    foreach (UFile file in files)
                    {
                        if (File.Exists(dir + file.fileName))
                        {
                            File.Delete(dir + file.fileName);
                        }
                    }

                    Done = true;

                    if (MessageBox.Show(null, "Program frissítés sikeres volt!\r\nMegnyissam a Mária Rádió programot?", "Program frissítő!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        Process.Start("MáriaRádió.exe");
                        Application.Exit();
                    }
                    else
                    {
                        Application.Exit();
                    }


                }
                else
                {
                    MessageBox.Show(null, "Nem érhető el frissítés!\nA frissítő bezárul!", "Program frissítő!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                }
            }
            catch (Exception e)
            {
                Invoke(new UpdateFailedDelegate(UpdateFailed), "A következő frissítés lépett fel frissítés közben: \r\n" + e.Message + "\r\nKérem jelezze a fejlesztő felé.");
            }

        }

        private void UpdateFailed(string msg)
        {
            MessageBox.Show(this, msg, "Program frissítő!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void updaterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Done)
            {
                if (MessageBox.Show(this, "Biztos, hogy megszakítja a frissítést?", "Program frissítő", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    thread.Abort();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        public static string FormatBytes(long bytes)
        {
            const int scale = 1024;
            string[] orders = { "GB", "MB", "KB", "B" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.00} {1}", decimal.Divide(bytes, max), order);

                max /= scale;
            }
            return "0 B";
        }

    }
}
