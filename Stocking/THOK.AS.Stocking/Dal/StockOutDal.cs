using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.Util;
using THOK.AS.Stocking.Dao;

namespace THOK.AS.Stocking.Dal
{
    public class StockOutDal
    {
        public DataTable GetAll()
        {
            using (PersistentManager pm = new PersistentManager())
            {
                StockOutDao outDao = new StockOutDao();
                return outDao.FindAll();
            }
        }
    }
}
