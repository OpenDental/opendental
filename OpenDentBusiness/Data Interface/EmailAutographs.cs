using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class EmailAutographs{
		#region CachePattern

		private class EmailAutographCache : CacheListAbs<EmailAutograph> {
			protected override List<EmailAutograph> GetCacheFromDb() {
				string command="SELECT * FROM emailautograph ORDER BY "+DbHelper.ClobOrderBy("Description");
				return Crud.EmailAutographCrud.SelectMany(command);
			}
			protected override List<EmailAutograph> TableToList(DataTable table) {
				return Crud.EmailAutographCrud.TableToList(table);
			}
			protected override EmailAutograph Copy(EmailAutograph emailAutograph) {
				return emailAutograph.Copy();
			}
			protected override DataTable ListToTable(List<EmailAutograph> listEmailAutographs) {
				return Crud.EmailAutographCrud.ListToTable(listEmailAutographs,"EmailAutograph");
			}
			protected override void FillCacheIfNeeded() {
				EmailAutographs.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static EmailAutographCache _emailAutographCache=new EmailAutographCache();

		public static List<EmailAutograph> GetDeepCopy(bool isShort=false) {
			return _emailAutographCache.GetDeepCopy(isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_emailAutographCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_emailAutographCache.FillCacheFromTable(table);
				return table;
			}
			return _emailAutographCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
	
		/////<summary>Gets one EmailAutograph from the db.</summary>
		//public static EmailAutograph GetOne(long emailAutographNum){
		//	if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
		//		return Meth.GetObject<EmailAutograph>(MethodBase.GetCurrentMethod(),emailAutographNum);
		//	}
		//	return Crud.EmailAutographCrud.SelectOne(emailAutographNum);
		//}
		
		///<summary>Insert one EmailAutograph in the database.</summary>
		public static long Insert(EmailAutograph emailAutograph){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				emailAutograph.EmailAutographNum=Meth.GetLong(MethodBase.GetCurrentMethod(),emailAutograph);
				return emailAutograph.EmailAutographNum;
			}
			return Crud.EmailAutographCrud.Insert(emailAutograph);
		}
		
		///<summary>Updates an existing EmailAutograph in the database.</summary>
		public static void Update(EmailAutograph emailAutograph){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),emailAutograph);
				return;
			}
			Crud.EmailAutographCrud.Update(emailAutograph);
		}

		///<summary>Delete on EmailAutograph from the database.</summary>
		public static void Delete(long emailAutographNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),emailAutographNum);
				return;
			}
			Crud.EmailAutographCrud.Delete(emailAutographNum);
		}
	}
}