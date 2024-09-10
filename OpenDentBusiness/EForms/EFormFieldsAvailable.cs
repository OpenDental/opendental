using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	public class EFormFieldsAvailable {
		//Any changes to the lists below should be duplicated in EFormFiller and EFormImport.

		public static List<string> GetList_RadioButtons(){
			List<string> listStrings=new List<string>();
			//We don't divide them up by type. All fields are available on all types.
			//if(enumEFormType==EnumEFormType.PatientForm){
			listStrings.Add("None");//Required for UI to work properly
			listStrings.Add("allergy:");
			listStrings.Add("Gender");
			listStrings.Add("ins1Relat");
			listStrings.Add("ins2Relat");
			listStrings.Add("Position");
			listStrings.Add("PreferConfirmMethod");
			listStrings.Add("PreferContactMethod");
			listStrings.Add("PreferRecallMethod");
			listStrings.Add("problem:");
			listStrings.Add("StudentStatus");
			//"Race" can't be added yet because it requires multiselect listbox or similar. And since race is stored in a separate table, this is a lot more like meds/allerg/probs.
			//But we could add it in the future with some work. listStrings=Enum.GetNames(typeof(PatientRaceOld)).ToList();
			//For now, we could just add it with no DbLink, so it would not be importable.
			return listStrings;
		}

		public static List<string> GetList_TextBox(){
			List<string> listStrings=new List<string>();
			listStrings.Add("None");//Required for UI to work properly
			listStrings.Add("Address");
			listStrings.Add("Address2");
			listStrings.Add("allergiesOther");
			listStrings.Add("City");
			listStrings.Add("Email");
			listStrings.Add("FName");
			listStrings.Add("HmPhone");
			listStrings.Add("ICEName");
			listStrings.Add("ICEPhone");
			listStrings.Add("ins1CarrierName");
			listStrings.Add("ins1CarrierPhone");
			listStrings.Add("ins1EmployerName");
			listStrings.Add("ins1GroupName");
			listStrings.Add("ins1GroupNum");
			listStrings.Add("ins1SubscriberID");
			listStrings.Add("ins1SubscriberNameF");
			listStrings.Add("ins2CarrierName");
			listStrings.Add("ins2CarrierPhone");
			listStrings.Add("ins2EmployerName");
			listStrings.Add("ins2GroupName");
			listStrings.Add("ins2GroupNum");
			listStrings.Add("ins2SubscriberID");
			listStrings.Add("ins2SubscriberNameF");
			listStrings.Add("LName");
			//listStrings.Add("medsOther");
			listStrings.Add("MiddleI");
			listStrings.Add("Preferred");
			listStrings.Add("problemsOther");
			listStrings.Add("referredFrom");
			listStrings.Add("SSN");
			listStrings.Add("State");
			listStrings.Add("StateNoValidation");//We assume this means that the UI won't enforce two uppercase letters.
			listStrings.Add("WirelessPhone");
			listStrings.Add("WkPhone");
			listStrings.Add("Zip");
			return listStrings;
		}

		public static List<string> GetList_DateField(){
			List<string> listStrings=new List<string>();
			listStrings.Add("None");
			listStrings.Add("Birthdate");
			return listStrings;
		}

		public static List<string> GetList_CheckBox(){
			List<string> listStrings=new List<string>();
			listStrings.Add("None");//Required for UI to work properly
			//"addressAndHmPhoneIsSameEntireFamily"
			listStrings.Add("allergy:");
			listStrings.Add("allergiesNone");
			//listStrings.Add("med:");
			//listStrings.Add("medsNone");
			listStrings.Add("problem:");
			listStrings.Add("problemsNone");
			return listStrings;
		}

		///<summary>For a given fieldName, returns a picklist of all possible Db options.</summary>
		public static List<string> GetRadioDbAll(string fieldName) {
			List<string> listStrings=new List<string>();
			if(fieldName.StartsWith("allergy:")
				|| fieldName.StartsWith("problem:"))
			{
				listStrings.Add("Y");
				listStrings.Add("N");
				return listStrings;
			}
			switch(fieldName) {
				case "None":
				default:
					return listStrings;
				case "Gender":
					listStrings.Add("Male");
					listStrings.Add("Female");
					listStrings.Add("Unknown");
					listStrings.Add("Other");
					break;
				case "ins1Relat":
				case "ins2Relat":
					listStrings=Enum.GetNames(typeof(Relat)).ToList();//there are 9, mostly useless
					break;
				case "Position":
					listStrings.Add("Single");
					listStrings.Add("Married");
					listStrings.Add("Child");
					listStrings.Add("Widowed");
					listStrings.Add("Divorced");
					
					break;
				case "PreferContactMethod":
				case "PreferConfirmMethod":
				case "PreferRecallMethod":
					listStrings=Enum.GetNames(typeof(ContactMethod)).ToList();
					break;
				case "StudentStatus":
					listStrings.Add("N");
					listStrings.Add("P");
					listStrings.Add("F");
					return listStrings;
			}
			return listStrings;
		}

		///<summary>For a given fieldName, returns a picklist of our suggested default Db options. This must be paired exactly with GetRadioVisDefault.</summary>
		public static List<string> GetRadioDbDefault(string fieldName) {
			List<string> listStrings=new List<string>();
			if(fieldName.StartsWith("allergy:")
				|| fieldName.StartsWith("problem:"))
			{
				listStrings.Add("Y");
				listStrings.Add("N");
				return listStrings;
			}
			switch(fieldName) {
				default:
					return listStrings;
				case "Gender":
					listStrings.Add("Male");
					listStrings.Add("Female");
					//All additional options are controversial, so not present by default.
					break;
				case "ins1Relat":
				case "ins2Relat":
					listStrings.Add("Self");
					listStrings.Add("Spouse");
					listStrings.Add("Child");
					listStrings.Add("Dependent");
					//The other 5 are just clutter.
					break;
				case "Position":
					listStrings.Add("Single");
					listStrings.Add("Married");
					listStrings.Add("Child");
					listStrings.Add("Widowed");
					listStrings.Add("Divorced");
					//This is all of them.
					break;
				case "PreferContactMethod":
				case "PreferConfirmMethod":
				case "PreferRecallMethod":
					listStrings.Add(ContactMethod.None.ToString());
					listStrings.Add(ContactMethod.DoNotCall.ToString());
					listStrings.Add(ContactMethod.HmPhone.ToString());
					listStrings.Add(ContactMethod.WkPhone.ToString());
					listStrings.Add(ContactMethod.WirelessPh.ToString());
					listStrings.Add(ContactMethod.Email.ToString());
					listStrings.Add(ContactMethod.SeeNotes.ToString());
					listStrings.Add(ContactMethod.Mail.ToString());
					listStrings.Add(ContactMethod.TextMessage.ToString());
					//Include all 9 by default, but make them human readable in the vis.
					break;
				case "StudentStatus":
					listStrings.Add("N");
					listStrings.Add("P");
					listStrings.Add("F");
					return listStrings;
			}
			return listStrings;
		}

		///<summary>For a given fieldName, returns a picklist to show to patient.</summary>
		public static List<string> GetRadioVisDefault(string fieldName) {
			List<string> listStrings=new List<string>();
			if(fieldName.StartsWith("allergy:")
				|| fieldName.StartsWith("problem:"))
			{
				listStrings.Add("Y");
				listStrings.Add("N");
				return listStrings;
			}
			switch(fieldName) {
				default:
					return listStrings;
				case "Gender":
					listStrings.Add("Male");
					listStrings.Add("Female");
					//All additional options are controversial, so not present by default.
					break;
				case "ins1Relat":
				case "ins2Relat":
					listStrings.Add("Self");
					listStrings.Add("Spouse");
					listStrings.Add("Child");
					listStrings.Add("Dependent");
					//The other 5 are just clutter.
					break;
				case "Position":
					listStrings.Add("Single");
					listStrings.Add("Married");
					listStrings.Add("Child");
					listStrings.Add("Widowed");
					listStrings.Add("Divorced");
					//This is all of them.
					break;
				case "PreferContactMethod":
				case "PreferConfirmMethod":
				case "PreferRecallMethod":
					listStrings.Add("None");
					listStrings.Add("Do Not Call");
					listStrings.Add("Home Phone");
					listStrings.Add("Work Phone");
					listStrings.Add("Wireless");
					listStrings.Add("EMail");
					listStrings.Add("See Notes");
					listStrings.Add("Mail");
					listStrings.Add("Text");
					//Include all 9 by default, but make them human readable.
					break;
				case "StudentStatus":
					listStrings.Add("Non-student");
					listStrings.Add("Part-time");
					listStrings.Add("Full-time");
					return listStrings;
			}
			return listStrings;
		}
	}
}
