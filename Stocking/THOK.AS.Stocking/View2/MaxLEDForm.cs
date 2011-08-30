using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing; 
using System.Text;
using System.Windows.Forms;

namespace THOK.AS.Stocking.View
{
    public partial class MaxLEDForm : Form
    {
        public MaxLEDForm()
        {
            InitializeComponent();
        }

        public void Initialize(THOK.MCP.Collection.AttributeCollection parameters)
        {
            lblTitle.Text = parameters["MaxLEDFormTitle"].ToString();
            Top = Convert.ToInt32(parameters["MaxLEDFormTop"]);
            Left = Convert.ToInt32(parameters["MaxLEDFormLeft"]);
            Width = Convert.ToInt32(parameters["MaxLEDFormWidth"]);
            Height = Convert.ToInt32(parameters["MaxLEDFormHeight"]);
        }

        private void MaxLEDForm_Resize(object sender, EventArgs e)
        {
            lblTitle.Left = (Width - lblTitle.Width )/2;
            sortingStatus_Line01.Top = 50;
            sortingStatus_Line01.Left = 2;
            sortingStatus_Line01.Width = Width - 4;
            sortingStatus_Line01.Height = (Height - 54 - lblCompanyName.Height - 8 ) / 2;

            sortingStatus_Line02.Top = 52 + sortingStatus_Line01.Height;
            sortingStatus_Line02.Left = 2;
            sortingStatus_Line02.Width = Width - 4;
            sortingStatus_Line02.Height = (Height - 54 - lblCompanyName.Height -8 ) / 2;

            lblCompanyName.Top = Height - lblCompanyName.Height - 4;
            lblCompanyName.Left = Width - lblCompanyName.Width - 2;
        }
    }
}