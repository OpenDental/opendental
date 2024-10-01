using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using CodeBase;

namespace OpenDentBusiness{
  ///<summary></summary>
	public class Screens {
		///<summary>Gets one Screen from the db.</summary>
		public static Screen GetOne(long screenNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Screen>(MethodBase.GetCurrentMethod(),screenNum);
			}
			return Crud.ScreenCrud.SelectOne(screenNum);
		}

		///<summary>After taking multiple screenings using a sheets, this method will import all sheets as screens and insert them into the db.
		///The goal of this method is that the office will fill out a bunch of sheets (in the web?).
		///Then after they get back to their office (with connection to their db) they will push a button to upload / insert a batch.</summary>
		public static List<Screen> CreateScreensFromSheets(List<Sheet> listSheets) {
			Meth.NoCheckMiddleTierRole();
			List<Screen> listScreens=new List<Screen>();
			for(int i=0;i<listSheets.Count;i++) {
				listScreens.Add(CreateScreenFromSheet(listSheets[i]));
			}
			return listScreens;
		}

		///<summary>After taking a screening using a sheet, this method will import the sheet as a screen and insert it into the db.
		///Returns null if the sheet passed in is not a Screening sheet type or if the sheet is missing the required ScreenGroupNum param.
		///Optionally supply a screen if you want to preset some values.  E.g. ScreenGroupOrder is often preset before calling this method.</summary>
		public static Screen CreateScreenFromSheet(Sheet sheet,Screen screen=null) {
			Meth.NoCheckMiddleTierRole();
			//Make sure that the sheet passed in is a screening and contains the required ScreenGroupNum parameter.
			if(sheet.SheetType!=SheetTypeEnum.Screening || SheetParameter.GetParamByName(sheet.Parameters,"ScreenGroupNum")==null) {
				return null;
			}
			if(screen==null) {
				screen=new Screen();
				screen.ScreenGroupNum=(long)SheetParameter.GetParamByName(sheet.Parameters,"ScreenGroupNum").ParamValue;
			}
			screen.SheetNum=sheet.SheetNum;
			for(int i=0;i<sheet.SheetFields.Count;i++) {
				switch(sheet.SheetFields[i].FieldName) {
					case "Gender":
						if(sheet.SheetFields[i].FieldValue.Trim().ToLower().StartsWith("m")) {
							screen.Gender=PatientGender.Male;
						}
						else if(sheet.SheetFields[i].FieldValue.Trim().ToLower().StartsWith("f")) {
							screen.Gender=PatientGender.Female;
						}
						else {
							screen.Gender=PatientGender.Unknown;
						}
						break;
					case "Race/Ethnicity":
						PatientRaceOld patientRace=PatientRaceOld.Unknown;
						Enum.TryParse<PatientRaceOld>(sheet.SheetFields[i].FieldValue.Split(';')[0],out patientRace);
						screen.RaceOld=patientRace;
						break;
					case "GradeLevel":
						PatientGrade patientGrade=PatientGrade.Unknown;
						Enum.TryParse<PatientGrade>(sheet.SheetFields[i].FieldValue.Split(';')[0],out patientGrade);
						screen.GradeLevel=patientGrade;
						break;
					case "Age":
						if(screen.Age!=0) {
							break;//Already calculated via Birthdate.
						}
						byte byteAge=0;
						byte.TryParse(sheet.SheetFields[i].FieldValue,out byteAge);
						screen.Age=byteAge;
						break;
					case "Urgency":
						TreatmentUrgency treatmentUrgency=TreatmentUrgency.Unknown;
						Enum.TryParse<TreatmentUrgency>(sheet.SheetFields[i].FieldValue.Split(';')[0],out treatmentUrgency);
						screen.Urgency=treatmentUrgency;
						break;
					case "ChartSealantTreatment":
						//Only mark "carious" if TP chart has C marked for any tooth surface.
						screen.HasCaries=YN.No;
						if(sheet.SheetFields[i].FieldValue.Contains("C")) {
							screen.HasCaries=YN.Yes;//Caries is present in TP'd chart.  Compl chart doesn't matter, it's only for sealant placement.
						}
						//Only mark "needs sealants" if TP chart has S marked for any tooth surface.
						screen.NeedsSealants=YN.No;
						if(sheet.SheetFields[i].FieldValue.Contains("S")) {
							screen.NeedsSealants=YN.Yes;
						}
						break;
					case "CariesExperience":
						screen.CariesExperience=sheet.SheetFields[i].FieldValue=="X" ? YN.Yes : YN.No;
						break;
					case "EarlyChildCaries":
						screen.EarlyChildCaries=sheet.SheetFields[i].FieldValue=="X" ? YN.Yes : YN.No;
						break;
					case "ExistingSealants":
						screen.ExistingSealants=sheet.SheetFields[i].FieldValue=="X" ? YN.Yes : YN.No;
						break;
					case "MissingAllTeeth":
						screen.MissingAllTeeth=sheet.SheetFields[i].FieldValue=="X" ? YN.Yes : YN.No;
						break;
					case "Birthdate":
						DateTime dateBirth=new DateTime(1,1,1);
						DateTime.TryParse(sheet.SheetFields[i].FieldValue,out dateBirth);
						screen.Birthdate=dateBirth;
						//Check to see if the sheet has Age manually filled out.  
						//If Age was not manually set, automatically calculate the age based on the birthdate entered.
						//This matches screening functionality.
						SheetField sheetFieldAge=sheet.SheetFields.FirstOrDefault(x => x.FieldName=="Age");
						if(sheetFieldAge!=null && string.IsNullOrEmpty(sheetFieldAge.FieldValue)) {
							screen.Age=PIn.Byte(Patients.DateToAge(dateBirth).ToString());
						}
						break;
					case "Comments":
						screen.Comments=sheet.SheetFields[i].FieldValue;
						break;
				}
			}
			if(screen.ScreenNum==0) {
				Insert(screen);
			}
			else {
				Update(screen);
			}
			return screen;
		}

		///<summary>Takes a screening sheet that is associated to a patient and processes any corresponding ScreenCharts found.
		///Processing will create treatment planned or completed procedures for the patient.
		///Supply the sheet and then a bitwise enum of screen chart types to digest.
		///listSheetFieldsProcOrig, nulls are allowed, the first represents the fluoride field, second is assessment field, all others are other procs.</summary>
		public static void ProcessScreenChart(Sheet sheet,ScreenChartType screenChartTypes,long provNum,long sheetNum,List<SheetField> listSheetFieldsChartOrig
			,List<SheetField> listSheetFieldsProcOrig) 
		{
			Meth.NoCheckMiddleTierRole();
			if(sheet==null || sheet.PatNum==0) {
				return;//An invalid screening sheet was passed in.
			}
			List<string> listToothVals=new List<string>();
			List<string> listToothValsOld=new List<string>();
			//Process treatment planned sealants.
			for(int i=0;i<sheet.SheetFields.Count;i++) {//Go through the supplied sheet's fields and find the field.
				if(screenChartTypes.HasFlag(ScreenChartType.TP) 
					&& sheet.SheetFields[i].FieldType==SheetFieldType.ScreenChart 
					&& sheet.SheetFields[i].FieldName=="ChartSealantTreatment") 
				{
					listToothVals=sheet.SheetFields[i].FieldValue.Split(';').ToList();
					if(listToothVals[0]=="1") {//Primary tooth chart
						continue;//Skip primary tooth charts because we do not need to create any TP procedures for them.
					}
					listToothVals.RemoveAt(0);//Remove the toothchart type value
					if(listSheetFieldsChartOrig[0]!=null) {//Shouldn't be null if ChartSealantTreatment exists
						listToothValsOld=listSheetFieldsChartOrig[0].FieldValue.Split(';').ToList();
						listToothValsOld.RemoveAt(0);//Remove the toothchart type value
					}
					ScreenChartType screenChartType=ScreenChartType.TP;
					ProcessScreenChartHelper(sheet.PatNum,listToothVals,screenChartType,provNum,sheetNum,listToothValsOld);
					break;
				}
			}
			listToothVals=new List<string>();//Clear out the tooth values for the next tooth chart.
			//Process completed sealants.
			for(int i=0;i<sheet.SheetFields.Count;i++) {//Go through the supplied sheet's fields and find the field.
				if(screenChartTypes.HasFlag(ScreenChartType.C) 
					&& sheet.SheetFields[i].FieldType==SheetFieldType.ScreenChart 
					&& sheet.SheetFields[i].FieldName=="ChartSealantComplete") 
				{
					listToothVals=sheet.SheetFields[i].FieldValue.Split(';').ToList();
					if(listToothVals[0]=="1") {//Primary tooth chart
						continue;//Skip primary tooth charts because we do not need to create any TP procedures for them.
					}
					listToothVals.RemoveAt(0);//Remove the toothchart type value
					if(listSheetFieldsChartOrig[1]!=null) {//Shouldn't be null if ChartSealantTreatment exists
						listToothValsOld=listSheetFieldsChartOrig[1].FieldValue.Split(';').ToList();
						listToothValsOld.RemoveAt(0);//Remove the toothchart type value
					}
					ScreenChartType screenChartType=ScreenChartType.C;
					ProcessScreenChartHelper(sheet.PatNum,listToothVals,screenChartType,provNum,sheetNum,listToothValsOld);
					break;
				}
			}
			//Process if the user wants to TP fluoride and/or assessment procedures and/or other procedures.
			for(int i=0;i<sheet.SheetFields.Count;i++) {
				if(sheet.SheetFields[i].FieldType!=SheetFieldType.CheckBox) {
					continue;//Only care about check box types.
				}
				if(sheet.SheetFields[i].FieldName!="FluorideProc"
					&& sheet.SheetFields[i].FieldName!="AssessmentProc"
					&& !sheet.SheetFields[i].FieldName.StartsWith("Proc:"))
				{
					continue;//Field name must be one of the two hard coded values, or a FieldName that starts with "Proc".
				}
				//Make other proc with provNum and patNum
				SheetField sheetFieldOrig=listSheetFieldsProcOrig.FirstOrDefault(x => x.FieldName==sheet.SheetFields[i].FieldName && x.FieldType==SheetFieldType.CheckBox);
				if(sheetFieldOrig==null || sheetFieldOrig.FieldValue!="" || sheet.SheetFields[i].FieldValue!="X") {
					//Either not found or field was previously checked (already charted proc) or field is not checked (do not chart).
					continue;
				}
				string strProcCode="";
				switch(sheet.SheetFields[i].FieldName) {
					case "FluorideProc"://Original value was blank, new value is "checked", make the D1206 (fluoride) proc.
						strProcCode="D1206";
						break;
					case "AssessmentProc"://Original value was blank, new value is "checked", make the D0191 (assessment) proc.
						strProcCode="D0191";
						break;
					default://Original value was blank, new value is "checked", make the proc.
						strProcCode=sheet.SheetFields[i].FieldName.Substring(5);//Drop "Proc:" from FieldName.
						break;
				}
				Procedure proc=Procedures.CreateProcForPatNum(sheet.PatNum,ProcedureCodes.GetCodeNum(strProcCode),"","",ProcStat.C,provNum);
				if(proc!=null) {
					SecurityLogs.MakeLogEntry(EnumPermType.ProcEdit,sheet.PatNum,strProcCode+" "+Lans.g("Screens","treatment planned during screening."));
				}
			}
		}

		///<summary>Helper method so that we do not have to duplicate code.  The length of toothValues must match the length of chartOrigVals.</summary>
		private static void ProcessScreenChartHelper(long patNum,List<string> listToothValues,ScreenChartType screenChartType,long provNum,long sheetNum
			,List<string> listChartOrigVals) 
		{
			Meth.NoCheckMiddleTierRole();
			for(int i=0;i<listToothValues.Count;i++) {//toothValues is in the order from low to high tooth number in the chart
				if(!listToothValues[i].Contains("S")) {//No sealant, nothing to do.
					continue;
				}
				//Logic to determine if the "S" changed surfaces or was erased between the time the toothchart was opened and when it was submitted.
				List<string> listSurfacesNew=listToothValues[i].Split(',').ToList();
				List<string> listSurfacesOrig=listChartOrigVals[i].Split(',').ToList();
				bool isDiff=false;
				for(int j=0;j<listSurfacesOrig.Count;j++) {//Both arrays have the same length unless the chart doesn't exist in the original.
					if((listSurfacesNew[j]=="S" && listSurfacesOrig[j]!="S") 
						|| (listSurfacesNew[j]!="S" && listSurfacesOrig[j]=="S")) 
					{
						//"S" changed surfaces or was removed.
						isDiff=true;
						break;
					}
				}
				//If there is no difference don't make any duplicates.  We don't care if they changed a surface from N to PS for example, only S surfaces are important.
				if(!isDiff) {
					continue;//All the "S" surfaces are the same.
				}
				string surf="";
				int toothNum=0;
				bool isMolar=false;
				bool isRight=false;
				bool isLing=false;
				string tooth="";
				#region Parse ScreenChart FieldValues
				if(i<=1) {//Top left quadrant of toothchart
					toothNum=i+2;
					isMolar=true;
					isRight=true;
					isLing=true;
				}
				else if(i>1 && i<=3) {//Top middle-left quadrant of toothchart
					toothNum=i+2;
					isMolar=false;
					isRight=true;
					isLing=true;
				}
				else if(i>3 && i<=5) {//Top middle-right quadrant of toothchart
					toothNum=i+8;
					isMolar=false;
					isRight=false;
					isLing=true;
				}
				else if(i>5 && i<=7) {//Top right quadrant of toothchart
					toothNum=i+8;
					isMolar=true;
					isRight=false;
					isLing=true;
				}
				else if(i>7 && i<=9) {//Lower right quadrant of toothchart
					toothNum=i+10;
					isMolar=true;
					isRight=false;
					isLing=false;
				}
				else if(i>9 && i<=11) {//Lower middle-right quadrant of toothchart
					toothNum=i+10;
					isMolar=false;
					isRight=false;
					isLing=false;
				}
				else if(i>11 && i<=13) {//Lower middle-left quadrant of toothchart
					toothNum=i+16;
					isMolar=false;
					isRight=true;
					isLing=false;
				}
				else if(i>13) {//Lower left quadrant of toothchart
					toothNum=i+16;
					isMolar=true;
					isRight=true;
					isLing=false;
				}
				if(isMolar) {
					if(isRight) {
						if(listSurfacesNew[0]=="S") {
							surf+="D";
						}
						if(listSurfacesNew[1]=="S") {
							surf+="M";
						}
					}
					else {//Is Left side
						if(listSurfacesNew[0]=="S") {
							surf+="M";
						}
						if(listSurfacesNew[1]=="S") {
							surf+="D";
						}
					}
					if(isLing && listSurfacesNew[2]=="S") {
						surf+="L";
					}
					if(!isLing && listSurfacesNew[2]=="S") {
						surf+="B";
					}
				}
				else {//Front teeth, only look at 3rd surface position in control as that's the only one the user can see.
					if(listSurfacesNew[2]=="S") {
						surf="O";//NOTE: Not sure what surface to enter here... This is just a placeholder for now until we figure it out...
					}
				}
				if(toothNum!=0) {
					tooth=toothNum.ToString();
				}
				#endregion Parse Toothchart FieldValues
				surf=Tooth.SurfTidyForDisplay(surf,tooth);
				if(screenChartType==ScreenChartType.TP) {//Create TP'd sealant procs if they don't already exist for this patient.
					if(Procedures.GetProcForPatByToothSurfStat(patNum,toothNum,surf,ProcStat.TP)!=null) {
						continue;
					}
					Procedure procedure=Procedures.CreateProcForPatNum(patNum,ProcedureCodes.GetCodeNum("D1351"),surf,tooth,ProcStat.TP,provNum);
					if(procedure!=null) {
						SecurityLogs.MakeLogEntry(EnumPermType.ProcEdit,patNum,"D1351 "+Lans.g("Screens","treatment planned during screening with tooth")
							+" "+procedure.ToothNum.ToString()+" "+Lans.g("Screens","and surface")+" "+procedure.Surf);
					}
				}
				else if(screenChartType==ScreenChartType.C) {
					Procedure procedure=Procedures.GetProcForPatByToothSurfStat(patNum,toothNum,surf,ProcStat.TP);
					if(procedure==null) {//A TP procedure does not already exist.
						procedure=Procedures.CreateProcForPatNum(patNum,ProcedureCodes.GetCodeNum("D1351"),surf,tooth,ProcStat.C,provNum);
					}
					else {//TP proc already exists, set it complete.
						Procedure procOld=procedure.Copy();
						procedure.ProcStatus=ProcStat.C;
						procedure.DateEntryC=DateTime.Now;
						procedure.ProcDate=DateTime.Now;
						procedure.ProvNum=provNum;
						Procedures.Update(procedure,procOld);
					}
					if(procedure!=null) {
						SecurityLogs.MakeLogEntry(EnumPermType.ProcComplCreate,patNum,"D1351 "+Lans.g("Screens","set complete during screening with tooth")
							+" "+procedure.ToothNum.ToString()+" "+Lans.g("Screens","and surface")+" "+procedure.Surf);
					}
				}
			}
			if(screenChartType==ScreenChartType.C) {
				Recalls.Synch(patNum);
			}
		}

		///<summary>Gets all screens associated to the screen group passed in.</summary>
		public static List<Screen> GetScreensForGroup(long screenGroupNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Screen>>(MethodBase.GetCurrentMethod(),screenGroupNum);
			}
			string command="SELECT * FROM screen "
				+"WHERE ScreenGroupNum = '"+POut.Long(screenGroupNum)+"' "
				+"ORDER BY ScreenGroupOrder";
			return Crud.ScreenCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(OpenDentBusiness.Screen screen) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				screen.ScreenNum=Meth.GetLong(MethodBase.GetCurrentMethod(),screen);
				return screen.ScreenNum;
			}
			return Crud.ScreenCrud.Insert(screen);
		}

		///<summary></summary>
		public static void Update(OpenDentBusiness.Screen screen){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),screen);
				return;
			}
			Crud.ScreenCrud.Update(screen);
		}

		///<summary></summary>
		public static void Delete(OpenDentBusiness.Screen screen){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),screen);
				return;
			}
			string command = "DELETE from screen WHERE ScreenNum = '"+POut.Long(screen.ScreenNum)+"'";
			Db.NonQ(command);
		}

		///<summary>Deletes a Screen that has the attached sheetNum.  Deleting screen sheets are the same as deleting the screen itself.</summary>
		public static void DeleteForSheet(long sheetNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),sheetNum);
				return;
			}
			string command="DELETE FROM screen WHERE SheetNum="+POut.Long(sheetNum);
			Db.NonQ(command);
		}

		/// <summary>Deletes a list of Screens.</summary>
		public static void DeleteScreens(List<long> listScreenNums) {
			if(listScreenNums.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(), listScreenNums);
				return;
			}
			string command="DELETE FROM screen WHERE ScreenNum IN ("+string.Join(",",listScreenNums.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}
	}

}