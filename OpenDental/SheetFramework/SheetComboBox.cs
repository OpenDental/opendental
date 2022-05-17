using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace OpenDental {
	public partial class SheetComboBox:Control {
		private bool _isHovering;
		public string SelectedOption;
		private string[] _arrayComboOptions;
		///<summary>A default option for the combo box to display.  Required to be set for screening tooth chart combo boxes.  E.g. "ling", "b".</summary>
		public string DefaultOption;
		public bool IsToothChart;
		private ContextMenu _contextMenu=new ContextMenu();

		[Category("Layout"),Description("Set true if this is a toothchart combo.")]
		public bool ToothChart {get { return IsToothChart; } set { IsToothChart=value; } }

		public string[] ComboOptions {
			get {
				return _arrayComboOptions;
			}
		}

		///<summary>Currently fills the combo box with the default options for combo boxes on screen charts.</summary>
		public SheetComboBox() : this(";None|S|PS|C|F|NFE|NN") {
		}

		public SheetComboBox(string values) {
			InitializeComponent();
			string[] arrayValues=values.Split(';');
			if(arrayValues.Length>1) {
				SelectedOption=arrayValues[0];
				_arrayComboOptions=arrayValues[1].Split('|');
			}
			else{//Incorrect format.
				//Default to empty string when 'values' is in format 'A|B|C', indicating only combobox options, 
				//rather than 'C;A|B|C' which indicates selection as well as options.
				//Upon Ok click this will correct the fieldvalue format.
				SelectedOption="";
				_arrayComboOptions=arrayValues[0].Split('|');//Will be an empty string if no '|' is present.
			}
			foreach(string option in _arrayComboOptions) {
				_contextMenu.MenuItems.Add(new MenuItem(option,menuItemContext_Click));
			}
		}

		///<summary>Formats the selected option and the list of options in a string that can be saved to the database.</summary>
		public string ToFieldValue() {
			//FieldValue will contain the selected option, followed by a semicolon, followed by a | delimited list of all options.
			return SelectedOption+";"+string.Join("|",ComboOptions);
		}

		private void menuItemContext_Click(object sender,EventArgs e) {
			if(sender.GetType()!=typeof(MenuItem)) {
				return;
			}
			SelectedOption=_arrayComboOptions[_contextMenu.MenuItems.IndexOf((MenuItem)sender)];
		}

		private void SheetComboBox_MouseDown(object sender,MouseEventArgs e) {
			_contextMenu.Show(this,new Point(0,Height));//Can't resize width, it's done according to width of items.
		}

		protected override void OnPaint(PaintEventArgs pe) {
			base.OnPaint(pe);
			Color surroundColor=Color.FromArgb(245,234,200);
			using(Brush hoverBrush = new SolidBrush(surroundColor))
			using(Pen outlinePen = new Pen(Color.Black))
			using(Pen surroundPen = new Pen(surroundColor))
			using(Font strFont = new Font(FontFamily.GenericSansSerif,IsToothChart ? 10f : this.Height-10))
			using(StringFormat strFormat = new StringFormat() { Alignment=StringAlignment.Center,LineAlignment=StringAlignment.Center })
			using(Graphics g = pe.Graphics) {
				g.SmoothingMode=SmoothingMode.HighQuality;
				g.CompositingQuality=CompositingQuality.HighQuality;
				g.FillRectangle(Brushes.White,0,0,Width,Height);//White background
				if(_isHovering) {
					g.FillRectangle(hoverBrush,0,0,Width-1,Height-1);
					g.DrawRectangle(surroundPen,0,0,Width-1,Height-1);
				}
				g.DrawRectangle(outlinePen,-1,-1,Width,Height);//Outline
				Brush brush = Brushes.Black;//Default to black.  Do not dispose brush because that will dispose of Brushes.Black... not good.
				if(ToothChart) {
					if(SelectedOption=="buc" || SelectedOption=="ling" || SelectedOption=="d" || SelectedOption=="m" || SelectedOption=="None") {
						brush=Brushes.LightGray;
					}
					if(SelectedOption=="None") {
						SelectedOption=DefaultOption;//Nothing has been selected so draw the "default" string in the combo box.  E.g. "b", "ling", etc.
					}
				}
				g.DrawString(SelectedOption,strFont,brush,new Point(this.Width/2,this.Height/2),strFormat);
			}
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(!_isHovering) {
				_isHovering=true;
				Invalidate();
			}
		}

		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			_isHovering=false;
			Invalidate();
		}

		private void SheetComboBox_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Space) {
				_contextMenu.Show(this,new Point(0,Height));
			}
			Invalidate();
		}

		private void SheetComboBox_Enter(object sender,EventArgs e) {
			_isHovering=true;
			Invalidate();
		}

		private void SheetComboBox_Leave(object sender,EventArgs e) {
			_isHovering=false;
			Invalidate();
		}
	}
}
