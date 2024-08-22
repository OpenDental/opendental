using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Bridges;
using CodeBase;
using OpenDentalHelp;
using OpenDentBusiness;

namespace OpenDental{
	
	///<summary></summary>
	public class PatientL{
		///<summary>Collection of Patient Names and Patnums for the last five patients. Displayed when clicking the dropdown button.</summary>
		private static List<Patient> _listPatientsLastFive=new List<Patient>();
		///<summary>Static variable to store the currently selected patient for updating the main title bar for the Update Time countdown.
		///Stored so we don't have to make a database call every second.</summary>
		private static Patient _patientSelected;
		///<summary>Static variable to store the currently selected clinicNum for updating the main title bar for the Update Time countdown.
		///Stored so we don't have to make a database call every second.</summary>
		private static long _clinicNumSelected;
		///<summary>Used to lock while changes are made to _listLastFivePatients.</summary>
		private static object _lock=new object();

		///<summary>Gets limited patient information for the patients that are in the patient select history drop down.</summary>
		public static List<Patient> GetPatientsLimFromMenu() {
			return Patients.GetLimForPats(_listPatientsLastFive.Select(x => x.PatNum).ToList(),true);
		}

		///<summary>Removes a patient from the dropdown menu.  Only used when Delete Patient is called.</summary>
		public static void RemoveFromMenu(long patNum) {
			//No need to check MiddleTierRole; no call to db.
			lock(_lock) {//Lock the list before removal to prevent concurrency issues.
				_listPatientsLastFive.RemoveAll(x => x.PatNum==patNum);
			}
		}

		///<summary>Removes all patients from the dropdown menu and from the history.  Only used when a user logs off and a different user logs on.  Important for enterprise customers with clinic restrictions for users.</summary>
		public static void RemoveAllFromMenu(ContextMenu contextMenu) {
			contextMenu.MenuItems.Clear();
			lock(_lock) {//Lock the list before clear to prevent concurrency issues.
				_listPatientsLastFive.Clear();
			}
		}

		///<summary>The current patient will already be on the button.  This adds the family members when user clicks dropdown arrow. Can handle null values for pat and fam.  Need to supply the menu to fill as well as the EventHandler to set for each item (all the same).</summary>
		public static void AddFamilyToMenu(ContextMenu contextMenu,EventHandler eventHandlerOnClick,long patNum,Family family) {
			//No need to check MiddleTierRole; no call to db.
			//fill menu
			contextMenu.MenuItems.Clear();
			if(_listPatientsLastFive.Count==0 && patNum==0) {
				return;//Without this the Select Patient dropdown would only have a bar and FAMILY.
			}
			for(int i=0;i<_listPatientsLastFive.Count;i++) {
				string name=_listPatientsLastFive[i].GetNameLF();
				if(PrefC.IsODHQ) {
					name+=" - "+POut.Long(_listPatientsLastFive[i].PatNum);
				}
				contextMenu.MenuItems.Add(name,eventHandlerOnClick);
			}
			contextMenu.MenuItems.Add("-");
			contextMenu.MenuItems.Add("FAMILY");
			if(patNum!=0 && family!=null) {
				for(int i=0;i<family.ListPats.Length;i++) {
					string name=family.ListPats[i].GetNameLF();
					if(PrefC.IsODHQ) {
						name+=" - "+POut.Long(family.ListPats[i].PatNum);
					}
					contextMenu.MenuItems.Add(name,eventHandlerOnClick);
				}
			}
		}

		///<summary>Does not handle null values. Use zero.  Does not handle adding family members.  Returns true if patient has changed.</summary>
		public static bool AddPatsToMenu(ContextMenu contextMenu,EventHandler eventHandlerOnClick, Patient patient) {
			//No need to check MiddleTierRole; no call to db.
			//add current patient
			if(patient.PatNum==0) {
				return false;
			}
			if(_listPatientsLastFive.Count>0 && patient.PatNum==_listPatientsLastFive[0].PatNum) {//same patient selected
				return false;
			}
			//Patient has changed
			int idx=_listPatientsLastFive.FindIndex(x => x.PatNum==patient.PatNum);
			lock(_lock) {//Lock the list before removal and/or insertion to prevent concurrency issues.
				if(idx>-1) {//It exists in this list of patnums
					_listPatientsLastFive.RemoveAt(idx);
				}
				
				_listPatientsLastFive.Insert(0,patient);
				if(_listPatientsLastFive.Count>5) {
					_listPatientsLastFive.RemoveAt(5);
				}
			}
			return true;
		}

		///<summary>Determines which menu Item was selected from the Patient dropdown list and returns the patNum for that patient. This will not be activated when click on 'FAMILY' or on separator, because they do not have events attached.  Calling class then does a ModuleSelected.</summary>
		public static long ButtonSelect(ContextMenu contextMenu,object sender,Family family) {
			//No need to check MiddleTierRole; no call to db.
			int index=contextMenu.MenuItems.IndexOf((MenuItem)sender);
			//Patients.PatIsLoaded=true;
			if(index<_listPatientsLastFive.Count) {
				return _listPatientsLastFive[index].PatNum;
			}
			if(family==null) {
				return 0;//will never happen
			}
			return family.ListPats[index-_listPatientsLastFive.Count-2].PatNum;
		}

		///<summary>Returns a string representation of the current state of the application designed for display in the main title.
		///Accepts null for pat and 0 for clinicNum.</summary>
		public static string GetMainTitle(Patient patient,long clinicNum) {
			string retVal=PrefC.GetString(PrefName.MainWindowTitle);
			object[] objectArrayParameters = { retVal };
			Plugins.HookAddCode(null,"PatientL.GetMainTitle_beginning",objectArrayParameters);
			retVal = (string)objectArrayParameters[0];
			_patientSelected=patient;
			_clinicNumSelected=clinicNum;
			if(PrefC.HasClinicsEnabled && clinicNum>0) {
				if(retVal!="") {
					retVal+=" - "+Lan.g("FormOpenDental","Clinic")+": ";
				}
				if(PrefC.GetBool(PrefName.TitleBarClinicUseAbbr)) {
					retVal+=Clinics.GetAbbr(clinicNum);
				}
				else {
					retVal+=Clinics.GetDesc(clinicNum);
				}
			}
			if(Security.CurUser!=null) {
				retVal+=" {"+Security.CurUser.UserName+"}";
			}
			if(patient==null || patient.PatNum==0 || patient.PatNum==-1) {
				retVal+=MainTitleUpdateCountdown();
				if(FormOpenDental.IsRegKeyForTesting) {
					retVal+=" - "+Lan.g("FormOpenDental","Developer Only License")+" - "+Lan.g("FormOpenDental","Not for use with live patient data")+" - ";
				}
				return retVal;
			}
			retVal+=" - "+patient.GetNameLF();
			if(PrefC.GetBool(PrefName.TitleBarShowSpecialty)) {
				retVal+=string.IsNullOrWhiteSpace(patient.Specialty) ? "" : " ("+patient.Specialty+")";
			}
			if(PrefC.GetLong(PrefName.ShowIDinTitleBar)==1) {
				retVal+=" - "+patient.PatNum.ToString();
			}
			else if(PrefC.GetLong(PrefName.ShowIDinTitleBar)==2) {
				retVal+=" - "+patient.ChartNumber;
			}
			else if(PrefC.GetLong(PrefName.ShowIDinTitleBar)==3) {
				if(patient.Birthdate.Year>1880) {
					retVal+=" - "+patient.Birthdate.ToShortDateString();
				}
			}
			if(patient.SiteNum!=0) {
				retVal+=" - "+Sites.GetDescription(patient.SiteNum);
			}
			retVal+=MainTitleUpdateCountdown();
			if(FormOpenDental.IsRegKeyForTesting) {
				retVal+=" - "+Lan.g("FormOpenDental","Developer Only License")+" - "+Lan.g("FormOpenDental","Not for use with live patient data")+" - ";
			}
			return retVal;
		}

		///<summary>Used to update the main title bar when neither the patient nor the clinic need to change. 
		///Currently only used to refresh the title bar when the timer ticks for the update time countdown.</summary>
		public static string GetMainTitleSamePat() {
			string retVal=PrefC.GetString(PrefName.MainWindowTitle);
			if(PrefC.HasClinicsEnabled && _clinicNumSelected>0) {
				if(retVal!="") {
					retVal+=" - "+Lan.g("FormOpenDental","Clinic")+": ";
				}
				if(PrefC.GetBool(PrefName.TitleBarClinicUseAbbr)) {
					retVal+=Clinics.GetAbbr(_clinicNumSelected);
				}
				else {
					retVal+=Clinics.GetDesc(_clinicNumSelected);
				}
			}
			if(Security.CurUser!=null) {
				retVal+=" {"+Security.CurUser.UserName+"}";
			}
			if(_patientSelected==null || _patientSelected.PatNum==0 || _patientSelected.PatNum==-1) {
				retVal+=MainTitleUpdateCountdown();
				if(FormOpenDental.IsRegKeyForTesting) {
					retVal+=" - "+Lan.g("FormOpenDental","Developer Only License")+" - "+Lan.g("FormOpenDental","Not for use with live patient data")+" - ";
				}
				//Now check to see if this database has been put into "Testing Mode"
				if(Introspection.IsTestingMode) {
					retVal+=" <TESTING MODE ENABLED> ";
				}
				return retVal;
			}
			retVal+=" - "+_patientSelected.GetNameLF();
			if(PrefC.GetBool(PrefName.TitleBarShowSpecialty)) {
				retVal+=string.IsNullOrWhiteSpace(_patientSelected.Specialty) ? "" : " ("+_patientSelected.Specialty+")";
			}
			if(PrefC.GetLong(PrefName.ShowIDinTitleBar)==1) {
				retVal+=" - "+_patientSelected.PatNum.ToString();
			}
			else if(PrefC.GetLong(PrefName.ShowIDinTitleBar)==2) {
				retVal+=" - "+_patientSelected.ChartNumber;
				object[] objectArray={ _patientSelected,retVal };
				Plugins.HookAddCode(null,"PatientL.GetMainTitleSamePat_afterAddChartNum",objectArray);
				retVal=(string)objectArray[1];
			}
			else if(PrefC.GetLong(PrefName.ShowIDinTitleBar)==3) {
				if(_patientSelected.Birthdate.Year>1880) {
					retVal+=" - "+_patientSelected.Birthdate.ToShortDateString();
				}
			}
			if(_patientSelected.SiteNum!=0) {
				retVal+=" - "+Sites.GetDescription(_patientSelected.SiteNum);
			}
			retVal+=MainTitleUpdateCountdown();
			if(FormOpenDental.IsRegKeyForTesting) {
				retVal+=" - "+Lan.g("FormOpenDental","Developer Only License")+" - "+Lan.g("FormOpenDental","Not for use with live patient data")+" - ";
			}
			//Now check to see if this database has been put into "Testing Mode"
			if(Introspection.IsTestingMode) {
				retVal+=" <TESTING MODE ENABLED> ";
			}
			return retVal;
		}

		///<summary>Sets the cached patient specialty to null so that the main title will refresh the specialty from the database if there is a valid
		///patient selected.</summary>
		public static void InvalidateSelectedPatSpecialty() {
			if(_patientSelected==null) {
				return;
			}
			_patientSelected.Specialty=null;
		}

		public static string MainTitleUpdateCountdown(string titleText = "") {
			string retVal;
			TimeSpan timeSpanLeft=PrefC.GetDateT(PrefName.UpdateDateTime) - DateTime.Now;
			string strTimeLeft="";
			if(timeSpanLeft.TotalSeconds<0){
				return titleText;
			}
			if(timeSpanLeft.Days>=1) {
				strTimeLeft=timeSpanLeft.Days+" "+Lan.g("FormOpenDental","days")+", "+timeSpanLeft.Hours+" "+Lan.g("FormOpenDental","hours");
			}
			else if(timeSpanLeft.Hours>=1) {
				strTimeLeft=timeSpanLeft.Hours+" "+Lan.g("FormOpenDental","hours")+", "+timeSpanLeft.Minutes+" "+Lan.g("FormOpenDental","minutes");
			}
			else if(timeSpanLeft.Minutes>=1) {
				strTimeLeft=timeSpanLeft.Minutes+" "+Lan.g("FormOpenDental","minutes")+", "+timeSpanLeft.Seconds+" "+Lan.g("FormOpenDental","seconds");
			}
			else if(timeSpanLeft.Seconds>=0) {
				strTimeLeft=timeSpanLeft.Seconds+" "+Lan.g("FormOpenDental","seconds");
			}
			retVal=titleText+"        "+Lan.g("FormOpenDental","Update In")+": "+strTimeLeft;
			return retVal;
		}

		///<summary>Checks if the patient is allowed to receive text messages. Also prompts user for whether or not they want to update the patient to allow texts to be received. This method is able to modify the
		///passed in patient object's TxtMsgOk field.</summary>
		public static bool CheckPatientTextingAllowed(Patient patient,object sender) {
			if(patient.TxtMsgOk==YN.Yes) {
				return true;
			}
			bool updateTextYN=false;
			if(patient.TxtMsgOk==YN.No) {
				if(MsgBox.Show(sender,MsgBoxButtons.YesNo,"This patient is marked to not receive text messages. "
					+"Would you like to mark this patient as okay to receive text messages?")) {
					updateTextYN=true;
				}
				else {
					return false;
				}
			}
			if(patient.TxtMsgOk==YN.Unknown && PrefC.GetBool(PrefName.TextMsgOkStatusTreatAsNo)) {
				if(MsgBox.Show(sender,MsgBoxButtons.YesNo,"This patient might not want to receive text messages. "
					+"Would you like to mark this patient as okay to receive text messages?")) {
					updateTextYN=true;
				}
				else {
					return false;
				}
			}
			if(updateTextYN) {
				if(!Security.IsAuthorized(EnumPermType.PatientEdit)) {
					return false;
				}
				Patient patientOld=patient.Copy();
				patient.TxtMsgOk=YN.Yes;
				Patients.Update(patient,patientOld);
				SecurityLogs.MakeLogEntry(EnumPermType.PatientEdit,patient.PatNum,$"Patient TxtMsgOK changed from {patientOld.TxtMsgOk} to {patient.TxtMsgOk}",0,LogSources.None,DateTime.Now,Security.CurUser.UserNum);
			}
			return true;
		}

		///<summary>Returns true when customer is in the USA, has address verification enabled, is not on trial, and is signed up for support. Otherwise returns false</summary>
		public static bool CanVerifyPatientAddressInteraction() {
			if(!System.Globalization.CultureInfo.CurrentCulture.Name.EndsWith("US")) {
				//USPS address validation only works for US addresses.
				return false;
			}
			if(!PrefC.GetBool(PrefName.AddressVerifyWithUSPS)) {
				return false;
			}
			if(ODBuild.IsTrial()) {
				return false;
			}
			if(ODHelp.IsEncryptedKeyValid()) {
				return true;
			}
			return false;
		}

		///<summary>Launches an interaction where a patient address is validated and corrected by USPS. The user is then prompted to choose between the address they provided, the one corrected by USPS, or choosing to return null, and go back to edit. Takes in a sender for translation.</summary>
		public static OpenDentBusiness.Address VerifyPatientAddressInteraction(OpenDentBusiness.Address address, object sender) {
			//if state is more than two characters, try to look up the abbreviation for the state with the name they entered.
			if(string.IsNullOrWhiteSpace(address.State) || string.IsNullOrWhiteSpace(address.Address1)) {
				if(MsgBox.Show(sender,MsgBoxButtons.YesNo, "State and Address1 are required in order for addresses to be verified with USPS. Import anyway?","Warning")) {
					//Just return the address they entered.
					return address;
				}
				else {
					//Returning null means they cancelled the interaction.
					return null;
				}
			}
			if(address.State!="" && address.State.Length>2) {
				StateAbbr stateAbbr=USlocales.ListAll.FirstOrDefault(x=>x.Description.ToUpper()==address.State.ToUpper());
				if(stateAbbr!=null) {
					//Found matching state Abbreviation.
					address.State=stateAbbr.Abbr;
				}
				else {
					//State could not be matched. show error, but allow them to save anyways.
					if(MsgBox.Show(sender,MsgBoxButtons.YesNo, "State could not be matched to a two letter abbreviation. Import anyways?","Warning")) {
						//Just return the address they entered.
						return address;
					}
					else {
						//Returning null means they cancelled the interaction.
						return null;
					}
				}
			}
			address=PatientL.VerifyPatientAddressInteractionHelper(address,sender);
			return address;
		}

		///<summary>Makes call to WSHQ for address validation and processes the result of this call</summary>
		private static OpenDentBusiness.Address VerifyPatientAddressInteractionHelper(OpenDentBusiness.Address address, object sender) {
			OpenDentBusiness.Address addressResult=new OpenDentBusiness.Address();
			USPSAddressValidationResponse uspsAddressValidationResponse=USPSAddressValidationDatas.GetUSPSAddressValidationResponse(address);
			//We want to include zip+4 in zip codes, so we will always show the prompt when we get a response.
			if(uspsAddressValidationResponse==null || uspsAddressValidationResponse.address==null) { 
				if(MsgBox.Show(sender, MsgBoxButtons.YesNo, "Failed to verify address with USPS. (Verification can be turned off in Preferences, Family, General). Save Anyway?", "Warning")) {
					//Import anyway.
					return address;
				}
				else {
					//Cancel import.
					return null;
				}
			}
			//Show comparison prompt form.
			FrmAddressCompare frmAddressCompare=new FrmAddressCompare();
			frmAddressCompare.AddressFromUserInput=address;
			frmAddressCompare.USPSAddressValidationResponse_=uspsAddressValidationResponse;
			bool acceptUSPSCorrections=frmAddressCompare.ShowDialog();
			if(acceptUSPSCorrections) {
				//User clicked the yes button. They accept the changes from USPS.
				//Set the patients fields to the values from USPS	
				addressResult.Address1=uspsAddressValidationResponse.address.streetAddress;
				addressResult.Address2=uspsAddressValidationResponse.address.secondaryAddress;
				addressResult.City=uspsAddressValidationResponse.address.city;
				addressResult.State=uspsAddressValidationResponse.address.state;
				//Combine ZIP5 and ZIP+4
				addressResult.Zip=uspsAddressValidationResponse.address.ZIPCode;
				if(!string.IsNullOrWhiteSpace(uspsAddressValidationResponse.address.ZIPPlus4)) {
					addressResult.Zip=addressResult.Zip+"-"+uspsAddressValidationResponse.address.ZIPPlus4;
				}
			}
			else {
				//User clicked X button to cancel, or No button to use the address they entered verbatim.
				if(!frmAddressCompare.UserChoseAddress) {
					//User clicked X button, return to the import form.
					return null;
				}
				//User Pressed No Button.
				//Ignore USPS corrections, return the original address.
				return address;
			}
			return addressResult;
		}
	}
}
