namespace OpenDental {
	partial class FormClaimAttachHistory {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClaimAttachHistory));
			this.gridMain = new OpenDental.UI.GridOD();
			this.labelClaimStatus = new System.Windows.Forms.Label();
			this.textClaimStatus = new OpenDental.ODtextBox();
			this.labelNarrative = new System.Windows.Forms.Label();
			this.textNarrative = new OpenDental.ODtextBox();
			this.labelAttachmentID = new System.Windows.Forms.Label();
			this.textAttachmentID = new OpenDental.ODtextBox();
			this.butClear = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(14, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(353, 501);
			this.gridMain.TabIndex = 49;
			this.gridMain.Title = "Attachments Sent";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// labelClaimStatus
			// 
			this.labelClaimStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelClaimStatus.Location = new System.Drawing.Point(384, 6);
			this.labelClaimStatus.Name = "labelClaimStatus";
			this.labelClaimStatus.Size = new System.Drawing.Size(135, 18);
			this.labelClaimStatus.TabIndex = 52;
			this.labelClaimStatus.Text = "Claim Validation Status";
			this.labelClaimStatus.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textClaimStatus
			// 
			this.textClaimStatus.AcceptsTab = true;
			this.textClaimStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textClaimStatus.BackColor = System.Drawing.SystemColors.Control;
			this.textClaimStatus.DetectLinksEnabled = false;
			this.textClaimStatus.DetectUrls = false;
			this.textClaimStatus.Location = new System.Drawing.Point(387, 28);
			this.textClaimStatus.Name = "textClaimStatus";
			this.textClaimStatus.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.Claim;
			this.textClaimStatus.ReadOnly = true;
			this.textClaimStatus.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textClaimStatus.Size = new System.Drawing.Size(257, 122);
			this.textClaimStatus.TabIndex = 51;
			this.textClaimStatus.Text = "";
			// 
			// labelNarrative
			// 
			this.labelNarrative.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNarrative.Location = new System.Drawing.Point(384, 206);
			this.labelNarrative.Name = "labelNarrative";
			this.labelNarrative.Size = new System.Drawing.Size(88, 18);
			this.labelNarrative.TabIndex = 53;
			this.labelNarrative.Text = "Narrative";
			this.labelNarrative.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textNarrative
			// 
			this.textNarrative.AcceptsTab = true;
			this.textNarrative.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textNarrative.BackColor = System.Drawing.SystemColors.Window;
			this.textNarrative.DetectLinksEnabled = false;
			this.textNarrative.DetectUrls = false;
			this.textNarrative.Location = new System.Drawing.Point(387, 227);
			this.textNarrative.Name = "textNarrative";
			this.textNarrative.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.Claim;
			this.textNarrative.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNarrative.Size = new System.Drawing.Size(257, 286);
			this.textNarrative.TabIndex = 54;
			this.textNarrative.Text = "";
			// 
			// labelAttachmentID
			// 
			this.labelAttachmentID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelAttachmentID.Location = new System.Drawing.Point(384, 156);
			this.labelAttachmentID.Name = "labelAttachmentID";
			this.labelAttachmentID.Size = new System.Drawing.Size(109, 18);
			this.labelAttachmentID.TabIndex = 55;
			this.labelAttachmentID.Text = "Attachment ID";
			this.labelAttachmentID.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textAttachmentID
			// 
			this.textAttachmentID.AcceptsTab = true;
			this.textAttachmentID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textAttachmentID.BackColor = System.Drawing.SystemColors.Control;
			this.textAttachmentID.DetectLinksEnabled = false;
			this.textAttachmentID.DetectUrls = false;
			this.textAttachmentID.Location = new System.Drawing.Point(387, 177);
			this.textAttachmentID.Name = "textAttachmentID";
			this.textAttachmentID.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.Claim;
			this.textAttachmentID.ReadOnly = true;
			this.textAttachmentID.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textAttachmentID.Size = new System.Drawing.Size(132, 20);
			this.textAttachmentID.TabIndex = 56;
			this.textAttachmentID.Text = "";
			// 
			// butClear
			// 
			this.butClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butClear.Location = new System.Drawing.Point(534, 174);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(99, 24);
			this.butClear.TabIndex = 57;
			this.butClear.Text = "Clear Attachment";
			this.butClear.Click += new System.EventHandler(this.butClear_Click);
			// 
			// FormClaimAttachHistory
			// 
			this.ClientSize = new System.Drawing.Size(656, 530);
			this.Controls.Add(this.butClear);
			this.Controls.Add(this.textAttachmentID);
			this.Controls.Add(this.labelAttachmentID);
			this.Controls.Add(this.textNarrative);
			this.Controls.Add(this.labelNarrative);
			this.Controls.Add(this.labelClaimStatus);
			this.Controls.Add(this.textClaimStatus);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormClaimAttachHistory";
			this.Text = "Attachment History";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClaimAttachHistory_FormClosing);
			this.Load += new System.EventHandler(this.FormClaimAttachHistory_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridMain;
		private System.Windows.Forms.Label labelClaimStatus;
		private ODtextBox textClaimStatus;
		private System.Windows.Forms.Label labelNarrative;
		private ODtextBox textNarrative;
		private System.Windows.Forms.Label labelAttachmentID;
		private ODtextBox textAttachmentID;
		private UI.Button butClear;
	}
}