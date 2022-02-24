namespace OpenDental{
	partial class FormWikiAllPages {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWikiAllPages));
            this.butOK = new OpenDental.UI.Button();
            this.butCancel = new OpenDental.UI.Button();
            this.webBrowserWiki = new System.Windows.Forms.WebBrowser();
            this.gridMain = new OpenDental.UI.GridOD();
            this.textSearch = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.butBrackets = new OpenDental.UI.Button();
            this.butAdd = new OpenDental.UI.Button();
            this.checkIncludeArchived = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // butOK
            // 
            this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butOK.Location = new System.Drawing.Point(925, 617);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(75, 24);
            this.butOK.TabIndex = 3;
            this.butOK.Text = "&OK";
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // butCancel
            // 
            this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butCancel.Location = new System.Drawing.Point(925, 647);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(75, 24);
            this.butCancel.TabIndex = 2;
            this.butCancel.Text = "&Cancel";
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // webBrowserWiki
            // 
            this.webBrowserWiki.AllowWebBrowserDrop = false;
            this.webBrowserWiki.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowserWiki.IsWebBrowserContextMenuEnabled = false;
            this.webBrowserWiki.Location = new System.Drawing.Point(266, 32);
            this.webBrowserWiki.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserWiki.Name = "webBrowserWiki";
            this.webBrowserWiki.Size = new System.Drawing.Size(653, 639);
            this.webBrowserWiki.TabIndex = 9;
            this.webBrowserWiki.WebBrowserShortcutsEnabled = false;
            this.webBrowserWiki.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowserWiki_Navigated);
            // 
            // gridMain
            // 
            this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gridMain.Location = new System.Drawing.Point(12, 32);
            this.gridMain.Name = "gridMain";
            this.gridMain.Size = new System.Drawing.Size(248, 639);
            this.gridMain.TabIndex = 8;
            this.gridMain.Title = "All Wiki Pages";
            this.gridMain.TranslationName = "TableWikiHistory";
            this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
            this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
            // 
            // textSearch
            // 
            this.textSearch.Location = new System.Drawing.Point(160, 6);
            this.textSearch.Name = "textSearch";
            this.textSearch.Size = new System.Drawing.Size(100, 20);
            this.textSearch.TabIndex = 0;
            this.textSearch.TextChanged += new System.EventHandler(this.textSearch_TextChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(72, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 18);
            this.label1.TabIndex = 11;
            this.label1.Text = "Search Titles";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // butBrackets
            // 
            this.butBrackets.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butBrackets.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.butBrackets.Location = new System.Drawing.Point(925, 587);
            this.butBrackets.Name = "butBrackets";
            this.butBrackets.Size = new System.Drawing.Size(75, 24);
            this.butBrackets.TabIndex = 12;
            this.butBrackets.Text = "[[  ]]";
            this.butBrackets.Click += new System.EventHandler(this.butBrackets_Click);
            // 
            // butAdd
            // 
            this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butAdd.Location = new System.Drawing.Point(925, 557);
            this.butAdd.Name = "butAdd";
            this.butAdd.Size = new System.Drawing.Size(75, 24);
            this.butAdd.TabIndex = 13;
            this.butAdd.Text = "Add";
            this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
            // 
            // checkIncludeArchived
            // 
            this.checkIncludeArchived.Location = new System.Drawing.Point(266, 6);
            this.checkIncludeArchived.Name = "checkIncludeArchived";
            this.checkIncludeArchived.Size = new System.Drawing.Size(177, 17);
            this.checkIncludeArchived.TabIndex = 14;
            this.checkIncludeArchived.Text = "Include Archived Pages";
            this.checkIncludeArchived.UseVisualStyleBackColor = true;
            this.checkIncludeArchived.CheckedChanged += new System.EventHandler(this.checkIncludeArchived_CheckedChanged);
            // 
            // FormWikiAllPages
            // 
            this.ClientSize = new System.Drawing.Size(1012, 683);
            this.Controls.Add(this.checkIncludeArchived);
            this.Controls.Add(this.butAdd);
            this.Controls.Add(this.butBrackets);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textSearch);
            this.Controls.Add(this.webBrowserWiki);
            this.Controls.Add(this.gridMain);
            this.Controls.Add(this.butOK);
            this.Controls.Add(this.butCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormWikiAllPages";
            this.Text = "All Wiki Pages";
            this.Load += new System.EventHandler(this.FormWikiAllPages_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.WebBrowser webBrowserWiki;
		private UI.GridOD gridMain;
		private System.Windows.Forms.TextBox textSearch;
		private System.Windows.Forms.Label label1;
		private UI.Button butBrackets;
		private UI.Button butAdd;
        private System.Windows.Forms.CheckBox checkIncludeArchived;
    }
}