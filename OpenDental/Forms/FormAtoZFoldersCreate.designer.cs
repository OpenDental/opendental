using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormAtoZFoldersCreate {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAtoZFoldersCreate));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textLocation = new System.Windows.Forms.TextBox();
			this.textName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(592,222);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(592,181);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24,23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(578,76);
			this.label1.TabIndex = 2;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(26,115);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(184,18);
			this.label2.TabIndex = 3;
			this.label2.Text = "Location of new folder";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textLocation
			// 
			this.textLocation.Location = new System.Drawing.Point(214,112);
			this.textLocation.Name = "textLocation";
			this.textLocation.Size = new System.Drawing.Size(323,20);
			this.textLocation.TabIndex = 4;
			this.textLocation.Text = "C:\\";
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(214,148);
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(206,20);
			this.textName.TabIndex = 6;
			this.textName.Text = "OpenDentImages";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(26,151);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(184,18);
			this.label3.TabIndex = 5;
			this.label3.Text = "Folder name";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// FormAtoZFoldersCreate
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(719,273);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textLocation);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAtoZFoldersCreate";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Create AtoZ Folder";
			this.Load += new System.EventHandler(this.FormAtoZFoldersCreate_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label label1;
		private Label label2;
		private TextBox textLocation;
		private TextBox textName;
		private Label label3;
	}
}
