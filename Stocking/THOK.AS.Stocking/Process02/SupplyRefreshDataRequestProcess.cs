using System;
using System.Collections.Generic;
using System.Text;
using THOK.MCP;

namespace THOK.AS.Stocking.Process
{
    class SupplyRefreshDataRequestProcess : AbstractProcess
    {
        View.MaxLEDForm ledForm = null;
        public override void Initialize(Context context)
        {
            base.Initialize(context);
            context.OnPostInitialize += new EventHandler(context_OnPostInitialize);
        }

        void context_OnPostInitialize(object sender, EventArgs e)
        {
            ledForm = new THOK.AS.Stocking.View.MaxLEDForm();
            ledForm.Initialize(Context.Attributes);

            ledForm.sortingStatus_Line01.SetLineName("一号分拣线");
            ledForm.sortingStatus_Line02.SetLineName("二号分拣线");
            Context.RegisterProcessControl (ledForm.sortingStatus_Line01);
            Context.RegisterProcessControl (ledForm.sortingStatus_Line02);

            ledForm.Show();
        }

        public override void Release()
        {
            base.Release();
            ledForm.Close();
            ledForm = null;
        }
        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            if (stateItem.ItemName  == "RefreshData")
            {
                RefreshData refreshData = new RefreshData();
                string lineCode = "";
                string sortNo = "";

                Dictionary<string, string> parameter = (Dictionary<string, string>)stateItem.State;
                lineCode = parameter["LineCode"];
                sortNo = parameter["SortNo"];

                refreshData.TotalCustomer = Convert.ToInt32(parameter["TotalCustomer"]);
                refreshData.TotalRoute = Convert.ToInt32(parameter["TotalRoute"]);
                refreshData.TotalQuantity = Convert.ToInt32(parameter["TotalQuantity"]);

                refreshData.CompleteCustomer = Convert.ToInt32(parameter["CompleteCustomer"]);
                refreshData.CompleteRoute = Convert.ToInt32(parameter["CompleteRoute"]);
                refreshData.CompleteQuantity = Convert.ToInt32(parameter["CompleteQuantity"]);

                switch (lineCode)
                {
                    case "01":
                        dispatcher.WriteToProcess("sortingStatus_Line01", "RefreshData", refreshData);
                        break;
                    case "02":
                        dispatcher.WriteToProcess("sortingStatus_Line02", "RefreshData", refreshData);
                        break;
                    default:
                        break;
                }                  
            }
        }
    }
}
