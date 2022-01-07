using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using OpenDentBusiness;

namespace OpenDental.Reporting.Allocators
{
	/// <summary>
	/// The class that is to be inherited by anyone creating an allocator.
	/// </summary>
	abstract class Allocator : IAllocate
	{
		
		protected string Description = "Explanation of what your allocator does";
		protected string Name = "Name of your Allocator";
		/// <summary>
		/// The text that provides the help documentation for the user to see.
		/// Pass it thru the Lan.F before you want to display it.
		/// </summary>
		protected string HelpDoc = "No Helpdoc Available";
		/// <summary>
		/// This is the name given to the table that will be used to hold the allocated
		/// data.  Set by programer in inherited member.
		/// </summary>
		protected string DbaseStorageTable = "";
		/// <summary>
		/// The column names used for the DbaseStorageTable
		/// </summary>
		protected string[] DbaseTableColumns = null;

		public Allocator(string TableName, string[] Columns)
		{
			SetDbaseTable_and_Columns(TableName, Columns);
		}
		public Allocator()
		{

		}

		#region IAllocate Members

		public abstract bool Allocate(int iGuarantor);
		//{
		//    throw new Exception("The method or operation is not implemented.");
		//}

		public abstract bool DeAllocate(int iGuarantor);
		//{
		//    throw new Exception("The method or operation is not implemented.");
		//}
		/// <summary>
		/// Designed to be called by the constructor to set the tableName and 
		/// column definitions of the allocator.  I really wanted this member
		/// to be protected not public.
		/// </summary>
		private void SetDbaseTable_and_Columns(string tableName, string[] Columns)
		{
			this.DbaseStorageTable = tableName;
			this.DbaseTableColumns = Columns;
		}
		
		#endregion
		

		/// <summary>
		/// Tries to select 1 row of elements with each of the columns stated in 
		/// DbaseTableColumns.  Code was faster than "DESCRIBE Table";
		/// Plus I'm not checking field types.
		/// </summary>
		/// <returns>false if querry throws an exception.</returns>
		bool CheckDbase()
		{
			bool rVal = true;
			string cmd = "SELECT ";
			for (int i = 0; i < DbaseTableColumns.Length; i++)
			{
				cmd += DbaseTableColumns[i];
				if (i < DbaseTableColumns.Length - 1)
					cmd += ", ";
			}
			cmd += " FROM " + DbaseStorageTable + " "+DbHelper.LimitWhere(1);
			try
			{
				Db.GetTableOld(cmd);
			}
			catch
			{
				rVal = false;
			}
			return rVal;


		}
		/// <summary>
		/// Uses "SHOW TABLES" to see if table exists.
		/// </summary>
		/// <returns></returns>
		bool TableExists()
		{
			bool rVal = false;
			string cmd = "SHOW FULL TABLES WHERE Table_type='BASE TABLE'";//Tables, not views.  Does not work in MySQL 4.1, however we test for MySQL version >= 5.0 in PrefL.
			string thistable = this.DbaseStorageTable.ToLower();
			try
			{
				DataTable dt = Db.GetTableOld(cmd);
				if (dt.Rows.Count != 0)
				{
					for (int i=0; i<dt.Rows.Count; i++)
					{
						string tblname = dt.Rows[i][0].ToString().ToLower();
						if (thistable == tblname)
						{
							i = dt.Rows.Count;
							rVal = true;
						}
					}
				}
				
			}
			catch
			{
			}
			return rVal;
		}

		
	}
}
