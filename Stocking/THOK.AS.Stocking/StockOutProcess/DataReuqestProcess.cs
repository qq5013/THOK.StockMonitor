using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Windows.Forms;
using THOK.MCP;
using THOK.Util;
using THOK.AS.Stocking.Dao;

namespace THOK.AS.Stocking.StockOutProcess
{
    public class DataReuqestProcess: AbstractProcess
    {
        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            try
            {
                using (PersistentManager pm = new PersistentManager())
                {
                    StockOutBatchDao stockOutBatchDao = new StockOutBatchDao();
                    StockOutDao stockOutDao = new StockOutDao();
                    StockInDao stockInDao = new StockInDao();

                    DataTable batchTable = stockOutBatchDao.FindBatch();
                    DataTable stockInTable = stockInDao.FindStockInForIsInAndNotOut();

                    if (batchTable.Rows.Count != 0)
                    {
                        foreach (DataRow row in batchTable.Rows)
                        {
                            try
                            {
                                string batchNo = row["BATCHNO"].ToString();
                                DataTable outTable = stockOutDao.FindSupply(batchNo);
                                int count = 0;
                                int outCount = 0;

                                if (outTable.Rows.Count > 0)
                                {
                                    if (Convert.ToBoolean(Context.Attributes["IsMerge"]))
                                    {
                                        outTable = stockOutDao.FindSupply();
                                    }

                                    pm.BeginTransaction();

                                    for (int i = 0; i < outTable.Rows.Count; i++)
                                    {
                                        DataRow[] stockInRows = stockInTable.Select(string.Format("CIGARETTECODE='{0}' AND STATE ='1' AND ( STOCKOUTID IS NULL OR STOCKOUTID = 0 )", 
                                            outTable.Rows[i]["CIGARETTECODE"].ToString()), "STOCKINID");

                                        if (stockInRows.Length <= Convert.ToInt32(Context.Attributes["StockInRequestRemainQuantity"]) + 1)
                                        {
                                            WriteToProcess("StockInRequestProcess", "StockInRequest", outTable.Rows[i]["CIGARETTECODE"].ToString());
                                        }
                                        else if (stockInRows.Length > 0 && stockInRows.Length + Convert.ToInt32(stockInRows[0]["STOCKINQUANTITY"]) <= 30 + 1)
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

                                            stockInRows[0]["STOCKOUTID"] = outTable.Rows[i]["STOCKOUTID"].ToString();
                                            outTable.Rows[i]["STATE"] = 1;
                                        }
                                        else
                                        {
                                            Logger.Error(string.Format("[{0}] [{1}] 库存不足！", outTable.Rows[i]["CIGARETTECODE"].ToString(), outTable.Rows[i]["CIGARETTENAME"].ToString()));
                                            WriteToProcess("LEDProcess", "StockInRequestShow", outTable.Rows[0]["CIGARETTENAME"]);
                                            break;
                                        }
                                    }

                                    if (outCount == 0)
                                    {
                                        return;
                                    }

                                    stockOutDao.UpdateStatus(outTable);
                                    stockOutBatchDao.UpdateBatch(batchNo, count);
                                    stockInDao.UpdateStockOutIdToStockIn(stockInTable);

                                    pm.Commit();
                                    Logger.Info("处理出库数据成功。");
                                }
                                else
                                    stockOutBatchDao.UpdateBatch(batchNo, Convert.ToInt32(row["QUANTITY"].ToString()) - Convert.ToInt32(row["OUTQUANTITY"].ToString()));
                            }
                            catch (Exception e)
                            {
                                Logger.Error("处理出库数据失败，原因：" + e.Message);
                                pm.Rollback();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("处理出库数据失败，原因：" + e.Message);
            }
        }
    }
}
