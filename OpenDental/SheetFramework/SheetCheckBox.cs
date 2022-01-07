using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace OpenDental {
	public partial class SheetCheckBox:Control {
		private bool isChecked;
		private Pen pen;
		private bool isHovering;
		private PathGradientBrush hoverBrush;
		private Color surroundColor;

		public bool IsChecked {
			get{ 
				return isChecked;
			}
			set{
				isChecked=value;
				Invalidate();
			}
		}

		public SheetCheckBox() {
			InitializeComponent();
			SetBrushes();
		}

		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			SetBrushes();
		}

		private void SetBrushes(){
			pen=new Pen(Color.Black,1.6f);
			hoverBrush=new PathGradientBrush(
				new Point[] {new Point(0,0),new Point(Width-1,0),new Point(Width-1,Height-1),new Point(0,Height-1)});
			hoverBrush.CenterColor=Color.White;
			surroundColor=Color.FromArgb(249,187,67);
			hoverBrush.SurroundColors=new Color[] {surroundColor,surroundColor,surroundColor,surroundColor};
			Blend blend=new Blend();
			float[] myFactors = {0f,.5f,1f,1f,1f,1f};
			float[] myPositions = {0f,.2f,.4f,.6f,.8f,1f};
			blend.Factors=myFactors;
			blend.Positions=myPositions;
			hoverBrush.Blend=blend;
		}

		protected override void OnPaint(PaintEventArgs pe) {
			base.OnPaint(pe);
			Graphics g=pe.Graphics;
			g.SmoothingMode=SmoothingMode.HighQuality;
			g.CompositingQuality=CompositingQuality.HighQuality;
			if(isHovering){
				g.FillRectangle(hoverBrush,0,0,Width-1,Height-1);
				g.DrawRectangle(new Pen(surroundColor),0,0,Width-1,Height-1);
			}
			if(isChecked){
				g.DrawLine(pen,0,0,Width-1,Height-1);
				g.DrawLine(pen,Width-1,0,0,Height-1);
			}
			g.Dispose();
		}

		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			IsChecked=!IsChecked;
			Focus();//Give this check box focus otherwise it looks strange if a different check box has focus (would leave the other always highlighted).
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(Focused) {
				return;//Check box already has focus so isHovering will be taken care of by focus events.
			}
			isHovering=true;
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			if(Focused) {
				return;//Check box already has focus so isHovering will be taken care of by focus events.
			}
			isHovering=false;
			Invalidate();
		}

		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			isHovering=true;
			Invalidate();
		}

		protected override void OnLostFocus(EventArgs e) {
			base.OnLostFocus(e);
			isHovering=false;
			Invalidate();
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.KeyCode==Keys.Space) {
				IsChecked=!IsChecked;
			}
		}

	}
}
