using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class EmployeeT {

		public static Employee CreateEmployee(string prefix) {
			return CreateEmployee(fName:prefix+"Bob",lName:prefix+"Boberson",middleI:prefix+"Bobbity",clockStatus:"Home");
		}

		public static Employee CreateEmployee(string fName,string lName,string middleI="",string clockStatus="",string emailWork="",bool doInsert=true) {
			Employee employee=new Employee() {
				ClockStatus=clockStatus,
				EmailWork=emailWork,
				FName=fName,
				LName=lName,
				MiddleI=middleI
			};
			if(doInsert) {
				Employees.Insert(employee);
				Employees.RefreshCache();
			}
			return employee;
		}

		///<summary>Deletes everything from the employee table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearEmployeeTable() {
			string command="DELETE FROM employee WHERE EmployeeNum > 0";
			DataCore.NonQ(command);
			Employees.RefreshCache();
		}

		/// <summary>Creates a list of three employees.</summary>
		public static List<Employee> CreateEmployeeList() {
			List<Employee> listEmployees=new List<Employee>();
			Employee employeeOne=new Employee();
			employeeOne.FName="Frodo";
			employeeOne.LName="Baggins";
			Employee employeeTwo=new Employee();
			employeeTwo.FName="Gandalf";
			employeeTwo.LName="The Grey";
			Employee employeeThree=new Employee();
			employeeThree.FName="Gimli";
			employeeThree.LName="??";
			listEmployees.Add(employeeOne);
			listEmployees.Add(employeeTwo);
			listEmployees.Add(employeeThree);
			return listEmployees;
		}

		/// <summary>Creates a list of three employees with some empty fields.</summary>
		public static List<Employee> CreateEmployeeListEmpties() {
			List<Employee> listEmployees=new List<Employee>();
			Employee employeeOne=new Employee();
			employeeOne.FName="Frodo";
			Employee employeeTwo=new Employee();
			employeeTwo.FName="Gandalf";
			Employee employeeThree=new Employee();
			employeeThree.FName="Gimli";
			listEmployees.Add(employeeOne);
			listEmployees.Add(employeeTwo);
			listEmployees.Add(employeeThree);
			return listEmployees;
		}

	}
}
