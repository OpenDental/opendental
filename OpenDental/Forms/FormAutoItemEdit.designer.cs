using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormAutoItemEdit {
		/// <summary>
		/// Required designer variable.
		/// </summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutoItemEdit));
			this.textADA = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.listConditions = new OpenDental.UI.ListBoxOD();
			this.butChange = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textADA
			// 
			this.textADA.Location = new System.Drawing.Point(108,54);
			this.textADA.Name = "textADA";
			this.textADA.ReadOnly = true;
			this.textADA.Size = new System.Drawing.Size(100,20);
			this.textADA.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10,58);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96,12);
			this.label1.TabIndex = 1;
			this.label1.Text = "Code";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listConditions
			// 
			this.listConditions.Location = new System.Drawing.Point(334,56);
			this.listConditions.Name = "listConditions";
			this.listConditions.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listConditions.Size = new System.Drawing.Size(166,407);
			this.listConditions.TabIndex = 2;
			// 
			// butChange
			// 
			this.butChange.Location = new System.Drawing.Point(214,50);
			this.butChange.Name = "butChange";
			this.butChange.Size = new System.Drawing.Size(76,25);
			this.butChange.TabIndex = 24;
			this.butChange.Text = "C&hange";
			this.butChange.Click += new System.EventHandler(this.butChange_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(540,442);
			this.butCancel.Name = "butCancel";
			this.butCancel.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.butCancel.Size = new System.Drawing.Size(75,26);
			this.butCancel.TabIndex = 23;
			this.butCancel.Text = "&Cancel";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(540,408);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,26);
			this.butOK.TabIndex = 22;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(356,40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(118,14);
			this.label2.TabIndex = 25;
			this.label2.Text = "Conditions";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// FormAutoItemEdit
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(644,490);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butChange);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.listConditions);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textADA);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAutoItemEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit AutoItem";
			this.Load += new System.EventHandler(this.FormAutoItemEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textADA;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.ListBoxOD listConditions;
		private OpenDental.UI.Button butChange;
	}
}
