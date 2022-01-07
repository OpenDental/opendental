using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness
{
	public static class JobHelper {
		private static List<Engineer> _listEngineers;
		
		public static List<Engineer> ListEngineers {
			get {
				if(_listEngineers==null) {
					FillEngineerList();
				}
				return _listEngineers;
			}
		}

		public static List<Userod> ListEngineerUsers {
			get {
				return ListEngineers.Select(x => x.User).ToList();
			}
		}

		public static List<Employee> ListEngineerEmployees {
			get {
				return ListEngineers.Select(x => x.Employee).ToList();
			}
		}

		public static List<long> ListEngineerUserNums {
			get {
				return ListEngineers.Select(x => x.User.UserNum).ToList();
			}
		}

		public static List<long> ListEngineerEmployeeNums {
			get {
				return ListEngineers.Select(x => x.Employee.EmployeeNum).ToList();
			}
		}

		///<summary>Attempts to fill the list of engineers from the wikilist. Fills with empty if something failed</summary>
		private static void FillEngineerList() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			_listEngineers=new List<Engineer>();
			try {
				string command="SELECT Title,EmployeeNum FROM wikilist_employees WHERE Title LIKE '%Engineer%'";
				DataTable dt=Db.GetTable(command);
				foreach(DataRow dr in dt.Rows) {
					Employee emp=Employees.GetEmp(PIn.Long(dr["EmployeeNum"].ToString()));
					Userod user=Userods.GetUserByEmployeeNum(emp.EmployeeNum);
					Engineer newEngineer=new Engineer(user,emp,PIn.String(dr["Title"].ToString()));
					_listEngineers.Add(newEngineer);
				}
			}
			catch(Exception e) {
				//Do nothing
			}
		}
	}

	public class Engineer {
		public Userod User;
		public Employee Employee;
		public string Title;
		
		public Engineer(Userod user,Employee employee,string title) {
			User=user;
			Employee=employee;
			Title=title;
		}
	}
}
