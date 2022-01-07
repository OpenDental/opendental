namespace OpenDentalGraph {
	partial class GraphIncome {
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
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
			System.Windows.Forms.DataVisualization.Charting.Title title4 = new System.Windows.Forms.DataVisualization.Charting.Title();
			System.Windows.Forms.DataVisualization.Charting.Title title5 = new System.Windows.Forms.DataVisualization.Charting.Title();
			System.Windows.Forms.DataVisualization.Charting.Title title6 = new System.Windows.Forms.DataVisualization.Charting.Title();
			this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.comboGroupBy = new System.Windows.Forms.ComboBox();
			this.checkShowDailyLabels = new System.Windows.Forms.CheckBox();
			this.checkShowGroupLabels = new System.Windows.Forms.CheckBox();
			this.numberDailyLabelDensity = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.dateTimeFrom = new System.Windows.Forms.DateTimePicker();
			this.dateTimeTo = new System.Windows.Forms.DateTimePicker();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numberDailyLabelDensity)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// chart1
			// 
			this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.chart1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(223)))), ((int)(((byte)(193)))));
			this.chart1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
			this.chart1.BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(64)))), ((int)(((byte)(1)))));
			this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
			this.chart1.BorderlineWidth = 2;
			this.chart1.BorderSkin.PageColor = System.Drawing.Color.Transparent;
			this.chart1.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
			chartArea3.Area3DStyle.Inclination = 40;
			chartArea3.Area3DStyle.IsClustered = true;
			chartArea3.Area3DStyle.IsRightAngleAxes = false;
			chartArea3.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
			chartArea3.Area3DStyle.Rotation = 25;
			chartArea3.Area3DStyle.WallWidth = 3;
			chartArea3.AxisX.Interval = 7D;
			chartArea3.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Days;
			chartArea3.AxisX.LabelAutoFitStyle = ((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles)(((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.StaggeredLabels | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep30) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.WordWrap)));
			chartArea3.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
			chartArea3.AxisX.LabelStyle.Format = "MM/dd/yy";
			chartArea3.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea3.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea3.AxisX.ScaleView.MinSize = 14D;
			chartArea3.AxisX.ScaleView.MinSizeType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Days;
			chartArea3.AxisX.ScaleView.Position = 1D;
			chartArea3.AxisX.ScaleView.Size = 1D;
			chartArea3.AxisX.ScaleView.SizeType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Days;
			chartArea3.AxisX.ScrollBar.IsPositionedInside = false;
			chartArea3.AxisY.IsLabelAutoFit = false;
			chartArea3.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			chartArea3.AxisY.LabelStyle.Format = "C0";
			chartArea3.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea3.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea3.AxisY.MajorTickMark.Enabled = false;
			chartArea3.AxisY2.LabelStyle.Format = "C";
			chartArea3.BackColor = System.Drawing.Color.OldLace;
			chartArea3.BackSecondaryColor = System.Drawing.Color.White;
			chartArea3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea3.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
			chartArea3.CursorX.IsUserSelectionEnabled = true;
			chartArea3.Name = "Default";
			chartArea3.ShadowOffset = 5;
			chartArea4.AlignWithChartArea = "Default";
			chartArea4.AxisX.Interval = 1D;
			chartArea4.AxisX.LabelAutoFitStyle = ((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles)(((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.StaggeredLabels | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.LabelsAngleStep30) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.WordWrap)));
			chartArea4.AxisX.LabelStyle.Format = "dd MMM";
			chartArea4.AxisX.LabelStyle.Interval = 1D;
			chartArea4.AxisX.LabelStyle.IsStaggered = true;
			chartArea4.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea4.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea4.AxisX.MajorTickMark.Enabled = false;
			chartArea4.AxisX.ScaleView.MinSize = 14D;
			chartArea4.AxisX.ScaleView.Position = 1D;
			chartArea4.AxisX.ScaleView.Size = 1D;
			chartArea4.AxisX.ScaleView.SizeType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Days;
			chartArea4.AxisX.ScrollBar.ButtonStyle = System.Windows.Forms.DataVisualization.Charting.ScrollBarButtonStyles.SmallScroll;
			chartArea4.AxisX.ScrollBar.IsPositionedInside = false;
			chartArea4.AxisY.IsLabelAutoFit = false;
			chartArea4.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			chartArea4.AxisY.LabelStyle.Format = "C0";
			chartArea4.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea4.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea4.AxisY.MajorTickMark.Enabled = false;
			chartArea4.BackColor = System.Drawing.Color.OldLace;
			chartArea4.BackSecondaryColor = System.Drawing.Color.White;
			chartArea4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea4.Name = "Group";
			chartArea4.ShadowOffset = 5;
			this.chart1.ChartAreas.Add(chartArea3);
			this.chart1.ChartAreas.Add(chartArea4);
			this.chart1.Location = new System.Drawing.Point(0, 53);
			this.chart1.Name = "chart1";
			series3.ChartArea = "Default";
			series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			series3.Color = System.Drawing.Color.Red;
			series3.LabelFormat = "C0";
			series3.Name = "Daily";
			series4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
			series4.BorderWidth = 3;
			series4.ChartArea = "Group";
			series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
			series4.Color = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(65)))), ((int)(((byte)(140)))), ((int)(((byte)(240)))));
			series4.IsValueShownAsLabel = true;
			series4.LabelFormat = "C0";
			series4.MarkerSize = 8;
			series4.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
			series4.Name = "Group";
			series4.ShadowColor = System.Drawing.Color.Black;
			series4.ShadowOffset = 2;
			series4.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
			series4.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
			this.chart1.Series.Add(series3);
			this.chart1.Series.Add(series4);
			this.chart1.Size = new System.Drawing.Size(992, 412);
			this.chart1.TabIndex = 2;
			title4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(126)))), ((int)(((byte)(65)))));
			title4.DockedToChartArea = "Group";
			title4.IsDockedInsideChartArea = false;
			title4.Name = "titleGroup";
			title4.ShadowOffset = 5;
			title4.Text = "Grouped By Day";
			title5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(126)))), ((int)(((byte)(65)))));
			title5.DockedToChartArea = "Default";
			title5.IsDockedInsideChartArea = false;
			title5.Name = "titleDaily";
			title5.ShadowOffset = 5;
			title5.Text = "Daily";
			title6.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
			title6.Name = "ChartTitle";
			title6.Text = "Procedure Log Income";
			this.chart1.Titles.Add(title4);
			this.chart1.Titles.Add(title5);
			this.chart1.Titles.Add(title6);
			this.chart1.AxisViewChanged += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ViewEventArgs>(this.chart1_AxisViewChanged);
			// 
			// comboGroupBy
			// 
			this.comboGroupBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboGroupBy.FormattingEnabled = true;
			this.comboGroupBy.Location = new System.Drawing.Point(10, 16);
			this.comboGroupBy.Name = "comboGroupBy";
			this.comboGroupBy.Size = new System.Drawing.Size(82, 21);
			this.comboGroupBy.TabIndex = 3;
			this.comboGroupBy.SelectedIndexChanged += new System.EventHandler(this.comboGroupBy_SelectedIndexChanged);
			// 
			// checkShowDailyLabels
			// 
			this.checkShowDailyLabels.AutoSize = true;
			this.checkShowDailyLabels.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowDailyLabels.Location = new System.Drawing.Point(6, 18);
			this.checkShowDailyLabels.Name = "checkShowDailyLabels";
			this.checkShowDailyLabels.Size = new System.Drawing.Size(113, 17);
			this.checkShowDailyLabels.TabIndex = 5;
			this.checkShowDailyLabels.Text = "Show Daily Labels";
			this.checkShowDailyLabels.UseVisualStyleBackColor = true;
			this.checkShowDailyLabels.CheckedChanged += new System.EventHandler(this.checkShowDailyLabels_CheckedChanged);
			// 
			// checkShowGroupLabels
			// 
			this.checkShowGroupLabels.AutoSize = true;
			this.checkShowGroupLabels.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowGroupLabels.Checked = true;
			this.checkShowGroupLabels.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowGroupLabels.Location = new System.Drawing.Point(134, 18);
			this.checkShowGroupLabels.Name = "checkShowGroupLabels";
			this.checkShowGroupLabels.Size = new System.Drawing.Size(119, 17);
			this.checkShowGroupLabels.TabIndex = 6;
			this.checkShowGroupLabels.Text = "Show Group Labels";
			this.checkShowGroupLabels.UseVisualStyleBackColor = true;
			this.checkShowGroupLabels.CheckedChanged += new System.EventHandler(this.checkShowGroupLabels_CheckedChanged);
			// 
			// numberDailyLabelDensity
			// 
			this.numberDailyLabelDensity.Location = new System.Drawing.Point(370, 16);
			this.numberDailyLabelDensity.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.numberDailyLabelDensity.Name = "numberDailyLabelDensity";
			this.numberDailyLabelDensity.Size = new System.Drawing.Size(41, 20);
			this.numberDailyLabelDensity.TabIndex = 7;
			this.numberDailyLabelDensity.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
			this.numberDailyLabelDensity.ValueChanged += new System.EventHandler(this.numberDailyLabelDensity_ValueChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(259, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Daily Label Density:";
			// 
			// dateTimeFrom
			// 
			this.dateTimeFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateTimeFrom.Location = new System.Drawing.Point(48, 16);
			this.dateTimeFrom.Name = "dateTimeFrom";
			this.dateTimeFrom.Size = new System.Drawing.Size(113, 20);
			this.dateTimeFrom.TabIndex = 9;
			this.dateTimeFrom.ValueChanged += new System.EventHandler(this.dateTimeFilter_ValueChanged);
			// 
			// dateTimeTo
			// 
			this.dateTimeTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateTimeTo.Location = new System.Drawing.Point(200, 16);
			this.dateTimeTo.Name = "dateTimeTo";
			this.dateTimeTo.Size = new System.Drawing.Size(113, 20);
			this.dateTimeTo.TabIndex = 11;
			this.dateTimeTo.ValueChanged += new System.EventHandler(this.dateTimeFilter_ValueChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.dateTimeTo);
			this.groupBox2.Controls.Add(this.dateTimeFrom);
			this.groupBox2.Location = new System.Drawing.Point(530, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(321, 44);
			this.groupBox2.TabIndex = 17;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Filter Dates";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(9, 20);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(33, 13);
			this.label5.TabIndex = 10;
			this.label5.Text = "From:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(171, 20);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(23, 13);
			this.label6.TabIndex = 12;
			this.label6.Text = "To:";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.comboGroupBy);
			this.groupBox3.Location = new System.Drawing.Point(3, 3);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(98, 44);
			this.groupBox3.TabIndex = 18;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Group By";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.checkShowDailyLabels);
			this.groupBox1.Controls.Add(this.checkShowGroupLabels);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.numberDailyLabelDensity);
			this.groupBox1.Location = new System.Drawing.Point(107, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(417, 44);
			this.groupBox1.TabIndex = 18;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "View";
			// 
			// GraphIncome
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.chart1);
			this.Name = "IncomeChart";
			this.Size = new System.Drawing.Size(992, 468);
			((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numberDailyLabelDensity)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
		private System.Windows.Forms.ComboBox comboGroupBy;
		private System.Windows.Forms.CheckBox checkShowDailyLabels;
		private System.Windows.Forms.CheckBox checkShowGroupLabels;
		private System.Windows.Forms.NumericUpDown numberDailyLabelDensity;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.DateTimePicker dateTimeFrom;
		private System.Windows.Forms.DateTimePicker dateTimeTo;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox1;

	}
}
