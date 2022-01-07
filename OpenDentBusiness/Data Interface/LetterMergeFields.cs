using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace OpenDentBusiness{
	///<summary></summary>
	public class LetterMergeFields {
		#region CachePattern

		private class LetterMergeFieldCache : CacheListAbs<LetterMergeField> {
			protected override List<LetterMergeField> GetCacheFromDb() {
				string command="SELECT * FROM lettermergefield ORDER BY FieldName";
				return Crud.LetterMergeFieldCrud.SelectMany(command);
			}
			protected override List<LetterMergeField> TableToList(DataTable table) {
				return Crud.LetterMergeFieldCrud.TableToList(table);
			}
			protected override LetterMergeField Copy(LetterMergeField letterMergeField) {
				return letterMergeField.Copy();
			}
			protected override DataTable ListToTable(List<LetterMergeField> listLetterMergeFields) {
				return Crud.LetterMergeFieldCrud.ListToTable(listLetterMergeFields,"LetterMergeField");
			}
			protected override void FillCacheIfNeeded() {
				LetterMergeFields.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static LetterMergeFieldCache _letterMergeFieldCache=new LetterMergeFieldCache();

		public static List<LetterMergeField> GetWhere(Predicate<LetterMergeField> match,bool isShort=false) {
			return _letterMergeFieldCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_letterMergeFieldCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_letterMergeFieldCache.FillCacheFromTable(table);
				return table;
			}
			return _letterMergeFieldCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		///<summary>Inserts this lettermergefield into database.</summary>
		public static long Insert(LetterMergeField lmf) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				lmf.FieldNum=Meth.GetLong(MethodBase.GetCurrentMethod(),lmf);
				return lmf.FieldNum;
			}
			return Crud.LetterMergeFieldCrud.Insert(lmf);
		}

		/*
		///<summary></summary>
		public void Update(){
			string command="UPDATE lettermergefield SET "
				+"LetterMergeNum = '"+POut.PInt   (LetterMergeNum)+"' "
				+",FieldName = '"    +POut.PString(FieldName)+"' "
				+"WHERE FieldNum = '"+POut.PInt(FieldNum)+"'";
			DataConnection dcon=new DataConnection();
 			Db.NonQ(command);
		}*/

		/*
		///<summary></summary>
		public void Delete(){
			string command="DELETE FROM lettermergefield "
				+"WHERE FieldNum = "+POut.PInt(FieldNum);
			DataConnection dcon=new DataConnection();
			Db.NonQ(command);
		}*/

		///<summary>Called from LetterMerge.Refresh() to get all field names for a given letter.  The result is a collection of strings representing field names.</summary>
		public static List<string> GetForLetter(long letterMergeNum) {
			//No need to check RemotingRole; no call to db.
			return GetWhere(x => x.LetterMergeNum==letterMergeNum).Select(x => x.FieldName).ToList();
		}

		///<summary>Deletes all lettermergefields for the given letter.  This is then followed by adding them all back, which is simpler than just updating.</summary>
		public static void DeleteForLetter(long letterMergeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),letterMergeNum);
				return;
			}
			string command="DELETE FROM lettermergefield "
				+"WHERE LetterMergeNum = "+POut.Long(letterMergeNum);
			Db.NonQ(command);
		}
	}

	



}









