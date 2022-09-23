using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDental.Reporting.Allocators.MyAllocator1
{
	class ODTableDefs
	{
	}
	class ODTable
	{
		public readonly string [] columns=null;
		public readonly Type [] columntypes=null;
		public readonly string name="";
		/// <summary>
		/// Just Throws an exception if setup is bad.  Call only from a constructor.
		/// </summary>
		protected void CheckODTableSetup()
		{
			if (columns == null || columntypes == null || columns.Length != columntypes.Length )
				throw new Exception("ODTable constructor Failed");
		}
		
	}
	class ProcedureLog:ODTable
	{
		
		public static readonly string ProcNum = "ProcNum";
		public static readonly Type ProcNumType = typeof(string);

		public ProcedureLog()
		{
			
			string[] cols = { ProcNum };
			Type[] types1 = { ProcNumType };
			//string tablename = "ProcedureLog";
			CheckODTableSetup();
		}

	}
	class Patient : ODTable
	{

		public readonly string PatNum = "PatNum";
		public readonly Type PatNumType = typeof(uint);
		public readonly string Guarantor= "Guarantor";
		public readonly Type GuarantorType = typeof(uint);
		public readonly string  LName= "LName";
		public readonly Type LNameType = typeof(string);
		public readonly string FName = "FName";
		public readonly Type FNameType = typeof(string);


		//public readonly string  = "";
		//public readonly Type Type = typeof(string);

		public Patient()
		{
			string[] cols = { PatNum, Guarantor, LName, FName };
			Type[] types1 = { PatNumType, GuarantorType, LNameType, FNameType };
			//string tablename = "Patient";
			CheckODTableSetup();
		}

	}
}
