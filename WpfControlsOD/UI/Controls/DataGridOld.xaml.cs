using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfControls.UI{
/*
Jordan is the only one allowed to edit this file.

How to use the DataGrid control:

			gridMain.ColumnsClear();
			GridColumn gridColumn = new GridColumn(Lans.g("Table...","colName"),100);
			gridMain.ColumnAdd(gridColumn);
			gridColumn=new GridColumn(Lans.g("Table...","colName"),100,HorizontalAlignment.Center);
			gridColumn.IsWidthDynamic=true;//For the last column. This is new. This was not required in WinForms.
			gridMain.ColumnAdd(gridColumn);
			List<GridRow> listGridRows = new List<GridRow>();
			for(int i=0;i<_list.Count;i++){
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(_list[i].);
				gridRow.Cells.Add("");
				gridRow.Cells.Add(new GridCell(""){ColorBackG=.., etc});//object initializer is not allowed
			  GridCell cell=new GridCell("");//instead of above line
				cell.ColorBackG=...;
				gridRow.Cells.Add(cell);
				gridRow.Cells.Add(new GridCell(_list[i].IsHidden?"X":""));
				gridRow.Tag=_list[i];
			}
			gridMain.SetListGridRows(listGridRows);

*/
	///<summary></summary>
	public partial class DataGridOld:UserControl {
		private string title;

		//public List<GridColumn> ListGridColumns=new List<GridColumn>();

		public DataGridOld() {
			InitializeComponent();
			//textBlockTitle.FontWeight=FontWeights.Medium;//Bold is ugly. DemiBold and Medium look the same: slightly bold.
			dataGrid.AutoGenerateColumns=false;
			dataGrid.Background=Brushes.White;
			//dataGrid.CanUserReorderColumns=false;
			//dataGrid.CanUserResizeColumns=false;
			//dataGrid.CanUserSortColumns=false;
			float pointToDip = 96f/72f;
			dataGrid.ColumnHeaderHeight=11*pointToDip;//this seems wrong
			//dataGrid.IsEnabled=false;//just for testing
			dataGrid.GridLinesVisibility=DataGridGridLinesVisibility.None;
			dataGrid.HeadersVisibility=DataGridHeadersVisibility.Column;//hides the row selectors at the left
			dataGrid.HorizontalGridLinesBrush=new SolidColorBrush(Color.FromRgb(220,220,220));
			//dataGrid.IsReadOnly=true;//no editing allowed within grid unless we separately explicity allow it.
			dataGrid.SelectionMode=DataGridSelectionMode.Single;
			dataGrid.SelectionUnit=DataGridSelectionUnit.FullRow;
			dataGrid.VerticalGridLinesBrush=new SolidColorBrush(Color.FromRgb(220,220,220));
			VirtualizingPanel.SetScrollUnit(dataGrid,ScrollUnit.Pixel);
			dataGrid.LoadingRow+=DataGrid_LoadingRow;
			dataGrid.MouseDoubleClick+=DataGrid_MouseDoubleClick;
			//dataGrid.MouseMove+=DataGrid_MouseMove;
		}

		private void DataGrid_MouseMove(object sender,MouseEventArgs e) {
			/*
			Point pointMouse=e.GetPosition(dataGrid);
			DependencyObject dependencyObject = dataGrid.InputHitTest(pointMouse) as DependencyObject;
			while(true){
				if(dependencyObject is null){
					break;
				}
				if(dependencyObject is DataGridRow){
					break;
				}
				dependencyObject=VisualTreeHelper.GetParent(dependencyObject);
			}
			if(dependencyObject is null){
				return;
			}
			DataGridRow dataGridRow=(DataGridRow)dependencyObject;
			if(dataGridRow.Template is null){
				return;
			}
			Border border=dataGridRow.Template.FindName("DGR_Border",dataGridRow) as Border;
			border.Background=Brushes.Red;*/
			//this has two problems:
			//1: most of it is blocked by something like the cell background, so all I get is a hint of red at the edges.
			//2. I'm not reverting their color so they stay red.
		}

		#region Events - raise
		public event EventHandler<GridClickEventArgs> CellDoubleClick;
		#endregion Events - raise

		#region Properties - Public
		///<summary>Set true to allow user to click on column headers to sort rows, alternating between ascending and descending.</summary>
		[Category("OD")]
		[Description("Set true to allow user to click on column headers to sort rows, alternating between ascending and descending.")]
		[DefaultValue(false)]
		public bool AllowSortingByColumn {
			get => dataGrid.CanUserSortColumns;
			set => dataGrid.CanUserSortColumns=value;
		}

		///<summary>None, OneRow, OneCell, MultiExtended. Default is OneRow.</summary>
		[Category("OD")]
		[Description("None, OneRow, OneCell, MultiExtended. Default is OneRow.")]
		[DefaultValue(typeof(GridSelectionMode),"OneRow")]
		public GridSelectionMode SelectionMode {
			get {
				//todo: Add support for None. Can't do it by setting IsDisabled false because then the scrollbar doesn't work
				//We'll probably need to do it like this, where we set a properties on DataGridRow.
				//https://stackoverflow.com/questions/2496814/disable-selecting-in-wpf-datagrid
				//We might use a trigger for SelectionMode, but I would rather just change the style here manually.
				if(dataGrid.SelectionMode==DataGridSelectionMode.Single) {
					if(dataGrid.SelectionUnit==DataGridSelectionUnit.Cell) {
						return GridSelectionMode.OneCell;
					}
					if(dataGrid.SelectionUnit==DataGridSelectionUnit.FullRow) {
						return GridSelectionMode.OneRow;
					}
				}
				if(dataGrid.SelectionMode==DataGridSelectionMode.Extended) {
					if(dataGrid.SelectionUnit==DataGridSelectionUnit.FullRow) {
						return GridSelectionMode.MultiExtended;
					}
				}
				throw new Exception("SelectionMode not supported.");//this shouldn't happen
			}
			set {
				if(value==GridSelectionMode.OneCell) {
					dataGrid.SelectionMode=DataGridSelectionMode.Single;
					dataGrid.SelectionUnit=DataGridSelectionUnit.Cell;
					return;
				}
				if(value==GridSelectionMode.OneRow) {
					dataGrid.SelectionMode=DataGridSelectionMode.Single;
					dataGrid.SelectionUnit=DataGridSelectionUnit.FullRow;
					return;
				}
				if(value==GridSelectionMode.MultiExtended) {
					dataGrid.SelectionMode=DataGridSelectionMode.Extended;
					dataGrid.SelectionUnit=DataGridSelectionUnit.FullRow;
					return;
				}
				throw new Exception("SelectionMode not supported.");////todo: Add support for None
			}
		}

		///<summary>The title of the grid which shows across the top.</summary>
		[Category("OD")]
		[Description("The title of the grid which shows across the top.")]
		[DefaultValue("")]
		public string Title { 
			get => textTitle.Text; 
			set => textTitle.Text=value; 
		}
		#endregion Properties - Public

		#region Properties - Not Browsable
		//<summary>Not doing this because it wasn't present in GridOD</summary>
		//public int SelectedIndex { 
		//	get => dataGrid.SelectedIndex; 
		//	set => dataGrid.SelectedIndex=value; 
		//}

		///<summary></summary>
		[Browsable(false)]
		public List<int> SelectedIndices {
			get{
				//todo:
				//if(SelectionMode==GridSelectionMode.OneCell) {
				//	if(_selectedCell.Y==-1){
				//		return new int[0];
				//	}
				//	return new int[] { _selectedCell.Y };
				//}
				List<int> listIdxs=new List<int>();
				for(int i=0;i<dataGrid.SelectedItems.Count;i++){
					int idx=dataGrid.Items.IndexOf(dataGrid.SelectedItems[i]);
					listIdxs.Add(idx);
				}
				listIdxs.Sort();
				return listIdxs;
			}
		}
		#endregion Properties - Not Browsable

		#region Methods - public
		public void ColumnAdd(GridColumnOld gridColumn) {
			DataGridTemplateColumn dataGridTemplateColumn = new DataGridTemplateColumn();
			dataGridTemplateColumn.Header=gridColumn.Heading;
			DataGridLength dataGridLength;
			if(gridColumn.IsWidthDynamic) {
				dataGridLength=new DataGridLength(gridColumn.DynamicWeight,DataGridLengthUnitType.Star);
			}
			else {
				dataGridLength=new DataGridLength(gridColumn.ColWidth);
			}
			dataGridTemplateColumn.Width=dataGridLength;
			//Alignment---------------------------------------------------
			int idx = dataGrid.Columns.Count;
			//DataTemplate
			//Can't do this in XAML because my binding depends on the column idx, and XAML has no mechanism to let me set that.
			//XAML also wouldn't support my if statements
			DataTemplate dataTemplate = new DataTemplate();
			//Wrap all 3 elements in a grid that share the same single cell
			FrameworkElementFactory frameworkElementFactoryGrid = new FrameworkElementFactory(typeof(Grid));
			//Border1 is the lower border and the background. Every row will have these set and user might have changed them.
			FrameworkElementFactory frameworkElementFactoryBorder1 = new FrameworkElementFactory(typeof(Border));
			frameworkElementFactoryBorder1.SetValue(Border.BorderThicknessProperty, new Thickness(0,0,0,bottom:1));
			frameworkElementFactoryBorder1.SetBinding(Border.BorderBrushProperty, new Binding("ColorLborderStr"));
			frameworkElementFactoryBorder1.SetBinding(Border.BackgroundProperty, new Binding("Cells["+idx+"].ColorBackGStr"));
			frameworkElementFactoryGrid.AppendChild(frameworkElementFactoryBorder1);
			//Border1 is the right border. Always Gray220.
			FrameworkElementFactory frameworkElementFactoryBorder2 = new FrameworkElementFactory(typeof(Border));
			frameworkElementFactoryBorder2.SetValue(Border.BorderThicknessProperty, new Thickness(0,0,right:1,0));
			frameworkElementFactoryBorder2.SetValue(Border.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(220,220,220)));
			frameworkElementFactoryGrid.AppendChild(frameworkElementFactoryBorder2);
			//Finally, the textBlock
			FrameworkElementFactory frameworkElementFactoryTextBlock = new FrameworkElementFactory(typeof(TextBlock));
			frameworkElementFactoryTextBlock.SetBinding(TextBlock.TextProperty, new Binding("Cells["+idx+"].Text"));
			frameworkElementFactoryTextBlock.SetBinding(TextBlock.ForegroundProperty, new Binding("Cells["+idx+"].ColorTextStr"));
			frameworkElementFactoryTextBlock.SetBinding(TextBlock.FontWeightProperty, new Binding("Cells["+idx+"].BoldStr"));
			if(gridColumn.TextAlign==HorizontalAlignment.Left) {
				frameworkElementFactoryTextBlock.SetValue(TextBlock.TextAlignmentProperty,TextAlignment.Left);
			}
			else if(gridColumn.TextAlign==HorizontalAlignment.Center) {
				frameworkElementFactoryTextBlock.SetValue(TextBlock.TextAlignmentProperty,TextAlignment.Center);
			}
			else if(gridColumn.TextAlign==HorizontalAlignment.Right) {
				frameworkElementFactoryTextBlock.SetValue(TextBlock.TextAlignmentProperty,TextAlignment.Right);
			}
			frameworkElementFactoryGrid.AppendChild(frameworkElementFactoryTextBlock);
			dataTemplate.VisualTree = frameworkElementFactoryGrid;
			dataGridTemplateColumn.CellTemplate=dataTemplate;
			dataGrid.Columns.Add(dataGridTemplateColumn);
		}

		/*
		public void ColumnAddOld(GridColumn gridColumn) {
			DataGridTextColumn dataGridTextColumn = new DataGridTextColumn();
			dataGridTextColumn.Header=gridColumn.Heading;
			DataGridLength dataGridLength;
			if(gridColumn.IsWidthDynamic) {
				dataGridLength=new DataGridLength(gridColumn.DynamicWeight,DataGridLengthUnitType.Star);
			}
			else {
				dataGridLength=new DataGridLength(gridColumn.ColWidth);
			}
			dataGridTextColumn.Width=dataGridLength;
			int idx = dataGrid.Columns.Count;
			//binding data-------------------------------------------------
			Binding binding = new Binding("Cells["+idx+"].Text");
			dataGridTextColumn.Binding=binding;
			//Alignment---------------------------------------------------
			Style styleElement = new Style(typeof(TextBlock));
			Setter setter = new Setter();
			setter.Property=TextBlock.TextAlignmentProperty;
			if(gridColumn.TextAlign==HorizontalAlignment.Left) {
				setter.Value=TextAlignment.Left;
			}
			else if(gridColumn.TextAlign==HorizontalAlignment.Center) {
				setter.Value=TextAlignment.Center;
			}
			else if(gridColumn.TextAlign==HorizontalAlignment.Right) {
				setter.Value=TextAlignment.Right;
			}
			styleElement.Setters.Add(setter);
			//Foreground color------------------------------------------
			setter = new Setter();
			setter.Property=TextBlock.ForegroundProperty;
			setter.Value=new Binding("Cells["+idx+"].ColorTextStr");
			styleElement.Setters.Add(setter);
			dataGridTextColumn.ElementStyle=styleElement;
			//Binding background color----------------------------------
			//https://stackoverflow.com/questions/7107044/styling-datagridcell-correctly
			//ChatGPT says the precedence is in this order: DataGridCell, Column.CellStyle, RowStyle.
			//This is the middle of the three, which means that DataGridCell can override this, but not RowStyle.
			//When I set cell style here, it overrides any style in my resource dictionary, so all the triggers must be here instead of in xaml.
			//Or I can put them in my template triggers section
			
			Style styleCell = new Style(typeof(DataGridCell));
			setter = new Setter();
			setter.Property=BackgroundProperty;
			setter.Value=new Binding("Cells["+idx+"].ColorBackGStr");
			styleCell.Setters.Add(setter);
			setter = new Setter();
			setter.Property=BorderBrushProperty;//Setting border the same. I might want to keep it around.
			setter.Value=new Binding("Cells["+idx+"].ColorBackGStr");
			styleCell.Setters.Add(setter);
			//dataGridTextColumn.CellStyle=styleCell;
			
			//Triggers for IsSelected------------------------------------
			Trigger trigger=new Trigger();
			trigger.Property=DataGridCell.IsSelectedProperty;
			trigger.Value=true;
			setter=new Setter();
			setter.Property=BackgroundProperty;
			setter.Value=new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cddceb"));
			//In GridOD, selected is 205,220,235, kind of a lightish blue gray
			trigger.Setters.Add(setter);
			//Setting border the same.
			setter=new Setter();
			setter.Property=BorderBrushProperty;
			setter.Value=new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cddceb"));
			trigger.Setters.Add(setter);
			styleCell.Triggers.Add(trigger);
			//Triggers for hover-----------------------------------------
			trigger=new Trigger();
			trigger.Property=DataGridCell.IsMouseOverProperty;
			trigger.Value=true;
			setter=new Setter();
			//setter.TargetName="borderSelectedAndHover";
			setter.Property=BackgroundProperty;
			setter.Value=new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ebf3fb"));
			//This is 235, 243, 251
			//in GridOD, hover is 247,251,253 or #f7fbfd", but that's a little pale for here, so we went slightly darker.
			trigger.Setters.Add(setter);
			//border
			setter=new Setter();
			//setter.TargetName="borderSelectedAndHover";
			setter.Property=BorderBrushProperty;
			setter.Value=new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ebf3fb"));
			trigger.Setters.Add(setter);
			styleCell.Triggers.Add(trigger);
			dataGridTextColumn.CellStyle=styleCell;
			//https://social.msdn.microsoft.com/Forums/vstudio/en-US/5cddaaf0-4653-445c-a845-26ea224edb79/datagrid-setting-column-background-color-removes-the-cell-padding?forum=wpf
			dataGrid.Columns.Add(dataGridTextColumn);
		}*/

		public void ColumnsClear() {
			dataGrid.Columns.Clear();
		}

		public List<GridRowOld> GetListGridRows(){
			List<GridRowOld> listGridRows=new List<GridRowOld>();
			for(int i=0;i<dataGrid.Items.Count;i++){
				listGridRows.Add((GridRowOld)dataGrid.Items[i]);
			}
			return listGridRows;
		}

		public void SetListGridRows(List<GridRowOld> listGridRows){
			ObservableCollection<GridRowOld> observableCollectionGridRows=new ObservableCollection<GridRowOld>();
			for(int r=0;r<listGridRows.Count;r++){
				listGridRows[r].ColorLborderStr=listGridRows[r].ColorLborder.ToString();
				for(int c=0;c<listGridRows[r].Cells.Count;c++){
					if(listGridRows[r].Cells[c].Bold==null){
						if(listGridRows[r].Bold){//cell is null, so use row setting
							listGridRows[r].Cells[c].BoldStr="Bold";
							continue;
						}
						listGridRows[r].Cells[c].BoldStr="Normal";
						continue;
					}
					//cell overrride
					if(listGridRows[r].Cells[c].Bold.Value==true){
						listGridRows[r].Cells[c].BoldStr="Bold";
						continue;
					}
					listGridRows[r].Cells[c].BoldStr="Normal";
				}
				for(int c=0;c<listGridRows[r].Cells.Count;c++){
					if(listGridRows[r].Cells[c].ColorBackG!=Colors.Transparent){
						listGridRows[r].Cells[c].ColorBackGStr=listGridRows[r].Cells[c].ColorBackG.ToString();
						continue;
					}
					listGridRows[r].Cells[c].ColorBackGStr=listGridRows[r].ColorBackG.ToString();
				}
				for(int c=0;c<listGridRows[r].Cells.Count;c++){
					if(listGridRows[r].Cells[c].ColorText!=Colors.Transparent){
						listGridRows[r].Cells[c].ColorTextStr=listGridRows[r].Cells[c].ColorText.ToString();
						continue;
					}
					listGridRows[r].Cells[c].ColorTextStr=listGridRows[r].ColorText.ToString();
				}
				observableCollectionGridRows.Add(listGridRows[r]);
			}
			dataGrid.ItemsSource=observableCollectionGridRows;
		}
		#endregion Methods - public

		#region Methods - selections
		///<summary>Untested. If one row is selected, it returns the index to that row.  If more than one row are selected, it returns the first selected row.  Really only useful for SelectionMode.One.  If no rows selected, returns -1.</summary>
		public int GetSelectedIndex() {
			//todo:
			//if(SelectionMode==GridSelectionMode.OneCell) {
			//	return _selectedCell.Y;
			//}
			return dataGrid.SelectedIndex;
		}

		///<summary>Returns the tag of the selected item.  If T does not match the type of tag, it will return the default of T (usually null).</summary>
		public T GetSelectedTag<T>() {
			int idx=dataGrid.SelectedIndex;
			if(idx==-1){
				return default;
			}
			object tag=((GridRowOld)dataGrid.Items[idx]).Tag;
			if(tag is T){
				return (T)tag;
			}
			return default;
		}
		#endregion Methods - selections

		#region Methods - event handlers
		private void DataGrid_LoadingRow(object sender,DataGridRowEventArgs e) {
			//Some ways to set color for a row:
			//https://stackoverflow.com/questions/18053281/how-to-set-datagrids-row-background-based-on-a-property-value-using-data-bindi
			//or changing it here:
			//https://stackoverflow.com/questions/10056657/change-wpf-datagrid-row-color
			//((System.Data.DataRowView)(e.Row.DataContext)).Row.ItemArray[3].ToString();
			
			//e.Row.ba
			//GridRow gridRow=(GridRow)e.Row.Item;
			//dataRow.ItemArray[0]=gridRow.Cells[0].Text;
			//dataRow.ItemArray[1]=gridRow.Cells[1].Text;
			//e.Row.Background=new SolidColorBrush(gridRow.ColorBackG);
		}

		private void DataGrid_MouseDoubleClick(object sender,MouseButtonEventArgs e) {
			GridClickEventArgs dataGridClickEventArgs=GetMouseInfo(e.OriginalSource);
			if(dataGridClickEventArgs.Col==-1 || dataGridClickEventArgs.Row==-1){
				return;
			}
			CellDoubleClick?.Invoke(this,dataGridClickEventArgs);
		}
		#endregion Methods - event handlers

		#region Methods - private
		//https://blog.scottlogic.com/2008/12/02/wpf-datagrid-detecting-clicked-cell-and-row.html
		private GridClickEventArgs GetMouseInfo(object objectOriginalSource){
			DependencyObject dependencyObject=(DependencyObject)objectOriginalSource;
			while(true){
				if(dependencyObject is null){
					break;
				}
				if(dependencyObject is DataGridCell){
					break;
				}
				dependencyObject=VisualTreeHelper.GetParent(dependencyObject);
			}
			if(dependencyObject is null){
				return new GridClickEventArgs(-1,-1,MouseButton.Left);
			}
			DataGridCell dataGridCell=(DataGridCell)dependencyObject;
			int col=dataGridCell.Column.DisplayIndex;
			while(true){
				if(dependencyObject is null){
					break;
				}
				if(dependencyObject is DataGridRow){
					break;
				}
				dependencyObject=VisualTreeHelper.GetParent(dependencyObject);
			}
			if(dependencyObject is null){
				return new GridClickEventArgs(-1,-1,MouseButton.Left);
			}
			DataGridRow dataGridRow=(DataGridRow)dependencyObject;
			System.Windows.Controls.DataGrid dataGrid2=ItemsControl.ItemsControlFromItemContainer(dataGridRow) as System.Windows.Controls.DataGrid;
			int row=dataGrid2.ItemContainerGenerator.IndexFromContainer(dataGridRow);
			return new GridClickEventArgs(col,row,MouseButton.Left);
		}
		#endregion Methods - private
	}

	
}
