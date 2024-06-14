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
	public class GridColumnOld {
		///<summary>Property backer.</summary>
		private float _dynamicWeight=1;
	
		#region Constructors
		///<summary>Creates a new WpfControls.UI.GridColumn.</summary>
		public GridColumnOld(){
		
		}

			///<summary>Creates a new WpfControls.UI.GridColumn with the given heading and width. Alignment left</summary>
		public GridColumnOld(string heading,int colWidth){
			Heading=heading;
			ColWidth=colWidth;
		}

		///<summary>Creates a new WpfControls.UI.GridColumn with the given heading and width.</summary>
		public GridColumnOld(string heading,int colWidth,HorizontalAlignment textAlign){
			Heading=heading;
			ColWidth=colWidth;
			TextAlign=textAlign;
		}
		#endregion Constructors

		#region Properties
		///<summary>Column width, default 80. This width is always 96 dpi and gets scaled internally to State.Width at current dpi.</summary>
		[DefaultValue(80)]
		public int ColWidth { get; set; } = 80;

		///<summary>If IsWidthDynamic and if there are multiple dynamic columns, then this can also optionally be set.  By default, the weight of a dynamic column is 1.  Setting this to 2.5, for example, will make it have a dynamic width that is 2.5x bigger than a column with a weight of 1.  Must be 1 or greater.</summary>
		[DefaultValue(1f)]
		public float DynamicWeight{
			get{
				return _dynamicWeight;
			}
			set{
				if(value<1){
					_dynamicWeight=1;
					return;
				}
				_dynamicWeight=value;
			}
		}

		///<summary>String that shows in the top heading cell.</summary>
		[DefaultValue("")]
		public string Heading { get; set; } = "";

		///<summary>Default false. Set a non-zero starting width for the column, then set this to true.  Column(s) will resize dynamically as long as not hScrollVisible.  If no columns are set to be dynamic, then the right column resizes automatically.</summary>
		[DefaultValue(false)]
		public bool IsWidthDynamic{ get; set; } = false;

		///<summary>Default Left</summary>
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment TextAlign { get; set; } = HorizontalAlignment.Left;
		#endregion Properties
	}
}
