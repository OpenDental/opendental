using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormSchoolCourseEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSchoolCourseEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textDescript = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.textCourseID = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(474, 162);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(474, 121);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(11, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(155, 19);
			this.label1.TabIndex = 2;
			this.label1.Text = "Course ID";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescript
			// 
			this.textDescript.Location = new System.Drawing.Point(168, 48);
			this.textDescript.Name = "textDescript";
			this.textDescript.Size = new System.Drawing.Size(381, 20);
			this.textDescript.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(11, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(155, 19);
			this.label2.TabIndex = 4;
			this.label2.Text = "Description";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(50, 161);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(95, 26);
			this.butDelete.TabIndex = 2;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textCourseID
			// 
			this.textCourseID.Location = new System.Drawing.Point(168, 19);
			this.textCourseID.Name = "textCourseID";
			this.textCourseID.Size = new System.Drawing.Size(144, 20);
			this.textCourseID.TabIndex = 0;
			// 
			// FormSchoolCourseEdit
			// 
			this.ClientSize = new System.Drawing.Size(601, 213);
			this.Controls.Add(this.textCourseID);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textDescript);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormSchoolCourseEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Dental School Course";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSchoolCourseEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormSchoolCourseEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.TextBox textDescript;
		private System.Windows.Forms.TextBox textCourseID;
	}
}
