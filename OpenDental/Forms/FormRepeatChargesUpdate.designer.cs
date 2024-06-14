using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRepeatChargesUpdate {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRepeatChargesUpdate));
			this.butOK = new OpenDental.UI.Button();
			this.checkRunAging = new OpenDental.UI.CheckBox();
			this.labelDescription = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(433, 201);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// checkRunAging
			// 
			this.checkRunAging.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkRunAging.Location = new System.Drawing.Point(12, 210);
			this.checkRunAging.Name = "checkRunAging";
			this.checkRunAging.Size = new System.Drawing.Size(290, 17);
			this.checkRunAging.TabIndex = 4;
			this.checkRunAging.Text = "Run aging on accounts after posting charges.";
			this.checkRunAging.CheckedChanged += new System.EventHandler(this.checkRunAging_CheckedChanged);
			// 
			// labelDescription
			// 
			this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDescription.Location = new System.Drawing.Point(12, 13);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(496, 182);
			this.labelDescription.TabIndex = 5;
			this.labelDescription.Text = resources.GetString("labelDescription.Text");
			// 
			// FormRepeatChargesUpdate
			// 
			this.ClientSize = new System.Drawing.Size(520, 243);
			this.Controls.Add(this.labelDescription);
			this.Controls.Add(this.checkRunAging);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRepeatChargesUpdate";
			this.ShowInTaskbar = false;
			this.Text = "Update Repeating Charges";
			this.Load += new System.EventHandler(this.FormRepeatChargesUpdate_Load);
			this.ResumeLayout(false);

		}
		#endregion
		private UI.Button butOK;
		private OpenDental.UI.CheckBox checkRunAging;
		private Label labelDescription;
	}
}
