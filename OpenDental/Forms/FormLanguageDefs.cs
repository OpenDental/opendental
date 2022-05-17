using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public class FormLanguageDefs : System.Windows.Forms.Form{
		private OpenDental.UI.Button butClose;
		private ListBox listAvailable;
		private Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		///<summary></summary>
		public FormLanguageDefs()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			Lan.F(this);
		}

		///<summary>Clean up any resources being used.</summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLanguageDefs));
			this.butClose = new OpenDental.UI.Button();
			this.listAvailable = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.AdjustImageLocation = new System.Drawing.Point(0,0);
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Autosize = true;
			this.butClose.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butClose.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butClose.Location = new System.Drawing.Point(592,513);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75,26);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// listAvailable
			// 
			this.listAvailable.FormattingEnabled = true;
			this.listAvailable.Location = new System.Drawing.Point(45,33);
			this.listAvailable.Name = "listAvailable";
			this.listAvailable.Size = new System.Drawing.Size(278,498);
			this.listAvailable.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(42,9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100,23);
			this.label1.TabIndex = 2;
			this.label1.Text = "Available Languages";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormLanguageDefs
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(719,564);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listAvailable);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormLanguageDefs";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Language Definitions";
			this.Load += new System.EventHandler(this.FormLanguageDefs_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormLanguageDefs_Load(object sender,EventArgs e) {
			//for(int i=0;i<CultureInfo.GetCultures(CultureTypes.AllCultures ;i++){
			//another change
			//change #2
			//}
		}
		
		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		


	}
}





















