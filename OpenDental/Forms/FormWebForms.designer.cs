namespace OpenDental{
	partial class FormWebForms {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebForms));
			this.menuWebFormsRight = new System.Windows.Forms.ContextMenu();
			this.menuItemViewAllSheets = new System.Windows.Forms.MenuItem();
			this.groupFilters = new OpenDental.UI.GroupBox();
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.butRefresh = new OpenDental.UI.Button();
			this.butToday = new OpenDental.UI.Button();
			this.textDateStart = new OpenDental.ValidDate();
			this.labelStartDate = new System.Windows.Forms.Label();
			this.labelEndDate = new System.Windows.Forms.Label();
			this.textDateEnd = new OpenDental.ValidDate();
			this.label1 = new System.Windows.Forms.Label();
			this.butRetrieve = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butClose = new OpenDental.UI.Button();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.textBatchSize = new OpenDental.ValidNum();
			this.label2 = new System.Windows.Forms.Label();
			this.groupFilters.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuWebFormsRight
			// 
			this.menuWebFormsRight.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemViewAllSheets});
			this.menuWebFormsRight.Popup += new System.EventHandler(this.menuWebFormsRight_Popup);
			// 
			// menuItemViewAllSheets
			// 
			this.menuItemViewAllSheets.Index = 0;
			this.menuItemViewAllSheets.Text = "View this patient\'s forms";
			this.menuItemViewAllSheets.Click += new System.EventHandler(this.menuItemViewAllSheets_Click);
			// 
			// groupFilters
			// 
			this.groupFilters.Controls.Add(this.comboClinics);
			this.groupFilters.Controls.Add(this.butRefresh);
			this.groupFilters.Controls.Add(this.butToday);
			this.groupFilters.Controls.Add(this.textDateStart);
			this.groupFilters.Controls.Add(this.labelStartDate);
			this.groupFilters.Controls.Add(this.labelEndDate);
			this.groupFilters.Controls.Add(this.textDateEnd);
			this.groupFilters.Location = new System.Drawing.Point(12, 28);
			this.groupFilters.Name = "groupFilters";
			this.groupFilters.Size = new System.Drawing.Size(495, 69);
			this.groupFilters.TabIndex = 0;
			this.groupFilters.Text = "Show Retrieved Forms";
			// 
			// comboClinics
			// 
			this.comboClinics.HqDescription = "Headquarters";
			this.comboClinics.IncludeAll = true;
			this.comboClinics.IncludeUnassigned = true;
			this.comboClinics.Location = new System.Drawing.Point(275, 37);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.SelectionModeMulti = true;
			this.comboClinics.Size = new System.Drawing.Size(200, 21);
			this.comboClinics.TabIndex = 4;
			this.comboClinics.SelectionChangeCommitted += new System.EventHandler(this.ComboClinics_SelectionChangeCommitted);
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(163, 39);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(77, 24);
			this.butRefresh.TabIndex = 3;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// butToday
			// 
			this.butToday.Location = new System.Drawing.Point(163, 14);
			this.butToday.Name = "butToday";
			this.butToday.Size = new System.Drawing.Size(77, 24);
			this.butToday.TabIndex = 2;
			this.butToday.Text = "Today";
			this.butToday.Click += new System.EventHandler(this.butToday_Click);
			// 
			// textDateStart
			// 
			this.textDateStart.BackColor = System.Drawing.SystemColors.Window;
			this.textDateStart.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textDateStart.Location = new System.Drawing.Point(75, 16);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(77, 20);
			this.textDateStart.TabIndex = 0;
			// 
			// labelStartDate
			// 
			this.labelStartDate.Location = new System.Drawing.Point(2, 19);
			this.labelStartDate.Name = "labelStartDate";
			this.labelStartDate.Size = new System.Drawing.Size(69, 14);
			this.labelStartDate.TabIndex = 221;
			this.labelStartDate.Text = "Start Date";
			this.labelStartDate.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelEndDate
			// 
			this.labelEndDate.Location = new System.Drawing.Point(2, 44);
			this.labelEndDate.Name = "labelEndDate";
			this.labelEndDate.Size = new System.Drawing.Size(69, 14);
			this.labelEndDate.TabIndex = 222;
			this.labelEndDate.Text = "End Date";
			this.labelEndDate.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDateEnd
			// 
			this.textDateEnd.Location = new System.Drawing.Point(75, 41);
			this.textDateEnd.Name = "textDateEnd";
			this.textDateEnd.Size = new System.Drawing.Size(77, 20);
			this.textDateEnd.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.Location = new System.Drawing.Point(140, 300);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(521, 32);
			this.label1.TabIndex = 239;
			this.label1.Text = "Retrieved forms are automatically attached to the correct patient if they are a m" +
    "atch.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butRetrieve
			// 
			this.butRetrieve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butRetrieve.Location = new System.Drawing.Point(12, 305);
			this.butRetrieve.Name = "butRetrieve";
			this.butRetrieve.Size = new System.Drawing.Size(123, 24);
			this.butRetrieve.TabIndex = 2;
			this.butRetrieve.Text = "&Retrieve New Forms";
			this.butRetrieve.Click += new System.EventHandler(this.butRetrieve_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 103);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(731, 170);
			this.gridMain.TabIndex = 1;
			this.gridMain.Title = "Web Forms";
			this.gridMain.TranslationName = "TableWebforms";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(668, 305);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(755, 24);
			this.menuMain.TabIndex = 240;
			// 
			// textBatchSize
			// 
			this.textBatchSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBatchSize.Location = new System.Drawing.Point(12, 279);
			this.textBatchSize.MinVal = 1;
			this.textBatchSize.Name = "textBatchSize";
			this.textBatchSize.Size = new System.Drawing.Size(33, 20);
			this.textBatchSize.TabIndex = 1;
			this.textBatchSize.Text = "10";
			this.textBatchSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.Location = new System.Drawing.Point(51, 281);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(692, 14);
			this.label2.TabIndex = 242;
			this.label2.Text = "Batch size. All pending forms will be retrieved but are downloaded in batches. Sm" +
    "aller batch sizes are more likely to succeed.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormWebForms
			// 
			this.ClientSize = new System.Drawing.Size(755, 341);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBatchSize);
			this.Controls.Add(this.groupFilters);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butRetrieve);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWebForms";
			this.Text = "Web Forms";
			this.Load += new System.EventHandler(this.FormWebForms_Load);
			this.groupFilters.ResumeLayout(false);
			this.groupFilters.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.Button butRetrieve;
		private OpenDental.UI.GroupBox groupFilters;
		private OpenDental.UI.Button butToday;
		private ValidDate textDateStart;
		private System.Windows.Forms.Label labelStartDate;
		private System.Windows.Forms.Label labelEndDate;
		private ValidDate textDateEnd;
		private OpenDental.UI.Button butRefresh;
		private System.Windows.Forms.ContextMenu menuWebFormsRight;
		private System.Windows.Forms.MenuItem menuItemViewAllSheets;
		private System.Windows.Forms.Label label1;
		private UI.ComboBoxClinicPicker comboClinics;
		private UI.MenuOD menuMain;
		private ValidNum textBatchSize;
		private System.Windows.Forms.Label label2;
	}
}