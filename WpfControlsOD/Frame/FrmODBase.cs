using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
/*
Jordan is the only one allowed to edit this file.

How to launch a dialog:
FrmAccountEdit frmAccountEdit=new FrmAccountEdit();//don't worry about the "using" statement required in WinForms.
frmAccountEdit.AccountCur=new Account();
frmAccountEdit.ShowDialog();
if(frmAccountEdit.IsDialogOK){
	etc.
}

Or for a non-modal window:
FrmAccounts frmAccounts=new FrmAccounts();
frmAccountEdit.Show();
...many options later, like... 
Account accountSelected=frmAccounts.AccountSelected;
frmAccounts.Close();

Housekeeping:
If VS starts inserting leading spaces anywhere, go to Options, Text Editor, Advanced, uncheck "Use adaptive formatting".
The XAML debugging toolbar is useless to us, so go to VS, Debugging, XAML Hot Reload, uncheck "Enable in-app toolbar".

Rules for using WPF:
-No WPF Windows.  Always use a FrmODBase.
-Maintain the anchor strategy that we have always used in WinForms.  Relative positioning is not allowed.
		-Jordan creates all of our controls. Stock controls are not allowed.
		-All of our controls default to Top Left instead of stretch. This is key.
		-Drag controls around the window, and the Top and Left margins will automatically change. These are relative to the parent grid, not to siblings.
		-To anchor to the RB, click the anchor icons in the UI (They look like chain links at the edges of the container). Click U and L to unanchor. This automatically flips the anchors to R and B.
		-To anchor to both L and R, click the R anchor icon.  This changes hAlign to stretch and width to auto.  This cannot be done by using the properties panel at the right.  It must be done by clicking on the anchors in the designer.  If you accidentally tried to change it in properties panel, it will malfunction. To fix, edit the XAML to remove the width.  You cannot specify both stretch and width.
-No binding.  We manually set and get each value in the UI just like always.
-Jordan reviews all UI changes.

How to:
-The various frms are only UserControls, not actual Windows or Forms, but we supply methods and events that make them work identically.
-Go to WpfControlsOD/UI/Controls. Look at the xaml.cs code for each control for instructions on how to implement that control.
-Issues regarding the conversion from WinForms to WPF are discussed over in WpfConverter.
-Fonts throughout are now Segoe UI 11.5, which is about 8.6 points. In spite of similar size, Segoe UI does take up a bit more horizontal space.
-For pixel drawing, we are using the same coordinate system for drawing as in WinForms. They didn't change it until DirectX10.
//https://learn.microsoft.com/en-us/windows/win32/direct3d10/d3d10-graphics-programming-guide-resources-coordinates
-Instead of Windows.Forms.Timer, use System.Windows.Threading.DispatcherTimer.
//See a few hundred lines down for how to handle key events
*/
	/// <summary></summary>
	public partial class FrmODBase:System.Windows.Controls.UserControl {
		private bool _isDialogOK=false;
		private string _text="Form";
		private FormFrame _formFrame=null;

		public FrmODBase() {
			//If no fontSize is set, then Control defines it as 12 points.
			FontFamily=new FontFamily("Segoe UI");
			FontSize=11.5;
			Focusable=true;//so key down will work
			PreviewKeyDown+=Frm_PreviewKeyDown;
		}

		public event EventHandler FormClosed;
		public event System.ComponentModel.CancelEventHandler FormClosing;

		#region Properties
		///<summary>True means OK or Yes.  False means Cancel or No.  Default is false.</summary>
		public bool IsDialogOK{
			get{
				return _isDialogOK;
			}
			set{
				_isDialogOK=value;
				Close();//regardless of the value they set
			}
		}

		///<summary>Shows in titlebar of window.</summary>
		public string Text{
			get{
				return _text;
			}
			set{
				_text=value;
			}
		}
		#endregion Properties

		public bool ShowDialog(){
			if(_formFrame!=null){
				return false;//already showing
			}
			_formFrame=new FormFrame();
			_formFrame.FormClosed+=FormFrame_FormClosed;
			_formFrame.FormClosing+=FormFrame_FormClosing;
			_formFrame.InitializeFormMaker(this);
			_formFrame.ShowDialog();
			//formFrame won't have a meaningful dialogResult.
			//The programmer has already set IsDialogOK from inside the derived frm.
			return _isDialogOK;
		}

		public void Show(){
			if(_formFrame!=null){
				return;//already showing
			}
			_formFrame=new FormFrame();
			_formFrame.FormClosed+=FormFrame_FormClosed;
			_formFrame.FormClosing+=FormFrame_FormClosing;
			_formFrame.InitializeFormMaker(this);
			_formFrame.Show();
		}

		private void FormFrame_FormClosed(object sender,System.Windows.Forms.FormClosedEventArgs e) {
			//I don't care about the CloseReason, so this can be a plain EventArgs
			FormClosed?.Invoke(sender,new EventArgs());
			Dispose();
		}

		///<summary>If you have some unmanaged resources to dispose of, you can do it here.  Make sure to call base.Dispose() at the end.</summary>
		protected void Dispose(){
			_formFrame?.Dispose();
			//The above might not be needed. When we change isDialogOK, we also Close.
			//I think MS instead makes the dialog window not visible, which is why we must either dispose or close when using WinForms.
		}

		private void FormFrame_FormClosing(object sender,System.Windows.Forms.FormClosingEventArgs e) {
			System.ComponentModel.CancelEventArgs cancelEventArgs=new System.ComponentModel.CancelEventArgs();
			FormClosing?.Invoke(sender,cancelEventArgs);
			if(cancelEventArgs.Cancel){
				e.Cancel=true;
			}
		}

		///<summary>Calling Close() from within any Frm or externally will cause the parent FormFrame to close.</summary>
		public void Close() {
			if(_formFrame==null){
				return;//already closed
			}
			_formFrame.Close();
		}

		private void Frm_PreviewKeyDown(object sender,KeyEventArgs e) {
			//We use preview so that this will work even if user is inside a textbox.
			//That textbox would mark a KeyDown event as handled so we wouldn't see it.
			//Preview is a tunneling event which is raised before a bubbling event like KeyDown.
			//If you want to handle an Alt-key combo, then create another event handler like this one, but in your derived frm.
			//The code would look something like this:
			//if(e.SystemKey==Key.S && Keyboard.Modifiers==ModifierKeys.Alt) {
			//	butSave_Click(this,new EventArgs());
			//}
			//Use e.SystemKey above instead of e.Key because the presence of the Alt modifier causes it to be interpreted as a system command.
			//In addition to the code above, also include an underscore inside the button text so that the user knows about the keyboard shortcut.
			//An underscore is used in WPF instead of the & that's used in WinForms. 
			//--------------------------------------------------------------------------------------------------------------
			//Using Esc key to close the form:
			//This was previously done by setting Form.CancelButton to point to the Cancel button.
			//WPF does not have that, so instead, we make Esc be equivalent to clicking the window X in all cases.
			//You can always create an event handler for that X or formClosing if you want to warn or block them.
			if(e.Key==Key.Escape) {
				Close();
			}
			//--------------------------------------------------------------------------------------------------------------
			//Hooking the Enter key up to the OK button was previously done in WinForms by setting Form.AcceptButton.
			//We didn't use this a lot because multiline textboxes also accept enter, so it's inconsistent.
			//In the appointment edit window, we instead use C for cancel and O for ok, which is weird and non-standard.
			//But Enter can work if there are no or few multiline textboxes.
			//To implement Enter, do it in your own KeyDown (not PreviewKeyDown) event handler.  Like this:
			//In the constructor:
			//KeyDown+=Frm_KeyDown;
			//...then...
			//private void Frm_KeyDown(object sender,KeyEventArgs e) {
			//	if(e.Key==Key.Enter) {
			//		butSave_Click(this,new EventArgs());
			//	}
			//}
			//The reason the above lines should not be inside PreviewKeyDown is because then they will intercept Enter used in a multiline comboBox.
			//Instead, just use KeyDown, which is a bubbling event.
			//--------------------------------------------------------------------------------------------------------------
			//There is another detail that is sometimes confused with the above discussion of Enter/Esc for OK/Cancel.
			//In WinForms, there is a setting for button.DialogResult.
			//Our pattern was to always ignore that property and to instead explicitly set DialogResult in the OK and Cancel button event handlers.
			//WPF does not support button.DialogResult anyway, so our existing pattern will continue as it always has.
		}
	}
}

#region ignore
/*
Why we are switching to WPF:
GDI+ is just getting too old.  I keep running into unsolvable issues when drawing, and I'm just done with it.
Because of the clearly unsolvable high DPI scaling issues, I expect MS to deprecate WinForms. There's just no other way. MS doesn't have the resources to plug a million holes.
WPF uses DirectX, which means the drawing is predictable and clean, with proper layers, transparency, fonts, and antialiasing. No compromises.
In some cases, WPF might be significantly faster, like if we load images onto the GPU and then manipulate them with filters instead of depending on the CPU.
DirectX9 uses a declarative retained-mode scene. DirectX11 uses immediate procedural drawing that does not retain a scene.
WPF uses DX9, which is being deprecated. But MS has mapped it to DX11 so it's fairly future proof. The DX9 paradigm is much easier to program against, and we will take advantage of that. 
WPF controls and classes are rich and very organized. I can build custom controls so fast and with so much less code. It's incredibly fun compared to WinForms.
But there are two major downsides to WPF: 
1. Binding is not powerful enough. When deciding what value to set in a property, I need the power of C# flow, logic, loops, variables, etc. XAML doesn't cut it. We will never use binding or MVVM.
2. The WPF designer. I thought it didn't support anchoring, but it does if you use it a certain way. So this is no longer an issue.

Long Term Transition Strategy:
A. (done) Use UI Manager to swap out all controls for WPF on some simple forms. This allows validation of the new custom WPF controls. 
B. (done) Organize a way to load up and use a new form that consists solely of a WPF UserControl.
C. (done) Create a tool that converts one form at a time to a WPF UserControl. The UI Manager becomes obsolete. 
D. (In progress) Gradually convert all forms except FormOpenDental, starting with the simplest forms.
E. Move the main modules over to WPF UserControls.
F. Create a new WPF exe project and manually rebuild FormOpenDental as a WPF Window.
At the moment, we are in steps D, meaning we are designing entire windows in WPF.
Getting firmly on WPF seems to give us more options moving forward, like with WPF in dotnet 7, WinUI, Avalonia, or whatever else they come up with.


We are using the same coordinate system for drawing as in WinForms. They didn't change it until DirectX10.
//https://learn.microsoft.com/en-us/windows/win32/direct3d10/d3d10-graphics-programming-guide-resources-coordinates

How to turn it off:
There will not be a way to turn it off.  Since we are quickly moving to entire WPF windows, there's no going back. This is a one-way trip.
If something is truly broken beyond repair, then I suppose we could manually bring back the old form. But that would need to be a big bug.

There are a number of existing places where the LayoutManager is used to manually move controls around.
Those places will need to be lightly refactored to removing all scaling code. WPF automatically scales.
LayoutManager will only be deprecated after the conversion to WPF is complete.
*/
#endregion ignore
