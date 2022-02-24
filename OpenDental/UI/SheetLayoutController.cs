using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {

	///<summary>A class that directly manipulates a UserControl which uses the dynamic sheetDef framework.</summary>
	public class SheetLayoutController {
		///<summary>Gets set in constructor.</summary>
		public LayoutManagerForms LayoutManager;
		///<summary>List of all custom and internal SheetDefs for this dynamic control based on given _sheetType.  Will always contain the internal definition as last item in list.</summary>
		public List<SheetDef> ListSheetDefsLayout;
		///<summary>The control that contains other controls that are placed and sized based on their corresponding sheetFieldDefs.</summary>
		private UserControl _controlHosting;
		///<summary>The sheetDef layout from ListLayoutSheetDefs which is currently being used to set the layout of this control.</summary>
		private SheetDef _sheetDefDynamicLayoutCur;
		///<summary>The sheet type which is used to define the dynamic layout.</summary>
		private SheetTypeEnum _sheetType;
		///<summary>Used when there are controls that are not part of they dynamic framework but still need to show.  Controls in list will affect the dynamic controls fill logic based on their position and visibility.</summary>
		private List<Control> _listControlsStatic;
		///<summary>The currently logged in UserOd.UserNum.</summary>
		private long _userNumCur;
		///<summary>Only set true when user has been logged off, after a layout is finished this is always set back to false.</summary>
		private bool _hasUserLoggedOff;
		///<summary>The selected ClinicNum the last time the SheetLayout was initialized.</summary>
		private long _clinicNumCur;

		///<summary>Returns the currently loaded layout. May be null if ListLayoutSheetDefs is empty and the layout hasn't been initialized.</summary>
		public SheetDef SheetDefDynamicLayoutCur {
			get {
				return _sheetDefDynamicLayoutCur;
			}
		}

		///<summary>Set arrayStaticControls to any controls that are always visible or handle their own layout logic.  Controls that are dynamically resized will never impinge on these static controls.</summary>
		public SheetLayoutController(LayoutManagerForms layoutManager,UserControl controlHosting,params Control[] arrayStaticControls) {
			LayoutManager=layoutManager;
			_controlHosting=controlHosting;
			if(controlHosting is ControlChart) {
				_sheetType=SheetTypeEnum.ChartModule;
			}
			else {
				throw new ApplicationException("Host control of type "+controlHosting.GetType().Name+" not supported yet.");
			}
			_listControlsStatic=arrayStaticControls.ToList();
		}
		
		///<summary>Reloads the control with any new sheet layouts available.  This should only be called if a dynamic sheetDef was added/modified/deleted, the SheetLayoutMode has changed, or a new user signed in.</summary>
		public void ReloadSheetLayout(SheetFieldLayoutMode sheetFieldLayoutMode,Dictionary<string,Control> dictionaryControls) {
			long practiceDefaultSheetDefNum=PrefC.GetLong(PrefName.SheetsDefaultChartModule);
			ListSheetDefsLayout=SheetDefs.GetCustomForType(_sheetType);
			SheetInternalType sheetInternalType=SheetsInternal.GetInternalType(_sheetType);
			SheetDef sheetDefInternal=SheetsInternal.GetSheetDef(sheetInternalType);
			ListSheetDefsLayout.Add(sheetDefInternal);//Internal at bottom of list. UI reflects this too.
			ListSheetDefsLayout=ListSheetDefsLayout
				.OrderBy(x => x.SheetDefNum!=practiceDefaultSheetDefNum)//practice default sheetdef should be at the top of the list
				.ThenBy(x => x.SheetDefNum==0)//if the internal sheetdef is not the default it should be last
				.ThenBy(x => x.Description)//order custom sheetdefs by description
				.ThenBy(x => x.SheetDefNum).ToList();//then by SheetDefNum to be deterministic order
			SheetDef sheetDefUser=GetLayoutForUser();
			InitLayoutForSheetDef(sheetDefUser,sheetFieldLayoutMode,dictionaryControls,isForcedRefresh:true);//Force refresh in case they edit current layout.
		}
		
		///<summary>Attempts to find a UserOdPref indicating to the the most recently loaded layout for user, defaulting to the practice default if not found, then defaulting to the first SheetDef in listLayoutSheetDefs.  Returns null if listLayoutSheetDefs is null or empty.</summary>
		public SheetDef GetLayoutForUser() {
			//Avoid changing the layout when user is simply navigating and switching view modes.
			//If this user is using the practice default and then the practice default is changed by another user
			//we want to continue to use the same layout the user is currently viewing.
			//If the user logs off/exits for the day the next time they log in they will get the new practice default layout.
			if(!_hasUserLoggedOff
				&&_userNumCur==Security.CurUser.UserNum
				&&_clinicNumCur==Clinics.ClinicNum
				&&_sheetDefDynamicLayoutCur!=null
				&&ListSheetDefsLayout.Any(x => x.SheetDefNum==_sheetDefDynamicLayoutCur.SheetDefNum))
			{
				return ListSheetDefsLayout.First(x => x.SheetDefNum==_sheetDefDynamicLayoutCur.SheetDefNum);
			}
			#region UserOdPref based layout selection. If no UserOdPref use practice default or first item in list as last line of defense.
			SheetDef def=null;
			List<UserOdPref> listPrefs=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.DynamicChartLayout);
			UserOdPref userPref=listPrefs.FirstOrDefault();//There is only at most a single link per user.
			if(userPref!=null//User has viewed the chart module before
				&& ListSheetDefsLayout.Any(x => x.SheetDefNum==userPref.Fkey))//Layout still exists in the list of options
			{
				def=ListSheetDefsLayout.FirstOrDefault(x => x.SheetDefNum==userPref.Fkey);//Use previous layout when not practice or Clinic default.
			}
			else {//Try to use the practice default.
				//If there is a Clinic default, get it.
				if(!PrefC.HasClinicsEnabled || Clinics.ClinicNum==0 
					|| !ClinicPrefs.TryGetLong(PrefName.SheetsDefaultChartModule,Clinics.ClinicNum,out long sheetDefNum)) 
				{
					//Either, clinics are off, HQ is selected, or ClinicPref did not exist.
					if(_hasUserLoggedOff) {//Currently the cache is not loaded fast enough after logging back on to trust.
						sheetDefNum=PIn.Long(PrefC.GetStringNoCache(PrefName.SheetsDefaultChartModule));
					}
					else {
						sheetDefNum=PrefC.GetLong(PrefName.SheetsDefaultChartModule);//Serves as our HQ default.
					}
				}
				def=ListSheetDefsLayout.FirstOrDefault(x => x.SheetDefNum==sheetDefNum);//Can be null
			}
			if(def==null) {//Just in case.
				def=ListSheetDefsLayout[0];//Use first in the list.
			}
			#endregion
			return def;
		}
		
		///<summary>Uses the given sheetDef to set the location, size, and anchors for dynamic controls.  Usually only called from outside base DynamicLayoutControl when UI is switching to a different layout sheetDef.  Set isUserSelection true in order to save the current layout to the user's preference override.</summary>
		public void InitLayoutForSheetDef(SheetDef sheetDef,SheetFieldLayoutMode sheetFieldLayoutMode,Dictionary<string,Control> dictionaryControls,
			bool isForcedRefresh=false,bool isUserSelection=false)
		{
			_hasUserLoggedOff=false;//At this point we are showing the chart module to a user, reset.
			_userNumCur=Security.CurUser.UserNum;
			_clinicNumCur=Clinics.ClinicNum;
			if(!isForcedRefresh && _sheetDefDynamicLayoutCur!=null && sheetDef.SheetDefNum==_sheetDefDynamicLayoutCur.SheetDefNum){
				//Not forcing a refresh and _dynamicLayoutCur and sheetDef are the same sheet. No point in re-running logic.  Prevents flicker.
				return;
			}
			_sheetDefDynamicLayoutCur=sheetDef;
			if(_sheetDefDynamicLayoutCur.SheetDefNum!=0) {//0 represents an internal sheetDef, internal sheet defs do not need their field and params set.
				SheetDefs.GetFieldsAndParameters(_sheetDefDynamicLayoutCur);
			}
			List<SheetFieldDef> listSheetFieldDefs=GetPertinentSheetFieldDefs(sheetFieldLayoutMode);
			List<Control> listControlsDynamic=new List<Control>();
			for(int i=0;i<listSheetFieldDefs.Count;i++){
				if(!TryGetControlForField(listSheetFieldDefs[i],dictionaryControls,out Control control)) {
					continue;//For example, HQ only controls when running from customer location.
				}
				//The height above and below calculations assumes the static controls are all entire at the top or bottom of the this dynamic control.
				int heightAbove=GetStaticControlsHeightAbove(LayoutManager.Scale(listSheetFieldDefs[i].YPos));
				int heightBelow=GetStaticControlsHeightBelow(LayoutManager.Scale(listSheetFieldDefs[i].YPos));
				control.MinimumSize=new Size(0,0);//Do not limit min size
				control.MaximumSize=new Size(0,0);//Do not limit max size
				//oddly, the line below sets minimum sizes:
				SheetUtil.SetControlSizeAndAnchors(listSheetFieldDefs[i],control,_controlHosting,LayoutManager.ScaleMy(),fillHeightAdj:heightAbove+heightBelow);
				control.Location=new Point(LayoutManager.Scale(listSheetFieldDefs[i].XPos),LayoutManager.Scale(listSheetFieldDefs[i].YPos)+heightAbove);
				control.Visible=true;
				listControlsDynamic.Add(control);
			}
			RefreshGridVerticalSpace(sheetFieldLayoutMode,dictionaryControls,listSheetFieldDefs);
			RefreshGridHorizontalSpace(sheetFieldLayoutMode,dictionaryControls);
			_listControlsStatic.ForEach(x => x.BringToFront());
			_controlHosting.Controls.OfType<Control>()
				.Where(x => !listControlsDynamic.Contains(x) && !_listControlsStatic.Contains(x))
				.ForEach(x => x.Visible=false);
			if(isUserSelection) {
				UpdateChartLayoutUserPref();
			}
		}

		///<summary>Returns list of SheetFieldDefs for the given sheetFieldLayoutMode. Orders the list exactly in the same order that it is drawn.</summary>
		public List<SheetFieldDef> GetPertinentSheetFieldDefs(SheetFieldLayoutMode sheetFieldLayoutMode) {
			return  _sheetDefDynamicLayoutCur.SheetFieldDefs.Where(x => x.LayoutMode==sheetFieldLayoutMode)
				.OrderByDescending(x => x.FieldType==SheetFieldType.Grid)//Grids First
				.ThenBy(x => x.FieldName.Contains("Button"))//Buttons last, always drawn on top.
				.ToList();
		}

		///<summary>Updates or inserts (if necessary) the user's preference dictating which chart layout sheet def the user last viewed.
		///Should only be called when a user selects a specific layout.</summary>
		private void UpdateChartLayoutUserPref() {
			UserOdPref userPref=UserOdPrefs.GetFirstOrNewByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.DynamicChartLayout);
			userPref.Fkey=_sheetDefDynamicLayoutCur.SheetDefNum;
			if(!PrefC.HasClinicsEnabled || Clinics.ClinicNum==0 
				|| !ClinicPrefs.TryGetLong(PrefName.SheetsDefaultChartModule,Clinics.ClinicNum,out long defaultSheetDefNum)) 
			{
				defaultSheetDefNum=PrefC.GetLong(PrefName.SheetsDefaultChartModule);
			}
			if(userPref.Fkey==defaultSheetDefNum) {
				if(!userPref.IsNew) {
					UserOdPrefs.Delete(userPref.UserOdPrefNum);//Delete old entry, this will cause user to view any newly selected practice or clinic defaults.
				}
				//User selected the practice or clinic default, flag so that this user continues to get the appropriate default.
				return;
			}
			UserOdPrefs.Upsert(userPref);
		}

		///<summary>Once all controls are set in their initial place this is called to adjust any controls which might be overlapping on the X axis
		///due to a grid having GrowthBehaviorEnum.FillDownFitColumns.
		///Since the width of the grid is determined by the sum of the column widths we might need to move some controls to the right.</summary>
		public void RefreshGridHorizontalSpace(SheetFieldLayoutMode sheetFieldLayoutModeCur,Dictionary<string,Control> dictionaryControls) {
			List<SheetFieldDef> listFillDownDefs=_sheetDefDynamicLayoutCur.SheetFieldDefs
				.FindAll(x => x.LayoutMode==sheetFieldLayoutModeCur && x.GrowthBehavior==GrowthBehaviorEnum.FillDownFitColumns);
			List<Control> listControlsDynamic=new List<Control>();
			List<SheetFieldDef> listDefsForLayoutMode=_sheetDefDynamicLayoutCur.SheetFieldDefs.FindAll(x=>x.LayoutMode==sheetFieldLayoutModeCur).ToList();
			for(int i=0;i<listDefsForLayoutMode.Count;i++) {
				if(TryGetControlForField(listDefsForLayoutMode[i],dictionaryControls,out Control control)) {
					listControlsDynamic.Add(control);
				}
			}
			//There is a grid that might be overlaping controls to the right of the grid since the width of the grid is defined by its columns.
			//We only expect there to be one field with FillDownFitColumns growth behavior, however we loop in case more are added later.
			for(int i=0;i<listFillDownDefs.Count;i++){
				if(TryGetControlForField(listFillDownDefs[i],dictionaryControls,out Control control)) {
					int gridMaxX=control.Location.X+control.Width;
					List<Control> listControlsIntersecting=listControlsDynamic.FindAll(x => x!=control && x.Bounds.IntersectsWith(control.Bounds));
					for(int j=0;j<listControlsIntersecting.Count;j++){
						int diff=(gridMaxX-listControlsIntersecting[j].Location.X)+1;
						listControlsIntersecting[j].Location=new Point(listControlsIntersecting[j].Location.X+diff,listControlsIntersecting[j].Location.Y);
					}
				}
			}
		}

		///<summary>Once all controls are set in their initial place this is called to adjust any controls which might be overlapping on the Y axis
		///due to a grid having GrowthBehaviorEnum.FillDownFitColumns or GrowthBehaviorEnum.FillDown.</summary>
		public void RefreshGridVerticalSpace(SheetFieldLayoutMode sheetFieldLayoutModeCur,Dictionary<string,Control> dictControls,List<SheetFieldDef> listSheetFieldDefs) {
			List<SheetFieldDef> listFillDownDefs=_sheetDefDynamicLayoutCur.SheetFieldDefs
				.FindAll(x => x.LayoutMode==sheetFieldLayoutModeCur && ListTools.In(x.GrowthBehavior,
				GrowthBehaviorEnum.FillDownFitColumns,GrowthBehaviorEnum.FillDown))
				.OrderBy(x => x.YPos)
				.ToList();
			List<Control> listControlsDynamic=new List<Control>();
			List<SheetFieldDef> listSheetFieldDefsForLayout=_sheetDefDynamicLayoutCur.SheetFieldDefs.FindAll(x => x.LayoutMode==sheetFieldLayoutModeCur).ToList();
			for(int i=0;i<listSheetFieldDefsForLayout.Count;i++ ) {
				if(TryGetControlForField(listSheetFieldDefsForLayout[i],dictControls,out Control control)) {
					listControlsDynamic.Add(control);
				}
			}
			//tabProc might be overlaping controls vertically below because tabProcs has two different heights depending on state.
			//We only expect there to be one field with FillDownFitColumns growth behavior, however we loop in case more are added later.
			for(int i=0;i<listFillDownDefs.Count;i++){
				if(TryGetControlForField(listFillDownDefs[i],dictControls,out Control control)) {
					//Do not need to use GetHeightAbove(), all controls in listAboveControls have the above offset accounted for in their possitions.
					int heightAbove=0;
					int heightBelow=0;
					bool anchorToBottom=true;
					switch(listFillDownDefs[i].GrowthBehavior) {
						case GrowthBehaviorEnum.FillDownFitColumns:
							int minY=GetStaticControlsHeightAbove(LayoutManager.Scale(listFillDownDefs[i].YPos));//Uses y position relative to the sheet def.
							AdjustGridYPosition(listFillDownDefs[i],control,listControlsDynamic,minY);
							heightBelow=GetStaticControlsHeightBelow(LayoutManager.Scale(listFillDownDefs[i].YPos));
							break;
						case GrowthBehaviorEnum.FillDown:
							//Since we update the location below this maintains the correct Y locations set in InitLayoutForSheetDef(...)
							//Also important that we set this to the real Y value relative to _controlHosting so that GetSheetFieldsHeightBelow(...) can work correctly.
							heightAbove=GetStaticControlsHeightAbove(LayoutManager.Scale(listFillDownDefs[i].YPos));
							heightBelow=GetStaticControlsHeightBelow(LayoutManager.Scale(listFillDownDefs[i].YPos)+heightAbove);
							heightBelow+=GetSheetFieldsHeightBelow(listFillDownDefs[i],heightAbove,listSheetFieldDefs,dictControls,out anchorToBottom);
							break;
					}
					SheetUtil.SetControlSizeAndAnchors(listFillDownDefs[i],control,_controlHosting,LayoutManager.ScaleMy(),fillHeightAdj:heightAbove+heightBelow,anchorToBottom);
					control.Location=new Point(LayoutManager.Scale(listFillDownDefs[i].XPos),LayoutManager.Scale(listFillDownDefs[i].YPos)+heightAbove);
				}
			}
		}

		/// <summary>Helper function that adjusts the Y position of a grid with growth behavior based on controls that are above it. Takes in an optional existing-adjustment parameter</summary>
		private void AdjustGridYPosition(SheetFieldDef sheetFieldDef, Control fieldControl, List<Control> listDynamicControls,int adjustment=0) {
			List<Control> listControlsAbove=listDynamicControls.FindAll(x => IsControlAbove(fieldControl,x));
			if(listControlsAbove.Count>0) {
				adjustment=listControlsAbove.Max(x => x.Bottom)+1; //Move the grid underneath any existing controls above it
			}
			sheetFieldDef.YPos=(int)LayoutManager.UnscaleF(adjustment);
		}

		///<summary>Attempts to return a corresponding control for the given fieldDef from dictControls.
		///Returns false if control is null.</summary>
		private bool TryGetControlForField(SheetFieldDef sheetFieldDef,Dictionary<string,Control> dictionaryControls,out Control control) {
			return dictionaryControls.TryGetValue(sheetFieldDef.FieldName,out control);
		}

		/// <summary>Returns the height beneath a given SheetFieldDef</summary>
		private int GetSheetFieldsHeightBelow(SheetFieldDef fieldDef,int heightOffset,List<SheetFieldDef> listSheetFieldDefs,Dictionary<string,Control> dictControls,out bool anchorToBottom) {
			anchorToBottom=false;
			List<SheetFieldDef> listSheetFieldDefsToCheck=listSheetFieldDefs.FindAll(x=>!x.FieldName.Contains("Button"));
			List<Control> listBelowSheetFieldDefs=new List<Control>();
			if(TryGetControlForField(fieldDef,dictControls,out Control controlOrigin)) {
				for(int i=0;i<listSheetFieldDefsToCheck.Count;i++) {
					if(TryGetControlForField(listSheetFieldDefsToCheck[i],dictControls,out Control controlTarget) && IsControlBelow(controlOrigin,controlTarget)) {
						listBelowSheetFieldDefs.Add(controlTarget);
					}
				}
			}
			if(listBelowSheetFieldDefs.Count==0) { //If there are no controls under the initially-provided SheetFieldDef
				anchorToBottom=true;
				return 0;
			}
			return Math.Max(0,(_controlHosting.Height-GetStaticControlsHeightBelow(LayoutManager.Scale(fieldDef.YPos)+heightOffset))-(listBelowSheetFieldDefs.Min(x => x.Top)));
		}

		///<summary>The height above and below calculations assumes the staic controls are all entire at the top or bottom of the this dynamic control.</summary>
		private int GetStaticControlsHeightAbove(int yPos) {
			int heightAbove=0;
			List<Control> listStaticControlsAbove=_listControlsStatic.FindAll(x => x.Visible && x.Location.Y<=yPos);
			if(listStaticControlsAbove.Count>0) {
				heightAbove=listStaticControlsAbove.Sum(x => x.Height);//ex toolbarmain
			}
			return heightAbove;
		}

		///<summary>The height above and below calculations assumes the staic controls are all entire at the top or bottom of the this dynamic control.</summary>
		private int GetStaticControlsHeightBelow(int yPos) {
			int heightBelow=0;
			List<Control> listStaticControlsBelow=_listControlsStatic.FindAll(x => x.Visible && x.Location.Y>=yPos);
			if(listStaticControlsBelow.Count>0) {
				heightBelow=listStaticControlsBelow.Sum(x => x.Height);//ex image panel
			}
			return heightBelow;
		}

		///<summary>Does not check for vertical overlap.</summary>
		private bool IsControlAbove(Control control,Control controlOther) {
			return (controlOther.Top<control.Top
				&& (controlOther.Left.Between(control.Left,control.Right)
				|| controlOther.Right.Between(control.Left,control.Right)
				|| control.Left.Between(controlOther.Left,controlOther.Right)
				|| control.Right.Between(controlOther.Left,controlOther.Right))
			);
		}

		///<summary>Does not check for vertical overlap.</summary>
		private bool IsControlBelow(Control controlOrigin,Control controlTarget) {
			return (controlTarget.Top>controlOrigin.Top//0,0 origin is top left
				&& (controlTarget.Left.Between(controlOrigin.Left,controlOrigin.Right)
				|| controlTarget.Right.Between(controlOrigin.Left,controlOrigin.Right)
				|| controlOrigin.Right.Between(controlTarget.Left,controlTarget.Right)
				|| controlOrigin.Right.Between(controlTarget.Left,controlTarget.Right))
			);
		}

		public void UserLogOffCommited(){
			_hasUserLoggedOff=true;
		}

	}
}
