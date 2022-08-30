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
		public List<ClaimProc> ListClaimProcs;
		private List<Procedure> _listProcedures;
		private Patient _patient;
		private Family _family;
		private List<InsPlan> _listInsPlans;
		private List<PatPlan> _listPatPlans;
		private List<InsSub> _listInsSubs;

		///<summary></summary>
		public FormClaimPayPreAuth(Patient patient,Family family,List<InsPlan> listInsPlans,List<PatPlan> listPatPlans,List<InsSub> listInsSubs) {
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			_family=family;
			_patient=patient;
			_listInsPlans=listInsPlans;
			_listInsSubs=listInsSubs;
			_listPatPlans=listPatPlans;
			Lan.F(this);
		}

		private void FormClaimPayPreAuth_Load(object sender,EventArgs e) {
			_listProcedures=Procedures.Refresh(_patient.PatNum);
			FillGrid();
		}

		private void FormClaimPayTotal_Shown(object sender,EventArgs e) {
			int idxToothOffset=0;
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				idxToothOffset=1;
			}
			//InsPlan insPlan=InsPlans.GetPlan(ListClaimProcs[0].PlanNum,_listInsPlans);
			gridMain.SetSelected(new Point(4-idxToothOffset,0));
		}

		private void FillGrid(){
			//Changes made in this window do not get saved until after this window closes.
			//But if you double click on a row, then you will end up saving.  That shouldn't hurt anything, but could be improved.
			//also calculates totals for this "payment"
			//the payment itself is imaginary and is simply the sum of the claimprocs on this form
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				col=new GridColumn(Lan.g(this,"Code"),85);
				gridMain.Columns.Add(col);
			}
			else {
				col=new GridColumn(Lan.g(this,"Code"),50);
				gridMain.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"Tth"),35);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Description"),120);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Fee"),55,HorizontalAlignment.Right);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Estimate"),55,HorizontalAlignment.Right,isEditable:true);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Remarks"),170,isEditable:true);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			for(int i=0;i<ListClaimProcs.Count;i++){
				GridRow row=new GridRow();
				//for pre-auths, there are no total payments, so ProcNum must be >0
				Procedure procedure=Procedures.GetProcFromList(_listProcedures,ListClaimProcs[i].ProcNum);
				row.Cells.Add(ProcedureCodes.GetProcCode(procedure.CodeNum).ProcCode);
				if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
					row.Cells.Add(Tooth.Display(procedure.ToothNum));
				}
				row.Cells.Add(ProcedureCodes.GetProcCode(procedure.CodeNum).Descript);
				row.Cells.Add(ListClaimProcs[i].FeeBilled.ToString("F"));
				row.Cells.Add(ListClaimProcs[i].InsPayEst.ToString("F"));
				row.Cells.Add(ListClaimProcs[i].Remarks);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			FillTotals();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			try{
				SaveGridChanges();
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			List<ClaimProcHist> listClaimProcHists=null;
			List<ClaimProcHist> listClaimProcHistsLoop=null;
			using FormClaimProc formClaimProc=new FormClaimProc(ListClaimProcs[e.Row],procedure:null,_family,_patient,_listInsPlans,listClaimProcHists,ref listClaimProcHistsLoop,_listPatPlans,doSaveToDb:false,_listInsSubs);
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
			double insPayEst=0;
			int idxToothOffset=0;
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				idxToothOffset=1;
			}
			for(int i=0;i<gridMain.ListGridRows.Count;i++){
				try {
					insPayEst+=Convert.ToDouble(gridMain.ListGridRows[i].Cells[4-idxToothOffset].Text);
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
			for(int i=0;i<ListClaimProcs.Count;i++){
				ListClaimProcs[i].InsPayEst=PIn.Double(gridMain.ListGridRows[i].Cells[4-toothIndexOffset].Text);
				ListClaimProcs[i].Remarks=gridMain.ListGridRows[i].Cells[5-toothIndexOffset].Text;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
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

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	
		

	
		

		

		

		

		

		

		



	}
}







