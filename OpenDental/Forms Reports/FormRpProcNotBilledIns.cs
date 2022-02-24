using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Data;
using OpenDental.ReportingComplex;
using OpenDental.UI;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using CodeBase;

namespace OpenDental{
	///<summary></summary>
	public partial class FormRpProcNotBilledIns : FormODBase {
		private ReportComplex _myReport;
		private decimal _procTotalAmt;
		private DateTime _myReportDateFrom;
		private DateTime _myReportDateTo;
		private const int _colWidthPatName=200;
		private const int _colWidthStat=45;
		private const int _colWidthProcDate=110;
		private const int _colWidthAmount=90;
		private const int _colWidthClinic=75;
		private DateTime _dateFromPrev=DateTime.MaxValue;
		private DateTime _dateToPrev=DateTime.MaxValue;
		///<summary>Called when claims are created, only executes if at least 1 claim was created.</summary>
		public event OnPostClaimCreationHandler OnPostClaimCreation=null;

		///<summary></summary>
		public FormRpProcNotBilledIns(){
			InitializeComponent();
			InitializeLayoutManager();
 			Lan.F(this);
		}

		private void FormProcNotAttach_Load(object sender, System.EventArgs e) {
			gridMain.ContextMenu=contextMenuGrid;
			dateRangePicker.SetDateTimeTo(DateTime.Today);
			dateRangePicker.SetDateTimeFrom(DateTime.Today);
			if(PrefC.GetBool(PrefName.ShowFeatureMedicalInsurance)) {
				checkMedical.Visible=true;
			}
			if(PrefC.GetBool(PrefName.ClaimProcsNotBilledToInsAutoGroup)) {
				checkAutoGroupProcs.Checked=true;
			}
			FillGrid();
		}
		
		public void FillGrid() {
			RefreshReport();
			gridMain.BeginUpdate();
			GridColumn col=null;
			if(gridMain.ListGridColumns.Count==0) {
				col=new GridColumn(Lan.g(this,"Patient Name"),_colWidthPatName);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Stat"),_colWidthStat,HorizontalAlignment.Center);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Procedure Date"),_colWidthProcDate,HorizontalAlignment.Center);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Procedure Description"),410) { IsWidthDynamic=true };
				gridMain.ListGridColumns.Add(col);
				if(PrefC.HasClinicsEnabled) {
					col=new GridColumn(Lan.g(this,"Clinic"),_colWidthClinic);
					gridMain.ListGridColumns.Add(col);
				}
				col=new GridColumn(Lan.g(this,"Amount"),_colWidthAmount,HorizontalAlignment.Right);
				gridMain.ListGridColumns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_myReport.ReportObjects.Count;i++) {
				if(_myReport.ReportObjects[i].ObjectType!=ReportObjectType.QueryObject) {
					continue;
				}
				QueryObject queryObj=(QueryObject)_myReport.ReportObjects[i];
				for(int j=0;j<queryObj.ReportTable.Rows.Count;j++) {
					row=new GridRow();
					row.Cells.Add(queryObj.ReportTable.Rows[j][0].ToString());//Procedure Name
					row.Cells.Add(Lan.g("enumProcStat",PIn.String(queryObj.ReportTable.Rows[j][1].ToString())));//Stat
					row.Cells.Add(PIn.Date(queryObj.ReportTable.Rows[j][2].ToString()).ToShortDateString());//Procedure Date
					row.Cells.Add(queryObj.ReportTable.Rows[j][3].ToString());//Procedure Description
					if(PrefC.HasClinicsEnabled) {
						long clinicNum=PIn.Long(queryObj.ReportTable.Rows[j][6].ToString());
						if(clinicNum==0) {
							row.Cells.Add("Unassigned");
						}
						else {
							row.Cells.Add(Clinics.GetAbbr(clinicNum));
						}
					}
					row.Cells.Add(PIn.Double(queryObj.ReportTable.Rows[j][4].ToString()).ToString("c"));//Amount
					_procTotalAmt+=PIn.Decimal(queryObj.ReportTable.Rows[j][4].ToString());
					row.Tag=((QueryObject)_myReport.ReportObjects[i]).ReportTable.Rows[j];
					gridMain.ListGridRows.Add(row);
				}
			}
			gridMain.EndUpdate();
		}

		//Only called in FillGrid().
		private void RefreshReport() {
			bool hasValidationPassed=ValidateFields();
			//Above line also sets "All" clinics option if no items are selected.
			DataTable tableNotBilled=new DataTable();
			if(hasValidationPassed) {
				//not truly all clinics; just the ones user has permission for
				tableNotBilled=RpProcNotBilledIns.GetProcsNotBilled(comboClinics.ListSelectedClinicNums,checkMedical.Checked,_myReportDateFrom,_myReportDateTo,
					checkShowProcsNoIns.Checked,checkShowProcsInProcess.Checked);
			}
			string subtitleClinics="";
			if(PrefC.HasClinicsEnabled) {
				subtitleClinics=Lan.g(this,"Clinics: ")+comboClinics.GetStringSelectedClinics();
			}
			_myReport=new ReportComplex(true,false);
			_myReport.ReportName=Lan.g(this,"Procedures Not Billed to Insurance");
			_myReport.AddTitle("Title",Lan.g(this,"Procedures Not Billed to Insurance"));
			_myReport.AddSubTitle("Practice Name",PrefC.GetString(PrefName.PracticeTitle));
			if(_myReportDateFrom==_myReportDateTo) {
				_myReport.AddSubTitle("Report Dates",_myReportDateFrom.ToShortDateString());
			}
			else {
				_myReport.AddSubTitle("Report Dates",_myReportDateFrom.ToShortDateString()+" - "+_myReportDateTo.ToShortDateString());
			}
			if(PrefC.HasClinicsEnabled) {
				_myReport.AddSubTitle("Clinics",subtitleClinics);
			}
			QueryObject query=_myReport.AddQuery(tableNotBilled,DateTime.Today.ToShortDateString());
			query.AddColumn("Patient Name",_colWidthPatName,FieldValueType.String);
			query.AddColumn("Stat",_colWidthStat,FieldValueType.String);
			query.AddColumn("Procedure Date",_colWidthProcDate,FieldValueType.Date);
			query.GetColumnDetail("Procedure Date").StringFormat="d";
			query.AddColumn("Procedure Description",300,FieldValueType.String);
			query.AddColumn("Amount",_colWidthAmount,FieldValueType.Number);
			_myReport.AddPageNum();
			_myReport.SubmitQueries();
		}
		
		//Only called in RefreshReport().
		private bool ValidateFields() {
			_myReportDateFrom=dateRangePicker.GetDateTimeFrom();
			_myReportDateTo=dateRangePicker.GetDateTimeTo();
			if(_myReportDateFrom>_myReportDateTo) {
				_myReportDateFrom=DateTime.MinValue;
				_myReportDateTo=DateTime.MaxValue;
			}
			if(PrefC.HasClinicsEnabled) {
				if(comboClinics.ListSelectedClinicNums.Count==0){
					comboClinics.IsAllSelected=true;
				}
			}
			if(_myReportDateFrom==DateTime.MinValue || _myReportDateTo==DateTime.MinValue) {
				return false;
			}
			return true;
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}
		
		private void butPrint_Click(object sender,EventArgs e) {
			using FormReportComplex FormR=new FormReportComplex(_myReport);
			FormR.ShowDialog();
		}
		
		private void butSelectAll_Click(object sender,EventArgs e) {
			gridMain.SetAll(true);
		}

		private void butNewClaims_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {//No selections made.
				MsgBox.Show(this,"Please select at least one procedure.");
				return;
			}
			if(!ClaimL.CheckClearinghouseDefaults()) {
				return;
			}
			//Ignore date lock for now, we just want to check the general permission for the button.
			if(!Security.IsAuthorized(Permissions.NewClaimsProcNotBilled,DateTime.Today)) {
				return;
			}
			//Generate List and Table----------------------------------------------------------------------------------------------------------------------
			//List of all procedures being shown.
			//Pulls procedures based off of the PatNum, if the row was selected in gridMain and if it has been attached to a claim.
			List<ProcNotBilled> listNotBilledProcs=new List<ProcNotBilled>();
			List<long> listPatNums=new List<long>();
			Patient patOld=new Patient();
			List<Claim> listPatClaims=new List<Claim>();
			List<ClaimProc> listPatClaimProcs=new List<ClaimProc>();
			List<ClaimProc> listCurClaimProcs=new List<ClaimProc>();
			//find the date user is restricted by for this permission so it doesn't get called in a loop. General permission was already checked.
			DateTime dateRestricted=GroupPermissions.GetDateRestrictedForPermission(Permissions.NewClaimsProcNotBilled,
				Security.CurUser.GetGroups(true).Select(x => x.UserGroupNum).ToList());
			//Table rows need to be 1:1 with gridMain rows due to logic in ContrAccount.toolBarButIns_Click(...).
			DataTable table=new DataTable();
			//Required columns as mentioned by ContrAccount.toolBarButIns_Click().
			table.Columns.Add("ProcNum");
			table.Columns.Add("chargesDouble");
			table.Columns.Add("ProcNumLab");
			List<long> listProcNumsPastLockDate=new List<long>();
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {//Loop through every row in gridMain to construct datatable and listNotBilledProcs.
				//Table is passed to toolBarButIns_Click(...) and must contain data for every row in the grid.
				DataRow rowCur=(DataRow)gridMain.ListGridRows[i].Tag;
				long procNumCur=PIn.Long(rowCur["ProcNum"].ToString());
				Procedure procCur=Procedures.GetOneProc(procNumCur,false);
				if(procCur.ProcDate <= dateRestricted) {//current procedure is past or on the lock date. 
					listProcNumsPastLockDate.Add(procNumCur);
				}
				long patNumCur=procCur.PatNum;
				if(patOld.PatNum!=patNumCur) {//Procedures in gridMain are ordered by patient, so when the patient changes, we know previous patient is complete.
					listPatClaims=Claims.Refresh(patNumCur);
					listPatClaimProcs=ClaimProcs.Refresh(patNumCur);
					patOld=Patients.GetPat(procCur.PatNum);
				}
				listCurClaimProcs=ClaimProcs.GetForProc(listPatClaimProcs,procNumCur);
				bool hasPriClaim=false;
				bool hasSecClaim=false;
				for(int j=0;j<listCurClaimProcs.Count;j++) {
					ClaimProc claimProcCur=listCurClaimProcs[j];
					if(claimProcCur.ClaimNum > 0 && claimProcCur.Status!=ClaimProcStatus.Preauth && claimProcCur.Status!=ClaimProcStatus.Estimate) {
						Claim claimCur=Claims.GetFromList(listPatClaims,claimProcCur.ClaimNum);
						switch(claimCur.ClaimType) {
							case "P":
								hasPriClaim=true;
								break;
							case "S":
								hasSecClaim=true;
								break;
						}
					}
				}
				bool isSelected=gridMain.SelectedIndices.Contains(i);
				listNotBilledProcs.Add(new ProcNotBilled(patOld,procNumCur,i,isSelected,hasPriClaim,hasSecClaim,procCur.ClinicNum,procCur.PlaceService));
				DataRow row=table.NewRow();
				row["ProcNum"]=procNumCur;
				#region Calculate chargesDouble
				double writeOffCapSum=listPatClaimProcs.Where(x => x.Status==ClaimProcStatus.CapComplete).Sum(y => y.WriteOff);
				row["chargesDouble"]=procCur.ProcFeeTotal-writeOffCapSum;
				row["ProcNumLab"]=procCur.ProcNumLab;
				#endregion Calculate chargesDouble
				table.Rows.Add(row);
				if(listPatNums.Contains(patNumCur)) {
					continue;
				}
				listPatNums.Add(patNumCur);
			}
			List<List<ProcNotBilled>> listGroupedProcs=new List<List<ProcNotBilled>>();
			Patient patCur=null;
			List<PatPlan> listPatPlans=null;
			List<InsSub> listInsSubs=null;
			List<InsPlan> listInsPlans=null;
			List<Procedure> listPatientProcs=null;
			ProcNotBilled procNotBilled=new ProcNotBilled();//When automatically grouping,  this is used as the procedure to group by.
			long patNumOld=0;
			int claimCreatedCount=0;
			int patIndex=0;
			//The procedures show in the grid ordered by patient.  Also listPatNums contains unique patnums which are in the same order as the grid.
			while(patIndex < listPatNums.Count) {
				List<ProcNotBilled> listProcs=listNotBilledProcs.Where(x => x.Patient.PatNum==listPatNums[patIndex] && x.IsRowSelected && !x.IsAttached 
					&& !ListTools.In(x.ProcNum,listProcNumsPastLockDate)).ToList();
				if(listProcs.Count==0) {
					patNumOld=listPatNums[patIndex];
					patIndex++;//No procedures were selected for this patient.
					continue;
				}
				else {
					//Maintain the same patient, in order to create one or more additional claims for the remaining procedures.
					//Currently will only happen for specific instances; 
					//--Canadian customers who are attempting to create a claim with over 7 procedures.
					//--When checkAutoGroupProcs is checked and when there are multiple procedure groupings by GroupKey status, ClinicNum, and placeService.
				}
				if(patNumOld!=listPatNums[patIndex]) {//The patient could repeat if we had to group the procedures for the patinet into multiple claims.
					patCur=Patients.GetPat(listPatNums[patIndex]);
					listPatPlans=PatPlans.Refresh(patCur.PatNum);
					listInsSubs=InsSubs.RefreshForFam(Patients.GetFamily(patCur.PatNum));
					listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
					listPatientProcs=Procedures.Refresh(patCur.PatNum);
				}
				if(checkAutoGroupProcs.Checked) {//Automatically Group Procedures.
					procNotBilled=listProcs[0];
					//Update listProcs to reflect those that match the procNotBilled values.
					listProcs=listProcs.FindAll(x => x.HasPriClaim==procNotBilled.HasPriClaim && x.HasSecClaim==procNotBilled.HasSecClaim);
					if(PrefC.HasClinicsEnabled) {//Group by clinic only if clinics enabled.
						listProcs=listProcs.FindAll(x => x.ClinicNum==procNotBilled.ClinicNum);
					}
					else if(!PrefC.GetBool(PrefName.EasyHidePublicHealth)) {//Group by Place of Service only if Public Health feature is enabled.
						listProcs=listProcs.FindAll(x => x.PlaceService==procNotBilled.PlaceService);
					}
				}
				GetUniqueDiagnosticCodes(listProcs,listPatientProcs,listPatPlans,listInsSubs,listInsPlans);
				if(listProcs.Count>7 && CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					listProcs=listProcs.Take(7).ToList();//Returns first 7 items of the list.
				}
				listProcs.ForEach(x => x.IsAttached=true);//This way we can not attach procedures to multiple claims thanks to the logic above.
				if(listProcs.Any(x => listProcs[0].PlaceService!=x.PlaceService) || listProcs.Any(x => listProcs[0].ClinicNum!=x.ClinicNum)) {
					//Regardless if we are automatically grouping or not,
					//if all procs in our list at this point do not share the same PlaceService or ClinicNum then claims will not be made.
				}
				else {//Basic validation passed.
					if(!listProcs[0].HasPriClaim //Medical claim.
						&& PatPlans.GetOrdinal(PriSecMed.Medical,listPatPlans,listInsPlans,listInsSubs)>0 //Has medical ins.
						&& PatPlans.GetOrdinal(PriSecMed.Primary,listPatPlans,listInsPlans,listInsSubs)==0 //Does not have primary dental ins.
						&& PatPlans.GetOrdinal(PriSecMed.Secondary,listPatPlans,listInsPlans,listInsSubs)==0) //Does not have secondary dental ins.
					{
						claimCreatedCount++;
					}
					else {//Not a medical claim.
						if(!listProcs[0].HasPriClaim&&PatPlans.GetOrdinal(PriSecMed.Primary,listPatPlans,listInsPlans,listInsSubs)>0) {//Primary claim.
							claimCreatedCount++;
						}
						if(!listProcs[0].HasSecClaim&&PatPlans.GetOrdinal(PriSecMed.Secondary,listPatPlans,listInsPlans,listInsSubs)>0) {//Secondary claim.
							claimCreatedCount++;
						}
					}
				}
				listGroupedProcs.Add(listProcs);
			}
			if(claimCreatedCount<=0 && listProcNumsPastLockDate.Count>0) {//No claims can be created because of the lock date.
				MsgBox.Show(this,"No claims can be created because all procedure dates extend past the lock date for this report.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Clicking OK will create up to "+POut.Int(claimCreatedCount)
				+" claims and cannot be undone, except by manually going to each account.  "
				+"Some claims may not be created if there are validation issues.\r\n"
				+"Click OK to continue, otherwise click Cancel."))
			{
				return;
			}
			//Create Claims--------------------------------------------------------------------------------------------------------------------------------
			claimCreatedCount=0;
			string claimErrors="";
			foreach(List<ProcNotBilled> listProcs in listGroupedProcs) { 
				patCur=listProcs[0].Patient;
				gridMain.SetAll(false);//Need to deslect all rows each time so that ContrAccount.toolBarButIns_Click(...) only uses pertinent rows.
				for(int j=0;j<listProcs.Count;j++) {
					gridMain.SetSelected(listProcs[j].RowIndex,true);//Select the pertinent rows so that they will be attached to the claim below.
				}
				int[] arraySelectedIndices=(int[])gridMain.SelectedIndices.Clone();
				CreateClaimDataWrapper createClaimDataWrapper=ClaimL.GetCreateClaimDataWrapper(patCur,Patients.GetFamily(patCur.PatNum)
					,ClaimL.GetCreateClaimItems(table,arraySelectedIndices),false);
				createClaimDataWrapper=ClaimL.CreateClaimFromWrapper(false,createClaimDataWrapper,procNotBilled.HasPriClaim,procNotBilled.HasSecClaim);
				string errorTitle=patCur.PatNum+" "+patCur.GetNameLFnoPref()+" - ";
				if(patNumOld==patCur.PatNum && !string.IsNullOrEmpty(createClaimDataWrapper.ErrorMessage)) {
					claimErrors+="\t\t"+createClaimDataWrapper.ErrorMessage+"\r\n";
				}
				else if(!string.IsNullOrEmpty(createClaimDataWrapper.ErrorMessage)) {
					claimErrors+=errorTitle+createClaimDataWrapper.ErrorMessage+"\r\n";
				}
				claimCreatedCount+=createClaimDataWrapper.ClaimCreatedCount;
				patNumOld=patCur.PatNum;
			}
			FillGrid();
			if(claimCreatedCount>0) {
				OnPostClaimCreation?.Invoke();
			}
			if(!string.IsNullOrEmpty(claimErrors)) {
				using MsgBoxCopyPaste form=new MsgBoxCopyPaste(claimErrors);
				form.ShowDialog();
			}
			MessageBox.Show(Lan.g(this,"Number of claims created")+": "+claimCreatedCount);
		}
		
		///<summary>Mimics ContrAccount.CreateClaim(...).  Removes items from listProcs until unique diagnosis code count is low enough.</summary>
		private void GetUniqueDiagnosticCodes(List<ProcNotBilled> listProcs,List<Procedure> listPatProcs,List <PatPlan> listPatPlans,
			List<InsSub> listInsSubs,List<InsPlan> listInsPlans)
		{
			List<Procedure> listProcedures=new List<Procedure>();
			for(int i=0;i<listProcs.Count;i++) {
				listProcedures.Add(Procedures.GetProcFromList(listPatProcs,listProcs[i].ProcNum));
			}
			//If they have medical insurance and no dental, make the claim type Medical.  This is to avoid the scenario of multiple med ins and no dental.
			bool isMedical=false;
			if(PatPlans.GetOrdinal(PriSecMed.Medical,listPatPlans,listInsPlans,listInsSubs)>0
				&& PatPlans.GetOrdinal(PriSecMed.Primary,listPatPlans,listInsPlans,listInsSubs)==0
				&& PatPlans.GetOrdinal(PriSecMed.Secondary,listPatPlans,listInsPlans,listInsSubs)==0)
			{
				isMedical=true;
			}
			while(!isMedical && Procedures.GetUniqueDiagnosticCodes(listProcedures,false).Count > 4) {//dental
				int index=listProcedures.Count-1;
				listProcedures.RemoveAt(index);
				listProcs.RemoveAt(index);
			}
			while(isMedical && Procedures.GetUniqueDiagnosticCodes(listProcedures,true).Count > 12) {//medical
				int index=listProcedures.Count-1;
				listProcedures.RemoveAt(index);
				listProcs.RemoveAt(index);
			}
		}

		private void menuItemGridGoToAccount_Click(object sender,EventArgs e) {
			//accessed by right clicking the history grid
			if(gridMain.SelectedIndices.Length!=1) {
				MsgBox.Show(this,"Please select exactly one item first.");
				return;
			}
			DataRow row=(DataRow)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			long patNum=PIn.Long(row["PatNum"].ToString());
			if(patNum==0) {
				MsgBox.Show(this,"Please select an item with a patient.");
				return;
			}
			ODEvent.Fire(ODEventType.FormProcNotBilled_GoTo,patNum);
			SendToBack();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		private void FormRpProcNotBilledIns_FormClosing(object sender,FormClosingEventArgs e) {
			Prefs.UpdateBool(PrefName.ClaimProcsNotBilledToInsAutoGroup,checkAutoGroupProcs.Checked);
		}

		public delegate void OnPostClaimCreationHandler();

	}//end class FormRpProcNotBilledIns

	///<summary>Used so that we can easily select pertinent procedures for a specific patient when creating claims.</summary>
	internal class ProcNotBilled {
		public Patient Patient;
		public long ProcNum;
		public int RowIndex;
		public bool IsRowSelected;
		///<summary>Flag used to make sure we do not attach procedures to multiple claims.
		///Very important for Canadian customers when we need to make multiple claims.</summary>
		public bool IsAttached;
		public bool HasPriClaim;
		public bool HasSecClaim;
		public long ClinicNum;
		public PlaceOfService PlaceService;

		public ProcNotBilled() {
			HasPriClaim=false;
			HasSecClaim=false;
		}

		public ProcNotBilled(Patient pat,long procNum,int rowIndex,bool isRowSelected,
			bool hasPriClaim,bool hasSecClaim,long clinicNum,PlaceOfService placeService)
		{
			Patient=pat;
			ProcNum=procNum;
			RowIndex=rowIndex;
			IsRowSelected=isRowSelected;
			IsAttached=false;
			HasPriClaim=hasPriClaim;
			HasSecClaim=hasSecClaim;
			ClinicNum=clinicNum;
			PlaceService=placeService;
		}
	}

}