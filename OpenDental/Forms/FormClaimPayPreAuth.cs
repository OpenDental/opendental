using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental
{
	/// <summary>
	/// Summary description for FormClaimPayTotal.
	/// </summary>
	public partial class FormClaimPayPreAuth:FormODBase {
		///<summary></summary>
		public List<ClaimProc> ClaimProcsToEdit;
		private List<Procedure> ProcList;
		private Patient PatCur;
		private Family FamCur;
		private List<InsPlan> PlanList;
		private List<PatPlan> PatPlanList;
		private List<InsSub> SubList;

		///<summary></summary>
		public FormClaimPayPreAuth(Patient patCur,Family famCur,List<InsPlan> planList,List<PatPlan> patPlanList,List<InsSub> subList) {
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			FamCur=famCur;
			PatCur=patCur;
			PlanList=planList;
			SubList=subList;
			PatPlanList=patPlanList;
			Lan.F(this);
		}

		private void FormClaimPayPreAuth_Load(object sender, System.EventArgs e) {
			ProcList=Procedures.Refresh(PatCur.PatNum);
			FillGrid();
		}

		private void FormClaimPayTotal_Shown(object sender,EventArgs e) {
			int toothIndexOffset=0;
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				toothIndexOffset=1;
			}
			InsPlan plan=InsPlans.GetPlan(ClaimProcsToEdit[0].PlanNum,PlanList);
			gridMain.SetSelected(new Point(4-toothIndexOffset,0));
		}

		private void FillGrid(){
			//Changes made in this window do not get saved until after this window closes.
			//But if you double click on a row, then you will end up saving.  That shouldn't hurt anything, but could be improved.
			//also calculates totals for this "payment"
			//the payment itself is imaginary and is simply the sum of the claimprocs on this form
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				col=new GridColumn(Lan.g(this,"Code"),85);
				gridMain.ListGridColumns.Add(col);
			}
			else {
				col=new GridColumn(Lan.g(this,"Code"),50);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Tth"),35);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Description"),120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Fee"),55,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Estimate"),55,HorizontalAlignment.Right,true);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Remarks"),170,true);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			Procedure ProcCur;
			for(int i=0;i<ClaimProcsToEdit.Count;i++){
				row=new GridRow();
				//for pre-auths, there are no total payments, so ProcNum must be >0
				ProcCur=Procedures.GetProcFromList(ProcList,ClaimProcsToEdit[i].ProcNum);
				row.Cells.Add(ProcedureCodes.GetProcCode(ProcCur.CodeNum).ProcCode);
				if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
					row.Cells.Add(Tooth.ToInternat(ProcCur.ToothNum));
				}
				row.Cells.Add(ProcedureCodes.GetProcCode(ProcCur.CodeNum).Descript);
				row.Cells.Add(ClaimProcsToEdit[i].FeeBilled.ToString("F"));
				row.Cells.Add(ClaimProcsToEdit[i].InsPayEst.ToString("F"));
				row.Cells.Add(ClaimProcsToEdit[i].Remarks);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			FillTotals();
		}

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			try{
				SaveGridChanges();
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			List<ClaimProcHist> histList=null;
			List<ClaimProcHist> loopList=null;
			using FormClaimProc FormCP=new FormClaimProc(ClaimProcsToEdit[e.Row],null,FamCur,PatCur,PlanList,histList,ref loopList,PatPlanList,false,SubList);
			FormCP.IsInClaim=true;
			//no need to worry about permissions here
			FormCP.ShowDialog();
			if(FormCP.DialogResult!=DialogResult.OK){
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
			double insPayEst=0;
			int toothIndexOffset=0;
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				toothIndexOffset=1;
			}
			for(int i=0;i<gridMain.ListGridRows.Count;i++){
				try {
					insPayEst+=Convert.ToDouble(gridMain.ListGridRows[i].Cells[4-toothIndexOffset].Text);
				}
				catch { 
				
				}
			}
			textTotal.Text=insPayEst.ToString("F");
		}

		///<Summary>Surround with try-catch.</Summary>
		private void SaveGridChanges(){
			//validate all grid cells
			double dbl;
			int toothIndexOffset=0;
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				toothIndexOffset=1;
			}
			for(int i=0;i<gridMain.ListGridRows.Count;i++){
				if(gridMain.ListGridRows[i].Cells[4-toothIndexOffset].Text!="") {
					try{
						dbl=Convert.ToDouble(gridMain.ListGridRows[i].Cells[4-toothIndexOffset].Text);
					}
					catch{
						throw new ApplicationException(Lan.g(this,"Amount not valid: ")+gridMain.ListGridRows[i].Cells[4-toothIndexOffset].Text);
					}
				}
			}
			for(int i=0;i<ClaimProcsToEdit.Count;i++){
				ClaimProcsToEdit[i].InsPayEst=PIn.Double(gridMain.ListGridRows[i].Cells[4-toothIndexOffset].Text);
				ClaimProcsToEdit[i].Remarks=gridMain.ListGridRows[i].Cells[5-toothIndexOffset].Text;
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(!textTotal.IsValid()) {
				MessageBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			try {
				SaveGridChanges();
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormClaimPayTotal_Activated(object sender,EventArgs e) {

		}

	
		

	
		

		

		

		

		

		

		



	}
}







