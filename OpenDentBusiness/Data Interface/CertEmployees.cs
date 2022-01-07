using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class CertEmployees{
		
		//Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		///<summary>Gets all CertEmployees.</summary>
		public static List<CertEmployee> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<CertEmployee>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM certemployee";
			return Crud.CertEmployeeCrud.SelectMany(command);
		}

		///<summary>Gets all CertEmployees for one employee.</summary>
		public static List<CertEmployee> GetAllForEmployee(long employeeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<CertEmployee>>(MethodBase.GetCurrentMethod(),employeeNum);
			}
			string command="SELECT * FROM certemployee WHERE EmployeeNum = "+POut.Long(employeeNum);
			return Crud.CertEmployeeCrud.SelectMany(command);
		}

		///<summary>Gets all CertEmployees for one Cert.</summary>
		public static List<CertEmployee> GetAllForCert(long certNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<CertEmployee>>(MethodBase.GetCurrentMethod(),certNum);
			}
			string command="SELECT * FROM certemployee WHERE CertNum = "+POut.Long(certNum);
			return Crud.CertEmployeeCrud.SelectMany(command);
		}

		public static CertEmployee GetOne(long certNum,long employeeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<CertEmployee>(MethodBase.GetCurrentMethod(),certNum,employeeNum);
			}
			string command="SELECT * FROM certemployee WHERE CertNum = "+POut.Long(certNum)+" AND EmployeeNum = "+POut.Long(employeeNum);
			return Crud.CertEmployeeCrud.SelectOne(command);
		}
		///<summary></summary>
		//public static List<CertEmployee> Refresh(long patNum){
		//	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
		//		return Meth.GetObject<List<CertEmployee>>(MethodBase.GetCurrentMethod(),patNum);
		//	}
		//	string command="SELECT * FROM certemployee WHERE PatNum = "+POut.Long(patNum);
		//	return Crud.CertEmployeeCrud.SelectMany(command);
		//}
		
		/////<summary>Gets one CertEmployee from the db.</summary>
		//public static CertEmployee GetOne(long certEmployeeNum){
		//	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
		//		return Meth.GetObject<CertEmployee>(MethodBase.GetCurrentMethod(),certEmployeeNum);
		//	}
		//	return Crud.CertEmployeeCrud.SelectOne(certEmployeeNum);
		//}
		#endregion Methods - Get

		#region Methods - Modify
		///<summary></summary>
		public static long Insert(CertEmployee certEmployee){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				certEmployee.CertEmployeeNum=Meth.GetLong(MethodBase.GetCurrentMethod(),certEmployee);
				return certEmployee.CertEmployeeNum;
			}
			return Crud.CertEmployeeCrud.Insert(certEmployee);
		}

		///<summary></summary>
		public static void Update(CertEmployee certEmployee){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),certEmployee);
				return;
			}
			Crud.CertEmployeeCrud.Update(certEmployee);
		}

		///<summary></summary>
		public static void Delete(long certEmployeeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),certEmployeeNum);
				return;
			}
			Crud.CertEmployeeCrud.Delete(certEmployeeNum);
		}
		#endregion Methods - Modify
		



	}
}