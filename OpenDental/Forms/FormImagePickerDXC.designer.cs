namespace OpenDental{
	partial class FormImagePickerDXC {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImagePickerDXC));
			this.butSend = new OpenDental.UI.Button();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.elementHostImageSelector = new System.Windows.Forms.Integration.ElementHost();
			this.checkIsXrayMirrored = new OpenDental.UI.CheckBox();
			this.listBoxImageType = new OpenDental.UI.ListBox();
			this.textFileName = new System.Windows.Forms.TextBox();
			this.labelDateTimeCreate = new System.Windows.Forms.Label();
			this.textDateCreated = new OpenDental.ValidDate();
			this.labelFileName = new System.Windows.Forms.Label();
			this.labelImageType = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textClaimStatus = new OpenDental.ODtextBox();
			this.butSendAndAgain = new OpenDental.UI.Button();
			this.textNarrative = new OpenDental.ODtextBox();
			this.labelNarrative = new System.Windows.Forms.Label();
			this.labelCharCount = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// butSend
			// 
			this.butSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSend.Location = new System.Drawing.Point(1143, 660);
			this.butSend.Name = "butSend";
			this.butSend.Size = new System.Drawing.Size(75, 24);
			this.butSend.TabIndex = 19;
			this.butSend.Text = "&Send";
			this.butSend.Click += new System.EventHandler(this.butSend_Click);
			// 
			// pictureBox
			// 
			this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox.Location = new System.Drawing.Point(254, 12);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(734, 672);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox.TabIndex = 22;
			this.pictureBox.TabStop = false;
			// 
			// elementHostImageSelector
			// 
			this.elementHostImageSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.elementHostImageSelector.Location = new System.Drawing.Point(11, 12);
			this.elementHostImageSelector.Name = "elementHostImageSelector";
			this.elementHostImageSelector.Size = new System.Drawing.Size(237, 672);
			this.elementHostImageSelector.TabIndex = 39;
			this.elementHostImageSelector.Text = "elementHost1";
			this.elementHostImageSelector.Child = null;
			// 
			// checkIsXrayMirrored
			// 
			this.checkIsXrayMirrored.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkIsXrayMirrored.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsXrayMirrored.Location = new System.Drawing.Point(1079, 222);
			this.checkIsXrayMirrored.Name = "checkIsXrayMirrored";
			this.checkIsXrayMirrored.Size = new System.Drawing.Size(126, 18);
			this.checkIsXrayMirrored.TabIndex = 45;
			this.checkIsXrayMirrored.Text = "Is xray mirror image";
			// 
			// listBoxImageType
			// 
			this.listBoxImageType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxImageType.Location = new System.Drawing.Point(1079, 71);
			this.listBoxImageType.Name = "listBoxImageType";
			this.listBoxImageType.Size = new System.Drawing.Size(139, 150);
			this.listBoxImageType.TabIndex = 43;
			this.listBoxImageType.Text = "Image Type";
			// 
			// textFileName
			// 
			this.textFileName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textFileName.Location = new System.Drawing.Point(1079, 15);
			this.textFileName.Name = "textFileName";
			this.textFileName.Size = new System.Drawing.Size(139, 20);
			this.textFileName.TabIndex = 40;
			this.textFileName.Text = "Attachment";
			// 
			// labelDateTimeCreate
			// 
			this.labelDateTimeCreate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDateTimeCreate.Location = new System.Drawing.Point(994, 45);
			this.labelDateTimeCreate.Name = "labelDateTimeCreate";
			this.labelDateTimeCreate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelDateTimeCreate.Size = new System.Drawing.Size(84, 15);
			this.labelDateTimeCreate.TabIndex = 46;
			this.labelDateTimeCreate.Text = "Date Created";
			// 
			// textDateCreated
			// 
			this.textDateCreated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDateCreated.Location = new System.Drawing.Point(1079, 42);
			this.textDateCreated.Name = "textDateCreated";
			this.textDateCreated.Size = new System.Drawing.Size(139, 20);
			this.textDateCreated.TabIndex = 41;
			// 
			// labelFileName
			// 
			this.labelFileName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelFileName.Location = new System.Drawing.Point(994, 18);
			this.labelFileName.Name = "labelFileName";
			this.labelFileName.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelFileName.Size = new System.Drawing.Size(84, 15);
			this.labelFileName.TabIndex = 44;
			this.labelFileName.Text = "File Name";
			// 
			// labelImageType
			// 
			this.labelImageType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelImageType.Location = new System.Drawing.Point(994, 71);
			this.labelImageType.Name = "labelImageType";
			this.labelImageType.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelImageType.Size = new System.Drawing.Size(84, 15);
			this.labelImageType.TabIndex = 42;
			this.labelImageType.Text = "Image Type";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(1021, 268);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(135, 13);
			this.label1.TabIndex = 48;
			this.label1.Text = "Claim Validation Status";
			// 
			// textClaimStatus
			// 
			this.textClaimStatus.AcceptsTab = true;
			this.textClaimStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textClaimStatus.BackColor = System.Drawing.SystemColors.Control;
			this.textClaimStatus.DetectLinksEnabled = false;
			this.textClaimStatus.DetectUrls = false;
			this.textClaimStatus.Location = new System.Drawing.Point(1024, 284);
			this.textClaimStatus.Name = "textClaimStatus";
			this.textClaimStatus.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.Claim;
			this.textClaimStatus.ReadOnly = true;
			this.textClaimStatus.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textClaimStatus.Size = new System.Drawing.Size(194, 91);
			this.textClaimStatus.TabIndex = 47;
			this.textClaimStatus.Text = "";
			// 
			// butSendAndAgain
			// 
			this.butSendAndAgain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSendAndAgain.Location = new System.Drawing.Point(1079, 628);
			this.butSendAndAgain.Name = "butSendAndAgain";
			this.butSendAndAgain.Size = new System.Drawing.Size(139, 24);
			this.butSendAndAgain.TabIndex = 49;
			this.butSendAndAgain.Text = "Send and Select &Another";
			this.butSendAndAgain.Click += new System.EventHandler(this.butSendAndAgain_Click);
			// 
			// textNarrative
			// 
			this.textNarrative.AcceptsTab = true;
			this.textNarrative.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textNarrative.BackColor = System.Drawing.SystemColors.Window;
			this.textNarrative.DetectLinksEnabled = false;
			this.textNarrative.DetectUrls = false;
			this.textNarrative.Location = new System.Drawing.Point(1024, 402);
			this.textNarrative.MaxLength = 2000;
			this.textNarrative.Name = "textNarrative";
			this.textNarrative.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.Claim;
			this.textNarrative.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNarrative.Size = new System.Drawing.Size(194, 210);
			this.textNarrative.TabIndex = 55;
			this.textNarrative.Text = "";
			this.textNarrative.TextChanged += new System.EventHandler(this.textNarrative_TextChanged);
			// 
			// labelNarrative
			// 
			this.labelNarrative.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNarrative.Location = new System.Drawing.Point(1021, 381);
			this.labelNarrative.Name = "labelNarrative";
			this.labelNarrative.Size = new System.Drawing.Size(88, 18);
			this.labelNarrative.TabIndex = 56;
			this.labelNarrative.Text = "Narrative";
			this.labelNarrative.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelCharCount
			// 
			this.labelCharCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCharCount.Location = new System.Drawing.Point(1142, 382);
			this.labelCharCount.Name = "labelCharCount";
			this.labelCharCount.Size = new System.Drawing.Size(76, 18);
			this.labelCharCount.TabIndex = 61;
			this.labelCharCount.Text = "/2000";
			this.labelCharCount.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// FormImagePickerDXC
			// 
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.labelCharCount);
			this.Controls.Add(this.labelNarrative);
			this.Controls.Add(this.textNarrative);
			this.Controls.Add(this.butSendAndAgain);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textClaimStatus);
			this.Controls.Add(this.checkIsXrayMirrored);
			this.Controls.Add(this.listBoxImageType);
			this.Controls.Add(this.textFileName);
			this.Controls.Add(this.labelDateTimeCreate);
			this.Controls.Add(this.textDateCreated);
			this.Controls.Add(this.labelFileName);
			this.Controls.Add(this.labelImageType);
			this.Controls.Add(this.elementHostImageSelector);
			this.Controls.Add(this.pictureBox);
			this.Controls.Add(this.butSend);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormImagePickerDXC";
			this.Text = "Select Image";
			this.Load += new System.EventHandler(this.FormImagePickerPatient_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private UI.Button butSend;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.Integration.ElementHost elementHostImageSelector;
		private UI.CheckBox checkIsXrayMirrored;
		private UI.ListBox listBoxImageType;
		private System.Windows.Forms.TextBox textFileName;
		private System.Windows.Forms.Label labelDateTimeCreate;
		private ValidDate textDateCreated;
		private System.Windows.Forms.Label labelFileName;
		private System.Windows.Forms.Label labelImageType;
		private System.Windows.Forms.Label label1;
		private ODtextBox textClaimStatus;
		private UI.Button butSendAndAgain;
		private ODtextBox textNarrative;
		private System.Windows.Forms.Label labelNarrative;
		private System.Windows.Forms.Label labelCharCount;
	}
}