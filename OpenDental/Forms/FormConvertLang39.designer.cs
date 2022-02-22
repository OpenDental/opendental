using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormConvertLang39 {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConvertLang39));
			this.butOK = new OpenDental.UI.Button();
			this.textOldCode = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.listCulture = new OpenDental.UI.ListBoxOD();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(422, 457);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.UpdateLanguageCode);
			// 
			// textOldCode
			// 
			this.textOldCode.Location = new System.Drawing.Point(31, 37);
			this.textOldCode.Name = "textOldCode";
			this.textOldCode.Size = new System.Drawing.Size(344, 20);
			this.textOldCode.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(30, 12);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(152, 22);
			this.label2.TabIndex = 5;
			this.label2.Text = "Old neutral culture";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(30, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(189, 20);
			this.label3.TabIndex = 7;
			this.label3.Text = "New specific culture";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listCulture
			// 
			this.listCulture.Location = new System.Drawing.Point(31, 89);
			this.listCulture.Name = "listCulture";
			this.listCulture.Size = new System.Drawing.Size(345, 394);
			this.listCulture.TabIndex = 8;
			this.listCulture.DoubleClick += new System.EventHandler(this.UpdateLanguageCode);
			// 
			// FormConvertLang39
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(539, 509);
			this.Controls.Add(this.listCulture);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textOldCode);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormConvertLang39";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Convert Language Codes";
			this.Load += new System.EventHandler(this.FormConvertLang39_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.TextBox textOldCode;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.ListBoxOD listCulture;
	}
}
