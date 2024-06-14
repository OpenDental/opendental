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
	//Jordan is the only one allowed to edit this file.

	public class GridCell {
		#region Constructors
		public GridCell(){

		}

		public GridCell(string text){
			Text=text;
		}
		#endregion Constructors

		#region Properties
		///<summary>If null, then the row state is used for bold.  Otherwise, this overrides the row.</summary>
		public bool? Bold { get; set; } = null;

		///<summary>Default is Color.Transparent.  If any color is set, it will override the background color.</summary>
		public Color ColorBackG {get;set;}= Colors.Transparent;

		///<summary>Default is Color.Transparent.  If any color is set, it will override the row color.</summary>
		public Color ColorText {get;set;}= Colors.Transparent;

		///<summary></summary>
		public int ComboSelectedIndex { get; set; } = -1;

		///<summary></summary>
		public string Text {get;set; } = "";

		///<summary>False by default. It can be set to true for each individual cell.</summary>
		public bool Underline { get; set; } = false;
		#endregion Properties
		
		
	}
}
