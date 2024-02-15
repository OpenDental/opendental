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
	public class GridCellOld {
		///<summary>If null, then the row state is used for bold.  Otherwise, this overrides the row.</summary>
		public bool? Bold { get; set; } = null;
		///<summary>Do not set this manually.  Only used for binding.</summary>
		public string BoldStr { get; set; }
		///<summary>Default is Color.Transparent.  If any color is set, it will override the background color.</summary>
		public Color ColorBackG {get;set;}= Colors.Transparent;
		///<summary>Do not set this manually.  Only used for binding. This is a combination of row colors and cell overrides.  So all cells get an individual color set for binding.  Triggers are used to change color of a separate transparent overlay for selection and hover so they are unrelated.</summary>
		public string ColorBackGStr{get;set;}
		///<summary>Default is Color.Transparent.  If any color is set, it will override the row color.</summary>
		public Color ColorText {get;set;}= Colors.Transparent;
		///<summary>Do not set this manually.  Only used for binding.</summary>
		public string ColorTextStr{get;set; }
		///<summary></summary>
		public string Text {get;set; } = "";

		
		public GridCellOld(){

		}

		public GridCellOld(string text){
			Text=text;
		}
	}
}
