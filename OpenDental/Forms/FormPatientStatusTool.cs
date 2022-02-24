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
		private List<PatientStatus> _listPatStatus=new List<PatientStatus>() { PatientStatus.Patient,PatientStatus.Inactive,PatientStatus.Archived};
		///<summary>Returns the patient status from the Criteria filters.</summary>
		private PatientStatus _patStatusFrom => comboPatientStatusCur.GetSelected<PatientStatus>();
		///<summary>Returns the patient status from the Change Patient Status To combobox.</summary>
		private PatientStatus _patStatusTo => comboChangePatientStatusTo.GetSelected<PatientStatus>();

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
			comboPatientStatusCur.Items.AddList(_listPatStatus,x => x.GetDescription());
			comboPatientStatusCur.SetSelected(0);//Default to first item.
			FillComboPatientStatusTo();
		}

		private void FillComboPatientStatusTo() {
			//Exclude the patient status from the criteria filter
			List<PatientStatus> listPatStatusesTo=_listPatStatus.FindAll(x => x!=_patStatusFrom);
			comboChangePatientStatusTo.Items.Clear();
			comboChangePatientStatusTo.Items.AddList(listPatStatusesTo,x => x.GetDescription());
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
					listClinicNums=comboClinic.ListSelectedClinicNums;
				}
				else {
					listClinicNums.Add(comboClinic.SelectedClinicNum);
				}
			}
			List<Patient> listPatients=Patients.GetPatsToChangeStatus(_patStatusFrom,odDatePickerSince.GetDateTime()
				,includeTPProc,includeCompletedProc,includeAppointments,listClinicNums);
			gridMain.BeginUpdate();
			if(gridMain.ListGridColumns.Count==0) {
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"PatNum"),75,GridSortingStrategy.AmountParse));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"PatStatusCur"),100,GridSortingStrategy.StringCompare));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Last Name"),125,GridSortingStrategy.StringCompare));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"First Name"),125,GridSortingStrategy.StringCompare));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Middle Name"),125,GridSortingStrategy.StringCompare));
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Birthdate"),75,GridSortingStrategy.DateParse));
				if(PrefC.HasClinicsEnabled) {
					gridMain.ListGridColumns.Add(new GridColumn(Lan.g(this,"Clinic"),75,GridSortingStrategy.StringCompare));
				}
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(Patient pat in listPatients) {
				row=new GridRow();
				row.Cells.Add(POut.Long(pat.PatNum));
				row.Cells.Add(pat.PatStatus.GetDescription());
				row.Cells.Add(pat.LName);
				row.Cells.Add(pat.FName);
				row.Cells.Add(pat.MiddleI);
				row.Cells.Add(pat.Birthdate.Year<1880 ?"": pat.Birthdate.ToShortDateString());
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(pat.ClinicNum));
				}
				row.Tag=pat;
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
			string patientStatusFrom=Lan.g("enumPatientStatus",_patStatusFrom.GetDescription());
			string patientStatusTo=Lan.g("enumPatientStatus",_patStatusTo.GetDescription());;
			string msgText=Lan.g(this,"This will change the status for selected patients from")+" "
				+patientStatusFrom+" "
				+Lans.g(this,"to")+" "
				+patientStatusTo+".\r\n"+
				Lan.g(this,"Do you wish to continue?");
			if(MessageBox.Show(msgText,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
				return;//The user chose not to change the statuses.
			}
			StringBuilder builder=new StringBuilder();
			List<long> listPatNums=new List<long>();
			foreach(int index in gridMain.SelectedIndices) {
				Patient patOld=(Patient)gridMain.ListGridRows[index].Tag;
				Patient patCur=patOld.Copy();
				listPatNums.Add(patCur.PatNum);
				patCur.PatStatus=_patStatusTo;
				Patients.UpdateRecalls(patCur,patOld,"Patient Status Tool");
				Patients.Update(patCur,patOld);
				builder.AppendLine(
					Lans.g(this,"Patient")+" "+POut.Long(patCur.PatNum)+": "+patCur.GetNameLF()+" "
					+Lans.g(this,"patient status changed from")+" "+patientStatusFrom+" "
					+Lans.g(this,"to")+" "+patientStatusTo
				);//Like "Patient 123: John Doe patient status changed from X to Y"
			}
			using MsgBoxCopyPaste msg=new MsgBoxCopyPaste(builder.ToString());
			msg.Text=Lans.g(this,"Done");
			msg.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,listPatNums,Lans.g(this,"Patient status changed from")+" "
				+patientStatusFrom+" "
				+Lans.g(this,"to")+" "+patientStatusTo
				+Lans.g(this," by the Patient Status Setter tool."));
			FillGrid();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}