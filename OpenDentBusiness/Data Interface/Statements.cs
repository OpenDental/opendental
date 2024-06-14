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
using PdfSharp.Pdf;
using OpenDentBusiness.Crud;

namespace OpenDentBusiness {

	///<summary></summary>
	public class Statements{
		#region Get Methods
		///<Summary>Gets one statement from the database.</Summary>
		public static Statement GetStatement(long statementNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Statement>(MethodBase.GetCurrentMethod(),statementNum);
			}
			return Crud.StatementCrud.SelectOne(statementNum);
		}

		///<summary>Gets a list of statements based on the passed in primary keys. If clinics are enabled and the preference 
		///PrintStatementsAlphabetically is set, the statements will be sorted by patients last name then first name.
		///Otherwise statements will be ordered in the order of the listStatementNums passed in.</summary>
		public static List<Statement> GetStatements(List<long> listStatementNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Statement>>(MethodBase.GetCurrentMethod(),listStatementNums);
			}
			if(listStatementNums==null || listStatementNums.Count < 1) {
				return new List<Statement>();
			}
			//Sort by patient num pref only valid when clinics turned on.
			bool sortByPatientName=(PrefC.HasClinicsEnabled && PrefC.GetBool(PrefName.PrintStatementsAlphabetically));
			string command="SELECT * FROM statement ";
			if(sortByPatientName) {
				command+="INNER JOIN patient ON patient.PatNum = statement.PatNum ";
			}
			command+="WHERE StatementNum IN ("+string.Join(",",listStatementNums)+") ";
			if(sortByPatientName) {
				command+="ORDER BY patient.LName,patient.FName";
			}
			List<Statement> listStatements=StatementCrud.SelectMany(command);
			//If clinics are enabled, the practice has the option to order statements alphabetically.
			//For other cases, we are going to order the statements the way they are displayed in the grid.
			if(!sortByPatientName) {
				listStatements=listStatements.OrderBy(x => listStatementNums.IndexOf(x.StatementNum)).ToList();
			}
			return listStatements;
		}

		///<summary>For orderBy, use 0 for BillingType and 1 for PatientName.</summary>
		public static DataTable GetBilling(bool isSent,int orderBy,DateTime dateFrom,DateTime dateTo,List<long> listClinicNums){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),isSent,orderBy,dateFrom,dateTo,listClinicNums);
			}
			DataTable table=new DataTable();
			DataRow dataRow;
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
			if(listClinicNums.Count>0) {
				command+="AND patient.ClinicNum IN ("+string.Join(",",listClinicNums)+") ";
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
			DataTable tableRaw=Db.GetTable(command);
			double balTotal;
			double insEst;
			double payPlanDue;
			DateTime dateLastStatement;
			List<Patient> listPatientsFamilyGuarantors;
			for(int i=0;i<tableRaw.Rows.Count;i++) {
				dataRow=table.NewRow();
				if(tableRaw.Rows[i]["SuperFamily"].ToString()=="0") {//not a super statement, just get bal info from guarantor
					balTotal=PIn.Double(tableRaw.Rows[i]["BalTotal"].ToString());
					insEst=PIn.Double(tableRaw.Rows[i]["InsEst"].ToString());
					payPlanDue=PIn.Double(tableRaw.Rows[i]["PayPlanDue"].ToString());
				}
				else {//super statement, add all guar positive balances to get bal total for super family
					listPatientsFamilyGuarantors=Patients.GetSuperFamilyGuarantors(PIn.Long(tableRaw.Rows[i]["SuperFamily"].ToString())).FindAll(x => x.HasSuperBilling);
					//exclude fams with neg balances in the total for super family stmts (per Nathan 5/25/2016)
					if(PrefC.GetBool(PrefName.BalancesDontSubtractIns)) {
						listPatientsFamilyGuarantors=listPatientsFamilyGuarantors.FindAll(x => x.BalTotal>0);
						insEst=0;
					}
					else {
						listPatientsFamilyGuarantors=listPatientsFamilyGuarantors.FindAll(x => (x.BalTotal-x.InsEst)>0);
						insEst=listPatientsFamilyGuarantors.Sum(x => x.InsEst);
					}
					balTotal=listPatientsFamilyGuarantors.Sum(x => x.BalTotal);
					payPlanDue=listPatientsFamilyGuarantors.Sum(x => x.PayPlanDue);
				}
				dataRow["amountDue"]=(balTotal-insEst).ToString("F");
				dataRow["balTotal"]=balTotal.ToString("F");;
				dataRow["billingType"]=Defs.GetName(DefCat.BillingTypes,PIn.Long(tableRaw.Rows[i]["BillingType"].ToString()));
				if(insEst==0){
					dataRow["insEst"]="";
				}
				else{
					dataRow["insEst"]=insEst.ToString("F");
				}
				dataRow["IsSent"]=tableRaw.Rows[i]["IsSent"].ToString();
				dateLastStatement=PIn.Date(tableRaw.Rows[i]["LastStatement"].ToString());
				if(dateLastStatement.Year<1880){
					dataRow["lastStatement"]="";
				}
				else{
					dataRow["lastStatement"]=dateLastStatement.ToShortDateString();
				}
				dataRow["mode"]=Lans.g("enumStatementMode",((StatementMode)PIn.Int(tableRaw.Rows[i]["Mode_"].ToString())).ToString());
				dataRow["name"]=Patients.GetNameLF(tableRaw.Rows[i]["LName"].ToString(),tableRaw.Rows[i]["FName"].ToString(),tableRaw.Rows[i]["Preferred"].ToString(),tableRaw.Rows[i]["MiddleI"].ToString());
				dataRow["PatNum"]=tableRaw.Rows[i]["PatNum"].ToString();
				if(payPlanDue==0){
					dataRow["payPlanDue"]="";
				}
				else{
					dataRow["payPlanDue"]=payPlanDue.ToString("F");
				}
				dataRow["StatementNum"]=tableRaw.Rows[i]["StatementNum"].ToString();
				dataRow["SuperFamily"]=tableRaw.Rows[i]["SuperFamily"].ToString();
				dataRow["ClinicNum"]=tableRaw.Rows[i]["ClinicNum"].ToString();
				table.Rows.Add(dataRow);
			}
			return table;
		}

		///<summary>This query is flawed.</summary>
		public static DataTable GetStatementNotesPracticeWeb(long patnum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetTable(MethodBase.GetCurrentMethod(),patnum);
			}
			string command=@"SELECT Note FROM statement Where Patnum="+patnum;
			return Db.GetTable(command);
		}

		///<summary>This query is flawed.</summary>
		public static Statement GetStatementInfoPracticeWeb(long patnum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<Statement>(MethodBase.GetCurrentMethod(),patnum);
			}
			string command=@"Select SinglePatient,DateRangeFrom,DateRangeTo,Intermingled FROM statement WHERE PatNum = "+patnum;
			return Crud.StatementCrud.SelectOne(command);
		}

		///<summary>Fetches StatementNums restricted by the DateTStamp, PatNums and a limit of records per patient. If limitPerPatient is zero all StatementNums of a patient are fetched</summary>
		public static List<long> GetChangedSinceStatementNums(DateTime dateChangedSince,List<long> listPatnumsEligibleForUpload,int limitPerPatient) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),dateChangedSince,listPatnumsEligibleForUpload,limitPerPatient);
			}
			List<long> listStatementNums = new List<long>();
			string strLimit="";
			if(limitPerPatient>0) {
				strLimit="LIMIT "+ limitPerPatient;
			}
			DataTable table;
			// there are possibly more efficient ways to implement this using a single sql statement but readability of the sql can be compromised
			if(listPatnumsEligibleForUpload.Count>0) {
				for(int i=0;i<listPatnumsEligibleForUpload.Count;i++) {
					string command="SELECT StatementNum FROM statement WHERE DateTStamp > "+POut.DateT(dateChangedSince)+" AND PatNum='" 
						+listPatnumsEligibleForUpload[i].ToString()+"' ORDER BY DateSent DESC, StatementNum DESC "+strLimit;
					table=Db.GetTable(command);
					for(int j=0;j<table.Rows.Count;j++) {
						listStatementNums.Add(PIn.Long(table.Rows[j]["StatementNum"].ToString()));
					}
				}
			}
			return listStatementNums;
		}

		///<summary>Used along with GetChangedSinceStatementNums</summary>
		public static List<Statement> GetMultStatements(List<long> listStatementNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<Statement>>(MethodBase.GetCurrentMethod(),listStatementNums);
			}
			string strStatementNums="";
			DataTable table;
			if(listStatementNums.Count>0) {
				for(int i=0;i<listStatementNums.Count;i++) {
					if(i>0) {
						strStatementNums+="OR ";
					}
					strStatementNums+="StatementNum='"+listStatementNums[i].ToString()+"' ";
				}
				string command="SELECT * FROM statement WHERE "+strStatementNums;
				table=Db.GetTable(command);
			}
			else {
				table=new DataTable();
			}
			List<Statement> listStatements=Crud.StatementCrud.TableToList(table);
			return listStatements;
		}

		///<summary>Returns an email message for the patient based on the statement passed in.</summary>
		public static EmailMessage GetEmailMessageForStatement(Statement statement,Patient patient,EmailAddress fromAddress=null) {
			if(statement.PatNum!=patient.PatNum) {
				string logMsg=Lans.g("Statements","Mismatched PatNums detected between current patient and current statement:")+"\r\n"
					+Lans.g("Statements","Statement PatNum:")+" "+statement.PatNum+" "+Lans.g("Statements","(assumed correct)")+"\r\n"
					+Lans.g("Statements","Patient PatNum:")+" "+patient.PatNum+" "+Lans.g("Statements","(possibly incorrect)");
				SecurityLogs.MakeLogEntry(EnumPermType.StatementPatNumMismatch,statement.PatNum,logMsg,LogSources.Diagnostic);
			}
			//No need to check MiddleTierRole; no call to db.
			EmailMessage emailMessage=new EmailMessage();
			emailMessage.PatNum=patient.PatNum;
			emailMessage.ToAddress=patient.Email;
			EmailAddress emailAddress=fromAddress;
			if(emailAddress==null) {
				emailAddress=EmailAddresses.GetByClinic(patient.ClinicNum);
			}
			emailMessage.FromAddress=EmailAddresses.OverrideSenderAddressClinical(emailAddress,patient.ClinicNum).GetFrom();
			string str;
			if(statement.EmailSubject!=null && statement.EmailSubject!="") {
				str=statement.EmailSubject;//Set str to the email subject if one was already set.
			}
			else {//Subject was not set.  Set str to the default billing email subject.
				str=PrefC.GetString(PrefName.BillingEmailSubject);
			}
			emailMessage.Subject=Statements.ReplaceVarsForEmail(str,patient,statement);
			if(statement.EmailBody!=null && statement.EmailBody!="") {
				str=statement.EmailBody;//Set str to the email body if one was already set.
			}
			else {//Body was not set.  Set str to the default billing email body text.
				str=PrefC.GetString(PrefName.BillingEmailBodyText);
			}
			emailMessage.BodyText=Statements.ReplaceVarsForEmail(str,patient,statement);
			emailMessage.MsgType=EmailMessageSource.Statement;
			return emailMessage;
		}

		public static EmailMessage GetEmailMessageForPortalStatement(Statement statement,Patient patient) {
			//No need to check MiddleTierRole; no call to db.
			if(statement.PatNum!=patient.PatNum) {
				string logMsg=Lans.g("Statements","Mismatched PatNums detected between current patient and current statement:")+"\r\n"
					+Lans.g("Statements","Statement PatNum:")+" "+statement.PatNum+" "+Lans.g("Statements","(assumed correct)")+"\r\n"
					+Lans.g("Statements","Patient PatNum:")+" "+patient.PatNum+" "+Lans.g("Statements","(possibly incorrect)");
				SecurityLogs.MakeLogEntry(EnumPermType.StatementPatNumMismatch,statement.PatNum,logMsg,LogSources.Diagnostic);
			}
			EmailMessage emailMessage=new EmailMessage();
			emailMessage.PatNum=patient.PatNum;
			emailMessage.ToAddress=patient.Email;
			EmailAddress emailAddress=EmailAddresses.GetByClinic(patient.ClinicNum);
			emailMessage.FromAddress=EmailAddresses.OverrideSenderAddressClinical(emailAddress,patient.ClinicNum).GetFrom();
			string emailBody;
			if(statement.EmailSubject!=null && statement.EmailSubject!="") {
				emailMessage.Subject=statement.EmailSubject;
			}
			else {//Subject was not preset, set a default subject.
				emailMessage.Subject=Lans.g("Statements","New Statement Available");
			}
			if(statement.EmailBody!=null && statement.EmailBody!="") {
				emailBody=statement.EmailBody;
			}
			else {//Body was not preset, set a body text.
				emailBody=Lans.g("Statements","Dear")+" [nameFnoPref],\r\n\r\n"
					+Lans.g("Statements","A new account statement is available.")+"\r\n\r\n"
					+Lans.g("Statements","To view your account statement, log on to our portal by following these steps:")+"\r\n\r\n"
					+Lans.g("Statements","1. Visit the following URL in a web browser:")+" "+PrefC.GetString(PrefName.PatientPortalURL)+"\r\n"
					+Lans.g("Statements","2. Enter your credentials to gain access to your account.")+"\r\n"
					+Lans.g("Statements","3. Click the Account icon on the left and select the most recent Statement to view.");
			}
			emailMessage.BodyText=Statements.ReplaceVarsForEmail(emailBody,patient,statement);
			emailMessage.MsgType=EmailMessageSource.Statement;
			return emailMessage;
		}

		///<summary>Gets a list of unsent StatementNums.</summary>
		public static List<long> GetUnsentStatements(params StatementMode[] statementModeArray) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<List<long>>(MethodBase.GetCurrentMethod(),statementModeArray);
			}
			string command=$"SELECT StatementNum FROM statement WHERE IsSent=0 ";
			if(statementModeArray.Length!=0) {
				command+=$"AND Mode_ IN({string.Join(",",statementModeArray.Select(x => POut.Enum(x)))})";
			}
			return Db.GetListLong(command);
		}
		#endregion

		#region Insert
		///<summary></summary>
		public static long Insert(Statement statement) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				//Unusual middle tier check. The billing tool can send thousands of statements to this function. In that case, middle tier will error
				//saying "Request Too Large". Because this is simple inserting, we will instead break it up into trips of 100 statements.
				for(int i=0;i<listStatements.Count;i+=100) {
					List<Statement> listStatementsToSend=listStatements.GetRange(i,Math.Min(100,listStatements.Count-i));
					Meth.GetVoid(MethodBase.GetCurrentMethod(),listStatementsToSend);
				}
				return;
			}
			Crud.StatementCrud.InsertMany(listStatements);
		}
		#endregion

		#region Update

		///<summary>Updates the statements with the send status.</summary>
		public static void UpdateSmsSendStatus(List<long> listStmtNumsToUpdate,AutoCommStatus autoCommStatus) {
			if(listStmtNumsToUpdate.Count==0) {
				return;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listStmtNumsToUpdate,autoCommStatus);
				return;
			}
			string command="UPDATE statement SET SmsSendStatus="+POut.Int((int)autoCommStatus)
				+" WHERE StatementNum IN("+string.Join(",",listStmtNumsToUpdate.Select(x => POut.Long(x)))+")";
			Db.NonQ(command);
		}

		///<summary></summary>
		public static void Update(Statement statement) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),statement);
				return;
			}
			Crud.StatementCrud.Update(statement);
		}

		///<summary></summary>
		public static bool Update(Statement statement,Statement statementOld) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetBool(MethodBase.GetCurrentMethod(),statement,statementOld);
			}
			return Crud.StatementCrud.Update(statement,statementOld);
		}

		public static void MarkSent(long statementNum,DateTime dateSent) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),statementNum,dateSent);
				return;
			}
			string command="UPDATE statement SET DateSent="+POut.Date(dateSent)+", "
				+"IsSent=1 WHERE StatementNum="+POut.Long(statementNum);
			Db.NonQ(command);
		}

		public static void AttachDoc(long statementNum,Document document,bool doUpdateDoc=true) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),statementNum,document,doUpdateDoc);
				return;
			}
			if(doUpdateDoc) {
				Documents.Update(document);
			}
			string command="UPDATE statement SET DocNum="+POut.Long(document.DocNum)
				+" WHERE StatementNum="+POut.Long(statementNum);
			Db.NonQ(command);
		}
		
		public static void DetachDocFromStatements(long docNum) {
			if(docNum==0) {
				return;//Avoid MiddleTier.
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),docNum);
				return;
			}
			Db.NonQ("UPDATE statement SET DocNum=0 WHERE DocNum="+POut.Long(docNum));
		}

		///<summary>Changes the value of the DateTStamp column to the current time stamp for all statements of a patient</summary>
		public static void ResetTimeStamps(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listStatements,forceImageDelete);
				return;
			}
			for(int i=0;i<listStatements.Count;i++) {
				//Per Nathan the image should not be deleted if the user does not have the Image Delete permission.  The statement can still be deleted.
				if(listStatements[i].DocNum!=0 && (forceImageDelete || Security.IsAuthorized(EnumPermType.ImageDelete,listStatements[i].DateSent,true))) {
					//deleted the pdf
					Patient patient=Patients.GetPat(listStatements[i].PatNum);
					string patFolder=ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
					List<Document> listDocuments=new List<Document>();
					listDocuments.Add(Documents.GetByNum(listStatements[i].DocNum,true));
					ImageStore.DeleteDocuments(listDocuments,patFolder);//May throw if document is in use.
				}
				Delete(listStatements[i]);
			}
		}

		///<summary></summary>
		public static void Delete(Statement statement) {
			//No need to check MiddleTierRole; no call to db.
			Delete(statement.StatementNum);
		}

		///<summary>For deleting a statement when user clicks Cancel.  No need to make entry in DeletedObject table.</summary>
		public static void Delete(long statementNum) {
			//No need to check MiddleTierRole; no call to db.
			DeleteAll(new List<long> { statementNum });
		}

		public static void DeleteAll(List<long> listStatementNums) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
		private static string ReplaceVarsForEmail(string str,Patient patient,Statement statement) {
			//No need to check MiddleTierRole; no call to db.
			str=ReplaceVarsForSms(str,patient,statement);
			//These were not inluded in ReplaceVarsForSms because the last name is considered PHI.
			str=str.Replace("[nameFL]",patient.GetNameFL());
			str=str.Replace("[nameFLnoPref]",patient.GetNameFLnoPref());
			str=str.Replace("[nameFnoPref]",patient.FName);
			return str;
		}

		///<summary>Replaces variable tags with the information from the patient passed in. Throws exceptions if attempt to get short guids from ODHQ has issues.</summary>
		public static string ReplaceVarsForSms(string smsTemplate,Patient patient,Statement statement,Clinic clinic=null,bool includeInsEst=false) {
			//No need to check MiddleTierRole; no call to db.
			StringBuilder stringBuilder=new StringBuilder();
			stringBuilder.Append(smsTemplate);
			if(smsTemplate.Contains("[monthlyCardsOnFile]")) {
				StringTools.RegReplace(stringBuilder,"\\[monthlyCardsOnFile]",CreditCards.GetMonthlyCardsOnFile(patient.PatNum));
			}
			StringTools.RegReplace(stringBuilder,"\\[nameF]",patient.GetNameFirst());
			StringTools.RegReplace(stringBuilder,"\\[namePref]",patient.Preferred);
			StringTools.RegReplace(stringBuilder,"\\[PatNum]",patient.PatNum.ToString());
			StringTools.RegReplace(stringBuilder,"\\[currentMonth]",DateTime.Now.ToString("MMMM"));
			if(clinic==null) { 
				clinic=Clinics.GetClinic(patient.ClinicNum)??Clinics.GetPracticeAsClinicZero();
			}
			string officePhone=clinic.Phone;
			if(string.IsNullOrEmpty(officePhone)) {
				officePhone=PrefC.GetString(PrefName.PracticePhone);
			}
			StringTools.RegReplace(stringBuilder,"\\[OfficePhone]",TelephoneNumbers.ReFormat(officePhone));
			string officeName=clinic.Description;
			if(string.IsNullOrEmpty(officeName)) {
				officeName=PrefC.GetString(PrefName.PracticeTitle);
			}
			StringTools.RegReplace(stringBuilder,"\\[OfficeName]",officeName);
			if(smsTemplate.ToLower().Contains("[statementurl]") || smsTemplate.ToLower().Contains("[statementshorturl]")) {
				AssignURLsIfNecessary(statement,patient);
			}
			StringTools.RegReplace(stringBuilder,"\\[StatementURL]",statement.StatementURL);
			StringTools.RegReplace(stringBuilder,"\\[StatementShortURL]",statement.StatementShortURL);
			double balance=statement.BalTotal;
			if(includeInsEst) {
				balance=Math.Max(balance-statement.InsEst,0);
			}
			if(balance<=0) {
				stringBuilder.Replace("[StatementBalance]",balance.ToString("c"));
			}
			else {
				StringTools.RegReplace(stringBuilder,"\\[StatementBalance]",balance.ToString("c"));
			}
			return stringBuilder.ToString();
		}

		///<summary>Allows an email receipt to be sent to a patient through the portal.  Throws exceptions in the case where the email from address is not valid. Sends a seperate email if the document was unable to be created.</summary>
		public static void EmailStatementPatientPortal(Statement statement,string toAddress,EmailAddress emailAddressFrom,Patient patient) {
			//Create the Statement Object
			Insert(statement);
			//Create the .pdf file
			SheetDef sheetDef=SheetUtil.GetStatementSheetDef(statement);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,statement.PatNum,statement.HidePayment);
			DataSet dataSet=AccountModules.GetAccount(statement.PatNum,statement,doShowHiddenPaySplits:statement.IsReceipt);
			sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=statement });
			SheetFiller.FillFields(sheet,dataSet,statement);
			SheetUtil.CalculateHeights(sheet,dataSet,statement);
			string tempPath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),statement.PatNum.ToString()+".pdf");
			SheetDrawingJob sheetDrawingJob=new SheetDrawingJob();
			PdfDocument pdfDocument=sheetDrawingJob.CreatePdf(sheet,statement,dataSet:dataSet);
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
			Document document=null;
			string errorMessage="";
			try {
				SheetDrawingJob.SavePdfToFile(pdfDocument,tempPath);
				document=ImageStore.Import(tempPath,category,Patients.GetPat(statement.PatNum));
			}
			catch {
				errorMessage="Thank you for your recent payment. An error occurred when creating your receipt, please contact your provider to request a copy.";
				EmailMessage emailMessageError=GetEmailMessageForStatement(statement,Patients.GetPat(statement.PatNum));
				emailMessageError.ToAddress=toAddress;
				emailMessageError.MsgType=EmailMessageSource.PaymentReceipt;
				emailMessageError.SentOrReceived=EmailSentOrReceived.Sent;
				emailMessageError.MsgDateTime=DateTime_.Now;
				emailMessageError.BodyText=errorMessage;
				if(emailAddressFrom==null || !EmailAddresses.IsValidEmail(emailAddressFrom.GetFrom())) {//Check to make sure that our "from" email address is valid.
					throw new ODException("Thank you for your recent payment. An error occurred when attempting to email your receipt," 
						+" please contact your provider.",ODException.ErrorCodes.ReceiptEmailAddressInvalid);
				}
				emailAddressFrom=EmailAddresses.OverrideSenderAddressClinical(emailAddressFrom,patient.ClinicNum); //Use clinic's Email Sender Address Override, if present
				try {
					EmailMessages.SendEmail(emailMessageError,emailAddressFrom);
				}
				catch (Exception ex) {
					ex.DoNothing();
					throw new ODException(errorMessage);
				}
				return;
			}
			Patient patientGuarantor=Patients.GetPat(statement.PatNum);
			string guarFolder=ImageStore.GetPatientFolder(patientGuarantor,ImageStore.GetPreferredAtoZpath());
			string fileName="";
			document.ImgType=ImageType.Document;
			document.Description=Lans.g("Statement","Receipt");
			document.DateCreated=statement.DateSent;
			statement.DocNum=document.DocNum;//this signals the calling class that the pdf was created successfully.
			//Attach Doc to the statement in the DB
			AttachDoc(statement.StatementNum,document);
			//Doc fileName and Copy to emailAttach Folder
			string attachPath=EmailAttaches.GetAttachPath();
			fileName=DateTime.Now.ToString("yyyyMMdd")+DateTime.Now.TimeOfDay.Ticks.ToString()+ODRandom.Next(1000).ToString()+".pdf";
			string filePathAndName=FileIO.FileAtoZ.CombinePaths(attachPath,fileName);
			FileIO.FileAtoZ.Copy(ImageStore.GetFilePath(Documents.GetByNum(statement.DocNum),guarFolder),filePathAndName,FileAtoZSourceDestination.AtoZToAtoZ);
			if(emailAddressFrom==null || !EmailAddresses.IsValidEmail(emailAddressFrom.GetFrom())) {//Check to make sure that our "from" email address is valid.
				throw new ODException("Thank you for your recent payment. An error occurred when attempting to email your receipt," 
					+" please refresh your page to view your statement.",ODException.ErrorCodes.ReceiptEmailAddressInvalid);
			}
			//Create email and attachment objects
			EmailMessage emailMessage=GetEmailMessageForStatement(statement,patientGuarantor);
			EmailAttach emailAttach=new EmailAttach();
			emailAttach.DisplayedFileName="Statement.pdf";
			emailAttach.ActualFileName=fileName;
			emailMessage.Attachments.Add(emailAttach);
			emailMessage.ToAddress=toAddress;
			emailMessage.MsgType=EmailMessageSource.PaymentReceipt;
			emailMessage.SentOrReceived=EmailSentOrReceived.Sent;
			emailMessage.MsgDateTime=DateTime_.Now;
			try {
				EmailMessages.SendEmail(emailMessage,emailAddressFrom);
			}
			catch(Exception ex) {
				ex.DoNothing();
				throw new ODException("Thank you for your recent payment. An error occurred when attempting to send an email receipt," 
					+" please refresh your page to view your statement.",ODException.ErrorCodes.ReceiptEmailFailedToSend);
			}
		}

		public static Statement CreateReceiptStatement(Patient patient, StatementMode statementMode) {
			Statement statement=new Statement();
			statement.PatNum=patient.PatNum;
			statement.DateSent=DateTime.Today;
			statement.IsSent=true;
			statement.Mode_=StatementMode.Email;
			statement.HidePayment=true;
			statement.Intermingled=PrefC.GetBool(PrefName.IntermingleFamilyDefault);
			statement.SinglePatient=!statement.Intermingled;
			statement.IsReceipt=true;
			statement.StatementType=StmtType.NotSet;
			statement.DateRangeFrom=DateTime.Today;
			statement.DateRangeTo=DateTime.Today;
			statement.Note="";
			statement.NoteBold="";
			Patient patientGuarantor=null;
			if(patient!=null) {
				patientGuarantor=Patients.GetPat(patient.Guarantor);
			}
			if(patientGuarantor!=null) {
				statement.IsBalValid=true;
				statement.BalTotal=patientGuarantor.BalTotal;
				statement.InsEst=patientGuarantor.InsEst;
			}
			return statement;
		}

		///<summary>If the statement does not have a short guid or URL, a call will be made to HQ to assign it one. The statement will be updated
		///to the database.</summary>
		public static void AssignURLsIfNecessary(Statement statement,Patient patient) {
			if(!string.IsNullOrEmpty(statement.ShortGUID) && !string.IsNullOrEmpty(statement.StatementURL)) {
				return;
			}
			List<WebServiceMainHQProxy.ShortGuidResult> listShortGuidResultsUrls=WebServiceMainHQProxy.GetShortGUIDs(1,1,patient.ClinicNum,
				eServiceCode.PatientPortalViewStatement);
			Statement statementOld=statement.Copy();
			statement.ShortGUID=listShortGuidResultsUrls[0].ShortGuid;
			statement.StatementURL=listShortGuidResultsUrls[0].MediumURL;
			statement.StatementShortURL=listShortGuidResultsUrls[0].ShortURL;
			Statements.Update(statement,statementOld);
		}

		public static Statement CreateLimitedStatement(List<long> listPatNumsSelected,long patNum,List<long> listPayClaimNums,List<long> listAdjustments,
			List<long> listPayNums,List<long> listProcedures,List<long> listPayPlanChargeNums,long superFamily=0,
			EnumLimitedCustomFamily limitedCustomFamily=EnumLimitedCustomFamily.None)
		{
			Statement statement=new Statement();
			statement.PatNum=patNum;
			statement.DateSent=DateTime.Today;
			statement.IsSent=false;
			statement.Mode_=StatementMode.InPerson;
			statement.HidePayment=false;
			statement.SinglePatient=listPatNumsSelected.Count==1;//SinglePatient determined by the selected transactions
			statement.Intermingled=listPatNumsSelected.Count>1 && PrefC.GetBool(PrefName.IntermingleFamilyDefault);
			statement.IsReceipt=false;
			statement.IsInvoice=false;
			statement.StatementType=StmtType.LimitedStatement;
			statement.DateRangeFrom=DateTime.MinValue;
			statement.DateRangeTo=DateTime.Today;
			statement.Note="";
			statement.NoteBold="";
			statement.IsBalValid=true;
			statement.BalTotal=0;
			statement.InsEst=0;
			statement.SuperFamily=superFamily;
			statement.LimitedCustomFamily=limitedCustomFamily;
			Statements.Insert(statement);//we need stmt.StatementNum for attaching procs, adjustments, paysplits, and payment plan charges to the statement
			for(int i=0;i<listAdjustments.Count;i++) {
				StmtLink stmtLink=new StmtLink();
				stmtLink.FKey=listAdjustments[i];
				stmtLink.StatementNum=statement.StatementNum;
				stmtLink.StmtLinkType=StmtLinkTypes.Adj;
				StmtLinks.Insert(stmtLink);
			}
			for(int i=0;i<listPayNums.Count;i++) {
				Payment payment=Payments.GetPayment(listPayNums[i]);
				List<PaySplit> listPaySplits=PaySplits.GetForPayment(listPayNums[i]);
				for(int l=0;l<listPaySplits.Count;l++) {
					StmtLink stmtLink=new StmtLink();
					stmtLink.FKey=listPaySplits[l].SplitNum;
					stmtLink.StatementNum=statement.StatementNum;
					stmtLink.StmtLinkType=StmtLinkTypes.PaySplit;
					StmtLinks.Insert(stmtLink);
				}
			}
			for(int i=0;i<listProcedures.Count;i++) {
				StmtLink stmtLink=new StmtLink();
				stmtLink.FKey=listProcedures[i];
				stmtLink.StatementNum=statement.StatementNum;
				stmtLink.StmtLinkType=StmtLinkTypes.Proc;
				StmtLinks.Insert(stmtLink);
			}
			for(int i=0;i<listPayClaimNums.Count;i++) {
				StmtLink stmtLink=new StmtLink();
				stmtLink.FKey=listPayClaimNums[i];
				stmtLink.StatementNum=statement.StatementNum;
				stmtLink.StmtLinkType=StmtLinkTypes.ClaimPay;
				StmtLinks.Insert(stmtLink);
			}
			for(int i=0;i<listPayPlanChargeNums.Count;i++) {
				StmtLink stmtLink=new StmtLink();
				stmtLink.FKey=listPayPlanChargeNums[i];
				stmtLink.StatementNum=statement.StatementNum;
				stmtLink.StmtLinkType=StmtLinkTypes.PayPlanCharge;
				StmtLinks.Insert(stmtLink);
			}
			if(limitedCustomFamily!=EnumLimitedCustomFamily.None || listPatNumsSelected.Any(x => x!=patNum)) {
				for(int i=0;i<listPatNumsSelected.Count;i++) {
					StmtLink stmtLink=new StmtLink();
					stmtLink.FKey=listPatNumsSelected[i];
					stmtLink.StatementNum=statement.StatementNum;
					stmtLink.StmtLinkType=StmtLinkTypes.PatNum;
					StmtLinks.Insert(stmtLink);
				}
			}
			//set statement lists to null in order to force refresh the lists now that we've inserted all of the StmtAttaches
			statement.ListAdjNums=null;
			statement.ListPaySplitNums=null;
			statement.ListProcNums=null;
			statement.ListInsPayClaimNums=null;
			statement.ListPayPlanChargeNums=null;
			statement.ListPatNums=null;
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				//Currently when using MiddleTier null lists inside an object like above causes the deserialized statement to incorrectly set the null lists
				//to empty lists.
				//In the case of a Statement, the public property associated to list in question does not run queries to populate the list as expected because
				//it checks for a null list, but instead sees an empty list.
				//We may fix the underlying MiddleTier bug later, but this is an immediate patch to address this symptom.
				statement.ListAdjNums=StmtLinks.GetForStatementAndType(statement.StatementNum,StmtLinkTypes.Adj);
				statement.ListPaySplitNums=StmtLinks.GetForStatementAndType(statement.StatementNum,StmtLinkTypes.PaySplit);
				if(statement.IsInvoice) {
					statement.ListProcNums=Procedures.GetForInvoice(statement.StatementNum);
				}
				else {
					statement.ListProcNums=StmtLinks.GetForStatementAndType(statement.StatementNum,StmtLinkTypes.Proc);
				}
				statement.ListInsPayClaimNums=StmtLinks.GetForStatementAndType(statement.StatementNum,StmtLinkTypes.ClaimPay);
				statement.ListPayPlanChargeNums=StmtLinks.GetForStatementAndType(statement.StatementNum,StmtLinkTypes.PayPlanCharge);
				statement.ListPatNums=StmtLinks.GetForStatementAndType(statement.StatementNum,StmtLinkTypes.PatNum);
			}
			return statement;
		}

		///<summary>Creates statement prods for a statement based off of the dataSet passed in, then syncs this list with the existing statementprods for the statement in the DB.</summary>
		public static void SyncStatementProdsForStatement(DataSet dataSet,long statementNum,long docNum) {
			//No need to check MiddleTierRole; no call to db.
			if(docNum==0) {
				return;
			}
			StatementProds.SyncForStatement(dataSet,statementNum,docNum);
		}

		///<summary>Pass in a list of statement DataSets. Creates statement prods for the statements based off of their dataSets, then syncs these statementprods with the existing statementprods for the statements in the DB.</summary>
		public static void SyncStatementProdsForMultipleStatements(List<StatementData> listStatementDatas) {
			//No need to check MiddleTierRole; no call to db.
			StatementProds.SyncForMultipleStatements(listStatementDatas);
		}

		///<summary>Sets the installment plans field on each of the statements passed in.</summary>
		public static void AddInstallmentPlansToStatements(List<Statement> listStatements,Dictionary<long,Family> dictFamilies=null) {
			//No need to check MiddleTierRole; no call to db.
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
					continue;
				}
				if(dictionaryFamInstallmentPlans.ContainsKey(dictFamilies[listStatements[i].PatNum].Guarantor.PatNum)) {
					listStatements[i].ListInstallmentPlans=new List<InstallmentPlan> { dictionaryFamInstallmentPlans[dictFamilies[listStatements[i].PatNum].Guarantor.PatNum] };
					continue;
				}
				listStatements[i].ListInstallmentPlans=new List<InstallmentPlan>();
			}
		}

		///<summary>Returns the family's balance according to the most recent statement across the entire family.</summary>
		public static double GetFamilyBalance(long patNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				return Meth.GetObject<double>(MethodBase.GetCurrentMethod(),patNum);
			}
			List<Patient> listPatients=OpenDentBusiness.Patients.GetFamily(patNum).ListPats.ToList();
			string command="SELECT * FROM statement "
										+"WHERE PatNum IN("+string.Join(",",listPatients.Select(x => POut.Long(x.PatNum)).ToList())+") "
										+"AND IsSent=1 "
										+"ORDER BY StatementNum DESC LIMIT 1";
			Statement statement=OpenDentBusiness.Crud.StatementCrud.SelectOne(command);
			if(statement==null) {
				return 0;
			}
			return statement.BalTotal-statement.InsEst;
		}

		///<summary>Returns a dictionary of Key-PatNum, Value-Family for the statements passed in.</summary>
		public static Dictionary<long,Family> GetFamiliesForStatements(List<Statement> listStatements) {
			//No need to check MiddleTierRole; no call to db.
			if(listStatements.IsNullOrEmpty()) {
				return new Dictionary<long, Family>();
			}
			Dictionary<long,Family> dictionaryFamilyValues=Patients.GetFamilies(listStatements.Select(x => x.PatNum).ToList())
				.SelectMany(fam => fam.ListPats.Select(y => new { y.PatNum,fam }))
				.Distinct()
				.ToDictionary(x => x.PatNum,x => x.fam);
			return dictionaryFamilyValues;
		}

		///<summary>List of batches of statements. BatchNum will start with 1 for UI.</summary>
		public static List<StatementBatch> GetBatchesForStatements(List<Statement> listStatements,List<Patient> listPatients) 
		{
			//No need to check MiddleTierRole; no call to db.
			List<StatementBatch> listBatchesOfStatements=new List<StatementBatch>();
			if(listStatements.IsNullOrEmpty()) {
				return listBatchesOfStatements;
			}
			//If clinics are on, batch by guarantor of statement's clinicnum. Otherwise, batch by BillingElectBatchMax pref.
			//3/1/24 SamO. Bug that I don't care to fix. When clinics are on, we don't enforce the BillingElectBatchMax batch size for each clinic.
			//So if a clinic has a batch of statements that is bigger than max allowed by that vendor (EDS), that is likely a problem. Otherwise why did we implement batches in the first place?
			if(PrefC.HasClinicsEnabled) {
				listBatchesOfStatements=listStatements.GroupBy(x => {
					//No need to test for null. Guaranteed to have this patient.
					Patient patient=listPatients.Find(y => y.PatNum==x.PatNum);
					//Find the guranantor clinicnum.
					Patient patientGuarantor=listPatients.Find(x => x.PatNum==patient.Guarantor);
					if(patientGuarantor==null) {
						return patient.ClinicNum; //should never happen, every pat should have a guarantor (sometimes yourself)
					}
					return patientGuarantor.ClinicNum;
				})
				.Select((x,i) => new StatementBatch {
					BatchNum=i+1, //1-based batch num. For UI.
					ClinicNum=x.Key, //We grouped by guar ClinicNum above.
					ListStatements=x.ToList(), //The grouping of statements for this ClinicNum.
					ListStatementDatas=new List<StatementData>(),
					ListEbillStatements=new List<EbillStatement>(),
				}).ToList();
				return listBatchesOfStatements;
			}
			int maxStmtsPerBatch=PrefC.GetInt(PrefName.BillingElectBatchMax);
			BillingUseElectronicEnum electronicBillingType=PrefC.GetEnum<BillingUseElectronicEnum>(PrefName.BillingUseElectronic);
			if(maxStmtsPerBatch==0 || electronicBillingType.In(BillingUseElectronicEnum.POS,BillingUseElectronicEnum.EDS)) {//Max is disabled for Output to File billing option or using EDS.
				maxStmtsPerBatch=listStatements.Count;//Make the batch size equal to the list of statements so that we send them all at once.
			}
			StatementBatch statementBatch=null;
			for(int i=0;i<listStatements.Count;i++) {
				if(i % maxStmtsPerBatch==0) {
					statementBatch=new StatementBatch {
						BatchNum=listBatchesOfStatements.Count+1, //1-based batch num. For UI.
						ClinicNum=0,
						ListStatements=new List<Statement>(),
						ListStatementDatas=new List<StatementData>(),
						ListEbillStatements=new List<EbillStatement>(),
					};
					listBatchesOfStatements.Add(statementBatch);
				}
				statementBatch.ListStatements.Add(listStatements[i]);
			}
			return listBatchesOfStatements;
		}

		///<summary>The filePath is the full path to the output file if the clinics feature is disabled (for a single location practice).</summary>
		public static string GetEbillFilePathForClinic(string filePath,long clinicNum) {
			//No need to check MiddleTierRole; no call to db.
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
			return ODFileUtils.CombinePaths(Path.GetDirectoryName(filePath),ODFileUtils.CleanFileName(fileName));
		}

		///<summary>Returns a list of failed messages. If list is empty then all messages succeeded. 
		///Statement.TagOD must be set to SmsToMobile.GuidMessage before calling this method.</summary>
		public static List<SmsToMobile> HandleSmsSent(List<SmsToMobile> listSmsToMobiles,List<Statement> listStatements)
		{
			//No need to check MiddleTierRole; no call to db.
			//WSHQ.SmsSend will only return FailNoCharge or Pending so we only need to handle those 2 cases here. FailWithCharge is impossible at this stage of the text message life.
			List<long> listStatementNumsSuccess=listSmsToMobiles
				//SmsToMobile that were queued successfully. GuidMessage was boxed into Statement.TagOD by BillingL.
				.Where(x => x.SmsStatus!=SmsDeliveryStatus.FailNoCharge && listStatements.Any(y => y.TagOD is string guidMessage && guidMessage==x.GuidMessage))
				//That correspond to a statement
				.Select(x => listStatements.Find(y => y.TagOD is string guidMessage && guidMessage==x.GuidMessage).StatementNum)
				.ToList();
			UpdateSmsSendStatus(listStatementNumsSuccess,AutoCommStatus.SendSuccessful);
			List<long> listStatementNumsFailure=listSmsToMobiles
				//SmsToMobile that were queued successfully. GuidMessage was boxed into Statement.TagOD by BillingL.
				.Where(x => x.SmsStatus==SmsDeliveryStatus.FailNoCharge && listStatements.Any(y => y.TagOD is string guidMessage && guidMessage==x.GuidMessage))
				//That correspond to a statement
				.Select(x => listStatements.Find(y => y.TagOD is string guidMessage && guidMessage==x.GuidMessage).StatementNum)
				.ToList();
			UpdateSmsSendStatus(listStatementNumsFailure,AutoCommStatus.SendFailed);
			return listSmsToMobiles.FindAll(x => x.SmsStatus==SmsDeliveryStatus.FailNoCharge);
		}

		///<summary>Returns the mode for the statement.</summary>
		public static StatementMode GetStatementMode(PatAging patAging) {
			//No need to check MiddleTierRole; no call to db.
			StatementMode statementMode;
			if(PrefC.GetEnum<BillingUseElectronicEnum>(PrefName.BillingUseElectronic)==BillingUseElectronicEnum.None) {
				statementMode=StatementMode.Mail; 
			}
			else {
				statementMode=StatementMode.Electronic;
			}
			Def defBillingType=Defs.GetDef(DefCat.BillingTypes,patAging.BillingType);
			if(defBillingType != null && defBillingType.ItemValue=="E") {
				statementMode=StatementMode.Email;
			}
			return statementMode;
		}

		///<summary>Returns true if the patient statement mode has an option to send by SMS.</summary>
		public static bool DoSendSms(PatAging patAging,Dictionary<long,PatAgingData> dictPatAgingData,List<StatementMode> listStatementModes) {
			//No need to check MiddleTierRole; no call to db.
			PatAgingData patAgingData;
			dictPatAgingData.TryGetValue(patAging.PatNum,out patAgingData);
			if(listStatementModes.Contains(GetStatementMode(patAging))
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
		public static string CreateStatementPdfSheets(Statement statement,Patient patient,Family family,DataSet dataSet) {
			//No need to check MiddleTierRole; no call to db.
			Statement statementNew=statement;
			SheetDef sheetDef=SheetUtil.GetStatementSheetDef(statementNew);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,statementNew.PatNum,statementNew.HidePayment);
			sheet.Parameters.Add(new SheetParameter(true,"Statement") { ParamValue=statementNew });
			SheetFiller.FillFields(sheet,dataSet,statementNew,patient: patient,family: family);
			SheetUtil.CalculateHeights(sheet,dataSet,statementNew,pat: patient,patGuar: family.Guarantor);
			string tempPath=ODFileUtils.CombinePaths(PrefC.GetTempFolderPath(),statementNew.PatNum.ToString()+".pdf");
			SheetPrinting.CreatePdf(sheet,tempPath,statementNew,dataSet,null,pat: patient,patGuar: family.Guarantor);
			List<Def> listDefsImageCat=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			long category=0;
			for(int i = 0;i<listDefsImageCat.Count;i++) {
				if(Regex.IsMatch(listDefsImageCat[i].ItemValue,@"S")) {
					category=listDefsImageCat[i].DefNum;
					break;
				}
			}
			if(category==0) {
				category=listDefsImageCat[0].DefNum;//put it in the first category.
			}
			//create doc--------------------------------------------------------------------------------------
			OpenDentBusiness.Document document=null;
			try {
				document=ImageStore.Import(tempPath,category,patient);
			}
			catch {
				return "";//Error saving the document
			}
			document.ImgType=ImageType.Document;
			if(statementNew.IsInvoice) {
				document.Description=Lans.g(nameof(Statements),"Invoice");
			}
			else {
				if(statementNew.IsReceipt==true) {
					document.Description=Lans.g(nameof(Statements),"Receipt");
				}
				else {
					document.Description=Lans.g(nameof(Statements),"Statement");
				}
			}
			statementNew.DocNum=document.DocNum;//this signals the calling class that the pdf was created successfully.
			Statements.AttachDoc(statementNew.StatementNum,document);
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
			StringBuilder stringBuilderExportCSV=new StringBuilder();
			long stateNum=statement.StatementNum;
			stringBuilderExportCSV.AppendLine("Invoice/Statement Number,"
				+"Date Created," 
				+"Procedure Total," 
				+"Ins Estimate," 
				+"Total Amount,");
			stringBuilderExportCSV.AppendLine($"\"{stateNum}\","
				+$"\"{statement.DateSent.ToShortDateString()}\","
				+$"\"{statement.BalTotal}\"," 
				+$"\"{statement.InsEst}\"," 
				+$"\"{statement.BalTotal-statement.InsEst}\",");
			stringBuilderExportCSV.AppendLine("");
			stringBuilderExportCSV.AppendLine("");
			stringBuilderExportCSV.AppendLine("Date,"
				+"Patient Number,"
				+"Patient Name,"
				+"Code,"
				+"Description,"
				+"Charges,"
				+"Credits,"
				+"Balance");
			for(int i=0;i<dataTable.Rows.Count;i++) {
				long patNum=PIn.Long(dataTable.Rows[i]["PatNum"].ToString());
				stringBuilderExportCSV.AppendLine($"\"{dataTable.Rows[i]["date"]}\","
					+$"\"{patNum}\"," 
					+$"\"{Patients.GetNameFL(patNum)}\","
					+$"\"{dataTable.Rows[i]["ProcCode"]}\","
					+$"\"{dataTable.Rows[i]["description"]}\","
					+$"\"{dataTable.Rows[i]["charges"].ToString()}\","
					+$"\"{dataTable.Rows[i]["credits"].ToString()}\","
					+$"\"{dataTable.Rows[i]["balance"].ToString()}\",");
			}
			File.WriteAllText(path,stringBuilderExportCSV.ToString());
			return path;
		}

		///<summary>Takes the passed in patient to create a statement for the guarantor. This logic used to just exist behind the toolBarButStatement_Click in the account controller</summary>
		public static Statement GenerateStatement(Patient patient,DateTime dateStart,DateTime dateEnd,StatementMode statementMode) {
			Statement statement=new Statement();
			statement.PatNum=patient.Guarantor;
			statement.DateSent=DateTime.Today;
			statement.IsSent=true;
			statement.Mode_=statementMode;
			statement.HidePayment=false;
			statement.SinglePatient=false;
			statement.Intermingled=PrefC.GetBool(PrefName.IntermingleFamilyDefault);
			statement.StatementType=StmtType.NotSet;
			statement.DateRangeTo=DateTime.Today;//This is needed for payment plan accuracy.//new DateTime(2200,1,1);
			if(dateEnd!=DateTime.MinValue) {
				statement.DateRangeTo=dateEnd;
			}
			statement.DateRangeFrom=DateTime.MinValue;
			if(dateStart!=DateTime.MinValue) {//dateStart has ultimate precedence. User may have intentionally set the date range for statement.
				statement.DateRangeFrom=dateStart;
			}
			else {//Use preferences to determine the "from" date.
				long billingDefaultsLastDaysPref=PrefC.GetLong(PrefName.BillingDefaultsLastDays);
				if(billingDefaultsLastDaysPref > 0) {//0 days means ignore preference and show everything.
					statement.DateRangeFrom=DateTime.Today.AddDays(-billingDefaultsLastDaysPref);
				}
				if(PrefC.GetBool(PrefName.BillingShowTransSinceBalZero)) {
					Patient patientForAging=Patients.GetPat(statement.PatNum);
					List<PatAging> listPatAgings=Patients.GetAgingListSimple(new List<long> {}, new List<long> { patientForAging.Guarantor },true);
					DataTable tableBals=Ledgers.GetDateBalanceBegan(listPatAgings,isSuperBills:false);//More Options selection has a super family option. We would need new checkbox here.
					if(tableBals.Rows.Count > 0) {
						DateTime dateTimeFrom=PIn.Date(tableBals.Rows[0]["DateZeroBal"].ToString());
						if(dateTimeFrom > statement.DateRangeFrom) {//Zero balance date range has precedence if it's more recent than billing default date range.
							statement.DateRangeFrom=dateTimeFrom;
						}
					}
				}
			}
			statement.Note="";
			statement.NoteBold="";
			Patient patientGuarantor=null;
			if(patient!=null) {
				patientGuarantor=Patients.GetPat(patient.Guarantor);
			}
			if(patientGuarantor!=null) {
				statement.IsBalValid=true;
				statement.BalTotal=patientGuarantor.BalTotal;
				statement.InsEst=patientGuarantor.InsEst;
			}
			return statement;
		}

		///<summary>Returns the PatNum of the patient that this statement is responsible for. Typically returns StatementCur.PatNum.
		///Can return a different PatNum if this is a SinglePatient statement and there is only one PatNum StmtLink associated with this statement.</summary>
		public static long GetPatNumForGetAccount(Statement statement) {
			long patNum=statement.PatNum;
			if(statement.SinglePatient && statement.ListPatNums.Distinct().Count()==1) {
				patNum=statement.ListPatNums.First();
			}
			return patNum;
		}
		#endregion
	}

	///<summary>Holds all of the statement and StatementProd data relevant to syncing StatementProds and late charges.</summary>
	[Serializable]
	public class StatementData {
		///<summary>Date the statement was sent.</summary>
		public DateTime DateSent;
		///<summary>PatNum of the guarantor of the family that the statement is for or the PatNum of the SuperFamily head if the statement is a SuperFamily statement.</summary>
		public long PatNumGuarantor;
		///<summary>Guarantor's primary provider's ProvNum or the SuperFamily head's primary provider's ProvNum if the statement is a SuperFamily statement.</summary>
		public long ProvNumPriGuarantor;
		///<summary>True if the statement is a superfamily statement.</summary>
		public bool IsSuperFamilyStatement;
		///<summary>Holds the PatNums of all guarantors in super family if the statement is a super family statement, otherwise it just holds the family's guarantor.</summary>
		public List<long> ListPatNumsGuarantor=new List<long>();
		///<summary>The StatementProds associated to the statement.</summary>
		public List<StatementProd> ListStatementProds=new List<StatementProd>();
		///<summary>The DocNum of the document associated to the statement.</summary>
		public long DocNum;
		///<summary>Specific tables and columns from the DataSet used to create the statement for inserting or syncing StatementProds.</summary>
		[XmlIgnore]
		public DataSet DataSetStmtNew;

		///<summary>For serialization purposes.</summary>
		public StatementData() {
		}

		///<summary>This constructur only sets the DocNum and StmtDataSet and is only used when building a collection of StatementData sets for the purpose of syncing StatementProds for multiple statements.</summary>
		public StatementData(DataSet dataSetStmt,long docNum) {
			DataSetStmtNew=new DataSet();
			for(int i=0;i<dataSetStmt.Tables.Count;i++) {
				//Each family member will have their own account table, so only consider tables that start with 'account'.
				if(!dataSetStmt.Tables[i].TableName.StartsWith("account")) {
					continue;
				}
				DataTable tableAccount=dataSetStmt.Tables[i].Copy();
				//Remove columns that are not used when inserting or syncing StatementProds to save memory.
				for(int j=tableAccount.Columns.Count-1;j>=0;j--) {
					if(!tableAccount.Columns[j].ColumnName.In("AdjNum","creditsDouble","PayPlanChargeNum","ProcNum","procsOnObj")) {
						tableAccount.Columns.RemoveAt(j);
					}
				}
				DataSetStmtNew.Tables.Add(tableAccount);
			}
			DocNum=docNum;
		}

		///<summary>Used when creating late charges. Gets a list of StatementData objects based on the filters used in ForLateCharges. Should only be run after aging has been run.</summary>
		public static List<StatementData> GetListStatementDataForLateCharges(bool isExcludeAccountNoTil,bool isExcludeExistingLateCharges,
			decimal excludeBalancesLessThan,DateTime dateRangeStart,DateTime dateRangeEnd,List<long> listBillingTypes)
		{
			if(listBillingTypes.IsNullOrEmpty()) {
				return new List<StatementData>();
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
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
			List<StatementData> listStatementDatas=new List<StatementData>();
			//If we end up wanting to prevent late charges from being created twice for a single statement that has had multiple documents made for it,
			//we can remove the dictionary entry for the statement in this loop if we come across a StatementProd that has a non-zero LateChargeAdjNum.
			//We would then have to remove the "ON statementprod.LateChargeAdjNum=0 clause from the query above.
			for(int i=0;i<table.Rows.Count;i++) {
				DataRow dataRow=table.Rows[i];
				long statementNum=PIn.Long(dataRow["StatementNum"].ToString());
				//If a list entry for the statement does not yet exist, create one.
				//All the StatementProds should have the same StatementNum.
				StatementData statementData=listStatementDatas.Find(x => x.ListStatementProds.Any(y => y.StatementNum==statementNum));
				if(statementData==null) {
					statementData=new StatementData();
					statementData.DateSent=PIn.Date(dataRow["DateSent"].ToString());
					statementData.PatNumGuarantor=PIn.Long(dataRow["PatNum"].ToString());
					statementData.ProvNumPriGuarantor=PIn.Long(dataRow["PriProv"].ToString());
					statementData.IsSuperFamilyStatement=PIn.Long(dataRow["SuperFamily"].ToString())!=0;
					statementData.ListPatNumsGuarantor.Add(statementData.PatNumGuarantor);
					if(statementData.IsSuperFamilyStatement) {
						statementData.ListPatNumsGuarantor=Patients.GetSuperFamilyGuarantors(statementData.PatNumGuarantor).Select(x => x.PatNum).ToList();
					}
					listStatementDatas.Add(statementData);
				}
				//Then, add the statement prod to the list in this statement's list entry.
				StatementProd statementProd=new StatementProd();
				statementProd.StatementProdNum=PIn.Long(dataRow["StatementProdNum"].ToString());
				statementProd.StatementNum=statementNum;
				statementProd.FKey=PIn.Long(dataRow["FKey"].ToString());
				statementProd.ProdType=(ProductionType)PIn.Int(dataRow["ProdType"].ToString());
				statementProd.LateChargeAdjNum=PIn.Long(dataRow["LateChargeAdjNum"].ToString());
				statementData.ListStatementProds.Add(statementProd);
			}
			return listStatementDatas;
		}

	}

	public struct EbillStatement {
		public Statement Statement;
		public Family Family;
	}

}