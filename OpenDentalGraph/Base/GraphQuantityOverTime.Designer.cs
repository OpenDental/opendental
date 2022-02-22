namespace OpenDentalGraph {
	partial class GraphQuantityOverTime {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.Windows.Forms.DataVisualization.Charting.HorizontalLineAnnotation horizontalLineAnnotation1 = new System.Windows.Forms.DataVisualization.Charting.HorizontalLineAnnotation();
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
			System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
			this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.comboGroupBy = new System.Windows.Forms.ComboBox();
			this.dateTimeFrom = new System.Windows.Forms.DateTimePicker();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.dateTimeTo = new System.Windows.Forms.DateTimePicker();
			this.groupBoxBreakdown = new System.Windows.Forms.GroupBox();
			this.radBreakdownNone = new System.Windows.Forms.RadioButton();
			this.comboBreakdownBy = new System.Windows.Forms.ComboBox();
			this.numericTop = new System.Windows.Forms.NumericUpDown();
			this.radBreakdownAll = new System.Windows.Forms.RadioButton();
			this.radBreakdownTop = new System.Windows.Forms.RadioButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboQuickRange = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.comboLegendDock = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.comboQuantityType = new System.Windows.Forms.ComboBox();
			this.comboChartType = new System.Windows.Forms.ComboBox();
			this.splitContainerMain = new System.Windows.Forms.SplitContainer();
			this.textChartTitle = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.splitContainerChart = new System.Windows.Forms.SplitContainer();
			this.chartLegend1 = new OpenDentalGraph.Legend();
			this.panelChart = new System.Windows.Forms.Panel();
			this.pictureBoxLoading = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
			this.groupBoxBreakdown.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericTop)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
			this.splitContainerMain.Panel1.SuspendLayout();
			this.splitContainerMain.Panel2.SuspendLayout();
			this.splitContainerMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerChart)).BeginInit();
			this.splitContainerChart.Panel1.SuspendLayout();
			this.splitContainerChart.Panel2.SuspendLayout();
			this.splitContainerChart.SuspendLayout();
			this.panelChart.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxLoading)).BeginInit();
			this.SuspendLayout();
			// 
			// chart1
			// 
			horizontalLineAnnotation1.AnchorY = 0D;
			horizontalLineAnnotation1.ClipToChartArea = "Default";
			horizontalLineAnnotation1.IsInfinitive = true;
			horizontalLineAnnotation1.LineColor = System.Drawing.Color.Red;
			horizontalLineAnnotation1.Name = "HorizontalLineAnnotation1";
			horizontalLineAnnotation1.YAxisName = "Default\\rY";
			this.chart1.Annotations.Add(horizontalLineAnnotation1);
			this.chart1.BorderlineColor = System.Drawing.Color.Silver;
			this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
			this.chart1.BorderlineWidth = 2;
			this.chart1.BorderSkin.PageColor = System.Drawing.Color.Transparent;
			chartArea1.Area3DStyle.Inclination = 40;
			chartArea1.Area3DStyle.IsClustered = true;
			chartArea1.Area3DStyle.IsRightAngleAxes = false;
			chartArea1.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
			chartArea1.Area3DStyle.Rotation = 25;
			chartArea1.Area3DStyle.WallWidth = 3;
			chartArea1.AxisX.Interval = 7D;
			chartArea1.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Days;
			chartArea1.AxisX.IsMarginVisible = false;
			chartArea1.AxisX.LabelAutoFitStyle = ((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles)(((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.StaggeredLabels) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep45)));
			chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
			chartArea1.AxisX.LabelStyle.Format = "MM/dd/yy";
			chartArea1.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea1.AxisX.ScaleView.MinSize = 14D;
			chartArea1.AxisX.ScaleView.MinSizeType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Days;
			chartArea1.AxisX.ScaleView.Position = 1D;
			chartArea1.AxisX.ScaleView.Size = 1D;
			chartArea1.AxisX.ScaleView.SizeType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Days;
			chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			chartArea1.AxisY.LabelStyle.Format = "C0";
			chartArea1.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea1.AxisY.MajorTickMark.Enabled = false;
			chartArea1.AxisY2.LabelStyle.Format = "C";
			chartArea1.BackColor = System.Drawing.Color.White;
			chartArea1.BackSecondaryColor = System.Drawing.Color.White;
			chartArea1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
			chartArea1.CursorX.IsUserSelectionEnabled = true;
			chartArea1.Name = "Default";
			this.chart1.ChartAreas.Add(chartArea1);
			this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.chart1.Location = new System.Drawing.Point(0, 0);
			this.chart1.Name = "chart1";
			series1.ChartArea = "Default";
			series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedArea;
			series1.Color = System.Drawing.Color.Red;
			series1.LabelFormat = "C0";
			series1.Name = "Daily";
			this.chart1.Series.Add(series1);
			this.chart1.Size = new System.Drawing.Size(789, 302);
			this.chart1.TabIndex = 2;
			title1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
			title1.Name = "ChartTitle";
			title1.Text = "Procedure Log Income";
			title1.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
			title2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
			title2.Name = "ChartSubTitle";
			title2.Visible = false;
			this.chart1.Titles.Add(title1);
			this.chart1.Titles.Add(title2);
			this.chart1.GetToolTipText += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ToolTipEventArgs>(this.chart1_GetToolTipText);
			this.chart1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.chart1_MouseDoubleClick);
			// 
			// comboGroupBy
			// 
			this.comboGroupBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboGroupBy.FormattingEnabled = true;
			this.comboGroupBy.Location = new System.Drawing.Point(83, 68);
			this.comboGroupBy.Name = "comboGroupBy";
			this.comboGroupBy.Size = new System.Drawing.Size(137, 21);
			this.comboGroupBy.TabIndex = 0;
			this.comboGroupBy.SelectedIndexChanged += new System.EventHandler(this.FilterData);
			// 
			// dateTimeFrom
			// 
			this.dateTimeFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateTimeFrom.Location = new System.Drawing.Point(90, 44);
			this.dateTimeFrom.Name = "dateTimeFrom";
			this.dateTimeFrom.Size = new System.Drawing.Size(127, 20);
			this.dateTimeFrom.TabIndex = 0;
			this.dateTimeFrom.Value = new System.DateTime(2015, 12, 12, 10, 19, 0, 0);
			this.dateTimeFrom.ValueChanged += new System.EventHandler(this.FilterData);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(75, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "From:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 72);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(75, 13);
			this.label4.TabIndex = 12;
			this.label4.Text = "To:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// dateTimeTo
			// 
			this.dateTimeTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateTimeTo.Location = new System.Drawing.Point(90, 68);
			this.dateTimeTo.Name = "dateTimeTo";
			this.dateTimeTo.Size = new System.Drawing.Size(127, 20);
			this.dateTimeTo.TabIndex = 1;
			this.dateTimeTo.ValueChanged += new System.EventHandler(this.FilterData);
			// 
			// groupBoxBreakdown
			// 
			this.groupBoxBreakdown.Controls.Add(this.radBreakdownNone);
			this.groupBoxBreakdown.Controls.Add(this.comboBreakdownBy);
			this.groupBoxBreakdown.Controls.Add(this.numericTop);
			this.groupBoxBreakdown.Controls.Add(this.radBreakdownAll);
			this.groupBoxBreakdown.Controls.Add(this.radBreakdownTop);
			this.groupBoxBreakdown.Location = new System.Drawing.Point(470, 32);
			this.groupBoxBreakdown.Name = "groupBoxBreakdown";
			this.groupBoxBreakdown.Size = new System.Drawing.Size(251, 123);
			this.groupBoxBreakdown.TabIndex = 1;
			this.groupBoxBreakdown.TabStop = false;
			this.groupBoxBreakdown.Text = "Series Grouping";
			// 
			// radBreakdownNone
			// 
			this.radBreakdownNone.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radBreakdownNone.Location = new System.Drawing.Point(4, 68);
			this.radBreakdownNone.Name = "radBreakdownNone";
			this.radBreakdownNone.Size = new System.Drawing.Size(53, 17);
			this.radBreakdownNone.TabIndex = 2;
			this.radBreakdownNone.Text = "None";
			this.radBreakdownNone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radBreakdownNone.UseVisualStyleBackColor = true;
			this.radBreakdownNone.CheckedChanged += new System.EventHandler(this.radBreakdown_CheckedChanged);
			// 
			// comboBreakdownBy
			// 
			this.comboBreakdownBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBreakdownBy.FormattingEnabled = true;
			this.comboBreakdownBy.Location = new System.Drawing.Point(129, 40);
			this.comboBreakdownBy.Name = "comboBreakdownBy";
			this.comboBreakdownBy.Size = new System.Drawing.Size(117, 21);
			this.comboBreakdownBy.TabIndex = 3;
			this.comboBreakdownBy.SelectedIndexChanged += new System.EventHandler(this.FilterData);
			// 
			// numericTop
			// 
			this.numericTop.Location = new System.Drawing.Point(68, 41);
			this.numericTop.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericTop.Name = "numericTop";
			this.numericTop.Size = new System.Drawing.Size(56, 20);
			this.numericTop.TabIndex = 2;
			this.numericTop.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.numericTop.ValueChanged += new System.EventHandler(this.FilterData);
			// 
			// radBreakdownAll
			// 
			this.radBreakdownAll.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radBreakdownAll.Location = new System.Drawing.Point(15, 20);
			this.radBreakdownAll.Name = "radBreakdownAll";
			this.radBreakdownAll.Size = new System.Drawing.Size(42, 17);
			this.radBreakdownAll.TabIndex = 0;
			this.radBreakdownAll.Text = "All";
			this.radBreakdownAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radBreakdownAll.UseVisualStyleBackColor = true;
			this.radBreakdownAll.CheckedChanged += new System.EventHandler(this.radBreakdown_CheckedChanged);
			// 
			// radBreakdownTop
			// 
			this.radBreakdownTop.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radBreakdownTop.Checked = true;
			this.radBreakdownTop.Location = new System.Drawing.Point(12, 43);
			this.radBreakdownTop.Name = "radBreakdownTop";
			this.radBreakdownTop.Size = new System.Drawing.Size(45, 17);
			this.radBreakdownTop.TabIndex = 1;
			this.radBreakdownTop.TabStop = true;
			this.radBreakdownTop.Text = "Top";
			this.radBreakdownTop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radBreakdownTop.UseVisualStyleBackColor = true;
			this.radBreakdownTop.CheckedChanged += new System.EventHandler(this.radBreakdown_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.dateTimeFrom);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.comboQuickRange);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.dateTimeTo);
			this.groupBox2.Location = new System.Drawing.Point(241, 32);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(223, 123);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Filter Dates";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 23);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(75, 13);
			this.label2.TabIndex = 13;
			this.label2.Text = "Quick Range:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboQuickRange
			// 
			this.comboQuickRange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboQuickRange.FormattingEnabled = true;
			this.comboQuickRange.Location = new System.Drawing.Point(90, 19);
			this.comboQuickRange.Name = "comboQuickRange";
			this.comboQuickRange.Size = new System.Drawing.Size(127, 21);
			this.comboQuickRange.TabIndex = 2;
			this.comboQuickRange.SelectedIndexChanged += new System.EventHandler(this.comboQuickRange_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 72);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 13);
			this.label1.TabIndex = 13;
			this.label1.Text = "Group By:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.comboLegendDock);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.comboQuantityType);
			this.groupBox1.Controls.Add(this.comboChartType);
			this.groupBox1.Controls.Add(this.comboGroupBy);
			this.groupBox1.Location = new System.Drawing.Point(8, 32);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(227, 123);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Chart Options";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(26, 99);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(51, 13);
			this.label7.TabIndex = 17;
			this.label7.Text = "Legend:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboLegendDock
			// 
			this.comboLegendDock.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboLegendDock.FormattingEnabled = true;
			this.comboLegendDock.Location = new System.Drawing.Point(83, 95);
			this.comboLegendDock.Name = "comboLegendDock";
			this.comboLegendDock.Size = new System.Drawing.Size(137, 21);
			this.comboLegendDock.TabIndex = 16;
			this.comboLegendDock.SelectedIndexChanged += new System.EventHandler(this.comboLegendDock_SelectedIndexChanged);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(6, 48);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(71, 13);
			this.label6.TabIndex = 15;
			this.label6.Text = "Series Type:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 23);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(71, 13);
			this.label5.TabIndex = 14;
			this.label5.Text = "Display:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboQuantityType
			// 
			this.comboQuantityType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboQuantityType.FormattingEnabled = true;
			this.comboQuantityType.Location = new System.Drawing.Point(83, 19);
			this.comboQuantityType.Name = "comboQuantityType";
			this.comboQuantityType.Size = new System.Drawing.Size(137, 21);
			this.comboQuantityType.TabIndex = 1;
			this.comboQuantityType.SelectedIndexChanged += new System.EventHandler(this.FilterData);
			// 
			// comboChartType
			// 
			this.comboChartType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboChartType.FormattingEnabled = true;
			this.comboChartType.Location = new System.Drawing.Point(83, 44);
			this.comboChartType.Name = "comboChartType";
			this.comboChartType.Size = new System.Drawing.Size(137, 21);
			this.comboChartType.TabIndex = 0;
			this.comboChartType.SelectedIndexChanged += new System.EventHandler(this.FilterData);
			// 
			// splitContainerMain
			// 
			this.splitContainerMain.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
			this.splitContainerMain.Name = "splitContainerMain";
			this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerMain.Panel1
			// 
			this.splitContainerMain.Panel1.Controls.Add(this.textChartTitle);
			this.splitContainerMain.Panel1.Controls.Add(this.label8);
			this.splitContainerMain.Panel1.Controls.Add(this.groupBox1);
			this.splitContainerMain.Panel1.Controls.Add(this.groupBox2);
			this.splitContainerMain.Panel1.Controls.Add(this.groupBoxBreakdown);
			// 
			// splitContainerMain.Panel2
			// 
			this.splitContainerMain.Panel2.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainerMain.Panel2.Controls.Add(this.splitContainerChart);
			this.splitContainerMain.Size = new System.Drawing.Size(900, 463);
			this.splitContainerMain.SplitterDistance = 157;
			this.splitContainerMain.TabIndex = 4;
			// 
			// textChartTitle
			// 
			this.textChartTitle.Location = new System.Drawing.Point(91, 6);
			this.textChartTitle.Name = "textChartTitle";
			this.textChartTitle.Size = new System.Drawing.Size(373, 20);
			this.textChartTitle.TabIndex = 5;
			this.textChartTitle.TextChanged += new System.EventHandler(this.textChartTitle_TextChanged);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(5, 4);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(80, 23);
			this.label8.TabIndex = 4;
			this.label8.Text = "Chart Title:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// splitContainerChart
			// 
			this.splitContainerChart.BackColor = System.Drawing.SystemColors.Control;
			this.splitContainerChart.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerChart.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainerChart.Location = new System.Drawing.Point(0, 0);
			this.splitContainerChart.Name = "splitContainerChart";
			// 
			// splitContainerChart.Panel1
			// 
			this.splitContainerChart.Panel1.Controls.Add(this.chartLegend1);
			this.splitContainerChart.Panel1MinSize = 1;
			// 
			// splitContainerChart.Panel2
			// 
			this.splitContainerChart.Panel2.Controls.Add(this.panelChart);
			this.splitContainerChart.Panel2MinSize = 1;
			this.splitContainerChart.Size = new System.Drawing.Size(900, 302);
			this.splitContainerChart.SplitterDistance = 110;
			this.splitContainerChart.SplitterWidth = 1;
			this.splitContainerChart.TabIndex = 3;
			// 
			// chartLegend1
			// 
			this.chartLegend1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.chartLegend1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.chartLegend1.LegendDock = OpenDentalGraph.Enumerations.LegendDockType.Left;
			this.chartLegend1.Location = new System.Drawing.Point(0, 0);
			this.chartLegend1.Margin = new System.Windows.Forms.Padding(0);
			this.chartLegend1.Name = "chartLegend1";
			this.chartLegend1.PaddingPx = 3F;
			this.chartLegend1.Size = new System.Drawing.Size(110, 302);
			this.chartLegend1.TabIndex = 1;
			// 
			// panelChart
			// 
			this.panelChart.Controls.Add(this.chart1);
			this.panelChart.Controls.Add(this.pictureBoxLoading);
			this.panelChart.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelChart.Location = new System.Drawing.Point(0, 0);
			this.panelChart.Name = "panelChart";
			this.panelChart.Size = new System.Drawing.Size(789, 302);
			this.panelChart.TabIndex = 4;
			// 
			// pictureBoxLoading
			// 
			this.pictureBoxLoading.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBoxLoading.Image = global::OpenDentalGraph.Properties.Resources.loadingAnim;
			this.pictureBoxLoading.Location = new System.Drawing.Point(0, 0);
			this.pictureBoxLoading.Name = "pictureBoxLoading";
			this.pictureBoxLoading.Size = new System.Drawing.Size(789, 302);
			this.pictureBoxLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBoxLoading.TabIndex = 3;
			this.pictureBoxLoading.TabStop = false;
			// 
			// GraphQuantityOverTime
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.splitContainerMain);
			this.Name = "GraphQuantityOverTime";
			this.Size = new System.Drawing.Size(900, 463);
			this.Resize += new System.EventHandler(this.ChartQuantityOverTime_Resize);
			((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
			this.groupBoxBreakdown.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numericTop)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.splitContainerMain.Panel1.ResumeLayout(false);
			this.splitContainerMain.Panel1.PerformLayout();
			this.splitContainerMain.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
			this.splitContainerMain.ResumeLayout(false);
			this.splitContainerChart.Panel1.ResumeLayout(false);
			this.splitContainerChart.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerChart)).EndInit();
			this.splitContainerChart.ResumeLayout(false);
			this.panelChart.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxLoading)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
		private System.Windows.Forms.ComboBox comboGroupBy;
		private System.Windows.Forms.DateTimePicker dateTimeFrom;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.DateTimePicker dateTimeTo;
		private System.Windows.Forms.GroupBox groupBoxBreakdown;
		private System.Windows.Forms.NumericUpDown numericTop;
		private System.Windows.Forms.RadioButton radBreakdownAll;
		private System.Windows.Forms.RadioButton radBreakdownTop;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox comboBreakdownBy;
		private System.Windows.Forms.ComboBox comboQuickRange;
		private System.Windows.Forms.RadioButton radBreakdownNone;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox comboChartType;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboQuantityType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.SplitContainer splitContainerMain;
		private System.Windows.Forms.SplitContainer splitContainerChart;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox comboLegendDock;
		private System.Windows.Forms.TextBox textChartTitle;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.PictureBox pictureBoxLoading;
		private Legend chartLegend1;
		private System.Windows.Forms.Panel panelChart;
	}
}
