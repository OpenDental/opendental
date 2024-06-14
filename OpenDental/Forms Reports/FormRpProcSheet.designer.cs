using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpProcSheet {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpProcSheet));
			this.butOK = new OpenDental.UI.Button();
			this.date2 = new System.Windows.Forms.MonthCalendar();
			this.date1 = new System.Windows.Forms.MonthCalendar();
			this.labelTO = new System.Windows.Forms.Label();
			this.listProv = new OpenDental.UI.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.radioIndividual = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.radioGrouped = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.textCode = new System.Windows.Forms.TextBox();
			this.checkAllProv = new OpenDental.UI.CheckBox();
			this.checkAllClin = new OpenDental.UI.CheckBox();
			this.listClin = new OpenDental.UI.ListBox();
			this.labelClin = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(634, 367);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// date2
			// 
			this.date2.Location = new System.Drawing.Point(284, 33);
			this.date2.Name = "date2";
			this.date2.TabIndex = 2;
			// 
			// date1
			// 
			this.date1.Location = new System.Drawing.Point(28, 33);
			this.date1.Name = "date1";
			this.date1.TabIndex = 1;
			// 
			// labelTO
			// 
			this.labelTO.Location = new System.Drawing.Point(234, 41);
			this.labelTO.Name = "labelTO";
			this.labelTO.Size = new System.Drawing.Size(72, 23);
			this.labelTO.TabIndex = 22;
			this.labelTO.Text = "TO";
			this.labelTO.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// listProv
			// 
			this.listProv.Location = new System.Drawing.Point(534, 48);
			this.listProv.Name = "listProv";
			this.listProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProv.Size = new System.Drawing.Size(175, 147);
			this.listProv.TabIndex = 33;
			this.listProv.Click += new System.EventHandler(this.listProv_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(532, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 32;
			this.label1.Text = "Providers";
			// 
			// radioIndividual
			// 
			this.radioIndividual.Checked = true;
			this.radioIndividual.Location = new System.Drawing.Point(11, 17);
			this.radioIndividual.Name = "radioIndividual";
			this.radioIndividual.Size = new System.Drawing.Size(207, 21);
			this.radioIndividual.TabIndex = 35;
			this.radioIndividual.TabStop = true;
			this.radioIndividual.Text = "Individual Procedures";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioGrouped);
			this.groupBox1.Controls.Add(this.radioIndividual);
			this.groupBox1.Location = new System.Drawing.Point(28, 229);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(239, 70);
			this.groupBox1.TabIndex = 36;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Type";
			// 
			// radioGrouped
			// 
			this.radioGrouped.Location = new System.Drawing.Point(11, 40);
			this.radioGrouped.Name = "radioGrouped";
			this.radioGrouped.Size = new System.Drawing.Size(215, 21);
			this.radioGrouped.TabIndex = 36;
			this.radioGrouped.Text = "Grouped By Procedure Code";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(26, 324);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(290, 20);
			this.label2.TabIndex = 37;
			this.label2.Text = "Only for procedure codes similar to:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textCode
			// 
			this.textCode.Location = new System.Drawing.Point(28, 348);
			this.textCode.Name = "textCode";
			this.textCode.Size = new System.Drawing.Size(100, 20);
			this.textCode.TabIndex = 38;
			// 
			// checkAllProv
			// 
			this.checkAllProv.Checked = true;
			this.checkAllProv.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllProv.Location = new System.Drawing.Point(534, 30);
			this.checkAllProv.Name = "checkAllProv";
			this.checkAllProv.Size = new System.Drawing.Size(95, 16);
			this.checkAllProv.TabIndex = 48;
			this.checkAllProv.Text = "All";
			this.checkAllProv.Click += new System.EventHandler(this.checkAllProv_Click);
			// 
			// checkAllClin
			// 
			this.checkAllClin.Location = new System.Drawing.Point(322, 227);
			this.checkAllClin.Name = "checkAllClin";
			this.checkAllClin.Size = new System.Drawing.Size(154, 16);
			this.checkAllClin.TabIndex = 54;
			this.checkAllClin.Text = "All (Includes hidden)";
			this.checkAllClin.Click += new System.EventHandler(this.checkAllClin_Click);
			// 
			// listClin
			// 
			this.listClin.Location = new System.Drawing.Point(322, 246);
			this.listClin.Name = "listClin";
			this.listClin.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClin.Size = new System.Drawing.Size(154, 147);
			this.listClin.TabIndex = 53;
			this.listClin.Click += new System.EventHandler(this.listClin_Click);
			// 
			// labelClin
			// 
			this.labelClin.Location = new System.Drawing.Point(319, 209);
			this.labelClin.Name = "labelClin";
			this.labelClin.Size = new System.Drawing.Size(104, 16);
			this.labelClin.TabIndex = 52;
			this.labelClin.Text = "Clinics";
			this.labelClin.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormRpProcSheet
			// 
			this.AcceptButton = this.butOK;
			this.ClientSize = new System.Drawing.Size(719, 402);
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.Controls.Add(this.checkAllClin);
			this.Controls.Add(this.listClin);
			this.Controls.Add(this.labelClin);
			this.Controls.Add(this.checkAllProv);
			this.Controls.Add(this.textCode);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.listProv);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.date2);
			this.Controls.Add(this.date1);
			this.Controls.Add(this.labelTO);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpProcSheet";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Daily Procedures Report";
			this.Load += new System.EventHandler(this.FormDailySummary_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.MonthCalendar date2;
		private System.Windows.Forms.MonthCalendar date1;
		private System.Windows.Forms.Label labelTO;
		private OpenDental.UI.ListBox listProv;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton radioIndividual;
		private OpenDental.UI.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioGrouped;
		private Label label2;
		private TextBox textCode;
		private OpenDental.UI.CheckBox checkAllProv;
		private OpenDental.UI.CheckBox checkAllClin;
		private OpenDental.UI.ListBox listClin;
		private Label labelClin;
	}
}
