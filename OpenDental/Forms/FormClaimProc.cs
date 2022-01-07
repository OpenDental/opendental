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
		private ClaimProc ClaimProcCur;
		///<summary>If user hits cancel, then the claimproc is reset using this.  Do not modify the values in this claimproc anywhere in this form.</summary>
		private ClaimProc ClaimProcOld;
		///<summary>Similar to ClaimProcOld, except this variable is set at the end of initialization, because the ClaimProcCur can change during
		///initialization, due to calling ComputeAmounts()</summary>
		public ClaimProc ClaimProcInitial;
		///<summary>The procedure to which this claimproc is attached.  Sent in if this is launched from the Procedure Edit window,
		///otherwise pulled from the db when form loads if ClaimProcCur.ProcNum>0, which also causes IsProc to be set to true.</summary>
		private Procedure proc;
		//Note: Consider removing IsProc. IsProc seems to be an unecessary bool since proc==null is equivalent to IsProc==false.
		///<summary>True if this is a procedure, and false if only a claim total.  Private variable, name should be updated.</summary>
		private bool IsProc;
		private Family FamCur;
		private Patient PatCur;
		private List <InsPlan> PlanList;
		///<summary>List of substitution links.  Lazy loaded, do not directly use this variable, use the property instead.</summary>
		private List<SubstitutionLink> _listSubLinks=null;
		private InsPlan Plan;
		private long PatPlanNum;
		private List<Benefit> BenefitList;
		private List<ClaimProcHist> HistList;
		private List<ClaimProcHist> LoopList;
		private List<PatPlan> PatPlanList;
		///<summary>This value is obtained by a query when this window first opens.  It includes estimates if the other claims are not received and 
		///includes the payment amount if the other claims are received.  Will be 0 if this is a primary estimate.</summary>
		private double PaidOtherInsTotal;
		private double PaidOtherInsBaseEst;
		///<summary>This value is obtained by a query when this window first opens.  It includes both actual writeoffs and estimated writeoffs.  Will be 0 if this is a primary estimate.</summary>
		private double WriteOffOtherIns;
		private bool SaveToDb;
		private List<InsSub> SubList;
		private List<Def> _listPayTrackDefs;
		private List<Provider> _listProviders;
		///<summary>Holds all data needed to calculate a blue book allowed amount.</summary>
		private BlueBookEstimateData _blueBookEstimateData;
		///<summary>Set to true if this claimProc is accessed from within a claim or from within FormClaimPayTotal. This changes the behavior of the form, allowing more freedom with fields that are also totalled for entire claim.  This freedom is normally restricted so that claim totals will stay synchronized with individual claimprocs.  If true, it will still save changes to db, even though this is duplicated effort in FormClaimPayTotal.</summary>
		public bool IsInClaim;
		///<summary>Set this to true if user does not have permission to edit procedure.</summary>
		public bool NoPermissionProc;
		public bool IsSaved;
		///<summary>Is only set when called from FormClaimEdit and to signify the recieving of a claim.</summary>
		public bool IsCalledFromClaimEdit=false;

		private List<SubstitutionLink> ListSubLinks {
			get {
				if(_listSubLinks==null) {
					_listSubLinks=SubstitutionLinks.GetAllForPlans(PlanList);
				}
				return _listSubLinks;
			}
		}

		///<summary>procCur can be null if not editing from within an actual procedure.  If the save is to happen within this window, then set saveToDb true.  If the object is to be altered here, but saved in a different window, then saveToDb=false.</summary>
		public FormClaimProc(ClaimProc claimProcCur,Procedure procCur,Family famCur,Patient patCur,List<InsPlan> planList,List<ClaimProcHist> histList
			,ref List<ClaimProcHist> loopList,List<PatPlan> patPlanList,bool saveToDb,List<InsSub> subList,BlueBookEstimateData blueBookEstimateData=null)
		{
			ClaimProcCur=claimProcCur;//always work directly with the original object.  Revert if we change our mind.
			ClaimProcOld=ClaimProcCur.Copy();
			proc=procCur;
			FamCur=famCur;
			PatCur=patCur;
			PlanList=planList;
			SubList=subList;
			HistList=histList;
			LoopList=loopList;
			PatPlanList=patPlanList;
			SaveToDb=saveToDb;
			_blueBookEstimateData=blueBookEstimateData;
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
				butOK,
				butCancel,
				butDelete,
			});
		}

		private void FormClaimProcEdit_Load(object sender, System.EventArgs e) {
			Initialize();
		}

		///<summary>Same as calling FormClaimProcEdit_Load().  Used in unit test 28.</summary>
		public void Initialize() {
			//Check to see if the Claim is a transfer, if TRUE disable all but Cancel and Delete buttons
			if(ClaimProcCur.IsTransfer) {
				this.DisableAllExcept(new Control[]{butCancel,butDelete});
			}
			if(ClaimProcCur.ClaimNum>0) {
				Claim claim=Claims.GetClaim(ClaimProcCur.ClaimNum);
				if(claim==null) {
					MsgBox.Show(this,"Claim has been deleted by another user.");
					DialogResult=DialogResult.Abort;
					return;
				}
				if(ClaimProcCur.ClaimNum>0 && ClaimProcCur.Status==ClaimProcStatus.Received
					//Prior to version 16.3.7 this perm check used claim.DateReceived but users with ClaimSentEdit perm but not ClaimProcReceivedEdit perm could
					//edit the claim Date Received field and subvert the security perm intended to prevent them from editing the claimproc
					&& !Security.IsAuthorized(Permissions.ClaimProcReceivedEdit,ClaimProcCur.DateEntry,false))
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
					butOK.Enabled=false;
					butDelete.Enabled=false;
				}
				else if((claim.ClaimStatus=="S" || claim.ClaimStatus=="R")//sent or received
					&& !Security.IsAuthorized(Permissions.ClaimSentEdit,claim.DateSent,true)) //attached to claim, no permission for claims.
				{
					butOK.Enabled=false;
					butDelete.Enabled=false;
				}
			}
			if((butOK.Enabled || butDelete.Enabled) && NoPermissionProc) {//blocks users with no permission to edit procedure
					butOK.Enabled=false;
					butDelete.Enabled=false;
			}
			InsSub sub=InsSubs.GetSub(ClaimProcCur.InsSubNum,SubList);
			Plan=InsPlans.GetPlan(sub.PlanNum,PlanList);
			PatPlanNum=PatPlans.GetPatPlanNum(sub.InsSubNum,PatPlanList);
			BenefitList=null;//only fill it if proc
			PaidOtherInsTotal=ClaimProcs.GetPaidOtherInsTotal(ClaimProcCur,PatPlanList);
			PaidOtherInsBaseEst=ClaimProcs.GetPaidOtherInsBaseEst(ClaimProcCur,PatPlanList);
			WriteOffOtherIns=ClaimProcs.GetWriteOffOtherIns(ClaimProcCur,PatPlanList);
			List<InsSub> subList=InsSubs.RefreshForFam(FamCur);
			textInsPlan.Text=InsPlans.GetDescript(ClaimProcCur.PlanNum,FamCur,PlanList,ClaimProcCur.InsSubNum,subList);
			checkNoBillIns.Checked=ClaimProcCur.NoBillIns;
			if(ClaimProcCur.ClaimPaymentNum>0) {//attached to ins check
				textDateCP.ReadOnly=true;//DateCP always the same as the payment date and can't be changed here
				if(!Security.IsAuthorized(Permissions.InsPayEdit,ClaimProcCur.DateCP)) {
					butOK.Enabled=false;
					if(ClaimProcCur.Status==ClaimProcStatus.Received) {
						comboStatus.Enabled=false;
					}
				}
				textInsPayAmt.ReadOnly=true;
				labelAttachedToCheck.Visible=true;
				butDelete.Enabled=false;
			}
			//This new expanded security prevents editing completed claimprocs, even if not attached to an ins check.
			//For example, a zero payment with a writeoff amount.  Must prevent changing that date.
			else if((ListTools.In(ClaimProcCur.Status,ClaimProcStatus.CapComplete,ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.InsHist))
				&& (IsProc || !ClaimProcCur.IsNew)
				&& !Security.IsAuthorized(Permissions.InsPayEdit,ClaimProcCur.DateCP))//
			{
				textDateCP.ReadOnly=true;
				butOK.Enabled=false;
				textInsPayAmt.ReadOnly=true;
				labelAttachedToCheck.Visible=false;
				//listStatus.Enabled=false;//this is handled in the mousedown event
				butDelete.Enabled=false;
				comboStatus.Enabled=false;
			}
			else {
				labelAttachedToCheck.Visible=false;
			}
			if(ClaimProcCur.ProcNum==0) {//total payment for a claim
				IsProc=false;
				textDescription.Text="Total Payment";
				textProcDate.ReadOnly=false;
			}
			else {
				IsProc=true;
				BenefitList=Benefits.RefreshForPlan(ClaimProcCur.PlanNum,PatPlanNum);
				if(proc==null) {
					proc=Procedures.GetOneProc(ClaimProcCur.ProcNum,false);
				}
				textDescription.Text=ProcedureCodes.GetProcCode(proc.CodeNum).Descript;
				textProcDate.ReadOnly=true;//user not allowed to edit ProcDate unless it's for a total payment
			}
			if(_blueBookEstimateData==null) {
				List<Procedure> listProcedures=new List<Procedure>();
				if(proc!=null) {
					listProcedures.Add(proc);
				}
				_blueBookEstimateData=new BlueBookEstimateData(PlanList,SubList,PatPlanList,listProcedures,ListSubLinks);
			}
			//get the date to use for checking whether the user has InsWriteOffEdit permission
			DateTime writeOffSecDate=ClaimProcCur.SecDateEntry;//if this is a total payment, there is no proc so use ClaimProcCur.SecDateEntry
			//if this is claimproc is attached to a proc, and the proc returned by GetOneProc (called above if proc was null) is a valid proc, use DateEntryC
			if(IsProc && proc.ProcDate!=DateTime.MinValue) {
				writeOffSecDate=proc.DateEntryC;
			}
			if(CompareDouble.IsZero(ClaimProcCur.InsPayAmt)) { 
				if(!Security.IsAuthorized(Permissions.InsPayCreate,true)) { //user not allowed to create an insurance payment
					textInsPayAmt.ReadOnly=true;
				}
			}
			else {
				if(!Security.IsAuthorized(Permissions.InsPayEdit,ClaimProcCur.DateCP,true)) { //user not allowed to edit an insurance payment
					textInsPayAmt.ReadOnly=true;
				}
			}
			if(!Security.IsAuthorized(Permissions.InsWriteOffEdit,writeOffSecDate,true)) {//user not allowed to edit/create a writeoff
				textWriteOff.ReadOnly=true;
				textWriteOffEstOverride.ReadOnly=true;
				//cannot edit the writeoff, so block deleting the claimproc, otherwise they could delete and recreate to bypass the date/days restriction
				butDelete.Enabled=false;
			}
			if(ClaimProcCur.ClaimNum>0) {//attached to claim
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
				if(ClaimProcCur.ProcNum==0) {//if a total entry rather than by proc
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
				else if(ClaimProcCur.Status==ClaimProcStatus.Received) {
					labelInsPayAmt.Font=new Font(labelInsPayAmt.Font,FontStyle.Bold);
				}
				if(ListTools.In(ClaimProcCur.Status,ClaimProcStatus.Received,ClaimProcStatus.NotReceived,ClaimProcStatus.CapClaim) 
					&& !Security.IsAuthorized(Permissions.ClaimProcClaimAttachedProvEdit,true))
				{
					comboProvider.Enabled=false;
					butPickProv.Enabled=false;
				}
				//butOK.Enabled=false;
				//butDelete.Enabled=false;
				//MessageBox.Show(panelEstimateInfo.Visible.ToString());
			}
			else if(ClaimProcCur.PlanNum>0 && ListTools.In(ClaimProcCur.Status,ClaimProcStatus.CapEstimate,ClaimProcStatus.CapComplete)) { //not attached to a claim
				//InsPlans.Cur.PlanType=="c"){//capitation proc,whether Estimate or CapComplete,never billed to ins
				foreach(System.Windows.Forms.Control control in panelEstimateInfo.Controls) {
					control.Visible=false;
				}
				foreach(System.Windows.Forms.Control control in groupClaimInfo.Controls) {
					control.Visible=false;
				}
				groupClaimInfo.Text="";
				labelFee.Visible=true;
				textFee.Visible=true;
				labelCopayAmt.Visible=true;
				textCopayAmt.Visible=true;
				textCopayOverride.Visible=true;
				if(ClaimProcCur.Status==ClaimProcStatus.CapEstimate) {
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
			else if(ClaimProcCur.PlanNum>0 && ClaimProcCur.Status==ClaimProcStatus.InsHist) {
				groupClaimInfo.Visible=false;
				groupAllowed.Visible=false;
				groupClaim.Visible=false;
				//InsPlans.Cur.PlanType=="c"){//capitation proc,whether Estimate or CapComplete,never billed to ins
				foreach(System.Windows.Forms.Control control in panelEstimateInfo.Controls) {
					control.Visible=false;
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
			SetComboStatus(ClaimProcCur.Status);
			if(ClaimProcCur.Status==ClaimProcStatus.Received || ClaimProcCur.Status==ClaimProcStatus.Supplemental) {
				labelDateEntry.Visible=true;
				textDateEntry.Visible=true;
			}
			else {
				labelDateEntry.Visible=false;
				textDateEntry.Visible=false;
			}
			comboProvider.Items.Clear();
			_listProviders=Providers.GetProvsForClinic(ClaimProcCur.ClinicNum);
			for(int i=0;i<_listProviders.Count;i++) {
				comboProvider.Items.Add(_listProviders[i].Abbr,_listProviders[i]);
				if(ClaimProcCur.ProvNum==_listProviders[i].ProvNum) {
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
				textClinic.Text=Clinics.GetAbbr(ClaimProcCur.ClinicNum);
			}
			textDateEntry.Text=ClaimProcCur.DateEntry.ToShortDateString();
			if(ClaimProcCur.ProcDate.Year<1880) {
				textProcDate.Text="";
			}
			else {
				textProcDate.Text=ClaimProcCur.ProcDate.ToShortDateString();
			}
			if(ClaimProcCur.DateCP.Year<1880) {
				textDateCP.Text="";
			}
			else {
				textDateCP.Text=ClaimProcCur.DateCP.ToShortDateString();
			}
			textCodeSent.Text=ClaimProcCur.CodeSent;
			textFeeBilled.Text=ClaimProcCur.FeeBilled.ToString("n");
			textClaimAdjReasonCodes.Text=ClaimProcCur.ClaimAdjReasonCodes;
			textRemarks.Text=ClaimProcCur.Remarks;
			if(ClaimProcCur.PayPlanNum==0) {
				checkPayPlan.Checked=false;
			}
			else {
				checkPayPlan.Checked=true;
			}
			_listPayTrackDefs=Defs.GetDefsForCategory(DefCat.ClaimPaymentTracking,true);
			comboPayTracker.Items.Add("None");
			for(int i=0;i<_listPayTrackDefs.Count;i++) {
				comboPayTracker.Items.Add(_listPayTrackDefs[i].ItemName);
				if(_listPayTrackDefs[i].DefNum==ClaimProcCur.ClaimPaymentTracking) {
					comboPayTracker.SelectedIndex=i+1;
				}
			}
			if(comboPayTracker.SelectedIndex==-1) {
				comboPayTracker.SelectedIndex=0;
			}
			//Not allowed to change status if attached to a claim payment.
			if(ClaimProcOld.ClaimPaymentNum > 0) {
				comboStatus.Enabled=false;
				if(Plan.PlanType!="c" &&
					(ClaimProcOld.Status==ClaimProcStatus.CapComplete
						|| ClaimProcOld.Status==ClaimProcStatus.CapClaim
						|| ClaimProcOld.Status==ClaimProcStatus.CapEstimate))
				{
					//One of our customers somehow had CapComplete procedures attached to insurance payments for insurnace plans that are not capitation.
					comboStatus.Enabled=true;
				}
			}
			//Not allowed to change status if cap estimate or cap complete and the plan is a capitation plan.
			if(Plan.PlanType=="c" && (ClaimProcOld.Status==ClaimProcStatus.CapComplete || ClaimProcOld.Status==ClaimProcStatus.CapEstimate)) {
				comboStatus.Enabled=false;
			}
			//Not allowed to change status if estimate or inshist and is not a capitation plan.
			if(Plan.PlanType!="c" && ListTools.In(ClaimProcOld.Status,ClaimProcStatus.Estimate,ClaimProcStatus.InsHist)) {
				comboStatus.Enabled=false;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				if(IsProc && proc.ProcNumLab!=0) {//We're loading this form with a lab procedure selected.
					comboStatus.Enabled=false;//The status of a lab should not change independent of its parent proc.
					if(Canadian.IsValidForLabEstimates(Plan)) {
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
			ClaimProcInitial=ClaimProcCur.Copy();
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
			foreach(Control controlChild in control.Controls) {
				string result=GetTextValue(controlChild,textBoxName);
				if(result!=null) {
					return result;
				}
			}
			return null;
		}

		private void SetComboStatus(ClaimProcStatus status){
			switch(status){
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
				textFee.Text=proc.ProcFeeTotal.ToString("f");
				InsPlan plan=InsPlans.GetPlan(ClaimProcCur.PlanNum,PlanList);
				long insFeeSchedNum=FeeScheds.GetFeeSched(PatCur,PlanList,PatPlanList,SubList,proc.ProvNum);
				textFeeSched.Text=FeeScheds.GetDescription(insFeeSchedNum);//show ins fee sched, unless PPO plan and standard fee is greater, checked below
				if(plan.PlanType=="p") {//if ppo
					double insFee=Fees.GetAmount0(proc.CodeNum,insFeeSchedNum,proc.ClinicNum,proc.ProvNum);
					long standFeeSchedNum=Providers.GetProv(Patients.GetProvNum(PatCur)).FeeSched;
					double standardfee=Fees.GetAmount0(proc.CodeNum,standFeeSchedNum,proc.ClinicNum,proc.ProvNum);
					if(standardfee>insFee) {//if standard fee is greater than ins fee for a PPO plan, show standard fee sched
						textFeeSched.Text=FeeScheds.GetDescription(standFeeSchedNum);
					}
				}
				string stringProcCode=ProcedureCodes.GetStringProcCode(proc.CodeNum);
				//int codeNum=proc.CodeNum;
				long substCodeNum=proc.CodeNum;
				if(SubstitutionLinks.HasSubstCodeForPlan(plan,proc.CodeNum,ListSubLinks)) {
					substCodeNum=ProcedureCodes.GetSubstituteCodeNum(stringProcCode,proc.ToothNum,plan.PlanNum,ListSubLinks);//for posterior composites
				}
				if(proc.CodeNum!=substCodeNum) {
					textSubstCode.Text=ProcedureCodes.GetStringProcCode(substCodeNum);
				}
				if(plan.PlanType=="p"){//if ppo
					textPPOFeeSched.Text=FeeScheds.GetDescription(plan.FeeSched);
					textAllowedFeeSched.Text="---";
				}
				else{
					textPPOFeeSched.Text="---";
					if(plan.AllowedFeeSched!=0 && !_blueBookEstimateData.IsValidForEstimate(ClaimProcCur,false)) {
						textAllowedFeeSched.Text=FeeScheds.GetDescription(plan.AllowedFeeSched);
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
			FillAllowed(_blueBookEstimateData);
			if(ClaimProcCur.AllowedOverride!=-1){
				textAllowedOverride.Text=ClaimProcCur.AllowedOverride.ToString("f");
			}
			if(ClaimProcCur.CopayAmt!=-1){
				textCopayAmt.Text=ClaimProcCur.CopayAmt.ToString("f");
			}
			if(ClaimProcCur.CopayOverride!=-1){
				textCopayOverride.Text=ClaimProcCur.CopayOverride.ToString("f");
			}
			if(ClaimProcCur.DedEst > 0) {
				textDedEst.Text=ClaimProcCur.DedEst.ToString("f");
			}
			if(ClaimProcCur.DedEstOverride!=-1) {
				textDedEstOverride.Text=ClaimProcCur.DedEstOverride.ToString("f");
			}
			if(ClaimProcCur.Percentage!=-1){
				textPercentage.Text=ClaimProcCur.Percentage.ToString();
			}
			if(ClaimProcCur.PercentOverride!=-1){
				textPercentOverride.Text=ClaimProcCur.PercentOverride.ToString();
			}
			if(ClaimProcCur.PaidOtherIns!=-1){
				textPaidOtherIns.Text=ClaimProcCur.PaidOtherIns.ToString("f");
			}
			if(ClaimProcCur.PaidOtherInsOverride!=-1) {
				textPaidOtherInsOverride.Text=ClaimProcCur.PaidOtherInsOverride.ToString("f");
			}
			textBaseEst.Text=ClaimProcCur.BaseEst.ToString("f");
			if(ClaimProcCur.InsEstTotal!=-1) {
				textInsEstTotal.Text=ClaimProcCur.InsEstTotal.ToString("f");
			}
			if(ClaimProcCur.InsEstTotalOverride!=-1) {
				textInsEstTotalOverride.Text=ClaimProcCur.InsEstTotalOverride.ToString("f");
			}
			if(ClaimProcCur.WriteOffEst!=-1) {
				textWriteOffEst.Text=ClaimProcCur.WriteOffEst.ToString("f");
			}
			if(ClaimProcCur.WriteOffEstOverride!=-1) {
				textWriteOffEstOverride.Text=ClaimProcCur.WriteOffEstOverride.ToString("f");
			}
			textDedApplied.Text=ClaimProcCur.DedApplied.ToString("f");
			textInsPayEst.Text=ClaimProcCur.InsPayEst.ToString("f");
			textInsPayAmt.Text=ClaimProcCur.InsPayAmt.ToString("f");
			textWriteOff.Text=ClaimProcCur.WriteOff.ToString("f");
		}

		///<summary>Fills the carrier allowed amount.  Called from FillInitialAmounts and from butUpdateAllowed_Click</summary>
		private void FillAllowed(BlueBookEstimateData blueBookEstimateData=null){
			if(IsProc){
				decimal allowed=InsPlans.GetAllowedForProc(proc,ClaimProcCur,PlanList,ListSubLinks,null,blueBookEstimateData);
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
			InsPlan plan=InsPlans.GetPlan(ClaimProcCur.PlanNum,PlanList);
			if(plan==null){
				//this should never happen
			}
			if(_blueBookEstimateData.IsValidForEstimate(ClaimProcCur,false)) {
				MsgBox.Show(this,"Allowed fee schedules are not used for Out of Network primary dental insurance plans when the Blue Book feature is on.");
				return;
			}
			if(plan.AllowedFeeSched==0 && plan.PlanType!="p"){
				MsgBox.Show(this,"Plan must either be a PPO type or it must have an 'Allowed' fee schedule set.");
				return;
			}
			long feeSched=-1;
			if(plan.AllowedFeeSched!=0) {
				feeSched=plan.AllowedFeeSched;
			}
			else if(plan.PlanType=="p") {
				//The only other way to manually edit allowed fee schedule amounts is blocked via the Setup permission.
				//We only want to block PPO patients so that we don't partially break Blue Book users.
				if(!Security.IsAuthorized(Permissions.Setup)) {
					return;
				}
				feeSched=plan.FeeSched;
			}
			if(FeeScheds.GetIsHidden(feeSched)){
				MsgBox.Show(this,"Allowed fee schedule is hidden, so no changes can be made.");
				return;
			}
			Fee FeeCur=Fees.GetFee(proc.CodeNum,feeSched,proc.ClinicNum,proc.ProvNum);
			using FormFeeEdit FormFE=new FormFeeEdit();
			if(FeeCur==null) {
				FeeSched feeSchedObj=FeeScheds.GetFirst(x => x.FeeSchedNum==feeSched);
				FeeCur=new Fee();
				FeeCur.FeeSched=feeSched;
				FeeCur.CodeNum=proc.CodeNum;
				FeeCur.ClinicNum=(feeSchedObj.IsGlobal) ? 0 : proc.ClinicNum;
				FeeCur.ProvNum=(feeSchedObj.IsGlobal) ? 0 : proc.ProvNum;
				Fees.Insert(FeeCur);
				//SecurityLog is updated in FormFeeEdit.
				FormFE.IsNew=true;
			}
			DateTime datePrevious=FeeCur.SecDateTEdit;
			//Make an audit entry that the user manually launched the Fee Edit window from this location.
			SecurityLogs.MakeLogEntry(Permissions.ProcFeeEdit,0,Lan.g(this,"Procedure")+": "+ProcedureCodes.GetStringProcCode(FeeCur.CodeNum)
				+", "+Lan.g(this,"Fee")+": "+FeeCur.Amount.ToString("c")+", "+Lan.g(this,"Fee Schedule")+": "+FeeScheds.GetDescription(FeeCur.FeeSched)
				+". "+Lan.g(this,"Manually launched Edit Fee window via Edit Claim Procedure window."),FeeCur.CodeNum,DateTime.MinValue);
			SecurityLogs.MakeLogEntry(Permissions.LogFeeEdit,0,Lan.g(this,"Fee Inserted"),FeeCur.FeeNum,datePrevious);
			FormFE.FeeCur=FeeCur;
			FormFE.ShowDialog();
			//The Fees cache is updated in the closing of FormFeeEdit if there were any changes made.  Simply refresh our window.
			if(FormFE.DialogResult==DialogResult.OK) {
				FillAllowed();
				ComputeAmounts();//?
			}
		}

		private void ComputeAmounts(){
			if(!AllAreValid()){
				return;
			}
			ClaimProcCur.NoBillIns=checkNoBillIns.Checked;
			if(ClaimProcCur.Status==ClaimProcStatus.CapEstimate || ClaimProcCur.Status==ClaimProcStatus.CapComplete) {
				panelEstimateInfo.Visible=true;
				groupClaimInfo.Visible=true;
			}
			else if(checkNoBillIns.Checked) {
				panelEstimateInfo.Visible=false;
				groupClaimInfo.Visible=false;
				return;
			}
			else{
				if(ClaimProcCur.ProcNum!=0){//if a total payment, then this protects panel from inadvertently
						//being set visible again.  All other situations, it's based on NoBillIns
					panelEstimateInfo.Visible=true;
				}
				if(ClaimProcCur.ClaimNum>0) {//attached to claim
					groupClaimInfo.Visible=true;
				}
				else {
					groupClaimInfo.Visible=false;
				}
			}
			if(textAllowedOverride.Text=="") {
				ClaimProcCur.AllowedOverride=-1;
			}
			else {
				ClaimProcCur.AllowedOverride=PIn.Double(textAllowedOverride.Text);
			}
			if(textCopayOverride.Text=="") {
				ClaimProcCur.CopayOverride=-1;
			}
			else {
				ClaimProcCur.CopayOverride=PIn.Double(textCopayOverride.Text);
			}
			if(textDedEstOverride.Text=="") {
				ClaimProcCur.DedEstOverride=-1;
			}
			else {
				ClaimProcCur.DedEstOverride=PIn.Double(textDedEstOverride.Text);
			}
			if(textPercentOverride.Text=="") {
				ClaimProcCur.PercentOverride=-1;
			}
			else {
				ClaimProcCur.PercentOverride=PIn.Int(textPercentOverride.Text);
			}
			if(textPaidOtherInsOverride.Text=="") {
				ClaimProcCur.PaidOtherInsOverride=-1;
			}
			else {
				ClaimProcCur.PaidOtherInsOverride=PIn.Double(textPaidOtherInsOverride.Text);
			}
			if(textInsEstTotalOverride.Text=="") {
				ClaimProcCur.InsEstTotalOverride=-1;
			}
			else {
				ClaimProcCur.InsEstTotalOverride=PIn.Double(textInsEstTotalOverride.Text);
			}
			if(textWriteOffEstOverride.Text=="") {
				ClaimProcCur.WriteOffEstOverride=-1;
			}
			else {
				ClaimProcCur.WriteOffEstOverride=PIn.Double(textWriteOffEstOverride.Text);
			}
			if(IsProc) {
				//doCheckCanadianLabs is false because we are simply making in memory changs to ClaimProcCur.
				//If ClaimProcCur.Status was changed then the inner CanadianLabBaseEstHelper(...) call would insert new lab claimProc rows.
				//We currently do not use the lab claimProcs in any way in this window so we only need to worry about update the statuses
				//on an OK click when commiting ClaimProcCur changes to DB.
				ClaimProcs.ComputeBaseEst(ClaimProcCur,proc,Plan,PatPlanNum,BenefitList,
					HistList,LoopList,PatPlanList,PaidOtherInsTotal,PaidOtherInsBaseEst,PatCur.Age,WriteOffOtherIns,PlanList,SubList,ListSubLinks,false,null,
					doCheckCanadianLabs:false,blueBookEstimateData:_blueBookEstimateData);
				//Paid other ins is not accurate
			}
			//else {
			//	ClaimProcs.ComputeBaseEst(ClaimProcCur,0,"",0,Plan,PatPlanNum,BenefitList,HistList,LoopList);
			//}
			if(ClaimProcCur.CopayAmt == -1) {
				textCopayAmt.Text="";
			}
			else {
				textCopayAmt.Text=ClaimProcCur.CopayAmt.ToString("f");
			}
			if(ClaimProcCur.DedEst == -1) {
				textDedEst.Text="";
			}
			else {
				textDedEst.Text=ClaimProcCur.DedEst.ToString("f");
			}
			if(ClaimProcCur.Percentage == -1) {
				textPercentage.Text="";
			}
			else {
				textPercentage.Text=ClaimProcCur.Percentage.ToString("f0");
			}
			if(ClaimProcCur.PaidOtherIns == -1) {
				textPaidOtherIns.Text="";
			}
			else {
				textPaidOtherIns.Text=ClaimProcCur.PaidOtherIns.ToString("f");
			}
			textBaseEst.Text=ClaimProcCur.BaseEst.ToString("f");
			textInsEstTotal.Text=ClaimProcCur.InsEstTotal.ToString("f");
			if(ClaimProcCur.WriteOffEst==-1) {
				textWriteOffEst.Text="";
			}
			else {
				textWriteOffEst.Text=ClaimProcCur.WriteOffEst.ToString("f");
			}
			if(IsProc) {
				//Compute the patient portion ignorant of other claimprocs and adjustments to preserve old behavior.
				textPatPortion1.Text=ClaimProcs.GetPatPortion(proc,new List<ClaimProc>() { ClaimProcCur }).ToString("f");
			}
			textEstimateNote.Text=ClaimProcCur.EstimateNote;
			//insurance box---------------------------------------------------------------
			if(groupClaimInfo.Visible){
				ClaimProcCur.DedApplied=PIn.Double(textDedApplied.Text);
				ClaimProcCur.InsPayEst=PIn.Double(textInsPayEst.Text);
				ClaimProcCur.InsPayAmt=PIn.Double(textInsPayAmt.Text);
				ClaimProcCur.WriteOff=PIn.Double(textWriteOff.Text);
				if(IsProc) {
					//Compute the patient portion ignorant of other claimprocs and adjustments to preserve old behavior.
					textPatPortion2.Text=ClaimProcs.GetPatPortion(proc,new List<ClaimProc>() { ClaimProcCur }).ToString("f");
					labelPatPortion1.Visible=false;
					textPatPortion1.Visible=false;
				}
			}
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			using FormProviderPick formp=new FormProviderPick(_listProviders);
			if(comboProvider.SelectedIndex > -1) {
				formp.SelectedProvNum=_listProviders[comboProvider.SelectedIndex].ProvNum;
			}
			formp.ShowDialog();
			if(formp.DialogResult!=DialogResult.OK) {
				return;
			}
			//Set the combo box to the ODBoxItem that contains the provider that was just selected.
			//If we can't find it, reselect the same item that was already selected.
			comboProvider.SetSelectedKey<Provider>(formp.SelectedProvNum, x => x.ProvNum);
		}

		private void comboStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			//new selected index will already be set
			if(!ListTools.In(ClaimProcOld.Status,ClaimProcStatus.Estimate,ClaimProcStatus.InsHist)//not an estimate or inshist
				&& ListTools.In(comboStatus.SelectedIndex,0,8))//and clicked on estimate or insHist
			{
				SetComboStatus(ClaimProcOld.Status);//no change
				return;
			}
			if(ClaimProcOld.Status==ClaimProcStatus.Supplemental) {
				//Get all non-supplemental claimprocs. If there is at least one, do not let them change this status if this a supplemental claim.
				//Prevents creating two received claimprocs for the same procedure and insurance. If there is not one, allow them to change this status
				//so they don't get stuck with a supplemental claimproc and no way to create one of a different type.
				List<ClaimProc> listClaimProcsForClaim=ClaimProcs.RefreshForClaim(ClaimProcOld.ClaimNum)
					.Where(x => x.ClaimProcNum!=ClaimProcOld.ClaimProcNum 
					&& x.Status!=ClaimProcStatus.Supplemental 
					&& x.ProcNum==ClaimProcOld.ProcNum).ToList();
				if(!listClaimProcsForClaim.IsNullOrEmpty()) {
					MsgBox.Show(this,"Cannot change the status of a supplemental claim procedure when there is at least one claim procedure of a different"
						+" status. There should be a maximum of one claim procedure of status received for each procedure in the claim.");
					SetComboStatus(ClaimProcStatus.Supplemental);
					return;
				}
			}
			#region Capitation Claim Attached
			if(Plan.PlanType=="c" && ClaimProcOld.ClaimNum > 0 && comboStatus.SelectedIndex!=5) {
				MsgBox.Show(this,"A capitation insurance plan is associated with this claim procedure.\r\n"
					+"This claim procedure is currently part of a claim.\r\n"
					+"CapClaim is the only valid status for this scenario.");
				ClaimProcCur.Status=ClaimProcStatus.CapClaim;
				SetComboStatus(ClaimProcCur.Status);//Force CapClaim status.
				return;
			}
			#endregion
			#region Claim Payment Attached
			if(ClaimProcOld.ClaimPaymentNum > 0//Attached to a payment
				&& Plan.PlanType!="c"//Is a category percentage plan, or PPO percentage plan, or a flat co-pay plan.
				&& comboStatus.SelectedIndex!=2//User did not select Received
				&& comboStatus.SelectedIndex!=4)//User did not select Supplemental
			{
				if(Plan.PlanType==""){
					MsgBox.Show(this,"This claim procedure is attached to an insurance payment.\r\n"
						+"Since the insurance plan is a category percentage plan,\r\n"
						+"you may only set the status to Received or Supplemental.");
				}
				else if(Plan.PlanType=="p"){
					MsgBox.Show(this,"This claim procedure is attached to an insurance payment.\r\n"
						+"Since the insurance plan is a PPO percentage plan,\r\n"
						+"you may only set the status to Received or Supplemental.");
				}
				else if(Plan.PlanType=="f"){
					MsgBox.Show(this,"This claim procedure is attached to an insurance payment.\r\n"
						+"Since the insurance plan is a flat co-pay plan,\r\n"
						+"you may only set the status to Received or Supplemental.");
				}
				SetComboStatus(ClaimProcCur.Status);//Go back to previous selection.
				return;
			}
			#endregion
			#region Insurance Plan Attached
			bool isValidPlanType=true;
			switch(comboStatus.SelectedIndex) {
				case 0:
					if(Plan.PlanType=="c") {
						isValidPlanType=false;
						break;
					}
					ClaimProcCur.Status=ClaimProcStatus.Estimate;
					break;
				case 1:
					if(Plan.PlanType=="c") {
						isValidPlanType=false;
						break;
					}
					ClaimProcCur.Status=ClaimProcStatus.NotReceived;
					break;
				case 2:
					if(Plan.PlanType=="c") {
						isValidPlanType=false;
						break;
					}
					ClaimProcCur.Status=ClaimProcStatus.Received;
					break;
				case 3:
					ClaimProcCur.Status=ClaimProcStatus.Preauth;
					break;
				case 4:
					if(Plan.PlanType=="c") {
						isValidPlanType=false;
						break;
					}
					ClaimProcCur.Status=ClaimProcStatus.Supplemental;
					break;
				case 5:
					if(Plan.PlanType!="c") {
						isValidPlanType=false;
						break;
					}
					ClaimProcCur.Status=ClaimProcStatus.CapClaim;
					break;
				case 6:
					if(Plan.PlanType!="c") {
						isValidPlanType=false;
						break;
					}
					ClaimProcCur.Status=ClaimProcStatus.CapEstimate;
					break;
				case 7:
					if(Plan.PlanType!="c") {
						isValidPlanType=false;
						break;
					}
					ClaimProcCur.Status=ClaimProcStatus.CapComplete;
					//Capitation procedures are not usually attached to a claim.
					//In order for Aging to calculate properly the ProcDate (Date Completed) and DateCP (Payment Date) must be the same.
					ClaimProcCur.DateCP=proc.ProcDate;
					break;
				case 8:
					if(Plan.PlanType=="c") {
						isValidPlanType=false;
						break;
					}
					ClaimProcCur.Status=ClaimProcStatus.InsHist;
					break;
			}
			if(!isValidPlanType) {
				if(Plan.PlanType=="") {
					MsgBox.Show(this,"A category percentage insurance plan is associated with this claim procedure.\r\n"
						+"You may only select statuses which are related to category percentage,\r\n"
						+"including Estimate, NotReceived, Received, Supplemental, and PreAuthorization.\r\n"
						+"To change the status to a different option, you must change the plan type.");
				}
				else if(Plan.PlanType=="p") {
					MsgBox.Show(this,"A PPO percentage insurance plan is associated with this claim procedure.\r\n"
						+"You may only select statuses which are related to PPO percentage,\r\n"
						+"including Estimate, NotReceived, Received, Supplemental, and PreAuthorization.\r\n"
						+"To change the status to a different option, you must change the plan type.");
				}
				else if(Plan.PlanType=="f") {
					MsgBox.Show(this,"A flat co-pay insurance plan is associated with this claim procedure.\r\n"
						+"You may only select statuses which are related to flat co-pay insurance,\r\n"
						+"including Estimate, NotReceived, Received, Supplemental, and PreAuthorization.\r\n"
						+"To change the status to a different option, you must change the plan type.");
				}
				else if(Plan.PlanType=="c") {
					MsgBox.Show(this,"A capitation insurance plan is associated with this claim procedure.\r\n"
						+"You may only select statuses which are related to capitation insurance,\r\n"
						+"including CapClaim, CapEstimate, CapComplete, and PreAuthorization.\r\n"
						+"To change the status to a different option, you must change the plan type.");
				}
				SetComboStatus(ClaimProcCur.Status);//Go back to previous selection.
			}
			#endregion
			if(ClaimProcCur.Status==ClaimProcStatus.Received || ClaimProcCur.Status==ClaimProcStatus.Supplemental) {
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
			if(ClaimProcCur.Status!=ClaimProcStatus.CapEstimate
				&& ClaimProcCur.Status!=ClaimProcStatus.CapComplete){
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
			double writeoff=proc.ProcFee-copay;
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
			if(CompareDouble.IsZero(ClaimProcCur.InsPayAmt)) { 
				if(!Security.IsAuthorized(Permissions.InsPayCreate)) { //user not allowed to create an insurance payment
					return;
				}
			}
			else {
				if(!Security.IsAuthorized(Permissions.InsPayEdit,ClaimProcCur.DateCP)) { //user not allowed to edit an insurance payment
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
			DateTime writeOffSecDate=ClaimProcCur.SecDateEntry;
			if(IsProc && proc.ProcDate!=DateTime.MinValue) {
				writeOffSecDate=proc.DateEntryC;
			}
			if(!Security.IsAuthorized(Permissions.InsWriteOffEdit,writeOffSecDate)) {
				return;
			}
		}

		private void textWriteOffEstOverride_Enter(object sender,EventArgs e) {
			if(!textWriteOffEstOverride.ReadOnly) {//In this window if the box is readonly when the user clicks into it show them the security warning that disabled it.
				return;
			}
			DateTime writeOffSecDate=ClaimProcCur.SecDateEntry;
			if(IsProc && proc.ProcDate!=DateTime.MinValue) {
				writeOffSecDate=proc.DateEntryC;
			}
			if(!Security.IsAuthorized(Permissions.InsWriteOffEdit,writeOffSecDate)) {
				return;
			}
		}

		private void checkPayPlan_Click(object sender,EventArgs e) {
			if(checkPayPlan.Checked) {
				List<PayPlan> payPlanList=PayPlans.GetValidInsPayPlans(ClaimProcCur.PatNum,ClaimProcCur.PlanNum,ClaimProcCur.InsSubNum,ClaimProcCur.ClaimNum);
				if(payPlanList.Count==0) {//no valid plans
					MsgBox.Show(this,"The patient does not have a valid payment plan with this insurance plan attached that has not been paid in full and is not tracking expected payments for an existing claim already.");
					checkPayPlan.Checked=false;
					return;
				}
				if(payPlanList.Count==1) { //if there is only one valid payplan
					ClaimProcCur.PayPlanNum=payPlanList[0].PayPlanNum;
					return;
				}
				//more than one valid PayPlan
				using FormPayPlanSelect FormPPS=new FormPayPlanSelect(payPlanList);
				FormPPS.ShowDialog();
				if(FormPPS.DialogResult==DialogResult.Cancel) {
					checkPayPlan.Checked=false;
					return;
				}
				ClaimProcCur.PayPlanNum=FormPPS.SelectedPayPlanNum;
			}
			else {//payPlan unchecked
				ClaimProcCur.PayPlanNum=0;
			}
		}

		///<summary>Claimprocs with various statuses can be deleted,
		///except certain specific scenarios where the user does not have permission (multiple different permissions are considered).</summary>
		private void butDelete_Click(object sender, System.EventArgs e) {
			if(ClaimProcCur.IsTransfer) {
				if(MessageBox.Show(Lan.g(this,"This Claim Procedure is part of an income transfer."+"\r\n"
					+"Deleting this claim procedure will delete all of the income transfers for this claim.  Continue?"),""
					,MessageBoxButtons.OKCancel)!=DialogResult.OK)	
				{
					return;
				}
			}
			else if(CultureInfo.CurrentCulture.Name.EndsWith("CA") 
				&& ClaimProcCur.ProcNum!=0//not a 'Total Payment' row
				&& ClaimProcCur.ProcNum==proc.ProcNum && proc.ProcNumLab==0)//not a lab
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
				ClaimProcs.DeleteAfterValidating(ClaimProcCur);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			ClaimProcs.RemoveSupplementalTransfersForClaims(ClaimProcCur.ClaimNum);
			ClaimProcCur.DoDelete=true;
			IsSaved=false;
			DialogResult=DialogResult.OK;
		}

		/// <summary>Returns true if ClaimProcAllowCreditsGreaterThanProcFee preference allows the user to add credits greater than the proc fee. Otherwise returns false </summary>
		private bool IsClaimProcGreaterThanProcFee() {
			if(!IsProc) {
				return true;
			}
			ClaimProcCreditsGreaterThanProcFee creditsGreaterPref=(ClaimProcCreditsGreaterThanProcFee)PrefC.GetInt(PrefName.ClaimProcAllowCreditsGreaterThanProcFee);
			if(creditsGreaterPref==ClaimProcCreditsGreaterThanProcFee.Allow) {
				return true;
			}
			List<ClaimProc> listClaimProcsForPat=ClaimProcs.Refresh(PatCur.PatNum);
			List<PaySplit> listPaySplitForSelectedCP= PaySplits.GetPaySplitsFromProcs(new List<long> { proc.ProcNum });
			List<Adjustment> listAdjForSelectedCP=Adjustments.GetForProcs(new List<long> { proc.ProcNum });
			decimal insPayAmt=(decimal)ClaimProcs.ProcInsPay(listClaimProcsForPat.FindAll(x => x.ClaimProcNum!=ClaimProcCur.ClaimProcNum),proc.ProcNum)
				+PIn.Decimal(textInsPayAmt.Text);
			decimal writeOff=(decimal)ClaimProcs.ProcWriteoff(listClaimProcsForPat.FindAll(x => x.ClaimProcNum!=ClaimProcCur.ClaimProcNum),proc.ProcNum)
				+PIn.Decimal(textWriteOff.Text);
			decimal feeAcct=(decimal)proc.ProcFeeTotal;
			decimal creditRem=0;
			decimal adj=listAdjForSelectedCP.Select(x => (decimal)x.AdjAmt).Sum();
			decimal patPayAmt=listPaySplitForSelectedCP.Select(x => (decimal)x.SplitAmt).Sum();
			//Any changes to this calculation should also consider FormClaimPayTotal.IsClaimProcGreaterThanProcFee().
			creditRem=feeAcct-patPayAmt-insPayAmt-writeOff+adj;
			bool isCreditGreater=CompareDecimal.IsLessThanZero(creditRem);
			string procDescript=ProcedureCodes.GetProcCode(proc.CodeNum).ProcCode
				+"\t"+Lan.g(this,"Fee")+": "+feeAcct.ToString("F")
				+"\t"+Lan.g(this,"Credits")+": "+Math.Abs((-patPayAmt-insPayAmt-writeOff+adj)).ToString("F")
				+"\t"+Lan.g(this,"Remaining")+": ("+Math.Abs(creditRem).ToString("F")+")";
			if(!isCreditGreater) {
				return true;
			}
			if(creditsGreaterPref==ClaimProcCreditsGreaterThanProcFee.Block) {
				MessageBox.Show(this,Lan.g(this,"Remaining amount is negative")+":\r\n"+procDescript+"\r\n"+Lan.g(this,"Not allowed to continue."),
					Lan.g(this,"Overpaid Procedure Warning"));
				return false;
			}
			if(creditsGreaterPref==ClaimProcCreditsGreaterThanProcFee.Warn) {
				return MessageBox.Show(this,Lan.g(this,"Remaining amount is negative")+":\r\n"+procDescript+"\r\n"+Lan.g(this,"Continue?"),
					Lan.g(this,"Overpaid Procedure Warning"),MessageBoxButtons.YesNo)==DialogResult.Yes;
			}
			return true;//should never get to this line, only possible if another enum value is added to allow, warn, and block
		}

		///<summary>Returns true if InsPayNoWriteoffMoreThanProc preference is turned on and the sum of write off amount is greater than the proc fee.
		///Otherwise returns false </summary>
		private bool IsWriteOffGreaterThanProcFee() {
			if(!IsProc || !PrefC.GetBool(PrefName.InsPayNoWriteoffMoreThanProc)) {
				return false;
			}
			List<ClaimProc> listClaimProcsForPat=ClaimProcs.Refresh(PatCur.PatNum);
			decimal writeOff=(decimal)ClaimProcs.ProcWriteoff(listClaimProcsForPat.FindAll(x => x.ClaimProcNum!=ClaimProcCur.ClaimProcNum),proc.ProcNum)
				+PIn.Decimal(textWriteOff.Text);
			decimal feeAcct=(decimal)proc.ProcFeeTotal;
			decimal adjAcct=Adjustments.GetForProcs(new List<long> { proc.ProcNum }).Sum(x => (decimal)x.AdjAmt);
			decimal writeoffRem=0;
			//Any changes to this calculation should also consider FormClaimPayTotal.IsWriteoffGreaterThanProcFee().
			writeoffRem=feeAcct-writeOff+adjAcct;
			bool isWriteoffGreater=CompareDecimal.IsLessThanZero(writeoffRem) && CompareDecimal.IsGreaterThanZero(writeOff);
			string procDescript=Lan.g(this,"Fee")+": "+feeAcct.ToString("F")
				+"\t"+Lan.g(this,"Adjustments")+": "+adjAcct.ToString("F")
				+"\t"+Lan.g(this,"Write-off")+": "+Math.Abs((-writeOff)).ToString("F")
				+"\t"+Lan.g(this,"Remaining")+": ("+Math.Abs(writeoffRem).ToString("F")+")";
			if(isWriteoffGreater) {
				MessageBox.Show(this,Lan.g(this,"Write-off amount is greater than the adjusted procedure fee")+":\r\n"+procDescript+"\r\n"
					+Lan.g(this,"Not allowed to continue."),Lan.g(this,"Excessive Write-off"));
				return true;
			}
			return false;
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
			List<InsBlueBookLog> listInsBlueBookLogs=InsBlueBookLogs.GetAllByClaimProcNum(ClaimProcCur.ClaimProcNum);
			InsBlueBookLog blueBookLog=_blueBookEstimateData.CreateInsBlueBookLog(ClaimProcCur,false);
			if(blueBookLog!=null) {
				//Create a placeholder log entry for what the user should expect when the OK button is clicked.
				blueBookLog.DateTEntry=DateTime.Now;
				listInsBlueBookLogs.Add(blueBookLog);
			}
			if(listInsBlueBookLogs.Count==0) {
				MsgBox.Show(this,"This Claim Procedure has no Blue Book Log history.");
				return;
			}
			using FormClaimProcBlueBookLog formClaimProcBlueBookLog=new FormClaimProcBlueBookLog(listInsBlueBookLogs);
			formClaimProcBlueBookLog.ShowDialog();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			//no security check here because if attached to a payment, nobody is allowed to change the date or amount anyway.
			if(!AllAreValid()){
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			if(PIn.Date(textDateCP.Text).Date > DateTime.Today.Date
				&& !PrefC.GetBool(PrefName.FutureTransDatesAllowed) 
				&& !PrefC.GetBool(PrefName.AllowFutureInsPayments)
				&& ListTools.In(ClaimProcCur.Status,ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.CapClaim,ClaimProcStatus.CapComplete)) 
			{ 
				MsgBox.Show(this,"Payment date cannot be for the future.");
				return;
			}
			if(ClaimProcCur.WriteOff<0 && ClaimProcCur.Status!=ClaimProcStatus.Supplemental) {
				MsgBox.Show(this,"Only supplemental payments may have a negative WriteOff amount.");
				return;
			}
			double claimWriteOffTotal=ClaimProcs.GetClaimWriteOffTotal(ClaimProcCur.ClaimNum,ClaimProcCur.ProcNum,new List<ClaimProc>() { ClaimProcCur });
			if(claimWriteOffTotal+ClaimProcCur.WriteOff<0) {
				MsgBox.Show(this,"The current writeoff value will cause the procedure's total writeoff to be negative.  Please change it to at least "+(ClaimProcCur.WriteOff-(claimWriteOffTotal+ClaimProcCur.WriteOff)).ToString()+" to continue.");
				return;
			}
			if(IsWriteOffGreaterThanProcFee()) {
				return;
			}
			if(!IsClaimProcGreaterThanProcFee()) {
				return;
			}
			if(ListTools.In(ClaimProcCur.Status,ClaimProcStatus.Received,ClaimProcStatus.Supplemental)
				&& !Security.IsAuthorized(Permissions.InsPayEdit,PIn.Date(textDateCP.Text))) {
				return;
			}
			if(OrthoProcLinks.IsProcLinked(ClaimProcCur.ProcNum) && 
				(ClaimProcOld.AllowedOverride!=PIn.Double(textAllowedOverride.Text)
				|| !ClaimProcOld.CopayOverride.Equals(PIn.Double(textCopayOverride.Text))
				|| !ClaimProcOld.DedEstOverride.Equals(PIn.Double(textDedEstOverride.Text))
				|| !ClaimProcOld.PercentOverride.Equals(PIn.Int(textPercentOverride.Text))
				|| !ClaimProcOld.PaidOtherInsOverride.Equals(PIn.Double(textPaidOtherInsOverride.Text))
				|| !ClaimProcOld.InsEstTotalOverride.Equals(PIn.Double(textInsEstTotalOverride.Text))
				|| !ClaimProcOld.WriteOffEstOverride.Equals(PIn.Double(textWriteOffEstOverride.Text))
				|| !ClaimProcOld.InsPayEst.Equals(PIn.Double(textInsPayEst.Text))
				)) 
			{
				MsgBox.Show(this,"Cannot edit estimate information for procedures attached to ortho cases.");
				return;
			}
			//status already handled
			if(comboProvider.SelectedIndex!=-1) {//if no prov selected, then that prov must simply be hidden,
				//because all claimprocs are initially created with a prov(except preauth).
				//So, in this case, don't change.
				ClaimProcCur.ProvNum=_listProviders[comboProvider.SelectedIndex].ProvNum;
			}
			ClaimProcCur.ProcDate=PIn.Date(textProcDate.Text);
			if(!textDateCP.ReadOnly){
				ClaimProcCur.DateCP=PIn.Date(textDateCP.Text);
			}
			ClaimProcCur.CodeSent=textCodeSent.Text;
			ClaimProcCur.FeeBilled=PIn.Double(textFeeBilled.Text);
			ClaimProcCur.Remarks=textRemarks.Text;
			//if status was changed to received, then set DateEntry
			if(ClaimProcOld.Status!=ClaimProcStatus.Received && ClaimProcOld.Status!=ClaimProcStatus.Supplemental){
				if(ClaimProcCur.Status==ClaimProcStatus.Received || ClaimProcOld.Status==ClaimProcStatus.Supplemental){
					ClaimProcCur.DateEntry=DateTime.Now;
				}
			}
			ClaimProcCur.ClaimPaymentTracking=comboPayTracker.SelectedIndex==0 ? 0 : _listPayTrackDefs[comboPayTracker.SelectedIndex-1].DefNum;
			if(SaveToDb) {
				//Fix pre-auth statuses.
				Claim curClaim=Claims.GetClaim(ClaimProcCur.ClaimNum);
				if(curClaim?.ClaimType=="PreAuth" && ClaimProcCur.Status!=ClaimProcStatus.Preauth) {
						ClaimProcCur.Status=ClaimProcStatus.Preauth;//change the status to preauth.
						MsgBox.Show(this,"Status of procedure was changed back to preauth to match status of claim.");
				}
				InsBlueBookLog blueBookLog=_blueBookEstimateData.CreateInsBlueBookLog(ClaimProcCur);
				if(blueBookLog!=null) {
					InsBlueBookLogs.Insert(blueBookLog);
				}
				ClaimProcs.Update(ClaimProcCur,ClaimProcOld);
				if(ClaimProcCrud.UpdateComparison(ClaimProcCur,ClaimProcOld)) {
					ClaimProcs.RemoveSupplementalTransfersForClaims(ClaimProcCur.ClaimNum);
				}
				if(ClaimProcCur.Status!=ClaimProcOld.Status && IsProc) {
					//We must update the DB such that any associated Canadian labs have the same status as their parent claimproc.
					//If we do not do this then ClaimProcs.CanadianLabBaseEstHelper(...) will fail to match and will not update any existing lab claimpros either.
					//Instead it would insert an new lab claim proc with the parents new claim procs status.
					ClaimProcs.UpdatePertinentLabStatuses(ClaimProcCur,Plan);//Checks for Canada, simply returns if not.
				}
			}//otherwise, the change to db will be made by calling class
			//there is no functionality here for insert cur, because all claimprocs are
			//created before editing.
			if(ClaimProcCur.ClaimPaymentNum>0){//attached to ins check
				//note: the amount and the date will not have been changed.
				SecurityLogs.MakeLogEntry(Permissions.InsPayEdit,ClaimProcCur.PatNum,
					Patients.GetLim(ClaimProcCur.PatNum).GetNameLF()+", "
					+Lan.g(this,"Date and amount not changed."));//I'm really not sure what they would have changed.
			}
			if(ListTools.In(ClaimProcCur.Status,ClaimProcStatus.Received,ClaimProcStatus.NotReceived,ClaimProcStatus.CapClaim)
				&& ClaimProcCur.ProvNum != ClaimProcOld.ProvNum) 
			{
				string strSecLog;
				if(proc == null) {
					strSecLog = "Total Payment for "+textInsPlan.Text+". "+Lan.g(this,"Provider changed from")+" "
					+Providers.GetAbbr(ClaimProcOld.ProvNum)+" "+Lan.g(this,"to")+" "+Providers.GetAbbr(ClaimProcCur.ProvNum);
				}
				else {
					strSecLog = ProcedureCodes.GetProcCode(proc.CodeNum).ProcCode+" - "+textInsPlan.Text+". "+Lan.g(this,"Provider changed from")+" "
					+Providers.GetAbbr(ClaimProcOld.ProvNum)+" "+Lan.g(this,"to")+" "+Providers.GetAbbr(ClaimProcCur.ProvNum);
				}
				SecurityLogs.MakeLogEntry(Permissions.ClaimProcClaimAttachedProvEdit,ClaimProcCur.PatNum,strSecLog);
			}
			IsSaved=true;
			MakeAuditTrailEntries();
			DialogResult=DialogResult.OK;
		}

		private void MakeAuditTrailEntries() {
			string insWriteOffEditLog="";
			string insPayEditLog="";
			if(proc!=null && proc.CodeNum!=0) {//Could happen with pay "As Total". Still want to log, just without procedure.
				string strProcCode=ProcedureCodes.GetStringProcCode(proc.CodeNum);
				insWriteOffEditLog=$"Procedure: {strProcCode}. ";
				insPayEditLog=$"Procedure: {strProcCode}. ";
			}
			bool needsEditLog=false;
			if(ClaimProcOld.WriteOff!=ClaimProcCur.WriteOff) {
				insWriteOffEditLog+=$"Write off amount changed from {ClaimProcOld.WriteOff.ToString("C")} to {ClaimProcCur.WriteOff.ToString("C")}. ";
				needsEditLog=true;
			}
			double writeOffEstOld=ClaimProcs.GetWriteOffEstimate(ClaimProcOld);//WriteOffEst is never user editable, we have to check overrides
			double writeOffEstCur=ClaimProcs.GetWriteOffEstimate(ClaimProcCur);//WriteOffEst is never user editable, we have to check overrides
			if(writeOffEstOld!=writeOffEstCur) {
				insWriteOffEditLog+=$"Write off estimate amount changed from {writeOffEstOld.ToString("C")} to {writeOffEstCur.ToString("C")}. ";
				needsEditLog=true;
			}
			if(needsEditLog) {
				SecurityLogs.MakeLogEntry(Permissions.InsWriteOffEdit,ClaimProcCur.PatNum,insWriteOffEditLog);
			}
			needsEditLog=false;
			if(ClaimProcOld.InsPayAmt!=ClaimProcCur.InsPayAmt) {
				insPayEditLog+=$"Insurance payment amount changed from {ClaimProcOld.InsPayAmt.ToString("C")} to "
					+$"{ClaimProcCur.InsPayAmt.ToString("C")}. ";
				needsEditLog=true;
			}
			if(ClaimProcOld.InsPayEst!=ClaimProcCur.InsPayEst) {
				insPayEditLog+=$"Insurance payment estimate amount changed from {ClaimProcOld.InsPayEst.ToString("C")} to "
					+$"{ClaimProcCur.InsPayEst.ToString("C")}. ";
				needsEditLog=true;
			}
			if(!needsEditLog && IsCalledFromClaimEdit) {
				string insPayCreateLog="";
				if(proc!=null && proc.CodeNum!=0) { 
					insPayCreateLog+=$"Procedure: {ProcedureCodes.GetStringProcCode(proc.CodeNum)}. ";
				}
				insPayCreateLog+=$"Insurance payment amount {ClaimProcCur.InsPayAmt.ToString("C")}. ";
				insPayCreateLog+=$"Insurance estimate amount {ClaimProcCur.InsPayEst.ToString("C")}. ";
				SecurityLogs.MakeLogEntry(Permissions.InsPayCreate,ClaimProcCur.PatNum,insPayCreateLog);
			} 
			else if(needsEditLog && IsCalledFromClaimEdit) {
				SecurityLogs.MakeLogEntry(Permissions.InsPayCreate,ClaimProcCur.PatNum,insPayEditLog);
			}
			else if(needsEditLog && !IsCalledFromClaimEdit){
				SecurityLogs.MakeLogEntry(Permissions.InsPayEdit,ClaimProcCur.PatNum,insPayEditLog);
			}
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			IsSaved=false;
			DialogResult=DialogResult.Cancel;
		}

		private void FormClaimProc_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			
		}

		private void FormClaimProc_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK){
				InsBlueBooks.SynchForClaimNums(ClaimProcCur.ClaimNum);
				return;
			}
			ClaimProcCur=ClaimProcOld.Copy();//revert back to the old ClaimProc.  Only important if not SaveToDb
		}
	}
}

















