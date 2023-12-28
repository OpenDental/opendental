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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod());
				return;
			}
			_listEngineers=new List<Engineer>();
			string command="SELECT Title,EmployeeNum FROM wikilist_employees WHERE Title LIKE '%Engineer%'";
			DataTable table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				DataRow row=table.Rows[i];
				Employee employee=Employees.GetEmp(PIn.Long(row["EmployeeNum"].ToString()));
				if(employee is null) { //Employee not found/created yet
					continue;
				}
				Userod user=Userods.GetUserByEmployeeNum(employee.EmployeeNum);
				if(user is null) { //UserOD not found/created yet
					continue;
				}
				Engineer engineer=new Engineer(user,employee,PIn.String(row["Title"].ToString()));
				_listEngineers.Add(engineer);
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
