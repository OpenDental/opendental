using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBase;
using OpenDentalCloud;
using OpenDentalCloud.Core;

namespace OpenDentBusiness{
	///<summary></summary>
	public class TsiTransLogs{
		/// <summary>Adjustment types that are excluded from being sent to TransWorld and from balance calculations.</summary>
		public static List<string> ExcludeAdjustTypes {
			get {
				return new List<string> { 
					ProgramProperties.PropertyDescs.TransWorld.SyncExcludePosAdjType,
					ProgramProperties.PropertyDescs.TransWorld.SyncExcludeNegAdjType 
				};
			}
		}

		#region Get Methods

		///<summary>Returns all tsitranslogs for the patients in listPatNums.  Returns empty list if listPatNums is empty or null.</summary>
		public static List<TsiTransLog> SelectMany(List<long> listPatNums){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TsiTransLog>>(MethodBase.GetCurrentMethod(),listPatNums);
			}
			if(listPatNums==null || listPatNums.Count<1) {
				return new List<TsiTransLog>();
			}
			string command="SELECT * FROM tsitranslog "
				+"WHERE PatNum IN ("+string.Join(",",listPatNums.Select(x => POut.Long(x)))+")";
			return Crud.TsiTransLogCrud.SelectMany(command);
		}

		///<summary>Returns all tsitranslogs for all patients.  Used in FormTsiHistory only.</summary>
		public static List<TsiTransLog> GetAll(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<TsiTransLog>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM tsitranslog ORDER BY TransDateTime DESC";
			return Crud.TsiTransLogCrud.SelectMany(command);
		}

		///<summary>Returns a list of PatNums for guars who have a TsiTransLog with type SS (suspend) less than 50 days ago who don't have a TsiTransLog
		///with type CN (cancel), PF (paid in full), PT (paid in full, thank you), or PL (placement) with a more recent date, since this would change the
		///account status from suspended to either closed/canceled or if the more recent message had type PL (placement) back to active.</summary>
		public static List<long> GetSuspendedGuarNums() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod());
			}
			int[] arrayStatusTransTypes=new[] { (int)TsiTransType.SS,(int)TsiTransType.CN,(int)TsiTransType.RI,(int)TsiTransType.PF,(int)TsiTransType.PT,(int)TsiTransType.PL };
			string command="SELECT DISTINCT tsitranslog.PatNum "
				+"FROM tsitranslog "
				+"INNER JOIN ("
					+"SELECT PatNum,MAX(TransDateTime) transDateTime "
					+"FROM tsitranslog "
					+"WHERE TransType IN("+string.Join(",",arrayStatusTransTypes)+") "
					+"AND TransDateTime>"+POut.DateT(DateTime.Now.AddDays(-50))+" "
					+"GROUP BY PatNum"
				+") mostRecentTrans ON tsitranslog.PatNum=mostRecentTrans.PatNum "
					+"AND tsitranslog.TransDateTime=mostRecentTrans.transDateTime "
				+"WHERE tsitranslog.TransType="+(int)TsiTransType.SS;
			return Db.GetListLong(command);
		}

		public static bool IsGuarSuspended(long guarNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),guarNum);
			}
			int[] arrayStatusTransTypes=new[] { (int)TsiTransType.SS,(int)TsiTransType.CN,(int)TsiTransType.RI,(int)TsiTransType.PF,(int)TsiTransType.PT,(int)TsiTransType.PL };
			string command="SELECT (CASE WHEN tsitranslog.TransType="+(int)TsiTransType.SS+" THEN 1 ELSE 0 END) isGuarSuspended "
				+"FROM tsitranslog "
				+"INNER JOIN ("
					+"SELECT PatNum,MAX(TransDateTime) transDateTime "
					+"FROM tsitranslog "
					+"WHERE PatNum="+POut.Long(guarNum)+" " 
					+"AND TransType IN("+string.Join(",",arrayStatusTransTypes)+") "
					+"AND TransDateTime>"+POut.DateT(DateTime.Now.AddDays(-50))+" "
					+"GROUP BY PatNum"
				+") mostRecentLog ON tsitranslog.PatNum=mostRecentLog.PatNum AND tsitranslog.TransDateTime=mostRecentLog.transDateTime";
			return PIn.Bool(Db.GetScalar(command));
		}

		#endregion Get Methods

		#region Insert

		public static long Insert(TsiTransLog tsiTransLog) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				tsiTransLog.TsiTransLogNum=Meth.GetLong(MethodBase.GetCurrentMethod(),tsiTransLog);
				return tsiTransLog.TsiTransLogNum;
			}
			return Crud.TsiTransLogCrud.Insert(tsiTransLog);
		}

		public static void InsertMany(List<TsiTransLog> listTsiTransLogs) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listTsiTransLogs);
				return;
			}
			Crud.TsiTransLogCrud.InsertMany(listTsiTransLogs);
		}

		private static void InsertTsiLogsForAdjustment(long patGuar,Adjustment adj,string msgText,TsiTransType transType) {
			//insert tsitranslog for this transaction so the ODService won't send it to Transworld.  _isTsiAdj means Transworld received a payment on
			//behalf of this guar and took a percentage and send the rest to the office for the account.  This will result in a payment being entered
			//into the account, having been received from Transworld, and an adjustment to account for Transorld's cut.
			PatAging patAgingCur=Patients.GetAgingListFromGuarNums(new List<long>() { patGuar }).FirstOrDefault();//should only ever be 1
			if(patAgingCur==null) {
				return;
			}
			double offsetAmt=adj.AdjAmt-patAgingCur.ListTsiLogs.FindAll(x => x.FKeyType==TsiFKeyType.Adjustment && x.FKey==adj.AdjNum).Sum(x => x.TransAmt);
			if(CompareDouble.IsZero(offsetAmt)) {
				return;
			}
			double balFromMsgs=GetBalFromMsgs(patAgingCur);
			if(CompareDouble.IsZero(balFromMsgs)) {
				return;
			}
			Insert(new TsiTransLog() {
				PatNum=patAgingCur.PatNum,
				UserNum=Security.CurUser.UserNum,
				TransType=transType,
				//TransDateTime=DateTime.Now,//set on insert, not editable by user
				//ServiceType=TsiServiceType.Accelerator,//only valid for placement msgs
				//ServiceCode=TsiServiceCode.Diplomatic,//only valid for placement msgs
				ClientId=patAgingCur.ListTsiLogs.FirstOrDefault()?.ClientId??"",//can be blank, not used since this isn't really sent to Transworld
				TransAmt=offsetAmt,
				AccountBalance=balFromMsgs+(transType==TsiTransType.Excluded?0:offsetAmt),
				FKeyType=TsiFKeyType.Adjustment,
				FKey=adj.AdjNum,
				RawMsgText=msgText,
				//TransJson=""//only valid for placement msgs
				ClinicNum=(PrefC.HasClinicsEnabled?patAgingCur.ClinicNum:0)
			});
		}

		/// <summary>Inserts a TsiTransLog for the adjustment if necessary.</summary>
		public static void CheckAndInsertLogsIfAdjTypeExcluded(Adjustment adj,bool isFromTsi=false) {
			Program progCur=Programs.GetCur(ProgramName.Transworld);
			if(progCur==null || !progCur.Enabled) {
				return;
			}
			Patient guar=Patients.GetGuarForPat(adj.PatNum);
			if(guar==null || !IsTransworldEnabled(guar.ClinicNum) || !Patients.IsGuarCollections(guar.PatNum)) {
				return;
			}
			string msgText="This was not a message sent to Transworld.  This adjustment was entered due to a payment received from Transworld.";
			TsiTransType typeCur=TsiTransType.None;
			Dictionary<long,List<ProgramProperty>> dictClinicProps=ProgramProperties
				.GetWhere(x => x.ProgramNum==progCur.ProgramNum && ListTools.In(x.PropertyDesc,ExcludeAdjustTypes) && ListTools.In(x.ClinicNum,0,guar.ClinicNum))
				.GroupBy(x => x.ClinicNum)
				.ToDictionary(x => x.Key,x => x.ToList());
			//use guar's clinic if clinics are enabled and props for that clinic exist, otherwise use ClinicNum 0
			long clinicNum=(PrefC.HasClinicsEnabled && dictClinicProps.ContainsKey(guar.ClinicNum))?guar.ClinicNum:0;
			if(!dictClinicProps.TryGetValue(clinicNum,out List<ProgramProperty> listPropsCur)//should always be props for ClinicNum 0
				|| listPropsCur.All(x => PIn.Long(x.PropertyValue,false)!=adj.AdjType))
			{
				if(!isFromTsi) {
					return;//if this adjustment is not an excluded type and not from TSI, return
				}
				//if this adjustment is not an excluded type but is from TSI, use TsiTransType.None
			}
			else {
				//If this adjustment is an excluded type, mark it excluded regardless of if the adjustment is from TSI.
				//This means if an adjustment is created for a Collections patient using the "No - this adjustment is the result
				//of a payment received from TSI" option and the adjustment is marked as an excluded type then the previous
				//decision will be effectively overridden to to behave as though the adjustment was applied by the office.
				msgText="Adjustment type is set to excluded type from transworld program properties.";
				typeCur=TsiTransType.Excluded;
			}
			InsertTsiLogsForAdjustment(guar.PatNum,adj,msgText,typeCur);
		}
		
		#endregion Insert

		#region Update

		///<summary></summary>
		public static void Update(TsiTransLog tsiTransLog,TsiTransLog tsiTransLogOld){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				Meth.GetVoid(MethodBase.GetCurrentMethod(),tsiTransLog,tsiTransLogOld);
				return;
			}
			Crud.TsiTransLogCrud.Update(tsiTransLog,tsiTransLogOld);
		}

		#endregion Update

		#region Delete

		///<summary>Used by the OpenDentalService for aggregate logs that are pre-inserted to get a primary key so that messages aggregated will have
		///a FK pointing to the agg message and sending to TSI fails.  We need to delete the agg log so that we can try again later.</summary>
		public static void DeleteMany(List<long> listLogNums) {
			if(listLogNums.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listLogNums);
				return;
			}
			string command="DELETE FROM tsitranslog WHERE TsiTransLogNum IN("+string.Join(",",listLogNums)+")";
			Db.NonQ(command);
		}
		
		#endregion Delete

		#region Misc Methods
		
		/// <summary>Getting the balance from the messages from the patAging object using logs.</summary>
		public static double GetBalFromMsgs(PatAging patAging) {
			TsiTransLog placeLog=patAging.ListTsiLogs.FirstOrDefault(x => x.TransType==TsiTransType.PL);
			if(placeLog==null) {
				return 0;//should never happen, this is a collection guarantor so there must be a placement log
			}
			return placeLog.AccountBalance+
				patAging.ListTsiLogs
					.Where(x => x.TransDateTime>placeLog.TransDateTime
						&& !ListTools.In(x.TransType,TsiTransType.PL,TsiTransType.RI,TsiTransType.SS,TsiTransType.CN,TsiTransType.Agg,TsiTransType.Excluded))
					.Sum(x => x.TransAmt);
		}

		///<summary>Returns true if the guarantor has been sent to TSI and has not been canceled or paid in full.</summary>
		public static bool HasGuarBeenSentToTSI(Patient guar) {
			if(guar==null || !IsTransworldEnabled(guar.ClinicNum)) {
				return false;
			}
			TsiTransType[] arrayStatusTransTypes=new[] { TsiTransType.SS,TsiTransType.CN,TsiTransType.RI,TsiTransType.PF,TsiTransType.PT,TsiTransType.PL };
			TsiTransLog mostRecentTransStatusChangeLog=SelectMany(new List<long>(){ guar.Guarantor }).FindAll(x => ListTools.In(x.TransType,arrayStatusTransTypes))
				.OrderBy(x => x.TransDateTime).LastOrDefault();
			if(mostRecentTransStatusChangeLog==null) {
				return false;//Not being managed by TSI
			}
			//Check if the most recent log is of type SS, PL, or RI. 
			return ListTools.In(mostRecentTransStatusChangeLog.TransType,new[] { TsiTransType.SS,TsiTransType.PL,TsiTransType.RI });
		}

		public static bool ValidateClinicSftpDetails(List<ProgramProperty> listPropsForClinic,bool doTestConnection=true) {
			//No need to check RemotingRole;no call to db.
			if(listPropsForClinic==null || listPropsForClinic.Count==0) {
				return false;
			}
			string sftpAddress=listPropsForClinic.Find(x => x.PropertyDesc=="SftpServerAddress")?.PropertyValue;
			int sftpPort;
			if(!int.TryParse(listPropsForClinic.Find(x => x.PropertyDesc=="SftpServerPort")?.PropertyValue,out sftpPort)
				|| sftpPort<ushort.MinValue//0
				|| sftpPort>ushort.MaxValue)//65,535
			{
				sftpPort=22;//default to port 22
			}
			string userName=listPropsForClinic.Find(x => x.PropertyDesc=="SftpUsername")?.PropertyValue;
			string userPassword=CDT.Class1.TryDecrypt(listPropsForClinic.Find(x => x.PropertyDesc=="SftpPassword")?.PropertyValue);
			if(string.IsNullOrWhiteSpace(sftpAddress) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(userPassword)) {
				return false;
			}
			string[] selectedServices=listPropsForClinic.FirstOrDefault(x => x.PropertyDesc=="SelectedServices")
				?.PropertyValue
				?.Split(new[] { ',' },StringSplitOptions.RemoveEmptyEntries);
			if(selectedServices.IsNullOrEmpty()) {//must have at least one service selected, i.e. Accelerator, Profit Recovery, and/or Collection
				return false;
			}
			if(doTestConnection) {
				return Sftp.IsConnectionValid(sftpAddress,userName,userPassword,sftpPort);
			}
			return true;
		}

		public static bool IsTransworldEnabled(long clinicNum) {
			//No need to check RemotingRole;no call to db.
			Program progCur=Programs.GetCur(ProgramName.Transworld);
			if(progCur==null || !progCur.Enabled) {
				return false;
			}
			Dictionary<long,List<ProgramProperty>> dictAllProps=ProgramProperties.GetForProgram(progCur.ProgramNum)
				.GroupBy(x => x.ClinicNum)
				.ToDictionary(x => x.Key,x => x.ToList());
			if(dictAllProps.Count==0) {
				return false;
			}
			List<long> listDisabledClinicNums=new List<long>();
			if(!PrefC.HasClinicsEnabled) {
				return TsiTransLogs.ValidateClinicSftpDetails(dictAllProps[0],false);
			}
			List<Clinic> listAllClinics=Clinics.GetDeepCopy();
			listDisabledClinicNums.AddRange(dictAllProps.Where(x => !TsiTransLogs.ValidateClinicSftpDetails(x.Value,false)).Select(x => x.Key));
			listDisabledClinicNums.AddRange(listAllClinics
				.FindAll(x => x.IsHidden || (listDisabledClinicNums.Contains(0) && !dictAllProps.ContainsKey(x.ClinicNum)))//if no props for HQ, skip other clinics without props
				.Select(x => x.ClinicNum)
			);
			return !listDisabledClinicNums.Contains(clinicNum);
		}

		///<summary>Sends an SFTP message to TSI to suspend the account for the guarantor passed in.  Returns empty string if successful.
		///Returns a translated error message that should be displayed to the user if anything goes wrong.</summary>
		public static string SuspendGuar(Patient guar) {
			PatAging patAging=Patients.GetAgingListFromGuarNums(new List<long>() { guar.PatNum }).FirstOrDefault();
			if(patAging==null) {//this would only happen if the patient was not in the db??, just in case
				return Lans.g("TsiTransLogs","An error occurred when trying to send a suspend message to TSI.");
			}
			long clinicNum=(PrefC.HasClinicsEnabled?guar.ClinicNum:0);
			Program prog=Programs.GetCur(ProgramName.Transworld);
			if(prog==null) {//shouldn't be possible, the program link should always exist, just in case
				return Lans.g("TsiTransLogs","The Transworld program link does not exist.  Contact support.");
			}
			Dictionary<long,List<ProgramProperty>> dictAllProps=ProgramProperties.GetForProgram(prog.ProgramNum)
				.GroupBy(x => x.ClinicNum)
				.ToDictionary(x => x.Key,x => x.ToList());
			if(dictAllProps.Count==0) {//shouldn't be possible, there should always be a set of props for ClinicNum 0 even if disabled, just in case
				return Lans.g("TsiTransLogs","The Transworld program link is not setup properly.");
			}
			if(PrefC.HasClinicsEnabled && !dictAllProps.ContainsKey(clinicNum) && dictAllProps.ContainsKey(0)) {
				clinicNum=0;
			}
			string clinicDesc=clinicNum==0?"Headquarters":Clinics.GetDesc(clinicNum);
			if(!dictAllProps.ContainsKey(clinicNum)
				||  !ValidateClinicSftpDetails(dictAllProps[clinicNum],true)) //the props should be valid, but this will test the connection using the props
			{
				return Lans.g("TsiTransLogs","The Transworld program link is not enabled")+" "
					+(PrefC.HasClinicsEnabled?(Lans.g("TsiTransLogs","for the guarantor's clinic")+", "+clinicDesc+", "):"")
					+Lans.g("TsiTransLogs","or is not setup properly.");
			}
			List<ProgramProperty> listProps=dictAllProps[clinicNum];
			Def newBillType=Defs.GetDef(DefCat.BillingTypes,PrefC.GetLong(PrefName.TransworldPaidInFullBillingType));
			if(newBillType==null) {
				return Lans.g("TsiTransLogs","The default paid in full billing type is not set.  An automated suspend message cannot be sent until the "
					+"default paid in full billing type is set in the Transworld program link")
					+(PrefC.HasClinicsEnabled?(" "+Lans.g("TsiTransLogs","for the guarantor's clinic")+", "+clinicDesc):"")+".";
			}
			string clientId="";
			if(patAging.ListTsiLogs.Count>0) {
				clientId=patAging.ListTsiLogs[0].ClientId;
			}
			if(string.IsNullOrEmpty(clientId)) {
				clientId=listProps.Find(x => x.PropertyDesc=="ClientIdAccelerator")?.PropertyValue;
			}
			if(string.IsNullOrEmpty(clientId)) {
				clientId=listProps.Find(x => x.PropertyDesc=="ClientIdCollection")?.PropertyValue;
			}
			if(string.IsNullOrEmpty(clientId)) {
				return Lans.g("TsiTransLogs","There is no client ID in the Transworld program link")
					+(PrefC.HasClinicsEnabled?(" "+Lans.g("TsiTransLogs","for the guarantor's clinic")+", "+clinicDesc):"")+".";
			}
			string sftpAddress=listProps.Find(x => x.PropertyDesc=="SftpServerAddress")?.PropertyValue??"";
			int sftpPort;
			if(!int.TryParse(listProps.Find(x => x.PropertyDesc=="SftpServerPort")?.PropertyValue??"",out sftpPort)) {
				sftpPort=22;//default to port 22
			}
			string userName=listProps.Find(x => x.PropertyDesc=="SftpUsername")?.PropertyValue??"";
			string userPassword=CDT.Class1.TryDecrypt(listProps.Find(x => x.PropertyDesc=="SftpPassword")?.PropertyValue??"");
			if(new[] { sftpAddress,userName,userPassword }.Any(x => string.IsNullOrEmpty(x))) {
				return Lans.g("TsiTransLogs","The SFTP address, username, or password for the Transworld program link")+" "
					+(PrefC.HasClinicsEnabled?(Lans.g("TsiTransLogs","for the guarantor's clinic")+", "+clinicDesc+", "):"")+Lans.g("TsiTransLogs","is blank.");
			}
			string msg=TsiMsgConstructor.GenerateUpdate(patAging.PatNum,clientId,TsiTransType.SS,0.00,patAging.AmountDue);
			try {
				byte[] fileContents=Encoding.ASCII.GetBytes(TsiMsgConstructor.GetUpdateFileHeader()+"\r\n"+msg);
				TaskStateUpload state=new Sftp.Upload(sftpAddress,userName,userPassword,sftpPort) {
					Folder="/xfer/incoming",
					FileName="TsiUpdates_"+DateTime.Now.ToString("yyyyMMddhhmmss")+".txt",
					FileContent=fileContents,
					HasExceptions=true
				};
				state.Execute(false);
			}
			catch(Exception ex) {
				return Lans.g("TsiTransLogs","There was an error sending the update message to Transworld")
					+(PrefC.HasClinicsEnabled?(" "+Lans.g("TsiTransLogs","using the program properties for the guarantor's clinic")+", "+clinicDesc):"")+".\r\n"
					+ex.Message;
			}
			//Upload was successful
			TsiTransLog log=new TsiTransLog() {
				PatNum=patAging.PatNum,
				UserNum=Security.CurUser.UserNum,
				TransType=TsiTransType.SS,
				//TransDateTime=DateTime.Now,//set on insert, not editable by user
				//ServiceType=TsiServiceType.Accelerator,//only valid for placement msgs
				//ServiceCode=TsiServiceCode.Diplomatic,//only valid for placement msgs
				ClientId=clientId,
				TransAmt=0.00,
				AccountBalance=patAging.AmountDue,
				FKeyType=TsiFKeyType.None,//only used for account trans updates
				FKey=0,//only used for account trans updates
				RawMsgText=msg,
				ClinicNum=clinicNum
				//,TransJson=""//only valid for placement msgs
			};
			TsiTransLogs.Insert(log);
			//update family billing type to the paid in full billing type pref
			List<Patient> listAllPats=Patients.GetFamily(patAging.Guarantor).ListPats.ToList();
			Patients.UpdateFamilyBillingType(newBillType.DefNum,patAging.PatNum);
			foreach(Patient pat in listAllPats) {
				if(pat.BillingType==newBillType.DefNum) {
					continue;
				}
				string logTxt="Patient billing type changed from '"+Defs.GetName(DefCat.BillingTypes,pat.BillingType)+"' to '"+newBillType.ItemName
					+"' due to a status update message being sent to Transworld from the account module.";
				SecurityLogs.MakeLogEntry(Permissions.PatientBillingEdit,pat.PatNum,logTxt);
			}
			return "";
		}

		#endregion Misc Methods
	}

}