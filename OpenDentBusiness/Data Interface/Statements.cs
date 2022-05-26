using OpenDentBusiness.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using CodeBase;

namespace OpenDentBusiness {

	///<summary></summary>
	public class Statements{
		#region Get Methods
		///<Summary>Gets one statement from the database.</Summary>
		public static Statement GetStatement(long statementNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Statement>(MethodBase.GetCurrentMethod(),statementNum);
			}
			return Crud.StatementCrud.SelectOne(statementNum);
		}

		///<summary>Gets a list of statements based on the passed in primary keys. If clinics are enabled and the preference 
		///PrintStatementsAlphabetically is set, the statements will be sorted by patients last name then first name.</summary>
		public static List<Statement> GetStatements(List<long> listStatementNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Statement>>(MethodBase.GetCurrentMethod(),listStatementNums);
			}
			if(listStatementNums==null || listStatementNums.Count < 1) {
				return new List<Statement>();
			}
			bool sortByPatientName=(PrefC.HasClinicsEnabled && PrefC.GetBool(PrefName.PrintStatementsAlphabetically));
			string command="SELECT * FROM statement ";
			if(sortByPatientName) {
				command+="INNER JOIN patient ON patient.PatNum = statement.PatNum ";
			}
			command+="WHERE StatementNum IN ("+string.Join(",",listStatementNums)+") ";
			if(sortByPatientName) {
				command+="ORDER BY patient.LName,patient.FName";
			}
			return Crud.StatementCrud.SelectMany(command);
		}

		///<summary>For orderBy, use 0 for BillingType and 1 for PatientName.</summary>
		public static DataTable GetBilling(bool isSent,int orderBy,DateTime dateFrom,DateTime dateTo,List<long> clinicNums){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),isSent,orderBy,dateFrom,dateTo,clinicNums);
			}
			DataTable table=new DataTable();
			DataRow row;
			//columns that start with lowercase are altered for display rather than being raw data.
			table.Columns.Add("amountDue");
			table.Columns.Add("balTotal");
			table.Columns.Add("billingType");
			table.Columns.Add("insEst");
			table.Columns.Add("IsSent");
			table.Columns.Add("lastStatement");
			table.Columns.Add("mode");
			table.Columns.Add("name");
			table.Columns.Add("PatNum");
			table.Columns.Add("payPlanDue");
			table.Columns.Add("StatementNum");
			table.Columns.Add("SuperFamily");
			table.Columns.Add("ClinicNum");
			string command="SELECT guar.BalTotal,patient.BillingType,patient.FName,guar.InsEst,statement.IsSent,"
				+"IFNULL(MAX(s2.DateSent),"+POut.Date(DateTime.MinValue)+") LastStatement,"
				+"patient.LName,patient.MiddleI,statement.Mode_,guar.PayPlanDue,patient.Preferred,"
				+"statement.PatNum,statement.StatementNum,statement.SuperFamily,patient.ClinicNum "
				+"FROM statement "
				+"LEFT JOIN patient ON statement.PatNum=patient.PatNum "
				+"LEFT JOIN patient guar ON guar.PatNum=patient.Guarantor "
				+"LEFT JOIN statement s2 ON s2.PatNum=patient.PatNum "
				+"AND s2.IsSent=1 ";
			if(PrefC.GetBool(PrefName.BillingIgnoreInPerson)) {
				command+="AND s2.Mode_ !=1 ";
			}
			if(orderBy==0){//BillingType
				command+="LEFT JOIN definition ON patient.BillingType=definition.DefNum ";
			}
			command+="WHERE statement.IsSent="+POut.Bool(isSent)+" ";
			//if(dateFrom.Year>1800){
			command+="AND statement.DateSent>="+POut.Date(dateFrom)+" ";//greater than midnight this morning
			//}
			//if(dateFrom.Year>1800){
			command+="AND statement.DateSent<"+POut.Date(dateTo.AddDays(1))+" ";//less than midnight tonight
			//}
			if(clinicNums.Count>0) {
				command+="AND patient.ClinicNum IN ("+string.Join(",",clinicNums)+") ";
			}
			command+="GROUP BY guar.BalTotal,patient.BillingType,patient.FName,guar.InsEst,statement.IsSent,"
				+"patient.LName,patient.MiddleI,statement.Mode_,guar.PayPlanDue,patient.Preferred,"
				+"statement.PatNum,statement.StatementNum,statement.SuperFamily "; 
			if(orderBy==0){//BillingType
				command+="ORDER BY definition.ItemOrder,patient.LName,patient.FName,patient.MiddleI,guar.PayPlanDue,statement.StatementNum";
			}
			else{
				command+="ORDER BY patient.LName,patient.FName,statement.StatementNum";
			}
			DataTable rawTable=Db.GetTable(command);
			double balTotal;
			double insEst;
			double payPlanDue;
			DateTime lastStatement;
			List<Patient> listFamilyGuarantors;
			foreach(DataRow rawRow in rawTable.Rows) {
				row=table.NewRow();
				if(rawRow["SuperFamily"].ToString()=="0") {//not a super statement, just get bal info from guarantor
					balTotal=PIn.Double(rawRow["BalTotal"].ToString());
					insEst=PIn.Double(rawRow["InsEst"].ToString());
					payPlanDue=PIn.Double(rawRow["PayPlanDue"].ToString());
				}
				else {//super statement, add all guar positive balances to get bal total for super family
					listFamilyGuarantors=Patients.GetSuperFamilyGuarantors(PIn.Long(rawRow["SuperFamily"].ToString())).FindAll(x => x.HasSuperBilling);
					//exclude fams with neg balances in the total for super family stmts (per Nathan 5/25/2016)
					if(PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
						listFamilyGuarantors=listFamilyGuarantors.FindAll(x => x.BalTotal>0);
						insEst=0;
					}
					else {
						listFamilyGuarantors=listFamilyGuarantors.FindAll(x => (x.BalTotal-x.InsEst)>0);
						insEst=listFamilyGuarantors.Sum(x => x.InsEst);
					}
					balTotal=listFamilyGuarantors.Sum(x => x.BalTotal);
					payPlanDue=listFamilyGuarantors.Sum(x => x.PayPlanDue);
				}
				row["amountDue"]=(balTotal-insEst).ToString("F");
				row["balTotal"]=balTotal.ToString("F");;
				row["billingType"]=Defs.GetName(DefCat.BillingTypes,PIn.Long(rawRow["BillingType"].ToString()));
				if(insEst==0){
					row["insEst"]="";
				}
				else{
					row["insEst"]=insEst.ToString("F");
				}
				row["IsSent"]=rawRow["IsSent"].ToString();
				lastStatement=PIn.Date(rawRow["LastStatement"].ToString());
				if(lastStatement.Year<1880){
					row["lastStatement"]="";
				}
				else{
					row["lastStatement"]=lastStatement.ToShortDateString();
				}
				row["mode"]=Lans.g("enumStatementMode",((StatementMode)PIn.Int(rawRow["Mode_"].ToString())).ToString());
				row["name"]=Patients.GetNameLF(rawRow["LName"].ToString(),rawRow["FName"].ToString(),rawRow["Preferred"].ToString(),rawRow["MiddleI"].ToString());
				row["PatNum"]=rawRow["PatNum"].ToString();
				if(payPlanDue==0){
					row["payPlanDue"]="";
				}
				else{
					row["payPlanDue"]=payPlanDue.ToString("F");
				}
				row["StatementNum"]=rawRow["StatementNum"].ToString();
				row["SuperFamily"]=rawRow["SuperFamily"].ToString();
				row["ClinicNum"]=rawRow["ClinicNum"].ToString();
				table.Rows.Add(row);
			}
			return table;
		}

		///<summary>This query is flawed.</summary>
		public static DataTable GetStatementNotesPracticeWeb(long PatientID) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),PatientID);
			}
			string command=@"SELECT Note FROM statement Where Patnum="+PatientID;
			return Db.GetTable(command);
		}

		///<summary>This query is flawed.</summary>
		public static Statement GetStatementInfoPracticeWeb(long PatientID) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<Statement>(MethodBase.GetCurrentMethod(),PatientID);
			}
			string command=@"Select SinglePatient,DateRangeFrom,DateRangeTo,Intermingled
                        FROM statement WHERE PatNum = "+PatientID;
			return Crud.StatementCrud.SelectOne(command);
		}

		///<summary>Fetches StatementNums restricted by the DateTStamp, PatNums and a limit of records per patient. If limitPerPatient is zero all StatementNums of a patient are fetched</summary>
		public static List<long> GetChangedSinceStatementNums(DateTime changedSince,List<long> eligibleForUploadPatNumList,int limitPerPatient) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),changedSince,eligibleForUploadPatNumList,limitPerPatient);
			}
			List<long> statementnums = new List<long>();
			string limitStr="";
			if(limitPerPatient>0) {
				limitStr="LIMIT "+ limitPerPatient;
			}
			DataTable table;
			// there are possibly more efficient ways to implement this using a single sql statement but readability of the sql can be compromised
			if(eligibleForUploadPatNumList.Count>0) {
				for(int i=0;i<eligibleForUploadPatNumList.Count;i++) {
					string command="SELECT StatementNum FROM statement WHERE DateTStamp > "+POut.DateT(changedSince)+" AND PatNum='" 
						+eligibleForUploadPatNumList[i].ToString()+"' ORDER BY DateSent DESC, StatementNum DESC "+limitStr;
					table=Db.GetTable(command);
					for(int j=0;j<table.Rows.Count;j++) {
						statementnums.Add(PIn.Long(table.Rows[j]["StatementNum"].ToString()));
					}
				}
			}
			return statementnums;
		}

		///<summary>Used along with GetChangedSinceStatementNums</summary>
		public static List<Statement> GetMultStatements(List<long> statementNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<Statement>>(MethodBase.GetCurrentMethod(),statementNums);
			}
			string strStatementNums="";
			DataTable table;
			if(statementNums.Count>0) {
				for(int i=0;i<statementNums.Count;i++) {
					if(i>0) {
						strStatementNums+="OR ";
					}
					strStatementNums+="StatementNum='"+statementNums[i].ToString()+"' ";
				}
				string command="SELECT * FROM statement WHERE "+strStatementNums;
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			Statement[] multStatements=Crud.StatementCrud.TableToList(table).ToArray();
			List<Statement> statementList=new List<Statement>(multStatements);
			return statementList;
		}

		///<summary>Returns an email message for the patient based on the statement passed in.</summary>
		public static EmailMessage GetEmailMessageForStatement(Statement stmt,Patient pat) {
			if(stmt.PatNum!=pat.PatNum) {
				string logMsg=Lans.g("Statements","Mismatched PatNums detected between current patient and current statement:")+"\r\n"
					+Lans.g("Statements","Statement PatNum:")+" "+stmt.PatNum+" "+Lans.g("Statements","(assumed correct)")+"\r\n"
					+Lans.g("Statements","Patient PatNum:")+" "+pat.PatNum+" "+Lans.g("Statements","(possibly incorrect)");
				SecurityLogs.MakeLogEntry(Permissions.StatementPatNumMismatch,stmt.PatNum,logMsg,LogSources.Diagnostic);
			}
			//No need to check RemotingRole; no call to db.
			EmailMessage message=new EmailMessage();
			message.PatNum=pat.PatNum;
			message.ToAddress=pat.Email;
			EmailAddress emailAddress=EmailAddresses.GetByClinic(pat.ClinicNum);
			message.FromAddress=EmailAddresses.OverrideSenderAddressClinical(emailAddress,pat.ClinicNum).GetFrom();
			string str;
			if(stmt.EmailSubject!=null && stmt.EmailSubject!="") {
				str=stmt.EmailSubject;//Set str to the email subject if one was already set.
			}
			else {//Subject was not set.  Set str to the default billing email subject.
				str=PrefC.GetString(PrefName.BillingEmailSubject);
			}
			message.Subject=Statements.ReplaceVarsForEmail(str,pat,stmt);
			if(stmt.EmailBody!=null && stmt.EmailBody!="") {
				str=stmt.EmailBody;//Set str to the email body if one was already set.
			}
			else {//Body was not set.  Set str to the default billing email body text.
				str=PrefC.GetString(PrefName.BillingEmailBodyText);
			}
			message.BodyText=Statements.ReplaceVarsForEmail(str,pat,stmt);
			message.MsgType=EmailMessageSource.Statement;
			return message;
		}

		public static EmailMessage GetEmailMessageForPortalStatement(Statement stmt,Patient pat) {
			//No need to check RemotingRole; no call to db.
			if(stmt.PatNum!=pat.PatNum) {
				string logMsg=Lans.g("Statements","Mismatched PatNums detected between current patient and current statement:")+"\r\n"
					+Lans.g("Statements","Statement PatNum:")+" "+stmt.PatNum+" "+Lans.g("Statements","(assumed correct)")+"\r\n"
					+Lans.g("Statements","Patient PatNum:")+" "+pat.PatNum+" "+Lans.g("Statements","(possibly incorrect)");
				SecurityLogs.MakeLogEntry(Permissions.StatementPatNumMismatch,stmt.PatNum,logMsg,LogSources.Diagnostic);
			}
			EmailMessage message=new EmailMessage();
			message.PatNum=pat.PatNum;
			message.ToAddress=pat.Email;
			EmailAddress emailAddress=EmailAddresses.GetByClinic(pat.ClinicNum);
			message.FromAddress=EmailAddresses.OverrideSenderAddressClinical(emailAddress,pat.ClinicNum).GetFrom();
			string emailBody;
			if(stmt.EmailSubject!=null && stmt.EmailSubject!="") {
				message.Subject=stmt.EmailSubject;
			}
			else {//Subject was not preset, set a default subject.
				message.Subject=Lans.g("Statements","New Statement Available");
			}
			if(stmt.EmailBody!=null && stmt.EmailBody!="") {
				emailBody=stmt.EmailBody;
			}
			else {//Body was not preset, set a body text.
				emailBody=Lans.g("Statements","Dear")+" [nameFnoPref],\r\n\r\n"
					+Lans.g("Statements","A new account statement is available.")+"\r\n\r\n"
					+Lans.g("Statements","To view your account statement, log on to our portal by following these steps:")+"\r\n\r\n"
					+Lans.g("Statements","1. Visit the following URL in a web browser:")+" "+PrefC.GetString(PrefName.PatientPortalURL)+"\r\n"
					+Lans.g("Statements","2. Enter your credentials to gain access to your account.")+"\r\n"
					+Lans.g("Statements","3. Click the Account icon on the left and select the most recent Statement to view.");
			}
			message.BodyText=Statements.ReplaceVarsForEmail(emailBody,pat,stmt);
			message.MsgType=EmailMessageSource.Statement;
			return message;
		}

		///<summary>Gets a list of unsent StatementNums.</summary>
		public static List<long> GetUnsentStatements(params StatementMode[] arrayStatementMode) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),arrayStatementMode);
			}
			string command=$"SELECT StatementNum FROM statement WHERE IsSent=0 ";
			if(arrayStatementMode.Length!=0) {
				command+=$"AND Mode_ IN({string.Join(",",arrayStatementMode.Select(x => POut.Enum(x)))})";
			}
			return Db.GetListLong(command);
		}
		#endregion

		#region Insert
		///<summary></summary>
		public static long Insert(Statement statement) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				statement.StatementNum=Meth.GetLong(MethodBase.GetCurrentMethod(),statement);
				return statement.StatementNum;
			}
			return Crud.StatementCrud.Insert(statement);
		}

		///<summary></summary>
		public static void InsertMany(List<Statement> listStatements) {
			if(listStatements==null || listStatements.Count==0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				//Unusual middle tier check. The billing tool can send thousands of statements to this function. In that case, middle tier will error
				//saying "Request Too Large". Because this is simple inserting, we will instead break it up into trips of 100 statements.
				for(int i=0;i<listStatements.Count;i+=100) {
					List<Statement> listStatementToSend=listStatements.GetRange(i,Math.Min(100,listStatements.Count-i));
					Meth.GetVoid(MethodBase.GetCurrentMethod(),listStatementToSend);
				}
				return;
			}
			Crud.StatementCrud.InsertMany(listStatements);
		}
		#endregion

		#region Update

		///<summary>Updates the statements with the send status.</summary>
		public static void UpdateSmsSendStatus(List<long> listStmtNumsToUpdate,AutoCommStatus sendStatus) {
			if(listStmtNumsToUpdate.Count==0) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listStmtNumsToUpdate,sendStatus);
				return;
			}
			string command="UPDATE statement SET SmsSendStatus="+POut.Int((int)sendStatus)
				+" WHERE StatementNum IN("+string.Join(",",listStmtNumsToUpdate.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

		///<summary></summary>
		public static void Update(Statement statement) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),statement);
				return;
			}
			Crud.StatementCrud.Update(statement);
		}

		///<summary></summary>
		public static bool Update(Statement statement,Statement statementOld) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),statement,statementOld);
			}
			return Crud.StatementCrud.Update(statement,statementOld);
		}

		public static void MarkSent(long statementNum,DateTime dateSent) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),statementNum,dateSent);
				return;
			}
			string command="UPDATE statement SET DateSent="+POut.Date(dateSent)+", "
				+"IsSent=1 WHERE StatementNum="+POut.Long(statementNum);
			Db.NonQ(command);
		}

		public static void AttachDoc(long statementNum,Document doc,bool doUpdateDoc=true) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),statementNum,doc,doUpdateDoc);
				return;
			}
			if(doUpdateDoc) {
				Documents.Update(doc);
			}
			string command="UPDATE statement SET DocNum="+POut.Long(doc.DocNum)
				+" WHERE StatementNum="+POut.Long(statementNum);
			Db.NonQ(command);
		}
		
		public static void DetachDocFromStatements(long docNum) {
			if(docNum==0) {
				return;//Avoid MiddleTier.
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),docNum);
				return;
			}
			Db.NonQ("UPDATE statement SET DocNum=0 WHERE DocNum="+POut.Long(docNum));
		}

		///<summary>Changes the value of the DateTStamp column to the current time stamp for all statements of a patient</summary>
		public static void ResetTimeStamps(long patNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),patNum);
				return;
			}
			string command="UPDATE statement SET DateTStamp = CURRENT_TIMESTAMP WHERE PatNum ="+POut.Long(patNum);
			Db.NonQ(command);
		}

		#endregion

		#region Delete

		///<summary>Deletes the passed in list of statements. Checks for permission before deleting the stored image in ODI folder. Can force to delete the
		///stored image without the permission check. Will always delete the statement object. Throws UE.</summary>
		public static void DeleteStatements(List<Statement> listStatements,bool forceImageDelete=false) {
			if(listStatements.IsNullOrEmpty()) {
				return;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listStatements,forceImageDelete);
				return;
			}
			foreach(Statement stmt in listStatements) { 
				//Per Nathan the image should not be deleted if the user does not have the Image Delete permission.  The statement can still be deleted.
				if(stmt.DocNum!=0 && (forceImageDelete || Security.IsAuthorized(Permissions.ImageDelete,stmt.DateSent,true))) {
					//deleted the pdf
					Patient pat=Patients.GetPat(stmt.PatNum);
					string patFolder=ImageStore.GetPatientFolder(pat,ImageStore.GetPreferredAtoZpath());
					List<Document> listdocs=new List<Document>();
					listdocs.Add(Documents.GetByNum(stmt.DocNum,true));
					ImageStore.DeleteDocuments(listdocs,patFolder);//May throw if document is in use.
				}
				Delete(stmt);
			}
		}

		///<summary></summary>
		public static void Delete(Statement statement) {
			//No need to check RemotingRole; no call to db.
			Delete(statement.StatementNum);
		}

		///<summary>For deleting a statement when user clicks Cancel.  No need to make entry in DeletedObject table.</summary>
		public static void Delete(long statementNum) {
			//No need to check RemotingRole; no call to db.
			DeleteAll(new List<long> { statementNum });
		}

		public static void DeleteAll(List<long> listStatementNums) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listStatementNums);
				return;
			}
			if(listStatementNums==null || listStatementNums.Count==0) {
				return;
			}
			//Removed all linked dependencies from these statements.
			StmtLinks.DetachAllFromStatements(listStatementNums);
			string command=DbHelper.WhereIn("UPDATE procedurelog SET StatementNum=0 WHERE StatementNum IN ({0})",false,listStatementNums.Select(x => POut.Long(x)).ToList());
			Db.NonQ(command);
			command=DbHelper.WhereIn("UPDATE adjustment SET StatementNum=0 WHERE StatementNum IN({0})",false,listStatementNums.Select(x => POut.Long(x)).ToList());
			Db.NonQ(command);
			command=DbHelper.WhereIn("UPDATE payplancharge SET StatementNum=0 WHERE StatementNum IN({0})",false,listStatementNums.Select(x => POut.Long(x)).ToList());
			Db.NonQ(command);
			command=DbHelper.WhereIn("DELETE FROM statement WHERE StatementNum IN ({0})",false,listStatementNums.Select(x => POut.Long(x)).ToList());
			Db.NonQ(command);
		}
		#endregion

		#region Misc Methods
		///<summary>Queries the database to determine if there are any unsent statements.</summary>
		public static bool UnsentStatementsExist(){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod());
			}
			string command="SELECT COUNT(*) FROM statement WHERE IsSent=0";
			if(Db.GetCount(command)=="0"){
				return false;
			}
			return true;
		}

		///<summary>Queries the database to determine if there are any unsent statements for a particular clinic.</summary>
		public static bool UnsentClinicStatementsExist(long clinicNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),clinicNum);
			}
			if(clinicNum==0) {//All clinics.
				return UnsentStatementsExist();
			}
			string command=@"SELECT COUNT(*) FROM statement 
				LEFT JOIN patient ON statement.PatNum=patient.PatNum
				WHERE statement.IsSent=0
				AND patient.ClinicNum="+clinicNum;
			if(Db.GetCount(command)=="0") {
				return false;
			}
			return true;
		}

		///<summary>Email statements allow variables to be present in the message body and subject, this method replaces those variables with the information from the patient passed in.  Simply pass in the string for the subject or body and the corresponding patient.</summary>
		private static string ReplaceVarsForEmail(string str,Patient pat,Statement stmt) {
			//No need to check RemotingRole; no call to db.
			str=ReplaceVarsForSms(str,pat,stmt);
			//These were not inluded in ReplaceVarsForSms because the last name is considered PHI.
			str=str.Replace("[nameFL]",pat.GetNameFL());
			str=str.Replace("[nameFLnoPref]",pat.GetNameFLnoPref());
			str=str.Replace("[nameFnoPref]",pat.FName);
			return str;
		}

		///<summary>Replaces variable tags with the information from the patient passed in.</summary>
		public static string ReplaceVarsForSms(string smsTemplate,Patient pat,Statement stmt,Clinic clinic=null,
			bool includeInsEst=false) {
			//No need to check RemotingRole; no call to db.
			StringBuilder retVal=new StringBuilder();
			retVal.Append(smsTemplate);
			if(smsTemplate.Contains("[monthlyCardsOnFile]")) {
				StringTools.RegReplace(retVal,"\\[monthlyCardsOnFile]",CreditCards.GetMonthlyCardsOnFile(pat.PatNum));
			}
			StringTools.RegReplace(retVal,"\\[nameF]",pat.GetNameFirst());
			StringTools.RegReplace(retVal,"\\[namePref]",pat.Preferred);
			StringTools.RegReplace(retVal,"\\[PatNum]",pat.PatNum.ToString());
			StringTools.RegReplace(retVal,"\\[currentMonth]",DateTime.Now.ToString("MMMM"));
			if(clinic==null) { 
				clinic=Clinics.GetClinic(pat.ClinicNum)??Clinics.GetPracticeAsClinicZero();
			}
			string officePhone=clinic.Phone;
			if(string.IsNullOrEmpty(officePhone)) {
				officePhone=PrefC.GetString(PrefName.PracticePhone);
			}
			StringTools.RegReplace(retVal,"\\[OfficePhone]",TelephoneNumbers.ReFormat(officePhone));
			string officeName=clinic.Description;
			if(string.IsNullOrEmpty(officeName)) {
				officeName=PrefC.GetString(PrefName.PracticeTitle);
			}
			StringTools.RegReplace(retVal,"\\[OfficeName]",officeName);
			if(smsTemplate.ToLower().Contains("[statementurl]") || smsTemplate.ToLower().Contains("[statementshorturl]")) {
				AssignURLsIfNecessary(stmt,pat);
			}
			StringTools.RegReplace(retVal,"\\[StatementURL]",stmt.StatementURL);
			StringTools.RegReplace(retVal,"\\[StatementShortURL]",stmt.StatementShortURL);
			double balance=stmt.BalTotal;
			if(includeInsEst) {
				balance=Math.Max(balance-stmt.InsEst,0);
			}
			if(balance<=0) {
				retVal.Replace("[StatementBalance]",balance.ToString("c"));
			}
			else {
				StringTools.RegReplace(retVal,"\\[StatementBalance]",balance.ToString("c"));
			}
			return retVal.ToString();
		}

		///<summary>Allows an email receipt to be sent to a patient through the portal.  Throws exceptions in the case where the email from address is not valid. Sends a seperate email if the document was unable to be created.</summary>
		public static void EmailStatementPatientPortal(Statement stmt,string toAddress,EmailAddress fromAddress,Patient patCur) {
			//Create the Statement Object
			Insert(stmt);
			//Create the .pdf file
			SheetDef sheetDef=SheetUtil.GetStatementSheetDef(stmt);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,stmt.PatNum,stmt.HidePayment);
			DataSet dataSet=AccountModules.GetAccount(stmt.PatNum,stmt,doShowHiddenPaySplits:stmt.IsReceipt);
			sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=stmt });
			SheetFiller.FillFields(sheet,dataSet,stmt);
			SheetUtil.CalculateHeights(sheet,dataSet,stmt);
			string tempPath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),stmt.PatNum.ToString()+".pdf");
			SheetDrawingJob sheetDrawingJob=new SheetDrawingJob();
			sheetDrawingJob.CreatePdf(sheet,tempPath,stmt,dataSet,null);
			long category=0;
			//Find the image category for statements
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			for(int i=0;i<listDefs.Count;i++) {
				if(Regex.IsMatch(listDefs[i].ItemValue,@"S")) {
					category=listDefs[i].DefNum;
					break;
				}
			}
			if(category==0) {
				category=listDefs[0].DefNum;//put it in the first category.
			}
			//create doc--------------------------------------------------------------------------------------
			Document docc=null;
			string errorMessage="";
			try {
				docc=ImageStore.Import(tempPath,category,Patients.GetPat(stmt.PatNum));
			}
			catch {
				errorMessage="Thank you for your recent payment. An error occurred when creating your receipt, please contact your provider to request a copy.";
				EmailMessage emailMessageError=GetEmailMessageForStatement(stmt,Patients.GetPat(stmt.PatNum));
				emailMessageError.ToAddress=toAddress;
				emailMessageError.MsgType=EmailMessageSource.PaymentReceipt;
				emailMessageError.SentOrReceived=EmailSentOrReceived.Sent;
				emailMessageError.MsgDateTime=DateTime_.Now;
				emailMessageError.BodyText=errorMessage;
				if(fromAddress==null || !EmailAddresses.IsValidEmail(fromAddress.SenderAddress)) {//Check to make sure that our "from" email address is valid.
					throw new ODException("Thank you for your recent payment. An error occurred when attempting to email your receipt," 
						+" please contact your provider.");
				}
				fromAddress=EmailAddresses.OverrideSenderAddressClinical(fromAddress,patCur.ClinicNum); //Use clinic's Email Sender Address Override, if present
				try {
					EmailMessages.SendEmail(emailMessageError,fromAddress);
				}
				catch (Exception ex) {
					ex.DoNothing();
					throw new ODException(errorMessage);
				}
				return;
			}
			Patient guarantor=Patients.GetPat(stmt.PatNum);
			string guarFolder=ImageStore.GetPatientFolder(guarantor,ImageStore.GetPreferredAtoZpath());
			string fileName="";
			docc.ImgType=ImageType.Document;
			docc.Description=Lans.g("Statement","Receipt");
			docc.DateCreated=stmt.DateSent;
			stmt.DocNum=docc.DocNum;//this signals the calling class that the pdf was created successfully.
			//Attach Doc to the statement in the DB
			AttachDoc(stmt.StatementNum,docc);
			//Doc fileName and Copy to emailAttach Folder
			string attachPath=EmailAttaches.GetAttachPath();
			fileName=DateTime.Now.ToString("yyyyMMdd")+DateTime.Now.TimeOfDay.Ticks.ToString()+ODRandom.Next(1000).ToString()+".pdf";
			string filePathAndName=FileIO.FileAtoZ.CombinePaths(attachPath,fileName);
			FileIO.FileAtoZ.Copy(ImageStore.GetFilePath(Documents.GetByNum(stmt.DocNum),guarFolder),filePathAndName,FileAtoZSourceDestination.AtoZToAtoZ);
			if(fromAddress==null || !EmailAddresses.IsValidEmail(fromAddress.SenderAddress)) {//Check to make sure that our "from" email address is valid.
				throw new ODException("Thank you for your recent payment. An error occurred when attempting to email your receipt," 
					+" please refresh your page to view your statement.");
			}
			//Create email and attachment objects
			EmailMessage emailMessage=GetEmailMessageForStatement(stmt,guarantor);
			EmailAttach emailAttachment=new EmailAttach();
			emailAttachment.DisplayedFileName="Statement.pdf";
			emailAttachment.ActualFileName=fileName;
			emailMessage.Attachments.Add(emailAttachment);
			emailMessage.ToAddress=toAddress;
			emailMessage.MsgType=EmailMessageSource.PaymentReceipt;
			emailMessage.SentOrReceived=EmailSentOrReceived.Sent;
			emailMessage.MsgDateTime=DateTime_.Now;
			try {
				EmailMessages.SendEmail(emailMessage,fromAddress);
			}
			catch(Exception ex) {
				ex.DoNothing();
				throw new ODException("Thank you for your recent payment. An error occurred when attempting to send an email receipt," 
					+" please refresh your page to view your statement.");
			}
		}

		public static Statement CreateReceiptStatement(Patient patCur, StatementMode mode) {
			Statement stmt=new Statement();
			stmt.PatNum=patCur.PatNum;
			stmt.DateSent=DateTime.Today;
			stmt.IsSent=true;
			stmt.Mode_=StatementMode.Email;
			stmt.HidePayment=true;
			stmt.Intermingled=PrefC.GetBool(PrefName.IntermingleFamilyDefault);
			stmt.SinglePatient=!stmt.Intermingled;
			stmt.IsReceipt=true;
			stmt.StatementType=StmtType.NotSet;
			stmt.DateRangeFrom=DateTime.Today;
			stmt.DateRangeTo=DateTime.Today;
			stmt.Note="";
			stmt.NoteBold="";
			Patient guarantor=null;
			if(patCur!=null) {
				guarantor=Patients.GetPat(patCur.Guarantor);
			}
			if(guarantor!=null) {
				stmt.IsBalValid=true;
				stmt.BalTotal=guarantor.BalTotal;
				stmt.InsEst=guarantor.InsEst;
			}
			return stmt;
		}

		///<summary>If the statement does not have a short guid or URL, a call will be made to HQ to assign it one. The statement will be updated
		///to the database.</summary>
		public static void AssignURLsIfNecessary(Statement stmt,Patient pat) {
			if(string.IsNullOrEmpty(stmt.ShortGUID) || string.IsNullOrEmpty(stmt.StatementURL)) {
				List<WebServiceMainHQProxy.ShortGuidResult> listShortGuidUrls=WebServiceMainHQProxy.GetShortGUIDs(1,1,pat.ClinicNum,
					eServiceCode.PatientPortalViewStatement);
				Statement stmtOld=stmt.Copy();
				stmt.ShortGUID=listShortGuidUrls[0].ShortGuid;
				stmt.StatementURL=listShortGuidUrls[0].MediumURL;
				stmt.StatementShortURL=listShortGuidUrls[0].ShortURL;
				Statements.Update(stmt,stmtOld);
			}
		}

		public static Statement CreateLimitedStatement(List<long> listPatNumsSelected,long patNum,List<long> listPayClaimNums,List<long> listAdjustments,
			List<long> listPayNums,List<long> listProcedures,List<long> listPayPlanChargeNums)
		{
			Statement stmt =new Statement();
			if(listPatNumsSelected.Count==1) {
				stmt.PatNum=listPatNumsSelected[0];
			}
			else {
				stmt.PatNum=patNum;
			}
			stmt.DateSent=DateTime.Today;
			stmt.IsSent=false;
			stmt.Mode_=StatementMode.InPerson;
			stmt.HidePayment=false;
			stmt.SinglePatient=listPatNumsSelected.Count==1;//SinglePatient determined by the selected transactions
			stmt.Intermingled=listPatNumsSelected.Count>1 && PrefC.GetBool(PrefName.IntermingleFamilyDefault);
			stmt.IsReceipt=false;
			stmt.IsInvoice=false;
			stmt.StatementType=StmtType.LimitedStatement;
			stmt.DateRangeFrom=DateTime.MinValue;
			stmt.DateRangeTo=DateTime.Today;
			stmt.Note="";
			stmt.NoteBold="";
			stmt.IsBalValid=true;
			stmt.BalTotal=0;
			stmt.InsEst=0;
			Statements.Insert(stmt);//we need stmt.StatementNum for attaching procs, adjustments, paysplits, and payment plan charges to the statement
			foreach(long adjNum in listAdjustments) {
				StmtLinks.Insert(new StmtLink() { FKey=adjNum,StatementNum=stmt.StatementNum,StmtLinkType=StmtLinkTypes.Adj});
			}
			foreach(long payNum in listPayNums) {
				Payment payment=Payments.GetPayment(payNum);
				PaySplits.GetForPayment(payNum)
						.ForEach(x => StmtLinks.Insert(new StmtLink() { FKey=x.SplitNum,StatementNum=stmt.StatementNum,StmtLinkType=StmtLinkTypes.PaySplit }));
			}
			foreach(long procNum in listProcedures) {
				StmtLinks.Insert(new StmtLink() { FKey=procNum,StatementNum=stmt.StatementNum,StmtLinkType=StmtLinkTypes.Proc});
			}
			foreach(long claimNum in listPayClaimNums) {
				StmtLinks.Insert(new StmtLink() {FKey=claimNum,StatementNum=stmt.StatementNum,StmtLinkType=StmtLinkTypes.ClaimPay});
			}
			for(int i=0;i<listPayPlanChargeNums.Count;i++) {
				StmtLinks.Insert(new StmtLink() { FKey=listPayPlanChargeNums[i],StatementNum=stmt.StatementNum,StmtLinkType=StmtLinkTypes.PayPlanCharge });
			}
			//set statement lists to null in order to force refresh the lists now that we've inserted all of the StmtAttaches
			stmt.ListAdjNums=null;
			stmt.ListPaySplitNums=null;
			stmt.ListProcNums=null;
			stmt.ListInsPayClaimNums=null;
			stmt.ListPayPlanChargeNums=null;
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				//Currently when using MiddleTier null lists inside an object like above causes the deserialized statement to incorrectly set the null lists
				//to empty lists.
				//In the case of a Statement, the public property associated to list in question does not run queries to populate the list as expected because
				//it checks for a null list, but instead sees an empty list.
				//We may fix the underlying MiddleTier bug later, but this is an immediate patch to address this symptom.
				stmt.ListAdjNums=StmtLinks.GetForStatementAndType(stmt.StatementNum,StmtLinkTypes.Adj);
				stmt.ListPaySplitNums=StmtLinks.GetForStatementAndType(stmt.StatementNum,StmtLinkTypes.PaySplit);
				if(stmt.IsInvoice) {
					stmt.ListProcNums=Procedures.GetForInvoice(stmt.StatementNum);
				}
				else {
					stmt.ListProcNums=StmtLinks.GetForStatementAndType(stmt.StatementNum,StmtLinkTypes.Proc);
				}
				stmt.ListInsPayClaimNums=StmtLinks.GetForStatementAndType(stmt.StatementNum,StmtLinkTypes.ClaimPay);
				stmt.ListPayPlanChargeNums=StmtLinks.GetForStatementAndType(stmt.StatementNum,StmtLinkTypes.PayPlanCharge);
			}
			return stmt;
		}

		///<summary>Creates statement prods for a statement based off of the dataSet passed in, then syncs this list with the existing statementprods for the statement in the DB.</summary>
		public static void SyncStatementProdsForStatement(DataSet dataSet,long statementNum,long docNum) {
			//No need to check RemotingRole; no call to db.
			if(docNum==0) {
				return;
			}
			StatementProds.SyncForStatement(dataSet,statementNum,docNum);
		}

		///<summary>Pass in a dictionary for which the keys are StatementNums and the values are statement DataSets. Creates statement prods for the statements based off of their dataSets, then syncs these statementprods with the existing statementprods for the statements in the DB.</summary>
		public static void SyncStatementProdsForMultipleStatements(Dictionary<long,StatementData> dictStmtDataSets) {
			//No need to check RemotingRole; no call to db.
			StatementProds.SyncForMultipleStatements(dictStmtDataSets);
		}

		///<summary>Sets the installment plans field on each of the statements passed in.</summary>
		public static void AddInstallmentPlansToStatements(List<Statement> listStatements,Dictionary<long,Family> dictFamilies=null) {
			//No need to check RemotingRole; no call to db.
			if(listStatements.IsNullOrEmpty()) {
				return;
			}
			if(dictFamilies==null) {
				dictFamilies=GetFamiliesForStatements(listStatements);
			}
			Dictionary<long,List<InstallmentPlan>> dictionarySuperFamInstallmentPlans=InstallmentPlans.GetForSuperFams(
				listStatements.Where(x => x.SuperFamily > 0)
					.Select(x => dictFamilies[x.PatNum].Guarantor.SuperFamily).ToList());
			Dictionary<long,InstallmentPlan> dictionaryFamInstallmentPlans=InstallmentPlans.GetForFams(
				listStatements.Where(x => x.SuperFamily==0)
					.Select(x => dictFamilies[x.PatNum].Guarantor.PatNum).ToList());
			for(int i=0;i<listStatements.Count;++i) {
				if(listStatements[i].SuperFamily > 0) {
					if(!dictionarySuperFamInstallmentPlans.TryGetValue(dictFamilies[listStatements[i].PatNum].Guarantor.SuperFamily,out listStatements[i].ListInstallmentPlans)) {
						listStatements[i].ListInstallmentPlans=new List<InstallmentPlan>();
					}
				}
				else if(dictionaryFamInstallmentPlans.ContainsKey(dictFamilies[listStatements[i].PatNum].Guarantor.PatNum)) {
					listStatements[i].ListInstallmentPlans=new List<InstallmentPlan> { dictionaryFamInstallmentPlans[dictFamilies[listStatements[i].PatNum].Guarantor.PatNum] };
				}
				else {
					listStatements[i].ListInstallmentPlans=new List<InstallmentPlan>();
				}
			}
		}

		///<summary>Returns a dictionary of Key-PatNum, Value-Family for the statements passed in.</summary>
		public static Dictionary<long,Family> GetFamiliesForStatements(List<Statement> listStatements) {
			//No need to check RemotingRole; no call to db.
			if(listStatements.IsNullOrEmpty()) {
				return new Dictionary<long, Family>();
			}
			return Patients.GetFamilies(listStatements.Select(x => x.PatNum).ToList())
				.SelectMany(fam => fam.ListPats.Select(y => new { y.PatNum,fam }))
				.Distinct()
				.ToDictionary(x => x.PatNum,x => x.fam);
		}

		///<summary>Returns a dictionary of batches of statements.  The key is the batch num which is 1 based (helpful when displaying to the user).</summary>
		public static Dictionary<int,List<Statement>> GetBatchesForStatements(List<Statement> listStatements,out List<long> listClinicNums,
			Dictionary<long,Family> dictFamilies=null) 
		{
			//No need to check RemotingRole; no call to db.
			listClinicNums=new List<long>();
			Dictionary<int,List<Statement>> dictStatementsForSend=new Dictionary<int,List<Statement>>();
			if(listStatements.IsNullOrEmpty()) {
				return dictStatementsForSend;
			}
			if(dictFamilies==null) {
				dictFamilies=GetFamiliesForStatements(listStatements);
			}
			//A dictionary of batches of statements.  The key is the batch num which is 1 based (helpful when displaying to the user).
			//If clinics are on, batch by clinics. Otherwise, batch by BillingElectBatchMax.
			if(PrefC.HasClinicsEnabled) {
				//Make each clinic one batch
				for(int i=0;i<listStatements.Count;i++) {
					//Find the clinicnum.
					long clinicNum=dictFamilies[listStatements[i].PatNum].Guarantor.ClinicNum;
					if(!listClinicNums.Contains(clinicNum)) {
						listClinicNums.Add(clinicNum);
					}
					int index=listClinicNums.IndexOf(clinicNum);
					if(!dictStatementsForSend.TryGetValue(index,out List<Statement> listStatementsBatch)) {
						dictStatementsForSend.Add(index,new List<Statement>());
					}
					dictStatementsForSend[index].Add(listStatements[i]);
				}
			}
			else {
				int maxStmtsPerBatch=PrefC.GetInt(PrefName.BillingElectBatchMax);
				if(maxStmtsPerBatch==0 || PrefC.GetString(PrefName.BillingUseElectronic)=="2" || PrefC.GetString(PrefName.BillingUseElectronic)=="4") {//Max is disabled or Output to File billing option or using EDS.
					maxStmtsPerBatch=listStatements.Count;//Make the batch size equal to the list of statements so that we send them all at once.
				}
				int batchCount=-1; //-1 so we can start the dictionary at 0
				for(int i=0;i<listStatements.Count;i++) {
					if(i % maxStmtsPerBatch==0) {
						batchCount++;
						dictStatementsForSend.Add(batchCount,new List<Statement>());
					}
					dictStatementsForSend[batchCount].Add(listStatements[i]);
				}
			}
			return dictStatementsForSend;
		}


		///<summary>The filePath is the full path to the output file if the clinics feature is disabled (for a single location practice).</summary>
		public static string GetEbillFilePathForClinic(string filePath,long clinicNum) {
			//No need to check RemotingRole; no call to db.
			if(!PrefC.HasClinicsEnabled) {
				return filePath;
			}
			string clinicAbbr;
			//Check for zero clinic
			if(clinicNum==0) {
				clinicAbbr=Lans.g("SendEBills","Unassigned");
			}
			else {
				clinicAbbr=Clinics.GetClinic(clinicNum).Abbr;//Abbr is required by our interface, so no need to check if blank.
			}
			string fileName=Path.GetFileNameWithoutExtension(filePath)+'-'+clinicAbbr+Path.GetExtension(filePath);
			return ODFileUtils.CombinePaths(Path.GetDirectoryName(filePath),fileName);
		}

		public static void HandleSmsSent(List<SmsToMobile> listSmsToMobiles,List<Tuple<string,long>> listStmtNumsToUpdate,
			Dictionary<long,string> dictionaryPatnumsSkippedMisc)
		{
			//No need to check RemotingRole; no call to db.
			List<long> listSuccessStatementNums=listSmsToMobiles
				//SmsToMobile that were queued successfully
				.Where(x => x.SmsStatus!=SmsDeliveryStatus.FailNoCharge)
				//That correspond to a statement
				.Select(x => listStmtNumsToUpdate.FirstOrDefault(y => y.Item1==x.GuidMessage)?.Item2??0)
				.Where(x => x!=0)
				.ToList();
			UpdateSmsSendStatus(listSuccessStatementNums,AutoCommStatus.SendSuccessful);
			List<long> listFailureStatementNums=listSmsToMobiles
				//SmsToMobile that were not queued successfully
				.Where(x => x.SmsStatus==SmsDeliveryStatus.FailNoCharge)
				//That correspond to a statement
				.Select(x => listStmtNumsToUpdate.FirstOrDefault(y => y.Item1==x.GuidMessage)?.Item2??0)
				.Where(x => x!=0)
				.ToList();
			UpdateSmsSendStatus(listFailureStatementNums,AutoCommStatus.SendFailed);
			//listDictPatnumsSkipped[2] is for displaying miscellaneous (in this case, sms related) errors to user.
			List<SmsToMobile> listSmsToMobileFails = listSmsToMobiles.Where(x => x.SmsStatus==SmsDeliveryStatus.FailNoCharge).ToList();
			for(int i=0;i<listSmsToMobileFails.Count;++i) {
				if(!dictionaryPatnumsSkippedMisc.ContainsKey(listSmsToMobileFails[i].PatNum)) {
					dictionaryPatnumsSkippedMisc[listSmsToMobileFails[i].PatNum]="";
				}
				dictionaryPatnumsSkippedMisc[listSmsToMobileFails[i].PatNum]+=Lans.g("Statements","Error Sending text messages")+": "+listSmsToMobileFails[i].CustErrorText;
			}
		}

		///<summary>Returns the mode for the statement.</summary>
		public static StatementMode GetStatementMode(PatAging patAging) {
			//No need to check RemotingRole; no call to db.
			StatementMode statementMode;
			if(ListTools.In(PrefC.GetInt(PrefName.BillingUseElectronic),1,2,3,4)) {
				statementMode=StatementMode.Electronic;
			}
			else {
				statementMode=StatementMode.Mail;
			}
			Def billingType=Defs.GetDef(DefCat.BillingTypes,patAging.BillingType);
			if(billingType != null && billingType.ItemValue=="E") {
				statementMode=StatementMode.Email;
			}
			return statementMode;
		}

		///<summary>Returns true if the patient statement mode has an option to send by SMS.</summary>
		public static bool DoSendSms(PatAging patAge,Dictionary<long,PatAgingData> dictPatAgingData,List<StatementMode> listStatementModes) {
			//No need to check RemotingRole; no call to db.
			PatAgingData patAgingData;
			dictPatAgingData.TryGetValue(patAge.PatNum,out patAgingData);
			if(ListTools.In(GetStatementMode(patAge),listStatementModes)
				&& patAgingData!=null
				&& patAgingData.PatComm!=null
				&& patAgingData.PatComm.IsSmsAnOption) {
				return true;
			}
			return false;
		}

		///<summary>Creates a new pdf, attaches it to a new doc, and attaches that to the statement.  If it cannot create a pdf, for example if no AtoZ 
		///folders, then it will simply result in a docnum of zero, so no attached doc. Only used for batch statment printing. Returns the path of the
		///temp file where the pdf is saved.Temp file should be deleted manually.  Will return an empty string when unable to create the file.</summary>
		public static string CreateStatementPdfSheets(Statement stmt,Patient pat,Family fam,DataSet dataSet) {
			//No need to check RemotingRole; no call to db.
			Statement StmtCur=stmt;
			SheetDef sheetDef=SheetUtil.GetStatementSheetDef(StmtCur);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,StmtCur.PatNum,StmtCur.HidePayment);
			sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=StmtCur });
			SheetFiller.FillFields(sheet,dataSet,StmtCur,pat: pat,fam: fam);
			SheetUtil.CalculateHeights(sheet,dataSet,StmtCur,pat: pat,patGuar: fam.Guarantor);
			string tempPath=CodeBase.ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),StmtCur.PatNum.ToString()+".pdf");
			SheetPrinting.CreatePdf(sheet,tempPath,StmtCur,dataSet,null,pat: pat,patGuar: fam.Guarantor);
			List<Def> listImageCatDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			long category=0;
			for(int i = 0;i<listImageCatDefs.Count;i++) {
				if(Regex.IsMatch(listImageCatDefs[i].ItemValue,@"S")) {
					category=listImageCatDefs[i].DefNum;
					break;
				}
			}
			if(category==0) {
				category=listImageCatDefs[0].DefNum;//put it in the first category.
			}
			//create doc--------------------------------------------------------------------------------------
			OpenDentBusiness.Document docc=null;
			try {
				docc=ImageStore.Import(tempPath,category,pat);
			}
			catch {
				return "";//Error saving the document
			}
			docc.ImgType=ImageType.Document;
			if(StmtCur.IsInvoice) {
				docc.Description=Lans.g(nameof(Statements),"Invoice");
			}
			else {
				if(StmtCur.IsReceipt==true) {
					docc.Description=Lans.g(nameof(Statements),"Receipt");
				}
				else {
					docc.Description=Lans.g(nameof(Statements),"Statement");
				}
			}
			docc.DateCreated=StmtCur.DateSent;
			StmtCur.DocNum=docc.DocNum;//this signals the calling class that the pdf was created successfully.
			Statements.AttachDoc(StmtCur.StatementNum,docc);
			return tempPath;
		}

		///<summary></summary>
		public static string SaveStatementAsCSV(Statement statement) {
			long statementCategory = Defs.GetImageCat(ImageCategorySpecial.S);
			string prependCategoryNum = "";
			if(statementCategory > 0) {
				//Files that start with "_###_" will automatically have Document entries created for them when the Imaging module loads.
				prependCategoryNum="_" + statementCategory + "_";
			}
			Patient patient=Patients.GetPat(statement.PatNum);
			string patFolder=ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
			string fileName=prependCategoryNum+patient.LName+patient.FName+statement.DocNum.ToString()+".csv";
			return WriteStatementToCSV(statement,fileName,patFolder);
    }

		///<summary></summary>
		private static string WriteStatementToCSV(Statement statement,string fileName,string filePath) {
			if(statement==null) {
				return "";
			}
			string path=FileAtoZ.CombinePaths(filePath,fileName); 
			DataSet dataSet=AccountModules.GetStatementDataSet(statement,true,doIncludePatLName:PrefC.IsODHQ);
			DataTable dataTable=SheetFramework.SheetDataTableUtil.GetTable_StatementMain(dataSet,statement);
			StringBuilder exportCSV=new StringBuilder();
			long stateNum=statement.StatementNum;
			exportCSV.AppendLine("Invoice/Statement Number," 
				+"Date Created," 
				+"Procedure Total," 
				+"Ins Estimate," 
				+"Total Amount,");
			exportCSV.AppendLine($"\"{stateNum}\"," 
				+$"\"{statement.DateSent.ToShortDateString()}\"," 
				+$"\"{statement.BalTotal}\"," 
				+$"\"{statement.InsEst}\"," 
				+$"\"{statement.BalTotal-statement.InsEst}\",");
			exportCSV.AppendLine("");
			exportCSV.AppendLine("");
			exportCSV.AppendLine("Date,"
				+"Patient Number,"
				+"Patient Name,"
				+"Code,"
				+"Description,"
				+"Charges,"
				+"Credits,"
				+"Balance");
			for(int i=0;i<dataTable.Rows.Count;i++) {
				long patNum=PIn.Long(dataTable.Rows[i]["PatNum"].ToString());
				exportCSV.AppendLine($"\"{dataTable.Rows[i]["date"]}\"," 
					+$"\"{patNum}\"," 
					+$"\"{Patients.GetNameFL(patNum)}\"," 
					+$"\"{dataTable.Rows[i]["ProcCode"]}\"," 
					+$"\"{dataTable.Rows[i]["description"]}\"," 
					+$"\"{dataTable.Rows[i]["charges"].ToString()}\"," 
					+$"\"{dataTable.Rows[i]["credits"].ToString()}\"," 
					+$"\"{dataTable.Rows[i]["balance"].ToString()}\",");
			}
			File.WriteAllText(path,exportCSV.ToString());
			return path;
		}
		#endregion
	}

	///<summary>Holds all of the statement and StatementProd data relevant to syncing StatementProds and late charges.</summary>
	[Serializable]
	public class StatementData {
		///<summary>Date the statement was sent.</summary>
		public DateTime DateSent;
		///<summary>PatNum of the guarantor of the family that the statement is for or the PatNum of the SuperFamily head if the statement is a SuperFamily statement.</summary>
		public long GuarantorPatNum;
		///<summary>Guarantor's primary provider's ProvNum or the SuperFamily head's primary provider's ProvNum if the statement is a SuperFamily statement.</summary>
		public long GuarantorPriProvNum;
		///<summary>True if the statement is a superfamily statement.</summary>
		public bool IsSuperFamilyStatement;
		///<summary>Holds the PatNums of all guarantors in super family if the statement is a super family statement, otherwise it just holds the family's guarantor.</summary>
		public List<long> ListGuarantorPatNums=new List<long>();
		///<summary>The StatementProds associated to the statement.</summary>
		public List<StatementProd> ListStatementProds=new List<StatementProd>();
		///<summary>The DocNum of the document associated to the statement.</summary>
		public long DocNum;
		///<summary>Specific tables and columns from the DataSet used to create the statement for inserting or syncing StatementProds.</summary>
		[XmlIgnore]
		public DataSet StmtDataSet;

		///<summary>For serialization purposes.</summary>
		public StatementData() {
		}

		///<summary>This constructur only sets the DocNum and StmtDataSet and is only used when building a collection of StatementData sets for the purpose of syncing StatementProds for multiple statements.</summary>
		public StatementData(DataSet stmtDataSet,long docNum) {
			StmtDataSet=new DataSet();
			for(int i=0;i<stmtDataSet.Tables.Count;i++) {
				//Each family member will have their own account table, so only consider tables that start with 'account'.
				if(!stmtDataSet.Tables[i].TableName.StartsWith("account")) {
					continue;
				}
				DataTable tableAccount=stmtDataSet.Tables[i].Copy();
				//Remove columns that are not used when inserting or syncing StatementProds to save memory.
				for(int j=tableAccount.Columns.Count-1;j>=0;j--) {
					if(!ListTools.In(tableAccount.Columns[j].ColumnName,"AdjNum","creditsDouble","PayPlanChargeNum","ProcNum","procsOnObj")) {
						tableAccount.Columns.RemoveAt(j);
					}
				}
				StmtDataSet.Tables.Add(tableAccount);
			}
			DocNum=docNum;
		}

		///<summary>Used when creating late charges. Gets a list of StatementData objects based on the filters used in ForLateCharges. Should only be run after aging has been run.</summary>
		public static List<StatementData> GetListStatementDataForLateCharges(bool isExcludeAccountNoTil,bool isExcludeExistingLateCharges
			,decimal excludeBalancesLessThan,DateTime dateRangeStart,DateTime dateRangeEnd,List<long> listBillingTypes)
		{
			if(listBillingTypes.IsNullOrEmpty()) {
				return new List<StatementData>();
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				return Meth.GetObject<List<StatementData>>(MethodBase.GetCurrentMethod(),isExcludeAccountNoTil,isExcludeExistingLateCharges
					,excludeBalancesLessThan,dateRangeStart,dateRangeEnd,listBillingTypes);
			}
			string command=$@"
				SELECT statementprod.*,statement.SuperFamily,statement.DateSent,guar.PatNum,guar.PriProv
				FROM statementprod
				INNER JOIN statement
					ON statement.StatementNum=statementprod.StatementNum
					AND statement.DocNum=statementprod.DocNum
					AND statementprod.LateChargeAdjNum=0
					AND statement.IsInvoice=0
					AND statement.IsReceipt=0
					AND statement.IsSent=1
					AND statement.Mode_!={POut.Int((int)StatementMode.InPerson)}
					AND statement.DateSent BETWEEN {POut.Date(dateRangeStart)} AND {POut.Date(dateRangeEnd)}
				INNER JOIN patient
					ON statement.PatNum=patient.PatNum
				INNER JOIN patient guar
					ON patient.Guarantor=guar.PatNum 
					AND guar.BillingType IN({string.Join(",",listBillingTypes.Select(x => POut.Long(x)).ToList())}) ";
			if(isExcludeAccountNoTil) {
				command+="AND guar.HasSignedTil=1 ";
			}
			command+=@"
				LEFT JOIN document
					ON statement.DocNum=document.DocNum
				LEFT JOIN (
					SELECT patient.SuperFamily,
						SUM(CASE WHEN (patient.BalTotal-patient.InsEst)>0 THEN (patient.BalTotal-patient.InsEst) ELSE 0 END) AS 'SuperFamBal'
					FROM patient
					WHERE patient.PatNum=patient.Guarantor
					AND patient.SuperFamily!=0
					GROUP BY patient.SuperFamily
				) AS superfam
				ON superfam.SuperFamily=patient.PatNum ";
			if(isExcludeExistingLateCharges) {
				command+=@$"
					LEFT JOIN statementprod sp 
						ON statementprod.FKey=sp.LateChargeAdjNum 
						AND statementprod.ProdType={(int)ProductionType.Adjustment} ";
			}
			//Filter out statements with missing documents, and superfamily statements or regular statements under the balance filter
			command+=@$"WHERE (CASE WHEN statementprod.DocNum!=0 AND document.DocNum IS NULL THEN 0 ELSE 1 END)
				AND (CASE WHEN statement.SuperFamily=0 THEN guar.BalTotal-guar.InsEst ELSE COALESCE(superfam.SuperFamBal,0) END)
				>={POut.Decimal(excludeBalancesLessThan)} ";
			if(isExcludeExistingLateCharges) {
				command+="AND sp.LateChargeAdjNum IS NULL ";
			}
			command+="ORDER BY statement.DateSent,statement.StatementNum";
			DataTable table=Db.GetTable(command);
			Dictionary<long,StatementData> dictStatementData=new Dictionary<long,StatementData>();
			//If we end up wanting to prevent late charges from being created twice for a single statement that has had multiple documents made for it,
			//we can remove the dictionary entry for the statement in this loop if we come across a StatementProd that has a non-zero LateChargeAdjNum.
			//We would then have to remove the "ON statementprod.LateChargeAdjNum=0 clause from the query above.
			for(int i=0;i<table.Rows.Count;i++) {
				DataRow rowCur=table.Rows[i];
				long statementNum=PIn.Long(rowCur["StatementNum"].ToString());
				//If a dictionary entry for the statement does not yet exist, create one.
				if(!dictStatementData.ContainsKey(statementNum)) {
					StatementData statementData=new StatementData();
					statementData.DateSent=PIn.Date(rowCur["DateSent"].ToString());
					statementData.GuarantorPatNum=PIn.Long(rowCur["PatNum"].ToString());
					statementData.GuarantorPriProvNum=PIn.Long(rowCur["PriProv"].ToString());
					statementData.IsSuperFamilyStatement=PIn.Long(rowCur["SuperFamily"].ToString())!=0;
					statementData.ListGuarantorPatNums.Add(statementData.GuarantorPatNum);
					if(statementData.IsSuperFamilyStatement) {
						statementData.ListGuarantorPatNums=Patients.GetSuperFamilyGuarantors(statementData.GuarantorPatNum).Select(x => x.PatNum).ToList();
					}
					dictStatementData.Add(statementNum,statementData);
				}
				//Then, add the statement prod to the list in this statement's dictionary entry.
				StatementProd statementProd=new StatementProd();
				statementProd.StatementProdNum=PIn.Long(rowCur["StatementProdNum"].ToString());
				statementProd.StatementNum=statementNum;
				statementProd.FKey=PIn.Long(rowCur["FKey"].ToString());
				statementProd.ProdType=(ProductionType)PIn.Int(rowCur["ProdType"].ToString());
				statementProd.LateChargeAdjNum=PIn.Long(rowCur["LateChargeAdjNum"].ToString());
				dictStatementData[statementNum].ListStatementProds.Add(statementProd);
			}
			List<StatementData> listStatementData=dictStatementData.Values.ToList();
			return listStatementData;
		}

	}

	public struct EbillStatement {

		public Statement statement;
		public Family family;

	}

}










