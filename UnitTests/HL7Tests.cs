using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDentBusiness.HL7;
using OpenDentBusiness;
using OpenDental;
using OpenDental.Bridges;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeBase;
using UnitTestsCore;

namespace UnitTests.HL7_Tests {
	[TestClass]
	public class HL7Tests:TestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[TestInitialize]
		public void SetupTest() {
			//Practice default provider must be a valid provider. ProvNum 1 and 2 never get deleted from unit test databases so use ProvNum 1.
			PrefT.UpdateLong(PrefName.PracticeDefaultProv,1);
		}

		[TestMethod]
		public void HL7_TestAll() {
			//Loop through every possible HL7 mode and execute every test scenario on each one.
			foreach(HL7TestInterfaceEnum hl7TestInterfaceEnum in Enum.GetValues(typeof(HL7TestInterfaceEnum))) {
				//Clear the DB between each and every iteration (preserves old behavior).
				UnitTestsCore.DatabaseTools.ClearDb();
				//Enable the correct eCW program link or HL7Def
				switch(hl7TestInterfaceEnum) {
					case HL7TestInterfaceEnum.HL7DefEcwFull:
					HL7Defs.EnableInternalForTests(HL7InternalType.eCWFull);
					HL7Defs.RefreshCache();
					break;
					case HL7TestInterfaceEnum.HL7DefEcwStandalone:
					HL7Defs.EnableInternalForTests(HL7InternalType.eCWStandalone);
					HL7Defs.RefreshCache();
					break;
					case HL7TestInterfaceEnum.HL7DefEcwTight:
					HL7Defs.EnableInternalForTests(HL7InternalType.eCWTight);
					HL7Defs.RefreshCache();
					break;
				}
				Test1(hl7TestInterfaceEnum);
				Test2(hl7TestInterfaceEnum);
				Test3(hl7TestInterfaceEnum);
				Test4(hl7TestInterfaceEnum);
				Test5(hl7TestInterfaceEnum);
				Test6(hl7TestInterfaceEnum);
				Test7(hl7TestInterfaceEnum);
				Test8(hl7TestInterfaceEnum);
				Test9(hl7TestInterfaceEnum);
				Test10(hl7TestInterfaceEnum);
				Test11(hl7TestInterfaceEnum);
				Test12(hl7TestInterfaceEnum);
				Test13(hl7TestInterfaceEnum);
				Test15(hl7TestInterfaceEnum);
			}
		}

		#region HL7_TestAll Helper Methods

		///<summary>Test 1: Fail to locate the patient by PatNum in PID.2, so insert a new patient into the database.  Fail to locate the guarantor by PatNum in GT1.2, so insert a new patient into the database.  Set patient.Guarantor=guarantor.PatNum and guarantor.Guarantor=guarantor.PatNum.  Set patient.PriProv=preference.PracticeDefaultProv and guarantor.PriProv=preference.PracticeDefaultProv.  Set patient.BillingType=preference.PracticeDefaultBillType and guarantor.BillingType=preference.PracticeDefaultBillType.  Other patient fields: LName,FName,MiddleI,Birthdate,Gender,Race,Address,Address2,City,State,Zip,HmPhone,WkPhone,Position,SSN,FeeSched.  Other guarantor fields: LName,FName,MiddleI,Birthdate,Gender,Address,Address2,City,State,Zip,HmPhone,WkPhone,SSN.  EcwOldTight,EcwOldFull,EcwTight,HL7DefEcwFull: Set patient.PatNum=10, patient.ChartNumber=A10, guarantor.PatNum=11, guarantor.ChartNumber="".  EcwOldStandalone,HL7DefEcwStandalone: Set patient.ChartNumber="10", guarantor.ChartNumber="11". (PatNums will be next auto-incremented value.)</summary>
		private void Test1(HL7TestInterfaceEnum hl7TestInterfaceEnum) {
			string msgtext=@"MSH|^~\&|||||20120901100000||ADT^A04||P|2.3"+"\r\n"
				+"EVN|A04|20120901100000||\r\n"
				+"PID|1|10||A10|Smiths^Jan^F||19760210|M||Hispanic|421 Main Ave^Apt 7^Salem^MA^97330||5305554045|5305554234||Single|||111226666|||Standard\r\n"
				+"GT1|1|11|Smiths^Jon^F||421 Main Ave^Apt 7^Salem^MA^97330|5305554743|5303635432|19770730|F|||111225555";
			MessageHL7 msg=new MessageHL7(msgtext);
			try {
				switch(hl7TestInterfaceEnum) {
					case HL7TestInterfaceEnum.EcwOldTight:
 					case HL7TestInterfaceEnum.EcwOldFull:
						EcwADT.ProcessMessage(msg,false,false);
						break;
					case HL7TestInterfaceEnum.EcwOldStandalone:
						EcwADT.ProcessMessage(msg,true,false);
						break;
					default:
						MessageParser.Process(msg,false);
						break;
				}
			}
			catch(Exception ex) {
				Assert.Fail("Test 1: Message processing error: "+ex);
			}
			Patient pat=null;
			Patient guar=null;
			Patient correctPat=new Patient();
			Patient correctGuar=new Patient();
			switch(hl7TestInterfaceEnum) {
				case HL7TestInterfaceEnum.EcwOldStandalone:
				case HL7TestInterfaceEnum.HL7DefEcwStandalone:
					correctPat.ChartNumber="10";
					correctGuar.ChartNumber="11";
					pat=Patients.GetPatByChartNumber(correctPat.ChartNumber);
					guar=Patients.GetPatByChartNumber(correctGuar.ChartNumber);
					break;
				default:
					correctPat.ChartNumber="A10";
					correctPat.PatNum=10;
					correctGuar.ChartNumber="";
					correctGuar.PatNum=11;
					pat=Patients.GetPat(correctPat.PatNum);
					guar=Patients.GetPat(correctGuar.PatNum);
					break;
			}
			if(pat==null) {
				Assert.Fail("Test 1: Couldn't locate patient.");
			}
			if(guar==null) {
				Assert.Fail("Test 1: Couldn't locate guarantor.");
			}
			//Set up the rest of pat information
			correctPat.PriProv=PrefC.GetLong(PrefName.PracticeDefaultProv);
			correctPat.BillingType=PrefC.GetLong(PrefName.PracticeDefaultBillType);
			correctPat.FName="Jan";
			correctPat.MiddleI="F";
			correctPat.LName="Smiths";
			correctPat.Birthdate=new DateTime(1976,02,10);
			correctPat.Gender=PatientGender.Male;
			//correctPat.Race=PatientRaceOld.HispanicLatino;
			List<PatientRace> races=new List<PatientRace>();
			races.Add(new PatientRace(correctPat.PatNum,"2106-3"));//White
			races.Add(new PatientRace(correctPat.PatNum,"2135-2"));//Hispanic
			PatientRaces.Reconcile(correctPat.PatNum,races);
			correctPat.Address="421 Main Ave";
			correctPat.Address2="Apt 7";
			correctPat.City="Salem";
			correctPat.State="MA";
			correctPat.Zip="97330";
			correctPat.HmPhone="(530)555-4045";
			correctPat.WkPhone="(530)555-4234";
			correctPat.Position=PatientPosition.Single;
			correctPat.SSN="111226666";
			correctPat.FeeSched=FeeScheds.GetByExactName("Standard").FeeSchedNum;
			//Set up the rest of guar information
			correctGuar.PriProv=PrefC.GetLong(PrefName.PracticeDefaultProv);
			correctGuar.BillingType=PrefC.GetLong(PrefName.PracticeDefaultBillType);
			correctGuar.FName="Jon";
			correctGuar.MiddleI="F";
			correctGuar.LName="Smiths";
			correctGuar.Birthdate=new DateTime(1977,07,30);
			correctGuar.Gender=PatientGender.Female;
			correctGuar.Address="421 Main Ave";
			correctGuar.Address2="Apt 7";
			correctGuar.City="Salem";
			correctGuar.State="MA";
			correctGuar.Zip="97330";
			correctGuar.HmPhone="(530)555-4743";
			correctGuar.WkPhone="(530)363-5432";
			correctGuar.SSN="111225555";
			CompareGuarAndPat(pat,correctPat,guar,correctGuar,hl7TestInterfaceEnum,races);
		}

		///<summary>Test 2: Locate the patient inserted in TEST 1 with PID.2 and the guarantor from TEST 1 with GT1.2.  EcwOldTight,EcwOldFull,EcwTight,HL7DefEcwFull: PID.2 and GT1.2 are PatNum so locate by PatNum.  Update patient.ChartNumber to A11.  All other demographic information should be updated.  EcwOldStandalone,HL7DefEcwStandalone: PID.2 and GT1.2 are ChartNumber so locate by ChartNumber.  patient.ChartNumber is not changed.  All other demographic information should be updated.</summary>
		private void Test2(HL7TestInterfaceEnum hl7TestInterfaceEnum) {
			string msgtext=@"MSH|^~\&|||||20120901100000||ADT^A04||P|2.3"+"\r\n"
				+"EVN|A04|20120901100000||\r\n"
				+"PID|1|10||A11|Smith^Jane^N||19760205|Female||White|421 Main St^Apt 17^Dallas^OR^97338||5035554045|5035554234||Married|||111224444|||Standard\r\n"
				+"GT1|1|11|Smith^John^L||421 Main St^Apt 17^Dallas^OR^97338|5035554743|5033635432|19770704|Male|||111223333";
			MessageHL7 msg=new MessageHL7(msgtext);
			try {
				switch(hl7TestInterfaceEnum) {
					case HL7TestInterfaceEnum.EcwOldTight:
					case HL7TestInterfaceEnum.EcwOldFull:
						EcwADT.ProcessMessage(msg,false,false);
						break;
					case HL7TestInterfaceEnum.EcwOldStandalone:
						EcwADT.ProcessMessage(msg,true,false);
						break;
					default:
						MessageParser.Process(msg,false);
						break;
				}
			}
			catch(Exception ex) {
				Assert.Fail("Test 2: Message processing error: "+ex);
			}
			Patient pat=null;
			Patient guar=null;
			Patient correctPat=new Patient();
			Patient correctGuar=new Patient();
			switch(hl7TestInterfaceEnum) {
				case HL7TestInterfaceEnum.EcwOldStandalone:
				case HL7TestInterfaceEnum.HL7DefEcwStandalone:
					correctPat.ChartNumber="10";
					correctGuar.ChartNumber="11";
					pat=Patients.GetPatByChartNumber(correctPat.ChartNumber);
					guar=Patients.GetPatByChartNumber(correctGuar.ChartNumber);
					break;
				default:
					correctPat.ChartNumber="A11";
					correctPat.PatNum=10;
					correctGuar.ChartNumber="";
					correctGuar.PatNum=11;
					pat=Patients.GetPat(correctPat.PatNum);
					guar=Patients.GetPat(correctGuar.PatNum);
					break;
			}
			if(pat==null) {
				Assert.Fail("Test 2: Couldn't locate patient.");
			}
			if(guar==null) {
				Assert.Fail("Test 2: Couldn't locate guarantor.");
			}
			//Set up the rest of pat information
			correctPat.PriProv=PrefC.GetLong(PrefName.PracticeDefaultProv);
			correctPat.BillingType=PrefC.GetLong(PrefName.PracticeDefaultBillType);
			correctPat.FName="Jane";
			correctPat.MiddleI="N";
			correctPat.LName="Smith";
			correctPat.Birthdate=new DateTime(1976,02,05);
			correctPat.Gender=PatientGender.Female;
			//correctPat.Race=PatientRaceOld.White;
			List<PatientRace> races=new List<PatientRace>();
			races.Add(new PatientRace(correctPat.PatNum,"2106-3"));//White
			PatientRaces.Reconcile(correctPat.PatNum,races);
			correctPat.Address="421 Main St";
			correctPat.Address2="Apt 17";
			correctPat.City="Dallas";
			correctPat.State="OR";
			correctPat.Zip="97338";
			correctPat.HmPhone="(503)555-4045";
			correctPat.WkPhone="(503)555-4234";
			correctPat.Position=PatientPosition.Married;
			correctPat.SSN="111224444";
			correctPat.FeeSched=FeeScheds.GetByExactName("Standard").FeeSchedNum;
			//Set up the rest of guar information
			correctGuar.PriProv=PrefC.GetLong(PrefName.PracticeDefaultProv);
			correctGuar.BillingType=PrefC.GetLong(PrefName.PracticeDefaultBillType);
			correctGuar.FName="John";
			correctGuar.MiddleI="L";
			correctGuar.LName="Smith";
			correctGuar.Birthdate=new DateTime(1977,07,04);
			correctGuar.Gender=PatientGender.Male;
			correctGuar.Address="421 Main St";
			correctGuar.Address2="Apt 17";
			correctGuar.City="Dallas";
			correctGuar.State="OR";
			correctGuar.Zip="97338";
			correctGuar.HmPhone="(503)555-4743";
			correctGuar.WkPhone="(503)363-5432";
			correctGuar.SSN="111223333";
			CompareGuarAndPat(pat,correctPat,guar,correctGuar,hl7TestInterfaceEnum,races);
		}

		///<summary>Test 3: If either the FName or LName fields are blank, no information should be updated.  Compare test patient and guarantor before message to test patient and guarantor after message.</summary>
		private void Test3(HL7TestInterfaceEnum hl7TestInterfaceEnum) {
			string msgtext=@"MSH|^~\&|||||20120901100000||ADT^A04||P|2.3"+"\r\n"
				+"EVN|A04|20120901100000||\r\n"
				+"PID|1|10||A10|Smith^^N||19760215|M||Asian|421 Main Ave^Apt 7^Salem^MA^97330||5305554045|5305554234||Single|||111224444|||Standard\r\n"
				+"GT1|1|11|^John^L||421 Main Ave^Apt 7^Salem^MA^97330|5305554743|5303635432|19770730|F|||111223333";
			Patient correctPat=null;
			Patient correctGuar=null;
			switch(hl7TestInterfaceEnum) {
				case HL7TestInterfaceEnum.EcwOldStandalone:
				case HL7TestInterfaceEnum.HL7DefEcwStandalone:
					correctPat=Patients.GetPatByChartNumber("10");
					correctGuar=Patients.GetPatByChartNumber("11");
					break;
				default:
					correctPat=Patients.GetPat(10);
					correctGuar=Patients.GetPat(11);
					break;
			}
			if(correctPat==null) {
				Assert.Fail("Test 3: Couldn't locate patient.");
			}
			if(correctGuar==null) {
				Assert.Fail("Test 3: Couldn't locate guarantor.");
			}
			MessageHL7 msg=new MessageHL7(msgtext);
			try {
				switch(hl7TestInterfaceEnum) {
					case HL7TestInterfaceEnum.EcwOldTight:
					case HL7TestInterfaceEnum.EcwOldFull:
						EcwADT.ProcessMessage(msg,false,false);
						break;
					case HL7TestInterfaceEnum.EcwOldStandalone:
						EcwADT.ProcessMessage(msg,true,false);
						break;
					default:
						MessageParser.Process(msg,false);
						break;
				}
			}
			catch(Exception ex) {
				Assert.Fail("Test 3: Message processing error: "+ex);
			}
			Patient pat=null;
			Patient guar=null;
			switch(hl7TestInterfaceEnum) {
				case HL7TestInterfaceEnum.EcwOldStandalone:
				case HL7TestInterfaceEnum.HL7DefEcwStandalone:
					pat=Patients.GetPatByChartNumber("10");
					guar=Patients.GetPatByChartNumber("11");
					break;
				default:
					pat=Patients.GetPat(10);
					guar=Patients.GetPat(11);
					break;
			}
			if(pat==null) {
				Assert.Fail("Test 3: Couldn't locate patient.");
			}
			if(guar==null) {
				Assert.Fail("Test 3: Couldn't locate guarantor.");
			}
			CompareGuarAndPat(pat,correctPat,guar,correctGuar,hl7TestInterfaceEnum);
		}

		/// <summary>Test 4: Insert two new patients, patient from PID and guarantor from GT1.  If birthdate is less than 8 digits, set to MinValue of 0001-01-01.  If birthdate is 8 digits or more, set to the correct precision with yyyyMMddHHmmss format and HHmmss... all optional.  patient.Birthdate=0001-01-01.  guarantor.Birthdate=1977-07-03 7:11 AM but inserted as date 1977-07-03.</summary>
		private void Test4(HL7TestInterfaceEnum hl7TestInterfaceEnum) {
			string msgtext=@"MSH|^~\&|||||20120901100000||ADT^A04||P|2.3"+"\r\n"
				+"EVN|A04|20120901100000||\r\n"
				+"PID|1|20||A100|Smith^Jane^N||197602|Female||White|421 Main St^Apt 17^Dallas^OR^97338||5035554045|5035554234||Married|||111224444|||Standard\r\n"
				+"GT1|1|21|Smith^John^L||421 Main St^Apt 17^Dallas^OR^97338|5035554743|5033635432|197707030711|Male|||111223333";
			MessageHL7 msg=new MessageHL7(msgtext);
			try {
				switch(hl7TestInterfaceEnum) {
					case HL7TestInterfaceEnum.EcwOldTight:
					case HL7TestInterfaceEnum.EcwOldFull:
						EcwADT.ProcessMessage(msg,false,false);
						break;
					case HL7TestInterfaceEnum.EcwOldStandalone:
						EcwADT.ProcessMessage(msg,true,false);
						break;
					default:
						MessageParser.Process(msg,false);
						break;
				}
			}
			catch(Exception ex) {
				Assert.Fail("Test 4: Message processing error: "+ex);
			}
			Patient pat=null;
			Patient guar=null;
			switch(hl7TestInterfaceEnum) {
				case HL7TestInterfaceEnum.EcwOldStandalone:
				case HL7TestInterfaceEnum.HL7DefEcwStandalone:
					pat=Patients.GetPatByChartNumber("20");
					guar=Patients.GetPatByChartNumber("21");
					break;
				default:
					pat=Patients.GetPat(20);
					guar=Patients.GetPat(21);
					break;
			}
			if(pat==null) {
				Assert.Fail("Test 4: Couldn't locate patient.");
			}
			if(guar==null) {
				Assert.Fail("Test 4: Couldn't locate guarantor.");
			}
			if(pat.Birthdate!=DateTime.MinValue) {
				Assert.Fail("Test 4: Patient Birthdate should be 0001-01-01.");
			}
			switch(hl7TestInterfaceEnum) {
				case HL7TestInterfaceEnum.EcwOldFull:
				case HL7TestInterfaceEnum.EcwOldStandalone:
				case HL7TestInterfaceEnum.EcwOldTight:
					if(guar.Birthdate!=DateTime.MinValue) {
						Assert.Fail("Test 4: Guarantor Birthdate should be 0001-01-01.");
					}
					break;
				default:
					if(guar.Birthdate!=new DateTime(1977,07,03,0,0,0)) {
						Assert.Fail("Test 4: Guarantor Birthdate should be 1977-07-03.");
					}
					break;
			}
		}

		/// <summary>Test 5: EcwOldTight,EcwOldFull,EcwTight,HL7DefEcwFull: Fail to locate patient so insert new patient.  No PV1 or AIG segment.  New appointment so insert appointment into database with appointment.AptNum=100.  appointment.PatNum=30; appointment.AptStatus=ApptStatus.Scheduled; appointment.Note="Checkup"; appointment.AptDateTime="2012-09-01 10:00 AM"; appointment.Pattern="XXXX"; appointment.ProvNum=pat.PriProv=preference.PracticeDefaultProv.  EcwOldStandalone,HL7DefEcwStandalone: SIU messages are not processed in Standalone mode.</summary>
		private void Test5(HL7TestInterfaceEnum hl7TestInterfaceEnum) {
			string msgtext=@"MSH|^~\&|||||20120901100000||SIU^S12||P|2.3"+"\r\n"
				+"SCH|100|100|||||Checkup||||^^1200^20120901100000^20120901102000||||||||||||||\r\n"
				+"PID|1|30||A30|Smiths2^Jan2^L||19760210|Male||Hispanic|421 Main Ave^Apt 7^Salem^MA^97330||5305554045|5305554234||Single|||222334444|||Standard";
			MessageHL7 msg=new MessageHL7(msgtext);
			try {
				switch(hl7TestInterfaceEnum) {
					case HL7TestInterfaceEnum.EcwOldTight:
					case HL7TestInterfaceEnum.EcwOldFull:
						EcwSIU.ProcessMessage(msg,false);
						break;
					case HL7TestInterfaceEnum.EcwOldStandalone:
					case HL7TestInterfaceEnum.HL7DefEcwStandalone:
						return;//Test 5: Passed.
					default:
						MessageParser.Process(msg,false);
						break;
				}
			}
			catch(Exception ex) {
				Assert.Fail("Test 5: Message processing error: "+ex);
			}
			Patient pat=Patients.GetPat(30);
			if(pat==null) {
				Assert.Fail("Test 5: Couldn't locate patient.");
			}
			Appointment apt=Appointments.GetOneApt(100);
			if(apt==null) {
				Assert.Fail("Test 5: Couldn't locate appointment.");
			}
			string retval="";
			if(apt.PatNum!=pat.PatNum) {
				retval+="Appointment.PatNum does not match patient.PatNum.\r\n";
			}
			if(apt.AptStatus!=ApptStatus.Scheduled) {
				retval+="Appointment.AptStatus is not 'Scheduled'.\r\n";
			}
			if(apt.Note!="Checkup") {
				retval+="Appointment.Note is not 'Checkup'.\r\n";
			}
			if(apt.AptDateTime!=new DateTime(2012,09,01,10,0,0)) {
				retval+="Appointment.AptDateTime is not correct.\r\n";
			}
			if(apt.Pattern!="XXXX") {
				retval+="Appointment.Pattern is not 'XXXX'.\r\n";
			}
			if(apt.ProvNum!=pat.PriProv || pat.PriProv!=PrefC.GetLong(PrefName.PracticeDefaultProv)) {
				retval+="Appointment.ProvNum is not PracticeDefaultProv.\r\n";
			}
			if(retval.Length>0) {
				Assert.Fail("Test 5: "+retval);
			}
		}

		///<summary>Test 6: EcwOldTight,EcwOldFull,EcwTight,HL7DefEcwFull: Locate the patient with PatNum in PID.2 and appointment with AptNum in SCH.1 (inserted in Test 5).  Update the appointment and patient information.  appointment.Note="Checkup and Fillings"; appointment.AptDateTime="2012-09-02 10:00 AM"; appointment.Pattern="XXXXXXXX".  EcwOldStandalone,HL7DefEcwStandalone: SIU messages are not processed in Standalone mode.</summary>
		private void Test6(HL7TestInterfaceEnum hl7TestInterfaceEnum) {
			string msgtext=@"MSH|^~\&|||||20120901100000||SIU^S12||P|2.3"+"\r\n"
				+"SCH|100|100|||||Checkup and Fillings||||^^2400^20120902100000^20120902104000||||||||||||||\r\n"
				+"PID|1|30||A300|Smith2^Jane2^N||19760205|Female||White|421 Main St^Apt 17^Dallas^OR^97338||5035554045|5035554234||Married|||222334444|||Standard";
			MessageHL7 msg=new MessageHL7(msgtext);
			try {
				switch(hl7TestInterfaceEnum) {
					case HL7TestInterfaceEnum.EcwOldTight:
					case HL7TestInterfaceEnum.EcwOldFull:
						EcwSIU.ProcessMessage(msg,false);
						break;
					case HL7TestInterfaceEnum.EcwOldStandalone:
					case HL7TestInterfaceEnum.HL7DefEcwStandalone:
						return;//Test 6: Passed.
					default:
						MessageParser.Process(msg,false);
						break;
				}
			}
			catch(Exception ex) {
				Assert.Fail("Test 6: Message processing error: "+ex);
			}
			Patient pat=null;
			Patient guar=null;
			Patient correctPat=new Patient();
			Patient correctGuar=null;
			correctPat.ChartNumber="A300";
			correctPat.PatNum=30;
			pat=Patients.GetPat(correctPat.PatNum);
			guar=Patients.GetPat(correctPat.PatNum);//Get copies of same pat to send to our comparison function CompareGuarAndPat
			correctGuar=Patients.GetPat(correctPat.PatNum);
			if(pat==null) {
				Assert.Fail("Test 6: Couldn't locate patient.");
			}
			//Set up the rest of pat information
			correctPat.PriProv=PrefC.GetLong(PrefName.PracticeDefaultProv);
			correctPat.BillingType=PrefC.GetLong(PrefName.PracticeDefaultBillType);
			correctPat.FName="Jane2";
			correctPat.MiddleI="N";
			correctPat.LName="Smith2";
			correctPat.Birthdate=new DateTime(1976,02,05);
			correctPat.Gender=PatientGender.Female;
			//correctPat.Race=PatientRaceOld.White;
			List<PatientRace> races=new List<PatientRace>();
			races.Add(new PatientRace(correctPat.PatNum,"2106-3"));//White
			PatientRaces.Reconcile(correctPat.PatNum,races);
			correctPat.Address="421 Main St";
			correctPat.Address2="Apt 17";
			correctPat.City="Dallas";
			correctPat.State="OR";
			correctPat.Zip="97338";
			correctPat.HmPhone="(503)555-4045";
			correctPat.WkPhone="(503)555-4234";
			correctPat.Position=PatientPosition.Married;
			correctPat.SSN="222334444";
			correctPat.FeeSched=FeeScheds.GetByExactName("Standard").FeeSchedNum;
			Appointment apt=Appointments.GetOneApt(100);
			if(apt==null) {
				Assert.Fail("Test 6: Couldn't locate appointment.");
			}
			string retval="";
			if(apt.PatNum!=pat.PatNum) {
				retval+="Appointment.PatNum does not match patient.PatNum.\r\n";
			}
			if(apt.AptStatus!=ApptStatus.Scheduled) {
				retval+="Appointment.AptStatus is not 'Scheduled'.\r\n";
			}
			if(ListTools.In(hl7TestInterfaceEnum,HL7TestInterfaceEnum.EcwOldTight,HL7TestInterfaceEnum.EcwOldFull)) {
				if(apt.Note!="Checkup and Fillings") {
					retval+="Appointment.Note is not 'Checkup and Fillings'.\r\n";
				}
			}
			else {
				//Our new way of processing the appointment note is to APPEND text to appointments instead of overwriting them.
				//Test 5 will have created appointment #100 with a note of 'Checkup'.
				if(apt.Note!="Checkup\r\nCheckup and Fillings") {
					retval+="Appointment.Note is not 'Checkup\r\nCheckup and Fillings'.\r\n";
				}
			}
			if(apt.AptDateTime!=new DateTime(2012,09,02,10,0,0)) {
				retval+="Appointment.AptDateTime is not correct.\r\n";
			}
			if(apt.Pattern!="XXXXXXXX") {
				retval+="Appointment.Pattern is not 'XXXXXXXX'.\r\n";
			}
			if(apt.ProvNum!=pat.PriProv || pat.PriProv!=PrefC.GetLong(PrefName.PracticeDefaultProv)) {
				retval+="Appointment.ProvNum is not PracticeDefaultProv.\r\n";
			}
			CompareGuarAndPat(pat,correctPat,guar,correctGuar,hl7TestInterfaceEnum,races);
		}

		///<summary>Test 7: EcwOldTight,EcwOldFull,EcwTight,HL7DefEcwFull: Before processing this message, change appointment.PatNum from appointment updated in Test 6 to 40.  Locate the patient with PID.2 and appointment with SCH.1.  Compare appointment.PatNum (now 40) to patient.PatNum (30) and since they don't match anymore, do not update any information.  We will use appointment.AptDateTime to verify that nothing was changed in the appointment table.  Compare patient before processing message to patient after processing to verify no changes were made.  After test return appointment.PatNum to 30 for TEST 8.  EcwOldStandalone,HL7DefEcwStandalone: SIU messages are not processed in Standalone mode.</summary>
		private void Test7(HL7TestInterfaceEnum hl7TestInterfaceEnum) {
			string msgtext=@"MSH|^~\&|||||20120901100000||SIU^S12||P|2.3"+"\r\n"
				+"SCH|100|100|||||Checkup and Fillings||||^^2400^20120903100000^20120903104000||||||||||||||\r\n"
				+"PID|1|30||A300|Smith2^Jane2^N||19760210|Male||White|421 Main St^Apt 17^Dallas^OR^97338||5035554045|5035554234||Married|||222334444|||Standard";
			Patient correctPat=null;
			Patient correctGuar=null;
			switch(hl7TestInterfaceEnum) {
				case HL7TestInterfaceEnum.EcwOldStandalone:
				case HL7TestInterfaceEnum.HL7DefEcwStandalone://same patient as test above
					return;//Test 7: Passed.
				default:
					correctPat=Patients.GetPat(30);
					correctGuar=Patients.GetPat(30);
					break;
			}
			if(correctPat==null) {
				Assert.Fail("Test 7: Couldn't locate patient.");
			}
			Appointment apt=Appointments.GetOneApt(100);
			if(apt==null) {
				Assert.Fail("Test 7: Couldn't locate appointment.");
			}
			Appointment correctApt=apt.Copy();
			apt.PatNum=40;
			Appointments.Update(apt,correctApt);
			MessageHL7 msg=new MessageHL7(msgtext);
			try {
				switch(hl7TestInterfaceEnum) {
					case HL7TestInterfaceEnum.EcwOldTight:
					case HL7TestInterfaceEnum.EcwOldFull:
						EcwSIU.ProcessMessage(msg,false);
						break;
					default:
						MessageParser.Process(msg,false);
						break;
				}
			}
			catch(Exception ex) {
				//Catch the exception and if it is the expected exception that would normally get logged in event log just move on.
				if(ex.ToString().Substring(18,35)!="Appointment does not match patient ") {
					Assert.Fail("Test 7: Message processing error: "+ex);
				}
			}
			Patient pat=null;
			Patient guar=null;
			pat=Patients.GetPat(30);
			guar=Patients.GetPat(30);
			if(pat==null) {
				Assert.Fail("Test 7: Couldn't locate patient.");
			}
			CompareGuarAndPat(pat,correctPat,guar,correctGuar,hl7TestInterfaceEnum);
			if(apt.AptDateTime!=new DateTime(2012,09,02,10,0,0)) {
				Assert.Fail("Appointment.AptDateTime is not correct.");
			}
			Appointments.Update(correctApt,apt);
		}

		///<summary>Test 8: EcwOldTight,EcwOldFull,EcwTight,HL7DefEcwFull: Locate the patient and appointment but since there is no LName, do not update any information.  We will use appointment.Note and patient information to verify that no information was changed.  EcwOldStandalone,HL7DefEcwStandalone: SIU messages are not processed in Standalone mode.</summary>
		private void Test8(HL7TestInterfaceEnum hl7TestInterfaceEnum) {
			string msgtext=@"MSH|^~\&|||||20120901100000||SIU^S12||P|2.3"+"\r\n"
				+"SCH|100|100|||||Checkup and Fillings and Crown||||^^2400^20120902100000^20120901104000||||||||||||||\r\n"
				+"PID|1|30||A300|^Jane2^N||19760205|Male||White|421 Test St^Apt 17^Dallas^OR^97338||5035554041|5035554234||Married|||222334444|||Standard";
			Patient correctPat=null;
			Patient correctGuar=null;
			switch(hl7TestInterfaceEnum) {
				case HL7TestInterfaceEnum.EcwOldStandalone:
				case HL7TestInterfaceEnum.HL7DefEcwStandalone:
					return;//Test 8: Passed.
				default:
					correctPat=Patients.GetPat(30);
					correctGuar=Patients.GetPat(30);
					break;
			}
			if(correctPat==null) {
				Assert.Fail("Test 8: Couldn't locate patient.");
			}
			if(correctGuar==null) {
				Assert.Fail("Test 8: Couldn't locate guarantor.");
			}
			Appointment apt=Appointments.GetOneApt(100);
			if(apt==null) {
				Assert.Fail("Test 8: Couldn't locate appointment.");
			}
			MessageHL7 msg=new MessageHL7(msgtext);
			try {
				switch(hl7TestInterfaceEnum) {
					case HL7TestInterfaceEnum.EcwOldTight:
					case HL7TestInterfaceEnum.EcwOldFull:
						EcwSIU.ProcessMessage(msg,false);
						break;
					default:
						MessageParser.Process(msg,false);
						break;
				}
			}
			catch(Exception ex) {
				//Catch the exception and if it is the expected exception that would normally get logged in event log just move on.
				if(ex.ToString().Substring(18,68)!="Appointment not processed due to missing patient first or last name.") {
					Assert.Fail("Test 8: Message processing error: "+ex);
				}
			}
			Patient pat=null;
			Patient guar=null;
			pat=Patients.GetPat(30);
			guar=Patients.GetPat(30);
			if(pat==null) {
				Assert.Fail("Test 8: Couldn't locate patient.");
			}
			if(guar==null) {
				Assert.Fail("Test 8: Couldn't locate guarantor.");
			}
			CompareGuarAndPat(pat,correctPat,guar,correctGuar,hl7TestInterfaceEnum);
			if(ListTools.In(hl7TestInterfaceEnum,HL7TestInterfaceEnum.EcwOldTight,HL7TestInterfaceEnum.EcwOldFull)) {
				if(apt.Note!="Checkup and Fillings") {
					Assert.Fail("Appointment.Note is not 'Checkup and Fillings'.");
				}
			}
			else {
				//Our new way of processing the appointment note is to APPEND text to appointments instead of overwriting them.
				//Test 5 will have created appointment #100 with a note of 'Checkup'.
				if(apt.Note!="Checkup\r\nCheckup and Fillings") {
					Assert.Fail("Appointment.Note is not 'Checkup\r\nCheckup and Fillings'.");
				}
			}
		}

		///<summary>Test 9: EcwOldTight,EcwOldFull,EcwTight,HL7DefEcwFull: Optional AIG segment is included so use that segment to insert a new provider.  provider.Abbr=DOC1; provider.EcwID=DOC1; provider.LName=Albert; provider.FName=Brian; provider.MI=S; appointment.ProvNum=provider.ProvNum; patient.PriProv=provider.ProvNum.  EcwOldStandalone,HL7DefEcwStandalone: SIU messages are not processed in Standalone mode.</summary>
		private void Test9(HL7TestInterfaceEnum hl7TestInterfaceEnum) {
			string msgtext=@"MSH|^~\&|||||20120901100000||SIU^S12||P|2.3"+"\r\n"
				+"SCH|100|100|||||Checkup and Fillings||||^^2400^20120902100000^20120902104000||||||||||||||\r\n"
				+"PID|1|30||A300|Smith2^Jane2^N||19760205|Female||White|421 Main St^Apt 17^Dallas^OR^97338||5035554045|5035554234||Married|||222334444|||Standard\r\n"
				+"AIG|1||DOC1^Albert, Brian S||||";
			MessageHL7 msg=new MessageHL7(msgtext);
			try {
				switch(hl7TestInterfaceEnum) {
					case HL7TestInterfaceEnum.EcwOldFull:
					case HL7TestInterfaceEnum.EcwOldTight:
						EcwSIU.ProcessMessage(msg,false);
						break;
					case HL7TestInterfaceEnum.EcwOldStandalone:
					case HL7TestInterfaceEnum.HL7DefEcwStandalone:
						return;//Test 9: Passed.
					default:
						MessageParser.Process(msg,false);
						break;
				}
			}
			catch(Exception ex) {
				Assert.Fail("Test 9: Message processing error: "+ex);
			}
			Patient pat=null;
			pat=Patients.GetPat(30);
			if(pat==null) {
				Assert.Fail("Test 9: Couldn't locate patient.");
			}
			Provider prov=Providers.GetProvByEcwID("DOC1");
			if(prov==null) {
				Assert.Fail("Test 9: Couldn't locate provider.");
			}
			Appointment apt=Appointments.GetOneApt(100);
			if(apt==null) {
				Assert.Fail("Test 9: Couldn't locate appointment.");
			}
			string retval="";
			switch(hl7TestInterfaceEnum) {
				case HL7TestInterfaceEnum.EcwOldFull:
				case HL7TestInterfaceEnum.EcwOldTight:
					if(prov.LName!="Albert, Brian S") {
						retval+="Provider.LName is not 'Albert, Brian S'.\r\n";
					}
					break;
				case HL7TestInterfaceEnum.HL7DefEcwFull:
				case HL7TestInterfaceEnum.HL7DefEcwTight:
					if(prov.LName!="Albert") {
						retval+="Provider.LName is not 'Albert'.\r\n";
					}
					if(prov.FName!="Brian") {
						retval+="Provider.FName is not 'Brian'.\r\n";
					}
					if(prov.MI!="S") {
						retval+="Provider.MI is not 'S'.\r\n";
					}
					break;
			}
			if(prov.Abbr!="DOC1") {
				retval+="Provider.Abbr is not 'DOC1'.\r\n";
			}
			if(apt.ProvNum!=prov.ProvNum) {
				retval+="Appointment.ProvNum does not match provider.ProvNum.\r\n";
			}
			if(pat.PriProv!=prov.ProvNum) {
				retval+="Patient.ProvNum does not match provider.ProvNum.\r\n";
			}
			if(retval.Length>0) {
				Assert.Fail("Test 9: "+retval);
			}
		}

		///<summary>Test 10: EcwOldTight,EcwOldFull,EcwTight,HL7DefEcwFull: Use the PV1 segment to insert new provider and set appropriate fields as in Test 9.  EcwOldStandalone,HL7DefEcwStandalone: SIU messages are not processed in Standalone mode.</summary>
		private void Test10(HL7TestInterfaceEnum hl7TestInterfaceEnum) {
			string msgtext=@"MSH|^~\&|||||20120901100000||SIU^S12||P|2.3"+"\r\n"
				+"SCH|100|100|||||Checkup and Fillings||||^^2400^20120902100000^20120902104000||||||||||||||\r\n"
				+"PID|1|30||A300|Smith2^Jane2^N||19760205|Male||White|421 Main St^Apt 17^Dallas^OR^97338||5035554045|5035554234||Married|||222334444|||Standard\r\n"
				+"PV1|1||||||DOC2^Lexington^Sarah^J||||||||||||";
			MessageHL7 msg=new MessageHL7(msgtext);
			try {
				switch(hl7TestInterfaceEnum) {
					case HL7TestInterfaceEnum.EcwOldTight:
					case HL7TestInterfaceEnum.EcwOldFull:
						EcwSIU.ProcessMessage(msg,false);
						break;
					case HL7TestInterfaceEnum.EcwOldStandalone:
					case HL7TestInterfaceEnum.HL7DefEcwStandalone:
						return;//Test 10: Passed.
					default:
						MessageParser.Process(msg,false);
						break;
				}
			}
			catch(Exception ex) {
				Assert.Fail("Test 10: Message processing error: "+ex);
			}
			Patient pat=null;
			pat=Patients.GetPat(30);
			if(pat==null) {
				Assert.Fail("Test 10: Couldn't locate patient.");
			}
			Provider prov=Providers.GetProvByEcwID("DOC2");
			if(prov==null) {
				Assert.Fail("Test 10: Couldn't locate provider.");
			}
			Appointment apt=Appointments.GetOneApt(100);
			if(apt==null) {
				Assert.Fail("Test 10: Couldn't locate appointment.");
			}
			
			string retval="";
			if(prov.Abbr!="DOC2") {
				retval+="Provider.Abbr is not 'DOC2'.\r\n";
			}
			if(prov.LName!="Lexington") {
				retval+="Provider.LName is not 'Lexington'.\r\n";
			}
			if(prov.FName!="Sarah") {
				retval+="Provider.FName is not 'Sarah'.\r\n";
			}
			if(prov.MI!="J") {
				retval+="Provider.MI is not 'J'.\r\n";
			}
			if(apt.ProvNum!=prov.ProvNum) {
				retval+="Appointment.ProvNum does not match provider.ProvNum.\r\n";
			}
			if(pat.PriProv!=prov.ProvNum) {
				retval+="Patient.ProvNum does not match provider.ProvNum.\r\n";
			}
			if(retval.Length>0) {
				Assert.Fail("Test 10: "+retval);
			}
		}

		///<summary>Test 11: EcwOldTight,EcwOldFull,EcwTight,HL7DefEcwFull: Add 2 D0230 procedures, a D0150 procedure, and a D2332 procedure all with complete status and ProcDate='2012-09-06' to patient with PatNum=10.  Set the D2332 procedurelog.ToothNum=26 and procedurelog.Surf=MID.  Make sure the procedurelog.ProcFee for D0230 is 20.00, the D0150 is 75.00, and the D2332 is 150.00.  Using DOC1 provider from TEST 9, schedule appointment for patient 10 with appointment.AptNum=500 and appointment.ProvNum=provider.ProvNum and attach the four procedures.  Create a DFT message for this patient, provider, appointment, and procedures.  EcwOldStandalone,HL7DefEcwStandalone: DFT messages are not created in Standalone mode.</summary>
		private void Test11(HL7TestInterfaceEnum hl7TestInterfaceEnum) {
			if(hl7TestInterfaceEnum==HL7TestInterfaceEnum.EcwOldStandalone 
				|| hl7TestInterfaceEnum==HL7TestInterfaceEnum.HL7DefEcwStandalone) 
			{
				return;//Test 11: Passed.
			}
			ProcedureCodeT.AddIfNotPresent("D0150");
			ProcedureCodeT.AddIfNotPresent("D0230");
			ProcedureCodeT.AddIfNotPresent("D2332");
			List<Procedure> procList=new List<Procedure>();
			//Add the 4 procs to the procedurelog table on the correct date, for the correct appointment and patient and provider.
			Procedure proc=new Procedure();
			proc.AptNum=500;
			proc.PatNum=10;
			proc.ProcDate=new DateTime(2012,09,06);
			proc.CodeNum=ProcedureCodeT.GetCodeNum("D0150");
			proc.ProcStatus=ProcStat.C;
			if(Providers.GetProvByEcwID("DOC1")==null) {
				Assert.Fail("Test 11: Couldn't locate provider.");
			}
			proc.ProvNum=Providers.GetProvByEcwID("DOC1").ProvNum;
			proc.ProcFee=75.00;
			proc.DiagnosticCode="";
			Procedures.Insert(proc);
			procList.Add(proc);
			proc=new Procedure();
			proc.AptNum=500;
			proc.PatNum=10;
			proc.ProcDate=new DateTime(2012,09,06);
			proc.CodeNum=ProcedureCodeT.GetCodeNum("D0230");
			proc.ProcStatus=ProcStat.C;
			proc.ProvNum=Providers.GetProvByEcwID("DOC1").ProvNum;
			proc.ProcFee=20.00;
			proc.DiagnosticCode="";
			Procedures.Insert(proc);
			procList.Add(proc);
			Procedures.Insert(proc);
			procList.Add(proc);
			proc=new Procedure();
			proc.AptNum=500;
			proc.PatNum=10;
			proc.ProcDate=new DateTime(2012,09,06);
			proc.CodeNum=ProcedureCodeT.GetCodeNum("D2332");
			proc.ProcStatus=ProcStat.C;
			proc.ProvNum=Providers.GetProvByEcwID("DOC1").ProvNum;
			proc.ToothNum="26";
			proc.Surf="MID";
			proc.ProcFee=150.00;
			proc.DiagnosticCode="";
			Procedures.Insert(proc);
			procList.Add(proc);
			Appointment apt=new Appointment();
			apt.AptNum=500;
			apt.AptDateTime=new DateTime(2012,09,06,10,0,0);
			apt.PatNum=10;
			apt.ProvNum=Providers.GetProvByEcwID("DOC1").ProvNum;
			long aptNum=Appointments.InsertIncludeAptNum(apt,true);
			long provNum=apt.ProvNum;
			Patient pat=Patients.GetPat(10);
			if(pat==null) {
				Assert.Fail("Test 11: Couldn't locate patient.");
			}
			Patient oldPat=pat.Copy();
			pat.PriProv=Providers.GetProvByEcwID("DOC1").ProvNum;
			Patients.Update(pat,oldPat);
			Patient guar=Patients.GetPat(11);
			if(guar==null) {
				Assert.Fail("Test 11: Couldn't locate guarantor.");
			}
			MessageHL7 msg=null;
			try {
				switch(hl7TestInterfaceEnum) {
					//EcwOldStandalone and HL7DefEcwStandalone were handled higher up
					case HL7TestInterfaceEnum.EcwOldFull:
					case HL7TestInterfaceEnum.EcwOldTight:
						OpenDentBusiness.HL7.EcwDFT dft=new OpenDentBusiness.HL7.EcwDFT();
						dft.InitializeEcw(aptNum,provNum,pat,"Test Message","treatment",false,procList);
						msg=new MessageHL7(dft.GenerateMessage());
						break;
					case HL7TestInterfaceEnum.HL7DefEcwFull:
					case HL7TestInterfaceEnum.HL7DefEcwTight:
						msg=new MessageHL7(OpenDentBusiness.HL7.MessageConstructor.GenerateDFT(procList,EventTypeHL7.P03,pat,guar,aptNum,"treatment","Test Message").ToString());
						//msg will be null if there's not DFT defined for the def.  Should handle results for those defs higher up
						break;
					default:
						throw new Exception("interface not found.");
				}
			}
			catch(Exception ex) {
				Assert.Fail("Test 11: Message creation error. "+ex+".");
			}
			string provField="";
			string msgStructure="";
			switch(hl7TestInterfaceEnum) {
				case HL7TestInterfaceEnum.EcwOldFull:
				case HL7TestInterfaceEnum.EcwOldTight:
					provField="DOC1^Albert, Brian S^^";
					break;
				default:
					provField="DOC1^Albert^Brian^S";
					msgStructure="^DFT_P03";
					break;
			}
			string msgtext=@"MSH|^~\&|OD||ECW||"+msg.Segments[0].GetFieldFullText(6)+"||DFT^P03"+msgStructure+"|"+msg.Segments[0].GetFieldFullText(9)+"|P|2.3\r\n"
				+"EVN|P03|"+msg.Segments[1].GetFieldFullText(2)+"|\r\n"
				+"PID|1|A11|10||Smith^Jane^N||19760205|F||White|421 Main St^Apt 17^Dallas^OR^97338||5035554045|5035554234||Married|||111224444|||\r\n"
				+"PV1|||||||"+provField+"||||||||||||500|||||||||||||||||||||||||||||||\r\n"
				+"FT1|1|||20120906|20120906|CG||||1.0||||||||||"+provField+"|"+provField+"|75.00|||D0150|^\r\n"
				+"FT1|2|||20120906|20120906|CG||||1.0||||||||||"+provField+"|"+provField+"|20.00|||D0230|^\r\n"
				+"FT1|3|||20120906|20120906|CG||||1.0||||||||||"+provField+"|"+provField+"|20.00|||D0230|^\r\n"
				+"FT1|4|||20120906|20120906|CG||||1.0||||||||||"+provField+"|"+provField+"|150.00|||D2332|26^MID\r\n"
				+"ZX1|6|PDF|PATHOLOGY^Pathology Report^L|treatment|Test Message";
			MessageHL7 correctMsg=new MessageHL7(msgtext);
			CompareMsgs(msg,correctMsg);
		}

		///<summary>Test 12: EcwOldTight,EcwOldFull,EcwTight,HL7DefEcwFull: Create message with multiple providers.  Change the provider for the D2332 procedure on the appointment with AptNum=500 to DOC2 from Test 10.  Create DFT message again for this patient, provider, appointment, and procedures and verify that the FT1 segment for that procedure lists the correct provider.  EcwOldStandalone,HL7DefEcwStandalone: DFT messages are not created in Standalone mode.</summary>
		private void Test12(HL7TestInterfaceEnum hl7TestInterfaceEnum) {
			if(hl7TestInterfaceEnum==HL7TestInterfaceEnum.EcwOldStandalone 
				|| hl7TestInterfaceEnum==HL7TestInterfaceEnum.HL7DefEcwStandalone) 
			{
				return;//Test 12: Passed.
			}
			long aptNum=500;
			ProcedureCodeT.AddIfNotPresent("D2332");
			Procedure proc=new Procedure();
			List<Procedure> procList=Procedures.GetProcsForSingle(aptNum,false);
			if(procList==null) {
				Assert.Fail("Test 12: Couldn't locate procedures for appointment.");
			}
			for(int i=0;i<procList.Count;i++) {
				if(procList[i].CodeNum==ProcedureCodeT.GetCodeNum("D2332")) {
					proc=procList[i];
					break;
				}
			}
			if(proc==null) {
				Assert.Fail("Test 12: Couldn't locate procedure D2332.");
			}
			Procedure oldProc=proc.Copy();
			Provider prov2=Providers.GetProvByEcwID("DOC2");
			if(prov2==null) {
				Assert.Fail("Test 12: Couldn't locate DOC2 provider.");
			}
			proc.ProvNum=prov2.ProvNum;
			Procedures.Update(proc,oldProc);
			Patient pat=Patients.GetPat(10);
			Patient guar=Patients.GetPat(11);
			if(pat==null) {
				Assert.Fail("Test 12: Couldn't locate patient.");
			}
			if(guar==null) {
				Assert.Fail("Test 12: Couldn't locate guarantor.");
			}
			Provider prov=Providers.GetProvByEcwID("DOC1");
			if(prov==null) {
				Assert.Fail("Test 12: Couldn't locate DOC1 provider.");
			}
			long provNum=prov.ProvNum;
			MessageHL7 msg=null;
			try {
				switch(hl7TestInterfaceEnum) {
					//EcwOldStandalone and HL7DefEcwStandalone were handled higher up
					case HL7TestInterfaceEnum.EcwOldFull:
					case HL7TestInterfaceEnum.EcwOldTight:
						OpenDentBusiness.HL7.EcwDFT dft=new OpenDentBusiness.HL7.EcwDFT();
						dft.InitializeEcw(aptNum,provNum,pat,"Test Message","treatment",false,procList);
						msg=new MessageHL7(dft.GenerateMessage());
						break;
					case HL7TestInterfaceEnum.HL7DefEcwFull:
					case HL7TestInterfaceEnum.HL7DefEcwTight:
						msg=new MessageHL7(OpenDentBusiness.HL7.MessageConstructor.GenerateDFT(procList,EventTypeHL7.P03,pat,guar,aptNum,"treatment","Test Message").ToString());
						//msg will be null if there's not DFT defined for the def.  Should handle results for those defs higher up
						break;
					default:
						throw new Exception("interface not found.");
				}
			}
			catch(Exception ex) {
				Assert.Fail("Test 12: Message creation error. "+ex+".");
			}
			string provField="";
			string provField2="";
			string msgStructure="";
			switch(hl7TestInterfaceEnum) {
				case HL7TestInterfaceEnum.EcwOldFull:
				case HL7TestInterfaceEnum.EcwOldTight:
					provField="DOC1^Albert, Brian S^^";
					provField2="DOC2^Lexington^Sarah^J";
					break;
				default:
					provField="DOC1^Albert^Brian^S";
					provField2="DOC2^Lexington^Sarah^J";
					msgStructure="^DFT_P03";
					break;
			}
			string msgtext=@"MSH|^~\&|OD||ECW||"+msg.Segments[0].GetFieldFullText(6)+"||DFT^P03"+msgStructure+"|"+msg.Segments[0].GetFieldFullText(9)+"|P|2.3\r\n"
			  +"EVN|P03|"+msg.Segments[1].GetFieldFullText(2)+"|\r\n"
			  +"PID|1|A11|10||Smith^Jane^N||19760205|F||White|421 Main St^Apt 17^Dallas^OR^97338||5035554045|5035554234||Married|||111224444|||\r\n"
			  +"PV1|||||||"+provField+"||||||||||||500|||||||||||||||||||||||||||||||\r\n"
			  +"FT1|1|||20120906|20120906|CG||||1.0||||||||||"+provField+"|"+provField+"|75.00|||D0150|^\r\n"
			  +"FT1|2|||20120906|20120906|CG||||1.0||||||||||"+provField+"|"+provField+"|20.00|||D0230|^\r\n"
			  +"FT1|3|||20120906|20120906|CG||||1.0||||||||||"+provField+"|"+provField+"|20.00|||D0230|^\r\n"
			  +"FT1|4|||20120906|20120906|CG||||1.0||||||||||"+provField2+"|"+provField2+"|150.00|||D2332|26^MID\r\n"
			  +"ZX1|6|PDF|PATHOLOGY^Pathology Report^L|treatment|Test Message";
			MessageHL7 correctMsg=new MessageHL7(msgtext);
			CompareMsgs(msg,correctMsg);
		}

		///<summary>Test 13: EcwOldTight,EcwOldFull,EcwTight,HL7DefEcwFull: Truncate D codes to Dxxxx, if not a D code don't truncate: Insert a procedure D2332A, with treatment area surface and ProcFee=200.00.  Insert another procedure 2332AA with treatment area surface and ProcFee=250.00.  Add a D2332A on tooth 1 with surfaces MOD, and a 2332AA on tooth 2 with surfaces MOD to patient's chart.  Schedule an appointment for patient with AptNum=600, appointment.ProvNum=ProvNum for provider DOC1 and attach the two procedures.  Create a DFT message for these procedures, appointment, patient, and provider and verify that the D2332A gets truncated to D2332 and the 2332AA doesn't get changed.  EcwOldStandalone,HL7DefEcwStandalone: DFT messages are not created in Standalone mode.</summary>
		private void Test13(HL7TestInterfaceEnum hl7TestInterfaceEnum) {
			if(hl7TestInterfaceEnum==HL7TestInterfaceEnum.EcwOldStandalone 
				|| hl7TestInterfaceEnum==HL7TestInterfaceEnum.HL7DefEcwStandalone) {
				return;//Test 13: Passed.
			}
			Provider prov=Providers.GetProvByEcwID("DOC1");
			if(prov==null) {
				Assert.Fail("Test 13: Couldn't locate provider.");
			}
			Appointment apt=new Appointment();
			apt.AptNum=600;
			apt.AptDateTime=new DateTime(2012,09,06,10,0,0);
			apt.PatNum=10;
			apt.ProvNum=prov.ProvNum;
			Appointments.InsertIncludeAptNum(apt,true);
			ProcedureCode code=ProcedureCodes.GetFirstOrDefault(x => x.ProcCode=="D2332A");
			if(code==null) {
				code=new ProcedureCode();
				code.ProcCode="D2332A";
				ProcedureCodes.Insert(code);
			}
			code=ProcedureCodes.GetFirstOrDefault(x => x.ProcCode=="2332AA");
			if(code==null) {
				code=new ProcedureCode();
				code.ProcCode="2332AA";
				ProcedureCodes.Insert(code);
			}
			List<Procedure> procList=new List<Procedure>();
			ProcedureCodes.RefreshCache();
			//Add the 2 procs to the procedurelog table on the correct date, for the correct appointment and patient and provider.
			Procedure proc=new Procedure();
			proc.AptNum=600;
			proc.PatNum=10;
			proc.ProcDate=new DateTime(2012,09,06);
			proc.CodeNum=ProcedureCodeT.GetCodeNum("D2332A");
			proc.ProcStatus=ProcStat.C;
			proc.ProvNum=prov.ProvNum;
			proc.ProcFee=200.00;
			proc.ToothNum="1";
			proc.Surf="MOD";
			proc.DiagnosticCode="";
			Procedures.Insert(proc);
			procList.Add(proc);
			proc=new Procedure();
			proc.AptNum=600;
			proc.PatNum=10;
			proc.ProcDate=new DateTime(2012,09,06);
			proc.CodeNum=ProcedureCodeT.GetCodeNum("2332AA");
			proc.ProcStatus=ProcStat.C;
			proc.ProvNum=prov.ProvNum;
			proc.ProcFee=250.00;
			proc.ToothNum="2";
			proc.Surf="MOD";
			proc.DiagnosticCode="";
			Procedures.Insert(proc);
			procList.Add(proc);
			Patient pat=Patients.GetPat(10);
			if(pat==null) {
				Assert.Fail("Test 13: Couldn't locate patient.");
			}
			Patient guar=Patients.GetPat(11);
			if(guar==null) {
				Assert.Fail("Test 13: Couldn't locate guarantor.");
			}
			MessageHL7 msg=null;
			try {
				switch(hl7TestInterfaceEnum) {
					//EcwOldStandalone and HL7DefEcwStandalone were handled higher up
					case HL7TestInterfaceEnum.EcwOldFull:
					case HL7TestInterfaceEnum.EcwOldTight:
						OpenDentBusiness.HL7.EcwDFT dft=new OpenDentBusiness.HL7.EcwDFT();
						dft.InitializeEcw(apt.AptNum,prov.ProvNum,pat,"Test Message","treatment",false,procList);
						msg=new MessageHL7(dft.GenerateMessage());
						break;
					case HL7TestInterfaceEnum.HL7DefEcwFull:
					case HL7TestInterfaceEnum.HL7DefEcwTight:
						msg=new MessageHL7(OpenDentBusiness.HL7.MessageConstructor.GenerateDFT(procList,EventTypeHL7.P03,pat,guar,apt.AptNum,"treatment","Test Message").ToString());
						//msg will be null if there's not DFT defined for the def.  Should handle results for those defs higher up
						break;
					default:
						throw new Exception("interface not found.");
				}
			}
			catch(Exception ex) {
				Assert.Fail("Test 13: Message creation error. "+ex+".");
			}
			string provField="";
			string msgStructure="";
			switch(hl7TestInterfaceEnum) {
				case HL7TestInterfaceEnum.EcwOldFull:
				case HL7TestInterfaceEnum.EcwOldTight:
					provField="DOC1^Albert, Brian S^^";
					break;
				default:
					provField="DOC1^Albert^Brian^S";
					msgStructure="^DFT_P03";
					break;
			}
			string msgtext=@"MSH|^~\&|OD||ECW||"+msg.Segments[0].GetFieldFullText(6)+"||DFT^P03"+msgStructure+"|"+msg.Segments[0].GetFieldFullText(9)+"|P|2.3\r\n"
			  +"EVN|P03|"+msg.Segments[1].GetFieldFullText(2)+"|\r\n"
			  +"PID|1|A11|10||Smith^Jane^N||19760205|F||White|421 Main St^Apt 17^Dallas^OR^97338||5035554045|5035554234||Married|||111224444|||\r\n"
			  +"PV1|||||||"+provField+"||||||||||||600|||||||||||||||||||||||||||||||\r\n"
			  +"FT1|1|||20120906|20120906|CG||||1.0||||||||||"+provField+"|"+provField+"|200.00|||D2332|1^MOD\r\n"
			  +"FT1|2|||20120906|20120906|CG||||1.0||||||||||"+provField+"|"+provField+"|250.00|||2332AA|2^MOD\r\n"
			  +"ZX1|6|PDF|PATHOLOGY^Pathology Report^L|treatment|Test Message";
			MessageHL7 correctMsg=new MessageHL7(msgtext);
			CompareMsgs(msg,correctMsg);
		}

		///<summary>Test 15: EcwOldTight,EcwOldFull,EcwTight,HL7DefEcwFull: Optional AIG segment is included, so use that segment to insert a new provider.  Make sure this new provider has the correct fee schedule associated, based on the non-hidden fee schedule with the lowest item order.  No need to test PV1 segment for the same fee schedule issue since the code to process the provider information is the same as for the AIG segment.  EcwOldStandalone,HL7DefEcwStandalone: SIU messages are not processed in Standalone mode.</summary>
		private void Test15(HL7TestInterfaceEnum hl7TestInterfaceEnum) {
			string msgtext=@"MSH|^~\&|||||20121213100000||SIU^S12||P|2.3"+"\r\n"
				+"SCH|1500|1500|||||Checkup and Fillings||||^^2400^20121214100000^20121214104000||||||||||||||\r\n"
				+"PID|1|30||A300|Smith2^Jane2^N||19760205|Female||White|421 Main St^Apt 17^Dallas^OR^97338||5035554045|5035554234||Married|||222334444|||Standard\r\n"
				+"AIG|1||DOC3^Jones, Tina W||||";
			MessageHL7 msg=new MessageHL7(msgtext);
			try {
				switch(hl7TestInterfaceEnum) {
					case HL7TestInterfaceEnum.EcwOldFull:
					case HL7TestInterfaceEnum.EcwOldTight:
						EcwSIU.ProcessMessage(msg,false);
						break;
					case HL7TestInterfaceEnum.EcwOldStandalone:
					case HL7TestInterfaceEnum.HL7DefEcwStandalone:
						return;//Test 15: Passed.
					default:
						MessageParser.Process(msg,false);
						break;
				}
			}
			catch(Exception ex) {
				Assert.Fail("Test 15: Message processing error: "+ex);
			}
			Provider prov=Providers.GetProvByEcwID("DOC3");
			if(prov==null) {
				Assert.Fail("Test 15: Couldn't locate provider.");
			}
			if(prov.FeeSched!=FeeScheds.GetFirst(true).FeeSchedNum) {
				Assert.Fail("Test 15: Provider fee schedule is not set to "+FeeScheds.GetFirst(true).Description+".");
			}
		}

		///<summary>Compares the pat and guar to correctPat and correctGuar to make sure every field matches.</summary>
		private void CompareGuarAndPat(Patient pat,Patient correctPat,Patient guar,Patient correctGuar,HL7TestInterfaceEnum hl7TestInterfaceEnum
			,List<PatientRace> listCorrectRaces=null)
		{
			string retval="";
			if(pat.Guarantor!=guar.PatNum) {
				retval+="Patient inserted isn't assigned to the guarantor specified in the GT1 segment.\r\n";
			}
			if(guar.Guarantor!=guar.PatNum) {
				retval+="Guarantor inserted should have self for guarantor.\r\n";
			}
			if(pat.PriProv!=correctPat.PriProv) {
				retval+="Patient PriProv should be "+Providers.GetAbbr(correctPat.PriProv)+" and is "+Providers.GetAbbr(correctPat.PriProv)+".\r\n";
				retval+="Guarantor PriProv should be "+Providers.GetAbbr(correctGuar.PriProv)+" and is "+Providers.GetAbbr(guar.PriProv)+".\r\n";
			}
			if(pat.BillingType!=correctPat.BillingType || guar.BillingType!=correctGuar.BillingType) {
				retval+="Patient BillingType should be "+Defs.GetName(DefCat.BillingTypes,correctPat.BillingType)+" and is "+Defs.GetName(DefCat.BillingTypes,pat.BillingType)+".\r\n";
				retval+="Guarantor BillingType should be "+Defs.GetName(DefCat.BillingTypes,correctGuar.BillingType)+" and is "+Defs.GetName(DefCat.BillingTypes,guar.BillingType)+".\r\n";
			}
			if(pat.ChartNumber!=correctPat.ChartNumber || guar.ChartNumber!=correctGuar.ChartNumber) {
				retval+="Patient ChartNumber should be "+correctPat.ChartNumber+" and is "+pat.ChartNumber+".\r\n";
				retval+="Guarantor ChartNumber should be "+correctGuar.ChartNumber+" and is "+guar.ChartNumber+".\r\n";
			}
			if(hl7TestInterfaceEnum==HL7TestInterfaceEnum.EcwOldFull
				|| hl7TestInterfaceEnum==HL7TestInterfaceEnum.EcwOldTight
				|| hl7TestInterfaceEnum==HL7TestInterfaceEnum.HL7DefEcwFull
				|| hl7TestInterfaceEnum==HL7TestInterfaceEnum.HL7DefEcwTight)
			{
				if(pat.PatNum!=correctPat.PatNum || guar.PatNum!=correctGuar.PatNum) {
					retval+="Patient PatNum should be "+correctPat.PatNum.ToString()+" and is "+pat.PatNum.ToString()+".\r\n";
					retval+="Guarantor PatNum should be "+correctGuar.PatNum.ToString()+" and is "+correctGuar.PatNum.ToString()+".\r\n";
				}
			}
			if(pat.LName!=correctPat.LName || guar.LName!=correctPat.LName) {
				retval+="Patient LName should be "+correctPat.LName+" and is "+pat.LName+".\r\n";
				retval+="Guarantor LName should be "+correctGuar.LName+" and is "+guar.LName+".\r\n";
			}
			if(pat.FName!=correctPat.FName || guar.FName!=correctGuar.FName) {
				retval+="Patient FName should be "+correctPat.FName+" and is "+pat.FName+".\r\n";
				retval+="Guarantor FName should be "+correctGuar.FName+" and is "+guar.FName+".\r\n";
			}
			if(pat.MiddleI!=correctPat.MiddleI || guar.MiddleI!=correctGuar.MiddleI) {
				retval+="Patient MiddleI should be "+correctPat.MiddleI+" and is "+pat.MiddleI+".\r\n";
				retval+="Guarantor MiddleI should be "+correctGuar.MiddleI+" and is "+guar.MiddleI+".\r\n";
			}
			if(pat.Birthdate!=correctPat.Birthdate || guar.Birthdate!=correctGuar.Birthdate) {
				retval+="Patient Birthdate should be "+correctPat.Birthdate.ToString()+" and is "+pat.Birthdate.ToString()+".\r\n";
				retval+="Guarantor Birthdate should be "+correctGuar.Birthdate.ToString()+" and is "+guar.Birthdate.ToString()+".\r\n";
			}
			if(pat.Gender!=correctPat.Gender || guar.Gender!=correctGuar.Gender) {
				retval+="Patient Gender should be "+correctPat.Gender.ToString()+" and is "+pat.Gender.ToString()+".\r\n";
				retval+="Guarantor Gender should be "+correctGuar.Gender.ToString()+" and is "+guar.Gender.ToString()+".\r\n";
			}
			List<PatientRace> listPatRace=PatientRaces.GetForPatient(pat.PatNum);
			List<PatientRace> listPatCorRace=PatientRaces.GetForPatient(correctPat.PatNum);
			//The "correct" patient might have a PatNum of 0 (standalone) so we should prefer to use the passed in list if provided.
			if(listCorrectRaces!=null) {
				listPatCorRace=listCorrectRaces;
			}
			if(listPatRace.Count!=listPatCorRace.Count) {
				retval+="Patient Races do not match.\r\n";
			}
			else {
				for(int i=0;i<listPatRace.Count;i++) {
					if(!listPatCorRace.Any(x => x.CdcrecCode==listPatRace[i].CdcrecCode)) {
						retval+="Patient Races do not match.\r\n";
						break;
					}
				}
			}
			//if(PatientRaces.GetForPatient(pat.PatNum)!=correctPat.Race) {
			//	retval+="Patient Races should be "+listCorRaces.ToString()+" and is "+listPatRaces.ToString()+".\r\n";
			//}
			if(pat.Address!=correctPat.Address || pat.Address2!=correctPat.Address2 || pat.City!=correctPat.City || pat.State!=correctPat.State || pat.Zip!=correctPat.Zip) {
				retval+="Patient Address should be "+correctPat.Address+" "+correctPat.Address2+" "+correctPat.City+", "+correctPat.State+" "+correctPat.Zip;
				retval+=" and is "+pat.Address+" "+pat.Address2+" "+pat.City+", "+pat.State+" "+pat.Zip+".\r\n";
			}
			if(guar.Address!=correctGuar.Address || guar.Address2!=correctGuar.Address2 || guar.City!=correctGuar.City || guar.State!=correctGuar.State || guar.Zip!=correctGuar.Zip) {
				retval+="Guarantor Address should be "+correctGuar.Address+" "+correctGuar.Address2+" "+correctGuar.City+", "+correctGuar.State+" "+correctGuar.Zip;
				retval+=" and is "+guar.Address+" "+guar.Address2+" "+guar.City+", "+guar.State+" "+guar.Zip+".\r\n";
			}
			if(pat.HmPhone!=correctPat.HmPhone || guar.HmPhone!=correctGuar.HmPhone) {
				retval+="Patient HmPhone should be "+correctPat.HmPhone+" and is "+pat.HmPhone+".\r\n";
				retval+="Guarantor HmPhone should be "+correctGuar.HmPhone+" and is "+guar.HmPhone+".\r\n";
			}
			if(pat.WkPhone!=correctPat.WkPhone || guar.WkPhone!=correctGuar.WkPhone) {
				retval+="Patient WkPhone should be "+correctPat.WkPhone+" and is "+pat.WkPhone+".\r\n";
				retval+="Guarantor WkPhone should be "+correctGuar.WkPhone+" and is "+guar.WkPhone+".\r\n";
			}
			if(pat.SSN!=correctPat.SSN || guar.SSN!=correctGuar.SSN) {
				retval+="Patient SSN should be "+correctPat.SSN+" and is "+pat.SSN+".\r\n";
				retval+="Guarantor SSN should be "+correctGuar.SSN+" and is "+guar.SSN+".\r\n";
			}
			if(pat.FeeSched!=correctPat.FeeSched) {
				retval+="Patient FeeSched should be "+FeeScheds.GetDescription(correctPat.FeeSched)+" and is "+FeeScheds.GetDescription(pat.FeeSched)+".\r\n";
			}
			if(!string.IsNullOrEmpty(retval)) {
				Assert.Fail(retval);
			}
		}

		/// <summary>Compare an HL7 message to a correct HL7 message and return any mismatched fields.</summary>
		private void CompareMsgs(MessageHL7 msg,MessageHL7 correctMsg) {
			string retval="";
			for(int s=0;s<msg.Segments.Count;s++) {
				for(int f=0;f<msg.Segments[s].Fields.Count;f++) {
					if(msg.Segments[s].Fields[f].ToString()!=correctMsg.Segments[s].Fields[f].ToString()) {
						retval+="Expected value in "+correctMsg.Segments[s].Name.ToString()+" field "+f+": "+correctMsg.Segments[s].Fields[f].ToString()
							+"\r\nCurrent value in "+msg.Segments[s].Name.ToString()+" field "+f+": "+msg.Segments[s].Fields[f].ToString()+".\r\n";
					}
				}
			}
			if(!string.IsNullOrEmpty(retval)) {
				Assert.Fail(retval);
			}
		}

		///<summary>Keep these in alphabetical order</summary>
		private enum HL7TestInterfaceEnum {
			EcwOldFull,
			EcwOldStandalone,
			EcwOldTight,
			HL7DefEcwFull,
			HL7DefEcwStandalone,
			HL7DefEcwTight
		}

		#endregion

		///<summary></summary>
		[TestMethod]
		public void MessageHL7_TryGetSegmentOrder_OptionalSegmentPresent() {
			HL7DefMessage hl7defmsg=new HL7DefMessage();
			hl7defmsg.AddSegment(new HL7DefSegment(),0,SegmentNameHL7.MSH);
			hl7defmsg.AddSegment(new HL7DefSegment(),1,SegmentNameHL7.SCH);
			hl7defmsg.AddSegment(new HL7DefSegment(),2,true,true,SegmentNameHL7.NTE);
			hl7defmsg.AddSegment(new HL7DefSegment(),3,SegmentNameHL7.PID);
			hl7defmsg.AddSegment(new HL7DefSegment(),4,false,true,SegmentNameHL7.PV1);
			string msgtext=@"MSH|^~\&|||||20120901100000||SIU^S12||P|2.3
SCH|100|100|||||Checkup||||^^1200^20120901100000^20120901102000||||||||||||||
NTE|1||||||||||||||||||
PID|1|30||A30|Smiths2^Jan2^L||19760210|Male||Hispanic|||5305554045|5305554234||Single|||222334444|||Standard
PV1|1||||||DOC2^Lexington^Sarah^J||||||||||||";
			//Create an HL7 message with the optional NTE segment present.
			MessageHL7 hl7withNTE=new MessageHL7(msgtext);
			int segmentOrder;
			int segmentDefOrder;
			if(!hl7withNTE.TryGetSegmentOrder(SegmentNameHL7.PID,hl7defmsg,out segmentOrder,out segmentDefOrder)) {
				Assert.Fail("PID segment not found.");
			}
			Assert.AreEqual(3,segmentOrder);
			Assert.AreEqual(3,segmentDefOrder);
		}

		///<summary></summary>
		[TestMethod]
		public void MessageHL7_TryGetSegmentOrder_OptionalSegmentRepeats() {
			HL7DefMessage hl7defmsg=new HL7DefMessage();
			hl7defmsg.AddSegment(new HL7DefSegment(),0,SegmentNameHL7.MSH);
			hl7defmsg.AddSegment(new HL7DefSegment(),1,SegmentNameHL7.SCH);
			hl7defmsg.AddSegment(new HL7DefSegment(),2,true,true,SegmentNameHL7.NTE);
			hl7defmsg.AddSegment(new HL7DefSegment(),3,SegmentNameHL7.PID);
			hl7defmsg.AddSegment(new HL7DefSegment(),4,false,true,SegmentNameHL7.PV1);
			string msgtext=@"MSH|^~\&|||||20120901100000||SIU^S12||P|2.3
SCH|100|100|||||Checkup||||^^1200^20120901100000^20120901102000||||||||||||||
NTE|1||||||||||||||||||
NTE|2||||||||||||||||||
NTE|3||||||||||||||||||
NTE|4||||||||||||||||||
PID|1|30||A30|Smiths2^Jan2^L||19760210|Male||Hispanic|||5305554045|5305554234||Single|||222334444|||Standard
PV1|1||||||DOC2^Lexington^Sarah^J||||||||||||";
			//Create an HL7 message with the optional NTE segment repeating several times.
			MessageHL7 hl7withNTE=new MessageHL7(msgtext);
			int segmentOrder;
			int segmentDefOrder;
			if(!hl7withNTE.TryGetSegmentOrder(SegmentNameHL7.PID,hl7defmsg,out segmentOrder,out segmentDefOrder)) {
				Assert.Fail("PID segment not found.");
			}
			Assert.AreEqual(6,segmentOrder);
			Assert.AreEqual(3,segmentDefOrder);
		}

		///<summary>The segment 'order' should be detected even when optional segments are missing.</summary>
		[TestMethod]
		public void MessageHL7_TryGetSegmentOrder_OptionalSegmentMissing() {
			HL7DefMessage hl7defmsg=new HL7DefMessage();
			hl7defmsg.AddSegment(new HL7DefSegment(),0,SegmentNameHL7.MSH);
			hl7defmsg.AddSegment(new HL7DefSegment(),1,SegmentNameHL7.SCH);
			hl7defmsg.AddSegment(new HL7DefSegment(),2,true,true,SegmentNameHL7.NTE);
			hl7defmsg.AddSegment(new HL7DefSegment(),3,SegmentNameHL7.PID);
			hl7defmsg.AddSegment(new HL7DefSegment(),4,false,true,SegmentNameHL7.PV1);
			string msgtext=@"MSH|^~\&|||||20120901100000||SIU^S12||P|2.3
SCH|100|100|||||Checkup||||^^1200^20120901100000^20120901102000||||||||||||||
PID|1|30||A30|Smiths2^Jan2^L||19760210|Male||Hispanic|||5305554045|5305554234||Single|||222334444|||Standard
PV1|1||||||DOC2^Lexington^Sarah^J||||||||||||";
			//Create an HL7 message that is missing the optional NTE segment.
			MessageHL7 hl7withoutNTE=new MessageHL7(msgtext);
			if(!hl7withoutNTE.TryGetSegmentOrder(SegmentNameHL7.PID,hl7defmsg,out int segmentOrder,out int segmentDefOrder)) {
				Assert.Fail("PID segment not found.");
			}
			Assert.AreEqual(2,segmentOrder);
			Assert.AreEqual(3,segmentDefOrder);
		}

	}
}
