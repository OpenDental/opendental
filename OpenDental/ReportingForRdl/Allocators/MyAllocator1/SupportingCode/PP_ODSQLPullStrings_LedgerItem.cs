using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDental.Reporting.Allocators.MyAllocator1
{
	
	/// <summary>
	/// Used to Generate Query strings to pull the Ledger Data Required 
	/// to equalize payments from Opendental.
	/// </summary>
	class PP_ODSQLPullStrings_LedgerItem
	{
		/// <summary>
		/// Can be used to check to make sure queries return correct headers.
		/// </summary>
		public static readonly string[] HeaderOfQuery = 
		{ "LedgerItemType", "Guarantor", "PatNum",
			"ProvNum", "ItemDate", "Ammount", 
			"ItemNum", "TableSource"
		};
		/// <summary>
		/// Just for Reference Else where.  Should mathc HeaderOfQuery
		/// </summary>
		public enum HeaderOfQueryEnum 
		{ LedgerItemType, Guarantor, PatNum,
			ProvNum, ItemDate, Ammount, 
			ItemNum, TableSource
		};
		/// <summary>
		/// Whole idea here is to have a method that changes the column names to match
		/// what we believe the query should generate.  Throws an exception if
		/// the Column Count does not match that linsted in HeaderOfQuery
		/// </summary>
		/// <param name="dt"></param>
		public static void SetColumnNames(ref System.Data.DataTable dt)
		{
			if (dt.Columns.Count != HeaderOfQuery.Length)
				PU.Ex = "Column Count of DataTable passed to " + System.Reflection.MethodInfo.GetCurrentMethod().Name;
			else
				for (int i = 0; i < dt.Columns.Count; i++)
					dt.Columns[i].ColumnName = HeaderOfQuery[i];			
		}

		#region Inherited Members
		/// <summary>
		/// Provides a string that will pull ledger items for a specific guarantor.  If Guarantor is 0 it 
		/// will attempt to pull all items for all guarantors.
		/// </summary>
		/// <param name="Guarantor"></param>
		/// <param name="IsPulledFlag"></param>
		/// <returns></returns>
		public virtual string ODPullString(long Guarantor,bool IsPulledFlag)
		{
			return ""; // just used to override
		}
		#endregion

		/// <summary>
		/// Generates a Query string that should pull all the ledger items for a specific Guarantor.
		/// 
		/// If Guarantor = 0 then it will pull for all Guarantors which is a lot of data.
		/// So check for this condition.
		/// 
		/// <b>Note</b>: IsPulledFlag is currently depreciated 01-12-2008
		/// </summary>
		/// <param name="IsPulledFlag">indicates whether you want to pull items with the IsPulled status or not</param>
		/// <returns></returns>
		public static string PullAll_FromOD(long Guarantor,bool IsPulledFlag)
		{
			string rVal = "";
			L1_NegativeAdjustments L1 = new L1_NegativeAdjustments();
			L2_PositiveAdjustments L2 = new L2_PositiveAdjustments();
			L3_ClaimWriteOffAdjustments L3 = new L3_ClaimWriteOffAdjustments();
		//	L4_PaymentsFromPaySplit L4 = new L4_PaymentsFromPaySplit();
			// replaced with L4b_
			L4b_PaymentsFromPayment L4b = new L4b_PaymentsFromPayment();
			L5_PaymentsFromInsurance L5 = new L5_PaymentsFromInsurance();
			L6_Charges L6 = new L6_Charges();
			PP_ODSQLPullStrings_LedgerItem [] LArray = { L1, L2, L3, L4b, L5, L6 };
			for (int i = 0; i < LArray.Length; i++)
			{
				rVal += LArray[i].ODPullString(Guarantor, IsPulledFlag);
				if (i != LArray.Length - 1)
					rVal += "\n UNION ALL\n";
			}
			return rVal;
		}
	}
	#region Classes Providing SQL Segments to Pull from ODental
	class L1_NegativeAdjustments : PP_ODSQLPullStrings_LedgerItem
	{
		/// <summary>
		/// Provides the String to select NegativeAdjustments from OD Adjustment Table
		/// </summary>
		/// <param name="Guarantor"></param>
		/// <param name="IsPulledFlag">Which state of flag do you want to pull</param>
		/// <returns></returns>
		public override string ODPullString(long Guarantor,bool IsPulledFlag)
		{ // string NegAdjustmentUnion -- From GuarantorLedgerItem
			string rVal = "SELECT "
					+ "\n '" + (int) LedgerItemTypes.NegAdjustment + "', " //2
					+ "\n      patient.Guarantor, "
					+ "\n      patient.PatNum, "
					+ "\n      adjustment.ProvNum, "
					+ "\n      adjustment.ADjDate, " // AS 'ItemDate', " --was a problem if not first in union
					+ "\n      adjustment.AdjAmt, "
					+ "\n       Adjustment.AdjNum," // ProcNum not used For this Union
					+ "\n       " + (int)ODTablesUsed.ODTablesPulled.Adjustment
					+ "\n FROM adjustment, patient \n WHERE "
					+ "\n      adjustment.PatNum = Patient.PatNum "
					+ "\n      && adjustment.AdjAmt <= 0";
			if (Guarantor != 0)
				rVal += "\n      && Patient.Guarantor = " + Guarantor;

			//if (IsPulledFlag)
			//    rVal += "\n    && adjustment." + TableUtility.ColumnName_IsPulledToDansLedger + " = 1";
			//else
			//    rVal += "\n    && adjustment." + TableUtility.ColumnName_IsPulledToDansLedger + " = 0";
			return rVal;
		}


	}
	class L2_PositiveAdjustments : PP_ODSQLPullStrings_LedgerItem
	{
		public override string ODPullString(long Guarantor,bool IsPulledFlag)
		{
			//PosAdjustmentUnion
			string rVal = "SELECT "
						+ "\n '" + (int)LedgerItemTypes.PosAdjustment + "', " //2
						+ "\n      patient.Guarantor, "
						+ "\n      patient.PatNum, "
						+ "\n      adjustment.ProvNum, "
						+ "\n      adjustment.ADjDate, "
						+ "\n      adjustment.AdjAmt, " 
						+ "\n       adjustment.AdjNum," // 
					+ "\n       " + (int)ODTablesUsed.ODTablesPulled.Adjustment
						+ "\n FROM adjustment, patient \n WHERE "
						+ "\n      adjustment.PatNum = Patient.PatNum "
						+ "\n      && adjustment.AdjAmt > 0";
			if (Guarantor != 0)
				rVal += "\n      && Patient.Guarantor = " + Guarantor;
			//if (IsPulledFlag)
			//    rVal += "\n    && adjustment." + TableUtility.ColumnName_IsPulledToDansLedger + " = 1";
			//else
			//    rVal += "\n    && adjustment." + TableUtility.ColumnName_IsPulledToDansLedger + " = 0";
			return rVal;
		}
	}

	class L3_ClaimWriteOffAdjustments : PP_ODSQLPullStrings_LedgerItem
	{
		public override string ODPullString(long Guarantor,bool IsPulledFlag)
		{ //ClaimWriteOFFUnion
			string rVal = "SELECT "
					+ "\n      '" + (int)LedgerItemTypes.NegAdjustment + "', " //2
					+ "\n      patient.Guarantor, "
					+ "\n      patient.PatNum, "
					+ "\n      claimproc.ProvNum, " // Need to Check this to make sure includes 2ndary Provider
					+ "\n      claimpayment.CheckDate, "
					+ "\n      -(claimproc.WriteOff), "
					+ "\n       claimproc.ClaimProcNum," // 
					+ "\n       " + (int)ODTablesUsed.ODTablesPulled.ClaimProcWriteOff
					+ "\n FROM claimpayment,claimproc, patient \n WHERE "
					+ "\n      claimproc.PatNum = Patient.PatNum "
					+ "\n      && claimproc.ClaimPaymentNum = claimpayment.ClaimPaymentNum "
					+ "\n      && (claimproc.Status = 1 OR Claimproc.Status = 4)";
			if (Guarantor != 0)
				rVal += "\n      && Patient.Guarantor = " + Guarantor;
			//if (IsPulledFlag)
			//    rVal += "\n    && claimproc." + TableUtility.ColumnName_IsPulledToDansLedger + " = 1";
			//else
			//    rVal += "\n    && claimproc." + TableUtility.ColumnName_IsPulledToDansLedger + " = 0";
			return rVal;
		}


	}

	class L4_PaymentsFromPaySplit : PP_ODSQLPullStrings_LedgerItem
	{

		public override string ODPullString(long Guarantor,bool IsPulledFlag)
		{ //Payments1Union
			string rVal = "SELECT "
					+ "\n      '" + (int)LedgerItemTypes.Payment + "', " //1
					+ "\n      patient.Guarantor, "
					+ "\n      patient.PatNum, "
					+ "\n      0, " //Assums there is no provider #0
					+ "\n      paysplit.DatePay, "
					+ "\n      -paysplit.SplitAmt, "
					+ "\n       paysplit.SplitNum," // 
					+ "\n       " + (int)ODTablesUsed.ODTablesPulled.PaySplit
					+ "\n FROM paysplit,patient \n WHERE "
					+ "\n      paysplit.PatNum = Patient.PatNum ";
			if (Guarantor != 0)
				rVal += "\n      && Patient.Guarantor = " + Guarantor;
			//if (IsPulledFlag)
			//    rVal += "\n    && paysplit." + TableUtility.ColumnName_IsPulledToDansLedger + " = 1";
			//else
			//    rVal += "\n    && paysplit." + TableUtility.ColumnName_IsPulledToDansLedger + " = 0";
			return rVal;
		}
	}
	/// <summary>
	/// Used to replace L4_  when it was determined that we will not be using paysplit info
	/// </summary>
	class L4b_PaymentsFromPayment : PP_ODSQLPullStrings_LedgerItem
	{

		public override string ODPullString(long Guarantor,bool IsPulledFlag)
		{ //Payments1Union
			string rVal = "SELECT "
					+ "\n      '" + (int)LedgerItemTypes.Payment + "', " //1
					+ "\n      patient.Guarantor, "
					+ "\n      patient.PatNum, "
					+ "\n      0, " //Assums there is no provider #0
					+ "\n      payment.PayDate, "
					+ "\n      -payment.PayAmt, "
					+ "\n       payment.PayNum," // 
					+ "\n       " + (int)ODTablesUsed.ODTablesPulled.Payment
					+ "\n FROM payment,patient \n WHERE "
					+ "\n      payment.PatNum = Patient.PatNum ";
			if (Guarantor != 0)
				rVal += "\n      && Patient.Guarantor = " + Guarantor;
			//if (IsPulledFlag)
			//    rVal += "\n    && paysplit." + TableUtility.ColumnName_IsPulledToDansLedger + " = 1";
			//else
			//    rVal += "\n    && paysplit." + TableUtility.ColumnName_IsPulledToDansLedger + " = 0";
			return rVal;
		}
	}

	class L5_PaymentsFromInsurance : PP_ODSQLPullStrings_LedgerItem
	{
		public override string ODPullString(long Guarantor,bool IsPulledFlag)
		{//Payments2InsuranceUnion
			string rVal = "SELECT "
					+ "\n      '" + (int)LedgerItemTypes.Payment + "', " //1
					+ "\n      patient.Guarantor, "
					+ "\n      patient.PatNum, "
					+ "\n      0, " //Assums there is no provider #0
					+ "\n      claimpayment.CheckDate, "
					+ "\n      -claimproc.InsPayAmt, "
					+ "\n       claimproc.ClaimProcNum," // 
					+ "\n       " + (int)ODTablesUsed.ODTablesPulled.ClaimProcPayment
					+ "\n FROM claimproc,patient,claimpayment \n WHERE "
					+ "\n      claimproc.PatNum = Patient.PatNum "
					+ "\n      && claimpayment.ClaimPaymentNum = claimproc.ClaimPaymentNum ";
			if (Guarantor != 0)
				rVal += "\n      && Patient.Guarantor = " + Guarantor;
			//if (IsPulledFlag)
			//    rVal += "\n    && claimproc." + TableUtility.ColumnName_IsPulledToDansLedger + " = 1";
			//else
			//    rVal += "\n    && claimproc." + TableUtility.ColumnName_IsPulledToDansLedger + " = 0";
			return rVal;
		}
	}

	class L6_Charges : PP_ODSQLPullStrings_LedgerItem
	{
		public override string ODPullString(long Guarantor,bool IsPulledFlag)
		{ //ChargesCommandUnion
			string rVal =
				   "SELECT "
			   + "\n    " + (int)LedgerItemTypes.Charge + ","
			   + "\n    Patient.Guarantor,"
			   + "\n    Patient.PatNum,"
			   + "\n    Procedurelog.ProvNum,  "
			   + "\n    Procedurelog.ProcDate ,  "
			   + "\n    Procedurelog.ProcFee, "
			   + "\n    Procedurelog.ProcNum ," // 
					+ "\n       " + (int)ODTablesUsed.ODTablesPulled.ProcedureLog
			   + "\nFROM "
			   + "\n     Procedurelog, Patient"
			   + "\nWHERE "
			   + "\n     Patient.PatNum = Procedurelog.Patnum "  
			   + "\n     && Procedurelog.ProcStatus = " + ((int)OpenDentBusiness.ProcStat.C).ToString(); ;
			if (Guarantor != 0)
				rVal += "\n      && Patient.Guarantor = " + Guarantor;
			//if (IsPulledFlag)
			//    rVal += "\n    && Procedurelog." + TableUtility.ColumnName_IsPulledToDansLedger + " = 1";
			//else
			//    rVal += "\n    && Procedurelog." + TableUtility.ColumnName_IsPulledToDansLedger + " = 0";
			return rVal;
		}
	}
	#endregion
}
