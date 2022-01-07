using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Certs{	
		//Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Methods - Get
		/////<summary></summary>
		//public static List<Cert> Refresh(long patNum){
		//	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
		//		return Meth.GetObject<List<Cert>>(MethodBase.GetCurrentMethod(),patNum);
		//	}
		//	string command="SELECT * FROM cert WHERE PatNum = "+POut.Long(patNum);
		//	return Crud.CertCrud.SelectMany(command);
		//}

		///<summary>Gets all Certs.</summary>
		public static List<Cert> GetAll(bool showHidden) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Cert>>(MethodBase.GetCurrentMethod(),showHidden);
			}
			string command="SELECT * FROM cert";
			if(!showHidden) {
				command+=" WHERE IsHidden=0";
			}
			command+=" ORDER BY ItemOrder";
			return Crud.CertCrud.SelectMany(command);
		}

		public static List<Cert> GetAllForCategory(long categoryNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Cert>>(MethodBase.GetCurrentMethod(),categoryNum);
			}
			string command="SELECT * FROM cert WHERE CertCategoryNum="+POut.Long(categoryNum);
			command+=" ORDER BY ItemOrder";
			return Crud.CertCrud.SelectMany(command);
		}
		
		///<summary>Gets one Cert from the db.</summary>
		public static Cert GetOne(long certNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<Cert>(MethodBase.GetCurrentMethod(),certNum);
			}
			return Crud.CertCrud.SelectOne(certNum);
		}
		#endregion Methods - Get

		#region Methods - Modify
		///<summary></summary>
		public static long Insert(Cert cert){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				cert.CertNum=Meth.GetLong(MethodBase.GetCurrentMethod(),cert);
				return cert.CertNum;
			}
			return Crud.CertCrud.Insert(cert);
		}
		///<summary></summary>
		public static void Update(Cert cert){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cert);
				return;
			}
			Crud.CertCrud.Update(cert);
		}
		///<summary></summary>
		public static void Delete(long certNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),certNum);
				return;
			}
			Crud.CertCrud.Delete(certNum);
		}
		#endregion Methods - Modify
		



	}
}