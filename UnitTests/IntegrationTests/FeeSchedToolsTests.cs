using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.FeeSchedTools_Tests {
	[TestClass]
	public class FeeSchedToolsTests:FeeTestBase {
		private const string _feeDeleteDuplicatesExpectedResult=@"Procedure codes with duplicate fee entries: 0
   Double click to see a break down.";

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
		}

		///<summary>Creates a single fee for a given fee schedule, copies the feeschedule to an empty schedule and checks that the fee is the same
		///in both fee schedules.</summary>
		[TestMethod]
		public void FeeSchedTools_CopyFeeSched() {
			//Create two fee schedules; from and to
			FeeSched fromFeeSched=FeeSchedT.GetNewFeeSched(FeeScheduleType.Normal,MethodBase.GetCurrentMethod().Name+"_FROM");
			FeeSched toFeeSched=FeeSchedT.GetNewFeeSched(FeeScheduleType.Normal,MethodBase.GetCurrentMethod().Name+"_TO");
			//Create a single fee and associate it to the "from" fee schedule.
			long feeCodeNum=_listProcCodes[_rand.Next(_listProcCodes.Count-1)].CodeNum;
			FeeT.CreateFee(fromFeeSched.FeeSchedNum,feeCodeNum,_defaultFeeAmt*_rand.NextDouble());
			FeeScheds.CopyFeeSchedule(fromFeeSched,0,0,toFeeSched,null,0);
			//Get the two fees and check that they are the same.
			Fee fromFee=Fees.GetFee(feeCodeNum,fromFeeSched.FeeSchedNum,0,0);
			Fee toFee=Fees.GetFee(feeCodeNum,toFeeSched.FeeSchedNum,0,0);
			Assert.AreEqual(fromFee.Amount,toFee.Amount);
		}

		///<summary>Mimics two instances of Open Dental being open and copying a fee schedule into the same fee schedule from each instance.
		///No matter how many instances of Open Dental invoke the same fee schedule action they should never create duplicate fees.</summary>
		[TestMethod]
		public void FeeSchedTools_CopyFeeSched_Concurrency() {
			//Make sure there are no duplicate fees already present within the database.
			string dbmResult=DatabaseMaintenances.FeeDeleteDuplicates(true,DbmMode.Check);
			if(dbmResult.Trim()!=_feeDeleteDuplicatesExpectedResult) {
				DatabaseMaintenances.FeeDeleteDuplicates(true,DbmMode.Fix);
			}
			//Create two fee schedules; from and to
			FeeSched feeSchedFrom=FeeSchedT.GetNewFeeSched(FeeScheduleType.Normal,MethodBase.GetCurrentMethod().Name+"_FROM");
			FeeSched feeSchedTo=FeeSchedT.GetNewFeeSched(FeeScheduleType.Normal,MethodBase.GetCurrentMethod().Name+"_TO");
			//Create a single fee and associate it to the "from" fee schedule.
			FeeT.CreateFee(feeSchedFrom.FeeSchedNum,_listProcCodes[_rand.Next(_listProcCodes.Count-1)].CodeNum,_defaultFeeAmt);
			//Create a helper action that will simply copy the "from" schedule into the "to" schedule for the given fee cache passed in.
			Action actionCopyFromTo=new Action(() => {
				FeeScheds.CopyFeeSchedule(feeSchedFrom,0,0,feeSchedTo,null,0);
			});
			//Mimic each user clicking the "Copy" button from within the Fee Tools window one right after the other (before they click OK).
			actionCopyFromTo();
			actionCopyFromTo();
			//Make sure that there was NOT a duplicate fee inserted into the database.
			dbmResult=DatabaseMaintenances.FeeDeleteDuplicates(true,DbmMode.Check);
			Assert.AreEqual(dbmResult.Trim(),_feeDeleteDuplicatesExpectedResult,"Duplicate fees detected due to concurrent copying.");
		}

		///<summary>Users can copy one fee schedule to multiple clinics at once.  There was a problem with copying fee schedules to more than six clinics
		///at the same time due to running the copy fee schedule logic in parallel.  This unit test is designed to make sure that the bug fix of running
		///the copy fee schedule logic synchronously is preserved OR if parallel threads are reintroduced that they have fixed the bug.</summary>
		[TestMethod]
		public void FeeSchedTools_CopyFeeSched_Clinics() {
			//Make sure there are no duplicate fees already present within the database.
			string dbmResult=DatabaseMaintenances.FeeDeleteDuplicates(true,DbmMode.Check);
			if(dbmResult.Trim()!=_feeDeleteDuplicatesExpectedResult) {
				DatabaseMaintenances.FeeDeleteDuplicates(true,DbmMode.Fix);
			}
			ClinicT.ClearClinicTable();
			for(int i=0;i<3;i++) {
				ClinicT.CreateClinic(MethodBase.GetCurrentMethod().Name+"_"+i);
			}
			//Create two fee schedules; from and to
			FeeSched feeSchedNumFrom=FeeSchedT.GetNewFeeSched(FeeScheduleType.Normal,MethodBase.GetCurrentMethod().Name+"_FROM");
			FeeSched feeSchedNumTo=FeeSchedT.GetNewFeeSched(FeeScheduleType.Normal,MethodBase.GetCurrentMethod().Name+"_TO");
			//Create a fee for the first 100 procedure codes in the database and associate it to the "from" fee schedule.
			List<Fee> listFees=new List<Fee>();
			foreach(ProcedureCode code in _listProcCodes.Take(100)) {
				listFees.Add(FeeT.GetNewFee(feeSchedNumFrom.FeeSchedNum,code.CodeNum,_rand.Next(5000),doInsert:false));
			}
			Fees.InsertMany(listFees);
			//Copy the "from" fee schedule into the "to" fee schedule and do it for at least seven clinics.
			FeeScheds.CopyFeeSchedule(feeSchedNumFrom,0,0,feeSchedNumTo,Clinics.GetDeepCopy(true).Select(x => x.ClinicNum).ToList(),0);
			//Make sure that there was NOT a duplicate fee inserted into the database.
			dbmResult=DatabaseMaintenances.FeeDeleteDuplicates(true,DbmMode.Check);
			Assert.AreEqual(dbmResult.Trim(),_feeDeleteDuplicatesExpectedResult,"Duplicate fees detected due to concurrent copying.");
		}

		///<summary>Creates and exports a fee schedule, then tries to import the fee schedule from the exported file then checks that the new fee 
		///schedule was imported correctly. If there are procedurecodes with an empty proccode we exclude these from the check, as we cannot look up
		///the correct code nums during the import (intended behavior). </summary>
		[TestMethod]
		public void FeeSchedTools_ImportExport() {
			FeeTestArgs feeArgs=CreateManyFees(1,1,1,MethodBase.GetCurrentMethod().Name);
			long exportedSched=feeArgs.ListFeeSchedNums[0];
			long importedSched=feeArgs.EmptyFeeSchedNum;
			long clinicNum=feeArgs.ListClinics[0].ClinicNum;
			string filename=MethodBase.GetCurrentMethod().Name;
			FeeScheds.ExportFeeSchedule(exportedSched,clinicNum,feeArgs.ListProvNums[0],filename);
			OpenDental.FeeL.ImportFees(filename,importedSched,clinicNum,feeArgs.ListProvNums[0]);
			foreach(ProcedureCode procCode in _listProcCodes.Where(x => !string.IsNullOrWhiteSpace(x.ProcCode))) { //unable to import without a proccodes
				Fee expected=Fees.GetFee(procCode.CodeNum,exportedSched,clinicNum,feeArgs.ListProvNums[0]);
				Fee actual=Fees.GetFee(procCode.CodeNum,importedSched,clinicNum,feeArgs.ListProvNums[0]);
				Assert.AreEqual(expected.Amount,actual.Amount);
			}
		}

		///<summary>Import canada fees from a file.</summary>
		[TestMethod]
		public void FeeSchedTools_ImportCanada() {
			string canadianCodes=Properties.Resources.canadianprocedurecodes;
			//If we need to import these procedures codes, do so
			foreach(string line in canadianCodes.Split(new[] { "\r\n", "\r", "\n" },StringSplitOptions.None)) {
				string[] properties=line.Split('\t');
				if(properties.Count()!=10) {
					continue;
				}
				if(ProcedureCodeT.GetCodeNum(properties[0])!=0) {
					continue;
				}
				ProcedureCode procCode=new ProcedureCode()
				{
					ProcCode=properties[0],
					Descript=properties[1],
					ProcTime=properties[8],
					AbbrDesc=properties[9],
				};
				ProcedureCodes.Insert(procCode);
			}
			//Now import the fees
			string feeData=Properties.Resources.BC_BCDA_2018_GPOOC;
			FeeSched feeSched=FeeSchedT.GetNewFeeSched(FeeScheduleType.Normal,MethodBase.GetCurrentMethod().Name,isGlobal:false);
			List<Fee> listNewFees=FeeScheds.ImportCanadaFeeSchedule2(feeSched,feeData,0,0,out int numImported,out int numSkipped);
			Assert.IsTrue(DoAmountsMatch(listNewFees,feeSched.FeeSchedNum,0,0));
		}

		///<summary>Create and fill a fee schedule, then clear the fee schedule and make sure it is empty.</summary>
		[TestMethod]
		public void FeeSchedTools_ClearFeeSchedule() {
			FeeTestArgs feeArgs=CreateManyFees(1,1,1,MethodBase.GetCurrentMethod().Name);
			long feeSchedNum=feeArgs.ListFeeSchedNums[0];
			Assert.IsTrue(Fees.GetCountByFeeSchedNum(feeSchedNum) > 0);
			Fees.DeleteFees(feeSchedNum,feeArgs.ListClinics[0].ClinicNum,feeArgs.ListProvNums[0]);
			Assert.AreEqual(0,Fees.GetCountByFeeSchedNum(feeSchedNum));
		}

		///<summary>Create the standard fee schedule and increase by 5% to the nearest penny.</summary>
		[TestMethod]
		public void FeeSchedTools_Increase() {
			FeeTestArgs args=CreateManyFees(0,0,0,MethodBase.GetCurrentMethod().Name);
			int percent=5;
			List<Fee> listStandardFees=Fees.GetListExact(_standardFeeSchedNum,0,0).OrderBy(x => x.FeeNum).ToList();
			List<Fee> listIncreasedFees=listStandardFees.Select(x => x.Copy()).ToList();
			listIncreasedFees=Fees.IncreaseNew(_standardFeeSchedNum,percent,2,listIncreasedFees,0,0).OrderBy(x => x.FeeNum).ToList();
			foreach(Fee fee in listIncreasedFees) {
				Fee expectedFee=Fees.GetFee(fee.CodeNum,fee.FeeSched,fee.ClinicNum,fee.ProvNum);
				double amount=fee.Amount/(1+(double)percent/100);
				amount=Math.Round(amount,2);
				try {
					Assert.AreEqual(expectedFee.Amount,amount);
				}
				catch(Exception e) {
					throw new Exception("Failed for fee: " + expectedFee.FeeNum,e);
				}
			}
		}

		///<summary>Attach some fees to procedures, change half the fees and call Global Update Fees.</summary>
		[TestMethod]
		public void FeeSchedTools_GlobalUpdateFees() {
			PrefT.UpdateBool(PrefName.MedicalFeeUsedForNewProcs,false);
			string suffix=MethodBase.GetCurrentMethod().Name;
			string procStr="D0120";
			string procStr2="D0145";
			double procFee=100;
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode(procStr);
			ProcedureCode procCode2=ProcedureCodeT.CreateProcCode(procStr2);
			//Set up clinic, prov, pat
			Clinic clinic=ClinicT.CreateClinic(suffix);
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix,false);
			long provNum=ProviderT.CreateProvider(suffix,feeSchedNum:feeSchedNum);
			Fee fee=FeeT.GetNewFee(feeSchedNum,procCode.CodeNum,procFee,clinic.ClinicNum,provNum);
			Fee fee2=FeeT.GetNewFee(feeSchedNum,procCode2.CodeNum,procFee,clinic.ClinicNum,provNum);
			Patient pat=PatientT.CreatePatient(suffix,provNum,clinic.ClinicNum);
			//Chart a procedure for this proccode/pat as well as a different proccode
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.TP,"",fee.Amount);
			Procedure proc2=ProcedureT.CreateProcedure(pat,procStr2,ProcStat.TP,"",fee2.Amount);
			//Update the fee amount for only the D0120 code
			fee.Amount=50;
			Fees.Update(fee);
			//Now run global update fees
			Procedures.GlobalUpdateFees(Fees.GetByClinicNum(clinic.ClinicNum),clinic.ClinicNum,clinic.Abbr);
			//Make sure we have the same number of updated fees, and fee amounts for both procs
			proc=Procedures.GetOneProc(proc.ProcNum,false);
			proc2=Procedures.GetOneProc(proc2.ProcNum,false);
			Assert.AreEqual(fee.Amount,proc.ProcFee);
			Assert.AreEqual(fee2.Amount,proc2.ProcFee);
		}
		
		///<summary>Create a single procedure, and call GlobalUpdateWriteoffs.</summary>
		[TestMethod]
		public void FeeSchedTools_GlobalUpdateWriteoffEstimates() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			string procStr="D0145";
			double procFee=100;
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode(procStr);
			//Set up clinic, prov, pat
			Clinic clinic=ClinicT.CreateClinic(suffix);
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.FixedBenefit,suffix);
			long provNum=ProviderT.CreateProvider(suffix,feeSchedNum:feeSchedNum);
			Fee fee=FeeT.GetNewFee(feeSchedNum,procCode.CodeNum,procFee,clinic.ClinicNum,provNum);
			Patient pat=PatientT.CreatePatient(suffix,provNum,clinic.ClinicNum);
			//Set up insurance
			InsuranceInfo info=InsuranceT.AddInsurance(pat,suffix,"c",feeSchedNum);
			List<InsSub> listSubs=info.ListInsSubs;
			List<InsPlan> listPlans=info.ListInsPlans;
			List<PatPlan> listPatPlans=info.ListPatPlans;
			InsPlan priPlan=info.PriInsPlan;
			InsSub priSub=info.PriInsSub;
			info.ListBenefits.Add(BenefitT.CreatePercentForProc(priPlan.PlanNum,procCode.CodeNum,90));
			//Create the procedure and claimproc
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.TP,"",procFee);
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,priPlan.PlanNum,priSub.InsSubNum,DateTime.Today,-1,-1,-1,
				ClaimProcStatus.CapEstimate);
			Procedures.ComputeEstimates(proc,pat.PatNum,new List<ClaimProc>(),true,listPlans,listPatPlans,info.ListBenefits,pat.Age,info.ListInsSubs);
			priClaimProc=ClaimProcs.Refresh(pat.PatNum).FirstOrDefault(x => x.ProcNum==proc.ProcNum);
			Assert.AreEqual(procFee,priClaimProc.WriteOff);
			procFee=50;
			Procedure procNew=proc.Copy();
			procNew.ProcFee=procFee;
			Procedures.Update(procNew,proc);
			//GlobalUpdate
			long updated=GlobalUpdateWriteoffs(clinic.ClinicNum);
			Assert.AreEqual(1,updated);
			ClaimProc priClaimProcDb=ClaimProcs.Refresh(pat.PatNum).FirstOrDefault(x => x.ClaimProcNum==priClaimProc.ClaimProcNum);
			Assert.AreEqual(procFee,priClaimProcDb.WriteOff);
		}


		///<summary>Create a single procedure, and call GlobalUpdateWriteoffs.</summary>
		[TestMethod]
		public void FeeSchedTools_GlobalUpdateWriteoffEstimates_SubscriberInDifferentFamily() {
			string suffix=MethodBase.GetCurrentMethod().Name;
			string procStr="D0145";
			double procFee=100;
			ProcedureCode procCode=ProcedureCodeT.CreateProcCode(procStr);
			//Set up clinic, prov, pat
			Clinic clinic=ClinicT.CreateClinic(suffix);
			long feeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix);
			long provNum=ProviderT.CreateProvider(suffix,feeSchedNum:feeSchedNum);
			Fee fee=FeeT.GetNewFee(feeSchedNum,procCode.CodeNum,procFee,clinic.ClinicNum,provNum);
			Patient pat=PatientT.CreatePatient(suffix,provNum,clinic.ClinicNum);
			Patient patSubscriber=PatientT.CreatePatient(suffix+"_Subscriber",provNum,clinic.ClinicNum);
			//Set up insurance
			InsuranceInfo info=InsuranceT.AddInsurance(pat,suffix,"p",feeSchedNum);
			info.PriInsSub.Subscriber=patSubscriber.PatNum;
			InsSubs.Update(info.PriInsSub);
			info.ListBenefits.Add(BenefitT.CreatePercentForProc(info.PriInsPlan.PlanNum,procCode.CodeNum,100));
			//Create the procedure and claimproc
			Procedure proc=ProcedureT.CreateProcedure(pat,procStr,ProcStat.TP,"",procFee);
			ClaimProc priClaimProc=ClaimProcT.CreateClaimProc(pat.PatNum,proc.ProcNum,info.PriInsPlan.PlanNum,info.PriInsSub.InsSubNum,DateTime.Today,
				-1,-1,-1,	ClaimProcStatus.CapEstimate);
			Procedures.ComputeEstimates(proc,pat.PatNum,new List<ClaimProc>(),true,info.ListInsPlans,info.ListPatPlans,info.ListBenefits,pat.Age,
				info.ListInsSubs);
			priClaimProc=ClaimProcs.Refresh(pat.PatNum).FirstOrDefault(x => x.ProcNum==proc.ProcNum);
			Assert.AreEqual(procFee,priClaimProc.InsPayEst);
			GlobalUpdateWriteoffs(clinic.ClinicNum);
			priClaimProc=ClaimProcs.Refresh(pat.PatNum).FirstOrDefault(x => x.ClaimProcNum==priClaimProc.ClaimProcNum);
			Assert.AreEqual(procFee,priClaimProc.InsPayEst);
		}


		private long GlobalUpdateWriteoffs(long clinicNum) {
			List<long> listWriteoffClinics=new List<long>() { clinicNum };
			ODProgressExtended progress=new ODProgressExtended(ODEventType.FeeSched,new FeeSchedEvent(),new System.Windows.Forms.Form(),
				tag:new ProgressBarHelper(Lans.g(this,"Write-off Update Progress"),progressBarEventType: ProgBarEventType.Header),
				cancelButtonText:Lans.g(this,"Close"));
			progress.Fire(ODEventType.FeeSched,new ProgressBarHelper("","0%"
						,0,100,ProgBarStyle.Blocks,"WriteoffProgress"));
			return FeeScheds.GlobalUpdateWriteoffs(listWriteoffClinics,progress);
		}
	}
}
