namespace OpenDental{
	partial class FormBugSearch {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBugSearch));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textFilter = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkToken = new System.Windows.Forms.CheckBox();
			this.butRefresh = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.checkShow = new System.Windows.Forms.CheckBox();
			this.butAdd = new OpenDental.UI.Button();
			this.butViewSubs = new OpenDental.UI.Button();
			this.checkMobile = new System.Windows.Forms.CheckBox();
			this.butAddMobile = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(767, 516);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(848, 516);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textFilter
			// 
			this.textFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textFilter.Location = new System.Drawing.Point(69, 12);
			this.textFilter.Name = "textFilter";
			this.textFilter.Size = new System.Drawing.Size(359, 20);
			this.textFilter.TabIndex = 29;
			this.textFilter.TextChanged += new System.EventHandler(this.textFilter_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 12);
			this.label1.TabIndex = 32;
			this.label1.Text = "Search";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkToken
			// 
			this.checkToken.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkToken.Location = new System.Drawing.Point(443, 13);
			this.checkToken.Name = "checkToken";
			this.checkToken.Size = new System.Drawing.Size(149, 18);
			this.checkToken.TabIndex = 33;
			this.checkToken.Text = "Tokenize Search Terms";
			this.checkToken.UseVisualStyleBackColor = true;
			this.checkToken.CheckedChanged += new System.EventHandler(this.checkToken_CheckedChanged);
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(848, 10);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 34;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 38);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(911, 472);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = null;
			this.gridMain.TranslationName = "TableBugs";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// checkShow
			// 
			this.checkShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShow.Location = new System.Drawing.Point(598, 13);
			this.checkShow.Name = "checkShow";
			this.checkShow.Size = new System.Drawing.Size(116, 18);
			this.checkShow.TabIndex = 35;
			this.checkShow.Text = "Show Complete";
			this.checkShow.UseVisualStyleBackColor = true;
			this.checkShow.CheckedChanged += new System.EventHandler(this.checkShow_CheckedChanged);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(12, 516);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 36;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butViewSubs
			// 
			this.butViewSubs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butViewSubs.Location = new System.Drawing.Point(210, 516);
			this.butViewSubs.Name = "butViewSubs";
			this.butViewSubs.Size = new System.Drawing.Size(109, 24);
			this.butViewSubs.TabIndex = 37;
			this.butViewSubs.Text = "&View Submissions";
			this.butViewSubs.Click += new System.EventHandler(this.butViewSubs_Click);
			// 
			// checkMobile
			// 
			this.checkMobile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkMobile.Location = new System.Drawing.Point(720, 13);
			this.checkMobile.Name = "checkMobile";
			this.checkMobile.Size = new System.Drawing.Size(103, 18);
			this.checkMobile.TabIndex = 38;
			this.checkMobile.Text = "Show Mobile";
			this.checkMobile.UseVisualStyleBackColor = true;
			this.checkMobile.CheckedChanged += new System.EventHandler(this.checkMobile_CheckedChanged);
			// 
			// butAddMobile
			// 
			this.butAddMobile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddMobile.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddMobile.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddMobile.Location = new System.Drawing.Point(93, 516);
			this.butAddMobile.Name = "butAddMobile";
			this.butAddMobile.Size = new System.Drawing.Size(111, 24);
			this.butAddMobile.TabIndex = 39;
			this.butAddMobile.Text = "Add Mobile";
			this.butAddMobile.Click += new System.EventHandler(this.butAddMobile_Click);
			// 
			// FormBugSearch
			// 
			this.ClientSize = new System.Drawing.Size(935, 552);
			this.Controls.Add(this.butAddMobile);
			this.Controls.Add(this.checkMobile);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butViewSubs);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.checkShow);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.checkToken);
			this.Controls.Add(this.textFilter);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormBugSearch";
			this.Text = "Search Bugs";
			this.Load += new System.EventHandler(this.FormBugSearch_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridMain;
		private System.Windows.Forms.TextBox textFilter;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkToken;
		private UI.Button butRefresh;
		private System.Windows.Forms.CheckBox checkShow;
		private UI.Button butAdd;
		private UI.Button butViewSubs;
		private System.Windows.Forms.CheckBox checkMobile;
		private UI.Button butAddMobile;
	}
}