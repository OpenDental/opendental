using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.Thinfinity;
using OpenDental.UI;
using OpenDentalCloud.Core;
using OpenDentBusiness;
using OpenDentBusiness.Eclaims;

namespace OpenDental{
	///<summary></summary>
	public partial class FormClaimEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		///<summary>If this claim edit window is accessed from the batch ins window, then set this to true to hide the batch button in this window and prevent loop.</summary>
		public bool IsFromBatchWindow;
		///<summary>Call SetListClaimProcsForClaim() instead of assigning a list directly to this. Contains the claimprocs for the claim.</summary>
		private List<ClaimProc> _listClaimProcsForClaim;
		///<summary>All claimprocs for the patient. Used to calculate remaining benefits, etc.</summary>
		private List<ClaimProc> _listClaimProcs;
		/// <summary>List of all procedures for this patient.  Used to get descriptions, etc.</summary>
		private List<Procedure> _listProcedures;
		private Patient _patient;
		private Family _family;
		private List <InsPlan> _listInsPlans;
		///<summary>List of substitution links.  Lazy loaded, do not directly use this variable, use the property instead.</summary>
		private List<SubstitutionLink> _listSubstituionLinks=null;
		private DataTable _dataTablePayments;
		///<summary>When user first opens form, if the claim is S or R, and the user does not have permission, the user is informed, and this is set to true.</summary>
		private bool _isNotAuthorized;
		private List <PatPlan> _listPatPlans;
		private Claim _claim;
		private Claim _claimOld;
		private List<ClaimValCodeLog> _listClaimValCodeLogs;
		///<summary>can be null</summary>
		private ClaimCondCodeLog _claimCondCodeLog;
		private bool _hasDoubleClickWarningAlreadyBeenDisplayed=false;
		private List<InsSub> _listInsSubs;
		private bool _isCanadianClaimSending=false;
		///<summary>Can be zero. Only populated when a secondary Canadian claim is automatically sent as a result of sending primary.</summary>
		private long _canadianSecondaryClaimNum=0;
		///<summary>The Ordering provider, for medical claims.</summary>
		private long _provNumOrdering;
		///<summary>If this claim is attached to an ordering referral, then this varible will not be null.</summary>
		private Referral _referralOrdering=null;
		///<summary>When true, a supplemental payment will get automatically created on Shown().</summary>
		private bool _isForOrthoAutoPay;
		private bool _isDeleting;
		private List<ClaimForm> _listClaimForms;
		///<summary>Set true if user entered payment.</summary>
		private bool _isPaymentEntered;
		///<summary>Data necessary to load the form.</summary>
		private ClaimEdit.LoadData _loadData;
		/// <summary>Claim status based on preference and claim type. Used to fill comboClaimStatus too.</summary>
		private List<ClaimStatus> _listClaimStatuses;
		///<summary>Call GetBlueBookEstimateData() instead of using this directly. Holds all data needed to make blue book estimates.</summary>
		private BlueBookEstimateData _blueBookEstimateData;
		///<summary>Clearinghouse for current claim set in load method and then used to skip any unneccessary flag setting logic in UpdateClaim()</summary>
		private Clearinghouse _clearinghouse;

		private List<SubstitutionLink> ListSubstitutionLinks {
			get {
				if(_listSubstituionLinks==null) {
					_listSubstituionLinks=SubstitutionLinks.GetAllForPlans(_listInsPlans);
				}
				return _listSubstituionLinks;
			}
		}

		///<summary>The permissions that logs should be made under. Uses the status of the claim when entering the window.</summary>
		private Permissions PermissionClaimEdit {
			get {
				if(_claimOld==null) {
					return Permissions.ClaimEdit;
				}
				return _claimOld.ClaimStatus.In("S","R") ? Permissions.ClaimSentEdit : Permissions.ClaimEdit;
			}
		}

		protected override string GetHelpOverride(){
			if(_claim.ClaimType.ToLower()=="preauth"){
				return "FormClaimEditPreauth";
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormClaimEditCanada";
			}
			else{
				return "FormClaimEdit";
			}
		}

		///<summary>Set isForOrthoAutoPay to true to automatically show the supplemental payment window on Shown().</summary>
		public FormClaimEdit(Claim claim, Patient patient,Family family, bool isForOrthoAutoPay = false) {
			_patient=patient;
			_family=family;
			_claim=claim;
			_claimOld=claim.Copy();
			_isForOrthoAutoPay=isForOrthoAutoPay;
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			//tbPay.CellDoubleClicked += new OpenDental.ContrTable.CellEventHandler(tbPay_CellDoubleClicked);
			//tbProc.CellClicked += new OpenDental.ContrTable.CellEventHandler(tbProc_CellClicked);
			//tbPay.CellClicked += new OpenDental.ContrTable.CellEventHandler(tbPay_CellClicked);
			Lan.F(this);
			listAttachments.ContextMenu=contextMenuAttachments;
			gridProc.ContextMenu=contextAdjust;
		}

		private void FormClaimEdit_Shown(object sender,EventArgs e) {
			if(!_isForOrthoAutoPay) {
				return;
			}
			//Automatically show the supplemental payment window after automatically selecting all ortho banding codes.
			List<long> listCodeNums = ProcedureCodes.GetOrthoBandingCodeNums();
			List<Procedure> listProcedures=Procedures.GetManyProc(_listClaimProcsForClaim.Select(x => x.ProcNum).ToList(),false);
			for(int i = 0;i < _listClaimProcsForClaim.Count;i++) {
				ClaimProc claimProc = _listClaimProcsForClaim[i];
				Procedure procedure = listProcedures.Find(x => x.ProcNum==claimProc.ProcNum);
				if(procedure==null) {
					continue;
				}
				if(claimProc.Status == ClaimProcStatus.Received
					&& listCodeNums.Contains(procedure.CodeNum))
				{
					gridProc.SetSelected(i,true);
				}
			}
			MakeSuppPayment();
		}
		
		private void FormClaimEdit_Load(object sender, System.EventArgs e) {
			_loadData=ClaimEdit.GetLoadData(_patient,_family,_claim);
			textPatResp.Visible=_loadData.DoShowPatResp;
			if(IsFromBatchWindow) {
				groupFinalizePayment.Visible=false;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				labelPredeterm.Text=Lan.g(this,"Predeterm Num");
				labelPriorAuth.Visible=false;
				textPriorAuth.Visible=false;
				labelSpecialProgram.Visible=false;
				comboSpecialProgram.Visible=false;
				groupProsth.Visible=false;
				groupReferral.Visible=false;
				groupOrtho.Visible=false;
				groupAttachments.Visible=false;
				groupAttachedImages.Visible=false;
				groupAccident.Visible=false;
				labelNote.Text=Lan.g(this,"Claim Note (will only show on printed claims)");
				tabControlMain.SelectedTab=tabCanadian;
				if(_claim.DateSent.Date==MiscData.GetNowDateTime().Date) { //Reversal can only happen on the same day that the claim was originally sent.
					butReverse.Enabled=(_claim.CanadaTransRefNum!=null && _claim.CanadaTransRefNum.Trim()!="");
				}
				butSend.Enabled=(_claim.CanadaTransRefNum==null || _claim.CanadaTransRefNum.Trim()=="");
				butViewEra.Visible=false;
			}
			else {
				tabControlMain.TabPages.Remove(tabCanadian);
			}
			if(IsNew){
				//butCheckAdd.Enabled=false; //button was removed.
				groupEnterPayment.Enabled=false;
			}
			else if(_claim.ClaimStatus.In(ClaimStatus.Sent.GetDescription(true),ClaimStatus.Received.GetDescription(true))){//sent or received
				if((_claim.ClaimType=="PreAuth" && !Security.IsAuthorized(Permissions.PreAuthSentEdit,_claim.DateSent))
					|| (_claim.ClaimType!="PreAuth" && !Security.IsAuthorized(Permissions.ClaimSentEdit,_claim.DateSent))) 
				{ 
					butOK.Enabled=false;
					butDelete.Enabled=false;
					//butPrint.Enabled=false;//allowed to print, but just won't save changes.
					_isNotAuthorized=true;
					groupEnterPayment.Enabled=false;
					//gridProc.Enabled=false; //leave this enabled so users can still scroll through.
					comboClaimStatus.Enabled=false;
					//butCheckAdd.Enabled=false; //button was removed.
				}
			}
			if(!IsNew && ((_claim.ClaimType!="PreAuth" && !Security.IsAuthorized(Permissions.ClaimDelete,_claim.SecDateEntry,true)))) { 
				butDelete.Enabled=false;
			}
			if(_claim.ClaimType=="PreAuth"){
				labelPredeterm.Visible=false;
				textPredeterm.Visible=false;
				textDateService.Visible=false;
				labelDateService.Visible=false;
				label20.Visible=false;//warning when delete
				textReasonUnder.Visible=false;
				label4.Visible=false;//reason under
				butPayTotal.Visible=false;
				butPaySupp.Visible=false;
				butSplit.Visible=false;
				groupFinalizePayment.Visible=false;
				labelNote.Text=Lan.g(this,"Preauth Note (this will show on the preauth when submitted)");
				groupEnterPayment.Text=Lan.g(this,"Enter Estimate");
				this.Text=Lan.g(this,"Edit Preauthorization");
			}
			comboClaimType.Items.Add(Lan.g(this,"Primary"));
			comboClaimType.Items.Add(Lan.g(this,"Secondary"));
			comboClaimType.Items.Add(Lan.g(this,"PreAuth"));
			comboClaimType.Items.Add(Lan.g(this,"Other"));
			comboClaimType.Items.Add(Lan.g(this,"Capitation"));
			_listClaimStatuses=GetListComboClaimStatus();
			comboClaimStatus.Items.Clear();
			for(int i=0;i<_listClaimStatuses.Count;i++) {
				comboClaimStatus.Items.Add(_listClaimStatuses[i].GetDescription(false));
			}
			comboSpecialProgram.Items.Clear();
			string[] stringArrayEnumClaimSpecialPrograms=Enum.GetNames(typeof(EnumClaimSpecialProgram));
			for(int i=0;i<stringArrayEnumClaimSpecialPrograms.Length;i++) {
				comboSpecialProgram.Items.Add(stringArrayEnumClaimSpecialPrograms[i]);
			}
			string[] stringArrayEnumRelat=Enum.GetNames(typeof(Relat));
			for(int i=0;i<stringArrayEnumRelat.Length;i++){;
				comboPatRelat.Items.Add(Lan.g("enumRelat",stringArrayEnumRelat[i]));
				comboPatRelat2.Items.Add(Lan.g("enumRelat",stringArrayEnumRelat[i]));
			}
			_listClaimProcs=_loadData.ListClaimProcs;
			_listProcedures=_loadData.ListProcs;
			_listInsSubs=_loadData.ListInsSubs;
			_listInsPlans=_loadData.ListInsPlans;
			_listPatPlans=_loadData.ListPatPlans;
			_dataTablePayments=_loadData.TablePayments;
			InsPlan insPlan=InsPlans.GetPlan(_claim.PlanNum,_listInsPlans);
			//If there is no insplan then we need to close
			if(insPlan==null) {
				MsgBox.Show(Lans.g(this,"Invalid insurance plan associated with claim. Please run database maintenance method")+" "+nameof(DatabaseMaintenances.InsSubNumMismatchPlanNum)
					+" "+Lans.g(this,"found in the old tab"));
				this.DialogResult=DialogResult.Cancel;
				this.Close();
				return;
			}
			if(insPlan.PlanType=="p"){//ppo
				butPayTotal.Enabled=false;	
			}
			else if(insPlan.PlanType=="c") {//Capitation
				butPayProc.Enabled=false;//Only By Total payments should be allowed => https://www.opendental.com/manual/plancapitation.html
			}
			//this section used to be "supplemental"---------------------------------------------------------------------------------
			textRefNum.Text=_claim.RefNumString;
			string[] stringArrayEnumsPlaceOfService=Enum.GetNames(typeof(PlaceOfService));
			for(int i=0;i<stringArrayEnumsPlaceOfService.Length;i++) {
				comboPlaceService.Items.Add(Lan.g("enumPlaceOfService",stringArrayEnumsPlaceOfService[i]));
			}
			comboPlaceService.SelectedIndex=(int)_claim.PlaceService;
			string[] stringArrayEnumYN=Enum.GetNames(typeof(YN));
			for(int i=0;i<stringArrayEnumYN.Length;i++) {
				comboEmployRelated.Items.Add(Lan.g("enumYN",stringArrayEnumYN[i]));
			}
			comboEmployRelated.SelectedIndex=(int)_claim.EmployRelated;
			comboAccident.Items.Add(Lan.g(this,"No"));
			comboAccident.Items.Add(Lan.g(this,"Auto"));
			comboAccident.Items.Add(Lan.g(this,"Employment"));
			comboAccident.Items.Add(Lan.g(this,"Other"));
			switch(_claim.AccidentRelated) {
				case "":
					comboAccident.SelectedIndex=0;
					break;
				case "A":
					comboAccident.SelectedIndex=1;
					break;
				case "E":
					comboAccident.SelectedIndex=2;
					break;
				case "O":
					comboAccident.SelectedIndex=3;
					break;
			}
			//accident date is further down
			textAccidentST.Text=_claim.AccidentST;
			textRefProv.Text=Referrals.GetNameLF(_claim.ReferringProv);
			if(_claim.ReferringProv==0){
				butReferralEdit.Enabled=false;
			}
			else{
				butReferralEdit.Enabled=true;
			}
			//medical data
			_listClaimValCodeLogs=_loadData.ListClaimValCodes;
			_claimCondCodeLog=_loadData.ClaimCondCodeLogCur;
			comboClinic.SelectedClinicNum=_claim.ClinicNum;
			SetOrderingProvider(null);//Clears both the internal ordering and referral ordering providers.
			if(_claim.ProvOrderOverride!=0) {
				SetOrderingProvider(Providers.GetProv(_claim.ProvOrderOverride));
			}
			else if(_claim.OrderingReferralNum!=0) {
				Referral referral;
				Referrals.TryGetReferral(_claim.OrderingReferralNum,out referral);
				SetOrderingReferral(referral);
			}
			FillCombosProv();
			if(_claim.ProvBill==0){
				//setting combo to 0 would just show "0", and this field is required.
				comboProvBill.SetSelectedProvNum(Providers.GetFirst(true).ProvNum);
			}
			else{
				comboProvBill.SetSelectedProvNum(_claim.ProvBill);
			}
			if(_claim.ProvTreat==0){
				comboProvTreat.SetSelectedProvNum(Providers.GetFirst(true).ProvNum);
			}
			else{
				comboProvTreat.SetSelectedProvNum(_claim.ProvTreat);
			}
			if(Clinics.IsMedicalPracticeOrClinic(comboClinic.SelectedClinicNum)) {
				groupProsth.Visible=false;
				groupOrtho.Visible=false;
				labelOralImages.Visible=false;
				textAttachImages.Visible=false;
				checkAttachPerio.Visible=false;
				butAttachPerio.Visible=false;
			}
			FillForm();
			//_listClaimProcsForClaim gets filled within FillGrids() which gets called at the end of FillForm().
			//Because of that, this PlaceOfService code has to be after FillForm();
			if(IsNew) {
				//Check to see if any procedures associated to this claim have a site specified.
				//If so, link this claim's POS to the POS of the first procedure we come across.
				List<Procedure> listProceduresSites=_listProcedures.FindAll(x => _listClaimProcsForClaim.Any(y => y.ProcNum==x.ProcNum) && x.SiteNum!=0);
				//Null is an acceptable site.
				if(listProceduresSites.Count > 0) {
					Site site=Sites.GetFirstOrDefault(x => x.SiteNum==listProceduresSites[0].SiteNum);
					if(site!=null) {
						comboPlaceService.SelectedIndex=(int)site.PlaceService;
					}
				}
			}
			FillCanadian();
			if(PrefC.HasClinicsEnabled//Clinics enabled.
					&& !comboClinic.UserHasPermission())//User does not have access to clinic associated to claim.
			{
				SetFormReadOnly(this.PanelClient);
				this.Text+=" - "+Lan.g(this,"CLAIM CLINIC IS HIDDEN");
			}
			if(!PrefC.GetBool(PrefName.AllowProcAdjFromClaim)) {
				contextAdjust.MenuItems.Remove(menuItemAddAdj);
			}
			//Show the claim attachment button if the office is using ClaimConnect
			ClaimSendQueueItem[] claimSendQueueItemsArray=Claims.GetQueueList(_claim.ClaimNum,_claim.ClinicNum,0);
			if(claimSendQueueItemsArray==null || claimSendQueueItemsArray.Length==0) {
				//Similar to how we handle clicking Payment As Total button when claim no longer exists in the database.
				MsgBox.Show(this,"Claim has been deleted by another user.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			Clearinghouse clearinghouseHq=ClearinghouseL.GetClearinghouseHq(claimSendQueueItemsArray[0].ClearinghouseNum);
			_clearinghouse=Clearinghouses.OverrideFields(clearinghouseHq,_claim.ClinicNum);
			if(_clearinghouse!=null && _clearinghouse.IsAttachmentSendAllowed && _clearinghouse.CommBridge==EclaimsCommBridge.ClaimConnect) {
				//Disabling and hiding UI elements that don't make sense when using the ClaimConnect attachment service.
				tabNEA.Enabled=false;
				fillGridSentAttachments();
				tabControlAttach.SelectedTab=tabDXC;
			}
			SetBounds();
		}

		private void SetBounds(){
			Rectangle rectangleWorkingArea=System.Windows.Forms.Screen.FromControl(this).WorkingArea;
			//Window must start at 735 tall as a fail safe for compatibility with remote app.
			//We can adjust bigger if there is space.
			//But this adjustment cannot happen in the constructor because of the layout manager.
			int heightAvail=rectangleWorkingArea.Height;
			if(heightAvail>LayoutManager.Scale(871)){
				heightAvail=LayoutManager.Scale(871);
			}
			if(Height<heightAvail){
				Height=heightAvail;
			}	
			//Center the window on the parent.
			Location=new Point(rectangleWorkingArea.Left+rectangleWorkingArea.Width/2-Width/2,
				y:rectangleWorkingArea.Top+rectangleWorkingArea.Height/2-Height/2);
		}
		
		private void FormClaimEdit_Paint(object sender,PaintEventArgs e) {
			//this seems like a terrible idea
			//if(System.Windows.Forms.Screen.FromControl(this).WorkingArea.Height<Height) {
			//	Height=System.Windows.Forms.Screen.FromControl(this).WorkingArea.Height;//make this window as tall as possible.
			//}
			//SetHeight();
		}

		///<summary>Only using the Paint event handler caused the form to flicker when moving it. This method greatly reduces flickering.</summary>
		private void FormClaimEdit_LocationChanged(object sender,EventArgs e) {
			//if(System.Windows.Forms.Screen.FromControl(this).WorkingArea.Height<Height) {
			//	Height=System.Windows.Forms.Screen.FromControl(this).WorkingArea.Height;//make this window as tall as possible.
			//}
			//SetHeight();//Can't do this unless we are very certain it won't happen in constructor.
		}

		///<summary>Recursively disables all controls for the control passed in by looping through any sub controls and disabling them.</summary>
		private void SetFormReadOnly(System.Windows.Forms.Control controlsInput) {
			for(int i=0;i<controlsInput.Controls.Count;i++) {
				for(int j=0;j<controlsInput.Controls[i].Controls.Count;j++) {//Make sure all sub controls are read only.
					SetFormReadOnly(controlsInput.Controls[i].Controls[j]);
				}
				try {
					//Controls we wish to keep enabled for navigation purposes.
					if(controlsInput.Controls[i]==butCancel || controlsInput.Controls[i]==tabControlMain) {
						continue;
					}
					controlsInput.Controls[i].Enabled=false;
				}
				catch(Exception e) {//Just in case.
					e.DoNothing();
				}
			}
		}

		#region Provider Controls
		private void ComboClinic_SelectionChangeCommitted(object sender, EventArgs e){
			FillCombosProv();
		}

		private void comboClaimStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			ClaimStatus claimStatus=_listClaimStatuses.Where(x=>x.GetDescription(useShortVersionIfAvailable:true)==_claimOld.ClaimStatus).FirstOrDefault();
			if(claimStatus==ClaimStatus.HoldForInProcess && _listClaimStatuses[comboClaimStatus.SelectedIndex]!=ClaimStatus.HoldForInProcess) { //Status is I and trying to change it to something else
				MsgBox.Show("FormClaimEdit","A Claim Status of 'Hold for In Process' cannot be changed here.");
				comboClaimStatus.SelectedIndex=_listClaimStatuses.IndexOf(claimStatus);//put it back to previous selection
				return;
			}
			if(_claim.ClaimType=="PreAuth") {//we're in a PreAuth and trying to change the status from the dropdown
				//https://opendental.com/manual/preauth.html - See the "Receive a preauthorization" section.
				if(_listClaimStatuses[comboClaimStatus.SelectedIndex]==ClaimStatus.Received) {
					MsgBox.Show("FormClaimEdit","Claim Status can only be marked as 'Received' by using the 'By Procedure' button.");
					comboClaimStatus.SelectedIndex=_listClaimStatuses.IndexOf(claimStatus);//put it back to previous selection
				}
			}
		}

		private void butPickOrderProvInternal_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick = new FormProviderPick(comboProvBill.Items.GetAll<Provider>());
			formProviderPick.ProvNumSelected=_provNumOrdering;
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			SetOrderingProvider(Providers.GetProv(formProviderPick.ProvNumSelected));
		}

		private void butPickOrderProvReferral_Click(object sender,EventArgs e) {
			using FormReferralSelect formReferralSelect=new FormReferralSelect();
			formReferralSelect.IsSelectionMode=true;
			formReferralSelect.IsDoctorSelectionMode=true;
			formReferralSelect.IsShowPat=false;
			formReferralSelect.IsShowDoc=true;
			formReferralSelect.IsShowOther=false;
			formReferralSelect.ShowDialog();
			if(formReferralSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			SetOrderingReferral(formReferralSelect.ReferralSelected);
		}

		private void butNoneOrderProv_Click(object sender,EventArgs e) {
			SetOrderingProvider(null);//Clears both the internal ordering and referral ordering providers.
		}

		private void SetOrderingProvider(Provider provider) {
			if(provider==null) {
				_provNumOrdering=0;
				textOrderingProviderOverride.Text="";
			}
			else {
				_provNumOrdering=provider.ProvNum;
				textOrderingProviderOverride.Text=provider.GetFormalName()+"  NPI: "+(provider.NationalProvID.Trim()==""?"Missing":provider.NationalProvID);
			}
			_referralOrdering=null;
		}

		private void SetOrderingReferral(Referral referral) {
			_referralOrdering=referral;
			if(referral==null) {
				textOrderingProviderOverride.Text="";
			}
			else {
				textOrderingProviderOverride.Text=referral.GetNameFL()+"  NPI: "+(referral.NationalProvID.Trim()==""?"Missing":referral.NationalProvID);
			}
			_provNumOrdering=0;
		}

		private void butPickProvBill_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick = new FormProviderPick(comboProvBill.Items.GetAll<Provider>());
			formProviderPick.ProvNumSelected=comboProvBill.GetSelectedProvNum();
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProvBill.SetSelectedProvNum(formProviderPick.ProvNumSelected);
		}

		private void butPickProvTreat_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick = new FormProviderPick(comboProvTreat.Items.GetAll<Provider>());
			formProviderPick.ProvNumSelected=comboProvTreat.GetSelectedProvNum();
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProvTreat.SetSelectedProvNum(formProviderPick.ProvNumSelected);
		}

		///<summary>Fills combo provider based on which clinic is selected and attempts to preserve provider selection if any.</summary>
		private void FillCombosProv() {
			List<Provider> listProvidersForClinics=Providers.GetProvsForClinic(comboClinic.SelectedClinicNum);
			long providerNum=comboProvBill.GetSelectedProvNum();
			comboProvBill.Items.Clear();
			comboProvBill.Items.AddProvsAbbr(listProvidersForClinics);
			comboProvBill.SetSelectedProvNum(providerNum);
			providerNum=comboProvTreat.GetSelectedProvNum();
			comboProvTreat.Items.Clear();
			comboProvTreat.Items.AddProvsAbbr(listProvidersForClinics);
			comboProvTreat.SetSelectedProvNum(providerNum);
		}
		#endregion Provider Controls

		///<summary>Returns list of claim status that is filled on load based on PriClaimAllowSetToHoldUntilPriReceived pref and ClaimCur.ClaimType and status of claim.</summary>
		private List<ClaimStatus> GetListComboClaimStatus() {
			List<ClaimStatus> listClaimStatusRetVals=Enum.GetValues(typeof(ClaimStatus)).OfType<ClaimStatus>().ToList();
			if(PrefC.GetBool(PrefName.PriClaimAllowSetToHoldUntilPriReceived)
				|| _claim.ClaimType!="P"//Pref only applies to primary claims.
				|| _claim.ClaimStatus==ClaimStatus.HoldUntilPriReceived.GetDescription(true))//Status not allowed but we must maintain current selection.
			{
				return listClaimStatusRetVals;
			}
			//Preference does not allow status and claim was not set to status prior to disabling
			return listClaimStatusRetVals.FindAll(x => x!=ClaimStatus.HoldUntilPriReceived);
		}

		///<summary>Provides ClaimStatus enum from the current claim. Defaults to None (this should never happen)</summary>
		private bool TryGetClaimStatusEnumFromCurClaim(out ClaimStatus claimStatus) {
			claimStatus=ClaimStatus.Unsent;//Can set to anything initially.
			for(int i=0;i<_listClaimStatuses.Count;i++) {
				if(_claim.ClaimStatus!=_listClaimStatuses[i].GetDescription(true)) {
					continue;
				}
				claimStatus=_listClaimStatuses[i];
				return true;
			}
			return false;
		}

		///<summary></summary>
		public void FillForm(){
			if(_claim==null) {
				MsgBox.Show(this, "This Claim has been deleted by another user.");
				DialogResult=DialogResult.Cancel;
				this.Close();
				return;
			}
			if(_claim.ClaimType=="PreAuth") {
				this.Text=Lan.g(this,"Edit Preauthorization")+" - "+_patient.GetNameLF();
			}
			else {
				this.Text=Lan.g(this,"Edit Claim")+" - "+_patient.GetNameLF();
			}
			if(_claim.DateService.Year<1880) {
				textDateService.Text="";
			}
			else {
				textDateService.Text=_claim.DateService.ToShortDateString();
			}
			if(_claim.DateSent.Year<1880) {
				textDateSent.Text="";
			}
			else {
				textDateSent.Text=_claim.DateSent.ToShortDateString();
			}
			if(_claim.DateReceived.Year<1880) {
				textDateRec.Text="";
			}
			else {
				textDateRec.Text=_claim.DateReceived.ToShortDateString();
			}
			if(TryGetClaimStatusEnumFromCurClaim(out ClaimStatus claimStatus)){
				comboClaimStatus.SelectedIndex=_listClaimStatuses.IndexOf(claimStatus);
			}
			else {//This should never happen.
				comboClaimStatus.SetSelectedKey<ClaimStatus>(-1,x => _listClaimStatuses.IndexOf(claimStatus),x => Lan.g(this,"Not Set"));
			}
			switch(_claim.ClaimType){
				case "P":
					comboClaimType.SelectedIndex=0;
					break;
				case "S":
					comboClaimType.SelectedIndex=1;
					break;
				case "PreAuth":
					comboClaimType.SelectedIndex=2;
					break;
				case "Other":
					comboClaimType.SelectedIndex=3;
					break;
				case "Cap":
					comboClaimType.SelectedIndex=4;
					break;
			}
			comboMedType.Items.Clear();
			for(int i=0;i<Enum.GetNames(typeof(EnumClaimMedType)).Length;i++){
				comboMedType.Items.Add(Enum.GetNames(typeof(EnumClaimMedType))[i]);
			}
			comboMedType.SelectedIndex=(int)_claim.MedType;
			comboClaimForm.Items.Clear();
			comboClaimForm.Items.Add(Lan.g(this,"None"));
			comboClaimForm.SelectedIndex=0;
			_listClaimForms=ClaimForms.GetDeepCopy(true);
			for(int i=0;i<_listClaimForms.Count;i++){
				comboClaimForm.Items.Add(_listClaimForms[i].Description);
				if(_claim.ClaimForm==_listClaimForms[i].ClaimFormNum){
					comboClaimForm.SelectedIndex=i+1;
				}
			}
			comboClinic.SelectedClinicNum=_claim.ClinicNum;
			if(Defs.GetDefsForCategory(DefCat.ClaimCustomTracking,true).Count==0) {  // Disable Add button if all defs are hidden (nothing to add)
				butAdd.Enabled=false;
				labelHistoryNone.Visible=true;
			}
			else{
				butAdd.Enabled=true;
				labelHistoryNone.Visible=false;
			}
			textPriorAuth.Text=_claim.PriorAuthorizationNumber;
			textPredeterm.Text=_claim.PreAuthString;
			comboSpecialProgram.SelectedIndex=(int)_claim.SpecialProgramCode;
			textPlan.Text=InsPlans.GetDescript(_claim.PlanNum,_family,_listInsPlans,_claim.InsSubNum,_listInsSubs);
			comboPatRelat.SelectedIndex=(int)_claim.PatRelat;
			textPlan2.Text=InsPlans.GetDescript(_claim.PlanNum2,_family,_listInsPlans,_claim.InsSubNum2,_listInsSubs);
			comboPatRelat2.SelectedIndex=(int)_claim.PatRelat2;
			if(textPlan2.Text==""){
				comboPatRelat2.Visible=false;
				label10.Visible=false;
			}
			else{
				comboPatRelat2.Visible=true;
				label10.Visible=true;
			}
			switch(_claim.IsProsthesis){
				case "N"://no
					radioProsthN.Checked=true;
					break;
				case "I"://initial
					radioProsthI.Checked=true;
					break;
				case "R"://replacement
					radioProsthR.Checked=true;
					break;
			}
			if(_claim.PriorDate.Year < 1880){
				textPriorDate.Text="";
			}
			else{
				textPriorDate.Text=_claim.PriorDate.ToShortDateString();
			}
			textReasonUnder.Text=_claim.ReasonUnderPaid;
			textNote.Text=_claim.ClaimNote;
			checkIsOrtho.Checked=_claim.IsOrtho;
			textOrthoTotalM.Text=_claim.OrthoTotalM.ToString();
			textOrthoRemainM.Text=_claim.OrthoRemainM.ToString();
			if(_claim.OrthoDate.Year < 1860){
				textOrthoDate.Text="";
			}
			else{
				textOrthoDate.Text=_claim.OrthoDate.ToShortDateString();
				if(PrefC.GetBool(PrefName.OrthoClaimUseDatePlacement) && _claim.OrthoDate != null && _claim.OrthoDate.Year > 1880) {
					textOrthoDate.Enabled=false;
				}
			}
			if(_claim.DateResent.Year>1880) {
				textDateSent.Text=_claim.DateResent.ToShortDateString();
				labelDateSent.Text="Date Resent";
			}
			if(_claim.DateSentOrig.Year>1880) {
				textDateSentOrig.Text=_claim.DateSentOrig.ToShortDateString();
			}
			else {
				textDateSentOrig.Text="";
			}
			string[] stringArrayClaimCorrectionTypeNames=Enum.GetNames(typeof(ClaimCorrectionType));
			Array arrayClaimCorrectionTypeValues=Enum.GetValues(typeof(ClaimCorrectionType));
			comboCorrectionType.Items.Clear();
			for(int i=0;i<stringArrayClaimCorrectionTypeNames.Length;i++) {
				comboCorrectionType.Items.Add(stringArrayClaimCorrectionTypeNames[i]);
				if((ClaimCorrectionType)arrayClaimCorrectionTypeValues.GetValue(i)==_claim.CorrectionType) {
					comboCorrectionType.SelectedIndex=i;
				}
			}
			textClaimIdOriginal.Text=Claims.ConvertClaimId(_claim,_patient);
			textClaimIdentifier.Text=string.IsNullOrWhiteSpace(_claim.ClaimIdentifier) ? textClaimIdOriginal.Text : _claim.ClaimIdentifier;
			textOrigRefNum.Text=_claim.OrigRefNum;
			if(_claim.ShareOfCost > 0) {
				textShareOfCost.Text=_claim.ShareOfCost.ToString("F2");
			}
			//Canadian------------------------------------------------------------------
			//(there's also a FillCanadian section for fields that do not collide with USA fields)
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				if(_claim.AccidentDate.Year<1880) {
					textCanadianAccidentDate.Text="";
				}
				else {
					textCanadianAccidentDate.Text=_claim.AccidentDate.ToShortDateString();
				}
				checkCanadianIsOrtho.Checked=_claim.IsOrtho;
			}
			else {
				if(_claim.AccidentDate.Year<1880) {
					textAccidentDate.Text="";
				}
				else {
					textAccidentDate.Text=_claim.AccidentDate.ToShortDateString();
				}
				checkIsOrtho.Checked=_claim.IsOrtho;
			}
			textCanadaTransRefNum.Text=_claim.CanadaTransRefNum;
			groupCanadaOrthoPredeterm.Enabled=(_claim.ClaimType=="PreAuth");
			if(_claim.CanadaEstTreatStartDate.Year<1880) {
				textDateCanadaEstTreatStartDate.Text="";
			}
			else {
				textDateCanadaEstTreatStartDate.Text=_claim.CanadaEstTreatStartDate.ToShortDateString();
			}
			if(_claim.CanadaInitialPayment==0) {
				textCanadaInitialPayment.Text="";
			}
			else {
				textCanadaInitialPayment.Text=_claim.CanadaInitialPayment.ToString("F");
			}
			textCanadaExpectedPayCycle.Text=_claim.CanadaPaymentMode.ToString("#");
			textCanadaTreatDuration.Text=_claim.CanadaTreatDuration.ToString("##");
			textCanadaNumPaymentsAnticipated.Text=_claim.CanadaNumAnticipatedPayments.ToString("##");
			if(_claim.CanadaAnticipatedPayAmount==0) {
				textCanadaAnticipatedPayAmount.Text="";
			}
			else {
				textCanadaAnticipatedPayAmount.Text=_claim.CanadaAnticipatedPayAmount.ToString("F");
			}
			//attachments------------------
			textRadiographs.Text=_claim.Radiographs.ToString();
			textAttachImages.Text=_claim.AttachedImages.ToString();
			textAttachModels.Text=_claim.AttachedModels.ToString();
			checkAttachEoB.Checked=false;
			checkAttachNarrative.Checked=false;
			checkAttachPerio.Checked=false;
			checkAttachMisc.Checked=false;
			radioAttachMail.Checked=true;
			string[] stringArrayFlags=_claim.AttachedFlags.Split(',');
			for(int i=0;i<stringArrayFlags.Length;i++){
				switch(stringArrayFlags[i]){
					case "EoB":
						checkAttachEoB.Checked=true;
						break;
					case "Note":
						checkAttachNarrative.Checked=true;
						break;
					case "Perio":
						checkAttachPerio.Checked=true;
						break;
					case "Misc":
						checkAttachMisc.Checked=true;
						break;
					case "Mail":
						radioAttachMail.Checked=true;
						break;
					case "Elect":
						radioAttachElect.Checked=true;
						break;
				}
			}
			if(_claim.AttachmentID.ToLower().StartsWith("dxc")) {
				textAttachmentID.Text=_claim.AttachmentID;
			}
			else {
				textAttachID.Text=_claim.AttachmentID;
			}
			//medical/inst
			textBillType.Text=_claim.UniformBillType;
			textAdmissionType.Text=_claim.AdmissionTypeCode;
			textAdmissionSource.Text=_claim.AdmissionSourceCode;
			textPatientStatus.Text=_claim.PatientStatusCode;
			if(_claimCondCodeLog!=null && _claimCondCodeLog.ClaimNum!=0) {
				textCode0.Text=_claimCondCodeLog.Code0.ToString();
				textCode1.Text=_claimCondCodeLog.Code1.ToString();
				textCode2.Text=_claimCondCodeLog.Code2.ToString();
				textCode3.Text=_claimCondCodeLog.Code3.ToString();
				textCode4.Text=_claimCondCodeLog.Code4.ToString();
				textCode5.Text=_claimCondCodeLog.Code5.ToString();
				textCode6.Text=_claimCondCodeLog.Code6.ToString();
				textCode7.Text=_claimCondCodeLog.Code7.ToString();
				textCode8.Text=_claimCondCodeLog.Code8.ToString();
				textCode9.Text=_claimCondCodeLog.Code9.ToString();
				textCode10.Text=_claimCondCodeLog.Code10.ToString();
			}
			if(_listClaimValCodeLogs!=null) {
				FillValCode(_listClaimValCodeLogs,0,textVC39aCode,textVC39aAmt);
				FillValCode(_listClaimValCodeLogs,1,textVC40aCode,textVC40aAmt);
				FillValCode(_listClaimValCodeLogs,2,textVC41aCode,textVC41aAmt);
				FillValCode(_listClaimValCodeLogs,3,textVC39bCode,textVC39bAmt);
				FillValCode(_listClaimValCodeLogs,4,textVC40bCode,textVC40bAmt);
				FillValCode(_listClaimValCodeLogs,5,textVC41bCode,textVC41bAmt);
				FillValCode(_listClaimValCodeLogs,6,textVC39cCode,textVC39cAmt);
				FillValCode(_listClaimValCodeLogs,7,textVC40cCode,textVC40cAmt);
				FillValCode(_listClaimValCodeLogs,8,textVC41cCode,textVC41cAmt);
				FillValCode(_listClaimValCodeLogs,9,textVC39dCode,textVC39dAmt);
				FillValCode(_listClaimValCodeLogs,10,textVC40dCode,textVC40dAmt);
				FillValCode(_listClaimValCodeLogs,11,textVC41dCode,textVC41dAmt);
			}
			textDateIllness.Text=_claim.DateIllnessInjuryPreg.Year<1880?"":_claim.DateIllnessInjuryPreg.ToShortDateString();
			comboDateIllnessQualifier.Items.AddEnums<DateIllnessInjuryPregQualifier>();
			//this enum has non-standard number values, so we can't just cast to int
			comboDateIllnessQualifier.SelectedIndex=Array.IndexOf(Enum.GetValues(typeof(DateIllnessInjuryPregQualifier)),_claim.DateIllnessInjuryPregQualifier);
			if(comboDateIllnessQualifier.SelectedIndex==-1){
				comboDateIllnessQualifier.SelectedIndex=0;
			}
			textDateOther.Text=_claim.DateOther.Year<1880?"":_claim.DateOther.ToShortDateString();
			comboDateOtherQualifier.Items.AddEnums<DateOtherQualifier>();
			//this enum has non-standard number values, so we can't just cast to int
			comboDateOtherQualifier.SelectedIndex=Array.IndexOf(Enum.GetValues(typeof(DateOtherQualifier)),_claim.DateOtherQualifier);
			if(comboDateOtherQualifier.SelectedIndex==-1){
				comboDateOtherQualifier.SelectedIndex=0;
			}
			checkIsOutsideLab.Checked=_claim.IsOutsideLab;
			FillGrids(false);
			FillAttachments();
		}

		///<summary>If valCodeIdx>listClaimValCodes.Count then nothing will happen, otherwise it will fill the text boxes with appropriate values
		///from the listClaimValCodes[valCodeIdx].</summary>
		private static void FillValCode(List<ClaimValCodeLog> listClaimValCodeLogs,int valCodeIdx,TextBox textBoxVCCode,TextBox textBoxVCAmount) {
			if(listClaimValCodeLogs.Count>valCodeIdx) {
				textBoxVCCode.Text=listClaimValCodeLogs[valCodeIdx].ValCode;
				textBoxVCAmount.Text=listClaimValCodeLogs[valCodeIdx].ValAmount.ToString();
			}
		}

		private void FillCanadian() {
			comboReferralReason.Items.Clear();
			comboReferralReason.Items.Add("none");//0. -1 never used
			comboReferralReason.Items.Add("Pathological Anomalies");//1
			comboReferralReason.Items.Add("Disabled (physical or mental)");
			comboReferralReason.Items.Add("Complexity of Treatment");
			comboReferralReason.Items.Add("Seizure Disorders");
			comboReferralReason.Items.Add("Extensive Surgery");
			comboReferralReason.Items.Add("Surgical Complexity");
			if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				comboReferralReason.Items.Add("Rampant decay");
			}
			comboReferralReason.Items.Add("Medical History (to provide details upon request)");
			if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				comboReferralReason.Items.Add("Temporal Mandibular Joint Anomalies");
			}
			comboReferralReason.Items.Add("Accidental Injury");
			comboReferralReason.Items.Add("Anaesthesia complications (local or general)");
			comboReferralReason.Items.Add("Developmental Anomalies");
			comboReferralReason.Items.Add("Behavioral Management");//13
			//max
			comboMaxProsth.Items.Clear();
			comboMaxProsth.Items.Add("Please Choose");
			comboMaxProsth.Items.Add("Yes");
			comboMaxProsth.Items.Add("No");
			comboMaxProsth.Items.Add("Not an upper denture, crown, or bridge");
			comboMaxProsthMaterial.Items.Clear();
			comboMaxProsthMaterial.Items.Add("not applicable");//this always starts out selected. -1 never used.
			comboMaxProsthMaterial.Items.Add("Fixed bridge");
			comboMaxProsthMaterial.Items.Add("Maryland bridge");
			comboMaxProsthMaterial.Items.Add("Denture (Acrylic)");
			comboMaxProsthMaterial.Items.Add("Denture (Chrome Cobalt)");
			comboMaxProsthMaterial.Items.Add("Implant (Fixed)");
			comboMaxProsthMaterial.Items.Add("Implant (Removable)");
			comboMaxProsthMaterial.Items.Add("Crown");//7.  not an official type
			//mand
			comboMandProsth.Items.Clear();
			comboMandProsth.Items.Add("Please Choose");
			comboMandProsth.Items.Add("Yes");
			comboMandProsth.Items.Add("No");
			comboMandProsth.Items.Add("Not a lower denture, crown, or bridge");
			comboMandProsthMaterial.Items.Clear();
			comboMandProsthMaterial.Items.Add("not applicable");//this always starts out selected. -1 never used.
			comboMandProsthMaterial.Items.Add("Fixed bridge");
			comboMandProsthMaterial.Items.Add("Maryland bridge");
			comboMandProsthMaterial.Items.Add("Denture (Acrylic)");
			comboMandProsthMaterial.Items.Add("Denture (Chrome Cobalt)");
			comboMandProsthMaterial.Items.Add("Implant (Fixed)");
			comboMandProsthMaterial.Items.Add("Implant (Removable)");
			comboMandProsthMaterial.Items.Add("Crown");
			//Load data for this claim---------------------------------------------------------------------------------------------
			if(_claim.CanadianMaterialsForwarded.Contains("E")) {
				checkEmail.Checked=true;
			}
			if(_claim.CanadianMaterialsForwarded.Contains("C")) {
				checkCorrespondence.Checked=true;
			}
			if(_claim.CanadianMaterialsForwarded.Contains("M")) {
				checkModels.Checked=true;
			}
			if(_claim.CanadianMaterialsForwarded.Contains("X")) {
				checkXrays.Checked=true;
			}
			if(_claim.CanadianMaterialsForwarded.Contains("I")) {
				checkImages.Checked=true;
			}
			textReferralProvider.Text=_claim.CanadianReferralProviderNum;
			comboReferralReason.SelectedIndex=_claim.CanadianReferralReason;
			//max prosth-----------------------------------------------------------------------------------------------------
			switch(_claim.CanadianIsInitialUpper){
				case "Y":
					comboMaxProsth.SelectedIndex=1;
					break;
				case "N":
					comboMaxProsth.SelectedIndex=2;
					break;
				default: //"X"
					comboMaxProsth.SelectedIndex=3;
					break;
			}
			if(_claim.CanadianDateInitialUpper.Year<1880) {
				textDateInitialUpper.Text="";
			}
			else {
				textDateInitialUpper.Text=_claim.CanadianDateInitialUpper.ToShortDateString();
			}
			comboMaxProsthMaterial.SelectedIndex=_claim.CanadianMaxProsthMaterial;
			//mand prosth-----------------------------------------------------------------------------------------------------
			switch(_claim.CanadianIsInitialLower) {
				case "Y":
					comboMandProsth.SelectedIndex=1;
					break;
				case "N":
					comboMandProsth.SelectedIndex=2;
					break;
				default: //"X"
					comboMandProsth.SelectedIndex=3;
					break;
			}
			if(_claim.CanadianDateInitialLower.Year<1880) {
				textDateInitialLower.Text="";
			}
			else {
				textDateInitialLower.Text=_claim.CanadianDateInitialLower.ToShortDateString();
			}
			comboMandProsthMaterial.SelectedIndex=_claim.CanadianMandProsthMaterial;
			//Missing and Extracted Teeth--------------------------------------------------------------------------------------
			SetCanadianExtractedTeeth();
			List<string> listStringMissingTeeth=ToothInitials.GetMissingOrHiddenTeeth(_loadData.ListToothInitials);
			string stringMissingTeeth="";
			for(int i=0;i<listStringMissingTeeth.Count;i++){
				if(i>0){
					stringMissingTeeth+=", ";
				}
				stringMissingTeeth+=Tooth.Display(listStringMissingTeeth[i]);
			}
			textMissingTeeth.Text=stringMissingTeeth;
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				checkCanadianIsOrtho.Visible=false;
				groupCanadaOrthoPredeterm.Visible=false;
				groupMaxPros.Visible=false;
				groupMandPros.Visible=false;
				labelExtractedTeeth.Visible=false;
				butMissingTeethHelp.Visible=false;
				listExtractedTeeth.Visible=false;
				labelCanadaMissingTeeth.Visible=false;
				textMissingTeeth.Visible=false;
			}
		}

		///<summary>Returns true if the claim's carrier is set to block users from entering payments on claims made by the Auto Ortho Tool. Otherwise, returns false.</summary>
		private bool DoConsolidateOrthoPayments() {
			return Carriers.DoConsolidateOrthoPayments(InsPlans.GetPlan(_claim.PlanNum,_listInsPlans));
		}

		private void SetCanadianExtractedTeeth() {
			listExtractedTeeth.Items.Clear();
			if(comboMaxProsth.SelectedIndex==1 || comboMandProsth.SelectedIndex==1){
				List<Procedure> listProcedureExtracted=Procedures.GetCanadianExtractedTeeth(_listProcedures);				
				for(int i=0;i<listProcedureExtracted.Count;i++) {
					listExtractedTeeth.Items.Add(Tooth.Display(listProcedureExtracted[i].ToothNum)+"\t"+listProcedureExtracted[i].ProcDate.ToShortDateString());
				}
			}
		}

		private void comboMaxProsth_SelectionChangeCommitted(object sender,EventArgs e) {
			SetCanadianExtractedTeeth();
		}

		private void comboMandProsth_SelectionChangeCommitted(object sender,EventArgs e) {
			SetCanadianExtractedTeeth();
		}

		private void butRecalc_Click(object sender, System.EventArgs e) {
			if(PatPlans.GetCountForPatAndInsSub(_claim.InsSubNum,_claim.PatNum)==0) {//If the plan has been dropped
				//Don't let the user recalculate estimates.  Our estimate calculation code would zero all claimproc insurance estimate amounts.
				MsgBox.Show(this,"You cannot recalculate estimates for an insurance plan that has been dropped.");
				return;
			}
			if(!ClaimIsValid()){
				return;
			}
			List <Benefit> listBenefits=Benefits.Refresh(_listPatPlans,_listInsSubs);
			Claims.CalculateAndUpdate(_listProcedures,_listInsPlans,_claim,_listPatPlans,listBenefits,_patient,_listInsSubs);
			_listClaimProcs=ClaimProcs.Refresh(_patient.PatNum);
			FillGrids();
			if(!_recalcErrorProvider.GetError(this.butRecalc).Equals(String.Empty)) {//ClaimProcedures values have been modified.
				_recalcErrorProvider.SetError(this.butRecalc,String.Empty);//Above logic results in no longer have missmatching estimates and totals.
			}
		}

		private void FillGrids(bool doRefreshData=true){
			//must run claimprocs.refresh separately beforehand
			//also recalculates totals because user might have changed certain items.
			_claim.ClaimFee=0;
			_claim.DedApplied=0;
			_claim.InsPayEst=0;
			_claim.InsPayAmt=0;
			_claim.WriteOff=0;
			gridProc.BeginUpdate();
			gridProc.Columns.Clear();
			gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","#"),25));
			gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Date"),66));
			gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Prov"),62));
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Code"),75));
			}
			else {
				gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Code"),50));
				gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Tth"),25));
			}
			gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Description"),130));
			gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Fee"),62,HorizontalAlignment.Right));
			gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Billed to Ins"),75,HorizontalAlignment.Right));
			gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Deduct"),62,HorizontalAlignment.Right));
			gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Ins Est"),62,HorizontalAlignment.Right));
			gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Ins Pay"),62,HorizontalAlignment.Right));
			gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Write-off"),62,HorizontalAlignment.Right));
			if(_loadData.DoShowPatResp) {
				gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Pat Resp"),62,HorizontalAlignment.Right));
			}
			gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Status"),50,HorizontalAlignment.Center));
			gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Pmt"),30,HorizontalAlignment.Center));
			if(PrefC.GetBool(PrefName.ClaimEditShowPayTracking)) {
				gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Pay Tracking"),90,HorizontalAlignment.Center));
			}
			gridProc.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Remarks"),60){ IsWidthDynamic=true });
			gridProc.ListGridRows.Clear();
			GridRow row;
			SetListClaimProcsForClaim(ClaimProcs.RefreshForClaim(_claim.ClaimNum,_listProcedures,_listClaimProcs));//Excludes labs.
			//ClaimProcs should always be ordered by their LineNumber (most important).
			//However, there can be supplemental transfers mixed into the list of ClaimProcs for this claim which need to show up next to what they offset.
			List<ClaimProc> listClaimProcsTxfr=_listClaimProcsForClaim.FindAll(x => x.IsTransfer);
			if(listClaimProcsTxfr.Count > 0) {
				_listClaimProcsForClaim.RemoveAll(x => x.IsTransfer);
				for(int i=0;i<listClaimProcsTxfr.Count;i++) {
					//If this is a supplemental transfer that is not associated to a procedure, just throw it at the top of the list (no ItemOrder).
					if(listClaimProcsTxfr[i].ProcNum==0) {
						_listClaimProcsForClaim.Insert(0,listClaimProcsTxfr[i]);
						continue;
					}
					//Try and find a corresponding ClaimProc with the same ProcNum.
					int index=_listClaimProcsForClaim.FindIndex(x => x.ProcNum==listClaimProcsTxfr[i].ProcNum);
					//Default to inserting into the beginning of the list if a match was not found.
					_listClaimProcsForClaim.Insert((index==-1 ? 0 : index+1),listClaimProcsTxfr[i]);
				}
			}
			List<ClaimProc> listClaimProcsIntermingled=new List<ClaimProc>();//This list will contain procs and their labs, such that attached labs follow their parent proc.
			for(int i=0;i<_listClaimProcsForClaim.Count;i++){
				row=new GridRow();
				listClaimProcsIntermingled.Add(_listClaimProcsForClaim[i]);
				ClaimProcRowHelper(row,_listClaimProcsForClaim[i]);
				gridProc.ListGridRows.Add(row);
				Procedure procedure=Procedures.GetProcFromList(_listProcedures,_listClaimProcsForClaim[i].ProcNum);
				List<ClaimProc> listClaimProcsForLab=GetListClaimProcsForProcNumLabsParent(procedure.ProcNum);
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && listClaimProcsForLab.Count!=0) {//This proc has one or more labs attached.
					for(int j=0;j<listClaimProcsForLab.Count;j++) {
						//The following 'continue' logic does not skip adding the parent lab rows to gridProc.
						//Instead it ensures that received or unreceived parent claimProcs show above their associated received or unreceived lab claimProcs.
						if(_listClaimProcsForClaim[i].Status != listClaimProcsForLab[j].Status) {
							continue;
						}	
						//Needed for supplemental payments. When there are supplemental payments we want the associated supplemental lab payment to show under the supplemental parent claimProc, not received.
						//Original estimates will show together and all supplementals will show together.
						if(_listClaimProcsForClaim[i].Status==ClaimProcStatus.Supplemental && _listClaimProcsForClaim[i].DateCP!=listClaimProcsForLab[j].DateCP){//Needed when there are multiple supplemental payments.
							//Edge Case: If DateEntry is the same for multiple supplemental payments then the rows will duplicate under each parent supplemental row.
							//This will cause issues with this logic but is an edge case and we will handle it if it ever happens to a customer.
							continue;
						}
						GridRow labRow=new GridRow();
						listClaimProcsIntermingled.Add(listClaimProcsForLab[j]);
						ClaimProcRowHelper(labRow,listClaimProcsForLab[j]);
						labRow.Tag=listClaimProcsForLab[j];
						gridProc.ListGridRows.Add(labRow);
					}
				}
				row.Tag=_listClaimProcsForClaim[i];
			}
			gridProc.EndUpdate();
			if(_listClaimProcsForClaim.Any(x => x.Status.In(ClaimProcStatus.Estimate
				,ClaimProcStatus.CapComplete,ClaimProcStatus.CapEstimate,ClaimProcStatus.Adjustment,ClaimProcStatus.InsHist)))
			{
				MsgBox.Show(this,"One or more claim procedures have an invalid status.");
			}
			SetListClaimProcsForClaim(listClaimProcsIntermingled);//Now _listClaimProcsForClaim matches the grid order.
			if(_claim.ClaimType=="Cap"){
				//zero out ins info if Cap.  This keeps it from affecting the balance.  It could be slightly improved later if there is enough demand to show the inspayamt in the Account module.
				_claim.InsPayEst=0;
				_claim.InsPayAmt=0;
			}
			textClaimFee.Text=_claim.ClaimFee.ToString("F");
			textDedApplied.Text=_claim.DedApplied.ToString("F");
			textInsPayEst.Text=_claim.InsPayEst.ToString("F");
			textInsPayAmt.Text=_claim.InsPayAmt.ToString("F");
			textWriteOff.Text=_claim.WriteOff.ToString("F");
			if(_loadData.DoShowPatResp) {
				double procFeeTotal=gridProc.GetTags<ClaimProc>()
					.Where(x => x.Status!=ClaimProcStatus.Supplemental)
					.Select(x => Procedures.GetProcFromList(_listProcedures,x.ProcNum))
					.Sum(x => x.ProcFeeTotal);
				textPatResp.Text=(procFeeTotal-_claim.WriteOff-_claim.InsPayAmt).ToString("F");
			}
			//payments
			gridPay.BeginUpdate();
			gridPay.Columns.Clear();
			gridPay.Columns.Add(new GridColumn(Lan.g("TableClaimPay","Date"),70));
			gridPay.Columns.Add(new GridColumn(Lan.g("TableClaimPay","Type"),60));
			gridPay.Columns.Add(new GridColumn(Lan.g("TableClaimPay","Amount"),80,HorizontalAlignment.Right));
			gridPay.Columns.Add(new GridColumn(Lan.g("TableClaimPay","Check Num"),90));
			gridPay.Columns.Add(new GridColumn(Lan.g("TableClaimPay","Bank/Branch"),100));
			gridPay.Columns.Add(new GridColumn(Lan.g("TableClaimPay","Note"),180));
			gridPay.ListGridRows.Clear();
			if(doRefreshData) {
				_dataTablePayments=ClaimPayments.GetForClaim(_claim.ClaimNum);
			}
			for(int i=0;i<_dataTablePayments.Rows.Count;i++){
				row=new GridRow();
				row.Cells.Add(_dataTablePayments.Rows[i]["checkDate"].ToString());
				row.Cells.Add(_dataTablePayments.Rows[i]["payType"].ToString());
				row.Cells.Add(_dataTablePayments.Rows[i]["amount"].ToString());
				row.Cells.Add(_dataTablePayments.Rows[i]["CheckNum"].ToString());
				row.Cells.Add(_dataTablePayments.Rows[i]["BankBranch"].ToString());
				row.Cells.Add(_dataTablePayments.Rows[i]["Note"].ToString());
				gridPay.ListGridRows.Add(row);
			}
			gridPay.EndUpdate();
			FillStatusHistory(doRefreshData);
		}

		private void fillGridSentAttachments() {
			gridSent.BeginUpdate();
			gridSent.Columns.Clear();
			gridSent.ListGridRows.Clear();
			GridColumn col;
			col=new GridColumn("File",200);
			gridSent.Columns.Add(col);
			GridRow row;
			for(int i=0;i<_claim.Attachments.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_claim.Attachments[i].DisplayedFileName);
				row.Tag=_claim.Attachments[i];
				gridSent.ListGridRows.Add(row);
			}
			gridSent.EndUpdate();
		}

		private void gridSent_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			ClaimAttach claimAttach=(ClaimAttach)gridSent.ListGridRows[e.Row].Tag;
			string patFolder=ImageStore.GetPatientFolder(_patient,ImageStore.GetPreferredAtoZpath());
			if(CloudStorage.IsCloudStorage) {
				string pathAndFileName=ODFileUtils.CombinePaths(patFolder,claimAttach.ActualFileName,'/');
				if(!CloudStorage.FileExists(pathAndFileName)) {
					//Couldn't find file, display message and return
					MsgBox.Show(this,"File no longer exists.");
					return;
				}
				//found it, download and display
				//This chunk of code was pulled from FormFilePicker.cs
				using FormProgress formProgress=new FormProgress();
				formProgress.DisplayText="Downloading...";
				formProgress.NumberFormat="F";
				formProgress.NumberMultiplication=1;
				formProgress.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
				formProgress.TickMS=1000;
				TaskStateDownload taskStateDownload=CloudStorage.DownloadAsync(patFolder,claimAttach.ActualFileName,
					new OpenDentalCloud.ProgressHandler(formProgress.UpdateProgress));
				if(formProgress.ShowDialog()==DialogResult.Cancel) {
					taskStateDownload.DoCancel=true;
					return;
				}
				string tempFile=PrefC.GetRandomTempFile(Path.GetExtension(pathAndFileName));
				File.WriteAllBytes(tempFile,taskStateDownload.FileContent);
				if(ODBuild.IsWeb()) {
					ThinfinityUtils.HandleFile(tempFile);
				}
				else {
					Process.Start(tempFile);
				}
			}
			else {//Local storage
				string pathAndFileName=ODFileUtils.CombinePaths(patFolder,claimAttach.ActualFileName);
				if(ODBuild.IsWeb()) {
					ThinfinityUtils.HandleFile(pathAndFileName);
				}
				else {
					try {
						Process.Start(pathAndFileName);
					}
					catch(Exception ex) {
						ex.DoNothing();
						MsgBox.Show(this,"Could not open the attachment.");
					}
				}
			}
		}

		private void ClaimProcRowHelper(GridRow row,ClaimProc claimProc) {
			if(claimProc.LineNumber==0){
				row.Cells.Add("");
			}
			else{
				row.Cells.Add(claimProc.LineNumber.ToString());
			}
			string date=claimProc.ProcDate.ToShortDateString();
			if(claimProc.ProcNum==0) {//Total payment
				//We want to always show the "Payment Date" instead of the procedure date for total payments because they are not associated to procedures.
				date=claimProc.DateCP.ToShortDateString();
			}
			//Check if there was insurance payment amount entered but no claim payment (insurance check / not finalized). 
			if(claimProc.InsPayAmt>0 && claimProc.ClaimPaymentNum==0) {
				//Many users will just enter payment via clicking 'By Procedure' or 'Total' which does not "finalize" claim payments.
				//Instead of showing a "payment date" for insurance payments, we are going to make it visually obvious that the payments have not been finalized yet.
				date=Lan.g(this,"Not Final");//"Not Finalized" is too long.
			}
			row.Cells.Add(date);
			row.Cells.Add(Providers.GetAbbr(claimProc.ProvNum));
			double procedureFeeTotal=0;
			if(claimProc.ProcNum==0) {
				row.Cells.Add("");//code
				if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
					row.Cells.Add("");//tooth
				}
				if(claimProc.Status==ClaimProcStatus.NotReceived)
					row.Cells.Add(Lan.g(this,"Estimate"));
				else
					row.Cells.Add(Lan.g(this,"Total Payment"));
			}
			else {
				//claimProcsForClaim list already handles ProcNum=0 above
				Procedure procedure=Procedures.GetProcFromList(_listProcedures,claimProc.ProcNum);
				procedureFeeTotal=procedure.ProcFeeTotal;
				row.Cells.Add(claimProc.CodeSent);
				if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
					row.Cells.Add(Tooth.Display(procedure.ToothNum));
				}
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(procedure.CodeNum);
				ProcedureCode procedureCodeSent=ProcedureCodes.GetProcCode(claimProc.CodeSent);
				InsPlan insPlan=_listInsPlans.Where(x => x.PlanNum == claimProc.PlanNum).FirstOrDefault();
				string descript=Procedures.GetClaimDescript(claimProc,procedureCodeSent,procedure,procedureCode,insPlan);
				if(procedureCodeSent.IsCanadianLab) {
					descript="^ ^ "+descript;
				}
				row.Cells.Add(descript);
			}
			row.Cells.Add(procedureFeeTotal.ToString("F"));
			row.Cells.Add(claimProc.FeeBilled.ToString("F"));
			row.Cells.Add(claimProc.DedApplied.ToString("F"));
			row.Cells.Add(claimProc.InsPayEst.ToString("F"));
			row.Cells.Add(claimProc.InsPayAmt.ToString("F"));
			row.Cells.Add(claimProc.WriteOff.ToString("F"));
			if(_loadData.DoShowPatResp) {
				if(claimProc.Status==ClaimProcStatus.Supplemental) {
					procedureFeeTotal=0;
				}
				row.Cells.Add((procedureFeeTotal-claimProc.WriteOff-claimProc.InsPayAmt).ToString("F"));
			}
			switch(claimProc.Status){
				case ClaimProcStatus.Received:
					row.Cells.Add(Lan.g("TableClaimProc","Recd"));
					break;
				case ClaimProcStatus.NotReceived:
					row.Cells.Add("");
					break;
				case ClaimProcStatus.Preauth:
					row.Cells.Add(Lan.g("TableClaimProc","PreA"));
					break;
				case ClaimProcStatus.Supplemental:
					//The income transfer system will create supplemental ClaimProcs that are designed to offset other ClaimProcs.
					//These entries need to stand out to the user and the easiest way to do so is to not display them as supplemental payments (Supp).
					if(claimProc.IsTransfer) {
						row.Cells.Add(Lan.g("TableClaimProc","Txfr"));
					}
					else {
						row.Cells.Add(Lan.g("TableClaimProc","Supp"));
					}
					break;
				case ClaimProcStatus.CapClaim:
					row.Cells.Add(Lan.g("TableClaimProc","Cap"));
					break;
				case ClaimProcStatus.Adjustment:
				case ClaimProcStatus.InsHist:
				case ClaimProcStatus.Estimate:
				case ClaimProcStatus.CapEstimate:
				case ClaimProcStatus.CapComplete:
					row.ColorBackG=Color.Salmon;
					row.Cells.Add("");
					break;
				default:
					row.Cells.Add("");
					break;
			}
			if(claimProc.ClaimPaymentNum>0){
				row.Cells.Add("X");
			}
			else{
				row.Cells.Add("");
			}
			if(PrefC.GetBool(PrefName.ClaimEditShowPayTracking)) {
				if(claimProc.ClaimPaymentTracking!=0) {
					row.Cells.Add(Defs.GetDef(DefCat.ClaimPaymentTracking,claimProc.ClaimPaymentTracking).ItemName);//EOB Code
				}
				else {
					row.Cells.Add("");
				}
			}
			row.Cells.Add(claimProc.Remarks);
			_claim.ClaimFee+=claimProc.FeeBilled;
			_claim.DedApplied+=claimProc.DedApplied;
			_claim.InsPayEst+=claimProc.InsPayEst;
			_claim.InsPayAmt+=claimProc.InsPayAmt;
			_claim.WriteOff+=claimProc.WriteOff;
		}

		private void FillStatusHistory(bool doRefreshStatusEntries=true) {
			////status history
			gridStatusHistory.BeginUpdate();
			gridStatusHistory.Columns.Clear();
			gridStatusHistory.Columns.Add(new GridColumn(Lan.g("TableStatusHistory","Date"),120));
			gridStatusHistory.Columns.Add(new GridColumn(Lan.g("TableStatusHistory","Description"),270));
			gridStatusHistory.Columns.Add(new GridColumn(Lan.g("TableStatusHistory","Log Note"),320));
			gridStatusHistory.Columns.Add(new GridColumn(Lan.g("TableStatusHistory","ErrorCode"),90));
			gridStatusHistory.Columns.Add(new GridColumn(Lan.g("TableStatusHistory","User"),90));
			gridStatusHistory.ListGridRows.Clear();
			List<ClaimTracking> listClaimTrackingsCustomStatusEntries;
			if(doRefreshStatusEntries) {
				listClaimTrackingsCustomStatusEntries=ClaimTrackings.RefreshForClaim(ClaimTrackingType.StatusHistory,_claim.ClaimNum);
			}
			else {
				listClaimTrackingsCustomStatusEntries=_loadData.ListCustomStatusEntries;
			}
			GridRow row;
			listClaimTrackingsCustomStatusEntries=listClaimTrackingsCustomStatusEntries.OrderByDescending(x => x.DateTimeEntry).ToList();
			for(int i=0;i<listClaimTrackingsCustomStatusEntries.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listClaimTrackingsCustomStatusEntries[i].DateTimeEntry.ToShortDateString()+" "+listClaimTrackingsCustomStatusEntries[i].DateTimeEntry.ToShortTimeString());
				String defValue=Defs.GetName(DefCat.ClaimCustomTracking,listClaimTrackingsCustomStatusEntries[i].TrackingDefNum);//get definition Name
				row.Cells.Add(defValue);
				row.Cells.Add(listClaimTrackingsCustomStatusEntries[i].Note);
				row.Cells.Add(Defs.GetName(DefCat.ClaimErrorCode,listClaimTrackingsCustomStatusEntries[i].TrackingErrorDefNum));
				row.Cells.Add(Userods.GetName(listClaimTrackingsCustomStatusEntries[i].UserNum));
				gridStatusHistory.ListGridRows.Add(row);
				row.Tag=listClaimTrackingsCustomStatusEntries[i];
			}
			gridStatusHistory.EndUpdate();
		}

		private void gridProc_CellClick(object sender,ODGridClickEventArgs e) {
			if(_claim.ClaimStatus.In(ClaimStatus.Sent.GetDescription(true),ClaimStatus.Received.GetDescription(true))) {//sent or received
				if((_claim.ClaimType!="PreAuth" && !Security.IsAuthorized(Permissions.ClaimSentEdit,_claim.DateSent,true))
					|| (_claim.ClaimType=="PreAuth" && !Security.IsAuthorized(Permissions.PreAuthSentEdit,_claim.DateSent,true))) 
				{
					return;
				}
			}
			if(gridPay.GetSelectedIndex()!=-1) {
				gridPay.SetAll(false);
			}
			SelectLabProcs();
		}

		///<summary>Selects any lab procedures that go along with the currently selected procedures.</summary>
		private void SelectLabProcs() {
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				return;
			}
			for(int i=0;i<gridProc.SelectedIndices.Count();i++) {
				int index=gridProc.SelectedIndices[i];
				if(_listClaimProcsForClaim[index].ProcNum==0) {
					continue;//current index is associated to a 'Total Payment' row.
				}
				Procedure procedure=_listProcedures.FirstOrDefault(x => x.ProcNum==_listClaimProcsForClaim[index].ProcNum);
				List<ClaimProc> listClaimProcsForLab=GetListClaimProcsForProcNumLabsParent(procedure.ProcNum);//This Claimproc's Procedure is a Lab
				//The procedure is a parent procedure (ProcNumLab==0) with one or more child labs who have ClaimProcs on this Claim (listClaimProcsForLab did not return 0)
				if(procedure.ProcNumLab==0 && listClaimProcsForLab.Count!=0) {
					for(int j=1;j<=GetListLabsForProc(procedure.ProcNum,_listClaimProcsForClaim[index]).Count;j++) {
						gridProc.SetSelected(index+j,true);//Labs are grouped with their parent procedure. See FillGrids().
					}
					continue;
				}
				else if(procedure.ProcNumLab!=0) {//Otherwise, if it has a ProcNumLab set, this is a lab procedure.
					gridProc.SetSelected(index-1,true);//Guaranteed the row above is either a parent or another lab.  See FillGrids().
					if(GetListLabsForProc(procedure.ProcNum,_listClaimProcsForClaim[index]).Count>1) {//User already clicked the 1 lab and it is already selected when 1.
						if(_listClaimProcsForClaim[index-1].ProcNum==procedure.ProcNumLab) {//The row above the currently clicked row is the parent proc.
							gridProc.SetSelected(index+1,true);//The second lab is the next row, since the previous row is the parent and the current row is the first lab.
						}
						else {//The above row is the first lab and the current row is the second lab.
							gridProc.SetSelected(index-2,true);//Select the parent proc, since we already selected the first lab proc above.
						}
					}
				}
			}
			return;
		}

		///<summary>Takes in the appropriate ProcNum and current claimproc to get the number of claimprocs associated with that procedure</summary>
		private List<ClaimProc> GetListLabsForProc(long procNumParent,ClaimProc claimProc) {
			List<ClaimProc> listClaimProcsForLab=GetListClaimProcsForProcNumLabsParent(procNumParent);
			if(claimProc.Status==ClaimProcStatus.Supplemental) { 
				return listClaimProcsForLab.Where(x=>x.Status==ClaimProcStatus.Supplemental && x.DateCP==claimProc.DateCP).ToList();
			}
			return listClaimProcsForLab.Where(x=>x.Status==claimProc.Status).ToList();
		}

		///<summary>For a given procNumParent, return a list of ClaimProcs for that lab(s) that is on the current _claim.</summary>
		private List<ClaimProc> GetListClaimProcsForProcNumLabsParent(long procNumParent) {
			List<ClaimProc> listClaimProcsForLabOnClaim=new List<ClaimProc>();
			if(procNumParent==0) {
				return listClaimProcsForLabOnClaim;
			}
			//List of child procedures associated to the passed in procNumParent belonging to this claim.
			List<Procedure> listProceduresLab=_listProcedures.FindAll(x => x.ProcNumLab==procNumParent);
			for(int i=0;i<listProceduresLab.Count;i++){//For each lab proc associated to the passed in procNumParent, add it to the list
				listClaimProcsForLabOnClaim.AddRange(_listClaimProcs.FindAll(x => x.ProcNum==listProceduresLab[i].ProcNum && x.ClaimNum==_claim.ClaimNum));
			}
			return listClaimProcsForLabOnClaim;
		}
		
		private void gridProc_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_claim.ClaimStatus.In(ClaimStatus.Sent.GetDescription(true),ClaimStatus.Received.GetDescription(true))) {//sent or received
				if((_claim.ClaimType!="PreAuth" && !Security.IsAuthorized(Permissions.ClaimSentEdit,_claim.DateSent,true))
					|| (_claim.ClaimType=="PreAuth" && !Security.IsAuthorized(Permissions.PreAuthSentEdit,_claim.DateSent,true)))
				{
					return;
				}
			}
			if(!_hasDoubleClickWarningAlreadyBeenDisplayed){
				_hasDoubleClickWarningAlreadyBeenDisplayed=true;
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"If you are trying to enter payment information, please use the payments buttons at the upper right.\r\nThen, don't forget to finish by creating the check using the button below this section.\r\nYou should probably click cancel unless you are just editing estimates.\r\nContinue anyway?")){
					return;
				}
			}
			ClaimProc claimProc=_listClaimProcsForClaim[e.Row];
			if(!CheckRecalcEstimates(claimProc)) {
				return;
			}
			_listClaimProcs=ClaimProcs.Refresh(_patient.PatNum);
			FillGrids();
		}
		
		private void tabControlMain_SelectedIndexChanged(object sender,EventArgs e) {
			if(tabControlMain.SelectedTab==tabHistory){
				LayoutManager.Move(gridStatusHistory,new Rectangle(
					x:LayoutManager.Scale(6), y:LayoutManager.Scale(33),
					width:tabControlMain.ClientSize.Width-LayoutManager.Scale(12),
					height:tabControlMain.ClientSize.Height-LayoutManager.Scale(33)-2));
			}
		}

		private void contextAdjust_Popup(object sender,EventArgs e) {
			if(gridProc.SelectedGridRows.Count>0) {
				menuItemAddAdj.Enabled=true;
			}
			if(gridProc.SelectedGridRows.Count<=0) {
				menuItemAddAdj.Enabled=false;
			}
		}
		
		private bool CheckRecalcEstimates(ClaimProc claimProc) {
			List<ClaimProcHist> listClaimProcHists=null;
			List<ClaimProcHist> listClaimProcHistsLoop=null;
			using FormClaimProc formClaimProc=new FormClaimProc(claimProc,null,_family,_patient,_listInsPlans,listClaimProcHists,ref listClaimProcHistsLoop,_listPatPlans,true,_listInsSubs);
			formClaimProc.IsInClaim=true;
			formClaimProc.ShowDialog();
			if(formClaimProc.DialogResult==DialogResult.Abort) {//Claim was deleted. Close claim edit window.
				DialogResult=DialogResult.Abort;
				return false;
			}
			else if(formClaimProc.DialogResult!=DialogResult.OK) {
				return false;
			}
			if(claimProc.DoDelete) {
			//If we delete a Canadian claimproc that had a lab procedure, 
			//then _listClaimProcsForLabProcs will be rebuilt from ClaimProcList in FillGrids().
			//There is no need to worry about removing those claimprocs in memory.
				_listClaimProcsForClaim.Remove(claimProc);
			}
			if(claimProc.Status!=ClaimProcStatus.NotReceived) {
				return true;
			}
			ClaimProc claimProcInitial=formClaimProc.ClaimProcInitial;//Unedited ClaimProc with base estimates calculated in FormClaimProc.
			bool isClaimProcRecalcNeeded=false;
			isClaimProcRecalcNeeded=IsRecalcNeeded(claimProc.DedApplied,claimProc.DedEstOverride,claimProc.DedEst,
				claimProcInitial.DedEstOverride,claimProcInitial.DedEst);
			isClaimProcRecalcNeeded|=IsRecalcNeeded(claimProc.InsPayEst,claimProc.InsEstTotalOverride,claimProc.InsEstTotal,
				claimProcInitial.InsEstTotalOverride,claimProcInitial.InsEstTotal);
			isClaimProcRecalcNeeded|=IsRecalcNeeded(claimProc.WriteOff,claimProc.WriteOffEstOverride,claimProc.WriteOffEst,
				claimProcInitial.WriteOffEstOverride,claimProcInitial.WriteOffEst);
			if(isClaimProcRecalcNeeded) {//Claim Procedure values have changed.
				//We require that the user clicks Recalculate Estimates to remove this error due to the complexity of how ClaimProcs are edited
				//and how their base estimates are calulated.  Ideally we would be able to loop through ClaimProcList and and check for conflicting
				//estimates and values.However we can not do this currently due to FormClaimProc handling the base estimate calculations in ComputeAmounts()
				//and that we save the ClaimProc to the DB when exiting the edit window.
				_recalcErrorProvider.SetError(this.butRecalc,Lan.g(this,
				 "Claim procedures values have been changed.\n"
				 +"Modified values can result in claim totals not matching claim procedure sums.\n"
				 +"Click to recalculate the claim totals and claim procedure estimates\n"
				 +"Otherwise, ignore to leave values unchanged."));
			}
			return true;
		}
		
		private bool IsRecalcNeeded(double claimInfoAmt,double overrideAmt,double baseAmt,double initOverrideAmt,double initBaseAmt) {
			//Get pertinent base amount.
			double initAmt=initBaseAmt;
			if(initOverrideAmt!=-1) {
				initAmt=initOverrideAmt;
			}
			//Get pertinent initial amount.
			double amt=baseAmt;
			if(overrideAmt!=-1) {
				amt=overrideAmt;
			}
			//Verify that the amount was changed by the user.
			if(CompareDouble.IsEqual(amt,initAmt)) {
				return false;
			}
			//If pertinent amount is different than claim info amount, then the user probably wants to recalculate, since the claimproc is NotReceived.
			if(!CompareDouble.IsEqual(claimInfoAmt,amt)) {
				return true;
			}
			return false;
		}

		private void gridPay_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Row>_dataTablePayments.Rows.Count-1){
				return;//prevents crash after deleting a check?
			}
			for(int i=0;i<_listClaimProcsForClaim.Count;i++){
				if(_listClaimProcsForClaim[i].ClaimPaymentNum.ToString()==_dataTablePayments.Rows[e.Row]["ClaimPaymentNum"].ToString())
					gridProc.SetSelected(i,true);
				else
					gridProc.SetSelected(i,false);
			}
		}

		private void gridPay_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			long tempClaimNum=_claim.ClaimNum;
			ClaimPayment claimPayment=ClaimPayments.GetOne(PIn.Long(_dataTablePayments.Rows[e.Row]["ClaimPaymentNum"].ToString()));
			using FormClaimPayBatch formClaimPayBatch=new FormClaimPayBatch(claimPayment);
			//FormClaimPayEditOld FormCPE=new FormClaimPayEditOld(claimPaymentCur);
			//Security handled in that form.  Anyone can view.
			formClaimPayBatch.IsFromClaim=true;//but not IsNew.
			formClaimPayBatch.ShowDialog();
			//FormCPE.OriginatingClaimNum=ClaimCur.ClaimNum;
			//FormCPE.ShowDialog();
			_listClaimProcs=ClaimProcs.Refresh(_patient.PatNum);
			FillGrids();
		}

		private void butOtherCovChange_Click(object sender, System.EventArgs e) {
			using FormInsPlanSelect formInsPlanSelect=new FormInsPlanSelect(_patient.PatNum);
			formInsPlanSelect.ShowDialog();
			if(formInsPlanSelect.DialogResult!=DialogResult.OK){
				return;
			}
			_claim.PlanNum2=formInsPlanSelect.InsPlanSelected.PlanNum;
			_claim.InsSubNum2=formInsPlanSelect.InsSubSelected.InsSubNum;
			textPlan2.Text=InsPlans.GetDescript(_claim.PlanNum2,_family,_listInsPlans,_claim.InsSubNum2,_listInsSubs);
			if(textPlan2.Text==""){
				comboPatRelat2.Visible=false;
				label10.Visible=false;
			}
			else{
				comboPatRelat2.Visible=true;
				label10.Visible=true;
			}
		}

		private void butOtherNone_Click(object sender, System.EventArgs e) {
			_claim.PlanNum2=0;
			_claim.InsSubNum2=0;
			_claim.PatRelat2=Relat.Self;
			textPlan2.Text="";
			comboPatRelat2.Visible=false;
			label10.Visible=false;
		}

		private void butPayTotal_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPayCreate)){//date not checked here, but it will be checked when actually creating the check
				return;
			}
			//preauths are only allowed "payment" entry by procedure since a total would be meaningless
			if(_claim.ClaimType=="PreAuth"){
				MessageBox.Show(Lan.g(this,"PreAuthorizations can only be entered by procedure."));
				return;
			}
			if(DoConsolidateOrthoPayments()) {
				InsPlan insPlan = InsPlans.GetPlan(_claim.PlanNum,_listInsPlans);
				long orthoAutoCodeNum = InsPlans.GetOrthoAutoProc(insPlan);
				//if all the procedures on this claim are ortho auto procedures...
				if(_listClaimProcsForClaim.All(x => Procedures.GetProcFromList(_listProcedures,x.ProcNum).CodeNum == orthoAutoCodeNum)) {
					MsgBox.Show(this,"You may not attach payments to this claim. Please attach this payment to the claim with the original banding procedure.");
					return;
				}
			}
			bool hasMadePayment;
			bool hasCapClaimProcs;
			hasCapClaimProcs=_listClaimProcsForClaim.Exists(cp => cp.Status == ClaimProcStatus.CapClaim); //check for CapClaim
			if(PrefC.GetBool(PrefName.ClaimPayByTotalSplitsAuto) && !hasCapClaimProcs) {//Determines if the user prefers line item accounting.
				if(_listClaimProcsForClaim.Any(x => x.Status==ClaimProcStatus.Received)) {//This prevents the user from making multiple as total payments if there are recieved claims procs.
					MsgBox.Show("As Total payments cannot be made on received claims. Use Supplemental payments instead.");
					return;
				} 
				else {
					hasMadePayment=PayAsTotalToClaimProcsExplicitly();
				}
			}
			else {
				hasMadePayment=PayAsTotal();
			}
			if(hasMadePayment) {
				_isPaymentEntered=true;
				comboClaimStatus.SelectedIndex=_listClaimStatuses.IndexOf(ClaimStatus.Received);//Received
				if(textDateRec.Text==""){
					textDateRec.Text=DateTime.Today.ToShortDateString();
				}
			}
			_listClaimProcs=ClaimProcs.Refresh(_patient.PatNum);
			FillGrids();
		}

		private bool PayAsTotalToClaimProcsExplicitly() { 
			List<InputBoxParam> listInputBoxParams=new List<InputBoxParam>();
			InputBoxParam inputBoxParam=new InputBoxParam(InputBoxType.ValidDouble,Lan.g(this,"Please enter an amount: "));
			listInputBoxParams.Add(inputBoxParam);
			Func<string, bool> funcOkClick=new Func<string, bool>((text) => {
				if(PIn.Double(text)<0) {
					MsgBox.Show(this,"Please enter a value greater than or equal to 0.");
					return false;//Should stop user from continuing to payment window.
				}
				return true;//Allow user to the payment window.
			});
			using InputBox inputBox=new InputBox(listInputBoxParams,funcOkClick);
			if(inputBox.ShowDialog()!=DialogResult.OK) {
				return false;
			}
			double result=PIn.Double(inputBox.textResult.Text);
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			for(int i=0;i<_listClaimProcsForClaim.Count;i++) {
				listClaimProcs.Add(_listClaimProcsForClaim[i].Copy());
			}
			using FormClaimPayTotal formClaimPayTotal=new FormClaimPayTotal(_patient,_family,_listInsPlans,_listPatPlans,_listInsSubs,GetBlueBookEstimateData(),totalPayAmt:result);
			formClaimPayTotal.ClaimProcArray=_listClaimProcsForClaim.ToArray(); //Might want to filter out all things that aren't marked as recieved or supplemental
			formClaimPayTotal.ShowDialog();
			if(formClaimPayTotal.DialogResult!=DialogResult.OK){
				SetListClaimProcsForClaim(listClaimProcs);
				return false;
			} 
			else {
				for(int i=0;i<formClaimPayTotal.ClaimProcArray.Length;i++){
					ClaimProcs.Update(formClaimPayTotal.ClaimProcArray[i]);
				}
			}
			ClaimProcs.RemoveSupplementalTransfersForClaims(_claim.ClaimNum);
			return true;
		}

		private bool PayAsTotal() { 
			Double dedEst=0;
			Double payEst=0;
			for(int i=0;i<_listClaimProcsForClaim.Count;i++){
				if(_listClaimProcsForClaim[i].Status!=ClaimProcStatus.NotReceived){
					continue;
				}
				if(_listClaimProcsForClaim[i].ProcNum==0){
					continue;//also ignore non-procedures.
				}
				//ClaimProcs.Cur=ClaimProcs.ForClaim[i];
				dedEst+=_listClaimProcsForClaim[i].DedApplied;
				payEst+=_listClaimProcsForClaim[i].InsPayEst;
			}
			ClaimProc claimProc=new ClaimProc();
			claimProc.IsNew=true;
			//ClaimProcs.Cur.ProcNum 
			claimProc.ClaimNum=_claim.ClaimNum;
			claimProc.PatNum=_claim.PatNum;
			claimProc.ProvNum=_claim.ProvTreat;
			//ClaimProcs.Cur.FeeBilled
			//ClaimProcs.Cur.InsPayEst
			claimProc.DedApplied=dedEst;
			claimProc.Status=ClaimProcStatus.Received;
			claimProc.InsPayAmt=payEst;
			//remarks
			//ClaimProcs.Cur.ClaimPaymentNum
			claimProc.PlanNum=_claim.PlanNum;
			claimProc.InsSubNum=_claim.InsSubNum;
			claimProc.DateCP=DateTime.Today;
			claimProc.ProcDate=_claim.DateService;
			claimProc.DateEntry=DateTime.Now;//will get set anyway
			claimProc.ClinicNum=_claim.ClinicNum;
			claimProc.SecDateEntry=_claim.SecDateEntry;//Manually setting this because FormClaimProc checks this field for security.
			//Automatically set PayPlanNum if there is a payplan with matching PatNum, PlanNum, and InsSubNum that has not been paid in full.
			//By sending in ClaimNum, we ensure that we only get the payplan a claimproc from this claim was already attached to or payplans with no claimprocs attached.
			List<PayPlan> listPayPlans=PayPlans.GetValidInsPayPlans(claimProc.PatNum,claimProc.PlanNum,claimProc.InsSubNum,claimProc.ClaimNum);
			claimProc.PayPlanNum=0;
			if(listPayPlans.Count==1) {
				claimProc.PayPlanNum=listPayPlans[0].PayPlanNum;
			}
			else if(listPayPlans.Count>1) {
				//more than one valid PayPlan
				using FormPayPlanSelect formPayPlanSelect=new FormPayPlanSelect(listPayPlans);
				formPayPlanSelect.ShowDialog();
				if(formPayPlanSelect.DialogResult==DialogResult.OK) {
					claimProc.PayPlanNum=formPayPlanSelect.PayPlanNumSelected;
				}
			}
			ClaimProcs.Insert(claimProc);
			List<ClaimProcHist> listClaimProcHistsLoop=null;
			using FormClaimProc formClaimProc=new FormClaimProc(claimProc,null,_family,_patient,_listInsPlans,null,ref listClaimProcHistsLoop,_listPatPlans,true,_listInsSubs);
			formClaimProc.IsInClaim=true;
			formClaimProc.IsCalledFromClaimEdit=true;
			formClaimProc.ShowDialog();
			if(formClaimProc.DialogResult==DialogResult.Abort) {//Claim was deleted. Close claim edit window.
				ClaimProcs.Delete(claimProc);
				DialogResult=DialogResult.Abort;
				return false;
			}
			else if(formClaimProc.DialogResult!=DialogResult.OK){
				ClaimProcs.Delete(claimProc);
				return false;
			}
			else{//Claim still exists, and user didn't click cancel
				for(int i=0;i<_listClaimProcsForClaim.Count;i++){
					if(_listClaimProcsForClaim[i].Status!=ClaimProcStatus.NotReceived){
						continue;
					}
					//ClaimProcs.Cur=ClaimProcs.ForClaim[i];
					_listClaimProcsForClaim[i].Status=ClaimProcStatus.Received;
					if(_listClaimProcsForClaim[i].DedApplied>0){
						_listClaimProcsForClaim[i].InsPayEst+=_listClaimProcsForClaim[i].DedApplied;
						_listClaimProcsForClaim[i].DedApplied=0;//because ded will show as part of payment now.
					}
					_listClaimProcsForClaim[i].DateEntry=DateTime.Now;//the date is was switched to rec'd
					ClaimProcs.Update(_listClaimProcsForClaim[i]);
				}
			}
			return true;
		}

		private void butPayProc_Click(object sender, System.EventArgs e) {
			if(_claim.ClaimType!="PreAuth" && !Security.IsAuthorized(Permissions.InsPayCreate)){//date not checked here, but it will be checked when actually creating the check
				return;
			}
			//this will work for regular claims and for preauths.
			//it will enter edit mode if it can only find received procs not attached to payments yet.
			if(gridProc.SelectedIndices.Length==0){
				//first, autoselect rows if not received:
				for(int i=0;i<_listClaimProcsForClaim.Count;i++){
					if(_listClaimProcsForClaim[i].Status==ClaimProcStatus.NotReceived
						&& _listClaimProcsForClaim[i].ProcNum>0){//and is procedure
						gridProc.SetSelected(i,true);
					}
				}
			}
			if(gridProc.SelectedIndices.Length==0){
				//then, autoselect rows if not paid on:
				for(int i=0;i<_listClaimProcsForClaim.Count;i++){
					if(_listClaimProcsForClaim[i].ClaimPaymentNum==0
						&& _listClaimProcsForClaim[i].ProcNum>0){//and is procedure
						gridProc.SetSelected(i,true);
					}
				}
			}
			if(gridProc.SelectedIndices.Length==0){
				//if still no rows selected
				MessageBox.Show(Lan.g(this,"All procedures in the list have already been paid."));
				return;
			}
			bool areAllProcs=true;
			for(int i=0;i<gridProc.SelectedIndices.Length;i++){
				if(_listClaimProcsForClaim[gridProc.SelectedIndices[i]].ProcNum==0)
					areAllProcs=false;
			}
			if(!areAllProcs){
				MessageBox.Show(Lan.g(this,"You can only select procedures."));
				return;
			}
			for(int i=0;i<gridProc.SelectedIndices.Length;i++) {
				if(_listClaimProcsForClaim[gridProc.SelectedIndices[i]].ClaimPaymentNum!=0) {//if attached to a check
					MessageBox.Show(Lan.g(this,"Procedures that are attached to checks cannot be included."));
					return;
				}
			}
			for(int i=0;i<gridProc.SelectedIndices.Length;i++) {
				if(_listClaimProcsForClaim[gridProc.SelectedIndices[i]].Status==ClaimProcStatus.Received
					|| _listClaimProcsForClaim[gridProc.SelectedIndices[i]].Status==ClaimProcStatus.Supplemental
					|| _listClaimProcsForClaim[gridProc.SelectedIndices[i]].Status==ClaimProcStatus.CapComplete) 
				{
					MessageBox.Show(Lan.g(this,"Procedures that are already received cannot be included."));
					//This expanded security prevents making changes to historical entries of zero with a writeoff.
					return;
				}
			}
			SelectLabProcs();
			#region OrthoInsPayConsolidated
			if(DoConsolidateOrthoPayments()) {
				List<int> listIntOrthoAutoGridRows = new List<int>();
				InsPlan insPlan = InsPlans.GetPlan(_claim.PlanNum,_listInsPlans);
				long orthoAutoCodeNum = InsPlans.GetOrthoAutoProc(insPlan);
				for(int i = 0;i<gridProc.SelectedIndices.Length;i++) {
					//if all the procedures on this claim are ortho auto procedures...
					if(Procedures.GetProcFromList(_listProcedures,_listClaimProcsForClaim[gridProc.SelectedIndices[i]].ProcNum).CodeNum == orthoAutoCodeNum) {
						listIntOrthoAutoGridRows.Add(i);
					}
				}
				if(listIntOrthoAutoGridRows.Count>0) {
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"You have selected an ortho auto procedure with consolidated insurance payments turned on. "
						+"Payments for this procedure must be made to the claim with the original banding procedure. Would you like to go to that claim now?"))
					{
						return;
					}
					//get the most recent claim marked as ortho with an ortho banding proc on it.
					Claim claimBanding =Claims.GetOrthoBandingClaim(_patient.PatNum,insPlan.PlanNum);
					if(claimBanding==null) {
						MsgBox.Show(this,"The claim with the original banding procedure was not marked as received or could not be found."
							+"\r\nPlease manually add the payment to that claim.");
					}
					else {//Show the claim edit window and have the insurance payment window automatically show.
						if(!Security.IsAuthorized(Permissions.ClaimView)) {//this should ever get hit if this permission is not granted, but just in case
							return;
						}
						using FormClaimEdit formClaimEdit=new FormClaimEdit(claimBanding,_patient,_family,true);
						formClaimEdit.ShowDialog();
					}
					//Zero out ortho auto claimprocs on this claim regardless of what happened above.
					//We will simply assume they received the payment on the other claim correctly.
					for(int i=0;i<listIntOrthoAutoGridRows.Count;i++) {
						_listClaimProcsForClaim[listIntOrthoAutoGridRows[i]].Status=ClaimProcStatus.Received;
						_listClaimProcsForClaim[listIntOrthoAutoGridRows[i]].DateEntry=DateTime.Now;//date is was set rec'd
						_listClaimProcsForClaim[listIntOrthoAutoGridRows[i]].InsPayAmt=0;
						ClaimProcs.Update(_listClaimProcsForClaim[listIntOrthoAutoGridRows[i]]);
						//Deselect any ortho auto claimprocs so that the regular "payment" code below doesn't act on ortho auto procs.
						gridProc.SetSelected(listIntOrthoAutoGridRows[i],false);
					}
					ClaimProcs.RemoveSupplementalTransfersForClaims(_claim.ClaimNum);
					}
			}
			#endregion
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			for(int i=0;i<gridProc.SelectedIndices.Length;i++) {
				//copy selected claimprocs to temporary array for editing.
				//no changes to the database will be made within that form.
				listClaimProcs.Add(_listClaimProcsForClaim[gridProc.SelectedIndices[i]].Copy());
				if(_claim.ClaimType=="PreAuth"){
					listClaimProcs[i].Status=ClaimProcStatus.Preauth;
				}
				else if(_claim.ClaimType=="Cap"){
					;//do nothing.  The claimprocstatus will remain Capitation.
				}
				else{
					listClaimProcs[i].Status=ClaimProcStatus.Received;
					listClaimProcs[i].DateEntry=DateTime.Now;//date is was set rec'd
					listClaimProcs[i].InsPayAmt=listClaimProcs[i].InsPayEst;
					listClaimProcs[i].PayPlanNum=0;
					if(i==0) {
						//Automatically set PayPlanNum if there is a payplan with matching PatNum, PlanNum, and InsSubNum that has not been paid in full.
						//By sending in ClaimNum, we ensure that we only get the payplan a claimproc from this claim was already attached to or payplans with no claimprocs attached.
						List<PayPlan> listPayPlans=PayPlans.GetValidInsPayPlans(listClaimProcs[i].PatNum,listClaimProcs[i].PlanNum,listClaimProcs[i].InsSubNum,listClaimProcs[i].ClaimNum);
						if(listPayPlans.Count==1) {
							listClaimProcs[i].PayPlanNum=listPayPlans[0].PayPlanNum;
						}
						else if(listPayPlans.Count>1) {
							//more than one valid PayPlan
							using FormPayPlanSelect formPayPlanSelect=new FormPayPlanSelect(listPayPlans);
							formPayPlanSelect.ShowDialog();
							if(formPayPlanSelect.DialogResult==DialogResult.OK) {
								listClaimProcs[i].PayPlanNum=formPayPlanSelect.PayPlanNumSelected;
							}
						}
					}
					else {
						listClaimProcs[i].PayPlanNum=listClaimProcs[0].PayPlanNum;//set all procs to the same payplan, they can change it later if not correct for each claimproc that is different
					}
				}
				listClaimProcs[i].DateCP=DateTime.Today;
			}
			if(listClaimProcs.Count > 0) {
				if(_claim.ClaimType=="PreAuth") {
					using FormClaimPayPreAuth formClaimPayPreAuth=new FormClaimPayPreAuth(_patient,_family,_listInsPlans,_listPatPlans,_listInsSubs);
					formClaimPayPreAuth.ListClaimProcs=listClaimProcs;
					formClaimPayPreAuth.ShowDialog();
					if(formClaimPayPreAuth.DialogResult!=DialogResult.OK) {
						return;
					}
					//save changes now
					for(int i=0;i<formClaimPayPreAuth.ListClaimProcs.Count;i++) {
						ClaimProcs.Update(formClaimPayPreAuth.ListClaimProcs[i]);
						ClaimProcs.SetInsEstTotalOverride(formClaimPayPreAuth.ListClaimProcs[i].ProcNum,formClaimPayPreAuth.ListClaimProcs[i].PlanNum,
							formClaimPayPreAuth.ListClaimProcs[i].InsSubNum,formClaimPayPreAuth.ListClaimProcs[i].InsPayEst,_listClaimProcs);
					}
				}
				else {
					using FormClaimPayTotal formClaimPayTotal=new FormClaimPayTotal(_patient,_family,_listInsPlans,_listPatPlans,_listInsSubs,GetBlueBookEstimateData());
					formClaimPayTotal.IsCalledFromClaimEdit=true;
					formClaimPayTotal.ClaimProcArray=listClaimProcs.ToArray();
					formClaimPayTotal.ShowDialog();
					if(formClaimPayTotal.DialogResult!=DialogResult.OK){
						return;
					}
					//save changes now
					for(int i=0;i<formClaimPayTotal.ClaimProcArray.Length;i++){
						ClaimProcs.Update(formClaimPayTotal.ClaimProcArray[i]);
					}
				}
			}
			_isPaymentEntered=true;
			comboClaimStatus.SelectedIndex=_listClaimStatuses.IndexOf(ClaimStatus.Received);//Received
			if(textDateRec.Text==""){
				textDateRec.Text=DateTime.Today.ToShortDateString();
			}
			ClaimProcs.RemoveSupplementalTransfersForClaims(_claim.ClaimNum);
			_listClaimProcs=ClaimProcs.Refresh(_patient.PatNum);
			if(!AreAllReceived()) {
				if(MsgBox.Show(MsgBoxButtons.YesNo,"Not all claim procedures on the claim are marked received. Would you like to mark them all as received?")) {
					MarkAllReceived();
				}
			}
			FillGrids();
		}

		private void butPaySupp_Click(object sender, System.EventArgs e) {
			if(DoConsolidateOrthoPayments()) {
				InsPlan insPlanCur=InsPlans.GetPlan(_claim.PlanNum,_listInsPlans);
				long orthoAutoCodeNum=InsPlans.GetOrthoAutoProc(insPlanCur);
				//if all the procedures on this claim are ortho auto procedures...
				if(_listClaimProcsForClaim.All(x => Procedures.GetProcFromList(_listProcedures,x.ProcNum).CodeNum == orthoAutoCodeNum)) {
					MsgBox.Show(this,"You may not attach supplemental payments to this claim. Please attach this payment to the claim with the original banding procedure.");
					return;
				}
			}
			MakeSuppPayment();
		}

		private void menuItemAddAdj_Click(object sender,EventArgs e) {
			//check to make sure one and only one row is selected
			if(gridProc.SelectedGridRows.Count!=1) {
				MsgBox.Show(this,"Select one procedure to add an adjustment.");
				return;
			}
			long procNum=((ClaimProc)gridProc.SelectedGridRows[0].Tag).ProcNum;
			Procedure procedureForAdj=new Procedure();
			procedureForAdj=Procedures.GetOneProc(procNum,false);
			if(procedureForAdj.ProcStatus!=ProcStat.C) {
				MsgBox.Show(this,"Adjustments may only be added to completed procedures.");
				return;
			}
			Adjustment adjustment=new Adjustment();
			adjustment.PatNum=_patient.PatNum;
			adjustment.ProvNum=procedureForAdj.ProvNum;
			adjustment.DateEntry=DateTime.Today;//but will get overwritten to server date
			adjustment.AdjDate=DateTime.Today;
			adjustment.ProcDate=procedureForAdj.ProcDate;
			adjustment.ProcNum=procedureForAdj.ProcNum;
			adjustment.ClinicNum=procedureForAdj.ClinicNum;
			using FormAdjust formAdjust=new FormAdjust(_patient,adjustment);
			formAdjust.IsNew=true;
			formAdjust.ShowDialog();
		}

		private void MakeSuppPayment() {
			if(!Security.IsAuthorized(Permissions.InsPayCreate)){//date not checked here, but it will be checked when actually creating the check
				return;
			}
			if(gridProc.SelectedIndices.Length==0){
				MessageBox.Show(Lan.g(this,"This is only for additional payments on procedures already marked received.  Please highlight procedures first."));
				return;
			}
			bool areAllRecd=true;
			bool hasClickedTotalPayment=false;
			for(int i=0;i<gridProc.SelectedIndices.Length;i++){
				if(_listClaimProcsForClaim[gridProc.SelectedIndices[i]].Status!=ClaimProcStatus.Received) {
					areAllRecd=false;
				}
				if(_listClaimProcsForClaim[gridProc.SelectedIndices[i]].ProcNum==0) { //total payment
					hasClickedTotalPayment=true;
				}
			}
			if(!areAllRecd){
				MessageBox.Show(Lan.g(this,"All selected procedures must be status received."));
				return;
			}
			if(hasClickedTotalPayment) {
				MessageBox.Show(Lan.g(this,"Select the procedures that you want to enter a supplemental payment for.  If you want to make a supplemental "
					+"payment on a payment previously entered as total, click 'As Total' again."));
				return;
			}
			List<ClaimProc> listClaimProcsSelected=gridProc.SelectedIndices.Select(x => _listClaimProcsForClaim[x]).ToList();
			List<ClaimProc> listClaimProcsSupplemental=ClaimProcs.CreateSuppClaimProcs(listClaimProcsSelected);
			using FormClaimPayTotal formClaimPayTotal=new FormClaimPayTotal(_patient,_family,_listInsPlans,_listPatPlans,_listInsSubs,GetBlueBookEstimateData());
			formClaimPayTotal.ClaimProcArray=listClaimProcsSupplemental.ToArray();
			formClaimPayTotal.ShowDialog();
			if(formClaimPayTotal.DialogResult!=DialogResult.OK) {
				//ClaimProcs were inserted already so we need to delete them if user clicks 'Cancel'.
				ClaimProcs.DeleteMany(listClaimProcsSupplemental);
				FillGrids();
				return;
			}
			_isPaymentEntered=true;
			//save changes now
			for(int i=0;i<formClaimPayTotal.ClaimProcArray.Length;i++) {
				ClaimProcs.Update(formClaimPayTotal.ClaimProcArray[i]);
			}
//fix: need to debug the recalculation feature to take this status into account.
			ClaimProcs.RemoveSupplementalTransfersForClaims(_claim.ClaimNum);
			_listClaimProcs=ClaimProcs.Refresh(_patient.PatNum);
			FillGrids();
		}

		private void butSplit_Click(object sender, System.EventArgs e) {
			if(!ClaimIsValid()){
				return;
			}
			UpdateClaim();
			if(gridProc.SelectedIndices.Length==0){
				MessageBox.Show(Lan.g(this,"Please highlight procedures first."));
				return;
			}
			List<long> listSelectedProcNums=new List<long>();
			for(int i=0;i<gridProc.SelectedIndices.Length;i++){
				if(_listClaimProcsForClaim[gridProc.SelectedIndices[i]].ProcNum==0){
					MessageBox.Show(Lan.g(this,"Only procedures can be selected."));
					return;
				}
				if(_listClaimProcsForClaim[gridProc.SelectedIndices[i]].InsPayAmt!=0){
					MessageBox.Show(Lan.g(this,"All selected procedures must have zero insurance payment amounts."));
					return;
				}
				listSelectedProcNums.Add(_listClaimProcsForClaim[gridProc.SelectedIndices[i]].ProcNum);
			}
			//Make sure that there is at least one procedure left on the claim before splitting.
			//The claim would become orphaned if we allow users to split off all procedures on the claim and DBM would be required to run to clean up.
			bool hasProcLeft=false;
			for(int i=0;i<_listClaimProcsForClaim.Count;i++) {
				if(!listSelectedProcNums.Contains(_listClaimProcsForClaim[i].ProcNum)) {
					hasProcLeft=true;
					break;
				}
			}
			if(!hasProcLeft) {//All procedures are selected for the split...
				MsgBox.Show(this,"At least one procedure needs to remain on this claim in order to split it.");
				return;
			}
			//Selection validation logic ensures these are only procs that are eligible to split from this claim.
			Claims.InsertSplitClaim(_claim,gridProc.SelectedTags<ClaimProc>());
			_listClaimProcs=ClaimProcs.Refresh(_patient.PatNum);
			List<long> listModifiedClaimNums=new List<long>();
			//Update the multi-visit groups
			for(int i=0;i<_listClaimProcs.Count;i++) {
				Procedure procedure=Procedures.GetOneProc(_listClaimProcs[i].ProcNum,false);
				List<long> listGroupModifiedClaimNums=ProcMultiVisits.UpdateGroupForProc(procedure.ProcNum,procedure.ProcStatus);
				listModifiedClaimNums.AddRange(listGroupModifiedClaimNums);
			}
			if(listModifiedClaimNums.Contains(_claim.ClaimNum)) {
				Claim claim=Claims.GetClaim(_claim.ClaimNum);//Refresh claim from database to get current status since it might have changed.
				ClaimStatus claimStatus=_listClaimStatuses.Where(x=>x.GetDescription(useShortVersionIfAvailable:true)==claim.ClaimStatus).FirstOrDefault();
				comboClaimStatus.SelectedIndex=_listClaimStatuses.IndexOf(claimStatus);
			}	
			FillGrids();
		}

		private void butViewEra_Click(object sender,EventArgs e) {
			EtransL.ViewEra(_claim);
		}
		
		private void butViewEob_Click(object sender,EventArgs e) {
			List<long> listLongClaimPaymentNums=_listClaimProcsForClaim.FindAll(x => x.ClaimPaymentNum!=0).Select(x => x.ClaimPaymentNum).ToList();
			List<ClaimPayment> listClaimPayments=ClaimPayments.GetClaimPaymentsWithEobAttach(listLongClaimPaymentNums);
			ClaimPayment claimPayment=null;
			switch(listClaimPayments.Count) {
				case 0:
					MsgBox.Show(this,"There are no EOBs for this claim.");
					return;
				case 1:
					claimPayment=listClaimPayments[0];
					break;
				default:
					if(!TrySelectClaimPayment(listClaimPayments,out claimPayment)) {
						return;
					}
					break;
			}
			//Mimics FormClaimPayBatch.butView_Click(...)
			using FormImages formImages=new FormImages();
			formImages.ClaimPaymentNum=claimPayment.ClaimPaymentNum;
			formImages.ShowDialog();
		}

		///<summary>Called when there are multiple ClaimPayments that have EOBs attached to them and we don't know what one the user would like to view.</summary>
		private bool TrySelectClaimPayment(List<ClaimPayment> listClaimPayments,out ClaimPayment claimPayment) {
			claimPayment=null;
			List<GridColumn> listGridColumnHeaders=new List<GridColumn>() {
				new GridColumn(Lan.g(this,"Carrier"),80){ IsWidthDynamic=true },
				new GridColumn(Lan.g(this,"Check Date"),70,HorizontalAlignment.Center),
				new GridColumn(Lan.g(this,"Amount"),80,HorizontalAlignment.Right)
			};
			List<GridRow> listGridRowValues=new List<GridRow>();
			listClaimPayments.ForEach(x => {
				GridRow row=new GridRow(x.CarrierName,x.CheckDate.ToShortDateString(),POut.Double(x.CheckAmt));
				row.Tag=x;
				listGridRowValues.Add(row);
			});
			string formTitle=Lan.g(this,"EOB Picker");
			string gridTitle=Lan.g(this,"EOBs");
			using FormGridSelection formGridSelection=new FormGridSelection(listGridColumnHeaders,listGridRowValues,formTitle,gridTitle);
			if(formGridSelection.ShowDialog()!=DialogResult.OK) {
				return false;
			}
			claimPayment=(ClaimPayment)formGridSelection.ListSelectedTags[0];//DialogResult.OK means a selection was made.
			return true;
		}

		/*
		///<summary>Creates insurance check</summary>
		private void butCheckAdd_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPayCreate)){//date not checked here, but it will be checked when saving the check to prevent backdating
				return;
			}
			bool existsReceived=false;
			for(int i=0;i<ClaimProcsForClaim.Count;i++){
				if((ClaimProcsForClaim[i].Status==ClaimProcStatus.Received
					|| ClaimProcsForClaim[i].Status==ClaimProcStatus.Supplemental)
					&& ClaimProcsForClaim[i].InsPayAmt!=0)
				{
					existsReceived=true;
				}
			}
			if(!existsReceived){
				MessageBox.Show(Lan.g(this,"There are no valid received payments for this claim."));
				return;
			}
			long tempClaimNum=ClaimCur.ClaimNum;
			ClaimPayment ClaimPaymentCur=new ClaimPayment();
			ClaimPaymentCur.CheckDate=DateTime.Today;
			ClaimPaymentCur.ClinicNum=PatCur.ClinicNum;
			ClaimPaymentCur.CarrierName=Carriers.GetName(InsPlans.GetPlan(ClaimCur.PlanNum,PlanList).CarrierNum);
			ClaimPayments.Insert(ClaimPaymentCur);
			FormClaimPayEditOld FormCPE=new FormClaimPayEditOld(ClaimPaymentCur);
			FormCPE.OriginatingClaimNum=ClaimCur.ClaimNum;
			FormCPE.IsNew=true;
			FormCPE.ShowDialog();
			//ClaimPaymentCur gets deleted within that form if user clicks cancel.
			ClaimList=Claims.Refresh(PatCur.PatNum);
			ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
			FillGrids();
		}*/

		private void butBatch_Click(object sender,EventArgs e) {
			FinalizePayment(false);
		}

		private void butThisClaimOnly_Click(object sender,EventArgs e) {
			FinalizePayment(true);
		}

		private void FinalizePayment(bool isThisClaimOnly) {
			if(!Security.IsAuthorized(Permissions.InsPayCreate)) {//date not checked here, but it will be checked when saving the check to prevent backdating
				return;
			}
			if(PrefC.GetBool(PrefName.ClaimPaymentBatchOnly)) {
				//Is there a permission in the manage module that would block this behavior? Are we sending the user into a TRAP?!
				MsgBox.Show(this,"Please use Batch Insurance in Manage Module to Finalize Payments.");
				return;
			}
			if(!ClaimIsValid()) {
				return;
			}
			UpdateClaim();
			if(!_listClaimProcsForClaim.Any(x => ClaimProcs.GetInsPaidStatuses().Contains(x.Status))) {
				MessageBox.Show(Lan.g(this,"There are no valid received payments for this claim."));
				return;
			}
			ClaimPayment claimPayment=new ClaimPayment();
			claimPayment.CheckDate=MiscData.GetNowDateTime().Date;//Today's date for easier tracking by the office and to avoid backdating before accounting lock dates.
			claimPayment.IsPartial=true;
			claimPayment.ClinicNum=_claim.ClinicNum;
			claimPayment.CarrierName=Carriers.GetName(InsPlans.GetPlan(_claim.PlanNum,_listInsPlans).CarrierNum);
			ClaimPayments.Insert(claimPayment);
			long onlyOneClaimNum=0;
			if(isThisClaimOnly) {
				onlyOneClaimNum=_claim.ClaimNum;
			}
			List<ClaimProc> listClaimProcs=ClaimProcs.AttachAllOutstandingToPayment(claimPayment.ClaimPaymentNum,PrefC.DateClaimReceivedAfter,onlyOneClaimNum);
			if(listClaimProcs.Count==0) {
				MsgBox.Show(this,"There are no insurance payments to attach. Older insurance payments may be filtered out due to the Preference\r\n"+
					"'Show claims received after days (blank to disable)'.");
				try {
					ClaimPayments.Delete(claimPayment);
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
				}
				return;
			}
			double amt=listClaimProcs.Sum(x => x.InsPayAmt);
			claimPayment.CheckAmt=amt;
			try {
				ClaimPayments.Update(claimPayment);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			FormFinalizePaymentHelper(claimPayment,_claim,_patient,_family,onlyOneClaimNum);
			DialogResult=DialogResult.OK;
		}

		///<summary>Called after finalizing an insurance payment to bring up various forms for the user.
		///If finalizing an individual claim (Edit Claim window only), then onlyOneClaimNum must be the current ClaimNum.</summary>
		public static void FormFinalizePaymentHelper(ClaimPayment claimPayment,Claim claim,Patient patient,Family family,long onlyOneClaimNum=0) {
			using FormClaimPayEdit formClaimPayEdit=new FormClaimPayEdit(claimPayment);
			formClaimPayEdit.IsCreateLogEntry=true;
			formClaimPayEdit.IsFinalizePayment=true;
			//FormCPE.IsNew=true;//not new.  Already added.
			formClaimPayEdit.ShowDialog();
			if(formClaimPayEdit.DialogResult!=DialogResult.OK) {
				try {
					ClaimPayments.Delete(claimPayment);
				}
				catch(ApplicationException ex) {
					MessageBox.Show(ex.Message);
				}
				return;
			}
			using FormClaimPayBatch formClaimPayBatch=new FormClaimPayBatch(claimPayment,true);
			formClaimPayBatch.IsFromClaim=true;
			formClaimPayBatch.IsNew=true;
			formClaimPayBatch.ShowDialog();
			if(formClaimPayBatch.DialogResult!=DialogResult.OK) {
				//The user attached EOBs to the new claim payment and then clicked cancel. Then the user was asked if they wanted to delete the payment and they chose yes.
				//Since we are deleting the claim payment we must remove the attached EOBs or else ClaimPayments.Delete() will throw an exception.
				List<EobAttach> listEobAttaches=EobAttaches.Refresh(claimPayment.ClaimPaymentNum);
				for(int i=0;i<listEobAttaches.Count;i++) {
					EobAttaches.Delete(listEobAttaches[i].EobAttachNum);
				}
				try {
					ClaimPayments.Delete(claimPayment);
				}
				catch(ApplicationException ex) {
					MessageBox.Show(ex.Message);
				}
				return;
			}
			ClaimProcs.DateInsFinalizedHelper(claimPayment.ClaimPaymentNum,PrefC.DateClaimReceivedAfter,onlyOneClaimNum);
			//ClaimList=Claims.Refresh(PatCur.PatNum);
			//ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
			//FillGrids();
			//At this point we know the user just added an insurance payment.  Check to see if they want the provider transfer window to show.
			ShowProviderTransferWindow(claim,patient,family);
		}

		///<summary>Helper method that shows the payment window if the user has the "Show provider income transfer window after entering insurance payment"
		///preference enabled.  This method should always be called after an insurance payment has been made.</summary>
		public static void ShowProviderTransferWindow(Claim claim,Patient patient, Family family) {
			if(!PrefC.GetBool(PrefName.ProviderIncomeTransferShows) || claim.ClaimType=="PreAuth") {
				return;
			}
			Payment payment=new Payment();
			payment.PayDate=DateTime.Today;
			payment.PatNum=patient.PatNum;
			//Explicitly set ClinicNum=0, since a pat's ClinicNum will remain set if the user enabled clinics, assigned patients to clinics, and then
			//disabled clinics because we use the ClinicNum to determine which PayConnect or XCharge/XWeb credentials to use for payments.
			payment.ClinicNum=0;
			if(PrefC.HasClinicsEnabled) {//if clinics aren't enabled default to 0
				payment.ClinicNum=patient.ClinicNum;
			}
			payment.PayType=0;//txfr
			payment.DateEntry=DateTime.Today;//So that it will show properly in the new window.
			payment.PaymentSource=CreditCardSource.None;
			payment.ProcessStatus=ProcessStat.OfficeProcessed;
			Payments.Insert(payment);
			using FormPayment formPayment=new FormPayment(patient,family,payment,false);
			formPayment.IsNew=true;
			formPayment.ShowDialog();
		}

		private void radioProsthN_Click(object sender, System.EventArgs e) {
			_claim.IsProsthesis="N";
		}

		private void radioProsthI_Click(object sender, System.EventArgs e) {
			_claim.IsProsthesis="I";
		}

		private void radioProsthR_Click(object sender, System.EventArgs e) {
			_claim.IsProsthesis="R";
		}

		private void butReferralNone_Click(object sender,EventArgs e) {
			textRefProv.Text="";
			_claim.ReferringProv=0;
			butReferralEdit.Enabled=false;
		}

		private void butReferralSelect_Click(object sender,EventArgs e) {
			using FormReferralSelect formReferralSelect=new FormReferralSelect();
			formReferralSelect.IsSelectionMode=true;
			formReferralSelect.ShowDialog();
			if(formReferralSelect.DialogResult!=DialogResult.OK) {
				return;
			}
			_claim.ReferringProv=formReferralSelect.ReferralSelected.ReferralNum;
			textRefProv.Text=Referrals.GetNameLF(formReferralSelect.ReferralSelected.ReferralNum);
			butReferralEdit.Enabled=true;
		}

		private void butReferralEdit_Click(object sender,EventArgs e) {
			//only enabled if ClaimCur.ReferringProv!=0
			Referral referral=ReferralL.GetReferral(_claim.ReferringProv);
			if(referral==null) {
				textRefProv.Text="";
				_claim.ReferringProv=0;
				butReferralEdit.Enabled=false;
				return;
			}
			using FormReferralEdit formReferralEdit=new FormReferralEdit(referral);
			formReferralEdit.ShowDialog();
			if(formReferralEdit.DialogResult==DialogResult.OK){
				//it's impossible to delete referral from that window.
				Referrals.RefreshCache();
				textRefProv.Text=Referrals.GetNameLF(referral.ReferralNum);
			}
		}

		private void FillAttachments(){
			listAttachments.Items.Clear();
			for(int i=0;i<_claim.Attachments.Count;i++){
				listAttachments.Items.Add(_claim.Attachments[i].DisplayedFileName);
			}
		}

		private void butAttachAdd_Click(object sender,EventArgs e) {
			using FormImageSelectClaimAttach formImageSelectClaimAttach=new FormImageSelectClaimAttach();
			formImageSelectClaimAttach.PatNum=_claim.PatNum;
			formImageSelectClaimAttach.ShowDialog();
			if(formImageSelectClaimAttach.DialogResult!=DialogResult.OK){
				return;
			}
			_claim.Attachments.Add(formImageSelectClaimAttach.ClaimAttachNew);
			FillAttachments();
			if(textRadiographs.IsValid()) {
				int radiographs=PIn.Int(textRadiographs.Text);
				radiographs++;
				textRadiographs.Text=radiographs.ToString();
			}
		}

		private void butAttachPerio_Click(object sender,EventArgs e) {
			//Patient PatCur=Patients.GetPat(PatNum);
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				MsgBox.Show(this,"Error. Not using AtoZ images folder.");
				return;
			}
			ContrPerio contrPerioGrid=new ContrPerio();
			contrPerioGrid.BackColor = System.Drawing.SystemColors.Window;
			contrPerioGrid.Size = new System.Drawing.Size(602,665);
			PerioExams.Refresh(_patient.PatNum);
			PerioMeasures.Refresh(_patient.PatNum,PerioExams.ListExams);
			contrPerioGrid.SelectedExam=PerioExams.ListExams.Count-1;
			contrPerioGrid.LoadData();
			using Bitmap bitmap=new Bitmap(602,665);
			Graphics g=Graphics.FromImage(bitmap);
			contrPerioGrid.DrawChart(g);
			g.Dispose();
			using Bitmap bitmapBig=new Bitmap(602,715);
			g=Graphics.FromImage(bitmapBig);
			g.Clear(Color.White);
			string text=_patient.GetNameFL();
			Font font=new Font("Microsoft Sans Serif",12,FontStyle.Bold);
			g.DrawString(text,font,Brushes.Black,602/2-g.MeasureString(text,font).Width/2,5);
			text=PrefC.GetString(PrefName.PracticeTitle);
			font=new Font("Microsoft Sans Serif",9,FontStyle.Bold);
			g.DrawString(text,font,Brushes.Black,602/2-g.MeasureString(text,font).Width/2,28);
			g.DrawImage(bitmap,0,50);
			g.Dispose();
			Random rnd=new Random();
			string newName=DateTime.Now.ToString("yyyyMMdd")+"_"+DateTime.Now.TimeOfDay.Ticks.ToString()+rnd.Next(1000).ToString()+".jpg";
			string attachPath=EmailAttaches.GetAttachPath();
			string newPath=ODFileUtils.CombinePaths(attachPath,newName);
			if(CloudStorage.IsCloudStorage) {
				using FormProgress formProgress=new FormProgress();
				formProgress.DisplayText="Uploading...";
				formProgress.NumberFormat="F";
				formProgress.NumberMultiplication=1;
				formProgress.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
				formProgress.TickMS=1000;
				OpenDentalCloud.Core.TaskStateUpload taskStateUpload=null;
				using MemoryStream memoryStream=new MemoryStream();
				try {
					bitmapBig.Save(memoryStream,System.Drawing.Imaging.ImageFormat.Bmp);
					taskStateUpload=CloudStorage.UploadAsync(
						CloudStorage.AtoZPath+"/EmailAttachments"
						,newName
						,memoryStream.ToArray()
						,new OpenDentalCloud.ProgressHandler(formProgress.UpdateProgress));
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
				formProgress.ShowDialog();
				if(formProgress.DialogResult==DialogResult.Cancel) {
					taskStateUpload.DoCancel=true;
					return;
				}
				//Upload was successful, so continue attaching
			}
			else {
				try{
					bitmapBig.Save(newPath);
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
			}
			ClaimAttach claimAttach=new ClaimAttach();
			claimAttach.DisplayedFileName=Lan.g(this,"PerioChart.jpg");
			claimAttach.ActualFileName=newName;
			_claim.Attachments.Add(claimAttach);
			FillAttachments();
		}

		private void butExport_Click(object sender,EventArgs e) {
			if(ODBuild.IsWeb()) {
				for(int i=0;i<_claim.Attachments.Count;i++) {
					string fileName=_patient.FName+_patient.LName+_patient.PatNum+"_"+i+Path.GetExtension(_claim.Attachments[i].ActualFileName);
					string tempPath=ODFileUtils.CombinePaths(Path.GetTempPath(),fileName);
					string currentPath=FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),_claim.Attachments[i].ActualFileName);
					FileAtoZ.Copy(currentPath,tempPath,FileAtoZSourceDestination.AtoZToLocal,"Exporting attachment...");
					ThinfinityUtils.ExportForDownload(tempPath);
				}
				return;
			}
			string claimAttachExportPath=PrefC.GetString(PrefName.ClaimAttachExportPath);
			if(!Directory.Exists(claimAttachExportPath)){
				if(MessageBox.Show(Lan.g(this,"The claim export path no longer exists at:")+" "+claimAttachExportPath+"\r\n"
					+Lan.g(this,"Would you like to create it?"),"", MessageBoxButtons.YesNo)==DialogResult.Yes) 
				{
					try {
						Directory.CreateDirectory(claimAttachExportPath);
					}
					catch {	//May throw an exception for a variety of reasons.
						MessageBox.Show(Lan.g(this,"The directory was unable to be created.  Try running as Administrator."));
						return;
					}
				}
				else {
					return;
				}
			}
			for(int i=0;i<_claim.Attachments.Count;i++){
				string curAttachPath=FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),_claim.Attachments[i].ActualFileName);
				string newFilePath=ODFileUtils.CombinePaths(claimAttachExportPath,
					_patient.FName+_patient.LName+_patient.PatNum+"_"+i+Path.GetExtension(_claim.Attachments[i].ActualFileName));
				if(!FileAtoZ.Exists(curAttachPath)) {
					MessageBox.Show(Lan.g(this,"The attachment file")+" "+curAttachPath+" "+Lan.g(this,"has been moved, deleted or is inaccessible."));
					return;
				}
				try {
					FileAtoZ.Copy(curAttachPath,newFilePath,FileAtoZSourceDestination.AtoZToLocal,"Downloading file...");
				}
				catch {
					MessageBox.Show(Lan.g(this,"The attachment")+" "+curAttachPath+" "
						+Lan.g(this,"could not be copied to the export folder, probably because of an incorrect file permission. Aborting export operation."));
					return;
				}				
			}
			MsgBox.Show(this,"Done");
		}

		private void contextMenuAttachments_Popup(object sender,EventArgs e) {
			if(listAttachments.SelectedIndex==-1) {
				menuItemOpen.Enabled=false;
				menuItemRename.Enabled=false;
				menuItemRemove.Enabled=false;
			}
			else {
				menuItemOpen.Enabled=true;
				menuItemRename.Enabled=true;
				menuItemRemove.Enabled=true;
			}
		}

		private void menuItemOpen_Click(object sender,EventArgs e) {
			FileAtoZ.OpenFile(FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),_claim.Attachments[listAttachments.SelectedIndex].ActualFileName),
				_claim.Attachments[listAttachments.SelectedIndex].DisplayedFileName);
		}

		private void menuItemRename_Click(object sender,EventArgs e) {
			using InputBox inputBox=new InputBox(Lan.g(this,"Filename"));
			inputBox.textResult.Text=_claim.Attachments[listAttachments.SelectedIndex].DisplayedFileName;
			inputBox.ShowDialog();
			if(inputBox.DialogResult!=DialogResult.OK) {
				return;
			}
			_claim.Attachments[listAttachments.SelectedIndex].DisplayedFileName=inputBox.textResult.Text;
			FillAttachments();
		}

		private void menuItemRemove_Click(object sender,EventArgs e) {
			_claim.Attachments.RemoveAt(listAttachments.SelectedIndex);
			FillAttachments();
		}

		private void listAttachments_DoubleClick(object sender,EventArgs e) {
			if(listAttachments.SelectedIndex==-1) {
				return;
			}
			FileAtoZ.OpenFile(FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),_claim.Attachments[listAttachments.SelectedIndex].ActualFileName),
				_claim.Attachments[listAttachments.SelectedIndex].DisplayedFileName);
		}		

		private void listAttachments_MouseDown(object sender,MouseEventArgs e) {
			//A right click also needs to select an items so that the context menu will work properly.
			if(e.Button==MouseButtons.Right) {
				int clickedIndex=listAttachments.IndexFromPoint(e.Location);
				if(clickedIndex!=-1) {
					listAttachments.SelectedIndex=clickedIndex;
				}
			}
		}

		private void butMissingTeethHelp_Click(object sender,EventArgs e) {
			MessageBox.Show("As explained in the manual, extracted teeth are pulled from the procedure history.  Any extraction with a status of Complete, Existing Current, or Existing Other will be included.  But the extraction must also have a valid date.  So to add an extracted tooth to this list, go to the Chart module, and add an extraction with a status of EO and a date that is as accurate as possible.  Furthermore, extracted teeth will only show here if at least one of the fields for initial placement upper or lower is marked Yes.\r\n\r\nMissing teeth are not pulled from procedure history, but from the missing teeth tab of the Chart module.  Teeth can be marked missing without having an extraction date.");
		}

		private void butLabel_Click(object sender, System.EventArgs e) {
			//LabelSingle label=new LabelSingle();
			//PrintDocument pd=new PrintDocument();//only used to pass printerName
			//if(!PrinterL.SetPrinter(pd,PrintSituation.LabelSingle)){
			//  return;
			//}
			//ask if print secondary?
			InsPlan insPlanCur=InsPlans.GetPlan(_claim.PlanNum,_listInsPlans);
			Carrier carrierCur=Carriers.GetCarrier(insPlanCur.CarrierNum);
			LabelSingle.PrintCarrier(carrierCur.CarrierNum);//pd.PrinterSettings.PrinterName);
		}

		private void butPreview_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.ClaimSend)) {
				return;
			}
			if(!ClaimIsValid()) {
				return;
			}			
			SecurityLogs.MakeLogEntry(PermissionClaimEdit,_patient.PatNum,"Claim Previewed for "+_patient.LName+","+_patient.FName,
				_claim.ClaimNum,_claim.SecDateTEdit);
			UpdateClaim();
			using FormClaimPrint formClaimPrint=new FormClaimPrint();
			formClaimPrint.PatNum=_claim.PatNum;
			formClaimPrint.ClaimNum=_claim.ClaimNum;
			formClaimPrint.DoPrintImmediately=false;
			formClaimPrint.ShowDialog();
			if(formClaimPrint.DialogResult==DialogResult.OK) {
				//status will have changed to sent.
				_claim=Claims.GetClaim(_claim.ClaimNum);
			}
			else if(formClaimPrint.DialogResult==DialogResult.Abort) {//if claim has been deleted, close out of current form.
				DialogResult=DialogResult.Cancel;
				return;
			}
			_listClaimProcs=ClaimProcs.Refresh(_patient.PatNum);
			FillForm();
			//no need to FillCanadian.  Nothing has changed.
		}

		private void ButPrint_Click(object sender,System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.ClaimSend)) {
				return;
			}
			if(!ClaimIsValid()){
				return;
			}
			if(textDateSent.Text=="") {
				textDateSent.Text=DateTime.Today.ToShortDateString();
			}
			DateTime dateTimeClaimCurSectDateTEdit=_claim.SecDateTEdit;//Preserve the date prior to any claim updates effecting it.
			UpdateClaim();
			FormClaimPrint formClaimPrint=new FormClaimPrint();
			formClaimPrint.PatNum=_claim.PatNum;
			formClaimPrint.ClaimNum=_claim.ClaimNum;
			if(!formClaimPrint.PrintImmediate(Lan.g(this,"Claim from")+" "+_claim.DateService.ToShortDateString()+" "+Lan.g(this,"printed"),
				PrintSituation.Claim,_patient.PatNum))
			{
				return;
			}
			if(!_isNotAuthorized) {//if already sent, we want to block users from changing sent date without permission.
				//also changes claimstatus to sent, and date:
				Etranss.SetClaimSentOrPrinted(_claim.ClaimNum,_claim.PatNum,0,EtransType.ClaimPrinted,0,Security.CurUser.UserNum);
			}
			//ClaimCur.ClaimStatus="S";
			//ClaimCur.DateSent=DateTime.Today;
			//Claims.Update(ClaimCur);
			SecurityLogs.MakeLogEntry(Permissions.ClaimSend,_claim.PatNum,Lan.g(this,"Claim printed from Claim Edit window."),
				_claim.ClaimNum,dateTimeClaimCurSectDateTEdit);
			DialogResult=DialogResult.OK;
		}

		private void butSend_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.ClaimSend)) {
				return;
			}
			//Check provider terms here instead of SendClaim or ClaimIsValid as we only want to check 
			//provider term dates when sending claims for the first time.
			if(!CheckProviderTerm()) {
				return;
			}
			SendClaim();
		}

		///<summary>Checks the providers' term dates to see if a claim should be sent.
		///True represents valid providers.  Otherwise; false.</summary>
		private bool CheckProviderTerm() {
			List<long> listLongInvalidProvNums=Providers.GetInvalidProvsByTermDate(new List<long> 
				{ comboProvBill.GetSelectedProvNum(),_provNumOrdering,comboProvTreat.GetSelectedProvNum() },PIn.DateT(textDateService.Text));
			if(listLongInvalidProvNums.Count==0) {
				return true;
			}
			string message="";
			if(listLongInvalidProvNums.Contains(comboProvBill.GetSelectedProvNum()) 
				&& listLongInvalidProvNums.Contains(comboProvTreat.GetSelectedProvNum())
				&& listLongInvalidProvNums.Contains(_provNumOrdering)) 
			{//Used for grammar
				message+="Billing Provider, Treating Provider, and Ordering Provider Override";
			}
			else {
				if(listLongInvalidProvNums.Contains(comboProvBill.GetSelectedProvNum())) {
					message+="Billing Provider";
				}
				if(listLongInvalidProvNums.Contains(comboProvTreat.GetSelectedProvNum())) {
					if(message!="") {
						message+=" and ";
					}
					message+="Treating Provider";
				}
				if(listLongInvalidProvNums.Contains(_provNumOrdering)) {
					if(message!="") {
						message+=" and ";
					}
					message+="Ordering Provider Override";
				}
			}
			if(message.Contains("and")) {//Used for grammar
				message="The "+message+" attached to this claim have Term Dates past the Date of Service for this claim.";
			}
			else {
				message="The "+message+" attached to this claim has a Term Date past the Date of Service for this claim.";
			}
			MsgBox.Show(this,message);
			return false;
		}

		private void SendClaim() {
			if(!ClaimIsValid()) {
				return;
			}
			if(_claim.ClaimStatus==ClaimStatus.HoldForInProcess.GetDescription(useShortVersionIfAvailable:true)) {
				MsgBox.Show(this,"Cannot send a claim with a status of Hold for In Process.");
				return;
			}
			//Do not allow Canadian users to send secondary predeterminations because there is no format that supports such a transaction in ITRANS.		
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && _claim.ClaimType=="PreAuth") {
				for(int i=0;i<_listPatPlans.Count;i++) {
					if(_listPatPlans[i].InsSubNum==_claim.InsSubNum && _listPatPlans[i].Ordinal!=1) {
						MsgBox.Show(this,"Preauthorizations for secondary insurance cannot be sent because there is no supported format.");
						return;
					}
				}
			}
			if(HasIcd9Codes()) {
				return;
			}
			if(comboCorrectionType.SelectedIndex!=0) { //not original (replacement or void)
				textDateSent.Text=DateTime.Now.ToShortDateString();
				labelDateSent.Text="Date Resent";
			}
			DateTime dateTimeClaimCurSectEdit=_claim.SecDateTEdit;//Preserve the date prior to any claim updates effecting it.
			UpdateClaim();
			ClaimSendQueueItem[] claimSendQueueItemsArrayCA=Claims.GetQueueList(_claim.ClaimNum,_claim.ClinicNum,0);
			if(claimSendQueueItemsArrayCA.IsNullOrEmpty()) {//Happens if the Claim has been deleted.
				MsgBox.Show(this, "Unable to find claim data.  Exit claim window and try again.");
				return;
			}
			if(claimSendQueueItemsArrayCA[0].NoSendElect==NoSendElectType.NoSendElect) {
				MsgBox.Show(this,"This carrier is marked to not receive e-claims.");
				//Later: we need to let user send anyway, using all 0's for electronic id.
				return;
			}
			if(claimSendQueueItemsArrayCA[0].NoSendElect==NoSendElectType.NoSendSecondaryElect && claimSendQueueItemsArrayCA[0].Ordinal!=1) {
				MsgBox.Show(this,"This carrier is marked to only receive primary insurance e-claims.");
				return;
			}
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=_claim.ClinicNum;
			}
			Clearinghouse clearinghouseHq=ClearinghouseL.GetClearinghouseHq(claimSendQueueItemsArrayCA[0].ClearinghouseNum);
			Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,clinicNum);
			if(ODBuild.IsWeb() && Clearinghouses.IsDisabledForWeb(clearinghouseClin)) {
				MessageBox.Show(Lans.g("Eclaims","This clearinghouse is not available while viewing through the web."));
				return;
			}
			//string warnings;
			//string missingData=
			claimSendQueueItemsArrayCA[0]=Eclaims.GetMissingData(clearinghouseClin,claimSendQueueItemsArrayCA[0]);
			if(claimSendQueueItemsArrayCA[0].MissingData!=""){
				MessageBox.Show(Lan.g(this,"Cannot send claim until missing/invalid data is fixed:")+"\r\n"+claimSendQueueItemsArrayCA[0].MissingData);
				return;
			}
			else if(clearinghouseClin.IsAttachmentSendAllowed && clearinghouseClin.CommBridge==EclaimsCommBridge.ClaimConnect) {
				//No general missing data, but can send attachments electronically.  Check the clearinghouse for missing data.
				ClaimConnect.ValidateClaimResponse validateClaimResponse=null;
				try {
					validateClaimResponse=ClaimConnect.ValidateClaim(_claim,true);
				}
				catch(ODException ex) {
					//ODExceptions should have already been translated to reduce the number of times a message needs translating
					MessageBox.Show(ex.Message);
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to continue sending the claim?")) {
						return;
					}
				}
				catch(Exception ex) {
					FriendlyException.Show(ex.Message,ex);
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to continue sending the claim?")) {
						return;
					}
				}
				if(validateClaimResponse!=null //the catches above don't alway return
					&& validateClaimResponse.IsAttachmentRequired) 
				{
					if(MsgBox.Show(this,MsgBoxButtons.YesNo,"An attachment is required for this claim. Would you like to open the claim attachment form?")) {
						FormClaimAttachment.Open(_claim);
						DialogResult=DialogResult.OK;
						return;
					}
					else if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to continue sending the claim?")) {
						return;
					}
				}
			}
			Cursor=Cursors.WaitCursor;			
			if(clearinghouseHq.Eformat==ElectronicClaimFormat.Canadian) {
				if(!SendClaimCanadian(clearinghouseClin,claimSendQueueItemsArrayCA[0])) {
					return;
				}
			}
			else {
				List<ClaimSendQueueItem> listClaimSendQueueItems=new List<ClaimSendQueueItem>();
				listClaimSendQueueItems.Add(claimSendQueueItemsArrayCA[0]);
				EnumClaimMedType enumClaimMedType=claimSendQueueItemsArrayCA[0].MedType;
				using FormClaimFormItemEdit formClaimFormItemEdit=new FormClaimFormItemEdit();
				Eclaims.SendBatch(clearinghouseClin,listClaimSendQueueItems,enumClaimMedType,formClaimFormItemEdit,
					FormClaimPrint.FillRenaissance,new FormTerminalConnection());//this also calls SetClaimSentOrPrinted which creates the etrans entry.
			}
			Cursor=Cursors.Default;
			SecurityLogs.MakeLogEntry(Permissions.ClaimSend,_patient.PatNum,Lan.g(this,"Claim sent from Claim Edit Window."),_claim.ClaimNum,dateTimeClaimCurSectEdit);
			DialogResult=DialogResult.OK;
		}

		///<summary>Helper method that only allows one Canadian claim to be sent at a time. Returns true if the claim was sent, otherwise false.</summary>
		private bool SendClaimCanadian(Clearinghouse clearinghouseClin,ClaimSendQueueItem claimSendQueueItem) {
			//Claim sending logic will synchronously wait up to 2 minutes for a response from Canadian clearinghouses.
			//However, Application.DoEvents() gets invoked every 2/10ths of a second while the synchronous method is waiting in order to not let Open Dental go 'Not Responding'.
			//This is a problem because Application.DoEvents() allows the application to process messages queued up in the message pump of the OS (e.g. mouse clicks).
			//If the user clicks on the Send button multiple times while waiting for their claim to send, a 'recursive-esq' call stack will be introduced.
			//Display a warning message to Canadian users to prepare them for the potential 2 minute wait and as an attempt to prevent this 'recursive-esq' call stack.
			//E.g. butSend_Click() => SendClaim() => Canadian.SendClaim() => Application.DoEvents() => butSend_Click() => SendClaim() => Canadian.SendClaim() => Application.DoEvents()...
			//The key point in the example above is that the first invocation of Canadian.SendClaim() has not finished yet and has circled back around due to the Application.DoEvents.
			if(_isCanadianClaimSending) {
				return false;
			}
			_isCanadianClaimSending=true;
			if(!string.IsNullOrEmpty(claimSendQueueItem.Warnings)) {
				System.Text.StringBuilder sb=new System.Text.StringBuilder();
				sb.AppendLine("Warning:");
				claimSendQueueItem.Warnings.Split(",",StringSplitOptions.RemoveEmptyEntries).ForEach(x=>sb.AppendLine(x));
				sb.AppendLine("Click 'OK' to continue sending this claim.");
				if(MessageBox.Show(this,sb.ToString(),"",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
					return false;
				}
			}
			try {
				_canadianSecondaryClaimNum=Canadian.SendClaim(clearinghouseClin,claimSendQueueItem,true,false,FormCCDPrint.PrintCCD,ShowProviderTransferWindow,
					FormClaimPrint.PrintCdaClaimForm);//Ignore the etransNum result. Physically print the form.
			}
			catch(ApplicationException ex){
				//Custom error messages are thrown as ApplicationExceptions in SendClaim().
				//There are probably no other scenarios where an ApplicationException thrown.  If there are, then the user will still get the message, but not the details.  Not a big deal.
				Cursor=Cursors.Default;
				//The message is translated before thrown, so we do not need to translate here.
				//We show the message in a copy/paste window so that our techs and users can quickly copy the message and search for a solution.
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(ex.Message);
				msgBoxCopyPaste.ShowDialog();
				return false;
			}
			catch(Exception ex) {
				Cursor=Cursors.Default;
				//The message is translated before thrown, so we do not need to translate here.
				//We show the message in a copy/paste window so that our techs and users can quickly copy the message and search for a solution.
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(ex.ToString());
				msgBoxCopyPaste.ShowDialog();
				return false;
			}
			finally {
				_isCanadianClaimSending=false;
			}
			return true;
		}

		///<summary>Returns true if the claim has ICD9 codes and the user insists on sending the claim with them attached.</summary>
		private bool HasIcd9Codes() {
			List<Procedure> listProceduresOnClaim=_listProcedures.FindAll(x => _listClaimProcsForClaim.Any(y => y.ProcNum==x.ProcNum));			
			if(ICD9s.HasICD9Codes(listProceduresOnClaim)) {
				string msgText="There are ICD-9 codes attached to a procedure.  Would you like to send the claim without the ICD-9 codes? ";
				if(MessageBox.Show(msgText,"",MessageBoxButtons.YesNo)==DialogResult.Yes) {
					return false;//They have codes, but they are willing to send without them.
				}
				return true;
			}
			return false;
		}

		private void butResend_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.ClaimSend)) {
				return;
			}
			using FormClaimResend formClaimResend=new FormClaimResend();
			if(formClaimResend.ShowDialog()==DialogResult.OK) {
				if(formClaimResend.IsClaimReplacement()) {
					comboCorrectionType.SelectedIndex=1;//replacement
				}
				else {
					comboCorrectionType.SelectedIndex=0;//original
					textDateSent.Text=DateTime.Now.ToShortDateString();
					_claim.DateResent=DateTime.Now;
					labelDateSent.Text="Date Resent";
				}
				SendClaim();
			}
		}

		private void butHistory_Click(object sender,EventArgs e) {
			List<Etrans> listETrans=Etranss.GetHistoryOneClaim(_claim.ClaimNum);
			if(listETrans.Count==0) {
				MsgBox.Show(this,"No history found of sent e-claim.");
				return;
			}
			using FormEtransEdit formEtransEdit=new FormEtransEdit();
			formEtransEdit.EtransCur=listETrans[0];
			formEtransEdit.ShowDialog();
			_claim=Claims.GetClaim(_claim.ClaimNum);
			_listClaimProcs=ClaimProcs.Refresh(_patient.PatNum);
			FillForm();
		}

		///<summary>Canada only.</summary>
		private void butReverse_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			InsPlan insPlan=InsPlans.GetPlan(_claim.PlanNum,null);
			InsSub insSub=InsSubs.GetOne(_claim.InsSubNum);
			Carrier carrier=Carriers.GetCarrier(insPlan.CarrierNum);
			Clearinghouse clearinghouseHq=Canadian.GetCanadianClearinghouseHq(carrier);
			Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
			//See SendClaimCanadian() for explanation of boolean.
			if(_isCanadianClaimSending) {
				return;
			}
			_isCanadianClaimSending=true;
			long etransNumAck=0;
			try {
				etransNumAck=CanadianOutput.SendClaimReversal(clearinghouseClin,_claim,insPlan,insSub,false,FormCCDPrint.PrintCCD);
			}
			catch(Exception ex) {
				// there is a SubString() error coming most likley from CanadianOutput.SendClaimReversal, this will help us track down exactly where.
				FriendlyException.Show(Lan.g(this,"Failed to reverse claim"),ex);
			}
			finally {
				_isCanadianClaimSending=false;
			}
			Etrans etransAck=Etranss.GetEtrans(etransNumAck);
			if(etransNumAck!=0){//the catch does not return
				if(etransAck.AckCode!="R") {
					//If the claim was successfully reversed, clear the claim transaction reference number so the user can resend the claim if desired.
					//Will make the claim look like it was not ever sent, except in send claims window there will be extra history.
					_claim.CanadaTransRefNum="";
					Claims.Update(_claim);
				}
			}
			Cursor=Cursors.Default;
		}

		private void butAdd_Click(object sender,EventArgs e) {
			AddClaimCustomTracking(_claim);
			FillStatusHistory();
		}

		///<summary>Opens FormClaimAttachment. This form is used for managing attachments to send to DentalXChange.</summary>
		private void buttonClaimAttachment_Click(object sender,EventArgs e) {
			if(IsNew) {//Must have a claim object to do claim attachments
				MsgBox.Show(this,"The claim must be saved before you can start adding attachments.");
				return;
			}
			if(comboClaimStatus.SelectedIndex==_listClaimStatuses.IndexOf(ClaimStatus.WaitingToSend)) {//Waiting to send claim
				//Check to see if Claim is Valid
				if(!ClaimIsValid()) {
					return;
				}
				//Determine if there is any missing data
				ClaimEdit.UpdateData updateData=UpdateClaim();
				ClaimSendQueueItem[] claimSendQueueItemsArray=updateData.ListSendQueueItems;
				//There is no benefit to the user by attaching and sending files to DXC if the carrier cannot send an electronic claim within Open Dental.
				//If it is determined later that there is a desire to allow these carriers in, we can simply remove this block.
				if(!claimSendQueueItemsArray[0].CanSendElect) {
					MsgBox.Show(this,"Carrier is not set to Send Claims Electronically.");
					return;
				}
				Clearinghouse clearinghouseHq=ClearinghouseL.GetClearinghouseHq(claimSendQueueItemsArray[0].ClearinghouseNum);
				Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
				claimSendQueueItemsArray[0]=Eclaims.GetMissingData(clearinghouseClin,claimSendQueueItemsArray[0]);
				if(claimSendQueueItemsArray[0].MissingData!="") {
					MessageBox.Show("Cannot add attachments until missing data is fixed:"+"\r\n"+claimSendQueueItemsArray[0].MissingData);
					return;
				}
			}
			if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will close the claim edit window without saving any changes. Continue?")) {
				FormClaimAttachment.Open(_claim);
				DialogResult=DialogResult.Cancel;
			}
		}

		///<summary>Returns true if user inserted a new ClaimTracking and selected an Error Code.</summary>
		public static bool AddClaimCustomTracking(Claim claim,string noteText="") {
			using FormClaimCustomTrackingUpdate formClaimCustomTrackingUpdate=new FormClaimCustomTrackingUpdate(claim,noteText);
			formClaimCustomTrackingUpdate.ShowDialog();
			if(formClaimCustomTrackingUpdate.DialogResult!=DialogResult.OK) {
				return false;
			}
			ClaimTracking claimTracking=formClaimCustomTrackingUpdate.ListClaimTrackingsNew.FirstOrDefault();
			if(claimTracking==null){
				//No valid TrackingErrorDefNum was found, no selection/insert can be made
				return false; 
			}
			long trackingErrorDefNum=claimTracking.TrackingErrorDefNum;//A valid TrackingErrorDefNum was found and can be selected/inserted
			if(trackingErrorDefNum==0) {
				return false;
			}
			return true;
		}

		private void gridStatusHistory_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			ClaimTracking claimTracking=(ClaimTracking)gridStatusHistory.ListGridRows[e.Row].Tag;
			if(Security.IsAuthorized(Permissions.ClaimHistoryEdit,claimTracking.DateTimeEntry)){
				using FormClaimCustomTrackingUpdate formClaimCustomTrackingUpdate=new FormClaimCustomTrackingUpdate(_claim,claimTracking);
				formClaimCustomTrackingUpdate.ShowDialog();
				if(formClaimCustomTrackingUpdate.DialogResult==DialogResult.OK) {
					FillStatusHistory();//Refresh grid
				}
			}
		}
		
		/// <summary>Also handles Canadian warnings.</summary>
		private bool ClaimIsValid() {
			if(	 !textDateService.IsValid()
				|| !textDateSent.IsValid()
				|| !textDateSentOrig.IsValid()
				|| !textDateRec.IsValid()
				|| !textPriorDate.IsValid()
				|| !textDedApplied.IsValid()
				|| !textInsPayAmt.IsValid()
				|| !textOrthoDate.IsValid()
				|| !textRadiographs.IsValid()
				|| !textAccidentDate.IsValid()
				|| !textAttachImages.IsValid()
				|| !textAttachModels.IsValid()
				|| !textDateInitialUpper.IsValid()
				|| !textDateInitialLower.IsValid()
				|| !textShareOfCost.IsValid()
				|| !textOrthoRemainM.IsValid()
				|| !textOrthoTotalM.IsValid()
				|| !textDateIllness.IsValid()
				|| !textDateOther.IsValid()
				)
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			//We are not sure how these text boxes can have invalid values, but we have received many bug submissions.
			try {
				PIn.Byte(textOrthoTotalM.Text);
			}
			catch(Exception ex) {
				ex.DoNothing();
				MsgBox.Show(this,"Please enter a valid value for Ortho Months Total.");
				return false;
			}
			try {
				PIn.Byte(textOrthoRemainM.Text);
			}
			catch(Exception ex) {
				ex.DoNothing();
				MsgBox.Show(this,"Please enter a valid value for Ortho Months Remaining.");
				return false;
			}
			if(comboClaimStatus.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a valid Claim Status.");
				return false;
			}
			bool isSentOrReceived= comboClaimStatus.SelectedIndex.In(_listClaimStatuses.IndexOf(ClaimStatus.Sent),_listClaimStatuses.IndexOf(ClaimStatus.Received));
			ClaimIsValidState claimIsValidState=ClaimL.ClaimIsValid(textDateService.Text,_claim.ClaimType,isSentOrReceived,textDateSent.Text,_listClaimProcsForClaim
				,_claim.PlanNum,_listInsPlans,textNote.Text,_claim.UniformBillType,(ClaimCorrectionType)comboCorrectionType.SelectedIndex
			);
			switch(claimIsValidState) {
				case ClaimIsValidState.False:
					return false;
				case ClaimIsValidState.FalseClaimProcsChanged:
					FillGrids();//Fill grid because of status change.
					return false;
				case ClaimIsValidState.True:
					if(!CanadianWarnings()) {
						//This doesn't exist in ClaimL.ClaimIsValid(...) because the logic in that method was moved there for ERA customers in US.
						return false;
					}
					return true;
			}
			return false;//Just in case
		}

		///<summary>Only called from one location above.  In its own method for readability.</summary>
		private bool CanadianWarnings(){
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//if not Canadian
				return true;//skip this entire method
			}
			if(!textCanadianAccidentDate.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			if(textCanadianAccidentDate.Text!="" && DateTime.Parse(textCanadianAccidentDate.Text).Date>DateTime.Today) {
				MsgBox.Show(this,"Accident date cannot be a date in the future.");
				return false;
			}
			string warning="";
			if(textReferralProvider.Text!="" && comboReferralReason.SelectedIndex==0){
				if(warning!=""){
					warning+="\r\n";
				}
				warning+="Referral reason is required if provider indicated.";
			}
			if(textReferralProvider.Text=="" && comboReferralReason.SelectedIndex!=0){
				if(warning!=""){
					warning+="\r\n";
				}
				warning+="Referring provider required if referring reason is indicated.";
			}
			//Max prosth----------------------------------------------------------------------------------
			if(comboMaxProsth.SelectedIndex==0){
				if(warning!=""){
					warning+="\r\n";
				}
				warning+="Max prosth not indicated.";
			}
			if(textDateInitialUpper.Text!="") {
				if(PIn.Date(textDateInitialUpper.Text)>DateTime.Today) {
					if(warning!="") {
						warning+="\r\n";
					}
					warning+="Initial max date must be in the past.";
				}
				if(PIn.Date(textDateInitialUpper.Text).Year<1900) {
					if(warning!="") {
						warning+="\r\n";
					}
					warning+="Initial max date is not reasonable.";
				}
			}
			if(comboMaxProsth.SelectedIndex==2){//no
				if(textDateInitialUpper.Text==""){
					if(warning!=""){
						warning+="\r\n";
					}
					warning+="Max initial date is required if 'no' is selected.";
				}
				if(comboMaxProsthMaterial.SelectedIndex==0){
					if(warning!=""){
						warning+="\r\n";
					}
					warning+="Max prosth material must be indicated";
				}
			}
			if(comboMaxProsthMaterial.SelectedIndex!=0 && comboMaxProsth.SelectedIndex==3){//not an upper prosth
				if(warning!="") {
					warning+="\r\n";
				}
				warning+="Max prosth should not have a material selected.";
			}
			//Mand prosth----------------------------------------------------------------------------------------------------------
			if(comboMandProsth.SelectedIndex==0){
				if(warning!=""){
					warning+="\r\n";
				}
				warning+="Mand prosth not indicated.";
			}
			if(textDateInitialLower.Text!=""){
				if(PIn.Date(textDateInitialLower.Text)>DateTime.Today){
					if(warning!=""){
						warning+="\r\n";
					}
					warning+="Initial mand date must be in the past.";
				}
				if(PIn.Date(textDateInitialLower.Text).Year<1900){
					if(warning!=""){
						warning+="\r\n";
					}
					warning+="Initial mand date is not reasonable.";
				}
			}
			if(comboMandProsth.SelectedIndex==2) {//no
				if(textDateInitialLower.Text==""){
					if(warning!=""){
						warning+="\r\n";
					}
					warning+="Mand initial date is required if 'no' is checked.";
				}
				if(comboMandProsthMaterial.SelectedIndex==0){
					if(warning!=""){
						warning+="\r\n";
					}
					warning+="Mand prosth material must be indicated";// (unless for a crown).";
				}
			}
			if(comboMandProsthMaterial.SelectedIndex!=0 && comboMandProsth.SelectedIndex==3) {//not a lower prosth
				if(warning!="") {
					warning+="\r\n";
				}
				warning+="Mand prosth should not have a material selected.";
			}
			if(warning!=""){
				DialogResult dialogResult=MessageBox.Show("Warnings:\r\n"+warning+"\r\nDo you wish to continue anyway?","",
					MessageBoxButtons.OKCancel);
				if(dialogResult!=DialogResult.OK){
					return false;
				}
			}
			//Ortho Treatment------------------------------------------------------------------------------------------------------
			if(groupCanadaOrthoPredeterm.Enabled && textDateCanadaEstTreatStartDate.Text!="" && 
				textCanadaInitialPayment.Text!="" && textCanadaExpectedPayCycle.Text!="" &&
				textCanadaTreatDuration.Text!="" && textCanadaNumPaymentsAnticipated.Text!="" && 
				textCanadaAnticipatedPayAmount.Text!="") {
				if(!textDateCanadaEstTreatStartDate.IsValid()) {
					MsgBox.Show(this,"Please fix data entry errors first.");
					return false;
				}
				try {
					Double.Parse(textCanadaInitialPayment.Text);
				}
				catch {
					MsgBox.Show(this,"Invalid initial payment amount.");
					return false;
				}
				byte payCycle=0;
				try {
					payCycle=byte.Parse(textCanadaExpectedPayCycle.Text);
				}
				catch {
					MsgBox.Show(this,"Invalid expected payment cycle.");
					return false;
				}
				if(payCycle<1 || payCycle>4) {
					MsgBox.Show(this,"Expected payment cycle must be a value between 1 and 4.");
					return false;
				}
				try {
					byte.Parse(textCanadaTreatDuration.Text);
				}
				catch {
					MsgBox.Show(this,"Invalid treatment duration.");
					return false;
				}
				try {
					byte.Parse(textCanadaNumPaymentsAnticipated.Text);
				}
				catch {
					MsgBox.Show(this,"Invalid number of payments anticipated.");
					return false;
				}
				try {
					Double.Parse(textCanadaAnticipatedPayAmount.Text);
				}
				catch {
					MsgBox.Show(this,"Invalid anticipated pay amount.");
					return false;
				}
			}
			return true;
		}

		///<summary>Updates this claim to the database.</summary>
		private ClaimEdit.UpdateData UpdateClaim(){
			if(_isNotAuthorized){
				return null;
			}
			//patnum
			_claim.DateService=PIn.Date(textDateService.Text);
			if(textDateSent.Text==""){
				_claim.DateSent=DateTime.MinValue;
			}
			else{
				_claim.DateSent=PIn.Date(textDateSent.Text);
			}
			_claim.ClaimStatus=_listClaimStatuses[comboClaimStatus.SelectedIndex].GetDescription(true);
			bool wasSentOrReceived= _claim.ClaimStatus.In(ClaimStatus.Sent.GetDescription(true),ClaimStatus.Received.GetDescription(true));
			if(wasSentOrReceived && _claim.DateSentOrig.Year<1880) {
				textDateSentOrig.Text=DateTime.Today.ToShortDateString();
				_claim.DateSentOrig=DateTime.Today;
			}
			//claimType can't be changed here.
			_claim.MedType=(EnumClaimMedType)comboMedType.SelectedIndex;
			if(comboClaimForm.SelectedIndex==0){
				_claim.ClaimForm=0;
			}
			else{
				_claim.ClaimForm=_listClaimForms[comboClaimForm.SelectedIndex-1].ClaimFormNum;
			}
			if(textDateRec.Text==""){
				_claim.DateReceived=DateTime.MinValue;
			}
			else{
				_claim.DateReceived=PIn.Date(textDateRec.Text);
			}
			//planNum
			_claim.SpecialProgramCode=(EnumClaimSpecialProgram)comboSpecialProgram.SelectedIndex;
			//patRelats will always be selected
			_claim.PatRelat=(Relat)comboPatRelat.SelectedIndex;
			_claim.PatRelat2=(Relat)comboPatRelat2.SelectedIndex;
			_claim.ProvTreat=comboProvTreat.GetSelectedProvNum();
			_claim.PriorAuthorizationNumber=textPriorAuth.Text;
			_claim.PreAuthString=textPredeterm.Text;
			//isprosthesis handled earlier
			_claim.PriorDate=PIn.Date(textPriorDate.Text);
			_claim.ReasonUnderPaid=textReasonUnder.Text;
			_claim.ClaimNote=textNote.Text;
			//ispreauth
			_claim.ProvBill=comboProvBill.GetSelectedProvNum();
			_claim.IsOrtho=checkIsOrtho.Checked;
			_claim.OrthoTotalM=PIn.Byte(textOrthoTotalM.Text);
			_claim.OrthoRemainM=PIn.Byte(textOrthoRemainM.Text);
			_claim.OrthoDate=PIn.Date(textOrthoDate.Text);
			_claim.RefNumString=textRefNum.Text;
			_claim.PlaceService=(PlaceOfService)comboPlaceService.SelectedIndex;
			_claim.EmployRelated=(YN)comboEmployRelated.SelectedIndex;
			switch(comboAccident.SelectedIndex){
				case 0:
					_claim.AccidentRelated="";
					break;
				case 1:
					_claim.AccidentRelated="A";
					break;
				case 2:
					_claim.AccidentRelated="E";
					break;
				case 3:
					_claim.AccidentRelated="O";
					break;
			}
			//AccidentDate is further down
			_claim.AccidentST=textAccidentST.Text;
			_claim.ClinicNum=comboClinic.SelectedClinicNum;
			_claim.CorrectionType=(ClaimCorrectionType)Enum.GetValues(typeof(ClaimCorrectionType)).GetValue(comboCorrectionType.SelectedIndex);
			_claim.ClaimIdentifier=string.IsNullOrWhiteSpace(textClaimIdentifier.Text) ? Claims.ConvertClaimId(_claim,_patient) : textClaimIdentifier.Text;
			_claim.OrigRefNum=textOrigRefNum.Text;
			_claim.ShareOfCost=PIn.Double(textShareOfCost.Text);
			//attachments
			_claim.Radiographs=PIn.Byte(textRadiographs.Text);
			_claim.AttachedImages=PIn.Int(textAttachImages.Text);
			_claim.AttachedModels=PIn.Int(textAttachModels.Text);
			List<string> listStringFlags=new List<string>();
			#region Not for ClaimConnect, see OK_Click()
			//offices might have ClaimConnect as their default clearinghouse but not make use of DXC. In this case we want to consider information in the NEA tab
			if(!_clearinghouse.IsAttachmentSendAllowed) {
				if(checkAttachEoB.Checked){
					listStringFlags.Add("EoB");
				}
				if(checkAttachNarrative.Checked){
					listStringFlags.Add("Note");
				}
				if(checkAttachPerio.Checked){
					listStringFlags.Add("Perio");
				}
				if(checkAttachMisc.Checked){
					listStringFlags.Add("Misc");
				}
				if(radioAttachMail.Checked){
					listStringFlags.Add("Mail");
				}
				if(radioAttachElect.Checked){
					listStringFlags.Add("Elect");
				}
				if(textAttachID.Text!="") {
					listStringFlags.Add("Misc");
				}
				_claim.AttachedFlags="";
				for(int i=0;i<listStringFlags.Count;i++){
					if(i>0){
						_claim.AttachedFlags+=",";
					}
					_claim.AttachedFlags+=listStringFlags[i];
				}
			}
			#endregion
			//Use the DXC attachmentID if it is not blank, otherwise use the NEA attachmentID. In the case they are both blank it does not matter,
			//but users can have DXC attachments enabled with old NEA numbers attached. This will preserve those.
			_claim.AttachmentID=textAttachmentID.Text!="" ? textAttachmentID.Text : textAttachID.Text;
			//Canadian---------------------------------------------------------------------------------
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				_claim.CanadianMaterialsForwarded="";
				if(checkEmail.Checked) {
					_claim.CanadianMaterialsForwarded+="E";
				}
				if(checkCorrespondence.Checked) {
					_claim.CanadianMaterialsForwarded+="C";
				}
				if(checkModels.Checked) {
					_claim.CanadianMaterialsForwarded+="M";
				}
				if(checkXrays.Checked) {
					_claim.CanadianMaterialsForwarded+="X";
				}
				if(checkImages.Checked) {
					_claim.CanadianMaterialsForwarded+="I";
				}
				_claim.CanadianReferralProviderNum=textReferralProvider.Text;
				_claim.CanadianReferralReason=(byte)comboReferralReason.SelectedIndex;
				_claim.AccidentDate=PIn.Date(textCanadianAccidentDate.Text);
				_claim.IsOrtho=checkCanadianIsOrtho.Checked;
				//max prosth-----------------------------------------------------------------------------------------------------
				switch(comboMaxProsth.SelectedIndex) {
					case 0:
						_claim.CanadianIsInitialUpper="";
						break;
					case 1:
						_claim.CanadianIsInitialUpper="Y";
						break;
					case 2:
						_claim.CanadianIsInitialUpper="N";
						break;
					case 3:
						_claim.CanadianIsInitialUpper="X";
						break;
				}
				_claim.CanadianDateInitialUpper=PIn.Date(textDateInitialUpper.Text);
				_claim.CanadianMaxProsthMaterial=(byte)comboMaxProsthMaterial.SelectedIndex;
				//mand prosth-----------------------------------------------------------------------------------------------------
				switch(comboMandProsth.SelectedIndex) {
					case 0:
						_claim.CanadianIsInitialLower="";
						break;
					case 1:
						_claim.CanadianIsInitialLower="Y";
						break;
					case 2:
						_claim.CanadianIsInitialLower="N";
						break;
					case 3:
						_claim.CanadianIsInitialLower="X";
						break;
				}
				_claim.CanadianDateInitialLower=PIn.Date(textDateInitialLower.Text);
				_claim.CanadianMandProsthMaterial=(byte)comboMandProsthMaterial.SelectedIndex;
				//ortho treatment
				if(groupCanadaOrthoPredeterm.Enabled && textDateCanadaEstTreatStartDate.Text!="" && 
					textCanadaInitialPayment.Text!="" && textCanadaExpectedPayCycle.Text!="" &&
					textCanadaTreatDuration.Text!="" && textCanadaNumPaymentsAnticipated.Text!="" && 
					textCanadaAnticipatedPayAmount.Text!="") {
					_claim.CanadaEstTreatStartDate=DateTime.Parse(textDateCanadaEstTreatStartDate.Text);
					_claim.CanadaInitialPayment=Double.Parse(textCanadaInitialPayment.Text);
					_claim.CanadaPaymentMode=byte.Parse(textCanadaExpectedPayCycle.Text);
					_claim.CanadaTreatDuration=byte.Parse(textCanadaTreatDuration.Text);
					_claim.CanadaNumAnticipatedPayments=byte.Parse(textCanadaNumPaymentsAnticipated.Text);
					_claim.CanadaAnticipatedPayAmount=Double.Parse(textCanadaAnticipatedPayAmount.Text);
				}
				else {
					_claim.CanadaEstTreatStartDate=DateTime.MinValue;
					_claim.CanadaInitialPayment=0;
					_claim.CanadaPaymentMode=0;
					_claim.CanadaTreatDuration=0;
					_claim.CanadaNumAnticipatedPayments=0;
					_claim.CanadaAnticipatedPayAmount=0;
				}
			}//End Canadian-----------------------------------------------------------------------------
			else {
				_claim.AccidentDate=PIn.Date(textAccidentDate.Text);
				_claim.IsOrtho=checkIsOrtho.Checked;
			}
			_claim.UniformBillType=textBillType.Text;
			_claim.AdmissionTypeCode=textAdmissionType.Text;
			_claim.AdmissionSourceCode=textAdmissionSource.Text;
			_claim.PatientStatusCode=textPatientStatus.Text;
			_claim.ProvOrderOverride=_provNumOrdering;
			if(_referralOrdering==null) {
				_claim.OrderingReferralNum=0;
			}
			else {
				_claim.OrderingReferralNum=_referralOrdering.ReferralNum;
			}
			if(_listClaimValCodeLogs!=null) {
				GetValCodes(_listClaimValCodeLogs,0,_claim.ClaimNum,textVC39aCode,textVC39aAmt);
				GetValCodes(_listClaimValCodeLogs,1,_claim.ClaimNum,textVC40aCode,textVC40aAmt);
				GetValCodes(_listClaimValCodeLogs,2,_claim.ClaimNum,textVC41aCode,textVC41aAmt);
				GetValCodes(_listClaimValCodeLogs,3,_claim.ClaimNum,textVC39bCode,textVC39bAmt);
				GetValCodes(_listClaimValCodeLogs,4,_claim.ClaimNum,textVC40bCode,textVC40bAmt);
				GetValCodes(_listClaimValCodeLogs,5,_claim.ClaimNum,textVC41bCode,textVC41bAmt);
				GetValCodes(_listClaimValCodeLogs,6,_claim.ClaimNum,textVC39cCode,textVC39cAmt);
				GetValCodes(_listClaimValCodeLogs,7,_claim.ClaimNum,textVC40cCode,textVC40cAmt);
				GetValCodes(_listClaimValCodeLogs,8,_claim.ClaimNum,textVC41cCode,textVC41cAmt);
				GetValCodes(_listClaimValCodeLogs,9,_claim.ClaimNum,textVC39dCode,textVC39dAmt);
				GetValCodes(_listClaimValCodeLogs,10,_claim.ClaimNum,textVC40dCode,textVC40dAmt);
				GetValCodes(_listClaimValCodeLogs,11,_claim.ClaimNum,textVC41dCode,textVC41dAmt);
			}
			if(_claimCondCodeLog!=null || textCode0.Text!="" || textCode1.Text!="" || textCode2.Text!="" || textCode3.Text!="" || 
				textCode4.Text!="" || textCode5.Text!="" || textCode6.Text!="" || textCode7.Text!="" || textCode8.Text!="" || 
				textCode9.Text!="" || textCode10.Text!="") {
				if(_claimCondCodeLog==null) {
					_claimCondCodeLog=new ClaimCondCodeLog();
					_claimCondCodeLog.ClaimNum=_claim.ClaimNum;
					_claimCondCodeLog.IsNew=true;
				}
				_claimCondCodeLog.Code0=textCode0.Text;
				_claimCondCodeLog.Code1=textCode1.Text;
				_claimCondCodeLog.Code2=textCode2.Text;
				_claimCondCodeLog.Code3=textCode3.Text;
				_claimCondCodeLog.Code4=textCode4.Text;
				_claimCondCodeLog.Code5=textCode5.Text;
				_claimCondCodeLog.Code6=textCode6.Text;
				_claimCondCodeLog.Code7=textCode7.Text;
				_claimCondCodeLog.Code8=textCode8.Text;
				_claimCondCodeLog.Code9=textCode9.Text;
				_claimCondCodeLog.Code10=textCode10.Text;
			}
			_claim.DateIllnessInjuryPreg=PIn.Date(textDateIllness.Text);
			_claim.DateIllnessInjuryPregQualifier=comboDateIllnessQualifier.GetSelected<DateIllnessInjuryPregQualifier>();
			_claim.DateOther=PIn.Date(textDateOther.Text);
			_claim.DateOtherQualifier=comboDateOtherQualifier.GetSelected<DateOtherQualifier>();
			_claim.IsOutsideLab=checkIsOutsideLab.Checked;
			List<Procedure> listProceduresToUpdatePlaceOfService=new List<Procedure>();
			for(int i=0;i<_listClaimProcsForClaim.Count;i++) {
				Procedure procedure=Procedures.GetProcFromList(_listProcedures,_listClaimProcsForClaim[i].ProcNum);
				if(procedure.ProcNum==0) {
					continue;//ignores payments, etc
				}
				if(procedure.PlaceService!=_claim.PlaceService) {
					listProceduresToUpdatePlaceOfService.Add(procedure);
				}
			}
			ClaimEdit.UpdateData updateData=ClaimEdit.UpdateClaim(_claim,_listClaimValCodeLogs,_claimCondCodeLog,listProceduresToUpdatePlaceOfService,_patient,
				wasSentOrReceived,PermissionClaimEdit);
			for(int i=0;i<listProceduresToUpdatePlaceOfService.Count;i++) {
				listProceduresToUpdatePlaceOfService[i].PlaceService=_claim.PlaceService;
			}
			if(_listClaimProcsForClaim.Any(x => x.ClinicNum!=_claim.ClinicNum)) {
				ClaimProcs.UpdateClinicNumForClaim(_claim.ClaimNum,_claim.ClinicNum);
			}
			return updateData;
		}

		private void GetValCodes(List<ClaimValCodeLog> listClaimValCodeLogs,int valCodeIdx,long claimNum,TextBox textBoxVCCode,TextBox textBoxVCAmount) {
			if(valCodeIdx<listClaimValCodeLogs.Count) {//update existing ClaimValCodeLog
				listClaimValCodeLogs[valCodeIdx].ValCode=textBoxVCCode.Text;
				listClaimValCodeLogs[valCodeIdx].ValAmount=PIn.Double(textBoxVCAmount.Text);
			}
			else if(PIn.Double(textBoxVCAmount.Text)>0 || textBoxVCCode.Text!="") {//add a new ClaimValCodeLog
				ClaimValCodeLog claimValCodeLog=new ClaimValCodeLog();
				claimValCodeLog.ValCode=textBoxVCCode.Text;
				claimValCodeLog.ValAmount=PIn.Double(textBoxVCAmount.Text);
				claimValCodeLog.ClaimNum=claimNum;
				listClaimValCodeLogs.Add(claimValCodeLog);
			}
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;//jump straight to Closing, where the claimprocs will be changed
				return;
			}
			if(!ClaimIsValid()){
				return;
			}
			DateTime dateTimeClaimCurSectEdit=_claim.SecDateTEdit;//Preserve the date prior to any claim updates effecting it.
			UpdateClaim();
			bool isPaymentAttached=false;
			bool isClaimInsPayAttached=false;
			//Loops through each of the procedures and checks that there is no payment, or insurance amount attached
			//if there is then break out and cancel deleting the claim.
			for(int i=0;i<_listClaimProcsForClaim.Count;i++){
				if(_listClaimProcsForClaim[i].ClaimPaymentNum>0){
					isPaymentAttached=true;
					break;
				}
				if(_listClaimProcsForClaim[i].InsPayAmt>0){
					isClaimInsPayAttached=true;
					break;
				}
			}
			if(isPaymentAttached){
				MsgBox.Show(this,"You cannot delete this claim while any insurance checks are attached.  You will have to detach all insurance checks first.");
				return;
			}
			if(isClaimInsPayAttached){
				MsgBox.Show(this,"You cannot delete this claim while there are insurance payments.  Set all Insurance Paid amounts to zero first.");
				return;
			}
			if(_claim.ClaimStatus==ClaimStatus.Received.GetDescription(true)){//received
				MessageBox.Show(Lan.g(this,"You cannot delete this claim while status is Received.  You will have to change the status first."));
				return;
			}
			List<long> listETrans835Attaches=Etrans835Attaches.GetForClaimNums(_claim.ClaimNum).Select(x => x.Etrans835AttachNum).ToList();
			if(_claim.ClaimType=="PreAuth"){
				string msgAttaches="";
				if(listETrans835Attaches.Count>0) {
					msgAttaches="This claim is attached to at least one ERA."
					+"\r\nDeleting the claim will unassociate this claim from the ERA."
					+"\r\n";
				}
				if(MessageBox.Show(msgAttaches+Lan.g(this,"Delete PreAuthorization?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
					return;
				}
			}
			else{
				string msgAttaches="";
				if(listETrans835Attaches.Count>0) {
					msgAttaches="This claim is attached to at least one ERA."
					+"\r\nDeleting the claim will unassociate this claim from the ERA"
					+"\r\nand the associated ERA will need to be edited before it can be Finalized."
					+"\r\n";
				}
				if(MessageBox.Show(msgAttaches+Lan.g(this,"Delete Claim?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
					return;
				}
			}
			DeleteClaimHelper(listETrans835Attaches);
			SecurityLogs.MakeLogEntry(Permissions.ClaimDelete,_claim.PatNum,_patient.GetNameLF()
				+", "+Lan.g(this,"Date Entry")+": "+_claim.SecDateEntry.ToShortDateString()
				+", "+Lan.g(this,"Date of Service")+": "+_claim.DateService.ToShortDateString(),
				_claim.ClaimNum,dateTimeClaimCurSectEdit);
			_isDeleting=true;
			DialogResult=DialogResult.OK;
		}

		///<summary>This window is complicated and can do many things. Therefore, we pre-insert the claim and other associated entities.
		///This helper method does all of the work necessary to clean up after all of said entities and deletes the claim as well.
		///If ClaimType is 'PreAuth' or 'Cap', claimprocs will be deleted. Otherwise, all pay-as-total and supplemental claimprocs
		///are deleted, and all other claimprocs are returned to estimate status, their ClaimNum is set to zero, we run 
		///ClaimProcs.ComputeBaseEstimates() for them, and they are updated to the DB. Any claimprocs associated to dropped patplans are deleted. 
		///Canadian lab claimprocs have their status synced with parent claimprocs and are ommitted when running ClaimProcs.ComputeBaseEstimates()
		///as their estimates are calculated when ClaimProcs.ComputeBaseEstimates() is called for thier parent claimprocs.
		///It is okay to default list835Attaches to null if this is called when cancelling a new claim as none should exist.</summary>
		private void DeleteClaimHelper(List<long> list835Attaches=null) {
			if(_claim.ClaimType=="PreAuth"//all preauth claimprocs are just duplicates
				|| _claim.ClaimType=="Cap") //all cap claimprocs are just duplicates
			{
				ClaimProcs.DeleteMany(_listClaimProcsForClaim);
			}
			else {//all other claim types use original estimate claimproc.
				List<Benefit> listBenefits=Benefits.Refresh(_listPatPlans,_listInsSubs);
				InsPlan insPlan=InsPlans.GetPlan(_claim.PlanNum,_listInsPlans);
				for(int i=0;i<_listClaimProcsForClaim.Count;i++) {
					if(_listClaimProcsForClaim[i].Status==ClaimProcStatus.Supplemental//supplementals are duplicate
						|| _listClaimProcsForClaim[i].ProcNum==0)//total payments get deleted
					{
						ClaimProcs.Delete(_listClaimProcsForClaim[i]);
						continue;
					}
					//so only changed back to estimate if attached to a proc
					_listClaimProcsForClaim[i].Status=ClaimProcStatus.Estimate;
					//already handled the case where claimproc.ProcNum=0 for payments etc. above
					Procedure procedure=Procedures.GetProcFromList(_listProcedures,_listClaimProcsForClaim[i].ProcNum);
					List<ClaimProc> listClaimProcsForLab=GetListClaimProcsForProcNumLabsParent(procedure.ProcNum);//Get any potential Claimprocs for the Proc "i"
					if(listClaimProcsForLab.Count!=0) {//And proc "i" has labs. 
						for(int j=0;j<listClaimProcsForLab.Count;j++) {//Foreach lab
							listClaimProcsForLab[j].Status=_listClaimProcsForClaim[i].Status;//Match to parent status so estimates for ClaimProcs.CanadianLabBaseEstHelper().
							ClaimProcs.Update(listClaimProcsForLab[j]);//Both parent procs and labs need to have their statuses in synch when ComputeBaseEst
						}
					}
					_listClaimProcsForClaim[i].ClaimNum=0;
					if(procedure.ProcNumLab!=0) {
						//Skip lab procedures because their parent will handle their estimate updates below (including the status and claimNum set above).
						continue;
					}
					PatPlan patPlan=_listPatPlans.FirstOrDefault(x => x.InsSubNum==_listClaimProcsForClaim[i].InsSubNum);
					if(patPlan==null) {
						continue;
					}
					if(patPlan.Ordinal==1) {
						ClaimProcs.ComputeBaseEst(_listClaimProcsForClaim[i],procedure,insPlan,patPlan.PatPlanNum,listBenefits,null,null,_listPatPlans,0,0,_patient.Age,0
							,_listInsPlans,_listInsSubs,ListSubstitutionLinks,false,null,blueBookEstimateData:GetBlueBookEstimateData());
					}
					else {
						//We're not going to bother to also get paidOtherInsBaseEst:
						double paidOtherInsTotal=ClaimProcs.GetPaidOtherInsTotal(_listClaimProcsForClaim[i],_listPatPlans);
						double writeOffOtherIns=ClaimProcs.GetWriteOffOtherIns(_listClaimProcsForClaim[i],_listPatPlans);
						ClaimProcs.ComputeBaseEst(_listClaimProcsForClaim[i],procedure,insPlan,patPlan.PatPlanNum,listBenefits,null,null,_listPatPlans,paidOtherInsTotal
							,paidOtherInsTotal,_patient.Age,writeOffOtherIns,_listInsPlans,_listInsSubs,ListSubstitutionLinks,false,null,blueBookEstimateData:GetBlueBookEstimateData());
					}
					InsBlueBookLog insBlueBookLog=GetBlueBookEstimateData().CreateInsBlueBookLog(_listClaimProcsForClaim[i]);
					if(insBlueBookLog!=null) {
						InsBlueBookLogs.Insert(insBlueBookLog);
					}
					ClaimProcs.Update(_listClaimProcsForClaim[i]);
				}
			}
			ClaimProcs.DeleteEstimatesForDroppedPatPlan(_listClaimProcsForClaim);
			Claims.Delete(_claim,list835Attaches);
		}

		private bool AreAllReceived() {
			SetListClaimProcsForClaim(ClaimProcs.RefreshForClaim(_claim.ClaimNum,_listProcedures,_listClaimProcs));
			bool areAllReceived=true;
			for(int i = 0;i<_listClaimProcsForClaim.Count;i++) {
				if(((ClaimProc)_listClaimProcsForClaim[i]).Status==ClaimProcStatus.NotReceived) {
					areAllReceived=false;
				}
			}
			return areAllReceived;
		}

		private void MarkAllReceived() {
			for(int i = 0;i<_listClaimProcsForClaim.Count;i++) {
				if(_listClaimProcsForClaim[i].Status==ClaimProcStatus.NotReceived) {
					//ClaimProcs.Cur=(ClaimProc)ClaimProcs.ForClaim[i];
					_listClaimProcsForClaim[i].Status=ClaimProcStatus.Received;
					//We set the DateCP to Today's date when the user presses the buttons By Total, By Proc or Supplemental.
					//When there is a no payment claim, the user might simply change the claim status to received and press OK instead of entering payments the normal way, since there is no check.
					//Logically, we are changing claimproc status to received, and the claimproc will now be treated as a payment in the reports.
					//If we did not update DateCP, then DateCP for a zero payment claim would still be the procedure treatment planned date as much as a year ago, so the claimproc writeoffs (if present) would be accidentally back dated.
					_listClaimProcsForClaim[i].DateCP=DateTime.Today;
					_listClaimProcsForClaim[i].DateEntry=DateTime.Now;//date it was set rec'd
					ClaimProcs.Update(_listClaimProcsForClaim[i]);
				}
				//Get the lab procs associated to the current proc and mark as Received.
				List<ClaimProc> listClaimProcsForLab=GetListClaimProcsForProcNumLabsParent(_listClaimProcsForClaim[i].ProcNum);
				for(int j=0;j < listClaimProcsForLab.Count;j++) {
					if(listClaimProcsForLab[j].ClaimNum!=_listClaimProcsForClaim[i].ClaimNum) {
						continue;
					}
					listClaimProcsForLab[j].Status=ClaimProcStatus.Received;
					listClaimProcsForLab[j].DateCP=DateTime.Today;
					listClaimProcsForLab[j].DateEntry=DateTime.Now;//date it was set rec'd
					ClaimProcs.Update(listClaimProcsForLab[j]);
				}
			}
		}

		///<summary>Sets _blueBookEstimateData if it is null, then returns it.</summary>
		private BlueBookEstimateData GetBlueBookEstimateData() {
			if(_blueBookEstimateData==null) {
				List<Procedure> listProcedures=_listProcedures.FindAll(x => _listClaimProcsForClaim.Any(y => y.ProcNum==x.ProcNum));
				_blueBookEstimateData=new BlueBookEstimateData(_listInsPlans,_listInsSubs,_listPatPlans,listProcedures,ListSubstitutionLinks);
			}
			return _blueBookEstimateData;
		}

		///<summary>If InsAutoReceiveNoAssign pref is on, can automatically receive the claim displayed in the form if its status has changed to Sent.
		///If a Canadian claim was sent, this may receive it and any automatically sent secondary claims.</summary>
		private void ReceiveAsNoPaymentIfNeeded() {
			//Refresh because _claim and UI may not be updated.
			Claim claim=Claims.GetClaim(_claim.ClaimNum);
			//Kick out if status hasn't changed to "Sent" from some other status.
			if(_claimOld.ClaimStatus=="S" || claim.ClaimStatus!="S") {
				return;
			}
			if(Claims.ReceiveAsNoPaymentIfNeeded(claim.ClaimNum)) {
				//The method above changes and updates the claim and claimprocs.
				//Refresh them for use in closing method.
				_listClaimProcs=ClaimProcs.Refresh(_patient.PatNum);
				_claim=Claims.GetClaim(_claim.ClaimNum);
				FillGrids();
				//These must be set to trigger prompt for secondary claim if needed.
				_isPaymentEntered=true;
				comboClaimStatus.SelectedIndex=_listClaimStatuses.IndexOf(ClaimStatus.Received);
				textDateRec.Text=DateTime.Today.ToShortDateString();
			}
			if(_canadianSecondaryClaimNum!=0) {
				Claims.ReceiveAsNoPaymentIfNeeded(_canadianSecondaryClaimNum);
			}
		}

		///<summary>We have to set _blueBookEstimateData to null when _listClaimProcsForClaim is assigned because we may not have all of the
		///blue book data that we need for the procedures associated to our new list of ClaimProcs.</summary>
		private void SetListClaimProcsForClaim(List<ClaimProc> listClaimProcs) {
			_listClaimProcsForClaim=listClaimProcs;
			_blueBookEstimateData=null;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!ClaimIsValid()){
				return;
			}
			if(comboClaimStatus.SelectedIndex==_listClaimStatuses.IndexOf(ClaimStatus.HoldForInProcess) 
				&& _claimOld.ClaimStatus!=ClaimStatus.HoldForInProcess.GetDescription(useShortVersionIfAvailable:true)) { //If status changed to I from something else
				MsgBox.Show(Lan.g(this,"Cannot change status to "+ClaimStatus.HoldForInProcess.GetDescription()+"."));
				return;
			}
			//if status is received, all claimprocs must also be received.
			if(comboClaimStatus.SelectedIndex==_listClaimStatuses.IndexOf(ClaimStatus.Received)) {
				if(!AreAllReceived()){
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"All items will be marked received.  Continue?")){
						return;
					}
					MarkAllReceived();
				}
			}
			else{//claim is any status except received
				bool areAnyReceived=false;
				for(int i=0;i<_listClaimProcsForClaim.Count;i++){
					if(((ClaimProc)_listClaimProcsForClaim[i]).Status==ClaimProcStatus.Received){
						areAnyReceived=true;
					}
				}
				if(areAnyReceived){
					//Too dangerous to automatically set items not received because I would have to check for attachments to checks, etc.
					//Also too annoying to block user.
					//So just warn user.
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Some of the items are marked received.  This is not a good idea since it will cause them to show in the Account as a 'payment'.  Continue anyway?")){
						return;
					}
				}
			}
			//if status is received and there is no received date
			if(comboClaimStatus.SelectedIndex==_listClaimStatuses.IndexOf(ClaimStatus.Received) && textDateRec.Text==""){
				textDateRec.Text=DateTime.Today.ToShortDateString();
			}
			ClaimEdit.UpdateData updateData=UpdateClaim();
			if(comboClaimStatus.SelectedIndex==_listClaimStatuses.IndexOf(ClaimStatus.WaitingToSend)){//waiting to send
				ClaimSendQueueItem[] claimSendQueueItemsArray=updateData.ListSendQueueItems;
				if(claimSendQueueItemsArray.Length==0 || !claimSendQueueItemsArray[0].CanSendElect) {//listQueue can be empty if another workstation deleted the claim
					DialogResult=DialogResult.OK;
					return;
				}
				//string warnings;
				//string missingData=
				Clearinghouse clearinghouseHq=ClearinghouseL.GetClearinghouseHq(claimSendQueueItemsArray[0].ClearinghouseNum);
				Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
				claimSendQueueItemsArray[0]=Eclaims.GetMissingData(clearinghouseClin,claimSendQueueItemsArray[0]);
				if(!string.IsNullOrEmpty(claimSendQueueItemsArray[0].ErrorsPreventingSave)) {
					MessageBox.Show(claimSendQueueItemsArray[0].ErrorsPreventingSave);
					return;
				}
				else if(claimSendQueueItemsArray[0].MissingData!="") {
					if(MessageBox.Show(Lan.g(this,"Cannot send claim until missing/invalid data is fixed:")+"\r\n"+claimSendQueueItemsArray[0].MissingData+"\r\n\r\nContinue anyway?",
						"",MessageBoxButtons.OKCancel)==DialogResult.OK)
					{
						DialogResult=DialogResult.OK;
					}
					return;
				}
				else if(clearinghouseClin.IsAttachmentSendAllowed && clearinghouseClin.CommBridge==EclaimsCommBridge.ClaimConnect) {//No missing data, but can send attachments electronically
					//Validating for attachments is currently only supported for ClaimConnect customers
					ClaimConnect.ValidateClaimResponse validateClaimResponse=null;
					try {
						validateClaimResponse=ClaimConnect.ValidateClaim(_claim,true);
					}
					catch(ODException ex) {
						MessageBox.Show(ex.Message);
					}
					catch(Exception ex) {
						FriendlyException.Show(ex.Message,ex);
					}
					if(validateClaimResponse!=null//catches do not return
						&& validateClaimResponse.IsAttachmentRequired) 
					{
						if(_claim.AttachedFlags!="Misc") {//No need to visit the database again if this is already the case
							_claim.AttachedFlags="Misc";
							Claims.Update(_claim);
						}
						if(MsgBox.Show(this,MsgBoxButtons.YesNo,"An attachment is required for this claim. Would you like to open the claim attachment form?")) {
							FormClaimAttachment.Open(_claim);
						}
					}
					else if(_claim.Attachments.Count==0 && _claim.AttachedFlags!="Mail") {//Don't set to 'Mail' on claims with attachments or make unneccessary DB trips.
						_claim.AttachedFlags="Mail";
						Claims.Update(_claim);
					}
					if(validateClaimResponse!=null && validateClaimResponse.ValidationErrors.Any(x=>x.ToLower().Contains("payer does not support attachments"))) {
						if(_claim.AttachedFlags!="") {
							_claim.AttachedFlags+=","; //Flags exist already, so add a comma before adding "Unsup"
						}
						_claim.AttachedFlags+="Unsup";
						Claims.Update(_claim);
					}
				}
				//if(MsgBox.Show(this,true,"Send electronic claim immediately?")){
				//	List<ClaimSendQueueItem> queueItems=new List<ClaimSendQueueItem>();
				//	queueItems.Add(listQueue[0]);
				//	Eclaims.Eclaims.SendBatches(queueItems);//this also calls SetClaimSentOrPrinted which creates the etrans entry.
				//}
			}
			if(comboClaimStatus.SelectedIndex==_listClaimStatuses.IndexOf(ClaimStatus.Received)) {//Received
				ShowProviderTransferWindow(_claim,_patient,_family);
			}
			Plugins.HookAddCode(this,"FormClaimEdit.butOK_Click_end",_claim);
			DialogResult=DialogResult.OK;
		}

		//cancel does not cancel in some circumstances because cur gets updated in some areas.
		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormClaimEdit_CloseXClicked(object sender, CancelEventArgs e){
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Changes will be lost.  Continue anyway?")){
				e.Cancel=true;
			}
		}

		private void FormClaimEdit_Closing(object sender,System.ComponentModel.CancelEventArgs e) {
			DateTime dateTimeClaimSecEdit=_claim.SecDateTEdit;//Preserve the date prior to any claim updates effecting it.
			if(DialogResult==DialogResult.OK) {
				if(_isDeleting) {
					InsBlueBooks.DeleteByClaimNums(_claim.ClaimNum);
					return;
				}
				InsBlueBooks.SynchForClaimNums(_claim.ClaimNum);
				if(IsNew) {
					SecurityLogs.MakeLogEntry(PermissionClaimEdit,_patient.PatNum,"New claim created for "+_patient.LName+","+_patient.FName,
						_claim.ClaimNum,dateTimeClaimSecEdit);
				}
				else {//save for all except Delete
					SecurityLogs.MakeLogEntry(PermissionClaimEdit,_patient.PatNum,"Claim saved for "+_patient.LName+","+_patient.FName,
						_claim.ClaimNum,dateTimeClaimSecEdit);
				}
				ReceiveAsNoPaymentIfNeeded();
				//Claim is flagged as Received and the user entered payment information while the window was open.
				if(comboClaimStatus.SelectedIndex==_listClaimStatuses.IndexOf(ClaimStatus.Received) && _isPaymentEntered) {
					if(PrefC.GetBool(PrefName.PromptForSecondaryClaim) && Security.IsAuthorized(Permissions.ClaimSend,true)) {
						//We currenlty require that payment be entered in this instance of the form.
						//We might later decide that we want to check for secondary whenever the primary is recieved and there is financial values entered
						//regardless of when they were entered.
						ClaimL.PromptForSecondaryClaim(_listClaimProcsForClaim);
					}
					//Try to transfer any patient payment around on the procedures associated to this claim if necessary (e.g. if Open Dental guessed deductible incorrectly).
					if(Security.IsAuthorized(Permissions.PaymentCreate,DateTime.Today,true)) {
						PaymentEdit.MakeIncomeTransferForClaimProcs(_patient.PatNum,_listClaimProcsForClaim);
					}
				}
				return;
			}
			if(!IsNew) {
				InsBlueBooks.SynchForClaimNums(_claim.ClaimNum);
				return;
			}
			if(_claim.InsPayAmt>0) {
				MsgBox.Show(this,"Not allowed to cancel because an insurance payment was entered.  Either click OK, or zero out the insurance payments.");
				e.Cancel=true;
				return;
			}
			//Check to see if the claim has already been deleted by another instance of Open Dental.
			Claim claimFromDb=Claims.GetClaim(_claim.ClaimNum);
			if(claimFromDb==null) {
				MsgBox.Show(Lan.g(this,"This claim has been deleted by another instance of the program."));
				return;
			}
			//Compare the current claim status to the one from the database to make sure it has not changed before deleting it if new.
			if(_claim.ClaimStatus!=claimFromDb.ClaimStatus) {
				MsgBox.Show(Lan.g(this,"The claim status has been changed by another instance of the program. Claim has not been deleted."));
				return;
			}
			//This is a new claim where they clicked "Cancel".
			SecurityLogs.MakeLogEntry(Permissions.ClaimEdit,_patient.PatNum,"New claim cancelled for "+_patient.LName+","+_patient.FName,
				_claim.ClaimNum,dateTimeClaimSecEdit);
			DeleteClaimHelper();
			InsBlueBooks.DeleteByClaimNums(_claim.ClaimNum);
			//When the user "cancels" out of a new claim we want to delete any corresponding claim snapshots, but only if not using the service trigger type
			//The service trigger type snapshots have nothing to do with creating this claim, so leave as is.
			if(PrefC.GetBool(PrefName.ClaimSnapshotEnabled)
				&& PIn.Enum<ClaimSnapshotTrigger>(PrefC.GetString(PrefName.ClaimSnapshotTriggerType),true)!=ClaimSnapshotTrigger.Service)
			{
				ClaimSnapshots.DeleteForClaimProcs(_listClaimProcsForClaim.Select(x => x.ClaimProcNum).ToList());
			}
		}
	}
}
