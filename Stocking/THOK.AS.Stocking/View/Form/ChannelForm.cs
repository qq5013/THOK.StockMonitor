using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using THOK.AS.Stocking.Dal;
using THOK.MCP;

namespace THOK.AS.Stocking.View
{
    public partial class ChannelForm : THOK.AF.View.ToolbarForm
    {
        public ChannelForm()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ChannelDal channelDal = new ChannelDal();
            DataTable table = channelDal.GetAllChannel();
            bsMain.DataSource = table;
        }
    }
}

