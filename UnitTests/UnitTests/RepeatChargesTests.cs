using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenDentBusiness;
using UnitTestsCore;
using CodeBase;
using OpenDental;

namespace UnitTests.RepeatCharges_Tests {
	[TestClass]
	public class RepeatChargesTests:TestBase {

		///<summary>Creates patients with billing cycle day. Sets BillingUseBillingCycleDay preference to true. Creates an UpdateHistory 
		///entry for version 16.1.1.0. Deletes all current repeat charges.</summary>
		public Patient CreatePatForRepeatCharge(string suffix,int billingCycleDay) {
			Patient pat=PatientT.CreatePatient(suffix);
			Patient patOld=pat.Copy();
			pat.BillingCycleDay=billingCycleDay;
			Patients.Update(pat,patOld);
			PrefT.UpdateBool(PrefName.BillingUseBillingCycleDay,true);
			UpdateHistoryT.CreateUpdateHistory("16.1.1.0");
			Prefs.RefreshCache();
			List<RepeatCharge> listRepeatingCharges=RepeatCharges.Refresh(0).ToList();
			listRepeatingCharges.ForEach(x => RepeatCharges.Delete(x));
			return pat;
		}

		///<summary>Tests that repeat charges are added correctly after the stop date.
		///Scenario #1: The start day is before the stop day which is before the billing day. Start 12/08/2015, Stop 12/09/2015. Add a charge on 11/11/2015 and 12/11/2015.
		///Scenario #2: The start day is after the billing day which is after the stop day. Start 11/25/2015, Stop 12/01/2015. Add a charge on 12/11/2015.
		///Scenario #3: The start day is the same as the stop day but before the billing day. Start 11/10/2015, Stop 12/10/2015. Add a charge on 11/11/2015 and 12/11/2015.
		///Scenario #4: The start day is the same as the stop day and the billing day. Start 10/11/2015, Stop 11/11/2015. Add a charge on 11/11/2015.
		///Scenario #5: The start day is after the stop day which is after the billing day. Start 10/15/2015, Stop 11/13/2015. Add a charge on 11/11/2015.
		///Scenario #6: The start day is before the billing day which is before the stop day. Start 10/05/2015, Stop 11/20/2015. Add a charge on 11/11/2015.</summary>
		[Documentation.Description(@"Tests that repeat charges are added correctly after the stop date.

		Scenario #1: The start day is before the stop day which is before the billing day. Start 12/08/2015, Stop 12/09/2015. Add a charge on 11/11/2015 and 12/11/2015.

		Scenario #2: The start day is after the billing day which is after the stop day. Start 11/25/2015, Stop 12/01/2015. Add a charge on 12/11/2015.

		Scenario #3: The start day is the same as the stop day but before the billing day. Start 11/10/2015, Stop 12/10/2015. Add a charge on 11/11/2015 and 12/11/2015.

		Scenario #4: The start day is the same as the stop day and the billing day. Start 10/11/2015, Stop 11/11/2015. Add a charge on 11/11/2015.

		Scenario #5: The start day is after the stop day which is after the billing day. Start 10/15/2015, Stop 11/13/2015. Add a charge on 11/11/2015.

		Scenario #6: The start day is before the billing day which is before the stop day. Start 10/05/2015, Stop 11/20/2015. Add a charge on 11/11/2015.")]
		[Documentation.Numbering(Documentation.EnumTestNum.Legacy_TestFiftyThree)]
		[TestMethod]
		public void Legacy_TestFiftyThree() {
			//Repeat charges should be added after the stop date if the duration of the repeating charge if the number of charges added to the account is 
			//less than the number of months the repeat charge was active (a partial month is counted as a full month). 
			string suffix ="53";
			Patient pat=PatientT.CreatePatient(suffix);
			Patient patOld=pat.Copy();
			pat.BillingCycleDay=11;
			Patients.Update(pat,patOld);
			Prefs.UpdateBool(PrefName.BillingUseBillingCycleDay,true);
			UpdateHistoryT.CreateUpdateHistory("16.1.1.0");
			Prefs.RefreshCache();
			//delete all existing repeating charges
			List<RepeatCharge> listRepeatingCharges=RepeatCharges.Refresh(0).ToList();
			listRepeatingCharges.ForEach(x => RepeatCharges.Delete(x));
			DateTime dateRun=new DateTime(2015,12,15);
			//Subtest 1 =====================================================
			//The start day is before the stop day which is before the billing day. Add a charge after the stop date.
			RepeatCharge rc=new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";//arbitrary code
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(2015,11,8);
			rc.DateStop=new DateTime(2015,12,9);			
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			RepeatCharges.RunRepeatingCharges(dateRun);
			List<Procedure> procs=Procedures.Refresh(pat.PatNum);
			Assert.AreEqual(2,procs.Count);
			Assert.AreEqual(1,procs.Count(x => x.ProcDate==new DateTime(2015,11,11)));
			Assert.AreEqual(1,procs.Count(x => x.ProcDate==new DateTime(2015,12,11)));
			//Subtest 2 =====================================================
			//The start day is after the billing day which is after the stop day. Add a charge after the stop date.
			procs.ForEach(x => Procedures.Delete(x.ProcNum));
			RepeatCharges.Delete(rc);
			rc=new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(2015,10,25);
			rc.DateStop=new DateTime(2015,12,1);
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			RepeatCharges.RunRepeatingCharges(dateRun);
			procs=Procedures.Refresh(pat.PatNum);
			Assert.AreEqual(2,procs.Count);
			Assert.AreEqual(1,procs.Count(x => x.ProcDate==new DateTime(2015,11,11)));
			Assert.AreEqual(1,procs.Count(x => x.ProcDate==new DateTime(2015,12,11)));
			//Subtest 3 =====================================================
			//The start day is the same as the stop day but before the billing day. Add a charge after the stop date.
			procs.ForEach(x => Procedures.Delete(x.ProcNum));
			RepeatCharges.Delete(rc);
			rc=new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(2015,11,10);
			rc.DateStop=new DateTime(2015,12,10);
			RepeatCharges.Insert(rc);
			RepeatCharges.RunRepeatingCharges(dateRun);
			procs=Procedures.Refresh(pat.PatNum);
			Assert.AreEqual(2,procs.Count);
			Assert.AreEqual(1,procs.Count(x => x.ProcDate==new DateTime(2015,11,11)));
			Assert.AreEqual(1,procs.Count(x => x.ProcDate==new DateTime(2015,12,11)));
			//Subtest 4 =====================================================
			//The start day is the same as the stop day and the billing day. Don't add a charge after the stop date.
			procs.ForEach(x => Procedures.Delete(x.ProcNum));
			RepeatCharges.Delete(rc);
			dateRun=new DateTime(2015,11,15);
			rc=new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(2015,10,11);
			rc.DateStop=new DateTime(2015,11,11);
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			RepeatCharges.RunRepeatingCharges(dateRun);
			procs=Procedures.Refresh(pat.PatNum);
			Assert.AreEqual(2,procs.Count);
			Assert.AreEqual(1,procs.Count(x => x.ProcDate==new DateTime(2015,10,11)));
			Assert.AreEqual(1,procs.Count(x => x.ProcDate==new DateTime(2015,11,11)));
			//Subtest 5 =====================================================
			//The start day is after the stop day which is after the billing day. Don't add a charge after the stop date.
			procs.ForEach(x => Procedures.Delete(x.ProcNum));
			RepeatCharges.Delete(rc);
			rc=new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(2015,10,15);
			rc.DateStop=new DateTime(2015,11,13);
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			Procedure p=new Procedure();
			RepeatCharges.RunRepeatingCharges(dateRun);
			procs=Procedures.Refresh(pat.PatNum);
			Assert.AreEqual(1,procs.Count);
			Assert.AreEqual(1,procs.Count(x => x.ProcDate==new DateTime(2015,11,11)));
			//Subtest 6 =====================================================
			//The start day is before billing day which is before the stop day. Don't add a charge after the stop date.
			procs.ForEach(x => Procedures.Delete(x.ProcNum));
			RepeatCharges.Delete(rc);
			rc=new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(2015,11,05);
			rc.DateStop=new DateTime(2015,11,20);
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			RepeatCharges.RunRepeatingCharges(dateRun);
			procs=Procedures.Refresh(pat.PatNum);
			Assert.AreEqual(1,procs.Count);
			Assert.AreEqual(1,procs.Count(x => x.ProcDate==new DateTime(2015,11,11)));
		}

		///<summary>Tests that deleting a charge does not cause the wrong charges to be added back.
		///
		///Scenario #1: Add three repeating charges with a start date of 11/01/2015 and an amount of $100 with a note of 'Charge #1','Charge #2', and 
		///'Charge #3' respectively. Run repeating charge tool. Delete all the procedures with a date of 11/11/2015.Run repeating charge tool. Result 
		///should be a procedure with a billing note of 'Charge #1', a procedure with a billing note of 'Charge #2', a procedure note of 'Charge #3' 
		///on 11/11/2015, and a procedure with a billing note of 'Charge #1', a procedure with a billing note of 'Charge #2', and a procedure with a
		///billing note of 'Charge #3' on 12/11/2015. 
		///
		///Scenario #2: Add three repeating charges with a start date of 11/01/2015 and an amount of $100 with a note of 'Charge #1', 'Charge #2', and 
		///'Charge #3', respectively. Run repeating charge tool. Delete all the procedures with a date of 12/11/2015. Run repeating charge tool. Result 
		///should be a procedure with a billing note of 'Charge #1', a procedure with a billing note of 'Charge #2', a procedure with a billing note of 
		///'Charge #3' on 11/11/2015, and a procedure with a billing note of 'Charge #1', a procedure with a billing note of 'Charge #2', and a procedure 
		///with a billing note of 'Charge #3' on 12/11/2015. 
		///
		///Scenario #3: Add three repeating charges with a start date of 11/01/2015 and an amount of $100 with a note of 'Charge #1', 'Charge #2', and 
		///'Charge #3' respectively. Run repeating charge tool. Delete one procedure with a date of 11/11/2015. Run repeating charge tool. Result should 
		///be a procedure with a billing note of 'Charge #1', a procedure with a billing note of 'Charge #2', a procedure with a billing note of 
		///'Charge #3' on 11/11/2015, and a procedure with a billing note of 'Charge #1', a procedure with a billing note of 'Charge #2', a procedure with
		///a billing note of 'Charge #3' on 12/11/2015</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Legacy_TestFiftyFour)]
		[Documentation.Description(@"Tests that deleting a charge does not cause the wrong charges to be added back.

		Scenario #1: Add three repeating charges with a start date of 11/01/2015 and an amount of $100 with a note of 'Charge #1','Charge #2', and 'Charge #3' respectively. Run repeating charge tool. Delete all the procedures with a date of 11/11/2015.Run repeating charge tool. Result should be a procedure with a billing note of 'Charge #1', a procedure with a billing note of 'Charge #2', a procedure note of 'Charge #3' on 11/11/2015, and a procedure with a billing note of 'Charge #1', a procedure with a billing note of 'Charge #2', and a procedure with a billing note of 'Charge #3' on 12/11/2015. 
		
		Scenario #2: Add three repeating charges with a start date of 11/01/2015 and an amount of $100 with a note of 'Charge #1', 'Charge #2', and 'Charge #3', respectively. Run repeating charge tool. Delete all the procedures with a date of 12/11/2015. Run repeating charge tool. Result should be a procedure with a billing note of 'Charge #1', a procedure with a billing note of 'Charge #2', a procedure with a billing note of 'Charge #3' on 11/11/2015, and a procedure with a billing note of 'Charge #1', a procedure with a billing note of 'Charge #2', and a procedure with a billing note of 'Charge #3' on 12/11/2015. 
		
		Scenario #3: Add three repeating charges with a start date of 11/01/2015 and an amount of $100 with a note of 'Charge #1', 'Charge #2', and 'Charge #3' respectively. Run repeating charge tool. Delete one procedure with a date of 11/11/2015. Run repeating charge tool. Result should be a procedure with a billing note of 'Charge #1', a procedure with a billing note of 'Charge #2', a procedure with a billing note of 'Charge #3' on 11/11/2015, and a procedure with a billing note of 'Charge #1', a procedure with a billing note of 'Charge #2', a procedure with	a billing note of 'Charge #3' on 12/11/2015")]
		public void Legacy_TestFiftyFour() {
			//When there are multiple repeat charges on one account and the repeat charge tool is run, and then a procedure from the account is deleted, 
			//and then the repeat charges tool is run again, the same number of procedures that were deleted should be added.
			string suffix ="54";
			Patient pat=PatientT.CreatePatient(suffix);
			Patient patOld=pat.Copy();
			pat.BillingCycleDay=11;
			Patients.Update(pat,patOld);
			PrefT.UpdateBool(PrefName.BillingUseBillingCycleDay,true);
			PrefT.UpdateBool(PrefName.FutureTransDatesAllowed,true);
			UpdateHistoryT.CreateUpdateHistory("16.1.1.0");//Sets a timestamp that determines which logic we use to calculate repeate charge procedures
			Prefs.RefreshCache();
			List<RepeatCharge> listRepeatingCharges=RepeatCharges.Refresh(0).ToList();
			listRepeatingCharges.ForEach(x => RepeatCharges.Delete(x));
			DateTime dateRun=new DateTime(DateTime.Today.AddMonths(2).Year,DateTime.Today.AddMonths(2).Month,15);//The 15th of two months from now
			List<int> listFailedTests=new List<int>();
			RepeatCharge rc=new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(DateTime.Today.Year,DateTime.Today.Month,15);//The 15th of this month
			rc.Note="Charge #1";
			rc.CopyNoteToProc=true;
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			rc=new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(DateTime.Today.Year,DateTime.Today.Month,15);
			rc.Note="Charge #2";
			rc.CopyNoteToProc=true;
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			rc=new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(DateTime.Today.Year,DateTime.Today.Month,15);
			rc.Note="Charge #3";
			rc.CopyNoteToProc=true;
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			//Subtest 1 ===============================================================
			//There are three procedures with the same amount, proc code, and start date. Run the repeat charge tool. Delete all procedures from 
			//last month. Run the repeat charge tool again. Make sure that the correct repeat charges were added back.
			RepeatCharges.RunRepeatingCharges(dateRun);
			List<Procedure> procs=Procedures.Refresh(pat.PatNum);
			int lastMonth=dateRun.AddMonths(-1).Month;
			int thisMonth=dateRun.Month;
			//Delete all procedures from last month
			procs.FindAll(x => x.ProcDate.Month==lastMonth)
				.ForEach(x => Procedures.Delete(x.ProcNum));
			RepeatCharges.RunRepeatingCharges(dateRun);
			procs=Procedures.Refresh(pat.PatNum);
			//Make sure that the correct number of procedures were added using the correct repeating charges
			if(procs.Count!=6
				|| procs.FindAll(x => x.ProcDate.Month==lastMonth && x.BillingNote=="Charge #1").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==lastMonth && x.BillingNote=="Charge #2").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==lastMonth && x.BillingNote=="Charge #3").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==thisMonth && x.BillingNote=="Charge #1").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==thisMonth && x.BillingNote=="Charge #2").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==thisMonth && x.BillingNote=="Charge #3").Count!=1) {
				listFailedTests.Add(1);
			}
			//Subtest 2 ===============================================================
			//Run the repeat charge tool. Delete all procedures from this month. Run the repeat charge tool again. Make sure that the correct
			//repeat charges were added back.
			procs.ForEach(x => Procedures.Delete(x.ProcNum));
			RepeatCharges.RunRepeatingCharges(dateRun);
			procs=Procedures.Refresh(pat.PatNum);
			//Delete all procedures from this month
			procs.FindAll(x => x.ProcDate.Month==thisMonth)
				.ForEach(x => Procedures.Delete(x.ProcNum));
			RepeatCharges.RunRepeatingCharges(dateRun);
			procs=Procedures.Refresh(pat.PatNum);
			//Make sure that the correct number of procedures were added using the correct repeating charges
			if(procs.Count!=6
				|| procs.FindAll(x => x.ProcDate.Month==lastMonth && x.BillingNote=="Charge #1").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==lastMonth && x.BillingNote=="Charge #2").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==lastMonth && x.BillingNote=="Charge #3").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==thisMonth && x.BillingNote=="Charge #1").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==thisMonth && x.BillingNote=="Charge #2").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==thisMonth && x.BillingNote=="Charge #3").Count!=1) {
				listFailedTests.Add(2);
			}
			//Subtest 3 ===============================================================
			//Run the repeat charge tool. Delete one procedure from this month. Run the repeat charge tool again. Make sure that the correct
			//repeat charges were added back.
			procs.ForEach(x => Procedures.Delete(x.ProcNum));
			RepeatCharges.RunRepeatingCharges(dateRun);
			procs=Procedures.Refresh(pat.PatNum);
			//Delete one procedure from this month
			procs.FindAll(x => x.ProcDate.Month==thisMonth && x.BillingNote=="Charge #1")
				.ForEach(x => Procedures.Delete(x.ProcNum));
			RepeatCharges.RunRepeatingCharges(dateRun);
			procs=Procedures.Refresh(pat.PatNum);
			//Make sure that the correct number of procedures were added using the correct repeating charges
			if(procs.Count!=6
				|| procs.FindAll(x => x.ProcDate.Month==lastMonth && x.BillingNote=="Charge #1").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==lastMonth && x.BillingNote=="Charge #2").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==lastMonth && x.BillingNote=="Charge #3").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==thisMonth && x.BillingNote=="Charge #1").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==thisMonth && x.BillingNote=="Charge #2").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==thisMonth && x.BillingNote=="Charge #3").Count!=1) {
				listFailedTests.Add(3);
			}
			//Subtest 4 ===============================================================
			//Run the repeat charge tool. Delete one procedure from last month. Run the repeat charge tool again. Make sure that the correct
			//repeat charges were added back.
			procs.ForEach(x => Procedures.Delete(x.ProcNum));
			RepeatCharges.RunRepeatingCharges(dateRun);
			procs=Procedures.Refresh(pat.PatNum);
			//Delete one procedure from last month
			procs.FindAll(x => x.ProcDate.Month==lastMonth && x.BillingNote=="Charge #1")
				.ForEach(x => Procedures.Delete(x.ProcNum));
			RepeatCharges.RunRepeatingCharges(dateRun);
			procs=Procedures.Refresh(pat.PatNum);
			//Make sure that the correct number of procedures were added using the correct repeating charges
			if(procs.Count!=6
				|| procs.FindAll(x => x.ProcDate.Month==lastMonth && x.BillingNote=="Charge #1").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==lastMonth && x.BillingNote=="Charge #2").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==lastMonth && x.BillingNote=="Charge #3").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==thisMonth && x.BillingNote=="Charge #1").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==thisMonth && x.BillingNote=="Charge #2").Count!=1
				|| procs.FindAll(x => x.ProcDate.Month==thisMonth && x.BillingNote=="Charge #3").Count!=1) {
				listFailedTests.Add(4);
			}
			Assert.AreEqual(0,listFailedTests.Count);
		}

		///<summary>Tests that changing the amount or start date on a repeat charge does not cause an additional one to be added.
		///
		///Scenario #1: Add a repeating charge with a start date of 11/01/2015 and an amount of $100. Run repeating charge tool. Change the amount of the 
		///repeating charge to $80. Run repeating charge tool. Result should be a procedure of amount $100 on 11/11/2015 and a procedure of amount $100 
		///on 12/11/2015.
		///
		///Scenario #2: Add a repeating charge with a start date of 11/01/2015 and an amount of $100. Run repeating charge tool. Start date of the 
		///repeating charge to 11/02/2015. Run repeating charge tool. Result should be a procedure of amount $100 on 11/11/2015 and a procedure of amount 
		///$100 on 12/11/2015.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Legacy_TestFiftyFive)]
		[Documentation.Description(@"Tests that changing the amount or start date on a repeat charge does not cause an additional one to be added.

		Scenario #1: Add a repeating charge with a start date of 11/01/2015 and an amount of $100. Run repeating charge tool. Change the amount of the repeating charge $80. Run repeating charge tool. Result should be a procedure of amount $100 on 11/11/2015 and a procedure of amount $100 on 12/11/2015.

		Scenario #2: Add a repeating charge with a start date of 11/01/2015 and an amount of $100. Run repeating charge tool. Start date of the	repeating charge to 11/02/2015. Run repeating charge tool. Result should be a procedure of amount $100 on 11/11/2015 and a procedure of amount $100 on 12/11/2015.")]
		public void Legacy_TestFiftyFive() {
			//Changing the amount or start date on a repeat charge should not cause the repeat charge to be added again.
			string suffix ="55";
			Patient pat=PatientT.CreatePatient(suffix);
			Patient patOld=pat.Copy();
			pat.BillingCycleDay=11;
			Patients.Update(pat,patOld);
			Prefs.UpdateBool(PrefName.BillingUseBillingCycleDay,true);
			UpdateHistoryT.CreateUpdateHistory("16.1.1.0");
			Prefs.RefreshCache();
			List<RepeatCharge> listRepeatingCharges=RepeatCharges.Refresh(0).ToList();
			listRepeatingCharges.ForEach(x => RepeatCharges.Delete(x));
			DateTime dateRun=new DateTime(2015,12,15);
			List<int> listFailedTests=new List<int>();
			//Subtest 1 ===============================================================
			//Run the repeat charge tool. Change the charge amount on the repeat charge. Run the repeat charge tool again. Make sure that no  
			//extra procedures are added.
			RepeatCharge rc =new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(2015,11,1);
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			RepeatCharges.RunRepeatingCharges(dateRun);
			rc.ChargeAmt=80;
			RepeatCharges.Update(rc);
			RepeatCharges.RunRepeatingCharges(dateRun);
			List<Procedure> procs=Procedures.Refresh(pat.PatNum);
			if(procs.Count!=2
				|| procs.FindAll(x => x.ProcDate.ToShortDateString()=="11/11/2015" && x.ProcFee==99).Count!=1
				|| procs.FindAll(x => x.ProcDate.ToShortDateString()=="12/11/2015" && x.ProcFee==99).Count!=1) {
				listFailedTests.Add(1);
			}
			//Subtest 2 ===============================================================
			//Run the repeat charge tool. Change the start date on the repeat charge. Run the repeat charge tool again. Make sure that no  
			//extra procedures are added.
			procs.ForEach(x => Procedures.Delete(x.ProcNum));
			RepeatCharges.Delete(rc);
			rc=new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(2015,11,1);
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			RepeatCharges.RunRepeatingCharges(dateRun);
			rc.DateStart=new DateTime(2015,11,2);
			RepeatCharges.Update(rc);
			RepeatCharges.RunRepeatingCharges(dateRun);
			procs=Procedures.Refresh(pat.PatNum);
			if(procs.Count!=2
				|| procs.FindAll(x => x.ProcDate.ToShortDateString()=="11/11/2015" && x.ProcFee==99).Count!=1
				|| procs.FindAll(x => x.ProcDate.ToShortDateString()=="12/11/2015" && x.ProcFee==99).Count!=1) {
				listFailedTests.Add(2);
			}
			procs.ForEach(x => Procedures.Delete(x.ProcNum));
			RepeatCharges.Delete(rc);
			Assert.AreEqual(0,listFailedTests.Count);
		}

		///<summary>Tests that repeat charges are not posted before the start date.
		///Scenario #1: Add a repeat charge with a start date of 12/15/2015. Set the billing cycle day to 15. Run repeating charge tool. Result should be a procedure on 12/15/2015.
		///Scenario #2: Add a repeat charge with a start date of 12/15/2015. Set the billing cycle day to 12. Run repeating charge tool. Result should be no procedure added.
		///Scenario #3: Add a repeat charge with a start date of 12/18/2015. Set the billing cycle day to 15. Run repeating charge tool. Result should be no procedure added.</summary>
		[TestMethod]
		[Documentation.Numbering(Documentation.EnumTestNum.Legacy_TestFiftySix)]
		[Documentation.Description(@"Tests that repeat charges are not posted before the start date.

		Scenario #1: Add a repeat charge with a start date of 12/15/2015. Set the billing cycle day to 15. Run repeating charge tool. Result should be a procedure on 12/15/2015.

		Scenario #2: Add a repeat charge with a start date of 12/15/2015. Set the billing cycle day to 12. Run repeating charge tool. Result should be no procedure added.

		Scenario #3: Add a repeat charge with a start date of 12/18/2015. Set the billing cycle day to 15. Run repeating charge tool. Result should be no procedure added.")]
		public void Legacy_TestFiftySix() {
			//Repeat charges should not be posted before the start date.
			string suffix="56";
			Patient pat=PatientT.CreatePatient(suffix);
			Patient patOld=pat.Copy();
			pat.BillingCycleDay=15;
			Patients.Update(pat,patOld);
			Prefs.UpdateBool(PrefName.BillingUseBillingCycleDay,true);
			UpdateHistoryT.CreateUpdateHistory("16.1.1.0");
			Prefs.RefreshCache();
			List<RepeatCharge> listRepeatingCharges=RepeatCharges.Refresh(0).ToList();
			listRepeatingCharges.ForEach(x => RepeatCharges.Delete(x));
			DateTime dateRun=new DateTime(2015,12,15);
			List<int> listFailedTests=new List<int>();
			//Subtest 1 ===============================================================
			//The date start is the same as the date ran and the same as the billing day. Add a procedure that day.
			RepeatCharge rc =new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(2015,12,15);
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			RepeatCharges.RunRepeatingCharges(dateRun);
			List<Procedure> procs=Procedures.Refresh(pat.PatNum);
			if(procs.Count!=1 || procs.FindAll(x => x.ProcDate.ToShortDateString()=="12/15/2015" && x.ProcFee==99).Count!=1) {
				listFailedTests.Add(1);
			}
			//Subtest 2 ===============================================================
			//The start date is the same as the date ran but the billing day is three days earlier. Don't add a procedure that day.
			procs.ForEach(x => Procedures.Delete(x.ProcNum));
			RepeatCharges.Delete(rc);
			patOld=pat.Copy();
			pat.BillingCycleDay=12;
			Patients.Update(pat,patOld);
			rc=new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(2015,12,15);
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			RepeatCharges.RunRepeatingCharges(dateRun);
			procs=Procedures.Refresh(pat.PatNum);
			if(procs.Count!=0) {
				listFailedTests.Add(2);
			}
			//Subtest 3 ===============================================================
			//The start date is the same as the billing day but is three days after the date ran. Don't add a procedure that day.
			procs.ForEach(x => Procedures.Delete(x.ProcNum));
			RepeatCharges.Delete(rc);
			patOld=pat.Copy();
			pat.BillingCycleDay=15;
			Patients.Update(pat,patOld);
			rc=new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(2015,12,18);
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			RepeatCharges.RunRepeatingCharges(dateRun);
			procs=Procedures.Refresh(pat.PatNum);
			if(procs.Count!=0) {
				listFailedTests.Add(3);
			}
			Assert.AreEqual(0,listFailedTests.Count);
		}

		///<summary>There is a run date after the billing day but in the month following the month of a stop date.  Since the stop date is before the billing date
		///there should not be a charge in the month following the stop date. An existing procedure is on Billing day for month of stop date and prior month. 
		//Ex: Start: 10/15, Stop 11/30,BillCycleDay:13 and charges posted 10/13 and 11/13. Run date on 12/15. Don't add a procedure for 12/13.</summary>
		[TestMethod]
		public void RepeatCharges_ExistingProcsOnBillingDate() {
			Patient pat=CreatePatForRepeatCharge("RepeatCharge",15);
			RepeatCharge rc=new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(2017,10,15);
			rc.DateStop=new DateTime(2017,11,30);
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			DateTime dateRun=new DateTime(2017,10,15);
			RepeatCharges.RunRepeatingCharges(dateRun);
			List<Procedure> listProcs=Procedures.Refresh(pat.PatNum);
			Assert.AreEqual(1,listProcs.Count);
			Assert.AreEqual(1,listProcs.Count(x => x.ProcDate==new DateTime(2017,10,15)));
			dateRun=new DateTime(2017,11,15);
			RepeatCharges.RunRepeatingCharges(dateRun);
			listProcs=Procedures.Refresh(pat.PatNum);
			Assert.AreEqual(2,listProcs.Count);
			Assert.AreEqual(1,listProcs.Count(x => x.ProcDate==new DateTime(2017,10,15)));
			Assert.AreEqual(1,listProcs.Count(x => x.ProcDate==new DateTime(2017,11,15)));
			Patient patOld=pat.Copy();
			pat.BillingCycleDay=13;
			Patients.Update(pat,patOld);
			dateRun=new DateTime(2017,12,15);
			RepeatCharges.RunRepeatingCharges(dateRun);
			listProcs=Procedures.Refresh(pat.PatNum);
			Assert.AreEqual(2,listProcs.Count);
		}

		///<summary>Procedures with the ProcDate prior to the RepeatCharge.StartDate should not be considered as valid procedures 
		///associated to the current repeat charge.</summary>
		[TestMethod]
		public void RepeatCharges_RunRepeatingCharges_ProcDateBeforeRepeatChargeStartDate() {
			Patient pat=CreatePatForRepeatCharge("RepeatCharge",15);
			//add a repeat charge with start date 11/15/2017
			RepeatCharge rc=new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(2017,11,pat.BillingCycleDay);
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			//add a new procedure with procdate 10/15/2017
			Procedure p=new Procedure();
			p.PatNum=pat.PatNum;
			p.ProcStatus=ProcStat.C;
			p.ProcDate=new DateTime(2017,10,pat.BillingCycleDay);
			p.ProcFee=99;
			p.CodeNum=ProcedureCodeT.GetCodeNum("D2750");
			p.RepeatChargeNum=rc.RepeatChargeNum;
			Procedures.Insert(p);
			//add a new procedure with procdate 11/15/2017
			p=new Procedure();
			p.PatNum=pat.PatNum;
			p.ProcStatus=ProcStat.C;
			p.ProcDate=new DateTime(2017,11,pat.BillingCycleDay);
			p.ProcFee=99;
			p.CodeNum=ProcedureCodeT.GetCodeNum("D2750");
			p.RepeatChargeNum=rc.RepeatChargeNum;
			Procedures.Insert(p);
			DateTime dateRun=new DateTime(2017,12,pat.BillingCycleDay);
			RepeatCharges.RunRepeatingCharges(dateRun);
			List<Procedure> listProcs=Procedures.Refresh(pat.PatNum);
			Assert.AreEqual(3,listProcs.Count);
		}

		///<summary>Changing the the repeat charge start date of an existing repeat charge should add a procedure.
		///The new repeat charge start date has the same day.</summary>
		[TestMethod]
		public void RepeatCharges_RunRepeatingCharges_ChangeRepeatChargeStartDate() {
			Patient pat=CreatePatForRepeatCharge("RepeatCharge",15);
			//add a repeat charge with start 01/15/2017
			RepeatCharge rc=new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(2017,01,pat.BillingCycleDay);
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			//add a procedures with procdate 01/15/2017
			Procedure p=new Procedure();
			p.PatNum=pat.PatNum;
			p.ProcStatus=ProcStat.C;
			p.ProcDate=new DateTime(2017,01,pat.BillingCycleDay);
			p.ProcFee=99;
			p.CodeNum=ProcedureCodeT.GetCodeNum("D2750");
			p.RepeatChargeNum=rc.RepeatChargeNum;
			Procedures.Insert(p);
			//add a procedures with procdate 02/15/2017
			p=new Procedure();
			p.PatNum=pat.PatNum;
			p.ProcStatus=ProcStat.C;
			p.ProcDate=new DateTime(2017,02,pat.BillingCycleDay);
			p.ProcFee=99;
			p.CodeNum=ProcedureCodeT.GetCodeNum("D2750");
			p.RepeatChargeNum=rc.RepeatChargeNum;
			Procedures.Insert(p);
			//add a procedures with procdate 03/15/2017
			p=new Procedure();
			p.PatNum=pat.PatNum;
			p.ProcStatus=ProcStat.C;
			p.ProcDate=new DateTime(2017,03,pat.BillingCycleDay);
			p.ProcFee=99;
			p.CodeNum=ProcedureCodeT.GetCodeNum("D2750");
			p.RepeatChargeNum=rc.RepeatChargeNum;
			Procedures.Insert(p);
			//add a procedures with procdate 04/15/2017
			p=new Procedure();
			p.PatNum=pat.PatNum;
			p.ProcStatus=ProcStat.C;
			p.ProcDate=new DateTime(2017,04,pat.BillingCycleDay);
			p.ProcFee=99;
			p.CodeNum=ProcedureCodeT.GetCodeNum("D2750");
			p.RepeatChargeNum=rc.RepeatChargeNum;
			Procedures.Insert(p);
			//change the repeatcharge start date to 05/15/2017
			rc.DateStart=new DateTime(2017,05,pat.BillingCycleDay);
			RepeatCharges.Update(rc);
			DateTime dateRun=new DateTime(2017,05,pat.BillingCycleDay);
			RepeatCharges.RunRepeatingCharges(dateRun);
			List<Procedure> listProcs=Procedures.Refresh(pat.PatNum);
			Assert.AreEqual(5,listProcs.Count);
		}

		///<summary>Add a manual repeat charge after the repeat charge start date, then change the start date to after the proc date.</summary>
		[TestMethod]
		public void RepeatCharges_RunRepeatingCharges_ChangeRepeatChargeStartDateWithManuallyAddedProc() {
			Patient pat=CreatePatForRepeatCharge("RepeatCharge",15);
			//add a repeat charge with start 01/15/2017
			RepeatCharge rc=new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(2017,01,pat.BillingCycleDay);
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			//add a procedures with procdate 01/18/2017
			Procedure p=new Procedure();
			p.PatNum=pat.PatNum;
			p.ProcStatus=ProcStat.C;
			p.ProcDate=new DateTime(2017,01,18);
			p.ProcFee=99;
			p.CodeNum=ProcedureCodeT.GetCodeNum("D2750");
			p.RepeatChargeNum=rc.RepeatChargeNum;
			Procedures.Insert(p);
			//change the repeatcharge start date to 02/15/2017
			rc.DateStart=new DateTime(2017,02,pat.BillingCycleDay);
			RepeatCharges.Update(rc);
			DateTime dateRun=new DateTime(2017,02,pat.BillingCycleDay);
			RepeatCharges.RunRepeatingCharges(dateRun);
			List<Procedure> listProcs=Procedures.Refresh(pat.PatNum);
			Assert.AreEqual(2,listProcs.Count);
		}

		///<summary>Change repeat charge start date and patient billing day with multiple repeat charges.</summary>
		[TestMethod]
		public void RepeatCharges_RunRepeatingCharges_ChangeRepeatWithMultipleCharges() {
			Patient pat=CreatePatForRepeatCharge("RepeatCharge",9);
			//add a repeat charge with start 08/09/2017
			RepeatCharge rc=new RepeatCharge();
			rc.ChargeAmt=15;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(2017,08,09);
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			rc=new RepeatCharge();
			rc.ChargeAmt=99;
			rc.PatNum=pat.PatNum;
			rc.ProcCode="D2750";
			rc.IsEnabled=true;
			rc.DateStart=new DateTime(2017,08,09);
			rc.RepeatChargeNum=RepeatCharges.Insert(rc);
			DateTime dateRun=new DateTime(2018,05,09);
			RepeatCharges.RunRepeatingCharges(dateRun);
			List<Procedure> listProcs=Procedures.Refresh(pat.PatNum);
			Assert.AreEqual(4,listProcs.Count);
			//change the billing date to the 12th
			Patient patOld=pat.Copy();
			pat.BillingCycleDay=12;
			Patients.Update(pat,patOld);
			//second repeat charge has a new start date
			dateRun=new DateTime(2018,06,12);
			rc.DateStart=dateRun;
			RepeatCharges.Update(rc);
			RepeatCharges.RunRepeatingCharges(dateRun);
			listProcs=Procedures.Refresh(pat.PatNum);
			Assert.AreEqual(6,listProcs.Count);
		}

		///<summary>Runs repeat charges, updating preferences which track last run datetime.</summary>
		[TestMethod]
		public void RepeatCharges_RunRepeatingCharges_PrefsUpdate() {
			DateTime dateTimeLastRunExpected=new DateTime(2018,05,09);
			string strBeginDateTimeExpected="";//Will be empty after a successfully run Repeating Charges.
			RepeatCharges.RunRepeatingCharges(dateTimeLastRunExpected);
			Prefs.RefreshCache();
			Assert.AreEqual(strBeginDateTimeExpected,PrefC.GetString(PrefName.RepeatingChargesBeginDateTime));
			Assert.AreEqual(dateTimeLastRunExpected,PrefC.GetDateT(PrefName.RepeatingChargesLastDateTime));
		}
	}
}
