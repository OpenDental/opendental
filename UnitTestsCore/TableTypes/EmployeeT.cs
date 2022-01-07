using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class EmployeeT {
		public static Employee CreateEmployee(string prefix){
			Employee emp=new Employee();
			emp.ClockStatus="Home";
			emp.FName=prefix+"Bob";
			emp.LName=prefix+"Boberson";
			emp.MiddleI=prefix+"Bobbity";
			Employees.Insert(emp);
			return emp;
		}


	}
}
