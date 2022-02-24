namespace OpenDental {
	partial class FormBugSubmissionHashVitals {
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
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBugSubmissionHashVitals));
			this.gridHashes = new OpenDental.UI.GridOD();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.comboGrouping = new System.Windows.Forms.ComboBox();
			this.label13 = new System.Windows.Forms.Label();
			this.textBugIds = new OpenDental.ODtextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.butRefreshSearch = new OpenDental.UI.Button();
			this.datePicker = new OpenDental.UI.ODDateRangePicker();
			this.textFullHash = new OpenDental.ODtextBox();
			this.textPartHash = new OpenDental.ODtextBox();
			this.textHashNum = new OpenDental.ODtextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.splitContainerNoFlicker1 = new OpenDental.SplitContainerNoFlicker();
			this.gridSubs = new OpenDental.UI.GridOD();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.butCheckHash = new OpenDental.UI.Button();
			this.chartVitals = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.gridHashData = new OpenDental.UI.GridOD();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerNoFlicker1)).BeginInit();
			this.splitContainerNoFlicker1.Panel1.SuspendLayout();
			this.splitContainerNoFlicker1.Panel2.SuspendLayout();
			this.splitContainerNoFlicker1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chartVitals)).BeginInit();
			this.SuspendLayout();
			// 
			// gridHashes
			// 
			this.gridHashes.AllowSortingByColumn = true;
			this.gridHashes.Location = new System.Drawing.Point(15, 83);
			this.gridHashes.Name = "gridHashes";
			this.gridHashes.Size = new System.Drawing.Size(774, 257);
			this.gridHashes.TabIndex = 4;
			this.gridHashes.Title = "Hashes";
			this.gridHashes.TranslationName = "BugHashesGrid";
			this.gridHashes.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridHashes_CellClick);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.comboGrouping);
			this.groupBox1.Controls.Add(this.label13);
			this.groupBox1.Controls.Add(this.textBugIds);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.butRefreshSearch);
			this.groupBox1.Controls.Add(this.datePicker);
			this.groupBox1.Controls.Add(this.textFullHash);
			this.groupBox1.Controls.Add(this.textPartHash);
			this.groupBox1.Controls.Add(this.textHashNum);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(1127, 65);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Filters";
			// 
			// comboGrouping
			// 
			this.comboGrouping.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboGrouping.FormattingEnabled = true;
			this.comboGrouping.Location = new System.Drawing.Point(466, 40);
			this.comboGrouping.Name = "comboGrouping";
			this.comboGrouping.Size = new System.Drawing.Size(102, 21);
			this.comboGrouping.TabIndex = 38;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(413, 40);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(54, 21);
			this.label13.TabIndex = 37;
			this.label13.Text = "Group By:";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBugIds
			// 
			this.textBugIds.AcceptsTab = true;
			this.textBugIds.BackColor = System.Drawing.SystemColors.Window;
			this.textBugIds.DetectLinksEnabled = false;
			this.textBugIds.DetectUrls = false;
			this.textBugIds.Location = new System.Drawing.Point(669, 16);
			this.textBugIds.Multiline = false;
			this.textBugIds.Name = "textBugIds";
			this.textBugIds.QuickPasteType = OpenDentBusiness.QuickPasteType.Procedure;
			this.textBugIds.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textBugIds.Size = new System.Drawing.Size(162, 21);
			this.textBugIds.TabIndex = 8;
			this.textBugIds.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(633, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(65, 21);
			this.label4.TabIndex = 9;
			this.label4.Text = "BugId:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butRefreshSearch
			// 
			this.butRefreshSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefreshSearch.Location = new System.Drawing.Point(1046, 35);
			this.butRefreshSearch.Name = "butRefreshSearch";
			this.butRefreshSearch.Size = new System.Drawing.Size(75, 24);
			this.butRefreshSearch.TabIndex = 7;
			this.butRefreshSearch.Text = "Refresh";
			this.butRefreshSearch.UseVisualStyleBackColor = true;
			this.butRefreshSearch.Click += new System.EventHandler(this.butRefreshSearch_Click);
			// 
			// datePicker
			// 
			this.datePicker.BackColor = System.Drawing.Color.Transparent;
			this.datePicker.EnableWeekButtons = false;
			this.datePicker.Location = new System.Drawing.Point(-5, 37);
			this.datePicker.MinimumSize = new System.Drawing.Size(453, 22);
			this.datePicker.Name = "datePicker";
			this.datePicker.Size = new System.Drawing.Size(453, 24);
			this.datePicker.TabIndex = 6;
			// 
			// textFullHash
			// 
			this.textFullHash.AcceptsTab = true;
			this.textFullHash.BackColor = System.Drawing.SystemColors.Window;
			this.textFullHash.DetectLinksEnabled = false;
			this.textFullHash.DetectUrls = false;
			this.textFullHash.Location = new System.Drawing.Point(231, 16);
			this.textFullHash.Multiline = false;
			this.textFullHash.Name = "textFullHash";
			this.textFullHash.QuickPasteType = OpenDentBusiness.QuickPasteType.Procedure;
			this.textFullHash.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textFullHash.Size = new System.Drawing.Size(166, 21);
			this.textFullHash.TabIndex = 4;
			this.textFullHash.Text = "";
			// 
			// textPartHash
			// 
			this.textPartHash.AcceptsTab = true;
			this.textPartHash.BackColor = System.Drawing.SystemColors.Window;
			this.textPartHash.DetectLinksEnabled = false;
			this.textPartHash.DetectUrls = false;
			this.textPartHash.Location = new System.Drawing.Point(466, 16);
			this.textPartHash.Multiline = false;
			this.textPartHash.Name = "textPartHash";
			this.textPartHash.QuickPasteType = OpenDentBusiness.QuickPasteType.Procedure;
			this.textPartHash.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textPartHash.Size = new System.Drawing.Size(164, 21);
			this.textPartHash.TabIndex = 2;
			this.textPartHash.Text = "";
			// 
			// textHashNum
			// 
			this.textHashNum.AcceptsTab = true;
			this.textHashNum.BackColor = System.Drawing.SystemColors.Window;
			this.textHashNum.DetectLinksEnabled = false;
			this.textHashNum.DetectUrls = false;
			this.textHashNum.Location = new System.Drawing.Point(61, 16);
			this.textHashNum.Multiline = false;
			this.textHashNum.Name = "textHashNum";
			this.textHashNum.QuickPasteType = OpenDentBusiness.QuickPasteType.Procedure;
			this.textHashNum.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textHashNum.Size = new System.Drawing.Size(117, 21);
			this.textHashNum.TabIndex = 0;
			this.textHashNum.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(181, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(58, 21);
			this.label3.TabIndex = 5;
			this.label3.Text = "FullHash:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(403, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 21);
			this.label2.TabIndex = 3;
			this.label2.Text = "PartialHash:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(58, 21);
			this.label1.TabIndex = 1;
			this.label1.Text = "HashNum:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// splitContainerNoFlicker1
			// 
			this.splitContainerNoFlicker1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainerNoFlicker1.Location = new System.Drawing.Point(12, 346);
			this.splitContainerNoFlicker1.Name = "splitContainerNoFlicker1";
			this.splitContainerNoFlicker1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerNoFlicker1.Panel1
			// 
			this.splitContainerNoFlicker1.Panel1.Controls.Add(this.gridSubs);
			// 
			// splitContainerNoFlicker1.Panel2
			// 
			this.splitContainerNoFlicker1.Panel2.Controls.Add(this.groupBox2);
			this.splitContainerNoFlicker1.Panel2.Controls.Add(this.chartVitals);
			this.splitContainerNoFlicker1.Size = new System.Drawing.Size(1127, 484);
			this.splitContainerNoFlicker1.SplitterDistance = 154;
			this.splitContainerNoFlicker1.TabIndex = 10;
			// 
			// gridSubs
			// 
			this.gridSubs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridSubs.Location = new System.Drawing.Point(3, 3);
			this.gridSubs.Name = "gridSubs";
			this.gridSubs.Size = new System.Drawing.Size(1118, 148);
			this.gridSubs.TabIndex = 11;
			this.gridSubs.Title = "Submissions";
			this.gridSubs.TranslationName = "gridSubs";
			this.gridSubs.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSubs_CellDoubleClick);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.butCheckHash);
			this.groupBox2.Location = new System.Drawing.Point(587, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(321, 320);
			this.groupBox2.TabIndex = 10;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Tools";
			// 
			// butCheckHash
			// 
			this.butCheckHash.Location = new System.Drawing.Point(6, 19);
			this.butCheckHash.Name = "butCheckHash";
			this.butCheckHash.Size = new System.Drawing.Size(75, 24);
			this.butCheckHash.TabIndex = 8;
			this.butCheckHash.Text = "Check Hash";
			this.butCheckHash.UseVisualStyleBackColor = true;
			this.butCheckHash.Click += new System.EventHandler(this.butCheckHash_Click);
			// 
			// chartVitals
			// 
			chartArea1.Name = "ChartArea1";
			this.chartVitals.ChartAreas.Add(chartArea1);
			this.chartVitals.Location = new System.Drawing.Point(3, 3);
			this.chartVitals.Name = "chartVitals";
			series1.ChartArea = "ChartArea1";
			series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			series1.IsVisibleInLegend = false;
			series1.Name = "Series1";
			series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
			this.chartVitals.Series.Add(series1);
			this.chartVitals.Size = new System.Drawing.Size(578, 308);
			this.chartVitals.TabIndex = 9;
			this.chartVitals.Text = "chart1";
			// 
			// gridHashData
			// 
			this.gridHashData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridHashData.Location = new System.Drawing.Point(795, 83);
			this.gridHashData.Name = "gridHashData";
			this.gridHashData.Size = new System.Drawing.Size(338, 257);
			this.gridHashData.TabIndex = 7;
			this.gridHashData.Title = "Hash Data";
			this.gridHashData.TranslationName = "BugHashesGrid";
			this.gridHashData.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridHashData_CellClick);
			// 
			// FormBugSubmissionHashVitals
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1151, 842);
			this.Controls.Add(this.gridHashData);
			this.Controls.Add(this.splitContainerNoFlicker1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridHashes);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormBugSubmissionHashVitals";
			this.Text = "BugSubmissionHash Vitals";
			this.Load += new System.EventHandler(this.FormBugSubmissionHashVitals_Load);
			this.groupBox1.ResumeLayout(false);
			this.splitContainerNoFlicker1.Panel1.ResumeLayout(false);
			this.splitContainerNoFlicker1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerNoFlicker1)).EndInit();
			this.splitContainerNoFlicker1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chartVitals)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		private UI.GridOD gridHashes;
		private System.Windows.Forms.GroupBox groupBox1;
		private ODtextBox textHashNum;
		private System.Windows.Forms.Label label2;
		private ODtextBox textPartHash;
		private System.Windows.Forms.Label label1;
		private UI.ODDateRangePicker datePicker;
		private System.Windows.Forms.Label label3;
		private ODtextBox textFullHash;
		private UI.Button butRefreshSearch;
		private SplitContainerNoFlicker splitContainerNoFlicker1;
		private UI.GridOD gridHashData;
		private System.Windows.Forms.DataVisualization.Charting.Chart chartVitals;
		private UI.Button butCheckHash;
		private System.Windows.Forms.GroupBox groupBox2;
		private UI.GridOD gridSubs;
		private ODtextBox textBugIds;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboGrouping;
		private System.Windows.Forms.Label label13;
	}
}