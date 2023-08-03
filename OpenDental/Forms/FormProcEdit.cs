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
		private List<ClaimProc> _listClaimProcs;
		private List<PaySplit> _listPaySplits;
		private List<Adjustment> _listAdjustments;
		private Patient _patient;
		private Family _family;
		private List<InsPlan> _listInsPlans;
		///<summary>Lazy loaded, do not directly use this variable, use the property instead.</summary>
		private List<SubstitutionLink> _listSubstitutionLinks=null;
		///<summary>List of all payments (not paysplits) that this procedure is attached to.</summary>
		private List<Payment> _listPayments;
		private const string APPBAR_AUTOMATION_API_MESSAGE = "EZNotes.AppBarStandalone.Auto.API.Message"; 
		private const uint MSG_RESTORE=2;
		private const uint MSG_GETLASTNOTE=3;
		private List<PatPlan> _listPatPlans;
		private List<Benefit> _listBenefits;
		private bool _signatureChanged;
		///<summary>This keeps the noteChanged event from erasing the signature when first loading.</summary>
		private bool _isStartingUp;
		private List<Claim> _listClaims;
		private bool _startedAttachedToClaim;
		private List<InsSub> _listInsSubs;
		private List<Procedure> listProcedures;
		private Snomed _snomedBodySite=null;
		private bool _isQuickAdd=false;
		///<summary>Users can temporarily log in on this form.  Defaults to Security.CurUser.</summary>
		private Userod _Userod=Security.CurUser;
		///<summary>True if the user clicked the Change User button.</summary>
		private bool _hasUserChanged;
		///<summary></summary>
		private long _provNumSelectedOrder;
		///<summary>If this procedure is attached to an ordering referral, then this varible will not be null.</summary>
		private Referral _referralOrdering=null;
		///<summary>True only when modifications to this canadian lab proc will affect the attached parent proc ins estimate.</summary>
		private bool _isEstimateRecompute=false;
		private OrthoProcLink _orthoProcLink;
		private List<Def> _listDefsDiagnosis;
		private List<Def> _listDefsPrognosis;
		private List<Def> _listDefsTxPriority;
		private List<Def> _listDefsBillingType;
		///<summary>Most of the data necessary to load this form.</summary>
		private ProcEdit.LoadData _loadData;
		///<summary>There are a number of places in this form that need fees, but none of them are heavily used.  This will help a little.  Lazy loaded, do not directly use this variable, use the property instead.</summary>
		private Lookup<FeeKey2,Fee> _lookupFees;
		///<summary>See _lookupFees.  Sometimes, we need a list instead of a lookup.</summary>
		private List<Fee> _listFees;
		private List<ToothInitial> _listToothInitialsPat=null;
		///<summary>All primary teeth currently being displayed in the UI, but stored as permanent teeth so that indexes are easy to calculate.</summary>
		private List<string> _listPriTeeths=null;

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
			_listFees=Fees.GetListFromObjects(listProcedureCodes,listProcedures.Select(x=>x.MedicalCode).ToList(),
				Providers.GetProvsForClinic(comboClinic.SelectedClinicNum).Select(x=>x.ProvNum).ToList(), //Get fees for all selectable providers.
				_patient.PriProv,_patient.SecProv,_patient.FeeSched,_listInsPlans,listProcedures.Select(x=>x.ClinicNum).ToList(),null,//appts not needed
				_listSubstitutionLinks,discountPlanNum);
			_lookupFees=(Lookup<FeeKey2,Fee>)_listFees.ToLookup(x => new FeeKey2(x.CodeNum,x.FeeSched));
		}

		///<summary>Inserts are not done within this dialog, but must be done ahead of time from outside.  You must specify a procedure to edit, and only the changes that are made in this dialog get saved.  Only used when double click in Account, Chart, TP, and in ContrChart.AddProcedure().  The procedure may be deleted if new, and user hits Cancel.</summary>
		public FormProcEdit(Procedure procedure,Patient patient,Family family,bool isQuickAdd=false,List<ToothInitial> listToothInitials=null) {
			_procedure=procedure;
			_procedureOld=procedure.Copy();
			_patient=patient;
			_family=family;
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
			_listToothInitialsPat=listToothInitials;
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
			_listSubstitutionLinks=SubstitutionLinks.GetAllForPlans(_listInsPlans);
			FillFees();
			signatureBoxWrapper.SetAllowDigitalSig(true);
			_listClaimProcs=new List<ClaimProc>();
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
			Def def=Defs.GetDef(DefCat.AdjTypes,PrefC.GetLong(PrefName.TreatPlanDiscountAdjustmentType));
			if(!GroupPermissions.HasPermissionForAdjType(Permissions.AdjustmentCreate,def)) {
				textDiscount.Enabled=false;
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
			_listClaimProcs=_loadData.ListClaimProcsForProc;
			_listPatPlans=_loadData.ListPatPlans;
			_listBenefits=_loadData.ListBenefits;
			if(Procedures.IsAttachedToClaim(_procedure,_listClaimProcs)){
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
			if(Procedures.IsAttachedToClaim(_procedure,_listClaimProcs,false)) {
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
				listProcedures=Procedures.GetCanadianLabFees(_procedure.ProcNum);
				if(_procedureCode.IsCanadianLab) { //Prevent lab fees from having lab fees attached.
					labelCanadaLabFee1.Visible=false;
					textCanadaLabFee1.Visible=false;
					labelCanadaLabFee2.Visible=false;
					textCanadaLabFee2.Visible=false;
				}
				else {
					if(listProcedures.Count>0) {
						textCanadaLabFee1.Text=listProcedures[0].ProcFee.ToString("n");
						if(listProcedures[0].ProcStatus==ProcStat.C) {
							textCanadaLabFee1.ReadOnly=true;
						}
					}
					if(listProcedures.Count>1) {
						textCanadaLabFee2.Text=listProcedures[1].ProcFee.ToString("n");
						if(listProcedures[1].ProcStatus==ProcStat.C) {
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
				Referral referral=null;
				try {
					referral=Referrals.GetReferral(_procedure.OrderingReferralNum);
				}
				catch {}
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
			bool hasAutoNotePrompt=Regex.IsMatch(textNotes.Text,Procedures.AutoNotePromptRegex);
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
			comboClinic.SetUser(_Userod);//Not Security.CurUser
			if(clinicNum!=-1){
				comboClinic.SelectedClinicNum=clinicNum;
			}
		}

		private void ComboClinic_SelectionChangeCommitted(object sender, EventArgs e){
			FillComboProv();
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick = new FormProviderPick(comboProv.Items.GetAll<Provider>());
			formProviderPick.ProvNumSelected=comboProv.GetSelectedProvNum();
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProv.SetSelectedProvNum(formProviderPick.ProvNumSelected);
		}

		private void butPickOrderProvInternal_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick = new FormProviderPick(comboProv.Items.GetAll<Provider>());
			formProviderPick.ProvNumSelected=_provNumSelectedOrder;
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
				_provNumSelectedOrder=0;
				textOrderingProviderOverride.Text="";
			}
			else {
				_provNumSelectedOrder=provider.ProvNum;
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
			_provNumSelectedOrder=0;
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
			_listDefsDiagnosis=Defs.GetDefsForCategory(DefCat.Diagnosis,true);
			_listDefsPrognosis=Defs.GetDefsForCategory(DefCat.Prognosis,true);
			_listDefsTxPriority=Defs.GetDefsForCategory(DefCat.TxPriorities,true);
			_listDefsBillingType=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			comboDx.Items.Clear();
			for(int i=0;i<_listDefsDiagnosis.Count;i++){
				comboDx.Items.Add(_listDefsDiagnosis[i].ItemName);
				if(_listDefsDiagnosis[i].DefNum==_procedure.Dx)
					comboDx.SelectedIndex=i;
			}
			comboPrognosis.Items.Clear();
			comboPrognosis.Items.Add(Lan.g(this,"no prognosis"));
			comboPrognosis.SelectedIndex=0;
			for(int i=0;i<_listDefsPrognosis.Count;i++) {
				comboPrognosis.Items.Add(_listDefsPrognosis[i].ItemName);
				if(_listDefsPrognosis[i].DefNum==_procedure.Prognosis)
					comboPrognosis.SelectedIndex=i+1;
			}
			checkHideGraphics.Checked=_procedure.HideGraphics;
			comboPriority.Items.Clear();
			comboPriority.Items.Add(Lan.g(this,"no priority"));
			comboPriority.SelectedIndex=0;
			for(int i=0;i<_listDefsTxPriority.Count;i++){
				comboPriority.Items.Add(_listDefsTxPriority[i].ItemName);
				if(_listDefsTxPriority[i].DefNum==_procedure.Priority)
					comboPriority.SelectedIndex=i+1;
			}
			comboBillingTypeOne.Items.Clear();
			comboBillingTypeOne.Items.Add(Lan.g(this,"none"));
			comboBillingTypeOne.SelectedIndex=0;
			for(int i=0;i<_listDefsBillingType.Count;i++) {
				comboBillingTypeOne.Items.Add(_listDefsBillingType[i].ItemName);
				if(_listDefsBillingType[i].DefNum==_procedure.BillingTypeOne)
					comboBillingTypeOne.SelectedIndex=i+1;
			}
			comboBillingTypeTwo.Items.Clear();
			comboBillingTypeTwo.Items.Add(Lan.g(this,"none"));
			comboBillingTypeTwo.SelectedIndex=0;
			for(int i=0;i<_listDefsBillingType.Count;i++) {
				comboBillingTypeTwo.Items.Add(_listDefsBillingType[i].ItemName);
				if(_listDefsBillingType[i].DefNum==_procedure.BillingTypeTwo)
					comboBillingTypeTwo.SelectedIndex=i+1;
			}
			textBillingNote.Text=_procedure.BillingNote;
			textNotes.Text=_procedure.Note;
			comboPlaceService.Items.Clear();
			comboPlaceService.Items.AddList(Enum.GetNames(typeof(PlaceOfService)));
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
			string keyData=Procedures.GetSignatureKeyData(_procedure);
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
			int location=0;
			for(int i=0;i<listMatches.Count;i++) {
				string autoNoteTitle = listMatches[i].Value.TrimStart('[').TrimEnd(']');
				string note=AutoNotes.GetByTitle(autoNoteTitle);
				int matchloc=textNotes.Text.IndexOf(listMatches[i].Value,location);
				using FormAutoNoteCompose formAutoNoteCompose=new FormAutoNoteCompose();
				formAutoNoteCompose.StrMainTextNote=note;
				formAutoNoteCompose.ShowDialog();
				if(formAutoNoteCompose.DialogResult==DialogResult.Cancel) {
					location=matchloc+listMatches[i].Value.Length;
					continue;//if they cancel, go to the next autonote.
				}
				if(formAutoNoteCompose.DialogResult==DialogResult.OK) {
					//When setting the Text on a RichTextBox, \r\n is replaced with \n, so we need to do the same so that our location variable is correct.
					location=matchloc+formAutoNoteCompose.StrCompletedNote.Replace("\r\n","\n").Length;
					string resultstr=textNotes.Text.Substring(0,matchloc)+formAutoNoteCompose.StrCompletedNote;
					if(textNotes.Text.Length > matchloc+listMatches[i].Value.Length) {
						resultstr+=textNotes.Text.Substring(matchloc+listMatches[i].Value.Length);
					}
					textNotes.Text=resultstr;
				}
			}
			bool hasAutoNotePrompt=Regex.IsMatch(textNotes.Text,Procedures.AutoNotePromptRegex);
			butEditAutoNote.Visible=hasAutoNotePrompt;
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
					textTooth.Text=Tooth.Display(_procedure.ToothNum);
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
					textTooth.Text=Tooth.Display(_procedure.ToothNum);
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
				SetErrorProviderForSextant();
			}
			if(_procedureCode.TreatArea==TreatmentArea.Arch){
				groupArch.Visible=true;
				switch (_procedure.Surf){
					case "U": this.radioU.Checked=true; break;
					case "L": this.radioL.Checked=true; break;
				}
				SetErrorProviderForGroupArc();
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
				if(_listToothInitialsPat==null) {
					_listToothInitialsPat=ToothInitials.GetPatientData(_patient.PatNum);
				}
				//First add teeth flagged as primary teeth for this specific patient from the Chart into _listPriTeeth.
				_listPriTeeths=ToothInitials.GetPriTeeth(_listToothInitialsPat);
				//Preserve tooth range history for this procedure by ensuring that the UI shows the values from the database for the relevant teeth.
				string[] stringArrayToothNums=new string[0];
				if(!string.IsNullOrWhiteSpace(_procedure.ToothRange)){
					stringArrayToothNums=_procedure.ToothRange.Split(',');//in Universal (American) nomenclature
				}
				for(int i=0;i<stringArrayToothNums.Length;i++) {
					if(Tooth.IsPrimary(stringArrayToothNums[i])) {
						_listPriTeeths.Add(Tooth.ToInt(stringArrayToothNums[i]).ToString());//Convert the primary tooth to a permanent tooth.
					}
					else {//Permanent tooth
						_listPriTeeths.Remove(stringArrayToothNums[i]);//Preserve permanent tooth history.
					}
				}
				//Fill Maxillary/Upper Arch
				listBoxTeeth.Items.Clear();
				for(int toothNum=1;toothNum<=16;toothNum++) {
					string toothId=toothNum.ToString();
					if(_listPriTeeths.Contains(toothNum.ToString())) {//Is Primary
						toothId=Tooth.PermToPri(toothId);
					}
					listBoxTeeth.Items.Add(Tooth.GetDisplayGraphic(toothId));//Display tooth is dependent on nomenclature preference.
				}
				//Fill Mandibular/Lower	Arch
				listBoxTeeth2.Items.Clear();
				for(int toothNum=32;toothNum>=17;toothNum--) {
					string toothId=toothNum.ToString();
					if(_listPriTeeths.Contains(toothNum.ToString())) {//Is Primary
						toothId=Tooth.PermToPri(toothId);
					}
					listBoxTeeth2.Items.Add(Tooth.GetDisplayGraphic(toothId));//Display tooth is dependent on nomenclature preference.
				}
				//Select tooth numbers in each arch depending on the database data stored in the procedure ToothRange.
				for(int i=0;i<stringArrayToothNums.Length;i++) {
					if(Tooth.IsMaxillary(stringArrayToothNums[i])) {//Works for primary or permanent tooth numbers.
						int toothIndex=Tooth.ToInt(stringArrayToothNums[i])-1;//Works for primary or permanent tooth numbers.
						listBoxTeeth.SetSelected(toothIndex,true);
					}
					else {//Mandibular
						int toothIndex=32-Tooth.ToInt(stringArrayToothNums[i]);//Works for primary or permanent tooth numbers.
						if(toothIndex<0) {
							//Tooth Range could be 3 to 4 digits (outside of range).  Split the numbers in order to select the correct teeth.
							toothIndex=Tooth.ToInt(stringArrayToothNums[i].Remove(stringArrayToothNums[i].Length-2))-1;
							int toothIndex2=32-Tooth.ToInt(stringArrayToothNums[i].Substring(stringArrayToothNums[i].Length-2,2));
							listBoxTeeth.SetSelected(toothIndex,true);
							listBoxTeeth2.SetSelected(toothIndex2,true);
						}
						else {
							listBoxTeeth2.SetSelected(toothIndex,true);
						}
					}
				}//forloop
			}//if toothrange
			textProcFee.Text=_procedure.ProcFee.ToString("n");
		}

		/// <summary>Takes in a list of controls and loops through to disable the necesary components</summary>
		private void SetControlsDisabled(List<Control> listControls) {
			if(listControls.IsNullOrEmpty()) {
				return;
			}
			for(int i=0;i<listControls.Count;i++) {
				if(listControls[i] is TextBoxBase) {
					((TextBoxBase)listControls[i]).ReadOnly=true;
					listControls[i].BackColor=SystemColors.Control;
				}
				else if(listControls[i] is UI.Button
					|| listControls[i] is System.Windows.Forms.ComboBox
					|| listControls[i] is System.Windows.Forms.CheckBox
					|| listControls[i] is System.Windows.Forms.GroupBox
					|| listControls[i] is Panel
					|| listControls[i] is SignatureBoxWrapper
					|| listControls[i] is UI.ComboBox
					|| listControls[i] is ComboBoxClinicPicker) 
				{
					listControls[i].Enabled=false;
				}
			}
		}

		///<summary>Enable/disable controls based on permissions for a completed procedures.</summary>
		private void SetControlsEnabled(bool isSilent) {
			//Return if the current procedure isn't considered complete (C, EC, EO).
			//Don't allow adding an estimate, since a new estimate could change the total writeoff amount for the proc.
			if(!_procedure.ProcStatus.In(ProcStat.C,ProcStat.EO,ProcStat.EC)) {
				return;
			}
			DateTime dateForPerm=Procedures.GetDateForPermCheck(_procedure);//Use ProcDate to compare to the date/days newer restriction.
			bool isProcStatComplete=_procedure.ProcStatus==ProcStat.C;
			Permissions permissions=GroupPermissions.SwitchExistingPermissionIfNeeded(Permissions.ProcCompleteEdit,_procedure);
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
			dictPermControls[Permissions.ProcCompleteEditMisc]=new List<Control> { checkHideGraphics,checkNoBillIns,comboClinic,textSurfaces,comboDx };
			dictPermControls[Permissions.ProcCompleteEditMisc].AddRange(new List<System.Windows.Forms.TabPage>{tabPageMisc,tabPageMedical}.SelectMany(x => x.Controls.OfType<Control>()));
			#endregion
			List<Control> listDisabled=new List<Control>();
			bool isGlobalDateLocked=Security.IsGlobalDateLock(permissions,dateForPerm,isSilent);//only used to silence other security messages.
			if(!isProcStatComplete) {//either Eo or Ec
				if(!Security.IsAuthorized(permissions,dateForPerm,isSilent,isGlobalDateLocked)) {
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
			if(!Security.IsAuthorized(permissions,dateForPerm,true,true)
				&& Security.IsAuthorized(permissions,dateForPerm,true,true,_procedure.CodeNum,_procedure.ProcFee,0,0)) 
			{
				//This is a $0 procedure for a proc code marked as bypassed.
				butDelete.Enabled=true;
			}
		}//end SetControls

		private void FillReferral(bool doRefreshData=true) {
			if(doRefreshData) {
				_loadData.ListRefAttaches=RefAttaches.RefreshFiltered(_procedure.PatNum,false,_procedure.ProcNum);
			}
			List<RefAttach> listRefAttaches=_loadData.ListRefAttaches;
			if(listRefAttaches.Count==0) {
				textReferral.Text="";
			}
			else {
				Referral referral;
				if(Referrals.TryGetReferral(listRefAttaches[0].ReferralNum,out referral)) {
					textReferral.Text=referral.LName+", ";
				}
				if(listRefAttaches[0].DateProcComplete.Year<1880) {
					textReferral.Text+=listRefAttaches[0].RefDate.ToShortDateString();
				}
				else{
					textReferral.Text+=Lan.g(this,"done:")+listRefAttaches[0].DateProcComplete.ToShortDateString();
				}
				if(listRefAttaches[0].RefToStatus!=ReferralToStatus.None){
					textReferral.Text+=listRefAttaches[0].RefToStatus.ToString();
				}
			}
		}

		private void butReferral_Click(object sender,EventArgs e) {
			using FormReferralsPatient formReferralsPatient=new FormReferralsPatient();
			formReferralsPatient.PatNum=_procedure.PatNum;
			formReferralsPatient.ProcNum=_procedure.ProcNum;
			formReferralsPatient.ShowDialog();
			FillReferral();
		}

		private void FillIns(){
			FillIns(true);
		}

		private void FillIns(bool refreshClaimProcsFirst){
			if(refreshClaimProcsFirst) {
				//ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
				//}
				_listClaimProcs=ClaimProcs.RefreshForProc(_procedure.ProcNum);
			}
			gridIns.BeginUpdate();
			gridIns.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableProcIns","Ins Plan"),190);
			gridIns.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","Pri/Sec"),50,HorizontalAlignment.Center);
			gridIns.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","Status"),50,HorizontalAlignment.Center);
			gridIns.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","NoBill"),45,HorizontalAlignment.Center);
			gridIns.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","Copay"),55,HorizontalAlignment.Right);
			gridIns.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","Deduct"),55,HorizontalAlignment.Right);
			gridIns.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","Percent"),55,HorizontalAlignment.Center);
			gridIns.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","Ins Est"),55,HorizontalAlignment.Right);
			gridIns.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","Ins Pay"),55,HorizontalAlignment.Right);
			gridIns.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","Write-off"),55,HorizontalAlignment.Right);
			gridIns.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcIns","Estimate Note"),100);
			gridIns.Columns.Add(col);		 
			col=new GridColumn(Lan.g("TableProcIns","Remarks"),165);
			gridIns.Columns.Add(col);		 
			gridIns.ListGridRows.Clear();
			GridRow row;
			checkNoBillIns.CheckState=CheckState.Unchecked;
			bool allNoBillIns=true;
			InsPlan insPlan;
			//ODGridCell cell;
			for(int i=0;i<_listClaimProcs.Count;i++) {
				if(_listClaimProcs[i].NoBillIns){
					checkNoBillIns.CheckState=CheckState.Indeterminate;
				}
				else{
					allNoBillIns=false;
				}
				row=new GridRow();
				row.Cells.Add(InsPlans.GetDescript(_listClaimProcs[i].PlanNum,_family,_listInsPlans,_listClaimProcs[i].InsSubNum,_listInsSubs));
				insPlan=InsPlans.GetPlan(_listClaimProcs[i].PlanNum,_listInsPlans);
				if(insPlan==null) {
					MsgBox.Show(this,"No insurance plan exists for this claim proc.  Please run database maintenance.");
					return;
				}
				if(insPlan.IsMedical) {
					row.Cells.Add("Med");
				}
				else if(_listClaimProcs[i].InsSubNum==PatPlans.GetInsSubNum(_listPatPlans,PatPlans.GetOrdinal(PriSecMed.Primary,_listPatPlans,_listInsPlans,_listInsSubs))){
					row.Cells.Add("Pri");
				}
				else if(_listClaimProcs[i].InsSubNum==PatPlans.GetInsSubNum(_listPatPlans,PatPlans.GetOrdinal(PriSecMed.Secondary,_listPatPlans,_listInsPlans,_listInsSubs))) {
					row.Cells.Add("Sec");
				}
				else {
					row.Cells.Add("");
				}
				switch(_listClaimProcs[i].Status) {
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
				if(_listClaimProcs[i].NoBillIns) {
					row.Cells.Add("X");
					if(!_listClaimProcs[i].Status.In(ClaimProcStatus.CapComplete,ClaimProcStatus.CapEstimate)) {
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
				row.Cells.Add(ClaimProcs.GetCopayDisplay(_listClaimProcs[i]));
				double deductibleDisplay=ClaimProcs.GetDeductibleDisplay(_listClaimProcs[i]);
				if(deductibleDisplay>0) {
					row.Cells.Add(deductibleDisplay.ToString("n"));
				}
				else {
					row.Cells.Add("");
				}
				row.Cells.Add(ClaimProcs.GetPercentageDisplay(_listClaimProcs[i]));
				row.Cells.Add(ClaimProcs.GetEstimateDisplay(_listClaimProcs[i]));
				if(_listClaimProcs[i].Status==ClaimProcStatus.Estimate
					|| _listClaimProcs[i].Status==ClaimProcStatus.CapEstimate) 
				{
					row.Cells.Add("");
					row.Cells.Add(ClaimProcs.GetWriteOffEstimateDisplay(_listClaimProcs[i]));
				}
				else {
					row.Cells.Add(_listClaimProcs[i].InsPayAmt.ToString("n"));
					row.Cells.Add(_listClaimProcs[i].WriteOff.ToString("n"));
				}
				row.Cells.Add(_listClaimProcs[i].EstimateNote);
				row.Cells.Add(_listClaimProcs[i].Remarks);			  
				gridIns.ListGridRows.Add(row);
			}
			gridIns.EndUpdate();
			if(_listClaimProcs.Count==0) {
				checkNoBillIns.CheckState=CheckState.Unchecked;
			}
			else if(allNoBillIns) {
				checkNoBillIns.CheckState=CheckState.Checked;
			}
		}

		private void gridIns_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormClaimProc formClaimProc=new FormClaimProc(_listClaimProcs[e.Row],_procedure,_family,_patient,_listInsPlans,ListClaimProcHists,ref ListClaimProcHistsLoop,_listPatPlans,true,_listInsSubs);
			if(_procedure.IsLocked || !Procedures.IsProcComplEditAuthorized(_procedure)) {
				formClaimProc.NoPermissionProc=true;
			}
			formClaimProc.ShowDialog();
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
			using FormInsPlanSelect formInsPlanSelect=new FormInsPlanSelect(_patient.PatNum);
			if(formInsPlanSelect.InsPlanSelected==null) {
				formInsPlanSelect.ShowDialog();
				if(formInsPlanSelect.DialogResult==DialogResult.Cancel){
					return;
				}
			}
			InsPlan insPlan=formInsPlanSelect.InsPlanSelected;
			InsSub insSub=formInsPlanSelect.InsSubSelected;
			_listClaimProcs=ClaimProcs.RefreshForProc(_procedure.ProcNum);
			ClaimProc claimProcForProcInsPlan=_listClaimProcs
				.Where(x => x.PlanNum == insPlan.PlanNum)
				.Where(x => x.Status != ClaimProcStatus.Preauth)
				.FirstOrDefault();
			ClaimProc claimProc = new ClaimProc();
			BlueBookEstimateData blueBookEstimateData=new BlueBookEstimateData(_listInsPlans,_listInsSubs,_listPatPlans,new List<Procedure>{_procedure},_listSubstitutionLinks);
			claimProc.IsNew=true;
			if(claimProcForProcInsPlan!=null) {
				claimProc = claimProcForProcInsPlan;
				claimProc.IsNew=false;
			}
			else {
				List<Benefit> listBenefits = Benefits.Refresh(_listPatPlans,_listInsSubs);
				ClaimProcs.CreateEst(claimProc,_procedure,insPlan,insSub);
				if(insPlan.PlanType=="c") {//capitation
					double procFee = PIn.Double(textProcFee.Text);
					claimProc.BaseEst=procFee;
					claimProc.InsEstTotal=procFee;
					claimProc.CopayAmt=InsPlans.GetCopay(_procedure.CodeNum,insPlan.FeeSched,insPlan.CopayFeeSched,
						!SubstitutionLinks.HasSubstCodeForPlan(insPlan,_procedure.CodeNum,_listSubstitutionLinks),_procedure.ToothNum,_procedure.ClinicNum,_procedure.ProvNum,insPlan.PlanNum,
						_listSubstitutionLinks,_lookupFees);
					if(claimProc.CopayAmt > procFee) {//if the copay is greater than the allowed fee calculated above
						claimProc.CopayAmt=procFee;//reduce the copay
					}
					if(claimProc.CopayAmt==-1) {
						claimProc.CopayAmt=0;
					}
					claimProc.WriteOffEst=claimProc.BaseEst-claimProc.CopayAmt;
					if(claimProc.WriteOffEst<0) {
						claimProc.WriteOffEst=0;
					}
					claimProc.WriteOff=claimProc.WriteOffEst;
					ClaimProcs.Update(claimProc);
				}
				long patPlanNum = PatPlans.GetPatPlanNum(insSub.InsSubNum,_listPatPlans);
				if(patPlanNum > 0) {
					double paidOtherInsTotal = ClaimProcs.GetPaidOtherInsTotal(claimProc,_listPatPlans);
					double writeOffOtherIns = ClaimProcs.GetWriteOffOtherIns(claimProc,_listPatPlans);
					ClaimProcs.ComputeBaseEst(claimProc,_procedure,insPlan,patPlanNum,listBenefits,
						ListClaimProcHists,ListClaimProcHistsLoop,_listPatPlans,paidOtherInsTotal,paidOtherInsTotal,_patient.Age,writeOffOtherIns,_listInsPlans,_listInsSubs,
						_listSubstitutionLinks,false,_lookupFees,blueBookEstimateData:blueBookEstimateData);
				}
			}
			using FormClaimProc formClaimProc=new FormClaimProc(claimProc,_procedure,_family,_patient,_listInsPlans,ListClaimProcHists,ref ListClaimProcHistsLoop,_listPatPlans,true,_listInsSubs);
			formClaimProc.BlueBookEstimateData_=blueBookEstimateData;
			//FormC.NoPermission not needed because butAddEstimate not enabled
			formClaimProc.ShowDialog();
			if(formClaimProc.DialogResult==DialogResult.Cancel && claimProc.IsNew){
				ClaimProcs.Delete(claimProc);
			}
			FillIns();
		}

		private void FillPayments(bool doRefreshData=true){
			if(doRefreshData) {
				_loadData.ListPaySplitsForProc=PaySplits.GetForProcs(ListTools.FromSingle(_procedure.ProcNum));
				_loadData.ListPaymentsForProc=Payments.GetPayments(_loadData.ListPaySplitsForProc.Select(x => x.PayNum).ToList());
			}
			_listPayments=_loadData.ListPaymentsForProc;
			_listPaySplits=PaySplits.GetForProc(_procedure.ProcNum,_loadData.ListPaySplitsForProc.ToArray());
			gridPay.BeginUpdate();
			gridPay.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableProcPay","Entry Date"),70,HorizontalAlignment.Center);
			gridPay.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcPay","Amount"),55,HorizontalAlignment.Right);
			gridPay.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcPay","Tot Amt"),55,HorizontalAlignment.Right);
			gridPay.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcPay","Note"),250,HorizontalAlignment.Left);
			gridPay.Columns.Add(col);
			gridPay.ListGridRows.Clear();
			GridRow row;
			Payment payment;//used in loop
			for(int i=0;i<_listPaySplits.Count;i++){
				row=new GridRow();
				row.Cells.Add((_listPaySplits[i]).DatePay.ToShortDateString());
				row.Cells.Add((_listPaySplits[i]).SplitAmt.ToString("F"));
				row.Cells[row.Cells.Count-1].Bold=YN.Yes;
				payment=Payments.GetFromList((_listPaySplits[i]).PayNum,_listPayments);
				row.Cells.Add(payment.PayAmt.ToString("F"));
				row.Cells.Add(payment.PayNote);
				gridPay.ListGridRows.Add(row);
			}
			gridPay.EndUpdate();
		}

		private void gridPay_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Payment payment=Payments.GetFromList((_listPaySplits[e.Row]).PayNum,_listPayments);
			using FormPayment formPayment=new FormPayment(_patient,_family,payment,false);
			formPayment.PaySplitNumInitial=(_listPaySplits[e.Row]).SplitNum;
			formPayment.ShowDialog();
			FillPayments();
		}

		private void FillAdj(){
			Adjustment[] adjustmentArray=_loadData.ArrAdjustments;
			_listAdjustments=Adjustments.GetForProc(_procedure.ProcNum,adjustmentArray);
			gridAdj.BeginUpdate();
			gridAdj.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableProcAdj","Entry Date"),70,HorizontalAlignment.Center);
			gridAdj.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcAdj","Amount"),55,HorizontalAlignment.Right);
			gridAdj.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcAdj","Type"),100,HorizontalAlignment.Left);
			gridAdj.Columns.Add(col);
			col=new GridColumn(Lan.g("TableProcAdj","Note"),250,HorizontalAlignment.Left);
			gridAdj.Columns.Add(col);
			gridAdj.ListGridRows.Clear();
			GridRow row;
			double discountAmt=0;//Total discount amount from all adjustments of default type.
			for(int i=0;i<_listAdjustments.Count;i++){
				row=new GridRow();
				Adjustment adjustment=_listAdjustments[i];
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
			using FormAdjust formAdjust=new FormAdjust(_patient,_listAdjustments[e.Row]);
			if(formAdjust.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_loadData.ArrAdjustments=Adjustments.GetForProcs(new List<long>() { _procedure.ProcNum }).ToArray();
			FillAdj();
		}

		private void butAddAdjust_Click(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.ProcCompleteAddAdj,Procedures.GetDateForPermCheck(_procedure))) {
				return;
			}
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
			Adjustment adjustment=new Adjustment();
			adjustment.PatNum=_patient.PatNum;
			adjustment.ProvNum=comboProv.GetSelectedProvNum();
			adjustment.DateEntry=DateTime.Today;//but will get overwritten to server date
			adjustment.AdjDate=DateTime.Today;
			adjustment.ProcDate=_procedure.ProcDate;
			adjustment.ProcNum=_procedure.ProcNum;
			adjustment.ClinicNum=_procedure.ClinicNum;
			using FormAdjust formAdjust=new FormAdjust(_patient,adjustment,isTsiAdj);
			formAdjust.IsNew=true;
			if(formAdjust.ShowDialog()!=DialogResult.OK) {
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
			if(!Security.IsAuthorized(Permissions.ProcCompleteAddAdj)) {
				return;
			}
			using FormAdjustmentPicker formAdjustmentPicker=new FormAdjustmentPicker(_patient.PatNum,true,clinicNum:_procedure.ClinicNum, provNum: _procedure.ProvNum);
			if(formAdjustmentPicker.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(!Security.IsAuthorized(Permissions.AdjustmentEdit,formAdjustmentPicker.AdjustmentSelected.AdjDate)) {
				return;
			}
			if(AvaTax.IsEnabled() && 
				(formAdjustmentPicker.AdjustmentSelected.AdjType==AvaTax.GetSalesTaxAdjType() || formAdjustmentPicker.AdjustmentSelected.AdjType==AvaTax.GetSalesTaxReturnAdjType()) && 
					!Security.IsAuthorized(Permissions.SalesTaxAdjEdit)) 
			{
				return;
			}
			decimal estPatPort=ClaimProcs.GetPatPortion(_procedure,_loadData.ListClaimProcsForProc,_loadData.ArrAdjustments.ToList());
			decimal procPatPaid=(decimal)PaySplits.GetTotForProc(_procedure);
			decimal adjRemAmt=estPatPort-procPatPaid+(decimal)formAdjustmentPicker.AdjustmentSelected.AdjAmt;
			if(adjRemAmt<0) {
				EnumAdjustmentBlockOrWarn enumAdjustmentBlockOrWarn=PrefC.GetEnum<EnumAdjustmentBlockOrWarn>(PrefName.AdjustmentBlockNegativeExceedingPatPortion);
				if(enumAdjustmentBlockOrWarn==EnumAdjustmentBlockOrWarn.Warn) {
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Remaining amount is negative.  Continue?","Overpaid Procedure Warning")) {
						return;
					}
				}
				if(enumAdjustmentBlockOrWarn==EnumAdjustmentBlockOrWarn.Block) {
					MsgBox.Show(this,"Cannot create a negative adjustment exceeding the remaining amount on the procedure.","Overpaid Procedure Warning");
					return;
				}
			}
			formAdjustmentPicker.AdjustmentSelected.ProcNum=_procedure.ProcNum;
			formAdjustmentPicker.AdjustmentSelected.ProcDate=_procedure.ProcDate;
			Adjustments.Update(formAdjustmentPicker.AdjustmentSelected);
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
			Procedures.ComputeEstimates(_procedure,_patient.PatNum,ref _listClaimProcs,false,_listInsPlans,_listPatPlans,_listBenefits,
				null,null,true,
				_patient.Age,_listInsSubs,
				null,false,false,_listSubstitutionLinks,false,
				null,_lookupFees);
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
			string surface=textSurfaces.Text;
			Action<string> actionOnFailure=(msg) => {
				errorProvider2.SetError(textSurfaces,msg);//msg is pre-translated
			};
			if(Procedures.ValidateToothValue(textTooth.Text,ref surface, actionOnFailure)) {
				errorProvider2.SetError(textSurfaces,"");//Clear any errors if valid
			}
			textSurfaces.Text=surface;//Update UI with tidy surface value.
			SetSurfButtons();
		}

		private void groupSextant_Validating(object sender,CancelEventArgs e) {
			SetErrorProviderForSextant();
		}

		private void SetErrorProviderForSextant() {
			Action<string> actionOnFailure=(msg) => {
				errorProvider2.SetError(groupSextant,msg);
			};
			if(Procedures.ValidateSextant(IsSextantSelected(),actionOnFailure)) {
				errorProvider2.SetError(groupSextant,"");
			}
		}

		private bool IsSextantSelected() {
			return groupSextant.Controls.OfType<RadioButton>().Any(x => x.Checked);
		}

		private void groupArch_Validating(object sender,CancelEventArgs e) {
			SetErrorProviderForGroupArc();
		}

		private void SetErrorProviderForGroupArc() {
			Action<string> actionOnFailure=(msg) => {
				errorProvider2.SetError(groupArch,msg);
			};
			if(Procedures.ValidateArch(IsArchSelected(),actionOnFailure)) {
				errorProvider2.SetError(groupArch,"");
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
			using FormProcCodes formProcCodes=new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			formProcCodes.ShowDialog();
			if(formProcCodes.DialogResult!=DialogResult.OK){
				return;
			}
			_listFees=null;
			_lookupFees=null;//will trigger another lazy load, later, with this new code.
			Procedure procedureOld=_procedure.Copy();
			ProcedureCode procedureCodeOld=ProcedureCodes.GetProcCode(_procedure.CodeNum);
			ProcedureCode procedureCode=ProcedureCodes.GetProcCode(formProcCodes.CodeNumSelected);
			if(procedureCodeOld.TreatArea != procedureCode.TreatArea
				|| procedureCodeOld.AreaAlsoToothRange != procedureCode.AreaAlsoToothRange) 
			{
				MsgBox.Show(this,"Not allowed due to treatment area mismatch.");
				return;
			}
			_procedure.CodeNum=formProcCodes.CodeNumSelected;
			_procedure.ProcFee=Procedures.GetProcFee(_patient,_listPatPlans,_listInsSubs,_listInsPlans,_procedure.CodeNum,_procedure.ProvNum,_procedure.ClinicNum
				,_procedure.MedicalCode,listFees:_listFees);
			switch(procedureCode.TreatArea){ 
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
			ClearClaimProcs(_listClaimProcs);
			_listClaimProcs=new List<ClaimProc>();
			Procedures.ComputeEstimates(_procedure,_patient.PatNum,ref _listClaimProcs,true,_listInsPlans,_listPatPlans,_listBenefits,
				null,null,true,
				_patient.Age,_listInsSubs,
				null,false,false,_listSubstitutionLinks,false,
				null,_lookupFees);
			#region New Procedure Code Overallocated
			if(_listPaySplits.Count>0 || _listAdjustments.Count>0) {
				//Need to refresh from the database because this Procedures.ComputeEstimates() may have lost the reference to a new list.
				_listClaimProcs=ClaimProcs.RefreshForProc(_procedure.ProcNum);
				double procFeeOld=procedureOld.ProcFeeTotal;
				double procFee=_procedure.ProcFeeTotal;
				double sumWriteoff=ClaimProcs.ProcWriteoff(_listClaimProcs,_procedure.ProcNum);
				double sumInsPaids=ClaimProcs.ProcInsPay(_listClaimProcs,_procedure.ProcNum);
				double sumInsEstsNotReceived=ClaimProcs.ProcEstNotReceived(_listClaimProcs,_procedure.ProcNum);
				//Adjustments are already negative if a discount, etc.
				double sumAdjs=_listAdjustments.Cast<Adjustment>().ToList().FindAll(x => x.ProcNum==_procedure.ProcNum).Sum(x => x.AdjAmt);
				double sumPaySplits=_listPaySplits.Cast<PaySplit>().ToList().FindAll(x => x.ProcNum==_procedure.ProcNum).Sum(x => x.SplitAmt);
				double credits=sumWriteoff+sumInsPaids+sumInsEstsNotReceived-sumAdjs+sumPaySplits;
				//Check if the new ProcCode will result in the procedure being overallocated due to a change in ProcFee.
				if(credits>procFee) {//Procedure is overallocated.
					string strMsg=Lan.g(this,"The fee will be changed from")+" "+procFeeOld.ToString("c")+" "
						+Lan.g(this,"to")+" "+procFee.ToString("c")
						+Lan.g(this,", and the procedure has credits attached in the amount of")+" "+credits.ToString("c")
						+Lan.g(this,".  This will result in an overallocated procedure")+".\r\n"+Lan.g(this,"Continue?");
					//Prompt user to accept the overallocation or revert back to old ProcCode.
					if(MessageBox.Show(this,strMsg,Lan.g(this,"Overpaid Procedure Warning"),MessageBoxButtons.YesNo)==DialogResult.No) {
						_procedure=procedureOld;
						ClearClaimProcs(_listClaimProcs);
						Procedures.ComputeEstimates(_procedure,_patient.PatNum,ref _listClaimProcs,false,_listInsPlans,_listPatPlans,_listBenefits,
							null,null,true,
							_patient.Age,_listInsSubs,
							null,false,false,_listSubstitutionLinks,false,
							null,_lookupFees);
						_listClaimProcs=ClaimProcs.RefreshForProc(_procedure.ProcNum);
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
			List<Permissions> listPermissionss=new List<Permissions>();
			List<long> listClaimProcClaimNums=listClaimProcs.Select(x=>x.ClaimNum).ToList();
			List<Claim> listClaimsSentOrReceived=listClaims.Where(x=>x.ClaimStatus=="R" || x.ClaimStatus=="S").ToList();
			for(int i=0;i<listClaimsSentOrReceived.Count;i++) {
				Claim claim=listClaimsSentOrReceived[i];
				if(listClaimProcClaimNums.Contains(claim.ClaimNum)) {
					hasSentOrRecPreauth|=claim.ClaimType=="PreAuth";
					hasSentOrRecClaim|=claim.ClaimType!="PreAuth";
				}
			}
			if(hasSentOrRecPreauth) {
				listPermissionss.Add(Permissions.PreAuthSentEdit);
			}
			if(hasSentOrRecClaim) {
				listPermissionss.Add(Permissions.ClaimSentEdit);
			}
			bool isAllowed=true;
			for(int i=0;i<listPermissionss.Count();i++) {
				if(listPermissionss[i]==Permissions.PreAuthSentEdit) {
					isAllowed&=Security.IsAuthorized(listPermissionss[i],dateOldestPreAuth);
				}
				else {
					isAllowed&=Security.IsAuthorized(listPermissionss[i],dateOldestClaim);
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
			if(ProcedureL.IsProcCompleteAttachedToClaim(_procedureOld,_listClaimProcs)) {
				comboProcStatus.SetSelectedEnum(ProcStat.C);//Complete
				return;
			}
			if(comboProcStatus.GetSelected<ProcStat>()==ProcStat.TP) {//fee starts out 0 if EO, EC, etc.  This updates fee if changing to TP so it won't stay 0.
				_procedure.ProcStatus=ProcStat.TP;
				if(_procedure.ProcFee==0) {
					_procedure.ProcFee=Procedures.GetProcFee(_patient,_listPatPlans,_listInsSubs,_listInsPlans,_procedure.CodeNum,_procedure.ProvNum,_procedure.ClinicNum,
						_procedure.MedicalCode,listFees:_listFees);
					textProcFee.Text=_procedure.ProcFee.ToString("f");
				}
			}
			if(comboProcStatus.GetSelected<ProcStat>()==ProcStat.C) {
				bool isAllowedToCompl=true;
				if(!PrefC.GetBool(PrefName.AllowSettingProcsComplete)) {
					MsgBox.Show(this,"Set the procedure complete by setting the appointment complete. "
						+"If you want to be able to set procedures complete, you must turn on that option in Setup | Preferences | Chart - Procedures.");
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
						if(_procedure.ProcStatus.In(ProcStat.EC,ProcStat.EO,ProcStat.R,ProcStat.Cn))
						{
							comboProcStatus.SetSelectedEnum(_procedure.ProcStatus);
						}
					}
					return;
				}
				if(_procedure.AptNum!=0) {//if attached to an appointment
					Appointment appointment=Appointments.GetOneApt(_procedure.AptNum);
					if(appointment.AptDateTime.Date > MiscData.GetNowDateTime().Date) {//if appointment is in the future
						MessageBox.Show(Lan.g(this,"Not allowed because procedure is attached to a future appointment with a date of ")
							+appointment.AptDateTime.ToShortDateString());
						return;
					}
					if(appointment.AptDateTime.Year<1880) {
						textDate.Text=MiscData.GetNowDateTime().ToShortDateString();
					}
					else {
						textDate.Text=appointment.AptDateTime.ToShortDateString();
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
			if(selecteStat.In(ProcStat.EC,ProcStat.EO,ProcStat.R,ProcStat.Cn)) {
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
				MsgBox.Show(this,"Set the procedure complete by setting the appointment complete. "
					+"If you want to be able to set procedures complete, you must turn on that option in Setup | Preferences | Chart - Procedures.");
				return;
			}
			//If user is trying to change status to complete and using eCW.
			if((IsNew || _procedureOld.ProcStatus!=ProcStat.C) && Programs.UsingEcwTightOrFullMode()) {
				MsgBox.Show(this,"Procedures cannot be set complete in this window.  Set the procedure complete by setting the appointment complete.");
				return;
			}
			if(ProcedureCodes.AreAnyProcCodesHidden(_procedure.CodeNum)) {
				MsgBox.Show($"{Lan.g(this,"Procedure cannot be set complete because it is in a hidden category")}: {ProcedureCodes.GetProcCode(_procedure.CodeNum).ProcCode}");
				return;
			}
			DateTime dateTimeProc=DateTime.MinValue;
			if(_procedure.AptNum!=0){//if attached to an appointment
				Appointment appointment=Appointments.GetOneApt(_procedure.AptNum);
				if(appointment==null) {//Appointment was deleted before proc could be set complete.
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Appointment has been deleted by another user. Continue with setting this procedure complete?")) {
						return;
					}
					dateTimeProc=MiscData.GetNowDateTime();
					_procedure.AptNum=0;//Remove the attached appointment.
				}
				else {
					if(appointment.AptDateTime.Date > MiscData.GetNowDateTime().Date){//if appointment is in the future
						MessageBox.Show(Lan.g(this,"Not allowed because procedure is attached to a future appointment with a date of ")
							+appointment.AptDateTime.ToShortDateString());
						return;
					}
					if(appointment.AptDateTime.Year<1880) {
						dateTimeProc=MiscData.GetNowDateTime();
					}
					else {
						dateTimeProc=appointment.AptDateTime;
					}
				}
			}
			else{
				dateTimeProc=MiscData.GetNowDateTime();
			}
			//Use procDateNew since this is the date that the procedure would end up as
			if(!Security.IsAuthorized(Permissions.ProcComplCreate,dateTimeProc,_procedure.CodeNum,PIn.Double(textProcFee.Text))) {
				return;
			}
			//broken appointment procedure codes shouldn't trigger DateFirstVisit update.
			if(ProcedureCodes.GetStringProcCode(_procedure.CodeNum)!="D9986" && ProcedureCodes.GetStringProcCode(_procedure.CodeNum)!="D9987") {
				Procedures.SetDateFirstVisit(DateTime.Today,2,_patient);
			}
			textDate.Text=dateTimeProc.ToShortDateString();
			if(_procedureCode.PaintType==ToothPaintingType.Extraction){//if an extraction, then mark previous procs hidden
				//Procedures.SetHideGraphical(ProcCur);//might not matter anymore
				ToothInitials.SetValue(_procedure.PatNum,_procedure.ToothNum,ToothInitialType.Missing);
			}
			_procedure.ProcStatus=ProcStat.C;
			_procedure.SiteNum=_patient.SiteNum;
			ProcNoteUiHelper();
			Plugins.HookAddCode(this,"FormProcEdit.butSetComplete_Click_end",_procedure,_procedureOld,textNotes);
			comboProcStatus.SelectedIndex=-1;
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
			if(_procedureOld.ProcStatus!=ProcStat.C && _procedure.ProcStatus==ProcStat.C) {//Only append the default note if the procedure changed status to Completed
				procNoteDefault+=ProcCodeNotes.GetNote(comboProv.GetSelectedProvNum(),_procedure.CodeNum,ProcStat.C);
				if(textNotes.Text!="" && procNoteDefault!="") { //check to see if a default note is defined.
					textNotes.Text+="\r\n"; //add a new line if there was already a ProcNote on the procedure.
				}
				if(!string.IsNullOrEmpty(procNoteDefault)) {
					textNotes.Text+=procNoteDefault;
				}
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
			for(int i=0;i<_listClaimProcs.Count;i++) {
				if(!_listClaimProcs[i].Status.In(ClaimProcStatus.Estimate,ClaimProcStatus.CapClaim,ClaimProcStatus.CapEstimate)) {
					continue;
				}
				_listClaimProcs[i].NoBillIns=(checkNoBillIns.CheckState==CheckState.Checked);
				ClaimProcs.Update(_listClaimProcs[i]);
			}
			//next lines are needed to recalc BaseEst, etc, for claimprocs that are no longer NoBillIns
			//also, if they are NoBillIns, then it clears out the other values.
			Procedures.ComputeEstimates(_procedure,_patient.PatNum,ref _listClaimProcs,false,_listInsPlans,_listPatPlans,_listBenefits,
				null,null,true,
				_patient.Age,_listInsSubs,
				null,false,false,_listSubstitutionLinks,false,
				null,_lookupFees
				);
			FillIns();
			Cursor=Cursors.Default;
		}

		private void textNotes_TextChanged(object sender, System.EventArgs e) {
			CheckForCompleteNote();
			if(!_isStartingUp//so this happens only if user changes the note
				&& !_signatureChanged)//and the original signature is still showing.
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
			using InputBox inputBox=new InputBox(Lan.g(this,"Search for"));
			inputBox.ShowDialog();
			if(inputBox.DialogResult!=DialogResult.OK) {
				return;
			}
			string searchText=inputBox.textResult.Text;
			int index=textNotes.Find(inputBox.textResult.Text);//Gets the location of the first character in the control.
			if(index<0) {//-1 is returned when the text is not found.
				textNotes.DeselectAll();
				MessageBox.Show("\""+searchText+"\"\r\n"+Lan.g(this,"was not found in the notes")+".");
				return;
			}
			textNotes.Select(index,searchText.Length);
		}

		private void signatureBoxWrapper_SignatureChanged(object sender,EventArgs e) {
			_signatureChanged=true;
			_procedure.UserNum=_Userod.UserNum;
			textUser.Text=_Userod.UserName;
		}

		private void buttonUseAutoNote_Click(object sender,EventArgs e) {
			using FormAutoNoteCompose formAutoNoteCompose=new FormAutoNoteCompose();
			formAutoNoteCompose.ShowDialog();
			if(formAutoNoteCompose.DialogResult==DialogResult.OK) {
				textNotes.AppendText(formAutoNoteCompose.StrCompletedNote);
				bool hasAutoNotePrompt=Regex.IsMatch(textNotes.Text,Procedures.AutoNotePromptRegex);
				butEditAutoNote.Visible=hasAutoNotePrompt;
			}
		}

		private void butEditAutoNote_Click(object sender,EventArgs e) {
			if(Regex.IsMatch(textNotes.Text,Procedures.AutoNotePromptRegex)) {
				using FormAutoNoteCompose formAutoNoteCompose=new FormAutoNoteCompose();
				formAutoNoteCompose.StrMainTextNote=textNotes.Text;
				formAutoNoteCompose.ShowDialog();
				if(formAutoNoteCompose.DialogResult==DialogResult.OK) {
					textNotes.Text=formAutoNoteCompose.StrCompletedNote;
					bool hasAutoNotePrompt=Regex.IsMatch(textNotes.Text,Procedures.AutoNotePromptRegex);
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
				if(Tooth.IsAnterior(Tooth.Parse(textTooth.Text))) {
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
				if(Tooth.IsAnterior(Tooth.Parse(textTooth.Text))) {
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
			using FormSites formSites=new FormSites();
			formSites.IsSelectionMode=true;
			formSites.SiteNumSelected=_procedure.SiteNum;
			formSites.ShowDialog();
			if(formSites.DialogResult!=DialogResult.OK){
				return;
			}
			_procedure.SiteNum=formSites.SiteNumSelected;
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
			Permissions permissions=GroupPermissions.SwitchExistingPermissionIfNeeded(Permissions.ProcCompleteStatusEdit,_procedure);
			DateTime dateForPerm=Procedures.GetDateForPermCheck(_procedure);
			//What this will really do is "delete" the procedure.
			if(!Security.IsAuthorized(permissions,dateForPerm)) {
				return;
			}
			if(Procedures.IsAttachedToClaim(_procedure,_listClaimProcs)) {
				MsgBox.Show(this,"This procedure is attached to a claim and cannot be invalidated without first deleting the claim.");
				return;
			}
			try {
				Procedures.Delete(_procedure.ProcNum,hideGraphics:true);//also deletes any claimprocs (other than ins payments of course) and hides graphics.
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			SecurityLogs.MakeLogEntry(permissions,_patient.PatNum,Lan.g(this,"Invalidated: ")+
				ProcedureCodes.GetStringProcCode(_procedure.CodeNum).ToString()+" ("+_procedure.ProcStatus+"), "
				+_procedure.ProcDate.ToShortDateString()+", "+_procedure.ProcFee.ToString("c")+", Deleted");
			DialogResult=DialogResult.OK;
		}

		///<summary>This button is only visible when proc IsLocked.</summary>
		private void butAppend_Click(object sender,EventArgs e) {
			using FormProcNoteAppend formProcNoteAppend=new FormProcNoteAppend();
			formProcNoteAppend.ProcedureCur=_procedure;
			formProcNoteAppend.ShowDialog();
			if(formProcNoteAppend.DialogResult!=DialogResult.OK) {
				return;
			}
			DialogResult=DialogResult.OK;//exit out of this window.  Change already saved, and OK button is disabled in this window, anyway.
		}

		private void butSnomedBodySiteSelect_Click(object sender,EventArgs e) {
			using FormSnomeds formSnomeds=new FormSnomeds();
			formSnomeds.IsSelectionMode=true;
			if(formSnomeds.ShowDialog()==DialogResult.OK) {
				_snomedBodySite=formSnomeds.SnomedSelected;
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
				using FormIcd10s formIcd10s=new FormIcd10s();
				formIcd10s.IsSelectionMode=true;
				if(formIcd10s.ShowDialog()==DialogResult.OK) {
					textBoxDiagnosisCode.Text=formIcd10s.Icd10Selected.Icd10Code;
				}
			}
			else {//ICD-9
				using FormIcd9s formIcd9s=new FormIcd9s();
				formIcd9s.IsSelectionMode=true;
				if(formIcd9s.ShowDialog()==DialogResult.OK) {
					textBoxDiagnosisCode.Text=formIcd9s.ICD9Selected.ICD9Code;
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
			if(!_procedureOld.ProcStatus.In(ProcStat.C,ProcStat.EO,ProcStat.EC)
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
				ClaimProc claimProcPreAuth=_listClaimProcs.Where(x=>x.ProcNum==_procedureOld.ProcNum && x.ClaimNum!=0 && x.Status==ClaimProcStatus.Preauth).FirstOrDefault();
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
			if(PrefC.GetBool(PrefName.ApptsRequireProc)) {
				bool areApptsGoingToBeEmpty=Appointments.AreApptsGoingToBeEmpty(new List<Procedure>() { _procedure });
				if(areApptsGoingToBeEmpty && !MsgBox.Show(this,MsgBoxButtons.YesNo,
						"This is the only procedure attached to an appointment. If you delete this procedure, it will leave the appointment empty. Continue?"))
				{
					return;
				}
			}
			try {
				Procedures.Delete(_procedure.ProcNum);//also deletes the claimProcs and adjustments. Might throw exception.
				Recalls.Synch(_procedure.PatNum);//needs to be moved into Procedures.Delete
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			_isEstimateRecompute=true;
			Permissions permissions=Permissions.ProcDelete;
			string tag="";
			switch(_procedureOld.ProcStatus) {
				case ProcStat.C:
					permissions=Permissions.ProcCompleteStatusEdit;
					tag=", "+Lan.g(this,"Deleted");
					break;
				case ProcStat.EO:
				case ProcStat.EC:
					permissions=Permissions.ProcExistingEdit;
					tag=", "+Lan.g(this,"Deleted");
					break;
			}
			SecurityLogs.MakeLogEntry(permissions,_procedureOld.PatNum,
				ProcedureCodes.GetProcCode(_procedureOld.CodeNum).ProcCode+" ("+_procedureOld.ProcStatus+"), "+_procedureOld.ProcFee.ToString("c")+tag);
			DialogResult=DialogResult.OK;
			Plugins.HookAddCode(this,"FormProcEdit.butDelete_Click_end",_procedure);
		}

		private bool EntriesAreValid() {
			//
			// This method is mimicked in OD mobile apps, please inform a mobile app developer if you think changes will be needed on that end as well.
			// Most logic should be in Procedures.EntriesAreValid(...). Things like UI validation and syncing can happen here in FormProcEdit.cs though.
			//
			string translationSource=nameof(FormProcEdit);//This maintains existing translations, Procedures.EntriesAreValid(...) use to exist in this file.
			Action<string> actionOnFailure=(msg) => {
				MsgBox.Show(msg);//Pre translated msg
			};
			Func<string,bool> actionYesNoPrompt=(msg) => {
				return MsgBox.Show(MsgBoxButtons.YesNo,msg);//Pre translated msg
			};
			Action actionOnProcedureCodeFailure=() => {
				string error="The procedure code you were trying to chart appears not to exist any more. Please use the change button to select a new procedure code.";
				MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(error);
				msgBoxCopyPaste.Text="Error: Procedure Code Invalid";
				msgBoxCopyPaste.Show();//Use .Show() to make it easy for the user to keep this window open while they call in.
			};
			#region Surfaces, Tooth, Sextant, Arch, Date UI
			if(!textDateTP.IsValid()
				|| !textDate.IsValid()
				|| !textProcFee.IsValid()
				|| !textDateOriginalProsth.IsValid()
				|| !textDiscount.IsValid())
			{
				actionOnFailure.Invoke(Lan.g(translationSource,"Please fix data entry errors first."));
				return false;
			}
			if(_procedure.ProcStatus!=ProcStat.EO && textDate.Text=="") {//Only ProcStat.EO can be empty.
				actionOnFailure.Invoke(Lans.g(translationSource,"Please enter a date first."));
				return false;
			}
			#endregion
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && (!textCanadaLabFee1.IsValid() || !textCanadaLabFee2.IsValid())) {
				actionOnFailure.Invoke(Lans.g(translationSource,"Please fix lab fees."));
				return false;
			}
			bool isCheckTypeCodeNonXChecked=checkTypeCodeA.Checked
						|| checkTypeCodeB.Checked
						|| checkTypeCodeC.Checked
						|| checkTypeCodeE.Checked
						|| checkTypeCodeL.Checked
						|| checkTypeCodeS.Checked;
			bool isQuadrantSelected=radioUL.Checked || radioUR.Checked || radioLL.Checked || radioLR.Checked;
			bool hasOrthoProcLink=_orthoProcLink!=null;
			bool isTextDrugNdcNotBlank=textDrugNDC.Text!="";
			bool isSigChangedAndNotBlank=_signatureChanged && !signatureBoxWrapper.SigIsBlank;
			int unityQty=PIn.Int(textUnitQty.Text, false);
			bool isTextDateOriginalProsthBlank=textDateOriginalProsth.Text=="";
			DateTime dateTimeProc=PIn.Date(textDate.Text);
			bool result=Procedures.EntriesAreValid(textNotes.Text,isSigChangedAndNotBlank,textTimeStart.Text,textTimeEnd.Text,unityQty,comboProv.GetSelectedProvNum(),textMedicalCode.Text,isTextDrugNdcNotBlank,textDrugQty.Text,
				ref dateTimeProc,IsNew,PIn.Double(textProcFee.Text),_isQuickAdd,isCheckTypeCodeNonXChecked,checkTypeCodeX.Checked,listProsth.SelectedIndex,isQuadrantSelected,
				hasOrthoProcLink,isTextDateOriginalProsthBlank,comboDrugUnit.SelectedIndex,textSurfaces.Text,textTooth.Text,IsSextantSelected(),IsArchSelected(),
				_procedure,_procedureOld,ref _procedureCode,translationSource,
				actionOnFailure,actionYesNoPrompt,actionOnProcedureCodeFailure,
				ref _listClaimProcs
			);
			textDate.Text=dateTimeProc.ToShortDateString();//The value can be changed in EntriesAreValid(...)
			return result;
		}

		///<summary>MUST call EntriesAreValid first.  Used from OK_Click and from butSetComplete_Click</summary>
		private void SaveAndClose() {
			//
			// This method is mimicked in OD mobile apps, please inform a mobile app developer if you think changes will be needed on that end as well.
			//
			#region Setup helper functions and translation source
			Func<string,bool> funcYesNoPrompt=(msg) => { 
				return MsgBox.Show(MsgBoxButtons.YesNo,msg);
			};
			Action<string> actionMsgBox=(msg) => {
				MsgBox.Show(msg);//Pre-translated msg
			};
			Func<long,Procedure> funcPromptFormACLI=(verifyCode) => { 
				using FormAutoCodeLessIntrusive formAutoCodeLessIntrusive=new FormAutoCodeLessIntrusive(_patient,_procedure,_procedureCode,verifyCode,_listPatPlans,_listInsSubs,_listInsPlans,
						_listBenefits,_listClaimProcs,listBoxTeeth.Text
				);
				if(formAutoCodeLessIntrusive.ShowDialog() != DialogResult.OK
					&& PrefC.GetBool(PrefName.ProcEditRequireAutoCodes)) 
				{
					return null;//send user back to fix information or use suggested auto code.
				}
				return formAutoCodeLessIntrusive.ProcedureCur;
			};
			string translationSource=nameof(FormProcEdit);
			#endregion Setup helper functions and translation source
			#region Sync UI with object fields.
			if(textProcFee.Text=="") {
				textProcFee.Text="0";
			}
			Procedures.UpdateProcedureFields(_procedure,_patient,textMedicalCode.Text,PIn.Double(textDiscount.Text),
				_snomedBodySite,checkIcdVersion.Checked,GetListDiagnosticCodes(),checkIsPrincDiag.Checked,_provNumSelectedOrder,_referralOrdering,
				textCodeMod1.Text,textCodeMod2.Text,textCodeMod3.Text,textCodeMod4.Text,PIn.Int(textUnitQty.Text),(ProcUnitQtyType)comboUnitType.SelectedIndex,
				textRevCode.Text,(EnumProcDrugUnit)comboDrugUnit.SelectedIndex,PIn.Float(textDrugQty.Text),(checkIsEmergency.Checked?ProcUrgency.Emergency:ProcUrgency.Normal),
				comboProv.GetSelectedProvNum(),comboClinic.SelectedClinicNum
			);
			ClaimProcs.TrySetProvFromProc(_procedure,_listClaimProcs);
			#endregion Sync UI with object fields.
			#region Verify security authorization for provider change and completed proc status change. Can also change a few procedure field values.
			if(!Procedures.VerifyProviderChange(_procedure,_procedureOld,_listAdjustments.Cast<Adjustment>().ToList(),out bool hasSplitProvChanged,out bool hasAdjProvChanged,
				translationSource,funcYesNoPrompt,actionMsgBox))
			{
				return;
			}
			if(!Procedures.VerifyCompletedProcStatusChange(_listPaySplits.Cast<PaySplit>().ToList(),_procedure,_procedureOld,translationSource,funcYesNoPrompt,actionMsgBox)) {
				return;
			}
			#endregion Verify security authorization for provider change and completed proc status change. Can also change a few procedure field values.
			#region Additional UI syncing (_procedure dates and fee).
			Procedures.SetMiscDateAndTimeEditFields(_procedure,textDateTP.Text,PIn.Date(textDate.Text),textTimeStart.Text,textTimeEnd.Text);
			DateTime procedureDate=DateTime.Parse(textDate.Text);
			for(int i=0;i<_listClaimProcs.Count;i++) {//if the proc date has changed update the ClaimProcs
				if(_listClaimProcs[i].DateCP!=procedureDate && procedureDate.Year>1880) {
					ClaimProc claimProcOld=_listClaimProcs[i].Copy();
					_listClaimProcs[i].ProcDate=procedureDate;
					ClaimProcs.Update(_listClaimProcs[i],claimProcOld);
				}
			}
			_procedure.ProcFee=PIn.Double(textProcFee.Text);
			#endregion Additional UI syncing (_procedure dates and fee).
			#region Tooth UI cleanup and validation, set various tooth fields for _procedure.
			ClearAdultToothSelections();
			if(!Procedures.SetAndValidateToothData(_procedureCode,_procedure,textTooth.Text,textSurfaces.Text,
				listBoxTeeth.SelectedIndices.Cast<int>().ToList(),listBoxTeeth2.SelectedIndices.Cast<int>().ToList(),_listPriTeeths,translationSource,actionMsgBox))
			{
				return;
			}
			#endregion Tooth UI cleanup and validation, set various tooth fields for _procedure.
			//Point of no return.
			#region Try save signature, doesn't do anything if _signatureChanged false.
			try {
				SaveSignature();
			}
			catch(Exception ex) {
				actionMsgBox.Invoke(Lans.g(translationSource,"Error saving signature.")+"\r\n"+ex.Message);
				//and continue with the rest of this method
			}
			#endregion Try save signature, doesn't do anything if _signatureChanged false.
			#region Update DB, paysplits and adjustments
			if(hasSplitProvChanged) {
				PaySplits.UpdateAttachedPaySplits(_procedure);//update the attached paysplits.
			}
			if(hasAdjProvChanged) {
				for(int i=0;i<_listAdjustments.Count;i++) {//update the attached adjustments
					_listAdjustments[i].ProvNum=_procedure.ProvNum;
					Adjustments.Update(_listAdjustments[i]);
				}
			}
			#endregion Update DB, paysplits and adjustments
			#region Additional UI syncing (_procedure fields). In Canada, Procedures.CanadianLabHelper(...) can also create and/or update rows in the DB.
			Procedures.SetNote(_procedure,_procedureOld,textNotes.Text);
			_procedure.HideGraphics=checkHideGraphics.Checked;
			if(comboDx.SelectedIndex!=-1) {
				_procedure.Dx=_listDefsDiagnosis[comboDx.SelectedIndex].DefNum;
			}
			_procedure.Prognosis=comboPrognosis.SelectedIndex==0 ? 0 : _listDefsPrognosis[comboPrognosis.SelectedIndex - 1].DefNum;
			_procedure.Priority=comboPriority.SelectedIndex==0 ? 0 : _listDefsTxPriority[comboPriority.SelectedIndex-1].DefNum;
			_procedure.PlaceService=(PlaceOfService)comboPlaceService.SelectedIndex;
			_procedure.BillingTypeOne=comboBillingTypeOne.SelectedIndex==0 ? 0 : _listDefsBillingType[comboBillingTypeOne.SelectedIndex-1].DefNum;
			_procedure.BillingTypeTwo=comboBillingTypeTwo.SelectedIndex==0 ? 0 : _listDefsBillingType[comboBillingTypeTwo.SelectedIndex-1].DefNum;
			_procedure.BillingNote=textBillingNote.Text;
			//ProcCur.HideGraphical=checkHideGraphical.Checked;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				Procedures.SetCanadianEditFields(_procedure,checkTypeCodeA.Checked,checkTypeCodeB.Checked,
					checkTypeCodeC.Checked,checkTypeCodeE.Checked,checkTypeCodeL.Checked,checkTypeCodeS.Checked,checkTypeCodeX.Checked
				);
				Procedures.CanadianLabHelper(textCanadaLabFee1.Text,_procedureCode.IsCanadianLab,listProcedures,_procedure,textCanadaLabFee2.Text);
			}
			else {
				//Sets various prosth based _procedure fields.
				Procedures.SetProsthEditFields(_procedureCode,_procedure,listProsth.SelectedIndex,PIn.Date(textDateOriginalProsth.Text),checkIsDateProsthEst.Checked);
			}
			_procedure.ClaimNote=textClaimNote.Text;
			#endregion Additional UI syncing (_procedure fields). In Canada, Procedures.SetCanadianEditFields(...) can also create and/or update rows in the DB.
			//Last chance to run this code before Proc gets updated.
			Procedures.TryValidateProcFee(_procedure,_procedureOld,_patient,_listFees,_listPatPlans,_listInsSubs,_listInsPlans,_listBenefits,funcYesNoPrompt);
			Procedures.TryAutoCodesPrompt(ref _procedure,_procedureOld,_procedureCode,(listBoxTeeth.SelectedIndices.Count < 1),_patient,ref _listClaimProcs,funcPromptFormACLI);
			bool isProcLinkedToOrthoCase=Procedures.IsProcLinkedToOrthoCase(_procedureOld,_procedure,ref _orthoProcLink);
			//The actual update----------------------------------------------------------------------------------------------------------------------------------
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && _procedure.ProcNumLab!=0) {
				//Update Canadian Lab Fees because they do not get updated singly in Procedures.FormProcEditUpdate
				Procedures.Update(_procedure,_procedureOld,false,isProcLinkedToOrthoCase:isProcLinkedToOrthoCase);
			}
			Procedures.FormProcEditUpdate(_procedure,_procedureOld,_procedureCode,isProcLinkedToOrthoCase,IsNew,listBoxTeeth.Text);
			for(int i=0;i<_listClaimProcs.Count;i++) {
				_listClaimProcs[i].ClinicNum=comboClinic.SelectedClinicNum;//These changes save in Form_Closing ComputeEstimates depending on DialogResult
			}
			//Recall synch---------------------------------------------------------------------------------------------------------------------------------
			Recalls.Synch(_procedure.PatNum);
			if(_procedureOld.ProcStatus!=ProcStat.C && _procedure.ProcStatus==ProcStat.C) {
				List<string> listprocCodes=new List<string>();
				listprocCodes.Add(ProcedureCodes.GetStringProcCode(_procedure.CodeNum));
				AutomationL.Trigger(AutomationTrigger.CompleteProcedure,listprocCodes,_procedure.PatNum);
				Procedures.AfterProcsSetComplete(new List<Procedure>() { _procedure });
			}
			DialogResult=DialogResult.OK;
			//it is assumed that we will do an immediate refresh after closing this window.
		}

		///<summary>Returns a list of all the diagnostic code boxes.</summary>
		private List<string> GetListDiagnosticCodes() {
			List<string> diagnosticCodes=new List<string>();
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
			return diagnosticCodes;
		}

		private void ClearAdultToothSelections() {
			//Dx taken care of when radio pushed
			if(_procedureCode.TreatArea==TreatmentArea.ToothRange || _procedureCode.AreaAlsoToothRange) {
				//Deselect empty tooth selections in Maxillary/Upper Arch.
				for(int j=0; j<listBoxTeeth.Items.Count; j++) {
					if(listBoxTeeth.Items[j].ToString()=="") {//Can be blank when the tooth is flagged as primary when it is an adult tooth.
						listBoxTeeth.SetSelected(j,false);
					}
				}
				//Deselect empty tooth selections in Mandibular/Lower Arch.
				for(int j=0; j<listBoxTeeth2.Items.Count; j++) {
					if(listBoxTeeth2.Items[j].ToString()=="") {//Can be blank when the tooth is flagged as primary when it is an adult tooth.
						listBoxTeeth2.SetSelected(j,false);
					}
				}
			}
		}

		private void butChangeUser_Click(object sender,EventArgs e) {
			using FormLogOn formLogOn=new FormLogOn(isSimpleSwitch:true);
			formLogOn.ShowDialog();
			if(formLogOn.DialogResult==DialogResult.OK) { //if successful
				_Userod=formLogOn.UserodSimpleSwitch; //assign temp user
				bool canUserSignNote=Userods.CanUserSignNote(_Userod);//only show if user can sign
				signatureBoxWrapper.Enabled=canUserSignNote;
				if(!labelPermAlert.Visible && !canUserSignNote) {
					labelPermAlert.Text=Lans.g(this,"Notes can only be signed by providers.");
					labelPermAlert.Visible=true;
				}
				FillComboClinic();
				FillComboProv();
				signatureBoxWrapper.ClearSignature(); //clear sig
				signatureBoxWrapper.UserSig=_Userod;
				textUser.Text=_Userod.UserName; //update user textbox.
				_signatureChanged=true;
				_hasUserChanged=true;
			}
		}

		private void SaveSignature(){
			if(_signatureChanged){
				string keyData=Procedures.GetSignatureKeyData(_procedure, textNotes.Text);
				_procedure.Signature=signatureBoxWrapper.GetSignature(keyData);
				_procedure.SigIsTopaz=signatureBoxWrapper.GetSigIsTopaz();
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(!EntriesAreValid()) {
				return;
			}
			if(!Security.IsAuthorized(Permissions.ProcComplCreate,PIn.Date(textDate.Text),_procedure.CodeNum,PIn.Double(textProcFee.Text))) {
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
			if(_procedure.ProcStatus==ProcStat.C && !_isQuickAdd){
				ProcNoteUiHelper();
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
			if(Userods.IsUserCpoe(_Userod) && !_procedureOld.IsCpoe) {
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
				if(ClaimProcs.IsAttachedToDifferentClaim(_procedure.ProcNum,_listClaimProcs)) {
					return;//The claimproc is not attached to the same claim it was originally pointing to.  Do not run ComputeEstimates which would point it back to the old (potentially deleted) claim.
				}
				List<ClaimProcHist> histList=ClaimProcs.GetHistList(_procedure.PatNum,_listBenefits,_listPatPlans,_listInsPlans,-1,DateTime.Today,_listInsSubs);
				//We don't want already existing claim procs on this procedure to affect the calculation for this procedure.
				histList.RemoveAll(x => x.ProcNum==_procedure.ProcNum);
				Procedures.ComputeEstimates(_procedure,_patient.PatNum,ref _listClaimProcs,_isQuickAdd,_listInsPlans,_listPatPlans,_listBenefits,
					histList,new List<ClaimProcHist> { },true,
					_patient.Age,_listInsSubs,
					null,false,false,_listSubstitutionLinks,false,
					null,_lookupFees,_orthoProcLink);
				if(_isEstimateRecompute
					&& _procedure.ProcNumLab!=0//By definition of procedure.ProcNumLab, this will only happen in Canada and if ProcCur is a lab fee.
					&& !Procedures.IsAttachedToClaim(_procedure.ProcNumLab))//If attached to a claim, then user should recreate claim because estimates will be inaccurate not matter what.
				{
					Procedure procedureParent=Procedures.GetOneProc(_procedure.ProcNumLab,false);
					if(procedureParent!=null) {//A null parent proc could happen in rare cases for older databases.
						List<ClaimProc> listClaimProcsParent=ClaimProcs.RefreshForProc(procedureParent.ProcNum);
						Procedures.ComputeEstimates(procedureParent,_patient.PatNum,ref listClaimProcsParent,false,_listInsPlans,_listPatPlans,_listBenefits,
							null,null,true,
							_patient.Age,_listInsSubs,
							null,false,false,_listSubstitutionLinks,false,
							null,_lookupFees);
					}
				}
				return;
			}
			if(IsNew) {//if cancelling on a new procedure
				//delete any newly created claimprocs
				for(int i=0;i<_listClaimProcs.Count;i++) {
					//if(ClaimProcsForProc[i].ProcNum==ProcCur.ProcNum) {
					ClaimProcs.Delete(_listClaimProcs[i]);
					//}
				}
				//delete any newly created adjustments (typically from a discount plan
				for(int i = 0;i<_listAdjustments.Count;++i) {
					Adjustments.Delete(_listAdjustments[i]);
				}
			}
		}

		
	}
}
