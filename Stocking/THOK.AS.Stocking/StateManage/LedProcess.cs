using System;
using System.Collections.Generic;
using System.Text;
using THOK.MCP;
using THOK.Util;

namespace THOK.AS.Stocking.StateManage
{
    class LedProcess : AbstractProcess
    {
        /// <summary>
        /// 状态管理器列表
        /// </summary>
        private IDictionary<string,LedStateManage> ledStateManages = new Dictionary<string,LedStateManage>();
        private LedStateManage GetStateManage(string stateItemCode)
        {
            if (!ledStateManages.ContainsKey(stateItemCode))
            {
                lock (ledStateManages)
                {
                    if (!ledStateManages.ContainsKey(stateItemCode))
                    {
                        ledStateManages[stateItemCode] = new LedStateManage(stateItemCode);
                    }
                }                
            }
            return ledStateManages[stateItemCode];
        }

        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            /*
             * stateItem.Name ： 消息来源 。
             * stateItem.ItemName ： 
             *      对应 (1).StateItemCode_MoveNext /? 来自PLC数据单元，请求件烟通过，PLC将用当前经过件烟流水号，请求件烟通过！
             *           (2).StateItemCode_MoveTo   /? 来自PLC数据单元，请求较正数据，PLC将用当前经过件烟流水号，请求较正数据！
             *           
             * stateItem.State ：来自PLC数据块的流水号。
             */
            try
            {
                if (stateItem.ItemName == "Init")
                {
                    foreach (LedStateManage ledStateManage in ledStateManages.Values)
                    {
                        ledStateManage.MoveTo(1);
                    }
                    return;
                }

                using (PersistentManager pm = new PersistentManager())
                {
                    string stateItemCode = stateItem.ItemName.Split('_')[0];
                    string action = stateItem.ItemName.Split('_')[1];
                    LedStateManage ledStateManage = GetStateManage(stateItemCode);
                    int index = 0;
                    switch (action)
                    {
                        case "LedMoveNext":
                            index = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                            if (index != 0 && ledStateManage.Check(index))
                            {
                                if (ledStateManage.MoveTo(index))
                                {
                                    if (ledStateManage.MoveNext())
                                    {
                                        ledStateManage.WriteToPlc(dispatcher);
                                    }
                                }
                            }
                            break;
                        case "LedMoveTo":
                            index = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                            if (index != 0)
                            {
                                ledStateManage.MoveTo(index);
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("LedProcess.StateChanged() 处理失败！原因：" + e.Message);
            }         
        }
    }
}
