using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;
using System.IO;
using System.Globalization;
using CodeBase;
using System.Diagnostics;
using OpenDentBusiness.Eclaims;
using System.Threading;
using System.Text;

namespace OpenDentBusiness {
	///<summary></summary>
	public class Clearinghouses {
		#region Cache Pattern

		private class ClearinghouseCache: CacheListAbs<Clearinghouse> {
			protected override List<Clearinghouse> GetCacheFromDb() {
				string command="SELECT * FROM clearinghouse WHERE ClinicNum=0 ORDER BY Description";
				List<Clearinghouse> listClearinghouses=Crud.ClearinghouseCrud.SelectMany(command);
				listClearinghouses.ForEach(x => x.Password=GetRevealPassword(x.Password));
				return listClearinghouses;
			}
			protected override List<Clearinghouse> TableToList(DataTable table) {
				return Crud.ClearinghouseCrud.TableToList(table);
			}
			protected override Clearinghouse Copy(Clearinghouse clearinghouse) {
				return clearinghouse.Copy();
			}
			protected override DataTable ListToTable(List<Clearinghouse> listClearinghouses) {
				return Crud.ClearinghouseCrud.ListToTable(listClearinghouses,"Clearinghouse");
			}
			protected override void FillCacheIfNeeded() {
				Clearinghouses.GetTableFromCache(false);
			}
			protected override bool IsInListShort(Clearinghouse clearinghouse) {
				return clearinghouse.CommBridge!=EclaimsCommBridge.MercuryDE;
			}
		}
		
		///<summary>The object that accesses the cache in a thread-safe manner.  The clearinghouse cache will only include HQ level houses.</summary>
		private static ClearinghouseCache _clearinghouseCache=new ClearinghouseCache();

		public static List<Clearinghouse> GetDeepCopy(bool isShort=false) {
			return _clearinghouseCache.GetDeepCopy(isShort);
		}

		public static List<Clearinghouse> GetWhere(Predicate<Clearinghouse> match,bool isShort=false) {
			return _clearinghouseCache.GetWhere(match,isShort);
		}

		public static Clearinghouse GetFirstOrDefault(Func<Clearinghouse,bool> match,bool isShort=false) {
			return _clearinghouseCache.GetFirstOrDefault(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_clearinghouseCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_clearinghouseCache.FillCacheFromTable(table);
				return table;
			}
			return _clearinghouseCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_clearinghouseCache.ClearCache();
		}
		#endregion

		#region Get Methods

		///<summary>Gets all clearinghouses for the specified clinic.  Returns an empty list if clinicNum=0.  
		///Use the cache if you want all HQ Clearinghouses.</summary>
		public static List<Clearinghouse> GetAllNonHq() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Clearinghouse>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM clearinghouse WHERE ClinicNum!=0 ORDER BY Description";
			List<Clearinghouse> listClearinghouses=Crud.ClearinghouseCrud.SelectMany(command);
			for(int i=0;i<listClearinghouses.Count;i++){ 
				listClearinghouses[i].Password=GetRevealPassword(listClearinghouses[i].Password);
			}
			return listClearinghouses;
		}

		///<summary>Returns the HQ-level default clearinghouse.  You must manually override using OverrideFields if needed.  If no default present, returns null.</summary>
		public static Clearinghouse GetDefaultEligibility() {
			//No need to check MiddleTierRole; no call to db.
			return GetClearinghouse(PrefC.GetLong(PrefName.ClearinghouseDefaultEligibility));
		}

		///<summary>Gets the last batch number from db for the HQ version of this clearinghouseClin and increments it by one.
		///Then saves the new value to db and returns it.  So even if the new value is not used for some reason, it will have already been incremented.
		///Remember that LastBatchNumber is never accurate with local data in memory.</summary>
		public static int GetNextBatchNumber(Clearinghouse clearinghouse){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),clearinghouse);
			}
			//get last batch number
			string command="SELECT LastBatchNumber FROM clearinghouse "
				+"WHERE ClearinghouseNum = "+POut.Long(clearinghouse.HqClearinghouseNum);
			DataTable table=Db.GetTable(command);
			int batchNum=PIn.Int(table.Rows[0][0].ToString());
			//and increment it by one
			if(clearinghouse.Eformat==ElectronicClaimFormat.Canadian){
				if(batchNum==999999){
					batchNum=1;
				}
				else{
					batchNum++;
				}
			}
			else{
				if(batchNum==999){
					batchNum=1;
				}
				else{
					batchNum++;
				}
			}
			//save the new batch number. Even if user cancels, it will have incremented.
			command="UPDATE clearinghouse SET LastBatchNumber="+batchNum.ToString()
				+" WHERE ClearinghouseNum = "+POut.Long(clearinghouse.HqClearinghouseNum);
			Db.NonQ(command);
			return batchNum;
		}

		///<summary>Returns the clearinghouseNum for claims for the supplied payorID.  If the payorID was not entered or if no default was set, then 0 is returned.</summary>
		public static long AutomateClearinghouseHqSelection(string payorID,EnumClaimMedType enumClaimMedType){
			//No need to check MiddleTierRole; no call to db.
			//payorID can be blank.  For example, Renaissance does not require payorID.
			Clearinghouse clearinghouseHq=null;
			if(enumClaimMedType==EnumClaimMedType.Dental){
				if(PrefC.GetLong(PrefName.ClearinghouseDefaultDent)==0){
					return 0;
				}
				clearinghouseHq=GetClearinghouse(PrefC.GetLong(PrefName.ClearinghouseDefaultDent));
			}
			if(enumClaimMedType==EnumClaimMedType.Medical || enumClaimMedType==EnumClaimMedType.Institutional){
				if(PrefC.GetLong(PrefName.ClearinghouseDefaultMed)==0){
					//No default set, substituting emdeon medical otherwise first medical clearinghouse.
					List<Clearinghouse> listClearingHouses=GetDeepCopy(false);
					clearinghouseHq=listClearingHouses.Find(x => x.CommBridge==EclaimsCommBridge.EmdeonMedical&&x.HqClearinghouseNum==x.ClearinghouseNum);
					if(clearinghouseHq==null) {
						clearinghouseHq=listClearingHouses.Find(x => x.Eformat==ElectronicClaimFormat.x837_5010_med_inst&&x.HqClearinghouseNum==x.ClearinghouseNum);
					}
					//If we can't find a clearinghouse at all, just return 0.
					if(clearinghouseHq==null) {
						return 0;
					}
					return clearinghouseHq.ClearinghouseNum;
				}
				clearinghouseHq=GetClearinghouse(PrefC.GetLong(PrefName.ClearinghouseDefaultMed));
			}
			if(clearinghouseHq==null){//we couldn't find a default clearinghouse for that medType.  Needs to always be a default.
				return 0;
			}
			Clearinghouse clearinghouseOverride=GetClearinghouseByPayorID(payorID);
			if(clearinghouseOverride==null){ //no override, so just return the default.
				return clearinghouseHq.ClearinghouseNum;
			}
			if(clearinghouseOverride.Eformat==ElectronicClaimFormat.x837D_4010 
				|| clearinghouseOverride.Eformat==ElectronicClaimFormat.x837D_5010_dental
				|| clearinghouseOverride.Eformat==ElectronicClaimFormat.Canadian 
				|| clearinghouseOverride.Eformat==ElectronicClaimFormat.Ramq)
			{//all dental formats
				if(enumClaimMedType==EnumClaimMedType.Dental){//med type matches
					return clearinghouseOverride.ClearinghouseNum;
				}
			}
			if(clearinghouseOverride.Eformat!=ElectronicClaimFormat.x837_5010_med_inst){
				return clearinghouseHq.ClearinghouseNum;
			}
			if(enumClaimMedType==EnumClaimMedType.Medical || enumClaimMedType==EnumClaimMedType.Institutional) {//med type matches
				return clearinghouseOverride.ClearinghouseNum;
			}
			return clearinghouseHq.ClearinghouseNum;
		}

		///<summary>Returns the first clearinghouse that is associated to the corresponding payorID passed in.  Returns null if no match found.</summary>
		private static Clearinghouse GetClearinghouseByPayorID(string payorID) {
			//No need to check MiddleTierRole; no call to db.
			Clearinghouse clearinghouse=null;
			if(string.IsNullOrEmpty(payorID)) {
				return clearinghouse;
			}
			//Take the entire clearinghouse cache (which is typically small) and flatten it into a dictionary by payor ID to clearinhouse.
			//Each clearinghouse can be associated to multiple payor IDs (comma delimited string) so that must be broken down first.
			GetDeepCopy().Select(x => new {
				listPayorToHouses=x.Payors.Split(',').ToList()//Take every clearinghouse's payors and split them up (comma delimited string per house).
					.Select(y => new { payor=y,house=x })      //Make a new object that ties the clearinghouse and payorID together (List<List<payor,house>>)
			}).SelectMany(x => x.listPayorToHouses)         //Flatten the list of lists to make one long list of new objects (List<payor,house>)
			.GroupBy(x => x.payor)                         //Group these new objects by the payor (if there are any duplicates we'll grab first in list)
			.ToDictionary(x => x.Key,x => x.First().house) //Make a dictionary out of the new objects where Key: payor Value: the first house
			.TryGetValue(payorID,out clearinghouse);       //Try and find the corresponding clearinghouse via the payorID passed in.
			return clearinghouse;//Can return null and that is just fine.
		}

		///<summary>Returns the HQ-level default clearinghouse.  You must manually override using OverrideFields if needed.  If no default present, returns null.</summary>
		public static Clearinghouse GetDefaultDental(){
			//No need to check MiddleTierRole; no call to db.
			return GetClearinghouse(PrefC.GetLong(PrefName.ClearinghouseDefaultDent));
		}

		///<summary>Gets an HQ clearinghouse from cache.  Will return null if invalid.</summary>
		public static Clearinghouse GetClearinghouse(long clearinghouseNum){
			//No need to check MiddleTierRole; no call to db.
			Clearinghouse clearinghouse=GetFirstOrDefault(x => x.ClearinghouseNum==clearinghouseNum);
			return clearinghouse;
		}

		///<summary>Gets revealed password for a clearinghouse password.</summary>
		public static string GetRevealPassword(string concealPassword) {
			//No need to check MiddleTierRole; no call to db.
			string revealedPassword="";
			CDT.Class1.RevealClearinghouse(concealPassword,out revealedPassword);
			return revealedPassword;
		}

		///<summary>Returns the clinic-level clearinghouse for the passed in Clearinghouse.  Usually used in conjunction with ReplaceFields().
		///Can return null.</summary>
		public static Clearinghouse GetForClinic(Clearinghouse clearinghouseHq,long clinicNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Clearinghouse>(MethodBase.GetCurrentMethod(),clearinghouseHq,clinicNum);
			}
			if(clinicNum==0) { //HQ
				return null;
			}
			string command="SELECT * FROM clearinghouse WHERE HqClearinghouseNum="+clearinghouseHq.ClearinghouseNum+" AND ClinicNum="+POut.Long(clinicNum);
			Clearinghouse clearinghouseRetVal=Crud.ClearinghouseCrud.SelectOne(command);
			if(clearinghouseRetVal==null) {
				return null;
			}
			clearinghouseRetVal.Password=GetRevealPassword(clearinghouseRetVal.Password);
			return clearinghouseRetVal;
		}
		#endregion

		#region Insert
		///<summary>Inserts one clearinghouse into the database.  Use this if you know that your clearinghouse will be inserted at the HQ-level.</summary>
		public static long Insert(Clearinghouse clearinghouse) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				clearinghouse.ClearinghouseNum=Meth.GetLong(MethodBase.GetCurrentMethod(),clearinghouse);
				clearinghouse.HqClearinghouseNum=clearinghouse.ClearinghouseNum;
				return clearinghouse.ClearinghouseNum;
			}
			long clearinghouseNum=Crud.ClearinghouseCrud.Insert(clearinghouse);
			clearinghouse.HqClearinghouseNum=clearinghouseNum;
			Crud.ClearinghouseCrud.Update(clearinghouse);
			return clearinghouseNum;
		}
		#endregion

		#region Update

		///<summary>Updates the clearinghouse in the database that has the same primary key as the passed-in clearinghouse.   
		///Use this if you know that your clearinghouse will be updated at the HQ-level, 
		///or if you already have a well-defined clinic-level clearinghouse.  For lists of clearinghouses, use the Sync method instead.</summary>
		public static void Update(Clearinghouse clearinghouse) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clearinghouse);
				return;
			}
			Crud.ClearinghouseCrud.Update(clearinghouse);
		}

		public static void Update(Clearinghouse clearinghouse,Clearinghouse clearinghouseOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clearinghouse,clearinghouseOld);
				return;
			}
			Crud.ClearinghouseCrud.Update(clearinghouse,clearinghouseOld);
		}

		///<summary>Syncs a given list of clinic-level clearinghouses to a list of old clinic-level clearinghouses.</summary>
		public static void Sync(List<Clearinghouse> listClearinghousesNew,List<Clearinghouse> listClearinghousesOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listClearinghousesNew,listClearinghousesOld);
				return;
			}
			Crud.ClearinghouseCrud.Sync(listClearinghousesNew,listClearinghousesOld);
		}
		#endregion

		#region Delete
		///<summary>Deletes the passed-in Hq clearinghouse for all clinics.  Only pass in clearinghouses with ClinicNum==0.</summary>
		public static void Delete(Clearinghouse clearinghouseHq) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clearinghouseHq);
				return;
			}
			string command="DELETE FROM clearinghouse WHERE ClearinghouseNum = '"+POut.Long(clearinghouseHq.ClearinghouseNum)+"'";
			Db.NonQ(command);
			command="DELETE FROM clearinghouse WHERE HqClearinghouseNum='"+POut.Long(clearinghouseHq.ClearinghouseNum)+"'";
			Db.NonQ(command);
		}
		#endregion

		#region Misc Methods

		///<summary>Replaces all clinic-level fields in ClearinghouseHq with non-blank fields 
		///from the clinic-level clearinghouse for the passed-in clinicNum. Non clinic-level fields are not replaced.
		///If Clinics are disabled, uses clearinghouseHq settings.</summary>
		public static Clearinghouse OverrideFields(Clearinghouse clearinghouseHq,long clinicNum) {
			//No need to check MiddleTierRole; no call to db.
			//Do not use given clinicNum when clinics are disabled.
			//Otherwise clinic level clearinghouse settings that were set when clinics were enabled would be used
			//and user would have no way of fixing them unless they turned clinics back on.
			//Use unassigned settings since they are what show in the UI when editing clearinghouse settings.
			clinicNum=(PrefC.HasClinicsEnabled?clinicNum:0);
			Clearinghouse clearinghouseClin=Clearinghouses.GetForClinic(clearinghouseHq,clinicNum);
			return OverrideFields(clearinghouseHq,clearinghouseClin);
		}

		///<summary>Replaces all clinic-level fields in ClearinghouseHq with non-blank fields in clearinghouseClin.
		///Non clinic-level fields are commented out and not replaced.</summary>
		public static Clearinghouse OverrideFields(Clearinghouse clearinghouseHq,Clearinghouse clearinghouseClin) {
			//No need to check MiddleTierRole; no call to db.
			if(clearinghouseHq==null) {
				return null;
			}
			Clearinghouse clearinghouseRetVal=clearinghouseHq.Copy();
			if(clearinghouseClin==null) { //if a null clearingHouseClin was passed in, just return clearinghouseHq.
				return clearinghouseRetVal;
			}
			//HqClearinghouseNum must be set for refreshing the cache when deleting.
			clearinghouseRetVal.HqClearinghouseNum=clearinghouseClin.HqClearinghouseNum;
			//ClearinghouseNum must be set so that updates do not create new entries every time.
			clearinghouseRetVal.ClearinghouseNum=clearinghouseClin.ClearinghouseNum;
			//ClinicNum must be set so that the correct clinic is assigned when inserting new clinic level clearinghouses.
			clearinghouseRetVal.ClinicNum=clearinghouseClin.ClinicNum;
			clearinghouseRetVal.IsEraDownloadAllowed=clearinghouseClin.IsEraDownloadAllowed;
			clearinghouseRetVal.IsClaimExportAllowed=clearinghouseClin.IsClaimExportAllowed;
			//fields that should not be replaced are commented out.
			//if(!String.IsNullOrEmpty(clearinghouseClin.Description)) {
			//	clearinghouseRetVal.Description=clearinghouseClin.Description;
			//}
			if(!String.IsNullOrEmpty(clearinghouseClin.ExportPath)) {
				clearinghouseRetVal.ExportPath=clearinghouseClin.ExportPath;
			}
			//if(!String.IsNullOrEmpty(clearinghouseClin.Payors)) {
			//	clearinghouseRetVal.Payors=clearinghouseClin.Payors;
			//}
			//if(clearinghouseClin.Eformat!=0 && clearinghouseClin.Eformat!=null) {
			//	clearinghouseRetVal.Eformat=clearinghouseClin.Eformat;
			//}
			//if(!String.IsNullOrEmpty(clearinghouseClin.ISA05)) {
			//	clearinghouseRetVal.ISA05=clearinghouseClin.ISA05;
			//}
			if(!String.IsNullOrEmpty(clearinghouseClin.SenderTIN)) {
				clearinghouseRetVal.SenderTIN=clearinghouseClin.SenderTIN;
			}
			//if(!String.IsNullOrEmpty(clearinghouseClin.ISA07)) {
			//	clearinghouseRetVal.ISA07=clearinghouseClin.ISA07;
			//}
			//if(!String.IsNullOrEmpty(clearinghouseClin.ISA08)) {
			//	clearinghouseRetVal.ISA08=clearinghouseClin.ISA08;
			//}
			//if(!String.IsNullOrEmpty(clearinghouseClin.ISA15)) {
			//	clearinghouseRetVal.ISA15=clearinghouseClin.ISA15;
			//}
			if(!String.IsNullOrEmpty(clearinghouseClin.Password)) {
				clearinghouseRetVal.Password=clearinghouseClin.Password;
			}
			if(!String.IsNullOrEmpty(clearinghouseClin.ResponsePath)) {
				clearinghouseRetVal.ResponsePath=clearinghouseClin.ResponsePath;
			}
			//if(clearinghouseClin.CommBridge!=0 && clearinghouseClin.CommBridge!=null) {
			//	clearinghouseRetVal.CommBridge=clearinghouseClin.CommBridge;
			//}
			if(!String.IsNullOrEmpty(clearinghouseClin.ClientProgram)) {
				clearinghouseRetVal.ClientProgram=clearinghouseClin.ClientProgram;
			}
			//clearinghouseRetVal.LastBatchNumber=;//Not editable is UI and should not be updated here.  See GetNextBatchNumber() above.
			//if(clearinghouseClin.ModemPort!=0 && clearinghouseClin.ModemPort!=null) {
			//	clearinghouseRetVal.ModemPort=clearinghouseClin.ModemPort;
			//}
			if(!String.IsNullOrEmpty(clearinghouseClin.LoginID)) {
				clearinghouseRetVal.LoginID=clearinghouseClin.LoginID;
			}
			if(!String.IsNullOrEmpty(clearinghouseClin.SenderName)) {
				clearinghouseRetVal.SenderName=clearinghouseClin.SenderName;
			}
			if(!String.IsNullOrEmpty(clearinghouseClin.SenderTelephone)) {
				clearinghouseRetVal.SenderTelephone=clearinghouseClin.SenderTelephone;
			}
			//if(!String.IsNullOrEmpty(clearinghouseClin.GS03)) {
			//	clearinghouseRetVal.GS03=clearinghouseClin.GS03;
			//}
			//if(!String.IsNullOrEmpty(clearinghouseClin.ISA02)) {
			//	clearinghouseRetVal.ISA02=clearinghouseClin.ISA02;
			//}
			//if(!String.IsNullOrEmpty(clearinghouseClin.ISA04)) {
			//	clearinghouseRetVal.ISA04=clearinghouseClin.ISA04;
			//}
			//if(!String.IsNullOrEmpty(clearinghouseClin.ISA16)) {
			//	clearinghouseRetVal.ISA16=clearinghouseClin.ISA16;
			//}
			//if(!String.IsNullOrEmpty(clearinghouseClin.SeparatorData)) {
			//	clearinghouseRetVal.SeparatorData=clearinghouseClin.SeparatorData;
			//}
			//if(!String.IsNullOrEmpty(clearinghouseClin.SeparatorSegment)) {
			//	clearinghouseRetVal.SeparatorSegment=clearinghouseClin.SeparatorSegment;
			//}
			clearinghouseRetVal.IsAttachmentSendAllowed=clearinghouseClin.IsAttachmentSendAllowed;
			return clearinghouseRetVal;
		}

		public static void RetrieveReportsAutomatic(bool isAllClinics) {
			List<long> listClinicNums=new List<long>();
			if(isAllClinics) {
				listClinicNums=Clinics.GetDeepCopy(true).Select(x => x.ClinicNum).ToList();
				listClinicNums.Add(0);//Include HQ.  Especially important for organizations not using Clinics.
			}
			else {
				listClinicNums=new List<long> { Clinics.ClinicNum };
			}
			string errorMessage;
			bool isTimeToRetrieve=IsTimeToRetrieveReports(true,out errorMessage);
			if(isTimeToRetrieve) {
				Prefs.UpdateDateT(PrefName.ClaimReportReceiveLastDateTime,DateTime.Now);
			}
			List<Clearinghouse> listClearinghouses=GetDeepCopy();
			long clearinghouseNumDefault=PrefC.GetLong(PrefName.ClearinghouseDefaultDent);
			for(int i=0;i<listClearinghouses.Count;i++) {
				Clearinghouse clearinghouseHq=listClearinghouses[i];
				Clearinghouse clearinghouseClin;
				for(int j=0;j<listClinicNums.Count;j++) {
					clearinghouseClin=OverrideFields(clearinghouseHq,listClinicNums[j]);
					RetrieveReportsAutomaticHelper(clearinghouseClin,clearinghouseHq,clearinghouseNumDefault,isTimeToRetrieve);
				}
			}

		}

		///<summary>Returns true if it is time to retrieve reports.</summary>
		private static bool IsTimeToRetrieveReports(bool isAutomaticMode,out string errorMessage,IODProgressExtended odProgressExtended=null) {
			odProgressExtended=odProgressExtended??new ODProgressExtendedNull();
			DateTime dateTimeLastReport=PIn.DateT(PrefC.GetStringNoCache(PrefName.ClaimReportReceiveLastDateTime));
			double minutesClaimReportReceiveInternal=PIn.Double(PrefC.GetStringNoCache(PrefName.ClaimReportReceiveInterval));//Interval in minutes.
			DateTime timeToReceive=DateTime.Now.Date+PrefC.GetDateT(PrefName.ClaimReportReceiveTime).TimeOfDay;
			double minutesDiff=DateTime.Now.Subtract(dateTimeLastReport).TotalMinutes;
			errorMessage="";
			if(isAutomaticMode) {
				if(minutesClaimReportReceiveInternal!=0) { //preference is set instead of pref for specific time. 
					if(minutesDiff < minutesClaimReportReceiveInternal) {
						//Automatically retrieving reports from this computer and the report interval has not passed yet.
						return false;
					}
				}
				else {//pref is set for specific time, not interval
					if(DateTime.Now.TimeOfDay < timeToReceive.TimeOfDay //We haven't reach to the time to retrieve
						|| dateTimeLastReport.Date==DateTime.Today)//Or we have already retrieved today
					{
						//Automatically retrieving reports and the time has not come to pass yet
						return false;
					}
				}
			}
			else if(minutesDiff < 1) {
				//When the user presses the Get Reports button manually we allow them to get reports up to once per minute
				errorMessage=Lans.g(odProgressExtended.LanThis,"Reports can only be retrieved once per minute.");
				odProgressExtended.UpdateProgress(Lans.g(odProgressExtended.LanThis,"Reports can only be retrieved once per minute. Attempting to import manually downloaded reports."));
				return false;
			}
			return true;
		}

		private static void RetrieveReportsAutomaticHelper(Clearinghouse clearinghouseClin,Clearinghouse clearinghouseHq,long clearinghouseNumDefault
			,bool isTimeToRetrieve)
		{
			if(!Directory.Exists(clearinghouseClin.ResponsePath)) {
				return;
			}
			if(clearinghouseHq.ClearinghouseNum==clearinghouseNumDefault) {//If it's the default dental clearinghouse
				RetrieveAndImport(clearinghouseClin,true,isTimeToRetrieve: isTimeToRetrieve);
			}
			else if(clearinghouseHq.Eformat==ElectronicClaimFormat.None) {//And the format is "None" (accessed from all regions)
				RetrieveAndImport(clearinghouseClin,true,isTimeToRetrieve: isTimeToRetrieve);
			}
			else if(clearinghouseHq.CommBridge==EclaimsCommBridge.BCBSGA) {
				BCBSGA.Retrieve(clearinghouseClin,true,new TerminalConnector());
			}
			else if(clearinghouseHq.Eformat==ElectronicClaimFormat.Canadian && CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				//Or the Eformat is Canadian and the region is Canadian.  In Canada, the "Outstanding Reports" are received upon request.
				//Canadian reports must be retrieved using an office num and valid provider number for the office,
				//which will cause all reports for that office to be returned.
				//Here we loop through all providers and find CDAnet providers with a valid provider number and office number, and we only send
				//one report download request for one provider from each office.  For most offices, the loop will only send a single request.
				List<Provider> listProviders=Providers.GetDeepCopy(true);
				List<string> listOfficeNums=new List<string>();
				for(int j=0;j<listProviders.Count;j++) {//Get all unique office numbers from the providers.
					if(!listProviders[j].IsCDAnet || listProviders[j].NationalProvID=="" || listProviders[j].CanadianOfficeNum=="") {
						continue;
					}
					if(!listOfficeNums.Contains(listProviders[j].CanadianOfficeNum)) {//Ignore duplicate office numbers.
						listOfficeNums.Add(listProviders[j].CanadianOfficeNum);
						try {
							clearinghouseHq=Eclaims.Canadian.GetCanadianClearinghouseHq(null);
							clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
							//Run both version 02 and version 04 reports for all carriers and all networks.
							Eclaims.CanadianOutput.GetOutstandingForDefault(listProviders[j]);
						}
						catch {
							//Supress errors importing reports.
						}
					}
				}
			}
			else if(clearinghouseHq.Eformat==ElectronicClaimFormat.Dutch && CultureInfo.CurrentCulture.Name.EndsWith("DE")) {
				//Or the Eformat is German and the region is German
				RetrieveAndImport(clearinghouseClin,true,isTimeToRetrieve: isTimeToRetrieve);
			}
			else if(clearinghouseHq.Eformat!=ElectronicClaimFormat.Canadian
				&& clearinghouseHq.Eformat!=ElectronicClaimFormat.Dutch
				&& CultureInfo.CurrentCulture.Name.EndsWith("US")) //Or the Eformat is in any other format and the region is US
			{
				RetrieveAndImport(clearinghouseClin,true,isTimeToRetrieve: isTimeToRetrieve);
			}
		}

		private static string RetrieveReports(Clearinghouse clearinghouseClin,bool isAutomaticMode,IODProgressExtended odProgressExtended=null) {
			odProgressExtended=odProgressExtended??new ODProgressExtendedNull();
			odProgressExtended.UpdateProgress(Lans.g(odProgressExtended.LanThis,"Beginning report retrieval..."),"reports","0%");
			if(odProgressExtended.IsPauseOrCancel()) {
				return Lans.g(odProgressExtended.LanThis,"Process canceled by user.");
			}
			StringBuilder stringBuilder=new StringBuilder();
			if(Plugins.HookMethod(null,"Clearinghouses.RetrieveReports_afterProgressPauseOrCancel",clearinghouseClin,stringBuilder,odProgressExtended)) {
				return stringBuilder.ToString();
			}
			if(clearinghouseClin.ISA08=="113504607") {//TesiaLink
																								//But the import will still happen
				return "";
			}
			if(clearinghouseClin.CommBridge==EclaimsCommBridge.None
				|| clearinghouseClin.CommBridge==EclaimsCommBridge.Renaissance
				|| clearinghouseClin.CommBridge==EclaimsCommBridge.RECS) 
			{
				return "";
			}
			if(clearinghouseClin.CommBridge==EclaimsCommBridge.WebMD) {
				if(!WebMD.Launch(clearinghouseClin,0,isAutomaticMode,odProgressExtended)) {
					return Lans.g("FormClaimReports","Error retrieving.")+"\r\n"+WebMD.ErrorMessage;
				}
			}
			else if(clearinghouseClin.CommBridge==EclaimsCommBridge.BCBSGA) {
				if(!BCBSGA.Retrieve(clearinghouseClin,true,new TerminalConnector(),odProgressExtended)) {
					return Lans.g("FormClaimReports","Error retrieving.")+"\r\n"+BCBSGA.ErrorMessage;
				}
			}
			else if(clearinghouseClin.CommBridge==EclaimsCommBridge.ClaimConnect) {
				if(!Directory.Exists(clearinghouseClin.ResponsePath)) {
					//The clearinghouse report path is not setup.  Therefore, the customer does not use ClaimConnect reports via web services.
					if(isAutomaticMode) {//The user opened FormClaimsSend, or FormOpenDental called this function automatically.
						return "";//Suppress error message.
					}
					else {//The user pressed the Get Reports button manually.
								//This cannot happen, because the user is blocked by the UI before they get to this point.
					}
				}
				else if(!ClaimConnect.Retrieve(clearinghouseClin,odProgressExtended)) {
					if(ClaimConnect.ErrorMessage.Contains(": 150\r\n")) {//Error message 150 "Service Not Contracted"
						if(isAutomaticMode) {//The user opened FormClaimsSend, or FormOpenDental called this function automatically.
							return "";//Pretend that there is no error when loading FormClaimsSend for those customers who do not pay for ERA service.
						}
						else {//The user pressed the Get Reports button manually.
									//The old way.  Some customers still prefer to go to the dentalxchange web portal to view reports because the ERA service costs money.
							try {
								Process.Start(@"http://www.dentalxchange.com");
							}
							catch(Exception ex) {
								ex.DoNothing();
								return Lans.g("FormClaimReports","Could not locate the site.");
							}
						}
					}
					return Lans.g("FormClaimReports","Error retrieving.")+"\r\n"+ClaimConnect.ErrorMessage;
				}
			}
			else if(clearinghouseClin.CommBridge==EclaimsCommBridge.AOS) {
				try {
					//This path would never exist on Unix, so no need to handle back slashes.
					Process.Start(@"C:\Program files\AOS\AOSCommunicator\AOSCommunicator.exe");
				}
				catch {
					return Lans.g("FormClaimReports","Could not locate the file.");
				}
			}
			else if(clearinghouseClin.CommBridge==EclaimsCommBridge.MercuryDE) {
				if(!MercuryDE.Launch(clearinghouseClin,0,odProgressExtended)) { 
					return Lans.g("FormClaimReports","Error retrieving.")+"\r\n"+MercuryDE.ErrorMessage;
				}
			}
			else if(clearinghouseClin.CommBridge==EclaimsCommBridge.EmdeonMedical) {
				if(!EmdeonMedical.Retrieve(clearinghouseClin,odProgressExtended)) {
					return Lans.g("FormClaimReports","Error retrieving.")+"\r\n"+EmdeonMedical.ErrorMessage;
				}
			}
			else if(clearinghouseClin.CommBridge==EclaimsCommBridge.DentiCal) {
				if(!DentiCal.Launch(clearinghouseClin,0,odProgressExtended)) {
					return Lans.g("FormClaimReports","Error retrieving.")+"\r\n"+DentiCal.ErrorMessage;
				}
			}
			else if(clearinghouseClin.CommBridge==EclaimsCommBridge.EDS) {
				List<string> listEdsErrors=new List<string>();
				if(!EDS.Retrieve277s(clearinghouseClin,odProgressExtended)) {
					listEdsErrors.Add(Lans.g("FormClaimReports","Error retrieving.")+"\r\n"+EDS.ErrorMessage);
				}
				if(!EDS.Retrieve835s(clearinghouseClin,odProgressExtended)) {
					listEdsErrors.Add(Lans.g("FormClaimReports","Error retrieving.")+"\r\n"+EDS.ErrorMessage);
				}
				if(listEdsErrors.Count>0) {
					return string.Join("\r\n",listEdsErrors);
				}
			}
			else if(clearinghouseClin.CommBridge==EclaimsCommBridge.Lantek) {
				try {
					//This path would never exist on Unix, so no need to handle back slashes.
					Process.Start(@"C:\Lantek\Program\Trakker.exe");
				}
				catch {
					return Lans.g("FormClaimReports","Could not locate the file.");
				}
			}
			return "";
		}

		///<summary>Takes any files found in the reports folder for the clearinghouse, and imports them into the database.
		///Moves the original file into an Archive sub folder.
		///Returns a string with any errors that occurred.</summary>
		private static string ImportReportFiles(Clearinghouse clearinghouseClin,IODProgressExtended odProgressExtended=null) { //uses clinic-level clearinghouse where necessary.
			odProgressExtended=odProgressExtended??new ODProgressExtendedNull();
			if(!Directory.Exists(clearinghouseClin.ResponsePath)) {
				return Lans.g("FormClaimReports","Report directory does not exist")+": "+clearinghouseClin.ResponsePath+"\r\n"+Lans.g("FormClaimReports","Go to Setup, Family/Insurance, Clearinghouses, and double-click the desired clearinghouse to update the path.");
			}
			if(clearinghouseClin.Eformat==ElectronicClaimFormat.Canadian||clearinghouseClin.Eformat==ElectronicClaimFormat.Ramq) {
				//the report path is shared with many other important files.  Do not process anything.  Comm is synchronous only.
				return "";
			}
			odProgressExtended.UpdateProgress(Lans.g(odProgressExtended.LanThis,"Reading download files"),"reports","55%",55);
			if(odProgressExtended.IsPauseOrCancel()) {
				return Lans.g(odProgressExtended.LanThis,"Import canceled by user.");
			}
			string[] stringArrayFiles=null;
			string pathToArchiveDir;
			try {
				stringArrayFiles=Directory.GetFiles(clearinghouseClin.ResponsePath);
				pathToArchiveDir=ODFileUtils.CombinePaths(clearinghouseClin.ResponsePath,"Archive"+"_"+DateTime.Now.Year.ToString());
				if(!Directory.Exists(pathToArchiveDir)) {
					Directory.CreateDirectory(pathToArchiveDir);
				}
			}
			catch(UnauthorizedAccessException ex) {
				ex.DoNothing();
				return Lans.g("FormClaimReports","Access to the Report Path is denied.  Try running as administrator or contact your network administrator.");
			}
			List<string> listFilesFailedToMove=new List<string>();
			List<string> listFilesFailedToImport=new List<string>();
			odProgressExtended.UpdateProgress(Lans.g(odProgressExtended.LanThis,"Files read."));
			odProgressExtended.UpdateProgress(Lans.g(odProgressExtended.LanThis,"Importing files"),"reports","83%",83);
			if(stringArrayFiles.Length>0) {
				odProgressExtended.UpdateProgressDetailed(Lans.g(odProgressExtended.LanThis,"Importing"),tagString:"import");//add a new progress bar for imports if there are any to import
			}
			else {
				odProgressExtended.UpdateProgress(Lans.g(odProgressExtended.LanThis,"No files to import."));
			}
			for(int i=0;i<stringArrayFiles.Length;i++) {
				int percentUpdated=(i/stringArrayFiles.Length)*100;
				odProgressExtended.UpdateProgress(Lans.g(odProgressExtended.LanThis,"Importing")+" "+i+" / "+stringArrayFiles.Length,"import",percentUpdated+"%",percentUpdated);
				if(odProgressExtended.IsPauseOrCancel()) {
					return Lans.g(odProgressExtended.LanThis,"Import canceled by user.");
				}
				string pathToFileSource=stringArrayFiles[i];
				string pathToFileDestination=ODFileUtils.CombinePaths(pathToArchiveDir,Path.GetFileName(stringArrayFiles[i]));
				try {
					File.Move(pathToFileSource,pathToFileDestination);
				}
				catch(Exception ex) {
					ex.DoNothing();//OK to continue, since ProcessIncomingReport() above saved the raw report into the etrans table.
					listFilesFailedToMove.Add(pathToFileSource);
					continue;//Skip current report file and leave in folder to processing later.
				}
				try {
					Etranss.ProcessIncomingReport(
						File.GetCreationTime(pathToFileDestination),
						clearinghouseClin.HqClearinghouseNum,
						File.ReadAllText(pathToFileDestination),
						Security.CurUser.UserNum);
				}
				catch(Exception ex) {
					ex.DoNothing();
					listFilesFailedToImport.Add(pathToFileSource);
					File.Move(pathToFileDestination,pathToFileSource);//Move file back so that the archived folder only contains succesfully processed reports.
				}
			}string errorMessage="";
			if(listFilesFailedToMove.Count>0) {
				errorMessage=Lans.g("FormClaimReports","Failed to move the following files to archive folder due to permission issues or duplicate file names:")
					+"\r\n"+string.Join(",\r\n",listFilesFailedToMove);
			}
			if(listFilesFailedToImport.Count>0) {
				errorMessage+="\r\n\r\n"+Lans.g("FormClaimReports","Failed to process following files due to malformed data:")
					+"\r\n"+string.Join(",\r\n",listFilesFailedToImport);
			}
			return errorMessage;
		}

		///<summary></summary>
		public static string RetrieveAndImport(Clearinghouse clearinghouse,bool isAutomaticMode,IODProgressExtended odProgressExtended=null
			,bool isTimeToRetrieve=false) 
		{
			odProgressExtended=odProgressExtended??new ODProgressExtendedNull();
			string errorMessage="";
			bool doRetrieveReports=isTimeToRetrieve || (!isAutomaticMode && IsTimeToRetrieveReports(isAutomaticMode,out errorMessage,odProgressExtended));			
			if(doRetrieveReports) {//Timer interval OK.  Now we can retrieve the reports from web services.
				if(!isAutomaticMode) {
					Prefs.UpdateDateT(PrefName.ClaimReportReceiveLastDateTime,DateTime.Now);
				}
				errorMessage=RetrieveReports(clearinghouse,isAutomaticMode,odProgressExtended);
				if(errorMessage!="") {
					odProgressExtended.UpdateProgress(Lans.g(odProgressExtended.LanThis,"Error getting reports, attempting to import manually downloaded reports."));
				}
				odProgressExtended.UpdateProgress(Lans.g(odProgressExtended.LanThis,"Report retrieval successful. Attempting to import."));
				//Don't return yet even if there was an error. This is so that Open Dental will automatically import reports that have been manually
				//downloaded to the Reports folder.
			}
			if(isAutomaticMode && clearinghouse.ResponsePath.Trim()=="") {
				return "";//The user opened FormClaimsSend, or FormOpenDental called this function automatically.
			}
			if(odProgressExtended.IsPauseOrCancel()) {
				odProgressExtended.UpdateProgress(Lans.g(odProgressExtended.LanThis,"Canceled by user."));
				return errorMessage;
			}
			string importErrors=ImportReportFiles(clearinghouse,odProgressExtended);
			if(!string.IsNullOrWhiteSpace(importErrors)) {
				if(string.IsNullOrWhiteSpace(errorMessage)) {
					errorMessage=importErrors;
					odProgressExtended.UpdateProgress(Lans.g(odProgressExtended.LanThis,"Error importing."));
				}
				else {
					errorMessage+="\r\n"+importErrors;
				}
			}
			if(string.IsNullOrWhiteSpace(errorMessage) && string.IsNullOrWhiteSpace(importErrors)) {
				odProgressExtended.UpdateProgress(Lans.g(odProgressExtended.LanThis,"Import successful."));
			}
			return errorMessage;
		}

		///<summary>Returns and error message to display to the user if default clearinghouses are not set up; Otherwise, empty string.</summary>
		public static string CheckClearinghouseDefaults() {
			if(PrefC.GetLong(PrefName.ClearinghouseDefaultDent)==0) {
				return Lans.g("ContrAccount","No default dental clearinghouse defined.");
			}
			if(PrefC.GetBool(PrefName.ShowFeatureMedicalInsurance) && PrefC.GetLong(PrefName.ClearinghouseDefaultMed)==0) {
				return Lans.g("ContrAccount","No default medical clearinghouse defined.");
			}
			return "";
		}
		
		///<summary>Calling methods will typically pass in all non-HQ clearinghouses (overrides).
		///This method will "sync" any clearinghouses that are associated to the same HQ Clearinghouse and Clinic with the values from clearinghouseNew.
		///This method is only used in FormClearinghouseEdit.cs to defend against DB's with duplicate override rows.
		///Loops through the list of overrides and updates each clearinghouse override associated to clearinghouseNew.ClinicNum.
		///This was put into a centralized method for unit testing purposes. For more details see jobnum 11387.</summary>
		///<param name="listClearinghousesOverrides">A list of all non-HQ clearinghouses which this method will manipulate (Clearinghouse overrides).</param>
		///<param name="clearinghouseNew">The new Clearinghouse override object.  ClinicNum will be used from this clearinghouse.</param>
		public static void SyncOverridesForClinic(ref List<Clearinghouse> listClearinghousesOverrides,Clearinghouse clearinghouseNew) {
			//No need to check MiddleTierRole; no call to db and uses an out parameter.
			if(clearinghouseNew.ClinicNum==0) {
				return;//Nothing to do when the ClinicNum associated to clearinghouseNew is 0.
			}
			//Get all clearinghouse overrides that are associated to the same HQ clearinghouse and clinic.
			for(int i=0;i<listClearinghousesOverrides.Count;i++) {
				if(listClearinghousesOverrides[i].HqClearinghouseNum!=clearinghouseNew.HqClearinghouseNum
					|| listClearinghousesOverrides[i].ClinicNum!=clearinghouseNew.ClinicNum)
				{
					continue;
				}
				//Take all of the values from clearinghouseNew and put them into the current clearinghouseOverride (sync them).
				//Make sure to preserve the ClearinghouseNum of the override before syncing the values.
				long clearinghouseNumOverride=listClearinghousesOverrides[i].ClearinghouseNum;
				listClearinghousesOverrides[i]=clearinghouseNew.Copy();
				listClearinghousesOverrides[i].ClearinghouseNum=clearinghouseNumOverride;
			}
		}

		///<summary>Some clearinghouses do not work in WEB mode.</summary>
		public static bool IsDisabledForWeb(Clearinghouse clearinghouse) {
			return IsDisabledForWeb(clearinghouse.Eformat,clearinghouse.CommBridge);
		}

		///<summary>Some clearinghouses do not work in WEB mode.</summary>
		public static bool IsDisabledForWeb(ElectronicClaimFormat electronicClaimFormat,EclaimsCommBridge eclaimsCommBridge) {
			if(electronicClaimFormat.In(ElectronicClaimFormat.Renaissance,ElectronicClaimFormat.Canadian)
				|| eclaimsCommBridge==EclaimsCommBridge.WebMD) {
				return true;
			}
			return false;
		}


		#endregion
	}
}