using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using THOK.MCP.Config;
using THOK.Util;
using THOK.ParamUtil;

namespace THOK.AS.Stocking.View
{
    public partial class SortingLedParameterForm : THOK.AF.View.ToolbarForm
    {
        private SortingLedParameter parameter = new SortingLedParameter();
        private THOK.MCP.Service.UDP.Config.Configuration udpConfig = new THOK.MCP.Service.UDP.Config.Configuration("UDP.xml");

        private Dictionary<string, string> attributes = null;

        public SortingLedParameterForm()
        {
            InitializeComponent();
            ReadParameter();
        }

        private void ReadParameter()
        {
            //读取Context配置文件参数
            ConfigUtil configUtil = new ConfigUtil();
            attributes = configUtil.GetAttribute();
            parameter.MaxLEDFormTitle = attributes["MaxLEDFormTitle"];
            parameter.MaxLEDFormTop = attributes["MaxLEDFormTop"];
            parameter.MaxLEDFormLeft = attributes["MaxLEDFormLeft"];
            parameter.MaxLEDFormWidth = attributes["MaxLEDFormWidth"];
            parameter.MaxLEDFormHeight = attributes["MaxLEDFormHeight"];

            //读取UDP配置文件参数
            parameter.IP = udpConfig.IP;
            parameter.Port = udpConfig.Port;        

            propertyGrid.SelectedObject = parameter;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //保存Context参数
                attributes["MaxLEDFormTitle"] = parameter.MaxLEDFormTitle;
                attributes["MaxLEDFormTop"] = parameter.MaxLEDFormTop;
                attributes["MaxLEDFormLeft"] = parameter.MaxLEDFormLeft;
                attributes["MaxLEDFormWidth"] = parameter.MaxLEDFormWidth;
                attributes["MaxLEDFormHeight"] = parameter.MaxLEDFormHeight;

                ConfigUtil configUtil = new ConfigUtil();
                configUtil.Save(attributes);

                //保存UDP参数
                udpConfig.IP = parameter.IP;
                udpConfig.Port = parameter.Port;
                udpConfig.Save();

                MessageBox.Show("系统参数保存成功，请重新启动本系统。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exp)
            {
                MessageBox.Show("保存系统参数过程中出现异常，原因：" + exp.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Exit();
        }
    }
}

