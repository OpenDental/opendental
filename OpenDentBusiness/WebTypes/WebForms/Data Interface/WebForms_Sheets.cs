using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeBase;
using WebServiceSerializer;

namespace OpenDentBusiness.WebTypes.WebForms {
	public class WebForms_Sheets {
		
		/// <summary></summary>
		/// <param name="regKey"></param>
		/// <returns></returns>
		public static bool TryGetSheets(out List<WebForms_Sheet> listWebFormsSheets,string regKey=null) {
			listWebFormsSheets=new List<WebForms_Sheet>();
			if(string.IsNullOrEmpty(regKey)) {
				regKey=PrefC.GetString(PrefName.RegistrationKey);
			}
			try {
				string payload=PayloadHelper.CreatePayloadWebHostSynch(regKey,new PayloadItem(regKey,"RegKey"));
				//Get all pending sheets for the office, will not limit to 20 anymore.
				listWebFormsSheets=WebSerializer.DeserializeTag<List<WebForms_Sheet>>(SheetsSynchProxy.GetWebServiceInstance().GetWebFormSheets(payload),"Success");
			}
			catch (Exception ex) {
				ex.DoNothing();
				return false;
			}
			return true;
		}

		/// <summary>Returns true if able to successfully delete the sheets. Returns false otherwise.</summary>
		/// <param name="regKey"></param>
		/// <param name="listSheetNums"></param>
		public static bool DeleteSheetData(List<long> listSheetNums,string regKey=null) {
			if(string.IsNullOrEmpty(regKey)) {
				regKey=PrefC.GetString(PrefName.RegistrationKey);
			}
			try {
				List<PayloadItem> listPayloadItems=new List<PayloadItem> {
					new PayloadItem(regKey,"RegKey"),
					new PayloadItem(listSheetNums,"SheetNumsForDeletion")
				};
				string payload=PayloadHelper.CreatePayloadWebHostSynch(regKey,listPayloadItems.ToArray());
				string result=SheetsSynchProxy.GetWebServiceInstance().DeleteSheetData(payload);
				PayloadHelper.CheckForError(result);
				return true;
			}
			catch (Exception ex) {
				ex.DoNothing();
				return false;
			}
		}

		///<summary>Takes in a webform_sheet and the culture preference. Trys to extract these fields off of the Sheet fields.</summary>
		public static void ParseWebFormSheet(WebForms_Sheet sheet,string webFormPrefCulture,out string lName,out string fName,out DateTime birthdate,
			out List<string> listPhoneNumbers,out string email) 
		{
			lName="";
			fName="";
			birthdate=new DateTime();
			listPhoneNumbers=new List<string>();
			email="";
			foreach(WebForms_SheetField field in sheet.SheetFields) {//Loop through each field.
				switch(field.FieldName.ToLower()) {
					case "lname":
					case "lastname":
						lName=field.FieldValue;
						break;
					case "fname":
					case "firstname":
						fName=field.FieldValue;
						break;
					case "bdate":
					case "birthdate":
						birthdate=ParseDateWebForms(field.FieldValue,webFormPrefCulture);
						break;
					case "hmphone":
					case "wkphone":
					case "wirelessphone":
						if(field.FieldValue!="") {
							listPhoneNumbers.Add(field.FieldValue);
						}
						break;
					case "email":
						email=field.FieldValue;
						break;
				}
			}
		}

		///<summary>Parses the given date with the given culture.  If the culture is not supplied, makes a web call to fetch the office's CultureName 
		///preference, defaulting to the LanguageAndRegion preference if not found.  Supports the following delimiters in the date string even if they 
		///do not match the format determined by webFormPrefCulture: '/','.','-','\'</summary>
		public static DateTime ParseDateWebForms(string date,string webFormPrefCulture=null) {
			string dateTimeFormat="M/d/yyyy";//Default to en-US format just in case we don't currently support the culture passed in.
			if(webFormPrefCulture.IsNullOrEmpty()) {
				if(WebForms_Preferences.TryGetPreference(out WebForms_Preference webFormPref)) {
					webFormPrefCulture=webFormPref.CultureName;
				}
				else {
					webFormPrefCulture=PrefC.GetString(PrefName.LanguageAndRegion);
				}
			}
			string delimiterSupported="/";
			switch(webFormPrefCulture) {
				case "ar-JO":
				case "en-CA":
				case "en-GB":
				case "en-ES":
				case "en-MX":
				case "en-PR":
					dateTimeFormat="dd/MM/yyyy";
					break;
				case "da-DK":
				case "en-IN":
					dateTimeFormat="dd-MM-yyyy";
					delimiterSupported="-";
					break;
				case "en-AU":
				case "en-NZ":
					dateTimeFormat="d/MM/yyyy";
					break;
				case "mn-MN":
					dateTimeFormat="yy.MM.dd";
					delimiterSupported=".";
					break;
				case "zh-CN":
					dateTimeFormat="yyyy/M/d";
					break;
			}
			DateTime retVal;
			//Ensure any characters in between digits are the correct delimiter.
			string dateScrubbed=string.Join(delimiterSupported,Regex.Split(date,"[^\\d]+").Where(x => !string.IsNullOrWhiteSpace(x)));
			if(!DateTime.TryParseExact(dateScrubbed,dateTimeFormat,new CultureInfo(webFormPrefCulture),DateTimeStyles.None,out retVal)) {
				retVal=DateTime.MinValue;
			}
			return retVal;
		}

		///<summary>Returns a list of the primary keys from the sheets passed in with the matching last name, first name, birthdate, email, 
		///and phone numbers attached to the sheetToMatch.</summary>
		public static List<long> FindSheetsForPat(WebForms_Sheet sheetToMatch,List<WebForms_Sheet> listSheets,string webFormPrefCulture) {
			string lName;
			string fName;
			DateTime birthdate;
			List<string> listPhoneNumbers;
			string email;
			ParseWebFormSheet(sheetToMatch,webFormPrefCulture,out lName,out fName,out birthdate,out listPhoneNumbers,out email);
			List<long> listSheetIdMatch=new List<long>();
			foreach(WebForms_Sheet sheet in listSheets) {
				string lNameSheet="";
				string fNameSheet="";
				DateTime birthdateSheet=new DateTime();
				List<string> listPhoneNumbersSheet=new List<string>();
				string emailSheet="";
				ParseWebFormSheet(sheet,webFormPrefCulture,out lNameSheet,out fNameSheet,out birthdateSheet,out listPhoneNumbersSheet,out emailSheet);
				if(lName==lNameSheet && fName==fNameSheet && birthdate==birthdateSheet && email==emailSheet //All phone numbers must match in both.
					&& listPhoneNumbers.Except(listPhoneNumbersSheet).Count()==0 && listPhoneNumbersSheet.Except(listPhoneNumbers).Count()==0)
				{
					listSheetIdMatch.Add(sheet.SheetID);
				}
			}
			return listSheetIdMatch;
		}
	}
}
