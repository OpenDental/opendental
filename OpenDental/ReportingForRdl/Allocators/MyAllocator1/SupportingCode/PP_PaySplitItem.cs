using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace OpenDental.Reporting.Allocators.MyAllocator1
{
	class PP_PaySplitItem
	{

		#region Private Member Fields
		//  private ulong m_SplitNum;        
		private decimal m_ItemAmt;
		private int m_ProvNum;
		//private ulong m_DansLedgerItemNum_Payment;

		private ulong m_AllocFromNum;
		private MyAllocator1.ODTablesUsed.ODTablesPulled m_AllocFromTbl;
		#endregion
	// 	public bool IsAllocated = false; // never was used

		#region Constructors
		/// <summary>
		/// The PP_PaySplitItem will be a paysplit that matches a certain charge.  The identity of the
		/// original charge is contained in the table indicated by AllocFromTable and procedure number in this
		/// table (as indicated by AllocFromProcNum).
		/// </summary>
		public PP_PaySplitItem(decimal Ammount, int Provider, 
			ulong AllocFromProcNum, MyAllocator1.ODTablesUsed.ODTablesPulled AllocFromTable)
		{
			m_ItemAmt = Ammount;
			m_ProvNum = Provider;
			m_AllocFromTbl = AllocFromTable;
			m_AllocFromNum = AllocFromProcNum;
			
		}
		public PP_PaySplitItem()
		{
		}
		#endregion

		#region Readonly Properties
		//     public ulong SPLITNUM { get { return this.m_SplitNum; } }
//		public ulong DL_NUM { get { return this.m_DansLedgerItemNum_Payment; } }
		public int PROVNUM { get { return this.m_ProvNum; } }

		public decimal AMMOUNT
		{
			get { return this.m_ItemAmt; }
			set { this.m_ItemAmt = value; }
		}
		/// <summary>The PP_PaySplit is matches a payment to a procedure or charge.  This contains a value that indicates what the table is. </summary>
		public MyAllocator1.ODTablesUsed.ODTablesPulled ALLOCATED_FROM_TABLE { get { return this.m_AllocFromTbl; } }
		/// <summary>The PP_PaySplit is matches a payment to a procedure or charge. This is the ProcNum or AdjNum or some other index Number that matches the  table indicated by ALLOCATED_FROM_TABLE</summary>
		public ulong ALLOCATED_FROM_NUM { get { return this.m_AllocFromNum; } }
		


		
		#endregion

	} // end class
}
