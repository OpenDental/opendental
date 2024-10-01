using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using CodeBase;

namespace OpenDental.UIManagement {
	/*
	///<summary></summary>
	public class DataGridHelper{
		public static FrameworkElement CreateDataGrid(OpenDental.UI.GridOD grid){
			Wui.DataGrid dataGrid=new Wui.DataGrid();
			dataGrid.CanUserSortColumns=grid.AllowSortingByColumn;//not sure why no triangle
			switch(grid.SelectionMode){
				case UI.GridSelectionMode.None:
					throw new Exception("Not supported yet.");
				case UI.GridSelectionMode.OneCell:
					dataGrid.SelectionMode=Wui.GridSelectionMode.OneCell;
					break;
				case UI.GridSelectionMode.OneRow:
					dataGrid.SelectionMode=Wui.GridSelectionMode.OneRow;
					break;
				case UI.GridSelectionMode.MultiExtended:
					dataGrid.SelectionMode=Wui.GridSelectionMode.MultiExtended;
					break;
			}
			if(grid.ContextMenuStrip!=null){
				dataGrid.ContextMenu=MenuHelper.CreateContextMenu(grid.ContextMenuStrip);
				//Opening event is done separately in Um.ContextMenu_SetEventOpening()
			}
			dataGrid.Title=grid.Title;
			dataGrid.CellDoubleClick+=(sender,dataGridClickEventArgs)=>
				grid.PerformCellDoubleClick(sender,new UI.ODGridClickEventArgs(dataGridClickEventArgs.Col,dataGridClickEventArgs.Row,System.Windows.Forms.MouseButtons.Left));
			return dataGrid;
		}

		private static void DataGrid_ContextMenuOpening(object sender,ContextMenuEventArgs e) {
			
		}

		private static void DataGrid_CellDoubleClick(object sender,WpfControls.UI.DataGridClickEventArgs e) {
			
		}


		public static void ColumnsClear(FrameworkElement frameworkElementDataGrid){
			Wui.DataGrid dataGrid=(Wui.DataGrid)frameworkElementDataGrid;
			dataGrid.ColumnsClear();
		}

		public static UI.GridColumn ConvertGridCol(Wui.GridColumn gridColumn){
			UI.GridColumn uIGridColumn = new UI.GridColumn();
			uIGridColumn.ColWidth=gridColumn.ColWidth;
			uIGridColumn.DynamicWeight=gridColumn.DynamicWeight;
			uIGridColumn.Heading=gridColumn.Heading;
			uIGridColumn.IsWidthDynamic=gridColumn.IsWidthDynamic;
			if(gridColumn.TextAlign==HorizontalAlignment.Left){
				uIGridColumn.TextAlign=System.Windows.Forms.HorizontalAlignment.Left;
			}
			else if(gridColumn.TextAlign==HorizontalAlignment.Center){
				uIGridColumn.TextAlign=System.Windows.Forms.HorizontalAlignment.Center;
			}
			else if(gridColumn.TextAlign==HorizontalAlignment.Right){
				uIGridColumn.TextAlign=System.Windows.Forms.HorizontalAlignment.Right;
			}
			return uIGridColumn;
		}

		public static List<UI.GridRow> ConvertGridRowsFromWpf(List<Wui.GridRow> ListGridRows){
			List<UI.GridRow> listUIGridRows=new List<UI.GridRow>();
			for(int r=0;r<ListGridRows.Count;r++){
				UI.GridRow uIGridRow = new UI.GridRow();
				for(int c = 0;c<ListGridRows[r].Cells.Count;c++) {
					UI.GridCell uIGridCell = new UI.GridCell();
					if(ListGridRows[r].Cells[c].Bold is null){
						uIGridCell.Bold=OpenDentBusiness.YN.Unknown;
					}
					else if(ListGridRows[r].Cells[c].Bold.Value){
						uIGridCell.Bold=OpenDentBusiness.YN.Yes;
					}
					else{
						uIGridCell.Bold=OpenDentBusiness.YN.No;
					}
					uIGridCell.ColorBackG=System.Drawing.Color.Empty;
					if(ListGridRows[r].Cells[c].ColorBackG!=Colors.Transparent){
						uIGridCell.ColorBackG=ColorOD.FromWpf(ListGridRows[r].Cells[c].ColorBackG);
					}
					uIGridCell.ColorText=System.Drawing.Color.Empty;
					if(ListGridRows[r].Cells[c].ColorText!=Colors.Transparent){
						uIGridCell.ColorText=ColorOD.FromWpf(ListGridRows[r].Cells[c].ColorText);
					}
					uIGridCell.Text=ListGridRows[r].Cells[c].Text;
					uIGridRow.Cells.Add(uIGridCell);
				}
				uIGridRow.Bold=ListGridRows[r].Bold;
				uIGridRow.ColorBackG=ColorOD.FromWpf(ListGridRows[r].ColorBackG);
				uIGridRow.ColorLborder=ColorOD.FromWpf(ListGridRows[r].ColorLborder);
				uIGridRow.ColorText=ColorOD.FromWpf(ListGridRows[r].ColorText);
				uIGridRow.Tag=ListGridRows[r].Tag;
				listUIGridRows.Add(uIGridRow);
			}
			return listUIGridRows;
		}

		public static List<Wui.GridRow> ConvertGridRowsToWpf(List<UI.GridRow> listUIGridRows){
			List<Wui.GridRow> listGridRows=new List<Wui.GridRow>();
			for(int r=0;r<listUIGridRows.Count;r++){
				Wui.GridRow wGridRow=new Wui.GridRow();
				for(int c=0;c<listUIGridRows[r].Cells.Count;c++){
					Wui.GridCell wGridCell=new Wui.GridCell();
					if(listUIGridRows[r].Cells[c].Bold==OpenDentBusiness.YN.Unknown){
						wGridCell.Bold=null;
					}
					else if(listUIGridRows[r].Cells[c].Bold==OpenDentBusiness.YN.Yes){
						wGridCell.Bold=true;
					}
					else{
						wGridCell.Bold=false;
					}
					wGridCell.ColorBackG=Colors.Transparent;
					if(listUIGridRows[r].Cells[c].ColorBackG!=System.Drawing.Color.Empty){
						wGridCell.ColorBackG=ColorOD.ToWpf(listUIGridRows[r].Cells[c].ColorBackG);
					}
					wGridCell.ColorText=Colors.Transparent;
					if(listUIGridRows[r].Cells[c].ColorText!=System.Drawing.Color.Empty){
						wGridCell.ColorText=ColorOD.ToWpf(listUIGridRows[r].Cells[c].ColorText);
					}
					wGridCell.Text=listUIGridRows[r].Cells[c].Text;
					wGridRow.Cells.Add(wGridCell);
				}
				wGridRow.Bold=listUIGridRows[r].Bold;
				wGridRow.ColorBackG=ColorOD.ToWpf(listUIGridRows[r].ColorBackG);
				wGridRow.ColorLborder=ColorOD.ToWpf(listUIGridRows[r].ColorLborder);
				wGridRow.ColorText=ColorOD.ToWpf(listUIGridRows[r].ColorText);
				wGridRow.Tag=listUIGridRows[r].Tag;
				listGridRows.Add(wGridRow);
			}
			return listGridRows;
		}

		

	}*/
}
