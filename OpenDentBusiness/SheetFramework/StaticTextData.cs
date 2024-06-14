using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness {
	public class StaticTextData {
		#region Fields
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
		public List<Appointment> ListPlannedAppts;
		public List<Procedure> ListSelectedTpProcs;
		///<summary>Dictionary linking static text field names to the data fields in StaticTextData that are required to determine the value for
		///the static text field in a Sheet.</summary>
		private static readonly Dictionary<EnumStaticTextField,StaticTextFieldDependency> _dictDependencies
			=new Dictionary<EnumStaticTextField,StaticTextFieldDependency>() 
		{
			{EnumStaticTextField.activeAllergies,StaticTextFieldDependency.ListAllergies},
			{EnumStaticTextField.activeProblems,StaticTextFieldDependency.ListDiseases},
			{EnumStaticTextField.address,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.apptsAllFuture,StaticTextFieldDependency.ListFutureApptsForFam},
			{EnumStaticTextField.apptDateMonthSpelled,StaticTextFieldDependency.ListAppts},
			{EnumStaticTextField.apptModNote,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.apptProcs,StaticTextFieldDependency.ListAppts | StaticTextFieldDependency.ListProceduresPat},
			{EnumStaticTextField.apptProvNameFormal,StaticTextFieldDependency.ListAppts},
			{EnumStaticTextField.age,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.balTotal,StaticTextFieldDependency.Fam},
			{EnumStaticTextField.bal_0_30,StaticTextFieldDependency.Fam},
			{EnumStaticTextField.bal_31_60,StaticTextFieldDependency.Fam},
			{EnumStaticTextField.bal_61_90,StaticTextFieldDependency.Fam},
			{EnumStaticTextField.balOver90,StaticTextFieldDependency.Fam},
			{EnumStaticTextField.balInsEst,StaticTextFieldDependency.Fam},
			{EnumStaticTextField.balTotalMinusInsEst,StaticTextFieldDependency.Fam},
			{EnumStaticTextField.BillingType,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.Birthdate,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.carrierName,StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsSubs 
				| StaticTextFieldDependency.ListInsPlans},
			{EnumStaticTextField.carrier2Name,StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsSubs 
				| StaticTextFieldDependency.ListInsPlans},
			{EnumStaticTextField.ChartNumber,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.carrierAddress,StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsSubs 
				| StaticTextFieldDependency.ListInsPlans},
			{EnumStaticTextField.carrier2Address,StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsSubs 
				| StaticTextFieldDependency.ListInsPlans},
			{EnumStaticTextField.carrierCityStZip,StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsSubs 
				| StaticTextFieldDependency.ListInsPlans},
			{EnumStaticTextField.carrier2CityStZip,StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsSubs 
				| StaticTextFieldDependency.ListInsPlans},
			{EnumStaticTextField.cityStateZip,StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsSubs 
				| StaticTextFieldDependency.ListInsPlans},
			{EnumStaticTextField.clinicDescription,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.clinicAddress,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.clinicCityStZip,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.clinicPhone,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.clinicPatDescription,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.clinicPatAddress,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.clinicPatCityStZip,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.clinicPatPhone,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.clinicCurDescription,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.clinicCurAddress,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.clinicCurCityStZip,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.clinicCurPhone,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.currentMedications,StaticTextFieldDependency.ListMedicationPats},
			{EnumStaticTextField.DateFirstVisit,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.dateLastAppt,StaticTextFieldDependency.ListAppts},
			{EnumStaticTextField.dateLastBW,StaticTextFieldDependency.ListProceduresSome},
			{EnumStaticTextField.dateLastExam,StaticTextFieldDependency.ListProceduresSome},
			{EnumStaticTextField.dateLastPerio,StaticTextFieldDependency.ListProceduresSome},
			{EnumStaticTextField.dateLastPanoFMX,StaticTextFieldDependency.ListProceduresSome},
			{EnumStaticTextField.dateLastProphy,StaticTextFieldDependency.ListProceduresSome},
			{EnumStaticTextField.dateLastSrp,StaticTextFieldDependency.ListProceduresSome},
			{EnumStaticTextField.dateOfLastSavedTP,StaticTextFieldDependency.ListTreatPlans},
			{EnumStaticTextField.dateRecallDue,StaticTextFieldDependency.Pat | StaticTextFieldDependency.ListRecallsForFam},
			{EnumStaticTextField.dateTimeLastAppt,StaticTextFieldDependency.ListAppts},
			{EnumStaticTextField.dueForBWYN,StaticTextFieldDependency.ListRecallsForFam},
			{EnumStaticTextField.dueForPanoYN,StaticTextFieldDependency.ListRecallsForFam},
			{EnumStaticTextField.Email,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.famFinNote,StaticTextFieldDependency.PatNote},
			{EnumStaticTextField.famFinUrgNote,StaticTextFieldDependency.Fam},
			{EnumStaticTextField.famRecallDue,StaticTextFieldDependency.Fam | StaticTextFieldDependency.ListRecallsForFam},
			{EnumStaticTextField.guarantorHmPhone,StaticTextFieldDependency.Fam},
			{EnumStaticTextField.guarantorNameF,StaticTextFieldDependency.Fam},
			{EnumStaticTextField.guarantorNameFL,StaticTextFieldDependency.Fam},
			{EnumStaticTextField.guarantorNameL,StaticTextFieldDependency.Fam},
			{EnumStaticTextField.guarantorNamePref,StaticTextFieldDependency.Fam},
			{EnumStaticTextField.guarantorNameLF,StaticTextFieldDependency.Fam},
			{EnumStaticTextField.guarantorWirelessPhone,StaticTextFieldDependency.Fam},
			{EnumStaticTextField.guarantorWkPhone,StaticTextFieldDependency.Fam},
			{EnumStaticTextField.gender,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.genderHeShe,StaticTextFieldDependency.Pat | StaticTextFieldDependency.PatNote},
			{EnumStaticTextField.genderheshe,StaticTextFieldDependency.Pat | StaticTextFieldDependency.PatNote},
			{EnumStaticTextField.genderHimHer,StaticTextFieldDependency.Pat | StaticTextFieldDependency.PatNote},
			{EnumStaticTextField.genderhimher,StaticTextFieldDependency.Pat | StaticTextFieldDependency.PatNote},
			{EnumStaticTextField.genderHimselfHerself,StaticTextFieldDependency.Pat | StaticTextFieldDependency.PatNote},
			{EnumStaticTextField.genderhimselfherself,StaticTextFieldDependency.Pat | StaticTextFieldDependency.PatNote},
			{EnumStaticTextField.genderHisHer,StaticTextFieldDependency.Pat | StaticTextFieldDependency.PatNote},
			{EnumStaticTextField.genderhisher,StaticTextFieldDependency.Pat | StaticTextFieldDependency.PatNote},
			{EnumStaticTextField.genderHisHers,StaticTextFieldDependency.Pat | StaticTextFieldDependency.PatNote},
			{EnumStaticTextField.genderhishers,StaticTextFieldDependency.Pat | StaticTextFieldDependency.PatNote},
			{EnumStaticTextField.HmPhone,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.insAnnualMax,StaticTextFieldDependency.ListInsPlans | StaticTextFieldDependency.ListBenefits 
				| StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.insDeductible,StaticTextFieldDependency.ListInsPlans | StaticTextFieldDependency.ListBenefits 
				| StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.insDeductibleUsed,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans |
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.insEmployer,StaticTextFieldDependency.ListInsPlans | StaticTextFieldDependency.ListBenefits 
				| StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.insFeeSchedule,StaticTextFieldDependency.ListInsPlans | StaticTextFieldDependency.ListBenefits 
				| StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.insFreqBW,StaticTextFieldDependency.ListInsPlans | StaticTextFieldDependency.ListBenefits 
				| StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.insFreqExams,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.insFreqPanoFMX,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.insPending,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans |
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.insPercentages,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans |
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.insPlanGroupNumber,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.insPlanGroupName,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.insPlanNote,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.insType,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.insSubBirthDate,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.insSubNote,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.insRemaining,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans |
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.insUsed,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans |
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.ins2AnnualMax,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.ins2Deductible,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.ins2DeductibleUsed,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.ins2Employer,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.ins2FreqBW,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.ins2FreqExams,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.ins2FreqPanoFMX,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.ins2PlanGroupNumber,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.ins2PlanGroupName,StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.ins2Pending,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.ins2Percentages,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.ins2Remaining,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.ins2Used,StaticTextFieldDependency.HistList | StaticTextFieldDependency.ListInsPlans | 
				StaticTextFieldDependency.ListBenefits | StaticTextFieldDependency.ListInsSubs | StaticTextFieldDependency.ListPatPlans},
			{EnumStaticTextField.medicalSummary,StaticTextFieldDependency.PatNote},
			{EnumStaticTextField.MedUrgNote,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.nameF,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.nameFL,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.nameFLFormal,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.nameL,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.nameLF,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.nameMI,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.namePref,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.namePreferredOrFirst,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.nextSchedApptDate,StaticTextFieldDependency.ListAppts},
			{EnumStaticTextField.nextSchedApptDateT,StaticTextFieldDependency.ListAppts},
			{EnumStaticTextField.nextSchedApptsFam,StaticTextFieldDependency.Fam | StaticTextFieldDependency.ListFutureApptsForFam},
			{EnumStaticTextField.PatNum,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.famPopups,StaticTextFieldDependency.ListFamPopups},
			{EnumStaticTextField.patientPortalCredentials,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.plannedAppointmentInfo,StaticTextFieldDependency.ListProceduresPat | StaticTextFieldDependency.ListPlannedAppts 
				| StaticTextFieldDependency.ListAppts},
			{EnumStaticTextField.premedicateYN,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.priProvNameFormal,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.recallInterval,StaticTextFieldDependency.ListRecallsForFam},
			{EnumStaticTextField.recallScheduledYN,StaticTextFieldDependency.ListRecallsForFam},
			{EnumStaticTextField.referredFrom,StaticTextFieldDependency.ListRefAttaches},
			{EnumStaticTextField.referredTo,StaticTextFieldDependency.ListRefAttaches},
			{EnumStaticTextField.salutation,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.serviceNote,StaticTextFieldDependency.PatNote},
			{EnumStaticTextField.siteDescription,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.SSN,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.subscriberID,
					StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsPlans | StaticTextFieldDependency.ListInsSubs},
			{EnumStaticTextField.subscriberNameFL,
					StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsPlans | StaticTextFieldDependency.ListInsSubs},
			{EnumStaticTextField.subscriber2NameFL,
					StaticTextFieldDependency.ListPatPlans | StaticTextFieldDependency.ListInsPlans | StaticTextFieldDependency.ListInsSubs},
			{EnumStaticTextField.timeNow,StaticTextFieldDependency.None},
			{EnumStaticTextField.tpResponsPartyAddress,StaticTextFieldDependency.ListTreatPlans},
			{EnumStaticTextField.tpResponsPartyCityStZip,StaticTextFieldDependency.ListTreatPlans},
			{EnumStaticTextField.tpResponsPartyNameFL,StaticTextFieldDependency.ListTreatPlans},
			{EnumStaticTextField.treatmentNote,StaticTextFieldDependency.PatNote},
			{EnumStaticTextField.treatmentPlanProcs,StaticTextFieldDependency.ListProceduresPat},
			{EnumStaticTextField.treatmentPlanProcsPriority,StaticTextFieldDependency.ListProceduresPat},
			{EnumStaticTextField.WirelessPhone,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.WkPhone,StaticTextFieldDependency.Pat},
			{EnumStaticTextField.dateToday,StaticTextFieldDependency.None},
			{EnumStaticTextField.dateTodayLong,StaticTextFieldDependency.None},
			{EnumStaticTextField.practiceTitle,StaticTextFieldDependency.None},
		};
		#endregion Fields

		public static StaticTextFieldDependency GetStaticTextDependencies(List<EnumStaticTextField> listEnumStaticTextFields){
			StaticTextFieldDependency staticTextFieldDependencyRet=StaticTextFieldDependency.None;
			for(int i=0;i<listEnumStaticTextFields.Count;i++){
				StaticTextFieldDependency staticTextFieldDependency;
				try{
					staticTextFieldDependency=_dictDependencies[listEnumStaticTextFields[i]];
				}
				catch{
					if(ODBuild.IsDebug()){
						//Programmer must add [staticTextFieldValue] to the dictionary linking these text fields to the data in StaticTextData.
						//Only throwing in Debug in case a customer has some other [notAnActualStaticTextField] type string entered in their sheet.
						//We don't want to crash Sheets for an otherwise perfectly valid Sheet, this exception is intended to force a programmer to implement
						//the proper StaticTextFieldDependency for a new StaticTextField.
						throw new NotImplementedException("StaticTextFieldDependencies not implemented for "+listEnumStaticTextFields[i].ToString()+".");
					}
					continue;
				}
				staticTextFieldDependencyRet|=staticTextFieldDependency;
			}
			return staticTextFieldDependencyRet;
		}

		///<summary>Gets the data necessary for Static Text Field string replacements.  Returns a StaticTextData object so that 
		///references are preserved across Middle Tier.</summary>
		public static StaticTextData GetStaticTextData(StaticTextFieldDependency staticTextDependencies,Patient pat,Family fam
			,List<long> listProcCodeNums,StaticTextData staticTextData=null)
		{
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<StaticTextData>(MethodBase.GetCurrentMethod(),staticTextDependencies,pat,fam,listProcCodeNums,staticTextData);
			}
			staticTextData=staticTextData??new StaticTextData();
			staticTextData.LoadData(staticTextDependencies,pat,fam,listProcCodeNums);
			return staticTextData;
		}

		///<summary>Runs the required queries to populate the necessary StaticTextData fields corresponding to staticTextDependencies.</summary>
		private void LoadData(StaticTextFieldDependency staticTextDependencies,Patient pat,Family fam,List<long> listProcCodeNums) {
			bool isMiddleTier=(RemotingClient.MiddleTierRole==MiddleTierRole.ServerMT);
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
				ListAppts=Appointments.GetPatientData(pat.PatNum);
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
				ListPlannedAppts=new List<Appointment>();
				Appointment plannedAppt=Appointments.GetOneOrderedByItemOrder(pat.PatNum);
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

}
