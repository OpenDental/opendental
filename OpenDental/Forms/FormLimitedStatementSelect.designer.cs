namespace OpenDental{
	partial class FormLimitedStatementSelect {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLimitedStatementSelect));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butNone = new OpenDental.UI.Button();
			this.butAll = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.label1 = new System.Windows.Forms.Label();
			this.groupFilters = new System.Windows.Forms.GroupBox();
			this.labelTo = new System.Windows.Forms.Label();
			this.odDatePickerTo = new OpenDental.UI.ODDatePicker();
			this.labelFrom = new System.Windows.Forms.Label();
			this.odDatePickerFrom = new OpenDental.UI.ODDatePicker();
			this.listBoxTransTypes = new OpenDental.UI.ListBoxOD();
			this.labelTransTypes = new System.Windows.Forms.Label();
			this.butToday = new OpenDental.UI.Button();
			this.groupFilters.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(917, 630);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(917, 660);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butNone
			// 
			this.butNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butNone.Location = new System.Drawing.Point(765, 228);
			this.butNone.Name = "butNone";
			this.butNone.Size = new System.Drawing.Size(75, 24);
			this.butNone.TabIndex = 148;
			this.butNone.Text = "None";
			this.butNone.Click += new System.EventHandler(this.butNone_Click);
			// 
			// butAll
			// 
			this.butAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAll.Location = new System.Drawing.Point(765, 168);
			this.butAll.Name = "butAll";
			this.butAll.Size = new System.Drawing.Size(75, 24);
			this.butAll.TabIndex = 147;
			this.butAll.Text = "All";
			this.butAll.Click += new System.EventHandler(this.butAll_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 23);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(747, 661);
			this.gridMain.TabIndex = 146;
			this.gridMain.Title = "Limited Statement Items";
			this.gridMain.TranslationName = "TableLimitedStatementItems";
			this.gridMain.ColumnSorted += new System.EventHandler(this.gridMain_ColumnSorted);
			this.gridMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridMain_MouseUp);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(661, 16);
			this.label1.TabIndex = 145;
			this.label1.Text = "Select procedures, payments, adjustments, or claim payments to include on the lim" +
    "ited statement";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupFilters
			// 
			this.groupFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupFilters.Controls.Add(this.labelTo);
			this.groupFilters.Controls.Add(this.odDatePickerTo);
			this.groupFilters.Controls.Add(this.labelFrom);
			this.groupFilters.Controls.Add(this.odDatePickerFrom);
			this.groupFilters.Controls.Add(this.listBoxTransTypes);
			this.groupFilters.Controls.Add(this.labelTransTypes);
			this.groupFilters.Location = new System.Drawing.Point(765, 23);
			this.groupFilters.Name = "groupFilters";
			this.groupFilters.Size = new System.Drawing.Size(227, 139);
			this.groupFilters.TabIndex = 150;
			this.groupFilters.TabStop = false;
			this.groupFilters.Text = "Filters";
			// 
			// labelTo
			// 
			this.labelTo.Location = new System.Drawing.Point(6, 37);
			this.labelTo.Name = "labelTo";
			this.labelTo.Size = new System.Drawing.Size(96, 16);
			this.labelTo.TabIndex = 155;
			this.labelTo.Text = "To";
			this.labelTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// odDatePickerTo
			// 
			this.odDatePickerTo.AdjustCalendarLocation = new System.Drawing.Point(-40, 0);
			this.odDatePickerTo.BackColor = System.Drawing.Color.Transparent;
			this.odDatePickerTo.Location = new System.Drawing.Point(40, 34);
			this.odDatePickerTo.MaximumSize = new System.Drawing.Size(0, 184);
			this.odDatePickerTo.MinimumSize = new System.Drawing.Size(0, 23);
			this.odDatePickerTo.Name = "odDatePickerTo";
			this.odDatePickerTo.Size = new System.Drawing.Size(184, 23);
			this.odDatePickerTo.TabIndex = 154;
			this.odDatePickerTo.CalendarSelectionChanged += new OpenDental.UI.CalendarSelectionHandler(this.odDatePickerTo_CalendarSelectionChanged);
			// 
			// labelFrom
			// 
			this.labelFrom.Location = new System.Drawing.Point(6, 15);
			this.labelFrom.Name = "labelFrom";
			this.labelFrom.Size = new System.Drawing.Size(96, 16);
			this.labelFrom.TabIndex = 153;
			this.labelFrom.Text = "From";
			this.labelFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// odDatePickerFrom
			// 
			this.odDatePickerFrom.AdjustCalendarLocation = new System.Drawing.Point(-40, 0);
			this.odDatePickerFrom.BackColor = System.Drawing.Color.Transparent;
			this.odDatePickerFrom.Location = new System.Drawing.Point(40, 12);
			this.odDatePickerFrom.MaximumSize = new System.Drawing.Size(0, 184);
			this.odDatePickerFrom.MinimumSize = new System.Drawing.Size(0, 23);
			this.odDatePickerFrom.Name = "odDatePickerFrom";
			this.odDatePickerFrom.Size = new System.Drawing.Size(184, 23);
			this.odDatePickerFrom.TabIndex = 152;
			this.odDatePickerFrom.CalendarSelectionChanged += new OpenDental.UI.CalendarSelectionHandler(this.odDatePickerFrom_CalendarSelectionChanged);
			// 
			// listBoxTransTypes
			// 
			this.listBoxTransTypes.ItemStrings = new string[] {
        "Adjustment",
        "Procedure",
        "Payment",
        "Claim Payment"};
			this.listBoxTransTypes.Location = new System.Drawing.Point(103, 64);
			this.listBoxTransTypes.Name = "listBoxTransTypes";
			this.listBoxTransTypes.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBoxTransTypes.Size = new System.Drawing.Size(102, 69);
			this.listBoxTransTypes.TabIndex = 151;
			// 
			// labelTransTypes
			// 
			this.labelTransTypes.Location = new System.Drawing.Point(6, 64);
			this.labelTransTypes.Name = "labelTransTypes";
			this.labelTransTypes.Size = new System.Drawing.Size(96, 16);
			this.labelTransTypes.TabIndex = 151;
			this.labelTransTypes.Text = "Transaction types";
			this.labelTransTypes.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butToday
			// 
			this.butToday.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butToday.Location = new System.Drawing.Point(765, 198);
			this.butToday.Name = "butToday";
			this.butToday.Size = new System.Drawing.Size(75, 24);
			this.butToday.TabIndex = 151;
			this.butToday.Text = "Today";
			this.butToday.Click += new System.EventHandler(this.butToday_Click);
			// 
			// FormLimitedStatementSelect
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(1004, 696);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butToday);
			this.Controls.Add(this.groupFilters);
			this.Controls.Add(this.butNone);
			this.Controls.Add(this.butAll);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormLimitedStatementSelect";
			this.Text = "Limited Statement Select";
			this.Load += new System.EventHandler(this.FormLimitedStatementSelect_Load);
			this.groupFilters.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.Button butNone;
		private UI.Button butAll;
		private UI.GridOD gridMain;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupFilters;
		private System.Windows.Forms.Label labelTransTypes;
		private UI.ListBoxOD listBoxTransTypes;
		private UI.Button butToday;
		private UI.ODDatePicker odDatePickerFrom;
		private System.Windows.Forms.Label labelFrom;
		private System.Windows.Forms.Label labelTo;
		private UI.ODDatePicker odDatePickerTo;
	}
}