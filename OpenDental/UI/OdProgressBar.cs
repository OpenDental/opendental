using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental.UI {
	public class OdProgressBar:ProgressBar {
		private int _targetValue=0;
		private Color _targetColor=Color.Red;
		private Font _displayFont=DefaultFont;

		public OdProgressBar() {
			this.SetStyle(ControlStyles.UserPaint,true);
		}

		[Category("Appearance"),Description("Sets a value for the target line")]
		public int TargetValue {
			get {
				return _targetValue;
			}
			set {
				_targetValue=value;
			}
		}

		[Category("Appearance"),Description("Sets a color for the target line")]
		public Color TargetColor {
			get {
				return _targetColor;
			}
			set {
				_targetColor=value;
			}
		}

		protected override void OnPaintBackground(PaintEventArgs pevent) {
			//Limits flickering
		}

		protected override void OnPaint(PaintEventArgs e) {
			using(Image progressImage=new Bitmap(this.Width,this.Height)) {
				using(Graphics g=Graphics.FromImage(progressImage)) {
					Rectangle rect=new Rectangle(0,0,this.Width,this.Height);
					if(ProgressBarRenderer.IsSupported) {
						ProgressBarRenderer.DrawHorizontalBar(g,rect);
					}
					rect.Inflate(new Size(-1,-1));//Reduce progress bar color size
					rect.Width=(int)(rect.Width*(((double)Value-(double)Minimum)/((double)Maximum-(double)Minimum)));
					if(rect.Width==0) {
						rect.Width=1;//Can't draw a 0 width rectangle.
					}
					LinearGradientBrush brush=new LinearGradientBrush(rect,this.BackColor,this.ForeColor,LinearGradientMode.Vertical);
					g.FillRectangle(brush,1,1,rect.Width,rect.Height);
					SolidBrush brushLine=new SolidBrush(_targetColor);
					if(_targetValue>Minimum && _targetValue<=Maximum) {
						//Using a rectangle for a line because using a line was causing issues
						int targetValueLocation=(int)(this.Width*(((double)_targetValue-(double)Minimum)/((double)Maximum-(double)Minimum)));
						Rectangle targetLine=new Rectangle(targetValueLocation,0,4,this.Height);
						targetLine.Inflate(new Size(-1,-1));//Reduce target line color size
						g.FillRectangle(brushLine,targetValueLocation,1,targetLine.Width,targetLine.Height);
					}
					e.Graphics.DrawImage(progressImage,0,0);
					brushLine.Dispose();
					brush.Dispose();
				}
			}
		}
	}
}
