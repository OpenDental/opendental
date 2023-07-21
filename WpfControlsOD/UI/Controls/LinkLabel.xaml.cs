using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	///<summary></summary>
	public partial class LinkLabel : UserControl{
		private Color _colorBack;
		private int _linkStart;
		private int _linkLength;
		private HorizontalAlignment _hAlign=HorizontalAlignment.Left;	
		private Hyperlink _hyperlink;
		private string _text;
		private VerticalAlignment _vAlign=VerticalAlignment.Top;
		
		public LinkLabel(){
			InitializeComponent();
			_hyperlink=new Hyperlink();
			_hyperlink.NavigateUri=new Uri("http://www.google.com");//dummy link, or the event won't fire.
			_hyperlink.RequestNavigate+=Hyperlink_RequestNavigate;
		}

		public event EventHandler LinkClicked;

		#region Properties
		[Category("OD")]
		public Color ColorBack {
			get {
				return _colorBack;
			}
			set {
				_colorBack = value;
				grid.Background=new SolidColorBrush(value);
			}
		}

		[Category("OD")]
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment HAlign {
			get {
				return _hAlign;
			}
			set {
				_hAlign = value;
				textBlock.HorizontalAlignment=value;
				switch(value) {
					case HorizontalAlignment.Left:
						textBlock.TextAlignment=TextAlignment.Left;
						break;
					case HorizontalAlignment.Center:
						textBlock.TextAlignment=TextAlignment.Center;
						break;
					case HorizontalAlignment.Right:
						textBlock.TextAlignment=TextAlignment.Right;
						break;
					default: //HorizontalAlignment.Stretch:
						textBlock.TextAlignment=TextAlignment.Justify;
						break;
				}
			}
		}

		public int LinkStart{
			get=>_linkStart;
			set{
				_linkStart=value;
				CalculateFlow();
			}
		}

		public int LinkLength{
			get=>_linkLength;
			set{
				_linkLength=value;
				CalculateFlow();
			}
		}

		public string Text {
			get =>_text;
			set{
				_text=value;
				CalculateFlow();
			}
		}

		[Category("OD")]
		[DefaultValue(VerticalAlignment.Top)]
		public VerticalAlignment VAlign {
			get {
				return _vAlign;
			}
			set {
				_vAlign = value;
				textBlock.VerticalAlignment=value;
			}
		}

		///<summary>This property is for convenience. It toggles the Visibility property between Visible and Collapsed.</summary>
		[Browsable(false)]
		public bool Visible{
			get{
				if(Visibility==Visibility.Visible){
					return true;
				}
				return false;//Hidden or Collapsed
			}
			set{
				if(value){
					Visibility=Visibility.Visible;
					return;
				}
				Visibility=Visibility.Collapsed;
			}
		}
		#endregion Properties

		private void CalculateFlow(){
			if(_linkStart>_text.Length){//Example 9>8
				_linkStart=_text.Length;//Set to 8
			}
			if(_linkStart+_linkLength>_text.Length){//Example 7+2>8
				_linkLength=_text.Length-_linkStart;//Set to 8-7=1
			}
			textBlock.Inlines.Clear();
			if(_linkStart>0){
				textBlock.Inlines.Add(_text.Substring(0,_linkStart));//Example 01link67 _linkStart=2 substring="01"
			}
			_hyperlink.Inlines.Clear();
			_hyperlink.Inlines.Add(_text.Substring(_linkStart,_linkLength));//Example 2,4:"link"
			textBlock.Inlines.Add(_hyperlink);
			if(_linkStart+_linkLength<_text.Length){//example 2+4<8
				textBlock.Inlines.Add(_text.Substring(_linkStart+_linkLength));//Example "67"
			}
		}

		private void Hyperlink_RequestNavigate(object sender,RequestNavigateEventArgs e) {
			LinkClicked?.Invoke(this,new EventArgs());
		}
	}
}
