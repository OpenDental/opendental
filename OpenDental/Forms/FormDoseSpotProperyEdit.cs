using System;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;
using System.Linq;
using System.Collections.Generic;

namespace OpenDental {
	public partial class FormDoseSpotPropertyEdit:FormODBase {
		
		public string ClinicIdVal;
		public string ClinicKeyVal;
		public List<ProgramProperty> ListProgramProperties;
		private Clinic _clinic;

		public FormDoseSpotPropertyEdit(Clinic clinic,string strClinicIdPropertyVal,string strClinicKeyVal,List<ProgramProperty> listProgramProperties) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_clinic=clinic;
			ClinicIdVal=strClinicIdPropertyVal;
			ClinicKeyVal=strClinicKeyVal;
			ListProgramProperties=listProgramProperties;
		}

		private void FormDoseSpotPropertyEdit_Load(object sender,EventArgs e) {
			LayoutMenu();
			textClinicAbbr.Text=_clinic.Abbr;
			textClinicID.Text=ClinicIdVal;
			textClinicKey.Text=ClinicKeyVal;
			if(ClinicIdVal.Trim()!="" && ClinicKeyVal.Trim()!="") {//The clinic has values for the clinicID/clinicKey, so they are effectively registered.
				butRegisterClinic.Enabled=false;
			}
			if(_clinic.ClinicNum==0) {//Clinics disabled or is HQ.
				menuMain.Enabled=false;//There is no clinic record to edit.
			}
			Program program=Programs.GetCur(ProgramName.eRx);
			ProgramProperty programPropertyClinicID=ListProgramProperties
					.FirstOrDefault(x => x.ClinicNum!=_clinic.ClinicNum && x.PropertyDesc==Erx.PropertyDescs.ClinicID && x.PropertyValue!="");
			ProgramProperty programPropertyClinicKey=null;
			if(programPropertyClinicID!=null) {
				programPropertyClinicKey=ListProgramProperties
					.FirstOrDefault(x => x.ClinicNum==programPropertyClinicID.ClinicNum && x.PropertyDesc==Erx.PropertyDescs.ClinicKey && x.PropertyValue!="");
			}
			if(programPropertyClinicID==null 
				|| string.IsNullOrWhiteSpace(programPropertyClinicID.PropertyValue)
				|| programPropertyClinicKey==null 
				|| string.IsNullOrWhiteSpace(programPropertyClinicKey.PropertyValue))
			{
				//No clinicID/clinicKey found.  This would be the first clinic to register
				butRegisterClinic.Enabled=false;
				butClear.Enabled=false;
			}
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.EndUpdate();
		}

		private void menuItemSetup_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.ClinicEdit)) {
				return;
			}
			using FormClinicEdit formClinicEdit=new FormClinicEdit(_clinic.Copy());
			formClinicEdit.ShowDialog();
			if(formClinicEdit.DialogResult==DialogResult.OK) {
				Clinics.Update(formClinicEdit.ClinicCur,_clinic);
				DataValid.SetInvalid(InvalidType.Providers);
				_clinic=formClinicEdit.ClinicCur.Copy();
			}
		}

		private void butClear_Click(object sender,EventArgs e) {
			textClinicID.Text="";
			textClinicKey.Text="";
		}

		private void butRegisterClinic_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			Program program=Programs.GetCur(ProgramName.eRx);
			ProgramProperty programPropertyClinicID=ListProgramProperties
					.FirstOrDefault(x => x.ClinicNum!=_clinic.ClinicNum && x.PropertyDesc==Erx.PropertyDescs.ClinicID && x.PropertyValue!="");
			ProgramProperty programPropertyClinicKey=null;
			if(programPropertyClinicID!=null) {
				programPropertyClinicKey=ListProgramProperties
					.FirstOrDefault(x => x.ClinicNum==programPropertyClinicID.ClinicNum && x.PropertyDesc==Erx.PropertyDescs.ClinicKey && x.PropertyValue!="");
			}
			if(programPropertyClinicID==null 
				|| string.IsNullOrWhiteSpace(programPropertyClinicID.PropertyValue)
				|| programPropertyClinicKey==null 
				|| string.IsNullOrWhiteSpace(programPropertyClinicKey.PropertyValue)) 
			{
				//Should never happen since we disable this button if we can't find a valid clinicID/clinicKey combo ahead of time
				MsgBox.Show(this,"No registered clinics found.  There must be at least one registered clinic before adding additional clinics.");
				Cursor=Cursors.Default;
				return;
			}
			string clinicID="";
			string clinicKey="";
			string userID="";
			try {
				userID=DoseSpot.GetUserID(Security.CurUser,_clinic.ClinicNum);
				DoseSpot.RegisterClinic(_clinic.ClinicNum,programPropertyClinicID.PropertyValue,programPropertyClinicKey.PropertyValue,userID,out clinicID,out clinicKey);
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error: ")+ex.Message);
				return;
			}
			finally {
				Cursor=Cursors.Default;
			}
			textClinicID.Text=clinicID;
			textClinicKey.Text=clinicKey;
			MsgBox.Show(this,"This clinic has successfully been registered with DoseSpot.\r\n"
				+"If patients in this clinic can be shared with other clinics, contact DoseSpot to link this clinic before using.");
		}

		private void butSave_Click(object sender,EventArgs e) {
			ClinicIdVal=textClinicID.Text.Trim();
			ClinicKeyVal=textClinicKey.Text.Trim();
			DialogResult=DialogResult.OK;
		}

	}
}