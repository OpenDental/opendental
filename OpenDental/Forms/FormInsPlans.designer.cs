using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormInsPlans {
		private System.ComponentModel.IContainer components = null;// Required designer variable.

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInsPlans));
			this.textPlanNum = new System.Windows.Forms.TextBox();
			this.labelInsPlanID = new System.Windows.Forms.Label();
			this.butGetAll = new OpenDental.UI.Button();
			this.butHide = new OpenDental.UI.Button();
			this.checkShowHidden = new System.Windows.Forms.CheckBox();
			this.textTrojanID = new System.Windows.Forms.TextBox();
			this.labelTrojanID = new System.Windows.Forms.Label();
			this.butMerge = new OpenDental.UI.Button();
			this.textGroupNum = new System.Windows.Forms.TextBox();
			this.labelGroupNum = new System.Windows.Forms.Label();
			this.textGroupName = new System.Windows.Forms.TextBox();
			this.labelGroupName = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.textCarrier = new System.Windows.Forms.TextBox();
			this.labelCarrier = new System.Windows.Forms.Label();
			this.textEmployer = new System.Windows.Forms.TextBox();
			this.labelEmployer = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioOrderCarrier = new System.Windows.Forms.RadioButton();
			this.radioOrderEmp = new System.Windows.Forms.RadioButton();
			this.butOK = new OpenDental.UI.Button();
			this.butBlank = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
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
			this.labelInsPlanID.FlatStyle = System.Windows.Forms.FlatStyle.System;
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
			// butHide
			// 
			this.butHide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butHide.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butHide.Location = new System.Drawing.Point(104, 664);
			this.butHide.Name = "butHide";
			this.butHide.Size = new System.Drawing.Size(84, 24);
			this.butHide.TabIndex = 28;
			this.butHide.Text = "Hide Unused";
			this.butHide.Click += new System.EventHandler(this.butHide_Click);
			// 
			// checkShowHidden
			// 
			this.checkShowHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowHidden.Location = new System.Drawing.Point(837, 37);
			this.checkShowHidden.Name = "checkShowHidden";
			this.checkShowHidden.Size = new System.Drawing.Size(93, 20);
			this.checkShowHidden.TabIndex = 27;
			this.checkShowHidden.Text = "Show Hidden";
			this.checkShowHidden.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowHidden.UseVisualStyleBackColor = true;
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
			this.labelTrojanID.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelTrojanID.Location = new System.Drawing.Point(457, 18);
			this.labelTrojanID.Name = "labelTrojanID";
			this.labelTrojanID.Size = new System.Drawing.Size(81, 17);
			this.labelTrojanID.TabIndex = 26;
			this.labelTrojanID.Text = "Trojan ID";
			this.labelTrojanID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butMerge
			// 
			this.butMerge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butMerge.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butMerge.Location = new System.Drawing.Point(11, 664);
			this.butMerge.Name = "butMerge";
			this.butMerge.Size = new System.Drawing.Size(74, 24);
			this.butMerge.TabIndex = 24;
			this.butMerge.Text = "Combine";
			this.butMerge.Click += new System.EventHandler(this.butMerge_Click);
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
			this.labelGroupNum.FlatStyle = System.Windows.Forms.FlatStyle.System;
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
			this.labelGroupName.FlatStyle = System.Windows.Forms.FlatStyle.System;
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
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(11, 79);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(961, 578);
			this.gridMain.TabIndex = 19;
			this.gridMain.Title = "Insurance Plans";
			this.gridMain.TranslationName = "TableInsurancePlans";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
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
			this.labelCarrier.FlatStyle = System.Windows.Forms.FlatStyle.System;
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
			this.labelEmployer.FlatStyle = System.Windows.Forms.FlatStyle.System;
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
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(688, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(132, 40);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Order By";
			// 
			// radioOrderCarrier
			// 
			this.radioOrderCarrier.FlatStyle = System.Windows.Forms.FlatStyle.System;
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
			this.radioOrderEmp.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioOrderEmp.Location = new System.Drawing.Point(9, 13);
			this.radioOrderEmp.Name = "radioOrderEmp";
			this.radioOrderEmp.Size = new System.Drawing.Size(69, 16);
			this.radioOrderEmp.TabIndex = 0;
			this.radioOrderEmp.TabStop = true;
			this.radioOrderEmp.Text = "Employer";
			this.radioOrderEmp.Click += new System.EventHandler(this.radioOrderEmp_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(799, 664);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(78, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butBlank
			// 
			this.butBlank.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butBlank.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butBlank.Location = new System.Drawing.Point(427, 664);
			this.butBlank.Name = "butBlank";
			this.butBlank.Size = new System.Drawing.Size(87, 24);
			this.butBlank.TabIndex = 3;
			this.butBlank.Text = "Blank Plan";
			this.butBlank.Visible = false;
			this.butBlank.Click += new System.EventHandler(this.butBlank_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(894, 664);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(78, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textEmployer);
			this.groupBox1.Controls.Add(this.butGetAll);
			this.groupBox1.Controls.Add(this.textPlanNum);
			this.groupBox1.Controls.Add(this.labelEmployer);
			this.groupBox1.Controls.Add(this.checkShowHidden);
			this.groupBox1.Controls.Add(this.labelInsPlanID);
			this.groupBox1.Controls.Add(this.textCarrier);
			this.groupBox1.Controls.Add(this.labelCarrier);
			this.groupBox1.Controls.Add(this.groupBox2);
			this.groupBox1.Controls.Add(this.textGroupName);
			this.groupBox1.Controls.Add(this.textGroupNum);
			this.groupBox1.Controls.Add(this.textTrojanID);
			this.groupBox1.Controls.Add(this.labelGroupName);
			this.groupBox1.Controls.Add(this.labelTrojanID);
			this.groupBox1.Controls.Add(this.labelGroupNum);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(960, 61);
			this.groupBox1.TabIndex = 33;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Filter";
			// 
			// FormInsPlans
			// 
			this.ClientSize = new System.Drawing.Size(987, 696);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butHide);
			this.Controls.Add(this.butMerge);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butBlank);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormInsPlans";
			this.ShowInTaskbar = false;
			this.Text = "Insurance Plans";
			this.Load += new System.EventHandler(this.FormInsTemplates_Load);
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butBlank;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.GroupBox groupBox2;
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
		private OpenDental.UI.Button butMerge;
		private TextBox textTrojanID;
		private Label labelTrojanID;
		private CheckBox checkShowHidden;
		private OpenDental.UI.Button butHide;
		private UI.Button butGetAll;
		private TextBox textPlanNum;
		private Label labelInsPlanID;
		private GroupBox groupBox1;
	}
}
