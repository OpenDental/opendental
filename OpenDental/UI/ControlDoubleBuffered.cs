using System.Windows.Forms;

namespace OpenDental.UI{
	///<summary>This exists because DoubleBuffered can only be set from inside a control. If we are building our own control class, we don't need this.  This is just for those cases when we are whipping up a light control that gets painted from the parent.  If you are not doing your own painting and you don't need focus, then use Panel or PanelOD instead.</summary>
	public partial class ControlDoubleBuffered : Control{
		public ControlDoubleBuffered(){
			InitializeComponent();
			//Yes, we could avoid this class by invoking these properties in reflection, but this is cleaner.
			DoubleBuffered=true;
			ResizeRedraw=true;//redraw the whole control if resized.  Avoids needing to call Invalidate from a resize event.
			SetStyle(ControlStyles.UserMouse,true);//so that this control can gain focus with click
		}

		protected override void OnPaintBackground(PaintEventArgs paintEventArgs){
			if(DesignMode){
				base.OnPaintBackground(paintEventArgs);//don't normally paint it. Reduces flicker when we do our own painting and we don't have a "background".
			}
		}

		//If you want to support Keyboard events, you should add the following to the form where you are using this control:
		/*
		private void controlDoubleBuffered_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e){
			if(e.KeyCode==Keys.Left || e.KeyCode==Keys.Right || e.KeyCode==Keys.Up || e.KeyCode==Keys.Down){
				e.IsInputKey=true;//don't jump to next control
			}
		}*/

	}
}
