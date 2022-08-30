using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormMessagingSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMessagingSetup));
			this.listMessages = new OpenDental.UI.ListBoxOD();
			this.label5 = new System.Windows.Forms.Label();
			this.listExtras = new OpenDental.UI.ListBoxOD();
			this.label4 = new System.Windows.Forms.Label();
			this.listToFrom = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// listMessages
			// 
			this.listMessages.Location = new System.Drawing.Point(192, 38);
			this.listMessages.Name = "listMessages";
			this.listMessages.Size = new System.Drawing.Size(98, 368);
			this.listMessages.TabIndex = 18;
			this.listMessages.Click += new System.EventHandler(this.listMessages_Click);
			this.listMessages.DoubleClick += new System.EventHandler(this.listMessages_DoubleClick);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(190, 19);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 16);
			this.label5.TabIndex = 17;
			this.label5.Text = "Messages";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listExtras
			// 
			this.listExtras.Location = new System.Drawing.Point(111, 38);
			this.listExtras.Name = "listExtras";
			this.listExtras.Size = new System.Drawing.Size(75, 368);
			this.listExtras.TabIndex = 16;
			this.listExtras.Click += new System.EventHandler(this.listExtras_Click);
			this.listExtras.DoubleClick += new System.EventHandler(this.listExtras_DoubleClick);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(109, 19);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(78, 16);
			this.label4.TabIndex = 15;
			this.label4.Text = "Extras";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listToFrom
			// 
			this.listToFrom.Location = new System.Drawing.Point(30, 38);
			this.listToFrom.Name = "listToFrom";
			this.listToFrom.Size = new System.Drawing.Size(75, 368);
			this.listToFrom.TabIndex = 12;
			this.listToFrom.Click += new System.EventHandler(this.listToFrom_Click);
			this.listToFrom.DoubleClick += new System.EventHandler(this.listToFrom_DoubleClick);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(28, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(78, 16);
			this.label1.TabIndex = 11;
			this.label1.Text = "Users";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(365, 379);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(365, 241);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 26);
			this.butAdd.TabIndex = 19;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butUp
			// 
			this.butUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(365, 273);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(75, 26);
			this.butUp.TabIndex = 20;
			this.butUp.Text = "Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butDown
			// 
			this.butDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(365, 305);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(75, 26);
			this.butDown.TabIndex = 21;
			this.butDown.Text = "Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// FormMessagingSetup
			// 
			this.ClientSize = new System.Drawing.Size(492, 430);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.listMessages);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.listExtras);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.listToFrom);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMessagingSetup";
			this.ShowInTaskbar = false;
			this.Text = "Messaging Setup";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMessagingSetup_FormClosing);
			this.Load += new System.EventHandler(this.FormMessagingSetup_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.ListBoxOD listMessages;
		private Label label5;
		private OpenDental.UI.ListBoxOD listExtras;
		private Label label4;
		private OpenDental.UI.ListBoxOD listToFrom;
		private Label label1;
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butUp;
		private OpenDental.UI.Button butDown;
	}
}
