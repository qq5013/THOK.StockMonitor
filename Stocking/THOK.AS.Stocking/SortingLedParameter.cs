using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using THOK.ParamUtil;

namespace THOK.AS.Stocking
{
    public class SortingLedParameter : BaseObject
    {
        private int port;

        [CategoryAttribute("备货系统通信参数"), DescriptionAttribute("备货系统监听端口"), Chinese("监听端口")]
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        private string ip;

        [CategoryAttribute("备货系统通信参数"), DescriptionAttribute("备货系统IP地址"), Chinese("IP地址")]
        public string IP
        {
            get { return ip; }
            set { ip = value; }
        }

        private string maxLEDFormTitle;

        [CategoryAttribute("窗体显示参数"), DescriptionAttribute("窗体标题"), Chinese("标题")]
        public string MaxLEDFormTitle
        {
            get { return maxLEDFormTitle; }
            set { maxLEDFormTitle = value; }
        }

        private string maxLEDFormTop;

        [CategoryAttribute("窗体显示参数"), DescriptionAttribute("Top"), Chinese("Top")]
        public string MaxLEDFormTop
        {
            get { return maxLEDFormTop; }
            set { maxLEDFormTop = value; }
        }

        private string maxLEDFormLeft;

        [CategoryAttribute("窗体显示参数"), DescriptionAttribute("Left"), Chinese("Left")]
        public string MaxLEDFormLeft
        {
            get { return maxLEDFormLeft; }
            set { maxLEDFormLeft = value; }
        }

        private string maxLEDFormWidth;

        [CategoryAttribute("窗体显示参数"), DescriptionAttribute("Width"), Chinese("Width")]
        public string MaxLEDFormWidth
        {
            get { return maxLEDFormWidth; }
            set { maxLEDFormWidth = value; }
        }

        private string maxLEDFormHeight;

        [CategoryAttribute("窗体显示参数"), DescriptionAttribute("Height"), Chinese("Height")]
        public string MaxLEDFormHeight
        {
            get { return maxLEDFormHeight; }
            set { maxLEDFormHeight = value; }
        }


    }
}
