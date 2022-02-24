using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormMessagingButSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMessagingButSetup));
			this.label1 = new System.Windows.Forms.Label();
			this.listButtons = new OpenDental.UI.ListBoxOD();
			this.listComputers = new OpenDental.UI.ListBoxOD();
			this.label2 = new System.Windows.Forms.Label();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(274,26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(78,16);
			this.label1.TabIndex = 11;
			this.label1.Text = "Buttons";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listButtons
			// 
			this.listButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listButtons.Location = new System.Drawing.Point(276,45);
			this.listButtons.Name = "listButtons";
			this.listButtons.Size = new System.Drawing.Size(200,264);
			this.listButtons.TabIndex = 12;
			this.listButtons.DoubleClick += new System.EventHandler(this.listButtons_DoubleClick);
			// 
			// listComputers
			// 
			this.listComputers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listComputers.Location = new System.Drawing.Point(36,45);
			this.listComputers.Name = "listComputers";
			this.listComputers.Size = new System.Drawing.Size(198,264);
			this.listComputers.TabIndex = 23;
			this.listComputers.Click += new System.EventHandler(this.listComputers_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(34,26);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(78,16);
			this.label2.TabIndex = 22;
			this.label2.Text = "Computer";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butDown
			// 
			this.butDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(401,332);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(75,26);
			this.butDown.TabIndex = 21;
			this.butDown.Text = "Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butUp
			// 
			this.butUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(276,332);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(75,26);
			this.butUp.TabIndex = 20;
			this.butUp.Text = "Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(545,332);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormMessagingButSetup
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
			this.ClientSize = new System.Drawing.Size(654,383);
			this.Controls.Add(this.listComputers);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.listButtons);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMessagingButSetup";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Messaging Button Setup";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMessagingButSetup_FormClosing);
			this.Load += new System.EventHandler(this.FormMessagingButSetup_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butUp;
		private OpenDental.UI.Button butDown;
		private Label label1;
		private OpenDental.UI.ListBoxOD listButtons;
		private OpenDental.UI.ListBoxOD listComputers;
		private Label label2;
	}
}
