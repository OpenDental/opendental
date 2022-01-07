using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	
	///<summary></summary>
	public class PatientL{
		///<summary>Collection of Patient Names and Patnums for the last five patients. Displayed when clicking the dropdown button.</summary>
		private static List<ButPat> _listLastFivePatients=new List<ButPat>();
		///<summary>Static variable to store the currently selected patient for updating the main title bar for the Update Time countdown.
		///Stored so we don't have to make a database call every second.</summary>
		private static Patient _patSelectedCur;
		///<summary>Static variable to store the currently selected clinicNum for updating the main title bar for the Update Time countdown.
		///Stored so we don't have to make a database call every second.</summary>
		private static long _clinSelectedCur;
		///<summary>Used to lock while changes are made to _listLastFivePatients.</summary>
		private static object _lock=new object();

		///<summary>Gets limited patient information for the patients that are in the patient select history drop down.</summary>
		public static List<Patient> GetPatientsLimFromMenu() {
			return Patients.GetLimForPats(_listLastFivePatients.Select(x => x.PatNum).ToList(),true);
		}

		///<summary>Removes a patient from the dropdown menu.  Only used when Delete Patient is called.</summary>
		public static void RemoveFromMenu(long patNum) {
			//No need to check RemotingRole; no call to db.
			lock(_lock) {//Lock the list before removal to prevent concurrency issues.
				_listLastFivePatients.RemoveAll(x => x.PatNum==patNum);
			}
		}

		///<summary>Removes all patients from the dropdown menu and from the history.  Only used when a user logs off and a different user logs on.  Important for enterprise customers with clinic restrictions for users.</summary>
		public static void RemoveAllFromMenu(ContextMenu menu) {
			menu.MenuItems.Clear();
			lock(_lock) {//Lock the list before clear to prevent concurrency issues.
				_listLastFivePatients.Clear();
			}
		}

		///<summary>The current patient will already be on the button.  This adds the family members when user clicks dropdown arrow. Can handle null values for pat and fam.  Need to supply the menu to fill as well as the EventHandler to set for each item (all the same).</summary>
		public static void AddFamilyToMenu(ContextMenu menu,EventHandler onClick,long patNum,Family fam) {
			//No need to check RemotingRole; no call to db.
			//fill menu
			menu.MenuItems.Clear();
			if(_listLastFivePatients.Count==0 && patNum==0) {
				return;//Without this the Select Patient dropdown would only have a bar and FAMILY.
			}
			for(int i=0;i<_listLastFivePatients.Count;i++) {
				menu.MenuItems.Add(_listLastFivePatients[i].Name,onClick);
			}
			menu.MenuItems.Add("-");
			menu.MenuItems.Add("FAMILY");
			if(patNum!=0 && fam!=null) {
				for(int i=0;i<fam.ListPats.Length;i++) {
					menu.MenuItems.Add(fam.ListPats[i].GetNameLF(),onClick);
				}
			}
		}

		///<summary>Does not handle null values. Use zero.  Does not handle adding family members.  Returns true if patient has changed.</summary>
		public static bool AddPatsToMenu(ContextMenu menu,EventHandler onClick,string nameLF,long patNum) {
			//No need to check RemotingRole; no call to db.
			//add current patient
			if(patNum==0) {
				return false;
			}
			if(_listLastFivePatients.Count>0 && patNum==_listLastFivePatients[0].PatNum) {//same patient selected
				return false;
			}
			//Patient has changed
			int idx=_listLastFivePatients.FindIndex(x => x.PatNum==patNum);
			lock(_lock) {//Lock the list before removal and/or insertion to prevent concurrency issues.
				if(idx>-1) {//It exists in this list of patnums
					_listLastFivePatients.RemoveAt(idx);
				}
				_listLastFivePatients.Insert(0,new ButPat(patNum,nameLF));
				if(_listLastFivePatients.Count>5) {
					_listLastFivePatients.RemoveAt(5);
				}
			}
			return true;
		}

		///<summary>Determines which menu Item was selected from the Patient dropdown list and returns the patNum for that patient. This will not be activated when click on 'FAMILY' or on separator, because they do not have events attached.  Calling class then does a ModuleSelected.</summary>
		public static long ButtonSelect(ContextMenu menu,object sender,Family fam) {
			//No need to check RemotingRole; no call to db.
			int index=menu.MenuItems.IndexOf((MenuItem)sender);
			//Patients.PatIsLoaded=true;
			if(index<_listLastFivePatients.Count) {
				return _listLastFivePatients[index].PatNum;
			}
			if(fam==null) {
				return 0;//will never happen
			}
			return fam.ListPats[index-_listLastFivePatients.Count-2].PatNum;
		}

		///<summary>Returns a string representation of the current state of the application designed for display in the main title.
		///Accepts null for pat and 0 for clinicNum.</summary>
		public static string GetMainTitle(Patient pat,long clinicNum) {
			string retVal=PrefC.GetString(PrefName.MainWindowTitle);
			object[] parameters = { retVal };
			Plugins.HookAddCode(null,"PatientL.GetMainTitle_beginning",parameters);
			retVal = (string)parameters[0];
			_patSelectedCur=pat;
			_clinSelectedCur=clinicNum;
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
			if(pat==null || pat.PatNum==0 || pat.PatNum==-1) {
				retVal+=MainTitleUpdateCountdown();
				if(FormOpenDental.IsRegKeyForTesting) {
					retVal+=" - "+Lan.g("FormOpenDental","Developer Only License")+" - "+Lan.g("FormOpenDental","Not for use with live patient data")+" - ";
				}
				return retVal;
			}
			retVal+=" - "+pat.GetNameLF();
			if(PrefC.GetBool(PrefName.TitleBarShowSpecialty)) {
				retVal+=string.IsNullOrWhiteSpace(pat.Specialty) ? "" : " ("+pat.Specialty+")";
			}
			if(PrefC.GetLong(PrefName.ShowIDinTitleBar)==1) {
				retVal+=" - "+pat.PatNum.ToString();
			}
			else if(PrefC.GetLong(PrefName.ShowIDinTitleBar)==2) {
				retVal+=" - "+pat.ChartNumber;
			}
			else if(PrefC.GetLong(PrefName.ShowIDinTitleBar)==3) {
				if(pat.Birthdate.Year>1880) {
					retVal+=" - "+pat.Birthdate.ToShortDateString();
				}
			}
			if(pat.SiteNum!=0) {
				retVal+=" - "+Sites.GetDescription(pat.SiteNum);
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
			if(PrefC.HasClinicsEnabled && _clinSelectedCur>0) {
				if(retVal!="") {
					retVal+=" - "+Lan.g("FormOpenDental","Clinic")+": ";
				}
				if(PrefC.GetBool(PrefName.TitleBarClinicUseAbbr)) {
					retVal+=Clinics.GetAbbr(_clinSelectedCur);
				}
				else {
					retVal+=Clinics.GetDesc(_clinSelectedCur);
				}
			}
			if(Security.CurUser!=null) {
				retVal+=" {"+Security.CurUser.UserName+"}";
			}
			if(_patSelectedCur==null || _patSelectedCur.PatNum==0 || _patSelectedCur.PatNum==-1) {
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
			retVal+=" - "+_patSelectedCur.GetNameLF();
			if(PrefC.GetBool(PrefName.TitleBarShowSpecialty)) {
				retVal+=string.IsNullOrWhiteSpace(_patSelectedCur.Specialty) ? "" : " ("+_patSelectedCur.Specialty+")";
			}
			if(PrefC.GetLong(PrefName.ShowIDinTitleBar)==1) {
				retVal+=" - "+_patSelectedCur.PatNum.ToString();
			}
			else if(PrefC.GetLong(PrefName.ShowIDinTitleBar)==2) {
				retVal+=" - "+_patSelectedCur.ChartNumber;
			}
			else if(PrefC.GetLong(PrefName.ShowIDinTitleBar)==3) {
				if(_patSelectedCur.Birthdate.Year>1880) {
					retVal+=" - "+_patSelectedCur.Birthdate.ToShortDateString();
				}
			}
			if(_patSelectedCur.SiteNum!=0) {
				retVal+=" - "+Sites.GetDescription(_patSelectedCur.SiteNum);
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
			if(_patSelectedCur==null) {
				return;
			}
			_patSelectedCur.Specialty=null;
		}

		public static string MainTitleUpdateCountdown(string titleText = "") {
			string retVal;
			TimeSpan timeLeft=PrefC.GetDateT(PrefName.UpdateDateTime) - DateTime.Now;
			string strTimeLeft="";
			if(timeLeft.TotalSeconds>=0) {
				if(timeLeft.Days>=1) {
					strTimeLeft=timeLeft.Days+" "+Lan.g("FormOpenDental","days")+", "+timeLeft.Hours+" "+Lan.g("FormOpenDental","hours");
				}
				else if(timeLeft.Hours>=1) {
					strTimeLeft=timeLeft.Hours+" "+Lan.g("FormOpenDental","hours")+", "+timeLeft.Minutes+" "+Lan.g("FormOpenDental","minutes");
				}
				else if(timeLeft.Minutes>=1) {
					strTimeLeft=timeLeft.Minutes+" "+Lan.g("FormOpenDental","minutes")+", "+timeLeft.Seconds+" "+Lan.g("FormOpenDental","seconds");
				}
				else if(timeLeft.Seconds>=0) {
					strTimeLeft=timeLeft.Seconds+" "+Lan.g("FormOpenDental","seconds");
				}
			}
			else {
				return titleText;
			}
			retVal=titleText+"        "+Lan.g("FormOpenDental","Update In")+": "+strTimeLeft;
			return retVal;
		}

		///<summary>Internal class of Patient.PatNum and Patient First & Last Names (nameFL) used for presentation on Patient Select button.</summary>
		private class ButPat {
			public long PatNum;
			public string Name;

			public ButPat(long patNum,string nameLF) {
				PatNum=patNum;
				Name=nameLF;
			}
		}

	}
}










