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
		///<summary>final list of eclaims(as Claim.ClaimNum) to send</summary>
		public static ArrayList eClaimList;
		///<summary>The list of all claims regardless of any filters.  Filled on load.  Make sure to update this list with any updates (e.g. after validating claims)</summary>
		private ClaimSendQueueItem[] _arrayQueueAll;
		///<summary></summary>
		public long GotoPatNum;
		///<summary></summary>
		public long GotoClaimNum;
		private DataTable tableHistory;
		private int pagesPrinted;
		bool MouseIsDownOnSplitter;
		int SplitterOriginalY;
		int OriginalMouseY;
		bool headingPrinted;
		int headingPrintH;
		private List<EtransType> _listCurEtransTypes;
		//private ContextMenu contextMenuHist;
		private List<Clinic> _listClinics;
		///<summary>Represents the number of unsent claims per clinic. This is a 1:1 list with _listClinics.</summary>
		private List<int> _listNumberOfClaims;
		private List<Clearinghouse> _listClearinghouses;
		private List<Def> _listClaimCustomTrackingDefs;
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
			_arrayQueueAll=Claims.GetQueueList(0,0,0);
			_arrayQueueAll.ForEach(x => x.MissingData="(validated when sending)");
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
			_listClaimCustomTrackingDefs=Defs.GetDefsForCategory(DefCat.ClaimCustomTracking,true);
			if(_listClaimCustomTrackingDefs.Count==0){
				labelCustomTracking.Visible=false;
				comboCustomTracking.Visible=false;
			}
			else{
				for(int i=0;i<_listClaimCustomTrackingDefs.Count;i++) {
					comboCustomTracking.Items.Add(_listClaimCustomTrackingDefs[i].ItemName);
				}
			}
			if(PrefC.RandomKeys && PrefC.HasClinicsEnabled){//using random keys and clinics
				//Does not pull in reports automatically, because they could easily get assigned to the wrong clearinghouse
			}
			else{
				using FormClaimReports FormC=new FormClaimReports(); //the currently selected clinic is what the combobox defaults to.
				FormC.AutomaticMode=true;
				FormC.ShowDialog();
			}
			FillGrid(isRefreshRequired:false);//no need to refresh, we just got the list from the db (_arrayQueueAll above)
			//Validate all claims if the preference is enabled.
			if(PrefC.GetBool(PrefName.ClaimsSendWindowValidatesOnLoad)) {
				//This can be very slow if there are lots of claims to validate.
				ValidateClaims(_arrayQueueAll.ToList());
			}
			dateRangePicker.SetDateTimeFrom(DateTime.Today.AddDays(-7));
			dateRangePicker.SetDateTimeTo(DateTime.Today);
			_listCurEtransTypes=new List<EtransType>();
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				_listCurEtransTypes.Add(EtransType.ClaimPrinted);
				_listCurEtransTypes.Add(EtransType.Claim_CA);
				_listCurEtransTypes.Add(EtransType.Claim_Ren);
				_listCurEtransTypes.Add(EtransType.Eligibility_CA);
				_listCurEtransTypes.Add(EtransType.ClaimReversal_CA);
				_listCurEtransTypes.Add(EtransType.Predeterm_CA);
				_listCurEtransTypes.Add(EtransType.RequestOutstand_CA);
				_listCurEtransTypes.Add(EtransType.RequestSumm_CA);
				_listCurEtransTypes.Add(EtransType.RequestPay_CA);
				_listCurEtransTypes.Add(EtransType.ClaimCOB_CA);
				_listCurEtransTypes.Add(EtransType.Claim_Ramq);
				_listCurEtransTypes.Add(EtransType.TextReport);
				_listCurEtransTypes.Add(EtransType.ItransNcpl);
			}
			else {//United States
				_listCurEtransTypes.Add(EtransType.ClaimSent);
				_listCurEtransTypes.Add(EtransType.ClaimPrinted);
				_listCurEtransTypes.Add(EtransType.Claim_Ren);
				_listCurEtransTypes.Add(EtransType.StatusNotify_277);
				_listCurEtransTypes.Add(EtransType.TextReport);
				_listCurEtransTypes.Add(EtransType.ERA_835);
				_listCurEtransTypes.Add(EtransType.Ack_Interchange);
			}
			List<int> listSelectedItems=new List<int>();
			for(int i=0;i<_listCurEtransTypes.Count;i++) {
				comboHistoryType.Items.Add(_listCurEtransTypes[i].ToString());
				comboHistoryType.SetSelected(i,true);
			}
			FillHistory();
			SetFilterControlsAndAction(() => FillGrid(isRefreshRequired: false),500,textCarrier,textProv,textProc);
		}

		public void FillClinicsList(long claimCustomTracking) {
			int previousSelection=-1;
			if(comboClinic.SelectedIndex!=-1) {//Only -1 the first time this method is run.
				previousSelection=comboClinic.SelectedIndex;
			}
			comboClinic.Items.Clear();
			_listNumberOfClaims.Clear();
			int unassignedOffset=0;
			if(!Security.CurUser.ClinicIsRestricted) {
				comboClinic.Items.Add(Lan.g(this,"Unassigned/Default"));
				comboClinic.SelectedIndex=0;
				unassignedOffset=1;
			}
			for(int i=0;i<_listClinics.Count;i++) {
				_listNumberOfClaims.Add(0);
				for(int j=0;j<_arrayQueueAll.Length;j++) {
					if(_arrayQueueAll[j].ClinicNum==_listClinics[i].ClinicNum) {
						if(claimCustomTracking==0 || _arrayQueueAll[j].CustomTracking==claimCustomTracking) {
							_listNumberOfClaims[i]=_listNumberOfClaims[i]+1;
						}
					}
				}
				comboClinic.Items.Add(_listClinics[i].Abbr+"  ("+_listNumberOfClaims[i]+")");
				int curIndex=i+unassignedOffset;
				if(_listClinics[i].ClinicNum==Clinics.ClinicNum) {
					comboClinic.SelectedIndex=curIndex;
				}
			}
			if(previousSelection!=-1) {
				comboClinic.SelectedIndex=previousSelection;
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
			menuItemDeleteClaim.Enabled=Security.IsAuthorized(Permissions.ClaimDelete,true);
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
			ClaimSendQueueItem queueItem=gridMain.SelectedTag<ClaimSendQueueItem>();
			if(queueItem==null) {
				return;
			}
			if(queueItem.ProcedureCodeString=="") {
				Claim claim=Claims.GetClaim(queueItem.ClaimNum);
				List<ClaimProc> listClaimProcs=ClaimProcs.RefreshForClaim(claim.ClaimNum);
				if(listClaimProcs.Any(x => ListTools.In(x.Status,ClaimProcs.GetInsPaidStatuses()))) {
					MsgBox.Show(this,"Cannot delete, there are claim procedures attached to this claim.");
					return;
				}
				ClaimProcs.DeleteMany(listClaimProcs);
				List<long> listEtrans835AttachNums=Etrans835Attaches.GetForClaimNums(claim.ClaimNum).Select(x => x.Etrans835AttachNum).ToList();
				Claims.Delete(claim,listEtrans835AttachNums);
				SecurityLogs.MakeLogEntry(Permissions.ClaimDelete,claim.PatNum,gridMain.SelectedTag<ClaimSendQueueItem>().PatName
					+", "+Lan.g(this,"Date Entry")+": "+claim.SecDateEntry.ToShortDateString()
					+", "+Lan.g(this,"Date of Service")+": "+claim.DateService.ToShortDateString(),
					queueItem.ClaimNum,claim.SecDateTEdit);
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
				long claimCustomTracking=0;
				if(comboCustomTracking.SelectedIndex!=0) {
					claimCustomTracking=Defs.GetDefsForCategory(DefCat.ClaimCustomTracking,true)[comboCustomTracking.SelectedIndex-1].DefNum;
				}
				FillClinicsList(claimCustomTracking);
			}
			int oldScrollValue=0;
			List<long> listOldSelectedClaimNums=new List<long>();
			if(rememberSelection) {
				oldScrollValue=gridMain.ScrollValue;
				foreach(ClaimSendQueueItem queItem in gridMain.SelectedTags<ClaimSendQueueItem>()) {
					listOldSelectedClaimNums.Add(queItem.ClaimNum);
				}
			}
			if(isRefreshRequired) {
				ClaimSendQueueItem[] arrayClaimQueue=Claims.GetQueueList(0,0,0);//Get fresh new "all" list from db.
				for(int i=0;i<arrayClaimQueue.Length;i++) {
					//If any data in the new list needs to be refreshed because something changed, refresh it.
					//At this point, _arrayQueueAll is the old list of all claims.
					for(int j=0;j<_arrayQueueAll.Length;j++) {//Go through the old list of all claims.
						if(arrayClaimQueue[i].ClaimNum==_arrayQueueAll[j].ClaimNum) {//The same claim is in both the old and new "all" lists.
							arrayClaimQueue[i]=_arrayQueueAll[j];//Keep the same exact queue item as before so we can maintain the MissingData, etc.
						}
					}
					if(arrayClaimQueue[i].MissingData==null) {//Can only be null if the claim was not in the old "all" list.  For example when undo.
						arrayClaimQueue[i].MissingData="(validated when sending)";
					}
				}
				_arrayQueueAll=arrayClaimQueue;
			}
			//Get filtered list from list all
			ClaimSendQueueItem[] arrayQueueFiltered=GetListQueueFiltered();//We update the class wide variable because it is used in double clicking and other events.
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableQueue","DateService"),75,HorizontalAlignment.Center);//new column
			col.SortingStrategy=GridSortingStrategy.DateParse;
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableQueue","Patient Name"),120);//was 190
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableQueue","Carrier Name"),200);//was 220, before that was 100 but was too small.  In Insurance Plans window is 140.
			gridMain.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g("TableQueue","Clinic"),80);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g("TableQueue","Provider"),55);//Just large enough to show the title
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableQueue","M/D"),30);//Just large enough to hold 4 characters (see below)
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableQueue","Clearinghouse"),85);//Just large enough for the title
			gridMain.ListGridColumns.Add(col);
			clearinghouseIndex=gridMain.ListGridColumns.Count-1;
			col=new GridColumn(Lan.g("TableQueue","Warnings"),120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableQueue","Missing Info"),300);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableQueue","ProcCodes"),120){ IsWidthDynamic=true };
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(ClaimSendQueueItem queueItem in arrayQueueFiltered){
				row=new GridRow();
				row.Cells.Add(queueItem.DateService.ToShortDateString());
				row.Cells.Add(queueItem.PatName);
				row.Cells.Add(queueItem.Carrier);
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(queueItem.ClinicNum));
				}
				row.Cells.Add(Providers.GetAbbr(queueItem.ProvTreat));
				switch(queueItem.MedType){
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
				if(!queueItem.CanSendElect) {
					row.Cells.Add("Paper");
					row.Cells.Add("");
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(ClearinghouseL.GetDescript(queueItem.ClearinghouseNum));
					row.Cells.Add(queueItem.Warnings);
					if(queueItem.MissingData.Contains(Eclaims.GetNoProceduresOnClaimMessage()) && !queueItem.MissingData.Contains("and delete this one")) {
						string deleteString=" and delete this one";
						int insertPosition=queueItem.MissingData.IndexOf(Eclaims.GetNoProceduresOnClaimMessage())+Eclaims.GetNoProceduresOnClaimMessage().Length;
						queueItem.MissingData=queueItem.MissingData.Insert(insertPosition,deleteString);
					}
					row.Cells.Add(queueItem.MissingData);
				}
				row.Cells.Add(queueItem.ProcedureCodeString);
				row.Tag=queueItem.Copy();
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.ScrollValue=oldScrollValue;
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				if(ListTools.In(((ClaimSendQueueItem)gridMain.ListGridRows[i].Tag).ClaimNum,listOldSelectedClaimNums)) {
					gridMain.SetSelected(i,true);//select row
				}
			}
		}

		private void comboHistoryType_SelectionChangeCommitted(object sender,EventArgs e) {
			FillHistory();
		}

		///<summary>Returns a list of claim send queue items based on the filters.</summary>
		private ClaimSendQueueItem[] GetListQueueFiltered() {
			long clinicNum=0;
			long customTracking=0;
			if(PrefC.HasClinicsEnabled) {
				if(Security.CurUser.ClinicIsRestricted) {//If the user is restricted to specific clinics (has no Unassigned/Default option)
					clinicNum=_listClinics[comboClinic.SelectedIndex].ClinicNum;
				}
				else if(comboClinic.SelectedIndex!=0) {//If not restricted to specific clinics and not selecting Unassigned/Default
					clinicNum=_listClinics[comboClinic.SelectedIndex-1].ClinicNum;
				}
			}
			if(comboCustomTracking.SelectedIndex!=0) {
				customTracking=_listClaimCustomTrackingDefs[comboCustomTracking.SelectedIndex-1].DefNum;
			}
			List<ClaimSendQueueItem> listClaimSend=new List<ClaimSendQueueItem>();
			listClaimSend.AddRange(_arrayQueueAll);
			//Remove any non-matches
			//Creating a subset of listClaimSend with all entries c such that c.ClinicNum==clinicNum
			if(PrefC.HasClinicsEnabled) {//Filter by clinic only when clinics are enabled.
				listClaimSend=listClaimSend.FindAll(c => c.ClinicNum==clinicNum);
			}
			if(customTracking>0) {
				//Creating a subset of listClaimSend with all entries c such that c.CustomTracking==customTracking
				listClaimSend=listClaimSend.FindAll(c => c.CustomTracking==customTracking);
			}
			//Carrier
			if(!string.IsNullOrWhiteSpace(textCarrier.Text)) {
				listClaimSend.RemoveAll(x => !x.Carrier.ToLower().Trim().Contains(textCarrier.Text.ToLower().Trim()));
			}
			//Provider
			if(!string.IsNullOrWhiteSpace(textProv.Text)) {
				listClaimSend.RemoveAll(x => !Providers.GetAbbr(x.ProvTreat).ToLower().Trim().Contains(textProv.Text.ToLower().Trim()));
			}
			//Procedure
			if(!string.IsNullOrWhiteSpace(textProc.Text)) {
				listClaimSend.RemoveAll(x => !x.ProcedureCodeString.ToLower().Trim().Contains(textProc.Text.ToLower().Trim()));
			}
			return listClaimSend.ToArray();
		}

		private void gridMain_CellDoubleClick(object sender, ODGridClickEventArgs e){
			int selected=e.Row;
			using FormClaimPrint FormCP=new FormClaimPrint();
			FormCP.PatNumCur=((ClaimSendQueueItem)gridMain.ListGridRows[selected].Tag).PatNum;
			FormCP.ClaimNumCur=((ClaimSendQueueItem)gridMain.ListGridRows[selected].Tag).ClaimNum;
			FormCP.PrintImmediately=false;
			FormCP.ShowDialog();
			FillGrid();
			gridMain.SetSelected(selected,true);
			FillHistory();
		}

		private void ToolBarMain_ButtonClick(object sender, OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			ToolBarClick toolClick;
			switch(e.Button.Tag.ToString()){
				case "Preview":
					toolBarButPreview_Click();
					break;
				case "Blank":
					//The reason we are using a delegate and BeginInvoke() is because of a Microsoft bug that causes the Print Dialog window to not be in focus			
					//when it comes from a toolbar click.
					//https://social.msdn.microsoft.com/Forums/windows/en-US/681a50b4-4ae3-407a-a747-87fb3eb427fd/first-mouse-click-after-showdialog-hits-the-parent-form?forum=winforms
					toolClick=toolBarButBlank_Click;
					this.BeginInvoke(toolClick);
					break;
				case "Print":
					toolClick=toolBarButPrint_Click;
					this.BeginInvoke(toolClick);
					break;
				case "Labels":
					toolClick=toolBarButLabels_Click;
					this.BeginInvoke(toolClick);
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
					FormRpProcNotBilledIns FormProc=new FormRpProcNotBilledIns();
					FormProc.OnPostClaimCreation+=() => RefreshClaimsGrid();//Refresh grid to show any newly created claims.
					FormProc.FormClosed+=(s,ea) => { ODEvent.Fired-=formProcNotBilled_GoToChanged; };
					ODEvent.Fired+=formProcNotBilled_GoToChanged;
					FormProc.Show();//FormProcSend has a GoTo option and is shown as a non-modal window.
					FormProc.BringToFront();
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
			Patient pat=Patients.GetPat((long)e.Tag);
			FormOpenDental.S_Contr_PatientSelected(pat,false);
			GotoModule.GotoAccount((long)e.Tag);
		}

		private void toolBarButPreview_Click(){
			using FormClaimPrint FormCP=new FormClaimPrint();
			if(gridMain.SelectedTags<ClaimSendQueueItem>().Count==0){
				MessageBox.Show(Lan.g(this,"Please select a claim first."));
				return;
			}
			if(gridMain.SelectedTags<ClaimSendQueueItem>().Count>1){
				MessageBox.Show(Lan.g(this,"Please select only one claim."));
				return;
			}
			FormCP.PatNumCur=gridMain.SelectedTag<ClaimSendQueueItem>().PatNum;
			FormCP.ClaimNumCur=gridMain.SelectedTag<ClaimSendQueueItem>().ClaimNum;
			FormCP.PrintImmediately=false;
			FormCP.ShowDialog();
			FillGrid();
			FillHistory();
		}

		private void toolBarButBlank_Click(){
			FormClaimPrint FormCP=new FormClaimPrint();
			FormCP.DoPrintBlank=true;
			FormCP.PrintImmediate(Lan.g(this,"Blank claim printed"),PrintSituation.Claim,0);
		}

		private void toolBarButPrint_Click(){
			FormClaimPrint FormCP=new FormClaimPrint();
			if(gridMain.SelectedTags<ClaimSendQueueItem>().Count==0){
				for(int i=0;i<gridMain.ListGridRows.Count;i++) {
					ClaimSendQueueItem queueItem=(ClaimSendQueueItem)gridMain.ListGridRows[i].Tag;
					if((queueItem.ClaimStatus=="W" || queueItem.ClaimStatus=="P") && !queueItem.CanSendElect){
						gridMain.SetSelected(i,true);
					}
				}
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"No claims were selected.  Print all selected paper claims?")){
					return;
				}
			}
			bool doUsePrinterSettingsForAll=false;
			bool isFirstIteration=true;
			foreach(GridRow row in gridMain.SelectedGridRows) {
				ClaimSendQueueItem queueItem=(ClaimSendQueueItem)row.Tag;
				FormCP.PatNumCur=queueItem.PatNum;
				FormCP.ClaimNumCur=queueItem.ClaimNum;
				FormCP.ClaimFormCur=null;//so that it will pull from the individual claim or plan.
				if(isFirstIteration && gridMain.SelectedGridRows.Count>1) {
					doUsePrinterSettingsForAll=MsgBox.Show(MsgBoxButtons.YesNo,"Use the same printer settings for all selected claims?");
				}
				if(!FormCP.PrintImmediate(Lan.g(this,"Multiple claims printed"),PrintSituation.Claim,0,(doUsePrinterSettingsForAll && !isFirstIteration))) {
					return;
				}
				Etranss.SetClaimSentOrPrinted(queueItem.ClaimNum,queueItem.PatNum,0,EtransType.ClaimPrinted,0,Security.CurUser.UserNum);
				isFirstIteration=false;
			}
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
			InsPlan plan;
			List<long> carrierNums=new List<long>();
			foreach(GridRow row in gridMain.SelectedGridRows) {
				ClaimSendQueueItem queueItem=(ClaimSendQueueItem)row.Tag;
				claim=Claims.GetClaim(queueItem.ClaimNum);
				plan=InsPlans.GetPlan(claim.PlanNum,new List <InsPlan> ());
				carrierNums.Add(plan.CarrierNum);
			}
			//carrier=Carriers.GetCarrier(plan.CarrierNum);
			//LabelSingle label=new LabelSingle();
			LabelSingle.PrintCarriers(carrierNums);//,pd.PrinterSettings.PrinterName)){
			//	return;
			//}
		}

		private void toolBarButValidate_Click() {
			if(gridMain.SelectedTags<ClaimSendQueueItem>().Count==0) {
				MessageBox.Show(Lan.g(this,"Please select one or more claims first."));
				return;
			}
			RefreshAndValidateSelections();
		}

		private void toolBarButRefresh_Click() {
			FillGrid(true);
		}

		///<summary>Fills grid with updated information, unless all of the selected claims are marked NoBillIns and none of them were deleted.</summary>
		private void RefreshAndValidateSelections() {
			List<ClaimSendQueueItem> listQueueItems=new List<ClaimSendQueueItem>();//List of claims needing to be validated.
			//List of claimNums to fetch new ClaimSendQuiteItems
			List<long> listQueueClaimNums=gridMain.SelectedTags<ClaimSendQueueItem>().Select(x => x.ClaimNum).ToList();
			ClaimSendQueueItem[] arrayRefreshQueueItems=Claims.GetQueueList(listQueueClaimNums,0,0);
			int claimAlreadySentCount=0;
			for(int j=0;j<arrayRefreshQueueItems.Length;j++) {//Loop through all the refreshed ClaimSendQueueItems
				for(int k=0;k<_arrayQueueAll.Length;k++) {//Loop through all the ClaimSendQueueItems in the grid's main list
					if(arrayRefreshQueueItems[j].ClaimNum==_arrayQueueAll[k].ClaimNum) {//If you found the matching ClaimSendQueueItem
						if(_arrayQueueAll[k].ClaimStatus=="S" ||  _arrayQueueAll[k].ClaimStatus=="R") {
							claimAlreadySentCount++;
						}
						else {
							_arrayQueueAll[k]=arrayRefreshQueueItems[j];//Refresh the claim in the list
							listQueueItems.Add(_arrayQueueAll[k]);//Add to list to be validated again
						}
						break;
					}
				}
			}
			if(claimAlreadySentCount>0) {
				MsgBox.Show(this,"WARNING: Some of the selected claims have already been sent or received.  They will be removed from the grid.");
			}
			if(arrayRefreshQueueItems.Length!=gridMain.SelectedTags<ClaimSendQueueItem>().Count) {
				MsgBox.Show(this,"WARNING: One or more claims were deleted from outside this window.  They will be removed from the grid.");
			}
			if(listQueueItems.Count>0) {//At least one claim still exists
				ValidateClaims(listQueueItems);//Validate refeshed claims, also fills grid
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
			Clearinghouse clearDefault;
			if(hqClearinghouseNum==0){
				clearDefault=Clearinghouses.GetDefaultDental();
			}
			else{
				clearDefault=ClearinghouseL.GetClearinghouseHq(hqClearinghouseNum);
			}
			if(clearDefault!=null && clearDefault.ISA08=="113504607" && Process.GetProcessesByName("TesiaLink").Length==0){
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
					ClaimSendQueueItem queueItem=(ClaimSendQueueItem)gridMain.ListGridRows[i].Tag;
					if(queueItem.CanSendElect) {
						if(hqClearinghouseNum==0) {//they did not use the dropdown list for specific clearinghouse
							//If clearinghouse is zero because they just pushed the button instead of using the dropdown list,
							//then don't check the clearinghouse of each claim.  Just select them if they are electronic.
							gridMain.SetSelected(i,true);
						}
						else {//if they used the dropdown list,
							//then first, try to only select items in the list that match the clearinghouse.
							if(queueItem.ClearinghouseNum==hqClearinghouseNum) {
								gridMain.SetSelected(i,true);
							}
						}
					}
				}
				//If they used the dropdown list, and there still aren't any in the list that match the selected clearinghouse
				//then ask user if they want to send all of the electronic ones through this clearinghouse.
				if(hqClearinghouseNum!=0 && gridMain.SelectedTags<ClaimSendQueueItem>().Count==0) {
					if(comboClinic.SelectedIndex==0) {
						MsgBox.Show(this,"Please filter by clinic first.");
						return;
					}
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
					foreach(GridRow row in gridMain.SelectedGridRows){
						ClaimSendQueueItem queueItem=(ClaimSendQueueItem)row.Tag;
						Clearinghouse clearRow=Clearinghouses.GetClearinghouse(queueItem.ClearinghouseNum);
						if(clearDefault.Eformat!=clearRow.Eformat) {
							MsgBox.Show(this,"The default clearinghouse format does not match the format of the selected clearinghouse.  You may need to change the clearinghouse format.  Or, you may need to add a Payor ID into a clearinghouse.");
							return;
						}
						if(queueItem.CanSendElect) {
							//Only change the text to the clearing house name for electronic claims.
							row.Cells[clearinghouseIndex].Text=clearDefault.Description;
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
					foreach(GridRow row in gridMain.SelectedGridRows){
						ClaimSendQueueItem queueItem=(ClaimSendQueueItem)row.Tag;
						Clearinghouse clearRow=Clearinghouses.GetClearinghouse(queueItem.ClearinghouseNum);
						if(clearDefault.Eformat!=clearRow.Eformat) {
							MsgBox.Show(this,"The default clearinghouse format does not match the format of the selected clearinghouse.  You may need to change the clearinghouse format.  Or, you may need to add a Payor ID into a clearinghouse.");
							return;
						}
						if(queueItem.CanSendElect) {
							//Only change the text to the clearing house name for electronic claims.
							row.Cells[clearinghouseIndex].Text=clearDefault.Description;//show the changed clearinghouse
						}
					}
				}
			}
			RefreshAndValidateSelections();
			if(gridMain.SelectedTags<ClaimSendQueueItem>().Count==0){//No claims selected after Validation
				MsgBox.Show(this,"No claims to send.");
				return;
			}
			List<ClaimSendQueueItem> listSelectedClaimSendQueueItems=gridMain.SelectedTags<ClaimSendQueueItem>();
			Cursor=Cursors.WaitCursor;
			List<ClaimSendQueueItem> queueItems=ClaimL.SendClaimSendQueueItems(listSelectedClaimSendQueueItems,hqClearinghouseNum);
			Cursor=Cursors.Default;
			//Loop through _listQueueAll and remove all items that were sent.
			List<ClaimSendQueueItem> listTempQueueItem=new List<ClaimSendQueueItem>(_arrayQueueAll);
			for(int i=0;i<queueItems.Count;i++) {
				if(queueItems[i].ClaimStatus=="S") {
					//Find the claim in the unfiltered list that was just sent and remove it.
					//(Find the index of listTempQueueItem c where c.ClaimNum is the same as the ClaimNum of the item just sent.)
					listTempQueueItem.RemoveAt(listTempQueueItem.FindIndex(c => c.ClaimNum==queueItems[i].ClaimNum));
					//one securitylog entry for each sent claim
					SecurityLogs.MakeLogEntry(Permissions.ClaimSend,queueItems[i].PatNum,Lan.g(this,"Claim sent from Claims Send Window."),
						queueItems[i].ClaimNum,queueItems[i].SecDateTEdit);
				}
			}
			_arrayQueueAll=listTempQueueItem.ToArray();
			//statuses changed to S in SendBatches
			FillGrid();
			FillHistory();
			//Now, the cool part.  Highlight all the claims that were just sent in the history grid
			for(int i=0;i<queueItems.Count;i++){
				for(int j=0;j<tableHistory.Rows.Count;j++){
					long claimNum=PIn.Long(tableHistory.Rows[j]["ClaimNum"].ToString());
					if(claimNum==queueItems[i].ClaimNum){
						gridHistory.SetSelected(j,true);
						break;
					}
				}
			}
		}

		private ClaimSendQueueItem ValidateProvidersTerms(ClaimSendQueueItem queueItem,List<Claim> listClaims) {
			Claim claim=listClaims.Find(x => x.ClaimNum==queueItem.ClaimNum);
			List<long> listInvalidProvs=Providers.GetInvalidProvsByTermDate(new List<long> 
				{ claim.ProvBill,claim.ProvTreat,claim.ProvOrderOverride },queueItem.DateService);
			if(listInvalidProvs.Count > 0) {
				if(listInvalidProvs.Contains(claim.ProvBill)) {
					queueItem.MissingData+=(queueItem.MissingData=="" ? "" : ", ")+"Billing Provider invalid Term Date";
				}
				if(listInvalidProvs.Contains(claim.ProvTreat)) {
					queueItem.MissingData+=(queueItem.MissingData=="" ? "" : ", ")+"Treating Provider invalid Term Date";
				}
				if(listInvalidProvs.Contains(claim.ProvOrderOverride)) {
					queueItem.MissingData+=(queueItem.MissingData=="" ? "" : ", ")+"Ordering Provider Override invalid Term Date";
				}
			}
			return queueItem;
		} 

		///<summary>Validates all non-validated e-claims passed in.  Directly manipulates the corresponding ClaimSendQueueItem in _arrayQueueAll
		///If any information has changed, the grid will be refreshed and the selected items will remain selected.</summary>
		private void ValidateClaims(List<ClaimSendQueueItem> listClaimSendQueueItems) {
			//Only get a list of non-validated e-claims from the list passed in.
			List<ClaimSendQueueItem> listClaimsToValidate=listClaimSendQueueItems.FindAll(x => !x.IsValid && x.CanSendElect);
			if(listClaimsToValidate.Count==0) {
				return;
			}
			Cursor.Current=Cursors.WaitCursor;
			//Loop through and validate all claims.
			Clearinghouse clearinghouseHq=ClearinghouseL.GetClearinghouseHq(listClaimsToValidate[0].ClearinghouseNum);
			Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
			//Grabs list of claims here to prevent multiple database calls. Needed to extract provnums
			List<Claim> listClaims=Claims.GetClaimsFromClaimNums(listClaimsToValidate.Select(x => x.ClaimNum).ToList());
			for(int i=0;i<listClaimsToValidate.Count;i++) {
				listClaimsToValidate[i]=Eclaims.GetMissingData(clearinghouseClin,listClaimsToValidate[i]);
				//Checked here instead of Eclaims.GetMissingData as we do not want it check within the FormClaimEdit.
				//Claims with providers who have expired terms should still be able to be resent. Job F4401
				//Because of this, we implement FromClaimEdit and FormClaimSend checks for term dates separately.
				listClaimsToValidate[i]=ValidateProvidersTerms(listClaimsToValidate[i],listClaims);
				if(listClaimsToValidate[i].MissingData=="") {
					listClaimsToValidate[i].IsValid=true;
				}
			}
			//Push any changes made to the ClaimSendQueueItems passed in to _arrayQueueAll 
			for(int i=0;i<_arrayQueueAll.Length;i++) {
				ClaimSendQueueItem validatedClaimSendQueueItem=listClaimsToValidate.Find(x => x.ClaimNum==_arrayQueueAll[i].ClaimNum);
				if(validatedClaimSendQueueItem!=null) {
					_arrayQueueAll[i]=validatedClaimSendQueueItem.Copy();
				}
			}
			FillGrid(true);//Used here to display changes immediately
			Cursor.Current=Cursors.Default;
		}

		private void toolBarButReports_Click() {
			using FormClaimReports FormC=new FormClaimReports();
			FormC.ShowDialog();
			FillHistory();//To show 277s after imported.
		}

		private void toolBarButOutstanding_Click() {
			using FormCanadaOutstandingTransactions fcot=new FormCanadaOutstandingTransactions();
			fcot.ShowDialog();
		}

		private void toolBarButPayRec_Click() {
			using FormCanadaPaymentReconciliation fcpr=new FormCanadaPaymentReconciliation();
			fcpr.ShowDialog();
		}

		private void toolBarButSummaryRec_Click() {
			using FormCanadaSummaryReconciliation fcsr=new FormCanadaSummaryReconciliation();
			fcsr.ShowDialog();
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butNextUnsent_Click(object sender,EventArgs e) {
			int clinicSelectedAdjust=0;
			if(!Security.CurUser.ClinicIsRestricted && comboClinic.SelectedIndex!=0) {
				clinicSelectedAdjust=1;
			}
			int newClinicSelected=-1;
			for(int i=0;i<_listNumberOfClaims.Count;i++) {
				//Ignore currently selected clinic
				if(i==comboClinic.SelectedIndex-clinicSelectedAdjust) {
					continue;
				}
				if(i>comboClinic.SelectedIndex-clinicSelectedAdjust && _listNumberOfClaims[i]>0) {
					comboClinic.SelectedIndex=i+clinicSelectedAdjust;
					FillGrid();
					return;
				}
				if(_listNumberOfClaims[i]>0 && newClinicSelected==-1) {
					newClinicSelected=i+clinicSelectedAdjust;
				}
			}
			if(newClinicSelected>=0) {
				comboClinic.SelectedIndex=newClinicSelected;
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
				listSelectedEtransTypes.Add(_listCurEtransTypes[selectedIdx]);
			}
			if(comboHistoryType.SelectedIndices.Count==0) {//None selected.  The user can unselect each option manually.
				listSelectedEtransTypes.AddRange(_listCurEtransTypes);
			}
			tableHistory=Etranss.RefreshHistory(dateFrom,dateTo,listSelectedEtransTypes);
			//listQueue=Claims.GetQueueList();
			gridHistory.BeginUpdate();
			gridHistory.ListGridColumns.Clear();
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				GridColumn col;
				col=new GridColumn(Lan.g("TableClaimHistory","Patient Name"),120);
				gridHistory.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Carrier Name"),150);
				gridHistory.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Clearinghouse"),90);
				gridHistory.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Date"),70,HorizontalAlignment.Center);
				gridHistory.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Type"),90);
				gridHistory.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","AckCode"),90,HorizontalAlignment.Center);
				gridHistory.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Note"),90);
				gridHistory.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Office#"),90);
				gridHistory.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","User"),80);
				gridHistory.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","# Carriers"),30){ IsWidthDynamic=true };
				gridHistory.ListGridColumns.Add(col);
				gridHistory.ListGridRows.Clear();
				GridRow row;
				for(int i=0;i<tableHistory.Rows.Count;i++) {
					row=new GridRow();
					row.Cells.Add(tableHistory.Rows[i]["patName"].ToString());
					row.Cells.Add(tableHistory.Rows[i]["CarrierName"].ToString());
					row.Cells.Add(tableHistory.Rows[i]["Clearinghouse"].ToString());
					row.Cells.Add(tableHistory.Rows[i]["dateTimeTrans"].ToString());
					//((DateTime)tableHistory.Rows[i]["DateTimeTrans"]).ToShortDateString());
					//still need to trim the _CA
					row.Cells.Add(tableHistory.Rows[i]["etype"].ToString());
					row.Cells.Add(tableHistory.Rows[i]["ack"].ToString());
					row.Cells.Add(tableHistory.Rows[i]["Note"].ToString());
					row.Cells.Add(tableHistory.Rows[i]["OfficeSequenceNumber"].ToString());
					Userod user=Userods.GetUser(PIn.Long(tableHistory.Rows[i]["UserNum"].ToString()));
					row.Cells.Add(user==null ? "" : user.UserName);
					row.Cells.Add(tableHistory.Rows[i]["CarrierTransCounter"].ToString());
					gridHistory.ListGridRows.Add(row);
				}
			}
			else {
				GridColumn col;
				col=new GridColumn(Lan.g("TableClaimHistory","Patient Name"),130);
				gridHistory.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Carrier Name"),170);
				gridHistory.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Clearinghouse"),90);
				gridHistory.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Date"),70,HorizontalAlignment.Center);
				gridHistory.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Type"),100);
				gridHistory.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","AckCode"),100,HorizontalAlignment.Center);
				gridHistory.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","Note"),170);
				gridHistory.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableClaimHistory","User"),130){ IsWidthDynamic=true };
				gridHistory.ListGridColumns.Add(col);
				gridHistory.ListGridRows.Clear();
				GridRow row;
				for(int i=0;i<tableHistory.Rows.Count;i++) {
					row=new GridRow();
					row.Cells.Add(tableHistory.Rows[i]["patName"].ToString());
					row.Cells.Add(tableHistory.Rows[i]["CarrierName"].ToString());
					row.Cells.Add(tableHistory.Rows[i]["Clearinghouse"].ToString());
					row.Cells.Add(tableHistory.Rows[i]["dateTimeTrans"].ToString());
					row.Cells.Add(tableHistory.Rows[i]["etype"].ToString());
					row.Cells.Add(tableHistory.Rows[i]["ack"].ToString());
					row.Cells.Add(tableHistory.Rows[i]["Note"].ToString());
					Userod user=Userods.GetUser(PIn.Long(tableHistory.Rows[i]["UserNum"].ToString()));
					row.Cells.Add(user==null ? "" : user.UserName);
					gridHistory.ListGridRows.Add(row);
				}
			}
			gridHistory.EndUpdate();
			gridHistory.ScrollToEnd();
		}

		private void panelSplitter_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			MouseIsDownOnSplitter=true;
			SplitterOriginalY=panelSplitter.Top;
			OriginalMouseY=panelSplitter.Top+e.Y;
		}

		private void panelSplitter_MouseMove(object sender,System.Windows.Forms.MouseEventArgs e) {
			if(!MouseIsDownOnSplitter)
				return;
			int splitterNewY=SplitterOriginalY+(panelSplitter.Top+e.Y)-OriginalMouseY;
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
			MouseIsDownOnSplitter=false;
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
					ToolBarClick toolClick=PrintHistory_Click;
					this.BeginInvoke(toolClick);
					break;
				case "PrintItem":
					PrintItem_Click();
					break;
				case "OutstandingClaims":
					using(FormRpOutstandingIns formROI=new FormRpOutstandingIns()) {
						formROI.ShowDialog();
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
				EtransType etype;
				for(int i=0;i<gridHistory.SelectedIndices.Length;i++) {
					etype=(EtransType)PIn.Long(tableHistory.Rows[gridHistory.SelectedIndices[i]]["Etype"].ToString());
					if(etype!=EtransType.Claim_Ren && etype!=EtransType.ClaimSent && etype!=EtransType.ClaimPrinted){
						MsgBox.Show(this,"That type of transaction cannot be undone as a group.  Please undo one at a time.");
						return;
					}
				}
			}
			//loop through each selected item, and see if they are allowed to be "undone".
			//at this point, 
			for(int i=0;i<gridHistory.SelectedIndices.Length;i++) {
				if((EtransType)PIn.Long(tableHistory.Rows[gridHistory.SelectedIndices[i]]["Etype"].ToString())==EtransType.Claim_CA){
					//if a 
				}
				//else if(){
				
				//}
				
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Remove the selected claims from the history list, and change the claim status from 'Sent' back to 'Waiting to Send'?")){
				return;
			}
			for(int i=0;i<gridHistory.SelectedIndices.Length;i++){
				Etranss.Undo(PIn.Long(tableHistory.Rows[gridHistory.SelectedIndices[i]]["EtransNum"].ToString()));
			}
			FillGrid();
			FillHistory();
		}

		private void PrintHistory_Click() {
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd2_PrintPage,
				Lan.g(this,"Claim history list printed"),
				margins:new Margins(0,0,0,0),
				printoutOrigin:PrintoutOrigin.AtMargin
			);
		}

		private void menuItemHistoryGoToAccount_Click(object sender,EventArgs e) {
			//accessed by right clicking the history grid
			if(gridHistory.SelectedIndices.Length!=1) {
				MsgBox.Show(this,"Please select exactly one item first.");
				return;
			}
			DataRow row=tableHistory.Rows[gridHistory.GetSelectedIndex()];			
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
			Etrans et=Etranss.GetEtrans(PIn.Long(tableHistory.Rows[e.Row]["EtransNum"].ToString()));
			if(et.Etype==EtransType.StatusNotify_277) {
				using FormEtrans277Edit Form277=new FormEtrans277Edit();
				Form277.EtransCur=et;
				Form277.ShowDialog();
				Cursor=Cursors.Default;
				return;//No refresh needed because 277s are not editable, they are read only.
			}
			if(et.Etype==EtransType.ERA_835) {
				EtransL.ViewFormForEra(et,this);
			}
			else {
				using FormEtransEdit FormE=new FormEtransEdit();
				FormE.EtransCur=et;
				FormE.ShowDialog();
				if(FormE.DialogResult!=DialogResult.OK) {
					Cursor=Cursors.Default;
					return;
				}
			}
			int scroll=gridHistory.ScrollValue;
			FillHistory();
			for(int i=0;i<tableHistory.Rows.Count;i++){
				if(tableHistory.Rows[i]["EtransNum"].ToString()==et.EtransNum.ToString()){
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
			Rectangle bounds=new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!headingPrinted) {
				text=Lan.g(this,"Claim History");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				text=dateRangePicker.GetDateTimeFrom().ToShortDateString()+" "+Lan.g(this,"to")+" "+dateRangePicker.GetDateTimeTo().ToShortDateString();
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				headingPrinted=true;
				headingPrintH=yPos;
			}
			#endregion
			yPos=gridHistory.PrintPage(g,pagesPrinted,bounds,headingPrintH);
			pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
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
			Etrans etrans=Etranss.GetEtrans(PIn.Long(tableHistory.Rows[gridHistory.SelectedIndices[0]]["EtransNum"].ToString()));
			new FormCCDPrint(etrans,EtransMessageTexts.GetMessageText(etrans.EtransMessageTextNum,false),false);//Show the form and allow the user to print manually if desired.
			//MessageBox.Show(etrans.MessageText);
		}

		private void gridMain_LocationChanged(object sender, EventArgs e)
		{

		}
	}
}







