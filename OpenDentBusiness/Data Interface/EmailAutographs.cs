using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

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

		public static EmailAutograph GetFirstOrDefault(Func<EmailAutograph,bool> match,bool isShort=false) {
			return _emailAutographCache.GetFirstOrDefault(match,isShort);
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_emailAutographCache.FillCacheFromTable(table);
				return table;
			}
			return _emailAutographCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_emailAutographCache.ClearCache();
		}
		#endregion

		///<summary>Searches the cache of EmailAutographs and returns the first match, otherwise null.</summary>
		public static EmailAutograph GetForOutgoing(List<EmailAutograph> listAutographs,EmailAddress emailAddressOutgoing) {
			string emailUsername=EmailMessages.GetAddressSimple(emailAddressOutgoing.EmailUsername);
			string emailSender=EmailMessages.GetAddressSimple(emailAddressOutgoing.SenderAddress);
			string autographEmail;
			for(int i=0;i<listAutographs.Count;i++) {
				autographEmail=EmailMessages.GetAddressSimple(listAutographs[i].EmailAddress.Trim());
				//Use Contains() because an autograph can theoretically have multiple email addresses associated with it.
				if((!string.IsNullOrWhiteSpace(emailUsername) && autographEmail.Contains(emailUsername)) 
					|| (!string.IsNullOrWhiteSpace(emailSender) && autographEmail.Contains(emailSender)))
				{
					return listAutographs[i];
				}
			}
			return null;
		}

		///<summary>Gets the first autograph that matches the EmailAddress.SenderAddress if it has one, otherwise
		///the first that matches EmailAddress.EmailUserName. Returns null if neither yield a match.</summary>
		public static EmailAutograph GetFirstOrDefaultForEmailAddress(EmailAddress emailAddress) {
			string addressToMatch=emailAddress.GetFrom();
			return GetFirstOrDefault(x => emailAddress.GetFrom()==x.EmailAddress);
		}
	
		/////<summary>Gets one EmailAutograph from the db.</summary>
		//public static EmailAutograph GetOne(long emailAutographNum){
		//	if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
		//		return Meth.GetObject<EmailAutograph>(MethodBase.GetCurrentMethod(),emailAutographNum);
		//	}
		//	return Crud.EmailAutographCrud.SelectOne(emailAutographNum);
		//}

		///<summary>Insert one EmailAutograph in the database.</summary>
		public static long Insert(EmailAutograph emailAutograph){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				emailAutograph.EmailAutographNum=Meth.GetLong(MethodBase.GetCurrentMethod(),emailAutograph);
				return emailAutograph.EmailAutographNum;
			}
			return Crud.EmailAutographCrud.Insert(emailAutograph);
		}
		
		///<summary>Updates an existing EmailAutograph in the database.</summary>
		public static void Update(EmailAutograph emailAutograph){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),emailAutograph);
				return;
			}
			Crud.EmailAutographCrud.Update(emailAutograph);
		}

		///<summary>Delete on EmailAutograph from the database.</summary>
		public static void Delete(long emailAutographNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),emailAutographNum);
				return;
			}
			Crud.EmailAutographCrud.Delete(emailAutographNum);
		}
	}
}