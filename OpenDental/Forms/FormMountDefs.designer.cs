using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormMountDefs {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMountDefs));
			this.listBoxMain = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.butAdd = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// listBoxMain
			// 
			this.listBoxMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listBoxMain.IntegralHeight = false;
			this.listBoxMain.Location = new System.Drawing.Point(24, 45);
			this.listBoxMain.Name = "listBoxMain";
			this.listBoxMain.Size = new System.Drawing.Size(262, 393);
			this.listBoxMain.TabIndex = 2;
			this.listBoxMain.DoubleClick += new System.EventHandler(this.listMain_DoubleClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(21, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(383, 32);
			this.label1.TabIndex = 11;
			this.label1.Text = "This is a list of radiograph mounts and image composites.  You can freely edit, m" +
    "ove, or delete any of these mounts without affecting patient records";
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(24, 444);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(80, 26);
			this.butAdd.TabIndex = 10;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(343, 444);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 26);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butDown
			// 
			this.butDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(204, 444);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(82, 26);
			this.butDown.TabIndex = 36;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butUp
			// 
			this.butUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(113, 444);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(82, 26);
			this.butUp.TabIndex = 37;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// FormMountDefs
			// 
			this.ClientSize = new System.Drawing.Size(441, 487);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.listBoxMain);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMountDefs";
			this.ShowInTaskbar = false;
			this.Text = "Mount Definitions";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMounts_FormClosing);
			this.Load += new System.EventHandler(this.FormMountDefs_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.ListBoxOD listBoxMain;
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butDown;
		private OpenDental.UI.Button butUp;
	}
}
