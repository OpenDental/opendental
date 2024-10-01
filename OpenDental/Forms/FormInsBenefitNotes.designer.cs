using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormInsBenefitNotes {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInsBenefitNotes));
			this.butSave = new OpenDental.UI.Button();
			this.textBenefitNotes = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(466, 618);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 26);
			this.butSave.TabIndex = 1;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// textBenefitNotes
			// 
			this.textBenefitNotes.AcceptsReturn = true;
			this.textBenefitNotes.AcceptsTab = true;
			this.textBenefitNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBenefitNotes.Location = new System.Drawing.Point(10,12);
			this.textBenefitNotes.Multiline = true;
			this.textBenefitNotes.Name = "textBenefitNotes";
			this.textBenefitNotes.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBenefitNotes.Size = new System.Drawing.Size(450,632);
			this.textBenefitNotes.TabIndex = 2;
			// 
			// FormInsBenefitNotes
			// 
			this.Controls.Add(this.butSave);
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(561,660);
			this.Controls.Add(this.textBenefitNotes);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormInsBenefitNotes";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Benefit Notes";
			this.Load += new System.EventHandler(this.FormInsBenefitNotes_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.TextBox textBenefitNotes;
	}
}
