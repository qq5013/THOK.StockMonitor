using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.MCP;
using THOK.Util;
using THOK.AS.Stocking.Dao;

namespace THOK.AS.Stocking.StockOutProcess
{
    public class SupplyRequestProcess: AbstractProcess
    {
        [Serializable]
        public class OrderInfo
        {
            public string orderDate = "";
            public string batchNo = "";
        }
        private OrderInfo orderInfo = new OrderInfo();
        private bool isOk = false;
        private string orderDate = "";
        private string batchNo = "";

        public override void Initialize(Context context)
        {
            base.Initialize(context);
            orderInfo = Util.SerializableUtil.Deserialize<OrderInfo>(true, @".\orderInfo.sl");
        }
        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            try
            {
                bool needNotify = false;
                switch (stateItem.ItemName)
                {
                    case "OrderInfo":
                        object o = stateItem.State;
                        if (o is Array)
                        {
                            Array array = (Array)o;
                            if (array.Length == 2)
                            {
                                string[] orderinfo = new string[2];
                                array.CopyTo(orderinfo, 0);
                                orderInfo.orderDate = orderinfo[0];
                                orderInfo.batchNo = orderinfo[1];
                            }
                        }

                        Util.SerializableUtil.Serialize(true, @".\orderInfo.sl", orderInfo);
                        return;
                        break;
                    case "FirstBatch":
                        needNotify = AddFirstSupply();
                        break;
                    case "SupplyRequest":
                        Dictionary<string, string> parameter = (Dictionary<string, string>)stateItem.State;

                        if ((!isOk || orderDate != orderInfo.orderDate || batchNo != orderInfo.batchNo) && (parameter["OrderDate"] != orderInfo.orderDate || parameter["BatchNo"] != orderInfo.batchNo))
                        {
                            if (parameter["OrderDate"] != "")
                            {
                                Logger.Error("补货批次请求处理失败，原因：分拣订单批次与补货订单批次不符！");
                            }
                            return;
                        }

                        isOk = true;
                        orderDate = orderInfo.orderDate;
                        batchNo = orderInfo.batchNo;

                        needNotify = AddSupply(parameter["LineCode"], parameter["SortNo"], parameter["ChannelGroup"]);
                        break;
                }
                if (needNotify)
                    dispatcher.WriteToProcess("DataRequestProcess", "SupplyRequest", 1);
            }
            catch (Exception e)
            {
                Logger.Error("补货批次生成处理失败，原因：" + e.Message);
            }
        }

        private bool AddFirstSupply()
        {
            bool result = false;
            try
            {                
                using (PersistentManager pm = new PersistentManager())
                {
                    StockOutBatchDao batchDao = new StockOutBatchDao();
                    SupplyDao supplyDao = new SupplyDao();
                    StockOutDao outDao = new StockOutDao();

                    DataTable supplyTable = supplyDao.FindFirstSupply();

                    if (supplyTable.Rows.Count != 0)
                    {
                        try
                        {
                            pm.BeginTransaction();

                            int batchNo = batchDao.FindMaxBatchNo() + 1;
                            batchDao.InsertBatch(batchNo, "00", "0", supplyTable.Rows.Count,"0");

                            int outID = outDao.FindMaxOutID();
                            outDao.Insert(outID, batchNo, supplyTable);

                            pm.Commit();
                            result = true;

                            Logger.Info("生产第一批次出库任务成功");
                            WriteToProcess("LedStateProcess", "Refresh", null);
                        }
                        catch (Exception e)
                        {
                            Logger.Error("生成第一批次出库任务失败，原因：" + e.Message);
                            pm.Rollback();
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Logger.Error("第一补货批次生成处理失败，原因：" + ee.Message);
            }

            return result;
        }        

        private bool AddSupply(string lineCode, string sortNo,string channelGroup)
        {            
            bool result = false;
            try
            {
                using (PersistentManager pm = new PersistentManager())
                {
                    StockOutBatchDao batchDao = new StockOutBatchDao();
                    SupplyDao supplyDao = new SupplyDao();
                    StockOutDao outDao = new StockOutDao();

                    DataTable table = supplyDao.FindSupplyBatch(lineCode, sortNo,channelGroup);

                    foreach (DataRow row in table.Rows)
                    {
                        string tmpSortNo = row["SORTNO"].ToString();

                        DataTable supplyTable = supplyDao.FindSupply(lineCode, tmpSortNo,channelGroup);

                        if (supplyTable.Rows.Count != 0)
                        {
                            Logger.Info(string.Format("收到补货请求，生产线'{0}'，流水号'{1}'", lineCode, tmpSortNo));
                            try
                            {
                                pm.BeginTransaction();

                                int batchNo = batchDao.FindMaxBatchNo() + 1;
                                batchDao.InsertBatch(batchNo, lineCode, tmpSortNo, supplyTable.Rows.Count,channelGroup);

                                int outID = outDao.FindMaxOutID();
                                outDao.Insert(outID, batchNo, supplyTable);

                                pm.Commit();
                                result = true;

                                Logger.Info("生成出库任务成功");
                                WriteToProcess("LedStateProcess", "Refresh", null);
                            }
                            catch (Exception e)
                            {
                                Logger.Error("生成出库任务失败，原因：" + e.Message);
                                pm.Rollback();
                            }
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Logger.Error("补货点批次生成处理失败，原因：" + ee.Message);
            }
            return result;
        }
    }
}
