using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using THOK.MCP;
using THOK.Util;
using THOK.AS.Stocking.Dao;

namespace THOK.AS.Stocking.Process
{
    public class DataReuqestProcess: AbstractProcess
    {
        private int dataRequest = 0;

        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            try
            {
                //下数据的条件,1、PLC允许写数据，2、有出库数据
                if (stateItem.ItemName == "DataRequest")
                    dataRequest = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                //if (dataRequest == 0)
                //    dataRequest = Convert.ToInt32(ObjectUtil.GetObject(Context.Services["StockPLC_01"].Read("DataRequest")));

                int? a = null ;

                int b = a??0;

                if (dataRequest == 1)
                {
                    using (PersistentManager pm = new PersistentManager())
                    {
                        StockOutBatchDao batchDao = new StockOutBatchDao();
                        StockOutDao outDao = new StockOutDao();
                        StockInDao stockInDao = new StockInDao();

                        DataTable batchTable = batchDao.FindBatch();
                        DataTable stockInTable = stockInDao.FindStockInForIsInAndNotOut();

                        if (batchTable.Rows.Count != 0)
                        {
                            foreach (DataRow row in batchTable.Rows)
                            {
                                try
                                {
                                    string batchNo = row["BATCHNO"].ToString();
                                    DataTable outTable = outDao.FindSupply(batchNo);
                                    int count = 0 ;
                                    int outCount = 0 ;
                                    if (outTable.Rows.Count > 0)
                                    {
                                        if (dataRequest == 0)
                                        {
                                            return;
                                        }

                                        if (Convert.ToBoolean(Context.Attributes["IsMerge"]))
                                        {
                                            outTable = outDao.FindSupply();
                                        }                                        

                                        int[] data = new int[162];

                                        pm.BeginTransaction();

                                        string tmp = "";
                                        for (int i = 0; i < outTable.Rows.Count; i++)
                                        {
                                            DataRow[] stockInRows = stockInTable.Select(string.Format("CIGARETTECODE='{0}' AND STATE ='1' AND ( STOCKOUTID IS NULL OR STOCKOUTID = 0 )", outTable.Rows[i]["CIGARETTECODE"].ToString()), "STOCKINID");

                                            if ( stockInRows.Length<=  Convert.ToInt32(Context.Attributes["StockInRequestRemainQuantity"]) + 1 )
                                            {
                                                WriteToProcess("StockInRequestProcess", "StockInRequest", outTable.Rows[i]["CIGARETTECODE"].ToString());
                                            }
                                            else if (stockInRows.Length > 0 && stockInRows.Length + Convert.ToInt32(stockInRows[0]["STOCKINQUANTITY"])  <= 30 + 1)
                                            {
                                                WriteToProcess("StockInRequestProcess", "StockInRequest", outTable.Rows[i]["CIGARETTECODE"].ToString());
                                            }

                                            if (stockInRows.Length > 0)
                                            {
                                                if (outTable.Rows[i]["BATCHNO"].ToString() == batchNo)
                                                {
                                                    count++;
                                                }
                                                outCount++;

                                                data[i] = Convert.ToInt32(outTable.Rows[i]["SCHANNELCODE"]);
                                                tmp += string.Format("[{0}]", data[i]);

                                                stockInRows[0]["STOCKOUTID"] = outTable.Rows[i]["STOCKOUTID"].ToString();
                                                outTable.Rows[i]["STATE"] = 1;
                                            }
                                            else
                                            {
                                                Logger.Error(string.Format("[{0}] [{1}] 库存不足！", outTable.Rows[i]["CIGARETTECODE"].ToString(), outTable.Rows[i]["CIGARETTENAME"].ToString()));
                                                WriteToProcess("LEDProcess","StockInRequestShow", outTable.Rows[0]["CIGARETTENAME"]);
                                                break;
                                            }
                                        }

                                        if (outCount == 0)
                                        {                                            
                                            return;
                                        }

                                        data[160] = outCount;
                                        data[161] = 1;
                                        //写PLC
                                        if (dispatcher.WriteToService("StockPLC_01", "OutData", data))
                                        {
                                            outDao.UpdateStatus(outTable);
                                            batchDao.UpdateBatch(batchNo, count);
                                            stockInDao.UpdateStockOutIdToStockIn(stockInTable);
                                        }

                                        pm.Commit();

                                        int j = 0;
                                        while (dataRequest != 0)
                                        {
                                            System.Threading.Thread.Sleep(50);
                                            dataRequest = Convert.ToInt32(ObjectUtil.GetObject(Context.Services["StockPLC_01"].Read("DataRequest")));
                                            j++;
                                            if (j > 50)
                                            {
                                                ShowMessageBox("PLC  数据返回异常，请重启软件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);          
                                                break;
                                            }
                                        }
                                        
                                        dataRequest = 0;
                                        Logger.Info("写出库数据成功。" + tmp);
                                    }
                                    else
                                        batchDao.UpdateBatch(batchNo, Convert.ToInt32(row["QUANTITY"].ToString()) - Convert.ToInt32(row["OUTQUANTITY"].ToString()));
                                }
                                catch (Exception e)
                                {
                                    Logger.Error("写出库数据失败，原因：" + e.Message);
                                    pm.Rollback();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("补货订单请求处理失败，原因：" + e.Message);
            }
        }

        private void ShowMessageBox(string msg, string title, MessageBoxButtons messageBoxButtons, MessageBoxIcon messageBoxIcon)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("msg", msg);
            parameters.Add("title", title);
            parameters.Add("messageBoxButtons", messageBoxButtons);
            parameters.Add("messageBoxIcon", messageBoxIcon);
            WriteToProcess("buttonArea", "MessageBox", parameters);
        }
    }
}
