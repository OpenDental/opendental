namespace OpenDental{
	partial class FormCloudUsers {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCloudUsers));
			this.gridUsers = new OpenDental.UI.GridOD();
			this.butAdd = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.labelSearch = new System.Windows.Forms.Label();
			this.textSearch = new System.Windows.Forms.TextBox();
			this.butClose = new OpenDental.UI.Button();
			this.butRefresh = new OpenDental.UI.Button();
			this.checkShowUserId = new OpenDental.UI.CheckBox();
			this.SuspendLayout();
			// 
			// gridUsers
			// 
			this.gridUsers.AllowSortingByColumn = true;
			this.gridUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridUsers.Location = new System.Drawing.Point(12, 38);
			this.gridUsers.Name = "gridUsers";
			this.gridUsers.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridUsers.Size = new System.Drawing.Size(1150, 461);
			this.gridUsers.TabIndex = 8;
			this.gridUsers.Title = "Users";
			this.gridUsers.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridUsers_CellDoubleClick);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(1168, 38);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(90, 24);
			this.butAdd.TabIndex = 3;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(1168, 68);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(90, 24);
			this.butDelete.TabIndex = 4;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// labelSearch
			// 
			this.labelSearch.Location = new System.Drawing.Point(12, 14);
			this.labelSearch.Name = "labelSearch";
			this.labelSearch.Size = new System.Drawing.Size(55, 17);
			this.labelSearch.TabIndex = 47;
			this.labelSearch.Text = "Search";
			this.labelSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSearch
			// 
			this.textSearch.Location = new System.Drawing.Point(68, 12);
			this.textSearch.Name = "textSearch";
			this.textSearch.Size = new System.Drawing.Size(181, 20);
			this.textSearch.TabIndex = 0;
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(1168, 475);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(90, 24);
			this.butClose.TabIndex = 7;
			this.butClose.Text = "&Close";
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(255, 10);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(90, 24);
			this.butRefresh.TabIndex = 1;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// checkShowUserId
			// 
			this.checkShowUserId.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowUserId.Location = new System.Drawing.Point(360, 13);
			this.checkShowUserId.Name = "checkShowUserId";
			this.checkShowUserId.Size = new System.Drawing.Size(120, 18);
			this.checkShowUserId.TabIndex = 48;
			this.checkShowUserId.Text = "Show User ID";
			this.checkShowUserId.CheckedChanged += new System.EventHandler(this.checkShowUserId_CheckedChanged);
			// 
			// FormCloudUsers
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(1270, 511);
			this.Controls.Add(this.checkShowUserId);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.labelSearch);
			this.Controls.Add(this.textSearch);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.gridUsers);
			this.Controls.Add(this.butAdd);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCloudUsers";
			this.Text = "Cloud Users";
			this.Load += new System.EventHandler(this.FormCloudUsers_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private UI.GridOD gridUsers;
		private UI.Button butAdd;
		private UI.Button butDelete;
		private System.Windows.Forms.Label labelSearch;
		private System.Windows.Forms.TextBox textSearch;
		private UI.Button butClose;
		private UI.Button butRefresh;
		private UI.CheckBox checkShowUserId;
	}
}