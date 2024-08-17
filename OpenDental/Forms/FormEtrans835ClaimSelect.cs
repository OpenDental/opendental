using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;
using System.Text.RegularExpressions;

namespace OpenDental {
	public partial class FormEtrans835ClaimSelect:FormODBase {

		///<summary>The selected patient.  Used to get list of claims to fill grid.</summary>
		private long _patNum;
		///<summary>The claim the user selected from the grid.</summary>
		public Claim ClaimSelected=null;
		private Hx835_Claim _hx835_Claim;
		///<summary>The list of claimProcs assocaited to the claim the user clicks/has highlighted in the grid.
		///Only contains the procedure claimProcs, exc ludes By Total rows.
		///We do this since we only care about if the ERA procs can match with the claims initial procs.</summary>
		private List<ClaimProc> _listClaimProcs;
		///<summary>PatPlans for the patient.  Lazy loaded when needed.</summary>
		private List<PatPlan> _listPatPlans;
		///<summary>Informs the user that a patient could not be automatically matched.</summary>
		private ErrorProvider _errorProvider=new ErrorProvider();

		///<summary>PatNum used to get claims to fill grid.  x835Claim used to fill default text for date and claim fee filters and disallow OK click if 
		///claim details do not match.</summary>
		public FormEtrans835ClaimSelect(long patNum,Hx835_Claim hx835_Claim) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_hx835_Claim=hx835_Claim;
			_patNum=patNum;
			if(_patNum==0) {
				_errorProvider.SetError(butPatFind,Lans.g(this,"Patient not found"));
			}
			if(_hx835_Claim.IsReversal) {
				this.Text+=" - "+Lans.g(this,"Pick Original Claim for this Claim Reversal");
			}
			else if(_hx835_Claim.IsSplitClaim) {
				this.Text+=" - "+Lans.g(this,"Pick Original Claim for this Split Claim");
				labelSplitClaims.Visible=true;
			}
			if(_patNum!=0) {//Otherwise a comma and period would show.
				textPatient.Text=Patients.GetLim(_patNum).GetNameLF();
			}
			decimal claimFee=hx835_Claim.ClaimFee;
			if(_hx835_Claim.IsReversal) {
				//Claim reversals have an exact negation of the original claimFee.
				claimFee=Math.Abs(claimFee);
			}
			textClaimFee.Text=claimFee.ToString();
			textDateFrom.Text=hx835_Claim.DateServiceStart.ToShortDateString();
			textDateTo.Text=hx835_Claim.DateServiceEnd.ToShortDateString();
		}

		private void FormEtrans835ClaimSelect_Load(object sender,EventArgs e) {
			FillGridClaims();
			HighlightRows();
		}

		///<summary>Gets all claims for the patient selected.  Fills gridClaims and tags each row with its corrisponding claim object.</summary>
		private void FillGridClaims() {
			int sortByColIdx=gridClaims.SortedByColumnIdx;  //Keep previous sorting
			bool isSortAsc=gridClaims.SortedIsAscending;
			if(sortByColIdx==-1) {
				sortByColIdx=0;
				isSortAsc=false;
			}
			gridClaims.BeginUpdate();
			gridClaims.ListGridRows.Clear();
			gridClaims.Columns.Clear();
			UI.GridColumn col;
			col=new UI.GridColumn("Date Service",100,HorizontalAlignment.Center);
			col.SortingStrategy=UI.GridSortingStrategy.DateParse;
			gridClaims.Columns.Add(col);
			col=new UI.GridColumn("Carrier",240,HorizontalAlignment.Center);
			col.SortingStrategy=UI.GridSortingStrategy.StringCompare;
			gridClaims.Columns.Add(col);
			col=new UI.GridColumn("Status",120,HorizontalAlignment.Center);
			col.SortingStrategy=UI.GridSortingStrategy.StringCompare;
			gridClaims.Columns.Add(col);			
			if(PrefC.HasClinicsEnabled) {//Using clinics
				col=new UI.GridColumn("Clinic",190,HorizontalAlignment.Left);
				col.SortingStrategy=UI.GridSortingStrategy.StringCompare;
				gridClaims.Columns.Add(col);
			}
			col=new UI.GridColumn("ClaimFee",70,HorizontalAlignment.Right);
			col.SortingStrategy=UI.GridSortingStrategy.AmountParse;
			gridClaims.Columns.Add(col);
			List<Claim> listClaims=Claims.Refresh(_patNum);
			for(int i=0;i<listClaims.Count;i++) {
				UI.GridRow row=new UI.GridRow();
				row.Tag=listClaims[i];
				row.Cells.Add(listClaims[i].DateService.ToShortDateString());//DOS
				row.Cells.Add(Carriers.GetName(InsPlans.RefreshOne(listClaims[i].PlanNum).CarrierNum));//Carrier
				ClaimStatus claimStatus=Claims.GetClaimStatusEnumFromCurClaim(listClaims[i]);
				row.Cells.Add(claimStatus.GetDescription());//Status
				if(PrefC.HasClinicsEnabled) {//Using clinics
					Clinic clinic=Clinics.GetClinic(listClaims[i].ClinicNum);
					if(clinic==null) {
						row.Cells.Add("");//Clinic
					}
					else {
						row.Cells.Add(clinic.Description);//Clinic
					}
				}
				row.Cells.Add(listClaims[i].ClaimFee.ToString("f"));//Claimfee
				gridClaims.ListGridRows.Add(row);
			}
			gridClaims.EndUpdate();
			gridClaims.SortForced(sortByColIdx,isSortAsc);
		}

		///<summary>Sets the foreground text to red if any row has a DOS between textDOSFrom and textDOSTo and matches textClaimFee </summary>
		private void HighlightRows() {
			DateTime dateFrom=PIn.Date(textDateFrom.Text);
			DateTime dateTo=PIn.Date(textDateTo.Text);
			double fee=PIn.Double(textClaimFee.Text);
			int countRowsHighlighted=0;
			int idxLastRowSelected=0;
			gridClaims.BeginUpdate();
			for(int i=0;i<gridClaims.ListGridRows.Count;i++) {
				gridClaims.ListGridRows[i].ColorText=Color.Black;  //reset row highlighting
				gridClaims.ListGridRows[i].Bold=false;  //reset row highlighting
				Claim claim=(Claim)gridClaims.ListGridRows[i].Tag;
				YN ynIsFeeMatch=YN.No;  //If fee matches then yes, if fee doesnt match then no, if no fee entered then unknown
				YN ynIsDateMatch=YN.No; //If both dates match then yes, if both dates dont match then no, if no dates entered then unknown
				//Check fee
				if(textClaimFee.Text==""){  //No fee entered
					ynIsFeeMatch=YN.Unknown;
				}
				else {
					if(claim.ClaimFee.ToString("f").Contains(textClaimFee.Text)){
						ynIsFeeMatch=YN.Yes;
					}
				}
				//Check date
				if(dateFrom==DateTime.MinValue && dateTo==DateTime.MinValue) {  //No dates entered
					ynIsDateMatch=YN.Unknown;
				}
				else {  //At least one date entered
					if((dateFrom.CompareTo(claim.DateService)<=0 || dateFrom==DateTime.MinValue) 
						&& (dateTo.CompareTo(claim.DateService)>=0 || dateTo==DateTime.MinValue)) {
							ynIsDateMatch=YN.Yes;
					}
				}
				Hx835_Proc hx835_Proc=_hx835_Claim.ListProcs.FirstOrDefault();
				bool isPlanMatch=true;//Assume true for backwards compatiablity, because older 837s did not send out the Ordinal and PlanNum.
				bool isBasicMatch=(ynIsFeeMatch==YN.Yes || ynIsDateMatch==YN.Yes) && (ynIsFeeMatch!=YN.No && ynIsDateMatch!=YN.No);//If either match and neither don't match
				if(isBasicMatch && hx835_Proc!=null && hx835_Proc.PlanNum!=0) {//Consider new 837 REF*6R pattern for values starting with 'x'.
					if(_listPatPlans==null) {
						_listPatPlans=PatPlans.GetPatPlansForPat(_patNum);
					}
					//Strict PlanNum AND Ordinal match.  Users can manually change ordinals easily after sending claims.
					//A failure to match here is OK because we will give the user the option to manually select the correct claim from the grid.
					if(hx835_Proc.PlanNum!=claim.PlanNum || PatPlans.GetOrdinal(claim.InsSubNum,_listPatPlans)!=hx835_Proc.PlanOrdinal) {
						isPlanMatch=false;
					}
				}
				if(isBasicMatch && isPlanMatch) {
					//Highlight row
					gridClaims.ListGridRows[i].ColorText=Color.Red;
					gridClaims.ListGridRows[i].Bold=true;
					countRowsHighlighted++;
					idxLastRowSelected=i;
				}
			}
			gridClaims.EndUpdate();
			if(countRowsHighlighted==1) {
				gridClaims.SetSelected(idxLastRowSelected,setValue:true);
				FillClaimDetails(idxLastRowSelected);
			}
		}

		private void butPatFind_Click(object sender,EventArgs e) {
			Patient patient=new Patient();
			patient.LName=_hx835_Claim.PatientName.Lname;
			patient.FName=_hx835_Claim.PatientName.Fname;
			using FormPatientSelect formPatientSelect=new FormPatientSelect(patient);
			formPatientSelect.PreFillSearchBoxes(patient);
			if(formPatientSelect.ShowDialog()!=DialogResult.OK) {
				return;
			}
			_errorProvider.Clear();
			_patNum=formPatientSelect.PatNumSelected;
			textPatient.Text=Patients.GetLim(_patNum).GetNameLF();
			FillGridClaims();
		}
		
		private void gridClaims_CellClick(object sender,UI.ODGridClickEventArgs e) {
			FillClaimDetails(e.Row);
		}

		private void FillClaimDetails(int rowIndex) {
			Claim claimSelected=(Claim)gridClaims.ListGridRows[rowIndex].Tag;
			bool isSupplemental=(claimSelected.ClaimStatus=="R");
			_listClaimProcs=ClaimProcs.RefreshForClaim(claimSelected.ClaimNum)
				.Where(x => x.ProcNum!=0 && x.Status.In(ClaimProcStatus.Received,ClaimProcStatus.NotReceived,ClaimProcStatus.Preauth))
				.ToList();
			gridClaimDetails.BeginUpdate();
			if(gridClaimDetails.Columns.Count==0) {
				#region grid columns
				gridClaimDetails.Columns.Add(new UI.GridColumn("ProcCode",100));
				gridClaimDetails.Columns.Add(new UI.GridColumn("ProcFee",100));
				UI.GridColumn col=new UI.GridColumn("ProcStatus",50);
				col.IsWidthDynamic=true;
				gridClaimDetails.Columns.Add(col);
				col=new UI.GridColumn("IsMatch",15);
				col.IsWidthDynamic=true;
				gridClaimDetails.Columns.Add(col);
				gridClaimDetails.Columns.Add(new UI.GridColumn("EraCode",100));
				gridClaimDetails.Columns.Add(new UI.GridColumn("EraFee",100));
				#endregion
			}
			gridClaimDetails.ListGridRows.Clear();
			List<Hx835_Proc> listHx835_ProcsUnmatched=new List<Hx835_Proc>();
			List<Tuple<Hx835_Proc,ClaimProc>> listHx835_ProcsClaimProcsMatched=new List<Tuple<Hx835_Proc,ClaimProc>>();
			for(int i=0;i<_hx835_Claim.ListProcs.Count;i++) {
				ClaimProc claimProc=_listClaimProcs.FirstOrDefault(x =>
						//Mimics proc matching in claimPaid.GetPaymentsForClaimProcs(...)
						x.ProcNum!=0 && ((x.ProcNum==_hx835_Claim.ListProcs[i].ProcNum)//Consider using Hx835_Proc.TryGetMatchedClaimProc(...)
						|| (x.CodeSent==_hx835_Claim.ListProcs[i].ProcCodeBilled
						&& (decimal)x.FeeBilled==_hx835_Claim.ListProcs[i].ProcFee
						&& (isSupplemental && x.Status==ClaimProcStatus.Received || !isSupplemental && x.Status==ClaimProcStatus.NotReceived)
						&& x.TagOD==null))
					);
				if(claimProc==null) {//Not found
					listHx835_ProcsUnmatched.Add(_hx835_Claim.ListProcs[i]);
				}
				else {
					claimProc.TagOD=true;//Flag set to indicate that claimProc has been handled
					listHx835_ProcsClaimProcsMatched.Add(new Tuple<Hx835_Proc,ClaimProc>(_hx835_Claim.ListProcs[i],claimProc));
				}
			}
			#region ERA procs that could not be matched
			for(int i=0;i<listHx835_ProcsUnmatched.Count;i++) {
				UI.GridRow row=new UI.GridRow();
				row.Cells.Add("");
				row.Cells.Add("");
				row.Cells.Add("");
				row.Cells.Add("N");
				row.Cells.Add(listHx835_ProcsUnmatched[i].ProcCodeBilled);
				row.Cells.Add(POut.Decimal(listHx835_ProcsUnmatched[i].ProcFee));
				row.ColorText=Color.Red;
				row.Bold=true;
				gridClaimDetails.ListGridRows.Add(row);
			}
			#endregion
			#region ERA procs that we can match.
			for(int i=0;i<listHx835_ProcsClaimProcsMatched.Count;i++) {
				Hx835_Proc hx835_Proc=listHx835_ProcsClaimProcsMatched[i].Item1;
				ClaimProc claimProc=listHx835_ProcsClaimProcsMatched[i].Item2;
				UI.GridRow row=new UI.GridRow();
				row.Cells.Add(claimProc.CodeSent);
				row.Cells.Add(POut.Double(claimProc.FeeBilled));
				#region Status column
				switch(claimProc.Status) {
					case ClaimProcStatus.Received:
						row.Cells.Add("Recd");
						break;
					case ClaimProcStatus.NotReceived:
						row.Cells.Add("");
						break;
				}
				#endregion
				row.Cells.Add("Y");
				row.Cells.Add(hx835_Proc.ProcCodeBilled);
				row.Cells.Add(POut.Decimal(hx835_Proc.ProcFee));
				row.ColorText=Color.Green;
				row.Bold=true;
				gridClaimDetails.ListGridRows.Add(row);
			}
			#endregion
			#region Claim claimProcs that could not be matched.
			for(int i=0;i<_listClaimProcs.Count;i++) {
				if(_listClaimProcs[i].TagOD!=null) {
					continue;
				}
				UI.GridRow row=new UI.GridRow();
				row.Cells.Add(_listClaimProcs[i].CodeSent);
				row.Cells.Add(POut.Double(_listClaimProcs[i].FeeBilled));
				switch(_listClaimProcs[i].Status) {
				#region Status column
					case ClaimProcStatus.Received:
						row.Cells.Add("Recd");
						break;
					case ClaimProcStatus.NotReceived:
						row.Cells.Add("");
						break;
				}
				#endregion
				row.Cells.Add("N");
				row.Cells.Add("");
				row.Cells.Add("");
				gridClaimDetails.ListGridRows.Add(row);
			}
			#endregion
			gridClaimDetails.EndUpdate();
		}

		private void gridClaims_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(Permissions.ClaimView)) {
				return;
			}
			Claim claim=Claims.GetClaim(((Claim)gridClaims.ListGridRows[e.Row].Tag).ClaimNum);//This is the easiest way to determine if the claim was deleted.
			if(claim==null) {//Was deleted.
				MsgBox.Show(this,"Claim has been deleted by another user.");
				gridClaims.BeginUpdate();
				gridClaims.ListGridRows.RemoveAt(e.Row);//This will also deselect the row.
				gridClaims.EndUpdate();
				return;
			}
			using FormClaimEdit formClaimEdit=new FormClaimEdit(claim,Patients.GetPat(_patNum),Patients.GetFamily(_patNum));
			formClaimEdit.ShowDialog();
			claim=Claims.GetClaim(((Claim)gridClaims.ListGridRows[e.Row].Tag).ClaimNum);//This is the easiest way to determine if the claim was deleted.
			if(claim==null) {//Was deleted.
				gridClaims.BeginUpdate();
				gridClaims.ListGridRows.RemoveAt(e.Row);//This will also deselect the row.
				gridClaims.EndUpdate();
				return;
			}
			if(formClaimEdit.DialogResult==DialogResult.OK) {
				//Update row
				UI.GridRow row=new UI.GridRow();
				row.Tag=claim;
				row.Cells.Add(claim.DateService.ToShortDateString());//DOS
				row.Cells.Add(Carriers.GetName(InsPlans.RefreshOne(claim.PlanNum).CarrierNum));//Carrier
				ClaimStatus claimStatus=Claims.GetClaimStatusEnumFromCurClaim(claim);
				row.Cells.Add(claimStatus.GetDescription());//Status
				if(PrefC.HasClinicsEnabled) {//Using clinics
					Clinic clinic=Clinics.GetClinic(claim.ClinicNum);
					if(clinic==null) {
						row.Cells.Add("");//Clinic
					}
					else {
						row.Cells.Add(clinic.Description);//Clinic
					}
				}
				row.Cells.Add(claim.ClaimFee.ToString("f"));//Claimfee
				gridClaims.BeginUpdate();
				gridClaims.ListGridRows[e.Row]=row;
				gridClaims.EndUpdate();
				gridClaims.SetSelected(e.Row,setValue:true);//Reselect Row
			}
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			HighlightRows();
		}

		///<summary>Called when validating split claim seleciton information.
		///Returns true if the selected claims claimProcs sum up to the sub set proc information present on this split claim.</summary>
		private bool HasValidSplitClaimTotals() {
			double splitClaimFee=0;
			for(int i=0;i<_hx835_Claim.ListProcs.Count;i++) {
				ClaimProc claimProcMatched=_listClaimProcs.FirstOrDefault(x =>
						//Mimics proc matching in claimPaid.GetPaymentsForClaimProcs(...)
						x.ProcNum!=0 && ((x.ProcNum==_hx835_Claim.ListProcs[i].ProcNum)//Consider using Hx835_Proc.TryGetMatchedClaimProc(...)
						|| (x.CodeSent==_hx835_Claim.ListProcs[i].ProcCodeBilled 
						&& (decimal)x.FeeBilled==_hx835_Claim.ListProcs[i].ProcFee
						&& x.TagOD!=null))//Tag set in FillClaimDetails(...)
					);
				if(claimProcMatched==null){
					return false;//The ERA proc could not be matched to any of the selected claims claim procs.
				}
				splitClaimFee+=claimProcMatched.FeeBilled;
			}
			return ((decimal)splitClaimFee==_hx835_Claim.ClaimFee);
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridClaims.GetSelectedIndex()==-1) {
				MsgBox.Show(this,"You must select a claim.");
				return;
			}
			Claim claimSelected=(Claim)gridClaims.ListGridRows[gridClaims.GetSelectedIndex()].Tag;
			double fee=(double)_hx835_Claim.ClaimFee;
			if(_hx835_Claim.IsReversal) {
				fee=Math.Abs(fee);
			}
			bool isValidClaimFee=true;
			if(_hx835_Claim.IsSplitClaim) {
				isValidClaimFee=HasValidSplitClaimTotals();
			}
			else {
				isValidClaimFee=CompareDouble.IsEqual(claimSelected.ClaimFee,fee);
			}
			if(!isValidClaimFee) {
				MessageBox.Show(Lan.g(this,"Claim fee on claim does not match ERA.")+"  "+Lan.g(this,"Expected")+" "+fee.ToString("f"));
				return;
			}
			if(claimSelected.ClaimType=="PreAuth" && _hx835_Claim.DateServiceStart.Date.Year<=1900) {
				//Some 835s for PreaAuths have been returning 01/01/1900 as an equivalent to DateTime.MinVale. We will treat them as logically equal.
			}
			else if((claimSelected.DateService.Date.CompareTo(_hx835_Claim.DateServiceStart.Date) < 0)
				|| (claimSelected.DateService.Date.CompareTo(_hx835_Claim.DateServiceEnd.Date) > 0))
			{
				MessageBox.Show(Lan.g(this,"Date of service on claim does not match service date range on ERA.")+"\r\n"+Lan.g(this,"Expected")+" "
					+_hx835_Claim.DateServiceStart.ToShortDateString()+" - "+_hx835_Claim.DateServiceEnd.ToShortDateString());
				return;
			}
			ClaimSelected=claimSelected;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}