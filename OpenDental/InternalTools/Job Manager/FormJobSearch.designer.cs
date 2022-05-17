namespace OpenDental{
	partial class FormJobSearch {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJobSearch));
			this.label2 = new System.Windows.Forms.Label();
			this.textSearch = new System.Windows.Forms.TextBox();
			this.listBoxUsers = new OpenDental.UI.ListBoxOD();
			this.listBoxPhases = new OpenDental.UI.ListBoxOD();
			this.listBoxCategory = new OpenDental.UI.ListBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butSearch = new OpenDental.UI.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.dateTo = new System.Windows.Forms.DateTimePicker();
			this.dateFrom = new System.Windows.Forms.DateTimePicker();
			this.label7 = new System.Windows.Forms.Label();
			this.listBoxPriorities = new OpenDental.UI.ListBoxOD();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 11);
			this.label2.Margin = new System.Windows.Forms.Padding(0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(77, 20);
			this.label2.TabIndex = 229;
			this.label2.Text = "Search Terms";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSearch
			// 
			this.textSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSearch.Location = new System.Drawing.Point(89, 12);
			this.textSearch.Name = "textSearch";
			this.textSearch.Size = new System.Drawing.Size(1244, 20);
			this.textSearch.TabIndex = 228;
			// 
			// listBoxUsers
			// 
			this.listBoxUsers.Location = new System.Drawing.Point(89, 243);
			this.listBoxUsers.Name = "listBoxUsers";
			this.listBoxUsers.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxUsers.Size = new System.Drawing.Size(144, 134);
			this.listBoxUsers.TabIndex = 230;
			// 
			// listBoxPhases
			// 
			this.listBoxPhases.Location = new System.Drawing.Point(89, 383);
			this.listBoxPhases.Name = "listBoxPhases";
			this.listBoxPhases.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxPhases.Size = new System.Drawing.Size(144, 108);
			this.listBoxPhases.TabIndex = 234;
			// 
			// listBoxCategory
			// 
			this.listBoxCategory.Location = new System.Drawing.Point(89, 497);
			this.listBoxCategory.Name = "listBoxCategory";
			this.listBoxCategory.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxCategory.Size = new System.Drawing.Size(144, 147);
			this.listBoxCategory.TabIndex = 245;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(31, 243);
			this.label3.Margin = new System.Windows.Forms.Padding(0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(55, 20);
			this.label3.TabIndex = 249;
			this.label3.Text = "Users";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(31, 383);
			this.label1.Margin = new System.Windows.Forms.Padding(0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 20);
			this.label1.TabIndex = 250;
			this.label1.Text = "Phase";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(31, 497);
			this.label5.Margin = new System.Windows.Forms.Padding(0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(55, 20);
			this.label5.TabIndex = 251;
			this.label5.Text = "Category";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HasMultilineHeaders = true;
			this.gridMain.Location = new System.Drawing.Point(239, 38);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(1180, 611);
			this.gridMain.TabIndex = 227;
			this.gridMain.Title = "Details Grid";
			this.gridMain.WrapText = false;
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(1252, 663);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(80, 24);
			this.butOK.TabIndex = 248;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(1339, 663);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(80, 24);
			this.butCancel.TabIndex = 247;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butSearch
			// 
			this.butSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSearch.Location = new System.Drawing.Point(1339, 9);
			this.butSearch.Name = "butSearch";
			this.butSearch.Size = new System.Drawing.Size(80, 24);
			this.butSearch.TabIndex = 252;
			this.butSearch.Text = "Search";
			this.butSearch.Click += new System.EventHandler(this.butSearch_Click);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(12, 61);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(75, 23);
			this.label6.TabIndex = 256;
			this.label6.Text = "To";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(15, 35);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 23);
			this.label4.TabIndex = 255;
			this.label4.Text = "From";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// dateTo
			// 
			this.dateTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateTo.Location = new System.Drawing.Point(89, 64);
			this.dateTo.Name = "dateTo";
			this.dateTo.Size = new System.Drawing.Size(144, 20);
			this.dateTo.TabIndex = 254;
			// 
			// dateFrom
			// 
			this.dateFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateFrom.Location = new System.Drawing.Point(89, 38);
			this.dateFrom.Name = "dateFrom";
			this.dateFrom.Size = new System.Drawing.Size(144, 20);
			this.dateFrom.TabIndex = 253;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(31, 90);
			this.label7.Margin = new System.Windows.Forms.Padding(0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(55, 20);
			this.label7.TabIndex = 258;
			this.label7.Text = "Priority";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listBoxPriorities
			// 
			this.listBoxPriorities.Location = new System.Drawing.Point(89, 90);
			this.listBoxPriorities.Name = "listBoxPriorities";
			this.listBoxPriorities.SelectionMode = UI.SelectionMode.MultiExtended;
			this.listBoxPriorities.Size = new System.Drawing.Size(144, 147);
			this.listBoxPriorities.TabIndex = 257;
			// 
			// FormJobSearch
			// 
			this.AcceptButton = this.butSearch;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1431, 699);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.listBoxPriorities);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.dateTo);
			this.Controls.Add(this.dateFrom);
			this.Controls.Add(this.butSearch);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.listBoxCategory);
			this.Controls.Add(this.listBoxPhases);
			this.Controls.Add(this.listBoxUsers);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textSearch);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormJobSearch";
			this.Text = "Search Jobs";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormJobSearch_FormClosing);
			this.Load += new System.EventHandler(this.FormJobSearch_Load);
			this.Shown += new System.EventHandler(this.FormJobSearch_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.GridOD gridMain;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textSearch;
		private OpenDental.UI.ListBoxOD listBoxUsers;
		private OpenDental.UI.ListBoxOD listBoxPhases;
		private OpenDental.UI.ListBoxOD listBoxCategory;
		private UI.Button butCancel;
		private UI.Button butOK;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label5;
		private UI.Button butSearch;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.DateTimePicker dateTo;
		private System.Windows.Forms.DateTimePicker dateFrom;
		private System.Windows.Forms.Label label7;
		private UI.ListBoxOD listBoxPriorities;
	}
}