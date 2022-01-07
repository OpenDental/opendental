/*=============================================================================================================
Open Dental is a dental practice management program.
Copyright (C) 2003,2004,2005,2006,2007  Jordan Sparks, DMD.  http://www.open-dent.com,  http://www.docsparks.com

This program is free software; you can redistribute it and/or modify it under the terms of the
GNU Db Public License as published by the Free Software Foundation; either version 2 of the License,
or (at your option) any later version.

This program is distributed in the hope that it will be useful, but without any warranty. See the GNU Db Public License
for more details, available at http://www.opensource.org/licenses/gpl-license.php

Any changes to this program must follow the guidelines of the GPL license if a modified version is to be
redistributed.
 * 
 * Original Author of this file is Daniel W. Krueger, DDS
===============================================================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using OpenDental;

namespace OpenDental.Reporting.Allocators.MyAllocator1
{
	/// <summary>
	/// A place to consolidate the queries for pulling data from opendental.
	/// A collection of classes that provide different sements of the SQL querry that pulls the different 
	/// opendental tables for the data we need to reconstruct a Ledger for a specific guarantor.
	/// 
	/// Call  LedgerItemsRaw(guarantor, IsPulled) to return a datatable with Structure Defined in Columns
	/// </summary>
	class LedgerItemsGenerator : LedgerItemsGeneratorInterface
	{
		#region Data Structure Defintions
		public static readonly int IndexofODItemNum = 5;
		public static readonly int IndexofODTableSource = 6;
		/// <summary>
		/// The columns in the data structor
		/// </summary>
		public static readonly string[] Columns = {"LedgerItemType", "Guarantor","PatientNum","ProviderNum","ItemDate",
            "Amount","ODItemNum","TableSource"}; // TablesSource is TableUtility.AvailableTables
		#endregion

		#region Connection Parameter
		private string _MySqlConnectionString = "";
		//private string _MyOracleConnectionString = ""; // Not implemented yet
		#endregion

		#region Enumerations
		public enum ColumnTypesPulledFromOD
		{
			Guarantor, PatientNum, ProviderNum, ItemDate,
			Amount, ODItemNum, TableSource
		};
		/// <summary>
		/// Used to distiquish what is the basic payment type.
		/// </summary>
		
		/// <summary>
		/// The tables from which data is pulled.  Put into an enumaration to minimize misspelling
		/// </summary>
		public enum ODTablesPulled { ProcedureLog, Adjustment, ClaimProc, PaySplit };
		#endregion

		#region Constructors
		public LedgerItemsGenerator(string MySqlConnectionString1)
		{
			this._MySqlConnectionString = MySqlConnectionString1;
		}
		public LedgerItemsGenerator() { }
		#endregion

		#region Virtual Methods
		/// <summary>
		/// Provides a string that will pull ledger items for a specific guarantor.  If Guarantor is 0 it 
		/// will attempt to pull all items for all guarantors.
		/// </summary>
		/// <param name="Guarantor"></param>
		/// <param name="IsPulledFlag"></param>
		/// <returns></returns>
		public virtual string ODPullString(int Guarantor, bool IsPulledFlag)
		{
		    throw new Exception("Method not Implemented Yet");
		    //return ""; // just used to override
		}
		#endregion

		#region Public Methods
		
		public DataTable LedgerItemsRaw(int Guarantor, bool IsPulledStatus)
		{
			DataTable rValdtWorking = new DataTable();
			MySqlConnection con = new MySqlConnection(this._MySqlConnectionString);
			try
			{
				con.Open();

				MySqlDataAdapter da = new MySqlDataAdapter(PP_ODSQLPullStrings_LedgerItem.PullAll_FromOD(Guarantor, IsPulledStatus), con);

				da.Fill(rValdtWorking);
				

			}
			catch (Exception exc)
			{
				exc.ToString();

			}
			finally
			{
				if (con.State == ConnectionState.Open)
					con.Close();
			}
			for (int i = 0; i < Columns.Length; i++) // If exception is thrown here I want to know it because it means my columns array is not right.
				rValdtWorking.Columns[i].ColumnName = Columns[i];
			return rValdtWorking;
		}
		/// <summary>
		/// Returns the Number of Guarantors
		/// </summary>
		/// <param name="Guarantor"></param>
		/// <param name="IsPulledStatus"></param>
		/// <returns></returns>
		public int LedgerItemsRawCount(int Guarantor, bool IsPulledStatus)
		{
			DataTable rValdtWorking = new DataTable();
			int ItemsInRawQuery = -1;
			MySqlConnection con = new MySqlConnection(this._MySqlConnectionString);
			try
			{
				con.Open();
				string command = PP_ODSQLPullStrings_LedgerItem.PullAll_FromOD(Guarantor, IsPulledStatus);
				command = "SELECT Count(Distinct (tb1.Guarantor)) FROM ( " + command + ") as tb1";
				MySqlDataAdapter da = new MySqlDataAdapter(command, con);
				da.Fill(rValdtWorking);
				if (rValdtWorking != null && rValdtWorking.Rows.Count != 0 && rValdtWorking.Columns.Count != 0)
					if (rValdtWorking.Rows[0][0] != null)
						ItemsInRawQuery = Int32.Parse(rValdtWorking.Rows[0][0].ToString());



			}
			catch (Exception exc)
			{
				exc.ToString();

			}
			finally
			{
				if (con.State == ConnectionState.Open)
					con.Close();
			}
			return ItemsInRawQuery;

		}

	
		#endregion

		#region Protected Methods
		/// <summary>
		/// The IsPulled Condtion was used to minimze the ammount of data that needed to be 
		/// accessed for reallocation by only dealling with new entries.  Currently We are not 
		/// using it but put it here in case I need it when I migrate it over to Opendental.
		/// </summary>
		/// <param name="IsPulled"></param>
		/// <returns></returns>
		protected string SQL_IsPulledCondition(ODTablesPulled table, bool IsPulled)
		{
			string rVal = "";
			if (IsPulled)
				throw new Exception("Method Not Implemented Yet");

			//Below is modified
			//rVal += String.Format("\n    && {0}.{1} = {2}", table.ToString().ToLower(), TableUtility.ColumnName_IsPulledToDansLedger, (IsPulled ? 1 : 0));

			//old version
			//if (IsPulledFlag)
			//    rVal += "\n    && adjustment." + TableUtility.ColumnName_IsPulledToDansLedger + " = 1";
			//else
			//    rVal += "\n    && adjustment." + TableUtility.ColumnName_IsPulledToDansLedger + " = 0";
			return rVal;
		}
		#endregion

	}

	/// <summary>
	/// Single required interface item - ODPullString(....)
	/// </summary>
	interface LedgerItemsGeneratorInterface
	{
		string ODPullString(int Guarantor,bool IsPulledFlag);
	}
	#region  Code Moved to PP_ODSQLPullStrings_LedgerItem -   Classes Providing SQL Segments to Pull from ODental
	//class L1_NegativeAdjustments :LedgerItemsGenerator
	//{
	//    #region LedgerItemsGeneratorInterface Members
	//    /// <summary>
	//    /// Provides the String to select NegativeAdjustments from OD Adjustment Table
	//    /// </summary>
	//    /// <param name="Guarantor"></param>
	//    /// <param name="IsPulledFlag">Which state of flag do you want to pull</param>
	//    /// <returns></returns>
	//    public string ODPullString(uint Guarantor, bool IsPulledFlag)
	//    { // string NegAdjustmentUnion -- From GuarantorLedgerItem
	//        string rVal = "SELECT "
	//                + "\n '" + (int)LedgerItemTypes.NegAdjustment + "', " //2
	//                + "\n      patient.Guarantor, "
	//                + "\n      patient.PatNum, "
	//                + "\n      adjustment.ProvNum, "
	//                + "\n      adjustment.ADjDate, " // AS 'ItemDate', " --was a problem if not first in union
	//                + "\n      adjustment.AdjAmt, "
	//                + "\n       Adjustment.AdjNum," // ProcNum not used For this Union
	//                + "\n       " + (int)ODTablesPulled.Adjustment
	//                + "\n FROM adjustment, patient \n WHERE "
	//                + "\n      adjustment.PatNum = Patient.PatNum "
	//                + "\n      && adjustment.AdjAmt <= 0";
	//        if (Guarantor != 0)
	//            rVal += "\n      && Patient.Guarantor = " + Guarantor;
	//        if (IsPulledFlag)
	//            rVal += SQL_IsPulledCondition(ODTablesPulled.Adjustment, IsPulledFlag);
	//        //if (IsPulledFlag)
	//        //    rVal += "\n    && adjustment." + TableUtility.ColumnName_IsPulledToDansLedger + " = 1";
	//        //else
	//        //    rVal += "\n    && adjustment." + TableUtility.ColumnName_IsPulledToDansLedger + " = 0";
	//        return rVal;
	//    }



		

		

	//    #endregion
	//}
	//class L2_PositiveAdjustments : LedgerItemsGenerator
	//{
	//    public override string ODPullString(uint Guarantor, bool IsPulledFlag)
	//    {
	//        //PosAdjustmentUnion
	//        string rVal = "SELECT "
	//                    + "\n '" + (int)LedgerItemTypes.PosAdjustment + "', " //2
	//                    + "\n      patient.Guarantor, "
	//                    + "\n      patient.PatNum, "
	//                    + "\n      adjustment.ProvNum, "
	//                    + "\n      adjustment.ADjDate, "
	//                    + "\n      adjustment.AdjAmt, "
	//                    + "\n       adjustment.AdjNum," // 
	//                + "\n       " + (int)ODTablesPulled.Adjustment
	//                    + "\n FROM adjustment, patient \n WHERE "
	//                    + "\n      adjustment.PatNum = Patient.PatNum "
	//                    + "\n      && adjustment.AdjAmt > 0";
	//        if (Guarantor != 0)
	//            rVal += "\n      && Patient.Guarantor = " + Guarantor;
	//        if (IsPulledFlag)
	//            rVal += SQL_IsPulledCondition(ODTablesPulled.Adjustment, IsPulledFlag);

	//        //if (IsPulledFlag)
	//        //    rVal += "\n    && adjustment." + TableUtility.ColumnName_IsPulledToDansLedger + " = 1";
	//        //else
	//        //    rVal += "\n    && adjustment." + TableUtility.ColumnName_IsPulledToDansLedger + " = 0";
	//        return rVal;
	//    }
	//}

	//class L3_ClaimWriteOffAdjustments : LedgerItemsGenerator
	//{
	//    public override string ODPullString(uint Guarantor, bool IsPulledFlag)
	//    { //ClaimWriteOFFUnion
	//        string rVal = "SELECT "
	//                + "\n      '" + (int)LedgerItemTypes.NegAdjustment + "', " //2
	//                + "\n      patient.Guarantor, "
	//                + "\n      patient.PatNum, "
	//                + "\n      claimproc.ProvNum, " // Need to Check this to make sure includes 2ndary Provider
	//                + "\n      claimpayment.CheckDate, "
	//                + "\n      -(claimproc.WriteOff), "
	//                + "\n       claimproc.ClaimProcNum," // 
	//                + "\n       " + (int)ODTablesPulled.ClaimProc
	//                + "\n FROM claimpayment,claimproc, patient \n WHERE "
	//                + "\n      claimproc.PatNum = Patient.PatNum "
	//                + "\n      && claimproc.ClaimPaymentNum = claimpayment.ClaimPaymentNum "
	//                + "\n      && (claimproc.Status = 1 OR Claimproc.Status = 4)";
	//        if (Guarantor != 0)
	//            rVal += "\n      && Patient.Guarantor = " + Guarantor;

	//        if (IsPulledFlag)
	//            rVal += SQL_IsPulledCondition(ODTablesPulled.ClaimProc, IsPulledFlag);
	//        //if (IsPulledFlag)
	//        //    rVal += "\n    && claimproc." + TableUtility.ColumnName_IsPulledToDansLedger + " = 1";
	//        //else
	//        //    rVal += "\n    && claimproc." + TableUtility.ColumnName_IsPulledToDansLedger + " = 0";
	//        return rVal;
	//    }


	//}

	//class L4_PaymentsFromPaySplit : LedgerItemsGenerator
	//{

	//    public override string ODPullString(uint Guarantor, bool IsPulledFlag)
	//    { //Payments1Union
	//        string rVal = "SELECT "
	//                + "\n      '" + (int)LedgerItemTypes.Payment + "', " //1
	//                + "\n      patient.Guarantor, "
	//                + "\n      patient.PatNum, "
	//                + "\n      0, " //Assums there is no provider #0
	//                + "\n      paysplit.DatePay, "
	//                + "\n      -paysplit.SplitAmt, "
	//                + "\n       paysplit.SplitNum," // 
	//                + "\n       " + (int)ODTablesPulled.PaySplit
	//                + "\n FROM paysplit,patient \n WHERE "
	//                + "\n      paysplit.PatNum = Patient.PatNum ";
	//        if (Guarantor != 0)
	//            rVal += "\n      && Patient.Guarantor = " + Guarantor;
	//        if (IsPulledFlag)
	//            rVal += SQL_IsPulledCondition(ODTablesPulled.PaySplit, IsPulledFlag);
	//        //if (IsPulledFlag)
	//        //    rVal += "\n    && paysplit." + TableUtility.ColumnName_IsPulledToDansLedger + " = 1";
	//        //else
	//        //    rVal += "\n    && paysplit." + TableUtility.ColumnName_IsPulledToDansLedger + " = 0";
	//        return rVal;
	//    }
	//}

	//class L5_PaymentsFromInsurance : LedgerItemsGenerator
	//{
	//    public override string ODPullString(uint Guarantor, bool IsPulledFlag)
	//    {//Payments2InsuranceUnion
	//        string rVal = "SELECT "
	//                + "\n      '" + (int)LedgerItemTypes.Payment + "', " //1
	//                + "\n      patient.Guarantor, "
	//                + "\n      patient.PatNum, "
	//                + "\n      0, " //Assums there is no provider #0
	//                + "\n      claimpayment.CheckDate, "
	//                + "\n      -claimproc.InsPayAmt, "
	//                + "\n       claimproc.ClaimProcNum," // 
	//                + "\n       " + (int)ODTablesPulled.ClaimProc
	//                + "\n FROM claimproc,patient,claimpayment \n WHERE "
	//                + "\n      claimproc.PatNum = Patient.PatNum "
	//                + "\n      && claimpayment.ClaimPaymentNum = claimproc.ClaimPaymentNum ";
	//        if (Guarantor != 0)
	//            rVal += "\n      && Patient.Guarantor = " + Guarantor;
	//        if (IsPulledFlag)
	//            rVal += SQL_IsPulledCondition(ODTablesPulled.ClaimProc, IsPulledFlag);
	//        //if (IsPulledFlag)
	//        //    rVal += "\n    && claimproc." + TableUtility.ColumnName_IsPulledToDansLedger + " = 1";
	//        //else
	//        //    rVal += "\n    && claimproc." + TableUtility.ColumnName_IsPulledToDansLedger + " = 0";
	//        return rVal;
	//    }
	//}

	//class L6_Charges : LedgerItemsGenerator
	//{
	//    public override string ODPullString(uint Guarantor, bool IsPulledFlag)
	//    { //ChargesCommandUnion
	//        string rVal =
	//               "SELECT "
	//           + "\n    " + (int)LedgerItemTypes.Charge + ","
	//           + "\n    Patient.Guarantor,"
	//           + "\n    Patient.PatNum,"
	//           + "\n    Procedurelog.ProvNum,  "
	//           + "\n    Procedurelog.ProcDate ,  "
	//           + "\n    Procedurelog.ProcFee, "
	//           + "\n    Procedurelog.ProcNum ," // 
	//                + "\n       " + (int)ODTablesPulled.ProcedureLog
	//           + "\nFROM "
	//           + "\n     Procedurelog, Patient"
	//           + "\nWHERE "
	//           + "\n     Patient.PatNum = Procedurelog.Patnum " 
	//           + "\n     && Procedurelog.ProcStatus = " + ((int)OpenDentBusiness.ProcStat.C).ToString(); ;
	//        if (Guarantor != 0)
	//            rVal += "\n      && Patient.Guarantor = " + Guarantor;
	//        if (IsPulledFlag)
	//            rVal += SQL_IsPulledCondition(ODTablesPulled.ProcedureLog, IsPulledFlag);
	//        //if (IsPulledFlag)
	//        //    rVal += "\n    && Procedurelog." + TableUtility.ColumnName_IsPulledToDansLedger + " = 1";
	//        //else
	//        //    rVal += "\n    && Procedurelog." + TableUtility.ColumnName_IsPulledToDansLedger + " = 0";
	//        return rVal;
	//    }
	//}
	#endregion
}
