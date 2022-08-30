using System.Drawing;

namespace OpenDental {
	partial class PhoneTile {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.labelExtensionName = new System.Windows.Forms.Label();
			this.labelStatusAndNote = new System.Windows.Forms.Label();
			this.pictureInUse = new System.Windows.Forms.PictureBox();
			this.labelTime = new System.Windows.Forms.Label();
			this.service11 = new OpenDental.localhost.Service1();
			this.timerFlash = new System.Windows.Forms.Timer(this.components);
			this.pictureProx = new System.Windows.Forms.PictureBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.pictureNeedsHelpButton = new System.Windows.Forms.PictureBox();
			this.pictureGTA = new System.Windows.Forms.PictureBox();
			this.pictureWebChat = new System.Windows.Forms.PictureBox();
			this.labelCustomer = new System.Windows.Forms.Label();
			this.pictureRemoteSupport = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureInUse)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureProx)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureNeedsHelpButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureGTA)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureWebChat)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureRemoteSupport)).BeginInit();
			this.SuspendLayout();
			// 
			// labelExtensionName
			// 
			this.labelExtensionName.BackColor = System.Drawing.Color.Transparent;
			this.labelExtensionName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelExtensionName.Location = new System.Drawing.Point(0, 0);
			this.labelExtensionName.Name = "labelExtensionName";
			this.labelExtensionName.Size = new System.Drawing.Size(97, 16);
			this.labelExtensionName.TabIndex = 1;
			this.labelExtensionName.Text = "104-JordanS";
			this.labelExtensionName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelExtensionName.DoubleClick += new System.EventHandler(this.labelExtensionName_DoubleClick);
			// 
			// labelStatusAndNote
			// 
			this.labelStatusAndNote.AutoEllipsis = true;
			this.labelStatusAndNote.BackColor = System.Drawing.Color.Transparent;
			this.labelStatusAndNote.Location = new System.Drawing.Point(21, 18);
			this.labelStatusAndNote.Name = "labelStatusAndNote";
			this.labelStatusAndNote.Size = new System.Drawing.Size(73, 16);
			this.labelStatusAndNote.TabIndex = 2;
			this.labelStatusAndNote.Text = "Available";
			this.labelStatusAndNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelStatusAndNote.MouseUp += new System.Windows.Forms.MouseEventHandler(this.labelStatusAndNote_MouseUp);
			// 
			// pictureInUse
			// 
			this.pictureInUse.Image = global::OpenDental.Properties.Resources.phoneInUse;
			this.pictureInUse.InitialImage = global::OpenDental.Properties.Resources.phoneInUse;
			this.pictureInUse.Location = new System.Drawing.Point(0, 16);
			this.pictureInUse.Name = "pictureInUse";
			this.pictureInUse.Size = new System.Drawing.Size(21, 17);
			this.pictureInUse.TabIndex = 3;
			this.pictureInUse.TabStop = false;
			// 
			// labelTime
			// 
			this.labelTime.BackColor = System.Drawing.Color.Lime;
			this.labelTime.Location = new System.Drawing.Point(99, 3);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(56, 16);
			this.labelTime.TabIndex = 5;
			this.labelTime.Text = "01:10:13";
			this.labelTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// service11
			// 
			this.service11.Url = "http://localhost:3824/Service1.asmx";
			this.service11.UseDefaultCredentials = true;
			// 
			// timerFlash
			// 
			this.timerFlash.Interval = 300;
			this.timerFlash.Tick += new System.EventHandler(this.timerFlash_Tick);
			// 
			// pictureProx
			// 
			this.pictureProx.Image = global::OpenDental.Properties.Resources.Figure;
			this.pictureProx.InitialImage = global::OpenDental.Properties.Resources.Figure;
			this.pictureProx.Location = new System.Drawing.Point(161, 3);
			this.pictureProx.Name = "pictureProx";
			this.pictureProx.Size = new System.Drawing.Size(21, 17);
			this.pictureProx.TabIndex = 7;
			this.pictureProx.TabStop = false;
			// 
			// pictureNeedsHelpButton
			// 
			this.pictureNeedsHelpButton.Image = global::OpenDental.Properties.Resources.raisehanddisabled;
			this.pictureNeedsHelpButton.InitialImage = global::OpenDental.Properties.Resources.raisehanddisabled;
			this.pictureNeedsHelpButton.Location = new System.Drawing.Point(188, 0);
			this.pictureNeedsHelpButton.Name = "pictureNeedsHelpButton";
			this.pictureNeedsHelpButton.Size = new System.Drawing.Size(23, 23);
			this.pictureNeedsHelpButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureNeedsHelpButton.TabIndex = 12;
			this.pictureNeedsHelpButton.TabStop = false;
			this.toolTip1.SetToolTip(this.pictureNeedsHelpButton, "Request Assistance");
			this.pictureNeedsHelpButton.Visible = false;
			this.pictureNeedsHelpButton.Click += new System.EventHandler(this.labelNeedsHelpButton_Click);
			// 
			// pictureGTA
			// 
			this.pictureGTA.Image = global::OpenDental.Properties.Resources.gtaicon3;
			this.pictureGTA.InitialImage = global::OpenDental.Properties.Resources.gtaicon3;
			this.pictureGTA.Location = new System.Drawing.Point(0, 16);
			this.pictureGTA.Name = "pictureGTA";
			this.pictureGTA.Size = new System.Drawing.Size(18, 18);
			this.pictureGTA.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureGTA.TabIndex = 13;
			this.pictureGTA.TabStop = false;
			// 
			// pictureWebChat
			// 
			this.pictureWebChat.Image = global::OpenDental.Properties.Resources.WebChatIcon;
			this.pictureWebChat.InitialImage = global::OpenDental.Properties.Resources.WebChatIcon;
			this.pictureWebChat.Location = new System.Drawing.Point(0, 16);
			this.pictureWebChat.Name = "pictureWebChat";
			this.pictureWebChat.Size = new System.Drawing.Size(18, 18);
			this.pictureWebChat.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureWebChat.TabIndex = 14;
			this.pictureWebChat.TabStop = false;
			this.pictureWebChat.Visible = false;
			// 
			// labelCustomer
			// 
			this.labelCustomer.Location = new System.Drawing.Point(96, 18);
			this.labelCustomer.Name = "labelCustomer";
			this.labelCustomer.Size = new System.Drawing.Size(88, 16);
			this.labelCustomer.TabIndex = 15;
			this.labelCustomer.Text = "Customer phone #";
			this.labelCustomer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelCustomer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelCustomer_MouseClick);
			this.labelCustomer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.labelCustomer_MouseUp);
			// 
			// pictureRemoteSupport
			// 
			this.pictureRemoteSupport.Image = global::OpenDental.Properties.Resources.remoteSupportIcon;
			this.pictureRemoteSupport.InitialImage = global::OpenDental.Properties.Resources.remoteSupportIcon;
			this.pictureRemoteSupport.Location = new System.Drawing.Point(0, 16);
			this.pictureRemoteSupport.Name = "pictureRemoteSupport";
			this.pictureRemoteSupport.Size = new System.Drawing.Size(18, 18);
			this.pictureRemoteSupport.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureRemoteSupport.TabIndex = 16;
			this.pictureRemoteSupport.TabStop = false;
			// 
			// PhoneTile
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.pictureInUse);
			this.Controls.Add(this.pictureNeedsHelpButton);
			this.Controls.Add(this.pictureProx);
			this.Controls.Add(this.labelExtensionName);
			this.Controls.Add(this.labelTime);
			this.Controls.Add(this.labelCustomer);
			this.Controls.Add(this.labelStatusAndNote);
			this.Controls.Add(this.pictureGTA);
			this.Controls.Add(this.pictureWebChat);
			this.Controls.Add(this.pictureRemoteSupport);
			this.DoubleBuffered = true;
			this.Name = "PhoneTile";
			this.Size = new System.Drawing.Size(183, 36);
			((System.ComponentModel.ISupportInitialize)(this.pictureInUse)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureProx)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureNeedsHelpButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureGTA)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureWebChat)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureRemoteSupport)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Label labelExtensionName;
		private System.Windows.Forms.Label labelStatusAndNote;
		private System.Windows.Forms.PictureBox pictureInUse;
		private System.Windows.Forms.Label labelTime;
		private localhost.Service1 service11;
		private System.Windows.Forms.Timer timerFlash;
		private System.Windows.Forms.PictureBox pictureProx;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.PictureBox pictureNeedsHelpButton;
		private System.Windows.Forms.PictureBox pictureGTA;
		private System.Windows.Forms.PictureBox pictureWebChat;
		private System.Windows.Forms.Label labelCustomer;
		private System.Windows.Forms.PictureBox pictureRemoteSupport;
	}
}
