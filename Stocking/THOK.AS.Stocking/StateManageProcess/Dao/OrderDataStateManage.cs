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
            this.index =Convert.ToInt32(table.Rows[0]["INDEXNO"].ToString());
            this.plcServicesName = table.Rows[0]["PLCSERVICESNAME"].ToString();
            this.orderItemName = table.Rows[0]["ORDERITEMNAME"].ToString();
            this.checkItemName = table.Rows[0]["CHECKITEMNAME"].ToString();
        }
      
        public bool Check(int index)
        {
            if (this.index + 1 != index)
            {
                string str = "������ˮ��У��������λ����ˮ��Ϊ��[{0}]��PLC��ˮ��Ϊ��[{1}]�����˹�ȷ�ϡ�";
                Logger.Error(string.Format(str, this.index+1, index));
                return false;
            }
            else
                return true;
        }

        public bool MoveTo(int index)
        {
            bool result = false;

            this.index = index - 1;
            string sql = "UPDATE AS_STATEMANAGER_ORDER SET INDEXNO = {0} WHERE STATECODE = '{1}'";
            sql = string.Format(sql, this.index, stateItemCode);
            ExecuteNonQuery(sql);            
    
            //дУ����ɱ�־��PLC
            dispatcher.WriteToService(plcServicesName,checkItemName,1);
            Logger.Info("����У�����");

            result = true;
            return result;
        }

        public bool WriteToPlc()
        {
            bool result = false;
            int quantity = 50;
            
            //��PLCд�������� 
            Stack<int> data = new Stack<int>();

            string sql = "SELECT TOP {0} * FROM {1} WHERE INDEX > {2}";
            sql = string.Format(sql,quantity,dataView, this.index);
            DataTable table = ExecuteQuery(sql).Tables[0];

            foreach (DataRow  row in table.Rows)
            {
                data.Push(Convert.ToInt32(row["CHANNELCODE"]));
                this.index++;
            }

            while (data.Count < quantity)
            {
                data.Push(0);
            }

            data.Push(this.index);//���һ����ˮ��
            data.Push(table.Rows.Count);//�ܼ���
            data.Push(1);//��ɱ�־

            int[] dataItems = data.ToArray();
            Array.Reverse(dataItems);

            if (dispatcher.WriteToService(plcServicesName, orderItemName,dataItems))
            {
                sql = "UPDATE AS_STATEMANAGER_ORDER SET INDEXNO = {0} WHERE STATECODE = '{1}'";
                sql = string.Format(sql, this.index, stateItemCode);
                ExecuteNonQuery(sql);
                result = true;
            }
            return result;
        }
    }
}