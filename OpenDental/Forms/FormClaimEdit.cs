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
		///<summary>Contains the claimprocs for the claim.</summary>
		private List<ClaimProc> _listClaimProcsForClaim;
		///<summary>All claimprocs for the patient. Used to calculate remaining benefits, etc.</summary>
		private List<ClaimProc> ClaimProcList;
		/// <summary>List of all procedures for this patient.  Used to get descriptions, etc.</summary>
		private List<Procedure> ProcList;
		private Patient PatCur;
		private Family FamCur;
		private List <InsPlan> PlanList;
		///<summary>List of substitution links.  Lazy loaded, do not directly use this variable, use the property instead.</summary>
		private List<SubstitutionLink> _listSubLinks=null;
		private DataTable tablePayments;
		///<summary>When user first opens form, if the claim is S or R, and the user does not have permission, the user is informed, and this is set to true.</summary>
		private bool notAuthorized;
		private List <PatPlan> PatPlanList;
		private Claim ClaimCur;
		private Claim _claimOld;
		private List<ClaimValCodeLog> ListClaimValCodes;
		///<summary>can be null</summary>
		private ClaimCondCodeLog ClaimCondCodeLogCur;
		private bool doubleClickWarningAlreadyDisplayed=false;
		private List<InsSub> SubList;
		///<summary>If this claim edit window is accessed from the batch ins window, then set this to true to hide the batch button in this window and prevent loop.</summary>
		public bool IsFromBatchWindow;
		///<summary>The Ordering provider, for medical claims.</summary>
		private long _provNumOrdering;
		///<summary>If this claim is attached to an ordering referral, then this varible will not be null.</summary>
		private Referral _referralOrdering=null;
		///<summary>Dictionary such that the key is a parent procNum and the value is a list of claimProcs for labs associated to the parent proc.</summary>
		private Dictionary<long,List<ClaimProc>> _dictCanadianLabClaimProcs=new Dictionary<long, List<ClaimProc>>();
		///<summary>When true, a supplemental payment will get automatically created on Shown().</summary>
		private bool _isForOrthoAutoPay;
		private bool _isDeleting;
		private List<ClaimForm> _listClaimForms;
		///<summary>Set true if user entered payment.</summary>
		private bool _isPaymentEntered;
		///<summary>Data necessary to load the form.</summary>
		private ClaimEdit.LoadData _loadData;
		/// <summary>Claim status based on preference and claim type. Used to fill comboClaimStatus too.</summary>
		private List<ClaimStatus> _listClaimStatus;
		///<summary>Holds all data needed to make blue book estimates.</summary>
		private BlueBookEstimateData _blueBookEstimateData;
		///<summary>Clearinghouse for current claim set in load method and then used to skip any unneccessary flag setting logic in UpdateClaim()</summary>
		private Clearinghouse _clearinghouse;

		private List<SubstitutionLink> ListSubLinks {
			get {
				if(_listSubLinks==null) {
					_listSubLinks=SubstitutionLinks.GetAllForPlans(PlanList);
				}
				return _listSubLinks;
			}
		}

		///<summary>The permissions that logs should be made under. Uses the status of the claim when entering the window.</summary>
		private Permissions _claimEditPermission {
			get {
				if(_claimOld==null) {
					return Permissions.ClaimEdit;
				}
				return ListTools.In(_claimOld.ClaimStatus,"S","R") ? Permissions.ClaimSentEdit : Permissions.ClaimEdit;
			}
		}

		protected override string GetHelpOverride(){
			if(ClaimCur.ClaimType.ToLower()=="preauth"){
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
		public FormClaimEdit(Claim claimCur, Patient patCur,Family famCur, bool isForOrthoAutoPay = false) {
			PatCur=patCur;
			FamCur=famCur;
			ClaimCur=claimCur;
			_claimOld=claimCur.Copy();
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
			List<long> listOrthoBandingCodeNums = ProcedureCodes.GetOrthoBandingCodeNums();
			List<Procedure> listProcs=Procedures.GetManyProc(_listClaimProcsForClaim.Select(x => x.ProcNum).ToList(),false);
			for(int i = 0;i < _listClaimProcsForClaim.Count;i++) {
				ClaimProc cpCur = _listClaimProcsForClaim[i];
				Procedure procCur = listProcs.Find(x => x.ProcNum==cpCur.ProcNum);
				if(procCur==null) {
					continue;
				}
				if(cpCur.Status == ClaimProcStatus.Received
					&& listOrthoBandingCodeNums.Contains(procCur.CodeNum))
				{
					gridProc.SetSelected(i,true);
				}
			}
			MakeSuppPayment();
		}
		
		private void FormClaimEdit_Load(object sender, System.EventArgs e) {
			_loadData=ClaimEdit.GetLoadData(PatCur,FamCur,ClaimCur);
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
				tabMain.SelectedTab=tabCanadian;
				if(ClaimCur.DateSent.Date==MiscData.GetNowDateTime().Date) { //Reversal can only happen on the same day that the claim was originally sent.
					butReverse.Enabled=(ClaimCur.CanadaTransRefNum!=null && ClaimCur.CanadaTransRefNum.Trim()!="");
				}
				butSend.Enabled=(ClaimCur.CanadaTransRefNum==null || ClaimCur.CanadaTransRefNum.Trim()=="");
				butViewEra.Visible=false;
			}
			else {
				LayoutManager.Remove(tabCanadian);
			}
			if(IsNew){
				//butCheckAdd.Enabled=false; //button was removed.
				groupEnterPayment.Enabled=false;
			}
			else if(ListTools.In(ClaimCur.ClaimStatus,ClaimStatus.Sent.GetDescription(true),ClaimStatus.Received.GetDescription(true))){//sent or received
				if((ClaimCur.ClaimType=="PreAuth" && !Security.IsAuthorized(Permissions.PreAuthSentEdit,ClaimCur.DateSent))
					|| (ClaimCur.ClaimType!="PreAuth" && !Security.IsAuthorized(Permissions.ClaimSentEdit,ClaimCur.DateSent))) 
				{ 
					butOK.Enabled=false;
					butDelete.Enabled=false;
					//butPrint.Enabled=false;//allowed to print, but just won't save changes.
					notAuthorized=true;
					groupEnterPayment.Enabled=false;
					//gridProc.Enabled=false; //leave this enabled so users can still scroll through.
					comboClaimStatus.Enabled=false;
					//butCheckAdd.Enabled=false; //button was removed.
				}
			}
			if(!IsNew && ((ClaimCur.ClaimType!="PreAuth" && !Security.IsAuthorized(Permissions.ClaimDelete,ClaimCur.SecDateEntry,true)))) { 
				butDelete.Enabled=false;
			}
			if(ClaimCur.ClaimType=="PreAuth"){
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
			_listClaimStatus=GetListComboClaimStatus();
			comboClaimStatus.Items.Clear();
			for(int i=0;i<_listClaimStatus.Count;i++) {
				comboClaimStatus.Items.Add(_listClaimStatus[i].GetDescription(false));
			}
			comboSpecialProgram.Items.Clear();
			for(int i=0;i<Enum.GetNames(typeof(EnumClaimSpecialProgram)).Length;i++) {
				comboSpecialProgram.Items.Add(Enum.GetNames(typeof(EnumClaimSpecialProgram))[i]);
			}
			string[] enumRelat=Enum.GetNames(typeof(Relat));
			for(int i=0;i<enumRelat.Length;i++){;
				comboPatRelat.Items.Add(Lan.g("enumRelat",enumRelat[i]));
				comboPatRelat2.Items.Add(Lan.g("enumRelat",enumRelat[i]));
			}
      ClaimProcList=_loadData.ListClaimProcs;
			ProcList=_loadData.ListProcs;
			SubList=_loadData.ListInsSubs;
			PlanList=_loadData.ListInsPlans;
			PatPlanList=_loadData.ListPatPlans;
			tablePayments=_loadData.TablePayments;
			InsPlan insPlan=InsPlans.GetPlan(ClaimCur.PlanNum,PlanList);
			//If there is no insplan then we need to close
			if(insPlan==null) {
				MsgBox.Show(Lans.g(this,"Invalid insurance plan associated to claim. Please run database maintenance method")+" "+nameof(DatabaseMaintenances.InsSubNumMismatchPlanNum)
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
			textRefNum.Text=ClaimCur.RefNumString;
			string[] enumPlaceOfService=Enum.GetNames(typeof(PlaceOfService));
			for(int i=0;i<enumPlaceOfService.Length;i++) {
				comboPlaceService.Items.Add(Lan.g("enumPlaceOfService",enumPlaceOfService[i]));
			}
			comboPlaceService.SelectedIndex=(int)ClaimCur.PlaceService;
			string[] enumYN=Enum.GetNames(typeof(YN));
			for(int i=0;i<enumYN.Length;i++) {
				comboEmployRelated.Items.Add(Lan.g("enumYN",enumYN[i]));
			}
			comboEmployRelated.SelectedIndex=(int)ClaimCur.EmployRelated;
			comboAccident.Items.Add(Lan.g(this,"No"));
			comboAccident.Items.Add(Lan.g(this,"Auto"));
			comboAccident.Items.Add(Lan.g(this,"Employment"));
			comboAccident.Items.Add(Lan.g(this,"Other"));
			switch(ClaimCur.AccidentRelated) {
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
			textAccidentST.Text=ClaimCur.AccidentST;
			textRefProv.Text=Referrals.GetNameLF(ClaimCur.ReferringProv);
			if(ClaimCur.ReferringProv==0){
				butReferralEdit.Enabled=false;
			}
			else{
				butReferralEdit.Enabled=true;
			}
			//medical data
			ListClaimValCodes=_loadData.ListClaimValCodes;
			ClaimCondCodeLogCur=_loadData.ClaimCondCodeLogCur;
			comboClinic.SelectedClinicNum=ClaimCur.ClinicNum;
			SetOrderingProvider(null);//Clears both the internal ordering and referral ordering providers.
			if(ClaimCur.ProvOrderOverride!=0) {
				SetOrderingProvider(Providers.GetProv(ClaimCur.ProvOrderOverride));
			}
			else if(ClaimCur.OrderingReferralNum!=0) {
				Referral referral;
				Referrals.TryGetReferral(ClaimCur.OrderingReferralNum,out referral);
				SetOrderingReferral(referral);
			}
			FillCombosProv();
			if(ClaimCur.ProvBill==0){
				//setting combo to 0 would just show "0", and this field is required.
				comboProvBill.SetSelectedProvNum(Providers.GetFirst(true).ProvNum);
			}
			else{
				comboProvBill.SetSelectedProvNum(ClaimCur.ProvBill);
			}
			if(ClaimCur.ProvTreat==0){
				comboProvTreat.SetSelectedProvNum(Providers.GetFirst(true).ProvNum);
			}
			else{
				comboProvTreat.SetSelectedProvNum(ClaimCur.ProvTreat);
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
				List<Procedure> listSiteProcs=ProcList.FindAll(x => _listClaimProcsForClaim.Any(y => y.ProcNum==x.ProcNum) && x.SiteNum!=0);
				//Null is an acceptable site.
				if(listSiteProcs.Count > 0) {
					Site site=Sites.GetFirstOrDefault(x => x.SiteNum==listSiteProcs[0].SiteNum);
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
			ClaimSendQueueItem[] arrQueueList=Claims.GetQueueList(ClaimCur.ClaimNum,ClaimCur.ClinicNum,0);
			if(arrQueueList==null || arrQueueList.Length==0) {
				//Similar to how we handle clicking Payment As Total button when claim no longer exists in the database.
				MsgBox.Show(this,"Claim has been deleted by another user.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			Clearinghouse clearinghouseHq=ClearinghouseL.GetClearinghouseHq(arrQueueList[0].ClearinghouseNum);
			_clearinghouse=Clearinghouses.OverrideFields(clearinghouseHq,ClaimCur.ClinicNum);
			if(_clearinghouse!=null && _clearinghouse.IsAttachmentSendAllowed && _clearinghouse.CommBridge==EclaimsCommBridge.ClaimConnect) {
				//Disabling and hiding UI elements that don't make sense when using the ClaimConnect attachment service.
				tabNEA.Enabled=false;
				fillGridSentAttachments();
				tabAttach.SelectedTab=tabDXC;
			}
			SetBounds();
		}

		private void SetBounds(){
			Rectangle workingArea=System.Windows.Forms.Screen.FromControl(this).WorkingArea;
			//Window must start at 735 tall as a fail safe for compatibility with remote app.
			//We can adjust bigger if there is space.
			//But this adjustment cannot happen in the constructor because of the layout manager.
			int heightAvail=workingArea.Height;
			if(heightAvail>LayoutManager.Scale(871)){
				heightAvail=LayoutManager.Scale(871);
			}
			if(Height<heightAvail){
				Height=heightAvail;
			}	
			//Center the window on the parent.
			Location=new Point(workingArea.Left+workingArea.Width/2-Width/2,
				y:workingArea.Top+workingArea.Height/2-Height/2);
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
			foreach(Control ctrl in controlsInput.Controls) {
				foreach(Control ctrlSub in ctrl.Controls) {//Make sure all sub controls are read only.
					SetFormReadOnly(ctrlSub);
				}
				try {
					//Controls we wish to keep enabled for navigation purposes.
					if(ctrl==butCancel || ctrl==tabMain) {
						continue;
					}
					ctrl.Enabled=false;
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
			ClaimStatus curClaimStat=_listClaimStatus.Where(x=>x.GetDescription(true)==_claimOld.ClaimStatus).FirstOrDefault();
			if(ClaimCur.ClaimType=="PreAuth") {//we're in a PreAuth and trying to change the status from the dropdown
				//https://opendental.com/manual/preauth.html - See the "Receive a preauthorization" section.
				if(_listClaimStatus[comboClaimStatus.SelectedIndex]==ClaimStatus.Received) {
					MsgBox.Show("FormClaimEdit","Claim Status can only be marked as 'Received' by using the 'By Procedure' button.");
					comboClaimStatus.SelectedIndex=_listClaimStatus.IndexOf(curClaimStat);//put it back to previous selection
				}
			}
		}

		private void butPickOrderProvInternal_Click(object sender,EventArgs e) {
			using FormProviderPick formP = new FormProviderPick(comboProvBill.Items.GetAll<Provider>());
			formP.SelectedProvNum=_provNumOrdering;
			formP.ShowDialog();
			if(formP.DialogResult!=DialogResult.OK) {
				return;
			}
			SetOrderingProvider(Providers.GetProv(formP.SelectedProvNum));
		}

		private void butPickOrderProvReferral_Click(object sender,EventArgs e) {
			using FormReferralSelect form=new FormReferralSelect();
			form.IsSelectionMode=true;
			form.IsDoctorSelectionMode=true;
			form.IsShowPat=false;
			form.IsShowDoc=true;
			form.IsShowOther=false;
			form.ShowDialog();
			if(form.DialogResult!=DialogResult.OK) {
				return;
			}
			SetOrderingReferral(form.SelectedReferral);
		}

		private void butNoneOrderProv_Click(object sender,EventArgs e) {
			SetOrderingProvider(null);//Clears both the internal ordering and referral ordering providers.
		}

		private void SetOrderingProvider(Provider prov) {
			if(prov==null) {
				_provNumOrdering=0;
				textOrderingProviderOverride.Text="";
			}
			else {
				_provNumOrdering=prov.ProvNum;
				textOrderingProviderOverride.Text=prov.GetFormalName()+"  NPI: "+(prov.NationalProvID.Trim()==""?"Missing":prov.NationalProvID);
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
			using FormProviderPick formP = new FormProviderPick(comboProvBill.Items.GetAll<Provider>());
			formP.SelectedProvNum=comboProvBill.GetSelectedProvNum();
			formP.ShowDialog();
			if(formP.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProvBill.SetSelectedProvNum(formP.SelectedProvNum);
		}

		private void butPickProvTreat_Click(object sender,EventArgs e) {
			using FormProviderPick formP = new FormProviderPick(comboProvTreat.Items.GetAll<Provider>());
			formP.SelectedProvNum=comboProvTreat.GetSelectedProvNum();
			formP.ShowDialog();
			if(formP.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProvTreat.SetSelectedProvNum(formP.SelectedProvNum);
		}

		///<summary>Fills combo provider based on which clinic is selected and attempts to preserve provider selection if any.</summary>
		private void FillCombosProv() {
			List<Provider> listProvsForClinic=Providers.GetProvsForClinic(comboClinic.SelectedClinicNum);
			long provNum=comboProvBill.GetSelectedProvNum();
			comboProvBill.Items.Clear();
			comboProvBill.Items.AddProvsAbbr(listProvsForClinic);
			comboProvBill.SetSelectedProvNum(provNum);
			provNum=comboProvTreat.GetSelectedProvNum();
			comboProvTreat.Items.Clear();
			comboProvTreat.Items.AddProvsAbbr(listProvsForClinic);
			comboProvTreat.SetSelectedProvNum(provNum);
		}
		#endregion Provider Controls

		///<summary>Returns list of claim status that is filled on load based on PriClaimAllowSetToHoldUntilPriReceived pref and ClaimCur.ClaimType and status of claim.</summary>
		private List<ClaimStatus> GetListComboClaimStatus() {
			List<ClaimStatus> retVal=Enum.GetValues(typeof(ClaimStatus)).OfType<ClaimStatus>().ToList();
			if(PrefC.GetBool(PrefName.PriClaimAllowSetToHoldUntilPriReceived)
				|| ClaimCur.ClaimType!="P"//Pref only applies to primary claims.
				|| ClaimCur.ClaimStatus==ClaimStatus.HoldUntilPriReceived.GetDescription(true))//Status not allowed but we must maintain current selection.
			{
				return retVal;
			}
			//Preference does not allow status and claim was not set to status prior to disabling
			return retVal.FindAll(x => x!=ClaimStatus.HoldUntilPriReceived);
		}

		///<summary>Provides ClaimStatus enum from the current claim. Defaults to None (this should never happen)</summary>
		private bool TryGetClaimStatusEnumFromCurClaim(out ClaimStatus status) {
			status=ClaimStatus.Unsent;//Can set to anything initially.
			for(int i=0;i<_listClaimStatus.Count;i++) {
				if(ClaimCur.ClaimStatus!=_listClaimStatus[i].GetDescription(true)) {
					continue;
				}
				status=_listClaimStatus[i];
				return true;
			}
			return false;
		}

		///<summary></summary>
		public void FillForm(){
			if(ClaimCur==null) {
				MsgBox.Show(this, "This Claim has been deleted by another user.");
				DialogResult=DialogResult.Cancel;
				this.Close();
				return;
			}
			if(ClaimCur.ClaimType=="PreAuth") {
				this.Text=Lan.g(this,"Edit Preauthorization")+" - "+PatCur.GetNameLF();
			}
			else {
				this.Text=Lan.g(this,"Edit Claim")+" - "+PatCur.GetNameLF();
			}
			if(ClaimCur.DateService.Year<1880) {
				textDateService.Text="";
			}
			else {
				textDateService.Text=ClaimCur.DateService.ToShortDateString();
			}
			if(ClaimCur.DateSent.Year<1880) {
				textDateSent.Text="";
			}
			else {
				textDateSent.Text=ClaimCur.DateSent.ToShortDateString();
			}
			if(ClaimCur.DateReceived.Year<1880) {
				textDateRec.Text="";
			}
			else {
				textDateRec.Text=ClaimCur.DateReceived.ToShortDateString();
			}
			if(TryGetClaimStatusEnumFromCurClaim(out ClaimStatus claimStatus)){
				comboClaimStatus.SelectedIndex=_listClaimStatus.IndexOf(claimStatus);
			}
			else {//This should never happen.
				MsgBox.Show("There was an error setting the claim status.");
				this.DialogResult=DialogResult.Cancel;
				this.Close();
			}
			switch(ClaimCur.ClaimType){
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
			comboMedType.SelectedIndex=(int)ClaimCur.MedType;
			comboClaimForm.Items.Clear();
			comboClaimForm.Items.Add(Lan.g(this,"None"));
			comboClaimForm.SelectedIndex=0;
			_listClaimForms=ClaimForms.GetDeepCopy(true);
			for(int i=0;i<_listClaimForms.Count;i++){
				comboClaimForm.Items.Add(_listClaimForms[i].Description);
				if(ClaimCur.ClaimForm==_listClaimForms[i].ClaimFormNum){
					comboClaimForm.SelectedIndex=i+1;
				}
			}
			comboClinic.SelectedClinicNum=ClaimCur.ClinicNum;
			if(Defs.GetDefsForCategory(DefCat.ClaimCustomTracking,true).Count==0) {  // Disable Add button if all defs are hidden (nothing to add)
				butAdd.Visible=false;
			}
			else{
				butAdd.Visible=true;
			}
			textPriorAuth.Text=ClaimCur.PriorAuthorizationNumber;
			textPredeterm.Text=ClaimCur.PreAuthString;
			comboSpecialProgram.SelectedIndex=(int)ClaimCur.SpecialProgramCode;
			textPlan.Text=InsPlans.GetDescript(ClaimCur.PlanNum,FamCur,PlanList,ClaimCur.InsSubNum,SubList);
			comboPatRelat.SelectedIndex=(int)ClaimCur.PatRelat;
			textPlan2.Text=InsPlans.GetDescript(ClaimCur.PlanNum2,FamCur,PlanList,ClaimCur.InsSubNum2,SubList);
			comboPatRelat2.SelectedIndex=(int)ClaimCur.PatRelat2;
			if(textPlan2.Text==""){
				comboPatRelat2.Visible=false;
				label10.Visible=false;
			}
			else{
				comboPatRelat2.Visible=true;
				label10.Visible=true;
			}
			switch(ClaimCur.IsProsthesis){
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
			if(ClaimCur.PriorDate.Year < 1880){
				textPriorDate.Text="";
			}
			else{
				textPriorDate.Text=ClaimCur.PriorDate.ToShortDateString();
			}
			textReasonUnder.Text=ClaimCur.ReasonUnderPaid;
			textNote.Text=ClaimCur.ClaimNote;
			checkIsOrtho.Checked=ClaimCur.IsOrtho;
			textOrthoTotalM.Text=ClaimCur.OrthoTotalM.ToString();
			textOrthoRemainM.Text=ClaimCur.OrthoRemainM.ToString();
			if(ClaimCur.OrthoDate.Year < 1860){
				textOrthoDate.Text="";
			}
			else{
				textOrthoDate.Text=ClaimCur.OrthoDate.ToShortDateString();
				if(PrefC.GetBool(PrefName.OrthoClaimUseDatePlacement) && ClaimCur.OrthoDate != null && ClaimCur.OrthoDate.Year > 1880) {
					textOrthoDate.Enabled=false;
				}
			}
			if(ClaimCur.DateResent.Year>1880) {
				textDateSent.Text=ClaimCur.DateResent.ToShortDateString();
				labelDateSent.Text="Date Resent";
			}
			if(ClaimCur.DateSentOrig.Year>1880) {
				textDateSentOrig.Text=ClaimCur.DateSentOrig.ToShortDateString();
			}
			else {
				textDateSentOrig.Text="";
			}
			string[] claimCorrectionTypeNames=Enum.GetNames(typeof(ClaimCorrectionType));
			Array claimCorrectionTypeValues=Enum.GetValues(typeof(ClaimCorrectionType));
			comboCorrectionType.Items.Clear();
			for(int i=0;i<claimCorrectionTypeNames.Length;i++) {
				comboCorrectionType.Items.Add(claimCorrectionTypeNames[i]);
				if((ClaimCorrectionType)claimCorrectionTypeValues.GetValue(i)==ClaimCur.CorrectionType) {
					comboCorrectionType.SelectedIndex=i;
				}
			}
			textClaimIdOriginal.Text=Claims.ConvertClaimId(ClaimCur,PatCur);
			textClaimIdentifier.Text=string.IsNullOrWhiteSpace(ClaimCur.ClaimIdentifier) ? textClaimIdOriginal.Text : ClaimCur.ClaimIdentifier;
			textOrigRefNum.Text=ClaimCur.OrigRefNum;
			if(ClaimCur.ShareOfCost > 0) {
				textShareOfCost.Text=ClaimCur.ShareOfCost.ToString("F2");
			}
			//Canadian------------------------------------------------------------------
			//(there's also a FillCanadian section for fields that do not collide with USA fields)
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				if(ClaimCur.AccidentDate.Year<1880) {
					textCanadianAccidentDate.Text="";
				}
				else {
					textCanadianAccidentDate.Text=ClaimCur.AccidentDate.ToShortDateString();
				}
				checkCanadianIsOrtho.Checked=ClaimCur.IsOrtho;
			}
			else {
				if(ClaimCur.AccidentDate.Year<1880) {
					textAccidentDate.Text="";
				}
				else {
					textAccidentDate.Text=ClaimCur.AccidentDate.ToShortDateString();
				}
				checkIsOrtho.Checked=ClaimCur.IsOrtho;
			}
			textCanadaTransRefNum.Text=ClaimCur.CanadaTransRefNum;
			groupCanadaOrthoPredeterm.Enabled=(ClaimCur.ClaimType=="PreAuth");
			if(ClaimCur.CanadaEstTreatStartDate.Year<1880) {
				textDateCanadaEstTreatStartDate.Text="";
			}
			else {
				textDateCanadaEstTreatStartDate.Text=ClaimCur.CanadaEstTreatStartDate.ToShortDateString();
			}
			if(ClaimCur.CanadaInitialPayment==0) {
				textCanadaInitialPayment.Text="";
			}
			else {
				textCanadaInitialPayment.Text=ClaimCur.CanadaInitialPayment.ToString("F");
			}
			textCanadaExpectedPayCycle.Text=ClaimCur.CanadaPaymentMode.ToString("#");
			textCanadaTreatDuration.Text=ClaimCur.CanadaTreatDuration.ToString("##");
			textCanadaNumPaymentsAnticipated.Text=ClaimCur.CanadaNumAnticipatedPayments.ToString("##");
			if(ClaimCur.CanadaAnticipatedPayAmount==0) {
				textCanadaAnticipatedPayAmount.Text="";
			}
			else {
				textCanadaAnticipatedPayAmount.Text=ClaimCur.CanadaAnticipatedPayAmount.ToString("F");
			}
			//attachments------------------
			textRadiographs.Text=ClaimCur.Radiographs.ToString();
			textAttachImages.Text=ClaimCur.AttachedImages.ToString();
			textAttachModels.Text=ClaimCur.AttachedModels.ToString();
			checkAttachEoB.Checked=false;
			checkAttachNarrative.Checked=false;
			checkAttachPerio.Checked=false;
			checkAttachMisc.Checked=false;
			radioAttachMail.Checked=true;
			string[] flags=ClaimCur.AttachedFlags.Split(',');
			for(int i=0;i<flags.Length;i++){
				switch(flags[i]){
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
			if(ClaimCur.AttachmentID.ToLower().StartsWith("dxc")) {
				textAttachmentID.Text=ClaimCur.AttachmentID;
			}
			else {
				textAttachID.Text=ClaimCur.AttachmentID;
			}
			//medical/inst
			textBillType.Text=ClaimCur.UniformBillType;
			textAdmissionType.Text=ClaimCur.AdmissionTypeCode;
			textAdmissionSource.Text=ClaimCur.AdmissionSourceCode;
			textPatientStatus.Text=ClaimCur.PatientStatusCode;
			if(ClaimCondCodeLogCur!=null && ClaimCondCodeLogCur.ClaimNum!=0) {
				textCode0.Text=ClaimCondCodeLogCur.Code0.ToString();
				textCode1.Text=ClaimCondCodeLogCur.Code1.ToString();
				textCode2.Text=ClaimCondCodeLogCur.Code2.ToString();
				textCode3.Text=ClaimCondCodeLogCur.Code3.ToString();
				textCode4.Text=ClaimCondCodeLogCur.Code4.ToString();
				textCode5.Text=ClaimCondCodeLogCur.Code5.ToString();
				textCode6.Text=ClaimCondCodeLogCur.Code6.ToString();
				textCode7.Text=ClaimCondCodeLogCur.Code7.ToString();
				textCode8.Text=ClaimCondCodeLogCur.Code8.ToString();
				textCode9.Text=ClaimCondCodeLogCur.Code9.ToString();
				textCode10.Text=ClaimCondCodeLogCur.Code10.ToString();
			}
			if(ListClaimValCodes!=null) {
				FillValCode(ListClaimValCodes,0,textVC39aCode,textVC39aAmt);
				FillValCode(ListClaimValCodes,1,textVC40aCode,textVC40aAmt);
				FillValCode(ListClaimValCodes,2,textVC41aCode,textVC41aAmt);
				FillValCode(ListClaimValCodes,3,textVC39bCode,textVC39bAmt);
				FillValCode(ListClaimValCodes,4,textVC40bCode,textVC40bAmt);
				FillValCode(ListClaimValCodes,5,textVC41bCode,textVC41bAmt);
				FillValCode(ListClaimValCodes,6,textVC39cCode,textVC39cAmt);
				FillValCode(ListClaimValCodes,7,textVC40cCode,textVC40cAmt);
				FillValCode(ListClaimValCodes,8,textVC41cCode,textVC41cAmt);
				FillValCode(ListClaimValCodes,9,textVC39dCode,textVC39dAmt);
				FillValCode(ListClaimValCodes,10,textVC40dCode,textVC40dAmt);
				FillValCode(ListClaimValCodes,11,textVC41dCode,textVC41dAmt);
			}
			textDateIllness.Text=ClaimCur.DateIllnessInjuryPreg.Year<1880?"":ClaimCur.DateIllnessInjuryPreg.ToShortDateString();
			comboDateIllnessQualifier.Items.AddEnums<DateIllnessInjuryPregQualifier>();
			//this enum has non-standard number values, so we can't just cast to int
			comboDateIllnessQualifier.SelectedIndex=Array.IndexOf(Enum.GetValues(typeof(DateIllnessInjuryPregQualifier)),ClaimCur.DateIllnessInjuryPregQualifier);
			if(comboDateIllnessQualifier.SelectedIndex==-1){
				comboDateIllnessQualifier.SelectedIndex=0;
			}
			textDateOther.Text=ClaimCur.DateOther.Year<1880?"":ClaimCur.DateOther.ToShortDateString();
			comboDateOtherQualifier.Items.AddEnums<DateOtherQualifier>();
			//this enum has non-standard number values, so we can't just cast to int
			comboDateOtherQualifier.SelectedIndex=Array.IndexOf(Enum.GetValues(typeof(DateOtherQualifier)),ClaimCur.DateOtherQualifier);
			if(comboDateOtherQualifier.SelectedIndex==-1){
				comboDateOtherQualifier.SelectedIndex=0;
			}
			checkIsOutsideLab.Checked=ClaimCur.IsOutsideLab;
			FillGrids(false);
			FillAttachments();
		}

		///<summary>If valCodeIdx>listClaimValCodes.Count then nothing will happen, otherwise it will fill the text boxes with appropriate values
		///from the listClaimValCodes[valCodeIdx].</summary>
		private static void FillValCode(List<ClaimValCodeLog> listClaimValCodes,int valCodeIdx,TextBox textVCCode,TextBox textVCAmount) {
			if(listClaimValCodes.Count>valCodeIdx) {
				textVCCode.Text=listClaimValCodes[valCodeIdx].ValCode;
				textVCAmount.Text=listClaimValCodes[valCodeIdx].ValAmount.ToString();
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
			if(ClaimCur.CanadianMaterialsForwarded.Contains("E")) {
				checkEmail.Checked=true;
			}
			if(ClaimCur.CanadianMaterialsForwarded.Contains("C")) {
				checkCorrespondence.Checked=true;
			}
			if(ClaimCur.CanadianMaterialsForwarded.Contains("M")) {
				checkModels.Checked=true;
			}
			if(ClaimCur.CanadianMaterialsForwarded.Contains("X")) {
				checkXrays.Checked=true;
			}
			if(ClaimCur.CanadianMaterialsForwarded.Contains("I")) {
				checkImages.Checked=true;
			}
			textReferralProvider.Text=ClaimCur.CanadianReferralProviderNum;
			comboReferralReason.SelectedIndex=ClaimCur.CanadianReferralReason;
			//max prosth-----------------------------------------------------------------------------------------------------
			switch(ClaimCur.CanadianIsInitialUpper){
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
			if(ClaimCur.CanadianDateInitialUpper.Year<1880) {
				textDateInitialUpper.Text="";
			}
			else {
				textDateInitialUpper.Text=ClaimCur.CanadianDateInitialUpper.ToShortDateString();
			}
			comboMaxProsthMaterial.SelectedIndex=ClaimCur.CanadianMaxProsthMaterial;
			//mand prosth-----------------------------------------------------------------------------------------------------
			switch(ClaimCur.CanadianIsInitialLower) {
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
			if(ClaimCur.CanadianDateInitialLower.Year<1880) {
				textDateInitialLower.Text="";
			}
			else {
				textDateInitialLower.Text=ClaimCur.CanadianDateInitialLower.ToShortDateString();
			}
			comboMandProsthMaterial.SelectedIndex=ClaimCur.CanadianMandProsthMaterial;
			//Missing and Extracted Teeth--------------------------------------------------------------------------------------
			SetCanadianExtractedTeeth();
			List<string> al=ToothInitials.GetMissingOrHiddenTeeth(_loadData.ListToothInitials);
			string missingstr="";
			for(int i=0;i<al.Count;i++){
				if(i>0){
					missingstr+=", ";
				}
				missingstr+=Tooth.ToInternat(al[i]);
			}
			textMissingTeeth.Text=missingstr;
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

		private void SetCanadianExtractedTeeth() {
			listExtractedTeeth.Items.Clear();
			if(comboMaxProsth.SelectedIndex==1 || comboMandProsth.SelectedIndex==1){
				List<Procedure> extractedList=Procedures.GetCanadianExtractedTeeth(ProcList);				
				for(int i=0;i<extractedList.Count;i++) {
					listExtractedTeeth.Items.Add(Tooth.ToInternat(extractedList[i].ToothNum)+"\t"+extractedList[i].ProcDate.ToShortDateString());
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
			if(PatPlans.GetCountForPatAndInsSub(ClaimCur.InsSubNum,ClaimCur.PatNum)==0) {//If the plan has been dropped
				//Don't let the user recalculate estimates.  Our estimate calculation code would zero all claimproc insurance estimate amounts.
				MsgBox.Show(this,"You cannot recalculate estimates for an insurance plan that has been dropped.");
				return;
			}
			if(!ClaimIsValid()){
				return;
			}
			List <Benefit> benefitList=Benefits.Refresh(PatPlanList,SubList);
			Claims.CalculateAndUpdate(ProcList,PlanList,ClaimCur,PatPlanList,benefitList,PatCur,SubList);
			ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
			FillGrids();
			if(!_recalcErrorProvider.GetError(this.butRecalc).Equals(String.Empty)) {//ClaimProcedures values have been modified.
				_recalcErrorProvider.SetError(this.butRecalc,String.Empty);//Above logic results in no longer have missmatching estimates and totals.
			}
		}

		private void FillGrids(bool doRefreshData=true){
			//must run claimprocs.refresh separately beforehand
			//also recalculates totals because user might have changed certain items.
			ClaimCur.ClaimFee=0;
			ClaimCur.DedApplied=0;
			ClaimCur.InsPayEst=0;
			ClaimCur.InsPayAmt=0;
			ClaimCur.WriteOff=0;
			gridProc.BeginUpdate();
			gridProc.ListGridColumns.Clear();
			gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","#"),25));
			gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","Date"),66));
			gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","Prov"),62));
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","Code"),75));
			}
			else {
				gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","Code"),50));
				gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","Tth"),25));
			}
			gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","Description"),130));
			gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","Fee"),62,HorizontalAlignment.Right));
			gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","Billed to Ins"),75,HorizontalAlignment.Right));
			gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","Deduct"),62,HorizontalAlignment.Right));
			gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","Ins Est"),62,HorizontalAlignment.Right));
			gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","Ins Pay"),62,HorizontalAlignment.Right));
			gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","WriteOff"),62,HorizontalAlignment.Right));
			if(_loadData.DoShowPatResp) {
				gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","Pat Resp"),62,HorizontalAlignment.Right));
			}
			gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","Status"),50,HorizontalAlignment.Center));
			gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","Pmt"),30,HorizontalAlignment.Center));
			if(PrefC.GetYN(PrefName.ClaimEditShowPayTracking)) {
				gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","Pay Tracking"),90,HorizontalAlignment.Center));
			}
			gridProc.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimProc","Remarks"),60){ IsWidthDynamic=true });
			gridProc.ListGridRows.Clear();
			GridRow row;
			_listClaimProcsForClaim=ClaimProcs.RefreshForClaim(ClaimCur.ClaimNum,ProcList,ClaimProcList);//Excludes labs.
			//ClaimProcs should always be ordered by their LineNumber (most important).
			//However, there can be supplemental transfers mixed into the list of ClaimProcs for this claim which need to show up next to what they offset.
			List<ClaimProc> listTxfrClaimProcs=_listClaimProcsForClaim.FindAll(x => x.IsTransfer);
			if(listTxfrClaimProcs.Count > 0) {
				_listClaimProcsForClaim.RemoveAll(x => x.IsTransfer);
				foreach(ClaimProc txfrClaimProc in listTxfrClaimProcs) {
					//If this is a supplemental transfer that is not associated to a procedure, just throw it at the top of the list (no ItemOrder).
					if(txfrClaimProc.ProcNum==0) {
						_listClaimProcsForClaim.Insert(0,txfrClaimProc);
						continue;
					}
					//Try and find a corresponding ClaimProc with the same ProcNum.
					int index=_listClaimProcsForClaim.FindIndex(x => x.ProcNum==txfrClaimProc.ProcNum);
					//Default to inserting into the beginning of the list if a match was not found.
					_listClaimProcsForClaim.Insert((index==-1 ? 0 : index+1),txfrClaimProc);
				}
			}
			List<ClaimProc> listClaimProcIntermingle=new List<ClaimProc>();//This list will contain procs and their labs, such that attached labs follow their parent proc.
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				//List of parent ProcNums for this claim.
				List<long> listClaimProcNums=_listClaimProcsForClaim.Select(x => x.ProcNum).ToList();
				//List of child procedures associated to parent procedures belonging to this claim.
				List<Procedure> listLabProcs=ProcList.FindAll(x => x.ProcNumLab!=0 && listClaimProcNums.Contains(x.ProcNumLab));
				_dictCanadianLabClaimProcs=new Dictionary<long,List<ClaimProc>>();
				foreach(Procedure proc in listLabProcs) {
					if(_dictCanadianLabClaimProcs.ContainsKey(proc.ProcNumLab)) {
						_dictCanadianLabClaimProcs[proc.ProcNumLab].AddRange(ClaimProcList.FindAll(x => x.ProcNum==proc.ProcNum && x.ClaimNum==ClaimCur.ClaimNum));
						continue;
					}
					_dictCanadianLabClaimProcs.Add(proc.ProcNumLab,ClaimProcList.FindAll(x => x.ProcNum==proc.ProcNum && x.ClaimNum==ClaimCur.ClaimNum));
				}
			}
			for(int i=0;i<_listClaimProcsForClaim.Count;i++){
				row=new GridRow();
				listClaimProcIntermingle.Add(_listClaimProcsForClaim[i]);
				ClaimProcRowHelper(row,_listClaimProcsForClaim[i]);
				gridProc.ListGridRows.Add(row);
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && _dictCanadianLabClaimProcs.ContainsKey(_listClaimProcsForClaim[i].ProcNum)) {//This proc has one or more labs attached.
					foreach(ClaimProc labClaimProc in _dictCanadianLabClaimProcs[_listClaimProcsForClaim[i].ProcNum]) {//Add one row for each attached proc, just below the parent proc.
						//The following 'continue' logic does not skip adding lab rows to gridProc.
						//Instead it ensures that recieved or unrecieved parent claimProcs show above their associated recieved or unrecieved lab claimProcs.
						//Needed for supplemental payments. When there are supplemental payments we want the associated supplemental lab payment to show under the supplemental parent claimProc, not recieved.
						if(_listClaimProcsForClaim[i].Status!=labClaimProc.Status//Needed for supplemental payments. Original estimates will show together and all supplementals will show together.
							|| (_listClaimProcsForClaim[i].Status==ClaimProcStatus.Supplemental && _listClaimProcsForClaim[i].DateCP!=labClaimProc.DateCP))//Needed when there are multiple supplemental payments.
						{
							//Edge Case: If DateEntry is the same for multiple supplemental payments then the rows will duplicate under each parent supplemental row.
							//This will cause issues with this logic but is an edge case and we will handle it if it ever happens to a customer.
							continue;
						}
						GridRow labRow=new GridRow();
						listClaimProcIntermingle.Add(labClaimProc);
						ClaimProcRowHelper(labRow,labClaimProc);
						labRow.Tag=labClaimProc;
						gridProc.ListGridRows.Add(labRow);
					}
				}
				row.Tag=_listClaimProcsForClaim[i];
			}
			gridProc.EndUpdate();
			if(_listClaimProcsForClaim.Any(x => ListTools.In(x.Status,ClaimProcStatus.Estimate
				,ClaimProcStatus.CapComplete,ClaimProcStatus.CapEstimate,ClaimProcStatus.Adjustment,ClaimProcStatus.InsHist)))
			{
				MsgBox.Show(this,"One or more claim procedures have an invalid status.");
			}
			_listClaimProcsForClaim=listClaimProcIntermingle;//Now _listClaimProcsForClaim matches the grid order.
			if(ClaimCur.ClaimType=="Cap"){
				//zero out ins info if Cap.  This keeps it from affecting the balance.  It could be slightly improved later if there is enough demand to show the inspayamt in the Account module.
				ClaimCur.InsPayEst=0;
				ClaimCur.InsPayAmt=0;
			}
			textClaimFee.Text=ClaimCur.ClaimFee.ToString("F");
			textDedApplied.Text=ClaimCur.DedApplied.ToString("F");
			textInsPayEst.Text=ClaimCur.InsPayEst.ToString("F");
			textInsPayAmt.Text=ClaimCur.InsPayAmt.ToString("F");
			textWriteOff.Text=ClaimCur.WriteOff.ToString("F");
			if(_loadData.DoShowPatResp) {
				double procFeeTotal=gridProc.GetTags<ClaimProc>()
					.Where(x => x.Status!=ClaimProcStatus.Supplemental)
					.Select(x => Procedures.GetProcFromList(ProcList,x.ProcNum))
					.Sum(x => x.ProcFeeTotal);
				textPatResp.Text=(procFeeTotal-ClaimCur.WriteOff-ClaimCur.InsPayAmt).ToString("F");
			}
			//payments
			gridPay.BeginUpdate();
			gridPay.ListGridColumns.Clear();
			gridPay.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimPay","Date"),70));
			gridPay.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimPay","Type"),60));
			gridPay.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimPay","Amount"),80,HorizontalAlignment.Right));
			gridPay.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimPay","Check Num"),90));
			gridPay.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimPay","Bank/Branch"),100));
			gridPay.ListGridColumns.Add(new GridColumn(Lan.g("TableClaimPay","Note"),180));
			gridPay.ListGridRows.Clear();
			if(doRefreshData) {
				tablePayments=ClaimPayments.GetForClaim(ClaimCur.ClaimNum);
			}
			for(int i=0;i<tablePayments.Rows.Count;i++){
				row=new GridRow();
				row.Cells.Add(tablePayments.Rows[i]["checkDate"].ToString());
				row.Cells.Add(tablePayments.Rows[i]["payType"].ToString());
				row.Cells.Add(tablePayments.Rows[i]["amount"].ToString());
				row.Cells.Add(tablePayments.Rows[i]["CheckNum"].ToString());
				row.Cells.Add(tablePayments.Rows[i]["BankBranch"].ToString());
				row.Cells.Add(tablePayments.Rows[i]["Note"].ToString());
				gridPay.ListGridRows.Add(row);
			}
			gridPay.EndUpdate();
			FillStatusHistory(doRefreshData);
		}

		private void fillGridSentAttachments() {
			gridSent.BeginUpdate();
			gridSent.ListGridColumns.Clear();
			gridSent.ListGridRows.Clear();
			GridColumn col;
			col=new GridColumn("File",200);
			gridSent.ListGridColumns.Add(col);
			GridRow row;
			for(int i=0;i<ClaimCur.Attachments.Count;i++) {
				row=new GridRow();
				row.Cells.Add(ClaimCur.Attachments[i].DisplayedFileName);
				row.Tag=ClaimCur.Attachments[i];
				gridSent.ListGridRows.Add(row);
			}
			gridSent.EndUpdate();
		}

		private void gridSent_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			ClaimAttach claimAttachCur=(ClaimAttach)gridSent.ListGridRows[e.Row].Tag;
			string patFolder=ImageStore.GetPatientFolder(PatCur,ImageStore.GetPreferredAtoZpath());
			if(CloudStorage.IsCloudStorage) {
				string pathAndFileName=ODFileUtils.CombinePaths(patFolder,claimAttachCur.ActualFileName,'/');
				if(!CloudStorage.FileExists(pathAndFileName)) {
					//Couldn't find file, display message and return
					MsgBox.Show(this,"File no longer exists.");
					return;
				}
				//found it, download and display
				//This chunk of code was pulled from FormFilePicker.cs
				using FormProgress FormP=new FormProgress();
				FormP.DisplayText="Downloading...";
				FormP.NumberFormat="F";
				FormP.NumberMultiplication=1;
				FormP.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
				FormP.TickMS=1000;
				TaskStateDownload state=CloudStorage.DownloadAsync(patFolder,claimAttachCur.ActualFileName,
					new OpenDentalCloud.ProgressHandler(FormP.UpdateProgress));
				if(FormP.ShowDialog()==DialogResult.Cancel) {
					state.DoCancel=true;
					return;
				}
				string tempFile=PrefC.GetRandomTempFile(Path.GetExtension(pathAndFileName));
				File.WriteAllBytes(tempFile,state.FileContent);
				if(ODBuild.IsWeb()) {
					ThinfinityUtils.HandleFile(tempFile);
				}
				else {
					Process.Start(tempFile);
				}
			}
			else {//Local storage
				string pathAndFileName=ODFileUtils.CombinePaths(patFolder,claimAttachCur.ActualFileName);
				try {
					Process.Start(pathAndFileName);
				}
				catch(Exception ex) {
					ex.DoNothing();
					MsgBox.Show(this,"Could not open the attachment.");
				}
			}
		}

		private void ClaimProcRowHelper(GridRow row,ClaimProc claimProcCur) {
			if(claimProcCur.LineNumber==0){
				row.Cells.Add("");
			}
			else{
				row.Cells.Add(claimProcCur.LineNumber.ToString());
			}
			string date=claimProcCur.ProcDate.ToShortDateString();
			if(claimProcCur.ProcNum==0) {//Total payment
				//We want to always show the "Payment Date" instead of the procedure date for total payments because they are not associated to procedures.
				date=claimProcCur.DateCP.ToShortDateString();
			}
			//Check if there was insurance payment amount entered but no claim payment (insurance check / not finalized). 
			if(claimProcCur.InsPayAmt>0 && claimProcCur.ClaimPaymentNum==0) {
				//Many users will just enter payment via clicking 'By Procedure' or 'Total' which does not "finalize" claim payments.
				//Instead of showing a "payment date" for insurance payments, we are going to make it visually obvious that the payments have not been finalized yet.
				date=Lan.g(this,"Not Final");//"Not Finalized" is too long.
			}
			row.Cells.Add(date);
			row.Cells.Add(Providers.GetAbbr(claimProcCur.ProvNum));
			double procFeeTotal=0;
			if(claimProcCur.ProcNum==0) {
				row.Cells.Add("");//code
				if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
					row.Cells.Add("");//tooth
				}
				if(claimProcCur.Status==ClaimProcStatus.NotReceived)
					row.Cells.Add(Lan.g(this,"Estimate"));
				else
					row.Cells.Add(Lan.g(this,"Total Payment"));
			}
			else {
				//claimProcsForClaim list already handles ProcNum=0 above
				Procedure procCur=Procedures.GetProcFromList(ProcList,claimProcCur.ProcNum);
				procFeeTotal=procCur.ProcFeeTotal;
				row.Cells.Add(claimProcCur.CodeSent);
				if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
					row.Cells.Add(Tooth.ToInternat(procCur.ToothNum));
				}
				ProcedureCode procCodeCur=ProcedureCodes.GetProcCode(procCur.CodeNum);
				ProcedureCode procCodeSent=ProcedureCodes.GetProcCode(claimProcCur.CodeSent);
				InsPlan planCur=PlanList.Where(x => x.PlanNum == claimProcCur.PlanNum).FirstOrDefault();
				string descript=Procedures.GetClaimDescript(claimProcCur,procCodeSent,procCur,procCodeCur,planCur);
				if(procCodeSent.IsCanadianLab) {
					descript="^ ^ "+descript;
				}
				row.Cells.Add(descript);
			}
			row.Cells.Add(procFeeTotal.ToString("F"));
			row.Cells.Add(claimProcCur.FeeBilled.ToString("F"));
			row.Cells.Add(claimProcCur.DedApplied.ToString("F"));
			row.Cells.Add(claimProcCur.InsPayEst.ToString("F"));
			row.Cells.Add(claimProcCur.InsPayAmt.ToString("F"));
			row.Cells.Add(claimProcCur.WriteOff.ToString("F"));
			if(_loadData.DoShowPatResp) {
				if(claimProcCur.Status==ClaimProcStatus.Supplemental) {
					procFeeTotal=0;
				}
				row.Cells.Add((procFeeTotal-claimProcCur.WriteOff-claimProcCur.InsPayAmt).ToString("F"));
			}
			switch(claimProcCur.Status){
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
					if(claimProcCur.IsTransfer) {
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
			if(claimProcCur.ClaimPaymentNum>0){
				row.Cells.Add("X");
			}
			else{
				row.Cells.Add("");
			}
			if(PrefC.GetYN(PrefName.ClaimEditShowPayTracking)) {
				if(claimProcCur.ClaimPaymentTracking!=0) {
					row.Cells.Add(Defs.GetDef(DefCat.ClaimPaymentTracking,claimProcCur.ClaimPaymentTracking).ItemName);//EOB Code
				}
				else {
					row.Cells.Add("");
				}
			}
			row.Cells.Add(claimProcCur.Remarks);
			ClaimCur.ClaimFee+=claimProcCur.FeeBilled;
			ClaimCur.DedApplied+=claimProcCur.DedApplied;
			ClaimCur.InsPayEst+=claimProcCur.InsPayEst;
			ClaimCur.InsPayAmt+=claimProcCur.InsPayAmt;
			ClaimCur.WriteOff+=claimProcCur.WriteOff;
		}

		private void FillStatusHistory(bool doRefreshStatusEntries=true) {
			////status history
			gridStatusHistory.BeginUpdate();
			gridStatusHistory.ListGridColumns.Clear();
			gridStatusHistory.ListGridColumns.Add(new GridColumn(Lan.g("TableStatusHistory","Date"),120));
			gridStatusHistory.ListGridColumns.Add(new GridColumn(Lan.g("TableStatusHistory","Description"),270));
			gridStatusHistory.ListGridColumns.Add(new GridColumn(Lan.g("TableStatusHistory","Log Note"),320));
			gridStatusHistory.ListGridColumns.Add(new GridColumn(Lan.g("TableStatusHistory","ErrorCode"),90));
			gridStatusHistory.ListGridColumns.Add(new GridColumn(Lan.g("TableStatusHistory","User"),90));
			gridStatusHistory.ListGridRows.Clear();
			List<ClaimTracking> listCustomStatusEntries;
			if(doRefreshStatusEntries) {
				listCustomStatusEntries=ClaimTrackings.RefreshForClaim(ClaimTrackingType.StatusHistory,ClaimCur.ClaimNum);
			}
			else {
				listCustomStatusEntries=_loadData.ListCustomStatusEntries;
			}
			GridRow row;
			foreach(ClaimTracking claimTrackingEntry in listCustomStatusEntries.OrderByDescending(x => x.DateTimeEntry)) {
				row=new GridRow();
				row.Cells.Add(claimTrackingEntry.DateTimeEntry.ToShortDateString()+" "+claimTrackingEntry.DateTimeEntry.ToShortTimeString());
				String defValue=Defs.GetName(DefCat.ClaimCustomTracking,claimTrackingEntry.TrackingDefNum);//get definition Name
				row.Cells.Add(defValue);
				row.Cells.Add(claimTrackingEntry.Note);
				row.Cells.Add(Defs.GetName(DefCat.ClaimErrorCode,claimTrackingEntry.TrackingErrorDefNum));
				row.Cells.Add(Userods.GetName(claimTrackingEntry.UserNum));
				gridStatusHistory.ListGridRows.Add(row);
				row.Tag=claimTrackingEntry;
			}
			gridStatusHistory.EndUpdate();
		}

		private void gridProc_CellClick(object sender,ODGridClickEventArgs e) {
			if(ListTools.In(ClaimCur.ClaimStatus,ClaimStatus.Sent.GetDescription(true),ClaimStatus.Received.GetDescription(true))) {//sent or received
				if((ClaimCur.ClaimType!="PreAuth" && !Security.IsAuthorized(Permissions.ClaimSentEdit,ClaimCur.DateSent,true))
					|| (ClaimCur.ClaimType=="PreAuth" && !Security.IsAuthorized(Permissions.PreAuthSentEdit,ClaimCur.DateSent,true))) 
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
			foreach(int index in gridProc.SelectedIndices) {
				ClaimProc claimProc=_listClaimProcsForClaim[index];
				if(claimProc.ProcNum==0) {
					continue;//current index is associated to a 'Total Payment' row.
				}
				Procedure proc=ProcList.FirstOrDefault(x => x.ProcNum==claimProc.ProcNum);
				if(proc.ProcNumLab==0 && _dictCanadianLabClaimProcs.ContainsKey(proc.ProcNum)) {//This proc has one or two labs attached.
					for(int i=1;i<=GetListLabsForProc(proc.ProcNum,claimProc).Count;i++) {
						gridProc.SetSelected(index+i,true);//Labs are grouped with their parent procedure. See FillGrids().
					}
					return;
				}
				else if(proc.ProcNumLab!=0) {//This is a lab procedure.
					gridProc.SetSelected(index-1,true);//Guaranteed the row above is either a parent or another lab.  See FillGrids().
					if(GetListLabsForProc(proc.ProcNumLab,claimProc).Count>1) {//User already clicked the 1 lab and it is already selected when 1.
						if(_listClaimProcsForClaim[index-1].ProcNum==proc.ProcNumLab) {//The row above the currently clicked row is the parent proc.
							gridProc.SetSelected(index+1,true);//The second lab is the next row, since the previous row is the parent and the current row is the first lab.
						}
						else {//The above row is the first lab and the current row is the second lab.
							gridProc.SetSelected(index-2,true);//Select the parent proc, since we already selected the first lab proc above.
						}
					}
				}
			}
		}

		///<summary>Takes in the appropriate ProcNum and current claimproc to get the number of claimprocs associated with that procedure</summary>
		private List<ClaimProc> GetListLabsForProc(long procNumParent,ClaimProc claimProc) {
			if(claimProc.Status==ClaimProcStatus.Supplemental) { 
				return _dictCanadianLabClaimProcs[procNumParent].Where(x=>x.Status==ClaimProcStatus.Supplemental && x.DateCP==claimProc.DateCP).ToList();
			}
			return _dictCanadianLabClaimProcs[procNumParent].Where(x=>x.Status==claimProc.Status).ToList();
		}
		
		private void gridProc_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(ListTools.In(ClaimCur.ClaimStatus,ClaimStatus.Sent.GetDescription(true),ClaimStatus.Received.GetDescription(true))) {//sent or received
				if((ClaimCur.ClaimType!="PreAuth" && !Security.IsAuthorized(Permissions.ClaimSentEdit,ClaimCur.DateSent,true))
					|| (ClaimCur.ClaimType=="PreAuth" && !Security.IsAuthorized(Permissions.PreAuthSentEdit,ClaimCur.DateSent,true)))
				{
					return;
				}
			}
			if(!doubleClickWarningAlreadyDisplayed){
				doubleClickWarningAlreadyDisplayed=true;
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"If you are trying to enter payment information, please use the payments buttons at the upper right.\r\nThen, don't forget to finish by creating the check using the button below this section.\r\nYou should probably click cancel unless you are just editing estimates.\r\nContinue anyway?")){
					return;
				}
			}
			ClaimProc claimProcCur=_listClaimProcsForClaim[e.Row];
			if(!CheckRecalcEstimates(claimProcCur)) {
				return;
			}
			ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
			FillGrids();
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
			List<ClaimProcHist> histList=null;
			List<ClaimProcHist> loopList=null;
			using FormClaimProc FormCP=new FormClaimProc(claimProc,null,FamCur,PatCur,PlanList,histList,ref loopList,PatPlanList,true,SubList);
			FormCP.IsInClaim=true;
			FormCP.ShowDialog();
			if(FormCP.DialogResult==DialogResult.Abort) {//Claim was deleted. Close claim edit window.
				DialogResult=DialogResult.Abort;
				return false;
			}
			else if(FormCP.DialogResult!=DialogResult.OK) {
				return false;
			}
			if(claimProc.DoDelete) {
			//If we delete a Canadian claimproc that had a lab procedure, 
			//then _dictCanadianLabClaimProcs will be rebuilt from ClaimProcList in FillGrids().
			//There is no need to worry about removing those claimprocs in memory.
				_listClaimProcsForClaim.Remove(claimProc);
			}
			if(claimProc.Status!=ClaimProcStatus.NotReceived) {
				return true;
			}
			ClaimProc ClaimProcInitial=FormCP.ClaimProcInitial;//Unedited ClaimProc with base estimates calculated in FormClaimProc.
			bool isClaimProcRecalcNeeded=false;
			isClaimProcRecalcNeeded=IsRecalcNeeded(claimProc.DedApplied,claimProc.DedEstOverride,claimProc.DedEst,
				ClaimProcInitial.DedEstOverride,ClaimProcInitial.DedEst);
			isClaimProcRecalcNeeded|=IsRecalcNeeded(claimProc.InsPayEst,claimProc.InsEstTotalOverride,claimProc.InsEstTotal,
				ClaimProcInitial.InsEstTotalOverride,ClaimProcInitial.InsEstTotal);
			isClaimProcRecalcNeeded|=IsRecalcNeeded(claimProc.WriteOff,claimProc.WriteOffEstOverride,claimProc.WriteOffEst,
				ClaimProcInitial.WriteOffEstOverride,ClaimProcInitial.WriteOffEst);
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
			if(e.Row>tablePayments.Rows.Count-1){
				return;//prevents crash after deleting a check?
			}
			for(int i=0;i<_listClaimProcsForClaim.Count;i++){
				if(_listClaimProcsForClaim[i].ClaimPaymentNum.ToString()==tablePayments.Rows[e.Row]["ClaimPaymentNum"].ToString())
					gridProc.SetSelected(i,true);
				else
					gridProc.SetSelected(i,false);
			}
		}

		private void gridPay_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			long tempClaimNum=ClaimCur.ClaimNum;
			ClaimPayment claimPaymentCur=ClaimPayments.GetOne(PIn.Long(tablePayments.Rows[e.Row]["ClaimPaymentNum"].ToString()));
			using FormClaimPayBatch formCPB=new FormClaimPayBatch(claimPaymentCur);
			//FormClaimPayEditOld FormCPE=new FormClaimPayEditOld(claimPaymentCur);
			//Security handled in that form.  Anyone can view.
			formCPB.IsFromClaim=true;//but not IsNew.
			formCPB.ShowDialog();
			//FormCPE.OriginatingClaimNum=ClaimCur.ClaimNum;
			//FormCPE.ShowDialog();
			ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
			FillGrids();
		}

		private void butOtherCovChange_Click(object sender, System.EventArgs e) {
			using FormInsPlanSelect FormIPS=new FormInsPlanSelect(PatCur.PatNum);
			FormIPS.ShowDialog();
			if(FormIPS.DialogResult!=DialogResult.OK){
				return;
			}
			ClaimCur.PlanNum2=FormIPS.SelectedPlan.PlanNum;
			ClaimCur.InsSubNum2=FormIPS.SelectedSub.InsSubNum;
			textPlan2.Text=InsPlans.GetDescript(ClaimCur.PlanNum2,FamCur,PlanList,ClaimCur.InsSubNum2,SubList);
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
			ClaimCur.PlanNum2=0;
			ClaimCur.InsSubNum2=0;
			ClaimCur.PatRelat2=Relat.Self;
			textPlan2.Text="";
			comboPatRelat2.Visible=false;
			label10.Visible=false;
		}

		private void butPayTotal_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPayCreate)){//date not checked here, but it will be checked when actually creating the check
				return;
			}
			//preauths are only allowed "payment" entry by procedure since a total would be meaningless
			if(ClaimCur.ClaimType=="PreAuth"){
				MessageBox.Show(Lan.g(this,"PreAuthorizations can only be entered by procedure."));
				return;
			}
			if(PrefC.GetBool(PrefName.OrthoInsPayConsolidated)) {
				InsPlan planCur = InsPlans.GetPlan(ClaimCur.PlanNum,PlanList);
				long orthoAutoCodeNum = InsPlans.GetOrthoAutoProc(planCur);
				//if all the procedures on this claim are ortho auto procedures...
				if(_listClaimProcsForClaim.All(x => Procedures.GetProcFromList(ProcList,x.ProcNum).CodeNum == orthoAutoCodeNum)) {
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
				comboClaimStatus.SelectedIndex=_listClaimStatus.IndexOf(ClaimStatus.Received);//Received
				if(textDateRec.Text==""){
					textDateRec.Text=DateTime.Today.ToShortDateString();
				}
			}
			ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
			FillGrids();
		}

		private bool PayAsTotalToClaimProcsExplicitly() { 
			List<InputBoxParam> listInputBoxParams=new List<InputBoxParam>();
			listInputBoxParams.Add(new InputBoxParam(InputBoxType.ValidDouble,Lan.g(this,"Please enter an amount: ")));
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
			if(_blueBookEstimateData==null) {
				_blueBookEstimateData=new BlueBookEstimateData(PlanList,SubList,PatPlanList);
			}
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			for(int i=0;i<_listClaimProcsForClaim.Count;i++) {
				listClaimProcs.Add(_listClaimProcsForClaim[i].Copy());
			}
			using FormClaimPayTotal FormCPT=new FormClaimPayTotal(PatCur,FamCur,PlanList,PatPlanList,SubList,_blueBookEstimateData,totalPayAmt:result);
			FormCPT.ClaimProcsToEdit=_listClaimProcsForClaim.ToArray(); //Might want to filter out all things that aren't marked as recieved or supplemental
			FormCPT.ShowDialog();
			if(FormCPT.DialogResult!=DialogResult.OK){
				_listClaimProcsForClaim=listClaimProcs;
				return false;
			} 
			else {
				for(int i=0;i<FormCPT.ClaimProcsToEdit.Length;i++){
					ClaimProcs.Update(FormCPT.ClaimProcsToEdit[i]);
				}
			}
			ClaimProcs.RemoveSupplementalTransfersForClaims(ClaimCur.ClaimNum);
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
			ClaimProc ClaimProcCur=new ClaimProc();
			ClaimProcCur.IsNew=true;
			//ClaimProcs.Cur.ProcNum 
			ClaimProcCur.ClaimNum=ClaimCur.ClaimNum;
			ClaimProcCur.PatNum=ClaimCur.PatNum;
			ClaimProcCur.ProvNum=ClaimCur.ProvTreat;
			//ClaimProcs.Cur.FeeBilled
			//ClaimProcs.Cur.InsPayEst
			ClaimProcCur.DedApplied=dedEst;
			ClaimProcCur.Status=ClaimProcStatus.Received;
			ClaimProcCur.InsPayAmt=payEst;
			//remarks
			//ClaimProcs.Cur.ClaimPaymentNum
			ClaimProcCur.PlanNum=ClaimCur.PlanNum;
			ClaimProcCur.InsSubNum=ClaimCur.InsSubNum;
			ClaimProcCur.DateCP=DateTime.Today;
			ClaimProcCur.ProcDate=ClaimCur.DateService;
			ClaimProcCur.DateEntry=DateTime.Now;//will get set anyway
			ClaimProcCur.ClinicNum=ClaimCur.ClinicNum;
			ClaimProcCur.SecDateEntry=ClaimCur.SecDateEntry;//Manually setting this because FormClaimProc checks this field for security.
			//Automatically set PayPlanNum if there is a payplan with matching PatNum, PlanNum, and InsSubNum that has not been paid in full.
			//By sending in ClaimNum, we ensure that we only get the payplan a claimproc from this claim was already attached to or payplans with no claimprocs attached.
			List<PayPlan> payPlanList=PayPlans.GetValidInsPayPlans(ClaimProcCur.PatNum,ClaimProcCur.PlanNum,ClaimProcCur.InsSubNum,ClaimProcCur.ClaimNum);
			ClaimProcCur.PayPlanNum=0;
			if(payPlanList.Count==1) {
				ClaimProcCur.PayPlanNum=payPlanList[0].PayPlanNum;
			}
			else if(payPlanList.Count>1) {
				//more than one valid PayPlan
				using FormPayPlanSelect FormPPS=new FormPayPlanSelect(payPlanList);
				FormPPS.ShowDialog();
				if(FormPPS.DialogResult==DialogResult.OK) {
					ClaimProcCur.PayPlanNum=FormPPS.SelectedPayPlanNum;
				}
			}
			ClaimProcs.Insert(ClaimProcCur);
			List<ClaimProcHist> loopList=null;
			using FormClaimProc FormCP=new FormClaimProc(ClaimProcCur,null,FamCur,PatCur,PlanList,null,ref loopList,PatPlanList,true,SubList);
			FormCP.IsInClaim=true;
			FormCP.IsCalledFromClaimEdit=true;
			FormCP.ShowDialog();
			if(FormCP.DialogResult==DialogResult.Abort) {//Claim was deleted. Close claim edit window.
				ClaimProcs.Delete(ClaimProcCur);
				DialogResult=DialogResult.Abort;
				return false;
			}
			else if(FormCP.DialogResult!=DialogResult.OK){
				ClaimProcs.Delete(ClaimProcCur);
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
			if(ClaimCur.ClaimType!="PreAuth" && !Security.IsAuthorized(Permissions.InsPayCreate)){//date not checked here, but it will be checked when actually creating the check
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
			bool allAreProcs=true;
			for(int i=0;i<gridProc.SelectedIndices.Length;i++){
				if(_listClaimProcsForClaim[gridProc.SelectedIndices[i]].ProcNum==0)
					allAreProcs=false;
			}
			if(!allAreProcs){
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
			if(PrefC.GetBool(PrefName.OrthoInsPayConsolidated)) {
				List<int> listOrthoAutoGridRows = new List<int>();
				InsPlan planCur = InsPlans.GetPlan(ClaimCur.PlanNum,PlanList);
				long orthoAutoCodeNum = InsPlans.GetOrthoAutoProc(planCur);
				for(int i = 0;i<gridProc.SelectedIndices.Length;i++) {
					//if all the procedures on this claim are ortho auto procedures...
					if(Procedures.GetProcFromList(ProcList,_listClaimProcsForClaim[gridProc.SelectedIndices[i]].ProcNum).CodeNum == orthoAutoCodeNum) {
						listOrthoAutoGridRows.Add(i);
					}
				}
				if(listOrthoAutoGridRows.Count>0) {
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"You have selected an ortho auto procedure with consolidated insurance payments turned on. "
						+"Payments for this procedure must be made to the claim with the original banding procedure. Would you like to go to that claim now?"))
					{
						return;
					}
					//get the most recent claim marked as ortho with an ortho banding proc on it.
					Claim claimBanding =Claims.GetOrthoBandingClaim(PatCur.PatNum,planCur.PlanNum);
					if(claimBanding==null) {
						MsgBox.Show(this,"The claim with the original banding procedure was not marked as received or could not be found."
							+"\r\nPlease manually add the payment to that claim.");
					}
					else {//Show the claim edit window and have the insurance payment window automatically show.
						if(!Security.IsAuthorized(Permissions.ClaimView)) {//this should ever get hit if this permission is not granted, but just in case
							return;
						}
						using FormClaimEdit formClaimEdit=new FormClaimEdit(claimBanding,PatCur,FamCur,true);
						formClaimEdit.ShowDialog();
					}
					//Zero out ortho auto claimprocs on this claim regardless of what happened above.
					//We will simply assume they received the payment on the other claim correctly.
					foreach(int gridRowNum in listOrthoAutoGridRows) {
						_listClaimProcsForClaim[gridRowNum].Status=ClaimProcStatus.Received;
						_listClaimProcsForClaim[gridRowNum].DateEntry=DateTime.Now;//date is was set rec'd
						_listClaimProcsForClaim[gridRowNum].InsPayAmt=0;
						ClaimProcs.Update(_listClaimProcsForClaim[gridRowNum]);
						//Deselect any ortho auto claimprocs so that the regular "payment" code below doesn't act on ortho auto procs.
						gridProc.SetSelected(gridRowNum,false);
					}
					ClaimProcs.RemoveSupplementalTransfersForClaims(ClaimCur.ClaimNum);
				}
			}
			#endregion
			List<ClaimProc> cpList=new List<ClaimProc>();
			for(int i=0;i<gridProc.SelectedIndices.Length;i++) {
				//copy selected claimprocs to temporary array for editing.
				//no changes to the database will be made within that form.
				cpList.Add(_listClaimProcsForClaim[gridProc.SelectedIndices[i]].Copy());
				if(ClaimCur.ClaimType=="PreAuth"){
					cpList[i].Status=ClaimProcStatus.Preauth;
				}
				else if(ClaimCur.ClaimType=="Cap"){
					;//do nothing.  The claimprocstatus will remain Capitation.
				}
				else{
					cpList[i].Status=ClaimProcStatus.Received;
					cpList[i].DateEntry=DateTime.Now;//date is was set rec'd
					cpList[i].InsPayAmt=cpList[i].InsPayEst;
					cpList[i].PayPlanNum=0;
					if(i==0) {
						//Automatically set PayPlanNum if there is a payplan with matching PatNum, PlanNum, and InsSubNum that has not been paid in full.
						//By sending in ClaimNum, we ensure that we only get the payplan a claimproc from this claim was already attached to or payplans with no claimprocs attached.
						List<PayPlan> payPlanList=PayPlans.GetValidInsPayPlans(cpList[i].PatNum,cpList[i].PlanNum,cpList[i].InsSubNum,cpList[i].ClaimNum);
						if(payPlanList.Count==1) {
							cpList[i].PayPlanNum=payPlanList[0].PayPlanNum;
						}
						else if(payPlanList.Count>1) {
							//more than one valid PayPlan
							using FormPayPlanSelect FormPPS=new FormPayPlanSelect(payPlanList);
							FormPPS.ShowDialog();
							if(FormPPS.DialogResult==DialogResult.OK) {
								cpList[i].PayPlanNum=FormPPS.SelectedPayPlanNum;
							}
						}
					}
					else {
						cpList[i].PayPlanNum=cpList[0].PayPlanNum;//set all procs to the same payplan, they can change it later if not correct for each claimproc that is different
					}
				}
				cpList[i].DateCP=DateTime.Today;
			}
			if(cpList.Count > 0) {
				if(ClaimCur.ClaimType=="PreAuth") {
					using FormClaimPayPreAuth FormCPP=new FormClaimPayPreAuth(PatCur,FamCur,PlanList,PatPlanList,SubList);
					FormCPP.ClaimProcsToEdit=cpList;
					FormCPP.ShowDialog();
					if(FormCPP.DialogResult!=DialogResult.OK) {
						return;
					}
					//save changes now
					for(int i=0;i<FormCPP.ClaimProcsToEdit.Count;i++) {
						ClaimProcs.Update(FormCPP.ClaimProcsToEdit[i]);
						ClaimProcs.SetInsEstTotalOverride(FormCPP.ClaimProcsToEdit[i].ProcNum,FormCPP.ClaimProcsToEdit[i].PlanNum,
							FormCPP.ClaimProcsToEdit[i].InsSubNum,FormCPP.ClaimProcsToEdit[i].InsPayEst,ClaimProcList);
					}
				}
				else {
					if(_blueBookEstimateData==null) {
						_blueBookEstimateData=new BlueBookEstimateData(PlanList,SubList,PatPlanList);
					}
					using FormClaimPayTotal FormCPT=new FormClaimPayTotal(PatCur,FamCur,PlanList,PatPlanList,SubList,_blueBookEstimateData);
					FormCPT.IsCalledFromClaimEdit=true;
					FormCPT.ClaimProcsToEdit=cpList.ToArray();
					FormCPT.ShowDialog();
					if(FormCPT.DialogResult!=DialogResult.OK){
						return;
					}
					//save changes now
					for(int i=0;i<FormCPT.ClaimProcsToEdit.Length;i++){
						ClaimProcs.Update(FormCPT.ClaimProcsToEdit[i]);
					}
				}
			}
			_isPaymentEntered=true;
			comboClaimStatus.SelectedIndex=_listClaimStatus.IndexOf(ClaimStatus.Received);//Received
			if(textDateRec.Text==""){
				textDateRec.Text=DateTime.Today.ToShortDateString();
			}
			ClaimProcs.RemoveSupplementalTransfersForClaims(ClaimCur.ClaimNum);
			ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
			FillGrids();
		}

		private void butPaySupp_Click(object sender, System.EventArgs e) {
			if(PrefC.GetBool(PrefName.OrthoInsPayConsolidated)) {
				InsPlan planCur=InsPlans.GetPlan(ClaimCur.PlanNum,PlanList);
				long orthoAutoCodeNum=InsPlans.GetOrthoAutoProc(planCur);
				//if all the procedures on this claim are ortho auto procedures...
				if(_listClaimProcsForClaim.All(x => Procedures.GetProcFromList(ProcList,x.ProcNum).CodeNum == orthoAutoCodeNum)) {
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
			Procedure procForAdj=new Procedure();
			procForAdj=Procedures.GetOneProc(procNum,false);
			if(procForAdj.ProcStatus!=ProcStat.C) {
				MsgBox.Show(this,"Adjustments may only be added to completed procedures.");
				return;
			}
			Adjustment adj=new Adjustment();
			adj.PatNum=PatCur.PatNum;
			adj.ProvNum=procForAdj.ProvNum;
			adj.DateEntry=DateTime.Today;//but will get overwritten to server date
			adj.AdjDate=DateTime.Today;
			adj.ProcDate=procForAdj.ProcDate;
			adj.ProcNum=procForAdj.ProcNum;
			adj.ClinicNum=procForAdj.ClinicNum;
			using FormAdjust FormA=new FormAdjust(PatCur,adj);
			FormA.IsNew=true;
			FormA.ShowDialog();
		}

		private void MakeSuppPayment() {
			if(!Security.IsAuthorized(Permissions.InsPayCreate)){//date not checked here, but it will be checked when actually creating the check
				return;
			}
			if(gridProc.SelectedIndices.Length==0){
				MessageBox.Show(Lan.g(this,"This is only for additional payments on procedures already marked received.  Please highlight procedures first."));
				return;
			}
			bool allAreRecd=true;
			bool clickedTotalPayment=false;
			for(int i=0;i<gridProc.SelectedIndices.Length;i++){
				if(_listClaimProcsForClaim[gridProc.SelectedIndices[i]].Status!=ClaimProcStatus.Received) {
					allAreRecd=false;
				}
				if(_listClaimProcsForClaim[gridProc.SelectedIndices[i]].ProcNum==0) { //total payment
					clickedTotalPayment=true;
				}
			}
			if(!allAreRecd){
				MessageBox.Show(Lan.g(this,"All selected procedures must be status received."));
				return;
			}
			if(clickedTotalPayment) {
				MessageBox.Show(Lan.g(this,"Select the procedures that you want to enter a supplemental payment for.  If you want to make a supplemental "
					+"payment on a payment previously entered as total, click 'As Total' again."));
				return;
			}
			List<ClaimProc> listSelectedClaimProcs=gridProc.SelectedIndices.Select(x => _listClaimProcsForClaim[x]).ToList();
			List<ClaimProc> listSuppClaimProcs=ClaimProcs.CreateSuppClaimProcs(listSelectedClaimProcs);
			if(_blueBookEstimateData==null) {
				_blueBookEstimateData=new BlueBookEstimateData(PlanList,SubList,PatPlanList);
			}
			using FormClaimPayTotal FormCPT=new FormClaimPayTotal(PatCur,FamCur,PlanList,PatPlanList,SubList,_blueBookEstimateData);
			FormCPT.ClaimProcsToEdit=listSuppClaimProcs.ToArray();
			FormCPT.ShowDialog();
			if(FormCPT.DialogResult!=DialogResult.OK) {
				//ClaimProcs were inserted already so we need to delete them if user clicks 'Cancel'.
				ClaimProcs.DeleteMany(listSuppClaimProcs);
				FillGrids();
				return;
			}
			_isPaymentEntered=true;
			//save changes now
			for(int i=0;i<FormCPT.ClaimProcsToEdit.Length;i++) {
				ClaimProcs.Update(FormCPT.ClaimProcsToEdit[i]);
			}
//fix: need to debug the recalculation feature to take this status into account.
			ClaimProcs.RemoveSupplementalTransfersForClaims(ClaimCur.ClaimNum);
			ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
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
			foreach(ClaimProc claimProc in _listClaimProcsForClaim) {
				if(!listSelectedProcNums.Contains(claimProc.ProcNum)) {
					hasProcLeft=true;
					break;
				}
			}
			if(!hasProcLeft) {//All procedures are selected for the split...
				MsgBox.Show(this,"At least one procedure needs to remain on this claim in order to split it.");
				return;
			}
			//Selection validation logic ensures these are only procs that are eligible to split from this claim.
			Claims.InsertSplitClaim(ClaimCur,gridProc.SelectedTags<ClaimProc>());
			ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
			FillGrids();
		}

		private void butViewEra_Click(object sender,EventArgs e) {
			EtransL.ViewEra(ClaimCur);
		}
		
		private void butViewEob_Click(object sender,EventArgs e) {
			List<long> listClaimPaymentNums=_listClaimProcsForClaim.FindAll(x => x.ClaimPaymentNum!=0).Select(x => x.ClaimPaymentNum).ToList();
			List<ClaimPayment> listClaimPayments=ClaimPayments.GetClaimPaymentsWithEobAttach(listClaimPaymentNums);
			ClaimPayment claimPaymentCur=null;
			switch(listClaimPayments.Count) {
				case 0:
					MsgBox.Show(this,"There are no EOBs for this claim.");
					return;
				case 1:
					claimPaymentCur=listClaimPayments[0];
					break;
				default:
					if(!TrySelectClaimPayment(listClaimPayments,out claimPaymentCur)) {
						return;
					}
					break;
			}
			//Mimics FormClaimPayBatch.butView_Click(...)
			using FormImages formI=new FormImages();
			formI.ClaimPaymentNum=claimPaymentCur.ClaimPaymentNum;
			formI.ShowDialog();
		}

		///<summary>Called when there are multiple ClaimPayments that have EOBs attached to them and we don't know what one the user would like to view.</summary>
		private bool TrySelectClaimPayment(List<ClaimPayment> listClaimPayments,out ClaimPayment claimPayment) {
			claimPayment=null;
			List<GridColumn> listColumnHeaders=new List<GridColumn>() {
				new GridColumn(Lan.g(this,"Carrier"),80){ IsWidthDynamic=true },
				new GridColumn(Lan.g(this,"Check Date"),70,HorizontalAlignment.Center),
				new GridColumn(Lan.g(this,"Amount"),80,HorizontalAlignment.Right)
			};
			List<GridRow> listRowValues=new List<GridRow>();
			listClaimPayments.ForEach(x => {
				GridRow row=new GridRow(x.CarrierName,x.CheckDate.ToShortDateString(),POut.Double(x.CheckAmt));
				row.Tag=x;
				listRowValues.Add(row);
			});
			string formTitle=Lan.g(this,"EOB Picker");
			string gridTitle=Lan.g(this,"EOBs");
			using FormGridSelection form=new FormGridSelection(listColumnHeaders,listRowValues,formTitle,gridTitle);
			if(form.ShowDialog()!=DialogResult.OK) {
				return false;
			}
			claimPayment=(ClaimPayment)form.ListSelectedTags[0];//DialogResult.OK means a selection was made.
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
			if(!_listClaimProcsForClaim.Any(x => ListTools.In(x.Status,ClaimProcs.GetInsPaidStatuses()))) {
				MessageBox.Show(Lan.g(this,"There are no valid received payments for this claim."));
				return;
			}
			ClaimPayment claimPayment=new ClaimPayment();
			claimPayment.CheckDate=MiscData.GetNowDateTime().Date;//Today's date for easier tracking by the office and to avoid backdating before accounting lock dates.
			claimPayment.IsPartial=true;
			claimPayment.ClinicNum=PatCur.ClinicNum;
			claimPayment.CarrierName=Carriers.GetName(InsPlans.GetPlan(ClaimCur.PlanNum,PlanList).CarrierNum);
			ClaimPayments.Insert(claimPayment);
			long onlyOneClaimNum=0;
			if(isThisClaimOnly) {
				onlyOneClaimNum=ClaimCur.ClaimNum;
			}
			double amt=ClaimProcs.AttachAllOutstandingToPayment(claimPayment.ClaimPaymentNum,PrefC.DateClaimReceivedAfter,onlyOneClaimNum);
			claimPayment.CheckAmt=amt;
			try {
				ClaimPayments.Update(claimPayment);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			FormFinalizePaymentHelper(claimPayment,ClaimCur,PatCur,FamCur,onlyOneClaimNum);
			DialogResult=DialogResult.OK;
		}

		///<summary>Called after finalizing an insurance payment to bring up various forms for the user.
		///If finalizing an individual claim (Edit Claim window only), then onlyOneClaimNum must be the current ClaimNum.</summary>
		public static void FormFinalizePaymentHelper(ClaimPayment claimPayment,Claim claimCur,Patient patCur,Family famCur,long onlyOneClaimNum=0) {
			using FormClaimPayEdit FormCPE=new FormClaimPayEdit(claimPayment);
			FormCPE.IsCreateLogEntry=true;
			FormCPE.IsFinalizePayment=true;
			//FormCPE.IsNew=true;//not new.  Already added.
			FormCPE.ShowDialog();
			if(FormCPE.DialogResult!=DialogResult.OK) {
				try {
					ClaimPayments.Delete(claimPayment);
				}
				catch(ApplicationException ex) {
					MessageBox.Show(ex.Message);
				}
				return;
			}
			using FormClaimPayBatch FormCPB=new FormClaimPayBatch(claimPayment,true);
			FormCPB.IsFromClaim=true;
			FormCPB.IsNew=true;
			FormCPB.ShowDialog();
			if(FormCPB.DialogResult!=DialogResult.OK) {
				//The user attached EOBs to the new claim payment and then clicked cancel. Then the user was asked if they wanted to delete the payment and they chose yes.
				//Since we are deleting the claim payment we must remove the attached EOBs or else ClaimPayments.Delete() will throw an exception.
				List<EobAttach> eobsAttached=EobAttaches.Refresh(claimPayment.ClaimPaymentNum);
				for(int i=0;i<eobsAttached.Count;i++) {
					EobAttaches.Delete(eobsAttached[i].EobAttachNum);
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
			ShowProviderTransferWindow(claimCur,patCur,famCur);
		}

		///<summary>Helper method that shows the payment window if the user has the "Show provider income transfer window after entering insurance payment"
		///preference enabled.  This method should always be called after an insurance payment has been made.</summary>
		public static void ShowProviderTransferWindow(Claim claimCur,Patient patCur, Family famCur) {
			if(!PrefC.GetBool(PrefName.ProviderIncomeTransferShows) || claimCur.ClaimType=="PreAuth") {
				return;
			}
			Payment PaymentCur=new Payment();
			PaymentCur.PayDate=DateTime.Today;
			PaymentCur.PatNum=patCur.PatNum;
			//Explicitly set ClinicNum=0, since a pat's ClinicNum will remain set if the user enabled clinics, assigned patients to clinics, and then
			//disabled clinics because we use the ClinicNum to determine which PayConnect or XCharge/XWeb credentials to use for payments.
			PaymentCur.ClinicNum=0;
			if(PrefC.HasClinicsEnabled) {//if clinics aren't enabled default to 0
				PaymentCur.ClinicNum=patCur.ClinicNum;
			}
			PaymentCur.PayType=0;//txfr
			PaymentCur.DateEntry=DateTime.Today;//So that it will show properly in the new window.
			PaymentCur.PaymentSource=CreditCardSource.None;
			PaymentCur.ProcessStatus=ProcessStat.OfficeProcessed;
			Payments.Insert(PaymentCur);
			using FormPayment Formp=new FormPayment(patCur,famCur,PaymentCur,false);
			Formp.IsNew=true;
			Formp.ShowDialog();
		}

		private void radioProsthN_Click(object sender, System.EventArgs e) {
			ClaimCur.IsProsthesis="N";
		}

		private void radioProsthI_Click(object sender, System.EventArgs e) {
			ClaimCur.IsProsthesis="I";
		}

		private void radioProsthR_Click(object sender, System.EventArgs e) {
			ClaimCur.IsProsthesis="R";
		}

		private void butReferralNone_Click(object sender,EventArgs e) {
			textRefProv.Text="";
			ClaimCur.ReferringProv=0;
			butReferralEdit.Enabled=false;
		}

		private void butReferralSelect_Click(object sender,EventArgs e) {
			using FormReferralSelect FormR=new FormReferralSelect();
			FormR.IsSelectionMode=true;
			FormR.ShowDialog();
			if(FormR.DialogResult!=DialogResult.OK) {
				return;
			}
			ClaimCur.ReferringProv=FormR.SelectedReferral.ReferralNum;
			textRefProv.Text=Referrals.GetNameLF(FormR.SelectedReferral.ReferralNum);
			butReferralEdit.Enabled=true;
		}

		private void butReferralEdit_Click(object sender,EventArgs e) {
			//only enabled if ClaimCur.ReferringProv!=0
			Referral refer=ReferralL.GetReferral(ClaimCur.ReferringProv);
			if(refer==null) {
				textRefProv.Text="";
				ClaimCur.ReferringProv=0;
				butReferralEdit.Enabled=false;
				return;
			}
			using FormReferralEdit FormR=new FormReferralEdit(refer);
			FormR.ShowDialog();
			if(FormR.DialogResult==DialogResult.OK){
				//it's impossible to delete referral from that window.
				Referrals.RefreshCache();
				textRefProv.Text=Referrals.GetNameLF(refer.ReferralNum);
			}
		}

		private void FillAttachments(){
			listAttachments.Items.Clear();
			for(int i=0;i<ClaimCur.Attachments.Count;i++){
				listAttachments.Items.Add(ClaimCur.Attachments[i].DisplayedFileName);
			}
		}

		private void butAttachAdd_Click(object sender,EventArgs e) {
			using FormImageSelectClaimAttach FormI=new FormImageSelectClaimAttach();
			FormI.PatNum=ClaimCur.PatNum;
			FormI.ShowDialog();
			if(FormI.DialogResult!=DialogResult.OK){
				return;
			}
			ClaimCur.Attachments.Add(FormI.ClaimAttachNew);
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
			ContrPerio gridP=new ContrPerio();
			gridP.BackColor = System.Drawing.SystemColors.Window;
			gridP.Size = new System.Drawing.Size(602,665);
			PerioExams.Refresh(PatCur.PatNum);
			PerioMeasures.Refresh(PatCur.PatNum,PerioExams.ListExams);
			gridP.SelectedExam=PerioExams.ListExams.Count-1;
			gridP.LoadData();
			Bitmap bitmap=new Bitmap(602,665);
			Graphics g=Graphics.FromImage(bitmap);
			gridP.DrawChart(g);
			g.Dispose();
			Bitmap bitmapBig=new Bitmap(602,715);
			g=Graphics.FromImage(bitmapBig);
			g.Clear(Color.White);
			string text=PatCur.GetNameFL();
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
			try {
				if(CloudStorage.IsCloudStorage) {
					using FormProgress FormP=new FormProgress();
					FormP.DisplayText="Uploading...";
					FormP.NumberFormat="F";
					FormP.NumberMultiplication=1;
					FormP.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
					FormP.TickMS=1000;
					OpenDentalCloud.Core.TaskStateUpload state=null;
					using(MemoryStream ms=new MemoryStream()) {
						bitmapBig.Save(ms,System.Drawing.Imaging.ImageFormat.Bmp);
						state=CloudStorage.UploadAsync(
							CloudStorage.AtoZPath+"/EmailAttachments"
							,newName
							,ms.ToArray()
							,new OpenDentalCloud.ProgressHandler(FormP.UpdateProgress));
					}
					FormP.ShowDialog();
					if(FormP.DialogResult==DialogResult.Cancel) {
						state.DoCancel=true;
						return;
					}
					//Upload was successful, so continue attaching
				}
				else { 
					bitmapBig.Save(newPath);
				}
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			ClaimAttach claimAttach=new ClaimAttach();
			claimAttach.DisplayedFileName=Lan.g(this,"PerioChart.jpg");
			claimAttach.ActualFileName=newName;
			ClaimCur.Attachments.Add(claimAttach);
			FillAttachments();
		}

		private void butExport_Click(object sender,EventArgs e) {
			if(ODBuild.IsWeb()) {
				for(int i=0;i<ClaimCur.Attachments.Count;i++) {
					string fileName=PatCur.FName+PatCur.LName+PatCur.PatNum+"_"+i+Path.GetExtension(ClaimCur.Attachments[i].ActualFileName);
					string tempPath=ODFileUtils.CombinePaths(Path.GetTempPath(),fileName);
					string currentPath=FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),ClaimCur.Attachments[i].ActualFileName);
					FileAtoZ.Copy(currentPath,tempPath,FileAtoZSourceDestination.AtoZToLocal,"Exporting attachment...");
					ThinfinityUtils.ExportForDownload(tempPath);
				}
				return;
			}
			string exportPath=PrefC.GetString(PrefName.ClaimAttachExportPath);
			if(!Directory.Exists(exportPath)){
				if(MessageBox.Show(Lan.g(this,"The claim export path no longer exists at:")+" "+exportPath+"\r\n"
					+Lan.g(this,"Would you like to create it?"),"", MessageBoxButtons.YesNo)==DialogResult.Yes) 
				{
					try {
						Directory.CreateDirectory(exportPath);
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
			for(int i=0;i<ClaimCur.Attachments.Count;i++){
				string curAttachPath=FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),ClaimCur.Attachments[i].ActualFileName);
				string newFilePath=ODFileUtils.CombinePaths(exportPath,
					PatCur.FName+PatCur.LName+PatCur.PatNum+"_"+i+Path.GetExtension(ClaimCur.Attachments[i].ActualFileName));
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
			FileAtoZ.OpenFile(FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),ClaimCur.Attachments[listAttachments.SelectedIndex].ActualFileName),
				ClaimCur.Attachments[listAttachments.SelectedIndex].DisplayedFileName);
		}

		private void menuItemRename_Click(object sender,EventArgs e) {
			using InputBox input=new InputBox(Lan.g(this,"Filename"));
			input.textResult.Text=ClaimCur.Attachments[listAttachments.SelectedIndex].DisplayedFileName;
			input.ShowDialog();
			if(input.DialogResult!=DialogResult.OK) {
				return;
			}
			ClaimCur.Attachments[listAttachments.SelectedIndex].DisplayedFileName=input.textResult.Text;
			FillAttachments();
		}

		private void menuItemRemove_Click(object sender,EventArgs e) {
			ClaimCur.Attachments.RemoveAt(listAttachments.SelectedIndex);
			FillAttachments();
		}

		private void listAttachments_DoubleClick(object sender,EventArgs e) {
			if(listAttachments.SelectedIndex==-1) {
				return;
			}
			FileAtoZ.OpenFile(FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),ClaimCur.Attachments[listAttachments.SelectedIndex].ActualFileName),
				ClaimCur.Attachments[listAttachments.SelectedIndex].DisplayedFileName);
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
			InsPlan planCur=InsPlans.GetPlan(ClaimCur.PlanNum,PlanList);
			Carrier carrierCur=Carriers.GetCarrier(planCur.CarrierNum);
			LabelSingle.PrintCarrier(carrierCur.CarrierNum);//pd.PrinterSettings.PrinterName);
		}

		private void butPreview_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.ClaimSend)) {
				return;
			}
			if(!ClaimIsValid()) {
				return;
			}			
			SecurityLogs.MakeLogEntry(_claimEditPermission,PatCur.PatNum,"Claim Previewed for "+PatCur.LName+","+PatCur.FName,
				ClaimCur.ClaimNum,ClaimCur.SecDateTEdit);
			UpdateClaim();
			using FormClaimPrint FormCP=new FormClaimPrint();
			FormCP.PatNumCur=ClaimCur.PatNum;
			FormCP.ClaimNumCur=ClaimCur.ClaimNum;
			FormCP.PrintImmediately=false;
			FormCP.ShowDialog();
			if(FormCP.DialogResult==DialogResult.OK) {
				//status will have changed to sent.
				ClaimCur=Claims.GetClaim(ClaimCur.ClaimNum);
			}
			else if(FormCP.DialogResult==DialogResult.Abort) {//if claim has been deleted, close out of current form.
				DialogResult=DialogResult.Cancel;
				return;
			}
			ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
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
			DateTime claimCurSectDateTEdit=ClaimCur.SecDateTEdit;//Preserve the date prior to any claim updates effecting it.
			UpdateClaim();
			FormClaimPrint FormCP=new FormClaimPrint();
			FormCP.PatNumCur=ClaimCur.PatNum;
			FormCP.ClaimNumCur=ClaimCur.ClaimNum;
			if(!FormCP.PrintImmediate(Lan.g(this,"Claim from")+" "+ClaimCur.DateService.ToShortDateString()+" "+Lan.g(this,"printed"),
				PrintSituation.Claim,PatCur.PatNum))
			{
				return;
			}
			if(!notAuthorized) {//if already sent, we want to block users from changing sent date without permission.
				//also changes claimstatus to sent, and date:
				Etranss.SetClaimSentOrPrinted(ClaimCur.ClaimNum,ClaimCur.PatNum,0,EtransType.ClaimPrinted,0,Security.CurUser.UserNum);
			}
			//ClaimCur.ClaimStatus="S";
			//ClaimCur.DateSent=DateTime.Today;
			//Claims.Update(ClaimCur);
			SecurityLogs.MakeLogEntry(Permissions.ClaimSend,ClaimCur.PatNum,Lan.g(this,"Claim printed from Claim Edit window."),
				ClaimCur.ClaimNum,claimCurSectDateTEdit);
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
			List<long> listInvalidProvs=Providers.GetInvalidProvsByTermDate(new List<long> 
				{ comboProvBill.GetSelectedProvNum(),_provNumOrdering,comboProvTreat.GetSelectedProvNum() },PIn.DateT(textDateService.Text));
			if(listInvalidProvs.Count==0) {
				return true;
			}
			string message="";
			if(listInvalidProvs.Contains(comboProvBill.GetSelectedProvNum()) 
				&& listInvalidProvs.Contains(comboProvTreat.GetSelectedProvNum())
				&& listInvalidProvs.Contains(_provNumOrdering)) 
			{//Used for grammar
				message+="Billing Provider, Treating Provider, and Ordering Provider Override";
			}
			else {
				if(listInvalidProvs.Contains(comboProvBill.GetSelectedProvNum())) {
					message+="Billing Provider";
				}
				if(listInvalidProvs.Contains(comboProvTreat.GetSelectedProvNum())) {
					if(message!="") {
						message+=" and ";
					}
					message+="Treating Provider";
				}
				if(listInvalidProvs.Contains(_provNumOrdering)) {
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
			//Do not allow Canadian users to send secondary predeterminations because there is no format that supports such a transaction in ITRANS.		
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && ClaimCur.ClaimType=="PreAuth") {
				for(int i=0;i<PatPlanList.Count;i++) {
					if(PatPlanList[i].InsSubNum==ClaimCur.InsSubNum && PatPlanList[i].Ordinal!=1) {
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
			DateTime claimCurSectDateTEdit=ClaimCur.SecDateTEdit;//Preserve the date prior to any claim updates effecting it.
			UpdateClaim();
			ClaimSendQueueItem[] listQueue=Claims.GetQueueList(ClaimCur.ClaimNum,ClaimCur.ClinicNum,0);
			if(listQueue.IsNullOrEmpty()) {//Happens if the Claim has been deleted.
				MsgBox.Show(this, "Unable to find claim data.  Exit claim window and try again.");
				return;
			}
			if(listQueue[0].NoSendElect==NoSendElectType.NoSendElect) {
				MsgBox.Show(this,"This carrier is marked to not receive e-claims.");
				//Later: we need to let user send anyway, using all 0's for electronic id.
				return;
			}
			if(listQueue[0].NoSendElect==NoSendElectType.NoSendSecondaryElect && listQueue[0].Ordinal!=1) {
				MsgBox.Show(this,"This carrier is marked to only receive primary insurance e-claims.");
				return;
			}
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=ClaimCur.ClinicNum;
			}
			Clearinghouse clearinghouseHq=ClearinghouseL.GetClearinghouseHq(listQueue[0].ClearinghouseNum);
			Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,clinicNum);
			if(ODBuild.IsWeb() && Clearinghouses.IsDisabledForWeb(clearinghouseClin)) {
				MessageBox.Show(Lans.g("Eclaims","This clearinghouse is not available while viewing through the web."));
				return;
			}
			//string warnings;
			//string missingData=
			listQueue[0]=Eclaims.GetMissingData(clearinghouseClin,listQueue[0]);
			if(listQueue[0].MissingData!=""){
				MessageBox.Show(Lan.g(this,"Cannot send claim until missing data is fixed:")+"\r\n"+listQueue[0].MissingData);
				return;
			}
			else if(clearinghouseClin.IsAttachmentSendAllowed && clearinghouseClin.CommBridge==EclaimsCommBridge.ClaimConnect) {
				//No general missing data, but can send attachments electronically.  Check the clearinghouse for missing data.
				try {
					ClaimConnect.ValidateClaimResponse response=ClaimConnect.ValidateClaim(ClaimCur,true);
					if(response.IsAttachmentRequired) {
						if(MsgBox.Show(this,MsgBoxButtons.YesNo,"An attachment is required for this claim. Would you like to open the claim attachment form?")) {
							FormClaimAttachment.Open(ClaimCur);
							DialogResult=DialogResult.OK;
							return;
						}
						else if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to continue sending the claim?")) {
							return;
						}
					}
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
			}
			Cursor=Cursors.WaitCursor;			
			if(clearinghouseHq.Eformat==ElectronicClaimFormat.Canadian) {
				try {
					Canadian.SendClaim(clearinghouseClin,listQueue[0],true,false,FormCCDPrint.PrintCCD,ShowProviderTransferWindow,
						FormClaimPrint.PrintCdaClaimForm);//Ignore the etransNum result. Physically print the form.
				}
				catch(ApplicationException ex){
					//Custom error messages are thrown as ApplicationExceptions in SendClaim().
					//There are probably no other scenarios where an ApplicationException thrown.  If there are, then the user will still get the message, but not the details.  Not a big deal.
					Cursor=Cursors.Default;
					//The message is translated before thrown, so we do not need to translate here.
					//We show the message in a copy/paste window so that our techs and users can quickly copy the message and search for a solution.
					using MsgBoxCopyPaste form=new MsgBoxCopyPaste(ex.Message);
					form.ShowDialog();
					return;
				}
				catch(Exception ex) {
					Cursor=Cursors.Default;
					//The message is translated before thrown, so we do not need to translate here.
					//We show the message in a copy/paste window so that our techs and users can quickly copy the message and search for a solution.
					using MsgBoxCopyPaste form=new MsgBoxCopyPaste(ex.ToString());
					form.ShowDialog();
					return;
				}
			}
			else {
				List<ClaimSendQueueItem> queueItems=new List<ClaimSendQueueItem>();
				queueItems.Add(listQueue[0]);
				EnumClaimMedType medType=listQueue[0].MedType;
				using FormClaimFormItemEdit formClaimFormItemEdit=new FormClaimFormItemEdit();
				Eclaims.SendBatch(clearinghouseClin,queueItems,medType,formClaimFormItemEdit,
					FormClaimPrint.FillRenaissance,new FormTerminalConnection());//this also calls SetClaimSentOrPrinted which creates the etrans entry.
			}
			Cursor=Cursors.Default;
			SecurityLogs.MakeLogEntry(Permissions.ClaimSend,PatCur.PatNum,Lan.g(this,"Claim sent from Claim Edit Window."),ClaimCur.ClaimNum,claimCurSectDateTEdit);
			DialogResult=DialogResult.OK;
		}

		///<summary>Returns true if the claim has ICD9 codes and the user insists on sending the claim with them attached.</summary>
		private bool HasIcd9Codes() {
			List<Procedure> listProcsOnClaim=ProcList.FindAll(x => _listClaimProcsForClaim.Any(y => y.ProcNum==x.ProcNum));			
			if(ICD9s.HasICD9Codes(listProcsOnClaim)) {
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
			using FormClaimResend fcr=new FormClaimResend();
			if(fcr.ShowDialog()==DialogResult.OK) {
				if(fcr.IsClaimReplacement) {
					comboCorrectionType.SelectedIndex=1;//replacement
				}
				else {
					comboCorrectionType.SelectedIndex=0;//original
					textDateSent.Text=DateTime.Now.ToShortDateString();
					ClaimCur.DateResent=DateTime.Now;
					labelDateSent.Text="Date Resent";
				}
				SendClaim();
			}
		}

		private void butHistory_Click(object sender,EventArgs e) {
			List<Etrans> etransList=Etranss.GetHistoryOneClaim(ClaimCur.ClaimNum);
			if(etransList.Count==0) {
				MsgBox.Show(this,"No history found of sent e-claim.");
				return;
			}
			using FormEtransEdit FormE=new FormEtransEdit();
			FormE.EtransCur=etransList[0];
			FormE.ShowDialog();
			ClaimCur=Claims.GetClaim(ClaimCur.ClaimNum);
			ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
			FillForm();
		}

		///<summary>Canada only.</summary>
		private void butReverse_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			InsPlan insPlan=InsPlans.GetPlan(ClaimCur.PlanNum,null);
			InsSub insSub=InsSubs.GetOne(ClaimCur.InsSubNum);
			Carrier carrier=Carriers.GetCarrier(insPlan.CarrierNum);
			Clearinghouse clearinghouseHq=Canadian.GetCanadianClearinghouseHq(carrier);
			Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
			try {
				long etransNumAck=CanadianOutput.SendClaimReversal(clearinghouseClin,ClaimCur,insPlan,insSub,false,FormCCDPrint.PrintCCD);
				Etrans etransAck=Etranss.GetEtrans(etransNumAck);
				if(etransAck.AckCode!="R") {
					//If the claim was successfully reversed, clear the claim transaction reference number so the user can resend the claim if desired.
					//Will make the claim look like it was not ever sent, except in send claims window there will be extra history.
					ClaimCur.CanadaTransRefNum="";
					Claims.Update(ClaimCur);
				}
			}
			catch(Exception ex) {
				// there is a SubString() error coming most likley from CanadianOutput.SendClaimReversal, this will help us track down exactly where.
				FriendlyException.Show(Lan.g(this,"Failed to reverse claim"),ex);
			}
			Cursor=Cursors.Default;
		}

		private void butAdd_Click(object sender,EventArgs e) {
			AddClaimCustomTracking(ClaimCur);
			FillStatusHistory();
		}

		///<summary>Opens FormClaimAttachment. This form is used for managing attachments to send to DentalXChange.</summary>
		private void buttonClaimAttachment_Click(object sender,EventArgs e) {
			if(IsNew) {//Must have a claim object to do claim attachments
				MsgBox.Show(this,"The claim must be saved before you can start adding attachments.");
				return;
			}
			if(comboClaimStatus.SelectedIndex==_listClaimStatus.IndexOf(ClaimStatus.WaitingToSend)) {//Waiting to send claim
				//Check to see if Claim is Valid
				if(!ClaimIsValid()) {
					return;
				}
				//Determine if there is any missing data
				ClaimEdit.UpdateData updateData=UpdateClaim();
				ClaimSendQueueItem[] listQueue=updateData.ListSendQueueItems;
				//There is no benefit to the user by attaching and sending files to DXC if the carrier cannot send an electronic claim within Open Dental.
				//If it is determined later that there is a desire to allow these carriers in, we can simply remove this block.
				if(!listQueue[0].CanSendElect) {
					MsgBox.Show(this,"Carrier is not set to Send Claims Electronically.");
					return;
				}
				Clearinghouse clearinghouseHq=ClearinghouseL.GetClearinghouseHq(listQueue[0].ClearinghouseNum);
				Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
				listQueue[0]=Eclaims.GetMissingData(clearinghouseClin,listQueue[0]);
				if(listQueue[0].MissingData!="") {
					MessageBox.Show("Cannot add attachments until missing data is fixed:"+"\r\n"+listQueue[0].MissingData);
					return;
				}
			}
			if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will close the claim edit window without saving any changes. Continue?")) {
				FormClaimAttachment.Open(ClaimCur);
				DialogResult=DialogResult.Cancel;
			}
		}

		///<summary>Returns true if user inserted a new ClaimTracking and selected an Error Code.</summary>
		public static bool AddClaimCustomTracking(Claim claim,string noteText="") {
			using FormClaimCustomTrackingUpdate FormCCTU=new FormClaimCustomTrackingUpdate(claim,noteText);
			FormCCTU.ShowDialog();
			if(FormCCTU.DialogResult!=DialogResult.OK) {
				return false;
			}
			return ((FormCCTU.ListNewClaimTracks.FirstOrDefault()?.TrackingErrorDefNum??0)!=0);
		}

		private void gridStatusHistory_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			ClaimTracking claimTrack=(ClaimTracking)gridStatusHistory.ListGridRows[e.Row].Tag;
			if(Security.IsAuthorized(Permissions.ClaimHistoryEdit,claimTrack.DateTimeEntry)){
				using FormClaimCustomTrackingUpdate FormCCTU=new FormClaimCustomTrackingUpdate(ClaimCur,claimTrack);
				FormCCTU.ShowDialog();
				if(FormCCTU.DialogResult==DialogResult.OK) {
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
			bool isSentOrReceived=ListTools.In(comboClaimStatus.SelectedIndex,_listClaimStatus.IndexOf(ClaimStatus.Sent),_listClaimStatus.IndexOf(ClaimStatus.Received));
			ClaimIsValidState state=ClaimL.ClaimIsValid(textDateService.Text,ClaimCur.ClaimType,isSentOrReceived,textDateSent.Text,_listClaimProcsForClaim
				,ClaimCur.PlanNum,PlanList,textNote.Text,ClaimCur.UniformBillType,(ClaimCorrectionType)comboCorrectionType.SelectedIndex
			);
			switch(state) {
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
				DialogResult result=MessageBox.Show("Warnings:\r\n"+warning+"\r\nDo you wish to continue anyway?","",
					MessageBoxButtons.OKCancel);
				if(result!=DialogResult.OK){
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
			if(notAuthorized){
				return null;
			}
			//patnum
			ClaimCur.DateService=PIn.Date(textDateService.Text);
			if(textDateSent.Text==""){
				ClaimCur.DateSent=DateTime.MinValue;
			}
			else{
				ClaimCur.DateSent=PIn.Date(textDateSent.Text);
			}
			ClaimCur.ClaimStatus=_listClaimStatus[comboClaimStatus.SelectedIndex].GetDescription(true);
			bool wasSentOrReceived=ListTools.In(ClaimCur.ClaimStatus,ClaimStatus.Sent.GetDescription(true),ClaimStatus.Received.GetDescription(true));
			if(wasSentOrReceived && ClaimCur.DateSentOrig.Year<1880) {
				textDateSentOrig.Text=DateTime.Today.ToShortDateString();
				ClaimCur.DateSentOrig=DateTime.Today;
			}
			//claimType can't be changed here.
			ClaimCur.MedType=(EnumClaimMedType)comboMedType.SelectedIndex;
			if(comboClaimForm.SelectedIndex==0){
				ClaimCur.ClaimForm=0;
			}
			else{
				ClaimCur.ClaimForm=_listClaimForms[comboClaimForm.SelectedIndex-1].ClaimFormNum;
			}
			if(textDateRec.Text==""){
				ClaimCur.DateReceived=DateTime.MinValue;
			}
			else{
				ClaimCur.DateReceived=PIn.Date(textDateRec.Text);
			}
			//planNum
			ClaimCur.SpecialProgramCode=(EnumClaimSpecialProgram)comboSpecialProgram.SelectedIndex;
			//patRelats will always be selected
			ClaimCur.PatRelat=(Relat)comboPatRelat.SelectedIndex;
			ClaimCur.PatRelat2=(Relat)comboPatRelat2.SelectedIndex;
			ClaimCur.ProvTreat=comboProvTreat.GetSelectedProvNum();
			ClaimCur.PriorAuthorizationNumber=textPriorAuth.Text;
			ClaimCur.PreAuthString=textPredeterm.Text;
			//isprosthesis handled earlier
			ClaimCur.PriorDate=PIn.Date(textPriorDate.Text);
			ClaimCur.ReasonUnderPaid=textReasonUnder.Text;
			ClaimCur.ClaimNote=textNote.Text;
			//ispreauth
			ClaimCur.ProvBill=comboProvBill.GetSelectedProvNum();
			ClaimCur.IsOrtho=checkIsOrtho.Checked;
			ClaimCur.OrthoTotalM=PIn.Byte(textOrthoTotalM.Text);
			ClaimCur.OrthoRemainM=PIn.Byte(textOrthoRemainM.Text);
			ClaimCur.OrthoDate=PIn.Date(textOrthoDate.Text);
			ClaimCur.RefNumString=textRefNum.Text;
			ClaimCur.PlaceService=(PlaceOfService)comboPlaceService.SelectedIndex;
			ClaimCur.EmployRelated=(YN)comboEmployRelated.SelectedIndex;
			switch(comboAccident.SelectedIndex){
				case 0:
					ClaimCur.AccidentRelated="";
					break;
				case 1:
					ClaimCur.AccidentRelated="A";
					break;
				case 2:
					ClaimCur.AccidentRelated="E";
					break;
				case 3:
					ClaimCur.AccidentRelated="O";
					break;
			}
			//AccidentDate is further down
			ClaimCur.AccidentST=textAccidentST.Text;
			ClaimCur.ClinicNum=comboClinic.SelectedClinicNum;
			ClaimCur.CorrectionType=(ClaimCorrectionType)Enum.GetValues(typeof(ClaimCorrectionType)).GetValue(comboCorrectionType.SelectedIndex);
			ClaimCur.ClaimIdentifier=string.IsNullOrWhiteSpace(textClaimIdentifier.Text) ? Claims.ConvertClaimId(ClaimCur,PatCur) : textClaimIdentifier.Text;
			ClaimCur.OrigRefNum=textOrigRefNum.Text;
			ClaimCur.ShareOfCost=PIn.Double(textShareOfCost.Text);
			//attachments
			ClaimCur.Radiographs=PIn.Byte(textRadiographs.Text);
			ClaimCur.AttachedImages=PIn.Int(textAttachImages.Text);
			ClaimCur.AttachedModels=PIn.Int(textAttachModels.Text);
			List<string> flags=new List<string>();
			#region Not for ClaimConnect, see OK_Click()
			//offices might have ClaimConnect as their default clearinghouse but not make use of DXC. In this case we want to consider information in the NEA tab
			if(!_clearinghouse.IsAttachmentSendAllowed) {
				if(checkAttachEoB.Checked){
					flags.Add("EoB");
				}
				if(checkAttachNarrative.Checked){
					flags.Add("Note");
				}
				if(checkAttachPerio.Checked){
					flags.Add("Perio");
				}
				if(checkAttachMisc.Checked){
					flags.Add("Misc");
				}
				if(radioAttachMail.Checked){
					flags.Add("Mail");
				}
				if(radioAttachElect.Checked){
					flags.Add("Elect");
				}
				if(textAttachID.Text!="") {
					flags.Add("Misc");
				}
				ClaimCur.AttachedFlags="";
				for(int i=0;i<flags.Count;i++){
					if(i>0){
						ClaimCur.AttachedFlags+=",";
					}
					ClaimCur.AttachedFlags+=flags[i];
				}
			}
			#endregion
			//Use the DXC attachmentID if it is not blank, otherwise use the NEA attachmentID. In the case they are both blank it does not matter,
			//but users can have DXC attachments enabled with old NEA numbers attached. This will preserve those.
			ClaimCur.AttachmentID=textAttachmentID.Text!="" ? textAttachmentID.Text : textAttachID.Text;
			//Canadian---------------------------------------------------------------------------------
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				ClaimCur.CanadianMaterialsForwarded="";
				if(checkEmail.Checked) {
					ClaimCur.CanadianMaterialsForwarded+="E";
				}
				if(checkCorrespondence.Checked) {
					ClaimCur.CanadianMaterialsForwarded+="C";
				}
				if(checkModels.Checked) {
					ClaimCur.CanadianMaterialsForwarded+="M";
				}
				if(checkXrays.Checked) {
					ClaimCur.CanadianMaterialsForwarded+="X";
				}
				if(checkImages.Checked) {
					ClaimCur.CanadianMaterialsForwarded+="I";
				}
				ClaimCur.CanadianReferralProviderNum=textReferralProvider.Text;
				ClaimCur.CanadianReferralReason=(byte)comboReferralReason.SelectedIndex;
				ClaimCur.AccidentDate=PIn.Date(textCanadianAccidentDate.Text);
				ClaimCur.IsOrtho=checkCanadianIsOrtho.Checked;
				//max prosth-----------------------------------------------------------------------------------------------------
				switch(comboMaxProsth.SelectedIndex) {
					case 0:
						ClaimCur.CanadianIsInitialUpper="";
						break;
					case 1:
						ClaimCur.CanadianIsInitialUpper="Y";
						break;
					case 2:
						ClaimCur.CanadianIsInitialUpper="N";
						break;
					case 3:
						ClaimCur.CanadianIsInitialUpper="X";
						break;
				}
				ClaimCur.CanadianDateInitialUpper=PIn.Date(textDateInitialUpper.Text);
				ClaimCur.CanadianMaxProsthMaterial=(byte)comboMaxProsthMaterial.SelectedIndex;
				//mand prosth-----------------------------------------------------------------------------------------------------
				switch(comboMandProsth.SelectedIndex) {
					case 0:
						ClaimCur.CanadianIsInitialLower="";
						break;
					case 1:
						ClaimCur.CanadianIsInitialLower="Y";
						break;
					case 2:
						ClaimCur.CanadianIsInitialLower="N";
						break;
					case 3:
						ClaimCur.CanadianIsInitialLower="X";
						break;
				}
				ClaimCur.CanadianDateInitialLower=PIn.Date(textDateInitialLower.Text);
				ClaimCur.CanadianMandProsthMaterial=(byte)comboMandProsthMaterial.SelectedIndex;
				//ortho treatment
				if(groupCanadaOrthoPredeterm.Enabled && textDateCanadaEstTreatStartDate.Text!="" && 
					textCanadaInitialPayment.Text!="" && textCanadaExpectedPayCycle.Text!="" &&
					textCanadaTreatDuration.Text!="" && textCanadaNumPaymentsAnticipated.Text!="" && 
					textCanadaAnticipatedPayAmount.Text!="") {
					ClaimCur.CanadaEstTreatStartDate=DateTime.Parse(textDateCanadaEstTreatStartDate.Text);
					ClaimCur.CanadaInitialPayment=Double.Parse(textCanadaInitialPayment.Text);
					ClaimCur.CanadaPaymentMode=byte.Parse(textCanadaExpectedPayCycle.Text);
					ClaimCur.CanadaTreatDuration=byte.Parse(textCanadaTreatDuration.Text);
					ClaimCur.CanadaNumAnticipatedPayments=byte.Parse(textCanadaNumPaymentsAnticipated.Text);
					ClaimCur.CanadaAnticipatedPayAmount=Double.Parse(textCanadaAnticipatedPayAmount.Text);
				}
				else {
					ClaimCur.CanadaEstTreatStartDate=DateTime.MinValue;
					ClaimCur.CanadaInitialPayment=0;
					ClaimCur.CanadaPaymentMode=0;
					ClaimCur.CanadaTreatDuration=0;
					ClaimCur.CanadaNumAnticipatedPayments=0;
					ClaimCur.CanadaAnticipatedPayAmount=0;
				}
			}//End Canadian-----------------------------------------------------------------------------
			else {
				ClaimCur.AccidentDate=PIn.Date(textAccidentDate.Text);
				ClaimCur.IsOrtho=checkIsOrtho.Checked;
			}
			ClaimCur.UniformBillType=textBillType.Text;
			ClaimCur.AdmissionTypeCode=textAdmissionType.Text;
			ClaimCur.AdmissionSourceCode=textAdmissionSource.Text;
			ClaimCur.PatientStatusCode=textPatientStatus.Text;
			ClaimCur.ProvOrderOverride=_provNumOrdering;
			if(_referralOrdering==null) {
				ClaimCur.OrderingReferralNum=0;
			}
			else {
				ClaimCur.OrderingReferralNum=_referralOrdering.ReferralNum;
			}
			if(ListClaimValCodes!=null) {
				GetValCodes(ListClaimValCodes,0,ClaimCur.ClaimNum,textVC39aCode,textVC39aAmt);
				GetValCodes(ListClaimValCodes,1,ClaimCur.ClaimNum,textVC40aCode,textVC40aAmt);
				GetValCodes(ListClaimValCodes,2,ClaimCur.ClaimNum,textVC41aCode,textVC41aAmt);
				GetValCodes(ListClaimValCodes,3,ClaimCur.ClaimNum,textVC39bCode,textVC39bAmt);
				GetValCodes(ListClaimValCodes,4,ClaimCur.ClaimNum,textVC40bCode,textVC40bAmt);
				GetValCodes(ListClaimValCodes,5,ClaimCur.ClaimNum,textVC41bCode,textVC41bAmt);
				GetValCodes(ListClaimValCodes,6,ClaimCur.ClaimNum,textVC39cCode,textVC39cAmt);
				GetValCodes(ListClaimValCodes,7,ClaimCur.ClaimNum,textVC40cCode,textVC40cAmt);
				GetValCodes(ListClaimValCodes,8,ClaimCur.ClaimNum,textVC41cCode,textVC41cAmt);
				GetValCodes(ListClaimValCodes,9,ClaimCur.ClaimNum,textVC39dCode,textVC39dAmt);
				GetValCodes(ListClaimValCodes,10,ClaimCur.ClaimNum,textVC40dCode,textVC40dAmt);
				GetValCodes(ListClaimValCodes,11,ClaimCur.ClaimNum,textVC41dCode,textVC41dAmt);
			}
			if(ClaimCondCodeLogCur!=null || textCode0.Text!="" || textCode1.Text!="" || textCode2.Text!="" || textCode3.Text!="" || 
				textCode4.Text!="" || textCode5.Text!="" || textCode6.Text!="" || textCode7.Text!="" || textCode8.Text!="" || 
				textCode9.Text!="" || textCode10.Text!="") {
				if(ClaimCondCodeLogCur==null) {
					ClaimCondCodeLogCur=new ClaimCondCodeLog();
					ClaimCondCodeLogCur.ClaimNum=ClaimCur.ClaimNum;
					ClaimCondCodeLogCur.IsNew=true;
				}
				ClaimCondCodeLogCur.Code0=textCode0.Text;
				ClaimCondCodeLogCur.Code1=textCode1.Text;
				ClaimCondCodeLogCur.Code2=textCode2.Text;
				ClaimCondCodeLogCur.Code3=textCode3.Text;
				ClaimCondCodeLogCur.Code4=textCode4.Text;
				ClaimCondCodeLogCur.Code5=textCode5.Text;
				ClaimCondCodeLogCur.Code6=textCode6.Text;
				ClaimCondCodeLogCur.Code7=textCode7.Text;
				ClaimCondCodeLogCur.Code8=textCode8.Text;
				ClaimCondCodeLogCur.Code9=textCode9.Text;
				ClaimCondCodeLogCur.Code10=textCode10.Text;
			}
			ClaimCur.DateIllnessInjuryPreg=PIn.Date(textDateIllness.Text);
			ClaimCur.DateIllnessInjuryPregQualifier=comboDateIllnessQualifier.GetSelected<DateIllnessInjuryPregQualifier>();
			ClaimCur.DateOther=PIn.Date(textDateOther.Text);
			ClaimCur.DateOtherQualifier=comboDateOtherQualifier.GetSelected<DateOtherQualifier>();
			ClaimCur.IsOutsideLab=checkIsOutsideLab.Checked;
			List<Procedure> listProcsToUpdatePlaceOfService=new List<Procedure>();
			for(int i=0;i<_listClaimProcsForClaim.Count;i++) {
				Procedure proc=Procedures.GetProcFromList(ProcList,_listClaimProcsForClaim[i].ProcNum);
				if(proc.ProcNum==0) {
					continue;//ignores payments, etc
				}
				if(proc.PlaceService!=ClaimCur.PlaceService) {
					listProcsToUpdatePlaceOfService.Add(proc);
				}
			}
			ClaimEdit.UpdateData updateData=ClaimEdit.UpdateClaim(ClaimCur,ListClaimValCodes,ClaimCondCodeLogCur,listProcsToUpdatePlaceOfService,PatCur,
				wasSentOrReceived,_claimEditPermission);
			foreach(Procedure proc in listProcsToUpdatePlaceOfService) {
				proc.PlaceService=ClaimCur.PlaceService;
			}
			return updateData;
		}

		private void GetValCodes(List<ClaimValCodeLog> listClaimValCodes,int valCodeIdx,long claimNum,TextBox textVCCode,TextBox textVCAmount) {
			if(valCodeIdx<listClaimValCodes.Count) {//update existing ClaimValCodeLog
				listClaimValCodes[valCodeIdx].ValCode=textVCCode.Text;
				listClaimValCodes[valCodeIdx].ValAmount=PIn.Double(textVCAmount.Text);
			}
			else if(PIn.Double(textVCAmount.Text)>0 || textVCCode.Text!="") {//add a new ClaimValCodeLog
				ClaimValCodeLog claimVC=new ClaimValCodeLog();
				claimVC.ValCode=textVCCode.Text;
				claimVC.ValAmount=PIn.Double(textVCAmount.Text);
				claimVC.ClaimNum=claimNum;
				listClaimValCodes.Add(claimVC);
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
			DateTime claimCurSectDateTEdit=ClaimCur.SecDateTEdit;//Preserve the date prior to any claim updates effecting it.
			UpdateClaim();
			bool paymentIsAttached=false;
			bool claimInsPayAttached=false;
			//Loops through each of the procedures and checks that there is no payment, or insurance amount attached
			//if there is then break out and cancel deleting the claim.
			for(int i=0;i<_listClaimProcsForClaim.Count;i++){
				if(_listClaimProcsForClaim[i].ClaimPaymentNum>0){
					paymentIsAttached=true;
					break;
				}
				if(_listClaimProcsForClaim[i].InsPayAmt>0){
					claimInsPayAttached=true;
					break;
				}
			}
			if(paymentIsAttached){
				MsgBox.Show(this,"You cannot delete this claim while any insurance checks are attached.  You will have to detach all insurance checks first.");
				return;
			}
			if(claimInsPayAttached){
				MsgBox.Show(this,"You cannot delete this claim while there are insurance payments.  Set all Insurance Paid amounts to zero first.");
				return;
			}
			if(ClaimCur.ClaimStatus==ClaimStatus.Received.GetDescription(true)){//received
				MessageBox.Show(Lan.g(this,"You cannot delete this claim while status is Received.  You will have to change the status first."));
				return;
			}
			List<long> list835Attaches=Etrans835Attaches.GetForClaimNums(ClaimCur.ClaimNum).Select(x => x.Etrans835AttachNum).ToList();
			if(ClaimCur.ClaimType=="PreAuth"){
				string msgAttaches="";
				if(list835Attaches.Count>0) {
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
				if(list835Attaches.Count>0) {
					msgAttaches="This claim is attached to at least one ERA."
					+"\r\nDeleting the claim will unassociate this claim from the ERA"
					+"\r\nand the associated ERA will need to be edited before it can be Finalized."
					+"\r\n";
				}
				if(MessageBox.Show(msgAttaches+Lan.g(this,"Delete Claim?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
					return;
				}
			}
			DeleteClaimHelper(list835Attaches);
			SecurityLogs.MakeLogEntry(Permissions.ClaimDelete,ClaimCur.PatNum,PatCur.GetNameLF()
				+", "+Lan.g(this,"Date Entry")+": "+ClaimCur.SecDateEntry.ToShortDateString()
				+", "+Lan.g(this,"Date of Service")+": "+ClaimCur.DateService.ToShortDateString(),
				ClaimCur.ClaimNum,claimCurSectDateTEdit);
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
			if(ClaimCur.ClaimType=="PreAuth"//all preauth claimprocs are just duplicates
				|| ClaimCur.ClaimType=="Cap") //all cap claimprocs are just duplicates
			{
				ClaimProcs.DeleteMany(_listClaimProcsForClaim);
			}
			else {//all other claim types use original estimate claimproc.
				List<Benefit> benList=Benefits.Refresh(PatPlanList,SubList);
				InsPlan plan=InsPlans.GetPlan(ClaimCur.PlanNum,PlanList);
				if(_blueBookEstimateData==null) {
					_blueBookEstimateData=new BlueBookEstimateData(PlanList,SubList,PatPlanList);
				}
				for(int i=0;i<_listClaimProcsForClaim.Count;i++) {
					if(_listClaimProcsForClaim[i].Status==ClaimProcStatus.Supplemental//supplementals are duplicate
						|| _listClaimProcsForClaim[i].ProcNum==0)//total payments get deleted
					{
						ClaimProcs.Delete(_listClaimProcsForClaim[i]);
						continue;
					}
					//so only changed back to estimate if attached to a proc
					_listClaimProcsForClaim[i].Status=ClaimProcStatus.Estimate;
					if(_dictCanadianLabClaimProcs.ContainsKey(_listClaimProcsForClaim[i].ProcNum)) {//If proc "i" has labs.
						foreach(ClaimProc labClaimProc in _dictCanadianLabClaimProcs[_listClaimProcsForClaim[i].ProcNum]) {//Foreach lab
							labClaimProc.Status=_listClaimProcsForClaim[i].Status;//Match to parent status so estimates for ClaimProcs.CanadianLabBaseEstHelper().
							ClaimProcs.Update(labClaimProc);//Both parent procs and labs need to have their statuses in synch when ComputeBaseEst
						}
					}
					_listClaimProcsForClaim[i].ClaimNum=0;
					//already handled the case where claimproc.ProcNum=0 for payments etc. above
					Procedure proc=Procedures.GetProcFromList(ProcList,_listClaimProcsForClaim[i].ProcNum);
					if(proc.ProcNumLab!=0) {
						//Skip lab procedures because their parent will handle their estimate updates below (including the status and claimNum set above).
						continue;
					}
					PatPlan patPlan=PatPlanList.FirstOrDefault(x => x.InsSubNum==_listClaimProcsForClaim[i].InsSubNum);
					if(patPlan==null) {
						continue;
					}
					if(patPlan.Ordinal==1) {
						ClaimProcs.ComputeBaseEst(_listClaimProcsForClaim[i],proc,plan,patPlan.PatPlanNum,benList,null,null,PatPlanList,0,0,PatCur.Age,0
							,PlanList,SubList,ListSubLinks,false,null,blueBookEstimateData:_blueBookEstimateData);
					}
					else {
						//We're not going to bother to also get paidOtherInsBaseEst:
						double paidOtherInsTotal=ClaimProcs.GetPaidOtherInsTotal(_listClaimProcsForClaim[i],PatPlanList);
						double writeOffOtherIns=ClaimProcs.GetWriteOffOtherIns(_listClaimProcsForClaim[i],PatPlanList);
						ClaimProcs.ComputeBaseEst(_listClaimProcsForClaim[i],proc,plan,patPlan.PatPlanNum,benList,null,null,PatPlanList,paidOtherInsTotal
							,paidOtherInsTotal,PatCur.Age,writeOffOtherIns,PlanList,SubList,ListSubLinks,false,null,blueBookEstimateData:_blueBookEstimateData);
					}
					InsBlueBookLog blueBookLog=_blueBookEstimateData.CreateInsBlueBookLog(_listClaimProcsForClaim[i]);
					if(blueBookLog!=null) {
						InsBlueBookLogs.Insert(blueBookLog);
					}
					ClaimProcs.Update(_listClaimProcsForClaim[i]);
				}
			}
			ClaimProcs.DeleteEstimatesForDroppedPatPlan(_listClaimProcsForClaim);
			Claims.Delete(ClaimCur,list835Attaches);
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!ClaimIsValid()){
				return;
			}
			//if status is received, all claimprocs must also be received.
			if(comboClaimStatus.SelectedIndex==_listClaimStatus.IndexOf(ClaimStatus.Received)) {
				bool allReceived=true;
				for(int i=0;i<_listClaimProcsForClaim.Count;i++){
					if(((ClaimProc)_listClaimProcsForClaim[i]).Status==ClaimProcStatus.NotReceived){
						allReceived=false;
					}
				}
				if(!allReceived){
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"All items will be marked received.  Continue?")){
						return;
					}
					for(int i=0;i<_listClaimProcsForClaim.Count;i++){
						if(_listClaimProcsForClaim[i].Status==ClaimProcStatus.NotReceived){
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
					}
				}
			}
			else{//claim is any status except received
				bool anyReceived=false;
				for(int i=0;i<_listClaimProcsForClaim.Count;i++){
					if(((ClaimProc)_listClaimProcsForClaim[i]).Status==ClaimProcStatus.Received){
						anyReceived=true;
					}
				}
				if(anyReceived){
					//Too dangerous to automatically set items not received because I would have to check for attachments to checks, etc.
					//Also too annoying to block user.
					//So just warn user.
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Some of the items are marked received.  This is not a good idea since it will cause them to show in the Account as a 'payment'.  Continue anyway?")){
						return;
					}
				}
			}
			//if status is received and there is no received date
			if(comboClaimStatus.SelectedIndex==_listClaimStatus.IndexOf(ClaimStatus.Received) && textDateRec.Text==""){
				textDateRec.Text=DateTime.Today.ToShortDateString();
			}
			ClaimEdit.UpdateData updateData=UpdateClaim();
			if(comboClaimStatus.SelectedIndex==_listClaimStatus.IndexOf(ClaimStatus.WaitingToSend)){//waiting to send
				ClaimSendQueueItem[] listQueue=updateData.ListSendQueueItems;
				if(listQueue.Length==0 || !listQueue[0].CanSendElect) {//listQueue can be empty if another workstation deleted the claim
					DialogResult=DialogResult.OK;
					return;
				}
				//string warnings;
				//string missingData=
				Clearinghouse clearinghouseHq=ClearinghouseL.GetClearinghouseHq(listQueue[0].ClearinghouseNum);
				Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
				listQueue[0]=Eclaims.GetMissingData(clearinghouseClin,listQueue[0]);
				if(!string.IsNullOrEmpty(listQueue[0].ErrorsPreventingSave)) {
					MessageBox.Show(listQueue[0].ErrorsPreventingSave);
					return;
				}
				else if(listQueue[0].MissingData!="") {
					if(MessageBox.Show(Lan.g(this,"Cannot send claim until missing data is fixed:")+"\r\n"+listQueue[0].MissingData+"\r\n\r\nContinue anyway?",
						"",MessageBoxButtons.OKCancel)==DialogResult.OK)
					{
						DialogResult=DialogResult.OK;
					}
					return;
				}
				else if(clearinghouseClin.IsAttachmentSendAllowed && clearinghouseClin.CommBridge==EclaimsCommBridge.ClaimConnect) {//No missing data, but can send attachments electronically
					//Validating for attachments is currently only supported for ClaimConnect customers
					try {
						ClaimConnect.ValidateClaimResponse response=ClaimConnect.ValidateClaim(ClaimCur,true);
						if(response.IsAttachmentRequired) {
							if(ClaimCur.AttachedFlags!="Misc") {//No need to visit the database again if this is already the case
								ClaimCur.AttachedFlags="Misc";
								Claims.Update(ClaimCur);
							}
							if(MsgBox.Show(this,MsgBoxButtons.YesNo,"An attachment is required for this claim. Would you like to open the claim attachment form?")) {
								FormClaimAttachment.Open(ClaimCur);
							}
						}
						else if(ClaimCur.Attachments.Count==0 && ClaimCur.AttachedFlags!="Mail") {//Don't set to 'Mail' on claims with attachments or make unneccessary DB trips.
							ClaimCur.AttachedFlags="Mail";
							Claims.Update(ClaimCur);
						}
					}
					catch(ODException ex) {
						MessageBox.Show(ex.Message);
					}
					catch(Exception ex) {
						FriendlyException.Show(ex.Message,ex);
					}
				}
				//if(MsgBox.Show(this,true,"Send electronic claim immediately?")){
				//	List<ClaimSendQueueItem> queueItems=new List<ClaimSendQueueItem>();
				//	queueItems.Add(listQueue[0]);
				//	Eclaims.Eclaims.SendBatches(queueItems);//this also calls SetClaimSentOrPrinted which creates the etrans entry.
				//}
			}
			if(comboClaimStatus.SelectedIndex==_listClaimStatus.IndexOf(ClaimStatus.Received)) {//Received
				ShowProviderTransferWindow(ClaimCur,PatCur,FamCur);
			}
			Plugins.HookAddCode(this,"FormClaimEdit.butOK_Click_end",ClaimCur);
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
			DateTime claimSecDateTEdit=ClaimCur.SecDateTEdit;//Preserve the date prior to any claim updates effecting it.
			if(DialogResult==DialogResult.OK) {
				if(_isDeleting) {
					InsBlueBooks.DeleteByClaimNums(ClaimCur.ClaimNum);
					return;
				}
				InsBlueBooks.SynchForClaimNums(ClaimCur.ClaimNum);
				if(IsNew) {
					SecurityLogs.MakeLogEntry(_claimEditPermission,PatCur.PatNum,"New claim created for "+PatCur.LName+","+PatCur.FName,
						ClaimCur.ClaimNum,claimSecDateTEdit);
				}
				else {//save for all except Delete
					SecurityLogs.MakeLogEntry(_claimEditPermission,PatCur.PatNum,"Claim saved for "+PatCur.LName+","+PatCur.FName,
						ClaimCur.ClaimNum,claimSecDateTEdit);
				}
				if(comboClaimStatus.SelectedIndex==_listClaimStatus.IndexOf(ClaimStatus.Received)) {//Received
					if(_isPaymentEntered && PrefC.GetBool(PrefName.PromptForSecondaryClaim) && Security.IsAuthorized(Permissions.ClaimSend,true)) {
						//We currenlty require that payment be entered in this instance of the form.
						//We might later decide that we want to check for secondary whenever the primary is recieved and there is financial values entered
						//regardless of when they were entered.
						ClaimL.PromptForSecondaryClaim(_listClaimProcsForClaim);
					}
				}
				return;
			}
			if(!IsNew) {
				InsBlueBooks.SynchForClaimNums(ClaimCur.ClaimNum);
				return;
			}
			if(ClaimCur.InsPayAmt>0) {
				MsgBox.Show(this,"Not allowed to cancel because an insurance payment was entered.  Either click OK, or zero out the insurance payments.");
				e.Cancel=true;
				return;
			}
			// check current claim status to make sure it has not changed before deleting it if new
            if(ClaimCur.ClaimStatus!=Claims.GetClaim(ClaimCur.ClaimNum).ClaimStatus) {
				MsgBox.Show(Lan.g(this,"The claim status has been changed by another instance of the program. Claim has not been deleted."));
				return;
            }
			//This is a new claim where they clicked "Cancel".
			SecurityLogs.MakeLogEntry(Permissions.ClaimEdit,PatCur.PatNum,"New claim cancelled for "+PatCur.LName+","+PatCur.FName,
				ClaimCur.ClaimNum,claimSecDateTEdit);
			DeleteClaimHelper();
			InsBlueBooks.DeleteByClaimNums(ClaimCur.ClaimNum);
			//When the user "cancels" out of a new claim we want to delete any corresponding claim snapshots, but only if not using the service trigger type
			//The service trigger type snapshots have nothing to do with creating this claim, so leave as is.
			if(PrefC.GetBool(PrefName.ClaimSnapshotEnabled)
				&& PIn.Enum<ClaimSnapshotTrigger>(PrefC.GetString(PrefName.ClaimSnapshotTriggerType),true)!=ClaimSnapshotTrigger.Service)
			{
				ClaimSnapshots.DeleteForClaimProcs(_listClaimProcsForClaim.Select(x => x.ClaimProcNum).ToList());
			}
		}

		///<summary>ClaimStatus is a string in the database and this enum gives us a centralized point to maintain that</summary>
		private enum ClaimStatus {
			///<summary>0- "U" - unsent claim</summary>
			[ShortDescription("U")]
			[Description("Unsent")]
			Unsent,
			///<summary>1- "H" - Intended for secondary claim to wait until primary has been received</summary>
			[ShortDescription("H")]
			[Description("Hold Until Pri Received")]
			HoldUntilPriReceived,
			///<summary>2- "W" - Claim is ready to be sent/printed </summary>
			[ShortDescription("W")]
			[Description("Waiting to Send")]
			WaitingToSend,
			///<summary>3- "P" - Claim has ben sent but has not been verified, rarely used</summary>
			[ShortDescription("P")]
			[Description("Probably Sent")]
			ProbablySent,
			///<summary>4- "S" - Claim verified to be sent</summary>
			[ShortDescription("S")]
			[Description("Sent - Verified")]
			Sent,
			///<summary>5 - "R" - Claim has been received from insurance and is either accepted or denied</summary>
			[ShortDescription("R")]
			[Description("Received")]
			Received
		}
	}
}
