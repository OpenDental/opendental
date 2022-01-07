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
		private Program _progCur;
		private ProgramProperty _apiToken;
		private ProgramProperty _compName;
		///<summary>Dictionary used to store changes for each clinic to be updated or inserted when saving to DB.</summary>
		private Dictionary<long,ProgramProperty> _dictLocationIDs=new Dictionary<long, ProgramProperty>();
		private ProgramProperty _useService;
		private ProgramProperty _disableAdvertising;
		private ProgramProperty _apptSetCompleteMins;
		private ProgramProperty _apptTimeArrivedMins;
		private ProgramProperty _apptTimeDismissedMins;
		private ProgramProperty _newPatTriggerType;
		private ProgramProperty _existingPatTriggerType;
		private ProgramProperty _showCommlogsInChartAndAccount;
		private ReviewInvitationTrigger _existingPatTriggerEnum;
		private ReviewInvitationTrigger _newPatTriggerEnum;
		private bool _hasProgramPropertyChanged;
		private List<ProgramProperty> _listProgramProperties;
		///<summary>Can be 0 for "Headquarters" or non clinic users.</summary>
		private long _clinicNumCur;

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
				comboClinic.SelectedClinicNum=Clinics.ClinicNum;
				_clinicNumCur=comboClinic.SelectedClinicNum;
			}
			else {//clinics are not enabled, use ClinicNum 0 to indicate 'Headquarters' or practice level program properties
				_clinicNumCur=0;
			}
			_progCur=Programs.GetCur(ProgramName.Podium);
			if(_progCur==null) {
				MsgBox.Show(this,"The Podium bridge is missing from the database.");//should never happen
				DialogResult=DialogResult.Cancel;
				return;
			}
			try {
				List<long> listUserClinicNums;
				if(Security.CurUser.ClinicIsRestricted) {//user is clinic restricted and shouldn't have access to HQ
					listUserClinicNums=Clinics.GetForUserod(Security.CurUser).Select(x => x.ClinicNum).ToList();
				}
				else {
					listUserClinicNums=Clinics.GetForUserod(Security.CurUser,true).Select(x => x.ClinicNum).ToList();
				}
				long clinicNum=0;
				if(!comboClinic.IsUnassignedSelected) {
					clinicNum=comboClinic.SelectedClinicNum;
				}
				_listProgramProperties=ProgramProperties.GetListForProgramAndClinicWithDefault(_progCur.ProgramNum,clinicNum);
				_useService=_listProgramProperties.FirstOrDefault(x => x.PropertyDesc==Podium.PropertyDescs.UseService);
				_disableAdvertising=_listProgramProperties.FirstOrDefault(x => x.PropertyDesc==Podium.PropertyDescs.DisableAdvertising);
				_apptSetCompleteMins=_listProgramProperties.FirstOrDefault(x => x.PropertyDesc==Podium.PropertyDescs.ApptSetCompletedMinutes);
				_apptTimeArrivedMins=_listProgramProperties.FirstOrDefault(x => x.PropertyDesc==Podium.PropertyDescs.ApptTimeArrivedMinutes);
				_apptTimeDismissedMins=_listProgramProperties.FirstOrDefault(x => x.PropertyDesc==Podium.PropertyDescs.ApptTimeDismissedMinutes);
				_compName=_listProgramProperties.FirstOrDefault(x => x.PropertyDesc==Podium.PropertyDescs.ComputerNameOrIP);
				_apiToken=_listProgramProperties.FirstOrDefault(x => x.PropertyDesc==Podium.PropertyDescs.APIToken);
				List<ProgramProperty> listLocationIDs=ProgramProperties.GetForProgram(_progCur.ProgramNum).FindAll(x => x.PropertyDesc==Podium.PropertyDescs.LocationID);
				_dictLocationIDs.Clear();
				foreach(ProgramProperty ppCur in listLocationIDs) {//If clinics is off, this will only grab the program property with a 0 clinic num (_listUserClinicNums will only have 0).
					if(_dictLocationIDs.ContainsKey(ppCur.ClinicNum) || !listUserClinicNums.Contains(ppCur.ClinicNum)) {
						continue;
					}
					_dictLocationIDs.Add(ppCur.ClinicNum,ppCur);
				}
				_newPatTriggerType=_listProgramProperties.FirstOrDefault(x => x.PropertyDesc==Podium.PropertyDescs.NewPatientTriggerType);
				_existingPatTriggerType=_listProgramProperties.FirstOrDefault(x => x.PropertyDesc==Podium.PropertyDescs.ExistingPatientTriggerType);
				_showCommlogsInChartAndAccount=_listProgramProperties.FirstOrDefault(x => x.PropertyDesc==Podium.PropertyDescs.ShowCommlogsInChartAndAccount);
			}
			catch(Exception) {
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
			ProgramProperty prop=ProgramProperties.GetForProgram(_progCur.ProgramNum).FirstOrDefault(x => x.PropertyDesc=="Disable Advertising");
			if(checkEnabled.Checked || prop==null) {
				checkHideButtons.Visible=false;
			}
			if(prop!=null) {
				checkHideButtons.Checked=(prop.PropertyValue=="1");
			}
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			SaveClinicCurProgramPropertiesToDict();
			_clinicNumCur=comboClinic.SelectedClinicNum;
			//This will either display the HQ value, or the clinic specific value.
			if(_dictLocationIDs.ContainsKey(_clinicNumCur)) {
				textLocationID.Text=_dictLocationIDs[_clinicNumCur].PropertyValue;
			}
			else {
				textLocationID.Text=_dictLocationIDs[0].PropertyValue;//Default to showing the HQ value when filling info for a clinic with no program property.
			}
		}
		
		///<summary>Updates the in memory dictionary with any changes made to the current locationID for each clinic before showing the next one.</summary>
		private void SaveClinicCurProgramPropertiesToDict() {
			//First check if Headquarters (default) is selected.
			if(_clinicNumCur==0) {
				//Headquarters is selected so only update the location ID (might have changed) on all other location ID properties that match the "old" location ID of HQ.
				if(_dictLocationIDs.ContainsKey(_clinicNumCur)) {
					//Get the location ID so that we correctly update all program properties with a matching location ID.
					string locationIdOld=_dictLocationIDs[_clinicNumCur].PropertyValue;
					foreach(KeyValuePair<long,ProgramProperty> item in _dictLocationIDs) {
						ProgramProperty ppCur=item.Value;
						if(ppCur.PropertyValue==locationIdOld) {
							ppCur.PropertyValue=textLocationID.Text;
						}
					}
				}
				return;//No other clinic specific changes could have been made, we need to return.
			}
			//Update or Insert clinic specific properties into memory
			ProgramProperty ppLocationID=new ProgramProperty();
			if(_dictLocationIDs.ContainsKey(_clinicNumCur)) {
				ppLocationID=_dictLocationIDs[_clinicNumCur];//Override the database's property with what is in memory.
			}
			else {//Get default programproperty from db.
				ppLocationID=ProgramProperties.GetListForProgramAndClinicWithDefault(_progCur.ProgramNum,_clinicNumCur)
					.FirstOrDefault(x => x.PropertyDesc==Podium.PropertyDescs.LocationID);
			}
			if(ppLocationID.ClinicNum==0) {//No program property for current clinic, since _clinicNumCur!=0
				ProgramProperty ppLocationIDNew=ppLocationID.Copy();
				ppLocationIDNew.ProgramPropertyNum=0;
				ppLocationIDNew.ClinicNum=_clinicNumCur;
				ppLocationIDNew.PropertyValue=textLocationID.Text;
				if(!_dictLocationIDs.ContainsKey(_clinicNumCur)) {//Should always happen
					_dictLocationIDs.Add(_clinicNumCur,ppLocationIDNew);
				}
				return;
			}
			//At this point we know that the clinicnum isn't 0 and the database has a property for that clinicnum.
			if(_dictLocationIDs.ContainsKey(_clinicNumCur)) {//Should always happen
				ppLocationID.PropertyValue=textLocationID.Text;
				_dictLocationIDs[_clinicNumCur]=ppLocationID;
			}
			else {
				_dictLocationIDs.Add(_clinicNumCur,ppLocationID);//Should never happen.
			}
		}

		private void FillForm() {
			if(_dictLocationIDs.Count==0) {
				MsgBox.Show(this,"User can't be clinic restricted.");
				DialogResult=DialogResult.Cancel;
				return;
			}
			try {
				checkUseService.Checked=PIn.Bool(_useService.PropertyValue);
				checkShowCommlogsInChart.Checked=PIn.Bool(_showCommlogsInChartAndAccount.PropertyValue);
				checkEnabled.Checked=_progCur.Enabled;
				checkHideButtons.Checked=PIn.Bool(_disableAdvertising.PropertyValue);
				textApptSetComplete.Text=_apptSetCompleteMins.PropertyValue;
				textApptTimeArrived.Text=_apptTimeArrivedMins.PropertyValue;
				textApptTimeDismissed.Text=_apptTimeDismissedMins.PropertyValue;
				textCompNameOrIP.Text=_compName.PropertyValue;
				textAPIToken.Text=_apiToken.PropertyValue;
				if(_dictLocationIDs.ContainsKey(_clinicNumCur)) {
					textLocationID.Text=_dictLocationIDs[_clinicNumCur].PropertyValue;
				}
				else {
					textLocationID.Text=_dictLocationIDs[0].PropertyValue;//Default to showing the HQ value when filling info for a clinic with no program property.
				}
				_existingPatTriggerEnum=PIn.Enum<ReviewInvitationTrigger>(_existingPatTriggerType.PropertyValue);
				_newPatTriggerEnum=PIn.Enum<ReviewInvitationTrigger>(_newPatTriggerType.PropertyValue);
				switch(_existingPatTriggerEnum) {
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
				switch(_newPatTriggerEnum) {
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
			}
			catch(Exception) {
				MsgBox.Show(this,"You are missing a program property from the database.  Please call support to resolve this issue.");
				DialogResult=DialogResult.Cancel;
				return;
			}
		}

		private void RadioButton_CheckChanged(object sender,EventArgs e) {
			if(sender.GetType()!=typeof(RadioButton)) {
				return;
			}
			RadioButton radioButtonCur=(RadioButton)sender;
			if(radioButtonCur.Checked) {
				switch(radioButtonCur.Name) {
					case "radioSetCompleteExistingPat":
						_existingPatTriggerEnum=ReviewInvitationTrigger.AppointmentCompleted;
						break;
					case "radioTimeArrivedExistingPat":
						_existingPatTriggerEnum=ReviewInvitationTrigger.AppointmentTimeArrived;
						break;
					case "radioTimeDismissedExistingPat":
						_existingPatTriggerEnum=ReviewInvitationTrigger.AppointmentTimeDismissed;
						break;
					case "radioSetCompleteNewPat":
						_newPatTriggerEnum=ReviewInvitationTrigger.AppointmentCompleted;
						break;
					case "radioTimeArrivedNewPat":
						_newPatTriggerEnum=ReviewInvitationTrigger.AppointmentTimeArrived;
						break;
					case "radioTimeDismissedNewPat":
						_newPatTriggerEnum=ReviewInvitationTrigger.AppointmentTimeDismissed;
						break;
					default:
						throw new Exception("Unknown Radio Button Name");
				}
			}
		}

		private void SaveProgram() {
			SaveClinicCurProgramPropertiesToDict();
			_progCur.Enabled=checkEnabled.Checked;
			UpdateProgramProperty(_useService,POut.Bool(checkUseService.Checked));
			UpdateProgramProperty(_showCommlogsInChartAndAccount,POut.Bool(checkShowCommlogsInChart.Checked));
			UpdateProgramProperty(_disableAdvertising,POut.Bool(checkHideButtons.Checked));
			UpdateProgramProperty(_apptSetCompleteMins,textApptSetComplete.Text);
			UpdateProgramProperty(_apptTimeArrivedMins,textApptTimeArrived.Text);
			UpdateProgramProperty(_apptTimeDismissedMins,textApptTimeDismissed.Text);
			UpdateProgramProperty(_compName,textCompNameOrIP.Text);
			UpdateProgramProperty(_apiToken,textAPIToken.Text);
			UpdateProgramProperty(_newPatTriggerType,POut.Int((int)_newPatTriggerEnum));
			UpdateProgramProperty(_existingPatTriggerType,POut.Int((int)_existingPatTriggerEnum));
			UpsertProgramPropertiesForClinics();
			Programs.Update(_progCur);
		}

		private void UpdateProgramProperty(ProgramProperty ppFromDb,string newpropertyValue) {
			if(ppFromDb.PropertyValue==newpropertyValue) {
				return;
			}
			ppFromDb.PropertyValue=newpropertyValue;
			ProgramProperties.Update(ppFromDb);
			_hasProgramPropertyChanged=true;
		}

		private void UpsertProgramPropertiesForClinics() {
			List<ProgramProperty> listLocationIDsFromDb=ProgramProperties.GetForProgram(_progCur.ProgramNum).FindAll(x => x.PropertyDesc==Podium.PropertyDescs.LocationID);
			List<ProgramProperty> listLocationIDsCur=_dictLocationIDs.Values.ToList();
			foreach(ProgramProperty ppCur in listLocationIDsCur) {
				if(listLocationIDsFromDb.Exists(x => x.ProgramPropertyNum == ppCur.ProgramPropertyNum)) {
					UpdateProgramProperty(listLocationIDsFromDb[listLocationIDsFromDb.FindIndex(x => x.ProgramPropertyNum == ppCur.ProgramPropertyNum)],ppCur.PropertyValue);//ppCur.PropertyValue will match textLocationID.Text
				}
				else {
					ProgramProperties.Insert(ppCur);//Program property for that clinicnum didn't exist, so insert it into the db.
					_hasProgramPropertyChanged=true;
				}
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