using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness {
	public class StaticTextFields {

		///<summary>Returns a list of StaticTextField available for the sheet type passed in. Exludes obsolete fields.</summary>
		public static List<StaticTextField> GetForType(SheetTypeEnum sheetType) {
			List<StaticTextField> listFields=Enum.GetValues(typeof(StaticTextField))
				.Cast<StaticTextField>()
				.Where(x => !x.IsStaticTextFieldObsolete())
				.ToList();
			//Not including patientPortalCredentials because simply viewing the dashboard would create a username and password for the patient
			if(SheetDefs.IsDashboardType(sheetType)) {
				listFields.RemoveAll(x => ListTools.In(x,StaticTextField.patientPortalCredentials));
			}
			//Remove fields that should not be available for the sheet type passed in.
			if(!ListTools.In(sheetType,SheetTypeEnum.PatientLetter,SheetTypeEnum.ReferralLetter)) {
				listFields.RemoveAll(x => ListTools.In(x,StaticTextField.apptDateMonthSpelled,StaticTextField.apptProcs,StaticTextField.apptProvNameFormal));
			}
			return listFields;
		}

	}

	public class StaticTextData {
		//Any new fields/lists added to this object must also be implemented in UserControlDashboard.PatientDashboardDataEventArgs.CreateStaticTextData()
		public PatientNote PatNote;
		public List<RefAttach> ListRefAttaches;
		public List<InsSub> ListInsSubs;
		public List<InsPlan> ListInsPlans;
		public List<PatPlan> ListPatPlans;
		public List<Benefit> ListBenefits;
		public List<ClaimProcHist> HistList;
		public List<TreatPlan> ListTreatPlans;
		public List<Recall> ListRecallsForFam;
		public List<Appointment> ListAppts;
		public List<Appointment> ListFutureApptsForFam;
		public List<Disease> ListDiseases;
		public List<Allergy> ListAllergies;
		public List<MedicationPat> ListMedicationPats;
		public List<Popup> ListFamPopups;
		///<summary>Only contains the procedures for the code nums passed in.</summary>
		public List<Procedure> ListProceduresSome;
		public List<Document> ListDocuments;
		public List<Procedure> ListProceduresPat;
		public List<PlannedAppt> ListPlannedAppts;
		public List<Procedure> ListSelectedTpProcs;
		///<summary>Dictionary linking static text field names to the data fields in StaticTextData that are required to determine the value for
		///the static text field in a Sheet.</summary>
		private static readonly Dictionary<StaticTextField,StaticTextFieldDependency> _dictDependencies
			=new Dictionary<StaticTextField,StaticTextFieldDependency>() 
		{
			{StaticTextField.activeAllergies,StaticTextFieldDependency.ListAllergies},
			{StaticTextField.activeProblems,StaticTextFieldDependency.ListDiseases},
			{StaticTextField.address,StaticTextFieldDependency.Pat},
			{StaticTextField.apptsAllFuture,StaticTextFieldDependency.ListFutureApptsForFam},
			{StaticTextField.apptDateMonthSpelled,StaticTextFieldDependency.ListAppts},
			{StaticTextField.apptModNote,StaticTextFieldDependency.Pat},
			{StaticTextField.apptProcs,StaticTextFieldDependency.ListAppts | StaticTextFieldDependency.ListProceduresPat},
			{StaticTextField.apptProvNameFormal,StaticTextFieldDependency.ListAppts},
			{StaticTextField.age,StaticTextFieldDependency.Pat},
			{StaticTextField.balTotal,StaticTextFieldDependency.Fam},
			{StaticTextField.bal_0_30,StaticTextFieldDependency.Fam},
			{StaticTextField.bal_31_60,StaticTextFieldDependency.Fam},
			{StaticTextField.bal_61_90,StaticTextFieldDependency.Fam},
			{StaticTextField.balOver90,StaticTextFieldDependency.Fam},
			{StaticTextField.balInsEst,StaticTextFieldDependency.Fam},
			{StaticTextField.balTotalMinusInsEst,StaticTextFieldDependency.Fam},
			{StaticTextField.BillingType,StaticTextFieldDependency.Pat},
			{StaticTextField.Birthdate,StaticTextFieldDependency.Pat},
			{StaticTextField.carrierName,StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsSubs 
				| StaticTextFieldDependency.ListInsPlans},
			{StaticTextField.carrier2Name,StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsSubs 
				| StaticTextFieldDependency.ListInsPlans},
			{StaticTextField.ChartNumber,StaticTextFieldDependency.Pat},
			{StaticTextField.carrierAddress,StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsSubs 
				| StaticTextFieldDependency.ListInsPlans},
			{StaticTextField.carrier2Address,StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsSubs 
				| StaticTextFieldDependency.ListInsPlans},
			{StaticTextField.carrierCityStZip,StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsSubs 
				| StaticTextFieldDependency.ListInsPlans},
			{StaticTextField.carrier2CityStZip,StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsSubs 
				| StaticTextFieldDependency.ListInsPlans},
			{StaticTextField.cityStateZip,StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsSubs 
				| StaticTextFieldDependency.ListInsPlans},
			{StaticTextField.clinicDescription,StaticTextFieldDependency.Pat},
			{StaticTextField.clinicAddress,StaticTextFieldDependency.Pat},
			{StaticTextField.clinicCityStZip,StaticTextFieldDependency.Pat},
			{StaticTextField.clinicPhone,StaticTextFieldDependency.Pat},
			{StaticTextField.clinicPatDescription,StaticTextFieldDependency.Pat},
			{StaticTextField.clinicPatAddress,StaticTextFieldDependency.Pat},
			{StaticTextField.clinicPatCityStZip,StaticTextFieldDependency.Pat},
			{StaticTextField.clinicPatPhone,StaticTextFieldDependency.Pat},
			{StaticTextField.clinicCurDescription,StaticTextFieldDependency.Pat},
			{StaticTextField.clinicCurAddress,StaticTextFieldDependency.Pat},
			{StaticTextField.clinicCurCityStZip,StaticTextFieldDependency.Pat},
			{StaticTextField.clinicCurPhone,StaticTextFieldDependency.Pat},
			{StaticTextField.currentMedications,StaticTextFieldDependency.ListMedicationPats},
			{StaticTextField.DateFirstVisit,StaticTextFieldDependency.Pat},
			{StaticTextField.dateLastAppt,StaticTextFieldDependency.ListAppts},
			{StaticTextField.dateLastBW,StaticTextFieldDependency.ListProceduresSome},
			{StaticTextField.dateLastExam,StaticTextFieldDependency.ListProceduresSome},
			{StaticTextField.dateLastPerio,StaticTextFieldDependency.ListProceduresSome},
			{StaticTextField.dateLastPanoFMX,StaticTextFieldDependency.ListProceduresSome},
			{StaticTextField.dateLastProphy,StaticTextFieldDependency.ListProceduresSome},
			{StaticTextField.dateLastSrp,StaticTextFieldDependency.ListProceduresSome},
			{StaticTextField.dateOfLastSavedTP,StaticTextFieldDependency.ListTreatPlans},
			{StaticTextField.dateRecallDue,StaticTextFieldDependency.Pat | StaticTextFieldDependency.ListRecallsForFam},
			{StaticTextField.dateTimeLastAppt,StaticTextFieldDependency.ListAppts},
			{StaticTextField.dueForBWYN,StaticTextFieldDependency.ListRecallsForFam},
			{StaticTextField.dueForPanoYN,StaticTextFieldDependency.ListRecallsForFam},
			{StaticTextField.Email,StaticTextFieldDependency.Pat},
			{StaticTextField.famFinNote,StaticTextFieldDependency.PatNote},
			{StaticTextField.famFinUrgNote,StaticTextFieldDependency.Fam},
			{StaticTextField.famRecallDue,StaticTextFieldDependency.Fam | StaticTextFieldDependency.ListRecallsForFam},
			{StaticTextField.guarantorHmPhone,StaticTextFieldDependency.Fam},
			{StaticTextField.guarantorNameF,StaticTextFieldDependency.Fam},
			{StaticTextField.guarantorNameFL,StaticTextFieldDependency.Fam},
			{StaticTextField.guarantorNameL,StaticTextFieldDependency.Fam},
			{StaticTextField.guarantorNamePref,StaticTextFieldDependency.Fam},
			{StaticTextField.guarantorNameLF,StaticTextFieldDependency.Fam},
			{StaticTextField.guarantorWirelessPhone,StaticTextFieldDependency.Fam},
			{StaticTextField.guarantorWkPhone,StaticTextFieldDependency.Fam},
			{StaticTextField.gender,StaticTextFieldDependency.Pat},
			{StaticTextField.genderHeShe,StaticTextFieldDependency.Pat},
			{StaticTextField.genderheshe,StaticTextFieldDependency.Pat},
			{StaticTextField.genderHimHer,StaticTextFieldDependency.Pat},
			{StaticTextField.genderhimher,StaticTextFieldDependency.Pat},
			{StaticTextField.genderHimselfHerself,StaticTextFieldDependency.Pat},
			{StaticTextField.genderhimselfherself,StaticTextFieldDependency.Pat},
			{StaticTextField.genderHisHer,StaticTextFieldDependency.Pat},
			{StaticTextField.genderhisher,StaticTextFieldDependency.Pat},
			{StaticTextField.genderHisHers,StaticTextFieldDependency.Pat},
			{StaticTextField.genderhishers,StaticTextFieldDependency.Pat},
			{StaticTextField.HmPhone,StaticTextFieldDependency.Pat},
			{StaticTextField.insAnnualMax,StaticTextFieldDependency.ListInsPlans | StaticTextFieldDependency.ListBenefits 
				| StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.insDeductible,StaticTextFieldDependency.ListInsPlans | StaticTextFieldDependency.ListBenefits 
				| StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.insDeductibleUsed,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans |
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.insEmployer,StaticTextFieldDependency.ListInsPlans | StaticTextFieldDependency.ListBenefits 
				| StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.insFeeSchedule,StaticTextFieldDependency.ListInsPlans | StaticTextFieldDependency.ListBenefits 
				| StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.insFreqBW,StaticTextFieldDependency.ListInsPlans | StaticTextFieldDependency.ListBenefits 
				| StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.insFreqExams,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.insFreqPanoFMX,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.insPending,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans |
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.insPercentages,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans |
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.insPlanGroupNumber,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.insPlanGroupName,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.insPlanNote,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.insType,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.insSubBirthDate,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.insSubNote,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.insRemaining,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans |
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.insUsed,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans |
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.ins2AnnualMax,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.ins2Deductible,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.ins2DeductibleUsed,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.ins2Employer,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.ins2FreqBW,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.ins2FreqExams,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.ins2FreqPanoFMX,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.ins2PlanGroupNumber,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.ins2PlanGroupName,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.ins2Pending,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.ins2Percentages,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.ins2Remaining,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.ins2Used,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{StaticTextField.medicalSummary,StaticTextFieldDependency.PatNote},
			{StaticTextField.MedUrgNote,StaticTextFieldDependency.Pat},
			{StaticTextField.nameF,StaticTextFieldDependency.Pat},
			{StaticTextField.nameFL,StaticTextFieldDependency.Pat},
			{StaticTextField.nameFLFormal,StaticTextFieldDependency.Pat},
			{StaticTextField.nameL,StaticTextFieldDependency.Pat},
			{StaticTextField.nameLF,StaticTextFieldDependency.Pat},
			{StaticTextField.nameMI,StaticTextFieldDependency.Pat},
			{StaticTextField.namePref,StaticTextFieldDependency.Pat},
			{StaticTextField.namePreferredOrFirst,StaticTextFieldDependency.Pat},
			{StaticTextField.nextSchedApptDate,StaticTextFieldDependency.ListAppts},
			{StaticTextField.nextSchedApptDateT,StaticTextFieldDependency.ListAppts},
			{StaticTextField.nextSchedApptsFam,StaticTextFieldDependency.Fam | StaticTextFieldDependency.ListFutureApptsForFam},
			{StaticTextField.PatNum,StaticTextFieldDependency.Pat},
			{StaticTextField.famPopups,StaticTextFieldDependency.ListFamPopups},
			{StaticTextField.patientPortalCredentials,StaticTextFieldDependency.Pat},
			{StaticTextField.plannedAppointmentInfo,StaticTextFieldDependency.ListProceduresPat | StaticTextFieldDependency.ListPlannedAppts 
				| StaticTextFieldDependency.ListAppts},
			{StaticTextField.premedicateYN,StaticTextFieldDependency.Pat},
			{StaticTextField.priProvNameFormal,StaticTextFieldDependency.Pat},
			{StaticTextField.recallInterval,StaticTextFieldDependency.ListRecallsForFam},
			{StaticTextField.recallScheduledYN,StaticTextFieldDependency.ListRecallsForFam},
			{StaticTextField.referredFrom,StaticTextFieldDependency.ListRefAttaches},
			{StaticTextField.referredTo,StaticTextFieldDependency.ListRefAttaches},
			{StaticTextField.salutation,StaticTextFieldDependency.Pat},
			{StaticTextField.serviceNote,StaticTextFieldDependency.PatNote},
			{StaticTextField.siteDescription,StaticTextFieldDependency.Pat},
			{StaticTextField.SSN,StaticTextFieldDependency.Pat},
			{StaticTextField.subscriberID,
					StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsPlans | StaticTextFieldDependency.ListInsSubs},
			{StaticTextField.subscriberNameFL,
					StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsPlans | StaticTextFieldDependency.ListInsSubs},
			{StaticTextField.subscriber2NameFL,
					StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsPlans | StaticTextFieldDependency.ListInsSubs},
			{StaticTextField.timeNow,StaticTextFieldDependency.None},
			{StaticTextField.tpResponsPartyAddress,StaticTextFieldDependency.ListTreatPlans},
			{StaticTextField.tpResponsPartyCityStZip,StaticTextFieldDependency.ListTreatPlans},
			{StaticTextField.tpResponsPartyNameFL,StaticTextFieldDependency.ListTreatPlans},
			{StaticTextField.treatmentNote,StaticTextFieldDependency.PatNote},
			{StaticTextField.treatmentPlanProcs,StaticTextFieldDependency.ListProceduresPat},
			{StaticTextField.treatmentPlanProcsPriority,StaticTextFieldDependency.ListProceduresPat},
			{StaticTextField.WirelessPhone,StaticTextFieldDependency.Pat},
			{StaticTextField.WkPhone,StaticTextFieldDependency.Pat},
			{StaticTextField.dateToday,StaticTextFieldDependency.None},
			{StaticTextField.dateTodayLong,StaticTextFieldDependency.None},
			{StaticTextField.practiceTitle,StaticTextFieldDependency.None},
		};

		public static StaticTextFieldDependency GetStaticTextDependencies(List<SheetField> listSheetFields) 
		{
			StaticTextFieldDependency staticTextDependencies=StaticTextFieldDependency.None;
			foreach(SheetField sheetField in listSheetFields.FindAll(x => x.FieldType==SheetFieldType.StaticText)) {
				Regex staticTextPattern=new Regex(@"(?<=)\[.+?\](?=)");//Matches on static text field replacement strings. e.g. [PatLName]
				var matches=staticTextPattern.Matches(sheetField.FieldValue);
				foreach(Match match in matches) {
					string strMatch=match.ToString().Replace("[","").Replace("]","");//Pull out just the replacement string.
					if(!Enum.TryParse(strMatch,out StaticTextField enumStaticText)) {
						continue;
					}
					if(_dictDependencies.TryGetValue(enumStaticText,out StaticTextFieldDependency sheetFieldDependencies)) {
						staticTextDependencies|=sheetFieldDependencies;
					}
					else if(ODBuild.IsDebug()){
						//Programmer must add [staticTextFieldValue] to the dictionary linking these text fields to the data in StaticTextData.
						//Only throwing in Debug in case a customer has some other [notAnActualStaticTextField] type string entered in their sheet.
						//We don't want to crash Sheets for an otherwise perfectly valid Sheet, this exception is intended to force a programmer to implement
						//the proper StaticTextFieldDependency for a new StaticTextField.
						throw new NotImplementedException("StaticTextFieldDependencies not implemented for "+match.Value+".");
					}
				}
			}
			return staticTextDependencies;
		}

		///<summary>Gets the data necessary for Static Text Field string replacements.  Returns a StaticTextData object so that 
		///references are preserved across Middle Tier.</summary>
		public static StaticTextData GetStaticTextData(StaticTextFieldDependency staticTextDependencies,Patient pat,Family fam
			,List<long> listProcCodeNums,StaticTextData data=null)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<StaticTextData>(MethodBase.GetCurrentMethod(),staticTextDependencies,pat,fam,listProcCodeNums,data);
			}
			data=data??new StaticTextData();
			data.LoadData(staticTextDependencies,pat,fam,listProcCodeNums);
			return data;
		}

		///<summary>Runs the required queries to populate the necessary StaticTextData fields corresponding to staticTextDependencies.</summary>
		private void LoadData(StaticTextFieldDependency staticTextDependencies,Patient pat,Family fam,List<long> listProcCodeNums) {
			bool isMiddleTier=(RemotingClient.RemotingRole==RemotingRole.ServerWeb);
			System.Diagnostics.Stopwatch timer=null;
			if(ODBuild.IsDebug()) {
				timer=new System.Diagnostics.Stopwatch();
				timer.Start();
			}
			if(staticTextDependencies.HasFlag(StaticTextFieldDependency.Pat)) {
				//patient should already be loaded.  
			}
			if(fam==null && staticTextDependencies.HasFlag(StaticTextFieldDependency.Fam)) {
				fam=Patients.GetFamily(pat.PatNum);
			}
			if(PatNote==null) {
				if(staticTextDependencies.HasFlag(StaticTextFieldDependency.PatNote)) {
					PatNote=PatientNotes.Refresh(pat.PatNum,pat.Guarantor);
				}
				else {
					PatNote=new PatientNote();
				}
			}
			bool IsQueryNeeded<T>(ref List<T> list,StaticTextFieldDependency dependency) {
				if(list==null || (isMiddleTier && list.Count==0)) {//Middle Tier deserializes null lists to empty lists.
					if(staticTextDependencies.HasFlag(dependency)) {
						return true;
					}
					else {
						list=new List<T>();
					}
				}
				return false;
			}
			if(IsQueryNeeded(ref ListRefAttaches,StaticTextFieldDependency.ListRefAttaches)) {
				ListRefAttaches=RefAttaches.Refresh(pat.PatNum);
			}
			if(IsQueryNeeded(ref ListInsSubs,StaticTextFieldDependency.ListInsSubs)) {
				ListInsSubs=InsSubs.RefreshForFam(fam);
			}
			if(IsQueryNeeded(ref ListInsPlans,StaticTextFieldDependency.ListInsPlans)) {
				ListInsPlans=InsPlans.RefreshForSubList(ListInsSubs);
			}
			if(IsQueryNeeded(ref ListPatPlans,StaticTextFieldDependency.ListPatPlans)) {
				ListPatPlans=PatPlans.Refresh(pat.PatNum);
			}
			if(IsQueryNeeded(ref ListBenefits,StaticTextFieldDependency.ListBenefits)) {
				ListBenefits=Benefits.Refresh(ListPatPlans,ListInsSubs);
			}
			if(IsQueryNeeded(ref HistList,StaticTextFieldDependency.HistList)) {
				HistList=ClaimProcs.GetHistList(pat.PatNum,ListBenefits,ListPatPlans,ListInsPlans,DateTime.Today,ListInsSubs);
			}
			if(IsQueryNeeded(ref ListTreatPlans,StaticTextFieldDependency.ListTreatPlans)) {
				ListTreatPlans=TreatPlans.Refresh(pat.PatNum);
			}
			if(IsQueryNeeded(ref ListRecallsForFam,StaticTextFieldDependency.ListRecallsForFam)) {
				ListRecallsForFam=Recalls.GetList(fam.ListPats.Select(x => x.PatNum).ToList());
			}
			if(IsQueryNeeded(ref ListAppts,StaticTextFieldDependency.ListAppts)) {
				ListAppts=Appointments.GetListForPat(pat.PatNum);
			}
			if(IsQueryNeeded(ref ListFutureApptsForFam,StaticTextFieldDependency.ListFutureApptsForFam)) {
				ListFutureApptsForFam=Appointments.GetFutureSchedApts(fam.ListPats.Select(x => x.PatNum).ToList());
			}
			if(IsQueryNeeded(ref ListDiseases,StaticTextFieldDependency.ListDiseases)) {
				ListDiseases=Diseases.Refresh(pat.PatNum,true);
			}
			if(IsQueryNeeded(ref ListAllergies,StaticTextFieldDependency.ListAllergies)) {
				ListAllergies=Allergies.GetAll(pat.PatNum,false);
			}
			if(IsQueryNeeded(ref ListMedicationPats,StaticTextFieldDependency.ListMedicationPats)) {
				ListMedicationPats=MedicationPats.Refresh(pat.PatNum,false);
			}
			if(IsQueryNeeded(ref ListFamPopups,StaticTextFieldDependency.ListFamPopups)) {
				ListFamPopups=Popups.GetForFamily(pat);
			}
			if(IsQueryNeeded(ref ListProceduresSome,StaticTextFieldDependency.ListProceduresSome)) {
				ListProceduresSome=Procedures.RefreshForProcCodeNums(pat.PatNum,listProcCodeNums);
			}
			if(IsQueryNeeded(ref ListProceduresPat,StaticTextFieldDependency.ListProceduresPat)) {
				ListProceduresPat=Procedures.Refresh(pat.PatNum);
			}
			if(IsQueryNeeded(ref ListPlannedAppts,StaticTextFieldDependency.ListPlannedAppts)) {
				ListPlannedAppts=new List<PlannedAppt>();
				PlannedAppt plannedAppt=PlannedAppts.GetOneOrderedByItemOrder(pat.PatNum);
				if(plannedAppt!=null) {
					ListPlannedAppts.Add(plannedAppt);
				}
			}
			if(IsQueryNeeded(ref ListSelectedTpProcs,StaticTextFieldDependency.ListSelectedTpProcs)) {
				ListSelectedTpProcs=Procedures.RefreshForProcCodeNums(pat.PatNum, listProcCodeNums);
			}
			if(ODBuild.IsDebug()) {
				timer.Stop();
				Console.WriteLine("Static text field query time (ms): "+timer.ElapsedMilliseconds);
			}
		}

	}

	///<summary>Bitwise flag indicating a dependency upon a certain StaticTextData field.</summary>
	[Flags]
	public enum StaticTextFieldDependency : long {
		None=					0b0,
		Pat=					0b1,
		Fam=					0b10,
		PatNote=				0b100,
		ListRefAttaches=		0b1000,
		ListInsSubs=			0b10000,
		ListInsPlans=			0b100000,
		ListPatPlans=			0b1000000,
		ListBenefits=			0b10000000,
		HistList=				0b100000000,
		ListTreatPlans=			0b1000000000,
		ListRecallsForFam=		0b10000000000,
		ListAppts=				0b100000000000,
		ListFutureApptsForFam=	0b1000000000000,
		ListDiseases=			0b10000000000000,
		ListAllergies=			0b100000000000000,
		ListMedicationPats=		0b1000000000000000,
		ListFamPopups=			0b10000000000000000,
		ListProceduresSome=		0b100000000000000000,
		ListDocuments=			0b1000000000000000000,
		ListProceduresPat=		0b10000000000000000000,
		ListPlannedAppts=		0b100000000000000000000,
		ListSelectedTpProcs=	0b1000000000000000000000,
	}

	public enum StaticTextField {
		activeAllergies, 
		activeProblems, 
		address,
		age,
		apptsAllFuture,
		apptDateMonthSpelled,
		apptModNote,
		apptProcs,
		apptProvNameFormal,
		balTotal,
		bal_0_30,
		bal_31_60,
		bal_61_90,
		balOver90,
		balInsEst,
		balTotalMinusInsEst,
		BillingType,
		Birthdate,
		carrierName,
		carrier2Name,
		carrierAddress,
		carrier2Address,
		carrierCityStZip,
		carrier2CityStZip,
		ChartNumber,
		cityStateZip,
		clinicDescription,
		clinicAddress,
		clinicCityStZip,
		clinicPhone,
		clinicPatDescription,
		clinicPatAddress,
		clinicPatCityStZip,
		clinicPatPhone,
		clinicCurDescription,
		clinicCurAddress,
		clinicCurCityStZip,
		clinicCurPhone,
		currentMedications,
		DateFirstVisit,
		dateLastAppt,
		dateLastBW,
		dateLastExam,
		dateLastPanoFMX,
		dateLastPerio,
		dateLastProphy,
		dateLastSrp,
		dateOfLastSavedTP,
		dateRecallDue,
		dateTimeLastAppt,
		dateTodayLong,
		dateToday,
		dueForBWYN,
		dueForPanoYN,
		Email,
		famFinNote,
		famFinUrgNote,
		famPopups,
		famRecallDue,
		gender,
		genderHeShe,
		genderheshe,
		genderHimHer,
		genderhimher,
		genderHimselfHerself,
		genderhimselfherself,
		genderHisHer,
		genderhisher,
		genderHisHers,
		genderhishers,
		guarantorHmPhone,
		guarantorNameF,
		guarantorNameFL,
		guarantorNameL,
		guarantorNamePref,
		guarantorNameLF,
		guarantorWirelessPhone,
		guarantorWkPhone,
		HmPhone,
		insAnnualMax,
		insDeductible,
		insDeductibleUsed,
		insEmployer,
		insFeeSchedule,
		insFreqBW,
		insFreqExams,
		insFreqPanoFMX,
		insPending,
		insPercentages,
		insPlanGroupName,
		insPlanGroupNumber,
		insPlanNote,
		insRemaining,
		insSubBirthDate,
		insSubNote,
		insType,
		insUsed,
		ins2AnnualMax,
		ins2Deductible,
		ins2DeductibleUsed,
		ins2Employer,
		ins2FreqBW,
		ins2FreqExams,
		ins2FreqPanoFMX,
		ins2Pending,
		ins2Percentages,
		ins2PlanGroupName,
		ins2PlanGroupNumber,
		ins2Remaining,
		ins2Used,
		medicalSummary,
		MedUrgNote,
		nameF,
		nameFL,
		nameFLFormal,
		nameL,
		nameLF,
		nameMI,
		namePref,
		namePreferredOrFirst,
		nextSchedApptDate,
		nextSchedApptDateT,
		nextSchedApptsFam,
		patientPortalCredentials,
		PatNum,
		plannedAppointmentInfo,
		practiceTitle,
		premedicateYN,
		priProvNameFormal,
		recallInterval,
		recallScheduledYN,
		referredFrom,
		referredTo,
		salutation,
		serviceNote,
		siteDescription,
		SSN,
		subscriberID,
		subscriberNameFL,
		subscriber2NameFL,
		timeNow,
		tpResponsPartyAddress,
		tpResponsPartyCityStZip,
		tpResponsPartyNameFL,
		treatmentNote,
		treatmentPlanProcs,
		treatmentPlanProcsPriority,
		WirelessPhone,
		WkPhone,
	}

	
	public static class StaticTextFieldExtensions {
		public static bool IsStaticTextFieldObsolete(this StaticTextField value) {
			if(ListTools.In(value,StaticTextField.clinicDescription,StaticTextField.clinicAddress,StaticTextField.clinicCityStZip,StaticTextField.clinicPhone)) {
				return true;
			}
			return false;
		}
	}
}
