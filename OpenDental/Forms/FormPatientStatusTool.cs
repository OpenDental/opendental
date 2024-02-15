using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormPatientStatusTool:FormODBase {

		///<summary>Returns a list of all the patient statuses this form can handle.</summary>
		private List<PatientStatus> _listPatientStatuses=new List<PatientStatus>() { PatientStatus.Patient,PatientStatus.Inactive,PatientStatus.Archived, PatientStatus.Deleted};
		///<summary>Returns the patient status from the Criteria filters.</summary>
		private PatientStatus _patientStatusFrom => comboPatientStatusCur.GetSelected<PatientStatus>();
		///<summary>Returns the patient status from the Change Patient Status To combobox.</summary>
		private PatientStatus _patientStatusTo => comboChangePatientStatusTo.GetSelected<PatientStatus>();

		public FormPatientStatusTool() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPatientStatusTool_Load(object sender,EventArgs e) {
			//Auto set the date picker to two years in the past.
			odDatePickerSince.SetDateTime(DateTime.Today.AddYears(-2));
			//Fill listbox
			listOptions.Items.Add(Lan.g(this,"Planned Procedures"));
			listOptions.Items.Add(Lan.g(this,"Completed Procedures"));
			listOptions.Items.Add(Lan.g(this,"Appointments"));
			for(int i=0;i<listOptions.Items.Count;i++) {
				//Set all options to true by default
				listOptions.SetSelected(i,true);
			}
			//Fill and enable ComboBoxClinic if clinics are enabled
			if(PrefC.HasClinicsEnabled) {
				comboClinic.IsAllSelected=true;
			}
			comboPatientStatusCur.Items.Clear();
			comboPatientStatusCur.Items.AddList(_listPatientStatuses,x => x.GetDescription());
			comboPatientStatusCur.SetSelected(0);//Default to first item.
			FillComboPatientStatusTo();
		}

		private void FillComboPatientStatusTo() {
			//Exclude the patient status from the criteria filter
			List<PatientStatus> listPatientStatusesTo=_listPatientStatuses.FindAll(x => x!=_patientStatusFrom && x!=PatientStatus.Deleted);
			comboChangePatientStatusTo.Items.Clear();
			comboChangePatientStatusTo.Items.AddList(listPatientStatusesTo,x => x.GetDescription());
			comboChangePatientStatusTo.SetSelected(0);//Default to first item
		}

		private void FillGrid() {
			//Sets bools for which filters to add
			bool includeTPProc=listOptions.SelectedIndices.Contains(0);
			bool includeCompletedProc=listOptions.SelectedIndices.Contains(1);
			bool includeAppointments=listOptions.SelectedIndices.Contains(2);
			//Gets clinic selection
			List<long> listClinicNums=new List<long>();
			if(PrefC.HasClinicsEnabled) {
				if(comboClinic.IsAllSelected) {
					listClinicNums=comboClinic.ListClinicNumsSelected;
				}
				else {
					listClinicNums.Add(comboClinic.ClinicNumSelected);
				}
			}
			List<Patient> listPatients=Patients.GetPatsToChangeStatus(_patientStatusFrom,odDatePickerSince.GetDateTime()
				,includeTPProc,includeCompletedProc,includeAppointments,listClinicNums);
			List<PatientLink> listPatientLinks=PatientLinks.GetLinks(listPatients.Select(x => x.PatNum).ToList(),PatientLinkType.Merge);
			for(int i=listPatients.Count-1;i>=0;i--) {
				if(PatientLinks.WasPatientMerged(listPatients[i].PatNum,listPatientLinks)) {
					listPatients.RemoveAt(i);
				}
			}
			gridMain.BeginUpdate();
			if(gridMain.Columns.Count==0) {
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"PatNum"),75,GridSortingStrategy.AmountParse));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"PatStatusCur"),100,GridSortingStrategy.StringCompare));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Last Name"),125,GridSortingStrategy.StringCompare));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"First Name"),125,GridSortingStrategy.StringCompare));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Middle Name"),125,GridSortingStrategy.StringCompare));
				gridMain.Columns.Add(new GridColumn(Lan.g(this,"Birthdate"),75,GridSortingStrategy.DateParse));
				if(PrefC.HasClinicsEnabled) {
					gridMain.Columns.Add(new GridColumn(Lan.g(this,"Clinic"),75,GridSortingStrategy.StringCompare));
				}
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listPatients.Count;i++) {
				row=new GridRow();
				row.Cells.Add(POut.Long(listPatients[i].PatNum));
				row.Cells.Add(listPatients[i].PatStatus.GetDescription());
				row.Cells.Add(listPatients[i].LName);
				row.Cells.Add(listPatients[i].FName);
				row.Cells.Add(listPatients[i].MiddleI);
				row.Cells.Add(listPatients[i].Birthdate.Year<1880 ?"": listPatients[i].Birthdate.ToShortDateString());
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(listPatients[i].ClinicNum));
				}
				row.Tag=listPatients[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butSelectAll_Click(object sender,EventArgs e) {
			gridMain.SetAll(true);
		}

		private void butDeselectAll_Click(object sender,EventArgs e) {
			gridMain.SetAll(false);
		}

		private void butCreateList_Click(object sender,EventArgs e) {
			if(listOptions.SelectedIndices.Count==0) {
				MsgBox.Show(this,"Please select an option from the list.");
				return;
			}
			if(!odDatePickerSince.IsValid()) {
				MsgBox.Show(this,"Please select a valid from and to date.");
				return;
			}
			FillGrid();
		}

		private void comboPatientStatusCur_SelectionChangeCommitted(object sender,EventArgs e) {
			FillComboPatientStatusTo();
		}

		private void butRun_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please make a selection first");
				return;
			}
			string patientStatusFrom=Lan.g("enumPatientStatus",_patientStatusFrom.GetDescription());
			string patientStatusTo=Lan.g("enumPatientStatus",_patientStatusTo.GetDescription());;
			string msgText=Lan.g(this,"This will change the status for selected patients from")+" "
				+patientStatusFrom+" "
				+Lans.g(this,"to")+" "
				+patientStatusTo+".\r\n"+
				Lan.g(this,"Do you wish to continue?");
			if(MessageBox.Show(msgText,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
				return;//The user chose not to change the statuses.
			}
			StringBuilder stringBuilder=new StringBuilder();
			List<long> listPatNums=new List<long>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				Patient patientOld=(Patient)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag;
				Patient patient=patientOld.Copy();
				listPatNums.Add(patient.PatNum);
				patient.PatStatus=_patientStatusTo;
				Patients.UpdateRecalls(patient,patientOld,"Patient Status Tool");
				Patients.Update(patient,patientOld);
				stringBuilder.AppendLine(
					Lans.g(this,"Patient")+" "+POut.Long(patient.PatNum)+": "+patient.GetNameLF()+" "
					+Lans.g(this,"patient status changed from")+" "+patientStatusFrom+" "
					+Lans.g(this,"to")+" "+patientStatusTo
				);//Like "Patient 123: John Doe patient status changed from X to Y"
			}
			using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(stringBuilder.ToString());
			msgBoxCopyPaste.Text=Lans.g(this,"Done");
			msgBoxCopyPaste.ShowDialog();
			SecurityLogs.MakeLogEntry(EnumPermType.SecurityAdmin,listPatNums,Lans.g(this,"Patient status changed from")+" "
				+patientStatusFrom+" "
				+Lans.g(this,"to")+" "+patientStatusTo
				+Lans.g(this," by the Patient Status Setter tool."));
			FillGrid();
		}

	}
}