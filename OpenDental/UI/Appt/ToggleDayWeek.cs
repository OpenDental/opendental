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

	///<summary>A toggle for Day/Week in the Appt module. Maybe add Month later.</summary>
	public partial class ToggleDayWeek : Control {
		#region Fields - Public
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		#endregion Fields - Public

		#region Fields - Private Static
		//These are static for life of program. Not disposed.
		private static SolidBrush _brushSelectedBack=new SolidBrush(Color.FromArgb(186,199,219));
		private static SolidBrush _brushHover=new SolidBrush(ColorOD.Hover);
		#endregion Fields - Private Static

		#region Fields - Private
		private int _hoverIndex=-1;
		private bool _isMouseDown;
		private List<string> Items;
		///<summary></summary>
		private long _selectedIndex;
		#endregion Fields - Private

		#region Constructors
		public ToggleDayWeek() {
			InitializeComponent();
			DoubleBuffered=true;
			Size=new Size(80,35);//same as default
			Items=new List<string>();
			Items.Add("Day");
			Items.Add("Week");
		}
		#endregion Constructors

		#region Events - Public Raise
		///<summary></summary>
		[Category("OD")]
		[Description("Occurs when user clicks Day.")]
		public event EventHandler DayClick;

		///<summary></summary>
		[Category("OD")]
		[Description("Occurs when user clicks Week.")]
		public event EventHandler WeekClick;
		#endregion Events - Public Raise

		#region Event - Event Handlers - OnPaint
		protected override void OnPaint(PaintEventArgs e){
			Graphics g=e.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.Clear(Color.White);
			StringFormat stringFormat=new StringFormat(StringFormatFlags.NoWrap);
			stringFormat.LineAlignment=StringAlignment.Center;
			stringFormat.Alignment=StringAlignment.Near;//left
			int heightLine=Height/Items.Count;
			for(int i=0; i<Items.Count; i++) {
				bool isSelected=false;
				if(_selectedIndex==i){
					isSelected=true;
				}
				Color colorBack=Color.White;
				Color colorText=ColorOD.Gray(30);//not quite black
				if(_hoverIndex==i){
					colorBack=Color.FromArgb(248,249,255);//ColorOD.Hover=229,239,251, which is just too dark for this control
					//unfortunately, because of all the other blue, this just looks light gray.
				}
				if(isSelected){
					colorBack=Color.FromArgb(220,230,255);//light blue
					colorText=Color.FromArgb(0,20,255);//bright blue
				}
				using SolidBrush solidBrushBack=new SolidBrush(colorBack);
				using SolidBrush solidBrushText=new SolidBrush(colorText);
				using Pen penLines=new Pen(colorText);
				g.FillRectangle(solidBrushBack,new Rectangle(0,i*heightLine,Width,heightLine));
				Rectangle rectangleText=new Rectangle(Width/2,i*heightLine+1,Width/2,heightLine);
				g.DrawString(Items[i],this.Font,solidBrushText,rectangleText,stringFormat);
				if(i==0){//day
					g.DrawRectangle(penLines,LayoutManager.Scale(15),LayoutManager.Scale(5),LayoutManager.Scale(10),LayoutManager.Scale(10));
				}
				else{//week
					g.DrawRectangle(penLines,LayoutManager.Scale(8),heightLine+LayoutManager.Scale(5),LayoutManager.Scale(25),LayoutManager.Scale(10));

				}
			}
			Color colorBorder=ColorOD.Gray(190);
			using Pen penBorder=new Pen(colorBorder);
			g.DrawRectangle(penBorder,new Rectangle(0,0,Width-1,Height-1));
		}
		#endregion Event - Event Handlers - OnPaint

		#region Events - Event Handlers - Mouse
		protected override void OnMouseDown(MouseEventArgs e){
			Focus();
			_isMouseDown=true;
			int heightLine=Height/Items.Count;
			_selectedIndex=(e.Location.Y/heightLine);
			Invalidate();
			if(_selectedIndex==0){
				DayClick?.Invoke(this,new EventArgs());
			}
			else{
				WeekClick.Invoke(this,new EventArgs());
			}
			base.OnMouseDown(e);//at end so that index will be selected before mouse down fires somewhere else (e.g. FormApptEdit).  Matches MS.
			//but, sometimes, if an event from above resulted in a dialog, then there will be no mouse up event.  Handle that below.
			MouseButtons mouseButtons=Control.MouseButtons;//introducing variable for debugging because this state is not preserved at break points.
			if(mouseButtons==MouseButtons.None){
				_isMouseDown=false;
			}
		}

		protected override void OnMouseLeave(EventArgs e){
			base.OnMouseLeave(e);
			_hoverIndex=-1;
			Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e){
			base.OnMouseMove(e);
			int heightLine=Height/Items.Count;
			//subtract 3 from the location to take border offset into account
			_hoverIndex=(e.Location.Y/heightLine);
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			_isMouseDown=false;
			Invalidate();
		}
		#endregion Events - Event Handlers - Mouse

		#region Methods - Event Handlers
		
		#endregion Methods - Event Handlers

		#region Properties - override
		protected override Size DefaultSize => new Size(80,35);
		#endregion Properties - override

		#region Methods - Public
		public void SetDay(){
			_selectedIndex=0;
			Invalidate();
		}

		public void SetWeek(){
			_selectedIndex=1;
			Invalidate();
		}
		#endregion Methods - Public

	
	}


}


