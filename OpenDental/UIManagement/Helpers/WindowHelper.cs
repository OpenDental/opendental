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
/*
	///<summary></summary>
	public class WindowProxy:Proxy {


		
		#region Fields - public
		//public OpenDental.FormODBase FormODBase_;
		#endregion Fields - public

		#region Constructor
		public WindowProxy(){
			
		}
		#endregion Constructor

		#region Methods - Event Handlers
		public override void OnMouseLeftButtonDown(MouseButtonEventArgs e){
			base.OnMouseLeftButtonDown(e);			
		}

		public override void OnMouseLeave(){
			base.OnMouseLeave();
		}

		public override void OnMouseMove(MouseEventArgs e){
			base.OnMouseMove(e);*/
			/*This all failed because the canvas doesn't capture the mouse.
			bool isMouseDown = System.Windows.Input.Mouse.LeftButton == MouseButtonState.Pressed;
			if(!isMouseDown){
				//todo: hover effects
				//UpdateDrawing();
				return;
			}
			Point pointMouseScreen=UIManager_.Canvas_.PointToScreen(e.GetPosition(UIManager_.Canvas_));
			System.Drawing.Point pointDelta=new System.Drawing.Point((int)(pointMouseScreen.X-_pointMouseDownScreen.X),(int)(pointMouseScreen.Y-_pointMouseDownScreen.Y));
			System.Drawing.Point pointNew=new System.Drawing.Point(_pointFormOnMouseDown.X+pointDelta.X,_pointFormOnMouseDown.Y+pointDelta.Y);
			FormODBase_.Location=pointNew;*/
		/*}

		public override void OnMouseUp(){
			base.OnMouseUp();
		}
		#endregion Methods - Event Handlers

		#region Methods - public
		public override void UpdateDrawing(){*/
			/*
			RectBoundsLocal=new Rect(0,0,FormODBase_.Width,UIManager_.GetHeightTitleBar());
			RectBoundsCanvas=new Rect(0,0,FormODBase_.Width,UIManager_.GetHeightTitleBar());
			_rectangle.Fill=new SolidColorBrush(UIManager_.ColorBorder);
			//_rectangle.Stroke=new SolidColorBrush(Colors.Red);
			_rectangle.Width=RectBoundsCanvas.Width;
			_rectangle.Height=RectBoundsCanvas.Height;
			Canvas.SetLeft(_rectangle,RectBoundsCanvas.Left);
			Canvas.SetTop(_rectangle,RectBoundsCanvas.Top);
			_labelTitle.Content=FormODBase_.Text;
			_labelTitle.Foreground=Brushes.White;
			_labelTitle.FontSize=FormODBase_.Font.Size;
			_labelTitle.HorizontalContentAlignment=System.Windows.HorizontalAlignment.Left;
			_labelTitle.VerticalContentAlignment=System.Windows.VerticalAlignment.Center;
			_labelTitle.Width=RectBoundsCanvas.Width;
			_labelTitle.Height=RectBoundsCanvas.Height;
			Canvas.SetLeft(_labelTitle,RectBoundsCanvas.Left);
			Canvas.SetTop(_labelTitle,RectBoundsCanvas.Top);
		//}
		//#endregion Methods - public
	}*/
}
