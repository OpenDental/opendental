using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpPatients {
		private System.ComponentModel.IContainer components=null;

		///<summary></summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpPatients));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.tabPatients = new System.Windows.Forms.TabControl();
			this.tabData = new System.Windows.Forms.TabPage();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.labelReferredFrom = new System.Windows.Forms.Label();
			this.listReferredFromSelect = new OpenDental.UI.ListBoxOD();
			this.labelReferredTo = new System.Windows.Forms.Label();
			this.listReferredToSelect = new OpenDental.UI.ListBoxOD();
			this.labelPatient = new System.Windows.Forms.Label();
			this.listPatientSelect = new OpenDental.UI.ListBoxOD();
			this.tabFilters = new System.Windows.Forms.TabPage();
			this.labelHelp = new System.Windows.Forms.Label();
			this.listBoxColumns = new OpenDental.UI.ListBoxOD();
			this.TextDate = new OpenDental.ValidDate();
			this.TextValidAge = new OpenDental.ValidNum();
			this.butDeleteFilter = new OpenDental.UI.Button();
			this.listPrerequisites = new OpenDental.UI.ListBoxOD();
			this.butAddFilter = new OpenDental.UI.Button();
			this.listConditions = new OpenDental.UI.ListBoxOD();
			this.TextBox = new System.Windows.Forms.TextBox();
			this.DropListFilter = new System.Windows.Forms.ComboBox();
			this.TextSQL = new System.Windows.Forms.TextBox();
			this.tabPatients.SuspendLayout();
			this.tabData.SuspendLayout();
			this.tabFilters.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(876, 664);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Enabled = false;
			this.butOK.Location = new System.Drawing.Point(876, 630);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// tabPatients
			// 
			this.tabPatients.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabPatients.Controls.Add(this.tabData);
			this.tabPatients.Controls.Add(this.tabFilters);
			this.tabPatients.Location = new System.Drawing.Point(16, 6);
			this.tabPatients.Name = "tabPatients";
			this.tabPatients.SelectedIndex = 0;
			this.tabPatients.Size = new System.Drawing.Size(840, 544);
			this.tabPatients.TabIndex = 1;
			// 
			// tabData
			// 
			this.tabData.Controls.Add(this.textBox1);
			this.tabData.Controls.Add(this.labelReferredFrom);
			this.tabData.Controls.Add(this.listReferredFromSelect);
			this.tabData.Controls.Add(this.labelReferredTo);
			this.tabData.Controls.Add(this.listReferredToSelect);
			this.tabData.Controls.Add(this.labelPatient);
			this.tabData.Controls.Add(this.listPatientSelect);
			this.tabData.Location = new System.Drawing.Point(4, 22);
			this.tabData.Name = "tabData";
			this.tabData.Size = new System.Drawing.Size(832, 518);
			this.tabData.TabIndex = 1;
			this.tabData.Text = "SELECT";
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.SystemColors.Control;
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Location = new System.Drawing.Point(220, 20);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(556, 38);
			this.textBox1.TabIndex = 13;
			this.textBox1.Text = resources.GetString("textBox1.Text");
			// 
			// labelReferredFrom
			// 
			this.labelReferredFrom.Location = new System.Drawing.Point(438, 72);
			this.labelReferredFrom.Name = "labelReferredFrom";
			this.labelReferredFrom.Size = new System.Drawing.Size(170, 14);
			this.labelReferredFrom.TabIndex = 12;
			this.labelReferredFrom.Text = "Referred From";
			this.labelReferredFrom.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// listReferredFromSelect
			// 
			this.listReferredFromSelect.Location = new System.Drawing.Point(438, 86);
			this.listReferredFromSelect.Name = "listReferredFromSelect";
			this.listReferredFromSelect.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listReferredFromSelect.Size = new System.Drawing.Size(170, 420);
			this.listReferredFromSelect.TabIndex = 11;
			this.listReferredFromSelect.SelectedIndexChanged += new System.EventHandler(this.listReferredFromSelect_SelectedIndexChanged);
			// 
			// labelReferredTo
			// 
			this.labelReferredTo.Location = new System.Drawing.Point(220, 72);
			this.labelReferredTo.Name = "labelReferredTo";
			this.labelReferredTo.Size = new System.Drawing.Size(168, 14);
			this.labelReferredTo.TabIndex = 8;
			this.labelReferredTo.Text = "Referred To";
			this.labelReferredTo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// listReferredToSelect
			// 
			this.listReferredToSelect.Location = new System.Drawing.Point(220, 86);
			this.listReferredToSelect.Name = "listReferredToSelect";
			this.listReferredToSelect.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listReferredToSelect.Size = new System.Drawing.Size(168, 420);
			this.listReferredToSelect.TabIndex = 7;
			this.listReferredToSelect.SelectedIndexChanged += new System.EventHandler(this.listReferredToSelect_SelectedIndexChanged);
			// 
			// labelPatient
			// 
			this.labelPatient.Location = new System.Drawing.Point(8, 8);
			this.labelPatient.Name = "labelPatient";
			this.labelPatient.Size = new System.Drawing.Size(170, 14);
			this.labelPatient.TabIndex = 4;
			this.labelPatient.Text = "Patient";
			this.labelPatient.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// listPatientSelect
			// 
			this.listPatientSelect.Location = new System.Drawing.Point(8, 22);
			this.listPatientSelect.Name = "listPatientSelect";
			this.listPatientSelect.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listPatientSelect.Size = new System.Drawing.Size(170, 485);
			this.listPatientSelect.TabIndex = 3;
			this.listPatientSelect.SelectedIndexChanged += new System.EventHandler(this.listPatientSelect_SelectedIndexChanged);
			// 
			// tabFilters
			// 
			this.tabFilters.Controls.Add(this.labelHelp);
			this.tabFilters.Controls.Add(this.listBoxColumns);
			this.tabFilters.Controls.Add(this.TextDate);
			this.tabFilters.Controls.Add(this.TextValidAge);
			this.tabFilters.Controls.Add(this.butDeleteFilter);
			this.tabFilters.Controls.Add(this.listPrerequisites);
			this.tabFilters.Controls.Add(this.butAddFilter);
			this.tabFilters.Controls.Add(this.listConditions);
			this.tabFilters.Controls.Add(this.TextBox);
			this.tabFilters.Controls.Add(this.DropListFilter);
			this.tabFilters.Location = new System.Drawing.Point(4, 22);
			this.tabFilters.Name = "tabFilters";
			this.tabFilters.Size = new System.Drawing.Size(832, 518);
			this.tabFilters.TabIndex = 0;
			this.tabFilters.Text = "WHERE";
			// 
			// labelHelp
			// 
			this.labelHelp.Location = new System.Drawing.Point(360, 14);
			this.labelHelp.Name = "labelHelp";
			this.labelHelp.Size = new System.Drawing.Size(262, 18);
			this.labelHelp.TabIndex = 13;
			// 
			// listBoxColumns
			// 
			this.listBoxColumns.Location = new System.Drawing.Point(360, 40);
			this.listBoxColumns.Name = "listBoxColumns";
			this.listBoxColumns.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxColumns.Size = new System.Drawing.Size(262, 121);
			this.listBoxColumns.TabIndex = 12;
			this.listBoxColumns.Visible = false;
			this.listBoxColumns.SelectedIndexChanged += new System.EventHandler(this.listBoxColumns_SelectedIndexChanged);
			// 
			// TextDate
			// 
			this.TextDate.Location = new System.Drawing.Point(360, 40);
			this.TextDate.Name = "TextDate";
			this.TextDate.Size = new System.Drawing.Size(262, 20);
			this.TextDate.TabIndex = 11;
			// 
			// TextValidAge
			// 
			this.TextValidAge.Location = new System.Drawing.Point(360, 40);
			this.TextValidAge.Name = "TextValidAge";
			this.TextValidAge.ShowZero = false;
			this.TextValidAge.Size = new System.Drawing.Size(262, 20);
			this.TextValidAge.TabIndex = 10;
			this.TextValidAge.Visible = false;
			// 
			// butDeleteFilter
			// 
			this.butDeleteFilter.Enabled = false;
			this.butDeleteFilter.Image = ((System.Drawing.Image)(resources.GetObject("butDeleteFilter.Image")));
			this.butDeleteFilter.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteFilter.Location = new System.Drawing.Point(10, 420);
			this.butDeleteFilter.Name = "butDeleteFilter";
			this.butDeleteFilter.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.butDeleteFilter.Size = new System.Drawing.Size(108, 26);
			this.butDeleteFilter.TabIndex = 8;
			this.butDeleteFilter.Text = "      Delete Row";
			this.butDeleteFilter.Click += new System.EventHandler(this.butDeleteFilter_Click);
			// 
			// listPrerequisites
			// 
			this.listPrerequisites.Location = new System.Drawing.Point(10, 200);
			this.listPrerequisites.Name = "listPrerequisites";
			this.listPrerequisites.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listPrerequisites.Size = new System.Drawing.Size(608, 212);
			this.listPrerequisites.TabIndex = 7;
			this.listPrerequisites.SelectedIndexChanged += new System.EventHandler(this.listPrerequisites_SelectedIndexChanged);
			// 
			// butAddFilter
			// 
			this.butAddFilter.Enabled = false;
			this.butAddFilter.Location = new System.Drawing.Point(664, 40);
			this.butAddFilter.Name = "butAddFilter";
			this.butAddFilter.Size = new System.Drawing.Size(75, 24);
			this.butAddFilter.TabIndex = 6;
			this.butAddFilter.Text = "Add";
			this.butAddFilter.Click += new System.EventHandler(this.butAddFilter_Click);
			// 
			// listConditions
			// 
			this.listConditions.ItemStrings = new string[] {
        "LIKE",
        "=",
        ">",
        "<",
        ">=",
        "<=",
        "<>"};
			this.listConditions.Location = new System.Drawing.Point(232, 40);
			this.listConditions.Name = "listConditions";
			this.listConditions.Size = new System.Drawing.Size(78, 95);
			this.listConditions.TabIndex = 5;
			// 
			// TextBox
			// 
			this.TextBox.Location = new System.Drawing.Point(360, 40);
			this.TextBox.Name = "TextBox";
			this.TextBox.Size = new System.Drawing.Size(262, 20);
			this.TextBox.TabIndex = 2;
			this.TextBox.Visible = false;
			// 
			// DropListFilter
			// 
			this.DropListFilter.Location = new System.Drawing.Point(8, 40);
			this.DropListFilter.MaxDropDownItems = 45;
			this.DropListFilter.Name = "DropListFilter";
			this.DropListFilter.Size = new System.Drawing.Size(172, 21);
			this.DropListFilter.TabIndex = 1;
			this.DropListFilter.Text = "WHERE";
			this.DropListFilter.SelectedIndexChanged += new System.EventHandler(this.DropListFilter_SelectedIndexChanged);
			// 
			// TextSQL
			// 
			this.TextSQL.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TextSQL.Location = new System.Drawing.Point(18, 560);
			this.TextSQL.Multiline = true;
			this.TextSQL.Name = "TextSQL";
			this.TextSQL.ReadOnly = true;
			this.TextSQL.Size = new System.Drawing.Size(840, 128);
			this.TextSQL.TabIndex = 38;
			// 
			// FormRpPatients
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(962, 700);
			this.Controls.Add(this.tabPatients);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.TextSQL);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpPatients";
			this.ShowInTaskbar = false;
			this.Text = "Patients Report";
			this.tabPatients.ResumeLayout(false);
			this.tabData.ResumeLayout(false);
			this.tabData.PerformLayout();
			this.tabFilters.ResumeLayout(false);
			this.tabFilters.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.TabControl tabPatients;
		private System.Windows.Forms.TabPage tabFilters;
		private System.Windows.Forms.TabPage tabData;
		private OpenDental.UI.ListBoxOD listBoxColumns;
		private OpenDental.UI.ListBoxOD listPatientSelect;
		private OpenDental.UI.ListBoxOD listPrerequisites;
		private OpenDental.UI.ListBoxOD listReferredFromSelect;
		private OpenDental.UI.ListBoxOD listReferredToSelect;
		private OpenDental.UI.ListBoxOD listConditions;
		private System.Windows.Forms.ComboBox DropListFilter;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butAddFilter;
		private OpenDental.UI.Button butDeleteFilter;
		private System.Windows.Forms.TextBox TextSQL;
		private System.Windows.Forms.TextBox TextBox;
		private OpenDental.ValidNum TextValidAge;
		private OpenDental.ValidDate TextDate;
		private System.Windows.Forms.Label labelPatient;
		private System.Windows.Forms.Label labelReferredTo;
		private System.Windows.Forms.Label labelReferredFrom;
		private System.Windows.Forms.Label labelHelp;  //fields used in SELECT
		private System.Windows.Forms.TextBox textBox1;
	}
}
