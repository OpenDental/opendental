using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpPHRawPop {
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

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpPHRawPop));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.date2 = new System.Windows.Forms.MonthCalendar();
			this.date1 = new System.Windows.Forms.MonthCalendar();
			this.labelTO = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.listAdjType = new OpenDental.UI.ListBoxOD();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(665, 392);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(665, 360);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// date2
			// 
			this.date2.Location = new System.Drawing.Point(290, 67);
			this.date2.MaxSelectionCount = 1;
			this.date2.Name = "date2";
			this.date2.TabIndex = 2;
			// 
			// date1
			// 
			this.date1.Location = new System.Drawing.Point(34, 67);
			this.date1.MaxSelectionCount = 1;
			this.date1.Name = "date1";
			this.date1.TabIndex = 1;
			// 
			// labelTO
			// 
			this.labelTO.Location = new System.Drawing.Point(240, 75);
			this.labelTO.Name = "labelTO";
			this.labelTO.Size = new System.Drawing.Size(69, 23);
			this.labelTO.TabIndex = 28;
			this.labelTO.Text = "TO";
			this.labelTO.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(537, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(195, 43);
			this.label1.TabIndex = 29;
			this.label1.Text = "Adjustment types for broken appointments (hold down the control key to select mul" +
    "tiple)";
			// 
			// listAdjType
			// 
			this.listAdjType.Location = new System.Drawing.Point(538, 67);
			this.listAdjType.Name = "listAdjType";
			this.listAdjType.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listAdjType.Size = new System.Drawing.Size(149, 238);
			this.listAdjType.TabIndex = 30;
			// 
			// FormRpPHRawPop
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(765, 440);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.listAdjType);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.date2);
			this.Controls.Add(this.date1);
			this.Controls.Add(this.labelTO);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpPHRawPop";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Raw Population Data";
			this.Load += new System.EventHandler(this.FormRpPHRawPop_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.MonthCalendar date2;
		private System.Windows.Forms.MonthCalendar date1;
		private System.Windows.Forms.Label labelTO;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.ListBoxOD listAdjType;
	}
}
