using System;
using System.Collections.Generic;
using System.Text;
using THOK.MCP;
using THOK.Util;
using System.Data;
using THOK.AS.Stocking.Util;

namespace THOK.AS.Stocking.StateManage
{
    class LedStateManage : BaseDao
    {
        private LEDUtil ledUtil = new LEDUtil();
        internal class LedItem
        {
            public string Name;
            public int Count = 0;
            public override string ToString()
            {
                return string.Format("{0}-{1}", Count > 0 ? Count.ToString().PadLeft(2, ' ') : "", Name);
            }
        }

        private string stateItemCode = "";
        private string ledCode = "";
        private string dataView = "";
        private int index = 0;
        private string plcServicesName = "";
        private string plcItemName = "";

            
        public LedStateManage(string stateItemCode)
        {
            this.stateItemCode = stateItemCode;
            GetParameters();
            ShowData();
        }

        public void GetParameters()
        {
            string sql = "SELECT * FROM AS_STATEMANAGER_LED WHERE STATECODE = '{0}'";
            sql = string.Format(sql, stateItemCode);
            DataTable table = ExecuteQuery(sql).Tables[0];

            this.ledCode = table.Rows[0]["LEDCODE"].ToString();
            this.dataView = table.Rows[0]["VIEWNAME"].ToString();
            this.index = Convert.ToInt32(table.Rows[0]["INDEXNO"].ToString());
            this.plcServicesName = table.Rows[0]["PLCSERVICESNAME"].ToString();
            this.plcItemName = table.Rows[0]["PLCITEMNAME"].ToString();
        }

        public bool Check(int index)
        {
            if (this.index + 1 != index && this.index != index )
            {
                string strErr = "{0}LED流水号检正错误：上位机当前流水号为{1},PLC当前流水号为{2}; ";
                Logger.Error(string.Format(strErr, ledCode, this.index + 1, index));

                Stack<LedItem> data = new Stack<LedItem>();

                LedItem item = new LedItem();
                item.Name = string.Format("{0}号LED流水号检正错误：", ledCode);
                data.Push(item);

                item = new LedItem();
                item.Name = string.Format("上位机当前流水号为{0},", this.index + 1);
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
            string sql = "UPDATE AS_STATEMANAGER_LED SET INDEXNO = {0} WHERE STATECODE = '{1}'";
            sql = string.Format(sql, index - 1, stateItemCode);
            ExecuteNonQuery(sql);
            ShowData();

            result = true;
            return result;
        }

        public bool MoveNext()
        {
            bool result = false;

            index++;
            string sql = "UPDATE AS_STATEMANAGER_LED SET INDEXNO = {0} WHERE STATECODE = '{1}'";
            sql = string.Format(sql,index, stateItemCode);
            ExecuteNonQuery(sql);
            ShowData();

            result = true;
            return result;
        }

        public void ShowData()
        {
            string sql = "SELECT * FROM {0} WHERE INDEX > {1}";
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
            ledUtil.Show(ledCode,ledItems);
        }   

        public bool WriteToPlc(IProcessDispatcher dispatcher)
        {
            bool result = false;
            if (dispatcher.WriteToService(plcServicesName,plcItemName, this.index))
            {
                result = true;
            }
            return result;
        }
    }
}
