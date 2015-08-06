using System;
using System.Windows.Forms;

namespace Updater
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length != 1)
            {
                MessageBox.Show(null, "A frissítő program a Mária Rádió programból indítható!", "Program frissítő!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                Application.Run(new updaterForm(args));
            }
        }
    }
}
