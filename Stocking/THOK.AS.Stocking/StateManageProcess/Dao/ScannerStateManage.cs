using System;
using System.Collections.Generic;
using System.Text;

using THOK.MCP;
using THOK.Util;
using System.Data;
using THOK.AS.Stocking.Util.LED2008;

namespace THOK.AS.Stocking.StateManageProcess.Dao
{
    class ScannerStateManage:BaseDao
    {
        private IProcessDispatcher dispatcher;

        private string stateItemCode = "";
        private string dataView = "";
        private int index = 0;
        private string plcServicesName = "";
        private string releaseItemName = "";
        private string ledCode = "";

        public ScannerStateManage()
        {
        }

        internal IList<string> GetStateItemCodeList()
        {
            IList<string> stateItemCodeList = new List<string>();

            string sql = "SELECT * FROM AS_STATEMANAGER_SCANNER";
            sql = string.Format(sql, stateItemCode);
            DataTable table = ExecuteQuery(sql).Tables[0];

            foreach (DataRow  row in table.Rows)
            {
                stateItemCodeList.Add(row["STATECODE"].ToString());
            }

            return stateItemCodeList;
        }

        public ScannerStateManage(string stateItemCode, IProcessDispatcher dispatcher)
        {
            this.stateItemCode = stateItemCode;
            this.dispatcher = dispatcher;
            GetParameters();
            ShowData();
        }

        public void GetParameters()
        {
            string sql = "SELECT * FROM AS_STATEMANAGER_SCANNER WHERE STATECODE = '{0}'";
            sql = string.Format(sql, stateItemCode);
            DataTable table = ExecuteQuery(sql).Tables[0];

            this.dataView = table.Rows[0]["VIEWNAME"].ToString();
            this.index = Convert.ToInt32(table.Rows[0]["ROW_INDEX"].ToString());
            this.plcServicesName = table.Rows[0]["PLCSERVICESNAME"].ToString();
            this.releaseItemName = table.Rows[0]["RELEASEITEMNAME"].ToString();
            this.ledCode = table.Rows[0]["LEDCODE"].ToString();
        }

        internal bool Check(string barcode)
        {
            if (barcode.Length==32 || barcode.Length==6)
            {
                string sql = "SELECT * FROM {0} WHERE ROW_INDEX = {1}";
                sql = string.Format(sql, dataView, this.index + 1);
                DataTable table = ExecuteQuery(sql).Tables[0];
                if (barcode.Length==32)
                {
                    barcode = barcode.Substring(2, 6);
                }
                if (table.Rows[0]["BARCODE"].ToString() == barcode)
                    return true;
                else
                    return false;  
            }
            else
            {
                return false;
            }
        }

        public bool Check(int index)
        {
            if (this.index + 1 != index && this.index != index)
            {
                string strErr = "{0}号扫码器流水号检正错误：上位机流水号为：[{1}]，PLC流水号为：[{2}]，请人工确认。 ";
                Logger.Error(string.Format(strErr, stateItemCode, this.index + 1, index));

                Stack<LedItem> data = new Stack<LedItem>();

                LedItem item = new LedItem();
                item.Name = string.Format("{0}扫码器流水号错误", stateItemCode);
                data.Push(item);

                item = new LedItem();
                item.Name = string.Format("PC当前流水号为：{0}", this.index + 1);
                data.Push(item);

                item = new LedItem();
                item.Name = string.Format("PLC当前流水号为：{0}", index);
                data.Push(item);

                LedItem[] ledItems = data.ToArray();
                Array.Reverse(ledItems);

                Show(ledItems);
                return false;
            }
            else
                return true;
        }

        public bool MoveNext()
        {
            bool result = false;

            this.index++;
            string sql = "UPDATE AS_STATEMANAGER_SCANNER SET ROW_INDEX = {0} WHERE STATECODE = '{1}'";
            sql = string.Format(sql, this.index, stateItemCode);
            ExecuteNonQuery(sql);

            result = true;
            return result;
        }

        public bool MoveTo(int index)
        {
            bool result = false;

            this.index = index - 1;
            string sql = "UPDATE AS_STATEMANAGER_SCANNER SET ROW_INDEX = {0} WHERE STATECODE = '{1}'";
            sql = string.Format(sql, this.index, stateItemCode);
            ExecuteNonQuery(sql);
            result = true;
            return result;
        }

        public void ShowData()
        {
            ShowData(this.index);
        }

        public void ShowData(int index)
        {
            string sql = "SELECT * FROM {0} WHERE ROW_INDEX > {1}";
            sql = string.Format(sql, dataView, index);
            DataTable table = ExecuteQuery(sql).Tables[0];
            LedItem[] ledItems = TableToLedItemArray(table);
            Show(ledItems);
        }

        public LedItem[] TableToLedItemArray(DataTable table)
        {
            Stack<LedItem> data = new Stack<LedItem>();
            foreach (DataRow row in table.Rows)
            {
                if (data.Count != 0)
                {
                    LedItem item = data.Pop();
                    if (item.Name == row["CIGARETTENAME"].ToString())
                    {
                        item.Count++;
                        data.Push(item);
                    }
                    else
                    {
                        data.Push(item);//添加原来

                        if (data.Count >= 10)
                        {
                            break;
                        }

                        //添加新的
                        item = new LedItem();
                        item.Name = row["CIGARETTENAME"].ToString();
                        item.Count = 1;
                        data.Push(item);
                    }
                }
                else
                {
                    LedItem item = new LedItem();
                    item.Name = row["CIGARETTENAME"].ToString();
                    item.Count = 1;
                    data.Push(item);
                }
            }

            LedItem[] ledItems = data.ToArray();
            Array.Reverse(ledItems);
            return ledItems;
        }

        public void Show(LedItem[] ledItems)
        {
            dispatcher.WriteToProcess("LEDProcess", ledCode, ledItems);
        }

        public bool WriteToPlc(Context context)
        {
            bool result = false;
            
            int[] data = new int[2];

            string sql = "SELECT * FROM {0} WHERE ROW_INDEX  = {1}";
            sql = string.Format(sql, dataView, this.index);
            DataTable table = ExecuteQuery(sql).Tables[0];

            if (table.Rows.Count > 0)
            {
                int lineCode = Convert.ToInt32(table.Rows[0]["LINECODE"]);

                switch (context.Attributes["SupplyToSortLine"].ToString())
                {
                    case "01":
                        lineCode = 1;
                        break;
                    case "02":
                        lineCode = 2;
                        break;
                    default:
                        break;
                }

                int supplyAddress = Convert.ToInt32(lineCode.ToString() + table.Rows[0]["SUPPLYADDRESS"].ToString().PadLeft(2, "0"[0]));

                data[0] = supplyAddress;
                data[1] = this.index;

                if (dispatcher.WriteToService(plcServicesName, releaseItemName, data))
                {
                    result = true;
                    Logger.Info(string.Format("{0}号扫码器，写分流数据成功，目标：'{1}'流水号：'{2}'", stateItemCode, data[0],this.index));
                }
            }
            else
            {
                Logger.Info(string.Format("{0}号扫码器，无任务数据！", stateItemCode));

                Stack<LedItem> LedItemData = new Stack<LedItem>();

                LedItem item = new LedItem();
                item.Name = string.Format("{0}号扫码器，无任务数据！", stateItemCode);
                LedItemData.Push(item);

                LedItem[] ledItems = LedItemData.ToArray();
                Array.Reverse(ledItems);

                Show(ledItems);

                result = false;
            }

            return result;
        }
    }
}
