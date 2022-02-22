using CodeBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenDentBusiness{
	///<summary></summary>
	public class AppointmentTypes {
		#region Get Methods

		///<summary>Returns an empty string for invalid AppointmentTypeNum.  Appends (hidden) to the end of the name if necessary.</summary>
		public static string GetName(long AppointmentTypeNum) {
			//No need to check RemotingRole; no call to db.
			string retVal="";
			AppointmentType appointmentType=GetFirstOrDefault(x => x.AppointmentTypeNum==AppointmentTypeNum);
			if(appointmentType!=null) {
				retVal=appointmentType.AppointmentTypeName+(appointmentType.IsHidden ? " "+Lans.g("AppointmentTypes","(hidden)") : "");
			}
			return retVal;
		}

		///<summary>Returns the time pattern for the specified appointment type (time pattern returned will always be in 5 min increments).
		///If the Pattern variable is not set on the appointment type object then the pattern will be dynamically calculated.
		///Optionally pass in provider information in order to use specific provider time patterns.</summary>
		public static string GetTimePatternForAppointmentType(AppointmentType appointmentType,long provNumDentist=0,long provNumHyg=0) {
			//No need to check RemotingRole; no call to db.
			string timePattern="";
			if(string.IsNullOrEmpty(appointmentType.Pattern)) {
				//Dynamically calculate the timePattern from the procedure codes associated to the appointment type passed in.
				List<string> listProcCodeStrings=appointmentType.CodeStr.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).ToList();
				List<ProcedureCode> listProcCodes=new List<ProcedureCode>();
				listProcCodeStrings.ForEach(x => listProcCodes.Add(ProcedureCodes.GetProcCode(x)));
				timePattern=OpenDentBusiness.Appointments.CalculatePattern(provNumDentist,provNumHyg,listProcCodes.Select(x => x.CodeNum).ToList(),true);
			}
			else {
				timePattern=appointmentType.Pattern;//Already in 5 minute increment so no conversion required.
			}
			return timePattern;
		}

		///<summary>Returns the appointment type associated to the definition passed in.  Returns null if no match found.</summary>
		public static AppointmentType GetApptTypeForDef(long defNum) {
			//No need to check RemotingRole; no call to db.
			List<DefLink> listDefLinks=DefLinks.GetDefLinksByType(DefLinkType.AppointmentType);
			DefLink defLink=listDefLinks.FirstOrDefault(x => x.DefNum==defNum);
			if(defLink==null) {
				return null;
			}
			return AppointmentTypes.GetFirstOrDefault(x => x.AppointmentTypeNum==defLink.FKey,true);
		}

		#endregion

		#region CachePattern

		private class AppointmentTypeCache : CacheListAbs<AppointmentType> {
			protected override List<AppointmentType> GetCacheFromDb() {
				string command="SELECT * FROM appointmenttype ORDER BY ItemOrder";
				return Crud.AppointmentTypeCrud.SelectMany(command);
			}
			protected override List<AppointmentType> TableToList(DataTable table) {
				return Crud.AppointmentTypeCrud.TableToList(table);
			}
			protected override AppointmentType Copy(AppointmentType appointmentType) {
				return appointmentType.Copy();
			}
			protected override DataTable ListToTable(List<AppointmentType> listAppointmentTypes) {
				return Crud.AppointmentTypeCrud.ListToTable(listAppointmentTypes,"AppointmentType");
			}
			protected override void FillCacheIfNeeded() {
				AppointmentTypes.GetTableFromCache(false);
			}
			protected override bool IsInListShort(AppointmentType appointmentType) {
				return !appointmentType.IsHidden;
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static AppointmentTypeCache _appointmentTypeCache=new AppointmentTypeCache();

		public static List<AppointmentType> GetDeepCopy(bool isShort=false) {
			return _appointmentTypeCache.GetDeepCopy(isShort);
		}

		public static AppointmentType GetFirstOrDefault(Func<AppointmentType,bool> match,bool isShort=false) {
			return _appointmentTypeCache.GetFirstOrDefault(match,isShort);
		}

		public static List<AppointmentType> GetWhere(Predicate<AppointmentType> match,bool isShort=false) {
			return _appointmentTypeCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_appointmentTypeCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_appointmentTypeCache.FillCacheFromTable(table);
				return table;
			}
			return _appointmentTypeCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		#region Sync Pattern

		///<summary>Inserts, updates, or deletes database rows to match supplied list.</summary>
		public static void Sync(List<AppointmentType> listNew,List<AppointmentType> listOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listNew,listOld);//never pass DB list through the web service
				return;
			}
			Crud.AppointmentTypeCrud.Sync(listNew,listOld);
		}

		#endregion

		///<summary>Gets one AppointmentType from the cache.  Returns null if no match found.</summary>
		public static AppointmentType GetOne(long appointmentTypeNum) {
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.AppointmentTypeNum==appointmentTypeNum);
		}

		///<summary></summary>
		public static long Insert(AppointmentType appointmentType){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				appointmentType.AppointmentTypeNum=Meth.GetLong(MethodBase.GetCurrentMethod(),appointmentType);
				return appointmentType.AppointmentTypeNum;
			}
			return Crud.AppointmentTypeCrud.Insert(appointmentType);
		}

		///<summary></summary>
		public static void Update(AppointmentType appointmentType){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),appointmentType);
				return;
			}
			Crud.AppointmentTypeCrud.Update(appointmentType);
		}

		///<summary>Surround with try catch.</summary>
		public static void Delete(long appointmentTypeNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),appointmentTypeNum);
				return;
			}
			string s=AppointmentTypes.CheckInUse(appointmentTypeNum);
			if(s!="") {
				throw new ApplicationException(Lans.g("AppointmentTypes",s));
			}
			string command="DELETE FROM appointmenttype WHERE AppointmentTypeNum = "+POut.Long(appointmentTypeNum);
			Db.NonQ(command);
		}

		///<summary>Used when attempting to delete.  Returns empty string if not in use and an untranslated string if in use.</summary>
		public static string CheckInUse(long appointmentTypeNum) {
			if(appointmentTypeNum==0) {//New appointment type, so should not be associated with any appointments.
				return "";
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetString(MethodBase.GetCurrentMethod(),appointmentTypeNum);
			}
			string command="SELECT COUNT(*) FROM appointment WHERE AppointmentTypeNum = "+POut.Long(appointmentTypeNum);
			if(PIn.Int(Db.GetCount(command))>0) {
				return "Not allowed to delete appointment types that are in use on an appointment.";
			}
			command="SELECT COUNT(*) FROM deflink "
				+"WHERE LinkType = "+POut.Int((int)DefLinkType.AppointmentType)+" "
				+"AND FKey = "+POut.Long(appointmentTypeNum)+" ";
			if(PIn.Int(Db.GetCount(command))>0) {
				//This message will need to change in the future if more definition categories utilize appointment types with the deflink table.
				return "Not allowed to delete appointment types that are in use by Web Sched New Pat Appt Types definitions.";
			}
			return "";
		}

		public static int SortItemOrder(AppointmentType a1,AppointmentType a2) {
			if(a1.ItemOrder!=a2.ItemOrder){
				return a1.ItemOrder.CompareTo(a2.ItemOrder);
			}
			return a1.AppointmentTypeNum.CompareTo(a2.AppointmentTypeNum);
		}

		///<summary>Returns true if all members are the same.</summary>
		public static bool Compare(AppointmentType a1,AppointmentType a2) {
			if(a1.AppointmentTypeColor==a2.AppointmentTypeColor
				&& a1.AppointmentTypeName==a2.AppointmentTypeName
				&& a1.IsHidden==a2.IsHidden
				&& a1.ItemOrder==a2.ItemOrder)
			{
				return true;
			}
			return false;
		}

		//If this table type will exist as cached data, uncomment the CachePattern region below and edit.
		/**/
		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		*/
	}
}