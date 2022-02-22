using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormClaimCreate {
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose(bool disposing){
			if(disposing){
				if(components!=null){
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClaimCreate));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.labelRelat = new System.Windows.Forms.Label();
			this.listRelat = new OpenDental.UI.ListBoxOD();
			this.gridMain = new OpenDental.UI.GridOD();
			this.checkShowPlansNotInUse = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(649, 275);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(76, 26);
			this.butCancel.TabIndex = 6;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(556, 275);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(76, 26);
			this.butOK.TabIndex = 5;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelRelat
			// 
			this.labelRelat.Location = new System.Drawing.Point(563, 17);
			this.labelRelat.Name = "labelRelat";
			this.labelRelat.Size = new System.Drawing.Size(192, 20);
			this.labelRelat.TabIndex = 8;
			this.labelRelat.Text = "Relationship to Subscriber";
			this.labelRelat.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listRelat
			// 
			this.listRelat.Location = new System.Drawing.Point(565, 38);
			this.listRelat.Name = "listRelat";
			this.listRelat.Size = new System.Drawing.Size(180, 186);
			this.listRelat.TabIndex = 9;
			// 
			// gridMain
			// 
			this.gridMain.Location = new System.Drawing.Point(8, 38);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(551, 186);
			this.gridMain.TabIndex = 10;
			this.gridMain.Title = "Insurance Plans for Family";
			this.gridMain.TranslationName = "TableInsPlans";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// checkShowPlansNotInUse
			// 
			this.checkShowPlansNotInUse.Location = new System.Drawing.Point(8, 19);
			this.checkShowPlansNotInUse.Name = "checkShowPlansNotInUse";
			this.checkShowPlansNotInUse.Size = new System.Drawing.Size(329, 17);
			this.checkShowPlansNotInUse.TabIndex = 11;
			this.checkShowPlansNotInUse.Text = "Show plans for family which are not in use by the current patient.";
			this.checkShowPlansNotInUse.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.checkShowPlansNotInUse.UseVisualStyleBackColor = true;
			this.checkShowPlansNotInUse.Click += new System.EventHandler(this.checkPlansNotInUse_Click);
			// 
			// FormClaimCreate
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(755, 319);
			this.Controls.Add(this.checkShowPlansNotInUse);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.listRelat);
			this.Controls.Add(this.labelRelat);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormClaimCreate";
			this.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Create New Claim";
			this.Load += new System.EventHandler(this.FormClaimCreate_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.ListBoxOD listRelat;
		private System.Windows.Forms.Label labelRelat;
		private OpenDental.UI.GridOD gridMain;
		private CheckBox checkShowPlansNotInUse;
	}
}
