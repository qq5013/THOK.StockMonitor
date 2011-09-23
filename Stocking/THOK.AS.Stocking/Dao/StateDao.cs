using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;

namespace THOK.AS.Stocking.Dao
{
    public class StateDao:BaseDao
    {
        /// <summary>
        /// 查询所有扫码状态管理器
        /// </summary>
        /// <returns></returns>
        public DataTable FindStateQueryTypeTable()
        {
            string sql = "SELECT STATECODE,STATECODE + '|' + REMARK AS STATENAME FROM AS_STATEMANAGER_SCANNER";
            return ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 根据状态管理器编号查找相应的INDEXNO
        /// </summary>
        /// <param name="stateCode">状态管理器编号</param>
        /// <returns></returns>
        public DataTable FindScannerIndexNoByStateCode(string stateCode)
        {
            string sql = string.Format("SELECT INDEXNO FROM dbo.AS_STATEMANAGER_SCANNER WHERE STATECODE='{0}'",stateCode);
            return ExecuteQuery(sql).Tables[0];
        }

        /// <summary>
        /// 根据INDEXNO查询订单的信息
        /// </summary>
        /// <param name="indexNo">流水号</param>
        /// <returns></returns>
        public DataTable FindOrderStateByIndexNo(string indexNo)
        {
            string sql = string.Format(@"SELECT ROW_INDEX,LINECODE,CIGARETTECODE,CIGARETTENAME,CHANNELCODE,
                            CASE CHANNELTYPE 
                                WHEN '3' THEN '通道机'
                                WHEN '2' THEN '立式机'
                                END CHANNELTYPENAME,
                            CASE 
                                WHEN ROW_INDEX < {0} THEN '已扫描'
                                WHEN ROW_INDEX = {0} THEN '已扫描'
                                WHEN ROW_INDEX > {0} THEN '未扫描'
                                END SCANNERSTATE
                            FROM V_STATE_SCANNER02 ",indexNo);
            return ExecuteQuery(sql).Tables[0];
        }
    }
}
