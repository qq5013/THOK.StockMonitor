using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using THOK.AS.Stocking.Dao;
using THOK.AS.Stocking.Util;
using THOK.MCP;
using THOK.Util;

namespace THOK.AS.Stocking.Process
{
    public class Scan_O1_Process : AbstractProcess
    {      
        [Serializable]
        public class SerializableScannerParameters
        {
            private Dictionary<string, Dictionary<string, object>> Parameters = new Dictionary<string, Dictionary<string, object>>();

            public void SetParameter(string scannerCode,string parameterName,object parameterValue)
            {
                Dictionary<string, object> param = null;
                if (Parameters.ContainsKey(scannerCode))
                {
                    param = Parameters[scannerCode];
                }
                else
                {
                    param = new Dictionary<string, object>();
                }

                param[parameterName] = parameterValue;

                Parameters[scannerCode] = param;

                Serialize();
            }

            public object GetParameter(string scannerCode,string parameterName)
            {
                Dictionary<string, object> param = null;
                if (Parameters.ContainsKey(scannerCode))
                {
                    param = Parameters[scannerCode];
                    if (param.ContainsKey(parameterName))
                    {
                        return param[parameterName];
                    }
                    else
                        return null;
                }
                else
                    return null;
            }

            private void Serialize()
            {
                SerializableUtil.Serialize(true, @".\Scan_O1_ProcessParameters.sl", this);
            }

            public static SerializableScannerParameters Deserialize()
            {
                return SerializableUtil.Deserialize<SerializableScannerParameters>(true, @".\Scan_O1_ProcessParameters.sl"); 
            }

            public void Init()
            {
                Parameters.Clear();
                Serialize();
            }
        }

        private SerializableScannerParameters scannerParameters = new SerializableScannerParameters();

        public override void Initialize(Context context)
        {
            base.Initialize(context);
            scannerParameters = SerializableScannerParameters.Deserialize();
        }

        private string scan = "01";
        private bool isProcessingError = false;
        private int processingOrderNo = 0;
        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            /*  处理事项：
             *  stateItem.Name ： "" - 主动消息，SickScan - 扫码器消息， StockPLC_01 - 叠垛缓存线PLC消息
             *  stateItem.ItemName ：00 初始化扫码器参数， 01~05 分别对应5个扫码器，进行处理。
             *  stateItem.State ：参数。
             *      Barcode：条码/NOREAD/RESCAN_OK      
             *      OrderNo：当前件烟顺序号（故障恢复时须带本参数其他不需要）
            */

            try
            {
                string scannerCode = "";
                string barcode = "";
                int orderNo = 0;

                switch (stateItem.Name)
                {
                    case "":
                        switch (stateItem.ItemName)
                        {
                            case "Init":
                                scannerParameters.Init();
                                return;
                            case "01":
                                scannerCode = stateItem.ItemName;
                                if (stateItem.State != null && stateItem.State is Dictionary<string, string>)
                                {
                                    barcode = ((Dictionary<string, string>)stateItem.State)["barcode"];
                                    if (barcode == "NOREAD")
                                    {
                                        Logger.Error(string.Format("{0} 号条码扫描处理失败！详情：未扫到条码！", scannerCode));
                                        return;
                                    }
                                    if (processingOrderNo == (scannerParameters.GetParameter(scannerCode, "OrderNo") != null ? (int)scannerParameters.GetParameter(scannerCode, "OrderNo") : 0) + 1)
                                    {
                                        processingOrderNo = 0;
                                        Scanner_Process_StockIn(scannerCode, barcode);
                                    } 
                                }
                                return;
                            default:
                                return;
                        }
                    case "SickScan":
                        switch (stateItem.ItemName)
                        {
                            case "01":
                                scannerCode = stateItem.ItemName;
                                if (stateItem.State != null && stateItem.State is Dictionary<string, string>)
                                {
                                    barcode = ((Dictionary<string, string>)stateItem.State)["barcode"];
                                    if (barcode == "NOREAD")
                                    {
                                        Logger.Error(string.Format("{0} 号条码扫描处理失败！详情：未扫到条码！", scannerCode));
                                        return;
                                    }
                                    Scanner_Process_StockIn(scannerCode, barcode);
                                }
                                return;
                            default:
                                return;
                        }
                    case "StockPLC_01":
                        scannerCode = stateItem.ItemName.Split("_"[0])[1];  //处理的条码扫码器号
                        string info = stateItem.ItemName.Split("_"[0])[0];  //
                        orderNo = Convert.ToInt32(THOK.MCP.ObjectUtil.GetObject(stateItem.State));
                        
                        object state = THOK.MCP.ObjectUtil.GetObject(Context.Services["StockPLC_01"].Read("ErrTag_01"));
                        int errTag = state != null ? Convert.ToInt32(state) : 0;

                        if (info == "ErrTag")
                        {
                            if (orderNo == 1)
                            {
                                return;
                            }
                            else
                            {
                                return;
                            }
                        }

                        int supplyAddress = scannerParameters.GetParameter(scannerCode, "SupplyAddress") != null ? (int)scannerParameters.GetParameter(scannerCode, "SupplyAddress") : 0;
                        int change = scannerParameters.GetParameter(scannerCode, "Change") != null ? (int)scannerParameters.GetParameter(scannerCode, "Change") : 0;
                        int orderNo_sl = scannerParameters.GetParameter(scannerCode, "OrderNo") != null ? (int)scannerParameters.GetParameter(scannerCode, "OrderNo") : 0;

                        if (orderNo == orderNo_sl)
                        {
                            int[] data = new int[3];
                            data[0] = supplyAddress;
                            data[1] = change;
                            data[2] = orderNo_sl;
                            WriteToService("StockPLC_01", "Scanner_DirectoryData_" + scannerCode, data);
                        }
                        else if (orderNo == orderNo_sl + 1)
                        {
                            if (!isProcessingError && errTag == 1)
                            {
                                isProcessingError = true;
                                processingOrderNo = orderNo_sl + 1;
                                WriteToProcess("buttonArea", "SimulateDialog", "01");
                            }
                        }
                        else if (orderNo != 0)
                        {
                            Logger.Error(string.Format(scannerCode + "号扫码器，故障恢复处理失败，原因：PLC记录当前经过件烟流水号{0},上位机记录当前流水号应为{1},请人工确人！", orderNo, orderNo_sl + 1));
                            ShowMessageBox(string.Format(scannerCode + "号扫码器，故障恢复处理失败，原因：PLC记录当前经过件烟流水号{0},上位机记录当前流水号应为{1},请人工确人！", orderNo, orderNo_sl + 1), "询问", MessageBoxButtons.OK, MessageBoxIcon.Question);
                        }
                        return;
                    default:
                        return;
                }
            }
            catch (Exception e)
            {
                Logger.Error(string.Format("{0} 号条码扫描处理失败，原因：{1} ",this.scan ,e.Message));
            }
        }

        private void Scanner_Process_StockIn(string scannerCode, string barcode)
        {
            if (barcode.Length == 32)
            {
                barcode = barcode.Substring(2, 6);
            }

            if (barcode.Length != 6)
            {
                string text = scannerCode + "号条码扫描器处理失败！详情：条码格式不正确！";
                ShowMessageBox(text, "询问", MessageBoxButtons.OK, MessageBoxIcon.Question);
                Logger.Error(text);
                return;
            }

            using (PersistentManager pm = new PersistentManager())
            {
                StockInDao stockInDao = new StockInDao();
                StockInBatchDao stockInBatch = new StockInBatchDao();

                DataTable stockInTable = null;

                int pQuantity = scannerParameters.GetParameter(scannerCode, "Quantity") != null ? (int)scannerParameters.GetParameter(scannerCode, "Quantity") : 0;
                string pBarcode = scannerParameters.GetParameter(scannerCode, "Barcode") != null ? (string)scannerParameters.GetParameter(scannerCode, "Barcode") : "";

                stockInTable = stockInDao.FindCigarette();

                if (stockInTable.Rows.Count == 0)
                {
                    //提示当前已无入库申请任务。
                    string text = scannerCode + "号条码扫描器处理失败！详情：当前已无入库申请任务！";
                    ShowMessageBox(text, "询问", MessageBoxButtons.OK, MessageBoxIcon.Question);
                    Logger.Error(text);
                    return;
                }

                if (barcode != pBarcode && pBarcode != string.Empty)
                {
                    //是否可换品牌，进行入库
                    if (pQuantity == 5)
                    {
                        //可换品牌入库（未完成的品牌可，下次再重入库）
                        stockInTable = stockInDao.FindCigarette(barcode);
                    }
                    else
                    {
                        //不可换品牌入库。
                        //进行提示，并要求人工确认。进行搬回卷烟，按原计划重新上烟。
                        //或人工强行更换品牌（将未完成的入库品牌，强行结束，不再重新入库）。
                        stockInTable = stockInDao.FindCigarette(pBarcode);
                        if (stockInTable.Rows.Count != 0)
                        {
                            string text = string.Format("当前计划入库卷烟品牌为‘{0}’，是否强制更换其他品牌！是请点击‘Yes’否则请点击‘No’", stockInTable.Rows[0]["CIGARETTENAME"]);
                            if (DialogResult.Yes == MessageBox.Show(Application.OpenForms["MainForm"],text, "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            {
                                //强行更换品牌。                          
                                stockInBatch.UpdateState(stockInTable.Rows[0]["BATCHNO"].ToString());

                                stockInTable = stockInDao.FindCigarette(barcode);

                                scannerParameters.SetParameter(scannerCode, "Barcode", "");
                                scannerParameters.SetParameter(scannerCode, "Quantity", 5);
                            }
                            else
                            {
                                //提示进行搬回卷烟，按原计划重新上烟。并退出本次扫码流程。
                                text = scannerCode + "号条码扫描器处理失败！详情：请搬回卷烟，按原计划重新上烟！";
                                ShowMessageBox(text, "询问", MessageBoxButtons.OK, MessageBoxIcon.Question);
                                Logger.Error(text);
                                return;
                            }
                        }
                        else
                            stockInTable = stockInDao.FindCigarette(barcode);
                    }
                }
                else
                {
                    stockInTable = stockInDao.FindCigarette(barcode);
                }

                
                //判断当前扫到卷烟条码是否有入库申请任务，如果没有则提示：进行搬回卷烟，按原计划重新上烟。并退出本次扫码流程。
                if (stockInTable.Rows.Count == 0)
                {
                    //提示当前扫到条码卷烟，已无入库申请任务。
                    string text = scannerCode + "号条码扫描器处理失败！详情：当前卷烟已无入库申请任务！";
                    Logger.Error(text);
                    ShowMessageBox(text, "询问", MessageBoxButtons.OK, MessageBoxIcon.Question);                    

                    text = scannerCode + "号条码扫描器处理失败！详情：当前卷烟可能需要更新更条码，请确认！";
                    string cigaretteCode = "";
                    Scan(text, cigaretteCode, barcode);
                    return;
                }

                if (barcode == stockInTable.Rows[0]["BARCODE"].ToString())
                {
                    int [] data = new int [3];                

                    int supplyAddress = Convert.ToInt32(stockInTable.Rows[0]["CHANNELCODE"].ToString());                    
                    int change = Convert.ToInt32(stockInTable.Rows[0]["QUANTITY"]) <= 1 ? 1 : 0;
                    int orderNo = scannerParameters.GetParameter(scannerCode, "OrderNo") != null ? (int)scannerParameters.GetParameter(scannerCode, "OrderNo") : 0;

                    data[0] = supplyAddress;                    
                    data[1] = change;
                    data[2] = orderNo + 1;

                    if (WriteToService("StockPLC_01", "Scanner_DirectoryData_" + scannerCode, data))
                    {                       
                        scannerParameters.SetParameter(scannerCode, "SupplyAddress",supplyAddress);
                        scannerParameters.SetParameter(scannerCode, "Change", change);
                        scannerParameters.SetParameter(scannerCode, "OrderNo", orderNo + 1);

                        //更新数据库状态
                        pm.BeginTransaction();
                        //更新为已入库，并更新当前批次已入库数量。
                        stockInDao.UpdateScanStatus(stockInTable.Rows[0]["STOCKINID"].ToString());
                        stockInBatch.UpdateQuantityForBatch(stockInTable.Rows[0]["BATCHNO"].ToString());
                        if (Convert.ToInt32(stockInTable.Rows[0]["QUANTITY"]) <= 1)
                        {
                            stockInBatch.UpdateState(stockInTable.Rows[0]["BATCHNO"].ToString());                            
                        }
                        pm.Commit();
                        //更新扫码器参数信息
                        if (pBarcode == barcode)
                        {
                            if (pQuantity < 5)
                            {
                                scannerParameters.SetParameter(scannerCode, "Quantity", pQuantity + 1);
                            }
                            else
                            {
                                scannerParameters.SetParameter(scannerCode, "Quantity", 1);
                                WriteToProcess("DataRequestProcess", "StockInRequest", 1);
                            }
                        }
                        else
                        {
                            scannerParameters.SetParameter(scannerCode, "Barcode", barcode);
                            scannerParameters.SetParameter(scannerCode, "Quantity", 1);
                            WriteToProcess("DataRequestProcess", "StockInRequest", 1);
                        }

                        if (Convert.ToInt32(stockInTable.Rows[0]["QUANTITY"]) <= 1)
                        {
                            scannerParameters.SetParameter(scannerCode, "Barcode", "");
                            scannerParameters.SetParameter(scannerCode, "Quantity", 5);
                            WriteToProcess("DataRequestProcess", "StockInRequest", 1);
                        }

                        WriteToProcess("LEDProcess", "Refresh_01", null);
                        Logger.Info(string.Format(scannerCode + "号条码扫描，写分流数据，卷烟名称:{0}，目标:{1} ！", stockInTable.Rows[0]["CIGARETTENAME"], Convert.ToInt32(stockInTable.Rows[0]["CHANNELCODE"].ToString())));
                    }
                    return;
                }
            }
        }

        private void Scan(string text, string cigaretteCode, string barcode)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("text", text);
            parameters.Add("cigaretteCode", cigaretteCode);
            parameters.Add("barcode", barcode);
            WriteToProcess("buttonArea", "ScanDialog", parameters);
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
