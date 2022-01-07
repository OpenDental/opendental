using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDental.User_Controls;
using OpenDentBusiness;

namespace OpenDental{
	///<summary>This handles the bounds and fonts of all the controls on a form.  See notes on FormODBase.LayoutManager.  Gets passed to our custom controls on a form.</summary>
	public class LayoutManagerForms{
		#region Fields
		///<summary>Height of title bar at 96 dpi. Includes any border lines, if we decide to draw them. Icon is usually 16.</summary>
		private int _heightTitleBar96=26;
		///<summary>See FormODBase.IsLayoutMS.  This is a copy.</summary>
		public bool IsLayoutMS;
		public bool Is96dpi=false;
		///<summary>True if in the middle of laying out. Moving controls can sometimes trigger another layout event. Example, sizing a split container triggers its splitterMoved event.  This bool prevents another layout from starting in cases like that.</summary>
		public bool IsLayingOut;
		///<summary>This is a list of the original 96dpi layout info, as it was in the designer.</summary>
		private List<Control96Info> _listControl96Infos;
		///<summary>When maximized, this is the additional inset of panelClient on all 4 sides to compensate for the perimeter getting cut off. This doesn't get scaled.</summary>
		public int MaxInset=8;
		///<summary>Example 1.5. This is the scale of the current screen for the form that owns this instance of LayoutManager.  It gets combined with ComputerPrefs.LocalComputer.Zoom to create ScaleMy.</summary>
		private float _scaleMS=1;
		private Size _sizeClientOriginal;
		///<summary>L,R,B border at 96dpi</summary>
		private int _widthBorder96=5;
		///<summary>Pulled from ComputerPrefs.LocalComputer.Zoom.</summary>
		private int _zoomLocal=0;
		///<summary>Example 40 to test 140% zoom</summary>
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
			_sizeClientOriginal=formODBase.ClientSize;//permanently remember the original client size before Windows fiddles with it.
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
		///<summary>Adds any Control to any parent Control.  Prior to adding a control here, set its properties.  Location, Size, and Font must first be adjusted to current dpi scale.  For Location, it's usually simplest to use relative or measured numbers because they are already scaled. For Font, usually just set it to form.Font or to any other Font which is already scaled.  For absolute values that are at 96dpi, like maybe a specific Size for example, pass it through LayoutManager.Scale, .ScaleF, .ScaleSize, etc. prior to adding the Control.  Recursively adds info about the children to our internal tracking list.  Supports re-adding a control that was previously removed from the form programmatically as long as you didn't manually move or add any children.  When you are setting properties prior to using add, just do it like normal rather than using LayoutManager.Move.</summary>
		public void Add(Control control, Control parent){
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
					//throw new ApplicationException("Not allowed to add controls to the form itself.  Add to form.PanelClient instead.");
				}
			}
			//We support multiple Add/Removes of the same controls.
			Control96Info control96Info=_listControl96Infos.FirstOrDefault(x=>x.ControlRef==control);
			if(control96Info==null){
				//this must be the initial add. Otherwise, this control and all its children will already still be in _listControl96Infos
				AddToList96(control,parent);//and children
			}
			else{
				//this control was found, no need to add it.
				//We are assuming that all the children are still in the same place as when the control was removed.
				//If not, it will be very obvious when it hits our included Exception.
				//We are also assuming that no new controls were added to this.
				//Again, a new control will be obvious when it hits our exception.
			}
			//can't easily swap line below with line above, because adding a control to a form frequently changes its size and location.  Example: docking. 
			//The above line gets rid of all innate docking so that adding it to the form results in no change.
			parent.Controls.Add(control);//only this control gets added.  The children are already part of it.
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
			control96Info.ClientSize96=new SizeF(UnscaleF(control.ClientSize.Width),UnscaleF(control.ClientSize.Height));
			control96Info.FontSize96=UnscaleF(control.Font.Size);
			control96Info.BoundsF96=CalculateBounds96(control.Bounds,parent,control.Anchor);
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
					sizeParentClientOriginal96=new SizeF(_sizeClientOriginal);
				}
				else{
					throw new ApplicationException("Parent control not found in _listControl96Infos.  You're probably using this method wrong.");
				}
			}
			else{
				sizeParentClientOriginal96=control96InfoParent.ClientSize96;
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

		///<summary>Removes the specified control from its parent.  Works for any of the situations where controls are dynamically added/removed. Example: TabPages.  This does not do control.Dispose, nor does it remove the control from our tracked list.  This way, if it (and its children) are later added back, they are still available.  But, if a control is disposed in form code, we should add another method here to remove it from our list.</summary>
		public void Remove(Control control){
			//This doesn't actually do anything, but it's a placeholder for possible changes to TabPages.
			//For all other Remove situations, don't run them through LayoutManager.  Just use native Remove, Clear, etc.
			//Only gotcha is that Form.Controls.Clear() needs to change to Form.PanelClient.Controls.Clear()
			if(!(control is TabPage)){
				throw new Exception("Not yet designed for anything but TabPage.");
			}
			if(control.Parent!=null){
				control.Parent.Controls.Remove(control);
			}
			//Do not remove children from tabPage, because we need those children to still be there if we add the tabPage back later.
			//Do not remove children from _listControl96Infos because we would lose info about their original anchors, etc.
			//One reason this works for tabPages is that they have no layout properties.
			//Also, we added code to the ADD method
			//We're not even going to remove this control from _listControl96Infos because it's harmless sitting there, and then we don't have to add it back later.
			//If a control is then disposed in form code, the Control96Info object will remain here.  That shouldn't be a problem except possibly on the main form.
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
			control96Info.BoundsF96=CalculateBounds96(boundsScaled,control.Parent,control96Info.Anchor);
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
				throw new Exception("Splitters are not allowed. Use a SplitContainer or a manual panel that behaves like a splitter.");
			}
			control.Anchor=AnchorStyles.Left | AnchorStyles.Top;
			control.Dock=DockStyle.None;
			if(control is ButtonBase buttonBase){
				buttonBase.AutoSize=false;
			}
			if(control is ContainerControl containerControl){
				containerControl.AutoScaleMode=AutoScaleMode.None; 
			}
			if(control is ListBox listBox){
				listBox.IntegralHeight=false;
			}
		}

		///<summary>Adds PanelClient to this form, then moves all the controls into that panel.  Sets all the controls to no longer be docked or anchored so that the custom layout code on the form is responsible for that.</summary>
		public void SetPanelClient(FormODBase formODBase){
			formODBase.PanelClient=new Panel();
			formODBase.PanelClient.Name="PanelClient";
			//This fixes the problem caused by the system border, which had slightly shrunk our clientSize, especially if launching on high dpi.
			//It will trigger a resize event, which will layout controls under MS control, restoring all positions to original.
			if(!FormODBase.AreBordersMS){
				formODBase.Size=_sizeClientOriginal;
			}
			formODBase.PanelClient.Size=_sizeClientOriginal;
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
				formODBase.PanelClient.Anchor=AnchorStyles.Left|AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Right;
			}
			else{
				//These get ignored if maximized:
				formODBase.Width=formODBase.PanelClient.Width+2*_widthBorder96;//Warning: this triggers resize event
				formODBase.Height=formODBase.PanelClient.Height+_heightTitleBar96+_widthBorder96;
				//Application.DoEvents();
				formODBase.PanelClient.Location=new Point(_widthBorder96,_heightTitleBar96);//Dpi.Scale(this,_widthBorder96),Dpi.Scale(this,_heightTitleBar96));
			}
			formODBase.Controls.Add(formODBase.PanelClient);
			_listControl96Infos.Add(new Control96Info(formODBase.PanelClient));
			formODBase.PanelBorders=new PanelOD();
			formODBase.PanelBorders.Name="PanelBorders";
			formODBase.PanelBorders.Bounds=new Rectangle(new Point(0,0),formODBase.Size);
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
					continue;//PanelClient needs to stay anchored to all for sides
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
			control96Info.BoundsF96=CalculateBounds96(control.Bounds,control.Parent,control.Anchor);
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
		public int HeightTitleBar(){
			return Scale(_heightTitleBar96);
		}

		///<summary>Example 1.5. This is the scale of this form and all its controls, compared to 96dpi as 100%.  It's a combination of _scaleMS and ComputerPrefs.LocalComputer.Zoom.</summary>
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
			int zoomLocal=_zoomLocal;
			try{
				zoomLocal=ComputerPrefs.LocalComputer.Zoom;
			}
			catch{
				//this fails during version update, for example
			}
			if(zoomLocal!=_zoomLocal){
				_zoomLocal=zoomLocal;
				ZoomChanged?.Invoke(this,new EventArgs());
			}
			float retVal=_scaleMS+zoomLocal/100f;
			return retVal;
		}

		public float GetScaleMS(){
			return _scaleMS;
		}

		public void SetScaleMS(float scaleMS){
			_scaleMS=scaleMS;
			ZoomChanged?.Invoke(this,new EventArgs());
		}

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
			return (int)Math.Round(val96*ScaleMy());
		}

		///<summary>Converts a float or int from 96dpi to current scale.</summary>
		public float ScaleF(float val96){
			return val96*ScaleMy();
		}

		///<summary>Converts a size from 96dpi to current scale.</summary>
		public Size ScaleSize(Size size96){
			return new Size((int)Math.Round(size96.Width*ScaleMy()),(int)Math.Round(size96.Height*ScaleMy()));
		}

		///<summary>Converts a float or int from current screen dpi to 96dpi. There is no need for an Unscale method that returns int because unscaling should always use floats.</summary>
		public float UnscaleF(float valScreen){
			return valScreen/ScaleMy();
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
				formODBase.Font=new Font("Microsoft Sans Serif",ScaleF(8.25f));
			}
			if(FormODBase.AreBordersMS){
				formODBase.PanelClient.Bounds=formODBase.ClientRectangle;
			}
			else if(formODBase.WindowState==FormWindowState.Maximized){
				formODBase.PanelClient.Bounds=new Rectangle(MaxInset+Scale(_widthBorder96),MaxInset+Scale(_heightTitleBar96),
					formODBase.Width-MaxInset*2-Scale(_widthBorder96*2), formODBase.Height-MaxInset*2-Scale(_heightTitleBar96+_widthBorder96));
			}
			else{
				formODBase.PanelClient.Bounds=new Rectangle(Scale(_widthBorder96),Scale(_heightTitleBar96),
					formODBase.Width-Scale(_widthBorder96*2),formODBase.Height-Scale(_heightTitleBar96+_widthBorder96));
			}
			formODBase.PanelBorders.Bounds=new Rectangle(0,0,formODBase.Width,formODBase.Height);
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
			if(control is TabPage tabPage){
				//TabControl only resizes tabPages that are visible/selected.  
				//We aren't allowed to resize tabPage ourselves, but we know what the size should be.
				//So, we must handle tabPage size specially. No support here for autoscroll, with bigger clientSize.
				Size sizeTabPage=((TabControl)tabPage.Parent).DisplayRectangle.Size;
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
				if(control.Controls[i] is ListBox listbox){
					//listboxes have a bug that resets their selected index each time you change the font. This bug happens for both One and MultiExtended
					for(int s=0;s<listbox.SelectedIndices.Count;s++){
						listBoxSelectedIndices.Add(listbox.SelectedIndices[s]);  
					}
				}
				if(control.Controls[i] is RichTextBox){
					//These are smart buggers.  They adapt their own internal scaling to the current dpi, so we need to leave font at equivalent 96dpi font.
				}
				else{
					//Text was found to not fit when scaled exactly the same as the control.  Did testing.
					//Main test was MS SS 8.25 label, check, and radio.  They all behaved very similarly in my test environment.
					//I used FlatStyle=Standard for the testing.  If text on checkbox looks too small, change it to FlatStyle.Standard.
					//UseCompatibleTextRendering=false. This is the default.  This means not compatible with .NET 1.0 GDI+. This is more compact.
					//Width of each was about 400, adjusted so that even one reduction in pixel width would make the text wrap.
					//I also tested at 3 pixels wider.  I tested in increments between -20% and 240%.
					//There was huge variation.  I think the font size jumps in increments instead of smoothly scaling.
					//Tried Segoe font, but it's bigger than MSSS, so it would require re-layout of all forms.  No.
					//In the end, after hours of testing, it required a factor of 0.9 to avoid all artifacts.
					//One quirk is that a 10% zoom doesn't increase font at all.
					//But 90% was too small in practice, especially for FlatStyle.System.  0.92 looks better, and no obvious artifacts in FormModuleSetup.
					float scaledFont=ScaleF(control96Info.FontSize96);
					if(ScaleMy()!=1){
						if(control.Controls[i] is UI.Button){
							//no change
						}
						else if(control.Controls[i] is ListBox){
							scaledFont*=0.95f;//prevents most of the issues with scrollbars being automatically added 
							//the new ListBoxOD does not require this hack; it scales nicely.
						}
						else if(control.Controls[i] is Label
							|| control.Controls[i] is CheckBox
							|| control.Controls[i] is RadioButton)
						{
							scaledFont*=0.92f;
						}
						else{
							//no change
						}
					}
					if(control.Controls[i].Font.Bold){
						control.Controls[i].Font=new Font(control.Controls[i].Font.FontFamily,scaledFont,FontStyle.Bold);
					}
					else{
						control.Controls[i].Font=new Font(control.Controls[i].Font.FontFamily,scaledFont);
					}
				}
				if(control.Controls[i] is ListBox listbox2){
					listbox2.ClearSelected();//jordan If it crashes on this line, then see Wiki: Programming Pattern - Layout Manager, ListBoxes.
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
				float x=control96Info.BoundsF96.X;
				float y=control96Info.BoundsF96.Y;
				float width=control96Info.BoundsF96.Width;
				float height=control96Info.BoundsF96.Height;
				//No support for AnchorStyles.None
				if((control96Info.Anchor & (AnchorStyles.Left | AnchorStyles.Right)) == (AnchorStyles.Left | AnchorStyles.Right)){//left and right
					width+=sizeParentClientNow96.Width-control96InfoParent.ClientSize96.Width;
				}
				else if((control96Info.Anchor & (AnchorStyles.Left | AnchorStyles.Right)) == AnchorStyles.Right){//only right
					x+=sizeParentClientNow96.Width-control96InfoParent.ClientSize96.Width;
				}
				if((control96Info.Anchor & (AnchorStyles.Top | AnchorStyles.Bottom)) == (AnchorStyles.Top | AnchorStyles.Bottom)){//top and bottom
					height+=sizeParentClientNow96.Height-control96InfoParent.ClientSize96.Height;
				}
				else if((control96Info.Anchor & (AnchorStyles.Top | AnchorStyles.Bottom)) == AnchorStyles.Bottom){
					y+=sizeParentClientNow96.Height-control96InfoParent.ClientSize96.Height;
				}
				if(control.Controls[i] is ListBox listBox3){
					if(ScaleMy()<1 && listBox3.Name=="listTextOk"){
						//this fixes a specific bug in FormPatientEdit.ListTextOk, where we could not select any item when negative zoom was in place
						width=control96Info.BoundsF96.Width+5;
					}
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
					height=control96Info.BoundsF96.Height;
					rectangleRemaining96.Height-=height;
					rectangleRemaining96.Y+=height;
					//if(listControlsDocked[i].GetType()==typeof(OpenDental.UI.MenuOD)){
						//y-=2;//menus seem to want to draw one pixel down.  Not sure why.  Move up.
					//}
				}
				if(control96Info.Dock==DockStyle.Left){
					width=control96Info.BoundsF96.Width;
					rectangleRemaining96.Width-=width;
					rectangleRemaining96.X+=width;
				}
				if(control96Info.Dock==DockStyle.Right){
					width=control96Info.BoundsF96.Width;
					x=rectangleRemaining96.Right-width;
					rectangleRemaining96.Width-=width;
				}
				if(control96Info.Dock==DockStyle.Bottom){
					height=control96Info.BoundsF96.Height;
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
				gridOD.ScaleMy=ScaleMy();//grids are over in OpenDentBusiness, so this is instead of a ref to LayoutManager.
				return true;//grids handle their own layout of scrollbars, etc.
			}
			if(control is UI.ImageSelector || control is UI.ImageSelectorTemp){
				//The scrollbar is handled internally
				return true;
			}
			if(control is SparksToothChart.ToothChartWrapper){
				return true;//ignore the 3 toothchart controls contained within: ToothChartDirectX, ToothChart2D, and ToothChartOpenGL.
			}
			if(control is ScrollableControl scrollableControl && scrollableControl.AutoScroll){
				//We need to return true for these forms to avoid a bug with scrollable splitters or panels that can create uneccessary additional white space.
				List<string> listFormNames=new List<string>(){"FormSheetDefEdit","FormSheetFillEdit"};
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
			if(control is ComboBox){
				control96Info.BoundsLast.Height=control.Height;//we have no control over comboBox height, and we don't need automatic changes to trigger an alarm
				return false;//but do not skip this control.  It will ignore our attempt to change its height.
			}
			if(control is UI.MenuStripOD){//This is not MenuOD, but is the MenuStrip inside of it.
				//MenuOD lays out its own menustrip, I guess.  It's not entirely clear.
				return true;
			}
			//if(control is MonthCalendar){
				//These are badly written and resize for various reasons on their own
				//But the location still needs to change, even though we don't seeem to have good control over size
			//	return true;
			//}
			if(control is SplitterPanel){
				//these get changed programmatically when SplitterDistance is changed, and also by end user when dragging a splitter
				//We can ignore these
				//But we do need to layout all its children
				return true;
			}
			if(control is TabPage){
				//TabControl only resizes tabPages that are visible/selected.  
				//We aren't allowed to resize tabPage ourselves, but we know what the size should be.
				//So we do skip tabPage here, but we also must handle children of tabPage differently.
				return true;
			}
			return false;
		}

		///<summary>Passes this LayoutManager object to any of our custom controls so that they have access to the same scale number as the form.</summary>
		private void PassLayoutManager(Control control){
			if(control is BugSubmissionControl bugSubmissionControl){
				bugSubmissionControl.LayoutManager=this;
				return;
			}
			if(control is UI.Button button){
				button.LayoutManager=this;
				return;
			}
			if(control is UI.ComboBoxClinicPicker comboBoxClinicPicker){
				comboBoxClinicPicker.LayoutManager=this;
				return;
			}
			if(control is UI.ComboBoxOD comboBoxPlus){
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
			if(control is DashIndividualInsurance dashIndividualInsurance){//only gets hit during non-dashboard, in controlTreat
				dashIndividualInsurance.LayoutManager=this;
				return;
			}
			if(control is EmailPreviewControl emailPreviewControl){
				emailPreviewControl.LayoutManager=this;
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
			if(control is UI.GroupBoxOD groupBoxOD){
				groupBoxOD.LayoutManager=this;
				return;
			}
			if(control is UI.ImageSelector imageSelector){
				imageSelector.SetLayoutManager(this);
				return;
			}
			if(control is UI.ImageSelectorTemp imageSelectorTemp){
				imageSelectorTemp.SetLayoutManager(this);
				return;
			}
			if(control is UI.LightSignalGrid lightSignalGrid){
				lightSignalGrid.LayoutManager=this;
				return;
			}
			if(control is UI.ListBoxOD listBoxOD){
				listBoxOD.LayoutManager=this;
				return;
			}
			if(control is MapAreaPanel mapAreaPanel){
				mapAreaPanel.LayoutManager=this;
				return;
			}
			if(control is MapAreaRoomControl mapAreaRoomControl){
				mapAreaRoomControl.LayoutManager=this;
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
			if(control is PhoneTile phoneTile){
				phoneTile.LayoutManager=this;
				return;
			}
			if(control is UI.PinBoard pinBoard){
				pinBoard.LayoutManager=this;
				return;
			}
			if(control is SmsThreadView smsThreadView){
				smsThreadView.LayoutManager=this;
				return;
			}
			if(control is UI.ToolBarOD toolBarOD){
				toolBarOD.LayoutManager=this;
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
		public RectangleF BoundsF96;
		///<summary>Nobody but the LayoutManager should be moving controls for any reason.  If they do, this lets the LayoutManager sense it.  These bounds are at current scale, not 96 dpi, so not float.</summary>
		public Rectangle BoundsLast;
		///<summary>ClientSize at 96 dpi.</summary>
		public SizeF ClientSize96;
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
			BoundsF96=control.Bounds;
			if(control.Bounds.Height<0 || control.Bounds.Width<0){
				throw new Exception();
			}
			BoundsLast=control.Bounds;
			ClientSize96=control.ClientSize;
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

//2021-01-03
//There are still a few minor Font issues when primary monitor is not 100%, including:
//Print progress notes, rows are slightly too tall.
//Need to check sheet drawing, like with grids
//Messaging buttons at lower left of main window are too short.
//Thumbnail not avail text is either too big or too small
//At 200%, a few crowded labels in Module Prefs
//I hate the way the calendar scales.
//Buttons generally look too big
//Appt prov bar is too narrow.
//At 200%, notice clicking area of +/- in Imaging is too small


