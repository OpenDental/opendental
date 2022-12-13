using CodeBase;
using Health.Direct.Common.Extensions;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;

namespace OpenDental.UI {
	//Jordan is the only one allowed to edit this file.
	//This checkBox is written from scratch, so we have more control. 
	//Some basic problems with the MS stock checkBox: Windows 11 introduced a stupid bold blue color, no hover effect, flatStyle confusion/errors, UseVisualStyleBackColor confusion, and poor scaling for high dpi.

	///<summary>Designed to replace the stock MS checkBox throughout OD.</summary>
	public partial class CheckBox : Control {
		#region Fields - Public
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields - Public

		#region Fields - Private
		private bool _isHovering;
		private int _sqrSize96=12;
		#endregion Fields - Private

		#region Fields - Private for Properties
		private ContentAlignment _checkAlign=ContentAlignment.MiddleLeft;
		private bool _checked;
		private CheckState _checkState;
		private bool _threeState;
		#endregion Fields - Private for Properties

		#region Constructors
		public CheckBox() {
			InitializeComponent();
			DoubleBuffered=true;
			Size=new Size(120,18);//same as default
		}
		#endregion Constructors

		#region Events - Public Raise
		///<summary>Try not to use this.  Use Click instead so that the event won't fire when the checkBox is changed programmatically.</summary>
		[Category("OD")]
		[Description("Try not to use this.  Use Click instead so that the event won't fire when the checkBox is changed programmatically.")]
		public event EventHandler CheckedChanged;//Yes, I considered 3-state. This does not fire on one of the 3-state changes, just like the MS version.
		
		///<summary>For 3-state events.</summary>
		[Category("OD")]
		[Description("For 3-state events.")]
		public event EventHandler CheckStateChanged;
		#endregion Events - Public Raise

		#region Properties - override
		protected override Size DefaultSize => new Size(120,18);
		#endregion Properties - override

		#region Properties
		///<summary>Gets or set a value indicating whether the Checked or CheckState values and the CheckBox's appearance are automatically changed when the CheckBox is clicked.</summary>
		[Category("OD")]
		[Description("Gets or set a value indicating whether the Checked or CheckState values and the CheckBox's appearance are automatically changed when the CheckBox is clicked.")]
		[DefaultValue(true)]
		public bool AutoCheck{get;set; }=true;

		///<summary>Unlike MS, text alignment also automatically follows this property. Default is MiddleLeft. Only support MiddleLeft, MiddleRight, and TopLeft.</summary>
		[Category("OD")]
		[Description("Unlike MS, text alignment also automatically follows this property. Default is MiddleLeft. Only support MiddleLeft, MiddleRight, and TopLeft.")]
		[DefaultValue(ContentAlignment.MiddleLeft)]
		public ContentAlignment CheckAlign {
			get{
				return _checkAlign;
			}
			set{
				if(!value.In(ContentAlignment.MiddleLeft,ContentAlignment.MiddleRight,ContentAlignment.TopLeft)){
					//don't allow.  I already carefully verified that no existing MS checkboxes use any other alignment.
					throw new Exception(value.ToString()+" is not a valid alignment. This checkbox only supports MiddleLeft, MiddleRight, and TopLeft.");
				}
				else{
					_checkAlign=value;
				}
				Invalidate();
			}
		}

		///<summary></summary>
		[Category("OD"),Description("Just like the MS version.")]
		[DefaultValue(false)]
		public bool Checked {
			get {
				return _checked;
			}
			set { 
				_checked=value;
				if(_checked){
					_checkState=CheckState.Checked;//using private field to preven infinite loop
				}
				else{//unchecked
					_checkState=CheckState.Unchecked;
				}
				Invalidate();
				CheckedChanged?.Invoke(this,new EventArgs());
				CheckStateChanged?.Invoke(this,new EventArgs());
			}
		}

		///<summary></summary>
		[Category("OD"),Description("Just like the MS version. 3 states.")]
		[DefaultValue(CheckState.Unchecked)]
		public CheckState CheckState {
			get {
				return _checkState;
			}
			set { 
				_checkState=value;
				if(_checkState==CheckState.Checked){
					_checked=true;//using private field to preven infinite loop
				}
				if(_checkState==CheckState.Unchecked){
					_checked=false;
				}
				if(_checkState==CheckState.Indeterminate){
					_checked=true;
				}
				Invalidate();
				CheckStateChanged?.Invoke(this,new EventArgs());
				CheckedChanged?.Invoke(this,new EventArgs());
			}
		}

		///<summary></summary>
		[Category("OD"),Description("Set true to allow 3 check states instead of two.")]
		[DefaultValue(false)]
		public bool ThreeState {
			get {
				return _threeState;
			}
			set { 
				_threeState=value;
				//MS doesn't enforce anything here, so we won't.
				//If in 3rd state, and we set to 2 state, it is allowed to stay in 3rd state until clicked.
				Invalidate();
			}
		}

		/////<summary></summary>
		//[Category("OD"), Description("Just a stub for backward compatibility. Does nothing.")]
		//[DefaultValue(System.Windows.Forms.FlatStyle.Standard)]
		//public System.Windows.Forms.FlatStyle FlatStyle { get; set; } = System.Windows.Forms.FlatStyle.Standard;

		/////<summary></summary>
		//[Category("OD"), Description("Just a stub for backward compatibility. Does nothing.")]
		//[DefaultValue(ContentAlignment.MiddleLeft)]
		//public System.Drawing.ContentAlignment TextAlign { get; set; } = ContentAlignment.MiddleLeft;

		/////<summary></summary>
		//[Category("OD"), Description("Just a stub for backward compatibility. Does nothing.")]
		//[DefaultValue(false)]
		//public bool UseVisualStyleBackColor { get; set; } = false;
		#endregion Properties

		#region Event - Event Handlers - OnPaint
		protected override void OnPaint(PaintEventArgs e){
			Graphics g=e.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.Clear(BackColor);
			SolidBrush solidBrush;
			if(_isHovering){
				solidBrush=new SolidBrush(Color.FromArgb(210,239,255));
					//ColorOD.Hover=Color.FromArgb(229,239,251);//not blue enough for this situation
			}
			else{
				solidBrush=new SolidBrush(Color.White);
			}
			//The box always scales with LM zoom instead of with the font.
			//So if someone sets a checkbox to 10 font, the box will not get bigger.
			//When LM scales this control, it will also change the font, so the font will also grow with zoom.
			int sqrSize=LayoutManager.Scale(_sqrSize96);
			//int rectTop=(int)Math.Round(Height/2f-sqrSize/2f,0);
			Rectangle rectangleBox=new Rectangle(0,0,100,100);//This defines the checkbox itself.
			Rectangle rectangleText=new Rectangle(0,0,100,100);
			StringFormat stringFormat=new StringFormat();
			if(CheckAlign==ContentAlignment.TopLeft){
				rectangleBox=new Rectangle(0,0,sqrSize,sqrSize);
				rectangleText=new Rectangle(sqrSize+3,0,Width-sqrSize-3,Height);//no extra padding needed for larger fonts
				stringFormat.Alignment=StringAlignment.Near;
				stringFormat.LineAlignment=StringAlignment.Near;
			}
			else if(CheckAlign==ContentAlignment.MiddleLeft){
				int rectTop=(int)Math.Round(Height/2f-sqrSize/2f,0);
				//in testing, the box looked very slightly high
				//rectTop-=LayoutManager.Scale(2f);
				rectangleBox=new Rectangle(0,rectTop,sqrSize,sqrSize);
				rectangleText=new Rectangle(sqrSize+3,0,Width-sqrSize-3,Height);
				stringFormat.Alignment=StringAlignment.Near;
				stringFormat.LineAlignment=StringAlignment.Center;
			}
			else if(CheckAlign==ContentAlignment.MiddleRight){
				int rectTop=(int)Math.Round(Height/2f-sqrSize/2f,0);
				//rectTop-=LayoutManager.Scale(2f);
				rectangleBox=new Rectangle(Width-sqrSize-1,rectTop,sqrSize,sqrSize);
				rectangleText=new Rectangle(0,0,Width-sqrSize-4,Height);
				stringFormat.Alignment=StringAlignment.Far;
				stringFormat.LineAlignment=StringAlignment.Center;
			}
			//TopCenter and BottomCenter are not supported
			g.FillRectangle(solidBrush,rectangleBox);
			using Pen penBox=new Pen(ColorOD.Gray(50));
			g.DrawRectangle(penBox,rectangleBox);
			//We don't need to do this because when we zoom, we also change the font size separately.
			//float fontSize=Font.Size*_zoomTest/100f;
			//using Font font=new Font("Microsoft Sans Serif",fontSize);
			if(CheckState==CheckState.Indeterminate){
				float fontSize=LayoutManager.ScaleF(8.25f);
				Font fontQuestion=new Font("Microsoft Sans Serif",fontSize);
				g.DrawString("?",fontQuestion,Brushes.Black,rectangleBox.X+LayoutManager.ScaleF(1f),rectangleBox.Y-LayoutManager.ScaleF(.5f));
			}
			else if(Checked){
				using Pen pen=new Pen(ColorOD.Gray(90),LayoutManager.ScaleF(1.6f));
				//short start of check
				g.DrawLine(pen,
					x1:rectangleBox.Left+LayoutManager.ScaleF(1.25f),
					y1:rectangleBox.Top+LayoutManager.ScaleF(5.75f),
					x2:rectangleBox.Left+LayoutManager.ScaleF(4.75f),
					y2:rectangleBox.Top+LayoutManager.ScaleF(9.25f));
				//long end of check
				//the end points don't match up because we have to hide the overlap
				g.DrawLine(pen,
					x1:rectangleBox.Left+LayoutManager.ScaleF(4f),
					y1:rectangleBox.Top+LayoutManager.ScaleF(9.25f),
					x2:rectangleBox.Left+LayoutManager.ScaleF(10.5f),
					y2:rectangleBox.Top+LayoutManager.ScaleF(2.75f));
			}
			int textTop=Height/2-Font.Height/2;
			g.DrawString(Text,Font,Brushes.Black,rectangleText,stringFormat);
		}
		#endregion Event - Event Handlers - OnPaint

		#region Events - Event Handlers - Mouse
		protected override void OnMouseDown(MouseEventArgs e){
			if(!AutoCheck){
				return;
			}
			//order is off-on-indeterm
			if(!Checked){
				Checked=true;
				return;
			}
			if(CheckState==CheckState.Indeterminate){
				Checked=false;
			}
			//Order is important here because Indeterminate is also checked
			if(Checked){
				if(ThreeState){
					CheckState=CheckState.Indeterminate;
					return;
				}
				Checked=false;
				return;
			}
		}

		protected override void OnMouseLeave(EventArgs e){
			_isHovering=false;
			Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e){
			//bool isHovering=HitTestBox(e.X);
			if(_isHovering){
				return;
			}
			_isHovering=true;
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			
		}
		#endregion Events - Event Handlers - Mouse

		#region Events
		/*This is how I think checkboxes should behave, but we will instead
		//make our behave like MS checkBox, where the whole control is clickable.
		///<summary>This is not a strict hit test. The active area is a bit bigger than the actual box.</summary>
		private bool HitTestBox(int x){
			if(CheckAlign==ContentAlignment.MiddleLeft){
				if(x<_rectangle.Right+LayoutManager.Scale(4)){
					return true;
				}
			}
			else{//middle right
				if(x>_rectangle.Left-LayoutManager.Scale(4)){
					return true;
				}
			}
			return false;
		}*/
		#endregion Events




	}



}

/*
Notes:
Here's a regular expression to find MS checkboxes with checkalignment top or bottom alignment.
We are looking for this:
checkBrokenApptCommLog.CheckAlign = System.Drawing.ContentAlignment.Top
so here we go:
check[a-zA-Z_0-9]*\.CheckAlign[a-zA-Z_0-9 =\.]*Top
Found 26 which are mostly ok. Changed 2 of them from TopRight to TopMiddle.
All remaining are TopLeft.
check[a-zA-Z_0-9]*\.CheckAlign[a-zA-Z_0-9 =\.]*Bottom
Found 1 and fixed it, so now none.

*/


