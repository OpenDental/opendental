using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness;

namespace TestCanada {
	public class PatientTC {
		public static long PatNum1;
		public static long PatNum2;
		public static long PatNum3;
		public static long PatNum4;
		public static long PatNum5;
		public static long PatNum6;
		public static long PatNum7;
		public static long PatNum8;
		public static long PatNum9;

		public static string SetInitialPatients() {
			Patient pat;
			Patient oldPatient;
			InsPlan plan;
			InsSub sub;
			PatPlan patplan;
			pat=new Patient();
			pat.PatStatus=PatientStatus.Patient;
			pat.Position=PatientPosition.Single;
			pat.Gender=PatientGender.Female;
			pat.Birthdate=new DateTime(1960,04,12);
			pat.LName="Fête";
			pat.FName="Lisa";
			pat.MiddleI="Ç";
			pat.Address="124 - 1500 Rue";
			pat.City="Montréal";
			pat.State="QC";
			pat.Zip="H1C2D4";
			pat.Language="fr";
			pat.CanadianEligibilityCode=2;//disabled
			Patients.Insert(pat,false);
			PatNum1=pat.PatNum;
			oldPatient=pat.Copy();
			pat.Guarantor=pat.PatNum;
			Patients.Update(pat,oldPatient);
			//Extractions
			ProcTC.SetExtracted("23",new DateTime(1995,2,7),pat.PatNum);
			ProcTC.SetExtracted("26",new DateTime(1996,11,13),pat.PatNum);
			//Missing teeth
			plan=new InsPlan();
			plan.CarrierNum=CarrierTC.GetCarrierNumById("666666");
			plan.GroupNum="PLAN012";
			plan.DentaideCardSequence=3;
			plan.CanadianPlanFlag="";
			plan.ClaimFormNum=7;//Canadian claim form
			InsPlans.Insert(plan);
			sub=new InsSub();
			sub.PlanNum=plan.PlanNum;
			sub.Subscriber=pat.PatNum;
			sub.SubscriberID="AB123C4G";
			InsSubs.Insert(sub);
			patplan=new PatPlan();
			patplan.PatNum=pat.PatNum;
			patplan.InsSubNum=sub.InsSubNum;
			patplan.Relationship=Relat.Self;//1
			patplan.PatID="00";
			patplan.Ordinal=1;
			PatPlans.Insert(patplan);
			//PATIENT 2==================================================================
			pat=new Patient();
			pat.PatStatus=PatientStatus.Patient;
			pat.Position=PatientPosition.Married;
			pat.Gender=PatientGender.Male;
			pat.Birthdate=new DateTime(1948,3,2);
			pat.LName="Smith";
			pat.FName="John";
			pat.MiddleI="";
			pat.CanadianEligibilityCode=4;//code not applicable
			pat.Address="P.O. Box 1500";
			pat.Address2="Little Field Estates";
			pat.City="East Westchester";
			pat.State="ON";
			pat.Zip="M7F2J9";
			pat.Language="en";
			Patients.Insert(pat,false);
			PatNum2=pat.PatNum;
			oldPatient=pat.Copy();
			pat.Guarantor=pat.PatNum;
			Patients.Update(pat,oldPatient);
			//plan1
			plan=new InsPlan();
			plan.CarrierNum=CarrierTC.GetCarrierNumById("666666");
			plan.GroupNum="PLAN02";
			plan.DivisionNo="1542B";
			plan.DentaideCardSequence=0;
			plan.CanadianPlanFlag="";
			plan.ClaimFormNum=7;//Canadian claim form
			InsPlans.Insert(plan);
			long planNum_pat2_pri=plan.PlanNum;
			sub=new InsSub();
			sub.PlanNum=plan.PlanNum;
			sub.Subscriber=pat.PatNum;
			sub.SubscriberID="123432145222";
			InsSubs.Insert(sub);
			long subNum_pat2_pri=sub.InsSubNum;
			patplan=new PatPlan();
			patplan.PatNum=pat.PatNum;
			patplan.InsSubNum=sub.InsSubNum;
			patplan.Relationship=Relat.Self;//1
			patplan.PatID="00";
			patplan.Ordinal=1;
			PatPlans.Insert(patplan);
			//plan2
			plan=new InsPlan();
			plan.CarrierNum=CarrierTC.GetCarrierNumById("777777");
			plan.GroupNum="P9902";
			plan.DivisionNo="";
			plan.DentaideCardSequence=0;
			plan.CanadianPlanFlag="";
			plan.ClaimFormNum=7;//Canadian claim form
			InsPlans.Insert(plan);
			//long planNum_pat2_sec=plan.PlanNum;//won't need this
			sub=new InsSub();
			sub.PlanNum=plan.PlanNum;
			sub.Subscriber=pat.PatNum;
			sub.SubscriberID="12343B7";
			InsSubs.Insert(sub);
			patplan=new PatPlan();
			patplan.PatNum=pat.PatNum;
			patplan.InsSubNum=sub.InsSubNum;
			patplan.Relationship=Relat.Self;//1
			patplan.PatID="00";
			patplan.Ordinal=2;
			PatPlans.Insert(patplan);
			//PATIENT 3=========================================================================
			//common law spouse of pat2.  Pri and sec coverage from spouse.  
			pat=new Patient();
			pat.PatStatus=PatientStatus.Patient;
			pat.Position=PatientPosition.Married;
			pat.Gender=PatientGender.Female;
			pat.Birthdate=new DateTime(1978,4,12);
			pat.LName="Walls";
			pat.FName="Mary";
			pat.MiddleI="A";
			pat.CanadianEligibilityCode=4;//code not applicable
			pat.Address="P.O. Box 1500";
			pat.Address2="Little Field Estates";
			pat.City="East Westchester";
			pat.State="ON";
			pat.Zip="M7F2J9";
			pat.Language="en";
			pat.Guarantor=PatNum2;//same family as patient #1.
			Patients.Insert(pat,false);
			PatNum3=pat.PatNum;
			//primary coverage------------------------------
			patplan=new PatPlan();
			patplan.PatNum=pat.PatNum;
			patplan.InsSubNum=subNum_pat2_pri;
			patplan.Relationship=Relat.Spouse;//2
			patplan.PatID="01";
			patplan.Ordinal=1;
			PatPlans.Insert(patplan);
			//secondary----------------------------------------
			plan=new InsPlan();
			plan.CarrierNum=CarrierTC.GetCarrierNumById("777777");
			plan.GroupNum="P9902";
			plan.DivisionNo="";
			plan.DentaideCardSequence=0;
			plan.CanadianPlanFlag="";
			plan.ClaimFormNum=7;//Canadian claim form
			InsPlans.Insert(plan);
			sub=new InsSub();
			sub.PlanNum=plan.PlanNum;
			sub.Subscriber=PatNum2;
			sub.SubscriberID="12343C7";//had to add this as separate plan because of unique subscriber id.
			InsSubs.Insert(sub);
			patplan=new PatPlan();
			patplan.PatNum=pat.PatNum;
			patplan.InsSubNum=sub.InsSubNum;
			patplan.Relationship=Relat.Spouse;//2
			patplan.PatID="00";
			patplan.Ordinal=2;
			PatPlans.Insert(patplan);
			//PATIENT 4=========================================================================
			//son of pat#2
			pat=new Patient();
			pat.PatStatus=PatientStatus.Patient;
			pat.Position=PatientPosition.Child;
			pat.Gender=PatientGender.Male;
			pat.Birthdate=new DateTime(1988,11,2);
			pat.LName="Smith";
			pat.FName="John";
			pat.MiddleI="B";
			pat.CanadianEligibilityCode=4;//code not applicable
			pat.Address="P.O. Box 1500";
			pat.Address2="Little Field Estates";
			pat.City="East Westchester";
			pat.State="ON";
			pat.Zip="M7F2J9";
			pat.Language="en";
			pat.Guarantor=PatNum2;//same family as patient #2.
			Patients.Insert(pat,false);
			PatNum4=pat.PatNum;
			//primary coverage------------------------------
			patplan=new PatPlan();
			patplan.PatNum=pat.PatNum;
			patplan.InsSubNum=subNum_pat2_pri;
			patplan.Relationship=Relat.Child;//3
			patplan.PatID="02";
			patplan.Ordinal=1;
			PatPlans.Insert(patplan);
			//secondary----------------------------------------
			plan=new InsPlan();
			plan.CarrierNum=CarrierTC.GetCarrierNumById("777777");
			plan.GroupNum="P9902";
			plan.DivisionNo="";
			plan.DentaideCardSequence=0;
			plan.CanadianPlanFlag="";
			plan.ClaimFormNum=7;//Canadian claim form
			InsPlans.Insert(plan);
			sub=new InsSub();
			sub.PlanNum=plan.PlanNum;
			sub.Subscriber=PatNum2;
			sub.SubscriberID="12343D6";//had to add this as separate plan because of unique subscriber id.
			InsSubs.Insert(sub);
			patplan=new PatPlan();
			patplan.PatNum=pat.PatNum;
			patplan.InsSubNum=sub.InsSubNum;
			patplan.Relationship=Relat.Child;//3
			patplan.PatID="00";
			patplan.Ordinal=2;
			PatPlans.Insert(patplan);
			//PATIENT 5=========================================================================
			pat=new Patient();
			pat.PatStatus=PatientStatus.Patient;
			pat.Position=PatientPosition.Single;
			pat.Gender=PatientGender.Male;
			pat.Birthdate=new DateTime(1964,5,16);
			pat.LName="Howard";
			pat.FName="Bob";
			pat.MiddleI="L";
			pat.CanadianEligibilityCode=4;//code not applicable
			pat.Address="1542 West Boulevard";
			pat.Address2="";
			pat.City="Fort Happens";
			pat.State="SK";
			pat.Zip="S4J4D4";
			pat.Language="en";
			Patients.Insert(pat,false);
			PatNum5=pat.PatNum;
			oldPatient=pat.Copy();
			pat.Guarantor=pat.PatNum;
			Patients.Update(pat,oldPatient);
			ToothInitialTC.SetMissing("12",pat.PatNum);
			ToothInitialTC.SetMissing("33",pat.PatNum);
			ToothInitialTC.SetMissing("34",pat.PatNum);
			//ins----------------------------------------------------------------------
			plan=new InsPlan();
			plan.CarrierNum=CarrierTC.GetCarrierNumById("888888");
			plan.GroupNum="17542";
			plan.DivisionNo="";
			plan.DentaideCardSequence=0;
			plan.CanadianPlanFlag="N";
			plan.ClaimFormNum=7;//Canadian claim form
			InsPlans.Insert(plan);
			sub=new InsSub();
			sub.PlanNum=plan.PlanNum;
			sub.Subscriber=pat.PatNum;
			sub.SubscriberID="30322145";//concat bandNumber(303) and familyNumber(22145)
			InsSubs.Insert(sub);
			patplan=new PatPlan();
			patplan.PatNum=pat.PatNum;
			patplan.InsSubNum=sub.InsSubNum;
			patplan.Relationship=Relat.Self;//1
			patplan.PatID="00";
			patplan.Ordinal=1;
			PatPlans.Insert(patplan);
			//PATIENT 6=========================================================================
			pat=new Patient();
			pat.PatStatus=PatientStatus.Patient;
			pat.Position=PatientPosition.Married;
			pat.Gender=PatientGender.Female;
			pat.Birthdate=new DateTime(1954,12,25);
			pat.LName="West";
			pat.FName="Martha";
			pat.MiddleI="F";
			pat.CanadianEligibilityCode=4;//code not applicable
			pat.Address="156 East 154 Street";
			pat.Address2="";
			pat.City="100 Mile House";
			pat.State="BC";
			pat.Zip="V4V6D7";
			pat.Language="en";
			Patients.Insert(pat,false);
			PatNum6=pat.PatNum;
			oldPatient=pat.Copy();
			pat.Guarantor=pat.PatNum;
			Patients.Update(pat,oldPatient);
			//patient 6b--------------------------------------------------------------------------
			pat=new Patient();
			pat.PatStatus=PatientStatus.NonPatient;
			pat.Position=PatientPosition.Married;
			pat.Gender=PatientGender.Male;
			pat.Birthdate=new DateTime(1952,06,25);
			pat.LName="West";
			pat.FName="Henry";
			pat.MiddleI="B";
			pat.CanadianEligibilityCode=4;//code not applicable
			pat.Address="156 East 154 Street";
			pat.Address2="";
			pat.City="100 Mile House";
			pat.State="BC";
			pat.Zip="V4V6D7";
			pat.Language="en";
			pat.Guarantor=PatNum6;
			Patients.Insert(pat,false);
			//primary----------------------------------------------------------------------
			plan=new InsPlan();
			plan.CarrierNum=CarrierTC.GetCarrierNumById("666666");
			plan.GroupNum="2221";
			plan.DivisionNo="BA1765";
			plan.DentaideCardSequence=0;
			plan.CanadianPlanFlag="";
			plan.ClaimFormNum=7;//Canadian claim form
			InsPlans.Insert(plan);
			sub=new InsSub();
			sub.PlanNum=plan.PlanNum;
			sub.Subscriber=PatNum6;
			sub.SubscriberID="19234G";
			InsSubs.Insert(sub);
			patplan=new PatPlan();
			patplan.PatNum=PatNum6;
			patplan.InsSubNum=sub.InsSubNum;
			patplan.Relationship=Relat.Self;//1
			patplan.PatID="00";
			patplan.Ordinal=1;
			PatPlans.Insert(patplan);
			//secondary----------------------------------------------------------------------
			plan=new InsPlan();
			plan.CarrierNum=CarrierTC.GetCarrierNumById("777777");
			plan.GroupNum="P4042";
			plan.DivisionNo="15476";
			plan.DentaideCardSequence=0;
			plan.CanadianPlanFlag="";
			plan.ClaimFormNum=7;//Canadian claim form
			InsPlans.Insert(plan);
			sub=new InsSub();
			sub.PlanNum=plan.PlanNum;
			sub.Subscriber=pat.PatNum;//Henry
			sub.SubscriberID="D6PD4";
			InsSubs.Insert(sub);
			patplan=new PatPlan();
			patplan.PatNum=PatNum6;
			patplan.InsSubNum=sub.InsSubNum;
			patplan.Relationship=Relat.Spouse;//2
			patplan.PatID="01";
			patplan.Ordinal=2;
			PatPlans.Insert(patplan);
			//PATIENT 7=========================================================================
			pat=new Patient();
			pat.PatStatus=PatientStatus.Patient;
			pat.Position=PatientPosition.Married;
			pat.Gender=PatientGender.Female;
			pat.Birthdate=new DateTime(1940,5,1);
			pat.LName="Arpège";
			pat.FName="Madeleine";
			pat.MiddleI="É";
			pat.CanadianEligibilityCode=4;//code not applicable
			pat.Address="1542 Rue de Peel, suite 104";
			pat.Address2="";
			pat.City="Québec";
			pat.State="QC";
			pat.Zip="H4A2D7";
			pat.Language="fr";
			Patients.Insert(pat,false);
			PatNum7=pat.PatNum;
			oldPatient=pat.Copy();
			pat.Guarantor=pat.PatNum;
			Patients.Update(pat,oldPatient);
			//patient 7b--------------------------------------------------------------------------
			pat=new Patient();
			pat.PatStatus=PatientStatus.NonPatient;
			pat.Position=PatientPosition.Married;
			pat.Gender=PatientGender.Male;
			pat.Birthdate=new DateTime(1945,6,25);
			pat.LName="Arpège";
			pat.FName="Maurice";
			pat.MiddleI="L";
			pat.CanadianEligibilityCode=4;//code not applicable
			pat.Address="1542 Rue de Peel, suite 104";
			pat.Address2="";
			pat.City="Québec";
			pat.State="QC";
			pat.Zip="H4A2D7";
			pat.Language="fr";
			pat.Guarantor=PatNum7;
			Patients.Insert(pat,false);
			//primary----------------------------------------------------------------------
			plan=new InsPlan();
			plan.CarrierNum=CarrierTC.GetCarrierNumById("777777");
			plan.GroupNum="AN99012";
			plan.DivisionNo="887B3";
			plan.DentaideCardSequence=22;
			plan.CanadianPlanFlag="";
			plan.ClaimFormNum=7;//Canadian claim form
			InsPlans.Insert(plan);
			sub=new InsSub();
			sub.PlanNum=plan.PlanNum;
			sub.Subscriber=PatNum7;
			sub.SubscriberID="344C41";
			InsSubs.Insert(sub);
			patplan=new PatPlan();
			patplan.PatNum=PatNum7;
			patplan.InsSubNum=sub.InsSubNum;
			patplan.Relationship=Relat.Self;//1
			patplan.PatID="00";
			patplan.Ordinal=1;
			PatPlans.Insert(patplan);
			//secondary----------------------------------------------------------------------
			plan=new InsPlan();
			plan.CarrierNum=CarrierTC.GetCarrierNumById("666666");
			plan.GroupNum="P605B2";
			plan.DivisionNo="";
			plan.DentaideCardSequence=0;
			plan.CanadianPlanFlag="";
			plan.ClaimFormNum=7;//Canadian claim form
			InsPlans.Insert(plan);
			sub=new InsSub();
			sub.PlanNum=plan.PlanNum;
			sub.Subscriber=pat.PatNum;//Maurice
			sub.SubscriberID="D6577";
			InsSubs.Insert(sub);
			patplan=new PatPlan();
			patplan.PatNum=PatNum7;
			patplan.InsSubNum=sub.InsSubNum;
			patplan.Relationship=Relat.Spouse;//2
			patplan.PatID="01";
			patplan.Ordinal=2;
			PatPlans.Insert(patplan);
			//PATIENT 8=========================================================================
			pat=new Patient();
			pat.PatStatus=PatientStatus.Patient;
			pat.Position=PatientPosition.Married;
			pat.Gender=PatientGender.Male;
			pat.Birthdate=new DateTime(1946,5,1);
			pat.LName="Jones";
			pat.FName="Fred";
			pat.MiddleI="M";
			pat.CanadianEligibilityCode=4;//code not applicable
			pat.Address="100 Main Street";
			pat.Address2="";
			pat.City="Terrace";
			pat.State="BC";
			pat.Zip="V4A2D7";
			pat.Language="en";
			Patients.Insert(pat,false);
			PatNum8=pat.PatNum;
			oldPatient=pat.Copy();
			pat.Guarantor=pat.PatNum;
			Patients.Update(pat,oldPatient);
			//patient 8b--------------------------------------------------------------------------
			pat=new Patient();
			pat.PatStatus=PatientStatus.NonPatient;
			pat.Position=PatientPosition.Married;
			pat.Gender=PatientGender.Female;
			pat.Birthdate=new DateTime(1945,6,25);
			pat.LName="Jones";
			pat.FName="Wanda";
			pat.MiddleI="L";
			pat.CanadianEligibilityCode=4;//code not applicable
			pat.Address="100 Main Street";
			pat.Address2="";
			pat.City="Terrace";
			pat.State="BC";
			pat.Zip="V4A2D7";
			pat.Language="en";
			pat.Guarantor=PatNum8;
			Patients.Insert(pat,false);
			//primary----------------------------------------------------------------------
			plan=new InsPlan();
			plan.CarrierNum=CarrierTC.GetCarrierNumById("777777");
			plan.GroupNum="BN99012";
			plan.DivisionNo="887OP";
			plan.DentaideCardSequence=03;
			plan.CanadianPlanFlag="";
			plan.ClaimFormNum=7;//Canadian claim form
			InsPlans.Insert(plan);
			sub=new InsSub();
			sub.PlanNum=plan.PlanNum;
			sub.Subscriber=PatNum8;//Fred
			sub.SubscriberID="XX344C41";
			InsSubs.Insert(sub);
			patplan=new PatPlan();
			patplan.PatNum=PatNum8;
			patplan.InsSubNum=sub.InsSubNum;
			patplan.Relationship=Relat.Self;//1
			patplan.PatID="00";
			patplan.Ordinal=1;
			PatPlans.Insert(patplan);
			//secondary----------------------------------------------------------------------
			plan=new InsPlan();
			plan.CarrierNum=CarrierTC.GetCarrierNumById("999111");
			plan.GroupNum="P300";
			plan.DivisionNo="";
			plan.DentaideCardSequence=0;
			plan.CanadianPlanFlag="";
			plan.ClaimFormNum=7;//Canadian claim form
			InsPlans.Insert(plan);
			sub=new InsSub();
			sub.PlanNum=plan.PlanNum;
			sub.Subscriber=pat.PatNum;//Wanda
			sub.SubscriberID="12A6577";
			InsSubs.Insert(sub);
			patplan=new PatPlan();
			patplan.PatNum=PatNum8;
			patplan.InsSubNum=sub.InsSubNum;
			patplan.Relationship=Relat.Spouse;//2
			patplan.PatID="01";
			patplan.Ordinal=2;
			PatPlans.Insert(patplan);
			//PATIENT 9=========================================================================
			pat=new Patient();
			pat.PatStatus=PatientStatus.Patient;
			pat.Position=PatientPosition.Single;
			pat.Gender=PatientGender.Male;
			pat.Birthdate=new DateTime(1964,5,1);
			pat.LName="Smith";
			pat.FName="Fred";
			pat.MiddleI="A";
			pat.CanadianEligibilityCode=4;//code not applicable
			pat.Address="1500 West 4th Street";
			pat.Address2="";
			pat.City="Wells";
			pat.State="BC";
			pat.Zip="V2D2D7";
			pat.Language="en";
			Patients.Insert(pat,false);
			PatNum9=pat.PatNum;
			oldPatient=pat.Copy();
			pat.Guarantor=pat.PatNum;
			Patients.Update(pat,oldPatient);
			//ins-----------------------------------------------------------------------------
			plan=new InsPlan();
			plan.CarrierNum=CarrierTC.GetCarrierNumById("555555");
			plan.GroupNum="44C99";
			plan.DivisionNo="9914";
			plan.DentaideCardSequence=0;
			plan.CanadianPlanFlag="";
			plan.ClaimFormNum=7;//Canadian claim form
			InsPlans.Insert(plan);
			sub=new InsSub();
			sub.PlanNum=plan.PlanNum;
			sub.Subscriber=pat.PatNum;
			sub.SubscriberID="344941";
			InsSubs.Insert(sub);
			patplan=new PatPlan();
			patplan.PatNum=pat.PatNum;
			patplan.InsSubNum=sub.InsSubNum;
			patplan.Relationship=Relat.Self;//1
			patplan.PatID="";
			patplan.Ordinal=1;
			PatPlans.Insert(patplan);
			return "Patient objects set.\r\nInsPlan objects set.\r\n";
		}

	}
}
