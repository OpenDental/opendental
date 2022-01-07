using System;
using System.Collections.Generic;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	public class SheetFieldsAvailable {
		/*public static List<SheetFieldDef> GetListInput(SheetTypeEnum sheetType){
			return GetList(sheetType,OutInCheck.In);
		}

		public static List<SheetFieldDef> GetListOutput(SheetTypeEnum sheetType){
			return GetList(sheetType,OutInCheck.Out);
		}

		public static List<SheetFieldDef> GetListCheckBox(SheetTypeEnum sheetType){
			return GetList(sheetType,OutInCheck.Check);
		}*/

		///<summary>Returns a list of SheetFieldDefs associated to the given sheetType and layoutMode.</summary>
		///<param name="sheetType">The sheet type to consider.</param>
		///<param name="layoutMode">Currently only matters when retrieving fields for dynamic layout sheetTypes.</param>
		public static List<SheetFieldDef> GetSpecial(SheetTypeEnum sheetType,SheetFieldLayoutMode layoutMode) {
			List<SheetFieldDef> retVal=new List<SheetFieldDef>();
			switch(sheetType) {
				case SheetTypeEnum.PatientDashboardWidget:
					retVal.AddRange(new[] {
						SheetFieldDef.NewSpecial("familyInsurance",0,0,193,80),
						SheetFieldDef.NewSpecial("individualInsurance",0,0,193,160),
						SheetFieldDef.NewSpecial("toothChart",0,0,500,370),
						SheetFieldDef.NewSpecial("toothChartLegend",0,0,640,14)
					});
					break;
				case SheetTypeEnum.TreatmentPlan:
				case SheetTypeEnum.ReferralLetter:
					retVal.AddRange(new[] {
						SheetFieldDef.NewSpecial("toothChart",0,0,500,370),
						SheetFieldDef.NewSpecial("toothChartLegend",0,0,640,14)
					});
					break;
				case SheetTypeEnum.ChartModule://Dynamic layout, some special fields have growthBehavior. See below and FormSheetFieldSpecial
					retVal.AddRange(new[] {
						SheetFieldDef.NewSpecial("ChartModuleTabs",0,0,524,259),
						SheetFieldDef.NewSpecial("TreatmentNotes",0,0,412,69),
					});
					if(!ListTools.In(layoutMode,SheetFieldLayoutMode.MedicalPractice,SheetFieldLayoutMode.MedicalPracticeTreatPlan)) {
						retVal.AddRange(new[] {
							SheetFieldDef.NewSpecial("toothChart",0,0,410,307),
							SheetFieldDef.NewSpecial("TrackToothProcDates",0,0,329,27)
						});
					}
					if(ListTools.In(layoutMode,SheetFieldLayoutMode.TreatPlan,SheetFieldLayoutMode.EcwTreatPlan,SheetFieldLayoutMode.MedicalPracticeTreatPlan)) {
						retVal.AddRange(new[] {
							SheetFieldDef.NewSpecial("ButtonNewTP",0,0,77,23,fieldValue:"New TP"),
							SheetFieldDef.NewSpecial("SetPriorityListBox",0,0,77,300)//Has growthBehavior logic in FormSheetFieldSpecial.cs
						});
					}
					if(ListTools.In(layoutMode,SheetFieldLayoutMode.Ecw,SheetFieldLayoutMode.EcwTreatPlan)) {
						retVal.Add(SheetFieldDef.NewSpecial("PanelEcw",0,0,411,50));//Has growthBehavior logic in FormSheetFieldSpecial.cs
					}
					if(PrefC.IsODHQ || PrefC.GetBool(PrefName.DistributorKey)) {//Mimics ContrChart.InitializeLocalData(...)
						retVal.AddRange(new[] { 
							SheetFieldDef.NewSpecial("ButtonErxAccess",0,0,75,14,fieldValue:"Erx Access"),
							SheetFieldDef.NewSpecial("ButtonPhoneNums",0,0,75,14,fieldValue:"Phone Nums"),
							SheetFieldDef.NewSpecial("ButtonForeignKey",0,0,75,14,fieldValue:"Foreign Key"),
							SheetFieldDef.NewSpecial("ButtonUSAKey",0,0,75,14,fieldValue:"USA Key")
						});
					}
					break;
			}
			return retVal;
		}

		///<Summary>This is the list of input, output, or checkbox fieldnames for user to pick from.</Summary>
		public static List<SheetFieldDef> GetList(SheetTypeEnum sheetType,OutInCheck outInCheck) {
			switch(sheetType) {
				case SheetTypeEnum.LabelPatient:
					return GetLabelPatient(outInCheck);
				case SheetTypeEnum.LabelCarrier:
					return GetLabelCarrier(outInCheck);
				case SheetTypeEnum.LabelReferral:
					return GetLabelReferral(outInCheck);
				case SheetTypeEnum.ReferralSlip:
					return GetReferralSlip(outInCheck);
				case SheetTypeEnum.LabelAppointment:
					return GetLabelAppointment(outInCheck);
				case SheetTypeEnum.Rx:
					return GetRx(outInCheck);
				case SheetTypeEnum.Consent:
					return GetConsent(outInCheck);
				case SheetTypeEnum.PatientLetter:
					return GetPatientLetter(outInCheck);
				case SheetTypeEnum.ReferralLetter:
					return GetReferralLetter(outInCheck);
				case SheetTypeEnum.PatientForm:
					return GetPatientForm(outInCheck);
				case SheetTypeEnum.RoutingSlip:
					return GetRoutingSlip(outInCheck);
				case SheetTypeEnum.MedicalHistory:
					return GetMedicalHistory(outInCheck);
				case SheetTypeEnum.LabSlip:
					return GetLabSlip(outInCheck);
				case SheetTypeEnum.ExamSheet:
					return GetExamSheet(outInCheck);
				case SheetTypeEnum.DepositSlip:
					return GetDepositSlip(outInCheck);
				case SheetTypeEnum.Statement:
					return GetStatement(outInCheck);
				case SheetTypeEnum.MedLabResults:
					return GetMedLabResults(outInCheck);
				case SheetTypeEnum.TreatmentPlan:
					return GetTreatmentPlans(outInCheck);
				case SheetTypeEnum.Screening:
					return GetScreening(outInCheck);
				case SheetTypeEnum.PaymentPlan:
					return GetPaymentPlans(outInCheck);
				case SheetTypeEnum.RxMulti:
					return GetRxMulti(outInCheck);
				case SheetTypeEnum.ERA:
					return GetEra(outInCheck);
				case SheetTypeEnum.ERAGridHeader:
					return GetEraGridHeader(outInCheck);
				case SheetTypeEnum.RxInstruction:
					return GetRxInstruction(outInCheck);
			}
			return new List<SheetFieldDef>();
		}

		///<summary>For a given fieldName, return all the allowed radioButtonValues.  Will frequently be an empty list if a checkbox with this fieldname is not allowed to act as a radiobutton.</summary>
		public static List<string> GetRadio(string fieldName) {
			List<string> retVal=new List<string>();
			string[] stringAr=null;
			switch(fieldName) {
				default:
					return retVal;
				case "Gender":
					stringAr=Enum.GetNames(typeof(PatientGender));
					break;
				case "ins1Relat":
				case "ins2Relat":
					stringAr=Enum.GetNames(typeof(Relat));
					break;
				case "Position":
					stringAr=Enum.GetNames(typeof(PatientPosition));
					break;
				case "PreferContactMethod":
				case "PreferConfirmMethod":
				case "PreferRecallMethod":
					stringAr=Enum.GetNames(typeof(ContactMethod));
					break;
				case "StudentStatus":
					retVal.Add("Nonstudent");
					retVal.Add("Parttime");
					retVal.Add("Fulltime");
					return retVal;
				case "Race":
					//Sheets use PatientRaceOld for display purposes only.  They are not imported.  Therefore, the patientrace table does not need to be used here.
					stringAr=Enum.GetNames(typeof(PatientRaceOld));
					break;
			}
			for(int i=0;i<stringAr.Length;i++) {
				retVal.Add(stringAr[i]);
			}
			return retVal;
		}

		private static SheetFieldDef NewOutput(string fieldName) {
			return SheetFieldDef.NewOutput(fieldName,0,"",false,0,0,0,0,GrowthBehaviorEnum.None);
		}

		private static SheetFieldDef NewInput(string fieldName) {
			return SheetFieldDef.NewInput(fieldName,0,"",false,0,0,0,0);
		}

		private static SheetFieldDef NewCheck(string fieldName) {
			return SheetFieldDef.NewCheckBox(fieldName,0,0,0,0);
		}

		private static SheetFieldDef NewRadio(string fieldName,string radioButtonValue) {
			return SheetFieldDef.NewRadioButton(fieldName,radioButtonValue,0,0,0,0);
		}

		private static SheetFieldDef NewSpecial(string fieldName) {
			return SheetFieldDef.NewSpecial(fieldName,0,0,0,0);

		}

		private static List<SheetFieldDef> GetLabelPatient(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("nameFL"));
				list.Add(NewOutput("nameLF"));
				list.Add(NewOutput("address")); //includes address2
				list.Add(NewOutput("cityStateZip"));
				list.Add(NewOutput("ChartNumber"));
				list.Add(NewOutput("PatNum"));
				list.Add(NewOutput("dateTime.Today"));
				list.Add(NewOutput("birthdate"));
				list.Add(NewOutput("priProvName"));
			}
			return list;
		}

		private static List<SheetFieldDef> GetLabelCarrier(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("CarrierName"));
				list.Add(NewOutput("address")); //includes address2
				list.Add(NewOutput("cityStateZip"));
			}
			return list;
		}

		private static List<SheetFieldDef> GetLabelReferral(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("nameFL")); //includes Title
				list.Add(NewOutput("address")); //includes address2
				list.Add(NewOutput("cityStateZip"));
			}
			return list;
		}

		private static List<SheetFieldDef> GetReferralSlip(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("referral.nameFL"));
				list.Add(NewOutput("referral.address"));
				list.Add(NewOutput("referral.cityStateZip"));
				list.Add(NewOutput("referral.phone"));
				list.Add(NewOutput("referral.phone2"));
				list.Add(NewOutput("patient.nameFL"));
				list.Add(NewOutput("dateTime.Today"));
				list.Add(NewOutput("patient.WkPhone"));
				list.Add(NewOutput("patient.HmPhone"));
				list.Add(NewOutput("patient.WirelessPhone"));
				list.Add(NewOutput("patient.address"));
				list.Add(NewOutput("patient.cityStateZip"));
				list.Add(NewOutput("patient.provider"));
			}
			else if(outInCheck==OutInCheck.In) {
				list.Add(NewInput("notes"));
			}
			else if(outInCheck==OutInCheck.Check) {
				list.Add(NewCheck("misc"));
			}
			return list;
		}

		private static List<SheetFieldDef> GetLabelAppointment(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("nameFL"));
				list.Add(NewOutput("nameLF"));
				list.Add(NewOutput("weekdayDateTime"));
				list.Add(NewOutput("length"));
			}
			return list;
		}

		private static List<SheetFieldDef> GetRx(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("prov.nameFL"));
				list.Add(NewOutput("clinic.address"));
				list.Add(NewOutput("clinic.cityStateZip"));
				list.Add(NewOutput("clinic.phone"));
				list.Add(NewOutput("RxDate"));
				list.Add(NewOutput("RxDateMonthSpelled"));
				list.Add(NewOutput("prov.dEANum"));
				list.Add(NewOutput("pat.nameFL"));
				list.Add(NewOutput("pat.Birthdate"));
				list.Add(NewOutput("pat.HmPhone"));
				list.Add(NewOutput("pat.address"));
				list.Add(NewOutput("pat.cityStateZip"));
				list.Add(NewOutput("Drug"));
				list.Add(NewOutput("Disp"));
				list.Add(NewOutput("Sig"));
				list.Add(NewOutput("Refills"));
				list.Add(NewOutput("prov.stateRxID"));
				list.Add(NewOutput("prov.StateLicense"));
				list.Add(NewOutput("prov.NationalProvID"));
				list.Add(NewOutput("ProcCode"));
				list.Add(NewOutput("DaysOfSupply"));
			}
			else if(outInCheck==OutInCheck.In) {
				list.Add(NewInput("notes"));
			}
			return list;
		}

		private static List<SheetFieldDef> GetConsent(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("dateTime.Today"));
				list.Add(NewOutput("patient.nameFL"));
			}
			else if(outInCheck==OutInCheck.In) {
				list.Add(NewInput("toothNum"));
				list.Add(NewInput("misc"));
			}
			else if(outInCheck==OutInCheck.Check) {
				list.Add(NewCheck("misc"));
			}
			return list;
		}

		private static List<SheetFieldDef> GetPatientLetter(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("PracticeTitle"));
				list.Add(NewOutput("PracticeAddress"));
				list.Add(NewOutput("practiceCityStateZip"));
				list.Add(NewOutput("patient.nameFL"));
				list.Add(NewOutput("patient.address"));
				list.Add(NewOutput("patient.cityStateZip"));
				list.Add(NewOutput("today.DayDate"));
				list.Add(NewOutput("patient.salutation"));
				list.Add(NewOutput("patient.priProvNameFL"));
			}
			else if(outInCheck==OutInCheck.In) {
				//none
			}
			return list;
		}

		private static List<SheetFieldDef> GetRxInstruction(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput(Today.DayDate));
				//Drug items
				list.Add(NewOutput(RxPat.Drug));
				list.Add(NewOutput(RxPat.Sig));
				list.Add(NewOutput(RxPat.PatientInstruction));				
				list.Add(NewOutput(RxPat.Disp));
				list.Add(NewOutput(RxPat.Refills));				
				list.Add(NewOutput(Rx.DaysOfSupply));				
				list.Add(NewOutput(Rx.RxDate));
				list.Add(NewOutput(Rx.RxDateMonthSpelled));
				//Patient items				
				list.Add(NewOutput(Patient.NameFL));				
				list.Add(NewOutput(Patient.Salutation));
				list.Add(NewOutput(Patient.Address));
				list.Add(NewOutput(Patient.CityStateZip));	
				list.Add(NewOutput(Patient.HmPhone));
				list.Add(NewOutput(Patient.Birthdate));
				list.Add(NewOutput(Patient.PriProvNameFL));
				//Provider items
				list.Add(NewOutput(Prov.NameFL));				
				list.Add(NewOutput(Prov.StateRxID));
				list.Add(NewOutput(Prov.StateLicense));
				list.Add(NewOutput(Prov.DEANum));
				list.Add(NewOutput(Prov.NationalProvID));
				//clinic and practice items.
				list.Add(NewOutput(Clinic.Address));
				list.Add(NewOutput(Clinic.CityStateZip));
				list.Add(NewOutput(Clinic.Phone));
				list.Add(NewOutput(Practice.Title));
				list.Add(NewOutput(Practice.Address));
				list.Add(NewOutput(Practice.CityStateZip));				
				list.Add(NewOutput(Practice.Phone));				
			}
			return list;
		}

		private static List<SheetFieldDef> GetReferralLetter(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("PracticeTitle"));
				list.Add(NewOutput("PracticeAddress"));
				list.Add(NewOutput("PracticePhoneNumber"));
				list.Add(NewOutput("practiceCityStateZip"));
				list.Add(NewOutput("referral.phone"));
				list.Add(NewOutput("referral.phone2"));
				list.Add(NewOutput("referral.nameFL"));
				list.Add(NewOutput("referral.nameL"));
				list.Add(NewOutput("referral.address"));
				list.Add(NewOutput("referral.cityStateZip"));
				list.Add(NewOutput("today.DayDate"));
				list.Add(NewOutput("patient.nameFL"));
				list.Add(NewOutput("referral.salutation"));
				list.Add(NewOutput("patient.priProvNameFL"));
				list.Add(NewOutput("patient.Birthdate"));
			}
			else if(outInCheck==OutInCheck.In) {
				//none
			}
			else if(outInCheck==OutInCheck.Check) {
				list.Add(NewCheck("misc"));
			}
			return list;
		}

		private static List<SheetFieldDef> GetPatientForm(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				//I can't really think of any for this kind				
			}
			else if(outInCheck==OutInCheck.In) {
				list.Add(NewInput("Address"));
				list.Add(NewInput("Address2"));
				list.Add(NewInput("Birthdate"));
				list.Add(NewInput("City"));
				list.Add(NewInput("Email"));
				list.Add(NewInput("FName"));
				list.Add(NewInput("HmPhone"));
				list.Add(NewInput("ICEName"));
				list.Add(NewInput("ICEPhone"));
				list.Add(NewInput("ins1CarrierName"));
				list.Add(NewInput("ins1CarrierPhone"));
				list.Add(NewInput("ins1EmployerName"));
				list.Add(NewInput("ins1GroupName"));
				list.Add(NewInput("ins1GroupNum"));
				list.Add(NewInput("ins1SubscriberID"));
				list.Add(NewInput("ins1SubscriberNameF"));
				list.Add(NewInput("ins2CarrierName"));
				list.Add(NewInput("ins2CarrierPhone"));
				list.Add(NewInput("ins2EmployerName"));
				list.Add(NewInput("ins2GroupName"));
				list.Add(NewInput("ins2GroupNum"));
				list.Add(NewInput("ins2SubscriberID"));
				list.Add(NewInput("ins2SubscriberNameF"));
				list.Add(NewInput("LName"));
				list.Add(NewInput("MiddleI"));
				list.Add(NewInput("misc"));
				list.Add(NewInput("Preferred"));
				list.Add(NewInput("referredFrom"));
				list.Add(NewInput("SSN"));
				list.Add(NewInput("State"));
				list.Add(NewInput("WkPhone"));
				list.Add(NewInput("WirelessPhone"));
				list.Add(NewInput("wirelessCarrier"));
				list.Add(NewInput("Zip"));
			}
			else if(outInCheck==OutInCheck.Check) {
				list.Add(NewCheck("addressAndHmPhoneIsSameEntireFamily"));
				list.Add(NewCheck("Gender"));
				list.Add(NewCheck("ins1Relat"));
				list.Add(NewCheck("ins2Relat"));
				list.Add(NewCheck("misc"));
				list.Add(NewCheck("Position"));
				list.Add(NewCheck("PreferConfirmMethod"));
				list.Add(NewCheck("PreferContactMethod"));
				list.Add(NewCheck("PreferRecallMethod"));
				list.Add(NewCheck("StudentStatus"));
			}
			return list;
		}

		private static List<SheetFieldDef> GetRoutingSlip(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("appt.timeDate"));
				list.Add(NewOutput("appt.length"));
				list.Add(NewOutput("appt.providers"));
				list.Add(NewOutput("appt.procedures"));
				list.Add(NewOutput("appt.Note"));
				list.Add(NewOutput("appt.estPatientPortion"));
				list.Add(NewOutput("otherFamilyMembers"));
				list.Add(NewOutput("labName"));
				list.Add(NewOutput("dateLabSent"));
				list.Add(NewOutput("dateLabReceived"));
				list.Add(NewOutput("referral.FLName"));
				list.Add(NewOutput("referral.LName"));
				list.Add(NewOutput("referral.address"));
				list.Add(NewOutput("referral.cityStateZip"));
				//most fields turned out to work best as static text.
			}
			else if(outInCheck==OutInCheck.In) {
				//Not applicable
			}
			else if(outInCheck==OutInCheck.Check) {
				//Not applicable
			}
			return list;
		}

		private static List<SheetFieldDef> GetMedicalHistory(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				//none
			}
			else if(outInCheck==OutInCheck.In) {
				list.Add(NewInput("Birthdate"));
				list.Add(NewInput("FName"));
				list.Add(NewInput("LName"));
				list.Add(NewInput("misc"));
				list.Add(NewInput("ICEName"));
				list.Add(NewInput("ICEPhone"));
				list.Add(NewInput("inputMed1"));
				list.Add(NewInput("inputMed2"));
				list.Add(NewInput("inputMed3"));
				list.Add(NewInput("inputMed4"));
				list.Add(NewInput("inputMed5"));
				list.Add(NewInput("inputMed6"));
				list.Add(NewInput("inputMed7"));
				list.Add(NewInput("inputMed8"));
				list.Add(NewInput("inputMed9"));
				list.Add(NewInput("inputMed10"));
				list.Add(NewInput("inputMed11"));
				list.Add(NewInput("inputMed12"));
				list.Add(NewInput("inputMed13"));
				list.Add(NewInput("inputMed14"));
				list.Add(NewInput("inputMed15"));
				list.Add(NewInput("inputMed16"));
				list.Add(NewInput("inputMed17"));
				list.Add(NewInput("inputMed18"));
				list.Add(NewInput("inputMed19"));
				list.Add(NewInput("inputMed20"));
			}
			else if(outInCheck==OutInCheck.Check) {
				list.Add(NewCheck("allergy"));
				list.Add(NewCheck("problem"));
				list.Add(NewCheck("misc"));
				list.Add(NewInput("checkMed1"));
				list.Add(NewInput("checkMed2"));
				list.Add(NewInput("checkMed3"));
				list.Add(NewInput("checkMed4"));
				list.Add(NewInput("checkMed5"));
				list.Add(NewInput("checkMed6"));
				list.Add(NewInput("checkMed7"));
				list.Add(NewInput("checkMed8"));
				list.Add(NewInput("checkMed9"));
				list.Add(NewInput("checkMed10"));
				list.Add(NewInput("checkMed11"));
				list.Add(NewInput("checkMed12"));
				list.Add(NewInput("checkMed13"));
				list.Add(NewInput("checkMed14"));
				list.Add(NewInput("checkMed15"));
				list.Add(NewInput("checkMed16"));
				list.Add(NewInput("checkMed17"));
				list.Add(NewInput("checkMed18"));
				list.Add(NewInput("checkMed19"));
				list.Add(NewInput("checkMed20"));
			}
			return list;
		}

		private static List<SheetFieldDef> GetLabSlip(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("lab.Description"));
				list.Add(NewOutput("lab.Phone"));
				list.Add(NewOutput("lab.Notes"));
				list.Add(NewOutput("lab.WirelessPhone"));
				list.Add(NewOutput("lab.Address"));
				list.Add(NewOutput("lab.CityStZip"));
				list.Add(NewOutput("lab.Email"));
				list.Add(NewOutput("appt.DateTime"));
				list.Add(NewOutput("labcase.DateTimeDue"));
				list.Add(NewOutput("labcase.DateTimeCreated"));
				list.Add(NewOutput("prov.nameFormal"));
				list.Add(NewOutput("prov.stateLicence"));
				//patient fields already handled with static text: name,age,gender.
				//other fields already handled: dateToday, practice address and phone.
			}
			else if(outInCheck==OutInCheck.In) {
				list.Add(NewInput("notes"));
				list.Add(NewInput("labcase.Instructions"));
				list.Add(NewInput("misc"));
			}
			else if(outInCheck==OutInCheck.Check) {
				list.Add(NewCheck("misc"));
			}
			return list;
		}

		public static List<SheetFieldDef> GetExamSheet(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("patient.priProvNameFL"));
				list.Add(NewOutput("sheet.DateTimeSheet"));
			}
			else if(outInCheck==OutInCheck.In) {
				list.Add(NewInput("Birthdate"));
				list.Add(NewInput("FName"));
				list.Add(NewInput("LName"));
				list.Add(NewInput("MiddleI"));
				list.Add(NewInput("misc"));
				list.Add(NewInput("Preferred"));
			}
			else if(outInCheck==OutInCheck.Check) {
				list.Add(NewCheck("Gender"));
				list.Add(NewCheck("misc"));
				list.Add(NewCheck("Race")); //This is really race/ethnicity combined.
			}
			return list;
		}

		public static List<SheetFieldDef> GetDepositSlip(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("cashSumTotal"));
				list.Add(NewOutput("checkNumber01"));
				list.Add(NewOutput("checkNumber02"));
				list.Add(NewOutput("checkNumber03"));
				list.Add(NewOutput("checkNumber04"));
				list.Add(NewOutput("checkNumber05"));
				list.Add(NewOutput("checkNumber06"));
				list.Add(NewOutput("checkNumber07"));
				list.Add(NewOutput("checkNumber08"));
				list.Add(NewOutput("checkNumber09"));
				list.Add(NewOutput("checkNumber10"));
				list.Add(NewOutput("checkNumber11"));
				list.Add(NewOutput("checkNumber12"));
				list.Add(NewOutput("checkNumber13"));
				list.Add(NewOutput("checkNumber14"));
				list.Add(NewOutput("checkNumber15"));
				list.Add(NewOutput("checkNumber16"));
				list.Add(NewOutput("checkNumber17"));
				list.Add(NewOutput("checkNumber18"));
				list.Add(NewOutput("deposit.BankAccountInfo"));
				list.Add(NewOutput("deposit.DateDeposit"));
				list.Add(NewOutput("depositList"));
				list.Add(NewOutput("depositTotal"));
				list.Add(NewOutput("depositItemCount"));
				list.Add(NewOutput("depositItem01"));
				list.Add(NewOutput("depositItem02"));
				list.Add(NewOutput("depositItem03"));
				list.Add(NewOutput("depositItem04"));
				list.Add(NewOutput("depositItem05"));
				list.Add(NewOutput("depositItem06"));
				list.Add(NewOutput("depositItem07"));
				list.Add(NewOutput("depositItem08"));
				list.Add(NewOutput("depositItem09"));
				list.Add(NewOutput("depositItem10"));
				list.Add(NewOutput("depositItem11"));
				list.Add(NewOutput("depositItem12"));
				list.Add(NewOutput("depositItem13"));
				list.Add(NewOutput("depositItem14"));
				list.Add(NewOutput("depositItem15"));
				list.Add(NewOutput("depositItem16"));
				list.Add(NewOutput("depositItem17"));
				list.Add(NewOutput("depositItem18"));
			}
			else if(outInCheck==OutInCheck.In) {
			}
			else if(outInCheck==OutInCheck.Check) {
			}
			return list;
		}

		private static List<SheetFieldDef> GetStatement(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("accountNumber"));
				list.Add(NewOutput("statement.NoteBold"));
				list.Add(NewOutput("statement.Note"));
				list.Add(NewOutput("futureAppointments"));
				list.Add(NewOutput("totalLabel"));
				list.Add(NewOutput("totalValue"));
				list.Add(NewOutput("insEstLabel"));
				list.Add(NewOutput("insEstValue"));
				list.Add(NewOutput("balanceLabel"));
				list.Add(NewOutput("balanceValue"));
				list.Add(NewOutput("amountDueValue"));
				list.Add(NewOutput("invoicePaymentLabel"));	//only for invoices
				list.Add(NewOutput("invoicePaymentValue")); //only for invoices
				list.Add(NewOutput("invoiceTotalLabel")); //only for invoices
				list.Add(NewOutput("invoiceTotalValue")); //only for invoices
				list.Add(NewOutput("invoicePayPlanLabel")); //only for invoices
				list.Add(NewOutput("invoicePayPlanValue")); //only for invoices
				list.Add(NewOutput("payPlanAmtDueValue"));
				list.Add(NewOutput("statementReceiptInvoice"));
				list.Add(NewOutput("returnAddress"));
				list.Add(NewOutput("billingAddress"));
				list.Add(NewOutput("statement.DateSent"));
				list.Add(NewOutput("statementIsCopy"));
				list.Add(NewOutput("statementIsTaxReceipt"));
				list.Add(NewOutput("providerLegend"));
				list.Add(NewOutput("statementURL"));
				list.Add(NewOutput("statementShortURL"));
				list.Add(NewOutput("StatementNum"));
			}
			else if(outInCheck==OutInCheck.In) {
			}
			else if(outInCheck==OutInCheck.Check) {
			}
			return list;
		}

		private static List<SheetFieldDef> GetMedLabResults(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("medlab.ClinicalInfo"));
				list.Add(NewOutput("medlab.dateEntered"));
				list.Add(NewOutput("medlab.DateTimeCollected"));
				list.Add(NewOutput("medlab.DateTimeReported"));
				list.Add(NewOutput("medlab.NoteLab"));
				list.Add(NewOutput("medlab.obsTests"));
				list.Add(NewOutput("medlab.ProvID"));
				list.Add(NewOutput("medlab.provNameLF"));
				list.Add(NewOutput("medlab.ProvNPI"));
				list.Add(NewOutput("medlab.PatAccountNum"));
				list.Add(NewOutput("medlab.PatAge"));
				list.Add(NewOutput("medlab.PatFasting"));
				list.Add(NewOutput("medlab.PatIDAlt"));
				list.Add(NewOutput("medlab.PatIDLab"));
				list.Add(NewOutput("medlab.SpecimenID"));
				list.Add(NewOutput("medlab.SpecimenIDAlt"));
				list.Add(NewOutput("medlab.TotalVolume"));
				list.Add(NewOutput("medLabFacilityAddr"));
				list.Add(NewOutput("medLabFacilityDir"));
				list.Add(NewOutput("patient.addrCityStZip"));
				list.Add(NewOutput("patient.Birthdate"));
				list.Add(NewOutput("patient.FName"));
				list.Add(NewOutput("patient.Gender"));
				list.Add(NewOutput("patient.HmPhone"));
				list.Add(NewOutput("patient.MiddleI"));
				list.Add(NewOutput("patient.LName"));
				list.Add(NewOutput("patient.PatNum"));
				list.Add(NewOutput("patient.SSN"));
				list.Add(NewOutput("practiceAddrCityStZip"));
				list.Add(NewOutput("PracticePh"));
				list.Add(NewOutput("PracticeTitle"));
			}
			else if(outInCheck==OutInCheck.In) {
			}
			else if(outInCheck==OutInCheck.Check) {
			}
			return list;
		}

		private static List<SheetFieldDef> GetTreatmentPlans(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("Heading"));
				list.Add(NewOutput("defaultHeading"));
				list.Add(NewOutput("Note"));
				list.Add(NewOutput("tpPatPortionEst"));
				list.Add(NewOutput("SignatureText"));
				list.Add(NewOutput("SignaturePracticeText"));
				list.Add(NewOutput("DateTSigned"));
				list.Add(NewOutput("DateTPracticeSigned"));
			}
			else if(outInCheck==OutInCheck.In) {
			}
			else if(outInCheck==OutInCheck.Check) {
			}
			return list;
		}

		private static List<SheetFieldDef> GetScreening(OutInCheck outInCheck) {
			List<SheetFieldDef> listSheetFieldDefs=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				listSheetFieldDefs.Add(NewInput("Description"));
				listSheetFieldDefs.Add(NewInput("DateScreenGroup"));
				listSheetFieldDefs.Add(NewInput("ProvName"));
				listSheetFieldDefs.Add(NewInput("PlaceOfService"));
				listSheetFieldDefs.Add(NewInput("County"));
				listSheetFieldDefs.Add(NewInput("GradeSchool"));
			}
			else if(outInCheck==OutInCheck.In) {
				listSheetFieldDefs.Add(NewInput("Birthdate"));
				listSheetFieldDefs.Add(NewInput("FName"));
				listSheetFieldDefs.Add(NewInput("LName"));
				listSheetFieldDefs.Add(NewInput("MiddleI"));
				listSheetFieldDefs.Add(NewInput("Preferred"));
				listSheetFieldDefs.Add(NewInput("GradeLevel"));
				listSheetFieldDefs.Add(NewInput("Race/Ethnicity"));
				listSheetFieldDefs.Add(NewInput("Urgency"));
				listSheetFieldDefs.Add(NewInput("Comments"));
				listSheetFieldDefs.Add(NewInput("misc"));
			}
			else if(outInCheck==OutInCheck.Check) {
				listSheetFieldDefs.Add(NewCheck("HasCaries"));
				listSheetFieldDefs.Add(NewCheck("EarlyChildCaries"));
				listSheetFieldDefs.Add(NewCheck("CariesExperience"));
				listSheetFieldDefs.Add(NewCheck("ExistingSealants"));
				listSheetFieldDefs.Add(NewCheck("NeedsSealants"));
				listSheetFieldDefs.Add(NewCheck("MissingAllTeeth"));
				listSheetFieldDefs.Add(NewCheck("Gender"));
				listSheetFieldDefs.Add(NewCheck("misc"));
			}
			return listSheetFieldDefs;
		}

		private static List<SheetFieldDef> GetPaymentPlans(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("PracticeTitle"));
				list.Add(NewOutput("dateToday"));
				list.Add(NewOutput("nameLF"));
				list.Add(NewOutput("guarantor"));
				list.Add(NewOutput("Principal"));
				list.Add(NewOutput("DateOfAgreement"));
				list.Add(NewOutput("APR"));
				list.Add(NewOutput("totalFinanceCharge"));
				list.Add(NewOutput("totalCostOfLoan"));
				list.Add(NewOutput("Note"));
				list.Add(NewOutput("ccNumberMaskedWithExp"));
			}
			else if(outInCheck==OutInCheck.In) {
			}
			else if(outInCheck==OutInCheck.Check) {
			}
			return list;
		}

		private static List<SheetFieldDef> GetRxMulti(OutInCheck outInCheck) {
			List<SheetFieldDef> list=new List<SheetFieldDef>();
			if(outInCheck==OutInCheck.Out) {
				list.Add(NewOutput("prov.nameFL"));
				list.Add(NewOutput("prov.nameFL2"));
				list.Add(NewOutput("prov.nameFL3"));
				list.Add(NewOutput("prov.nameFL4"));
				list.Add(NewOutput("prov.nameFL5"));
				list.Add(NewOutput("prov.nameFL6"));
				list.Add(NewOutput("clinic.address"));
				list.Add(NewOutput("clinic.address2"));
				list.Add(NewOutput("clinic.address3"));
				list.Add(NewOutput("clinic.address4"));
				list.Add(NewOutput("clinic.address5"));
				list.Add(NewOutput("clinic.address6"));
				list.Add(NewOutput("clinic.cityStateZip"));
				list.Add(NewOutput("clinic.cityStateZip2"));
				list.Add(NewOutput("clinic.cityStateZip3"));
				list.Add(NewOutput("clinic.cityStateZip4"));
				list.Add(NewOutput("clinic.cityStateZip5"));
				list.Add(NewOutput("clinic.cityStateZip6"));
				list.Add(NewOutput("clinic.phone"));
				list.Add(NewOutput("clinic.phone2"));
				list.Add(NewOutput("clinic.phone3"));
				list.Add(NewOutput("clinic.phone4"));
				list.Add(NewOutput("clinic.phone5"));
				list.Add(NewOutput("clinic.phone6"));
				list.Add(NewOutput("prov.stateRxID"));
				list.Add(NewOutput("prov.stateRxID2"));
				list.Add(NewOutput("prov.stateRxID3"));
				list.Add(NewOutput("prov.stateRxID4"));
				list.Add(NewOutput("prov.stateRxID5"));
				list.Add(NewOutput("prov.stateRxID6"));
				list.Add(NewOutput("prov.StateLicense"));
				list.Add(NewOutput("prov.StateLicense2"));
				list.Add(NewOutput("prov.StateLicense3"));
				list.Add(NewOutput("prov.StateLicense4"));
				list.Add(NewOutput("prov.StateLicense5"));
				list.Add(NewOutput("prov.StateLicense6"));
				list.Add(NewOutput("prov.NationalProvID"));
				list.Add(NewOutput("prov.NationalProvID2"));
				list.Add(NewOutput("prov.NationalProvID3"));
				list.Add(NewOutput("prov.NationalProvID4"));
				list.Add(NewOutput("prov.NationalProvID5"));
				list.Add(NewOutput("prov.NationalProvID6"));
				list.Add(NewOutput("prov.dEANum"));
				list.Add(NewOutput("prov.dEANum2"));
				list.Add(NewOutput("prov.dEANum3"));
				list.Add(NewOutput("prov.dEANum4"));
				list.Add(NewOutput("prov.dEANum5"));
				list.Add(NewOutput("prov.dEANum6"));
				list.Add(NewOutput("RxDate"));
				list.Add(NewOutput("RxDate2"));
				list.Add(NewOutput("RxDate3"));
				list.Add(NewOutput("RxDate4"));
				list.Add(NewOutput("RxDate5"));
				list.Add(NewOutput("RxDate6"));
				list.Add(NewOutput("RxDateMonthSpelled"));
				list.Add(NewOutput("RxDateMonthSpelled2"));
				list.Add(NewOutput("RxDateMonthSpelled3"));
				list.Add(NewOutput("RxDateMonthSpelled4"));
				list.Add(NewOutput("RxDateMonthSpelled5"));
				list.Add(NewOutput("RxDateMonthSpelled6"));
				list.Add(NewOutput("pat.nameFL"));
				list.Add(NewOutput("pat.nameFL2"));
				list.Add(NewOutput("pat.nameFL3"));
				list.Add(NewOutput("pat.nameFL4"));
				list.Add(NewOutput("pat.nameFL5"));
				list.Add(NewOutput("pat.nameFL6"));
				list.Add(NewOutput("pat.Birthdate"));
				list.Add(NewOutput("pat.Birthdate2"));
				list.Add(NewOutput("pat.Birthdate3"));
				list.Add(NewOutput("pat.Birthdate4"));
				list.Add(NewOutput("pat.Birthdate5"));
				list.Add(NewOutput("pat.Birthdate6"));
				list.Add(NewOutput("pat.HmPhone"));
				list.Add(NewOutput("pat.HmPhone2"));
				list.Add(NewOutput("pat.HmPhone3"));
				list.Add(NewOutput("pat.HmPhone4"));
				list.Add(NewOutput("pat.HmPhone5"));
				list.Add(NewOutput("pat.HmPhone6"));
				list.Add(NewOutput("pat.address"));
				list.Add(NewOutput("pat.address2"));
				list.Add(NewOutput("pat.address3"));
				list.Add(NewOutput("pat.address4"));
				list.Add(NewOutput("pat.address5"));
				list.Add(NewOutput("pat.address6"));
				list.Add(NewOutput("pat.cityStateZip"));
				list.Add(NewOutput("pat.cityStateZip2"));
				list.Add(NewOutput("pat.cityStateZip3"));
				list.Add(NewOutput("pat.cityStateZip4"));
				list.Add(NewOutput("pat.cityStateZip5"));
				list.Add(NewOutput("pat.cityStateZip6"));
				list.Add(NewOutput("Drug"));
				list.Add(NewOutput("Drug2"));
				list.Add(NewOutput("Drug3"));
				list.Add(NewOutput("Drug4"));
				list.Add(NewOutput("Drug5"));
				list.Add(NewOutput("Drug6"));
				list.Add(NewOutput("Disp"));
				list.Add(NewOutput("Disp2"));
				list.Add(NewOutput("Disp3"));
				list.Add(NewOutput("Disp4"));
				list.Add(NewOutput("Disp5"));
				list.Add(NewOutput("Disp6"));
				list.Add(NewOutput("Sig"));
				list.Add(NewOutput("Sig2"));
				list.Add(NewOutput("Sig3"));
				list.Add(NewOutput("Sig4"));
				list.Add(NewOutput("Sig5"));
				list.Add(NewOutput("Sig6"));
				list.Add(NewOutput("Refills"));
				list.Add(NewOutput("Refills2"));
				list.Add(NewOutput("Refills3"));
				list.Add(NewOutput("Refills4"));
				list.Add(NewOutput("Refills5"));
				list.Add(NewOutput("Refills6"));
				list.Add(NewOutput("ProcCode"));
				list.Add(NewOutput("ProcCode2"));
				list.Add(NewOutput("ProcCode3"));
				list.Add(NewOutput("ProcCode4"));
				list.Add(NewOutput("ProcCode5"));
				list.Add(NewOutput("ProcCode6"));
				list.Add(NewOutput("DaysOfSupply"));
				list.Add(NewOutput("DaysOfSupply2"));
				list.Add(NewOutput("DaysOfSupply3"));
				list.Add(NewOutput("DaysOfSupply4"));
				list.Add(NewOutput("DaysOfSupply5"));
				list.Add(NewOutput("DaysOfSupply6"));
			}
			else if(outInCheck==OutInCheck.In) {
				list.Add(NewInput("notes"));
			}
			return list;
		}

		private static List<SheetFieldDef> GetEra(OutInCheck outInCheck) {
			List<SheetFieldDef> retList=new List<SheetFieldDef>();
			switch(outInCheck) {
				case OutInCheck.Out:
					retList.Add(NewOutput("PayerName"));
					retList.Add(NewOutput("PayerID"));
					retList.Add(NewOutput("PayerAddress"));
					retList.Add(NewOutput("PayerCity"));
					retList.Add(NewOutput("PayerState"));
					retList.Add(NewOutput("PayerZip"));
					retList.Add(NewOutput("PayerContactInfo"));
					retList.Add(NewOutput("PayeeName"));
					retList.Add(NewOutput("PayeeId"));
					retList.Add(NewOutput("TransHandlingDesc"));
					retList.Add(NewOutput("PaymentMethod"));
					retList.Add(NewOutput("AcctNumEndingIn"));
					retList.Add(NewOutput("Check#"));
					retList.Add(NewOutput("DateEffective"));
					retList.Add(NewOutput("InsPaid"));
					break;
				case OutInCheck.In:
					//none
					break;
				case OutInCheck.Check:
					//none
					break;
			}
			return retList;
		}
		
		private static List<SheetFieldDef> GetEraGridHeader(OutInCheck outInCheck) {
			List<SheetFieldDef> retList=new List<SheetFieldDef>();
			switch(outInCheck) {
				case OutInCheck.Out:
					retList.Add(NewOutput("Subscriber"));
					retList.Add(NewOutput("Patient"));
					retList.Add(NewOutput("ClaimIdentifier"));
					retList.Add(NewOutput("PayorControlNum"));
					retList.Add(NewOutput("Status"));
					retList.Add(NewOutput("DateService"));
					retList.Add(NewOutput("ClaimFee"));
					retList.Add(NewOutput("InsPaid"));
					retList.Add(NewOutput("PatientResponsibility"));
					retList.Add(NewOutput("DatePayerReceived"));
					retList.Add(NewOutput("ClaimIndexNum"));
					break;
				case OutInCheck.In:
					//none
					break;
				case OutInCheck.Check:
					//none
					break;
			}
			return retList;
		}

		#region SheetField String Classes

		public class Today {
			public const string DayDate="today.DayDate";
		}

		public class RxPat {
			public const string Drug="rxpat.Drug";
			public const string Sig="rxpat.Sig";
			public const string PatientInstruction="rxpat.PatientInstruction";
			public const string Disp="rxpat.Disp";
			public const string Refills="rxpat.Refills";
		}

		public class Rx {
			public const string DaysOfSupply="rx.DaysOfSupply";
			public const string RxDate="rx.RxDate";
			public const string RxDateMonthSpelled="rx.RxDateMonthSpelled";
		}

		public class Patient {
			public const string NameFL="patient.NameFL";
			public const string Salutation="patient.Salutation";
			public const string Address="patient.Address";
			public const string CityStateZip="patient.CityStateZip";
			public const string HmPhone="patient.HmPhone";
			public const string Birthdate="patient.Birthdate";
			public const string PriProvNameFL="patient.PriProvNameFL";
		}

		public class Prov {
			public const string NameFL="prov.NameFL";		
			public const string StateRxID="prov.StateRxID";
			public const string StateLicense="prov.StateLicense";
			public const string DEANum="prov.DEANum";
			public const string NationalProvID="prov.NationalProvID";
		}

		public class Clinic {
			public const string Address="clinic.Address";
			public const string CityStateZip="clinic.CityStateZip";
			public const string Phone="clinic.Phone";
		}

		public class Practice {
			public const string Title="practice.Title";
			public const string Address="practice.Address";
			public const string CityStateZip="practice.CityStateZip";
			public const string Phone="practice.Phone";
		}
		#endregion SheetField String Classes
	}
}
