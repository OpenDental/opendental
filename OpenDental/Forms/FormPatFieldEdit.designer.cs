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
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.labelName = new System.Windows.Forms.Label();
			this.textValue = new System.Windows.Forms.TextBox();
			this.butUseAutoNote = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(280, 147);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(186, 147);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
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
			this.butUseAutoNote.Location = new System.Drawing.Point(21, 147);
			this.butUseAutoNote.Name = "butUseAutoNote";
			this.butUseAutoNote.Size = new System.Drawing.Size(75, 24);
			this.butUseAutoNote.TabIndex = 4;
			this.butUseAutoNote.Text = "Auto Note";
			this.butUseAutoNote.Visible = false;
			this.butUseAutoNote.Click += new System.EventHandler(this.butUseAutoNote_Click);
			// 
			// FormPatFieldEdit
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(375, 193);
			this.Controls.Add(this.butUseAutoNote);
			this.Controls.Add(this.textValue);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPatFieldEdit";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Patient Field";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPatFieldDefEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormPatFieldEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label labelName;
		private UI.Button butUseAutoNote;
		private TextBox textValue;
	}
}
