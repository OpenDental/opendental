using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
					List<long> listMatchedClaimNums=new List<long>();
					for(int i=0;i<listEtrans.Count;i++) {
						etrans=listEtrans[i];
						X835 x835=list835s[i];
						Etranss.Insert(etrans);//insert
						List<long> listClaimNums=x835.ListClaimsPaid.Select(x => x.ClaimNum).Where(x => x!=0).ToList();
						listMatchedClaimNums.AddRange(listClaimNums);
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
					//Get all of the etrans that we should attempt to process.
					List<Hx835_ShortClaim> listShortClaimsAll=Hx835_ShortClaim.GetClaimsFromClaimNums(listMatchedClaimNums);
					List<long> listPlanNumsAll=listShortClaimsAll.Select(x => x.PlanNum).ToList();
					List<InsPlan> listInsPlansAll=InsPlans.GetPlans(listPlanNumsAll);
					List<Etrans> listEtransToProcess=new List<Etrans>();
					List<long> listMatchedClaimNumsToProcess=new List<long>();
					for(int i=0;i<listEtrans.Count;i++) {
						List<long> listClaimNums=list835s[i].ListClaimsPaid.Select(x => x.ClaimNum).Where(x => x!=0).ToList();
						List<Hx835_ShortClaim> listShortClaims=listShortClaimsAll.FindAll(x => ListTools.In(x.ClaimNum,listClaimNums));
						List<long> listPlanNumsForClaims=listShortClaims.Select(x => x.PlanNum).Distinct().ToList();
						List<InsPlan> listInsPlansForClaims=listInsPlansAll.FindAll(x => ListTools.In(x.PlanNum,listPlanNumsForClaims));
						List<Carrier> listCarriers=Carriers.GetForInsPlans(listInsPlansForClaims);
						if(IsEtransAutomatable(listCarriers,list835s[i].PayerName,isFullyAutomatic:true)) {
							listEtransToProcess.Add(listEtrans[i]);
							listMatchedClaimNumsToProcess.AddRange(listClaimNums);
						}
					}
					//Try to auto-process ERAs.
					List<long> listEtransNums=listEtransToProcess.Select(x => x.EtransNum).ToList();
					List<Etrans835Attach> listAllAttaches=Etrans835Attaches.GetForEtransNumOrClaimNums(false,listEtransNums,listMatchedClaimNumsToProcess.ToArray());
					if(listEtransToProcess.Count > 0) {
						//TryAutoProcessEras(listEtransToProcess,listAllAttaches,isFullyAutomatic:true);
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

		///<summary>Uses the attached claims, carriers with a matching name, or the global EraAutomationBehavior pref to determine if an ERA is automatable.
		///If isFullyAutomatic is true, EraAutomationMode.FullyAutomatic is the only mode that will return true.</summary>
		public static bool IsEtransAutomatable(List<Carrier> listCarriersForClaims,string payerName,bool isFullyAutomatic) {
			//No need to check RemotingRole; no call to db.
			List<EraAutomationMode> listAllowedEraAutomationModes=new List<EraAutomationMode>{EraAutomationMode.FullyAutomatic};
			if(!isFullyAutomatic) {
				listAllowedEraAutomationModes.Add(EraAutomationMode.SemiAutomatic);
			}
			if(listCarriersForClaims.Count>0) {
				//The ERA is automatable if it has any claims attached to it that are for a Carrier that allows automation.
				//The ERA is not automatable if it has one or more claims attached and all Carriers for all attached claims don't allow automation.
				return listCarriersForClaims.Any(x => ListTools.In(x.GetEraAutomationMode(),listAllowedEraAutomationModes));
			}
			//If there are no attached claims, we see if the Carrier name from the ERA matches one or more Carriers in the DB.
			List<Carrier> listCarrierNameMatches=Carriers.GetExactNames(payerName);
			if(listCarrierNameMatches.Count==0) {
				//If no claims are attached and no carriers have a matching name, we use the global preference for ERA automation.
				return ListTools.In(PrefC.GetEnum<EraAutomationMode>(PrefName.EraAutomationBehavior),listAllowedEraAutomationModes);
			}
			//If we have no attached claims and one or more names match, we allow automation if any name-matched carriers allow it.
			return listCarrierNameMatches.Any(x => ListTools.In(x.GetEraAutomationMode(),listAllowedEraAutomationModes));
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

				///<summary>Attempts to automatically receive claims and finalize payments for multiple ERAs.</summary>
		public static List<EraAutomationResult> TryAutoProcessEras(List<Etrans> listEtrans,List<Etrans835Attach> listAttaches,bool isFullyAutomatic) {
			//No need to check RemotingRole; no call to db.
			List<long> listEtransMessageTextNums=listEtrans.Select(x => x.EtransMessageTextNum).ToList();
			Dictionary<long,string> dictMessagText835s=EtransMessageTexts.GetMessageTexts(listEtransMessageTextNums);
			List<Etrans835> listEtrans835s=Etrans835s.GetByEtransNums(listEtrans.Select(x => x.EtransNum).ToArray());
			List<EraAutomationResult> listAutomationResults=new List<EraAutomationResult>();
			for(int i=0;i<listEtrans.Count;i++) {
				string messageText835=dictMessagText835s[listEtrans[i].EtransMessageTextNum];
				X12object x12=new X12object(messageText835);
				List<string> listTranSetIds=x12.GetTranSetIds();
				X835Status overallStatus=X835Status.Finalized;
				EraAutomationResult automationResult=new EraAutomationResult();
				//If TranSetId835 is blank and we have multiple TranSetIds in the list, we know we are dealing with an Etrans from 14.2 or an older version
				//that represents multiple transactions from a single 835. We loop through the transactions and process each of them separately.
				for(int j=0;j<listTranSetIds.Count;j++) {//There is always at least 1 transaction.
					X835 x835=new X835(listEtrans[i],messageText835,listTranSetIds[j],listAttaches);
					automationResult=TryAutoProcessEraEob(x835,listAttaches,isFullyAutomatic);
					automationResult.TransactionNumber=j+1;
					automationResult.TransactionCount=listTranSetIds.Count;
					listAutomationResults.Add(automationResult);
					if(automationResult.Status!=X835Status.Finalized) {
						overallStatus=X835Status.Partial;//Could be Unprocessed if 0 were processed, but CreateEtransNote() only cares if Finalized or not.
					}
					Etrans835 etrans835=listEtrans835s.FirstOrDefault(x => x.EtransNum==listEtrans[i].EtransNum);
					if(etrans835==null) {
						etrans835=new Etrans835();
						etrans835.EtransNum=listEtrans[i].EtransNum;
					}
					Etrans835s.Upsert(etrans835,x835);
				}
				Etrans etransOld=listEtrans[i].Copy();
				listEtrans[i].Note=EraAutomationResult.CreateEtransNote(overallStatus,listEtrans[i].Note);
				if(overallStatus==X835Status.Finalized) {
					listEtrans[i].AckCode="Recd";
				}
				else {
					listEtrans[i].AckCode="";
				}
				Etranss.Update(listEtrans[i],etransOld);//Pass an old copy of the Etrans to ensure that we only update the AckCode.
			}
			return listAutomationResults;
		}

		///<summary>Attempts to automatically receive claims and finalize payment for one EOB on an 835.
		///A deposit will be made if the ShowAutoDeposit pref is on.</summary>
		public static EraAutomationResult TryAutoProcessEraEob(X835 x835,List<Etrans835Attach> listAttaches,bool isFullyAutomatic) {
			//No need to check RemotingRole; no call to db.
			//Find claims for manually detached 835 claims. Then, refresh claims.
			List<Hx835_Claim>listDetachedPaidClaims=x835.ListClaimsPaid.FindAll(x => x.ClaimNum==0 && x.IsAttachedToClaim);
			x835.SetClaimNumsForUnattached(null,listDetachedPaidClaims);
			List<Claim> listMatchedClaims=x835.RefreshClaims();
			List<Hx835_Claim> listSplitClaimsToSkip=new List<Hx835_Claim>();
			List<Hx835_Claim> listClaimsPaidToProcess=new List<Hx835_Claim>();
			List<Claim> listClaimsToProcess=new List<Claim>();
			EraAutomationResult automationResult=new EraAutomationResult();
			automationResult.X835Cur=x835;
			//Find matching claims and make sure they are attached and that split claims are attached.
			for(int i=0;i<x835.ListClaimsPaid.Count;i++) {
				if(listSplitClaimsToSkip.Any(x => x.ClpSegmentIndex==x835.ListClaimsPaid[i].ClpSegmentIndex)) {
					//Any Hx835_Claims in listSplitClaimsToSkip already have attaches and will be processed with the first split claim identified for a claim.
					continue;
				}
				//Each iteration in this loop mimics attach creation from FormEtrans835Edit.gridClaimDetails_CellDoubleClick().
				Claim claim=listMatchedClaims.FirstOrDefault(x => x.ClaimNum==x835.ListClaimsPaid[i].ClaimNum);
				if(claim==null) {
					automationResult.ListPatNamesWithoutClaimMatch.Add(x835.ListClaimsPaid[i].PatientName.ToString());
					continue;//Couldn't find a matching claim, so skip to the next 835 claim.
				}
				bool isAttachNeeded=(!x835.ListClaimsPaid[i].IsAttachedToClaim);
				Etrans835Attaches.CreateForClaim(x835,x835.ListClaimsPaid[i],claim.ClaimNum,isAttachNeeded,listAttaches,true);
				//Sync ClaimNum for all split claims in the same group.
				if(x835.ListClaimsPaid[i].IsSplitClaim) {
					List<Hx835_Claim> listOtherSplitClaims=x835.ListClaimsPaid[i].GetOtherNotDetachedSplitClaims();
					for(int j=0;j<listOtherSplitClaims.Count;j++) {
						Etrans835Attaches.CreateForClaim(x835,listOtherSplitClaims[j],claim.ClaimNum,isAttachNeeded,listAttaches,true);
					}
					//These will get processed with the current Hx835_Claim, so we don't want to add them to the listClaimsPaidToProcess.
					listSplitClaimsToSkip.AddRange(listOtherSplitClaims);
				}
				//Add the 835 claim and the claim from DB to the lists of claims to process.
				listClaimsPaidToProcess.Add(x835.ListClaimsPaid[i]);
				listClaimsToProcess.Add(claim);
			}
			List<long> listClaimNums=listMatchedClaims.Select(x => x.ClaimNum).ToList();
			List<ClaimProc> listClaimProcsAll=ClaimProcs.RefreshForClaims(listClaimNums);
			List<Hx835_ShortClaimProc>listShortClaimProcsAll=listClaimProcsAll.Select(x => new Hx835_ShortClaimProc(x)).ToList();
			List<Hx835_ShortClaim> listShortClaims=listMatchedClaims.Select(x => new Hx835_ShortClaim(x)).ToList();
			X835Status status=x835.GetStatus(listShortClaims,listShortClaimProcsAll,listAttaches);
			if(!ListTools.In(status,X835Status.Unprocessed,X835Status.Partial,X835Status.NotFinalized)) {
				automationResult.DidEraStartAsFinalized=true;
				return automationResult;
			}
			//At this point we know that the user is allowed to make ins payments, the ERA has a status of unprocessed or partial,
			//and attaches are created for ERA claims and split claims. Now, we will try to process as many claims on the ERA as we can.
			List<long> listPatNumsForClaims=listMatchedClaims.Select(x => x.PatNum).ToList();
			List<long> listPlanNums=listClaimsToProcess.Select(x => x.PlanNum).ToList();
			List<Patient> listPatients=Patients.GetMultPats(listPatNumsForClaims).ToList();
			List<InsPlan> listInsPlans=InsPlans.GetPlans(listPlanNums);
			List<PayPlan> listValidInsPayPlansForClaims=PayPlans.GetAllValidInsPayPlansForClaims(listClaimsToProcess);
			for(int i=0;i<listClaimsPaidToProcess.Count;i++) {
				//Need to refresh this list in each loop iteration because listClaimProcsAll may have changed.
				listShortClaimProcsAll=listClaimProcsAll.Select(x => new Hx835_ShortClaimProc(x)).ToList();
				if(listClaimsPaidToProcess[i].IsProcessed(listShortClaimProcsAll,listAttaches)) {
					automationResult.CountClaimsAlreadyProcessed++;
					continue;
				}
				Patient patient=listPatients.FirstOrDefault(x => x.PatNum==listClaimsToProcess[i].PatNum);
				InsPlan insPlan=listInsPlans.FirstOrDefault(x => x.PlanNum==listClaimsToProcess[i].PlanNum);
				//Get copy of claimprocs for claim from list so that we don't modify the list.
				List<ClaimProc> listClaimProcsForClaimCopy=listClaimProcsAll
					.Where(x => x.ClaimNum==listClaimsToProcess[i].ClaimNum)
					.Select(x => x.Copy())
					.ToList();
				List<PayPlan> listPayPlans=FilterValidInsPayPlansForClaimHelper(listValidInsPayPlansForClaims,listClaimProcsAll,listClaimsToProcess[i]);
				bool canClaimBeAutoProcessed=automationResult.CanClaimBeAutoProcessed(isFullyAutomatic,patient,insPlan,listClaimsPaidToProcess[i],listPayPlans,listShortClaimProcsAll,
					listClaimProcsForClaimCopy.Select(x => new Hx835_ShortClaimProc(x)).ToList(),
					listAttaches);
				if(!canClaimBeAutoProcessed) {
					continue;
				}
				long payPlanNum=0;
				if(listPayPlans.Count==1) {
					//We won't get here if listPayPlans.Count is greater than 1 because canClaimBeAutoProcessed will be false,
					//so it should be safe to choose the first PayPlanNum in the list.
					payPlanNum=listPayPlans[0].PayPlanNum;
				}
				bool isClaimRecieved=TryImportEraClaimData(x835,listClaimsPaidToProcess[i],listClaimsToProcess[i],
					patient,isAutomatic:true,listClaimProcsForClaimCopy,payPlanNum,automationResult);
				if(isClaimRecieved) {//If claim was received, claimprocs must have been modified and updated to DB, so update the claimprocs for the claim in our list.
					listClaimProcsAll.RemoveAll(x => x.ClaimNum==listClaimsToProcess[i].ClaimNum);
					listClaimProcsAll.AddRange(listClaimProcsForClaimCopy);
				}
			}
			if(automationResult.AreAllClaimsReceived()) {//Only attempt to finalize the payment if automation has processed all of the claims.
				List<Claim> listClaimsForFinalization=x835.GetClaimsForFinalization(listMatchedClaims);
				List<long> listClaimNumsForFinalization=listClaimsForFinalization.Select(x => x.ClaimNum).ToList();
				List<ClaimProc> listClaimProcsForFinalization=listClaimProcsAll.FindAll(x => ListTools.In(x.ClaimNum,listClaimNumsForFinalization));
				automationResult.IsPaymentFinalized=TryFinalizeBatchPayment(x835,listClaimsForFinalization,listClaimProcsForFinalization,
					listPatients[0].ClinicNum,isAutomatic:true,automationResult:automationResult);
			}
			else {//Some claims could not be processed.
				automationResult.PaymentFinalizationError=Lans.g("X835","Payment could not be finalized because one or more claims could not be processed.");
				automationResult.IsPaymentFinalized=false;
			}
			return automationResult;
		}

		///<summary>Returns all etrans.EtransNum which correspond to ERA 835s and which do not have an Etrans835 record yet.</summary>
		public static List<long> GetErasMissingEtrans835(DateTime dateFrom,DateTime dateTo) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),dateFrom,dateTo);
			}
			string dateTimeTrans=DbHelper.DtimeToDate("etrans.DateTimeTrans");
			string command="SELECT etrans.EtransNum FROM etrans "
				+"LEFT JOIN etrans835 ON etrans835.EtransNum=etrans.EtransNum "
				+"WHERE etrans.Etype="+((int)EtransType.ERA_835)
					+" AND "+dateTimeTrans+" >= "+POut.Date(dateFrom)+" AND "+dateTimeTrans+" <= "+POut.Date(dateTo)
					+" AND etrans835.Etrans835Num IS NULL";
			return Db.GetListLong(command);
		}

		///<summary>Must pass in the list of payplans returned by PayPlans.GetAllValidInsPayPlansForClaims(), a list of all claimprocs for claims being processed,
		///and the current claim being processed. Returns a list of insurance payplans that are valid for the claim passed in.</summary>
		public static List<PayPlan> FilterValidInsPayPlansForClaimHelper(List<PayPlan> listPayPlans,List<ClaimProc> listClaimProcsAll,Claim claim) {
			//No need to check RemotingRole; no call to db.
			List<ClaimProc> listClaimProcsForClaim=listClaimProcsAll.FindAll(x => x.ClaimNum==claim.ClaimNum);
			List<long> listPayPlanNumsForClaimProcs=listClaimProcsForClaim.Select(x => x.PayPlanNum).ToList();
			PayPlan payPlanForClaim=listPayPlans.FirstOrDefault(x => ListTools.In(x.PayPlanNum,listPayPlanNumsForClaimProcs));
			if(payPlanForClaim!=null) {
				//If we find an insurance payplan that has claimprocs from the claim attached to it,
				//it is the only payplan we need because a claim can only be associated to one payplan.
				return new List<PayPlan>(){payPlanForClaim};
			}
			//The claim is not associated to a payplan yet, so we will return a list of payplans that are valid for it to associate to.
			List<PayPlan> listValidInsPayPlansForClaim=new List<PayPlan>();
			List<long> listPayPlanNumsForAllClaimProcs=listClaimProcsAll.Select(x => x.PayPlanNum).ToList();
			for(int i=0;i<listPayPlans.Count;i++) {
				if(listPayPlans[i].PatNum!=claim.PatNum
					|| listPayPlans[i].PlanNum!=claim.PlanNum
					|| listPayPlans[i].InsSubNum!=claim.InsSubNum)
				{
					continue;//Exclude payplans that aren't for the pat, insplan, and inssub of the claim.
				}
				if(ListTools.In(listPayPlans[i].PayPlanNum,listPayPlanNumsForAllClaimProcs)) {
					continue;//If the payplan is associated to any claimproc in the list, it must be for a different claim.
				}
				listValidInsPayPlansForClaim.Add(listPayPlans[i]);//We have a payplan that is valid for the claim and isn't attached to another claim.
			}
			return listValidInsPayPlansForClaim;
		}

		///<summary>Returns false if we are automatically processing an ERA but can't proceed. 
		///Enter either by total and/or by procedure, depending on whether or not procedure detail was provided in the 835 for this claim.
		///When isAutomatic is true, processing will not proceed if a by total payment would be made, or if we can't match all claimprocs to payments.
		///This function creates the payment claimprocs.</summary>
		public static bool TryImportEraClaimData(X835 x835,Hx835_Claim claimPaid,
			Claim claim,Patient pat,bool isAutomatic,List<ClaimProc> listClaimProcsForClaim,long insPayPlanNum,EraAutomationResult automationResult=null)
		{
			//No need to check RemotingRole; no call to db.
			List<ClaimProc>listClaimProcsOld=listClaimProcsForClaim.Select(x => x.Copy()).ToList();
			//CapClaim status is not considered because there should not be supplemental payments for capitaiton claims.
			bool isSupplementalPay=(claim.ClaimStatus=="R" || listClaimProcsForClaim.All(x => ListTools.In(x.Status,ClaimProcStatus.Received,ClaimProcStatus.Supplemental)));
			List<Hx835_Claim> listNotDetachedPaidClaims=new List<Hx835_Claim>();
			listNotDetachedPaidClaims.Add(claimPaid);
			if(claimPaid.IsSplitClaim) {
				listNotDetachedPaidClaims.AddRange(claimPaid.GetOtherNotDetachedSplitClaims());
			}
			ClaimProc cpByTotal=new ClaimProc();
			cpByTotal.FeeBilled=0;//All attached claimprocs will show in the grid and be used for the total sum.
			cpByTotal.DedApplied=(double)(listNotDetachedPaidClaims.Sum(x => x.PatientDeductAmt));
			cpByTotal.AllowedOverride=(double)(listNotDetachedPaidClaims.Sum(x => x.AllowedAmt));
			cpByTotal.InsPayAmt=(double)(listNotDetachedPaidClaims.Sum(x => x.InsPaid));
			cpByTotal.WriteOff=0;
			if(!isSupplementalPay) {
				cpByTotal.WriteOff=(double)(listNotDetachedPaidClaims.Sum(x => x.WriteoffAmt));
			}
			List<ClaimProc> listClaimProcsToEdit=new List<ClaimProc>();
			//Automatically set PayPlanNum if there is a payplan with matching PatNum, PlanNum, and InsSubNum that has not been paid in full.
			if(isSupplementalPay) {
				List<ClaimProc> listClaimCopyProcs=listClaimProcsForClaim.Select(x => x.Copy()).ToList();
				if(claimPaid.IsSplitClaim) {
					//Split supplemental payment, only CreateSuppClaimProcs for the sub set of split claim procs.
					foreach(Hx835_Claim splitClaim in listNotDetachedPaidClaims) {
						foreach(Hx835_Proc proc in splitClaim.ListProcs) {
							ClaimProc claimProcForClaim=listClaimCopyProcs.FirstOrDefault(x =>
								x.ProcNum!=0 && ((x.ProcNum==proc.ProcNum)//Consider using Hx835_Proc.TryGetMatchedClaimProc(...)
								|| x.CodeSent==proc.ProcCodeBilled
								&& (decimal)x.FeeBilled==proc.ProcFee
								&& x.Status==ClaimProcStatus.Received
								&& x.TagOD==null)
							);
							if(claimProcForClaim==null) {
								continue;
							}
							claimProcForClaim.TagOD=true;
						}
					}
					//Remove all claimProcs that were not matched, to avoid entering payment on unmatched claimprocs.
					listClaimCopyProcs.RemoveAll(x => x.TagOD==null);
				}
				//Selection logic inside ClaimProcs.CreateSuppClaimProcs() mimics FormClaimEdit "Supplemental" button click.
				listClaimProcsToEdit=ClaimProcs.CreateSuppClaimProcs(listClaimCopyProcs,claimPaid.IsReversal,false);
				listClaimProcsForClaim.AddRange(listClaimProcsToEdit);//listClaimProcsToEdit is a subsSet of listClaimProcsForClaim, like above
			}
			else if(claimPaid.IsSplitClaim) {//Not supplemental, simply a split.
				//For split claims we only want to edit the sub-set of procs that exist on the internal claim.
				foreach(Hx835_Proc proc in listNotDetachedPaidClaims.SelectMany(x => x.ListProcs).ToList()) {
					ClaimProc claimProcFromClaim=listClaimProcsForClaim.FirstOrDefault(x => 
						//Mimics proc matching in claimPaid.GetPaymentsForClaimProcs(...)
						x.ProcNum!=0 && ((x.ProcNum==proc.ProcNum)//Consider using Hx835_Proc.TryGetMatchedClaimProc(...)
						|| (x.CodeSent==proc.ProcCodeBilled
						&& (decimal)x.FeeBilled==proc.ProcFee
						&& x.Status==ClaimProcStatus.NotReceived
						&& x.TagOD==null))
					);
					if(claimProcFromClaim==null) {//Not found, By Total payment row will be inserted.
						continue;
					}
					claimProcFromClaim.TagOD=true;
					listClaimProcsToEdit.Add(claimProcFromClaim);
				}
			}
			else {//Original payment
				//Selection logic mimics FormClaimEdit "By Procedure" button selection logic.
				//Choose the claimprocs which are not received.
				for(int i=0;i<listClaimProcsForClaim.Count;i++) {
					if(listClaimProcsForClaim[i].ProcNum==0) {//Exclude any "by total" claimprocs.  Choose claimprocs for procedures only.
						continue;
					}
					if(listClaimProcsForClaim[i].Status!=ClaimProcStatus.NotReceived) {//Ignore procedures already received.
						continue;
					}
					listClaimProcsToEdit.Add(listClaimProcsForClaim[i]);//Procedures not yet received.
				}
				//If all claimprocs are received, then choose claimprocs if not paid on.
				if(listClaimProcsToEdit.Count==0) {
					for(int i=0;i<listClaimProcsForClaim.Count;i++) {
						if(listClaimProcsForClaim[i].ProcNum==0) {//Exclude any "by total" claimprocs.  Choose claimprocs for procedures only.
							continue;
						}
						if(listClaimProcsForClaim[i].ClaimPaymentNum!=0) {//Exclude claimprocs already paid.
							continue;
						}
						listClaimProcsToEdit.Add(listClaimProcsForClaim[i]);//Procedures not paid yet.
					}
				}
			}
			List<Hx835_Proc> listProcsUnassigned=listNotDetachedPaidClaims.SelectMany(x => x.ListProcs).ToList();
			//For each NotReceived/unpaid procedure on the claim where the procedure information can be successfully located on the EOB, enter the payment information.
			List <List <Hx835_Proc>> listProcsForClaimProcs=Hx835_Claim.GetPaymentsForClaimProcs(listClaimProcsToEdit,listProcsUnassigned);
			if(isAutomatic && listProcsUnassigned.Count > 0) {
				//A claimproc was not found for one or more of the procedures on the ERA claim, so we cannot auto-process.
				if(isSupplementalPay) {
					ClaimProcs.DeleteMany(listClaimProcsToEdit);//Supplemental claimProcs are pre inserted, delete if we do not post payment information.
				}
				if(automationResult!=null) {
					string errorMessage=Lans.g("X835","One or more payments from the ERA could not be matched to a procedure on the claim.");
					automationResult.AddClaimError(pat,errorMessage);
				}
				return false;
			}
			for(int i=0;i<listClaimProcsToEdit.Count;i++) {
				ClaimProc claimProc=listClaimProcsToEdit[i];
				List<Hx835_Proc> listProcsForProcNum=listProcsForClaimProcs[i];
				if(isAutomatic && listProcsForProcNum.IsNullOrEmpty()) {
					//Couldn't find an Hx835_Proc for one of the claimprocs that we are editing, so we cannot auto-process.
					if(isSupplementalPay) {
						ClaimProcs.DeleteMany(listClaimProcsToEdit);//Supplemental claimProcs are pre inserted, delete if we do not post payment information.
					}
					if(automationResult!=null) {
						string errorMessage=Lans.g("X835","Payment information for one or more procedures on the claim were not found on the ERA.");
						automationResult.AddClaimError(pat,errorMessage);
					}
					return false;
				}
				//If listProcsForProcNum.Count==0, then procedure payment details were not not found for this one specific procedure.
				//This can happen with procedures from older 837s, when we did not send out the procedure identifiers, in which case ProcNum would be 0.
				//Since we cannot place detail on the service line, we will leave the amounts for the procedure on the total payment line.
				//If listProcsForPorcNum.Count==1, then we know that the procedure was adjudicated as is or it might have been bundled, but we treat both situations the same way.
				//The 835 is required to include one line for each bundled procedure, which gives is a direct manner in which to associate each line to its original procedure.
				//If listProcForProcNum.Count > 1, then the procedure was either split or unbundled when it was adjudicated by the payer.
				//We will not bother to modify the procedure codes on the claim, because the user can see how the procedure was split or unbunbled by viewing the 835 details.
				//Instead, we will simply add up all of the partial payment lines for the procedure, and report the full payment amount on the original procedure.
				claimProc.DedApplied=0;
				claimProc.AllowedOverride=0;
				claimProc.InsPayAmt=0;
				claimProc.ClaimAdjReasonCodes="";
				if(claimProc.Status==ClaimProcStatus.Preauth) {
					claimProc.InsPayEst=0;
				}
				claimProc.WriteOff=0;
				if(isSupplementalPay) {
					//This mimics how a normal supplemental payment is created in FormClaim edit "Supplemental" button click.
					//We do not do this in ClaimProcs.CreateSuppClaimProcs(...) for matching reasons.
					//Stops the claim totals from being incorrect.
					claimProc.FeeBilled=0;
				}
				StringBuilder sb=new StringBuilder();
				List<string> listClaimAdjReasonCodes=new List<string>();
				//TODO: Will the negative writeoff be cleared back to zero anywhere else (ex ClaimProc edit window)?
				for(int j=0;j<listProcsForProcNum.Count;j++) {
					Hx835_Proc procPaidPartial=listProcsForProcNum[j];
					claimProc.DedApplied+=(double)procPaidPartial.DeductibleAmt;
					//Claim reversals purposefully exclude the Patient Responsibility Amount in the CLP segment.
					//See section 1.10.2.8 'Reversals and Corrections' on page 29 of the 835 documentation.
					if(claimPaid.IsReversal) {
						//Remove the PatientRespAmt from the AllowedAmt since it includes PatientRespAmt.
						//This makes it so a Total Payment row for this particular difference is not erroneously suggested to the user.
						claimProc.AllowedOverride+=(double)(procPaidPartial.AllowedAmt - procPaidPartial.PatientPortionAmt);
					}
					else {
						claimProc.AllowedOverride+=(double)procPaidPartial.AllowedAmt;
					}
					if(claimProc.Status==ClaimProcStatus.Preauth) {
						claimProc.InsPayEst+=(double)procPaidPartial.InsPaid;
					}
					else {
						claimProc.InsPayAmt+=(double)procPaidPartial.InsPaid;
					}
					if(!isSupplementalPay) {
						claimProc.WriteOff+=(double)procPaidPartial.WriteoffAmt;
					}
					if(sb.Length>0) {
						sb.Append("\r\n");
					}
					sb.Append(procPaidPartial.GetRemarks());
					listClaimAdjReasonCodes.AddRange(procPaidPartial.ListProcAdjustments.SelectMany(x => x.ListClaimAdjReasonCodes));
				}
				claimProc.ClaimAdjReasonCodes=string.Join(",",listClaimAdjReasonCodes.Distinct());//Save all distinct reason codes as comma delimited list.
				claimProc.Remarks=sb.ToString();
				if(claim.ClaimType=="PreAuth") {
					claimProc.Status=ClaimProcStatus.Preauth;
				}
				else if(claim.ClaimType=="Cap") {
					//Do nothing.  The claimprocstatus will remain Capitation.
				}
				else {//Received or Supplemental
					if(isSupplementalPay) {
						//This is already set in ClaimProcs.CreateSuppClaimProcs() above, but lets make it clear for others.
						claimProc.Status=ClaimProcStatus.Supplemental;
						claimProc.IsNew=true;//Used in FormEtrans835ClaimPay.FillGridProcedures().
					}
					else {//Received.  Original payment
						claimProc.Status=ClaimProcStatus.Received;
						claimProc.PayPlanNum=insPayPlanNum;//Payment plans do not exist for PreAuths or Capitation claims, by definition.
						if(claimPaid.IsSplitClaim) {
							claimProc.IsNew=true;//Used in FormEtrans835ClaimPay.FillGridProcedures() to highlight the procs on this split claim
						}
					}
					claimProc.DateEntry=DateTime.Now;//Date is was set rec'd or supplemental.
				}
				claimProc.DateCP=DateTime.Today;
			}
			//Limit the scope of the "By Total" payment to the new claimprocs only.
			//This "By Total" payment will account for any procedures that could not be matched to the
			//procedures reported on the ERA due to any changes that occurred after the claim was originally sent.
			for(int i=0;i<listClaimProcsToEdit.Count;i++) {
				ClaimProc claimProc=listClaimProcsToEdit[i];
				cpByTotal.DedApplied-=claimProc.DedApplied;
				cpByTotal.AllowedOverride-=claimProc.AllowedOverride;
				cpByTotal.InsPayAmt-=claimProc.InsPayAmt;
				if(!isSupplementalPay) {
					cpByTotal.WriteOff-=claimProc.WriteOff;//May cause cpByTotal.Writeoff to go negative if the user typed in the value for claimProc.Writeoff.
				}
				if(isAutomatic) {
					//We don't want to set AllowedOverrides for automatically processed claimprocs because the allowed amounts we calculate from ERAs may be
					//inaccurate, and we don't want to automatically create inaccurate blue book data. We set AllowedOverride to -1 for all claimprocs we
					//are editing after making cpByTotal calculations so that we don't
					//throw the calculations off and make a By Total claimproc that we shouldn't.
					claimProc.AllowedOverride=-1;//-1 represents a blank AllowedOverride.
				}
			}
			//The writeoff may be negative if the user manually entered some payment amounts before loading this window, if UCR fee schedule incorrect, and is always negative for reversals.
			if(!claimPaid.IsReversal && cpByTotal.WriteOff<0) {
				cpByTotal.WriteOff=0;
			}
			bool isByTotalIncluded=true;
			//Do not create a total payment if the payment contains all zero amounts, because it would not be useful.  Written to account for potential rounding errors in the amounts.
			if(Math.Round(cpByTotal.DedApplied,2,MidpointRounding.AwayFromZero)==0
				&& Math.Round(cpByTotal.AllowedOverride,2,MidpointRounding.AwayFromZero)==0
				&& Math.Round(cpByTotal.InsPayAmt,2,MidpointRounding.AwayFromZero)==0
				&& Math.Round(cpByTotal.WriteOff,2,MidpointRounding.AwayFromZero)==0)
			{
				isByTotalIncluded=false;
			}
			if(claim.ClaimType=="PreAuth") {
				//In the claim edit window we currently block users from entering PreAuth payments by total, presumably because total payments affect the patient balance.
				isByTotalIncluded=false;
			}
			else if(claim.ClaimType=="Cap") {
				//In the edit claim window, we currently warn and discourage users from entering Capitation payments by total, because total payments affect the patient balance.
				isByTotalIncluded=false;
			}
			if(isByTotalIncluded) {
				if(isAutomatic) {//Cannot create by total payments when processing automatically.
					if(isSupplementalPay) {
						ClaimProcs.DeleteMany(listClaimProcsToEdit);//Supplemental claimProcs are pre inserted, delete if we do not post payment information.
					}
					if(automationResult!=null) {
						string errorMessage=Lans.g("FormEtrans835Edit","Automatic processing would have resulted in an As Total payment.");
						automationResult.AddClaimError(pat,errorMessage);
					}
					return false;
				}
				cpByTotal.Status=(isSupplementalPay?ClaimProcStatus.Supplemental:ClaimProcStatus.Received);//Without this two payment lines would show in the account moudle.
				cpByTotal.ClaimNum=claim.ClaimNum;
				cpByTotal.PatNum=claim.PatNum;
				cpByTotal.ProvNum=claim.ProvTreat;
				cpByTotal.PlanNum=claim.PlanNum;
				cpByTotal.InsSubNum=claim.InsSubNum;
				cpByTotal.DateCP=DateTime.Today;
				cpByTotal.ProcDate=claim.DateService;
				cpByTotal.DateEntry=DateTime.Now;
				cpByTotal.ClinicNum=claim.ClinicNum;
				cpByTotal.Remarks=string.Join("\r\n",listNotDetachedPaidClaims.Select(x => x.GetRemarks()));
				cpByTotal.PayPlanNum=insPayPlanNum;
				cpByTotal.IsNew=true;//Used in FormEtrans835ClaimPay.FillGridProcedures().
				//Add the total payment to the beginning of the list, so that the ins paid amount for the total payment will be highlighted when FormEtrans835ClaimPay loads.
				listClaimProcsForClaim.Insert(0,cpByTotal);
			}
			if(isAutomatic) {
				ReceiveEraPayment(claim,claimPaid,listClaimProcsForClaim,PrefC.GetBool(PrefName.EraIncludeWOPercCoPay),isSupplementalPay,isAutomatic:isAutomatic);
				if(automationResult!=null) {
					automationResult.CountClaimsProcessed++;
				}
				if(PrefC.GetBool(PrefName.ClaimSnapshotEnabled)) {
					Claim claimCur=Claims.GetClaim(listClaimProcsOld[0].ClaimNum);
					if(claimCur.ClaimType!="PreAuth") {
						ClaimSnapshots.CreateClaimSnapshot(listClaimProcsOld,ClaimSnapshotTrigger.InsPayment,claimCur.ClaimType);
					}
				}
			}
			return true;
		}

		///<summary>Receives the claim and to set the claim dates and totals properly. isIncludeWOPercCoPay=true causes WriteOffs to be posted for 
		///ClaimProcs associated to Category Percentage or Medicaid/Flat CoPay insurance plans, false does not post WriteOffs for these insurance plan 
		///types.  isSupplementalPay=true causes claim to not be marked received because Supplemental payments can only be applied to previously 
		///received claims, false allows the claim to be marked received if all ClaimProcs in listClaimProcsForClaim meet requirements.
		public static void ReceiveEraPayment(Claim claim,Hx835_Claim claimPaid,List<ClaimProc> listClaimProcsForClaim,bool isIncludeWOPercCoPay,
			bool isSupplementalPay,InsPlan insPlan=null,bool isAutomatic=false) 
		{
			//No need to check RemotingRole; no call to db.
			Claim claimOld=claim.Copy();
			//Recalculate insurance paid, deductible, and writeoff amounts for the claim based on the final claimproc values, then save the results to the database.
			claim.InsPayAmt=0;
			claim.DedApplied=0;
			claim.WriteOff=0;
			InsPlan insPlanCur=insPlan;
			if(!isIncludeWOPercCoPay && insPlanCur==null) {//Might not want to include WOs, need to check plan type.
				insPlanCur=InsPlans.RefreshOne(claim.PlanNum);
			}
			List<ClaimProc> listAllClaimProcsForProcs=null;
			foreach(ClaimProc claimProc in listClaimProcsForClaim) {
				if(claimProc.Status==ClaimProcStatus.Preauth) {//Mimics FormClaimEdit preauth by procedure logic.
					if(listAllClaimProcsForProcs==null) {
						List<long> listProcNumsForClaim=listClaimProcsForClaim.Select(x => x.ProcNum).ToList();
						listAllClaimProcsForProcs=ClaimProcs.GetForProcs(listProcNumsForClaim);//Get All ClaimProcs for Procs on Claim.
					}
					ClaimProcs.SetInsEstTotalOverride(claimProc.ProcNum,claimProc.PlanNum,
						claimProc.InsSubNum,claimProc.InsPayEst,listAllClaimProcsForProcs);
					ClaimProcs.Update(claimProc);
					continue;//SetInsEstTotalOverride() updates claimProc to database.
				}
				claim.InsPayAmt+=claimProc.InsPayAmt;
				claim.DedApplied+=claimProc.DedApplied;
				//If pref is off, Category Percentage or Medicaid/FlatCopay do not include Writeoff.
				if(!isIncludeWOPercCoPay && insPlanCur!=null && ListTools.In(insPlanCur.PlanType,"","f")) { 
					//Do not include WriteOff in claim total.
					//Also need to change the claimProc directly, this really only matters when automatically recieving the ERA payment.
					claimProc.WriteOff=0;
				}
				claim.WriteOff+=claimProc.WriteOff;
				if(claimProc.ClaimProcNum==0) {//Total payment claimproc which was created in FormEtrans835Edit just before loading this window.
					ClaimProcs.Insert(claimProc);
				}
				else {//Procedure claimproc, because the estimate already existed before entering payment.
					ClaimProcs.Update(claimProc);
				}
			}
			if(!isSupplementalPay//Supplemental payments can only be applied to previously received claims
				//Split claims mark claimProcs recieved one at a time.
				&& listClaimProcsForClaim.All(x => ListTools.In(x.Status,
					ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.CapClaim,ClaimProcStatus.Preauth)))
			{
				//Do not mark received until all claim procs are handled.
				claim.ClaimStatus="R";//Received.
				claim.DateReceived=claimPaid.DateReceived;
			}
			if(isAutomatic) {
				MakeEraClaimAutomationLog(claimOld,claim);
			}
			Claims.Update(claim);
			ClaimProcs.RemoveSupplementalTransfersForClaims(claim.ClaimNum);
			InsBlueBooks.SynchForClaimNums(claim.ClaimNum);
		}

		///<summary>Creates a security log entry indicating that a claim was recieved via ERA automation.</summary>
		private static void MakeEraClaimAutomationLog(Claim claimOld,Claim claimNew) {
			//No need to check RemotingRole; no call to db.
			StringBuilder stringBuilderLog=new StringBuilder();
			stringBuilderLog.Append(Lans.g("X835","Claim payment received during automatic processing of ERA."));
			string old=Lans.g("X835","Old");
			string strNew=Lans.g("X835","new");
			string insurancePayment=Lans.g("X835","insurance payment:");
			string deductableApplied=Lans.g("X835","deductable applied:");
			string writeoff=Lans.g("X835","writeoff:");
			if(!CompareDouble.Equals(claimNew.InsPayAmt,claimOld.InsPayAmt)) {
				stringBuilderLog.Append($" {old} {insurancePayment} {claimOld.InsPayAmt.ToString("c")}, "+
					$"{strNew} {insurancePayment} {claimNew.InsPayAmt.ToString("c")}.");
			}
			if(!CompareDouble.Equals(claimNew.DedApplied,claimOld.DedApplied)) {
				stringBuilderLog.Append($" {old} {deductableApplied} {claimOld.DedApplied.ToString("c")}, "+
					$"{strNew} {deductableApplied} {claimNew.DedApplied.ToString("c")}.");
			}
			if(!CompareDouble.Equals(claimNew.WriteOff,claimOld.WriteOff)) {
				stringBuilderLog.Append($" {old} {writeoff} {claimOld.WriteOff.ToString("c")}, "+
					$"{strNew} {writeoff} {claimNew.WriteOff.ToString("c")}.");
			}
			string stringLog=stringBuilderLog.ToString();
			SecurityLogs.MakeLogEntry(Permissions.InsPayCreate,claimNew.PatNum,stringLog,claimNew.ClaimNum,claimNew.SecDateTEdit);
		}

		///<summary>Returns false if an error is encountered and payment is not finalized.
		///Attempts to finalize the batch insurance payment for an ERA.
		///If isAutomatic is true, the batch payment is created without user input.
		///A deposit will be automatically madef or the payment if the ShowAutoDeposit pref is on.</summary>
		public static bool TryFinalizeBatchPayment(X835 x835,List<Claim> listClaims,List<ClaimProc> listClaimProcsAll,long clinicNum,
			ClaimPayment claimPay=null,bool isAutomatic=false,EraAutomationResult automationResult=null)
		{
			//No need to check RemotingRole; no call to db.
			//Date not considered here, but it will be considered when saving the claimpayment to prevent backdating.
			//When isAutomatic is true this check should be done in the forms that lead to this method being called.
			if(!isAutomatic && !Security.IsAuthorized(Permissions.InsPayCreate)) { 
				return false;
			}
			if(listClaims.Count==0) {
				if(isAutomatic && automationResult!=null) {
					automationResult.PaymentFinalizationError=Lans.g("X835","Payment could not be finalized because all "
						+"claims have been detached from this ERA or are preauths (there is no payment).");
				}
				return false;
			}
			if(listClaims.Exists(x => x.ClaimNum==0 || x.ClaimStatus!="R")) {
				if(isAutomatic && automationResult!=null) {
					automationResult.PaymentFinalizationError=Lans.g("X835","Payment could not be finalized because one or more claims are not marked recieved.");
				}
				return false;
			}
			if(listClaimProcsAll.Exists(x => !ListTools.In(x.Status,ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.CapClaim))) {
				if(isAutomatic && automationResult!=null) {
					automationResult.PaymentFinalizationError=Lans.g("X835","Payment could not be finalized because not all "
						+"claim procedures have a status of Received, Supplemental, or CapClaim.");
				}
				return false;
			}
			#region ClaimPayment creation
			if(claimPay==null) {
				claimPay=new ClaimPayment();
			}
			//Mimics FormClaimEdit.butBatch_Click(...)
			claimPay.CheckDate=MiscData.GetNowDateTime().Date;//Today's date for easier tracking by the office and to avoid backdating before accounting lock dates.
			claimPay.ClinicNum=clinicNum;
			claimPay.CarrierName=x835.PayerName;
			claimPay.CheckAmt=listClaimProcsAll.Where(x => x.ClaimPaymentNum==0).Sum(x => x.InsPayAmt);//Ignore claimprocs associated to previously finalized payments.
			claimPay.CheckNum=x835.TransRefNum;
			long defNum=0;
			if(x835._paymentMethodCode=="CHK") {//Physical check
				defNum=Defs.GetByExactName(DefCat.InsurancePaymentType,"Check");
			}
			else if(x835._paymentMethodCode=="ACH") {//Electronic check
				defNum=Defs.GetByExactName(DefCat.InsurancePaymentType,"EFT");
			}
			else if(x835._paymentMethodCode=="FWT") {//Wire transfer
				defNum=Defs.GetByExactName(DefCat.InsurancePaymentType,"Wired");
			}
			claimPay.PayType=defNum;
			claimPay.IsPartial=true;//This flag is changed to "false" when the payment is finalized from inside FormClaimPayBatch.
			if(isAutomatic) {
				//We shouldn't automatically make payments that don't match the amount on the ERA.
				if(!CompareDecimal.IsEqual((decimal)claimPay.CheckAmt,x835.InsPaid)) {
					if(automationResult!=null) {
						automationResult.PaymentFinalizationError=Lans.g("X835","Payment could not be finalized because the "
							+"amount paid for claim procedures does not match the total payment from the ERA.");
					}
					return false;
				}
				claimPay.IsPartial=false;
				AutoFinalizeBatchPaymentHelper(claimPay,listClaimProcsAll);//Inserts the claimPay
			}
			else {
				ClaimPayments.Insert(claimPay);
				List<ClaimProc> listClaimProcsToUpdate=listClaimProcsAll.FindAll(x => x.ClaimPaymentNum==0 && x.IsTransfer==false);
				for(int i=0;i<listClaimProcsToUpdate.Count;i++) {
					listClaimProcsToUpdate[i].ClaimPaymentNum=claimPay.ClaimPaymentNum;
					ClaimProcs.Update(listClaimProcsToUpdate[i]);
				}
			}
			#endregion
			return true;
		}

		///<summary>Creates a deposit for the claim payment if PrefName.ShowAutoDeposit is true, logs the claim payment, and updates ClaimProcs.</summary>
		private static void AutoFinalizeBatchPaymentHelper(ClaimPayment claimPay,List<ClaimProc> listClaimProcs) {
			//No need to check RemotingRole; no call to db.
			//AutoDeposit. Normally done in FormClaimPayEdit.butOK_Click()
			bool doAutoDeposit=PrefC.GetBool(PrefName.ShowAutoDeposit);
			if(doAutoDeposit) {
				Deposit deposit=new Deposit();
				//I don't think there is any way for us to know what batch number or account to use, so they are left as default values.
				//deposit.Batch="";
				//deposit.DepositAccountNum=0;
				deposit.Amount=claimPay.CheckAmt;
				deposit.DateDeposit=claimPay.CheckDate;
				claimPay.DepositNum=Deposits.Insert(deposit);
				//Log deposit
				SecurityLogs.MakeLogEntry(Permissions.DepositSlips,0
					,Lans.g("FormClaimPayEdit","Auto Deposit created during automatic processing of ERA:")+" "+deposit.DateDeposit.ToShortDateString()
					+" "+Lans.g("FormClaimPayEdit","New")+" "+deposit.Amount.ToString("c"));
			}
			//Insert ClaimPayment and make log that would normally be made in FormClaimPayEdit.butOK_Click()
			ClaimPayments.Insert(claimPay);
			SecurityLogs.MakeLogEntry(Permissions.InsPayCreate,0,
				Lans.g("FormClaimPayEdit","Insurance Payment created during automatic processing of ERA.")+" "
				+Lans.g("FormClaimPayEdit","Carrier Name: ")+claimPay.CarrierName+", "
				+Lans.g("FormClaimPayEdit","Total Amount: ")+claimPay.CheckAmt.ToString("c")+", "
				+Lans.g("FormClaimPayEdit","Check Date: ")+claimPay.CheckDate.ToShortDateString()+", "//Date the check is entered in the system (i.e. today)
				+"ClaimPaymentNum: "+claimPay.ClaimPaymentNum);//Column name, not translated.
			//Update the ClaimPaymentNum and DateCP for ClaimProcs
			//Only update claimprocs that are not already associated to another claim payment.
			//This will happen when this ERA contains claim reversals or corrections, both are entered as supplemental payments.
			List<long> listClaimProcNums=listClaimProcs
				.Where(x => x.ClaimPaymentNum==0 && x.IsTransfer==false)
				.Select(x => x.ClaimProcNum)
				.ToList();
			ClaimProcs.UpdateForClaimPayment(listClaimProcNums,claimPay);
			//Sets DateInsFinalized for ClaimProcs. Normally done in FormClaimEdit.FormFinalizePaymentHelper()
			ClaimProcs.DateInsFinalizedHelper(claimPay.ClaimPaymentNum,PrefC.DateClaimReceivedAfter);
		}

	}
}