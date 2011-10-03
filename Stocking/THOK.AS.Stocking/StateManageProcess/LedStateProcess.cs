using System;
using System.Collections.Generic;
using System.Text;
using THOK.MCP;
using THOK.Util;
using THOK.AS.Stocking.StateManageProcess.Dao;

namespace THOK.AS.Stocking.StateManageProcess
{
    class LedStateProcess : AbstractProcess
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
                        ledStateManages[stateItemCode] = new LedStateManage(stateItemCode,this.Context.ProcessDispatcher);
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
             *      对应    (0)Init                      ： 初始化（下载新数据时的初始化！）
             *              (1)Refresh                   ： 刷新数据
             *              (2)StateItemCode_LedMoveNext ： 来自PLC数据单元，请求件烟通过，PLC将用当前经过件烟流水号，请求件烟通过！
             *              (3)StateItemCode_LedMoveTo   ： 来自PLC数据单元，请求较正数据，PLC将用当前经过件烟流水号，请求较正数据！
             *              (4)StateItemCode_LedShowData ： 来自PLC数据单元，请求刷新数据
             * stateItem.State ：来自PLC数据块的流水号。
             */
            try
            {
                using (PersistentManager pm = new PersistentManager())
                {
                    if (stateItem.ItemName == "Init")
                    {
                        foreach (string stateCode in (new LedStateManage()).GetStateItemCodeList())
                        {
                            GetStateManage(stateCode);
                        }

                        foreach (LedStateManage ledStateManageItem in ledStateManages.Values)
                        {
                            ledStateManageItem.MoveTo(1);
                        }
                        return;
                    }

                    if (stateItem.ItemName == "Refresh")
                    {
                        foreach (LedStateManage ledStateManageItem in ledStateManages.Values)
                        {
                            ledStateManageItem.ShowData();
                        }
                        return;
                    }

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
                                        if(!ledStateManage.WriteToPlc())
                                        {
                                            Logger.Info(string.Format("{0} 号LED请求通过，写入放行失败流水号：[{1}]", stateItemCode, index));
                                        }
                                    }
                                }
                                ledStateManage.ShowData();
                            }
                            break;
                        case "LedMoveTo":
                            index = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                            if (index != 0)
                            {
                                ledStateManage.MoveTo(index);
                                Logger.Info(string.Format("{0} 号LED，校正完成,流水号：{1}", stateItemCode, index));
                                ledStateManage.ShowData();
                            }                            
                            break;
                        case "LedShowData":
                            index = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                            if (index != 0 && ledStateManage.Check(index))
                            {
                                ledStateManage.ShowData(index-1);
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("LedStateProcess.StateChanged() 处理失败！原因：" + e.Message);
            }         
        }
    }
}
