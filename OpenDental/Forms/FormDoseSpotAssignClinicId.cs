using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	public partial class FormDoseSpotAssignClinicId:FormODBase {
		private List<Clinic> _listClinics;
		private ClinicErx _clinicErx;
		List<ProgramProperty> _listProgramPropertiesClinicIDs;
		List<ProgramProperty> _listProgramPropertiesClinicKeys;

		public FormDoseSpotAssignClinicId(long clinicErxNum) {
			InitializeComponent();
			InitializeLayoutManager();
			_clinicErx=ClinicErxs.GetFirstOrDefault(x => x.ClinicErxNum==clinicErxNum);
			Lan.F(this);
		}

		private void FormDoseSpotAssignUserId_Load(object sender,EventArgs e) {
			_listClinics=Clinics.GetForUserod(Security.CurUser,true,"Headquarters");
			List<ProgramProperty> listProgramProperties=ProgramProperties.GetForProgram(Programs.GetCur(ProgramName.eRx).ProgramNum);
			_listProgramPropertiesClinicIDs=listProgramProperties.FindAll(x => x.PropertyDesc==Erx.PropertyDescs.ClinicID);
			_listProgramPropertiesClinicKeys=listProgramProperties.FindAll(x => x.PropertyDesc==Erx.PropertyDescs.ClinicKey);
			_listClinics.RemoveAll(x =>//Remove all clinics that already have a DoseSpot Clinic ID OR Clinic Key entered
				_listProgramPropertiesClinicIDs.FindAll(y => !string.IsNullOrWhiteSpace(y.PropertyValue)).Select(y => y.ClinicNum).Contains(x.ClinicNum)
				|| _listProgramPropertiesClinicKeys.FindAll(y => !string.IsNullOrWhiteSpace(y.PropertyValue)).Select(y => y.ClinicNum).Contains(x.ClinicNum)
			);
			FillComboBox();
			textClinicId.Text=_clinicErx.ClinicId;//ClinicID passed from Alert
			textClinicKey.Text=_clinicErx.ClinicKey;//ClinicKey passed from Alert
			textClinicDesc.Text=_clinicErx.ClinicDesc;//ClinicDesc passed from Alert
		}

		private void FillComboBox(long selectedClinicNum=-1) {
			comboClinics.Items.Clear();//this is not a comboBoxClinicPicker because the list of clinics is filtered. Combo is still full of Clinics.
			for(int i=0;i<_listClinics.Count;i++){
				comboClinics.Items.Add(_listClinics[i].Description,_listClinics[i]);
				if(_listClinics[i].ClinicNum==selectedClinicNum) {
					comboClinics.SelectedIndex=comboClinics.Items.Count-1;//Select The item that was just added if it is the selected num.
				}
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(comboClinics.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a clinic.");
				return;
			}
			_clinicErx.ClinicNum=comboClinics.GetSelected<Clinic>().ClinicNum;
			Program program=Programs.GetCur(ProgramName.eRx);
			ProgramProperty programPropertyClinicID=_listProgramPropertiesClinicIDs.FirstOrDefault(x => x.ClinicNum==_clinicErx.ClinicNum);
			if(programPropertyClinicID==null) {
				programPropertyClinicID=new ProgramProperty();
				programPropertyClinicID.ProgramNum=program.ProgramNum;
				programPropertyClinicID.ClinicNum=_clinicErx.ClinicNum;
				programPropertyClinicID.PropertyDesc=Erx.PropertyDescs.ClinicID;
				programPropertyClinicID.PropertyValue=_clinicErx.ClinicId;
				ProgramProperties.Insert(programPropertyClinicID);
			}
			else {
				programPropertyClinicID.PropertyValue=_clinicErx.ClinicId;
				ProgramProperties.Update(programPropertyClinicID);
			}
			ProgramProperty programPropertyClinicKey=_listProgramPropertiesClinicKeys.FirstOrDefault(x => x.ClinicNum==_clinicErx.ClinicNum);
			if(programPropertyClinicKey==null) {
				programPropertyClinicKey=new ProgramProperty();
				programPropertyClinicKey.ProgramNum=program.ProgramNum;
				programPropertyClinicKey.ClinicNum=_clinicErx.ClinicNum;
				programPropertyClinicKey.PropertyDesc=Erx.PropertyDescs.ClinicKey;
				programPropertyClinicKey.PropertyValue=_clinicErx.ClinicKey;
				ProgramProperties.Insert(programPropertyClinicKey);
			}
			else {
				programPropertyClinicKey.PropertyValue=_clinicErx.ClinicKey;
				ProgramProperties.Update(programPropertyClinicKey);
			}
			DataValid.SetInvalid(InvalidType.Programs);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butClinicPick_Click(object sender,EventArgs e) {
			using FormClinics formClinics=new FormClinics();
			formClinics.IsSelectionMode=true;
			formClinics.ListClinics=_listClinics;
			formClinics.ShowDialog();
			if(formClinics.DialogResult!=DialogResult.OK) {
				return;
			}
			FillComboBox(formClinics.ClinicNumSelected);
		}
	}
}