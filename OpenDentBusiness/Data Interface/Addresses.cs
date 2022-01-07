using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Addresses{
		#region Get Methods
		///<summary>Returns null if none.</summary>
		public static Address GetOneByPatNum(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Address>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM address WHERE PatNumTaxPhysical = "+POut.Long(patNum);
			return Crud.AddressCrud.SelectOne(command);
		}
		#endregion Get Methods
		
		#region Modification Methods
		///<summary></summary>
		public static long Insert(Address address){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				address.AddressNum=Meth.GetLong(MethodBase.GetCurrentMethod(),address);
				return address.AddressNum;
			}
			return Crud.AddressCrud.Insert(address);
		}

		///<summary></summary>
		public static void Update(Address address){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),address);
				return;
			}
			Crud.AddressCrud.Update(address);
		}

		///<summary></summary>
		public static void Delete(long addressNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),addressNum);
				return;
			}
			Crud.AddressCrud.Delete(addressNum);
		}
		#endregion Modification Methods
	}
}