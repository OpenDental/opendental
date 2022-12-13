using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental.UI {
	//Instructions for use:
	//Rationale:
	//Neither the MS Splitter nor SplitContainer work well with our custom layout manager. They are especially troublesome when nested or when interacting with TabControls. They also have issue with docking vs anchoring to 4 sides. They do not support high dpi layout, which is a deal breaker. The SplitContainer also has an ugly drag animation and is also just buggy. Completely separately from our layout manager issues, the SplitContainer will frequently inaccurately serialize/deserialize in the designer, causing a mangled layout. This is unacceptable. So we have our own splitterOD that is dead simple and guaranteed to work.
	//Designer:
	//You must manually lay out a panel-SplitterOD-panel in the designer. This means adding the xpos and width of the left panel to determine the xpos of the splitter, etc. There is no designer support for dragging the splitter and having the panels follow. But designer support is decent if the splitter and the panels are anchored properly. Example: PanelLeft(docked TBLR)-SplitterOD(docked TBR)-PanelRight(docked TBR). In this example, you could resize the form in the designer and all three controls would nicely reposition, with the left panel resizing as desired.  So there is designer support, but just not for dragging the splitter. In this example, if you want to change the size of the right panel, do not do it manually. Instead, change the anchoring, resize the form slightly, then restore the anchoring. So manual layout only needs to be a one-time operation rather than a repeated chore. 
	//Steps to implement:
	//First, layout your 3 controls: panel-splitter-panel.
	//Use math to ensure they are all touching.  Here's an example of the math:
	//Panel(0,0,100,300)-Splitter(100,0,5,300)-Panel(105,0,150,300)
	//Form.ClientSize is not easily avaiable. You can find it in the Designer.cs file at the bottom,
	//  or you can just visually drag a panel until it looks like it fills the form.
	//Suggested thickness of splitter is 5 pixels, but it can be different
	//Set the Orientation to TopBottom or LeftRight
	//In designer Properties, set the two panels, either PanelTop and PanelBottom or PanelLeft and PanelRight.
	//They don't actually need to be panels. They can be any controls, but are typically panels.
	//Check your anchoring as described earlier. Test by dragging the LR corner of the form, and then undo the drag.
	//If you want to programmatically change the splitter location, use SetLoc() or SetPercent().
	//Yes, this splitter also supports single panels, not just pairs of panels.
	//If you don't want the splitter to be draggable, then you probably don't really need a splitter.  Just use two panels without any splitter between them.

	///<summary>This should be used instead of MS.Splitter or SplitContainer.</summary>
	public partial class SplitterOD:Control {
		private bool _isMouseDown;
		//private bool _isMouseOver;
		private Point _pointMouseDownScreen;
		private Point _pointControlWhenMouseDown;

		public SplitterOD() {
			InitializeComponent();
			BackColor=ColorOD.Gray(210);
			Text="";
		}

		#region Properties
		[Category("OD")]
		[Description("")]
		[DefaultValue(EnumSplitterOrientation.None)]
		public EnumSplitterOrientation Orientation{get;set;}

		[Category("OD")]
		[Description("")]
		[DefaultValue(null)]
		public Control PanelLeft{get;set;}

		[Category("OD")]
		[Description("")]
		[DefaultValue(null)]
		public Control PanelRight{get;set;}

		[Category("OD")]
		[Description("")]
		[DefaultValue(null)]
		public Control PanelTop{get;set;}

		[Category("OD")]
		[Description("")]
		[DefaultValue(null)]
		public Control PanelBottom{get;set;}

		[DefaultValue(typeof(Color),"210,210,210")]
		public override Color BackColor {
			get => base.BackColor;
			set => base.BackColor=value;
		}
		#endregion Properties

		#region Methods - Public
		///<summary>Use current dpi, not 96dpi.</summary>
		public void SetLoc(int newloc){
			if(Orientation==EnumSplitterOrientation.LeftRight){
				if(PanelLeft !=null && newloc<PanelLeft.Left){
					return;
				}
				if(PanelRight !=null && newloc>PanelRight.Right-Width){
					return;
				}
				Left=newloc;
			}
			if(Orientation==EnumSplitterOrientation.TopBottom){
				if(PanelTop!=null && newloc<PanelTop.Top){
					return;
				}
				if(PanelBottom!=null && newloc>PanelBottom.Bottom-Height){
					return;
				}
				Top=newloc;
			}
			SynchPanels();
		}

		///<summary>Any value from 0 to 100. Meaningless for single panels, so only works with pairs.</summary>
		public void SetPercent(int percent){
			if(percent<0 || percent>100){
				return;
			}
			if(Orientation==EnumSplitterOrientation.LeftRight){
				if(PanelLeft is null || PanelRight is null){
					return;
				}
				int sum=PanelLeft.Width+PanelRight.Width;
				int widthLeft=sum*percent/100;//rounds down
				Left=PanelLeft.Left+widthLeft;
			}
			if(Orientation==EnumSplitterOrientation.TopBottom){
				if(PanelTop is null || PanelBottom is null){
					return;
				}
				int sum=PanelTop.Height+PanelBottom.Height;
				int heightTop=sum*percent/100;//rounds down
				Top=PanelTop.Top+heightTop;
			}
			SynchPanels();
		}
		#endregion Methods - Public

		#region Methods - Event Handlers
		protected override void OnPaint(PaintEventArgs pe) {
			//base.OnPaint(pe);
			Graphics g=pe.Graphics;
			g.Clear(BackColor);
		}

		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			_isMouseDown=true;
			_pointMouseDownScreen=PointToScreen(e.Location);
			_pointControlWhenMouseDown=Location;
		}

		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			//_isMouseOver=false;
			Cursor=Cursors.Default;
		}

		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			//_isMouseOver=true;
			if(Orientation==EnumSplitterOrientation.LeftRight){
				Cursor=Cursors.SizeWE;
			}
			else if(Orientation==EnumSplitterOrientation.TopBottom){
				Cursor=Cursors.SizeNS;
			}
			else{
				Cursor=Cursors.Default;
			}
			if(!_isMouseDown){
				return;
			}
			Point pointNowScreen=PointToScreen(e.Location);
			if(Orientation==EnumSplitterOrientation.LeftRight){
				Location=new Point(_pointControlWhenMouseDown.X+pointNowScreen.X-_pointMouseDownScreen.X,_pointControlWhenMouseDown.Y);
			}
			if(Orientation==EnumSplitterOrientation.TopBottom){
				Location=new Point(_pointControlWhenMouseDown.X,_pointControlWhenMouseDown.Y+pointNowScreen.Y-_pointMouseDownScreen.Y);
			}
			SynchPanels();
		}

		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			_isMouseDown=false;
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		///<summary>After moving the splitter, this synchs the panels with the splitter.</summary>
		private void SynchPanels(){
			if(Orientation==EnumSplitterOrientation.LeftRight){
				if(PanelLeft!=null){
					PanelLeft.Width=Left-PanelLeft.Left;
				}
				if(PanelRight!=null){
					int rightRight=PanelRight.Right;
					PanelRight.Left=Right;
					PanelRight.Width=rightRight-Right;
				}
			}
			if(Orientation==EnumSplitterOrientation.TopBottom){
				if(PanelTop!=null){
					PanelTop.Height=Top-PanelTop.Top;
				}
				if(PanelBottom!=null){
					int botBot=PanelBottom.Bottom;
					PanelBottom.Top=Bottom;
					PanelBottom.Height=botBot-Bottom;
				}
			}
		}
		#endregion Methods - Private
	}

	public enum EnumSplitterOrientation{
		None,
		TopBottom,
		LeftRight
	}
}
