using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness{
  ///<summary></summary>
	public class Screens {
		///<summary>Gets one Screen from the db.</summary>
		public static Screen GetOne(long screenNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Screen>(MethodBase.GetCurrentMethod(),screenNum);
			}
			return Crud.ScreenCrud.SelectOne(screenNum);
		}

		///<summary>After taking multiple screenings using a sheets, this method will import all sheets as screens and insert them into the db.
		///The goal of this method is that the office will fill out a bunch of sheets (in the web?).
		///Then after they get back to their office (with connection to their db) they will push a button to upload / insert a batch.</summary>
		public static List<Screen> CreateScreensFromSheets(List<Sheet> listSheets) {
			//No need to check RemotingRole; no call to db.
			List<Screen> listScreens=new List<Screen>();
			foreach(Sheet sheet in listSheets) {
				listScreens.Add(CreateScreenFromSheet(sheet));
			}
			return listScreens;
		}

		///<summary>After taking a screening using a sheet, this method will import the sheet as a screen and insert it into the db.
		///Returns null if the sheet passed in is not a Screening sheet type or if the sheet is missing the required ScreenGroupNum param.
		///Optionally supply a screen if you want to preset some values.  E.g. ScreenGroupOrder is often preset before calling this method.</summary>
		public static Screen CreateScreenFromSheet(Sheet sheet,Screen screen=null) {
			//No need to check RemotingRole; no call to db.
			//Make sure that the sheet passed in is a screening and contains the required ScreenGroupNum parameter.
			if(sheet.SheetType!=SheetTypeEnum.Screening || SheetParameter.GetParamByName(sheet.Parameters,"ScreenGroupNum")==null) {
				return null;
			}
			if(screen==null) {
				screen=new Screen();
				screen.ScreenGroupNum=(long)SheetParameter.GetParamByName(sheet.Parameters,"ScreenGroupNum").ParamValue;
			}
			screen.SheetNum=sheet.SheetNum;
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "Gender":
						if(field.FieldValue.Trim().ToLower().StartsWith("m")) {
							screen.Gender=PatientGender.Male;
						}
						else if(field.FieldValue.Trim().ToLower().StartsWith("f")) {
							screen.Gender=PatientGender.Female;
						}
						else {
							screen.Gender=PatientGender.Unknown;
						}
						break;
					case "Race/Ethnicity":
						PatientRaceOld patientRace=PatientRaceOld.Unknown;
						Enum.TryParse<PatientRaceOld>(field.FieldValue.Split(';')[0],out patientRace);
						screen.RaceOld=patientRace;
						break;
					case "GradeLevel":
						PatientGrade patientGrade=PatientGrade.Unknown;
						Enum.TryParse<PatientGrade>(field.FieldValue.Split(';')[0],out patientGrade);
						screen.GradeLevel=patientGrade;
						break;
					case "Age":
						if(screen.Age!=0) {
							break;//Already calculated via Birthdate.
						}
						byte age=0;
						byte.TryParse(field.FieldValue,out age);
						screen.Age=age;
						break;
					case "Urgency":
						TreatmentUrgency treatmentUrgency=TreatmentUrgency.Unknown;
						Enum.TryParse<TreatmentUrgency>(field.FieldValue.Split(';')[0],out treatmentUrgency);
						screen.Urgency=treatmentUrgency;
						break;
					case "ChartSealantTreatment":
						//Only mark "carious" if TP chart has C marked for any tooth surface.
						if(field.FieldValue.Contains("C")) {
							screen.HasCaries=YN.Yes;//Caries is present in TP'd chart.  Compl chart doesn't matter, it's only for sealant placement.
						}
						else {
							screen.HasCaries=YN.No;
						}
						//Only mark "needs sealants" if TP chart has S marked for any tooth surface.
						if(field.FieldValue.Contains("S")) {
							screen.NeedsSealants=YN.Yes;
						}
						else {
							screen.NeedsSealants=YN.No;
						}
						break;
					case "CariesExperience":
						screen.CariesExperience=field.FieldValue=="X" ? YN.Yes : YN.No;
						break;
					case "EarlyChildCaries":
						screen.EarlyChildCaries=field.FieldValue=="X" ? YN.Yes : YN.No;
						break;
					case "ExistingSealants":
						screen.ExistingSealants=field.FieldValue=="X" ? YN.Yes : YN.No;
						break;
					case "MissingAllTeeth":
						screen.MissingAllTeeth=field.FieldValue=="X" ? YN.Yes : YN.No;
						break;
					case "Birthdate":
						DateTime birthdate=new DateTime(1,1,1);
						DateTime.TryParse(field.FieldValue,out birthdate);
						screen.Birthdate=birthdate;
						//Check to see if the sheet has Age manually filled out.  
						//If Age was not manually set, automatically calculate the age based on the birthdate entered.
						//This matches screening functionality.
						SheetField sheetFieldAge=sheet.SheetFields.FirstOrDefault(x => x.FieldName=="Age");
						if(sheetFieldAge!=null && string.IsNullOrEmpty(sheetFieldAge.FieldValue)) {
							screen.Age=PIn.Byte(Patients.DateToAge(birthdate).ToString());
						}
						break;
					case "Comments":
						screen.Comments=field.FieldValue;
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
		///listProcOrigVals, nulls are allowed, the first represents the fluoride field, second is assessment field, all others are other procs.</summary>
		public static void ProcessScreenChart(Sheet sheet,ScreenChartType chartTypes,long provNum,long sheetNum,List<SheetField> listChartOrigVals
			,List<SheetField> listProcOrigVals) 
		{
			//No need to check RemotingRole; no call to db.
			if(sheet==null || sheet.PatNum==0) {
				return;//An invalid screening sheet was passed in.
			}
			List<string> listToothVals=new List<string>();
			List<string> listToothValsOld=new List<string>();
			//Process treatment planned sealants.
			foreach(SheetField field in sheet.SheetFields) {//Go through the supplied sheet's fields and find the field.
				if(chartTypes.HasFlag(ScreenChartType.TP) && field.FieldType==SheetFieldType.ScreenChart && field.FieldName=="ChartSealantTreatment") {
					listToothVals=field.FieldValue.Split(';').ToList();
					if(listToothVals[0]=="1") {//Primary tooth chart
						continue;//Skip primary tooth charts because we do not need to create any TP procedures for them.
					}
					listToothVals.RemoveAt(0);//Remove the toothchart type value
					if(listChartOrigVals[0]!=null) {//Shouldn't be null if ChartSealantTreatment exists
						listToothValsOld=listChartOrigVals[0].FieldValue.Split(';').ToList();
						listToothValsOld.RemoveAt(0);//Remove the toothchart type value
					}
					ScreenChartType chartType=ScreenChartType.TP;
					ProcessScreenChartHelper(sheet.PatNum,listToothVals,chartType,provNum,sheetNum,listToothValsOld);
					break;
				}
			}
			listToothVals=new List<string>();//Clear out the tooth values for the next tooth chart.
			//Process completed sealants.
			foreach(SheetField field in sheet.SheetFields) {//Go through the supplied sheet's fields and find the field.
				if(chartTypes.HasFlag(ScreenChartType.C) && field.FieldType==SheetFieldType.ScreenChart && field.FieldName=="ChartSealantComplete") {
					listToothVals=field.FieldValue.Split(';').ToList();
					if(listToothVals[0]=="1") {//Primary tooth chart
						continue;//Skip primary tooth charts because we do not need to create any TP procedures for them.
					}
					listToothVals.RemoveAt(0);//Remove the toothchart type value
					if(listChartOrigVals[1]!=null) {//Shouldn't be null if ChartSealantTreatment exists
						listToothValsOld=listChartOrigVals[1].FieldValue.Split(';').ToList();
						listToothValsOld.RemoveAt(0);//Remove the toothchart type value
					}
					ScreenChartType chartType=ScreenChartType.C;
					ProcessScreenChartHelper(sheet.PatNum,listToothVals,chartType,provNum,sheetNum,listToothValsOld);
					break;
				}
			}
			//Process if the user wants to TP fluoride and/or assessment procedures and/or other procedures.
			foreach(SheetField field in sheet.SheetFields) {
				if(field.FieldType!=SheetFieldType.CheckBox) {
					continue;//Only care about check box types.
				}
				if(field.FieldName!="FluorideProc" && field.FieldName!="AssessmentProc" && !field.FieldName.StartsWith("Proc:")) {
					continue;//Field name must be one of the two hard coded values, or a FieldName that starts with "Proc".
				}
				//Make other proc with provNum and patNum
				SheetField sheetFieldOrig=listProcOrigVals.FirstOrDefault(x => x.FieldName==field.FieldName && x.FieldType==SheetFieldType.CheckBox);
				if(sheetFieldOrig==null || sheetFieldOrig.FieldValue!="" || field.FieldValue!="X") {
					//Either not found or field was previously checked (already charted proc) or field is not checked (do not chart).
					continue;
				}
				string strProcCode="";
				switch(field.FieldName) {
					case "FluorideProc"://Original value was blank, new value is "checked", make the D1206 (fluoride) proc.
						strProcCode="D1206";
						break;
					case "AssessmentProc"://Original value was blank, new value is "checked", make the D0191 (assessment) proc.
						strProcCode="D0191";
						break;
					default://Original value was blank, new value is "checked", make the proc.
						strProcCode=field.FieldName.Substring(5);//Drop "Proc:" from FieldName.
						break;
				}
				Procedure proc=Procedures.CreateProcForPatNum(sheet.PatNum,ProcedureCodes.GetCodeNum(strProcCode),"","",ProcStat.C,provNum);
				if(proc!=null) {
					SecurityLogs.MakeLogEntry(Permissions.ProcEdit,sheet.PatNum,strProcCode+" "+Lans.g("Screens","treatment planned during screening."));
				}
			}
		}

		///<summary>Helper method so that we do not have to duplicate code.  The length of toothValues must match the length of chartOrigVals.</summary>
		private static void ProcessScreenChartHelper(long patNum,List<string> toothValues,ScreenChartType chartType,long provNum,long sheetNum
			,List<string> chartOrigVals) 
		{
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<toothValues.Count;i++) {//toothValues is in the order from low to high tooth number in the chart
				if(!toothValues[i].Contains("S")) {//No sealant, nothing to do.
					continue;
				}
				//Logic to determine if the "S" changed surfaces or was erased between the time the toothchart was opened and when it was submitted.
				string[] newSurfaces=toothValues[i].Split(',');
				string[] origSurfaces=chartOrigVals[i].Split(',');
				bool isDiff=false;
				for(int j=0;j<origSurfaces.Length;j++) {//Both arrays have the same length unless the chart doesn't exist in the original.
					if((newSurfaces[j]=="S" && origSurfaces[j]!="S") || (newSurfaces[j]!="S" && origSurfaces[j]=="S")) {//"S" changed surfaces or was removed.
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
						if(newSurfaces[0]=="S") {
							surf+="D";
						}
						if(newSurfaces[1]=="S") {
							surf+="M";
						}
					}
					else {//Is Left side
						if(newSurfaces[0]=="S") {
							surf+="M";
						}
						if(newSurfaces[1]=="S") {
							surf+="D";
						}
					}
					if(isLing && newSurfaces[2]=="S") {
						surf+="L";
					}
					if(!isLing && newSurfaces[2]=="S") {
						surf+="B";
					}
				}
				else {//Front teeth, only look at 3rd surface position in control as that's the only one the user can see.
					if(newSurfaces[2]=="S") {
						surf="O";//NOTE: Not sure what surface to enter here... This is just a placeholder for now until we figure it out...
					}
				}
				if(toothNum!=0) {
					tooth=toothNum.ToString();
				}
				#endregion Parse Toothchart FieldValues
				surf=Tooth.SurfTidyForDisplay(surf,tooth);
				if(chartType==ScreenChartType.TP) {//Create TP'd sealant procs if they don't already exist for this patient.
					if(Procedures.GetProcForPatByToothSurfStat(patNum,toothNum,surf,ProcStat.TP)!=null) {
						continue;
					}
					Procedure proc=Procedures.CreateProcForPatNum(patNum,ProcedureCodes.GetCodeNum("D1351"),surf,tooth,ProcStat.TP,provNum);
					if(proc!=null) {
						SecurityLogs.MakeLogEntry(Permissions.ProcEdit,patNum,"D1351 "+Lans.g("Screens","treatment planned during screening with tooth")
							+" "+proc.ToothNum.ToString()+" "+Lans.g("Screens","and surface")+" "+proc.Surf);
					}
				}
				else if(chartType==ScreenChartType.C) {
					Procedure proc=Procedures.GetProcForPatByToothSurfStat(patNum,toothNum,surf,ProcStat.TP);
					if(proc==null) {//A TP procedure does not already exist.
						proc=Procedures.CreateProcForPatNum(patNum,ProcedureCodes.GetCodeNum("D1351"),surf,tooth,ProcStat.C,provNum);
					}
					else {//TP proc already exists, set it complete.
						Procedure procOld=proc.Copy();
						proc.ProcStatus=ProcStat.C;
						proc.DateEntryC=DateTime.Now;
						Procedures.Update(proc,procOld);
					}
					if(proc!=null) {
						SecurityLogs.MakeLogEntry(Permissions.ProcComplCreate,patNum,"D1351 "+Lans.g("Screens","set complete during screening with tooth")
							+" "+proc.ToothNum.ToString()+" "+Lans.g("Screens","and surface")+" "+proc.Surf);
					}
				}
			}
			if(chartType==ScreenChartType.C) {
				Recalls.Synch(patNum);
			}
		}

		///<summary>Gets all screens associated to the screen group passed in.</summary>
		public static List<Screen> GetScreensForGroup(long screenGroupNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Screen>>(MethodBase.GetCurrentMethod(),screenGroupNum);
			}
			string command="SELECT * FROM screen "
				+"WHERE ScreenGroupNum = '"+POut.Long(screenGroupNum)+"' "
				+"ORDER BY ScreenGroupOrder";
			return Crud.ScreenCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(OpenDentBusiness.Screen Cur) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Cur.ScreenNum=Meth.GetLong(MethodBase.GetCurrentMethod(),Cur);
				return Cur.ScreenNum;
			}
			return Crud.ScreenCrud.Insert(Cur);
		}

		///<summary></summary>
		public static void Update(OpenDentBusiness.Screen Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			Crud.ScreenCrud.Update(Cur);
		}

		///<summary></summary>
		public static void Delete(OpenDentBusiness.Screen Cur){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),Cur);
				return;
			}
			string command = "DELETE from screen WHERE ScreenNum = '"+POut.Long(Cur.ScreenNum)+"'";
			Db.NonQ(command);
		}

		///<summary>Deletes a Screen that has the attached sheetNum.  Deleting screen sheets are the same as deleting the screen itself.</summary>
		public static void DeleteForSheet(long sheetNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),sheetNum);
				return;
			}
			string command="DELETE FROM screen WHERE SheetNum="+POut.Long(sheetNum);
			Db.NonQ(command);
		}
	}

}













