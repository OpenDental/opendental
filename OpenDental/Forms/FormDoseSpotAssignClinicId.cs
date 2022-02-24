using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	public partial class FormDoseSpotAssignClinicId:FormODBase {
		private List<Clinic> _listClinicsInComboBox;
		private ClinicErx _clinicErxCur;
    List<ProgramProperty> _listClinicIDs;
    List<ProgramProperty> _listClinicKeys;

    public FormDoseSpotAssignClinicId(long clinicErxNum) {
			InitializeComponent();
			InitializeLayoutManager();
			_clinicErxCur=ClinicErxs.GetFirstOrDefault(x => x.ClinicErxNum==clinicErxNum);
			Lan.F(this);
		}

		private void FormDoseSpotAssignUserId_Load(object sender,EventArgs e) {
      _listClinicsInComboBox=Clinics.GetForUserod(Security.CurUser,true,"Headquarters");
      List<ProgramProperty> listProgramProperties=ProgramProperties.GetForProgram(Programs.GetCur(ProgramName.eRx).ProgramNum);
      _listClinicIDs=listProgramProperties.FindAll(x => x.PropertyDesc==Erx.PropertyDescs.ClinicID);
      _listClinicKeys=listProgramProperties.FindAll(x => x.PropertyDesc==Erx.PropertyDescs.ClinicKey);
      _listClinicsInComboBox.RemoveAll(x =>//Remove all clinics that already have a DoseSpot Clinic ID OR Clinic Key entered
        _listClinicIDs.FindAll(y => !string.IsNullOrWhiteSpace(y.PropertyValue)).Select(y => y.ClinicNum).Contains(x.ClinicNum) 
        || _listClinicKeys.FindAll(y => !string.IsNullOrWhiteSpace(y.PropertyValue)).Select(y => y.ClinicNum).Contains(x.ClinicNum)
      );
      FillComboBox();
			textClinicId.Text=_clinicErxCur.ClinicId;//ClinicID passed from Alert
      textClinicKey.Text=_clinicErxCur.ClinicKey;//ClinicKey passed from Alert
      textClinicDesc.Text=_clinicErxCur.ClinicDesc;//ClinicDesc passed from Alert
    }

		private void FillComboBox(long selectedClinicNum=-1) {
			comboClinics.Items.Clear();//this is not a comboBoxClinicPicker because the list of clinics is filtered. Combo is still full of Clinics.
			foreach(Clinic clinicCur in _listClinicsInComboBox) {
				comboClinics.Items.Add(clinicCur.Description,clinicCur);
				if(clinicCur.ClinicNum==selectedClinicNum) {
					comboClinics.SelectedIndex=comboClinics.Items.Count-1;//Select The item that was just added if it is the selected num.
				}
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(comboClinics.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a clinic.");
				return;
			}
      _clinicErxCur.ClinicNum=comboClinics.GetSelected<Clinic>().ClinicNum;
      Program progErx=Programs.GetCur(ProgramName.eRx);
      ProgramProperty ppClinicID=_listClinicIDs.FirstOrDefault(x => x.ClinicNum==_clinicErxCur.ClinicNum);
      if(ppClinicID==null) {
        ppClinicID=new ProgramProperty();
        ppClinicID.ProgramNum=progErx.ProgramNum;
        ppClinicID.ClinicNum=_clinicErxCur.ClinicNum;
        ppClinicID.PropertyDesc=Erx.PropertyDescs.ClinicID;
        ppClinicID.PropertyValue=_clinicErxCur.ClinicId;
        ProgramProperties.Insert(ppClinicID);
      }
      else {
        ppClinicID.PropertyValue=_clinicErxCur.ClinicId;
        ProgramProperties.Update(ppClinicID);
      }
      ProgramProperty ppClinicKey=_listClinicKeys.FirstOrDefault(x => x.ClinicNum==_clinicErxCur.ClinicNum);
      if(ppClinicKey==null) {
        ppClinicKey=new ProgramProperty();
        ppClinicKey.ProgramNum=progErx.ProgramNum;
        ppClinicKey.ClinicNum=_clinicErxCur.ClinicNum;
        ppClinicKey.PropertyDesc=Erx.PropertyDescs.ClinicKey;
        ppClinicKey.PropertyValue=_clinicErxCur.ClinicKey;
        ProgramProperties.Insert(ppClinicKey);
      }
      else {
        ppClinicKey.PropertyValue=_clinicErxCur.ClinicKey;
        ProgramProperties.Update(ppClinicKey);
      }
      DataValid.SetInvalid(InvalidType.Programs);
      DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butClinicPick_Click(object sender,EventArgs e) {
      using FormClinics FormC=new FormClinics();
      FormC.IsSelectionMode=true;
      FormC.ListClinics=_listClinicsInComboBox;
      FormC.ShowDialog();
      if(FormC.DialogResult!=DialogResult.OK) {
        return;
      }
			FillComboBox(FormC.SelectedClinicNum);
		}
	}
}