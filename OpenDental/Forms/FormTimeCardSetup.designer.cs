using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTimeCardSetup {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTimeCardSetup));
			this.checkUseDecimal = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkShowSeconds = new System.Windows.Forms.CheckBox();
			this.checkAdjOverBreaks = new System.Windows.Forms.CheckBox();
			this.butAddRule = new OpenDental.UI.Button();
			this.gridRules = new OpenDental.UI.GridOD();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butAdd = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textADPCompanyCode = new System.Windows.Forms.TextBox();
			this.butGenerate = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.checkHideOlder = new System.Windows.Forms.CheckBox();
			this.butDeleteRules = new OpenDental.UI.Button();
			this.labelADPRunIID = new System.Windows.Forms.Label();
			this.textADPRunIID = new System.Windows.Forms.TextBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkUseDecimal
			// 
			this.checkUseDecimal.Location = new System.Drawing.Point(12, 19);
			this.checkUseDecimal.Name = "checkUseDecimal";
			this.checkUseDecimal.Size = new System.Drawing.Size(362, 18);
			this.checkUseDecimal.TabIndex = 12;
			this.checkUseDecimal.Text = "Use decimal format rather than colon format";
			this.checkUseDecimal.UseVisualStyleBackColor = true;
			this.checkUseDecimal.Click += new System.EventHandler(this.checkUseDecimal_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.checkShowSeconds);
			this.groupBox1.Controls.Add(this.checkAdjOverBreaks);
			this.groupBox1.Controls.Add(this.checkUseDecimal);
			this.groupBox1.Location = new System.Drawing.Point(305, 584);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(380, 74);
			this.groupBox1.TabIndex = 14;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Options";
			// 
			// checkShowSeconds
			// 
			this.checkShowSeconds.Location = new System.Drawing.Point(12, 51);
			this.checkShowSeconds.Name = "checkShowSeconds";
			this.checkShowSeconds.Size = new System.Drawing.Size(362, 18);
			this.checkShowSeconds.TabIndex = 14;
			this.checkShowSeconds.Text = "Use seconds on time card when using colon format";
			this.checkShowSeconds.UseVisualStyleBackColor = true;
			this.checkShowSeconds.Click += new System.EventHandler(this.checkShowSeconds_Click);
			// 
			// checkAdjOverBreaks
			// 
			this.checkAdjOverBreaks.Location = new System.Drawing.Point(12, 35);
			this.checkAdjOverBreaks.Name = "checkAdjOverBreaks";
			this.checkAdjOverBreaks.Size = new System.Drawing.Size(362, 18);
			this.checkAdjOverBreaks.TabIndex = 13;
			this.checkAdjOverBreaks.Text = "Calc Daily button makes adjustments if breaks over 30 minutes.";
			this.checkAdjOverBreaks.UseVisualStyleBackColor = true;
			this.checkAdjOverBreaks.Click += new System.EventHandler(this.checkAdjOverBreaks_Click);
			// 
			// butAddRule
			// 
			this.butAddRule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddRule.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddRule.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddRule.Location = new System.Drawing.Point(305, 554);
			this.butAddRule.Name = "butAddRule";
			this.butAddRule.Size = new System.Drawing.Size(80, 24);
			this.butAddRule.TabIndex = 15;
			this.butAddRule.Text = "Add";
			this.butAddRule.Click += new System.EventHandler(this.butAddRule_Click);
			// 
			// gridRules
			// 
			this.gridRules.AllowSortingByColumn = true;
			this.gridRules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridRules.Location = new System.Drawing.Point(305, 27);
			this.gridRules.Name = "gridRules";
			this.gridRules.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridRules.Size = new System.Drawing.Size(687, 523);
			this.gridRules.TabIndex = 13;
			this.gridRules.Title = "Rules";
			this.gridRules.TranslationName = "FormTimeCardSetup";
			this.gridRules.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridRules_CellDoubleClick);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.Location = new System.Drawing.Point(19, 27);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(272, 523);
			this.gridMain.TabIndex = 11;
			this.gridMain.Title = "Pay Periods";
			this.gridMain.TranslationName = "TablePayPeriods";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(19, 554);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(88, 24);
			this.butAdd.TabIndex = 10;
			this.butAdd.Text = "&Add One";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(917, 664);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.Location = new System.Drawing.Point(13, 668);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(119, 16);
			this.label2.TabIndex = 17;
			this.label2.Text = "ADP Company Code";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textADPCompanyCode
			// 
			this.textADPCompanyCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textADPCompanyCode.Location = new System.Drawing.Point(133, 666);
			this.textADPCompanyCode.Name = "textADPCompanyCode";
			this.textADPCompanyCode.Size = new System.Drawing.Size(97, 20);
			this.textADPCompanyCode.TabIndex = 16;
			// 
			// butGenerate
			// 
			this.butGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butGenerate.Icon = OpenDental.UI.EnumIcons.Add;
			this.butGenerate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butGenerate.Location = new System.Drawing.Point(19, 584);
			this.butGenerate.Name = "butGenerate";
			this.butGenerate.Size = new System.Drawing.Size(114, 23);
			this.butGenerate.TabIndex = 18;
			this.butGenerate.Text = "Generate Many";
			this.butGenerate.UseVisualStyleBackColor = true;
			this.butGenerate.Click += new System.EventHandler(this.butGenerate_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(180, 554);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(111, 24);
			this.butDelete.TabIndex = 19;
			this.butDelete.Text = "Delete Selected";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// checkHideOlder
			// 
			this.checkHideOlder.Checked = true;
			this.checkHideOlder.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkHideOlder.Location = new System.Drawing.Point(20, 7);
			this.checkHideOlder.Name = "checkHideOlder";
			this.checkHideOlder.Size = new System.Drawing.Size(288, 18);
			this.checkHideOlder.TabIndex = 20;
			this.checkHideOlder.Text = "Hide pay periods older than 6 months";
			this.checkHideOlder.UseVisualStyleBackColor = true;
			this.checkHideOlder.CheckedChanged += new System.EventHandler(this.checkHideOlder_CheckedChanged);
			// 
			// butDeleteRules
			// 
			this.butDeleteRules.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDeleteRules.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDeleteRules.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteRules.Location = new System.Drawing.Point(881, 556);
			this.butDeleteRules.Name = "butDeleteRules";
			this.butDeleteRules.Size = new System.Drawing.Size(111, 24);
			this.butDeleteRules.TabIndex = 21;
			this.butDeleteRules.Text = "Delete Selected";
			this.butDeleteRules.Click += new System.EventHandler(this.butDeleteRules_Click);
			// 
			// labelADPRunIID
			// 
			this.labelADPRunIID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelADPRunIID.Location = new System.Drawing.Point(13, 642);
			this.labelADPRunIID.Name = "labelADPRunIID";
			this.labelADPRunIID.Size = new System.Drawing.Size(119, 16);
			this.labelADPRunIID.TabIndex = 23;
			this.labelADPRunIID.Text = "ADPRunIID";
			this.labelADPRunIID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textADPRunIID
			// 
			this.textADPRunIID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textADPRunIID.Location = new System.Drawing.Point(133, 640);
			this.textADPRunIID.Name = "textADPRunIID";
			this.textADPRunIID.Size = new System.Drawing.Size(97, 20);
			this.textADPRunIID.TabIndex = 22;
			// 
			// FormTimeCardSetup
			// 
			this.ClientSize = new System.Drawing.Size(1020, 696);
			this.Controls.Add(this.labelADPRunIID);
			this.Controls.Add(this.textADPRunIID);
			this.Controls.Add(this.butDeleteRules);
			this.Controls.Add(this.checkHideOlder);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butGenerate);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textADPCompanyCode);
			this.Controls.Add(this.butAddRule);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridRules);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTimeCardSetup";
			this.ShowInTaskbar = false;
			this.Text = "Time Card Setup";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPayPeriods_FormClosing);
			this.Load += new System.EventHandler(this.FormPayPeriods_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butClose;
		private OpenDental.UI.GridOD gridMain;
		private CheckBox checkUseDecimal;
		private UI.GridOD gridRules;
		private GroupBox groupBox1;
		private OpenDental.UI.Button butAddRule;
		private CheckBox checkAdjOverBreaks;
		private Label label2;
		private TextBox textADPCompanyCode;
		private CheckBox checkShowSeconds;
		private UI.Button butGenerate;
		private UI.Button butDelete;
		private CheckBox checkHideOlder;
		private UI.Button butDeleteRules;
		private Label labelADPRunIID;
		private TextBox textADPRunIID;
	}
}
