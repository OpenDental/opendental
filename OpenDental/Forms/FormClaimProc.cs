using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Globalization;
using CodeBase;
using OpenDentBusiness.Crud;
using OpenDentBusiness.Eclaims;

namespace OpenDental {
	///<summary>Summary description for FormClaimProcEdit.</summary>
	public partial class FormClaimProc : FormODBase {
		private ClaimProc _claimProc;
		///<summary>If user hits cancel, then the claimproc is reset using this.  Do not modify the values in this claimproc anywhere in this form.</summary>
		private ClaimProc _claimProcOld;
		///<summary>Similar to ClaimProcOld, except this variable is set at the end of initialization, because the ClaimProcCur can change during
		///initialization, due to calling ComputeAmounts()</summary>
		public ClaimProc ClaimProcInitial;
		///<summary>The procedure to which this claimproc is attached.  Sent in if this is launched from the Procedure Edit window,
		///otherwise pulled from the db when form loads if ClaimProcCur.ProcNum>0, which also causes IsProc to be set to true.</summary>
		private Procedure _procedure;
		//Note: Consider removing IsProc. IsProc seems to be an unecessary bool since proc==null is equivalent to IsProc==false.
		///<summary>True if this is a procedure, and false if only a claim total.  Private variable, name should be updated.</summary>
		private bool IsProc;
		private Family _family;
		private Patient _patient;
		private List <InsPlan> _listInsPlans;
		///<summary>List of substitution links.  Lazy loaded, do not directly use this variable, use the property instead.</summary>
		private List<SubstitutionLink> _listSubstitutionLinks=null;
		private InsPlan _insPlan;
		private long _patPlanNum;
		private List<Benefit> _listBenefits;
		private List<ClaimProcHist> _listClaimProcHists;
		private List<ClaimProcHist> _listClaimProcHistsLoop;
		private List<PatPlan> _listPatPlans;
		///<summary>This value is obtained by a query when this window first opens.  It includes estimates if the other claims are not received and 
		///includes the payment amount if the other claims are received.  Will be 0 if this is a primary estimate.</summary>
		private double _paidOtherInsTotal;
		private double _paidOtherInsBaseEst;
		///<summary>This value is obtained by a query when this window first opens.  It includes both actual writeoffs and estimated writeoffs.  Will be 0 if this is a primary estimate.</summary>
		private double _writeOffOtherIns;
		private bool _doSaveToDb;
		private List<InsSub> _listInsSub;
		private List<Def> _listDefsPayTracks;
		private List<Provider> _listProviders;
		///<summary>Holds all data needed to calculate a blue book allowed amount.</summary>
		public BlueBookEstimateData BlueBookEstimateData_=null;
		///<summary>Set to true if this claimProc is accessed from within a claim or from within FormClaimPayTotal. This changes the behavior of the form, allowing more freedom with fields that are also totalled for entire claim.  This freedom is normally restricted so that claim totals will stay synchronized with individual claimprocs.  If true, it will still save changes to db, even though this is duplicated effort in FormClaimPayTotal.</summary>
		public bool IsInClaim;
		///<summary>Set this to true if user does not have permission to edit procedure.</summary>
		public bool NoPermissionProc;
		//public bool IsSaved;
		///<summary>Is only set when called from FormClaimEdit and to signify the recieving of a claim.</summary>
		public bool IsCalledFromClaimEdit=false;

		private List<SubstitutionLink> GetListSubLinks() {
			if(_listSubstitutionLinks==null) {
				_listSubstitutionLinks=SubstitutionLinks.GetAllForPlans(_listInsPlans);
			}
			return _listSubstitutionLinks;
		}

		///<summary>procCur can be null if not editing from within an actual procedure.  If the save is to happen within this window, then set saveToDb true.  If the object is to be altered here, but saved in a different window, then saveToDb=false.</summary>
		public FormClaimProc(ClaimProc claimProc,Procedure procedure,Family family,Patient patient,List<InsPlan> listInsPlans,List<ClaimProcHist> listClaimProcHists
			,ref List<ClaimProcHist> listClaimProcHistsLoop,List<PatPlan> listPatPlans,bool doSaveToDb,List<InsSub> listInsSubs)
		{
			_claimProc=claimProc;//always work directly with the original object.  Revert if we change our mind.
			_claimProcOld=_claimProc.Copy();
			_procedure=procedure;
			_family=family;
			_patient=patient;
			_listInsPlans=listInsPlans;
			_listInsSub=listInsSubs;
			_listClaimProcHists=listClaimProcHists;
			_listClaimProcHistsLoop=listClaimProcHistsLoop;
			_listPatPlans=listPatPlans;
			_doSaveToDb=doSaveToDb;
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			//can't use Lan.F because of complexity of label use
			Lan.C(this, new System.Windows.Forms.Control[]
				{
					this,
					this.label1,
					this.label9,
					this.label30,
					this.labelProcDate,
					this.label28,
					this.label29,
					this.groupClaim,
					this.radioEstimate,
					this.radioClaim,
					this.labelCodeSent,
					this.labelFeeBilled,
					this.labelClaimAdjReasonCodes,
					this.labelRemarks,
					this.labelNotInClaim,
					this.checkNoBillIns,
					this.labelFee,
					this.labelCopayAmt,
					this.label4,
					this.groupClaimInfo,
					this.labelDedApplied,
					this.labelPaidOtherIns,
					this.labelInsPayEst,
					this.labelInsPayAmt,
					this.labelWriteOff,
					this.labelDateEntry,
					this.checkPayPlan
					//this.butRecalc
			});
			Lan.C("All", new System.Windows.Forms.Control[] {
				butSave,
				butDelete,
			});
		}

		private void FormClaimProcEdit_Load(object sender, System.EventArgs e) {
			Initialize();
		}

		///<summary>Same as calling FormClaimProcEdit_Load().  Used in unit test 28.</summary>
		public void Initialize() {
			//Check to see if the Claim is a transfer, if TRUE disable all but Cancel and Delete buttons
			if(_claimProc.IsTransfer) {
				this.DisableAllExcept(new Control[]{butDelete});
			}
			if(_claimProc.IsOverpay) {
				if(Claims.GetClaim(_claimProc.ClaimNum)==null) {
					MsgBox.Show(this,"Claim has been deleted by another user.");
					DialogResult=DialogResult.Abort;
					return;
				}
				bool isOverpaid=(_claimProc.InsEstTotalOverride<0);
				List<ClaimProc> listClaimProcs=ClaimProcs.RefreshForClaims(new List<long> { _claimProc.ClaimNum }).ToList();
				FormClaimOverpay formClaimOverpay=new FormClaimOverpay(_claimProc.ClaimNum,listClaimProcs,_patient.PatNum,isOverpaid,_claimProc.ClaimProcNum);
				formClaimOverpay.ShowDialog();
				DialogResult=DialogResult.OK;
				return;
			}
			if(_claimProc.ClaimNum>0) {
				Claim claim=Claims.GetClaim(_claimProc.ClaimNum);
				if(claim==null) {
					MsgBox.Show(this,"Claim has been deleted by another user.");
					DialogResult=DialogResult.Abort;
					return;
				}
				if(_claimProc.ClaimNum>0 && (_claimProc.Status==ClaimProcStatus.Received || _claimProc.Status==ClaimProcStatus.Supplemental)
					//Prior to version 16.3.7 this perm check used claim.DateReceived but users with ClaimSentEdit perm but not ClaimProcReceivedEdit perm could
					//edit the claim Date Received field and subvert the security perm intended to prevent them from editing the claimproc
					&& !Security.IsAuthorized(EnumPermType.ClaimProcReceivedEdit,_claimProc.DateEntry,false))
				{
					//Don't allow user to change anything.
					//We could have used .ReadOnly for textboxes but some of them have events on them and I dont want them to fire for no reason.
					//The downside is that user can't highlight to copy paste.  If there is complaints we could always set .ReadOnly=true and make the event
					//	code return if the field is readonly instead.
					comboStatus.Enabled=false;
					comboPayTracker.Enabled=false;
					comboProvider.Enabled=false;
					butPickProv.Enabled=false;
					textDateCP.Enabled=false;
					textProcDate.Enabled=false;
					radioEstimate.Enabled=false;
					radioClaim.Enabled=false;
					textCodeSent.Enabled=false;
					textFeeBilled.Enabled=false;
					textRemarks.Enabled=false;
					checkNoBillIns.Enabled=false;
					butUpdateAllowed.Enabled=false;
					textAllowedOverride.Enabled=false;
					textCopayOverride.Enabled=false;
					textDedEstOverride.Enabled=false;
					textPercentOverride.Enabled=false;
					textPaidOtherInsOverride.Enabled=false;
					textInsEstTotalOverride.Enabled=false;
					textWriteOffEstOverride.Enabled=false;
					textDedApplied.Enabled=false;
					textInsPayEst.Enabled=false;
					textInsPayAmt.Enabled=false;
					textWriteOff.Enabled=false;
					checkPayPlan.Enabled=false;
					butSave.Enabled=false;
					butDelete.Enabled=false;
				}
				else if((claim.ClaimStatus=="S" || claim.ClaimStatus=="R")) {//sent or received 
					if(!Security.IsAuthorized(EnumPermType.ClaimSentEdit,claim.DateSent,true)) { //attached to claim, no permission for claims.
						butSave.Enabled=false;
						butDelete.Enabled=false; 
					}
				}
			}
			if((butSave.Enabled || butDelete.Enabled) && NoPermissionProc) {//blocks users with no permission to edit procedure
				butSave.Enabled=false;
				butDelete.Enabled=false;
			}
			InsSub insSub=InsSubs.GetSub(_claimProc.InsSubNum,_listInsSub);
			_insPlan=InsPlans.GetPlan(insSub.PlanNum,_listInsPlans);
			_patPlanNum=PatPlans.GetPatPlanNum(insSub.InsSubNum,_listPatPlans);
			_listBenefits=null;//only fill it if proc
			_paidOtherInsTotal=ClaimProcs.GetPaidOtherInsTotal(_claimProc,_listPatPlans);
			_paidOtherInsBaseEst=ClaimProcs.GetPaidOtherInsBaseEst(_claimProc,_listPatPlans);
			_writeOffOtherIns=ClaimProcs.GetWriteOffOtherIns(_claimProc,_listPatPlans);
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(_family);
			textInsPlan.Text=InsPlans.GetDescript(_claimProc.PlanNum,_family,_listInsPlans,_claimProc.InsSubNum,listInsSubs);
			checkNoBillIns.Checked=_claimProc.NoBillIns;
			if(_claimProc.ClaimPaymentNum>0) {//attached to ins check
				textDateCP.ReadOnly=true;//DateCP always the same as the payment date and can't be changed here
				if(!Security.IsAuthorized(EnumPermType.InsPayEdit,_claimProc.DateCP)) {
					butSave.Enabled=false;
					if(_claimProc.Status==ClaimProcStatus.Received) {
						comboStatus.Enabled=false;
					}
				}
				textInsPayAmt.ReadOnly=true;
				labelAttachedToCheck.Visible=true;
				butDelete.Enabled=false;
			}
			//This new expanded security prevents editing completed claimprocs, even if not attached to an ins check.
			//For example, a zero payment with a writeoff amount.  Must prevent changing that date.
			else if((_claimProc.Status.In(ClaimProcStatus.CapComplete,ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.InsHist))
				&& (IsProc || !_claimProc.IsNew)
				&& !Security.IsAuthorized(EnumPermType.InsPayEdit,_claimProc.DateCP))//
			{
				textDateCP.ReadOnly=true;
				butSave.Enabled=false;
				textInsPayAmt.ReadOnly=true;
				labelAttachedToCheck.Visible=false;
				//listStatus.Enabled=false;//this is handled in the mousedown event
				butDelete.Enabled=false;
				comboStatus.Enabled=false;
			}
			else {
				labelAttachedToCheck.Visible=false;
			}
			if(_claimProc.ProcNum==0) {//total payment for a claim
				IsProc=false;
				textDescription.Text="Total Payment";
				textProcDate.ReadOnly=false;
			}
			else {
				IsProc=true;
				_listBenefits=Benefits.GetForPlanOrPatPlan(_claimProc.PlanNum,_patPlanNum);
				if(_procedure==null) {
					_procedure=Procedures.GetOneProc(_claimProc.ProcNum,false);
				}
				textDescription.Text=ProcedureCodes.GetProcCode(_procedure.CodeNum).Descript;
				textProcDate.ReadOnly=true;//user not allowed to edit ProcDate unless it's for a total payment
			}
			if(BlueBookEstimateData_==null) {
				List<Procedure> listProcedures=new List<Procedure>();
				if(_procedure!=null) {
					listProcedures.Add(_procedure);
				}
				BlueBookEstimateData_=new BlueBookEstimateData(_listInsPlans,_listInsSub,_listPatPlans,listProcedures,GetListSubLinks());
			}
			//get the date to use for checking whether the user has InsWriteOffEdit permission
			DateTime dateWriteoffSec=_claimProc.SecDateEntry;//if this is a total payment, there is no proc so use ClaimProcCur.SecDateEntry
			//if this is claimproc is attached to a proc, and the proc returned by GetOneProc (called above if proc was null) is a valid proc, use DateEntryC
			if(IsProc && _procedure.ProcDate!=DateTime.MinValue) {
				dateWriteoffSec=_procedure.DateEntryC;
			}
			if(CompareDouble.IsZero(_claimProc.InsPayAmt)) { 
				if(!Security.IsAuthorized(EnumPermType.InsPayCreate,true)) { //user not allowed to create an insurance payment
					textInsPayAmt.ReadOnly=true;
				}
			}
			else {
				if(!Security.IsAuthorized(EnumPermType.InsPayEdit,_claimProc.DateCP,true)) { //user not allowed to edit an insurance payment
					textInsPayAmt.ReadOnly=true;
				}
			}
			if(!Security.IsAuthorized(EnumPermType.InsWriteOffEdit,dateWriteoffSec,true)) {//user not allowed to edit/create a writeoff
				textWriteOff.ReadOnly=true;
				textWriteOffEstOverride.ReadOnly=true;
				//cannot edit the writeoff, so block deleting the claimproc, otherwise they could delete and recreate to bypass the date/days restriction
				butDelete.Enabled=false;
			}
			if(_claimProc.ClaimNum>0) {//attached to claim
				radioClaim.Checked=true;
				checkNoBillIns.Enabled=false;
				if(IsInClaim) {//(not from the procedure window)
					labelNotInClaim.Visible=false;
				}
				else {//must be accessing it from the Procedure window
					textCodeSent.ReadOnly=true;
					textFeeBilled.ReadOnly=true;
					labelNotInClaim.Visible=true;
					textDedApplied.ReadOnly=true;
					textInsPayEst.ReadOnly=true;
					textInsPayAmt.ReadOnly=true;
					textWriteOff.ReadOnly=true;
				}
				groupClaimInfo.Visible=true;
				if(_claimProc.ProcNum==0) {//if a total entry rather than by proc
					panelEstimateInfo.Visible=false;
					//labelPatTotal.Visible=false;
					labelInsPayAmt.Font=new Font(labelInsPayAmt.Font,FontStyle.Bold);
					labelProcDate.Visible=false;
					textProcDate.Visible=false;
					labelCodeSent.Visible=false;
					textCodeSent.Visible=false;
					labelFeeBilled.Visible=false;
					textFeeBilled.Visible=false;
					ActiveControl=textInsPayAmt;
				}
				else if(_claimProc.Status==ClaimProcStatus.Received) {
					labelInsPayAmt.Font=new Font(labelInsPayAmt.Font,FontStyle.Bold);
				}
				if(_claimProc.Status.In(ClaimProcStatus.Received,ClaimProcStatus.NotReceived,ClaimProcStatus.CapClaim) 
					&& !Security.IsAuthorized(EnumPermType.ClaimProcClaimAttachedProvEdit,true))
				{
					comboProvider.Enabled=false;
					butPickProv.Enabled=false;
				}
				//butOK.Enabled=false;
				//butDelete.Enabled=false;
				//MessageBox.Show(panelEstimateInfo.Visible.ToString());
			}
			else if(_claimProc.PlanNum>0 && _claimProc.Status.In(ClaimProcStatus.CapEstimate,ClaimProcStatus.CapComplete)) { //not attached to a claim
				//InsPlans.Cur.PlanType=="c"){//capitation proc,whether Estimate or CapComplete,never billed to ins
				for(int i=0;i<panelEstimateInfo.Controls.Count;i++) {
					panelEstimateInfo.Controls[i].Visible=false;
				}
				for(int i=0;i<groupClaimInfo.Controls.Count;i++) {
					groupClaimInfo.Controls[i].Visible=false;
				}
				groupClaimInfo.Text="";
				labelFee.Visible=true;
				textFee.Visible=true;
				labelCopayAmt.Visible=true;
				textCopayAmt.Visible=true;
				textCopayOverride.Visible=true;
				if(_claimProc.Status==ClaimProcStatus.CapEstimate) {
					labelWriteOffEst.Visible=true;
					textWriteOffEst.Visible=true;
				}
				else {//capcomplete
					labelWriteOff.Visible=true;
					textWriteOff.Visible=true;
				}
				//labelPatTotal.Visible=true;
				groupClaim.Visible=false;
				labelNotInClaim.Visible=false;
			}
			else if(_claimProc.PlanNum>0 && _claimProc.Status==ClaimProcStatus.InsHist) {
				groupClaimInfo.Visible=false;
				groupAllowed.Visible=false;
				groupClaim.Visible=false;
				//InsPlans.Cur.PlanType=="c"){//capitation proc,whether Estimate or CapComplete,never billed to ins
				for(int i=0;i<panelEstimateInfo.Controls.Count;i++) {
					panelEstimateInfo.Controls[i].Visible=false;
				}
				labelFee.Visible=true;
				textFee.Visible=true;
				checkPayPlan.Visible=false;
				checkNoBillIns.Visible=false;
				label6.Visible=false;
				label28.Visible=false;
				textDateCP.Visible=false;
				label12.Visible=false;
				comboPayTracker.Visible=false;
			}
			else {//estimate
				groupClaimInfo.Visible=false;
				radioEstimate.Checked=true;
				labelNotInClaim.Visible=false;
				panelClaimExtras.Visible=false;
			}
			//The order of the items in comboStatus matter inside the comboStatus_SelectionChangeCommitted(), because we use hard coded indices.
			comboStatus.Items.Clear();
			comboStatus.Items.Add(Lan.g(this,"Estimate"));
			comboStatus.Items.Add(Lan.g(this,"Not Received"));
			comboStatus.Items.Add(Lan.g(this,"Received"));
			comboStatus.Items.Add(Lan.g(this,"PreAuthorization"));
			comboStatus.Items.Add(Lan.g(this,"Supplemental"));
			comboStatus.Items.Add(Lan.g(this,"CapClaim"));
			comboStatus.Items.Add(Lan.g(this,"CapEstimate"));
			comboStatus.Items.Add(Lan.g(this,"CapComplete"));
			comboStatus.Items.Add(Lan.g(this,"InsHist"));
			SetComboStatus(_claimProc.Status);
			if(_claimProc.Status==ClaimProcStatus.Received || _claimProc.Status==ClaimProcStatus.Supplemental) {
				labelDateEntry.Visible=true;
				textDateEntry.Visible=true;
			}
			else {
				labelDateEntry.Visible=false;
				textDateEntry.Visible=false;
			}
			comboProvider.Items.Clear();
			_listProviders=Providers.GetProvsForClinic(_claimProc.ClinicNum);
			for(int i=0;i<_listProviders.Count;i++) {
				comboProvider.Items.Add(_listProviders[i].Abbr,_listProviders[i]);
				if(_claimProc.ProvNum==_listProviders[i].ProvNum) {
					comboProvider.SelectedIndex=i;
				}
			}
			//this is not used, because the provider might simply be hidden. See bottom of page.
			//if(listProv.SelectedIndex==-1){
			//	listProv.SelectedIndex=0;//there should always be a provider
			//}
			if(!PrefC.HasClinicsEnabled) {
				labelClinic.Visible=false;
				textClinic.Visible=false;
			}
			else {
				textClinic.Text=Clinics.GetAbbr(_claimProc.ClinicNum);
			}
			textDateEntry.Text=_claimProc.DateEntry.ToShortDateString();
			if(_claimProc.ProcDate.Year<1880) {
				textProcDate.Text="";
			}
			else {
				textProcDate.Text=_claimProc.ProcDate.ToShortDateString();
			}
			if(_claimProc.DateCP.Year<1880) {
				textDateCP.Text="";
			}
			else {
				textDateCP.Text=_claimProc.DateCP.ToShortDateString();
			}
			textCodeSent.Text=_claimProc.CodeSent;
			textFeeBilled.Text=_claimProc.FeeBilled.ToString("n");
			textClaimAdjReasonCodes.Text=_claimProc.ClaimAdjReasonCodes;
			textRemarks.Text=_claimProc.Remarks;
			if(_claimProc.PayPlanNum==0) {
				checkPayPlan.Checked=false;
			}
			else {
				checkPayPlan.Checked=true;
			}
			_listDefsPayTracks=Defs.GetDefsForCategory(DefCat.ClaimPaymentTracking,true);
			comboPayTracker.Items.Add("None");
			for(int i=0;i<_listDefsPayTracks.Count;i++) {
				comboPayTracker.Items.Add(_listDefsPayTracks[i].ItemName);
				if(_listDefsPayTracks[i].DefNum==_claimProc.ClaimPaymentTracking) {
					comboPayTracker.SelectedIndex=i+1;
				}
			}
			if(comboPayTracker.SelectedIndex==-1) {
				comboPayTracker.SelectedIndex=0;
			}
			//Not allowed to change status if attached to a claim payment.
			if(_claimProcOld.ClaimPaymentNum > 0) {
				comboStatus.Enabled=false;
				if(_insPlan.PlanType!="c") { 
					if(_claimProcOld.Status==ClaimProcStatus.CapComplete
						|| _claimProcOld.Status==ClaimProcStatus.CapClaim
						|| _claimProcOld.Status==ClaimProcStatus.CapEstimate) 
					{
						//One of our customers somehow had CapComplete procedures attached to insurance payments for insurnace plans that are not capitation.
						comboStatus.Enabled=true;
					}
				}
			}
			//Not allowed to change status if cap estimate or cap complete and the plan is a capitation plan.
			if(_insPlan.PlanType=="c" && (_claimProcOld.Status==ClaimProcStatus.CapComplete || _claimProcOld.Status==ClaimProcStatus.CapEstimate)) {
				comboStatus.Enabled=false;
			}
			//Not allowed to change status if estimate or inshist and is not a capitation plan.
			if(_insPlan.PlanType!="c" && _claimProcOld.Status.In(ClaimProcStatus.Estimate,ClaimProcStatus.InsHist)) {
				comboStatus.Enabled=false;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				if(IsProc && _procedure.ProcNumLab!=0) {//We're loading this form with a lab procedure selected.
					comboStatus.Enabled=false;//The status of a lab should not change independent of its parent proc.
					if(Canadian.IsValidForLabEstimates(_insPlan)) {
						//labcase. Takes the parent proc's percent override. Disable so uses cannot make changes. 
						textPercentOverride.ReadOnly=true;
						textCopayOverride.ReadOnly=true;
						textDedEstOverride.ReadOnly=true;
						textPaidOtherInsOverride.ReadOnly=true;
					}
				}
			}
			FillInitialAmounts();
			ComputeAmounts();
			ClaimProcInitial=_claimProc.Copy();
			//MessageBox.Show(panelEstimateInfo.Visible.ToString());
		}

		///<summary>Do not use in release mode.  Used for unit test 28 to get UI values for text boxes in this form.  Returns null if textBoxName does not exist.</summary>
		public string GetTextValue(string textBoxName) {
			return GetTextValue(this,textBoxName);
		}

		///<summary>Recursive.  Do not use in release mode.  Used for unit test 28 to get UI values for text boxes in this form.  Returns null if textBoxName does not exist.</summary>
		private string GetTextValue(Control control,string textBoxName) {
			if(control.GetType()==typeof(TextBox)) {
				TextBox textBox=(TextBox)control;
				if(textBox.Name==textBoxName) {
					return textBox.Text;
				}
			}
			else if(control.GetType()==typeof(ValidDouble)) {
				ValidDouble validDouble=(ValidDouble)control;
				if(validDouble.Name==textBoxName) {
					return validDouble.Text;
				}
			}
			else if(control.GetType()==typeof(ValidDate)) {
				ValidDate validDate=(ValidDate)control;
				if(validDate.Name==textBoxName) {
					return validDate.Text;
				}
			}
			for(int i=0;i<control.Controls.Count;i++) {
				string result=GetTextValue(control.Controls[i],textBoxName);
				if(result!=null) {
					return result;
				}
			}
			return null;
		}

		private void SetComboStatus(ClaimProcStatus claimProcStatus){
			switch(claimProcStatus){
				case ClaimProcStatus.Estimate:
					comboStatus.SelectedIndex=0;
					break;
				case ClaimProcStatus.NotReceived:
					comboStatus.SelectedIndex=1;
					break;
				case ClaimProcStatus.Received:
					comboStatus.SelectedIndex=2;
					break;
				case ClaimProcStatus.Preauth:
					comboStatus.SelectedIndex=3;
					break;
				//adjustments have a completely different user interface. Cannot access from here.
				case ClaimProcStatus.Supplemental:
					comboStatus.SelectedIndex=4;
					break;
				case ClaimProcStatus.CapClaim:
					comboStatus.SelectedIndex=5;
					break;
				case ClaimProcStatus.CapEstimate:
					comboStatus.SelectedIndex=6;
					break;
				case ClaimProcStatus.CapComplete:
					comboStatus.SelectedIndex=7;
					break;
				case ClaimProcStatus.InsHist:
					comboStatus.SelectedIndex=8;
					break;
			}
		}

		///<summary>All text boxes will be blank before this is run.  It is only run once.</summary>
		private void FillInitialAmounts(){
			if(IsProc){
				textFee.Text=_procedure.ProcFeeTotal.ToString("f");
				InsPlan insPlan=InsPlans.GetPlan(_claimProc.PlanNum,_listInsPlans);
				long insFeeSchedNum=FeeScheds.GetFeeSched(_patient,_listInsPlans,_listPatPlans,_listInsSub,_procedure.ProvNum);
				textFeeSched.Text=FeeScheds.GetDescription(insFeeSchedNum);//show ins fee sched, unless PPO plan and standard fee is greater, checked below
				if(insPlan.PlanType=="p") {//if ppo
					double insFee=Fees.GetAmount0(_procedure.CodeNum,insFeeSchedNum,_procedure.ClinicNum,_procedure.ProvNum);
					long standFeeSchedNum=Providers.GetProv(Patients.GetProvNum(_patient)).FeeSched;
					if(_procedure.ProcFee!=insFee) {
						textFeeSched.Text=FeeScheds.GetDescription(standFeeSchedNum);
					}
				}
				string stringProcCode=ProcedureCodes.GetStringProcCode(_procedure.CodeNum);
				//int codeNum=proc.CodeNum;
				long substCodeNum=_procedure.CodeNum;
				if(SubstitutionLinks.HasSubstCodeForPlan(insPlan,_procedure.CodeNum,GetListSubLinks())) {
					substCodeNum=ProcedureCodes.GetSubstituteCodeNum(stringProcCode,_procedure.ToothNum,insPlan.PlanNum,GetListSubLinks());//for posterior composites
				}
				if(_procedure.CodeNum!=substCodeNum) {
					textSubstCode.Text=ProcedureCodes.GetStringProcCode(substCodeNum);
				}
				if(insPlan.PlanType=="p"){//if ppo
					textPPOFeeSched.Text=FeeScheds.GetDescription(insPlan.FeeSched);
					textAllowedFeeSched.Text="---";
				}
				else{
					textPPOFeeSched.Text="---";
					if(insPlan.AllowedFeeSched!=0 && !BlueBookEstimateData_.IsValidForEstimate(_claimProc,false)) {
						textAllowedFeeSched.Text=FeeScheds.GetDescription(insPlan.AllowedFeeSched);
					}
					else{
						textAllowedFeeSched.Text="---";
					}
				}
			}
			else{//not a proc
				textFee.Text="";//because this textbox starts with a value just as a placeholder
				labelFeeSched.Visible=false;
				textFeeSched.Visible=false;
				groupAllowed.Visible=false;
			}
			FillAllowed(BlueBookEstimateData_);
			if(_claimProc.AllowedOverride!=-1){
				textAllowedOverride.Text=_claimProc.AllowedOverride.ToString("f");
			}
			if(_claimProc.CopayAmt!=-1){
				textCopayAmt.Text=_claimProc.CopayAmt.ToString("f");
			}
			if(_claimProc.CopayOverride!=-1){
				textCopayOverride.Text=_claimProc.CopayOverride.ToString("f");
			}
			if(_claimProc.DedEst > 0) {
				textDedEst.Text=_claimProc.DedEst.ToString("f");
			}
			if(_claimProc.DedEstOverride!=-1) {
				textDedEstOverride.Text=_claimProc.DedEstOverride.ToString("f");
			}
			if(_claimProc.Percentage!=-1){
				textPercentage.Text=_claimProc.Percentage.ToString();
			}
			if(_claimProc.PercentOverride!=-1){
				textPercentOverride.Text=_claimProc.PercentOverride.ToString();
			}
			if(_claimProc.PaidOtherIns!=-1){
				textPaidOtherIns.Text=_claimProc.PaidOtherIns.ToString("f");
			}
			if(_claimProc.PaidOtherInsOverride!=-1) {
				textPaidOtherInsOverride.Text=_claimProc.PaidOtherInsOverride.ToString("f");
			}
			textBaseEst.Text=_claimProc.BaseEst.ToString("f");
			if(_claimProc.InsEstTotal!=-1) {
				textInsEstTotal.Text=_claimProc.InsEstTotal.ToString("f");
			}
			if(_claimProc.InsEstTotalOverride!=-1) {
				textInsEstTotalOverride.Text=_claimProc.InsEstTotalOverride.ToString("f");
			}
			if(_claimProc.WriteOffEst!=-1) {
				textWriteOffEst.Text=_claimProc.WriteOffEst.ToString("f");
			}
			if(_claimProc.WriteOffEstOverride!=-1) {
				textWriteOffEstOverride.Text=_claimProc.WriteOffEstOverride.ToString("f");
			}
			textDedApplied.Text=_claimProc.DedApplied.ToString("f");
			textInsPayEst.Text=_claimProc.InsPayEst.ToString("f");
			textInsPayAmt.Text=_claimProc.InsPayAmt.ToString("f");
			textWriteOff.Text=_claimProc.WriteOff.ToString("f");
		}

		///<summary>Fills the carrier allowed amount.  Called from FillInitialAmounts and from butUpdateAllowed_Click</summary>
		private void FillAllowed(BlueBookEstimateData blueBookEstimateData=null){
			if(IsProc){
				Appointment appointment=null;
				if(_procedure.AptNum!=0) {
					appointment=Appointments.GetOneApt(_procedure.AptNum);
				}
				else if(_procedure.PlannedAptNum!=0) {
					appointment=Appointments.GetOneApt(_procedure.PlannedAptNum); //If a scheduled appointment doesn't exist, grab the planned one.
				}
				decimal allowed=InsPlans.GetAllowedForProc(_procedure,_claimProc,_listInsPlans,GetListSubLinks(),null,blueBookEstimateData,appointment);
				if(allowed==-1){
					textCarrierAllowed.Text="";
				}
				else {
					textCarrierAllowed.Text=allowed.ToString("f");
				}
			}
			else{
				textCarrierAllowed.Text="";
			}
		}

		private void butUpdateAllowed_Click(object sender, System.EventArgs e) {
			InsPlan insPlan=InsPlans.GetPlan(_claimProc.PlanNum,_listInsPlans);
			if(insPlan==null){
				//this should never happen
			}
			if(BlueBookEstimateData_.IsValidForEstimate(_claimProc,false)) {
				MsgBox.Show(this,"Allowed fee schedules are not used for Out of Network primary dental insurance plans when the Blue Book feature is on.");
				return;
			}
			if(insPlan.AllowedFeeSched==0 && insPlan.PlanType!="p"){
				MsgBox.Show(this,"Plan must either be a PPO type or it must have an 'Allowed' fee schedule set.");
				return;
			}
			long feeSchedNum=-1;
			if(insPlan.AllowedFeeSched!=0) {
				feeSchedNum=insPlan.AllowedFeeSched;
			}
			else if(insPlan.PlanType=="p") {
				//The only other way to manually edit allowed fee schedule amounts is blocked via the Setup permission.
				//We only want to block PPO patients so that we don't partially break Blue Book users.
				if(!Security.IsAuthorized(EnumPermType.Setup)) {
					return;
				}
				feeSchedNum=insPlan.FeeSched;
			}
			if(FeeScheds.GetIsHidden(feeSchedNum)){
				MsgBox.Show(this,"Allowed fee schedule is hidden, so no changes can be made.");
				return;
			}
			Fee fee=Fees.GetFee(_procedure.CodeNum,feeSchedNum,_procedure.ClinicNum,_procedure.ProvNum);
			using FormFeeEdit formFeeEdit=new FormFeeEdit();
			if(fee==null) {
				FeeSched feeSched=FeeScheds.GetFirst(x => x.FeeSchedNum==feeSchedNum);
				fee=new Fee();
				fee.FeeSched=feeSchedNum;
				fee.CodeNum=_procedure.CodeNum;
				fee.ClinicNum=(feeSched.IsGlobal) ? 0 : _procedure.ClinicNum;
				fee.ProvNum=(feeSched.IsGlobal) ? 0 : _procedure.ProvNum;
				Fees.Insert(fee);
				//SecurityLog is updated in FormFeeEdit.
				formFeeEdit.IsNew=true;
			}
			DateTime datePrevious=fee.SecDateTEdit;
			//Make an audit entry that the user manually launched the Fee Edit window from this location.
			SecurityLogs.MakeLogEntry(EnumPermType.ProcFeeEdit,0,Lan.g(this,"Procedure")+": "+ProcedureCodes.GetStringProcCode(fee.CodeNum)
				+", "+Lan.g(this,"Fee")+": "+fee.Amount.ToString("c")+", "+Lan.g(this,"Fee Schedule")+": "+FeeScheds.GetDescription(fee.FeeSched)
				+". "+Lan.g(this,"Manually launched Edit Fee window via Edit Claim Procedure window."),fee.CodeNum,DateTime.MinValue);
			SecurityLogs.MakeLogEntry(EnumPermType.LogFeeEdit,0,Lan.g(this,"Fee Inserted"),fee.FeeNum,datePrevious);
			formFeeEdit.FeeCur=fee;
			formFeeEdit.ShowDialog();
			//The Fees cache is updated in the closing of FormFeeEdit if there were any changes made.  Simply refresh our window.
			if(formFeeEdit.DialogResult==DialogResult.OK) {
				FillAllowed();
				ComputeAmounts();//?
			}
		}

		private void ComputeAmounts(){
			if(!AllAreValid()){
				return;
			}
			_claimProc.NoBillIns=checkNoBillIns.Checked;
			if(_claimProc.Status==ClaimProcStatus.CapEstimate || _claimProc.Status==ClaimProcStatus.CapComplete) {
				panelEstimateInfo.Visible=true;
				groupClaimInfo.Visible=true;
			}
			else if(checkNoBillIns.Checked) {
				panelEstimateInfo.Visible=false;
				groupClaimInfo.Visible=false;
				return;
			}
			else{
				if(_claimProc.ProcNum!=0){//if a total payment, then this protects panel from inadvertently
						//being set visible again.  All other situations, it's based on NoBillIns
					panelEstimateInfo.Visible=true;
				}
				if(_claimProc.ClaimNum>0) {//attached to claim
					groupClaimInfo.Visible=true;
				}
				else {
					groupClaimInfo.Visible=false;
				}
			}
			if(textAllowedOverride.Text=="") {
				_claimProc.AllowedOverride=-1;
			}
			else {
				_claimProc.AllowedOverride=PIn.Double(textAllowedOverride.Text);
			}
			if(textCopayOverride.Text=="") {
				_claimProc.CopayOverride=-1;
			}
			else {
				_claimProc.CopayOverride=PIn.Double(textCopayOverride.Text);
			}
			if(textDedEstOverride.Text=="") {
				_claimProc.DedEstOverride=-1;
			}
			else {
				_claimProc.DedEstOverride=PIn.Double(textDedEstOverride.Text);
			}
			if(textPercentOverride.Text=="") {
				_claimProc.PercentOverride=-1;
			}
			else {
				_claimProc.PercentOverride=PIn.Int(textPercentOverride.Text);
			}
			if(textPaidOtherInsOverride.Text=="") {
				_claimProc.PaidOtherInsOverride=-1;
			}
			else {
				_claimProc.PaidOtherInsOverride=PIn.Double(textPaidOtherInsOverride.Text);
			}
			if(textInsEstTotalOverride.Text=="") {
				_claimProc.InsEstTotalOverride=-1;
			}
			else {
				_claimProc.InsEstTotalOverride=PIn.Double(textInsEstTotalOverride.Text);
			}
			if(textWriteOffEstOverride.Text=="") {
				_claimProc.WriteOffEstOverride=-1;
			}
			else {
				_claimProc.WriteOffEstOverride=PIn.Double(textWriteOffEstOverride.Text);
			}
			if(IsProc && _procedure.ProcNumLab == 0) {
				//doCheckCanadianLabs is false because we are simply making in memory changs to ClaimProcCur.
				//If ClaimProcCur.Status was changed then the inner CanadianLabBaseEstHelper(...) call would insert new lab claimProc rows.
				//We currently do not use the lab claimProcs in any way in this window so we only need to worry about update the statuses
				//on an OK click when commiting ClaimProcCur changes to DB.
				ClaimProcs.ComputeBaseEst(_claimProc,_procedure,_insPlan,_patPlanNum,_listBenefits,
					_listClaimProcHists,_listClaimProcHistsLoop,_listPatPlans,_paidOtherInsTotal,_paidOtherInsBaseEst,_patient.Age,_writeOffOtherIns,_listInsPlans,
					_listInsSub,GetListSubLinks(),false,null,doCheckCanadianLabs:false,blueBookEstimateData:BlueBookEstimateData_);
				//Paid other ins is not accurate
			}
			//else {
			//	ClaimProcs.ComputeBaseEst(ClaimProcCur,0,"",0,Plan,PatPlanNum,BenefitList,HistList,LoopList);
			//}
			if(_claimProc.CopayAmt == -1) {
				textCopayAmt.Text="";
			}
			else {
				textCopayAmt.Text=_claimProc.CopayAmt.ToString("f");
			}
			if(_claimProc.DedEst == -1) {
				textDedEst.Text="";
			}
			else {
				textDedEst.Text=_claimProc.DedEst.ToString("f");
			}
			if(_claimProc.Percentage == -1) {
				textPercentage.Text="";
			}
			else {
				textPercentage.Text=_claimProc.Percentage.ToString("f0");
			}
			if(_claimProc.PaidOtherIns == -1) {
				textPaidOtherIns.Text="";
			}
			else {
				textPaidOtherIns.Text=_claimProc.PaidOtherIns.ToString("f");
			}
			textBaseEst.Text=_claimProc.BaseEst.ToString("f");
			textInsEstTotal.Text=_claimProc.InsEstTotal.ToString("f");
			if(_claimProc.WriteOffEst==-1) {
				textWriteOffEst.Text="";
			}
			else {
				textWriteOffEst.Text=_claimProc.WriteOffEst.ToString("f");
			}
			if(IsProc) {
				//Compute the patient portion ignorant of other claimprocs and adjustments to preserve old behavior.
				textPatPortion1.Text=ClaimProcs.GetPatPortion(_procedure,new List<ClaimProc>() { _claimProc }).ToString("f");
			}
			textEstimateNote.Text=_claimProc.EstimateNote;
			//insurance box---------------------------------------------------------------
			if(groupClaimInfo.Visible){
				_claimProc.DedApplied=PIn.Double(textDedApplied.Text);
				_claimProc.InsPayEst=PIn.Double(textInsPayEst.Text);
				_claimProc.InsPayAmt=PIn.Double(textInsPayAmt.Text);
				_claimProc.WriteOff=PIn.Double(textWriteOff.Text);
				if(IsProc) {
					//Compute the patient portion ignorant of other claimprocs and adjustments to preserve old behavior.
					textPatPortion2.Text=ClaimProcs.GetPatPortion(_procedure,new List<ClaimProc>() { _claimProc }).ToString("f");
					labelPatPortion1.Visible=false;
					textPatPortion1.Visible=false;
				}
			}
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick=new FormProviderPick(_listProviders);
			if(comboProvider.SelectedIndex > -1) {
				formProviderPick.ProvNumSelected=_listProviders[comboProvider.SelectedIndex].ProvNum;
			}
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			//Set the combo box to the ODBoxItem that contains the provider that was just selected.
			//If we can't find it, reselect the same item that was already selected.
			comboProvider.SetSelectedKey<Provider>(formProviderPick.ProvNumSelected, x => x.ProvNum);
		}

		private void comboStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			//new selected index will already be set
			if(!_claimProcOld.Status.In(ClaimProcStatus.Estimate,ClaimProcStatus.InsHist)//not an estimate or inshist
				&& comboStatus.SelectedIndex.In(0,8))//and clicked on estimate or insHist
			{
				SetComboStatus(_claimProcOld.Status);//no change
				return;
			}
			if(_claimProcOld.Status==ClaimProcStatus.Supplemental) {
				//Get all non-supplemental claimprocs. If there is at least one, do not let them change this status if this a supplemental claim.
				//Prevents creating two received claimprocs for the same procedure and insurance. If there is not one, allow them to change this status
				//so they don't get stuck with a supplemental claimproc and no way to create one of a different type.
				List<ClaimProc> listClaimProcsForClaims=ClaimProcs.RefreshForClaim(_claimProcOld.ClaimNum)
					.Where(x => x.ClaimProcNum!=_claimProcOld.ClaimProcNum 
					&& x.Status!=ClaimProcStatus.Supplemental 
					&& x.ProcNum==_claimProcOld.ProcNum).ToList();
				if(!listClaimProcsForClaims.IsNullOrEmpty()) {
					MsgBox.Show(this,"Cannot change the status of a supplemental claim procedure when there is at least one claim procedure of a different"
						+" status. There should be a maximum of one claim procedure of status received for each procedure in the claim.");
					SetComboStatus(ClaimProcStatus.Supplemental);
					return;
				}
			}
			#region Capitation Claim Attached
			if(_insPlan.PlanType=="c" && _claimProcOld.ClaimNum > 0 && comboStatus.SelectedIndex!=5) {
				MsgBox.Show(this,"A capitation insurance plan is associated with this claim procedure.\r\n"
					+"This claim procedure is currently part of a claim.\r\n"
					+"CapClaim is the only valid status for this scenario.");
				_claimProc.Status=ClaimProcStatus.CapClaim;
				SetComboStatus(_claimProc.Status);//Force CapClaim status.
				return;
			}
			#endregion
			#region Claim Payment Attached
			if(_claimProcOld.ClaimPaymentNum > 0//Attached to a payment
				&& _insPlan.PlanType!="c"//Is a category percentage plan, or PPO percentage plan, or a flat co-pay plan.
				&& comboStatus.SelectedIndex!=2//User did not select Received
				&& comboStatus.SelectedIndex!=4)//User did not select Supplemental
			{
				if(_insPlan.PlanType==""){
					MsgBox.Show(this,"This claim procedure is attached to an insurance payment.\r\n"
						+"Since the insurance plan is a category percentage plan,\r\n"
						+"you may only set the status to Received or Supplemental.");
				}
				else if(_insPlan.PlanType=="p"){
					MsgBox.Show(this,"This claim procedure is attached to an insurance payment.\r\n"
						+"Since the insurance plan is a PPO percentage plan,\r\n"
						+"you may only set the status to Received or Supplemental.");
				}
				else if(_insPlan.PlanType=="f"){
					MsgBox.Show(this,"This claim procedure is attached to an insurance payment.\r\n"
						+"Since the insurance plan is a flat co-pay plan,\r\n"
						+"you may only set the status to Received or Supplemental.");
				}
				SetComboStatus(_claimProc.Status);//Go back to previous selection.
				return;
			}
			#endregion
			#region Insurance Plan Attached
			bool isValidPlanType=true;
			switch(comboStatus.SelectedIndex) {
				case 0:
					if(_insPlan.PlanType=="c") {
						isValidPlanType=false;
						break;
					}
					_claimProc.Status=ClaimProcStatus.Estimate;
					break;
				case 1:
					if(_insPlan.PlanType=="c") {
						isValidPlanType=false;
						break;
					}
					_claimProc.Status=ClaimProcStatus.NotReceived;
					break;
				case 2:
					if(_insPlan.PlanType=="c") {
						isValidPlanType=false;
						break;
					}
					_claimProc.Status=ClaimProcStatus.Received;
					break;
				case 3:
					_claimProc.Status=ClaimProcStatus.Preauth;
					break;
				case 4:
					if(_insPlan.PlanType=="c") {
						isValidPlanType=false;
						break;
					}
					_claimProc.Status=ClaimProcStatus.Supplemental;
					break;
				case 5:
					if(_insPlan.PlanType!="c") {
						isValidPlanType=false;
						break;
					}
					_claimProc.Status=ClaimProcStatus.CapClaim;
					break;
				case 6:
					if(_insPlan.PlanType!="c") {
						isValidPlanType=false;
						break;
					}
					_claimProc.Status=ClaimProcStatus.CapEstimate;
					break;
				case 7:
					if(_insPlan.PlanType!="c") {
						isValidPlanType=false;
						break;
					}
					_claimProc.Status=ClaimProcStatus.CapComplete;
					//Capitation procedures are not usually attached to a claim.
					//In order for Aging to calculate properly the ProcDate (Date Completed) and DateCP (Payment Date) must be the same.
					_claimProc.DateCP=_procedure.ProcDate;
					break;
				case 8:
					if(_insPlan.PlanType=="c") {
						isValidPlanType=false;
						break;
					}
					_claimProc.Status=ClaimProcStatus.InsHist;
					break;
			}
			if(!isValidPlanType) {
				if(_insPlan.PlanType=="") {
					MsgBox.Show(this,"A category percentage insurance plan is associated with this claim procedure.\r\n"
						+"You may only select statuses which are related to category percentage,\r\n"
						+"including Estimate, NotReceived, Received, Supplemental, and PreAuthorization.\r\n"
						+"To change the status to a different option, you must change the plan type.");
				}
				else if(_insPlan.PlanType=="p") {
					MsgBox.Show(this,"A PPO percentage insurance plan is associated with this claim procedure.\r\n"
						+"You may only select statuses which are related to PPO percentage,\r\n"
						+"including Estimate, NotReceived, Received, Supplemental, and PreAuthorization.\r\n"
						+"To change the status to a different option, you must change the plan type.");
				}
				else if(_insPlan.PlanType=="f") {
					MsgBox.Show(this,"A flat co-pay insurance plan is associated with this claim procedure.\r\n"
						+"You may only select statuses which are related to flat co-pay insurance,\r\n"
						+"including Estimate, NotReceived, Received, Supplemental, and PreAuthorization.\r\n"
						+"To change the status to a different option, you must change the plan type.");
				}
				else if(_insPlan.PlanType=="c") {
					MsgBox.Show(this,"A capitation insurance plan is associated with this claim procedure.\r\n"
						+"You may only select statuses which are related to capitation insurance,\r\n"
						+"including CapClaim, CapEstimate, CapComplete, and PreAuthorization.\r\n"
						+"To change the status to a different option, you must change the plan type.");
				}
				SetComboStatus(_claimProc.Status);//Go back to previous selection.
			}
			#endregion
			if(_claimProc.Status==ClaimProcStatus.Received || _claimProc.Status==ClaimProcStatus.Supplemental) {
				labelDateEntry.Visible=true;
				textDateEntry.Visible=true;
			}
			else {
				labelDateEntry.Visible=false;
				textDateEntry.Visible=false;
			}
		}

		private void checkNoBillIns_Click(object sender, System.EventArgs e) {
			ComputeAmounts();
		}

		private void textAllowedOverride_Leave(object sender,System.EventArgs e) {
			ComputeAmounts();
		}

		private void textCopayOverride_Leave(object sender,System.EventArgs e) {
			ComputeAmounts();
		}

		private void textCopayOverride_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(_claimProc.Status!=ClaimProcStatus.CapEstimate
				&& _claimProc.Status!=ClaimProcStatus.CapComplete){
				return;
			}
			if(!textCopayOverride.IsValid()) {
				return;
			}
			double copay=PIn.Double(textCopayAmt.Text);//Default to the default copay amount
			if(textCopayOverride.Text!="") {//If override is specified, use that amount instead
				copay=PIn.Double(textCopayOverride.Text);
			}
			//always a procedure
			double writeoff=_procedure.ProcFee-copay;
			if(writeoff<0) {
				writeoff=0;
			}
			textWriteOff.Text=writeoff.ToString("n");
		}

		private void textPercentOverride_Leave(object sender, System.EventArgs e) {
			ComputeAmounts();
		}

		private void textDedEstOverride_Leave(object sender,EventArgs e) {
			ComputeAmounts();
		}

		private void textPaidOtherInsOverride_Leave(object sender,EventArgs e) {
			ComputeAmounts();
		}

		private void textInsEstTotalOverride_Leave(object sender,EventArgs e) {
			ComputeAmounts();
		}

		private void textDedApplied_Leave(object sender,System.EventArgs e) {
			ComputeAmounts();
		}

		private void textInsPayEst_Leave(object sender, System.EventArgs e) {
			ComputeAmounts();
		}

		private void textWriteOffEstOverride_Leave(object sender,EventArgs e) {
			ComputeAmounts();
		}

		private void textInsPayAmt_Leave(object sender, System.EventArgs e) {
			ComputeAmounts();
		}

		private void textInsPayAmt_Enter(object sender,EventArgs e) {
			if(!textInsPayAmt.ReadOnly) {//If this box is readonly, show them the security warning that disabled it when the user clicks into it.
				return;
			}
			if(CompareDouble.IsZero(_claimProc.InsPayAmt)) { 
				if(!Security.IsAuthorized(EnumPermType.InsPayCreate)) { //user not allowed to create an insurance payment
					return;
				}
			}
			else {
				if(!Security.IsAuthorized(EnumPermType.InsPayEdit,_claimProc.DateCP)) { //user not allowed to edit an insurance payment
					return;
				}
			}
		}

		private void textWriteOff_Leave(object sender, System.EventArgs e) {
			ComputeAmounts();
		}

		private void textWriteOff_Enter(object sender,EventArgs e) {
			if(!textWriteOff.ReadOnly) {//In this window if the box is readonly when the user clicks into it show them the security warning that disabled it.
				return;
			}
			DateTime dateWriteoffSec=_claimProc.SecDateEntry;
			if(IsProc && _procedure.ProcDate!=DateTime.MinValue) {
				dateWriteoffSec=_procedure.DateEntryC;
			}
			if(!Security.IsAuthorized(EnumPermType.InsWriteOffEdit,dateWriteoffSec)) {
				return;
			}
		}

		private void textWriteOffEstOverride_Enter(object sender,EventArgs e) {
			if(!textWriteOffEstOverride.ReadOnly) {//In this window if the box is readonly when the user clicks into it show them the security warning that disabled it.
				return;
			}
			DateTime dateWriteoffSec=_claimProc.SecDateEntry;
			if(IsProc && _procedure.ProcDate!=DateTime.MinValue) {
				dateWriteoffSec=_procedure.DateEntryC;
			}
			if(!Security.IsAuthorized(EnumPermType.InsWriteOffEdit,dateWriteoffSec)) {
				return;
			}
		}

		private void checkPayPlan_Click(object sender,EventArgs e) {
			if(checkPayPlan.Checked) {
				List<PayPlan> listPayPlans=PayPlans.GetValidInsPayPlans(_claimProc.PatNum,_claimProc.PlanNum,_claimProc.InsSubNum,_claimProc.ClaimNum);
				if(listPayPlans.Count==0) {//no valid plans
					MsgBox.Show(this,"The patient does not have a valid payment plan with this insurance plan attached that has not been paid in full and is not tracking expected payments for an existing claim already.");
					checkPayPlan.Checked=false;
					return;
				}
				if(listPayPlans.Count==1) { //if there is only one valid payplan
					_claimProc.PayPlanNum=listPayPlans[0].PayPlanNum;
					return;
				}
				//more than one valid PayPlan
				using FormPayPlanSelect formPayPlanSelect=new FormPayPlanSelect(listPayPlans);
				formPayPlanSelect.ShowDialog();
				if(formPayPlanSelect.DialogResult==DialogResult.Cancel) {
					checkPayPlan.Checked=false;
					return;
				}
				_claimProc.PayPlanNum=formPayPlanSelect.PayPlanNumSelected;
			}
			else {//payPlan unchecked
				_claimProc.PayPlanNum=0;
			}
		}

		///<summary>Claimprocs with various statuses can be deleted,
		///except certain specific scenarios where the user does not have permission (multiple different permissions are considered).</summary>
		private void butDelete_Click(object sender, System.EventArgs e) {
			if(_claimProc.IsTransfer) {
				if(MessageBox.Show(Lan.g(this,"This Claim Procedure is part of an income transfer."+"\r\n"
					+"Deleting this claim procedure will delete all of the income transfers for this claim.  Continue?"),""
					,MessageBoxButtons.OKCancel)!=DialogResult.OK)	
				{
					return;
				}
			}
			else if(CultureInfo.CurrentCulture.Name.EndsWith("CA") 
				&& _claimProc.ProcNum!=0//not a 'Total Payment' row
				&& _claimProc.ProcNum==_procedure.ProcNum && _procedure.ProcNumLab==0)//not a lab
			{
				if(MessageBox.Show(Lan.g(this,
					"Deleting this insurance payment will also delete the insurance payment for any attached labs. Continue?"),
					"",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
					return;
				}
			}
			else {
				if(MessageBox.Show(Lan.g(this,"Delete this estimate?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK){
					return;
				}
			}
			try {
				ClaimProcs.DeleteAfterValidating(_claimProc);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			ClaimProcs.RemoveSupplementalTransfersForClaims(_claimProc.ClaimNum);
			_claimProc.DoDelete=true;
			//IsSaved=false;
			DialogResult=DialogResult.OK;
		}

		private List<ClaimProc> GetListClaimProcHypothetical() {
			List<ClaimProc> listClaimProcHypothetical=new List<ClaimProc>();
			ClaimProc claimProcHypothetical=_claimProc.Copy();
			claimProcHypothetical.InsPayAmt=PIn.Double(textInsPayAmt.Text);
			claimProcHypothetical.WriteOff=PIn.Double(textWriteOff.Text);
			listClaimProcHypothetical.Add(claimProcHypothetical);
			return listClaimProcHypothetical;
		}

		private bool AllAreValid(){
			if(  !textFeeBilled.IsValid()
				|| !textAllowedOverride.IsValid()
				|| !textCopayOverride.IsValid()
				|| !textPercentOverride.IsValid()
				|| !textDedEstOverride.IsValid()
				|| !textPaidOtherInsOverride.IsValid()
				|| !textInsEstTotalOverride.IsValid()
				|| !textDedApplied.IsValid()
				|| !textInsPayEst.IsValid()
				|| !textWriteOffEstOverride.IsValid()//Disallow negative writeoffs, field must be >=0
				|| !textInsPayAmt.IsValid()
				|| !textWriteOff.IsValid()
				|| !textProcDate.IsValid()
				|| !textDateCP.IsValid()
				){
				return false;
			}
			return true;
		}

		private void ButBlueBookLog_Click(object sender,EventArgs e) {
			List<InsBlueBookLog> listInsBlueBookLogs=InsBlueBookLogs.GetAllByClaimProcNum(_claimProc.ClaimProcNum);
			InsBlueBookLog insBlueBookLog=BlueBookEstimateData_.CreateInsBlueBookLog(_claimProc,false);
			if(insBlueBookLog!=null) {
				//Create a placeholder log entry for what the user should expect when the OK button is clicked.
				insBlueBookLog.DateTEntry=DateTime.Now;
				listInsBlueBookLogs.Add(insBlueBookLog);
			}
			if(listInsBlueBookLogs.Count==0) {
				MsgBox.Show(this,"This Claim Procedure has no Blue Book Log history.");
				return;
			}
			using FormClaimProcBlueBookLog formClaimProcBlueBookLog=new FormClaimProcBlueBookLog(listInsBlueBookLogs);
			formClaimProcBlueBookLog.ShowDialog();
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			//no security check here because if attached to a payment, nobody is allowed to change the date or amount anyway.
			if(!AllAreValid()){
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			if(PIn.Date(textDateCP.Text).Date > DateTime.Today.Date
				&& !PrefC.GetBool(PrefName.FutureTransDatesAllowed) 
				&& !PrefC.GetBool(PrefName.AllowFutureInsPayments)
				&& _claimProc.Status.In(ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.CapClaim,ClaimProcStatus.CapComplete)) 
			{ 
				MsgBox.Show(this,"Payment date cannot be for the future.");
				return;
			}
			if(_claimProc.WriteOff<0 && _claimProc.Status!=ClaimProcStatus.Supplemental) {
				MsgBox.Show(this,"Only supplemental payments may have a negative write-off amount.");
				return;
			}
			double claimWriteoffTotal=ClaimProcs.GetClaimWriteOffTotal(_claimProc.ClaimNum,_claimProc.ProcNum,new List<ClaimProc>() { _claimProc });
			if(claimWriteoffTotal+_claimProc.WriteOff<0) {
				MsgBox.Show(this,"The current write-off value will cause the procedure's total write-off to be negative.  Please change it to at least "+(_claimProc.WriteOff-(claimWriteoffTotal+_claimProc.WriteOff)).ToString()+" to continue.");
				return;
			}
			if(IsProc && ClaimL.AreCreditsGreaterThanProcFee(GetListClaimProcHypothetical())) {
				return;
			}
			if(_claimProc.Status.In(ClaimProcStatus.Received,ClaimProcStatus.Supplemental)
				&& !Security.IsAuthorized(EnumPermType.InsPayEdit,PIn.Date(textDateCP.Text))) {
				return;
			}
			if(OrthoProcLinks.IsProcLinked(_claimProc.ProcNum) && 
				(_claimProcOld.AllowedOverride!=PIn.Double(textAllowedOverride.Text)
				|| !_claimProcOld.CopayOverride.Equals(PIn.Double(textCopayOverride.Text))
				|| !_claimProcOld.DedEstOverride.Equals(PIn.Double(textDedEstOverride.Text))
				|| !_claimProcOld.PercentOverride.Equals(PIn.Int(textPercentOverride.Text))
				|| !_claimProcOld.PaidOtherInsOverride.Equals(PIn.Double(textPaidOtherInsOverride.Text))
				|| !_claimProcOld.InsEstTotalOverride.Equals(PIn.Double(textInsEstTotalOverride.Text))
				|| !_claimProcOld.WriteOffEstOverride.Equals(PIn.Double(textWriteOffEstOverride.Text))
				|| !_claimProcOld.InsPayEst.Equals(PIn.Double(textInsPayEst.Text))
				)) 
			{
				MsgBox.Show(this,"Cannot edit estimate information for procedures attached to ortho cases.");
				return;
			}
			//status already handled
			if(comboProvider.SelectedIndex!=-1) {//if no prov selected, then that prov must simply be hidden,
				//because all claimprocs are initially created with a prov(except preauth).
				//So, in this case, don't change.
				_claimProc.ProvNum=_listProviders[comboProvider.SelectedIndex].ProvNum;
			}
			_claimProc.ProcDate=PIn.Date(textProcDate.Text);
			if(!textDateCP.ReadOnly){
				_claimProc.DateCP=PIn.Date(textDateCP.Text);
			}
			_claimProc.CodeSent=textCodeSent.Text;
			_claimProc.FeeBilled=PIn.Double(textFeeBilled.Text);
			_claimProc.Remarks=textRemarks.Text;
			//if status was changed to received, then set DateEntry
			if(_claimProcOld.Status!=ClaimProcStatus.Received && _claimProcOld.Status!=ClaimProcStatus.Supplemental){
				if(_claimProc.Status==ClaimProcStatus.Received || _claimProc.Status==ClaimProcStatus.Supplemental){
					_claimProc.DateEntry=DateTime.Now;
				}
			}
			_claimProc.ClaimPaymentTracking=comboPayTracker.SelectedIndex==0 ? 0 : _listDefsPayTracks[comboPayTracker.SelectedIndex-1].DefNum;
			if(_doSaveToDb) {
				//Fix pre-auth statuses.
				Claim claim=Claims.GetClaim(_claimProc.ClaimNum);
				if(claim?.ClaimType=="PreAuth" && _claimProc.Status!=ClaimProcStatus.Preauth) {
						_claimProc.Status=ClaimProcStatus.Preauth;//change the status to preauth.
						MsgBox.Show(this,"Status of procedure was changed back to preauth to match status of claim.");
				}
				InsBlueBookLog insBlueBookLog=BlueBookEstimateData_.CreateInsBlueBookLog(_claimProc);
				if(insBlueBookLog!=null) {
					InsBlueBookLogs.Insert(insBlueBookLog);
				}
				ClaimProcs.Update(_claimProc,_claimProcOld);
				if(ClaimProcCrud.UpdateComparison(_claimProc,_claimProcOld)) {
					ClaimProcs.RemoveSupplementalTransfersForClaims(_claimProc.ClaimNum);
				}
				if(_claimProc.Status!=_claimProcOld.Status && IsProc) {
					//We must update the DB such that any associated Canadian labs have the same status as their parent claimproc.
					//If we do not do this then ClaimProcs.CanadianLabBaseEstHelper(...) will fail to match and will not update any existing lab claimpros either.
					//Instead it would insert an new lab claim proc with the parents new claim procs status.
					ClaimProcs.UpdatePertinentLabStatuses(_claimProc,_insPlan);//Checks for Canada, simply returns if not.
				}
			}//otherwise, the change to db will be made by calling class
			//there is no functionality here for insert cur, because all claimprocs are
			//created before editing.
			if(_claimProc.ClaimPaymentNum>0){//attached to ins check
				//note: the amount and the date will not have been changed.
				SecurityLogs.MakeLogEntry(EnumPermType.InsPayEdit,_claimProc.PatNum,
					Patients.GetLim(_claimProc.PatNum).GetNameLF()+", "
					+Lan.g(this,"Date and amount not changed."));//I'm really not sure what they would have changed.
			}
			if(_claimProc.Status.In(ClaimProcStatus.Received,ClaimProcStatus.NotReceived,ClaimProcStatus.CapClaim)
				&& _claimProc.ProvNum != _claimProcOld.ProvNum) 
			{
				string strSecLog;
				if(_procedure == null) {
					strSecLog = "Total Payment for "+textInsPlan.Text+". "+Lan.g(this,"Provider changed from")+" "
					+Providers.GetAbbr(_claimProcOld.ProvNum)+" "+Lan.g(this,"to")+" "+Providers.GetAbbr(_claimProc.ProvNum);
				}
				else {
					strSecLog = ProcedureCodes.GetProcCode(_procedure.CodeNum).ProcCode+" - "+textInsPlan.Text+". "+Lan.g(this,"Provider changed from")+" "
					+Providers.GetAbbr(_claimProcOld.ProvNum)+" "+Lan.g(this,"to")+" "+Providers.GetAbbr(_claimProc.ProvNum);
				}
				SecurityLogs.MakeLogEntry(EnumPermType.ClaimProcClaimAttachedProvEdit,_claimProc.PatNum,strSecLog);
			}
			//IsSaved=true;
			MakeAuditTrailEntries();
			DialogResult=DialogResult.OK;
		}

		private void MakeAuditTrailEntries() {
			string insWriteoffEditLog="";
			string insPayEditLog="";
			if(_procedure!=null && _procedure.CodeNum!=0) {//Could happen with pay "As Total". Still want to log, just without procedure.
				string strProcCode=ProcedureCodes.GetStringProcCode(_procedure.CodeNum);
				insWriteoffEditLog=$"Procedure: {strProcCode}. ";
				insPayEditLog=$"Procedure: {strProcCode}. ";
			}
			bool needsEditLog=false;
			if(_claimProcOld.WriteOff!=_claimProc.WriteOff) {
				insWriteoffEditLog+=$"Write-off amount changed from {_claimProcOld.WriteOff.ToString("C")} to {_claimProc.WriteOff.ToString("C")}. ";
				needsEditLog=true;
			}
			double writeoffEstOld=ClaimProcs.GetWriteOffEstimate(_claimProcOld);//WriteOffEst is never user editable, we have to check overrides
			double writeoffEstCur=ClaimProcs.GetWriteOffEstimate(_claimProc);//WriteOffEst is never user editable, we have to check overrides
			if(writeoffEstOld!=writeoffEstCur) {
				insWriteoffEditLog+=$"Write-off estimate amount changed from {writeoffEstOld.ToString("C")} to {writeoffEstCur.ToString("C")}. ";
				needsEditLog=true;
			}
			if(needsEditLog) {
				SecurityLogs.MakeLogEntry(EnumPermType.InsWriteOffEdit,_claimProc.PatNum,insWriteoffEditLog);
			}
			needsEditLog=false;
			if(_claimProcOld.InsPayAmt!=_claimProc.InsPayAmt) {
				insPayEditLog+=$"Insurance payment amount changed from {_claimProcOld.InsPayAmt.ToString("C")} to "
					+$"{_claimProc.InsPayAmt.ToString("C")}. ";
				needsEditLog=true;
			}
			if(_claimProcOld.InsPayEst!=_claimProc.InsPayEst) {
				insPayEditLog+=$"Insurance payment estimate amount changed from {_claimProcOld.InsPayEst.ToString("C")} to "
					+$"{_claimProc.InsPayEst.ToString("C")}. ";
				needsEditLog=true;
			}
			if(!needsEditLog && IsCalledFromClaimEdit) {
				string insPayCreateLog="";
				if(_procedure!=null && _procedure.CodeNum!=0) { 
					insPayCreateLog+=$"Procedure: {ProcedureCodes.GetStringProcCode(_procedure.CodeNum)}. ";
				}
				insPayCreateLog+=$"Insurance payment amount {_claimProc.InsPayAmt.ToString("C")}. ";
				insPayCreateLog+=$"Insurance estimate amount {_claimProc.InsPayEst.ToString("C")}. ";
				SecurityLogs.MakeLogEntry(EnumPermType.InsPayCreate,_claimProc.PatNum,insPayCreateLog);
			} 
			else if(needsEditLog && IsCalledFromClaimEdit) {
				SecurityLogs.MakeLogEntry(EnumPermType.InsPayCreate,_claimProc.PatNum,insPayEditLog);
			}
			else if(needsEditLog && !IsCalledFromClaimEdit){
				SecurityLogs.MakeLogEntry(EnumPermType.InsPayEdit,_claimProc.PatNum,insPayEditLog);
			}
		}

		private void FormClaimProc_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK){
				InsBlueBooks.SynchForClaimNums(_claimProc.ClaimNum);
				return;
			}
			_claimProc=_claimProcOld.Copy();//revert back to the old ClaimProc.  Only important if not SaveToDb
		}

	}
}