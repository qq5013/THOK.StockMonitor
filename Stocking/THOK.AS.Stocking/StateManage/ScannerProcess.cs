using System;
using System.Collections.Generic;
using System.Text;
using THOK.MCP;
using THOK.Util;

namespace THOK.AS.Stocking.StateManage
{
    class ScannerProcess : AbstractProcess
    {
        /// <summary>
        /// 状态管理器列表
        /// </summary>
        private IDictionary<string, ScannerStateManage> scannerStateManages = new Dictionary<string, ScannerStateManage>();
        private ScannerStateManage GetStateManage(string stateItemCode)
        {
            if (!scannerStateManages.ContainsKey(stateItemCode))
            {
                lock (scannerStateManages)
                {
                    if (!scannerStateManages.ContainsKey(stateItemCode))
                    {
                        scannerStateManages[stateItemCode] = new ScannerStateManage(stateItemCode);
                    }
                }
            }
            return scannerStateManages[stateItemCode];
        }

        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            /*
             * stateItem.Name ： 消息来源 
             * stateItem.ItemName ： 
             *      对应 (1).StateItemCode_MoveNext /? 来自PLC数据单元，请求件烟通过，PLC将用当前经过件烟流水号，请求件烟通过！
             *           (2).StateItemCode_MoveTo   /? 来自PLC数据单元，请求较正数据，PLC将用当前经过件烟流水号，请求较正数据！
             *           (3).StateItemCode_ShowData /? 来自PLC数据单元，请求显示数据，PLC将用当前经过件烟流水号，请求显示数据！
             *           
             * stateItem.State
             */
            try
            {
                using (PersistentManager pm = new PersistentManager())
                {
                    string stateItemCode = stateItem.ItemName.Split('_')[0];
                    string action = stateItem.ItemName.Split('_')[1];
                    ScannerStateManage scannerStateManage = GetStateManage(stateItemCode);
                    int index = 0;
                    switch (action)
                    {
                        case "MoveNext":
                            index = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                            if (index != 0 && scannerStateManage.Check(index))
                            {
                                if (scannerStateManage.MoveNext())
                                {
                                    scannerStateManage.WriteToPlc(dispatcher);
                                }
                            }
                            break;
                        case "MoveTo":
                            index = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                            if (index != 0)
                            {
                                scannerStateManage.MoveTo(index);
                            }
                            break;
                        case "ShowData":
                            index = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                            if (index != 0 && scannerStateManage.Check(index))
                            {
                                scannerStateManage.ShowData(index);
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("ScannerProcess.StateChanged() 处理失败！原因：" + e.Message);
            }
        }
    }
}
