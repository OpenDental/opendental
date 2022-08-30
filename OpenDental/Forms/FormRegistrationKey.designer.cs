using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRegistrationKey {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components=null;

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRegistrationKey));
			this.labelRegKey = new System.Windows.Forms.Label();
			this.textKey1 = new System.Windows.Forms.TextBox();
			this.labelLicenseAgree = new System.Windows.Forms.Label();
			this.checkAgree = new System.Windows.Forms.CheckBox();
			this.richTextAgreement = new System.Windows.Forms.RichTextBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.listBoxRegistration = new OpenDental.UI.ListBoxOD();
			this.SuspendLayout();
			// 
			// labelRegKey
			// 
			this.labelRegKey.Location = new System.Drawing.Point(168, 9);
			this.labelRegKey.Name = "labelRegKey";
			this.labelRegKey.Size = new System.Drawing.Size(177, 19);
			this.labelRegKey.TabIndex = 2;
			this.labelRegKey.Text = "Registration Key";
			this.labelRegKey.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textKey1
			// 
			this.textKey1.Location = new System.Drawing.Point(349, 8);
			this.textKey1.Name = "textKey1";
			this.textKey1.Size = new System.Drawing.Size(243, 20);
			this.textKey1.TabIndex = 0;
			this.textKey1.WordWrap = false;
			this.textKey1.TextChanged += new System.EventHandler(this.textKey1_TextChanged);
			this.textKey1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textKey1_KeyUp);
			// 
			// labelLicenseAgree
			// 
			this.labelLicenseAgree.Location = new System.Drawing.Point(12, 26);
			this.labelLicenseAgree.Name = "labelLicenseAgree";
			this.labelLicenseAgree.Size = new System.Drawing.Size(150,13);
			this.labelLicenseAgree.TabIndex = 6;
			this.labelLicenseAgree.Text = "License Agreement";
			this.labelLicenseAgree.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkAgree
			// 
			this.checkAgree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkAgree.Location = new System.Drawing.Point(12, 373);
			this.checkAgree.Name = "checkAgree";
			this.checkAgree.Size = new System.Drawing.Size(373,17);
			this.checkAgree.TabIndex = 7;
			this.checkAgree.Text = "I agree to the terms of all of the above license agreements in their entirety.";
			this.checkAgree.UseVisualStyleBackColor = true;
			this.checkAgree.CheckedChanged += new System.EventHandler(this.checkAgree_CheckedChanged);
			// 
			// richTextAgreement
			// 
			this.richTextAgreement.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.richTextAgreement.Location = new System.Drawing.Point(168, 45);
			this.richTextAgreement.Name = "richTextAgreement";
			this.richTextAgreement.Size = new System.Drawing.Size(583, 316);
			this.richTextAgreement.TabIndex = 8;
			this.richTextAgreement.Text = "";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Enabled = false;
			this.butOK.Location = new System.Drawing.Point(597, 367);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,26);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(678, 367);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// listBoxRegistration
			// 
			this.listBoxRegistration.Location = new System.Drawing.Point(12, 45);
			this.listBoxRegistration.Name = "listBoxRegistration";
			this.listBoxRegistration.Size = new System.Drawing.Size(150, 316);
			this.listBoxRegistration.TabIndex = 9;
			this.listBoxRegistration.SelectedIndexChanged += new System.EventHandler(this.listRegistration_SelectedIndexChanged);
			// 
			// FormRegistrationKey
			// 
			this.ClientSize = new System.Drawing.Size(763, 399);
			this.Controls.Add(this.listBoxRegistration);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.richTextAgreement);
			this.Controls.Add(this.checkAgree);
			this.Controls.Add(this.labelLicenseAgree);
			this.Controls.Add(this.textKey1);
			this.Controls.Add(this.labelRegKey);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRegistrationKey";
			this.ShowInTaskbar = false;
			this.Text = "Registration Key";
			this.Load += new System.EventHandler(this.FormRegistrationKey_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label labelRegKey;
		private TextBox textKey1;
		private Label labelLicenseAgree;
		private CheckBox checkAgree;
		private OpenDental.UI.ListBoxOD listBoxRegistration;
		private RichTextBox richTextAgreement;
	}
}
