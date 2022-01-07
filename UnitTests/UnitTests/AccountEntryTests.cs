using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenDentBusiness;
using UnitTestsCore;

namespace UnitTests {
	[TestClass]
	public class AccountEntryTests: TestBase {

		///<summary></summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.AccountEntry_MultipleAdjustments_ConsiderPatPayments)]
		[Documentation.VersionAdded("21.1")]
		[Documentation.Description("In the 'Add Multiple Adjustments' window, we have a procedure that costs $200. The patient makes a $20 payment and insurance is estimated to cover $30. If the office goes to make an adjustment,this should be calculated based off the remaining balance of the procedure, which will be $150.")]
		public void AccountEntry_MultipleAdjustments_ConsiderPatPayments() {
			//consider patient payments in final amount
			Patient pat=PatientT.CreatePatient(suffix:"Pat 1");
			Procedure proc=ProcedureT.CreateProcedure(pat,"D5010",ProcStat.C,"",200);//Procedure costs $200
			PaySplit split=PaySplitT.CreateOne(pat.PatNum,20,0,0,procNum:proc.ProcNum);//Patient makes $20 payment
			AccountEntry accountEntry=new AccountEntry();
			accountEntry.PatNum=pat.PatNum;
			accountEntry.ProcNum=proc.ProcNum;
			accountEntry.SplitCollection.Add(split);
			accountEntry.InsPayAmt=30;//$30 insurance payment
			accountEntry.Tag=proc;
			decimal retVal=AccountEntry.GetExplicitlyLinkedProcAmt(accountEntry,doConsiderPatPayments:true);
			Assert.AreEqual(150,retVal);
		}


		[TestMethod]
		public void AccountEntry_GetExplicitlyLinkedProcAmt_DontConsiderPatPayments() {
			//do not consider patient payments in final amount
			Patient pat=PatientT.CreatePatient(suffix:"Pat 1");
			Procedure proc=ProcedureT.CreateProcedure(pat,"D5010",ProcStat.C,"",200);//Procedure costs $200
			PaySplit split=PaySplitT.CreateOne(pat.PatNum,20,0,0,procNum:proc.ProcNum);//Patient makes $20 payment
			AccountEntry accountEntry=new AccountEntry();
			accountEntry.PatNum=pat.PatNum;
			accountEntry.ProcNum=proc.ProcNum;
			accountEntry.SplitCollection.Add(split);
			accountEntry.InsPayAmt=30;//$30 insurance payment
			accountEntry.Tag=proc;
			decimal retVal=AccountEntry.GetExplicitlyLinkedProcAmt(accountEntry,doConsiderPatPayments:false);
			Assert.AreEqual(170,retVal);
		}
	}
}
