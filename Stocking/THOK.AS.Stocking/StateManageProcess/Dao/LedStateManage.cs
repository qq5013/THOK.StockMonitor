using System;
using System.Collections.Generic;
using System.Text;
using THOK.MCP;
using THOK.Util;
using System.Data;
using THOK.AS.Stocking.Util.LED2008;

namespace THOK.AS.Stocking.StateManageProcess.Dao
{
    class LedStateManage : BaseDao
    {
        private IProcessDispatcher dispatcher;

        private string stateItemCode = "";         
        private string dataView = "";
        private int index = 0;
        private string plcServicesName = "";
        private string releaseItemName = "";
        private string ledCode = "";

        public LedStateManage()
        {
        }

        internal IList<string> GetStateItemCodeList()
        {
            IList<string> stateItemCodeList = new List<string>();

            string sql = "SELECT * FROM AS_STATEMANAGER_LED";
            sql = string.Format(sql, stateItemCode);
            DataTable table = ExecuteQuery(sql).Tables[0];

            foreach (DataRow  row in table.Rows)
            {
                stateItemCodeList.Add(row["STATECODE"].ToString());
            }

            return stateItemCodeList;
        }

        public LedStateManage(string stateItemCode, IProcessDispatcher dispatcher)
        {
            this.stateItemCode = stateItemCode;
            this.dispatcher = dispatcher;
            GetParameters();
            ShowData();
        }

        public void GetParameters()
        {
            string sql = "SELECT * FROM AS_STATEMANAGER_LED WHERE STATECODE = '{0}'";
            sql = string.Format(sql, stateItemCode);
            DataTable table = ExecuteQuery(sql).Tables[0];
            
            this.dataView = table.Rows[0]["VIEWNAME"].ToString();
            this.index = Convert.ToInt32(table.Rows[0]["ROW_INDEX"].ToString());
            this.plcServicesName = table.Rows[0]["PLCSERVICESNAME"].ToString();
            this.releaseItemName = table.Rows[0]["RELEASEITEMNAME"].ToString();
            this.ledCode = table.Rows[0]["LEDCODE"].ToString();
        }

        public bool Check(int index)
        {
            if (this.index + 1 != index && this.index != index )
            {
                string strErr = "{0}号LED流水号校验错误：上位机当前流水号为{1},PLC当前流水号为{2}; ";
                Logger.Error(string.Format(strErr, stateItemCode,this.index + 1, index));

                Stack<LedItem> data = new Stack<LedItem>();

                LedItem item = new LedItem();
                item.Name = string.Format("{0}号LED流水号校验错误：", stateItemCode);
                data.Push(item);

                item = new LedItem();
                item.Name = string.Format("PC当前流水号为{0},", this.index + 1);
                data.Push(item);

                item = new LedItem();
                item.Name = string.Format("PLC当前流水号为{0};",index);
                data.Push(item);

                LedItem[] ledItems = data.ToArray();
                Array.Reverse(ledItems);

                Show(ledItems);
                return false;
            }
            else 
                return true;
        }

        public bool MoveTo(int index)
        {
            bool result = false;

            this.index = index - 1;
            string sql = "UPDATE AS_STATEMANAGER_LED SET ROW_INDEX = {0} WHERE STATECODE = '{1}'";
            sql = string.Format(sql,this.index, stateItemCode);
            ExecuteNonQuery(sql);
            result = true;
            return result;
        }

        public bool MoveNext()
        {
            bool result = false;

            this.index++;
            string sql = "UPDATE AS_STATEMANAGER_LED SET ROW_INDEX = {0} WHERE STATECODE = '{1}'";
            sql = string.Format(sql,this.index, stateItemCode);
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
            sql = string.Format(sql, dataView,index);
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

        public bool WriteToPlc()
        {
            bool result = false;
            if (dispatcher.WriteToService(plcServicesName,releaseItemName, this.index))
            {
                result = true;
            }
            return result;
        }
    }
}
