using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using OpenDentBusiness.Crud;
using CodeBase;
using DataConnectionBase;
using System.Xml.Serialization;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Claims{
		#region Get Methods

		///<summary>Returns a list of outstanding ClaimPaySplits for a given provider. 
		///It will only get outstanding claims with a date of service past dateTerm.</summary>
		public static List<ClaimPaySplit> GetOutstandingClaimsByProvider(long provNum,DateTime dateTerm) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ClaimPaySplit>>(MethodBase.GetCurrentMethod(),provNum,dateTerm);
			}
			string command="SELECT claim.ClaimNum,claim.PatNum,claim.ClaimStatus,claim.ClinicNum,claim.DateService,claim.ProvTreat,"
					+"claim.ClaimFee feeBilled_,"+DbHelper.Concat("patient.LName","', '","patient.FName")+" patName_,carrier.CarrierName,clinic.Description, "
					+"0 ClaimPaymentNum,0 insPayAmt_,claim.ClaimIdentifier,0 PaymentRow "//These values are not used in the consuming method.
				+"FROM claim "
				+"LEFT JOIN patient ON claim.PatNum = patient.PatNum "
				+"LEFT JOIN insplan ON claim.PlanNum = insplan.PlanNum "
				+"LEFT JOIN carrier ON insplan.CarrierNum = carrier.CarrierNum "
				+"LEFT JOIN clinic ON clinic.ClinicNum = claim.ClinicNum "
				+"WHERE (claim.ProvBill = "+POut.Long(provNum)+" "
					+"OR claim.ProvTreat = "+POut.Long(provNum)+" "
					+"OR claim.ProvOrderOverride = "+POut.Long(provNum)+") "
				+"AND claim.ClaimStatus != 'R' "
				+"AND claim.DateService > "+POut.Date(dateTerm)+" "
				+"GROUP BY claim.ClaimNum "
				+"ORDER BY clinic.Description,patient.PatNum";
			return ClaimPaySplitTableToList(Db.GetTable(command));
		}

		#endregion
		
		///<summary>Gets claimpaysplits attached to a claimpayment with the associated patient, insplan, and carrier. If showUnattached it also shows all claimpaysplits that have not been attached to a claimpayment. Pass (0,true) to just get all unattached (outstanding) claimpaysplits.</summary>
		public static List<ClaimPaySplit> RefreshByCheckOld(long claimPaymentNum,bool showUnattached) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ClaimPaySplit>>(MethodBase.GetCurrentMethod(),claimPaymentNum,showUnattached);
			}
			string command=
				"SELECT claim.DateService,claim.ProvTreat,CONCAT(CONCAT(patient.LName,', '),patient.FName) patName_"//Changed from \"_patName\" to patName_ for MySQL 5.5. Also added checks for #<table> and $<table>
				+",carrier.CarrierName,SUM(claimproc.FeeBilled) feeBilled_,SUM(claimproc.InsPayAmt) insPayAmt_,claim.ClaimNum"
				+",claimproc.ClaimPaymentNum,(SELECT clinic.Description FROM clinic WHERE claimproc.ClinicNum = clinic.ClinicNum) Description,claim.PatNum,PaymentRow,claim.ClaimStatus,claim.ClaimIdentifier "
				+" FROM claim,patient,insplan,carrier,claimproc"
				+" WHERE claimproc.ClaimNum = claim.ClaimNum"
				+" AND patient.PatNum = claim.PatNum"
				+" AND insplan.PlanNum = claim.PlanNum"
				+" AND insplan.CarrierNum = carrier.CarrierNum"
				+" AND (claimproc.Status = '1' OR claimproc.Status = '4' OR claimproc.Status=5)"//received or supplemental or capclaim
 				+" AND (claimproc.ClaimPaymentNum = '"+POut.Long(claimPaymentNum)+"'";
			if(showUnattached){
				command+=" OR (claimproc.InsPayAmt != 0 AND claimproc.ClaimPaymentNum = '0')";
			}
			//else shows only items attached to this payment
			command+=")"
				+" GROUP BY claim.DateService,claim.ProvTreat,CONCAT(CONCAT(patient.LName,', '),patient.FName) "
				+",carrier.CarrierName,claim.ClaimNum"
				+",claimproc.ClaimPaymentNum,claim.PatNum";
			command+=" ORDER BY patName_";
			DataTable table=Db.GetTable(command);
			return ClaimPaySplitTableToList(table);
		}

		///<summary></summary>
		public static List<Claim> GetClaimsByCheck(long claimPaymentNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Claim>>(MethodBase.GetCurrentMethod(),claimPaymentNum);
			}
			string command=
				"SELECT * "
				+"FROM claim "
				+"WHERE claim.ClaimNum IN "
				+"(SELECT DISTINCT claimproc.ClaimNum "
				+"FROM claimproc "
				+"WHERE claimproc.ClaimPaymentNum="+claimPaymentNum+")";
			return ClaimCrud.SelectMany(command);
		}

		///<summary>Gets all outstanding claims for the batch payment window.</summary>
		///<param name="carrierName">If not empty, will return claims with matching or partially matching carrier name.</param>
		///<param name="claimPayDate">DateClaimReceivedAfter preference. Only considers claims after this day.</param>
		public static List<ClaimPaySplit> GetOutstandingClaims(string carrierName,DateTime claimPayDate) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ClaimPaySplit>>(MethodBase.GetCurrentMethod(),carrierName,claimPayDate);
			}
			//Per Nathan, it is OK to return the DateService in the query result to display in the batch insurance window,
			//because that is the date which will be displayed in the Account module when you use the GoTo feature from batch insurance window.
			string command="SELECT outstanding.*,CONCAT(patient.LName,', ',patient.FName) AS patName_,";
			if(PrefC.HasClinicsEnabled && Clinics.GetCount() > 0) {
				command+="IFNULL(clinic.Description,'') ";
			}
			else {
				command+="'' ";
			}
			command+="AS Description FROM ("//Start outstanding
				+"SELECT * FROM ("
				+"SELECT claim.DateService,claim.ProvTreat,carrierA.CarrierName,claim.ClaimFee feeBilled_,claim.ClaimStatus,"
				+"SUM(claimproc.InsPayAmt) insPayAmt_,claim.ClaimNum,0 AS ClaimPaymentNum,claim.ClinicNum,claim.PatNum,0 AS PaymentRow,"
				+"SUM(CASE WHEN claimproc.ClaimPaymentNum=0 THEN 0 ELSE 1 END) AttachedCount,"
				+"SUM(CASE WHEN claimproc.ClaimPaymentNum=0 AND claimproc.InsPayAmt!=0 THEN 1 ELSE 0 END) UnattachedPayCount,claim.ClaimIdentifier "
				+"FROM ("//Start carrierA
					+"SELECT insplan.PlanNum,carrier.CarrierName "
					+"FROM carrier "
					+"INNER JOIN insplan ON carrier.CarrierNum=insplan.CarrierNum ";
				if(carrierName!="") {
					command+="WHERE carrier.CarrierName LIKE '%"+POut.String(carrierName)+"%'";
				}
			command+=") carrierA "//End carrierA
				+"INNER JOIN claim ON carrierA.PlanNum=claim.PlanNum AND claim.ClaimType!='PreAuth' ";
			//See job #7423.
			//The claimproc.DateCP is essentially the same as the claim.DateReceived.
			//We used to use the claimproc.ProcDate, which is essentially the same as the claim.DateService.
			//Since the service date could be weeks or months in the past, it makes more sense to use the received date, which will be more recent.
			//Additionally, users found using the date of service to be unintuitive.
			//STRONG CAUTION not to use the claimproc.ProcDate here in the future.
			command+="INNER JOIN claimproc ON claimproc.ClaimNum=claim.ClaimNum "
				+"WHERE " 
				+"claimproc.IsTransfer=0 AND " 
				+"(claim.ClaimStatus='S' OR "
				+"(claim.ClaimStatus='R' AND (claimproc.InsPayAmt!=0 "+((claimPayDate.Year>1880)?("OR claimproc.DateCP>="+POut.Date(claimPayDate)):"")+"))) "
				+"GROUP BY claim.ClaimNum";
			command+=") outstanding " 
				+"WHERE UnattachedPayCount > 0 "//Either unfinalized ins pay amounts on at least one claimproc on the claim,
				//or if preference is enabled with a specific date, also include received "NO PAYMENT" claims.
				//Always show Sent claims regardless of preference to match version 16.4 behavior (see job B8189).
				+"OR (AttachedCount=0"+((claimPayDate.Year>1880)?"":" AND ClaimStatus='S'")+")"
				+") outstanding "//End outstanding
				+"INNER JOIN patient ON patient.PatNum = outstanding.PatNum ";
			if(PrefC.HasClinicsEnabled && Clinics.GetCount() > 0) {
				command+="LEFT JOIN clinic ON clinic.ClinicNum = outstanding.ClinicNum ";
			}
			return ClaimPaySplitTableToList(Db.GetTable(command)).OrderByDescending(x => x.Carrier.StartsWith(carrierName)).ThenBy(x => x.Carrier)
				.ThenBy(x => x.PatName).ToList();
		}

		/// <summary>Gets all 'claims' attached to the claimpayment.</summary>
		public static List<ClaimPaySplit> GetAttachedToPayment(long claimPaymentNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ClaimPaySplit>>(MethodBase.GetCurrentMethod(),claimPaymentNum);
			}
			string command=
				"SELECT claim.DateService,claim.ProvTreat,"+DbHelper.Concat("patient.LName","', '","patient.FName")+" patName_,"
				+"carrier.CarrierName,ClaimFee feeBilled_,SUM(claimproc.InsPayAmt) insPayAmt_,claim.ClaimNum,claim.ClaimStatus,"
				+"claimproc.ClaimPaymentNum,clinic.Description,claim.PatNum,claim.ClaimIdentifier,PaymentRow "
				+" FROM claim,patient,insplan,carrier,claimproc"
				+" LEFT JOIN clinic ON clinic.ClinicNum = claimproc.ClinicNum"
				+" WHERE claimproc.ClaimNum = claim.ClaimNum"
				+" AND patient.PatNum = claim.PatNum"
				+" AND insplan.PlanNum = claim.PlanNum"
				+" AND insplan.CarrierNum = carrier.CarrierNum"
				+" AND claimproc.ClaimPaymentNum = "+claimPaymentNum+" ";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command+="GROUP BY claim.ClaimNum ";
			}
			else {//oracle
				command+="GROUP BY claim.DateService,claim.ProvTreat,"+DbHelper.Concat("patient.LName","', '","patient.FName")
					+",carrier.CarrierName,claim.ClaimNum,claimproc.ClaimPaymentNum,claim.PatNum,ClaimFee,clinic.Description,PaymentRow ";
			}
			command+="ORDER BY claimproc.PaymentRow";
			DataTable table=Db.GetTable(command);
			return ClaimPaySplitTableToList(table);
		}

		/// <summary>Gets all secondary claims for the related ClaimPaySplits. Called after a payment has been received.</summary>
		public static DataTable GetSecondaryClaims(List<ClaimPaySplit> claimsAttached) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),claimsAttached);
			}
			string command="SELECT DISTINCT ProcNum FROM claimproc WHERE ClaimNum IN (";
			string claimNums="";//used twice
			for(int i=0;i<claimsAttached.Count;i++) {
				if(i>0) {
					claimNums+=",";
				}
				claimNums+=claimsAttached[i].ClaimNum;
			}
			command+=claimNums+") AND ProcNum!=0";
			//List<ClaimProc> tempClaimProcs=ClaimProcCrud.SelectMany(command);
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				return new DataTable();//No procedures are attached to these claims.  This frequently happens in conversions.  No need to look for related secondary claims.
			}
			command="SELECT claimproc.PatNum,claimproc.ProcDate"
				+" FROM claimproc"
				+" JOIN claim ON claimproc.ClaimNum=claim.ClaimNum"
				+" WHERE ProcNum IN (";
			for(int i=0;i<table.Rows.Count;i++) {
				if(i>0) {
					command+=",";
				}
				command+=table.Rows[i]["ProcNum"].ToString();
			}
			command+=") AND claimproc.ClaimNum NOT IN ("+claimNums+")"
				+" AND ClaimType = 'S'"
				+" GROUP BY claimproc.ClaimNum,claimproc.PatNum,claimproc.ProcDate";
			DataTable secondaryClaims=Db.GetTable(command);
			return secondaryClaims;
		}

		///<summary></summary>
		public static List<ClaimPaySplit> GetInsPayNotAttachedForFixTool() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<ClaimPaySplit>>(MethodBase.GetCurrentMethod());
			}
			string command=
				"SELECT claim.DateService,claim.ProvTreat,CONCAT(CONCAT(patient.LName,', '),patient.FName) patName_"
				+",carrier.CarrierName,SUM(claimproc.FeeBilled) feeBilled_,SUM(claimproc.InsPayAmt) insPayAmt_,claim.ClaimNum,claim.ClaimStatus"
				+",claimproc.ClaimPaymentNum,(SELECT clinic.Description FROM clinic WHERE claimproc.ClinicNum = clinic.ClinicNum) Description,claim.PatNum,PaymentRow "
				+",claim.ClaimIdentifier "
				+" FROM claim,patient,insplan,carrier,claimproc"
				+" WHERE claimproc.ClaimNum = claim.ClaimNum"
				+" AND patient.PatNum = claim.PatNum"
				+" AND insplan.PlanNum = claim.PlanNum"
				+" AND insplan.CarrierNum = carrier.CarrierNum"
				+" AND (claimproc.Status = '1' OR claimproc.Status = '4' OR claimproc.Status=5)"//received or supplemental or capclaim
				+" AND (claimproc.InsPayAmt != 0 AND claimproc.ClaimPaymentNum = '0')"
				+" AND claimproc.IsTransfer=0"
				+" GROUP BY claim.DateService,claim.ProvTreat,CONCAT(CONCAT(patient.LName,', '),patient.FName)"
				+",carrier.CarrierName,claim.ClaimNum,claimproc.ClaimPaymentNum,claim.PatNum"
				+" ORDER BY patName_";
			DataTable table=Db.GetTable(command);
			return ClaimPaySplitTableToList(table);
		}

		///<summary></summary>
		private static List<ClaimPaySplit> ClaimPaySplitTableToList(DataTable table) {
			//No need to check RemotingRole; no call to db.
			List<ClaimPaySplit> splits=new List<ClaimPaySplit>();
			ClaimPaySplit split;
			for(int i=0;i<table.Rows.Count;i++){
				split=new ClaimPaySplit();
				split.DateClaim      =PIn.Date  (table.Rows[i]["DateService"].ToString());
				split.ProvAbbr       =Providers.GetAbbr(PIn.Long(table.Rows[i]["ProvTreat"].ToString()));
				split.PatName        =PIn.String(table.Rows[i]["patName_"].ToString());
				split.PatNum         =PIn.Long  (table.Rows[i]["PatNum"].ToString());
				split.Carrier        =PIn.String(table.Rows[i]["CarrierName"].ToString());
				split.FeeBilled      =PIn.Double(table.Rows[i]["feeBilled_"].ToString());
				split.InsPayAmt      =PIn.Double(table.Rows[i]["insPayAmt_"].ToString());
				split.ClaimNum       =PIn.Long  (table.Rows[i]["ClaimNum"].ToString());
				split.ClaimPaymentNum=PIn.Long  (table.Rows[i]["ClaimPaymentNum"].ToString());
				split.PaymentRow     =PIn.Int   (table.Rows[i]["PaymentRow"].ToString());
				split.ClinicDesc		 =PIn.String(table.Rows[i]["Description"].ToString());
				split.ClaimStatus    =PIn.String(table.Rows[i]["ClaimStatus"].ToString());
				split.ClaimIdentifier=PIn.String(table.Rows[i]["ClaimIdentifier"].ToString());
				splits.Add(split);
			}
			return splits;
		}

		///<summary>Gets the specified claim from the database.  Can be null.</summary>
		public static Claim GetClaim(long claimNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Claim>(MethodBase.GetCurrentMethod(),claimNum);
			}
			string command="SELECT * FROM claim"
				+" WHERE ClaimNum = "+claimNum.ToString();
			Claim retClaim=Crud.ClaimCrud.SelectOne(command);
			if(retClaim==null){
				return null;
			}
			command="SELECT * FROM claimattach WHERE ClaimNum = "+POut.Long(claimNum);
			retClaim.Attachments=Crud.ClaimAttachCrud.SelectMany(command);
			return retClaim;
		}

		public static List<Claim> GetClaimsFromClaimNums(List<long> listClaimNums) {
			if(listClaimNums.IsNullOrEmpty()) {
				return new List<Claim>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Claim>>(MethodBase.GetCurrentMethod(),listClaimNums);
			}
			string command=$"SELECT * FROM claim WHERE ClaimNum IN ({string.Join(",",listClaimNums)})";
			return ClaimCrud.SelectMany(command);
		}

		///<summary>Gets all claims for the specified patient. But without any attachments.</summary>
		public static List<Claim> Refresh(long patNum) {
			if(patNum==0) {
				return new List<Claim>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Claim>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command=
				"SELECT * FROM claim"
				+" WHERE PatNum = "+patNum.ToString()
				+" ORDER BY dateservice";
			return Crud.ClaimCrud.SelectMany(command);
		}

		public static Claim GetFromList(List<Claim> list,long claimNum) {
			//No need to check RemotingRole; no call to db.
			for(int i=0;i<list.Count;i++) {
				if(list[i].ClaimNum==claimNum) {
					return list[i].Copy();
				}
			}
			return null;
		}

		///<summary></summary>
		public static long Insert(Claim claim) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				claim.ClaimNum=Meth.GetLong(MethodBase.GetCurrentMethod(),claim);
				return claim.ClaimNum;
			}
			//Security.CurUser.UserNum gets set on MT by the DtoProcessor so it matches the user from the client WS.
			claim.SecUserNumEntry=Security.CurUser.UserNum;
			return Crud.ClaimCrud.Insert(claim);
		}

		///<summary></summary>
		public static void Update(Claim claim){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),claim);
				return;
			}
			Crud.ClaimCrud.Update(claim);
			//now, delete all attachments and recreate.
			string command="DELETE FROM claimattach WHERE ClaimNum="+POut.Long(claim.ClaimNum);
			Db.NonQ(command);
			for(int i=0;i<claim.Attachments.Count;i++) {
				claim.Attachments[i].ClaimNum=claim.ClaimNum;
				ClaimAttaches.Insert(claim.Attachments[i]);
			}
		}

		///<summary>Deletes the claim and also deletes any Etrans835Attaches when specified.</summary>
		public static void Delete(Claim claim,List<long> listEtrans835AttachNums=null){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),claim,listEtrans835AttachNums);
				return;
			}
			Etrans835Attaches.DeleteMany(listEtrans835AttachNums);
			Crud.ClaimCrud.Delete(claim.ClaimNum);
		}
		
		///<summary>Called from claimsend window and from Claim edit window.  Use 0 to get all waiting claims, or an actual claimnum to get just one claim.</summary>
		public static ClaimSendQueueItem[] GetQueueList(long claimNum,long clinicNum,long customTracking) {
			List<long> listClaimNums=new List<long>();
			if(claimNum!=0) {
				listClaimNums.Add(claimNum);
			}
			return GetQueueList(listClaimNums,clinicNum,customTracking);
		}

		///<summary>Called from claimsend window and from Claim edit window.  Use an empty listClaimNums to get all waiting claims.</summary>
		public static ClaimSendQueueItem[] GetQueueList(List<long> listClaimNums,long clinicNum,long customTracking) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<ClaimSendQueueItem[]>(MethodBase.GetCurrentMethod(),listClaimNums,clinicNum,customTracking);
			}
			List<string> listWhereAnds=new List<string>();
			if(listClaimNums.Count==0) {
				listWhereAnds.Add("claim.ClaimStatus IN ('W','P') ");
			}
			else {
				listWhereAnds.Add("claim.ClaimNum IN ("+string.Join(",",listClaimNums)+") ");
			}
			if(clinicNum>0) {
				listWhereAnds.Add("claim.ClinicNum="+POut.Long(clinicNum)+" ");
			}
			if(customTracking>0) {
				listWhereAnds.Add("claim.CustomTracking="+POut.Long(customTracking)+" ");
			}
			string icd9Command=@"IFNULL((SELECT COUNT(*)
						FROM procedurelog
						INNER JOIN claimproc ON claimproc.ProcNum=procedurelog.ProcNum
						WHERE procedurelog.IcdVersion=9
						AND (procedurelog.DiagnosticCode!='' OR procedurelog.DiagnosticCode2!='' OR procedurelog.DiagnosticCode3!='' 
							OR procedurelog.DiagnosticCode4!='')
						AND claimproc.ClaimNum=claim.ClaimNum
						),FALSE) HasIcd9 ";
			string command=$@"SELECT claim.ClaimNum,carrier.NoSendElect,claim.ClaimStatus,carrier.CarrierName,patient.PatNum,carrier.ElectID,claim.MedType,
				claim.DateService,claim.ClinicNum,claim.CustomTracking,claim.ProvTreat,claim.SecDateTEdit,
				{DbHelper.Concat("patient.LName","', '","patient.FName","IF(patient.MiddleI='','',CONCAT(' ',patient.MiddleI))")} patName,
				(SELECT MIN(patplan.Ordinal) FROM patplan WHERE patplan.PatNum=claim.PatNum AND patplan.InsSubNum=claim.InsSubNum) Ordinal,
				{icd9Command}
				FROM claim
				LEFT JOIN patient ON patient.PatNum=claim.PatNum
				LEFT JOIN insplan ON claim.PlanNum=insplan.PlanNum
				LEFT JOIN carrier ON insplan.CarrierNum=carrier.CarrierNum
				WHERE {string.Join("AND ",listWhereAnds)}
				ORDER BY claim.DateService,patient.LName,patient.FName";
			Dictionary<long,DataRow> dictClaimRows=Db.GetTable(command).Select().GroupBy(x => PIn.Long(x["ClaimNum"].ToString())).ToDictionary(x => x.Key,x => x.First());
			List<ClaimProc> listClaimProcs=ClaimProcs.RefreshForClaims(dictClaimRows.Keys.ToList());
			Dictionary<long,string> dictProcCodeStrs=Procedures.GetProcsFromClaimProcs(listClaimProcs)
				.ToDictionary(x => x.ProcNum,x => ProcedureCodes.GetStringProcCode(x.CodeNum));
			Dictionary<long,string> dictProcCodeStrsPerClaim=listClaimProcs
				.Where(x => dictProcCodeStrs.ContainsKey(x.ProcNum) && !string.IsNullOrEmpty(dictProcCodeStrs[x.ProcNum]))
				.GroupBy(x => x.ClaimNum,x => dictProcCodeStrs[x.ProcNum])
				.ToDictionary(x => x.Key,x => string.Join(", ",x));
			return dictClaimRows.Select(x => new ClaimSendQueueItem() { 
				ClaimNum						= x.Key,
				NoSendElect					= PIn.Enum<NoSendElectType>(x.Value["NoSendElect"].ToString()),
				Ordinal							= PIn.Int(x.Value["Ordinal"].ToString()),
				PatName							= PIn.String(x.Value["patName"].ToString()),
				ClaimStatus					= PIn.String(x.Value["ClaimStatus"].ToString()),
				Carrier							= PIn.String(x.Value["CarrierName"].ToString()),
				PatNum							= PIn.Long(x.Value["PatNum"].ToString()),
				MedType							= PIn.Enum<EnumClaimMedType>(x.Value["MedType"].ToString()),
				ClearinghouseNum		= Clearinghouses.AutomateClearinghouseHqSelection(PIn.String(x.Value["ElectID"].ToString()),
					PIn.Enum<EnumClaimMedType>(x.Value["MedType"].ToString())),
				DateService					= PIn.Date(x.Value["DateService"].ToString()),
				ClinicNum						= PIn.Long(x.Value["ClinicNum"].ToString()),
				CustomTracking			= PIn.Long(x.Value["CustomTracking"].ToString()),
				HasIcd9							= PIn.Bool(x.Value["HasIcd9"].ToString()),
				ProvTreat						= PIn.Long(x.Value["ProvTreat"].ToString()),
				ProcedureCodeString = dictProcCodeStrsPerClaim.TryGetValue(x.Key,out string procCodeStr)?procCodeStr:"",
				SecDateTEdit				= PIn.Date(x.Value["SecDateTEdit"].ToString())
			}).ToArray();
		}

		///<summary>Supply claimnums. Called from X12 to begin the sorting process on claims going to one clearinghouse.</summary>
		public static List<X12TransactionItem> GetX12TransactionInfo(long claimNum) {
			//No need to check RemotingRole; no call to db.
			List<long> claimNums=new List<long>();
			claimNums.Add(claimNum);
			return GetX12TransactionInfo(claimNums);
		}

		///<summary>Supply claimnums. Called from X12 to begin the sorting process on claims going to one clearinghouse.</summary>
		public static List<X12TransactionItem> GetX12TransactionInfo(List<long> claimNums) {//ArrayList queueItemss){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<X12TransactionItem>>(MethodBase.GetCurrentMethod(),claimNums);
			}
			List<X12TransactionItem> retVal=new List<X12TransactionItem>();
			if(claimNums.Count<1) {
				return retVal;
			}
			string command;
			command="SELECT carrier.ElectID,claim.ProvBill,inssub.Subscriber,"
				+"claim.PatNum,claim.ClaimNum,CASE WHEN inssub.Subscriber!=claim.PatNum THEN 1 ELSE 0 END AS subscNotPatient "
				+"FROM claim,insplan,inssub,carrier "
				+"WHERE claim.PlanNum=insplan.PlanNum "
				+"AND claim.InsSubNum=inssub.InsSubNum "
				+"AND carrier.CarrierNum=insplan.CarrierNum "
				+"AND claim.ClaimNum IN ("+String.Join(",",claimNums)+") "
				+"ORDER BY carrier.ElectID,claim.ProvBill,inssub.Subscriber,subscNotPatient,claim.PatNum";
			DataTable table=Db.GetTable(command);
			//object[,] myA=new object[5,table.Rows.Count];
			X12TransactionItem item;
			for(int i=0;i<table.Rows.Count;i++){
				item=new X12TransactionItem();
				item.PayorId0=PIn.String(table.Rows[i][0].ToString());
				item.ProvBill1=PIn.Long   (table.Rows[i][1].ToString());
				item.Subscriber2=PIn.Long   (table.Rows[i][2].ToString());
				item.PatNum3=PIn.Long   (table.Rows[i][3].ToString());
				item.ClaimNum4=PIn.Long   (table.Rows[i][4].ToString());
				retVal.Add(item);
			}
			return retVal;
		}

		///<summary>Also sets the DateSent to today.</summary>
		public static void SetClaimSent(long claimNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),claimNum);
				return;
			}
			DateTime dateT=MiscData.GetNowDateTime();
			string command="UPDATE claim SET ClaimStatus = 'S',"
				+"DateSent="+POut.Date(dateT)+", "
				+"DateSentOrig=(CASE WHEN DateSentOrig='0001-01-01' THEN "+POut.Date(dateT)+" ELSE DateSentOrig END) "
				+"WHERE ClaimNum = "+POut.Long(claimNum);
			Db.NonQ(command);
		}

		public static bool IsClaimIdentifierInUse(string claimIdentifier,long claimNumExclude,string claimType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),claimIdentifier,claimNumExclude,claimType);
			}
			string command="SELECT COUNT(*) FROM claim WHERE ClaimIdentifier='"+POut.String(claimIdentifier)+"' AND ClaimNum<>"+POut.Long(claimNumExclude);
			if(claimType=="PreAuth") {
				command+=" AND ClaimType='PreAuth'";
			}
			else {
				command+=" AND ClaimType!='PreAuth'";
			}
			return (Db.GetTable(command).Rows[0][0].ToString()!="0");
		}

		public static bool IsClaimPreAuth(Claim claim) {
			//No need to check RemotingRole; no call to db.
			return claim!=null && claim.ClaimType=="PreAuth";
		}

		public static bool IsReferralAttached(long referralNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),referralNum);
			}
			string command="SELECT COUNT(*) FROM claim WHERE OrderingReferralNum="+POut.Long(referralNum);
 			if(Db.GetCount(command)=="0") {
				return false;
			}
			return true;
		}

		///<summary>Returns a list of claimnums matching the list of x12claims given.
		///The returned list is always same length as the list of x12claims, unless there is an error, in which case null is returned.
		///If a claim in the database is not found for a specific x12claim, then a value of 0 will be placed into the return list for that x12claim.
		///Each matched claim will either begin with the specified claimIdentifier, or will be for the patient name and subscriber ID specified.</summary>
		public static List <long> GetClaimFromX12(List <X12ClaimMatch> listX12claims) {
			if(listX12claims.Count==0) {
				return null;
			}
			#region Either get a list of dates for given X12ClaimMatches, or a dateMin and dateMax.
			List<DateTime> listDateTimes=new List<DateTime>();
			DateTime dateMin=DateTime.MinValue;
			DateTime dateMax=DateTime.MinValue;
			if(PrefC.GetBool(PrefName.EraStrictClaimMatching)) {
				for(int i=0;i<listX12claims.Count;i++) {
					listDateTimes.AddRange(MiscUtils.GetDatesInRange(listX12claims[i].DateServiceStart,listX12claims[i].DateServiceEnd));
					for(int j=0;j<listX12claims[i].List835Procs.Count;j++) {
						listDateTimes.AddRange(MiscUtils.GetDatesInRange(listX12claims[i].List835Procs[j].DateServiceStart,listX12claims[i].List835Procs[j].DateServiceEnd));
					}
				}
				//Carrier will send 01/01/1900 as a default date instead of MinVal or omission.
				listDateTimes=listDateTimes.Distinct().Where(x => x.Year>1900).ToList();
				//If there are no dates to consider, return early.
				if(listDateTimes.Count==0) {
					return null;
				}
			}
			else {
				//Usually claims from the same ERA will all have dates of service within a few weeks of each other.
				if(listX12claims.Where(x => x.DateServiceStart.Year>1900).Count() > 0) {
					dateMin=listX12claims.Where(x => x.DateServiceStart.Year>1900).Select(x => x.DateServiceStart).Min();//DateServiceStart can be 1900 for PreAuths.
				}
				if(listX12claims.Where(x => x.DateServiceEnd.Year>1900).Count() > 0) {
					dateMax=listX12claims.Where(x => x.DateServiceEnd.Year>1900).Select(x => x.DateServiceEnd).Max();//DateServiceEnd can be 1900 for PreAuths.
				}
				if(dateMin.Year<1880 || dateMax.Year<1880) {
					//Service dates are required for us to continue.
					//In 227s, the claim dates of service are required and should be present.
					//In 835s, we pull the procedure dates up into the claim dates of service if the claim dates are of service are not present.
					return null;
				}
			}
			#endregion
			#region Construct etrans/835 dictionary.  Use for matching loop and internal claim query.
			//Since listX12claims can contain values from different etrans/835s we want to run matching logic per each 835.
			//We have seen 835s contain split claims (one procedure per 835 claim in many cases) so we group by claimIdentifier to calculate their claimFees.
			//This allows us to more accurately match all grouped 835 claims in one pass for split claims.
			//Dictionary such that:
			//Key => EtransNum
			//Value => Dictionary such that: key => ClaimIdentifier and value => list of X12ClaimMatches.
			Dictionary<long,Dictionary<string,List<X12ClaimMatch>>> dictMatchesPerClaimId=listX12claims
				.GroupBy(x => x.EtransNum)
				.ToDictionary(
					x => x.Key,//EtransNum
					x => x.GroupBy(y => y.ClaimIdentifier)
						.ToDictionary(
							y => y.Key,//ClaimIdentifier
							y => y.ToList()//List of X12ClaimMatches
						)
				);
			#endregion
			#region Get a list of fees for given X12ClaimMatches.  Claims in DB must match these because that is how we do initial matching.
			Dictionary<long,Dictionary<string,double>> dictTotalClaimFee=new Dictionary<long,Dictionary<string,double>>();
			List<double> listTotalClaimFees=new List<double>();
			foreach(long etransNum in dictMatchesPerClaimId.Keys) {
				dictTotalClaimFee[etransNum]=new Dictionary<string,double>();
				foreach(string claimIdentifier in dictMatchesPerClaimId[etransNum].Keys) {
					double claimFee=dictMatchesPerClaimId[etransNum][claimIdentifier]
						.Sum(x => (x.Is835Reversal?0:x.ClaimFee));//Ignore claim reversals, because they negate the original claim fee.
					dictTotalClaimFee[etransNum][claimIdentifier]=claimFee;
					if(!listTotalClaimFees.Contains(claimFee)) {
						listTotalClaimFees.Add(claimFee);
					}
				}
			}
			#endregion
			#region Get List of Claims For Date and Fee Ranges
			Dictionary<DateTime,List<DataRow>> dictClaims;
			if(PrefC.GetBool(PrefName.EraStrictClaimMatching)) {
				dictClaims=GetClaimTable(listDateTimes,listTotalClaimFees).Select()
					.GroupBy(x => PIn.Date(x["DateService"].ToString()))
					.ToDictionary(x => x.Key,x => x.ToList());
			}
			else {
				dictClaims=GetClaimTable(dateMin,dateMax,listTotalClaimFees).Select()
					.GroupBy(x => PIn.Date(x["DateService"].ToString()))
					.ToDictionary(x => x.Key,x => x.ToList());
			}
			#endregion
			#region Get claimProcs for given 835 procNums that are associated to a claim.
			List<long> listAllEraProcNums=dictMatchesPerClaimId
				.SelectMany(x => x.Value.SelectMany(y => y.Value))//x.Value => Dictionary<string,List<X12ClaimMatch>> to one big List<X12ClaimMatch>
				.SelectMany(y => y.List835Procs.Select(z => z.ProcNum)).Distinct().ToList();//List<X12ClaimMatch> to List<ProcNums>
			List<ClaimProcStatus> listClaimProcStatuses=new List<ClaimProcStatus>();//ClaimProcStatuses that have procNums.
			listClaimProcStatuses.Add(ClaimProcStatus.NotReceived);
			listClaimProcStatuses.Add(ClaimProcStatus.Received);
			listClaimProcStatuses.Add(ClaimProcStatus.Preauth);
			listClaimProcStatuses.Add(ClaimProcStatus.CapClaim);
			listClaimProcStatuses.Add(ClaimProcStatus.CapComplete);
			List<ClaimProc> listAllClaimProcs=ClaimProcs.GetForProcs(listAllEraProcNums,listClaimProcStatuses);//Only runs query if procNumList not empty
			List<ClaimProc> listAccountClaimProcs=listAllClaimProcs.Where(x => x.Status!=ClaimProcStatus.Preauth).ToList();
			List<ClaimProc> listTreatPlanClaimProcs=listAllClaimProcs.Where(x => x.Status==ClaimProcStatus.Preauth).ToList();
			List<PatPlan> listPatPlans=PatPlans.GetListByInsSubNums(listAllClaimProcs.Select(x => x.InsSubNum).ToList());//Only runs query if list contains items.
			#endregion
			List <long> listClaimNums=new List<long>(new long[listX12claims.Count]);//Done this way to guarantee that each claimnum is initialized to 0.
			//For each provided etrans, we look at 1 group such that the key is the claimIdentifier and the value is the list of all claim matches assocaited to the claimIdentifier.
			//This means that each entry in the list of claim matches should share many fields like, claimIdentifier, patient FName, patient LName and subscriber ID.
			foreach(long etransNum in dictMatchesPerClaimId.Keys) {//Consider a single etrans at a time.
				foreach(string claimIdentifier in dictMatchesPerClaimId[etransNum].Keys) {//Claims that are split by procedure from the carrier's side are grouped together by claimIdentifier above.
					X12ClaimMatch xclaim=dictMatchesPerClaimId[etransNum][claimIdentifier].First();//Just use the first 835 claim to try and match because all fields we use should be identical. 
					List<long> listEraProcNums=dictMatchesPerClaimId[etransNum][claimIdentifier].SelectMany(x => x.List835Procs.Select(y => y.ProcNum)).ToList();//All identified procNums reported from 835.
					//Begin with basic filtering by date of service and claim total fee.
					List <DataRow> listDbClaims=new List<DataRow>();
					foreach(DateTime d in dictClaims.Keys.Where(x => x>=xclaim.DateServiceStart && x<=xclaim.DateServiceEnd)) {
						listDbClaims.AddRange(dictClaims[d].FindAll(x => PIn.Double(x["ClaimFee"].ToString())==dictTotalClaimFee[etransNum][claimIdentifier]));
					}
					#region 835 ProcNum matching in conjunction with PlanNum and Ordinal matching.  Helps distinctly identify primary vs secondary claims.
					if(xclaim.List835Procs.Count>0 && ListTools.In(xclaim.List835Procs.First().MatchingVersion,EraProcMatchingFormat.X,EraProcMatchingFormat.Y)) {
						Hx835_Proc eraProc=xclaim.List835Procs.First();//PlanNum and Ordinal should be the same for all procs.
						Dictionary<long,List<ClaimProc>> dictClaimProcs=null;
						switch(eraProc.MatchingVersion) {
							case EraProcMatchingFormat.X:
								//Matching 'x(procNum)/(ordinal)/(InsPlan.planNum)' format.
								dictClaimProcs=listAllClaimProcs//Keys are ClaimNum from database and values are list of claimprocs for claim.
									.Where(x => listEraProcNums.Contains(x.ProcNum)
										&& x.PlanNum==eraProc.PlanNum
										&& PatPlans.GetOrdinal(x.InsSubNum,listPatPlans.FindAll(y => y.PatNum==x.PatNum))==eraProc.PlanOrdinal
									)//List of claimProcs matched by ProcNum, PlanNum and ordinal.
									.GroupBy(x => x.ClaimNum)//All claimProcs on the same claim should share pertinent fields.
									.ToDictionary(x => x.Key,y => y.ToList()
								);
								break;
							case EraProcMatchingFormat.Y:
								//Matching 'y(procNum)/(ordinal)/(partial InsPlan.planNum)' format.
								dictClaimProcs=listAllClaimProcs//Keys are ClaimNum from database and values are list of claimprocs for claim.
									.Where(x => listEraProcNums.Contains(x.ProcNum)
										&& x.PlanNum.ToString().EndsWith(eraProc.PartialPlanNum.ToString())
										&& PatPlans.GetOrdinal(x.InsSubNum,listPatPlans.FindAll(y => y.PatNum==x.PatNum))==eraProc.PlanOrdinal
									)//List of claimProcs matched by ProcNum, partial PlanNum and ordinal.
									.GroupBy(x => x.ClaimNum)//All claimProcs on the same claim should share pertinent fields.
									.ToDictionary(x => x.Key,y => y.ToList()
								);
								break;
						}
						if(dictClaimProcs.Count==1) {//Single claim match found.
							foreach(X12ClaimMatch match in dictMatchesPerClaimId[etransNum][claimIdentifier]) {
								int index=listX12claims.IndexOf(match);
								listClaimNums[index]=dictClaimProcs.Keys.First();
							}
							continue;//Move to the next claim.
						}
						else {
							//Either multiple matches or no matches. Either way consider other matching logic.
						}
					}
					#endregion
					List<int> listIndiciesForIdentifier=new List<int>();//Stores indicies from listDbCLaims where we find a matching claimIdentifier.
					#region 835 ProcNum matching in conjunction with ClaimIdentifier matching.  Helps distinctly identify primary vs secondary claims.
					if(xclaim.ClaimIdentifier.Length>0 && xclaim.ClaimIdentifier!="0") {
						//The following dicitonary is constructed so that we can compare the 835s procNums to internal claims.
						//Previously we have seen that the CLP02 flag matching (see below) isn't always trustworthy so we want to first consider exact 835s ClaimIdentifier matches.
						//So we construct a list of claimProcs for the given 835 proc nums and group them by claimNum since we really just care about the claim.
						Dictionary<long,List<ClaimProc>> dictClaimProcs=listAllClaimProcs.FindAll(x => listEraProcNums.Contains(x.ProcNum))
							.GroupBy(x => x.ClaimNum)
							.ToDictionary(x => x.Key,y => y.ToList()
						);
						foreach(long cpClaimNum in dictClaimProcs.Keys) {
							for(int i=0;i<listDbClaims.Count;i++) {
								if(cpClaimNum!=PIn.Long(listDbClaims[i]["ClaimNum"].ToString())//Not the claim we are looking for.
									|| xclaim.ClaimIdentifier!=PIn.String(listDbClaims[i]["ClaimIdentifier"].ToString()))//Correct claim, but wrong claimIdentifier.
								{
									continue;
								}
								listIndiciesForIdentifier.Add(i);//Match based on claim identifier, claim date of service, claim fee and given procNums.
								if(listIndiciesForIdentifier.Count>1) {//don't need to continue looping if we find more than 1
									break;
								}
							}
							if(listIndiciesForIdentifier.Count>1) {
								break;
							}
						}
						if(listIndiciesForIdentifier.Count==1) {//A single match based on claim identifier, claim date of service, claim fee and given procNums.
							long claimNum=PIn.Long(listDbClaims[listIndiciesForIdentifier[0]]["ClaimNum"].ToString());
							foreach(X12ClaimMatch match in dictMatchesPerClaimId[etransNum][claimIdentifier]) {
								int index=listX12claims.IndexOf(match);
								listClaimNums[index]=claimNum;
							}
							continue;//Move to the next claim.
						}
					}
					#endregion
					#region 835 ProcNum and CLP02 matching and setting.
					List<ClaimProc> listClaimProcs=new List<ClaimProc>();
					switch(xclaim.CodeClp02) {
						case "1"://"Processed as Primary"
						case "19"://"Processed as Primary, Forwarded to Additional Payer(s)"
							listClaimProcs=ClaimProcs.GetForProcsWithOrdinalFromList(listEraProcNums,1,listPatPlans,listAccountClaimProcs);
							break;
						case "2"://"Processed as Secondary"
						case "20"://"Processed as Secondary, Forwarded to Additional Payer(s)"
							listClaimProcs=ClaimProcs.GetForProcsWithOrdinalFromList(listEraProcNums,2,listPatPlans,listAccountClaimProcs);
							break;
						case "3"://"Processed as Tertiary"
						case "21"://"Processed as Tertiary, Forwarded to Additional Payer(s)"
							listClaimProcs=ClaimProcs.GetForProcsWithOrdinalFromList(listEraProcNums,3,listPatPlans,listAccountClaimProcs);
							break;
						case "4"://"Denied"
						case "22"://"Reversal of Previous Payment"
						case "23"://"Not Our Claim, Forwarded to Additional Payer(s)"
							//The odds of all the claim nums matching here is lower, because we could match both primary and secondary.
							listClaimProcs=listAccountClaimProcs.Where(x => listEraProcNums.Contains(x.ProcNum)).ToList();
							break;
						case "25"://"Predetermination Pricing Only - No Payment"
							listClaimProcs=listTreatPlanClaimProcs.Where(x => listEraProcNums.Contains(x.ProcNum)).ToList();
							break;
					}
					if(listClaimProcs.Count>0) {//Successfully found internal claimProcs.
						long claimNumKey=listClaimProcs.First().ClaimNum;
						if(listClaimProcs.All(x => x.ClaimNum==claimNumKey)) {//All claimNums must match.
							foreach(X12ClaimMatch match in dictMatchesPerClaimId[etransNum][claimIdentifier]) {
								int index=listX12claims.IndexOf(match);
								listClaimNums[index]=claimNumKey;
							}
							continue;//Move to the next claim.
						}
					}
					#endregion
					#region ClaimIdentifier matching and setting.
					//Look for claim matched by full or partial claim identifier.
					listIndiciesForIdentifier=new List<int>();
					if(xclaim.ClaimIdentifier.Length > 0 && xclaim.ClaimIdentifier!="0") {//Ensure an ID is present and that it is not for a printed claim (when ID=="0").
						//Look for a single exact match by claim identifier.  This step is first, so that the user can override claim association to the 835 or 277 by changing the claim identifier if desired.
						for(int i=0;i<listDbClaims.Count;i++) {
							string claimId=PIn.String(listDbClaims[i]["ClaimIdentifier"].ToString());
							if(claimId==xclaim.ClaimIdentifier) {
								listIndiciesForIdentifier.Add(i);
							}
						}
						if(listIndiciesForIdentifier.Count==0 && xclaim.ClaimIdentifier.Length>15) {//No exact match found.  Look for similar claim identifiers if the identifer was possibly truncated when sent out.
							//Our claim identifiers can be longer than 20 characters (mostly when using replication). When the claim identifier is sent out on the claim, it is truncated to 20
							//characters. Therefore, if the claim identifier is longer than 20 characters, then it was truncated when sent out, so we have to look for claims beginning with the 
							//claim identifier given if there is not an exact match.  We also send shorter identifiers for some clearinghouses.  For example, the maximum claim identifier length
							//for Denti-Cal is 17 characters.
							for(int i=0;i<listDbClaims.Count;i++) {
								string claimId=PIn.String(listDbClaims[i]["ClaimIdentifier"].ToString());
								if(claimId.StartsWith(xclaim.ClaimIdentifier)) {
									listIndiciesForIdentifier.Add(i);
								}
							}
						}
					}
					if(listIndiciesForIdentifier.Count==0) {
						//No matches were found for the identifier.  Continue to more advanced matching below.
					}
					else if(listIndiciesForIdentifier.Count==1) {
						//A single match based on claim identifier, claim date of service, and claim fee.
						long claimNum=PIn.Long(listDbClaims[listIndiciesForIdentifier[0]]["ClaimNum"].ToString());
						foreach(X12ClaimMatch match in dictMatchesPerClaimId[etransNum][claimIdentifier]) {
							int index=listX12claims.IndexOf(match);
							listClaimNums[index]=claimNum;
						}
						continue;//Move to the next claim.
					}
					else if(listIndiciesForIdentifier.Count>1) {//Edge case.
						//Multiple matches for the specified claim identifier AND date service AND fee.  The claim must have been split (rare because the split claims must have the same fee).
						//Continue to more advanced matching below, although it probably will not help.  We could enhance this specific scenario by picking a claim based on the procedures attached, but that is not a guarantee either.
					}
					#endregion
					#region Patient LName and FName (exact/partial) matching.
					//Locate claims exactly matching patient last name.
					List<DataRow> listMatches=new List<DataRow>();
					string patLname=xclaim.PatLname.Trim().ToLower();
					for(int i=0;i<listDbClaims.Count;i++) {
						string lastNameInDb=PIn.String(listDbClaims[i]["LName"].ToString()).Trim().ToLower();
						if(lastNameInDb==patLname) {
							listMatches.Add(listDbClaims[i]);
						}
					}
					//Locate claims matching exact first name or partial first name, with a preference for exact match.
					List<DataRow> listExactFirst=new List<DataRow>();
					List<DataRow> listPartFirst=new List<DataRow>();
					string patFname=xclaim.PatFname.Trim().ToLower();
					for(int i=0;i<listMatches.Count;i++) {
						string firstNameInDb=PIn.String(listMatches[i]["FName"].ToString()).Trim().ToLower();
						if(firstNameInDb==patFname) {
							listExactFirst.Add(listMatches[i]);
						}
						else if(firstNameInDb.Length>=2 && patFname.StartsWith(firstNameInDb)) {
							//Unfortunately, in the real world, we have observed carriers returning the patients first name followed by a space followed by the patient middle name all within the first name field.
							//This issue is probably due to human error when the carrier's staff typed the patient name into their system.  All we can do is try to cope with this situation.
							listPartFirst.Add(listMatches[i]);
						}
					}
					if(listExactFirst.Count>0) {
						listMatches=listExactFirst;//One or more exact matches found.  Ignore any partial matches.
					}
					else {
						listMatches=listPartFirst;//Use partial matches only if no exact matches were found.
					}
					#endregion
					#region SubscriberID (exact/partial) matching.
					//Locate claims matching exact subscriber ID or partial subscriber ID, with a preference for exact match.
					List<DataRow> listExactId=new List<DataRow>();
					List<DataRow> listPartId=new List<DataRow>();
					string subscriberId=xclaim.SubscriberId.Trim().ToUpper();
					for(int i=0;i<listMatches.Count;i++) {
						string subIdInDb=PIn.String(listMatches[i]["SubscriberID"].ToString()).Trim().ToUpper();
						if(subIdInDb==subscriberId) {
							listExactId.Add(listMatches[i]);
						}
						else if(subIdInDb.Length>=3 && (subscriberId==subIdInDb.Substring(0,subIdInDb.Length-1) || subscriberId==subIdInDb.Substring(0,subIdInDb.Length-2))) {
							//Partial subscriber ID matches are somewhat common.
							//Insurance companies sometimes create a base subscriber ID for all family members, then append a one or two digit number to make IDs unique for each family member.
							//We have seen at least one real world example where the ERA contained the base subscriber ID instead of the patient specific ID.
							//We also check that the subscriber ID in OD is at least 3 characters long, because we must allow for the 2 optional ending characters and we require an extra leading character to avoid matching blank IDs.
							listPartId.Add(listMatches[i]);
						}
						else if(subscriberId.Length>=3 && (subIdInDb==subscriberId.Substring(0,subscriberId.Length-1) || subIdInDb==subscriberId.Substring(0,subscriberId.Length-2))) {
							//Partial match in the other direction.  Comparable to the scenario above.
							listPartId.Add(listMatches[i]);
						}
						else if(subscriberId.Length >= 3 && subIdInDb.TrimStart('0')==subscriberId) {
							//Allow matches for leading zeros.
							listPartId.Add(listMatches[i]);
						}
						else if(subIdInDb.Length >= 3 && subscriberId.TrimStart('0')==subIdInDb) {
							//Allow matches for leading zeros.
							listPartId.Add(listMatches[i]);
						}
					}
					if(listExactId.Count>0) {
						listMatches=listExactId;//One or more exact matches found.  Ignore any partial matches.
					}
					else {
						listMatches=listPartId;//Use partial matches only if no exact matches were found.
					}
					#endregion
					long matchClaimNum=0;
					//We have finished locating the matches.  Now decide what to do based on the number of matches found.
					if(listMatches.Count==0) {
						matchClaimNum=0;
					}
					else if(listMatches.Count==1) {
						//A single match based on patient first name, patient last name, subscriber ID, claim date of service, and claim fee.
						matchClaimNum=PIn.Long(listMatches[0]["ClaimNum"].ToString());
					}
					else if(listMatches.Count>1) {//Edge case.
						//Multiple matches (rare).  We might be able to pick the correct claim based on the attached procedures, but we can worry about this situation later if it happens more than we expect.
						matchClaimNum=0;
					}
					foreach(X12ClaimMatch match in dictMatchesPerClaimId[etransNum][claimIdentifier]) {
						int index=listX12claims.IndexOf(match);
						listClaimNums[index]=matchClaimNum;
					}
				}//end foreach claim identifier
			}//end foreach etrans/835
			return listClaimNums;
		}

		///<summary>We always require the claim fee and dates of service to match, then we use additional criteria to wisely choose from the shorter list
		///of claims.  The list of claims with matching fee and date of service should be very short.  Worst case, the list would contain all of the
		///claims if every claim had the same fee (rare).</summary>
		public static DataTable GetClaimTable(List<DateTime> listDateTimes,List<double> listClaimFees) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),listDateTimes,listClaimFees);
			}
			string command=$@"SELECT claim.ClaimNum,claim.ClaimIdentifier,claim.ClaimStatus,ROUND(ClaimFee,2) ClaimFee,claim.DateService,patient.LName,
				patient.FName,inssub.SubscriberID
				FROM claim
				INNER JOIN patient ON patient.PatNum=claim.PatNum
				INNER JOIN inssub ON inssub.InsSubNum=claim.InsSubNum AND claim.PlanNum=inssub.PlanNum
				WHERE DATE(DateService) IN ({string.Join(",",listDateTimes.Select(x => POut.Date(x)))})
				AND ROUND(ClaimFee,2) IN ({string.Join(",",listClaimFees.Select(x => POut.Double(x)))})";
			return Db.GetTable(command);
		}

		///<summary>We always require the claim fee and dates of service to match, then we use additional criteria to wisely choose from the shorter list
		///of claims.  The list of claims with matching fee and date of service should be very short.  Worst case, the list would contain all of the
		///claims if every claim had the same fee (rare).</summary>
		public static DataTable GetClaimTable(DateTime dateMin,DateTime dateMax,List<double> listClaimFees) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateMin,dateMax,listClaimFees);
			}
			string command=$@"SELECT claim.ClaimNum,claim.ClaimIdentifier,claim.ClaimStatus,ROUND(ClaimFee,2) ClaimFee,claim.DateService,patient.LName,
				patient.FName,inssub.SubscriberID
				FROM claim
				INNER JOIN patient ON patient.PatNum=claim.PatNum
				INNER JOIN inssub ON inssub.InsSubNum=claim.InsSubNum AND claim.PlanNum=inssub.PlanNum
				WHERE {DbHelper.BetweenDates("DateService",dateMin,dateMax)}
				AND ROUND(ClaimFee,2) IN ({string.Join(",",listClaimFees.Select(x => POut.Double(x)))})";
			return Db.GetTable(command);
		}

		///<summary>Returns the number of received claims attached to specified insplan.</summary>
		public static int GetCountReceived(long planNum) {
			//No need to check RemotingRole; no call to db.
			return GetCountReceived(planNum,0);
		}

		///<summary>Returns the number of received claims attached to specified subscriber with specified insplan.  Set insSubNum to zero to check all claims for all patients for the plan.</summary>
		public static int GetCountReceived(long planNum,long insSubNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetInt(MethodBase.GetCurrentMethod(),planNum,insSubNum);
			}
			string command;
			command="SELECT COUNT(*) "
				+"FROM claim "
				+"WHERE claim.ClaimStatus='R' "
				+"AND claim.PlanNum="+POut.Long(planNum)+" ";
			if(insSubNum!=0) {
				command+="AND claim.InsSubNum="+POut.Long(insSubNum);
			}
			return PIn.Int(Db.GetCount(command));
		}

		///<summary>Returns a human readable ClaimStatus string.</summary>
		public static string GetClaimStatusString(string claimStatus) {
			string retVal="";
			switch(claimStatus){
				case "U":
					retVal="Unsent";
					break;
				case "H":
					retVal="Hold until Pri received";
					break;
				case "W":
					retVal="Waiting to Send";
					break;
				case "P":
					retVal="Probably Sent";
					break;
				case "S":
					retVal="Sent - Verified";
					break;
				case "R":
					retVal="Received";
					break;
			}
			return retVal;
		}

		///<summary>Updates ClaimIdentifier for specified claim.</summary>
		public static void UpdateClaimIdentifier(long claimNum,string claimIdentifier) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),claimNum,claimIdentifier);
				return;
			}
			string command="UPDATE claim SET ClaimIdentifier='"+POut.String(claimIdentifier)+"' WHERE ClaimNum="+POut.Long(claimNum);
			Db.NonQ(command);
		}
		
		///<summary>Updates all claimproc estimates and also updates claim totals to db. Must supply procList which includes all procedures that this 
		///claim is linked to. Will also need to refresh afterwards to see the results. 
		///If the Claim is "S" Sent or "R" Received, FeeBilled and ClaimFee will not be updated.</summary>
		public static void CalculateAndUpdate(List<Procedure> listProcedures,List <InsPlan> listInsPlans,Claim claimCur,List <PatPlan> patPlans,List <Benefit> benefitList,Patient patient,List<InsSub> subList){
			//No remoting role check; no call to db
			//we need more than just the claimprocs for this claim.
			//in order to run Procedures.ComputeEstimates, we need all claimprocs for all procedures attached to this claim
			List<ClaimProc> ClaimProcsAll=ClaimProcs.Refresh(claimCur.PatNum);
			List<ClaimProc> listClaimProcsForClaim=ClaimProcs.RefreshForClaim(claimCur.ClaimNum);//will be ordered by line number.
			List<SubstitutionLink> listSubstLinks=SubstitutionLinks.GetAllForPlans(listInsPlans);//from db.  If we don't do it here, it will get done in loops in Procedures.ComputeEstimates.
			List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>();
			bool isFeeBilledUpdateNeeded=!ListTools.In(claimCur.ClaimStatus,"R","S");//If claimCur is not received/sent then we can update the feeBilled/claimFee
			for(int i=0;i<listProcedures.Count;i++){
				listProcedureCodes.Add(ProcedureCodes.GetProcCode(listProcedures[i].CodeNum));
			}
			long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(patient.PatNum);
			List<Fee> listFees=Fees.GetListFromObjects(listProcedureCodes,listProcedures.Select(x=>x.MedicalCode).ToList(),listProcedures.Select(x=>x.ProvNum).ToList(),
				patient.PriProv,patient.SecProv,patient.FeeSched,listInsPlans,listProcedures.Select(x=>x.ClinicNum).ToList(),
				null,//appts not needed because not setting providers. We already have provs. 
				listSubstLinks,discountPlanNum);
			double claimFee=0;
			double dedApplied=0;
			double insPayEst=0;
			double insPayAmt=0;
			double writeoff=0;
			InsPlan plan=InsPlans.GetPlan(claimCur.PlanNum,listInsPlans);
			if(plan==null){
				return;
			}
			long patPlanNum=PatPlans.GetPatPlanNum(claimCur.InsSubNum,patPlans);
			//first loop handles totals for received items.
			for(int i=0;i<listClaimProcsForClaim.Count;i++){
				if(listClaimProcsForClaim[i].Status!=ClaimProcStatus.Received){
					continue;//disregard any status except Receieved.
				}
				claimFee+=listClaimProcsForClaim[i].FeeBilled;
				dedApplied+=listClaimProcsForClaim[i].DedApplied;
				insPayEst+=listClaimProcsForClaim[i].InsPayEst;
				insPayAmt+=listClaimProcsForClaim[i].InsPayAmt;
				writeoff+=listClaimProcsForClaim[i].WriteOff;
			}
			//loop again only for procs not received.
			//And for preauth.
			Procedure ProcCur;
			//InsPlan plan=InsPlans.GetPlan(claimCur.PlanNum,planList);
			List<ClaimProcHist> histList=ClaimProcs.GetHistList(claimCur.PatNum,benefitList,patPlans,listInsPlans,claimCur.ClaimNum,claimCur.DateService,subList);
			List<ClaimProc> claimProcListOld=new List<ClaimProc>();//make a copy
			for(int i=0;i<ClaimProcsAll.Count;i++) {
				claimProcListOld.Add(ClaimProcsAll[i].Copy());
			}
			List<ClaimProcHist> loopList=new List<ClaimProcHist>();
			//Get data for any OrthoCases that may be linked to procs in listProcedures
			List<OrthoProcLink> listOrthoProcLinksAll=new List<OrthoProcLink>();
			Dictionary<long,OrthoProcLink> dictOrthoProcLinksForProcList=new Dictionary<long,OrthoProcLink>();
			Dictionary<long,OrthoCase> dictOrthoCases=new Dictionary<long,OrthoCase>();
			Dictionary<long,OrthoSchedule> dictOrthoSchedules=new Dictionary<long,OrthoSchedule>();
			OrthoCases.GetDataForListProcs(ref listOrthoProcLinksAll,ref dictOrthoProcLinksForProcList,ref dictOrthoCases,ref dictOrthoSchedules,listProcedures);
			OrthoProcLink orthoProcLink=null;
			OrthoCase orthoCase=null;
			OrthoSchedule orthoSchedule=null;
			List<OrthoProcLink> listOrthoProcLinksForOrthoCase=null;
			for(int i=0;i<listClaimProcsForClaim.Count;i++) {//loop through each proc
				ProcCur=Procedures.GetProcFromList(listProcedures,listClaimProcsForClaim[i].ProcNum);
				//in order for ComputeEstimates to give accurate Writeoff when creating a claim, InsPayEst must be filled for the claimproc with status of NotReceived.
				//So, we must set it here.  We need to set it in the claimProcsAll list.  Find the matching one.
				for(int j=0;j<ClaimProcsAll.Count;j++){
					if(ClaimProcsAll[j].ClaimProcNum==listClaimProcsForClaim[i].ClaimProcNum){//same claimproc in a different list
						if(listClaimProcsForClaim[i].Status==ClaimProcStatus.NotReceived) {//ignores received, etc
							ClaimProcsAll[j].InsPayEst=ClaimProcs.GetInsEstTotal(ClaimProcsAll[j]);
						}
					}
				}
				//When this is the secondary claim, HistList includes the primary estimates, which is something we don't want because the primary calculations gets confused.
				//So, we must remove those bad entries from histList.
				for(int h=histList.Count-1;h>=0;h--) {//loop through the histList backwards
					if(histList[h].ProcNum!=ProcCur.ProcNum) {
						continue;//Makes sure we will only be excluding histList entries for procs on this claim.
					}
					//we already excluded this claimNum when getting the histList.
					if(histList[h].Status!=ClaimProcStatus.NotReceived) {
						continue;//The only ones that are a problem are the ones on the primary claim not received yet.
					}
					histList.RemoveAt(h);
				}
				OrthoCases.FillOrthoCaseObjectsForProc(ProcCur.ProcNum,ref orthoProcLink,ref orthoCase,ref orthoSchedule,
					ref listOrthoProcLinksForOrthoCase,dictOrthoProcLinksForProcList,dictOrthoCases,dictOrthoSchedules,listOrthoProcLinksAll);
				Procedures.ComputeEstimates(ProcCur,claimCur.PatNum,ref ClaimProcsAll,false,listInsPlans,patPlans,benefitList,histList,loopList,false,patient.Age
					,subList,listSubstLinks:listSubstLinks,listFees:listFees,
					orthoProcLink:orthoProcLink,orthoCase:orthoCase,orthoSchedule:orthoSchedule,listOrthoProcLinksForOrthoCase:listOrthoProcLinksForOrthoCase);
				//then, add this information to loopList so that the next procedure is aware of it.
				//Exclude preauths becase thier estimates would incorrectly add both NotRecieved and Preauth estimates when calculating limitations.
				List<ClaimProc> listClaimProcs=ClaimProcsAll.Where(x => x.ProcNum==ProcCur.ProcNum && x.Status!=ClaimProcStatus.Preauth).ToList();
				loopList.AddRange(ClaimProcs.GetHistForProc(listClaimProcs,ProcCur.ProcNum,ProcCur.CodeNum));
			}
			//save changes in the list to the database
			ClaimProcs.Synch(ref ClaimProcsAll,claimProcListOld);
			listClaimProcsForClaim=ClaimProcs.RefreshForClaim(claimCur.ClaimNum);
			//But ClaimProcsAll has not been refreshed.
			for(int i=0;i<listClaimProcsForClaim.Count;i++) {
				if(listClaimProcsForClaim[i].Status!=ClaimProcStatus.NotReceived
					&& listClaimProcsForClaim[i].Status!=ClaimProcStatus.Preauth
					&& listClaimProcsForClaim[i].Status!=ClaimProcStatus.CapClaim) {
					continue;
				}
				ProcCur=Procedures.GetProcFromList(listProcedures,listClaimProcsForClaim[i].ProcNum);
				if(ProcCur.ProcNum==0) {
					continue;//ignores payments, etc
				}
				//fee:
				if(isFeeBilledUpdateNeeded && plan.ClaimsUseUCR) {//use UCR for the provider of the procedure
					long provNum=ProcCur.ProvNum;
					if(provNum==0) {//if no prov set, then use practice default.
						provNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
					}
					Provider providerFirst=Providers.GetFirst();//Used in order to preserve old behavior...  If this fails, then old code would have failed.
					Provider provider=Providers.GetFirstOrDefault(x => x.ProvNum==provNum)??providerFirst;
					//get the fee based on code and prov fee sched
					double ppoFee=Fees.GetAmount0(ProcCur.CodeNum,provider.FeeSched,ProcCur.ClinicNum,provNum,listFees);
					double ucrFee=ProcCur.ProcFee;//Usual Customary and Regular (UCR) fee.  Also known as billed fee.
					if(ucrFee > ppoFee) {
						listClaimProcsForClaim[i].FeeBilled=ProcCur.Quantity*ucrFee;
					}
					else {
						listClaimProcsForClaim[i].FeeBilled=ProcCur.Quantity*ppoFee;
					}
				}
				else if(isFeeBilledUpdateNeeded) {//don't use ucr. Use the procedure fee instead.
					listClaimProcsForClaim[i].FeeBilled=ProcCur.ProcFeeTotal;
				}
				claimFee+=listClaimProcsForClaim[i].FeeBilled;
				if(claimCur.ClaimType=="PreAuth" || claimCur.ClaimType=="Cap" || (claimCur.ClaimType=="Other" && !plan.IsMedical)) {
					//12-18-2015 ==tg:  We added medical plans as an exclusion to the above logic.  In past versions Medical plans did not copy over values into
					//the claimproc InsPayEst, DedApplied, or Writeoff columns.  DG and I determined that for now this is acceptable.	 If we ever implement a 
					//medical claimtype in the future, or if there are issues with claims this will need to be changed.
					ClaimProcs.Update(listClaimProcsForClaim[i]);//only the fee gets calculated, the rest does not
					continue;
				}
				//ClaimProcs.ComputeBaseEst(ClaimProcsForClaim[i],ProcCur.ProcFee,ProcCur.ToothNum,ProcCur.CodeNum,plan,patPlanNum,benefitList,histList,loopList);
				listClaimProcsForClaim[i].InsPayEst=ClaimProcs.GetInsEstTotal(listClaimProcsForClaim[i]);//Yes, this is duplicated from further up.
				listClaimProcsForClaim[i].DedApplied=ClaimProcs.GetDedEst(listClaimProcsForClaim[i]);
				if(listClaimProcsForClaim[i].Status==ClaimProcStatus.NotReceived){//(vs preauth)
					listClaimProcsForClaim[i].WriteOff=ClaimProcs.GetWriteOffEstimate(listClaimProcsForClaim[i]);
					writeoff+=listClaimProcsForClaim[i].WriteOff;
					/*
					ClaimProcsForClaim[i].WriteOff=0;
					if(claimCur.ClaimType=="P" && plan.PlanType=="p") {//Primary && PPO
						double insplanAllowed=Fees.GetAmount(ProcCur.CodeNum,plan.FeeSched);
						if(insplanAllowed!=-1) {
							ClaimProcsForClaim[i].WriteOff=ProcCur.ProcFee-insplanAllowed;
						}
						//else, if -1 fee not found, then do not show a writeoff. User can change writeoff if they disagree.
					}
					writeoff+=ClaimProcsForClaim[i].WriteOff;*/
				}
				dedApplied+=listClaimProcsForClaim[i].DedApplied;
				insPayEst+=listClaimProcsForClaim[i].InsPayEst;
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && listProcedures.Exists(x => x.ProcNumLab==listClaimProcsForClaim[i].ProcNum)) {
					//In Canada we will need to consider lab insurance estimates.
					List<long> listLabProcNums=listProcedures.FindAll(x => x.ProcNumLab==listClaimProcsForClaim[i].ProcNum).Select(x => x.ProcNum).ToList();
					insPayEst+=ClaimProcsAll.FindAll(x => listLabProcNums.Contains(x.ProcNum) 
							&& x.InsSubNum==listClaimProcsForClaim[i].InsSubNum && x.PlanNum==listClaimProcsForClaim[i].PlanNum)
						.Sum(x => ClaimProcs.GetInsEstTotal(x));
				}
				ClaimProcs.Update(listClaimProcsForClaim[i]);
				//but notice that the ClaimProcs lists are not refreshed until the loop is finished.
			}//for claimprocs.forclaim
			if(isFeeBilledUpdateNeeded){ //if claimStatus is sent or recieved, don't update. This may be unneccessary but it doesn't hurt anything and it makes make our intentions clear
				claimCur.ClaimFee=claimFee;
			}
			claimCur.DedApplied=dedApplied;
			claimCur.InsPayEst=insPayEst;
			claimCur.InsPayAmt=insPayAmt;
			claimCur.WriteOff=writeoff;
			//Cur=ClaimCur;
			Claims.Update(claimCur);
		}

		///<summary>Creates a claim for a newly created repeat charge procedure.</summary>
		public static Claim CreateClaimForRepeatCharge(string claimType,List<PatPlan> patPlanList,List<InsPlan> planList,List<ClaimProc> claimProcList,
			Procedure proc,List<InsSub> subList,Patient pat) {
			//No remoting role check; no call to db
			long claimFormNum=0;
			InsPlan planCur=new InsPlan();
			InsSub subCur=new InsSub();
			Relat relatOther=Relat.Self;
			switch(claimType) {
				case "P":
					subCur=InsSubs.GetSub(PatPlans.GetInsSubNum(patPlanList,PatPlans.GetOrdinal(PriSecMed.Primary,patPlanList,planList,subList)),subList);
					planCur=InsPlans.GetPlan(subCur.PlanNum,planList);
					break;
				case "S":
					subCur=InsSubs.GetSub(PatPlans.GetInsSubNum(patPlanList,PatPlans.GetOrdinal(PriSecMed.Secondary,patPlanList,planList,subList)),subList);
					planCur=InsPlans.GetPlan(subCur.PlanNum,planList);
					break;
				case "Med":
					//It's already been verified that a med plan exists
					subCur=InsSubs.GetSub(PatPlans.GetInsSubNum(patPlanList,PatPlans.GetOrdinal(PriSecMed.Medical,patPlanList,planList,subList)),subList);
					planCur=InsPlans.GetPlan(subCur.PlanNum,planList);
					break;
			}
			ClaimProc claimProcCur=Procedures.GetClaimProcEstimate(proc.ProcNum,claimProcList,planCur,subCur.InsSubNum);
			if(claimProcCur==null) {
				claimProcCur=new ClaimProc();
				ClaimProcs.CreateEst(claimProcCur,proc,planCur,subCur);
			}
			Claim claimCur=new Claim();
			claimCur.PatNum=proc.PatNum;
			claimCur.DateService=proc.ProcDate;
			claimCur.ClinicNum=proc.ClinicNum;
			claimCur.PlaceService=proc.PlaceService;
			claimCur.ClaimStatus="W";
			claimCur.DateSent=DateTime.Today;
			claimCur.DateSentOrig=DateTime.MinValue;
			claimCur.PlanNum=planCur.PlanNum;
			claimCur.InsSubNum=subCur.InsSubNum;
			InsSub sub;
			switch(claimType) {
				case "P":
					claimCur.PatRelat=PatPlans.GetRelat(patPlanList,PatPlans.GetOrdinal(PriSecMed.Primary,patPlanList,planList,subList));
					claimCur.ClaimType="P";
					claimCur.InsSubNum2=PatPlans.GetInsSubNum(patPlanList,PatPlans.GetOrdinal(PriSecMed.Secondary,patPlanList,planList,subList));
					sub=InsSubs.GetSub(claimCur.InsSubNum2,subList);
					if(sub.PlanNum>0 && InsPlans.RefreshOne(sub.PlanNum).IsMedical) {
						claimCur.PlanNum2=0;//no sec ins
						claimCur.PatRelat2=Relat.Self;
					}
					else {
						claimCur.PlanNum2=sub.PlanNum;//might be 0 if no sec ins
						claimCur.PatRelat2=PatPlans.GetRelat(patPlanList,PatPlans.GetOrdinal(PriSecMed.Secondary,patPlanList,planList,subList));
					}
					break;
				case "S":
					claimCur.PatRelat=PatPlans.GetRelat(patPlanList,PatPlans.GetOrdinal(PriSecMed.Secondary,patPlanList,planList,subList));
					claimCur.ClaimType="S";
					claimCur.InsSubNum2=PatPlans.GetInsSubNum(patPlanList,PatPlans.GetOrdinal(PriSecMed.Primary,patPlanList,planList,subList));
					sub=InsSubs.GetSub(claimCur.InsSubNum2,subList);
					claimCur.PlanNum2=sub.PlanNum;
					claimCur.PatRelat2=PatPlans.GetRelat(patPlanList,PatPlans.GetOrdinal(PriSecMed.Primary,patPlanList,planList,subList));
					break;
				case "Med":
					claimCur.PatRelat=PatPlans.GetFromList(patPlanList,subCur.InsSubNum).Relationship;
					claimCur.ClaimType="Other";
					if(PrefC.GetBool(PrefName.ClaimMedTypeIsInstWhenInsPlanIsMedical)) {
						claimCur.MedType=EnumClaimMedType.Institutional;
					}
					else {
						claimCur.MedType=EnumClaimMedType.Medical;
					}
					break;
				case "Other":
					claimCur.PatRelat=relatOther;
					claimCur.ClaimType="Other";
					//plannum2 is not automatically filled in.
					claimCur.ClaimForm=claimFormNum;
					if(planCur.IsMedical) {
						if(PrefC.GetBool(PrefName.ClaimMedTypeIsInstWhenInsPlanIsMedical)) {
							claimCur.MedType=EnumClaimMedType.Institutional;
						}
						else {
							claimCur.MedType=EnumClaimMedType.Medical;
						}
					}
					break;
			}
			if(planCur.PlanType=="c") {//if capitation
				claimCur.ClaimType="Cap";
			}
			claimCur.ProvTreat=proc.ProvNum;
			if(Providers.GetIsSec(proc.ProvNum)) {
				claimCur.ProvTreat=pat.PriProv;
				//OK if zero, because auto select first in list when open claim
			}
			claimCur.IsProsthesis="N";
			claimCur.ProvBill=Providers.GetBillingProvNum(claimCur.ProvTreat,claimCur.ClinicNum);//OK if zero, because it will get fixed in claim
			claimCur.EmployRelated=YN.No;
			claimCur.ClaimForm=planCur.ClaimFormNum;
			claimCur.AttachedFlags="Mail";
			Claims.Insert(claimCur);
			claimCur.ClaimIdentifier=ConvertClaimId(claimCur,pat);
			Update(claimCur);
			//attach procedure
			claimProcCur.ClaimNum=claimCur.ClaimNum;
			if(planCur.PlanType=="c") {//if capitation
				claimProcCur.Status=ClaimProcStatus.CapClaim;
			}
			else {
				claimProcCur.Status=ClaimProcStatus.NotReceived;
			}
			if(planCur.UseAltCode && (ProcedureCodes.GetProcCode(proc.CodeNum).AlternateCode1!="")) {
				claimProcCur.CodeSent=ProcedureCodes.GetProcCode(proc.CodeNum).AlternateCode1;
			}
			else if(planCur.IsMedical && proc.MedicalCode!="") {
				claimProcCur.CodeSent=proc.MedicalCode;
			}
			else {
				claimProcCur.CodeSent=ProcedureCodes.GetProcCode(proc.CodeNum).ProcCode;
				if(claimProcCur.CodeSent.Length>5 && claimProcCur.CodeSent.Substring(0,1)=="D") {
					claimProcCur.CodeSent=claimProcCur.CodeSent.Substring(0,5);
				}
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					if(claimProcCur.CodeSent.Length>5) {//In Canadian e-claims, codes can contain letters or numbers and cannot be longer than 5 characters.
						claimProcCur.CodeSent=claimProcCur.CodeSent.Substring(0,5);
					}
				}
			}
			claimProcCur.LineNumber=1;
			ClaimProcs.Update(claimProcCur);
			return claimCur;
		}
		
		///<summary>Create claim for the automatic ortho procedure.</summary>
		public static Claim CreateClaimForOrthoProc(string claimType,PatPlan patPlanCur,InsPlan insPlanCur,InsSub inssubCur,
			ClaimProc claimProc,Procedure proc, double feeBilled, DateTime dateBanding, int totalMonths, int monthsRem) {
			//No remoting role check; no call to db
			ClaimProc claimProcCur=Procedures.GetClaimProcEstimate(proc.ProcNum,new List<ClaimProc> { claimProc },insPlanCur,inssubCur.InsSubNum);
			List<PatPlan> listPatPlansForPat = PatPlans.GetPatPlansForPat(patPlanCur.PatNum);
			List<InsPlan> listInsPlansForPat = InsPlans.GetByInsSubs(listPatPlansForPat.Select(x => x.InsSubNum).ToList());
			List<InsSub> listInsSubsForPat = InsSubs.GetMany(listPatPlansForPat.Select(x => x.InsSubNum).ToList());
			if(claimProcCur==null) {
				claimProcCur=new ClaimProc();
				ClaimProcs.CreateEst(claimProcCur,proc,insPlanCur,inssubCur);
			}
			Claim claimCur=new Claim();
			claimCur.PatNum=proc.PatNum;
			claimCur.DateService=proc.ProcDate;
			claimCur.ClinicNum=proc.ClinicNum;
			claimCur.PlaceService=proc.PlaceService;
			claimCur.ClaimStatus="W";
			claimCur.DateSent=DateTime.Today;
			claimCur.DateSentOrig=DateTime.MinValue;
			claimCur.PlanNum=insPlanCur.PlanNum;
			claimCur.InsSubNum=inssubCur.InsSubNum;
			claimCur.ClaimFee=feeBilled;
			if(PrefC.GetBool(PrefName.OrthoClaimMarkAsOrtho)) {
				claimCur.IsOrtho=true;
			}
			if(PrefC.GetBool(PrefName.OrthoClaimUseDatePlacement)) {
				claimCur.OrthoDate=dateBanding;
				claimCur.OrthoTotalM=PIn.Byte(totalMonths.ToString(),false);
				claimCur.OrthoRemainM=PIn.Byte(monthsRem.ToString(),false);
			}
			InsSub sub;
			PatPlan patPlanOther;
			switch(claimType) {
				case "P":
					claimCur.PatRelat=patPlanCur.Relationship;
					claimCur.ClaimType="P";
					patPlanOther=PatPlans.GetPatPlan(patPlanCur.PatNum,PatPlans.GetOrdinal(PriSecMed.Secondary,listPatPlansForPat,listInsPlansForPat,listInsSubsForPat));
					if(patPlanOther==null) {
						claimCur.InsSubNum2=0;
						claimCur.PlanNum2=0;//no sec ins
						claimCur.PatRelat2=Relat.Self;
					}
					else {
						sub=InsSubs.GetOne(patPlanOther.InsSubNum);
						if(sub.PlanNum>0 && !InsPlans.RefreshOne(sub.PlanNum).IsMedical) {
							claimCur.PlanNum2=sub.PlanNum;//might be 0 if no sec ins
							claimCur.PatRelat2=patPlanOther.Relationship;
							claimCur.InsSubNum2=sub.InsSubNum;
						}
					}
					break;
				case "S":
					claimCur.PatRelat=patPlanCur.Relationship;
					claimCur.ClaimType="S";
					patPlanOther=PatPlans.GetPatPlan(patPlanCur.PatNum,PatPlans.GetOrdinal(PriSecMed.Primary,listPatPlansForPat,listInsPlansForPat,listInsSubsForPat));
					if(patPlanOther==null) { //should never happen
						claimCur.InsSubNum2=0;
						claimCur.PlanNum2=0;
						claimCur.PatRelat2=Relat.Self;
					}
					else {
						sub=InsSubs.GetOne(patPlanOther.InsSubNum);
						if(sub.PlanNum>0 && !InsPlans.RefreshOne(sub.PlanNum).IsMedical) {
							claimCur.PlanNum2=sub.PlanNum;
							claimCur.PatRelat2=patPlanOther.Relationship;
							claimCur.InsSubNum2=sub.InsSubNum;
						}
					}
					break;
			}
			if(insPlanCur.PlanType=="c") {//if capitation
				claimCur.ClaimType="Cap";
			}
			claimCur.ProvTreat=proc.ProvNum;
			if(Providers.GetIsSec(proc.ProvNum)) {
				claimCur.ProvTreat=Patients.GetPat(proc.PatNum).PriProv;
				//OK if zero, because auto select first in list when open claim
			}
			claimCur.IsProsthesis="N";
			claimCur.ProvBill=Providers.GetBillingProvNum(claimCur.ProvTreat,claimCur.ClinicNum);//OK if zero, because it will get fixed in claim
			claimCur.EmployRelated=YN.No;
			claimCur.ClaimForm=insPlanCur.ClaimFormNum;
			claimCur.AttachedFlags="Mail";
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				//Defaults to X in edit claim form
				claimCur.CanadianIsInitialUpper="X";
				claimCur.CanadianIsInitialLower="X";
			}
			Claims.Insert(claimCur);
			claimCur.ClaimIdentifier=Claims.ConvertClaimId(claimCur);
			Claims.Update(claimCur);
			//attach procedure
			claimProcCur.ClaimNum=claimCur.ClaimNum;
			if(insPlanCur.PlanType=="c") {//if capitation
				claimProcCur.Status=ClaimProcStatus.CapClaim;
			}
			else {
				claimProcCur.Status=ClaimProcStatus.NotReceived;
			}
			if(insPlanCur.UseAltCode && (ProcedureCodes.GetProcCode(proc.CodeNum).AlternateCode1!="")) {
				claimProcCur.CodeSent=ProcedureCodes.GetProcCode(proc.CodeNum).AlternateCode1;
			}
			else if(insPlanCur.IsMedical && proc.MedicalCode!="") {
				claimProcCur.CodeSent=proc.MedicalCode;
			}
			else {
				claimProcCur.CodeSent=ProcedureCodes.GetProcCode(proc.CodeNum).ProcCode;
				if(claimProcCur.CodeSent.Length>5 && claimProcCur.CodeSent.Substring(0,1)=="D") {
					claimProcCur.CodeSent=claimProcCur.CodeSent.Substring(0,5);
				}
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					if(claimProcCur.CodeSent.Length>5) {//In Canadian e-claims, codes can contain letters or numbers and cannot be longer than 5 characters.
						claimProcCur.CodeSent=claimProcCur.CodeSent.Substring(0,5);
					}
				}
			}
			claimProcCur.LineNumber=1;
			claimProcCur.FeeBilled=feeBilled;
			ClaimProcs.Update(claimProcCur);
			return claimCur;
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching claimNum as FKey and are related to Claim.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the Claim table type.</summary>
		public static void ClearFkey(long claimNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),claimNum);
				return;
			}
			Crud.ClaimCrud.ClearFkey(claimNum);
		}

		///<summary>Zeros securitylog FKey column for rows that are using the matching claimNums as FKey and are related to Claim.
		///Permtypes are generated from the AuditPerms property of the CrudTableAttribute within the Claim table type.</summary>
		public static void ClearFkey(List<long> listClaimNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listClaimNums);
				return;
			}
			Crud.ClaimCrud.ClearFkey(listClaimNums);
		}

		public static DateTime GetDateLastOrthoClaim(PatPlan patPlanCur,OrthoClaimType claimType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<DateTime>(MethodBase.GetCurrentMethod(),patPlanCur,claimType);
			}
			long orthoDefaultAutoCodeNum=PrefC.GetLong(PrefName.OrthoAutoProcCodeNum);
			string command="";
			if(claimType == OrthoClaimType.InitialPlusPeriodic) {
				command = @"	
				SELECT MAX(claim.DateSent) LastSent
				FROM claim
				INNER JOIN claimproc ON claimproc.ClaimNum = claim.ClaimNum
				INNER JOIN insplan ON claim.PlanNum = insplan.PlanNum
				INNER JOIN procedurelog ON procedurelog.ProcNum = claimproc.ProcNum
					AND procedurelog.CodeNum LIKE 
						IF(insplan.OrthoAutoProcCodeNumOverride = 0, 
						"+orthoDefaultAutoCodeNum+@",
						insplan.OrthoAutoProcCodeNumOverride)
				WHERE claim.ClaimStatus IN ('S','R')
				AND claim.PatNum = "+patPlanCur.PatNum+@"
				AND claim.InsSubNum = "+patPlanCur.InsSubNum;
			}
			else {
				command = @"	
				SELECT MAX(claim.DateSent) LastSent
				FROM claim
				INNER JOIN claimproc ON claimproc.ClaimNum = claim.ClaimNum
				INNER JOIN insplan ON claim.PlanNum = insplan.PlanNum
				INNER JOIN procedurelog ON procedurelog.ProcNum = claimproc.ProcNum
				INNER JOIN procedurecode ON procedurecode.CodeNum = procedurelog.CodeNum
				INNER JOIN covspan ON covspan.FromCode <= procedurecode.ProcCode AND covspan.ToCode >= procedurecode.ProcCode
				INNER JOIN covcat ON covcat.CovCatNum = covspan.CovCatNum
					AND covcat.EbenefitCat = "+POut.Int((int)EbenefitCategory.Orthodontics)+@"
				WHERE claim.ClaimStatus IN ('S','R')
				AND claim.PatNum = "+patPlanCur.PatNum+@"
				AND claim.InsSubNum = "+patPlanCur.InsSubNum;
			}
			return PIn.Date(Db.GetScalar(command));
		}

		public static List<Claim> GetForPat(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Claim>>(MethodBase.GetCurrentMethod(),patNum);
			}
			return Crud.ClaimCrud.SelectMany("SELECT * FROM claim WHERE PatNum = "+patNum);
		}

		///<summary>Gets the most recent ortho claim with a banding code attached.
		///Returns null if no ortho banding code nums found or no corresponding claim found.</summary>
		public static Claim GetOrthoBandingClaim(long patNum,long planNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Claim>(MethodBase.GetCurrentMethod(),patNum,planNum);
			}
			List<long> listProcCodeNums=ProcedureCodes.GetOrthoBandingCodeNums();
			if(listProcCodeNums==null || listProcCodeNums.Count < 1) {
				return null;
			}
			string command = @"
				SELECT claim.* 
				FROM claim
				WHERE claim.PatNum = "+POut.Long(patNum)+@"
				AND claim.PlanNum = "+POut.Long(planNum)+@"
				AND claim.IsOrtho = 1
				AND claim.ClaimStatus = 'R'
				AND EXISTS(
					SELECT * FROM claimproc
					INNER JOIN procedurelog ON claimproc.ProcNum = procedurelog.ProcNum
					INNER JOIN procedurecode ON procedurecode.CodeNum = procedurelog.CodeNum
						AND procedurecode.CodeNum IN ("+String.Join(",",listProcCodeNums)+@")
					WHERE claimproc.ClaimNum = claim.ClaimNum
				)
				ORDER BY claim.DateSent DESC";
			return Crud.ClaimCrud.SelectOne(command);
		}

		///<summary>Returns the defalt/calculated claim ID based on the ClaimIdPrefix preference.</summary>
		public static string ConvertClaimId(Claim claim,Patient pat=null) {
			if(pat==null) {
				pat=Patients.GetPat(claim.PatNum);
			}
			return Patients.ReplacePatient(PrefC.GetString(PrefName.ClaimIdPrefix),pat)+claim.ClaimNum;
		}

		///<summary>Caller should validate claim and listClaimProcsToSplit prior to calling.
		///Inserts and updates a new split claim. Also updates the given claimOriginal to reflect new values.</summary>
		public static Claim InsertSplitClaim(Claim claimOriginal,List<ClaimProc> listClaimProcsToSplit,Patient pat=null) {
			Claim newClaim=claimOriginal.Copy();
			newClaim.ClaimFee=0;
			newClaim.DedApplied=0;
			newClaim.InsPayEst=0;
			newClaim.InsPayAmt=0;
			newClaim.WriteOff=0;
			newClaim.CustomTracking=0;
			Claims.Insert(newClaim);//We must insert here so that we have the primary key in the loop below.
			//Split claims can occur for two reasons:
			//1) The insurance company rejects a claim because of one procedure.  The office staff then split off the "faulty" procedure and submit the
			//original claim.  Then the office corrects the information on the procedure for the split claim and sends the split claim separately.  In this
			//case, the Claim Identifier on the split claim should be different than the Claim Identifier on the original claim, because both claims
			//are independent of each other.
			//2) The insurance company decides to split off one procedure because it will take more time to process than the other procedures.  They do
			//this so that the provider can receive most of their payment as quickly as possible.  In this case, the provider will notice on the EOB or ERA
			//that the claim was split and they will manually split the appropriate procedures from the original claim in OD.  The procedure on the split
			//claim has already been submitted to the insurance company and does not need to be sent.  The Claim Identifier on the original claim and split
			//claim will be the same when received in an ERA and should also be the same in OD.  However, if the Claim Identifier is different on the split
			//claim than on the original claim, ERA matching should still work because of our secondary matching methods.
			newClaim.ClaimIdentifier=Claims.ConvertClaimId(newClaim,pat);
			//Now this claim has been duplicated, except it has a new ClaimNum and new totals.  There are no attached claimprocs yet.
			foreach(ClaimProc claimProc in listClaimProcsToSplit){
				ClaimProc claimProcOld=claimProc.Copy();
				claimProc.ClaimNum=newClaim.ClaimNum;
				claimProc.PayPlanNum=0;//detach from payplan if previously attached, claimprocs from two claims cannot be attached to the same payplan
				ClaimProcs.Update(claimProc,claimProcOld);//moves it to the new claim
				newClaim.ClaimFee+=claimProc.FeeBilled;
				newClaim.DedApplied+=claimProc.DedApplied;
				newClaim.InsPayEst+=claimProc.InsPayEst;
				newClaim.InsPayAmt+=claimProc.InsPayAmt;
				newClaim.WriteOff+=claimProc.WriteOff;
			}
			Claims.Update(newClaim);
			ClaimTrackings.CopyToClaim(claimOriginal.ClaimNum,newClaim.ClaimNum);
			claimOriginal.ClaimFee-=newClaim.ClaimFee;
			claimOriginal.DedApplied-=newClaim.DedApplied;
			claimOriginal.InsPayEst-=newClaim.InsPayEst;
			claimOriginal.InsPayAmt-=newClaim.InsPayAmt;
			claimOriginal.WriteOff-=newClaim.WriteOff;
			Claims.Update(claimOriginal);
			ClaimProcs.RemoveSupplementalTransfersForClaims(claimOriginal.ClaimNum);
			InsBlueBooks.SynchForClaimNums(claimOriginal.ClaimNum,newClaim.ClaimNum);
			return newClaim;
		}

		///<summary>Returns a dictionary such that they key is patnum and the value is the patients GetNameLF(...).</summary>
		public static SerializableDictionary<long,string> GetAllUniquePatNamesForClaims(List<Claim> listClaims) {
			SerializableDictionary<long,string> dictPatNames=new SerializableDictionary<long, string>();
			foreach(Claim claim in listClaims) {
				if(dictPatNames.ContainsKey(claim.PatNum)) {//Can be 0.  We want to add 0 to dictionary so blank name will show below.
					continue;
				}
				Patient patCur=Patients.GetPat(claim.PatNum);
				if(patCur==null) {
					dictPatNames.Add(0,"");
				}
				else {
					dictPatNames.Add(claim.PatNum,Patients.GetNameLF(patCur));
				}
			}
			return dictPatNames;
		}

		///<summary>There is a Clinic override that will cause the InsPlan-level setting to be completely ignored.  Otherwise, this just returns the insSub.AssignBen.  The override is based on the clinic of the subscriber.</summary>
		public static bool GetAssignmentOfBenefits(Claim claim,InsSub insSub) {
			//No need to check RemotingRole; no call to db.
			ClinicPref clinicPref=ClinicPrefs.GetPref(PrefName.InsDefaultAssignBen,claim.ClinicNum);
			if(clinicPref!=null) {//If the clinicpref exists, we know that we are always assigning to patient, which translates to false for the pref.
				//We don't actually check the value of the clinicpref.
				return false;
			}
			return insSub.AssignBen;
		}
	}//end class Claims

	///<summary>This is an odd class.  It holds data for the X12 (4010 only) generation process.  It replaces an older multi-dimensional array, so the names are funny, but helpful to prevent bugs.  Not an actual database table.</summary>
	[Serializable]
	public class X12TransactionItem {
		public string PayorId0;
		public long ProvBill1;
		public long Subscriber2;
		public long PatNum3;
		public long ClaimNum4;
	}

	///<summary>Holds a list of claims to show in the claims 'queue' waiting to be sent.  Not an actual database table.</summary>
	[Serializable()]
	public class ClaimSendQueueItem{
		///<summary></summary>
		public long ClaimNum;
		///<summary>Enum:NoSendElectType 0 - send electronically, 1 - don't send electronically, 2 - don't send non-primary (secondary,
		///tertiary, etc.) claims electronically.</summary>
		public NoSendElectType NoSendElect;
		///<summary></summary>
		public string PatName;
		///<summary>Single char: U,H,W,P,S,or R.</summary>
		///<remarks>U=Unsent, H=Hold until pri received, W=Waiting in queue, P=Probably sent, S=Sent, R=Received.  A(adj) is no longer used.</remarks>
		public string ClaimStatus;
		///<summary></summary>
		public string Carrier;
		///<summary></summary>
		public long PatNum;
		///<summary>ClearinghouseNum of HQ.</summary>
		public long ClearinghouseNum;
		///<summary></summary>
		public long ClinicNum;
		///<summary>Enum:EnumClaimMedType 0=Dental, 1=Medical, 2=Institutional</summary>
		public EnumClaimMedType MedType;
		///<summary></summary>
		public string MissingData;
		///<summary></summary>
		public string Warnings;
		///<summary></summary>
		public DateTime DateService;
		///<summary>False by default.  For speed purposes, claims should only be validated once, which is just before they are sent.</summary>
		public bool IsValid;
		/// <summary>Used to save what tracking is used for filtering.</summary>
		public long CustomTracking;
		///<summary>Claim has procedures with IcdVersion=9 and at least one Diagnostic.</summary>
		public bool HasIcd9;
		///<summary>The ordinal of the insurance plan for the subscriber associated with this claim.</summary>
		public int Ordinal;
		///<summary>Errors which will prevent FormClaimEdit.cs from saving the claim when the user clicks OK.</summary>
		public string ErrorsPreventingSave;
		///<summary>The Provider of a given clinic.</summary>
		public long ProvTreat;
		///<summary>Comma separated ProcedureCode string for this claim.</summary>
		public string ProcedureCodeString;
		///<summary>Date the claim was last edited.</summary>
		public DateTime SecDateTEdit;

		[XmlIgnore]
		public bool CanSendElect {
			get {
				return NoSendElect==NoSendElectType.SendElect || (NoSendElect==NoSendElectType.NoSendSecondaryElect && Ordinal==1);
			}
		}

		public ClaimSendQueueItem Copy(){
			return (ClaimSendQueueItem)MemberwiseClone();
		}
	}

	///<summary>Holds a list of claims to show in the Claim Pay Edit window.  Not an actual database table.</summary>
	[Serializable]
	public class ClaimPaySplit {
		///<summary></summary>
		public long ClaimNum;
		///<summary></summary>
		public string PatName;
		///<summary></summary>
		public long PatNum;
		///<summary></summary>
		public string Carrier;
		///<summary></summary>
		public DateTime DateClaim;
		///<summary></summary>
		public string ProvAbbr;
		///<summary></summary>
		public double FeeBilled;
		///<summary></summary>
		public double InsPayAmt;
		///<summary></summary>
		public long ClaimPaymentNum;
		///<summary>1-based</summary>
		public int PaymentRow;
		///<summary></summary>
		public string ClinicDesc;
		///<summary></summary>
		public string ClaimStatus;
		///<summary></summary>
		public string ClaimIdentifier;
	}

	///<summary>Different types of filters for the Claims Not Sent report.</summary>
	public enum ClaimNotSentStatuses {
		///<summary>0</summary>
		All,
		///<summary>1</summary>
		Primary,
		///<summary>2</summary>
		Secondary,
		///<summary>3</summary>
		Holding
	}

}