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
			SetFilterControlsAndAction(() => FillGridCustom(doRefreshCache:false),comboUserGroup);
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
			_listGroupPermissions=GroupPermissions.GetForUserGroups(_listUserGroups.Select(x => x.UserGroupNum).ToList(),Permissions.DashboardWidget);
			_listGroupPermissionsOld=_listGroupPermissions.Select(x => x.Copy()).ToList();
		}

		private void FillGridInternal() {
			//Get the list of SheetDefs that are for Dashboards.
			SheetDef[] sheetDefs=Enum.GetValues(typeof(SheetInternalType)).OfType<SheetInternalType>()
				.Where(x => EnumTools.GetAttributeOrDefault<SheetInternalAttribute>(x).DoShowInDashboardSetup)
				.Select(x => SheetsInternal.GetSheetDef(x))
				.ToArray();
			FillGrid(gridInternal,true,sheetDefs);
		}

		private void FillGridCustom(bool isSelectionMaintained=true,bool doRefreshCache=true) {
			if(doRefreshCache) {
				SheetDefs.RefreshCache();
				SheetFieldDefs.RefreshCache();
			}
			FillGrid(gridCustom,isSelectionMaintained,SheetDefs.GetCustomForType(SheetTypeEnum.PatientDashboardWidget).ToArray());
		}

		private void FillGrid(GridOD grid,bool isSelectionMaintained,params SheetDef[] arrDashboardSheetDefs) {
			List<SheetDef> listSelectedDashboards=grid.SelectedTags<SheetDef>();
			grid.BeginUpdate();
			grid.ListGridColumns.Clear();
			grid.ListGridColumns.Add(new GridColumn("Name",100,HorizontalAlignment.Left){ IsWidthDynamic=true});
			if(grid==gridCustom) {
				grid.ListGridColumns.Add(new GridColumn("Allowed",50,HorizontalAlignment.Center));
				_colAllowed=gridCustom.ListGridColumns.Count-1;//Dynamically determines the 'Allowed' column index in case we add others later.
			}
			grid.ListGridRows.Clear();
			foreach(SheetDef sheetDefWidget in arrDashboardSheetDefs) {
				GridRow row=new GridRow();
				row.Cells.Add(sheetDefWidget.Description);
				if(grid==gridCustom) {
					bool isAllowed=IsUserGroupAllowed(sheetDefWidget,comboUserGroup.GetSelected<UserGroup>());
					row.Cells.Add(isAllowed ? "X":"");
				}
				row.Tag=sheetDefWidget;
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
			if(isSelectionMaintained) {
				foreach(SheetDef sheetDef in listSelectedDashboards) {
					SelectDashboardDef(sheetDef);
				}
			}
		}

		///<summary>Determines if there is a GroupPermission for any of the arrayUserGroups that matches the sheetDashboard.</summary>
		private bool IsUserGroupAllowed(SheetDef sheetDefWidget,UserGroup userGroup) {
			if(_listGroupPermissions.Any(x => x.UserGroupNum==userGroup.UserGroupNum && x.FKey==sheetDefWidget.SheetDefNum)) {
				return true;
			}
			return false;
		}

		private void ToggleDashboardPermission(SheetDef sheetDefWidget,params UserGroup[] arrUserGroups) {
			foreach(UserGroup userGroup in arrUserGroups) {
				ToggleDashboardPermission(userGroup,sheetDefWidget);
			}
		}

		private bool ToggleDashboardPermission(UserGroup userGroup,SheetDef sheetDefWidget) {
			GroupPermission selectedGroupPermission=_listGroupPermissions
				.FirstOrDefault(x => x.UserGroupNum==userGroup.UserGroupNum && x.FKey==sheetDefWidget.SheetDefNum);
			if(selectedGroupPermission==null) {
				GroupPermission groupPermission=new GroupPermission() {
					NewerDate=DateTime.MinValue,
					NewerDays=0,
					PermType=Permissions.DashboardWidget,
					UserGroupNum=userGroup.UserGroupNum,
					FKey=sheetDefWidget.SheetDefNum,
				};
				_listGroupPermissions.Add(groupPermission);
				return true;
			}
			else {
				_listGroupPermissions.Remove(selectedGroupPermission);//Clear permission to this dashboard for this userGroup.
				return false;
			}
		}

		private void SetDashboardPermission(UserGroup userGroup,params SheetDef[] arraySheetDefWidgets) {
			foreach(SheetDef sheetDefWidget in arraySheetDefWidgets) {
				GroupPermission selectedGroupPermission=_listGroupPermissions
					.FirstOrDefault(x => x.UserGroupNum==userGroup.UserGroupNum && x.FKey==sheetDefWidget.SheetDefNum);
				if(selectedGroupPermission==null) {
					GroupPermission groupPermission=new GroupPermission() {
						NewerDate=DateTime.MinValue,
						NewerDays=0,
						PermType=Permissions.DashboardWidget,
						UserGroupNum=userGroup.UserGroupNum,
						FKey=sheetDefWidget.SheetDefNum,
					};
					_listGroupPermissions.Add(groupPermission);
				}
			}
		}

		///<summary>Opens a SheetDefEdit window and returns the SheetDef.SheetDefNum.  Returns -1 if user cancels or deletes the Dashboard.</summary>
		private long EditWidget(SheetDef sheetDefWidget=null) {
			if(sheetDefWidget==null) {
				sheetDefWidget=new SheetDef() {
					SheetType=SheetTypeEnum.PatientDashboardWidget,
					FontName="Microsoft Sans Serif",
					FontSize=9,
					Height=800,
					Width=400,
				};
				using FormSheetDef FormSD=new FormSheetDef();
				FormSD.SheetDefCur=sheetDefWidget;
				FormSD.ShowDialog();
				if(FormSD.DialogResult!=DialogResult.OK) {
					return -1;
				}
				sheetDefWidget.SheetFieldDefs=new List<SheetFieldDef>();
				sheetDefWidget.IsNew=true;
			}
			SheetDefs.GetFieldsAndParameters(sheetDefWidget);
			FormHelpBrowser.InitializeIfNull();
			Dpi.SetUnaware();
			using FormSheetDefEdit FormS=new FormSheetDefEdit(sheetDefWidget);
			Dpi.SetAware();
			if(FormS.ShowDialog()==DialogResult.OK) {
				DataValid.SetInvalid(InvalidType.Sheets);
				return sheetDefWidget.SheetDefNum;
			}
			return -1;
		}

		private void gridInternal_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridInternal.SelectedGridRows.Count==0) {
				return;
			}
			FormHelpBrowser.InitializeIfNull();
			Dpi.SetUnaware();
			using FormSheetDefEdit FormS=new FormSheetDefEdit(gridInternal.SelectedTag<SheetDef>());
			Dpi.SetAware();
			FormS.IsInternal=true;
			FormS.ShowDialog();
		}

		private void gridCustom_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridCustom.SelectedCell.X==-1 || gridCustom.SelectedCell.Y==-1) {//Invalid cell.
				return;
			}
			if(gridCustom.SelectedCell.X==_colAllowed) {//Do not open the edit window when double clicking the 'Allowed' column.
				return;
			}
			EditWidget(gridCustom.SelectedTag<SheetDef>());
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
			bool isAllowed=ToggleDashboardPermission(comboUserGroup.GetSelected<UserGroup>(),gridCustom.SelectedTag<SheetDef>());
			gridCustom.BeginUpdate();
			gridCustom.SelectedGridRows[0].Cells[_colAllowed].Text=(isAllowed ? "X":"");
			gridCustom.EndUpdate();
		}

		private void ButAdd_Click(object sender, EventArgs e){
			long sheetDefWidgetNum=EditWidget();//Adding a new Dashboard Widget.
			if(sheetDefWidgetNum==-1) {
				return;
			}
			SheetDef sheetDef=SheetDefs.GetWhere(x => x.SheetDefNum==sheetDefWidgetNum).FirstOrDefault();
			if(sheetDef!=null) {
				ToggleDashboardPermission(sheetDef,_listUserGroups.ToArray());
			}
			FillGridCustom(false);
			SelectDashboardDef(sheetDef);
		}

		private void butSetAll_Click(object sender,EventArgs e) {
			SetDashboardPermission(comboUserGroup.GetSelected<UserGroup>(),gridCustom.GetTags<SheetDef>().ToArray());
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
			ToggleDashboardPermission(sheetDef,_listUserGroups.ToArray());
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
			ToggleDashboardPermission(sheetDef,_listUserGroups.ToArray());
			FillGridCustom(false);
			SelectDashboardDef(sheetDef);
		}

		///<summary>Selects the Cell corresponding to sheetDef.  Important to only select the Cell, as opposed to the Row, since in OneCell mode.
		///</summary>
		private void SelectDashboardDef(SheetDef sheetDef) {
			if(sheetDef==null) {
				return;
			}
			gridCustom.SetSelected(new Point(0,gridCustom.GetTags<SheetDef>().FindIndex(x => x.SheetDefNum==sheetDef.SheetDefNum)));
		}

		private void butTools_Click(object sender,EventArgs e) {
			using FormSheetTools formSheetTools=new FormSheetTools(true);
			formSheetTools.ShowDialog();
			if(formSheetTools.HasSheetsChanged) {
				SheetDefs.RefreshCache();
				SheetFieldDefs.RefreshCache();
				SheetDef sheetDefImported=SheetDefs.GetFirstOrDefault(x => x.SheetDefNum==formSheetTools.ImportedSheetDefNum);
				ToggleDashboardPermission(sheetDefImported,_listUserGroups.ToArray());
				FillGridCustom(false,doRefreshCache:false);
				SelectDashboardDef(sheetDefImported);
			}
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