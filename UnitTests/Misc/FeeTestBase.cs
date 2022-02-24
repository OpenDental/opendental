using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests {

	///<summary>A special base class that can be used for both unit and integration tests for fees, the fee cache, and fee schedules.</summary>
	[TestClass]
	public abstract class FeeTestBase:TestBase {
		protected const long _standardFeeSchedNum=53;
		protected static List<ProcedureCode> _listProcCodes=new List<ProcedureCode>();
		protected static List<ProcedureCode> _listProcCodesOld=new List<ProcedureCode>();
		protected const int _defaultFeeAmt=50;
		protected static Random _rand=new Random();
		protected static double _precision=0.01;

		protected static void FeeTestSetup() {
			//The program isn't designed to support orthoproclinks that share the same ProcNum.  Simply clear the table between each test.
			OrthoProcLinkT.ClearTable();
			//Some unit tests cannot handle duplicate procedure codes being present within the database.
			//This is acceptable because that scenario should be impossible (we block the user via the UI layer).
			//Delete all duplicate procedure codes.
			Dictionary<string,ProcedureCode> dictProcCodes=ProcedureCodes.GetAllCodes().GroupBy(x => x.ProcCode).ToDictionary(x => x.Key,x => x.First());
			ProcedureCodeT.ClearProcedureCodeTable();
			List<ProcedureCode> listCodes=new List<ProcedureCode>();
			foreach(ProcedureCode procedureCode in dictProcCodes.Values) {
				listCodes.Add(procedureCode);
			}
			ProcedureCodes.InsertMany(listCodes);
			ProcedureCodes.RefreshCache();
			_listProcCodes=ProcedureCodes.GetAllCodes();//Just in case the PKs matter to some tests.
			_listProcCodesOld=_listProcCodes.Select(x => x.Copy()).ToList();
			if(Fees.GetCountByFeeSchedNum(_standardFeeSchedNum) <= 0) {
			List<Fee> listFees=new List<Fee>();
				foreach(ProcedureCode procCode in _listProcCodes) {
						listFees.Add(new Fee() {
						FeeSched=_standardFeeSchedNum,
						CodeNum=procCode.CodeNum,
						Amount=_defaultFeeAmt*_rand.NextDouble(),
						ClinicNum=0,
						ProvNum=0
					}); //create the default fee schedule fee
				}
				Fees.InsertMany(listFees);
			}
		}

		///<summary>After running FeeTestSetup() this can be called to 
		///set procs in _listProcCodes back to their original state via _listProcCodesOld.</summary>
		protected static void FeeTestTearDown() {
			foreach(ProcedureCode procCode in _listProcCodesOld) {//Reset 
				ProcedureCodes.Update(procCode);
			}
			ProcedureCodes.RefreshCache();
		}

		#region Factory Methods
		/// <summary>Creates the request number of fee schedules, clinics and providers, and creates fees for each combination and code num.
		/// Always includes the the standard fees for HQ (ClinicNum=0) and will always create an empty fee schedule.</summary>
		protected FeeTestArgs CreateManyFees(int numFeeScheds,int numClinics,int numProvs,string suffix) {
			FeeTestArgs retVal=new FeeTestArgs();
			//Set up fee schedules
			for(int i=0;i<numFeeScheds;i++) {
				retVal.ListFeeSchedNums.Add(FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix,false));
			}
			//Set up clinics
			Prefs.UpdateBool(PrefName.EasyNoClinics,false);
			for(int i=0;i<numClinics;i++) {
				retVal.ListClinics.Add(ClinicT.CreateClinic(suffix+i));
			}
			//Set up providers
			for(int i=0;i<numProvs;i++) {
				retVal.ListProvNums.Add(ProviderT.CreateProvider(suffix));
			}
			//Create the fees
			List<Fee> listFees=new List<Fee>();
			foreach(long codeNum in _listProcCodes.Select(x => x.CodeNum)) {
				foreach(long feeSchedNum in retVal.ListFeeSchedNums) {
					foreach(Clinic clinic in retVal.ListClinics) {
						foreach(long provNum in retVal.ListProvNums) {
							listFees.Add(new Fee() {
								FeeSched=feeSchedNum,
								ClinicNum=clinic.ClinicNum,
								ProvNum=provNum,
								CodeNum=codeNum,
								Amount=_defaultFeeAmt*_rand.NextDouble()
							});
						}
					}
				}
			}
			Fees.InsertMany(listFees);
			if(retVal.ListFeeSchedNums.Count > 0 && retVal.ListClinics.Count > 0) {
				retVal.ListFees=Fees.GetByFeeSchedNumsClinicNums(retVal.ListFeeSchedNums,retVal.ListClinics.Select(x => x.ClinicNum).ToList())
					.Select(x => (Fee)x).ToList();
			}
			//Always include the standard fee schedule for HQ.
			retVal.ListFees=retVal.ListFees.Union(Fees.GetByFeeSchedNumsClinicNums(
					new List<long>() { _standardFeeSchedNum },
					new List<long>() { 0 })
				.Select(x => (Fee)x)).ToList();
			//create an empty feeschedule for the Insert/Update/Delete tests
			retVal.EmptyFeeSchedNum=FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,"Empty"+suffix,false);
			return retVal;
		}

		/// <summary>Creates a FeeSchedule, and if specified, a clinic and a provnum, and a fee associated to these three parameters with the specified
		/// fee amount.. The procedure code is assigned randomly from the list of procedure codes, though the last procedure code will never be picked so 
		/// that we can safely use _listProcCodes.Last() for testing non-existing fees.</summary>
		protected Fee CreateSingleFee(string suffix,double amt=_defaultFeeAmt,bool hasClinic=false,bool hasProv=false,long feeSchedNum=0,long codeNum=0,
			long clinicNum=0,long provNum=0,bool isGlobalFeeSched=false) 
		{
			feeSchedNum=feeSchedNum==0 ? FeeSchedT.CreateFeeSched(FeeScheduleType.Normal,suffix,isGlobalFeeSched) : feeSchedNum;
			clinicNum=hasClinic && clinicNum==0 ? ClinicT.CreateClinic(suffix).ClinicNum : clinicNum;
			provNum=hasProv && provNum==0 ? ProviderT.CreateProvider(suffix) : provNum;
			codeNum=codeNum==0 ? _listProcCodes[_rand.Next(_listProcCodes.Count-2)].CodeNum : codeNum;
			return FeeT.GetNewFee(feeSchedNum,codeNum,amt,clinicNum,provNum);
		}
		#endregion Factory Methods

		#region Helper Methods

		///<summary>Returns true if the "important" fields are the same. Throws an error when fields are mismatched.</summary>
		protected bool AreSimilar(Fee fee1,Fee fee2) {
			string error="";
			if(fee1.FeeNum!=fee2.FeeNum) {
				error+="FeeNum mismatch, expected: "+fee1.FeeNum+" actual: "+fee2.FeeNum+Environment.NewLine;
			}
			if(fee1.FeeSched!=fee2.FeeSched) {
				error+="FeeSched mismatch, expected: "+fee1.FeeSched+" actual: "+fee2.FeeSched+Environment.NewLine;
			}
			if(fee1.ClinicNum!=fee2.ClinicNum) {
				error+="ClinicNum mismatch, expected: "+fee1.ClinicNum+" actual: "+fee2.ClinicNum+Environment.NewLine;
			}
			if(fee1.CodeNum!=fee2.CodeNum) {
				error+="CodeNum mismatch, expected: "+fee1.CodeNum+" actual: "+fee2.CodeNum+Environment.NewLine;
			}
			if(fee1.ProvNum!=fee2.ProvNum) {
				error+="ProvNum mismatch, expected: "+fee1.ProvNum+" actual: "+fee2.ProvNum+Environment.NewLine;
			}
			if(Math.Abs(fee1.Amount-fee2.Amount) > _precision) {
				error+="Amount mismatch, expected: "+fee1.Amount+" actual: "+fee2.Amount+Environment.NewLine;
			}
			if(error!="") {
				throw new Exception(error);
			}
			return true;
		}

		///<summary>Returns true if the "important" fields are the same across two lists. Assumes the lists are sorted.</summary>
		protected bool AreListsSimilar(List<Fee> listExpected, List<Fee> listActual) {
			string error="";
			if(listExpected.Count!=listActual.Count) {
				error+="Mismatch in list counts. Expected count: "+listExpected.Count + " Actual count: "+listActual.Count+Environment.NewLine;
				//throw new Exception("Mismatch in list counts. Expected count: "+listExpected.Count + " Actual count: "+listActual.Count);
				//return false;
			}
			for(int i=0;i<listExpected.Count;i++) {
				try {
					if(AreSimilar(listExpected[i],listActual[i])) {
						continue;
					}
				}
				catch(Exception e) {
					//throw new Exception("Mismatch at index: "+i+". "+e.Message);
					error+="Mismatch at index: "+i+". "+e.Message;
					throw new Exception(error);
				}
			}
			return true;
		}

		///<summary>Primarily for fee import/copy. Returns true if we can retrieve all of the listActual fees from the db, and all of the fee amounts match those in the list.</summary>
		protected bool DoAmountsMatch(List<Fee> listActual, long feeSchedNum, long clinicNum, long provNum) {
			for(int i=0;i<listActual.Count;i++) {
				Fee fee=listActual[i];
				Fee originalFee=Fees.GetFee(fee.CodeNum,feeSchedNum,fee.ClinicNum,fee.ProvNum);
				try {
					if(originalFee.Amount!=fee.Amount) {
						return false;
					}
				}
				catch (Exception e) {
					throw new Exception("List check failed for fee at index :"+i+" with CodeNum="+fee.CodeNum+" Inner:"+e.Message);
				}
			}
			return true;
		}

		#endregion Helper Methods

		protected class FeeTestArgs {
			public List<long> ListFeeSchedNums=new List<long>();
			public List<Clinic> ListClinics=new List<Clinic>();
			public List<long> ListProvNums=new List<long>();
			public List<Fee> ListFees=new List<Fee>();
			public long EmptyFeeSchedNum=0;
		}
	}
}
