using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public partial class FormProcEdit : FormODBase {
		///<summary>Mostly used for permissions.</summary>
		public bool IsNew;
		public List<ClaimProcHist> ListClaimProcHists;
		public List<ClaimProcHist> ListClaimProcHistsLoop;
		private ProcedureCode _procedureCode;
		private Procedure _procedure;
		private Procedure _procedureOld;
		private List<ClaimProc> _listClaimProcsForProc;
		private ArrayList _paySplitsForProc;
		private ArrayList _adjustmentsForProc;
		private Patient _patient;
		private Family _family;
		private List<InsPlan> _listInsPlans;
		///<summary>Lazy loaded, do not directly use this variable, use the property instead.</summary>
		private List<SubstitutionLink> _listSubstLinks=null;
		///<summary>List of all payments (not paysplits) that this procedure is attached to.</summary>
		private List<Payment> _listPaymentsForProc;
		private const string APPBAR_AUTOMATION_API_MESSAGE = "EZNotes.AppBarStandalone.Auto.API.Message"; 
		private const uint MSG_RESTORE=2;
		private const uint MSG_GETLASTNOTE=3;
		private List<PatPlan> _listPatPlans;
		private List<Benefit> _listBenefits;
		private bool _sigChanged;
		///<summary>This keeps the noteChanged event from erasing the signature when first loading.</summary>
		private bool _isStartingUp;
		private List<Claim> _listClaims;
		private bool _startedAttachedToClaim;
		private List<InsSub> _listInsSubs;
		private List<Procedure> _listCanadaLabFees;
		private Snomed _snomedBodySite=null;
		private bool _isQuickAdd=false;
		///<summary>Users can temporarily log in on this form.  Defaults to Security.CurUser.</summary>
		private Userod _curUser=Security.CurUser;
		///<summary>True if the user clicked the Change User button.</summary>
		private bool _hasUserChanged;
		///<summary></summary>
		private long _selectedProvOrderNum;
		///<summary>If this procedure is attached to an ordering referral, then this varible will not be null.</summary>
		private Referral _referralOrdering=null;
		private const string _autoNotePromptRegex=@"\[Prompt:""[a-zA-Z_0-9 ]+""\]";
		///<summary>True only when modifications to this canadian lab proc will affect the attached parent proc ins estimate.</summary>
		private bool _isEstimateRecompute=false;
		private OrthoProcLink _orthoProcLink;
		private List<Def> _listDiagnosisDefs;
		private List<Def> _listPrognosisDefs;
		private List<Def> _listTxPriorityDefs;
		private List<Def> _listBillingTypeDefs;
		///<summary>Most of the data necessary to load this form.</summary>
		private ProcEdit.LoadData _loadData;
		///<summary>There are a number of places in this form that need fees, but none of them are heavily used.  This will help a little.  Lazy loaded, do not directly use this variable, use the property instead.</summary>
		private Lookup<FeeKey2,Fee> _lookupFees;
		///<summary>See _lookupFees.  Sometimes, we need a list instead of a lookup.</summary>
		private List<Fee> _listFees;
		private List<ToothInitial> _listPatToothInitials=null;
		///<summary>All primary teeth currently being displayed in the UI, but stored as permanent teeth so that indexes are easy to calculate.</summary>
		private List<string> _listPriTeeth=null;

		private List<SubstitutionLink> ListSubstLinks {
			get {
				if(_listSubstLinks==null) {
					_listSubstLinks=SubstitutionLinks.GetAllForPlans(_listInsPlans);
				}
				return _listSubstLinks;
			}
		}

		private Lookup<FeeKey2,Fee> LookupFees {
			get {
				if(_lookupFees==null) {
					FillFees();
				}
				return _lookupFees;
			}
		}

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && tabControl.SelectedTab==tabPageCanada) {
				return "FormProcEditCanada";
			}
			return "FormProcEdit";
		}

		private void FillFees(){
			List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>(){ ProcedureCodes.GetProcCode(_procedure.CodeNum) };
			List<Procedure> listProcedures=new List<Procedure>(){_procedure };
			long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(_patient.PatNum,_procedure.ProcDate);
			_listFees=Fees.GetListFromObjects(listProcedureCodes,listProcedures.Select(x=>x.MedicalCode).ToList(),listProcedures.Select(x=>x.ProvNum).ToList(),
				_patient.PriProv,_patient.SecProv,_patient.FeeSched,_listInsPlans,listProcedures.Select(x=>x.ClinicNum).ToList(),null,//appts not needed
				ListSubstLinks,discountPlanNum);
			_lookupFees=(Lookup<FeeKey2,Fee>)_listFees.ToLookup(x => new FeeKey2(x.CodeNum,x.FeeSched));
		}

		private List<Fee> ListFees {
			get {
				if(_listFees==null) {
					FillFees();
				}
				return _listFees;
			}
		}

		///<summary>Inserts are not done within this dialog, but must be done ahead of time from outside.  You must specify a procedure to edit, and only the changes that are made in this dialog get saved.  Only used when double click in Account, Chart, TP, and in ContrChart.AddProcedure().  The procedure may be deleted if new, and user hits Cancel.</summary>
		public FormProcEdit(Procedure proc,Patient patCur,Family famCur,bool isQuickAdd=false,List<ToothInitial> listPatToothInitials=null) {
			_procedure=proc;
			_procedureOld=proc.Copy();
			_patient=patCur;
			_family=famCur;
			//HistList=null;
			//LoopList=null;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			if(!PrefC.GetBool(PrefName.ShowFeatureMedicalInsurance)) {
				tabControl.TabPages.Remove(tabPageMedical);
				//groupMedical.Visible=false;
			}
			_isQuickAdd=isQuickAdd;
			if(isQuickAdd) {
				//Note: We normally don't use Opacity. This is a special case to prevent the form from flashing on load.
				this.Opacity=0;
			}
			_listPatToothInitials=listPatToothInitials;
		}

		private void FormProcInfo_Load(object sender,System.EventArgs e) {
			if(PrefC.IsODHQ) {
				labelTaxEst.Visible=true;
				textTaxAmt.Visible=true;
				textTaxAmt.Text=POut.Double(_procedure.TaxAmt);
				if(_procedure.ProcStatus==ProcStat.C) {
					labelTaxEst.Text="Tax Amt";
				}
			}
			_loadData=ProcEdit.GetLoadData(_procedure,_patient,_family);
			_orthoProcLink=_loadData.OrthoProcedureLink;
			if(_orthoProcLink!=null) {
				textProcFee.Enabled=false;
				butChange.Enabled=false;
				checkNoBillIns.Enabled=false;
				textDiscount.Enabled=false;
				butAddAdjust.Enabled=false;
				butAddExistAdj.Enabled=false;
			}
			_listInsSubs=_loadData.ListInsSubs;
			_listInsPlans=_loadData.ListInsPlans;
			signatureBoxWrapper.SetAllowDigitalSig(true);
			_listClaimProcsForProc=new List<ClaimProc>();
			//Set the title bar to show the patient's name much like the main screen does.
			this.Text+=" - "+_patient.GetNameLF();
			textDateEntry.Text=_procedure.DateEntryC.ToShortDateString();
			if(PrefC.GetBool(PrefName.EasyHidePublicHealth)){
				labelPlaceService.Visible=false;
				comboPlaceService.Visible=false;
				labelSite.Visible=false;
				textSite.Visible=false;
				butPickSite.Visible=false;
			}
			if(!Security.IsAuthorized(Permissions.ProcEditShowFee,true)){
				labelAmount.Visible=false;
				textProcFee.Visible=false;
				labelTaxEst.Visible=false;
				textTaxAmt.Visible=false;
			}
			_listClaims=_loadData.ListClaims;
			_procedureCode=ProcedureCodes.GetProcCode(_procedure.CodeNum);
			if(_procedure.ProcStatus==ProcStat.C && PrefC.GetBool(PrefName.ProcLockingIsAllowed) && !_procedure.IsLocked) {
				butLock.Visible=true;
			}
			else {
				butLock.Visible=false;
			}
			if(IsNew){
				if(_procedure.ProcStatus==ProcStat.C){
					if(!_isQuickAdd && !Security.IsAuthorized(Permissions.ProcComplCreate)){
						DialogResult=DialogResult.Cancel;
						return;
					}
				}
				//SetControls();
				//return;
			}
			else{
				if(_procedure.ProcStatus==ProcStat.C){
					textDiscount.Enabled=false;
					if(_procedure.IsLocked) {//Whether locking is currently allowed, this proc may have been locked previously.
						butOK.Enabled=false;//use this state to cascade permission to any form opened from here
						butDelete.Enabled=false;
						butChange.Enabled=false;
						butEditAnyway.Enabled=false;
						butSetComplete.Enabled=false;
						butSnomedBodySiteSelect.Enabled=false;
						butNoneSnomedBodySite.Enabled=false;
						labelLocked.Visible=true;
						butAppend.Visible=true;
						textNotes.ReadOnly=true;//just for visual cue.  No way to save changes, anyway.
						textNotes.BackColor=SystemColors.Control;
						butInvalidate.Visible=true;
						butInvalidate.Location=butLock.Location;
					}
					else{
						butInvalidate.Visible=false;
					}
				}
			}
			//ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
			_listClaimProcsForProc=_loadData.ListClaimProcsForProc;
			_listPatPlans=_loadData.ListPatPlans;
			_listBenefits=_loadData.ListBenefits;
			if(Procedures.IsAttachedToClaim(_procedure,_listClaimProcsForProc)){
				_startedAttachedToClaim=true;
				//however, this doesn't stop someone from creating a claim while this window is open,
				//so this is checked at the end, too.
				panel1.Enabled=false;
				comboProcStatus.Enabled=false;
				checkNoBillIns.Enabled=false;
				butChange.Enabled=false;
				butEditAnyway.Visible=true;
				butSetComplete.Enabled=false;
				textCanadaLabFee1.Enabled=false;
				textCanadaLabFee2.Enabled=false;
			}
			if(Procedures.IsAttachedToClaim(_procedure,_listClaimProcsForProc,false)) {
				butDelete.Enabled=false;
				labelClaim.Visible=true;
				butAddEstimate.Enabled=false;
			}
			if(PrefC.GetBool(PrefName.EasyHideClinical)){
				labelDx.Visible=false;
				comboDx.Visible=false;
				labelPrognosis.Visible=false;
				comboPrognosis.Visible=false;
			}
			if(PrefC.GetBool(PrefName.EasyHideMedicaid)) {
				comboBillingTypeOne.Visible=false;
				labelBillingTypeOne.Visible=false;
				comboBillingTypeTwo.Visible=false;
				labelBillingTypeTwo.Visible=false;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				//groupCanadianProcType.Location=new Point(106,301);
				groupProsth.Visible=false;
				labelClaimNote.Visible=false;
				textClaimNote.Visible=false;
				butBF.Text=Lan.g(this,"B/V");//vestibular instead of facial
				butV.Text=Lan.g(this,"5");
				if(_procedureCode.IsCanadianLab) { //Prevent lab fees from having lab fees attached.
					labelCanadaLabFee1.Visible=false;
					textCanadaLabFee1.Visible=false;
					labelCanadaLabFee2.Visible=false;
					textCanadaLabFee2.Visible=false;
				}
				else {
					_listCanadaLabFees=Procedures.GetCanadianLabFees(_procedure.ProcNum);
					if(_listCanadaLabFees.Count>0) {
						textCanadaLabFee1.Text=_listCanadaLabFees[0].ProcFee.ToString("n");
						if(_listCanadaLabFees[0].ProcStatus==ProcStat.C) {
							textCanadaLabFee1.ReadOnly=true;
						}
					}
					if(_listCanadaLabFees.Count>1) {
						textCanadaLabFee2.Text=_listCanadaLabFees[1].ProcFee.ToString("n");
						if(_listCanadaLabFees[1].ProcStatus==ProcStat.C) {
							textCanadaLabFee2.ReadOnly=true;
						}
					}
				}
			}
			else {
				tabControl.Controls.Remove(tabPageCanada);
				//groupCanadianProcType.Visible=false;
			}
			if(PrefC.GetBool(PrefName.ShowFeatureMedicalInsurance)) {
				labelEndTime.Visible=true;
				textTimeEnd.Visible=true;
				butNow.Visible=true;
				labelTimeFinal.Visible=true;
				textTimeFinal.Visible=true;
			}
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				textNotes.HideSelection=false;//When text is selected programmatically using our Search function, this causes the selection to be visible to the users.
			}
			else {
				butSearch.Visible=false;
				labelSnomedBodySite.Visible=false;
				textSnomedBodySite.Visible=false;
				butSnomedBodySiteSelect.Visible=false;
				butNoneSnomedBodySite.Visible=false;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				radioS1.Text="03";//Sextant 1 in the United States is sextant 03 in Canada.
				radioS2.Text="04";//Sextant 2 in the United States is sextant 04 in Canada.
				radioS3.Text="05";//Sextant 3 in the United States is sextant 05 in Canada.
				radioS4.Text="06";//Sextant 4 in the United States is sextant 06 in Canada.
				radioS5.Text="07";//Sextant 5 in the United States is sextant 07 in Canada.
				radioS6.Text="08";//Sextant 6 in the United States is sextant 08 in Canada.
			}
			SetOrderingProvider(null);//Clears both the internal ordering and referral ordering providers.
			if(_procedure.ProvOrderOverride!=0) {
				SetOrderingProvider(Providers.GetProv(_procedure.ProvOrderOverride));
			}
			else if(_procedure.OrderingReferralNum!=0) {
				Referral referral;
				Referrals.TryGetReferral(_procedure.OrderingReferralNum,out referral);
				SetOrderingReferral(referral);
			}
			FillComboClinic();
			comboClinic.SelectedClinicNum=_procedure.ClinicNum;
			FillComboProv();
			comboProv.SetSelectedProvNum(_procedure.ProvNum);
			_isStartingUp=true;
			FillControlsOnStartup();
			SetControlsUpperLeft();
			SetControlsEnabled(_isQuickAdd);
			FillReferral(false);
			FillIns(false);
			FillPayments(false);
			FillAdj();
			_isStartingUp=false;
			bool canEditNote=false;
			if(Security.IsAuthorized(Permissions.ProcedureNoteFull,true)) {
				canEditNote=true;
			}
			else if(Security.IsAuthorized(Permissions.ProcedureNoteUser,true) && (_procedure.UserNum==Security.CurUser.UserNum || signatureBoxWrapper.SigIsBlank)) {
				canEditNote=true;//They have limited permission and this is their note that they signed.
			}
			if(!canEditNote) {
				textNotes.ReadOnly=true;
				buttonUseAutoNote.Enabled=false;
				butEditAutoNote.Enabled=false;
				signatureBoxWrapper.Enabled=false;
				labelPermAlert.Visible=true;
				butAppend.Enabled=false;//don't allow appending notes either.
				butChangeUser.Enabled=false;
			}
			else if(!Userods.CanUserSignNote()) {
				signatureBoxWrapper.Enabled=false;
				labelPermAlert.Visible=true;
				labelPermAlert.Text=Lans.g(this,"Notes can only be signed by providers.");
			}
			bool hasAutoNotePrompt=Regex.IsMatch(textNotes.Text,_autoNotePromptRegex);
			butEditAutoNote.Visible=hasAutoNotePrompt;
			//string retVal=ProcCur.Note+ProcCur.UserNum.ToString();
			//using MsgBoxCopyPaste msgb=new MsgBoxCopyPaste(retVal);
			//msgb.ShowDialog();
			if(_isQuickAdd) {
				textDate.Enabled=false;
				ProcNoteUiHelper();//Add any default notes.
				butOK_Click(this,new EventArgs());
				if(this.DialogResult!=DialogResult.OK) {
					this.Opacity=100;
					this.CenterToScreen();
					this.BringToFront();
				}
			}
		}

		private void tabControl_SizeChanged(object sender,EventArgs e) {
			LayoutManager.Move(gridIns,
				new Rectangle(LayoutManager.Scale(3),LayoutManager.Scale(32),
				tabPageFinancial.ClientSize.Width-LayoutManager.Scale(4),LayoutManager.Scale(102)));
			//tried to do relative to gridPay, but that was unreliable
			LayoutManager.Move(gridAdj,new Rectangle(LayoutManager.Scale(458),LayoutManager.Scale(137),
				tabPageFinancial.ClientSize.Width-LayoutManager.Scale(458)-LayoutManager.Scale(1),
				tabPageFinancial.ClientSize.Height-LayoutManager.Scale(137)-LayoutManager.Scale(5)));
		}

		private void FillComboClinic() {
			long clinicNum=comboClinic.SelectedClinicNum;
			comboClinic.SetUser(_curUser);//Not Security.CurUser
			if(clinicNum!=-1){
				comboClinic.SelectedClinicNum=clinicNum;
			}
		}

		private void ComboClinic_SelectionChangeCommitted(object sender, EventArgs e){
			FillComboProv();
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			using FormProviderPick formp = new FormProviderPick(comboProv.Items.GetAll<Provider>());
			formp.SelectedProvNum=comboProv.GetSelectedProvNum();
			formp.ShowDialog();
			if(formp.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProv.SetSelectedProvNum(formp.SelectedProvNum);
		}

		private void butPickOrderProvInternal_Click(object sender,EventArgs e) {
			using FormProviderPick formP = new FormProviderPick(comboProv.Items.GetAll<Provider>());
			formP.SelectedProvNum=_selectedProvOrderNum;
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
				_selectedProvOrderNum=0;
				textOrderingProviderOverride.Text="";
			}
			else {
				_selectedProvOrderNum=prov.ProvNum;
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
			_selectedProvOrderNum=0;
		}

		///<summary>Fills combo provider based on which clinic is selected and attempts to preserve provider selection if any.</summary>
		private void FillComboProv() {
			long provNum=comboProv.GetSelectedProvNum();
			comboProv.Items.Clear();
			comboProv.Items.AddProvsAbbr(Providers.GetProvsForClinic(comboClinic.SelectedClinicNum));
			comboProv.SetSelectedProvNum(provNum);
		}

		///<summary>ONLY run on startup. Fills the basic controls, except not the ones in the upper left panel which are handled in SetControlsUpperLeft.</summary>
		private void FillControlsOnStartup(){
			if(_procedure.ProcStatus==ProcStat.D && _procedure.IsLocked) {//only set this when coming in with this status. 
				comboProcStatus.Items.Add(Lan.g("Procedures","Invalidated"),ProcStat.D);
			}
			else{
				comboProcStatus.Items.Add(Lan.g("Procedures","Treatment Planned"),ProcStat.TP);
				//For the "Complete" option, instead of showing current value,
				//show what the value would represent if set to complete, in case user changes to complete from another status.
				bool isInProcess=ProcMultiVisits.IsProcInProcess(_procedure.ProcNum,true);
				comboProcStatus.Items.Add(Lan.g("Procedures","Complete"+(isInProcess?" (In Process)":"")),ProcStat.C);
				if(!PrefC.GetBool(PrefName.EasyHideClinical)) {
					comboProcStatus.Items.Add(Lan.g("Procedures","Existing-Current Prov"),ProcStat.EC);
					comboProcStatus.Items.Add(Lan.g("Procedures","Existing-Other Prov"),ProcStat.EO);
					comboProcStatus.Items.Add(Lan.g("Procedures","Referred Out"),ProcStat.R);
					comboProcStatus.Items.Add(Lan.g("Procedures","Condition"),ProcStat.Cn);
				}
				if(_procedure.ProcStatus==ProcStat.TPi) {//only set this when coming in with that status, users should not choose this status otherwise.
					comboProcStatus.Items.Add(Lan.g("Procedures","Treatment Planned Inactive"),ProcStat.TPi);
				}
			}
			comboProcStatus.SetSelectedEnum(_procedure.ProcStatus);//has no effect if enum isn't in the list
			if(comboProcStatus.GetSelected<ProcStat>()==ProcStat.TPi) {
				comboProcStatus.Enabled=false;
				butSetComplete.Enabled=false;
			}
			if(comboProcStatus.GetSelected<ProcStat>()==ProcStat.D){//an invalidated proc
				comboProcStatus.Enabled=false;
				butInvalidate.Visible=false;
				butOK.Enabled=false;
				butDelete.Enabled=false;
				butChange.Enabled=false;
				butEditAnyway.Enabled=false;
				butSetComplete.Enabled=false;
				butAddEstimate.Enabled=false;
				butAddAdjust.Enabled=false;
			}
			//if clinical is hidden, then there's a chance that no item is selected at this point.
			_listDiagnosisDefs=Defs.GetDefsForCategory(DefCat.Diagnosis,true);
			_listPrognosisDefs=Defs.GetDefsForCategory(DefCat.Prognosis,true);
			_listTxPriorityDefs=Defs.GetDefsForCategory(DefCat.TxPriorities,true);
			_listBillingTypeDefs=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			comboDx.Items.Clear();
			for(int i=0;i<_listDiagnosisDefs.Count;i++){
				comboDx.Items.Add(_listDiagnosisDefs[i].ItemName);
				if(_listDiagnosisDefs[i].DefNum==_procedure.Dx)
					comboDx.SelectedIndex=i;
			}
			comboPrognosis.Items.Clear();
			comboPrognosis.Items.Add(Lan.g(this,"no prognosis"));
			comboPrognosis.SelectedIndex=0;
			for(int i=0;i<_listPrognosisDefs.Count;i++) {
				comboPrognosis.Items.Add(_listPrognosisDefs[i].ItemName);
				if(_listPrognosisDefs[i].DefNum==_procedure.Prognosis)
					comboPrognosis.SelectedIndex=i+1;
			}
			checkHideGraphics.Checked=_procedure.HideGraphics;
			comboPriority.Items.Clear();
			comboPriority.Items.Add(Lan.g(this,"no priority"));
			comboPriority.SelectedIndex=0;
			for(int i=0;i<_listTxPriorityDefs.Count;i++){
				comboPriority.Items.Add(_listTxPriorityDefs[i].ItemName);
				if(_listTxPriorityDefs[i].DefNum==_procedure.Priority)
					comboPriority.SelectedIndex=i+1;
			}
			comboBillingTypeOne.Items.Clear();
			comboBillingTypeOne.Items.Add(Lan.g(this,"none"));
			comboBillingTypeOne.SelectedIndex=0;
			for(int i=0;i<_listBillingTypeDefs.Count;i++) {
				comboBillingTypeOne.Items.Add(_listBillingTypeDefs[i].ItemName);
				if(_listBillingTypeDefs[i].DefNum==_procedure.BillingTypeOne)
					comboBillingTypeOne.SelectedIndex=i+1;
			}
			comboBillingTypeTwo.Items.Clear();
			comboBillingTypeTwo.Items.Add(Lan.g(this,"none"));
			comboBillingTypeTwo.SelectedIndex=0;
			for(int i=0;i<_listBillingTypeDefs.Count;i++) {
				comboBillingTypeTwo.Items.Add(_listBillingTypeDefs[i].ItemName);
				if(_listBillingTypeDefs[i].DefNum==_procedure.BillingTypeTwo)
					comboBillingTypeTwo.SelectedIndex=i+1;
			}
			textBillingNote.Text=_procedure.BillingNote;
			textNotes.Text=_procedure.Note;
			comboPlaceService.Items.Clear();
			comboPlaceService.Items.AddRange(Enum.GetNames(typeof(PlaceOfService)));
			comboPlaceService.SelectedIndex=(int)_procedure.PlaceService;
			//checkHideGraphical.Checked=ProcCur.HideGraphical;
			textSite.Text=Sites.GetDescription(_procedure.SiteNum);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				if(_procedure.CanadianTypeCodes==null || _procedure.CanadianTypeCodes=="") {
					checkTypeCodeX.Checked=true;
				}
				else {
					if(_procedure.CanadianTypeCodes.Contains("A")) {
						checkTypeCodeA.Checked=true;
					}
					if(_procedure.CanadianTypeCodes.Contains("B")) {
						checkTypeCodeB.Checked=true;
					}
					if(_procedure.CanadianTypeCodes.Contains("C")) {
						checkTypeCodeC.Checked=true;
					}
					if(_procedure.CanadianTypeCodes.Contains("E")) {
						checkTypeCodeE.Checked=true;
					}
					if(_procedure.CanadianTypeCodes.Contains("L")) {
						checkTypeCodeL.Checked=true;
					}
					if(_procedure.CanadianTypeCodes.Contains("S")) {
						checkTypeCodeS.Checked=true;
					}
					if(_procedure.CanadianTypeCodes.Contains("X")) {
						checkTypeCodeX.Checked=true;
					}
				}
			}
			else{
				if(_procedureCode.IsProsth){
					listProsth.Items.Add(Lan.g(this,"No"));
					listProsth.Items.Add(Lan.g(this,"Initial"));
					listProsth.Items.Add(Lan.g(this,"Replacement"));
					switch(_procedure.Prosthesis){
						case "":
							listProsth.SelectedIndex=0;
							break;
						case "I":
							listProsth.SelectedIndex=1;
							break;
						case "R":
							listProsth.SelectedIndex=2;
							break;
					}
					if(_procedure.DateOriginalProsth.Year>1880){
						textDateOriginalProsth.Text=_procedure.DateOriginalProsth.ToShortDateString();
					}
					checkIsDateProsthEst.Checked=_procedure.IsDateProsthEst;
				}
				else{
					groupProsth.Visible=false;
				}
			}
			textDiscount.Text=_procedure.Discount.ToString("f");
			//medical
			textMedicalCode.Text=_procedure.MedicalCode;
			if(_procedure.IcdVersion==9) {
				checkIcdVersion.Checked=false;
			}
			else {//ICD-10
				checkIcdVersion.Checked=true;
			}
			SetIcdLabels();
			textDiagnosisCode.Text=_procedure.DiagnosticCode;
			textDiagnosisCode2.Text=_procedure.DiagnosticCode2;
			textDiagnosisCode3.Text=_procedure.DiagnosticCode3;
			textDiagnosisCode4.Text=_procedure.DiagnosticCode4;
			checkIsPrincDiag.Checked=_procedure.IsPrincDiag;
			textCodeMod1.Text = _procedure.CodeMod1;
			textCodeMod2.Text = _procedure.CodeMod2;
			textCodeMod3.Text = _procedure.CodeMod3;
			textCodeMod4.Text = _procedure.CodeMod4;
			textUnitQty.Text = _procedure.UnitQty.ToString();
			comboUnitType.Items.Clear();
			_snomedBodySite=Snomeds.GetByCode(_procedure.SnomedBodySite);
			if(_snomedBodySite==null) {
				textSnomedBodySite.Text="";
			}
			else {
				textSnomedBodySite.Text=_snomedBodySite.Description;
			}
			for(int i=0;i<Enum.GetNames(typeof(ProcUnitQtyType)).Length;i++) {
				comboUnitType.Items.Add(Enum.GetNames(typeof(ProcUnitQtyType))[i]);
			}
			comboUnitType.SelectedIndex=(int)_procedure.UnitQtyType;
			textRevCode.Text = _procedure.RevCode;
			//DrugNDC is handled in SetControlsUpperLeft
			comboDrugUnit.Items.Clear();
			for(int i=0;i<Enum.GetNames(typeof(EnumProcDrugUnit)).Length;i++){
				comboDrugUnit.Items.Add(Enum.GetNames(typeof(EnumProcDrugUnit))[i]);
			}
			comboDrugUnit.SelectedIndex=(int)_procedure.DrugUnit;
			if(_procedure.DrugQty!=0){
				textDrugQty.Text=_procedure.DrugQty.ToString();
			}
			checkIsEmergency.Checked=(_procedure.Urgency==ProcUrgency.Emergency);
			textClaimNote.Text=_procedure.ClaimNote;
			textUser.Text=Userods.GetName(_procedure.UserNum);//might be blank. Will change automatically if user changes note or alters sig.
			string keyData=GetSignatureKey();
			signatureBoxWrapper.FillSignature(_procedure.SigIsTopaz,keyData,_procedure.Signature);
			Plugins.HookAddCode(this,"FormProcEdit.FillControlsOnStartup_end",_procedure,_procedureOld);
		}

		private void FormProcEdit_Shown(object sender,EventArgs e) {
			//Prompt users for auto notes if they have the preference set.
			if(PrefC.GetBool(PrefName.ProcPromptForAutoNote)) {//Replace [[text]] sections within the note with AutoNotes.
				PromptForAutoNotes();
			}
			//Scroll to the end of the note for procedures for today (or completed today).
			if(_procedure.DateEntryC.Date==DateTime.Today) {
				textNotes.Select(textNotes.Text.Length,0);
			}
			CheckForCompleteNote();
		}

		///<summary>Loops through textNotes.Text and will insert auto notes and prompt them for prompting auto notes.</summary>
		private void PromptForAutoNotes() {
			List<Match> listMatches=Regex.Matches(textNotes.Text,@"\[\[.+?\]\]").OfType<Match>().ToList();
			listMatches.RemoveAll(x => AutoNotes.GetByTitle(x.Value.TrimStart('[').TrimEnd(']'))=="");//remove matches that are not autonotes.
			int loc=0;
			foreach(Match match in listMatches) {
				string autoNoteTitle=match.Value.TrimStart('[').TrimEnd(']');
				string note=AutoNotes.GetByTitle(autoNoteTitle);
				int matchloc=textNotes.Text.IndexOf(match.Value,loc);
				using FormAutoNoteCompose FormA=new FormAutoNoteCompose();
				FormA.MainTextNote=note;
				FormA.ShowDialog();
				if(FormA.DialogResult==DialogResult.Cancel) {
					loc=matchloc+match.Value.Length;
					continue;//if they cancel, go to the next autonote.
				}
				if(FormA.DialogResult==DialogResult.OK) {
					//When setting the Text on a RichTextBox, \r\n is replaced with \n, so we need to do the same so that our location variable is correct.
					loc=matchloc+FormA.CompletedNote.Replace("\r\n","\n").Length;
					string resultstr=textNotes.Text.Substring(0,matchloc)+FormA.CompletedNote;
					if(textNotes.Text.Length > matchloc+match.Value.Length) {
						resultstr+=textNotes.Text.Substring(matchloc+match.Value.Length);
					}
					textNotes.Text=resultstr;
				}
			}
			bool hasAutoNotePrompt=Regex.IsMatch(textNotes.Text,_autoNotePromptRegex);
			butEditAutoNote.Visible=hasAutoNotePrompt;
		}

		private string GetSignatureKey() {
			string keyData=_procedure.Note;
			keyData+=_procedure.UserNum.ToString();
			keyData=keyData.Replace("\r\n","\n");//We need all newlines to be the same, a mix of /r/n and /n can invalidate the procedure signature.
			return keyData;
		}

		private void SetSurfButtons(){
			butBF.BackColor=(textSurfaces.Text.Contains("B") || textSurfaces.Text.Contains("F"))?Color.White:SystemColors.Control;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				if(textSurfaces.Text.Contains("V")) butBF.BackColor=Color.White;
			}
			butOI.BackColor=(textSurfaces.Text.Contains("O") || textSurfaces.Text.Contains("I"))?Color.White:SystemColors.Control;
			butM.BackColor=textSurfaces.Text.Contains("M")?Color.White:SystemColors.Control;
			butD.BackColor=textSurfaces.Text.Contains("D")?Color.White:SystemColors.Control;
			butL.BackColor=textSurfaces.Text.Contains("L")?Color.White:SystemColors.Control;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				butV.BackColor=textSurfaces.Text.Contains("5")?Color.White:SystemColors.Control;
			}
			else{
				butV.BackColor=textSurfaces.Text.Contains("V")?Color.White:SystemColors.Control;
			}
		}

		///<summary>Called on open and after changing code.  Sets the visibilities and the data of all the fields in the upper left panel.</summary>
		private void SetControlsUpperLeft(){
			textDateTP.Text=_procedure.DateTP.ToString("d");
			if(_procedure.DateTP.Year<1880) {
				textDateTP.Text="";
			}
			DateTime dateT;
			if(_isStartingUp){
				if(_procedure.ProcDate.Year>=1880) {
					textDate.Text=_procedure.ProcDate.ToString("d");
				}
				if(_procedure.ProcDate.Date!=_procedure.DateComplete.Date && _procedure.DateComplete.Year>1880) {
					//show proc date Original if the date is different than proc date and set.
					labelOrigDateComp.Visible=true;
					textOrigDateComp.Visible=true;
					textOrigDateComp.Text=_procedure.DateComplete.ToString("d");
				}
				else {//Hide Orig Date Comp if same as current procedure date.
					labelOrigDateComp.Visible=false;
					textOrigDateComp.Visible=false;
				}
				dateT=PIn.DateT(_procedure.ProcTime.ToString());
				if(dateT.ToShortTimeString()!="12:00 AM"){
					textTimeStart.Text+=dateT.ToShortTimeString();
				}
				if(PrefC.GetBool(PrefName.ShowFeatureMedicalInsurance)) {
					dateT=PIn.DateT(_procedure.ProcTimeEnd.ToString());
					if(dateT.ToShortTimeString()!="12:00 AM") {
						textTimeEnd.Text=dateT.ToShortTimeString();
					}
					UpdateFinalMin();			
				}
			}
			textProc.Text=_procedureCode.ProcCode;
			textDesc.Text=_procedureCode.Descript;
			textDrugNDC.Text=_procedureCode.DrugNDC;
			if(_procedureCode.TreatArea==TreatmentArea.Surf){
				textTooth.Visible=true;
				labelTooth.Visible=true;
				textSurfaces.Visible=true;
				labelSurfaces.Visible=true;
				panelSurfaces.Visible=true;
				if(Tooth.IsValidDB(_procedure.ToothNum)) {
					errorProvider2.SetError(textTooth,"");
					textTooth.Text=Tooth.ToInternat(_procedure.ToothNum);
					textSurfaces.Text=Tooth.SurfTidyFromDbToDisplay(_procedure.Surf,_procedure.ToothNum);
					SetSurfButtons();
				}
				else{
					errorProvider2.SetError(textTooth,Lan.g(this,"Invalid tooth number."));
					textTooth.Text=_procedure.ToothNum;
					//textSurfaces.Text=Tooth.SurfTidy(ProcCur.Surf,"");//only valid toothnums allowed
				}
				if(textSurfaces.Text==""){
					errorProvider2.SetError(textSurfaces,"No surfaces selected.");
				}
				else{
					errorProvider2.SetError(textSurfaces,"");
				}
			}
			if(_procedureCode.TreatArea==TreatmentArea.Tooth){
				textTooth.Visible=true;
				labelTooth.Visible=true;
				if(Tooth.IsValidDB(_procedure.ToothNum)){
					errorProvider2.SetError(textTooth,"");
					textTooth.Text=Tooth.ToInternat(_procedure.ToothNum);
				}
				else{
					errorProvider2.SetError(textTooth,Lan.g(this,"Invalid tooth number."));
					textTooth.Text=_procedure.ToothNum;
				}
			}
			if(_procedureCode.TreatArea==TreatmentArea.Quad){
				groupQuadrant.Visible=true;
				switch (_procedure.Surf){
					case "UR": this.radioUR.Checked=true; break;
					case "UL": this.radioUL.Checked=true; break;
					case "LR": this.radioLR.Checked=true; break;
					case "LL": this.radioLL.Checked=true; break;
					//default : 
				}
			}
			if(_procedureCode.TreatArea==TreatmentArea.Sextant){
				groupSextant.Visible=true;
				switch (_procedure.Surf){
					case "1": this.radioS1.Checked=true; break;
					case "2": this.radioS2.Checked=true; break;
					case "3": this.radioS3.Checked=true; break;
					case "4": this.radioS4.Checked=true; break;
					case "5": this.radioS5.Checked=true; break;
					case "6": this.radioS6.Checked=true; break;
					//default:
				}
				if(IsSextantSelected()) {
					errorProvider2.SetError(groupSextant,"");
				}
				else {
					errorProvider2.SetError(groupSextant,Lan.g(this,"Please select a sextant treatment area."));
				}
			}
			if(_procedureCode.TreatArea==TreatmentArea.Arch){
				groupArch.Visible=true;
				switch (_procedure.Surf){
					case "U": this.radioU.Checked=true; break;
					case "L": this.radioL.Checked=true; break;
				}
				if(IsArchSelected()) {
					errorProvider2.SetError(groupArch,"");
				}
				else {
					errorProvider2.SetError(groupArch,Lan.g(this,"Please select an arch treatment area."));
				}
			}
			if(_procedureCode.TreatArea==TreatmentArea.ToothRange
				|| _procedureCode.AreaAlsoToothRange)
			{
				labelRange.Visible=true;
				listBoxTeeth.Visible=true;
				listBoxTeeth.ColumnWidth=LayoutManager.Scale(16);
				listBoxTeeth2.Visible=true;
				listBoxTeeth2.ColumnWidth=LayoutManager.Scale(16);
				listBoxTeeth.SelectionMode=System.Windows.Forms.SelectionMode.MultiExtended;
				listBoxTeeth2.SelectionMode=System.Windows.Forms.SelectionMode.MultiExtended;
				if(_listPatToothInitials==null) {
					_listPatToothInitials=ToothInitials.Refresh(_patient.PatNum);
				}
				//First add teeth flagged as primary teeth for this specific patient from the Chart into _listPriTeeth.
				_listPriTeeth=ToothInitials.GetPriTeeth(_listPatToothInitials);
				//Preserve tooth range history for this procedure by ensuring that the UI shows the values from the database for the relevant teeth.
				string[] arrayProcToothNums=new string[0];
				if(!string.IsNullOrWhiteSpace(_procedure.ToothRange)){
					arrayProcToothNums=_procedure.ToothRange.Split(',');//in Universal (American) nomenclature
				}
				foreach(string procToothNum in arrayProcToothNums) {
					if(Tooth.IsPrimary(procToothNum)) {
						_listPriTeeth.Add(Tooth.ToInt(procToothNum).ToString());//Convert the primary tooth to a permanent tooth.
					}
					else {//Permanent tooth
						_listPriTeeth.Remove(procToothNum);//Preserve permanent tooth history.
					}
				}
				//Fill Maxillary/Upper Arch
				listBoxTeeth.Items.Clear();
				for(int toothNum=1;toothNum<=16;toothNum++) {
					string toothId=toothNum.ToString();
					if(_listPriTeeth.Contains(toothNum.ToString())) {//Is Primary
						toothId=Tooth.PermToPri(toothId);
					}
					listBoxTeeth.Items.Add(Tooth.GetToothLabelGraphic(toothId));//Display tooth is dependent on nomenclature preference.
				}
				//Fill Mandibular/Lower	Arch
				listBoxTeeth2.Items.Clear();
				for(int toothNum=32;toothNum>=17;toothNum--) {
					string toothId=toothNum.ToString();
					if(_listPriTeeth.Contains(toothNum.ToString())) {//Is Primary
						toothId=Tooth.PermToPri(toothId);
					}
					listBoxTeeth2.Items.Add(Tooth.GetToothLabelGraphic(toothId));//Display tooth is dependent on nomenclature preference.
				}
				//Select tooth numbers in each arch depending on the database data stored in the procedure ToothRange.
				foreach(string toothNum in arrayProcToothNums) {
					if(Tooth.IsMaxillary(toothNum)) {//Works for primary or permanent tooth numbers.
						int toothIndex=Tooth.ToInt(toothNum)-1;//Works for primary or permanent tooth numbers.
						listBoxTeeth.SetSelected(toothIndex,true);
					}
					else {//Mandibular
						int toothIndex=32-Tooth.ToInt(toothNum);//Works for primary or permanent tooth numbers.
						if(toothIndex<0) {
							//Tooth Range could be 3 to 4 digits (outside of range).  Split the numbers in order to select the correct teeth.
							toothIndex=Tooth.ToInt(toothNum.Remove(toothNum.Length-2))-1;
							int toothIndex2=32-Tooth.ToInt(toothNum.Substring(toothNum.Length-2,2));
							listBoxTeeth.SetSelected(toothIndex,true);
							listBoxTeeth2.SetSelected(toothIndex2,true);
						}
						else {
							listBoxTeeth2.SetSelected(toothIndex,true);
						}
					}
				}//foreach
			}//if toothrange
			textProcFee.Text=_procedure.ProcFee.ToString("n");
		}

		/// <summary>Takes in a list of controls and loops through to disable the necesary components</summary>
		private void SetControlsDisabled(List<Control> listControls) {
			if(listControls.IsNullOrEmpty()) {
				return;
			}
			foreach(Control control in listControls) {
				if(control is TextBoxBase) {
					((TextBoxBase)control).ReadOnly=true;
					control.BackColor=SystemColors.Control;
				}
				else if(control is UI.Button
					|| control is ComboBox 
					|| control is CheckBox 
					|| control is GroupBox 
					|| control is Panel 
					|| control is SignatureBoxWrapper 
					|| control is ComboBoxOD 
					|| control is ComboBoxClinicPicker) 
				{
					control.Enabled=false;
				}
			}
		}

		///<summary>Enable/disable controls based on permissions for a completed procedures.</summary>
		private void SetControlsEnabled(bool isSilent) {
			//Return if the current procedure isn't considered complete (C, EC, EO).
			//Don't allow adding an estimate, since a new estimate could change the total writeoff amount for the proc.
			if(!ListTools.In(_procedure.ProcStatus,ProcStat.C,ProcStat.EO,ProcStat.EC)) {
				return;
			}
			DateTime dateForPerm=Procedures.GetDateForPermCheck(_procedure);//Use ProcDate to compare to the date/days newer restriction.
			bool isProcStatComplete=_procedure.ProcStatus==ProcStat.C;
			Permissions perm=GroupPermissions.SwitchExistingPermissionIfNeeded(Permissions.ProcCompleteEdit,_procedure);
			#region Setting up dictionary for removing controls
			Dictionary<Permissions,List<Control>> dictPermControls=new Dictionary<Permissions, List<Control>>();
			dictPermControls[Permissions.ProcCompleteEdit]=new List<Control> {
				butChange,textReferral,butReferral,comboPriority,butAddEstimate,butAddExistAdj,comboProv,butPickProv,comboClinic
			};
			dictPermControls[Permissions.ProcCompleteEdit].AddRange(new List<Control> { panel1,tabPageCanada }.SelectMany(x => x.Controls.OfType<Control>()));
			dictPermControls[Permissions.ProcCompleteStatusEdit]=new List<Control> {comboProcStatus,butSetComplete,butDelete };
			dictPermControls[Permissions.ProcCompleteNote]=new List<Control> {
				textNotes,signatureBoxWrapper,buttonUseAutoNote,butEditAutoNote,textClaimNote,butAppend,textProc
			};
			dictPermControls[Permissions.ProcCompleteAddAdj]=new List<Control> { butAddAdjust };
			dictPermControls[Permissions.ProcCompleteEditMisc]=new List<Control> { checkHideGraphics,checkNoBillIns,comboClinic,textSurfaces,comboDx };
			dictPermControls[Permissions.ProcCompleteEditMisc].AddRange(new List<TabPage>{tabPageMisc,tabPageMedical}.SelectMany(x => x.Controls.OfType<Control>()));
			#endregion
			List<Control> listDisabled=new List<Control>();
			bool isGlobalDateLocked=Security.IsGlobalDateLock(perm,dateForPerm,isSilent);//only used to silence other security messages.
			if(!isProcStatComplete) {//either Eo or Ec
				if(!Security.IsAuthorized(perm,dateForPerm,isSilent,isGlobalDateLocked)) {
					listDisabled.AddRange(dictPermControls.Values.SelectMany(x => x));
					listDisabled.Add(butOK);
				}
			}
			else {
				bool isSuppressed=(!isProcStatComplete||isSilent||isGlobalDateLocked);//don't want a bunch of popups in a row so suppressing the message in checking permissions
				foreach(Permissions permission in dictPermControls.Keys) {
					bool isAuthorized=Security.IsAuthorized(permission,dateForPerm,isSuppressed,isGlobalDateLocked);
					if(!isAuthorized) {
						listDisabled.AddRange(dictPermControls[permission]);
					}
					isSuppressed=(!isAuthorized||!isProcStatComplete);//only first permission not afforded to a user gets a warning message
				}
				//disable 'OK' if user has no completed procedure permissions
				if(dictPermControls.Keys.All(x => !Security.IsAuthorized(x,dateForPerm,true))) {
					butOK.Enabled=false;
				}
			}
			SetControlsDisabled(listDisabled);
			if(!Security.IsAuthorized(perm,dateForPerm,true,true)
				&& Security.IsAuthorized(perm,dateForPerm,true,true,_procedure.CodeNum,_procedure.ProcFee,0,0)) 
			{
				//This is a $0 procedure for a proc code marked as bypassed.
				butDelete.Enabled=true;
			}
		}//end SetControls

		private void FillReferral(bool doRefreshData=true) {
			if(doRefreshData) {
				_loadData.ListRefAttaches=RefAttaches.RefreshFiltered(_procedure.PatNum,false,_procedure.ProcNum);
			}
			List<RefAttach> refsList=_loadData.ListRefAttaches;
			if(refsList.Count==0) {
				textReferral.Text="";
			}
			else {
				Referral referral;
				if(Referrals.TryGetReferral(refsList[0].ReferralNum,out referral)) {
					textReferral.Text=referral.LName+", ";
				}
				if(refsList[0].DateProcComplete.Year<1880) {
					textReferral.Text+=refsList[0].RefDate.ToShortDateString();
				}
				else{
					textReferral.Text+=Lan.g(this,"done:")+refsList[0].DateProcComplete.ToShortDateString();
				}
				if(refsList[0].RefToStatus!=ReferralToStatus.None){
					textReferral.Text+=refsList[0].RefToStatus.ToString();
				}
			}
		}

		private void butReferral_Click(object sender,EventArgs e) {
			using FormReferralsPatient FormRP=new FormReferralsPatient();
			FormRP.PatNum=_procedure.PatNum;
			FormRP.ProcNum=_procedure.ProcNum;
			FormRP.ShowDialog();
			FillReferral();
		}

		private void FillIns(){
			FillIns(true);
		}

		private void FillIns(bool refreshClaimProcsFirst){
			if(refreshClaimProcsFirst) {
				//ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
				//}
				_listClaimProcsForProc=ClaimProcs.RefreshForProc(_procedure.ProcNum);
			}
			gridIns.BeginUpdate();
			gridIns.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableProcIns","Ins Plan"),190);
			gridIns.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","Pri/Sec"),50,HorizontalAlignment.Center);
			gridIns.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","Status"),50,HorizontalAlignment.Center);
			gridIns.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","NoBill"),45,HorizontalAlignment.Center);
			gridIns.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","Copay"),55,HorizontalAlignment.Right);
			gridIns.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","Deduct"),55,HorizontalAlignment.Right);
			gridIns.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","Percent"),55,HorizontalAlignment.Center);
			gridIns.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","Ins Est"),55,HorizontalAlignment.Right);
			gridIns.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","Ins Pay"),55,HorizontalAlignment.Right);
			gridIns.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","WriteOff"),55,HorizontalAlignment.Right);
			gridIns.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","Estimate Note"),100);
			gridIns.ListGridColumns.Add(col);		 
			col=new GridColumn(Lan.g("TableProcIns","Remarks"),165);
			gridIns.ListGridColumns.Add(col);		 
			gridIns.ListGridRows.Clear();
			GridRow row;
			checkNoBillIns.CheckState=CheckState.Unchecked;
			bool allNoBillIns=true;
			InsPlan plan;
			//ODGridCell cell;
			for(int i=0;i<_listClaimProcsForProc.Count;i++) {
				if(_listClaimProcsForProc[i].NoBillIns){
					checkNoBillIns.CheckState=CheckState.Indeterminate;
				}
				else{
					allNoBillIns=false;
				}
				row=new GridRow();
				row.Cells.Add(InsPlans.GetDescript(_listClaimProcsForProc[i].PlanNum,_family,_listInsPlans,_listClaimProcsForProc[i].InsSubNum,_listInsSubs));
				plan=InsPlans.GetPlan(_listClaimProcsForProc[i].PlanNum,_listInsPlans);
				if(plan==null) {
					MsgBox.Show(this,"No insurance plan exists for this claim proc.  Please run database maintenance.");
					return;
				}
				if(plan.IsMedical) {
					row.Cells.Add("Med");
				}
				else if(_listClaimProcsForProc[i].InsSubNum==PatPlans.GetInsSubNum(_listPatPlans,PatPlans.GetOrdinal(PriSecMed.Primary,_listPatPlans,_listInsPlans,_listInsSubs))){
					row.Cells.Add("Pri");
				}
				else if(_listClaimProcsForProc[i].InsSubNum==PatPlans.GetInsSubNum(_listPatPlans,PatPlans.GetOrdinal(PriSecMed.Secondary,_listPatPlans,_listInsPlans,_listInsSubs))) {
					row.Cells.Add("Sec");
				}
				else {
					row.Cells.Add("");
				}
				switch(_listClaimProcsForProc[i].Status) {
					case ClaimProcStatus.Received:
						row.Cells.Add("Recd");
						break;
					case ClaimProcStatus.NotReceived:
						row.Cells.Add("NotRec");
						break;
					//adjustment would never show here
					case ClaimProcStatus.Preauth:
						row.Cells.Add("PreA");
						break;
					case ClaimProcStatus.Supplemental:
						row.Cells.Add("Supp");
						break;
					case ClaimProcStatus.CapClaim:
						row.Cells.Add("CapClaim");
						break;
					case ClaimProcStatus.Estimate:
						row.Cells.Add("Est");
						break;
					case ClaimProcStatus.CapEstimate:
						row.Cells.Add("CapEst");
						break;
					case ClaimProcStatus.CapComplete:
						row.Cells.Add("CapComp");
						break;
					case ClaimProcStatus.InsHist:
						row.Cells.Add("InsHist");
						break;
					default:
						row.Cells.Add("");
						break;
				}
				if(_listClaimProcsForProc[i].NoBillIns) {
					row.Cells.Add("X");
					if(!ListTools.In(_listClaimProcsForProc[i].Status,ClaimProcStatus.CapComplete,ClaimProcStatus.CapEstimate)) {
						row.Cells.Add("");
						row.Cells.Add("");
						row.Cells.Add("");
						row.Cells.Add("");
						row.Cells.Add("");
						row.Cells.Add("");
						row.Cells.Add("");
						row.Cells.Add("");
						row.Cells.Add("");
						gridIns.ListGridRows.Add(row);
						continue;
					}
				}
				else {
					row.Cells.Add("");
				}
				row.Cells.Add(ClaimProcs.GetCopayDisplay(_listClaimProcsForProc[i]));
				double ded=ClaimProcs.GetDeductibleDisplay(_listClaimProcsForProc[i]);
				if(ded>0) {
					row.Cells.Add(ded.ToString("n"));
				}
				else {
					row.Cells.Add("");
				}
				row.Cells.Add(ClaimProcs.GetPercentageDisplay(_listClaimProcsForProc[i]));
				row.Cells.Add(ClaimProcs.GetEstimateDisplay(_listClaimProcsForProc[i]));
				if(_listClaimProcsForProc[i].Status==ClaimProcStatus.Estimate
					|| _listClaimProcsForProc[i].Status==ClaimProcStatus.CapEstimate) 
				{
					row.Cells.Add("");
					row.Cells.Add(ClaimProcs.GetWriteOffEstimateDisplay(_listClaimProcsForProc[i]));
				}
				else {
					row.Cells.Add(_listClaimProcsForProc[i].InsPayAmt.ToString("n"));
					row.Cells.Add(_listClaimProcsForProc[i].WriteOff.ToString("n"));
				}
				row.Cells.Add(_listClaimProcsForProc[i].EstimateNote);
				row.Cells.Add(_listClaimProcsForProc[i].Remarks);			  
				gridIns.ListGridRows.Add(row);
			}
			gridIns.EndUpdate();
			if(_listClaimProcsForProc.Count==0) {
				checkNoBillIns.CheckState=CheckState.Unchecked;
			}
			else if(allNoBillIns) {
				checkNoBillIns.CheckState=CheckState.Checked;
			}
		}

		private void gridIns_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormClaimProc FormC=new FormClaimProc(_listClaimProcsForProc[e.Row],_procedure,_family,_patient,_listInsPlans,ListClaimProcHists,ref ListClaimProcHistsLoop,_listPatPlans,true,_listInsSubs);
			if(_procedure.IsLocked || !Procedures.IsProcComplEditAuthorized(_procedure)) {
				FormC.NoPermissionProc=true;
			}
			FormC.ShowDialog();
			FillIns();
		}

		void butNow_Click(object sender,EventArgs e) {
			if(textTimeStart.Text.Trim()=="") {
				textTimeStart.Text=MiscData.GetNowDateTime().ToShortTimeString();
			}
			else {
				textTimeEnd.Text=MiscData.GetNowDateTime().ToShortTimeString();
			}
		}

		private void butAddEstimate_Click(object sender, System.EventArgs e) {
			if(_procedure.ProcNumLab!=0) {
				MsgBox.Show(this,"Estimates cannot be added directly to labs.  Lab estimates will be created automatically when the parent procedure estimates are calculated.");
				return;
			}
			if(!Security.IsAuthorized(Permissions.InsWriteOffEdit,_procedure.DateEntryC)) {
				return;
			}
			using FormInsPlanSelect FormIS=new FormInsPlanSelect(_patient.PatNum);
			if(FormIS.SelectedPlan==null) {
				FormIS.ShowDialog();
				if(FormIS.DialogResult==DialogResult.Cancel){
					return;
				}
			}
			InsPlan plan=FormIS.SelectedPlan;
			InsSub sub=FormIS.SelectedSub;
			_listClaimProcsForProc=ClaimProcs.RefreshForProc(_procedure.ProcNum);
			ClaimProc claimProcForProcInsPlan=_listClaimProcsForProc
				.Where(x => x.PlanNum == plan.PlanNum)
				.Where(x => x.Status != ClaimProcStatus.Preauth)
				.FirstOrDefault();
			ClaimProc cp = new ClaimProc();
			BlueBookEstimateData blueBookEstimateData=new BlueBookEstimateData(_listInsPlans,_listInsSubs,_listPatPlans,new List<Procedure>{_procedure},ListSubstLinks);
			cp.IsNew=true;
			if(claimProcForProcInsPlan!=null) {
				cp = claimProcForProcInsPlan;
				cp.IsNew=false;
			}
			else {
				List<Benefit> benList = Benefits.Refresh(_listPatPlans,_listInsSubs);
				ClaimProcs.CreateEst(cp,_procedure,plan,sub);
				if(plan.PlanType=="c") {//capitation
					double allowed = PIn.Double(textProcFee.Text);
					cp.BaseEst=allowed;
					cp.InsEstTotal=allowed;
					cp.CopayAmt=InsPlans.GetCopay(_procedure.CodeNum,plan.FeeSched,plan.CopayFeeSched,
						!SubstitutionLinks.HasSubstCodeForPlan(plan,_procedure.CodeNum,ListSubstLinks),_procedure.ToothNum,_procedure.ClinicNum,_procedure.ProvNum,plan.PlanNum,
						ListSubstLinks,LookupFees);
					if(cp.CopayAmt > allowed) {//if the copay is greater than the allowed fee calculated above
						cp.CopayAmt=allowed;//reduce the copay
					}
					if(cp.CopayAmt==-1) {
						cp.CopayAmt=0;
					}
					cp.WriteOffEst=cp.BaseEst-cp.CopayAmt;
					if(cp.WriteOffEst<0) {
						cp.WriteOffEst=0;
					}
					cp.WriteOff=cp.WriteOffEst;
					ClaimProcs.Update(cp);
				}
				long patPlanNum = PatPlans.GetPatPlanNum(sub.InsSubNum,_listPatPlans);
				if(patPlanNum > 0) {
					double paidOtherInsTotal = ClaimProcs.GetPaidOtherInsTotal(cp,_listPatPlans);
					double writeOffOtherIns = ClaimProcs.GetWriteOffOtherIns(cp,_listPatPlans);
					ClaimProcs.ComputeBaseEst(cp,_procedure,plan,patPlanNum,benList,
						ListClaimProcHists,ListClaimProcHistsLoop,_listPatPlans,paidOtherInsTotal,paidOtherInsTotal,_patient.Age,writeOffOtherIns,_listInsPlans,_listInsSubs,
						ListSubstLinks,false,LookupFees,blueBookEstimateData:blueBookEstimateData);
				}
			}
			using FormClaimProc FormC=new FormClaimProc(cp,_procedure,_family,_patient,_listInsPlans,ListClaimProcHists,ref ListClaimProcHistsLoop,_listPatPlans,true,_listInsSubs
				,blueBookEstimateData);
			//FormC.NoPermission not needed because butAddEstimate not enabled
			FormC.ShowDialog();
			if(FormC.DialogResult==DialogResult.Cancel && cp.IsNew){
				ClaimProcs.Delete(cp);
			}
			FillIns();
		}

		private void FillPayments(bool doRefreshData=true){
			if(doRefreshData) {
				_loadData.ListPaySplitsForProc=PaySplits.GetForProcs(ListTools.FromSingle(_procedure.ProcNum));
				_loadData.ListPaymentsForProc=Payments.GetPayments(_loadData.ListPaySplitsForProc.Select(x => x.PayNum).ToList());
			}
			_listPaymentsForProc=_loadData.ListPaymentsForProc;
			_paySplitsForProc=PaySplits.GetForProc(_procedure.ProcNum,_loadData.ListPaySplitsForProc.ToArray());
			gridPay.BeginUpdate();
			gridPay.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableProcPay","Entry Date"),70,HorizontalAlignment.Center);
			gridPay.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcPay","Amount"),55,HorizontalAlignment.Right);
			gridPay.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcPay","Tot Amt"),55,HorizontalAlignment.Right);
			gridPay.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcPay","Note"),250,HorizontalAlignment.Left);
			gridPay.ListGridColumns.Add(col);
			gridPay.ListGridRows.Clear();
			GridRow row;
			Payment PaymentCur;//used in loop
			for(int i=0;i<_paySplitsForProc.Count;i++){
				row=new GridRow();
				row.Cells.Add(((PaySplit)_paySplitsForProc[i]).DatePay.ToShortDateString());
				row.Cells.Add(((PaySplit)_paySplitsForProc[i]).SplitAmt.ToString("F"));
				row.Cells[row.Cells.Count-1].Bold=YN.Yes;
				PaymentCur=Payments.GetFromList(((PaySplit)_paySplitsForProc[i]).PayNum,_listPaymentsForProc);
				row.Cells.Add(PaymentCur.PayAmt.ToString("F"));
				row.Cells.Add(PaymentCur.PayNote);
				gridPay.ListGridRows.Add(row);
			}
			gridPay.EndUpdate();
		}

		private void gridPay_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Payment PaymentCur=Payments.GetFromList(((PaySplit)_paySplitsForProc[e.Row]).PayNum,_listPaymentsForProc);
			using FormPayment FormP=new FormPayment(_patient,_family,PaymentCur,false);
			FormP.InitialPaySplitNum=((PaySplit)_paySplitsForProc[e.Row]).SplitNum;
			FormP.ShowDialog();
			FillPayments();
		}

		private void FillAdj(){
			Adjustment[] AdjustmentList=_loadData.ArrAdjustments;
			_adjustmentsForProc=Adjustments.GetForProc(_procedure.ProcNum,AdjustmentList);
			gridAdj.BeginUpdate();
			gridAdj.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableProcAdj","Entry Date"),70,HorizontalAlignment.Center);
			gridAdj.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcAdj","Amount"),55,HorizontalAlignment.Right);
			gridAdj.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcAdj","Type"),100,HorizontalAlignment.Left);
			gridAdj.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableProcAdj","Note"),250,HorizontalAlignment.Left);
			gridAdj.ListGridColumns.Add(col);
			gridAdj.ListGridRows.Clear();
			GridRow row;
			double discountAmt=0;//Total discount amount from all adjustments of default type.
			for(int i=0;i<_adjustmentsForProc.Count;i++){
				row=new GridRow();
				Adjustment adjustment=(Adjustment)_adjustmentsForProc[i];
				row.Cells.Add(adjustment.AdjDate.ToShortDateString());
				row.Cells.Add(adjustment.AdjAmt.ToString("F"));
				row.Cells[row.Cells.Count-1].Bold=YN.Yes;
				row.Cells.Add(Defs.GetName(DefCat.AdjTypes,adjustment.AdjType));
				row.Cells.Add(adjustment.AdjNote);
				gridAdj.ListGridRows.Add(row);
				if(adjustment.AdjType==PrefC.GetLong(PrefName.TreatPlanDiscountAdjustmentType)) {
					discountAmt-=adjustment.AdjAmt;//Discounts are stored as negatives, we want a positive discount value.
				}
			}
			gridAdj.EndUpdate();
			//Because we keep the discount field in sync with the discount adjustment when the procedure has a status of TP,
			//we considered it a bug that the opposite didn't happen once the procedure was set complete.
			if(_procedure.ProcStatus==ProcStat.C) {
				//Updating the discount text box will cause the procedure to get updated if the user clicks OK.
				//This is fine because the Discount column is not designed for accuracy (after being set complete) and is loosely kept updated.
				textDiscount.Text=discountAmt.ToString("F");//Calculated based on all adjustments of type if complete
			}
		}

		private void gridAdj_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormAdjust FormA=new FormAdjust(_patient,(Adjustment)_adjustmentsForProc[e.Row]);
			if(FormA.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_loadData.ArrAdjustments=Adjustments.GetForProcs(new List<long>() { _procedure.ProcNum }).ToArray();
			FillAdj();
		}

		private void butAddAdjust_Click(object sender, System.EventArgs e) {
			if(_procedure.ProcStatus!=ProcStat.C){
				MsgBox.Show(this,"Adjustments may only be added to completed procedures.");
				return;
			}
			bool isTsiAdj=(TsiTransLogs.IsTransworldEnabled(_patient.ClinicNum)
				&& Patients.IsGuarCollections(_patient.Guarantor)
				&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"The guarantor of this family has been sent to TSI for a past due balance.  "
					+"Is this an adjustment applied by the office?\r\n\r\n"
					+"Yes - this is an adjustment applied by the office\r\n\r\n"
					+"No - this adjustment is the result of a payment received from TSI"));
			Adjustment adj=new Adjustment();
			adj.PatNum=_patient.PatNum;
			adj.ProvNum=comboProv.GetSelectedProvNum();
			adj.DateEntry=DateTime.Today;//but will get overwritten to server date
			adj.AdjDate=DateTime.Today;
			adj.ProcDate=_procedure.ProcDate;
			adj.ProcNum=_procedure.ProcNum;
			adj.ClinicNum=_procedure.ClinicNum;
			using FormAdjust FormA=new FormAdjust(_patient,adj,isTsiAdj);
			FormA.IsNew=true;
			if(FormA.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_loadData.ArrAdjustments=Adjustments.GetForProcs(new List<long>() { _procedure.ProcNum }).ToArray();
			FillAdj();
		}

		private void butAddExistAdj_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AdjustmentEdit)) {
				return;
			}
			if(_procedure.ProcStatus!=ProcStat.C){
				MsgBox.Show(this,"Adjustments may only be added to completed procedures.");
				return;
			}
			using FormAdjustmentPicker FormAP=new FormAdjustmentPicker(_patient.PatNum,true);
			if(FormAP.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(!Security.IsAuthorized(Permissions.AdjustmentEdit,FormAP.SelectedAdjustment.AdjDate)) {
				return;
			}
			if(AvaTax.IsEnabled() && 
				(FormAP.SelectedAdjustment.AdjType==AvaTax.SalesTaxAdjType || FormAP.SelectedAdjustment.AdjType==AvaTax.SalesTaxReturnAdjType) && 
					!Security.IsAuthorized(Permissions.SalesTaxAdjEdit)) 
			{
				return;
			}
			List<PaySplit> listPaySplitsForAdjustment=PaySplits.GetForAdjustments(new List<long>{FormAP.SelectedAdjustment.AdjNum});
			if(listPaySplitsForAdjustment.Count>0) {
				MsgBox.Show(this,"Cannot attach adjustment which has associated payments");
				return;
			}
			List<PayPlanLink> listPayPlanLinks=PayPlanLinks.GetForFKeyAndLinkType(FormAP.SelectedAdjustment.AdjNum,PayPlanLinkType.Adjustment);
			if(listPayPlanLinks.Count>0) {
				MsgBox.Show(this,"Cannot attach adjustment which is associated to a payment plan.");
				return;
			}
			decimal estPatPort=ClaimProcs.GetPatPortion(_procedure,_loadData.ListClaimProcsForProc,_loadData.ArrAdjustments.ToList());
			decimal procPatPaid=(decimal)PaySplits.GetTotForProc(_procedure);
			decimal adjRemAmt=estPatPort-procPatPaid+(decimal)FormAP.SelectedAdjustment.AdjAmt;
			if(adjRemAmt<0 && !MsgBox.Show(this,MsgBoxButtons.OKCancel,"Remaining amount is negative.  Continue?","Overpaid Procedure Warning")) {
				return;
			}
			FormAP.SelectedAdjustment.ProcNum=_procedure.ProcNum;
			FormAP.SelectedAdjustment.ProcDate=_procedure.ProcDate;
			Adjustments.Update(FormAP.SelectedAdjustment);
			_loadData.ArrAdjustments=Adjustments.GetForProcs(new List<long>() { _procedure.ProcNum }).ToArray();
			FillAdj();
		}

		private void textProcFee_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			if(!textProcFee.IsValid()) {
				return;
			}
			double procFee;
			if(textProcFee.Text==""){
				procFee=0;
			}
			else{
				procFee=PIn.Double(textProcFee.Text);
			}
			if(_procedure.ProcFee==procFee){
				return;
			}
			_procedure.ProcFee=procFee;
			_isEstimateRecompute=true;
			Procedures.ComputeEstimates(_procedure,_patient.PatNum,ref _listClaimProcsForProc,false,_listInsPlans,_listPatPlans,_listBenefits,
				null,null,true,
				_patient.Age,_listInsSubs,
				null,false,false,ListSubstLinks,false,
				null,LookupFees);
			FillIns();
		}

		private void textTooth_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			textTooth.Text=textTooth.Text.ToUpper();
			if(!Tooth.IsValidEntry(textTooth.Text))
				errorProvider2.SetError(textTooth,Lan.g(this,"Invalid tooth number."));
			else
				errorProvider2.SetError(textTooth,"");
		}

		private void textSurfaces_TextChanged(object sender, System.EventArgs e) {
			int cursorPos = textSurfaces.SelectionStart;
			textSurfaces.Text=textSurfaces.Text.ToUpper();
			textSurfaces.SelectionStart=cursorPos;
		}

		private void textSurfaces_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			if(Tooth.IsValidEntry(textTooth.Text)){
				textSurfaces.Text=Tooth.SurfTidyForDisplay(textSurfaces.Text,Tooth.FromInternat(textTooth.Text));
			}
			else{
				textSurfaces.Text=Tooth.SurfTidyForDisplay(textSurfaces.Text,"");
			}
			if(textSurfaces.Text=="")
				errorProvider2.SetError(textSurfaces,"No surfaces selected.");
			else
				errorProvider2.SetError(textSurfaces,"");
			SetSurfButtons();
		}

		private void groupSextant_Validating(object sender,CancelEventArgs e) {
			if(IsSextantSelected()) {
				errorProvider2.SetError(groupSextant,"");
			}
			else {
				errorProvider2.SetError(groupSextant,Lan.g(this,"Please select a sextant treatment area."));
			}
		}

		private bool IsSextantSelected() {
			return groupSextant.Controls.OfType<RadioButton>().Any(x => x.Checked);
		}

		private void groupArch_Validating(object sender,CancelEventArgs e) {
			if(IsArchSelected()) {
				errorProvider2.SetError(groupArch,"");
			}
			else {
				errorProvider2.SetError(groupArch,Lan.g(this,"Please select a arch treatment area."));
			}
		}

		private bool IsArchSelected() {
			return groupArch.Controls.OfType<RadioButton>().Any(x => x.Checked);
		}

		///<summary>Deletes any ClaimProcs in the list that do not have a claim payment.</summary>
		private void ClearClaimProcs(List<ClaimProc> listClaimProcs) {
			for(int i=listClaimProcs.Count-1;i>=0;i--) {
				if(listClaimProcs[i].ClaimPaymentNum!=0) {
					continue;
				}
				ClaimProcs.Delete(listClaimProcs[i]);//that way, completely new ones will be added back, and NoBillIns will be accurate.
				listClaimProcs.RemoveAt(i);
			}
		}

		private void butChange_Click(object sender, System.EventArgs e) {
			using FormProcCodes FormP=new FormProcCodes();
      FormP.IsSelectionMode=true;
      FormP.ShowDialog();
      if(FormP.DialogResult!=DialogResult.OK){
				return;
			}
			_listFees=null;
			_lookupFees=null;//will trigger another lazy load, later, with this new code.
			Procedure procOld=_procedure.Copy();
			ProcedureCode procCodeOld=ProcedureCodes.GetProcCode(_procedure.CodeNum);
			ProcedureCode procCodeNew=ProcedureCodes.GetProcCode(FormP.SelectedCodeNum);
			if(procCodeOld.TreatArea != procCodeNew.TreatArea
				|| procCodeOld.AreaAlsoToothRange != procCodeNew.AreaAlsoToothRange) 
			{
				MsgBox.Show(this,"Not allowed due to treatment area mismatch.");
				return;
			}
      _procedure.CodeNum=FormP.SelectedCodeNum;
			_procedure.ProcFee=Procedures.GetProcFee(_patient,_listPatPlans,_listInsSubs,_listInsPlans,_procedure.CodeNum,_procedure.ProvNum,_procedure.ClinicNum
				,_procedure.MedicalCode,listFees:ListFees);
			switch(procCodeNew.TreatArea){ 
				case TreatmentArea.Quad:
					_procedure.Surf="UR";
					break;
				case TreatmentArea.Sextant:
					_procedure.Surf="1";
					break;
				case TreatmentArea.Arch:
					_procedure.Surf="U";
					break;
			}
			ClearClaimProcs(_listClaimProcsForProc);
			_listClaimProcsForProc=new List<ClaimProc>();
			Procedures.ComputeEstimates(_procedure,_patient.PatNum,ref _listClaimProcsForProc,true,_listInsPlans,_listPatPlans,_listBenefits,
				null,null,true,
				_patient.Age,_listInsSubs,
				null,false,false,ListSubstLinks,false,
				null,LookupFees);
			#region New Procedure Code Overallocated
			if(_paySplitsForProc.Count>0 || _adjustmentsForProc.Count>0) {
				//Need to refresh from the database because this Procedures.ComputeEstimates() may have lost the reference to a new list.
				_listClaimProcsForProc=ClaimProcs.RefreshForProc(_procedure.ProcNum);
				double chargeOld=procOld.ProcFeeTotal;
				double chargeNew=_procedure.ProcFeeTotal;
				double sumWO=ClaimProcs.ProcWriteoff(_listClaimProcsForProc,_procedure.ProcNum);
				double sumInsPaids=ClaimProcs.ProcInsPay(_listClaimProcsForProc,_procedure.ProcNum);
				double sumInsEsts=ClaimProcs.ProcEstNotReceived(_listClaimProcsForProc,_procedure.ProcNum);
				//Adjustments are already negative if a discount, etc.
				double sumAdjs=_adjustmentsForProc.Cast<Adjustment>().ToList().FindAll(x => x.ProcNum==_procedure.ProcNum).Sum(x => x.AdjAmt);
				double sumPaySplits=_paySplitsForProc.Cast<PaySplit>().ToList().FindAll(x => x.ProcNum==_procedure.ProcNum).Sum(x => x.SplitAmt);
				double credits=sumWO+sumInsPaids+sumInsEsts-sumAdjs+sumPaySplits;
				//Check if the new ProcCode will result in the procedure being overallocated due to a change in ProcFee.
				if(credits>chargeNew) {//Procedure is overallocated.
					string strMsg=Lan.g(this,"The fee will be changed from")+" "+chargeOld.ToString("c")+" "
						+Lan.g(this,"to")+" "+chargeNew.ToString("c")
						+Lan.g(this,", and the procedure has credits attached in the amount of")+" "+credits.ToString("c")
						+Lan.g(this,".  This will result in an overallocated procedure")+".\r\n"+Lan.g(this,"Continue?");
					//Prompt user to accept the overallocation or revert back to old ProcCode.
					if(MessageBox.Show(this,strMsg,Lan.g(this,"Overpaid Procedure Warning"),MessageBoxButtons.YesNo)==DialogResult.No) {
						_procedure=procOld;
						ClearClaimProcs(_listClaimProcsForProc);
						Procedures.ComputeEstimates(_procedure,_patient.PatNum,ref _listClaimProcsForProc,false,_listInsPlans,_listPatPlans,_listBenefits,
							null,null,true,
							_patient.Age,_listInsSubs,
							null,false,false,ListSubstLinks,false,
							null,LookupFees);
						_listClaimProcsForProc=ClaimProcs.RefreshForProc(_procedure.ProcNum);
					}
				}
			}
			#endregion
			_procedureCode=ProcedureCodes.GetProcCode(_procedure.CodeNum);
			textDesc.Text=_procedureCode.Descript;
			//Update the UI now that we know the procedure code change is fine
			switch(_procedureCode.TreatArea){ 
				case TreatmentArea.Quad:
					radioUR.Checked=true;
					break;
				case TreatmentArea.Sextant:
					radioS1.Checked=true;
					break;
				case TreatmentArea.Arch:
					radioU.Checked=true;
					break;
			}
			FillIns();
      SetControlsUpperLeft();
		}

		///<summary>This method is called where a user is attempting to edit a procedure that is attached to a claim. There are two separate permissions 
		///	that deal with this scenario and we need to consider if we need to check for one or both and why.</summary>
		private bool HasPermissions(List<ClaimProc> listClaimProcs,List<Claim> listClaims) {
			bool hasSentOrRecPreauth=false;
			bool hasSentOrRecClaim=false;
			DateTime dateOldestClaim=Procedures.GetOldestClaimDate(listClaimProcs,includePreAuth:false);
			DateTime dateOldestPreAuth=Procedures.GetOldestPreAuth(listClaimProcs);
			List<Permissions> perms=new List<Permissions>();
			List<long> listClaimProcClaimNums=listClaimProcs.Select(x=>x.ClaimNum).ToList();
			List<Claim> listSentOrReceivedClaims=listClaims.Where(x=>x.ClaimStatus=="R" || x.ClaimStatus=="S").ToList();
			for(int i=0;i<listSentOrReceivedClaims.Count;i++) {
				Claim claim=listSentOrReceivedClaims[i];
				if(listClaimProcClaimNums.Contains(claim.ClaimNum)) {
					hasSentOrRecPreauth|=claim.ClaimType=="PreAuth";
					hasSentOrRecClaim|=claim.ClaimType!="PreAuth";
				}
			}
			if(hasSentOrRecPreauth) {
				perms.Add(Permissions.PreAuthSentEdit);
			}
			if(hasSentOrRecClaim) {
				perms.Add(Permissions.ClaimSentEdit);
			}
			bool isAllowed=true;
			for(int i=0;i<perms.Count();i++) {
				if(perms[i]==Permissions.PreAuthSentEdit) {
					isAllowed&=Security.IsAuthorized(perms[i],dateOldestPreAuth);
				}
				else {
					isAllowed&=Security.IsAuthorized(perms[i],dateOldestClaim);
				}
			}
			return isAllowed;
		}

		private void butEditAnyway_Click(object sender, System.EventArgs e) {
			if(!HasPermissions(_loadData.ListClaimProcsForProc,_loadData.ListClaims)) {
				return;
			}
			if(_orthoProcLink!=null) {
				MsgBox.Show(this,"Not allowed to edit specific procedure fields for procedures linked to an ortho case.");
				return;
			}
			panel1.Enabled=true;
			comboProcStatus.Enabled=true;
			checkNoBillIns.Enabled=true;
			butDelete.Enabled=true;
			butSetComplete.Enabled=true;
			SetControlsEnabled(true);//enables/disables controls based on whether or not the user has permission (limited and/or full) to edit completed procs
			//Disable controls that may have been overzealously enabled.
			textTooth.BackColor=SystemColors.Control;
			textTooth.ReadOnly=true;
			textSurfaces.BackColor=SystemColors.Control;
			textSurfaces.ReadOnly=true;
			butAddEstimate.Enabled=true;
			radioL.Enabled=false;
			radioU.Enabled=false;
			radioLL.Enabled=false;
			radioLR.Enabled=false;
			radioUL.Enabled=false;
			radioUR.Enabled=false;
			radioS1.Enabled=false;
			radioS2.Enabled=false;
			radioS3.Enabled=false;
			radioS4.Enabled=false;
			radioS5.Enabled=false;
			radioS6.Enabled=false;
			listBoxTeeth.Enabled=false;
			listBoxTeeth2.Enabled=false;
			panelSurfaces.Enabled=false;
			//butChange.Enabled=true;//No. We no longer allow this because part of "change" is to delete all the claimprocs.  This is a terrible idea for a completed proc attached to a claim.
			//checkIsCovIns.Enabled=true;
		}

		private void comboProcStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			//status cannot be changed for completed procedures attached to a claim, except we allow changing status for preauths.
			//cannot edit status for TPi procedures.
			if(ProcedureL.IsProcCompleteAttachedToClaim(_procedureOld,_listClaimProcsForProc)) {
				comboProcStatus.SetSelectedEnum(ProcStat.C);//Complete
				return;
			}
			if(comboProcStatus.GetSelected<ProcStat>()==ProcStat.TP) {//fee starts out 0 if EO, EC, etc.  This updates fee if changing to TP so it won't stay 0.
				_procedure.ProcStatus=ProcStat.TP;
				if(_procedure.ProcFee==0) {
					_procedure.ProcFee=Procedures.GetProcFee(_patient,_listPatPlans,_listInsSubs,_listInsPlans,_procedure.CodeNum,_procedure.ProvNum,_procedure.ClinicNum,
						_procedure.MedicalCode,listFees:ListFees);
					textProcFee.Text=_procedure.ProcFee.ToString("f");
				}
			}
			if(comboProcStatus.GetSelected<ProcStat>()==ProcStat.C) {
				bool isAllowedToCompl=true;
				if(!PrefC.GetBool(PrefName.AllowSettingProcsComplete)) {
					MsgBox.Show(this,"Set the procedure complete by setting the appointment complete.  "
						+"If you want to be able to set procedures complete, you must turn on that option in Setup | Chart | Chart Preferences.");
					isAllowedToCompl=false;
				}
				//else if so that we don't give multiple notifications to the user.
				else if(!Security.IsAuthorized(Permissions.ProcComplCreate,PIn.Date(textDate.Text),_procedure.CodeNum,PIn.Double(textProcFee.Text))) {
					isAllowedToCompl=false;
				}
				//Check to see if the user is allowed to set the procedure complete.
				if(!isAllowedToCompl) {
					//User not allowed to complete procedures so set it back to whatever it was before
					if(_procedure.ProcStatus==ProcStat.TP) {
						comboProcStatus.SetSelectedEnum(ProcStat.TP);
					}
					else if(PrefC.GetBool(PrefName.EasyHideClinical)) {
						comboProcStatus.SelectedIndex=-1;//original status must not be visible
					}
					else {
						if(ListTools.In(_procedure.ProcStatus,ProcStat.EC,ProcStat.EO,ProcStat.R,ProcStat.Cn))
						{
							comboProcStatus.SetSelectedEnum(_procedure.ProcStatus);
						}
					}
					return;
				}
				if(_procedure.AptNum!=0) {//if attached to an appointment
					Appointment apt=Appointments.GetOneApt(_procedure.AptNum);
					if(apt.AptDateTime.Date > MiscData.GetNowDateTime().Date) {//if appointment is in the future
						MessageBox.Show(Lan.g(this,"Not allowed because procedure is attached to a future appointment with a date of ")
							+apt.AptDateTime.ToShortDateString());
						return;
					}
					if(apt.AptDateTime.Year<1880) {
						textDate.Text=MiscData.GetNowDateTime().ToShortDateString();
					}
					else {
						textDate.Text=apt.AptDateTime.ToShortDateString();
					}
				}
				else {
					textDate.Text=MiscData.GetNowDateTime().ToShortDateString();
				}
				//broken appointment procedure codes shouldn't trigger DateFirstVisit update.
				if(ProcedureCodes.GetStringProcCode(_procedure.CodeNum)!="D9986" && ProcedureCodes.GetStringProcCode(_procedure.CodeNum)!="D9987") {
					Procedures.SetDateFirstVisit(DateTime.Today,2,_patient);
				}
				_procedure.ProcStatus=ProcStat.C;
				//Setting a procedure to complete from the dropdown menu should only change the procedure status, not the procedure dates or copy default 
				//procedure notes, per the manual (https://opendental.com/manual/procedureedit.html). It is desireable to not add the note from the combobox
				//as this allows a way for the user to change Complete to TP (to update fees) and back without adding duplicate notes.
			}
			ProcStat selecteStat=comboProcStatus.GetSelected<ProcStat>();
			if(ListTools.In(selecteStat,ProcStat.EC,ProcStat.EO,ProcStat.R,ProcStat.Cn)) {
				_procedure.ProcStatus=selecteStat;
			}
			//If it's already locked, there's simply no way to save the changes made to this control.
			//If status was just changed to C, then we should show the lock button.
			if(_procedure.ProcStatus==ProcStat.C) {
				if(PrefC.GetBool(PrefName.ProcLockingIsAllowed) && !_procedure.IsLocked) {
					butLock.Visible=true;
				}
			}
		}

		private void butSetComplete_Click(object sender, System.EventArgs e) {
			//can't get to here if attached to a claim, even if use the Edit Anyway button.
			if(_procedureOld.ProcStatus==ProcStat.C){
				MsgBox.Show(this,"Procedure was already set complete.");
				return;
			}
			if(!PrefC.GetBool(PrefName.AllowSettingProcsComplete)) {
				MsgBox.Show(this,"Set the procedure complete by setting the appointment complete.  "
					+"If you want to be able to set procedures complete, you must turn on that option in Setup | Chart | Chart Preferences.");
				return;
			}
			//If user is trying to change status to complete and using eCW.
			if((IsNew || _procedureOld.ProcStatus!=ProcStat.C) && Programs.UsingEcwTightOrFullMode()) {
				MsgBox.Show(this,"Procedures cannot be set complete in this window.  Set the procedure complete by setting the appointment complete.");
				return;
			}
			DateTime procDateNew=DateTime.MinValue;
			if(_procedure.AptNum!=0){//if attached to an appointment
				Appointment apt=Appointments.GetOneApt(_procedure.AptNum);
				if(apt==null) {//Appointment was deleted before proc could be set complete.
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Appointment has been deleted by another user. Continue with setting this procedure complete?")) {
						return;
					}
					procDateNew=MiscData.GetNowDateTime();
					_procedure.AptNum=0;//Remove the attached appointment.
				}
				else {
					if(apt.AptDateTime.Date > MiscData.GetNowDateTime().Date){//if appointment is in the future
						MessageBox.Show(Lan.g(this,"Not allowed because procedure is attached to a future appointment with a date of ")
							+apt.AptDateTime.ToShortDateString());
						return;
					}
					if(apt.AptDateTime.Year<1880) {
						procDateNew=MiscData.GetNowDateTime();
					}
					else {
						procDateNew=apt.AptDateTime;
					}
				}
			}
			else{
				procDateNew=MiscData.GetNowDateTime();
			}
			//Use procDateNew since this is the date that the procedure would end up as
			if(!Security.IsAuthorized(Permissions.ProcComplCreate,procDateNew,_procedure.CodeNum,PIn.Double(textProcFee.Text))) {
				return;
			}
			//broken appointment procedure codes shouldn't trigger DateFirstVisit update.
			if(ProcedureCodes.GetStringProcCode(_procedure.CodeNum)!="D9986" && ProcedureCodes.GetStringProcCode(_procedure.CodeNum)!="D9987") {
				Procedures.SetDateFirstVisit(DateTime.Today,2,_patient);
			}
			textDate.Text=procDateNew.ToShortDateString();
			if(_procedureCode.PaintType==ToothPaintingType.Extraction){//if an extraction, then mark previous procs hidden
				//Procedures.SetHideGraphical(ProcCur);//might not matter anymore
				ToothInitials.SetValue(_procedure.PatNum,_procedure.ToothNum,ToothInitialType.Missing);
			}
			ProcNoteUiHelper();
			Plugins.HookAddCode(this,"FormProcEdit.butSetComplete_Click_end",_procedure,_procedureOld,textNotes);
			comboProcStatus.SelectedIndex=-1;
			_procedure.ProcStatus=ProcStat.C;
			_procedure.SiteNum=_patient.SiteNum;
			comboPlaceService.SelectedIndex=PrefC.GetInt(PrefName.DefaultProcedurePlaceService);
			if(EntriesAreValid()){
				SaveAndClose();
			}
		}

		///<summary>Sets the UI textNotes.Text to the default proc note if any. Also checks PrefName.ProcPromptForAutoNote and remots auto notes if needed.</summary>
		private void ProcNoteUiHelper() {
			string procNoteDefault="";
			if(_isQuickAdd) {//Quick Procs should insert both TP Default Note and C Default Note.
				procNoteDefault=ProcCodeNotes.GetNote(comboProv.GetSelectedProvNum(),_procedure.CodeNum,ProcStat.TP);
				if(!string.IsNullOrEmpty(procNoteDefault)) {
					procNoteDefault+="\r\n";
				}
			}
			procNoteDefault+=ProcCodeNotes.GetNote(comboProv.GetSelectedProvNum(),_procedure.CodeNum,ProcStat.C);
			if(textNotes.Text!="" && procNoteDefault!="") { //check to see if a default note is defined.
				textNotes.Text+="\r\n"; //add a new line if there was already a ProcNote on the procedure.
			}
			if(!string.IsNullOrEmpty(procNoteDefault)) {
				textNotes.Text+=procNoteDefault;
			}
			if(!PrefC.GetBool(PrefName.ProcPromptForAutoNote)) {
				//Users do not want to be prompted for auto notes, so remove them all from the procedure note.
				textNotes.Text=Regex.Replace(textNotes.Text,@"\[\[.+?\]\]","");
			}
		}

		private void radioUR_Click(object sender, System.EventArgs e) {
			_procedure.Surf="UR";
		}

		private void radioUL_Click(object sender, System.EventArgs e) {
			_procedure.Surf="UL";
		}

		private void radioLR_Click(object sender, System.EventArgs e) {
			_procedure.Surf="LR";
		}

		private void radioLL_Click(object sender, System.EventArgs e) {
			_procedure.Surf="LL";
		}

		private void radioU_Click(object sender, System.EventArgs e) {
			_procedure.Surf="U";
			errorProvider2.SetError(groupArch,"");
		}

		private void radioL_Click(object sender, System.EventArgs e) {
			_procedure.Surf="L";
			errorProvider2.SetError(groupArch,"");
		}

		private void radioS1_Click(object sender, System.EventArgs e) {
			_procedure.Surf="1";
			errorProvider2.SetError(groupSextant,"");
		}

		private void radioS2_Click(object sender, System.EventArgs e) {
			_procedure.Surf="2";
			errorProvider2.SetError(groupSextant,"");
		}

		private void radioS3_Click(object sender, System.EventArgs e) {
			_procedure.Surf="3";
			errorProvider2.SetError(groupSextant,"");
		}

		private void radioS4_Click(object sender, System.EventArgs e) {
			_procedure.Surf="4";
			errorProvider2.SetError(groupSextant,"");
		}

		private void radioS5_Click(object sender, System.EventArgs e) {
			_procedure.Surf="5";
			errorProvider2.SetError(groupSextant,"");
		}

		private void radioS6_Click(object sender, System.EventArgs e) {
			_procedure.Surf="6";
			errorProvider2.SetError(groupSextant,"");
		}

		private void checkNoBillIns_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsWriteOffEdit,_procedure.DateEntryC)) {
				checkNoBillIns.CheckState=checkNoBillIns.Checked ? CheckState.Unchecked : CheckState.Checked;
				return;
			}
			if(checkNoBillIns.CheckState==CheckState.Indeterminate){
				//not allowed to set to indeterminate, so move on
				checkNoBillIns.CheckState=CheckState.Unchecked;
			}
			Cursor=Cursors.WaitCursor;
			foreach(ClaimProc claimProc in _listClaimProcsForProc) {
				if(!ListTools.In(claimProc.Status,ClaimProcStatus.Estimate,ClaimProcStatus.CapClaim,ClaimProcStatus.CapEstimate)) {
					continue;
				}
				claimProc.NoBillIns=(checkNoBillIns.CheckState==CheckState.Checked);
				ClaimProcs.Update(claimProc);
			}
			//next lines are needed to recalc BaseEst, etc, for claimprocs that are no longer NoBillIns
			//also, if they are NoBillIns, then it clears out the other values.
			Procedures.ComputeEstimates(_procedure,_patient.PatNum,ref _listClaimProcsForProc,false,_listInsPlans,_listPatPlans,_listBenefits,
				null,null,true,
				_patient.Age,_listInsSubs,
				null,false,false,ListSubstLinks,false,
				null,LookupFees
				);
			FillIns();
			Cursor=Cursors.Default;
		}

		private void textNotes_TextChanged(object sender, System.EventArgs e) {
			CheckForCompleteNote();
			if(!_isStartingUp//so this happens only if user changes the note
				&& !_sigChanged)//and the original signature is still showing.
			{
				signatureBoxWrapper.ClearSignature();
				//this will call OnSignatureChanged to set UserNum, textUser, and SigChanged
			}
		}

		private void CheckForCompleteNote(){
			if(textNotes.Text.IndexOf("\"\"")==-1){
				//no occurances of ""
				labelIncomplete.Visible=false;
			}
			else{
				labelIncomplete.Visible=true;
			}
		}

		private void butSearch_Click(object sender,EventArgs e) {
			using InputBox input=new InputBox(Lan.g(this,"Search for"));
			input.ShowDialog();
			if(input.DialogResult!=DialogResult.OK) {
				return;
			}
			string searchText=input.textResult.Text;
			int index=textNotes.Find(input.textResult.Text);//Gets the location of the first character in the control.
			if(index<0) {//-1 is returned when the text is not found.
				textNotes.DeselectAll();
				MessageBox.Show("\""+searchText+"\"\r\n"+Lan.g(this,"was not found in the notes")+".");
				return;
			}
			textNotes.Select(index,searchText.Length);
		}

		private void signatureBoxWrapper_SignatureChanged(object sender,EventArgs e) {
			_sigChanged=true;
			_procedure.UserNum=_curUser.UserNum;
			textUser.Text=_curUser.UserName;
		}

		private void buttonUseAutoNote_Click(object sender,EventArgs e) {
			using FormAutoNoteCompose FormA=new FormAutoNoteCompose();
			FormA.ShowDialog();
			if(FormA.DialogResult==DialogResult.OK) {
				textNotes.AppendText(FormA.CompletedNote);
				bool hasAutoNotePrompt=Regex.IsMatch(textNotes.Text,_autoNotePromptRegex);
				butEditAutoNote.Visible=hasAutoNotePrompt;
			}
		}

		private void butEditAutoNote_Click(object sender,EventArgs e) {
			if(Regex.IsMatch(textNotes.Text,_autoNotePromptRegex)) {
				using FormAutoNoteCompose FormA=new FormAutoNoteCompose();
				FormA.MainTextNote=textNotes.Text;
				FormA.ShowDialog();
				if(FormA.DialogResult==DialogResult.OK) {
					textNotes.Text=FormA.CompletedNote;
					bool hasAutoNotePrompt=Regex.IsMatch(textNotes.Text,_autoNotePromptRegex);
					butEditAutoNote.Visible=hasAutoNotePrompt;
				}
			}
			else {
				MessageBox.Show(Lan.g(this,"No Auto Note available to edit."));
			}
		}

		/*private void butShowMedical_Click(object sender,EventArgs e) {
			if(groupMedical.Height<200) {
				groupMedical.Height=200;
				butShowMedical.Text="^";
			}
			else {
				groupMedical.Height=170;
				butShowMedical.Text="V";
			}
		}*/

		private void textTimeStart_TextChanged(object sender,EventArgs e) {
			UpdateFinalMin();			
		}

		private void textTimeEnd_TextChanged(object sender,EventArgs e) {
			UpdateFinalMin();
		}

		///<summary>Populates final time box with total number of minutes.</summary>
		private void UpdateFinalMin() {
			if(textTimeStart.Text=="" || textTimeEnd.Text=="") {
				return;
			}
			int startTime=0;
			int stopTime=0;
			try {
				startTime=PIn.Int(textTimeStart.Text);
			}
			catch { 
				try {//Try DateTime format.
					DateTime sTime=DateTime.Parse(textTimeStart.Text);
					startTime=(sTime.Hour*(int)Math.Pow(10,2))+sTime.Minute;
				}
				catch {//Not a valid time.
					return;
				}
			}
			try {
				stopTime=PIn.Int(textTimeEnd.Text);
			}
			catch { 
				try {//Try DateTime format.
					DateTime eTime=DateTime.Parse(textTimeEnd.Text);
					stopTime=(eTime.Hour*(int)Math.Pow(10,2))+eTime.Minute;
				}
				catch {//Not a valid time.
					return;
				}
			}
			int total=(((stopTime/100)*60)+(stopTime%100))-(((startTime/100)*60)+(startTime%100));
			textTimeFinal.Text=total.ToString();
		}

		///<summary>Returns min value if blank or invalid string passed in.</summary>
		private DateTime ParseTime(string time) {
			string militaryTime=time;
			DateTime dTime=DateTime.MinValue;
			if(militaryTime=="") {
				return dTime;
			}
			if(militaryTime.Length<4) {
				militaryTime=militaryTime.PadLeft(4,'0');
			}
			//Test if user typed in military time. Ex: 0830 or 1536
			try {
				int hour=PIn.Int(militaryTime.Substring(0,2));
				int minute=PIn.Int(militaryTime.Substring(2,2));
				dTime=new DateTime(1,1,1,hour,minute,0);
				return dTime;
			}
			catch { }
			//Test if user typed in a typical DateTime format. Ex: 1:00 PM
			try { 
				return DateTime.Parse(time);
			}
			catch { }
			return dTime;
		}

		private void UpdateSurf() {
			if(!Tooth.IsValidEntry(textTooth.Text)){
				return;
			}
			errorProvider2.SetError(textSurfaces,"");
			textSurfaces.Text="";
			if(butM.BackColor==Color.White) {
				textSurfaces.AppendText("M");
			}
			if(butOI.BackColor==Color.White) {
				//if(ToothGraphic.IsAnterior(Tooth.FromInternat(textTooth.Text))) {
				if(Tooth.IsAnterior(Tooth.FromInternat(textTooth.Text))) {
					textSurfaces.AppendText("I");
				}
				else {
					textSurfaces.AppendText("O");
				}
			}
			if(butD.BackColor==Color.White) {
				textSurfaces.AppendText("D");
			}
			if(butV.BackColor==Color.White) {
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					textSurfaces.AppendText("5");
				}
				else {
					textSurfaces.AppendText("V");
				}
			}
			if(butBF.BackColor==Color.White) {
				//if(ToothGraphic.IsAnterior(Tooth.FromInternat(textTooth.Text))) {
				if(Tooth.IsAnterior(Tooth.FromInternat(textTooth.Text))) {
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						textSurfaces.AppendText("V");//vestibular
					}
					else {
						textSurfaces.AppendText("F");
					}
				}
				else {
					textSurfaces.AppendText("B");
				}
			}
			if(butL.BackColor==Color.White) {
				textSurfaces.AppendText("L");
			}
		}

		private void butM_Click(object sender,EventArgs e) {
			if(butM.BackColor==Color.White) {
				butM.BackColor=SystemColors.Control;
			}
			else {
				butM.BackColor=Color.White;
			}
			UpdateSurf();
		}

		private void butOI_Click(object sender,EventArgs e) {
			if(butOI.BackColor==Color.White) {
				butOI.BackColor=SystemColors.Control;
			}
			else {
				butOI.BackColor=Color.White;
			}
			UpdateSurf();
		}

		private void butL_Click(object sender,EventArgs e) {
			if(butL.BackColor==Color.White) {
				butL.BackColor=SystemColors.Control;
			}
			else {
				butL.BackColor=Color.White;
			}
			UpdateSurf();
		}

		private void butV_Click(object sender,EventArgs e) {
			if(butV.BackColor==Color.White) {
				butV.BackColor=SystemColors.Control;
			}
			else {
				butV.BackColor=Color.White;
			}
			UpdateSurf();
		}

		private void butBF_Click(object sender,EventArgs e) {
			if(butBF.BackColor==Color.White) {
				butBF.BackColor=SystemColors.Control;
			}
			else {
				butBF.BackColor=Color.White;
			}
			UpdateSurf();
		}

		private void butD_Click(object sender,EventArgs e) {
			if(butD.BackColor==Color.White) {
				butD.BackColor=SystemColors.Control;
			}
			else {
				butD.BackColor=Color.White;
			}
			UpdateSurf();
		}

		private void butPickSite_Click(object sender,EventArgs e) {
			using FormSites FormS=new FormSites();
			FormS.IsSelectionMode=true;
			FormS.SelectedSiteNum=_procedure.SiteNum;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK){
				return;
			}
			_procedure.SiteNum=FormS.SelectedSiteNum;
			textSite.Text=Sites.GetDescription(_procedure.SiteNum);
		}

		///<summary>This button is only visible if 1. Pref ProcLockingIsAllowed is true, 2. Proc isn't already locked, 3. Proc status is C.</summary>
		private void butLock_Click(object sender,EventArgs e) {
			if(!EntriesAreValid()) {
				return;
			}
			_procedure.IsLocked=true;
			SaveAndClose();//saves all the other various changes that the user made
			DialogResult=DialogResult.OK;
		}

		///<summary>This button is only visible when proc IsLocked.</summary>
		private void butInvalidate_Click(object sender,EventArgs e) {
			Permissions perm=GroupPermissions.SwitchExistingPermissionIfNeeded(Permissions.ProcCompleteStatusEdit,_procedure);
			DateTime dateForPerm=Procedures.GetDateForPermCheck(_procedure);
			//What this will really do is "delete" the procedure.
			if(!Security.IsAuthorized(perm,dateForPerm)) {
				return;
			}
			if(Procedures.IsAttachedToClaim(_procedure,_listClaimProcsForProc)) {
				MsgBox.Show(this,"This procedure is attached to a claim and cannot be invalidated without first deleting the claim.");
				return;
			}
			try {
				Procedures.Delete(_procedure.ProcNum);//also deletes any claimprocs (other than ins payments of course).
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			SecurityLogs.MakeLogEntry(perm,_patient.PatNum,Lan.g(this,"Invalidated: ")+
				ProcedureCodes.GetStringProcCode(_procedure.CodeNum).ToString()+" ("+_procedure.ProcStatus+"), "
				+_procedure.ProcDate.ToShortDateString()+", "+_procedure.ProcFee.ToString("c")+", Deleted");
			DialogResult=DialogResult.OK;
		}

		///<summary>This button is only visible when proc IsLocked.</summary>
		private void butAppend_Click(object sender,EventArgs e) {
			using FormProcNoteAppend formPNA=new FormProcNoteAppend();
			formPNA.ProcCur=_procedure;
			formPNA.ShowDialog();
			if(formPNA.DialogResult!=DialogResult.OK) {
				return;
			}
			DialogResult=DialogResult.OK;//exit out of this window.  Change already saved, and OK button is disabled in this window, anyway.
		}

		private void butSnomedBodySiteSelect_Click(object sender,EventArgs e) {
			using FormSnomeds formS=new FormSnomeds();
			formS.IsSelectionMode=true;
			if(formS.ShowDialog()==DialogResult.OK) {
				_snomedBodySite=formS.SelectedSnomed;
				textSnomedBodySite.Text=_snomedBodySite.Description;
			}
		}

		private void butNoneSnomedBodySite_Click(object sender,EventArgs e) {
			_snomedBodySite=null;
			textSnomedBodySite.Text="";
		}

		private void SetIcdLabels() {
			byte icdVersion=9;
			if(checkIcdVersion.Checked) {
				icdVersion=10;
			}
			labelDiagnosisCode.Text=Lan.g(this,"ICD")+"-"+icdVersion+" "+Lan.g(this,"Diagnosis Code 1");
			labelDiagnosisCode2.Text=Lan.g(this,"ICD")+"-"+icdVersion+" "+Lan.g(this,"Diagnosis Code 2");
			labelDiagnosisCode3.Text=Lan.g(this,"ICD")+"-"+icdVersion+" "+Lan.g(this,"Diagnosis Code 3");
			labelDiagnosisCode4.Text=Lan.g(this,"ICD")+"-"+icdVersion+" "+Lan.g(this,"Diagnosis Code 4");
		}

		private void checkIcdVersion_Click(object sender,EventArgs e) {
			SetIcdLabels();
		}

		private void PickDiagnosisCode(TextBox textBoxDiagnosisCode) {
			if(checkIcdVersion.Checked) {//ICD-10
				using FormIcd10s formI=new FormIcd10s();
				formI.IsSelectionMode=true;
				if(formI.ShowDialog()==DialogResult.OK) {
					textBoxDiagnosisCode.Text=formI.SelectedIcd10.Icd10Code;
				}
			}
			else {//ICD-9
				using FormIcd9s formI=new FormIcd9s();
				formI.IsSelectionMode=true;
				if(formI.ShowDialog()==DialogResult.OK) {
					textBoxDiagnosisCode.Text=formI.SelectedIcd9.ICD9Code;
				}
			}
		}

		private void butDiagnosisCode1_Click(object sender,EventArgs e) {
			PickDiagnosisCode(textDiagnosisCode);
		}

		private void butNoneDiagnosisCode1_Click(object sender,EventArgs e) {
			textDiagnosisCode.Text="";
		}

		private void butDiagnosisCode2_Click(object sender,EventArgs e) {
			PickDiagnosisCode(textDiagnosisCode2);
		}

		private void butNoneDiagnosisCode2_Click(object sender,EventArgs e) {
			textDiagnosisCode2.Text="";
		}

		private void butDiagnosisCode3_Click(object sender,EventArgs e) {
			PickDiagnosisCode(textDiagnosisCode3);
		}

		private void butNoneDiagnosisCode3_Click(object sender,EventArgs e) {
			textDiagnosisCode3.Text="";
		}

		private void butDiagnosisCode4_Click(object sender,EventArgs e) {
			PickDiagnosisCode(textDiagnosisCode4);
		}

		private void butNoneDiagnosisCode4_Click(object sender,EventArgs e) {
			textDiagnosisCode4.Text="";
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;//verified that this triggers a delete when window closed from all places where FormProcEdit is used, and where proc could be new.
				return;
			}
			//If this is an existing completed proc, then this delete button is only enabled if the user has permission for ProcComplEdit based on the ProcDate.
			if(!ListTools.In(_procedureOld.ProcStatus,ProcStat.C,ProcStat.EO,ProcStat.EC)
				&& !Security.IsAuthorized(Permissions.ProcDelete,Procedures.GetDateForPermCheck(_procedure))) //This should be a much more lenient permission since completed procedures are already safeguarded.
			{
				return;
			}
			if(!Procedures.IsProcComplEditAuthorized(_procedureOld,true)) {
				return;
			}
			if(MessageBox.Show(Lan.g(this,"Delete Procedure?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
				return;
			}
			if(_procedureOld.ProcStatus==ProcStat.TP || _procedureOld.ProcStatus==ProcStat.TPi) {
				ClaimProc claimProcPreAuth=_listClaimProcsForProc.Where(x=>x.ProcNum==_procedureOld.ProcNum && x.ClaimNum!=0 && x.Status==ClaimProcStatus.Preauth).FirstOrDefault();
				if(claimProcPreAuth!=null && ClaimProcs.RefreshForClaim(claimProcPreAuth.ClaimNum).GroupBy(x=>x.ProcNum).Count()==1) {
					MsgBox.Show(this,"Not allowed to delete the last procedure from a preauthorization. The entire preauthorization would have to be deleted.");
					return;
				}
			}
			if(_orthoProcLink!=null) {
				MsgBox.Show(this,"Not allowed to delete a procedure that is linked to an ortho case. " +
					"Detach the procedure from the ortho case or delete the ortho case first.");
				return;
			}
			try {
				Procedures.Delete(_procedure.ProcNum);//also deletes the claimProcs and adjustments. Might throw exception.
				_isEstimateRecompute=true;
				Recalls.Synch(_procedure.PatNum);//needs to be moved into Procedures.Delete
				Permissions perm=Permissions.ProcDelete;
				string tag="";
				switch(_procedureOld.ProcStatus) {
					case ProcStat.C:
						perm=Permissions.ProcCompleteStatusEdit;
						tag=", "+Lan.g(this,"Deleted");
						break;
					case ProcStat.EO:
					case ProcStat.EC:
						perm=Permissions.ProcExistingEdit;
						tag=", "+Lan.g(this,"Deleted");
						break;
				}
				SecurityLogs.MakeLogEntry(perm,_procedureOld.PatNum,
					ProcedureCodes.GetProcCode(_procedureOld.CodeNum).ProcCode+" ("+_procedureOld.ProcStatus+"), "+_procedureOld.ProcFee.ToString("c")+tag);
				DialogResult=DialogResult.OK;
				Plugins.HookAddCode(this,"FormProcEdit.butDelete_Click_end",_procedure);
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
		}

		private bool EntriesAreValid(){
			#region Surfaces, Tooth, Sextant, Arch, Date UI
			if(!textDateTP.IsValid()
				|| !textDate.IsValid()
				|| !textProcFee.IsValid()
				|| !textDateOriginalProsth.IsValid()
				|| !textDiscount.IsValid())
			{
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return false;
			}
			if(_procedure.ProcStatus!=ProcStat.EO && textDate.Text==""){//Only ProcStat.EO can be empty.
				MessageBox.Show(Lan.g(this,"Please enter a date first."));
				return false;
			}
			#endregion
			#region Note
			//There have been 2 or 3 cases where a customer entered a note with thousands of new lines and when OD tries to display such a note in the chart, a GDI exception occurs because the progress notes grid is very tall and takes up too much video memory. To help prevent this issue, we block the user from entering any note where there are 50 or more consecutive new lines anywhere in the note. Any number of new lines less than 50 are considered to be intentional.
			StringBuilder tooManyNewLines=new StringBuilder();
			for(int i=0;i<50;i++) {
				tooManyNewLines.Append("\r\n");
			}
			if(textNotes.Text.Contains(tooManyNewLines.ToString())) {
				MsgBox.Show(this,"The notes contain 50 or more consecutive blank lines. Probably unintentional and must be fixed.");
				return false;
			}
			#endregion
			#region textTimeStart, textTimeEnd validation
			if(!ProcedureL.AreTimesValid(textTimeStart.Text,textTimeEnd.Text)) {
				return false;
			}
			#endregion
			#region textUnitQty validation
			if(!ProcedureL.IsQuantityValid(PIn.Int(textUnitQty.Text,false))) {
				return false;
			}
			#endregion
			#region Provider UI
			if(comboProv.GetSelectedProvNum()==0){
				MsgBox.Show(this,"You must select a provider first.");
				return false;
			}
			#endregion
			if(errorProvider2.GetError(textSurfaces)!=""
				|| errorProvider2.GetError(textTooth)!="")
			{
				MsgBox.Show(this,"Please fix tooth number or surfaces first.");
				return false;
			}
			if(errorProvider2.GetError(groupSextant)!=""
				|| errorProvider2.GetError(groupArch)!="") 
			{
				MsgBox.Show(this,"Please fix arch or sextant first.");
				return false;
			}
			#region Medical Code
			if(textMedicalCode.Text!="" && !ProcedureCodes.GetContainsKey(textMedicalCode.Text)){
				MsgBox.Show(this,"Invalid medical code.  It must refer to an existing procedure code.");
				return false;
			}
			#endregion
			#region Drug UI
			if(textDrugNDC.Text!=""){
				if(comboDrugUnit.SelectedIndex==(int)EnumProcDrugUnit.None || textDrugQty.Text==""){
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Drug quantity and unit are not entered.  Continue anyway?")){
						return false;
					}
				}
			}
			if(textDrugQty.Text!=""){
				try{
					float.Parse(textDrugQty.Text);
				}
				catch{
					MsgBox.Show(this,"Please fix drug qty first.");
					return false;
				}
			}
			#endregion
			#region Procedure Status
			//If user is trying to change status to complete and using eCW.
			if(_procedure.ProcStatus==ProcStat.C && (IsNew || _procedureOld.ProcStatus!=ProcStat.C) && Programs.UsingEcwTightOrFullMode()) {
				MsgBox.Show(this,"Procedures cannot be set complete in this window.  Set the procedure complete by setting the appointment complete.");
				return false;
			}
			if(_procedure.ProcStatus==ProcStat.C && PIn.Date(textDate.Text).Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				MsgBox.Show(this,"Completed procedures cannot have future dates.");
				return false;
			}
			if(_procedureOld.ProcStatus!=ProcStat.C && _procedure.ProcStatus==ProcStat.C){//if status was changed to complete
				if(_procedure.AptNum!=0) {//if attached to an appointment
					Appointment apt=Appointments.GetOneApt(_procedure.AptNum);
					if(apt.AptDateTime.Date > MiscData.GetNowDateTime().Date) {//if appointment is in the future
						MessageBox.Show(Lan.g(this,"Not allowed because procedure is attached to a future appointment with a date of ")
							+apt.AptDateTime.ToShortDateString());
						return false;
					}
					if(apt.AptDateTime.Year>=1880) {
						textDate.Text=apt.AptDateTime.ToShortDateString();
					}
				}
				if(!_isQuickAdd && !Security.IsAuthorized(Permissions.ProcComplCreate,PIn.Date(textDate.Text))){//use the new date
					return false;
				}
			}
			else if(!_isQuickAdd && IsNew && _procedure.ProcStatus==ProcStat.C) {//if new procedure is complete
				if(!Security.IsAuthorized(Permissions.ProcComplCreate,PIn.Date(textDate.Text),_procedure.CodeNum,PIn.Double(textProcFee.Text))){
					return false;
				}
			}
			else if(!IsNew){//an old procedure
				if(ListTools.In(_procedureOld.ProcStatus,ProcStat.C,ProcStat.EO,ProcStat.EC)) {//that was already complete
					if(!ProcedureL.CheckPermissionsAndGlobalLockDate(_procedureOld,_procedure,PIn.Date(textDate.Text),PIn.Double(textProcFee.Text))) {
						return false;
					}
				}
			}
			#endregion
			#region Canada and Prosthesis
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				if(checkTypeCodeX.Checked) {
					if(checkTypeCodeA.Checked
						|| checkTypeCodeB.Checked
						|| checkTypeCodeC.Checked
						|| checkTypeCodeE.Checked
						|| checkTypeCodeL.Checked
						|| checkTypeCodeS.Checked) 
					{
						MsgBox.Show(this,"If type code 'none' is checked, no other type codes may be checked.");
						return false;
					}
				}
				if(_procedureCode.IsProsth
					&& !checkTypeCodeA.Checked
					&& !checkTypeCodeB.Checked
					&& !checkTypeCodeC.Checked
					&& !checkTypeCodeE.Checked
					&& !checkTypeCodeL.Checked
					&& !checkTypeCodeS.Checked
					&& !checkTypeCodeX.Checked) 
				{
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"At least one type code should be checked for prosthesis.  Continue anyway?")) {
						return false;
					}
				}
				if(!textCanadaLabFee1.IsValid() || !textCanadaLabFee2.IsValid()) {
					MessageBox.Show(Lan.g(this,"Please fix lab fees."));
					return false;
				}
			}
			else {
				if(_procedureCode.IsProsth) {
					if(listProsth.SelectedIndex==0
					|| (listProsth.SelectedIndex==2 && textDateOriginalProsth.Text=="")) {
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Prosthesis date not entered. Continue anyway?")){
							return false;
						}
					}
				}
			}
#endregion
			#region Quadrant UI
			if(_procedureCode.TreatArea==TreatmentArea.Quad) {
				if(!radioUL.Checked && !radioUR.Checked && !radioLL.Checked && !radioLR.Checked) {
					MsgBox.Show(this,"Please select a quadrant.");
					return false;
				}
			}
			#endregion
			#region Provider
			_listClaimProcsForProc=ClaimProcs.GetForProc(ClaimProcs.Refresh(_procedure.PatNum),_procedure.ProcNum);//update for accuracy
			if(!ProcedureL.ValidateProvider(_listClaimProcsForProc,comboProv.GetSelectedProvNum(),_procedureOld.ProvNum)) {
				return false;
			}
			#endregion
			//Block if proc is linked to ortho case and user tries to set status from complete to any other status.
			if(_orthoProcLink!=null && (_procedureOld.ProcStatus==ProcStat.C && _procedure.ProcStatus!=ProcStat.C)) {
				MsgBox.Show(this,"The status of a completed procedure that is attached to an ortho case cannot be changed. " +
					"Detach the procedure from the ortho case or delete the ortho case first.");
				return false;
			}
			//Customers have been complaining about procedurelog entries changing their CodeNum column to 0.
			//Based on a security log provided by a customer, we were able to determine that this is one of two potential violators.
			//The following code is here simply to try and get the user to call us so that we can have proof and hopefully find the core of the issue.
			long verifyCode=ProcedureCodes.GetProcCode(textProc.Text).CodeNum;
			try {
				if(verifyCode < 1) {
					throw new ApplicationException("Invalid Procedure Text");
				}
			}
			catch(ApplicationException ae) {
				string error="Please notify support with the following information.\r\n"
					+"Error: "+ae.Message+"\r\n"
					+"verifyCode: "+verifyCode.ToString()+"\r\n"
					+"textProc.Text: "+textProc.Text+"\r\n"
					+"ProcOld.CodeNum: "+(_procedureOld==null ? "NULL" : _procedureOld.CodeNum.ToString())+"\r\n"
					+"ProcCur.CodeNum: "+(_procedure==null ? "NULL" : _procedure.CodeNum.ToString())+"\r\n"
					+"ProcedureCode2.CodeNum: "+(_procedureCode==null ? "NULL" : _procedureCode.CodeNum.ToString())+"\r\n"
					+"\r\n"
					+"StackTrace:\r\n"+ae.StackTrace;
				MsgBoxCopyPaste MsgBCP=new MsgBoxCopyPaste(error);
				MsgBCP.Text="Fatal Error!!!";
				MsgBCP.Show();//Use .Show() to make it easy for the user to keep this window open while they call in.
				return false;
			}
			return true;
		}

		///<summary>MUST call EntriesAreValid first.  Used from OK_Click and from butSetComplete_Click</summary>
		private void SaveAndClose() {
			if(textProcFee.Text=="") {
				textProcFee.Text="0";
			}
			_procedure.PatNum=_patient.PatNum;
			//ProcCur.Code=this.textProc.Text;
			_procedureCode=ProcedureCodes.GetProcCode(textProc.Text);
			_procedure.CodeNum=_procedureCode.CodeNum;
			_procedure.MedicalCode=textMedicalCode.Text;
			_procedure.Discount=PIn.Double(textDiscount.Text);
			if(_snomedBodySite==null) {
				_procedure.SnomedBodySite="";
			}
			else {
				_procedure.SnomedBodySite=_snomedBodySite.SnomedCode;
			}
			_procedure.IcdVersion=9;
			if(checkIcdVersion.Checked) {
				_procedure.IcdVersion=10;
			}
			_procedure.DiagnosticCode="";
			_procedure.DiagnosticCode2="";
			_procedure.DiagnosticCode3="";
			_procedure.DiagnosticCode4="";
			List<string> diagnosticCodes=new List<string>();//A list of all the diagnostic code boxes.
			if(textDiagnosisCode.Text!="") {
				diagnosticCodes.Add(textDiagnosisCode.Text);
			}
			if(textDiagnosisCode2.Text!="") {
				diagnosticCodes.Add(textDiagnosisCode2.Text);
			}
			if(textDiagnosisCode3.Text!="") {
				diagnosticCodes.Add(textDiagnosisCode3.Text);
			}
			if(textDiagnosisCode4.Text!="") {
				diagnosticCodes.Add(textDiagnosisCode4.Text);
			}
			if(diagnosticCodes.Count>0) {
				_procedure.DiagnosticCode=diagnosticCodes[0];
			}
			if(diagnosticCodes.Count>1) {
				_procedure.DiagnosticCode2=diagnosticCodes[1];
			}
			if(diagnosticCodes.Count>2) {
				_procedure.DiagnosticCode3=diagnosticCodes[2];
			}
			if(diagnosticCodes.Count>3) {
				_procedure.DiagnosticCode4=diagnosticCodes[3];
			}
			_procedure.IsPrincDiag=checkIsPrincDiag.Checked;
			_procedure.ProvOrderOverride=_selectedProvOrderNum;
			if(_referralOrdering==null) {
				_procedure.OrderingReferralNum=0;
			}
			else {
				_procedure.OrderingReferralNum=_referralOrdering.ReferralNum;
			}
			_procedure.CodeMod1 = textCodeMod1.Text;
			_procedure.CodeMod2 = textCodeMod2.Text;
			_procedure.CodeMod3 = textCodeMod3.Text;
			_procedure.CodeMod4 = textCodeMod4.Text;
			_procedure.UnitQty = PIn.Int(textUnitQty.Text);
			_procedure.UnitQtyType=(ProcUnitQtyType)comboUnitType.SelectedIndex;
			_procedure.RevCode = textRevCode.Text;
			_procedure.DrugUnit=(EnumProcDrugUnit)comboDrugUnit.SelectedIndex;
			_procedure.DrugQty=PIn.Float(textDrugQty.Text);
			_procedure.Urgency=(checkIsEmergency.Checked?ProcUrgency.Emergency:ProcUrgency.Normal);
			_procedure.ProvNum=comboProv.GetSelectedProvNum();			
			ClaimProcs.TrySetProvFromProc(_procedure,_listClaimProcsForProc);
			_procedure.ClinicNum=comboClinic.SelectedClinicNum;
			bool hasSplitProvChanged=false;
			bool hasAdjProvChanged=false;
			if(_procedure.ProvNum!=_procedureOld.ProvNum) {
				if(PaySplits.IsPaySplitAttached(_procedure.ProcNum)) {
					List<PaySplit> listPaySplit=PaySplits.GetPaySplitsFromProc(_procedure.ProcNum);
					foreach(PaySplit paySplit in listPaySplit) {
						if(!Security.IsAuthorized(Permissions.PaymentEdit,Payments.GetPayment(paySplit.PayNum).PayDate)) {
							return;
						}
						if(_procedure.ProvNum != paySplit.ProvNum) {
							hasSplitProvChanged=true;
						}
					}
					if(hasSplitProvChanged
						&& !MsgBox.Show(this,MsgBoxButtons.OKCancel,"The provider for the associated payment splits will be changed to match the provider on the procedure.")) {
						return;
					}
				}
				List<Adjustment> listAdjusts=_adjustmentsForProc.Cast<Adjustment>().ToList();
				foreach(Adjustment adjust in listAdjusts) {
					if(!Security.IsAuthorized(Permissions.AdjustmentEdit,adjust.AdjDate)) {
						return;
					}
					if(_procedure.ProvNum!=adjust.ProvNum && PrefC.GetInt(PrefName.RigorousAdjustments)==(int)RigorousAdjustments.EnforceFully) {
						hasAdjProvChanged=true;
					}
				}
				if(hasAdjProvChanged
					&& !MsgBox.Show(this,MsgBoxButtons.OKCancel,"The provider for the associated adjustments will be changed to match the provider on the procedure.")) {
					return;
				}
			}
			double sumPaySplits=0;
			for(int i=0;i<_paySplitsForProc.Count;i++) {
				sumPaySplits+=((PaySplit)_paySplitsForProc[i]).SplitAmt;
			}
			if(_procedureOld.ProcStatus==ProcStat.C && _procedure.ProcStatus!=ProcStat.C) {//Proc was complete but was changed.
				if(Adjustments.GetForProc(_procedure.ProcNum,Adjustments.Refresh(_procedure.PatNum)).Count!=0
				&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"This procedure has adjustments attached to it. Changing the status from completed will delete any adjustments for the procedure. Continue?")) {
					return;
				}
				if(sumPaySplits!=0) {
					MsgBox.Show(this,"Not allowed to modify the status of a procedure that has payments attached to it. Detach payments from the procedure first.");
					return;
				}
			}
			else if(_procedureOld.ProcStatus!=ProcStat.C && _procedure.ProcStatus==ProcStat.C) {//Proc set complete.
				_procedure.DateEntryC=DateTime.Now;//this triggers it to set to server time NOW().
				if(_procedure.DiagnosticCode=="") {
					_procedure.DiagnosticCode=PrefC.GetString(PrefName.ICD9DefaultForNewProcs);
					_procedure.IcdVersion=PrefC.GetByte(PrefName.DxIcdVersion);
				}
			}
			// textDateTP.Text is blank upon load if date in DB is before 1/1/1880. We don't want to update this if the DateTP box is left blank.
			if(_procedure.DateTP.Year>1880 || this.textDateTP.Text!="") {
				_procedure.DateTP=PIn.Date(this.textDateTP.Text);
			}
			_procedure.ProcDate=PIn.Date(this.textDate.Text);
			DateTime dateT=PIn.DateT(this.textTimeStart.Text);
			_procedure.ProcTime=new TimeSpan(dateT.Hour,dateT.Minute,0);
			if(PrefC.GetBool(PrefName.ShowFeatureMedicalInsurance)) {
				dateT=ParseTime(textTimeStart.Text);
				_procedure.ProcTime=new TimeSpan(dateT.Hour,dateT.Minute,0);
				dateT=ParseTime(textTimeEnd.Text);
				_procedure.ProcTimeEnd=new TimeSpan(dateT.Hour,dateT.Minute,0);
			}
			_procedure.ProcFee=PIn.Double(textProcFee.Text);
			//ProcCur.LabFee=PIn.PDouble(textLabFee.Text);
			//ProcCur.LabProcCode=textLabCode.Text;
			//MessageBox.Show(ProcCur.ProcFee.ToString());
			//Dx taken care of when radio pushed
			if(_procedureCode.TreatArea==TreatmentArea.None
				|| _procedureCode.TreatArea==TreatmentArea.Mouth)
			{
				_procedure.Surf="";
				_procedure.ToothNum="";	
			}
			if(_procedureCode.TreatArea==TreatmentArea.Surf){
				_procedure.ToothNum=Tooth.FromInternat(textTooth.Text);
				_procedure.Surf=Tooth.SurfTidyFromDisplayToDb(textSurfaces.Text,_procedure.ToothNum);
			}
			if(_procedureCode.TreatArea==TreatmentArea.Tooth){
				_procedure.Surf="";
				_procedure.ToothNum=Tooth.FromInternat(textTooth.Text);
			}
			if(_procedureCode.TreatArea==TreatmentArea.Quad){
				//surf set when radio pushed
				_procedure.ToothNum="";	
			}
			if(_procedureCode.TreatArea==TreatmentArea.Sextant){
				//surf taken care of when radio pushed
				_procedure.ToothNum="";	
			}
			if(_procedureCode.TreatArea==TreatmentArea.Arch){
				//taken care of when radio pushed
				_procedure.ToothNum="";	
			}
			if(_procedureCode.TreatArea==TreatmentArea.ToothRange
				|| _procedureCode.AreaAlsoToothRange)
			{
				//Deselect empty tooth selections in Maxillary/Upper Arch.
				for(int j=0;j<listBoxTeeth.Items.Count;j++) {
					if(listBoxTeeth.Items[j].ToString()=="") {//Can be blank when the tooth is flagged as primary when it is an adult tooth.
						listBoxTeeth.SetSelected(j,false);
					}
				}
				//Deselect empty tooth selections in Mandibular/Lower Arch.
				for(int j=0;j<listBoxTeeth2.Items.Count;j++) {
					if(listBoxTeeth2.Items[j].ToString()=="") {//Can be blank when the tooth is flagged as primary when it is an adult tooth.
						listBoxTeeth2.SetSelected(j,false);
					}
				}
				if(listBoxTeeth.SelectedItems.Count<1 && listBoxTeeth2.SelectedItems.Count<1) {
					MessageBox.Show(Lan.g(this,"Must pick at least 1 tooth"));
					return;
				}
				List <string> listSelectedToothNums=new List<string>();
				//Store selected teeth in Maxillary/Upper Arch.
				foreach(int index in listBoxTeeth.SelectedIndices) {
					listSelectedToothNums.Add((index+1).ToString());
				}
				//Store selected teeth in Mandibular/Lower Arch.
				foreach(int index in listBoxTeeth2.SelectedIndices) {
					listSelectedToothNums.Add((32-index).ToString());
				}
				//Identify selected teeth which are primary and convert from permanent tooth num to primary tooth num for storage into database.
				for(int j=0;j<listSelectedToothNums.Count;j++) {
					if(_listPriTeeth.Contains(listSelectedToothNums[j])) {
						listSelectedToothNums[j]=Tooth.PermToPri(listSelectedToothNums[j]);
					}
				}
				_procedure.ToothRange=String.Join(",",listSelectedToothNums);
				_procedure.ToothNum="";	
				if(_procedureCode.AreaAlsoToothRange){
					//arch or quad stored in surf
				}
				else{
					_procedure.Surf="";
				}
			}
			//Status taken care of when list pushed
			_procedure.Note=this.textNotes.Text;
			//Larger offices have trouble with doctors editing specific procedure notes at the same time.
			//One of our customers paid for custom programming that will merge the two notes together in a specific fashion if there was concurrency issues.
			//A specific preference was added because this functionality is so custom.  Typical users can just use the Chart View Audit mode for this info.
			if(_procedureOld.ProcNum > 0 && PrefC.GetBool(PrefName.ProcNoteConcurrencyMerge)) {
				//Go to the database to get the most recent version of the current procedure's note and check it against ProcOld.Note to see if they differ.
				List<ProcNote> listProcNotes=ProcNotes.GetProcNotesForProc(_procedureOld.ProcNum)
					.OrderByDescending(x => x.EntryDateTime)
					.ThenBy(x => x.ProcNoteNum)//Just in case two notes were entered at the "same time" (current version of MySQL can't handle milliseconds)
					.ToList();
				//If there are notes for the current procedure, get the most recent note and compare it to ProcOld.Note.
				//If the current database note differs from the ProcOld.Note then there was a concurrency issue and we have to merge the db note.
				if(listProcNotes.Count > 0 && _procedureOld.Note!=listProcNotes[0].Note) {
					//Manipulate ProcCur.Note to include the most recent note in its entirety with some custom information required by job #2484
					//Use DateTime.Now because the ProcNote won't get inserted until farther down in this method but we have to do this manipulation before sig.
					_procedure.Note=DateTime.Now.ToString()+"  "+Userods.GetName(_procedure.UserNum)+"\r\n"+_procedure.Note;
					//Now we need to append the old note from the database in the same format.
					_procedure.Note+="\r\n------------------------------------------------------\r\n"
						+listProcNotes[0].EntryDateTime.ToString()+"  "+Userods.GetName(listProcNotes[0].UserNum)
						+"\r\n"+listProcNotes[0].Note;
				}
			}
			try {
				SaveSignature();
			}
			catch(Exception ex){
				MessageBox.Show(Lan.g(this,"Error saving signature.")+"\r\n"+ex.Message);
				//and continue with the rest of this method
			}
			#region Update paysplits
			if(hasSplitProvChanged) {
				PaySplits.UpdateAttachedPaySplits(_procedure);//update the attached paysplits.
			}
			if(hasAdjProvChanged) {
				foreach(Adjustment adjust in _adjustmentsForProc) {//update the attached adjustments
					adjust.ProvNum=_procedure.ProvNum;
					Adjustments.Update(adjust);
				}
			}
			#endregion
			_procedure.HideGraphics=checkHideGraphics.Checked;
			if(comboDx.SelectedIndex!=-1) {
				_procedure.Dx=_listDiagnosisDefs[comboDx.SelectedIndex].DefNum;
			}
			if(comboPrognosis.SelectedIndex==0) {
				_procedure.Prognosis=0;
			}
			else {
				_procedure.Prognosis=_listPrognosisDefs[comboPrognosis.SelectedIndex-1].DefNum;
			}
			if(comboPriority.SelectedIndex==0) {
				_procedure.Priority=0;
			}
			else {
				_procedure.Priority=_listTxPriorityDefs[comboPriority.SelectedIndex-1].DefNum;
			}
			_procedure.PlaceService=(PlaceOfService)comboPlaceService.SelectedIndex;
			//site set when user picks from list.
			if(comboBillingTypeOne.SelectedIndex==0){
				_procedure.BillingTypeOne=0;
			}
			else{
				_procedure.BillingTypeOne=_listBillingTypeDefs[comboBillingTypeOne.SelectedIndex-1].DefNum;
			}
			if(comboBillingTypeTwo.SelectedIndex==0) {
				_procedure.BillingTypeTwo=0;
			}
			else {
				_procedure.BillingTypeTwo=_listBillingTypeDefs[comboBillingTypeTwo.SelectedIndex-1].DefNum;
			}
			_procedure.BillingNote=textBillingNote.Text;
			//ProcCur.HideGraphical=checkHideGraphical.Checked;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				_procedure.CanadianTypeCodes="";
				if(checkTypeCodeA.Checked) {
					_procedure.CanadianTypeCodes+="A";
				}
				if(checkTypeCodeB.Checked) {
					_procedure.CanadianTypeCodes+="B";
				}
				if(checkTypeCodeC.Checked) {
					_procedure.CanadianTypeCodes+="C";
				}
				if(checkTypeCodeE.Checked) {
					_procedure.CanadianTypeCodes+="E";
				}
				if(checkTypeCodeL.Checked) {
					_procedure.CanadianTypeCodes+="L";
				}
				if(checkTypeCodeS.Checked) {
					_procedure.CanadianTypeCodes+="S";
				}
				if(checkTypeCodeX.Checked) {
					_procedure.CanadianTypeCodes+="X";
				}
				double canadaLabFee1=0;
				if(textCanadaLabFee1.Text!="") {
					canadaLabFee1=PIn.Double(textCanadaLabFee1.Text);
				}
				if(canadaLabFee1==0) {
					if(textCanadaLabFee1.Visible && _listCanadaLabFees.Count>0) { //Don't worry about deleting child lab fees if we are editing a lab fee. No such concept.
						Procedures.TryDeleteLab(_listCanadaLabFees[0]);
					}
				}
				else { //canadaLabFee1!=0
					if(_listCanadaLabFees.Count>0) { //Retain the old lab code if present.
						Procedure labFee1Old=_listCanadaLabFees[0].Copy();
						_listCanadaLabFees[0].ProcFee=canadaLabFee1;
						Procedures.Update(_listCanadaLabFees[0],labFee1Old);
					}
					else {
						Procedure labFee1=new Procedure();
						labFee1.PatNum=_procedure.PatNum;
						labFee1.ProcDate=_procedure.ProcDate;
						labFee1.ProcFee=canadaLabFee1;
						labFee1.ProcStatus=_procedure.ProcStatus;
						labFee1.ProvNum=_procedure.ProvNum;
						labFee1.DateEntryC=DateTime.Now;
						labFee1.ClinicNum=_procedure.ClinicNum;
						labFee1.ProcNumLab=_procedure.ProcNum;
						labFee1.CodeNum=ProcedureCodes.GetCodeNum("99111");
						//Not sure if Place of Service is required for canadian labs. (I don't see any reason why this would/could/should break anything.)
						labFee1.PlaceService=(PlaceOfService)PrefC.GetInt(PrefName.DefaultProcedurePlaceService);//Default proc place of service for the Practice is used.
						if(labFee1.CodeNum==0) { //Code does not exist.
							ProcedureCode code99111=new ProcedureCode();
							code99111.IsCanadianLab=true;
							code99111.ProcCode="99111";
							code99111.Descript="+L Commercial Laboratory Procedures";
							code99111.AbbrDesc="Lab Fee";
							code99111.ProcCat=Defs.GetByExactNameNeverZero(DefCat.ProcCodeCats,"Adjunctive General Services");
							ProcedureCodes.Insert(code99111);
							labFee1.CodeNum=code99111.CodeNum;
							ProcedureCodes.RefreshCache();
						}
						Procedures.Insert(labFee1);
					}
				}
				double canadaLabFee2=0;
				if(textCanadaLabFee2.Text!="") {
					canadaLabFee2=PIn.Double(textCanadaLabFee2.Text);
				}
				if(canadaLabFee2==0) {
					if(textCanadaLabFee2.Visible && _listCanadaLabFees.Count>1) { //Don't worry about deleting child lab fees if we are editing a lab fee. No such concept.
						Procedures.TryDeleteLab(_listCanadaLabFees[1]);
					}
				}
				else { //canadaLabFee2!=0
					if(_listCanadaLabFees.Count>1) { //Retain the old lab code if present.
						Procedure labFee2Old=_listCanadaLabFees[1].Copy();
						_listCanadaLabFees[1].ProcFee=canadaLabFee2;
						Procedures.Update(_listCanadaLabFees[1],labFee2Old);
					}
					else {
						Procedure labFee2=new Procedure();
						labFee2.PatNum=_procedure.PatNum;
						labFee2.ProcDate=_procedure.ProcDate;
						labFee2.ProcFee=canadaLabFee2;
						labFee2.ProcStatus=_procedure.ProcStatus;
						labFee2.ProvNum=_procedure.ProvNum;
						labFee2.DateEntryC=DateTime.Now;
						labFee2.ClinicNum=_procedure.ClinicNum;
						labFee2.ProcNumLab=_procedure.ProcNum;
						labFee2.CodeNum=ProcedureCodes.GetCodeNum("99111");
						//Not sure if Place of Service is required for canadian labs. (I don't see any reason why this would/could/should break anything.)
						labFee2.PlaceService=(PlaceOfService)PrefC.GetInt(PrefName.DefaultProcedurePlaceService);//Default proc place of service for the Practice is used.
						if(labFee2.CodeNum==0) { //Code does not exist.
							ProcedureCode code99111=new ProcedureCode();
							code99111.IsCanadianLab=true;
							code99111.ProcCode="99111";
							code99111.Descript="+L Commercial Laboratory Procedures";
							code99111.AbbrDesc="Lab Fee";
							code99111.ProcCat=Defs.GetByExactNameNeverZero(DefCat.ProcCodeCats,"Adjunctive General Services");
							ProcedureCodes.Insert(code99111);
							labFee2.CodeNum=code99111.CodeNum;
							ProcedureCodes.RefreshCache();
						}
						Procedures.Insert(labFee2);
					}
				}
			}
			else {
				if(_procedureCode.IsProsth) {
					switch(listProsth.SelectedIndex) {
						case 0:
							_procedure.Prosthesis="";
							break;
						case 1:
							_procedure.Prosthesis="I";
							break;
						case 2:
							_procedure.Prosthesis="R";
							break;
					}
					_procedure.DateOriginalProsth=PIn.Date(textDateOriginalProsth.Text);
					_procedure.IsDateProsthEst=checkIsDateProsthEst.Checked;
				}
				else {
					_procedure.Prosthesis="";
					_procedure.DateOriginalProsth=DateTime.MinValue;
					_procedure.IsDateProsthEst=false;
				}
			}
			_procedure.ClaimNote=textClaimNote.Text;
			//Last chance to run this code before Proc gets updated.
			if(_procedure.ProvNum!=_procedureOld.ProvNum 
				&& _procedure.ProcFee==_procedureOld.ProcFee)
			{
				string promptText="";
				ProcFeeHelper procFeeHelper=new ProcFeeHelper(_patient,ListFees,_listPatPlans,_listInsSubs,_listInsPlans,_listBenefits);
				bool isUpdatingFee=Procedures.ShouldFeesChange(new List<Procedure>() { _procedure.Copy() },new List<Procedure>() { _procedureOld.Copy() },
					ref promptText,procFeeHelper);
				if(isUpdatingFee) {//Made it past the pref check.
					if(promptText!="" && !MsgBox.Show(MsgBoxButtons.YesNo,promptText)) {
							isUpdatingFee=false;
					}
					if(isUpdatingFee) {
						_procedure.ProcFee=Procedures.GetProcFee(_patient,_listPatPlans,_listInsSubs,_listInsPlans,_procedure.CodeNum,_procedure.ProvNum,
							_procedure.ClinicNum,_procedure.MedicalCode,_listBenefits,listFees:ListFees);
					}
				}
			}
			//Autocodes----------------------------------------------------------------------------------------------------------------------------------------
			Permissions perm=GroupPermissions.SwitchExistingPermissionIfNeeded(Permissions.ProcCompleteEdit,_procedure);
			DateTime dateForPerm=Procedures.GetDateForPermCheck(_procedure);
			if(!ListTools.In(_procedureOld.ProcStatus,ProcStat.C,ProcStat.EO,ProcStat.EC)
				|| Security.IsAuthorized(perm,dateForPerm,true)) {
				//Only check auto codes if the procedure is not complete or the user has permission to edit completed procedures.
				long verifyCode;
				bool isMandibular=(listBoxTeeth.SelectedIndices.Count < 1);
				if(AutoCodeItems.ShouldPromptForCodeChange(_procedure,_procedureCode,_patient,isMandibular,_listClaimProcsForProc,out verifyCode)) {
					using FormAutoCodeLessIntrusive FormACLI=new FormAutoCodeLessIntrusive(_patient,_procedure,_procedureCode,verifyCode,_listPatPlans,_listInsSubs,_listInsPlans,
						_listBenefits,_listClaimProcsForProc,listBoxTeeth.Text);
					if(FormACLI.ShowDialog() != DialogResult.OK
						&& PrefC.GetBool(PrefName.ProcEditRequireAutoCodes)) 
					{
						return;//send user back to fix information or use suggested auto code.
					}
					_procedure=FormACLI.Proc;
					_listClaimProcsForProc=ClaimProcs.RefreshForProc(_procedure.ProcNum);//FormAutoCodeLessIntrusive may have added claimprocs.
				}
			}
			//OrthoCase-------------------------------------------------------------------------------------------------------------------------
			//If proc is set complete and orthocases are on, check if we need to link it to an ortho case.
			if(_procedureOld.ProcStatus!=ProcStat.C && _procedure.ProcStatus==ProcStat.C) {
				OrthoCaseProcLinkingData linkingData=new OrthoCaseProcLinkingData(_procedure.PatNum);
				_orthoProcLink=OrthoProcLinks.TryLinkProcForActiveOrthoCase(linkingData,_procedure);//Updates the _procCur.ProcFee for in memory object.
			}
			else if(_orthoProcLink!=null && _procedureOld.ProcDate!=_procedure.ProcDate) {
				OrthoCases.UpdateDatesByLinkedProc(_orthoProcLink,_procedure);
			}
			bool isProcLinkedToOrthoCase=_orthoProcLink!=null;
			//The actual update----------------------------------------------------------------------------------------------------------------------------------
			Procedures.FormProcEditUpdate(_procedure,_procedureOld,_procedureCode,isProcLinkedToOrthoCase,IsNew,listBoxTeeth.Text);
			_listClaimProcsForProc.ForEach(x => x.ClinicNum=comboClinic.SelectedClinicNum);//These changes save in Form_Closing ComputeEstimates depending on DialogResult
			//Recall synch---------------------------------------------------------------------------------------------------------------------------------
			Recalls.Synch(_procedure.PatNum);
			if(_procedureOld.ProcStatus!=ProcStat.C && _procedure.ProcStatus==ProcStat.C) {
				List<string> procCodeList=new List<string>();
				procCodeList.Add(ProcedureCodes.GetStringProcCode(_procedure.CodeNum));
				AutomationL.Trigger(AutomationTrigger.CompleteProcedure,procCodeList,_procedure.PatNum);
				ProcedureL.AfterProcsSetComplete(new List<Procedure>() { _procedure });
			}
			DialogResult=DialogResult.OK;
			//it is assumed that we will do an immediate refresh after closing this window.
		}

		private void butChangeUser_Click(object sender,EventArgs e) {
			using FormLogOn FormChangeUser=new FormLogOn(isSimpleSwitch:true);
			FormChangeUser.ShowDialog();
			if(FormChangeUser.DialogResult==DialogResult.OK) { //if successful
				_curUser=FormChangeUser.CurUserSimpleSwitch; //assign temp user
				bool canUserSignNote=Userods.CanUserSignNote(_curUser);//only show if user can sign
				signatureBoxWrapper.Enabled=canUserSignNote;
				if(!labelPermAlert.Visible && !canUserSignNote) {
					labelPermAlert.Text=Lans.g(this,"Notes can only be signed by providers.");
					labelPermAlert.Visible=true;
				}
				FillComboClinic();
				FillComboProv();
				signatureBoxWrapper.ClearSignature(); //clear sig
				signatureBoxWrapper.UserSig=_curUser;
				textUser.Text=_curUser.UserName; //update user textbox.
				_sigChanged=true;
				_hasUserChanged=true;
			}
		}

		private void SaveSignature(){
			if(_sigChanged){
				string keyData=GetSignatureKey();
				_procedure.Signature=signatureBoxWrapper.GetSignature(keyData);
				_procedure.SigIsTopaz=signatureBoxWrapper.GetSigIsTopaz();
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(!EntriesAreValid()) {
				return;
			}
			//Ask the user to re-sign if the user has changed, there was a signature when the window loaded, and the signature box is currently blank.
			if(_hasUserChanged 
				&& !_procedureOld.Signature.IsNullOrEmpty()
				&& signatureBoxWrapper.SigIsBlank 
				&& !MsgBox.Show(this,MsgBoxButtons.OKCancel,
					"The signature box has not been re-signed.  Continuing will remove the previous signature from this procedure.  Exit anyway?")) 
			{
				return;
			}
			SaveAndClose();
			Plugins.HookAddCode(this,"FormProcEdit.butOK_Click_end",_procedure); 
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormProcEdit_FormClosing(object sender,FormClosingEventArgs e) {
			signatureBoxWrapper?.SetTabletState(0);
			//We need to update the CPOE status even if the user is cancelling out of the window.
			if(Userods.IsUserCpoe(_curUser) && !_procedureOld.IsCpoe) {
				//There's a possibility that we are making a second, unnecessary call to the database here but it is worth it to help meet EHR measures.
				Procedures.UpdateCpoeForProc(_procedure.ProcNum,true);
				//Make a log that we edited this procedure's CPOE flag.
				SecurityLogs.MakeLogEntry(Permissions.ProcEdit,_procedure.PatNum,ProcedureCodes.GetProcCode(_procedure.CodeNum).ProcCode
					+", "+_procedure.ProcFee.ToString("c")+", "+Lan.g(this,"automatically flagged as CPOE."));
			}
			if(DialogResult==DialogResult.OK) {
				//this catches date,prov,fee,status,etc for all claimProcs attached to this proc.
				if(_startedAttachedToClaim!=Procedures.IsAttachedToClaim(_procedure.ProcNum)) {
					//unless they got attached to a claim while this window was open.  Then it doesn't touch them.
					//We don't want to allow ComputeEstimates to reattach the procedure to the old claim which could have deleted.
					return;
				}
				//Now we have to double check that every single claimproc is attached to the same claim that they were originally attached to.
				if(ClaimProcs.IsAttachedToDifferentClaim(_procedure.ProcNum,_listClaimProcsForProc)) {
					return;//The claimproc is not attached to the same claim it was originally pointing to.  Do not run ComputeEstimates which would point it back to the old (potentially deleted) claim.
				}
				List<ClaimProcHist> histList=ClaimProcs.GetHistList(_procedure.PatNum,_listBenefits,_listPatPlans,_listInsPlans,-1,DateTime.Today,_listInsSubs);
				//We don't want already existing claim procs on this procedure to affect the calculation for this procedure.
				histList.RemoveAll(x => x.ProcNum==_procedure.ProcNum);
				Procedures.ComputeEstimates(_procedure,_patient.PatNum,ref _listClaimProcsForProc,_isQuickAdd,_listInsPlans,_listPatPlans,_listBenefits,
					histList,new List<ClaimProcHist> { },true,
					_patient.Age,_listInsSubs,
					null,false,false,ListSubstLinks,false,
					null,LookupFees,_orthoProcLink);
				if(_isEstimateRecompute
					&& _procedure.ProcNumLab!=0//By definition of procedure.ProcNumLab, this will only happen in Canada and if ProcCur is a lab fee.
					&& !Procedures.IsAttachedToClaim(_procedure.ProcNumLab))//If attached to a claim, then user should recreate claim because estimates will be inaccurate not matter what.
				{
					Procedure procParent=Procedures.GetOneProc(_procedure.ProcNumLab,false);
					if(procParent!=null) {//A null parent proc could happen in rare cases for older databases.
						List<ClaimProc> listParentClaimProcs=ClaimProcs.RefreshForProc(procParent.ProcNum);
						Procedures.ComputeEstimates(procParent,_patient.PatNum,ref listParentClaimProcs,false,_listInsPlans,_listPatPlans,_listBenefits,
							null,null,true,
							_patient.Age,_listInsSubs,
							null,false,false,ListSubstLinks,false,
							null,LookupFees);
					}
				}
				return;
			}
			if(IsNew) {//if cancelling on a new procedure
				//delete any newly created claimprocs
				for(int i=0;i<_listClaimProcsForProc.Count;i++) {
					//if(ClaimProcsForProc[i].ProcNum==ProcCur.ProcNum) {
					ClaimProcs.Delete(_listClaimProcsForProc[i]);
					//}
				}
				//delete any newly created adjustments (typically from a discount plan
				for(int i = 0;i<_adjustmentsForProc.Count;++i) {
					Adjustments.Delete((Adjustment)_adjustmentsForProc[i]);
				}
			}
		}

		
	}
}
