using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMedLabResultHist:FormODBase {
		public Patient PatientCur;
		public MedLabResult MedLabResultCur;

		public FormMedLabResultHist() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMedLabResultHist_Load(object sender, EventArgs e) {
			FillGridResultHist();
		}

		private void FillGridResultHist() {
			MedLab medLab=MedLabs.GetOne(MedLabResultCur.MedLabNum);
			if(medLab==null) {//should never happen, but we must have a MedLab object to fill the grid
				return;
			}
			gridResultHist.BeginUpdate();
			gridResultHist.Columns.Clear();
			GridColumn col;
			col=new GridColumn("Result Description / Value",425);
			gridResultHist.Columns.Add(col);
			col=new GridColumn("Flag",110);
			gridResultHist.Columns.Add(col);
			col=new GridColumn("Units",65);
			gridResultHist.Columns.Add(col);
			col=new GridColumn("Date/Time Reported",130);//OBR-22, Date/Time Observations Reported
			col.SortingStrategy=GridSortingStrategy.DateParse;
			gridResultHist.Columns.Add(col);
			col=new GridColumn("Date/Time Observed",130);//OBX-11, Date/Time of Observation
			col.SortingStrategy=GridSortingStrategy.DateParse;
			gridResultHist.Columns.Add(col);
			col=new GridColumn("Status",70);
			gridResultHist.Columns.Add(col);
			gridResultHist.ListGridRows.Clear();
			GridRow row;
			long patNum=0;
			if(PatientCur!=null) {
				patNum=PatientCur.PatNum;
			}
			List<MedLabResult> listMedLabResults=MedLabResults.GetResultHist(MedLabResultCur,patNum,medLab.SpecimenID,medLab.SpecimenIDFiller);
			for(int i=0;i<listMedLabResults.Count;i++) {
				row=new GridRow();
				string obsVal=listMedLabResults[i].ObsText;
				if(listMedLabResults[i].ObsValue!="Test Not Performed") {
					obsVal+="\r\n    "+listMedLabResults[i].ObsValue.Replace("\r\n","\r\n    ");
				}
				if(listMedLabResults[i].Note!="") {
					obsVal+="\r\n    "+listMedLabResults[i].Note.Replace("\r\n","\r\n    ");
				}
				row.Cells.Add(obsVal);
				row.Cells.Add(MedLabResults.GetAbnormalFlagDescript(listMedLabResults[i].AbnormalFlag));
				row.Cells.Add(listMedLabResults[i].ObsUnits);
				medLab=MedLabs.GetOne(listMedLabResults[i].MedLabNum);
				string dateReported="";
				if(medLab!=null) {
					dateReported=medLab.DateTimeReported.ToString("MM/dd/yyyy hh:mm tt");//DT format matches LabCorp examples (US only company)
				}
				row.Cells.Add(dateReported);
				row.Cells.Add(listMedLabResults[i].DateTimeObs.ToString("MM/dd/yyyy hh:mm tt"));//DT format matches LabCorp examples (US only company)
				row.Cells.Add(MedLabs.GetStatusDescript(listMedLabResults[i].ResultStatus));
				gridResultHist.ListGridRows.Add(row);
			}
			gridResultHist.EndUpdate();
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}
		
	}
}
