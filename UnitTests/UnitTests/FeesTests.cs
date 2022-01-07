using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;
using CodeBase;

namespace UnitTests.Fees_Tests {
	[TestClass]
	///<summary></summary>
	public class FeesTests:FeeTestBase {

		///<summary>This method will execute only once, just before any tests in this class run.</summary>
		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			FeeTestSetup();
		}

		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			//PrefT.UpdateBool(PrefName.FeesUseCache,false);
		}

		///<summary>This method will execute after each test in this class.</summary>
		[TestCleanup]
		public void TearDownTest() {
		}

		///<summary>This method will execute only once, just after all tests in this class have run.</summary>
		[ClassCleanup]
		public static void TearDownClass() {
			FeeTestTearDown();
		}

		///<summary>Fees logic: #2: Clinic A, B, and C have different standard UCR fees.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Fees_GetFee_ClinicSpecificFees)]
		[Documentation.Description("Fees logic: #2: Clinic A, B, and C have different standard UCR fees.")]
		public void Fees_GetFee_ClinicSpecificFees() {
			Patient pat=PatientT.CreatePatient("58");
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"Standard UCR",false);
			long codeNum=ProcedureCodeT.GetCodeNum("D2750");
			long clinicNum2=ClinicT.CreateClinic("2-58").ClinicNum;
			long clinicNum3=ClinicT.CreateClinic("3-58").ClinicNum;
			long clinicNum1=ClinicT.CreateClinic("1-58").ClinicNum;
			FeeT.CreateFee(feeSchedNum,codeNum,65,clinicNum1,0);
			FeeT.CreateFee(feeSchedNum,codeNum,70,clinicNum2,0);
			FeeT.CreateFee(feeSchedNum,codeNum,75,clinicNum3,0);
			double fee1=Fees.GetFee(codeNum,feeSchedNum,clinicNum1,0).Amount;
			double fee2=Fees.GetFee(codeNum,feeSchedNum,clinicNum2,0).Amount;
			double fee3=Fees.GetFee(codeNum,feeSchedNum,clinicNum3,0).Amount;
			Assert.AreEqual(fee1,65);
			Assert.AreEqual(fee2,70);
			Assert.AreEqual(fee3,75);
		}

		///<summary>Fees logic: #3: Dr. Jane and Dr. George have different standard UCR fees. Dr. George's works in two clinics (A and B),
		///and his standard fees are different depending on the clinic.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Fees_GetFee_ClinicAndProviderSpecificFees)]
		[Documentation.Description(@"Fees logic: #3: Dr. Jane and Dr. George have different standard UCR fees. Dr. George's works in two clinics (A and B),and his standard fees are different depending on the clinic.")]
		public void Fees_GetFee_ClinicAndProviderSpecificFees() {
			Patient pat=PatientT.CreatePatient("59");
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"Standard",false);
			long codeNum=ProcedureCodeT.GetCodeNum("D2750");
			long provNum1=ProviderT.CreateProvider("1-59");
			long provNum2=ProviderT.CreateProvider("2-59");
			long clinicNum1=ClinicT.CreateClinic("1-59").ClinicNum;
			long clinicNum2=ClinicT.CreateClinic("2-59").ClinicNum;
			FeeT.CreateFee(feeSchedNum,codeNum,80,clinicNum1,provNum1);
			FeeT.CreateFee(feeSchedNum,codeNum,85,clinicNum1,provNum2);
			FeeT.CreateFee(feeSchedNum,codeNum,90,clinicNum2,provNum2);
			double fee1=Fees.GetFee(codeNum,feeSchedNum,clinicNum1,provNum1).Amount;
			double fee2=Fees.GetFee(codeNum,feeSchedNum,clinicNum1,provNum2).Amount;
			double fee3=Fees.GetFee(codeNum,feeSchedNum,clinicNum2,provNum2).Amount;
			Assert.AreEqual(fee1,80);
			Assert.AreEqual(fee2,85);
			Assert.AreEqual(fee3,90);
		}

		/// <summary>Create a single fee and get the exact match.</summary>
		[TestMethod]
		public void Fees_GetFee_Exact() {
			Fee expectedFee=CreateSingleFee(MethodBase.GetCurrentMethod().Name,_defaultFeeAmt*_rand.NextDouble(),true,true);
			Fee actualFee=Fees.GetFee(expectedFee.CodeNum,expectedFee.FeeSched,expectedFee.ClinicNum,expectedFee.ProvNum);
			Assert.IsTrue(AreSimilar(expectedFee,actualFee));
		}

		/// <summary>Create a single fee, search the wrong code num, the exact match returns null.</summary>
		[TestMethod]
		public void Fees_GetFee_ExactNotFound() {
			Fee createdFee=CreateSingleFee(MethodBase.GetCurrentMethod().Name,_defaultFeeAmt*_rand.NextDouble(),true,true);
			Assert.IsNull(Fees.GetFee(_listProcCodes.Last().CodeNum,createdFee.FeeSched,createdFee.ClinicNum,createdFee.ProvNum));
		}

		/// <summary></summary>
		[TestMethod]
		public void Fees_GetFee_PartialProvDefaultClinic() {
			Fee expectedFee=CreateSingleFee(MethodBase.GetCurrentMethod().Name,_defaultFeeAmt*_rand.NextDouble(),false,true);
			Clinic clinic=ClinicT.CreateClinic(MethodBase.GetCurrentMethod().Name);
			Fee actualFee=Fees.GetFee(expectedFee.CodeNum,expectedFee.FeeSched,clinic.ClinicNum,expectedFee.ProvNum);
			Assert.IsTrue(AreSimilar(expectedFee,actualFee));
		}

		[TestMethod]
		public void Fees_GetFee_PartialClinicNoProv() {
			Fee expectedFee=CreateSingleFee(MethodBase.GetCurrentMethod().Name,_defaultFeeAmt*_rand.NextDouble(),true,false);
			long provNum=ProviderT.CreateProvider(MethodBase.GetCurrentMethod().Name);
			Fee actualFee=Fees.GetFee(expectedFee.CodeNum,expectedFee.FeeSched,expectedFee.ClinicNum,provNum);
			Assert.IsTrue(AreSimilar(expectedFee,actualFee));
		}

		[TestMethod]
		public void Fees_GetFee_PartialDefaultForCode() {
			string name=MethodBase.GetCurrentMethod().Name;
			Fee expectedFee=CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble());
			Clinic clinic=ClinicT.CreateClinic(name);
			long provNum=ProviderT.CreateProvider(name);
			Fee actualFee=Fees.GetFee(expectedFee.CodeNum,expectedFee.FeeSched,clinic.ClinicNum,provNum);
			Assert.IsTrue(AreSimilar(expectedFee,actualFee));
		}

		[TestMethod]
		public void Fees_GetFee_PartialNotFound() {
			string name=MethodBase.GetCurrentMethod().Name;
			Fee createdFee=CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble());
			Clinic clinic=ClinicT.CreateClinic(name);
			long provNum=ProviderT.CreateProvider(name);
			Assert.IsNull(Fees.GetFee(_listProcCodes.Last().CodeNum,createdFee.FeeSched,clinic.ClinicNum,provNum));
		}

		///<summary>Fees logic: #1: For PPOInsPlan1, Dr. Jones, Dr. Smith, and Dr. Wilson have different fees.</summary>
		[TestMethod]
		[Documentation.Description("Fees logic: #1: For PPOInsPlan1, Dr. Jones, Dr. Smith, and Dr. Wilson have different fees.")]
		[Documentation.Numbering(Documentation.EnumTestNum.Fees_GetFee_ProviderSpecificFees)]
		public void Fees_GetFee_ProviderSpecificFees() {
			Patient pat=PatientT.CreatePatient("57");
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"PPOInsPlan1",false);
			long codeNum=ProcedureCodeT.GetCodeNum("D2750");
			long provNum1=ProviderT.CreateProvider("1-57");
			long provNum2=ProviderT.CreateProvider("2-57");
			long provNum3=ProviderT.CreateProvider("3-57");
			FeeT.CreateFee(feeSchedNum,codeNum,50,0,provNum1);
			FeeT.CreateFee(feeSchedNum,codeNum,55,0,provNum2);
			FeeT.CreateFee(feeSchedNum,codeNum,60,0,provNum3);
			double fee1=Fees.GetFee(codeNum,feeSchedNum,0,provNum1).Amount;
			double fee2=Fees.GetFee(codeNum,feeSchedNum,0,provNum2).Amount;
			double fee3=Fees.GetFee(codeNum,feeSchedNum,0,provNum3).Amount;
			Assert.AreEqual(50,fee1);
			Assert.AreEqual(55,fee2);
			Assert.AreEqual(60,fee3);
		}

		/// <summary>Create a single fee. Test that searching for the wrong fee returns -1 and searching for the correct fee returns the amount.</summary>
		[TestMethod]
		public void Fees_GetAmount() {
			double amt=_defaultFeeAmt * _rand.NextDouble();
			Fee fee=CreateSingleFee(MethodBase.GetCurrentMethod().Name,amt,true,true);
			Assert.AreEqual(-1,Fees.GetAmount(_listProcCodes.Last().CodeNum,fee.FeeSched,fee.ClinicNum,fee.ProvNum));
			Assert.AreEqual(fee.Amount,Fees.GetAmount(fee.CodeNum,fee.FeeSched,fee.ClinicNum,fee.ProvNum),_precision);
		}

		/// <summary>Create a single fee. Test that searching for the wrong fee returns 0 and searching for the correct fee returns the amount.</summary>
		[TestMethod]
		public void Fees_GetAmount0() {
			double amt=_defaultFeeAmt * _rand.NextDouble();
			Fee fee=CreateSingleFee(MethodBase.GetCurrentMethod().Name,amt,true,true);
			Assert.AreEqual(0,Fees.GetAmount0(_listProcCodes.Last().CodeNum,fee.FeeSched,fee.ClinicNum,fee.ProvNum));
			Assert.AreEqual(fee.Amount,Fees.GetAmount0(fee.CodeNum,fee.FeeSched,fee.ClinicNum,fee.ProvNum),_precision);
		}
		
		///<summary>Gets fees for a bunch of fee schedules over Middle Tier.</summary>
		[TestMethod]
		public void Fees_GetByFeeSchedNumsClinicNums_MiddleTier() {
			List<long> listFeeSchedNums=new List<long>();
			long codeNum1=ProcedureCodeT.GetCodeNum("D1110");
			long codeNum2=ProcedureCodeT.GetCodeNum("D1206");
			for(int i=0;i<300;i++) {
				FeeSched feeSched=FeeSchedT.GetNewFeeSched(FeeScheduleType.Normal,"FS"+i);
				FeeT.GetNewFee(feeSched.FeeSchedNum,codeNum1,11);
				FeeT.GetNewFee(feeSched.FeeSchedNum,codeNum2,13);
				listFeeSchedNums.Add(feeSched.FeeSchedNum);
			}
			DataAction.RunMiddleTierMock(() => {
				List<FeeLim> listFees=Fees.GetByFeeSchedNumsClinicNums(listFeeSchedNums,new List<long> { 0 });
				Assert.AreEqual(600,listFees.Count);
			});
		}

		///<summary>Searches for a fee where there are multiple potential matches.</summary>
		[TestMethod]
		public void Fees_GetFee_ManyPossibleMatches() {
			string name=MethodBase.GetCurrentMethod().Name;
			Fee expectedFee=CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble(),true,true);
			CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble(),true,false,expectedFee.FeeSched,expectedFee.CodeNum,expectedFee.ClinicNum);
			CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble(),false,true,expectedFee.FeeSched,expectedFee.CodeNum,0,expectedFee.ProvNum);
			CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble(),false,false,expectedFee.FeeSched,expectedFee.CodeNum);
			Fee actualFee=Fees.GetFee(expectedFee.CodeNum,expectedFee.FeeSched,expectedFee.ClinicNum,expectedFee.ProvNum);
			Assert.IsTrue(AreSimilar(expectedFee,actualFee));
		}

		///<summary>Searches for a fee for a provider and clinic but accepts the fee for the default clinic and no provider due to the fee schedule being
		///a global fee schedule.</summary>
		[TestMethod]
		public void Fees_GetFee_GlobalFeeSched() {
			string name=MethodBase.GetCurrentMethod().Name;
			Fee expectedFee=CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble(),false,false,isGlobalFeeSched: true);
			Fee otherFee1=CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble(),true,false,expectedFee.FeeSched,expectedFee.CodeNum);
			Fee otherFee2=CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble(),false,true,expectedFee.FeeSched,expectedFee.CodeNum);
			CreateSingleFee(name,_defaultFeeAmt*_rand.NextDouble(),true,true,expectedFee.FeeSched,expectedFee.CodeNum);
			Fee actualFee=Fees.GetFee(expectedFee.CodeNum,expectedFee.FeeSched,otherFee1.ClinicNum,otherFee2.ProvNum);
			Assert.IsTrue(AreSimilar(expectedFee,actualFee));
		}

		///<summary>Attest that fees associated to the fee schedule for the first provider in the cache are always returned.</summary>
		[TestMethod]
		public void Fees_GetListFromObjects_ProviderFirst() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("D1110");
			Fee fee=CreateSingleFee(suffix,(_defaultFeeAmt * _rand.NextDouble()),codeNum:procedureCode.CodeNum);
			//Update the database so that provFirst is associated to the new fee schedule we just created.
			Provider provFirst=Providers.GetFirst();
			provFirst.FeeSched=fee.FeeSched;
			ProviderT.Update(provFirst);
			//Since our new fee is associated to the fee schedule of the first provider, it should be returned by GetListFromObjects().
			List<Fee> listFees=Fees.GetListFromObjects(new List<ProcedureCode>(){ procedureCode },
				null,
				null,
				0,
				0,
				0,
				null,
				null,
				null,
				null,
				0);
			Assert.IsTrue(listFees.Exists(x => x.FeeNum==fee.FeeNum));
		}

		///<summary>Attest that fees associated to the fee schedule for the practice provider are always returned.</summary>
		[TestMethod]
		public void Fees_GetListFromObjects_ProviderPracticeDefault() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("D1110");
			Fee fee=CreateSingleFee(suffix,(_defaultFeeAmt * _rand.NextDouble()),hasProv:true,codeNum:procedureCode.CodeNum);
			//Update our new provider so that they are associated to the new fee schedule that was just created.
			Provider prov=Providers.GetProv(fee.ProvNum);
			prov.FeeSched=fee.FeeSched;
			ProviderT.Update(prov);
			//Update the database so that the practice provider is the provider associated to our new fee.
			long practiceDefaultProvOld=PrefC.GetLong(PrefName.PracticeDefaultProv);
			PrefT.UpdateLong(PrefName.PracticeDefaultProv,fee.ProvNum);
			//Since our new fee is associated to the fee schedule of the practice default provider, it should be returned by GetListFromObjects().
			List<Fee> listFees=Fees.GetListFromObjects(new List<ProcedureCode>(){ procedureCode },
				null,
				null,
				0,
				0,
				0,
				null,
				null,
				null,
				null,
				0);
			//Put the default provider back the way it was prior to asserting.
			PrefT.UpdateLong(PrefName.PracticeDefaultProv,practiceDefaultProvOld);
			Assert.IsTrue(listFees.Exists(x => x.FeeNum==fee.FeeNum));
		}

		///<summary>Attest that fees associated to the fee schedule for the treating provider are returned.</summary>
		[TestMethod]
		public void Fees_GetListFromObjects_ProviderTreating() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("D1110");
			Fee fee=CreateSingleFee(suffix,(_defaultFeeAmt * _rand.NextDouble()),hasProv:true,codeNum:procedureCode.CodeNum);
			//Update our new provider so that they are associated to the new fee schedule that was just created.
			Provider prov=Providers.GetProv(fee.ProvNum);
			prov.FeeSched=fee.FeeSched;
			ProviderT.Update(prov);
			//Since our new fee is associated to the fee schedule of the treating provider, it should be returned by GetListFromObjects().
			List<Fee> listFees=Fees.GetListFromObjects(new List<ProcedureCode>(){ procedureCode },
				null,
				new List<long>() { prov.ProvNum },
				0,
				0,
				0,
				null,
				null,
				null,
				null,
				0);
			Assert.IsTrue(listFees.Exists(x => x.FeeNum==fee.FeeNum));
		}

		///<summary>Attest that fees associated to the fee schedule for the patient's primary provider are returned.</summary>
		[TestMethod]
		public void Fees_GetListFromObjects_ProviderPatientPrimary() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("D1110");
			Fee fee=CreateSingleFee(suffix,(_defaultFeeAmt * _rand.NextDouble()),hasProv:true,codeNum:procedureCode.CodeNum);
			//Update our new provider so that they are associated to the new fee schedule that was just created.
			Provider prov=Providers.GetProv(fee.ProvNum);
			prov.FeeSched=fee.FeeSched;
			ProviderT.Update(prov);
			//Act like this provider is a primary provider of a patient which should return the fees for the provider by GetListFromObjects().
			List<Fee> listFees=Fees.GetListFromObjects(new List<ProcedureCode>(){ procedureCode },
				null,
				null,
				prov.ProvNum,
				0,
				0,
				null,
				null,
				null,
				null,
				0);
			Assert.IsTrue(listFees.Exists(x => x.FeeNum==fee.FeeNum));
		}

		///<summary>Attest that fees associated to the fee schedule for the patient's secondary provider are returned.</summary>
		[TestMethod]
		public void Fees_GetListFromObjects_ProviderPatientSecondary() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("D1110");
			Fee fee=CreateSingleFee(suffix,(_defaultFeeAmt * _rand.NextDouble()),hasProv:true,codeNum:procedureCode.CodeNum);
			//Update our new provider so that they are associated to the new fee schedule that was just created.
			Provider prov=Providers.GetProv(fee.ProvNum);
			prov.FeeSched=fee.FeeSched;
			ProviderT.Update(prov);
			//Act like this provider is a secondary provider of a patient which should return the fees for the provider by GetListFromObjects().
			List<Fee> listFees=Fees.GetListFromObjects(new List<ProcedureCode>(){ procedureCode },
				null,
				null,
				0,
				prov.ProvNum,
				0,
				null,
				null,
				null,
				null,
				0);
			Assert.IsTrue(listFees.Exists(x => x.FeeNum==fee.FeeNum));
		}

		///<summary>Attest that fees associated to the fee schedule for the procedure code's default provider are returned.</summary>
		[TestMethod]
		public void Fees_GetListFromObjects_ProviderProcCodeDefault() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("D1110");
			Fee fee=CreateSingleFee(suffix,(_defaultFeeAmt * _rand.NextDouble()),hasProv:true,codeNum:procedureCode.CodeNum);
			//Update our new provider so that they are associated to the new fee schedule that was just created.
			Provider prov=Providers.GetProv(fee.ProvNum);
			prov.FeeSched=fee.FeeSched;
			ProviderT.Update(prov);
			//Make the default provider associated to the procedure code our new provider.
			procedureCode.ProvNumDefault=prov.ProvNum;
			ProcedureCodeT.Update(procedureCode);
			//The fees associated to the fee schedule of the procedure code's default provider should be returned by GetListFromObjects().
			List<Fee> listFees=Fees.GetListFromObjects(new List<ProcedureCode>(){ procedureCode },
				null,
				null,
				0,
				0,
				0,
				null,
				null,
				null,
				null,
				0);
			Assert.IsTrue(listFees.Exists(x => x.FeeNum==fee.FeeNum));
		}

		///<summary>Attest that fees associated to the fee schedule for the appointment's primary provider are returned.</summary>
		[TestMethod]
		public void Fees_GetListFromObjects_AppointmentProviderPrimary() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("D1110");
			Fee fee=CreateSingleFee(suffix,(_defaultFeeAmt * _rand.NextDouble()),hasProv:true,codeNum:procedureCode.CodeNum);
			//Update our new provider so that they are associated to the new fee schedule that was just created.
			Provider prov=Providers.GetProv(fee.ProvNum);
			prov.FeeSched=fee.FeeSched;
			ProviderT.Update(prov);
			//Make an appointment that has the new provider set as the primary provider.
			Appointment appt=AppointmentT.CreateAppointment(0,DateTime.Now,0,prov.ProvNum);
			//The fees associated to the fee schedule of the appointment's primary provider should be returned by GetListFromObjects().
			List<Fee> listFees=Fees.GetListFromObjects(new List<ProcedureCode>(){ procedureCode },
				null,
				null,
				0,
				0,
				0,
				null,
				null,
				new List<Appointment>() { appt },
				null,
				0);
			Assert.IsTrue(listFees.Exists(x => x.FeeNum==fee.FeeNum));
		}

		///<summary>Attest that fees associated to the fee schedule for the appointment's secondary provider are returned.</summary>
		[TestMethod]
		public void Fees_GetListFromObjects_AppointmentProviderSecondary() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("D1110");
			Fee fee=CreateSingleFee(suffix,(_defaultFeeAmt * _rand.NextDouble()),hasProv:true,codeNum:procedureCode.CodeNum);
			//Update our new provider so that they are associated to the new fee schedule that was just created.
			Provider prov=Providers.GetProv(fee.ProvNum);
			prov.FeeSched=fee.FeeSched;
			ProviderT.Update(prov);
			//Make an appointment that has the new provider set as the secondary provider.
			Appointment appt=AppointmentT.CreateAppointment(0,DateTime.Now,0,0,provHyg:prov.ProvNum);
			//The fees associated to the fee schedule of the appointment's secondary provider should be returned by GetListFromObjects().
			List<Fee> listFees=Fees.GetListFromObjects(new List<ProcedureCode>(){ procedureCode },
				null,
				null,
				0,
				0,
				0,
				null,
				null,
				new List<Appointment>() { appt },
				null,
				0);
			Assert.IsTrue(listFees.Exists(x => x.FeeNum==fee.FeeNum));
		}

		///<summary>Attest that fees associated to the fee schedule for the patient are returned.</summary>
		[TestMethod]
		public void Fees_GetListFromObjects_PatientFeeSched() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("D1110");
			Fee fee=CreateSingleFee(suffix,(_defaultFeeAmt * _rand.NextDouble()),hasProv:true,codeNum:procedureCode.CodeNum);
			//Act like a patient has our fee schedule which should return the corresponding fees by GetListFromObjects().
			List<Fee> listFees=Fees.GetListFromObjects(new List<ProcedureCode>(){ procedureCode },
				null,
				null,
				0,
				0,
				fee.FeeSched,
				null,
				null,
				null,
				null,
				0);
			Assert.IsTrue(listFees.Exists(x => x.FeeNum==fee.FeeNum));
		}

		///<summary>Attest that fees associated to the fee schedule for the insurance plan are returned.</summary>
		[TestMethod]
		public void Fees_GetListFromObjects_InsPlanFeeSched() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("D1110");
			Fee fee=CreateSingleFee(suffix,(_defaultFeeAmt * _rand.NextDouble()),hasProv:true,codeNum:procedureCode.CodeNum);
			//Create an insurance plan and associate it to the fee schedule that was just created.
			InsPlan insPlan=InsPlanT.CreateInsPlan(0,feeSched:fee.FeeSched);
			//Act like an ins plan has our fee schedule which should return the corresponding fees by GetListFromObjects().
			List<Fee> listFees=Fees.GetListFromObjects(new List<ProcedureCode>(){ procedureCode },
				null,
				null,
				0,
				0,
				0,
				new List<InsPlan>() { insPlan },
				null,
				null,
				null,
				0);
			Assert.IsTrue(listFees.Exists(x => x.FeeNum==fee.FeeNum));
		}

		///<summary>Attest that fees associated to the fee schedule for the insurance plan's allowed fee schedule are returned.</summary>
		[TestMethod]
		public void Fees_GetListFromObjects_InsPlanAllowedFeeSched() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("D1110");
			Fee fee=CreateSingleFee(suffix,(_defaultFeeAmt * _rand.NextDouble()),hasProv:true,codeNum:procedureCode.CodeNum);
			//Create an insurance plan and associate the allowed fee schedule to the fee schedule that was just created.
			InsPlan insPlan=InsPlanT.CreateInsPlan(0,allowedFeeSched:fee.FeeSched);
			//Act like an ins plan has our fee schedule which should return the corresponding fees by GetListFromObjects().
			List<Fee> listFees=Fees.GetListFromObjects(new List<ProcedureCode>(){ procedureCode },
				null,
				null,
				0,
				0,
				0,
				new List<InsPlan>() { insPlan },
				null,
				null,
				null,
				0);
			Assert.IsTrue(listFees.Exists(x => x.FeeNum==fee.FeeNum));
		}

		///<summary>Attest that fees associated to the fee schedule for the insurance plan's copay fee schedule are returned.</summary>
		[TestMethod]
		public void Fees_GetListFromObjects_InsPlanCopayFeeSched() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("D1110");
			Fee fee=CreateSingleFee(suffix,(_defaultFeeAmt * _rand.NextDouble()),hasProv:true,codeNum:procedureCode.CodeNum);
			//Create an insurance plan and associate the copay fee schedule to the fee schedule that was just created.
			InsPlan insPlan=InsPlanT.CreateInsPlan(0,copayFeeSched:fee.FeeSched);
			//Act like an ins plan has our fee schedule which should return the corresponding fees by GetListFromObjects().
			List<Fee> listFees=Fees.GetListFromObjects(new List<ProcedureCode>(){ procedureCode },
				null,
				null,
				0,
				0,
				0,
				new List<InsPlan>() { insPlan },
				null,
				null,
				null,
				0);
			Assert.IsTrue(listFees.Exists(x => x.FeeNum==fee.FeeNum));
		}

		///<summary>Attest that fees associated to the fee schedule for the discount plan are returned.</summary>
		[TestMethod]
		public void Fees_GetListFromObjects_DiscountPlan() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			ProcedureCode procedureCode=ProcedureCodeT.CreateProcCode("D1110");
			Fee fee=CreateSingleFee(suffix,(_defaultFeeAmt * _rand.NextDouble()),hasProv:true,codeNum:procedureCode.CodeNum);
			//Create a discount plan and associate it to the fee schedule that was just created.
			DiscountPlan discountPlan=DiscountPlanT.CreateDiscountPlan(suffix,feeSchedNum:fee.FeeSched);
			//Act like a discount plan has our fee schedule which should return the corresponding fees by GetListFromObjects().
			List<Fee> listFees=Fees.GetListFromObjects(new List<ProcedureCode>(){ procedureCode },
				null,
				null,
				0,
				0,
				0,
				null,
				null,
				null,
				null,
				discountPlan.DiscountPlanNum);
			Assert.IsTrue(listFees.Exists(x => x.FeeNum==fee.FeeNum));
		}

	}
}
