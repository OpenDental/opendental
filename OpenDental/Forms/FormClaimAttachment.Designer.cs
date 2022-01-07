namespace OpenDental {
	partial class FormClaimAttachment {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				components?.Dispose();
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
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.labelNarrative = new System.Windows.Forms.Label();
			this.labelPasteImage = new System.Windows.Forms.Label();
			this.butPasteImage = new OpenDental.UI.Button();
			this.labelCharCount = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textClaimStatus = new OpenDental.ODtextBox();
			this.gridAttachedImages = new OpenDental.UI.GridOD();
			this.butSnipTool = new OpenDental.UI.Button();
			this.butAddImage = new OpenDental.UI.Button();
			this.textNarrative = new OpenDental.ODtextBox();
			this.labelClaimAttachWarning = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.timerMonitorClipboard = new System.Windows.Forms.Timer(this.components);
			this.timerKillSnipToolProcesses = new System.Windows.Forms.Timer(this.components);
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(108, 26);
			this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ContextMenu_ItemClicked);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.deleteToolStripMenuItem.Text = "Delete";
			// 
			// labelNarrative
			// 
			this.labelNarrative.Location = new System.Drawing.Point(11, 248);
			this.labelNarrative.Name = "labelNarrative";
			this.labelNarrative.Size = new System.Drawing.Size(121, 13);
			this.labelNarrative.TabIndex = 12;
			this.labelNarrative.Text = "Narrative (optional)";
			// 
			// labelPasteImage
			// 
			this.labelPasteImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPasteImage.Location = new System.Drawing.Point(491, 98);
			this.labelPasteImage.Name = "labelPasteImage";
			this.labelPasteImage.Size = new System.Drawing.Size(87, 18);
			this.labelPasteImage.TabIndex = 16;
			this.labelPasteImage.Text = "(from clipboard)";
			this.labelPasteImage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// butPasteImage
			// 
			this.butPasteImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butPasteImage.Location = new System.Drawing.Point(492, 71);
			this.butPasteImage.Name = "butPasteImage";
			this.butPasteImage.Size = new System.Drawing.Size(85, 24);
			this.butPasteImage.TabIndex = 15;
			this.butPasteImage.Text = "Paste Image";
			this.butPasteImage.UseVisualStyleBackColor = true;
			this.butPasteImage.Click += new System.EventHandler(this.ButPasteImage_Click);
			// 
			// labelCharCount
			// 
			this.labelCharCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCharCount.Location = new System.Drawing.Point(184, 248);
			this.labelCharCount.Name = "labelCharCount";
			this.labelCharCount.Size = new System.Drawing.Size(76, 13);
			this.labelCharCount.TabIndex = 14;
			this.labelCharCount.Text = "/2000";
			this.labelCharCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(287, 248);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(135, 13);
			this.label1.TabIndex = 13;
			this.label1.Text = "Claim Validation Status";
			// 
			// textClaimStatus
			// 
			this.textClaimStatus.AcceptsTab = true;
			this.textClaimStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textClaimStatus.BackColor = System.Drawing.SystemColors.Control;
			this.textClaimStatus.DetectLinksEnabled = false;
			this.textClaimStatus.DetectUrls = false;
			this.textClaimStatus.Location = new System.Drawing.Point(290, 264);
			this.textClaimStatus.Name = "textClaimStatus";
			this.textClaimStatus.QuickPasteType = OpenDentBusiness.QuickPasteType.Claim;
			this.textClaimStatus.ReadOnly = true;
			this.textClaimStatus.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textClaimStatus.Size = new System.Drawing.Size(301, 91);
			this.textClaimStatus.TabIndex = 6;
			this.textClaimStatus.Text = "";
			// 
			// gridAttachedImages
			// 
			this.gridAttachedImages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridAttachedImages.ContextMenuStrip = this.contextMenuStrip1;
			this.gridAttachedImages.Location = new System.Drawing.Point(10, 12);
			this.gridAttachedImages.Name = "gridAttachedImages";
			this.gridAttachedImages.Size = new System.Drawing.Size(460, 231);
			this.gridAttachedImages.TabIndex = 10;
			this.gridAttachedImages.Title = "Images to Send";
			this.gridAttachedImages.TranslationName = "TableImagesToSend";
			this.gridAttachedImages.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridAttachedImages_CellDoubleClick);
			// 
			// butSnipTool
			// 
			this.butSnipTool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSnipTool.Location = new System.Drawing.Point(492, 41);
			this.butSnipTool.Name = "butSnipTool";
			this.butSnipTool.Size = new System.Drawing.Size(85, 24);
			this.butSnipTool.TabIndex = 0;
			this.butSnipTool.Text = "Snipping Tool";
			this.butSnipTool.UseVisualStyleBackColor = true;
			this.butSnipTool.Click += new System.EventHandler(this.buttonSnipTool_Click);
			// 
			// butAddImage
			// 
			this.butAddImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddImage.Location = new System.Drawing.Point(492, 12);
			this.butAddImage.Name = "butAddImage";
			this.butAddImage.Size = new System.Drawing.Size(85, 24);
			this.butAddImage.TabIndex = 2;
			this.butAddImage.Text = "Add Image";
			this.butAddImage.UseVisualStyleBackColor = true;
			this.butAddImage.Click += new System.EventHandler(this.buttonAddImage_Click);
			// 
			// textNarrative
			// 
			this.textNarrative.AcceptsTab = true;
			this.textNarrative.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textNarrative.BackColor = System.Drawing.SystemColors.Window;
			this.textNarrative.DetectLinksEnabled = false;
			this.textNarrative.DetectUrls = false;
			this.textNarrative.Location = new System.Drawing.Point(10, 264);
			this.textNarrative.MaxLength = 2000;
			this.textNarrative.Name = "textNarrative";
			this.textNarrative.QuickPasteType = OpenDentBusiness.QuickPasteType.Claim;
			this.textNarrative.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNarrative.Size = new System.Drawing.Size(261, 91);
			this.textNarrative.TabIndex = 11;
			this.textNarrative.Text = "";
			this.textNarrative.TextChanged += new System.EventHandler(this.textNarrative_TextChanged);
			// 
			// labelClaimAttachWarning
			// 
			this.labelClaimAttachWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelClaimAttachWarning.ForeColor = System.Drawing.Color.DarkRed;
			this.labelClaimAttachWarning.Location = new System.Drawing.Point(7, 365);
			this.labelClaimAttachWarning.Name = "labelClaimAttachWarning";
			this.labelClaimAttachWarning.Size = new System.Drawing.Size(411, 27);
			this.labelClaimAttachWarning.TabIndex = 15;
			this.labelClaimAttachWarning.Text = "No claim attachment image category definition found.  Images will be saved using " +
    "the first image category.";
			this.labelClaimAttachWarning.Visible = false;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(519, 366);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(438, 366);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// timerMonitorClipboard
			// 
			this.timerMonitorClipboard.Interval = 250;
			this.timerMonitorClipboard.Tick += new System.EventHandler(this.timerMonitorClipboard_Tick);
			// 
			// timerKillSnipToolProcesses
			// 
			this.timerKillSnipToolProcesses.Tick += new System.EventHandler(this.timerKillSnipToolProcesses_Tick);
			// 
			// FormClaimAttachment
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(606, 399);
			this.Controls.Add(this.labelPasteImage);
			this.Controls.Add(this.butPasteImage);
			this.Controls.Add(this.labelClaimAttachWarning);
			this.Controls.Add(this.labelCharCount);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.textClaimStatus);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.gridAttachedImages);
			this.Controls.Add(this.butSnipTool);
			this.Controls.Add(this.textNarrative);
			this.Controls.Add(this.labelNarrative);
			this.Controls.Add(this.butAddImage);
			this.Name = "FormClaimAttachment";
			this.Text = "Claim Attachments";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormClaimAttachment_FormClosed);
			this.Load += new System.EventHandler(this.FormClaimAttachment_Load);
			this.Shown += new System.EventHandler(this.FormClaimAttachment_Shown);
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Button butSnipTool;
		private UI.Button butAddImage;
		private UI.Button butOK;
		private UI.Button butCancel;
		private ODtextBox textClaimStatus;
		private UI.GridOD gridAttachedImages;
		private ODtextBox textNarrative;
		private System.Windows.Forms.Label labelNarrative;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.Label labelClaimAttachWarning;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelCharCount;
		private UI.Button butPasteImage;
		private System.Windows.Forms.Label labelPasteImage;
		private System.Windows.Forms.Timer timerMonitorClipboard;
		private System.Windows.Forms.Timer timerKillSnipToolProcesses;
	}
}