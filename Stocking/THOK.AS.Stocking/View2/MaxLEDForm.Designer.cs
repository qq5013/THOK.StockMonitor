namespace THOK.AS.Stocking.View
{
    partial class MaxLEDForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblCompanyName = new System.Windows.Forms.Label();
            this.sortingStatus_Line02 = new THOK.AS.Stocking.View.SortingStatus();
            this.sortingStatus_Line01 = new THOK.AS.Stocking.View.SortingStatus();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblTitle.Location = new System.Drawing.Point(163, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(208, 27);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "分拣信息显示屏";
            // 
            // lblCompanyName
            // 
            this.lblCompanyName.AutoSize = true;
            this.lblCompanyName.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCompanyName.ForeColor = System.Drawing.Color.Lime;
            this.lblCompanyName.Location = new System.Drawing.Point(333, 342);
            this.lblCompanyName.Name = "lblCompanyName";
            this.lblCompanyName.Size = new System.Drawing.Size(231, 13);
            this.lblCompanyName.TabIndex = 3;
            this.lblCompanyName.Text = "天海欧康科技信息（厦门）有限公司";
            // 
            // sortingStatus_Line02
            // 
            this.sortingStatus_Line02.BackColor = System.Drawing.SystemColors.ControlText;
            this.sortingStatus_Line02.Location = new System.Drawing.Point(2, 189);
            this.sortingStatus_Line02.Name = "sortingStatus_Line02";
            this.sortingStatus_Line02.Size = new System.Drawing.Size(514, 130);
            this.sortingStatus_Line02.TabIndex = 1;
            // 
            // sortingStatus_Line01
            // 
            this.sortingStatus_Line01.BackColor = System.Drawing.SystemColors.ControlText;
            this.sortingStatus_Line01.Location = new System.Drawing.Point(2, 50);
            this.sortingStatus_Line01.Name = "sortingStatus_Line01";
            this.sortingStatus_Line01.Size = new System.Drawing.Size(514, 130);
            this.sortingStatus_Line01.TabIndex = 0;
            // 
            // MaxLEDForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(728, 392);
            this.Controls.Add(this.lblCompanyName);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.sortingStatus_Line02);
            this.Controls.Add(this.sortingStatus_Line01);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MaxLEDForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MaxLEDForm";
            this.TransparencyKey = System.Drawing.SystemColors.ActiveBorder;
            this.Resize += new System.EventHandler(this.MaxLEDForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public SortingStatus sortingStatus_Line01;
        public SortingStatus sortingStatus_Line02;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblCompanyName;
    }
}