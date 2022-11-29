using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NAL_SMTA
{
    public partial class MsgFrom : Form
    {
        string ptxtWarning;
        string ptxtWarningII;
        int pSTS;

        public MsgFrom(string txtWarning, string txtWarningII, int STS)
        {
            InitializeComponent();
            ptxtWarning = txtWarning;
            ptxtWarningII = txtWarningII;
            pSTS = STS;
        }
        private void MsgFrom_Load(object sender, EventArgs e)
        {
            buttonOK.Focus();
            //WarningTXT(ptxtWarning, ptxtWarningII, pSTS);

            lbWarning.Text = ptxtWarning;
            lbWarningII.Text = ptxtWarningII;
            if (pSTS == 1)
            {
                lbWarning.ForeColor = Color.Red;
                lbWarningII.ForeColor = Color.Red;
            }
            else
            {
                lbWarning.ForeColor = Color.Green;
                lbWarningII.ForeColor = Color.Green;
            }
        }
        public void WarningTXT(string txtWarning, string txtWarningII, int STS)
        {
            lbWarning.Text = txtWarning;
            lbWarningII.Text = txtWarningII;
            if (STS == 1)
            {
                lbWarning.ForeColor = Color.Red;
                lbWarningII.ForeColor = Color.Red;
            }
            else
            {
                lbWarning.ForeColor = Color.Green;
                lbWarningII.ForeColor = Color.Green;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
