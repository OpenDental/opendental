using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpProcCodes {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpProcCodes));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.listBoxFeeSched = new OpenDental.UI.ListBoxOD();
			this.radioCategories = new System.Windows.Forms.RadioButton();
			this.radioCode = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.listBoxClinics = new OpenDental.UI.ListBoxOD();
			this.listBoxProviders = new OpenDental.UI.ListBoxOD();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.checkShowBlankFees = new System.Windows.Forms.CheckBox();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.groupBox5.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(337, 276);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(337, 241);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// listBoxFeeSched
			// 
			this.listBoxFeeSched.Location = new System.Drawing.Point(12, 42);
			this.listBoxFeeSched.Name = "listBoxFeeSched";
			this.listBoxFeeSched.Size = new System.Drawing.Size(129, 173);
			this.listBoxFeeSched.TabIndex = 0;
			// 
			// radioCategories
			// 
			this.radioCategories.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioCategories.Location = new System.Drawing.Point(6, 39);
			this.radioCategories.Name = "radioCategories";
			this.radioCategories.Size = new System.Drawing.Size(88, 24);
			this.radioCategories.TabIndex = 1;
			this.radioCategories.Text = "Categories";
			// 
			// radioCode
			// 
			this.radioCode.Checked = true;
			this.radioCode.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioCode.Location = new System.Drawing.Point(6, 15);
			this.radioCode.Name = "radioCode";
			this.radioCode.Size = new System.Drawing.Size(88, 24);
			this.radioCode.TabIndex = 0;
			this.radioCode.TabStop = true;
			this.radioCode.Text = "Code";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(9, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(132, 22);
			this.label1.TabIndex = 4;
			this.label1.Text = "Fee Schedule";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listBoxClinics
			// 
			this.listBoxClinics.Location = new System.Drawing.Point(147, 42);
			this.listBoxClinics.Name = "listBoxClinics";
			this.listBoxClinics.Size = new System.Drawing.Size(129, 173);
			this.listBoxClinics.TabIndex = 5;
			// 
			// listBoxProviders
			// 
			this.listBoxProviders.Location = new System.Drawing.Point(283, 42);
			this.listBoxProviders.Name = "listBoxProviders";
			this.listBoxProviders.Size = new System.Drawing.Size(129, 173);
			this.listBoxProviders.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(144, 17);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(132, 22);
			this.label2.TabIndex = 7;
			this.label2.Text = "Clinic";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(280, 17);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(132, 22);
			this.label3.TabIndex = 8;
			this.label3.Text = "Provider";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkShowBlankFees
			// 
			this.checkShowBlankFees.Location = new System.Drawing.Point(45, 246);
			this.checkShowBlankFees.Name = "checkShowBlankFees";
			this.checkShowBlankFees.Size = new System.Drawing.Size(166, 24);
			this.checkShowBlankFees.TabIndex = 9;
			this.checkShowBlankFees.Text = "Show blank fees";
			this.checkShowBlankFees.UseVisualStyleBackColor = true;
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.radioCategories);
			this.groupBox5.Controls.Add(this.radioCode);
			this.groupBox5.Location = new System.Drawing.Point(217, 231);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(114, 71);
			this.groupBox5.TabIndex = 31;
			this.groupBox5.TabStop = false;
			// 
			// FormRpProcCodes
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(423, 314);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.checkShowBlankFees);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listBoxProviders);
			this.Controls.Add(this.listBoxClinics);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listBoxFeeSched);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpProcCodes";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Procedure Codes Report";
			this.Load += new System.EventHandler(this.FormRpProcCodes_Load);
			this.groupBox5.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.ListBoxOD listBoxFeeSched;
		private System.Windows.Forms.RadioButton radioCategories;
		private System.Windows.Forms.RadioButton radioCode;
		private Label label1;
		private OpenDental.UI.ListBoxOD listBoxClinics;
		private OpenDental.UI.ListBoxOD listBoxProviders;
		private Label label2;
		private Label label3;
		private CheckBox checkShowBlankFees;
		private GroupBox groupBox5;
	}
}
