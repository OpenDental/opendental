using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPatFieldCheckEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPatFieldCheckEdit));
			this.butSave = new OpenDental.UI.Button();
			this.checkFieldValue = new OpenDental.UI.CheckBox();
			this.labelName = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(185, 79);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 25);
			this.butSave.TabIndex = 1;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// checkFieldValue
			// 
			this.checkFieldValue.Location = new System.Drawing.Point(22, 40);
			this.checkFieldValue.Name = "checkFieldValue";
			this.checkFieldValue.Size = new System.Drawing.Size(16, 16);
			this.checkFieldValue.TabIndex = 4;
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(42, 34);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(229, 20);
			this.labelName.TabIndex = 3;
			this.labelName.Text = "Field Name";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormPatFieldCheckEdit
			// 
			this.AcceptButton = this.butSave;
			this.ClientSize = new System.Drawing.Size(272, 116);
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.Controls.Add(this.checkFieldValue);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPatFieldCheckEdit";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Patient Field Checkbox";
			this.Load += new System.EventHandler(this.FormPatFieldCheckEdit_Load);
			this.ResumeLayout(false);

		}
		#endregion
		private OpenDental.UI.Button butSave;
		private Label labelName;
		private OpenDental.UI.CheckBox checkFieldValue;
	}
}
