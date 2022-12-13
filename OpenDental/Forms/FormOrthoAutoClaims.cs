using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormOrthoAutoClaims:FormODBase {
		///<summary>OutstandingAutoClaims</summary>
		private DataTable _table;

		public FormOrthoAutoClaims() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAutoOrtho_Load(object sender,EventArgs e) {
			_table=PatPlans.GetOutstandingOrtho();
			if(!Security.CurUser.ClinicIsRestricted) {
				comboClinics.IncludeAll=true;
			}
			if(comboClinics.IncludeAll && Clinics.ClinicNum==0) {
				comboClinics.IsAllSelected=true;
			}
			else {
				comboClinics.SelectedClinicNum=Clinics.ClinicNum;
			}
			FillGrid();
		}

		private void FillGrid() {
			long clinicNum = 0;
			if(!comboClinics.IsAllSelected) {
				clinicNum=comboClinics.SelectedClinicNum;
			}
			int clinicWidth=80;
			int patientWidth=180;
			int carrierWidth=220;
			if(PrefC.HasClinicsEnabled) {
				patientWidth=140;
				carrierWidth=200;
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableAutoOrthoClaims","Patient"),patientWidth);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableAutoOrthoClaims","Carrier"),carrierWidth);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableAutoOrthoClaims","TxMonths"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableAutoOrthoClaims","Banding"),80,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableAutoOrthoClaims","MonthsRem"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableAutoOrthoClaims","#Sent"),60);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableAutoOrthoClaims","LastSent"),80,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableAutoOrthoClaims","NextClaim"),80,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) { //clinics is turned on
				col=new GridColumn(Lan.g("TableAutoOrthoClaims","Clinic"),clinicWidth,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_table.Rows.Count;i++) {
				//need a check for if clinics is on here
				if(PrefC.HasClinicsEnabled //Clinics are enabled
					&& (Security.CurUser.ClinicIsRestricted || !comboClinics.IsAllSelected) 
					&& clinicNum!=PIn.Long(_table.Rows[i]["ClinicNum"].ToString()))   //currently selected clinic doesn't match the row's clinic
				{
					continue;
				}
				row=new GridRow();
				DateTime dateLastSeen=PIn.Date(_table.Rows[i]["LastSent"].ToString());
				DateTime dateBanding=PIn.Date(_table.Rows[i]["DateBanding"].ToString());
				DateTime dateNextClaim=PIn.Date(_table.Rows[i]["OrthoAutoNextClaimDate"].ToString());
				DateSpan dateSpanMonthsRem=new DateSpan(PIn.Date(_table.Rows[i]["DateBanding"].ToString()).AddMonths(PIn.Int(_table.Rows[i]["MonthsTreat"].ToString())),DateTime.Today);
				row.Cells.Add(PIn.String(_table.Rows[i]["Patient"].ToString()));
				row.Cells.Add(PIn.String(_table.Rows[i]["CarrierName"].ToString()));
				row.Cells.Add(PIn.String(_table.Rows[i]["MonthsTreat"].ToString()));
				row.Cells.Add(dateBanding.Year < 1880 ? "" : dateBanding.ToShortDateString());//add blank if there is no banding
				if(dateBanding.Year < 1880) { //add blank if there is no banding
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(((dateSpanMonthsRem.YearsDiff * 12) + dateSpanMonthsRem.MonthsDiff)+" "+Lan.g(this,"months")
					+", "+dateSpanMonthsRem.DaysDiff +" "+Lan.g(this,"days"));
				}
				row.Cells.Add(PIn.String(_table.Rows[i]["NumSent"].ToString()));
				row.Cells.Add(dateLastSeen.Year < 1880 ? "" : dateLastSeen.ToShortDateString());
				row.Cells.Add(dateNextClaim.Year < 1880 ? "" : dateNextClaim.ToShortDateString());
				if(PrefC.HasClinicsEnabled) { //clinics is turned on
					//Use the long list of clinics so that hidden clinics can be shown for unrestricted users.
					row.Cells.Add(Clinics.GetAbbr(PIn.Long(_table.Rows[i]["ClinicNum"].ToString())));
				}
				row.Tag=_table.Rows[i];
				gridMain.ListGridRows.Add(row);

			}
			gridMain.EndUpdate();
		}

		private void butGenerateClaims_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Count() < 1) {
				MsgBox.Show(this,"Please select the rows for which you would like to create procedures and claims.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you want to generate claims and procedures for all patients and insurance plans?")) {
				return;
			}
			List<long> listPlanNums = new List<long>();
			List<long> listPatPlanNums = new List<long>();
			List<long> listInsSubNums = new List<long>();
			for(int i = 0;i < gridMain.SelectedIndices.Count();i++) {
				DataRow row =(DataRow)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag;
				listPlanNums.Add(PIn.Long(row["PlanNum"].ToString()));
				listPatPlanNums.Add(PIn.Long(row["PatPlanNum"].ToString()));
				listInsSubNums.Add(PIn.Long(row["InsSubNum"].ToString()));
			}
			List<InsPlan> listInsPlansSelected=InsPlans.GetPlans(listPlanNums);
			List<PatPlan> listPatPlansSelected=PatPlans.GetPatPlans(listPatPlanNums);
			List<InsSub> listInsSubsSelected=InsSubs.GetMany(listInsSubNums);
			List<DataRow> listRowsSucceeded=new List<DataRow>();
			int rowsFailed=0;
			List<Benefit> listBenefitsAll=Benefits.Refresh(listPatPlansSelected,listInsSubsSelected);
			List<string> listHiddenProcCodes=new List<string>();
			for(int i = 0;i < gridMain.SelectedIndices.Count();i++) {
				DataRow row =(DataRow)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag;
				long patNum = PIn.Long(row["PatNum"].ToString());
				Patient patient = Patients.GetPat(patNum);
				PatientNote patientNote = PatientNotes.Refresh(patNum,patient.Guarantor);
				long codeNum = PIn.Long(row["AutoCodeNum"].ToString());
				long provNum = PIn.Long(row["ProvNum"].ToString());
				long clinicNum = PIn.Long(row["ClinicNum"].ToString());
				long insPlanNum = PIn.Long(row["PlanNum"].ToString());
				long patPlanNum = PIn.Long(row["PatPlanNum"].ToString());
				long insSubNum = PIn.Long(row["InsSubNum"].ToString());
				int monthsTreat = PIn.Int(row["MonthsTreat"].ToString());
				DateTime dateTimeDue =  PIn.Date(row["OrthoAutoNextClaimDate"].ToString());
				string procCode=ProcedureCodes.GetProcCode(codeNum).ProcCode;
				if(!listHiddenProcCodes.Contains(procCode) && ProcedureCodes.AreAnyProcCodesHidden(codeNum)) {
					listHiddenProcCodes.Add(procCode);
				}
				//for each selected row
				//create a procedure
				//Procedures.CreateProcForPat(patNumCur,codeNumCur,"","",ProcStat.C,provNumCur);
				if(ProcedureCodes.AreAnyProcCodesHidden(codeNum)) {
					//We do not show a message here because it would be annoying in a loop.
					//Todo: show the message somehow at the end.
					//MsgBox.Show($"Cannot create auto ortho procedure because procedure is in a hidden category: {ProcedureCodes.GetProcCode(codeNum)}");
					rowsFailed++;
					continue;
				}
				Procedure procedure = Procedures.CreateOrthoAutoProcsForPat(patNum,codeNum,provNum,clinicNum,dateTimeDue);
				InsPlan insPlan = InsPlans.GetPlan(insPlanNum,listInsPlansSelected);
				PatPlan patPlan = listPatPlansSelected.FirstOrDefault(x => x.PatPlanNum == patPlanNum);
				InsSub insSub = listInsSubsSelected.FirstOrDefault(x => x.InsSubNum==insSubNum);
				List<Benefit> listBenefits=listBenefitsAll.FindAll(x => x.PatPlanNum==patPlan.PatPlanNum || x.PlanNum==insSub.PlanNum);
				//create a claimproc
				List<ClaimProc> listClaimProcs=new List<ClaimProc>();
				Procedures.ComputeEstimates(procedure,patNum,ref listClaimProcs,true,new List<InsPlan> { insPlan },
					new List<PatPlan> { patPlan },listBenefits,null,null,true,patient.Age,new List<InsSub> { insSub },isForOrtho:true);
				//make the feebilled == the insplan feebilled or patplan feebilled
				double feeBilled = patPlan.OrthoAutoFeeBilledOverride == -1 ? insPlan.OrthoAutoFeeBilled : patPlan.OrthoAutoFeeBilledOverride;
				//create a claim with that claimproc
				string claimType="";
				switch(patPlan.Ordinal) {
					case 1:
						claimType="P";
						break;
					case 2:
						claimType="S";
						break;
				}
				DateSpan dateSpanMonthsRem=new DateSpan(PIn.Date(row["DateBanding"].ToString()).AddMonths(PIn.Int(row["MonthsTreat"].ToString())),DateTime.Today);
				Claims.CreateClaimForOrthoProc(claimType,patPlan,insPlan,insSub,
					ClaimProcs.GetForProcWithOrdinal(procedure.ProcNum,patPlan.Ordinal),procedure,feeBilled,PIn.Date(row["DateBanding"].ToString()),
					PIn.Int(row["MonthsTreat"].ToString()),((dateSpanMonthsRem.YearsDiff * 12) + dateSpanMonthsRem.MonthsDiff));
				PatPlans.IncrementOrthoNextClaimDates(patPlan,insPlan,monthsTreat,patientNote);
				listRowsSucceeded.Add(row);
				SecurityLogs.MakeLogEntry(Permissions.ProcComplCreate,patient.PatNum
						,Lan.g(this,"Automatic ortho procedure and claim generated for")+" "+dateTimeDue.ToShortDateString());
			}
			string message=Lan.g(this,"Done.")+" "+Lan.g(this,"There were")+" "+listRowsSucceeded.Count+" "
				+Lan.g(this,"claim(s) generated and")+" "+rowsFailed+" "+Lan.g(this,"failures")+".";
			if(listHiddenProcCodes.Count > 0) {
				message+="\r\n"+Lan.g(this,"Some failed because the following procedures are in a hidden category")+$": {string.Join(", ",listHiddenProcCodes)}";
			}
			MsgBox.Show(message);
			for(int i=0;i<listRowsSucceeded.Count;i++) {
				_table.Rows.Remove(listRowsSucceeded[i]);
			}
			FillGrid();
		}

		private void butSelectAll_Click(object sender,EventArgs e) {
			gridMain.SetAll(true);
		}

		private void comboClinics_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}
	}
}