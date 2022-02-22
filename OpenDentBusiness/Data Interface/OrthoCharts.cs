using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness{
	///<summary></summary>
	public class OrthoCharts{
		///<summary></summary>
		public static List<OrthoChart> GetAll() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoChart>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT * FROM orthochart";
			return Crud.OrthoChartCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<OrthoChart> GetAllForPatient(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoChart>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM orthochart WHERE PatNum ="+POut.Long(patNum)
				//FieldValue='' were stored as a result of a bug. DBM now removes those rows from the DB. This prevents them from being seen until DBM is run.
				+" AND FieldValue!=''";
			return Crud.OrthoChartCrud.SelectMany(command);
		}

		///<summary></summary>
		public static List<OrthoChart> GetByOrthoChartRowNums(List<long> listOrthoChartRowNums) {
			if(listOrthoChartRowNums.IsNullOrEmpty()) {
				return new List<OrthoChart>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoChart>>(MethodBase.GetCurrentMethod(),listOrthoChartRowNums);
			}
			string command=$"SELECT * FROM orthochart WHERE OrthoChartRowNum IN({string.Join(",",listOrthoChartRowNums.Select(x => POut.Long(x)))}) ORDER BY OrthoChartNum";
			return Crud.OrthoChartCrud.SelectMany(command);
		}

		///<summary>Gets all distinct field names used by any ortho chart.  Useful for displaying the "available" display fields.</summary>
		public static List<string> GetDistinctFieldNames() {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<string>>(MethodBase.GetCurrentMethod());
			}
			string command="SELECT FieldName FROM orthochart GROUP BY FieldName";
			List<string> listFieldNames=Db.GetListString(command);
			listFieldNames.Add("Signature");//OrthoChart will always have a Signature field.
			return listFieldNames.Distinct().ToList();
		}

		///<summary></summary>
		public static long Insert(OrthoChart orthoChart) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				orthoChart.OrthoChartNum=Meth.GetLong(MethodBase.GetCurrentMethod(),orthoChart);
				return orthoChart.OrthoChartNum;
			}
			return Crud.OrthoChartCrud.Insert(orthoChart);
		}

		///<summary></summary>
		public static void Update(OrthoChart orthoChart) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoChart);
				return;
			}
			Crud.OrthoChartCrud.Update(orthoChart);
		}

		///<summary></summary>
		public static void Update(OrthoChart orthoChart,OrthoChart oldOrthoChart) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoChart,oldOrthoChart);
				return;
			}
			string command="";
			if(orthoChart.PatNum != oldOrthoChart.PatNum) {
				if(command!=""){ command+=",";}
				command+="PatNum = "+POut.Long(orthoChart.PatNum)+"";
			}
			if(orthoChart.DateService != oldOrthoChart.DateService) {
				if(command!=""){ command+=",";}
				command+="DateService = "+POut.Date(orthoChart.DateService)+"";
			}
			if(orthoChart.FieldName != oldOrthoChart.FieldName) {
				if(command!=""){ command+=",";}
				command+="FieldName = '"+POut.String(orthoChart.FieldName)+"'";
			}
			if(orthoChart.FieldValue != oldOrthoChart.FieldValue) {
				if(command!=""){ command+=",";}
				command+="FieldValue = '"+POut.String(orthoChart.FieldValue)+"'";
			}
			if(orthoChart.UserNum != oldOrthoChart.UserNum) {
				if(command!="") { command+=","; }
				command+="UserNum = '"+POut.Long(orthoChart.UserNum)+"'";
			}
			if(command==""){
				return;
			}
			command="UPDATE orthochart SET "+command
				+" WHERE OrthoChartNum = "+POut.Long(oldOrthoChart.OrthoChartNum);
			Db.NonQ(command);
			//Crud.OrthoChartCrud.Update(orthoChartNew,orthoChartOld);
		}
		  
		///<summary>Ortho charts were briefly not deleted between 05/06/2014 and 01/02/2015.  Deleting occurs regularly when FieldValue="".</summary>
		public static void Delete(long orthoChartNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),orthoChartNum);
				return;
			}
			string command= "DELETE FROM orthochart WHERE OrthoChartNum = "+POut.Long(orthoChartNum);
			Db.NonQ(command);
		}

		///<summary>Modified Sync pattern for the OrthoChart.  We cannot use the standard Sync pattern because we have to perform logging when updating 
		///or deleting.</summary>
		public static void Sync(Patient patCur,List<OrthoChartRow> listOrthoChartRows,List<OrthoChart> listNew,List<DisplayField> listOrthDisplayFields) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patCur,listOrthoChartRows,listNew,listOrthDisplayFields);
				return;
			}
			List<OrthoChartRow> listOrthChartRowsForPat=OrthoChartRows.GetAllForPatient(patCur.PatNum,doIncludeOrthoCharts:false);
			List<OrthoChart> listDB=GetByOrthoChartRowNums(listOrthChartRowsForPat.Select(x => x.OrthoChartRowNum).ToList());
			//This code is mostly a copy of the Crud sync.  Differences include sort and logging.
			//Inserts, updates, or deletes database rows to match supplied list.
			//Adding items to lists changes the order of operation. All inserts are completed first, then updates, then deletes.
			List<OrthoChart> listIns    =new List<OrthoChart>();
			List<OrthoChart> listUpdNew =new List<OrthoChart>();
			List<OrthoChart> listUpdDB  =new List<OrthoChart>();
			List<OrthoChart> listDel    =new List<OrthoChart>();
			List<string> listColNames=new List<string>();
			//Remove fields from both lists that are not currently set to display.
			for(int i=0;i<listOrthDisplayFields.Count;i++){
				listColNames.Add(listOrthDisplayFields[i].Description);
			}
			for(int i=listDB.Count-1;i>=0;i--){
				if(!listColNames.Contains(listDB[i].FieldName)){
					listDB.RemoveAt(i);
				}
			}
			listNew=listNew.FindAll(x => listColNames.Contains(x.FieldName));
			listNew.Sort(OrthoCharts.Sort);
			listDB.Sort(OrthoCharts.Sort);
			int idxNew=0;
			int idxDB=0;
			OrthoChart fieldNew;
			OrthoChart fieldDB;
			//Because both lists have been sorted using the same criteria, we can now walk each list to determine which list contians the next element.  The next element is determined by Primary Key.
			//If the New list contains the next item it will be inserted.  If the DB contains the next item, it will be deleted.  If both lists contain the next item, the item will be updated.
			while(idxNew<listNew.Count || idxDB<listDB.Count) {
				fieldNew=null;
				if(idxNew<listNew.Count) {
					fieldNew=listNew[idxNew];
				}
				fieldDB=null;
				if(idxDB<listDB.Count) {
					fieldDB=listDB[idxDB];
				}
				//begin compare
				if(fieldNew!=null && fieldDB==null) {//listNew has more items, listDB does not.
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew==null && fieldDB!=null) {//listDB has more items, listNew does not.
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				else if(fieldNew.OrthoChartRowNum<fieldDB.OrthoChartRowNum) {//newPK less than dbPK, newItem is 'next'
					listIns.Add(fieldNew);
					idxNew++;
					continue;
				}
				else if(fieldNew.OrthoChartRowNum>fieldDB.OrthoChartRowNum) {//dbPK less than newPK, dbItem is 'next'
					listDel.Add(fieldDB);
					idxDB++;
					continue;
				}
				//Both lists contain the 'next' item, update required
				listUpdNew.Add(fieldNew);
				listUpdDB.Add(fieldDB);
				idxNew++;
				idxDB++;
			}
			//Commit changes to DB
			for(int i=0;i<listIns.Count;i++) {
				if(listIns[i].FieldValue=="") {//do not insert new blank values. This happens when fields from today are not used.
					continue;
				}
				Insert(listIns[i]);
			}
			for(int i=0;i<listUpdNew.Count;i++) {
				if(listUpdDB[i].FieldValue==listUpdNew[i].FieldValue) {
					continue;//values equal. do not update/create log entry.
				}
				if(listUpdNew[i].FieldValue!="") {//Actually update rows that have a new value.
					Update(listUpdNew[i],listUpdDB[i]);
				}
				else {//instead of updating to a blank value, we delete the row from the DB.
					listDel.Add(listUpdDB[i]);
				}
				#region security log entry
				string logText=Lans.g("OrthoCharts","Ortho chart field edited. ")
					+Lans.g("OrthoCharts","Field name")+": "+listUpdNew[i].FieldName+"\r\n";
				//Do not log the Base64 information into the audit trail if this is a signature column, log some short descriptive text instead.
				logText+=Lans.g("OrthoCharts","Old value")+": \""+listUpdDB[i].FieldValue+"\"  "
							+Lans.g("OrthoCharts","New value")+": \""+listUpdNew[i].FieldValue+"\" ";
				logText+=Lans.g("OrthoCharts","OrthoChartRowNum")+": \""+listUpdNew[i].OrthoChartRowNum+"\" ";
				OrthoChartRow orthoChartRow=listOrthoChartRows.FirstOrDefault(x => x.OrthoChartRowNum==listUpdNew[i].OrthoChartRowNum);
				if(orthoChartRow!=null) {
					logText+=orthoChartRow.DateTimeService.ToString("yyyyMMdd");
				}
				SecurityLogs.MakeLogEntry(Permissions.OrthoChartEditFull,patCur.PatNum,logText);
				#endregion
			}
			for(int i=0;i<listDel.Count;i++) {//All logging should have been performed above in the "Update block"
				Delete(listDel[i].OrthoChartNum);
			}
		}

		///<summary>Used for ortho chart audit trail.  Attempts to parse the DateOfService from the security log text. If it is unable to parse the date, it will return MinDate.
		///<para>Returning MinDate from this function results in the audit trail entries for multiple dates of service displaying intermingled on the date "0001-01-01", harmless.</para></summary>
		public static DateTime GetOrthoDateFromLog(SecurityLog securityLog) {
			//There are 3 cases to try, in order of ascending complexity. If a simple case succeeds at parsing a date, that date is returned.
			//1) Using the new log text, there should be an 8 digit number at the end of each log entry. This is in the format "YYYYMMDD" and should be culture invariant.
			//2) Using the old log text, the Date of service appeared as a string in the middle of the text block.
			//3) Using the old log text, the Date of service appeared as a string in the middle of the text block in a culture dependant format.
			DateTime retVal=DateTime.MinValue;
			#region Ideal Case, Culture invariant
			try {
				string dateString=securityLog.LogText.Substring(securityLog.LogText.Length-8,8);
				retVal=new DateTime(int.Parse(dateString.Substring(0,4)),int.Parse(dateString.Substring(4,2)),int.Parse(dateString.Substring(6,2)));
				if(retVal!=DateTime.MinValue) {
					return retVal;
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			#endregion
			#region Depricated, log written in english
			try {
				if(securityLog.LogText.StartsWith("Ortho chart field edited.  Field date: ")) {
					retVal=DateTime.Parse(securityLog.LogText.Substring("Ortho chart field edited.  Field date: ".Length,10));//Date usually in the format MM/DD/YYYY, unless using en-UK for example
					if(retVal!=DateTime.MinValue) {
						return retVal;
					}
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			#endregion
			#region Depricated, log written in current culture
			try {
				if(securityLog.LogText.StartsWith(Lans.g("FormOrthoChart","Ortho chart field edited.  Field date"))) {
					string[] tokens=securityLog.LogText.Split(new string[] { ": " },StringSplitOptions.None);
					retVal=DateTime.Parse(tokens[1].Replace(Lans.g("FormOrthoChart","Field name"),""));
					if(retVal!=DateTime.MinValue) {
						return retVal;
					}
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			#endregion
			#region Depricated, log written in non-english non-current culture
				//not particularly common or useful.
			#endregion
			return retVal;//Should be DateTime.MinVal if we are returning here.
		}

		public static int Sort(OrthoChart x,OrthoChart y) {
			return x.OrthoChartNum.CompareTo(y.OrthoChartNum);
		}

		///<summary>Gets the hashstring for generating signatures.
		///Should only be used when saving signatures, for validating see GetKeyDataForSignatureHash() and GetHashStringForSignature()</summary>
		public static string GetKeyDataForSignatureSaving(List<OrthoChart> listOrthoCharts,DateTime dateService) {
			//No need to check RemotingRole; no call to db.
			string keyData=GetKeyDataForSignatureHash(null,listOrthoCharts,dateService,doUsePatName:false);
			return GetHashStringForSignature(keyData);
		}

		///<summary>Gets the key data string needed to create a hashstring to be used later when filling the signature.
		///This is done seperate of the hashing so that new line replacements can be done when validating signatures before hashing.
		///The reason for the doUsePatName parameter is that we originally hashed ortho charts using the patient name. Later we switched to not use
		///the patient name. For ortho charts that existed before we made the switch, we have to use the patient name when hashing.</summary>
		public static string GetKeyDataForSignatureHash(Patient pat,List<OrthoChart> listOrthoCharts,DateTime dateService,bool doUsePatName=false) {
			//No need to check RemotingRole; no call to db.
			StringBuilder strb=new StringBuilder();
			if(doUsePatName) {
				strb.Append(pat.FName);
				strb.Append(pat.LName);
			}
			string strDateService=dateService.ToString("yyyyMMdd");
			if(dateService.TimeOfDay!=TimeSpan.Zero) {
				strDateService=dateService.ToString();
			}
			strb.Append(strDateService);
			foreach(OrthoChart orChart in listOrthoCharts.OrderBy(x=>x.FieldName)) {
				strb.Append(orChart.FieldName);
				strb.Append(orChart.FieldValue);
			}
			return strb.ToString();
		}
		
		///<summary>Gets the hashstring from the provided string that is typically generated from GetStringForSignatureHash().
		///This is done seperate of building the string so that new line replacements can be done when validating signatures before hashing.</summary>
		public static string GetHashStringForSignature(string str) {
			return Encoding.ASCII.GetString(ODCrypt.MD5.Hash(Encoding.UTF8.GetBytes(str)));
		}

		/*
		Only pull out the methods below as you need them.  Otherwise, leave them commented out.

		///<summary></summary>
		public static List<OrthoChart> Refresh(long patNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<OrthoChart>>(MethodBase.GetCurrentMethod(),patNum);
			}
			string command="SELECT * FROM orthochart WHERE PatNum = "+POut.Long(patNum);
			return Crud.OrthoChartCrud.SelectMany(command);
		}

		///<summary>Gets one OrthoChart from the db.</summary>
		public static OrthoChart GetOne(long orthoChartNum){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb){
				return Meth.GetObject<OrthoChart>(MethodBase.GetCurrentMethod(),orthoChartNum);
			}
			return Crud.OrthoChartCrud.SelectOne(orthoChartNum);
		}

		
		*/
	}
}