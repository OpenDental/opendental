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
		public static long Insert(Automation auto) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				auto.AutomationNum=Meth.GetLong(MethodBase.GetCurrentMethod(),auto);
				return auto.AutomationNum;
			}
			return Crud.AutomationCrud.Insert(auto);
		}

		///<summary></summary>
		public static void Update(Automation auto) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),auto);
				return;
			}
			Crud.AutomationCrud.Update(auto);
		}

		///<summary></summary>
		public static void Delete(Automation auto) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),auto);
				return;
			}
			string command="DELETE FROM automation" 
				+" WHERE AutomationNum = "+POut.Long(auto.AutomationNum);
 			Db.NonQ(command);
		}

		public static bool Trigger<T>(AutomationTrigger trigger,List<string> procCodes,long patNum,
			Dictionary<long, Dictionary<long, DateTime>> dicBlockedAutomations,Action<string> actionOnShowMsg,Func<string,string,bool> funcYesNoMsgPrompt,
			Action<Commlog> actionOnShowCommLog,Action<Sheet> actionOnShowSheetFillEdit,Func<List<Procedure>,Image> funcCreateToothChartImage,
			long aptNum=0,T triggerObj=default(T))
		{
			if(patNum==0) {//Could happen for OpenPatient trigger
				return false;
			}
			List<Automation> listAutomations=Automations.GetDeepCopy();
			bool automationHappened=false;
			for(int i=0;i<listAutomations.Count;i++) {
				if(listAutomations[i].Autotrigger!=trigger) {
					continue;
				}
				if(trigger==AutomationTrigger.CompleteProcedure || trigger==AutomationTrigger.ScheduleProcedure) {
					if(procCodes==null || procCodes.Count==0) {
						continue;//fail silently
					}
					string[] arrayCodes=listAutomations[i].ProcCodes.Split(',');
					if(procCodes.All(x => !arrayCodes.Contains(x))) {
						continue;
					}
				}
				//matching automation item has been found
				//Get possible list of conditions that exist for this automation item
				List<AutomationCondition> autoConditionsList=AutomationConditions.GetListByAutomationNum(listAutomations[i].AutomationNum);
				if(autoConditionsList.Count>0 && !CheckAutomationConditions(autoConditionsList,patNum,triggerObj)) {
					continue;
				}
				SheetDef sheetDef;
				Sheet sheet;
				Appointment aptNew;
				Appointment aptOld;
				bool isPatApptSchedRestricted=PatRestrictions.IsRestricted(patNum,PatRestrict.ApptSchedule);
				switch(listAutomations[i].AutoAction) {
					case AutomationAction.CreateCommlog:
						#region CreateCommLog
						if(Plugins.HookMethod(null,"AutomationL.Trigger_CreateCommlog_start",patNum,aptNum,listAutomations[i].CommType,
							listAutomations[i].MessageContent,trigger))
						{
                automationHappened=true;
                continue;
            }
						Commlog commlogCur=new Commlog();
						commlogCur.PatNum=patNum;
						commlogCur.CommDateTime=DateTime.Now;
						commlogCur.CommType=listAutomations[i].CommType;
						commlogCur.Note=listAutomations[i].MessageContent;
						commlogCur.Mode_=CommItemMode.None;
						commlogCur.UserNum=Security.CurUser.UserNum;
						commlogCur.IsNew=true;
						actionOnShowCommLog.Invoke(commlogCur);
						automationHappened=true;
						#endregion CreateCommLog
						continue;
					case AutomationAction.PopUp:
						#region Popup
						actionOnShowMsg?.Invoke(Lans.g(nameof(Automations),listAutomations[i].MessageContent));
						automationHappened=true;
						#endregion Popup
						continue;
					case AutomationAction.PopUpThenDisable10Min:
						#region PopUpThenDisable10Min
						Plugins.HookAddCode(null,"AutomationL.Trigger_PopUpThenDisable10Min_begin",listAutomations[i],procCodes,patNum);
						long automationNum=listAutomations[i].AutomationNum;
						bool hasAutomationBlock=dicBlockedAutomations.ContainsKey(automationNum);
						if(hasAutomationBlock && dicBlockedAutomations[automationNum].ContainsKey(patNum)) {//Automation block exist for current patient.
							continue;
						}
						if(hasAutomationBlock){
							dicBlockedAutomations[automationNum].Add(patNum,DateTime.Now.AddMinutes(10));//Disable for 10 minutes.
						}
						else {//Add automationNum to higher level dictionary .
							dicBlockedAutomations.Add(automationNum,
								new Dictionary<long,DateTime>() 
								{ 
									{ patNum,DateTime.Now.AddMinutes(10) }//Disable for 10 minutes.
								});
						}
						actionOnShowMsg?.Invoke(listAutomations[i].MessageContent);
						automationHappened=true;
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
						actionOnShowSheetFillEdit.Invoke(sheet);
						automationHappened=true;
						#endregion Sheets
						continue;
					case AutomationAction.PrintReferralLetter:
						#region PrintReferralLetter
						long referralNum =RefAttaches.GetReferralNum(patNum);
						if(referralNum==0) {
							actionOnShowMsg?.Invoke(Lans.g(nameof(Automations),"This patient has no referral source entered."));
							automationHappened=true;
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
						actionOnShowSheetFillEdit.Invoke(sheet);
						automationHappened=true;
						#endregion PrintReferralLetter
						continue;
					case AutomationAction.SetApptASAP:
						#region SetApptASAP
						aptNew = Appointments.GetOneApt(aptNum);
						if(aptNew==null) {
							actionOnShowMsg?.Invoke(Lans.g(nameof(Automations),"Invalid appointment for automation."));
							automationHappened=true;
							continue;
						}
						aptOld=aptNew.Copy();
						aptNew.Priority=ApptPriority.ASAP;
						Appointments.Update(aptNew,aptOld);//Appointments S-Class handles Signalods
						continue;
					case AutomationAction.SetApptType:
						aptNew=Appointments.GetOneApt(aptNum);
						if(aptNew==null) {
							actionOnShowMsg?.Invoke(Lans.g(nameof(Automations),"Invalid appointment for automation."));
							automationHappened=true;
							continue;
						}
						aptOld=aptNew.Copy();
						aptNew.AppointmentTypeNum=listAutomations[i].AppointmentTypeNum;
						AppointmentType aptTypeCur=AppointmentTypes.GetFirstOrDefault(x => x.AppointmentTypeNum==aptNew.AppointmentTypeNum);
						if(aptTypeCur!=null) {
							aptNew.ColorOverride=aptTypeCur.AppointmentTypeColor;
							aptNew.Pattern=AppointmentTypes.GetTimePatternForAppointmentType(aptTypeCur);
							List<Procedure> listProcs=Appointments.ApptTypeMissingProcHelper(aptNew,aptTypeCur,new List<Procedure>());
							Procedures.UpdateAptNums(listProcs.Select(x => x.ProcNum).ToList(),aptNew.AptNum,aptNew.AptStatus==ApptStatus.Planned);
						}
						Appointments.Update(aptNew,aptOld);//Appointments S-Class handles Signalods
						#endregion SetApptASAP
						continue;
					case AutomationAction.PatRestrictApptSchedTrue:
						#region PatRestrictApptSchedTrue
						if(!Security.IsAuthorized(Permissions.PatientApptRestrict,true)) {
							SecurityLogs.MakeLogEntry(Permissions.PatientApptRestrict,patNum,"Attempt to restrict patient scheduling was blocked due to lack of user permission.");
							continue;
						}
						isPatApptSchedRestricted=PatRestrictions.IsRestricted(patNum,PatRestrict.ApptSchedule);
						PatRestrictions.Upsert(patNum,PatRestrict.ApptSchedule);
						PatRestrictions.InsertPatRestrictApptChangeSecurityLog(patNum,isPatApptSchedRestricted,PatRestrictions.IsRestricted(patNum,PatRestrict.ApptSchedule));
						automationHappened=true;
						continue;
					case AutomationAction.PatRestrictApptSchedFalse:
						if(!Security.IsAuthorized(Permissions.PatientApptRestrict,true)) {
							SecurityLogs.MakeLogEntry(Permissions.PatientApptRestrict,patNum,"Attempt to allow patient scheduling was blocked due to lack of user permission.");
							continue;
						}
						isPatApptSchedRestricted=PatRestrictions.IsRestricted(patNum,PatRestrict.ApptSchedule);
						PatRestrictions.RemovePatRestriction(patNum,PatRestrict.ApptSchedule);
						PatRestrictions.InsertPatRestrictApptChangeSecurityLog(patNum,isPatApptSchedRestricted,PatRestrictions.IsRestricted(patNum,PatRestrict.ApptSchedule));
						automationHappened=true;
						#endregion PatRestrictApptSchedTrue
						continue;
					case AutomationAction.PrintRxInstruction:
						#region PrintRxInstruction
						List<RxPat> listRx=(List<RxPat>)(object)triggerObj;
						if(listRx==null) {
							//Got here via a pre-existing trigger that doesn't pass in triggerObj.  We now block creation of automation triggers that could get 
							//here via code that does not pass in triggerObj.
							continue;
						}
						//We go through each new Rx where the patient note isn't blank.
						//There should only usually be one new rx, but we'll loop just in case.
						foreach(RxPat rx in listRx.Where(x => !string.IsNullOrWhiteSpace(x.PatientInstruction))){
							//This logic is an exact copy of FormRxManage.butPrintSelect_Click()'s logic when 1 Rx is selected.  
							//If this is updated, that method needs to be updated as well.
							sheetDef=SheetDefs.GetSheetDef(listAutomations[i].SheetDefNum);
							sheet=SheetUtil.CreateSheet(sheetDef,patNum);
							SheetParameter.SetParameter(sheet,"RxNum",rx.RxNum);
							SheetFiller.FillFields(sheet);
							SheetUtil.CalculateHeights(sheet);
							actionOnShowSheetFillEdit.Invoke(sheet);
							automationHappened=true;
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
							actionOnShowMsg?.Invoke(Lans.g(nameof(Automations),"Not allowed to change the status of a merged patient."));
							continue;
						}
						switch(pat.PatStatus) {
							case PatientStatus.Deceased:
								if(patOld.PatStatus!=PatientStatus.Deceased) {
									List<Appointment> listFutureAppts=Appointments.GetFutureSchedApts(pat.PatNum);
									if(listFutureAppts.Count>0) {
										string apptDates=string.Join("\r\n",listFutureAppts.Take(10).Select(x => x.AptDateTime.ToString()));
										if(listFutureAppts.Count>10) {
											apptDates+="(...)";
										}
										if(funcYesNoMsgPrompt.Invoke(
											Lans.g("FormPatientEdit","This patient has scheduled appointments in the future")+":\r\n"+apptDates+"\r\n"
												+Lans.g("FormPatientEdit","Would you like to delete them and set the patient to Deceased?"),
											Lans.g("FormPatientEdit","Delete future appointments?")))
										{
											foreach(Appointment appt in listFutureAppts) {
												Appointments.Delete(appt.AptNum,true);
											}
										}
										else {
											continue;
										}
									}
								}
								break;
						}
						//Re-activate or disable recalls depending on the the status that the patient is changing to.
						Patients.UpdateRecalls(pat,patOld,"ChangePatStatus automation");
						if(Patients.Update(pat,patOld)) {
							SecurityLogs.MakeLogEntry(Permissions.PatientEdit,patNum,"Patient status changed from "+patOld.PatStatus.GetDescription()+
								" to "+listAutomations[i].PatStatus.GetDescription()+" through ChangePatStatus automation.");
						}
						automationHappened=true;
						#endregion ChangePatStatus
						continue;
				}
			}
			return automationHappened;
		}

		private static bool CheckAutomationConditions<T>(List<AutomationCondition> autoConditionsList,long patNum,T triggerObj=default(T)) {
			//Make sure every condition returns true
			for(int i=0;i<autoConditionsList.Count;i++) {
				switch(autoConditionsList[i].CompareField) {
					case AutoCondField.NeedsSheet:
						if(NeedsSheet(autoConditionsList[i],patNum)) {
							return false;
						}
						break;
					case AutoCondField.Problem:
						if(!ProblemComparison(autoConditionsList[i],patNum))	{
							return false;
						}
						break;
					case AutoCondField.Medication:
						if(!MedicationComparison(autoConditionsList[i],patNum))	{
							return false;
						}
						break;
					case AutoCondField.Allergy:
						if(!AllergyComparison(autoConditionsList[i],patNum))	{
							return false;
						}
						break;
					case AutoCondField.Age:
						if(!AgeComparison(autoConditionsList[i],patNum)) {
							return false;
						}
						break;
					case AutoCondField.Gender:
						if(!GenderComparison(autoConditionsList[i],patNum)) {
							return false;
						}
						break;
					case AutoCondField.Labresult:
						if(!LabresultComparison(autoConditionsList[i],patNum)) {
							return false;
						}
						break;
					case AutoCondField.InsuranceNotEffective:
						if(!InsuranceNotEffectiveComparison(autoConditionsList[i],patNum)) {
							return false;
						}
						break;
					case AutoCondField.BillingType:
						if(!BillingTypeComparison(autoConditionsList[i],patNum)) {
							return false;
						}
						break;
					case AutoCondField.IsProcRequired:
						//ONLY TO BE USED FOR RxCreate AUTOMATION TRIGGER
						if(!IsProcRequiredComparison(autoConditionsList[i],patNum,triggerObj)) {
							return false;
						}
						break;
					case AutoCondField.IsControlled:
						//ONLY TO BE USED FOR RxCreate AUTOMATION TRIGGER
						if(!IsControlledComparison(autoConditionsList[i],patNum,triggerObj)) {
							return false;
						}
						break;
					case AutoCondField.IsPatientInstructionPresent:
						//ONLY TO BE USED FOR RxCreate AUTOMATION TRIGGER
						if(!IsPatientInstructionPresent(autoConditionsList[i],patNum,triggerObj)) {
							return false;
						}
						break;
					case AutoCondField.PlanNum:
						if(!PlanNumComparison(autoConditionsList[i],patNum)) {
							return false;
						}
						break;
					case AutoCondField.ClaimContainsProcCode:
						//ONLY TO BE USED FOR CreateClaim AND OpenClaim AUTOMATION TRIGGERS
						if(!DoesClaimContainProcCode(autoConditionsList[i],patNum,triggerObj)) {
							return false;
						}
						break;
				}
			}
			return true;
		}

		#region Comparisons
		private static bool NeedsSheet(AutomationCondition autoCond, long patNum) {
			List<Sheet> sheetList=Sheets.GetForPatientForToday(patNum);
			switch(autoCond.Comparison) {//Find out what operand to use.
				case AutoCondComparison.Equals:
					//Loop through every sheet to find one that matches the condition.
					for(int i=0;i<sheetList.Count;i++) {
						if(sheetList[i].Description==autoCond.CompareString) {//Operand based on AutoCondComparison.
							return true;
						}
					}
					break;
				case AutoCondComparison.Contains:
					for(int i=0;i<sheetList.Count;i++) {
						if(sheetList[i].Description.ToLower().Contains(autoCond.CompareString.ToLower())) {
							return true;
						}
					}
					break;
			}
			return false;
		}

		private static bool ProblemComparison(AutomationCondition autoCond,long patNum) {
			List<Disease> problemList=Diseases.Refresh(patNum,true);
			switch(autoCond.Comparison) {//Find out what operand to use.
				case AutoCondComparison.Equals:
					for(int i=0;i<problemList.Count;i++) {//Includes hidden
						if(DiseaseDefs.GetName(problemList[i].DiseaseDefNum)==autoCond.CompareString) {
							return true;
						}
					}
					break;
				case AutoCondComparison.Contains:
					for(int i=0;i<problemList.Count;i++) {
						if(DiseaseDefs.GetName(problemList[i].DiseaseDefNum).ToLower().Contains(autoCond.CompareString.ToLower())) {
							return true;
						}
					}
					break;
			}
			return false;
		}

		private static bool MedicationComparison(AutomationCondition autoCond,long patNum) {
			List<Medication> medList=Medications.GetMedicationsByPat(patNum);
			switch(autoCond.Comparison) {
				case AutoCondComparison.Equals:
					for(int i=0;i<medList.Count;i++) {
						if(medList[i].MedName==autoCond.CompareString) {
							return true;
						}
					}
					break;
				case AutoCondComparison.Contains:
					for(int i=0;i<medList.Count;i++) {
						if(medList[i].MedName.ToLower().Contains(autoCond.CompareString.ToLower())) {
							return true;
						}
					}
					break;
			}
			return false;
		}

		private static bool AllergyComparison(AutomationCondition autoCond,long patNum) {
			List<AllergyDef> listAllergyDefs=AllergyDefs.GetAllergyDefs(patNum,false);
			switch(autoCond.Comparison) {
				case AutoCondComparison.Equals:
					return listAllergyDefs.Any(x => x.Description==autoCond.CompareString);
				case AutoCondComparison.Contains:
					return listAllergyDefs.Any(x => x.Description.ToLower().Contains(autoCond.CompareString.ToLower()));
				default:
					return false;
			}
		}

		private static bool AgeComparison(AutomationCondition autoCond,long patNum) {
			Patient pat=Patients.GetPat(patNum);
			int age=pat.Age;
			int ageTrigger=0;
			if(!int.TryParse(autoCond.CompareString,out ageTrigger)){
				return false;//This is only possible due to an old bug that was fixed.
			}
			switch(autoCond.Comparison) {
				case AutoCondComparison.Equals:
					return (age==ageTrigger);
				case AutoCondComparison.Contains:
					return (age.ToString().Contains(autoCond.CompareString));
				case AutoCondComparison.GreaterThan:
					return (age>ageTrigger);
				case AutoCondComparison.LessThan:
					return (age<ageTrigger);
				default:
					return false;
			}
		}

		private static bool GenderComparison(AutomationCondition autoCond,long patNum) {
			Patient pat=Patients.GetPat(patNum);
			switch(autoCond.Comparison) {
				case AutoCondComparison.Equals:
					return (pat.Gender.ToString().Substring(0,1).ToLower()==autoCond.CompareString.ToLower());
				case AutoCondComparison.Contains:
					return (pat.Gender.ToString().Substring(0,1).ToLower().Contains(autoCond.CompareString.ToLower()));
				default:
					return false;
			}
		}

		private static bool LabresultComparison(AutomationCondition autoCond,long patNum) {
			List<LabResult> listResults=LabResults.GetAllForPatient(patNum);
			switch(autoCond.Comparison) {
				case AutoCondComparison.Equals:
					for(int i=0;i<listResults.Count;i++) {
						if(listResults[i].TestName==autoCond.CompareString) {
							return true;
						}
					}
					break;
				case AutoCondComparison.Contains:
					for(int i=0;i<listResults.Count;i++) {
						if(listResults[i].TestName.ToLower().Contains(autoCond.CompareString.ToLower())) {
							return true;
						}
					}
					break;
			}
			return false;
		}

		///<summary>Returns false if the insurance plan is effective.  True if today is outside of the insurance effective date range.</summary>
		private static bool InsuranceNotEffectiveComparison(AutomationCondition autoCond,long patNum) {
			PatPlan patPlanCur=PatPlans.GetPatPlan(patNum,1);
			if(patPlanCur==null) {
				return false;
			}
			InsSub insSubCur=InsSubs.GetOne(patPlanCur.InsSubNum);
			if(insSubCur==null) {
				return false;
			}
			if(DateTime.Today>=insSubCur.DateEffective && DateTime.Today<=insSubCur.DateTerm) {
				return false;//Allen - Not not effective
			}
			return true;
		}

		///<summary>Returns true if the patient's billing type matches the autocondition billing type.</summary>
		private static bool BillingTypeComparison(AutomationCondition autoCond,long patNum) {			
			Patient pat=Patients.GetPat(patNum);
			Def patBillType=Defs.GetDef(DefCat.BillingTypes,pat.BillingType);
			if(patBillType==null) {
				return false;
			}
			switch(autoCond.Comparison) {
				case AutoCondComparison.Equals:
					return patBillType.ItemName.ToLower()==autoCond.CompareString.ToLower();
				case AutoCondComparison.Contains:
					return patBillType.ItemName.ToLower().Contains(autoCond.CompareString.ToLower());
				default:
					return false;
			}
		}

		///<summary>Returns true if the patient is a RxPat and if IsProcRequired is true</summary>
		///ONLY TO BE USED FOR RxCreate AUTOMATION TRIGGER
		private static bool IsProcRequiredComparison<T>(AutomationCondition autoCond,long patNum,T triggerObj) {
			try {
				List<RxPat> listRx=(List<RxPat>)(object)triggerObj;
				return listRx.Any(x => x.IsProcRequired);
			}
			catch(Exception e) {
				e.DoNothing();
				return false;
			}
		}

		///<summary>Returns true if the patient is a RxPat and if IsControlled is true</summary>
		///ONLY TO BE USED FOR RxCreate AUTOMATION TRIGGER
		private static bool IsControlledComparison<T>(AutomationCondition autoCond,long patNum,T triggerObj) {
			try {
				List<RxPat> listRx=(List<RxPat>)(object)triggerObj;
				return listRx.Any(x => x.IsControlled);
			}
			catch(Exception e) {
				e.DoNothing();
				return false;
			}
		}
		
		///<summary>Returns true if at least one RxPat has a patient letter filled out.</summary>
		private static bool IsPatientInstructionPresent<T>(AutomationCondition autoCond,long patNum,T triggerObj) {
			try {
				List<RxPat> listRx=(List<RxPat>)(object)triggerObj;
				return listRx.Any(x => !String.IsNullOrWhiteSpace(x.PatientInstruction));
			}
			catch(Exception e) {
				e.DoNothing();
				return false;
			}
		}

		///<summary>Returns true if the patient's insurance plan ID matches the autocondition PlanNum.</summary>
		private static bool PlanNumComparison(AutomationCondition autoCond,long patNum) {
			List<PatPlan> listPatPlans=PatPlans.Refresh(patNum);
			if(listPatPlans.Count==0) {
				return false;
      }
			List<InsSub> listInsSubs=InsSubs.GetMany(listPatPlans.Select(x => x.InsSubNum).ToList());
			switch(autoCond.Comparison) {
				case AutoCondComparison.Equals:
					return listInsSubs.Any(x => x.PlanNum.ToString().ToLower()==autoCond.CompareString.ToLower());
				case AutoCondComparison.Contains:
					return listInsSubs.Any(x => x.PlanNum.ToString().ToLower().Contains(autoCond.CompareString.ToLower()));
				default:
					return false;
			}
		}

		///<summary>Returns true if the claim contains the ProcCode supplied.</summary>
		//ONLY TO BE USED FOR CreateClaim AND OpenClaim AUTOMATION TRIGGERS
		private static bool DoesClaimContainProcCode<T>(AutomationCondition autoCond,long patNum,T triggerObj) {
			try {
				List<ClaimProc> listClaimProcs=(List<ClaimProc>)(object)triggerObj;
				List<Procedure> listProcedures=Procedures.GetManyProc(listClaimProcs.Select(x => x.ProcNum).ToList(),includeNote:false);
				List<ProcedureCode> listProcedureCodesOnClaim=ProcedureCodes.GetCodesForCodeNums(listProcedures.Select(x => x.CodeNum).ToList());
				List<string> listProcCodes=listProcedureCodesOnClaim.Select(x => x.ProcCode).ToList();
				return listProcCodes.Contains(autoCond.CompareString);
			}
			catch(Exception e) {
				e.DoNothing();
				return false;
			}
		}
		#endregion
	}



}













