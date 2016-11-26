using System;
using System.ComponentModel;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StageprojectService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //BackgroundWorker bk = new BackgroundWorker();
            //bk.DoWork += Bk_DoWork;
            //bk.RunWorkerAsync();
            var exists = System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Length > 1;
            if (exists)
            {
                MessageBox.Show("Programme est ouvert");
                Environment.Exit(0);
            }
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()

            };

            ServiceBase.Run(ServicesToRun);
            
            
        }
        public static void window()
        {
            new MainPage().Show();
        }

        private static void Bk_DoWork(object sender, DoWorkEventArgs e)
        {
            new test().Show();
        }
    }
}
