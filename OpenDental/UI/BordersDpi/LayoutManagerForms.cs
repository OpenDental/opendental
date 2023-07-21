using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDental.User_Controls;
using OpenDentBusiness;

namespace OpenDental{
	///<summary>This handles the bounds and fonts of all the controls on a form.  See notes below.  Gets passed to our custom controls on a form.</summary>
	public class LayoutManagerForms{
		/*
The LayoutManager will be obsolete someday. It's being superseded by the move to WPF.

There are a number of rules and restrictions to follow to make the entire system work.  These rules apply to forms derived from FormODBase, as well as our custom and user controls used on those forms.

InitializeLayoutManager() must go in the constructor of each form right after InitializeComponent(). There are comments in FormODBase.LayoutManager describing how some of this works, but you don't need to understand it.  If you have a form that does not inherit from FormODBase, make sure it's set to AutoScale=none. In these cases, you typically pass in a LayoutManager so that this non FormODBase knows how to scale.  In the constructor of every UserControl and of every form that does not inherit from FormODBase, there should be this line: Font=LayoutManagerForms.FontInitial;  Form.MinimumSize and Form.MaximumSize should always be the default 0,0.

Controls: Any programmatic resizing, moving, or adding of controls must be done using a LayoutManager.Move... or Add commands. These can be done at any time after InitializeLayoutManager. Be aware that PanelClient is present on all forms and acts as a container for the controls instead of the form itself.

DPI: Your code must work even at different dpi.  For example, a control that was 75x25 in the designer might be 113x38 on a 150% monitor.  So programmatic moving and sizing must be built for that.  When adjusting a control's size to a set amount, you will need to scale this amount using the layoutManager.  Example: LayoutManager.MoveWidth(label2,LayoutManager.Scale(75));

Fonts: Fonts are very tricky.  Our basic strategy is to manually scale all fonts by the zoom amount.  We use that size to actually draw the fonts because MS automatically scales the additional 50% in this example which is entirely out of our control.		When using LayoutManager.Add(), the font should already be scaled by the zoom amount, but not the MS amount.  Fonts are an ambient property, so they normally just use the parent control font.  But the LayoutManager stores the 96dpi version of each font and then explicitly sets each font to the zoom amount.

More on Fonts: Fonts has been by far the hardest part of scaling.  MS will not give us direct control of font size, but insists on scaling our fonts by the Windows amount.  But there are a huge number of variables that go into their decision on how much to automatically scale.  It depends on Win10 vs 11, the registry entry for fixing blurry apps, whether this is primary or secondary monitor, CreateGraphics vs e.Graphics vs TextRenderer, printer vs screen, etc.  For example, if primary is 100% and secondary is 150%, I can do some math and make it work. But if they are switched, then MS chooses a different scaling strategy and I would need to surround every font size with (more) if statements to handle that scenario.  I did go to that extreme for the window title font, but it was a lot of work. It's insane.  It's why I finally decided to move to WPF, where everything is scaled automatically.  Until that's done, I think we will have the limitation that the primary monitor should either be 100% or all monitors should be the same.  We're getting better at the scenario of all monitors the same, even with WinForms.

Long example of font math:
OD zoom=20.
MS scale=150.
Old math: scale=150+20=170
New math is either: scale=120*1.5=180 or 150*1.2=180.
So the result is slightly different than previous version, but it's better math because the order is no longer important.
We set all fonts with just the zoom component using for example ScaleFontODZoom(8.25f).
In this example the zoom amount is 1.2, so the font size is set to 9.9.
Microsoft will then automatically scale by 150% to give us 180% total, and we have no control over that.
MeasureString will also be already be scaled bigger by MS, just like DrawString.
But if you use CreateGraphics instead of the one passed in to paint, then MS will not automatically scale the font, and you must do that manually before measuring.
TextRenderer will measure a string as too large when printing, so use Graphics instead. TextRenderer was added by MS for GDI (not GDI+) measurements, so it's not accurate.
So when we are getting ready to draw, our font will usually be only scaled by the zoom amount.
Remember that measuring things other than fonts is much easier. We always use the full scale for that, 1.8 in this example.

Printing does not use scaled fonts of any kind.  There are different ways to handle this.  One way to reuse drawing code for printing is to set the scale numbers to default of 1,1, do the printing, and then set the scale numbers back to screen drawing values like 1.5,1.3.

Testing: Manual layout must be tested at a different monitor scale.  To test, you need to use a 4k monitor so that you have room to crank up the scaling percentages.  You can ask IT for one or two 4k monitors for permanent use.  Set your resolution to 4k and your MS scaling to 150%.  In Visual Studio, do not edit any designer without restarting VS at 100% scaling. Because of this issue in VS and because older versions of OD do not support higher scaling, you will probably reset your main VS monitor to HD after this testing. Or, you could do what I do and create a shortcut to start VS with no DPI awareness:
Shortcut Target="C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\devenv.exe" /noScale
For the shortcut above, there is no way to have it automatically show the Start window, so that's one more click in the toolbar.
Once OD is started up, change the Zoom to 50%.  With MS=150% and zoom=150%, you will be able to easily see any scaling issues with either number.

Form.Size: If you need to programmatically change a Form.Size, do not change it in the constructor (controls can still be changed in the constructor). Also do not change form size from outside when initializing the form.  Change it in the Load or after the load.  Unlike controls, changing form size does not require a call to LayoutManager.Move.  Just scale it.
Example: this.Width=LayoutManager.Scale(240);

Form.Location: This would usually never be done prior to changing size.  Here's how you get the allowed area, and then do basic math from there:
	Rectangle rectangleWorkingArea=System.Windows.Forms.Screen.FromHandle(this.Handle).WorkingArea;

If you're running in Debug, then ProgramEntry line 94 will be skipped so your registry won't be changed.  If fonts are too big, look at that code and make the change in your Windows settings.

Specific Controls=====================================================================================================================
All controls had to be adapted and many unique situations arose. Here are some of the most common control issues to be aware of:

Labels, radiobuttons, MS checkboxes, etc: There is a choice on these controls between FlatStyle.Standard and FlatStyle.System.  Always use Standard.  System was only added as an option for those who wished to support WinXP themes, but you lose a lot of other functionality and it can't scale at all.

Menu: Main menus should be MenuOD, not Menu or MenuStrip.  Context menus don't matter.

Splitter/SplitContainer: Do not use System.Windows.Forms.Splitter or System.Windows.Forms.SplitContainer.  Use OpenDental.UI.SplitContainer instead. You can grab a fresh copy from BasicTemplate.cs designer. 

ListBoxes: Do not use System.Windows.Forms.ListBoxes. Instead use OpenDental.UI.ListBox (fka ListBoxOD).  In the extremely rare situation where you must use a MS listbox, be aware that Microsoft has a bug/annoyance where it resets the selected indices of a listbox when we change the font.  The layout manager must change the font in order to support different dpi, so it also fixes the selected indices.  For example, this could change the selected index from 0 to -1 and back to 0 again.  If you have an event for SelectedIndexChanged, that event might be fired twice during this process. One way to solve this is to add logic to test for SelectedIndex==-1 in your event handler.

TabControl: Do not use System.Windows.Forms.TabControl. Instead, use OpenDental.UI.TabControl. You can grab a fresh copy from BasicTemplate.cs designer.  The historical problem with MS tabControls is that they do not layout tabPages that are not visible/selected. So secondary tab pages won't lay out properly, which is especially noticeable with LL anchor or dock fill. This is not a problem with the UI.TabControl.

Scrollable Control: For example, a panel that's set to AutoScroll=true.  These can cause artifacts such as additional white space.  See the method below called LayoutSkipChildren().  There's a section in there for scrollable controls.  There are usually some additional steps that need to be taken in the form itself, but now at least the LayoutManager will not be fighting you.  To keep a panel from jumping, see the small class at the bottom of FormSheetFillEdit.
		*/

		#region Fields
		///<summary>"Microsoft Sans Serif",8.25f. Every single form and UserControl must have this set, or MS will use its own default font, which is different for each computer based on Windows version and dpi. This is already present in FormODBase so that inherited forms do not also need it, so UserControls is where you have to watch out for it.</summary>
		public static Font FontInitial=new Font("Microsoft Sans Serif",8.25f);
		///<summary>Height of title bar at 96 dpi. Includes any border lines, if we decide to draw them. Icon is usually 16.</summary>
		private int _heightTitleBar96=26;
		///<summary>See FormODBase.IsLayoutMS.  This is a copy.</summary>
		public bool IsLayoutMS;
		public bool Is96dpi=false;
		///<summary>True if in the middle of laying out. Moving controls can sometimes trigger another layout event. Example, sizing a split container triggers its splitterMoved event.  This bool prevents another layout from starting in cases like that.  2022-12-26- This may be deprecated since we no longer use MS SplitContainers.</summary>
		public bool IsLayingOut;
		///<summary>This is a list of the original 96dpi layout info, as it was in the designer.</summary>
		private List<Control96Info> _listControl96Infos;
		///<summary>When maximized, this is the additional inset of panelClient on all 4 sides to compensate for the perimeter getting cut off. This doesn't get scaled.</summary>
		public int MaxInset=0;//deprecated
		///<summary>Example 1.5. This is the scale of the current screen for the form that owns this instance of LayoutManager.  It gets combined with ComputerPrefs.LocalComputer.Zoom to create ScaleMy.</summary>
		private float _scaleMS=1;
		public Size SizeClientOriginal;
		///<summary>5. L,R,B border at 96dpi</summary>
		private int _widthBorder96=5;
		///<summary>Pulled from ComputerPrefs.LocalComputer.Zoom/100. Example 1.2. Default is 1, and this is never 0.</summary>
		private float _zoomLocal=1;
		///<summary>Example 140 to test 140% zoom</summary>
		public float ZoomTest=0;
		#endregion Fields

		#region Constructor
		///<summary>This constructor just results in a default shell of a class.  ScaleMy is 1, and the scaling methods are available but always return 1.  None of the other functionality is used.</summary>
		public LayoutManagerForms(){

		}

		public LayoutManagerForms(FormODBase formODBase,bool isLayoutMS,bool is96dpi){
			IsLayoutMS=isLayoutMS;
			Is96dpi=is96dpi;
			if(IsTestingMode()){
				if(formODBase.MinimumSize != Size.Empty) {
					throw new Exception("Form.MinimumSize is not allowed.");
				}
				if(formODBase.MaximumSize != Size.Empty) {
					throw new Exception("Form.MinimumSize is not allowed.");
				}
			}
			SizeClientOriginal=formODBase.ClientSize;//permanently remember the original client size before Windows fiddles with it.
			//Remember original 96dpi bounds and font of the form and all child Controls for later dpi scaling.
			_listControl96Infos=new List<Control96Info>();
			//first, the parent itself
			//Control96Info original96=new Control96Info(formODBase);//this line causes form to undesirably resize, so we can't add the form.
			//Form size is dependent on ClientSize when initializing
			//original96.Bounds=new Rectangle(0,0,_sizeClientOriginal.Width+_widthBorder96*2,_sizeClientOriginal.Height+_heightTitleBar96+_widthBorder96);
			//_listControl96Infos.Add(original96);
			//then, the children
			List<Control> listControls=UIHelper.GetAllControls(formODBase).ToList();
			for(int i=0;i<listControls.Count;i++){
				if(listControls[i].Bounds.Height<0 || listControls[i].Bounds.Width<0){
					if(listControls[i].Parent is DomainUpDown){//the child textbox has negative height because we made it so short
						continue;
					}
				}
				Control96Info original96=new Control96Info(listControls[i]);
				_listControl96Infos.Add(original96);
				PassLayoutManager(listControls[i]);
			}
		}
		#endregion Constructor

		#region Events - Raise
		///<summary>Whether because of device dpi change or user changing the zoom level.  Frequently, SizeChanged is good enough and this isn't needed.  But sometimes, size doesn't change when zoom changes.</summary>
		public event EventHandler ZoomChanged;
		#endregion Events - Raise
		
		#region Methods - Add Move
		///<summary>Adds any Control to any parent Control.  Prior to adding a control here, set its properties.  Location and Size must first be adjusted to full scale (MS plus zoom) for current dpi.  For Location, it's usually simplest to use relative or measured numbers because they are already scaled. For absolute values that are at 96dpi, like maybe a specific Size for example, pass it through LayoutManager.Scale, .ScaleF, .ScaleSize, etc. prior to adding the Control.  Fonts are really tricky and hard to explain.  See the discussion at the top of this file and the one in ScaleMyFont().  Usually just set it to form.Font or to any other Font which is already scaled by the zoom amount.  Do not pass in fonts that are already scaled with the MS scaling.  Recursively adds info about the children to our internal tracking list.  Supports re-adding a control that was previously removed from the form programmatically as long as you didn't manually move or add any children.  When you are setting properties prior to using add, just do it like normal rather than using LayoutManager.Move.</summary>
		public void Add(Control control, Control parent,int tabIdx=-1){
			//note: we could obviously auto-sense a new control and add it automatically.
			//We don't do this because programmers will always forget to scale it first.
			//We must prompt them and track for quality.
			//Adding controls programmatically must be a very intentional and controlled action.
			if(_listControl96Infos is null){
				//This gets hit when adding ControlChart because we have not yet passed LayoutManager to chart.
				parent.Controls.Add(control);
				return;
			}
			if(IsLayoutMS){ 
				parent.Controls.Add(control);
				return;
			}
			if(parent is FormODBase formODBase){
				if(formODBase.PanelClient!=null){
					parent=formODBase.PanelClient;
				}
			}
			//We support multiple Add/Removes of the same controls.
			Control96Info control96Info=_listControl96Infos.FirstOrDefault(x=>x.ControlRef==control);
			if(control96Info==null){
				//this must be the initial add. Otherwise, this control and all its children will already still be in _listControl96Infos
				AddToList96(control,parent);//and children
			}
			else{
				//this control was found, so no need to add it.
				//We are assuming that all the children are still in the same place as when the control was removed.
				//If not, it will be very obvious when it hits our included Exception.
				//We are also assuming that no new controls were added to this.
				//Again, a new control will be obvious when it hits our exception.
			}
			//can't easily swap line below with line above, because adding a control to a form frequently changes its size and location.  Example: docking. 
			//The above line gets rid of all innate docking so that adding it to the form results in no change.
			if(parent is UI.TabControl tabControl){
				if(tabIdx>-1){
					tabControl.TabPages.Insert(tabIdx,(UI.TabPage)control);//this will also add it to the controls
				}
				tabControl.TabPages.Add((UI.TabPage)control);//this will also add it to the controls
				return;
			}
			parent.Controls.Add(control);//only this control gets added.  The children are already part of it.
		}

		public void AddAt(UI.TabPage tabPage,UI.TabControl tabControl,int idx){
			Add(tabPage,tabControl,idx);
		}

		///<summary>Extremely rare.  Used when adding a control to a form, and it contains controls of its own. This scales them all as it adds them.</summary>
		public void AddUnscaled(Control control, Control parent){
			//This makes some assumptions.
			//In our particular scenario, it's always a new control, not an existing control.
			AddToList96Unscaled(control,parent);//and children
			parent.Controls.Add(control);//only this control gets added.  The children are already part of it.
		}

		///<summary>Adds this control to _listControl96Infos. Recursively adds its children.</summary>
		private void AddToList96(Control control, Control parent){
			//We're passing in the parent because we frequently won't have actually added the control to the parent yet.
			Control96Info control96Info=new Control96Info(control);
			//the above line initialized all the fields, but many of them are wrong.
			//Since it's the responsibility of whoever's doing the Add to scale everything to current dpi,
			//then we must here unscale it to record the 96 dpi version.
			control96Info.ClientSize96Orig=new SizeF(UnscaleF(control.ClientSize.Width),UnscaleF(control.ClientSize.Height));
			if(control is System.Windows.Forms.TextBox
				|| control is System.Windows.Forms.ComboBox
				|| control is System.Windows.Forms.RichTextBox
				|| control is System.Windows.Forms.TreeView
				|| control is System.Windows.Forms.ListView
				|| control is System.Windows.Forms.ListBox)
			{
				//this unscale balances the scale that happens in the Font section of LayoutChildren.
				control96Info.FontSize96=UnscaleF(control.Font.Size);
			}
			else{
				control96Info.FontSize96=UnscaleFontODZoom(control.Font.Size);
			}
			control96Info.RectangleF96Orig=CalculateBounds96(control.Bounds,parent,control.Anchor);
			if(control.Bounds.Height<0 || control.Bounds.Width<0){
				throw new Exception();
			}
			control96Info.BoundsLast=control.Bounds;
			_listControl96Infos.Add(control96Info);
			SetNoAnchorDock(control);
			PassLayoutManager(control);
			for(int i=0;i<control.Controls.Count;i++){
				AddToList96(control.Controls[i],control);
			}
		}

		///<summary>Extremely rare. Recursive.</summary>
		private void AddToList96Unscaled(Control control, Control parent){
			Control96Info control96Info=new Control96Info(control);
			control96Info.BoundsLast=control.Bounds;
			_listControl96Infos.Add(control96Info);
			SetNoAnchorDock(control);
			PassLayoutManager(control);
			for(int i=0;i<control.Controls.Count;i++){
				AddToList96Unscaled(control.Controls[i],control);
			}
		}

		///<summary>Calculates backward to the original position it would have been at 96dpi and if parent had not resized.  Also incorporates math for any changes to control.Anchor.</summary>
		private RectangleF CalculateBounds96(Rectangle boundsScaled,Control controlParent,AnchorStyles anchor){
			//We're passing in the parent because we frequently won't have actually added the control to the parent yet.
			Control96Info control96InfoParent=_listControl96Infos.FirstOrDefault(x=>x.ControlRef==controlParent);
			SizeF sizeParentClientOriginal96;
			if(control96InfoParent==null){
				if(controlParent is FormODBase formODBase){
					//We couldn't add the form to _listControl96Infos during initialization because it would cause a resize.
					sizeParentClientOriginal96=new SizeF(SizeClientOriginal);
				}
				else{
					throw new ApplicationException("Parent control not found in _listControl96Infos.  You're probably using this method wrong.");
				}
			}
			else{
				sizeParentClientOriginal96=control96InfoParent.ClientSize96Orig;
			}
			SizeF sizeParentClientNow96=new SizeF(UnscaleF(controlParent.ClientSize.Width),UnscaleF(controlParent.ClientSize.Height));
			//but the bounds are wrong.  It must be calculated backward to the original position it would be in if the parent had not resized.
			float x=UnscaleF(boundsScaled.Left);
			float y=UnscaleF(boundsScaled.Top);
			float width=UnscaleF(boundsScaled.Width);
			float height=UnscaleF(boundsScaled.Height);
			//if(control.Dock!=DockStyle.None){
				//It would take an awful lot of math to figure out all the previous dockings.
				//But all we care about is size anyway.  Docking ignores location and one of the size dimensions.
			//}
			//else{
			if((anchor & (AnchorStyles.Left | AnchorStyles.Right)) == (AnchorStyles.Left | AnchorStyles.Right)){//left and right
				width+=sizeParentClientOriginal96.Width-sizeParentClientNow96.Width;
			}
			else if((anchor & (AnchorStyles.Left | AnchorStyles.Right)) == AnchorStyles.Right){//only right
				x+=sizeParentClientOriginal96.Width-sizeParentClientNow96.Width;
			}
			if((anchor & (AnchorStyles.Top | AnchorStyles.Bottom)) == (AnchorStyles.Top | AnchorStyles.Bottom)){//top and bottom
				height+=sizeParentClientOriginal96.Height-sizeParentClientNow96.Height;
			}
			else if((anchor & (AnchorStyles.Top | AnchorStyles.Bottom)) == AnchorStyles.Bottom){
				y+=sizeParentClientOriginal96.Height-sizeParentClientNow96.Height;
			}
			//none of the above will be hit if control.Anchor is just Top Left.
			//So, this can be run against existing controls as well as new controls
			//}
			//Return value is still unscaled.
			//I think it's ok if any of them are negative, but not certain.
			return new RectangleF(x,y,width,height);
		}

		///<summary>Moves a control location and size. Read the notes on Add() for details on how to scale bounds prior to passing them in here.  This takes effect immediately, and the LayoutManager is now informed about the change.  This also performs a recursive Layout on the children of this control so that they get sized based on the change of this parent.</summary>
		public void Move(Control control,Rectangle boundsScaled){
			if(IsLayoutMS){
				control.Bounds=boundsScaled;
				return;
			}
			if(control is FormODBase){
				throw new ApplicationException("Don't pass in the form.  Form size can simply be changed without using this.  The only restriction is that you can't change form size in constructor. Do that in Load.");
			}
			if(_listControl96Infos==null){//this is here to keep it from crashing further down.
				if(control.TopLevelControl is null){
					return;
				}
				if(control.TopLevelControl.GetType() != typeof(FormODBase)){
					//But if the control is on a windows form instead of a FormODBase, then the resize still needs to work.
					//Example: CEMT FormCentralReportSetup --> UserControlSecurityGroup --> listAssociatedUsers --> vScroll move.
					control.Bounds=boundsScaled;
				}
				return;
			}
			if(control.Parent==null){
				if(IsTestingMode()){
					//We would throw an exception, but if there is a try/catch surrounding it, the error message won't show and will be wrong.
					//MessageBox.Show("Error Message for OD Engineer:\r\n" +
					//	"A small layout bug was found.  Please describe to Jordan how to duplicate this error.");
					/* Later:
						"LayoutManagerForms.Move commands should not be called on controls until they have parents.  Prior to being added to a form, the normal control properties can be set.  LayoutManagerForms.Move is only for controls already on a form.\r\n" +
						"Click pause, then go back a few steps in Call Stack to find the Move that needs to be rewritten.");*/
					//throw new ApplicationException("Move commands should not be called on controls until they have parents.  Prior to being added to a form, the normal control properties can be set.  LayoutManager.Move is only for controls already on a form.");
				}
				return;//silently fail
			}
			if(boundsScaled.Width<0){
				boundsScaled.Width=0;
			}
			if(boundsScaled.Height<0){
				boundsScaled.Height=0;
			}
			Control96Info control96Info=_listControl96Infos.FirstOrDefault(x=>x.ControlRef==control);
			if(control96Info==null){
				//throw new ApplicationException("Control not found in _listControl96Infos.");
				return;
			}
			Control96Info control96InfoParent=_listControl96Infos.FirstOrDefault(x=>x.ControlRef==control.Parent);
			if(control96InfoParent==null){
				return;
			}
			//We can't easily compensate for anchor changes here
			//We might decide to check for anchor changes, though.
			control96Info.BoundsLast=boundsScaled;
			//Calculate how it would have been in original.  
			control96Info.RectangleF96Orig=CalculateBounds96(boundsScaled,control.Parent,control96Info.Anchor);
			control.Bounds=boundsScaled;
			LayoutControlBoundsAndFonts(control);
		}

		///<summary>Changes the height on a control. Read the notes on Add() for details on how to scale height prior to passing it in here.</summary>
		public void MoveHeight(Control control,int height){
			Move(control,new Rectangle(control.Left,control.Top,control.Width,height));
		}

		///<summary>Changes the location of a control.  Read the notes on Add() for details on how to scale location prior to passing it in here.</summary>
		public void MoveLocation(Control control,Point location){
			Move(control,new Rectangle(location,control.Size));
		}

		///<summary>Changes the size of a control.  Read the notes on Add() for details on how to scale size prior to passing it in here.</summary>
		public void MoveSize(Control control,Size size){
			Move(control,new Rectangle(control.Location,size));
		}

		///<summary>Changes the width of a control.  Read the notes on Add() for details on how to scale width prior to passing it in here.</summary>
		public void MoveWidth(Control control,int width){
			Move(control,new Rectangle(control.Left,control.Top,width,control.Height));
		}

		///<summary>Sets this control to have no anchor, docking, autosize, etc.  This prevents Windows from moving the controls and lets us take responsibility for that.</summary>
		private void SetNoAnchorDock(Control control){
			if(control is Splitter){
				throw new Exception("Splitters are not allowed. Use a UI.SplitContainer instead.");
			}
			if(control is System.Windows.Forms.SplitContainer){
				throw new Exception("MS SplitContainers are not allowed. Use a UI.SplitContainer instead.");
			}
			//In UserControlReminderAgg, there's a groupBox inside a tabcontrol.
			//Changing anchors on the groupBox triggers layout event, which calls LayoutTabs, causing crash.
			if(control.Name=="groupBoxEmailSubjAggShared"){
				return;
			}
			control.Anchor=AnchorStyles.Left | AnchorStyles.Top;
			control.Dock=DockStyle.None;
			if(control is ButtonBase buttonBase){
				buttonBase.AutoSize=false;
			}
			if(control is ContainerControl containerControl){
				containerControl.AutoScaleMode=AutoScaleMode.None; 
			}
			if(control is System.Windows.Forms.ListBox listBox){
				listBox.IntegralHeight=false;
			}
		}

		///<summary>Adds PanelClient to this form, then moves all the controls into that panel.  Sets all the controls to no longer be docked or anchored so that the custom layout code on the form is responsible for that.</summary>
		public void SetPanelClient(FormODBase formODBase){
			formODBase.PanelClient=new Panel();
			formODBase.PanelClient.Name="PanelClient";
			//if(!FormODBase.AreBordersMS){
				//This fixes the problem caused by the system border, which had slightly shrunk our clientSize, especially if launching on high dpi.
				//It will trigger a resize event, which will layout controls under MS control, restoring all positions to original.
			//	formODBase.Size=_sizeClientOriginal;//We will enlarge the form down below.
			//}
			formODBase.PanelClient.Size=SizeClientOriginal;
			//Can't simply loop through controls because the list would change as we reparent them.
			List<Control> listControls=new List<Control>();
			for(int i=0;i<formODBase.Controls.Count;i++){
				//if(odForm.Controls[i].Name==odForm.PanelClient.Name){
				//	continue;
				//}
				listControls.Add(formODBase.Controls[i]);
			}
			IButtonControl acceptButton=formODBase.AcceptButton;//it seems to forget these when the parent is changed
			IButtonControl cancelButton=formODBase.CancelButton;
			for(int i=0;i<listControls.Count;i++){//this list is just the direct children
				listControls[i].Parent=formODBase.PanelClient;
			}
			formODBase.AcceptButton=acceptButton;
			formODBase.CancelButton=cancelButton;
			//Enlarge the form
			//At this point, the scaling has not happened yet.  Guaranteed to still be 96dpi, I think.
			if(FormODBase.AreBordersMS){
				formODBase.PanelClient.Location=new Point(0,0);
				formODBase.PanelClient.Dock=DockStyle.Fill;
			}
			else{
				//These get ignored if maximized:
				//formODBase.Width=formODBase.PanelClient.Width+2*_widthBorder96;//Warning: this triggers resize event
				//formODBase.Height=formODBase.PanelClient.Height+_heightTitleBar96+_widthBorder96;
				//Application.DoEvents();
				formODBase.PanelClient.Location=new Point(_widthBorder96,_heightTitleBar96);//Dpi.Scale(this,_widthBorder96),Dpi.Scale(this,_heightTitleBar96));
			}
			formODBase.Controls.Add(formODBase.PanelClient);
			_listControl96Infos.Add(new Control96Info(formODBase.PanelClient));
			formODBase.PanelBorders=new PanelOD();
			formODBase.PanelBorders.Name="PanelBorders";
			//formODBase.PanelBorders.Bounds=new Rectangle(new Point(0,0),formODBase.Size);
			if(FormODBase.AreBordersMS){
				formODBase.PanelBorders.Visible=false;
			}
			formODBase.Controls.Add(formODBase.PanelBorders);
			if(IsLayoutMS){
				return;//we don't want to change the anchors
			}
			listControls=UIHelper.GetAllControls(formODBase).ToList();//this list is all controls, no heirarchy
			for(int i=0;i<listControls.Count;i++){
				if(FormODBase.AreBordersMS && listControls[i]==formODBase.PanelClient){
					continue;//PanelClient needs to stay anchored to all four sides
				}
				SetNoAnchorDock(listControls[i]);
			}
		}

		///<summary>Jordan needs to do these.  Rare.  Use this immediately after a control has been moved or sized by something not under our control and we need to synch that change over to the LayoutManager so that it's aware of the move.  Make a comment each time this is used so everyone knows what change we are trying to pick up. </summary>
		public void Synch(Control control){
			if(IsLayoutMS){
				return;
			}
			if(_listControl96Infos==null){
				return;
			}
			Control96Info control96Info=_listControl96Infos.FirstOrDefault(x=>x.ControlRef==control);
			if(control96Info==null){
				AddToList96(control,control.Parent);
				//throw new ApplicationException("Control not found in _listControl96Infos.");
				return;
			}
			//Compensate for changed anchors:
			control96Info.RectangleF96Orig=CalculateBounds96(control.Bounds,control.Parent,control.Anchor);
			if(control.Bounds.Height<0 || control.Bounds.Width<0){
				throw new Exception();
			}
			control96Info.BoundsLast=control.Bounds;
			//this won't handle changes to docking unless we add that feature.
			if(control.Anchor!=(AnchorStyles.Left|AnchorStyles.Top)){
				control96Info.Anchor=control.Anchor;//All anchor changes are moved over to LayoutManager.
			}
			SetNoAnchorDock(control);
			LayoutControlBoundsAndFonts(control);
		}

		/*
		///<summary>Jordan needs to do these. Rare. Recursive version of Synch, which is probably not what we want.  The layout of children are normally under control of the LayoutManager, and we don't want to run this unless we're sure we are happy with the exact current position and size of each child.  Normally, we would instead use Synch, which internally performs a layout of children.</summary>
		public void SynchRecursive(Control control){
			if(IsLayoutMS){
				return;
			}
			if(_listControl96Infos==null){
				return;
			}
			Control96Info control96Info=_listControl96Infos.FirstOrDefault(x=>x.ControlRef==control);
			if(control96Info==null){
				AddToList96(control,control.Parent);
				//throw new ApplicationException("Control not found in _listControl96Infos.");
				return;
			}
			//Compensate for changed anchors:
			control96Info.BoundsF96=CalculateBounds96(control.Bounds,control.Parent,control.Anchor);
			if(control.Bounds.Height<0 || control.Bounds.Width<0){
				throw new Exception();
			}
			control96Info.BoundsLast=control.Bounds;
			//this won't handle changes to docking unless we add that feature.
			if(control.Anchor!=(AnchorStyles.Left|AnchorStyles.Top)){
				control96Info.Anchor=control.Anchor;
			}
			SetNoAnchorDock(control);
			for(int i=0;i<control.Controls.Count;i++){
				SynchRecursive(control.Controls[i]);
			}
		}*/
		#endregion Methods - Add Move

		#region Methods - Public
		///<summary>This is scaled. 26 at 96dpi.</summary>
		public int GetHeightTitleBar(){
			return Scale(_heightTitleBar96);
		}

		///<summary>Example 1.8. This is the scale of this form and all its controls, compared to 96dpi as 100%.  It's a combination of _scaleMS and ComputerPrefs.LocalComputer.Zoom.</summary>
		public float ScaleMy(){
			if(Is96dpi){
				return 1;
			}
			if(ZoomTest!=0){
				return _scaleMS+ZoomTest/100f;
			}
			if(!Db.HasDatabaseConnection()){
				//Cannot continue because ComputerPrefs.LocalComputer will be null and then it will get set to default instead of getting the computer prefs from the database.
				//The symptom of this would be evident as the task dock y position would not be pulled from the database, so it would start in the wrong position.
				return _scaleMS;
			}
			if(ComputerPrefs.IsLocalComputerNull()){
				//This happens repeatedly during a conversion, as the progress bar redraws.
				//If we were to continue, the computerpref table could have a different number of columns, so the query below would fail.
				//This would drastically slow down the conversion.
				//ComputerPrefs.LocalComputer will get set right after the conversion is done.
				return _scaleMS;
			}
			float zoomLocal=_zoomLocal;
			try{
				zoomLocal=ComputerPrefs.LocalComputer.Zoom/100f;
			}
			catch{
				//this fails during version update, for example
			}
			if(zoomLocal==0){
				zoomLocal=1;
			}
			if(zoomLocal!=_zoomLocal){
				_zoomLocal=zoomLocal;
				ZoomChanged?.Invoke(this,new EventArgs());
			}
			float retVal=_scaleMS*zoomLocal;//1.5*1.2=1.8
			return retVal;
		}

		///<summary>Example 1.2. This is the scale for Fonts, which is only ComputerPrefs.LocalComputer.Zoom/100.</summary>
		public float ScaleMyFont(){
			if(Is96dpi){
				return 1;
			}
			if(ZoomTest!=0){
				return (ZoomTest/100f*_scaleMS);//120/100*1.5=1.8
			}
			if(!Db.HasDatabaseConnection()){
				//Cannot continue because ComputerPrefs.LocalComputer will be null and then it will get set to default instead of getting the computer prefs from the database.
				//The symptom of this would be evident as the task dock y position would not be pulled from the database, so it would start in the wrong position.
				return 1;
			}
			if(ComputerPrefs.IsLocalComputerNull()){
				//This happens repeatedly during a conversion, as the progress bar redraws.
				//If we were to continue, the computerpref table could have a different number of columns, so the query below would fail.
				//This would drastically slow down the conversion.
				//ComputerPrefs.LocalComputer will get set right after the conversion is done.
				return 1;
			}
			float zoomLocal=_zoomLocal;//example 1.2
			try{
				zoomLocal=ComputerPrefs.LocalComputer.Zoom/100f;
			}
			catch{
				//this fails during version update, for example
			}
			if(zoomLocal==0){
				zoomLocal=1;
			}
			if(zoomLocal!=_zoomLocal){
				_zoomLocal=zoomLocal;
				ZoomChanged?.Invoke(this,new EventArgs());
			}
			return zoomLocal;
		}

		///<summary>Example 1.5</summary>
		public float GetScaleMS(){
			return _scaleMS;
		}
		
		///<summary>Example 1.2</summary>
		public float GetZoomLocal(){
			return _zoomLocal;
		}

		///<summary>Example 1.5. Gets combined with zoom to create ScaleMy.</summary>
		public void SetScaleMS(float scaleMS){
			_scaleMS=scaleMS;
			ZoomChanged?.Invoke(this,new EventArgs());
		}

		///<summary>5, scaled.</summary>
		public int WidthBorder(){
			return Scale(_widthBorder96);
		}
		#endregion Methods - Public

		#region Methods - Public Scaling
		///<summary></summary>
		public int Round(float val){
			return (int)Math.Round(val);
		}

		///<summary>Converts a float or int from 96dpi to current scale.  Rounds to nearest int.</summary>
		public int Scale(float val96){
			return Round(val96*ScaleMy());
		}

		///<summary>Converts a float or int from 96dpi to current scale.</summary>
		public float ScaleF(float val96){
			float retVal=val96*ScaleMy();
			return retVal;
		}

		///<summary>Converts a float or int from 96dpi to current scale, but only (most of) the OD zoom, not the MS scale.</summary>
		public float ScaleFontODZoom(float val96){
			float scaleMyFont=ScaleMyFont();
			float retVal=val96*scaleMyFont;
			return retVal;
		}

		///<summary>Converts a font to current scale, but only (most of) the OD zoom, not the MS scale.</summary>
		public Font ScaleFontODZoom(Font font){
			Font fontRet=new Font(font.Name, font.Size*ScaleMyFont(),font.Style);
			return fontRet;
		}

		///<summary>Scales a number only by the MS scale component.  This is used after we measure a font that was only scaled by the ODZoom component.</summary>
		public float ScaleMS(float val96){
			float retVal=val96*_scaleMS;
			return retVal;
		}

		///<summary>Converts a point from 96dpi to current scale.</summary>
		public Point ScalePoint(Point point96){
			return new Point((int)Math.Round(point96.X*ScaleMy()),(int)Math.Round(point96.Y*ScaleMy()));
		}
		///<summary>Converts a size from 96dpi to current scale.</summary>
		public Size ScaleSize(Size size96){
			return new Size((int)Math.Round(size96.Width*ScaleMy()),(int)Math.Round(size96.Height*ScaleMy()));
		}

		///<summary>Converts a float or int from current screen dpi to 96dpi.  Not used very often because floats are usually better.</summary>
		public int Unscale(float valScreen){
			return (int)Math.Round(valScreen/ScaleMy());
		}		

		///<summary>Converts a float or int from current screen dpi to 96dpi.</summary>
		public float UnscaleF(float valScreen){
			return valScreen/ScaleMy();
		}		

		///<summary>Converts a float or int from zoom dpi to 96dpi.</summary>
		public float UnscaleFontODZoom(float valScreen){
			return valScreen/ScaleMyFont();
		}		

		///<summary>Example 10/1.5=6.7, which is smaller</summary>
		public float UnscaleMS(float valInput){
			return valInput/GetScaleMS();
		}
		#endregion Methods - Public Scaling

		#region Methods - Layout
		public static bool IsTestingMode(){
			if(!ODBuild.IsDebug()){
				return false;//any layout issue for a customer will tend to just misdraw slightly instead of crashing
			}
			//But for all of our engineers, it will hard crash.  Jordan will usually be involved.
			//if(Environment.MachineName.ToLower()=="jordanhome" || Environment.MachineName.ToLower()=="jordancryo" || Environment.MachineName.ToLower()=="jordanxps15"){
			//	return true;
			//}
			//if(Environment.MachineName.ToLower()=="adrianv"){
			//	return true;
			//}
			return true;
		}

		///<summary>Sets bounds and font of all child controls.  Does not alter bounds of this form itself. This must get called whether IsLayoutMS or not because it also needs to handle FormODBase.AreBordersMS or not.</summary>
		public void LayoutFormBoundsAndFonts(FormODBase formODBase){
			if(formODBase.PanelBorders==null){
				return;
			}
			if(formODBase.PanelClient==null){
				return;
			}
			if(formODBase.IsDisposed){
				return;
			}
			if(IsLayingOut){
				return;
			}
			IsLayingOut=true;
			if(!IsLayoutMS){
				//formODBase.Font=ScaleFontODZoom(formODBase.Font);//infinite loop
				//Font was set in constructor to 8.25
				//We actually always ignore form.Font, so this doesn't matter.
			}
			int hTitle=GetHeightTitleBar();
			//See notes over in FormODBase.Load
			int widthBorder=(int)Math.Round(8+(5*(GetScaleMS()-1)),MidpointRounding.AwayFromZero);
			if(FormODBase.AreBordersMS){
				formODBase.PanelClient.Bounds=formODBase.ClientRectangle;
			}
			else if(formODBase.WindowState==FormWindowState.Maximized){
				formODBase.PanelClient.Bounds=new Rectangle(
					x: 0,
					y: hTitle+widthBorder,
					width: formODBase.Width-widthBorder*2,
					height: formODBase.Height-hTitle-widthBorder*2);
				formODBase.PanelBorders.Bounds=new Rectangle(-1,widthBorder-1,formODBase.Width-widthBorder*2+1,formODBase.Height-widthBorder);
			}
			else{
				formODBase.PanelClient.Bounds=new Rectangle(
					x: 0,
					y: hTitle,
					width: formODBase.Width-widthBorder*2,
					height: formODBase.Height-hTitle-widthBorder);
				formODBase.PanelBorders.Bounds=new Rectangle(-1,-1,formODBase.Width-widthBorder*2+1,formODBase.Height-widthBorder);
			}
			/*
			else if(formODBase.WindowState==FormWindowState.Maximized){
				formODBase.PanelClient.Bounds=new Rectangle(MaxInset+Scale(_widthBorder96),MaxInset+Scale(_heightTitleBar96),
					formODBase.Width-MaxInset*2-Scale(_widthBorder96*2), formODBase.Height-MaxInset*2-Scale(_heightTitleBar96+_widthBorder96));
			}
			else{
				formODBase.PanelClient.Bounds=new Rectangle(Scale(_widthBorder96),Scale(_heightTitleBar96),
					formODBase.Width-Scale(_widthBorder96*2),formODBase.Height-Scale(_heightTitleBar96+_widthBorder96));
			}*/
			LayoutChildren(formODBase.PanelClient);
			IsLayingOut=false;
		}

		///<summary>Sets bounds and font of all child controls.  Does not alter bounds of this control itself.  The control passed in will already be at the correct size. Intentionally separate from MS layout, which should have no effect on the controls because we removed all their anchors.</summary>
		public void LayoutControlBoundsAndFonts(Control control){
			if(IsLayoutMS){
				return;
			}
			if(IsLayingOut){
				return;
			}
			IsLayingOut=true;
			LayoutChildren(control);
			IsLayingOut=false;
		}

		///<summary>Recursive. Only changes children, not the control itself.  The control passed in will already be at the new size. </summary>
		private void LayoutChildren(Control control){
			if(IsLayoutMS){
				return;
			}
			if(_listControl96Infos==null){
				return;
			}
			if(control.Controls.Count==0){
				return;
			}
			Control96Info control96InfoParent=_listControl96Infos.FirstOrDefault(x=>x.ControlRef==control);
			if(LayoutSkipChildren(control96InfoParent,control)){
				return;
			}
			if(control96InfoParent==null){
				return;
			}
			string name=control.Name;
			//for some operations, we will need both the original and current size of the parent, in 96dpi
			SizeF sizeParentClientNow96=new SizeF(UnscaleF(control.ClientSize.Width),UnscaleF(control.ClientSize.Height));
			if(control is System.Windows.Forms.TabPage tabPage){
				//TabControl only resizes tabPages that are visible/selected.  
				//We aren't allowed to resize tabPage ourselves, but we know what the size should be.
				//So, we must handle tabPage size specially. No support here for autoscroll, with bigger clientSize.
				Size sizeTabPage =((System.Windows.Forms.TabControl)tabPage.Parent).DisplayRectangle.Size;
				sizeParentClientNow96=new SizeF(UnscaleF(sizeTabPage.Width),UnscaleF(sizeTabPage.Height));
			}
			//control.SuspendLayout();//no, this would prevent things from resizing
			List<Control> listControlsDocked=new List<Control>();
			for(int i=0;i<control.Controls.Count;i++){
				List<Control96Info> listControl96Infos=_listControl96Infos.FindAll(x=>x.ControlRef==control.Controls[i]);
				if(listControl96Infos.Count>1){
					throw new ApplicationException("Duplicate");
				}
				Control96Info control96Info;
				if(listControl96Infos.Count==0){
					control96Info=null;
				}
				else{
					control96Info=listControl96Infos[0];
				}
				if(LayoutSkipSelf(control96Info,control.Controls[i])){
					LayoutChildren(control.Controls[i]);
					continue;
				}
				LayoutHandleManualAlteration(ref control96Info,control.Controls[i]);
				if(control96Info==null){
					//any new or moved control will be caught above, in LayoutHandleManualAlteration
					//temporarily, if we're not in test mode, LayoutHandleManualAlteration mostly ignores these, so we might get a null here.
					continue;
				}
				//if(control.Controls[i].Anchor!=AnchorStyles.Left|Top || control.Controls[i].Dock!=DockStyle.None){
					//throw new ApplicationException("Controls cannot have their docking or anchor changed programmatically after creation: "+control.Controls[i].ToString());
				//}
				List<int> listBoxSelectedIndices=new List<int>();
				if(control.Controls[i] is System.Windows.Forms.ListBox listbox){
					//listboxes have a bug that resets their selected index each time you change the font. This bug happens for both One and MultiExtended
					for(int s=0;s<listbox.SelectedIndices.Count;s++){
						listBoxSelectedIndices.Add(listbox.SelectedIndices[s]);  
					}
				}
				//=======================================================================================================================================================================================
				//FONT===================================================================================================================================================================================
				//=======================================================================================================================================================================================
				float scaledFont;
				if(control.Controls[i] is System.Windows.Forms.TextBox
					|| control.Controls[i] is System.Windows.Forms.ComboBox
					|| control.Controls[i] is System.Windows.Forms.RichTextBox
					|| control.Controls[i] is System.Windows.Forms.TreeView
					|| control.Controls[i] is System.Windows.Forms.ListView
					|| control.Controls[i] is System.Windows.Forms.ListBox)//FormSheetFieldStatic
				{
					//Some MS controls need to have their font scaled completely, both MS and zoom.
					//Any change here must be matched in AddToList96 to unscale by the same amount.
					scaledFont=ScaleF(control96Info.FontSize96);
					//Well, this is frightening.
					//I created a public static field in ColorOD that initialized like this: =System.Windows.Media.Color.FromRgb(
					//That resulted in a change in behavior in all the Forms.TextBox.
					//scaledFont gets set above to 12.375, which used to behave well, showing as 12.375 in UI.
					//But after the Color reference, it shows an additional 50% bigger, meaning it shouldn't be scaled above in that scenario.
					//There's something in the black box that's altering it.
					//But the scary part is that it only behaves this way in one project: Unit Tests. Additional scaling does not happen in OpenDental.exe.
					//I cannot find any difference between the two projects.
					//It's easy enough to avoid hitting Media.Color in a public static field.
					//But if I get into a mixed window environment, then the first time I hit Media.Color (and probably other namespaces), all TextBoxes could flip to new behavior.
					//So I'll need to test that as I get closer.
					//This adds additional impetus to my move away from the black boxes. They are dangerous in so many unpredictable ways.
				}
				else{
					//But most controls only get zoomed by us, and then MS automatically does their own MS scaling.
					scaledFont=ScaleFontODZoom(control96Info.FontSize96);
					//Bad info: This group also includes RichTextBoxes, which adapt their own font to the current dpi, except OD zoom portion which we set.
				}
				if(control.Controls[i].Font.Bold){
					control.Controls[i].Font=new Font(control.Controls[i].Font.FontFamily,scaledFont,FontStyle.Bold);
				}
				else{
					control.Controls[i].Font=new Font(control.Controls[i].Font.FontFamily,scaledFont);
				}
				if(control.Controls[i] is System.Windows.Forms.ListBox listbox2){
					listbox2.ClearSelected();//jordan If it crashes on this line, then the top of this file regarding ListBoxes.
					for(int s=0;s<listBoxSelectedIndices.Count;s++){
						//for selectionMode.One, this should only run once
						listbox2.SetSelected(listBoxSelectedIndices[s],true);
					}  
				}
				//Interestingly, anchors and docks don't matter at all when simply scaling.
				//But they matter very much if the container is being resized.
				//Because MS dpi scaling logic is relative instead of absolute, it can't handle the resizing scenario without quickly getting buggy.
				if(control96Info.Dock!=DockStyle.None){
					listControlsDocked.Add(control.Controls[i]);
					continue;
				}
				float x=control96Info.RectangleF96Orig.X;
				float y=control96Info.RectangleF96Orig.Y;
				float width=control96Info.RectangleF96Orig.Width;
				float height=control96Info.RectangleF96Orig.Height;
				//No support for AnchorStyles.None
				if((control96Info.Anchor & (AnchorStyles.Left | AnchorStyles.Right)) == (AnchorStyles.Left | AnchorStyles.Right)){//left and right
					width+=sizeParentClientNow96.Width-control96InfoParent.ClientSize96Orig.Width;
				}
				else if((control96Info.Anchor & (AnchorStyles.Left | AnchorStyles.Right)) == AnchorStyles.Right){//only right
					x+=sizeParentClientNow96.Width-control96InfoParent.ClientSize96Orig.Width;
				}
				if((control96Info.Anchor & (AnchorStyles.Top | AnchorStyles.Bottom)) == (AnchorStyles.Top | AnchorStyles.Bottom)){//top and bottom
					height+=sizeParentClientNow96.Height-control96InfoParent.ClientSize96Orig.Height;
				}
				else if((control96Info.Anchor & (AnchorStyles.Top | AnchorStyles.Bottom)) == AnchorStyles.Bottom){
					y+=sizeParentClientNow96.Height-control96InfoParent.ClientSize96Orig.Height;
				}
				if(width<0){
					width=0;
				}
				if(height<0){
					height=0;
				}
				//Now, we apply the newly calculated positions to the control, converting to new scale
				control.Controls[i].Bounds=new Rectangle(Scale(x),Scale(y),Scale(width),Scale(height));
				if(control.Controls[i].Bounds.Height<0 || control.Controls[i].Bounds.Width<0){
					throw new Exception();
				}
				control96Info.BoundsLast=control.Controls[i].Bounds;
				//Control controlThis=listControls[i];
				//listControls[i].Left=Scale(scaleMy,x);
				//listControls[i].Top=Scale(scaleMy,y);
				//listControls[i].Width=Scale(scaleMy,width);
				//listControls[i].Height=Scale(scaleMy,height);
				//listControls[i].Location=new Point(xScaled,yScaled);
				LayoutChildren(control.Controls[i]);
			}
			//docking order matters.  Highest priority docked control is the one at the bottom of the list, so we go backward.
			RectangleF rectangleRemaining96=new RectangleF(0,0,sizeParentClientNow96.Width,sizeParentClientNow96.Height);
			for(int i=listControlsDocked.Count-1;i>=0;i--){
				Control96Info control96Info=_listControl96Infos.FirstOrDefault(x=>x.ControlRef==listControlsDocked[i]);//already verified that there is only one.
				float x=rectangleRemaining96.X;
				float y=rectangleRemaining96.Y;
				float width=rectangleRemaining96.Width;
				float height=rectangleRemaining96.Height;	
				if(control96Info.Dock==DockStyle.Top){
					height=control96Info.RectangleF96Orig.Height;
					rectangleRemaining96.Height-=height;
					rectangleRemaining96.Y+=height;
					//if(listControlsDocked[i].GetType()==typeof(OpenDental.UI.MenuOD)){
						//y-=2;//menus seem to want to draw one pixel down.  Not sure why.  Move up.
					//}
				}
				if(control96Info.Dock==DockStyle.Left){
					width=control96Info.RectangleF96Orig.Width;
					rectangleRemaining96.Width-=width;
					rectangleRemaining96.X+=width;
				}
				if(control96Info.Dock==DockStyle.Right){
					width=control96Info.RectangleF96Orig.Width;
					x=rectangleRemaining96.Right-width;
					rectangleRemaining96.Width-=width;
				}
				if(control96Info.Dock==DockStyle.Bottom){
					height=control96Info.RectangleF96Orig.Height;
					y=rectangleRemaining96.Bottom-height;
					rectangleRemaining96.Height-=height;
				}
				if(rectangleRemaining96.Width<1 || rectangleRemaining96.Height<1){
					continue;
				}
				if(control96Info.Dock==DockStyle.Fill){
					//already handled					
				}
				listControlsDocked[i].Bounds=new Rectangle(Scale(x),Scale(y),Scale(width),Scale(height));
				if(listControlsDocked[i].Bounds.Height<0 || listControlsDocked[i].Bounds.Width<0){
					throw new Exception();
				}
				control96Info.BoundsLast=listControlsDocked[i].Bounds;
				LayoutChildren(listControlsDocked[i]);
			}
			//control.ResumeLayout();
		}

		///<summary>If a programmer added, moved, or resized a control, this will identify it and throw an exception.</summary>
		private void LayoutHandleManualAlteration(ref Control96Info control96Info,Control control){
			if(control96Info!=null && control96Info.BoundsLast==control.Bounds){
				return;//no manual alteration
			}
			//if(control.Parent is TabPage){
				//tabPages are buggy beasts. If a control is too big, it will suddently move it hundreds of pixels to the left.  This fixes that problem.
			//	control.Bounds=control96Info.BoundsLast;
			//	return;
			//}
			if(!IsTestingMode()){
				return;//It will still malfunction, but at least it won't crash
			}
			//LayoutManager ran across a control that had been programmatically added, moved, or resized
			//This is not allowed.  It will be caught and fixed in basic early testing.
			//This section is for troubleshooting.  Use it to help identify the exact control that is the problem
			string nameControl=control.Name;
			string typeControl=control.GetType().ToString();
			Control controlParent=control.Parent;
			string nameParent=controlParent.Name;
			Control controlParentParent=null;
			string nameParentParent="";
			Control controlParentParentParent=null;
			string nameParentParentParent="";
			if(controlParent.Parent!=null){
				controlParentParent=controlParent.Parent;
				nameParentParent=controlParentParent.Name;
				if(controlParentParent.Parent!=null){
					controlParentParentParent=controlParentParent.Parent;
					nameParentParentParent=controlParentParentParent.Name;
				}
			}
			if(control96Info==null){
				//see notes above
				throw new ApplicationException("Controls cannot be added programmatically.  Add them using LayoutManager.Add(): "+control.Name);
			}
			if(control96Info.BoundsLast != control.Bounds){
				//see notes above
				Debug.WriteLine("");
				//throw new ApplicationException("Controls cannot be moved or resized programmatically.  Use one of the LayoutManager.Move() methods. Control: "+control.Name);
			}
		}

		///<summary>Returns true to indicate that we skip layout of children.</summary>
		private bool LayoutSkipChildren(Control96Info control96Info,Control control){
			if(control is UI.ComboBoxClinicPicker){
				return true;//there is a comboFake that we need to ignore.
			}
			if(control is UI.ControlApptPanel){
				//The 2 scrollbars are handled internally by ControlApptPanel.LayoutRecalcAfterResize
				return true;
			}
			if(control is DomainUpDown){
				//the child textbox has negative height because we made it so short
				return true;
			}
			if(control is UI.GridOD gridOD){
				gridOD.SetScaleAndZoom(_scaleMS,_zoomLocal);//grids are over in OpenDentBusiness, so this is instead of a ref to LayoutManager.
				return true;//grids handle their own layout of scrollbars, etc.
			}
			if(control is UI.GridOld gridOld){
				gridOld.SetScaleAndFont(_scaleMS,_zoomLocal);
				return true;
			}
			if(control is UI.ImageSelector){
				//The scrollbar is handled internally
				return true;
			}
			if(control is UI.ListBox){
				//The vScroll is handled internally.
				return true;
			}
			if(control is SignatureBoxWrapper signatureBoxWrapper){
				signatureBoxWrapper.SetScaleAndZoom(_scaleMS,_zoomLocal);//grids are over in OpenDentBusiness, so this is instead of a ref to LayoutManager.
				return true;//ignore the child SignatureBox and TopazWrapper
			}
			if(control is UI.TextBox){
				//The vScroll is handled internally.
				return true;
			}
			if(control is SparksToothChart.ToothChartWrapper){
				return true;//ignore the 3 toothchart controls contained within: ToothChartDirectX, ToothChart2D, and ToothChartOpenGL.
			}
			if(control is ScrollableControl scrollableControl && scrollableControl.AutoScroll){
				//We need to return true for these forms to avoid a bug with scrollable splitters or panels that can create uneccessary additional white space.
				List<string> listFormNames=new List<string>(){
					typeof(FormSheetDefEdit).Name,typeof(FormSheetFillEdit).Name,typeof(FormWebChatSession).Name,typeof(FormSmsTextMessaging).Name
				};
				if(control.TopLevelControl!=null && listFormNames.Contains(control.TopLevelControl.Name)) {
					return true;
				}
				//FormClaimEdit tabs at bottom work fine without this.
				//FormInsPlan cannot function if this returns true.
				//We always make notes here about each possible scrollable control that we run across and whether it needs true or false.
			}
			if(control is UserControlDashboard){
				return true;
			}
			return false;
		}

		///<summary>Returns true indicate that we skip layout of this type of control.  The decision to layout its children is separate.  Because these types of controls are skipped, there is also no complaint when they get resized by something else.</summary>
		private bool LayoutSkipSelf(Control96Info control96Info,Control control){
			if(control96Info==null){
				//this control must have been programmatically added, so it's not in the list yet.
				if(control is SparksToothChart.ToothChartDirectX){//this one can't be added manually, so we'll add it.
					AddToList96(control,control.Parent);
					control96Info=_listControl96Infos.FirstOrDefault(x=>x.ControlRef==control);
					return true;
				}
				if(control is SparksToothChart.ToothChart2D){
					AddToList96(control,control.Parent);
					control96Info=_listControl96Infos.FirstOrDefault(x=>x.ControlRef==control);
					return true;
				}
				if(control is SparksToothChart.ToothChartOpenGL){
					AddToList96(control,control.Parent);
					control96Info=_listControl96Infos.FirstOrDefault(x=>x.ControlRef==control);
					return true;
				}
				if(control is DataGridTextBox){
					//This is a child of DataGrid, which we don't control.
					return true;
				}
				return false;//don't skip.  Let it get caught in HandleManualAlteration
			}
			if(control is System.Windows.Forms.ComboBox){
				control96Info.BoundsLast.Height=control.Height;//we have no control over comboBox height, and we don't need automatic changes to trigger an alarm
				return false;//but do not skip this control.  It will ignore our attempt to change its height.
			}
			if(control is UI.MenuStripOD){//This is not MenuOD, but is the MenuStrip inside of it.
				//MenuOD lays out its own menustrip, I guess.  It's not entirely clear.
				return true;
			}
			//if(control is MonthCalendar){//removed all of them.
				//These are badly written and resize for various reasons on their own
				//But the location still needs to change, even though we don't seeem to have good control over size
			//	return true;
			//}
			//if(control is System.Windows.Forms.SplitterPanel){//removed all of them.
				//these get changed programmatically when SplitterDistance is changed, and also by end user when dragging a splitter
				//We can ignore these
				//But we do need to layout all its children
				//return true;
			//}
			if(control is UI.SplitterPanel){
				//SplitContainer is solely responsible for resizing SplitterPanel
				//But we do need to layout all its children
				return true;
			}
			if(control is System.Windows.Forms.TabPage){
				//TabControl only resizes tabPages that are visible/selected.  
				//We aren't allowed to resize tabPage ourselves, but we know what the size should be.
				//So we do skip tabPage here, but we also must handle children of tabPage differently.
				return true;
			}
			if(control is UI.TabPage){
				return true;
			}
			return false;
		}

		///<summary>Passes this LayoutManager object to any of our custom controls so that they have access to the same scale numbers as the form.</summary>
		private void PassLayoutManager(Control control){
			if(control is BugSubmissionControl bugSubmissionControl){
				bugSubmissionControl.LayoutManager=this;
				return;
			}
			if(control is UI.Button button){
				button.LayoutManager=this;
				return;
			}
			if(control is UI.CheckBox checkBox){
				checkBox.LayoutManager=this;
				return;
			}
			if(control is UI.ComboBoxClinicPicker comboBoxClinicPicker){
				comboBoxClinicPicker.LayoutManager=this;
				return;
			}
			if(control is UI.ComboBox comboBoxPlus){
				comboBoxPlus.LayoutManager=this;
				return;
			}
			if(control is ControlAccount controlAccount){
				controlAccount.LayoutManager=this;
				return;
			}
			if(control is ControlAppt controlAppt){
				controlAppt.LayoutManager=this;
				return;
			}
			if(control is UI.ControlApptPanel controlApptPanel){
				controlApptPanel.LayoutManager=this;
				return;
			}
			if(control is UI.ControlApptProvSlider controlApptProvSlider){
				controlApptProvSlider.LayoutManager=this;
				return;
			}
			if(control is ControlChart controlChart){
				controlChart.LayoutManager=this;
				return;
			}
			if(control is ControlFamily controlFamily){
				controlFamily.LayoutManager=this;
				return;
			}
			if(control is ControlImages controlImages){
				controlImages.LayoutManager=this;
				return;
			}
			if(control is ControlImagesJ controlImagesJ){
				controlImagesJ.LayoutManager=this;
				return;
			}
			if(control is ControlTreat controlTreat){
				controlTreat.LayoutManager=this;
				return;
			}
			if(control is ContrPerio contrPerio){
				contrPerio.LayoutManager=this;
				return;
			}
			if(control is ContrTable contrTable){
				contrTable.LayoutManager=this;
				return;
			}
			if(control is DashFamilyInsurance dashFamilyInsurance){//only gets hit during non-dashboard, in controlTreat
				dashFamilyInsurance.LayoutManager=this;
				return;
			}
			if(control is DashIndividualDiscount dashIndividualDiscount){//only gets hit during non-dashboard, in controlTreat
				dashIndividualDiscount.LayoutManager=this;
				return;
			}
			if(control is DashIndividualInsurance dashIndividualInsurance){//only gets hit during non-dashboard, in controlTreat
				dashIndividualInsurance.LayoutManager=this;
				return;
			}
			if(control is EmailPreviewControl emailPreviewControl){
				emailPreviewControl.LayoutManager=this;
				return;
			}
			if(control is InternalTools.Phones.EscalationView escalationView){
				escalationView.LayoutManager=this;
				return;
			}
			if(control is GraphScheduleDay graphScheduleDay){
				graphScheduleDay.LayoutManager=this;
				return;
			}
			//if(control.GetType()==typeof(UI.GridOD)){
			//Because grids are in OpenDentBusiness, we can't send them a ref to LayoutManager.
			//We can send them a scale, but it needs to be more frequent than just once here.
			//Grid scales are instead handled in LayoutChildren
			//	((UI.GridOD)control).ScaleMy=this.ScaleMy;
			//	return;
			//}
			if(control is OpenDental.UI.GroupBox groupBoxOD){
				groupBoxOD.LayoutManager=this;
				return;
			}
			if(control is UI.ImageSelector imageSelector){
				imageSelector.SetLayoutManager(this);
				return;
			}
			if(control is UI.LightSignalGrid lightSignalGrid){
				lightSignalGrid.LayoutManager=this;
				return;
			}
			if(control is UI.ListBox listBoxOD){
				listBoxOD.LayoutManager=this;
				return;
			}
			if(control is MapAreaPanel mapAreaPanel){
				mapAreaPanel.LayoutManager=this;
				return;
			}
			if(control is MapCubicle mapAreaRoomControl){
				mapAreaRoomControl.LayoutManager=this;
				return;
			}
			if(control is InternalTools.Phones.MapNumber mapNumber){
				mapNumber.LayoutManager=this;
				return;
			}
			if(control is InternalTools.Phones.MapPanel mapPanel){
				mapPanel.LayoutManager=this;
				return;
			}
			if(control is UI.MenuOD menuOD){
				menuOD.LayoutManager=this;
				return;
			}
			if(control is ModuleBar moduleBar){
				moduleBar.LayoutManager=this;
				return;
			}
			if(control is UI.MonthCalendarOD monthCalendarOD){
				monthCalendarOD.LayoutManager=this;
				return;
			}
			if(control is ODButtonPanel oDButtonPanel){
				oDButtonPanel.LayoutManager=this;
				return;
			}
			if(control is ODDatePicker oDDatePicker){
				oDDatePicker.LayoutManager=this;
				return;
			}
			if(control is ODDateRangePicker oDDateRangePicker){
				oDDateRangePicker.LayoutManager=this;
				return;
			}
			if(control is ODtextBox oDtextBox){
				oDtextBox.LayoutManager=this;
				return;
			}
			if(control is PhoneTile phoneTile){
				phoneTile.LayoutManager=this;
				return;
			}
			if(control is UI.PinBoard pinBoard){
				pinBoard.LayoutManager=this;
				return;
			}
			if(control is SheetEditMobileCtrl sheetEditMobileCtrl){
				sheetEditMobileCtrl.LayoutManager=this;
				return;
			}
			if(control is SmsThreadView smsThreadView){
				smsThreadView.LayoutManager=this;
				return;
			}
			if(control is UI.SplitContainer splitContainer){
				splitContainer.LayoutManager=this;
				return;
			}
			if(control is UI.TabControl tabControl){
				tabControl.LayoutManager=this;
				return;
			}
			if(control is UI.TabPage tabPage){
				tabPage.LayoutManager=this;
				return;
			}
			if(control is UI.TextBox textBox){
				textBox.LayoutManager=this;
				return;
			}
			if(control is UI.ToggleDayWeek toggleDayWeek){
				toggleDayWeek.LayoutManager=this;
				return;
			}
			if(control is UI.ToolBarOD toolBarOD){
				toolBarOD.LayoutManager=this;
				return;
			}
			if(control is UI.UnmountedBar unmountedBar){
				unmountedBar.LayoutManager=this;
				return;
			}
			if(control is UserControlDashboard userControlDashboard){
				userControlDashboard.LayoutManager=this;
				return;
			}
			if(control is UserControlPhoneSmall userControlPhoneSmall){
				userControlPhoneSmall.LayoutManager=this;
				return;
			}
			if(control is UserControlReminderAgg userControlReminderAgg){
				userControlReminderAgg.LayoutManager=this;
				return;
			}
			if(control is UserControlSecurityTree userControlSecurityTree){
				userControlSecurityTree.LayoutManager=this;
				return;
			}
			if(control is UserControlSecurityUserGroup userControlSecurityUserGroup){
				userControlSecurityUserGroup.LayoutManager=this;
				return;
			}
			if(control is UserControlTasks userControlTasks){
				userControlTasks.LayoutManager=this;
				return;
			}
			if(control is UI.WindowingSlider windowingSlider){
				windowingSlider.LayoutManager=this;
				return;
			}
			if(control is UI.ZoomSlider zoomSlider){
				zoomSlider.SetLayoutManager(this);
				return;
			}
		}
		#endregion Methods - Layout
	}

	///<summary>This keeps track of control or form bounds and font, all at 96dpi.  This allows absolute positioning instead of relative positioning.</summary>
	public class Control96Info{
		///<summary>A reference to the control that this info refers to.</summary>
		public Control ControlRef;
		///<summary>Helps with troubleshooting. Some controls do not have names, and other names are duplicates.  For example, vertical scrollbars on two different grids.</summary>
		public string Name;
		///<summary>The original bounds of this control at virtual 96dpi pixels.  Starts out as the actual numbers from the designer.  Doesn't get changed with any automatic sizing so that it preserves the original spacial relationships.  It does get changed when a programmer intentionally moves a control, but the number stored here is as it would be in the original unsized form.</summary>
		public RectangleF RectangleF96Orig;
		///<summary>Nobody but the LayoutManager should be moving controls for any reason.  If they do, this lets the LayoutManager sense it.  These bounds are at current scale, not 96 dpi, so not float.</summary>
		public Rectangle BoundsLast;
		///<summary>ClientSize at 96 dpi.</summary>
		public SizeF ClientSize96Orig;
		public float FontSize96;
		public AnchorStyles Anchor;
		///<summary>This takes priority over anchor.</summary>
		public DockStyle Dock;

		///<summary>This keeps track of control or form bounds and font, all at 96dpi.  This allows absolute positioning instead of relative positioning.</summary>
		public Control96Info(Control control){
			ControlRef=control;
			//Handle=control.Handle;//if a form is passed in, this line will trigger an undesirable resize of the form.
			if(control.Name==""){
				Name="Unnamed "+control.GetType().ToString();
			}
			else{
				Name = control.Name;
			}
			RectangleF96Orig=control.Bounds;
			if(control.Bounds.Height<0 || control.Bounds.Width<0){
				throw new Exception();
			}
			BoundsLast=control.Bounds;
			ClientSize96Orig=control.ClientSize;
			FontSize96=control.Font.Size;
			Anchor=control.Anchor;
			Dock=control.Dock;
		}

		public override string ToString(){
			return Name;
		}

		/*
		public Size Size=> Bounds.Size;

		public int Width=> Bounds.Width;

		public int Height=> Bounds.Height;

		public Point Location=> Bounds.Location;*/
	}


}

//2023-03-26 Sheets still to do:
//ScreenChart (done except for scaling. It's good enough)

//2023-03-26-Unresolved high dpi issues:
//Daily phone graph numbers are too big
//Sheet: dashboard testing
//Note at bottom of TP module is too big
//Calendar control at right of TP module is too small.
//Get rid of GridOld
//Text mount numbers
//Print preview shows too big
//Rx print preview shows too small?
//Appoint highlight outline needs to be thicker

//Bugs that I have not been able to fix, but might be tolerable for a while:
//When printing Chart Prog Notes, the rows on printout are too tall, and then the rows in UI get slightly too short.
//FormMap, text in MapNumbers boxes is slightly too small in some places and too tall in others
//Grids on sheets show with the dummy row, last column is too narrow. (this is not new)
//When editing chart layout, the right side of the window doesn't draw at first.

//Minor annoyances for later that I will not fix in the first pass have been moved to the Manual page: Zoom Artifacts

//Areas that still need to be tested. I put them off because they were too hard to set up:
//SmsThreadView for chat
//There are 6 kinds of grids that go on statements. Verify that they all work in both SheetDefEdit and when printing.

//Areas that look good, but the math needs to be reviewed:
//Pinboard, drag appts to this pinboard
//MapPanel HitTest.
//MapPanel.FitText. Why do I need to unscale it again. It was already unscaled earlier.
//CheckBox.IsOnActualText. I shouldn't have to scale the measurement

//Features to add later:
//Edit a referral letter.
//Custom textbox
//custom radiobutton
//convert validdouble etc into textbox


