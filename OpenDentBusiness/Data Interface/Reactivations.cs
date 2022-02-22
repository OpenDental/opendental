using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Reactivations{
		//If this table type will exist as cached data, uncomment the Cache Pattern region below and edit.
		/*
		#region Cache Pattern
		//This region can be eliminated if this is not a table type with cached data.
		//If leaving this region in place, be sure to add GetTableFromCache and FillCacheFromTable to the Cache.cs file with all the other Cache types.
		//Also, consider making an invalid type for this class in Cache.GetAllCachedInvalidTypes() if needed.

		private class ReactivationCache : CacheListAbs<Reactivation> {
			protected override List<Reactivation> GetCacheFromDb() {
				string command="SELECT * FROM reactivation";
				return Crud.ReactivationCrud.SelectMany(command);
			}
			protected override List<Reactivation> TableToList(DataTable table) {
				return Crud.ReactivationCrud.TableToList(table);
			}
			protected override Reactivation Copy(Reactivation reactivation) {
				return reactivation.Copy();
			}
			protected override DataTable ListToTable(List<Reactivation> listReactivations) {
				return Crud.ReactivationCrud.ListToTable(listReactivations,"Reactivation");
			}
			protected override void FillCacheIfNeeded() {
				Reactivations.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static ReactivationCache _reactivationCache=new ReactivationCache();

		public static List<Reactivation> GetDeepCopy(bool isShort=false) {
			return _reactivationCache.GetDeepCopy(isShort);
		}

		public static int GetCount(bool isShort=false) {
			return _reactivationCache.GetCount(isShort);
		}

		public static bool GetExists(Predicate<Reactivation> match,bool isShort=false) {
			return _reactivationCache.GetExists(match,isShort);
		}

		public static int GetFindIndex(Predicate<Reactivation> match,bool isShort=false) {
			return _reactivationCache.GetFindIndex(match,isShort);
		}

		public static Reactivation GetFirst(bool isShort=false) {
			return _reactivationCache.GetFirst(isShort);
		}

		public static Reactivation GetFirst(Func<Reactivation,bool> match,bool isShort=false) {
			return _reactivationCache.GetFirst(match,isShort);
		}

		public static Reactivation GetFirstOrDefault(Func<Reactivation,bool> match,bool isShort=false) {
			return _reactivationCache.GetFirstOrDefault(match,isShort);
		}

		public static Reactivation GetLast(bool isShort=false) {
			return _reactivationCache.GetLast(isShort);
		}

		public static Reactivation GetLastOrDefault(Func<Reactivation,bool> match,bool isShort=false) {
			return _reactivationCache.GetLastOrDefault(match,isShort);
		}

		public static List<Reactivation> GetWhere(Predicate<Reactivation> match,bool isShort=false) {
			return _reactivationCache.GetWhere(match,isShort);
		}

		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_reactivationCache.FillCacheFromTable(table);
		}

		///<summary>Returns the cache in the form of a DataTable. Always refreshes the ClientWeb's cache.</summary>
		///<param name="doRefreshCache">If true, will refresh the cache if RemotingRole is ClientDirect or ServerWeb.</param> 
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_reactivationCache.FillCacheFromTable(table);
				return table;
			}
			return _reactivationCache.GetTableFromCache(doRefreshCache);
		}
		#endregion Cache Pattern
		*/
		
		//Only pull out the methods below as you need them.  Otherwise, leave them commented out.
		#region Get Methods
		///<summary></summary>
		public static List<Reactivation> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Reactivation>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM reactivation WHERE PatNum = "+POut.Long(patNum);
			return Crud.ReactivationCrud.SelectMany(command);
		}
		
		///<summary>Gets one Reactivation from the db.</summary>
		public static Reactivation GetOne(long reactivationNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<Reactivation>(MethodBase.GetCurrentMethod(),reactivationNum);
			}
			return Crud.ReactivationCrud.SelectOne(reactivationNum);
		}

		public static int GetNumReminders(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetInt(MethodBase.GetCurrentMethod(),patNum);
			}
			long commType=Commlogs.GetTypeAuto(CommItemTypeAuto.REACT);
			if(commType==0) {
				return 0;
			}
			string cmd=
				@"SELECT
					COUNT(*) AS NumReminders
					FROM commlog
					WHERE commlog.CommType="+POut.Long(commType)+" "+
					"AND commlog.PatNum="+POut.Long(patNum);
			return PIn.Int(Db.GetScalar(cmd));
		}

		public static DateTime GetDateLastContacted(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<DateTime>(MethodBase.GetCurrentMethod(),patNum);
			}
			long commType=Commlogs.GetTypeAuto(CommItemTypeAuto.REACT);
			if(commType==0) {
				return DateTime.MinValue;
			}
			string cmd=
				@"SELECT
					MAX(commlog.CommDateTime) AS DateLastContacted
					FROM commlog
					WHERE commlog.CommType="+POut.Long(commType)+" "+
					"AND commlog.PatNum="+POut.Long(patNum)+" "+
					"GROUP BY commlog.PatNum";
			return PIn.DateT(Db.GetScalar(cmd));
		}

		///<summary>Gets the list of patients that need to be on the reactivation list based on the passed in filters.</summary>
		public static DataTable GetReactivationList(DateTime dateSince,DateTime dateStop,bool groupFamilies,bool showDoNotContact,bool isInactiveIncluded
			,long provNum,long clinicNum,long siteNum,long billingType,ReactivationListSort sortBy,RecallListShowNumberReminders showReactivations) 
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateSince,dateStop,groupFamilies,showDoNotContact,isInactiveIncluded,provNum,clinicNum
					,siteNum,billingType,sortBy,showReactivations);
			}
			//Get information we will need to do the query
			List<long> listReactCommLogTypeDefNums=Defs.GetDefsForCategory(DefCat.CommLogTypes,isShort:true)
				.FindAll(x => CommItemTypeAuto.REACT.GetDescription(useShortVersionIfAvailable:true).Equals(x.ItemValue)).Select(x => x.DefNum).ToList();
			int contactInterval=PrefC.GetInt(PrefName.ReactivationContactInterval);
			List<PatientStatus> listPatStatuses=new List<PatientStatus>() {PatientStatus.Patient,PatientStatus.Prospective};
			if(isInactiveIncluded) {
				listPatStatuses.Add(PatientStatus.Inactive);
			}
			string strPatStatuses=string.Join(",",listPatStatuses.Select(x => POut.Int((int)x)));
			//Get the raw set of patients who should be on the reactivation list
			string cmd=
				$@"SELECT 
						pat.PatNum,
						pat.LName,
						pat.FName,
						pat.MiddleI,
						pat.Preferred,
						pat.Guarantor,
						pat.PatStatus,
						pat.Birthdate,
						pat.PriProv,
						COALESCE(billingtype.ItemName,'') AS BillingType,
						pat.ClinicNum,
						pat.SiteNum,
						pat.PreferRecallMethod,
						'' AS ContactMethod,
						pat.HmPhone,
						pat.WirelessPhone,
						pat.WkPhone,
						{(groupFamilies?"COALESCE(guarantor.Email,pat.Email,'') AS Email,":"pat.Email,")}
						MAX(proc.ProcDate) AS DateLastProc,
						COALESCE(comm.DateLastContacted,'') AS DateLastContacted,
						COALESCE(comm.ContactedCount,0) AS ContactedCount,
						COALESCE(react.ReactivationNum,0) AS ReactivationNum,
						COALESCE(react.ReactivationStatus,0) AS ReactivationStatus,
						COALESCE(react.DoNotContact,0) as DoNotContact,
						react.ReactivationNote,
						guarantor.PatNum as GuarNum,
						guarantor.LName as GuarLName,
						guarantor.FName as GuarFName
					FROM patient pat
					INNER JOIN procedurelog proc ON pat.PatNum=proc.PatNum AND proc.ProcStatus={POut.Int((int)ProcStat.C)}
					INNER JOIN procedurecode ON procedurecode.CodeNum=proc.CodeNum AND procedurecode.ProcCode NOT IN ('D9986','D9987')
					LEFT JOIN appointment appt ON pat.PatNum=appt.PatNum AND appt.AptDateTime >= {DbHelper.Curdate()} 
					LEFT JOIN (
						SELECT
							commlog.PatNum,
							MAX(commlog.CommDateTime) AS DateLastContacted,
							COUNT(*) AS ContactedCount
							FROM commlog
							WHERE commlog.CommType IN ({string.Join(",",listReactCommLogTypeDefNums)}) 
							GROUP BY commlog.PatNum
					) comm ON pat.PatNum=comm.PatNum
					LEFT JOIN reactivation react ON pat.PatNum=react.PatNum
					LEFT JOIN definition billingtype ON pat.BillingType=billingtype.DefNum
					INNER JOIN patient guarantor ON pat.Guarantor=guarantor.PatNum
					WHERE pat.PatStatus IN ({strPatStatuses}) ";
			cmd+=provNum>0?" AND pat.PriProv="+POut.Long(provNum):"";
			cmd+=clinicNum>-1?" AND pat.ClinicNum="+POut.Long(clinicNum):"";//might still want to get the 0 clinic pats
			cmd+=siteNum>0?" AND pat.SiteNum="+POut.Long(siteNum):"";
			cmd+=billingType>0?" AND pat.BillingType="+POut.Long(billingType):"";
			cmd+=showDoNotContact?"":" AND (react.DoNotContact IS NULL OR react.DoNotContact=0)"; 
			cmd+=contactInterval>-1?" AND (comm.DateLastContacted IS NULL OR comm.DateLastContacted <= "+POut.DateT(DateTime.Today.AddDays(-contactInterval))+") ":"";
			//set number of contact attempts
			int maxReminds=PrefC.GetInt(PrefName.ReactivationCountContactMax);
			if(showReactivations==RecallListShowNumberReminders.SixPlus) {
				cmd+=" AND ContactedCount>=6 "; //don't need to look at pref this only shows in UI if the prefvalue allows it
			}
			else if(showReactivations==RecallListShowNumberReminders.Zero) {
				cmd+=" AND (comm.ContactedCount=0 OR comm.ContactedCount IS NULL) ";
			}
			else if(showReactivations!=RecallListShowNumberReminders.All) {
				int filter=(int)showReactivations-1;
				//if the contactmax pref is not -1 or 0, and the contactmax is smaller than the requested filter, replace the filter with the contactmax
				cmd+=" AND comm.ContactedCount="+POut.Int((maxReminds>0&&maxReminds<filter)?maxReminds:filter)+" "; 
			}
			else if (showReactivations==RecallListShowNumberReminders.All) { //get all but filter on the contactmax
				cmd+=" AND (comm.ContactedCount < "+POut.Int(maxReminds)+" OR comm.ContactedCount IS NULL) "; 
			}
			cmd+=$@" GROUP BY pat.PatNum 
							HAVING MAX(proc.ProcDate) < {POut.Date(dateSince)} AND MAX(proc.ProcDate) >= {POut.Date(dateStop)}
							AND MIN(appt.AptDateTime) IS NULL ";
			//set the sort by
			switch(sortBy) {
				case ReactivationListSort.Alphabetical:
					cmd+=" ORDER BY "+(groupFamilies?"guarantor.LName,guarantor.FName,pat.FName":"pat.LName,pat.FName");
					break;
				case ReactivationListSort.BillingType:
					cmd+=" ORDER BY billingtype.ItemName,DateLastContacted"+(groupFamilies?",guarantor.LName,guarantor.FName":"");
					break;
				case ReactivationListSort.LastContacted:
					cmd+=" ORDER BY IF(comm.DateLastContacted='' OR comm.DateLastContacted IS NULL,1,0),comm.DateLastContacted"+(groupFamilies?",guarantor.LName,guarantor.FName":"");
					break;
				case ReactivationListSort.LastSeen:
					cmd+=" ORDER BY MAX(proc.ProcDate)";
					break;
			}
			DataTable dtReturn=Db.GetTable(cmd);
			foreach(DataRow row in dtReturn.Rows) {
				//FOR REVIEW: currently, we are displaying PreferRecallMethod, which is what RecallList also does.  Just want to make sure we don't want to use PreferContactMethod
				row["ContactMethod"]=Recalls.GetContactFromMethod(PIn.Enum<ContactMethod>(row["PreferRecallMethod"].ToString()),groupFamilies
						,row["HmPhone"].ToString(),row["WkPhone"].ToString(),row["WirelessPhone"].ToString(),row["Email"].ToString()//guarEmail queried as Email
						,row["Email"].ToString());//Pat.Email is also "Email"
			}
			return dtReturn;
		}

		///<summary>Follows the format of the Recall addrTable, used in the RecallList to duplicate functionality for mailing/emailing patients.</summary>
		public static DataTable GetAddrTable(List<Patient> listPats,List<Patient> listGuars,bool groupFamilies,ReactivationListSort sortBy) {
			DataTable table=Recalls.GetAddrTableStructure();
			List<Patient> listPatsOrGuars=listPats;//Default to the list of patients passed in.
			//Utilize listGuars if groupFamilies is true so that family members do not get their own row.
			if(groupFamilies) {
				//This makes it so that we only return one family address even if the user has passed in every single member of the family.
				listPatsOrGuars=listGuars.FindAll(x => ListTools.In(x.PatNum,listPats.Select(y => y.Guarantor)));
			}
			foreach(Patient pat in listPatsOrGuars) {
				Patient patCur=pat;//Always the guarantor if grouping by family, otherwise a selected patient.
				Patient guar=listGuars.FirstOrDefault(x => x.PatNum==pat.Guarantor);
				//Only include Patients that were selected, rather than all family members.
				List<Patient> listSelectedPatsInFam=listPats.Where(x => x.Guarantor==guar.PatNum).ToList();
				if(listSelectedPatsInFam.Count==1) {//Selected patient may not be the guarantor.
					//So use first selected patient because this will result in an individual postcard, which should show the selected patient's name, not the
					//name of the guarantor.
					patCur=listSelectedPatsInFam.First();
				}
				DataRow row=table.NewRow();
				row["address"]						=patCur.Address+(!string.IsNullOrWhiteSpace(patCur.Address2)?Environment.NewLine+patCur.Address2:"");
				row["City"]								=patCur.City;
				row["clinicNum"]					=patCur.ClinicNum;
				row["dateDue"]						=DateTime.MinValue;//This isn't used for reactivations, but it's here keep the table the same as recall addrTable
				row["email"]							=patCur.Email;
				row["emailPatNum"]				=patCur.PatNum;
				row["famList"]						=listSelectedPatsInFam.Count>1 ? string.Join(",",listSelectedPatsInFam.Select(x => x.FName)) : "";
				row["guarLName"]					=guar.LName;
				row["numberOfReminders"]	=Reactivations.GetNumReminders(patCur.PatNum);
				row["patientNameF"]				=patCur.GetNameFirstOrPreferred();
				row["patientNameFL"]			=patCur.GetNameFLnoPref();
				row["patNums"]						=patCur.PatNum;
				row["State"]							=patCur.State;
				row["Zip"]								=patCur.Zip;
				table.Rows.Add(row);
			}
			return table;
		}

		#endregion Get Methods

		#region Insert
		///<summary></summary>
		public static long Insert(Reactivation reactivation){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				reactivation.ReactivationNum=Meth.GetLong(MethodBase.GetCurrentMethod(),reactivation);
				return reactivation.ReactivationNum;
			}
			return Crud.ReactivationCrud.Insert(reactivation);
		}
		#endregion Insert

		#region Update
		///<summary></summary>
		public static void Update(Reactivation reactivation){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),reactivation);
				return;
			}
			Crud.ReactivationCrud.Update(reactivation);
		}

		public static void UpdateStatus(long reactivationNum,long statusDefNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),reactivationNum,statusDefNum);
				return;
			}
			string cmd="UPDATE reactivation SET ReactivationStatus="+POut.Long(statusDefNum) +" WHERE ReactivationNum="+POut.Long(reactivationNum);
			Db.NonQ(cmd);
		}

		#endregion Update

		#region Delete
		///<summary></summary>
		public static void Delete(long reactivationNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),reactivationNum);
				return;
			}
			Crud.ReactivationCrud.Delete(reactivationNum);
		}
		#endregion Delete
	}

	///<summary></summary>
	public enum ReactivationListSort {
		///<summary></summary>
		LastContacted,
		///<summary></summary>
		BillingType,
		///<summary></summary>
		Alphabetical,
		///<summary></summary>
		LastSeen,
	}

}