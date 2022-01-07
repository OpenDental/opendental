using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormFeeScheds {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFeeScheds));
			this.butOK = new OpenDental.UI.Button();
			this.labelCleanUp = new System.Windows.Forms.Label();
			this.butCleanUp = new OpenDental.UI.Button();
			this.labelSort = new System.Windows.Forms.Label();
			this.butSort = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.butIns = new OpenDental.UI.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.listType = new OpenDental.UI.ListBoxOD();
			this.butAdd = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.labelHideUnused = new System.Windows.Forms.Label();
			this.butHideUnused = new OpenDental.UI.Button();
			this.butSetOrder = new System.Windows.Forms.Button();
			this.labelSetOrder = new System.Windows.Forms.Label();
			this.checkBoxShowHidden = new System.Windows.Forms.CheckBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.groupBox7.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(412, 570);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 23;
			this.butOK.Text = "&OK";
			this.butOK.Visible = false;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelCleanUp
			// 
			this.labelCleanUp.Location = new System.Drawing.Point(315, 442);
			this.labelCleanUp.Name = "labelCleanUp";
			this.labelCleanUp.Size = new System.Drawing.Size(161, 36);
			this.labelCleanUp.TabIndex = 22;
			this.labelCleanUp.Text = "Deletes any allowed fee schedules that are not in use.";
			// 
			// butCleanUp
			// 
			this.butCleanUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCleanUp.Location = new System.Drawing.Point(318, 415);
			this.butCleanUp.Name = "butCleanUp";
			this.butCleanUp.Size = new System.Drawing.Size(99, 24);
			this.butCleanUp.TabIndex = 21;
			this.butCleanUp.Text = "Clean Up Allowed";
			this.butCleanUp.Click += new System.EventHandler(this.butCleanUp_Click);
			// 
			// labelSort
			// 
			this.labelSort.Location = new System.Drawing.Point(315, 321);
			this.labelSort.Name = "labelSort";
			this.labelSort.Size = new System.Drawing.Size(123, 44);
			this.labelSort.TabIndex = 20;
			this.labelSort.Text = "Sorts by type and alphabetically";
			// 
			// butSort
			// 
			this.butSort.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSort.Location = new System.Drawing.Point(318, 294);
			this.butSort.Name = "butSort";
			this.butSort.Size = new System.Drawing.Size(75, 24);
			this.butSort.TabIndex = 19;
			this.butSort.Text = "Sort";
			this.butSort.Click += new System.EventHandler(this.butSort_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(315, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 18);
			this.label1.TabIndex = 18;
			this.label1.Text = "Type";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.butIns);
			this.groupBox7.Controls.Add(this.label6);
			this.groupBox7.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox7.Location = new System.Drawing.Point(17, 570);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(340, 58);
			this.groupBox7.TabIndex = 17;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Check Ins Plan Fee Schedules";
			// 
			// butIns
			// 
			this.butIns.Location = new System.Drawing.Point(248, 19);
			this.butIns.Name = "butIns";
			this.butIns.Size = new System.Drawing.Size(75, 24);
			this.butIns.TabIndex = 4;
			this.butIns.Text = "Go";
			this.butIns.Click += new System.EventHandler(this.butIns_Click);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(6, 16);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(229, 39);
			this.label6.TabIndex = 5;
			this.label6.Text = "This tool will help make sure your insurance plans have the right fee schedules s" +
    "et.";
			// 
			// butDown
			// 
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(318, 170);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(75, 24);
			this.butDown.TabIndex = 16;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butUp
			// 
			this.butUp.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(318, 138);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(75, 24);
			this.butUp.TabIndex = 15;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// listType
			// 
			this.listType.Location = new System.Drawing.Point(318, 26);
			this.listType.Name = "listType";
			this.listType.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listType.Size = new System.Drawing.Size(120, 69);
			this.listType.TabIndex = 12;
			this.listType.SelectionChangeCommitted += new System.EventHandler(this.listType_SelectionChangeCommitted);
			// 
			// butAdd
			// 
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(318, 368);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 10;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(412, 604);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// labelHideUnused
			// 
			this.labelHideUnused.Location = new System.Drawing.Point(315, 508);
			this.labelHideUnused.Name = "labelHideUnused";
			this.labelHideUnused.Size = new System.Drawing.Size(161, 36);
			this.labelHideUnused.TabIndex = 25;
			this.labelHideUnused.Text = "Hides any fee schedules that are not in use";
			// 
			// butHideUnused
			// 
			this.butHideUnused.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butHideUnused.Location = new System.Drawing.Point(318, 481);
			this.butHideUnused.Name = "butHideUnused";
			this.butHideUnused.Size = new System.Drawing.Size(99, 24);
			this.butHideUnused.TabIndex = 22;
			this.butHideUnused.Text = "Hide Unused";
			this.butHideUnused.Click += new System.EventHandler(this.butHideUnused_Click);
			// 
			// butSetOrder
			// 
			this.butSetOrder.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSetOrder.Location = new System.Drawing.Point(318, 218);
			this.butSetOrder.Name = "butSetOrder";
			this.butSetOrder.Size = new System.Drawing.Size(75, 24);
			this.butSetOrder.TabIndex = 26;
			this.butSetOrder.Text = "Set Order";
			this.butSetOrder.Click += new System.EventHandler(this.butSetOrder_Click);
			// 
			// labelSetOrder
			// 
			this.labelSetOrder.Location = new System.Drawing.Point(394, 211);
			this.labelSetOrder.Name = "labelSetOrder";
			this.labelSetOrder.Size = new System.Drawing.Size(123, 44);
			this.labelSetOrder.TabIndex = 27;
			this.labelSetOrder.Text = "1. Select row to move.\r\n2. Toggle this button.\r\n3. Click new position.";
			// 
			// checkBoxShowHidden
			// 
			this.checkBoxShowHidden.Location = new System.Drawing.Point(318, 101);
			this.checkBoxShowHidden.Name = "checkBoxShowHidden";
			this.checkBoxShowHidden.Size = new System.Drawing.Size(120, 17);
			this.checkBoxShowHidden.TabIndex = 28;
			this.checkBoxShowHidden.Text = "Show Hidden";
			this.checkBoxShowHidden.UseVisualStyleBackColor = true;
			this.checkBoxShowHidden.Click += new System.EventHandler(this.checkBoxShowHidden_Click);
			// 
			// gridMain
			// 
			this.gridMain.Location = new System.Drawing.Point(17, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(278, 552);
			this.gridMain.TabIndex = 11;
			this.gridMain.Title = "FeeSchedules";
			this.gridMain.TranslationName = "TableFeeScheds";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// FormFeeScheds
			// 
			this.ClientSize = new System.Drawing.Size(515, 644);
			this.Controls.Add(this.checkBoxShowHidden);
			this.Controls.Add(this.labelSetOrder);
			this.Controls.Add(this.butSetOrder);
			this.Controls.Add(this.labelHideUnused);
			this.Controls.Add(this.butHideUnused);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelCleanUp);
			this.Controls.Add(this.butCleanUp);
			this.Controls.Add(this.labelSort);
			this.Controls.Add(this.butSort);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox7);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.listType);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormFeeScheds";
			this.ShowInTaskbar = false;
			this.Text = "Fee Schedules";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormFeeSchedules_FormClosing);
			this.Load += new System.EventHandler(this.FormFeeSchedules_Load);
			this.groupBox7.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butClose;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.ListBoxOD listType;
		private OpenDental.UI.Button butDown;
		private OpenDental.UI.Button butUp;
		private GroupBox groupBox7;
		private OpenDental.UI.Button butIns;
		private Label label6;
		private Label label1;
		private OpenDental.UI.Button butSort;
		private Label labelSort;
		private Label labelCleanUp;
		private UI.Button butCleanUp;
		private UI.Button butOK;
		private Label labelHideUnused;
		private UI.Button butHideUnused;
		private System.Windows.Forms.Button butSetOrder;
		private Label labelSetOrder;
		private CheckBox checkBoxShowHidden;
	}
}
