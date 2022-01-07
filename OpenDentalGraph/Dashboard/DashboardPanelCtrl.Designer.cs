namespace OpenDentalGraph {
	partial class DashboardPanelCtrl {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.splitContainerAddRow = new System.Windows.Forms.SplitContainer();
			this.splitContainerAddColumn = new System.Windows.Forms.SplitContainer();
			this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.dashboardCell = new OpenDentalGraph.DashboardCellCtrl();
			this.butAddColumn = new System.Windows.Forms.Button();
			this.butAddRow = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerAddRow)).BeginInit();
			this.splitContainerAddRow.Panel1.SuspendLayout();
			this.splitContainerAddRow.Panel2.SuspendLayout();
			this.splitContainerAddRow.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerAddColumn)).BeginInit();
			this.splitContainerAddColumn.Panel1.SuspendLayout();
			this.splitContainerAddColumn.Panel2.SuspendLayout();
			this.splitContainerAddColumn.SuspendLayout();
			this.tableLayoutPanel.SuspendLayout();
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
			this.splitContainer.Panel1Collapsed = true;
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.splitContainerAddRow);
			this.splitContainer.Size = new System.Drawing.Size(411, 196);
			this.splitContainer.SplitterDistance = 25;
			this.splitContainer.TabIndex = 0;
			// 
			// splitContainerAddRow
			// 
			this.splitContainerAddRow.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerAddRow.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainerAddRow.IsSplitterFixed = true;
			this.splitContainerAddRow.Location = new System.Drawing.Point(0, 0);
			this.splitContainerAddRow.Name = "splitContainerAddRow";
			this.splitContainerAddRow.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerAddRow.Panel1
			// 
			this.splitContainerAddRow.Panel1.Controls.Add(this.splitContainerAddColumn);
			// 
			// splitContainerAddRow.Panel2
			// 
			this.splitContainerAddRow.Panel2.Controls.Add(this.butAddRow);
			this.splitContainerAddRow.Size = new System.Drawing.Size(411, 196);
			this.splitContainerAddRow.SplitterDistance = 159;
			this.splitContainerAddRow.SplitterWidth = 1;
			this.splitContainerAddRow.TabIndex = 2;
			// 
			// splitContainerAddColumn
			// 
			this.splitContainerAddColumn.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerAddColumn.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainerAddColumn.IsSplitterFixed = true;
			this.splitContainerAddColumn.Location = new System.Drawing.Point(0, 0);
			this.splitContainerAddColumn.Name = "splitContainerAddColumn";
			// 
			// splitContainerAddColumn.Panel1
			// 
			this.splitContainerAddColumn.Panel1.Controls.Add(this.tableLayoutPanel);
			// 
			// splitContainerAddColumn.Panel2
			// 
			this.splitContainerAddColumn.Panel2.Controls.Add(this.butAddColumn);
			this.splitContainerAddColumn.Size = new System.Drawing.Size(411, 159);
			this.splitContainerAddColumn.SplitterDistance = 364;
			this.splitContainerAddColumn.SplitterWidth = 1;
			this.splitContainerAddColumn.TabIndex = 1;
			// 
			// tableLayoutPanel
			// 
			this.tableLayoutPanel.AllowDrop = true;
			this.tableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
			this.tableLayoutPanel.ColumnCount = 1;
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel.Controls.Add(this.dashboardCell, 0, 0);
			this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 1;
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel.Size = new System.Drawing.Size(364, 159);
			this.tableLayoutPanel.TabIndex = 0;
			// 
			// dashboardCell
			// 
			this.dashboardCell.AllowDrop = true;
			this.dashboardCell.BackColor = System.Drawing.SystemColors.Control;
			this.dashboardCell.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dashboardCell.IsHighlighted = false;
			this.dashboardCell.Location = new System.Drawing.Point(1, 1);
			this.dashboardCell.Margin = new System.Windows.Forms.Padding(0);
			this.dashboardCell.Name = "dashboardCell";
			this.dashboardCell.Padding = new System.Windows.Forms.Padding(3);
			this.dashboardCell.Size = new System.Drawing.Size(362, 157);
			this.dashboardCell.TabIndex = 0;
			this.dashboardCell.DeleteColumnButtonMouseEnter += new System.EventHandler(this.dashboardCell_DeleteColumnButtonMouseEnter);
			this.dashboardCell.DeleteColumnButtonMouseLeave += new System.EventHandler(this.dashboardCell_DeleteColumnButtonMouseLeave);
			this.dashboardCell.DeleteRowButtonMouseEnter += new System.EventHandler(this.dashboardCell_DeleteRowButtonMouseEnter);
			this.dashboardCell.DeleteRowButtonMouseLeave += new System.EventHandler(this.dashboardCell_DeleteRowButtonMouseLeave);
			this.dashboardCell.DeleteColumnButtonClick += new System.EventHandler(this.dashboardCell_DeleteColumnButtonClick);
			this.dashboardCell.DeleteRowButtonClick += new System.EventHandler(this.dashboardCell_DeleteRowButtonClick);
			this.dashboardCell.DeleteCellButtonClick += new System.EventHandler(this.dashboardCell_DeleteCellButtonClick);
			// 
			// butAddColumn
			// 
			this.butAddColumn.Dock = System.Windows.Forms.DockStyle.Fill;
			this.butAddColumn.Image = global::OpenDentalGraph.Properties.Resources.addColumn;
			this.butAddColumn.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
			this.butAddColumn.Location = new System.Drawing.Point(0, 0);
			this.butAddColumn.Name = "butAddColumn";
			this.butAddColumn.Size = new System.Drawing.Size(46, 159);
			this.butAddColumn.TabIndex = 1;
			this.butAddColumn.Text = "Add Col";
			this.butAddColumn.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.butAddColumn.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
			this.butAddColumn.UseVisualStyleBackColor = true;
			this.butAddColumn.Click += new System.EventHandler(this.butAddColumn_Click);
			// 
			// butAddRow
			// 
			this.butAddRow.Dock = System.Windows.Forms.DockStyle.Fill;
			this.butAddRow.Image = global::OpenDentalGraph.Properties.Resources.addRow;
			this.butAddRow.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddRow.Location = new System.Drawing.Point(0, 0);
			this.butAddRow.Name = "butAddRow";
			this.butAddRow.Size = new System.Drawing.Size(411, 36);
			this.butAddRow.TabIndex = 0;
			this.butAddRow.Text = "Add Row";
			this.butAddRow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butAddRow.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.butAddRow.UseVisualStyleBackColor = true;
			this.butAddRow.Click += new System.EventHandler(this.butAddRow_Click);
			// 
			// DashboardPanelCtrl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer);
			this.Name = "DashboardPanelCtrl";
			this.Size = new System.Drawing.Size(411, 196);
			this.splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.splitContainerAddRow.Panel1.ResumeLayout(false);
			this.splitContainerAddRow.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerAddRow)).EndInit();
			this.splitContainerAddRow.ResumeLayout(false);
			this.splitContainerAddColumn.Panel1.ResumeLayout(false);
			this.splitContainerAddColumn.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerAddColumn)).EndInit();
			this.splitContainerAddColumn.ResumeLayout(false);
			this.tableLayoutPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.Button butAddColumn;
		private System.Windows.Forms.Button butAddRow;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private DashboardCellCtrl dashboardCell;
		private System.Windows.Forms.SplitContainer splitContainerAddColumn;
		private System.Windows.Forms.SplitContainer splitContainerAddRow;
	}
}
