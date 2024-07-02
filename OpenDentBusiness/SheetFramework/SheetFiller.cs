using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using OpenDentBusiness.SheetFramework;
using CodeBase;

namespace OpenDentBusiness {
	public class SheetFiller {

		///<summary>Gets some data from the database and fills the fields. Input should only be new sheets.
		///dataSet should be prefilled with AccountModules.GetAccount() prior to calling this method when filling fields for statements.
		///Returns empty string if no errors were encountered.  To avoid queries, pass in staticTextData with initialized fields; null fields will 
		///be handled by running the appropriate query.</summary>
		public static string FillFields(Sheet sheet,DataSet dataSet=null,Statement stmt=null,MedLab medLab=null,Patient patient=null,Family family=null
			,StaticTextData staticTextData=null,long refAttachProvNum=0) 
		{
			foreach(SheetParameter param in sheet.Parameters){
				if(param.IsRequired && param.ParamValue==null){
					throw new ApplicationException(Lans.g("Sheet","Parameter not specified for sheet")+": "+param.ParamName);
				}
			}
			Provider provider=null;
			Referral refer=null;
			Deposit deposit=null;
			switch(sheet.SheetType) {
				case SheetTypeEnum.LabelPatient:
					long patNum=(long)GetParamByName(sheet,"PatNum").ParamValue;
					patient=(patient==null || patient.PatNum!=patNum ? Patients.GetPat(patNum) : patient);
					FillFieldsForLabelPatient(sheet,patient);
					break;
				case SheetTypeEnum.LabelCarrier:
					Carrier carrier=Carriers.GetCarrier((long)GetParamByName(sheet,"CarrierNum").ParamValue);
					FillFieldsForLabelCarrier(sheet,carrier);
					break;
				case SheetTypeEnum.LabelReferral:
					Referrals.TryGetReferral((long)GetParamByName(sheet,"ReferralNum").ParamValue,out refer);
					FillFieldsForLabelReferral(sheet,refer);
					break;
				case SheetTypeEnum.ReferralSlip:
					patNum=(long)GetParamByName(sheet,"PatNum").ParamValue;
					patient=(patient==null || patient.PatNum!=patNum ? Patients.GetPat(patNum) : patient);
					Referrals.TryGetReferral((long)GetParamByName(sheet,"ReferralNum").ParamValue,out refer);
					FillFieldsForReferralSlip(sheet,patient,refer,refAttachProvNum);
					break;
				case SheetTypeEnum.LabelAppointment:
					Appointment appt=Appointments.GetOneApt((long)GetParamByName(sheet,"AptNum").ParamValue);
					patNum=appt.PatNum;
					patient=(patient==null || patient.PatNum!=patNum ? Patients.GetPat(patNum) : patient);
					FillFieldsForLabelAppointment(sheet,appt,patient);
					break;
				case SheetTypeEnum.Rx:
					RxPat rx=RxPats.GetRx((long)GetParamByName(sheet,"RxNum").ParamValue);
					patNum=rx.PatNum;
					patient=(patient==null || patient.PatNum!=patNum ? Patients.GetPat(patNum) : patient);
					Provider prov=Providers.GetProv(rx.ProvNum);
					FillFieldsForRx(sheet,rx,patient,prov);
					break;
				case SheetTypeEnum.Consent:
					patNum=(long)GetParamByName(sheet,"PatNum").ParamValue;
					patient=(patient==null || patient.PatNum!=patNum ? Patients.GetPat(patNum) : patient);
					FillFieldsForConsent(sheet,patient);
					break;
				case SheetTypeEnum.PatientLetter:
					patNum=(long)GetParamByName(sheet,"PatNum").ParamValue;
					patient=(patient==null || patient.PatNum!=patNum ? Patients.GetPat(patNum) : patient);
					FillFieldsForPatientLetter(sheet,patient);
					break;
				case SheetTypeEnum.ReferralLetter:
					patNum=(long)GetParamByName(sheet,"PatNum").ParamValue;
					patient=(patient==null || patient.PatNum!=patNum ? Patients.GetPat(patNum) : patient);
					Referrals.TryGetReferral((long)GetParamByName(sheet,"ReferralNum").ParamValue,out refer);
					FillFieldsForReferralLetter(sheet,patient,refer);
					break;
				case SheetTypeEnum.PatientForm:
					patNum=(long)GetParamByName(sheet,"PatNum").ParamValue;
					patient=(patient==null || patient.PatNum!=patNum ? Patients.GetPat(patNum) : patient);
					FillFieldsForPatientForm(sheet,patient);
					break;
				case SheetTypeEnum.RoutingSlip:
					Appointment apt=Appointments.GetOneApt((long)GetParamByName(sheet,"AptNum").ParamValue);
					if(apt==null) {
						return Lans.g("SheetFiller","Appointment no longer exists.");
					}
					patNum=apt.PatNum;
					patient=(patient==null || patient.PatNum!=patNum ? Patients.GetPat(patNum) : patient);
					FillFieldsForRoutingSlip(sheet,patient,apt);
					break;
				case SheetTypeEnum.MedicalHistory:
					patNum=(long)GetParamByName(sheet,"PatNum").ParamValue;
					patient=(patient==null || patient.PatNum!=patNum ? Patients.GetPat(patNum) : patient);
					FillFieldsForMedicalHistory(sheet,patient);
					break;
				case SheetTypeEnum.LabSlip:
					patNum=(long)GetParamByName(sheet,"PatNum").ParamValue;
					patient=(patient==null || patient.PatNum!=patNum ? Patients.GetPat(patNum) : patient);
					LabCase lab=LabCases.GetOne((long)GetParamByName(sheet,"LabCaseNum").ParamValue);
					FillFieldsForLabCase(sheet,patient,lab);
					break;
				case SheetTypeEnum.ExamSheet:
					patNum=(long)GetParamByName(sheet,"PatNum").ParamValue;
					patient=(patient==null || patient.PatNum!=patNum ? Patients.GetPat(patNum) : patient);
					FillFieldsForExamSheet(sheet,patient);
					break;
				case SheetTypeEnum.DepositSlip:
					deposit=Deposits.GetOne((long)GetParamByName(sheet,"DepositNum").ParamValue);
					FillFieldsForDepositSlip(sheet,deposit);
					break;
				case SheetTypeEnum.Statement:
					patient=(patient==null || patient.PatNum!=sheet.PatNum ? Patients.GetPat(sheet.PatNum) : patient);
					FillFieldsForStatement(sheet,stmt,dataSet,patient,family?.Guarantor);
					break;
				case SheetTypeEnum.MedLabResults:
					patient=(patient==null || patient.PatNum!=sheet.PatNum ? Patients.GetPat(sheet.PatNum) : patient);
					FillFieldsForMedLabResults(sheet,medLab,patient);
					break;
				case SheetTypeEnum.TreatmentPlan:
					patient=(patient==null || patient.PatNum!=sheet.PatNum ? Patients.GetPat(sheet.PatNum) : patient);
					FillFieldsForTreatPlan(sheet,patient);
					break;
				case SheetTypeEnum.Screening:
					ScreenGroup screenGroup=ScreenGroups.GetScreenGroup((long)GetParamByName(sheet,"ScreenGroupNum").ParamValue);
					//GetForScreen((long)GetParamByName(sheet,"ScreenNum").ParamValue);
					//Look for the optional PatNum param:
					SheetParameter paraPatNum=GetParamByName(sheet,"PatNum");
					if(paraPatNum!=null && paraPatNum.ParamValue!=null) {
						patNum=(long)paraPatNum.ParamValue;
						patient=(patient==null || patient.PatNum!=patNum ? Patients.GetPat(patNum) : patient);
					}
					//Look for the optional ProvNum param:
					SheetParameter paraProvNum=GetParamByName(sheet,"ProvNum");
					if(paraProvNum!=null && paraProvNum.ParamValue!=null) {
						provider=Providers.GetProv((long)paraProvNum.ParamValue);
					}
					FillFieldsForScreening(sheet,screenGroup,patient,provider);
					break;
				case SheetTypeEnum.PaymentPlan:
					patient=(patient==null || patient.PatNum!=sheet.PatNum ? Patients.GetPat(sheet.PatNum) : patient);
					FillFieldsForPaymentPlan(sheet,patient);
					break;
				case SheetTypeEnum.RxMulti:
					patient=(patient==null || patient.PatNum!=sheet.PatNum ? Patients.GetPat(sheet.PatNum) : patient);
					FillFieldsForRxMulti(sheet,patient);
					break;
				case SheetTypeEnum.ERA:
					FillFieldsForERA(sheet);
					break;
				case SheetTypeEnum.ERAGridHeader:
					FillFieldsForERAGridHeader(sheet);
					break;
				case SheetTypeEnum.RxInstruction:
					FillFieldsForRxInstruction(sheet);
					break;
				case SheetTypeEnum.PatientDashboardWidget:
					patient=(patient==null || patient.PatNum!=sheet.PatNum ? Patients.GetPat(sheet.PatNum) : patient);
					break;
			}
			List<EnumStaticTextField> listEnumStaticTextFields=GetAllStaticTextFieldsForSheet(sheet);
			StaticTextFieldDependency staticTextFieldDependency=StaticTextData.GetStaticTextDependencies(listEnumStaticTextFields);
			SheetParameter sheetParameterAptNum=GetParamByName(sheet,"AptNum");
			long aptNum=0;
			if(sheetParameterAptNum!=null && sheetParameterAptNum.ParamValue!=null && staticTextData!=null && !staticTextData.ListAppts.IsNullOrEmpty()) {
				aptNum=PIn.Long(sheetParameterAptNum.ParamValue.ToString(),hasExceptions:false);
			}
			List<StaticTextReplacement> listStaticTextReplacements=GetStaticTextReplacements(listEnumStaticTextFields,patient,family,staticTextData,staticTextFieldDependency,aptNum);
			ReplaceStaticTextFieldsInSheet(listStaticTextReplacements,sheet,patient,family);
			FillPatientImages(sheet,patient,staticTextData);
			Plugins.HookAddCode(null,"SheetFiller.FillFields_end",sheet,stmt,patient);
			return "";
		}

		public static List<EnumStaticTextField> GetAllStaticTextFieldsForSheet(Sheet sheet){
			List<EnumStaticTextField> listEnumStaticTextFields=new List<EnumStaticTextField>();
			for(int i=0;i<sheet.SheetFields.Count;i++) {
				if(sheet.SheetFields[i].FieldType!=SheetFieldType.StaticText) {
					continue;
				}
				string pattern = @"\["//beginning square bracket
					+@"("//beginning of group
					+@"\w+"//one or more word characters (letters, digits, or underscores)
					+@")"//end of group
					+@"\]";//ending square bracket
				//group 0 is the entire match, including []
				//group 1 is just the part inside (), so without []
				Regex regex=new Regex(pattern);
				MatchCollection matchCollection=regex.Matches(sheet.SheetFields[i].FieldValue);
				for(int m=0;m<matchCollection.Count;m++){
					EnumStaticTextField enumStaticTextField;
					try{
					 enumStaticTextField=(EnumStaticTextField)Enum.Parse(typeof(EnumStaticTextField),matchCollection[m].Groups[1].Value);
					}
					catch{
						continue;
					}
					listEnumStaticTextFields.Add(enumStaticTextField);
				}
			}
			return listEnumStaticTextFields;
		}

		private static SheetParameter GetParamByName(Sheet sheet,string paramName){
			foreach(SheetParameter param in sheet.Parameters){
				if(param.ParamName==paramName){
					return param;
				}
			}
			return null;
		}

		public static List<long> GetListProcCodeNumsForStaticText(out List<long> listCodeNumsPano,out List<long> listCodeNumsExam,out List<long> listCodeNumsProphy,out List<long> listCodeNumsPerio,
			out List<long> listCodeNumsBW,out List<long> listCodeNumsSRP)
		{
			//Fill each out parameter with the CodeNums associated to the corresponding fixed code group.
			listCodeNumsPano=ProcedureCodes.GetCodeNumsForCodeGroupFixed(EnumCodeGroupFixed.PanoFMX);
			listCodeNumsExam=ProcedureCodes.GetCodeNumsForCodeGroupFixed(EnumCodeGroupFixed.Exam);
			listCodeNumsProphy=ProcedureCodes.GetCodeNumsForCodeGroupFixed(EnumCodeGroupFixed.Prophy);
			listCodeNumsBW=ProcedureCodes.GetCodeNumsForCodeGroupFixed(EnumCodeGroupFixed.BW);
			listCodeNumsPerio=ProcedureCodes.GetCodeNumsForCodeGroupFixed(EnumCodeGroupFixed.Perio);
			listCodeNumsSRP=ProcedureCodes.GetCodeNumsForCodeGroupFixed(EnumCodeGroupFixed.SRP);
			//Fill listProcCodeNums with all of the code nums found.
			List<long> listProcCodeNums=new List<long>();
			listProcCodeNums.AddRange(listCodeNumsPano);
			listProcCodeNums.AddRange(listCodeNumsBW);
			listProcCodeNums.AddRange(listCodeNumsExam);
			listProcCodeNums.AddRange(listCodeNumsPerio);
			listProcCodeNums.AddRange(listCodeNumsProphy);
			listProcCodeNums.AddRange(listCodeNumsSRP);
			return listProcCodeNums;
		}

		///<summary>Pat can be null sometimes.  For example, in deposit slip.</summary>
		public static List<StaticTextReplacement> GetStaticTextReplacements(List<EnumStaticTextField> listEnumStaticTextFields,Patient patient,Family family,StaticTextData staticTextData,StaticTextFieldDependency staticTextFieldDependency,long aptNum) {
			#region Instantiate Strings
			string activeProblems="";
			string address="";
			string activeAllergies="";
			string apptDateMonthSpelled="";
			string apptModNote="";//This is the Appointment Module Note in the Appointments for window.  http://www.opendental.com/manual/apptsched.html
			string apptProcs="";
			string apptProvNameFormal="";
			string apptsAllFuture="";
			string birthdate="";
			string carrierAddress="";
			string carrier2Address="";
			string carrierCityStZip="";
			string carrier2CityStZip="";
			string carrierName="";
			string carrier2Name="";
			string clinicPatDescription="";
			string clinicPatAddress="";
			string clinicPatCityStZip="";
			string clinicPatPhone="";
			string clinicCurDescription="";
			string clinicCurAddress="";
			string clinicCurCityStZip="";
			string clinicCurPhone="";
			string dateFirstVisit="";
			string dateOfLastSavedTP="";
			string dateRecallDue="";
			string dateTimeLastAppt="";
			string dateLastAppt="";
			string dateLastBW="";
			string dateLastExam="";
			string dateLastPerio="";
			string dateLastPanoFMX="";
			string dateLastProphy="";
			string dateLastSrp="";
			string dueForBWYN="";
			string dueForPanoYN="";
			string famPopups="";
			string famRecallDue="";
			string pronounHeSheThey="";
			string pronounheshethey="";
			string pronounHimHerThem="";
			string pronounhimherthem="";
			string pronounHimselfHerselfThemself="";
			string pronounhimselfherselfthemself="";
			string pronounHisHerTheir="";
			string pronounhishertheir="";
			string pronounHisHersTheirs="";
			string pronounhisherstheirs="";
			string guarantorHmPhone="";
			string guarantorNameF="";
			string guarantorNameFL="";
			string guarantorNameL="";
			string guarantorNamePref="";
			string guarantorNameLF="";
			string guarantorWirelessPhone="";
			string guarantorWkPhone="";
			string insAnnualMax="";
			string insDeductible="";
			string insDeductibleUsed="";
			string insEmployer="";
			string insFeeSchedule="";
			string insPending="";
			string insPercentages="";
			string insPlanGroupNumber="";
			string insPlanGroupName="";
			string insPlanNote="";
			string insSubNote="";
			string insSubBirthDate="";
			string insRemaining="";
			string insUsed="";
			string insFreqBW="";
			string insFreqExams="";
			string insFreqPanoFMX="";
			string insType=""; //(ppo, etc)
			string ins2AnnualMax="";
			string ins2Deductible="";
			string ins2DeductibleUsed="";
			string ins2Employer="";
			string ins2FreqBW="";
			string ins2FreqExams="";
			string ins2FreqPanoFMX="";
			string ins2PlanGroupNumber="";
			string ins2PlanGroupName="";
			string ins2Pending="";
			string ins2Percentages="";
			string ins2Remaining="";
			string ins2Used="";
			string medicalSummary="";
			string currentMedications="";
			string namePreferredOrFirst="";
			string nextSchedApptDateT="";
			string nextSchedApptDate="";
			string nextSchedApptsFam="";
			string patientPortalCredentials="";
			string phone="";
			string plannedAppointmentInfo="";
			string premedicateYN="";
			string recallInterval="";
			string recallScheduledYN="";
			string referredFrom=""; //(just one)
			string referredTo=""; //(typically Drs. could be multiline. Include date)
			string serviceNote="";
			string subscriberId="";
			string subscriberNameFL="";
			string subscriber2NameFL="";
			string tpResponsPartyAddress="";
			string tpResponsPartyCityStZip="";
			string tpResponsPartyNameFL="";
			string treatmentNote="";
			string treatmentPlanProcs="";
			string treatmentPlanProcsPriority="";
			#endregion
			Provider providerPri=null;
			PatientNote patientNote=null;
			#region Patient Fields
			if(patient!=null) {
				//Use patient's preferred name if they have one, otherwise default to first name.
				namePreferredOrFirst=patient.FName;
				if(!string.IsNullOrWhiteSpace(patient.Preferred)) {
					namePreferredOrFirst=patient.Preferred;
				}
				#region Procedure CodeNums
				//Procedure CodeNums-------------------------------------------------------------------------------------------------------------
				List<long> listProcCodeNums=GetListProcCodeNumsForStaticText(out List<long> listCodeNumsPano,out List<long> listCodeNumsExam,out List<long> listCodeNumsProphy,
					out List<long> listCodeNumsPerio,out List<long> listCodeNumsBW,out List<long> listCodeNumsSRP);
				#endregion
				family=family??Patients.GetFamily(patient.PatNum);
				staticTextData=StaticTextData.GetStaticTextData(staticTextFieldDependency,patient,family,listProcCodeNums,staticTextData);
				premedicateYN=Lans.g("All","No");
				recallScheduledYN=Lans.g("All","No");
				dueForBWYN=Lans.g("All","No");
				dueForPanoYN=Lans.g("All","No");
				if(patient.Premed) {
					premedicateYN=Lans.g("All","Yes");
				}
				patientNote=staticTextData.PatNote;
				medicalSummary=patientNote.Medical;
				treatmentNote=patientNote.Treatment;
				apptModNote=patient.ApptModNote;
				#region Gender
				#region Male
				string pronounHe="He";
				string pronounHim="Him";
				string pronounHimself="Himself";
				string pronounHis="His";
				#endregion
				#region Female
				string pronounShe="She";
				string pronounHer="Her";
				string pronounHerself="Herself";
				string pronounHers="Hers";
				#endregion
				#region Intersex
				string pronounThey="They";
				string pronounThem="Them";
				string pronounThemself="Themself";
				string pronounTheir="Their";
				string pronounTheirs="Theirs";
				#endregion
				#region Unknown
				string pronounUnknown="The patient";
				string pronounUnknowns="The patient's";
				#endregion
				PronounPreferred pronounPreferred=Patients.GetPronoun(patient.Gender,patientNote.Pronoun);
				switch(pronounPreferred) {
					case PronounPreferred.HeHim:
						pronounHeSheThey=Lans.g("PatientInfo",pronounHe);
						pronounheshethey=Lans.g("PatientInfo",pronounHe.ToLower());
						pronounHimHerThem=Lans.g("PatientInfo",pronounHim);
						pronounhimherthem=Lans.g("PatientInfo",pronounHim.ToLower());
						pronounHimselfHerselfThemself=Lans.g("PatientInfo",pronounHimself);
						pronounhimselfherselfthemself=Lans.g("PatientInfo",pronounHimself.ToLower());
						pronounHisHerTheir=Lans.g("PatientInfo",pronounHis);
						pronounhishertheir=Lans.g("PatientInfo",pronounHis.ToLower());
						pronounHisHersTheirs=Lans.g("PatientInfo",pronounHis);
						pronounhisherstheirs=Lans.g("PatientInfo",pronounHis.ToLower());
						break;
					case PronounPreferred.SheHer:
						pronounHeSheThey=Lans.g("PatientInfo",pronounShe);
						pronounheshethey=Lans.g("PatientInfo",pronounShe.ToLower());
						pronounHimHerThem=Lans.g("PatientInfo",pronounHer);
						pronounhimherthem=Lans.g("PatientInfo",pronounHer.ToLower());
						pronounHimselfHerselfThemself=Lans.g("PatientInfo",pronounHerself);
						pronounhimselfherselfthemself=Lans.g("PatientInfo",pronounHerself.ToLower());
						pronounHisHerTheir=Lans.g("PatientInfo",pronounHer);
						pronounhishertheir=Lans.g("PatientInfo",pronounHer.ToLower());
						pronounHisHersTheirs=Lans.g("PatientInfo",pronounHers);
						pronounhisherstheirs=Lans.g("PatientInfo",pronounHers.ToLower());
						break;
					case PronounPreferred.TheyThem:
						pronounHeSheThey=Lans.g("PatientInfo",pronounThey);
						pronounheshethey=Lans.g("PatientInfo",pronounThey.ToLower());
						pronounHimHerThem=Lans.g("PatientInfo",pronounThem);
						pronounhimherthem=Lans.g("PatientInfo",pronounThem.ToLower());
						pronounHimselfHerselfThemself=Lans.g("PatientInfo",pronounThemself);
						pronounhimselfherselfthemself=Lans.g("PatientInfo",pronounThemself.ToLower());
						pronounHisHerTheir=Lans.g("PatientInfo",pronounTheir);
						pronounhishertheir=Lans.g("PatientInfo",pronounTheir.ToLower());
						pronounHisHersTheirs=Lans.g("PatientInfo",pronounTheirs);
						pronounhisherstheirs=Lans.g("PatientInfo",pronounTheirs.ToLower());
						break;
					case PronounPreferred.None:
						pronounHeSheThey=Lans.g("PatientInfo",pronounUnknown);
						pronounheshethey=Lans.g("PatientInfo",pronounUnknown.ToLower());
						pronounHimHerThem=Lans.g("PatientInfo",pronounUnknown);
						pronounhimherthem=Lans.g("PatientInfo",pronounUnknown.ToLower());
						pronounHimselfHerselfThemself=Lans.g("PatientInfo",pronounUnknown);
						pronounhimselfherselfthemself=Lans.g("PatientInfo",pronounUnknown.ToLower());
						pronounHisHerTheir=Lans.g("PatientInfo",pronounUnknowns);
						pronounhishertheir=Lans.g("PatientInfo",pronounUnknowns.ToLower());
						pronounHisHersTheirs=Lans.g("PatientInfo",pronounUnknowns);
						pronounhisherstheirs=Lans.g("PatientInfo",pronounUnknowns.ToLower());
						break;
				}
				#endregion
				#region Guarantor
				Patient guar=family.Guarantor??Patients.GetPat(patient.Guarantor);
				if(guar!=null) {
					guarantorNameF=guar.FName;
					guarantorNameFL=guar.GetNameFL();
					guarantorNameL=guar.LName;
					guarantorNameLF=guar.GetNameLF();
					guarantorNamePref=guar.Preferred;
					guarantorHmPhone=guar.HmPhone;
					guarantorWirelessPhone=guar.WirelessPhone;
					guarantorWkPhone=guar.WkPhone;
				}
				#endregion
				#region Address
				address=patient.Address;
				if(patient.Address2!="") {
					address+=", "+patient.Address2;
				}
				#endregion
				#region Birthdate
				birthdate=patient.Birthdate.ToShortDateString();
				if(patient.Birthdate.Year<1880) {
					birthdate="";
				}
				#endregion
				#region Date First Visit
				dateFirstVisit=patient.DateFirstVisit.ToShortDateString();
				if(patient.DateFirstVisit.Year<1880) {
					dateFirstVisit="";
				}
				#endregion
				#region Treatment Plan Procs
				//todo some day: move this section down to TP section
				List<Procedure> procsList=null;//there is another variable that does the same thing. Carefully combine them.
				if(listEnumStaticTextFields.Contains(EnumStaticTextField.treatmentPlanProcs)
					|| listEnumStaticTextFields.Contains(EnumStaticTextField.plannedAppointmentInfo)
					|| listEnumStaticTextFields.Contains(EnumStaticTextField.treatmentPlanProcsPriority))
				{
					procsList=staticTextData.ListProceduresPat;
					if(staticTextData.ListSelectedTpProcs.Count>0) {
						procsList=staticTextData.ListSelectedTpProcs;
					}
					if(listEnumStaticTextFields.Contains(EnumStaticTextField.treatmentPlanProcs) || listEnumStaticTextFields.Contains(EnumStaticTextField.treatmentPlanProcsPriority)){
						List<Procedure> listProceduresTPs=Procedures.GetListTPandTPi(procsList);//sorted by priority, then toothnum
						for(int i=0;i<listProceduresTPs.Count;i++) {
							if(listProceduresTPs[i].ProcStatus!=ProcStat.TP) {
								continue;
							}
							if(treatmentPlanProcs!="") {
								treatmentPlanProcs+="\r\n";
								treatmentPlanProcsPriority+="\r\n";
							}
							//Figure out what the procedure description will be like.
							string procDescript=ProcedureCodes.GetStringProcCode(listProceduresTPs[i].CodeNum)+", "
								+Procedures.GetDescription(listProceduresTPs[i])+", "
								+listProceduresTPs[i].ProcFee.ToString("c");
							//Get the procedure's priority.
							string priority=Defs.GetName(DefCat.TxPriorities,listProceduresTPs[i].Priority);
							if(priority=="") {
								priority=Lans.g("TreatmentPlans","No priority");
							}
							//Set the corresponding static field text.
							treatmentPlanProcsPriority+=priority+", "+procDescript;
							treatmentPlanProcs+=procDescript;
						}
					}
				}
				#endregion
				serviceNote=patientNote.Service;
				#region Referrals
				List<RefAttach> RefAttachList=staticTextData.ListRefAttaches;
				Referral tempReferralFrom = Referrals.GetReferralForPat(patient.PatNum,RefAttachList);
				if(tempReferralFrom!=null) {
					if(tempReferralFrom.IsDoctor) {
						referredFrom+=tempReferralFrom.FName+" "+tempReferralFrom.LName+" "+tempReferralFrom.Title+" : "
							+Defs.GetName(DefCat.ProviderSpecialties,tempReferralFrom.Specialty);
					}
					else {
						referredFrom+=tempReferralFrom.FName+" "+tempReferralFrom.LName;
					}
				}
				for(int i=0;i<RefAttachList.Count;i++) {
					if(RefAttachList[i].RefType!=ReferralType.RefTo) {
						continue;
					}
					Referral tempRef;
					if(Referrals.TryGetReferral(RefAttachList[i].ReferralNum,out tempRef)) {
						if(tempRef.IsDoctor) {
							referredTo+=tempRef.FName+" "+tempRef.LName+" "+tempRef.Title+" : "+Defs.GetName(DefCat.ProviderSpecialties,tempRef.Specialty)+" "
								+RefAttachList[i].RefDate.ToShortDateString()+"\r\n";
						}
						else {
							referredTo+=tempRef.FName+" "+tempRef.LName+" "+RefAttachList[i].RefDate.ToShortDateString()+"\r\n";
						}
					}
				}
				#endregion
				#region Insurance
				//Insurance-------------------------------------------------------------------------------------------------------------------
				List<PatPlan> listPatPlans=staticTextData.ListPatPlans;
				List<InsSub> listInsSubs=staticTextData.ListInsSubs;
				List<InsPlan> listInsPlans=staticTextData.ListInsPlans;
				if(!PatPlans.IsPatPlanListValid(listPatPlans,listInsSubs:listInsSubs,listInsPlans:listInsPlans)) {
					//need to validate due to call to GetHistList below
					listPatPlans=PatPlans.Refresh(patient.PatNum);
				}
				int ordinal=PatPlans.GetOrdinal(PriSecMed.Primary,listPatPlans,listInsPlans,listInsSubs);
				if(ordinal==0) { //No primary dental plan. See if they have a medical plan instead.
					ordinal=PatPlans.GetOrdinal(PriSecMed.Medical,listPatPlans,listInsPlans,listInsSubs);
				}
				long subNum=PatPlans.GetInsSubNum(listPatPlans,ordinal);
				long patPlanNum=PatPlans.GetPatPlanNum(subNum,listPatPlans);
				InsSub sub=InsSubs.GetSub(subNum,listInsSubs);
				Patient subscriber;
				if(sub.Subscriber==patient.PatNum) {
					subscriber=patient;
				}
				else if(guar!=null && sub.Subscriber==guar.PatNum) {
					subscriber=guar;
				}
				else {
					subscriber=Patients.GetPat(sub.Subscriber);
				}
				if(subscriber!=null) {
					insSubBirthDate=subscriber.Birthdate.ToShortDateString();
				}
				InsPlan plan=null;
				if(sub!=null) {
					plan=InsPlans.GetPlan(sub.PlanNum,listInsPlans);
					insSubNote=sub.SubscNote;
				}
				Carrier carrier=null;
				List<Benefit> benefitList=staticTextData.ListBenefits;
				List<ClaimProcHist> histList=staticTextData.HistList;
				double doubAnnualMax;
				double doubDeductible;
				double doubDeductibleUsed;
				double doubPending;
				double doubRemain;
				double doubUsed;
				if(plan!=null) {
					insFeeSchedule=FeeScheds.GetDescription(plan.FeeSched);
					insPlanGroupName=plan.GroupName;
					insPlanGroupNumber=plan.GroupNum;
					insPlanNote=plan.PlanNote;
					carrier=Carriers.GetCarrier(plan.CarrierNum);
					carrierName=carrier.CarrierName;
					carrierAddress=carrier.Address;
					if(carrier.Address2!="") {
						carrierAddress+=", "+carrier.Address2;
					}
					carrierCityStZip=carrier.City+", "+carrier.State+"  "+carrier.Zip;
					subscriberId=sub.SubscriberID;
					if(subscriber!=null) {
						subscriberNameFL=subscriber.GetNameFL();
					}
					doubAnnualMax=Benefits.GetAnnualMaxDisplay(benefitList,plan.PlanNum,patPlanNum,false);
					doubRemain=-1;
					if(doubAnnualMax!=-1) {
						insAnnualMax=doubAnnualMax.ToString("c");
						doubRemain=doubAnnualMax;
					}
					doubDeductible=Benefits.GetDeductGeneralDisplay(benefitList,plan.PlanNum,patPlanNum,BenefitCoverageLevel.Individual);
					if(doubDeductible!=-1) {
						insDeductible=doubDeductible.ToString("c");
					}
					doubDeductibleUsed=InsPlans.GetDedUsedDisplay(histList,DateTime.Today,plan.PlanNum,patPlanNum,-1,listInsPlans,BenefitCoverageLevel.Individual,patient.PatNum);
					if(doubDeductibleUsed!=-1) {
						insDeductibleUsed=doubDeductibleUsed.ToString("c");
					}
					doubPending=InsPlans.GetPendingDisplay(histList,DateTime.Today,plan,patPlanNum,-1,patient.PatNum,subNum,benefitList);
					if(doubPending!=-1) {
						insPending=doubPending.ToString("c");
						if(doubRemain!=-1) {
							doubRemain-=doubPending;
						}
					}
					doubUsed=InsPlans.GetInsUsedDisplay(histList,DateTime.Today,plan.PlanNum,patPlanNum,-1,listInsPlans,benefitList,patient.PatNum,subNum);
					if(doubUsed!=-1) {
						insUsed=doubUsed.ToString("c");
						if(doubRemain!=-1) {
							doubRemain-=doubUsed;
						}
					}
					if(doubRemain!=-1) {
						insRemaining=doubRemain.ToString("c");
					}
					for(int j=0;j<benefitList.Count;j++) {
						if(benefitList[j].PlanNum != plan.PlanNum) {
							continue;
						}
						if(benefitList[j].BenefitType != InsBenefitType.CoInsurance) {
							continue;
						}
						if(insPercentages!="") {
							insPercentages+=",  ";
						}
						insPercentages+=CovCats.GetDesc(benefitList[j].CovCatNum)+" "+benefitList[j].Percent.ToString()+"%";
					}
					insFreqBW=Benefits.GetFrequencyDisplay(FrequencyType.BW,benefitList,plan.PlanNum);
					insFreqExams=Benefits.GetFrequencyDisplay(FrequencyType.Exam,benefitList,plan.PlanNum);
					insFreqPanoFMX=Benefits.GetFrequencyDisplay(FrequencyType.PanoFMX,benefitList,plan.PlanNum);
					switch(plan.PlanType) {//(ppo, etc)
						case "p":
							insType=Lans.g("InsurancePlans","PPO Percentage");
							break;
						case "f":
							insType=Lans.g("InsurancePlans","Medicaid or Flat Copay");
							break;
						case "c":
							insType=Lans.g("InsurancePlans","Capitation");
							break;
						case "":
							insType=Lans.g("InsurancePlans","Category Percentage");
							break;
					}
					insEmployer=Employers.GetEmployer(plan.EmployerNum).EmpName; //blank if no Employer listed
				}
				subNum=PatPlans.GetInsSubNum(listPatPlans,PatPlans.GetOrdinal(PriSecMed.Secondary,listPatPlans,listInsPlans,listInsSubs));
				patPlanNum=PatPlans.GetPatPlanNum(subNum,listPatPlans);
				sub=InsSubs.GetSub(subNum,listInsSubs);
				if(sub!=null) {
					plan=InsPlans.GetPlan(sub.PlanNum,listInsPlans);
				}
				if(plan!=null) { //secondary insurance
					ins2PlanGroupName=plan.GroupName;
					ins2PlanGroupNumber=plan.GroupNum;
					carrier=Carriers.GetCarrier(plan.CarrierNum);
					carrier2Name=carrier.CarrierName;
					carrier2Address=carrier.Address;
					if(carrier.Address2!="") {
						carrier2Address+=", "+carrier.Address2;
					}
					carrier2CityStZip=carrier.City+", "+carrier.State+"  "+carrier.Zip;
					//subscriberId=plan.SubscriberID;
					subscriber2NameFL=Patients.GetLim(sub.Subscriber).GetNameFL();
					doubAnnualMax=Benefits.GetAnnualMaxDisplay(benefitList,plan.PlanNum,patPlanNum,false);
					doubRemain=-1;
					if(doubAnnualMax!=-1) {
						ins2AnnualMax=doubAnnualMax.ToString("c");
						doubRemain=doubAnnualMax;
					}
					doubDeductible=Benefits.GetDeductGeneralDisplay(benefitList,plan.PlanNum,patPlanNum,BenefitCoverageLevel.Individual);
					if(doubDeductible!=-1) {
						ins2Deductible=doubDeductible.ToString("c");
					}
					doubDeductibleUsed=InsPlans.GetDedUsedDisplay(histList,DateTime.Today,plan.PlanNum,patPlanNum,-1,listInsPlans,BenefitCoverageLevel.Individual,patient.PatNum);
					if(doubDeductibleUsed!=-1) {
						ins2DeductibleUsed=doubDeductibleUsed.ToString("c");
					}
					doubPending=InsPlans.GetPendingDisplay(histList,DateTime.Today,plan,patPlanNum,-1,patient.PatNum,subNum,benefitList);
					if(doubPending!=-1) {
						ins2Pending=doubPending.ToString("c");
						if(doubRemain!=-1) {
							doubRemain-=doubPending;
						}
					}
					doubUsed=InsPlans.GetInsUsedDisplay(histList,DateTime.Today,plan.PlanNum,patPlanNum,-1,listInsPlans,benefitList,patient.PatNum,subNum);
					if(doubUsed!=-1) {
						ins2Used=doubUsed.ToString("c");
						if(doubRemain!=-1) {
							doubRemain-=doubUsed;
						}
					}
					if(doubRemain!=-1) {
						ins2Remaining=doubRemain.ToString("c");
					}
					for(int j=0;j<benefitList.Count;j++) {
						if(benefitList[j].PlanNum != plan.PlanNum) {
							continue;
						}
						if(benefitList[j].BenefitType != InsBenefitType.CoInsurance) {
							continue;
						}
						if(ins2Percentages!="") {
							ins2Percentages+=",  ";
						}
						ins2Percentages+=CovCats.GetDesc(benefitList[j].CovCatNum)+" "+benefitList[j].Percent.ToString()+"%";
					}
					ins2FreqBW=Benefits.GetFrequencyDisplay(FrequencyType.BW,benefitList,plan.PlanNum);
					ins2FreqExams=Benefits.GetFrequencyDisplay(FrequencyType.Exam,benefitList,plan.PlanNum);
					ins2FreqPanoFMX=Benefits.GetFrequencyDisplay(FrequencyType.PanoFMX,benefitList,plan.PlanNum);
					ins2Employer=Employers.GetEmployer(plan.EmployerNum).EmpName;//blank if no Employer listed
				}
				#endregion
				#region Treatment Plan
				//Treatment plan-----------------------------------------------------------------------------------------------------------
				List<TreatPlan> treatPlanList=staticTextData.ListTreatPlans;
				TreatPlan treatPlan=null;
				if(treatPlanList.Count>0) {
					treatPlan=treatPlanList[treatPlanList.Count-1].Copy();
					dateOfLastSavedTP=treatPlan.DateTP.ToShortDateString();
					Patient patRespParty=Patients.GetPat(treatPlan.ResponsParty);
					if(patRespParty!=null) {
						tpResponsPartyAddress=patRespParty.Address;
						if(patRespParty.Address2!="") {
							tpResponsPartyAddress+=", "+patRespParty.Address2;
						}
						tpResponsPartyCityStZip=patRespParty.City+", "+patRespParty.State+"  "+patRespParty.Zip;
						tpResponsPartyNameFL=patRespParty.GetNameFL();
					}
				}
				#endregion
				#region Procedure Log
				//Procedure Log-------------------------------------------------------------------------------------------------------------
				List<Procedure> proceduresList=staticTextData.ListProceduresSome;
				DateTime dBW=DateTime.MinValue;
				DateTime dExam=DateTime.MinValue;
				DateTime dPerio=DateTime.MinValue;
				DateTime dPanoFMX=DateTime.MinValue;
				DateTime dProphy=DateTime.MinValue;
				DateTime dSRP=DateTime.MinValue;
				for(int i=0;i<proceduresList.Count;i++) {
					Procedure proc = proceduresList[i];//cache Proc to speed up process
					if(proc.ProcStatus!=ProcStat.C
						&& proc.ProcStatus!=ProcStat.EC
						&& proc.ProcStatus!=ProcStat.EO) {
						continue;//only look at completed or existing procedures
					}
					if(listCodeNumsBW.Contains(proc.CodeNum) && proc.ProcDate>dBW){ //newest
						dBW=proc.ProcDate;
						dateLastBW=proc.ProcDate.ToShortDateString();
					}
					if(listCodeNumsExam.Contains(proc.CodeNum) && proc.ProcDate>dExam) //newest
					{
						dExam=proc.ProcDate;
						dateLastExam=proc.ProcDate.ToShortDateString();
					}
					if(listCodeNumsPerio.Contains(proc.CodeNum)//Periodontal Maintenance 
						&& proc.ProcDate>dPerio)//newest 
					{
						dPerio=proc.ProcDate;
						dateLastPerio=proc.ProcDate.ToShortDateString();
					}
					if((listCodeNumsPano.Contains(proc.CodeNum))//panoramic film
						&& proc.ProcDate>dPanoFMX) //newest
					{
						dPanoFMX=proc.ProcDate;
						dateLastPanoFMX=proc.ProcDate.ToShortDateString();
					}
					if(listCodeNumsProphy.Contains(proc.CodeNum) && proc.ProcDate>dProphy) //newest
					{
						dProphy=proc.ProcDate;
						dateLastProphy=proc.ProcDate.ToShortDateString();
					}
					if(listCodeNumsSRP.Contains(proc.CodeNum) && proc.ProcDate>dSRP) {
						dSRP=proc.ProcDate;
						dateLastSrp=proc.ProcDate.ToShortDateString();
					}
				}
				#endregion
				#region Recall
				//Recall--------------------------------------------------------------------------------------------------------------------
				List<Recall> listRecalls=staticTextData.ListRecallsForFam.FindAll(x => x.PatNum==patient.PatNum);
				for(int i=0;i<listRecalls.Count;i++) {
					//don't care about recalls in the future, or without a due date as these recalls won't even show in the recall list.
					if(listRecalls[i].DateDue>DateTime.Today || listRecalls[i].DateDue.Year<1880) { 
						continue;
					}
					if(listRecalls[i].IsDisabled) { //don't care about recalls that are disabled.
						continue;
					}
					if(listRecalls[i].DisableUntilDate>DateTime.Today) { //don't care about recalls that are disabled until the future
						continue;
					}
					if(listRecalls[i].DisableUntilBalance>0 && listRecalls[i].DisableUntilBalance<(family.ListPats[0].BalTotal-family.ListPats[0].InsEst)) { 
						//don't care about recalls if they are disabled due to family balance
						continue;
					}
					List<string> listProcCodes=RecallTypes.GetProcs(listRecalls[i].RecallTypeNum);
					for(int j=0;j<listProcCodes.Count;j++) {
						if(listProcCodes[j]=="D0210"//intraoral - complete series (including bitewings) 				   
							||listProcCodes[j]=="D0270"//bitewing - single film													   
							||listProcCodes[j]=="D0272"//bitewings - two films														   
							||listProcCodes[j]=="D0274"//bitewings - four films													   
							||listProcCodes[j]=="D0277"//vertical bitewings - 7 to 8 films											   
							||listProcCodes[j]=="D0273")//bitewings - three films
						{
							dueForBWYN=Lans.g("All","Yes");
						}
						if(listProcCodes[j]=="D0210"//intraoral - complete series (including bitewings)				   
							||listProcCodes[j]=="D0330")//panoramic film
						{
							dueForPanoYN=Lans.g("All","Yes");
						}
					}
				}
				Recall recall=Recalls.GetRecallProphyOrPerio(patient.PatNum,listRecalls: staticTextData.ListRecallsForFam);
				if(recall!=null && !recall.IsDisabled) {
					if(recall.DateDue.Year>1880) {
						dateRecallDue=recall.DateDue.ToShortDateString();
					}
					recallInterval=recall.RecallInterval.ToString();
					if(recall.DateScheduled>=DateTime.Today) {
						recallScheduledYN=Lans.g("All","Yes");
					}
				}
				for(int i=0;i<family.ListPats.Length;i++) {
					recall=Recalls.GetRecallProphyOrPerio(family.ListPats[i].PatNum,true,listRecalls: staticTextData.ListRecallsForFam);
					if(recall==null || recall.IsDisabled || recall.DateDue==DateTime.MinValue || recall.DateDue>=DateTime.Today) {
						continue;
					}
					if(famRecallDue!="") {
						famRecallDue+="\r\n";
					}
					famRecallDue+=family.ListPats[i].FName+", "+recall.DateDue.ToShortDateString()+" "+RecallTypes.GetDescription(recall.RecallTypeNum);
				}
				#endregion
				#region Appointments
				//Appointments--------------------------------------------------------------------------------------------------------------
				List<Appointment> apptList=staticTextData.ListAppts;
				List<Appointment> apptFutureList=staticTextData.ListFutureApptsForFam.FindAll(x => x.PatNum==patient.PatNum);
				for(int i=0;i<apptList.Count;i++) {
					if(apptList[i].AptStatus != ApptStatus.Scheduled
					&& apptList[i].AptStatus != ApptStatus.Complete
					&& apptList[i].AptStatus != ApptStatus.None) {
						continue;
					}
					if(apptList[i].AptDateTime < DateTime.Now) {
						//this will happen repeatedly up until the most recent.
						dateTimeLastAppt=apptList[i].AptDateTime.ToShortDateString()+"  "+apptList[i].AptDateTime.ToShortTimeString();
						dateLastAppt=apptList[i].AptDateTime.ToShortDateString();
					}
					else {//after now
						if(nextSchedApptDateT=="") {//only the first one found
							nextSchedApptDateT=apptList[i].AptDateTime.ToShortDateString()+"  "+apptList[i].AptDateTime.ToShortTimeString();
							nextSchedApptDate=apptList[i].AptDateTime.ToShortDateString();
							break;//we're done with the list now.
						}
					}
				}
				for(int i=0;i<apptFutureList.Count;i++) {//cannot be combined in loop above because of the break in the loop.
					apptsAllFuture+=apptFutureList[i].AptDateTime.ToShortDateString()+" "+apptFutureList[i].AptDateTime.ToShortTimeString()+" : "+apptFutureList[i].ProcDescript+"\r\n";
				}
				for(int i=0;i<family.ListPats.Length;i++) {
					List<Appointment> futAptsList=staticTextData.ListFutureApptsForFam.FindAll(x => x.PatNum==family.ListPats[i].PatNum);
					if(futAptsList.Count>0) {//just gets one future appt for each person
						nextSchedApptsFam+=family.ListPats[i].FName+": "+futAptsList[0].AptDateTime.ToShortDateString()+" "+futAptsList[0].AptDateTime.ToShortTimeString()+" : "+futAptsList[0].ProcDescript+"\r\n";
					}
				}
				if(listEnumStaticTextFields.Contains(EnumStaticTextField.plannedAppointmentInfo)){
					Appointment plannedAppt=staticTextData.ListPlannedAppts?.OrderBy(x => x.ItemOrderPlanned)?.FirstOrDefault();
					for(int i=0;i<apptList.Count;i++) {
						if(plannedAppt!=null && apptList[i].AptNum==plannedAppt.AptNum) {
							plannedAppointmentInfo=Lans.g("Appointments","Procedures:")+" ";
							plannedAppointmentInfo+=apptList[i].ProcDescript+"\r\n";
							int minutesTotal=apptList[i].Pattern.Length*5;
							int hours=minutesTotal/60;//automatically rounds down
							int minutes=minutesTotal-hours*60;
							plannedAppointmentInfo+=Lans.g("Appointments","Appt Length:")+" ";
							if(hours>0) {
								plannedAppointmentInfo+=hours.ToString()+" "+Lans.g("Appointments","hours")+", ";
							}
							plannedAppointmentInfo+=minutes.ToString()+" "+Lans.g("Appointments","min")+"\r\n";
						}
					}
				}
				if(aptNum>0){
					Appointment appointment=staticTextData.ListAppts.FirstOrDefault(x => x.AptNum==aptNum);
					if(appointment!=null) {
						if(appointment.AptDateTime.Year > 1880) {
							//Month spelled followed by day and year. E.g. April 28, 2018
							apptDateMonthSpelled=appointment.AptDateTime.ToString("MMMM dd, yyyy");
						}
						if(!staticTextData.ListProceduresPat.IsNullOrEmpty()) {
							List<Procedure> listProcedures=staticTextData.ListProceduresPat.FindAll(x => x.AptNum==appointment.AptNum);
							apptProcs=string.Join("\r\n",listProcedures.Select(x => Procedures.GetDescriptionForLetter(x)));
						}
						Provider provider=Providers.GetProv(appointment.ProvNum);
						if(provider!=null) {
							apptProvNameFormal=provider.GetFormalName();//Matches the format of priProvNameFormal.
						}
					}
				}
				#endregion
				#region Clinic
				providerPri=Providers.GetProv(Patients.GetProvNum(patient));
				if(providerPri==null) {//Rare, temporary fix. We have previously seen this issue happen when a provider is removed from the DB somehow.
					providerPri=new Provider() {//If Provider is missing this will throw a UE where we attempt to use GetFullName(...)
						FName="",
						LName="",
						MI="",//This is the lynch pin to ensuring GetFullName(...) does not fail when looking for a missing Provider.
					};
				}
				//Pat Clinic-------------------------------------------------------------------------------------------------------------
				Clinic clinic=Clinics.GetClinic(patient.ClinicNum);
				if(clinic==null) {
					clinicPatDescription=PrefC.GetString(PrefName.PracticeTitle);
					clinicPatAddress=PrefC.GetString(PrefName.PracticeAddress);
					if(PrefC.GetString(PrefName.PracticeAddress2)!="") {
						clinicPatAddress+=", "+PrefC.GetString(PrefName.PracticeAddress2);
					}
					clinicPatCityStZip=PrefC.GetString(PrefName.PracticeCity)+", "+PrefC.GetString(PrefName.PracticeST)+"  "+PrefC.GetString(PrefName.PracticeZip);
					phone=PrefC.GetString(PrefName.PracticePhone);
				}
				else {
					clinicPatDescription=clinic.Description;
					clinicPatAddress=clinic.Address;
					if(clinic.Address2!="") {
						clinicPatAddress+=", "+clinic.Address2;
					}
					clinicPatCityStZip=clinic.City+", "+clinic.State+"  "+clinic.Zip;
					phone=clinic.Phone;
				}
				clinicPatPhone=TelephoneNumbers.ReFormat(phone);
				//Current selected Clinic-------------------------------------------------------------------------------------------------------
				Clinic clinicCur=Clinics.GetClinic(Clinics.ClinicNum);
				if(clinicCur==null) {
					clinicCurDescription=PrefC.GetString(PrefName.PracticeTitle);
					clinicCurAddress=PrefC.GetString(PrefName.PracticeAddress);
					if(PrefC.GetString(PrefName.PracticeAddress2)!="") {
						clinicCurAddress+=", "+PrefC.GetString(PrefName.PracticeAddress2);
					}
					clinicCurCityStZip=PrefC.GetString(PrefName.PracticeCity)+", "+PrefC.GetString(PrefName.PracticeST)+"  "+PrefC.GetString(PrefName.PracticeZip);
					phone=PrefC.GetString(PrefName.PracticePhone);
				}
				else {
					clinicCurDescription=clinicCur.Description;
					clinicCurAddress=clinicCur.Address;
					if(clinicCur.Address2!="") {
						clinicCurAddress+=", "+clinicCur.Address2;
					}
					clinicCurCityStZip=clinicCur.City+", "+clinicCur.State+"  "+clinicCur.Zip;
					phone=clinicCur.Phone;
				}
				clinicCurPhone=TelephoneNumbers.ReFormat(phone);
				#endregion
				#region Diseases/Allergies
				List<Disease> listDiseases=staticTextData.ListDiseases;
				for(int i=0;i<listDiseases.Count;i++) {
					if(activeProblems!="") {
						activeProblems+=", ";
					}
					activeProblems+=DiseaseDefs.GetName(listDiseases[i].DiseaseDefNum);
				}
				List<Allergy> listAllergies=staticTextData.ListAllergies;
				for(int i=0;i<listAllergies.Count;i++) {
					if(activeAllergies!="") {
						activeAllergies+=", ";
					}
					try {//unknown methods can cause the allergy to be deleted sometimes
						activeAllergies+=AllergyDefs.GetDescription(listAllergies[i].AllergyDefNum);
					}
					catch {
						continue;
					}
				}
				#endregion
				#region Medication
				List<MedicationPat> listMedicationPats=staticTextData.ListMedicationPats;
				foreach(MedicationPat medPat in listMedicationPats) {
					//Default to using the MedicationPat description (e.g. an eRx from NewCrop that is not in the Medication table).
					string medDescript=medPat.MedDescript??"";
					//Prefer the description associated to the Medication object if available.
					Medication medCur=Medications.GetMedication(medPat.MedicationNum);
					if(medCur!=null) {
						medDescript=medCur.MedName;
						if(medCur.MedicationNum!=medCur.GenericNum) {
							Medication medGeneric=Medications.GetMedication(medCur.GenericNum);
							medDescript+=(medGeneric==null) ? "" : " ("+medGeneric.MedName+")";
						}
					}
					currentMedications+=((currentMedications=="") ? "" : ", ")+medDescript;
				}
				#endregion
				#region Popups
				//patient, family & superfam popups
				List<Popup> listFamPopups=staticTextData.ListFamPopups;
				for(int i=0;i<listFamPopups.Count;i++) {
					if(listFamPopups[i].DateTimeDisabled!=DateTime.MinValue && listFamPopups[i].DateTimeDisabled < DateTime.Now) { //If disabled
						continue;
					}
					if(famPopups!="") {
						famPopups+=", ";
					}
					famPopups+=listFamPopups[i].Description;
				}
				#endregion
				#region Patient Portal
				if(listEnumStaticTextFields.Contains(EnumStaticTextField.patientPortalCredentials)){
					PatientPortalCredential patientPortalCredential=UserWebs.GetNewPatientPortalCredentials(patient);
					if(UserWebs.UpdateNewPatientPortalCredentials(patientPortalCredential)) {
						patientPortalCredentials=Lans.g("PatientPortal","Patient Portal Login")+"\r\n"
							+Lans.g("PatientPortal","UserName:")+" "+patientPortalCredential.GetUserName()+"\r\n"
							+Lans.g("PatientPortal","Password:")+" "+patientPortalCredential.PlainTextPassword;
					}
				}
				#endregion Patient Portal
			}//End of if(pat!=null)
			#endregion
			List<StaticTextReplacement> listStaticTextReplacements=new List<StaticTextReplacement>();
			for(int i=0;i<listEnumStaticTextFields.Count;i++){
				StaticTextReplacement staticTextReplacement=new StaticTextReplacement();
				staticTextReplacement.StaticTextField=listEnumStaticTextFields[i];
				switch(listEnumStaticTextFields[i]) {
					case EnumStaticTextField.dateToday: staticTextReplacement.NewValue=DateTime.Today.ToShortDateString(); break;
					case EnumStaticTextField.dateTodayLong: staticTextReplacement.NewValue=DateTime.Today.ToLongDateString(); break;
					case EnumStaticTextField.practiceTitle: staticTextReplacement.NewValue=PrefC.GetString(PrefName.PracticeTitle); break;
				}
				if(patient==null) {
					listStaticTextReplacements.Add(staticTextReplacement);
					continue;
				}
				switch(listEnumStaticTextFields[i]){
					case EnumStaticTextField.activeAllergies: staticTextReplacement.NewValue=activeAllergies; break;
					case EnumStaticTextField.activeProblems: staticTextReplacement.NewValue=activeProblems; break;
					case EnumStaticTextField.address: staticTextReplacement.NewValue=address; break;
					case EnumStaticTextField.apptsAllFuture: staticTextReplacement.NewValue=apptsAllFuture.TrimEnd(); break;
					case EnumStaticTextField.apptDateMonthSpelled: staticTextReplacement.NewValue=apptDateMonthSpelled; break;
					case EnumStaticTextField.apptModNote: staticTextReplacement.NewValue=apptModNote; break;
					case EnumStaticTextField.apptProcs: staticTextReplacement.NewValue=apptProcs; break;
					case EnumStaticTextField.apptProvNameFormal: staticTextReplacement.NewValue=apptProvNameFormal; break;
					case EnumStaticTextField.age: staticTextReplacement.NewValue=Patients.AgeToString(patient.Age); break;
					case EnumStaticTextField.balTotal: staticTextReplacement.NewValue=family.ListPats[0].BalTotal.ToString("c"); break;
					case EnumStaticTextField.bal_0_30: staticTextReplacement.NewValue=family.ListPats[0].Bal_0_30.ToString("c"); break;
					case EnumStaticTextField.bal_31_60: staticTextReplacement.NewValue=family.ListPats[0].Bal_31_60.ToString("c"); break;
					case EnumStaticTextField.bal_61_90: staticTextReplacement.NewValue=family.ListPats[0].Bal_61_90.ToString("c"); break;
					case EnumStaticTextField.balOver90: staticTextReplacement.NewValue=family.ListPats[0].BalOver90.ToString("c"); break;
					case EnumStaticTextField.balInsEst: staticTextReplacement.NewValue=family.ListPats[0].InsEst.ToString("c"); break;
					case EnumStaticTextField.balTotalMinusInsEst: staticTextReplacement.NewValue=(family.ListPats[0].BalTotal-family.ListPats[0].InsEst).ToString("c"); break;
					case EnumStaticTextField.BillingType: staticTextReplacement.NewValue=Defs.GetName(DefCat.BillingTypes,patient.BillingType); break;
					case EnumStaticTextField.Birthdate: staticTextReplacement.NewValue=birthdate; break;
					case EnumStaticTextField.carrierName: staticTextReplacement.NewValue=carrierName; break;
					case EnumStaticTextField.carrier2Name: staticTextReplacement.NewValue=carrier2Name; break;
					case EnumStaticTextField.ChartNumber: staticTextReplacement.NewValue=patient.ChartNumber; break;
					case EnumStaticTextField.carrierAddress: staticTextReplacement.NewValue=carrierAddress; break;
					case EnumStaticTextField.carrier2Address: staticTextReplacement.NewValue=carrier2Address; break;
					case EnumStaticTextField.carrierCityStZip: staticTextReplacement.NewValue=carrierCityStZip; break;
					case EnumStaticTextField.carrier2CityStZip: staticTextReplacement.NewValue=carrier2CityStZip; break;
					case EnumStaticTextField.cityStateZip: staticTextReplacement.NewValue=patient.City+", "+patient.State+"  "+patient.Zip; break;
					case EnumStaticTextField.clinicDescription: staticTextReplacement.NewValue=clinicPatDescription; break;
					case EnumStaticTextField.clinicAddress: staticTextReplacement.NewValue=clinicPatAddress; break;
					case EnumStaticTextField.clinicCityStZip: staticTextReplacement.NewValue=clinicPatCityStZip; break;
					case EnumStaticTextField.clinicPhone: staticTextReplacement.NewValue=clinicPatPhone; break;
					case EnumStaticTextField.clinicPatDescription: staticTextReplacement.NewValue=clinicPatDescription; break;
					case EnumStaticTextField.clinicPatAddress: staticTextReplacement.NewValue=clinicPatAddress; break;
					case EnumStaticTextField.clinicPatCityStZip: staticTextReplacement.NewValue=clinicPatCityStZip; break;
					case EnumStaticTextField.clinicPatPhone: staticTextReplacement.NewValue=clinicPatPhone; break;
					case EnumStaticTextField.clinicCurDescription: staticTextReplacement.NewValue=clinicCurDescription; break;
					case EnumStaticTextField.clinicCurAddress: staticTextReplacement.NewValue=clinicCurAddress; break;
					case EnumStaticTextField.clinicCurCityStZip: staticTextReplacement.NewValue=clinicCurCityStZip; break;
					case EnumStaticTextField.clinicCurPhone: staticTextReplacement.NewValue=clinicCurPhone; break;
					case EnumStaticTextField.currentMedications: staticTextReplacement.NewValue=currentMedications; break;
					case EnumStaticTextField.DateFirstVisit: staticTextReplacement.NewValue=dateFirstVisit; break;
					case EnumStaticTextField.dateLastAppt: staticTextReplacement.NewValue=dateLastAppt; break;
					case EnumStaticTextField.dateLastBW: staticTextReplacement.NewValue=dateLastBW; break;
					case EnumStaticTextField.dateLastExam: staticTextReplacement.NewValue=dateLastExam; break;
					case EnumStaticTextField.dateLastPerio: staticTextReplacement.NewValue=dateLastPerio; break;
					case EnumStaticTextField.dateLastPanoFMX: staticTextReplacement.NewValue=dateLastPanoFMX; break;
					case EnumStaticTextField.dateLastProphy: staticTextReplacement.NewValue=dateLastProphy; break;
					case EnumStaticTextField.dateLastSrp: staticTextReplacement.NewValue=dateLastSrp; break;
					case EnumStaticTextField.dateOfLastSavedTP: staticTextReplacement.NewValue=dateOfLastSavedTP; break;
					case EnumStaticTextField.dateRecallDue: staticTextReplacement.NewValue=dateRecallDue; break;
					case EnumStaticTextField.dateTimeLastAppt: staticTextReplacement.NewValue=dateTimeLastAppt; break;
					case EnumStaticTextField.dueForBWYN: staticTextReplacement.NewValue=dueForBWYN; break;
					case EnumStaticTextField.dueForPanoYN: staticTextReplacement.NewValue=dueForPanoYN; break;
					case EnumStaticTextField.Email: staticTextReplacement.NewValue=patient.Email; break;
					case EnumStaticTextField.famFinNote: staticTextReplacement.NewValue=patientNote.FamFinancial; break;
					case EnumStaticTextField.famFinUrgNote: staticTextReplacement.NewValue=family.ListPats[0].FamFinUrgNote; break;
					case EnumStaticTextField.famRecallDue: staticTextReplacement.NewValue=famRecallDue; break;
					case EnumStaticTextField.guarantorHmPhone: staticTextReplacement.NewValue=guarantorHmPhone; break;
					case EnumStaticTextField.guarantorNameF: staticTextReplacement.NewValue=guarantorNameF; break;
					case EnumStaticTextField.guarantorNameFL: staticTextReplacement.NewValue=guarantorNameFL; break;
					case EnumStaticTextField.guarantorNameL: staticTextReplacement.NewValue=guarantorNameL; break;
					case EnumStaticTextField.guarantorNamePref: staticTextReplacement.NewValue=guarantorNamePref; break;
					case EnumStaticTextField.guarantorNameLF: staticTextReplacement.NewValue=guarantorNameLF; break;
					case EnumStaticTextField.guarantorWirelessPhone: staticTextReplacement.NewValue=guarantorWirelessPhone; break;
					case EnumStaticTextField.guarantorWkPhone: staticTextReplacement.NewValue=guarantorWkPhone; break;
					case EnumStaticTextField.gender: staticTextReplacement.NewValue=Lans.g("enumPatientGender",patient.Gender.ToString()); break;
					case EnumStaticTextField.genderHeShe: staticTextReplacement.NewValue=pronounHeSheThey; break;
					case EnumStaticTextField.genderheshe: staticTextReplacement.NewValue=pronounheshethey; break;
					case EnumStaticTextField.genderHimHer: staticTextReplacement.NewValue=pronounHimHerThem; break;
					case EnumStaticTextField.genderhimher: staticTextReplacement.NewValue=pronounhimherthem; break;
					case EnumStaticTextField.genderHimselfHerself: staticTextReplacement.NewValue=pronounHimselfHerselfThemself; break;
					case EnumStaticTextField.genderhimselfherself: staticTextReplacement.NewValue=pronounhimselfherselfthemself; break;
					case EnumStaticTextField.genderHisHer: staticTextReplacement.NewValue=pronounHisHerTheir; break;
					case EnumStaticTextField.genderhisher: staticTextReplacement.NewValue=pronounhishertheir; break;
					case EnumStaticTextField.genderHisHers: staticTextReplacement.NewValue=pronounHisHersTheirs; break;
					case EnumStaticTextField.genderhishers: staticTextReplacement.NewValue=pronounhisherstheirs; break;
					case EnumStaticTextField.HmPhone: staticTextReplacement.NewValue=patient.HmPhone; break;
					case EnumStaticTextField.insAnnualMax: staticTextReplacement.NewValue=insAnnualMax; break;
					case EnumStaticTextField.insDeductible: staticTextReplacement.NewValue=insDeductible; break;
					case EnumStaticTextField.insDeductibleUsed: staticTextReplacement.NewValue=insDeductibleUsed; break;
					case EnumStaticTextField.insEmployer: staticTextReplacement.NewValue=insEmployer; break;
					case EnumStaticTextField.insFeeSchedule: staticTextReplacement.NewValue=insFeeSchedule; break;
					case EnumStaticTextField.insFreqBW: staticTextReplacement.NewValue=insFreqBW.TrimEnd(); break;
					case EnumStaticTextField.insFreqExams: staticTextReplacement.NewValue=insFreqExams.TrimEnd(); break;
					case EnumStaticTextField.insFreqPanoFMX: staticTextReplacement.NewValue=insFreqPanoFMX.TrimEnd(); break;
					case EnumStaticTextField.insPending: staticTextReplacement.NewValue=insPending; break;
					case EnumStaticTextField.insPercentages: staticTextReplacement.NewValue=insPercentages; break;
					case EnumStaticTextField.insPlanGroupNumber: staticTextReplacement.NewValue=insPlanGroupNumber; break;
					case EnumStaticTextField.insPlanGroupName: staticTextReplacement.NewValue=insPlanGroupName; break;
					case EnumStaticTextField.insPlanNote: staticTextReplacement.NewValue=insPlanNote; break;
					case EnumStaticTextField.insType: staticTextReplacement.NewValue=insType; break;
					case EnumStaticTextField.insSubBirthDate: staticTextReplacement.NewValue=insSubBirthDate; break;
					case EnumStaticTextField.insSubNote: staticTextReplacement.NewValue=insSubNote; break;
					case EnumStaticTextField.insRemaining: staticTextReplacement.NewValue=insRemaining; break;
					case EnumStaticTextField.insUsed: staticTextReplacement.NewValue=insUsed; break;
					case EnumStaticTextField.ins2AnnualMax: staticTextReplacement.NewValue=ins2AnnualMax; break;
					case EnumStaticTextField.ins2Deductible: staticTextReplacement.NewValue=ins2Deductible; break;
					case EnumStaticTextField.ins2DeductibleUsed: staticTextReplacement.NewValue=ins2DeductibleUsed; break;
					case EnumStaticTextField.ins2Employer: staticTextReplacement.NewValue=ins2Employer; break;
					case EnumStaticTextField.ins2FreqBW: staticTextReplacement.NewValue=ins2FreqBW.TrimEnd(); break;
					case EnumStaticTextField.ins2FreqExams: staticTextReplacement.NewValue=ins2FreqExams.TrimEnd(); break;
					case EnumStaticTextField.ins2FreqPanoFMX: staticTextReplacement.NewValue=ins2FreqPanoFMX.TrimEnd(); break;
					case EnumStaticTextField.ins2PlanGroupNumber: staticTextReplacement.NewValue=ins2PlanGroupNumber; break;
					case EnumStaticTextField.ins2PlanGroupName: staticTextReplacement.NewValue=ins2PlanGroupName; break;
					case EnumStaticTextField.ins2Pending: staticTextReplacement.NewValue=ins2Pending; break;
					case EnumStaticTextField.ins2Percentages: staticTextReplacement.NewValue=ins2Percentages; break;
					case EnumStaticTextField.ins2Remaining: staticTextReplacement.NewValue=ins2Remaining; break;
					case EnumStaticTextField.ins2Used: staticTextReplacement.NewValue=ins2Used; break;
					case EnumStaticTextField.medicalSummary: staticTextReplacement.NewValue=medicalSummary; break;
					case EnumStaticTextField.MedUrgNote: staticTextReplacement.NewValue=patient.MedUrgNote; break;
					case EnumStaticTextField.nameF: staticTextReplacement.NewValue=patient.FName; break;
					case EnumStaticTextField.nameFL: staticTextReplacement.NewValue=patient.GetNameFL(); break;
					case EnumStaticTextField.nameFLFormal: staticTextReplacement.NewValue=patient.GetNameFLFormal(); break;
					case EnumStaticTextField.nameL: staticTextReplacement.NewValue=patient.LName; break;
					case EnumStaticTextField.nameLF: staticTextReplacement.NewValue=patient.GetNameLF(); break;
					case EnumStaticTextField.nameMI: staticTextReplacement.NewValue=patient.MiddleI; break;
					case EnumStaticTextField.namePref: staticTextReplacement.NewValue=patient.Preferred; break;
					case EnumStaticTextField.namePreferredOrFirst: staticTextReplacement.NewValue=namePreferredOrFirst; break;
					case EnumStaticTextField.nextSchedApptDate: staticTextReplacement.NewValue=nextSchedApptDate; break;
					case EnumStaticTextField.nextSchedApptDateT: staticTextReplacement.NewValue=nextSchedApptDateT; break;
					case EnumStaticTextField.nextSchedApptsFam: staticTextReplacement.NewValue=nextSchedApptsFam.TrimEnd(); break;
					case EnumStaticTextField.PatNum: staticTextReplacement.NewValue=patient.PatNum.ToString(); break;
					case EnumStaticTextField.famPopups: staticTextReplacement.NewValue=famPopups; break;
					case EnumStaticTextField.patientPortalCredentials: staticTextReplacement.NewValue=patientPortalCredentials; break;
					case EnumStaticTextField.plannedAppointmentInfo: staticTextReplacement.NewValue=plannedAppointmentInfo; break;
					case EnumStaticTextField.premedicateYN: staticTextReplacement.NewValue=premedicateYN; break;
					case EnumStaticTextField.priProvNameFormal: staticTextReplacement.NewValue=providerPri.GetFormalName(); break;
					case EnumStaticTextField.recallInterval: staticTextReplacement.NewValue=recallInterval; break;
					case EnumStaticTextField.recallScheduledYN: staticTextReplacement.NewValue=recallScheduledYN; break;
					case EnumStaticTextField.referredFrom: staticTextReplacement.NewValue=referredFrom; break;
					case EnumStaticTextField.referredTo: staticTextReplacement.NewValue=referredTo.TrimEnd(); break;
					case EnumStaticTextField.salutation: staticTextReplacement.NewValue=patient.GetSalutation(); break;
					case EnumStaticTextField.serviceNote: staticTextReplacement.NewValue=serviceNote; break;
					case EnumStaticTextField.siteDescription: staticTextReplacement.NewValue=Sites.GetDescription(patient.SiteNum); break;
					case EnumStaticTextField.SSN: staticTextReplacement.NewValue=patient.SSN; break;
					case EnumStaticTextField.subscriberID: staticTextReplacement.NewValue=subscriberId; break;
					case EnumStaticTextField.subscriberNameFL: staticTextReplacement.NewValue=subscriberNameFL; break;
					case EnumStaticTextField.subscriber2NameFL: staticTextReplacement.NewValue=subscriber2NameFL; break;
					case EnumStaticTextField.timeNow: staticTextReplacement.NewValue=DateTime.Now.ToShortTimeString(); break;
					case EnumStaticTextField.tpResponsPartyAddress: staticTextReplacement.NewValue=tpResponsPartyAddress; break;
					case EnumStaticTextField.tpResponsPartyCityStZip: staticTextReplacement.NewValue=tpResponsPartyCityStZip; break;
					case EnumStaticTextField.tpResponsPartyNameFL: staticTextReplacement.NewValue=tpResponsPartyNameFL; break;
					case EnumStaticTextField.treatmentNote: staticTextReplacement.NewValue=treatmentNote; break;
					case EnumStaticTextField.treatmentPlanProcs: staticTextReplacement.NewValue=treatmentPlanProcs; break;
					case EnumStaticTextField.treatmentPlanProcsPriority: staticTextReplacement.NewValue=treatmentPlanProcsPriority; break;
					case EnumStaticTextField.WirelessPhone: staticTextReplacement.NewValue=patient.WirelessPhone; break;
					case EnumStaticTextField.WkPhone: staticTextReplacement.NewValue=patient.WkPhone; break;
				}
				listStaticTextReplacements.Add(staticTextReplacement);
			}
			return listStaticTextReplacements;
		}

		///<summary>Takes a list of replacements and actually performs the replacement within all the Sheet fields.</summary>
		private static void ReplaceStaticTextFieldsInSheet(List<StaticTextReplacement> listStaticTextReplacements,Sheet sheet,Patient patient,Family family){
			for(int f=0;f<sheet.SheetFields.Count;f++){
				if(sheet.SheetFields[f].FieldType!=SheetFieldType.StaticText) {
					continue;
				}
				for(int i=0;i<listStaticTextReplacements.Count;i++){
					sheet.SheetFields[f].FieldValue=sheet.SheetFields[f].FieldValue.Replace(
						"["+listStaticTextReplacements[i].StaticTextField.ToString()+"]",
						listStaticTextReplacements[i].NewValue);
				}
			}
			#region Fill Exam Sheet Fields
			//Fill Exam Sheet Fields----------------------------------------------------------------------------------------------
			//Example: ExamSheet:MyExamSheet;MyField
			if((sheet.SheetType==SheetTypeEnum.PatientLetter || sheet.SheetType==SheetTypeEnum.ReferralSlip || sheet.SheetType==SheetTypeEnum.ReferralLetter) && patient!=null) {
				for(int f=0;f<sheet.SheetFields.Count;f++){
					if(sheet.SheetFields[f].FieldType!=SheetFieldType.StaticText) {
						continue;
					}
					string fieldValue=sheet.SheetFields[f].FieldValue;
					string rgx=@"\[ExamSheet\:([^;]+);([^\]]+)\]";
					Match match=Regex.Match(fieldValue,rgx);
					while(match.Success) {
						string examSheetDescript=match.Result("$1");
						string fieldName=match.Result("$2");
						List<SheetField> examFields=SheetFields.GetFieldFromExamSheet(patient.PatNum,examSheetDescript,fieldName);//Either a list of fields (if radio button) or single field
						if(examFields==null || examFields.Count==0) {
							match=match.NextMatch();
							continue;
						}
						if(examFields[0].RadioButtonGroup!="") {//a user defined 'misc' radio button check box, find the selected item and replace with reportable name
							for(int i=0;i<examFields.Count;i++) {
								if(examFields[i].FieldValue=="X") {
									fieldValue=fieldValue.Replace(match.Value,examFields[i].ReportableName);//each radio button in the group has a different reportable name.
									break;
								}
							}
						}
						else if(examFields[0].ReportableName!="") {//not a radio button, so either user defined single misc check boxes, combobox, or misc input field with reportable name
							if(examFields[0].FieldType==SheetFieldType.ComboBox && match.Value.Contains(examFields[0].ReportableName)) {
								fieldValue=fieldValue.Replace(match.Value,examFields[0].FieldValue.Substring(0,examFields[0].FieldValue.IndexOf(';')));
							}
							else {
								fieldValue=fieldValue.Replace(match.Value,examFields[0].FieldValue);//checkboxes from exam sheets will show as X or blank on letter.
							}
						}
						else if(examFields[0].FieldName!="" && examFields[0].FieldName!="misc") {//internally defined
							if(examFields[0].RadioButtonValue=="") {//internally defined field, not part of a radio button group
								fieldValue=fieldValue.Replace(match.Value,examFields[0].FieldValue);//checkbox or input
							}
							else {//internally defined radio button, look for one selected
								for(int i=0;i<examFields.Count;i++) {
									if(examFields[i].FieldValue=="X") {
										fieldValue=fieldValue.Replace(match.Value,examFields[i].RadioButtonValue);
										break;
									}
								}
							}
						}
						match=match.NextMatch();
					}//while
					sheet.SheetFields[f].FieldValue=fieldValue;
				}
			}
			#endregion
			object[] parameters={patient,sheet,listStaticTextReplacements};
			Plugins.HookAddCode(null,"SheetFiller.ReplaceStaticTextFieldsInSheet_End",parameters);
		}

		private static string StripPhoneBeyondSpace(string phone) {
			if(!phone.Contains(" ")) {
				return phone;
			}
			int idx=phone.IndexOf(" ");
			return phone.Substring(0,idx);
		}

		///<summary>For new sheets only. Sets sheetField.FieldValue=DocumentNum(FK) or "MountNum:###" based on documentCategoryNum (stored in SheetField.FieldName) and pat.PatNum. Example: if DocCategory=132 (PatImages) then FieldValue would be set equal to the DocNum or MountNum of the most recent PatImage in the patient's image folder.  If there are no images in the patient's folder, FieldValue will be blank.</summary>
		private static void FillPatientImages(Sheet sheet,Patient patient,StaticTextData staticTextData=null){
			if(patient is null){
				return;
			}
			List<Document> listDocuments=new List<Document>();
			if(staticTextData!=null && staticTextData.ListDocuments!=null){
				listDocuments=staticTextData.ListDocuments;
			}
			else{
				listDocuments=Documents.GetPatientData(patient.PatNum);
			}
			List<Mount> listMounts=Mounts.GetPatientData(patient.PatNum);
			for(int i=0;i<sheet.SheetFields.Count;i++){
				if(sheet.SheetFields[i].FieldType!=SheetFieldType.PatImage){
					continue;
				}
				sheet.SheetFields[i].FieldValue="";
				long categoryNum=PIn.Long(sheet.SheetFields[i].FieldName);
				Document document=listDocuments.FindAll(x=>x.DocCategory==categoryNum && x.MountItemNum==0).LastOrDefault();
				Mount mount=listMounts.FindAll(x=>x.DocCategory==categoryNum).LastOrDefault();
				if(document!=null && mount!=null){
					if(document.DateCreated>mount.DateCreated){
						sheet.SheetFields[i].FieldValue=document.DocNum.ToString();
					}
					else{
						sheet.SheetFields[i].FieldValue="MountNum:"+mount.MountNum.ToString();
					}
				}
				else if(document!=null){
					sheet.SheetFields[i].FieldValue=document.DocNum.ToString();
				}
				else if(mount!=null){
					sheet.SheetFields[i].FieldValue="MountNum:"+mount.MountNum.ToString();
				}
			}
		}

		private static void FillFieldsForLabelPatient(Sheet sheet,Patient pat){
			foreach(SheetField field in sheet.SheetFields){
				switch(field.FieldName){
					case "nameFL":
						field.FieldValue=pat.GetNameFLFormal();
						break;
					case "nameLF":
						field.FieldValue=pat.GetNameLF();
						break;
					case "address":
						field.FieldValue=pat.Address;
						if(pat.Address2!=""){
							field.FieldValue+="\r\n"+pat.Address2;
						}
						break;
					case "cityStateZip":
						field.FieldValue=pat.City+", "+pat.State+" "+pat.Zip;
						break;
					case "ChartNumber":
						field.FieldValue=pat.ChartNumber;
						break;
					case "PatNum":
						field.FieldValue=pat.PatNum.ToString();
						break;
					case "dateTime.Today":
						field.FieldValue=DateTime.Today.ToShortDateString();
						break;
					case "birthdate":
						//only a temporary workaround:
						field.FieldValue="BD: "+pat.Birthdate.ToShortDateString();
						break;
					case "priProvName":
						field.FieldValue=Providers.GetLongDesc(pat.PriProv);
						break;
					case "text":
						//If the user sets a Label Text as their patient label, then the "text" param will be null.
						//We will handle this case by manually setting the field value to name and address here.
						SheetParameter paramCur=GetParamByName(sheet,"text");
						if(paramCur==null) {
							field.FieldValue=pat.FName+" "+pat.LName+"\r\n"+pat.Address+"\r\n"+pat.City+", "+pat.State+" "+pat.Zip+"\r\n";
						}
						else {
							field.FieldValue=paramCur.ParamValue.ToString();
						}
						break;
				}
			}

			
		}

		private static void FillFieldsForLabelCarrier(Sheet sheet,Carrier carrier) {
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "CarrierName":
						field.FieldValue=carrier.CarrierName;
						break;
					case "address":
						field.FieldValue=carrier.Address;
						if(carrier.Address2!="") {
							field.FieldValue+="\r\n"+carrier.Address2;
						}
						break;
					case "cityStateZip":
						field.FieldValue=carrier.City+", "+carrier.State+" "+carrier.Zip;
						break;
				}
			}
		}

		private static void FillFieldsForLabelReferral(Sheet sheet,Referral refer) {
			if(refer==null) {
				return;
			}
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "nameFL":
						field.FieldValue=Referrals.GetNameFL(refer.ReferralNum);
						break;
					case "address":
						field.FieldValue=refer.Address;
						if(refer.Address2!="") {
							field.FieldValue+="\r\n"+refer.Address2;
						}
						break;
					case "cityStateZip":
						field.FieldValue=refer.City+", "+refer.ST+" "+refer.Zip;
						break;
				}
			}
		}

		private static void FillFieldsForReferralSlip(Sheet sheet,Patient pat,Referral refer,long refAttachProvNum){
			List<SheetField> listPatientFields=new List<SheetField>();
			List<SheetField> listReferralFields=new List<SheetField>();
			#region misc fields
			foreach(SheetField field in sheet.SheetFields) {
				if(field.FieldName.StartsWith("patient.")) {
					listPatientFields.Add(field);
					continue;
				}
				if(field.FieldName.StartsWith("referral.")) {
					listReferralFields.Add(field);
					continue;
				}
				//All other fields are considered misc so put them through the switch case.
				switch(field.FieldName) {
					case "dateTime.Today":
						field.FieldValue=DateTime.Today.ToShortDateString();
						break;
				}
			}
			#endregion
			#region referral fields
			if(refer!=null) {
				foreach(SheetField field in listReferralFields) {
					switch(field.FieldName) {
						case "referral.nameFL":
							field.FieldValue=Referrals.GetNameFL(refer.ReferralNum);
							break;
						case "referral.address":
							field.FieldValue=refer.Address;
							if(refer.Address2!="") {
								field.FieldValue+="\r\n"+refer.Address2;
							}
							break;
						case "referral.cityStateZip":
							field.FieldValue=refer.City+", "+refer.ST+" "+refer.Zip;
							break;
						case "referral.phone":
							field.FieldValue="";
							if(refer.Telephone.Length==10){
								field.FieldValue=TelephoneNumbers.ReFormat(refer.Telephone);
							}
							break;
						case "referral.phone2":
							field.FieldValue=refer.Phone2;
							break;
					}
				}
			}
			#endregion
			#region patient fields
			if(pat!=null) {
				foreach(SheetField field in listPatientFields) {
					switch(field.FieldName) {
						case "patient.nameFL":
							field.FieldValue=pat.GetNameFL();
							break;
						case "patient.WkPhone":
							field.FieldValue=pat.WkPhone;
							break;
						case "patient.HmPhone":
							field.FieldValue=pat.HmPhone;
							break;
						case "patient.WirelessPhone":
							field.FieldValue=pat.WirelessPhone;
							break;
						case "patient.address":
							field.FieldValue=pat.Address;
							if(pat.Address2!="") {
								field.FieldValue+="\r\n"+pat.Address2;
							}
							break;
						case "patient.cityStateZip":
							field.FieldValue=pat.City+", "+pat.State+" "+pat.Zip;
							break;
						case "patient.provider":
							field.FieldValue=Providers.GetProv(Patients.GetProvNum(pat)).GetFormalName();//Use the patient's default provider.
							if(refAttachProvNum!=0){
								field.FieldValue=Providers.GetProv(refAttachProvNum).GetFormalName();//Referral type is "To", use selected referral's associated provider instead.
							}
							break;
						//case "notes"://an input field
					}
				}
			}
			#endregion
		}

		private static void FillFieldsForLabelAppointment(Sheet sheet,Appointment appt,Patient pat) {
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "nameFL":
						field.FieldValue=pat.GetNameFirstOrPrefL();
						break;
					case "nameLF":
						field.FieldValue=pat.GetNameLF();
						break;
					case "weekdayDateTime":
						field.FieldValue=appt.AptDateTime.ToString("ddd")+"   "
							+appt.AptDateTime.ToShortDateString()+"  "
							+appt.AptDateTime.ToShortTimeString();//  h:mm tt");
						break;
					case "length":
						int minutesTotal=appt.Pattern.Length*5;
						int hours=minutesTotal/60;//automatically rounds down
						int minutes=minutesTotal-hours*60;
						field.FieldValue="";
						if(hours>0){
							field.FieldValue=hours.ToString()+" hours, ";
						}
						field.FieldValue+=minutes.ToString()+" min";
						break;
				}
			}
		}

		private static void FillFieldsForRx(Sheet sheet,RxPat rx,Patient pat,Provider prov) {
			Clinic clinic=null;
			if(rx.ClinicNum!=0) {
				clinic=Clinics.GetClinic(rx.ClinicNum);
			}
			ProviderClinic provClinic=ProviderClinics.GetOneOrDefault(prov.ProvNum,(clinic==null ? 0 : clinic.ClinicNum));
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "prov.nameFL":
						field.FieldValue=prov.GetFormalName();
						break;
					case "clinic.address":
						field.FieldValue=AddressHelper(clinic);
						break;
					case "clinic.cityStateZip":
						field.FieldValue=CityStateHelper(clinic);
						break;
					case "clinic.phone":
						field.FieldValue=PhoneHelper(clinic);
						break;
					case "RxDate":
						field.FieldValue=rx.RxDate.ToShortDateString();
						break;
					case "RxDateMonthSpelled":
						field.FieldValue=rx.RxDate.ToString("MMM dd,yyyy");
						break;
					case "prov.dEANum":
						if(rx.IsControlled){
							field.FieldValue=ProviderClinics.GetDEANum(prov.ProvNum,(clinic==null ? 0 : clinic.ClinicNum));
						}
						else{
							field.FieldValue="";
						}
						break;
					case "pat.nameFL":
						//Can't include preferred, so:
						field.FieldValue=pat.FName+" "+pat.MiddleI+"  "+pat.LName;
						break;
					case "pat.Birthdate":
						if(pat.Birthdate.Year<1880){
							field.FieldValue="";
						}
						else{
							field.FieldValue=pat.Birthdate.ToShortDateString();
						}
						break;
					case "pat.HmPhone":
						field.FieldValue=pat.HmPhone;
						break;
					case "pat.address":
						field.FieldValue=pat.Address;
						if(pat.Address2!=""){
							field.FieldValue+="\r\n"+pat.Address2;
						}
						break;
					case "pat.cityStateZip":
						field.FieldValue=pat.City+", "+pat.State+" "+pat.Zip;
						break;
					case "Drug":
						field.FieldValue=rx.Drug;
						break;
					case "Disp":
						field.FieldValue=rx.Disp;
						break;
					case "Sig":
						field.FieldValue=rx.Sig;
						break;
					case "Refills":
						field.FieldValue=rx.Refills;
						break;	
					case "prov.stateRxID":
						field.FieldValue=(provClinic==null ? "" : provClinic.StateRxID);
						break;
					case "prov.StateLicense":
						field.FieldValue=(provClinic==null ? "" : provClinic.StateLicense);
						break;
					case "prov.NationalProvID":
						field.FieldValue=prov.NationalProvID;
						break;
					case "ProcCode":
						field.FieldValue=GetRxFieldProcCode(rx);
						break;
					case "DaysOfSupply":
						field.FieldValue=GetRxFieldDaysOfSupply(rx);
						break;
				}
			}
		}

		private static string GetRxFieldProcCode(RxPat rx) {
			if(rx.ProcNum==0) {
				return "";
			}
			Procedure proc=Procedures.GetOneProc(rx.ProcNum,false);
			ProcedureCode procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
			string retVal=Lans.g("Rx","Procedure Code:")+" "+procCode.ProcCode;
			if(proc.DiagnosticCode!="") {
				retVal+="  "+Lans.g("Rx","Diagnosis:")+" "+proc.DiagnosticCode;
			}
			return retVal;
		}

		private static string GetRxFieldDaysOfSupply(RxPat rx) {
			if(rx.DaysOfSupply<=0) {
				return "";
			}
			return Lans.g("Rx","Total Supply:")+" "+rx.DaysOfSupply+" "+((rx.DaysOfSupply > 1)?Lans.g("Rx","Days"):Lans.g("Rx","Day"));
		}

		private static void FillFieldsForRxInstruction(Sheet sheet) {
			long rxNum=(long)GetParamByName(sheet,"RxNum").ParamValue;
			RxPat rx=RxPats.GetRx(rxNum);
			Provider prov=Providers.GetProv(rx.ProvNum);
			Patient pat=Patients.GetPat(rx.PatNum);
			Clinic clinic=Clinics.GetClinic(pat.ClinicNum);
			ProviderClinic provClinic=ProviderClinics.GetOneOrDefault(prov.ProvNum,(clinic==null ? 0 : clinic.ClinicNum));
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case SheetFieldsAvailable.RxPat.Drug:
						field.FieldValue=rx.Drug;
						break;
					case SheetFieldsAvailable.RxPat.Sig:
						field.FieldValue=rx.Sig;
						break;
					case SheetFieldsAvailable.RxPat.PatientInstruction:
						field.FieldValue=rx.PatientInstruction;
						break;
					case SheetFieldsAvailable.RxPat.Disp:
						field.FieldValue=rx.Disp;
						break;
					case SheetFieldsAvailable.RxPat.Refills:
						field.FieldValue=rx.Refills;
						break;
					case SheetFieldsAvailable.Rx.DaysOfSupply:
						field.FieldValue=rx.DaysOfSupply.ToString();
						break;
					case SheetFieldsAvailable.Rx.RxDate:
						field.FieldValue=rx.RxDate.ToShortDateString();
						break;
					case SheetFieldsAvailable.Rx.RxDateMonthSpelled:
						field.FieldValue=rx.RxDate.ToString("MMM dd,yyyy");
						break;
					case SheetFieldsAvailable.Today.DayDate:
						field.FieldValue=DateTime.Today.ToString("dddd")+", "+DateTime.Today.ToShortDateString();
						break;
					case SheetFieldsAvailable.Patient.NameFL:
						field.FieldValue=pat.GetNameFL();
						break;
					case SheetFieldsAvailable.Patient.Salutation:
						field.FieldValue="Dear "+pat.GetSalutation()+":";
						break;
					case SheetFieldsAvailable.Patient.Address:
						field.FieldValue=pat.Address;
						if(pat.Address2!="") {
							field.FieldValue+="\r\n"+pat.Address2;
						}
						break;
					case SheetFieldsAvailable.Patient.CityStateZip:
						field.FieldValue=field.FieldValue=pat.City+", "+pat.State+" "+pat.Zip;
						break;
					case SheetFieldsAvailable.Patient.HmPhone:
						field.FieldValue=pat.HmPhone;
						break;
					case SheetFieldsAvailable.Patient.Birthdate:
						field.FieldValue=pat.Birthdate.ToShortDateString();
						break;
					case SheetFieldsAvailable.Patient.PriProvNameFL:
						field.FieldValue=Providers.GetFormalName(pat.PriProv);
						break;
					case SheetFieldsAvailable.Prov.NameFL:
						field.FieldValue=prov.FName+" "+prov.LName;
						break;
					case SheetFieldsAvailable.Prov.DEANum:
						field.FieldValue=prov.DEANum;
						break;
					case SheetFieldsAvailable.Prov.NationalProvID:
						field.FieldValue=prov.NationalProvID;
						break;
					case SheetFieldsAvailable.Prov.StateRxID:
						field.FieldValue=(provClinic==null ? "" : provClinic.StateRxID);
						break;
					case SheetFieldsAvailable.Prov.StateLicense:
						field.FieldValue=(provClinic==null ? "" : provClinic.StateLicense);
						break;
					case SheetFieldsAvailable.Clinic.Address:
						if(clinic!=null) {
							field.FieldValue=AddressHelper(clinic);
						}
						break;
					case SheetFieldsAvailable.Clinic.CityStateZip:
						if(clinic!=null) {
							field.FieldValue=CityStateHelper(clinic);
						}
						break;
					case SheetFieldsAvailable.Clinic.Phone:
						if(clinic!=null) {
							field.FieldValue=PhoneHelper(clinic);
						}
						break;
					case SheetFieldsAvailable.Practice.Title:
						field.FieldValue=PrefC.GetString(PrefName.PracticeTitle);
						break;
					case SheetFieldsAvailable.Practice.Address:
						field.FieldValue=PrefC.GetString(PrefName.PracticeAddress);
						if(PrefC.GetString(PrefName.PracticeAddress2) != ""){
							field.FieldValue+="\r\n"+PrefC.GetString(PrefName.PracticeAddress2);
						}
						break;
					case SheetFieldsAvailable.Practice.CityStateZip:
						field.FieldValue=PrefC.GetString(PrefName.PracticeCity)+", "
							+PrefC.GetString(PrefName.PracticeST)+"  "
							+PrefC.GetString(PrefName.PracticeZip);
						break;
					case SheetFieldsAvailable.Practice.Phone:
						field.FieldValue=PrefC.GetString(PrefName.PracticePhone);
						break;
					default:
						break;
				}
			}
		}

		private static void FillFieldsForConsent(Sheet sheet,Patient pat) {
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "patient.nameFL":
						field.FieldValue=pat.GetNameFL();
						break;
					case "dateTime.Today":
						field.FieldValue=DateTime.Today.ToShortDateString();
						break;
				}
			}
		}

		private static void FillFieldsForPatientLetter(Sheet sheet,Patient pat) {
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "PracticeTitle":
						field.FieldValue=PrefC.GetString(PrefName.PracticeTitle);
						break;
					case "PracticeAddress":
						field.FieldValue=PrefC.GetString(PrefName.PracticeAddress);
						if(PrefC.GetString(PrefName.PracticeAddress2) != ""){
							field.FieldValue+="\r\n"+PrefC.GetString(PrefName.PracticeAddress2);
						}
						break;
					case "practiceCityStateZip":
						field.FieldValue=PrefC.GetString(PrefName.PracticeCity)+", "
							+PrefC.GetString(PrefName.PracticeST)+"  "
							+PrefC.GetString(PrefName.PracticeZip);
						break;
					case "patient.nameFL":
						field.FieldValue=pat.GetNameFLFormal();
						break;
					case "patient.address":
						field.FieldValue=pat.Address;
						if(pat.Address2!="") {
							field.FieldValue+="\r\n"+pat.Address2;
						}
						break;
					case "patient.cityStateZip":
						field.FieldValue=pat.City+", "+pat.State+" "+pat.Zip;
						break;
					case "today.DayDate":
						field.FieldValue=DateTime.Today.ToString("dddd")+", "+DateTime.Today.ToShortDateString();
						break;
					case "patient.salutation":
						field.FieldValue="Dear "+pat.GetSalutation()+":";
						break;
					case "patient.priProvNameFL":
						field.FieldValue=Providers.GetFormalName(pat.PriProv);
						break;
				}
			}
		}

		///<summary>Fills sheet with CEMT patient transfer fields. This sheet is only filled from the CEMT tool.</summary>
		public static void FillFieldsForPatientTransferCEMT(Sheet sheet,Patient pat) {
			Family fam=Patients.GetFamily(pat.PatNum);
			List<PatPlan> patPlanList=PatPlans.Refresh(pat.PatNum);//Ordered by Ordinal
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			InsPlan insplan1=null;
			InsSub sub1=null;
			Carrier carrier1=null;
			if(patPlanList.Count>0) {
				sub1=InsSubs.GetSub(patPlanList[0].InsSubNum,subList);
				insplan1=InsPlans.GetPlan(sub1.PlanNum,planList);
				carrier1=Carriers.GetCarrierDB(insplan1.CarrierNum);
			}
			InsPlan insplan2=null;
			InsSub sub2=null;
			Carrier carrier2=null;
			if(patPlanList.Count>1) {
				sub2=InsSubs.GetSub(patPlanList[1].InsSubNum,subList);
				insplan2=InsPlans.GetPlan(sub2.PlanNum,planList);
				carrier2=Carriers.GetCarrierDB(insplan2.CarrierNum);
			}
			PatientNote patCurNote=PatientNotes.Refresh(pat.PatNum,pat.Guarantor);
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "Address":
						field.FieldValue=pat.Address;
						break;
					case "Address2":
						field.FieldValue=pat.Address2;
						break;
					case "Birthdate":
						field.FieldValue=pat.Birthdate.ToShortDateString();
						break;
					case "City":
						field.FieldValue=pat.City;
						break;
					case "Email":
						field.FieldValue=pat.Email;
						break;
					case "FName":
						field.FieldValue=pat.FName;
						break;
					case "Gender":
						SetGenderForCemtSheet(pat.Gender,field);
						break;
					case "HmPhone":
						field.FieldValue=pat.HmPhone;
						break;
					case "ICEName":
						field.FieldValue=patCurNote.ICEName;
						break;
					case "ICEPhone":
						field.FieldValue=patCurNote.ICEPhone;
						break;
					case "ins1CarrierName":
						if(carrier1!=null) {
							field.FieldValue=carrier1.CarrierName;
						}
						break;
					case "ins1CarrierPhone":
						if(carrier1!=null) {
							field.FieldValue=carrier1.Phone;
						}
						break;
					case "ins1EmployerName":
						if(insplan1!=null) {
							field.FieldValue=Employers.GetEmployerNoCache(insplan1.EmployerNum)?.EmpName ?? "";
						}
						break;
					case "ins1GroupName":
						if(insplan1!=null) {
							field.FieldValue=insplan1.GroupName;
						}
						break;
					case "ins1GroupNum":
						if(insplan1!=null) {
							field.FieldValue=insplan1.GroupNum;
						}
						break;
					case "ins1Relat":
						if(patPlanList.Count>0) {
							field.FieldValue=patPlanList[0].Relationship.ToString();
						}
						break;
					case "ins1SubscriberID":
						if(insplan1!=null) {
							field.FieldValue=sub1.SubscriberID;
						}
						break;
					case "ins1SubscriberNameF":
						if(insplan1!=null) {
							field.FieldValue=fam.GetNameInFamFLnoPref(sub1.Subscriber);
						}
						break;
					case "ins2CarrierName":
						if(carrier2!=null) {
							field.FieldValue=carrier2.CarrierName;
						}
						break;
					case "ins2CarrierPhone":
						if(carrier2!=null) {
							field.FieldValue=carrier2.Phone;
						}
						break;
					case "ins2EmployerName":
						if(insplan2!=null) {
							field.FieldValue=Employers.GetEmployerNoCache(insplan2.EmployerNum)?.EmpName ?? "";
						}
						break;
					case "ins2GroupName":
						if(insplan2!=null) {
							field.FieldValue=insplan2.GroupName;
						}
						break;
					case "ins2GroupNum":
						if(insplan2!=null) {
							field.FieldValue=insplan2.GroupNum;
						}
						break;
					case "ins2Relat":
						if(patPlanList.Count>1) {
							field.FieldValue=patPlanList[1].Relationship.ToString();
						}
						break;
					case "ins2SubscriberID":
						if(insplan2!=null) {
							field.FieldValue=sub2.SubscriberID;
						}
						break;
					case "ins2SubscriberNameF":
						if(insplan2!=null) {
							field.FieldValue=fam.GetNameInFamFLnoPref(sub2.Subscriber);
						}
						break;
					case "isTransfer":
						field.FieldValue="true";
						break;
					case "LName":
						field.FieldValue=pat.LName;
						break;
					case "MiddleI":
						field.FieldValue=pat.MiddleI;
						break;
					case "PreferConfirmMethod":
						field.FieldValue=pat.PreferConfirmMethod.ToString();
						break;
					case "PreferContactMethod":
						field.FieldValue=pat.PreferContactMethod.ToString();
						break;
					case "PreferRecallMethod":
						field.FieldValue=pat.PreferRecallMethod.ToString();
						break;
					case "Preferred":
						field.FieldValue=pat.Preferred;
						break;
					case "referredFrom":
						Referral referral=Referrals.GetReferralForPat(pat.PatNum);
						if(referral!=null) {
							field.FieldValue=Referrals.GetNameFL(referral.ReferralNum);
						}
						break;
					case "sendClinicCEMT":
						string strClinic=PrefC.GetStringNoCache(PrefName.PracticeTitle);
						if(Prefs.HasClinicsEnabledNoCache){
							Clinic clinic=Clinics.GetClinicNoCache(pat.ClinicNum);
							if(clinic!=null) {
								strClinic=clinic.Abbr;
							}
						}
						field.FieldValue=strClinic;
						break;
					case "SSN":
						if(CultureInfo.CurrentCulture.Name=="en-US" && pat.SSN.Length==9) {//and length exactly 9 (no data gets lost in formatting)
							field.FieldValue=pat.SSN.Substring(0,3)+"-"+pat.SSN.Substring(3,2)+"-"+pat.SSN.Substring(5,4);
						}
						else {
							field.FieldValue=pat.SSN;
						}
						break;
					case "State":
						field.FieldValue=pat.State;
						break;
					case "WirelessPhone":
						field.FieldValue=pat.WirelessPhone;
						break;
					case "WkPhone":
						field.FieldValue=pat.WkPhone;
						break;
					case "Zip":
						field.FieldValue=pat.Zip;
						break;
				}
			}
		}

		private static void SetGenderForCemtSheet(PatientGender gender,SheetField field) {
			if(gender==PatientGender.Male) {
				field.RadioButtonValue="Male";
				field.FieldValue="X";
			}
			else if(gender==PatientGender.Female) {
				field.RadioButtonValue="Female";
				field.FieldValue="X";
			}
		}
		
		private static void FillFieldsForReferralLetter(Sheet sheet,Patient pat,Referral refer) {
			List<SheetField> listPatientFields=new List<SheetField>();
			List<SheetField> listReferralFields=new List<SheetField>();
			#region misc fields
			foreach(SheetField field in sheet.SheetFields) {
				if(field.FieldName.StartsWith("patient.")) {
					listPatientFields.Add(field);
					continue;
				}
				if(field.FieldName.StartsWith("referral.")) {
					listReferralFields.Add(field);
					continue;
				}
				switch(field.FieldName) {
					case "PracticeTitle":
						field.FieldValue=PrefC.GetString(PrefName.PracticeTitle);
						break;
					case "PracticeAddress":
						field.FieldValue=PrefC.GetString(PrefName.PracticeAddress);
						if(PrefC.GetString(PrefName.PracticeAddress2) != ""){
							field.FieldValue+="\r\n"+PrefC.GetString(PrefName.PracticeAddress2);
						}
						break;
					case "PracticePhoneNumber":
						string practicePhone=PrefC.GetString(PrefName.PracticePhone);
						field.FieldValue=practicePhone;
						if(practicePhone.Length==10) {
							field.FieldValue=TelephoneNumbers.ReFormat(practicePhone);
						}
						break;
					case "practiceCityStateZip":
						field.FieldValue=PrefC.GetString(PrefName.PracticeCity)+", "
							+PrefC.GetString(PrefName.PracticeST)+"  "
							+PrefC.GetString(PrefName.PracticeZip);
						break;
					case "today.DayDate":
						field.FieldValue=DateTime.Today.ToString("dddd")+", "+DateTime.Today.ToShortDateString();
						break;
				}
			}
			#endregion
			#region referral fields
			if(refer!=null) {
				foreach(SheetField field in listReferralFields) {
					switch(field.FieldName) {
						case "referral.phone":
							field.FieldValue="";
							if(refer.Telephone.Length==10) {
								field.FieldValue=TelephoneNumbers.ReFormat(refer.Telephone);
							}
							break;
						case "referral.phone2":
							field.FieldValue=refer.Phone2;
							break;
						case "referral.nameFL":
							field.FieldValue=Referrals.GetNameFL(refer.ReferralNum);
							break;
						case "referral.nameL":
							field.FieldValue=refer.LName;
							break;
						case "referral.address":
							field.FieldValue=refer.Address;
							if(refer.Address2!="") {
								field.FieldValue+="\r\n"+refer.Address2;
							}
							break;
						case "referral.cityStateZip":
							field.FieldValue=refer.City+", "+refer.ST+" "+refer.Zip;
							break;
						case "referral.salutation":
							field.FieldValue="Dear "+refer.FName+":";
							break;
					}
				}
			}
			#endregion
			#region patient fields
			if(pat!=null) {
				foreach(SheetField field in listPatientFields) {
					switch(field.FieldName) {
						case "patient.nameFL":
							field.FieldValue=pat.GetNameFL();
							break;
						case "patient.Birthdate":
							field.FieldValue=pat.Birthdate.ToShortDateString();
							break;
						case "patient.priProvNameFL":
							field.FieldValue=Providers.GetFormalName(pat.PriProv);
							break;
					}
				}
			}
			#endregion
		}

		private static void FillFieldsForPatientForm(Sheet sheet,Patient pat) {
			Family fam=Patients.GetFamily(pat.PatNum);
			List<PatPlan> patPlanList=PatPlans.Refresh(pat.PatNum);
			if(!PatPlans.IsPatPlanListValid(patPlanList)) {
				patPlanList=PatPlans.Refresh(pat.PatNum);
			}
			List<InsSub> subList=InsSubs.RefreshForFam(fam);
			List<InsPlan> planList=InsPlans.RefreshForSubList(subList);
			InsPlan insplan1=null;
			InsSub sub1=null;
			Carrier carrier1=null;
			if(patPlanList.Count>0){
				sub1=InsSubs.GetSub(patPlanList[0].InsSubNum,subList);
				insplan1=InsPlans.GetPlan(sub1.PlanNum,planList);
				carrier1=Carriers.GetCarrier(insplan1.CarrierNum);
			}
			InsPlan insplan2=null;
			InsSub sub2=null;
			Carrier carrier2=null;
			if(patPlanList.Count>1) {
				sub2=InsSubs.GetSub(patPlanList[1].InsSubNum,subList);
				insplan2=InsPlans.GetPlan(sub2.PlanNum,planList);
				carrier2=Carriers.GetCarrier(insplan2.CarrierNum);
			}
			PatientNote patCurNote=PatientNotes.Refresh(pat.PatNum,pat.Guarantor);
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "Address":
						field.FieldValue=pat.Address;
						break;
					case "Address2":
						field.FieldValue=pat.Address2;
						break;
					case "addressAndHmPhoneIsSameEntireFamily":
						bool isSame=true;
						for(int i=0;i<fam.ListPats.Length;i++){
							if(pat.HmPhone!=fam.ListPats[i].HmPhone
								|| pat.Address!=fam.ListPats[i].Address
								|| pat.Address2!=fam.ListPats[i].Address2
								|| pat.City!=fam.ListPats[i].City
								|| pat.State!=fam.ListPats[i].State
								|| pat.Zip!=fam.ListPats[i].Zip)
							{
								isSame=false;
								break;
							}
						}
						if(isSame) {
							field.FieldValue="X";
						}
						break;
					case "Birthdate":
						field.FieldValue=pat.Birthdate.ToShortDateString();
						break;
					case "City":
						field.FieldValue=pat.City;
						break;
					case "Email":
						field.FieldValue=pat.Email;
						break;
					case "FName":
						field.FieldValue=pat.FName;
						break;
					case "Gender":
						if(field.RadioButtonValue==pat.Gender.ToString()) {
							field.FieldValue="X";
						}
						break;
					case "HmPhone":
						field.FieldValue=pat.HmPhone;
						break;
					case "ICEName":
						field.FieldValue=patCurNote.ICEName;
						break;
					case "ICEPhone":
						field.FieldValue=patCurNote.ICEPhone;
						break;
					case "ins1CarrierName":
						if(carrier1!=null){
							field.FieldValue=carrier1.CarrierName;
						}
						break;
					case "ins1CarrierPhone":
						if(carrier1!=null) {
							field.FieldValue=carrier1.Phone;
						}
						break;
					case "ins1EmployerName":
						if(insplan1!=null) {
							field.FieldValue=Employers.GetName(insplan1.EmployerNum);
						}
						break;
					case "ins1GroupName":
						if(insplan1!=null) {
							field.FieldValue=insplan1.GroupName;
						}
						break;
					case "ins1GroupNum":
						if(insplan1!=null) {
							field.FieldValue=insplan1.GroupNum;
						}
						break;
					case "ins1Relat":
						if(patPlanList.Count>0 && patPlanList[0].Relationship.ToString()==field.RadioButtonValue) {
							field.FieldValue="X";
						}
						break;
					case "ins1SubscriberID":
						if(insplan1!=null) {
							field.FieldValue=sub1.SubscriberID;
						}
						break;
					case "ins1SubscriberNameF":
						if(insplan1!=null) {
							field.FieldValue=fam.GetNameInFamFirst(sub1.Subscriber);
						}
						break;
					case "ins2CarrierName":
						if(carrier2!=null) {
							field.FieldValue=carrier2.CarrierName;
						}
						break;
					case "ins2CarrierPhone":
						if(carrier2!=null) {
							field.FieldValue=carrier2.Phone;
						}
						break;
					case "ins2EmployerName":
						if(insplan2!=null) {
							field.FieldValue=Employers.GetName(insplan2.EmployerNum);
						}
						break;
					case "ins2GroupName":
						if(insplan2!=null) {
							field.FieldValue=insplan2.GroupName;
						}
						break;
					case "ins2GroupNum":
						if(insplan2!=null) {
							field.FieldValue=insplan2.GroupNum;
						}
						break;
					case "ins2Relat":
						if(patPlanList.Count>1 && patPlanList[1].Relationship.ToString()==field.RadioButtonValue) {
							field.FieldValue="X";
						}
						break;
					case "ins2SubscriberID":
						if(insplan2!=null) {
							field.FieldValue=sub2.SubscriberID;
						}
						break;
					case "ins2SubscriberNameF":
						if(insplan2!=null) {
							field.FieldValue=fam.GetNameInFamFirst(sub2.Subscriber);
						}
						break;
					case "LName":
						field.FieldValue=pat.LName;
						break;
					case "MiddleI":
						field.FieldValue=pat.MiddleI;
						break;
					case "Position":
						if(pat.Position.ToString()==field.RadioButtonValue) {
							field.FieldValue="X";
						}
						break;
					case "PreferConfirmMethod":
						if(pat.PreferConfirmMethod.ToString()==field.RadioButtonValue) {
							field.FieldValue="X";
						}
						break;
					case "PreferContactMethod":
						if(pat.PreferContactMethod.ToString()==field.RadioButtonValue) {
							field.FieldValue="X";
						}
						break;
					case "PreferRecallMethod":
						if(pat.PreferRecallMethod.ToString()==field.RadioButtonValue) {
							field.FieldValue="X";
						}
						break;
					case "Preferred":
						field.FieldValue=pat.Preferred;
						break;
					case "referredFrom":
						Referral referral=Referrals.GetReferralForPat(pat.PatNum);
						if(referral!=null){
							field.FieldValue=Referrals.GetNameFL(referral.ReferralNum);
						}
						break;
					case "SSN":
						if(CultureInfo.CurrentCulture.Name=="en-US" && pat.SSN.Length==9){//and length exactly 9 (no data gets lost in formatting)
							field.FieldValue=pat.SSN.Substring(0,3)+"-"+pat.SSN.Substring(3,2)+"-"+pat.SSN.Substring(5,4);
						}
						else {
							field.FieldValue=pat.SSN;
						}
						break;
					case "State":
						field.FieldValue=pat.State;
						break;
					case "StudentStatus":
						if(pat.StudentStatus=="F" && field.RadioButtonValue=="Fulltime") {
							field.FieldValue="X";
						}
						if(pat.StudentStatus=="N" && field.RadioButtonValue=="Nonstudent") {
							field.FieldValue="X";
						}
						if(pat.StudentStatus=="P" && field.RadioButtonValue=="Parttime") {
							field.FieldValue="X";
						}

						break;
					case "WirelessPhone":
						field.FieldValue=pat.WirelessPhone;
						break;
					case "wirelessCarrier":
						field.FieldValue="";//not implemented
						break;
					case "WkPhone":
						field.FieldValue=pat.WkPhone;
						break;
					case "Zip":
						field.FieldValue=pat.Zip;
						break;
				}
			}
		}

		private static void FillFieldsForRoutingSlip(Sheet sheet,Patient pat,Appointment apt) {
			Family fam=Patients.GetFamily(apt.PatNum);
			LabCase labForApt=LabCases.GetForApt(apt.AptNum).FirstOrDefault();
			Referral referral=Referrals.GetReferralForPat(apt.PatNum);
			string str;
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "appt.timeDate":
						field.FieldValue=apt.AptDateTime.ToShortTimeString()+"  "+apt.AptDateTime.ToShortDateString();
						break;
					case "appt.length":
						field.FieldValue=(apt.Pattern.Length*5).ToString()+" "+Lans.g("SheetRoutingSlip","minutes");
						break;
					case "appt.providers":
						str=Providers.GetLongDesc(apt.ProvNum);
						if(apt.ProvHyg!=0){
							str+="\r\n"+Providers.GetLongDesc(apt.ProvHyg);
						}
						field.FieldValue=str;
						break;
					case "appt.procedures":
						str="";
						List<Procedure> procs=Procedures.GetProcsForSingle(apt.AptNum,false);
						bool isOnlyTP=true;
						for(int i=0;i<procs.Count;i++) {
							if(procs[i].ProcStatus!=ProcStat.TP) {
								isOnlyTP=false;
								break;
							}
						}
						if(isOnlyTP) {
							List<Procedure> listProceduresTPs=Procedures.GetListTPandTPi(procs);//this sorts.  Doesn't work unless all are TP.
							for(int i=0;i<listProceduresTPs.Count;i++) {
								if(i>0) {
									str+="\r\n";
								}
								str+=Procedures.GetDescription(listProceduresTPs[i]);
							}
						}
						else {
							for(int i=0;i<procs.Count;i++) {
								if(i>0) {
									str+="\r\n";
								}
								str+=Procedures.GetDescription(procs[i]);
							}
						}
						field.FieldValue=str;
						break;
					case "appt.Note":
						field.FieldValue=apt.Note;
						break;
					case "appt.estPatientPortion":
						field.FieldValue=Appointments.GetEstPatientPortion(apt).ToString("c");
						break;
					case "otherFamilyMembers":
						str="";
						for(int i=0;i<fam.ListPats.Length;i++) {
							if(fam.ListPats[i].PatNum==pat.PatNum) {
								continue;
							}
							if(fam.ListPats[i].PatStatus==PatientStatus.Archived
								|| fam.ListPats[i].PatStatus==PatientStatus.Deceased) {
								//Prospective patients will show.
								continue;
							}
							if(str!="") {
								str+="\r\n";
							}
							str+=fam.ListPats[i].GetNameFL();
							if(fam.ListPats[i].Age>0){
								str+=",   "+fam.ListPats[i].Age.ToString();
							}
						}
						field.FieldValue=str;
						break;
					case "labName":
						if(labForApt!=null) {
							field.FieldValue=Laboratories.GetOne(labForApt.LaboratoryNum).Description;
						}
						break;
					case "dateLabSent":
						if(labForApt!=null) {
							if(labForApt.DateTimeSent==DateTime.MinValue) {
								field.FieldValue="Not Sent";
							}
							else {
								field.FieldValue=labForApt.DateTimeSent.ToShortDateString();
							}
						}
						break;
					case "dateLabReceived":
						if(labForApt!=null) {
							if(labForApt.DateTimeRecd==DateTime.MinValue) {
								field.FieldValue="Not Received";
							}
							else {
								field.FieldValue=labForApt.DateTimeRecd.ToShortDateString();
							}
						}
						break;
					case "referral.address":
						field.FieldValue="";
						if(referral!=null) {
							field.FieldValue+=referral.Address;
							if(referral.Address2!="") {
								field.FieldValue+="\r\n"+referral.Address2;
							}
						}
						break;
					case "referral.cityStateZip":
						field.FieldValue="";
						if(referral!=null) {
							field.FieldValue+=referral.City;
							field.FieldValue+=(field.FieldValue!="" && referral.ST!="" ? ", " : "");
							field.FieldValue+=referral.ST;
							field.FieldValue+=(field.FieldValue!="" ? " ":"")+referral.Zip;
						}
						break;
					case "referral.FLName":
						field.FieldValue="";
						if(referral!=null) {
							field.FieldValue+=Patients.GetNameFL(referral.LName,referral.FName,"","");
						}
						break;
					case "referral.LName":
						field.FieldValue="";
						if(referral!=null) {
							field.FieldValue=referral.LName;
						}
						break;
				}
			}
		}

		private static void FillFieldsForMedicalHistory(Sheet sheet,Patient pat) {
			List<SheetField> inputMedList=new List<SheetField>();
			PatientNote patCurNote=PatientNotes.Refresh(pat.PatNum,pat.Guarantor);
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "Birthdate":
						field.FieldValue=pat.Birthdate.ToShortDateString();
						continue;
					case "FName":
						field.FieldValue=pat.FName;
						continue;
					case "ICEName":
						field.FieldValue=patCurNote.ICEName;
						continue;
					case "ICEPhone":
						field.FieldValue=patCurNote.ICEPhone;
						continue;
					case "LName":
						field.FieldValue=pat.LName;
						continue;
				}
				if(field.FieldType==SheetFieldType.CheckBox) {
					if(field.FieldName.StartsWith("allergy:")) {//"allergy:Pen"
						List<Allergy> allergies=Allergies.GetAll(pat.PatNum,true);
						for(int i=0;i<allergies.Count;i++) {
							if(AllergyDefs.GetDescription(allergies[i].AllergyDefNum)==field.FieldName.Remove(0,8)) {
								if(allergies[i].StatusIsActive && field.RadioButtonValue=="Y") {
									field.FieldValue="X";
								}
								else if(!allergies[i].StatusIsActive && field.RadioButtonValue=="N") {
									field.FieldValue="X";
								}
								break;
							}
						}
					}
					else if(field.FieldName.StartsWith("problem:")) {//"problem:Hepatitis B"
						List<Disease> diseases=Diseases.Refresh(pat.PatNum,false);
						for(int i=0;i<diseases.Count;i++) {
							if(DiseaseDefs.GetName(diseases[i].DiseaseDefNum)==field.FieldName.Remove(0,8)) {
								if(diseases[i].ProbStatus==ProblemStatus.Active && field.RadioButtonValue=="Y") {
									field.FieldValue="X";
								}
								else if(diseases[i].ProbStatus!=ProblemStatus.Active && field.RadioButtonValue=="N") {
									field.FieldValue="X";
								}
								break;
							}
						}
					}
				}
				else if(field.FieldType==SheetFieldType.InputField && field.FieldName.StartsWith("inputMed")) {
					inputMedList.Add(field);
				}
			}
			//Special logic for checkMed and inputMed.
			if(inputMedList.Count>0) {
				inputMedList.Sort(CompareSheetFieldNames);
				//Loop through the patients medications and fill in the input fields.
				List<MedicationPat> medPatList=MedicationPats.Refresh(pat.PatNum,false);
				for(int i=0;i<medPatList.Count;i++) {
					if(i==inputMedList.Count) {
						break;//Pat has more medications than inputMed fields on sheet.
					}
					if(medPatList[i].MedicationNum==0) {
						inputMedList[i].FieldValue=medPatList[i].MedDescript;
					}
					else {
						inputMedList[i].FieldValue=Medications.GetDescription(medPatList[i].MedicationNum);
					}
					inputMedList[i].FieldType=SheetFieldType.OutputText;//Don't try to import as a new medication.
				}
			}
		}

		private static void FillFieldsForLabCase(Sheet sheet,Patient pat,LabCase labcase) {
			Laboratory lab=Laboratories.GetOne(labcase.LaboratoryNum);//might possibly be null
			Provider prov=Providers.GetProv(labcase.ProvNum);
			Appointment appt=Appointments.GetOneApt(labcase.AptNum);//might be null
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled) {//Only get clinic specific information if clinics are turned on.
				clinicNum=pat.ClinicNum;
			}
			ProviderClinic provClinic=ProviderClinics.GetOneOrDefault(prov.ProvNum,clinicNum);
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "lab.Description":
						if(lab!=null){
							field.FieldValue=lab.Description;
						}
						break;
					case "lab.Phone":
						if(lab!=null){
							field.FieldValue=lab.Phone;
						}
						break;
					case "lab.Notes":
						if(lab!=null){
							field.FieldValue=lab.Notes;
						}
						break;
					case "lab.WirelessPhone":
						if(lab!=null){
							field.FieldValue=lab.WirelessPhone;
						}
						break;
					case "lab.Address":
						if(lab!=null){
							field.FieldValue=lab.Address;
						}
						break;
					case "lab.CityStZip":
						if(lab!=null){
							field.FieldValue=lab.City+", "+lab.State+" "+lab.Zip;
						}
						break;
					case "lab.Email":
						if(lab!=null){
							field.FieldValue=lab.Email;
						}
						break;
					case "appt.DateTime":
						if(appt!=null) {
							field.FieldValue=appt.AptDateTime.ToShortDateString()+"  "+appt.AptDateTime.ToShortTimeString();
						}
						break;
					case "labcase.DateTimeDue":
						field.FieldValue=labcase.DateTimeDue.ToShortDateString()+"  "+labcase.DateTimeDue.ToShortTimeString();
						break;
					case "labcase.DateTimeCreated":
						field.FieldValue=labcase.DateTimeCreated.ToShortDateString()+"  "+labcase.DateTimeCreated.ToShortTimeString();
						break;
					case "labcase.Instructions":
						field.FieldValue=labcase.Instructions;
						break;
					case "prov.nameFormal":
						field.FieldValue=prov.GetFormalName();
						break;
					case "prov.stateLicence":
						field.FieldValue=(provClinic==null ? "" : provClinic.StateLicense);
						break;
					case "labcase.LabCaseNum":
						field.FieldValue=labcase.LabCaseNum.ToString();
						break;
				}
			}
		}

		private static void FillFieldsForExamSheet(Sheet sheet,Patient pat) {
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "Birthdate":
						field.FieldValue=pat.Birthdate.ToShortDateString();
						break;
					case "FName":
						field.FieldValue=pat.FName;
						break;
					case "Gender":
						if(field.RadioButtonValue==pat.Gender.ToString()) {
							field.FieldValue="X";
						}
						break;
					case "LName":
						field.FieldValue=pat.LName;
						break;
					case "MiddleI":
						field.FieldValue=pat.MiddleI;
						break;
					case "patient.priProvNameFL":
						field.FieldValue=Providers.GetFormalName(pat.PriProv);
						break;
					case "Preferred":
						field.FieldValue=pat.Preferred;
						break;
					case "Race":
						if(field.RadioButtonValue==PatientRaces.GetPatientRaceOldFromPatientRaces(pat.PatNum).ToString()) { //==pat.Race.ToString()) {
							field.FieldValue="X";
						}
						break;
					case "sheet.DateTimeSheet":
						field.FieldValue=sheet.DateTimeSheet.ToString();
						break;
				}
			}
		}

		private static void FillFieldsForDepositSlip(Sheet sheet,Deposit deposit) {
			List <Payment> PatPayList=Payments.GetForDeposit(deposit.DepositNum);
			ClaimPayment[] ClaimPayList=ClaimPayments.GetForDeposit(deposit.DepositNum);
			//Stores all deposit list items. Used for depositList sheetfield item.
			List<string[]> depositList=new List<string[]> ();
			//Stores the list of deposit items used to display itemized deposit items, not summary. It will not include deposit items that have the 
			//PayType=PrefC.AccountingCashPaymentType and the sheet has the sheetfield item of cashSumTotal.
			//Used for checkNumber(..) and depositItem(..) sheet fields 
			List<string[]> listDepositItems=new List<string[]> ();
			//true if the sheet has a sheetfield named "cashSumTotal". Used to get a list of depositItems. 
			bool hasCashSumTotal=sheet.GetSheetFieldByName("cashSumTotal")!=null;
			int[] colSize=new int[] {11,33,15,14,0};
			for(int i=0;i<PatPayList.Count;i++){
				string amount=PatPayList[i].PayAmt.ToString("F");
				colSize[4]=Math.Max(colSize[4],amount.Length);
			}
			for(int i=0;i<ClaimPayList.Length;i++){
				string amount=ClaimPayList[i].CheckAmt.ToString("F");
				colSize[4]=Math.Max(colSize[4],amount.Length);
			}
			decimal cashSumTotal=0;
			foreach(Payment payCur in PatPayList) { 
				string[] depositItem=new string[5];
				string date=payCur.PayDate.ToShortDateString();
				if(date.Length>colSize[0]){
					date=date.Substring(0,colSize[0]);
				}
				depositItem[0]=date.PadRight(colSize[0],' ')+" ";
				Patient pat=Patients.GetPat(payCur.PatNum);
				string name=pat.GetNameLF();
				if(name.Length>colSize[1]){
					name=name.Substring(0,colSize[1]);
				}
				depositItem[1]=name.PadRight(colSize[1],' ')+" ";
				string checkNum=payCur.CheckNum;
				if(checkNum.Length>colSize[2]){
					checkNum=checkNum.Substring(0,colSize[2]);
				}
				depositItem[2]=checkNum.PadRight(colSize[2],' ')+" ";
				string bankBranch=payCur.BankBranch;
				if(bankBranch.Length>colSize[3]){
					bankBranch=bankBranch.Substring(0,colSize[3]);
				}
				depositItem[3]=bankBranch.PadRight(colSize[3],' ')+" ";
				depositItem[4]=payCur.PayAmt.ToString("F").PadLeft(colSize[4],' ');
				depositList.Add(depositItem);
				if(hasCashSumTotal && payCur.PayType==PrefC.GetLong(PrefName.AccountingCashPaymentType)) {
					cashSumTotal+=(decimal)payCur.PayAmt;
					continue;
				}
				listDepositItems.Add(depositItem);
			}
			foreach(ClaimPayment claimPayCur in ClaimPayList) {
				string[] depositItem=new string[5];
				string date=claimPayCur.CheckDate.ToShortDateString();
				if(date.Length>colSize[0]){
					date=date.Substring(0,colSize[0]);
				}
				depositItem[0]=date.PadRight(colSize[0],' ')+" ";
				string name=claimPayCur.CarrierName;
				if(name.Length>colSize[1]){
					name=name.Substring(0,colSize[1]);
				}
				depositItem[1]=name.PadRight(colSize[1],' ')+" ";
				string checkNum=claimPayCur.CheckNum;
				if(checkNum.Length>colSize[2]){
					checkNum=checkNum.Substring(0,colSize[2]);
				}
				depositItem[2]=checkNum.PadRight(colSize[2],' ')+" ";
				string bankBranch=claimPayCur.BankBranch;
				if(bankBranch.Length>colSize[3]){
					bankBranch=bankBranch.Substring(0,colSize[3]);
				}
				depositItem[3]=bankBranch.PadRight(colSize[3],' ')+" ";
				depositItem[4]=claimPayCur.CheckAmt.ToString("F").PadLeft(colSize[4],' ');
				depositList.Add(depositItem);
				if(hasCashSumTotal && claimPayCur.PayType==PrefC.GetLong(PrefName.AccountingCashPaymentType)) {
					continue;
				}
				listDepositItems.Add(depositItem);
			}
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "cashSumTotal":
						field.FieldValue=cashSumTotal.ToString("n").PadLeft(12);
						break;
					case "checkNumber01":
						if(listDepositItems.Count>=1) {
							field.FieldValue=listDepositItems[0][2].PadLeft(15);
						}
						break;
					case "checkNumber02":
						if(listDepositItems.Count>=2) {
							field.FieldValue=listDepositItems[1][2].PadLeft(15);
						}
						break;
					case "checkNumber03":
						if(listDepositItems.Count>=3) {
							field.FieldValue=listDepositItems[2][2].PadLeft(15);
						}
						break;
					case "checkNumber04":
						if(listDepositItems.Count>=4) {
							field.FieldValue=listDepositItems[3][2].PadLeft(15);
						}
						break;
					case "checkNumber05":
						if(listDepositItems.Count>=5) {
							field.FieldValue=listDepositItems[4][2].PadLeft(15);
						}
						break;
					case "checkNumber06":
						if(listDepositItems.Count>=6) {
							field.FieldValue=listDepositItems[5][2].PadLeft(15);
						}
						break;
					case "checkNumber07":
						if(listDepositItems.Count>=7) {
							field.FieldValue=listDepositItems[6][2].PadLeft(15);
						}
						break;
					case "checkNumber08":
						if(listDepositItems.Count>=8) {
							field.FieldValue=listDepositItems[7][2].PadLeft(15);
						}
						break;
					case "checkNumber09":
						if(listDepositItems.Count>=9) {
							field.FieldValue=listDepositItems[8][2].PadLeft(15);
						}
						break;
					case "checkNumber10":
						if(listDepositItems.Count>=10) {
							field.FieldValue=listDepositItems[9][2].PadLeft(15);
						}
						break;
					case "checkNumber11":
						if(listDepositItems.Count>=11) {
							field.FieldValue=listDepositItems[10][2].PadLeft(15);
						}
						break;
					case "checkNumber12":
						if(listDepositItems.Count>=12) {
							field.FieldValue=listDepositItems[11][2].PadLeft(15);
						}
						break;
					case "checkNumber13":
						if(listDepositItems.Count>=13) {
							field.FieldValue=listDepositItems[12][2].PadLeft(15);
						}
						break;
					case "checkNumber14":
						if(listDepositItems.Count>=14) {
							field.FieldValue=listDepositItems[13][2].PadLeft(15);
						}
						break;
					case "checkNumber15":
						if(listDepositItems.Count>=15) {
							field.FieldValue=listDepositItems[14][2].PadLeft(15);
						}
						break;
					case "checkNumber16":
						if(listDepositItems.Count>=16) {
							field.FieldValue=listDepositItems[15][2].PadLeft(15);
						}
						break;
					case "checkNumber17":
						if(listDepositItems.Count>=17) {
							field.FieldValue=listDepositItems[16][2].PadLeft(15);
						}
						break;
					case "checkNumber18":
						if(listDepositItems.Count>=18) {
							field.FieldValue=listDepositItems[17][2].PadLeft(15);
						}
						break;
					case "deposit.BankAccountInfo":
						field.FieldValue=deposit.BankAccountInfo;
						break;
					case "deposit.DateDeposit":
						field.FieldValue=deposit.DateDeposit.ToShortDateString();
						break;
					case "depositList":
						StringBuilder depositListB=new StringBuilder(Lans.g("Deposits","Date").PadRight(12)+Lans.g("Deposits","Name").PadRight(34)
							+Lans.g("Deposits","Check Number").PadRight(16)+Lans.g("Deposits","Bank-Branch").PadRight(15)+Lans.g("Deposits","Amount")+Environment.NewLine);
						for(int i=0;i<depositList.Count;i++){
							if(i>0){
								depositListB.Append(Environment.NewLine);
							}
							for(int j=0;j<5;j++){
								depositListB.Append(depositList[i][j]);
							}
						}
						field.FieldValue=depositListB.ToString();
						break;
					case "depositTotal":
						decimal total=0;
						for(int i=0;i<PatPayList.Count;i++){
							total+=(decimal)PatPayList[i].PayAmt;
						}
						for(int i=0;i<ClaimPayList.Length;i++){
							total+=(decimal)ClaimPayList[i].CheckAmt;
						}
						field.FieldValue=total.ToString("n").PadLeft(12,' ');
						break;
					case "depositItemCount":
						field.FieldValue=(listDepositItems.Count+(cashSumTotal>0?1:0)).ToString().PadLeft(2,'0');
						break;
					case "depositItem01":
						if(listDepositItems.Count>=1){
							field.FieldValue=listDepositItems[0][4].PadLeft(12,' ');
						}
						break;
					case "depositItem02":
						if(listDepositItems.Count>=2){
							field.FieldValue=listDepositItems[1][4].PadLeft(12,' ');
						}
						break;
					case "depositItem03":
						if(listDepositItems.Count>=3){
							field.FieldValue=listDepositItems[2][4].PadLeft(12,' ');
						}
						break;
					case "depositItem04":
						if(listDepositItems.Count>=4){
							field.FieldValue=listDepositItems[3][4].PadLeft(12,' ');
						}
						break;
					case "depositItem05":
						if(listDepositItems.Count>=5){
							field.FieldValue=listDepositItems[4][4].PadLeft(12,' ');
						}
						break;
					case "depositItem06":
						if(listDepositItems.Count>=6){
							field.FieldValue=listDepositItems[5][4].PadLeft(12,' ');
						}
						break;
					case "depositItem07":
						if(listDepositItems.Count>=7){
							field.FieldValue=listDepositItems[6][4].PadLeft(12,' ');
						}
						break;
					case "depositItem08":
						if(listDepositItems.Count>=8){
							field.FieldValue=listDepositItems[7][4].PadLeft(12,' ');
						}
						break;
					case "depositItem09":
						if(listDepositItems.Count>=9){
							field.FieldValue=listDepositItems[8][4].PadLeft(12,' ');
						}
						break;
					case "depositItem10":
						if(listDepositItems.Count>=10){
							field.FieldValue=listDepositItems[9][4].PadLeft(12,' ');
						}
						break;
					case "depositItem11":
						if(listDepositItems.Count>=11){
							field.FieldValue=listDepositItems[10][4].PadLeft(12,' ');
						}
						break;
					case "depositItem12":
						if(listDepositItems.Count>=12){
							field.FieldValue=listDepositItems[11][4].PadLeft(12,' ');
						}
						break;
					case "depositItem13":
						if(listDepositItems.Count>=13){
							field.FieldValue=listDepositItems[12][4].PadLeft(12,' ');
						}
						break;
					case "depositItem14":
						if(listDepositItems.Count>=14){
							field.FieldValue=listDepositItems[13][4].PadLeft(12,' ');
						}
						break;
					case "depositItem15":
						if(listDepositItems.Count>=15){
							field.FieldValue=listDepositItems[14][4].PadLeft(12,' ');
						}
						break;
					case "depositItem16":
						if(listDepositItems.Count>=16){
							field.FieldValue=listDepositItems[15][4].PadLeft(12,' ');
						}
						break;
					case "depositItem17":
						if(listDepositItems.Count>=17){
							field.FieldValue=listDepositItems[16][4].PadLeft(12,' ');
						}
						break;
					case "depositItem18":
						if(listDepositItems.Count>=18){
							field.FieldValue=listDepositItems[17][4].PadLeft(12,' ');
						}
						break;
				}
			}
		}

		private static void FillFieldsForStatement(Sheet sheet,Statement Stmt,DataSet dataSet,Patient pat,Patient patGuar=null) {
			long patNum;
			if(Stmt.SuperFamily!=0) {//Superfamily statement
				patNum=Stmt.SuperFamily;
			}
			else {
				patNum=Stmt.PatNum;
			}
			pat=(pat==null || pat.PatNum!=patNum ? Patients.GetPat(patNum) : pat);
			patGuar=(patGuar==null || patGuar.PatNum!=pat.Guarantor ? Patients.GetPat(pat.Guarantor) : patGuar);
			DataTable tableAppt=dataSet.Tables["appts"];
			string[] totInsBalVals=totInsBalValsHelper(sheet,Stmt,dataSet,pat,patGuar);
			string[] totInsBalLabs=totInsBalLabsHelper(sheet,Stmt);
			if(tableAppt==null){
				tableAppt=new DataTable();	
			}
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "accountNumber":
						#region Account Number
						field.FieldValue=Lans.g("Statements","Account Number")+" ";
						if(PrefC.GetBool(PrefName.StatementAccountsUseChartNumber)) {
							field.FieldValue+=patGuar.ChartNumber;
						}
						else {
							field.FieldValue+=patGuar.PatNum;
						}
						#endregion
						break;
					case "futureAppointments":
						#region Future Appointments
						if(!Stmt.IsReceipt && !Stmt.IsInvoice) {
							if(tableAppt.Rows.Count>0) {
								field.FieldValue=Lans.g("Statements","Scheduled Appointments:");
							}
							for(int i=0;i<tableAppt.Rows.Count;i++) {
								field.FieldValue+="\r\n"+tableAppt.Rows[i]["descript"].ToString();
							}
						}
						#endregion
						break;
					case "statement.NoteBold":
						field.FieldValue=Stmt.NoteBold;
						if(field.FieldValue==null) {
							field.FieldValue="";
						}
						break;
					case "statement.Note":
						field.FieldValue=Stmt.Note;
						if(field.FieldValue==null) {
							field.FieldValue="";
						}
						break;
					case "totalLabel":
						field.FieldValue=totInsBalLabs[0];
						break;
					case "insEstLabel":
						field.FieldValue=totInsBalLabs[1];
						break;
					case "balanceLabel":
						field.FieldValue=totInsBalLabs[2];
						break;
					case "invoicePaymentLabel": //only for invoices
						field.FieldValue=totInsBalLabs[3];
						break;
					case "invoiceTotalLabel"://only for invoices
						field.FieldValue=totInsBalLabs[4];
						break;
					case "totalValue":
						field.FieldValue=totInsBalVals[0];
						break;
					case "insEstValue":
						field.FieldValue=totInsBalVals[1];
						break;
					case "balanceValue":
						field.FieldValue=totInsBalVals[2];
						break;
					case "amountDueValue":
						try {
							field.FieldValue=PIn.Double(SheetDataTableUtil.GetDataTableForGridType(sheet,dataSet,"StatementEnclosed",Stmt,null).Rows[0][0].ToString()).ToString("C");
						}
						catch {
							field.FieldValue=0.ToString("C");
						}
						break;
					case "payPlanAmtDueValue":
						DataTable tableMisc=dataSet.Tables["misc"];
						if(tableMisc==null){
							tableMisc=new DataTable();	
						}
						double payPlanDue=tableMisc.Select().Where(x => x["descript"].ToString()=="payPlanDue").Sum(x => PIn.Double(x["value"].ToString()));
						field.FieldValue=payPlanDue.ToString("c");
						break;
					case "invoicePaymentValue"://only for invoices
						field.FieldValue=totInsBalVals[3];
						break;
					case "invoiceTotalValue"://only for invoices
						field.FieldValue=totInsBalVals[4];
						break;
					case "statementReceiptInvoice":
						#region Sta/Rec/Inv
						if(Stmt.IsInvoice) {
							if(CultureInfo.CurrentCulture.Name=="en-NZ" || CultureInfo.CurrentCulture.Name=="en-AU") {//New Zealand and Australia
								field.FieldValue=Lans.g("Statements","TAX INVOICE");
							}
							else {
								field.FieldValue=Lans.g("Statements","INVOICE")+" #"+Stmt.StatementNum.ToString();
							}
						}
						else if(Stmt.IsReceipt) {
							field.FieldValue=Lans.g("Statements","RECEIPT");
							if(CultureInfo.CurrentCulture.Name.EndsWith("SG")) {//SG=Singapore
								field.FieldValue+=" #"+Stmt.StatementNum.ToString();
							}
						}
						else {
							field.FieldValue=Lans.g("Statements","STATEMENT");
							if(Stmt.StatementType==StmtType.LimitedStatement) {
								field.FieldValue+=" ("+Lans.g("Statements","Limited")+")";
							}
						}
						#endregion
						break;
					case "returnAddress":
						#region ReturnAddress
						if(!PrefC.GetBool(PrefName.StatementShowReturnAddress)) {
							field.FieldValue="";
							break;
						}
						#region Practice Address
						if(PrefC.HasClinicsEnabled && Clinics.GetCount() > 0 //if using clinics
							&& Clinics.GetClinic(patGuar.ClinicNum)!=null)//and this patient assigned to a clinic
						{
							Clinic clinic=Clinics.GetClinic(patGuar.ClinicNum);
							field.FieldValue=clinic.Description+"\r\n";
							if(CultureInfo.CurrentCulture.Name=="en-AU") {//Australia
								Provider defaultProv=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
								field.FieldValue+="ABN: "+defaultProv.NationalProvID+"\r\n";
							}
							if(CultureInfo.CurrentCulture.Name=="en-NZ") {//New Zealand
								Provider defaultProv=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
								field.FieldValue+="GST: "+defaultProv.SSN+"\r\n";
							}
							field.FieldValue+=clinic.Address+"\r\n";
							if(clinic.Address2!="") {
								field.FieldValue+=clinic.Address2+"\r\n";
							}
							if(CultureInfo.CurrentCulture.Name.EndsWith("CH")) {//CH is for switzerland. eg de-CH
								field.FieldValue+=clinic.Zip+" "+clinic.City+"\r\n";
							}
							else if(CultureInfo.CurrentCulture.Name.EndsWith("SG")) {//SG=Singapore
								field.FieldValue+=clinic.City+" "+clinic.Zip+"\r\n";
							}
							else {
								field.FieldValue+=clinic.City+", "+clinic.State+" "+clinic.Zip+"\r\n";
							}
							if(clinic.Phone.Length==10) {
								field.FieldValue+=TelephoneNumbers.ReFormat(clinic.Phone)+"\r\n";
							}
							else {
								field.FieldValue+=clinic.Phone+"\r\n";
							}
						}
						else {//no clinics
							field.FieldValue=PrefC.GetString(PrefName.PracticeTitle)+"\r\n";
							if(CultureInfo.CurrentCulture.Name=="en-AU") {//Australia
								Provider defaultProv=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
								field.FieldValue+="ABN: "+defaultProv.NationalProvID+"\r\n";
							}
							if(CultureInfo.CurrentCulture.Name=="en-NZ") {//New Zealand
								Provider defaultProv=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
								field.FieldValue+="GST: "+defaultProv.SSN+"\r\n";
							}
							field.FieldValue+=PrefC.GetString(PrefName.PracticeAddress)+"\r\n";
							if(PrefC.GetString(PrefName.PracticeAddress2)!="") {
								field.FieldValue+=PrefC.GetString(PrefName.PracticeAddress2)+"\r\n";
							}
							if(CultureInfo.CurrentCulture.Name.EndsWith("CH")) {//CH is for switzerland. eg de-CH
								field.FieldValue+=PrefC.GetString(PrefName.PracticeZip)+" "+PrefC.GetString(PrefName.PracticeCity)+"\r\n";
							}
							else if(CultureInfo.CurrentCulture.Name.EndsWith("SG")) {//SG=Singapore
								field.FieldValue+=PrefC.GetString(PrefName.PracticeCity)+" "+PrefC.GetString(PrefName.PracticeZip)+"\r\n";
							}
							else {
								field.FieldValue+=PrefC.GetString(PrefName.PracticeCity)+", "+PrefC.GetString(PrefName.PracticeST)+" "+PrefC.GetString(PrefName.PracticeZip)+"\r\n";
							}
							string practicePhone=PrefC.GetString(PrefName.PracticePhone);
							if(practicePhone.Length==10) {
								field.FieldValue+=TelephoneNumbers.ReFormat(practicePhone)+"\r\n";
							}
							else {
								field.FieldValue+=practicePhone+"\r\n";
							}
						}
						#endregion
						#endregion
						break;
					case "billingAddress":
						#region BillingAddress
						if(Stmt.SinglePatient){
							field.FieldValue=pat.GetNameFLnoPref()+"\r\n";
						}
						else{
							field.FieldValue=patGuar.GetNameFLFormal()+"\r\n";
						}
						field.FieldValue+=patGuar.Address+"\r\n";
						if(patGuar.Address2!="") {
							field.FieldValue+=patGuar.Address2+"\r\n";
						}
						if(CultureInfo.CurrentCulture.Name.EndsWith("CH")) {//CH is for switzerland. eg de-CH
							field.FieldValue+=(patGuar.Zip+" "+patGuar.City).Trim();//no line break
						}
						else if(CultureInfo.CurrentCulture.Name.EndsWith("SG")) {//SG=Singapore
							field.FieldValue+=(patGuar.City+" "+patGuar.Zip).Trim();//no line break
						}
						else {
							field.FieldValue+=((patGuar.City+", "+patGuar.State).Trim(new[] { ',',' ' })+" "+patGuar.Zip).Trim();//no line break
						}
						if(!string.IsNullOrWhiteSpace(patGuar.Country)) {
							if(CultureInfo.CurrentCulture.Name.EndsWith("CH")||CultureInfo.CurrentCulture.Name.EndsWith("SG")) {//if Singapore or Switzerland
								if(!string.IsNullOrWhiteSpace(patGuar.City+patGuar.Zip)) {//and either city or zip are not blank, add line break
									field.FieldValue+="\r\n";
								}
							}
							else {//all other cultures
								if(!string.IsNullOrWhiteSpace(patGuar.City+patGuar.State+patGuar.Zip)) {//any field, city, state or zip contain data, add line break
									field.FieldValue+="\r\n";
								}
							}
							field.FieldValue+=patGuar.Country;
						}
						#endregion
						break;
					case "practiceTitle":
						field.FieldValue=PrefC.GetString(PrefName.PracticeTitle);
						break;
					case "statementIsCopy":
						field.FieldValue=(Stmt.IsInvoiceCopy?Lans.g("Statements","COPY"):"");
						break;
					case "statementIsTaxReceipt":
						//if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) { field.FieldValue=""; break; }
						field.FieldValue=(Stmt.IsReceipt?Lans.g("Statements","KEEP THIS RECEIPT FOR INCOME TAX PURPOSES"):"");
						break;
					case "practiceAddress":
						field.FieldValue=PrefC.GetString(PrefName.PracticeAddress);
						if(PrefC.GetString(PrefName.PracticeAddress2) != "") {
							field.FieldValue+="\r\n"+PrefC.GetString(PrefName.PracticeAddress2);
						}
						break;
					case "practiceCityStateZip":
						field.FieldValue=PrefC.GetString(PrefName.PracticeCity)+", "
							+PrefC.GetString(PrefName.PracticeST)+"  "
							+PrefC.GetString(PrefName.PracticeZip);
						break;
					case "statement.DateSent":
						field.FieldValue=Stmt.DateSent.ToShortDateString();
						break;
					case "patient.salutation":
						field.FieldValue="Dear "+pat.GetSalutation()+":";
						break;
					case "patient.priProvNameFL":
						field.FieldValue=Providers.GetFormalName(pat.PriProv);
						break;
					case "ProviderLegendAUS":
						#region ProviderLegendAUS
						if(CultureInfo.CurrentCulture.Name!="en-AU") {//English (Australia)
							field.FieldValue="";
							break;
						}
						Providers.RefreshCache();
						List<Provider> listProviders=Providers.GetDeepCopy(true);
						field.FieldValue="PROVIDERS:"+"\r\n";
						for(int i=0;i<listProviders.Count;i++) {//All non-hidden providers are added to the legend.
							Provider prov=listProviders[i];
							string suffix="";
							if(prov.Suffix.Trim()!=""){
								suffix=", "+prov.Suffix.Trim();
							}
							field.FieldValue+=prov.Abbr+" - "+prov.FName+" "+prov.LName+suffix+" - "+prov.MedicaidID+"\r\n";
						}
						#endregion
						break;
					case "statementURL":
					case "statementShortURL":
						#region StatementURL
						try {
							Statements.AssignURLsIfNecessary(Stmt,pat);
							field.FieldValue=(field.FieldName=="statementURL" ? Stmt.StatementURL : Stmt.StatementShortURL);
							if(field.FieldValue!="") {
								if(!field.FieldValue.StartsWith("http")) {
									field.FieldValue="http://"+field.FieldValue;//Necessary for the URL to be recognized as a hyperlink in the pdf.
								}
							}
						}
						catch(Exception ex) {
							ex.DoNothing();
							field.FieldValue=PrefC.GetString(PrefName.PatientPortalURL);
						}
						break;
					case "invoicePayPlanValue":
						field.FieldValue=totInsBalVals[5];
						break;
					case "invoicePayPlanLabel":
						field.FieldValue=totInsBalLabs[5];
						break;
						#endregion
					case "StatementNum":
						field.FieldValue+=" #"+Stmt.StatementNum.ToString();
						break;
				}
			}
		}

		private static void FillFieldsForMedLabResults(Sheet sheet,MedLab medLab,Patient pat) {
			//pat might be null and sheet.PatNum might be invalid, that is ok.
			List<MedLab> listMedLabs=MedLabs.GetForPatAndSpecimen(pat.PatNum,medLab.SpecimenID,medLab.SpecimenIDFiller);//should always be at least one MedLab
			List<MedLabResult> listResults;
			List<MedLabFacility> listFacilities=MedLabFacilities.GetFacilityList(listMedLabs,out listResults);
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "medlab.ClinicalInfo":
						if(listMedLabs.Count==0) {
							break;
						}
						field.FieldValue=listMedLabs[0].ClinicalInfo;
						for(int i=0;i<listMedLabs.Count;i++) {
							if(Regex.IsMatch(field.FieldValue,Regex.Escape(listMedLabs[i].ClinicalInfo),RegexOptions.IgnoreCase)) {
								continue;
							}
							if(field.FieldValue!="") {
								field.FieldValue+="\r\n";
							}
							field.FieldValue+=listMedLabs[i].ClinicalInfo;
						}
						break;
					case "medlab.dateEntered":
						if(listMedLabs.Count==0) {
							break;
						}
						DateTime minDateEntered=DateTime.MaxValue;
						for(int i=0;i<listMedLabs.Count;i++) {
							if(listMedLabs[i].DateTimeEntered>DateTime.MinValue && listMedLabs[i].DateTimeEntered<minDateEntered) {
								minDateEntered=listMedLabs[i].DateTimeEntered;
							}
						}
						if(minDateEntered==DateTime.MaxValue) {
							field.FieldValue="";
							break;
						}
						field.FieldValue=minDateEntered.ToShortDateString();
						break;
					case "medlab.DateTimeCollected":
						if(listMedLabs.Count==0) {
							break;
						}
						DateTime minDateCollected=DateTime.MaxValue;
						for(int i=0;i<listMedLabs.Count;i++) {
							if(listMedLabs[i].DateTimeCollected>DateTime.MinValue && listMedLabs[i].DateTimeCollected<minDateCollected) {
								minDateCollected=listMedLabs[i].DateTimeCollected;
							}
						}
						if(minDateCollected==DateTime.MaxValue) {
							break;
						}
						field.FieldValue=minDateCollected.ToString("MM/dd/yyyy hh:mm tt");
						break;
					case "medlab.DateTimeReported":
						if(listMedLabs.Count==0) {
							break;
						}
						DateTime maxDateReported=DateTime.MinValue;
						for(int i=0;i<listMedLabs.Count;i++) {
							if(listMedLabs[i].DateTimeReported>maxDateReported) {
								maxDateReported=listMedLabs[i].DateTimeReported;
							}
						}
						if(maxDateReported==DateTime.MinValue) {
							break;
						}
						field.FieldValue=maxDateReported.ToString("MM/dd/yyyy hh:mm tt");
						break;
					case "medlab.NoteLab":
						if(listMedLabs.Count==0) {
							break;
						}
						string patNote="";
						string labNote="";
						for(int i=0;i<listMedLabs.Count;i++) {
							if(!Regex.IsMatch(patNote,Regex.Escape(listMedLabs[i].NotePat),RegexOptions.IgnoreCase)) {
								if(patNote!="") {
									patNote+="\r\n";
								}
								patNote+=listMedLabs[i].NotePat;
							}
							if(!Regex.IsMatch(labNote,Regex.Escape(listMedLabs[i].NoteLab),RegexOptions.IgnoreCase)) {
								if(labNote!="") {
									labNote+="\r\n";
								}
								labNote+=listMedLabs[i].NoteLab;
							}
						}
						field.FieldValue=patNote;
						if(field.FieldValue!="" && labNote!="") {
							field.FieldValue+="\r\n";
						}
						field.FieldValue+=labNote;
						break;
					case "medlab.obsTests":
						List<string> listTestIds=new List<string>();
						for(int i=0;i<listMedLabs.Count;i++) {
							if(listTestIds.Contains(listMedLabs[i].ObsTestID)) {
								continue;
							}
							if(listMedLabs[i].ActionCode==ResultAction.G) {//"G" indicates a reflex test, not a test originally ordered, so skip these
								continue;
							}
							listTestIds.Add(listMedLabs[i].ObsTestID);
							if(field.FieldValue!="") {
								field.FieldValue+="\r\n";
							}
							field.FieldValue+=listMedLabs[i].ObsTestID+" - "+listMedLabs[i].ObsTestDescript+"  (Reported: "
								+listMedLabs[i].DateTimeReported.ToString("MM/dd/yyyy hh:mm tt")+")";
						}
						break;
					case "medlab.ProvID":
						for(int i=0;i<listMedLabs.Count;i++) {
							if(listMedLabs[i].OrderingProvLocalID=="") {
								continue;
							}
							field.FieldValue=listMedLabs[i].OrderingProvLocalID;
							break;
						}
						break;
					case "medlab.provNameLF":
						if(listMedLabs.Count==0) {
							break;
						}
						string provLName="";
						string provFName="";
						for(int i=0;i<listMedLabs.Count;i++) {
							if(listMedLabs[i].OrderingProvFName=="" && listMedLabs[i].OrderingProvLName=="") {//both names missing, continue
								continue;
							}
							provFName=listMedLabs[i].OrderingProvFName;
							provLName=listMedLabs[i].OrderingProvLName;
							if(provFName!="" && provLName!="") {//if both first and last name are included, use the values from this medlab
								break;
							}
						}
						if(provLName!="") {
							field.FieldValue=provLName;
						}
						if(provFName!="") {
							if(field.FieldValue!="") {
								field.FieldValue+=", ";
							}
							field.FieldValue+=provFName;
						}
						break;
					case "medlab.ProvNPI":
						for(int i=0;i<listMedLabs.Count;i++) {
							if(listMedLabs[i].OrderingProvNPI=="") {
								continue;
							}
							field.FieldValue=listMedLabs[i].OrderingProvNPI;
							break;
						}
						break;
					case "medlab.PatAccountNum":
						for(int i=0;i<listMedLabs.Count;i++) {
							if(listMedLabs[i].PatAccountNum=="") {
								continue;
							}
							field.FieldValue=listMedLabs[i].PatAccountNum;
							break;
						}
						break;
					case "medlab.PatAge":
						for(int i=0;i<listMedLabs.Count;i++) {
							if(listMedLabs[i].PatAge=="") {
								continue;
							}
							field.FieldValue=listMedLabs[i].PatAge;
							break;
						}
						break;
					case "medlab.PatFasting":
						for(int i=0;i<listMedLabs.Count;i++) {
							if(listMedLabs[i].PatFasting==YN.Unknown) {
								continue;
							}
							field.FieldValue=listMedLabs[i].PatFasting.ToString();
							break;
						}
						break;
					case "medlab.PatIDAlt":
						for(int i=0;i<listMedLabs.Count;i++) {
							if(listMedLabs[i].PatIDAlt=="") {
								continue;
							}
							field.FieldValue=listMedLabs[i].PatIDAlt;
							break;
						}
						break;
					case "medlab.PatIDLab":
						for(int i=0;i<listMedLabs.Count;i++) {
							if(listMedLabs[i].PatIDLab=="") {
								continue;
							}
							field.FieldValue=listMedLabs[i].PatIDLab;
							break;
						}
						break;
					case "medlab.SpecimenID":
						for(int i=0;i<listMedLabs.Count;i++) {
							if(listMedLabs[i].SpecimenID=="") {
								continue;
							}
							field.FieldValue=listMedLabs[i].SpecimenID;
							break;
						}
						break;
					case "medlab.SpecimenIDAlt":
						for(int i=0;i<listMedLabs.Count;i++) {
							if(listMedLabs[i].SpecimenIDAlt=="") {
								continue;
							}
							field.FieldValue=listMedLabs[i].SpecimenIDAlt;
							break;
						}
						break;
					case "medlab.TotalVolume":
						for(int i=0;i<listMedLabs.Count;i++) {
							if(listMedLabs[i].TotalVolume=="") {
								continue;
							}
							field.FieldValue=listMedLabs[i].TotalVolume;
							break;
						}
						break;
					case "medLabFacilityAddr":
						if(listMedLabs.Count==0) {
							break;
						}
						string fieldValAddr="";
						MedLabFacility facilityCur;
						for(int i=0;i<listFacilities.Count;i++) {
							if(fieldValAddr!="") {
								fieldValAddr+="\r\n\r\n";
							}
							fieldValAddr+=(i+1).ToString().PadLeft(2,'0').PadRight(14,' ');
							string spaces=new string(' ',16);
							facilityCur=listFacilities[i];
							fieldValAddr+=facilityCur.FacilityName+"\r\n"+spaces+facilityCur.Address+"\r\n"+spaces+facilityCur.City+", "+facilityCur.State+"  ";
							if(facilityCur.Zip.Length==9) {
								fieldValAddr+=facilityCur.Zip.Substring(0,5)+"-"+facilityCur.Zip.Substring(5,4);
							}
							else {
								fieldValAddr+=facilityCur.Zip;
							}
						}
						field.FieldValue=fieldValAddr;
						break;
					case "medLabFacilityDir":
						if(listMedLabs.Count==0) {
							break;
						}
						string fieldValDir="";
						MedLabFacility facCur;
						for(int i=0;i<listFacilities.Count;i++) {
							if(fieldValDir!="") {
								fieldValDir+="\r\n\r\n";
							}
							facCur=listFacilities[i];
							fieldValDir+="Dir:  "+facCur.DirectorFName+" "+facCur.DirectorLName+", "+facCur.DirectorTitle+"\r\nPh:  ";
							if(facCur.Phone.Length==10 && TelephoneNumbers.IsFormattingAllowed()) {
								fieldValDir+=facCur.Phone.Substring(0,3)+"-"+facCur.Phone.Substring(3,3)+"-"+facCur.Phone.Substring(6);
							}
							else {
								fieldValDir+=facCur.Phone;
							}
							fieldValDir+="\r\n";//blank line to keep it in line with the address
						}
						field.FieldValue=fieldValDir;
						break;
					case "patient.addrCityStZip":
						if(pat==null) {
							continue;
						}
						field.FieldValue="";
						if(pat.Address!="") {
							field.FieldValue+=pat.Address+"\r\n";
						}
						if(pat.Address2!="") {
							field.FieldValue+=pat.Address2+"\r\n";
						}
						if(pat.City!="") {
							field.FieldValue+=pat.City;
							if(pat.State!="" || pat.Zip!="") {
								field.FieldValue+=", ";
							}
						}
						if(pat.State!="") {
							field.FieldValue+=pat.State;
							if(pat.Zip!="") {
								field.FieldValue+=" ";
							}
						}
						field.FieldValue+=pat.Zip;
						break;
					case "patient.Birthdate":
						if(pat==null) {
							continue;
						}
						if(pat.Birthdate.Year<1880) {
							field.FieldValue="";
							break;
						}
						field.FieldValue=pat.Birthdate.ToShortDateString();
						break;
					case "patient.FName":
						if(pat==null) {
							continue;
						}
						field.FieldValue=pat.FName;
						break;
					case "patient.Gender":
						if(pat==null) {
							continue;
						}
						field.FieldValue=Lans.g("enumPatientGender",pat.Gender.ToString());
						break;
					case "patient.HmPhone":
						if(pat==null) {
							continue;
						}
						if(pat.HmPhone.Length==10 && TelephoneNumbers.IsFormattingAllowed()) {
							field.FieldValue=pat.HmPhone.Substring(0,3)+"-"+pat.HmPhone.Substring(3,3)+"-"+pat.HmPhone.Substring(6);
							break;
						}
						field.FieldValue=pat.HmPhone;
						break;
					case "patient.MiddleI":
						if(pat==null) {
							continue;
						}
						field.FieldValue=pat.MiddleI;
						break;
					case "patient.LName":
						if(pat==null) {
							continue;
						}
						field.FieldValue=pat.LName;
						break;
					case "patient.PatNum":
						if(pat==null) {
							continue;
						}
						field.FieldValue=pat.PatNum.ToString();
						break;
					case "patient.SSN":
						if(pat==null) {
							continue;
						}
						field.FieldValue="***-**-";
						if(pat.SSN.Length>3) {
							field.FieldValue+=pat.SSN.Substring(pat.SSN.Length-4,4);
						}
						break;
					case "practiceAddrCityStZip":
						field.FieldValue="";
						if(PrefC.GetString(PrefName.PracticeAddress)!="") {
							field.FieldValue+=PrefC.GetString(PrefName.PracticeAddress)+"\r\n";
						}
						if(PrefC.GetString(PrefName.PracticeAddress2)!="") {
							field.FieldValue+=PrefC.GetString(PrefName.PracticeAddress2)+"\r\n";
						}
						if(PrefC.GetString(PrefName.PracticeCity)!="") {
							field.FieldValue+=PrefC.GetString(PrefName.PracticeCity);
							if(PrefC.GetString(PrefName.PracticeST)!="" || PrefC.GetString(PrefName.PracticeZip)!="") {
								field.FieldValue+=", ";
							}
						}
						if(PrefC.GetString(PrefName.PracticeST)!="") {
							field.FieldValue+=PrefC.GetString(PrefName.PracticeST);
							if(PrefC.GetString(PrefName.PracticeZip)!="") {
								field.FieldValue+=" ";
							}
						}
						field.FieldValue+=PrefC.GetString(PrefName.PracticeZip);
						break;
					case "PracticePh":
						if(PrefC.GetString(PrefName.PracticePhone).Length==10 && TelephoneNumbers.IsFormattingAllowed()) {
							field.FieldValue=PrefC.GetString(PrefName.PracticePhone).Substring(0,3)+"-"+PrefC.GetString(PrefName.PracticePhone).Substring(3,3)+
								"-"+PrefC.GetString(PrefName.PracticePhone).Substring(6);
							break;
						}
						field.FieldValue=PrefC.GetString(PrefName.PracticePhone);
						break;
					case "PracticeTitle":
						field.FieldValue=PrefC.GetString(PrefName.PracticeTitle);
						break;
				}
			}
		}

		private static void FillFieldsForTreatPlan(Sheet sheet,Patient pat) {
			TreatPlan treatPlan=(TreatPlan)SheetParameter.GetParamByName(sheet.Parameters,"TreatPlan").ParamValue;
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "Heading":
						field.FieldValue=treatPlan.Heading;
						break;
					case "DateTSigned":
						field.FieldValue=(treatPlan.DateTSigned.Year > 1880 ? treatPlan.DateTSigned.ToString() : "");
						break;
					case "DateTPracticeSigned":
						field.FieldValue=(treatPlan.DateTPracticeSigned.Year > 1880 ? treatPlan.DateTPracticeSigned.ToString() : "");
						break;
					case "defaultHeading":
						string value="";
						Clinic clinic;
						if(pat.ClinicNum==0 || !PrefC.HasClinicsEnabled) {
							clinic=Clinics.GetPracticeAsClinicZero();
						}
						else {
							clinic=Clinics.GetClinic(pat.ClinicNum);
						}
						value=clinic.Description;
						if(clinic.Phone.Length==10) {
							value+="\r\n"+TelephoneNumbers.ReFormat(clinic.Phone);
						}
						else {
							value+="\r\n"+clinic.Phone;
						}
						value+="\r\n"+pat.GetNameFLFormal()+", DOB "+pat.Birthdate.ToShortDateString();
						if(treatPlan.ResponsParty!=0) {
							value+="\r\n"+Lans.g("ContrTreat","Responsible Party")+": "+Patients.GetLim(treatPlan.ResponsParty).GetNameFL();
						}
						if(treatPlan.TPStatus==TreatPlanStatus.Saved) {
							value+="\r\n"+treatPlan.DateTP.ToShortDateString();
						}
						else {//active or inactive TP
							value+="\r\n"+DateTime.Today.ToShortDateString();
						}
						field.FieldValue=value;
						break;
					case "Note":
						field.FieldValue=treatPlan.Note;
						break;
					case "SignatureText":
						field.FieldValue=treatPlan.SignatureText;
						break;
					case "SignaturePracticeText":
						field.FieldValue=treatPlan.SignaturePracticeText;
						break;
					case "tpPatPortionEst":
						decimal tpFees=treatPlan.ListProcTPs.Sum(x => (decimal)x.PatAmt);
						field.FieldValue=tpFees.ToString("f");
						break;
				}
			}
		}

		private static void FillFieldsForScreening(Sheet sheet,ScreenGroup screenGroup,Patient pat,Provider prov) {
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "Description":
						field.FieldValue=screenGroup.Description;
						break;
					case "DateScreenGroup":
						field.FieldValue=screenGroup.SGDate.ToShortDateString();
						break;
					case "ProvName":
						if(prov!=null) {
							field.FieldValue=prov.LName+", "+prov.FName;
						}
						else { 
							field.FieldValue=screenGroup.ProvName;
						}
						break;
					case "PlaceOfService":
						field.FieldValue=screenGroup.PlaceService.ToString()+field.FieldValue;
						break;
					case "County":
						field.FieldValue=screenGroup.County;
						break;
					case "GradeSchool":
						field.FieldValue=screenGroup.GradeSchool;
						break;
				}
				//Patient specific fields--------------------------------------------------
				if(pat!=null) {
					switch(field.FieldName) {
						case "Birthdate":
							field.FieldValue=pat.Birthdate.ToShortDateString();
							break;
						case "Age":
							field.FieldValue=pat.Age.ToString();
							break;
						case "FName":
							field.FieldValue=pat.FName;
							break;
						case "LName":
							field.FieldValue=pat.LName;
							break;
						case "MiddleI":
							field.FieldValue=pat.MiddleI;
							break;
						case "Preferred":
							field.FieldValue=pat.Preferred;
							break;
						case "Gender":
							field.FieldValue=pat.Gender.ToString()+field.FieldValue;
							break;
						case "GradeLevel":
							field.FieldValue=pat.GradeLevel.ToString()+field.FieldValue;
							break;
						case "Race/Ethnicity":
							//The patient object no longer has the same type of race so default to Unknown.
							field.FieldValue=PatientRaceOld.Unknown.ToString()+field.FieldValue;
							break;
						case "Urgency":
							field.FieldValue=pat.Urgency.ToString()+field.FieldValue;
							break;
					}
				}
			}
		}

		///<summary>Returns three label strings: Total, Insurance, Balance. These labels change based on various settings.</summary>
		private static string[] totInsBalLabsHelper(Sheet sheet,Statement Stmt) {
			string sLine1="";//Total
			string sLine2="";//InsExt
			string sLine3="";//Balance
			string sLine4="";//InvoicePayments
			string sLine5="";//InvoiceBalRem
			string sLine6="";//Invoice Pay Plan Charges
			if(Stmt.IsInvoice) {//invoices can't be superstatements
				sLine1=Lans.g("Statements","Procedures:");
				sLine2=Lans.g("Statements","Adjustments:");
				sLine6=Lans.g("Statements","Pay Plan Charges:");
				sLine3=Lans.g("Statements","Total:");
				if(PrefC.GetBool(PrefName.InvoicePaymentsGridShowNetProd)) {
					sLine4=Lans.g("Statements","Payments & WriteOffs");
				}
				else {
					sLine4=Lans.g("Statements","Payments:");
				}
				sLine5=Lans.g("Statements","Balance Remaining:");
			}
			else if(PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
				if(Stmt.SuperFamily!=0) {
					sLine1=Lans.g("Statements","Sum of Balances:");
				}
				else {
					sLine1=Lans.g("Statements","Balance:");
				}
				//sLine2=Lans.g("Statements","Ins Pending:");
				//sLine3=Lans.g("Statements","After Ins:");
			}
			else {//this is more common
				if(Stmt.SuperFamily!=0) {
					sLine1=Lans.g("Statements","Sum of Totals:");
					sLine2=Lans.g("Statements","-Sum of Ins Estimates:");
					sLine3=Lans.g("Statements","=Sum of Balances:");
				}
				else {
					sLine1=Lans.g("Statements","Total:");
					sLine2=Lans.g("Statements","-Ins Estimate:");
					sLine3=Lans.g("Statements","=Balance:");
				}
			}
			//sLine4 and sLine5 are only used in invoices
			return new string[] { sLine1,sLine2,sLine3,sLine4,sLine5,sLine6 };
		}

		///<summary>Returns three value strings: Total, Insurance, Balance. These values change based on various settings.</summary>
		private static string[] totInsBalValsHelper(Sheet sheet,Statement Stmt,DataSet stmtData,Patient pat,Patient patGuar) {
			string sLine1="";//Total
			string sLine2="";//InsExt
			string sLine3="";//Balance
			string sLine4="";//InvoicePayments
			string sLine5="";//InvoiceBalanceRemaining
			string sLine6="";//Invoice Pay Plan Charges
			DataTable tableAcct;
			DataSet dataSet=stmtData;
			DataTable tableMisc=dataSet.Tables["misc"];
			if(tableMisc==null) {
				tableMisc=new DataTable();
			}
			if(Stmt.IsInvoice) {
				double adjAmt=0;
				double procAmt=0;
				double payPlanAmt=0;
				string tableName;
				for(int i=0;i<dataSet.Tables.Count;i++) {
					tableAcct=dataSet.Tables[i];
					tableName=tableAcct.TableName;
					if(!tableName.StartsWith("account")) {
						continue;
					}
					for(int p=0;p<tableAcct.Rows.Count;p++) {
						if(tableAcct.Rows[p]["AdjNum"].ToString()!="0") {
							adjAmt-=PIn.Double(tableAcct.Rows[p]["creditsDouble"].ToString());
							adjAmt+=PIn.Double(tableAcct.Rows[p]["chargesDouble"].ToString());
						}
						else if(tableAcct.Rows[p]["PayPlanChargeNum"].ToString()!="0") {
							payPlanAmt+=PIn.Double(tableAcct.Rows[p]["chargesDouble"].ToString());
						}
						else {//must be a procedure
							procAmt+=PIn.Double(tableAcct.Rows[p]["chargesDouble"].ToString());
						}
					}
				}
				double paymentValue=SheetDataTableUtil.GetDataTableForGridType(sheet,dataSet,"StatementInvoicePayment",Stmt,null).Select()
					.Sum(x => PIn.Double(x["amt"].ToString()));
				sLine1+=procAmt.ToString("c");
				sLine2+=adjAmt.ToString("c");
				sLine3+=(procAmt+adjAmt+payPlanAmt).ToString("c");
				sLine4+=paymentValue.ToString("c");
				sLine5+=(paymentValue + procAmt + adjAmt + payPlanAmt).ToString("c");
				sLine6+=payPlanAmt.ToString("c");
			}
			else if(Stmt.StatementType==StmtType.LimitedStatement) {
				double statementTotal=dataSet.Tables.OfType<DataTable>().Where(x => x.TableName.StartsWith("account"))
					.SelectMany(x => x.Rows.OfType<DataRow>())
					.Where(x => x["AdjNum"].ToString()!="0"//adjustments, may be charges or credits
						|| x["ProcNum"].ToString()!="0"//procs, will be charges with credits==0
						|| x["PayNum"].ToString()!="0"//patient payments, will be credits with charges==0
						|| x["ClaimPaymentNum"].ToString()!="0").ToList()//claimproc payments+writeoffs, will be credits with charges==0
					.Sum(x => PIn.Double(x["chargesDouble"].ToString())-PIn.Double(x["creditsDouble"].ToString()));//add charges-credits
				if(PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
					sLine1+=statementTotal.ToString("c");
				}
				else {
					double patInsEst=tableMisc.Rows.OfType<DataRow>()
						.Where(x=>x["descript"].ToString()=="patInsEst")
						.Sum(x=>PIn.Double(x["value"].ToString()));
					sLine1+=statementTotal.ToString("c");
					sLine2+=patInsEst.ToString("c");
					sLine3+=(statementTotal-patInsEst).ToString("c");
				}
			}
			else if(PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
				if(Stmt.SinglePatient) {
					sLine1+=pat.EstBalance.ToString("c");
				}
				else {
					if(Stmt.SuperFamily!=0) {//Superfam statement
						//If the family is included in superfamily billing, sum their totals into the running total.
						//don't include families with negative balances in the total balance for super family (per Nathan 5/25/2016)
						double balTot=Patients.GetSuperFamilyGuarantors(Stmt.SuperFamily).FindAll(x => x.HasSuperBilling && x.BalTotal>0).Sum(x => x.BalTotal);
						sLine1+=balTot.ToString("c");
					}
					else {
						//Show the current family's balance without subtracting insurance estimates.
						sLine1+=patGuar.BalTotal.ToString("c");
					}
				}
			}
			else {//more common
				if(Stmt.SinglePatient) {
					double patInsEst=0;
					for(int m=0;m<tableMisc.Rows.Count;m++) {
						if(tableMisc.Rows[m]["descript"].ToString()=="patInsEst") {
							patInsEst=PIn.Double(tableMisc.Rows[m]["value"].ToString());
						}
					}
					double patBal=pat.EstBalance-patInsEst;
					sLine1+=pat.EstBalance.ToString("c");
					sLine2+=patInsEst.ToString("c");
					sLine3+=patBal.ToString("c");
				}
				else {
					if(Stmt.SuperFamily!=0) {//Superfam statement
						List<Patient> listFamilyGuarantors=Patients.GetSuperFamilyGuarantors(Stmt.SuperFamily).FindAll(x => x.HasSuperBilling && (x.BalTotal-x.InsEst)>=0);
						double balTot=listFamilyGuarantors.Sum(x => x.BalTotal);
						double insEst=listFamilyGuarantors.Sum(x => x.InsEst);
						sLine1+=balTot.ToString("c");
						sLine2+=insEst.ToString("c");
						sLine3+=(balTot-insEst).ToString("c");
					}
					else {
						sLine1+=patGuar.BalTotal.ToString("c");
						sLine2+=patGuar.InsEst.ToString("c");
						sLine3+=(patGuar.BalTotal - patGuar.InsEst).ToString("c");
					}
				}
			}
			//sLine4, sLine5 and sLine6 are only used in invoices
			return new string[] { sLine1,sLine2,sLine3,sLine4,sLine5,sLine6 };
		}

		private static int CompareSheetFieldNames(SheetField input1,SheetField input2) {
			if(input1.IsRequired && !input2.IsRequired) {
				return -1;
			}
			if(!input1.IsRequired && input2.IsRequired) {
				return 1;
			}
			if(Convert.ToInt32(input1.FieldName.Remove(0,8)) < Convert.ToInt32(input2.FieldName.Remove(0,8))) {
				return -1;
			}
			if(Convert.ToInt32(input1.FieldName.Remove(0,8)) > Convert.ToInt32(input2.FieldName.Remove(0,8))) {
				return 1;
			}
			return 0;
		}

		private static void FillFieldsForPaymentPlan(Sheet sheet,Patient pat) {
			PayPlan payPlan=(PayPlan)SheetParameter.GetParamByName(sheet.Parameters,"payplan").ParamValue;
			SheetParameter principal=GetParamByName(sheet,"Principal");
			SheetParameter totFinCharge=GetParamByName(sheet,"totalFinanceCharge");
			SheetParameter totCostLoan=GetParamByName(sheet,"totalCostOfLoan");
			Patient PatGuar=Patients.GetPat(pat.Guarantor);
			List<CreditCard> listCreditCard=CreditCards.GetForPayPlan(payPlan.PayPlanNum);
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "PracticeTitle":
						field.FieldValue=PrefC.GetString(PrefName.PracticeTitle);
						break;
					case "dateToday":
						field.FieldValue=DateTime.Today.ToShortDateString();
						break;
					case "nameLF":
						field.FieldValue=pat.GetNameLF();
						break;
					case "guarantor":
						field.FieldValue=PatGuar.GetNameLFnoPref();
						break;
					case "Principal":
						field.FieldValue=principal.ParamValue.ToString();
						break;
					case "DateOfAgreement":
						field.FieldValue=payPlan.PayPlanDate.ToShortDateString();
						break;
					case "APR":
						field.FieldValue=payPlan.APR.ToString("f1");
						break;
					case "totalFinanceCharge":
						field.FieldValue=((double)totFinCharge.ParamValue).ToString("n");
						break;
					case "totalCostOfLoan":
						field.FieldValue=totCostLoan.ParamValue.ToString();
						break;
					case "Note":
						field.FieldValue=payPlan.Note;
						break;
					case "ccNumberMaskedWithExp":
						field.FieldValue=listCreditCard.IsNullOrEmpty() ? "" : 
							string.Join(", ",listCreditCard.Select(x => $"{x.CCNumberMasked} Exp {x.CCExpiration.ToString("MM/yy")}"));
						break;
					case "TermsAndConditions":
						field.FieldValue=PayPlans.GetTermsAndConditionsString(payPlan,pat,PatGuar);
						break;
				}
			}

		}

		private static void FillFieldsForRxMulti(Sheet sheet,Patient pat) {
			List<RxPat> listMultiRx=(List<RxPat>)SheetParameter.GetParamByName(sheet.Parameters,"ListRxNums").ParamValue;
			//listMultiRxSheet contains a list of ints that correspond to the rxmulti "defs" (1-6) that are on this sheet (i.e Disp5/Drug5/RxDate5 would have a 5 in the list). 
			List<int> listMultiRxSheet=(List<int>)SheetParameter.GetParamByName(sheet.Parameters,"ListRxSheet").ParamValue;//ONE BASED index
			Dictionary<int,RxPat> dictRxs=new Dictionary<int, RxPat>();//ONE BASED index
			Dictionary<int,Provider> dictProvs=new Dictionary<int, Provider>();//ONE BASED index
			for(int i=0;i<listMultiRx.Count;i++) {
				dictRxs.Add(listMultiRxSheet[i],listMultiRx[i].Copy());
				dictProvs.Add(listMultiRxSheet[i],Providers.GetProv(listMultiRx[i].ProvNum));
			}
			Clinic clinic=null;
			if(pat.ClinicNum!=0) {
				clinic=Clinics.GetClinic(pat.ClinicNum);
			}
			List<ProviderClinic> listProvClinics=ProviderClinics.GetByProvNums(dictProvs.Select(x => x.Value).Select(y=>y.ProvNum).Distinct().ToList());
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "prov.nameFL":
						if(dictRxs.ContainsKey(1)) {
							field.FieldValue=dictProvs[1].GetFormalName();
						}
						break;
					case "prov.nameFL2":
						if(dictRxs.ContainsKey(2)) {
							field.FieldValue=dictProvs[2].GetFormalName();
						}
						break;
					case "prov.nameFL3":
						if(dictRxs.ContainsKey(3)) {
							field.FieldValue=dictProvs[3].GetFormalName();
						}
						break;
					case "prov.nameFL4":
						if(dictRxs.ContainsKey(4)) {
							field.FieldValue=dictProvs[4].GetFormalName();
						}							
						break;
					case "prov.nameFL5":
						if(dictRxs.ContainsKey(5)) {
							field.FieldValue=dictProvs[5].GetFormalName();
						}
						break;
					case "prov.nameFL6":
						if(dictRxs.ContainsKey(6)) {
							field.FieldValue=dictProvs[6].GetFormalName();
						}
						break;
					case "clinic.address":
						if(dictRxs.ContainsKey(1)) {
							field.FieldValue=AddressHelper(clinic);
						}
						break;
					case "clinic.address2":
						if(dictRxs.ContainsKey(2)) {
							field.FieldValue=AddressHelper(clinic);
						}
						break;
					case "clinic.address3":
						if(dictRxs.ContainsKey(3)) {
							field.FieldValue=AddressHelper(clinic);
						}
						break;
					case "clinic.address4":
						if(dictRxs.ContainsKey(4)) {
							field.FieldValue=AddressHelper(clinic);
						}
						break;
					case "clinic.address5":
						if(dictRxs.ContainsKey(5)) {
							field.FieldValue=AddressHelper(clinic);
						}
						break;
					case "clinic.address6":
						if(dictRxs.ContainsKey(6)) {
							field.FieldValue=AddressHelper(clinic);
						}
						break;
					case "clinic.cityStateZip":
						if(dictRxs.ContainsKey(1)) {
							field.FieldValue=CityStateHelper(clinic);
						}
						break;
					case "clinic.cityStateZip2":
						if(dictRxs.ContainsKey(2)) {
							field.FieldValue=CityStateHelper(clinic);
						}
						break;
					case "clinic.cityStateZip3":
						if(dictRxs.ContainsKey(3)) {
							field.FieldValue=CityStateHelper(clinic);
						}
						break;
					case "clinic.cityStateZip4":
						if(dictRxs.ContainsKey(4)) {
							field.FieldValue=CityStateHelper(clinic);
						}
						break;
					case "clinic.cityStateZip5":
						if(dictRxs.ContainsKey(5)) {
							field.FieldValue=CityStateHelper(clinic);
						}
						break;
					case "clinic.cityStateZip6":
						if(dictRxs.ContainsKey(6)) {
							field.FieldValue=CityStateHelper(clinic);
						}
						break;
					case "clinic.phone":
						if(dictRxs.ContainsKey(1)) {
							field.FieldValue=PhoneHelper(clinic);
						}
						break;
					case "clinic.phone2":
						if(dictRxs.ContainsKey(2)) {
							field.FieldValue=PhoneHelper(clinic);
						}
						break;
					case "clinic.phone3":
						if(dictRxs.ContainsKey(3)) {
							field.FieldValue=PhoneHelper(clinic);
						}
						break;
					case "clinic.phone4":
						if(dictRxs.ContainsKey(4)) {
							field.FieldValue=PhoneHelper(clinic);
						}
						break;
					case "clinic.phone5":
						if(dictRxs.ContainsKey(5)) {
							field.FieldValue=PhoneHelper(clinic);
						}
						break;
					case "clinic.phone6":
						if(dictRxs.ContainsKey(6)) {
							field.FieldValue=PhoneHelper(clinic);
						}
						break;
					case "RxDate":
						if(dictRxs.ContainsKey(1)) {
							field.FieldValue=dictRxs[1].RxDate.ToShortDateString();
						}
						break;
					case "RxDate2":
						if(dictRxs.ContainsKey(2)) {
							field.FieldValue=dictRxs[2].RxDate.ToShortDateString();
						}
						break;
					case "RxDate3":
						if(dictRxs.ContainsKey(3)) {
							field.FieldValue=dictRxs[3].RxDate.ToShortDateString();
						}
						break;
					case "RxDate4":
						if(dictRxs.ContainsKey(4)) {
							field.FieldValue=dictRxs[4].RxDate.ToShortDateString();
						}
						break;
					case "RxDate5":
						if(dictRxs.ContainsKey(5)) {
							field.FieldValue=dictRxs[5].RxDate.ToShortDateString();
						}
						break;
					case "RxDate6":
						if(dictRxs.ContainsKey(6)) {
							field.FieldValue=dictRxs[6].RxDate.ToShortDateString();
						}
						break;
					case "RxDateMonthSpelled":
						if(dictRxs.ContainsKey(1)) {
							field.FieldValue=dictRxs[1].RxDate.ToString("MMM dd,yyyy");
						}
						break;
					case "RxDateMonthSpelled2":
						if(dictRxs.ContainsKey(2)) {
							field.FieldValue=dictRxs[2].RxDate.ToString("MMM dd,yyyy");
						}
						break;
					case "RxDateMonthSpelled3":
						if(dictRxs.ContainsKey(3)) {
							field.FieldValue=dictRxs[3].RxDate.ToString("MMM dd,yyyy");
						}
						break;
					case "RxDateMonthSpelled4":
						if(dictRxs.ContainsKey(4)) {
							field.FieldValue=dictRxs[4].RxDate.ToString("MMM dd,yyyy");
						}
						break;
					case "RxDateMonthSpelled5":
						if(dictRxs.ContainsKey(5)) {
							field.FieldValue=dictRxs[5].RxDate.ToString("MMM dd,yyyy");
						}
						break;
					case "RxDateMonthSpelled6":
						if(dictRxs.ContainsKey(6)) {
							field.FieldValue=dictRxs[6].RxDate.ToString("MMM dd,yyyy");
						}
						break;
					case "prov.dEANum":
						if(dictRxs.ContainsKey(1)) {
							if(dictRxs[1].IsControlled) {
								ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[1].ProvNum,dictRxs[1].ClinicNum,listProvClinics,true);
								field.FieldValue=(provClinicCur==null ? "" : provClinicCur.DEANum);
							}
							else {
								field.FieldValue="";
							}
						}
						break;
					case "prov.dEANum2":
						if(dictRxs.ContainsKey(2)) {
							if(dictRxs[2].IsControlled) {
								ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[2].ProvNum,dictRxs[2].ClinicNum,listProvClinics,true);
								field.FieldValue=(provClinicCur==null ? "" : provClinicCur.DEANum);
							}
							else {
								field.FieldValue="";
							}
						}
						break;
					case "prov.dEANum3":
						if(dictRxs.ContainsKey(3)) {
							if(dictRxs[3].IsControlled) {
								ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[3].ProvNum,dictRxs[3].ClinicNum,listProvClinics,true);
								field.FieldValue=(provClinicCur==null ? "" : provClinicCur.DEANum);
							}
							else {
								field.FieldValue="";
							}
						}
						break;
					case "prov.dEANum4":
						if(dictRxs.ContainsKey(4)) {
							if(dictRxs[4].IsControlled) {
								ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[4].ProvNum,dictRxs[4].ClinicNum,listProvClinics,true);
								field.FieldValue=(provClinicCur==null ? "" : provClinicCur.DEANum);
							}
							else {
								field.FieldValue="";
							}
						}
						break;
					case "prov.dEANum5":
						if(dictRxs.ContainsKey(5)) {
							if(dictRxs[5].IsControlled) {
								ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[5].ProvNum,dictRxs[5].ClinicNum,listProvClinics,true);
								field.FieldValue=(provClinicCur==null ? "" : provClinicCur.DEANum);
							}
							else {
								field.FieldValue="";
							}
						}
						break;
					case "prov.dEANum6":
						if(dictRxs.ContainsKey(6)) {
							if(dictRxs[6].IsControlled) {
								ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[6].ProvNum,dictRxs[6].ClinicNum,listProvClinics,true);
								field.FieldValue=(provClinicCur==null ? "" : provClinicCur.DEANum);
							}
							else {
								field.FieldValue="";
							}
						}
						break;
					case "pat.nameFL":
						if(dictRxs.ContainsKey(1)) {
							//Can't include preferred, so:
							field.FieldValue=pat.FName+" "+pat.MiddleI+"  "+pat.LName;
						}
						break;
					case "pat.nameFL2":
						if(dictRxs.ContainsKey(2)) {
							field.FieldValue=pat.FName+" "+pat.MiddleI+"  "+pat.LName;
						}
						break;
					case "pat.nameFL3":
						if(dictRxs.ContainsKey(3)) {
							field.FieldValue=pat.FName+" "+pat.MiddleI+"  "+pat.LName;
						}
						break;
					case "pat.nameFL4":
						if(dictRxs.ContainsKey(4)) {
							field.FieldValue=pat.FName+" "+pat.MiddleI+"  "+pat.LName;
						}
						break;
					case "pat.nameFL5":
						if(dictRxs.ContainsKey(5)) {
							field.FieldValue=pat.FName+" "+pat.MiddleI+"  "+pat.LName;
						}
						break;
					case "pat.nameFL6":
						if(dictRxs.ContainsKey(6)) {
							field.FieldValue=pat.FName+" "+pat.MiddleI+"  "+pat.LName;
						}
						break;
					case "pat.Birthdate":
						if(dictRxs.ContainsKey(1)) {
							if(pat.Birthdate.Year<1880) {
								field.FieldValue="";
							}
							else {
								field.FieldValue=pat.Birthdate.ToShortDateString();
							}
						}
						break;
					case "pat.Birthdate2":
						if(dictRxs.ContainsKey(2)) {
							if(pat.Birthdate.Year<1880) {
								field.FieldValue="";
							}
							else {
								field.FieldValue=pat.Birthdate.ToShortDateString();
							}
						}
						break;
					case "pat.Birthdate3":
						if(dictRxs.ContainsKey(3)) {
							if(pat.Birthdate.Year<1880) {
								field.FieldValue="";
							}
							else {
								field.FieldValue=pat.Birthdate.ToShortDateString();
							}
						}
						break;
					case "pat.Birthdate4":
						if(dictRxs.ContainsKey(4)) {
							if(pat.Birthdate.Year<1880) {
								field.FieldValue="";
							}
							else {
								field.FieldValue=pat.Birthdate.ToShortDateString();
							}
						}
						break;
					case "pat.Birthdate5":
						if(dictRxs.ContainsKey(5)) {
							if(pat.Birthdate.Year<1880) {
								field.FieldValue="";
							}
							else {
								field.FieldValue=pat.Birthdate.ToShortDateString();
							}
						}
						break;
					case "pat.Birthdate6":
						if(dictRxs.ContainsKey(6)) {
							if(pat.Birthdate.Year<1880) {
								field.FieldValue="";
							}
							else {
								field.FieldValue=pat.Birthdate.ToShortDateString();
							}
						}
						break;
					case "pat.HmPhone":
						if(dictRxs.ContainsKey(1)) {
							field.FieldValue=pat.HmPhone;
						}
							break;
					case "pat.HmPhone2":
						if(dictRxs.ContainsKey(2)) {
							field.FieldValue=pat.HmPhone;
						}
						break;
					case "pat.HmPhone3":
						if(dictRxs.ContainsKey(3)) {
							field.FieldValue=pat.HmPhone;
						}
						break;
					case "pat.HmPhone4":
						if(dictRxs.ContainsKey(4)) {
							field.FieldValue=pat.HmPhone;
						}
						break;
					case "pat.HmPhone5":
						if(dictRxs.ContainsKey(5)) {
							field.FieldValue=pat.HmPhone;
						}
						break;
					case "pat.HmPhone6":
						if(dictRxs.ContainsKey(6)) {
							field.FieldValue=pat.HmPhone;
						}
						break;
					case "pat.address":
						if(dictRxs.ContainsKey(1)) {
							field.FieldValue=pat.Address;
							if(pat.Address2!="") {
								field.FieldValue+="\r\n"+pat.Address2;
							}
						}
						break;
					case "pat.address2":
						if(dictRxs.ContainsKey(2)) {
							field.FieldValue=pat.Address;
							if(pat.Address2!="") {
								field.FieldValue+="\r\n"+pat.Address2;
							}
						}
						break;
					case "pat.address3":
						if(dictRxs.ContainsKey(3)) {
							field.FieldValue=pat.Address;
							if(pat.Address2!="") {
								field.FieldValue+="\r\n"+pat.Address2;
							}
						}
						break;
					case "pat.address4":
						if(dictRxs.ContainsKey(4)) {
							field.FieldValue=pat.Address;
							if(pat.Address2!="") {
								field.FieldValue+="\r\n"+pat.Address2;
							}
						}
						break;
					case "pat.address5":
						if(dictRxs.ContainsKey(5)) {
							field.FieldValue=pat.Address;
							if(pat.Address2!="") {
								field.FieldValue+="\r\n"+pat.Address2;
							}
						}
						break;
					case "pat.address6":
						if(dictRxs.ContainsKey(6)) {
							field.FieldValue=pat.Address;
							if(pat.Address2!="") {
								field.FieldValue+="\r\n"+pat.Address2;
							}
						}
						break;
					case "pat.cityStateZip":
						if(dictRxs.ContainsKey(1)) {
							field.FieldValue=pat.City+", "+pat.State+" "+pat.Zip;
						}
						break;
					case "pat.cityStateZip2":
						if(dictRxs.ContainsKey(2)) {
							field.FieldValue=pat.City+", "+pat.State+" "+pat.Zip;
						}
						break;
					case "pat.cityStateZip3":
						if(dictRxs.ContainsKey(3)) {
							field.FieldValue=pat.City+", "+pat.State+" "+pat.Zip;
						}
						break;
					case "pat.cityStateZip4":
						if(dictRxs.ContainsKey(4)) {
							field.FieldValue=pat.City+", "+pat.State+" "+pat.Zip;
						}
						break;
					case "pat.cityStateZip5":
						if(dictRxs.ContainsKey(5)) {
							field.FieldValue=pat.City+", "+pat.State+" "+pat.Zip;
						}
						break;
					case "pat.cityStateZip6":
						if(dictRxs.ContainsKey(6)) {
							field.FieldValue=pat.City+", "+pat.State+" "+pat.Zip;
						}
						break;
					case "Drug":
						if(dictRxs.ContainsKey(1)) {
							field.FieldValue=dictRxs[1].Drug;
						}
						else {
							field.FieldValue=Lans.g("RxMulti","VOID");
							field.FontSize=50;
							field.GrowthBehavior=GrowthBehaviorEnum.FillDown;
						}
						break;
					case "Drug2":
						if(dictRxs.ContainsKey(2)) {
							field.FieldValue=dictRxs[2].Drug;
						}
						else {
							field.FieldValue=Lans.g("RxMulti","VOID");
							field.FontSize=50;
							field.GrowthBehavior=GrowthBehaviorEnum.FillDown;
						}
						break;
					case "Drug3":
						if(dictRxs.ContainsKey(3)) {
							field.FieldValue=dictRxs[3].Drug;
						}
						else {
							field.FieldValue=Lans.g("RxMulti","VOID");
							field.FontSize=50;
							field.GrowthBehavior=GrowthBehaviorEnum.FillDown;
						}
						break;
					case "Drug4":
						if(dictRxs.ContainsKey(4)) {
							field.FieldValue=dictRxs[4].Drug;
						}
						else {
							field.FieldValue=Lans.g("RxMulti","VOID");
							field.FontSize=50;
							field.GrowthBehavior=GrowthBehaviorEnum.FillDown;
						}
						break;
					case "Drug5":
						if(dictRxs.ContainsKey(5)) {
							field.FieldValue=dictRxs[5].Drug;
						}
						else {
							field.FieldValue=Lans.g("RxMulti","VOID");
							field.FontSize=50;
							field.GrowthBehavior=GrowthBehaviorEnum.FillDown;
						}
						break;
					case "Drug6":
						if(dictRxs.ContainsKey(6)) {
							field.FieldValue=dictRxs[6].Drug;
						}
						else {
							field.FieldValue=Lans.g("RxMulti","VOID");
							field.FontSize=50;
							field.GrowthBehavior=GrowthBehaviorEnum.FillDown;
						}
						break;
					case "Disp":
						if(dictRxs.ContainsKey(1)) {
							field.FieldValue=dictRxs[1].Disp;
						}
						break;
					case "Disp2":
						if(dictRxs.ContainsKey(2)) {
							field.FieldValue=dictRxs[2].Disp;
						}
						break;
					case "Disp3":
						if(dictRxs.ContainsKey(3)) {
							field.FieldValue=dictRxs[3].Disp;
						}
						break;
					case "Disp4":
						if(dictRxs.ContainsKey(4)) {
							field.FieldValue=dictRxs[4].Disp;
						}
						break;
					case "Disp5":
						if(dictRxs.ContainsKey(5)) {
							field.FieldValue=dictRxs[5].Disp;
						}
						break;
					case "Disp6":
						if(dictRxs.ContainsKey(6)) {
							field.FieldValue=dictRxs[6].Disp;
						}
						break;
					case "Sig":
						if(dictRxs.ContainsKey(1)) {
							field.FieldValue=dictRxs[1].Sig;
						}
						break;
					case "Sig2":
						if(dictRxs.ContainsKey(2)) {
							field.FieldValue=dictRxs[2].Sig;
						}
						break;
					case "Sig3":
						if(dictRxs.ContainsKey(3)) {
							field.FieldValue=dictRxs[3].Sig;
						}
						break;
					case "Sig4":
						if(dictRxs.ContainsKey(4)) {
							field.FieldValue=dictRxs[4].Sig;
						}
						break;
					case "Sig5":
						if(dictRxs.ContainsKey(5)) {
							field.FieldValue=dictRxs[5].Sig;
						}
						break;
					case "Sig6":
						if(dictRxs.ContainsKey(6)) {
							field.FieldValue=dictRxs[6].Sig;
						}
						break;
					case "Refills":
						if(dictRxs.ContainsKey(1)) {
							field.FieldValue=dictRxs[1].Refills;
						}
						break;
					case "Refills2":
						if(dictRxs.ContainsKey(2)) {
							field.FieldValue=dictRxs[2].Refills;
						}
						break;
					case "Refills3":
						if(dictRxs.ContainsKey(3)) {
							field.FieldValue=dictRxs[3].Refills;
						}
						break;
					case "Refills4":
						if(dictRxs.ContainsKey(4)) {
							field.FieldValue=dictRxs[4].Refills;
						}
						break;
					case "Refills5":
						if(dictRxs.ContainsKey(5)) {
							field.FieldValue=dictRxs[5].Refills;
						}
						break;
					case "Refills6":
						if(dictRxs.ContainsKey(6)) {
							field.FieldValue=dictRxs[6].Refills;
						}
						break;
					case "prov.stateRxID":
						if(dictRxs.ContainsKey(1)) {
							ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[1].ProvNum,dictRxs[1].ClinicNum,listProvClinics,true);
							field.FieldValue=(provClinicCur==null ? "" : provClinicCur.StateRxID);
						}
						break;
					case "prov.stateRxID2":
						if(dictRxs.ContainsKey(2)) {
							ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[2].ProvNum,dictRxs[2].ClinicNum,listProvClinics,true);
							field.FieldValue=(provClinicCur==null ? "" : provClinicCur.StateRxID);
						}
						break;
					case "prov.stateRxID3":
						if(dictRxs.ContainsKey(3)) {
							ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[3].ProvNum,dictRxs[3].ClinicNum,listProvClinics,true);
							field.FieldValue=(provClinicCur==null ? "" : provClinicCur.StateRxID);
						}
						break;
					case "prov.stateRxID4":
						if(dictRxs.ContainsKey(4)) {
							ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[4].ProvNum,dictRxs[4].ClinicNum,listProvClinics,true);
							field.FieldValue=(provClinicCur==null ? "" : provClinicCur.StateRxID);
						}
						break;
					case "prov.stateRxID5":
						if(dictRxs.ContainsKey(5)) {
							ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[5].ProvNum,dictRxs[5].ClinicNum,listProvClinics,true);
							field.FieldValue=(provClinicCur==null ? "" : provClinicCur.StateRxID);
						}
						break;
					case "prov.stateRxID6":
						if(dictRxs.ContainsKey(6)) {
							ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[6].ProvNum,dictRxs[6].ClinicNum,listProvClinics,true);
							field.FieldValue=(provClinicCur==null ? "" : provClinicCur.StateRxID);
						}
						break;
					case "prov.StateLicense":
						if(dictRxs.ContainsKey(1)) {
							ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[1].ProvNum,dictRxs[1].ClinicNum,listProvClinics,true);
							field.FieldValue=(provClinicCur==null ? "" : provClinicCur.StateLicense);
						}
						break;
					case "prov.StateLicense2":
						if(dictRxs.ContainsKey(2)) {
							ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[2].ProvNum,dictRxs[2].ClinicNum,listProvClinics,true);
							field.FieldValue=(provClinicCur==null ? "" : provClinicCur.StateLicense);
						}
						break;
					case "prov.StateLicense3":
						if(dictRxs.ContainsKey(3)) {
							ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[3].ProvNum,dictRxs[3].ClinicNum,listProvClinics,true);
							field.FieldValue=(provClinicCur==null ? "" : provClinicCur.StateLicense);
						}
						break;
					case "prov.StateLicense4":
						if(dictRxs.ContainsKey(4)) {
							ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[4].ProvNum,dictRxs[4].ClinicNum,listProvClinics,true);
							field.FieldValue=(provClinicCur==null ? "" : provClinicCur.StateLicense);
						}
						break;
					case "prov.StateLicense5":
						if(dictRxs.ContainsKey(5)) {
							ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[5].ProvNum,dictRxs[5].ClinicNum,listProvClinics,true);
							field.FieldValue=(provClinicCur==null ? "" : provClinicCur.StateLicense);
						}
						break;
					case "prov.StateLicense6":
						if(dictRxs.ContainsKey(6)) {
							ProviderClinic provClinicCur=ProviderClinics.GetFromList(dictRxs[6].ProvNum,dictRxs[6].ClinicNum,listProvClinics,true);
							field.FieldValue=(provClinicCur==null ? "" : provClinicCur.StateLicense);
						}
						break;
					case "prov.NationalProvID":
						if(dictRxs.ContainsKey(1)) {
							field.FieldValue=dictProvs[1].NationalProvID;
						}
						break;
					case "prov.NationalProvID2":
						if(dictRxs.ContainsKey(2)) {
							field.FieldValue=dictProvs[2].NationalProvID;
						}
						break;
					case "prov.NationalProvID3":
						if(dictRxs.ContainsKey(3)) {
							field.FieldValue=dictProvs[3].NationalProvID;
						}
						break;
					case "prov.NationalProvID4":
						if(dictRxs.ContainsKey(4)) {
							field.FieldValue=dictProvs[4].NationalProvID;
						}
						break;
					case "prov.NationalProvID5":
						if(dictRxs.ContainsKey(5)) {
							field.FieldValue=dictProvs[5].NationalProvID;
						}
						break;
					case "prov.NationalProvID6":
						if(dictRxs.ContainsKey(6)) {
							field.FieldValue=dictProvs[6].NationalProvID;
						}
						break;
					case "ProcCode":
						if(dictRxs.ContainsKey(1)) {
							field.FieldValue=GetRxFieldProcCode(dictRxs[1]);
						}
						break;
					case "ProcCode2":
						if(dictRxs.ContainsKey(2)) {
							field.FieldValue=GetRxFieldProcCode(dictRxs[2]);
						}
						break;
					case "ProcCode3":
						if(dictRxs.ContainsKey(3)) {
							field.FieldValue=GetRxFieldProcCode(dictRxs[3]);
						}
						break;
					case "ProcCode4":
						if(dictRxs.ContainsKey(4)) {
							field.FieldValue=GetRxFieldProcCode(dictRxs[4]);
						}
						break;
					case "ProcCode5":
						if(dictRxs.ContainsKey(5)) {
							field.FieldValue=GetRxFieldProcCode(dictRxs[5]);
						}
						break;
					case "ProcCode6":
						if(dictRxs.ContainsKey(6)) {
							field.FieldValue=GetRxFieldProcCode(dictRxs[6]);
						}
						break;
					case "DaysOfSupply":
						if(dictRxs.ContainsKey(1)) {
							field.FieldValue=GetRxFieldDaysOfSupply(dictRxs[1]);
						}
						break;
					case "DaysOfSupply2":
						if(dictRxs.ContainsKey(2)) {
							field.FieldValue=GetRxFieldDaysOfSupply(dictRxs[2]);
						}
						break;
					case "DaysOfSupply3":
						if(dictRxs.ContainsKey(3)) {
							field.FieldValue=GetRxFieldDaysOfSupply(dictRxs[3]);
						}
						break;
					case "DaysOfSupply4":
						if(dictRxs.ContainsKey(4)) {
							field.FieldValue=GetRxFieldDaysOfSupply(dictRxs[4]);
						}
						break;
					case "DaysOfSupply5":
						if(dictRxs.ContainsKey(5)) {
							field.FieldValue=GetRxFieldDaysOfSupply(dictRxs[5]);
						}
						break;
					case "DaysOfSupply6":
						if(dictRxs.ContainsKey(6)) {
							field.FieldValue=GetRxFieldDaysOfSupply(dictRxs[6]);
						}
						break;
				}
			}
		}
		
		private static void FillFieldsForERA(Sheet sheet) {
			X835 era=(X835)GetParamByName(sheet,"ERA").ParamValue;
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "PayerName":
						field.FieldValue=era.PayerName;
						break;
					case "PayerID":
						field.FieldValue=era.PayerId;
						break;
					case "PayerAddress":
						field.FieldValue=era.PayerAddress;
						break;
					case "PayerCity":
						field.FieldValue=era.PayerCity;
						break;
					case "PayerState":
						field.FieldValue=era.PayerState;
						break;
					case "PayerZip":
						field.FieldValue=era.PayerZip;
						break;
					case "PayerContactInfo":
						field.FieldValue=era.GetPayerContactInfo();
						break;
					case "PayeeName":
						field.FieldValue=era.PayeeName;
						break;
					case "PayeeId":
						field.FieldValue=era.PayeeId;
						break;
					case "TransHandlingDesc":
						field.FieldValue=era.TransactionHandlingDescript;
						break;
					case "PaymentMethod":
						field.FieldValue=era.PayMethodDescript;
						break;
					case "AcctNumEndingIn":
						field.FieldValue=era.AccountNumReceiving;
						break;
					case "Check#":
						field.FieldValue=era.TransRefNum;
						break;
					case "DateEffective":
						field.FieldValue=era.DateEffective.ToShortDateString();
						break;
					case "InsPaid":
						field.FieldValue=era.InsPaid.ToString("C2");
						break;
				}
			}
		}
		
		private static void FillFieldsForERAGridHeader(Sheet sheet) {
			Hx835_Claim eraClaimPaid=(Hx835_Claim)GetParamByName(sheet,"EraClaimPaid").ParamValue;
			foreach(SheetField field in sheet.SheetFields) {
				switch(field.FieldName) {
					case "Subscriber":
						field.FieldValue=eraClaimPaid.SubscriberName.ToString();
						break;
					case "Patient":
						field.FieldValue=eraClaimPaid.PatientName.ToString();
						break;
					case "ClaimIdentifier":
						field.FieldValue=eraClaimPaid.ClaimTrackingNumber.ToString();
						break;
					case "PayorControlNum":
						field.FieldValue=eraClaimPaid.PayerControlNumber.ToString();
						break;
					case "Status":
						field.FieldValue=eraClaimPaid.StatusCodeDescript.ToString();
						break;
					case "DateService":
						field.FieldValue=eraClaimPaid.DateServiceStart.ToShortDateString();
						break;
					case "ClaimFee":
						field.FieldValue=eraClaimPaid.ClaimFee.ToString("C2");
						break;
					case "InsPaid":
						field.FieldValue=eraClaimPaid.InsPaid.ToString("C2");
						break;
					case "PatientResponsibility":
						field.FieldValue=eraClaimPaid.PatientRespAmt.ToString("C2");
						break;
					case "DatePayerReceived":
						string value="";
						if(eraClaimPaid.DatePayerReceived!=DateTime.MinValue) {
							value=eraClaimPaid.DatePayerReceived.ToShortDateString();
						}
						field.FieldValue=value;
						break;
					case "ClaimIndexNum":
						int claimIndex=PIn.Int(GetParamByName(sheet,"ClaimIndexNum").ParamValue.ToString());
						if(claimIndex!=0) {//Is 0 when IsSingleClaim parameter is true.
							field.FieldValue=claimIndex.ToString();
						}
						break;
				}
			}
		}

		private static string AddressHelper(Clinic clinic) {
			string text;
			if(PrefC.HasClinicsEnabled && clinic!=null) {
				text=clinic.Address;
				if(clinic.Address2!="") {
					text+="\r\n"+clinic.Address2;
				}
			}
			else {
				text=PrefC.GetString(PrefName.PracticeAddress);
				if(PrefC.GetString(PrefName.PracticeAddress2)!="") {
					text+="\r\n"+PrefC.GetString(PrefName.PracticeAddress2);
				}
			}
			return text;
		}

		private static string CityStateHelper(Clinic clinic) {
			string text;
			if(PrefC.HasClinicsEnabled && clinic!=null) {
				text=clinic.City+", "+clinic.State+" "+clinic.Zip;
			}
			else {
				text=PrefC.GetString(PrefName.PracticeCity)+", "+PrefC.GetString(PrefName.PracticeST)+" "+PrefC.GetString(PrefName.PracticeZip);
			}
			return text;
		}

		private static string PhoneHelper(Clinic clinic) {
			string text;
			if(PrefC.HasClinicsEnabled && clinic!=null) {
				text=clinic.Phone;
			}
			else {
				text=PrefC.GetString(PrefName.PracticePhone);
				text=new string(text.Where(x => char.IsDigit(x)).ToArray());
			}
			if(text.Length==10) {
				text=TelephoneNumbers.ReFormat(text);
			}
			return text;
		}
	}
}
