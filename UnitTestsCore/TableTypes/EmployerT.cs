using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class EmployerT {

		///<summary>Creates a new employer. Refreshes cache.</summary>
		public static Employer CreateEmployer(string empName="",bool doInsert=true) {
			Employer employer=new Employer() {
				EmpName=empName
			};
			if(doInsert) {
				Employers.Insert(employer);
				Employers.RefreshCache();
			}
			return employer;
		}

		///<summary>Deletes everything from the employer table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearEmployerTable() {
			string command="DELETE FROM employer WHERE EmployerNum > 0";
			DataCore.NonQ(command);
			Employers.RefreshCache();
		}

	}
}
