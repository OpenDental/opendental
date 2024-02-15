using CodeBase;
using OpenDentBusiness.SheetFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Automations {
		#region Cache Pattern

		private class AutomationCache : CacheListAbs<Automation> {
			protected override List<Automation> GetCacheFromDb() {
				string command="SELECT * FROM automation";
				return Crud.AutomationCrud.SelectMany(command);
			}
			protected override List<Automation> TableToList(DataTable table) {
				return Crud.AutomationCrud.TableToList(table);
			}
			protected override Automation Copy(Automation automation) {
				return automation.Copy();
			}
			protected override DataTable ListToTable(List<Automation> listAutomations) {
				return Crud.AutomationCrud.ListToTable(listAutomations,"Automation");
			}
			protected override void FillCacheIfNeeded() {
				Automations.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static AutomationCache _automationCache=new AutomationCache();

		public static List<Automation> GetDeepCopy(bool isShort=false) {
			return _automationCache.GetDeepCopy(isShort);
		}

		public static Automation GetFirstOrDefault(Func<Automation,bool> match,bool isShort=false) {
			return _automationCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_automationCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_automationCache.FillCacheFromTable(table);
				return table;
			}
			return _automationCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_automationCache.ClearCache();
		}
		#endregion Cache Pattern

		///<summary></summary>
		public static long Insert(Automation automation) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				automation.AutomationNum=Meth.GetLong(MethodBase.GetCurrentMethod(),automation);
				return automation.AutomationNum;
			}
			return Crud.AutomationCrud.Insert(automation);
		}

		///<summary></summary>
		public static void Update(Automation automation) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),automation);
				return;
			}
			Crud.AutomationCrud.Update(automation);
		}

		///<summary></summary>
		public static void Delete(Automation automation) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),automation);
				return;
			}
			string command="DELETE FROM automation" 
				+" WHERE AutomationNum = "+POut.Long(automation.AutomationNum);
 			Db.NonQ(command);
		}

		///<summary>Returns true if automation happened.</summary>
		public static bool Trigger<T>(
			EnumAutomationTrigger automationTrigger,
			List<string> listProcCodes,
			long patNum,
			Dictionary<long,Dictionary<long, DateTime>> dictionaryBlockedAutomations,
			Action<string> actionShowMsg,
			Func<string,string,bool> funcYesNoMsgPrompt,
			Action<Commlog> actionShowCommLog,
			Action<Sheet> actionShowSheetFillEdit,
			Func<List<Procedure>,Image> funcCreateToothChartImage,
			long aptNum=0,
			T triggerObj=default(T)) 
		{
			if(patNum==0) {//Could happen for OpenPatient trigger
				return false;
			}
			List<Automation> listAutomations=Automations.GetDeepCopy();
			bool didAutomationHappen=false;
			for(int i=0;i<listAutomations.Count;i++) {
				if(listAutomations[i].Autotrigger!=automationTrigger) {
					continue;
				}
				if(automationTrigger==EnumAutomationTrigger.ProcedureComplete || automationTrigger==EnumAutomationTrigger.ProcSchedule) {
					if(listProcCodes==null || listProcCodes.Count==0) {
						continue;//fail silently
					}
					string[] arrayCodes=listAutomations[i].ProcCodes.Split(',');
					if(listProcCodes.All(x => !arrayCodes.Contains(x))) {
						continue;
					}
				}
				//matching automation item has been found
				//Get possible list of conditions that exist for this automation item
				List<AutomationCondition> listAutomationConditions=AutomationConditions.GetListByAutomationNum(listAutomations[i].AutomationNum);
				if(listAutomationConditions.Count>0 && !CheckAutomationConditions(listAutomationConditions,patNum,triggerObj)) {
					continue;
				}
				SheetDef sheetDef;
				Sheet sheet;
				Appointment appointmentNew;
				Appointment appointmentOld;
				bool isPatApptSchedRestricted=PatRestrictions.IsRestricted(patNum,PatRestrict.ApptSchedule);
				switch(listAutomations[i].AutoAction) {
					case AutomationAction.CreateCommlog:
						#region CreateCommLog
						if(Plugins.HookMethod(null,"AutomationL.Trigger_CreateCommlog_start",patNum,aptNum,listAutomations[i].CommType,
							listAutomations[i].MessageContent,automationTrigger)) 
						{
							didAutomationHappen=true;
							continue;
						}
						Commlog commlog=new Commlog();
						commlog.PatNum=patNum;
						commlog.CommDateTime=DateTime.Now;
						commlog.CommType=listAutomations[i].CommType;
						commlog.Note=listAutomations[i].MessageContent;
						commlog.Mode_=CommItemMode.None;
						commlog.UserNum=Security.CurUser.UserNum;
						commlog.IsNew=true;
						actionShowCommLog.Invoke(commlog);
						didAutomationHappen=true;
						#endregion CreateCommLog
						continue;
					case AutomationAction.PopUp:
						#region Popup
						actionShowMsg?.Invoke(Lans.g(nameof(Automations),listAutomations[i].MessageContent));
						didAutomationHappen=true;
						#endregion Popup
						continue;
					case AutomationAction.PopUpThenDisable10Min:
						#region PopUpThenDisable10Min
						Plugins.HookAddCode(null,"AutomationL.Trigger_PopUpThenDisable10Min_begin",listAutomations[i],listProcCodes,patNum);
						long automationNum=listAutomations[i].AutomationNum;
						bool hasAutomationBlock=dictionaryBlockedAutomations.ContainsKey(automationNum);
						if(hasAutomationBlock && dictionaryBlockedAutomations[automationNum].ContainsKey(patNum)) {//Automation block exist for current patient.
							continue;
						}
						if(hasAutomationBlock){
							dictionaryBlockedAutomations[automationNum].Add(patNum,DateTime.Now.AddMinutes(10));//Disable for 10 minutes.
						}
						else {//Add automationNum to higher level dictionary .
							dictionaryBlockedAutomations.Add(automationNum,
								new Dictionary<long,DateTime>() 
								{ 
									{ patNum,DateTime.Now.AddMinutes(10) }//Disable for 10 minutes.
								});
						}
						actionShowMsg?.Invoke(listAutomations[i].MessageContent);
						didAutomationHappen=true;
						#endregion PopUpThenDisable10Min
						continue;
					case AutomationAction.PrintPatientLetter:
					case AutomationAction.ShowExamSheet:
					case AutomationAction.ShowConsentForm:
						#region Sheets
						sheetDef=SheetDefs.GetSheetDef(listAutomations[i].SheetDefNum);
						sheet=SheetUtil.CreateSheet(sheetDef,patNum);
						SheetParameter.SetParameter(sheet,"PatNum",patNum);
						SheetFiller.FillFields(sheet);
						SheetUtil.CalculateHeights(sheet);
						actionShowSheetFillEdit.Invoke(sheet);
						didAutomationHappen=true;
						#endregion Sheets
						continue;
					case AutomationAction.PrintReferralLetter:
						#region PrintReferralLetter
						long referralNum =RefAttaches.GetReferralNum(patNum);
						if(referralNum==0) {
							actionShowMsg?.Invoke(Lans.g(nameof(Automations),"This patient has no referral source entered."));
							didAutomationHappen=true;
							continue;
						}
						sheetDef=SheetDefs.GetSheetDef(listAutomations[i].SheetDefNum);
						sheet=SheetUtil.CreateSheet(sheetDef,patNum);
						SheetParameter.SetParameter(sheet,"PatNum",patNum);
						SheetParameter.SetParameter(sheet,"ReferralNum",referralNum);
						//Don't fill these params if the sheet doesn't use them.
						if(sheetDef.SheetFieldDefs.Any(x =>
							(x.FieldType==SheetFieldType.Grid && x.FieldName=="ReferralLetterProceduresCompleted")
							|| (x.FieldType==SheetFieldType.Special && x.FieldName=="toothChart")))
						{
							List<Procedure> listProcs=Procedures.GetCompletedForDateRange(DateTime.Today,DateTime.Today
								,listPatNums:new List<long>() { patNum }
								,includeNote:true
								,includeGroupNote:true
							);
							if(sheetDef.SheetFieldDefs.Any(x => x.FieldType==SheetFieldType.Grid && x.FieldName=="ReferralLetterProceduresCompleted")) {
								SheetParameter.SetParameter(sheet,"CompletedProcs",listProcs);
							}
							if(sheetDef.SheetFieldDefs.Any(x => x.FieldType==SheetFieldType.Special && x.FieldName=="toothChart")) {
								SheetParameter.SetParameter(sheet,"toothChartImg",funcCreateToothChartImage.Invoke(listProcs));
							}
						}
						SheetFiller.FillFields(sheet);
						SheetUtil.CalculateHeights(sheet);
						actionShowSheetFillEdit.Invoke(sheet);
						didAutomationHappen=true;
						#endregion PrintReferralLetter
						continue;
					case AutomationAction.SetApptASAP:
						#region SetApptASAP
						appointmentNew = Appointments.GetOneApt(aptNum);
						if(appointmentNew==null) {
							actionShowMsg?.Invoke(Lans.g(nameof(Automations),"Invalid appointment for automation."));
							didAutomationHappen=true;
							continue;
						}
						appointmentOld=appointmentNew.Copy();
						appointmentNew.Priority=ApptPriority.ASAP;
						Appointments.Update(appointmentNew,appointmentOld);//Appointments S-Class handles Signalods
						continue;
					case AutomationAction.SetApptType:
						appointmentNew=Appointments.GetOneApt(aptNum);
						if(appointmentNew==null) {
							actionShowMsg?.Invoke(Lans.g(nameof(Automations),"Invalid appointment for automation."));
							didAutomationHappen=true;
							continue;
						}
						appointmentOld=appointmentNew.Copy();
						appointmentNew.AppointmentTypeNum=listAutomations[i].AppointmentTypeNum;
						AppointmentType appointmentType=AppointmentTypes.GetFirstOrDefault(x => x.AppointmentTypeNum==appointmentNew.AppointmentTypeNum);
						if(appointmentType!=null) {
							appointmentNew.ColorOverride=appointmentType.AppointmentTypeColor;
							appointmentNew.Pattern=AppointmentTypes.GetTimePatternForAppointmentType(appointmentType);
							List<Procedure> listProcs=Appointments.ApptTypeMissingProcHelper(appointmentNew,appointmentType,new List<Procedure>());
							Procedures.UpdateAptNums(listProcs.Select(x => x.ProcNum).ToList(),appointmentNew.AptNum,appointmentNew.AptStatus==ApptStatus.Planned);
						}
						Appointments.Update(appointmentNew,appointmentOld);//Appointments S-Class handles Signalods
						#endregion SetApptASAP
						continue;
					case AutomationAction.PatRestrictApptSchedTrue:
						#region PatRestrictApptSchedTrue
						if(!Security.IsAuthorized(EnumPermType.PatientApptRestrict,true)) {
							SecurityLogs.MakeLogEntry(EnumPermType.PatientApptRestrict,patNum,"Attempt to restrict patient scheduling was blocked due to lack of user permission.");
							continue;
						}
						isPatApptSchedRestricted=PatRestrictions.IsRestricted(patNum,PatRestrict.ApptSchedule);
						PatRestrictions.Upsert(patNum,PatRestrict.ApptSchedule);
						PatRestrictions.InsertPatRestrictApptChangeSecurityLog(patNum,isPatApptSchedRestricted,PatRestrictions.IsRestricted(patNum,PatRestrict.ApptSchedule));
						didAutomationHappen=true;
						continue;
					case AutomationAction.PatRestrictApptSchedFalse:
						if(!Security.IsAuthorized(EnumPermType.PatientApptRestrict,true)) {
							SecurityLogs.MakeLogEntry(EnumPermType.PatientApptRestrict,patNum,"Attempt to allow patient scheduling was blocked due to lack of user permission.");
							continue;
						}
						isPatApptSchedRestricted=PatRestrictions.IsRestricted(patNum,PatRestrict.ApptSchedule);
						PatRestrictions.RemovePatRestriction(patNum,PatRestrict.ApptSchedule);
						PatRestrictions.InsertPatRestrictApptChangeSecurityLog(patNum,isPatApptSchedRestricted,PatRestrictions.IsRestricted(patNum,PatRestrict.ApptSchedule));
						didAutomationHappen=true;
						#endregion PatRestrictApptSchedTrue
						continue;
					case AutomationAction.PrintRxInstruction:
						#region PrintRxInstruction
						List<RxPat> listRxPats=(List<RxPat>)(object)triggerObj;
						if(listRxPats==null) {
							//Got here via a pre-existing trigger that doesn't pass in triggerObj.  We now block creation of automation triggers that could get 
							//here via code that does not pass in triggerObj.
							continue;
						}
						//We go through each new Rx where the patient note isn't blank.
						//There should only usually be one new rx, but we'll loop just in case.
						List<RxPat> listRxPatsWithNotes = listRxPats.FindAll(x => !string.IsNullOrWhiteSpace(x.PatientInstruction));
						for(int j=0;j<listRxPatsWithNotes.Count;j++){
							//This logic is an exact copy of FormRxManage.butPrintSelect_Click()'s logic when 1 Rx is selected.  
							//If this is updated, that method needs to be updated as well.
							sheetDef=SheetDefs.GetSheetDef(listAutomations[i].SheetDefNum);
							sheet=SheetUtil.CreateSheet(sheetDef,patNum);
							SheetParameter.SetParameter(sheet,"RxNum",listRxPatsWithNotes[j].RxNum);
							SheetFiller.FillFields(sheet);
							SheetUtil.CalculateHeights(sheet);
							actionShowSheetFillEdit.Invoke(sheet);
							didAutomationHappen=true;
						}
						#endregion PrintRxInstruction
						continue;
					case AutomationAction.ChangePatStatus:
						#region ChangePatStatus
						Patient pat =Patients.GetPat(patNum);
						Patient patOld=pat.Copy();
						pat.PatStatus=listAutomations[i].PatStatus;
						//Don't allow changing status from Archived if this is a merged patient.
						if(patOld.PatStatus!=pat.PatStatus 
							&& patOld.PatStatus==PatientStatus.Archived 
							&& PatientLinks.WasPatientMerged(patOld.PatNum))
						{
							actionShowMsg?.Invoke(Lans.g(nameof(Automations),"Not allowed to change the status of a merged patient."));
							continue;
						}
						switch(pat.PatStatus) {
							case PatientStatus.Deceased:
								if(patOld.PatStatus==PatientStatus.Deceased) {
									break;
								}
								List<Appointment> listFutureAppts=Appointments.GetFutureSchedApts(pat.PatNum);
								if(listFutureAppts.Count<=0) {
									break;
								}
								string apptDates=string.Join("\r\n",listFutureAppts.Take(10).Select(x => x.AptDateTime.ToString()));
								if(listFutureAppts.Count>10) {
									apptDates+="(...)";
								}
								if(!funcYesNoMsgPrompt.Invoke(
									Lans.g("FormPatientEdit","This patient has scheduled appointments in the future")+":\r\n"+apptDates+"\r\n"
										+Lans.g("FormPatientEdit","Would you like to delete them and set the patient to Deceased?"),
									Lans.g("FormPatientEdit","Delete future appointments?")))
								{
									continue;
								}
								for(int j=0;j<listFutureAppts.Count;j++){
									Appointments.Delete(listFutureAppts[j].AptNum,true);
								}
								break;
						}
						//Re-activate or disable recalls depending on the the status that the patient is changing to.
						Patients.UpdateRecalls(pat,patOld,"ChangePatStatus automation");
						if(Patients.Update(pat,patOld)) {
							SecurityLogs.MakeLogEntry(EnumPermType.PatientEdit,patNum,"Patient status changed from "+patOld.PatStatus.GetDescription()+
								" to "+listAutomations[i].PatStatus.GetDescription()+" through ChangePatStatus automation.");
						}
						didAutomationHappen=true;
						#endregion ChangePatStatus
						continue;
				}
			}
			return didAutomationHappen;
		}

		private static bool CheckAutomationConditions<T>(List<AutomationCondition> listAutomationConditions,long patNum,T triggerObj=default(T)) {
			//Make sure every condition returns true
			for(int i=0;i<listAutomationConditions.Count;i++) {
				switch(listAutomationConditions[i].CompareField) {
					case AutoCondField.NeedsSheet:
						if(NeedsSheet(listAutomationConditions[i],patNum)) {
							return false;
						}
						break;
					case AutoCondField.Problem:
						if(!ProblemComparison(listAutomationConditions[i],patNum))	{
							return false;
						}
						break;
					case AutoCondField.Medication:
						if(!MedicationComparison(listAutomationConditions[i],patNum))	{
							return false;
						}
						break;
					case AutoCondField.Allergy:
						if(!AllergyComparison(listAutomationConditions[i],patNum))	{
							return false;
						}
						break;
					case AutoCondField.Age:
						if(!AgeComparison(listAutomationConditions[i],patNum)) {
							return false;
						}
						break;
					case AutoCondField.Gender:
						if(!GenderComparison(listAutomationConditions[i],patNum)) {
							return false;
						}
						break;
					case AutoCondField.Labresult:
						if(!LabresultComparison(listAutomationConditions[i],patNum)) {
							return false;
						}
						break;
					case AutoCondField.InsuranceNotEffective:
						if(!InsuranceNotEffectiveComparison(listAutomationConditions[i],patNum)) {
							return false;
						}
						break;
					case AutoCondField.BillingType:
						if(!BillingTypeComparison(listAutomationConditions[i],patNum)) {
							return false;
						}
						break;
					case AutoCondField.IsProcRequired:
						//ONLY TO BE USED FOR RxCreate AUTOMATION TRIGGER
						if(!IsProcRequiredComparison(listAutomationConditions[i],patNum,triggerObj)) {
							return false;
						}
						break;
					case AutoCondField.IsControlled:
						//ONLY TO BE USED FOR RxCreate AUTOMATION TRIGGER
						if(!IsControlledComparison(listAutomationConditions[i],patNum,triggerObj)) {
							return false;
						}
						break;
					case AutoCondField.IsPatientInstructionPresent:
						//ONLY TO BE USED FOR RxCreate AUTOMATION TRIGGER
						if(!IsPatientInstructionPresent(listAutomationConditions[i],patNum,triggerObj)) {
							return false;
						}
						break;
					case AutoCondField.PlanNum:
						if(!PlanNumComparison(listAutomationConditions[i],patNum)) {
							return false;
						}
						break;
					case AutoCondField.ClaimContainsProcCode:
						//ONLY TO BE USED FOR CreateClaim AND OpenClaim AUTOMATION TRIGGERS
						if(!DoesClaimContainProcCode(listAutomationConditions[i],patNum,triggerObj)) {
							return false;
						}
						break;
				}
			}
			return true;
		}

		#region Comparisons
		private static bool NeedsSheet(AutomationCondition automationCondition, long patNum) {
			List<Sheet> listSheets=Sheets.GetForPatientForToday(patNum);
			switch(automationCondition.Comparison) {//Find out what operand to use.
				case AutoCondComparison.Equals:
					//Loop through every sheet to find one that matches the condition.
					for(int i=0;i<listSheets.Count;i++) {
						if(listSheets[i].Description==automationCondition.CompareString) {//Operand based on AutoCondComparison.
							return true;
						}
					}
					break;
				case AutoCondComparison.Contains:
					for(int i=0;i<listSheets.Count;i++) {
						if(listSheets[i].Description.ToLower().Contains(automationCondition.CompareString.ToLower())) {
							return true;
						}
					}
					break;
			}
			return false;
		}

		private static bool ProblemComparison(AutomationCondition automationCondition,long patNum) {
			List<Disease> listDiseases=Diseases.Refresh(patNum,true);
			switch(automationCondition.Comparison) {//Find out what operand to use.
				case AutoCondComparison.Equals:
					for(int i=0;i<listDiseases.Count;i++) {//Includes hidden
						if(DiseaseDefs.GetName(listDiseases[i].DiseaseDefNum)==automationCondition.CompareString) {
							return true;
						}
					}
					break;
				case AutoCondComparison.Contains:
					for(int i=0;i<listDiseases.Count;i++) {
						if(DiseaseDefs.GetName(listDiseases[i].DiseaseDefNum).ToLower().Contains(automationCondition.CompareString.ToLower())) {
							return true;
						}
					}
					break;
			}
			return false;
		}

		private static bool MedicationComparison(AutomationCondition automationCondition,long patNum) {
			List<Medication> listMedication=Medications.GetMedicationsByPat(patNum);
			switch(automationCondition.Comparison) {
				case AutoCondComparison.Equals:
					for(int i=0;i<listMedication.Count;i++) {
						if(listMedication[i].MedName==automationCondition.CompareString) {
							return true;
						}
					}
					break;
				case AutoCondComparison.Contains:
					for(int i=0;i<listMedication.Count;i++) {
						if(listMedication[i].MedName.ToLower().Contains(automationCondition.CompareString.ToLower())) {
							return true;
						}
					}
					break;
			}
			return false;
		}

		private static bool AllergyComparison(AutomationCondition automationCondition,long patNum) {
			List<AllergyDef> listAllergyDefs=AllergyDefs.GetAllergyDefs(patNum,false);
			switch(automationCondition.Comparison) {
				case AutoCondComparison.Equals:
					return listAllergyDefs.Any(x => x.Description==automationCondition.CompareString);
				case AutoCondComparison.Contains:
					return listAllergyDefs.Any(x => x.Description.ToLower().Contains(automationCondition.CompareString.ToLower()));
				default:
					return false;
			}
		}

		private static bool AgeComparison(AutomationCondition automationCondition,long patNum) {
			Patient pat=Patients.GetPat(patNum);
			int age=pat.Age;
			int ageTrigger=0;
			if(!int.TryParse(automationCondition.CompareString,out ageTrigger)){
				return false;//This is only possible due to an old bug that was fixed.
			}
			switch(automationCondition.Comparison) {
				case AutoCondComparison.Equals:
					return (age==ageTrigger);
				case AutoCondComparison.Contains:
					return (age.ToString().Contains(automationCondition.CompareString));
				case AutoCondComparison.GreaterThan:
					return (age>ageTrigger);
				case AutoCondComparison.LessThan:
					return (age<ageTrigger);
				default:
					return false;
			}
		}

		private static bool GenderComparison(AutomationCondition automationCondition,long patNum) {
			Patient pat=Patients.GetPat(patNum);
			switch(automationCondition.Comparison) {
				case AutoCondComparison.Equals:
					return (pat.Gender.ToString().Substring(0,1).ToLower()==automationCondition.CompareString.ToLower());
				case AutoCondComparison.Contains:
					return (pat.Gender.ToString().Substring(0,1).ToLower().Contains(automationCondition.CompareString.ToLower()));
				default:
					return false;
			}
		}

		private static bool LabresultComparison(AutomationCondition automationCondition,long patNum) {
			List<LabResult> listLabResults=LabResults.GetAllForPatient(patNum);
			switch(automationCondition.Comparison) {
				case AutoCondComparison.Equals:
					for(int i=0;i<listLabResults.Count;i++) {
						if(listLabResults[i].TestName==automationCondition.CompareString) {
							return true;
						}
					}
					break;
				case AutoCondComparison.Contains:
					for(int i=0;i<listLabResults.Count;i++) {
						if(listLabResults[i].TestName.ToLower().Contains(automationCondition.CompareString.ToLower())) {
							return true;
						}
					}
					break;
			}
			return false;
		}

		///<summary>Returns false if the insurance plan is effective.  True if today is outside of the insurance effective date range.</summary>
		private static bool InsuranceNotEffectiveComparison(AutomationCondition automationCondition,long patNum) {
			PatPlan patPlan=PatPlans.GetPatPlan(patNum,1);
			if(patPlan==null) {
				return false;
			}
			InsSub insSub=InsSubs.GetOne(patPlan.InsSubNum);
			if(insSub==null) {
				return false;
			}
			if(DateTime.Today>=insSub.DateEffective && DateTime.Today<=insSub.DateTerm) {
				return false;//Allen - Not not effective
			}
			return true;
		}

		///<summary>Returns true if the patient's billing type matches the autocondition billing type.</summary>
		private static bool BillingTypeComparison(AutomationCondition automationCondition,long patNum) {
			Patient patient=Patients.GetPat(patNum);
			Def defBillType=Defs.GetDef(DefCat.BillingTypes,patient.BillingType);
			if(defBillType==null) {
				return false;
			}
			switch(automationCondition.Comparison) {
				case AutoCondComparison.Equals:
					return defBillType.ItemName.ToLower()==automationCondition.CompareString.ToLower();
				case AutoCondComparison.Contains:
					return defBillType.ItemName.ToLower().Contains(automationCondition.CompareString.ToLower());
				default:
					return false;
			}
		}

		///<summary>Returns true if the patient is a RxPat and if IsProcRequired is true</summary>
		///ONLY TO BE USED FOR RxCreate AUTOMATION TRIGGER
		private static bool IsProcRequiredComparison<T>(AutomationCondition automationCondition,long patNum,T triggerObj) {
			try {
				List<RxPat> listRxPats=(List<RxPat>)(object)triggerObj;
				return listRxPats.Any(x => x.IsProcRequired);
			}
			catch(Exception e) {
				e.DoNothing();
				return false;
			}
		}

		///<summary>Returns true if the patient is a RxPat and if IsControlled is true</summary>
		///ONLY TO BE USED FOR RxCreate AUTOMATION TRIGGER
		private static bool IsControlledComparison<T>(AutomationCondition automationCondition,long patNum,T triggerObj) {
			try {
				List<RxPat> listRxPats=(List<RxPat>)(object)triggerObj;
				return listRxPats.Any(x => x.IsControlled);
			}
			catch(Exception e) {
				e.DoNothing();
				return false;
			}
		}
		
		///<summary>Returns true if at least one RxPat has a patient letter filled out.</summary>
		private static bool IsPatientInstructionPresent<T>(AutomationCondition automationCondition,long patNum,T triggerObj) {
			try {
				List<RxPat> listRxPats=(List<RxPat>)(object)triggerObj;
				return listRxPats.Any(x => !String.IsNullOrWhiteSpace(x.PatientInstruction));
			}
			catch(Exception e) {
				e.DoNothing();
				return false;
			}
		}

		///<summary>Returns true if the patient's insurance plan ID matches the autocondition PlanNum.</summary>
		private static bool PlanNumComparison(AutomationCondition automationCondition,long patNum) {
			List<PatPlan> listPatPlans=PatPlans.Refresh(patNum);
			if(listPatPlans.Count==0) {
				return false;
			}
			List<InsSub> listInsSubs=InsSubs.GetMany(listPatPlans.Select(x => x.InsSubNum).ToList());
			switch(automationCondition.Comparison) {
				case AutoCondComparison.Equals:
					return listInsSubs.Any(x => x.PlanNum.ToString().ToLower()==automationCondition.CompareString.ToLower());
				case AutoCondComparison.Contains:
					return listInsSubs.Any(x => x.PlanNum.ToString().ToLower().Contains(automationCondition.CompareString.ToLower()));
				default:
					return false;
			}
		}

		///<summary>Returns true if the claim contains the ProcCode supplied.</summary>
		//ONLY TO BE USED FOR CreateClaim AND OpenClaim AUTOMATION TRIGGERS
		private static bool DoesClaimContainProcCode<T>(AutomationCondition automationCondition,long patNum,T triggerObj) {
			try {
				List<ClaimProc> listClaimProcs=(List<ClaimProc>)(object)triggerObj;
				List<Procedure> listProcedures=Procedures.GetManyProc(listClaimProcs.Select(x => x.ProcNum).ToList(),includeNote:false);
				List<ProcedureCode> listProcedureCodesOnClaim=ProcedureCodes.GetCodesForCodeNums(listProcedures.Select(x => x.CodeNum).ToList());
				List<string> listProcCodes=listProcedureCodesOnClaim.Select(x => x.ProcCode).ToList();
				return listProcCodes.Contains(automationCondition.CompareString);
			}
			catch(Exception e) {
				e.DoNothing();
				return false;
			}
		}
		#endregion
	}



}