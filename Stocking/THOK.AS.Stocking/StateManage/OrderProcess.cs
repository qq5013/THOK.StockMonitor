using System;
using System.Collections.Generic;
using System.Text;

using THOK.MCP;
using THOK.Util;

namespace THOK.AS.Stocking.StateManage
{
    class OrderProcess:AbstractProcess
    {
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
                        orderDataStateManages[stateItemCode] = new OrderDataStateManage(stateItemCode);
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
             *      对应 (1).StateItemCode_MoveNext /? 来自PLC数据单元，请求写订单，PLC将用当前经过件烟流水号，请求写订单数据！
             *           (2).StateItemCode_MoveTo   /? 来自PLC数据单元，请求较正数据，PLC将用当前经过件烟流水号，请求较正数据！
             *           
             * stateItem.State  
             */
            try
            {
                if (stateItem.ItemName == "Init")
                {
                    foreach (OrderDataStateManage orderDataStateManage in orderDataStateManages.Values)
                    {
                        orderDataStateManage.MoveTo(1, dispatcher);
                    }
                    return;
                }

                using (PersistentManager pm = new PersistentManager())
                {
                    string stateItemCode = stateItem.ItemName.Split('_')[0];
                    string action = stateItem.ItemName.Split('_')[1];
                    OrderDataStateManage orderDataStateManage = GetStateManage(stateItemCode);
                    int index = 0;
                    switch (action)
                    {
                        case "OrderMoveNext":
                            index = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                            if (index != 0 && orderDataStateManage.Check(index))
                            {
                                if (orderDataStateManage.MoveNext())
                                {
                                    orderDataStateManage.WriteToPlc(dispatcher);
                                }
                            }
                            break;
                        case "OrderMoveTo":
                            index = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                            if (index != 0)
                            {
                                orderDataStateManage.MoveTo(index, dispatcher);
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("OrderProcess.StateChanged() 处理失败！原因：" + e.Message);
            }
        }
    }
}
