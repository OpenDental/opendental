using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormTransworldSetup:FormODBase {
		private Program _program;
		///<summary>Local cache of all of the clinic nums the current user has permission to access at the time the form loads.  Filled at the same time
		///as comboClinic and is used to set programproperty.ClinicNum when saving.</summary>
		private List<long> _listClinicNumsUser;
		///<summary>Dictionary is ClinicNum key with the list of Transworld program properties for the clinic and will be filled with all prog props in
		///the db for Transworld on load.  If a clinic is synced with HQ, i.e. all props for the clinic match those for HQ, the props for that clinic will
		///be deleted from the dict and on form close the sync will delete them from the db and the form will display the HQ 'clinic' details.  Any edit
		///to a clinic synced with HQ will break the sync.</summary>
		private Dictionary<long,List<ProgramProperty>> _dictionaryClinicListProgProps;
		///<summary>If the user changes clinics, this will hold the previous ClinicNum used to save the form details to the local list of props before
		///loading the new clinic props.</summary>
		private long _selectedClinicNum;
		///<summary>Contains all the program properties that existed when we opened the form. Does not get updated when a clinic's set of program properties 
		///are added or removed from _dictClinicListProgProps. Used for logging purposes.</summary>
		private List<ProgramProperty> _listProgramProperties;

		public FormTransworldSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTransworldSetup_Load(object sender,EventArgs e) {
			_program=Programs.GetCur(ProgramName.Transworld);
			if(_program==null) {
				MsgBox.Show(this,"The Transworld entry is missing from the database.  Please contact support.");//should never happen
				return;
			}
			checkEnabled.Checked=_program.Enabled;
			if(!PrefC.HasClinicsEnabled) {//clinics are not enabled, use ClinicNum 0 to indicate 'Headquarters' or practice level program properties
				checkEnabled.Text=Lan.g(this,"Enabled");
				groupClinicSettings.Text=Lan.g(this,"Transworld Settings");
				comboClinic.Visible=false;
				labelClinic.Visible=false;
				labelClinicEnable.Visible=false;
				_listClinicNumsUser=new List<long>() { 0 };//if clinics are disabled, programproperty.ClinicNum will be set to 0
				groupSendActivity.Text=Lan.g(this,"Account Activity Updates");//remove '(affects all clinics)' from text
				_selectedClinicNum=0;
			}
			else {//Using clinics
				groupClinicSettings.Text=Lan.g(this,"Transworld Clinic Settings");
				_listClinicNumsUser=new List<long>();
				//if Transworld is enabled and the user is restricted to a clinic, don't allow the user to disable for all clinics
				if(Security.CurUser.ClinicIsRestricted) {
					if(checkEnabled.Checked) {
						checkEnabled.Enabled=false;
					}
				}
				else {
					comboClinic.Items.Add(Lan.g(this,"Headquarters"));
					//this way both lists have the same number of items in it and if 'Headquarters' is selected the programproperty.ClinicNum will be set to 0
					_listClinicNumsUser.Add(0);
					comboClinic.SelectedIndex=0;
				}
				List<Clinic> listClinics=Clinics.GetForUserod(Security.CurUser);
				for(int i = 0;i<listClinics.Count;i++) {
					comboClinic.Items.Add(listClinics[i].Abbr);
					_listClinicNumsUser.Add(listClinics[i].ClinicNum);
					if(Clinics.ClinicNum==listClinics[i].ClinicNum) {//set selected index to the currently selected clinic in FormOpenDental
						_selectedClinicNum=listClinics[i].ClinicNum;
						comboClinic.SelectedIndex=comboClinic.Items.Count-1;
					}
				}
			}
			_listProgramProperties=ProgramProperties.GetForProgram(_program.ProgramNum);//get list of all props for the program
			_dictionaryClinicListProgProps=_listProgramProperties
				.GroupBy(x => x.ClinicNum)//group each clinic
				.ToDictionary(x => x.Key,x => x.ToList());//turn list into a dictionary of key=ClinicNum, value=List<ProgramProperty> for the clinic
			DateTime dateTimeSend=PrefC.GetDateT(PrefName.TransworldServiceTimeDue);
			if(dateTimeSend!=DateTime.MinValue) {
				textUpdatesTimeOfDay.Text=dateTimeSend.ToShortTimeString();
			}
			comboSendFrequencyUnits.Items.AddList(Enum.GetNames(typeof(FrequencyUnit)));
			string[] stringArraySendFreqs=PrefC.GetString(PrefName.TransworldServiceSendFrequency).Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
			if(stringArraySendFreqs.Length==2) {
				int sendFreq=PIn.Int(stringArraySendFreqs[0],false);
				FrequencyUnit sendFreqUnit;
				if(sendFreq>0 && Enum.TryParse(stringArraySendFreqs[1],out sendFreqUnit)) {
					numericSendFrequency.Value=sendFreq;
					comboSendFrequencyUnits.SelectedIndex=(int)sendFreqUnit;
				}
			}
			else {//if not set, default to repeat once a day
				comboSendFrequencyUnits.SelectedIndex=(int)FrequencyUnit.Days;
				numericSendFrequency.Value=1;
			}
			for(int i=0;i<_listProgramProperties.Count;i++){
				if(_listProgramProperties[i].IsHighSecurity){
					_listProgramProperties[i].TagOD=_listProgramProperties[i].PropertyValue;
				}
			}
			if(!ProgramProperties.CanEditProperties(_listProgramProperties)) {
				textSftpPassword.ReadOnly=true;
			}
			FillComboBoxes();
			FillFields();
		}

		/// <summary>Fill the combo boxes with items. Some will have their indicies set later in FillFields() </summary>
		private void FillComboBoxes() {
			comboPaidInFullBillType.Items.AddDefs(Defs.GetDefsForCategory(DefCat.BillingTypes,true).Where(x => x.ItemValue.ToLower()!="c").ToList());
			comboPaidInFullBillType.SetSelectedDefNum(PrefC.GetLong(PrefName.TransworldPaidInFullBillingType));
			comboPosAdjType.Items.AddDefNone();
			comboPosAdjType.SetSelected(0);
			comboPosAdjType.Items.AddDefs(Defs.GetDefsForCategory(DefCat.AdjTypes,true).Where(x => x.ItemValue.Contains("+")).ToList());
			comboNegAdjType.Items.AddDefNone();
			comboNegAdjType.SetSelected(0);
			comboNegAdjType.Items.AddDefs(Defs.GetDefsForCategory(DefCat.AdjTypes,true).Where(x => x.ItemValue.Contains("-")).ToList());
		}

		///<summary>Details displayed in form may be for HQ clinic and not the currently selected clinic if the current clinic is synced with HQ.  If the
		///values are modified and the currently selected clinic is not the HQ clinic but the HQ details are being displayed, the HQ clinic details will
		///remain unchanged and the currently selected clinic will no longer be synced with HQ and will have a set of props added to the dict.</summary>
		private void FillFields() {
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=_listClinicNumsUser[comboClinic.SelectedIndex];
			}
			List<ProgramProperty> listProgramPropertiesClinic;
			if(!_dictionaryClinicListProgProps.TryGetValue(clinicNum,out listProgramPropertiesClinic)) {
				listProgramPropertiesClinic=_dictionaryClinicListProgProps[0];//dictionary guaranteed to have ClinicNum 0 in it
			}
			for(int i = 0;i<listProgramPropertiesClinic.Count;i++) {
				switch(listProgramPropertiesClinic[i].PropertyDesc) {
					case "SftpServerAddress":
						textSftpAddress.Text=listProgramPropertiesClinic[i].PropertyValue;
						continue;
					case "SftpServerPort":
						textSftpPort.Text=listProgramPropertiesClinic[i].PropertyValue;
						continue;
					case "SftpUsername":
						textSftpUsername.Text=listProgramPropertiesClinic[i].PropertyValue;
						continue;
					case "SftpPassword":
						textSftpPassword.Text=CDT.Class1.TryDecrypt(listProgramPropertiesClinic[i].PropertyValue);
						continue;
					case "ClientIdAccelerator":
						textClientIdAccelerator.Text=listProgramPropertiesClinic[i].PropertyValue;
						continue;
					case "ClientIdCollection":
						textClientIdCollection.Text=listProgramPropertiesClinic[i].PropertyValue;
						continue;
					case "IsThankYouLetterEnabled":
						checkThankYouLetter.Checked=PIn.Bool(listProgramPropertiesClinic[i].PropertyValue);
						continue;
					case "SelectedServices":
						checkAccelService.Checked=listProgramPropertiesClinic[i].PropertyValue.Contains(((int)TsiServiceType.Accelerator).ToString());
						checkPRService.Checked=listProgramPropertiesClinic[i].PropertyValue.Contains(((int)TsiServiceType.ProfitRecovery).ToString());
						checkCollService.Checked=listProgramPropertiesClinic[i].PropertyValue.Contains(((int)TsiServiceType.ProfessionalCollections).ToString());
						continue;
					case "SyncExcludePosAdjType":
							comboPosAdjType.SetSelectedDefNum(PIn.Long(listProgramPropertiesClinic[i].PropertyValue)); 
						continue;
					case "SyncExcludeNegAdjType":
							comboNegAdjType.SetSelectedDefNum(PIn.Long(listProgramPropertiesClinic[i].PropertyValue));
						continue;
				}
			}
			SetAdvertising();
		}
		
		///<summary>Handles both visibility and checking of checkHideButtons.</summary>
		private void SetAdvertising() {
			ProgramProperty programProperty=_dictionaryClinicListProgProps[0].FirstOrDefault(x => x.PropertyDesc=="Disable Advertising");//dict guaranteed to contain key 0
			checkHideButtons.Visible=(programProperty!=null && !checkEnabled.Checked);//show check box if disable prop exists and program is not enabled
			checkHideButtons.Checked=(programProperty?.PropertyValue=="1");//check box checked if disable prop exists and is set to "1" for HQ
		}

		private void checkEnabled_Click(object sender,EventArgs e) {
			SetAdvertising();
		}
		
		///<summary>Saves form data to the dict and then removes any clinics from the dict that exactly match the HQ clinic details.  If editing a clinic
		///other than HQ and there are no props for that clinic and the form values are different than HQ, a new list is added to the dict.</summary>
		private void SyncWithHQ() {
			List<ProgramProperty> listProgramPropertiesHq=_dictionaryClinicListProgProps[0];//dict guaranteed to contain ClinicNum 0
			List<ProgramProperty> listProgramProperties;
			if(!_dictionaryClinicListProgProps.TryGetValue(_selectedClinicNum,out listProgramProperties)) {
				//if there isn't a list of props for the clinic, create a new list for comparison and possibly for inserting for the clinic
				listProgramProperties=listProgramPropertiesHq.Select(x => new ProgramProperty() {
					ProgramNum=x.ProgramNum,
					ClinicNum=_selectedClinicNum,
					PropertyDesc=x.PropertyDesc,
					IsMasked=x.IsMasked,
					IsHighSecurity=x.IsHighSecurity
				}).ToList();
			}
			//these are the props that will be synced with HQ and used to determine whether a clinic's props should be deleted or used instead of the HQ props
			//the disable advert and disable advert HQ props are only for the HQ clinic and will be ignored for syncing a clinic
			string[] stringArraySyncedProps=new[] { "SftpServerAddress","SftpServerPort","SftpUsername","SftpPassword","ClientIdAccelerator","ClientIdCollection",
				"IsThankYouLetterEnabled","SelectedServices","SyncExcludePosAdjType","SyncExcludeNegAdjType" };
			for(int i = 0;i<listProgramProperties.Count;i++) {//update the currently selected props with the current form values
				switch(listProgramProperties[i].PropertyDesc) {
					case "SftpServerAddress":
						listProgramProperties[i].PropertyValue=textSftpAddress.Text;
						continue;
					case "SftpServerPort":
						listProgramProperties[i].PropertyValue=textSftpPort.Text;
						continue;
					case "SftpUsername":
						listProgramProperties[i].PropertyValue=textSftpUsername.Text;
						continue;
					case "SftpPassword":
						listProgramProperties[i].PropertyValue=CDT.Class1.TryEncrypt(textSftpPassword.Text);
						continue;
					case "ClientIdAccelerator":
						listProgramProperties[i].PropertyValue=textClientIdAccelerator.Text;
						continue;
					case "ClientIdCollection":
						listProgramProperties[i].PropertyValue=textClientIdCollection.Text;
						continue;
					case "IsThankYouLetterEnabled":
						listProgramProperties[i].PropertyValue=POut.Bool(checkThankYouLetter.Checked);
						continue;
					case "Disable Advertising":
						_dictionaryClinicListProgProps.Values.SelectMany(x => x.Where(y => y.PropertyDesc=="Disable Advertising")).ToList()
							.ForEach(y => y.PropertyValue=POut.Bool(checkHideButtons.Checked));
						listProgramProperties[i].PropertyValue=POut.Bool(checkHideButtons.Checked);//in case list is for a new clinic and not in dict
						continue;
					case "Disable Advertising HQ":
						//false if prop is null or if the value is anything but "1"
						bool isAdvertDisabledHQ=(listProgramPropertiesHq.FirstOrDefault(x => x.PropertyDesc=="Disable Advertising HQ")?.PropertyValue=="1");
						_dictionaryClinicListProgProps.Values.SelectMany(x => x.Where(y => y.PropertyDesc=="Disable Advertising HQ")).ToList()
							.ForEach(x => x.PropertyValue=POut.Bool(isAdvertDisabledHQ));//in case list is for a new clinic and not in dict
						listProgramProperties[i].PropertyValue=POut.Bool(isAdvertDisabledHQ);
						continue;
					case "SelectedServices":
						List<int> listSelectedServices=new List<int>();
						if(checkAccelService.Checked) {
							listSelectedServices.Add((int)TsiServiceType.Accelerator);
						}
						if(checkPRService.Checked) {
							listSelectedServices.Add((int)TsiServiceType.ProfitRecovery);
						}
						if(checkCollService.Checked) {
							listSelectedServices.Add((int)TsiServiceType.ProfessionalCollections);
						}
						listProgramProperties[i].PropertyValue=string.Join(",",listSelectedServices);
						continue;
					case "SyncExcludePosAdjType":
							listProgramProperties[i].PropertyValue=comboPosAdjType.GetSelectedDefNum().ToString();
						continue;
					case "SyncExcludeNegAdjType":
							listProgramProperties[i].PropertyValue=comboNegAdjType.GetSelectedDefNum().ToString();
						continue;
				}
			}
			if(_selectedClinicNum==0) {//if HQ selected
				_dictionaryClinicListProgProps.ToList()//go through all clinic properties
					.RemoveAll(x => x.Key>0 //remove the non-HQ clinic props
						&& x.Value.All(y => !stringArraySyncedProps.Contains(y.PropertyDesc)//the prop is an HQ only prop, i.e. the clinic prop is ignored
							|| listProgramPropertiesHq.Any(z => z.PropertyDesc==y.PropertyDesc && z.PropertyValue==y.PropertyValue)));//have matching HQ prop desc and value
				return;
			}
			if(listProgramProperties.All(x => !stringArraySyncedProps.Contains(x.PropertyDesc)//if all props for the non-HQ clinic are HQ only props, i.e. the clinic prop is ignored
				|| listProgramPropertiesHq.Any(y => y.PropertyDesc==x.PropertyDesc && y.PropertyValue==x.PropertyValue)))//have matching HQ prop desc and value
			{
				//remove non-HQ clinic props, they are synced with HQ
				_dictionaryClinicListProgProps.Remove(_selectedClinicNum);//does not throw exception if dict doesn't contain the key! (according to MSDN)
			}
			else if(!_dictionaryClinicListProgProps.ContainsKey(_selectedClinicNum)) {//if the clinic is not in the dict (otherwise values are already updated)
				//add non-HQ clinic to the dict if any prop for the non-HQ clinic is different than the HQ clinic and it's not already in the dict
				_dictionaryClinicListProgProps.Add(_selectedClinicNum,listProgramProperties);
			}
		}

		private void MakeLogEntry(ProgramProperty programProperty) {
			StringBuilder stringBuilderLogText=new StringBuilder();
			stringBuilderLogText.Append(_program.ProgDesc+"'s "+programProperty.PropertyDesc);
			if(PrefC.HasClinicsEnabled) {
				if(programProperty.ClinicNum==0) {
					stringBuilderLogText.Append(" for Headquarters");
				}
				else {
					string clinicDesc=Clinics.GetDesc(programProperty.ClinicNum);
					stringBuilderLogText.Append(" for clinic "+clinicDesc);
				}
			}
			stringBuilderLogText.Append(" was altered.");
			SecurityLogs.MakeLogEntry(EnumPermType.ManageHighSecurityProgProperties,0,stringBuilderLogText.ToString(),_program.ProgramNum,DateTime.Now);
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboClinic.SelectedIndex<0
				|| comboClinic.SelectedIndex>=_listClinicNumsUser.Count
				|| _listClinicNumsUser[comboClinic.SelectedIndex]==_selectedClinicNum)//didn't change the selected clinic
			{
				return;
			}
			if(!textSftpPort.IsValid()) {
				MsgBox.Show(this,"Please enter a valid integer for the Sftp Server Port.");
				comboClinic.SelectedIndex=_listClinicNumsUser.IndexOf(_selectedClinicNum);
				return;
			}
			//save form values to dict before filling fields with selected clinic data
			SyncWithHQ();
			_selectedClinicNum=_listClinicNumsUser[comboClinic.SelectedIndex];
			FillFields();
		}

		private void comboSendFrequencyUnits_SelectedIndexChanged(object sender,EventArgs e) {
			if(comboSendFrequencyUnits.SelectedIndex<0 || comboSendFrequencyUnits.SelectedIndex>Enum.GetNames(typeof(FrequencyUnit)).Length) {
				return;
			}			
			if(comboSendFrequencyUnits.SelectedIndex==(int)FrequencyUnit.Days) {
				numericSendFrequency.Value=Math.Min(numericSendFrequency.Value,30);
				numericSendFrequency.Maximum=30;
			}
			else if(comboSendFrequencyUnits.SelectedIndex==(int)FrequencyUnit.Hours) {
				numericSendFrequency.Value=Math.Min(numericSendFrequency.Value,24);
				numericSendFrequency.Maximum=24;
			}
			else if(comboSendFrequencyUnits.SelectedIndex==(int)FrequencyUnit.Minutes) {
				numericSendFrequency.Value=Math.Min(numericSendFrequency.Value,60);
				numericSendFrequency.Maximum=60;
			}
		}

		private void checkAccelService_CheckedChanged(object sender,EventArgs e) {
			textClientIdAccelerator.Enabled=checkAccelService.Checked;
			labelClientIdAccelerator.Enabled=checkAccelService.Checked;
		}

		private void checkPRService_CheckedChanged(object sender,EventArgs e) {
			textClientIdCollection.Enabled=(checkPRService.Checked || checkCollService.Checked);
			labelClientIdCollection.Enabled=(checkPRService.Checked || checkCollService.Checked);
		}

		private void checkCollService_CheckedChanged(object sender,EventArgs e) {
			textClientIdCollection.Enabled=(checkPRService.Checked || checkCollService.Checked);
			labelClientIdCollection.Enabled=(checkPRService.Checked || checkCollService.Checked);
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(checkEnabled.Checked && !Programs.IsEnabledByHq(ProgramName.Transworld,out string err)) {
				MessageBox.Show(err);
				return;
			}
			if(!textSftpPort.IsValid()) {
				MsgBox.Show(this,"Please enter a valid integer for the Sftp Server Port.");
				return;
			}
			int sendFreq=(int)numericSendFrequency.Value;
			DateTime accountUpdatesRuntime=DateTime.MinValue;
			if(!string.IsNullOrWhiteSpace(textUpdatesTimeOfDay.Text) && !DateTime.TryParse(textUpdatesTimeOfDay.Text,out accountUpdatesRuntime)) {
				MsgBox.Show(this,"Account Updates Run Time must be blank or a valid time of day.");
				return;
			}
			if(comboSendFrequencyUnits.SelectedIndex<0 || comboSendFrequencyUnits.SelectedIndex>=Enum.GetNames(typeof(FrequencyUnit)).Length) {
				//shouldn't be possible, but just in case
				MsgBox.Show(this,"Please select a valid unit of measurement for the Account Activity Updates repeat frequency.");
				return;
			}
			if(numericSendFrequency.Value<1 || numericSendFrequency.Value>new[] { 30,24,60 }[comboSendFrequencyUnits.SelectedIndex]) {
				//shouldn't be possible, but just in case
				MsgBox.Show(this,"Please enter a valid value for the Account Activity Updates repeat frequency.");
				return;
			}
			long billTypePaidInFullDefNum=comboPaidInFullBillType.GetSelectedDefNum();
			if(billTypePaidInFullDefNum==0 && checkEnabled.Checked) {
				MsgBox.Show(this,"Please select a Paid in Full Billing Type.");
				return;
			}
			SyncWithHQ();//will remove any clinic from the dict if all props exactly match the HQ props, or add clinic props if different
			if(_program.Enabled!=checkEnabled.Checked) {//only update the program if the IsEnabled flag has changed
				_program.Enabled=checkEnabled.Checked;
				Programs.Update(_program);
			}
			ProgramProperties.Sync(_dictionaryClinicListProgProps.Where(x => _listClinicNumsUser.Contains(x.Key)).SelectMany(x => x.Value).ToList(),_program.ProgramNum,_listClinicNumsUser);
			DataValid.SetInvalid(InvalidType.Programs);
			string updateFreq=numericSendFrequency.Value+" "+(FrequencyUnit)comboSendFrequencyUnits.SelectedIndex;
			bool hasChanged=false;
			if(Prefs.UpdateString(PrefName.TransworldServiceTimeDue,accountUpdatesRuntime==DateTime.MinValue?"":POut.Time(accountUpdatesRuntime.TimeOfDay,false))
				| Prefs.UpdateString(PrefName.TransworldServiceSendFrequency,updateFreq))
			{
				Prefs.UpdateDateT(PrefName.TransworldDateTimeLastUpdated,DateTime.MinValue);
				hasChanged=true;
			}
			if(Prefs.UpdateLong(PrefName.TransworldPaidInFullBillingType,billTypePaidInFullDefNum) | hasChanged) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			//The code below is for audit trail purposes. Will create a security log whenever a high security property value is changed for a clinic.
			for(int i=0;i<_listProgramProperties.Count;i++){//Iterates through the original properties for each clinic as found when we load this form
				if(_listProgramProperties[i].IsHighSecurity) { 
					List<ProgramProperty> listProgramPropertiesClinic;
					//Checks if the clinic still has its own set of properties, if so uses those to check if any properties have changed. 
					if(!_dictionaryClinicListProgProps.TryGetValue(_listProgramProperties[i].ClinicNum,out listProgramPropertiesClinic)) {
						//Otherwise, if the clinic no longer has its own set of properties, and uses the HQ properties, we will check against the HQ properties for any changes. 
						_dictionaryClinicListProgProps.TryGetValue(0,out listProgramPropertiesClinic);
					}
					//Checks if the original property values is different from the current property value, if so, create logtext and make log entry.
					//We've already verified this property is high security AND we have determined that we're comparing properties within the same clinic/comparing the clinic with HQ
					if(listProgramPropertiesClinic.Any(x=>x.PropertyDesc==_listProgramProperties[i].PropertyDesc && x.PropertyValue!=_listProgramProperties[i].TagOD.ToString())) {
						MakeLogEntry(_listProgramProperties[i]);
					}
				}
			}
			//Deletes the clinics that had (and may still have) their own set of properties, so that we can then focus on the clinics that have now been added while in this form.
			List<long> listClinicNums=_listProgramProperties.Select(x => x.ClinicNum).Where(y => y!=0).Distinct().ToList();//Grab non-HQ clinics
			for(int i=0;i<listClinicNums.Count;i++) {
				_dictionaryClinicListProgProps.Remove(listClinicNums[i]);
			}
			//Flatten the dictionary and get the properties for the clinics that didn't have their own set of properties before. 
			List<ProgramProperty> listProgramPropertiesNew=_dictionaryClinicListProgProps.Where(x => x.Key!=0).SelectMany(x=> x.Value).ToList();
			_dictionaryClinicListProgProps.TryGetValue(0,out List<ProgramProperty> listProgramPropertiesHQ); //Get the HQ clinic set of properties.
			for(int i=0;i<listProgramPropertiesNew.Count;i++) {
				if(listProgramPropertiesHQ.IsNullOrEmpty()) {
					break;
				}
				// If the high security property value is different than the HQ property value, then we create the logtext and make log entry.
				if(listProgramPropertiesNew[i].IsHighSecurity && 
					listProgramPropertiesHQ.Any(x=> x.PropertyDesc==listProgramPropertiesNew[i].PropertyDesc && x.PropertyValue!=listProgramPropertiesNew[i].PropertyValue))
				{
					MakeLogEntry(listProgramPropertiesNew[i]);
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}