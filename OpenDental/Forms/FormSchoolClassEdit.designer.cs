using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormSchoolClassEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSchoolClassEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textDescript = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.textGradYear = new OpenDental.ValidNum();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(411, 171);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(411, 130);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(199, 19);
			this.label1.TabIndex = 2;
			this.label1.Text = "Graduation Year";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescript
			// 
			this.textDescript.Location = new System.Drawing.Point(206, 49);
			this.textDescript.Name = "textDescript";
			this.textDescript.Size = new System.Drawing.Size(196, 20);
			this.textDescript.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(199, 19);
			this.label2.TabIndex = 4;
			this.label2.Text = "Description (Dental or Hygiene)";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(50, 170);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(95, 26);
			this.butDelete.TabIndex = 2;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textGradYear
			// 
			this.textGradYear.Location = new System.Drawing.Point(206, 20);
			this.textGradYear.MaxVal = 2075;
			this.textGradYear.MinVal = 1990;
			this.textGradYear.Name = "textGradYear";
			this.textGradYear.ShowZero = false;
			this.textGradYear.Size = new System.Drawing.Size(71, 20);
			this.textGradYear.TabIndex = 0;
			// 
			// FormSchoolClassEdit
			// 
			this.ClientSize = new System.Drawing.Size(538, 222);
			this.Controls.Add(this.textGradYear);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textDescript);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormSchoolClassEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Dental School Class";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSchoolClassEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormSchoolClassEdit_Load);
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
		private OpenDental.ValidNum textGradYear;
	}
}
