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
		private List<GroupPermission> _listGroupPermissions;
		private List<GroupPermission> _listGroupPermissionsOld;
		///<summary>The index of the Allowed column in gridCustom.</summary>
		private int _colAllowed;

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
			_listGroupPermissions=GroupPermissions.GetForUserGroups(listUserGroupNums,Permissions.DashboardWidget);
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
					bool isAllowed=GroupPermissionFindAllowed(userGroupSelected,listSheetDefsWidgets[i])!=null;
					row.Cells.Add(isAllowed?"X":"");
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

		///<summary>Returns the first GroupPermission that matches the passed in UserGroup and SheetDef. If there are no matches, returns null.</summary>
		private GroupPermission GroupPermissionFindAllowed(UserGroup userGroup,SheetDef sheetDef) {
			GroupPermission groupPermission=_listGroupPermissions
				.FirstOrDefault(x => x.UserGroupNum==userGroup.UserGroupNum && x.FKey==sheetDef.SheetDefNum);
			return groupPermission;
		}

		private void ToggleDashboardPermission(SheetDef sheetDef) {
			for(int i=0;i<_listUserGroups.Count;i++) {
				ToggleDashboardPermission(_listUserGroups[i],sheetDef);
			}
		}

		private bool ToggleDashboardPermission(UserGroup userGroup,SheetDef sheetDef) {
			GroupPermission groupPermission=GroupPermissionFindAllowed(userGroup,sheetDef);
			if(groupPermission!=null) {
				_listGroupPermissions.Remove(groupPermission);//Clear permission to this dashboard for this userGroup.
				return false;
			}
			//No GroupPermission found for userGroup matching any SheetDef, create a new one.
			groupPermission=new GroupPermission();
			groupPermission.NewerDate=DateTime.MinValue;
			groupPermission.NewerDays=0;
			groupPermission.PermType=Permissions.DashboardWidget;
			groupPermission.UserGroupNum=userGroup.UserGroupNum;
			groupPermission.FKey=sheetDef.SheetDefNum;
			_listGroupPermissions.Add(groupPermission);
			return true;
		}

		private void SetDashboardPermission(UserGroup userGroup,List<SheetDef> listSheetDefsWidgets) {
			for(int i=0;i<listSheetDefsWidgets.Count;i++) {
				GroupPermission groupPermission=GroupPermissionFindAllowed(userGroup,listSheetDefsWidgets[i]);
				if(groupPermission!=null) {
					continue;
				}
				//No GroupPermission found for userGroup matching any SheetDef, create a new one.
				groupPermission=new GroupPermission();
				groupPermission.NewerDate=DateTime.MinValue;
				groupPermission.NewerDays=0;
				groupPermission.PermType=Permissions.DashboardWidget;
				groupPermission.UserGroupNum=userGroup.UserGroupNum;
				groupPermission.FKey=listSheetDefsWidgets[i].SheetDefNum;
				_listGroupPermissions.Add(groupPermission);
			}
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
			FormHelpBrowser.InitializeIfNull();
			Dpi.SetUnaware();
			using FormSheetDefEdit formSheetDefEdit=new FormSheetDefEdit(sheetDef);
			Dpi.SetAware();
			if(formSheetDefEdit.ShowDialog()==DialogResult.OK) {
				DataValid.SetInvalid(InvalidType.Sheets);
				return sheetDef.SheetDefNum;
			}
			return -1;
		}

		private void gridInternal_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridInternal.SelectedGridRows.Count==0) {
				return;
			}
			FormHelpBrowser.InitializeIfNull();
			Dpi.SetUnaware();
			SheetDef sheetDefSelected=gridInternal.SelectedTag<SheetDef>();
			using FormSheetDefEdit formSheetDefEdit=new FormSheetDefEdit(sheetDefSelected);
			Dpi.SetAware();
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
			EditWidget(sheetDefSelected);
			FillGridCustom();
		}

		private void gridCustom_CellClick(object sender,ODGridClickEventArgs e) {
			if(gridCustom.SelectedCell.X==-1 || gridCustom.SelectedCell.Y==-1) {//Invalid cell.
				return;
			}
			if(gridCustom.SelectedCell.X!=_colAllowed) {//Not setting/unsetting permission.
				return;
			}
			if(!Security.IsAuthorized(Permissions.SecurityAdmin)) {
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
			if(sheetDef!=null) {
				ToggleDashboardPermission(sheetDef);
			}
			FillGridCustom(false);
			SelectDashboardDef(sheetDef);
		}

		private void butSetAll_Click(object sender,EventArgs e) {
			UserGroup userGroupSelected=comboUserGroup.GetSelected<UserGroup>();
			List<SheetDef> listSheetDefs=gridCustom.GetTags<SheetDef>();
			SetDashboardPermission(userGroupSelected,listSheetDefs);
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
			ToggleDashboardPermission(sheetDef);
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
			ToggleDashboardPermission(sheetDef);
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
			SheetDef sheetDefImported=SheetDefs.GetFirstOrDefault(x => x.SheetDefNum==formSheetTools.ImportedSheetDefNum);
			ToggleDashboardPermission(sheetDefImported);
			FillGridCustom(false,doRefreshCache:false);
			SelectDashboardDef(sheetDefImported);
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(GroupPermissions.Sync(_listGroupPermissions,_listGroupPermissionsOld)) {
				DataValid.SetInvalid(InvalidType.Security);
			}
			DialogResult=DialogResult.OK;
		}

    private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}