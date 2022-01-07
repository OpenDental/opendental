using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormInstructorEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInstructorEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textLName = new System.Windows.Forms.TextBox();
			this.textFName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textSuffix = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(417, 209);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(417, 168);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 43);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(199, 19);
			this.label1.TabIndex = 2;
			this.label1.Text = "Last Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLName
			// 
			this.textLName.Location = new System.Drawing.Point(208, 43);
			this.textLName.Name = "textLName";
			this.textLName.Size = new System.Drawing.Size(196, 20);
			this.textLName.TabIndex = 0;
			// 
			// textFName
			// 
			this.textFName.Location = new System.Drawing.Point(208, 77);
			this.textFName.Name = "textFName";
			this.textFName.Size = new System.Drawing.Size(196, 20);
			this.textFName.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(7, 77);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(199, 19);
			this.label2.TabIndex = 4;
			this.label2.Text = "First Name";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSuffix
			// 
			this.textSuffix.Location = new System.Drawing.Point(208, 110);
			this.textSuffix.Name = "textSuffix";
			this.textSuffix.Size = new System.Drawing.Size(76, 20);
			this.textSuffix.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(7, 110);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(199, 19);
			this.label3.TabIndex = 6;
			this.label3.Text = "Suffix (DMD, RDH)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(50, 208);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(95, 26);
			this.butDelete.TabIndex = 5;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// FormInstructorEdit
			// 
			this.ClientSize = new System.Drawing.Size(544, 260);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textSuffix);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textFName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textLName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormInstructorEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Instructor";
			this.Load += new System.EventHandler(this.FormInstructorEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textLName;
		private System.Windows.Forms.TextBox textFName;
		private System.Windows.Forms.TextBox textSuffix;
		private OpenDental.UI.Button butDelete;
	}
}
