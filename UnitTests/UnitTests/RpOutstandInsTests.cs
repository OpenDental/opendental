using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;
using CodeBase;
using System.Globalization;
using System.Windows.Forms;

namespace UnitTests.RpOutstandingInsTests {
	[TestClass]
	public class RpOutstandingInsTests:TestBase {
		///<summary>This method will execute just before each test in this class.</summary>
		[TestInitialize]
		public void SetupTest() {
			CarrierT.ClearCarrierTable();
			ClaimT.ClearClaimTable();
			ClaimProcT.ClearClaimProcTable();
			InsPlanT.ClearInsPlanTable();
			InsSubT.ClearInsSubTable();
			PatientT.ClearPatientTable();
			PatPlanT.ClearPatPlanTable();
			ProcedureT.ClearProcedureTable();
			ProcedureCodeT.ClearProcedureCodeTable();
		}
		
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.RpOutstandingIns_ZeroClaim_SentClaim2ClaimProcsNotRecievedNoPayment)]
		[Documentation.VersionAdded("22.3")]
		[Documentation.Description(@"Creates a claim with two ClaimProcs attached. One of the ClaimProcs has a Status of 'Not Received'. The other has a Status of 'Preauth'. Neither ClaimProc has a ClaimPayment attached. The Claim is cleared by setting it's Status to 'Received' and it's DateReceived to today. Each ClaimProc has it's Writeoff, InsPayAmt, and InsPayEst set to 0 if it's Status is 'PreAuth' or 'Not Received' and it has no ClaimPayment attached.")]
		public void RpOutstandingIns_ZeroClaim_SentClaim2ClaimProcsNotRecievedNoPayment() {
			//Create test patient
			Patient patient=PatientT.CreatePatient();
			//Create 2 procs to attach to claim
			List<Procedure> listProcedures=new List<Procedure>();
			listProcedures.Add(ProcedureT.CreateProcedure(patient,"D1234",ProcStat.C,"1",25));
			listProcedures.Add(ProcedureT.CreateProcedure(patient,"D4321",ProcStat.C,"2",50));
			//Create insurance for claim
			InsuranceInfo insuranceInfo=InsuranceT.AddInsurance(patient,"Test Carrier");
			//Create claim 			
			Claim claim=ClaimT.CreateClaim(listProcedures,insuranceInfo);
			List<ClaimProc>listClaimProcs=ClaimProcs.RefreshForClaim(claim.ClaimNum);
			listClaimProcs[0].Status=ClaimProcStatus.Preauth;//status' are now preauth and not rec'd
			ClaimProcs.Update(listClaimProcs[0]);
			RpOutstandingIns.ZeroClaim(claim);
			List<ClaimProc> listClaimProcsZeroed=ClaimProcs.RefreshForClaim(claim.ClaimNum);
			//Assert that the claim status is received
			Assert.AreEqual("R",claim.ClaimStatus);
			//Assert that the date received is today
			Assert.AreEqual(DateTime.Today.ToShortDateString(),claim.DateReceived.ToShortDateString());
			//Assert that all claimprocs were zeroed correctly
			for(int i=0;i<listClaimProcs.Count;i++) {
				Assert.AreEqual(listClaimProcsZeroed[i].Status,ClaimProcStatus.Received);
				Assert.AreEqual(listClaimProcsZeroed[i].WriteOff,0);
				Assert.AreEqual(listClaimProcsZeroed[i].InsPayAmt,0);
				Assert.AreEqual(listClaimProcsZeroed[i].InsPayEst,0);
			}
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.RpOutstandingIns_ZeroClaim_SentClaim2ClaimProcsNotRecieved1Payment)]
		[Documentation.VersionAdded("22.3")]
		[Documentation.Description(@"Creates a claim with two ClaimProcs attached. One of the ClaimProcs has a Status of 'Not Received' and the other has a Status of 'Preauth'. One of the ClaimProcs has a ClaimPayment attached, the other does not. The Claim is cleared by setting it's Status to 'Received' and it's DateReceived to today. Each ClaimProc has it's Writeoff, InsPayAmt, and InsPayEst set to 0 if it's Status is 'PreAuth' or 'Not Received' and it has no ClaimPayment attached.")]
		public void RpOutstandingIns_ZeroClaim_SentClaim2ClaimProcsNotRecieved1Payment() {
			//Create test patient
			Patient patient=PatientT.CreatePatient();
			//Create 2 procs to attach to claim
			List<Procedure> listProcedures=new List<Procedure>();
			listProcedures.Add(ProcedureT.CreateProcedure(patient,"D1234",ProcStat.C,"1",25));
			listProcedures.Add(ProcedureT.CreateProcedure(patient,"D4321",ProcStat.C,"2",50));
			//Create insurance for claim
			InsuranceInfo insuranceInfo=InsuranceT.AddInsurance(patient,"Test Carrier");
			//Create claim 			
			Claim claim=ClaimT.CreateClaim(listProcedures,insuranceInfo);
			List<ClaimProc>listClaimProcs=ClaimProcs.RefreshForClaim(claim.ClaimNum);
			//Create a dummy claim payment for the first claimproc
			ClaimPayment claimPayment=new ClaimPayment();
			claimPayment.BankBranch="Test Bank";
			claimPayment.CarrierName="Test Carrier";
			claimPayment.CheckAmt=25;
			claimPayment.CheckDate=DateTime.Today;
			claimPayment.CheckNum="1234567890";
			listClaimProcs[0].ClaimPaymentNum=ClaimPayments.Insert(claimPayment);
			ClaimProcs.Update(listClaimProcs[0]);
			listClaimProcs[1].Status=ClaimProcStatus.Preauth;//status' are now preauth and not rec'd
			ClaimProcs.Update(listClaimProcs[1]);
			RpOutstandingIns.ZeroClaim(claim);
			List<ClaimProc> listClaimProcsZeroed=ClaimProcs.RefreshForClaim(claim.ClaimNum);
			//Assert that the claim status is received
			Assert.AreEqual("R",claim.ClaimStatus);
			//Assert that the date received is today
			Assert.AreEqual(DateTime.Today.ToShortDateString(),claim.DateReceived.ToShortDateString());
			//Assert that all claimprocs were zeroed correctly
			for(int i=0;i<listClaimProcs.Count;i++) {
				//Claims with these conditions should not have been zeroed
				if(!listClaimProcs[i].Status.In(ClaimProcStatus.Preauth,ClaimProcStatus.NotReceived) || listClaimProcs[i].ClaimPaymentNum>0) {
					Assert.AreEqual(listClaimProcs[i].Status,listClaimProcsZeroed[i].Status);
					Assert.AreEqual(listClaimProcs[i].WriteOff,listClaimProcsZeroed[i].WriteOff);
					Assert.AreEqual(listClaimProcs[i].InsPayAmt,listClaimProcsZeroed[i].InsPayAmt);
					Assert.AreEqual(listClaimProcs[i].InsPayEst,listClaimProcsZeroed[i].InsPayEst);
				}
				else {
					Assert.AreEqual(listClaimProcsZeroed[i].Status,ClaimProcStatus.Received);
					Assert.AreEqual(listClaimProcsZeroed[i].WriteOff,0);
					Assert.AreEqual(listClaimProcsZeroed[i].InsPayAmt,0);
					Assert.AreEqual(listClaimProcsZeroed[i].InsPayEst,0);
				}
			}
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.RpOutstandingIns_ZeroClaim_SentClaim2ClaimProcs1NotReceived1ReceivedNoPayment)]
		[Documentation.VersionAdded("22.2")]
		[Documentation.Description(@"Creates a claim with two ClaimProcs attached. One of the ClaimProcs has a Status of 'Not Received' and the other has a Status of 'Preauth'. One of the ClaimProcs has a ClaimPayment attached, the other does not. The Claim is cleared by setting it's Status to 'Received' and it's DateReceived to today. Each ClaimProc has it's Writeoff, InsPayAmt, and InsPayEst set to 0 if it's Status is 'PreAuth' or 'Not Received' and it has no ClaimPayment attached.")]
		public void RpOutstandingIns_ZeroClaim_SentClaim2ClaimProcs1NotReceived1ReceivedNoPayment() {
			//Create test patient
			Patient patient=PatientT.CreatePatient();
			//Create 2 procs to attach to claim
			List<Procedure> listProcedures=new List<Procedure>();
			listProcedures.Add(ProcedureT.CreateProcedure(patient,"D1234",ProcStat.C,"1",25));
			listProcedures.Add(ProcedureT.CreateProcedure(patient,"D4321",ProcStat.C,"2",50));
			//Create insurance for claim
			InsuranceInfo insuranceInfo=InsuranceT.AddInsurance(patient,"Test Carrier");
			//Create claim 			
			Claim claim=ClaimT.CreateClaim(listProcedures,insuranceInfo);
			List<ClaimProc>listClaimProcs=ClaimProcs.RefreshForClaim(claim.ClaimNum);
			//Set the first claimproc to have a status of rec'd. Should not be zeroed.
			listClaimProcs[0].Status=ClaimProcStatus.Received;
			ClaimProcs.Update(listClaimProcs[0]);
			RpOutstandingIns.ZeroClaim(claim);
			List<ClaimProc> listClaimProcsZeroed=ClaimProcs.RefreshForClaim(claim.ClaimNum);
			//Assert that the claim status is received
			Assert.AreEqual("R",claim.ClaimStatus);
			//Assert that the date received is today
			Assert.AreEqual(DateTime.Today.ToShortDateString(),claim.DateReceived.ToShortDateString());
			//Assert that all claimprocs were zeroed correctly
			for(int i=0;i<listClaimProcs.Count;i++) {
				//Claims with these conditions should not have been zeroed
				if(!listClaimProcs[i].Status.In(ClaimProcStatus.Preauth,ClaimProcStatus.NotReceived) || listClaimProcs[i].ClaimPaymentNum>0) {
					Assert.AreEqual(listClaimProcs[i].Status,listClaimProcsZeroed[i].Status);
					Assert.AreEqual(listClaimProcs[i].WriteOff,listClaimProcsZeroed[i].WriteOff);
					Assert.AreEqual(listClaimProcs[i].InsPayAmt,listClaimProcsZeroed[i].InsPayAmt);
					Assert.AreEqual(listClaimProcs[i].InsPayEst,listClaimProcsZeroed[i].InsPayEst);
				}
				else {
					Assert.AreEqual(listClaimProcsZeroed[i].Status,ClaimProcStatus.Received);
					Assert.AreEqual(listClaimProcsZeroed[i].WriteOff,0);
					Assert.AreEqual(listClaimProcsZeroed[i].InsPayAmt,0);
					Assert.AreEqual(listClaimProcsZeroed[i].InsPayEst,0);
				}
			}
		}

		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.RpOutstandingIns_ZeroClaim_CanadaSentClaim1LabFeeNoPayment)]
		[Documentation.VersionAdded("22.2")]
		[Documentation.Description(@"Creates a canadian claim with two ClaimProcs attached. One on the ClaimProcs is a lab proc and the other is the LabProcs' parent. Both the ClaimProcs have a status of 'Not Received'. The Claim is cleared by setting it's Status to 'Received' and it's DateReceived to today. Each ClaimProc has it's Writeoff, InsPayAmt, and InsPayEst set to 0 if it's Status is 'PreAuth' or 'Not Received' and it has no ClaimPayment attached.")]
		public void RpOutstandingIns_ZeroClaim_CanadaSentClaim1LabFeeNoPayment() {
			CultureInfo cultureInfoOld=SetCulture(new CultureInfo("en-CA"));
			Patient patient = PatientT.CreatePatient();			
			List<Procedure> listProcedures=new List<Procedure>();
			//Parent and Lab match on all fields except ProcStatus
			Procedure procedureParent=ProcedureT.CreateProcedure(patient,"12345",ProcStat.TP,"",91.00);
			listProcedures.Add(procedureParent);
			Procedure procedureLab=ProcedureT.CreateProcedure(patient,"54321",ProcStat.TP,"",89.00,procedureParent.ProcDate,procNumLab:procedureParent.ProcNum);
			listProcedures.Add(procedureLab);
			//Create insurance for claim
			InsuranceInfo insuranceInfo = InsuranceT.AddInsurance(patient,"Test Carrier");
			//Create claim 			
			Claim claim = ClaimT.CreateClaim(listProcedures,insuranceInfo);
			List<ClaimProc> listClaimProcs = ClaimProcs.RefreshForClaim(claim.ClaimNum);
			RpOutstandingIns.ZeroClaim(claim);
			List<ClaimProc> listClaimProcsZeroed = ClaimProcs.RefreshForClaim(claim.ClaimNum);
			//Assert that the claim status is received
			Assert.AreEqual("R",claim.ClaimStatus);
			//Assert that the date received is today
			Assert.AreEqual(DateTime.Today.ToShortDateString(),claim.DateReceived.ToShortDateString());
			//Assert that all claimprocs were zeroed correctly
			for(int i = 0;i<listClaimProcs.Count;i++) {
				Assert.AreEqual(listClaimProcsZeroed[i].Status,ClaimProcStatus.Received);
				Assert.AreEqual(listClaimProcsZeroed[i].WriteOff,0);
				Assert.AreEqual(listClaimProcsZeroed[i].InsPayAmt,0);
				Assert.AreEqual(listClaimProcsZeroed[i].InsPayEst,0);
			}
			SetCulture(cultureInfoOld);
		}

		private CultureInfo SetCulture(CultureInfo targetCulture) {
			CultureInfo oldCulture=Application.CurrentCulture;
			Application.CurrentCulture=targetCulture;
			CultureInfo.DefaultThreadCurrentCulture=Application.CurrentCulture;
			CultureInfo.DefaultThreadCurrentUICulture=Application.CurrentCulture;
			return oldCulture;
		}
	}
}
