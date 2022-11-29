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
    public partial class printTagNew : Form
    {
        DataTable DataBarcode;
        string DModel;
        string DSeiban;
        string DCheckBy;
        string DShift;
        string DQty;
        string DLotNo;
        public printTagNew()
        {
            InitializeComponent();
        }
        public printTagNew(DataTable dt, string Model, string SEIBAN, string CheckBy, string Shift, string Qty, string LotNo)
        {
            InitializeComponent();
            DataBarcode = dt;
            DModel = Model;
            DSeiban = SEIBAN;
            DCheckBy = CheckBy;
            DShift = Shift;
            DQty = Qty;
            DLotNo = LotNo;
        }
        private void printTagNew_Load(object sender, EventArgs e)
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
            string TQrSerialSET1 = "";
            string TQrSerialSET2 = "";
            string TQrSerialSET3 = "";
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

            int M = 0;
            for (int i = 0; i < totalRows; i++)
            {
                if (dt.Rows[i][columnz - 3].ToString() == "OK")
                {
                    M++;
                    if (M <= 3)
                    {
                        if (M < 3)
                        {
                            TQrSerialSET1 += dt.Rows[i]["Barcode"].ToString() + "\r\n";
                        }
                        else
                        {
                            TQrSerialSET1 += dt.Rows[i]["Barcode"].ToString();
                        }
                    }

                    if (M > 3 && M <= 6)
                    {
                        if (M < 6)
                        {
                            TQrSerialSET2 += dt.Rows[i]["Barcode"].ToString() + "\r\n";
                        }
                        else
                        {
                            TQrSerialSET2 += dt.Rows[i]["Barcode"].ToString();
                        }
                    }

                    if (M > 6 )
                    {
                        if(M < 10)
                        {
                            TQrSerialSET3 += dt.Rows[i]["Barcode"].ToString() + "\r\n";
                        }
                        else
                        {
                            TQrSerialSET3 += dt.Rows[i]["Barcode"].ToString();
                        }
                    }
                }
            }

            var dtB = new ReportDataSource("DataSet1", dtNew);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(dtB);

            ReportParameter pModel = new ReportParameter("rpModel", DModel);
            ReportParameter pShipdate = new ReportParameter("rpDate", ShipDate);
            ReportParameter pSeiban = new ReportParameter("rpSeiban", DSeiban);
            ReportParameter pCheckBy = new ReportParameter("rpCheckBy", DCheckBy);
            ReportParameter pQty = new ReportParameter("rpQty", DQty);
            ReportParameter pLotNo = new ReportParameter("rpLotNo", DLotNo);

            ReportParameter[] rpPaara = new ReportParameter[] { pModel, pShipdate, pSeiban, pCheckBy, pQty, pLotNo };



            reportViewer1.LocalReport.EnableExternalImages = true;

            QRCoder.QRCodeGenerator qRCodeGenerator = new QRCoder.QRCodeGenerator();
            QRCoder.QRCodeData qRCodeData = qRCodeGenerator.CreateQrCode(DModel, QRCoder.QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCodeData qRCodeSerialDataSet1 = qRCodeGenerator.CreateQrCode(TQrSerialSET1, QRCoder.QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCodeData qRCodeSerialDataSet2 = qRCodeGenerator.CreateQrCode(TQrSerialSET2, QRCoder.QRCodeGenerator.ECCLevel.Q);
            QRCoder.QRCodeData qRCodeSerialDataSet3 = qRCodeGenerator.CreateQrCode(TQrSerialSET3, QRCoder.QRCodeGenerator.ECCLevel.Q);

            QRCoder.QRCode qRCode = new QRCoder.QRCode(qRCodeData);

            QRCoder.QRCode qRCodeSerialSet1 = new QRCoder.QRCode(qRCodeSerialDataSet1);
            QRCoder.QRCode qRCodeSerialSet2 = new QRCoder.QRCode(qRCodeSerialDataSet2);
            QRCoder.QRCode qRCodeSerialSet3 = new QRCoder.QRCode(qRCodeSerialDataSet3);

            Bitmap png = qRCode.GetGraphic(2);
            Bitmap pngSerial = qRCodeSerialSet1.GetGraphic(2);
            Bitmap pngSerialSET2 = qRCodeSerialSet2.GetGraphic(2); 
            Bitmap pngSerialSET3 = qRCodeSerialSet3.GetGraphic(2);

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
                pngSerialSET2.Save(ms, ImageFormat.Png);

                DataSet1 dataSet1 = new DataSet1();
                DataSet1.QrcodeSerialSet2Row qrcodeSerialSet2Row = dataSet1.QrcodeSerialSet2.NewQrcodeSerialSet2Row();
                qrcodeSerialSet2Row.imageSerialSet2 = ms.ToArray();
                dataSet1.QrcodeSerialSet2.AddQrcodeSerialSet2Row(qrcodeSerialSet2Row);

                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "ReportQrSerialSet2";
                reportDataSource.Value = dataSet1.QrcodeSerialSet2;
                reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            }
            using (MemoryStream ms = new MemoryStream())
            {
                pngSerialSET3.Save(ms, ImageFormat.Png);

                DataSet1 dataSet1 = new DataSet1();
                DataSet1.QrcodeSerialSet3Row qrcodeSerialSet3Row = dataSet1.QrcodeSerialSet3.NewQrcodeSerialSet3Row();
                qrcodeSerialSet3Row.imageSerialSet3 = ms.ToArray();
                dataSet1.QrcodeSerialSet3.AddQrcodeSerialSet3Row(qrcodeSerialSet3Row);

                ReportDataSource reportDataSource = new ReportDataSource();
                reportDataSource.Name = "ReportQrSerialSet3";
                reportDataSource.Value = dataSet1.QrcodeSerialSet3;
                reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            }
            reportViewer1.LocalReport.SetParameters(rpPaara);
            reportViewer1.LocalReport.Refresh();
            reportViewer1.RefreshReport();
            this.reportViewer1.RefreshReport();
        }
    }
}
