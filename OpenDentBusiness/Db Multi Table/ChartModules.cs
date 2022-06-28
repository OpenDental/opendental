using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using CodeBase;
using DataConnectionBase;
using Newtonsoft.Json;

namespace OpenDentBusiness {
	public class ChartModules {
		private static DataTable rawApt;

		///<summary>Returns a DataTable with the contents of the progress notes grid.</summary>
		public static DataTable GetProgNotes(long patNum,bool isAuditMode,ChartModuleFilters componentsToLoad=null) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum,isAuditMode,componentsToLoad);
			}
			componentsToLoad=componentsToLoad??new ChartModuleFilters();
			DataConnection dcon=new DataConnection();
			DataTable table=new DataTable("ProgNotes");
			DataRow row;
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("aptDateTime",typeof(DateTime));
			table.Columns.Add("AbbrDesc");
			table.Columns.Add("AptNum");
			table.Columns.Add("clinic");
			table.Columns.Add("ClinicNum");
			table.Columns.Add("CodeNum");
			table.Columns.Add("colorBackG");
			table.Columns.Add("colorText");
			table.Columns.Add("CommlogNum");
			table.Columns.Add("CommSource");
			table.Columns.Add("commType");
			table.Columns.Add("dateEntryC");
			table.Columns.Add("DateEntryC");
			table.Columns.Add("dateTP");
			table.Columns.Add("DateTP");
			table.Columns.Add("description");
			table.Columns.Add("DocNum");
			table.Columns.Add("dx");
			table.Columns.Add("Dx");
			table.Columns.Add("EmailMessageHideIn");
			table.Columns.Add("EmailMessageHtmlType");
			table.Columns.Add("EmailMessageNum");
			table.Columns.Add("FormPatNum");
			table.Columns.Add("HideGraphics");
			table.Columns.Add("hl7Sent");
			table.Columns.Add("isLocked");
			table.Columns.Add("length");
			table.Columns.Add("LabCaseNum");
			table.Columns.Add("note");
			table.Columns.Add("orionDateScheduleBy");
			table.Columns.Add("orionDateStopClock");
			table.Columns.Add("orionDPC");
			table.Columns.Add("orionDPCpost");
			table.Columns.Add("orionIsEffectiveComm");
			table.Columns.Add("orionIsOnCall");
			table.Columns.Add("orionStatus2");
			table.Columns.Add("PatNum");//only used for Commlog and Task
			table.Columns.Add("Priority");//for sorting
			table.Columns.Add("priority");
			table.Columns.Add("ProcCode");
			table.Columns.Add("procDate");
			table.Columns.Add("ProcDate",typeof(DateTime));
			table.Columns.Add("procFee");
			table.Columns.Add("ProcNum");
			table.Columns.Add("ProcNumLab");
			table.Columns.Add("procStatus");
			table.Columns.Add("ProcStatus");
			table.Columns.Add("procTime");
			table.Columns.Add("procTimeEnd");
			table.Columns.Add("prognosis");
			table.Columns.Add("prov");
			table.Columns.Add("ProvNum");
			table.Columns.Add("quadrant");
			table.Columns.Add("RxNum");
			table.Columns.Add("RxType");//Used to determine if a program link was accessed for pdmp
			table.Columns.Add("SheetNum");
			table.Columns.Add("signature");
			table.Columns.Add("Surf");
			table.Columns.Add("surf");
			table.Columns.Add("TaskNum");
			table.Columns.Add("toothNum");
			table.Columns.Add("ToothNum");
			table.Columns.Add("ToothRange");
			table.Columns.Add("user");
			table.Columns.Add("WebChatSessionNum");
			//table.Columns.Add("");
			//but we won't actually fill this table with rows until the very end.  It's more useful to use a List<> for now.
			List<DataRow> rows=new List<DataRow>();
			string command;
			DateTime dateT;
			string txt;
			List<DataRow> labRows=new List<DataRow>();//Canadian lab procs, which must be added in a loop at the very end.
			List<Def> listMiscColorDefs=Defs.GetDefsForCategory(DefCat.MiscColors);
			List<Def> listProgNoteColorDefs=Defs.GetDefsForCategory(DefCat.ProgNoteColors);
			Family family=Patients.GetFamily(patNum);
			Dictionary<string,string> dictUserNames=Userods.GetUsers().ToDictionary(x => x.UserNum.ToString(),x => x.UserName);
			if(componentsToLoad.ShowTreatPlan
				|| componentsToLoad.ShowCompleted
				|| componentsToLoad.ShowExisting
				|| componentsToLoad.ShowReferred
				|| componentsToLoad.ShowConditions)
			{
				#region Procedures
				command="SELECT provider.Abbr,procedurecode.AbbrDesc,appointment.AptDateTime,procedurelog.BaseUnits,procedurelog.ClinicNum,"
				+"procedurelog.CodeNum,procedurelog.DateEntryC,orionproc.DateScheduleBy,orionproc.DateStopClock,procedurelog.DateTP,"
				+"procedurecode.Descript,orionproc.DPC,orionproc.DPCpost,Dx,HideGraphics,"
				+"(SELECT MAX(HL7ProcAttachNum)	FROM hl7procattach WHERE hl7procattach.ProcNum=procedurelog.ProcNum) HL7ProcAttachNum,"
				+"orionproc.IsEffectiveComm,IsLocked,"
				+"orionproc.IsOnCall,LaymanTerm,procedurelog.Priority,procedurecode.ProcCode,ProcDate,ProcFee,procedurelog.ProcNum,ProcNumLab,procedurelog.ProcTime,"
				+"procedurelog.ProcTimeEnd,procedurelog.Prognosis,provider.ProvNum,ProcStatus,orionproc.Status2,Surf,ToothNum,ToothRange,UnitQty "
				+"FROM procedurelog "
				+"LEFT JOIN procedurecode ON procedurecode.CodeNum=procedurelog.CodeNum "
				+"LEFT JOIN provider ON provider.ProvNum=procedurelog.ProvNum "
				+"LEFT JOIN orionproc ON procedurelog.ProcNum=orionproc.ProcNum "
				+"LEFT JOIN appointment ON appointment.AptNum=procedurelog.AptNum "
				+"AND (appointment.AptStatus="+POut.Long((int)ApptStatus.Scheduled)
				+" OR appointment.AptStatus="+POut.Long((int)ApptStatus.Broken)
				+" OR appointment.AptStatus="+POut.Long((int)ApptStatus.Complete)
				+") WHERE procedurelog.PatNum="+POut.Long(patNum);
				if(!isAuditMode) {//regular mode
					command+=" AND (ProcStatus !=6"//not deleted
						+" OR IsLocked=1)";//Any locked proc should show.  This forces invalidated (deleted locked) procs to show.
				}
				command+=" ORDER BY ProcDate";//we'll just have to reorder it anyway
				DataTable rawProcs=dcon.GetTable(command);
				command="SELECT ProcNum,EntryDateTime,UserNum,Note,"
				+"CASE WHEN Signature!='' THEN 1 ELSE 0 END AS SigPresent "
				+"FROM procnote WHERE PatNum="+POut.Long(patNum)
				+" ORDER BY EntryDateTime";// but this helps when looping for notes
				DataTable rawNotes=dcon.GetTable(command);
				Dictionary<string,List<DataRow>> dictNotes=rawNotes.Select().GroupBy(x => x["ProcNum"].ToString())
					.ToDictionary(x => x.Key,x => x.OrderByDescending(y => PIn.DateT(y["EntryDateTime"].ToString())).ToList());
				Dictionary<string,ProcedureCode> dictProcCodes=ProcedureCodes.GetAllCodes().GroupBy(x => x.CodeNum)
					.ToDictionary(x => x.Key.ToString(),x => x.First());
				foreach(DataRow rowProc in rawProcs.Rows) {
					ProcedureCode procCode;
					if(!dictProcCodes.TryGetValue(rowProc["CodeNum"].ToString(),out procCode)) {
						procCode=ProcedureCodes.GetProcCode(PIn.Long(rowProc["CodeNum"].ToString()));
					}
					row=table.NewRow();
					row["AbbrDesc"]=rowProc["AbbrDesc"].ToString();
					row["aptDateTime"]=PIn.DateT(rowProc["AptDateTime"].ToString());
					row["AptNum"]=0;
					row["clinic"]=Clinics.GetDesc(PIn.Long(rowProc["ClinicNum"].ToString()));
					row["ClinicNum"]=PIn.Long(rowProc["ClinicNum"].ToString());
					row["CodeNum"]=rowProc["CodeNum"].ToString();
					row["colorBackG"]=Color.White.ToArgb();
					if(((DateTime)row["aptDateTime"]).Date==DateTime.Today) {
						row["colorBackG"]=listMiscColorDefs[(int)DefCatMiscColors.ChartTodaysProcs].ItemColor.ToArgb().ToString();
					}
					switch((ProcStat)PIn.Long(rowProc["ProcStatus"].ToString())) {
						case ProcStat.TP:
						case ProcStat.TPi:
							row["colorText"]=listProgNoteColorDefs[0].ItemColor.ToArgb().ToString();
							break;
						case ProcStat.C:
							row["colorText"]=listProgNoteColorDefs[1].ItemColor.ToArgb().ToString();
							break;
						case ProcStat.EC:
							row["colorText"]=listProgNoteColorDefs[2].ItemColor.ToArgb().ToString();
							break;
						case ProcStat.EO:
							row["colorText"]=listProgNoteColorDefs[3].ItemColor.ToArgb().ToString();
							break;
						case ProcStat.R:
							row["colorText"]=listProgNoteColorDefs[4].ItemColor.ToArgb().ToString();
							break;
						case ProcStat.D:
							row["colorText"]=Color.Black.ToArgb().ToString();
							break;
						case ProcStat.Cn:
							row["colorText"]=listProgNoteColorDefs[22].ItemColor.ToArgb().ToString();
							break;
					}
					row["CommlogNum"]=0;
					row["CommSource"]="";
					row["commType"]="";
					dateT=PIn.DateT(rowProc["DateEntryC"].ToString());
					if(dateT.Year<1880) {
						row["dateEntryC"]="";
					}
					else {
						row["dateEntryC"]=dateT.ToString(Lans.GetShortDateTimeFormat());
					}
					row["DateEntryC"]=dateT.ToShortDateString();
					dateT=PIn.DateT(rowProc["DateTP"].ToString());
					if(dateT.Year<1880) {
						row["dateTP"]="";
					}
					else {
						row["dateTP"]=dateT.ToString(Lans.GetShortDateTimeFormat());
					}
					row["DateTP"]=dateT.ToShortDateString();
					if(rowProc["LaymanTerm"].ToString()=="") {
						row["description"]=rowProc["Descript"].ToString();
					}
					else {
						row["description"]=rowProc["LaymanTerm"].ToString();
					}
					if(rowProc["ToothRange"].ToString()!="") {
						row["description"]+=" #"+Tooth.DisplayRange(rowProc["ToothRange"].ToString());
					}
					row["DocNum"]=0;
					row["dx"]=Defs.GetValue(DefCat.Diagnosis,PIn.Long(rowProc["Dx"].ToString()));
					row["Dx"]=rowProc["Dx"].ToString();
					row["EmailMessageNum"]=0;
					row["FormPatNum"]=0;
					row["HideGraphics"]=rowProc["HideGraphics"].ToString();
					if(rowProc["HL7ProcAttachNum"].ToString()=="") {
						row["hl7Sent"]="";
					}
					else {
						row["hl7Sent"]="X";
					}
					row["isLocked"]=PIn.Bool(rowProc["isLocked"].ToString())?"X":"";
					row["LabCaseNum"]=0;
					row["length"]="";
					row["signature"]="";
					row["user"]="";
					List<DataRow> tableRawNotesForProc;
					dictNotes.TryGetValue(rowProc["ProcNum"].ToString(),out tableRawNotesForProc);
					if(componentsToLoad.ShowProcNotes) {
						#region note-----------------------------------------------------------------------------------------------------------
						row["note"]="";
						dateT=PIn.DateT(rowProc["DateScheduleBy"].ToString());
						if(dateT.Year<1880) {
							row["orionDateScheduleBy"]="";
						}
						else {
							row["orionDateScheduleBy"]=dateT.ToString(Lans.GetShortDateTimeFormat());
						}
						dateT=PIn.DateT(rowProc["DateStopClock"].ToString());
						if(dateT.Year<1880) {
							row["orionDateStopClock"]="";
						}
						else {
							row["orionDateStopClock"]=dateT.ToString(Lans.GetShortDateTimeFormat());
						}
						if(((OrionDPC)PIn.Int(rowProc["DPC"].ToString())).ToString()=="NotSpecified") {
							row["orionDPC"]="";
						}
						else {
							row["orionDPC"]=((OrionDPC)PIn.Int(rowProc["DPC"].ToString())).ToString();
						}
						if(((OrionDPC)PIn.Int(rowProc["DPCpost"].ToString())).ToString()=="NotSpecified") {
							row["orionDPCpost"]="";
						}
						else {
							row["orionDPCpost"]=((OrionDPC)PIn.Int(rowProc["DPCpost"].ToString())).ToString();
						}
						row["orionIsEffectiveComm"]="";
						if(rowProc["IsEffectiveComm"].ToString()=="1") {
							row["orionIsEffectiveComm"]="Y";
						}
						else if(rowProc["IsEffectiveComm"].ToString()=="0") {
							row["orionIsEffectiveComm"]="";
						}
						row["orionIsOnCall"]="";
						if(rowProc["IsOnCall"].ToString()=="1") {
							row["orionIsOnCall"]="Y";
						}
						else if(rowProc["IsOnCall"].ToString()=="0") {
							row["orionIsOnCall"]="";
						}
						row["orionStatus2"]=((OrionStatus)PIn.Int(rowProc["Status2"].ToString())).ToString();
						if(tableRawNotesForProc!=null && tableRawNotesForProc.Count>0) {
							if(isAuditMode) {//we will include all notes for each proc.  We will concat and make readable.
								foreach(DataRow rowCur in tableRawNotesForProc) {
									if(row["note"].ToString()!="") {//if there is an existing note
										row["note"]+="\r\n------------------------------------------------------\r\n";//start a new line
									}
									row["note"]+=PIn.DateT(rowCur["EntryDateTime"].ToString()).ToString();
									string userName;
									if(!dictUserNames.TryGetValue(rowCur["UserNum"].ToString(),out userName)) {
										userName=Userods.GetName(PIn.Long(rowCur["UserNum"].ToString()));
									}
									row["note"]+=string.IsNullOrEmpty(userName)?"":("  "+userName);
									if(rowCur["SigPresent"].ToString()=="1") {
										row["note"]+="  "+Lans.g("ChartModule","(signed)");
									}
									row["note"]+="\r\n"+rowCur["Note"].ToString();
								}
							}
							else {//Not audit mode.  We just want the most recent note
								row["note"]=tableRawNotesForProc[0]["Note"].ToString();
							}
						}
						#endregion Note
					}
					//This section is closely related to notes, but must be filled for all procedures regardless of whether showing the actual note.
					if(!isAuditMode && tableRawNotesForProc!=null && tableRawNotesForProc.Count>0) {//Audit mode is handled above by putting this info into the note section itself.
						DataRow noteRowCur=tableRawNotesForProc[0];
						string userName;
						if(!dictUserNames.TryGetValue(noteRowCur["UserNum"].ToString(),out userName)) {
							userName=Userods.GetName(PIn.Long(noteRowCur["UserNum"].ToString()));
						}
						row["user"]=userName;
						row["signature"]=(noteRowCur["SigPresent"].ToString()=="1")?Lans.g("ChartModule","Signed"):"";
					}
					row["PatNum"]="";
					row["Priority"]=rowProc["Priority"].ToString();
					row["priority"]=Defs.GetName(DefCat.TxPriorities,PIn.Long(rowProc["Priority"].ToString()));
					row["ProcCode"]=rowProc["ProcCode"].ToString();
					dateT=PIn.DateT(rowProc["ProcDate"].ToString());
					if(dateT.Year<1880) {
						row["procDate"]="";
					}
					else {
						row["procDate"]=dateT.ToString(Lans.GetShortDateTimeFormat());
					}
					row["ProcDate"]=dateT;
					double amt = PIn.Double(rowProc["ProcFee"].ToString());
					int qty = PIn.Int(rowProc["UnitQty"].ToString()) + PIn.Int(rowProc["BaseUnits"].ToString());
					if(qty>0) {
						amt *= qty;
					}
					row["procFee"]=amt.ToString("F");
					row["ProcNum"]=rowProc["ProcNum"].ToString();
					row["ProcNumLab"]=rowProc["ProcNumLab"].ToString();
					row["procStatus"]=Lans.g("enumProcStat",((ProcStat)PIn.Long(rowProc["ProcStatus"].ToString())).ToString());
					if(row["procStatus"].ToString()=="D") {
						if(row["isLocked"].ToString()=="X") {
							row["procStatus"]=Lans.g("enumProcStat",ProcStatExt.Invalid);
							row["description"]=Lans.g("ChartModule","-invalid-")+" "+row["description"].ToString();
						}
					}
					row["ProcStatus"]=rowProc["ProcStatus"].ToString();
					row["procTime"]="";
					dateT=PIn.DateT(rowProc["ProcTime"].ToString());
					if(dateT.TimeOfDay!=TimeSpan.Zero) {
						row["procTime"]=dateT.ToString("h:mm")+dateT.ToString("%t").ToLower();
					}
					row["procTimeEnd"]="";
					dateT=PIn.DateT(rowProc["ProcTimeEnd"].ToString());
					if(dateT.TimeOfDay!=TimeSpan.Zero) {
						row["procTimeEnd"]=dateT.ToString("h:mm")+dateT.ToString("%t").ToLower();
					}
					row["prognosis"]=Defs.GetName(DefCat.Prognosis,PIn.Long(rowProc["Prognosis"].ToString()));
					row["prov"]=rowProc["Abbr"].ToString();
					row["ProvNum"]=rowProc["ProvNum"].ToString();
					row["quadrant"]="";
					if(procCode.TreatArea==TreatmentArea.Tooth) {
						row["quadrant"]=Tooth.GetQuadrant(rowProc["ToothNum"].ToString());
					}
					else if(procCode.TreatArea==TreatmentArea.Surf) {
						row["quadrant"]=Tooth.GetQuadrant(rowProc["ToothNum"].ToString());
					}
					else if(procCode.TreatArea==TreatmentArea.Quad) {
						row["quadrant"]=rowProc["Surf"].ToString();
					}
					else if(procCode.TreatArea==TreatmentArea.ToothRange) {
						string[] toothNum=rowProc["ToothRange"].ToString().Split(',');
						bool sameQuad=false;//Don't want true if length==0.
						for(int n=0;n<toothNum.Length;n++) {//But want true if length==1 (check index 0 against itself).
							if(Tooth.GetQuadrant(toothNum[n])==Tooth.GetQuadrant(toothNum[0])) {
								sameQuad=true;
							}
							else {
								sameQuad=false;
								break;
							}
						}
						if(sameQuad) {
							row["quadrant"]=Tooth.GetQuadrant(toothNum[0]);
						}
					}
					row["RxNum"]=0;
					row["SheetNum"]=0;
					row["Surf"]=rowProc["Surf"].ToString();
					if(procCode.TreatArea==TreatmentArea.Surf) {
						row["surf"]=Tooth.SurfTidyFromDbToDisplay(rowProc["Surf"].ToString(),rowProc["ToothNum"].ToString());
					}
					else if(procCode.TreatArea==TreatmentArea.Sextant) {
						row["surf"]=Tooth.GetSextant(rowProc["Surf"].ToString(),
							(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
					}
					else {
						row["surf"]=rowProc["Surf"].ToString();
					}
					row["TaskNum"]=0;
					row["toothNum"]=Tooth.Display(rowProc["ToothNum"].ToString()
						,(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
					row["ToothNum"]=rowProc["ToothNum"].ToString();
					row["ToothRange"]=rowProc["ToothRange"].ToString();
					if(rowProc["ProcNumLab"].ToString()=="0") {//normal proc
						rows.Add(row);
					}
					else {
						row["description"]="^ ^ "+row["description"].ToString();
						labRows.Add(row);//these will be added in the loop at the end
					}
					row["WebChatSessionNum"]=0;
					row["EmailMessageHideIn"]="0";
					row["EmailMessageHtmlType"]="0";
				}
				#endregion Procedures
			}
			if(componentsToLoad.ShowCommLog) {
				#region Commlog
				List<Def> listCommLogTypeDefs=Defs.GetDefsForCategory(DefCat.CommLogTypes);
				long podiumProgramNum=Programs.GetCur(ProgramName.Podium).ProgramNum;
				bool showPodiumCommlogs=PIn.Bool(ProgramProperties.GetPropVal(podiumProgramNum,Podium.PropertyDescs.ShowCommlogsInChartAndAccount));
				string wherePodiumCommlog="";
				if(!showPodiumCommlogs) {
					wherePodiumCommlog="AND (commlog.CommSource!="+POut.Int((int)CommItemSource.ProgramLink)
						+" OR (commlog.CommSource="+POut.Int((int)CommItemSource.ProgramLink)+" AND commlog.ProgramNum!="+POut.Long(podiumProgramNum)+")) ";
				}
				string whereFamilyCommLog="AND p1.PatNum=p2.PatNum ";
				if(componentsToLoad.ShowSuperFamilyCommLog) {
					whereFamilyCommLog="AND (p1.Guarantor=p2.Guarantor OR (p1.SuperFamily>0 AND p1.SuperFamily=p2.SuperFamily)) ";
				}
				else if(componentsToLoad.ShowFamilyCommLog) {
					whereFamilyCommLog="AND p1.Guarantor=p2.Guarantor ";
				}
				command="SELECT CommlogNum,CommDateTime,commlog.DateTimeEnd,CommType,Note,commlog.PatNum,UserNum,p1.FName,p1.LName,CommSource,"
				+"CASE WHEN Signature!='' THEN 1 ELSE 0 END SigPresent "
				+"FROM patient p1,patient p2,commlog "
				+"WHERE commlog.PatNum=p1.PatNum "
				+whereFamilyCommLog
				+"AND p2.PatNum="+POut.Long(patNum)+" "
				+wherePodiumCommlog
				+"ORDER BY CommDateTime";
				DataTable rawComm=dcon.GetTable(command);
				for(int i=0;i<rawComm.Rows.Count;i++) {
					row=table.NewRow();
					row["AbbrDesc"]="";
					row["aptDateTime"]=DateTime.MinValue;
					row["AptNum"]=0;
					row["clinic"]="";
					row["ClinicNum"]=0;
					row["CodeNum"]="";
					row["colorBackG"]=Color.White.ToArgb();
					long commTypeDefNum=PIn.Long(rawComm.Rows[i]["CommType"].ToString());
					Def commlogType=listCommLogTypeDefs.FirstOrDefault(x => x.DefNum==commTypeDefNum);
					if(commlogType!=null && commlogType.ItemColor.ToArgb()!=Color.Empty.ToArgb()) {//Def exists and not an empty color.
						row["colorText"]=commlogType.ItemColor.ToArgb().ToString();
					}
					else {//Otherwise use default color for prog notes
						row["colorText"]=listProgNoteColorDefs[6].ItemColor.ToArgb().ToString();
					}
					row["CommlogNum"]=rawComm.Rows[i]["CommlogNum"].ToString();
					row["CommSource"]=rawComm.Rows[i]["CommSource"].ToString();
					Def defCur=Defs.GetDef(DefCat.CommLogTypes,commTypeDefNum,listCommLogTypeDefs);
					string commType=defCur==null?"":defCur.ItemValue;
					row["commType"]=commType;
					row["dateEntryC"]="";
					row["dateTP"]="";
					if(rawComm.Rows[i]["PatNum"].ToString()==patNum.ToString()) {
						txt="";
					}
					else {// if first name is not present, show last name. 
						if(rawComm.Rows[i]["FName"].ToString()=="") {
							txt="("+rawComm.Rows[i]["LName"].ToString()+") ";
						}
						else {
							txt="("+rawComm.Rows[i]["FName"].ToString()+") ";
						}
					}
					row["description"]=txt+Lans.g("ChartModule","Comm - ")+Defs.GetName(DefCat.CommLogTypes,commTypeDefNum,listCommLogTypeDefs);
					row["DocNum"]=0;
					row["dx"]="";
					row["Dx"]="";
					row["EmailMessageNum"]=0;
					row["FormPatNum"]=0;
					row["HideGraphics"]="";
					row["isLocked"]="";
					row["LabCaseNum"]=0;
					row["length"]="";
					if(PIn.DateT(rawComm.Rows[i]["DateTimeEnd"].ToString()).Year>1880) {
						DateTime startTime=PIn.DateT(rawComm.Rows[i]["CommDateTime"].ToString());
						DateTime endTime=PIn.DateT(rawComm.Rows[i]["DateTimeEnd"].ToString());
						row["length"]=(endTime-startTime).ToStringHmm();
					}
					row["note"]=rawComm.Rows[i]["Note"].ToString();
					row["orionDateScheduleBy"]="";
					row["orionDateStopClock"]="";
					row["orionDPC"]="";
					row["orionDPCpost"]="";
					row["orionIsEffectiveComm"]="";
					row["orionIsOnCall"]="";
					row["orionStatus2"]="";
					row["PatNum"]=rawComm.Rows[i]["PatNum"].ToString();
					row["Priority"]="";
					row["priority"]="";
					row["ProcCode"]="";
					dateT=PIn.DateT(rawComm.Rows[i]["CommDateTime"].ToString());
					if(dateT.Year<1880) {
						row["procDate"]="";
					}
					else {
						row["procDate"]=dateT.ToString(Lans.GetShortDateTimeFormat());
					}
					row["ProcDate"]=dateT;
					row["procTime"]="";
					if(dateT.TimeOfDay!=TimeSpan.Zero) {
						row["procTime"]=dateT.ToString("h:mm")+dateT.ToString("%t").ToLower();
					}
					row["procTimeEnd"]="";
					row["procFee"]="";
					row["ProcNum"]=0;
					row["ProcNumLab"]="";
					row["procStatus"]="";
					row["ProcStatus"]="";
					row["prov"]="";
					row["ProvNum"]="";
					row["quadrant"]="";
					row["RxNum"]=0;
					row["SheetNum"]=0;
					row["signature"]="";
					if(rawComm.Rows[i]["SigPresent"].ToString()=="1") {
						row["signature"]=Lans.g("ChartModule","Signed");
					}
					row["Surf"]="";
					row["TaskNum"]=0;
					row["toothNum"]="";
					row["ToothNum"]="";
					row["ToothRange"]="";
					row["user"]=Userods.GetName(PIn.Long(rawComm.Rows[i]["UserNum"].ToString()));
					row["WebChatSessionNum"]=0;
					row["EmailMessageHideIn"]="0";
					row["EmailMessageHtmlType"]="0";
					rows.Add(row);
				}
				#endregion Commlog
				#region WebChatNote - HQ only
				if(PrefC.IsODHQ) {
					//connect to the webchat db for this query
					List<WebChatSession> listWebChatSessions=new List<WebChatSession>();
					WebChatMisc.DbAction(delegate() {
						command=@"SELECT *
										FROM webchatsession
										WHERE webchatsession.PatNum="+POut.Long(patNum)+@" OR webchatsession.PatNum="+POut.Long(family.Guarantor.PatNum)+@" 
										ORDER BY webchatsession.DateTcreated";
						listWebChatSessions=Crud.WebChatSessionCrud.SelectMany(command);
					});
					//Since this is an internal database on another server, be defensive to allow chart access if the other server is down for any reason.
					if(listWebChatSessions!=null) {//Will be null if the query failed or the server is unavailable or any reason.
						foreach(WebChatSession session in listWebChatSessions) {
							row=table.NewRow();
							row["AbbrDesc"]="";
							row["aptDateTime"]=DateTime.MinValue;
							row["AptNum"]=0;
							row["clinic"]="";
							row["ClinicNum"]=0;
							row["CodeNum"]="";
							row["colorBackG"]=Color.White.ToArgb();
							row["colorText"]=listProgNoteColorDefs[6].ItemColor.ToArgb().ToString();
							row["CommlogNum"]=0;
							row["CommSource"]="";
							row["commType"]="";
							row["dateEntryC"]="";
							row["dateTP"]="";
							row["description"]=Lans.g("ChartModule","Web Chat Session")+" - "+session.WebChatSessionNum.ToString();
							row["DocNum"]=0;
							row["dx"]="";
							row["Dx"]="";
							row["EmailMessageNum"]=0;
							row["FormPatNum"]=0;
							row["HideGraphics"]="";
							row["isLocked"]="";
							row["LabCaseNum"]=0;
							row["length"]="";
							if(PIn.DateT(session.DateTend.ToString()).Year>1880) {
									DateTime startTime=PIn.DateT(session.DateTcreated.ToString());
									DateTime endTime=PIn.DateT(session.DateTend.ToString());
									row["length"]=(endTime-startTime).ToStringHmm();
							}
							row["note"]=session.Note.ToString();
							row["orionDateScheduleBy"]="";
							row["orionDateStopClock"]="";
							row["orionDPC"]="";
							row["orionDPCpost"]="";
							row["orionIsEffectiveComm"]="";
							row["orionIsOnCall"]="";
							row["orionStatus2"]="";
							row["PatNum"]=session.PatNum.ToString();
							row["Priority"]="";
							row["priority"]="";
							row["ProcCode"]="";
							dateT=PIn.DateT(session.DateTcreated.ToString()); 
							if(dateT.Year<1880) {
								row["procDate"]="";
							}
							else {
								row["procDate"]=dateT.ToString(Lans.GetShortDateTimeFormat());
							}
							row["ProcDate"]=dateT;
							row["procTime"]="";
							if(dateT.TimeOfDay!=TimeSpan.Zero) {
								row["procTime"]=dateT.ToString("h:mm")+dateT.ToString("%t").ToLower();
							}
							row["procTimeEnd"]="";
							if(dateT.TimeOfDay!=TimeSpan.Zero) {
								row["procTimeEnd"]=dateT.ToString("h:mm")+dateT.ToString("%t").ToLower();
							}
							row["procFee"]="";
							row["ProcNum"]=0;
							row["ProcNumLab"]="";
							row["procStatus"]="";
							row["ProcStatus"]="";
							row["prov"]="";
							row["ProvNum"]="";
							row["quadrant"]="";
							row["RxNum"]=0;
							row["SheetNum"]=0;
							row["signature"]="";
							row["Surf"]="";
							row["TaskNum"]=0;
							row["toothNum"]="";
							row["ToothNum"]="";
							row["ToothRange"]="";
							row["user"]=session.TechName;
							row["WebChatSessionNum"]=session.WebChatSessionNum.ToString();
							row["EmailMessageHideIn"]="0";
							row["EmailMessageHtmlType"]="0";
							rows.Add(row);
						}
					}
				}
				#endregion WebChatNote
			}
			if(componentsToLoad.ShowFormPat) {
				#region formpat
				command = "SELECT FormDateTime,FormPatNum "
					+ "FROM formpat WHERE PatNum =" + POut.Long(patNum) + " ORDER BY FormDateTime";
				DataTable rawForm = dcon.GetTable(command);
				for(int i = 0;i < rawForm.Rows.Count;i++) {
					row = table.NewRow();
					row["AbbrDesc"]="";
					row["aptDateTime"] = DateTime.MinValue;
					row["AptNum"] = 0;
					row["clinic"]="";
					row["ClinicNum"]=0;
					row["CodeNum"] = "";
					row["colorBackG"] = Color.White.ToArgb();
					row["colorText"] = listProgNoteColorDefs[6].ItemColor.ToArgb().ToString();
					row["CommlogNum"] =0;
					row["CommSource"]="";
					row["commType"]="";
					row["dateEntryC"]="";
					row["dateTP"]="";
					row["description"] = Lans.g("ChartModule","Questionnaire");
					row["DocNum"]=0;
					row["dx"] = "";
					row["Dx"] = "";
					row["EmailMessageNum"] = 0;
					row["FormPatNum"] = rawForm.Rows[i]["FormPatNum"].ToString();
					row["HideGraphics"]="";
					row["isLocked"]="";
					row["LabCaseNum"] = 0;
					row["length"]="";
					row["note"] = "";
					row["orionDateScheduleBy"]="";
					row["orionDateStopClock"]="";
					row["orionDPC"]="";
					row["orionDPCpost"]="";
					row["orionIsEffectiveComm"]="";
					row["orionIsOnCall"]="";
					row["orionStatus2"]="";
					row["PatNum"] = "";
					row["Priority"] = "";
					row["priority"]="";
					row["ProcCode"] = "";
					dateT = PIn.DateT(rawForm.Rows[i]["FormDateTime"].ToString());
					row["ProcDate"] = dateT.ToShortDateString();
					if(dateT.TimeOfDay != TimeSpan.Zero) {
						row["procTime"] = dateT.ToString("h:mm") + dateT.ToString("%t").ToLower();
					}
					if(dateT.Year < 1880) {
						row["procDate"] = "";
					}
					else {
						row["procDate"] = dateT.ToString(Lans.GetShortDateTimeFormat());
					}
					if(dateT.TimeOfDay != TimeSpan.Zero) {
						row["procTime"] = dateT.ToString("h:mm") + dateT.ToString("%t").ToLower();
					}
					row["procTimeEnd"]="";
					row["procFee"] = "";
					row["ProcNum"] = 0;
					row["ProcNumLab"] = "";
					row["procStatus"] = "";
					row["ProcStatus"] = "";
					row["prov"] = "";
					row["ProvNum"]="";
					row["quadrant"]="";
					row["RxNum"] = 0;
					row["SheetNum"] = 0;
					row["signature"] = "";
					row["Surf"] = "";
					row["TaskNum"] = 0;
					row["toothNum"] = "";
					row["ToothNum"] = "";
					row["ToothRange"] = "";
					row["user"] = "";
					row["WebChatSessionNum"]=0;
					row["EmailMessageHideIn"]="0";
					row["EmailMessageHtmlType"]="0";
					rows.Add(row);
				}
				#endregion formpat
			}
			if(componentsToLoad.ShowRX) {
				#region Rx
 				command="SELECT RxNum,RxDate,Drug,Disp,ProvNum,Notes,PharmacyNum,UserNum,RxType,DateTStamp FROM rxpat WHERE PatNum="+POut.Long(patNum)
				+" ORDER BY RxDate";
				DataTable rawRx=dcon.GetTable(command);
				for(int i=0;i<rawRx.Rows.Count;i++) {
					row=table.NewRow();
					row["AbbrDesc"]="";
					row["aptDateTime"]=DateTime.MinValue;
					row["AptNum"]=0;
					row["clinic"]="";
					row["ClinicNum"]=0;
					row["CodeNum"]="";
					row["colorBackG"]=Color.White.ToArgb();
					row["colorText"]=listProgNoteColorDefs[5].ItemColor.ToArgb().ToString();
					row["CommlogNum"]=0;
					row["CommSource"]="";
					row["commType"]="";
					row["dateEntryC"]="";
					row["dateTP"]="";
					row["description"]=Lans.g("ChartModule","Rx - ")+rawRx.Rows[i]["Drug"].ToString()+" - #"+rawRx.Rows[i]["Disp"].ToString();
					if(rawRx.Rows[i]["PharmacyNum"].ToString()!="0") {
						row["description"]+="\r\n"+Pharmacies.GetDescription(PIn.Long(rawRx.Rows[i]["PharmacyNum"].ToString()));
					}
					row["DocNum"]=0;
					row["dx"]="";
					row["Dx"]="";
					row["EmailMessageNum"]=0;
					row["FormPatNum"]=0;
					row["HideGraphics"]="";
					row["isLocked"]="";
					row["LabCaseNum"]=0;
					row["length"]="";
					row["note"]=rawRx.Rows[i]["Notes"].ToString();
					row["orionDateScheduleBy"]="";
					row["orionDateStopClock"]="";
					row["orionDPC"]="";
					row["orionDPCpost"]="";
					row["orionIsEffectiveComm"]="";
					row["orionIsOnCall"]="";
					row["orionStatus2"]="";
					row["PatNum"]="";
					row["Priority"]="";
					row["priority"]="";
					row["ProcCode"]="";
					dateT=PIn.Date(rawRx.Rows[i]["RxDate"].ToString());
					if(dateT.Year<1880) {
						row["procDate"]="";
					}
					else {
						row["procDate"]=dateT.ToString(Lans.GetShortDateTimeFormat());
					}
					row["ProcDate"]=dateT;
					row["procFee"]="";
					row["ProcNum"]=0;
					row["ProcNumLab"]="";
					row["procStatus"]="";
					row["ProcStatus"]="";
					row["procTime"]="";
					row["procTimeEnd"]="";
					row["prov"]=Providers.GetAbbr(PIn.Long(rawRx.Rows[i]["ProvNum"].ToString()));
					row["ProvNum"]=rawRx.Rows[i]["ProvNum"];
					row["quadrant"]="";
					row["RxNum"]=rawRx.Rows[i]["RxNum"].ToString();
					row["SheetNum"]=0;
					row["signature"]="";
					row["Surf"]="";
					row["TaskNum"]=0;
					row["toothNum"]="";
					row["ToothNum"]="";
					row["ToothRange"]="";
					row["user"]="";
					row["WebChatSessionNum"]=0;
					row["EmailMessageHideIn"]="0";
					row["EmailMessageHtmlType"]="0";
					RxTypes rxType=PIn.Enum<RxTypes>(rawRx.Rows[i]["RxType"].ToString());
					row["RxType"]=rxType;
					//If RxPat entry is a log of pdmp bridge access
					if(rxType!=RxTypes.Rx) {
						DateTime timeAccessed=PIn.DateT(rawRx.Rows[i]["DateTStamp"].ToString());
						row["Description"]="PDMP Access: "+rxType.GetDescription()+"\nTime Accessed: "+timeAccessed.ToShortTimeString();
						row["colorText"]=Color.Black.ToArgb();
						row["user"]=Userods.GetName(PIn.Long(rawRx.Rows[i]["UserNum"].ToString()));
					}
					rows.Add(row);
				}
				#endregion Rx
			}
			if(componentsToLoad.ShowLabCases) {
				#region LabCase
				command="SELECT labcase.*,Description,Phone FROM labcase,laboratory "
				+"WHERE labcase.LaboratoryNum=laboratory.LaboratoryNum "
				+"AND PatNum="+POut.Long(patNum)
				+" ORDER BY DateTimeCreated";
				DataTable rawLab=dcon.GetTable(command);
				DateTime duedate;
				for(int i=0;i<rawLab.Rows.Count;i++) {
					row=table.NewRow();
					row["AbbrDesc"]="";
					row["aptDateTime"]=DateTime.MinValue;
					row["AptNum"]=0;
					row["clinic"]="";
					row["ClinicNum"]=0;
					row["CodeNum"]="";
					row["colorBackG"]=Color.White.ToArgb();
					row["colorText"]=listProgNoteColorDefs[7].ItemColor.ToArgb().ToString();
					row["CommlogNum"]=0;
					row["CommSource"]="";
					row["commType"]="";
					row["dateEntryC"]="";
					row["dateTP"]="";
					row["description"]=Lans.g("ChartModule","LabCase - ")+rawLab.Rows[i]["Description"].ToString()+" "
					+rawLab.Rows[i]["Phone"].ToString();
					if(PIn.Date(rawLab.Rows[i]["DateTimeDue"].ToString()).Year>1880) {
						duedate=PIn.DateT(rawLab.Rows[i]["DateTimeDue"].ToString());
						row["description"]+="\r\n"+Lans.g("ChartModule","Due")+" "+duedate.ToString("ddd")+" "
						+duedate.ToShortDateString()+" "+duedate.ToShortTimeString();
					}
					if(PIn.Date(rawLab.Rows[i]["DateTimeChecked"].ToString()).Year>1880) {
						row["description"]+="\r\n"+Lans.g("ChartModule","Quality Checked");
					}
					else if(PIn.Date(rawLab.Rows[i]["DateTimeRecd"].ToString()).Year>1880) {
						row["description"]+="\r\n"+Lans.g("ChartModule","Received");
					}
					else if(PIn.Date(rawLab.Rows[i]["DateTimeSent"].ToString()).Year>1880) {
						row["description"]+="\r\n"+Lans.g("ChartModule","Sent");
					}
					row["DocNum"]=0;
					row["dx"]="";
					row["Dx"]="";
					row["EmailMessageNum"]=0;
					row["FormPatNum"]=0;
					row["HideGraphics"]="";
					row["isLocked"]="";
					row["LabCaseNum"]=rawLab.Rows[i]["LabCaseNum"].ToString();
					row["length"]="";
					row["note"]=rawLab.Rows[i]["Instructions"].ToString();
					row["orionDateScheduleBy"]="";
					row["orionDateStopClock"]="";
					row["orionDPC"]="";
					row["orionDPCpost"]="";
					row["orionIsEffectiveComm"]="";
					row["orionIsOnCall"]="";
					row["orionStatus2"]="";
					row["PatNum"]="";
					row["Priority"]="";
					row["priority"]="";
					row["ProcCode"]="";
					dateT=PIn.DateT(rawLab.Rows[i]["DateTimeCreated"].ToString());
					if(dateT.Year<1880) {
						row["procDate"]="";
					}
					else {
						row["procDate"]=dateT.ToString(Lans.GetShortDateTimeFormat());
					}
					row["procTime"]="";
					if(dateT.TimeOfDay!=TimeSpan.Zero) {
						row["procTime"]=dateT.ToString("h:mm")+dateT.ToString("%t").ToLower();
					}
					row["ProcDate"]=dateT;
					row["procTimeEnd"]="";
					row["procFee"]="";
					row["ProcNum"]=0;
					row["ProcNumLab"]="";
					row["procStatus"]="";
					row["ProcStatus"]="";
					row["prov"]="";
					row["ProvNum"]="";
					row["quadrant"]="";
					row["RxNum"]=0;
					row["SheetNum"]=0;
					row["signature"]="";
					row["Surf"]="";
					row["TaskNum"]=0;
					row["toothNum"]="";
					row["ToothNum"]="";
					row["ToothRange"]="";
					row["user"]="";
					row["WebChatSessionNum"]=0;
					row["EmailMessageHideIn"]="0";
					row["EmailMessageHtmlType"]="0";
					rows.Add(row);
				}
				#endregion LabCase
			}
			if(componentsToLoad.ShowTasks) {
				#region Task
				command="SELECT task.*,COALESCE(tasklist.Descript,'') ListDisc,fampat.FName,fampat.PatNum "
				+"FROM patient pat "
				+"INNER JOIN patient fampat ON fampat.Guarantor=pat.Guarantor "
				+"INNER JOIN task ON task.KeyNum=fampat.PatNum AND task.ObjectType="+POut.Int((int)TaskObjectType.Patient)+" "
				+"LEFT JOIN tasklist ON task.TaskListNum=tasklist.TaskListNum "
				+"WHERE pat.PatNum="+POut.Long(patNum)+" "
				+"UNION ALL "
				+"SELECT task.*,COALESCE(tasklist.Descript,'') ListDisc,patient.FName,patient.PatNum "
				+"FROM task "
				+"INNER JOIN appointment ON appointment.AptNum=task.KeyNum AND task.ObjectType="+POut.Int((int)TaskObjectType.Appointment)+" "
				+"LEFT JOIN tasklist ON task.TaskListNum=tasklist.TaskListNum "
				+"LEFT JOIN patient ON patient.PatNum=appointment.PatNum "
				+"WHERE appointment.PatNum IN ("+string.Join(",",family.ListPats.Select(x => x.PatNum))+") "
				+"ORDER BY DateTimeEntry";
				DataTable rawTask=dcon.GetTable(command);
				List<long> taskNums=rawTask.Select().Select(x => PIn.Long(x["TaskNum"].ToString())).ToList();
				List<TaskList> listTaskLists=TaskLists.GetAll();
				Dictionary<long,List<TaskNote>> dictTaskNotes=TaskNotes.RefreshForTasks(taskNums).GroupBy(x => x.TaskNum).ToDictionary(x => x.Key,x => x.ToList());
				for(int i=0;i<rawTask.Rows.Count;i++) {
					DataRow rawTaskRow=rawTask.Rows[i];
					row=table.NewRow();
					row["AbbrDesc"]="";
					row["aptDateTime"]=DateTime.MinValue;
					row["AptNum"]=0;
					row["clinic"]="";
					row["ClinicNum"]=0;
					row["CodeNum"]="";
					//colors the same as notes
					row["colorText"] = listProgNoteColorDefs[18].ItemColor.ToArgb().ToString();
					row["colorBackG"] = listProgNoteColorDefs[19].ItemColor.ToArgb().ToString();
					//row["colorText"] = Defs.Long[(int)DefCat.ProgNoteColors][6].ItemColor.ToArgb().ToString();//same as commlog
					row["CommlogNum"]=0;
					row["CommSource"]="";
					row["commType"]="";
					row["dateEntryC"]="";
					row["dateTP"]="";
					txt="";
					switch(PIn.Enum<TaskObjectType>(rawTaskRow["ObjectType"].ToString())) {
						case TaskObjectType.Patient:
						case TaskObjectType.Appointment:
							//Prepend the name of the family member so that it is apparent that this task is not for this specific patient.
							if(rawTaskRow["PatNum"].ToString()!=patNum.ToString()) {
								txt="("+rawTaskRow["FName"].ToString()+") ";
							}
							break;
						case TaskObjectType.None:
						default:
							//Do nothing.
							break;
					}
					if(rawTaskRow["TaskStatus"].ToString()=="2") {//completed
						txt += Lans.g("ChartModule","Completed ");
						row["colorBackG"] = Color.White.ToArgb();
						//use same as note colors for completed tasks
						row["colorText"] = listProgNoteColorDefs[20].ItemColor.ToArgb().ToString();
						row["colorBackG"] = listProgNoteColorDefs[21].ItemColor.ToArgb().ToString();
					}
					long taskListNum=PIn.Long(rawTaskRow["TaskListNum"].ToString());
					row["description"]=txt+Lans.g("ChartModule","Task - In List: ")+TaskLists.GetFullPath(taskListNum,listTaskLists);
					row["DocNum"]=0;
					row["dx"]="";
					row["Dx"]="";
					row["EmailMessageNum"]=0;
					row["FormPatNum"]=0;
					row["HideGraphics"]="";
					row["isLocked"]="";
					row["LabCaseNum"]=0;
					row["length"]="";
					txt="";
					string username;
					if(!rawTaskRow["Descript"].ToString().StartsWith("==") && rawTaskRow["UserNum"].ToString()!="") {
						if(!dictUserNames.TryGetValue(rawTaskRow["UserNum"].ToString(),out username)) {
							username=Userods.GetName(PIn.Long(rawTaskRow["UserNum"].ToString()));
						}
						txt+=username+" - ";
					}
					txt+=rawTaskRow["Descript"].ToString();
					long taskNum=PIn.Long(rawTaskRow["TaskNum"].ToString());
					List<TaskNote> listNotesCur;
					if(dictTaskNotes.TryGetValue(taskNum,out listNotesCur)) {
						foreach(TaskNote noteCur in listNotesCur) {
							string noteUserName;
							if(!dictUserNames.TryGetValue(noteCur.UserNum.ToString(),out noteUserName)) {
								noteUserName=Userods.GetName(noteCur.UserNum);
							}
							txt+="\r\n"//even on the first loop
								+"=="+noteUserName+" - "
								+noteCur.DateTimeNote.ToShortDateString()+" "
								+noteCur.DateTimeNote.ToShortTimeString()
								+" - "+noteCur.Note;
						}
					}
					row["note"]=txt;
					row["orionDateScheduleBy"]="";
					row["orionDateStopClock"]="";
					row["orionDPC"]="";
					row["orionDPCpost"]="";
					row["orionIsEffectiveComm"]="";
					row["orionIsOnCall"]="";
					row["orionStatus2"]="";
					row["PatNum"]=rawTaskRow["PatNum"].ToString();
					row["Priority"]="";
					row["priority"]="";
					row["ProcCode"]="";
					dateT = PIn.DateT(rawTaskRow["DateTask"].ToString());
					row["procTime"]="";
					if(dateT.Year < 1880) {//check if due date set for task or note
						dateT = PIn.DateT(rawTaskRow["DateTimeEntry"].ToString());
						if(dateT.Year < 1880) {//since dateT was just redefined, check it now
							row["procDate"] = "";
						}
						else {
							row["procDate"] = dateT.ToShortDateString();
						}
						if(dateT.TimeOfDay != TimeSpan.Zero) {
							row["procTime"] = dateT.ToString("h:mm") + dateT.ToString("%t").ToLower();
						}
						row["ProcDate"] = dateT;
					}
					else {
						row["procDate"] =dateT.ToString(Lans.GetShortDateTimeFormat());
						if(dateT.TimeOfDay != TimeSpan.Zero) {
							row["procTime"] = dateT.ToString("h:mm") + dateT.ToString("%t").ToLower();
						}
						row["ProcDate"] = dateT;
						//row["Surf"] = "DUE";
					}
					row["procTimeEnd"]="";
					row["procFee"]="";
					row["ProcNum"]=0;
					row["ProcNumLab"]="";
					row["procStatus"]="";
					row["ProcStatus"]="";
					row["prov"]="";
					row["ProvNum"]="";
					row["quadrant"]="";
					row["RxNum"]=0;
					row["SheetNum"]=0;
					row["signature"]="";
					row["Surf"]="";
					row["TaskNum"]=taskNum;
					row["toothNum"]="";
					row["ToothNum"]="";
					row["ToothRange"]="";
					row["user"]="";
					row["WebChatSessionNum"]=0;
					row["EmailMessageHideIn"]="0";
					row["EmailMessageHtmlType"]="0";
					rows.Add(row);
				}
				#endregion Task
			}
			#region Appointments
			command="SELECT * FROM appointment WHERE PatNum="+POut.Long(patNum);
			if(componentsToLoad.ShowAppointments) {//we will need this table later for planned appts, so always need to get.
				//get all appts
			}
			else{
				//only include planned appts.  We will need those later, but not in this grid.
				command+=" AND AptStatus = "+POut.Int((int)ApptStatus.Planned);
			}
			command+=" ORDER BY AptDateTime";
			rawApt=dcon.GetTable(command);
			long apptStatus;
			for(int i=0;i<rawApt.Rows.Count;i++) {
				row=table.NewRow();
				row["AbbrDesc"]="";
				row["aptDateTime"]=DateTime.MinValue;
				row["AptNum"]=rawApt.Rows[i]["AptNum"].ToString();
				row["clinic"]="";
				row["ClinicNum"]=rawApt.Rows[i]["ClinicNum"].ToString();
				row["colorBackG"]=Color.White.ToArgb();
				dateT=PIn.DateT(rawApt.Rows[i]["AptDateTime"].ToString());
				apptStatus=PIn.Long(rawApt.Rows[i]["AptStatus"].ToString());
				row["colorBackG"]="";
				row["colorText"]=listProgNoteColorDefs[8].ItemColor.ToArgb().ToString();
				row["CommlogNum"]=0;
				row["CommSource"]="";
				row["commType"]="";
				row["dateEntryC"]="";
				row["dateTP"]="";
				row["description"]=Lans.g("ChartModule","Appointment - ")+dateT.ToShortTimeString()+"\r\n"
				+rawApt.Rows[i]["ProcDescript"].ToString();
				if(dateT.Date.Date==DateTime.Today.Date) {
					row["colorBackG"]=listProgNoteColorDefs[9].ItemColor.ToArgb().ToString(); //deliniates nicely between old appts
					row["colorText"]=listProgNoteColorDefs[8].ItemColor.ToArgb().ToString();
				}
				else if(dateT.Date<DateTime.Today) {
					row["colorBackG"]=listProgNoteColorDefs[11].ItemColor.ToArgb().ToString();
					row["colorText"]=listProgNoteColorDefs[10].ItemColor.ToArgb().ToString();
				}
				else if(dateT.Date>DateTime.Today) {
					row["colorBackG"]=listProgNoteColorDefs[13].ItemColor.ToArgb().ToString(); //at a glace, you see green...the pt is good to go as they have a future appt scheduled
					row["colorText"]=listProgNoteColorDefs[12].ItemColor.ToArgb().ToString();
				}
				if(apptStatus==(int)ApptStatus.Broken) {
					row["colorText"]=listProgNoteColorDefs[14].ItemColor.ToArgb().ToString();
					row["colorBackG"]=listProgNoteColorDefs[15].ItemColor.ToArgb().ToString();
					row["description"]=Lans.g("ChartModule","BROKEN Appointment - ")+dateT.ToShortTimeString()+"\r\n"
					+rawApt.Rows[i]["ProcDescript"].ToString();
				}
				else if(apptStatus==(int)ApptStatus.UnschedList) {
					row["colorText"]=listProgNoteColorDefs[14].ItemColor.ToArgb().ToString();
					row["colorBackG"]=listProgNoteColorDefs[15].ItemColor.ToArgb().ToString();
					row["description"]=Lans.g("ChartModule","UNSCHEDULED Appointment - ")+dateT.ToShortTimeString()+"\r\n"
					+rawApt.Rows[i]["ProcDescript"].ToString();
				}
				else if(apptStatus==(int)ApptStatus.Planned) {
					row["colorText"]=listProgNoteColorDefs[16].ItemColor.ToArgb().ToString();
					row["colorBackG"]=listProgNoteColorDefs[17].ItemColor.ToArgb().ToString();
					row["description"]=Lans.g("ChartModule","PLANNED Appointment")+"\r\n"
					+rawApt.Rows[i]["ProcDescript"].ToString();
				}
				else if(apptStatus==(int)ApptStatus.PtNote) {
					row["colorText"]=listProgNoteColorDefs[18].ItemColor.ToArgb().ToString();
					row["colorBackG"]=listProgNoteColorDefs[19].ItemColor.ToArgb().ToString();
					row["description"] = Lans.g("ChartModule","*** Patient NOTE  *** - ") + dateT.ToShortTimeString();
				}
				else if(apptStatus ==(int)ApptStatus.PtNoteCompleted) {
					row["colorText"] = listProgNoteColorDefs[20].ItemColor.ToArgb().ToString();
					row["colorBackG"] = listProgNoteColorDefs[21].ItemColor.ToArgb().ToString();
					row["description"] = Lans.g("ChartModule","** Complete Patient NOTE ** - ") + dateT.ToShortTimeString();
				}
				row["DocNum"]=0;
				row["dx"]="";
				row["Dx"]="";
				row["EmailMessageNum"]=0;
				row["FormPatNum"]=0;
				row["HideGraphics"]="";
				row["isLocked"]="";
				row["LabCaseNum"]=0;
				row["length"]="";
				if(rawApt.Rows[i]["Pattern"].ToString()!="") {
					row["length"]=new TimeSpan(0,rawApt.Rows[i]["Pattern"].ToString().Length*5,0).ToStringHmm();
				}
				row["note"]=rawApt.Rows[i]["Note"].ToString();
				row["orionDateScheduleBy"]="";
				row["orionDateStopClock"]="";
				row["orionDPC"]="";
				row["orionDPCpost"]="";
				row["orionIsEffectiveComm"]="";
				row["orionIsOnCall"]="";
				row["orionStatus2"]="";
				row["PatNum"]="";
				row["Priority"]="";
				row["priority"]="";
				row["ProcCode"]="";
				if(dateT.Year<1880) {
					row["procDate"]="";
				}
				else {
					row["procDate"]=dateT.ToString(Lans.GetShortDateTimeFormat());
				}
				row["procTime"]="";
				if(dateT.TimeOfDay!=TimeSpan.Zero) {
					row["procTime"]=dateT.ToString("h:mm")+dateT.ToString("%t").ToLower();
				}
				row["ProcDate"]=dateT;
				row["procTimeEnd"]="";
				row["procFee"]="";
				row["ProcNum"]=0;
				row["ProcNumLab"]="";
				row["procStatus"]="";
				row["ProcStatus"]="";
				row["prov"]="";
				row["ProvNum"]="";
				row["quadrant"]="";
				row["RxNum"]=0;
				row["SheetNum"]=0;
				row["signature"]="";
				row["Surf"]="";
				row["TaskNum"]=0;
				row["toothNum"]="";
				row["ToothNum"]="";
				row["ToothRange"]="";
				row["user"]="";
				row["WebChatSessionNum"]=0;
				row["EmailMessageHideIn"]="0";
				row["EmailMessageHtmlType"]="0";
				rows.Add(row);
			}
			#endregion Appointments
			if(componentsToLoad.ShowEmail) {
				#region email
				List<EmailSentOrReceived> listAckTypes=EmailMessages.GetUnsentTypes(EmailPlatform.Ack).Concat(EmailMessages.GetSentTypes(EmailPlatform.Ack)).ToList();
				string ackTypesStr=string.Join(",",listAckTypes.Select(x => POut.Int((int)x)));
				//Get emails for patient
				//If a user creates an email that is attached to a patient, it will show up here for everyone.
				command="SELECT EmailMessageNum,MsgDateTime,Subject,BodyText,PatNum,SentOrReceived,UserNum,emailmessage.HideIn,emailmessage.HtmlType "
				+"FROM emailmessage "
				+"WHERE PatNum="+POut.Long(patNum)+" AND SentOrReceived NOT IN ("+ackTypesStr+") "//Do not show Direct message acknowledgements in Chart progress notes
				+"ORDER BY MsgDateTime";
				DataTable rawEmail=dcon.GetTable(command);
				for(int i=0;i<rawEmail.Rows.Count;i++) {
					row=table.NewRow();
					row["AbbrDesc"]="";
					row["aptDateTime"]=DateTime.MinValue;
					row["AptNum"]=0;
					row["clinic"]="";
					row["ClinicNum"]=0;
					row["CodeNum"]="";
					row["colorBackG"]=Color.White.ToArgb();
					row["colorText"]=listProgNoteColorDefs[6].ItemColor.ToArgb().ToString();//needs to change
					row["CommlogNum"]=0;
					row["CommSource"]="";
					row["commType"]="";
					row["dateEntryC"]="";
					row["dateTP"]="";
					txt="";
					if(rawEmail.Rows[i]["SentOrReceived"].ToString()=="0") {
						txt=Lans.g("ChartModule","(unsent) ");
					}
					row["description"]=Lans.g("ChartModule","Email - ")+txt+rawEmail.Rows[i]["Subject"].ToString();
					row["DocNum"]=0;
					row["dx"]="";
					row["Dx"]="";
					row["EmailMessageNum"]=rawEmail.Rows[i]["EmailMessageNum"].ToString();
					row["FormPatNum"]=0;
					row["HideGraphics"]="";
					row["isLocked"]="";
					row["LabCaseNum"]=0;
					row["length"]="";
					row["note"]=rawEmail.Rows[i]["BodyText"].ToString();
					row["orionDateScheduleBy"]="";
					row["orionDateStopClock"]="";
					row["orionDPC"]="";
					row["orionDPCpost"]="";
					row["orionIsEffectiveComm"]="";
					row["orionIsOnCall"]="";
					row["orionStatus2"]="";
					row["PatNum"]="";
					row["Priority"]="";
					row["priority"]="";
					row["ProcCode"]="";
					//row["PatNum"]=rawEmail.Rows[i]["PatNum"].ToString();
					dateT=PIn.DateT(rawEmail.Rows[i]["msgDateTime"].ToString());
					if(dateT.Year<1880) {
						row["procDate"]="";
					}
					else {
						row["procDate"]=dateT.ToString(Lans.GetShortDateTimeFormat());
					}
					row["ProcDate"]=dateT;
					row["procTime"]="";
					if(dateT.TimeOfDay!=TimeSpan.Zero) {
						row["procTime"]=dateT.ToString("h:mm")+dateT.ToString("%t").ToLower();
					}
					row["procTimeEnd"]="";
					row["procFee"]="";
					row["ProcNum"]=0;
					row["ProcNumLab"]="";
					row["procStatus"]="";
					row["ProcStatus"]="";
					row["prov"]="";
					row["ProvNum"]="";
					row["quadrant"]="";
					row["RxNum"]=0;
					row["SheetNum"]=0;
					row["signature"]="";
					row["Surf"]="";
					row["TaskNum"]=0;
					row["toothNum"]="";
					row["ToothNum"]="";
					row["ToothRange"]="";
					row["user"]=Userods.GetName(PIn.Long(rawEmail.Rows[i]["UserNum"].ToString()));
					row["WebChatSessionNum"]=0;
					row["EmailMessageHideIn"]=rawEmail.Rows[i]["HideIn"].ToString();
					row["EmailMessageHtmlType"]=rawEmail.Rows[i]["HtmlType"].ToString();
					rows.Add(row);
				}
				#endregion email
			}
			if(componentsToLoad.ShowSheets) {
				#region sheet
				string sigPresentCase="AVG(CASE WHEN FieldValue!='' THEN 1 ELSE 0 END) AS SigPresent ";
				//Oracle cannot use CLOB columns when DISTINCT, ORDER BY, or GROUP BY is used.
				//Since we only care about the sheer presence of data, we'll look at the first character (or byte) of the CLOB column by using SUBSTR.
				if(DataConnection.DBtype==DatabaseType.Oracle) {
					sigPresentCase="AVG(CASE WHEN DBMS_LOB.SUBSTR(FieldValue,1)!='' THEN 1 ELSE 0 END) AS SigPresent ";
				}
				command="SELECT PatNum,Description,sheet.SheetNum,sheet.DocNum,DateTimeSheet,SheetType,"+sigPresentCase
					+"FROM sheet "
					+"LEFT JOIN sheetfield ON sheet.SheetNum=sheetfield.SheetNum "
					+"AND sheetfield.FieldType="+POut.Long((int)SheetFieldType.SigBox)+" "
					+"WHERE (sheet.PatNum="+POut.Long(patNum);
				List<Patient> listPatientClonesAll=new List<Patient>();
				if(PrefC.GetBool(PrefName.ShowFeaturePatientClone)) {
					List<long> listPatientClonePatNums=Patients.GetClonePatNumsAll(patNum);
					//Always include every single sheet for ANY clone or master of said clones.
					command+=" OR PatNum IN("+string.Join(",",listPatientClonePatNums)+")";
					//Now go get the patient object for each clone which might be used below.
					listPatientClonesAll=Patients.GetLimForPats(listPatientClonePatNums);
				}
				command+=") AND SheetType!="+POut.Long((int)SheetTypeEnum.Rx)+" "//rx are only accesssible from within Rx edit window.
				+"AND SheetType!="+POut.Long((int)SheetTypeEnum.LabSlip)+" ";//labslips are only accesssible from within the labslip edit window.
				if(!isAuditMode) {
					command+="AND IsDeleted=0 ";//Don't show deleted sheets unless it's audit mode.
				}
				command+="GROUP BY sheet.SheetNum,PatNum,Description,DateTimeSheet,SheetType "//Oracle compatible
				+"ORDER BY DateTimeSheet";
				DataTable rawSheet=dcon.GetTable(command);
				//SheetTypeEnum sheetType;
				for(int i=0;i<rawSheet.Rows.Count;i++) {
					row=table.NewRow();
					row["AbbrDesc"]="";
					row["aptDateTime"]=DateTime.MinValue;
					row["AptNum"]=0;
					row["clinic"]="";
					row["ClinicNum"]=0;
					row["CodeNum"]="";
					row["colorBackG"]=Color.White.ToArgb();
					row["colorText"]=Color.Black.ToArgb();//Defs.Long[(int)DefCat.ProgNoteColors][6].ItemColor.ToArgb().ToString();//needs to change
					row["CommlogNum"]=0;
					row["CommSource"]="";
					row["commType"]="";
					dateT=PIn.DateT(rawSheet.Rows[i]["DateTimeSheet"].ToString());
					if(dateT.Year<1880) {
						row["dateEntryC"]="";
						row["dateTP"]="";
					}
					else {
						row["dateEntryC"]=dateT.ToString(Lans.GetShortDateTimeFormat());
						row["dateTP"]=dateT.ToString(Lans.GetShortDateTimeFormat());
					}
					//Add patient name if using clone feature and the sheet belongs to the clone.
					if(PrefC.GetBool(PrefName.ShowFeaturePatientClone) && rawSheet.Rows[i]["PatNum"].ToString()!=patNum.ToString()) {
						Patient patientClone=listPatientClonesAll.FirstOrDefault(x => x.PatNum==PIn.Long(rawSheet.Rows[i]["PatNum"].ToString()));
						if(patientClone!=null && !string.IsNullOrWhiteSpace(patientClone.FName)) {
							row["description"]="("+patientClone.FName+") ";
						}
					}
					row["description"]+=rawSheet.Rows[i]["Description"].ToString();
					row["DocNum"]=rawSheet.Rows[i]["DocNum"].ToString();
					row["dx"]="";
					row["Dx"]="";
					row["EmailMessageNum"]=0;
					row["FormPatNum"]=0;
					row["HideGraphics"]="";
					row["isLocked"]="";
					row["LabCaseNum"]=0;
					row["length"]="";
					row["note"]="";
					row["orionDateScheduleBy"]="";
					row["orionDateStopClock"]="";
					row["orionDPC"]="";
					row["orionDPCpost"]="";
					row["orionIsEffectiveComm"]="";
					row["orionIsOnCall"]="";
					row["orionStatus2"]="";
					row["PatNum"]="";
					row["Priority"]="";
					row["priority"]="";
					row["ProcCode"]="";
					if(dateT.Year<1880) {
						row["procDate"]="";
					}
					else {
						row["procDate"]=dateT.ToString(Lans.GetShortDateTimeFormat());
					}
					row["ProcDate"]=dateT;
					row["procTime"]="";
					if(dateT.TimeOfDay!=TimeSpan.Zero) {
						row["procTime"]=dateT.ToString("h:mm")+dateT.ToString("%t").ToLower();
					}
					row["procTimeEnd"]="";
					row["procFee"]="";
					row["ProcNum"]=0;
					row["ProcNumLab"]="";
					row["procStatus"]="";
					row["ProcStatus"]="";
					row["prov"]="";
					row["ProvNum"]="";
					row["quadrant"]="";
					row["RxNum"]=0;
					row["SheetNum"]=rawSheet.Rows[i]["SheetNum"].ToString();
					row["signature"]="";
					if(PIn.Double(rawSheet.Rows[i]["SigPresent"].ToString())==1) {
						row["signature"]=Lans.g("ChartModule","Signed");
					}
					else if(PIn.Double(rawSheet.Rows[i]["SigPresent"].ToString())==0) {
						row["signature"]="";
					}
					else {
						row["signature"]=Lans.g("ChartModule","Partial");
					}
					row["Surf"]="";
					row["TaskNum"]=0;
					row["toothNum"]="";
					row["ToothNum"]="";
					row["ToothRange"]="";
					row["user"]="";
					row["WebChatSessionNum"]=0;
					row["EmailMessageHideIn"]="0";
					row["EmailMessageHtmlType"]="0";
					rows.Add(row);
				}
				#endregion sheet
			}
			#region Sorting
			rows.Sort(CompareChartRows);
			//Canadian lab procedures need to come immediately after their corresponding proc---------------------------------
			for(int i=0;i<labRows.Count;i++) {
				for(int r=0;r<rows.Count;r++) {
					if(rows[r]["ProcNum"].ToString()==labRows[i]["ProcNumLab"].ToString()) {
						rows.Insert(r+1,labRows[i]);
						break;
					}
				}
			}
			#endregion Sorting
			for(int i=0;i<rows.Count;i++) {
				table.Rows.Add(rows[i]);
			}
			return table;
		}

		public static DataTable GetPlannedApt(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum);
			}
			DataConnection dcon=new DataConnection();
			DataTable table=new DataTable("Planned");
			DataRow row;
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("AptNum");
			table.Columns.Add("colorBackG");
			table.Columns.Add("colorText");
			table.Columns.Add("dateSched");
			table.Columns.Add("ItemOrder");
			table.Columns.Add("minutes");
			table.Columns.Add("Note");
			table.Columns.Add("ProcDescript");
			table.Columns.Add("PlannedApptNum");
			table.Columns.Add("AptStatus");
			table.Columns.Add("SchedAptNum");  //The AptNum of the appointment (scheduled or not) that is attached to the planned appointment.  Could be Scheduled, Complete, Broken, ASAP, or Unscheduled statuses.
			//but we won't actually fill this table with rows until the very end.  It's more useful to use a List<> for now.
			List<DataRow> rows=new List<DataRow>();
			//The query below was causing a max join error for big offices.  It's fixed now, 
			//but a better option for next time would be to put SET SQL_BIG_SELECTS=1; before the query.
			string command="SELECT plannedappt.AptNum,ItemOrder,PlannedApptNum,appointment.AptDateTime,"
				+"appointment.Pattern,appointment.AptStatus,"
				+"COUNT(DISTINCT procedurelog.ProcNum) someAreComplete, "
				+"appointment.AptNum AS schedAptNum "
				+"FROM plannedappt "
				+"LEFT JOIN appointment ON appointment.NextAptNum=plannedappt.AptNum AND appointment.NextAptNum!=0 "
				+"LEFT JOIN procedurelog ON procedurelog.PlannedAptNum=plannedappt.AptNum	AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+" "
				+"WHERE plannedappt.PatNum="+POut.Long(patNum)+" ";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command+="GROUP BY plannedappt.AptNum ";
			}
			else {
				command+="GROUP BY plannedappt.AptNum,ItemOrder,PlannedApptNum,appointment.AptDateTime,"
				+"appointment.Pattern,appointment.AptStatus,appointment.AptNum ";
			}
			command+="ORDER BY ItemOrder";
			//plannedappt.AptNum does refer to the planned appt, but the other fields in the result are for the linked scheduled appt.
			DataTable rawPlannedAppts=dcon.GetTable(command);
			DataRow aptRow;
			int itemOrder=1;
			DateTime dateSched;
			ApptStatus aptStatus;
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ProgNoteColors);
			for(int i=0;i<rawPlannedAppts.Rows.Count;i++) {
				aptRow=null;
				for(int a=0;a<rawApt.Rows.Count;a++) {
					if(rawApt.Rows[a]["AptNum"].ToString()==rawPlannedAppts.Rows[i]["AptNum"].ToString()) {
						aptRow=rawApt.Rows[a];
						break;
					}
				}
				if(aptRow==null) {
					continue;//this will have to be fixed in dbmaint.
				}
				//repair any item orders here rather than in dbmaint. It's really fast.
				if(itemOrder.ToString()!=rawPlannedAppts.Rows[i]["ItemOrder"].ToString()) {
					command="UPDATE plannedappt SET ItemOrder="+POut.Long(itemOrder)
						+" WHERE PlannedApptNum="+rawPlannedAppts.Rows[i]["PlannedApptNum"].ToString();
					dcon.NonQ(command);
				}
				//end of repair
				row=table.NewRow();
				row["AptNum"]=aptRow["AptNum"].ToString();
				dateSched=PIn.Date(rawPlannedAppts.Rows[i]["AptDateTime"].ToString());
				row["AptStatus"]=PIn.Long(rawPlannedAppts.Rows[i]["AptStatus"].ToString());
				row["SchedAptNum"]=PIn.Long(rawPlannedAppts.Rows[i]["schedAptNum"].ToString());
				//Colors----------------------------------------------------------------------------
				aptStatus=(ApptStatus)PIn.Long(rawPlannedAppts.Rows[i]["AptStatus"].ToString());
				//change color if completed, broken, or unscheduled no matter the date
				if(aptStatus==ApptStatus.Broken || aptStatus==ApptStatus.UnschedList) {
					row["colorBackG"]=listDefs[15].ItemColor.ToArgb().ToString();
					row["colorText"]=listDefs[14].ItemColor.ToArgb().ToString();
				}
				else if(aptStatus==ApptStatus.Complete) {
					row["colorBackG"]=listDefs[11].ItemColor.ToArgb().ToString();
					row["colorText"]=listDefs[10].ItemColor.ToArgb().ToString();
				}
				else if(aptStatus==ApptStatus.Scheduled && dateSched.Date!=DateTime.Today.Date) {
					row["colorBackG"]=listDefs[13].ItemColor.ToArgb().ToString();
					row["colorText"]=listDefs[12].ItemColor.ToArgb().ToString();
				}
				else if(dateSched.Date<DateTime.Today && dateSched!=DateTime.MinValue) {//Past
					row["colorBackG"]=listDefs[11].ItemColor.ToArgb().ToString();
					row["colorText"]=listDefs[10].ItemColor.ToArgb().ToString();
				}
				else if(dateSched.Date == DateTime.Today.Date) { //Today
					row["colorBackG"]=listDefs[9].ItemColor.ToArgb().ToString();
					row["colorText"]=listDefs[8].ItemColor.ToArgb().ToString();
				}
				else if(dateSched.Date > DateTime.Today) { //Future
					row["colorBackG"]=listDefs[13].ItemColor.ToArgb().ToString();
					row["colorText"]=listDefs[12].ItemColor.ToArgb().ToString();
				}
				else {
					row["colorBackG"]=Color.White.ToArgb().ToString();
					row["colorText"]=Color.Black.ToArgb().ToString();
				}
				//end of colors------------------------------------------------------------------------------
				if(dateSched.Year<1880) {
					row["dateSched"]="";
				}
				else {
					row["dateSched"]=dateSched.ToShortDateString();
				}
				row["ItemOrder"]=itemOrder.ToString();
				row["minutes"]=(aptRow["Pattern"].ToString().Length*5).ToString();
				row["Note"]=aptRow["Note"].ToString();
				row["PlannedApptNum"]=rawPlannedAppts.Rows[i]["PlannedApptNum"].ToString();
				row["ProcDescript"]=aptRow["ProcDescript"].ToString();
				if(aptStatus==ApptStatus.Complete) {
					row["ProcDescript"]=Lans.g("ContrChart","(Completed) ")+ row["ProcDescript"];
				}
				else if(dateSched == DateTime.Today.Date) {
					row["ProcDescript"]=Lans.g("ContrChart","(Today's) ")+ row["ProcDescript"];
				}
				else if(rawPlannedAppts.Rows[i]["someAreComplete"].ToString()!="0"){
					row["ProcDescript"]=Lans.g("ContrChart","(Some procs complete) ")+ row["ProcDescript"];
				}
				rows.Add(row);
				itemOrder++;
			}
			for(int i=0;i<rows.Count;i++) {
				table.Rows.Add(rows[i]);
			}
			return table;
		}

		///<summary>Creates a DataTable for Patient Info similar to ControlChart.FillPtGrid(). Uses the default fields.</summary>
		public static DataTable GetPtInfoForApi(Patient patient) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patient);
			}
			DataTable tableRetVal=new DataTable(); //Table creation & structure
			DataColumn col=new DataColumn("Field");
			tableRetVal.Columns.Add(col);
			col=new DataColumn("Contents");
			tableRetVal.Columns.Add(col);
			DataRow row; //Initialize our row
			PatientNote patientNote=PatientNotes.Refresh(patient.PatNum,patient.Guarantor); //Manually gathering local variables, as using LoadData will gather a lot of unneeded data.
			List<PatPlan> listPatPlans=PatPlans.Refresh(patient.PatNum);
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(Patients.GetFamily(patient.PatNum));
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			int ordinal=0;
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.ChartPatientInformation); //Each field we're going to get data for
			DisplayField fieldCur;
			for(int f=0;f<listDisplayFields.Count;f++) { //For each field, make a new row with its name as Field, then fill Contents on a case-by-case basis.
				fieldCur=listDisplayFields[f];
				row=tableRetVal.NewRow();
				//Depending on the case, the Field of a row can change later.
				if(fieldCur.Description=="") {
					row["Field"]=fieldCur.InternalName;
				}
				else { 
					row["Field"]=fieldCur.Description;
				}
				switch(fieldCur.InternalName) {
					#region ABC0
					case "ABC0": //Patient's credit rating or other personal attributes
						row["Contents"]=patient.CreditType;
						tableRetVal.Rows.Add(row);
						break;
					#endregion ABC0
					#region Age
					case "Age": 
						row["Contents"]=PatientLogic.DateToAgeString(patient.Birthdate,patient.DateTimeDeceased);
						tableRetVal.Rows.Add(row);
						break;
					#endregion Age
					#region Allergies
					case "Allergies": //There can be multiple allergies, so we may need to make multiple rows.
						List<Allergy> listAllergies=Allergies.GetAll(patient.PatNum,showInactive:false);
						if(listAllergies.Count==0) { //If we don't have any entries, make it known that there are none, then add this row.
							row["Contents"]="None";
							tableRetVal.Rows.Add(row); //Only add the "None" row if no sub-rows will exist.
						}
						for(int i=0;i<listAllergies.Count;i++) { //Make a new row for each allergy in the list
							row=tableRetVal.NewRow();
							AllergyDef allergyDef=AllergyDefs.GetOne(listAllergies[i].AllergyDefNum);
							if(allergyDef==null) {
								row["Field"]="Allergy - Missing";
							}
							else {
								row["Field"]="Allergy - "+allergyDef.Description;
							}
							row["Contents"]=listAllergies[i].Reaction;
							tableRetVal.Rows.Add(row);
						}
						break;
					#endregion Allergies
					#region Billing Type
					case "Billing Type":
						row["Contents"]=Defs.GetName(DefCat.BillingTypes,patient.BillingType);
						tableRetVal.Rows.Add(row);
						break;
					#endregion Billing Type
					#region Birthdate
					case "Birthdate":
						row["Contents"]=Patients.DOBFormatHelper(patient.Birthdate,doMask:false);
						tableRetVal.Rows.Add(row);
						break;
					#endregion Birthdate
					#region Date First Visit
					case "Date First Visit":
						if(patient.DateFirstVisit.Year<1880) {
							row["Contents"]="Invalid Date";
						}
						else if(patient.DateFirstVisit==DateTime.Today) {
							row["Contents"]="New Patient";
						}
						else {
							row["Contents"]=patient.DateFirstVisit.ToShortDateString();
						}
						tableRetVal.Rows.Add(row);
						break;
					#endregion Date First Visit
					#region Med Urgent
					case "Med Urgent":
						row["Contents"]=patient.MedUrgNote;
						tableRetVal.Rows.Add(row);
						break;
					#endregion Med Urgent
					#region Medical Summary
					case "Medical Summary":
						row["Contents"]=patientNote.Medical;
						tableRetVal.Rows.Add(row);
						break;
					#endregion Medical Summary
					#region Medications
					case "Medications": //There can be multiple medications, so we may need to make multiple rows.
						Medications.RefreshCache();
						List<MedicationPat> listMedicationPats=MedicationPats.Refresh(patient.PatNum,includeDiscontinued:false);
						if(listMedicationPats.Count==0) {
							row["Contents"]="None";
							tableRetVal.Rows.Add(row); //Only add the "None" row if no sub-rows will exist.
						}
						string text;
						Medication medication;
						for(int i=0;i<listMedicationPats.Count;i++) { //Create a new row for each medication.
							row=tableRetVal.NewRow();
							if(listMedicationPats[i].MedicationNum==0) { //If MedicationNum is 0, we need to use MedDescript for eRx reasons.
								row["Field"]="Medication - "+listMedicationPats[i].MedDescript;
							}
							else { //Get the correct name for this medication.
								medication=Medications.GetMedication(listMedicationPats[i].MedicationNum);
								text=medication.MedName;
								if(medication.MedicationNum != medication.GenericNum) {
									text+="("+Medications.GetMedication(medication.GenericNum).MedName+")";
								}
								row["Field"]="Medication - "+text;
							}
							text=listMedicationPats[i].PatNote; //Formatting our field for Contents
							string noteMedGeneric="";
							if(listMedicationPats[i].MedicationNum!=0) {
								noteMedGeneric=Medications.GetGeneric(listMedicationPats[i].MedicationNum).Notes;
							}
							if(!string.IsNullOrWhiteSpace(noteMedGeneric)) {
								text+="("+noteMedGeneric+")";
							}
							row["Contents"]=text;
							tableRetVal.Rows.Add(row);
						}
						break;
					#endregion Medications
					#region Pat Restrictions
					case "Pat Restrictions": //There can be multiple Pat Restrictions, so we may need multiple rows.
						List<PatRestriction> listPatRestricts=PatRestrictions.GetAllForPat(patient.PatNum);
						if(listPatRestricts.Count==0) {
							row["Contents"]="None";
							tableRetVal.Rows.Add(row); //Only add the "None" row if no sub-rows will exist.
						}
						for(int i=0;i<listPatRestricts.Count;i++) { //Create a new row for each patient restriction.
							row=tableRetVal.NewRow();
							if(fieldCur.Description=="") {
								row["Field"]=fieldCur.InternalName;
							}
							else {
								row["Field"]=fieldCur.Description;
							}
							row["Contents"]=PatRestrictions.GetPatRestrictDesc(listPatRestricts[i].PatRestrictType);
							tableRetVal.Rows.Add(row);
						}
						break;
					#endregion Pat Restrictions
					#region Payor Types
					case "Payor Types":
						row["Contents"]=PayorTypes.GetCurrentDescription(patient.PatNum);
						tableRetVal.Rows.Add(row);
						break;
					#endregion Payor Types
					#region Premedicate
					case "Premedicate":
						if(patient.Premed) { //Only create & add this row if patient.Premed is true.
							row["Field"]="";
							if(string.IsNullOrWhiteSpace(fieldCur.Description)) {
								row["Contents"]=fieldCur.InternalName;
							}
							else {
								row["Contents"]=fieldCur.Description;
							}
							tableRetVal.Rows.Add(row);
						}
						break;
					#endregion Premedicate
					#region Pri Ins
					case "Pri Ins":
						string name;
						ordinal=PatPlans.GetOrdinal(PriSecMed.Primary,listPatPlans,listInsPlans,listInsSubs);
						if(ordinal>0) {
							InsSub insSub=InsSubs.GetSub(PatPlans.GetInsSubNum(listPatPlans,ordinal),listInsSubs);
							name=InsPlans.GetCarrierName(insSub.PlanNum,listInsPlans);
							if(listPatPlans[0].IsPending) {
								name+=" (pending)";
							}
							row["Contents"]=name;
						}
						else {
							row["Contents"]="";
						}
						tableRetVal.Rows.Add(row);
						break;
					#endregion Pri Ins
					#region Problems
					case "Problems":
						List<Disease> listDiseases=Diseases.Refresh(patient.PatNum,true);
						if(string.IsNullOrWhiteSpace(fieldCur.Description)) { //Use an alternate description if one exists.
							row["Field"]=fieldCur.InternalName;
						}
						else {
							row["Field"]=fieldCur.Description;
						}
						if(listDiseases.Count==0) {
							row["Contents"]="None";
							tableRetVal.Rows.Add(row); //Only add the "None" row if no sub-rows will exist.
						}
						for(int i=0;i<listDiseases.Count;i++) { //Add a new row for each problem/disease.
							row=tableRetVal.NewRow();
							if(listDiseases[i].DiseaseDefNum!=0) {
								row["Field"]="Problem - "+DiseaseDefs.GetName(listDiseases[i].DiseaseDefNum);
								row["Contents"]=listDiseases[i].PatNote;
							}
							else {
								row["Field"]="Problem - Missing";
								row["Contents"]=DiseaseDefs.GetItem(listDiseases[i].DiseaseDefNum)?.DiseaseName??"Invalid Problem";
							}
							tableRetVal.Rows.Add(row);
						}
						break;
					#endregion Problems
					#region Prov. (Pri, Sec)
					case "Prov. (Pri, Sec)":
						string provText="";
						if(patient.PriProv!=0) {
							provText+=Providers.GetAbbr(patient.PriProv)+", ";							
						}
						else {
							provText+="None, ";
						}
						if(patient.SecProv!=0) {
							provText+=Providers.GetAbbr(patient.SecProv);
						}
						else {
							provText+="None";
						}
						row["Contents"]=provText;
						tableRetVal.Rows.Add(row);
						break;
					#endregion Prov. (Pri, Sec)
					#region Referred From
					case "Referred From":
						List<RefAttach> listRefAttach=RefAttaches.Refresh(patient.PatNum).DistinctBy(x => x.ReferralNum).ToList();
						string referral="";
						for(int i=0;i<listRefAttach.Count;i++) {
							if(listRefAttach[i].RefType==ReferralType.RefFrom) {
								referral=Referrals.GetNameLF(listRefAttach[i].ReferralNum);
								break;
							}
						}
						row["Contents"]=referral;
						tableRetVal.Rows.Add(row);
						break;
					#endregion Referred From
					#region Sec Ins
					case "Sec Ins":
						ordinal=PatPlans.GetOrdinal(PriSecMed.Secondary,listPatPlans,listInsPlans,listInsSubs);
						if(ordinal>0) {
							InsSub insSub=InsSubs.GetSub(PatPlans.GetInsSubNum(listPatPlans,ordinal),listInsSubs);
							name=InsPlans.GetCarrierName(insSub.PlanNum,listInsPlans);
							if(listPatPlans[1].IsPending) {
								name+=" (pending)";
							}
							row["Contents"]=name;
						}
						else {
							row["Contents"]="";
						}
						tableRetVal.Rows.Add(row);
						break;
					#endregion Sec Ins
					#region Service Notes
					case "Service Notes":
						row["Contents"]=patientNote.Service;
						tableRetVal.Rows.Add(row);
						break;
					#endregion Service Notes
				}
			}
			return tableRetVal;
		}

		///<summary>Creates a DataTable for Planned Appts for a patient, similar to GetPlannedApt().</summary>
		public static DataTable GetPlannedApptsForApi(long patNum,int offset,int limit,string dateTimeFormatString) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patNum,dateTimeFormatString);
			}
			DataConnection dcon=new DataConnection();
			DataTable table=new DataTable();
			DataRow row;
			table.Columns.Add("AptNum");  //The AptNum of the appointment (scheduled or not) that is attached to the planned appointment.
			table.Columns.Add("ProvNum");
			table.Columns.Add("ItemOrder");
			table.Columns.Add("minutes");
			table.Columns.Add("ProcDescript");
			table.Columns.Add("Note");
			table.Columns.Add("dateSched");
			table.Columns.Add("AptStatus");
			List<DataRow> rows=new List<DataRow>();
			string command="SELECT plannedappt.AptNum,ItemOrder,PlannedApptNum,appointment.AptDateTime,"
				+"appointment.Pattern,appointment.AptStatus,"
				+"COUNT(DISTINCT procedurelog.ProcNum) someAreComplete, "
				+"appointment.AptNum AS schedAptNum "
				+"FROM plannedappt "
				+"LEFT JOIN appointment ON appointment.NextAptNum=plannedappt.AptNum AND appointment.NextAptNum!=0 "
				+"LEFT JOIN procedurelog ON procedurelog.PlannedAptNum=plannedappt.AptNum	AND procedurelog.ProcStatus="+POut.Int((int)ProcStat.C)+" "
				+"WHERE plannedappt.PatNum="+POut.Long(patNum)+" ";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command+="GROUP BY plannedappt.AptNum ";
			}
			else {
				command+="GROUP BY plannedappt.AptNum,ItemOrder,PlannedApptNum,appointment.AptDateTime,"
				+"appointment.Pattern,appointment.AptStatus,appointment.AptNum ";
			}
			command+="ORDER BY ItemOrder";
			//plannedappt.AptNum does refer to the planned appt, but the other fields in the result are for the linked scheduled appt.
			DataTable rawPlannedAppts=dcon.GetTable(command);
			DataRow aptRow;
			int itemOrder=1;
			DateTime dateSched;
			ApptStatus aptStatus;
			for(int i=0;i<rawPlannedAppts.Rows.Count;i++) {
				aptRow=null;
				for(int a=0;a<rawApt.Rows.Count;a++) {
					if(rawApt.Rows[a]["AptNum"].ToString()==rawPlannedAppts.Rows[i]["AptNum"].ToString()) {
						aptRow=rawApt.Rows[a];
						break;
					}
				}
				if(aptRow==null) {
					continue;//this will have to be fixed in dbmaint.
				}
				//repair any item orders here rather than in dbmaint. It's really fast.
				if(itemOrder.ToString()!=rawPlannedAppts.Rows[i]["ItemOrder"].ToString()) {
					command="UPDATE plannedappt SET ItemOrder="+POut.Long(itemOrder)
						+" WHERE PlannedApptNum="+rawPlannedAppts.Rows[i]["PlannedApptNum"].ToString();
					dcon.NonQ(command);
				}
				//end of repair
				row=table.NewRow();
				row["AptNum"]=aptRow["AptNum"].ToString();
				dateSched=PIn.Date(aptRow["AptDateTime"].ToString());
				aptStatus=(ApptStatus)PIn.Long(aptRow["AptStatus"].ToString());
				row["AptStatus"]=aptStatus.ToString();
				if(dateSched.Year<1880) {
					row["dateSched"]="";
				}
				else {
					row["dateSched"]=dateSched.ToString(dateTimeFormatString);
				}
				row["ProvNum"]=PIn.Long(aptRow["ProvNum"].ToString());
				row["ItemOrder"]=itemOrder.ToString();
				row["minutes"]=(aptRow["Pattern"].ToString().Length*5).ToString();
				row["Note"]=aptRow["Note"].ToString();
				row["ProcDescript"]=aptRow["ProcDescript"].ToString();
				if(aptStatus==ApptStatus.Complete) {
					row["ProcDescript"]=Lans.g("ContrChart","(Completed) ")+row["ProcDescript"];
				}
				else if(dateSched==DateTime.Today.Date) {
					row["ProcDescript"]=Lans.g("ContrChart","(Today's) ")+row["ProcDescript"];
				}
				else if(rawPlannedAppts.Rows[i]["someAreComplete"].ToString()!="0"){
					row["ProcDescript"]=Lans.g("ContrChart","(Some procs complete) ")+row["ProcDescript"];
				}
				rows.Add(row);
				itemOrder++;
			}
			for(int i=offset;i<rows.Count;i++) {
				if(i>=offset+limit) {
					break;
				}
				table.Rows.Add(rows[i]);
			}
			return table;
		}

		///<summary>The supplied DataRows must include the following columns: ProcNum,ProcDate,Priority,ToothRange,ToothNum,ProcCode. This sorts all objects in Chart module based on their dates, times, priority, and toothnum.  For time comparisons, procs are not included.  But if other types such as comm have a time component in ProcDate, then they will be sorted by time as well.</summary>
		public static int CompareChartRows(DataRow x,DataRow y) {
			//if dates are different, then sort by date
			if(((DateTime)x["ProcDate"]).Date!=((DateTime)y["ProcDate"]).Date){
				return ((DateTime)x["ProcDate"]).Date.CompareTo(((DateTime)y["ProcDate"]).Date);
			}
			//Sort by Type. Types are: Appointments, Procedures, CommLog, Tasks, Email, Lab Cases, Rx, Sheets.----------------------------------------------------
			int xInd=0;
			if(x["AptNum"].ToString()!="0") {
				xInd=0;
			}
			else if(x["ProcNum"].ToString()!="0") {
				xInd=1;
			}
			else if(x["CommlogNum"].ToString()!="0") {
				xInd=2;//commlogs, web chats and tasks are intermingled and sorted by time
			}
			else if(x["WebChatSessionNum"].ToString()!="0") {
				xInd=2;//commlogs, web chats and tasks are intermingled and sorted by time
			}
			else if(x["TaskNum"].ToString()!="0") {
				xInd=2;//commlogs, web chats and tasks are intermingled and sorted by time
			}
			else if(x["EmailMessageNum"].ToString()!="0") {
				xInd=3;
			}
			else if(x["LabCaseNum"].ToString()!="0") {
				xInd=4;
			}
			else if(x["RxNum"].ToString()!="0") {
				xInd=5;
			}
			else if(x["SheetNum"].ToString()!="0") {
				xInd=6;
			}
			int yInd=0;
			if(y["AptNum"].ToString()!="0") {
				yInd=0;
			}
			else if(y["ProcNum"].ToString()!="0") {
				yInd=1;
			}
			else if(y["CommlogNum"].ToString()!="0") {
				yInd=2;//commlogs, web chats and tasks are intermingled and sorted by time
			}
			else if(y["WebChatSessionNum"].ToString()!="0") {
				yInd=2;//commlogs, web chats and tasks are intermingled and sorted by time
			}
			else if(y["TaskNum"].ToString()!="0") {
				yInd=2;//commlogs, web chats and tasks are intermingled and sorted by time
			}
			else if(y["EmailMessageNum"].ToString()!="0") {
				yInd=3;
			}
			else if(y["LabCaseNum"].ToString()!="0") {
				yInd=4;
			}
			else if(y["RxNum"].ToString()!="0") {
				yInd=5;
			}
			else if(y["SheetNum"].ToString()!="0") {
				yInd=6;
			}
			if(xInd!=yInd) {
				return xInd.CompareTo(yInd);
			}//End sort by type------------------------------------------------------------------------------------------------------------------------------------
			//Sort procedures by status, priority, tooth region/num, proc code
			if(x["ProcNum"].ToString()!="0" && y["ProcNum"].ToString()!="0") {//if both are procedures
				return ProcedureLogic.CompareProcedures(x,y);
			}
			//nothing below this point can be a procedure.
			//dates are guaranteed to match at this point.
			//they are also guaranteed to be the same type (commlogs and tasks intermingled).
			//Sort other types by time-----------------------------------------------------------------------------------------------------------------------------
			if(((DateTime)x["ProcDate"])!=((DateTime)y["ProcDate"])){
			  return ((DateTime)x["ProcDate"]).CompareTo(((DateTime)y["ProcDate"]));
			}
			return 0;
		}

		///<summary>Returns a ProgNotesRowType for given row, also sets rowPK.</summary>
		public static ProgNotesRowType GetRowType(DataRow row,out long rowPK) {
			rowPK=-1;
			foreach(ProgNotesRowType rowType in Enum.GetValues(typeof(ProgNotesRowType))){
				if(rowType==ProgNotesRowType.None){
					continue;
				}
				string pkColumnName=EnumTools.GetAttributeOrDefault<ProgNotesRowAttribute>(rowType).PkColumnName;
				if(pkColumnName==null){
					throw new ApplicationException("Please set the rows ProgNotesRowAttribute.PKColumnName");
				}
				rowPK=PIn.Long(row[pkColumnName].ToString());
				if(rowPK==0){
					continue;
				}
				return rowType;
			}
			return ProgNotesRowType.None;
		}
	}

	public class ChartModuleFilters{
			public bool ShowAppointments;
			public bool ShowCommLog;
			public bool ShowCompleted;
			public bool ShowConditions;
			public bool ShowEmail;
			public bool ShowExisting;
			public bool ShowFamilyCommLog;
			public bool ShowSuperFamilyCommLog;
			public bool ShowFormPat;
			public bool ShowLabCases;
			public bool ShowProcNotes;
			public bool ShowReferred;
			public bool ShowRX;
			public bool ShowSheets;
			public bool ShowTasks;
			public bool ShowTreatPlan;

		///<summary>Serialization requires a parameterless constructor.</summary>
		public ChartModuleFilters() : this(true) { }

		///<summary>Sets all Module Components to defaultStatus.</summary>
		public ChartModuleFilters(bool isAllEnabled) {
			ShowAppointments=isAllEnabled;
			ShowCommLog=isAllEnabled;
			ShowCompleted=isAllEnabled;
			ShowConditions=isAllEnabled;
			ShowEmail=isAllEnabled;
			ShowExisting=isAllEnabled;
			ShowFamilyCommLog=isAllEnabled;
			ShowSuperFamilyCommLog=isAllEnabled;
			ShowFormPat=isAllEnabled;
			ShowLabCases=isAllEnabled;
			ShowProcNotes=isAllEnabled;
			ShowReferred=isAllEnabled;
			ShowRX=isAllEnabled;
			ShowSheets=isAllEnabled;
			ShowTasks=isAllEnabled;
			ShowTreatPlan=isAllEnabled;
		}

		///<summary>These are the various kinds of items that might show in the Progress Notes. This is the filter settings that the user controls.</summary>
		public ChartModuleFilters(
			bool showAppointments,
			bool showCommLog,
			bool showCompleted,
			bool showConditions,
			bool showEmail,
			bool showExisting,
			bool showFamilyCommLog,
			bool showSuperFamilyCommLog,
			bool showFormPat,
			bool showLabCases,
			bool showProcNotes,
			bool showReferred,
			bool showRX,
			bool showSheets,
			bool showTasks,
			bool showTreatPlan) 
		{
			ShowAppointments=showAppointments;
			ShowCommLog=showCommLog;
			ShowCompleted=showCompleted;
			ShowConditions=showConditions;
			ShowEmail=showEmail;
			ShowExisting=showExisting;
			ShowFamilyCommLog=showFamilyCommLog;
			ShowSuperFamilyCommLog=showSuperFamilyCommLog;
			ShowFormPat=showFormPat;
			ShowLabCases=showLabCases;
			ShowProcNotes=showProcNotes;
			ShowReferred=showReferred;
			ShowRX=showRX;
			ShowSheets=showSheets;
			ShowTasks=showTasks;
			ShowTreatPlan=showTreatPlan;
		}

	}


	//public class DtoChartModuleGetAll:DtoQueryBase {
	//	public int PatNum;
	//	public bool IsAuditMode;
	//}

	///<summary>Every row that shows in the Chart modules ProgNotes grid is associated to one of these types.</summary>
	public enum ProgNotesRowType {
		///<summary>0 - Does not define a real row type. Used as default.</summary>
		None,
		///<summary>1 - ProcNum is not 0.</summary>
		[ProgNotesRow("ProcNum")]
		Proc,
		///<summary>2 - CommLogNum is not 0.</summary>
		[ProgNotesRow("CommLogNum")]
		CommLog,
		///<summary>3 - WebChatSessionNum is not 0.</summary>
		[ProgNotesRow("WebChatSessionNum")]
		WebChatSession,
		///<summary>4 - RxNum is not 0.</summary>
		[ProgNotesRow("RxNum")]
		Rx,
		///<summary>5 - LabCaseNum is not 0.</summary>
		[ProgNotesRow("LabCaseNum")]
		LabCase,
		///<summary>6 - TaskNum is not 0.</summary>
		[ProgNotesRow("TaskNum")]
		Task,
		///<summary>7 - AptNum is not 0.</summary>
		[ProgNotesRow("AptNum")]
		Apt,
		///<summary>8 - EmailMessageNum is not 0.</summary>
		[ProgNotesRow("EmailMessageNum")]
		EmailMessage,
		///<summary>9 - SheetNum is not 0.</summary>
		[ProgNotesRow("SheetNum")]
		Sheet,
		///<summary>10 - FormPatNum is not 0.</summary>
		[ProgNotesRow("FormPatNum")]
		FormPat,
	}

	/// <summary>
	/// An attribute that defines the PK column for Chart modules ProgNotes grid.
	/// </summary>
	public class ProgNotesRowAttribute : Attribute {
		/// <summary>
		/// Column used when checking a row type.
		/// </summary>
		public string PkColumnName { get; set; }

		public ProgNotesRowAttribute() : this(null) {
			
		}

		public ProgNotesRowAttribute(string pkColumnName){
			this.PkColumnName=pkColumnName;
		}
	}
}
