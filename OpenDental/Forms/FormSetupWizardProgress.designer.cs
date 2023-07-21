namespace OpenDental{
	partial class FormSetupWizardProgress {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSetupWizardProgress));
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.labelTitle = new System.Windows.Forms.Label();
			this.butNext = new OpenDental.UI.Button();
			this.butBack = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butSkip = new OpenDental.UI.Button();
			this.panelContent = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "chevdoubleright.png");
			this.imageList1.Images.SetKeyName(1, "chevright.png");
			this.imageList1.Images.SetKeyName(2, "chevLeft.png");
			// 
			// labelTitle
			// 
			this.labelTitle.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTitle.Location = new System.Drawing.Point(2, 2);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(506, 25);
			this.labelTitle.TabIndex = 0;
			this.labelTitle.Text = "title text";
			this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butNext
			// 
			this.butNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butNext.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butNext.ImageIndex = 1;
			this.butNext.ImageList = this.imageList1;
			this.butNext.Location = new System.Drawing.Point(654, 555);
			this.butNext.Name = "butNext";
			this.butNext.Size = new System.Drawing.Size(75, 23);
			this.butNext.TabIndex = 1;
			this.butNext.Text = "Next";
			this.butNext.UseVisualStyleBackColor = true;
			this.butNext.Click += new System.EventHandler(this.butNext_Click);
			// 
			// butBack
			// 
			this.butBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butBack.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butBack.ImageIndex = 2;
			this.butBack.ImageList = this.imageList1;
			this.butBack.Location = new System.Drawing.Point(12, 555);
			this.butBack.Name = "butBack";
			this.butBack.Size = new System.Drawing.Size(75, 23);
			this.butBack.TabIndex = 4;
			this.butBack.Text = "Back";
			this.butBack.UseVisualStyleBackColor = true;
			this.butBack.Click += new System.EventHandler(this.butBack_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(843, 555);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butSkip
			// 
			this.butSkip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSkip.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butSkip.ImageIndex = 0;
			this.butSkip.ImageList = this.imageList1;
			this.butSkip.Location = new System.Drawing.Point(735, 555);
			this.butSkip.Name = "butSkip";
			this.butSkip.Size = new System.Drawing.Size(75, 23);
			this.butSkip.TabIndex = 2;
			this.butSkip.Text = "Skip";
			this.butSkip.UseVisualStyleBackColor = true;
			this.butSkip.Click += new System.EventHandler(this.butSkip_Click);
			// 
			// panelContent
			// 
			this.panelContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelContent.Location = new System.Drawing.Point(0, 30);
			this.panelContent.Name = "panelContent";
			this.panelContent.Size = new System.Drawing.Size(930, 519);
			this.panelContent.TabIndex = 1;
			// 
			// FormSetupWizardProgress
			// 
			this.ClientSize = new System.Drawing.Size(930, 590);
			this.Controls.Add(this.butNext);
			this.Controls.Add(this.panelContent);
			this.Controls.Add(this.butSkip);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butBack);
			this.Controls.Add(this.labelTitle);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSetupWizardProgress";
			this.Text = "Setup Items";
			this.Load += new System.EventHandler(this.FormSetupWizardProgress_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.ImageList imageList1;
		private UI.Button butClose;
		private UI.Button butSkip;
		private UI.Button butBack;
		private UI.Button butNext;
		private System.Windows.Forms.Label labelTitle;
		private System.Windows.Forms.Panel panelContent;
	}
}