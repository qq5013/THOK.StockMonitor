using System;
using System.Collections.Generic;
using System.Text;
using THOK.MCP;
using THOK.Util;
using System.Data;
using THOK.AS.Stocking.Util;

namespace THOK.AS.Stocking.StateManage
{
    class OrderDataStateManage: BaseDao
    {
        private string stateItemCode = "";
        private string dataView = "";
        private int index = 0;
        private string plcServicesName = "";
        private string orderItemName = "";
        private string checkItemName = "";

        public OrderDataStateManage(string stateItemCode)
        {
            this.stateItemCode = stateItemCode;
            GetParameters();
        }

        public void GetParameters()
        {
            string sql = "SELECT * FROM AS_STATEMANAGER_ORDER WHERE STATECODE = '{0}'";
            sql = string.Format(sql, stateItemCode);
            DataTable table = ExecuteQuery(sql).Tables[0];

            this.dataView = table.Rows[0]["VIEWNAME"].ToString();
            this.index =Convert.ToInt32(table.Rows[0]["INDEXNO"].ToString());
            this.plcServicesName = table.Rows[0]["PLCSERVICESNAME"].ToString();
            this.orderItemName = table.Rows[0]["ORDERITEMNAME"].ToString();
            this.checkItemName = table.Rows[0]["CHECKITEMNAME"].ToString();
        }
      
        public bool Check(int index)
        {
            if (this.index + 1 != index)
            {
                string str = "订单流水号校正错误：上位机流水号为：[{0}]，PLC流水号为：[{1}]，请人工确认。";
                Logger.Error(string.Format(str, this.index+1, index));
                return false;
            }
            else
            {
                string str = string.Format("订单写入成功，流水号：[{0}]",index);
                Logger.Info(str);
                return true;
            }
        }

        public bool MoveNext()
        {
            bool result = false;

            index++;
            string sql = "UPDATE AS_STATEMANAGER_ORDER SET INDEXNO = {0} WHERE STATECODE = '{1}'";
            sql = string.Format(sql, index, "01");
            ExecuteNonQuery(sql);

            result = true;
            return result;
        }

        public bool MoveTo(int index,IProcessDispatcher dispatcher)
        {
            bool result = false;

            this.index = index - 1;
            string sql = "UPDATE AS_STATEMANAGER_ORDER SET INDEXNO = {0} WHERE STATECODE = '01'";
            sql = string.Format(sql, index - 1, 01);
            ExecuteNonQuery(sql);            
    
            //写校正完成标志到PLC
            dispatcher.WriteToService(plcServicesName,checkItemName,1);
            Logger.Info("订单校正完成");

            result = true;
            return result;
        }

        public bool WriteToPlc(IProcessDispatcher dispatcher)
        {
            bool result = false;
            //给PLC写订单数据 
            //TODO 订单数据加工处理
            if (dispatcher.WriteToService(plcServicesName,orderItemName, 2))
            {
                result = true;
            }
            return result;
        }
    }
}
