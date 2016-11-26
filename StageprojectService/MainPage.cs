using Gma.UserActivityMonitor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StageprojectService
{
    public partial class MainPage : Form
    {
        private Keys prevKey = Keys.None;
        private Keys newKey = Keys.None;
        public static MainPage mp = null;
        public MainPage()
        {
            InitializeComponent();

            HookManager.KeyDown -= HookManager_KeyDown;
            HookManager.KeyDown += HookManager_KeyDown;
            mp = this;
          
        }


        private void HookManager_KeyDown(object sender, KeyEventArgs e)
        {
            newKey = e.KeyCode;
            if (newKey == Keys.A && prevKey == Keys.Tab && isHide)
            {
                if (fermer.f != null)
                {
                    this.Show();
                    fermer.f.Close();
                }
                else
                {
                    this.Show();
                    isHide = false;
                }
            }
            else if (newKey == Keys.A && prevKey == Keys.Tab && !isHide)
            {
                this.Hide();
                isHide = true;
            }
            prevKey = newKey;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            isHide = false;
           userControl11.cpasswoed.Password = String.Empty;
            userControl11.ipasswoed.Password = String.Empty;
            userControl11.icpasswoed.Password = String.Empty;
            base.OnVisibleChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            isHide = true;
            this.Hide();
            base.OnClosing(e);
        }

        private bool isHide = true;

        protected override void OnResize(EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                isHide = true;
            }
        }


    }
}
