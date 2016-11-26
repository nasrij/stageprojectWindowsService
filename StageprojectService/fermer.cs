using Gma.UserActivityMonitor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StageprojectService
{
    public partial class fermer : Form
    {
        public static fermer f = null;
       
        public fermer()
        {
            InitializeComponent();
           
            f = this;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
           
            f = null;
            MainPage.mp.Show();
           
            base.OnClosing(e);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
           
        }
    }
}
