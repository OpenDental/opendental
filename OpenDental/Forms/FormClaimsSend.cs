using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.Eclaims;

namespace OpenDental{
///<summary></summary>
	public partial class FormClaimsSend:FormODBase {
		///<summary>The list of all claims regardless of any filters.  Filled on load.  Make sure to update this list with any updates (e.g. after validating claims)</summary>
		private ClaimSendQueueItem[] _claimSendQueueItemArrayQueueAll;
		///<summary></summary>
		public long GotoPatNum;
		///<summary></summary>
		public long GotoClaimNum;
		private DataTable _tableHistory;
		private int _pagesPrinted;
		private bool _isMouseDownOnSplitter;
		private int _ySplitterOriginal;
		private int _yMouseOriginal;
		private bool _isHeadingPrinted;
		private int _heightHeadingPrint;
		private List<EtransType> _listEtransTypes;
		//private ContextMenu contextMenuHist;
		private List<Clinic> _listClinics;
		///<summary>Represents the number of unsent claims per clinic. This is a 1:1 list with _listClinics.</summary>
		private List<int> _listNumberOfClaims;
		private List<Clearinghouse> _listClearinghouses;
		private List<Def> _listDefsClaimCustomTracking;
		///<summary>Index of the Clearinghouse column in the main grid.  Can change depending on if clinics feature is enabled or not.</summary>
		private int clearinghouseIndex=-1;

		private delegate void ToolBarClick();

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormClaimsSendCanada";
			}
			return "FormClaimsSend";
		}

		///<summary></summary>
		public FormClaimsSend(){
			InitializeComponent();
			InitializeLayoutManager();
			//tbQueue.CellDoubleClicked += new OpenDental.ContrTable.CellEventHandler(tbQueue_CellDoubleClicked);
			Lan.F(this);
		}

		private void FormClaimsSend_Load(object sender, System.EventArgs e) {
			Plugins.HookAddCode(this,"FormClaimsSend.FormClaimsSend_Load_start");
			_claimSendQueueItemArrayQueueAll=Claims.GetQueueList(0,0,0);
			_claimSendQueueItemArrayQueueAll.ForEach(x => x.MissingData="(validated when sending)");
			_listNumberOfClaims=new List<int>();
			contextMenuStatus.MenuItems.Add(Lan.g(this,"Go to Account"),new EventHandler(GotoAccount_Clicked));
			MenuItem menuItemDeleteClaim=new MenuItem();
			menuItemDeleteClaim.Name="menuItemDeleteClaim";
			menuItemDeleteClaim.Text=Lan.g(this,"Delete Claim");
			menuItemDeleteClaim.Click+=new EventHandler(menuItemDeleteClaim_Click);
			contextMenuStatus.MenuItems.Add(menuItemDeleteClaim);
			contextMenuStatus.Popup+=MenuItemPopup;
			gridMain.ContextMenu=contextMenuStatus;
			gridHistory.ContextMenu=contextMenuHistory;
			_listClearinghouses=Clearinghouses.GetDeepCopy(true);
			for(int i=0;i<_listClearinghouses.Count;i++){
				contextMenuEclaims.MenuItems.Add(_listClearinghouses[i].Description,new EventHandler(menuItemClearinghouse_Click));
			}
			LayoutToolBars();
			if(!PrefC.HasClinicsEnabled) {
				comboClinic.Visible=false;
				labelClinic.Visible=false;
				butNextUnsent.Visible=false;
			}
			else {
				_listClinics=Clinics.GetForUserod(Security.CurUser);
			}
			comboCustomTracking.Items.Add(Lan.g(this,"all"));
			comboCustomTracking.SelectedIndex=0;
			_listDefsClaimCustomTracking=Defs.GetDefsForCategory(DefCat.ClaimCustomTracking,true);
			if(_listDefsClaimCustomTracking.Count==0){
				labelCustomTracking.Visible=false;
				comboCustomTracking.Visible=false;
			}
			else{
				for(int i=0;i<_listDefsClaimCustomTracking.Count;i++) {
					comboCustomTracking.Items.Add(_listDefsClaimCustomTracking[i].ItemName);
				}
			}
			if(PrefC.RandomKeys && PrefC.HasClinicsEnabled){//using random keys and clinics
				//Does not pull in reports automatically, because they could easily get assigned to the wrong clearinghouse
			}
			else{
				using FormClaimReports formClaimReports=new FormClaimReports(); //the currently selected clinic is what the combobox defaults to.
				formClaimReports.IsAutomaticMode=true;
				formClaimReports.ShowDialog();
			}
			FillGrid(isRefreshRequired:false);//no need to refresh, we just got the list from the db (_arrayQueueAll above)
			//Validate all claims if the preference is enabled.
			if(PrefC.GetBool(PrefName.ClaimsSendWindowValidatesOnLoad)) {
				//This can be very slow if there are lots of claims to validate.
				ValidateClaims(_claimSendQueueItemArrayQueueAll.ToList());
			}
			dateRangePicker.SetDateTimeFrom(DateTime.Today.AddDays(-7));
			dateRangePicker.SetDateTimeTo(DateTime.Today);
			_listEtransTypes=new List<EtransType>();
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				_listEtransTypes.Add(EtransType.ClaimPrinted);
				_listEtransTypes.Add(EtransType.Claim_CA);
				_listEtransTypes.Add(EtransType.Claim_Ren);
				_listEtransTypes.Add(EtransType.Eligibility_CA);
				_listEtransTypes.Add(EtransType.ClaimReversal_CA);
				_listEtransTypes.Add(EtransType.Predeterm_CA);
				_listEtransTypes.Add(EtransType.RequestOutstand_CA);
				_listEtransTypes.Add(EtransType.RequestSumm_CA);
				_listEtransTypes.Add(EtransType.RequestPay_CA);
				_listEtransTypes.Add(EtransType.ClaimCOB_CA);
				_listEtransTypes.Add(EtransType.Claim_Ramq);
				_listEtransTypes.Add(EtransType.TextReport);
				_listEtransTypes.Add(EtransType.ItransNcpl);
			}
			else {//United States
				_listEtransTypes.Add(EtransType.ClaimSent);
				_listEtransTypes.Add(EtransType.ClaimPrinted);
				_listEtransTypes.Add(EtransType.Claim_Ren);
				_listEtransTypes.Add(EtransType.StatusNotify_277);
				_listEtransTypes.Add(EtransType.TextReport);
				_listEtransTypes.Add(EtransType.ERA_835);
				_listEtransTypes.Add(EtransType.Ack_Interchange);
			}
			List<int> listSelectedItems=new List<int>();
			for(int i=0;i<_listEtransTypes.Count;i++) {
				comboHistoryType.Items.Add(_listEtransTypes[i].ToString());
				comboHistoryType.SetSelected(i,true);
			}
			FillHistory();
			SetFilterControlsAndAction(() => FillGrid(isRefreshRequired: false),500,textCarrier,textProv,textProc);
		}

		public void FillClinicsList(long claimCustomTrackingNum) {//Not using ComboBoxClinicPicker due to adding number of unsent clinics to label of each item
			comboClinic.IncludeAll=true;
			List<Clinic> listClinicsPrevious=new List<Clinic>();
			if(comboClinic.GetListSelected<Clinic>().Count!=0) {//Only 0 the first time this method is run.
				listClinicsPrevious=comboClinic.GetListSelected<Clinic>();
			}
			comboClinic.Items.Clear();
			_listNumberOfClaims.Clear();
			if(!Security.CurUser.ClinicIsRestricted) {
				Clinic clinicUnassigned=new Clinic();
				clinicUnassigned.Abbr=Lan.g(this,"Unassigned/Default");
				clinicUnassigned.ClinicNum=0;
				comboClinic.Items.Add(clinicUnassigned.Abbr,clinicUnassigned);
				comboClinic.SetSelectedKey<Clinic>(clinicUnassigned.ClinicNum,x=>x.ClinicNum,x=>clinicUnassigned.Abbr);
			}
			for(int i=0;i<_listClinics.Count;i++) {
				int countClaims=0;
				for(int j=0;j<_claimSendQueueItemArrayQueueAll.Length;j++) {
					if(_claimSendQueueItemArrayQueueAll[j].ClinicNum!=_listClinics[i].ClinicNum) {
						continue;
					}
					if(claimCustomTrackingNum==0 || _claimSendQueueItemArrayQueueAll[j].CustomTracking==claimCustomTrackingNum) {
						countClaims+=1;
					}
				}
				_listNumberOfClaims.Add(countClaims);
				string strClinicWithCount=_listClinics[i].Abbr+"  ("+countClaims+")";
				comboClinic.Items.Add(strClinicWithCount,_listClinics[i]);
				if(_listClinics[i].ClinicNum==Clinics.ClinicNum) {
					comboClinic.SetSelectedKey<Clinic>(Clinics.ClinicNum,x=>x.ClinicNum,x=>strClinicWithCount);
				}
			}
			List<int> listSelectedIndices=new List<int>();
			List<long> listClinicNumOptions=comboClinic.Items.GetAll<Clinic>().Select(x=>x.ClinicNum).ToList();
			for(int i=0;i<listClinicsPrevious.Count;i++) {
				int selectedClinicIndex=listClinicNumOptions.IndexOf(listClinicsPrevious[i].ClinicNum);
				if(selectedClinicIndex!=-1) {
					listSelectedIndices.Add(selectedClinicIndex);
				}
			}
			if(listSelectedIndices.Count > 0) {
				comboClinic.SelectedIndices=listSelectedIndices;
			}
		}

		///<summary></summary>
		public void LayoutToolBars(){
			ToolBarMain.Buttons.Clear();
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Preview"),0,Lan.g(this,"Preview the Selected Claim"),"Preview"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Blank"),1,Lan.g(this,"Print a Blank Claim Form"),"Blank"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print"),2,Lan.g(this,"Print Selected Claims"),"Print"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Labels"),6,Lan.g(this,"Print Single Labels"),"Labels"));
			/*ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ODToolBarButton button=new ODToolBarButton(Lan.g(this,"Change Status"),-1,Lan.g(this,"Changes Status of Selected Claims"),"Status");
			button.Style=ODToolBarButtonStyle.DropDownButton;
			button.DropDownMenu=contextMenuStatus;
			ToolBarMain.Buttons.Add(button);*/
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ODToolBarButton button=new ODToolBarButton(Lan.g(this,"Send E-Claims"),4,Lan.g(this,"Send claims Electronically"),"Eclaims");
			button.Style=ODToolBarButtonStyle.DropDownButton;
			button.DropDownMenu=contextMenuEclaims;
			ToolBarMain.Buttons.Add(button);
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Validate Claims"),-1,Lan.g(this,"Refresh and Validate Selected Claims"),"Validate"));
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Outstanding"),-1,Lan.g(this,"Get Outstanding Transactions"),"Outstanding"));
				ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Payment Rec"),-1,Lan.g(this,"Get Payment Reconciliation Transactions"),"PayRec"));
				//ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Summary Rec"),-1,Lan.g(this,"Get Summary Reconciliation Transactions"),"SummaryRec"));
			}
			else {
				ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Get Reports"),5,Lan.g(this,"Get Reports from Other Clearinghouses"),"Reports"));
			}
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Refresh"),-1,Lan.g(this,"Refresh the Grid"),"Refresh"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Procs Not Billed"),-1,"","ProcsNotBilled"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Close"),-1,"","Close"));
			/*ArrayList toolButItems=ToolButItems.GetForToolBar(ToolBarsAvail.ClaimsSend);
			for(int i=0;i<toolButItems.Count;i++){
				ToolBarMain.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
				ToolBarMain.Buttons.Add(new ODToolBarButton(((ToolButItem)toolButItems[i]).ButtonText
					,-1,"",((ToolButItem)toolButItems[i]).ProgramNum));
			}*/
			ToolBarMain.Invalidate();
			ToolBarHistory.Buttons.Clear();
			ToolBarHistory.Buttons.Add(new ODToolBarButton(Lan.g(this,"Refresh"),-1,Lan.g(this,"Refresh this list."),"Refresh"));
			ToolBarHistory.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarHistory.Buttons.Add(new ODToolBarButton(Lan.g(this,"Undo"),-1,
				Lan.g(this,"Change the status of claims back to 'Waiting'."),"Undo"));
			ToolBarHistory.Buttons.Add(new ODToolBarButton(ODToolBarButtonStyle.Separator));
			ToolBarHistory.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print List"),2,
				Lan.g(this,"Print history list."),"PrintList"));
			ToolBarHistory.Buttons.Add(new ODToolBarButton(Lan.g(this,"Outstanding Claims"),-1,"","OutstandingClaims"));
			//if(ODBuild.IsDebug()) {
			//	ToolBarHistory.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print Item"),2,
			//		Lan.g(this,"For debugging, this will simply display the first item in the list."),"PrintItem"));
			//}
			//else {
			//	ToolBarHistory.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print Item"),2,
			//		Lan.g(this,"Print one item from the list."),"PrintItem"));
			//}
			ToolBarHistory.Invalidate();
		}

		private void MenuItemPopup(object sender,EventArgs e) {
			MenuItem menuItemDeleteClaim=gridMain.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Name=="menuItemDeleteClaim");
			menuItemDeleteClaim.Enabled=Security.IsAuthorized(EnumPermType.ClaimDelete,true);
			if(gridMain.SelectedTags<ClaimSendQueueItem>().Count==1
				&& gridMain.SelectedTag<ClaimSendQueueItem>().MissingData.Contains(Eclaims.GetNoProceduresOnClaimMessage()))
			{
				menuItemDeleteClaim.Visible=true;
			}
			else {
				menuItemDeleteClaim.Visible=false;
			}
		}

		private void menuItemDeleteClaim_Click(object sender,EventArgs e) {
			ClaimSendQueueItem claimSendQueueItem=gridMain.SelectedTag<ClaimSendQueueItem>();
			if(claimSendQueueItem==null) {
				return;
			}
			if(claimSendQueueItem.ProcedureCodeString=="") {
				Claim claim=Claims.GetClaim(claimSendQueueItem.ClaimNum);
				List<ClaimProc> listClaimProcs=ClaimProcs.RefreshForClaim(claim.ClaimNum);
				if(listClaimProcs.Any(x => ClaimProcs.GetInsPaidStatuses().Contains(x.Status))) {
					MsgBox.Show(this,"Cannot delete, there are claim procedures attached to this claim.");
					return;
				}
				ClaimProcs.DeleteMany(listClaimProcs);
				List<long> listEtrans835AttachNums=Etrans835Attaches.GetForClaimNums(claim.ClaimNum).Select(x => x.Etrans835AttachNum).ToList();
				Claims.Delete(claim,listEtrans835AttachNums);
				SecurityLogs.MakeLogEntry(EnumPermType.ClaimDelete,claim.PatNum,gridMain.SelectedTag<ClaimSendQueueItem>().PatName
					+", "+Lan.g(this,"Date Entry")+": "+claim.SecDateEntry.ToShortDateString()
					+", "+Lan.g(this,"Date of Service")+": "+claim.DateService.ToShortDateString(),
					claimSendQueueItem.ClaimNum,claim.SecDateTEdit);
				FillGrid();
			}
			else {
				MsgBox.Show(this,"Cannot delete, there are procedures attached to this claim.");
			}
		}


		private void GotoAccount_Clicked(object sender, System.EventArgs e){
			//accessed by right clicking
			if(gridMain.SelectedTags<ClaimSendQueueItem>().Count!=1) {
				MsgBox.Show(this,"Please select exactly one item first.");
				return;
			}
			ODEvent.Fire(ODEventType.FormClaimSend_GoTo,gridMain.SelectedTags<ClaimSendQueueItem>().First());
			SendToBack();
		}

		private void menuItemClearinghouse_Click(object sender, System.EventArgs e){
			MenuItem menuitem=(MenuItem)sender;
			SendEclaimsToClearinghouse(_listClearinghouses[menuitem.Index].ClearinghouseNum);
		}

		private void FillGrid(bool rememberSelection=false,bool isRefreshRequired=true) {
			if(PrefC.HasClinicsEnabled) {
				long claimCustomTrackingNum=0;
				if(comboCustomTracking.SelectedIndex!=0) {
					claimCustomTrackingNum=Defs.GetDefsForCategory(DefCat.ClaimCustomTracking,true)[comboCustomTracking.SelectedIndex-1].DefNum;
				}
				FillClinicsList(claimCustomTrackingNum);
			}
			int oldScrollValue=0;
			List<long> listOldSelectedClaimNums=new List<long>();
			if(rememberSelection) {
				oldScrollValue=gridMain.ScrollValue;
				for(int i=0;i<gridMain.SelectedTags<ClaimSendQueueItem>().Count;i++) {
					listOldSelectedClaimNums.Add(gridMain.SelectedTags<ClaimSendQueueItem>()[i].ClaimNum);
				}
			}
			if(isRefreshRequired) {
				ClaimSendQueueItem[] claimSendQueueItemArray=Claims.GetQueueList(0,0,0);//Get fresh new "all" list from db.
				for(int i=0;i<claimSendQueueItemArray.Length;i++) {
					//If any data in the new list needs to be refreshed because something changed, refresh it.
					//At this point, _arrayQueueAll is the old list of all claims.
					for(int j=0;j<_claimSendQueueItemArrayQueueAll.Length;j++) {//Go through the old list of all claims.
						if(claimSendQueueItemArray[i].ClaimNum==_claimSendQueueItemArrayQueueAll[j].ClaimNum) {//The same claim is in both the old and new "all" lists.
							claimSendQueueItemArray[i]=_claimSendQueueItemArrayQueueAll[j];//Keep the same exact queue item as before so we can maintain the MissingData, etc.
						}
					}
					if(claimSendQueueItemArray[i].MissingData==null) {//Can only be null if the claim was not in the old "all" list.  For example when undo.
						claimSendQueueItemArray[i].MissingData="(validated when sending)";
					}
				}
				_claimSendQueueItemArrayQueueAll=claimSendQueueItemArray;
			}
			//Get filtered list from list all
			ClaimSendQueueItem[] claimSendQueueItemArrayFiltered=GetListQueueFiltered();//We update the class wide variable because it is used in double clicking and other events.
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableQueue","DateService"),75,HorizontalAlignment.Center);//new column
			col.SortingStrategy=GridSortingStrategy.DateParse;
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableQueue","Patient Name"),120);//was 190
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableQueue","Carrier Name"),180);//was 220, before that was 100 but was too small.  In Insurance Plans window is 140.
			gridMain.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g("TableQueue","Clinic"),80);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g("TableQueue","Provider"),55);//Just large enough to show the title
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableQueue","M/D"),30);//Just large enough to hold 4 characters (see below)
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableQueue","Clearinghouse"),85);//Just large enough for the title
			gridMain.Columns.Add(col);
			clearinghouseIndex=gridMain.Columns.Count-1;
			col=new GridColumn(Lan.g("TableQueue","Warnings"),120);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableQueue","Missing Info"),280);//was 300, reduced to fit Ordinal.
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableQueue","Ordinal"),50);//Just large enough for the title.
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableQueue","ProcCodes"),120){ IsWidthDynamic=true };
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<claimSendQueueItemArrayFiltered.Length;i++) {
				row=new GridRow();
				row.Cells.Add(claimSendQueueItemArrayFiltered[i].DateService.ToShortDateString());
				row.Cells.Add(claimSendQueueItemArrayFiltered[i].PatName);
				row.Cells.Add(claimSendQueueItemArrayFiltered[i].Carrier);
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(claimSendQueueItemArrayFiltered[i].ClinicNum));
				}
				row.Cells.Add(Providers.GetAbbr(claimSendQueueItemArrayFiltered[i].ProvTreat));
				switch(claimSendQueueItemArrayFiltered[i].MedType){
					case EnumClaimMedType.Dental:
						row.Cells.Add("Dent");
						break;
					case EnumClaimMedType.Medical:
						row.Cells.Add("Med");
						break;
					case EnumClaimMedType.Institutional:
						row.Cells.Add("Inst");
						break;
				}
				if(!claimSendQueueItemArrayFiltered[i].CanSendElect) {
					row.Cells.Add("Paper");
					row.Cells.Add("");
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(ClearinghouseL.GetDescript(claimSendQueueItemArrayFiltered[i].ClearinghouseNum));
					row.Cells.Add(claimSendQueueItemArrayFiltered[i].Warnings);
					if(claimSendQueueItemArrayFiltered[i].MissingData.Contains(Eclaims.GetNoProceduresOnClaimMessage()) && !claimSendQueueItemArrayFiltered[i].MissingData.Contains("and delete this one")) {
						string deleteString=" and delete this one";
						int insertPosition=claimSendQueueItemArrayFiltered[i].MissingData.IndexOf(Eclaims.GetNoProceduresOnClaimMessage())+Eclaims.GetNoProceduresOnClaimMessage().Length;
						claimSendQueueItemArrayFiltered[i].MissingData=claimSendQueueItemArrayFiltered[i].MissingData.Insert(insertPosition,deleteString);
					}
					row.Cells.Add(claimSendQueueItemArrayFiltered[i].MissingData);
				}
				row.Cells.Add(claimSendQueueItemArrayFiltered[i].Ordinal.ToString());
				row.Cells.Add(claimSendQueueItemArrayFiltered[i].ProcedureCodeString);
				row.Tag=claimSendQueueItemArrayFiltered[i].Copy();
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.ScrollValue=oldScrollValue;
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				if(listOldSelectedClaimNums.Contains(((ClaimSendQueueItem)gridMain.ListGridRows[i].Tag).ClaimNum)) {
					gridMain.SetSelected(i,true);//select row
				}
			}
		}

		private void comboHistoryType_SelectionChangeCommitted(object sender,EventArgs e) {
			FillHistory();
		}

		///<summary>Returns a list of claim send queue items based on the filters.</summary>
		private ClaimSendQueueItem[] GetListQueueFiltered() {
			List<long> listClinicNums=new List<long>();
			long customTrackingNum=0;
			if(PrefC.HasClinicsEnabled) {
				listClinicNums=comboClinic.GetListSelected<Clinic>().Select(x => x.ClinicNum).ToList();
			}
			if(comboCustomTracking.SelectedIndex!=0) {
				customTrackingNum=_listDefsClaimCustomTracking[comboCustomTracking.SelectedIndex-1].DefNum;
			}
			List<ClaimSendQueueItem> listClaimSendQueueItems=new List<ClaimSendQueueItem>();
			listClaimSendQueueItems.AddRange(_claimSendQueueItemArrayQueueAll);
			//Remove any non-matches
			//Creating a subset of listClaimSend with all entries c such that c.ClinicNum==clinicNum
			if(PrefC.HasClinicsEnabled) {//Filter by clinic only when clinics are enabled.
				listClaimSendQueueItems=listClaimSendQueueItems.FindAll(c => listClinicNums.Contains(c.ClinicNum));
			}
			if(customTrackingNum>0) {
				//Creating a subset of listClaimSend with all entries c such that c.CustomTracking==customTracking
				listClaimSendQueueItems=listClaimSendQueueItems.FindAll(c => c.CustomTracking==customTrackingNum);
			}
			//Carrier
			if(!string.IsNullOrWhiteSpace(textCarrier.Text)) {
				listClaimSendQueueItems.RemoveAll(x => !x.Carrier.ToLower().Trim().Contains(textCarrier.Text.ToLower().Trim()));
			}
			//Provider
			if(!string.IsNullOrWhiteSpace(textProv.Text)) {
				listClaimSendQueueItems.RemoveAll(x => !Providers.GetAbbr(x.ProvTreat).ToLower().Trim().Contains(textProv.Text.ToLower().Trim()));
			}
			//Procedure
			if(!string.IsNullOrWhiteSpace(textProc.Text)) {
				listClaimSendQueueItems.RemoveAll(x => !x.ProcedureCodeString.ToLower().Trim().Contains(textProc.Text.ToLower().Trim()));
			}
			return listClaimSendQueueItems.ToArray();
		}

		private void gridMain_CellDoubleClick(object sender, ODGridClickEventArgs e){
			int selected=e.Row;
			using FormClaimPrint formClaimPrint=new FormClaimPrint();
			formClaimPrint.PatNum=((ClaimSendQueueItem)gridMain.ListGridRows[selected].Tag).PatNum;
			formClaimPrint.ClaimNum=((ClaimSendQueueItem)gridMain.ListGridRows[selected].Tag).ClaimNum;
			formClaimPrint.DoPrintImmediately=false;
			formClaimPrint.ShowDialog();
			if(formClaimPrint.WasSent && Claims.ReceiveAsNoPaymentIfNeeded(formClaimPrint.ClaimNum)) {
				ClaimL.PromptForPrimaryOrSecondaryClaim(ClaimProcs.RefreshForClaim(formClaimPrint.ClaimNum));
			}
			FillGrid();
			gridMain.SetSelected(selected,true);
			FillHistory();
		}

		private void ToolBarMain_ButtonClick(object sender, OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			ToolBarClick toolBarClick;
			switch(e.Button.Tag.ToString()){
				case "Preview":
					toolBarButPreview_Click();
					break;
				case "Blank":
					//The reason we are using a delegate and BeginInvoke() is because of a Microsoft bug that causes the Print Dialog window to not be in focus			
					//when it comes from a toolbar click.
					//https://social.msdn.microsoft.com/Forums/windows/en-US/681a50b4-4ae3-407a-a747-87fb3eb427fd/first-mouse-click-after-showdialog-hits-the-parent-form?forum=winforms
					toolBarClick=toolBarButBlank_Click;
					this.BeginInvoke(toolBarClick);
					break;
				case "Print":
					toolBarClick=toolBarButPrint_Click;
					this.BeginInvoke(toolBarClick);
					break;
				case "Labels":
					toolBarClick=toolBarButLabels_Click;
					this.BeginInvoke(toolBarClick);
					break;
				case "Eclaims":
					SendEclaimsToClearinghouse(0);
					break;
				case "Validate":
					toolBarButValidate_Click();
					break;
				case "Reports":
					toolBarButReports_Click();
					break;
				case "Outstanding":
					toolBarButOutstanding_Click();
					break;
				case "PayRec":
					toolBarButPayRec_Click();
					break;
				case "SummaryRec":
					toolBarButSummaryRec_Click();
					break;
				case "Refresh":
					toolBarButRefresh_Click();
					break;
				case "ProcsNotBilled":
					FormRpProcNotBilledIns formRpProcNotBilledIns=new FormRpProcNotBilledIns(this);
					formRpProcNotBilledIns.OnPostClaimCreation+=() => RefreshClaimsGrid();//Refresh grid to show any newly created claims.
					formRpProcNotBilledIns.FormClosed+=(s,ea) => { ODEvent.Fired-=formProcNotBilled_GoToChanged; };
					ODEvent.Fired+=formProcNotBilled_GoToChanged;
					formRpProcNotBilledIns.Show();//FormProcSend has a GoTo option and is shown as a non-modal window.
					formRpProcNotBilledIns.BringToFront();
					break;
				case "Close":
					Close();
					break;
			}
		}

		///<summary>Used to fill the grid from extrenal places that spawn FormClaimsSend.</summary>
		public void RefreshClaimsGrid() {
			FillGrid();
		}

		private void formProcNotBilled_GoToChanged(ODEventArgs e) {
			if(e.EventType!=ODEventType.FormProcNotBilled_GoTo) {
				return;
			}
			Patient patient=Patients.GetPat((long)e.Tag);
			GlobalFormOpenDental.PatientSelected(patient,false);
			GlobalFormOpenDental.GotoAccount((long)e.Tag);
		}

		private void toolBarButPreview_Click(){
			using FormClaimPrint formClaimPrint=new FormClaimPrint();
			if(gridMain.SelectedTags<ClaimSendQueueItem>().Count==0){
				MessageBox.Show(Lan.g(this,"Please select a claim first."));
				return;
			}
			if(gridMain.SelectedTags<ClaimSendQueueItem>().Count>1){
				MessageBox.Show(Lan.g(this,"Please select only one claim."));
				return;
			}
			formClaimPrint.PatNum=gridMain.SelectedTag<ClaimSendQueueItem>().PatNum;
			formClaimPrint.ClaimNum=gridMain.SelectedTag<ClaimSendQueueItem>().ClaimNum;
			formClaimPrint.DoPrintImmediately=false;
			formClaimPrint.ShowDialog();
			if(formClaimPrint.WasSent && Claims.ReceiveAsNoPaymentIfNeeded(formClaimPrint.ClaimNum)) {
				ClaimL.PromptForPrimaryOrSecondaryClaim(ClaimProcs.RefreshForClaim(formClaimPrint.ClaimNum));
			}
			FillGrid();
			FillHistory();
		}

		private void toolBarButBlank_Click(){
			FormClaimPrint formClaimPrint=new FormClaimPrint();
			formClaimPrint.DoPrintBlank=true;
			formClaimPrint.PrintImmediate(Lan.g(this,"Blank claim printed"),PrintSituation.Claim,0);
		}

		private void toolBarButPrint_Click(){
			FormClaimPrint formClaimPrint=new FormClaimPrint();
			if(gridMain.SelectedTags<ClaimSendQueueItem>().Count==0){
				for(int i=0;i<gridMain.ListGridRows.Count;i++) {
					ClaimSendQueueItem claimSendQueueItem=(ClaimSendQueueItem)gridMain.ListGridRows[i].Tag;
					if((claimSendQueueItem.ClaimStatus=="W" || claimSendQueueItem.ClaimStatus=="P") && !claimSendQueueItem.CanSendElect){
						gridMain.SetSelected(i,true);
					}
				}
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"No claims were selected.  Print all selected paper claims?")){
					return;
				}
			}
			bool doUsePrinterSettingsForAll=false;
			bool isFirstIteration=true;
			List<long> listClaimNums=new List<long>();
			for(int i=0;i<gridMain.SelectedGridRows.Count;i++) {
				ClaimSendQueueItem claimSendQueueItem=(ClaimSendQueueItem)gridMain.SelectedGridRows[i].Tag;
				formClaimPrint.PatNum=claimSendQueueItem.PatNum;
				formClaimPrint.ClaimNum=claimSendQueueItem.ClaimNum;
				formClaimPrint.ClaimFormCur=null;//so that it will pull from the individual claim or plan.
				if(isFirstIteration && gridMain.SelectedGridRows.Count>1) {
					doUsePrinterSettingsForAll=MsgBox.Show(MsgBoxButtons.YesNo,"Use the same printer settings for all selected claims?");
				}
				if(!formClaimPrint.PrintImmediate(Lan.g(this,"Multiple claims printed"),PrintSituation.Claim,0,(doUsePrinterSettingsForAll && !isFirstIteration))) {
					return;
				}
				Etranss.SetClaimSentOrPrinted(claimSendQueueItem.ClaimNum,claimSendQueueItem.ClaimStatus,claimSendQueueItem.PatNum,0,EtransType.ClaimPrinted,0,Security.CurUser.UserNum);
				if(Claims.ReceiveAsNoPaymentIfNeeded(claimSendQueueItem.ClaimNum)) {
					listClaimNums.Add(claimSendQueueItem.ClaimNum);
				}
				isFirstIteration=false;
			}
			List<ClaimProc> listClaimProcs=ClaimProcs.RefreshForClaims(listClaimNums);
			ClaimL.PromptForPrimaryOrSecondaryClaim(listClaimProcs);
			FillGrid();
			FillHistory();
		}

		private void toolBarButLabels_Click(){
			if(gridMain.SelectedTags<ClaimSendQueueItem>().Count==0){
				MessageBox.Show(Lan.g(this,"Please select a claim first."));
				return;
			}
			//PrintDocument pd=new PrintDocument();//only used to pass printerName
			//if(!PrinterL.SetPrinter(pd,PrintSituation.LabelSingle)){
			//	return;
			//}
			//Carrier carrier;
			Claim claim;
			InsPlan insPlan;
			List<long> listCarrierNums=new List<long>();
			for(int i=0;i<gridMain.SelectedGridRows.Count;i++) {
				ClaimSendQueueItem claimSendQueueItem=(ClaimSendQueueItem)gridMain.SelectedGridRows[i].Tag;
				claim=Claims.GetClaim(claimSendQueueItem.ClaimNum);
				insPlan=InsPlans.GetPlan(claim.PlanNum,new List <InsPlan> ());
				listCarrierNums.Add(insPlan.CarrierNum);
			}
			//carrier=Carriers.GetCarrier(plan.CarrierNum);
			//LabelSingle label=new LabelSingle();
			LabelSingle.PrintCarriers(listCarrierNums);//,pd.PrinterSettings.PrinterName)){
			//	return;
			//}
		}

		private void toolBarButValidate_Click() {
			if(gridMain.ListGridRows.Count==0) {
				return;//No unsent claims to validate.
			}
			//If user has not selected anything prompt to validate all otherwise Refresh and validate as normal
			if(gridMain.SelectedTags<ClaimSendQueueItem>().Count==0) {
				if(!MsgBox.Show(MsgBoxButtons.OKCancel,Lan.g(this,"Validate all claims?"))) {//skip validate if cancel
					return;
				}
				gridMain.SetAll(true);
			}
			RefreshAndValidateSelections();
		}

		private void toolBarButRefresh_Click() {
			FillGrid(true);
		}

		///<summary>Fills grid with updated information, unless all of the selected claims are marked NoBillIns and none of them were deleted.</summary>
		private void RefreshAndValidateSelections() {
			List<ClaimSendQueueItem> listClaimSendQueueItems=new List<ClaimSendQueueItem>();//List of claims needing to be validated.
			//List of claimNums to fetch new ClaimSendQuiteItems
			List<long> listQueueClaimNums=gridMain.SelectedTags<ClaimSendQueueItem>().Select(x => x.ClaimNum).ToList();
			ClaimSendQueueItem[] claimSendQueueItemArrayRefreshItems=Claims.GetQueueList(listQueueClaimNums,0,0);
			int claimAlreadySentCount=0;
			for(int j=0;j<claimSendQueueItemArrayRefreshItems.Length;j++) {//Loop through all the refreshed ClaimSendQueueItems
				for(int k=0;k<_claimSendQueueItemArrayQueueAll.Length;k++) {//Loop through all the ClaimSendQueueItems in the grid's main list
					if(claimSendQueueItemArrayRefreshItems[j].ClaimNum==_claimSendQueueItemArrayQueueAll[k].ClaimNum) {//If you found the matching ClaimSendQueueItem
						if(_claimSendQueueItemArrayQueueAll[k].ClaimStatus=="S" ||  _claimSendQueueItemArrayQueueAll[k].ClaimStatus=="R") {
							claimAlreadySentCount++;
						}
						else {
							_claimSendQueueItemArrayQueueAll[k]=claimSendQueueItemArrayRefreshItems[j];//Refresh the claim in the list
							listClaimSendQueueItems.Add(_claimSendQueueItemArrayQueueAll[k]);//Add to list to be validated again
						}
						break;
					}
				}
			}
			if(claimAlreadySentCount>0) {
				MsgBox.Show(this,"WARNING: Some of the selected claims have already been sent or received.  They will be removed from the grid.");
			}
			if(claimSendQueueItemArrayRefreshItems.Length!=gridMain.SelectedTags<ClaimSendQueueItem>().Count) {
				MsgBox.Show(this,"WARNING: One or more claims were deleted from outside this window.  They will be removed from the grid.");
			}
			if(listClaimSendQueueItems.Count>0) {//At least one claim still exists
				ValidateClaims(listClaimSendQueueItems);//Validate refeshed claims, also fills grid
			}
			else {
				FillGrid(true);//Refresh the grid so that the deleted claims disapear.
			}
		}

		///<Summary>Use clearinghouseNum of 0 to indicate automatic calculation of clearinghouses.</Summary>
		private void SendEclaimsToClearinghouse(long hqClearinghouseNum) {
			if(PrefC.HasClinicsEnabled) {//Clinics is in use
				if(hqClearinghouseNum==0){
					MsgBox.Show(this,"When the Clinics option is enabled, you must use the dropdown list to select the clearinghouse to send to.");
					return;
				}
			}
			Clearinghouse clearinghouseDefault;
			if(hqClearinghouseNum==0){
				clearinghouseDefault=Clearinghouses.GetDefaultDental();
			}
			else{
				clearinghouseDefault=ClearinghouseL.GetClearinghouseHq(hqClearinghouseNum);
			}
			if(clearinghouseDefault!=null && clearinghouseDefault.ISA08=="113504607" && Process.GetProcessesByName("TesiaLink").Length==0){
				if(ODBuild.IsDebug()) {
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"TesiaLink is not started.  Create file anyway?")){
						return;
					}
				}
				else{
					MsgBox.Show(this,"Please start TesiaLink first.");
					return;
				}
			}
			if(gridMain.SelectedTags<ClaimSendQueueItem>().Count==0){//if none are selected
				for(int i=0;i<gridMain.ListGridRows.Count;i++) {//loop through all rows
					ClaimSendQueueItem claimSendQueueItem=(ClaimSendQueueItem)gridMain.ListGridRows[i].Tag;
					if(claimSendQueueItem.CanSendElect) {
						if(hqClearinghouseNum==0) {//they did not use the dropdown list for specific clearinghouse
							//If clearinghouse is zero because they just pushed the button instead of using the dropdown list,
							//then don't check the clearinghouse of each claim.  Just select them if they are electronic.
							gridMain.SetSelected(i,true);
						}
						else {//if they used the dropdown list,
							//then first, try to only select items in the list that match the clearinghouse.
							if(claimSendQueueItem.ClearinghouseNum==hqClearinghouseNum) {
								gridMain.SetSelected(i,true);
							}
						}
					}
				}
				//If they used the dropdown list, and there still aren't any in the list that match the selected clearinghouse
				//then ask user if they want to send all of the electronic ones through this clearinghouse.
				if(hqClearinghouseNum!=0 && gridMain.SelectedTags<ClaimSendQueueItem>().Count==0) {
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Send all e-claims through selected clearinghouse?")) {
						return;
					}
					for(int i=0;i<gridMain.ListGridRows.Count;i++) {//loop through all filtered rows
						ClaimSendQueueItem queueItem=(ClaimSendQueueItem)gridMain.ListGridRows[i].Tag;
						if(queueItem.CanSendElect) {
							gridMain.SetSelected(i,true);//this will include other clearinghouses
						}
					}
				}
				if(gridMain.SelectedTags<ClaimSendQueueItem>().Count==0){//No claims in filtered list
					MsgBox.Show(this,"No claims to send.");
					return;
				}
				if(hqClearinghouseNum!=0) {//if they used the dropdown list to specify clearinghouse
					for(int i=0;i<gridMain.SelectedGridRows.Count;i++) {
						ClaimSendQueueItem claimSendQueueItem=(ClaimSendQueueItem)gridMain.SelectedGridRows[i].Tag;
						Clearinghouse cleainghouserRow=Clearinghouses.GetClearinghouse(claimSendQueueItem.ClearinghouseNum);
						if(clearinghouseDefault.Eformat!=cleainghouserRow.Eformat) {
							MsgBox.Show(this,"The default clearinghouse format does not match the format of the selected clearinghouse.  You may need to change the clearinghouse format.  Or, you may need to add a Payor ID into a clearinghouse.");
							return;
						}
						if(claimSendQueueItem.CanSendElect) {
							//Only change the text to the clearing house name for electronic claims.
							gridMain.SelectedGridRows[i].Cells[clearinghouseIndex].Text=clearinghouseDefault.Description;
						}
					}
					FillGrid(true);
				}
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Send all selected e-claims?")){
					FillGrid();//this changes back any clearinghouse descriptions that we changed manually.
					return;
				}
			}
			else {//some rows were manually selected by the user
				if(hqClearinghouseNum!=0) {//if they used the dropdown list to specify clearinghouse
					for(int i=0;i<gridMain.SelectedGridRows.Count;i++) {
						ClaimSendQueueItem claimSendQueueItem=(ClaimSendQueueItem)gridMain.SelectedGridRows[i].Tag;
						Clearinghouse clearinghouseRow=Clearinghouses.GetClearinghouse(claimSendQueueItem.ClearinghouseNum);
						if(clearinghouseDefault.Eformat!=clearinghouseRow.Eformat) {
							MsgBox.Show(this,"The default clearinghouse format does not match the format of the selected clearinghouse.  You may need to change the clearinghouse format.  Or, you may need to add a Payor ID into a clearinghouse.");
							return;
						}
						if(claimSendQueueItem.CanSendElect) {
							//Only change the text to the clearing house name for electronic claims.
							gridMain.SelectedGridRows[i].Cells[clearinghouseIndex].Text=clearinghouseDefault.Description;//show the changed clearinghouse
						}
					}
				}
			}
			RefreshAndValidateSelections();
			if(gridMain.SelectedTags<ClaimSendQueueItem>().Count==0){//No claims selected after Validation
				MsgBox.Show(this,"No claims to send.");
				return;
			}
			List<ClaimSendQueueItem> listClaimSendQueueItemsSelected=gridMain.SelectedTags<ClaimSendQueueItem>();
			Cursor=Cursors.WaitCursor;
			List<ClaimSendQueueItem> listClaimSendQueueItems=ClaimL.SendClaimSendQueueItems(listClaimSendQueueItemsSelected,hqClearinghouseNum);
			List<long> listClaimNums=new List<long>();
			for(int i=0;i<listClaimSendQueueItems.Count;i++) {
				if(listClaimSendQueueItems[i].ClaimStatus!="S") {
					continue;
				}
				if(Claims.ReceiveAsNoPaymentIfNeeded(listClaimSendQueueItems[i].ClaimNum)) {
					listClaimNums.Add(listClaimSendQueueItems[i].ClaimNum);
				}
			}
			List<ClaimProc> listClaimProcs=ClaimProcs.RefreshForClaims(listClaimNums);
			ClaimL.PromptForPrimaryOrSecondaryClaim(listClaimProcs);
			Cursor=Cursors.Default;
			//Loop through _listQueueAll and remove all items that were sent.
			List<ClaimSendQueueItem> listClaimSendQueueItemsTemps=new List<ClaimSendQueueItem>(_claimSendQueueItemArrayQueueAll);
			for(int i=0;i<listClaimSendQueueItems.Count;i++) {
				if(listClaimSendQueueItems[i].ClaimStatus=="S") {
					//Find the claim in the unfiltered list that was just sent and remove it.
					//(Find the index of listTempQueueItem c where c.ClaimNum is the same as the ClaimNum of the item just sent.)
					listClaimSendQueueItemsTemps.RemoveAt(listClaimSendQueueItemsTemps.FindIndex(c => c.ClaimNum==listClaimSendQueueItems[i].ClaimNum));
					//one securitylog entry for each sent claim
					SecurityLogs.MakeLogEntry(EnumPermType.ClaimSend,listClaimSendQueueItems[i].PatNum,Lan.g(this,"Claim sent from Claims Send Window."),
						listClaimSendQueueItems[i].ClaimNum,listClaimSendQueueItems[i].SecDateTEdit);
				}
			}
			_claimSendQueueItemArrayQueueAll=listClaimSendQueueItemsTemps.ToArray();
			//statuses changed to S in SendBatches
			FillGrid();
			FillHistory();
			//Now, the cool part.  Highlight all the claims that were just sent in the history grid
			for(int i=0;i<listClaimSendQueueItems.Count;i++){
				for(int j=0;j<_tableHistory.Rows.Count;j++){
					long claimNum=PIn.Long(_tableHistory.Rows[j]["ClaimNum"].ToString());
					if(claimNum==listClaimSendQueueItems[i].ClaimNum){
						gridHistory.SetSelected(j,true);
						break;
					}
				}
			}
		}

		private ClaimSendQueueItem ValidateProvidersTerms(ClaimSendQueueItem claimSendQueueItems,List<Claim> listClaims) {
			Claim claim=listClaims.Find(x => x.ClaimNum==claimSendQueueItems.ClaimNum);
			List<long> listInvalidProvs=Providers.GetInvalidProvsByTermDate(new List<long> 
				{ claim.ProvBill,claim.ProvTreat,claim.ProvOrderOverride },claimSendQueueItems.DateService);
			if(listInvalidProvs.Count > 0) {
				if(listInvalidProvs.Contains(claim.ProvBill)) {
					claimSendQueueItems.MissingData+=(claimSendQueueItems.MissingData=="" ? "" : ", ")+"Billing Provider invalid Term Date";
				}
				if(listInvalidProvs.Contains(claim.ProvTreat)) {
					claimSendQueueItems.MissingData+=(claimSendQueueItems.MissingData=="" ? "" : ", ")+"Treating Provider invalid Term Date";
				}
				if(listInvalidProvs.Contains(claim.ProvOrderOverride)) {
					claimSendQueueItems.MissingData+=(claimSendQueueItems.MissingData=="" ? "" : ", ")+"Ordering Provider Override invalid Term Date";
				}
			}
			return claimSendQueueItems;
		} 

		///<summary>Validates all non-validated e-claims passed in.  Directly manipulates the corresponding ClaimSendQueueItem in _arrayQueueAll
		///If any information has changed, the grid will be refreshed and the selected items will remain selected.</summary>
		private void ValidateClaims(List<ClaimSendQueueItem> listClaimSendQueueItems) {
			//Only get a list of non-validated e-claims from the list passed in.
			List<ClaimSendQueueItem> listClaimSendQueueItemsToValidate=listClaimSendQueueItems.FindAll(x => !x.IsValid && x.CanSendElect);
			if(listClaimSendQueueItemsToValidate.Count==0) {
				return;
			}
			Cursor.Current=Cursors.WaitCursor;
			//Loop through and validate all claims.
			Clearinghouse clearinghouseHq=ClearinghouseL.GetClearinghouseHq(listClaimSendQueueItemsToValidate[0].ClearinghouseNum);
			Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
			//Grabs list of claims here to prevent multiple database calls. Needed to extract provnums
			List<Claim> listClaims=Claims.GetClaimsFromClaimNums(listClaimSendQueueItemsToValidate.Select(x => x.ClaimNum).ToList());
			for(int i=0;i<listClaimSendQueueItemsToValidate.Count;i++) {
				listClaimSendQueueItemsToValidate[i]=Eclaims.GetMissingData(clearinghouseClin,listClaimSendQueueItemsToValidate[i]);
				//Checked here instead of Eclaims.GetMissingData as we do not want it check within the FormClaimEdit.
				//Claims with providers who have expired terms should still be able to be resent. Job F4401
				//Because of this, we implement FromClaimEdit and FormClaimSend checks for term dates separately.
				listClaimSendQueueItemsToValidate[i]=ValidateProvidersTerms(listClaimSendQueueItemsToValidate[i],listClaims);
				if(listClaimSendQueueItemsToValidate[i].MissingData=="" && XConnect.IsEnabled(clearinghouseClin)) {
					Claim claim=listClaims.FirstOrDefault(x => x.ClaimNum==listClaimSendQueueItems[i].ClaimNum);
					XConnectWebResponse xConnectWebResponse=null;
					try {
						xConnectWebResponse=XConnect.ValidateClaim(claim);
						if(xConnectWebResponse.messages.Length!=0) {//Errors will go in the messages array of the object
							listClaimSendQueueItemsToValidate[i].MissingData+=(listClaimSendQueueItemsToValidate[i].MissingData==""?"":", ")
								+string.Join("\r\n",xConnectWebResponse.messages);
						}
					}
					catch(Exception ex) {
						listClaimSendQueueItemsToValidate[i].MissingData+=(listClaimSendQueueItemsToValidate[i].MissingData==""?"":", ")
							+"XConnect Validation Call Failed: "+ex.Message;
					}
				}
				if(listClaimSendQueueItemsToValidate[i].MissingData=="") {
					listClaimSendQueueItemsToValidate[i].IsValid=true;
				}
			}
			//Push any changes made to the ClaimSendQueueItems passed in to _arrayQueueAll 
			for(int i=0;i<_claimSendQueueItemArrayQueueAll.Length;i++) {
				ClaimSendQueueItem claimSendQueueItemValidated=listClaimSendQueueItemsToValidate.Find(x => x.ClaimNum==_claimSendQueueItemArrayQueueAll[i].ClaimNum);
				if(claimSendQueueItemValidated!=null) {
					_claimSendQueueItemArrayQueueAll[i]=claimSendQueueItemValidated.Copy();
				}
			}
			FillGrid(true);//Used here to display changes immediately
			Cursor.Current=Cursors.Default;
		}

		private void toolBarButReports_Click() {
			using FormClaimReports formClaimReports=new FormClaimReports();
			formClaimReports.ShowDialog();
			FillHistory();//To show 277s after imported.
		}

		private void toolBarButOutstanding_Click() {
			using FormCanadaOutstandingTransactions formCanadaOutstandingTransactions=new FormCanadaOutstandingTransactions();
			formCanadaOutstandingTransactions.ShowDialog();
		}

		private void toolBarButPayRec_Click() {
			using FormCanadaPaymentReconciliation formCanadaPaymentReconciliation=new FormCanadaPaymentReconciliation();
			formCanadaPaymentReconciliation.ShowDialog();
		}

		private void toolBarButSummaryRec_Click() {
			using FormCanadaSummaryReconciliation formCanadaSummaryReconciliation=new FormCanadaSummaryReconciliation();
			formCanadaSummaryReconciliation.ShowDialog();
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butNextUnsent_Click(object sender,EventArgs e) {
			int indexClinicOffset=0;
			//Check to see if Unassigned/Default is an option in the clinic combo box.
			if(comboClinic.Items.GetAll<Clinic>().Any(x => x.ClinicNum==0)) {
				//We want to skip Unassigned/Default so treat the selected indices as 1-based.
				indexClinicOffset=1;
			}
			int indexClinicNext=-1;
			int indexSelectedMax=comboClinic.SelectedIndices.Max();
			for(int i=0;i<_listNumberOfClaims.Count;i++) {
				int indexClinic=i+indexClinicOffset;
				//Ignore currently selected clinics
				if(comboClinic.SelectedIndices.Contains(indexClinic)) {
					continue;
				}
				//Initialize indexClinicNext to the very first clinic in the list that has unsent claims.
				if(_listNumberOfClaims[i]>0 && indexClinicNext==-1) {
					//Set as a failsafe just in case the "next" clinic with unsent claims falls before the last selected clinic.
					indexClinicNext=indexClinic;
				}
				//Do not consider clinics prior to the last selected clinic after this point.
				if(indexClinic<=indexSelectedMax) {
					continue;
				}
				//This clinic falls after the last selected clinic and it has unsent claims.
				if(_listNumberOfClaims[i]>0) {
					indexClinicNext=indexClinic;
					break;
				}
			}
			if(indexClinicNext>=0) {
				comboClinic.SelectedIndices=ListTools.FromSingle<int>(indexClinicNext);
				FillGrid();
				return;
			}
		}

		private void comboCustomTracking_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillHistory(){
			if(!dateRangePicker.IsValid()) {
				return;
			}
			DateTime dateFrom=dateRangePicker.GetDateTimeFrom();
			DateTime dateTo=dateRangePicker.GetDateTimeTo();
			if(dateRangePicker.HasEmptyDateTimeTo()) {
				dateTo=DateTime.MaxValue;  //Maintains previous implementation behavior by defaulting to DateTime.MaxValue
			}
			List<EtransType> listSelectedEtransTypes=new List<EtransType>();
			for(int i=0;i<comboHistoryType.SelectedIndices.Count;i++) {//Some selected, add only those selected
				int selectedIdx=(int)comboHistoryType.SelectedIndices[i];
				listSelectedEtransTypes.Add(_listEtransTypes[selectedIdx]);
			}
			if(comboHistoryType.SelectedIndices.Count==0) {//None selected.  The user can unselect each option manually.
				listSelectedEtransTypes.AddRange(_listEtransTypes);
			}
			_tableHistory=Etranss.RefreshHistory(dateFrom,dateTo,listSelectedEtransTypes);
			//listQueue=Claims.GetQueueList();
			gridHistory.BeginUpdate();
			gridHistory.Columns.Clear();
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				GridColumn col;
				col=new GridColumn(Lan.g("TableClaimHistory","Patient Name"),120);
				gridHistory.Columns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Carrier Name"),150);
				gridHistory.Columns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Clearinghouse"),90);
				gridHistory.Columns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Date"),70,HorizontalAlignment.Center);
				gridHistory.Columns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Type"),90);
				gridHistory.Columns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","AckCode"),90,HorizontalAlignment.Center);
				gridHistory.Columns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Note"),90);
				gridHistory.Columns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Office#"),90);
				gridHistory.Columns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","User"),80);
				gridHistory.Columns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","# Carriers"),30){ IsWidthDynamic=true };
				gridHistory.Columns.Add(col);
				gridHistory.ListGridRows.Clear();
				GridRow row;
				for(int i=0;i<_tableHistory.Rows.Count;i++) {
					row=new GridRow();
					row.Cells.Add(_tableHistory.Rows[i]["patName"].ToString());
					row.Cells.Add(_tableHistory.Rows[i]["CarrierName"].ToString());
					row.Cells.Add(_tableHistory.Rows[i]["Clearinghouse"].ToString());
					row.Cells.Add(_tableHistory.Rows[i]["dateTimeTrans"].ToString());
					//((DateTime)tableHistory.Rows[i]["DateTimeTrans"]).ToShortDateString());
					//still need to trim the _CA
					row.Cells.Add(_tableHistory.Rows[i]["etype"].ToString());
					row.Cells.Add(_tableHistory.Rows[i]["ack"].ToString());
					row.Cells.Add(_tableHistory.Rows[i]["Note"].ToString());
					row.Cells.Add(_tableHistory.Rows[i]["OfficeSequenceNumber"].ToString());
					Userod user=Userods.GetUser(PIn.Long(_tableHistory.Rows[i]["UserNum"].ToString()));
					row.Cells.Add(user==null ? "" : user.UserName);
					row.Cells.Add(_tableHistory.Rows[i]["CarrierTransCounter"].ToString());
					gridHistory.ListGridRows.Add(row);
				}
			}
			else {
				GridColumn col;
				col=new GridColumn(Lan.g("TableClaimHistory","Patient Name"),130);
				gridHistory.Columns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Carrier Name"),170);
				gridHistory.Columns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Clearinghouse"),90);
				gridHistory.Columns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Date"),70,HorizontalAlignment.Center);
				gridHistory.Columns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Type"),100);
				gridHistory.Columns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","AckCode"),100,HorizontalAlignment.Center);
				gridHistory.Columns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","User"),130);
				gridHistory.Columns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Note"),170){ IsWidthDynamic=true };
				gridHistory.Columns.Add(col);
				gridHistory.ListGridRows.Clear();
				GridRow row;
				for(int i=0;i<_tableHistory.Rows.Count;i++) {
					row=new GridRow();
					row.Cells.Add(_tableHistory.Rows[i]["patName"].ToString());
					row.Cells.Add(_tableHistory.Rows[i]["CarrierName"].ToString());
					row.Cells.Add(_tableHistory.Rows[i]["Clearinghouse"].ToString());
					row.Cells.Add(_tableHistory.Rows[i]["dateTimeTrans"].ToString());
					row.Cells.Add(_tableHistory.Rows[i]["etype"].ToString());
					row.Cells.Add(_tableHistory.Rows[i]["ack"].ToString());
					Userod user=Userods.GetUser(PIn.Long(_tableHistory.Rows[i]["UserNum"].ToString()));
					row.Cells.Add(user==null ? "" : user.UserName);
					row.Cells.Add(_tableHistory.Rows[i]["Note"].ToString());
					gridHistory.ListGridRows.Add(row);
				}
			}
			gridHistory.EndUpdate();
			gridHistory.ScrollToEnd();
		}

		private void panelSplitter_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			_isMouseDownOnSplitter=true;
			_ySplitterOriginal=panelSplitter.Top;
			_yMouseOriginal=panelSplitter.Top+e.Y;
		}

		private void panelSplitter_MouseMove(object sender,System.Windows.Forms.MouseEventArgs e) {
			if(!_isMouseDownOnSplitter)
				return;
			int splitterNewY=_ySplitterOriginal+(panelSplitter.Top+e.Y)-_yMouseOriginal;
			if(splitterNewY<130)//keeps it from going too high
				splitterNewY=130;
			if(splitterNewY>Height-130)//keeps it from going off the bottom edge
				splitterNewY=Height-130;
			panelSplitter.Top=splitterNewY;
			LayoutManager.MoveHeight(gridMain,panelSplitter.Top-gridMain.Top);
			LayoutManager.MoveLocation(panelHistory,new Point(panelHistory.Location.X,panelSplitter.Bottom));
			LayoutManager.MoveHeight(panelHistory,this.ClientSize.Height-panelHistory.Top-1);
		}

		private void panelSplitter_MouseUp(object sender,System.Windows.Forms.MouseEventArgs e) {
			_isMouseDownOnSplitter=false;
		}

		private void dateRangePicker_CalendarSelection(object sender,EventArgs e) {
			FillHistory();
		}

		private void ToolBarHistory_ButtonClick(object sender,ODToolBarButtonClickEventArgs e) {
			switch(e.Button.Tag.ToString()){
				case "Refresh":
					RefreshHistory_Click();
					break;
				case "Undo":
					Undo_Click();
					break;
				case "PrintList":
					//The reason we are using a delegate and BeginInvoke() is because of a Microsoft bug that causes the Print Dialog window to not be in focus
					//when it comes from a toolbar click.
					ToolBarClick toolBarClick=PrintHistory_Click;
					this.BeginInvoke(toolBarClick);
					break;
				case "PrintItem":
					PrintItem_Click();
					break;
				case "OutstandingClaims":
					using(FormRpOutstandingIns formRpOutstandingIns=new FormRpOutstandingIns()) {
						formRpOutstandingIns.ShowDialog();
					}
					break;
			}
		}

		private void RefreshHistory_Click() {
			if(!dateRangePicker.IsValid()) {
				MsgBox.Show(this,"Please fix date entry errors first.");
				return;
			}
			FillHistory();
		}

		private void Undo_Click(){
			if(gridHistory.SelectedIndices.Length==0){
				MsgBox.Show(this,"Please select at least one item first.");
				return;
			}
			if(gridHistory.SelectedIndices.Length>1){//if there are multiple items selected.
				//then they must all be Claim_Ren, ClaimSent, or ClaimPrinted
				EtransType etransType;
				for(int i=0;i<gridHistory.SelectedIndices.Length;i++) {
					etransType=(EtransType)PIn.Long(_tableHistory.Rows[gridHistory.SelectedIndices[i]]["Etype"].ToString());
					if(etransType!=EtransType.Claim_Ren && etransType!=EtransType.ClaimSent && etransType!=EtransType.ClaimPrinted){
						MsgBox.Show(this,"That type of transaction cannot be undone as a group.  Please undo one at a time.");
						return;
					}
				}
			}
			//loop through each selected item, and see if they are allowed to be "undone".
			//at this point, 
			for(int i=0;i<gridHistory.SelectedIndices.Length;i++) {
				if((EtransType)PIn.Long(_tableHistory.Rows[gridHistory.SelectedIndices[i]]["Etype"].ToString())==EtransType.Claim_CA){
					//if a 
				}
				//else if(){
				
				//}
				
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Remove the selected claims from the history list, and change the claim status from 'Sent' back to 'Waiting to Send'?")){
				return;
			}
			for(int i=0;i<gridHistory.SelectedIndices.Length;i++){
				Etranss.Undo(PIn.Long(_tableHistory.Rows[gridHistory.SelectedIndices[i]]["EtransNum"].ToString()));
			}
			FillGrid();
			FillHistory();
		}

		private void PrintHistory_Click() {
			_pagesPrinted=0;
			_isHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd2_PrintPage,
				Lan.g(this,"Claim history list printed"),
				margins:new Margins(0,0,0,100),
				printoutOrigin:PrintoutOrigin.AtMargin,
				printoutOrientation:PrintoutOrientation.Landscape
			);
		}

		private void menuItemHistoryGoToAccount_Click(object sender,EventArgs e) {
			//accessed by right clicking the history grid
			if(gridHistory.SelectedIndices.Length!=1) {
				MsgBox.Show(this,"Please select exactly one item first.");
				return;
			}
			DataRow row=_tableHistory.Rows[gridHistory.GetSelectedIndex()];			
			long patNum=PIn.Long(row["PatNum"].ToString());
			if(patNum==0) {
				MsgBox.Show(this,"Please select an item with a patient.");
				return;
			}
			ClaimSendQueueItem claimSendQueueItem=new ClaimSendQueueItem();
			claimSendQueueItem.PatNum=PIn.Long(row["PatNum"].ToString());
			claimSendQueueItem.ClaimNum=PIn.Long(row["ClaimNum"].ToString());
			ODEvent.Fire(ODEventType.FormClaimSend_GoTo,claimSendQueueItem);
			SendToBack();
		}

		private void gridHistory_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Cursor=Cursors.WaitCursor;
			Etrans etrans=Etranss.GetEtrans(PIn.Long(_tableHistory.Rows[e.Row]["EtransNum"].ToString()));
			if(etrans.Etype==EtransType.StatusNotify_277) {
				using FormEtrans277Edit formEtrans277Edit=new FormEtrans277Edit();
				formEtrans277Edit.EtransCur=etrans;
				formEtrans277Edit.ShowDialog();
				Cursor=Cursors.Default;
				return;//No refresh needed because 277s are not editable, they are read only.
			}
			if(etrans.Etype==EtransType.ERA_835) {
				EtransL.ViewFormForEra(etrans,this);
			}
			else {
				using FormEtransEdit formEtransEdit=new FormEtransEdit();
				formEtransEdit.EtransCur=etrans;
				formEtransEdit.ShowDialog();
				if(formEtransEdit.DialogResult!=DialogResult.OK) {
					Cursor=Cursors.Default;
					return;
				}
			}
			int scroll=gridHistory.ScrollValue;
			FillHistory();
			for(int i=0;i<_tableHistory.Rows.Count;i++){
				if(_tableHistory.Rows[i]["EtransNum"].ToString()==etrans.EtransNum.ToString()){
					gridHistory.SetSelected(i,true);
				}
			}
			gridHistory.ScrollValue=scroll;
			Cursor=Cursors.Default;
		}

		private void ShowRawMessage_Clicked(object sender,System.EventArgs e) {
			//accessed by right clicking on history
			
		}

		private void pd2_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle rectangleBounds=new Rectangle(50,40,1035,800);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=rectangleBounds.Top;
			int centerPos=rectangleBounds.X+rectangleBounds.Width/2;
			#region printHeading
			if(!_isHeadingPrinted) {
				text=Lan.g(this,"Claim History");
				g.DrawString(text,fontHeading,Brushes.Black,centerPos-g.MeasureString(text,fontHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				text=dateRangePicker.GetDateTimeFrom().ToShortDateString()+" "+Lan.g(this,"to")+" "+dateRangePicker.GetDateTimeTo().ToShortDateString();
				g.DrawString(text,fontSubHeading,Brushes.Black,centerPos-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=20;
				_isHeadingPrinted=true;
				_heightHeadingPrint=yPos;
			}
			#endregion
			Rectangle rectangleOriginalBounds=gridHistory.Bounds; //Store the gridHistory bounds to revert after printing.
			LayoutManager.MoveSize(gridHistory,new Size(LayoutManager.Scale(1035),gridHistory.Height)); //Adjust gridHistory so that no columns are cut off when printing.
			yPos=gridHistory.PrintPage(g,_pagesPrinted,rectangleBounds,_heightHeadingPrint);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			LayoutManager.MoveSize(gridHistory,new Size(rectangleOriginalBounds.Width,rectangleOriginalBounds.Height));//Revert gridHistory to original bounds.
			g.Dispose();
		}

		private void PrintItem_Click(){
			//not currently accessible
			if(gridHistory.ListGridRows.Count==0){
				MsgBox.Show(this,"There are no items to print.");
				return;
			}
			if(gridHistory.SelectedIndices.Length==0){
				if(ODBuild.IsDebug()) {
					gridHistory.SetSelected(0,true);//saves you a click when testing
				}
				else {
					MsgBox.Show(this,"Please select at least one item first.");
					return;
				}
			}
			//does not yet handle multiple selections
			Etrans etrans=Etranss.GetEtrans(PIn.Long(_tableHistory.Rows[gridHistory.SelectedIndices[0]]["EtransNum"].ToString()));
			new FormCCDPrint(etrans,EtransMessageTexts.GetMessageText(etrans.EtransMessageTextNum),false);//Show the form and allow the user to print manually if desired.
			//MessageBox.Show(etrans.MessageText);
		}

		private void gridMain_LocationChanged(object sender, EventArgs e)
		{

		}
	}
}







