using System;
using System.Collections.Generic;
using System.Text;
using THOK.MCP;
using THOK.Util;
using System.Data;
using THOK.AS.Stocking.Util;

namespace THOK.AS.Stocking.StateManageProcess.Dao
{
    class OrderDataStateManage: BaseDao
    {
        private IProcessDispatcher dispatcher;

        private string stateItemCode = "";
        private string dataView = "";
        private int index = 0;
        private string plcServicesName = "";
        private string orderItemName = "";
        private string checkItemName = "";
        private string orderQuantity = "";

        public OrderDataStateManage()
        {
        }

        internal IList<string> GetStateItemCodeList()
        {
            IList<string> stateItemCodeList = new List<string>();

            string sql = "SELECT * FROM AS_STATEMANAGER_ORDER";
            sql = string.Format(sql, stateItemCode);
            DataTable table = ExecuteQuery(sql).Tables[0];

            foreach (DataRow  row in table.Rows)
            {
                stateItemCodeList.Add(row["STATECODE"].ToString());
            }

            return stateItemCodeList;
        }

        public OrderDataStateManage(string stateItemCode, IProcessDispatcher dispatcher)
        {
            this.stateItemCode = stateItemCode;
            this.dispatcher = dispatcher;
            GetParameters();
        }

        public void GetParameters()
        {
            string sql = "SELECT * FROM AS_STATEMANAGER_ORDER WHERE STATECODE = '{0}'";
            sql = string.Format(sql, stateItemCode);
            DataTable table = ExecuteQuery(sql).Tables[0];

            this.dataView = table.Rows[0]["VIEWNAME"].ToString();
            this.index =Convert.ToInt32(table.Rows[0]["ROW_INDEX"].ToString());
            this.plcServicesName = table.Rows[0]["PLCSERVICESNAME"].ToString();
            this.orderItemName = table.Rows[0]["ORDERITEMNAME"].ToString();
            this.checkItemName = table.Rows[0]["CHECKITEMNAME"].ToString();
            this.orderQuantity=table.Rows[0]["ORDERQUANTITY"].ToString();
        }
      
        public bool Check(int index)
        {
            if (this.index + 1 != index)
            {
                string str = "{0} 号订单请求，订单流水号校验错误：上位机流水号为：[{1}]，PLC流水号为：[{2}]，请人工确认。";
                Logger.Error(string.Format(str,stateItemCode, this.index + 1, index));
                return false;
            }
            else
                return true;
        }

        public bool MoveTo(int index)
        {
            bool result = false;

            this.index = index - 1;
            string sql = "UPDATE AS_STATEMANAGER_ORDER SET ROW_INDEX = {0} WHERE STATECODE = '{1}'";
            sql = string.Format(sql, this.index, stateItemCode);
            ExecuteNonQuery(sql);            
    
            //写校正完成标志到PLC
            dispatcher.WriteToService(plcServicesName,checkItemName,1);
            Logger.Info(string.Format("{0} 号订单请求，订单校正完成,流水号：{1}",stateItemCode,index));

            result = true;
            return result;
        }

        public bool WriteToPlc()
        {
            bool result = false;
            string channelCodes = "";
            int quantity =Convert.ToInt32(this.orderQuantity);
            
            //给PLC写订单数据 
            Stack<int> data = new Stack<int>();

            string sql = "SELECT TOP {0} * FROM {1} WHERE ROW_INDEX > {2}";
            sql = string.Format(sql,quantity,dataView, this.index);
            DataTable table = ExecuteQuery(sql).Tables[0];

            foreach (DataRow  row in table.Rows)
            {
                data.Push(Convert.ToInt32(row["CHANNELCODE"]));
                channelCodes = channelCodes + "|" + (Convert.ToInt32(row["CHANNELCODE"])).ToString();
                this.index++;
            }

            while (data.Count < quantity)
            {
                data.Push(0);
            }

            data.Push(this.index);//最后一件流水号
            data.Push(table.Rows.Count);//总件数
            data.Push(1);//完成标志

            int[] dataItems = data.ToArray();
            Array.Reverse(dataItems);

            if (dispatcher.WriteToService(plcServicesName, orderItemName,dataItems))
            {
                sql = "UPDATE AS_STATEMANAGER_ORDER SET ROW_INDEX = {0} WHERE STATECODE = '{1}'";
                sql = string.Format(sql, this.index, stateItemCode);
                ExecuteNonQuery(sql);
                result = true;
            }
            if (result && table.Rows.Count > 0)
            {
                Logger.Info(string.Format("{0} 号订单请求，订单数据写入成功!流水号：[{1}]；总件数：[{2}]；数据：[{3}|]", stateItemCode,index,table.Rows.Count,channelCodes));
            }
            return result;
        }
    }
}
