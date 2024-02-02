using OpenDental;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Task = OpenDentBusiness.Task;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmTaskFilter:FrmODBase {
		private static bool _wereDefaultsSet;
		///<summary>Passed in so that we know the patient currently selected in the main window.</summary>
		public Patient PatientMain;
		public Patient PatientSelected;
		///<summary>Passed in so that we know the patient for the current task.</summary>
		public Patient PatientTask;
		public EnumTaskPatientFilterType EnumTaskPatientFilterType_;
		public DateTime DateStart;
		public DateTime DateEnd;
		///<summary>Empty list means no clinics selected, so show all clinics</summary>
		public List<long> ListClinicNumsSelected=new List<long>();
		///<summary>Empty list means no regions selected, so show all region</summary>
		public List<long> ListDefNumsRegionsSelected=new List<long>();
		public bool ClearAllClicked=false;

		public FrmTaskFilter() {
			InitializeComponent();
			Load+=FrmTaskFilter_Load;
			comboClinic.SelectionChangeCommitted+=ComboClinic_SelectionChangeCommitted;
			comboRegion.SelectionChangeCommitted+=ComboRegion_SelectionChangeCommitted;
			radioAllPatients.Click+=RadioAllPatients_Click;
			radioPatientMain.Click+=RadioPatientSelected_Click;
			radioPatientSelectedTask.Click+=RadioPatientSelectedTask_Click;
			butSelectPatient.Visible=false;//Hidden until formSelectPatient is built in WPF
			radioSpecificPatient.Visible=false;//Hidden until formSelectPatient is built in WPF
			comboClinic.IncludeAll=true;
			comboRegion.IncludeAll=true;
		}

		private void FrmTaskFilter_Load(object sender,EventArgs e) {
			textVDateStart.Value=DateStart;
			textVDateEnd.Value=DateEnd;
			if(PatientMain==null) {
				radioPatientMain.IsEnabled=false;
			}
			if(PatientTask==null) {
				radioPatientSelectedTask.IsEnabled=false;
			}
			if(EnumTaskPatientFilterType_==EnumTaskPatientFilterType.PatientMain && PatientMain!=null) {
				radioPatientMain.Checked=true;
				PatientSelected=PatientMain;
				textPatientSelected.Text=PatientSelected.GetNameLF()+" - "+PatientSelected.PatNum.ToString();
			}
			else if(EnumTaskPatientFilterType_==EnumTaskPatientFilterType.PatientSelectedTask && PatientTask!=null) {
				radioPatientSelectedTask.Checked=true;
				PatientSelected=PatientTask;
				textPatientSelected.Text=PatientSelected.GetNameLF()+" - "+PatientSelected.PatNum.ToString();
			}
			else if(EnumTaskPatientFilterType_==EnumTaskPatientFilterType.SpecificPatient && PatientSelected!=null) {
				radioSpecificPatient.Checked=true;
				textPatientSelected.Text=PatientSelected.GetNameLF()+" - "+PatientSelected.PatNum.ToString();
			}
			else{
				EnumTaskPatientFilterType_=EnumTaskPatientFilterType.AllPatients;
				radioAllPatients.Checked=true;
				textPatientSelected.Text="";
			}
			FillComboBoxClinics();
			FillComboBoxRegions();
		}

		private void FillComboBoxClinics() {
			List<Clinic> listClinics=Clinics.GetAllForUserod(Security.CurUser);
			comboClinic.Items.AddList(listClinics,x=>x.Abbr);
			for(int i = 0;i<listClinics.Count;i++) {
				if(ListClinicNumsSelected.Contains(listClinics[i].ClinicNum)) {
					comboClinic.SetSelected(i);
				}
			}
			if(comboClinic.SelectedIndices.Count==0) {
				comboClinic.IsAllSelected=true;
			}
		}

		private void FillComboBoxRegions() {
			List<Clinic> listClinics=Clinics.GetAllForUserod(Security.CurUser);
			List<long> listDefNumsRegion=listClinics.Select(x=>x.Region).Distinct().ToList();
			List<Def> listDefsRegions=Defs.GetDefs(DefCat.Regions,listDefNumsRegion);
			comboRegion.Items.AddDefs(listDefsRegions);
			for(int i=0;i<listDefsRegions.Count;i++){
				if(ListDefNumsRegionsSelected.Contains(listDefsRegions[i].DefNum)) {
					comboRegion.SetSelected(i);
				}
			}
			if(comboRegion.SelectedIndices.Count==0) {
				comboRegion.IsAllSelected=true;
			}
		}

		private void RadioAllPatients_Click(object sender, EventArgs e) {
			PatientSelected=null;
			textPatientSelected.Text="";
		}

		private void RadioPatientSelected_Click(object sender, EventArgs e) {
			PatientSelected=PatientMain;
			textPatientSelected.Text=PatientSelected.GetNameLF()+" - "+PatientSelected.PatNum.ToString();
		}

		private void RadioPatientSelectedTask_Click(object sender, EventArgs e) {
			PatientSelected=PatientTask;
			textPatientSelected.Text=PatientTask.GetNameLF()+" - "+PatientTask.PatNum.ToString();
		}

		private void ComboClinic_SelectionChangeCommitted(object sender, EventArgs e) {
			if(comboClinic.SelectedIndices.Count>0) {
				comboRegion.IsAllSelected=true;
			}
		}

		private void ComboRegion_SelectionChangeCommitted(object sender, EventArgs e) {
			if(comboRegion.SelectedIndices.Count>0) {
				comboClinic.IsAllSelected=true;
			}
		}

		private void butAll_Click(object sender, EventArgs e) {
			comboClinic.IsAllSelected=true;
			comboRegion.IsAllSelected=true;
		}

		private void butAllDates_Click(object sender, EventArgs e) {
			textVDateStart.Value=DateTime.MinValue;
			textVDateEnd.Value=DateTime.MinValue;
		}

		private void butToday_Click(object sender, EventArgs e) {
			textVDateStart.Value=DateTime.Today;
			textVDateEnd.Value=DateTime.Today;
		}

		private void butTomorrow_Click(object sender, EventArgs e) {
			textVDateStart.Value=DateTime.Today.AddDays(1);
			textVDateEnd.Value=DateTime.Today.AddDays(1);
		}

		private void butNext7Days_Click(object sender, EventArgs e) {
			textVDateStart.Value=DateTime.Today;
			//Only add 6 days because Today is included 
			textVDateEnd.Value=DateTime.Today.AddDays(6);
		}

		private void butSelectPatient_Click(object sender, EventArgs e) {
			//We do not have access to open up the Select Patient Window yet from WPF will be added when FormPatientSelect is built in WPF
		}

		private void butClearAll_Click(object sender, EventArgs e) {
			ClearAllClicked=true;
			IsDialogOK=true;
		}

		private void butOK_Click(object sender, EventArgs e) {
			if(!textVDateEnd.IsValid()) {
				MsgBox.Show(this,"Please input a valid End Date.");
				return;
			}
			if(!textVDateStart.IsValid()) {
				MsgBox.Show(this,"Please input a valid Start Date.");
				return;
			}
			if(textVDateEnd.Value>DateTime.MinValue && textVDateEnd.Value<textVDateStart.Value) {
				MsgBox.Show(this,"End Date cannot be before Start Date.");
				return;
			}
			DateStart=textVDateStart.Value;
			DateEnd=textVDateEnd.Value;
			if(comboClinic.SelectedIndices.Count>0) {
				ListClinicNumsSelected=comboClinic.GetListSelected<Clinic>().Select(x=>x.ClinicNum).ToList();
				ListDefNumsRegionsSelected.Clear();
			}
			else if(comboRegion.SelectedIndices.Count>0) {
				ListDefNumsRegionsSelected=comboRegion.GetListSelected<Def>().Select(x=>x.DefNum).ToList();
				ListClinicNumsSelected.Clear();
			}
			else {
				ListDefNumsRegionsSelected.Clear();
				ListClinicNumsSelected.Clear();
			}
			if(radioAllPatients.Checked) {
				EnumTaskPatientFilterType_=EnumTaskPatientFilterType.AllPatients;
			}
			else if(radioPatientMain.Checked) {
				EnumTaskPatientFilterType_=EnumTaskPatientFilterType.PatientMain;
			}
			else if(radioPatientSelectedTask.Checked) {
				EnumTaskPatientFilterType_=EnumTaskPatientFilterType.PatientSelectedTask;
			}
			else if(radioSpecificPatient.Checked) {
				EnumTaskPatientFilterType_=EnumTaskPatientFilterType.SpecificPatient;
			}
			IsDialogOK=true;
		}

	}

	///<summary>Used to determine which tasks should be filtered to show in a tasklist based upon patient filter.</summary>
	public enum EnumTaskPatientFilterType {
		///<summary>0 - All Patients</summary>
		AllPatients=0,
		///<summary>1 - Selected patient in main window</summary>
		PatientMain=1,
		///<summary>2 - Patient of selected task</summary>
		PatientSelectedTask=2,
		///<summary>3 - Selected patient in FormSelectPatient from FrmTaskFilter</summary>
		SpecificPatient=3,
	}
}
