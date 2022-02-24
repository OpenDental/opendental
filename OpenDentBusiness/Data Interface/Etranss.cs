using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeBase;
using OpenDentBusiness;
using OpenDentBusiness.Eclaims;

namespace OpenDentBusiness{
	///<summary></summary>
	public class Etranss {
		///<summary>Gets data for the history grid in the SendClaims window.  The listEtransType must contain as least one item.</summary>
		public static DataTable RefreshHistory(DateTime dateFrom,DateTime dateTo,List<EtransType> listEtransType) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),dateFrom,dateTo,listEtransType);
			}
			string command="SELECT (CASE WHEN etrans.PatNum=0 THEN etrans.PatientNameRaw "
				+"ELSE CONCAT(CONCAT(patient.LName,', '),patient.FName) END) AS PatName,"
				+"(CASE WHEN etrans.carrierNum=0 THEN etrans.CarrierNameRaw ELSE carrier.CarrierName END) AS CarrierName,"
				+"clearinghouse.Description AS Clearinghouse,DateTimeTrans,etrans.OfficeSequenceNumber,"
				+"etrans.CarrierTransCounter,Etype,etrans.ClaimNum,etrans.EtransNum,etrans.AckCode,etrans.Note,etrans.EtransMessageTextNum,etrans.TranSetId835,"
				+"etrans.UserNum,etrans.PatNum "
				+"FROM etrans "
				+"LEFT JOIN carrier ON etrans.CarrierNum=carrier.CarrierNum "
				+"LEFT JOIN patient ON patient.PatNum=etrans.PatNum "
				+"LEFT JOIN clearinghouse ON clearinghouse.ClearinghouseNum=etrans.ClearinghouseNum WHERE "
				+DbHelper.DtimeToDate("DateTimeTrans")+" >= "+POut.Date(dateFrom)+" AND "
				+DbHelper.DtimeToDate("DateTimeTrans")+" <= "+POut.Date(dateTo)+" "
				+"AND Etype IN ("+POut.Long((int)listEtransType[0]);
				for(int i=1;i<listEtransType.Count;i++){//String.Join doesn't work because there's no way to cast the enums to ints in the function, db uses longs.
					command+=", "+POut.Long((int)listEtransType[i]);
				}				
				command+=") "
				//For Canada, when the undo button is used from Manage | Send Claims, the ClaimNum is set to 0 instead of deleting the etrans entry.
				//For transaction types related to claims where the claimnum=0, we do not want them to show in the history section of Manage | Send Claims because they have been undone.
				+"AND (ClaimNum<>0 OR Etype NOT IN ("+POut.Long((int)EtransType.Claim_CA)+","+POut.Long((int)EtransType.ClaimCOB_CA)+","+POut.Long((int)EtransType.Predeterm_CA)+","+POut.Long((int)EtransType.ClaimReversal_CA)+")) "
				+"ORDER BY DateTimeTrans";
			DataTable table=Db.GetTable(command);
			DataTable tHist=new DataTable("Table");
			tHist.Columns.Add("patName");
			tHist.Columns.Add("CarrierName");
			tHist.Columns.Add("Clearinghouse");
			tHist.Columns.Add("dateTimeTrans");
			tHist.Columns.Add("OfficeSequenceNumber");
			tHist.Columns.Add("CarrierTransCounter");
			tHist.Columns.Add("etype");
			tHist.Columns.Add("Etype");
			tHist.Columns.Add("ClaimNum");
			tHist.Columns.Add("EtransNum");
			tHist.Columns.Add("AckCode");
			tHist.Columns.Add("ack");
			tHist.Columns.Add("Note");
			tHist.Columns.Add("EtransMessageTextNum");
			tHist.Columns.Add("TranSetId835");
			tHist.Columns.Add("UserNum");
			tHist.Columns.Add("PatNum");
			DataRow row;
			string etype;
			for(int i=0;i<table.Rows.Count;i++) {
				row=tHist.NewRow();
				row["patName"]=table.Rows[i]["PatName"].ToString();
				row["CarrierName"]=table.Rows[i]["CarrierName"].ToString();
				row["Clearinghouse"]=table.Rows[i]["Clearinghouse"].ToString();
				row["dateTimeTrans"]=PIn.DateT(table.Rows[i]["DateTimeTrans"].ToString()).ToShortDateString();
				row["OfficeSequenceNumber"]=table.Rows[i]["OfficeSequenceNumber"].ToString();
				row["CarrierTransCounter"]=table.Rows[i]["CarrierTransCounter"].ToString();
				row["Etype"]=table.Rows[i]["Etype"].ToString();
				etype=Lans.g("enumEtransType",((EtransType)PIn.Long(table.Rows[i]["Etype"].ToString())).ToString());
				if(etype.EndsWith("_CA")){
					etype=etype.Substring(0,etype.Length-3);
				}
				row["etype"]=etype;
				row["ClaimNum"]=table.Rows[i]["ClaimNum"].ToString();
				row["EtransNum"]=table.Rows[i]["EtransNum"].ToString();
				row["AckCode"]=table.Rows[i]["AckCode"].ToString();
				if(table.Rows[i]["AckCode"].ToString()=="A"){
					row["ack"]=Lans.g("Etrans","Accepted");
				}
				else if(table.Rows[i]["AckCode"].ToString()=="R") {
					row["ack"]=Lans.g("Etrans","Rejected");
				}
				else if(table.Rows[i]["AckCode"].ToString()=="Recd") {
					row["ack"]=Lans.g("Etrans","Received");
				}
				else {
					row["ack"]="";
				}
				row["Note"]=table.Rows[i]["Note"].ToString();
				row["EtransMessageTextNum"]=table.Rows[i]["EtransMessageTextNum"].ToString();
				row["TranSetId835"]=table.Rows[i]["TranSetId835"].ToString();
				row["UserNum"]=table.Rows[i]["UserNum"].ToString();
				row["PatNum"]=table.Rows[i]["PatNum"].ToString();
				tHist.Rows.Add(row);
			}
			return tHist;
		}

		///<summary></summary>
		public static List<Etrans> GetHistoryOneClaim(long claimNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Etrans>>(MethodBase.GetCurrentMethod(),claimNum);
			}
			string command="SELECT * FROM etrans WHERE ClaimNum="+POut.Long(claimNum)+" "
				+"AND (Etype="+POut.Int((int)EtransType.Claim_CA)+" "
				+"OR Etype="+POut.Int((int)EtransType.ClaimCOB_CA)+" "
				+"OR Etype="+POut.Int((int)EtransType.Predeterm_CA)+" "
				+"OR Etype="+POut.Int((int)EtransType.ClaimReversal_CA)+" "
				+"OR Etype="+POut.Int((int)EtransType.ClaimSent)+" "
				+"OR Etype="+POut.Int((int)EtransType.Claim_Ramq)+" "
				+"OR Etype="+POut.Int((int)EtransType.ClaimPrinted)+") "
				+"ORDER BY DateTimeTrans DESC"; //Because when we want the most recent in the list, we use List[0].
			return Crud.EtransCrud.SelectMany(command);
		}

		///<summary>Gets all types of transactions for the given claim number.</summary>
		public static List<Etrans> GetAllForOneClaim(long claimNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Etrans>>(MethodBase.GetCurrentMethod(),claimNum);
			}
			string command="SELECT * FROM etrans WHERE ClaimNum="+POut.Long(claimNum);
			return Crud.EtransCrud.SelectMany(command);
		}

		///<summary></summary>
		public static Etrans GetEtrans(long etransNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Etrans>(MethodBase.GetCurrentMethod(),etransNum);
			}
			string command="SELECT * FROM etrans WHERE EtransNum="+POut.Long(etransNum);
			return Crud.EtransCrud.SelectOne(command);
		}

		///<summary></summary>
		public static List<Etrans> GetMany(params long[] listEtransNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Etrans>>(MethodBase.GetCurrentMethod(),listEtransNums);
			}
			if(listEtransNums.Length==0) {
				return new List<Etrans>();
			}
			string command="SELECT * FROM etrans WHERE EtransNum IN ("+String.Join(",",listEtransNums.Select(x => POut.Long(x)))+")";
			return Crud.EtransCrud.SelectMany(command);
		}

		///<summary>Gets all X12 835 etrans entries relating to a specific claim.</summary>
		public static List<Etrans> GetErasOneClaim(string claimIdentifier,DateTime dateClaimService) {
			//The main goal of this check is to prevent null claimIdentifiers from causing an exception.
			//However, an empty claim identifier should also return an empty list because that is a terrible identifier IMO.
			if(string.IsNullOrEmpty(claimIdentifier)) {
				return new List<Etrans>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Etrans>>(MethodBase.GetCurrentMethod(),claimIdentifier,dateClaimService);
			}
			string claimId=claimIdentifier;
			if(claimId.Length>16) {
				//Our claim identifiers in the database can be longer than 20 characters (mostly when using replication).
				//When the claim identifier is sent out on the claim, it is truncated to 20 characters.
				//Therefore, if the claim identifier is longer than 20 characters,
				//then it was truncated when sent out, so we have to look for claims beginning with the claim identifier given if there is not an exact match.
				//We also send shorter identifiers for some clearinghouses.  For example, the maximum claim identifier length for Denti-Cal is 17 characters.
				claimId=claimId.Substring(0,16);
			}
			string command="SELECT * FROM etrans"
				+" INNER JOIN etransmessagetext ON etransmessagetext.EtransMessageTextNum=etrans.EtransMessageTextNum"
					+" AND etransmessagetext.MessageText REGEXP 'CLP."+POut.String(claimId)+"'"
					//CLP = match CLP, . = match any character, then up to 16 other chars after.
				+" WHERE Etype="+POut.Int((int)EtransType.ERA_835)+" AND etrans.DateTimeTrans >= "+POut.Date(dateClaimService);
			if(claimIdentifier.Length<16) {
				DataTable tableEtrans=Db.GetTable(command);
				List<Etrans> listEtrans=Crud.EtransCrud.TableToList(tableEtrans);
				List<Etrans> retVal=new List<Etrans>();
				for(int i=0;i<tableEtrans.Rows.Count;i++) {
					string messageText=PIn.String(tableEtrans.Rows[i]["MessageText"].ToString());
					string separator=messageText.Substring(3,1);//The character that is used as a separator is always at the third index of the message text.
					if(messageText.Contains("CLP"+separator+claimId+separator)) {						
						retVal.Add(listEtrans[i]);//This Etrans has an exact match for the claimIdentifier, it's an accurate search result.
					}
				}
				return retVal;
			}
			else {
				//If the claimIdentifier is > 16 we trust it's unique enough we don't need to do more searching.
				//Plus, we cannot trust any characters after the 16th character, since the identifier might have been truncated at some point.
				return Crud.EtransCrud.SelectMany(command);
			}
		}

		///<summary>Gets a list of all 270's and Canadian eligibilities for this plan.</summary>
		public static List<Etrans> GetList270ForPlan(long planNum,long insSubNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Etrans>>(MethodBase.GetCurrentMethod(),planNum,insSubNum);
			}
			string command="SELECT * FROM etrans WHERE PlanNum="+POut.Long(planNum)
				+" AND InsSubNum="+POut.Long(insSubNum)
				+" AND (Etype="+POut.Long((int)EtransType.BenefitInquiry270)
				+" OR Etype="+POut.Long((int)EtransType.Eligibility_CA)+")";
			return Crud.EtransCrud.SelectMany(command);
		}

		///<summary>Use for Canadian claims only. Finds the most recent etrans record which matches the unique officeSequenceNumber specified. The officeSequenceNumber corresponds to field A02.</summary>
		public static Etrans GetForSequenceNumberCanada(string officeSequenceNumber) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Etrans>(MethodBase.GetCurrentMethod(),officeSequenceNumber);
			}
			string command="SELECT * FROM etrans WHERE OfficeSequenceNumber="+POut.String(officeSequenceNumber)+" ORDER BY EtransNum DESC LIMIT 1";
			return Crud.EtransCrud.SelectOne(command);

		}

		/*
		///<summary></summary>
		public static Etrans GetAckForTrans(int etransNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Etrans>(MethodBase.GetCurrentMethod(),etransNum);
			}
			//first, get the actual trans.
			string command="SELECT * FROM etrans WHERE EtransNum="+POut.PInt(etransNum);
			DataTable table=Db.GetTable(command);
			Etrans etrans=SubmitAndFill(table);
			command="SELECT * FROM etrans WHERE "
				+"Etype=21 "//ack997
				+"AND ClearingHouseNum="+POut.PInt(etrans.ClearingHouseNum)
				+" AND BatchNumber= "+POut.PInt(etrans.BatchNumber)
				+" AND DateTimeTrans < "+POut.PDateT(etrans.DateTimeTrans.AddDays(14))//less than 2wks in the future
				+" AND DateTimeTrans > "+POut.PDateT(etrans.DateTimeTrans.AddDays(-1));//and no more than one day before claim
			table=Db.GetTable(command);
			return SubmitAndFill(table);
		}*/

		/*
		private static List<Etrans> SubmitAndFill(DataTable table){
			//No need to check RemotingRole; no call to db.
			//if(table.Rows.Count==0){
			//	return null;
			//}
			List<Etrans> retVal=new List<Etrans>();
			Etrans etrans;
			for(int i=0;i<table.Rows.Count;i++) {
				etrans=new Etrans();
				etrans.EtransNum           =PIn.Long(table.Rows[i][0].ToString());
				etrans.DateTimeTrans       =PIn.DateT(table.Rows[i][1].ToString());
				etrans.ClearingHouseNum    =PIn.Long(table.Rows[i][2].ToString());
				etrans.Etype               =(EtransType)PIn.Long(table.Rows[i][3].ToString());
				etrans.ClaimNum            =PIn.Long(table.Rows[i][4].ToString());
				etrans.OfficeSequenceNumber=PIn.Int(table.Rows[i][5].ToString());
				etrans.CarrierTransCounter =PIn.Int(table.Rows[i][6].ToString());
				etrans.CarrierTransCounter2=PIn.Int(table.Rows[i][7].ToString());
				etrans.CarrierNum          =PIn.Long(table.Rows[i][8].ToString());
				etrans.CarrierNum2         =PIn.Long(table.Rows[i][9].ToString());
				etrans.PatNum              =PIn.Long(table.Rows[i][10].ToString());
				etrans.BatchNumber         =PIn.Int(table.Rows[i][11].ToString());
				etrans.AckCode             =PIn.String(table.Rows[i][12].ToString());
				etrans.TransSetNum         =PIn.Int(table.Rows[i][13].ToString());
				etrans.Note                =PIn.String(table.Rows[i][14].ToString());
				etrans.EtransMessageTextNum=PIn.Long(table.Rows[i][15].ToString());
				etrans.AckEtransNum        =PIn.Long(table.Rows[i][16].ToString());
				etrans.PlanNum             =PIn.Long(table.Rows[i][17].ToString());
				retVal.Add(etrans);
			}
			return retVal;
		}*/

		///<summary>DateTimeTrans handled automatically here.</summary>
		public static long Insert(Etrans etrans) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				etrans.EtransNum=Meth.GetLong(MethodBase.GetCurrentMethod(),etrans);
				return etrans.EtransNum;
			}
			return Crud.EtransCrud.Insert(etrans);
		}

		///<summary></summary>
		public static void Update(Etrans etrans) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),etrans);
				return;
			}
			Crud.EtransCrud.Update(etrans);
		}

		///<summary>Only updates fields in etrans that are different from etransOld.</summary>
		public static void Update(Etrans etrans,Etrans etransOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),etrans,etransOld);
				return;
			}
			Crud.EtransCrud.Update(etrans,etransOld);
		}

		///<summary>Not for claim types, just other types, including Eligibility. This function gets run first.  Then, the messagetext is created and an attempt is made to send the message.  Finally, the messagetext is added to the etrans.  This is necessary because the transaction numbers must be incremented and assigned to each message before creating the message and attempting to send.  If it fails, we will need to roll back.  Provide EITHER a carrierNum OR a canadianNetworkNum.  Many transactions can be sent to a carrier or to a network.</summary>
		public static Etrans CreateCanadianOutput(long patNum,long carrierNum,long canadianNetworkNum
			,long clearinghouseNum,EtransType etype,long planNum,long insSubNum,long userNum,bool hasSecondary=false)
		{
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Etrans>(MethodBase.GetCurrentMethod(),patNum,carrierNum,canadianNetworkNum,clearinghouseNum,etype,planNum,insSubNum,userNum,hasSecondary);
			}
			//validation of carrier vs network
			if(etype==EtransType.Eligibility_CA){
				//only carrierNum is allowed (and required)
				if(carrierNum==0){
					throw new ApplicationException("Carrier not supplied for Etranss.CreateCanadianOutput.");
				}
				if(canadianNetworkNum!=0){
					throw new ApplicationException("NetworkNum not allowed for Etranss.CreateCanadianOutput.");
				}
			}
			Etrans etrans=new Etrans();
			//etrans.DateTimeTrans handled automatically
			etrans.ClearingHouseNum=clearinghouseNum;
			etrans.Etype=etype;
			etrans.ClaimNum=0;//no claim involved
			etrans.PatNum=patNum;
			//CanadianNetworkNum?
			etrans.CarrierNum=carrierNum;
			etrans.PlanNum=planNum;
			etrans.InsSubNum=insSubNum;
			etrans.UserNum=userNum;
			etrans.BatchNumber=0;
			//Get next OfficeSequenceNumber-----------------------------------------------------------------------------------------
			etrans=SetCanadianEtransFields(etrans,hasSecondary);
			Insert(etrans);
			return GetEtrans(etrans.EtransNum);//Since the DateTimeTrans is set upon insert, we need to read the record again in order to get the date.
		}

		///<summary>Throws exceptions.
		///When etrans.Etype is associated to a Canadian request EType, this runs multiple queries to set etrans.CarrierTransCounter and
		///etrans.CarrierTransCounter2.  Otherwise returns without making any changes.
		///The etrans.CarrierNum, etrans.CarrierNum2 and etrans.Etype columns must be set prior to running this.</summary>
		public static Etrans SetCanadianEtransFields(Etrans etrans,bool hasSecondary=true) {
			if(!EnumTools.GetAttributeOrDefault<EtransTypeAttr>(etrans.Etype).IsCanadaType || !EnumTools.GetAttributeOrDefault<EtransTypeAttr>(etrans.Etype).IsRequestType) {
				return etrans;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Etrans>(MethodBase.GetCurrentMethod(),etrans,hasSecondary);
			}
			etrans.OfficeSequenceNumber=0;
			//find the next officeSequenceNumber
			string command="SELECT MAX(OfficeSequenceNumber) FROM etrans";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count>0) {
				etrans.OfficeSequenceNumber=PIn.Int(table.Rows[0][0].ToString());
				if(etrans.OfficeSequenceNumber==999999){//if the office has sent > 1 million messages, and has looped back around to 1.
					throw new ApplicationException("OfficeSequenceNumber has maxed out at 999999.  This program will need to be enhanced.");
				}
			}
			etrans.OfficeSequenceNumber++;
			//find the next CarrierTransCounter for the primary carrier
			#region CarrierTransCounter
			etrans.CarrierTransCounter=0;
			command="SELECT MAX(CarrierTransCounter) FROM etrans "
				+"WHERE CarrierNum="+POut.Long(etrans.CarrierNum);
			table=Db.GetTable(command);
			int tempcounter=0;
			if(table.Rows.Count>0) {
				tempcounter=PIn.Int(table.Rows[0][0].ToString());
			}
			if(tempcounter>etrans.CarrierTransCounter) {
				etrans.CarrierTransCounter=tempcounter;
			}
			command="SELECT MAX(CarrierTransCounter2) FROM etrans "
				+"WHERE CarrierNum2="+POut.Long(etrans.CarrierNum);
			table=Db.GetTable(command);
			if(table.Rows.Count>0) {
				tempcounter=PIn.Int(table.Rows[0][0].ToString());
			}
			if(tempcounter>etrans.CarrierTransCounter) {
				etrans.CarrierTransCounter=tempcounter;
			}
			if(etrans.CarrierTransCounter==99999){
				throw new ApplicationException("CarrierTransCounter has maxed out at 99999.  This program will need to be enhanced.");
				//maybe by adding a reset date to the preference table which will apply to all counters as a whole.
			}
			etrans.CarrierTransCounter++;
			#endregion CarrierTransCounter
			if(!hasSecondary || etrans.CarrierNum2==0) {
				return etrans;
			}
			#region CarrierTransCounter2
			etrans.CarrierTransCounter2=1;
			command="SELECT MAX(CarrierTransCounter) FROM etrans "
				+"WHERE CarrierNum="+POut.Long(etrans.CarrierNum2);
			table=Db.GetTable(command);
			if(table.Rows.Count>0) {
				tempcounter=PIn.Int(table.Rows[0][0].ToString());
			}
			if(tempcounter>etrans.CarrierTransCounter2) {
				etrans.CarrierTransCounter2=tempcounter;
			}
			command="SELECT MAX(CarrierTransCounter2) FROM etrans "
				+"WHERE CarrierNum2="+POut.Long(etrans.CarrierNum2);
			table=Db.GetTable(command);
			if(table.Rows.Count>0) {
				tempcounter=PIn.Int(table.Rows[0][0].ToString());
			}
			if(tempcounter>etrans.CarrierTransCounter2) {
				etrans.CarrierTransCounter2=tempcounter;
			}
			if(etrans.CarrierTransCounter2==99999) {
				throw new ApplicationException("CarrierTransCounter has maxed out at 99999.  This program will need to be enhanced.");
			}
			etrans.CarrierTransCounter2++;
			#endregion
			return etrans;
		}

		///<summary>Inserts EtransMessageText row with given messageText then updates the etrans.EtransMessageTextNum in the DB based on given etransNum.
		///CAUTION: This does not update the EtransMessageTextNum field of an object in memory.
		///Instead it returns the inserted EtransMessageTextNum, this should be used to update the in memory object if needed.</summary>
		public static long SetMessage(long etransNum,string messageText) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetLong(MethodBase.GetCurrentMethod(),etransNum,messageText);
			}
			EtransMessageText msg=new EtransMessageText();
			msg.MessageText=messageText;
			EtransMessageTexts.Insert(msg);
			//string command=
			string command= "UPDATE etrans SET EtransMessageTextNum="+POut.Long(msg.EtransMessageTextNum)+" "
				+"WHERE EtransNum = '"+POut.Long(etransNum)+"'";
			Db.NonQ(command);
			return msg.EtransMessageTextNum;
		}

		///<summary>Deletes the etrans entry and changes the status of the claim back to W.  If it encounters an entry that's not a claim, it skips it for now.  Later, it will handle all types of undo.  It will also check Canadian claims to prevent alteration if an ack or EOB has been received.</summary>
		public static void Undo(long etransNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),etransNum);
				return;
			}
			//see if it's a claim.
			string command="SELECT ClaimNum FROM etrans WHERE EtransNum="+POut.Long(etransNum);
			DataTable table=Db.GetTable(command);
			long claimNum=PIn.Long(table.Rows[0][0].ToString());
			if(claimNum==0){//if no claim
				return;//for now
			}
			//future Canadian check will go here

			//Change the claim back to W.
			command="UPDATE claim SET ClaimStatus='W' WHERE ClaimNum="+POut.Long(claimNum);
			Db.NonQ(command);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				//We cannot delete etrans entries, because we need to retain the OfficeSequenceNumber in order to prevent reuse.
				//We used to allow deleting here, but some customers were getting the "invalid dental claim number or office sequence number" error message when sending claims thereafter.
				//Office sequence numbers must be unique for every request and response, whether related to a claim or not.
				//Instead of deleting, we simply detach from the claim, so that this historic entry will no longer display within the Manage | Send Claims window.
				command="UPDATE etrans SET ClaimNum=0 WHERE EtransNum="+POut.Long(etransNum);
				Db.NonQ(command);
			}
			else {
				//Delete this etrans
				command="DELETE FROM etrans WHERE EtransNum="+POut.Long(etransNum);
				Db.NonQ(command);
			}
		}

		///<summary>Deletes the etrans entry.  Mostly used when the etrans entry was created, but then the communication with the clearinghouse failed.
		///So this is just a rollback function.  Will not delete the message associated with the etrans.  That must be done separately.</summary>		
		public static void Delete(long etransNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),etransNum);
				return;
			}
			string command="DELETE FROM etrans WHERE EtransNum="+POut.Long(etransNum);
			Db.NonQ(command);
		}

		public static void Delete835(Etrans etrans) {
			EtransMessageTexts.Delete(etrans.EtransMessageTextNum,etrans.EtransNum);
			Etranss.Delete(etrans.EtransNum);
			Etrans835Attaches.DeleteMany(-1,etrans.EtransNum);
		}

		///<summary>Sets the status of the claim to sent, usually as part of printing.  Also makes an entry in etrans.  If this is Canadian eclaims, then this function gets run first.  If the claim is to be sent elecronically, then the messagetext is created after this method and an attempt is made to send the claim.  Finally, the messagetext is added to the etrans.  This is necessary because the transaction numbers must be incremented and assigned to each claim before creating the message and attempting to send.  For Canadians, it will always record the attempt as an etrans even if claim is not set to status of sent.</summary>
		public static Etrans SetClaimSentOrPrinted(long claimNum,long patNum,long clearinghouseNum,EtransType etype,int batchNumber,long userNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Etrans>(MethodBase.GetCurrentMethod(),claimNum,patNum,clearinghouseNum,etype,batchNumber,userNum);
			}
			Etrans etrans=CreateEtransForClaim(claimNum,patNum,clearinghouseNum,etype,batchNumber,userNum);
			etrans=SetCanadianEtransFields(etrans);//etrans.CarrierNum, etrans.CarrierNum2 and etrans.EType all set prior to calling this.
			Claims.SetClaimSent(claimNum);
			Insert(etrans);
			return GetEtrans(etrans.EtransNum);//Since the DateTimeTrans is set upon insert, we need to read the record again in order to get the date.
		}

		///<summary>Returns an etrans that has not been inserted into the DB.
		///Should only be called with etrans is related an EtransType that is of claim type, currently no validation is done in this function to ensure this.</summary>
		public static Etrans CreateEtransForClaim(long claimNum,long patNum,long clearinghouseNum,EtransType etype,int batchNumber,long userNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Etrans>(MethodBase.GetCurrentMethod(),claimNum,patNum,clearinghouseNum,etype,batchNumber,userNum);
			}
			Etrans etrans=new Etrans();
			//etrans.DateTimeTrans handled automatically
			etrans.ClearingHouseNum=clearinghouseNum;
			etrans.Etype=etype;
			etrans.ClaimNum=claimNum;
			etrans.PatNum=patNum;
			etrans.UserNum=userNum;
			//Get the primary and secondary carrierNums for this claim.
			string command="SELECT carrier1.CarrierNum,carrier2.CarrierNum AS CarrierNum2 FROM claim "
				+"LEFT JOIN insplan insplan1 ON insplan1.PlanNum=claim.PlanNum "
				+"LEFT JOIN carrier carrier1 ON carrier1.CarrierNum=insplan1.CarrierNum "
				+"LEFT JOIN insplan insplan2 ON insplan2.PlanNum=claim.PlanNum2 "
				+"LEFT JOIN carrier carrier2 ON carrier2.CarrierNum=insplan2.CarrierNum "
				+"WHERE claim.ClaimNum="+POut.Long(claimNum);
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count > 0) {//The claim could have been deleted by someone else.  Don't worry about preserving the carrier information.  Set to 0.
				etrans.CarrierNum=PIn.Long(table.Rows[0][0].ToString());
				etrans.CarrierNum2=PIn.Long(table.Rows[0][1].ToString());//might be 0 if no secondary on this claim
			}
			else {
				etrans.Note=Lans.g(nameof(Etrans),"Primry carrier and secondary carrier are unknown due to missing claim.  Invalid ClaimNum.  "
					+"Claim may have been deleted during sending.");
			}
			etrans.BatchNumber=batchNumber;
			return etrans;
		}

		///<summary>Etrans type will be figured out by this class.  Either TextReport, Acknowledge_997, Acknowledge_999, or StatusNotify_277.</summary>
		public static void ProcessIncomingReport(DateTime dateTimeTrans,long hqClearinghouseNum,string messageText,long userNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),dateTimeTrans,hqClearinghouseNum,messageText,userNum);
				return;
			}
			Etrans etrans=CreateEtrans(dateTimeTrans,hqClearinghouseNum,messageText,userNum);
			string command;
			X12object Xobj=X12object.ToX12object(messageText);
			if(Xobj!=null) {//Is a correctly formatted X12 message.
				if(Xobj.IsAckInterchange()) {
					etrans.Etype=EtransType.Ack_Interchange;
					Etranss.Insert(etrans);
					//At some point in the future, we should use TA101 to match to batch number and TA104 to get the ack code, 
					//then update historic etrans entries like we do for 997s, 999s and 277s.
				}
				else if(Xobj.Is997()) {
					X997 x997=new X997(messageText);
					etrans.Etype=EtransType.Acknowledge_997;
					etrans.BatchNumber=x997.GetBatchNumber();
					Etranss.Insert(etrans);
					string batchack=x997.GetBatchAckCode();
					if(batchack=="A"||batchack=="R") {//accepted or rejected
						command="UPDATE etrans SET AckCode='"+batchack+"', "
							+"AckEtransNum="+POut.Long(etrans.EtransNum)
							+" WHERE BatchNumber="+POut.Long(etrans.BatchNumber)
							+" AND ClearinghouseNum="+POut.Long(hqClearinghouseNum)
							+" AND DateTimeTrans > "+POut.DateT(dateTimeTrans.AddDays(-14))
							+" AND DateTimeTrans < "+POut.DateT(dateTimeTrans.AddDays(1))
							+" AND AckEtransNum=0";
						Db.NonQ(command);
					}
					else {//partially accepted
						List<int> transNums=x997.GetTransNums();
						string ack;
						for(int i=0;i<transNums.Count;i++) {
							ack=x997.GetAckForTrans(transNums[i]);
							if(ack=="A"||ack=="R") {//accepted or rejected
								command="UPDATE etrans SET AckCode='"+ack+"', "
									+"AckEtransNum="+POut.Long(etrans.EtransNum)
									+" WHERE BatchNumber="+POut.Long(etrans.BatchNumber)
									+" AND TransSetNum="+POut.Long(transNums[i])
									+" AND ClearinghouseNum="+POut.Long(hqClearinghouseNum)
									+" AND DateTimeTrans > "+POut.DateT(dateTimeTrans.AddDays(-14))
									+" AND DateTimeTrans < "+POut.DateT(dateTimeTrans.AddDays(1))
									+" AND AckEtransNum=0";
								Db.NonQ(command);
							}
						}
					}
					//none of the other fields make sense, because this ack could refer to many claims.
				}
				else if(Xobj.Is999()) {
					X999 x999=new X999(messageText);
					etrans.Etype=EtransType.Acknowledge_999;
					etrans.BatchNumber=x999.GetBatchNumber();
					Etranss.Insert(etrans);
					string batchack=x999.GetBatchAckCode();
					if(batchack=="A"||batchack=="R") {//accepted or rejected
					  command="UPDATE etrans SET AckCode='"+batchack+"', "
					    +"AckEtransNum="+POut.Long(etrans.EtransNum)
					    +" WHERE BatchNumber="+POut.Long(etrans.BatchNumber)
					    +" AND ClearinghouseNum="+POut.Long(hqClearinghouseNum)
					    +" AND DateTimeTrans > "+POut.DateT(dateTimeTrans.AddDays(-14))
					    +" AND DateTimeTrans < "+POut.DateT(dateTimeTrans.AddDays(1))
					    +" AND AckEtransNum=0";
					  Db.NonQ(command);
					}
					else {//partially accepted
					  List<int> transNums=x999.GetTransNums();
					  string ack;
					  for(int i=0;i<transNums.Count;i++) {
					    ack=x999.GetAckForTrans(transNums[i]);
					    if(ack=="A"||ack=="R") {//accepted or rejected
					      command="UPDATE etrans SET AckCode='"+ack+"', "
					        +"AckEtransNum="+POut.Long(etrans.EtransNum)
					        +" WHERE BatchNumber="+POut.Long(etrans.BatchNumber)
					        +" AND TransSetNum="+POut.Long(transNums[i])
					        +" AND ClearinghouseNum="+POut.Long(hqClearinghouseNum)
					        +" AND DateTimeTrans > "+POut.DateT(dateTimeTrans.AddDays(-14))
					        +" AND DateTimeTrans < "+POut.DateT(dateTimeTrans.AddDays(1))
					        +" AND AckEtransNum=0";
					      Db.NonQ(command);
					    }
					  }
					}
					//none of the other fields make sense, because this ack could refer to many claims.
				}
				else if(X277.Is277(Xobj)) {
					X277 x277=new X277(messageText);
					etrans.Etype=EtransType.StatusNotify_277;
					Etranss.Insert(etrans);
					List<string> listClaimIdentifiers=x277.GetClaimTrackingNumbers();
					//Dictionary to run one update command per ack code for many claims.
					Dictionary <string,List<X12ClaimMatch>> dictClaimMatchesByAck=new Dictionary<string,List<X12ClaimMatch>>();
					for(int i=0;i<listClaimIdentifiers.Count;i++) {
						X12ClaimMatch claimMatch=new X12ClaimMatch();
						claimMatch.ClaimIdentifier=listClaimIdentifiers[i];
						string[] arrayClaimInfo=x277.GetClaimInfo(claimMatch.ClaimIdentifier);
						claimMatch.PatFname=PIn.String(arrayClaimInfo[0]);
						claimMatch.PatLname=PIn.String(arrayClaimInfo[1]);
						claimMatch.DateServiceStart=PIn.DateT(arrayClaimInfo[6]);
						claimMatch.DateServiceEnd=PIn.DateT(arrayClaimInfo[7]);
						claimMatch.ClaimFee=PIn.Double(arrayClaimInfo[9]);
						claimMatch.SubscriberId=PIn.String(arrayClaimInfo[10]);
						claimMatch.EtransNum=etrans.EtransNum;
						string ack=arrayClaimInfo[3];
						if(!dictClaimMatchesByAck.ContainsKey(ack)) {
							dictClaimMatchesByAck.Add(ack,new List<X12ClaimMatch>());
						}
						dictClaimMatchesByAck[ack].Add(claimMatch);
					}
					foreach(string ack in dictClaimMatchesByAck.Keys) {
						List <long> listClaimNums=Claims.GetClaimFromX12(dictClaimMatchesByAck[ack]);
						if(listClaimNums!=null) {
							listClaimNums=listClaimNums.Where(x => x!=0).ToList();
							if(listClaimNums.Count > 0) {
								//Locate the latest etrans entries for the claims based on DateTimeTrans with EType of ClaimSent or Claim_Ren and update the AckCode and AckEtransNum.
								//We overwrite existing acks from 997s, 999s and older 277s.
								command="UPDATE etrans SET AckCode='"+ack+"', "
									+"AckEtransNum="+POut.Long(etrans.EtransNum)
									+" WHERE EType IN ("+POut.Int((int)EtransType.ClaimSent)+","+POut.Int((int)EtransType.Claim_Ren)+") "
									+" AND ClaimNum IN("+String.Join(",",listClaimNums.Select(x => POut.Long(x)))+")"
									+" AND ClearinghouseNum="+POut.Long(hqClearinghouseNum)
									+" AND DateTimeTrans > "+POut.DateT(dateTimeTrans.AddDays(-14))
									+" AND DateTimeTrans < "+POut.DateT(dateTimeTrans.AddDays(1));
								Db.NonQ(command);
							}
						}
						//none of the other fields make sense, because this ack could refer to many claims.
					}
				}
				else if(X835.Is835(Xobj)) {
					etrans.Etype=EtransType.ERA_835;
					List <string> listTranSetIds=Xobj.GetTranSetIds();
					List <Etrans> listEtrans=new List<Etrans>();
					List <X835> list835s=new List<X835>();
					//We pull in the 835 data in two loops so that we can ensure the 835 is fully parsed before we create any etrans entries.
					for(int i=0;i<listTranSetIds.Count;i++) {
						etrans.TranSetId835=listTranSetIds[i];
						if(i>0) {
							etrans.EtransNum=0;//To get a new record to insert.
						}
						X835 x835=new X835(etrans,messageText,etrans.TranSetId835);//parse. If parsing fails, then no etrans entries will be inserted.
						etrans.CarrierNameRaw=x835.PayerName;
						List<string> listUniquePatientNames=new List<string>();
						for(int j=0;j<x835.ListClaimsPaid.Count;j++) {
							string patName=x835.ListClaimsPaid[j].PatientName.ToString(false);
							if(!listUniquePatientNames.Contains(patName)) {
								listUniquePatientNames.Add(patName);
							}
						}
						if(listUniquePatientNames.Count==1) {
							etrans.PatientNameRaw=listUniquePatientNames[0];
						}
						else {
							etrans.PatientNameRaw="("+listUniquePatientNames.Count+" "+Lans.g("Etranss","patients")+")";
						}
						listEtrans.Add(etrans.Copy());
						list835s.Add(x835);
					}
					//The 835 was completely parsed.  Create etrans entries.
					for(int i=0;i<listEtrans.Count;i++) {
						etrans=listEtrans[i];
						X835 x835=list835s[i];
						Etranss.Insert(etrans);//insert
						List<long> listClaimNums=x835.ListClaimsPaid.Select(x => x.ClaimNum).Where(x => x!=0).ToList();
						if(listClaimNums.Count > 0) {
							//Locate the latest etrans entries for the claim based on DateTimeTrans with EType of ClaimSent or Claim_Ren and update the AckCode and AckEtransNum.
							//We overwrite existing acks from 997s, 999s, and 277s.
							command="UPDATE etrans SET AckCode='A', "
								+"AckEtransNum="+POut.Long(etrans.EtransNum)
								+" WHERE EType IN (0,3) "//ClaimSent and Claim_Ren
								+" AND ClaimNum IN("+String.Join(",",listClaimNums.Select(x => POut.Long(x)))+")"
								+" AND ClearinghouseNum="+POut.Long(hqClearinghouseNum)
								+" AND DateTimeTrans > "+POut.DateT(dateTimeTrans.AddDays(-14))
								+" AND DateTimeTrans < "+POut.DateT(dateTimeTrans.AddDays(1));
							Db.NonQ(command);
						}
						//none of the other fields make sense, because this ack could refer to many claims.
					}
				}
				else {//unknown type of X12 report.
					etrans.Etype=EtransType.TextReport;
					Etranss.Insert(etrans);
				}
			}
			else {//not X12
				etrans.Etype=EtransType.TextReport;
				Etranss.Insert(etrans);
			}
		}

		///<summary>Creates new etrans object, does not insert to Etrans table though. Does insert EtransMessageText.</summary>
		public static Etrans CreateEtrans(DateTime dateTimeTrans,long hqClearinghouseNum,string messageText,long userNum){
			Etrans etrans=new Etrans();
			etrans.DateTimeTrans=dateTimeTrans;
			etrans.ClearingHouseNum=hqClearinghouseNum;
			EtransMessageText etransMessageText=new EtransMessageText();
			etransMessageText.MessageText=messageText;
			EtransMessageTexts.Insert(etransMessageText);
			etrans.EtransMessageTextNum=etransMessageText.EtransMessageTextNum;
			etrans.UserNum=userNum;
			return etrans;
		}

		/// <summary>Or Canadian elig.</summary>
		public static DateTime GetLastDate270(long planNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<DateTime>(MethodBase.GetCurrentMethod(),planNum);
			}
			string command="SELECT MAX(DateTimeTrans) FROM etrans "
				+"WHERE (Etype="+POut.Int((int)EtransType.BenefitInquiry270)+" "
				+"OR Etype="+POut.Int((int)EtransType.Eligibility_CA)+") "
				+" AND PlanNum="+POut.Long(planNum);
			return PIn.Date(Db.GetScalar(command));
		}	
	}
}