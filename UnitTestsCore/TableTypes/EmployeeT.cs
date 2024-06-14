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

		///<summary>Creates a list of five employees with all fields assigned.</summary>
		public static List<Employee> CreateEmployeeListAllFields() {
			List<Employee> listEmployees=new List<Employee>();
			#region Employee 1
			OpenDentBusiness.Employee odbEmployee1=new OpenDentBusiness.Employee();
			odbEmployee1.LName="Picard";
			odbEmployee1.FName="Jean-Luc";
			odbEmployee1.MiddleI="";
			odbEmployee1.IsHidden=false;
			odbEmployee1.ClockStatus="Working";
			odbEmployee1.PhoneExt=1701;
			odbEmployee1.PayrollID="SP-937-215";
			odbEmployee1.WirelessPhone="07-13-2305";
			odbEmployee1.EmailWork="PicardJL@StarFleet.fed";
			odbEmployee1.EmailPersonal="EarlGreyHot@gmail.com";
			odbEmployee1.IsFurloughed=false;
			odbEmployee1.EmployeeNum=OpenDentBusiness.Employees.Insert(odbEmployee1);
			odbEmployee1=OpenDentBusiness.Employees.GetEmpFromDB(odbEmployee1.EmployeeNum);
			listEmployees.Add(odbEmployee1);
			#endregion Employee 1
			#region Employee 2
			OpenDentBusiness.Employee odbEmployee2=new OpenDentBusiness.Employee();
			odbEmployee2.LName="Riker";
			odbEmployee2.FName="William";
			odbEmployee2.MiddleI="T";
			odbEmployee2.IsHidden=false;
			odbEmployee2.ClockStatus="Working";
			odbEmployee2.PhoneExt=1701;
			odbEmployee2.PayrollID="SP-231-427";
			odbEmployee2.WirelessPhone="08-19-2335";
			odbEmployee2.EmailWork="RikerWT@StarFleet.fed";
			odbEmployee2.EmailPersonal="RedAlert@gmail.com";
			odbEmployee2.IsFurloughed=false;
			odbEmployee2.ReportsTo=odbEmployee1.EmployeeNum;
			odbEmployee2.EmployeeNum=OpenDentBusiness.Employees.Insert(odbEmployee2);
			odbEmployee2=OpenDentBusiness.Employees.GetEmpFromDB(odbEmployee2.EmployeeNum);
			listEmployees.Add(odbEmployee2);
			#endregion Employee 2
			#region Employee 3
			OpenDentBusiness.Employee odbEmployee3=new OpenDentBusiness.Employee();
			odbEmployee3.LName="";
			odbEmployee3.FName="Data";
			odbEmployee3.MiddleI="";
			odbEmployee3.IsHidden=false;
			odbEmployee3.ClockStatus="Working";
			odbEmployee3.PhoneExt=1701;
			odbEmployee3.PayrollID="M-5-10";
			odbEmployee3.WirelessPhone="02-02-2338";
			odbEmployee3.EmailWork="Data@StarFleet.fed";
			odbEmployee3.EmailPersonal="SpotCat@gmail.com";
			odbEmployee3.IsFurloughed=false;
			odbEmployee3.ReportsTo=odbEmployee1.EmployeeNum;
			odbEmployee3.EmployeeNum=OpenDentBusiness.Employees.Insert(odbEmployee3);
			odbEmployee3=OpenDentBusiness.Employees.GetEmpFromDB(odbEmployee3.EmployeeNum);
			listEmployees.Add(odbEmployee3);
			#endregion Employee 3
			#region Employee 4
			OpenDentBusiness.Employee odbEmployee4=new OpenDentBusiness.Employee();
			odbEmployee4.LName="Yar";
			odbEmployee4.FName="Natasha";
			odbEmployee4.MiddleI="T";
			odbEmployee4.IsHidden=true;
			odbEmployee4.ClockStatus="Home";
			odbEmployee4.PhoneExt=0000;
			odbEmployee4.PayrollID="";
			odbEmployee4.WirelessPhone="01-01-2337";
			odbEmployee4.EmailWork="YarNT@StarFleet.fed";
			odbEmployee4.EmailPersonal="AllGoodThings@gmail.com";
			odbEmployee4.IsFurloughed=true;
			odbEmployee4.EmployeeNum=OpenDentBusiness.Employees.Insert(odbEmployee4);
			odbEmployee4=OpenDentBusiness.Employees.GetEmpFromDB(odbEmployee4.EmployeeNum);
			listEmployees.Add(odbEmployee4);
			#endregion Employee 4
			#region Employee 5
			OpenDentBusiness.Employee odbEmployee5=new OpenDentBusiness.Employee();
			odbEmployee5.LName="Crusher";
			odbEmployee5.FName="Beverly";
			odbEmployee5.MiddleI="C";
			odbEmployee5.IsHidden=false;
			odbEmployee5.ClockStatus="Working";
			odbEmployee5.PhoneExt=1701;
			odbEmployee5.PayrollID="";
			odbEmployee5.WirelessPhone="10-13-2324";
			odbEmployee5.EmailWork="CrusherBC@StarFleet.fed";
			odbEmployee5.EmailPersonal="DancingDoctor@gmail.com";
			odbEmployee5.IsFurloughed=true;
			odbEmployee5.ReportsTo=odbEmployee1.EmployeeNum;
			odbEmployee5.EmployeeNum=OpenDentBusiness.Employees.Insert(odbEmployee5);
			odbEmployee5=OpenDentBusiness.Employees.GetEmpFromDB(odbEmployee5.EmployeeNum);
			listEmployees.Add(odbEmployee5);
			#endregion Employee 5
			Employees.RefreshCache();
			return listEmployees;
		}
	}
}
