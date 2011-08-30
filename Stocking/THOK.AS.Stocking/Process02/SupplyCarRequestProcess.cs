using System;
using System.Collections.Generic;
using System.Text;
using THOK.MCP;
using THOK.AS.Stocking.Dao;
using System.Data;
using System.Windows.Forms;
using THOK.Util;

namespace THOK.AS.Stocking.Process
{
    class SupplyCarRequestProcess : AbstractProcess
    {
        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            try
            {
                object o = ObjectUtil.GetObject(stateItem.State);
                if (o == null || o.ToString() == "0")
                {
                    return;
                }

                //查询AS_STOCK_OUT表下发数据目标烟道给PLC
                using (PersistentManager pm = new PersistentManager())
                {
                    StockOutDao outDao = new StockOutDao();
                    string spupplyCarCode = stateItem.ItemName.Substring(stateItem.ItemName.Length - 2, 2);

                    DataTable outTable = outDao.FindCigaretteForSupplyCar(spupplyCarCode);
                    if (outTable.Rows.Count != 0)
                    {
                        string text = "";
                        switch (o.ToString())
                        {
                            case "1":
                                int channelAddress = Convert.ToInt32(outTable.Rows[0]["GROUPNO"]);
                                dispatcher.WriteToService("StockPLC", stateItem.ItemName + "_Data", channelAddress);
                                break;
                            case "2"://成功完成补货更新状态                               
                                outDao.UpdateSupplyCarStatus(outTable.Rows[0]["STOCKOUTID"].ToString());
                                break;
                            case "4"://自动补货失败，则进行手工补货，但也要更新状态
                                text = string.Format("{0} 号小车补货当前卷烟品牌为‘{1}’，烟道号为 ‘{2}’号， 请确认！", spupplyCarCode, outTable.Rows[0]["CIGARETTENAME"], outTable.Rows[0]["GROUPNO"]);
                                MessageBox.Show(text, "询问", MessageBoxButtons.OK , MessageBoxIcon.Question);
                                break;
                            default:
                                break;
                        }
                        
                    }
                }
            }
            catch (Exception ee)
            {
                Logger.Error("补货小车补货请求处理失败，原因：" + ee.Message);
            }
        }
    }
}
