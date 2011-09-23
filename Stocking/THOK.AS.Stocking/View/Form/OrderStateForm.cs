using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using THOK.AS.Stocking.Dal;
using THOK.AS.Stocking.Dao;
using THOK.MCP;
using THOK.Util;

namespace THOK.AS.Stocking.View
{
    public partial class OrderStateForm : THOK.AF.View.ToolbarForm 
    {
        public OrderStateForm()
        {
            InitializeComponent();
            this.Column2.FilteringEnabled = true;
            this.Column3.FilteringEnabled = true;
            this.Column4.FilteringEnabled = true;
            this.Column5.FilteringEnabled = true;
            this.Column8.FilteringEnabled = true;
         }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            GetDataSet();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void GetDataSet()
        {
            try
            {
                using (PersistentManager pm = new PersistentManager())
                {
                    StateDao stateDao=new StateDao();
                    DataTable FindStateQueryType=stateDao.FindStateQueryTypeTable();
                    StateQueryDialog stateQueryDialog = new StateQueryDialog(FindStateQueryType);
                    
                    if (stateQueryDialog.ShowDialog() == DialogResult.OK)
                    {
                        string stateCode = "";
                        string indexNo = "";

                        stateCode = stateQueryDialog.SelectedQueryType.ToString();
                        indexNo = stateDao.FindScannerIndexNoByStateCode(stateCode).Rows[0]["INDEXNO"].ToString();
                        bsMain.DataSource = stateDao.FindOrderStateByIndexNo(indexNo);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }
    }
}