using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Web.UI;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentalGraph.Cache;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormClaimOverpay:FormODBase {
		///<summary>List of all the procedures for a patient.</summary>
		private List<Procedure> _listProcedures;
		///<summary>List of some of the claimprocs attached to the claim we are editing. Includes overpayment/underpayment claimprocs (ClaimProc.IsOverpay=True).
		///Does NOT include every claimproc such as claimprocs that are total payments.</summary>
		private List<ClaimProc> _listClaimProcs;
		///<summary>Only one overpayment per ProcNum and it belongs to the NotReceived/Received claimproc.</summary>
		private List<ClaimProc> _listPaymentClaimProcs;
		/// <summary>Used to pre-select a row in the grid of claimprocs.</summary>
		private long _selectedClaimProcNum;
		/// <summary>Used to preselect a column in the grid of claimsprocs.</summary>
		private bool _isOverpaid;
		///<summary>Stores the primary key of the claim that the claimprocs which we are editing, belong to.</summary>
		private long _claimNum;
		///<summary>Index of the Overpaid column within the grid. Can vary based on preferences.</summary>
		private int _colInsOverpaidIndex;
		///<summary>Index of the Underpaid column within the grid. Can vary based on preferences.</summary>
		private int _colInsUnderpaidIndex;

		///<summary></summary>
		public FormClaimOverpay(long claimNum,List<ClaimProc> listClaimProcs,long patNum,bool isOverpaid,long selectedClaimProcNum=0) {
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			_isOverpaid=isOverpaid;
			_selectedClaimProcNum=selectedClaimProcNum;
			_claimNum=claimNum;
			_listClaimProcs=ClaimProcs.GetForClaimOverpay(listClaimProcs,_claimNum);
			_listProcedures=Procedures.Refresh(patNum);
			Lan.F(this);
		}

		private void FormClaimOverpay_Load(object sender,EventArgs e) {
			_listPaymentClaimProcs=_listClaimProcs.FindAll(x => !x.IsOverpay && x.Status!=ClaimProcStatus.Supplemental);//Only one overpayment per ProcNum and it belongs to the NotReceived/Received claimproc.
			if(_listPaymentClaimProcs.Count==0){//Job 49660. Will prevent phone calls to our techs by preventing UE below.
				MsgBox.Show(this,"No valid claim procedures attached. Please delete this claim and recreate it with the desired procedures attached or run database maintenance method "+nameof(DatabaseMaintenances.ClaimDeleteWithNoClaimProcs)+".","Error");
				Close();
				return;
			}
			FillGrid();
		}

		///<summary>Fills the grid with regular claimprocs. Does not include overpayment claimprocs in the grid.</summary>
		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Date"),66));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Prov"),50));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Code"),75));
			if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Tth"),25));
			}
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Description"),130));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Fee"),62,HorizontalAlignment.Right));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Ins Pay"),62,HorizontalAlignment.Right));
			_colInsOverpaidIndex=gridMain.Columns.Count;
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Ins Overpaid"),62,HorizontalAlignment.Right,true));
			_colInsUnderpaidIndex=gridMain.Columns.Count;
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Ins Underpaid"),62,HorizontalAlignment.Right,true));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Status"),50,HorizontalAlignment.Center));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Pmt"),30,HorizontalAlignment.Center));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableClaimProc","Remarks"),130){ IsWidthDynamic=true });
			gridMain.ListGridRows.Clear();
			ClaimProc claimProcSelected=_listClaimProcs.Find(x => x.ClaimProcNum==_selectedClaimProcNum);//Retrieve the claimproc that should be pre-selected in the grid.
			if(claimProcSelected!=null && claimProcSelected.IsOverpay) { //If the pre-selected claimproc is an under/overpayment claimproc, find its matching regular claimproc instead.
				claimProcSelected=_listClaimProcs.Find(x => x.ClaimNum==claimProcSelected.ClaimNum && x.ProcNum==claimProcSelected.ProcNum && !x.IsOverpay);
			}
			GridRow row;
			int selectedIndex=0;
			for(int i=0;i<_listPaymentClaimProcs.Count;i++) {//Loop through the non-under/overpayment claimprocs to insert them into the grid.
				row=new GridRow();
				row.Cells.Add(_listPaymentClaimProcs[i].ProcDate.ToShortDateString());
				row.Cells.Add(Providers.GetAbbr(_listPaymentClaimProcs[i].ProvNum));
				Procedure procedure=Procedures.GetProcFromList(_listProcedures,_listPaymentClaimProcs[i].ProcNum);//will return a new procedure if none found.
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(procedure.CodeNum);
				row.Cells.Add(procedureCode.ProcCode);
				if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
					row.Cells.Add(procedure.ToothNum=="" ? Tooth.SurfTidyFromDbToDisplay(procedure.Surf,procedure.ToothNum) : Tooth.Display(procedure.ToothNum));
				}
				row.Cells.Add(procedureCode.Descript);
				row.Cells.Add(procedure.ProcFeeTotal.ToString("F"));
				double insPayAmt=_listClaimProcs.FindAll(x => x.ProcNum==_listPaymentClaimProcs[i].ProcNum && !x.IsOverpay).Sum(x => x.InsPayAmt);
				row.Cells.Add(insPayAmt.ToString("F"));
				ClaimProc claimProcOverpay=_listClaimProcs//Should be at-most one overpayment claimproc per regular claimproc
					.Find(x => x.ClaimNum==_listPaymentClaimProcs[i].ClaimNum && x.ProcNum==_listPaymentClaimProcs[i].ProcNum && x.IsOverpay);
				if(claimProcOverpay==null || claimProcOverpay.InsEstTotalOverride==0) {
					row.Cells.Add("");
					row.Cells.Add("");
				}
				else if(claimProcOverpay.InsEstTotalOverride<0) { //Overpayment
					row.Cells.Add((-claimProcOverpay.InsEstTotalOverride).ToString("F"));
					row.Cells.Add("");
				}
				else { //Underpayment
					row.Cells.Add("");
					row.Cells.Add(claimProcOverpay.InsEstTotalOverride.ToString("F"));
				}
				switch(_listPaymentClaimProcs[i].Status){//Only statuses which can be attached to a claim.
					case ClaimProcStatus.Received:
						row.Cells.Add(Lan.g("TableClaimProc","Recd"));
						break;
					case ClaimProcStatus.Supplemental:
						row.Cells.Add(Lan.g("TableClaimProc","Supp"));
						break;
					case ClaimProcStatus.CapClaim:
						row.Cells.Add(Lan.g("TableClaimProc","Cap"));
						break;
					case ClaimProcStatus.NotReceived:
						row.Cells.Add(Lan.g("TableClaimProc","NotRec"));
						break;
					default:
						row.Cells.Add("");
						break;
				}
				if(_listPaymentClaimProcs[i].ClaimPaymentNum>0){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				if(claimProcOverpay==null) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(claimProcOverpay.Remarks);
				}
				row.Tag=_listPaymentClaimProcs[i];//Tag the row with the payment claimproc.
				gridMain.ListGridRows.Add(row);
				if(_listPaymentClaimProcs[i].ClaimProcNum==claimProcSelected?.ClaimProcNum) {
					selectedIndex=i;
				}
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(new Point(_isOverpaid?_colInsOverpaidIndex:_colInsUnderpaidIndex,selectedIndex));
			FillTotals();
		}

		private void gridMain_CellTextChanged(object sender,EventArgs e) {
			//Recalculate all of the Totals text boxes that show up underneath the Procedures grid.
			FillTotals();
		}

		///<Summary>Recalculate all of the Totals text boxes that show up underneath the Procedures grid.</Summary>
		private void FillTotals(){
			double overpayTotal=0;
			double underpayTotal=0;
			for(int i=0;i<gridMain.ListGridRows.Count;i++){
				overpayTotal+=PIn.Double(gridMain.ListGridRows[i].Cells[_colInsOverpaidIndex].Text);
				underpayTotal+=PIn.Double(gridMain.ListGridRows[i].Cells[_colInsUnderpaidIndex].Text);
			}
			textOverpayTotal.Text=overpayTotal.ToString("F");
			textUnderpayTotal.Text=underpayTotal.ToString("F");
		}

		private bool IsValid() {
			if(!textOverpayTotal.IsValid() || !textUnderpayTotal.IsValid()) {
				MsgBox.Show(this,"One or more column totals exceed the maximum allowed value, please fix data entry errors.","Error");
				return false;
			}
			for(int i=0;i<gridMain.ListGridRows.Count;i++){
				double insOverpaidAmount=PIn.Double(gridMain.ListGridRows[i].Cells[_colInsOverpaidIndex].Text);
				double insUnderpaidAmount=PIn.Double(gridMain.ListGridRows[i].Cells[_colInsUnderpaidIndex].Text);
				if(insOverpaidAmount!=0 && insUnderpaidAmount!=0) {
					MsgBox.Show(this,"The same procedure cannot be both overpaid and underpaid.","Error");
					return false;
				}
				if(insOverpaidAmount<0 || insUnderpaidAmount<0) {
					MsgBox.Show(this,"Overpaid or underpaid amount cannot be negative. Please fix any negative entries.","Error");
					return false;
				}
			}
			return true;
		}

		private void Save() {
			List<ClaimProc> listDeleteClaimProcs=new List<ClaimProc>();
			List<ClaimProc> listUpdateClaimProcs=new List<ClaimProc>();
			List<ClaimProc> listInsertClaimProcs=new List<ClaimProc>();
			List<long> listRequiredCanadaParentProcNums=new List<long>();
			for(int i=0;i<gridMain.ListGridRows.Count;i++){
				ClaimProc claimProc=(ClaimProc)gridMain.ListGridRows[i].Tag;
				ClaimProc claimProcOverpay=_listClaimProcs.Find(x => x.ClaimNum==claimProc.ClaimNum && x.ProcNum==claimProc.ProcNum && x.IsOverpay);
				double overpaidAmt=PIn.Double(gridMain.ListGridRows[i].Cells[_colInsOverpaidIndex].Text);
				double underpaidAmt=PIn.Double(gridMain.ListGridRows[i].Cells[_colInsUnderpaidIndex].Text);
				if(overpaidAmt==0 && underpaidAmt==0) {
					if(claimProcOverpay!=null) {
						listDeleteClaimProcs.Add(claimProcOverpay);
					}
				}
				else {
					bool isOverpaid=(overpaidAmt!=0);//overpaidAmt cannot be negative, because of validation before saving.
					double insEstTotalOverride=isOverpaid?(-overpaidAmt):underpaidAmt;
					claimProcOverpay=ClaimProcs.CreateOverpay(claimProc,insEstTotalOverride,claimProcOverpay);//claimProcOverpay will not be null after returning from here.
					if(isOverpaid) {
						claimProcOverpay.Remarks="Ins Overpaid";
					}
					else {
						claimProcOverpay.Remarks="Ins Underpaid";
					}
					Procedure procOverpay=_listProcedures.FirstOrDefault(x => x.ProcNum==claimProcOverpay.ProcNum);
					if(procOverpay.ProcNumLab!=0) {//Is a Canadian Lab Overpayment
						listRequiredCanadaParentProcNums.Add(procOverpay.ProcNumLab);
					}
					if(claimProcOverpay.ClaimProcNum==0) {
						listInsertClaimProcs.Add(claimProcOverpay);
					}
					else {
						listUpdateClaimProcs.Add(claimProcOverpay);
					}
				}
			}
			listRequiredCanadaParentProcNums=listRequiredCanadaParentProcNums.Distinct().ToList();
			//Canadian Lab Overpayments require the parent proc overpayment to display properly in FormClaimEdit, even if the parent overpayment is 0.
			listDeleteClaimProcs.RemoveAll(x => listRequiredCanadaParentProcNums.Contains(x.ProcNum));//Do not allow Canadian Lab Overpayment parent to be deleted.
			listRequiredCanadaParentProcNums.RemoveAll(x => listUpdateClaimProcs.Exists(y => y.ProcNum==x));//Canadian Lab Overpayments with existing parent procedure are good to go.
			listRequiredCanadaParentProcNums.RemoveAll(x => listInsertClaimProcs.Exists(y => y.ProcNum==x));//Canadian Lab Overpayments with parents being inserted are good to go.
			//Now listRequiredCanadaParentProcNums contains Canadian Lab Overpayment parent procedures which need to be inserted or updated.
			for(int i=0;i<listRequiredCanadaParentProcNums.Count;i++) {
				ClaimProc claimProc=_listPaymentClaimProcs.FirstOrDefault(x => x.ProcNum==listRequiredCanadaParentProcNums[i]);
				ClaimProc claimProcOverpay=_listClaimProcs.Find(x => x.ClaimNum==claimProc.ClaimNum && x.ProcNum==claimProc.ProcNum && x.IsOverpay);
				claimProcOverpay=ClaimProcs.CreateOverpay(claimProc,0,claimProcOverpay);//claimProcOverpay will not be null after returning from here.
				if(claimProcOverpay.ClaimProcNum==0) {
					listInsertClaimProcs.Add(claimProcOverpay);
				}
				else {
					listUpdateClaimProcs.Add(claimProcOverpay);
				}
			}
			ClaimProcs.DeleteMany(listDeleteClaimProcs);
			ClaimProcs.UpdateMany(listUpdateClaimProcs);
			ClaimProcs.InsertMany(listInsertClaimProcs);
		}

		private void butToSupplemental_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will permanently change all pending supplemental insurance payments for this claim to regular supplemental payments and cannot be reversed except manually. Continue?","Warning")) {
				return;
			}
			if(!IsValid()) {
				return;
			}
			Save();
			_listClaimProcs=ClaimProcs.RefreshForClaims(new List<long> { _claimNum }).ToList();
			_listClaimProcs=ClaimProcs.GetForClaimOverpay(_listClaimProcs,_claimNum);
			List<ClaimProc> listClaimProcsOverpaid=_listClaimProcs.FindAll(x => x.IsOverpay);
			if(listClaimProcsOverpaid.Count==0) {
				MsgBox.Show(this,"There are no pending supplemental insurance payments for this claim.");
				return;
			}
			for(int i=0;i<listClaimProcsOverpaid.Count;i++) {
				ClaimProc claimProcOld=listClaimProcsOverpaid[i].Copy();
				listClaimProcsOverpaid[i].Status=ClaimProcStatus.Supplemental;
				listClaimProcsOverpaid[i].InsPayAmt=listClaimProcsOverpaid[i].InsEstTotalOverride;
				//Zero out InsPayEst, because Supplemental payments always have InsPayEst set to 0 when entering manually from FormClaimEdit.
				//Also if we allow InsPayEst to continue to be non-zero, then DatabaseMaintenances.ClaimProcEstNoBillIns() will zero it out for us.
				listClaimProcsOverpaid[i].InsPayEst=0;
				listClaimProcsOverpaid[i].DateSuppReceived=DateTime.Today;
				listClaimProcsOverpaid[i].IsOverpay=false;
				ClaimProcs.Update(listClaimProcsOverpaid[i],claimProcOld);
			}
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!IsValid()) {
				return;
			}
			Save();
			DialogResult=DialogResult.OK;
		}

	}
}