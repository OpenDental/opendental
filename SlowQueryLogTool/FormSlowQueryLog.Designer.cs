using System.Drawing;

namespace SlowQueryTool {
	partial class FormSlowQueryLog {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSlowQueryLog));
			this.textLogURL = new System.Windows.Forms.TextBox();
			this.butAnalyze = new System.Windows.Forms.Button();
			this.textResults = new System.Windows.Forms.RichTextBox();
			this.labelFilePath = new System.Windows.Forms.Label();
			this.butNone = new System.Windows.Forms.Button();
			this.contextMenuQuery = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuItemPerpetrator = new System.Windows.Forms.ToolStripMenuItem();
			this.labelSummary = new System.Windows.Forms.Label();
			this.listBoxFilter = new System.Windows.Forms.ListBox();
			this.listBoxOptions = new System.Windows.Forms.ListBox();
			this.textFilterValue = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelSeconds = new System.Windows.Forms.Label();
			this.labelQueryCount = new System.Windows.Forms.Label();
			this.labelQueryGroupCount = new System.Windows.Forms.Label();
			this.labelVersion = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.comboClose = new System.Windows.Forms.ComboBox();
			this.comboOpen = new System.Windows.Forms.ComboBox();
			this.datePicker = new SlowQueryLog.UI.ODDateRangePicker();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labelFirstQuery = new System.Windows.Forms.Label();
			this.labelLastQuery = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.gridQueryGroups = new OpenDental.UI.GridOD();
			this.gridQueries = new OpenDental.UI.GridOD();
			this.contextMenuQuery.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// textLogURL
			// 
			this.textLogURL.Location = new System.Drawing.Point(12, 26);
			this.textLogURL.Name = "textLogURL";
			this.textLogURL.Size = new System.Drawing.Size(857, 20);
			this.textLogURL.TabIndex = 0;
			// 
			// butAnalyze
			// 
			this.butAnalyze.Location = new System.Drawing.Point(875, 25);
			this.butAnalyze.Name = "butAnalyze";
			this.butAnalyze.Size = new System.Drawing.Size(67, 23);
			this.butAnalyze.TabIndex = 1;
			this.butAnalyze.Text = "Analyze";
			this.butAnalyze.UseVisualStyleBackColor = true;
			this.butAnalyze.Click += new System.EventHandler(this.butAnalyze_Click);
			// 
			// textResults
			// 
			this.textResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textResults.Location = new System.Drawing.Point(12, 622);
			this.textResults.Name = "textResults";
			this.textResults.ReadOnly = true;
			this.textResults.Size = new System.Drawing.Size(1676, 278);
			this.textResults.TabIndex = 2;
			this.textResults.Text = "";
			// 
			// labelFilePath
			// 
			this.labelFilePath.AutoSize = true;
			this.labelFilePath.Location = new System.Drawing.Point(9, 9);
			this.labelFilePath.Name = "labelFilePath";
			this.labelFilePath.Size = new System.Drawing.Size(48, 13);
			this.labelFilePath.TabIndex = 6;
			this.labelFilePath.Text = "File Path";
			// 
			// butNone
			// 
			this.butNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butNone.Location = new System.Drawing.Point(1613, 97);
			this.butNone.Name = "butNone";
			this.butNone.Size = new System.Drawing.Size(75, 23);
			this.butNone.TabIndex = 7;
			this.butNone.Text = "None";
			this.butNone.UseVisualStyleBackColor = true;
			this.butNone.Click += new System.EventHandler(this.butNone_Click);
			// 
			// contextMenuQuery
			// 
			this.contextMenuQuery.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemPerpetrator});
			this.contextMenuQuery.Name = "contextMenuQuery";
			this.contextMenuQuery.Size = new System.Drawing.Size(162, 26);
			this.contextMenuQuery.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuQuery_Opening);
			// 
			// menuItemPerpetrator
			// 
			this.menuItemPerpetrator.Name = "menuItemPerpetrator";
			this.menuItemPerpetrator.Size = new System.Drawing.Size(161, 22);
			this.menuItemPerpetrator.Text = "View Perpetrator";
			this.menuItemPerpetrator.Visible = false;
			this.menuItemPerpetrator.Click += new System.EventHandler(this.menuItemPerpetrator_Click);
			// 
			// labelSummary
			// 
			this.labelSummary.AutoSize = true;
			this.labelSummary.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
			this.labelSummary.Location = new System.Drawing.Point(12, 595);
			this.labelSummary.Name = "labelSummary";
			this.labelSummary.Size = new System.Drawing.Size(90, 24);
			this.labelSummary.TabIndex = 8;
			this.labelSummary.Text = "Summary";
			// 
			// listBoxFilter
			// 
			this.listBoxFilter.FormattingEnabled = true;
			this.listBoxFilter.Items.AddRange(new object[] {
            "Execution Time",
            "Rows Examined"});
			this.listBoxFilter.Location = new System.Drawing.Point(9, 16);
			this.listBoxFilter.Name = "listBoxFilter";
			this.listBoxFilter.Size = new System.Drawing.Size(106, 43);
			this.listBoxFilter.TabIndex = 9;
			this.listBoxFilter.SelectedIndexChanged += new System.EventHandler(this.listBoxFilter_SelectedIndexChanged);
			// 
			// listBoxOptions
			// 
			this.listBoxOptions.FormattingEnabled = true;
			this.listBoxOptions.Items.AddRange(new object[] {
            "Greater Than",
            "Less Than"});
			this.listBoxOptions.Location = new System.Drawing.Point(121, 15);
			this.listBoxOptions.Name = "listBoxOptions";
			this.listBoxOptions.Size = new System.Drawing.Size(114, 43);
			this.listBoxOptions.TabIndex = 10;
			this.listBoxOptions.SelectedIndexChanged += new System.EventHandler(this.listBoxOptions_SelectedIndexChanged);
			// 
			// textFilterValue
			// 
			this.textFilterValue.Location = new System.Drawing.Point(241, 27);
			this.textFilterValue.Name = "textFilterValue";
			this.textFilterValue.Size = new System.Drawing.Size(82, 20);
			this.textFilterValue.TabIndex = 11;
			this.textFilterValue.TextChanged += new System.EventHandler(this.textFilterValue_TextChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.listBoxFilter);
			this.groupBox1.Controls.Add(this.textFilterValue);
			this.groupBox1.Controls.Add(this.listBoxOptions);
			this.groupBox1.Controls.Add(this.labelSeconds);
			this.groupBox1.Location = new System.Drawing.Point(594, 52);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(348, 68);
			this.groupBox1.TabIndex = 12;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Filter Queries";
			// 
			// labelSeconds
			// 
			this.labelSeconds.AutoSize = true;
			this.labelSeconds.Location = new System.Drawing.Point(323, 30);
			this.labelSeconds.Name = "labelSeconds";
			this.labelSeconds.Size = new System.Drawing.Size(12, 13);
			this.labelSeconds.TabIndex = 12;
			this.labelSeconds.Text = "s";
			this.labelSeconds.Visible = false;
			// 
			// labelQueryCount
			// 
			this.labelQueryCount.Location = new System.Drawing.Point(853, 120);
			this.labelQueryCount.Name = "labelQueryCount";
			this.labelQueryCount.Size = new System.Drawing.Size(89, 20);
			this.labelQueryCount.TabIndex = 13;
			this.labelQueryCount.Text = "Rows: ";
			this.labelQueryCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelQueryGroupCount
			// 
			this.labelQueryGroupCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelQueryGroupCount.Location = new System.Drawing.Point(1596, 120);
			this.labelQueryGroupCount.Name = "labelQueryGroupCount";
			this.labelQueryGroupCount.Size = new System.Drawing.Size(92, 20);
			this.labelQueryGroupCount.TabIndex = 14;
			this.labelQueryGroupCount.Text = "Rows: ";
			this.labelQueryGroupCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelVersion
			// 
			this.labelVersion.AutoSize = true;
			this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
			this.labelVersion.Location = new System.Drawing.Point(981, 22);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(152, 24);
			this.labelVersion.TabIndex = 15;
			this.labelVersion.Text = "MySQL Version: ";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.comboClose);
			this.groupBox2.Controls.Add(this.comboOpen);
			this.groupBox2.Controls.Add(this.datePicker);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Location = new System.Drawing.Point(253, 52);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(335, 68);
			this.groupBox2.TabIndex = 19;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Filter Date and Time";
			// 
			// comboClose
			// 
			this.comboClose.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClose.FormattingEnabled = true;
			this.comboClose.Items.AddRange(new object[] {
            "12am",
            "1am",
            "2am",
            "3am",
            "4am",
            "5am",
            "6am",
            "7am",
            "8am",
            "9am",
            "10am",
            "11am",
            "12pm",
            "1pm",
            "2pm",
            "3pm",
            "4pm",
            "5pm",
            "6pm",
            "7pm",
            "8pm",
            "9pm",
            "10pm",
            "11pm"});
			this.comboClose.Location = new System.Drawing.Point(254, 39);
			this.comboClose.Name = "comboClose";
			this.comboClose.Size = new System.Drawing.Size(72, 21);
			this.comboClose.TabIndex = 22;
			// 
			// comboOpen
			// 
			this.comboOpen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboOpen.FormattingEnabled = true;
			this.comboOpen.Items.AddRange(new object[] {
            "12am",
            "1am",
            "2am",
            "3am",
            "4am",
            "5am",
            "6am",
            "7am",
            "8am",
            "9am",
            "10am",
            "11am",
            "12pm",
            "1pm",
            "2pm",
            "3pm",
            "4pm",
            "5pm",
            "6pm",
            "7pm",
            "8pm",
            "9pm",
            "10pm",
            "11pm"});
			this.comboOpen.Location = new System.Drawing.Point(254, 16);
			this.comboOpen.Name = "comboOpen";
			this.comboOpen.Size = new System.Drawing.Size(72, 21);
			this.comboOpen.TabIndex = 21;
			// 
			// datePicker
			// 
			this.datePicker.BackColor = System.Drawing.Color.Transparent;
			this.datePicker.DefaultDateTimeFrom = new System.DateTime(((long)(0)));
			this.datePicker.DefaultDateTimeTo = new System.DateTime(((long)(0)));
			this.datePicker.EnableWeekButtons = false;
			this.datePicker.IsVertical = true;
			this.datePicker.Location = new System.Drawing.Point(-23, 15);
			this.datePicker.MinimumSize = new System.Drawing.Size(165, 46);
			this.datePicker.Name = "datePicker";
			this.datePicker.Size = new System.Drawing.Size(181, 47);
			this.datePicker.TabIndex = 16;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(160, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(95, 13);
			this.label1.TabIndex = 19;
			this.label1.Text = "Hour Office Opens";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(160, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(95, 13);
			this.label2.TabIndex = 20;
			this.label2.Text = "Hour Office Closes";
			// 
			// labelFirstQuery
			// 
			this.labelFirstQuery.AutoSize = true;
			this.labelFirstQuery.Location = new System.Drawing.Point(19, 16);
			this.labelFirstQuery.Name = "labelFirstQuery";
			this.labelFirstQuery.Size = new System.Drawing.Size(66, 13);
			this.labelFirstQuery.TabIndex = 20;
			this.labelFirstQuery.Text = "Begin Date: ";
			// 
			// labelLastQuery
			// 
			this.labelLastQuery.AutoSize = true;
			this.labelLastQuery.Location = new System.Drawing.Point(27, 39);
			this.labelLastQuery.Name = "labelLastQuery";
			this.labelLastQuery.Size = new System.Drawing.Size(58, 13);
			this.labelLastQuery.TabIndex = 21;
			this.labelLastQuery.Text = "End Date: ";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.labelFirstQuery);
			this.groupBox3.Controls.Add(this.labelLastQuery);
			this.groupBox3.Location = new System.Drawing.Point(12, 52);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(235, 68);
			this.groupBox3.TabIndex = 23;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Log Info";
			// 
			// gridQueryGroups
			// 
			this.gridQueryGroups.AllowSortingByColumn = true;
			this.gridQueryGroups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridQueryGroups.HasAutoWrappedHeaders = true;
			this.gridQueryGroups.HasMultilineHeaders = true;
			this.gridQueryGroups.Location = new System.Drawing.Point(948, 140);
			this.gridQueryGroups.Name = "gridQueryGroups";
			this.gridQueryGroups.Size = new System.Drawing.Size(740, 452);
			this.gridQueryGroups.TabIndex = 18;
			this.gridQueryGroups.Title = "Query Groups";
			this.gridQueryGroups.TranslationName = "gridQueryGroups";
			this.gridQueryGroups.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridQueryGroups_CellDoubleClick);
			this.gridQueryGroups.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridQueryGroups_CellClick);
			// 
			// gridQueries
			// 
			this.gridQueries.AllowSortingByColumn = true;
			this.gridQueries.ContextMenuStrip = this.contextMenuQuery;
			this.gridQueries.Location = new System.Drawing.Point(12, 140);
			this.gridQueries.Name = "gridQueries";
			this.gridQueries.Size = new System.Drawing.Size(930, 452);
			this.gridQueries.TabIndex = 17;
			this.gridQueries.Title = "Queries";
			this.gridQueries.TranslationName = "gridQueries";
			this.gridQueries.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridQueries_CellDoubleClick);
			this.gridQueries.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridQueries_CellClick);
			// 
			// FormSlowQueryLog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1700, 912);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.gridQueryGroups);
			this.Controls.Add(this.gridQueries);
			this.Controls.Add(this.labelVersion);
			this.Controls.Add(this.labelQueryGroupCount);
			this.Controls.Add(this.labelQueryCount);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.labelSummary);
			this.Controls.Add(this.butNone);
			this.Controls.Add(this.labelFilePath);
			this.Controls.Add(this.textResults);
			this.Controls.Add(this.butAnalyze);
			this.Controls.Add(this.textLogURL);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSlowQueryLog";
			this.Text = "Slow Queries";
			this.Load += new System.EventHandler(this.FormSlowQueryLog_Load);
			this.contextMenuQuery.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textLogURL;
		private System.Windows.Forms.Button butAnalyze;
		private System.Windows.Forms.RichTextBox textResults;
		private System.Windows.Forms.Label labelFilePath;
		private System.Windows.Forms.Button butNone;
		private System.Windows.Forms.Label labelSummary;
		private System.Windows.Forms.ContextMenuStrip contextMenuQuery;
		private System.Windows.Forms.ToolStripMenuItem menuItemPerpetrator;
		private System.Windows.Forms.ListBox listBoxFilter;
		private System.Windows.Forms.ListBox listBoxOptions;
		private System.Windows.Forms.TextBox textFilterValue;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label labelSeconds;
		private System.Windows.Forms.Label labelQueryCount;
		private System.Windows.Forms.Label labelQueryGroupCount;
		private System.Windows.Forms.Label labelVersion;
		private SlowQueryLog.UI.ODDateRangePicker datePicker;
		private OpenDental.UI.GridOD gridQueries;
		private OpenDental.UI.GridOD gridQueryGroups;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboClose;
		private System.Windows.Forms.ComboBox comboOpen;
		private System.Windows.Forms.Label labelFirstQuery;
		private System.Windows.Forms.Label labelLastQuery;
		private System.Windows.Forms.GroupBox groupBox3;
	}
}

