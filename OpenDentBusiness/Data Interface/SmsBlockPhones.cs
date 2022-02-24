using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class SmsBlockPhones{
		#region Cache Pattern

		private class SmsBlockPhoneCache : CacheListAbs<SmsBlockPhone> {
			protected override List<SmsBlockPhone> GetCacheFromDb() {
				string command="SELECT * FROM smsblockphone";
				return Crud.SmsBlockPhoneCrud.SelectMany(command);
			}
			protected override List<SmsBlockPhone> TableToList(DataTable table) {
				return Crud.SmsBlockPhoneCrud.TableToList(table);
			}
			protected override SmsBlockPhone Copy(SmsBlockPhone smsBlockPhone) {
				return smsBlockPhone.Copy();
			}
			protected override DataTable ListToTable(List<SmsBlockPhone> listSmsBlockPhones) {
				return Crud.SmsBlockPhoneCrud.ListToTable(listSmsBlockPhones,"SmsBlockPhone");
			}
			protected override void FillCacheIfNeeded() {
				SmsBlockPhones.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static SmsBlockPhoneCache _smsBlockPhoneCache=new SmsBlockPhoneCache();

		public static List<SmsBlockPhone> GetDeepCopy(bool isShort=false) {
			return _smsBlockPhoneCache.GetDeepCopy(isShort);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_smsBlockPhoneCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_smsBlockPhoneCache.FillCacheFromTable(table);
				return table;
			}
			return _smsBlockPhoneCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern

		#region Insert
		///<summary></summary>
		public static long Insert(SmsBlockPhone smsBlockPhone) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				smsBlockPhone.SmsBlockPhoneNum=Meth.GetLong(MethodBase.GetCurrentMethod(),smsBlockPhone);
				return smsBlockPhone.SmsBlockPhoneNum;
			}
			return Crud.SmsBlockPhoneCrud.Insert(smsBlockPhone);
		}
		#endregion

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Get Methods
		///<summary></summary>
		public static List<SmsBlockPhone> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<SmsBlockPhone>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM smsblockphone WHERE PatNum = "+POut.Long(patNum);
			return Crud.SmsBlockPhoneCrud.SelectMany(command);
		}
		
		///<summary>Gets one SmsBlockPhone from the db.</summary>
		public static SmsBlockPhone GetOne(long smsBlockPhoneNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<SmsBlockPhone>(MethodBase.GetCurrentMethod(),smsBlockPhoneNum);
			}
			return Crud.SmsBlockPhoneCrud.SelectOne(smsBlockPhoneNum);
		}
		#endregion
		#region Modification Methods			
			#region Update
		///<summary></summary>
		public static void Update(SmsBlockPhone smsBlockPhone){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),smsBlockPhone);
				return;
			}
			Crud.SmsBlockPhoneCrud.Update(smsBlockPhone);
		}
			#endregion
			#region Delete
		///<summary></summary>
		public static void Delete(long smsBlockPhoneNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),smsBlockPhoneNum);
				return;
			}
			Crud.SmsBlockPhoneCrud.Delete(smsBlockPhoneNum);
		}
			#endregion
		#endregion
		#region Misc Methods
		

		
		#endregion
		*/
	}
}