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

namespace OpenDental.UIManagement{
	///<summary>We keep a list of these proxies.  Each proxy has a FrameworkElementControl field. This control gets placed on the canvas or within another control.</summary>
	public class Proxy{
		#region Fields - public
		public EnumTypeControl TypeControl;
		public Contr96Info Contr96Info_;
		///<summary>A reference to the original control so that most existing code will not need to be changed. </summary>
		public System.Windows.Forms.Control ControlWinForm;
		//public float SizeFont;
		///<summary>This is how we track heirarchy of controls. If we need a list of children, we get that on the fly. This relationship is no longer present in our control copies. Null if no parents, like the form itself.</summary>
		public Proxy ProxyParent;
		///<summary>This is what actually gets placed in the window. It could be a Button, CheckBox, Canvas with DrawingVisual, etc. It's null for the Window.</summary>
		public Control Control_;
		public UIManager UIManager_;
		#endregion Fields - public

		//#region Fields - protected
		//protected List<DrawingVisual> _listDrawingVisuals=new List<DrawingVisual>();
		//These fields and their associated logic are so common that we do them here instead of doing them again and again in each proxy.
		//protected Point _pointMouseDownCanvas;
		//protected Point _pointMouseDownScreen;
		//#endregion Fields - protected

		#region Fields - private
		
		#endregion Fields - private

		#region Constructor
		
		#endregion Constructor
		
		/*
		//No, can't do these here, or controls added are not visible.
		#region Properties
		protected override int VisualChildrenCount{
			get{ 
				return _listDrawingVisuals.Count; 
			}
		}
		#endregion Properties

		#region Methods - virtual
		protected override Visual GetVisualChild(int index){
			if(index < 0 || index >= _listDrawingVisuals.Count){
				throw new ArgumentOutOfRangeException();
			}
			return _listDrawingVisuals[index];
		}
		#endregion Methods - virtual

		public virtual void UpdateState(){

		}*/

		public void SetFontSize(float sizeFont){
			Control_.FontSize=sizeFont;
		}

		/*
		#region Methods - Event Handlers
		///<summary></summary>
		public virtual void OnMouseLeftButtonDown(MouseButtonEventArgs e){
			//this is also used for click and double click
			//Double click: e.ClickCount==2
			_pointMouseDownCanvas=e.GetPosition(UIManager_.Canvas_);
			_pointMouseDownScreen=UIManager_.Canvas_.PointToScreen(_pointMouseDownCanvas);
		}

		public virtual void OnMouseMove(MouseEventArgs e){
			
		}

		public virtual void OnMouseLeave(){
			
		}

		public virtual void OnMouseUp(){
			
		}
		#endregion Methods - Event Handlers*/

		/*
		///<summary></summary>
		protected override Size MeasureOverride(Size availableSize){
			Size panelDesiredSize = new Size();
			// In our example, we just have one child.
			// Report that our panel requires just the size of its only child.
			foreach (UIElement child in InternalChildren){
				child.Measure(availableSize);
				panelDesiredSize = child.DesiredSize;
			}
			return panelDesiredSize ;
		}
		
		///<summary></summary>
		protected override Size ArrangeOverride(Size finalSize){
			for(int i=0;i<InternalChildren.Count;i++){
				double x = GetLeft(InternalChildren[i]);
				double y = GetTop(InternalChildren[i]);
				Rect rect=new Rect(new Point(x, y), InternalChildren[i].DesiredSize);
				InternalChildren[i].Arrange(rect);
			}
			return finalSize; // Returns the final Arranged size
		}*/
	}

	public enum EnumTypeControl{
		Window,
		Button,
		TextBox
	}
}
