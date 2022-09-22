using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Data;

namespace OpenDental.Reporting.Allocators.MyAllocator1
{
	public enum LedgerItemTypes
		{
			/// <summary>
			/// A charge item in the ledger = 0
			/// </summary>
			Charge = 0,
			/// <summary>
			/// A payment item in the ledger  = 1
			/// </summary>
			Payment,
			/// <summary>
			/// A Negative Adjustment item in the ledger = 2
			/// </summary>
			NegAdjustment,
			/// <summary>
			/// A Positive Adjustment item in the ledger = 3.  ie a Refund
			/// for an overpayment will have a positive adjustment to it.
			/// </summary>
			PosAdjustment
		}

		#region Commented out entire PP_Ledger class as GuarantorLedgerItemsCollection seems to be what is really needed
		///// <summary>
	///// Class holding the definitions for the DansLedger Table
	///// </summary>
	//class PP_Ledger
	//{
	//    public enum ODTablesPulled { ProcedureLog, Adjustment, ClaimProc, PaySplit };
	//    public enum Columns
	//    {
	//        LedgerItemNum, LedgerITemType, Guarantor, PatNum, PRovNum,
	//        ProcNum, ItemDate, ItemAmt, AmtAllocated, IsAllocated,
	//        AllocatedString, TableSource
	//    };
		

	//    #region Public Static Fields
	//    public static readonly string Name = "DansLedger";
	//    public static readonly string OrignalTableName = "DansLedgerOriginalData";
	//    #region Commented out Code
	//    //public static string CreateTableString = "CREATE TABLE " + PaymentProcessor_Ledger.Name
	//    //               + " (LedgerItemNum bigint unsigned not null auto_increment primary key, "
	//    //               + " LedgerItemType smallint unsigned," // the type as defined in SQLReportGenerator.LedgerItemTypes
	//    //               + " Guarantor int unsigned, "  //The Person who is the Guarantor of this procedure
	//    //               + " PatNum int unsigned, " // The person the charge item is associated with, 0 for ItemTypes != Charge
	//    //               + " ProvNum smallint unsigned, " //The provider Num the item is assigned to.
	//    //           + " ProcNum int unsigned, "    //The Procedure Number in the ProcedureLog Table                        
	//    //           + " ItemDate DATE, " // The date the Item is posted to the ledger
	//    //           + " ItemAmt decimal(10,2), " //Amount of charge, payment, or adjustment
	//    //           + " AmtAllocated decimal(10,2) DEFAULT 0, " //amount of ledgeritem that has been allocated
	//    //           + " IsAllocated bool DEFAULT 0, " // indicates wether the full amount of item has been allocated
	//    //            + " AllocationString text DEFAULT '', "
	//    //            + " TableSource smallint DEFAULT -1)"; // A debuging field that indicates what payments/refund were allocated to this item
	//    //public static string CreateTableStringByName(string tableName)
	//    //{
	//    //    return "CREATE TABLE " + tableName
	//    //                 + " (LedgerItemNum bigint unsigned not null auto_increment primary key, "
	//    //                 + " LedgerItemType smallint unsigned," // the type as defined in SQLReportGenerator.LedgerItemTypes
	//    //                 + " Guarantor int unsigned, "  //The Person who is the Guarantor of this procedure
	//    //                 + " PatNum int unsigned, " // The person the charge item is associated with, 0 for ItemTypes != Charge
	//    //                 + " ProvNum smallint unsigned, " //The provider Num the item is assigned to.
	//    //             + " ProcNum int unsigned, "    //The Procedure Number in the ProcedureLog Table                        
	//    //             + " ItemDate DATE, " // The date the Item is posted to the ledger
	//    //             + " ItemAmt decimal(10,2), " //Amount of charge, payment, or adjustment
	//    //             + " AmtAllocated decimal(10,2) DEFAULT 0, " //amount of ledgeritem that has been allocated
	//    //             + " IsAllocated bool DEFAULT 0, " // indicates wether the full amount of item has been allocated
	//    //              + " AllocationString text, "
	//    //             + " TableSource smallint DEFAULT -1)"; // A debuging field that indicates what payments/refund were allocated to this item
	//    //}
	//    #endregion

	//    #endregion

	//    /// <summary>
	//    /// Copies all the items from OD tables to DansOriginalTable if they do not have the IsPulledToDans flag
	//    /// 
	//    /// </summary>
	//    public static bool CopyNonPulledItemsToOriginal(object sender, DoWorkEventArgs e)
	//    { throw new Exception("Non Implemented Yet. Flag1"); return false; }
	//    #region Commented Out
	//    //public static bool CopyNonPulledItemsToOriginal(object sender, DoWorkEventArgs e)
	//    //{
	//    //    if (sender != null)
	//    //        if (!(sender is BackgroundWorker))
	//    //            System.Windows.Forms.MessageBox.Show("sender is not a BackgroundWorker. Set Breakpoint\n"
	//    //                 + "in DansTables.DansLedger.MakeOriginal(object sender, DoWorkEventArgs e) to debug.");

	//    //    BackgroundWorker worker = null;
	//    //    if (sender != null)
	//    //        worker = (BackgroundWorker)sender;
	//    //    bool rSuccess = true;

	//    //    /// Make sure The ODTables in Question have an IsPulled Flag
	//    //    TableUtility tu = new TableUtility();
	//    //    bool ODTablesHaveFlags = tu.CheckODTables(true);
	//    //    tu = null;

	//    //    System.Data.DataTable dtWorking = null;
	//    //    DateTime StartTime = DateTime.Now;

	//    //    MySqlConnection con = new MySqlConnection(MasterConnectionData.GetConnectionString());
	//    //    try
	//    //    {
	//    //        // Find All Guarantors
	//    //        dtWorking = new System.Data.DataTable();
	//    //        //   MySqlDataAdapter da = new MySqlDataAdapter("SELECT Distinct(Guarantor) FROM Patient", con);
	//    //        // MySqlCommand cmd = new MySqlCommand(LedgerItems.GuarantorsWNonPulledItems(), con);
	//    //        MySqlDataAdapter da = new MySqlDataAdapter(LedgerItems.GuarantorsWNonPulledItems(), con);
	//    //        if (con.State != System.Data.ConnectionState.Open)
	//    //            con.Open();
	//    //        da.Fill(dtWorking);
	//    //        bool ExitEarly = false;
	//    //        int progress = 0;
	//    //        if (worker != null)
	//    //        {
	//    //            worker.ReportProgress(progress, "**SCROLL****LOG**Preparing to Clone Data from OpenDental to  "
	//    //                + PaymentProcessor_Ledger.OrignalTableName + ".");
	//    //            worker.ReportProgress(progress, "**SCROLL****LOG** ");
	//    //        }
	//    //        for (int i = 0; i < dtWorking.Rows.Count; i++)
	//    //        {
	//    //            progress = 100 * i / dtWorking.Rows.Count;
	//    //            uint Guarantor = uint.Parse(dtWorking.Rows[i][0].ToString());
	//    //            //uint Guarantor = 8;
	//    //            if (Guarantor == 3858)
	//    //                3858.ToString();
	//    //            // Fill OriginalTable -- //Skip Guarantors in the Excluded List
	//    //            if (Guarantor != 0 && !Guarantors.GuarantorIsInExcludedList(Guarantor)) // See comments on what Guarantor = 0 does
	//    //                ExitEarly = !TableUtility.CopyNonPulledItemsTo_DanLedgerOriginal(Guarantor);
	//    //            if (ExitEarly)
	//    //                1.ToString();
	//    //            if (worker != null)
	//    //            {
	//    //                worker.ReportProgress(progress, "Copied Guarantor data " + Guarantor
	//    //                     + " into " + PaymentProcessor_Ledger.OrignalTableName + ".");

	//    //                if (ExitEarly)
	//    //                    worker.ReportProgress(0, "**SCROLL****LOG**Problem with TableUtility.CopyNonPulldItemsTo_DansLedgerOrigional\n"
	//    //                             + " Called From DansLedger.CopyNonPulledItemsToOriginal look for 'ExitEarly' Flag   ");

	//    //                if (worker.CancellationPending) // Problem arros
	//    //                {

	//    //                    rSuccess = false;
	//    //                    e.Cancel = true;
	//    //                    i = dtWorking.Rows.Count;
	//    //                    return false;
	//    //                }
	//    //            }
	//    //        }





	//    //    }
	//    //    catch (Exception exc)
	//    //    {
	//    //        string s = exc.Message;
	//    //        System.Windows.Forms.MessageBox.Show("Error creating original table.  Did you drop it first?\n" + s);
	//    //        rSuccess = false;
	//    //    }
	//    //    finally
	//    //    {
	//    //        if (con.State == System.Data.ConnectionState.Open)
	//    //            con.Close();
	//    //    }
	//    //    DateTime EndTime = DateTime.Now;
	//    //    TimeSpan ts = new TimeSpan(EndTime.Ticks - StartTime.Ticks);
	//    //    return rSuccess;
	//    //}
	//    #endregion

		
	//} // end class PP_Ledger

#endregion
	/// <summary>
	/// PP is for Payment Proccessor
	/// </summary>
	class PP_LedgerItem : IComparable
	{

		#region Private Member Fields
		private LedgerItemTypes m_LedgerItemType;
		private uint m_Guarantor;
		private uint m_PatNum;
		private int m_ProvNum;
		private uint m_ProcNum;
		private DateTime m_ItemDate;
		private decimal m_ItemAmt;
		private ulong m_LedgerItemNum;
		private MyAllocator1.ODTablesUsed.ODTablesPulled m_TableSource;
		/// <summary>
		/// Indicates wether LedgerItem Needs Updated.
		/// Currently the only fields affected are:
		///   IsAllocated
		///   m_DebugString
		///   AmtAllocated  
		/// </summary>
		private bool m_IsChanged = false;
		/// <summary>
		/// Indicates wether the Charge or Refund Item is fully allocated
		/// to a payment or adjustment.
		/// </summary>
		//  private bool IsAllocated = false;
		private bool m_IsAllocated = false;

		/// <summary>
		/// Provides backward compatibility to GuarantorLedgerItemsCollection.  Not a PROPERTY but 
		/// and exposed Variable.  <B>Doesn't</B> flag IsChanged.  
		/// 
		/// 
		/// Watch this method because it may change m_IsAllocated when you do not want it to.
		/// </summary>
		public bool IsAllocated_OLD
		{
			get { return this.m_IsAllocated; }
			set { this.m_IsAllocated = value; this.m_IsChanged = true; }
		}

		/// <summary>
		/// Works with Property ALLOCATED_AMMOUNT
		/// </summary>
		private decimal m_AmtAllocated;
		private string m_DebugString = "";
		#endregion


		#region Constructors
		/// <summary>
		/// Used for creating LedgerItems that can be sorted in a FIFO Manner
		/// Input values are similar to the Table Columns.  Readonly properties provided.
		/// IComparable Interface Provided for sorting  
		/// 
		/// Strips time off of cDate
		/// 
		/// Set cLedgerItemNumber =0 if unknown.
		/// </summary>
		public PP_LedgerItem(LedgerItemTypes cLedgerItemType, uint cGuarantor, uint cPatNum, int cProvNum,
			uint cProcNum, DateTime cDate, decimal cItemAmt, ulong cLedgerItemNumber, MyAllocator1.ODTablesUsed.ODTablesPulled cSourceTable)
		{
			//this.m_LedgerItemType = cLedgerItemType;
			//this.m_Guarantor = cGuarantor;
			//this.m_PatNum = cPatNum;
			//this.m_ProvNum = cProvNum;
			//this.m_ProcNum = cProcNum;
			//this.m_ItemDate = new DateTime(cDate.Year, cDate.Month, cDate.Day);
			//this.m_ItemAmt = cItemAmt;
			//this.m_LedgerItemNum = cLedgerItemNumber;

			SetConstructorValues(cLedgerItemType, cGuarantor, cPatNum, cProvNum,
			 cProcNum, cDate, cItemAmt, cLedgerItemNumber, cSourceTable);

		}
		/// <summary>
		/// 2nd OverLoad has values IsAllocated, DebugString, AmtAllocated
		/// </summary>
		public PP_LedgerItem(LedgerItemTypes cLedgerItemType, uint cGuarantor, uint cPatNum, ushort cProvNum,
			uint cProcNum, DateTime cDate, decimal cItemAmt, ulong cLedgerItemNumber,
			MyAllocator1.ODTablesUsed.ODTablesPulled cSourceTable,
			bool IsAllocated1, decimal AmtAllocated1, string DebugString1)
		{


			SetConstructorValues(cLedgerItemType, cGuarantor, cPatNum, cProvNum,
			 cProcNum, cDate, cItemAmt, cLedgerItemNumber,cSourceTable,
			 IsAllocated1, DebugString1, AmtAllocated1);

		}
		public PP_LedgerItem(PP_LedgerItem DLI)
		{
			PP_LedgerItem CloneCopy = (PP_LedgerItem)DLI.MemberwiseClone();


			SetConstructorValues(CloneCopy.m_LedgerItemType, CloneCopy.m_Guarantor, CloneCopy.m_PatNum,
				CloneCopy.m_ProvNum, CloneCopy.m_ProcNum, CloneCopy.m_ItemDate, CloneCopy.m_ItemAmt,
				CloneCopy.m_LedgerItemNum, CloneCopy.m_TableSource);

		}
		private void SetConstructorValues(LedgerItemTypes cLedgerItemType, uint cGuarantor, uint cPatNum, int cProvNum,
			uint cProcNum, DateTime cDate, decimal cItemAmt, ulong cLedgerItemNumber, MyAllocator1.ODTablesUsed.ODTablesPulled cSourceTable)
		{
			this.m_LedgerItemType = cLedgerItemType;
			this.m_Guarantor = cGuarantor;
			this.m_PatNum = cPatNum;
			this.m_ProvNum = cProvNum;
			this.m_ProcNum = cProcNum;
			this.m_ItemDate = new DateTime(cDate.Year, cDate.Month, cDate.Day);
			this.m_ItemAmt = cItemAmt;
			this.m_LedgerItemNum = cLedgerItemNumber;
			this.m_TableSource = cSourceTable;

		}
		/// <summary>
		/// Last 3 parameters are different.  Used if want to set IsAllocated, DebugString,or AmtAllocated
		/// </summary>
		private void SetConstructorValues(LedgerItemTypes cLedgerItemType, uint cGuarantor, uint cPatNum, ushort cProvNum,
			uint cProcNum, DateTime cDate, decimal cItemAmt, ulong cLedgerItemNumber, MyAllocator1.ODTablesUsed.ODTablesPulled cSourceTable,
			bool IsAllocated1, string DebugString1, decimal AmtAllocated1)
		{
			SetConstructorValues(cLedgerItemType, cGuarantor, cPatNum, cProvNum,
			 cProcNum, cDate, cItemAmt, cLedgerItemNumber, cSourceTable);
			this.m_IsAllocated = IsAllocated1;
			this.m_DebugString = DebugString1;
			this.m_AmtAllocated = AmtAllocated1;


		}

		#endregion

		#region Public Properties
		#region Readonly Properties
		public LedgerItemTypes ITEMTYPE
		{ get { return this.m_LedgerItemType; } }
		public uint GUARANTOR { get { return this.m_Guarantor; } }
		public uint PATNUM { get { return this.m_PatNum; } }
		public int PROVNUM { get { return this.m_ProvNum; } }
		public uint PROCNUM { get { return this.m_ProcNum; } }
		public DateTime ITEMDATE { get { return this.m_ItemDate; } }
		public bool IS_CHANGED { get { return this.m_IsChanged; } }
		public ulong ITEMNUM { get { return this.m_LedgerItemNum; } }
		#endregion ReadonlyPropreties
		#region other public properties
		public decimal AMMOUNT
		{
			get { return this.m_ItemAmt; }
			set { this.m_ItemAmt = value; }
		}
		/// <summary>
		/// The unique number from the database assigned to this transaction
		/// </summary>

		/// <summary>
		/// A string that is usefull to see how things are allocated.  More used
		/// in debugging problems
		/// As allocation occurs string will be xxx,yyyy; xxxx2, yyyy2; xxx3,yyyy3
		/// where xxx is the LedgerItemNumber and yyyy is the amount alocated from this 
		/// ledger item number
		/// </summary>
		public string ALLOCATION_STRING
		{
			get { return this.m_DebugString; }
			set { this.m_DebugString = value; this.m_IsChanged = true; }
		}

		/// <summary>
		/// Gets or sets the amount of this ledger items that is allocated
		/// When this value is set IS_CHANGED becomes true.
		/// </summary>
		public decimal ALLOCATED_AMMOUNT
		{
			get { return this.m_AmtAllocated; }
			set
			{
				this.m_AmtAllocated = value;
				this.m_IsChanged = true;
				if (this.AmtUnallocated == 0)
					this.IS_ALLOCATED = true;
			}
		}
		/// <summary>
		/// Holds the 
		/// </summary>
		public MyAllocator1.ODTablesUsed.ODTablesPulled OD_TABLESOURCE
		{
			get { return this.m_TableSource; }
			set { // make sure value matches ODTableTypes
				this.m_TableSource = value; }
		}
		#endregion
		/// <summary>
		/// Expects caller to be manipulating AmtAllocated Field.
		/// So this just returns this.AMMOUNT - this.AmtAllocated
		/// </summary>
		public decimal AmtUnallocated
		{
			get { return this.AMMOUNT - this.m_AmtAllocated; }
		}

		/// <summary>
		/// Returns true if Item has been allocated.  Also set IS_CHANGED = true;
		/// </summary>
		public bool IS_ALLOCATED
		{
			get { return this.m_IsAllocated; }
			set { this.m_IsAllocated = value; this.m_IsChanged = true; }

		}
		#endregion

		#region Public Method

		public PP_PaymentItem CreatePaymentItem()
		{
			return new PP_PaymentItem(this.m_LedgerItemType, this.m_Guarantor, m_PatNum, m_ProvNum, m_ProcNum,
				m_ItemDate, m_ItemAmt, m_LedgerItemNum,m_TableSource);
		}
		#endregion

		//public string GetInsertString()
		//{
		//    return "INSERT INTO " + DansTables.DansLedger.Name
		//    + " ( LedgerItemType, Guarantor, PatNum, ProvNum, ProcNum, ItemDate, ItemAmt, "
		//    + "  AmtAllocated, IsAllocated,AllocationString) "
		//    + " Values ( "
		//    + ((int)this.ITEMTYPE).ToString() + ","
		//    + this.GUARANTOR.ToString() + ","
		//    + this.PATNUM.ToString() + ","
		//    + this.PROVNUM.ToString() + ","
		//    + this.PROCNUM.ToString() + ","
		//    + String.Format("'{0}-{1}-{2}',", this.ITEMDATE.Year, this.ITEMDATE.Month, this.ITEMDATE.Day)
		//    + this.AMMOUNT.ToString() + ","
		//    + this.m_AmtAllocated.ToString() + ","
		//    + ((int)(this.m_IsAllocated ? 1 : 0)).ToString() + ","
		//    + "'" + this.m_DebugString + "' "
		//    + ")";


		//}

		#region Used in EqulizePayments (version 1)
		/// <summary>
		/// Only to be used if you don't want to change the provider number
		/// assigned to this LedgerItem but you want 
		/// the ProviderNumber to Appear in the allocation string        
		/// </summary>
		/// <param name="LedgerItemNumber"></param>
		/// <param name="allocatedAmount"></param>
		/// <param name="ProvNum"></param>
		/// <returns></returns>
		public decimal AddAllocation(ulong LedgerItemNumber, decimal allocatedAmount, int ProvNum)
		{

			decimal rVal = this.AddAllocation(LedgerItemNumber, allocatedAmount);
			if (this.m_DebugString != "")
				this.m_DebugString += ";";
			this.m_DebugString += ProvNum + "," + rVal.ToString();
			return rVal;
		}
		/// <summary>
		/// Need a LedgerItemNumber to correlate allocation to
		/// ---Returns amount actually allocated.
		/// </summary>
		/// <param name="LedgerItemNumber"></param>
		/// <param name="allocatedAmount"></param>
		/// <returns>The amount actually allocated</returns>
		public decimal AddAllocation(ulong LedgerItemNumber, decimal allocatedAmount)
		{
			/// Note this.AmtUnallocated and allocatedAmount should normally have opposite signs 
			/// because you are allocating a payment (-ve) to a charge (+ve) or vice versa
			/// If a different situation is found you need to figure out why
			if (this.AmtUnallocated * allocatedAmount > 0)
				2.ToString(); // set breakpoint here

			decimal MaxAmount; // Want MaxAmmount oposite sinage of this.Ammount
			if (this.AMMOUNT > 0)
				MaxAmount = (this.AmtUnallocated >= -allocatedAmount ? -allocatedAmount : this.AmtUnallocated);

			else
				MaxAmount = (-this.AmtUnallocated >= allocatedAmount ? -allocatedAmount : this.AmtUnallocated);
			this.ALLOCATED_AMMOUNT = this.m_AmtAllocated + MaxAmount;
			if (this.AmtUnallocated == 0)
				this.m_IsAllocated = true;
			return MaxAmount;

		}
		#region Commented out Code
		/*
		/// <summary>
		/// Splits the current ledger item up into an array of items with different 
		/// payments and providers.  Each string in parameters should be of the form
		/// xxx,yyyy
		/// xxx= provider num
		/// yyyy = amount
		/// Throws exception if 'this' is not a payment or writeoff type.
		/// </summary>
		/// <param name="Parameters"></param>
		/// <returns></returns>
		public PP_LedgerItem[] SplitPaymentWriteOffItem(string[] Parameters)
		{
			//   bool b1 = this.m_LedgerItemType != LedgerItemTypes.NegAdjustment;
			//    bool b2 = this.m_LedgerItemType != LedgerItemTypes.Payment;
			//    bool b3 = b1 || b2;
			if (!(this.m_LedgerItemType != LedgerItemTypes.NegAdjustment || this.m_LedgerItemType != LedgerItemTypes.Payment))
				throw new Exception("SplitPaymentWriteoffItem in DansLedger called \nfrom an object not of Payment or writeoff type!");
			if (Parameters[0] == "")
				return null;
			List<PP_LedgerItem> rList = new List<PP_LedgerItem>();
			PP_LedgerItem temp = null;
			ushort CurrentProvider = UInt16.Parse(Parameters[0].Split(',')[0]);
			//decimal runningBalance = 0;
			for (int i = 0; i < Parameters.Length; i++)
			{
				string[] ProvAmount = Parameters[i].Split(',');
				ushort Prov = UInt16.Parse(ProvAmount[0]);
				decimal ammount = Decimal.Parse(ProvAmount[1]);
				if (temp == null)
				{
					temp = new PP_LedgerItem(this.ITEMTYPE, this.GUARANTOR, this.PATNUM,
						  Prov, this.PROCNUM, this.ITEMDATE, 0, 0);
					rList.Add(temp);
				}
				if (temp.PROVNUM == Prov)
				{
					temp.AMMOUNT += ammount;
					temp.ALLOCATED_AMMOUNT += ammount;

				}
				if (Prov != temp.PROVNUM)
				{
					temp.ALLOCATION_STRING = temp.PROVNUM.ToString() + "," + temp.AMMOUNT;
					temp.m_IsAllocated = true;
					temp.ALLOCATED_AMMOUNT = temp.m_AmtAllocated;

					temp = new PP_LedgerItem(this.ITEMTYPE, this.GUARANTOR, this.PATNUM,
						  Prov, this.PROCNUM, this.ITEMDATE, ammount, 0);
					rList.Add(temp);
				}

				temp.ALLOCATION_STRING = temp.PROVNUM.ToString() + "," + temp.AMMOUNT;
				temp.m_IsAllocated = true;
				// temp.AmtAllocated += -37;
				//   rList.Add(temp);
			}
			return rList.ToArray();


		}
		*/
		#endregion
		#endregion
		#region IComparable Members
		/// <summary>
		/// Returns -1 if Date of this is sooner than obj, 1 if later
		/// If Dates are the same then returns -1,0,1 by giving -1
		/// to the first items in this order Charge->Adjustment->Payment
		/// if ItemTypes are the same 0 is returned
		/// 
		/// </summary>
		public int CompareTo(object obj)
		{
			/* elected to elimiate this item and let the system throw the exception
			 * because this method will get called a lot and 
			 * I want to reduce overhead.
			if (!(obj is DansLedgerItem))
				throw new Exception("Obj passed to DansLedgerItem.CompareTo() is not a DansLedgerItem");
			*/
			PP_LedgerItem item2 = (PP_LedgerItem)obj;
			int rValue = this.ITEMDATE.CompareTo(item2.ITEMDATE);
			if (rValue == 0)
			{

				switch (this.ITEMTYPE)
				{
					case LedgerItemTypes.PosAdjustment:
						if (item2.ITEMTYPE == LedgerItemTypes.PosAdjustment)
							return 0;
						else if (item2.ITEMTYPE == LedgerItemTypes.Charge)
							return 1;
						else
							return -1;
						//break;
					case LedgerItemTypes.Charge:
						if (item2.ITEMTYPE == LedgerItemTypes.Charge)
							rValue = 0;
						else
							rValue = 1; // Charge should always come first. So compared object should compare bigger.
						break;
					case LedgerItemTypes.Payment:
						if (item2.ITEMTYPE == LedgerItemTypes.Payment)
							rValue = 0;
						else
							rValue = 1; // Payments always last
						break;
					case LedgerItemTypes.NegAdjustment:
						if (item2.ITEMTYPE == LedgerItemTypes.NegAdjustment)
							rValue = 0;
						else if (item2.ITEMTYPE == LedgerItemTypes.Charge || item2.ITEMTYPE == LedgerItemTypes.PosAdjustment)
							rValue = 1; //ie this Should appear later then item2
						else // item2.ITEMTYPE == LedgerItemType.Payment
							rValue = -1;
						break;
					
					default:
						break;
				}

			} // if (rValue == 0)
			return rValue;
		}//public int CompareTo(object obj)

		#endregion
	} // end class PP_LedgerItem

	class PP_PaymentItem : PP_LedgerItem
	{
		#region Private Members
		private List<PP_PaySplitItem> Splits = new List<PP_PaySplitItem>();
		#endregion

		#region Constructors

		public PP_PaymentItem(LedgerItemTypes cLedgerItemType, uint cGuarantor, uint cPatNum, int cProvNum,
			uint cProcNum, DateTime cDate, decimal cItemAmt, ulong cLedgerItemNumber, MyAllocator1.ODTablesUsed.ODTablesPulled cSourceTable)
			:
				base(cLedgerItemType, cGuarantor, cPatNum, cProvNum,
			 cProcNum, cDate, cItemAmt, cLedgerItemNumber, cSourceTable)
		{
		}
		#endregion

		#region Public Methods
		public void Add(PP_PaySplitItem splitItem)
		{
			Splits.Add(splitItem);
		}
		public void Remove(PP_PaySplitItem splitItem)
		{
			Splits.Remove(splitItem);
		}
		#endregion

		#region Public Properties

		public List<PP_PaySplitItem> PAYMENT_SPLITS { get { return this.Splits; } }
		#endregion

	}
}

