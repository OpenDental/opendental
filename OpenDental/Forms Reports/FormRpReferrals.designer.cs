using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpReferrals {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpReferrals));
			this.tabReferrals = new System.Windows.Forms.TabControl();
			this.tabData = new System.Windows.Forms.TabPage();
			this.listSelect = new OpenDental.UI.ListBoxOD();
			this.butCheckAll = new OpenDental.UI.Button();
			this.butClear = new OpenDental.UI.Button();
			this.tabFilters = new System.Windows.Forms.TabPage();
			this.butDeleteFilter = new OpenDental.UI.Button();
			this.listOptions = new OpenDental.UI.ListBoxOD();
			this.listPrerequisites = new OpenDental.UI.ListBoxOD();
			this.butAddFilter = new OpenDental.UI.Button();
			this.listConditions = new OpenDental.UI.ListBoxOD();
			this.textBox = new System.Windows.Forms.TextBox();
			this.DropListFilter = new System.Windows.Forms.ComboBox();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.textSQL = new System.Windows.Forms.TextBox();
			this.tabReferrals.SuspendLayout();
			this.tabData.SuspendLayout();
			this.tabFilters.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabReferrals
			// 
			this.tabReferrals.Controls.Add(this.tabData);
			this.tabReferrals.Controls.Add(this.tabFilters);
			this.tabReferrals.Location = new System.Drawing.Point(14,16);
			this.tabReferrals.Name = "tabReferrals";
			this.tabReferrals.SelectedIndex = 0;
			this.tabReferrals.Size = new System.Drawing.Size(814,492);
			this.tabReferrals.TabIndex = 39;
			// 
			// tabData
			// 
			this.tabData.Controls.Add(this.listSelect);
			this.tabData.Controls.Add(this.butCheckAll);
			this.tabData.Controls.Add(this.butClear);
			this.tabData.Location = new System.Drawing.Point(4,22);
			this.tabData.Name = "tabData";
			this.tabData.Size = new System.Drawing.Size(806,466);
			this.tabData.TabIndex = 1;
			this.tabData.Text = "SELECT";
			// 
			// listSelect
			// 
			this.listSelect.Location = new System.Drawing.Point(8,8);
			this.listSelect.Name = "listSelect";
			this.listSelect.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listSelect.Size = new System.Drawing.Size(184,407);
			this.listSelect.TabIndex = 3;
			this.listSelect.SelectedIndexChanged += new System.EventHandler(this.listSelect_SelectedIndexChanged);
			// 
			// butCheckAll
			// 
			this.butCheckAll.Location = new System.Drawing.Point(10,430);
			this.butCheckAll.Name = "butCheckAll";
			this.butCheckAll.Size = new System.Drawing.Size(80,26);
			this.butCheckAll.TabIndex = 1;
			this.butCheckAll.Text = "&All";
			this.butCheckAll.Click += new System.EventHandler(this.butAll_Click);
			// 
			// butClear
			// 
			this.butClear.Location = new System.Drawing.Point(100,430);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(80,26);
			this.butClear.TabIndex = 2;
			this.butClear.Text = "&None";
			this.butClear.Click += new System.EventHandler(this.butNone_Click);
			// 
			// tabFilters
			// 
			this.tabFilters.Controls.Add(this.butDeleteFilter);
			this.tabFilters.Controls.Add(this.listOptions);
			this.tabFilters.Controls.Add(this.listPrerequisites);
			this.tabFilters.Controls.Add(this.butAddFilter);
			this.tabFilters.Controls.Add(this.listConditions);
			this.tabFilters.Controls.Add(this.textBox);
			this.tabFilters.Controls.Add(this.DropListFilter);
			this.tabFilters.Location = new System.Drawing.Point(4,22);
			this.tabFilters.Name = "tabFilters";
			this.tabFilters.Size = new System.Drawing.Size(806,466);
			this.tabFilters.TabIndex = 0;
			this.tabFilters.Text = "WHERE";
			this.tabFilters.Visible = false;
			// 
			// butDeleteFilter
			// 
			this.butDeleteFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDeleteFilter.Image = ((System.Drawing.Image)(resources.GetObject("butDeleteFilter.Image")));
			this.butDeleteFilter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteFilter.Location = new System.Drawing.Point(10,426);
			this.butDeleteFilter.Name = "butDeleteFilter";
			this.butDeleteFilter.Size = new System.Drawing.Size(110,26);
			this.butDeleteFilter.TabIndex = 34;
			this.butDeleteFilter.Text = "&Delete Row";
			this.butDeleteFilter.Click += new System.EventHandler(this.butDeleteFilter_Click);
			// 
			// comboBox
			// 
			this.listOptions.Location = new System.Drawing.Point(358,12);
			this.listOptions.Name = "comboBox";
			this.listOptions.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listOptions.Size = new System.Drawing.Size(266,199);
			this.listOptions.TabIndex = 12;
			this.listOptions.Visible = false;
			this.listOptions.SelectedIndexChanged += new System.EventHandler(this.listOptions_SelectedIndexChanged);
			// 
			// ListPrerequisites
			// 
			this.listPrerequisites.Location = new System.Drawing.Point(10,234);
			this.listPrerequisites.Name = "ListPrerequisites";
			this.listPrerequisites.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listPrerequisites.Size = new System.Drawing.Size(608,173);
			this.listPrerequisites.TabIndex = 7;
			this.listPrerequisites.SelectedIndexChanged += new System.EventHandler(this.ListPrerequisites_SelectedIndexChanged);
			// 
			// butAddFilter
			// 
			this.butAddFilter.Enabled = false;
			this.butAddFilter.Location = new System.Drawing.Point(664,12);
			this.butAddFilter.Name = "butAddFilter";
			this.butAddFilter.Size = new System.Drawing.Size(75,23);
			this.butAddFilter.TabIndex = 6;
			this.butAddFilter.Text = "&Add";
			this.butAddFilter.Click += new System.EventHandler(this.butAddFilter_Click);
			// 
			// ListConditions
			// 
			this.listConditions.Items.AddRange(new object[] {
            "LIKE",
            "=",
            ">",
            "<",
            ">=",
            "<=",
            "<>"});
			this.listConditions.Location = new System.Drawing.Point(232,12);
			this.listConditions.Name = "ListConditions";
			this.listConditions.Size = new System.Drawing.Size(78,95);
			this.listConditions.TabIndex = 5;
			// 
			// textBox
			// 
			this.textBox.Location = new System.Drawing.Point(358,12);
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(262,20);
			this.textBox.TabIndex = 2;
			this.textBox.Visible = false;
			// 
			// DropListFilter
			// 
			this.DropListFilter.Location = new System.Drawing.Point(8,12);
			this.DropListFilter.MaxDropDownItems = 45;
			this.DropListFilter.Name = "DropListFilter";
			this.DropListFilter.Size = new System.Drawing.Size(172,21);
			this.DropListFilter.TabIndex = 1;
			this.DropListFilter.Text = "WHERE";
			this.DropListFilter.SelectedIndexChanged += new System.EventHandler(this.DropListFilter_SelectedIndexChanged);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(750,640);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,26);
			this.butCancel.TabIndex = 41;
			this.butCancel.Text = "&Cancel";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Enabled = false;
			this.butOK.Location = new System.Drawing.Point(750,602);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,26);
			this.butOK.TabIndex = 40;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// textSQL
			// 
			this.textSQL.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSQL.Location = new System.Drawing.Point(16,542);
			this.textSQL.Multiline = true;
			this.textSQL.Name = "textSQL";
			this.textSQL.ReadOnly = true;
			this.textSQL.Size = new System.Drawing.Size(692,124);
			this.textSQL.TabIndex = 42;
			// 
			// FormRpReferrals
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(842,683);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.tabReferrals);
			this.Controls.Add(this.textSQL);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpReferrals";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "FormRpReferrals";
			this.tabReferrals.ResumeLayout(false);
			this.tabData.ResumeLayout(false);
			this.tabFilters.ResumeLayout(false);
			this.tabFilters.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.TabPage tabData;
		private OpenDental.UI.Button butCheckAll;
		private OpenDental.UI.Button butClear;
		private System.Windows.Forms.TabPage tabFilters;
		private OpenDental.UI.ListBoxOD listOptions;
		private OpenDental.UI.ListBoxOD listPrerequisites;
		private OpenDental.UI.Button butAddFilter;
		private OpenDental.UI.ListBoxOD listConditions;
		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.ComboBox DropListFilter;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.TextBox textSQL;
		private System.Windows.Forms.TabControl tabReferrals;
		private OpenDental.UI.ListBoxOD listSelect;
		private OpenDental.UI.Button butDeleteFilter;
	}
}
