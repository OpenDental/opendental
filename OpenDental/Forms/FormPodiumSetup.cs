using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	public partial class FormPodiumSetup:FormODBase {
		private Program _program;
		private ProgramProperty _programPropertyAPIToken;
		private ProgramProperty _programPropertyCompName;
		//list is used to store changes for each clinic to be updated or inserted when saving to DB.
		private List<ProgramProperty> _listProgramPropertiesChanged;
		private ProgramProperty _programPropertyUseService;
		private ProgramProperty _programPropertyDisableAdvertising;
		private ProgramProperty _programPropertyApptSetCompleteMins;
		private ProgramProperty _programPropertyApptTimeArrivedMins;
		private ProgramProperty _programPropertyApptTimeDismissedMins;
		private ProgramProperty _programPropertyNewPatTriggerType;
		private ProgramProperty _programPropertyExistingPatTriggerType;
		private ProgramProperty _programPropertyShowCommlogsInChartAndAccount;
		private ReviewInvitationTrigger _reviewInvitationTriggerExistingPat;
		private ReviewInvitationTrigger _reviewInvitationTriggerNewPat;
		private bool _hasProgramPropertyChanged;
		private List<ProgramProperty> _listProgramProperties;
		///<summary>Can be 0 for "Headquarters" or non clinic users.</summary>
		private long _clinicNum;

		public FormPodiumSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPodiumSetup_Load(object sender,EventArgs e) {
			if(PrefC.HasClinicsEnabled) {//Using clinics
				if(Security.CurUser.ClinicIsRestricted) {
					if(checkEnabled.Checked) {
						checkEnabled.Enabled=false;
					}
				}
				comboClinic.ClinicNumSelected=Clinics.ClinicNum;
				_clinicNum=comboClinic.ClinicNumSelected;
			}
			else {//clinics are not enabled, use ClinicNum 0 to indicate 'Headquarters' or practice level program properties
				_clinicNum=0;
			}
			_program=Programs.GetCur(ProgramName.Podium);
			if(_program==null) {
				MsgBox.Show(this,"The Podium bridge is missing from the database.");//should never happen
				DialogResult=DialogResult.Cancel;
				return;
			}
			List<long> listUserClinicNums;
			long clinicNum=0;
			if(Security.CurUser.ClinicIsRestricted) {//user is clinic restricted and shouldn't have access to HQ
					listUserClinicNums=Clinics.GetForUserod(Security.CurUser).Select(x => x.ClinicNum).ToList();
			}
			else {
					listUserClinicNums=Clinics.GetForUserod(Security.CurUser,true).Select(x => x.ClinicNum).ToList();
			}
			if(!comboClinic.IsUnassignedSelected) {
					clinicNum=comboClinic.ClinicNumSelected;
			}
			_listProgramProperties=ProgramProperties.GetListForProgramAndClinicWithDefault(_program.ProgramNum,clinicNum);
			_programPropertyUseService=_listProgramProperties.Find(x => x.PropertyDesc==Podium.PropertyDescs.UseService);
			_programPropertyDisableAdvertising=_listProgramProperties.Find(x => x.PropertyDesc==Podium.PropertyDescs.DisableAdvertising);
			_programPropertyApptSetCompleteMins=_listProgramProperties.Find(x => x.PropertyDesc==Podium.PropertyDescs.ApptSetCompletedMinutes);
			_programPropertyApptTimeArrivedMins=_listProgramProperties.Find(x => x.PropertyDesc==Podium.PropertyDescs.ApptTimeArrivedMinutes);
			_programPropertyApptTimeDismissedMins=_listProgramProperties.Find(x => x.PropertyDesc==Podium.PropertyDescs.ApptTimeDismissedMinutes);
			_programPropertyCompName=_listProgramProperties.Find(x => x.PropertyDesc==Podium.PropertyDescs.ComputerNameOrIP);
			_programPropertyAPIToken=_listProgramProperties.Find(x => x.PropertyDesc==Podium.PropertyDescs.APIToken);
			List<ProgramProperty> listProgramPropertiesLocationID=ProgramProperties.GetForProgram(_program.ProgramNum).FindAll(x => x.PropertyDesc==Podium.PropertyDescs.LocationID);
			_listProgramPropertiesChanged=new List<ProgramProperty>();
			for(int i=0;i<listProgramPropertiesLocationID.Count;i++) {//If clinics is off, this will only grab the program property with a 0 clinic num (_listUserClinicNums will only have 0).
				if(_listProgramPropertiesChanged.Exists(x => x.ClinicNum==listProgramPropertiesLocationID[i].ClinicNum)
					|| !listUserClinicNums.Contains(listProgramPropertiesLocationID[i].ClinicNum)) {
					continue;
				}
				_listProgramPropertiesChanged.Add(listProgramPropertiesLocationID[i]);
			}
			_programPropertyNewPatTriggerType=_listProgramProperties.Find(x => x.PropertyDesc==Podium.PropertyDescs.NewPatientTriggerType);
			_programPropertyExistingPatTriggerType=_listProgramProperties.Find(x => x.PropertyDesc==Podium.PropertyDescs.ExistingPatientTriggerType);
			_programPropertyShowCommlogsInChartAndAccount=_listProgramProperties.Find(x => x.PropertyDesc==Podium.PropertyDescs.ShowCommlogsInChartAndAccount);
			if(_programPropertyUseService==null
				|| _programPropertyDisableAdvertising==null
				|| _programPropertyApptSetCompleteMins==null
				|| _programPropertyApptTimeArrivedMins==null
				|| _programPropertyApptTimeDismissedMins==null
				|| _programPropertyCompName==null
				|| _programPropertyAPIToken==null
				|| _programPropertyNewPatTriggerType==null
				|| _programPropertyExistingPatTriggerType==null
				|| _programPropertyShowCommlogsInChartAndAccount==null) 
			{
				MsgBox.Show(this,"You are missing a program property for Podium.  Please contact support to resolve this issue.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			FillForm();
			SetAdvertising();
		}

		///<summary>Handles both visibility and checking of checkHideButtons.</summary>
		private void SetAdvertising() {
			checkHideButtons.Visible=true;
			ProgramProperty programProperty=ProgramProperties.GetForProgram(_program.ProgramNum).FirstOrDefault(x => x.PropertyDesc=="Disable Advertising");
			if(checkEnabled.Checked || programProperty==null) {
				checkHideButtons.Visible=false;
			}
			if(programProperty!=null) {
				checkHideButtons.Checked=(programProperty.PropertyValue=="1");
			}
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			SaveClinicCurProgramPropertiesToList();
			_clinicNum=comboClinic.ClinicNumSelected;
			//This will either display the HQ value, or the clinic specific value.
			ProgramProperty programProperty=_listProgramPropertiesChanged.Find(x => x.ClinicNum==_clinicNum);
			if(programProperty==null){
				textLocationID.Text=_listProgramPropertiesChanged.Find(x => x.ClinicNum==0).PropertyValue;//Default to showing the HQ value when filling info for a clinic with no program property.
				return;
			}
			textLocationID.Text=programProperty.PropertyValue;
		}
		
		///<summary>Updates the in memory _listProgramPropertiesChanged with any changes made to the current locationID for each clinic before showing the next one.</summary>
		private void SaveClinicCurProgramPropertiesToList() {
			ProgramProperty programProperty;//Reused throughout method for different purposes
			//First check if Headquarters (default) is selected.
			if(_clinicNum==0) {
				//Headquarters is selected so only update the location ID (might have changed) on all other location ID properties that match the "old" location ID of HQ.
				programProperty=_listProgramPropertiesChanged.Find(x => x.ClinicNum==_clinicNum);
				if(programProperty==null) {//should never happen, just in case
					return;
				}
				//Get the location ID so that we correctly update all program properties with a matching location ID.
				string locationIdOld=programProperty.PropertyValue;
				for(int i=0;i<_listProgramPropertiesChanged.Count;i++) {
					if(_listProgramPropertiesChanged[i].PropertyValue==locationIdOld) {
						_listProgramPropertiesChanged[i].PropertyValue=textLocationID.Text;
					}
				}
				return;//No other clinic specific changes could have been made, we need to return.
			}
			//Update or Insert clinic specific properties into memory
			ProgramProperty programPropertyLocationID=new ProgramProperty();
			programProperty=_listProgramPropertiesChanged.Find(x => x.ClinicNum==_clinicNum);
			if(programProperty==null) {//Get default programproperty from db.
				programPropertyLocationID=ProgramProperties.GetListForProgramAndClinicWithDefault(_program.ProgramNum,_clinicNum)
					.FirstOrDefault(x => x.PropertyDesc==Podium.PropertyDescs.LocationID);
			}
			else {
				programPropertyLocationID=programProperty;//Override the database's property with what is in memory.
			}
			if(programPropertyLocationID.ClinicNum==0) {//No program property for current clinic, since _clinicNumCur!=0
				ProgramProperty programPropertyLocationIDNew=programPropertyLocationID.Copy();
				programPropertyLocationIDNew.ProgramPropertyNum=0;
				programPropertyLocationIDNew.ClinicNum=_clinicNum;
				programPropertyLocationIDNew.PropertyValue=textLocationID.Text;
				if(!_listProgramPropertiesChanged.Exists(x => x.ClinicNum==_clinicNum)) {//Should always happen
					_listProgramPropertiesChanged.Add(programPropertyLocationIDNew);
				}
				return;
			}
			int index;
			//At this point we know that the clinicnum isn't 0 and the database has a property for that clinicnum.
			if(_listProgramPropertiesChanged.Exists(x => x.ClinicNum==_clinicNum)) {//Should always happen
				programPropertyLocationID.PropertyValue=textLocationID.Text;
				index=_listProgramPropertiesChanged.FindIndex(x => x.ClinicNum==_clinicNum);
				_listProgramPropertiesChanged[index]=programPropertyLocationID;
			}
			else {
				_listProgramPropertiesChanged.Add(programPropertyLocationID);
			}
		}

		private void FillForm() {
			if(_listProgramPropertiesChanged.Count==0) {
				MsgBox.Show(this,"User can't be clinic restricted.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			checkUseService.Checked=PIn.Bool(_programPropertyUseService.PropertyValue);
			checkShowCommlogsInChart.Checked=PIn.Bool(_programPropertyShowCommlogsInChartAndAccount.PropertyValue);
			checkEnabled.Checked=_program.Enabled;
			checkHideButtons.Checked=PIn.Bool(_programPropertyDisableAdvertising.PropertyValue);
			textApptSetComplete.Text=_programPropertyApptSetCompleteMins.PropertyValue;
			textApptTimeArrived.Text=_programPropertyApptTimeArrivedMins.PropertyValue;
			textApptTimeDismissed.Text=_programPropertyApptTimeDismissedMins.PropertyValue;
			textCompNameOrIP.Text=_programPropertyCompName.PropertyValue;
			textAPIToken.Text=_programPropertyAPIToken.PropertyValue;
			ProgramProperty programProperty=_listProgramPropertiesChanged.Find(x => x.ClinicNum==_clinicNum);
			if(programProperty==null) {
				textLocationID.Text=_listProgramPropertiesChanged.Find(x => x.ClinicNum==0).PropertyValue;//Default to showing the HQ value when filling info for a clinic with no program property.
			}
			else {
				textLocationID.Text=programProperty.PropertyValue;
			}
			_reviewInvitationTriggerExistingPat=PIn.Enum<ReviewInvitationTrigger>(_programPropertyExistingPatTriggerType.PropertyValue);
			_reviewInvitationTriggerNewPat=PIn.Enum<ReviewInvitationTrigger>(_programPropertyNewPatTriggerType.PropertyValue);
			switch(_reviewInvitationTriggerExistingPat) {
				case ReviewInvitationTrigger.AppointmentCompleted:
					radioSetCompleteExistingPat.Checked=true;
					break;
				case ReviewInvitationTrigger.AppointmentTimeArrived:
					radioTimeArrivedExistingPat.Checked=true;
					break;
				case ReviewInvitationTrigger.AppointmentTimeDismissed:
					radioTimeDismissedExistingPat.Checked=true;
					break;
			}
			switch(_reviewInvitationTriggerNewPat) {
				case ReviewInvitationTrigger.AppointmentCompleted:
					radioSetCompleteNewPat.Checked=true;
					break;
				case ReviewInvitationTrigger.AppointmentTimeArrived:
					radioTimeArrivedNewPat.Checked=true;
					break;
				case ReviewInvitationTrigger.AppointmentTimeDismissed:
					radioTimeDismissedNewPat.Checked=true;
					break;
			}
			if(_programPropertyUseService==null
				|| _programPropertyDisableAdvertising==null
				|| _programPropertyApptSetCompleteMins==null
				|| _programPropertyApptTimeArrivedMins==null
				|| _programPropertyApptTimeDismissedMins==null
				|| _programPropertyCompName==null
				|| _programPropertyAPIToken==null
				|| _programPropertyNewPatTriggerType==null
				|| _programPropertyExistingPatTriggerType==null
				|| _programPropertyShowCommlogsInChartAndAccount==null) 
			{
				MsgBox.Show(this,"You are missing a program property from the database.  Please call support to resolve this issue.");
				DialogResult=DialogResult.Cancel;
				return;
			}
		}

		private void RadioButton_CheckChanged(object sender,EventArgs e) {
			if(sender.GetType()!=typeof(RadioButton)) {
				return;
			}
			RadioButton radioButton=(RadioButton)sender;
			if(!radioButton.Checked) {
				return;
			}
			switch(radioButton.Name) {
				case "radioSetCompleteExistingPat":
					_reviewInvitationTriggerExistingPat=ReviewInvitationTrigger.AppointmentCompleted;
					break;
				case "radioTimeArrivedExistingPat":
					_reviewInvitationTriggerExistingPat=ReviewInvitationTrigger.AppointmentTimeArrived;
					break;
				case "radioTimeDismissedExistingPat":
					_reviewInvitationTriggerExistingPat=ReviewInvitationTrigger.AppointmentTimeDismissed;
					break;
				case "radioSetCompleteNewPat":
					_reviewInvitationTriggerNewPat=ReviewInvitationTrigger.AppointmentCompleted;
					break;
				case "radioTimeArrivedNewPat":
					_reviewInvitationTriggerNewPat=ReviewInvitationTrigger.AppointmentTimeArrived;
					break;
				case "radioTimeDismissedNewPat":
					_reviewInvitationTriggerNewPat=ReviewInvitationTrigger.AppointmentTimeDismissed;
					break;
				default:
					throw new Exception("Unknown Radio Button Name");
			}
		}

		private void SaveProgram() {
			SaveClinicCurProgramPropertiesToList();
			_program.Enabled=checkEnabled.Checked;
			UpdateProgramProperty(_programPropertyUseService,POut.Bool(checkUseService.Checked));
			UpdateProgramProperty(_programPropertyShowCommlogsInChartAndAccount,POut.Bool(checkShowCommlogsInChart.Checked));
			UpdateProgramProperty(_programPropertyDisableAdvertising,POut.Bool(checkHideButtons.Checked));
			UpdateProgramProperty(_programPropertyApptSetCompleteMins,textApptSetComplete.Text);
			UpdateProgramProperty(_programPropertyApptTimeArrivedMins,textApptTimeArrived.Text);
			UpdateProgramProperty(_programPropertyApptTimeDismissedMins,textApptTimeDismissed.Text);
			UpdateProgramProperty(_programPropertyCompName,textCompNameOrIP.Text);
			UpdateProgramProperty(_programPropertyAPIToken,textAPIToken.Text);
			UpdateProgramProperty(_programPropertyNewPatTriggerType,POut.Int((int)_reviewInvitationTriggerNewPat));
			UpdateProgramProperty(_programPropertyExistingPatTriggerType,POut.Int((int)_reviewInvitationTriggerExistingPat));
			UpsertProgramPropertiesForClinics();
			Programs.Update(_program);
		}

		private void UpdateProgramProperty(ProgramProperty programPropertyFromDb,string newpropertyValue) {
			if(programPropertyFromDb.PropertyValue==newpropertyValue) {
				return;
			}
			programPropertyFromDb.PropertyValue=newpropertyValue;
			ProgramProperties.Update(programPropertyFromDb);
			_hasProgramPropertyChanged=true;
		}

		private void UpsertProgramPropertiesForClinics() {
			List<ProgramProperty> listProgramPropertyLocationIDsFromDb=ProgramProperties.GetForProgram(_program.ProgramNum).FindAll(x => x.PropertyDesc==Podium.PropertyDescs.LocationID);
			for(int i=0;i<_listProgramPropertiesChanged.Count;i++) {
				if(listProgramPropertyLocationIDsFromDb.Exists(x => x.ProgramPropertyNum == _listProgramPropertiesChanged[i].ProgramPropertyNum)) {
					UpdateProgramProperty(listProgramPropertyLocationIDsFromDb[listProgramPropertyLocationIDsFromDb.FindIndex(x => x.ProgramPropertyNum == _listProgramPropertiesChanged[i].ProgramPropertyNum)],_listProgramPropertiesChanged[i].PropertyValue);//ppCur.PropertyValue will match textLocationID.Text
					continue;
				}
				ProgramProperties.Insert(_listProgramPropertiesChanged[i]);//Program property for that clinicnum didn't exist, so insert it into the db.
				_hasProgramPropertyChanged=true;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textApptSetComplete.IsValid()
				|| !textApptTimeArrived.IsValid()
				|| !textApptTimeDismissed.IsValid()) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			SaveProgram();
			if(_hasProgramPropertyChanged) {
				DataValid.SetInvalid(InvalidType.Programs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}