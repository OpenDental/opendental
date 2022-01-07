using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTerminalConnection {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTerminalConnection));
			this.label1 = new System.Windows.Forms.Label();
			this.textTx = new System.Windows.Forms.TextBox();
			this.textRx = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textProgress = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(25,0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100,20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Sent";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textTx
			// 
			this.textTx.Location = new System.Drawing.Point(25,22);
			this.textTx.Multiline = true;
			this.textTx.Name = "textTx";
			this.textTx.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textTx.Size = new System.Drawing.Size(452,28);
			this.textTx.TabIndex = 1;
			// 
			// textRx
			// 
			this.textRx.Location = new System.Drawing.Point(25,83);
			this.textRx.Multiline = true;
			this.textRx.Name = "textRx";
			this.textRx.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textRx.Size = new System.Drawing.Size(452,41);
			this.textRx.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(25,61);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100,20);
			this.label2.TabIndex = 2;
			this.label2.Text = "Received";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textProgress
			// 
			this.textProgress.Location = new System.Drawing.Point(25,157);
			this.textProgress.Multiline = true;
			this.textProgress.Name = "textProgress";
			this.textProgress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textProgress.Size = new System.Drawing.Size(452,418);
			this.textProgress.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(25,135);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100,20);
			this.label3.TabIndex = 5;
			this.label3.Text = "Progress";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormTerminal
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(637,612);
			this.Controls.Add(this.textProgress);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textRx);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textTx);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTerminal";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Modem Terminal";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormTerminal_Closing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textTx;
		private System.Windows.Forms.TextBox textRx;
		private System.Windows.Forms.TextBox textProgress;
		private System.Windows.Forms.Label label3;
	}
}
