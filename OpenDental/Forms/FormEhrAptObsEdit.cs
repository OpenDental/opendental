using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrAptObsEdit:FormODBase {

		private EhrAptObs _ehrAptObsCur=null;
		private Appointment _appt=null;
		private string _strValCodeSystem="";
		private Loinc _loincValue=null;
		private Snomed _snomedValue=null;
		private ICD9 _icd9Value=null;
		private Icd10 _icd10Value=null;

		public FormEhrAptObsEdit(EhrAptObs ehrAptObs) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_ehrAptObsCur=ehrAptObs;
		}

		private void FormEhrAptObsEdit_Load(object sender,EventArgs e) {
			_appt=Appointments.GetOneApt(_ehrAptObsCur.AptNum);
			comboObservationQuestion.Items.Clear();
			string[] arrayQuestionNames=Enum.GetNames(typeof(EhrAptObsIdentifier));
			for(int i=0;i<arrayQuestionNames.Length;i++) {
				comboObservationQuestion.Items.Add(arrayQuestionNames[i]);
				EhrAptObsIdentifier ehrAptObsIdentifier=(EhrAptObsIdentifier)i;
				if(_ehrAptObsCur.IdentifyingCode==ehrAptObsIdentifier) {
					comboObservationQuestion.SelectedIndex=i;
				}
			}
			listValueType.Items.Clear();
			listValueType.Items.AddEnums<EhrAptObsType>();
			for(int i=0;i<listValueType.Items.Count;i++) {
				if(_ehrAptObsCur.ValType==(EhrAptObsType)listValueType.Items.GetObjectAt(i)) {
					listValueType.SelectedIndex=i;
				}
			}
			if(_ehrAptObsCur.ValType==EhrAptObsType.Coded) {
				_strValCodeSystem=_ehrAptObsCur.ValCodeSystem;
				if(_ehrAptObsCur.ValCodeSystem=="LOINC") {
					_loincValue=Loincs.GetByCode(_ehrAptObsCur.ValReported);
					textValue.Text=_loincValue.NameShort;
				}
				else if(_ehrAptObsCur.ValCodeSystem=="SNOMEDCT") {
					_snomedValue=Snomeds.GetByCode(_ehrAptObsCur.ValReported);
					textValue.Text=_snomedValue.Description;
				}
				else if(_ehrAptObsCur.ValCodeSystem=="ICD9") {
					_icd9Value=ICD9s.GetByCode(_ehrAptObsCur.ValReported);
					textValue.Text=_icd9Value.Description;
				}
				else if(_ehrAptObsCur.ValCodeSystem=="ICD10") {
					_icd10Value=Icd10s.GetByCode(_ehrAptObsCur.ValReported);
					textValue.Text=_icd10Value.Description;
				}
			}
			else {
				textValue.Text=_ehrAptObsCur.ValReported;
			}
			comboUnits.Items.Clear();
			comboUnits.Items.Add("none");
			comboUnits.SelectedIndex=0;
			List<string> listUcumCodes=Ucums.GetAllCodes();
			for(int i=0;i<listUcumCodes.Count;i++) {
				string ucumCode=listUcumCodes[i];
				comboUnits.Items.Add(ucumCode);
				if(ucumCode==_ehrAptObsCur.UcumCode) {
					comboUnits.SelectedIndex=i+1;
				}
			}
			SetFlags();
		}

		private void listValueType_SelectedIndexChanged(object sender,EventArgs e) {
			textValue.Text="";
			_loincValue=null;
			_snomedValue=null;
			_icd9Value=null;
			_icd10Value=null;
			SetFlags();
		}

		private void SetFlags() {
			labelValue.Text="Value";
			textValue.ReadOnly=false;
			butPickValueLoinc.Enabled=false;
			butPickValueSnomedct.Enabled=false;
			butPickValueIcd9.Enabled=false;
			butPickValueIcd10.Enabled=false;
			if(listValueType.SelectedIndex==(int)EhrAptObsType.Coded) {
				labelValue.Text=_strValCodeSystem+" Value";
				textValue.ReadOnly=true;
				butPickValueLoinc.Enabled=true;
				butPickValueSnomedct.Enabled=true;
				butPickValueIcd9.Enabled=true;
				butPickValueIcd10.Enabled=true;
			}
			if(listValueType.SelectedIndex==(int)EhrAptObsType.Numeric) {
				comboUnits.Enabled=true;
			}
			else {
				comboUnits.Enabled=false;
			}
			if(listValueType.SelectedIndex==(int)EhrAptObsType.Address) {
				labelValue.Text="Facility Address";
				textValue.ReadOnly=true;
				string sendingFacilityName=PrefC.GetString(PrefName.PracticeTitle);
				string sendingFacilityAddress1=PrefC.GetString(PrefName.PracticeAddress);
				string sendingFacilityAddress2=PrefC.GetString(PrefName.PracticeAddress2);
				string sendingFacilityCity=PrefC.GetString(PrefName.PracticeCity);
				string sendingFacilityState=PrefC.GetString(PrefName.PracticeST);
				string sendingFacilityZip=PrefC.GetString(PrefName.PracticeZip);
				if(PrefC.HasClinicsEnabled && _appt.ClinicNum!=0) {//Using clinics and a clinic is assigned.
					Clinic clinic=Clinics.GetClinic(_appt.ClinicNum);
					sendingFacilityName=clinic.Description;
					sendingFacilityAddress1=clinic.Address;
					sendingFacilityAddress2=clinic.Address2;
					sendingFacilityCity=clinic.City;
					sendingFacilityState=clinic.State;
					sendingFacilityZip=clinic.Zip;
				}
				textValue.Text=sendingFacilityAddress1+" "+sendingFacilityAddress2+" "+sendingFacilityCity+" "+sendingFacilityState+" "+sendingFacilityZip;
			}
		}

		private void butPickValueLoinc_Click(object sender,EventArgs e) {
			using FormLoincs formL=new FormLoincs();
			formL.IsSelectionMode=true;
			if(formL.ShowDialog()==DialogResult.OK) {
				_loincValue=formL.SelectedLoinc;
				textValue.Text=_loincValue.NameShort;
				_strValCodeSystem="LOINC";
				labelValue.Text=_strValCodeSystem+" Value";
			}
		}

		private void butPickValueSnomedct_Click(object sender,EventArgs e) {
			using FormSnomeds formS=new FormSnomeds();
			formS.IsSelectionMode=true;
			if(formS.ShowDialog()==DialogResult.OK) {
				_snomedValue=formS.SelectedSnomed;
				textValue.Text=_snomedValue.Description;
				_strValCodeSystem="SNOMEDCT";
				labelValue.Text=_strValCodeSystem+" Value";
			}
		}

		private void butPickValueIcd9_Click(object sender,EventArgs e) {
			using FormIcd9s formI=new FormIcd9s();
			formI.IsSelectionMode=true;
			if(formI.ShowDialog()==DialogResult.OK) {
				_icd9Value=formI.SelectedIcd9;
				textValue.Text=_icd9Value.Description;
				_strValCodeSystem="ICD9";
				labelValue.Text=_strValCodeSystem+" Value";
			}
		}

		private void butPickValueIcd10_Click(object sender,EventArgs e) {
			using FormIcd10s formI=new FormIcd10s();
			formI.IsSelectionMode=true;
			if(formI.ShowDialog()==DialogResult.OK) {
				_icd10Value=formI.SelectedIcd10;
				textValue.Text=_icd10Value.Description;
				_strValCodeSystem="ICD10";
				labelValue.Text=_strValCodeSystem+" Value";
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_ehrAptObsCur.IsNew) {
				DialogResult=DialogResult.Cancel;
			}
			else {
				EhrAptObses.Delete(_ehrAptObsCur.EhrAptObsNum);
				_ehrAptObsCur.EhrAptObsNum=0;//Signal to the calling code that the object has been deleted.
				DialogResult=DialogResult.OK;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			EhrAptObsIdentifier ehrAptObsId=(EhrAptObsIdentifier)comboObservationQuestion.SelectedIndex;
			if(listValueType.SelectedIndex==(int)EhrAptObsType.Address && ehrAptObsId!=EhrAptObsIdentifier.TreatFacilityLocation ||
				listValueType.SelectedIndex!=(int)EhrAptObsType.Address && ehrAptObsId==EhrAptObsIdentifier.TreatFacilityLocation) {
				MsgBox.Show(this,"Value type Address must be used with question TreatFacilityLocation.");
				return;
			}
			if(listValueType.SelectedIndex==(int)EhrAptObsType.Coded && _loincValue==null && _snomedValue==null && _icd9Value==null && _icd10Value==null) {
				MsgBox.Show(this,"Missing value code.");
				return;
			}
			if(listValueType.SelectedIndex!=(int)EhrAptObsType.Coded && textValue.Text=="") {
				MsgBox.Show(this,"Missing value.");
				return;
			}
			_ehrAptObsCur.IdentifyingCode=ehrAptObsId;
			_ehrAptObsCur.ValType=(EhrAptObsType)listValueType.SelectedIndex;
			if(_ehrAptObsCur.ValType==EhrAptObsType.Coded) {
				_ehrAptObsCur.ValCodeSystem=_strValCodeSystem;
				if(_strValCodeSystem=="LOINC") {
					_ehrAptObsCur.ValReported=_loincValue.LoincCode;
				}
				else if(_strValCodeSystem=="SNOMEDCT") {
					_ehrAptObsCur.ValReported=_snomedValue.SnomedCode;
				}
				else if(_strValCodeSystem=="ICD9") {
					_ehrAptObsCur.ValReported=_icd9Value.ICD9Code;
				}
				else if(_strValCodeSystem=="ICD10") {
					_ehrAptObsCur.ValReported=_icd10Value.Icd10Code;
				}
			}
			else if(_ehrAptObsCur.ValType==EhrAptObsType.Address) {
				_ehrAptObsCur.ValCodeSystem="";
				_ehrAptObsCur.ValReported="";
			}
			else {
				_ehrAptObsCur.ValCodeSystem="";
				_ehrAptObsCur.ValReported=textValue.Text;
			}
			_ehrAptObsCur.UcumCode="";
			if(comboUnits.Enabled) {
				_ehrAptObsCur.UcumCode=comboUnits.Items[comboUnits.SelectedIndex].ToString();
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}