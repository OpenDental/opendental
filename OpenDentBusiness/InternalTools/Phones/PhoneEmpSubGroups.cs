using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class PhoneEmpSubGroups{
		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		/*
		#region CachePattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add RefreshCache and FillCache 
		//to the Cache.cs file with all the other Cache types.
		//Also, make sure to consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		///<summary>A list of all PhoneEmpSubGroups.</summary>
		private static List<PhoneEmpSubGroup> _list;

		///<summary>A list of all PhoneEmpSubGroups.</summary>
		public static List<PhoneEmpSubGroup> List {
			get {
				if(_list==null) {
					RefreshCache();
				}
				return _list;
			}
			set {
				_list=value;
			}
		}

		///<summary></summary>
		public static DataTable RefreshCache(){
			//No need to check RemotingRole; Calls GetTableRemotelyIfNeeded().
			string command="SELECT * FROM phoneempsubgroup ORDER BY ItemOrder";//stub query probably needs to be changed
			DataTable table=Cache.GetTableRemotelyIfNeeded(MethodBase.GetCurrentMethod(),command);
			table.TableName="PhoneEmpSubGroup";
			FillCache(table);
			return table;
		}

		///<summary></summary>
		public static void FillCache(DataTable table){
			//No need to check RemotingRole; no call to db.
			_list=Crud.PhoneEmpSubGroupCrud.TableToList(table);
		}
		#endregion
		*/

		///<summary></summary>
		public static List<PhoneEmpSubGroup> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PhoneEmpSubGroup>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM phoneempsubgroup WHERE PatNum = "+POut.Long(patNum);
			return Crud.PhoneEmpSubGroupCrud.SelectMany(command);
		}

		///<summary>Gets one PhoneEmpSubGroup from the db.</summary>
		public static PhoneEmpSubGroup GetOne(long phoneEmpSubGroupNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<PhoneEmpSubGroup>(MethodBase.GetCurrentMethod(),phoneEmpSubGroupNum);
			}
			return Crud.PhoneEmpSubGroupCrud.SelectOne(phoneEmpSubGroupNum);
		}

		///<summary></summary>
		public static List<PhoneEmpSubGroup> GetAll(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<PhoneEmpSubGroup>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM phoneempsubgroup";
			return Crud.PhoneEmpSubGroupCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(PhoneEmpSubGroup phoneEmpSubGroup){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				phoneEmpSubGroup.PhoneEmpSubGroupNum=Meth.GetLong(MethodBase.GetCurrentMethod(),phoneEmpSubGroup);
				return phoneEmpSubGroup.PhoneEmpSubGroupNum;
			}
			return Crud.PhoneEmpSubGroupCrud.Insert(phoneEmpSubGroup);
		}
		
		public static void Update(List<PhoneEmpSubGroup> list) {
			list.ForEach(Update);
		}

		///<summary></summary>
		public static void Update(PhoneEmpSubGroup phoneEmpSubGroup){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),phoneEmpSubGroup);
				return;
			}
			Crud.PhoneEmpSubGroupCrud.Update(phoneEmpSubGroup);
		}

		///<summary></summary>
		public static void Delete(long phoneEmpSubGroupNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),phoneEmpSubGroupNum);
				return;
			}
			Crud.PhoneEmpSubGroupCrud.Delete(phoneEmpSubGroupNum);
		}

		///<summary>Inserts, updates, or deletes db rows to match listNew.  No need to pass in userNum, it's set before remoting role check and passed to
		///the server if necessary.  Doesn't create ApptComm items, but will delete them.  If you use Sync, you must create new Apptcomm items.</summary>
		public static void Sync(List<PhoneEmpSubGroup> listNew,List<PhoneEmpSubGroup> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listNew,listOld);
				return;
			}
			Crud.PhoneEmpSubGroupCrud.Sync(listNew,listOld);
		}

	}
}