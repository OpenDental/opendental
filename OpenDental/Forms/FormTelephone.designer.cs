using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTelephone {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTelephone));
			this.butClose = new OpenDental.UI.Button();
			this.butReformat = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(509,266);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75,26);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butReformat
			// 
			this.butReformat.Location = new System.Drawing.Point(17,31);
			this.butReformat.Name = "butReformat";
			this.butReformat.Size = new System.Drawing.Size(108,26);
			this.butReformat.TabIndex = 1;
			this.butReformat.Text = "&Reformat";
			this.butReformat.Click += new System.EventHandler(this.butReformat_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(137,33);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(478,57);
			this.label1.TabIndex = 2;
			this.label1.Text = "Reformat all phone numbers in the database to (###)###-####.  Only certain matche" +
    "s will be reformatted.  No numbers will be lost, and no trailing comments will b" +
    "e affected.";
			// 
			// FormTelephone
			// 
			this.AcceptButton = this.butClose;
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(642,313);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butReformat);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTelephone";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Telephone Tools";
			this.Load += new System.EventHandler(this.FormTelephone_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butReformat;
		private System.Windows.Forms.Label label1;
	}
}
