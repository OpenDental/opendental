using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace OpenDental.UIManagement {
	public class TextBoxHelper{
		public static TextBox CreateTextBox(System.Windows.Forms.TextBox textBox,UIManager uIManager){
			TextBox textBoxNew=new TextBox();
			textBoxNew.Text=textBox.Text;
			textBoxNew.VerticalContentAlignment=VerticalAlignment.Center;
			return textBoxNew;
		}
	}
	/*
	///<summary>This handles all drawing for one textBox. It also tracks all runtime state info so that we don't ever have to change any fields on the original control.</summary>
	public class TextBoxProxy:Proxy{
		#region Fields - public
		//public System.Windows.Forms.TextBox TextBox_;
		#endregion Fields - public

		#region Fields - private
		private bool _isHovering;
		private Rectangle _rectangle;
		private System.Windows.Controls.Label _label;
		#endregion Fields - private

		/*#region Methods - Event Handlers
		public override void OnMouseMove(MouseEventArgs e){
			base.OnMouseMove(e);
			if(_isHovering){
				return;//already set
			}
			_isHovering=true;
			UpdateState();
		}

		public override void OnMouseLeave(){
			base.OnMouseLeave();
			_isHovering=false;
			UpdateState();
		}
		#endregion Methods - Event Handlers

		#region Methods - public
		public override void UpdateState(){
			Background=Brushes.Pink;
			/*_listDrawingVisuals.Clear();
			DrawingVisual drawingVisual=new DrawingVisual();
			DrawingContext drawingContext = drawingVisual.RenderOpen();
			Rect rect = new Rect(0,0,Width,Height);
			drawingContext.DrawRectangle(System.Windows.Media.Brushes.Pink,(System.Windows.Media.Pen)null,rect);
			drawingContext.Close();//Persist
			_listDrawingVisuals.Add(drawingVisual);*/
			/*
			RectBoundsLocal=new Rect(Contr96Info_.RectangleFBounds96.Left,Contr96Info_.RectangleFBounds96.Top,Contr96Info_.RectangleFBounds96.Width,Contr96Info_.RectangleFBounds96.Height);
			RectBoundsCanvas=new Rect(RectBoundsLocal.X,RectBoundsLocal.Y,RectBoundsLocal.Width,RectBoundsLocal.Height);
			if(_isHovering){
				_rectangle.Fill=new SolidColorBrush(Colors.LightBlue);
			}
			else{
				_rectangle.Fill=new SolidColorBrush(Colors.White);
			}
			_rectangle.Stroke=new SolidColorBrush(Colors.Gray);
			_rectangle.Width=RectBoundsCanvas.Width;
			_rectangle.Height=RectBoundsCanvas.Height;
			Canvas.SetLeft(_rectangle,RectBoundsCanvas.X);
			Canvas.SetTop(_rectangle,RectBoundsCanvas.Y);
			_label.Content=Contr96Info_.ControlCopy.Text;
			_label.FontSize=Contr96Info_.SizeFont96;
			_label.HorizontalContentAlignment=System.Windows.HorizontalAlignment.Left;
			_label.VerticalContentAlignment=System.Windows.VerticalAlignment.Center;
			_label.Width=RectBoundsCanvas.Width;
			_label.Height=RectBoundsCanvas.Height;
			Canvas.SetLeft(_label,RectBoundsCanvas.X);
			Canvas.SetTop(_label,RectBoundsCanvas.Y);
		}
		#endregion Methods - public
	}*/
}
