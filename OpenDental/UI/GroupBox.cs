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

namespace OpenDental.UI{//Jordan is the only one allowed to edit this file
	
	///<summary>Use this instead of MS GroupBox.  Supports our custom drawing and scaling.  Has rounded corners and a darker border.</summary>
	[Designer(typeof(System.Windows.Forms.Design.ScrollableControlDesigner))]//gets rid of dashed border in designer
	public partial class GroupBox : System.Windows.Forms.Panel{//inheriting from panel in order to be a container
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		private bool _drawBorder=true;
		private Color _colorBackLabel=Color.Empty;
		private Color _backColor=Color.Empty;
		private string _text=null;

		public GroupBox(){
			InitializeComponent();
			DoubleBuffered=true;
			ResizeRedraw=true;
			base.TabStop=true;
		}

		[AmbientValue(typeof(Color), "Empty")]
		[Category("Appearance")]
		//[DefaultValue(typeof(Color), "Empty")]//this can't be set, or it causes the color to show bold even if it's the parent color. AmbientValueAttribute seems to mutually exclusive.
		[Description("The background color of the component.")]
		public override Color BackColor{
			get{
				//never return Empty
				if(_backColor!=Color.Empty){
					return _backColor;
				}
				if(Parent is null){//not sure this would ever be used
					return Color.White;
				}
				if(Parent.BackColor==Color.Transparent){
					//TabPage backgrounds are transparent by default when visual styles are on.
					//Visual styles are a terrible old technology that we mostly need to override.
					return Color.White;
				}
				return Parent.BackColor;
			}
			set{
				_backColor=value;
				for(int i=0;i<Controls.Count;i++){
					Controls[i].Invalidate();
				}
				Invalidate();
			}
		}

		[Category("OD")]
		[DefaultValue(typeof(Color), "Empty")]
		[Description("Used when highlighting the text background on the groupBox.")]
		public Color ColorBackLabel{
			get{
				return _colorBackLabel;
			}
			set{
				_colorBackLabel=value;
				Invalidate();
			}
		}
		
		///<summary>Used by designer to enable resetting the property to its default value.</summary>
		public override void ResetBackColor(){
			BackColor = Color.Empty;
		}

		///<summary>Indicates to designer whether the property value is different from the ambient value, in which case the designer should persist the value.</summary>
		private bool ShouldSerializeBackColor(){
			return _backColor != Color.Empty;
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

		///<summary></summary>
		[Category("OD")]
		[DefaultValue(true)]
		[Description("Always true, with no way to change it. Allows controls within the groupbox to get focus.")]
		public new bool TabStop{
			get{
				return true;
			}
			set{
				base.TabStop=true;//just ignore what they pass in.
			}
		}

		///<summary></summary>
		[Browsable(true)]
		[Category("Appearance")]
		[DefaultValue(null)]
		[Description("Text.")]
		public override string Text{ 
			get{
				return _text;
			}
			set{
				_text=value;
				Invalidate();
			}
		}

		protected override void OnPaintBackground(PaintEventArgs pevent){
			//don't.  Old tech.
		}

		protected override void OnPaint(PaintEventArgs pe){
			//base.OnPaint(pe);
			//I could not come up with a way to let programmers add their own painting in addition to this.
			//No big deal.  If you need to do painting, just add a panel.
			Graphics g=pe.Graphics;
			Color colorParent=Color.White;
			if(Parent!=null && Parent.BackColor!=Color.Transparent){
				colorParent=Parent.BackColor;
			}
			using Brush brushCorners=new SolidBrush(colorParent);
			g.FillRectangle(brushCorners,ClientRectangle);//can't draw this rect with smoothing on, or an upper left border remains
			g.SmoothingMode=SmoothingMode.HighQuality;
			float radius=LayoutManager.ScaleF(4f);
			using GraphicsPath graphicsPath=GraphicsHelper.GetRoundedPath(new RectangleF(0,0,Width-1,Height-1),radius);
			Brush brush=new SolidBrush(BackColor);
			g.FillPath(brush,graphicsPath);
			if(ColorBackLabel!=Color.Empty){
				SizeF sizeF=g.MeasureString(Text,Font);
				RectangleF rectangleF=new RectangleF(LayoutManager.Scale(5),1,sizeF.Width,sizeF.Height);
				using Brush brushBackLabel=new SolidBrush(ColorBackLabel);
				g.FillRectangle(brushBackLabel,rectangleF);
			}
			if(DrawBorder){
				g.DrawPath(Pens.Silver,graphicsPath);
			}
			g.DrawString(Text,Font,Brushes.Black,LayoutManager.Scale(5),1);//yes, this is one pixel lower than original so that it doesn't touch the top line.	
		}




	}
}
