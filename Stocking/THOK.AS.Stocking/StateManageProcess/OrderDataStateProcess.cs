using System;
using System.Collections.Generic;
using System.Text;

using THOK.MCP;
using THOK.Util;
using THOK.AS.Stocking.StateManageProcess.Dao;

namespace THOK.AS.Stocking.StateManageProcess
{
    class OrderDataStateProcess:AbstractProcess
    {
        private bool isStockOut = false;

        /// <summary>
        /// 状态管理器列表
        /// </summary>
        private IDictionary<string, OrderDataStateManage> orderDataStateManages = new Dictionary<string, OrderDataStateManage>();
        private OrderDataStateManage GetStateManage(string stateItemCode)
        {
            if (!orderDataStateManages.ContainsKey(stateItemCode))
            {
                lock (orderDataStateManages)
                {
                    if (!orderDataStateManages.ContainsKey(stateItemCode))
                    {
                        orderDataStateManages[stateItemCode] = new OrderDataStateManage(stateItemCode,this.Context.ProcessDispatcher);
                    }
                }
            }
            return orderDataStateManages[stateItemCode];
        }

        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            /*
             * stateItem.Name ： 消息来源 
             * stateItem.ItemName ：
             *      对应 (0)Init                   ： 初始化（下载新数据时的初始化！）
             *           (1)StateItemCode_OrderDataMoveNext ： 来自PLC数据单元，请求写订单，PLC将用当前经过件烟流水号，请求写订单数据！
             *           (2)StateItemCode_OrderDataMoveTo   ： 来自PLC数据单元，请求较正数据，PLC将用当前经过件烟流水号，请求较正数据！
             *           
             * stateItem.State ：来自PLC数据块的流水号。
             */
            try
            {
                using (PersistentManager pm = new PersistentManager())
                {
                    if (stateItem.ItemName == "Init")
                    {
                        foreach (string stateCode in (new OrderDataStateManage()).GetStateItemCodeList())
                        {
                            GetStateManage(stateCode);
                        }

                        foreach (OrderDataStateManage orderDataStateManageItem in orderDataStateManages.Values)
                        {
                            orderDataStateManageItem.MoveTo(1);
                        }
                        return;
                    }

                    if (stateItem.ItemName == "Start")
                    {
                        isStockOut = true;
                        return;
                    }
                    if (stateItem.ItemName == "Stop")
                    {
                        isStockOut = false;
                        return;
                    }

                    if (!isStockOut)
                    {
                        return;
                    }

                    string stateItemCode = stateItem.ItemName.Split('_')[0];
                    string action = stateItem.ItemName.Split('_')[1];
                    OrderDataStateManage orderDataStateManage = GetStateManage(stateItemCode);
                    int index = 0;

                    switch (action)
                    {
                        case "OrderDataMoveNext":
                            index = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                            if (index != 0 && orderDataStateManage.Check(index))
                            {                               
                                if (!orderDataStateManage.WriteToPlc())
                                {
                                    Logger.Info(string.Format("{0} 号订单请求，订单数据写入失败流水号：[{1}]", stateItemCode, index));
                                }
                            }
                            break;
                        case "OrderDataMoveTo":
                            index = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                            if (index != 0)
                            {
                                orderDataStateManage.MoveTo(index);
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("OrderDataStateProcess.StateChanged() 处理失败！原因：" + e.Message);
            }
        }
    }
}
