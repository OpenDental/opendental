using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormLetterEdit {
		/// <summary>Required designer variable.</summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLetterEdit));
			this.butSave = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textBody = new OpenDental.ODtextBox();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(716, 425);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 26);
			this.butSave.TabIndex = 2;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(29,35);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100,14);
			this.label2.TabIndex = 3;
			this.label2.Text = "Description";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(132,30);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(346,20);
			this.textDescription.TabIndex = 0;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(43,56);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(87,79);
			this.label7.TabIndex = 7;
			this.label7.Text = "Body of Letter (do not include the address, greeting, or closing)";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textBody
			// 
			this.textBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
						| System.Windows.Forms.AnchorStyles.Left) 
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBody.Location = new System.Drawing.Point(132,56);
			this.textBody.Multiline = true;
			this.textBody.Name = "textBody";
			this.textBody.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.Letter;
			this.textBody.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textBody.Size = new System.Drawing.Size(659,363);
			this.textBody.TabIndex = 8;
			// 
			// FormLetterEdit
			// 
			this.ClientSize = new System.Drawing.Size(803, 463);
			this.Controls.Add(this.butSave);
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.Controls.Add(this.textBody);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormLetterEdit";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Letter";
			this.Load += new System.EventHandler(this.FormLetterEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label label7;
		private OpenDental.ODtextBox textBody;
	}
}
