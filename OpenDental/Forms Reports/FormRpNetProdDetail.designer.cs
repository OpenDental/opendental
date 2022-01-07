using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpNetProdDetail {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpNetProdDetail));
			this.label1 = new System.Windows.Forms.Label();
			this.listProv = new OpenDental.UI.ListBoxOD();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textToday = new System.Windows.Forms.TextBox();
			this.groupDateRange = new System.Windows.Forms.GroupBox();
			this.dtPickerTo = new System.Windows.Forms.DateTimePicker();
			this.dtPickerFrom = new System.Windows.Forms.DateTimePicker();
			this.butRight = new OpenDental.UI.Button();
			this.butThis = new OpenDental.UI.Button();
			this.butLeft = new OpenDental.UI.Button();
			this.listClin = new OpenDental.UI.ListBoxOD();
			this.labelClin = new System.Windows.Forms.Label();
			this.checkAllProv = new System.Windows.Forms.CheckBox();
			this.checkAllClin = new System.Windows.Forms.CheckBox();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.groupPayrollReportType = new System.Windows.Forms.GroupBox();
			this.radioTransactionalToday = new System.Windows.Forms.RadioButton();
			this.radioTransactionalHistorical = new System.Windows.Forms.RadioButton();
			this.labelSpecialReport = new System.Windows.Forms.Label();
			this.groupDateRange.SuspendLayout();
			this.groupPayrollReportType.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(26, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 29;
			this.label1.Text = "Providers";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listProv
			// 
			this.listProv.Location = new System.Drawing.Point(28, 54);
			this.listProv.Name = "listProv";
			this.listProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProv.Size = new System.Drawing.Size(154, 212);
			this.listProv.TabIndex = 30;
			this.listProv.Click += new System.EventHandler(this.listProv_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 18);
			this.label2.TabIndex = 37;
			this.label2.Text = "From";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7, 90);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82, 18);
			this.label3.TabIndex = 39;
			this.label3.Text = "To";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(355, 24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(127, 20);
			this.label4.TabIndex = 41;
			this.label4.Text = "Today\'s Date";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textToday
			// 
			this.textToday.Location = new System.Drawing.Point(483, 21);
			this.textToday.Name = "textToday";
			this.textToday.ReadOnly = true;
			this.textToday.Size = new System.Drawing.Size(100, 20);
			this.textToday.TabIndex = 42;
			// 
			// groupDateRange
			// 
			this.groupDateRange.Controls.Add(this.dtPickerTo);
			this.groupDateRange.Controls.Add(this.dtPickerFrom);
			this.groupDateRange.Controls.Add(this.butRight);
			this.groupDateRange.Controls.Add(this.butThis);
			this.groupDateRange.Controls.Add(this.label2);
			this.groupDateRange.Controls.Add(this.label3);
			this.groupDateRange.Controls.Add(this.butLeft);
			this.groupDateRange.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupDateRange.Location = new System.Drawing.Point(389, 48);
			this.groupDateRange.Name = "groupDateRange";
			this.groupDateRange.Size = new System.Drawing.Size(281, 125);
			this.groupDateRange.TabIndex = 43;
			this.groupDateRange.TabStop = false;
			this.groupDateRange.Text = "Pay Period Date Range";
			// 
			// dtPickerTo
			// 
			this.dtPickerTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dtPickerTo.Location = new System.Drawing.Point(95, 88);
			this.dtPickerTo.Name = "dtPickerTo";
			this.dtPickerTo.Size = new System.Drawing.Size(101, 20);
			this.dtPickerTo.TabIndex = 48;
			this.dtPickerTo.ValueChanged += new System.EventHandler(this.dtPickerTo_ValueChanged);
			// 
			// dtPickerFrom
			// 
			this.dtPickerFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dtPickerFrom.Location = new System.Drawing.Point(94, 62);
			this.dtPickerFrom.Name = "dtPickerFrom";
			this.dtPickerFrom.Size = new System.Drawing.Size(101, 20);
			this.dtPickerFrom.TabIndex = 47;
			this.dtPickerFrom.ValueChanged += new System.EventHandler(this.dtPickerFrom_ValueChanged);
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(205, 26);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(45, 26);
			this.butRight.TabIndex = 46;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butThis
			// 
			this.butThis.Location = new System.Drawing.Point(95, 26);
			this.butThis.Name = "butThis";
			this.butThis.Size = new System.Drawing.Size(101, 26);
			this.butThis.TabIndex = 45;
			this.butThis.Text = "This Period";
			this.butThis.Click += new System.EventHandler(this.butThis_Click);
			// 
			// butLeft
			// 
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(41, 26);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(45, 26);
			this.butLeft.TabIndex = 44;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// listClin
			// 
			this.listClin.Location = new System.Drawing.Point(206, 54);
			this.listClin.Name = "listClin";
			this.listClin.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listClin.Size = new System.Drawing.Size(154, 212);
			this.listClin.TabIndex = 45;
			this.listClin.Click += new System.EventHandler(this.listClin_Click);
			// 
			// labelClin
			// 
			this.labelClin.Location = new System.Drawing.Point(203, 17);
			this.labelClin.Name = "labelClin";
			this.labelClin.Size = new System.Drawing.Size(104, 16);
			this.labelClin.TabIndex = 44;
			this.labelClin.Text = "Clinics";
			this.labelClin.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkAllProv
			// 
			this.checkAllProv.Checked = true;
			this.checkAllProv.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllProv.Location = new System.Drawing.Point(29, 35);
			this.checkAllProv.Name = "checkAllProv";
			this.checkAllProv.Size = new System.Drawing.Size(47, 16);
			this.checkAllProv.TabIndex = 47;
			this.checkAllProv.Text = "All";
			this.checkAllProv.Click += new System.EventHandler(this.checkAllProv_Click);
			// 
			// checkAllClin
			// 
			this.checkAllClin.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllClin.Location = new System.Drawing.Point(206, 35);
			this.checkAllClin.Name = "checkAllClin";
			this.checkAllClin.Size = new System.Drawing.Size(95, 16);
			this.checkAllClin.TabIndex = 48;
			this.checkAllClin.Text = "All";
			this.checkAllClin.Click += new System.EventHandler(this.checkAllClin_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(692, 287);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(692, 255);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// groupPayrollReportType
			// 
			this.groupPayrollReportType.Controls.Add(this.radioTransactionalToday);
			this.groupPayrollReportType.Controls.Add(this.radioTransactionalHistorical);
			this.groupPayrollReportType.Location = new System.Drawing.Point(389, 179);
			this.groupPayrollReportType.Name = "groupPayrollReportType";
			this.groupPayrollReportType.Size = new System.Drawing.Size(281, 87);
			this.groupPayrollReportType.TabIndex = 53;
			this.groupPayrollReportType.TabStop = false;
			this.groupPayrollReportType.Text = "Report Types";
			// 
			// radioTransactionalToday
			// 
			this.radioTransactionalToday.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioTransactionalToday.Location = new System.Drawing.Point(12, 49);
			this.radioTransactionalToday.Name = "radioTransactionalToday";
			this.radioTransactionalToday.Size = new System.Drawing.Size(244, 17);
			this.radioTransactionalToday.TabIndex = 5;
			this.radioTransactionalToday.Text = "Today";
			this.radioTransactionalToday.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioTransactionalToday.UseVisualStyleBackColor = true;
			this.radioTransactionalToday.Click += new System.EventHandler(this.radioTransactionalToday_Click);
			// 
			// radioTransactionalHistorical
			// 
			this.radioTransactionalHistorical.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioTransactionalHistorical.Location = new System.Drawing.Point(12, 26);
			this.radioTransactionalHistorical.Name = "radioTransactionalHistorical";
			this.radioTransactionalHistorical.Size = new System.Drawing.Size(244, 17);
			this.radioTransactionalHistorical.TabIndex = 4;
			this.radioTransactionalHistorical.Text = "Date Range";
			this.radioTransactionalHistorical.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioTransactionalHistorical.UseVisualStyleBackColor = true;
			this.radioTransactionalHistorical.Click += new System.EventHandler(this.radioTransactionalHistorical_Click);
			// 
			// labelSpecialReport
			// 
			this.labelSpecialReport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelSpecialReport.Location = new System.Drawing.Point(26, 287);
			this.labelSpecialReport.Name = "labelSpecialReport";
			this.labelSpecialReport.Size = new System.Drawing.Size(645, 27);
			this.labelSpecialReport.TabIndex = 54;
			this.labelSpecialReport.Text = "*This is a special report that may not match other reports due to using different" +
    " transaction dates.  See manual for more details.";
			this.labelSpecialReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormRpNetProdDetail
			// 
			this.ClientSize = new System.Drawing.Size(791, 323);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelSpecialReport);
			this.Controls.Add(this.groupPayrollReportType);
			this.Controls.Add(this.checkAllClin);
			this.Controls.Add(this.checkAllProv);
			this.Controls.Add(this.listClin);
			this.Controls.Add(this.labelClin);
			this.Controls.Add(this.groupDateRange);
			this.Controls.Add(this.textToday);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.listProv);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpNetProdDetail";
			this.ShowInTaskbar = false;
			this.Text = "Net Production Detail Report";
			this.Load += new System.EventHandler(this.FormProduction_Load);
			this.groupDateRange.ResumeLayout(false);
			this.groupPayrollReportType.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.ListBoxOD listProv;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textToday;
		private System.Windows.Forms.GroupBox groupDateRange;
		private OpenDental.UI.ListBoxOD listClin;
		private Label labelClin;
		private CheckBox checkAllProv;
		private CheckBox checkAllClin;
		private GroupBox groupPayrollReportType;
		private UI.Button butLeft;
		private UI.Button butThis;
		private UI.Button butRight;
		private DateTimePicker dtPickerFrom;
		private DateTimePicker dtPickerTo;
		private RadioButton radioTransactionalHistorical;
		private RadioButton radioTransactionalToday;
		private Label labelSpecialReport;
	}
}
