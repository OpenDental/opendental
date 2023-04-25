using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace OpenDental.UIManagement {
	///<summary>This class is for internal use by UIManagement. Calls from OD proper should use ButtonState.</summary>
	public class ButtonHelper{
		public static Button CreateButton(OpenDental.UI.Button button,UIManager uIManager){
			Button buttonNew=new Button();
			buttonNew.Content=button.Text;//todo: better content to include image
			return buttonNew;
		}
	}

	/*
	///<summary>This handles all drawing for one button. It also tracks all runtime state info so that we don't ever have to change any fields on the original control.</summary>
	public class ButtonProxy{
		#region Fields - public
		public Button FrameworkElementControl;
		#endregion Fields - public

		#region Fields - private
		//private bool _isHovering;//this is not done at base level because there's so much varation between controls.
		//private Label _label;
		//private Rectangle _rectangle;
		#endregion Fields - private
		
		#region Constructor
		public ButtonProxy(){
			Button button = new Button();
			//button.Background=Brushes.Blue;
			button.Content="my button";
			Children.Add(button);
		}
		#endregion Constructor

		/*
		#region Methods - Event Handlers
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
			//Background=Brushes.LightCyan;
			//Children.Clear();
			//Button button = new Button();
			//button.Background=Brushes.Blue;
			//button.Content="my button";
			//button.HorizontalAlignment=HorizontalAlignment.Stretch;
			//button.VerticalAlignment=VerticalAlignment.Stretch;
			//Things don't stretch well onto a canvas.  I need something else.
			//Binding binding=new Binding("");
			//binding.ElementName="";
			//BindingOperations.SetBinding(target:button,Button.WidthProperty,binding);
			//button.Width=ActualWidth;
			//Children.Add(button);
			//button.Width=100;
			//button.Height=20;
			//Canvas.SetLeft(button,0);
			//Canvas.SetTop(button,0);
			//RectBounds=new Rect(UIManager_.ScaleF(Contr96Info_.RectangleFBounds96.Left),UIManager_.ScaleF(Contr96Info_.RectangleFBounds96.Top),UIManager_.ScaleF(Contr96Info_.RectangleFBounds96.Width),UIManager_.ScaleF//(Contr96Info_.RectangleFBounds96.Height));
			//RectBoundsCanvas=new Rect(RectBounds.Left,RectBounds.Top,RectBounds.Width,RectBounds.Height);
			/*
			_listDrawingVisuals.Clear();
			DrawingVisual drawingVisual=new DrawingVisual();
			DrawingContext drawingContext = drawingVisual.RenderOpen();
			Rect rect = new Rect(0,0,Width,Height);
			drawingContext.DrawRectangle(System.Windows.Media.Brushes.LightBlue,(System.Windows.Media.Pen)null,rect);
			drawingContext.Close();//Persist
			_listDrawingVisuals.Add(drawingVisual);*/
			/*
			if(_isHovering){
				_rectangle.Fill=new SolidColorBrush(Colors.LightBlue);
			}
			else{
				_rectangle.Fill=new SolidColorBrush(Colors.White);
			}
			_rectangle.Stroke=new SolidColorBrush(Colors.Gray);
			_rectangle.Width=this.  RectBoundsCanvas.Width;
			_rectangle.Height=RectBoundsCanvas.Height;
			Canvas.SetLeft(_rectangle,RectBoundsCanvas.X);
			Canvas.SetTop(_rectangle,RectBoundsCanvas.Y);
			_label.Content=Contr96Info_.ControlCopy.Text;
			_label.FontSize=UIManager_.ScaleF(Contr96Info_.SizeFont96);
			_label.HorizontalContentAlignment=System.Windows.HorizontalAlignment.Center;
			_label.VerticalContentAlignment=System.Windows.VerticalAlignment.Center;
			_label.Width=RectBoundsCanvas.Width;
			_label.Height=RectBoundsCanvas.Height;
			Canvas.SetLeft(_label,RectBoundsCanvas.X);
			Canvas.SetTop(_label,RectBoundsCanvas.Y);
		}
		#endregion Methods - public
	}*/
}
