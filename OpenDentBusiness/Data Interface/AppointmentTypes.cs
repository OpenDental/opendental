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
			//No need to check MiddleTierRole; no call to db.
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
			//No need to check MiddleTierRole; no call to db.
			string timePattern="";
			if(string.IsNullOrEmpty(appointmentType.Pattern)) {
				//Dynamically calculate the timePattern from the procedure codes associated to the appointment type passed in.
				List<string> listProcCodeStrings=appointmentType.CodeStr.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).ToList();
				List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>();
				listProcCodeStrings.ForEach(x => listProcedureCodes.Add(ProcedureCodes.GetProcCode(x)));
				timePattern=OpenDentBusiness.Appointments.CalculatePattern(provNumDentist,provNumHyg,listProcedureCodes.Select(x => x.CodeNum).ToList(),true);
			}
			else {
				timePattern=appointmentType.Pattern;//Already in 5 minute increment so no conversion required.
			}
			return timePattern;
		}

		///<summary>Returns the appointment type associated to the definition passed in.  Returns null if no match found.</summary>
		public static AppointmentType GetApptTypeForDef(long defNum) {
			//No need to check MiddleTierRole; no call to db.
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_appointmentTypeCache.FillCacheFromTable(table);
				return table;
			}
			return _appointmentTypeCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_appointmentTypeCache.ClearCache();
		}
		#endregion

		#region Sync Pattern

		///<summary>Inserts, updates, or deletes database rows to match supplied list.</summary>
		public static void Sync(List<AppointmentType> listAppointmentTypesNew,List<AppointmentType> listAppointmentTypesOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listAppointmentTypesNew,listAppointmentTypesOld);//never pass DB list through the web service
				return;
			}
			Crud.AppointmentTypeCrud.Sync(listAppointmentTypesNew,listAppointmentTypesOld);
		}

		#endregion

		///<summary>Gets one AppointmentType from the cache.  Returns null if no match found.</summary>
		public static AppointmentType GetOne(long appointmentTypeNum) {
			//No need to check MiddleTierRole; no call to db.
			return GetFirstOrDefault(x => x.AppointmentTypeNum==appointmentTypeNum);
		}

		///<summary>Gets all AppointmentTypes from the DB. Ordered by AppointmentTypeNum. Returns null if no match found.</summary>
		public static List<AppointmentType> GetAllAppointmentTypesForApi(int limit,int offset) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				return Meth.GetObject<List<AppointmentType>>(MethodBase.GetCurrentMethod(),limit,offset);
			}
			string command="SELECT * FROM appointmenttype ORDER BY AppointmentTypeNum "
				+"LIMIT "+POut.Int(offset)+", "+POut.Int(limit);
			return Crud.AppointmentTypeCrud.SelectMany(command);
		}

		///<summary></summary>
		public static long Insert(AppointmentType appointmentType){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				appointmentType.AppointmentTypeNum=Meth.GetLong(MethodBase.GetCurrentMethod(),appointmentType);
				return appointmentType.AppointmentTypeNum;
			}
			return Crud.AppointmentTypeCrud.Insert(appointmentType);
		}

		///<summary></summary>
		public static void Update(AppointmentType appointmentType){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),appointmentType);
				return;
			}
			Crud.AppointmentTypeCrud.Update(appointmentType);
		}

		///<summary>Surround with try catch.</summary>
		public static void Delete(long appointmentTypeNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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

		///<summary>Used when closing the FormApptEdit window. Takes appointmentTypeNum and list of selected (attached) procedures for the appointment open in the window. Returns empty string if required procs for appointment type are attached to the appointment otherwise returns message with missing required procedures and prevents closing the window.</summary>
		public static string CheckRequiredProcsAttached(long appointmentTypeNum, List<Procedure> listProcedures){
			//No need to check MiddleTierRole; no call to db.
			string message="";
			AppointmentType appointmentType=AppointmentTypes.GetOne(appointmentTypeNum);
			if(appointmentType!=null && appointmentType.RequiredProcCodesNeeded!=EnumRequiredProcCodesNeeded.None) { //Should never be null.
				List<string> listProcCodesRequiredForAppointmentType=appointmentType.CodeStrRequired.Split(",",StringSplitOptions.RemoveEmptyEntries).ToList(); //Includes duplicates.
				//Get the ProcCodes of the selected Procedures.
				List<Procedure> listProceduresSelected=listProcedures;
				List<long> listCodeNumsSelected=listProceduresSelected.Select(x => x.CodeNum).ToList();
				List<string> listProcCodesSelected=new List<string>();
				for(int i=0;i<listCodeNumsSelected.Count;i++) {
					ProcedureCode procedureCode=ProcedureCodes.GetFirstOrDefault(x=>x.CodeNum==listCodeNumsSelected[i]); //Should never return null.
					listProcCodesSelected.Add(procedureCode.ProcCode);
				}
				//Figure out how many of our required procedures are present in the selected codes, and which ones are not.
				int requiredCodesSelected=0;
				List<string> listRequiredProcCodesMissing=new List<string>();
				for(int i=0;i<listProcCodesRequiredForAppointmentType.Count;i++) {
					string requiredProcCode=listProcCodesRequiredForAppointmentType[i];
					if(listProcCodesSelected.Contains(requiredProcCode)) {
						requiredCodesSelected++;
						listProcCodesSelected.Remove(requiredProcCode);
						continue;
					}
					listRequiredProcCodesMissing.Add(requiredProcCode);
				}
				//If RequiredProcCodesNeeded is at least one, check for at least one CodeStrRequired code selected.
				if(appointmentType.RequiredProcCodesNeeded==EnumRequiredProcCodesNeeded.AtLeastOne) {
					if(requiredCodesSelected==0) {
						message="Appointment Type"+" \""+appointmentType.AppointmentTypeName+"\" "+"must contain at least one of the following procedures:"
							+"\r\n"+String.Join(", ", listProcCodesRequiredForAppointmentType);
						return message;
					}
				}
				//If its all, make sure all CodeStrRequired codes are selected
				if(appointmentType.RequiredProcCodesNeeded==EnumRequiredProcCodesNeeded.All) {
					if(requiredCodesSelected!=listProcCodesRequiredForAppointmentType.Count) {
						message="Appointment Type"+" \""+appointmentType.AppointmentTypeName+"\" "+"requires the following procedures:"
							+"\r\n"+String.Join(", ",listProcCodesRequiredForAppointmentType)
							+"\r\n\r\n"+"The following procedures are missing from this appointment:"
							+"\r\n"+String.Join(", ",listRequiredProcCodesMissing);
						return message;
					}
				}
			}
			return message;
		}

		public static int SortItemOrder(AppointmentType appointmentType1,AppointmentType appointmentType2) {
			if(appointmentType1.ItemOrder!=appointmentType2.ItemOrder){
				return appointmentType1.ItemOrder.CompareTo(appointmentType2.ItemOrder);
			}
			return appointmentType1.AppointmentTypeNum.CompareTo(appointmentType2.AppointmentTypeNum);
		}

		///<summary>Returns true if all members are the same.</summary>
		public static bool Compare(AppointmentType appointmentType1,AppointmentType appointmentType2) {
			if(appointmentType1.AppointmentTypeColor==appointmentType2.AppointmentTypeColor
				&& appointmentType1.AppointmentTypeName==appointmentType2.AppointmentTypeName
				&& appointmentType1.IsHidden==appointmentType2.IsHidden
				&& appointmentType1.ItemOrder==appointmentType2.ItemOrder)
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