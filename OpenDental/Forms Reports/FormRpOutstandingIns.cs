using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using OpenDentBusiness;
using System.Collections;
using OpenDental.UI;
using CodeBase;
using OpenDental.Thinfinity;

namespace OpenDental {
	public partial class FormRpOutstandingIns:FormODBase {
		#region Private Variables
		private ContextMenu rightClickMenu=new ContextMenu();
		private RpOutstandingIns.PreauthOptions _preauthOption;
		private bool headingPrinted;
		private int pagesPrinted;
		private int headingPrintH;
		private decimal total;
		///<summary>List of non-hidden users with ClaimSentEdit permission.</summary>
		private List<Userod> _listClaimSentEditUsers=new List<Userod>();
		private List<ClaimTracking> _listNewClaimTrackings=new List<ClaimTracking>();
		private List<ClaimTracking> _listOldClaimTrackings=new List<ClaimTracking>();
		private bool _hasFormLoaded;
		#endregion

		public FormRpOutstandingIns() {
			InitializeComponent();
			InitializeLayoutManager();
			gridMain.ContextMenu=rightClickMenu;
			Lan.F(this);
		}

		private void FormRpOutIns_Load(object sender,EventArgs e) {
			SetFilterControlsAndAction(() => FillGrid(),
				textDaysOldMin,textDaysOldMax);
			_hasFormLoaded=false;
			SetDates(DateTime.MinValue,DateTime.Today.Date.AddDays(-30));
			if(PrefC.GetInt(PrefName.OutstandingInsReportDateFilterTab)==(int)RpOutstandingIns.DateFilterTab.DaysOld) {
				tabControlDate.SelectTab(tabDaysOld);
			}
			else {
				tabControlDate.SelectTab(tabDateRange);
			}
			FillProvs();
			FillDateFilterBy();
			_listClaimSentEditUsers=Userods.GetUsersByPermission(Permissions.ClaimSentEdit,false);
			FillUsers();
			_listOldClaimTrackings=ClaimTrackings.RefreshForUsers(ClaimTrackingType.ClaimUser,_listClaimSentEditUsers.Select(x => x.UserNum).ToList());
			_listNewClaimTrackings=_listOldClaimTrackings.Select(x => x.Copy()).ToList();
			if(!Security.IsAuthorized(Permissions.UpdateCustomTracking,true)) {
				buttonUpdateCustomTrack.Enabled=false;
			}
			List<MenuItem> listMenuItems=new List<MenuItem>();
			//The first item in the list will always exists, but we toggle it's visibility to only show when 1 row is selected.
			listMenuItems.Add(new MenuItem(Lan.g(this,"Go to Account"),new EventHandler(gridMain_RightClickHelper)));
			listMenuItems[0].Tag=0;//Tags are used to identify what to do in gridMain_RightClickHelper.
			listMenuItems.Add(new MenuItem(Lan.g(this,"Assign to Me"),new EventHandler(gridMain_RightClickHelper)));
			listMenuItems[1].Tag=1;
			listMenuItems.Add(new MenuItem(Lan.g(this,"Assign to User")));
			List<MenuItem> listSubUserMenu=new List<MenuItem>();
			_listClaimSentEditUsers.ForEach(x => { 
				listSubUserMenu.Add(new MenuItem(x.UserName,new EventHandler(gridMain_RightClickHelper)));
				listSubUserMenu[listSubUserMenu.Count-1].Tag=2;
			});
			listMenuItems[2].MenuItems.AddRange(listSubUserMenu.ToArray());
			Menu.MenuItemCollection menuItemCollection=new Menu.MenuItemCollection(rightClickMenu);
			menuItemCollection.AddRange(listMenuItems.ToArray());
			rightClickMenu.Popup+=new EventHandler((o,ea) => {
				rightClickMenu.MenuItems[0].Visible=false;
				if(gridMain.SelectedIndices.Count()!=1) {//Only show 'Go to Account' when there is exactly 1 row selected.
					return;
				}
				rightClickMenu.MenuItems[0].Visible=true;
				if(PrefC.HasClinicsEnabled) {
					long clinicNum=gridMain.SelectedTags<RpOutstandingIns.OutstandingInsClaim>()[0].ClinicNum;
					//If the user is clinic restricted, only enable 'Got to Account' when the clinicNum matches
					rightClickMenu.MenuItems[0].Enabled=ListTools.In(clinicNum,Clinics.GetForUserod(Security.CurUser,true,comboClinics.HqDescription).Select(x => x.ClinicNum));
				}
			});
			FillCustomTrack();
			FillPreauth();
			FillErrorDef();
			_hasFormLoaded=true;
			FillGrid(true);
		}

		private void FillPreauth() {
			comboPreauthOptions.Items.AddEnums<RpOutstandingIns.PreauthOptions>();
			comboPreauthOptions.SelectedIndex=(0);
		}

		private void FillProvs() {
			comboBoxMultiProv.Items.AddProvsFull(Providers.GetListReports());
			comboBoxMultiProv.IsAllSelected=true;
		}

		private void FillUsers() {
			comboUserAssigned.Items.Add(Lans.g(this,"Unassigned"),new Userod() {UserNum=0});
			_listClaimSentEditUsers.ForEach(x => comboUserAssigned.Items.Add(x.UserName,x));
			comboUserAssigned.IsAllSelected=true;
		}

		private void FillClinics() {
			//most is already handled automatically, but the default is to select "unassigned"
			comboClinics.IsAllSelected=true;//this also unselects "unassigned".
			//this is treated in this window as truly All clinics, not just all visible.
		}

		private void FillCustomTrack() {
			comboLastClaimTrack.IncludeAll=true;
			comboLastClaimTrack.SelectionModeMulti=false;//just a reminder
			comboLastClaimTrack.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ClaimCustomTracking,true));
			comboLastClaimTrack.IsAllSelected=true;
		}

		private void FillErrorDef() {
			comboErrorDef.Items.AddDefNone();
			comboErrorDef.SelectedIndex=0;
			comboErrorDef.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ClaimErrorCode,true));
		}

		private void FillDateFilterBy() {
			comboDateFilterBy.Items.AddEnums<RpOutstandingIns.DateFilterBy>();
			comboDateFilterBy.SelectedIndex=0;
		}

		private void FillGrid(bool isOnLoad = false) {
			if(!_hasFormLoaded) {//Prevents multiple fill grid calls when loading the form.
				return;
			}
			_preauthOption=comboPreauthOptions.GetSelected<RpOutstandingIns.PreauthOptions>();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			List<DisplayField> listDisplayFields = DisplayFields.GetForCategory(DisplayFieldCategory.OutstandingInsReport);
			foreach(DisplayField fieldCur in listDisplayFields) {
				HorizontalAlignment textAlign=HorizontalAlignment.Left;
				GridSortingStrategy sortingStrat = GridSortingStrategy.StringCompare;
				if(fieldCur.InternalName == "DateService"
					|| fieldCur.InternalName == "DateSent"
					|| fieldCur.InternalName == "DateSentOrig"
					|| fieldCur.InternalName == "DateStat"
					|| fieldCur.InternalName == "SubDOB"
					|| fieldCur.InternalName == "PatDOB") 
				{
					sortingStrat=GridSortingStrategy.DateParse;
					textAlign=HorizontalAlignment.Center;
				}
				else if(fieldCur.InternalName == "Amount") {
					sortingStrat=GridSortingStrategy.AmountParse;
					textAlign=HorizontalAlignment.Right;
				}
				gridMain.ListGridColumns.Add(new GridColumn(string.IsNullOrEmpty(fieldCur.Description) ? fieldCur.InternalName : fieldCur.Description
					,fieldCur.ColumnWidth
					,textAlign
					,sortingStrat));
			}
			gridMain.ListGridRows.Clear();
			DateTime dateTimeFrom=GetDateFrom();
			DateTime dateTimeTo=GetDateTo();
			string carrier=textCarrier.Text;
			RpOutstandingIns.DateFilterBy comboFilterBy=comboDateFilterBy.GetSelected<RpOutstandingIns.DateFilterBy>();
			bool isIgnoreCustomChecked=checkIgnoreCustom.Checked;
			List<long> listClinicNums=comboClinics.ListSelectedClinicNums;//Includes all clinics in combobox if 'All' is selected.
			List<long> listSelectedUserNums=comboUserAssigned.GetListSelected<Userod>().Select(x => x.UserNum).ToList();
			List<GridRow> listRows=null;
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				List<RpOutstandingIns.OutstandingInsClaim> listOustandingInsClaims=
					RpOutstandingIns.GetOutInsClaims(comboBoxMultiProv.GetSelectedProvNums(),dateTimeFrom,dateTimeTo,
						_preauthOption,listClinicNums,carrier,listSelectedUserNums,comboFilterBy);
				if(isOnLoad && listOustandingInsClaims.Any(x => x.CustomTrackingDefNum != 0)) {
					//If on load and the results have custom tracking entries, uncheck the "Ignore custom tracking" box so we can show it.
					//If it's not on load don't do this check as the user manually set filters.
					isIgnoreCustomChecked=false;
				}
				listRows=GetGridRows(listOustandingInsClaims,listDisplayFields,isIgnoreCustomChecked);
			};
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception e){
				FriendlyException.Show(Lan.g(this,"Error filling the Claims grid."),e);
			}
			if(progressOD.IsCancelled){
				gridMain.EndUpdate();
				return;
			}
			if(listRows==null) {//There was an error running the query and it did not finish.
				gridMain.EndUpdate();
				return;
			}
			listRows.ForEach(x => gridMain.ListGridRows.Add(x));
			gridMain.EndUpdate();
			checkIgnoreCustom.Checked=isIgnoreCustomChecked;
			textBox1.Text=total.ToString("c");
			labelClaimCount.Text=string.Format("{0} {1}",gridMain.ListGridRows.Count,gridMain.ListGridRows.Count==1 ? Lan.g(this,"claim") : Lan.g(this,"claims"));
			RefreshSelectedInfo();
		}

		private List<GridRow> GetGridRows(List<RpOutstandingIns.OutstandingInsClaim> listOustandingInsClaims,List<DisplayField> listDisplayFields
			,bool checkIgnoreCustomChecked)
		{
			List<GridRow> listRows=new List<GridRow>();
			GridRow row;
			string type;
			total=0;
			List<Def> listErrorDefs=Defs.GetDefsForCategory(DefCat.ClaimErrorCode,true);
			foreach(RpOutstandingIns.OutstandingInsClaim claimCur in listOustandingInsClaims) {
				if(!checkIgnoreCustomChecked) {
					DateTime dateSuppressed;
					try {
						dateSuppressed=claimCur.DateLog.AddDays(claimCur.DaysSuppressed);
					}
					catch(ArgumentOutOfRangeException ex) {//Custom Claim Tracking def.ValueString is way too big (observed bug) or way too small.
						ex.DoNothing();
						dateSuppressed=DateTime.MaxValue;//Likely due to the Days Suppressed value intended to be some far off unimaginable day.
					}
					if(dateSuppressed>DateTime.Today) {
						continue;
					}
				}
				if(!comboLastClaimTrack.IsAllSelected && claimCur.CustomTrackingDefNum!=comboLastClaimTrack.GetSelectedDefNum()) {
					continue;
				}
				if(comboErrorDef.GetSelectedDefNum()!=0 && comboErrorDef.GetSelectedDefNum() != claimCur.ErrorCodeDefNum) {
					continue;
				}
				row=new GridRow();
				foreach(DisplayField fieldCur in listDisplayFields) {
					switch(fieldCur.InternalName) {
						case "Carrier":
							row.Cells.Add(claimCur.CarrierName);
							break;
						case "Phone":
							row.Cells.Add(claimCur.CarrierPhone);
							break;
						case "Type":
							switch(claimCur.ClaimType) {
								case "P":
									type="Pri";
									break;
								case "S":
									type="Sec";
									break;
								case "PreAuth":
									type="Preauth";
									break;
								case "Other":
									type="Other";
									break;
								case "Cap":
									type="Cap";
									break;
								case "Med":
									type="Medical";//For possible future use.
									break;
								default:
									type="Error";//Not allowed to be blank.
									break;
							}
							row.Cells.Add(Lan.g(this,type));
							break;
						case "User":
							row.Cells.Add(Userods.GetName(claimCur.UserNum));
							break;
						case "PatName":
							string patName=claimCur.PatLName+", "+claimCur.PatFName+" "+claimCur.PatMiddleI;
							if(PrefC.GetBool(PrefName.ReportsShowPatNum)) {
								row.Cells.Add(claimCur.PatNum+"-"+patName);
							}
							else {
								row.Cells.Add(patName);
							}
							break;
						case "Clinic":
							row.Cells.Add(Clinics.GetAbbr(claimCur.ClinicNum));
							break;
						case "DateService":
							row.Cells.Add(claimCur.DateService.ToShortDateString());
							break;
						case "DateSent":
							row.Cells.Add(claimCur.DateSent.ToShortDateString());
							break;
						case "DateSentOrig":
							row.Cells.Add(claimCur.DateOrigSent.ToShortDateString());
							break;
						case "TrackStat":
							row.Cells.Add(Defs.GetDefsForCategory(DefCat.ClaimCustomTracking)  // to display hidden defs' names, don't use short list
								.FirstOrDefault(x => x.DefNum==claimCur.CustomTrackingDefNum)?.ItemName??"-");
							break;
						case "DateStat":
							row.Cells.Add(claimCur.DateLog.ToShortDateString());
							break;
						case "Error":
							row.Cells.Add(listErrorDefs.FirstOrDefault(x => x.DefNum == claimCur.ErrorCodeDefNum)?.ItemName??"-");
							break;
						case "Amount":
							row.Cells.Add(claimCur.ClaimFee.ToString("f"));
							break;
						case "GroupNum":
							row.Cells.Add(claimCur.GroupNum);
							break;
						case "GroupName":
							row.Cells.Add(claimCur.GroupName);
							break;
						case "SubName":
							row.Cells.Add(claimCur.SubName);
							break;
						case "SubDOB":
							row.Cells.Add(claimCur.SubDOB.ToShortDateString());
							break;
						case "SubID":
							row.Cells.Add(claimCur.SubID);
							break;
						case "PatDOB":
							row.Cells.Add(claimCur.PatDOB.ToShortDateString());
							break;
						default:
							row.Cells.Add(Lan.g(this,"MISSING"));
							break;
					}
				}
				row.Tag=claimCur;
				listRows.Add(row);
				total+=claimCur.ClaimFee;
			}
			return listRows;
		}
		
		private void butRefresh_Click(object sender,EventArgs e) {
			Plugins.HookAddCode(this,"FormRpOutstandingIns.butRefresh_begin");
			FillGrid();
		}

		private void comboPreauthOptions_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void checkIgnoreCustom_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void comboBoxMultiProv_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboBoxMultiProv.SelectedIndices.Count==0) {
				comboBoxMultiProv.IsAllSelected=true;
			}
		}

		///<summary>Click method used by all gridMain right click options.
		///We identify what logic to run by the menuItem.Tag.</summary>
		private void gridMain_RightClickHelper(object sender,EventArgs e) {
			int index=gridMain.GetSelectedIndex();
			if(index==-1) {
				return;
			}
			int menuCode=(int)((MenuItem)sender).Tag;
			switch(menuCode) {
				case 0://Go to Account
					GotoModule.GotoAccount(((RpOutstandingIns.OutstandingInsClaim)gridMain.ListGridRows[index].Tag).PatNum);
				break;
				case 1://Assign to Me
					AssignUserHelper(Security.CurUser.UserNum);
				break;
				case 2://Assign to User
					AssignUserHelper(_listClaimSentEditUsers[((MenuItem)sender).Index].UserNum);
				break;
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(Permissions.ClaimView)) {
				return;
			}
			Claim claim=Claims.GetClaim(((RpOutstandingIns.OutstandingInsClaim)gridMain.ListGridRows[e.Row].Tag).ClaimNum);
			if(claim==null) {
				MsgBox.Show(this,"The claim has been deleted.");
				FillGrid();
				return;
			}
			Patient pat=Patients.GetPat(claim.PatNum);
			Family fam=Patients.GetFamily(pat.PatNum);
			using FormClaimEdit FormCE=new FormClaimEdit(claim,pat,fam);
			FormCE.IsNew=false;
			FormCE.ShowDialog();
		}

		private void buttonUpdateCustomTrack_Click(object sender,EventArgs e) {			
			List<long> listClaimNum=new List<long>();
			for(int i = 0;i<gridMain.ListGridRows.Count;i++) {
				listClaimNum.Add(((RpOutstandingIns.OutstandingInsClaim)gridMain.ListGridRows[i].Tag).ClaimNum);
			}
			if(listClaimNum.Count==0) {
				MsgBox.Show(this,"No claims in list. Must have at least one claim.");
				return;
			}
			List<Claim> listClaims=Claims.GetClaimsFromClaimNums(listClaimNum);
			using FormClaimCustomTrackingUpdate FormCT=new FormClaimCustomTrackingUpdate(listClaims);
			if(FormCT.ShowDialog()==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			Close();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			pagesPrinted=0;	
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Outstanding insurance report printed"),PrintoutOrientation.Landscape);
		}

		private void butAssignUser_Click(object sender,EventArgs e) {
			long userNum=PickUser(true);
			if(userNum==-2) {//User canceled selection.
				return;
			}
			AssignUserHelper(userNum);
		}

		///<summary>Loops through gridMain.SelectedIndices to create or update ClaimTracking rows.</summary>
		private void AssignUserHelper(long assignUserNum) {
			if(gridMain.SelectedIndices.Count()==0) {
				MsgBox.Show(this,"Please select at least one claim to assign a user.");
				return;
			}
			List<ODTuple<long,long>> listTrackingNumsAndClaimNums=new List<ODTuple<long,long>>();
			foreach(int index in gridMain.SelectedIndices) {
				RpOutstandingIns.OutstandingInsClaim outstandingInsClaim = (RpOutstandingIns.OutstandingInsClaim)gridMain.ListGridRows[index].Tag;
				long claimTrackingNum=_listNewClaimTrackings.FirstOrDefault(x => x.ClaimNum==outstandingInsClaim.ClaimNum)?.ClaimTrackingNum??0;
				long claimNum = outstandingInsClaim?.ClaimNum??0;
				listTrackingNumsAndClaimNums.Add(new Tuple<long, long>(claimTrackingNum,claimNum));
			}
			_listNewClaimTrackings=ClaimTrackings.Assign(listTrackingNumsAndClaimNums,assignUserNum);
			_listOldClaimTrackings.Clear();//After sync, the old list is updated.
			_listNewClaimTrackings.ForEach(x => _listOldClaimTrackings.Add(x.Copy()));
			FillGrid();
		}

		private void butPickUser_Click(object sender,EventArgs e) {
			long userNum=PickUser(false);
			if(userNum==-2) {//User canceled selection.
				return;
			}
			ComboUserPickHelper(userNum);
		}

		///<summary>After calling PickUser(false) this is used to set comboUserAssigneds selection.
		///Also calls FillGrid() to reflect new selection.</summary>
		private void ComboUserPickHelper(long filterUserNum) {
			if(filterUserNum==-1) {//Defaults to 'All', filterUserNum will be -1 in this case
				comboUserAssigned.IsAllSelected=true;
			}
			else {
				comboUserAssigned.IsAllSelected=false;
				comboUserAssigned.SetSelectedKey<Userod>(filterUserNum,x=>x.UserNum);
			}
			FillGrid();
		}

		///<summary>Opens FormUserPick to allow user to select a user.
		///Returns UserNum associated to selection.
		///0 represents Unassigned
		///-1 represents All
		///-2 represents a canceled selection</summary>
		private long PickUser(bool isAssigning) {
			using FormUserPick FormUP=new FormUserPick();
			FormUP.IsSelectionmode=true;
			FormUP.ListUserodsFiltered=_listClaimSentEditUsers;
			if(!isAssigning) {
				FormUP.IsPickAllAllowed=true;
			}
			FormUP.IsPickNoneAllowed=true;
			FormUP.ShowDialog();
			if(FormUP.DialogResult!=DialogResult.OK) {
				return -2;
			}
			return FormUP.SelectedUserNum;
		}

		/// <summary>Sets date controls in both date tabs</summary>
		private void SetDates(DateTime dateFrom,DateTime dateTo) {
			string daysOldMin=POut.Int((int)Math.Round((DateTime.Today-dateTo.Date).TotalDays,0));//Calculate min days old from dateTo.
			string daysOldMax="";
			if(dateFrom>dateTo) {
				dateFrom=dateTo.Date;//dateFrom cannot be after dateTo
			}
			if(dateFrom>DateTime.MinValue) {
				daysOldMax=POut.Int((int)Math.Round((DateTime.Today-dateFrom.Date).TotalDays,0));//Calculate max days old from dateFrom.
			}
			else {
				dateFrom=DateTime.MinValue;//MinValue, but show a blank in the date text box.
				daysOldMax="";//DaysOld max field should be blank.
			}
			textDaysOldMax.Text=daysOldMax;//DateFrom
			dateRangePicker.SetDateTimeFrom(dateFrom);//DateFrom
			textDaysOldMin.Text=daysOldMin;////DateTo
			dateRangePicker.SetDateTimeTo(dateTo);//DateTo			
		}

		///<summary>Gets date control for report</summary>
		private DateTime GetDateTo() {
			return GetDateTo(tabControlDate.SelectedTab);
		}

		///<summary>Gets date control for report from a specific tab in tabControlDate</summary>
		private DateTime GetDateTo(TabPage tabPageCur) {
			DateTime dateMin=DateTime.Today;
			if(tabPageCur==tabDaysOld) {
				int daysOldMin=0;
				int.TryParse(textDaysOldMin.Text.Trim(),out daysOldMin);
				//can't use error provider here because this fires on text changed and cursor may not have left the control, so there is no error message yet
				if(daysOldMin>0 && daysOldMin.Between(textDaysOldMin.MinVal,textDaysOldMin.MaxVal)) {
					dateMin=DateTime.Today.AddDays(-1*daysOldMin);
				}
			}
			else if(tabPageCur==tabDateRange) {
				dateMin=dateRangePicker.GetDateTimeTo();//Very end of day.
				if(dateMin>DateTime.Today) {
					dateMin=DateTime.Today.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);
					dateRangePicker.SetDateTimeTo(dateMin);
				}
			}
			return dateMin;
		}

		///<summary>Gets date control for report</summary>
		private DateTime GetDateFrom() {
			return GetDateFrom(tabControlDate.SelectedTab);
		}
		
		///<summary>Gets date control for report from a specific tab in tabControlDate</summary>
		private DateTime GetDateFrom(TabPage tabPageCur) {
			DateTime dateMax=DateTime.MinValue;
			if(tabPageCur==tabDaysOld) {
				int daysOldMax=0;
				int.TryParse(textDaysOldMax.Text.Trim(),out daysOldMax);
				//can't use error provider here because this fires on text changed and cursor may not have left the control, so there is no error message yet
				if(daysOldMax>0 && daysOldMax.Between(textDaysOldMax.MinVal,textDaysOldMax.MaxVal)) {
					dateMax=DateTime.Today.AddDays(-1*daysOldMax);
				}
			}
			else if(tabPageCur==tabDateRange) {
				dateMax=dateRangePicker.GetDateTimeFrom().Date;//Very beginning of day.
				if(dateMax>DateTime.Today) {
					dateMax=DateTime.Today;
					dateRangePicker.SetDateTimeFrom(dateMax);
				}
			}
			return dateMax;
		}

		private void butMine_Click(object sender,EventArgs e) {
			FillClinics();
			ComboUserPickHelper(Security.CurUser.UserNum);
		}

		private void ComboDateFilterBy_SelectionChangeCommitted(object sender, EventArgs e){
			FillGrid();
		}

		private void tabControlDate_SelectedIndexChanged(object sender,EventArgs e) {
			TabPage tabPagePrevious=(tabControlDate.SelectedTab==tabDaysOld?tabDateRange:tabDaysOld);//Get dates from tab we are leaving.
			DateTime dateFrom=GetDateFrom(tabPagePrevious);
			DateTime dateTo=GetDateTo(tabPagePrevious);
			SetDates(dateFrom,dateTo);//Make sure both tabDaysOld and tabDateRange are concurrent.
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			RefreshSelectedInfo();
		}

		private void dateRangePicker_CalendarClosed(object sender,EventArgs e) {
			FillGrid();
		}

		private void dateRangePicker_CalendarSelectionChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void RefreshSelectedInfo() {
			List<RpOutstandingIns.OutstandingInsClaim> listSelected=gridMain.SelectedGridRows.Select(x => x.Tag)
				.OfType<RpOutstandingIns.OutstandingInsClaim>().ToList();
			if(listSelected.Count == 0) {
				//clear textboxes
				textPatDOB.Text="";
				textSubscriberID.Text="";
				textSubscriberName.Text="";
				textSubscriberDOB.Text="";
				textCarrierName.Text="";
				textCarrierPhone.Text="";
				textGroupName.Text="";
				textGroupNumber.Text="";
			}
			else if(listSelected.Count == 1) {
				RpOutstandingIns.OutstandingInsClaim insClaim = listSelected.First();
				//fill textboxes
				textPatDOB.Text=insClaim.PatDOB.ToShortDateString();
				textSubscriberID.Text=insClaim.SubID;
				textSubscriberName.Text=insClaim.SubName;
				textSubscriberDOB.Text=insClaim.SubDOB.ToShortDateString();
				textCarrierName.Text=insClaim.CarrierName;
				textCarrierPhone.Text=insClaim.CarrierPhone;
				textGroupName.Text=insClaim.GroupName;
				textGroupNumber.Text=insClaim.GroupNum;
			}
			else if(listSelected.Count > 1) {
				//fill textboxes
				string multiSelectedStr = "<"+Lans.g(this,"Multiple Selected")+">";
				textPatDOB.Text=multiSelectedStr;
				textSubscriberID.Text=multiSelectedStr;
				textSubscriberName.Text=multiSelectedStr;
				textSubscriberDOB.Text=multiSelectedStr;
				textCarrierName.Text=multiSelectedStr;
				textCarrierPhone.Text=multiSelectedStr;
				textGroupName.Text=multiSelectedStr;
				textGroupNumber.Text=multiSelectedStr;
			}
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!headingPrinted) {
				text=Lan.g(this,"Outstanding Insurance Claims");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				if(_preauthOption==RpOutstandingIns.PreauthOptions.IncludingPreauths) {
					text=Lan.g(this,"Including Preauthorizations");
				}
				else if(_preauthOption==RpOutstandingIns.PreauthOptions.ExcludingPreauths) {
					text=Lan.g(this,"Not Including Preauthorizations");
				}
				else {
					text=Lan.g(this,"Only Preauthorizations");
				}
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				//print today's date
				text = Lan.g(this,"Run On:")+" "+DateTime.Today.ToShortDateString();
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				if(comboBoxMultiProv.IsAllSelected) {
					text=Lan.g(this,"For All Providers");
				}
				else {
					text=Lan.g(this,"For Providers:")+" ";
					text+=string.Join(", ",comboBoxMultiProv.GetSelectedProvNums().Select(provNum => Providers.GetFormalName(provNum)));
				}
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				headingPrinted=true;
				headingPrintH=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,pagesPrinted,bounds,headingPrintH);
			pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
				text="Total: $"+total.ToString("F");
				g.DrawString(text,subHeadingFont,Brushes.Black,center+bounds.Width/2-g.MeasureString(text,subHeadingFont).Width,yPos);
			}
			g.Dispose();
		}

		private void butExport_Click(object sender,System.EventArgs e) {			
			string fileName=Lan.g(this,"Outstanding Insurance Claims");
			string filePath=ODFileUtils.CombinePaths(Path.GetTempPath(),fileName);
			if(ODBuild.IsWeb()) {
				//file download dialog will come up later, after file is created.
				filePath+=".txt";//Provide the filepath an extension so that Thinfinity can offer as a download.
			}
			else {
				SaveFileDialog saveFileDialog=new SaveFileDialog();
				saveFileDialog.AddExtension=true;
				saveFileDialog.FileName=fileName;
				if(!Directory.Exists(PrefC.GetString(PrefName.ExportPath))) {
					try {
						Directory.CreateDirectory(PrefC.GetString(PrefName.ExportPath));
						saveFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
					}
					catch {
						//initialDirectory will be blank
					}
				}
				else {
					saveFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
				}
				saveFileDialog.Filter="Text files(*.txt)|*.txt|Excel Files(*.xls)|*.xls|All files(*.*)|*.*";
				saveFileDialog.FilterIndex=0;
				if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				filePath=saveFileDialog.FileName;
			}
			try {
				using(StreamWriter sw=new StreamWriter(filePath,false))
				//new FileStream(,FileMode.Create,FileAccess.Write,FileShare.Read)))
				{
					String line="";
					for(int i=0;i<gridMain.ListGridColumns.Count;i++) {
						line+=gridMain.ListGridColumns[i].Heading+"\t";
					}
					sw.WriteLine(line);
					for(int i=0;i<gridMain.ListGridRows.Count;i++) {
						line="";
						for(int j=0;j<gridMain.ListGridColumns.Count;j++) {
							line+=gridMain.ListGridRows[i].Cells[j].Text;
							if(j<gridMain.ListGridColumns.Count-1) {
								line+="\t";
							}
						}
						sw.WriteLine(line);
					}
				}
			}
			catch {
				MessageBox.Show(Lan.g(this,"File in use by another program.  Close and try again."));
				return;
			}
			if(ODBuild.IsWeb()) {
				ThinfinityUtils.ExportForDownload(filePath);
			}
			else {
				MessageBox.Show(Lan.g(this,"File created successfully"));
			}
		}

	
	}
}