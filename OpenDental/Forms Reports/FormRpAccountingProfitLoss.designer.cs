using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpAccountingProfitLoss {
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpAccountingProfitLoss));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.monthCalendarStart = new OpenDental.UI.MonthCalendarOD();
			this.monthCalendarEnd = new OpenDental.UI.MonthCalendarOD();
			this.labelTO = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(451, 251);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(370, 251);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// monthCalendarStart
			// 
			this.monthCalendarStart.Location = new System.Drawing.Point(30, 56);
			this.monthCalendarStart.Name = "monthCalendarStart";
			this.monthCalendarStart.Size = new System.Drawing.Size(227, 162);
			this.monthCalendarStart.TabIndex = 27;
			this.monthCalendarStart.Text = "monthCalendarStart";
			// 
			// monthCalendarEnd
			// 
			this.monthCalendarEnd.Location = new System.Drawing.Point(270, 56);
			this.monthCalendarEnd.Name = "monthCalendarEnd";
			this.monthCalendarEnd.Size = new System.Drawing.Size(227, 162);
			this.monthCalendarEnd.TabIndex = 28;
			this.monthCalendarEnd.Text = "monthCalendarEnd";
			// 
			// labelTO
			// 
			this.labelTO.Location = new System.Drawing.Point(27, 34);
			this.labelTO.Name = "labelTO";
			this.labelTO.Size = new System.Drawing.Size(72, 18);
			this.labelTO.TabIndex = 29;
			this.labelTO.Text = "From";
			this.labelTO.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(269, 34);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 18);
			this.label1.TabIndex = 30;
			this.label1.Text = "To";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormRpAccountingProfitLoss
			// 
			this.ClientSize = new System.Drawing.Size(566, 290);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelTO);
			this.Controls.Add(this.monthCalendarEnd);
			this.Controls.Add(this.monthCalendarStart);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpAccountingProfitLoss";
			this.ShowInTaskbar = false;
			this.Text = "Profit Loss Report";
			this.Load += new System.EventHandler(this.FormRpAccountingProfitLoss_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private UI.MonthCalendarOD monthCalendarStart;
		private UI.MonthCalendarOD monthCalendarEnd;
		private Label labelTO;
		private Label label1;
	}
}
