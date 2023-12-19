using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CodeBase;
using DataConnectionBase;
using OpenDentBusiness.WebTypes.WebForms;
using PdfSharp.Pdf;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Sheets{
		///<Summary>Gets one Sheet from the database.</Summary>
		public static Sheet GetOne(long sheetNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Sheet>(MethodBase.GetCurrentMethod(),sheetNum);
			}
			return Crud.SheetCrud.SelectOne(sheetNum);
		}

		///<summary>Gets a single sheet from the database.  Then, gets all the fields and parameters for it.  So it returns a fully functional sheet.
		///Returns null if the sheet isn't found in the database.</summary>
		public static Sheet GetSheet(long sheetNum) {
			//No need to check MiddleTierRole; no call to db.
			Sheet sheet=GetOne(sheetNum);
			if(sheet==null) {
				return null;//Sheet was deleted.
			}
			SheetFields.GetFieldsAndParameters(sheet);
			return sheet;
		}

		///<summary>Gets a list of Sheets from the database. The sheets returned will not have SheetFields.</summary>
		public static List<Sheet> GetSheets(List<long> listSheetNums) {
			if(listSheetNums.IsNullOrEmpty()) {
				return new List<Sheet>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Sheet>>(MethodBase.GetCurrentMethod(),listSheetNums);
			}
			string command="SELECT * FROM sheet WHERE SheetNum IN ("+string.Join(",",listSheetNums.Select(x => POut.Long(x)))+")";
			return Crud.SheetCrud.SelectMany(command);
		}

		///<Summary>This is normally done in FormSheetFillEdit, but if we bypass that window for some reason, we can also save a new sheet here. Signature
		///fields are inserted as they are, so they must be keyed to the field values already. Saves the sheet and sheetfields exactly as they are. Used by
		///webforms, for example, when a sheet is retrieved from the web server and the sheet signatures have already been keyed to the field values and
		///need to be inserted as-is into the user's db. Return the SheetNum in case we need to use it locally when using middle tier.</Summary>
		public static long SaveNewSheet(Sheet sheet) {
			//This remoting role check is technically unnecessary but it significantly speeds up the retrieval process for Middle Tier users due to looping.
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				sheet.SheetNum=Meth.GetLong(MethodBase.GetCurrentMethod(),sheet);
				return sheet.SheetNum;
			}
			if(!sheet.IsNew) {
				throw new ApplicationException("Only new sheets allowed");
			}
			Insert(sheet);
			//insert 'blank' sheetfields to get sheetfieldnums assigned, then use ordered sheetfieldnums with actual field data to update 'blank' db fields
			List<SheetField> listSheetFieldsBlank=sheet.SheetFields.Select(x => new SheetField() { SheetNum=sheet.SheetNum }).ToList();
			SheetFields.InsertMany(listSheetFieldsBlank);
			List<SheetField> listSheetFieldsDb=SheetFields.GetListForSheet(sheet.SheetNum);
			//The count can be off for offices that read and write to separate servers when we get objects that were just inserted so we try twice.
			if(listSheetFieldsDb.Count!=sheet.SheetFields.Count) {
				System.Threading.Thread.Sleep(100);
				listSheetFieldsDb=SheetFields.GetListForSheet(sheet.SheetNum);
			}
			if(listSheetFieldsDb.Count!=sheet.SheetFields.Count) {
				Delete(sheet.SheetNum);//any blank inserted sheetfields will be linked to the sheet marked deleted
				throw new ApplicationException("Incorrect sheetfield count.");
			}
			List<long> listSheetFieldNums=listSheetFieldsDb.Select(x => x.SheetFieldNum).OrderBy(x => x).ToList();
			//now that we have an ordered list of sheetfieldnums, update db blank fields with all field data from field in memory
			for(int i=0;i<sheet.SheetFields.Count;i++) {
				SheetField sheetField=sheet.SheetFields[i];
				sheetField.SheetFieldNum=listSheetFieldNums[i];
				sheetField.SheetNum=sheet.SheetNum;
				SheetFields.Update(sheetField);
			}
			return sheet.SheetNum;
		}

		///<summary>Gets sheets with PatNum=0 and IsDeleted=0. Sheets with no PatNums were most likely transferred from CEMT tool.
		///Also sets the sheet's SheetFields.</summary>
		public static List<Sheet> GetTransferSheets() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Sheet>>(MethodBase.GetCurrentMethod());
			}
			//Sheets with patnum=0 and the sheet has a sheetfield. 
			string command="SELECT * FROM sheet "
				+"INNER JOIN sheetfield ON sheetfield.SheetNum=sheet.SheetNum "
				+"WHERE PatNum=0 AND IsDeleted=0 "
				+"AND sheetfield.FieldName='isTransfer' "
				+$"AND SheetType={POut.Int((int)SheetTypeEnum.PatientForm)}";
			List<Sheet> listSheets=Crud.SheetCrud.SelectMany(command);
			//Get the Sheetfields and parameters for each of the CEMT sheets
			for(int i=0;i<listSheets.Count;i++) {
				SheetFields.GetFieldsAndParameters(listSheets[i]);
			}
			return listSheets;
		}

		///<Summary>Saves a list of sheets to the Database. Only saves new sheets, ignores sheets that are not new.</Summary>
		public static void SaveNewSheetList(List<Sheet> listSheets) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSheets);
				return;
			}
			for(int i=0;i<listSheets.Count;i++) {
				if(!listSheets[i].IsNew) {
					continue;
				}
				Crud.SheetCrud.Insert(listSheets[i]);
				List<SheetField> listSheetFields=listSheets[i].SheetFields;
				for(int j=0;j<listSheetFields.Count;j++) {
					listSheetFields[j].SheetNum=listSheets[i].SheetNum;
					Crud.SheetFieldCrud.Insert(listSheetFields[j]);
				}
			}
		}

		///<summary>Used in FormRefAttachEdit to show all referral slips for the patient/referral combo.  Usually 0 or 1 results.</summary>
		public static List<Sheet> GetReferralSlips(long patNum,long referralNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Sheet>>(MethodBase.GetCurrentMethod(),patNum,referralNum);
			}
			string command="SELECT * FROM sheet WHERE PatNum="+POut.Long(patNum)
				+" AND sheet.SheetType="+POut.Int((int)SheetTypeEnum.ReferralSlip)
				+" AND EXISTS(SELECT * FROM sheetfield "
				+"WHERE sheet.SheetNum=sheetfield.SheetNum "
				+"AND sheetfield.FieldType="+POut.Long((int)SheetFieldType.Parameter)
				+" AND sheetfield.FieldName='ReferralNum' "
				+"AND sheetfield.FieldValue='"+POut.Long(referralNum)+"') "
				+"AND IsDeleted=0 "
				+"ORDER BY DateTimeSheet";
			return Crud.SheetCrud.SelectMany(command);
		}

		///<summary>Used in FormLabCaseEdit to view an existing lab slip.  Will return null if none exist.</summary>
		public static Sheet GetLabSlip(long patNum,long labCaseNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Sheet>(MethodBase.GetCurrentMethod(),patNum,labCaseNum);
			}
			string command="SELECT sheet.* FROM sheet,sheetfield "
				+"WHERE sheet.SheetNum=sheetfield.SheetNum"
				+" AND sheet.PatNum="+POut.Long(patNum)
				+" AND sheet.SheetType="+POut.Long((int)SheetTypeEnum.LabSlip)
				+" AND sheetfield.FieldType="+POut.Long((int)SheetFieldType.Parameter)
				+" AND sheetfield.FieldName='LabCaseNum' "
				+"AND sheetfield.FieldValue='"+POut.Long(labCaseNum)+"' "
				+"AND IsDeleted=0";
			return Crud.SheetCrud.SelectOne(command);
		}

		///<summary>Used in FormRxEdit to view an existing rx.  Will return null if none exist.</summary>
		public static Sheet GetRx(long patNum,long rxNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Sheet>(MethodBase.GetCurrentMethod(),patNum,rxNum);
			}
			string command="SELECT sheet.* FROM sheet,sheetfield "
				+"WHERE sheet.PatNum="+POut.Long(patNum)
				+" AND sheet.SheetType="+POut.Long((int)SheetTypeEnum.Rx)
				+" AND sheetfield.FieldType="+POut.Long((int)SheetFieldType.Parameter)
				+" AND sheetfield.FieldName='RxNum' "
				+"AND sheetfield.FieldValue='"+POut.Long(rxNum)+"' "
				+"AND IsDeleted=0";
			return Crud.SheetCrud.SelectOne(command);
		}

		///<summary>Gets all sheets for a patient that have the terminal flag set.  Shallow list, no fields or parameters.</summary>
		public static List<Sheet> GetForTerminal(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Sheet>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM sheet WHERE PatNum="+POut.Long(patNum)
				+" AND ShowInTerminal > 0 AND IsDeleted=0"
				+" ORDER BY ShowInTerminal,DateTimeSheet";
			return Crud.SheetCrud.SelectMany(command);
		}

		/// <summary>Gets the maximum Terminal Num for the selected patient.  Returns 0 if there's no sheets marked to show in terminal.</summary>
		public static int GetMaxTerminalNum(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT MAX(ShowInTerminal) FROM sheet WHERE PatNum="+POut.Long(patNum)
				+" AND IsDeleted=0";
			return Db.GetInt(command);
		}

		///<summary>Trys to set the out params with sheet fields valus for LName,FName,DOB,PhoneNumbers, and email. Used when importing CEMT patient transfers.</summary>
		public static void ParseTransferSheet(Sheet sheet,out string lName,out string fName,out DateTime dateBirth,out List<string> listPhoneNumbers,
			out string email) 
		{
			lName="";
			fName="";
			dateBirth=new DateTime();
			listPhoneNumbers=new List<string>();
			email="";
			List<SheetField> listSheetFields=sheet.SheetFields;
			for(int i=0;i<listSheetFields.Count;i++) {
				switch(listSheetFields[i].FieldName.ToLower()) {
					case "lname":
					case "lastname":
						lName=listSheetFields[i].FieldValue;
						break;
					case "fname":
					case "firstname":
						fName=listSheetFields[i].FieldValue;
						break;
					case "bdate":
					case "birthdate":
						dateBirth=PIn.Date(listSheetFields[i].FieldValue);
						break;
					case "hmphone":
					case "wkphone":
					case "wirelessphone":
						if(listSheetFields[i].FieldValue!="") {
							listPhoneNumbers.Add(listSheetFields[i].FieldValue);
						}
						break;
					case "email":
						email=listSheetFields[i].FieldValue;
						break;
				}
			}
		}

		///<summary>Returns a list of SheetNums of matching sheets.</summary>
		public static List<long> FindSheetsForPat(Sheet sheetToMatch,List<Sheet> listSheets) {
			string lName;
			string fName;
			DateTime dateBirth;
			List<string> listPhoneNumbers;
			string email;
			ParseTransferSheet(sheetToMatch,out lName,out fName,out dateBirth,out listPhoneNumbers,out email);
			List<long> listSheetNumsIdMatch=new List<long>();
			for(int i=0;i<listSheets.Count;i++) {
				string lNameSheet="";
				string fNameSheet="";
				DateTime dateBirthSheet=new DateTime();
				List<string> listPhoneNumbersSheet=new List<string>();
				string emailSheet="";
				ParseTransferSheet(listSheets[i],out lNameSheet,out fNameSheet,out dateBirthSheet,out listPhoneNumbersSheet,out emailSheet);
				if(lName==lNameSheet && fName==fNameSheet && dateBirth==dateBirthSheet && email==emailSheet 
					//All phone numbers must match in both.
					&& listPhoneNumbers.Except(listPhoneNumbersSheet).Count()==0 && listPhoneNumbersSheet.Except(listPhoneNumbers).Count()==0) 
				{
					listSheetNumsIdMatch.Add(listSheets[i].SheetNum);
				}
			}
			return listSheetNumsIdMatch;
		}

		///<summary>Get all sheets for a patient for today.</summary>
		public static List<Sheet> GetForPatientForToday(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Sheet>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string dateSQL="CURDATE()";
			if(DataConnection.DBtype==DatabaseType.Oracle){
				dateSQL="(SELECT CURRENT_DATE FROM dual)";
			}
			string command="SELECT * FROM sheet WHERE PatNum="+POut.Long(patNum)+" "
				+"AND "+DbHelper.DtimeToDate("DateTimeSheet")+" = "+dateSQL+" "
				+"AND IsDeleted=0";
			return Crud.SheetCrud.SelectMany(command);
		}

		///<summary>Get all sheets for a patient.</summary>
		public static List<Sheet> GetForPatient(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Sheet>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM sheet WHERE IsDeleted=0 AND PatNum="+POut.Long(patNum);
			return Crud.SheetCrud.SelectMany(command);
		}

		///<summary>Get all sheets that reference a given document. Primarily used to prevent deleting an in use document.</summary>
		/// <returns>List of sheets that have fields that reference the given DocNum. Returns empty list if document is not referenced.</returns>
		public static List<Sheet> GetForDocument(long docNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Sheet>>(MethodBase.GetCurrentMethod(),docNum);
			}
			string command="";
			command="SELECT sheet.* FROM sheetfield "
				+"LEFT JOIN sheet ON sheet.SheetNum = sheetfield.SheetNum "
				+"WHERE IsDeleted=0 "
				+"AND FieldType = 10 "//PatImage
				+"AND FieldValue = '"+POut.Long(docNum)+"' "//FieldName == DocCategory, which we do not care about here.
				+"GROUP BY sheet.SheetNum "
				+"UNION "
				+"SELECT sheet.* "
				+"FROM sheet "
				+"WHERE sheet.SheetType="+POut.Int((int)SheetTypeEnum.ReferralLetter)+" "
				+"AND sheet.IsDeleted=0 "
				+"AND sheet.DocNum="+POut.Long(docNum);
			return Crud.SheetCrud.SelectMany(command);
		}

		///<summary>Gets the most recent Exam Sheet based on description to fill a patient letter.</summary>
		public static Sheet GetMostRecentExamSheet(long patNum,string examDescript) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Sheet>(MethodBase.GetCurrentMethod(),patNum,examDescript);
			}
			string command="SELECT * FROM sheet WHERE DateTimeSheet="
				+"(SELECT MAX(DateTimeSheet) FROM sheet WHERE PatNum="+POut.Long(patNum)+" "
				+"AND Description='"+POut.String(examDescript)+"' AND IsDeleted=0) "
				+"AND PatNum="+POut.Long(patNum)+" "
				+"AND Description='"+POut.String(examDescript)+"' "
				+"AND IsDeleted=0 "
				+"LIMIT 1";
			return Crud.SheetCrud.SelectOne(command);
		}

		///<summary>Called by eClipboard check-in once an appointment has been moved to the waiting room and the patient is ready to fill out forms.
		///Returns number of new sheets created and inserted into Sheet table.</summary>
		public static int CreateSheetsForCheckIn(Appointment appointment) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),appointment);
			}
			if(!MobileAppDevices.IsClinicSignedUpForEClipboard(PrefC.HasClinicsEnabled?appointment.ClinicNum:0)) { //this clinic isn't signed up for this feature
				return 0;
			}
			if(!ClinicPrefs.GetBool(PrefName.EClipboardCreateMissingFormsOnCheckIn,appointment.ClinicNum)) { //This feature is turned off
				return 0;
			}
			bool useDefault=ClinicPrefs.GetBool(PrefName.EClipboardUseDefaults,appointment.ClinicNum);
			List<EClipboardSheetDef> listEClipboardSheetDefsToCreate=EClipboardSheetDefs.GetForClinic(useDefault ? 0 : appointment.ClinicNum);
			if(listEClipboardSheetDefsToCreate.Count==0) { //There aren't any sheets to create here
				return 0;
			}
			List<Sheet> listSheetsAlreadyCompleted=Sheets.GetForPatient(appointment.PatNum);
			List<Sheet> listSheetsAlreadyInTerminal=Sheets.GetForTerminal(appointment.PatNum);
			//if we already have sheets queued for the patient don't add duplicates
			if(listSheetsAlreadyInTerminal.Count>0) {
				listSheetsAlreadyCompleted.RemoveAll(x => listSheetsAlreadyInTerminal.Select(y => y.SheetNum).Contains(x.SheetNum));
				listEClipboardSheetDefsToCreate.RemoveAll(x => listSheetsAlreadyInTerminal.Select(y => y.SheetDefNum).Contains(x.SheetDefNum));
			}
			Patient patient=Patients.GetPat(appointment.PatNum);
			//Remove any sheets that the patient shouldn't see based on age. A value of -1 means ignore.
			listEClipboardSheetDefsToCreate.RemoveAll(x => x.MinAge!=-1 && patient.Age<x.MinAge);
			listEClipboardSheetDefsToCreate.RemoveAll(x => x.MaxAge!=-1 && patient.Age>x.MaxAge);
			listEClipboardSheetDefsToCreate=EClipboardSheetDefs.FilterPrefillStatuses(listEClipboardSheetDefsToCreate,listSheetsAlreadyCompleted, clinicNum:appointment.ClinicNum, listSheetsInTerminal:listSheetsAlreadyInTerminal)
				.OrderBy(x => x.ItemOrder).ToList();
			byte showInTerminal=GetBiggestShowInTerminal(appointment.PatNum);
			List<Sheet> listSheetsNew=new List<Sheet>();
			for(int i=0;i<listEClipboardSheetDefsToCreate.Count;i++) {
				//First check if we've already completed this form against our resubmission interval rules
				Sheet sheetLastCompleted=listSheetsAlreadyCompleted
					.Where(x => x.SheetDefNum==listEClipboardSheetDefsToCreate[i].SheetDefNum)
					.OrderBy(x => x.DateTimeSheet)
					.LastOrDefault()??new Sheet();
				if(sheetLastCompleted.DateTimeSheet > DateTime.MinValue) {
					if(listEClipboardSheetDefsToCreate[i].ResubmitInterval.Days==0 && sheetLastCompleted.RevID >= listEClipboardSheetDefsToCreate[i].PrefillStatusOverride) {//Once should always equal 0 but in case check both
						continue; //If this interval is set to 0 and they've already completed this form once, we never want to create it automatically again
					}
					int elapsed=(DateTime.Today - sheetLastCompleted.DateTimeSheet.Date).Days;
					if(elapsed < listEClipboardSheetDefsToCreate[i].ResubmitInterval.Days) {
						continue; //The interval hasn't elapsed yet so we don't want to create this sheet
					}
				}
				SheetDef sheetDef=SheetDefs.GetSheetDef(listEClipboardSheetDefsToCreate[i].SheetDefNum);
				Sheet sheetNew=new Sheet();
				//Look up the most recent sheet filledout by this patients wth this def num
				Sheet sheet=Sheets.GetForPatient(appointment.PatNum).Where(x=>x.SheetDefNum==sheetDef.SheetDefNum).OrderByDescending(x=>x.DateTimeSheet).FirstOrDefault();
				if(listEClipboardSheetDefsToCreate[i].PrefillStatus==PrefillStatuses.PreFill && sheet!=null && sheet.RevID==sheetDef.RevID) {
					//do the pre-fill thing from  the other method here.
					sheetNew=PreFillSheetFromPreviousAndDatabase(sheetDef,sheet);
					sheetNew.IsNew=true; //Setting this to true because we want to insert this new sheet not update an old one
					sheetNew.DateTimeSheet=DateTime.Now;
				}
				else {
					sheetNew=CreateSheetFromSheetDef(sheetDef,appointment.PatNum);
					SheetParameter.SetParameter(sheetNew,"PatNum",appointment.PatNum);//must come before sheet filler
					SheetFiller.FillFields(sheetNew);
				}
				//Counting starts at 1 in this case and we don't want to ovewrite the previous number so increment first
				sheetNew.ShowInTerminal=++showInTerminal;
				listSheetsNew.Add(sheetNew);
				SecurityLogs.MakeLogEntry(EnumPermType.FormAdded,sheetNew.PatNum,$"{sheetNew.Description} Created in EClipboard");
				EServiceLogs.MakeLogEntry(eServiceAction.ECAddedForm,eServiceType.EClipboard,FKeyType.SheetNum,patNum:sheetNew.PatNum,FKey:sheetNew.SheetNum,clinicNum: appointment.ClinicNum);
			}
			SaveNewSheetList(listSheetsNew);
			return listSheetsNew.Count;
		}

		///<summary>Creates a new sheet instance based on sheetDefOriginal, and fills it with values from the db, and then fills remaining with values from sheet. Returns the new, pre-filled sheet.</summary>
		public static Sheet PreFillSheetFromPreviousAndDatabase(SheetDef sheetDefOriginal,Sheet sheet) {
			//No need to check MiddleTierRole; no call to db
			Sheet sheetNew=SheetUtil.CreateSheet(sheetDefOriginal);
			sheetNew.DateTimeSheet=DateTime.Now;
			sheetNew.PatNum=sheet.PatNum;
			//Only setting the PatNum sheet parameter was what the Add button was doing from the "Patient Forms and Medical Histories" window.
			SheetParameter.SetParameter(sheetNew,"PatNum",sheet.PatNum);
			//Fill the fields with the most recent values from the non-sheet related tables in database.
			SheetFiller.FillFields(sheetNew);
			if(sheet.SheetFields.IsNullOrEmpty()) {
				SheetFields.GetFieldsAndParameters(sheet);
			}
			//If there are current medications in the DB, display them on the prefilled sheet. If there are not, use the previous sheet.
			bool doUseMedsFromPrevSheet=!MedicationPats.GetPatientData(sheet.PatNum).Any(x=>MedicationPats.IsMedActive(x));
			bool doUseProblemFromPrevSheet=!Diseases.Refresh(sheet.PatNum,false).Any();
			bool doUseAllergyFromPrevSheet=!Allergies.GetAll(sheet.PatNum,true).Any();
			//Get the fields that we want to fill from previous sheet.
			//Always skip insurance fields, skip medications if they have active medications in the DB.
			//Always exclude static text. Allow combo or check boxes if the fieldName is misc
			List<SheetField> listSheetFieldsNewEmpty=sheetNew.SheetFields.FindAll(x => x.FieldType!=(SheetFieldType.StaticText)
				&& (!x.FieldType.In(SheetFieldType.CheckBox) || x.FieldName=="misc"
					|| (x.FieldName.StartsWith("problem") && doUseProblemFromPrevSheet) 
					|| (x.FieldName.StartsWith("allergy") && doUseAllergyFromPrevSheet))
				&& (x.FieldValue.IsNullOrEmpty() || x.FieldType.In(SheetFieldType.ComboBox))
				&& !x.FieldName.StartsWith("ins1")	
				&& !x.FieldName.StartsWith("ins2")	
				&& (!x.FieldName.StartsWith("inputMed")	|| doUseMedsFromPrevSheet)
			);
			//Find the fields that were passed in that can be used with pre-fill logic.
			List<SheetField> listSheetFieldsOrig=sheet.SheetFields.FindAll(x => x.SheetFieldDefNum > 0 && !x.FieldValue.IsNullOrEmpty());
			//Loop through fields on the new sheet, find their matching fields on the passed in sheet by sheetFieldDefNum.
			for(int i=0;i<listSheetFieldsNewEmpty.Count;i++) {
				for(int j=0;j<listSheetFieldsOrig.Count;j++) {
					if(listSheetFieldsNewEmpty[i].SheetFieldDefNum!=listSheetFieldsOrig[j].SheetFieldDefNum) {
						continue;
					}
					listSheetFieldsNewEmpty[i].FieldValue=listSheetFieldsOrig[j].FieldValue;
				}	
			}
			//Clear signiture boxes.
			for(int i=0;i<sheetNew.SheetFields.Count;i++) {
				if(sheetNew.SheetFields[i].FieldType==SheetFieldType.SigBox || sheetNew.SheetFields[i].FieldType==SheetFieldType.SigBoxPractice) {
					sheetNew.SheetFields[i].FieldValue="";
				}
			}
			return sheetNew;
		}

		public static Sheet CreateSheetFromSheetDef(SheetDef sheetDef,long patNum = 0,bool hidePaymentOptions = false) {
			bool FieldIsPaymentOptionHelper(SheetFieldDef sheetFieldDef) {
				if(sheetFieldDef.IsPaymentOption) {
					return true;
				}
				switch(sheetFieldDef.FieldName) {
					case "StatementEnclosed":
					case "StatementAging":
						return true;
				}
				return false;
			}
			List<SheetField> CreateFieldList(List<SheetFieldDef> listSheetFieldDefs,string language) {
				List<SheetField> listSheetFields=new List<SheetField>();
				//SheetDefs that are not setup with the desired language translation SheetFieldDefs should default to the non-translated SheetFieldDefs.
				bool hasTranslationForLanguage=listSheetFieldDefs.Any(x => x.Language==language);
				for(int i=0;i<listSheetFieldDefs.Count;i++) {
					//Only use the SheetFieldDefs for the specified language if available.
					if(hasTranslationForLanguage) {
						if(listSheetFieldDefs[i].Language!=language) {
							continue;
						}
					}
					//Otherwise, only use the SheetFieldDefs for the default language.
					else if(!string.IsNullOrWhiteSpace(listSheetFieldDefs[i].Language)) {
						continue;
					}
					if(hidePaymentOptions && FieldIsPaymentOptionHelper(listSheetFieldDefs[i])) {
						continue;
					}
					SheetField sheetField=new SheetField(); 
					sheetField.IsNew=true;
					sheetField.FieldName=listSheetFieldDefs[i].FieldName;
					sheetField.FieldType=listSheetFieldDefs[i].FieldType;
					sheetField.FieldValue=listSheetFieldDefs[i].FieldValue;
					sheetField.FontIsBold=listSheetFieldDefs[i].FontIsBold;
					sheetField.FontName=listSheetFieldDefs[i].FontName;
					sheetField.FontSize=listSheetFieldDefs[i].FontSize;
					sheetField.GrowthBehavior=listSheetFieldDefs[i].GrowthBehavior;
					sheetField.Height=listSheetFieldDefs[i].Height;
					sheetField.RadioButtonValue=listSheetFieldDefs[i].RadioButtonValue;
					//field.SheetNum=sheetFieldDef.SheetNum;//set later
					sheetField.Width=listSheetFieldDefs[i].Width;
					sheetField.XPos=listSheetFieldDefs[i].XPos;
					sheetField.YPos=listSheetFieldDefs[i].YPos;
					sheetField.RadioButtonGroup=listSheetFieldDefs[i].RadioButtonGroup;
					sheetField.IsRequired=listSheetFieldDefs[i].IsRequired;
					sheetField.TabOrder=listSheetFieldDefs[i].TabOrder;
					sheetField.ReportableName=listSheetFieldDefs[i].ReportableName;
					sheetField.SheetFieldDefNum=listSheetFieldDefs[i].SheetFieldDefNum;
					sheetField.TextAlign=listSheetFieldDefs[i].TextAlign;
					sheetField.ItemColor=listSheetFieldDefs[i].ItemColor;
					sheetField.IsLocked=listSheetFieldDefs[i].IsLocked;
					sheetField.TabOrderMobile=listSheetFieldDefs[i].TabOrderMobile;
					sheetField.UiLabelMobile=listSheetFieldDefs[i].UiLabelMobile;
					sheetField.UiLabelMobileRadioButton=listSheetFieldDefs[i].UiLabelMobileRadioButton;
					sheetField.CanElectronicallySign=listSheetFieldDefs[i].CanElectronicallySign;
					sheetField.IsSigProvRestricted=listSheetFieldDefs[i].IsSigProvRestricted;
					listSheetFields.Add(sheetField);
				}
				return listSheetFields;
			}
			string language=(patNum==0?"":Patients.GetPat(patNum).Language);//Blank string will use 'Default' translation.
			Sheet sheet=new Sheet();
			sheet.IsNew=true;
			sheet.DateTimeSheet=DateTime.Now;
			sheet.FontName=sheetDef.FontName;
			sheet.FontSize=sheetDef.FontSize;
			sheet.Height=sheetDef.Height;
			sheet.SheetType=sheetDef.SheetType;
			sheet.Width=sheetDef.Width;
			sheet.PatNum=patNum;
			sheet.Description=sheetDef.Description;
			sheet.IsLandscape=sheetDef.IsLandscape;
			sheet.IsMultiPage=sheetDef.IsMultiPage;
			sheet.SheetFields=CreateFieldList(sheetDef.SheetFieldDefs,language);//Blank fields with no values. Values filled later from SheetFiller.FillFields()
			sheet.Parameters=sheetDef.Parameters;
			sheet.SheetDefNum=sheetDef.SheetDefNum;
			sheet.HasMobileLayout=sheetDef.HasMobileLayout;
			sheet.RevID=sheetDef.RevID;
			return sheet;
		}

		///<summary></summary>
		public static long Insert(Sheet sheet) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				sheet.SheetNum=Meth.GetLong(MethodBase.GetCurrentMethod(),sheet);
				return sheet.SheetNum;
			}
			return Crud.SheetCrud.Insert(sheet);
		}

		///<summary></summary>
		public static void Update(Sheet sheet) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),sheet);
				return;
			}
			Crud.SheetCrud.Update(sheet);
		}

		///<summary>Sets the IsDeleted flag to true (1) for the specified sheetNum.  The sheet and associated sheetfields are not deleted.</summary>
		public static void Delete(long sheetNum,long patNum=0,byte showInTerminal=0) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),sheetNum,patNum,showInTerminal);
				return;
			}
			string command="UPDATE sheet SET IsDeleted=1,ShowInTerminal=0 WHERE SheetNum="+POut.Long(sheetNum);
			Db.NonQ(command);
			if(patNum>0 && showInTerminal>0) {//showInTerminal must be at least 1, so decrementing those that are at least 2
				command="UPDATE sheet SET ShowInTerminal=ShowInTerminal-1 "
					+"WHERE PatNum="+POut.Long(patNum)+" "
					+"AND IsDeleted=0 "
					+"AND ShowInTerminal>"+POut.Byte(showInTerminal);//decrement ShowInTerminal for all sheets with a bigger ShowInTerminal than the one deleted
				Db.NonQ(command);
				//Push deleted sheet to eClipboard.
				WebTypes.PushNotificationUtils.CI_RemoveSheet(patNum,sheetNum);
			}
		}

		///<summary>Converts parameters into sheetfield objects, and then saves those objects in the database.  
		///The parameters will never again enjoy full parameter status, but will just be read-only fields from here on out.
		///It ignores PatNum parameters, since those are already part of the sheet itself.</summary>
		public static void SaveParameters(Sheet sheet){
			//No need to check MiddleTierRole; no call to db
			List<SheetField> listSheetFields=new List<SheetField>();
			for(int i=0;i<sheet.Parameters.Count;i++){
				if(sheet.Parameters[i].ParamName.In("PatNum",
					//These types are not primitives so they cannot be saved to the database.
					"CompletedProcs","toothChartImg"))
				{
					continue;
				}
				if(!sheet.Parameters[i].IsRequired && sheet.Parameters[i].ParamValue==null) {
					continue;
				}
				SheetField sheetField=new SheetField();
				sheetField.IsNew=true;
				sheetField.SheetNum=sheet.SheetNum;
				sheetField.FieldType=SheetFieldType.Parameter;
				sheetField.FieldName=sheet.Parameters[i].ParamName;
				if(sheet.Parameters[i].ParamName=="ListProcNums") {//Save this parameter as a comma delimited list
					List<long> listProcNums=(List<long>)SheetParameter.GetParamByName(sheet.Parameters,"ListProcNums").ParamValue;
					sheetField.FieldValue=String.Join(",",listProcNums);
				}
				else {
					sheetField.FieldValue=sheet.Parameters[i].ParamValue.ToString();//the object will be an int. Stored as a string.
				}
				sheetField.FontSize=0;
				sheetField.FontName="";
				sheetField.FontIsBold=false;
				sheetField.XPos=0;
				sheetField.YPos=0;
				sheetField.Width=0;
				sheetField.Height=0;
				sheetField.GrowthBehavior=GrowthBehaviorEnum.None;
				sheetField.RadioButtonValue="";
				listSheetFields.Add(sheetField);
			}
			SheetFields.InsertMany(listSheetFields);
		}

		///<summary>Loops through all the fields in the sheet and appends together all the FieldValues.  It obviously excludes all SigBox fieldtypes.  It does include Drawing fieldtypes, so any change at all to any drawing will invalidate the signature.  It does include Image fieldtypes, although that's just a filename and does not really have any meaningful data about the image itself.  The order is absolutely critical.</summary>
		public static string GetSignatureKey(Sheet sheet) {
			//No need to check MiddleTierRole; no call to db
			//The order of sheet fields is absolutely critical when it comes to the signature key.
			//Therefore, we will make a local copy of the sheet fields and sort them how we want them here just in case their order has changed for any other reason.
			List<SheetField> listSheetFieldsCopy=new List<SheetField>();
			for(int i=0;i<sheet.SheetFields.Count;i++) {
				listSheetFieldsCopy.Add(sheet.SheetFields[i]);
			}
			if(listSheetFieldsCopy.All(x => x.SheetFieldNum > 0)) {//the sheet has not been loaded into the db, so it has no primary keys to sort on
				listSheetFieldsCopy.Sort(SheetFields.SortPrimaryKey);
			}
			return UI.SigBox.GetSignatureKeySheets(listSheetFieldsCopy);
		}

		public static DataTable GetPatientFormsTable(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum);
			}
			//DataConnection dcon=new DataConnection();
			DataTable table=new DataTable("");
			DataRow dataRow;
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("date");
			table.Columns.Add("dateOnly",typeof(DateTime));//to help with sorting
			table.Columns.Add("dateTime",typeof(DateTime));
			table.Columns.Add("DateTSheetEdited",typeof(DateTime));
			table.Columns.Add("description");
			table.Columns.Add("DocNum");
			table.Columns.Add("imageCat");
			table.Columns.Add("SheetNum");
			table.Columns.Add("showInTerminal");
			table.Columns.Add("time");
			table.Columns.Add("timeOnly",typeof(TimeSpan));//to help with sorting
			//but we won't actually fill this table with rows until the very end.  It's more useful to use a List<> for now.
			List<DataRow> listDataRows=new List<DataRow>();
			//sheet---------------------------------------------------------------------------------------
			string command="SELECT DateTimeSheet,SheetNum,Description,ShowInTerminal,DateTSheetEdited "
				+"FROM sheet WHERE IsDeleted=0 "
				+"AND PatNum ="+POut.Long(patNum)+" "
				+"AND (SheetType="+POut.Long((int)SheetTypeEnum.PatientForm)+" OR SheetType="+POut.Long((int)SheetTypeEnum.MedicalHistory);
			if(PrefC.GetBool(PrefName.PatientFormsShowConsent)) {
				command+=" OR SheetType="+POut.Long((int)SheetTypeEnum.Consent);//Show consent forms if pref is true.
			}
			command+=")";
				//+"ORDER BY ShowInTerminal";//DATE(DateTimeSheet),ShowInTerminal,TIME(DateTimeSheet)";
			DataTable tableRawSheet=Db.GetTable(command);
			DateTime dateT;
			for(int i=0;i<tableRawSheet.Rows.Count;i++) {
				dataRow=table.NewRow();
				dateT=PIn.DateT(tableRawSheet.Rows[i]["DateTimeSheet"].ToString());
				dataRow["date"]=dateT.ToShortDateString();
				dataRow["dateOnly"]=dateT.Date;
				dataRow["dateTime"]=dateT;
				dataRow["DateTSheetEdited"]=PIn.DateT(tableRawSheet.Rows[i]["DateTSheetEdited"].ToString());
				dataRow["description"]=tableRawSheet.Rows[i]["Description"].ToString();
				dataRow["DocNum"]="0";
				dataRow["imageCat"]="";
				dataRow["SheetNum"]=tableRawSheet.Rows[i]["SheetNum"].ToString();
				if(tableRawSheet.Rows[i]["ShowInTerminal"].ToString()=="0") {
					dataRow["showInTerminal"]="";
				}
				else {
					dataRow["showInTerminal"]=tableRawSheet.Rows[i]["ShowInTerminal"].ToString();
				}
				if(dateT.TimeOfDay!=TimeSpan.Zero) {
					dataRow["time"]=dateT.ToString("h:mm")+dateT.ToString("%t").ToLower();
				}
				dataRow["timeOnly"]=dateT.TimeOfDay;
				listDataRows.Add(dataRow);
			}
			//document---------------------------------------------------------------------------------------
			command="SELECT DateCreated,DocCategory,DocNum,Description,document.DateTStamp "
				+"FROM document,definition "
				+"WHERE document.DocCategory=definition.DefNum"
				+" AND PatNum ="+POut.Long(patNum)
				+" AND definition.ItemValue LIKE '%F%'";
				//+" ORDER BY DateCreated";
			DataTable tableRawDoc=Db.GetTable(command);
			long docCat;
			for(int i=0;i<tableRawDoc.Rows.Count;i++) {
				dataRow=table.NewRow();
				dateT=PIn.DateT(tableRawDoc.Rows[i]["DateCreated"].ToString());
				dataRow["date"]=dateT.ToShortDateString();
				dataRow["dateOnly"]=dateT.Date;
				dataRow["dateTime"]=dateT;
				dataRow["DateTSheetEdited"]=PIn.DateT(tableRawDoc.Rows[i]["DateTStamp"].ToString());
				dataRow["description"]=tableRawDoc.Rows[i]["Description"].ToString();
				dataRow["DocNum"]=tableRawDoc.Rows[i]["DocNum"].ToString();
				docCat=PIn.Long(tableRawDoc.Rows[i]["DocCategory"].ToString());
				dataRow["imageCat"]=Defs.GetName(DefCat.ImageCats,docCat);
				dataRow["SheetNum"]="0";
				dataRow["showInTerminal"]="";
				if(dateT.TimeOfDay!=TimeSpan.Zero) {
					dataRow["time"]=dateT.ToString("h:mm")+dateT.ToString("%t").ToLower();
				}
				dataRow["timeOnly"]=dateT.TimeOfDay;
				listDataRows.Add(dataRow);
			}
			//Sorting
			for(int i=0;i<listDataRows.Count;i++) {
				table.Rows.Add(listDataRows[i]);
			}
			DataView dataView = table.DefaultView;
			dataView.Sort = "dateOnly,showInTerminal,timeOnly";
			table = dataView.ToTable();
			return table;
		}

		///<summary>Returns all sheets for the given patient in the given date range which have a description matching the examDescript in a case insensitive manner. If examDescript is blank, then sheets with any description are returned.</summary>
		public static List<Sheet> GetExamSheetsTable(long patNum,DateTime dateStart,DateTime dateEnd,long sheetDefNum=-1) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Sheet>>(MethodBase.GetCurrentMethod(),patNum,dateStart,dateEnd,sheetDefNum);
			}
			string command="SELECT * "
				+"FROM sheet WHERE IsDeleted=0 "
				+"AND PatNum="+POut.Long(patNum)+" "
				+"AND SheetType="+POut.Int((int)SheetTypeEnum.ExamSheet)+" ";
			if(sheetDefNum!=-1){
				command+="AND SheetDefNum = "+POut.Long(sheetDefNum)+" ";
			}
			command+="AND "+DbHelper.DtimeToDate("DateTimeSheet")+">="+POut.Date(dateStart)+" AND "+DbHelper.DtimeToDate("DateTimeSheet")+"<="+POut.Date(dateEnd)+" "
				+"ORDER BY DateTimeSheet";
			return Crud.SheetCrud.SelectMany(command);
		}

		///<summary>Used to get sheets that were automatically downloaded by the Open Dental Service. These are the sheets that have many or no matching patients that still need to be manually attached to an existing or new pat.</summary>
		public static List<Sheet> GetUnmatchedWebFormSheets(List<long> listClinicNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Sheet>>(MethodBase.GetCurrentMethod(),listClinicNums);
			}
			string command="SELECT * "
				+"FROM sheet WHERE IsDeleted=0 " 
				+"AND PatNum=0 "
				+"AND IsWebForm = "+POut.Bool(true)+ " "
				+"AND (SheetType="+POut.Long((int)SheetTypeEnum.PatientForm)+" OR SheetType="+POut.Long((int)SheetTypeEnum.MedicalHistory)+") "
				+(PrefC.HasClinicsEnabled ? "AND ClinicNum IN ("+string.Join(",", listClinicNums)+") " : "");
			List<Sheet> listSheets=Crud.SheetCrud.SelectMany(command);
			//Get the Sheetfields and parameters for each of the auto downloaded sheets
			for(int i=0;i<listSheets.Count;i++) {
				SheetFields.GetFieldsAndParameters(listSheets[i]);
			}
			return listSheets;
		}

		///<summary>Used to get the count of sheets that are going to be downloaded by the Open Dental Service. Used for creating AlertItems.</summary>
		public static int GetUnmatchedWebFormSheetsCount(List<long> listClinicNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),listClinicNums);
			}
			string command="SELECT COUNT(SheetNum) "
				+"FROM sheet WHERE IsDeleted=0 " 
				+"AND PatNum=0 "
				+"AND IsWebForm = "+POut.Bool(true)+ " "
				+"AND (SheetType="+POut.Long((int)SheetTypeEnum.PatientForm)+" OR SheetType="+POut.Long((int)SheetTypeEnum.MedicalHistory)+") "
				+(PrefC.HasClinicsEnabled ? "AND ClinicNum IN ("+string.Join(",", listClinicNums)+") " : "");
			return Db.GetInt(command);
		}

		///<summary>Used to get sheets filled via the web.  Passing in a null or empty list of clinic nums will only return sheets that are not assigned to a clinic.</summary>
		public static DataTable GetWebFormSheetsTable(DateTime dateFrom,DateTime dateTo,List<long> listClinicNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listClinicNums);
			}
			if(listClinicNums==null || listClinicNums.Count==0) {
				listClinicNums=new List<long>() { 0 };//To ensure we filter on at least one clinic (HQ).
			}
			DataTable table=new DataTable("");
			DataRow dataRow;
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("date");
			table.Columns.Add("dateOnly",typeof(DateTime));//to help with sorting
			table.Columns.Add("dateTime",typeof(DateTime));
			table.Columns.Add("description");
			table.Columns.Add("time");
			table.Columns.Add("timeOnly",typeof(TimeSpan));//to help with sorting
			table.Columns.Add("PatNum");
			table.Columns.Add("SheetNum");
			table.Columns.Add("IsDeleted");
			table.Columns.Add("ClinicNum");
			List<DataRow> listDataRows=new List<DataRow>();
			string command="SELECT DateTimeSheet,Description,PatNum,SheetNum,IsDeleted,ClinicNum "
				+"FROM sheet WHERE " 
				+"DateTimeSheet >= "+POut.Date(dateFrom)+" AND DateTimeSheet <= "+POut.Date(dateTo.AddDays(1))+ " "
				+"AND IsWebForm = "+POut.Bool(true)+ " "
				+"AND (SheetType="+POut.Long((int)SheetTypeEnum.PatientForm)+" OR SheetType="+POut.Long((int)SheetTypeEnum.MedicalHistory)+") "
				+(PrefC.HasClinicsEnabled ? "AND ClinicNum IN ("+string.Join(",", listClinicNums)+") " : "");
			DataTable tableRawSheet=Db.GetTable(command);
			DateTime dateT;
			for(int i=0;i<tableRawSheet.Rows.Count;i++) {
				dataRow=table.NewRow();
				dateT=PIn.DateT(tableRawSheet.Rows[i]["DateTimeSheet"].ToString());
				dataRow["date"]=dateT.ToShortDateString();
				dataRow["dateOnly"]=dateT.Date;
				dataRow["dateTime"]=dateT;
				dataRow["description"]=tableRawSheet.Rows[i]["Description"].ToString();
				dataRow["PatNum"]=tableRawSheet.Rows[i]["PatNum"].ToString();
				dataRow["SheetNum"]=tableRawSheet.Rows[i]["SheetNum"].ToString();
				if(dateT.TimeOfDay!=TimeSpan.Zero) {
					dataRow["time"]=dateT.ToString("h:mm")+dateT.ToString("%t").ToLower();
				}
				dataRow["timeOnly"]=dateT.TimeOfDay;
				dataRow["IsDeleted"]=tableRawSheet.Rows[i]["IsDeleted"].ToString();
				dataRow["ClinicNum"]=PIn.Long(tableRawSheet.Rows[i]["ClinicNum"].ToString());
				listDataRows.Add(dataRow);
			}
			for(int i=0;i<listDataRows.Count;i++) {
				table.Rows.Add(listDataRows[i]);
			}
			DataView dataView = table.DefaultView;
			dataView.Sort = "dateOnly,timeOnly";
			table = dataView.ToTable();
			return table;
		}

		public static bool ContainsStaticField(Sheet sheet,string fieldName) {
			//No need to check MiddleTierRole; no call to db
			List<SheetField> listSheetFields=sheet.SheetFields;
			for(int i=0;i<listSheetFields.Count;i++) {
				if(listSheetFields[i].FieldType!=SheetFieldType.StaticText) {
					continue;
				}
				if(listSheetFields[i].FieldValue.Contains("["+fieldName+"]")) {
					return true;
				}
			}
			return false;
		}

		///<summary></summary>
		public static byte GetBiggestShowInTerminal(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<byte>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT MAX(ShowInTerminal) FROM sheet WHERE IsDeleted=0 AND PatNum="+POut.Long(patNum);
			return PIn.Byte(Db.GetScalar(command));
		}

		///<summary></summary>
		public static void ClearFromTerminal(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			string command="UPDATE sheet SET ShowInTerminal=0 WHERE PatNum="+POut.Long(patNum);
			Db.NonQ(command);
		}

		///<summary>This gives the number of pages required to print all fields. This must be calculated ahead of time when creating multi page pdfs.</summary>
		public static int CalculatePageCount(Sheet sheet,System.Drawing.Printing.Margins margins) {
			//HeightPage is the value of Width/Length depending on Landscape/Portrait.
			int bottomLastField=0;
			if(sheet.SheetFields.Count>0) {
				bottomLastField=sheet.SheetFields.Max(x=>x.Bounds.Bottom);
			}
			if(bottomLastField<=sheet.HeightPage && sheet.SheetType!=SheetTypeEnum.MedLabResults) {//MedLabResults always implements footer, needs true multi-page count
				return 1;//if all of the fields are less than one page, even if some of the fields fall within the margin of the first page.
			}
			if(SheetTypeIsSinglePage(sheet.SheetType)) {
				return 1;//labels and RX forms are always single pages
			}
			SetPageMargin(sheet,margins);
			int printableHeightPerPage=(sheet.HeightPage-(margins.Top+margins.Bottom));
			if(printableHeightPerPage<1) {
				return 1;//otherwise we get negative, infinite, or thousands of pages.
			}
			int maxY=0;
			for(int i=0;i<sheet.SheetFields.Count;i++) {
				maxY=Math.Max(maxY,sheet.SheetFields[i].Bounds.Bottom);
			}
			int pageCount=1;
			maxY-=margins.Top;//adjust for ignoring the top margin of the first page.
			pageCount=Convert.ToInt32(Math.Ceiling((double)maxY/printableHeightPerPage));
			pageCount=Math.Max(pageCount,1);//minimum of at least one page.
			return pageCount;
		}

		public static void SetPageMargin(Sheet sheet,System.Drawing.Printing.Margins margins) {
			margins.Left=0;
			margins.Right=0;
			if(SheetTypeIsSinglePage(sheet.SheetType)) {
				margins.Top=0;
				margins.Bottom=0;
				//m=new System.Drawing.Printing.Margins(0,0,0,0); //does not work, creates new reference.
				return;
			}
			margins.Top=40;
			if(sheet.SheetType==SheetTypeEnum.MedLabResults) {
				margins.Top=120;
			}
			margins.Bottom=60;
			return;
		}

		public static void SetSheetFieldsForSheets(List<Sheet> listSheets) {
			List<long> listSheetNums=listSheets.Select(x => x.SheetNum).ToList();
			List<SheetField> listSheetFields=SheetFields.GetListForSheets(listSheetNums);
			for(int i=0;i<listSheets.Count;i++) {
				List<SheetField> listSheetFieldsForSheet=listSheetFields.FindAll(x => x.SheetNum==listSheets[i].SheetNum);
				listSheetFieldsForSheet.Sort(SheetFields.SortDrawingOrderLayers);
				SheetFields.GetFieldsAndParameters(listSheets[i],listSheetFieldsForSheet);
				List<SheetField> listSheetFieldsSigBox=listSheets[i].SheetFields
					.FindAll(x => x.FieldType.In(SheetFieldType.SigBox,SheetFieldType.SigBoxPractice));
				for(int j=0;j<listSheetFieldsSigBox.Count;j++) {
					listSheetFieldsSigBox[j].SigKey=Sheets.GetSignatureKey(listSheets[i]);
				}
			}
		}

		public static bool SheetTypeIsSinglePage(SheetTypeEnum sheetType) {
			switch(sheetType) {
				case SheetTypeEnum.LabelPatient:
				case SheetTypeEnum.LabelCarrier:
				case SheetTypeEnum.LabelReferral:
				//case SheetTypeEnum.ReferralSlip:
				case SheetTypeEnum.LabelAppointment:
				case SheetTypeEnum.Rx:
				//case SheetTypeEnum.Consent:
				//case SheetTypeEnum.PatientLetter:
				//case SheetTypeEnum.ReferralLetter:
				//case SheetTypeEnum.PatientForm:
				//case SheetTypeEnum.RoutingSlip:
				//case SheetTypeEnum.MedicalHistory:
				//case SheetTypeEnum.LabSlip:
				//case SheetTypeEnum.ExamSheet:
				case SheetTypeEnum.DepositSlip:
				//case SheetTypeEnum.Statement:
				case SheetTypeEnum.PatientDashboardWidget:
					return true;
			}
			return false;
		}

		#region Xamarin Methods
		///<summary>This is supposed to be used explicity with Sheets and not the old Open Dental way of creating sheets.</summary>
		public static string CreatePdfForXamarin(long sheetNum) {
			Sheet sheet=GetSheet(sheetNum);
			SheetFields.GetFieldsAndParameters(sheet);
			SheetDrawingJob sheetDrawingJob=new SheetDrawingJob();
			string tempFile=PrefC.GetRandomTempFile(".pdf");
			string rawBase64="";
			//Create a PDF with the given sheet and file. The other parameters can remain null, because they aren't used for TreatPlan sheets.
			PdfDocument pdfDocument=sheetDrawingJob.CreatePdf(sheet);
			SheetDrawingJob.SavePdfToFile(pdfDocument, tempFile);
			//Convert the pdf into its raw bytes
			rawBase64=Convert.ToBase64String(System.IO.File.ReadAllBytes(tempFile));
			return Convert.ToBase64String(System.IO.File.ReadAllBytes(tempFile));
		}

		public static Sheet CreateExamSheet(long patNum,long sheetDefNum) {
			SheetDef sheetDef=SheetDefs.GetSheetDef(sheetDefNum);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,patNum);
			SheetParameter.SetParameter(sheet,"PatNum",patNum);
			SheetFiller.FillFields(sheet);
			SheetUtil.CalculateHeights(sheet);
			return sheet;
		}
		#endregion Xamarin Methods
	}
}