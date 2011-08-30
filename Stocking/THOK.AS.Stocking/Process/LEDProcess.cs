using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using THOK.MCP;
using THOK.Util;
using THOK.AS.Stocking.Dao;
using THOK.AS.Stocking.Util;

namespace THOK.AS.Stocking.Process
{
    public class LEDProcess: AbstractProcess
    {
        private LEDUtil ledUtil = new LEDUtil();
        private Dictionary<int, string> isActiveLeds = new Dictionary<int, string>();

        public override void Release()
        {
            try
            {
                ledUtil.Release();
                base.Release();
            }
            catch (Exception e)
            {
                Logger.Error("LEDProcess 资源释放失败，原因：" + e.Message);
            }
        }

        public override void Initialize(Context context)
        {
            base.Initialize(context);

            Microsoft.VisualBasic.Devices.Network network = new Microsoft.VisualBasic.Devices.Network();
            string[] ledConfig = context.Attributes["IsActiveLeds"].ToString().Split(';');

            foreach (string led in ledConfig)
            {
                if (network.Ping(led.Split(',')[1]))
                {
                    isActiveLeds.Add(Convert.ToInt32(led.Split(',')[0]), led.Split(',')[1]);
                }
                else
                {
                    Logger.Error(Convert.ToInt32(led.Split(',')[0]) + "号LED屏故障，请检查！IP:[" + led.Split(',')[1] + "]");
                }
            }

            ledUtil.isActiveLeds = isActiveLeds;
        }

        protected override void StateChanged(StateItem stateItem, IProcessDispatcher dispatcher)
        {
            /*  处理事项：
             *  Init：初始化
             *  Refresh：刷新LED屏。
             *      ‘01’：一号屏 显示请求入库托盘信息
             *      ‘02’：二号屏 显示请求补货的混合烟道补货顺序信息
             */
            string cigaretteName = "";

            switch (stateItem.ItemName)
            {
                case "Refresh":
                    this.Refresh();
                    break;
                case "Refresh_01":
                    this.Refresh_01();
                    break;
                case "Refresh_02":
                    this.Refresh_02();
                    break;
                case "Refresh_02_MoveNext":
                    this.Refresh_02_MoveNext();
                    break;
                case "StockInRequestShow":
                    cigaretteName = Convert.ToString(stateItem.State);
                    this.StockInRequestShow(cigaretteName);
                    break;
                case "Show_Scanner_01":
                    break;
                case "Show_Scanner_02":
                    cigaretteName = Convert.ToString(stateItem.State);
                    this.Show("1","02", cigaretteName);
                    break;
                case "Show_Scanner_03":
                    cigaretteName = Convert.ToString(stateItem.State);
                    this.Show("2", "03",cigaretteName);
                    break;
                case "Show_Scanner_04":
                    cigaretteName = Convert.ToString(stateItem.State);
                    this.Show("3", "04",cigaretteName);
                    break;
                case "Show_Scanner_05":
                    cigaretteName = Convert.ToString(stateItem.State);
                    this.Show("3","05",cigaretteName);
                    break;
                default:
                    break;
            }        
        }

        private void Refresh()
        {
            WriteToService("StockPLC_02", "RouteChannageTag", "京天安门们8");
            Logger.Info("写入PLC成功");
            //刷新1号屏
            Refresh_01();
            //刷新2号屏
            Refresh_02();
        }

        private void Refresh_01()
        {
            //刷新1号屏
            using (PersistentManager pm = new PersistentManager())
            {
                StockInBatchDao stockInBatchDao = new StockInBatchDao();

                DataTable batchTable = stockInBatchDao.FindStockInTopAnyBatch();
                ledUtil.RefreshStockInLED(batchTable, "1");
            }
        }

        private void Refresh_02()
        {
            //刷新2号屏
            using (PersistentManager pm = new PersistentManager())
            {
                StockOutDao outDao = new StockOutDao();
                ChannelDao channelDao = new ChannelDao();

                string channelCode = Context.Attributes["LED_02_CHANNELCODE"].ToString();
                DataTable table = outDao.FindLEDData(channelCode);
                string ledNo = channelDao.FindLed(channelCode);
                ledUtil.Refresh(table, ledNo);
            }
        }

        private void Refresh_02_MoveNext()
        {
            //更新已出一件烟状态，刷新2号屏
            using (PersistentManager pm = new PersistentManager())
            {
                StockOutDao outDao = new StockOutDao();
                ChannelDao channelDao = new ChannelDao();

                string channelCode = Context.Attributes["LED_02_CHANNELCODE"].ToString();

                string stockOutID = outDao.FindMinStockOutID(channelCode);
                outDao.UpdateLEDStatus(stockOutID);

                DataTable table = outDao.FindLEDData(channelCode);
                string ledNo = channelDao.FindLed(channelCode);

                ledUtil.Refresh(table, ledNo);
            }
        }

        private void StockInRequestShow(string cigaretteName)
        {
            ledUtil.RefreshStockInLED("1",cigaretteName);
            Logger.Info("缺烟提醒：请入库" + cigaretteName);
        }

        private void Show(string ledNo,string sccanerCode,string cigaretteName)
        {
            ledUtil.RefreshScannerLED(ledNo, sccanerCode, cigaretteName);
            Logger.Info(ledNo + "号屏显示：" + sccanerCode + "号扫码器当前卷烟为：" + cigaretteName);
        }
    }
}
 