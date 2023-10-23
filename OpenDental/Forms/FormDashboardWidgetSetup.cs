using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormDashboardWidgetSetup:FormODBase {
		private List<UserGroup> _listUserGroups;
		///<summary>A list of all DashboardWidget permissions. Safe to manipulate. Used to synchronize when leaving the window.</summary>
		private List<GroupPermission> _listGroupPermissions;
		///<summary>A list of all DashboardWidget permissions filled when the window loaded. Not safe to manipulate. Used to synchronize when leaving the window.</summary>
		private List<GroupPermission> _listGroupPermissionsOld;
		///<summary>The index of the Allowed column in gridCustom.</summary>
		private int _colAllowed;
		/// <summary>Set to true when a change is made to a sheetDef.</summary>
		private bool _hasChanged;

		public FormDashboardWidgetSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		
		private void FormDashboardSetup_Load(object sender,EventArgs e) {
			InitializeOnStartup();
			FillGridInternal();
			FillGridCustom();
			Action actionFillGridCustom=() => FillGridCustom(doRefreshCache:false);
			SetFilterControlsAndAction(actionFillGridCustom,comboUserGroup);
		}

		private void InitializeOnStartup() {
			_listUserGroups=UserGroups.GetList();
			comboUserGroup.Items.AddList(_listUserGroups,x => x.Description);
			UserGroup userGroup=UserGroups.GetForUser(Security.CurUser.UserNum,false).FirstOrDefault();
			if(userGroup!=null) {
				comboUserGroup.SetSelectedKey<UserGroup>(userGroup.UserGroupNum,x => x.UserGroupNum);
			}
			if(comboUserGroup.SelectedIndex==-1) {
				comboUserGroup.SelectedIndex=0;
			}
			List<long> listUserGroupNums=_listUserGroups.Select(x => x.UserGroupNum).ToList();
			_listGroupPermissions=GroupPermissions.GetForUserGroups(listUserGroupNums,EnumPermType.DashboardWidget);
			_listGroupPermissionsOld=_listGroupPermissions.Select(x => x.Copy()).ToList();
		}

		private void FillGridInternal() {
			//Get the list of SheetDefs that are for Dashboards.
			List<SheetDef> listSheetDefsWidgets=Enum.GetValues(typeof(SheetInternalType)).OfType<SheetInternalType>()
				.Where(x => EnumTools.GetAttributeOrDefault<SheetInternalAttribute>(x).DoShowInDashboardSetup)
				.Select(x => SheetsInternal.GetSheetDef(x))
				.ToList();
			FillGrid(gridInternal,isSelectionMaintained:true,listSheetDefsWidgets);
		}

		private void FillGridCustom(bool isSelectionMaintained=true,bool doRefreshCache=true) {
			if(doRefreshCache) {
				SheetDefs.RefreshCache();
				SheetFieldDefs.RefreshCache();
			}
			List<SheetDef> listSheetDefsWidgets=SheetDefs.GetCustomForType(SheetTypeEnum.PatientDashboardWidget);
			FillGrid(gridCustom,isSelectionMaintained,listSheetDefsWidgets);
		}

		private void FillGrid(GridOD grid,bool isSelectionMaintained,List<SheetDef> listSheetDefsWidgets) {
			List<SheetDef> listSheetDefsSelected=grid.SelectedTags<SheetDef>();
			grid.BeginUpdate();
			grid.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Name"),100,HorizontalAlignment.Left){ IsWidthDynamic=true };
			grid.Columns.Add(col);
			if(grid==gridCustom) {
				col=new GridColumn(Lan.g(this,"Allowed"),50,HorizontalAlignment.Center);
				grid.Columns.Add(col);
				_colAllowed=gridCustom.Columns.Count-1;//Dynamically determines the 'Allowed' column index in case we add others later.
			}
			grid.ListGridRows.Clear();
			for(int i=0;i<listSheetDefsWidgets.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(listSheetDefsWidgets[i].Description);
				if(grid==gridCustom) {
					UserGroup userGroupSelected=comboUserGroup.GetSelected<UserGroup>();
					if(_listGroupPermissions.Any(x => x.UserGroupNum==userGroupSelected.UserGroupNum && x.FKey==listSheetDefsWidgets[i].SheetDefNum) 
						|| HasDashboardWidgetPermissionAll(userGroupNum:userGroupSelected.UserGroupNum))
					{
						row.Cells.Add("X");
					}
					else {
						row.Cells.Add("");
					}
				}
				row.Tag=listSheetDefsWidgets[i];
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
			if(!isSelectionMaintained) {
				return;
			}
			for(int i=0;i<listSheetDefsSelected.Count;i++) {
				SelectDashboardDef(listSheetDefsSelected[i]);
			}
		}

		///<summary>Returns true if the currently selected user group has permission to all DashboardWidgets. Optionally pass in a specific user group.
		///This method can manipulate the 'new' group permission list in order to correct the GroupPermission cache when necessary.</summary>
		private bool HasDashboardWidgetPermissionAll(long userGroupNum=0) {
			if(userGroupNum<=0) {
				userGroupNum=comboUserGroup.GetSelected<UserGroup>().UserGroupNum;
			}
			List<GroupPermission> listGroupPermissions=_listGroupPermissions.FindAll(x => x.UserGroupNum==userGroupNum);
			GroupPermission groupPermissionAll=listGroupPermissions.FirstOrDefault(x => x.FKey==0);
			if(groupPermissionAll!=null) {
				//Correct the group permission cache if needed by removing all permissions related to this user group and making sure that only one 'zero FKey' row is present.
				_listGroupPermissions.RemoveAll(x => x.UserGroupNum==userGroupNum);
				_listGroupPermissions.Add(groupPermissionAll);
				return true;
			}
			//There are no explicit 'zero FKey' permissions. Check to see if the user group does in fact have permission to every single widget available.
			List<SheetDef> listSheetDefsWidgets=SheetDefs.GetCustomForType(SheetTypeEnum.PatientDashboardWidget);
			int countExplicitGroupPermissions=_listGroupPermissions.Count(x => x.PermType==EnumPermType.DashboardWidget && x.UserGroupNum==userGroupNum && x.FKey!=0);
			if(listSheetDefsWidgets.Count==countExplicitGroupPermissions) {
				//Correct the group permission cache by removing all permissions related to this user group and making sure that only one 'zero FKey' row is present.
				GiveUserGroupPermissionAll(userGroupNum);
				return true;
			}
			return false;
		}

		///<summary>Removes all widget specific permissions and gives the user group a single 'zero FKey' permission.</summary>
		private void GiveUserGroupPermissionAll(long userGroupNum) {
			_listGroupPermissions.RemoveAll(x => x.UserGroupNum==userGroupNum && x.FKey!=0);
			if(!_listGroupPermissions.Exists(x => x.UserGroupNum==userGroupNum && x.FKey==0)) {
				GroupPermission groupPermission=new GroupPermission();
				groupPermission.NewerDate=DateTime.MinValue;
				groupPermission.NewerDays=0;
				groupPermission.PermType=EnumPermType.DashboardWidget;
				groupPermission.UserGroupNum=userGroupNum;
				groupPermission.FKey=0;
				_listGroupPermissions.Add(groupPermission);
			}
		}

		///<summary>Toggles the widget specific permission for the user group passed in. Returns a boolean indicator that represents the new state for the permission.</summary>
		private bool ToggleDashboardPermission(UserGroup userGroup,SheetDef sheetDef) {
			GroupPermission groupPermission;
			List<SheetDef> listSheetDefsWidgets=gridCustom.GetTags<SheetDef>();
			//If user has 'all' permission remove it and create explicit list of all permissions.
			bool hasAll=_listGroupPermissions.Any(x => x.UserGroupNum==userGroup.UserGroupNum && x.FKey==0);
			if(hasAll) {
				_listGroupPermissions.RemoveAll(x => x.UserGroupNum==userGroup.UserGroupNum);
				//Add specific permission to all avaliable widgets.
				for(int i=0;i < listSheetDefsWidgets.Count;i++) {
					groupPermission=new GroupPermission();
					groupPermission.NewerDate=DateTime.MinValue;
					groupPermission.NewerDays=0;
					groupPermission.PermType=EnumPermType.DashboardWidget;
					groupPermission.UserGroupNum=userGroup.UserGroupNum;
					groupPermission.FKey=listSheetDefsWidgets[i].SheetDefNum;
					_listGroupPermissions.Add(groupPermission);
				}
			}
			groupPermission=_listGroupPermissions.FirstOrDefault(x => x.UserGroupNum==userGroup.UserGroupNum && x.FKey==sheetDef.SheetDefNum);
			if(groupPermission!=null) {
				_listGroupPermissions.Remove(groupPermission);//Clear permission to this dashboard for this userGroup.
				return false;
			}
			//No GroupPermission found for userGroup matching any SheetDef, create a new one.
			groupPermission=new GroupPermission();
			groupPermission.NewerDate=DateTime.MinValue;
			groupPermission.NewerDays=0;
			groupPermission.PermType=EnumPermType.DashboardWidget;
			groupPermission.UserGroupNum=userGroup.UserGroupNum;
			groupPermission.FKey=sheetDef.SheetDefNum;
			_listGroupPermissions.Add(groupPermission);
			//Invoke a cleanup method just in case the user just gave this user group permission to every single widget available (which should be a single 'zero FKey' row in the db).
			HasDashboardWidgetPermissionAll(userGroupNum:userGroup.UserGroupNum);
			return true;
		}

		///<summary>Opens a SheetDefEdit window and returns the SheetDef.SheetDefNum.  Returns -1 if user cancels or deletes the Dashboard.</summary>
		private long EditWidget(SheetDef sheetDef=null) {
			if(sheetDef==null) {
				sheetDef=new SheetDef();
				sheetDef.SheetType=SheetTypeEnum.PatientDashboardWidget;
				sheetDef.FontName="Microsoft Sans Serif";
				sheetDef.FontSize=9;
				sheetDef.Height=800;
				sheetDef.Width=400;
				using FormSheetDef formSheetDef=new FormSheetDef();
				formSheetDef.SheetDefCur=sheetDef;
				formSheetDef.ShowDialog();
				if(formSheetDef.DialogResult!=DialogResult.OK) {
					return -1;
				}
				sheetDef.SheetFieldDefs=new List<SheetFieldDef>();
				sheetDef.IsNew=true;
			}
			SheetDefs.GetFieldsAndParameters(sheetDef);
			FrmHelpBrowser.InitializeIfNull();
			using FormSheetDefEdit formSheetDefEdit=new FormSheetDefEdit(sheetDef);
			if(formSheetDefEdit.ShowDialog()==DialogResult.OK) {
				DataValid.SetInvalid(InvalidType.Sheets);
			}
			if(SheetDefs.GetSheetDef(sheetDef.SheetDefNum,hasExceptions:false)==null) {
				return -1;//sheet was deleted
			}
			return sheetDef.SheetDefNum;
		}

		private void gridInternal_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridInternal.SelectedGridRows.Count==0) {
				return;
			}
			FrmHelpBrowser.InitializeIfNull();
			SheetDef sheetDefSelected=gridInternal.SelectedTag<SheetDef>();
			using FormSheetDefEdit formSheetDefEdit=new FormSheetDefEdit(sheetDefSelected);
			formSheetDefEdit.IsInternal=true;
			formSheetDefEdit.ShowDialog();
		}

		private void gridCustom_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridCustom.SelectedCell.X==-1 || gridCustom.SelectedCell.Y==-1) {//Invalid cell.
				return;
			}
			if(gridCustom.SelectedCell.X==_colAllowed) {//Do not open the edit window when double clicking the 'Allowed' column.
				return;
			}
			SheetDef sheetDefSelected=gridCustom.SelectedTag<SheetDef>();
			if(EditWidget(sheetDefSelected)==-1) {
				//The sheetDef was deleted. Remove the permission to the sheetdef if one exists.
				_listGroupPermissions.RemoveAll(x => x.FKey == sheetDefSelected.SheetDefNum);
			}
			_hasChanged=true;
			FillGridCustom();
		}

		private void gridCustom_CellClick(object sender,ODGridClickEventArgs e) {
			if(gridCustom.SelectedCell.X==-1 || gridCustom.SelectedCell.Y==-1) {//Invalid cell.
				return;
			}
			if(gridCustom.SelectedCell.X!=_colAllowed) {//Not setting/unsetting permission.
				return;
			}
			if(!Security.IsAuthorized(EnumPermType.SecurityAdmin)) {
				return;
			}
			UserGroup userGroupSelected=comboUserGroup.GetSelected<UserGroup>();
			SheetDef sheetDefSelected=gridCustom.SelectedTag<SheetDef>();
			string strIsAllowed=ToggleDashboardPermission(userGroupSelected,sheetDefSelected)?"X":"";
			gridCustom.BeginUpdate();
			gridCustom.SelectedGridRows[0].Cells[_colAllowed].Text=strIsAllowed;
			gridCustom.EndUpdate();
		}

		private void ButAdd_Click(object sender,EventArgs e){
			long sheetDefWidgetNum=EditWidget();//Adding a new Dashboard Widget.
			if(sheetDefWidgetNum==-1) {
				return;
			}
			SheetDef sheetDef=SheetDefs.GetWhere(x => x.SheetDefNum==sheetDefWidgetNum).FirstOrDefault();
			_hasChanged=true;
			if(sheetDef!=null) {
				ToggleDashboardPermission(comboUserGroup.GetSelected<UserGroup>(),sheetDef);
			}
			FillGridCustom(false);
			SelectDashboardDef(sheetDef);
		}

		private void butSetAll_Click(object sender,EventArgs e) {
			UserGroup userGroupSelected=comboUserGroup.GetSelected<UserGroup>();
			if(userGroupSelected==null) {//Shouldn't happen.
				return;
			}
			GiveUserGroupPermissionAll(userGroupSelected.UserGroupNum);
			FillGridCustom(doRefreshCache:false);
		}

		private void butCopy_Click(object sender,EventArgs e) {
			if(gridInternal.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an internal sheet first.");
				return;
			}
			SheetDef sheetDef=gridInternal.SelectedTag<SheetDef>().Copy(); 
			sheetDef.IsNew=true;
			SheetDefs.InsertOrUpdate(sheetDef);
			SheetDefs.RefreshCache();
			_hasChanged=true;
			ToggleDashboardPermission(comboUserGroup.GetSelected<UserGroup>(),sheetDef);
			FillGridCustom(false);
			gridInternal.SetAll(false);//Clear selection.
			SelectDashboardDef(sheetDef);
		}

		private void butDuplicate_Click(object sender,EventArgs e) {
			if(gridCustom.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"Please select a sheet from the custom list first.");
				return;
			}
			SheetDef sheetDef=gridCustom.SelectedTag<SheetDef>().Copy();
			sheetDef.Description=sheetDef.Description+"2";
			SheetDefs.GetFieldsAndParameters(sheetDef);
			sheetDef.IsNew=true;
			SheetDefs.InsertOrUpdate(sheetDef);
			SheetDefs.RefreshCache();
			_hasChanged=true;
			ToggleDashboardPermission(comboUserGroup.GetSelected<UserGroup>(),sheetDef);
			FillGridCustom(false);
			SelectDashboardDef(sheetDef);
		}

		///<summary>Selects the Cell corresponding to sheetDef. Important to only select the Cell, as opposed to the Row, since in OneCell mode.</summary>
		private void SelectDashboardDef(SheetDef sheetDef) {
			if(sheetDef==null) {
				return;
			}
			int sheetDefSelectedIndex=gridCustom.GetTags<SheetDef>().FindIndex(x => x.SheetDefNum==sheetDef.SheetDefNum);
			gridCustom.SetSelected(new Point(0,sheetDefSelectedIndex));
		}

		private void butTools_Click(object sender,EventArgs e) {
			using FormSheetTools formSheetTools=new FormSheetTools(true);
			formSheetTools.ShowDialog();
			if(!formSheetTools.HasSheetsChanged) {
				return;
			}
			SheetDefs.RefreshCache();
			SheetFieldDefs.RefreshCache();
			SheetDef sheetDefImported=SheetDefs.GetFirstOrDefault(x => x.SheetDefNum==formSheetTools.SheetDefNumImported);
			ToggleDashboardPermission(comboUserGroup.GetSelected<UserGroup>(),sheetDefImported);
			FillGridCustom(false,doRefreshCache:false);
			SelectDashboardDef(sheetDefImported);
		}
		
		private void butSave_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void FormDashboardWidgetSetup_FormClosing(object sender,FormClosingEventArgs e) {
			if(GroupPermissions.Sync(_listGroupPermissions,_listGroupPermissionsOld)) {
				DataValid.SetInvalid(InvalidType.Security);
			}
			if(_hasChanged) {
				DataValid.SetInvalid(InvalidType.Sheets);
			}
		}

	}
}