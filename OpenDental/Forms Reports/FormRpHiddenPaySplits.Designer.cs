using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDental {
	public partial class FormRpHiddenPaySplits {
		private System.ComponentModel.Container components = null;

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null) {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpHiddenPaySplits));
			this.butOK = new OpenDental.UI.Button();
			this.odDateRangePicker = new OpenDental.UI.ODDateRangePicker();
			this.checkAllProv = new OpenDental.UI.CheckBox();
			this.listBoxProv = new OpenDental.UI.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.listBoxClinic = new OpenDental.UI.ListBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.checkAllClinics = new OpenDental.UI.CheckBox();
			this.checkAllUnearnedTypes = new OpenDental.UI.CheckBox();
			this.listBoxUnearnedTypes = new OpenDental.UI.ListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(435, 299);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.ButOK_Click);
			// 
			// odDateRangePicker
			// 
			this.odDateRangePicker.BackColor = System.Drawing.Color.Transparent;
			this.odDateRangePicker.Location = new System.Drawing.Point(12, 12);
			this.odDateRangePicker.MinimumSize = new System.Drawing.Size(453, 22);
			this.odDateRangePicker.Name = "odDateRangePicker";
			this.odDateRangePicker.Size = new System.Drawing.Size(453, 24);
			this.odDateRangePicker.TabIndex = 6;
			// 
			// checkAllProv
			// 
			this.checkAllProv.Checked = true;
			this.checkAllProv.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllProv.Location = new System.Drawing.Point(13, 74);
			this.checkAllProv.Name = "checkAllProv";
			this.checkAllProv.Size = new System.Drawing.Size(159, 16);
			this.checkAllProv.TabIndex = 62;
			this.checkAllProv.Text = "All";
			this.checkAllProv.Click += new System.EventHandler(this.CheckAllProv_Click);
			// 
			// listBoxProv
			// 
			this.listBoxProv.Location = new System.Drawing.Point(12, 93);
			this.listBoxProv.Name = "listBoxProv";
			this.listBoxProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxProv.Size = new System.Drawing.Size(160, 199);
			this.listBoxProv.TabIndex = 61;
			this.listBoxProv.Click += new System.EventHandler(this.ListBoxProv_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 54);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 60;
			this.label1.Text = "Providers";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listBoxClinic
			// 
			this.listBoxClinic.Location = new System.Drawing.Point(347, 93);
			this.listBoxClinic.Name = "listBoxClinic";
			this.listBoxClinic.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxClinic.Size = new System.Drawing.Size(160, 199);
			this.listBoxClinic.TabIndex = 53;
			this.listBoxClinic.Visible = false;
			this.listBoxClinic.Click += new System.EventHandler(this.ListBoxClinic_Click);
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(344, 56);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(104, 16);
			this.labelClinic.TabIndex = 52;
			this.labelClinic.Text = "Clinics";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.labelClinic.Visible = false;
			// 
			// checkAllClinics
			// 
			this.checkAllClinics.Location = new System.Drawing.Point(347, 74);
			this.checkAllClinics.Name = "checkAllClinics";
			this.checkAllClinics.Size = new System.Drawing.Size(160, 16);
			this.checkAllClinics.TabIndex = 54;
			this.checkAllClinics.Text = "All (includes hidden)";
			this.checkAllClinics.Visible = false;
			this.checkAllClinics.Click += new System.EventHandler(this.CheckAllClinics_Click);
			// 
			// checkAllUnearnedTypes
			// 
			this.checkAllUnearnedTypes.Checked = true;
			this.checkAllUnearnedTypes.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllUnearnedTypes.Location = new System.Drawing.Point(178, 73);
			this.checkAllUnearnedTypes.Name = "checkAllUnearnedTypes";
			this.checkAllUnearnedTypes.Size = new System.Drawing.Size(163, 17);
			this.checkAllUnearnedTypes.TabIndex = 64;
			this.checkAllUnearnedTypes.Text = "All";
			this.checkAllUnearnedTypes.Click += new System.EventHandler(this.CheckAllUnearnedTypes_Click);
			// 
			// listBoxUnearnedTypes
			// 
			this.listBoxUnearnedTypes.Location = new System.Drawing.Point(178, 93);
			this.listBoxUnearnedTypes.Name = "listBoxUnearnedTypes";
			this.listBoxUnearnedTypes.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxUnearnedTypes.Size = new System.Drawing.Size(163, 199);
			this.listBoxUnearnedTypes.TabIndex = 63;
			this.listBoxUnearnedTypes.Click += new System.EventHandler(this.ListBoxUnearnedTypes_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(175, 54);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 16);
			this.label3.TabIndex = 65;
			this.label3.Text = "Unearned Types";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormRpHiddenPaySplits
			// 
			this.ClientSize = new System.Drawing.Size(522, 337);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.checkAllUnearnedTypes);
			this.Controls.Add(this.listBoxUnearnedTypes);
			this.Controls.Add(this.checkAllProv);
			this.Controls.Add(this.listBoxProv);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkAllClinics);
			this.Controls.Add(this.odDateRangePicker);
			this.Controls.Add(this.listBoxClinic);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpHiddenPaySplits";
			this.Text = "Hidden Payment Splits Report";
			this.Load += new System.EventHandler(this.FormRpTpPreAllocation_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.ODDateRangePicker odDateRangePicker;
		private OpenDental.UI.CheckBox checkAllProv;
		private OpenDental.UI.ListBox listBoxProv;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.ListBox listBoxClinic;
		private System.Windows.Forms.Label labelClinic;
		private OpenDental.UI.CheckBox checkAllClinics;
		private OpenDental.UI.CheckBox checkAllUnearnedTypes;
		private OpenDental.UI.ListBox listBoxUnearnedTypes;
		private System.Windows.Forms.Label label3;
	}
}
