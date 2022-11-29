using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace NAL_SMTA
{
    public partial class Main : Form
    {
        readonly ConnectDB ConDB = new ConnectDB();
        DataTable DataBarcodPrt;
        DataTable SQLMsg = new DataTable();

        public int TotalSeiban = 0;
        string MainGruopWr;
        string MainSeiban;
        public string MainFromClose = "0";
        public int resultIns = 1;
        string tagModel;
        string DModel;
        string DSeiban;
        string Server;
        string DBNane;
        string User;
        string PW;
        string Process;
        string customer;
        string CLIENT;
        string PathExport;
        string totalSeibam;
        string CheckModelBArcode;
        string FontWR;
        string FontSerial;
        string GroupBarcode;
        string AutoSerial;
        bool AutoSerialChk;
        string vMsgShow = "";
        string Digi_Runing = "0";
        string SetPCB;
        bool SerialRunning = false;
        string DigiControl;
        string StartDigi;
        string cBModelBarcode;
        string ChkBModelBarcode;

        readonly FormPopUp frmPup = new FormPopUp();
        
        public Main()
        {
            InitializeComponent();
            ConDB.GetShift();
            tBPW.PasswordChar = '*';
        }
        private void MainFrom_Load(object sender, EventArgs e)
        {
            TextBox.CheckForIllegalCrossThreadCalls = false;
            AppSetting();
            GetChildAtPointSkip();
            lbCus.Text = customer;
            lbProcess.Text = Process;
            //lbClient.Text = CLIENT;
            tBLotNo.Enabled = false;
            tBQty.Enabled = false;
            tBShift.Enabled = false;
            btPrintTag.Enabled = false;
            tBBarcode.Enabled = false;
            tBBarcodeLH.Enabled = false;
            tBBarcodeRH.Enabled = false;
            tBCover.Enabled = false;

            tBShift.Text = ConDB.MainShift;
            tBUser.Focus();

            cbSeibam.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSeibam.SelectedIndex = 0;
            UpdateFontdGV();

            cbSeibam.SelectedIndex = cbSeibam.FindStringExact(totalSeibam + " Tracking");
            //cbSeibam.Enabled = false;
            GenGroupWR();

            ModeSeiban(totalSeibam);
            if (CheckModelBArcode == "2" & totalSeibam == "2")
            {
                rBChkRL.Checked = true;
            }
            else if (CheckModelBArcode == "3" & totalSeibam == "2")
            {
                rBChkRLN.Checked = true;
            }
            TextBox.CheckForIllegalCrossThreadCalls = false;

            btPrtTag.Hide();
            btPrintTagNew.Hide();
            btExportFinal.Hide();

            if (customer == "Transtron" & Process.Substring(0,5) == "Final")
            {
                btPrtTag.Show();
                //btPrintTagNew.Show();
                btExportFinal.Show();
            }

            tBServer.Text = Server;
            tBDB.Text = DBNane;
            tBUsers.Text = User;
            tBPW.Text = PW;
            tBCus.Text = customer;
            //tBClient.Text = CLIENT;
            tBProcess.Text = Process;
            tBPathExport.Text = PathExport;

            tBFontWR.Text = FontWR;
            tBFontSerial.Text = FontSerial;

            tBSetSeiban.Text = totalSeibam;
            tBDigiControl.Text = DigiControl;

            tBChkModelBarcode.Text = cBModelBarcode;
            tBStartDigi.Text = StartDigi;

            switch (CheckModelBArcode)
            {
                case "0":
                    rBNotMatch.Checked = true;
                    break;
                case "1":
                    rBChkBarcode.Checked = true;
                    break;
                case "2":
                    rBChkRL.Checked = true;
                    break;
                case "3":
                    rBChkRLN.Checked = true;
                    break;
            }

            lbCover.Hide();
            tBCover.Hide();

            if (Process == "ASSY" || Process == "BRACKET" || customer == "Yaskawa" || Process == "ASSY_FCT")
            {
                if (Process == "BRACKET")
                {
                    lbCover.Text = "BRACKET :";
                }

                if (customer == "Yaskawa")
                {
                    lbCover.Text = "Model :";
                }

                lbCover.Show();
                tBCover.Show();
            }

            SQLMsg.Columns.Add("Message", typeof(string));
            if (tBDigiControl.Text == "") {
                tBDigiControl.Text = "0";
            };
        }
        private void GetChildAtPointSkip()
        {
            string MachineName1 = Environment.MachineName;
            lbClient.Text = MachineName1;
            tBClient.Text = MachineName1;
        }
        void TBUser_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string txtUser = tBUser.Text;
                ConDB.CheckUserLogin(txtUser);

                string userNamr = GetUser();

                if (userNamr != "")
                {
                    this.tBUser.Text = GetUser();
                    this.tBUser.Enabled = false;
                    this.btPrintTag.Enabled = true;
                    this.tBLotNo.Enabled = true;
                    this.tBLotNo.Focus();
                    ShowStatusOK();
                    PrintResult(userNamr + " Login success!!!");
                }
                else
                {
                    this.tBUser.Text = "";
                    ShowStatusNG();
                    MsgFroms("Please try again.", "", 1);
                    //MessageBox.Show("รหัสไม่ถูกต้อง");
                }
            }
        }
        private string GetUser()
        {
            string userNamr = ConDB.UserName;
            return userNamr;
        }
        public void CheckCountSeiban()
        {
            int chkSeiban = Int32.Parse(lbCountSeiban.Text);
            //var appSettings = ConfigurationManager.AppSettings;
            //string totalSeibam = appSettings.Get("totalSeibam");
            if (chkSeiban < TotalSeiban)
            {
                string Result = "Tracking Check : " + chkSeiban;
                //PrintResult(Result);
            }
            else
            {
                btPrintTag.Enabled = false;
                tBQty.Enabled = false;
                tBLotNo.Enabled = false;
                bool chkRL = rBChkRL.Checked;
                bool ChkRLN = rBChkRLN.Checked;
                if (chkRL || ChkRLN)
                {
                    if (dGVBarcoode.Rows.Count > 0)
                    {
                        string pSerialChk = dGVBarcoode.Rows[0].Cells[1].Value.ToString();
                        int indexRH = pSerialChk.IndexOf("R");

                        if (indexRH > 0)
                        {
                            tBBarcodeRH.Enabled = true;
                            tBBarcodeRH.Focus();
                        }
                        else
                        {
                            tBBarcodeLH.Enabled = true;
                            tBBarcodeLH.Focus();
                        }
                    }
                    else
                    {
                        tBBarcodeRH.Enabled = true;
                        tBBarcodeRH.Focus();
                    }

                }
                else
                {
                    tBBarcode.Enabled = true;
                    tBBarcode.Focus();
                    //PrintResult("Focus");
                }
                ConDB.GetShift();
            }
        }
        //Input Lot
        private void TBLotNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string dateLot = DateTime.Now.ToString("MM");
                bool isChkQrTag = cBWithTagQr.Checked;
                string Lot;
                string Qty;
                if (isChkQrTag)
                {
                    string QrTag = tBLotNo.Text;
                    try
                    {
                        string[] authorsList = QrTag.Split(',');
                        Lot = authorsList[5];
                        tagModel = authorsList[1];
                        tBLotNo.Text = dateLot + "-" + Lot;
                        tBLotNo.Enabled = false;
                        PrintResult("Tag Model :" + tagModel);

                        Qty = authorsList[3];
                        tBQty.Text = Qty;
                        tBQty.Enabled = false;
                        btPrintTag.Enabled = true;
                        btPrintTag.Focus();
                    }
                    catch
                    {
                        tBLotNo.Text = "";
                        tBLotNo.Focus();
                        tBQty.Text = "";
                        tBQty.Enabled = false;
                    }
                }
                else
                {
                    Lot = tBLotNo.Text;
                    if (Lot != "")
                    {
                        string LotYesr = DateTime.Now.ToString("yy");
                        if ((customer == "SINFONIA" || customer == "Sinfonia") & (Process == "Final" || Process == "FINAL" || Process == "BRACKET" || Process == "Bracket"))
                        {
                            tBLotNo.Text = Lot + "-" + LotYesr;
                            tBLotNo.Enabled = false;
                            //tBQty.Enabled = true;
                            //tBQty.Focus();
                            btPrintTag.Enabled = true;
                            btPrintTag.Focus();
                        }
                        else
                        {
                            tBLotNo.Text = Lot;
                            tBLotNo.Enabled = false;
                            tBQty.Enabled = true;
                            tBQty.Focus();
                        }
                    }
                    else
                    {
                        tBLotNo.Enabled = true;
                        tBLotNo.Focus();
                        tBQty.Text = "";
                        tBQty.Enabled = false;
                    }
                }
            }
        }
        private void TBQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                int value;

                bool IsNumber = int.TryParse(tBQty.Text.Trim(), out value);

                if (IsNumber)
                {
                    tBQty.Enabled = false;
                    btPrintTag.Enabled = true;
                    btPrintTag.Focus();
                    ShowStatusOK();
                }
                else
                {
                    tBQty.Text = "";
                    tBQty.Focus();
                    ShowStatusNG();
                }
            }
        }
        //---------------GetDataWR------------------------\\
        private void GenGroupWR()
        {
            string guoupWRDecoded = DateTime.Now.ToString("yyMMddhhmmssmmm");
            //string base64Decode;
            //string guoupWREncoded;
            byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(guoupWRDecoded);
            //MainGruopWr = System.Convert.ToBase64String(data);
            //base64Decode = Encoding.Default.GetString(Convert.FromBase64String(MainGruopWr));
            try
            {
                string GetGroupWR = dGVDataWR.Rows[0].Cells[5].Value.ToString();
                MainGruopWr = GetGroupWR;
            }
            catch
            {
                MainGruopWr = System.Convert.ToBase64String(data);
            }

            //PrintResult(MainGruopWr+ " : "+guoupWRDecoded);
        }
        public void GetDataWR(string Lot, string Process, string SeibanWR)
        {
            try
            {
                DataTable dtRecord = ConDB.GetDataWR(Lot, Process, SeibanWR);
                dGVDataWR.ReadOnly = true;
                dGVDataWR.DataSource = dtRecord;
                foreach (DataGridViewColumn dcol in dGVDataWR.Columns)
                {
                    dcol.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
                string cSeiban = dGVDataWR.Rows.Count.ToString();
                lbCountSeiban.Text = cSeiban;
                CheckCountSeiban();

                dGVDataWR.Cursor = Cursors.Hand;

                dGVDataWR.Columns[0].HeaderText = "Model";
                dGVDataWR.Columns[1].HeaderText = "SEIBAN";
                dGVDataWR.Columns[2].HeaderText = "Qty";
                dGVDataWR.Columns[3].HeaderText = "Lot No";
                dGVDataWR.Columns[4].HeaderText = "Group";
                dGVDataWR.Columns[5].HeaderText = "Lot";

                dGVDataWR.Columns[5].Visible = false;
                dGVDataWR.Columns[4].Visible = false;

                lbCountQty.Text = dGVDataWR.Rows.Count.ToString();
                DModel = dGVDataWR.Rows[0].Cells[0].Value.ToString();
                DSeiban = dGVDataWR.Rows[0].Cells[1].Value.ToString();

                int sumTotalQty = 0;
                try
                {
                    string Seiban = "";
                    string Conma = "";
                    for (int i = 0; i < dGVDataWR.Rows.Count; ++i)
                    {
                        sumTotalQty += Convert.ToInt32(dGVDataWR.Rows[i].Cells[2].Value);
                        if (i > 0)
                        {
                            Conma = ",";
                        }
                        Seiban += Conma + dGVDataWR.Rows[i].Cells[1].Value.ToString();
                    }
                    lbCountQty.Text = sumTotalQty.ToString();
                }
                catch
                {
                    lbCountQty.Text = "0";
                }
                bool isChkQrTag = cBWithTagQr.Checked;
                string SeibanModel = dGVDataWR.Rows[0].Cells[0].Value.ToString();
                if (isChkQrTag)
                {
                    if (tagModel.Substring(0, 10) == SeibanModel.Substring(0, 10))
                    {
                        GenGroupWR();
                        getDataSrinal();
                    }
                    else
                    {
                        MsgFroms("Tag notmatch W/R", "Please check agian", 1);
                        PrintResult("SB Modwl: " + SeibanModel);
                        dGVDataWR.Columns.Clear();
                        btPrintTag.Enabled = true;
                        btPrintTag.Focus();
                        GenGroupWR();
                    }
                }
                else
                {
                    getDataSrinal();
                }

                GetDataLot(Process);
                if (AutoSerialChk)
                {
                    getDataMSSerial(SeibanModel);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Log Error");
            }
        }
        public void GetDataBarcode()
        {
            var appSettings = ConfigurationManager.AppSettings;
            string Process = appSettings.Get("Process");
            string Lot = tBLotNo.Text;
            string GetModel;

            if(dGVDataWR.Rows.Count > 0)
            {
                GetModel = dGVDataWR.Rows[0].Cells[0].Value.ToString();
            }
            else
            {
                GetModel = "";
            }
            string Seiban = "";
            string Conma = "";
            try
            {
                for (int i = 0; i < dGVDataWR.Rows.Count; ++i)
                {
                    if (i > 0)
                    {
                        Conma = ",";
                    }
                    Seiban += Conma + dGVDataWR.Rows[i].Cells[1].Value.ToString();
                }
            }
            catch
            {
                Seiban = "";
            }

            //PrintResult("BC: "+Process+" : "+Lot + " : " + GetModel + " : " + Seiban);
            try
            {
                DataTable dtRecord = ConDB.MAIN_SEC_DATA_STBL(Process, Lot, GetModel, Seiban);
                dGVBarcoode.ReadOnly = true;
                dGVBarcoode.DataSource = dtRecord;
                dGVBarcoode.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dGVBarcoode.Cursor = Cursors.Hand;

                DgvBarcodeGetHead(GetModel);

                int columnz = dGVBarcoode.Columns.Count;
                int rows = dGVBarcoode.RowCount;
                DataBarcodPrt = dtRecord;


                foreach (DataGridViewColumn dcol in dGVBarcoode.Columns)
                {
                    dcol.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

                int columnMain = dGVBarcoode.Columns.Count;
                for (int colM = 0; colM < columnMain; colM++)
                {
                    dGVBarcoode.Columns[colM].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                lbStatusOK.Text = "0";
                lbStatusNG.Text = "0";
                lbStatusMIX.Text = "0";
                //lbStatusNoData.Text = "0";

                int columnSTS = columnz - 3;
                int StatusOKTotal = 0;
                int StatusNGTotal = 0;
                int StatusMIXTotal = 0;
                //int StatusNDTotal = 0;
                for (int counter = 0; counter < (dGVBarcoode.Rows.Count); counter++)
                {
                    string vStatus = dGVBarcoode.Rows[counter].Cells[columnSTS].Value.ToString();
                    if (Process == "ASSY" || Process == "BRACKET" || Process == "ASSY_FCT")
                    {
                        if (vStatus.Trim() != null && vStatus.Trim() != "NG")
                        {
                            StatusOKTotal += 1;
                            lbStatusOK.Text = StatusOKTotal.ToString();
                        }
                    }
                    else
                    {
                        if (vStatus.Trim() != null && vStatus.Trim() == "OK")
                        {
                            StatusOKTotal += 1;
                            lbStatusOK.Text = StatusOKTotal.ToString();
                        }
                    }

                    if (vStatus.Trim() != null && vStatus.Trim() == "NG")
                    {
                        StatusNGTotal += 1;
                        lbStatusNG.Text = StatusNGTotal.ToString();
                    }

                    if (vStatus.Trim() != null && vStatus.Trim() == "MIX")
                    {
                        StatusMIXTotal += 1;
                        lbStatusMIX.Text = StatusMIXTotal.ToString();
                    }

                    //if (vStatus.Trim() != null && vStatus.Trim() == "No Data")
                    //{
                    //    StatusNDTotal += 1;
                    //    lbStatusNoData.Text = StatusNDTotal.ToString();
                    //}
                }

                if (rows > 0)
                {
                    string vStsShow = dGVBarcoode.Rows[0].Cells[columnSTS].Value.ToString();
                    if (Process == "ASSY" || Process == "BRACKET" || Process == "ASSY_FCT")
                    {
                        if (vStsShow.Trim() != null && vStsShow.Trim() != "NG")
                        {
                            ShowStatusOK();
                            PrintResult("Status : " + vStsShow.Trim());
                        }
                        else if (vStsShow.Trim() != null && vStsShow.Trim() == "NG")
                        {
                            ShowStatusNG();
                            PrintResult("Status : " + vStsShow.Trim());
                        }
                    }
                    else
                    {
                        if (vStsShow.Trim() != null && vStsShow.Trim() == "OK")
                        {
                            ShowStatusOK();
                            PrintResult("Status : " + vStsShow.Trim());
                        }
                        else
                        {
                            ShowStatusNG();
                            PrintResult("Status : " + vStsShow.Trim());
                        }

                        //if (vStsShow.Trim() != null && vStsShow.Trim() == "NG")
                        //{
                        //    ShowStatusNG();
                        //    PrintResult("Status : NG");
                        //}
                        //else if (vStsShow.Trim() != null && vStsShow.Trim() == "MIX")
                        //{
                        //    ShowStatusNG();
                        //    PrintResult("Status : MIX");
                        //}
                        //else if (vStsShow.Trim() != null && vStsShow.Trim() == "No Data")
                        //{
                        //    ShowStatusNG();
                        //    PrintResult("Status : No Data");
                        //}
                    }
                }
            }
            catch (Exception e)
            {
                PrintResult("SQL : Load Data Error: " + e);
                ShowStatusNG();
                MsgFroms("Can notconnect Server", "Please call (216,230).", 1);
                //MessageBox.Show("BC :Load Data Error", "Warning");
            }
            CheckLotComplete();
        }
        private void GetDataLot(string Process)
        {
            try
            {
                string Seiban = "";
                string Conma = "";
                try
                {
                    for (int i = 0; i < dGVDataWR.Rows.Count; ++i)
                    {
                        if (i > 0)
                        {
                            Conma = ",";
                        }
                        Seiban += Conma + dGVDataWR.Rows[i].Cells[1].Value.ToString();
                    }
                }
                catch
                {
                    Seiban = "";
                }

                DataTable dtRecord = ConDB.GetDataLot(Seiban, Process);
                dGVLotDetail.ReadOnly = true;
                dGVLotDetail.DataSource = dtRecord;
                foreach (DataGridViewColumn dcol in dGVLotDetail.Columns)
                {
                    dcol.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Log Error");
            }
        }
        private void DgvBarcodeGetHead(string GetModel)
        {
            int columnz = dGVBarcoode.Columns.Count;
            //int i = 1;
            for (int col = 2; col < columnz - 2; col++)
            {
                string[] HeaderName = new string[20];
                string[] StepHeader = new string[20];
                HeaderName[col] = dGVBarcoode.Columns[col].HeaderText;
                StepHeader[col] = ConDB.GetHeader(ref GetModel, ref HeaderName[col]);

                dGVBarcoode.Columns[col].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dGVBarcoode.Columns[col].HeaderText = StepHeader[col];
            }
            try
            {
                dGVBarcoode.Columns[columnz - 1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            catch
            {
                PrintResult("Header : Load Data Error");
            }
        }
        private void DgvDataBarcode_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string dgvFont = appSettings.Get("dgvFontMain");
            dGVBarcoode.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", Int32.Parse(dgvFont), FontStyle.Bold);
            dGVBarcoode.DefaultCellStyle.Font = new Font("Tahoma", Int32.Parse(dgvFont));

            int columnz = dGVBarcoode.Columns.Count;
            for (int col = 2; col < columnz - 2; col++)
            {
                string vStatus = dGVBarcoode.Rows[e.RowIndex].Cells[col].Value.ToString();
                //PrintResult(vStatus + '-' + col + '-' + e.RowIndex);
                if (vStatus.Trim() != null && vStatus.Trim() == "OK")
                {
                    dGVBarcoode.Rows[e.RowIndex].Cells[col].Style.ForeColor = Color.Green;
                }
                else if (vStatus.Trim() != "OK" && vStatus.Length <= 7)
                {
                    dGVBarcoode.Rows[e.RowIndex].Cells[col].Style.ForeColor = Color.Red;
                }
                else
                {
                    dGVBarcoode.Rows[e.RowIndex].Cells[col].Style.ForeColor = Color.Black;
                }
            }
        }
        //---------------MENU------------------------\\
        private void NewEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string vMainQty = lbCountQty.Text;
            string vTotalOK = lbStatusOK.Text;
            bool vUnlock = cBUnlockMenu.Checked;
            if (vMainQty == "0" || vMainQty == vTotalOK || vUnlock)
            {
                ConDB.GetShift();
                tBLotNo.Text = "";
                tBQty.Text = "";
                lbCountSeiban.Text = "0";
                tBUser.Enabled = false;
                tBLotNo.Enabled = true;
                tBLotNo.Focus();
                btPrintTag.Enabled = false;
                tBQty.Enabled = false;
                dGVDataWR.Columns.Clear();
                dGVBarcoode.Columns.Clear();
                //rTBResult.Text = "";
                lbStatusOK.Text = "0";
                lbStatusNG.Text = "0";
                lbStatusMIX.Text = "0";
                //lbStatusNoData.Text = "0";
                cBUnlockMenu.Checked = false;
                lbCountQty.Text = "0";
                tBBarcode.Enabled = false;
                tBBarcodeLH.Enabled = false;
                tBBarcodeRH.Enabled = false;
                tBCover.Enabled = false;
                SerialRunning = false;
                lbStatus.Text = "-";
                lbStatus.BackColor = Color.Gray;
                GenGroupWR();
            }
            else if (Int32.Parse(vMainQty) != Int32.Parse(vTotalOK))
            {
                MsgFroms("Qty not enough.", "Please call leader.", 1);
            }
        }
        private void EditQtyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string vMainQty = lbCountQty.Text;
            string vTotalOK = lbStatusOK.Text;
            bool vUnlock = cBUnlockMenu.Checked;
            if (vTotalOK == "0" || vMainQty == vTotalOK || vUnlock)
            {
                ConDB.GetShift();
                tBQty.Enabled = true;
                tBQty.Enabled = true;
                tBQty.Focus();
                lbCountSeiban.Text = "0";
                //rTBResult.Text = "";
                lbStatusOK.Text = "0";
                lbStatusNG.Text = "0";
                lbStatusMIX.Text = "0";
                //lbStatusNoData.Text = "0";
                lbCountQty.Text = "0";
                dGVDataWR.Columns.Clear();
                dGVBarcoode.Columns.Clear();
                cBUnlockMenu.Checked = false;
                tBBarcode.Enabled = false;
                tBBarcodeLH.Enabled = false;
                tBBarcodeRH.Enabled = false;
                tBCover.Enabled = false;
                SerialRunning = false;
                lbStatus.Text = "-";
                lbStatus.BackColor = Color.Gray;
                GenGroupWR();
            }
            else if (Int32.Parse(vMainQty) != Int32.Parse(vTotalOK))
            {
                MsgFroms("Qty not enough.", "Please call leader.", 1);
            }
        }
        private void LogOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string vMainQty = lbCountQty.Text;
            string vTotalOK = lbStatusOK.Text;
            bool vUnlock = cBUnlockMenu.Checked;
            if (vMainQty == "0" || vMainQty == vTotalOK || vUnlock)
            {
                this.tBUser.Text = "";
                this.tBLotNo.Text = "";
                this.lbCountSeiban.Text = "0";
                this.tBUser.Enabled = true;
                this.tBLotNo.Enabled = false;
                this.btPrintTag.Enabled = false;
                this.tBQty.Enabled = false;
                dGVDataWR.Columns.Clear();
                cBUnlockMenu.Checked = false;
                tBBarcode.Enabled = false;
                tBBarcodeLH.Enabled = false;
                tBBarcodeRH.Enabled = false;
                tBCover.Enabled = false;
            }
            else if (Int32.Parse(vMainQty) != Int32.Parse(vTotalOK))
            {
                MsgFroms("Qty not enough.", "Please call leader.", 1);
            }
        }
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //---------------MENU------------------------\\
        private void UpdateFontdGV()
        {
            var appSettings = ConfigurationManager.AppSettings;
            string dgvFontSeiban = appSettings.Get("dgvFontSeiban");
            //string dgvFont = appSettings.Get("dgvFontMain");

            dGVDataWR.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", Int32.Parse(dgvFontSeiban), FontStyle.Bold);
            dGVDataWR.DefaultCellStyle.Font = new Font("Tahoma", Int32.Parse(dgvFontSeiban));
            //string dgvFont = appSettings.Get("dgvFontMain");
            //dGVBarcoode.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", Int32.Parse(dgvFont), FontStyle.Bold);
            //dGVBarcoode.DefaultCellStyle.Font = new Font("Tahoma", Int32.Parse(dgvFont));
        }
        private void InsDataBarcode(string Barcode, string Seiban)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string Process = appSettings.Get("Process");
            string Lot = tBLotNo.Text;
            string Emp = ConDB.UserID;
            string ShiftID = ConDB.MainShiftID;
            string Shift = ConDB.MainShift;
            string CLIENT = appSettings.Get("CLIENT");
            string[] result;
            string type = CheckType();
            string MGroupWr = MainGruopWr;
            string Cover = tBCover.Text;
            //PrintResult(Process +'-'+ Barcode + '-' + Lot);
            //PrintResult(ShiftID + '-' + Shift + '-' + Emp);
            //PrintResult(Seiban + '-' + CLIENT + '-' + type);
            try
            {
                result = ConDB.InMAIN_INS_STBL(Process, Barcode, Lot, ShiftID, Shift, Emp, Seiban, CLIENT, type, Cover);

                if (result[1] == "1")
                {
                    resultIns = 1;
                    //ShowStatusOK();
                    ShowStatus();
                    PrintResult("SQL : " + result[0]);
                }
                else if (result[1] == "0")
                {
                    resultIns = 0;
                    PrintResult("SQL : " + result[0]);
                    AddSQLMsg(result[0]);
                    //MsgFroms(result[0],"", 1);
                    //ShowStatusNG();


                    ShowStatus();
                }
            }
            catch (Exception e)
            {
                PrintResult(e.ToString());
                AddSQLMsg("Please check Barcode");
                MsgFroms("Please check Barcode", "", 1);
                ShowStatusNG();
            }
        }
        private void TBWR_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string txtWR = btPrintTag.Text;
                string[] detail;

                detail = ConDB.GetDetialWR(txtWR);

                MainSeiban = detail[0];
                if (cBLotWithTag.Checked)
                {
                    string vLot = tBLotNo.Text;
                    tBLotNo.Text = MainSeiban + "-" + vLot;
                }

                if ((customer == "SINFONIA" || customer == "Sinfonia") & (Process == "Final" || Process == "FINAL" || Process == "BRACKET" || Process == "Bracket"))
                {
                    string vQty = ConDB.GetQtySinfonia(MainSeiban);
                    tBQty.Text = vQty;
                }

                string ChkQtyTest = tBQty.Text;

                if (ChkQtyTest == "")
                {
                    ShowStatusNG();
                    MsgFroms("โปรดใส่จำนวน", "", 1);
                    btPrintTag.Text = "";
                    btPrintTag.Enabled = false;
                    tBQty.Enabled = true;
                    tBQty.Focus();
                }
                else
                {

                    var appSettings = ConfigurationManager.AppSettings;
                    string Process = appSettings.Get("Process");
                    string CLCLIENT = appSettings.Get("CLIENT");

                    string Lot = tBLotNo.Text;
                    string itemCD = detail[1];
                    int ChkitemCD = (itemCD.Trim()).Length;
                    string shiftID = ConDB.MainShiftID;
                    string Emp = ConDB.UserName;
                    string qty = tBQty.Text;
                    string result = ConDB.Result;

                    //PrintResult(MainSeiban+"----"+ itemCD);

                    if (MainSeiban != "Error" && itemCD != "Error")
                    {
                        try
                        {
                            result = ConDB.InMAIN_INS_WR_CRL(Process, shiftID, Lot, Emp, MainSeiban, MainGruopWr, itemCD, CLCLIENT, qty);
                            PrintResult("WR : " + result);
                            ShowStatusOK();
                        }
                        catch
                        {
                            PrintResult("WR : Fail");
                            ShowStatusNG();
                            MsgFroms("Can't record W/R", "", 1);
                            //MessageBox.Show("ไม่สามารถบันทึกข้อมูล W/R", "Even Log");
                        }

                        lbCountQty.Text = ConDB.ShowTotalQty(Process, Lot); ;

                        GetDataWR(Lot, Process, MainSeiban);

                        btPrintTag.Text = "";
                        btPrintTag.Focus();
                    }
                    else
                    {
                        PrintResult(result);
                        btPrintTag.Text = "";
                        btPrintTag.Focus();
                        ShowStatusNG();
                        PrintResult("WR : Qrcode W/R Fail");
                        MsgFroms("Please check W/R. ", "", 1);
                        //MessageBox.Show("โปรดตรวจสอบ W/R ", "Warning");
                    }

                    CheckLotCompleteII();
                }
            }
        }
        private void TBBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string vDigiControl = tBDigiControl.Text;
                string vsB = tBBarcode.Text;
                int iDigiLen = vsB.Length;
                int iDigiControl = Int32.Parse(vDigiControl);

                if (iDigiLen > iDigiControl && iDigiControl > 0)
                {
                    string vB = tBBarcode.Text;
                    tBBarcode.Text = vB.Substring(0, iDigiControl);
                }

                if (Process == "ASSY" || Process == "BRACKET" || customer == "Yaskawa" || Process == "ASSY_FCT")
                {
                    tBBarcode.Enabled = false;
                    tBCover.Enabled = true;
                    tBCover.Focus();
                }
                else
                {
                    tBBarcode.Enabled = false;
                    if (bkwSerialINS.IsBusy != true)
                    {
                        SQLMsg.Clear();
                        vMsgShow = "";
                        bkwSerialINS.RunWorkerAsync();
                    }
                }

            }
        }
        private void ChkBarcode()
        {
            string svBar,sModel;
            string vBar = tBBarcode.Text;
            int vStartDigi = int.Parse(tBStartDigi.Text);
            int vChkModelBArcode = int.Parse(tBChkModelBarcode.Text);

            svBar = vBar.Substring(0, vChkModelBArcode);

            sModel = dGVDataWR.Rows[0].Cells[0].Value.ToString();

            if (cBChkModelBarcode.Checked == true)
            {
                if (svBar == sModel)
                {
                    if (bkwSerialINS.IsBusy != true)
                    {
                        SQLMsg.Clear();
                        vMsgShow = "";
                        bkwSerialINS.RunWorkerAsync();
                    }
                }
                else
                {
                    MsgFroms("Serial not match Model", "Please check Serial", 1);
                    tBBarcode.Text = "";
                    tBBarcode.Enabled = true;
                }
            }
            else
            {
                if (bkwSerialINS.IsBusy != true)
                {
                    SQLMsg.Clear();
                    vMsgShow = "";
                    bkwSerialINS.RunWorkerAsync();
                }
            }
        }
        private void tBCover_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tBCover.Enabled = false;
                if (bkwCover.IsBusy != true)
                {
                    SQLMsg.Clear();
                    vMsgShow = "";
                    bkwCover.RunWorkerAsync();
                }
            }
        }
        private void TBBarcodeRH_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bool ChkRLN = rBChkRLN.Checked;
                bool ChkRL = rBChkRL.Checked;
                if (ChkRLN)
                {
                    string vModelRH = dGVDataWR.Rows[0].Cells[0].Value.ToString();
                    int indexRH = vModelRH.IndexOf("(RH)");
                    int indexRight = vModelRH.IndexOf("(RIGHT)");

                    string vBarcode = tBBarcodeRH.Text;
                    int indexBarcodeRH = vBarcode.IndexOf("RH");

                    if ((indexRH + indexRight) > 0 && indexBarcodeRH > 0)
                    {
                        tBBarcodeRH.Enabled = false;
                        if (bkwRH.IsBusy != true)
                        {
                            SQLMsg.Clear();
                            vMsgShow = "";
                            bkwRH.RunWorkerAsync();
                        }
                    }
                    else
                    {
                        MsgFroms("PCB not (RH)", "Please check Serial", 1);
                        tBBarcodeRH.Text = "";
                        tBBarcodeRH.Enabled = true;
                        tBBarcodeRH.Focus();
                        CheckLotCompleteII();
                    }
                }
                else if(ChkRL)
                {
                    string vModelRH = dGVDataWR.Rows[0].Cells[0].Value.ToString();
                    int indexRH = vModelRH.IndexOf("(RH)");
                    int indexRight = vModelRH.IndexOf("(RIGHT)");

                    string vBarcode = tBBarcodeRH.Text;
                    int indexBarcodeRH = vBarcode.IndexOf("R");

                    if ((indexRH + indexRight > 0) & indexBarcodeRH > 0)
                    {
                        tBBarcodeRH.Enabled = false;
                        if (bkwRH.IsBusy != true)
                        {
                            SQLMsg.Clear();
                            vMsgShow = "";
                            bkwRH.RunWorkerAsync();
                        }
                    }
                    else
                    {
                        MsgFroms("PCB not (RH)", "Please check Serial", 1);
                        tBBarcodeRH.Text = "";
                        tBBarcodeRH.Enabled = true;
                        tBBarcodeRH.Focus();
                        CheckLotCompleteII();
                    }
                }
                
            }
        }
        private void TBBarcodeLH_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bool ChkRLN = rBChkRLN.Checked;
                bool ChkRL = rBChkRL.Checked;

                if(ChkRLN)
                {
                    string vModelLH = dGVDataWR.Rows[1].Cells[0].Value.ToString();
                    int indexLH = vModelLH.IndexOf("(LH)");
                    int indexLeft = vModelLH.IndexOf("(LEFT)");

                    string vBarcode = tBBarcodeLH.Text;
                    int indexBarcodeLH = vBarcode.IndexOf("LH");

                    if ((indexLH + indexLeft) > 0 & indexBarcodeLH > 0)
                    {
                        tBBarcodeLH.Enabled = false;
                        if (bkwLH.IsBusy != true)
                        {
                            SQLMsg.Clear();
                            vMsgShow = "";
                            bkwLH.RunWorkerAsync();
                        }
                    }
                    else
                    {
                        MsgFroms("PCB not (LH)", "Please check Serial", 1);
                        tBBarcodeLH.Text = "";
                        tBBarcodeLH.Enabled = true;
                        tBBarcodeLH.Focus();
                        CheckLotCompleteII();
                    }
                }
                else if (ChkRL)
                {
                    string vModelLH = dGVDataWR.Rows[1].Cells[0].Value.ToString();
                    int indexLH = vModelLH.IndexOf("(LH)");
                    int indexLeft = vModelLH.IndexOf("(LEFT)");

                    string vBarcode = tBBarcodeLH.Text;
                    int indexBarcodeLH = vBarcode.IndexOf("L");

                    if ((indexLH + indexLeft) > 0 & indexBarcodeLH > 0)
                    {
                        tBBarcodeLH.Enabled = false;
                        if (bkwLH.IsBusy != true)
                        {
                            SQLMsg.Clear();
                            vMsgShow = "";
                            bkwLH.RunWorkerAsync();
                        }
                    }
                    else
                    {
                        MsgFroms("PCB not (LH)", "Please check Serial", 1);
                        tBBarcodeLH.Text = "";
                        tBBarcodeLH.Enabled = true;
                        tBBarcodeLH.Focus();
                        CheckLotCompleteII();
                    }
                }
                
            }
        }
        private string CheckType()
        {
            bool ChkBarcode = rBChkBarcode.Checked;
            bool ChkRHLH = rBChkRL.Checked;
            if (ChkBarcode)
            {
                return "B";
            }
            else if (ChkRHLH)
            {
                return "A";
            }
            else
            {
                return "A";
            }
        }
        private void CheckLotComplete()
        {
            string TotalQty = lbCountQty.Text;
            string TotalQtyOK = lbStatusOK.Text;

            if (TotalQty == TotalQtyOK && TotalQty != "0")
            {
                tBBarcode.Enabled = false;
                tBBarcodeLH.Enabled = false;
                tBBarcodeRH.Enabled = false;
                tBCover.Enabled = false;
                ShowStatusOK();
                MsgFroms("Complete!!!", "Select Menu New Lot", 0);
                //MessageBox.Show("สแกนครบตามจำนวน!!!");
            }
        }
        private void CheckLotCompleteII()
        {
            string TotalQty = lbCountQty.Text;
            string TotalQtyOK = lbStatusOK.Text;

            if (Int32.Parse(TotalQty) <= Int32.Parse(TotalQtyOK) && TotalQty != "0")
            {
                tBBarcode.Enabled = false;
                tBBarcodeLH.Enabled = false;
                tBBarcodeRH.Enabled = false;
                tBCover.Enabled = false;
            }
        }
        public void PrintResult(string RText)
        {
            //rTBResult.Clear();

            //string dateLog = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            //rTBResult.Text += dateLog + "----> " + RText;
            //rTBResult.Text += Environment.NewLine;
            //rTBResult.Select(rTBResult.Text.Length - 1, 0);
            //rTBResult.ScrollToCaret();
        }
        private void CbSeibam_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ChkSeiban = cbSeibam.SelectedItem.ToString();
            switch (ChkSeiban)
            {
                case "1 Tracking":
                    TotalSeiban = 1;
                    tBSetSeiban.Text = "1";
                    break;
                case "2 Tracking":
                    TotalSeiban = 2;
                    tBSetSeiban.Text = "2";
                    break;
                case "3 Tracking":
                    TotalSeiban = 3;
                    tBSetSeiban.Text = "3";
                    break;
                case "4 Tracking":
                    TotalSeiban = 4;
                    tBSetSeiban.Text = "4";
                    break;
                case "5 Tracking":
                    TotalSeiban = 5;
                    tBSetSeiban.Text = "5";
                    break;
                case "6 Tracking":
                    TotalSeiban = 6;
                    tBSetSeiban.Text = "6";
                    break;
                case "7 Tracking":
                    TotalSeiban = 7;
                    tBSetSeiban.Text = "7";
                    break;
                case "8 Tracking":
                    TotalSeiban = 8;
                    tBSetSeiban.Text = "8";
                    break;
                case "9 Tracking":
                    TotalSeiban = 9;
                    tBSetSeiban.Text = "9";
                    break;
                case "10 Tracking":
                    TotalSeiban = 10;
                    tBSetSeiban.Text = "10";
                    break;
            }
            btPrintTag.Focus();
        }
        public void ShowStatus()
        {
            lBMainSts.BackColor = Color.Orange;
            lBMainSts.Text = "*";
            lBMainSts.ForeColor = Color.Black;
            lBMainSts.Font = new Font("Tahoma", 150, FontStyle.Bold);
        }
        public void ShowStatusOK()
        {
            lBMainSts.BackColor = Color.Aquamarine;
            lBMainSts.Text = "O";
            lBMainSts.ForeColor = Color.DarkGreen;
            lBMainSts.Font = new Font("Tahoma", 150, FontStyle.Bold);
        }
        public void ShowStatusNG()
        {
            lBMainSts.BackColor = Color.Red;
            lBMainSts.Text = "X";
            lBMainSts.ForeColor = Color.Black;
            lBMainSts.Font = new Font("Tahoma", 150, FontStyle.Bold);
        }
        public void ModeSeiban(string totalSeibam)
        {
            if (totalSeibam == "1")
            {
                //rBNotMatch.Checked = true;  //For other project
                //rBChkBarcode.Checked = true;
                rBNotMatch.Enabled = true;
                rBChkBarcode.Enabled = true;
                rBChkRL.Enabled = true;
                lbBarLH.Hide();
                lbBarRH.Hide();
                tBBarcodeLH.Hide();
                tBBarcodeRH.Hide();
            }
            else if (totalSeibam == "2")
            {
                //rBChkBarcode.Checked = true;
                //rBChkRL.Checked = true;
                rBNotMatch.Enabled = true;
                rBChkBarcode.Enabled = true;
                rBChkRL.Enabled = true;

                if (CheckModelBArcode == "2")
                {
                    lbBar.Hide();
                    tBBarcode.Hide();
                    lbBarLH.Show();
                    lbBarRH.Show();
                    tBBarcodeLH.Show();
                    tBBarcodeRH.Show();
                }
                if (CheckModelBArcode == "3")
                {
                    lbBar.Hide();
                    tBBarcode.Hide();
                    lbBarLH.Show();
                    lbBarRH.Show();
                    tBBarcodeLH.Show();
                    tBBarcodeRH.Show();
                }
                else
                {
                    lbBarLH.Hide();
                    lbBarRH.Hide();
                    tBBarcodeLH.Hide();
                    tBBarcodeRH.Hide();
                }
            }
            else
            {
                //rBChkBarcode.Checked = true;
                rBNotMatch.Enabled = true;
                rBChkBarcode.Enabled = true;
                rBChkRL.Enabled = true;
                lbBarLH.Hide();
                lbBarRH.Hide();
                tBBarcodeLH.Hide();
                tBBarcodeRH.Hide();
            }
        }
        //public void RTBResult_TextChanged(object sender, EventArgs e)
        //{
        //    // set the current caret position to the end
        //    rTBResult.SelectionStart = rTBResult.Text.Length;
        //    // scroll it automatically
        //    try
        //    {
        //        rTBResult.ScrollToCaret();
        //        throw new Exception();
        //    }
        //    catch (Exception Error)
        //    {
        //        logError(Error.ToString()); ;
        //    }
        //}
        private void rBNotMatch_CheckedChanged(object sender, EventArgs e)
        {
            /*bool ChkRL = rBChkRL.Checked;
            bool ChkRLN = rBChkBarcode.Checked;
            if (ChkRL || ChkRLN)
            {
                lbBarLH.Show();
                lbBarRH.Show();
                tBBarcodeLH.Show();
                tBBarcodeRH.Show();

                lbBar.Hide();
                tBBarcode.Hide();
                tBSetSeiban.Text = "2";
            }*/
            lbBar.Show();
            tBBarcode.Show();
            lbBarLH.Hide();
            lbBarRH.Hide();
            tBBarcodeLH.Hide();
            tBBarcodeRH.Hide();
        }
        public void RBChkBarcode_CheckedChanged(object sender, EventArgs e)
        {
            /*bool ChkRL = rBChkRL.Checked;
            bool ChkRLN = rBChkRLN.Checked;
            if (ChkRL || ChkRLN)
            {
                lbBarLH.Show();
                lbBarRH.Show();
                tBBarcodeLH.Show();
                tBBarcodeRH.Show();

                lbBar.Hide();
                tBBarcode.Hide();
                tBSetSeiban.Text = "2";
            }*/
            lbBar.Show();
            tBBarcode.Show();
            lbBarLH.Hide();
            lbBarRH.Hide();
            tBBarcodeLH.Hide();
            tBBarcodeRH.Hide();
        }
        public void RBChkRL_CheckedChanged(object sender, EventArgs e)
        {
            /*bool ChkRL = rBChkBarcode.Checked;
            bool NoMatch = rBNotMatch.Checked;
            if (ChkRL || NoMatch)
            {
                lbBarLH.Hide();
                lbBarRH.Hide();
                tBBarcodeLH.Hide();
                tBBarcodeRH.Hide();

                lbBar.Show();
                tBBarcode.Show();
            }*/

            lbBar.Hide();
            tBBarcode.Hide();
            lbBarLH.Show();
            lbBarRH.Show();
            tBBarcodeLH.Show();
            tBBarcodeRH.Show();

            tBSetSeiban.Text = "2";
        }
        private void rBChkRLN_CheckedChanged(object sender, EventArgs e)
        {
            /*bool NoMatch = rBNotMatch.Checked;
            bool ChkRLN = rBChkBarcode.Checked;
            if (ChkRLN || NoMatch)
            {
                lbBarLH.Hide();
                lbBarRH.Hide();
                tBBarcodeLH.Hide();
                tBBarcodeRH.Hide();

                lbBar.Show();
                tBBarcode.Show();
            }*/
            lbBar.Hide();
            tBBarcode.Hide();
            lbBarLH.Show();
            lbBarRH.Show();
            tBBarcodeLH.Show();
            tBBarcodeRH.Show();

            tBSetSeiban.Text = "2";
        }
        private void MainFrom_FormClosing(object sender, FormClosingEventArgs e)
        {
            string vMainQty = lbCountQty.Text;
            string vTotalOK = lbStatusOK.Text;
            Formlogin frm2 = new Formlogin();

            if (vMainQty == "0" || vMainQty == vTotalOK)
            {
                e.Cancel = false;
            }
            else if (Int32.Parse(vMainQty) != Int32.Parse(vTotalOK))
            {
                e.Cancel = true;
                if (frm2.StsClose == false)
                {
                    frm2.Owner = this;
                    frm2.AppClose = "1";
                    frm2.Show();
                }
            }

            //saveConfig();
        }
        public static void EnableTab(TabPage page, bool enable)
        {
            EnableControls(page.Controls, enable);
        }
        public static void EnableControls(Control.ControlCollection ctls, bool enable)
        {
            foreach (Control ctl in ctls)
            {
                ctl.Enabled = enable;
                EnableControls(ctl.Controls, enable);
            }
        }
        public void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Formlogin frm2 = new Formlogin();
            switch ((sender as TabControl).SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    EnableTab(MainTab.TabPages[MainTab.SelectedIndex = 1], false);
                    frm2.Owner = this;
                    frm2.Show();
                    break;
            }
        }
        public void Popup()
        {
            frmPup.ShowDialog();
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            if (dGVBarcoode.Rows.Count > 0)
            {
                SaveDataGridViewToCSV("Test");
            }
        }
        private void btPrtTag_Click(object sender, EventArgs e)
        {
            string DCheckBy;
            DCheckBy = tBUser.Text.ToString();
            string DShift = tBShift.Text;
            string DQty = lbStatusOK.Text.ToString();
            string DLot = tBLotNo.Text;
            Prints Prt = new Prints(DataBarcodPrt, DModel, DSeiban, DCheckBy, DShift, DQty, DLot);
            if (Prt.ShowDialog() == DialogResult.OK)
            {

            }
        }
        private void btPrintTagNew_Click(object sender, EventArgs e)
        {
            string DCheckBy;
            DCheckBy = tBUser.Text.ToString();
            string DShift = tBShift.Text;
            string DQty = lbStatusOK.Text.ToString();
            string DLotNo = tBLotNo.Text.ToString();
            printTagNew PrtTagNem = new printTagNew(DataBarcodPrt, DModel, DSeiban, DCheckBy, DShift, DQty, DLotNo);
            if (PrtTagNem.ShowDialog() == DialogResult.OK)
            {

            }
        }
        private void MsgFroms(string Msg1, string Msg2, int Type)
        {

            MessageBox.Show(Msg1 + System.Environment.NewLine + Msg2, "Traceability System.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            /*MsgFrom msgFrom = new MsgFrom(Msg1, Msg2, Type);
            if (msgFrom.ShowDialog() == DialogResult.OK)
            {

            }
            //msgFrom.WarningTXT(Msg1, Msg2, Type);*/
        }
        private void AddSQLMsg(string pMessage)
        {
            DataRow SQLMsgTableRow = SQLMsg.NewRow();
            try
            {
                SQLMsgTableRow["Message"] = pMessage;
                SQLMsg.Rows.Add(SQLMsgTableRow.ItemArray);
            }
            catch
            {
                PrintResult("Msg : Error");
            }
        }
        private void formPopup()
        {
            FormPopUp formPopUp = new FormPopUp();
            if (formPopUp.ShowDialog() == DialogResult.OK)
            {

            }
            System.Threading.Thread.Sleep(700);
        }
        private void getDataSrinal()
        {
            GetDataBarcode();
            //if (bkwLoadDataSerial.IsBusy != true)
            //{
            //    bkwLoadDataSerial.RunWorkerAsync();
            //}
            //else
            //{
            //    PrintResult("App Busy!!!");
            //}
        }
        private void getDataMSSerial(string Model)
        {
            try
            {

                string[] GMSS = ConDB.GetMSSerial(Model);
                Digi_Runing = GMSS[0];
                SetPCB = GMSS[1];
                //int DR = int.Parse(Digi_Runing);
                string DR = Digi_Runing;
                if (DR != "0")
                {
                    SerialRunning = true;
                    lbStatus.Text = "Auto Serial";
                    lbStatus.BackColor = Color.Green;
                    lbStatus.ForeColor = Color.White;
                }
                else
                {
                    SerialRunning = false;
                    lbStatus.Text = "-";
                    lbStatus.BackColor = Color.Gray;
                }
            }
            catch (Exception e)
            {
                logError(e.ToString());
            }
        }
        private void BarcodeINSMAIN()
        {
            string Barcode = tBBarcode.Text;
            string Seiban = dGVDataWR.Rows[0].Cells[1].Value.ToString();
            string vModel = dGVDataWR.Rows[0].Cells[0].Value.ToString();
            getDataMSSerial(vModel);

            bool GB = cBGroupBarcode.Checked;
            if (SerialRunning && AutoSerialChk)
            {
                if (Digi_Runing != "A" && Digi_Runing != "")
                {
                    int LenBarcode = Barcode.Length;
                    int StartRun = LenBarcode - int.Parse(Digi_Runing);
                    string vMainSerial = Barcode.Substring(0, StartRun);
                    string vChkSerial = Barcode.Substring(StartRun, int.Parse(Digi_Runing));

                    int vRun;
                    int vSetPCB = int.Parse(SetPCB);
                    if (int.Parse(Digi_Runing) == 1)
                    {
                        if (vSetPCB == 35)
                        {
                            if (vChkSerial == "0" || vChkSerial == "1")
                            {
                                for (vRun = 1; vRun <= vSetPCB; vRun++)
                                {
                                    string vBarcode = vMainSerial + stringRuning(vRun);
                                    InsDataBarcode(vBarcode, Seiban);
                                    //PrintResult(vMainSerial + vRun.ToString());
                                }
                            }
                            else
                            {
                                MsgFroms("Serial not match.", "Please check WI", 1);
                                resultIns = 0;
                            }
                        }
                        else
                        {
                            if (vChkSerial == "0" || vChkSerial == "1")
                            {
                                for (vRun = 1; vRun <= vSetPCB; vRun++)
                                {
                                    string vBarcode = vMainSerial + vRun.ToString();
                                    InsDataBarcode(vBarcode, Seiban);
                                    //PrintResult(vMainSerial + vRun.ToString());
                                }
                            }
                            else
                            {
                                MsgFroms("Serial not match.", "Please check WI", 1);
                                resultIns = 0;
                            }
                        }
                    }
                    else
                    {
                        if (vChkSerial == "00" || vChkSerial == "01")
                        {
                            for (vRun = 1; vRun <= vSetPCB; vRun++)
                            {
                                string vBarcode;
                                if (vRun < 10)
                                {
                                    vBarcode = vMainSerial + "0" + vRun.ToString();
                                }
                                else
                                {
                                    vBarcode = vMainSerial + vRun.ToString();
                                }

                                InsDataBarcode(vBarcode, Seiban);
                            }
                        }
                        else
                        {
                            MsgFroms("Serial not match.", "Please check WI", 1);
                            resultIns = 0;
                        }
                    }
                }
                else if (Digi_Runing == "A")
                {
                    Digi_Runing = "1";
                    int LenBarcode = Barcode.Length;
                    int StartRun = LenBarcode - int.Parse(Digi_Runing);
                    //string vMainSerial = Barcode.Substring(0, StartRun);
                    //string vChkSerial = Barcode.Substring(StartRun, int.Parse(Digi_Runing));

                    int vRun;
                    int vSetPCB = int.Parse(SetPCB);
                    if (int.Parse(Digi_Runing) == 1)
                    {
                        for (vRun = 1; vRun <= vSetPCB; vRun++)
                        {
                            string vBarcode = Barcode + stringRuning(vRun);
                            InsDataBarcode(vBarcode, Seiban);
                            //PrintResult(vMainSerial + vRun.ToString());
                        }
                    }
                }
                else
                {
                    MsgFroms("Program setting error.", "Auto setting program.", 1);
                    resultIns = 0;
                    cBAutoRnningSerial.Checked = false;
                    saveConfig();
                }
            }
            else if (GB)
            {
                DataTable dtGetBarcode = ConDB.GetBarcodeGroup(Barcode);
                int numberBarcode = dtGetBarcode.Rows.Count;

                if (numberBarcode > 0)
                {
                    foreach (DataRow row in dtGetBarcode.Rows)
                    {
                        string vBarcode = row["Barcode"].ToString();
                        InsDataBarcode(vBarcode, Seiban);
                    }
                }
                else
                {
                    InsDataBarcode(Barcode, Seiban);
                }
            }
            else
            {
                InsDataBarcode(Barcode, Seiban);
            }
        }
        private void bkwLoadDataSerial_DoWork(object sender, DoWorkEventArgs e)
        {
            string sModel;
            string vBar = tBBarcode.Text;
            int vStartDigi = 0;
            int vChkModelBArcode = 0;

            vStartDigi = int.Parse(tBStartDigi.Text);
            vChkModelBArcode = int.Parse(tBChkModelBarcode.Text);

            sModel = dGVDataWR.Rows[0].Cells[0].Value.ToString();

            if (cBChkModelBarcode.Checked == true)
            {
                if (vBar.Substring(0, vChkModelBArcode) == sModel.Substring(vStartDigi, vChkModelBArcode - vStartDigi))
                {
                    BarcodeINSMAIN();
                }
                else
                {
                    MsgFroms("Serial not macth Model", "Please check Serial", 1);
                    tBBarcode.Text = "";
                    tBBarcode.Enabled = true;
                }
            }
            else
            {
                BarcodeINSMAIN();
            }
            
        }
        private void bkwLoadDataSerial_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            getDataSrinal();
            if (Seiban != null)
            {
                tBBarcode.Text = "";
                tBBarcode.Enabled = true;
                tBBarcode.Focus();
                CheckCountSeiban();
                CheckLotCompleteII();
            }
            else
            {
                tBBarcode.Text = "";
                tBBarcode.Enabled = true;
                tBBarcode.Focus();
                CheckCountSeiban();
                CheckLotCompleteII();
            }

            DataTable dtRecord = SQLMsg;
            int rows = dtRecord.Rows.Count;

            if (rows > 0)
            {
                foreach (DataRow vText in dtRecord.Rows)
                {
                    vMsgShow += vText["Message"].ToString() + "\n";
                }
                MsgFroms(vMsgShow, "", 1);
            }
        }
        private void bkwCover_DoWork(object sender, DoWorkEventArgs e)
        {
            string Barcode = tBBarcode.Text;
            string Seiban = dGVDataWR.Rows[0].Cells[1].Value.ToString();

            InsDataBarcode(Barcode, Seiban);
        }
        private void bkwCover_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            getDataSrinal();
            if (Seiban != null)
            {
                tBBarcode.Text = "";
                tBBarcode.Enabled = true;
                tBBarcode.Focus();
                tBCover.Text = "";
                tBCover.Enabled = false;
                CheckCountSeiban();
                CheckLotCompleteII();
            }
            else
            {
                tBCover.Text = "";
                tBCover.Enabled = true;
                tBCover.Focus();
                CheckCountSeiban();
                CheckLotCompleteII();
            }

            DataTable dtRecord = SQLMsg;
            int rows = dtRecord.Rows.Count;

            if (rows > 0)
            {
                foreach (DataRow vText in dtRecord.Rows)
                {
                    vMsgShow += vText["Message"].ToString() + "\n";
                }
                MsgFroms(vMsgShow, "", 1);
            }
        }
        private void bkwRH_DoWork(object sender, DoWorkEventArgs e)
        {
            string Barcode = tBBarcodeRH.Text;
            string Seiban = dGVDataWR.Rows[0].Cells[1].Value.ToString();
            string vModel = dGVDataWR.Rows[0].Cells[0].Value.ToString();
            getDataMSSerial(vModel);

            bool GB = cBGroupBarcode.Checked;
            if (SerialRunning && AutoSerialChk)
            {
                int LenBarcode = Barcode.Length;
                int StartRun = LenBarcode - int.Parse(Digi_Runing);
                string vMainSerial = Barcode.Substring(0, StartRun);
                string vChkSerial = Barcode.Substring(StartRun, int.Parse(Digi_Runing));

                int vRun;
                int vSetPCB = int.Parse(SetPCB);
                if (int.Parse(Digi_Runing) == 1)
                {
                    if (vChkSerial == "0" || vChkSerial == "1")
                    {
                        for (vRun = 1; vRun <= vSetPCB; vRun++)
                        {
                            string vBarcode = vMainSerial + vRun.ToString();
                            InsDataBarcode(vBarcode, Seiban);
                            //PrintResult(vMainSerial + vRun.ToString());
                        }
                    }
                    else
                    {
                        MsgFroms("Serial not macth.", "Please check WI.", 1);
                        resultIns = 0;
                    }
                }
                else
                {
                    if (vChkSerial == "00" || vChkSerial == "01")
                    {
                        for (vRun = 1; vRun <= vSetPCB; vRun++)
                        {
                            string vBarcode;
                            if (vRun < 10)
                            {
                                vBarcode = vMainSerial + "0" + vRun.ToString();
                            }
                            else
                            {
                                vBarcode = vMainSerial + vRun.ToString();
                            }

                            InsDataBarcode(vBarcode, Seiban);
                        }
                    }
                    else
                    {
                        MsgFroms("Serial not macth.", "Please check WI.", 1);
                        resultIns = 0;
                    }
                }
            }
            else if (GB)
            {
                DataTable dtGetBarcode = ConDB.GetBarcodeGroup(Barcode);
                int numberBarcode = dtGetBarcode.Rows.Count;

                if (numberBarcode > 0)
                {
                    foreach (DataRow row in dtGetBarcode.Rows)
                    {
                        string vBarcode = row["Barcode"].ToString();
                        InsDataBarcode(vBarcode, Seiban);
                    }
                }
                else
                {
                    InsDataBarcode(Barcode, Seiban);
                }
            }
            else
            {
                InsDataBarcode(Barcode, Seiban);
            }
        }
        private void bkwRH_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            getDataSrinal();
            if (resultIns == 1)
            {
                tBBarcodeRH.Text = "";
                tBBarcodeLH.Enabled = true;
                tBBarcodeLH.Focus();
                //CheckCountSeiban();
                CheckLotCompleteII();
            }
            else
            {
                tBBarcodeRH.Text = "";
                tBBarcodeRH.Enabled = true;
                tBBarcodeRH.Focus();
                //CheckCountSeiban();
                CheckLotCompleteII();
            }

            DataTable dtRecord = SQLMsg;
            int rows = dtRecord.Rows.Count;

            if (rows > 0)
            {
                foreach (DataRow vText in dtRecord.Rows)
                {
                    vMsgShow += vText["Message"].ToString() + "\n";
                }
                MsgFroms(vMsgShow, "", 1);
            }
        }
        private void bkwLH_DoWork(object sender, DoWorkEventArgs e)
        {
            string Barcode = tBBarcodeLH.Text;
            string Seiban = dGVDataWR.Rows[1].Cells[1].Value.ToString();
            string vModel = dGVDataWR.Rows[1].Cells[0].Value.ToString();
            getDataMSSerial(vModel);

            bool GB = cBGroupBarcode.Checked;
            if (SerialRunning && AutoSerialChk)
            {
                int LenBarcode = Barcode.Length;
                int StartRun = LenBarcode - int.Parse(Digi_Runing);
                string vMainSerial = Barcode.Substring(0, StartRun);
                string vChkSerial = Barcode.Substring(StartRun, int.Parse(Digi_Runing));

                int vRun;
                int vSetPCB = int.Parse(SetPCB);
                if (int.Parse(Digi_Runing) == 1)
                {
                    if (vChkSerial == "0" || vChkSerial == "1")
                    {
                        for (vRun = 1; vRun <= vSetPCB; vRun++)
                        {
                            string vBarcode = vMainSerial + vRun.ToString();
                            InsDataBarcode(vBarcode, Seiban);
                            //PrintResult(vMainSerial + vRun.ToString());
                        }
                    }
                    else
                    {
                        MsgFroms("Serial not macth.", "Please check WI.", 1);
                        resultIns = 0;
                    }
                }
                else
                {
                    if (vChkSerial == "00" || vChkSerial == "01")
                    {
                        for (vRun = 1; vRun <= vSetPCB; vRun++)
                        {
                            string vBarcode;
                            if (vRun < 10)
                            {
                                vBarcode = vMainSerial + "0" + vRun.ToString();
                            }
                            else
                            {
                                vBarcode = vMainSerial + vRun.ToString();
                            }

                            InsDataBarcode(vBarcode, Seiban);
                        }
                    }
                    else
                    {
                        MsgFroms("Serial not macth.", "Please check WI.", 1);
                        resultIns = 0;
                    }
                }
            }
            else if (GB)
            {
                DataTable dtGetBarcode = ConDB.GetBarcodeGroup(Barcode);
                int numberBarcode = dtGetBarcode.Rows.Count;

                if (numberBarcode > 0)
                {
                    foreach (DataRow row in dtGetBarcode.Rows)
                    {
                        string vBarcode = row["Barcode"].ToString();
                        InsDataBarcode(vBarcode, Seiban);
                    }
                }
                else
                {
                    InsDataBarcode(Barcode, Seiban);
                }
            }
            else
            {
                InsDataBarcode(Barcode, Seiban);
            }
        }
        private void bkwLH_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            getDataSrinal();
            if (resultIns == 1)
            {
                tBBarcodeLH.Text = "";
                tBBarcodeRH.Enabled = true;
                tBBarcodeRH.Focus();
                //CheckCountSeiban();
                CheckLotCompleteII();
            }
            else
            {
                tBBarcodeLH.Text = "";
                tBBarcodeLH.Enabled = true;
                tBBarcodeLH.Focus();
                //CheckCountSeiban();
                CheckLotCompleteII();
            }

            DataTable dtRecord = SQLMsg;
            int rows = dtRecord.Rows.Count;

            if (rows > 0)
            {
                foreach (DataRow vText in dtRecord.Rows)
                {
                    vMsgShow += vText["Message"].ToString() + "\n";
                }
                MsgFroms(vMsgShow, "", 1);
            }
        }
        private void AppSetting()
        {
            var appSettings = ConfigurationManager.AppSettings;
            Server = appSettings.Get("Server");
            DBNane = appSettings.Get("DBNane");
            User = appSettings.Get("User");
            PW = appSettings.Get("PW");
            Process = appSettings.Get("Process");
            customer = appSettings.Get("Customer");
            CLIENT = appSettings.Get("CLIENT");
            totalSeibam = appSettings.Get("totalSeibam");
            CheckModelBArcode = appSettings.Get("CheckModelBArcode");
            FontWR = appSettings.Get("dgvFontSeiban");
            FontSerial = appSettings.Get("dgvFontMain");
            GroupBarcode = appSettings.Get("GroupBacode");
            AutoSerial = appSettings.Get("AutoSerial");
            PathExport = appSettings.Get("PathExport");
            DigiControl = appSettings.Get("DigiControl");

            cBModelBarcode = appSettings.Get("cBModelBarcode");
            StartDigi = appSettings.Get("StartDigi");

            ChkBModelBarcode = appSettings.Get("ChkBModelBarcode");

            if (GroupBarcode == "Yes")
            {
                cBGroupBarcode.Checked = true;
            }
            else
            {
                cBGroupBarcode.Checked = false;
            }

            if (AutoSerial == "1")
            {
                cBAutoRnningSerial.Checked = true;
                AutoSerialChk = true;
            }
            else
            {
                cBAutoRnningSerial.Checked = false;
                AutoSerialChk = false;
            }

            if (ChkBModelBarcode == "1")
            {
                cBChkModelBarcode.Checked = true;
            }
            else
            {
                cBChkModelBarcode.Checked = false;
            }

            lbCus.Text = customer + " " + Process;
        }
        private void btSaveConfig_Click(object sender, EventArgs e)
        {
            saveConfig();
        }
        private void btSaveConfigII_Click(object sender, EventArgs e)
        {
            saveConfig();
        }
        private void saveConfig()
        {
            string setType = CheckModelBArcode;
            string setGroupBarcode = GroupBarcode;
            string setAutoSerial = AutoSerial;
            string dChkBModelBarcode;

            if (rBNotMatch.Checked)
            {
                setType = "0";
            }
            else if (rBChkBarcode.Checked)
            {
                setType = "1";
            }
            else if (rBChkRL.Checked)
            {
                setType = "2";
            }
            else if (rBChkRLN.Checked)
            {
                setType = "3";
            }

            if (cBGroupBarcode.Checked == true)
            {
                setGroupBarcode = "Yes";
            }
            else
            {
                setGroupBarcode = "No";
            }

            if (cBAutoRnningSerial.Checked == true)
            {
                setAutoSerial = "1";
            }
            else
            {
                setAutoSerial = "0";
            }
            
            if (cBChkModelBarcode.Checked == true)
            {
                dChkBModelBarcode = "1";
            }
            else
            {
                dChkBModelBarcode = "0";
            }



            string dPath = tBPathExport.Text;
            string dDigiControl = tBDigiControl.Text;
            string dChkModelBarcode = tBChkModelBarcode.Text;
            string dStartDigi = tBStartDigi.Text;

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            config.AppSettings.Settings.Remove("Server");
            config.AppSettings.Settings.Add("Server", tBServer.Text);
            config.AppSettings.Settings.Remove("DBNane");
            config.AppSettings.Settings.Add("DBNane", tBDB.Text);
            config.AppSettings.Settings.Remove("User");
            config.AppSettings.Settings.Add("User", tBUsers.Text);
            config.AppSettings.Settings.Remove("PW");
            config.AppSettings.Settings.Add("PW", tBPW.Text);
            config.AppSettings.Settings.Remove("Process");
            config.AppSettings.Settings.Add("Process", tBProcess.Text);
            config.AppSettings.Settings.Remove("Customer");
            config.AppSettings.Settings.Add("Customer", tBCus.Text);
            config.AppSettings.Settings.Remove("CLIENT");
            config.AppSettings.Settings.Add("CLIENT", tBClient.Text);
            config.AppSettings.Settings.Remove("dgvFontSeiban");
            config.AppSettings.Settings.Add("dgvFontSeiban", tBFontWR.Text);
            config.AppSettings.Settings.Remove("dgvFontMain");
            config.AppSettings.Settings.Add("dgvFontMain", tBFontSerial.Text);
            config.AppSettings.Settings.Remove("totalSeibam");
            config.AppSettings.Settings.Add("totalSeibam", tBSetSeiban.Text);
            config.AppSettings.Settings.Remove("CheckModelBArcode");
            config.AppSettings.Settings.Add("CheckModelBArcode", setType);
            config.AppSettings.Settings.Remove("GroupBacode");
            config.AppSettings.Settings.Add("GroupBacode", setGroupBarcode);
            config.AppSettings.Settings.Remove("AutoSerial");
            config.AppSettings.Settings.Add("AutoSerial", setAutoSerial);
            config.AppSettings.Settings.Remove("PathExport");
            config.AppSettings.Settings.Add("PathExport", dPath);
            config.AppSettings.Settings.Remove("DigiControl");
            config.AppSettings.Settings.Add("DigiControl", dDigiControl);

            config.AppSettings.Settings.Remove("cBModelBarcode");
            config.AppSettings.Settings.Add("cBModelBarcode", dChkModelBarcode);

            config.AppSettings.Settings.Remove("StartDigi");
            config.AppSettings.Settings.Add("StartDigi", dStartDigi);

            config.AppSettings.Settings.Remove("ChkBModelBarcode");
            config.AppSettings.Settings.Add("ChkBModelBarcode", dChkBModelBarcode);

            config.Save(ConfigurationSaveMode.Minimal);
            AppSetting();
            Application.Restart();
            Environment.Exit(0);
            //MessageBox.Show("Save completed", "Setting");
            //PrintResult("Config SAVE!!!");
        }
        public void logError(string Texts)
        {
            string pathApp = Application.StartupPath;
            string Date = DateTime.Now.ToString("yyyyMMdd");
            string timeStemp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //Clipboard.SetDataObject(Date +"---->"+ Texts);

            string subdir = pathApp + "\\Log_Error\\" + Date;

            if (!Directory.Exists(subdir))
            {
                Directory.CreateDirectory(subdir);
            }
            try
            {
                File.AppendAllText(subdir + "\\" + "Error_" + Date + ".txt", timeStemp + "--->" + Texts + Environment.NewLine);
            }
            catch
            {
                PrintResult("Can't export file.");
            }
        }
        private void cBGroupBarcode_CheckedChanged(object sender, EventArgs e)
        {
            if (cBGroupBarcode.Checked == true)
            {
                GroupBarcode = "Yes";
            }
            else
            {
                GroupBarcode = "No";
            }
        }
        private void MainFrom_FormClosed(object sender, FormClosedEventArgs e)
        {
            string setType = CheckModelBArcode;
            string setGroupBarcode = GroupBarcode;

            if (rBNotMatch.Checked)
            {
                setType = "0";
            }
            else if (rBChkBarcode.Checked)
            {
                setType = "1";
            }
            else if (rBChkRL.Checked)
            {
                setType = "2";
            }
            else if (rBChkRLN.Checked)
            {
                setType = "3";
            }

            if (cBGroupBarcode.Checked == true)
            {
                setGroupBarcode = "Yes";
            }
            else
            {
                setGroupBarcode = "No";
            }

            Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            config.AppSettings.Settings.Remove("Server");
            config.AppSettings.Settings.Add("Server", tBServer.Text);
            config.AppSettings.Settings.Remove("DBNane");
            config.AppSettings.Settings.Add("DBNane", tBDB.Text);
            config.AppSettings.Settings.Remove("User");
            config.AppSettings.Settings.Add("User", tBUsers.Text);
            config.AppSettings.Settings.Remove("PW");
            config.AppSettings.Settings.Add("PW", tBPW.Text);
            config.AppSettings.Settings.Remove("Process");
            config.AppSettings.Settings.Add("Process", tBProcess.Text);
            config.AppSettings.Settings.Remove("Customer");
            config.AppSettings.Settings.Add("Customer", tBCus.Text);
            config.AppSettings.Settings.Remove("CLIENT");
            config.AppSettings.Settings.Add("CLIENT", tBClient.Text);
            config.AppSettings.Settings.Remove("dgvFontSeiban");
            config.AppSettings.Settings.Add("dgvFontSeiban", tBFontWR.Text);
            config.AppSettings.Settings.Remove("dgvFontMain");
            config.AppSettings.Settings.Add("dgvFontMain", tBFontSerial.Text);
            config.AppSettings.Settings.Remove("totalSeibam");
            config.AppSettings.Settings.Add("totalSeibam", tBSetSeiban.Text);
            config.AppSettings.Settings.Remove("CheckModelBArcode");
            config.AppSettings.Settings.Add("CheckModelBArcode", setType);
            config.AppSettings.Settings.Remove("GroupBacode");
            config.AppSettings.Settings.Add("GroupBacode", setGroupBarcode);

            config.Save(ConfigurationSaveMode.Minimal);
        }
        private void SaveDataGridViewToCSV(string Filename)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string PathCUS = appSettings.Get("PathExport");

                string DateYY = DateTime.Now.ToString("yyyy");
                string DateMM = DateTime.Now.ToString("MM");
                string root = PathCUS;
                string subdir = PathCUS;// + DateYY + "\\" + DateMM;
                long vChkRow;

                // If directory does not exist, create it. 
                if (!Directory.Exists(subdir))
                {
                    Directory.CreateDirectory(subdir);
                }

                try
                {
                    vChkRow = CountLinesInFile(subdir + "\\" + Filename + ".csv");
                }
                catch
                {
                    vChkRow = 0;
                }
                // Choose whether to write header. Use EnableWithoutHeaderText instead to omit header.
                //if (vChkRow == 0)
                //{
                    string vHeader = "";
                    int CountColumn = dGVBarcoode.Columns.Count;
                    for (int i=0; i < CountColumn; i++)
                    {
                        string columnHeader = dGVBarcoode.Columns[i].HeaderText;
                        if(i < CountColumn -1)
                        {
                            vHeader += columnHeader + ",";
                        }
                        else
                        {
                            vHeader += columnHeader;
                        }
                    }
                    
                    File.AppendAllText(subdir + "\\" + Filename + ".csv", vHeader + Environment.NewLine);
                /*}
                else
                {
                    dGVBarcoode.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;
                }*/
                dGVBarcoode.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithAutoHeaderText;
                // Select all the cells
                dGVBarcoode.SelectAll();
                // Copy (set clipboard)
                Clipboard.SetDataObject(dGVBarcoode.GetClipboardContent());

                string Lot = tBLotNo.Text;
                string EMP = tBUser.Text;
                string Seiban = dGVDataWR.Rows[0].Cells[1].Value.ToString();
                string Qty = dGVDataWR.Rows[0].Cells[2].Value.ToString();

                int BoxSts = Int32.Parse(ConDB.STBL_TRAN_CHK_LOT_EXP(Seiban, Lot));


                if (BoxSts == 0 && Lot.Trim() != "")
                {
                    try
                    {
                        File.AppendAllText(subdir + "\\" + Filename + ".csv", Clipboard.GetText(TextDataFormat.CommaSeparatedValue) + Environment.NewLine);

                        string InsSts = ConDB.STBL_TRAN_INS_EXP(Lot, Qty, Seiban, EMP);
                        if (InsSts == "1")
                        {
                            PrintResult("Record : Success");
                            PrintResult("File :" + Filename + ".csv");
                            PrintResult("Export file to " + customer + " finish.");
                            MsgFroms("Export data to  " + customer + "  done.", "", 0);
                        }
                        else
                        {
                            PrintResult("Record : Failed");
                            PrintResult("Export file to " + customer + " finish.");
                            MsgFroms("Export data to  " + customer + "  done.", "", 0);
                        }
                    }
                    catch
                    {
                        PrintResult("Can't export file.");
                        MsgFroms("Can't export data.", "", 1);
                    }
                }
                else
                {
                    PrintResult("Export : Duplicate data.");
                    MsgFroms("Lot No already exists.", "", 1);
                }
            }
            catch (Exception e)
            {
                PrintResult("File : File is open." + e);
                MsgFroms("ไม่สามารถส่งข้อมูลได้", "", 1);
            }
        }
        static long CountLinesInFile(string f)
        {
            long count = 0;
            using (StreamReader r = new StreamReader(f))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    count++;
                }
            }
            return count;
        }
        private void btExportFinal_Click(object sender, EventArgs e)
        {
            try
            {
                string DateMM = DateTime.Now.ToString("MM");
                string DateYYYY = DateTime.Now.ToString("ddMMyyyy");
                string ModelName = dGVDataWR.Rows[0].Cells[0].Value.ToString();
                string vLot = tBLotNo.Text;
                string Seiban = dGVDataWR.Rows[0].Cells[1].Value.ToString();
                string FileName = customer + "_" + Seiban + "_" + vLot;
                SaveDataGridViewToCSV(FileName + "_" + DateYYYY);
            }
            catch (Exception err)
            {
                PrintResult("File : File is open." + err);
                MsgFroms("Can't export data.", "", 1);
            }
        }
        public string stringRuning(int pNum)
        {
            string vNum = "";
            if(pNum > 9)
            {
                switch (pNum)
                {
                    case 10: vNum = "A"; break;
                    case 11: vNum = "B"; break;
                    case 12: vNum = "C"; break;
                    case 13: vNum = "D"; break;
                    case 14: vNum = "E"; break;
                    case 15: vNum = "F"; break;
                    case 16: vNum = "G"; break;
                    case 17: vNum = "H"; break;
                    case 18: vNum = "I"; break;
                    case 19: vNum = "J"; break;
                    case 20: vNum = "K"; break;
                    case 21: vNum = "L"; break;
                    case 22: vNum = "M"; break;
                    case 23: vNum = "N"; break;
                    case 24: vNum = "O"; break;
                    case 25: vNum = "P"; break;
                    case 26: vNum = "Q"; break;
                    case 27: vNum = "R"; break;
                    case 28: vNum = "S"; break;
                    case 29: vNum = "T"; break;
                    case 30: vNum = "U"; break;
                    case 31: vNum = "V"; break;
                    case 32: vNum = "W"; break;
                    case 33: vNum = "X"; break;
                    case 34: vNum = "Y"; break;
                    case 35: vNum = "Z"; break;
                }
            }
            else
            {
                vNum = pNum.ToString(); 
            }

            return vNum;
        }
    }
}
