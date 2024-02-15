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

namespace OpenDental.UI {
	public partial class TestForWpf:Control {
		//private bool isHover;
		private Bitmap _bitmap;

		public TestForWpf() {
			InitializeComponent();
		}

		[Category("OD")]
		[DefaultValue(null)]
		public Bitmap Bitmap_ {
			get => _bitmap;
			set{
				_bitmap?.Dispose();
				if(value !=null){
					_bitmap=(Bitmap)value.Clone();
				}
				Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs pe) {
			Graphics g=pe.Graphics;
			g.Clear(ColorOD.Gray(240));
			//if(isHover){
			g.DrawRectangle(Pens.DarkGray,0,0,Width-1,Height-1);
			//}
			if(Bitmap_ !=null){
				g.DrawImage(Bitmap_,2,1,Bitmap_.Width,Bitmap_.Height);
			}
			g.DrawString(Text,Font,Brushes.Black,27,5);
		}

		//protected override void OnMouseMove(MouseEventArgs e) {
		//	base.OnMouseMove(e);
		//	isHover=true;
		//	Invalidate();
		//}

		//protected override void OnMouseLeave(EventArgs e) {
		//	base.OnMouseLeave(e);
		//	isHover=false;
		//	Invalidate();
		//}
	}
}
