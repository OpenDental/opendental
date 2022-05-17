namespace OpenDental{
	partial class FormEmailEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEmailEdit));
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.textContentEmail = new OpenDental.ODcodeBox();
			this.webBrowserEmail = new System.Windows.Forms.WebBrowser();
			this.butOk = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.toolBarBottom = new OpenDental.UI.ToolBarOD();
			this.toolBarTop = new OpenDental.UI.ToolBarOD();
			this.labelPreview = new System.Windows.Forms.Label();
			this.labelPlainText = new System.Windows.Forms.Label();
			this.menuAutographDropdown = new System.Windows.Forms.ContextMenu();
			this.checkIsRaw = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// imageListMain
			// 
			this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
			this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMain.Images.SetKeyName(0, "refresh.gif");
			this.imageListMain.Images.SetKeyName(1, "save.gif");
			this.imageListMain.Images.SetKeyName(2, "cancel.gif");
			this.imageListMain.Images.SetKeyName(3, "cut.gif");
			this.imageListMain.Images.SetKeyName(4, "copy.gif");
			this.imageListMain.Images.SetKeyName(5, "paste.gif");
			this.imageListMain.Images.SetKeyName(6, "undo.gif");
			this.imageListMain.Images.SetKeyName(7, "link.gif");
			this.imageListMain.Images.SetKeyName(8, "linkExternal.gif");
			this.imageListMain.Images.SetKeyName(9, "h1.gif");
			this.imageListMain.Images.SetKeyName(10, "h2.gif");
			this.imageListMain.Images.SetKeyName(11, "h3.gif");
			this.imageListMain.Images.SetKeyName(12, "bold.gif");
			this.imageListMain.Images.SetKeyName(13, "italic.gif");
			this.imageListMain.Images.SetKeyName(14, "color.gif");
			this.imageListMain.Images.SetKeyName(15, "table.gif");
			this.imageListMain.Images.SetKeyName(16, "image.gif");
			this.imageListMain.Images.SetKeyName(17, "fontbutton.gif");
			this.imageListMain.Images.SetKeyName(18, "SaveDraft_2.png");
			this.imageListMain.Images.SetKeyName(19, "setupGears.gif");
			// 
			// textContentEmail
			// 
			this.textContentEmail.AcceptsTab = true;
			this.textContentEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.textContentEmail.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textContentEmail.Location = new System.Drawing.Point(0, 56);
			this.textContentEmail.Name = "textContentEmail";
			this.textContentEmail.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textContentEmail.Size = new System.Drawing.Size(616, 592);
			this.textContentEmail.TabIndex = 86;
			this.textContentEmail.Text = "";
			this.textContentEmail.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textContentEmail_KeyPress);
			this.textContentEmail.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.textContentEmail_MouseDoubleClick);
			// 
			// webBrowserEmail
			// 
			this.webBrowserEmail.AllowWebBrowserDrop = false;
			this.webBrowserEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webBrowserEmail.Location = new System.Drawing.Point(622, 56);
			this.webBrowserEmail.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowserEmail.Name = "webBrowserEmail";
			this.webBrowserEmail.Size = new System.Drawing.Size(606, 592);
			this.webBrowserEmail.TabIndex = 86;
			this.webBrowserEmail.WebBrowserShortcutsEnabled = false;
			this.webBrowserEmail.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowserEmail_Navigated);
			this.webBrowserEmail.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowserEmail_Navigating);
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.Location = new System.Drawing.Point(1050, 659);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(75, 25);
			this.butOk.TabIndex = 87;
			this.butOk.Text = "&OK";
			this.butOk.Click += new System.EventHandler(this.butOk_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(1134, 659);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 25);
			this.butCancel.TabIndex = 86;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// toolBarBottom
			// 
			this.toolBarBottom.Dock = System.Windows.Forms.DockStyle.Top;
			this.toolBarBottom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.toolBarBottom.ImageList = this.imageListMain;
			this.toolBarBottom.Location = new System.Drawing.Point(0, 25);
			this.toolBarBottom.Name = "toolBarBottom";
			this.toolBarBottom.Size = new System.Drawing.Size(1230, 25);
			this.toolBarBottom.TabIndex = 83;
			this.toolBarBottom.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.toolBarBottom_ButtonClick);
			// 
			// toolBarTop
			// 
			this.toolBarTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.toolBarTop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.toolBarTop.ImageList = this.imageListMain;
			this.toolBarTop.Location = new System.Drawing.Point(0, 0);
			this.toolBarTop.Name = "toolBarTop";
			this.toolBarTop.Size = new System.Drawing.Size(1230, 25);
			this.toolBarTop.TabIndex = 4;
			this.toolBarTop.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.toolBarTop_ButtonClick);
			// 
			// labelPreview
			// 
			this.labelPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelPreview.Location = new System.Drawing.Point(626, 650);
			this.labelPreview.Name = "labelPreview";
			this.labelPreview.Size = new System.Drawing.Size(63, 18);
			this.labelPreview.TabIndex = 89;
			this.labelPreview.Text = "Preview";
			this.labelPreview.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelPlainText
			// 
			this.labelPlainText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelPlainText.Location = new System.Drawing.Point(25, 650);
			this.labelPlainText.Name = "labelPlainText";
			this.labelPlainText.Size = new System.Drawing.Size(163, 18);
			this.labelPlainText.TabIndex = 88;
			this.labelPlainText.Text = "Plain Text/HTML";
			this.labelPlainText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkIsRaw
			// 
			this.checkIsRaw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkIsRaw.Location = new System.Drawing.Point(28, 671);
			this.checkIsRaw.Name = "checkIsRaw";
			this.checkIsRaw.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.checkIsRaw.Size = new System.Drawing.Size(319, 18);
			this.checkIsRaw.TabIndex = 90;
			this.checkIsRaw.Text = "Use Raw HTML (don\'t use master template)";
			this.checkIsRaw.UseVisualStyleBackColor = true;
			this.checkIsRaw.CheckedChanged += new System.EventHandler(this.checkIsRaw_CheckedChanged);
			// 
			// FormEmailEdit
			// 
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.checkIsRaw);
			this.Controls.Add(this.textContentEmail);
			this.Controls.Add(this.webBrowserEmail);
			this.Controls.Add(this.toolBarBottom);
			this.Controls.Add(this.toolBarTop);
			this.Controls.Add(this.labelPreview);
			this.Controls.Add(this.labelPlainText);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEmailEdit";
			this.Text = "Email HTML Edit";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEmailEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormEmailEdit_Load);
			this.SizeChanged += new System.EventHandler(this.FormEmailEdit_SizeChanged);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.ToolBarOD toolBarTop;
		private UI.ToolBarOD toolBarBottom;
		private System.Windows.Forms.ImageList imageListMain;
		private UI.Button butCancel;
		private UI.Button butOk;
		private ODcodeBox textContentEmail;
		private System.Windows.Forms.WebBrowser webBrowserEmail;
		private System.Windows.Forms.Label labelPreview;
		private System.Windows.Forms.Label labelPlainText;
		private System.Windows.Forms.ContextMenu menuAutographDropdown;
		private System.Windows.Forms.CheckBox checkIsRaw;
	}
}