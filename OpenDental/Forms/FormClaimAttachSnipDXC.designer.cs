namespace OpenDental {
	partial class FormClaimAttachSnipDXC {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClaimAttachSnipDXC));
			this.labelImageType = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textDateCreated = new OpenDental.ValidDate();
			this.labelDateTimeCreate = new System.Windows.Forms.Label();
			this.butSend = new OpenDental.UI.Button();
			this.textFileName = new System.Windows.Forms.TextBox();
			this.listBoxImageType = new OpenDental.UI.ListBox();
			this.checkIsXrayMirrored = new OpenDental.UI.CheckBox();
			this.pictureBoxImagePreview = new System.Windows.Forms.PictureBox();
			this.butSendAndAgain = new OpenDental.UI.Button();
			this.labelNewSnip = new System.Windows.Forms.Label();
			this.labelClaimAttachWarning = new System.Windows.Forms.Label();
			this.timerKillSnipToolProcesses = new System.Windows.Forms.Timer(this.components);
			this.timerMonitorClipboard = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxImagePreview)).BeginInit();
			this.SuspendLayout();
			// 
			// labelImageType
			// 
			this.labelImageType.Location = new System.Drawing.Point(54, 97);
			this.labelImageType.Name = "labelImageType";
			this.labelImageType.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelImageType.Size = new System.Drawing.Size(84, 15);
			this.labelImageType.TabIndex = 1;
			this.labelImageType.Text = "Image Type";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(54, 44);
			this.label1.Name = "label1";
			this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label1.Size = new System.Drawing.Size(84, 15);
			this.label1.TabIndex = 2;
			this.label1.Text = "File Name";
			// 
			// textDateCreated
			// 
			this.textDateCreated.Location = new System.Drawing.Point(139, 68);
			this.textDateCreated.Name = "textDateCreated";
			this.textDateCreated.Size = new System.Drawing.Size(139, 20);
			this.textDateCreated.TabIndex = 1;
			// 
			// labelDateTimeCreate
			// 
			this.labelDateTimeCreate.Location = new System.Drawing.Point(54, 71);
			this.labelDateTimeCreate.Name = "labelDateTimeCreate";
			this.labelDateTimeCreate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelDateTimeCreate.Size = new System.Drawing.Size(84, 15);
			this.labelDateTimeCreate.TabIndex = 6;
			this.labelDateTimeCreate.Text = "Date Created";
			// 
			// butSend
			// 
			this.butSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSend.Location = new System.Drawing.Point(390, 302);
			this.butSend.Name = "butSend";
			this.butSend.Size = new System.Drawing.Size(75, 24);
			this.butSend.TabIndex = 4;
			this.butSend.Text = "&Send";
			this.butSend.UseVisualStyleBackColor = true;
			this.butSend.Click += new System.EventHandler(this.butSend_Click);
			// 
			// textFileName
			// 
			this.textFileName.Location = new System.Drawing.Point(139, 41);
			this.textFileName.Name = "textFileName";
			this.textFileName.Size = new System.Drawing.Size(139, 20);
			this.textFileName.TabIndex = 0;
			this.textFileName.Text = "Attachment";
			// 
			// listBoxImageType
			// 
			this.listBoxImageType.Location = new System.Drawing.Point(139, 97);
			this.listBoxImageType.Name = "listBoxImageType";
			this.listBoxImageType.Size = new System.Drawing.Size(139, 145);
			this.listBoxImageType.TabIndex = 2;
			this.listBoxImageType.Text = "Image Type";
			// 
			// checkIsXrayMirrored
			// 
			this.checkIsXrayMirrored.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsXrayMirrored.Location = new System.Drawing.Point(27, 249);
			this.checkIsXrayMirrored.Name = "checkIsXrayMirrored";
			this.checkIsXrayMirrored.Size = new System.Drawing.Size(126, 18);
			this.checkIsXrayMirrored.TabIndex = 3;
			this.checkIsXrayMirrored.Text = "Is xray mirror image";
			// 
			// pictureBoxImagePreview
			// 
			this.pictureBoxImagePreview.BackColor = System.Drawing.SystemColors.Control;
			this.pictureBoxImagePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxImagePreview.Location = new System.Drawing.Point(323, 41);
			this.pictureBoxImagePreview.Name = "pictureBoxImagePreview";
			this.pictureBoxImagePreview.Size = new System.Drawing.Size(190, 190);
			this.pictureBoxImagePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBoxImagePreview.TabIndex = 9;
			this.pictureBoxImagePreview.TabStop = false;
			// 
			// butSendAndAgain
			// 
			this.butSendAndAgain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSendAndAgain.Location = new System.Drawing.Point(390, 262);
			this.butSendAndAgain.Name = "butSendAndAgain";
			this.butSendAndAgain.Size = new System.Drawing.Size(75, 24);
			this.butSendAndAgain.TabIndex = 6;
			this.butSendAndAgain.Text = "&Send";
			this.butSendAndAgain.UseVisualStyleBackColor = true;
			this.butSendAndAgain.Click += new System.EventHandler(this.butSendAndAgain_Click);
			// 
			// labelNewSnip
			// 
			this.labelNewSnip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNewSnip.Location = new System.Drawing.Point(469, 265);
			this.labelNewSnip.Name = "labelNewSnip";
			this.labelNewSnip.Size = new System.Drawing.Size(116, 15);
			this.labelNewSnip.TabIndex = 10;
			this.labelNewSnip.Text = "(and Snip Another)";
			this.labelNewSnip.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelClaimAttachWarning
			// 
			this.labelClaimAttachWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelClaimAttachWarning.ForeColor = System.Drawing.Color.DarkRed;
			this.labelClaimAttachWarning.Location = new System.Drawing.Point(12, 292);
			this.labelClaimAttachWarning.Name = "labelClaimAttachWarning";
			this.labelClaimAttachWarning.Size = new System.Drawing.Size(355, 34);
			this.labelClaimAttachWarning.TabIndex = 16;
			this.labelClaimAttachWarning.Text = "No claim attachment image category definition found.  Images will be saved using " +
    "the first image category.";
			// 
			// timerKillSnipToolProcesses
			// 
			this.timerKillSnipToolProcesses.Tick += new System.EventHandler(this.timerKillSnipToolProcesses_Tick);
			// 
			// timerMonitorClipboard
			// 
			this.timerMonitorClipboard.Interval = 250;
			this.timerMonitorClipboard.Tick += new System.EventHandler(this.timerMonitorClipboard_Tick);
			// 
			// FormClaimAttachSnipDXC
			// 
			this.AcceptButton = this.butSend;
			this.ClientSize = new System.Drawing.Size(584, 338);
			this.Controls.Add(this.labelClaimAttachWarning);
			this.Controls.Add(this.labelNewSnip);
			this.Controls.Add(this.butSendAndAgain);
			this.Controls.Add(this.pictureBoxImagePreview);
			this.Controls.Add(this.checkIsXrayMirrored);
			this.Controls.Add(this.listBoxImageType);
			this.Controls.Add(this.textFileName);
			this.Controls.Add(this.butSend);
			this.Controls.Add(this.labelDateTimeCreate);
			this.Controls.Add(this.textDateCreated);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelImageType);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormClaimAttachSnipDXC";
			this.Text = "Image Info";
			this.Load += new System.EventHandler(this.FormClaimAttachmentItemEdit_Load);
			this.Shown += new System.EventHandler(this.FormClaimAttachmentItemEdit_Shown);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxImagePreview)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label labelImageType;
		private System.Windows.Forms.Label label1;
		private ValidDate textDateCreated;
		private System.Windows.Forms.Label labelDateTimeCreate;
		private UI.Button butSend;
		private System.Windows.Forms.TextBox textFileName;
		private UI.ListBox listBoxImageType;
		private OpenDental.UI.CheckBox checkIsXrayMirrored;
		private System.Windows.Forms.PictureBox pictureBoxImagePreview;
		private UI.Button butSendAndAgain;
		private System.Windows.Forms.Label labelNewSnip;
		private System.Windows.Forms.Label labelClaimAttachWarning;
		private System.Windows.Forms.Timer timerKillSnipToolProcesses;
		private System.Windows.Forms.Timer timerMonitorClipboard;
	}
}