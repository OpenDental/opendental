namespace OpenDentalGraph {
	partial class GraphQuantityOverTimeFilter {
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
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.splitContainerOptions = new System.Windows.Forms.SplitContainer();
			this.groupingOptionsCtrl1 = new OpenDentalGraph.GroupingOptionsCtrl();
			this.graph = new OpenDentalGraph.GraphQuantityOverTime();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerOptions)).BeginInit();
			this.splitContainerOptions.Panel1.SuspendLayout();
			this.splitContainerOptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer.IsSplitterFixed = true;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.splitContainerOptions);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.graph);
			this.splitContainer.Size = new System.Drawing.Size(788, 410);
			this.splitContainer.SplitterDistance = 60;
			this.splitContainer.SplitterWidth = 1;
			this.splitContainer.TabIndex = 9;
			// 
			// splitContainerOptions
			// 
			this.splitContainerOptions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerOptions.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainerOptions.Location = new System.Drawing.Point(0, 0);
			this.splitContainerOptions.Name = "splitContainerOptions";
			// 
			// splitContainerOptions.Panel1
			// 
			this.splitContainerOptions.Panel1.Controls.Add(this.groupingOptionsCtrl1);
			this.splitContainerOptions.Size = new System.Drawing.Size(788, 60);
			this.splitContainerOptions.SplitterDistance = 120;
			this.splitContainerOptions.TabIndex = 0;
			// 
			// groupingOptionsCtrl1
			// 
			this.groupingOptionsCtrl1.CurGrouping = OpenDentalGraph.GroupingOptionsCtrl.Grouping.provider;
			this.groupingOptionsCtrl1.Location = new System.Drawing.Point(5, 0);
			this.groupingOptionsCtrl1.Name = "groupingOptionsCtrl1";
			this.groupingOptionsCtrl1.Size = new System.Drawing.Size(112, 60);
			this.groupingOptionsCtrl1.TabIndex = 0;
			this.groupingOptionsCtrl1.InputsChanged += new System.EventHandler(this.OnFormInputsChanged);
			// 
			// graph
			// 
			this.graph.BackColor = System.Drawing.Color.Transparent;
			this.graph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.graph.BreakdownPref = OpenDentalGraph.Enumerations.BreakdownType.items;
			this.graph.BreakdownVal = 5;
			this.graph.ChartSubTitle = "";
			this.graph.CountItemDescription = "Completed Procedures";
			this.graph.DateFrom = new System.DateTime(1880, 1, 1, 0, 0, 0, 0);
			this.graph.DateTo = new System.DateTime(2180, 1, 1, 0, 0, 0, 0);
			this.graph.Dock = System.Windows.Forms.DockStyle.Fill;
			this.graph.GraphTitle = "Production";
			this.graph.GroupByType = System.Windows.Forms.DataVisualization.Charting.IntervalType.Weeks;
			this.graph.IsLoading = false;
			this.graph.LegendDock = OpenDentalGraph.Enumerations.LegendDockType.Bottom;
			this.graph.LegendTitle = "Provider";
			this.graph.Location = new System.Drawing.Point(0, 0);
			this.graph.MoneyItemDescription = "Income";
			this.graph.Name = "graph";
			this.graph.QtyType = OpenDentalGraph.Enumerations.QuantityType.money;
			this.graph.QuickRangePref = OpenDentalGraph.Enumerations.QuickRange.allTime;
			this.graph.SeriesType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedArea;
			this.graph.ShowFilters = true;
			this.graph.Size = new System.Drawing.Size(788, 349);
			this.graph.TabIndex = 1;
			this.graph.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.graph.UseBuiltInColors = false;
			this.graph.OnGetGetColor = new OpenDentalGraph.GraphQuantityOverTime.OnGetColorArgs(this.graph_OnGetGetColor);
			// 
			// GraphQuantityOverTimeFilter
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.splitContainer);
			this.Name = "GraphQuantityOverTimeFilter";
			this.Size = new System.Drawing.Size(788, 410);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.splitContainerOptions.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerOptions)).EndInit();
			this.splitContainerOptions.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.SplitContainer splitContainer;
		private GraphQuantityOverTime graph;
		private System.Windows.Forms.SplitContainer splitContainerOptions;
		private GroupingOptionsCtrl groupingOptionsCtrl1;
	}
}
