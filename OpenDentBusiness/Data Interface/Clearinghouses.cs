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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_clearinghouseCache.FillCacheFromTable(table);
				return table;
			}
			return _clearinghouseCache.GetTableFromCache(doRefreshCache);
		}

		#endregion

		#region Get Methods

		///<summary>Gets all clearinghouses for the specified clinic.  Returns an empty list if clinicNum=0.  
		///Use the cache if you want all HQ Clearinghouses.</summary>
		public static List<Clearinghouse> GetAllNonHq() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Clearinghouse>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM clearinghouse WHERE ClinicNum!=0 ORDER BY Description";
			List<Clearinghouse> clearinghouseRetVal=Crud.ClearinghouseCrud.SelectMany(command);
			clearinghouseRetVal.ForEach(x => x.Password=GetRevealPassword(x.Password));
			return clearinghouseRetVal;
		}

		///<summary>Returns the HQ-level default clearinghouse.  You must manually override using OverrideFields if needed.  If no default present, returns null.</summary>
		public static Clearinghouse GetDefaultEligibility() {
			//No need to check RemotingRole; no call to db.
			return GetClearinghouse(PrefC.GetLong(PrefName.ClearinghouseDefaultEligibility));
		}

		///<summary>Gets the last batch number from db for the HQ version of this clearinghouseClin and increments it by one.
		///Then saves the new value to db and returns it.  So even if the new value is not used for some reason, it will have already been incremented.
		///Remember that LastBatchNumber is never accurate with local data in memory.</summary>
		public static int GetNextBatchNumber(Clearinghouse clearinghouseClin){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),clearinghouseClin);
			}
			//get last batch number
			string command="SELECT LastBatchNumber FROM clearinghouse "
				+"WHERE ClearinghouseNum = "+POut.Long(clearinghouseClin.HqClearinghouseNum);
			DataTable table=Db.GetTable(command);
			int batchNum=PIn.Int(table.Rows[0][0].ToString());
			//and increment it by one
			if(clearinghouseClin.Eformat==ElectronicClaimFormat.Canadian){
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
				+" WHERE ClearinghouseNum = "+POut.Long(clearinghouseClin.HqClearinghouseNum);
			Db.NonQ(command);
			return batchNum;
		}

		///<summary>Returns the clearinghouseNum for claims for the supplied payorID.  If the payorID was not entered or if no default was set, then 0 is returned.</summary>
		public static long AutomateClearinghouseHqSelection(string payorID,EnumClaimMedType medType){
			//No need to check RemotingRole; no call to db.
			//payorID can be blank.  For example, Renaissance does not require payorID.
			Clearinghouse clearinghouseHq=null;
			if(medType==EnumClaimMedType.Dental){
				if(PrefC.GetLong(PrefName.ClearinghouseDefaultDent)==0){
					return 0;
				}
				clearinghouseHq=GetClearinghouse(PrefC.GetLong(PrefName.ClearinghouseDefaultDent));
			}
			if(medType==EnumClaimMedType.Medical || medType==EnumClaimMedType.Institutional){
				if(PrefC.GetLong(PrefName.ClearinghouseDefaultMed)==0){
					//No default set, substituting emdeon medical otherwise first medical clearinghouse.
					List<Clearinghouse> listClearingHouses=GetDeepCopy(false);
					clearinghouseHq=listClearingHouses.FirstOrDefault(x => x.CommBridge==EclaimsCommBridge.EmdeonMedical&&x.HqClearinghouseNum==x.ClearinghouseNum);
					if(clearinghouseHq==null) {
						clearinghouseHq=listClearingHouses.FirstOrDefault(x => x.Eformat==ElectronicClaimFormat.x837_5010_med_inst&&x.HqClearinghouseNum==x.ClearinghouseNum);
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
			Clearinghouse clearingHouseOverride=GetClearinghouseByPayorID(payorID);
			if(clearingHouseOverride!=null) {//an override exists for this payorID
				if(clearingHouseOverride.Eformat==ElectronicClaimFormat.x837D_4010 || clearingHouseOverride.Eformat==ElectronicClaimFormat.x837D_5010_dental
					|| clearingHouseOverride.Eformat==ElectronicClaimFormat.Canadian || clearingHouseOverride.Eformat==ElectronicClaimFormat.Ramq)
				{//all dental formats
					if(medType==EnumClaimMedType.Dental){//med type matches
						return clearingHouseOverride.ClearinghouseNum;
					}
				}
				if(clearingHouseOverride.Eformat==ElectronicClaimFormat.x837_5010_med_inst){
					if(medType==EnumClaimMedType.Medical || medType==EnumClaimMedType.Institutional) {//med type matches
						return clearingHouseOverride.ClearinghouseNum;
					}
				}
			}
			//no override, so just return the default.
			return clearinghouseHq.ClearinghouseNum;
		}

		///<summary>Returns the first clearinghouse that is associated to the corresponding payorID passed in.  Returns null if no match found.</summary>
		private static Clearinghouse GetClearinghouseByPayorID(string payorID) {
			//No need to check RemotingRole; no call to db.
			Clearinghouse clearinghouse=null;
			if(string.IsNullOrEmpty(payorID)) {
				return clearinghouse;
			}
			//Take the entire clearinghouse cache (which is typically small) and flatten it into a dictionary by payor ID to clearinhouse.
			//Each clearinghouse can be associated to multiple payor IDs (comma delimited string) so that must be broken down first.
			GetDeepCopy().Select(x => new {
				listPayorToHouse=x.Payors.Split(',').ToList()//Take every clearinghouse's payors and split them up (comma delimited string per house).
					.Select(y => new { payor=y,house=x })      //Make a new object that ties the clearinghouse and payorID together (List<List<payor,house>>)
			}).SelectMany(x => x.listPayorToHouse)         //Flatten the list of lists to make one long list of new objects (List<payor,house>)
			.GroupBy(x => x.payor)                         //Group these new objects by the payor (if there are any duplicates we'll grab first in list)
			.ToDictionary(x => x.Key,x => x.First().house) //Make a dictionary out of the new objects where Key: payor Value: the first house
			.TryGetValue(payorID,out clearinghouse);       //Try and find the corresponding clearinghouse via the payorID passed in.
			return clearinghouse;//Can return null and that is just fine.
		}

		///<summary>Returns the HQ-level default clearinghouse.  You must manually override using OverrideFields if needed.  If no default present, returns null.</summary>
		public static Clearinghouse GetDefaultDental(){
			//No need to check RemotingRole; no call to db.
			return GetClearinghouse(PrefC.GetLong(PrefName.ClearinghouseDefaultDent));
		}

		///<summary>Gets an HQ clearinghouse from cache.  Will return null if invalid.</summary>
		public static Clearinghouse GetClearinghouse(long clearinghouseNum){
			//No need to check RemotingRole; no call to db.
			return GetFirstOrDefault(x => x.ClearinghouseNum==clearinghouseNum);
		}

		///<summary>Gets revealed password for a clearinghouse password.</summary>
		public static string GetRevealPassword(string concealPassword) {
			string revealPassword="";
			CDT.Class1.RevealClearinghouse(concealPassword,out revealPassword);
			return revealPassword;
		}

		///<summary>Returns the clinic-level clearinghouse for the passed in Clearinghouse.  Usually used in conjunction with ReplaceFields().
		///Can return null.</summary>
		public static Clearinghouse GetForClinic(Clearinghouse clearinghouseHq,long clinicNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Clearinghouse>(MethodBase.GetCurrentMethod(),clearinghouseHq,clinicNum);
			}
			if(clinicNum==0) { //HQ
				return null;
			}
			string command="SELECT * FROM clearinghouse WHERE HqClearinghouseNum="+clearinghouseHq.ClearinghouseNum+" AND ClinicNum="+clinicNum;
			Clearinghouse clearinghouseRetVal=Crud.ClearinghouseCrud.SelectOne(command);
			if(clearinghouseRetVal!=null) {
				clearinghouseRetVal.Password=GetRevealPassword(clearinghouseRetVal.Password);
			}
			return clearinghouseRetVal;
		}
		#endregion

		#region Insert
		///<summary>Inserts one clearinghouse into the database.  Use this if you know that your clearinghouse will be inserted at the HQ-level.</summary>
		public static long Insert(Clearinghouse clearinghouse) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clearinghouse);
				return;
			}
			Crud.ClearinghouseCrud.Update(clearinghouse);
		}

		public static void Update(Clearinghouse clearinghouse,Clearinghouse oldClearinghouse) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),clearinghouse,oldClearinghouse);
				return;
			}
			Crud.ClearinghouseCrud.Update(clearinghouse,oldClearinghouse);
		}

		///<summary>Syncs a given list of clinic-level clearinghouses to a list of old clinic-level clearinghouses.</summary>
		public static void Sync(List<Clearinghouse> listClearinghouseNew,List<Clearinghouse> listClearinghouseOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listClearinghouseNew,listClearinghouseOld);
				return;
			}
			Crud.ClearinghouseCrud.Sync(listClearinghouseNew,listClearinghouseOld);
		}
		#endregion

		#region Delete
		///<summary>Deletes the passed-in Hq clearinghouse for all clinics.  Only pass in clearinghouses with ClinicNum==0.</summary>
		public static void Delete(Clearinghouse clearinghouseHq) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			//No need to check RemotingRole; no call to db.
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
			//No need to check RemotingRole; no call to db.
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
			string errMsg;
			bool isTimeToRetrieve=IsTimeToRetrieveReports(true,out errMsg);
			if(isTimeToRetrieve) {
				Prefs.UpdateDateT(PrefName.ClaimReportReceiveLastDateTime,DateTime.Now);
			}
			List<Clearinghouse> listClearinghousesHq=GetDeepCopy();
			long defaultClearingHouseNum=PrefC.GetLong(PrefName.ClearinghouseDefaultDent);
			for(int i=0;i<listClearinghousesHq.Count;i++) {
				Clearinghouse clearinghouseHq=listClearinghousesHq[i];
				Clearinghouse clearinghouseClin;
				for(int j=0;j<listClinicNums.Count;j++) {
					clearinghouseClin=OverrideFields(clearinghouseHq,listClinicNums[j]);
					RetrieveReportsAutomaticHelper(clearinghouseClin,clearinghouseHq,defaultClearingHouseNum,isTimeToRetrieve);
				}
			}

		}

		///<summary>Returns true if it is time to retrieve reports.</summary>
		private static bool IsTimeToRetrieveReports(bool isAutomaticMode,out string errorMessage,IODProgressExtended progress=null) {
			progress=progress??new ODProgressExtendedNull();
			DateTime timeLastReport=PIn.DateT(PrefC.GetStringNoCache(PrefName.ClaimReportReceiveLastDateTime));
			double timeReceiveInternal=PIn.Double(PrefC.GetStringNoCache(PrefName.ClaimReportReceiveInterval));//Interval in minutes.
			DateTime timeToRecieve=DateTime.Now.Date+PrefC.GetDateT(PrefName.ClaimReportReceiveTime).TimeOfDay;
			double timeDiff=DateTime.Now.Subtract(timeLastReport).TotalMinutes;
			errorMessage="";
			if(isAutomaticMode) {
				if(timeReceiveInternal!=0) { //preference is set instead of pref for specific time. 
					if(timeDiff < timeReceiveInternal) {
						//Automatically retrieving reports from this computer and the report interval has not passed yet.
						return false;
					}
				}
				else {//pref is set for specific time, not interval
					if(DateTime.Now.TimeOfDay < timeToRecieve.TimeOfDay //We haven't reach to the time to retrieve
						|| timeLastReport.Date==DateTime.Today)//Or we have already retrieved today
					{
						//Automatically retrieving reports and the time has not come to pass yet
						return false;
					}
				}
			}
			else if(timeDiff < 1) {
				//When the user presses the Get Reports button manually we allow them to get reports up to once per minute
				errorMessage=Lans.g(progress.LanThis,"Reports can only be retrieved once per minute.");
				progress.UpdateProgress(Lans.g(progress.LanThis,"Reports can only be retrieved once per minute. Attempting to import manually downloaded reports."));
				return false;
			}
			return true;
		}

		private static void RetrieveReportsAutomaticHelper(Clearinghouse clearinghouseClin,Clearinghouse clearinghouseHq,long defaultClearingHouseNum
			,bool isTimeToRetrieve)
		{
			if(!Directory.Exists(clearinghouseClin.ResponsePath)) {
				return;
			}
			if(clearinghouseHq.ClearinghouseNum==defaultClearingHouseNum) {//If it's the default dental clearinghouse
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
				List<Provider> listProvs=Providers.GetDeepCopy(true);
				List<string> listOfficeNums=new List<string>();
				for(int j=0;j<listProvs.Count;j++) {//Get all unique office numbers from the providers.
					if(!listProvs[j].IsCDAnet || listProvs[j].NationalProvID=="" || listProvs[j].CanadianOfficeNum=="") {
						continue;
					}
					if(!listOfficeNums.Contains(listProvs[j].CanadianOfficeNum)) {//Ignore duplicate office numbers.
						listOfficeNums.Add(listProvs[j].CanadianOfficeNum);
						try {
							clearinghouseHq=Eclaims.Canadian.GetCanadianClearinghouseHq(null);
							clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
							//Run both version 02 and version 04 reports for all carriers and all networks.
							Eclaims.CanadianOutput.GetOutstandingForDefault(listProvs[j]);
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

		private static string RetrieveReports(Clearinghouse clearinghouseClin,bool isAutomaticMode,IODProgressExtended progress=null) {
			progress=progress??new ODProgressExtendedNull();
			progress.UpdateProgress(Lans.g(progress.LanThis,"Beginning report retrieval..."),"reports","0%");
			if(progress.IsPauseOrCancel()) {
				return Lans.g(progress.LanThis,"Process canceled by user.");
			}
			StringBuilder errorMessage=new StringBuilder();
			if(Plugins.HookMethod(null,"Clearinghouses.RetrieveReports_afterProgressPauseOrCancel",clearinghouseClin,errorMessage,progress)) {
				return errorMessage.ToString();
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
				if(!WebMD.Launch(clearinghouseClin,0,isAutomaticMode,progress)) {
					return Lans.g("FormClaimReports","Error retrieving.")+"\r\n"+WebMD.ErrorMessage;
				}
			}
			else if(clearinghouseClin.CommBridge==EclaimsCommBridge.BCBSGA) {
				if(!BCBSGA.Retrieve(clearinghouseClin,true,new TerminalConnector(),progress)) {
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
				else if(!ClaimConnect.Retrieve(clearinghouseClin,progress)) {
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
				if(!MercuryDE.Launch(clearinghouseClin,0,progress)) { 
					return Lans.g("FormClaimReports","Error retrieving.")+"\r\n"+MercuryDE.ErrorMessage;
				}
			}
			else if(clearinghouseClin.CommBridge==EclaimsCommBridge.EmdeonMedical) {
				if(!EmdeonMedical.Retrieve(clearinghouseClin,progress)) {
					return Lans.g("FormClaimReports","Error retrieving.")+"\r\n"+EmdeonMedical.ErrorMessage;
				}
			}
			else if(clearinghouseClin.CommBridge==EclaimsCommBridge.DentiCal) {
				if(!DentiCal.Launch(clearinghouseClin,0,progress)) {
					return Lans.g("FormClaimReports","Error retrieving.")+"\r\n"+DentiCal.ErrorMessage;
				}
			}
			else if(clearinghouseClin.CommBridge==EclaimsCommBridge.EDS) {
				List<string> listEdsErrors=new List<string>();
				if(!EDS.Retrieve277s(clearinghouseClin,progress)) {
					listEdsErrors.Add(Lans.g("FormClaimReports","Error retrieving.")+"\r\n"+EDS.ErrorMessage);
				}
				if(!EDS.Retrieve835s(clearinghouseClin,progress)) {
					listEdsErrors.Add(Lans.g("FormClaimReports","Error retrieving.")+"\r\n"+EDS.ErrorMessage);
				}
				if(listEdsErrors.Count>0) {
					return string.Join("\r\n",listEdsErrors);
				}
			}
			return "";
		}

		///<summary>Takes any files found in the reports folder for the clearinghouse, and imports them into the database.
		///Moves the original file into an Archive sub folder.
		///Returns a string with any errors that occurred.</summary>
		private static string ImportReportFiles(Clearinghouse clearinghouseClin,IODProgressExtended progress=null) { //uses clinic-level clearinghouse where necessary.
			progress=progress??new ODProgressExtendedNull();
			if(!Directory.Exists(clearinghouseClin.ResponsePath)) {
				return Lans.g("FormClaimReports","Report directory does not exist")+": "+clearinghouseClin.ResponsePath;
			}
			if(clearinghouseClin.Eformat==ElectronicClaimFormat.Canadian||clearinghouseClin.Eformat==ElectronicClaimFormat.Ramq) {
				//the report path is shared with many other important files.  Do not process anything.  Comm is synchronous only.
				return "";
			}
			progress.UpdateProgress(Lans.g(progress.LanThis,"Reading download files"),"reports","55%",55);
			if(progress.IsPauseOrCancel()) {
				return Lans.g(progress.LanThis,"Import canceled by user.");
			}
			string[] files=null;
			string archiveDir;
			try {
				files=Directory.GetFiles(clearinghouseClin.ResponsePath);
				archiveDir=ODFileUtils.CombinePaths(clearinghouseClin.ResponsePath,"Archive"+"_"+DateTime.Now.Year.ToString());
				if(!Directory.Exists(archiveDir)) {
					Directory.CreateDirectory(archiveDir);
				}
			}
			catch(UnauthorizedAccessException ex) {
				ex.DoNothing();
				return Lans.g("FormClaimReports","Access to the Report Path is denied.  Try running as administrator or contact your network administrator.");
			}
			List<string> listFailedFiles=new List<string>();
			progress.UpdateProgress(Lans.g(progress.LanThis,"Files read."));
			progress.UpdateProgress(Lans.g(progress.LanThis,"Importing files"),"reports","83%",83);
			if(files.Length>0) {
				progress.UpdateProgressDetailed(Lans.g(progress.LanThis,"Importing"),tagString:"import");//add a new progress bar for imports if there are any to import
			}
			else {
				progress.UpdateProgress(Lans.g(progress.LanThis,"No files to import."));
			}
			for(int i=0;i<files.Length;i++) {
				int percent=(i/files.Length)*100;
				progress.UpdateProgress(Lans.g(progress.LanThis,"Importing")+" "+i+" / "+files.Length,"import",percent+"%",percent);
				if(progress.IsPauseOrCancel()) {
					return Lans.g(progress.LanThis,"Import canceled by user.");
				}
				string fileSource=files[i];
				string fileDestination=ODFileUtils.CombinePaths(archiveDir,Path.GetFileName(files[i]));
				try {
					File.Move(fileSource,fileDestination);
				}
				catch(Exception ex) {
					ex.DoNothing();//OK to continue, since ProcessIncomingReport() above saved the raw report into the etrans table.
					listFailedFiles.Add(fileSource);
					continue;//Skip current report file and leave in folder to processing later.
				}
				try {
					Etranss.ProcessIncomingReport(
						File.GetCreationTime(fileDestination),
						clearinghouseClin.HqClearinghouseNum,
						File.ReadAllText(fileDestination),
						Security.CurUser.UserNum);
				}
				catch(Exception ex) {
					ex.DoNothing();
					listFailedFiles.Add(fileSource);
					File.Move(fileDestination,fileSource);//Move file back so that the archived folder only contains succesfully processed reports.
				}
			}
			if(listFailedFiles.Count>0) {
				return Lans.g("FormClaimReports","Failed to process the following files due to permission issues or malformed data")
					+":\r\n"+string.Join(",\r\n",listFailedFiles);
			}
			return "";
		}

		///<summary></summary>
		public static string RetrieveAndImport(Clearinghouse clearinghouseClin,bool isAutomaticMode,IODProgressExtended progress=null
			,bool isTimeToRetrieve=false) 
		{
			progress=progress??new ODProgressExtendedNull();
			string errorMessage="";
			bool doRetrieveReports=isTimeToRetrieve || (!isAutomaticMode && IsTimeToRetrieveReports(isAutomaticMode,out errorMessage,progress));			
			if(doRetrieveReports) {//Timer interval OK.  Now we can retrieve the reports from web services.
				if(!isAutomaticMode) {
					Prefs.UpdateDateT(PrefName.ClaimReportReceiveLastDateTime,DateTime.Now);
				}
				errorMessage=RetrieveReports(clearinghouseClin,isAutomaticMode,progress);
				if(errorMessage!="") {
					progress.UpdateProgress(Lans.g(progress.LanThis,"Error getting reports, attempting to import manually downloaded reports."));
				}
				progress.UpdateProgress(Lans.g(progress.LanThis,"Report retrieval successful. Attempting to import."));
				//Don't return yet even if there was an error. This is so that Open Dental will automatically import reports that have been manually
				//downloaded to the Reports folder.
			}
			if(isAutomaticMode && clearinghouseClin.ResponsePath.Trim()=="") {
				return "";//The user opened FormClaimsSend, or FormOpenDental called this function automatically.
			}
			if(progress.IsPauseOrCancel()) {
				progress.UpdateProgress(Lans.g(progress.LanThis,"Canceled by user."));
				return errorMessage;
			}
			string importErrors=ImportReportFiles(clearinghouseClin,progress);
			if(!string.IsNullOrWhiteSpace(importErrors)) {
				if(string.IsNullOrWhiteSpace(errorMessage)) {
					errorMessage=importErrors;
					progress.UpdateProgress(Lans.g(progress.LanThis,"Error importing."));
				}
				else {
					errorMessage+="\r\n"+importErrors;
				}
			}
			if(string.IsNullOrWhiteSpace(errorMessage) && string.IsNullOrWhiteSpace(importErrors)) {
				progress.UpdateProgress(Lans.g(progress.LanThis,"Import successful."));
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
		///<param name="listClearinghouseOverrides">A list of all non-HQ clearinghouses which this method will manipulate (Clearinghouse overrides).</param>
		///<param name="clearinghouseNew">The new Clearinghouse override object.  ClinicNum will be used from this clearinghouse.</param>
		public static void SyncOverridesForClinic(ref List<Clearinghouse> listClearinghouseOverrides,Clearinghouse clearinghouseNew) {
			//No need to check RemotingRole; no call to db and uses an out parameter.
			if(clearinghouseNew.ClinicNum==0) {
				return;//Nothing to do when the ClinicNum associated to clearinghouseNew is 0.
			}
			//Get all clearinghouse overrides that are associated to the same HQ clearinghouse and clinic.
			for(int i=0;i<listClearinghouseOverrides.Count;i++) {
				if(listClearinghouseOverrides[i].HqClearinghouseNum!=clearinghouseNew.HqClearinghouseNum
					|| listClearinghouseOverrides[i].ClinicNum!=clearinghouseNew.ClinicNum)
				{
					continue;
				}
				//Take all of the values from clearinghouseNew and put them into the current clearinghouseOverride (sync them).
				//Make sure to preserve the ClearinghouseNum of the override before syncing the values.
				long clearinghouseNumOverride=listClearinghouseOverrides[i].ClearinghouseNum;
				listClearinghouseOverrides[i]=clearinghouseNew.Copy();
				listClearinghouseOverrides[i].ClearinghouseNum=clearinghouseNumOverride;
			}
		}

		///<summary>Some clearinghouses do not work in WEB mode.</summary>
		public static bool IsDisabledForWeb(Clearinghouse clearinghouse) {
			return IsDisabledForWeb(clearinghouse.Eformat,clearinghouse.CommBridge);
		}

		///<summary>Some clearinghouses do not work in WEB mode.</summary>
		public static bool IsDisabledForWeb(ElectronicClaimFormat eformat,EclaimsCommBridge commBridge) {
			if(ListTools.In(eformat,ElectronicClaimFormat.Renaissance,ElectronicClaimFormat.Canadian)
				|| commBridge==EclaimsCommBridge.WebMD) {
				return true;
			}
			return false;
		}


		#endregion
	}
}