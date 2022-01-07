namespace OpenDental{
	partial class FormWikiListEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWikiListEdit));
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.butHistory = new OpenDental.UI.Button();
			this.butRenameList = new OpenDental.UI.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.butAddItem = new OpenDental.UI.Button();
			this.labelSearch = new System.Windows.Forms.Label();
			this.textSearch = new System.Windows.Forms.TextBox();
			this.butDelete = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butColumnDelete = new OpenDental.UI.Button();
			this.butHeaders = new OpenDental.UI.Button();
			this.butColumnInsert = new OpenDental.UI.Button();
			this.butColumnRight = new OpenDental.UI.Button();
			this.butColumnLeft = new OpenDental.UI.Button();
			this.checkBoxIncludeHiddenColumns = new System.Windows.Forms.CheckBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butClose = new OpenDental.UI.Button();
			this.butAdvSearch = new OpenDental.UI.Button();
			this.butClearAdvSearch = new OpenDental.UI.Button();
			this.radioButHighlight = new System.Windows.Forms.RadioButton();
			this.radioButFilter = new System.Windows.Forms.RadioButton();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.butHistory);
			this.groupBox3.Controls.Add(this.butRenameList);
			this.groupBox3.Location = new System.Drawing.Point(861, 46);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(87, 81);
			this.groupBox3.TabIndex = 6;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "List";
			// 
			// butHistory
			// 
			this.butHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butHistory.Location = new System.Drawing.Point(8, 49);
			this.butHistory.Name = "butHistory";
			this.butHistory.Size = new System.Drawing.Size(71, 24);
			this.butHistory.TabIndex = 1;
			this.butHistory.Text = "History";
			this.butHistory.Click += new System.EventHandler(this.butHistory_Click);
			// 
			// butRenameList
			// 
			this.butRenameList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRenameList.Location = new System.Drawing.Point(8, 19);
			this.butRenameList.Name = "butRenameList";
			this.butRenameList.Size = new System.Drawing.Size(71, 24);
			this.butRenameList.TabIndex = 0;
			this.butRenameList.Text = "Rename";
			this.butRenameList.Click += new System.EventHandler(this.butRenameList_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.butAddItem);
			this.groupBox2.Location = new System.Drawing.Point(861, 280);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(87, 51);
			this.groupBox2.TabIndex = 8;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Rows";
			// 
			// butAddItem
			// 
			this.butAddItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddItem.Location = new System.Drawing.Point(8, 19);
			this.butAddItem.Name = "butAddItem";
			this.butAddItem.Size = new System.Drawing.Size(71, 24);
			this.butAddItem.TabIndex = 31;
			this.butAddItem.Text = "Add";
			this.butAddItem.Click += new System.EventHandler(this.butAddItem_Click);
			// 
			// labelSearch
			// 
			this.labelSearch.Location = new System.Drawing.Point(12, 15);
			this.labelSearch.Name = "labelSearch";
			this.labelSearch.Size = new System.Drawing.Size(166, 16);
			this.labelSearch.TabIndex = 38;
			this.labelSearch.Text = "Search";
			this.labelSearch.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textSearch
			// 
			this.textSearch.Location = new System.Drawing.Point(184, 13);
			this.textSearch.Name = "textSearch";
			this.textSearch.Size = new System.Drawing.Size(218, 20);
			this.textSearch.TabIndex = 0;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 592);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(71, 24);
			this.butDelete.TabIndex = 9;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.butColumnDelete);
			this.groupBox1.Controls.Add(this.butHeaders);
			this.groupBox1.Controls.Add(this.butColumnInsert);
			this.groupBox1.Controls.Add(this.butColumnRight);
			this.groupBox1.Controls.Add(this.butColumnLeft);
			this.groupBox1.Location = new System.Drawing.Point(861, 133);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(87, 141);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Columns";
			// 
			// butColumnDelete
			// 
			this.butColumnDelete.Location = new System.Drawing.Point(8, 109);
			this.butColumnDelete.Name = "butColumnDelete";
			this.butColumnDelete.Size = new System.Drawing.Size(71, 24);
			this.butColumnDelete.TabIndex = 34;
			this.butColumnDelete.Text = "Delete";
			this.butColumnDelete.Click += new System.EventHandler(this.butColumnDelete_Click);
			// 
			// butHeaders
			// 
			this.butHeaders.Location = new System.Drawing.Point(8, 49);
			this.butHeaders.Name = "butHeaders";
			this.butHeaders.Size = new System.Drawing.Size(71, 24);
			this.butHeaders.TabIndex = 31;
			this.butHeaders.Text = "Edit";
			this.butHeaders.Click += new System.EventHandler(this.butColumnEdit_Click);
			// 
			// butColumnInsert
			// 
			this.butColumnInsert.Location = new System.Drawing.Point(8, 79);
			this.butColumnInsert.Name = "butColumnInsert";
			this.butColumnInsert.Size = new System.Drawing.Size(71, 24);
			this.butColumnInsert.TabIndex = 33;
			this.butColumnInsert.Text = "Add";
			this.butColumnInsert.Click += new System.EventHandler(this.butColumnAdd_Click);
			// 
			// butColumnRight
			// 
			this.butColumnRight.Location = new System.Drawing.Point(49, 19);
			this.butColumnRight.Name = "butColumnRight";
			this.butColumnRight.Size = new System.Drawing.Size(30, 24);
			this.butColumnRight.TabIndex = 30;
			this.butColumnRight.Text = "R";
			this.butColumnRight.Click += new System.EventHandler(this.butColumnRight_Click);
			// 
			// butColumnLeft
			// 
			this.butColumnLeft.Location = new System.Drawing.Point(8, 19);
			this.butColumnLeft.Name = "butColumnLeft";
			this.butColumnLeft.Size = new System.Drawing.Size(30, 24);
			this.butColumnLeft.TabIndex = 29;
			this.butColumnLeft.Text = "L";
			this.butColumnLeft.Click += new System.EventHandler(this.butColumnLeft_Click);
			// 
			// checkBoxIncludeHiddenColumns
			// 
			this.checkBoxIncludeHiddenColumns.Location = new System.Drawing.Point(768, 15);
			this.checkBoxIncludeHiddenColumns.Name = "checkBoxIncludeHiddenColumns";
			this.checkBoxIncludeHiddenColumns.Size = new System.Drawing.Size(180, 19);
			this.checkBoxIncludeHiddenColumns.TabIndex = 35;
			this.checkBoxIncludeHiddenColumns.Text = "Include Hidden Columns";
			this.checkBoxIncludeHiddenColumns.UseVisualStyleBackColor = true;
			this.checkBoxIncludeHiddenColumns.CheckedChanged += new System.EventHandler(this.checkBoxIncludeHiddenColumns_CheckedChanged);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.EditableAcceptsCR = true;
			this.gridMain.HasAutoWrappedHeaders = true;
			this.gridMain.HasMultilineHeaders = true;
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(12, 46);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridMain.Size = new System.Drawing.Size(843, 540);
			this.gridMain.TabIndex = 5;
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(869, 592);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(71, 24);
			this.butClose.TabIndex = 10;
			this.butClose.Text = "Close";
			// 
			// butAdvSearch
			// 
			this.butAdvSearch.Location = new System.Drawing.Point(408, 12);
			this.butAdvSearch.Name = "butAdvSearch";
			this.butAdvSearch.Size = new System.Drawing.Size(75, 23);
			this.butAdvSearch.TabIndex = 1;
			this.butAdvSearch.Text = "Adv. Search";
			this.butAdvSearch.UseVisualStyleBackColor = true;
			this.butAdvSearch.Click += new System.EventHandler(this.butAdvSearch_Click);
			// 
			// butClearAdvSearch
			// 
			this.butClearAdvSearch.Location = new System.Drawing.Point(489, 12);
			this.butClearAdvSearch.Name = "butClearAdvSearch";
			this.butClearAdvSearch.Size = new System.Drawing.Size(75, 23);
			this.butClearAdvSearch.TabIndex = 2;
			this.butClearAdvSearch.Text = "Clear";
			this.butClearAdvSearch.UseVisualStyleBackColor = true;
			this.butClearAdvSearch.Click += new System.EventHandler(this.butClearAdvSearch_Click);
			// 
			// radioButHighlight
			// 
			this.radioButHighlight.Checked = true;
			this.radioButHighlight.Location = new System.Drawing.Point(594, 15);
			this.radioButHighlight.Name = "radioButHighlight";
			this.radioButHighlight.Size = new System.Drawing.Size(89, 17);
			this.radioButHighlight.TabIndex = 3;
			this.radioButHighlight.TabStop = true;
			this.radioButHighlight.Text = "Highlight";
			this.radioButHighlight.UseVisualStyleBackColor = true;
			this.radioButHighlight.CheckedChanged += new System.EventHandler(this.radioButHighlight_CheckedChanged);
			// 
			// radioButFilter
			// 
			this.radioButFilter.Location = new System.Drawing.Point(689, 15);
			this.radioButFilter.Name = "radioButFilter";
			this.radioButFilter.Size = new System.Drawing.Size(73, 17);
			this.radioButFilter.TabIndex = 4;
			this.radioButFilter.Text = "Filter";
			this.radioButFilter.UseVisualStyleBackColor = true;
			this.radioButFilter.CheckedChanged += new System.EventHandler(this.radioButFilter_CheckedChanged);
			// 
			// FormWikiListEdit
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(952, 628);
			this.Controls.Add(this.radioButFilter);
			this.Controls.Add(this.checkBoxIncludeHiddenColumns);
			this.Controls.Add(this.radioButHighlight);
			this.Controls.Add(this.butClearAdvSearch);
			this.Controls.Add(this.butAdvSearch);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.labelSearch);
			this.Controls.Add(this.textSearch);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWikiListEdit";
			this.Text = "Edit Wiki List";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.FormWikiListEdit_Load);
			this.groupBox3.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.Button butClose;
		private UI.GridOD gridMain;
		private System.Windows.Forms.GroupBox groupBox1;
		private UI.Button butHeaders;
		private UI.Button butColumnRight;
		private UI.Button butColumnLeft;
		private UI.Button butAddItem;
		private UI.Button butColumnDelete;
		private UI.Button butColumnInsert;
		private UI.Button butDelete;
		private System.Windows.Forms.Label labelSearch;
		private System.Windows.Forms.TextBox textSearch;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private UI.Button butRenameList;
		private UI.Button butHistory;
		private UI.Button butAdvSearch;
		private UI.Button butClearAdvSearch;
		private System.Windows.Forms.RadioButton radioButHighlight;
		private System.Windows.Forms.RadioButton radioButFilter;
		private System.Windows.Forms.CheckBox checkBoxIncludeHiddenColumns;
	}
}