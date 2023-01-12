using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental.InternalTools.Phones {
	public partial class MapNumber:Control {
		public Color ColorOutline=Color.Black;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();

		public MapNumber() {
			InitializeComponent();
			DoubleBuffered=true;
		}

		[Category("Appearance")]
		[Description("")]
		[DefaultValue("")]
		public override string Text{
			get{
				return base.Text;
			}
			set{
				base.Text = value;
				Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs pe) {
			//base.OnPaint(pe);
			Graphics g=pe.Graphics;
			g.TextRenderingHint=TextRenderingHint.AntiAlias;
			using Brush brushInner=new SolidBrush(BackColor);
			g.FillRectangle(brushInner,0,0,Width-1,Height-1);
			using Pen penOuter=new Pen(ColorOutline);
			g.DrawRectangle(penOuter,0,0,Width-1,Height-1);
			using Brush brushText=new SolidBrush(ForeColor);
			RectangleF rectangleFOuter=new RectangleF(1,3,Width-2,Height-2);
			StringFormat stringFormat=new StringFormat(StringFormatFlags.NoWrap);
			stringFormat.Alignment=StringAlignment.Center;
			stringFormat.LineAlignment=StringAlignment.Center;
			FitText(Text,Font,brushText,rectangleFOuter,stringFormat,g);
			stringFormat?.Dispose();
		}

		public void SetAlertColors() {
			ForeColor=Color.White;
			ColorOutline=Color.Black;
			BackColor=Color.Red;
			Invalidate();
		}

		public void SetNormalColors() {
			ForeColor=Color.Black;
			ColorOutline=Color.Black;
			BackColor=Color.White;
			Invalidate();
		}

		public void SetTriageColors(long siteNum=0) {
			ForeColor=SiteLinks.GetSiteForeColorBySiteNum(siteNum,Color.Black);
			ColorOutline=SiteLinks.GetSiteOuterColorBySiteNum(siteNum,OpenDentBusiness.Phones.PhoneColorScheme.COLOR_DUAL_OuterTriage);
			BackColor=SiteLinks.GetSiteInnerColorBySiteNum(siteNum,OpenDentBusiness.Phones.PhoneColorScheme.COLOR_DUAL_InnerTriageHere);
		}

		public void SetWarnColors() {
			ForeColor=Color.Black;
			ColorOutline=Color.Black;
			BackColor=Color.FromArgb(255,237,102);//yellow
			Invalidate();
		}

		///<summary>Replaces Graphics.DrawString. If the text is wider than will fit, then this reduces its size.  It does not consider height.</summary>
		private static void FitText(string text,Font font,Brush brush,RectangleF rectangleF,StringFormat stringFormat,Graphics g) {
			float emSize=font.Size;
			Size size=TextRenderer.MeasureText(text,font);
			if(size.Width>=rectangleF.Width) {
				emSize=emSize*(rectangleF.Width/size.Width);//get the ratio of the room width to font width and multiply that by the font size
				if(emSize<2) {//don't let the font be smaller than 2 point font
					emSize=2F;
				}
			}
			using Font fontNew=new Font(font.FontFamily,emSize,font.Style);
			g.DrawString(text,fontNew,brush,rectangleF,stringFormat);
		}
	}
}
