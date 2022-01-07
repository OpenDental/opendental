using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormFeeSchedEdit {
		///<summary>Required designer variable.</summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFeeSchedEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.listType = new OpenDental.UI.ListBoxOD();
			this.checkIsHidden = new System.Windows.Forms.CheckBox();
			this.checkIsGlobal = new System.Windows.Forms.CheckBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(9, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(148, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(160, 20);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(291, 20);
			this.textDescription.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 46);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(148, 17);
			this.label2.TabIndex = 11;
			this.label2.Text = "Type";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listType
			// 
			this.listType.Location = new System.Drawing.Point(160, 46);
			this.listType.Name = "listType";
			this.listType.Size = new System.Drawing.Size(120, 69);
			this.listType.TabIndex = 12;
			// 
			// checkIsHidden
			// 
			this.checkIsHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHidden.Location = new System.Drawing.Point(176, 120);
			this.checkIsHidden.Name = "checkIsHidden";
			this.checkIsHidden.Size = new System.Drawing.Size(104, 19);
			this.checkIsHidden.TabIndex = 13;
			this.checkIsHidden.Text = "Hidden";
			this.checkIsHidden.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHidden.UseVisualStyleBackColor = true;
			this.checkIsHidden.Click += new System.EventHandler(this.checkIsHidden_Click);
			// 
			// checkIsGlobal
			// 
			this.checkIsGlobal.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsGlobal.Enabled = false;
			this.checkIsGlobal.Location = new System.Drawing.Point(112, 140);
			this.checkIsGlobal.Name = "checkIsGlobal";
			this.checkIsGlobal.Size = new System.Drawing.Size(168, 19);
			this.checkIsGlobal.TabIndex = 14;
			this.checkIsGlobal.Text = "Use Global Fees";
			this.checkIsGlobal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsGlobal.UseVisualStyleBackColor = true;
			this.checkIsGlobal.Click += new System.EventHandler(this.checkIsGlobal_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(309, 170);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 9;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(400, 170);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 10;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormFeeSchedEdit
			// 
			this.ClientSize = new System.Drawing.Size(501, 214);
			this.Controls.Add(this.checkIsGlobal);
			this.Controls.Add(this.checkIsHidden);
			this.Controls.Add(this.listType);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormFeeSchedEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Fee Schedule";
			this.Load += new System.EventHandler(this.FormFeeSchedEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textDescription;
		private Label label2;
		private OpenDental.UI.ListBoxOD listType;
		private CheckBox checkIsHidden;
		private CheckBox checkIsGlobal;
	}
}
