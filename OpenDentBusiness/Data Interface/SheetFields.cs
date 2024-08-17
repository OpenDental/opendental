﻿using System;
using System.Collections.Generic;
using System.Reflection;
using CodeBase;
using System.Linq;

namespace OpenDentBusiness{
	///<summary></summary>
	public class SheetFields{
		#region Insert

		public static void InsertMany(List<SheetField> listSheetFields) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSheetFields);
				return;
			}
			Crud.SheetFieldCrud.InsertMany(listSheetFields);
		}

		#endregion

		///<Summary>Gets one SheetField from the database.</Summary>
		public static SheetField CreateObject(long sheetFieldNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<SheetField>(MethodBase.GetCurrentMethod(),sheetFieldNum);
			}
			return Crud.SheetFieldCrud.SelectOne(sheetFieldNum);
		}

		public static List<SheetField> GetListForSheet(long sheetNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SheetField>>(MethodBase.GetCurrentMethod(),sheetNum);
			}
			string command="SELECT * FROM sheetfield WHERE SheetNum="+POut.Long(sheetNum)
				+" ORDER BY SheetFieldNum";//the ordering is CRITICAL because the signature key is based on order.
			return Crud.SheetFieldCrud.SelectMany(command);
		}

		///<summary>Returns a list of SheetFields for the list of SheetNums passed in.</summary>
		public static List<SheetField> GetListForSheets(List<long> listSheetNums) {
			if(listSheetNums.IsNullOrEmpty()) {
				return new List<SheetField>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SheetField>>(MethodBase.GetCurrentMethod(),listSheetNums);
			}
			string command=$"SELECT * FROM sheetfield WHERE SheetNum IN({string.Join(",",listSheetNums.Select(x => POut.Long(x)))})";
			return Crud.SheetFieldCrud.SelectMany(command);
		}

		///<summary>When we need to use a sheet, we must run this method to pull all the associated fields and parameters from the database.
		///Then it will be ready for printing, copying, etc.</summary>
		public static void GetFieldsAndParameters(Sheet sheet,List<SheetField> listSheetFields=null){
			//No need to check MiddleTierRole; no call to db.
			if(listSheetFields==null) {
				sheet.SheetFields=GetListForSheet(sheet.SheetNum);
			}
			else {
				sheet.SheetFields=listSheetFields;
			}
			//so parameters will also be in the field list, but they will just be ignored from here on out.
			//because we will have an explicit parameter list instead.
			sheet.Parameters=new List<SheetParameter>();
			SheetParameter param;
			//int paramVal;
			for(int i=0;i<sheet.SheetFields.Count;i++){
				if(sheet.SheetFields[i].FieldType==SheetFieldType.Parameter){
					param=new SheetParameter(true,sheet.SheetFields[i].FieldName,sheet.SheetFields[i].FieldValue);
					sheet.Parameters.Add(param);
				}
			}
		}

		///<summary>Used in SheetFiller to fill patient letter with exam sheet information.  Will return null if no exam sheet matching the description exists for the patient.  Usually just returns one field, but will return a list of fields if it's for a RadioButtonGroup.</summary>
		public static List<SheetField> GetFieldFromExamSheet(long patNum,string examDescript,string fieldName) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<SheetField>>(MethodBase.GetCurrentMethod(),patNum,examDescript,fieldName);
			}
			Sheet sheet=Sheets.GetMostRecentExamSheet(patNum,examDescript);
			if(sheet==null) {
				return null;
			}
			string command="SELECT * FROM sheetfield WHERE SheetNum="
				+POut.Long(sheet.SheetNum)+" "
				+"AND (RadioButtonGroup='"+POut.String(fieldName)+"' OR ReportableName='"+POut.String(fieldName)+"' OR FieldName='"+POut.String(fieldName)+"')";
			return Crud.SheetFieldCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(SheetField sheetField) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				sheetField.SheetFieldNum=Meth.GetLong(MethodBase.GetCurrentMethod(),sheetField);
				return sheetField.SheetFieldNum;
			}
			return Crud.SheetFieldCrud.Insert(sheetField);
		}

		///<summary></summary>
		public static void Update(SheetField sheetField) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),sheetField);
				return;
			}
			Crud.SheetFieldCrud.Update(sheetField);
		}

		///<summary></summary>
		public static void DeleteObject(long sheetFieldNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),sheetFieldNum);
				return;
			}
			Crud.SheetFieldCrud.Delete(sheetFieldNum);
		}

		///<summary>Deletes all existing drawing fields for a sheet from the database and then adds back the list supplied.</summary>
		public static void SetDrawings(List<SheetField> drawingList,long sheetNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),drawingList,sheetNum);
				return;
			}
			string command="DELETE FROM sheetfield WHERE SheetNum="+POut.Long(sheetNum)
				+" AND FieldType="+POut.Long((int)SheetFieldType.Drawing);
			Db.NonQ(command);
			foreach(SheetField field in drawingList){
				Insert(field);
			}
		}

		///<summary>Sorts fields in the order that they shoudl be drawn on top of eachother. First Images, then Drawings, Lines, Rectangles, Text, Check Boxes, and SigBoxes. In that order.</summary>
		public static int SortDrawingOrderLayers(SheetField f1,SheetField f2) {
			if(FieldTypeSortOrder(f1.FieldType)!=FieldTypeSortOrder(f2.FieldType)) {
				return FieldTypeSortOrder(f1.FieldType).CompareTo(FieldTypeSortOrder(f2.FieldType));
			}
			return f1.YPos.CompareTo(f2.YPos);
			//return f1.SheetFieldNum.CompareTo(f2.SheetFieldNum);
		}

		///<summary></summary>
		public static DateTime GetBirthDate(string strDate,bool isWebForm,bool isCemtTransfer,string cultureName="") {
			DateTime dateTime;
			//Parse the birthdate field using our websheet_preference for this practice if this sheet was a WebForm, otherwise, use the current 
			//computer's region/language settings.
			if(isWebForm && !isCemtTransfer) {
				dateTime=WebTypes.WebForms.WebForms_Sheets.ParseDateWebForms(strDate,cultureName);
			}
			else {
				dateTime=PIn.Date(strDate);
			}
			return dateTime;
		}

		///<summary>Re-orders the SheetFieldType enum to a drawing order. Images should be drawn first, then drawings, then lines, then rectangles, etc...</summary>
		internal static int FieldTypeSortOrder(SheetFieldType t) {
			switch(t) {
				case SheetFieldType.Image:
				case SheetFieldType.PatImage:
					return 0;
				case SheetFieldType.Drawing:
					return 1;
				case SheetFieldType.Line:
				case SheetFieldType.Rectangle:
					return 2;
				case SheetFieldType.Grid:
					return 3;
				case SheetFieldType.OutputText:
				case SheetFieldType.InputField:
				case SheetFieldType.StaticText:
					return 4;
				case SheetFieldType.CheckBox:
					return 5;
				case SheetFieldType.SigBox:
				case SheetFieldType.SigBoxPractice:
					return 6;
				case SheetFieldType.Special:
				case SheetFieldType.Parameter:
				default:
					return int.MaxValue;
			}
		}

		///<summary>Sorts the sheet fields by SheetFieldNum.  This is used when creating a signature key and is absolutely critical that it not change.</summary>
		public static int SortPrimaryKey(SheetField f1,SheetField f2) {
			return f1.SheetFieldNum.CompareTo(f2.SheetFieldNum);
		}

		///<summary>SigBoxes must be synced after all other fields have been synced for the keyData to be in the right order.
		///So sync must be called first without SigBoxes, then the keyData for the signature(s) can be retrieved, then the SigBoxes can be synced.
		///This function uses a DB comparison rather than a stale list because we are not worried about concurrency of a single sheet and enhancing the
		///functions that call this would take a lot of restructuring.</summary>
		public static void Sync(List<SheetField> listSheetFieldsNew,long sheetNum,bool isSigBoxOnly) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listSheetFieldsNew,sheetNum,isSigBoxOnly);
				return;
			}
			List<SheetField> listSheetFieldsDB=SheetFields.GetListForSheet(sheetNum);
			if(!isSigBoxOnly) {
				List<SheetField> listNoSigNew=listSheetFieldsNew.FindAll(x => x.FieldType!=SheetFieldType.Parameter 
					&& !x.FieldType.In(SheetFieldType.SigBox,SheetFieldType.SigBoxPractice));
				List<SheetField> listNoSigDB=listSheetFieldsDB.FindAll(x => x.FieldType!=SheetFieldType.Parameter 
					&& !x.FieldType.In(SheetFieldType.SigBox,SheetFieldType.SigBoxPractice));
				Crud.SheetFieldCrud.Sync(listNoSigNew,listNoSigDB);
			}
			else {
				//SigBoxes must come after ALL other types in order for the keyData to be in the right order.
				List<SheetField> listSigOnlyNew=listSheetFieldsNew.FindAll(x => x.FieldType.In(SheetFieldType.SigBox,SheetFieldType.SigBoxPractice));
				List<SheetField> listSigOnlyDB=listSheetFieldsDB.FindAll(x => x.FieldType.In(SheetFieldType.SigBox,SheetFieldType.SigBoxPractice));
				Crud.SheetFieldCrud.Sync(listSigOnlyNew,listSigOnlyDB);
			}
		}

		public static string GetComboSelectedOption(SheetField sheetField){
			//No need to check MiddleTierRole; no call to db.
			string[] stringArray=sheetField.FieldValue.Split(';');
			if(stringArray.Length>1) {
				string str=stringArray[0];//empty string when nothing selected
				return str;
			}
			//Incorrect format.
			return "";
		}

		public static List<string> GetComboMenuItems(SheetField sheetField){
			//No need to check MiddleTierRole; no call to db.
			List<string> listStringsReturn=new List<string>();
			string[] stringArray=sheetField.FieldValue.Split(';');
			if(stringArray.Length>1) {
				listStringsReturn=stringArray[1].Split('|').ToList();
			}
			else{//Incorrect format.
				//Default to empty string when 'values' is in format 'A|B|C', indicating only combobox options, 
				//rather than 'C;A|B|C' which indicates selection as well as options.
				//Upon Ok click this will correct the fieldvalue format.
				listStringsReturn=stringArray[0].Split('|').ToList();//Will be an empty string if no '|' is present.
			}
			for(int i=0;i<listStringsReturn.Count;i++) {
				//'&' is a special character in System.Windows.Forms.ContextMenu. We need to escapte all ampersands so that they are displayed correctly in the fill sheet window.
				listStringsReturn[i]=listStringsReturn[i].Replace("&","&&");
				//'-' by itself is a special character in System.Windows.Forms.ContextMenu. We need to escapte it so that it is displayed correctly in the fill sheet window.
				if(listStringsReturn[i]=="-") {
					listStringsReturn[i]="&-";
				}
			}
			return listStringsReturn;
		}

		public static void SetComboFieldValue(SheetField sheetField,string selectedOption){
			string stringAll="";
			string[] stringArray=sheetField.FieldValue.Split(';');
			if(stringArray.Length>1) {
				stringAll=stringArray[1];
			}
			else{//Incorrect format.
				stringAll=stringArray[0];
			}
			string fieldVal= selectedOption+";"+stringAll;
			sheetField.FieldValue=fieldVal;
		}
	}
}