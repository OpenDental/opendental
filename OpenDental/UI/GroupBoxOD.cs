using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental.UI{
	///<summary>Use this instead of MS GroupBox.  Supports our custom drawing and scaling.  Has rounded corners, a darker border, and a ColorBack with curated colors.</summary>
	public partial class GroupBoxOD : System.Windows.Forms.GroupBox{
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		private bool _drawBorder=true;
		private bool _isLighter=true;//for comparison, Control is 240,240,240

		public GroupBoxOD(){
			InitializeComponent();
			DoubleBuffered=true;
			ResizeRedraw=true;
			BackColor=Color.FromArgb(249,249,249);
		}

		///<summary></summary>
		[Category("OD")]
		[DefaultValue(true)]
		[Description("This lighter background color makes helps make groupboxes looks visually separate. If you need to use a custom BackgroundColor, set this to false.")]
		public bool IsLighter{
			get{
				return _isLighter;
			}
			set{
				_isLighter=value;
				if(!_isLighter){
					if(BackColor==Color.FromArgb(249,249,249)){
						BackColor=Parent.BackColor;
					}
				}
				else{
					BackColor=Color.FromArgb(249,249,249);
				}
				Invalidate();
			}
		}

		///<summary>Doing it this way forces groupboxes to paint and doesn't break any existing code.  FlatStyle.System doesn't let OnPaint fire, so we need Standard.  This property automatically disappears from the designer code if VS does any form edit.</summary>
		[DefaultValue(FlatStyle.Standard)]
		[Description("Always FlatStyle.Standard")]
		public new FlatStyle FlatStyle{
			get{
				return FlatStyle.Standard;
			}
			set{
				base.FlatStyle=FlatStyle.Standard;//ignore the value passed in
			}
		}

		///<summary></summary>
		[Category("OD")]
		[DefaultValue(true)]
		[Description("Default true, but you can turn off the border to keep the grouping without any visible indicator.")]
		public bool DrawBorder{
			get{
				return _drawBorder;
			}
			set{
				_drawBorder=value;
				Invalidate();
			}
		}

		protected override void OnPaintBackground(PaintEventArgs pevent){
			
		}

		protected override void OnPaint(PaintEventArgs pe){
			//base.OnPaint(pe);
			//I could not come up with a way to let programmers add their own painting in addition to this.
			//No big deal.  If you need to do painting, just add a panel.
			Graphics g=pe.Graphics;
			using(Brush brush=new SolidBrush(Parent.BackColor)){
				g.FillRectangle(brush,ClientRectangle);//for the corners
			}
			float radius=LayoutManager.ScaleF(4f);
			GraphicsPath graphicsPath=GraphicsHelper.GetRoundedPath(new RectangleF(0,0,Width-1,Height-1),radius);
			using(Brush brush=new SolidBrush(BackColor)){
				g.FillPath(brush,graphicsPath);
			}
			if(DrawBorder){
				g.DrawPath(Pens.Silver,graphicsPath);
			}
			graphicsPath.Dispose();
			g.DrawString(Text,Font,Brushes.Black,LayoutManager.Scale(5),1);//yes, this is one pixel lower than original so that it doesn't touch the top line.	
		}




	}
}
