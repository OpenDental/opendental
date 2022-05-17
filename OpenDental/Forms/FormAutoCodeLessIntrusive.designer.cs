using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormAutoCodeLessIntrusive {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutoCodeLessIntrusive));
			this.butNo = new OpenDental.UI.Button();
			this.butYes = new OpenDental.UI.Button();
			this.labelMain = new System.Windows.Forms.Label();
			this.labelPrompt = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butNo
			// 
			this.butNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butNo.Location = new System.Drawing.Point(406, 169);
			this.butNo.Name = "butNo";
			this.butNo.Size = new System.Drawing.Size(75, 26);
			this.butNo.TabIndex = 0;
			this.butNo.Text = "&No";
			this.butNo.Click += new System.EventHandler(this.butNo_Click);
			// 
			// butYes
			// 
			this.butYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butYes.Location = new System.Drawing.Point(406, 128);
			this.butYes.Name = "butYes";
			this.butYes.Size = new System.Drawing.Size(75, 26);
			this.butYes.TabIndex = 1;
			this.butYes.Text = "&Yes";
			this.butYes.Click += new System.EventHandler(this.butYes_Click);
			// 
			// labelMain
			// 
			this.labelMain.Location = new System.Drawing.Point(35, 32);
			this.labelMain.Name = "labelMain";
			this.labelMain.Size = new System.Drawing.Size(438, 73);
			this.labelMain.TabIndex = 3;
			this.labelMain.Text = "labelMain";
			this.labelMain.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// labelPrompt
			// 
			this.labelPrompt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelPrompt.Location = new System.Drawing.Point(12, 152);
			this.labelPrompt.Name = "labelPrompt";
			this.labelPrompt.Size = new System.Drawing.Size(388, 43);
			this.labelPrompt.TabIndex = 4;
			this.labelPrompt.Text = "If you don\'t want to be prompted to change this type of procedure in the future, " +
    "then edit this Auto Code and check the box for \"Do not check codes...\"";
			// 
			// FormAutoCodeLessIntrusive
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(511, 211);
			this.Controls.Add(this.butYes);
			this.Controls.Add(this.butNo);
			this.Controls.Add(this.labelPrompt);
			this.Controls.Add(this.labelMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAutoCodeLessIntrusive";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Change Code?";
			this.Load += new System.EventHandler(this.FormAutoCodeLessIntrusive_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Label labelMain;
		private OpenDental.UI.Button butNo;
		private OpenDental.UI.Button butYes;
		private Label labelPrompt;
	}
}
