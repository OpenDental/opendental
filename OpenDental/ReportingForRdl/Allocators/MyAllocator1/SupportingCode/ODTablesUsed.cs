using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDental.Reporting.Allocators.MyAllocator1
{
	public class ODTablesUsed
	{
		/// <summary>
		/// The tables from which data is pulled to go into DansLedger
		/// </summary>
		public enum ODTablesPulled { ProcedureLog, Adjustment, ClaimProcPayment, ClaimProcWriteOff, PaySplit, Payment };
		#region private static objects
		private static readonly List<ODTablesUsed> _TablesUsed = new List<ODTablesUsed>();
		private static string[] _ApprovedOD_Versions = { "5.1.0.0" };
		#endregion
		#region private static methods
		/// <summary>Throws Exception.  Usefull for unimplemented methods.  Would like to add reflection. </summary>
		void ThrowException() { throw new Exception("Method Not Implemented Yet"); }
		void ThrowException(string message) { throw new Exception(message); }
		#endregion
		#region public static methods
	
		/// <summary>Returns true if table is in the expected state to use with this dll. </summary>
		public bool TableStateValid(ODTablesUsed table)
		{
			bool rVal = false;
			ThrowException();
			return rVal;
		}
		/// <summary>Checks for the state of all tables in the _TablesUsed 
		/// and the dll is access from _ApprovedOD_Versions.  Just to make sure 
		/// dll is safe to run.  Put this here because of the very dynamic 
		/// state of opendental that if columns and tables get moved arround 
		/// this dll will become unstable creating bad things.
		/// </summary>
		public bool DllSafeToRun()
		{
			bool rVal = false;
			// check tables
			// check versions
			ThrowException();
			return rVal;
		}

		/// <summary>
		/// This is the main point where the tables used will be be built.
		/// </summary>
		private static void BuildStaticTableList()
		{
			string name = "ProcedureLog";
			string [] columns = { "col1" };
			Type[] types1 = { typeof(string) };
			ODTablesUsed table1 = new ODTablesUsed(name, columns, types1);
			//public enum ODTablesPulled { ProcedureLog, Adjustment, ClaimProc, PaySplit }
		}
		#endregion



		#region private instance objects
		private string _TableName;
		private string[] _ColumnsUsed = null;
		private Type[] _ColumnTypes = null;
		#endregion
		#region Constructors
		/// <summary>
		/// <param name="name">the OpenDental Table name that is used</param>
		/// <param name="columns">the array of column names that are used</param>
		/// <param name="types">the types of the columns listed in columns</param>
		public ODTablesUsed(string name, string [] columns, Type [] types1)
		{
			if (columns.Length != types1.Length)
				ThrowException("column count does not match type count in ODTablesUsed Constructor");
			if (columns == null  || types1 == null)
				ThrowException("null refernces not allowed in ODTablesUsed Constructor");
			_TableName = name;
			_ColumnsUsed = columns;
			_ColumnTypes = types1;
		}
		#endregion
		



	}
}
