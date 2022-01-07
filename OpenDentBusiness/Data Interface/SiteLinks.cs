using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	public class SiteLinks {
		#region Cache Pattern

		private class SiteLinkCache : CacheListAbs<SiteLink> {
			protected override List<SiteLink> GetCacheFromDb() {
				string command="SELECT * FROM sitelink";
				return Crud.SiteLinkCrud.SelectMany(command);
			}
			protected override List<SiteLink> TableToList(DataTable table) {
				return Crud.SiteLinkCrud.TableToList(table);
			}
			protected override SiteLink Copy(SiteLink siteLink) {
				return siteLink.Copy();
			}
			protected override DataTable ListToTable(List<SiteLink> listSiteLinks) {
				return Crud.SiteLinkCrud.ListToTable(listSiteLinks,"SiteLink");
			}
			protected override void FillCacheIfNeeded() {
				SiteLinks.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static SiteLinkCache _siteLinkCache=new SiteLinkCache();

		public static SiteLink GetFirstOrDefault(Func<SiteLink,bool> match,bool isShort=false) {
			return _siteLinkCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_siteLinkCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_siteLinkCache.FillCacheFromTable(table);
				return table;
			}
			return _siteLinkCache.GetTableFromCache(doRefreshCache);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}
		#endregion Cache Pattern

		#region Get Methods
		///<summary>Gets one SiteLink from the db.</summary>
		public static SiteLink GetOne(long siteLinkNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<SiteLink>(MethodBase.GetCurrentMethod(),siteLinkNum);
			}
			return Crud.SiteLinkCrud.SelectOne(siteLinkNum);
		}

		///<summary>Gets the first Site based on the first three octets in the default gateway of the local computer.
		///Returns the first site in the list if there is no match.  Can return null if there are no sites in the current cache or invalid site link.
		///The first three octets are used to dictate which location the calling computer is located.  Method designed for HQ only.</summary>
		public static Site GetSiteByGateway() {
			//No need to check RemotingRole; no call to db.
			//Defatult to first site in the list just in case we add a new location without first updating this method / paradigm.
			Site site=Sites.GetFirst();
			SiteLink siteLink=GetSiteLinkByGateway();
			if(siteLink!=null) {
				site=Sites.GetFirst(x => x.SiteNum==siteLink.SiteNum);
			}
			return site;
		}

		///<summary>Returns the first SiteLink that has a default gateway that matches the OctectStart value.  Returns null if no match found.</summary>
		public static SiteLink GetSiteLinkByGateway() {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => ODEnvironment.GetDefaultGateway().StartsWith(x.OctetStart));
		}

		///<summary>Gets the color of the first Site match based on the first three octets in the default gateway of the local computer.
		///Defaults to black if no valid site link can be found.
		///The first three octets are used to dictate which location the calling computer is located.  Method designed for HQ only.</summary>
		public static Color GetSiteColorByGateway() {
			//No need to check RemotingRole; no call to db.
			Color color=Color.Black;
			SiteLink siteLink=GetSiteLinkByGateway();
			if(siteLink!=null) {
				color=siteLink.SiteColor;
			}
			return color;
		}

		///<summary>Gets the fore color of the first site that matches the siteNum passed in.  Method designed for HQ only.</summary>
		public static Color GetSiteColorBySiteNum(long siteNum,Color colorDefault) {
			//No need to check RemotingRole; no call to db.
			Color color=colorDefault;
			SiteLink siteLink=GetFirstOrDefault(x => x.SiteNum==siteNum);
			if(siteLink!=null) {
				color=siteLink.SiteColor;
			}
			return color;
		}

		///<summary>Gets the fore color of the first site that matches the siteNum passed in.  Method designed for HQ only.</summary>
		public static Color GetSiteForeColorBySiteNum(long siteNum,Color colorDefault) {
			//No need to check RemotingRole; no call to db.
			Color color=colorDefault;
			SiteLink siteLink=GetFirstOrDefault(x => x.SiteNum==siteNum);
			if(siteLink!=null) {
				color=siteLink.ForeColor;
			}
			return color;
		}

		///<summary>Gets the inner color of the first site that matches the siteNum passed in.  Method designed for HQ only.</summary>
		public static Color GetSiteInnerColorBySiteNum(long siteNum,Color colorDefault) {
			//No need to check RemotingRole; no call to db.
			Color color=colorDefault;
			SiteLink siteLink=GetFirstOrDefault(x => x.SiteNum==siteNum);
			if(siteLink!=null) {
				color=siteLink.InnerColor;
			}
			return color;
		}

		///<summary>Gets the outer color of the first site that matches the siteNum passed in.  Method designed for HQ only.</summary>
		public static Color GetSiteOuterColorBySiteNum(long siteNum,Color colorDefault) {
			//No need to check RemotingRole; no call to db.
			Color color=colorDefault;
			SiteLink siteLink=GetFirstOrDefault(x => x.SiteNum==siteNum);
			if(siteLink!=null) {
				color=siteLink.OuterColor;
			}
			return color;
		}
		#endregion

		#region Insert
		///<summary></summary>
		public static long Insert(SiteLink siteLink) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				siteLink.SiteLinkNum=Meth.GetLong(MethodBase.GetCurrentMethod(),siteLink);
				return siteLink.SiteLinkNum;
			}
			return Crud.SiteLinkCrud.Insert(siteLink);
		}
		#endregion

		#region Update
		///<summary></summary>
		public static void Update(SiteLink siteLink) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),siteLink);
				return;
			}
			Crud.SiteLinkCrud.Update(siteLink);
		}

		///<summary></summary>
		public static void Upsert(SiteLink siteLink) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),siteLink);
				return;
			}
			if(siteLink.SiteLinkNum > 0) {
				Update(siteLink);
			}
			else {
				Insert(siteLink);
			}
		}

		///<summary>Updates all sitelinks to have the same triage coordinator, due to working from home.</summary>
		public static void UpdateAllTriageCoordinators(SiteLink siteLink) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),siteLink);
				return;
			}
			//The sitelink that is passed in has already been updated
			string command="UPDATE sitelink SET EmployeeNum = "+POut.Long(siteLink.EmployeeNum);
			Db.NonQ(command);
		}
		#endregion

		#region Delete
		///<summary>Deletes all site links with the passed in SiteNum.</summary>
		public static void DeleteForSite(long siteNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),siteNum);
				return;
			}
			string command="DELETE FROM sitelink WHERE SiteNum="+POut.Long(siteNum);
			Db.NonQ(command);
		}

		///<summary>Updates the EmployeeNum column for all of site links if the EmployeeNum changed for the site link passed in.  Returns true if something changed, otherwise false.</summary>
		public static bool UpdateTriageCoordinator(long siteLinkNum,long employeeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),siteLinkNum,employeeNum);
			}
			SiteLink siteLink=GetFirstOrDefault(x => x.SiteLinkNum==siteLinkNum);
			if(siteLink!=null && siteLink.EmployeeNum==employeeNum) {
				return false;
			}
			siteLink.EmployeeNum=employeeNum;
			UpdateAllTriageCoordinators(siteLink);
			//The following query needs to come back once everyone is back in the office (no longer working from home).
			//string command="UPDATE sitelink SET EmployeeNum="+POut.Long(employeeNum)+" "
			//	+"WHERE SiteLinkNum="+POut.Long(siteLinkNum);
			//Db.NonQ(command);
			return true;
		}
		#endregion
	}
}