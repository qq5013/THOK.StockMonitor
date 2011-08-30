using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using THOK.AS.Stocking.Dal;

namespace THOK.AS.Stocking.View
{
    public partial class StockOutTaskForm : THOK.AF.View.ToolbarForm
    {
        public StockOutTaskForm()
        {
            InitializeComponent();
            this.Column2.FilteringEnabled = true;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            StockBatchDal batchDal = new StockBatchDal();
            DataTable table = batchDal.GetAll();
            bsMain.DataSource = table;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Exit();
        }
    }
}

