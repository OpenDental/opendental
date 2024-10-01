using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfControls.UI {

	public class GridRowOld {
		///<summary>Bold all cells in the row.  Each cell also has a bold override.</summary>
		public bool Bold { get; set; } = false;
		public ListGridCellsOld Cells { get; } = new ListGridCellsOld();
		///<summary>Color of Background of row.  Default is White.  Each gridCell also has a colorBackG property that will override this if set.</summary>
		public Color ColorBackG {get;set; } = Colors.White;
		///<summary>Color of the lower border. Default is Gray220.</summary>
		public Color ColorLborder { get; set; } = Color.FromRgb(220,220,220);
		///<summary>Do not set this manually.  Only used for binding.</summary>
		public string ColorLborderStr { get; set; }
			///<summary>Color of all text in row.  Default is Black. Each gridCell also has a colorText property that will override this if set.</summary>
		public Color ColorText { get; set; } = Colors.Black;
		///<summary>Used to store any kind of object that is associated with the row.</summary>
		public object Tag {get;set;} = null;

		//public void AddCell(string text){
			//Not sure why we have this.  Cells.Add would be just as easy. Also badly named.
			//GridCell gridCell=new GridCell();
			//gridCell.Text=text;
			//Cells.Add(gridCell);
		//}
	}

	public class ListGridCellsOld:List<GridCellOld>{
				///<summary></summary>
		public void Add(string value) {
			this.Add(new GridCellOld(value));
		}
	}
}
