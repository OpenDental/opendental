namespace OpenDental {
	partial class FormFaqEdit {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing&&(components!=null)) {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFaqEdit));
			this.labelQuestion = new System.Windows.Forms.Label();
			this.textQuestion = new System.Windows.Forms.TextBox();
			this.labelAnswerText = new System.Windows.Forms.Label();
			this.textAnswer = new OpenDental.ODtextBox();
			this.buttonBold = new System.Windows.Forms.Button();
			this.buttonItalics = new System.Windows.Forms.Button();
			this.buttonUnderline = new System.Windows.Forms.Button();
			this.buttonHyperlink = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textManualVersion = new System.Windows.Forms.TextBox();
			this.butVersionPick = new System.Windows.Forms.Button();
			this.checkSticky = new System.Windows.Forms.CheckBox();
			this.labelImagePath = new System.Windows.Forms.Label();
			this.textImagePath = new System.Windows.Forms.TextBox();
			this.labelEmbeddedMediaUrl = new System.Windows.Forms.Label();
			this.textEmbeddedMediaURL = new System.Windows.Forms.TextBox();
			this.labelManualPageLink = new System.Windows.Forms.Label();
			this.textLinkPage = new System.Windows.Forms.TextBox();
			this.listAvailableManualPages = new OpenDental.UI.ListBoxOD();
			this.butRight = new System.Windows.Forms.Button();
			this.butLeft = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.listBoxLinkedPages = new OpenDental.UI.ListBoxOD();
			this.butDelete = new System.Windows.Forms.Button();
			this.butOK = new System.Windows.Forms.Button();
			this.butCancel = new System.Windows.Forms.Button();
			this.butBullet = new System.Windows.Forms.Button();
			this.webPreview = new System.Windows.Forms.WebBrowser();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labelQuestion
			// 
			this.labelQuestion.Location = new System.Drawing.Point(92, 25);
			this.labelQuestion.Name = "labelQuestion";
			this.labelQuestion.Size = new System.Drawing.Size(100, 14);
			this.labelQuestion.TabIndex = 6;
			this.labelQuestion.Text = "Question Text";
			this.labelQuestion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textQuestion
			// 
			this.textQuestion.Location = new System.Drawing.Point(194, 22);
			this.textQuestion.Name = "textQuestion";
			this.textQuestion.Size = new System.Drawing.Size(365, 20);
			this.textQuestion.TabIndex = 7;
			this.textQuestion.TextChanged += new System.EventHandler(this.textQuestion_TextChanged);
			// 
			// labelAnswerText
			// 
			this.labelAnswerText.Location = new System.Drawing.Point(77, 78);
			this.labelAnswerText.Name = "labelAnswerText";
			this.labelAnswerText.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelAnswerText.Size = new System.Drawing.Size(115, 21);
			this.labelAnswerText.TabIndex = 8;
			this.labelAnswerText.Text = "Answer Content";
			this.labelAnswerText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAnswer
			// 
			this.textAnswer.AcceptsTab = true;
			this.textAnswer.BackColor = System.Drawing.SystemColors.Window;
			this.textAnswer.DetectLinksEnabled = false;
			this.textAnswer.DetectUrls = false;
			this.textAnswer.Location = new System.Drawing.Point(198, 84);
			this.textAnswer.Name = "textAnswer";
			this.textAnswer.QuickPasteType = OpenDentBusiness.QuickPasteType.FAQ;
			this.textAnswer.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textAnswer.Size = new System.Drawing.Size(361, 209);
			this.textAnswer.TabIndex = 9;
			this.textAnswer.Text = "";
			this.textAnswer.TextChanged += new System.EventHandler(this.textAnswer_TextChanged);
			// 
			// buttonBold
			// 
			this.buttonBold.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonBold.Location = new System.Drawing.Point(194, 54);
			this.buttonBold.Name = "buttonBold";
			this.buttonBold.Size = new System.Drawing.Size(24, 24);
			this.buttonBold.TabIndex = 10;
			this.buttonBold.Text = "B";
			this.buttonBold.UseVisualStyleBackColor = true;
			this.buttonBold.Click += new System.EventHandler(this.ButtonBold_Click);
			// 
			// buttonItalics
			// 
			this.buttonItalics.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonItalics.Location = new System.Drawing.Point(221, 54);
			this.buttonItalics.Name = "buttonItalics";
			this.buttonItalics.Size = new System.Drawing.Size(24, 24);
			this.buttonItalics.TabIndex = 11;
			this.buttonItalics.Text = "I";
			this.buttonItalics.UseVisualStyleBackColor = true;
			this.buttonItalics.Click += new System.EventHandler(this.ButtonItalics_Click);
			// 
			// buttonUnderline
			// 
			this.buttonUnderline.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonUnderline.Location = new System.Drawing.Point(248, 54);
			this.buttonUnderline.Name = "buttonUnderline";
			this.buttonUnderline.Size = new System.Drawing.Size(24, 24);
			this.buttonUnderline.TabIndex = 12;
			this.buttonUnderline.Text = "U";
			this.buttonUnderline.UseVisualStyleBackColor = true;
			this.buttonUnderline.Click += new System.EventHandler(this.buttonUnderline_Click);
			// 
			// buttonHyperlink
			// 
			this.buttonHyperlink.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonHyperlink.Image = ((System.Drawing.Image)(resources.GetObject("buttonHyperlink.Image")));
			this.buttonHyperlink.Location = new System.Drawing.Point(308, 54);
			this.buttonHyperlink.Name = "buttonHyperlink";
			this.buttonHyperlink.Size = new System.Drawing.Size(25, 24);
			this.buttonHyperlink.TabIndex = 13;
			this.buttonHyperlink.UseVisualStyleBackColor = true;
			this.buttonHyperlink.Click += new System.EventHandler(this.buttonHyperlink_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(80, 300);
			this.label1.Name = "label1";
			this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.label1.Size = new System.Drawing.Size(108, 21);
			this.label1.TabIndex = 21;
			this.label1.Text = "Version";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textManualVersion
			// 
			this.textManualVersion.Location = new System.Drawing.Point(195, 301);
			this.textManualVersion.Name = "textManualVersion";
			this.textManualVersion.ReadOnly = true;
			this.textManualVersion.Size = new System.Drawing.Size(169, 20);
			this.textManualVersion.TabIndex = 22;
			// 
			// butVersionPick
			// 
			this.butVersionPick.Location = new System.Drawing.Point(367, 299);
			this.butVersionPick.Name = "butVersionPick";
			this.butVersionPick.Size = new System.Drawing.Size(27, 23);
			this.butVersionPick.TabIndex = 23;
			this.butVersionPick.Text = "...";
			this.butVersionPick.UseVisualStyleBackColor = true;
			this.butVersionPick.Click += new System.EventHandler(this.ButVersionPick_Click);
			// 
			// checkSticky
			// 
			this.checkSticky.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSticky.Location = new System.Drawing.Point(95, 326);
			this.checkSticky.Name = "checkSticky";
			this.checkSticky.Size = new System.Drawing.Size(113, 17);
			this.checkSticky.TabIndex = 24;
			this.checkSticky.Text = "Stickied to Top";
			this.checkSticky.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSticky.UseVisualStyleBackColor = true;
			// 
			// labelImagePath
			// 
			this.labelImagePath.Location = new System.Drawing.Point(68, 348);
			this.labelImagePath.Name = "labelImagePath";
			this.labelImagePath.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelImagePath.Size = new System.Drawing.Size(120, 21);
			this.labelImagePath.TabIndex = 25;
			this.labelImagePath.Text = "Image URL (optional)";
			this.labelImagePath.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textImagePath
			// 
			this.textImagePath.Location = new System.Drawing.Point(194, 349);
			this.textImagePath.Name = "textImagePath";
			this.textImagePath.Size = new System.Drawing.Size(446, 20);
			this.textImagePath.TabIndex = 26;
			this.textImagePath.TextChanged += new System.EventHandler(this.textImagePath_TextChanged);
			// 
			// labelEmbeddedMediaUrl
			// 
			this.labelEmbeddedMediaUrl.Location = new System.Drawing.Point(12, 377);
			this.labelEmbeddedMediaUrl.Name = "labelEmbeddedMediaUrl";
			this.labelEmbeddedMediaUrl.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelEmbeddedMediaUrl.Size = new System.Drawing.Size(176, 21);
			this.labelEmbeddedMediaUrl.TabIndex = 59;
			this.labelEmbeddedMediaUrl.Text = "Embedded Media URL (optional)";
			this.labelEmbeddedMediaUrl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textEmbeddedMediaURL
			// 
			this.textEmbeddedMediaURL.Location = new System.Drawing.Point(194, 377);
			this.textEmbeddedMediaURL.Name = "textEmbeddedMediaURL";
			this.textEmbeddedMediaURL.Size = new System.Drawing.Size(446, 20);
			this.textEmbeddedMediaURL.TabIndex = 60;
			this.textEmbeddedMediaURL.TextChanged += new System.EventHandler(this.textEmbeddedMediaURL_TextChanged);
			// 
			// labelManualPageLink
			// 
			this.labelManualPageLink.Location = new System.Drawing.Point(191, 415);
			this.labelManualPageLink.Name = "labelManualPageLink";
			this.labelManualPageLink.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelManualPageLink.Size = new System.Drawing.Size(94, 20);
			this.labelManualPageLink.TabIndex = 61;
			this.labelManualPageLink.Text = "Filter Page Names";
			this.labelManualPageLink.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLinkPage
			// 
			this.textLinkPage.Location = new System.Drawing.Point(291, 417);
			this.textLinkPage.Name = "textLinkPage";
			this.textLinkPage.Size = new System.Drawing.Size(139, 20);
			this.textLinkPage.TabIndex = 62;
			this.textLinkPage.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextLinkPage_KeyUp);
			// 
			// listAvailableManualPages
			// 
			this.listAvailableManualPages.Location = new System.Drawing.Point(194, 442);
			this.listAvailableManualPages.Name = "listAvailableManualPages";
			this.listAvailableManualPages.Size = new System.Drawing.Size(236, 238);
			this.listAvailableManualPages.TabIndex = 63;
			// 
			// butRight
			// 
			this.butRight.Image = ((System.Drawing.Image)(resources.GetObject("butRight.Image")));
			this.butRight.Location = new System.Drawing.Point(447, 529);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(35, 24);
			this.butRight.TabIndex = 64;
			this.butRight.UseVisualStyleBackColor = true;
			this.butRight.Click += new System.EventHandler(this.ButRight_Click);
			// 
			// butLeft
			// 
			this.butLeft.Image = ((System.Drawing.Image)(resources.GetObject("butLeft.Image")));
			this.butLeft.Location = new System.Drawing.Point(447, 571);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(35, 24);
			this.butLeft.TabIndex = 65;
			this.butLeft.UseVisualStyleBackColor = true;
			this.butLeft.Click += new System.EventHandler(this.ButLeft_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(501, 422);
			this.label3.Name = "label3";
			this.label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.label3.Size = new System.Drawing.Size(142, 18);
			this.label3.TabIndex = 66;
			this.label3.Text = "Linked Manual Pages";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listBoxLinkedPages
			// 
			this.listBoxLinkedPages.Location = new System.Drawing.Point(504, 442);
			this.listBoxLinkedPages.Name = "listBoxLinkedPages";
			this.listBoxLinkedPages.Size = new System.Drawing.Size(186, 238);
			this.listBoxLinkedPages.TabIndex = 67;
			// 
			// butDelete
			// 
			this.butDelete.Image = ((System.Drawing.Image)(resources.GetObject("butDelete.Image")));
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 662);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 23);
			this.butDelete.TabIndex = 68;
			this.butDelete.Text = "Delete";
			this.butDelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.ButDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Location = new System.Drawing.Point(1097, 626);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 69;
			this.butOK.Text = "&OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.ButOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Location = new System.Drawing.Point(1097, 656);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 70;
			this.butCancel.Text = "&Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butBullet
			// 
			this.butBullet.Image = global::OpenDental.Properties.Resources.Bullets_24x24;
			this.butBullet.Location = new System.Drawing.Point(278, 54);
			this.butBullet.Name = "butBullet";
			this.butBullet.Size = new System.Drawing.Size(24, 24);
			this.butBullet.TabIndex = 180;
			this.butBullet.Click += new System.EventHandler(this.butBullet_Click);
			// 
			// webPreview
			// 
			this.webPreview.IsWebBrowserContextMenuEnabled = false;
			this.webPreview.Location = new System.Drawing.Point(719, 41);
			this.webPreview.MinimumSize = new System.Drawing.Size(20, 20);
			this.webPreview.Name = "webPreview";
			this.webPreview.Size = new System.Drawing.Size(453, 579);
			this.webPreview.TabIndex = 181;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(716, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 14);
			this.label2.TabIndex = 182;
			this.label2.Text = "Preview";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormFaqEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1184, 696);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.webPreview);
			this.Controls.Add(this.butBullet);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.listBoxLinkedPages);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butLeft);
			this.Controls.Add(this.butRight);
			this.Controls.Add(this.listAvailableManualPages);
			this.Controls.Add(this.textLinkPage);
			this.Controls.Add(this.labelManualPageLink);
			this.Controls.Add(this.textEmbeddedMediaURL);
			this.Controls.Add(this.labelEmbeddedMediaUrl);
			this.Controls.Add(this.textImagePath);
			this.Controls.Add(this.labelImagePath);
			this.Controls.Add(this.checkSticky);
			this.Controls.Add(this.butVersionPick);
			this.Controls.Add(this.textManualVersion);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonHyperlink);
			this.Controls.Add(this.buttonUnderline);
			this.Controls.Add(this.buttonItalics);
			this.Controls.Add(this.buttonBold);
			this.Controls.Add(this.textAnswer);
			this.Controls.Add(this.labelAnswerText);
			this.Controls.Add(this.textQuestion);
			this.Controls.Add(this.labelQuestion);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFaqEdit";
			this.Text = "FAQ Edit";
			this.Load += new System.EventHandler(this.FormFaqEdit_Load);
			this.Shown += new System.EventHandler(this.FormFaqEdit_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelQuestion;
		private System.Windows.Forms.TextBox textQuestion;
		private System.Windows.Forms.Label labelAnswerText;
		private ODtextBox textAnswer;
		private System.Windows.Forms.Button buttonBold;
		private System.Windows.Forms.Button buttonItalics;
		private System.Windows.Forms.Button buttonUnderline;
		private System.Windows.Forms.Button buttonHyperlink;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textManualVersion;
		private System.Windows.Forms.Button butVersionPick;
		private System.Windows.Forms.CheckBox checkSticky;
		private System.Windows.Forms.Label labelImagePath;
		private System.Windows.Forms.TextBox textImagePath;
		private System.Windows.Forms.Label labelEmbeddedMediaUrl;
		private System.Windows.Forms.TextBox textEmbeddedMediaURL;
		private System.Windows.Forms.Label labelManualPageLink;
		private System.Windows.Forms.TextBox textLinkPage;
		private OpenDental.UI.ListBoxOD listAvailableManualPages;
		private System.Windows.Forms.Button butRight;
		private System.Windows.Forms.Button butLeft;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.ListBoxOD listBoxLinkedPages;
		private System.Windows.Forms.Button butDelete;
		private System.Windows.Forms.Button butOK;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butBullet;
		private System.Windows.Forms.WebBrowser webPreview;
		private System.Windows.Forms.Label label2;
	}
}