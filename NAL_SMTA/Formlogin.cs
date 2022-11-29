using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace NAL_SMTA
{
    public partial class Formlogin : Form
    {
        ConnectDB ConDB = new ConnectDB();
        Main MainFrom = new Main();
        public string User;
        public string Password;
        public string AppClose = "0";
        public bool StsClose = false;

        public Formlogin()
        {
            InitializeComponent();
            tBPass.PasswordChar = '*';
        }

        public bool openTab()
        {
            return true;
        }

        private void BCancel_Click(object sender, EventArgs e)
        {
            Formlogin settings = new Formlogin();
            this.Close();
            settings.Close();
        }

        public void BLogin_Click(object sender, EventArgs e)
        {
            int Result = CheckLogin();
            if(Result == 1)
            {
                if(AppClose == "0")
                {
                    Main.EnableTab((Owner as Main).MainTab.TabPages[(Owner as Main).MainTab.SelectedIndex = 1], true);

                    var appSettings = ConfigurationManager.AppSettings;
                    string totalSeibam = appSettings.Get("totalSeibam");

                    (Owner as Main).MainFromClose = "1";

                    (Owner as Main).ModeSeiban(totalSeibam);
                    Formlogin settings = new Formlogin();
                    this.Close();
                    settings.Close();
                }
                else
                {
                    (Owner as Main).lbCountQty.Text = "0";
                    Formlogin frmClose = new Formlogin();
                    this.Close();
                    frmClose.Close();

                    Application.Exit();
                }
            }
            else
            {
                tBUser.Text = "";
                tBPass.Text = "";
                tBUser.Focus();
            }
        }
        public int CheckLogin()
        {
            try
            {
                User = tBUser.Text;
                Password = tBPass.Text;
                string Result = ConDB.ChkUserLogIn(User, Password);
                (Owner as Main).PrintResult("Result :: " + Result);
                return Int32.Parse(Result);
            }
            catch
            {
                (Owner as Main).PrintResult("Result :: Error");
                return 0;
            }
        }

        private void TBUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tBPass.Focus();
            }
        }

        private void TBPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bLogin.Focus();
            }
        }
    }
}
