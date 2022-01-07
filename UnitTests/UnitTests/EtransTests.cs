using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDental;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests.Etrans_Tests {
	[TestClass]
	public class EtransTests:TestBase {
		private static X835 _x835;
		private static List<Etrans835Attach> _listEtrans835Attaches;
		private static Claim _claimPrimaryJustinSmith;
		private static Claim _claimPrimaryJacobJones;
		private static Claim _claimPrimaryStephanieMayer;
		private static Hx835_Claim _eraJustinSmithClaim;
		private static Hx835_Claim _eraJacobJonesClaim;
		private static Hx835_Claim _eraStephanieMayerClaim;
		private static X271 _x271;

		[ClassInitialize]
		public static void SetupClass(TestContext testContext) {
			//Add anything here that you want to run once before the tests in this class run.
			ProcedureCodeT.AddIfNotPresent("D0120");
			ProcedureCodeT.AddIfNotPresent("D1110");
			ProcedureCodeT.AddIfNotPresent("D0602");
			ProcedureCodeT.AddIfNotPresent("D1206");
			ProcedureCodes.RefreshCache();//Refresh cache if the codes above were added
			_x271=new X271(Properties.Resources.x271Test);
		}

		[TestInitialize]
		public void SetupTest() {
			#region Create Patient and claim: Justin Smith
			Patient patJustinSmith=PatientT.CreatePatient(lName: "Smith",fName: "Justin");
			List<EraTestProcCodeData> listProcData=new List<EraTestProcCodeData>();
			listProcData.Add(new EraTestProcCodeData("D0120",ProcStat.TP,43,new DateTime(2017,09,26)));
			listProcData.Add(new EraTestProcCodeData("D1110",ProcStat.TP,110,new DateTime(2017,09,26)));
			listProcData.Add(new EraTestProcCodeData("D0602",ProcStat.TP,5,new DateTime(2017,09,26)));
			_claimPrimaryJustinSmith=EtransT.SetupEraClaim(patJustinSmith,listProcData,"P","217308827",out List<InsuranceInfo> _listJustinSmithInsuranceInfo);
			#endregion
			#region Create Patient and claim: Jacob Jones
			Patient patJacobJones=PatientT.CreatePatient(lName: "Jones",fName: "Jacob");
			listProcData.Clear();
			listProcData.Add(new EraTestProcCodeData("D0120",ProcStat.TP,43,new DateTime(2017,09,26)));
			listProcData.Add(new EraTestProcCodeData("D1110",ProcStat.TP,110,new DateTime(2017,09,26)));
			listProcData.Add(new EraTestProcCodeData("D1206",ProcStat.TP,68,new DateTime(2017,09,26)));
			_claimPrimaryJacobJones=EtransT.SetupEraClaim(patJacobJones,listProcData,"P","217180995",out List<InsuranceInfo> _listJacobJonesInsuranceInfo);
			#endregion
			#region Create Patient and claim: Stephanie Mayer
			Patient patStephanieMayer=PatientT.CreatePatient(lName: "Mayer",fName: "Stephanie");
			listProcData.Clear();
			listProcData.Add(new EraTestProcCodeData("D0120",ProcStat.TP,43,new DateTime(2017,09,26)));
			listProcData.Add(new EraTestProcCodeData("D1110",ProcStat.TP,110,new DateTime(2017,09,26)));
			listProcData.Add(new EraTestProcCodeData("D0602",ProcStat.TP,5,new DateTime(2017,09,26)));
			listProcData.Add(new EraTestProcCodeData("D1206",ProcStat.TP,68,new DateTime(2017,09,26)));
			_claimPrimaryStephanieMayer=EtransT.SetupEraClaim(patStephanieMayer,listProcData,"P","217439125",out List<InsuranceInfo> _listStephanieMayerInsuranceInfo);
			#endregion
			//Create and insert etrans entry
			Etrans etrans835=EtransT.Insert835Etrans(Properties.Resources.JustinSmithERA,new DateTime(2017,09,30));//Spoof etrans imported 4 days after claim sent.
			List<ODTuple<Patient,long>> listPats=new List<ODTuple<Patient,long>>() {
				new ODTuple<Patient, long>(patJustinSmith,_claimPrimaryJustinSmith.ClaimNum),
				new ODTuple<Patient, long>(patJacobJones,_claimPrimaryJacobJones.ClaimNum),
				new ODTuple<Patient, long>(patStephanieMayer,_claimPrimaryStephanieMayer.ClaimNum)
			};
			_x835=EtransT.Construct835(etrans835,Properties.Resources.JustinSmithERA,listPats,out _listEtrans835Attaches);
			foreach(Hx835_Claim eraClaim in _x835.ListClaimsPaid) {
				switch(eraClaim.PatientName.Fname.ToLower()) {
					case "justin":
						_eraJustinSmithClaim=eraClaim;
						break;
					case "jacob":
						_eraJacobJonesClaim=eraClaim;
						break;
					case "stephanie":
						_eraStephanieMayerClaim=eraClaim;
						break;
				}
			}
		}

		[TestCleanup]
		public void TearDownTest() {
			BenefitT.ClearBenefitTable();
			CarrierT.ClearCarrierTable();
			ClaimT.ClearClaimTable();
			ClaimProcT.ClearClaimProcTable();
			EtransT.ClearEtransTable();
			InsPlanT.ClearInsPlanTable();
			InsSubT.ClearInsSubTable();
			PatientT.ClearPatientTable();
			PatPlanT.ClearPatPlanTable();
			ProcedureT.ClearProcedureTable();
			SubstitutionLinkT.ClearSubstitutionLinkTable();
		}

		[ClassCleanup]
		public static void TearDownClass() {
			//Add anything here that you want to run after all the tests in this class have been run.
		}

		#region X835
		[TestMethod]
		public void X835_ClaimMatching_ClaimIdentifier() {
			List<long> listClaimNums=Claims.GetClaimFromX12(_x835.GetClaimMatches(new List<Hx835_Claim>() { _eraJustinSmithClaim }));
			Assert.AreEqual(_eraJustinSmithClaim.ClaimNum,listClaimNums[0]);
		}

		[TestMethod]
		public void X835_TryGetMatchedClaimProc_AllProcsMatched() {
			//Will return if payment already entered.
			//Must enter payment to match claim procs since they consider financial information.
			TryEnterPayment(_x835,_eraJustinSmithClaim,_claimPrimaryJustinSmith,true);
			List<Hx835_ShortClaimProc> listClaimProcs=ClaimProcs.Refresh(_claimPrimaryJustinSmith.PatNum)//TODO: limit list to claimProcs for primary claim only.
				.Select(x => new Hx835_ShortClaimProc(x)).ToList();
			//Mimics X835.Hx835_Claim.IsProcessed(...)
			List<Hx835_Proc> listProcsNotMatched=new List<Hx835_Proc>();
			foreach(Hx835_Proc proc in _eraJustinSmithClaim.ListProcs) {
				Hx835_ShortClaimProc matchedClaimProc=null;
				if(!proc.TryGetMatchedClaimProc(out matchedClaimProc,listClaimProcs,false)) {
					listProcsNotMatched.Add(proc);
					continue;
				}
				listClaimProcs.Remove(matchedClaimProc);//So it cannot be matched twice.
			}
			Assert.AreEqual(0,listProcsNotMatched.Count);
		}

		[TestMethod]
		public void X835_IsProcessed_NoSupplemental() {
			//Must enter payment to match claim procs since they consider financial information.
			TryEnterPayment(_x835,_eraJustinSmithClaim,_claimPrimaryJustinSmith,true);//Will return if payment already entered.
			Assert.AreEqual(true,_eraJustinSmithClaim.IsProcessed(ClaimProcs.Refresh(_claimPrimaryJustinSmith.PatNum).Select(x => new Hx835_ShortClaimProc(x)).ToList(),_listEtrans835Attaches));
		}

		[TestMethod]
		public void X835_IsProcessed_WithPartialSupplemental() {
			TryEnterPayment(_x835,_eraJustinSmithClaim,_claimPrimaryJustinSmith,true);//Will return if payment already entered.
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(_claimPrimaryJustinSmith.PatNum);
			List<ClaimProc> listSupplementalClaimProcs=ClaimProcs.CreateSuppClaimProcs(listClaimProcs);
			listSupplementalClaimProcs.RemoveAt(0);//For this test, only enter supplemental payment for some of the procs.
			listClaimProcs.AddRange(listSupplementalClaimProcs);
			Assert.AreEqual(true,_eraJustinSmithClaim.IsProcessed(listClaimProcs.Select(x => new Hx835_ShortClaimProc(x)).ToList(),_listEtrans835Attaches));
		}

		[TestMethod]
		public void X835_TryGetMatchedClaimProc_SpecificSupplemental() {
			TryEnterPayment(_x835,_eraJustinSmithClaim,_claimPrimaryJustinSmith,true);//Will return if payment already entered.
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(_claimPrimaryJustinSmith.PatNum);//TODO: limit list to claimProcs for primary claim only.
			listClaimProcs.AddRange(ClaimProcs.CreateSuppClaimProcs(listClaimProcs));
			long claimProcNumExpected=listClaimProcs.Find(x => x.ClaimNum==_claimPrimaryJustinSmith.ClaimNum 
				&& x.Status==ClaimProcStatus.Received 
				&& x.CodeSent=="D0120"
			).ClaimProcNum;
			long claimProcNumActual=0;
			List<Hx835_ShortClaimProc> listShortClaimProcs=listClaimProcs.Select(x => new Hx835_ShortClaimProc(x)).ToList();
			bool isSupplemental=_eraJustinSmithClaim.GetIsSupplemental(_listEtrans835Attaches,listShortClaimProcs);
			List<Hx835_Proc> listProcsNotMatched=new List<Hx835_Proc>();
			foreach(Hx835_Proc proc in _eraJustinSmithClaim.ListProcs) {
				Hx835_ShortClaimProc matchedClaimProc=null;
				if(!proc.TryGetMatchedClaimProc(out matchedClaimProc,listShortClaimProcs,isSupplemental)) {
					listProcsNotMatched.Add(proc);
					continue;
				}
				listShortClaimProcs.Remove(matchedClaimProc);//So it cannot be matched twice.
				if(claimProcNumActual!=claimProcNumExpected) {
					claimProcNumActual=matchedClaimProc.ClaimProcNum;
				}
				else {//Found a match, thats all we care about.
					break;
				}
			}
			Assert.AreEqual(claimProcNumExpected,claimProcNumActual);
		}

		[TestMethod]
		public void X835_GetIsSupplemental_FalsePossitive() {
			TryEnterPayment(_x835,_eraJustinSmithClaim,_claimPrimaryJustinSmith,true);//Will return if payment already entered.
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(_claimPrimaryJustinSmith.PatNum);
			List<ClaimProc> listSupplementalClaimProcs=ClaimProcs.CreateSuppClaimProcs(listClaimProcs);
			listSupplementalClaimProcs.RemoveAt(0);//Only enter supplemental payment for some of the procs.
			listClaimProcs.AddRange(listSupplementalClaimProcs);
			bool isEraClaimSupplemental=_eraJustinSmithClaim.GetIsSupplemental(_listEtrans835Attaches,listClaimProcs.Select(x => new Hx835_ShortClaimProc(x)).ToList());
			Assert.AreEqual(false,isEraClaimSupplemental);
		}
		
		[TestMethod]
		public void X835_GetStatus_Unprocessed() {
			//Unprocessed => All ERA claims are one of the following; not attached, do not have financial information entered or are manually detached.
			_claimPrimaryJustinSmith.ClaimStatus="W";//Other methods have entered payment already, spoof matched but not recieved claims.
			_x835.ListClaimsPaid[0].IsAttachedToClaim=true;
			_x835.ListClaimsPaid[0].ClaimNum=_claimPrimaryJustinSmith.ClaimNum;
			_x835.ListClaimsPaid[1].IsAttachedToClaim=true;
			_x835.ListClaimsPaid[1].ClaimNum=0;
			_x835.ListClaimsPaid[2].IsAttachedToClaim=true;
			_x835.ListClaimsPaid[2].ClaimNum=0;
			List<Hx835_ShortClaim> listClaims=new[] { _claimPrimaryJustinSmith, _claimPrimaryJacobJones, _claimPrimaryStephanieMayer }.Select(x => new Hx835_ShortClaim(x)).ToList();
			List<long> listPatNums=new List<long>() { _claimPrimaryJustinSmith.PatNum,_claimPrimaryJacobJones.PatNum,_claimPrimaryStephanieMayer.PatNum };
			X835Status status=_x835.GetStatus(listClaims,ClaimProcs.Refresh(listPatNums).Select(x => new Hx835_ShortClaimProc(x)).ToList(),_listEtrans835Attaches);
			_claimPrimaryJustinSmith.ClaimStatus="R";//Revert value back to what it was, this value was determined by TryEnterPayment(...)
			Assert.AreEqual(X835Status.Unprocessed,status);
		}

		[TestMethod]
		public void X835_GetStatus_Partial() {
			//Partial => Some ERA claims are attached with financial info, no claim payment.
			//All others are one of the following; not attached, do not have financial information or are manually detached.
			_x835.ListClaimsPaid[0].IsAttachedToClaim=true;
			_x835.ListClaimsPaid[0].ClaimNum=_claimPrimaryJustinSmith.ClaimNum;
			_x835.ListClaimsPaid[1].IsAttachedToClaim=false;
			_x835.ListClaimsPaid[1].ClaimNum=0;
			_x835.ListClaimsPaid[2].IsAttachedToClaim=true;
			_x835.ListClaimsPaid[2].ClaimNum=0;
			TryEnterPayment(_x835,_eraJustinSmithClaim,_claimPrimaryJustinSmith,true);//Will return if payment already entered.
			List<Hx835_ShortClaim> listClaims=new List<Hx835_ShortClaim>() { new Hx835_ShortClaim(_claimPrimaryJustinSmith) };
			X835Status status=_x835.GetStatus(listClaims,ClaimProcs.Refresh(_claimPrimaryJustinSmith.PatNum).Select(x => new Hx835_ShortClaimProc(x)).ToList(),_listEtrans835Attaches);
			Assert.AreEqual(X835Status.Partial,status);
		}

		[TestMethod]
		public void X835_GetStatus_NotFinalized() {
			//NotFinalized => Some or all ERA claims attached with financial payment, no claim payment.
			//All un-attached ERA claims must be manually detached.
			_x835.ListClaimsPaid[0].IsAttachedToClaim=true;
			_x835.ListClaimsPaid[0].ClaimNum=_claimPrimaryJustinSmith.ClaimNum;
			_x835.ListClaimsPaid[1].IsAttachedToClaim=true;
			_x835.ListClaimsPaid[1].ClaimNum=0;
			_x835.ListClaimsPaid[2].IsAttachedToClaim=true;
			_x835.ListClaimsPaid[2].ClaimNum=0;
			TryEnterPayment(_x835,_eraJustinSmithClaim,_claimPrimaryJustinSmith,true);//Will return if payment already entered.
			List<Hx835_ShortClaim> listClaim=new List<Hx835_ShortClaim>() { new Hx835_ShortClaim(_claimPrimaryJustinSmith), new Hx835_ShortClaim(),new Hx835_ShortClaim() };
			X835Status status=_x835.GetStatus(listClaim,ClaimProcs.Refresh(_claimPrimaryJustinSmith.PatNum).Select(x => new Hx835_ShortClaimProc(x)).ToList(),_listEtrans835Attaches);
			Assert.AreEqual(X835Status.NotFinalized,status);
		}

		[TestMethod]
		public void X835_GetStatus_FinalizedSomeDetached() {
			//FinalizedSomeDetached => Some ERA claims has financial information entered and claim payment.
			//Others are manually detached.
			_x835.ListClaimsPaid[0].IsAttachedToClaim=true;
			_x835.ListClaimsPaid[0].ClaimNum=_eraJustinSmithClaim.ClaimNum;
			_x835.ListClaimsPaid[1].IsAttachedToClaim=true;
			_x835.ListClaimsPaid[1].ClaimNum=0;
			_x835.ListClaimsPaid[2].IsAttachedToClaim=true;
			_x835.ListClaimsPaid[2].ClaimNum=0;
			//Returns if claim is recieved or payment already entered (no supplemental).
			TryEnterPayment(_x835,_eraJustinSmithClaim,_claimPrimaryJustinSmith,true);
			List<Claim> listClaimsForFinalization=_x835.GetClaimsForFinalization();
			List<ClaimProc> listClaimProcsAll=ClaimProcs.RefreshForClaims(listClaimsForFinalization.Where(x => x.ClaimNum!=0).Select(x=>x.ClaimNum).ToList());
			Patient pat=Patients.GetPat(listClaimsForFinalization[0].PatNum);
			Etranss.TryFinalizeBatchPayment(_x835,listClaimsForFinalization,listClaimProcsAll,pat.ClinicNum,isAutomatic:false);
			//jsalmon - No, you should not situationally return out of a unit test without an explanation.
			//if(!EtransL.TryFinalizeBatchPayment(_x835,true,true)) {
			//	return;
			//}
			List<Hx835_ShortClaim> listClaims=new List<Hx835_ShortClaim>() { new Hx835_ShortClaim(_claimPrimaryJustinSmith), new Hx835_ShortClaim(),new Hx835_ShortClaim() };//Spoof manually detached claims.
			List<Hx835_ShortClaimProc> listClaimProcs=ClaimProcs.Refresh(_claimPrimaryJustinSmith.PatNum).Select(x => new Hx835_ShortClaimProc(x)).ToList();
			X835Status status=_x835.GetStatus(listClaims,listClaimProcs,_listEtrans835Attaches);
			Assert.AreEqual(X835Status.FinalizedSomeDetached,status);
		}
		
		[TestMethod]
		public void X835_GetStatus_FinalizedAllDetached() {
			//FinalizedAllDetached => All ERA claims are manually detached.
			_x835.ListClaimsPaid[0].IsAttachedToClaim=true;
			_x835.ListClaimsPaid[0].ClaimNum=0;
			_x835.ListClaimsPaid[1].IsAttachedToClaim=true;
			_x835.ListClaimsPaid[1].ClaimNum=0;
			_x835.ListClaimsPaid[2].IsAttachedToClaim=true;
			_x835.ListClaimsPaid[2].ClaimNum=0;
			//if(!EtransL.TryFinalizeBatchPayment(_x835,true,true)) {
			//	return;
			//}
			List<Hx835_ShortClaim> listClaims=new List<Hx835_ShortClaim>() { new Hx835_ShortClaim(),new Hx835_ShortClaim(),new Hx835_ShortClaim() };//Spoof manually detached claims.
			List<Hx835_ShortClaimProc> listClaimProcs=ClaimProcs.Refresh(_claimPrimaryJustinSmith.PatNum).Select(x => new Hx835_ShortClaimProc(x)).ToList();
			X835Status status=_x835.GetStatus(listClaims,listClaimProcs,_listEtrans835Attaches);
			Assert.AreEqual(X835Status.FinalizedAllDetached,status);
		}

		[TestMethod]
		public void X835_GetStatus_Finalized() {
			//Finalized => Some or all ERA claims are attached and have financial payment entered with claim payment.
			//Other claims must be manually detached.
			_x835.ListClaimsPaid[0].IsAttachedToClaim=true;
			_x835.ListClaimsPaid[0].ClaimNum=_claimPrimaryJustinSmith.ClaimNum;
			_x835.ListClaimsPaid[1].IsAttachedToClaim=true;
			_x835.ListClaimsPaid[1].ClaimNum=_claimPrimaryJacobJones.ClaimNum;
			_x835.ListClaimsPaid[2].IsAttachedToClaim=true;
			_x835.ListClaimsPaid[2].ClaimNum=_claimPrimaryStephanieMayer.ClaimNum;
			//TryEnterPayment returns if claim is recieved or payment already entered (no supplemental).
			TryEnterPayment(_x835,_eraJustinSmithClaim,_claimPrimaryJustinSmith,true);
			TryEnterPayment(_x835,_eraJacobJonesClaim,_claimPrimaryJacobJones,true);
			TryEnterPayment(_x835,_eraStephanieMayerClaim,_claimPrimaryStephanieMayer,true);
			X835Status status;
			List<Claim> listClaimsForFinalization=_x835.GetClaimsForFinalization();
			List<ClaimProc> listClaimProcsAll=ClaimProcs.RefreshForClaims(listClaimsForFinalization.Where(x => x.ClaimNum!=0).Select(x=>x.ClaimNum).ToList());
			Patient pat=Patients.GetPat(listClaimsForFinalization[0].PatNum);
			if(!Etranss.TryFinalizeBatchPayment(_x835,listClaimsForFinalization,listClaimProcsAll,pat.ClinicNum,isAutomatic:false)) {
				status=X835Status.None;
			}
			else {
				List<Hx835_ShortClaim> listClaims=new[] { _claimPrimaryJustinSmith, _claimPrimaryJacobJones, _claimPrimaryStephanieMayer }.Select(x => new Hx835_ShortClaim(x)).ToList();
				List<long> listPatNums=new List<long>() { _claimPrimaryJustinSmith.PatNum,_claimPrimaryJacobJones.PatNum,_claimPrimaryStephanieMayer.PatNum };
				status=_x835.GetStatus(listClaims,ClaimProcs.Refresh(listPatNums).Select(x => new Hx835_ShortClaimProc(x)).ToList(),_listEtrans835Attaches);
			}
			Assert.AreEqual(X835Status.Finalized,status);
		}

		///<summary>Claims within an ERA EOB should not be considered as 'split claims' when they have different payer control numbers.</summary>
		[TestMethod]
		public void X835_DiscoverSplitClaims_PayerControlNumber() {
			X12object x835=new X12object(Properties.Resources.x835PayerControlNumber);
			List<string> listTranSetIds=x835.GetTranSetIds();
			Assert.AreEqual(5,listTranSetIds.Count);
			//The fourth transaction contains four claims. The first three claims are related and fourth claim is completely unrelated.
			//The first claim is an original, the second is a reversal for the original, and the third is the corrected claim.
			//The original and correct claims have matching tracking numbers but have different payer control numbers and should not be treated as split.
			X835 x835_TransID4=new X835(new Etrans(),Properties.Resources.x835PayerControlNumber,listTranSetIds[3]);
			//Therefore, all four claims within the fourth transaction should all be treated as separate claims.
			Assert.AreEqual(4,x835_TransID4.ListClaimsPaid.Count);
			Assert.IsFalse(x835_TransID4.ListClaimsPaid.Any(x => x.IsSplitClaim));
		}

		///<summary>Claims within an ERA EOB from Massachussetts Health are always split up so that every claim only has one procedure attached.</summary>
		[TestMethod]
		public void X835_DiscoverSplitClaims_MassHealthMedicaid() {
			X12object x835=new X12object(Properties.Resources.x835MassHealth);
			List<string> listTranSetIds=x835.GetTranSetIds();
			Assert.AreEqual(1,listTranSetIds.Count);
			//This transaction contains 25 CLP segments. However, there are NOT 25 individual claims being referred to.
			//There are a plethora of claims that are 'split' between several CLP segments which need to be duct taped together.
			//There should be 13 individual claims after using duct tape.
			X835 x835_TransID=new X835(new Etrans(),Properties.Resources.x835MassHealth,listTranSetIds[0]);
			List<Hx835_Claim> listHx835_Claims=x835_TransID.ListClaimsPaid.FindAll(x => x.ClaimTrackingNumber=="62353/34260");
			Assert.AreEqual(2,listHx835_Claims.Count);
			Assert.IsTrue(listHx835_Claims.All(x => x.IsSplitClaim));
			listHx835_Claims=x835_TransID.ListClaimsPaid.FindAll(x => x.ClaimTrackingNumber=="2566/58633");
			Assert.AreEqual(1,listHx835_Claims.Count);
			Assert.IsFalse(listHx835_Claims.All(x => x.IsSplitClaim));
			listHx835_Claims=x835_TransID.ListClaimsPaid.FindAll(x => x.ClaimTrackingNumber=="28127/12820");
			Assert.AreEqual(3,listHx835_Claims.Count);
			Assert.IsTrue(listHx835_Claims.All(x => x.IsSplitClaim));
			listHx835_Claims=x835_TransID.ListClaimsPaid.FindAll(x => x.ClaimTrackingNumber=="53205/4650");
			Assert.AreEqual(3,listHx835_Claims.Count);
			Assert.IsTrue(listHx835_Claims.All(x => x.IsSplitClaim));
			listHx835_Claims=x835_TransID.ListClaimsPaid.FindAll(x => x.ClaimTrackingNumber=="5532/10853");
			Assert.AreEqual(3,listHx835_Claims.Count);
			Assert.IsTrue(listHx835_Claims.All(x => x.IsSplitClaim));
			listHx835_Claims=x835_TransID.ListClaimsPaid.FindAll(x => x.ClaimTrackingNumber=="83886/67072");
			Assert.AreEqual(1,listHx835_Claims.Count);
			Assert.IsFalse(listHx835_Claims.All(x => x.IsSplitClaim));
			listHx835_Claims=x835_TransID.ListClaimsPaid.FindAll(x => x.ClaimTrackingNumber=="46385/77712");
			Assert.AreEqual(3,listHx835_Claims.Count);
			Assert.IsTrue(listHx835_Claims.All(x => x.IsSplitClaim));
			listHx835_Claims=x835_TransID.ListClaimsPaid.FindAll(x => x.ClaimTrackingNumber=="7081/88087");
			Assert.AreEqual(1,listHx835_Claims.Count);
			Assert.IsFalse(listHx835_Claims.All(x => x.IsSplitClaim));
			listHx835_Claims=x835_TransID.ListClaimsPaid.FindAll(x => x.ClaimTrackingNumber=="23424/00472");
			Assert.AreEqual(1,listHx835_Claims.Count);
			Assert.IsFalse(listHx835_Claims.All(x => x.IsSplitClaim));
			listHx835_Claims=x835_TransID.ListClaimsPaid.FindAll(x => x.ClaimTrackingNumber=="44624/37120");
			Assert.AreEqual(2,listHx835_Claims.Count);
			Assert.IsTrue(listHx835_Claims.All(x => x.IsSplitClaim));
			listHx835_Claims=x835_TransID.ListClaimsPaid.FindAll(x => x.ClaimTrackingNumber=="82013/34444");
			Assert.AreEqual(2,listHx835_Claims.Count);
			Assert.IsTrue(listHx835_Claims.All(x => x.IsSplitClaim));
			listHx835_Claims=x835_TransID.ListClaimsPaid.FindAll(x => x.ClaimTrackingNumber=="5577/77045");
			Assert.AreEqual(2,listHx835_Claims.Count);
			Assert.IsTrue(listHx835_Claims.All(x => x.IsSplitClaim));
			listHx835_Claims=x835_TransID.ListClaimsPaid.FindAll(x => x.ClaimTrackingNumber=="68754/03554");
			Assert.AreEqual(1,listHx835_Claims.Count);
			Assert.IsFalse(listHx835_Claims.All(x => x.IsSplitClaim));
		}

		///<summary>Returns if given claim is already recieved. Currently ERA test do not consider supplemental.</summary>
		public static void TryEnterPayment(X835 x835,Hx835_Claim claimPaid,Claim claim,bool isAutomatic) {
			if(claim.ClaimStatus=="R") {//Currently we do not have test for supplemental payments.
				return;
			}
			List<ClaimProc> listClaimProcsForClaim=ClaimProcs.RefreshForClaim(claim.ClaimNum);
			Patient pat=Patients.GetPat(claim.PatNum);
			Etranss.TryImportEraClaimData(x835,claimPaid,claim,pat,isAutomatic,listClaimProcsForClaim,insPayPlanNum:0);
		}
		#endregion

		#region x834 Tests
		///<summary>Tests to see that when the user does not want to replace a patients existing plan, a different plan is added as a secondary.</summary>
		[TestMethod]
		public void X834_ImportInsurancePlans_DoNotReplacePatPlan() {
			Patient pat=Createx834Patient();
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Create old insurance plan and associate it to them.
			InsuranceInfo insuranceOld=InsuranceT.AddInsurance(pat,"Old Carrier"+suffix);
			//Create x834
			X834 x834=new X834(new X12object(Properties.Resources.x834Test));
			int createdPatsCount,updatedPatsCount,skippedPatsCount,createdCarrierCount,createdInsPlanCount,updatedInsPlanCount,createdInsSubCount,
				updatedInsSubCount,createdPatPlanCount,droppedPatPlanCount,updatedPatPlanCount,createdEmployerCount;
			StringBuilder sbErrorMessages;
			EtransL.ImportInsurancePlans(x834,new List<Patient> { pat },true,false,false,out createdPatsCount,
				out updatedPatsCount,out skippedPatsCount,out createdCarrierCount,out createdInsPlanCount,out updatedInsPlanCount,out createdInsSubCount,
				out updatedInsSubCount,out createdPatPlanCount,out droppedPatPlanCount,out updatedPatPlanCount,out createdEmployerCount,out sbErrorMessages);
			//Get the pat plans for this patient from the database.
			List<PatPlan> listPatPlans=PatPlans.GetPatPlansForPat(pat.PatNum);
			//No patient was created. Should have matched the one we created.
			Assert.AreEqual(0,createdPatsCount);
			//There should be two now as the old was kept.
			Assert.AreEqual(2,listPatPlans.Count);
			Assert.AreEqual(1,createdPatPlanCount);
		}

		///<summary>Tests to see that when the user does want to replace a patients existing plan, the current plan is dropped and the new plan is 
		///added.</summary>
		[TestMethod]
		public void X834_ImportInsurancePlans_ReplacePatPlan() {
			Patient pat=Createx834Patient();
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Create old insurance plan and associate it to them.
			InsuranceInfo insuranceOld=InsuranceT.AddInsurance(pat,"Old Carrier"+suffix);
			//Create x834
			X834 x834=new X834(new X12object(Properties.Resources.x834Test));
			int createdPatsCount,updatedPatsCount,skippedPatsCount,createdCarrierCount,createdInsPlanCount,updatedInsPlanCount,createdInsSubCount,
				updatedInsSubCount,createdPatPlanCount,droppedPatPlanCount,updatedPatPlanCount,createdEmployerCount;
			StringBuilder sbErrorMessages;
			//Pass in true for dropExistingInsurance
			EtransL.ImportInsurancePlans(x834,new List<Patient> { pat },true,false,true,out createdPatsCount,
				out updatedPatsCount,out skippedPatsCount,out createdCarrierCount,out createdInsPlanCount,out updatedInsPlanCount,out createdInsSubCount,
				out updatedInsSubCount,out createdPatPlanCount,out droppedPatPlanCount,out updatedPatPlanCount,out createdEmployerCount,out sbErrorMessages);
			//Get the pat plans for this patient from the database.
			List<PatPlan> listPatPlans=PatPlans.GetPatPlansForPat(pat.PatNum);
			Assert.AreEqual(1,listPatPlans.Count);
			Assert.AreEqual(1,droppedPatPlanCount);
			InsSub subForPatPlan=InsSubs.GetOne(listPatPlans[0].InsSubNum);
			//These should be different as a new plan was created and the old plan was dropped.
			Assert.AreNotEqual(insuranceOld.PriInsPlan.PlanNum,subForPatPlan.PlanNum);
		}

		///<summary>Tests to see that when the user does want to replace a patients existing plan, the secondary pat plan that is not in the 834 is
		///replaced while the primary, which is in the 834, remains untouched.</summary>
		[TestMethod]
		public void X834_ImportInsurancePlans_ReplaceSecondaryPatPlan() {
			Patient pat=Createx834Patient();
			string suffix=MethodBase.GetCurrentMethod().Name;
			//Create primary insurance plan that appears in the 834.  Both the carrier name and subscriberID need to match.
			InsuranceInfo insurancePrimary=InsuranceT.AddInsurance(pat,"TestCarrierNew",ordinal:1,subscriberID:"CG00000B");
			//Create secondary insurance that does not appear in the 834.
			InsuranceInfo insuranceSecondary=InsuranceT.AddInsurance(pat,"Secondary Carrier"+suffix,ordinal:2);
			//Get the pat plans for this patient from the database.
			List<PatPlan> listPatPlans=PatPlans.GetPatPlansForPat(pat.PatNum);
			Assert.AreEqual(2,listPatPlans.Count);
			//Create x834
			X834 x834=new X834(new X12object(Properties.Resources.x834Test));
			//Pass in true for dropExistingInsurance
			EtransL.ImportInsurancePlans(x834,new List<Patient> { pat },true,false,true,out int createdPatsCount,
				out int updatedPatsCount,out int skippedPatsCount,out int createdCarrierCount,out int createdInsPlanCount,out int updatedInsPlanCount,
				out int createdInsSubCount,out int updatedInsSubCount,out int createdPatPlanCount,out int droppedPatPlanCount,out int updatedPatPlanCount,
				out int createdEmployerCount,out StringBuilder sbErrorMessages);
			//Get the pat plans for this patient from the database.
			listPatPlans=PatPlans.GetPatPlansForPat(pat.PatNum);
			Assert.AreEqual(1,listPatPlans.Count);
			Assert.AreEqual(1,droppedPatPlanCount);
			//These should be different as a new plan was created and the old plan was dropped.
			Assert.AreEqual(insurancePrimary.ListPatPlans[0].PatPlanNum,listPatPlans[0].PatPlanNum);
		}

		///<summary>Drops the existing patient that matches the patient in the x834 test if there is one. Creates a fresh patient and returns that.</summary>
		private Patient Createx834Patient() {
			Patient pat=Patients.GetPatientsByPartialName("Testx834").FirstOrDefault();
			if(pat!=null) {
				//A patient exists from an old test. Remove it to avoid conflicting information.
				Patients.Delete(pat);
			}
			return PatientT.CreatePatient(fName:"Testx834",lName:"Patient",birthDate:new DateTime(2001,1,1));
		}
		#endregion

		[TestMethod]
		public void X271_EB271_SetInsuranceHistoryDates(){
			Patient pat=PatientT.CreatePatient(lName:"Doe",fName:"John");
			Carrier carrier=CarrierT.CreateCarrier("X271Test");
			InsPlan insPlan=InsPlanT.CreateInsPlan(carrier.CarrierNum);
			InsSub insSub=InsSubT.CreateInsSub(pat.PatNum,insPlan.PlanNum);
			List<EB271> listEb=_x271.GetListEB(true,carrier.IsCoinsuranceInverted);
			int countValidInsHist=EB271.SetInsuranceHistoryDates(listEb,pat.PatNum,insSub);
			Assert.AreEqual(3,countValidInsHist);//There are 4 valid fields but 1 has an invalid date.
		}

	}
}
