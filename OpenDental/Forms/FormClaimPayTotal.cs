using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>Summary description for FormClaimPayTotal.</summary>
	public partial class FormClaimPayTotal : FormODBase {
		private List<Procedure> _listProcedures;
		private Patient _patient;
		private Family _family;
		private List<InsPlan> _listInsPlans;
		private List<PatPlan> _listPatPlans;
		private List<InsSub> _listInsSubs;
		private List<Def> _listDefs;
		private List<ClaimProc> _listClaimProcsOld;
		///<summary>True if the user has permission to edit WriteOffs based on the minimum proc.DateEntryC of the procedures to which the claimprocs
		///in the ClaimProcsToEdit array are attached.</summary>
		private bool _isWriteOffEditable;
		///<summary>True if the Pat Resp column is showing in gridMain.</summary>
		private bool _doShowPatResp;
		///<summary>Holds all data needed to make blue book estimates.</summary>
		private BlueBookEstimateData _blueBookEstimateData;
		/// <summary>The amount of money to allocate coming from a Pay As Total payment. </summary>
		private double _totalPayAmt;
		///<summary></summary>
		public ClaimProc[] ClaimProcArray;
		///<summary>Is only set when called from FormClaimEdit and to signify the recieving of a claim.</summary>
		public bool IsCalledFromClaimEdit=false;

		///<summary></summary>
		public FormClaimPayTotal(Patient patient,Family family,List <InsPlan> listInsPlans,List<PatPlan> listPatPlans,List<InsSub> listInsSubs
			,BlueBookEstimateData blueBookEstimateData,double totalPayAmt=0)
		{
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			_family=family;
			_patient=patient;
			_listInsPlans=listInsPlans;
			_listInsSubs=listInsSubs;
			_listPatPlans=listPatPlans;
			_blueBookEstimateData=blueBookEstimateData;
			_totalPayAmt=totalPayAmt;
			Lan.F(this);
		}

		private void FormClaimPayTotal_Load(object sender,EventArgs e) {
			_listClaimProcsOld=new List<ClaimProc>();
			for(int i=0;i<ClaimProcArray.Length;i++) {
				_listClaimProcsOld.Add(ClaimProcArray[i].Copy());
			}
			_listProcedures=Procedures.Refresh(_patient.PatNum);
			_isWriteOffEditable=Security.IsAuthorized(Permissions.InsWriteOffEdit,
				_listProcedures.FindAll(x => ClaimProcArray.Any(y => y.ProcNum==x.ProcNum)).Select(x => x.DateEntryC).Min());
			butWriteOff.Enabled=_isWriteOffEditable;
			_doShowPatResp=PrefC.GetEnum<YN>(PrefName.ClaimEditShowPatResponsibility)==YN.Yes
				|| PrefC.GetEnum<YN>(PrefName.ClaimEditShowPatResponsibility)==YN.Unknown && !PrefC.GetYN(PrefName.ClaimEditShowPayTracking);
			textPatResp.Visible=_doShowPatResp;
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				textLabFees.Visible=false;
				LayoutManager.MoveLocation(textDedApplied,textLabFees.Location);
				LayoutManager.MoveLocation(textInsPayAllowed,new Point(textDedApplied.Right-1,textInsPayAllowed.Location.Y));
				LayoutManager.MoveLocation(textInsPayAmt,new Point(textInsPayAllowed.Right-1,textInsPayAllowed.Location.Y));
				LayoutManager.MoveLocation(textWriteOff,new Point(textInsPayAmt.Right-1,textInsPayAllowed.Location.Y));
				if(_doShowPatResp) {
					LayoutManager.MoveLocation(textPatResp,new Point(textWriteOff.Right-1,textInsPayAllowed.Location.Y));
				}
			}
			_listDefs=Defs.GetDefsForCategory(DefCat.ClaimPaymentTracking,true);
			if(_totalPayAmt>0) {
				ApplyAsTotalPayment();
			}
			FillGrid();
		}

		private void FormClaimPayTotal_Shown(object sender,EventArgs e) {
			InsPlan insPlan=InsPlans.GetPlan(ClaimProcArray[0].PlanNum,_listInsPlans);
			bool isBlueBookOn=PrefC.GetEnum<AllowedFeeSchedsAutomate>(PrefName.AllowedFeeSchedsAutomate)==AllowedFeeSchedsAutomate.BlueBook;
			//Start in the allowed column if plan uses blue book or it has an allowed fee schedule.
			if((isBlueBookOn && insPlan.PlanType=="" && insPlan.IsBlueBookEnabled) || insPlan.AllowedFeeSched!=0) {
				gridMain.SetSelected(new Point(gridMain.Columns.GetIndex(Lan.g("TableClaimProc","Allowed")),0));//Allowed, first row.
			}
			else{
				gridMain.SetSelected(new Point(gridMain.Columns.GetIndex(Lan.g("TableClaimProc","Ins Pay")),0));//InsPay, first row.
			}
		}

		private void FillGrid(){
			//Changes made in this window do not get saved until after this window closes.
			//But if you double click on a row, then you will end up saving.  That shouldn't hurt anything, but could be improved.
			//also calculates totals for this "payment"
			//the payment itself is imaginary and is simply the sum of the claimprocs on this form
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			List<string> listStringsDefDescriptions=new List<string>();
			listStringsDefDescriptions.Add("None");
			for(int i=0;i<_listDefs.Count;i++){
				listStringsDefDescriptions.Add(_listDefs[i].ItemName);
			}
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Date"),66));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Prov"),50));
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Code"),75));
			}
			else {
				gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Code"),50));
				gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Tth"),25));
			}
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Description"),130));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Fee"),62,HorizontalAlignment.Right));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Billed to Ins"),75,HorizontalAlignment.Right));
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Labs"),62,HorizontalAlignment.Right));
			}
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Deduct"),62,HorizontalAlignment.Right,true));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Allowed"),62,HorizontalAlignment.Right,true));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Ins Pay"),62,HorizontalAlignment.Right,true));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Write-off"),62,HorizontalAlignment.Right,_isWriteOffEditable));
			if(_doShowPatResp) {
				gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Pat Resp"),62,HorizontalAlignment.Right));
			}
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Status"),50,HorizontalAlignment.Center));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Pmt"),30,HorizontalAlignment.Center));
			if(PrefC.GetYN(PrefName.ClaimEditShowPayTracking)) {
				gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Pay Tracking"),90) { ListDisplayStrings=listStringsDefDescriptions,DropDownWidth=160 });
			}
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Remarks"),130,true){ IsWidthDynamic=true });
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ClaimProcArray.Length;i++) {
				row=new GridRow();
				if(ClaimProcArray[i].ProcNum==0) {//Total payment
					//We want to always show the "Payment Date" instead of the procedure date for total payments because they are not associated to procedures.
					row.Cells.Add(ClaimProcArray[i].DateCP.ToShortDateString());
				}
				else {
					row.Cells.Add(ClaimProcArray[i].ProcDate.ToShortDateString());
				}
				row.Cells.Add(Providers.GetAbbr(ClaimProcArray[i].ProvNum));
				double procFeeTotal=0;
				if(ClaimProcArray[i].ProcNum==0) {
					row.Cells.Add("");
					if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
						row.Cells.Add("");
					}
					row.Cells.Add(Lan.g(this,"Total Payment"));
				}
				else {
					Procedure procedure=Procedures.GetProcFromList(_listProcedures,ClaimProcArray[i].ProcNum);//will return a new procedure if none found.
					procFeeTotal=procedure.ProcFeeTotal;
					ProcedureCode procedureCode=ProcedureCodes.GetProcCode(procedure.CodeNum);
					row.Cells.Add(procedureCode.ProcCode);
					if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
						row.Cells.Add(procedure.ToothNum=="" ? Tooth.SurfTidyFromDbToDisplay(procedure.Surf,procedure.ToothNum) : Tooth.Display(procedure.ToothNum));
					}
					string descript=procedureCode.Descript;
					if(procedureCode.IsCanadianLab) {
						descript="^ ^ "+descript;
					}
					row.Cells.Add(descript);
				}
				row.Cells.Add(procFeeTotal.ToString("F"));
				row.Cells.Add(ClaimProcArray[i].FeeBilled.ToString("F"));
				double labFeesForProc=0;
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					labFeesForProc=Procedures.GetCanadianLabFees(ClaimProcArray[i].ProcNum,_listProcedures).Sum(x => x.ProcFee);
					row.Cells.Add(labFeesForProc.ToString("F"));
				}
				row.Cells.Add(ClaimProcArray[i].DedApplied.ToString("F"));
				if(ClaimProcArray[i].AllowedOverride==-1){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(ClaimProcArray[i].AllowedOverride.ToString("F"));
				}
				row.Cells.Add(ClaimProcArray[i].InsPayAmt.ToString("F"));
				row.Cells.Add(ClaimProcArray[i].WriteOff.ToString("F"));
				if(_doShowPatResp) {
					if(ClaimProcArray[i].Status==ClaimProcStatus.Supplemental) {
						procFeeTotal=0;
					}
					row.Cells.Add((procFeeTotal+labFeesForProc-ClaimProcArray[i].InsPayAmt-ClaimProcArray[i].WriteOff).ToString("F"));
				}
				switch(ClaimProcArray[i].Status){
					case ClaimProcStatus.Received:
						row.Cells.Add(Lan.g("TableClaimProc","Recd"));
						break;
					//adjustment would never show here
					case ClaimProcStatus.Preauth:
						row.Cells.Add(Lan.g("TableClaimProc","PreA"));
						break;
					case ClaimProcStatus.Supplemental:
						row.Cells.Add(Lan.g("TableClaimProc","Supp"));
						break;
					case ClaimProcStatus.CapClaim:
						row.Cells.Add(Lan.g("TableClaimProc","Cap"));
						break;
					case ClaimProcStatus.NotReceived:
					default:
						row.Cells.Add("");
						break;
					//Estimate would never show here
					//CapEstimate and CapComplete would never show here
				}
				if(ClaimProcArray[i].ClaimPaymentNum>0){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				if(PrefC.GetYN(PrefName.ClaimEditShowPayTracking)) {
					bool isDefPresent=false;
					for(int j=0;j<_listDefs.Count;j++) {
						if(ClaimProcArray[i].ClaimPaymentTracking==_listDefs[j].DefNum) {
							row.Cells.Add(_listDefs[j].ItemName);
							row.Cells[row.Cells.Count-1].ComboSelectedIndex=j+1;
							isDefPresent=true;
							break;
						}
					}
					if(!isDefPresent) { //The ClaimPaymentTracking definition has been hidden or ClaimPaymentTracking==0 (not possible to hide last pay tracking def)
						row.Cells.Add("");
						row.Cells[row.Cells.Count-1].ComboSelectedIndex=0;
					}
				}
				row.Cells.Add(ClaimProcArray[i].Remarks);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			FillTotals();
		}

		/// <summary>Goes through logic to apply an AsTotal payment to specific procedures depending on their current amount of allocated money. </summary>
		private void ApplyAsTotalPayment() {
			ClaimProcs.ApplyAsTotalPayment(ref ClaimProcArray,_totalPayAmt,_listProcedures);
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!SaveGridChanges()) {
				return;
			}
			List<ClaimProcHist> listClaimProcHists=null;
			List<ClaimProcHist> listClaimprocHistsLoop=null;
			using FormClaimProc formClaimProc=new FormClaimProc(ClaimProcArray[e.Row],null,_family,_patient,_listInsPlans,listClaimProcHists,ref listClaimprocHistsLoop,_listPatPlans,false,_listInsSubs);
			formClaimProc.IsInClaim=true;
			//no need to worry about permissions here
			formClaimProc.ShowDialog();
			if(formClaimProc.DialogResult!=DialogResult.OK){
				return;
			}
			FillGrid();
			FillTotals();
		}

		private void gridMain_CellTextChanged(object sender,EventArgs e) {
			FillTotals();
		}

		///<Summary>Fails silently if text is in invalid format.</Summary>
		private void FillTotals(){
			double claimFee=0;
			double labFees=0;
			double dedApplied=0;
			double insPayAmtAllowed=0;
			double insPayAmt=0;
			double writeOff=0;
			double procFeeTotal=0;
			for(int i=0;i<gridMain.ListGridRows.Count;i++){
				claimFee+=ClaimProcArray[i].FeeBilled;//5
				double labFeesForProc=0;
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					labFeesForProc=PIn.Double(gridMain.ListGridRows[i].Cells[gridMain.Columns.GetIndex(Lan.g("TableClaimProc","Labs"))].Text);
					labFees+=labFeesForProc;
				}
				dedApplied+=PIn.Double(gridMain.ListGridRows[i].Cells[gridMain.Columns.GetIndex(Lan.g("TableClaimProc","Deduct"))].Text);
				insPayAmtAllowed+=PIn.Double(gridMain.ListGridRows[i].Cells[gridMain.Columns.GetIndex(Lan.g("TableClaimProc","Allowed"))].Text);
				double insPayAmtCur=PIn.Double(gridMain.ListGridRows[i].Cells[gridMain.Columns.GetIndex(Lan.g("TableClaimProc","Ins Pay"))].Text);
				insPayAmt+=insPayAmtCur;
				double writeOffCur=PIn.Double(gridMain.ListGridRows[i].Cells[gridMain.Columns.GetIndex(Lan.g("TableClaimProc","Write-off"))].Text);
				writeOff+=writeOffCur;
				if(_doShowPatResp) {
					double procFeeCur=Procedures.GetProcFromList(_listProcedures,ClaimProcArray[i].ProcNum).ProcFeeTotal;
					if(ClaimProcArray[i].Status==ClaimProcStatus.Supplemental) {
						procFeeCur=0;
					}
					procFeeTotal+=procFeeCur;
					gridMain.ListGridRows[i].Cells[gridMain.Columns.GetIndex(Lan.g("TableClaimProc","Pat Resp"))].Text=
						(procFeeCur+labFeesForProc-insPayAmtCur-writeOffCur).ToString("F");
					gridMain.Invalidate();
				}
			}
			textClaimFee.Text=claimFee.ToString("F");
			textLabFees.Text=labFees.ToString("F");
			textDedApplied.Text=dedApplied.ToString("F");
			textInsPayAllowed.Text=insPayAmtAllowed.ToString("F");
			textInsPayAmt.Text=insPayAmt.ToString("F");
			textWriteOff.Text=writeOff.ToString("F");
			if(_doShowPatResp) {
				textPatResp.Text=(procFeeTotal+labFees-writeOff-insPayAmt).ToString("F");
			}
		}

		private bool SaveGridChanges(bool skipChecks=false){
			//validate all grid cells
			double dbl=0;
			int idxDeduct=gridMain.Columns.GetIndex(Lan.g("TableClaimProc","Deduct"));
			int idxAllowed=gridMain.Columns.GetIndex(Lan.g("TableClaimProc","Allowed"));
			int idxInsPay=gridMain.Columns.GetIndex(Lan.g("TableClaimProc","Ins Pay"));
			int idxWriteOff=gridMain.Columns.GetIndex(Lan.g("TableClaimProc","Write-off"));
			int idxStatus=gridMain.Columns.GetIndex(Lan.g("TableClaimProc","Status"));
			for(int i=0;i<gridMain.ListGridRows.Count;i++){
				bool isValid=true;
				if(gridMain.ListGridRows[i].Cells[idxDeduct].Text != "") {
					isValid=Double.TryParse(gridMain.ListGridRows[i].Cells[idxDeduct].Text,out double result);//we don't care about the result
				}
				if(gridMain.ListGridRows[i].Cells[idxAllowed].Text != "") {
					isValid &= Double.TryParse(gridMain.ListGridRows[i].Cells[idxAllowed].Text,out double result);	
				}
				if(gridMain.ListGridRows[i].Cells[idxInsPay].Text != "") {
					isValid &= Double.TryParse(gridMain.ListGridRows[i].Cells[idxInsPay].Text,out double result);	
				}
				if(gridMain.ListGridRows[i].Cells[idxWriteOff].Text != "") {
					isValid &= Double.TryParse(gridMain.ListGridRows[i].Cells[idxWriteOff].Text,out double result);	
				}
				if(!isValid) {
					MsgBox.Show(this,"Invalid number.  It needs to be in 0.00 form.");
					return false;
				}
				if(gridMain.ListGridRows[i].Cells[idxWriteOff].Text != "") {
					dbl=Convert.ToDouble(gridMain.ListGridRows[i].Cells[idxWriteOff].Text);
					if(dbl<0 && gridMain.ListGridRows[i].Cells[idxStatus].Text!="Supp") {
						MsgBox.Show(this,"Only supplemental payments can have a negative write-off.");
						return false;
					}
					double claimWriteOffTotal=ClaimProcs.GetClaimWriteOffTotal(ClaimProcArray[0].ClaimNum,ClaimProcArray[i].ProcNum,ClaimProcArray.ToList());
					if(claimWriteOffTotal+dbl<0) {
						MsgBox.Show(this,"The current write-off value for supplemental payment "+(i+1).ToString()+" will cause the procedure's total write-off to be negative.  Please change it to be at least "+(dbl-(claimWriteOffTotal+dbl)).ToString()+" to continue.");
						return false;
					}	
				}
			}
			if(!skipChecks && ClaimL.AreCreditsGreaterThanProcFee(GetListClaimProcHypothetical())) {
				return false;
			}
			for(int i=0;i<ClaimProcArray.Length;i++) {
				ClaimProcArray[i].DedApplied=PIn.Double(gridMain.ListGridRows[i].Cells[idxDeduct].Text);
				if(gridMain.ListGridRows[i].Cells[idxAllowed].Text=="") {
					ClaimProcArray[i].AllowedOverride=-1;
				}
				else {
					ClaimProcArray[i].AllowedOverride=PIn.Double(gridMain.ListGridRows[i].Cells[idxAllowed].Text);
				}
				ClaimProcArray[i].InsPayAmt=PIn.Double(gridMain.ListGridRows[i].Cells[idxInsPay].Text);
				ClaimProcArray[i].WriteOff=PIn.Double(gridMain.ListGridRows[i].Cells[idxWriteOff].Text);
				if(PrefC.GetYN(PrefName.ClaimEditShowPayTracking)) {
					int idxCol=gridMain.Columns.GetIndex(Lan.g("TableClaimProc","Pay Tracking"));
					if(idxCol>-1 && idxCol<gridMain.ListGridRows[i].Cells.Count) {
						int idx=gridMain.ListGridRows[i].Cells[idxCol].ComboSelectedIndex;
						ClaimProcArray[i].ClaimPaymentTracking=idx==0 ? 0 : _listDefs[idx-1].DefNum;
					}
				}
				ClaimProcArray[i].Remarks=gridMain.ListGridRows[i].Cells[gridMain.Columns.GetIndex(Lan.g("TableClaimProc","Remarks"))].Text;
			}
			return true;
		}

		///<summary>Checks the ValidDouble fields below the grid and makes sure they are within their min/max values.  Returns true if valid.</summary>
		private bool ValidateTotals() {
			if(textLabFees.IsValid() && textClaimFee.IsValid() && textDedApplied.IsValid() && textInsPayAllowed.IsValid() && textInsPayAmt.IsValid() && textWriteOff.IsValid() && textPatResp.IsValid()) {
				return true;
			}
			return false;
		}

		///<summary>Translates the grid into claimprocs, with edits applied.</summary>
		private List<ClaimProc> GetListClaimProcHypothetical() {
			List<ClaimProc> listClaimProcsHypothetical=new List<ClaimProc>();
			for(int i=0;i<ClaimProcArray.Length;i++) {
				ClaimProc claimProc=ClaimProcArray[i].Copy();
				int idxInsPay=gridMain.Columns.GetIndex(Lan.g("TableClaimProc","Ins Pay"));
				int idxWriteOff=gridMain.Columns.GetIndex(Lan.g("TableClaimProc","Write-off"));
				claimProc.InsPayAmt=PIn.Double(gridMain.ListGridRows[i].Cells[idxInsPay].Text);
				claimProc.WriteOff=PIn.Double(gridMain.ListGridRows[i].Cells[idxWriteOff].Text); 
				listClaimProcsHypothetical.Add(claimProc);
			}
			return listClaimProcsHypothetical;
    }

		private void butDeductible_Click(object sender,EventArgs e) {
			if(gridMain.SelectedCell.X==-1){
				MsgBox.Show(this,"Please select one procedure.  Then click this button to assign the deductible to that procedure.");
				return;
			}
			if(!SaveGridChanges()) {
				return;
			}
			double dedAmt=0;
			//remove the existing deductible from each procedure and move it to dedAmt.
			for(int i=0;i<ClaimProcArray.Length;i++){
				if(ClaimProcArray[i].DedApplied > 0){
					dedAmt+=ClaimProcArray[i].DedApplied;
					ClaimProcArray[i].InsPayEst+=ClaimProcArray[i].DedApplied;//dedAmt might be more
					ClaimProcArray[i].InsPayAmt+=ClaimProcArray[i].DedApplied;
					ClaimProcArray[i].DedApplied=0;
				}
			}
			if(dedAmt==0){
				MsgBox.Show(this,"There does not seem to be a deductible to apply.  You can still apply a deductible manually by double clicking on a procedure.");
				return;
			}
			//then move dedAmt to the selected proc
			ClaimProcArray[gridMain.SelectedCell.Y].DedApplied=dedAmt;
			ClaimProcArray[gridMain.SelectedCell.Y].InsPayEst-=dedAmt;
			ClaimProcArray[gridMain.SelectedCell.Y].InsPayAmt-=dedAmt;
			FillGrid();
		}

		public void butWriteOff_Click(object sender,EventArgs e) {
			DialogResult dialogResult=DialogResult.Cancel;
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				dialogResult=MessageBox.Show(
					 Lan.g(this,"Write off unpaid amounts on labs and procedures?")+"\r\n"
					+Lan.g(this,"Choose Yes to write off unpaid amounts on both labs and procedures.")+"\r\n"
					+Lan.g(this,"Choose No to write off unpaid amounts on procedures only."),"",MessageBoxButtons.YesNoCancel);
				if(dialogResult!=DialogResult.Yes && dialogResult!=DialogResult.No) {//Cancel
					return;
				}
			}
			else {//United States
				if(MessageBox.Show(Lan.g(this,"Write off unpaid amount on each procedure?"),"",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
					return;
				}
			}
			if(!SaveGridChanges(skipChecks:true)) {
				return;
			}
			bool isCanadian=CultureInfo.CurrentCulture.Name.EndsWith("CA");//Canadian. en-CA or fr-CA 
			List<Procedure> listProceduresLabs=new List<Procedure>();
			if(isCanadian) {
				listProceduresLabs=Procedures.GetCanadianLabFees(ClaimProcArray.Select(x => x.ProcNum).ToList());//0, 1 or 2 lab fees per procedure
			}
			if(isCanadian && dialogResult==DialogResult.Yes) {//Canadian lab fees should be written off.
				Claim claim=Claims.GetClaim(ClaimProcArray[0].ClaimNum);//There should be at least one, since a claim can only be created with one or more procedures.
				ClaimProc claimProcTotalLabs=new ClaimProc();
				claimProcTotalLabs.ClaimNum=claim.ClaimNum;
				claimProcTotalLabs.PatNum=claim.PatNum;
				claimProcTotalLabs.ProvNum=claim.ProvTreat;
				claimProcTotalLabs.Status=ClaimProcStatus.Received;
				claimProcTotalLabs.PlanNum=claim.PlanNum;
				claimProcTotalLabs.InsSubNum=claim.InsSubNum;
				claimProcTotalLabs.DateCP=DateTime.Today;
				claimProcTotalLabs.ProcDate=claim.DateService;
				claimProcTotalLabs.DateEntry=DateTime.Now;
				claimProcTotalLabs.ClinicNum=claim.ClinicNum;
				claimProcTotalLabs.WriteOff=0;
				claimProcTotalLabs.InsPayAmt=0;
				for(int i=0;i<ClaimProcArray.Length;i++) {
					ClaimProc claimProc=ClaimProcArray[i];
					double claimProcOverpayment=0;
					if(claimProc.InsPayAmt>claimProc.FeeBilled) {
						claimProcOverpayment=claimProc.InsPayAmt-claimProc.FeeBilled;//The amount of exceess greater than the fee billed.
						claimProc.InsPayAmt=claimProc.FeeBilled;
					}
					//Get a list of all of the lab procedures for this claimproc.
					List<Procedure> listProceduresLabsCur=listProceduresLabs.FindAll(x => x.ProcNumLab==claimProc.ProcNum);
					double procLabTotal=0;
					double procLabInsPaidAmt=0;
					//Loop through all labs that are associated to the claimproc to figure out how much they are worth and how much insurance has already paid on them.
					for(int j=0;j<listProceduresLabsCur.Count;j++) {
						//Go get the claimproc that is associated to this lab procedure.
						ClaimProc claimProcLab=ClaimProcArray.First(x => x.ProcNum==listProceduresLabsCur[j].ProcNum);
						procLabInsPaidAmt+=claimProcLab.InsPayAmt;
						procLabTotal+=listProceduresLabs[j].ProcFeeTotal;
					}
					if(claimProcOverpayment>procLabTotal) {//Could happen if the user enters a payment amount greater than the fee billed and lab fees added together.
						//The claimproc overpayment is the total lab procedure fees(production) minus the total insurance payment on the lab procedures(income).
						claimProcOverpayment=(procLabTotal - procLabInsPaidAmt);
					}
					claimProcTotalLabs.InsPayAmt+=claimProcOverpayment;
					claimProcTotalLabs.WriteOff+=(procLabTotal - (claimProcOverpayment + procLabInsPaidAmt));
				}
				//Never make a pay as total claimproc with a negative pay amount or write-off.
				claimProcTotalLabs.InsPayAmt=Math.Max(0,claimProcTotalLabs.InsPayAmt);
				claimProcTotalLabs.WriteOff=Math.Max(0,claimProcTotalLabs.WriteOff);
				if(claimProcTotalLabs.InsPayAmt>0 || claimProcTotalLabs.WriteOff>0) {//These amounts will both be zero if there are no lab fees on any of the procedures.  These amounts should never be negative.
					ClaimProcs.Insert(claimProcTotalLabs);
				}
			}
			//fix later: does not take into account other payments.
			double unpaidAmt=0;
			List<Procedure> listProcedures=Procedures.Refresh(_patient.PatNum);
			for(int i=0;i<ClaimProcArray.Length;i++){
				if(isCanadian && listProceduresLabs.Any(x => x.ProcNum==ClaimProcArray[i].ProcNum)) {
					//Canada - Don't calculate the write-offs for any of the claimprocs associated to a procedure lab. The write-offs were calculated/created above.
					continue;
				}
				//ClaimProcsToEdit guaranteed to only contain claimprocs for procedures before this form loads, payments are not in the list
				unpaidAmt=Procedures.GetProcFromList(listProcedures,ClaimProcArray[i].ProcNum).ProcFeeTotal
					//((Procedure)Procedures.HList[ClaimProcsToEdit[i].ProcNum]).ProcFee
					-ClaimProcArray[i].DedApplied
					-ClaimProcArray[i].InsPayAmt;
				if(unpaidAmt <= 0) { // clear out the writeoff if determined to be negative or 0
					ClaimProcArray[i].WriteOff = 0;
				}
				ClaimProcArray[i].WriteOff=unpaidAmt;
			}
			FillGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!SaveGridChanges()) {
				return;
			}
			if(!ValidateTotals()) {
				MsgBox.Show(this,"One or more column totals exceed the maximum allowed value, please fix data entry errors.");
				return;
			}
			//Only update allowed fee schedules when not using blue book.
			if(!_blueBookEstimateData.IsValidForEstimate(ClaimProcArray[0],false)) {
				//Out of Network fee schedule is not used if Blue Book feature is used for this insurance plan.
				FeeL.SaveAllowedFeesFromClaimPayment(ClaimProcArray.ToList(),_listInsPlans);
			}
			if(PrefC.GetBool(PrefName.ClaimSnapshotEnabled)) {
				Claim claim=Claims.GetClaim(_listClaimProcsOld[0].ClaimNum);
				if(claim.ClaimType!="PreAuth") {
					ClaimSnapshots.CreateClaimSnapshot(_listClaimProcsOld,ClaimSnapshotTrigger.InsPayment,claim.ClaimType);
				}
			}
			ClaimTrackings.InsertClaimProcReceived(ClaimProcArray[0].ClaimNum,Security.CurUser.UserNum);
			//Make audit trail entries if writeoff or inspayamt's were changed. The claimprocs are updated outside of this form,
			//but all of the information we need for audit trail logging lives inside the form so we do it here.
			for(int i=0;i<ClaimProcArray.Length;i++) {
				ClaimProc claimProcOld=_listClaimProcsOld.FirstOrDefault(x => x.ProcNum==ClaimProcArray[i].ProcNum);
				ClaimProcs.CreateAuditTrailEntryForClaimProcPayment(ClaimProcArray[i],claimProcOld,IsCalledFromClaimEdit);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	
	}
}







