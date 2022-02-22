using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpConfirm {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpConfirm));
			this.labelPatient = new System.Windows.Forms.Label();
			this.listSelect = new OpenDental.UI.ListBoxOD();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.listSelect2 = new OpenDental.UI.ListBoxOD();
			this.SuspendLayout();
			// 
			// labelPatient
			// 
			this.labelPatient.Location = new System.Drawing.Point(12,16);
			this.labelPatient.Name = "labelPatient";
			this.labelPatient.Size = new System.Drawing.Size(170,14);
			this.labelPatient.TabIndex = 6;
			this.labelPatient.Text = "Standard Fields";
			this.labelPatient.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// listSelect
			// 
			this.listSelect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listSelect.Location = new System.Drawing.Point(12,30);
			this.listSelect.Name = "listSelect";
			this.listSelect.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listSelect.Size = new System.Drawing.Size(170,355);
			this.listSelect.TabIndex = 5;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(434,362);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,26);
			this.butCancel.TabIndex = 8;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(434,326);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,26);
			this.butOK.TabIndex = 7;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(208,16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(170,14);
			this.label1.TabIndex = 10;
			this.label1.Text = "Other Available Fields";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// listSelect2
			// 
			this.listSelect2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listSelect2.Location = new System.Drawing.Point(208,30);
			this.listSelect2.Name = "listSelect2";
			this.listSelect2.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listSelect2.Size = new System.Drawing.Size(170,355);
			this.listSelect2.TabIndex = 9;
			// 
			// FormRpConfirm
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(538,408);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listSelect2);
			this.Controls.Add(this.labelPatient);
			this.Controls.Add(this.listSelect);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpConfirm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Confirmation Report";
			this.Load += new System.EventHandler(this.FormRpConfirm_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Label labelPatient;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.ListBoxOD listSelect;
		private OpenDental.UI.ListBoxOD listSelect2;
	}
}
