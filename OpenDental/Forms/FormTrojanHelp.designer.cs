using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTrojanHelp {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTrojanHelp));
			this.textMain = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// textMain
			// 
			this.textMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textMain.Location = new System.Drawing.Point(12, 12);
			this.textMain.Name = "textMain";
			this.textMain.Size = new System.Drawing.Size(592, 400);
			this.textMain.TabIndex = 1;
			this.textMain.Text = resources.GetString("textMain.Text");
			// 
			// FormTrojanHelp
			// 
			this.ClientSize = new System.Drawing.Size(617, 424);
			this.Controls.Add(this.textMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTrojanHelp";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Help";
			this.Load += new System.EventHandler(this.FormTrojanHelp_Load);
			this.ResumeLayout(false);

		}
		#endregion
		private RichTextBox textMain;
	}
}
