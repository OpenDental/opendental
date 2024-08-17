using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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
				contextMenu.MenuItems.Add(name,eventHandlerOnClick);
			}
			contextMenu.MenuItems.Add("-");
			contextMenu.MenuItems.Add("FAMILY");
			if(patNum!=0 && family!=null) {
				for(int i=0;i<family.ListPats.Length;i++) {
					contextMenu.MenuItems.Add(family.ListPats[i].GetNameLF(),eventHandlerOnClick);
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

	}
}
