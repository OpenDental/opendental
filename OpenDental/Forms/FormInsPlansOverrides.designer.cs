using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormInsPlansOverrides {
		private System.ComponentModel.IContainer components = null;// Required designer variable.

		///<summary></summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components!=null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInsPlansOverrides));
			this.textPlanNum = new System.Windows.Forms.TextBox();
			this.labelInsPlanID = new System.Windows.Forms.Label();
			this.butGetAll = new OpenDental.UI.Button();
			this.checkShowHidden = new OpenDental.UI.CheckBox();
			this.textTrojanID = new System.Windows.Forms.TextBox();
			this.labelTrojanID = new System.Windows.Forms.Label();
			this.textGroupNum = new System.Windows.Forms.TextBox();
			this.labelGroupNum = new System.Windows.Forms.Label();
			this.textGroupName = new System.Windows.Forms.TextBox();
			this.labelGroupName = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.textCarrier = new System.Windows.Forms.TextBox();
			this.labelCarrier = new System.Windows.Forms.Label();
			this.textEmployer = new System.Windows.Forms.TextBox();
			this.labelEmployer = new System.Windows.Forms.Label();
			this.groupBox2 = new OpenDental.UI.GroupBox();
			this.radioOrderCarrier = new System.Windows.Forms.RadioButton();
			this.radioOrderEmp = new System.Windows.Forms.RadioButton();
			this.butClose = new OpenDental.UI.Button();
			this.groupBoxFilters = new OpenDental.UI.GroupBox();
			this.groupOverrides = new OpenDental.UI.GroupBox();
			this.groupNoBillInsOverrides = new OpenDental.UI.GroupBox();
			this.labelNoBillIns = new System.Windows.Forms.Label();
			this.labelBillToIns = new System.Windows.Forms.Label();
			this.labelDeleteNoBillIns = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.ButDontBill = new OpenDental.UI.Button();
			this.butBillIns = new OpenDental.UI.Button();
			this.butSelectAll = new OpenDental.UI.Button();
			this.groupBox2.SuspendLayout();
			this.groupBoxFilters.SuspendLayout();
			this.groupOverrides.SuspendLayout();
			this.groupNoBillInsOverrides.SuspendLayout();
			this.SuspendLayout();
			// 
			// textPlanNum
			// 
			this.textPlanNum.Location = new System.Drawing.Point(540, 36);
			this.textPlanNum.Name = "textPlanNum";
			this.textPlanNum.Size = new System.Drawing.Size(140, 20);
			this.textPlanNum.TabIndex = 31;
			this.textPlanNum.TextChanged += new System.EventHandler(this.textPlanNum_TextChanged);
			// 
			// labelInsPlanID
			// 
			this.labelInsPlanID.Location = new System.Drawing.Point(457, 38);
			this.labelInsPlanID.Name = "labelInsPlanID";
			this.labelInsPlanID.Size = new System.Drawing.Size(81, 17);
			this.labelInsPlanID.TabIndex = 32;
			this.labelInsPlanID.Text = "Ins Plan ID";
			this.labelInsPlanID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butGetAll
			// 
			this.butGetAll.Location = new System.Drawing.Point(854, 12);
			this.butGetAll.Name = "butGetAll";
			this.butGetAll.Size = new System.Drawing.Size(75, 24);
			this.butGetAll.TabIndex = 30;
			this.butGetAll.Text = "Get All";
			this.butGetAll.Click += new System.EventHandler(this.butGetAll_Click);
			// 
			// checkShowHidden
			// 
			this.checkShowHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowHidden.Location = new System.Drawing.Point(837, 37);
			this.checkShowHidden.Name = "checkShowHidden";
			this.checkShowHidden.Size = new System.Drawing.Size(93, 20);
			this.checkShowHidden.TabIndex = 27;
			this.checkShowHidden.Text = "Show Hidden";
			this.checkShowHidden.CheckedChanged += new System.EventHandler(this.checkShowHidden_CheckedChanged);
			// 
			// textTrojanID
			// 
			this.textTrojanID.Location = new System.Drawing.Point(540, 15);
			this.textTrojanID.Name = "textTrojanID";
			this.textTrojanID.Size = new System.Drawing.Size(140, 20);
			this.textTrojanID.TabIndex = 25;
			this.textTrojanID.TextChanged += new System.EventHandler(this.textTrojanID_TextChanged);
			// 
			// labelTrojanID
			// 
			this.labelTrojanID.Location = new System.Drawing.Point(457, 18);
			this.labelTrojanID.Name = "labelTrojanID";
			this.labelTrojanID.Size = new System.Drawing.Size(81, 17);
			this.labelTrojanID.TabIndex = 26;
			this.labelTrojanID.Text = "Trojan ID";
			this.labelTrojanID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textGroupNum
			// 
			this.textGroupNum.Location = new System.Drawing.Point(84, 35);
			this.textGroupNum.Name = "textGroupNum";
			this.textGroupNum.Size = new System.Drawing.Size(140, 20);
			this.textGroupNum.TabIndex = 20;
			this.textGroupNum.TextChanged += new System.EventHandler(this.textGroupNum_TextChanged);
			// 
			// labelGroupNum
			// 
			this.labelGroupNum.Location = new System.Drawing.Point(6, 37);
			this.labelGroupNum.Name = "labelGroupNum";
			this.labelGroupNum.Size = new System.Drawing.Size(76, 17);
			this.labelGroupNum.TabIndex = 23;
			this.labelGroupNum.Text = "Group Num";
			this.labelGroupNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textGroupName
			// 
			this.textGroupName.Location = new System.Drawing.Point(311, 36);
			this.textGroupName.Name = "textGroupName";
			this.textGroupName.Size = new System.Drawing.Size(140, 20);
			this.textGroupName.TabIndex = 21;
			this.textGroupName.TextChanged += new System.EventHandler(this.textGroupName_TextChanged);
			// 
			// labelGroupName
			// 
			this.labelGroupName.Location = new System.Drawing.Point(231, 36);
			this.labelGroupName.Name = "labelGroupName";
			this.labelGroupName.Size = new System.Drawing.Size(78, 17);
			this.labelGroupName.TabIndex = 22;
			this.labelGroupName.Text = "Group Name";
			this.labelGroupName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(11, 79);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(727, 578);
			this.gridMain.TabIndex = 19;
			this.gridMain.Title = "Insurance Plans";
			this.gridMain.TranslationName = "TableInsurancePlans";
			// 
			// textCarrier
			// 
			this.textCarrier.Location = new System.Drawing.Point(311, 15);
			this.textCarrier.Name = "textCarrier";
			this.textCarrier.Size = new System.Drawing.Size(140, 20);
			this.textCarrier.TabIndex = 0;
			this.textCarrier.TextChanged += new System.EventHandler(this.textCarrier_TextChanged);
			// 
			// labelCarrier
			// 
			this.labelCarrier.Location = new System.Drawing.Point(230, 17);
			this.labelCarrier.Name = "labelCarrier";
			this.labelCarrier.Size = new System.Drawing.Size(79, 17);
			this.labelCarrier.TabIndex = 17;
			this.labelCarrier.Text = "Carrier";
			this.labelCarrier.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textEmployer
			// 
			this.textEmployer.Location = new System.Drawing.Point(84, 14);
			this.textEmployer.Name = "textEmployer";
			this.textEmployer.Size = new System.Drawing.Size(140, 20);
			this.textEmployer.TabIndex = 1;
			this.textEmployer.TextChanged += new System.EventHandler(this.textEmployer_TextChanged);
			// 
			// labelEmployer
			// 
			this.labelEmployer.Location = new System.Drawing.Point(6, 16);
			this.labelEmployer.Name = "labelEmployer";
			this.labelEmployer.Size = new System.Drawing.Size(76, 17);
			this.labelEmployer.TabIndex = 15;
			this.labelEmployer.Text = "Employer";
			this.labelEmployer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.radioOrderCarrier);
			this.groupBox2.Controls.Add(this.radioOrderEmp);
			this.groupBox2.Location = new System.Drawing.Point(688, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(132, 40);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.Text = "Order By";
			// 
			// radioOrderCarrier
			// 
			this.radioOrderCarrier.Location = new System.Drawing.Point(75, 13);
			this.radioOrderCarrier.Name = "radioOrderCarrier";
			this.radioOrderCarrier.Size = new System.Drawing.Size(48, 16);
			this.radioOrderCarrier.TabIndex = 1;
			this.radioOrderCarrier.Text = "Carrier";
			this.radioOrderCarrier.Click += new System.EventHandler(this.radioOrderCarrier_Click);
			// 
			// radioOrderEmp
			// 
			this.radioOrderEmp.Checked = true;
			this.radioOrderEmp.Location = new System.Drawing.Point(9, 13);
			this.radioOrderEmp.Name = "radioOrderEmp";
			this.radioOrderEmp.Size = new System.Drawing.Size(69, 16);
			this.radioOrderEmp.TabIndex = 0;
			this.radioOrderEmp.TabStop = true;
			this.radioOrderEmp.Text = "Employer";
			this.radioOrderEmp.Click += new System.EventHandler(this.radioOrderEmp_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(894, 664);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(78, 24);
			this.butClose.TabIndex = 4;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// groupBoxFilters
			// 
			this.groupBoxFilters.Controls.Add(this.textEmployer);
			this.groupBoxFilters.Controls.Add(this.butGetAll);
			this.groupBoxFilters.Controls.Add(this.textPlanNum);
			this.groupBoxFilters.Controls.Add(this.labelEmployer);
			this.groupBoxFilters.Controls.Add(this.checkShowHidden);
			this.groupBoxFilters.Controls.Add(this.labelInsPlanID);
			this.groupBoxFilters.Controls.Add(this.textCarrier);
			this.groupBoxFilters.Controls.Add(this.labelCarrier);
			this.groupBoxFilters.Controls.Add(this.groupBox2);
			this.groupBoxFilters.Controls.Add(this.textGroupName);
			this.groupBoxFilters.Controls.Add(this.textGroupNum);
			this.groupBoxFilters.Controls.Add(this.textTrojanID);
			this.groupBoxFilters.Controls.Add(this.labelGroupName);
			this.groupBoxFilters.Controls.Add(this.labelTrojanID);
			this.groupBoxFilters.Controls.Add(this.labelGroupNum);
			this.groupBoxFilters.Location = new System.Drawing.Point(12, 12);
			this.groupBoxFilters.Name = "groupBoxFilters";
			this.groupBoxFilters.Size = new System.Drawing.Size(960, 61);
			this.groupBoxFilters.TabIndex = 33;
			this.groupBoxFilters.Text = "Filter";
			// 
			// groupOverrides
			// 
			this.groupOverrides.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupOverrides.Controls.Add(this.groupNoBillInsOverrides);
			this.groupOverrides.Location = new System.Drawing.Point(741, 79);
			this.groupOverrides.Name = "groupOverrides";
			this.groupOverrides.Size = new System.Drawing.Size(231, 579);
			this.groupOverrides.TabIndex = 34;
			this.groupOverrides.Text = "Overrides";
			// 
			// groupNoBillInsOverrides
			// 
			this.groupNoBillInsOverrides.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupNoBillInsOverrides.Controls.Add(this.labelNoBillIns);
			this.groupNoBillInsOverrides.Controls.Add(this.labelBillToIns);
			this.groupNoBillInsOverrides.Controls.Add(this.labelDeleteNoBillIns);
			this.groupNoBillInsOverrides.Controls.Add(this.butDelete);
			this.groupNoBillInsOverrides.Controls.Add(this.ButDontBill);
			this.groupNoBillInsOverrides.Controls.Add(this.butBillIns);
			this.groupNoBillInsOverrides.Location = new System.Drawing.Point(9, 27);
			this.groupNoBillInsOverrides.Name = "groupNoBillInsOverrides";
			this.groupNoBillInsOverrides.Size = new System.Drawing.Size(212, 137);
			this.groupNoBillInsOverrides.TabIndex = 37;
			this.groupNoBillInsOverrides.Text = "\'Do not usually bill to Ins\' Overrides";
			// 
			// labelNoBillIns
			// 
			this.labelNoBillIns.Location = new System.Drawing.Point(94, 62);
			this.labelNoBillIns.Name = "labelNoBillIns";
			this.labelNoBillIns.Size = new System.Drawing.Size(115, 35);
			this.labelNoBillIns.TabIndex = 40;
			this.labelNoBillIns.Text = "Do not usually bill to Insurance";
			this.labelNoBillIns.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelBillToIns
			// 
			this.labelBillToIns.Location = new System.Drawing.Point(94, 33);
			this.labelBillToIns.Name = "labelBillToIns";
			this.labelBillToIns.Size = new System.Drawing.Size(76, 17);
			this.labelBillToIns.TabIndex = 39;
			this.labelBillToIns.Text = "Bill to Insurance";
			this.labelBillToIns.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelDeleteNoBillIns
			// 
			this.labelDeleteNoBillIns.Location = new System.Drawing.Point(94, 103);
			this.labelDeleteNoBillIns.Name = "labelDeleteNoBillIns";
			this.labelDeleteNoBillIns.Size = new System.Drawing.Size(115, 17);
			this.labelDeleteNoBillIns.TabIndex = 41;
			this.labelDeleteNoBillIns.Text = "Delete overrides";
			this.labelDeleteNoBillIns.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butDelete.Location = new System.Drawing.Point(8, 99);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(78, 24);
			this.butDelete.TabIndex = 38;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// ButDontBill
			// 
			this.ButDontBill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ButDontBill.Location = new System.Drawing.Point(8, 64);
			this.ButDontBill.Name = "ButDontBill";
			this.ButDontBill.Size = new System.Drawing.Size(78, 24);
			this.ButDontBill.TabIndex = 37;
			this.ButDontBill.Text = "Don\'t Bill";
			this.ButDontBill.Click += new System.EventHandler(this.ButDontBill_Click);
			// 
			// butBillIns
			// 
			this.butBillIns.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butBillIns.Location = new System.Drawing.Point(8, 29);
			this.butBillIns.Name = "butBillIns";
			this.butBillIns.Size = new System.Drawing.Size(78, 24);
			this.butBillIns.TabIndex = 36;
			this.butBillIns.Text = "Bill To Ins";
			this.butBillIns.Click += new System.EventHandler(this.butBillIns_Click);
			// 
			// butSelectAll
			// 
			this.butSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSelectAll.Location = new System.Drawing.Point(660, 664);
			this.butSelectAll.Name = "butSelectAll";
			this.butSelectAll.Size = new System.Drawing.Size(78, 24);
			this.butSelectAll.TabIndex = 35;
			this.butSelectAll.Text = "Select All";
			this.butSelectAll.Click += new System.EventHandler(this.butSelectAll_Click);
			// 
			// FormInsPlansOverrides
			// 
			this.ClientSize = new System.Drawing.Size(987, 696);
			this.Controls.Add(this.butSelectAll);
			this.Controls.Add(this.groupBoxFilters);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.groupOverrides);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormInsPlansOverrides";
			this.ShowInTaskbar = false;
			this.Text = "Insurance Plans Overrides";
			this.Load += new System.EventHandler(this.FormInsTemplates_Load);
			this.groupBox2.ResumeLayout(false);
			this.groupBoxFilters.ResumeLayout(false);
			this.groupBoxFilters.PerformLayout();
			this.groupOverrides.ResumeLayout(false);
			this.groupNoBillInsOverrides.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		private OpenDental.UI.Button butClose;
		private OpenDental.UI.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioOrderCarrier;
		private System.Windows.Forms.RadioButton radioOrderEmp;
		private Label labelEmployer;
		private TextBox textEmployer;
		private TextBox textCarrier;
		private Label labelCarrier;
		private OpenDental.UI.GridOD gridMain;
		private TextBox textGroupNum;
		private Label labelGroupNum;
		private TextBox textGroupName;
		private Label labelGroupName;
		private TextBox textTrojanID;
		private Label labelTrojanID;
		private OpenDental.UI.CheckBox checkShowHidden;
		private UI.Button butGetAll;
		private TextBox textPlanNum;
		private Label labelInsPlanID;
		private OpenDental.UI.GroupBox groupBoxFilters;
		private UI.GroupBox groupOverrides;
		private UI.Button butSelectAll;
		private UI.GroupBox groupNoBillInsOverrides;
		private UI.Button butDelete;
		private UI.Button ButDontBill;
		private UI.Button butBillIns;
		private Label labelNoBillIns;
		private Label labelBillToIns;
		private Label labelDeleteNoBillIns;
	}
}
