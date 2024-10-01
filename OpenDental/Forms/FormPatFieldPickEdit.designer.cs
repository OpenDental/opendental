using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPatFieldPickEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPatFieldPickEdit));
			this.butSave = new OpenDental.UI.Button();
			this.labelName = new System.Windows.Forms.Label();
			this.listBoxPick = new OpenDental.UI.ListBox();
			this.butClear = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(279, 501);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 25);
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
			// listBoxPick
			// 
			this.listBoxPick.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxPick.Location = new System.Drawing.Point(21, 41);
			this.listBoxPick.Name = "listBoxPick";
			this.listBoxPick.Size = new System.Drawing.Size(333, 446);
			this.listBoxPick.TabIndex = 4;
			// 
			// butClear
			// 
			this.butClear.Location = new System.Drawing.Point(21, 501);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(75, 24);
			this.butClear.TabIndex = 5;
			this.butClear.Text = "Clear";
			this.butClear.UseVisualStyleBackColor = true;
			this.butClear.Click += new System.EventHandler(this.butClear_Click);
			// 
			// FormPatFieldPickEdit
			// 
			this.AcceptButton = this.butSave;
			this.ClientSize = new System.Drawing.Size(375, 538);
			this.Controls.Add(this.butClear);
			this.Controls.Add(this.listBoxPick);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPatFieldPickEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Patient Field Pick List";
			this.Load += new System.EventHandler(this.FormPatFieldPickEdit_Load);
			this.ResumeLayout(false);

		}
		#endregion
		private OpenDental.UI.Button butSave;
		private Label labelName;
		private OpenDental.UI.ListBox listBoxPick;
		private UI.Button butClear;
	}
}
