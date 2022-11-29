using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Specialized;
using System.Data.OleDb;

namespace NAL_SMTA
{
    class ConnectDB
    {
        public string UserID;
        public string UserName;
        public string Error;
        public string MainShift;
        public string MainShiftID;
        public string Result;
        public List<String> vMainRecord = new List<String>();

        public SqlConnection ConnectionDB()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string Server = appSettings.Get("Server");
                string DataBase = appSettings.Get("DBNane");
                string User = appSettings.Get("User");
                string PW = appSettings.Get("PW");
                string PoolSize = appSettings.Get("PoolSize");
                string Timeout = appSettings.Get("Timeout");
                string connetionString;
                SqlConnection cnn;
                connetionString = @"Data Source=" + Server + ";"
                                + "Initial Catalog=" + DataBase + ";"
                                + "User ID=" + User + ";"
                                + "Password=" + PW + ";"
                                + "Max Pool Size=" + PoolSize + ";"
                                + "Connect Timeout=" + Timeout + ";";
                cnn = new SqlConnection(connetionString);
                return cnn;
            }
            catch (SqlException e)
            {
                SqlConnection cnn;
                cnn = new SqlConnection();
                Error = (e.ToString());
                return cnn;
            }
  
        }
        public void CheckUserLogin(string User)
        {
            try
            {
                SqlConnection cnn = this.ConnectionDB();
                SqlCommand objCmd = new SqlCommand();
                SqlDataAdapter dtAdapter = new SqlDataAdapter();

                DataSet ds = new DataSet();
                DataTable dt;
                String strStored;

                using (cnn)
                {
                    strStored = "CHECK_USER";
                    objCmd.Parameters.Add(new SqlParameter("@pUser", User));

                    objCmd.Connection = cnn;
                    objCmd.CommandText = strStored;
                    objCmd.CommandType = CommandType.StoredProcedure;

                    dtAdapter.SelectCommand = objCmd;

                    dtAdapter.Fill(ds);
                    dt = ds.Tables[0];

                    try
                    {
                        if (dt.Rows.Count > 0)
                        {
                            UserName = dt.Rows[0]["STAFF_NAME"].ToString();
                            UserID = dt.Rows[0]["STAFF_CODE"].ToString();
                        }
                        else
                        {
                            UserName = "";
                            UserID = "";
                        }
                    }
                    catch
                    {
                        UserName = "";
                        UserID = "";
                    }
                }
            }
            catch (SqlException e)
            {
                this.Error = (e.ToString());
            }
        }
        public string ChkUserLogIn(string User, string PW)
        {
            string Result;
            try
            {
                SqlConnection cnn = ConnectionDB();
                SqlCommand objCmd = new SqlCommand();
                SqlDataAdapter dtAdapter = new SqlDataAdapter();

                DataSet ds = new DataSet();
                DataTable dt;
                string strStored;

                using (cnn)
                {
                    strStored = "CHK_Staff_login";
                    objCmd.Parameters.Add(new SqlParameter("@vUser", User));
                    objCmd.Parameters.Add(new SqlParameter("@vPW", PW));

                    objCmd.Connection = cnn;
                    objCmd.CommandText = strStored;
                    objCmd.CommandType = CommandType.StoredProcedure;

                    dtAdapter.SelectCommand = objCmd;

                    dtAdapter.Fill(ds);
                    dt = ds.Tables[0];

                    try
                    {
                        Result = dt.Rows[0]["Result"].ToString();
                    }
                    catch
                    {
                        Result = "0";
                    }

                    return Result;
                }
            }
            catch (SqlException e)
            {
                Error = (e.ToString());
                Result = Error;
                return Result;
            }
        }
        public DataTable GetDataWR(string Lot, string Process, string Seiban)
        {
            SqlConnection cnn = ConnectionDB();
            SqlCommand objCmd = new SqlCommand();
            SqlDataAdapter dtAdapter = new SqlDataAdapter();

            string strStored;

            using (cnn)
            {
                strStored = "[dbo].[MAIN_GET_DATA_WR]";
                objCmd.Parameters.Add(new SqlParameter("@pLot", Lot));
                objCmd.Parameters.Add(new SqlParameter("@pProcess", Process));
                objCmd.Parameters.Add(new SqlParameter("@pSeiban", Seiban));

                objCmd.Connection = cnn;
                objCmd.CommandText = strStored;
                objCmd.CommandType = CommandType.StoredProcedure;

                dtAdapter.SelectCommand = objCmd;

                DataTable dtRecord = new DataTable();
                dtAdapter.Fill(dtRecord);
                return dtRecord;
            }
        }
        public DataTable GetDataLot(string Tracking, string Process)
        {
            SqlConnection cnn = ConnectionDB();
            SqlCommand objCmd = new SqlCommand();
            SqlDataAdapter dtAdapter = new SqlDataAdapter();

            string strStored;

            using (cnn)
            {
                strStored = "STBL_GET_LOT_DATA_I";
                objCmd.Parameters.Add(new SqlParameter("@pTracking", Tracking));
                objCmd.Parameters.Add(new SqlParameter("@pProcess", Process));

                objCmd.Connection = cnn;
                objCmd.CommandText = strStored;
                objCmd.CommandType = CommandType.StoredProcedure;

                dtAdapter.SelectCommand = objCmd;

                DataTable dtRecord = new DataTable();
                dtAdapter.Fill(dtRecord);
                return dtRecord;
            }
        }
        public string[] GetDetialWR(string textWR)
        {
            try
            {
                string seiban;

                if (textWR.Length > 15)
                {
                    seiban = textWR.Substring(38, 10);
                }
                else
                {
                    seiban = textWR.Substring(0, 10);
                }

                SqlConnection cnn = ConnectionDB();
                SqlCommand objCmd = new SqlCommand();
                SqlDataAdapter dtAdapter = new SqlDataAdapter();

                DataSet ds = new DataSet();
                DataTable dt;
                string strStored;

                using (cnn)
                {
                    strStored = "MAIN_GET_SEIBAN_MS";
                    objCmd.Parameters.Add(new SqlParameter("@pSEIBAN", seiban));


                    objCmd.Connection = cnn;
                    objCmd.CommandText = strStored;
                    objCmd.CommandType = CommandType.StoredProcedure;

                    dtAdapter.SelectCommand = objCmd;

                    dtAdapter.Fill(ds);
                    dt = ds.Tables[0];

                    try
                    {
                        string[] WR;
                        string Seiban = dt.Rows[0]["Seiban"].ToString();
                        string ItemCD = dt.Rows[0]["ItemCD"].ToString();
                        WR = new string[2] { Seiban, ItemCD };
                        return WR;
                    }
                    catch
                    {
                        string[] WR;
                        WR = new string[2] { "ErrorSQL", "ErrorSQL" };
                        return WR;
                    }
                }


               
            }
            catch
            {
                string[] WR;
                WR = new string[2] { "Error", "Error" };
                return WR;
            }
        }
        public string[] GetMSSerial(string Model)
        {
            try
            {

                SqlConnection cnn = ConnectionDB();
                SqlCommand objCmd = new SqlCommand();
                SqlDataAdapter dtAdapter = new SqlDataAdapter();

                DataSet ds = new DataSet();
                DataTable dt;
                string strStored;

                using (cnn)
                {
                    strStored = "MAIN_GET_MS_SERIAL";
                    objCmd.Parameters.Add(new SqlParameter("@pModel", Model));


                    objCmd.Connection = cnn;
                    objCmd.CommandText = strStored;
                    objCmd.CommandType = CommandType.StoredProcedure;

                    dtAdapter.SelectCommand = objCmd;

                    dtAdapter.Fill(ds);
                    dt = ds.Tables[0];

                    try
                    {
                        string[] WR;
                        string Digi_Running;
                        string Set_PCB;
                        if (dt.Rows.Count > 0)
                        {
                            Digi_Running = dt.Rows[0]["Digi_Running"].ToString();
                            Set_PCB = dt.Rows[0]["Set_PCB"].ToString();
                        }
                        else
                        {
                            Digi_Running = "";
                            Set_PCB = "";
                        }
                        
                        WR = new string[2] { Digi_Running, Set_PCB };

                        return WR;
                    }
                    catch
                    {
                        string[] WR;
                        WR = new string[2] { "0", "0" };
                        return WR;
                    }
                }



            }
            catch
            {
                string[] WR;
                WR = new string[2] { "Error", "Error" };
                return WR;
            }
        }
        public string InMAIN_INS_WR_CRL(string PROCESS, string SHIFT_ID, string LOT, string EMP, string SEIBAN_CD,string MainGruopWr, string ITEMCODE, string CLIENT, string Qty)
        {

            try
            {
                SqlConnection cnn = ConnectionDB();
                SqlCommand objCmd = new SqlCommand();
                SqlDataAdapter dtAdapter = new SqlDataAdapter();

                DataSet ds = new DataSet();
                DataTable dt;
                string strStored;

                using (cnn)
                {
                    strStored = "MAIN_INS_WR_CRL";
                    objCmd.Parameters.Add(new SqlParameter("@pPROCESS", PROCESS));
                    objCmd.Parameters.Add(new SqlParameter("@pSHIFT_ID", SHIFT_ID));
                    objCmd.Parameters.Add(new SqlParameter("@pLOT", LOT));
                    objCmd.Parameters.Add(new SqlParameter("@pEMP", EMP));
                    objCmd.Parameters.Add(new SqlParameter("@pSEIBAN_CD", SEIBAN_CD));
                    objCmd.Parameters.Add(new SqlParameter("@pSEIBAN_Group", MainGruopWr));
                    objCmd.Parameters.Add(new SqlParameter("@pITEMCODE", ITEMCODE));
                    objCmd.Parameters.Add(new SqlParameter("@pCLIENT", CLIENT));
                    objCmd.Parameters.Add(new SqlParameter("@pQty", Qty));

                    objCmd.Connection = cnn;
                    objCmd.CommandText = strStored;
                    objCmd.CommandType = CommandType.StoredProcedure;

                    dtAdapter.SelectCommand = objCmd;

                    dtAdapter.Fill(ds);
                    dt = ds.Tables[0];

                    try
                    {
                        Result = dt.Rows[0]["result"].ToString();
                        return Result;
                    }
                    catch (Exception e)
                    {
                        Result = "SQL Error : " + e;
                        return Result;
                    }
                }
            }
            catch (SqlException e)
            {
                Error = (e.ToString());
                return Error;
            }
        }
        public string[] InMAIN_INS_STBL(string PROCESS, string BARCODE, string LOT, string ShiftID, string Shift, string Emp, string Seiban, string CLIENT, string Type, string Cover)
        {

            try
            {
                SqlConnection cnn = ConnectionDB();
                SqlCommand objCmd = new SqlCommand();
                SqlDataAdapter dtAdapter = new SqlDataAdapter();

                DataSet ds = new DataSet();
                DataTable dt;
                string strStored;
                string[] Results;

                using (cnn)
                {
                    strStored = "MAIN_INS_STBL";
                    objCmd.Parameters.Add(new SqlParameter("@pProcess", PROCESS));
                    objCmd.Parameters.Add(new SqlParameter("@pBarcode", BARCODE));
                    objCmd.Parameters.Add(new SqlParameter("@pLot", LOT));
                    objCmd.Parameters.Add(new SqlParameter("@pShiftID", ShiftID));
                    objCmd.Parameters.Add(new SqlParameter("@pShift", Shift));
                    objCmd.Parameters.Add(new SqlParameter("@pEmp", Emp));
                    objCmd.Parameters.Add(new SqlParameter("@pSeiban", Seiban));
                    objCmd.Parameters.Add(new SqlParameter("@pClient", CLIENT));
                    objCmd.Parameters.Add(new SqlParameter("@pType", Type));
                    objCmd.Parameters.Add(new SqlParameter("@pCover", Cover));

                    objCmd.Connection = cnn;
                    objCmd.CommandText = strStored;
                    objCmd.CommandType = CommandType.StoredProcedure;

                    dtAdapter.SelectCommand = objCmd;

                    dtAdapter.Fill(ds);
                    dt = ds.Tables[0];

                    try
                    {
                        string Result = dt.Rows[0]["result"].ToString();
                        string RSts = dt.Rows[0]["RSts"].ToString();
                        Results = new string[2] { Result, RSts };
                        return Results;
                    }
                    catch (Exception e)
                    {
                        Results = new string[2] { "SQL Error : " + e, "0" };
                        return Results;
                    }
                }
            }
            catch (SqlException e)
            {
                string[] Results;
                Results = new string[] { "SQL Error" + e, "0" };
                return Results;
            }
        }
        public string GetHeader(ref string Model, ref string Step)
        {
            try
            {
                SqlConnection cnn = ConnectionDB();
                SqlCommand objCmd = new SqlCommand();
                SqlDataAdapter dtAdapter = new SqlDataAdapter();

                DataSet ds = new DataSet();
                DataTable dt;
                string strStored;

                using (cnn)
                {
                    try
                    {
                        strStored = "MAIN_GET_NAME_PROCESS";
                        objCmd.Parameters.Add(new SqlParameter("@pModel", Model));
                        objCmd.Parameters.Add(new SqlParameter("@pStep", Step));

                        objCmd.Connection = cnn;
                        objCmd.CommandText = strStored;
                        objCmd.CommandType = CommandType.StoredProcedure;

                        dtAdapter.SelectCommand = objCmd;

                        dtAdapter.Fill(ds);
                        dt = ds.Tables[0];

                        if (dt.Rows.Count > 0)
                        {
                            return dt.Rows[0]["StepName"].ToString();
                        }
                        else 
                        {
                            return Step;
                        }
                    }
                    catch
                    {
                        return Step;
                    }
                }
            }
            catch
            {
                Error = "0";
                return Error;
            }
        }
        public DataTable MAIN_SEC_DATA_STBL(string PROCESS, string LOT, string Model, string SEIBAN)
        {

                SqlConnection cnn = ConnectionDB();
                SqlCommand objCmd = new SqlCommand();
                SqlDataAdapter dtAdapter = new SqlDataAdapter();

                string strStored;

                using (cnn)
                {
                    strStored = "MAIN_SEC_DATA_STBL";
                    objCmd.Parameters.Add(new SqlParameter("@pProcess", PROCESS));
                    objCmd.Parameters.Add(new SqlParameter("@pLot", LOT));
                    objCmd.Parameters.Add(new SqlParameter("@pModel", Model));
                    objCmd.Parameters.Add(new SqlParameter("@pSeiban", SEIBAN));

                    objCmd.Connection = cnn;
                    objCmd.CommandText = strStored;
                    objCmd.CommandType = CommandType.StoredProcedure;

                    dtAdapter.SelectCommand = objCmd;
                    
                    DataTable dtRecord = new DataTable();
                    dtAdapter.Fill(dtRecord);
                    return dtRecord;
                }
        }
        public string ShowTotalQty(string Process, string Lot)
        {
            try
            {
                SqlConnection cnn = ConnectionDB();
                SqlCommand objCmd = new SqlCommand();
                SqlDataAdapter dtAdapter = new SqlDataAdapter();

                DataSet ds = new DataSet();
                DataTable dt;
                string strStored;

                using (cnn)
                {
                    strStored = "MAIN_COUNT_SEIBAN_WITHLOT";
                    objCmd.Parameters.Add(new SqlParameter("@pPROCESS", Process));
                    objCmd.Parameters.Add(new SqlParameter("@pLOT", Lot));

                    objCmd.Connection = cnn;
                    objCmd.CommandText = strStored;
                    objCmd.CommandType = CommandType.StoredProcedure;

                    dtAdapter.SelectCommand = objCmd;

                    dtAdapter.Fill(ds);
                    dt = ds.Tables[0];

                    try
                    {
                        return dt.Rows[0]["TOTAL_QTY"].ToString();
                    }
                    catch
                    {
                        return "0";
                    }
                }
            }
            catch
            {
                Error = "0";
                return Error;
            }
        }
        public void GetShift()
        {
            string getTime = DateTime.Now.ToString("HH");
            int getTimeNum = Convert.ToInt32(getTime);
            if (getTimeNum >= 7 && getTimeNum <= 15)
            {
                MainShift = "A";
                MainShiftID = "1";
            }
            else if (getTimeNum >= 15 && getTimeNum < 23)
            {
                MainShift = "B";
                MainShiftID = "2";
            }
            else if (getTimeNum == 23)
            {
                MainShift = "C";
                MainShiftID = "3";
            }
            else if (getTimeNum < 23 && getTimeNum < 7)
            {
                MainShift = "C";
                MainShiftID = "3";
            }
        }
        public string GetQtySinfonia(string Seiban)
        {
            try
            {
                SqlConnection cnn = ConnectionDB();
                SqlCommand objCmd = new SqlCommand();
                SqlDataAdapter dtAdapter = new SqlDataAdapter();

                DataSet ds = new DataSet();
                DataTable dt;
                string strStored;

                using (cnn)
                {
                    strStored = "STBL_SINF_GET_QTY";
                    objCmd.Parameters.Add(new SqlParameter("@pSeiban", Seiban));

                    objCmd.Connection = cnn;
                    objCmd.CommandText = strStored;
                    objCmd.CommandType = CommandType.StoredProcedure;

                    dtAdapter.SelectCommand = objCmd;

                    dtAdapter.Fill(ds);
                    dt = ds.Tables[0];

                    try
                    {
                        return dt.Rows[0]["Qty"].ToString();
                    }
                    catch
                    {
                        return "0";
                    }
                }
            }
            catch
            {
                Error = "0";
                return Error;
            }
        }
        public DataTable GetBarcodeGroup(string Barcode)
        {
            SqlConnection cnn = ConnectionDB();
            SqlCommand objCmd = new SqlCommand();
            SqlDataAdapter dtAdapter = new SqlDataAdapter();

            string strStored;

            using (cnn)
            {
                strStored = "MAIN_GET_BARCODE_GROUP";
                objCmd.Parameters.Add(new SqlParameter("@pBarcode", Barcode));

                objCmd.Connection = cnn;
                objCmd.CommandText = strStored;
                objCmd.CommandType = CommandType.StoredProcedure;

                dtAdapter.SelectCommand = objCmd;

                DataTable dtRecord = new DataTable();
                dtAdapter.Fill(dtRecord);
                return dtRecord;
            }
        }
        public string STBL_TRAN_INS_EXP(string LOT, string Qty, string SEIBAN, string EMP)
        {

            SqlConnection cnn = ConnectionDB();
            SqlCommand objCmd = new SqlCommand();
            SqlDataAdapter dtAdapter = new SqlDataAdapter();

            DataSet ds = new DataSet();
            DataTable dt;
            string strStored;

            using (cnn)
            {
                strStored = "STBL_TRAN_INS_EXP";
                objCmd.Parameters.Add(new SqlParameter("@pLot", LOT));
                objCmd.Parameters.Add(new SqlParameter("@pQty", Qty));
                objCmd.Parameters.Add(new SqlParameter("@pSEIBAN", SEIBAN));
                objCmd.Parameters.Add(new SqlParameter("@pEMP", EMP));

                objCmd.Connection = cnn;
                objCmd.CommandText = strStored;
                objCmd.CommandType = CommandType.StoredProcedure;

                dtAdapter.SelectCommand = objCmd;

                dtAdapter.Fill(ds);
                dt = ds.Tables[0];

                try
                {
                    Result = dt.Rows[0]["Result"].ToString();
                    return Result;
                }
                catch
                {
                    Result = "0";
                    return Result;
                }

            }
        }
        public string STBL_TRAN_CHK_LOT_EXP(string SEIBAN, string Lot)
        {

            SqlConnection cnn = ConnectionDB();
            SqlCommand objCmd = new SqlCommand();
            SqlDataAdapter dtAdapter = new SqlDataAdapter();

            DataSet ds = new DataSet();
            DataTable dt;
            string strStored;

            using (cnn)
            {
                strStored = "STBL_TRAN_CHK_LOT_EXP";
                objCmd.Parameters.Add(new SqlParameter("@pSeiban", SEIBAN));
                objCmd.Parameters.Add(new SqlParameter("@pLot", Lot));

                objCmd.Connection = cnn;
                objCmd.CommandText = strStored;
                objCmd.CommandType = CommandType.StoredProcedure;

                dtAdapter.SelectCommand = objCmd;

                dtAdapter.Fill(ds);
                dt = ds.Tables[0];

                try
                {
                    Result = dt.Rows[0]["Result"].ToString();
                    return Result;
                }
                catch
                {
                    Result = "0";
                    return Result;
                }

            }
        }
    }
}
