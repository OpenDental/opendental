using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPatFieldEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPatFieldEdit));
			this.butSave = new OpenDental.UI.Button();
			this.labelName = new System.Windows.Forms.Label();
			this.textValue = new System.Windows.Forms.TextBox();
			this.butUseAutoNote = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(279, 137);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 1;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(19, 17);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(335, 20);
			this.labelName.TabIndex = 3;
			this.labelName.Text = "Field Name";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textValue
			// 
			this.textValue.Location = new System.Drawing.Point(21, 40);
			this.textValue.Multiline = true;
			this.textValue.Name = "textValue";
			this.textValue.Size = new System.Drawing.Size(333, 74);
			this.textValue.TabIndex = 0;
			// 
			// butUseAutoNote
			// 
			this.butUseAutoNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butUseAutoNote.Location = new System.Drawing.Point(21, 137);
			this.butUseAutoNote.Name = "butUseAutoNote";
			this.butUseAutoNote.Size = new System.Drawing.Size(75, 24);
			this.butUseAutoNote.TabIndex = 4;
			this.butUseAutoNote.Text = "Auto Note";
			this.butUseAutoNote.Visible = false;
			this.butUseAutoNote.Click += new System.EventHandler(this.butUseAutoNote_Click);
			// 
			// FormPatFieldEdit
			// 
			this.AcceptButton = this.butSave;
			this.ClientSize = new System.Drawing.Size(375, 173);
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.Controls.Add(this.butUseAutoNote);
			this.Controls.Add(this.textValue);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPatFieldEdit";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Patient Field";
			this.Load += new System.EventHandler(this.FormPatFieldEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		private OpenDental.UI.Button butSave;
		private Label labelName;
		private UI.Button butUseAutoNote;
		private TextBox textValue;
	}
}
