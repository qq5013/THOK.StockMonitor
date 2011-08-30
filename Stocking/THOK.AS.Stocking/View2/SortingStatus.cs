using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using THOK.MCP.View;
namespace THOK.AS.Stocking.View
{
    public partial class SortingStatus : ProcessControl
    {
        public SortingStatus()
        {
            InitializeComponent();
        }
        private delegate void RefreshDelegate(RefreshData refreshData);

        private void Refresh(RefreshData refreshData)
        {
            if (InvokeRequired)
            {
                RefreshDelegate refreshDelegate = new RefreshDelegate(Refresh);
                Invoke(refreshDelegate, refreshData);
            }
            else
            {
                lblCompleteRoute.Text = refreshData.CompleteRoute.ToString();
                lblCompleteCustomer.Text = refreshData.CompleteCustomer.ToString();
                lblCompleteQuantity.Text = refreshData.CompleteQuantity.ToString();
                lblTotalRoute.Text = refreshData.TotalRoute.ToString();
                lblTotalCustomer.Text = refreshData.TotalCustomer.ToString();
                lblTotalQuantity.Text = refreshData.TotalQuantity.ToString();
                lblRoute.Text = (refreshData.TotalRoute - refreshData.CompleteRoute).ToString();
                lblCustomer.Text = (refreshData.TotalCustomer - refreshData.CompleteCustomer).ToString();
                lblQuantity.Text = (refreshData.TotalQuantity - refreshData.CompleteQuantity).ToString();
            }
        }

        public override void Process(THOK.MCP.StateItem stateItem)
        {
            RefreshData refreshData = (RefreshData)stateItem.State;
            Util.SerializableUtil.Serialize(true, @".\" + Name + ".RefreshData", refreshData);
            Refresh(refreshData);
        }
        public void SetLineName(string lineName)
        {
            lblLineName.Text = lineName;
            RefreshData refreshData = Util.SerializableUtil.Deserialize <RefreshData >(true, @".\" + Name + ".RefreshData");
            Refresh(refreshData);
        }
    }
}
