using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormReqNeededEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReqNeededEdit));
			this.butSave = new OpenDental.UI.Button();
			this.textDescript = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(474, 69);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 26);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// textDescript
			// 
			this.textDescript.Location = new System.Drawing.Point(168, 25);
			this.textDescript.Name = "textDescript";
			this.textDescript.Size = new System.Drawing.Size(381, 20);
			this.textDescript.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(11, 25);
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
			this.butDelete.Location = new System.Drawing.Point(14, 69);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(95, 26);
			this.butDelete.TabIndex = 2;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// FormReqNeededEdit
			// 
			this.ClientSize = new System.Drawing.Size(561, 107);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textDescript);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.label2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormReqNeededEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Requirement";
			this.Load += new System.EventHandler(this.FormReqNeededEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.TextBox textDescript;
	}
}
