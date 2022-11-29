using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NAL_SMTA
{
    public partial class Prints : Form
    {
        DataTable DataBarcode;
        string DModel;
        string DSeiban;
        string DCheckBy;
        string DShift;
        string DQty;
        string DLot;
        public Prints()
        {
            InitializeComponent();
        }
        public Prints(DataTable dt, string Model,string SEIBAN,string CheckBy,string Shift,string Qty,string Lot)
        {
            InitializeComponent();
            DataBarcode = dt;
            DModel = Model;
            DSeiban = SEIBAN;
            DCheckBy = CheckBy;
            DShift = Shift;
            DQty = Qty;
            DLot = Lot;
        }
        private void Print_Load(object sender, EventArgs e)
        {
            try
            {
                string ShipDate = DateTime.Now.ToString("dd-MM-yyyy");

                DataTable dt = DataBarcode;
                int totalRows = dt.Rows.Count;

                DataTable dtNew = new DataTable();
                DataColumn dtCID = dtNew.Columns.Add("ID");
                dtCID.DataType = System.Type.GetType("System.Int32");
                dtNew.Columns.Add("Serial");
                DataRow dr = null;
                int j = 1;
                string TQrSerial = "";
                string TQrSerial2 = "";
                string TQrSerial3 = "";
                string TQrSerial4 = "";
                int columnz = dt.Columns.Count;
                for (int i = 0; i < totalRows; i++)
                {
                    if (dt.Rows[i][columnz - 3].ToString() == "OK")
                    {
                        dr = dtNew.NewRow(); // have new row on each iteration
                        dr["ID"] = j;
                        dr["Serial"] = dt.Rows[i]["Barcode"].ToString();
                        dtNew.Rows.Add(dr);
                        j++;
                    }
                }

                int totalRowsNew = dtNew.Rows.Count;
                int vID = 0;
                DataRow drNew = null;

                if (int.Parse(DQty) <= 10)
                {
                    for (int i = 0; i < totalRowsNew; i++)
                    {
                        vID++;
                        drNew = dtNew.NewRow(); // have new row on each iteration
                        TQrSerial.Trim();
                        if (vID < 10)
                        {
                            TQrSerial += dtNew.Rows[i]["Serial"].ToString() + "\r\n";
                        }
                        else
                        {
                            TQrSerial += dtNew.Rows[i]["Serial"].ToString();
                        }
                    }
                }
                else if (int.Parse(DQty) > 10 && int.Parse(DQty) <= 40)
                {
                    int vloop = int.Parse(DQty);
                    if (vloop < 40)
                    {
                        vloop = 40;
                    }

                    for (int i = 0; i <= 9; i++)
                    {
                        vID++;
                        drNew = dtNew.NewRow(); // have new row on each iteration
                        TQrSerial.Trim();
                        if (vID < 10)
                        {
                            TQrSerial += dtNew.Rows[i]["Serial"].ToString() + "\r\n";
                        }
                        else
                        {
                            TQrSerial += dtNew.Rows[i]["Serial"].ToString();
                        }
                    }

                    for (int i = 10; i <= vloop - 21; i++)
                    {
                        vID++;
                        drNew = dtNew.NewRow(); // have new row on each iteration
                        TQrSerial.Trim();
                        if (vID < 20 && vID < (int.Parse(DQty)))
                        {
                            TQrSerial2 += dtNew.Rows[i]["Serial"].ToString() + "\r\n";
                        }
                        else
                        {
                            //TQrSerial2 += dtNew.Rows[i]["Serial"].ToString();
                            //break;
                            if (vID <= (int.Parse(DQty)))
                            {
                                TQrSerial2 += dtNew.Rows[i]["Serial"].ToString();
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    for (int i = 20; i <= vloop - 11; i++)
                    {
                        vID++;
                        drNew = dtNew.NewRow(); // have new row on each iteration
                        TQrSerial.Trim();
                        if (vID < 30 && vID < (int.Parse(DQty)))
                        {
                            TQrSerial3 += dtNew.Rows[i]["Serial"].ToString() + "\r\n";
                        }
                        else
                        {
                            //TQrSerial3 += dtNew.Rows[i]["Serial"].ToString();
                            //break;
                            if (vID <= (int.Parse(DQty)))
                            {
                                TQrSerial3 += dtNew.Rows[i]["Serial"].ToString();
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    for (int i = 30; i <= vloop - 1; i++)
                    {
                        vID++;
                        drNew = dtNew.NewRow(); // have new row on each iteration
                        TQrSerial.Trim();
                        if (vID < 40 && vID <= (int.Parse(DQty)))
                        {
                            TQrSerial4 += dtNew.Rows[i]["Serial"].ToString() + "\r\n";
                        }
                        else
                        {
                            if (vID <= (int.Parse(DQty)))
                            {
                                TQrSerial4 += dtNew.Rows[i]["Serial"].ToString();
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }


                var dtB = new ReportDataSource("DataSet1", dtNew);
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(dtB);

                ReportParameter pModel = new ReportParameter("pReportModel", DModel);
                ReportParameter pShipdate = new ReportParameter("pReportDate", ShipDate);
                ReportParameter pSeiban = new ReportParameter("pReportSeiban", DSeiban);
                ReportParameter pCheckBy = new ReportParameter("pReportChkBy", DCheckBy);
                ReportParameter pShift = new ReportParameter("pReportShift", DShift);
                ReportParameter pQty = new ReportParameter("pReportQty", DQty);
                ReportParameter pLot = new ReportParameter("pReportLotNo", DLot);

                ReportParameter[] rp = new ReportParameter[] { pModel, pShipdate, pSeiban, pCheckBy, pShift, pQty, pLot };

                reportViewer1.LocalReport.EnableExternalImages = true;

                QRCoder.QRCodeGenerator qRCodeGenerator = new QRCoder.QRCodeGenerator();
                QRCoder.QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(DModel + "," + DSeiban + "," + DQty + "," + DLot, QRCoder.QRCodeGenerator.ECCLevel.Q);

                QRCoder.QRCodeData qRCodeSerialData = qRCodeGenerator.CreateQrCode(TQrSerial, QRCoder.QRCodeGenerator.ECCLevel.Q);
                QRCoder.QRCodeData qRCodeSerialData2 = qRCodeGenerator.CreateQrCode(TQrSerial2, QRCoder.QRCodeGenerator.ECCLevel.Q);
                QRCoder.QRCodeData qRCodeSerialData3 = qRCodeGenerator.CreateQrCode(TQrSerial3, QRCoder.QRCodeGenerator.ECCLevel.Q);
                QRCoder.QRCodeData qRCodeSerialData4 = qRCodeGenerator.CreateQrCode(TQrSerial4, QRCoder.QRCodeGenerator.ECCLevel.Q);

                QRCoder.QRCode qRCode = new QRCoder.QRCode(qRCodeData);
                QRCoder.QRCode qRCodeSerial = new QRCoder.QRCode(qRCodeSerialData);
                QRCoder.QRCode qRCodeSerial2 = new QRCoder.QRCode(qRCodeSerialData2);
                QRCoder.QRCode qRCodeSerial3 = new QRCoder.QRCode(qRCodeSerialData3);
                QRCoder.QRCode qRCodeSerial4 = new QRCoder.QRCode(qRCodeSerialData4);

                Bitmap png = qRCode.GetGraphic(20);
                Bitmap pngSerial = qRCodeSerial.GetGraphic(20);
                Bitmap pngSerial2 = qRCodeSerial2.GetGraphic(20);
                Bitmap pngSerial3 = qRCodeSerial3.GetGraphic(20);
                Bitmap pngSerial4 = qRCodeSerial4.GetGraphic(20);

                using (MemoryStream ms = new MemoryStream())
                {
                    png.Save(ms, ImageFormat.Png);

                    DataSet1 dataSet1 = new DataSet1();
                    DataSet1.QrcodeModelRow qrcodeModelRow = dataSet1.QrcodeModel.NewQrcodeModelRow();
                    qrcodeModelRow.imageModel = ms.ToArray();
                    dataSet1.QrcodeModel.AddQrcodeModelRow(qrcodeModelRow);

                    ReportDataSource reportDataSource = new ReportDataSource();
                    reportDataSource.Name = "ReportQrImage";
                    reportDataSource.Value = dataSet1.QrcodeModel;
                    reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    pngSerial.Save(ms, ImageFormat.Png);

                    DataSet1 dataSet1 = new DataSet1();
                    DataSet1.QrcodeSerialRow qrcodeSerialRow = dataSet1.QrcodeSerial.NewQrcodeSerialRow();
                    qrcodeSerialRow.imageSerial = ms.ToArray();
                    dataSet1.QrcodeSerial.AddQrcodeSerialRow(qrcodeSerialRow);

                    ReportDataSource reportDataSource = new ReportDataSource();
                    reportDataSource.Name = "ReportQrSerial";
                    reportDataSource.Value = dataSet1.QrcodeSerial;
                    reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    pngSerial2.Save(ms, ImageFormat.Png);

                    DataSet1 dataSet1 = new DataSet1();
                    DataSet1.QrcodeSerialSet2Row qrcodeSerialSet2Row = dataSet1.QrcodeSerialSet2.NewQrcodeSerialSet2Row();
                    qrcodeSerialSet2Row.imageSerialSet2 = ms.ToArray();
                    dataSet1.QrcodeSerialSet2.AddQrcodeSerialSet2Row(qrcodeSerialSet2Row);

                    ReportDataSource reportDataSource = new ReportDataSource();
                    reportDataSource.Name = "ReportQrSerial2";
                    reportDataSource.Value = dataSet1.QrcodeSerialSet2;
                    reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    pngSerial3.Save(ms, ImageFormat.Png);

                    DataSet1 dataSet1 = new DataSet1();
                    DataSet1.QrcodeSerialSet3Row qrcodeSerialSet3Row = dataSet1.QrcodeSerialSet3.NewQrcodeSerialSet3Row();
                    qrcodeSerialSet3Row.imageSerialSet3 = ms.ToArray();
                    dataSet1.QrcodeSerialSet3.AddQrcodeSerialSet3Row(qrcodeSerialSet3Row);

                    ReportDataSource reportDataSource = new ReportDataSource();
                    reportDataSource.Name = "ReportQrSerial3";
                    reportDataSource.Value = dataSet1.QrcodeSerialSet3;
                    reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    pngSerial4.Save(ms, ImageFormat.Png);

                    DataSet1 dataSet1 = new DataSet1();
                    DataSet1.QrcodeSerialSet4Row qrcodeSerialSet4Row = dataSet1.QrcodeSerialSet4.NewQrcodeSerialSet4Row();
                    qrcodeSerialSet4Row.imageSerialSet4 = ms.ToArray();
                    dataSet1.QrcodeSerialSet4.AddQrcodeSerialSet4Row(qrcodeSerialSet4Row);

                    ReportDataSource reportDataSource = new ReportDataSource();
                    reportDataSource.Name = "ReportQrSerial4";
                    reportDataSource.Value = dataSet1.QrcodeSerialSet4;
                    reportViewer1.LocalReport.DataSources.Add(reportDataSource);
                }

                reportViewer1.LocalReport.SetParameters(rp);
                reportViewer1.LocalReport.Refresh();
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
