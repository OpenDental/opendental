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

	#region Get Methods

		///<summary>Returns all tsitranslogs for the patients in listPatNums.  Returns empty list if listPatNums is empty or null.</summary>
		public static List<TsiTransLog> SelectMany(List<long> listPatNums){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<TsiTransLog>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM tsitranslog ORDER BY TransDateTime DESC";
			return Crud.TsiTransLogCrud.SelectMany(command);
		}

		///<summary>Returns a list of PatNums for guars who have a TsiTransLog with type SS (suspend) less than 50 days ago who don't have a TsiTransLog
		///with type CN (cancel), PF (paid in full), PT (paid in full, thank you), or PL (placement) with a more recent date, since this would change the
		///account status from suspended to either closed/canceled or if the more recent message had type PL (placement) back to active.</summary>
		public static List<long> GetSuspendedGuarNums() {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod());
			}
			List<int> listStatusTransTypes=new List<int>();
			listStatusTransTypes.Add((int)TsiTransType.SS);
			listStatusTransTypes.Add((int)TsiTransType.CN);
			listStatusTransTypes.Add((int)TsiTransType.RI);
			listStatusTransTypes.Add((int)TsiTransType.PF);
			listStatusTransTypes.Add((int)TsiTransType.PT);
			listStatusTransTypes.Add((int)TsiTransType.PL);
			string command="SELECT DISTINCT tsitranslog.PatNum "
				+"FROM tsitranslog "
				+"INNER JOIN ("
					+"SELECT PatNum,MAX(TransDateTime) transDateTime "
					+"FROM tsitranslog "
					+"WHERE TransType IN("+string.Join(",",listStatusTransTypes)+") "
					+"AND TransDateTime>"+POut.DateT(DateTime.Now.AddDays(-50))+" "
					+"GROUP BY PatNum"
				+") mostRecentTrans ON tsitranslog.PatNum=mostRecentTrans.PatNum "
					+"AND tsitranslog.TransDateTime=mostRecentTrans.transDateTime "
				+"WHERE tsitranslog.TransType="+(int)TsiTransType.SS;
			return Db.GetListLong(command);
		}

		public static bool IsGuarSuspended(long guarNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),guarNum);
			}
			List<int> listStatusTransTypes=new List<int>();
			listStatusTransTypes.Add((int)TsiTransType.SS);
			listStatusTransTypes.Add((int)TsiTransType.CN);
			listStatusTransTypes.Add((int)TsiTransType.RI);
			listStatusTransTypes.Add((int)TsiTransType.PF);
			listStatusTransTypes.Add((int)TsiTransType.PT);
			listStatusTransTypes.Add((int)TsiTransType.PL);
			string command="SELECT (CASE WHEN tsitranslog.TransType="+(int)TsiTransType.SS+" THEN 1 ELSE 0 END) isGuarSuspended "
				+"FROM tsitranslog "
				+"INNER JOIN ("
					+"SELECT PatNum,MAX(TransDateTime) transDateTime "
					+"FROM tsitranslog "
					+"WHERE PatNum="+POut.Long(guarNum)+" " 
					+"AND TransType IN("+string.Join(",",listStatusTransTypes)+") "
					+"AND TransDateTime>"+POut.DateT(DateTime.Now.AddDays(-50))+" "
					+"GROUP BY PatNum"
				+") mostRecentLog ON tsitranslog.PatNum=mostRecentLog.PatNum AND tsitranslog.TransDateTime=mostRecentLog.transDateTime";
			return PIn.Bool(Db.GetScalar(command));
		}

		#endregion Get Methods

		#region Insert

		public static long Insert(TsiTransLog tsiTransLog) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				tsiTransLog.TsiTransLogNum=Meth.GetLong(MethodBase.GetCurrentMethod(),tsiTransLog);
				return tsiTransLog.TsiTransLogNum;
			}
			return Crud.TsiTransLogCrud.Insert(tsiTransLog);
		}

		public static void InsertMany(List<TsiTransLog> listTsiTransLogs) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listTsiTransLogs);
				return;
			}
			Crud.TsiTransLogCrud.InsertMany(listTsiTransLogs);
		}

		private static void InsertTsiLogsForAdjustment(long patGuar,Adjustment adjustment,string msgText,TsiTransType tsiTransType) {
			//No need to check MiddleTierRole; no call to db.
			//insert tsitranslog for this transaction so the ODService won't send it to Transworld.  _isTsiAdj means Transworld received a payment on
			//behalf of this guar and took a percentage and send the rest to the office for the account.  This will result in a payment being entered
			//into the account, having been received from Transworld, and an adjustment to account for Transorld's cut.
			PatAging patAging=Patients.GetAgingListFromGuarNums(new List<long>() { patGuar }).FirstOrDefault();//should only ever be 1
			if(patAging==null) {
				return;
			}
			double offsetAmt=adjustment.AdjAmt-patAging.ListTsiLogs.FindAll(x => x.FKeyType==TsiFKeyType.Adjustment && x.FKey==adjustment.AdjNum).Sum(x => x.TransAmt);
			if(CompareDouble.IsZero(offsetAmt)) {
				return;
			}
			double balFromMsgs=GetBalFromMsgs(patAging);
			if(CompareDouble.IsZero(balFromMsgs)) {
				return;
			}
			TsiTransLog tsiTransLog=new TsiTransLog();
			tsiTransLog.PatNum=patAging.PatNum;
			tsiTransLog.UserNum=Security.CurUser.UserNum;
			tsiTransLog.TransType=tsiTransType;
			//tsiTransLog.TransDateTime=DateTime.Now;//set on insert, not editable by user
			//tsiTransLog.ServiceType=TsiServiceType.Accelerator;//only valid for placement msgs
			//tsiTransLog.ServiceCode=TsiServiceCode.Diplomatic;//only valid for placement msgs
			tsiTransLog.ClientId=patAging.ListTsiLogs.FirstOrDefault()?.ClientId??"";//can be blank, not used since this isn't really sent to Transworld
			tsiTransLog.TransAmt=offsetAmt;
			tsiTransLog.AccountBalance=balFromMsgs+offsetAmt;
			if(tsiTransType==TsiTransType.Excluded) {
				tsiTransLog.AccountBalance=balFromMsgs;
			}
			tsiTransLog.FKeyType=TsiFKeyType.Adjustment;
			tsiTransLog.FKey=adjustment.AdjNum;
			tsiTransLog.RawMsgText=msgText;
			//tsi.Translog.TransJson="";//only valid for placement msgs
			tsiTransLog.ClinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				tsiTransLog.ClinicNum=patAging.ClinicNum;
			}
			Insert(tsiTransLog);
		}

		/// <summary>Inserts a TsiTransLog for the adjustment if necessary.</summary>
		public static void CheckAndInsertLogsIfAdjTypeExcluded(Adjustment adjustment,bool isFromTsi=false) {
			//No need to check MiddleTierRole; no call to db.
			Program program=Programs.GetCur(ProgramName.Transworld);
			if(program==null || !program.Enabled) {
				return;
			}
			Patient patientGuar=Patients.GetGuarForPat(adjustment.PatNum);
			if(patientGuar==null || !IsTransworldEnabled(patientGuar.ClinicNum) || !Patients.IsGuarCollections(patientGuar.PatNum)) {
				return;
			}
			string msgText="This was not a message sent to Transworld.  This adjustment was entered due to a payment received from Transworld.";
			TsiTransType tsiTransType=TsiTransType.None;
			List<ProgramProperty> listProgramProperties=ProgramProperties
				.GetWhere(x => x.ProgramNum==program.ProgramNum 
					&& (x.PropertyDesc==ProgramProperties.PropertyDescs.TransWorld.SyncExcludePosAdjType
						|| x.PropertyDesc==ProgramProperties.PropertyDescs.TransWorld.SyncExcludeNegAdjType));
			//use guar's clinic if clinics are enabled and props for that clinic exist, otherwise use ClinicNum 0
			List<ProgramProperty> listProgramPropertiesForClinic=listProgramProperties.FindAll(x=>x.ClinicNum==patientGuar.ClinicNum);
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled && listProgramPropertiesForClinic.Count>0){
				clinicNum=patientGuar.ClinicNum;
			}
			else{
				listProgramPropertiesForClinic=listProgramProperties.FindAll(x=>x.ClinicNum==0);
			}
			if(listProgramPropertiesForClinic.Count!=0//should always be props for ClinicNum 0
				&& listProgramPropertiesForClinic.Any(x => PIn.Long(x.PropertyValue,false)==adjustment.AdjType))
			{
				//If this adjustment is an excluded type, mark it excluded regardless of if the adjustment is from TSI.
				//This means if an adjustment is created for a Collections patient using the "No - this adjustment is the result
				//of a payment received from TSI" option and the adjustment is marked as an excluded type then the previous
				//decision will be effectively overridden to to behave as though the adjustment was applied by the office.
				msgText="Adjustment type is set to excluded type from transworld program properties.";
				tsiTransType=TsiTransType.Excluded;
			}
			else if(!isFromTsi) {
				return;//if this adjustment is not an excluded type and not from TSI, return
			}
			InsertTsiLogsForAdjustment(patientGuar.PatNum,adjustment,msgText,tsiTransType);
		}
		
		#endregion Insert

		#region Update

		///<summary></summary>
		public static void Update(TsiTransLog tsiTransLog,TsiTransLog tsiTransLogOld){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT){
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			//No need to check MiddleTierRole; no call to db.
			TsiTransLog tsiTransLog=patAging.ListTsiLogs.FirstOrDefault(x => x.TransType==TsiTransType.PL);
			if(tsiTransLog==null) {
				return 0;//should never happen, this is a collection guarantor so there must be a placement log
			}
			double balFromMsgs=tsiTransLog.AccountBalance
				+patAging.ListTsiLogs
					.Where(x => x.TransDateTime>tsiTransLog.TransDateTime
						&& !x.TransType.In(TsiTransType.PL,TsiTransType.RI,TsiTransType.SS,TsiTransType.CN,TsiTransType.Agg,TsiTransType.Excluded))
					.Sum(x => x.TransAmt);
			return balFromMsgs;
		}

		///<summary>Returns true if the guarantor has been sent to TSI and has not been canceled or paid in full.</summary>
		public static bool HasGuarBeenSentToTSI(Patient patient) {
			//No need to check MiddleTierRole; no call to db.
			if(patient==null || !IsTransworldEnabled(patient.ClinicNum)) {
				return false;
			}
			List<TsiTransType> listTsiTransTypes=new List<TsiTransType>();
			listTsiTransTypes.Add(TsiTransType.SS);
			listTsiTransTypes.Add(TsiTransType.CN);
			listTsiTransTypes.Add(TsiTransType.RI);
			listTsiTransTypes.Add(TsiTransType.PF);
			listTsiTransTypes.Add(TsiTransType.PT);
			listTsiTransTypes.Add(TsiTransType.PL);
			TsiTransLog tsiTransLogRecent=SelectMany(new List<long>(){ patient.Guarantor }).FindAll(x => listTsiTransTypes.Contains(x.TransType))
				.OrderBy(x => x.TransDateTime).LastOrDefault();
			if(tsiTransLogRecent==null) {
				return false;//Not being managed by TSI
			}
			//Check if the most recent log is of type SS, PL, or RI. 
			return tsiTransLogRecent.TransType.In(new[] { TsiTransType.SS,TsiTransType.PL,TsiTransType.RI });
		}

		public static bool ValidateClinicSftpDetails(List<ProgramProperty> listProgramProperties,bool hasConnection=true) {
			//No need to check MiddleTierRole; no call to db.
			if(listProgramProperties==null || listProgramProperties.Count==0) {
				return false;
			}
			string sftpAddress=listProgramProperties.Find(x => x.PropertyDesc=="SftpServerAddress")?.PropertyValue;
			int sftpPort;
			if(!int.TryParse(listProgramProperties.Find(x => x.PropertyDesc=="SftpServerPort")?.PropertyValue,out sftpPort)
				|| sftpPort<ushort.MinValue//0
				|| sftpPort>ushort.MaxValue)//65,535
			{
				sftpPort=22;//default to port 22
			}
			string userName=listProgramProperties.Find(x => x.PropertyDesc=="SftpUsername")?.PropertyValue;
			string userPassword=CDT.Class1.TryDecrypt(listProgramProperties.Find(x => x.PropertyDesc=="SftpPassword")?.PropertyValue);
			if(string.IsNullOrWhiteSpace(sftpAddress) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(userPassword)) {
				return false;
			}
			string[] stringArraySelectedServices=listProgramProperties.FirstOrDefault(x => x.PropertyDesc=="SelectedServices")
				?.PropertyValue
				?.Split(new[] { ',' },StringSplitOptions.RemoveEmptyEntries);
			if(stringArraySelectedServices.IsNullOrEmpty()) {//must have at least one service selected, i.e. Accelerator, Profit Recovery, and/or Collection
				return false;
			}
			if(hasConnection) {
				return Sftp.IsConnectionValid(sftpAddress,userName,userPassword,sftpPort);
			}
			return true;
		}

		public static bool IsTransworldEnabled(long clinicNum) {
			//No need to check MiddleTierRole; no call to db.
			Program program=Programs.GetCur(ProgramName.Transworld);
			if(program==null || !program.Enabled) {
				return false;
			}
			List<ProgramProperty> listProgramProperties=ProgramProperties.GetForProgram(program.ProgramNum);
			if(listProgramProperties.Count==0) {
				return false;
			}
			List<ProgramProperty> listProgramPropertiesForClinic=listProgramProperties.FindAll(x=>x.ClinicNum==clinicNum);
			if(PrefC.HasClinicsEnabled && listProgramPropertiesForClinic.Count>0) {
				return TsiTransLogs.ValidateClinicSftpDetails(listProgramPropertiesForClinic,false);
			}
			listProgramPropertiesForClinic=listProgramProperties.FindAll(x=>x.ClinicNum==0);
			if(listProgramPropertiesForClinic.Count>0) {
				return TsiTransLogs.ValidateClinicSftpDetails(listProgramPropertiesForClinic,false);
			}
			return false;
		}

		///<summary>Sends an SFTP message to TSI to suspend the account for the guarantor passed in.  Returns empty string if successful.
		///Returns a translated error message that should be displayed to the user if anything goes wrong.</summary>
		public static string SuspendGuar(Patient patient) {
			//No need to check MiddleTierRole; no call to db.
			PatAging patAging=Patients.GetAgingListFromGuarNums(new List<long>() { patient.PatNum }).FirstOrDefault();
			if(patAging==null) {//this would only happen if the patient was not in the db??, just in case
				return Lans.g("TsiTransLogs","An error occurred when trying to send a suspend message to TSI.");
			}
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=patient.ClinicNum;
			}
			Program program=Programs.GetCur(ProgramName.Transworld);
			if(program==null) {//shouldn't be possible, the program link should always exist, just in case
				return Lans.g("TsiTransLogs","The Transworld program link does not exist.  Contact support.");
			}
			List<ProgramProperty> listProgramPropertiesAll=ProgramProperties.GetForProgram(program.ProgramNum);
			if(listProgramPropertiesAll.Count==0) {//shouldn't be possible, there should always be a set of props for ClinicNum 0 even if disabled, just in case
				return Lans.g("TsiTransLogs","The Transworld program link is not setup properly.");
			}
			List<ProgramProperty> listProgramPropertiesClinic=listProgramPropertiesAll.FindAll(x=>x.ClinicNum==clinicNum);
			List<ProgramProperty> listProgramPropertiesClinicZero=listProgramPropertiesAll.FindAll(x=>x.ClinicNum==0);
			if(PrefC.HasClinicsEnabled && listProgramPropertiesClinic.Count==0 && listProgramPropertiesClinicZero.Count>0) {
				clinicNum=0;
				listProgramPropertiesClinic=listProgramPropertiesClinicZero;
			}
			string clinicDesc=Clinics.GetDesc(clinicNum);
			if(clinicNum==0) {
				clinicDesc="Headquarters";
			}
			if(listProgramPropertiesClinic.Count==0
				||  !ValidateClinicSftpDetails(listProgramPropertiesClinic,true)) //the props should be valid, but this will test the connection using the props
			{
				return Lans.g("TsiTransLogs","The Transworld program link is not enabled")+" "
					+(PrefC.HasClinicsEnabled?(Lans.g("TsiTransLogs","for the guarantor's clinic")+", "+clinicDesc+", "):"")
					+Lans.g("TsiTransLogs","or is not setup properly.");
			}
			Def defBillTypeNew=Defs.GetDef(DefCat.BillingTypes,PrefC.GetLong(PrefName.TransworldPaidInFullBillingType));
			if(defBillTypeNew==null) {
				return Lans.g("TsiTransLogs","The default paid in full billing type is not set.  An automated suspend message cannot be sent until the "
					+"default paid in full billing type is set in the Transworld program link")
					+(PrefC.HasClinicsEnabled?(" "+Lans.g("TsiTransLogs","for the guarantor's clinic")+", "+clinicDesc):"")+".";
			}
			string clientId="";
			if(patAging.ListTsiLogs.Count>0) {
				clientId=patAging.ListTsiLogs[0].ClientId;
			}
			if(string.IsNullOrEmpty(clientId)) {
				clientId=listProgramPropertiesClinic.Find(x => x.PropertyDesc=="ClientIdAccelerator")?.PropertyValue;
			}
			if(string.IsNullOrEmpty(clientId)) {
				clientId=listProgramPropertiesClinic.Find(x => x.PropertyDesc=="ClientIdCollection")?.PropertyValue;
			}
			if(string.IsNullOrEmpty(clientId)) {
				return Lans.g("TsiTransLogs","There is no client ID in the Transworld program link")
					+(PrefC.HasClinicsEnabled?(" "+Lans.g("TsiTransLogs","for the guarantor's clinic")+", "+clinicDesc):"")+".";
			}
			string sftpAddress=listProgramPropertiesClinic.Find(x => x.PropertyDesc=="SftpServerAddress")?.PropertyValue??"";
			int sftpPort;
			if(!int.TryParse(listProgramPropertiesClinic.Find(x => x.PropertyDesc=="SftpServerPort")?.PropertyValue??"",out sftpPort)) {
				sftpPort=22;//default to port 22
			}
			string userName=listProgramPropertiesClinic.Find(x => x.PropertyDesc=="SftpUsername")?.PropertyValue??"";
			string userPassword=CDT.Class1.TryDecrypt(listProgramPropertiesClinic.Find(x => x.PropertyDesc=="SftpPassword")?.PropertyValue??"");
			if(new[] { sftpAddress,userName,userPassword }.Any(x => string.IsNullOrEmpty(x))) {
				return Lans.g("TsiTransLogs","The SFTP address, username, or password for the Transworld program link")+" "
					+(PrefC.HasClinicsEnabled?(Lans.g("TsiTransLogs","for the guarantor's clinic")+", "+clinicDesc+", "):"")+Lans.g("TsiTransLogs","is blank.");
			}
			string msg=TsiMsgConstructor.GenerateUpdate(patAging.PatNum,clientId,TsiTransType.SS,0.00,patAging.AmountDue);
			byte[] byteArrayFileContents=Encoding.ASCII.GetBytes(TsiMsgConstructor.GetUpdateFileHeader()+"\r\n"+msg);
			TaskStateUpload taskStateUpload=new Sftp.Upload(sftpAddress,userName,userPassword,sftpPort);
			taskStateUpload.Folder="/xfer/incoming";
			taskStateUpload.FileName="TsiUpdates_"+DateTime.Now.ToString("yyyyMMddhhmmss")+".txt";
			taskStateUpload.FileContent=byteArrayFileContents;
			taskStateUpload.HasExceptions=true;
			try {
				taskStateUpload.Execute(false);
			}
			catch(Exception ex) {
				return Lans.g("TsiTransLogs","There was an error sending the update message to Transworld")
					+(PrefC.HasClinicsEnabled?(" "+Lans.g("TsiTransLogs","using the program properties for the guarantor's clinic")+", "+clinicDesc):"")+".\r\n"
					+ex.Message;
			}
			//Upload was successful
			TsiTransLog tsiTransLog=new TsiTransLog(); 
			tsiTransLog.PatNum=patAging.PatNum;
			tsiTransLog.UserNum=Security.CurUser.UserNum;
			tsiTransLog.TransType=TsiTransType.SS;
			//tsiTransLog.TransDateTime=DateTime.Now;//set on insert, not editable by user
			//tsiTransLog.ServiceType=TsiServiceType.Accelerator;//only valid for placement msgs
			//tsiTransLog.ServiceCode=TsiServiceCode.Diplomatic;//only valid for placement msgs
			tsiTransLog.ClientId=clientId;
			tsiTransLog.TransAmt=0.00;
			tsiTransLog.AccountBalance=patAging.AmountDue;
			tsiTransLog.FKeyType=TsiFKeyType.None;//only used for account trans updates
			tsiTransLog.FKey=0;//only used for account trans updates
			tsiTransLog.RawMsgText=msg;
			tsiTransLog.ClinicNum=clinicNum;
			//tsiTransLog.TransJson="";//only valid for placement msgs
			TsiTransLogs.Insert(tsiTransLog);
			//update family billing type to the paid in full billing type pref
			List<Patient> listPatientsAll=Patients.GetFamily(patAging.Guarantor).ListPats.ToList();
			Patients.UpdateFamilyBillingType(defBillTypeNew.DefNum,patAging.PatNum);
			for(int i=0;i<listPatientsAll.Count;i++) {
				if(listPatientsAll[i].BillingType==defBillTypeNew.DefNum) {
					continue;
				}
				string logTxt="Patient billing type changed from '"+Defs.GetName(DefCat.BillingTypes,listPatientsAll[i].BillingType)+"' to '"+defBillTypeNew.ItemName 
					+"' due to a status update message being sent to Transworld from the account module.";
				SecurityLogs.MakeLogEntry(EnumPermType.PatientBillingEdit,listPatientsAll[i].PatNum,logTxt);
			}
			return "";
		}

		#endregion Misc Methods
	}

}