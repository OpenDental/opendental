using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace OpenDentalGraph {
	public partial class DashboardPanelCtrl:UserControl {
		#region Private Data
		private DashboardLayout _dbItem;
		private bool _hasUnsavedChanges=false;
		#endregion

		#region Properties
		public bool CanDelete {
			get {
				for(int columnIndex = 0;columnIndex<tableLayoutPanel.ColumnCount;columnIndex++) {
					for(int rowIndex = 0;rowIndex<tableLayoutPanel.RowCount;rowIndex++) {
						Control c=tableLayoutPanel.GetControlFromPosition(columnIndex, rowIndex);
						if(c==null||!(c is DashboardCellCtrl)) {
							continue;
						}
						if(((DashboardCellCtrl)c).HasDockedControl) {
							return false;
						}
					}
				}
				return true;
			}
		}
		public int Rows { get { return tableLayoutPanel.RowCount; } }
		public int Columns { get { return tableLayoutPanel.ColumnCount; } }
		public int TabOrder { get; set; }
		///<summary>Setter only works to set to false. Setting to true does nothing. This is handled internally by the control itself.
		///Getter returns true if new 1) Any cells have HasUnsavedChanges==true 2) New row added 3) New column added 4) Row deleted 5) Column deleted 6) Cell deleted.</summary>
		public bool HasUnsavedChanges {
			get {
				if(_hasUnsavedChanges) { //Row, column, or cell layout has change.
					return true;
				}				
				//Check to see if individual cell contents have changed.
				for(int columnIndex = 0;columnIndex<tableLayoutPanel.ColumnCount;columnIndex++) {
					for(int rowIndex = 0;rowIndex<tableLayoutPanel.RowCount;rowIndex++) {
						Control c=tableLayoutPanel.GetControlFromPosition(columnIndex, rowIndex);
						if(c==null||!(c is DashboardCellCtrl)) {
							continue;
						}						
						DashboardCellCtrl dashboardCell=(DashboardCellCtrl)c;
						if(dashboardCell.HasUnsavedChanges) { //Update.
							return true;
						}
						if(!dashboardCell.HasDockedControl) {
							continue;
						}
					}
				}
				return false;
			}
			set {
				if(value) { //Only allow turn off (not on).
					return;
				}
				//Check to see if individual cell contents have changed.
				for(int columnIndex = 0;columnIndex<tableLayoutPanel.ColumnCount;columnIndex++) {
					for(int rowIndex = 0;rowIndex<tableLayoutPanel.RowCount;rowIndex++) {
						Control c=tableLayoutPanel.GetControlFromPosition(columnIndex, rowIndex);
						if(c==null||!(c is DashboardCellCtrl)) {
							continue;
						}
						DashboardCellCtrl dashboardCell=(DashboardCellCtrl)c;
						dashboardCell.HasUnsavedChanges=false;
					}
				}
				_hasUnsavedChanges=false;
			}
		}		
		public List<DashboardCell> Cells {
			get {
				List<DashboardCell> ret=new List<DashboardCell>();
				for(int columnIndex = 0;columnIndex<tableLayoutPanel.ColumnCount;columnIndex++) {
					for(int rowIndex = 0;rowIndex<tableLayoutPanel.RowCount;rowIndex++) {
						Control c=tableLayoutPanel.GetControlFromPosition(columnIndex, rowIndex);
						if(c==null||!(c is DashboardCellCtrl)) {
							continue;
						}
						DashboardCellCtrl dashboardCell=(DashboardCellCtrl)c;
						if(!dashboardCell.HasDockedControl) {
							continue;
						}
						DashboardCell cell=new DashboardCell();
						cell.IsNew=true;
						if(dashboardCell.DockedControlTag!=null&&dashboardCell.DockedControlTag is DashboardCell) {
							//Use the input db cell.
							cell=(DashboardCell)dashboardCell.DockedControlTag;
							cell.IsNew=false;							
						}
						cell.CellColumn=columnIndex;
						cell.CellRow=rowIndex;						
						if(!(dashboardCell.DockedControl is IDashboardDockContainer)) {
							throw new Exception("Unsupported DashboardCell type: "+dashboardCell.DockedControl.GetType().ToString()+". Must implement IDashboardDockContainer.");
						}
						IDashboardDockContainer dockContainer=(IDashboardDockContainer)dashboardCell.DockedControl;
						cell.CellType=dockContainer.GetCellType();
						cell.CellSettings=dockContainer.GetCellSettings();						
						ret.Add(cell);
					}
				}
				return ret;
			}
		}

		public List<GraphQuantityOverTimeFilter> Graphs {
			get {
				List<GraphQuantityOverTimeFilter> ret=new List<GraphQuantityOverTimeFilter>();
				for(int columnIndex = 0;columnIndex<tableLayoutPanel.ColumnCount;columnIndex++) {
					for(int rowIndex = 0;rowIndex<tableLayoutPanel.RowCount;rowIndex++) {
						Control c=tableLayoutPanel.GetControlFromPosition(columnIndex, rowIndex);
						if(c==null||!(c is DashboardCellCtrl)) {
							continue;
						}
						DashboardCellCtrl dashboardCell=(DashboardCellCtrl)c;
						if(!dashboardCell.HasDockedControl) {
							continue;
						}
						if(!(dashboardCell.DockedControl is GraphQuantityOverTimeFilter)) {
							continue;
						}
						ret.Add((GraphQuantityOverTimeFilter)dashboardCell.DockedControl);
					}
				}
				return ret;
			}
		}

		public DashboardLayout DbItem {
			get { return _dbItem; }
		}
		public bool IsEditMode {
			get { return !splitContainerAddColumn.Panel2Collapsed; }
			set {
				splitContainerAddColumn.Panel2Collapsed=!value;
				splitContainerAddRow.Panel2Collapsed=!value;
				for(int columnIndex = 0;columnIndex<tableLayoutPanel.ColumnCount;columnIndex++) {
					for(int rowIndex = 0;rowIndex<tableLayoutPanel.RowCount;rowIndex++) {
						Control c=tableLayoutPanel.GetControlFromPosition(columnIndex, rowIndex);
						if(c==null||!(c is DashboardCellCtrl)) {
							continue;
						}
						DashboardCellCtrl dashboardCell=(DashboardCellCtrl)c;
						dashboardCell.IsEditMode=IsEditMode;
					}
				}
			}
		}
		#endregion

		public DashboardPanelCtrl(DashboardLayout dbItem=null) {
			InitializeComponent();
			_dbItem=dbItem;
		}

		#region Public Methods
		///<summary>Get all the graphs associated to the current dashboardLayout.
		///For each row and column, get the chart and associate it to it's location.</summary>
		public Dictionary<Point,Chart> GetGraphsAsDictionary() {
			Dictionary<Point,Chart> ret=new Dictionary<Point, Chart>();
			for(int rows = 0;rows<Rows;rows++) {
				for(int cols = 0;cols<Columns;cols++) {
					GraphQuantityOverTimeFilter graph=GetGraphAtPoint(rows,cols);
					if(graph!=null) {
						ret.Add(new Point(rows,cols),graph.Graph.GetChartForPrinting());
					}
				}
			}
			return ret;
		}

		///<summary>Returns null if chart not found at point.</summary>
		public GraphQuantityOverTimeFilter GetGraphAtPoint(int row,int col) {
			Control c = tableLayoutPanel.GetControlFromPosition(col,row);
			if(c==null||!(c is DashboardCellCtrl)) {
				return null;
			}
			DashboardCellCtrl dashboardCell=(DashboardCellCtrl)c;
			if(!dashboardCell.HasDockedControl || !(dashboardCell.DockedControl is GraphQuantityOverTimeFilter)) {
				return null;
			}
			return (GraphQuantityOverTimeFilter)dashboardCell.DockedControl;
		}

		public void SetHightlightedAllCells(bool isHighlighted) {
			for(int columnIndex = 0;columnIndex<tableLayoutPanel.ColumnCount;columnIndex++) {
				for(int rowIndex = 0;rowIndex<tableLayoutPanel.RowCount;rowIndex++) {
					Control c=tableLayoutPanel.GetControlFromPosition(columnIndex, rowIndex);
					if(c==null||!(c is DashboardCellCtrl)) {
						continue;
					}
					((DashboardCellCtrl)c).IsHighlighted=isHighlighted;
				}
			}
		}

		public void SetCellLayout(int rows,int columns,List<DashboardCell> cells,GraphQuantityOverTime.OnGetColorFromSeriesGraphTypeArgs onGetSeriesColor,GraphQuantityOverTimeFilter.GetGraphPointsForHQArgs onGetODGraphPointsArgs) {
			try {
				tableLayoutPanel.SuspendLayout();
				tableLayoutPanel.Controls.Clear();
				tableLayoutPanel.RowStyles.Clear();
				tableLayoutPanel.ColumnStyles.Clear();
				tableLayoutPanel.RowCount=rows;
				tableLayoutPanel.ColumnCount=columns;
				for(int rowIndex = 0;rowIndex<rows;rowIndex++) {
					tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent));
					tableLayoutPanel.RowStyles[rowIndex].Height=100/(float)rows;
				}
				for(int columnIndex = 0;columnIndex<columns;columnIndex++) {
					tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent));
					tableLayoutPanel.ColumnStyles[columnIndex].Width=100/(float)columns;
				}
				for(int rowIndex = 0;rowIndex<rows;rowIndex++) {
					for(int columnIndex = 0;columnIndex<columns;columnIndex++) {
						DashboardCell cell=cells.FirstOrDefault(x => x.CellColumn==columnIndex&&x.CellRow==rowIndex);
						DashboardDockContainer cellHolder=null;
						if(cell!=null) {
							//Currently all CellTypes return GraphQuantityOverTimeFilter. Add a switch here if we ever want to dock a different control type.
							cellHolder=new GraphQuantityOverTimeFilter(cell.CellType,cell.CellSettings,onGetSeriesColor,onGetODGraphPointsArgs).CreateDashboardDockContainer(cell);
						}
						AddCell(columnIndex,rowIndex,cellHolder);
					}
				}
			}			
			finally {
				tableLayoutPanel.ResumeLayout();
			}
		}
		#endregion

		#region Manage Cells
		private int GetRowIndex(object sender) {
			if(!(sender is DashboardCellCtrl)) {
				return -1;
			}
			return tableLayoutPanel.GetRow(GetCell(sender));
		}

		private int GetColumnIndex(object sender) {
			if(!(sender is DashboardCellCtrl)) {
				return -1;
			}
			return tableLayoutPanel.GetColumn(GetCell(sender));
		}

		private DashboardCellCtrl GetCell(object sender) {
			if(!(sender is DashboardCellCtrl)) {
				return null;
			}
			return (DashboardCellCtrl)sender;
		}
				
		private void SetHightlightedColumn(object sender,bool isHighlighted) {
			int columnIndex=GetColumnIndex(sender);
			if(columnIndex<0) {
				return;
			}
			for(int rowIndex = 0;rowIndex<tableLayoutPanel.RowCount;rowIndex++) {
				Control c=tableLayoutPanel.GetControlFromPosition(columnIndex, rowIndex);
				if(c==null||!(c is DashboardCellCtrl)) {
					continue;
				}
				((DashboardCellCtrl)c).IsHighlighted=isHighlighted;
			}
		}

		private void SetHightlightedRow(object sender,bool isHighlighted) {
			int rowIndex=GetRowIndex(sender);
			if(rowIndex<0) {
				return;
			}
			for(int columnIndex = 0;columnIndex<tableLayoutPanel.ColumnCount;columnIndex++) {
				Control c=tableLayoutPanel.GetControlFromPosition(columnIndex, rowIndex);
				if(c==null||!(c is DashboardCellCtrl)) {
					continue;
				}
				((DashboardCellCtrl)c).IsHighlighted=isHighlighted;
			}
		}

		private bool ColumnHasItems(int columnIndex) {
			for(int rowIndex = 0;rowIndex<tableLayoutPanel.RowCount;rowIndex++) {
				Control c=tableLayoutPanel.GetControlFromPosition(columnIndex, rowIndex);
				if(c!=null&&(c is DashboardCellCtrl)&&((DashboardCellCtrl)c).HasDockedControl) {
					return true;
				}
			}
			return false;
		}

		private bool RowHasItems(int rowIndex) {
			for(int columnIndex = 0;columnIndex<tableLayoutPanel.ColumnCount;columnIndex++) {
				Control c=tableLayoutPanel.GetControlFromPosition(columnIndex, rowIndex);
				if(c!=null&&(c is DashboardCellCtrl)&&((DashboardCellCtrl)c).HasDockedControl) {
					return true;
				}
			}
			return false;
		}
		#endregion

		#region Cell Events
		private void dashboardCell_DeleteColumnButtonMouseEnter(object sender,EventArgs e) {
			SetHightlightedColumn(sender,true);
		}

		private void dashboardCell_DeleteColumnButtonMouseLeave(object sender,EventArgs e) {
			SetHightlightedColumn(sender,false);
		}

		private void dashboardCell_DeleteRowButtonMouseEnter(object sender,EventArgs e) {
			SetHightlightedRow(sender,true);
		}

		private void dashboardCell_DeleteRowButtonMouseLeave(object sender,EventArgs e) {
			SetHightlightedRow(sender,false);
		}			
		#endregion

		#region Button Clicks
		private void AddCell(int columnIndex,int rowIndex,DashboardDockContainer controlHolder=null) {
			DashboardCellCtrl cell=new DashboardCellCtrl(controlHolder);
			cell.IsEditMode=IsEditMode;
			cell.Dock=DockStyle.Fill;
			cell.DeleteCellButtonClick+=dashboardCell_DeleteCellButtonClick;
			cell.DeleteColumnButtonClick+=dashboardCell_DeleteColumnButtonClick;
			cell.DeleteColumnButtonMouseEnter+=dashboardCell_DeleteColumnButtonMouseEnter;
			cell.DeleteColumnButtonMouseLeave+=dashboardCell_DeleteColumnButtonMouseLeave;
			cell.DeleteRowButtonClick+=dashboardCell_DeleteRowButtonClick;
			cell.DeleteRowButtonMouseEnter+=dashboardCell_DeleteRowButtonMouseEnter;
			cell.DeleteRowButtonMouseLeave+=dashboardCell_DeleteRowButtonMouseLeave;
			tableLayoutPanel.Controls.Add(cell,columnIndex,rowIndex);
		}
				
		private void butAddRow_Click(object sender,EventArgs e) {
			try {
				tableLayoutPanel.SuspendLayout();
				int newRowIndex=tableLayoutPanel.RowCount;
				tableLayoutPanel.RowCount=newRowIndex+1;
				tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent));
				for(int rowIndex = 0;rowIndex<tableLayoutPanel.RowStyles.Count;rowIndex++) {
					tableLayoutPanel.RowStyles[rowIndex].Height=100/(float)tableLayoutPanel.RowStyles.Count;
				}
				for(int columnIndex = 0;columnIndex<tableLayoutPanel.ColumnCount;columnIndex++) {
					AddCell(columnIndex,newRowIndex);
				}
				_hasUnsavedChanges=true;
			}
			finally {
				tableLayoutPanel.ResumeLayout();
			}
		}

		private void butAddColumn_Click(object sender,EventArgs e) {
			try {
				tableLayoutPanel.SuspendLayout();
				int newColumnIndex=tableLayoutPanel.ColumnCount;
				tableLayoutPanel.ColumnCount=newColumnIndex+1;
				tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent));
				for(int columnIndex = 0;columnIndex<tableLayoutPanel.ColumnStyles.Count;columnIndex++) {
					tableLayoutPanel.ColumnStyles[columnIndex].Width=100/(float)tableLayoutPanel.ColumnStyles.Count;
				}
				for(int rowIndex = 0;rowIndex<tableLayoutPanel.RowCount;rowIndex++) {
					AddCell(newColumnIndex,rowIndex);
				}
				_hasUnsavedChanges=true;
			}
			finally {
				tableLayoutPanel.ResumeLayout();
			}
		}

		private void dashboardCell_DeleteRowButtonClick(object sender,EventArgs e) {
			try {
				tableLayoutPanel.SuspendLayout();
				int rowIndex=GetRowIndex(sender);
				if(RowHasItems(rowIndex)) {
					MessageBox.Show("Row "+rowIndex.ToString()+" has items. Remove all items from row before continuing.");
					return;
				}
				if(tableLayoutPanel.RowCount==1) {
					MessageBox.Show("Dashboard must contain a minimum of 1 row.");
					return;
				}
				tableLayoutPanel.RowStyles.RemoveAt(rowIndex);
				for(int columnIndex = 0;columnIndex<tableLayoutPanel.ColumnCount;columnIndex++) {
					Control c=tableLayoutPanel.GetControlFromPosition(columnIndex, rowIndex);
					tableLayoutPanel.Controls.Remove(c);
				}
				for(int i = rowIndex+1;i<tableLayoutPanel.RowCount;i++) {
					for(int columnIndex = 0;columnIndex<tableLayoutPanel.ColumnCount;columnIndex++) {
						Control c=tableLayoutPanel.GetControlFromPosition(columnIndex, i);
						tableLayoutPanel.SetRow(c,i-1);
					}
				}
				tableLayoutPanel.RowCount--;
				_hasUnsavedChanges=true;
			}
			finally {
				tableLayoutPanel.ResumeLayout();
			}
		}

		private void dashboardCell_DeleteColumnButtonClick(object sender,EventArgs e) {
			try {
				tableLayoutPanel.SuspendLayout();
				int columnIndex=GetColumnIndex(sender);
				if(ColumnHasItems(columnIndex)) {
					MessageBox.Show("Column "+columnIndex.ToString()+" has items. Remove all items from column before continuing.");
					return;
				}
				if(tableLayoutPanel.ColumnCount==1) {
					MessageBox.Show("Dashboard must contain a minimum of 1 column.");
					return;
				}
				tableLayoutPanel.ColumnStyles.RemoveAt(columnIndex);
				for(int rowIndex = 0;rowIndex<tableLayoutPanel.RowCount;rowIndex++) {
					Control c=tableLayoutPanel.GetControlFromPosition(columnIndex, rowIndex);
					tableLayoutPanel.Controls.Remove(c);
				}
				for(int i = columnIndex+1;i<tableLayoutPanel.ColumnCount;i++) {
					for(int rowIndex = 0;rowIndex<tableLayoutPanel.RowCount;rowIndex++) {
						Control c=tableLayoutPanel.GetControlFromPosition(i, rowIndex);
						tableLayoutPanel.SetColumn(c,i-1);
					}
				}
				tableLayoutPanel.ColumnCount--;
				_hasUnsavedChanges=true;
			}
			finally {
				tableLayoutPanel.ResumeLayout();
			}
		}

		private void dashboardCell_DeleteCellButtonClick(object sender,EventArgs e) {
			DashboardCellCtrl cell=GetCell(sender);
			if(cell==null) {
				return;
			}
			cell.RemoveDockedControl();
			_hasUnsavedChanges=true;
		}
		#endregion
	}
}
