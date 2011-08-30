using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
using THOK.AS.Stocking.Dao;

namespace THOK.AS.Stocking.Dal
{
    public class StockBatchDal
    {
        public DataTable GetAll()
        {
            using (PersistentManager pm = new PersistentManager())
            {
                StockOutBatchDao batchDao = new StockOutBatchDao();
                return batchDao.FindAll();
            }
        }
    }
}
