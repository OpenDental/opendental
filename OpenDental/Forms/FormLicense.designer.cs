using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormLicense {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLicense));
			this.butClose = new OpenDental.UI.Button();
			this.listBoxLicense = new OpenDental.UI.ListBoxOD();
			this.textLicense = new System.Windows.Forms.RichTextBox();
			this.labelLicense = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(764, 482);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// listBoxLicense
			// 
			this.listBoxLicense.Location = new System.Drawing.Point(12, 55);
			this.listBoxLicense.Name = "listBoxLicense";
			this.listBoxLicense.Size = new System.Drawing.Size(144, 316);
			this.listBoxLicense.TabIndex = 20;
			this.listBoxLicense.SelectedIndexChanged += new System.EventHandler(this.listLicense_SelectedIndexChanged);
			// 
			// textLicense
			// 
			this.textLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textLicense.Location = new System.Drawing.Point(162, 35);
			this.textLicense.Name = "textLicense";
			this.textLicense.Size = new System.Drawing.Size(662, 425);
			this.textLicense.TabIndex = 21;
			this.textLicense.Text = "";
			// 
			// labelLicense
			// 
			this.labelLicense.Location = new System.Drawing.Point(10, 35);
			this.labelLicense.Name = "labelLicense";
			this.labelLicense.Size = new System.Drawing.Size(147, 17);
			this.labelLicense.TabIndex = 22;
			this.labelLicense.Text = "Select License:";
			this.labelLicense.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormLicense
			// 
			this.ClientSize = new System.Drawing.Size(851, 520);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.labelLicense);
			this.Controls.Add(this.textLicense);
			this.Controls.Add(this.listBoxLicense);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormLicense";
			this.ShowInTaskbar = false;
			this.Text = "Licenses";
			this.Load += new System.EventHandler(this.FormLicense_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.ListBoxOD listBoxLicense;
		private RichTextBox textLicense;
		private Label labelLicense;
	}
}
