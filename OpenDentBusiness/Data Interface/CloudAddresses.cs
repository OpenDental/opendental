using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	public class CloudAddresses{

		#region Methods - Get
		public static List<CloudAddress> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<CloudAddress>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM cloudaddress;";
			return Crud.CloudAddressCrud.SelectMany(command);
		}
		
		///<summary>Gets one CloudAddress from the db.</summary>
		public static CloudAddress GetByIpAddress(string ipAddress){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<CloudAddress>(MethodBase.GetCurrentMethod(),ipAddress);
			}
			string command=$"SELECT * FROM cloudaddress WHERE IpAddress='{POut.String(ipAddress)}'";
			return Crud.CloudAddressCrud.SelectOne(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		///<summary></summary>
		public static List<CloudAddress> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<CloudAddress>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM cloudaddress WHERE PatNum = "+POut.Long(patNum);
			return Crud.CloudAddressCrud.SelectMany(command);
		}
		
		///<summary>Gets one CloudAddress from the db.</summary>
		public static CloudAddress GetOne(long cloudAddressNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<CloudAddress>(MethodBase.GetCurrentMethod(),cloudAddressNum);
			}
			return Crud.CloudAddressCrud.SelectOne(cloudAddressNum);
		}
		*/
		#endregion Methods - Get

		#region Methods - Modify
		///<summary></summary>
		public static long Insert(CloudAddress cloudAddress) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				cloudAddress.CloudAddressNum=Meth.GetLong(MethodBase.GetCurrentMethod(),cloudAddress);
				return cloudAddress.CloudAddressNum;
			}
			return Crud.CloudAddressCrud.Insert(cloudAddress);
		}

		///<summary></summary>
		public static void Update(CloudAddress cloudAddress){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cloudAddress);
				return;
			}
			Crud.CloudAddressCrud.Update(cloudAddress);
		}

		public static void DeleteMany(List<long> cloudAddressNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cloudAddressNums);
				return;
			}
			Crud.CloudAddressCrud.DeleteMany(cloudAddressNums);
		}

		/*
		///<summary></summary>
		public static void Delete(long cloudAddressNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),cloudAddressNum);
				return;
			}
			Crud.CloudAddressCrud.Delete(cloudAddressNum);
		}
		 */
		#endregion Methods - Modify



	}
}