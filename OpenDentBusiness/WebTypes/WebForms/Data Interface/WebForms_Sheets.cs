using CodeBase;
using DataConnectionBase;
using OpenDentBusiness.WebTypes.WebForms.Crud;
using OpenDentBusiness.WebTypes.WebForms.Data_Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using WebServiceSerializer;

namespace OpenDentBusiness.WebTypes.WebForms {
	public class WebForms_Sheets {

		///<summary>Makes a synchronous web call to the WebHostSynch web service at HQ.
		///Returns a list of SheetIDs (PKs) of filled out web forms that are pending for this office or the regKey passed in. Throws exceptions.</summary>
		public static List<long> GetSheetIDs(string regKey=null,List<long> listClinicNums=null) {
			Meth.NoCheckMiddleTierRole();
			if(string.IsNullOrEmpty(regKey)) {
				regKey=PrefC.GetString(PrefName.RegistrationKey);
			}
			if(listClinicNums==null) {
				listClinicNums=new List<long>();
			}
			List<PayloadItem> listPayloadItems=new List<PayloadItem>() {
				new PayloadItem(regKey,"RegKey"),
				new PayloadItem(listClinicNums,"ListClinicNums"),
			};
			string payload=PayloadHelper.CreatePayloadWebHostSynch(regKey,listPayloadItems.ToArray());
			//Gets all pending SheetIDs for the registration key and clinics.
			string response=SheetsSynchProxy.GetWebServiceInstance().GetWebFormSheetIDs(payload);
			return WebSerializer.DeserializeTag<List<long>>(response,"Success");
		}

		/// <summary>Returns a list of WebForms_Sheets. Surround with try/catch.</summary>
		public static List<WebForms_Sheet> GetSheets(string regKey=null,List<long> listSheetIDs=null,List<long> listClinicNums=null) {
			if(string.IsNullOrEmpty(regKey)) {
				regKey=PrefC.GetString(PrefName.RegistrationKey);
			}
			if(listSheetIDs==null) {
				listSheetIDs=new List<long>();
			}
			if(listClinicNums==null) {
				listClinicNums=new List<long>();
			}
			List<PayloadItem> listPayloadItems=new List<PayloadItem>() {
				new PayloadItem(regKey,"RegKey"),
				new PayloadItem(listSheetIDs,"ListSheetIDs"),
				new PayloadItem(listClinicNums,"ListClinicNums"),
			};
			string payload=PayloadHelper.CreatePayloadWebHostSynch(regKey,listPayloadItems.ToArray());
			//Get pending sheets from HQ.
			string resultXml=SheetsSynchProxy.GetWebServiceInstance().GetWebFormSheets(payload);
			List<WebForms_Sheet> listWebForms_Sheets = WebSerializer.DeserializeTag<List<WebForms_Sheet>>(resultXml,"Success");
			for(int i=0;i<listWebForms_Sheets.Count;i++) {
				EServiceLog eServiceLog=EServiceLogs.MakeLogEntryWebForms(eServiceAction.WFDownloadedForm,0,listWebForms_Sheets[i].ClinicNum,listWebForms_Sheets[i].SheetID);
				listWebForms_Sheets[i].EServiceLogGuid=eServiceLog.LogGuid;
			}
			return listWebForms_Sheets;
		}

		///<summary>Sends a payload to HQ requesting that they delete the following web forms from their database. Returns true if HQ successfully deleted the web forms. Otherwise; false.</summary>
		public static bool DeleteSheetData(List<WebForms_Sheet> listWebForms_Sheets,string regKey=null) {
			if(listWebForms_Sheets.IsNullOrEmpty()) {
				return true;
			}
			if(string.IsNullOrEmpty(regKey)) {
				regKey=PrefC.GetString(PrefName.RegistrationKey);
			}
			try {
				List<PayloadItem> listPayloadItems=new List<PayloadItem> {
					new PayloadItem(regKey,"RegKey"),
					new PayloadItem(listWebForms_Sheets.Select(x=>x.SheetID).ToList(),"SheetNumsForDeletion")
				};
				string payload=PayloadHelper.CreatePayloadWebHostSynch(regKey,listPayloadItems.ToArray());
				string result=SheetsSynchProxy.GetWebServiceInstance().DeleteSheetData(payload);
				PayloadHelper.CheckForError(result);
				for(int i=0;i<listWebForms_Sheets.Count;i++){
					EServiceLogs.MakeLogEntryWebForms(eServiceAction.WFDeletedForm,FKey:listWebForms_Sheets[i].SheetID,logGuid:listWebForms_Sheets[i].EServiceLogGuid);
				}
				return true;
			}
			catch(Exception ex) {
				string log=Lans.g("FormWebForms","There was a problem telling HQ that the web forms were retrieved:")+$" '{ex.Message}'";
				log+="\r\n"+"  ^"+Lans.g("FormWebForms","The following web forms will be downloaded again the next time forms are retrieved.");
				log+="\r\n"+"  ^"+Lans.g("FormWebForms","SheetIDs:")+" "+string.Join(", ",listWebForms_Sheets.Select(x=>x.SheetID));
				EServiceLogs.MakeLogEntryWebForms(eServiceAction.WFError,note:log);
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
				case "nl-NL":
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

		public static SerializableDictionary<long,int> GetSheetsCount(List<long> listClinicNums,string regKey=null) {
			if(string.IsNullOrEmpty(regKey)) {
				regKey=PrefC.GetString(PrefName.RegistrationKey);
			}
			List<PayloadItem> listPayloadItems=new List<PayloadItem>() {
				new PayloadItem(regKey,"RegKey"),
				new PayloadItem(listClinicNums,"ListClinicNums"),
			};
			string payload=PayloadHelper.CreatePayloadWebHostSynch(regKey,listPayloadItems.ToArray());
			//Get all pending sheets for the office, will not limit to 20 anymore.
			return WebSerializer.DeserializeTag<SerializableDictionary<long,int>>(SheetsSynchProxy.GetWebServiceInstance().GetSheetsCount(payload),"Success");
		}

		#region WebHostSynch

		///<summary></summary>
		public static void DeleteMany(List<long> listSheetIds) {
			if(listSheetIds==null || listSheetIds.Count==0) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSheetIds);
				return;
			}
			string command="DELETE FROM webforms_sheet "
				+"WHERE SheetID IN ("+string.Join(",",listSheetIds.Select(x => POut.Long(x)))+")";
			DataAction.Run(() => DataCore.NonQ(command),ConnectionNames.WebForms);
		}

		public static List<long> DeleteSheetData(long dentalOfficeID,long registrationKeyNum,string officeData) {
			List<long> listSheetNums=WebSerializer.DeserializeTag<List<long>>(officeData,"SheetNumsForDeletion");
			if(listSheetNums.IsNullOrEmpty()) {
				return listSheetNums;
			}
			string strSheetIDs=$"SheetIDs: {string.Join(", ",listSheetNums)}";
			//Make a log entry that the dental office requested these sheets be deleted so that we have an idea on how long it takes to delete this information from our servers.
			WebForms_Logs.TryInsertLog(dentalOfficeID,registrationKeyNum,$"Office requested deleting sheets with the following {strSheetIDs}");
			//Delete the sheet fields associated with the sheets first.
			try {
				WebForms_SheetFields.DeleteForSheetIDs(listSheetNums);
			}
			catch(Exception) {
				WebForms_Logs.TryInsertLog(dentalOfficeID,registrationKeyNum,$"ERROR - Failed deleting sheet fields for {strSheetIDs}",logLevel:LogLevel.Error);
				throw;
			}
			try {
				DeleteMany(listSheetNums);
			}
			catch(Exception) {
				WebForms_Logs.TryInsertLog(dentalOfficeID,registrationKeyNum,$"ERROR - Failed deleting sheets for {strSheetIDs}",logLevel:LogLevel.Error);
				throw;
			}
			WebForms_Logs.TryInsertLog(dentalOfficeID,registrationKeyNum,$"Success deleting sheets for {strSheetIDs}");
			return listSheetNums;
		}

		///<summary>Returns a list of unique patient first names found in the sheet fields associated to the sheets passed in.
		///Patient first names are pulled from the  FieldValue of the first WebForms_SheetField that has a FieldName of FName or FirstName for each sheet.</summary>
		public static List<string> GetPatientFirstNames(List<WebForms_Sheet> listWebForms_Sheets) {
			List<string> listPatientFirstNames=new List<string>();
			List<string> listFieldNameMatches=new List<string>() { "fname","firstname" };
			for(int i=0;i<listWebForms_Sheets.Count;i++) {
				WebForms_SheetField webForms_SheetFieldFName=listWebForms_Sheets[i].SheetFields.First(x => listFieldNameMatches.Contains(x.FieldName.ToLower()));
				listPatientFirstNames.Add(webForms_SheetFieldFName.FieldValue);
			}
			return listPatientFirstNames.Distinct().ToList();
		}

		///<summary>Returns a list of all SheetIDs that are pending to be downloaded for the dental office.</summary>
		public static List<long> GetSheetIDs(long dentalOfficeID,long registrationKeyNum,List<long> listClinicNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),dentalOfficeID,registrationKeyNum,listClinicNums);
			}
			string command=$@"SELECT SheetID FROM webforms_sheet
				WHERE DentalOfficeID = {POut.Long(dentalOfficeID)}
				AND RegistrationKeyNum IN (0,{POut.Long(registrationKeyNum)}) ";//Always include legacy SheetIDs which have a 0 RegistrationKeyNum.
			if(listClinicNums.Count > 0) {
				command+="AND ClinicNum IN("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+") ";
			}
			//Add the slowest filter at the end which is making sure that this sheet is ready for downloading (has SheetFields present).
			command+="AND SheetID IN (SELECT DISTINCT SheetID FROM webforms_sheetfield) ";
			return DataAction.GetT(() => {
				DataTable table=DataCore.GetTable(command);
				return table.Select().Select(x => PIn.Long(x["SheetID"].ToString())).ToList();
			},ConnectionNames.WebForms);
		}

		public static SerializableDictionary<long,int> GetSheetsCount(long dentalOfficeID,long registrationKeyNum,string officeData) {
			List<long> listClinicNums=new List<long>();//Backward compatibility. This web method previously fetched for all webforms.
			return GetSheetCountsForDentalOfficeId(registrationKeyNum,dentalOfficeID,listClinicNums);
		}

		///<summary>Gets dictionary for the OpenDental Service of ClinicNums with their count of available webforms to download.</summary>
		public static SerializableDictionary<long,int> GetSheetCountsForDentalOfficeId(long registrationKeyNum,long dentalOfficeID,List<long> listClinicNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetSerializableDictionary<long,int>(MethodBase.GetCurrentMethod(),registrationKeyNum,dentalOfficeID,listClinicNums);
			}
			string command=$@"SELECT  ClinicNum, COUNT(*) as SheetCount FROM webforms_sheet
				WHERE DentalOfficeID = {POut.Long(dentalOfficeID)}
				AND RegistrationKeyNum IN (0,{POut.Long(registrationKeyNum)}) ";//Always include legacy SheetIDs which have a 0 RegistrationKeyNum.
			if(listClinicNums.Count>0) {
				command+="AND ClinicNum IN("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+") ";
			}
			//Add the slowest filter at the end which is making sure that this sheet is ready for downloading (has SheetFields present).
			command+="AND SheetID IN (SELECT DISTINCT SheetID FROM webforms_sheetfield) ";
			command+="GROUP BY ClinicNum ";
			return DataAction.GetT<SerializableDictionary<long,int>>( () => {
				return DataCore.GetTable(command)
					.AsEnumerable()
					.ToSerializableDictionary(
						x => PIn.Long(x["ClinicNum"].ToString()),// Key is ClinicNum 
						x => PIn.Int(x["SheetCount"].ToString())// Value is count of available sheets
					);
			},ConnectionNames.WebForms); 
		}

		///<summary></summary>
		public static List<WebForms_Sheet> GetSheetsForDentalOfficeId(long dentalOfficeID,long registrationKeyNum,List<long> listSheetIDs,List<long> listClinicNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<WebForms_Sheet>>(MethodBase.GetCurrentMethod(),dentalOfficeID,registrationKeyNum,listSheetIDs,listClinicNums);
			}
			string command=$@"SELECT * FROM webforms_sheet
				WHERE DentalOfficeID = {POut.Long(dentalOfficeID)}
				AND RegistrationKeyNum IN (0,{POut.Long(registrationKeyNum)}) ";//Always include legacy SheetIDs which have a 0 RegistrationKeyNum.
			if(listSheetIDs.Count > 0) {
				command+="AND SheetID IN("+string.Join(",",listSheetIDs.Select(x => POut.Long(x)))+") ";
			}
			if(listClinicNums.Count > 0) {
				command+="AND ClinicNum IN("+string.Join(",",listClinicNums.Select(x => POut.Long(x)))+") ";
			}
			//Add the slowest filter at the end which is making sure that this sheet is ready for downloading (has SheetFields present).
			command+="AND SheetID IN (SELECT DISTINCT SheetID FROM webforms_sheetfield) ";
			return DataAction.GetT(() => {
				//Get the WebForms_Sheet objects from the database.
				List<WebForms_Sheet> listWebForms_Sheets=WebForms_SheetCrud.SelectMany(command);
				//Get the WebForms_SheetField objects from the database and associate them to their corresponding WebForms_Sheet parent.
				List<WebForms_SheetField> listWebFormSheetFields=WebForms_SheetFields.GetSheetFieldsForSheetIds(listWebForms_Sheets.Select(x => x.SheetID).ToList());
				for(int i=0;i<listWebForms_Sheets.Count;i++) {
					listWebForms_Sheets[i].SheetFields=listWebFormSheetFields.FindAll(x => x.SheetID==listWebForms_Sheets[i].SheetID);
					listWebForms_Sheets[i].DateTimeSheet=listWebForms_Sheets[i].DateTimeSheet.ToLocalTime();//Client will be able to convert to its local time.
				}
				return listWebForms_Sheets;
			},ConnectionNames.WebForms);
		}

		public static List<long> GetWebFormSheetIDs(long dentalOfficeID,long registrationKeyNum,string officeData) {
			List<long> listClinicNums=WebSerializer.DeserializeTag<List<long>>(officeData,"ListClinicNums");
			string logMessage="Office requested all pending SheetIDs";
			if(listClinicNums.Count > 0) {
				logMessage+=$" for ClinicNums: {string.Join(", ",listClinicNums)}";
			}
			WebForms_Logs.TryInsertLog(dentalOfficeID,registrationKeyNum,logMessage);
			//Get all pending SheetIDs for the dental office passed in.
			List<long> listWebFormSheetIDs=new List<long>();
			try {
				listWebFormSheetIDs=GetSheetIDs(dentalOfficeID,registrationKeyNum,listClinicNums);
			}
			catch(Exception) {
				WebForms_Logs.TryInsertLog(dentalOfficeID,registrationKeyNum,$"ERROR - Failed selecting pending sheet IDs.",logLevel:LogLevel.Error);
				throw;
			}
			if(listWebFormSheetIDs.Count > 0) {
				logMessage=$"Office told about {listWebFormSheetIDs.Count} pending SheetIDs: {string.Join(", ",listWebFormSheetIDs)}";
			}
			else {
				logMessage=$"Office has no pending sheets.";
			}
			WebForms_Logs.TryInsertLog(dentalOfficeID,registrationKeyNum,logMessage);
			return listWebFormSheetIDs;
		}

		public static List<WebForms_Sheet> GetWebFormSheets(long dentalOfficeID,long registrationKeyNum,string officeData) {
			List<long> listClinicNums=new List<long>();//Backward compatibility. This web method previously fetched for all webforms.
			ODException.SwallowAnyException(() => listClinicNums=WebSerializer.DeserializeTag<List<long>>(officeData,"ListClinicNums"));
			List<long> listSheetIDs=new List<long>();//Backward compatibility. This web method previously fetched for all webforms.
			ODException.SwallowAnyException(() => listSheetIDs=WebSerializer.DeserializeTag<List<long>>(officeData,"ListSheetIDs"));
			string logMessage="Office requesting all pending sheets";
			if(listClinicNums.Count > 0 || listSheetIDs.Count > 0) {
				logMessage+=" that match the following criteria:";
				if(listClinicNums.Count > 0) {
					logMessage+=$" ClinicNums of {string.Join(", ",listClinicNums)} ";
				}
				if(listSheetIDs.Count > 0) {
					logMessage+=$" SheetIDs of {string.Join(", ",listSheetIDs)} ";
				}
			}
			WebForms_Logs.TryInsertLog(dentalOfficeID,registrationKeyNum,logMessage);
			List<WebForms_Sheet> listWebFormSheets=new List<WebForms_Sheet>();
			try {
				listWebFormSheets=GetSheetsForDentalOfficeId(dentalOfficeID,registrationKeyNum,listSheetIDs,listClinicNums);
			}
			catch(Exception) {
				WebForms_Logs.TryInsertLog(dentalOfficeID,registrationKeyNum,"ERROR - Failed selecting pending sheets",logLevel:LogLevel.Error);
				throw;
			}
			if(listWebFormSheets.IsNullOrEmpty()) {
				logMessage="No pending sheets were found nor returned.";
			}
			else {
				List<string> listPatientFirstNames=GetPatientFirstNames(listWebFormSheets);
				logMessage=$"Returning {listWebFormSheets.Count} pending sheets for the following patients: {string.Join(", ",listPatientFirstNames.Select(x => $"'{x}'"))}";
			}
			WebForms_Logs.TryInsertLog(dentalOfficeID,registrationKeyNum,logMessage);
			return listWebFormSheets;
		}

		#endregion

	}


}
