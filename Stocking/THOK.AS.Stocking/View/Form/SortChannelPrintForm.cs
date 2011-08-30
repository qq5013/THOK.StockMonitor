using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using THOK.AS.Stocking.Report ;
using THOK.AS.Stocking.Dao;
using System.Threading;
using THOK.Util;

namespace THOK.AS.Stocking.View
{
    public partial class SortChannelPrintForm : THOK.AF.View.ToolbarForm 
    {
        public delegate void PrintHandler(SortChannelReport allTaskDataSet);
        public delegate void LblInfoHandler(string info);
        private  SortChannelReport channelReport = null;
        Thread t = null;

        public SortChannelPrintForm()
        {
            InitializeComponent();
            //lblInfo.Text = "数据加载中……";
            //t = new Thread(new ThreadStart(GetDataSet));
            //t.IsBackground = false;
            //t.Start();
        }

        private void GetDataSet()
        {
            try
            {
                using (PersistentManager pmServer = new PersistentManager("ServerConnection"))
                {
                    ServerDao serverDao = new ServerDao();
                    serverDao.SetPersistentManager(pmServer);
                    channelReport = new SortChannelReport();

                    DataTable printBatchTable = serverDao.FindPrintBatchTable();
                    PrintSelectDialog printSelectDialog = new PrintSelectDialog(printBatchTable);

                    if (printSelectDialog.ShowDialog() == DialogResult.OK)
                    {
                        string orderDate = "";
                        string batchNo = "";
                        string lineCode = "";

                        orderDate = printSelectDialog.SelectedPrintBatch.Split("|"[0])[0];
                        batchNo = printSelectDialog.SelectedPrintBatch.Split("|"[0])[1];
                        lineCode = printSelectDialog.SelectedPrintBatch.Split("|"[0])[2];

                        DataTable table = serverDao.FindChannelUSED(orderDate, batchNo, lineCode);

                        if (table.Rows.Count == 0)
                            throw new Exception("没有数据");
                        channelReport.SetDataSource(table);
                        SetReportToCrv(channelReport);
                    }
                }
            }
            catch (Exception ex)
            {
                SetLblInfo(ex.Message);
            }
            finally
            {
                //t.Abort();
            }
        }

        private void SetReportToCrv(SortChannelReport allTaskReport)
        {
            if (crvChannelReport.InvokeRequired)
            {
                crvChannelReport.Invoke(new PrintHandler(SetReportToCrv), allTaskReport);
            }
            else
            {
                lblInfo.Visible = false;
                crvChannelReport.ReportSource = allTaskReport;
            }
        }

        private void SetLblInfo(string info)
        {
            if (lblInfo.InvokeRequired)
            {
                crvChannelReport.Invoke(new LblInfoHandler(SetLblInfo), info);
            }
            else
            {
                lblInfo.Text = info;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            lblInfo.Text = "数据加载中……";
            //t = new Thread(new ThreadStart(GetDataSet));
            //t.IsBackground = false;
            //t.Start();
            GetDataSet();
        }
    }
}