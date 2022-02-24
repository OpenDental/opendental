using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Threading;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using OpenDental;

namespace OpenDental.Reporting.Allocators.MyAllocator1
{   
	/// <summary> 
	/// Instantiate an Object of EqualizeProviderPayments.
	/// Set Connection String to QuerryResult.SetConnectionString(string)
	/// Call Instance.EqualizeGuarantorPayments(guarantor)
	/// use returned DataTable for your information.
	/// </summary>
    public class EqualizeProviderPayments
    {
        /// <summary>
        /// This does Several Things       
        ///     3)  Calls EqualizeLedger
        ///             -For the guarantors passed it Performs a FIFO Provider accounting of 
        ///             charges, Adjustments and Payments.  It will change the payment 
        ///             to be multiple payments if Provider Payments require it 
        ///             to be so.
        /// </summary>


        #region MemberVariables
        
        private static int InstanceCounter = 0;
		GuarantorLedgerItemsCollection Ledger = null;

       // private bool m_DansLedgerTableExists = false; 
        #endregion
#region Constructors & Destructors
        public EqualizeProviderPayments(BackgroundWorker bw)
        {
			GC.Collect(); // Found out that the object may not be collected.  If it is not then the finzlizer won't be in the que to call
			GC.WaitForPendingFinalizers(); // Note this can make a serious performance hit.
			
            if (InstanceCounter > 0)
                throw new Exception("Not supposed to make more than one instance of this object! Check it out");
            InstanceCounter++;
        }
        ~EqualizeProviderPayments()
        {
            InstanceCounter--;
        }
#endregion

		/// <summary>
		/// In the future this will check to make sure OD Tables are compliant with
		/// the setup that this was intended to run with.
        /// </summary>
        private static bool CheckTables()
        {
			return true;            
        }
		
		/// <summary>
		/// The method that actually calls the Equalizer.  Currently the equalizer just equalizes this instances
		/// data so that we can see the data via the returned DataTable
		/// 
		/// Note this Erases the old ledger item and replaces it with a new one for the Specified Guarantor
		/// </summary>
		/// <param name="uGuarantor"></param>
		/// <returns></returns>
		public DataTable EqualizeGuarantorPayments(int iGuarantor)
		{
			this.Ledger = new GuarantorLedgerItemsCollection(iGuarantor);

			Ledger.Fill(false);
			// Old call need to keep to figure out what I was thinking before
			//Ledger.FillFromDansLedgerOriginalTable(openConnection);
			Ledger.EqualizePaymentsV2();

			Ledger.FullLedger.Sort();
		//	int c2 = Ledger.FullLedger[0].CompareTo(Ledger.FullLedger[1]);

			DataTable dt = new DataTable();
			string[] cols = { "Item Date", "Patient", "Guarantor", "ItemType", 
							  "Provider",  "Charge",  "Payment",   "Balance" };
			foreach (string s in cols)
				dt.Columns.Add(new DataColumn(s));

			//GuarantorLedgerItemsCollection MixedCollection = new GuarantorLedgerItemsCollection(
			decimal runningBalance = 0;
			decimal runningCharges = 0;
			decimal runningPayments = 0;
			DateTime CurDate = DateTime.MinValue;
			foreach (PP_LedgerItem li in Ledger.FullLedger)
			{
				if (!(li is PP_PaymentItem))
				{
					runningBalance += li.AMMOUNT;
					runningCharges += li.AMMOUNT;
					DataRow dr = dt.NewRow();
					//string s3 = li.ITEMDATE.ToShortDateString();
					if (li.ITEMDATE > CurDate)
					{
						CurDate = li.ITEMDATE;
						dr[0] = li.ITEMDATE.ToShortDateString();
						if (dt.Rows.Count > 0)
							dt.Rows[dt.Rows.Count - 1][7] = runningBalance.ToString();
					}
					else
						dr[0] = "";
					dr[1] = li.PATNUM.ToString();
					dr[2] = li.GUARANTOR.ToString();
					dr[3] = ((MyAllocator1.LedgerItemTypes)li.ITEMTYPE).ToString();
					dr[4] = li.PROVNUM.ToString();
					dr[5] = li.AMMOUNT.ToString();
					dr[6] = ""; //Payment Column
					dt.Rows.Add(dr);

				}
				else
				{
					PP_PaymentItem payitem = (PP_PaymentItem)li;
					foreach (PP_PaySplitItem psi in payitem.PAYMENT_SPLITS)
					{
						runningBalance += psi.AMMOUNT;
						runningPayments += psi.AMMOUNT;
						DataRow dr = dt.NewRow();
						if (li.ITEMDATE > CurDate)
						{
							CurDate = payitem.ITEMDATE;
							dr[0] = payitem.ITEMDATE.ToShortDateString();
							if (dt.Rows.Count > 0)
								dt.Rows[dt.Rows.Count - 1][7] = runningBalance.ToString();
						}
						else
							dr[0] = "";
						dr[1] = payitem.PATNUM.ToString();
						dr[2] = payitem.GUARANTOR.ToString();
						dr[3] = ((MyAllocator1.LedgerItemTypes)payitem.ITEMTYPE).ToString();
						dr[4] = psi.PROVNUM.ToString();
						dr[5] = ""; // Charge Column
						dr[6] = psi.AMMOUNT.ToString();
						dt.Rows.Add(dr);

					}
				}
			}
			if (dt.Rows.Count > 0)
				dt.Rows[dt.Rows.Count - 1][7] = runningBalance.ToString();
			{
				dt.Rows.Add(dt.NewRow()); // blank line
				DataRow dr = dt.NewRow();
				dr[0] = CurDate.ToShortDateString();
				dr[4] = "Totals";
				dr[5] = runningCharges.ToString();
				dr[6] = runningPayments.ToString();
				dr[7] = runningBalance.ToString();
				dt.Rows.Add(dr);
			}
			return dt;
		}
		
		/// <summary>
		/// Provides a data table that gives a running balance for each provider.
		/// 
		/// Will run the EqualizeGuarantorPayments(guarantor) first if this instance has not 
		/// done so yet.
		/// 
		/// // Setup DataTable  
		/// 
		/// Date				   Provider #							 	      Provider #
		///			   Charges  Payments  Adjustments  Balances	    Charges  Payments  Adjustments Balances
		/// [ 0   ][1][   2   ][   3    ][     4     ][    5   ][6][    7  ][    8   ][  etc......  
		/// </summary>
		/// <param name="uGuarantor"></param>
		/// <returns></returns>
		public DataTable ProviderBalancesDetail()//uint uGuarantor)
		{
			DataTable dt = new DataTable();
			string [] cols = {"Date","Provider"};
			string[] subcols = { "Charges", "Payments", "Adjustment", "Balance" };
			if (Ledger == null)
				PU.Ex = "Guarantor not set in " + PU.Method;
			if (!Ledger.IS_FILLED)
				Ledger.Fill(false);			
			if (!Ledger.IS_EQUALIZED)
				Ledger.EqualizePaymentsV2();
			if (!Ledger.IS_FILLED || !Ledger.IS_EQUALIZED)
			{
				dt.Columns.Add("ERROR");
				dt.Rows.Add(dt.NewRow()[0] = "Error Getting table filled");
				return dt;
			}
			Ledger.FullLedger.Sort();

			// Generate List of Providers
			List<int> ProviderNums = new List<int>();
			foreach (PP_LedgerItem li in Ledger.FullLedger)
			{
				if (li is PP_PaymentItem)
				{
					foreach (PP_PaySplitItem psi in ((PP_PaymentItem)li).PAYMENT_SPLITS)
						if (!ProviderNums.Contains(psi.PROVNUM))
							ProviderNums.Add(psi.PROVNUM);
				}
				else
					if (!ProviderNums.Contains(li.PROVNUM))
						ProviderNums.Add(li.PROVNUM);
			}
			// Setup DataTable  
			/// 
			/// Date				   Provider #							 	      Provider #
			///			   Charges  Payments  Adjustments  Balances	    Charges  Payments  Adjustments Balances
			/// [ 0   ][1][   2   ][   3    ][     4     ][    5   ][6][    7  ][    8   ][  etc......  
			dt.Columns.Add(new DataColumn("Date"));
			System.Collections.Hashtable ht_dtProvNumOffsets = new System.Collections.Hashtable();
			for (int i = 0; i < ProviderNums.Count; i++)
			{
				dt.Columns.Add(new DataColumn("")); // Blank Column
				dt.Columns.Add(new DataColumn(""));
				dt.Columns.Add(new DataColumn(""));
				dt.Columns.Add(new DataColumn());
				dt.Columns.Add(new DataColumn(""));
				

			}
			// Add Header Rows
			DataRow dr1 = dt.NewRow();
			DataRow dr2 = dt.NewRow();
			dr2[0] = "Date";
			for (int i = 0; i < ProviderNums.Count; i++)
			{
				dr1[3+5*i] = "Provider";
				dr1[4+5*i] = "# " + ProviderNums[i].ToString() ;
				for (int j = 0; j < subcols.Length; j++)
					dr2[2 + 5 * i + j] = subcols[j]; // Charges, Payments, Adjustments, Balances
			}
			dt.Rows.Add(dr1);
			dt.Rows.Add(dr2);

			// Generate Provider Balances
			System.Collections.Hashtable htProvBalance = new System.Collections.Hashtable();
			System.Collections.Hashtable htProvBalance_Cummulative = new System.Collections.Hashtable();
			DateTime curDate = DateTime.MinValue;
			if (Ledger.FullLedger.Count != 0)
				curDate = Ledger.FullLedger[0].ITEMDATE;
			foreach (int ProvNum in ProviderNums)
			{
				htProvBalance[ProvNum] = new ProviderBalance(ProvNum, curDate);
				htProvBalance_Cummulative[ProvNum] = new ProviderBalance(ProvNum, curDate);
			}



			for (int i = 0; i < Ledger.FullLedger.Count; i++)
			{
				PP_LedgerItem li = Ledger.FullLedger[i];


				#region Making the DataRow
				if (curDate < li.ITEMDATE || i == Ledger.FullLedger.Count - 1)
				{

					DataRow dr3 = this.MakeProviderBalanceDataRow(curDate, dt, htProvBalance, htProvBalance_Cummulative, subcols, ProviderNums);
					dt.Rows.Add(dr3);

					foreach (int ProvNum in ProviderNums)
					{
						htProvBalance[ProvNum] = new ProviderBalance(ProvNum, li.ITEMDATE);
						((ProviderBalance)htProvBalance_Cummulative[ProvNum]).Date_of_Balance = li.ITEMDATE;
					}
					curDate = li.ITEMDATE;
				}
				#endregion

				#region Calculate Balances
				if (li is PP_PaymentItem)
				{
					foreach (PP_PaySplitItem psi in ((PP_PaymentItem)li).PAYMENT_SPLITS)
					{
						ProviderBalance pb = (ProviderBalance)htProvBalance[psi.PROVNUM];
						ProviderBalance pb_Cumulative = (ProviderBalance)htProvBalance_Cummulative[psi.PROVNUM];
						pb.Ammounts[(int)li.ITEMTYPE] += psi.AMMOUNT;
						pb_Cumulative.Ammounts[(int)li.ITEMTYPE] += psi.AMMOUNT;
					}
				}
				else
				{
					ProviderBalance pb = (ProviderBalance)htProvBalance[li.PROVNUM];
					ProviderBalance pb_Cumulative = (ProviderBalance)htProvBalance_Cummulative[li.PROVNUM];
					pb.Ammounts[(int)li.ITEMTYPE] += li.AMMOUNT;
					pb_Cumulative.Ammounts[(int)li.ITEMTYPE] += li.AMMOUNT;
				}

				#endregion

				
			}
			#region Adding the End DataRow
			
			{
				DataRow dr3 = this.MakeProviderBalanceDataRow(curDate,dt, htProvBalance, htProvBalance_Cummulative, subcols, ProviderNums);
				dt.Rows.Add(dr3);
			}
			#endregion

			return dt;
		}
		/// <summary>
		/// 2 Tables. Will return null if error getting table filled.
		/// Columns:  Provider, Ammount
		/// </summary>
		public DataTable ProviderBalancesOnly()//uint uGuarantor)
		{
			DataTable rVal = null;
			if (Ledger == null)
				PU.Ex = "Did not assign Guarantor in " + PU.Method;
			if (!Ledger.IS_FILLED)
				Ledger.Fill(false);
			if (!Ledger.IS_EQUALIZED)
				Ledger.EqualizePaymentsV2();
			if (!Ledger.IS_FILLED || !Ledger.IS_EQUALIZED)
			{
				//dt.Columns.Add("ERROR");
				//dt.Rows.Add(dt.NewRow()[0] = "Error Getting table filled");
				return null;
			}
			try
			{
				rVal = new DataTable();
				
				Ledger.FullLedger.Sort();
				System.Collections.Hashtable htProvBalance = new System.Collections.Hashtable();
				int[] providers = Ledger.Providers;
				decimal initalbalance = 0;
				foreach (int p in providers)
				{
					htProvBalance[p] = initalbalance;
				}
				foreach (PP_LedgerItem pli in Ledger.Charges)
				{
					htProvBalance[pli.PROVNUM] = ((decimal)htProvBalance[pli.PROVNUM]) + pli.AMMOUNT;
				}
				foreach (PP_PaymentItem pi in Ledger.Payments)
				{
					if (pi.PAYMENT_SPLITS != null)
					{
						foreach (PP_PaySplitItem psi in pi.PAYMENT_SPLITS)
						{
							htProvBalance[psi.PROVNUM] = ((decimal)htProvBalance[psi.PROVNUM]) + psi.AMMOUNT;
						}
					}
					else
						htProvBalance[pi.PROVNUM] = ((decimal)htProvBalance[pi.PROVNUM]) + pi.AMMOUNT;

				}
				DataColumn dc1 = new DataColumn("Provider", typeof(uint));
				DataColumn dc2 = new DataColumn("Ammount", typeof(decimal));
				rVal.Columns.Add(dc1);
				rVal.Columns.Add(dc2);

				//for (int i = 0; i < htProvBalance.Count; i++)
				foreach (int p in providers)
				{
					DataRow dr = rVal.NewRow();
					dr[0] = p;
					dr[1] = (decimal)htProvBalance[p];
					rVal.Rows.Add(dr);
				}
			}
			catch (Exception exc)
			{
				exc.ToString();  // just to elimnate compiler warning
			}
			
			return rVal;
		}

		/// <summary>
		/// Takes the ammount to allocate then provides a recommendation of where to allocate the
		/// charges to.  Returns a DataTable with the following Collumns:
		///		Provider, Ammount
		/// 
		/// The idea being that the calling method will have the payment in their hand and will
		/// use the recommendation to generate the appropriate PROVIDER paysplit info.  So the calling
		/// method will have the Date of Payment, Paytable source etc.
		/// </summary>
		/// <param name="AmountToAllocate">Should be -ve because it is a payment.</param>
		/// <returns></returns>
		public DataTable NextPaySplitRecommendation(decimal AmountToAllocate)//(uint uGuarantor, decimal AmountToAllocate)
		{
			DataTable rVal = new DataTable();
			if (!Ledger.IS_FILLED)
				Ledger.Fill(false);
			if (!Ledger.IS_EQUALIZED)
				Ledger.EqualizePaymentsV2();
			if (!Ledger.IS_FILLED || !Ledger.IS_EQUALIZED)
			{
				//dt.Columns.Add("ERROR");
				//dt.Rows.Add(dt.NewRow()[0] = "Error Getting table filled");
				return null;
			}
			List<ProviderPayment> psiList = new List<ProviderPayment>();
			ProviderPayment curPayItem = new ProviderPayment();
			curPayItem.Provider = -1;
			

			decimal remainingAmmount = AmountToAllocate;
			for (int i= 0; i < Ledger.Charges.Count; i++)
			{
				if (Ledger.Charges[i].AmtUnallocated > 0)
				{
					// if first time thru create curPayItem
					if (curPayItem.Provider == -1)
					{

						curPayItem = new ProviderPayment();
						curPayItem.Provider = Ledger.Charges[i].PROVNUM;
					}
					// if 2+ thru and ProvNums don't match create a new curPayItem
					if (curPayItem.Provider != Ledger.Charges[i].PROVNUM)
					{
						psiList.Add(curPayItem);
						curPayItem = new ProviderPayment();
						curPayItem.Provider = Ledger.Charges[i].PROVNUM;
					}
					// Determin what ammount to add
					if (Ledger.Charges[i].AmtUnallocated <= remainingAmmount)
					{
						curPayItem.Ammount += Ledger.Charges[i].AmtUnallocated;
						remainingAmmount -= Ledger.Charges[i].AmtUnallocated;
					}
					else
					{
						curPayItem.Ammount += remainingAmmount;
						remainingAmmount =0;
					}
					if (remainingAmmount == 0) i = Ledger.Charges.Count; // end loop
				}

			}

			/// Scenarios:  1:  Partial Ammount was allocated.  But curPayItem not recorded
			///				2:	No Ammount was allocated so a curPayItem needs recorded
			///				for 1:  Need to recored  curPayItem and Generate a new CurPayItem with no provider for the remaining
			if (remainingAmmount != AmountToAllocate)
			{
				// allocation occurred but not saved
				psiList.Add(curPayItem);
				// any remaining ammount?
				if (remainingAmmount != 0)
				{
					curPayItem = new ProviderPayment();
					curPayItem.Provider = -1;
					curPayItem.Ammount = remainingAmmount;
					remainingAmmount = 0;
					psiList.Add(curPayItem);
				}
			}
			else // No allocation occurred
			{
				curPayItem = new ProviderPayment();
				curPayItem.Provider = -1;
				curPayItem.Ammount = remainingAmmount;
				remainingAmmount = 0;
				psiList.Add(curPayItem);
			}
			// last item created was not added.

			/// Generate the returning table.
			DataColumn dc1 = new DataColumn("Provider", typeof(uint));
			DataColumn dc2 = new DataColumn("Ammount", typeof(decimal));
			rVal.Columns.Add(dc1);
			rVal.Columns.Add(dc2);

			foreach (ProviderPayment pi in psiList)
			{
				DataRow dr = rVal.NewRow();
				if (pi.Provider < 0)
					dr[0] = 0;
				else
					dr[0] = pi.Provider;
				dr[1] = pi.Ammount;
				rVal.Rows.Add(dr);
			}
			return rVal;

		}
		private DataRow MakeProviderBalanceDataRow(DateTime date, DataTable SourceSchema, 
			System.Collections.Hashtable htProvBalance,
			System.Collections.Hashtable htCumulativeBalance, 
			string [] subcols, List<int> ProviderNums)
		{
			
				//MakeDTRow;
			DataRow dr3 = SourceSchema.NewRow();
			dr3[0] = date.ToShortDateString();
				for (int j = 0; j < ProviderNums.Count; j++)
				{
					object o1 = htProvBalance["3"];
					ProviderBalance pb = (ProviderBalance)htProvBalance[ProviderNums[j]];
					ProviderBalance pb_Cumulative = (ProviderBalance)htCumulativeBalance[ProviderNums[j]];
					for (int k = 0; k < subcols.Length - 1; k++)
						if (pb.Ammounts[k] != 0 )
							dr3[2 + j * 5 + k] = pb.Ammounts[k];
					dr3[2 + j * 5 + subcols.Length - 1] = pb_Cumulative.BALANCE;
				}


				return dr3;
		}

		


       

       
    }
	/// <summary>
	/// Provides a snapshot for a provider's at any point in time.
	/// Ammounts is a decimal array holding the balances corresponding to LedgerItemTypes
	/// </summary>
	class ProviderBalance :IComparable
	{
		public DateTime Date_of_Balance = DateTime.MinValue;
		/// <summary>
		/// The for LedgerItemTypes form ammounts.  
		/// </summary>
		public decimal[] Ammounts = new decimal[Enum.GetNames(typeof(LedgerItemTypes)).Length];
		public decimal BALANCE
		{
			get
			{
				return Ammounts[(int)LedgerItemTypes.Charge] +
					Ammounts[(int)LedgerItemTypes.PosAdjustment] +
					Ammounts[(int)LedgerItemTypes.NegAdjustment] +
					Ammounts[(int)LedgerItemTypes.Payment];
			}
		}
		public int ProvNum = int.MaxValue;
		public ProviderBalance(int Provider, DateTime DateOfBalance)
		{
			ProvNum = Provider;
			Date_of_Balance = DateOfBalance;
		}
		#region IComparable Members

		public int CompareTo(object obj)
		{
			ProviderBalance that = (ProviderBalance)obj;
			if (this.Date_of_Balance == that.Date_of_Balance)
				return this.ProvNum.CompareTo(that.ProvNum);
			else
				return this.Date_of_Balance.CompareTo(that.Date_of_Balance);

		}

		#endregion

		/// <summary>
		/// Give returned value the date of the left hand operand. Exception if ProvNums are not the same
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <returns></returns>
		public static ProviderBalance operator + (ProviderBalance p1, ProviderBalance p2)
		{
			if (p1.ProvNum != p2.ProvNum)
				PU.Ex = "Provider numbers not the same when operand + was applied to ProviderBalance Type";
			ProviderBalance rVal = new ProviderBalance(p1.ProvNum, p1.Date_of_Balance);
			for (int i = 0; i < rVal.Ammounts.Length; i++)
				rVal.Ammounts[i] = p1.Ammounts[i] + p2.Ammounts[i];
			return rVal;
		}
	}
	struct ProviderPayment
	{
		public int Provider;
		public decimal Ammount;
	}
}

