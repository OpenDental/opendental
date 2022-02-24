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
		public List<ProgramProperty> ListProperties;
		private Clinic _clinicCur;

		public FormDoseSpotPropertyEdit(Clinic clinicCur,string ppClinicIdVal,string ppClinicKeyVal,List<ProgramProperty> listProperties) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_clinicCur=clinicCur;
			ClinicIdVal=ppClinicIdVal;
			ClinicKeyVal=ppClinicKeyVal;
			ListProperties=listProperties;
		}

		private void FormDoseSpotPropertyEdit_Load(object sender,EventArgs e) {
			LayoutMenu();
			textClinicAbbr.Text=_clinicCur.Abbr;
			textClinicID.Text=ClinicIdVal;
			textClinicKey.Text=ClinicKeyVal;
			if(ClinicIdVal.Trim()!="" && ClinicKeyVal.Trim()!="") {//The clinic has values for the clinicId/clinicKey, so they are effectively registered.
				butRegisterClinic.Enabled=false;
			}
			if(_clinicCur.ClinicNum==0) {//Clinics disabled or is HQ.
				menuMain.Enabled=false;//There is no clinic record to edit.
			}
			Program programErx=Programs.GetCur(ProgramName.eRx);
			ProgramProperty ppClinicID=ListProperties
					.FirstOrDefault(x => x.ClinicNum!=_clinicCur.ClinicNum && x.PropertyDesc==Erx.PropertyDescs.ClinicID && x.PropertyValue!="");
			ProgramProperty ppClinicKey=null;
			if(ppClinicID!=null) {
				ppClinicKey=ListProperties
					.FirstOrDefault(x => x.ClinicNum==ppClinicID.ClinicNum && x.PropertyDesc==Erx.PropertyDescs.ClinicKey && x.PropertyValue!="");
			}
			if(ppClinicID==null || string.IsNullOrWhiteSpace(ppClinicID.PropertyValue)
				|| ppClinicKey==null || string.IsNullOrWhiteSpace(ppClinicKey.PropertyValue))
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
			using FormClinicEdit form=new FormClinicEdit(_clinicCur.Copy());
			form.ShowDialog();
			if(form.DialogResult==DialogResult.OK) {
				Clinics.Update(form.ClinicCur,_clinicCur);
				DataValid.SetInvalid(InvalidType.Providers);
				_clinicCur=form.ClinicCur.Copy();
			}
		}

		private void butClear_Click(object sender,EventArgs e) {
			textClinicID.Text="";
			textClinicKey.Text="";
		}

		private void butRegisterClinic_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			try {
				Program programErx=Programs.GetCur(ProgramName.eRx);
				ProgramProperty ppClinicID=ListProperties
					.FirstOrDefault(x => x.ClinicNum!=_clinicCur.ClinicNum && x.PropertyDesc==Erx.PropertyDescs.ClinicID && x.PropertyValue!="");
				ProgramProperty ppClinicKey=null;
				if(ppClinicID!=null) {
					ppClinicKey=ListProperties
						.FirstOrDefault(x => x.ClinicNum==ppClinicID.ClinicNum && x.PropertyDesc==Erx.PropertyDescs.ClinicKey && x.PropertyValue!="");
				}
				if(ppClinicID==null || string.IsNullOrWhiteSpace(ppClinicID.PropertyValue)
					|| ppClinicKey==null || string.IsNullOrWhiteSpace(ppClinicKey.PropertyValue))
				{
					//Should never happen since we disable this button if we can't find a valid clinicID/clinicKey combo ahead of time
					throw new ODException("No registered clinics found.  "
						+"There must be at least one registered clinic before adding additional clinics.");
				}
				string clinicID="";
				string clinicKey="";
				DoseSpot.RegisterClinic(_clinicCur.ClinicNum,ppClinicID.PropertyValue,ppClinicKey.PropertyValue
					,DoseSpot.GetUserID(Security.CurUser,_clinicCur.ClinicNum),out clinicID,out clinicKey);
				textClinicID.Text=clinicID;
				textClinicKey.Text=clinicKey;
			}
			catch(ODException ex) {
				MsgBox.Show(this,ex.Message);
				return;
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Error: ")+ex.Message);
				return;
			}
			finally {
				Cursor=Cursors.Default;
			}
			MsgBox.Show(this,"This clinic has successfully been registered with DoseSpot.\r\n"
				+"If patients in this clinic can be shared with other clinics, contact DoseSpot to link this clinic before using.");
		}

		private void butOK_Click(object sender,EventArgs e) {
			ClinicIdVal=textClinicID.Text.Trim();
			ClinicKeyVal=textClinicKey.Text.Trim();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}