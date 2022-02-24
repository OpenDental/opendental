using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public class FormTrojanHelp : FormODBase {
		private OpenDental.UI.Button butCancel;
		private RichTextBox textMain;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		///<summary></summary>
		public FormTrojanHelp()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

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
			this.butCancel = new OpenDental.UI.Button();
			this.textMain = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(530,386);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textMain
			// 
			this.textMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textMain.Location = new System.Drawing.Point(12,12);
			this.textMain.Name = "textMain";
			this.textMain.Size = new System.Drawing.Size(592,350);
			this.textMain.TabIndex = 1;
			this.textMain.Text = resources.GetString("textMain.Text");
			// 
			// FormTrojanHelp
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(617,424);
			this.Controls.Add(this.butCancel);
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

		private void butCancel_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormTrojanHelp_Load(object sender,EventArgs e) {
			textMain.Select(0,31);
			textMain.SelectionFont=new Font(Font,FontStyle.Bold);
			textMain.Select(323,20);
			textMain.SelectionFont=new Font(Font,FontStyle.Bold);
			textMain.Select(571,5);//skip
			textMain.SelectionFont=new Font(Font,FontStyle.Bold);
			textMain.Select(933,18);
			textMain.SelectionFont=new Font(Font,FontStyle.Bold);
			textMain.Select(1302,31);
			textMain.SelectionFont=new Font(Font,FontStyle.Bold);
			textMain.Select(1473,10);
			textMain.SelectionFont=new Font(Font,FontStyle.Bold);
		}


	}
}





















