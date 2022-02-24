using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class HL7DefSegments{
		#region CachePattern

		private class HL7DefSegmentCache : CacheListAbs<HL7DefSegment> {
			protected override List<HL7DefSegment> GetCacheFromDb() {
				string command="SELECT * FROM hl7defsegment ORDER BY ItemOrder";
				return Crud.HL7DefSegmentCrud.SelectMany(command);
			}
			protected override List<HL7DefSegment> TableToList(DataTable table) {
				return Crud.HL7DefSegmentCrud.TableToList(table);
			}
			protected override HL7DefSegment Copy(HL7DefSegment HL7DefSegment) {
				return HL7DefSegment.Clone();
			}
			protected override DataTable ListToTable(List<HL7DefSegment> listHL7DefSegments) {
				return Crud.HL7DefSegmentCrud.ListToTable(listHL7DefSegments,"HL7DefSegment");
			}
			protected override void FillCacheIfNeeded() {
				HL7DefSegments.GetTableFromCache(false);
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static HL7DefSegmentCache _HL7DefSegmentCache=new HL7DefSegmentCache();

		public static List<HL7DefSegment> GetDeepCopy(bool isShort=false) {
			return _HL7DefSegmentCache.GetDeepCopy(isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_HL7DefSegmentCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_HL7DefSegmentCache.FillCacheFromTable(table);
				return table;
			}
			return _HL7DefSegmentCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		/// <summary>Gets it straight from the database instead of from cache. No child objects included.</summary>
		public static List<HL7DefSegment> GetShallowFromDb(long hl7DefMessageNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<HL7DefSegment>>(MethodBase.GetCurrentMethod(),hl7DefMessageNum);
			}
			string command="SELECT * FROM hl7defsegment WHERE HL7DefMessageNum='"+POut.Long(hl7DefMessageNum)+"' ORDER BY ItemOrder";
			return Crud.HL7DefSegmentCrud.SelectMany(command);
		}

		///<summary>Gets deep list from cache.</summary>
		public static List<HL7DefSegment> GetDeepFromCache(long hl7DefMessageNum) {
			List<HL7DefSegment> list=new List<HL7DefSegment>();
			List<HL7DefSegment> listHL7DefSegment=GetDeepCopy(false);
			for(int i=0;i<listHL7DefSegment.Count;i++) {
				if(listHL7DefSegment[i].HL7DefMessageNum==hl7DefMessageNum) {
					list.Add(listHL7DefSegment[i]);
					list[list.Count-1].hl7DefFields=HL7DefFields.GetFromCache(listHL7DefSegment[i].HL7DefSegmentNum);
				}
			}
			return list;
		}

		///<summary>Gets a full deep list of all Segments for this message from the database.</summary>
		public static List<HL7DefSegment> GetDeepFromDb(long hl7DefMessageNum) {
			List<HL7DefSegment> hl7defsegs=new List<HL7DefSegment>();
			hl7defsegs=GetShallowFromDb(hl7DefMessageNum);
			foreach(HL7DefSegment s in hl7defsegs) {
				s.hl7DefFields=HL7DefFields.GetFromDb(s.HL7DefSegmentNum);
			}
			return hl7defsegs;
		}

		///<summary></summary>
		public static long Insert(HL7DefSegment hL7DefSegment) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				hL7DefSegment.HL7DefSegmentNum=Meth.GetLong(MethodBase.GetCurrentMethod(),hL7DefSegment);
				return hL7DefSegment.HL7DefSegmentNum;
			}
			return Crud.HL7DefSegmentCrud.Insert(hL7DefSegment);
		}

		///<summary></summary>
		public static void Update(HL7DefSegment hL7DefSegment) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),hL7DefSegment);
				return;
			}
			Crud.HL7DefSegmentCrud.Update(hL7DefSegment);
		}

		///<summary></summary>
		public static void Delete(long hL7DefSegmentNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),hL7DefSegmentNum);
				return;
			}
			string command= "DELETE FROM hl7defsegment WHERE HL7DefSegmentNum = "+POut.Long(hL7DefSegmentNum);
			Db.NonQ(command);
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<HL7DefSegment> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<HL7DefSegment>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM hl7defsegment WHERE PatNum = "+POut.Long(patNum);
			return Crud.HL7DefSegmentCrud.SelectMany(command);
		}

		///<summary>Gets one HL7DefSegment from the db.</summary>
		public static HL7DefSegment GetOne(long hL7DefSegmentNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<HL7DefSegment>(MethodBase.GetCurrentMethod(),hL7DefSegmentNum);
			}
			return Crud.HL7DefSegmentCrud.SelectOne(hL7DefSegmentNum);
		}

		*/
	}
}
