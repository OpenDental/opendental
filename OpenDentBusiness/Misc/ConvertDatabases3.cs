
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.Cryptography;
using CodeBase;
using DataConnectionBase;

namespace OpenDentBusiness {
	public partial class ConvertDatabases {

		#region Helper Functions

		///<summary>Encrypts signature text and returns a base 64 string so that it can go directly into the database.
		///Copied from MiscUtils.Encrypt() so that the data conversion will never change historically.</summary>
		public static string Encrypt(string encrypt) {
			UTF8Encoding enc=new UTF8Encoding();
			byte[] arrayEncryptBytes=Encoding.UTF8.GetBytes(encrypt);
			MemoryStream ms=new MemoryStream();
			CryptoStream cs=null;
			Aes aes=new AesManaged();
			aes.Key=enc.GetBytes("AKQjlLUjlcABVbqp");
			aes.IV=new byte[16];
			ICryptoTransform encryptor=aes.CreateEncryptor(aes.Key,aes.IV);
			cs=new CryptoStream(ms,encryptor,CryptoStreamMode.Write);
			cs.Write(arrayEncryptBytes,0,arrayEncryptBytes.Length);
			cs.FlushFinalBlock();
			byte[] retval=new byte[ms.Length];
			ms.Position=0;
			ms.Read(retval,0,(int)ms.Length);
			cs.Dispose();
			ms.Dispose();
			if(aes!=null) {
				aes.Clear();
			}
			return Convert.ToBase64String(retval);
		}

		#endregion Helper Functions

		///<summary>Oracle compatible: 07/11/2013</summary>
		private static void To13_2_1() {
			if(FromVersion<new Version("13.2.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.2.1");//No translation in convert script.
				string command;
				//Add TaskEdit permission to everyone------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				DataTable table=Db.GetTable(command);
				long groupNum;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",66)";//POut.Int((int)Permissions.TaskEdit)
						Db.NonQ(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",66)";//POut.Int((int)Permissions.TaskEdit)
						Db.NonQ(command);
					}
				}
				//add WikiListSetup permissions for users that have security admin------------------------------------------------------
				command="SELECT UserGroupNum FROM grouppermission WHERE PermType=24";//POut.Int((int)Permissions.SecurityAdmin)
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i][0].ToString());
						command="INSERT INTO grouppermission (NewerDate,UserGroupNum,PermType) "
						+"VALUES('0001-01-01',"+POut.Long(groupNum)+",67)";//POut.Int((int)Permissions.WikiListSetup);
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i][0].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,NewerDate,UserGroupNum,PermType) "
						+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,TO_DATE('0001-01-01','YYYY-MM-DD'),"+POut.Long(groupNum)+",67)";//POut.Int((int)Permissions.WikiListSetup)
						Db.NonQ32(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PatientPortalURL','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PatientPortalURL','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD BillingNote varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD BillingNote varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD RepeatChargeNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog ADD INDEX (RepeatChargeNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD RepeatChargeNum number(20)";
					Db.NonQ(command);
					command="UPDATE procedurelog SET RepeatChargeNum = 0 WHERE RepeatChargeNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog MODIFY RepeatChargeNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX procedurelog_RepeatChargeNum ON procedurelog (RepeatChargeNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS reseller";
					Db.NonQ(command);
					command=@"CREATE TABLE reseller (
						ResellerNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						UserName varchar(255) NOT NULL,
						ResellerPassword varchar(255) NOT NULL,
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE reseller'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE reseller (
						ResellerNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						UserName varchar2(255),
						ResellerPassword varchar2(255),
						CONSTRAINT reseller_ResellerNum PRIMARY KEY (ResellerNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX reseller_PatNum ON reseller (PatNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS resellerservice";
					Db.NonQ(command);
					command=@"CREATE TABLE resellerservice (
						ResellerServiceNum bigint NOT NULL auto_increment PRIMARY KEY,
						ResellerNum bigint NOT NULL,
						CodeNum bigint NOT NULL,
						Fee double NOT NULL,
						INDEX(ResellerNum),
						INDEX(CodeNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE resellerservice'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE resellerservice (
						ResellerServiceNum number(20) NOT NULL,
						ResellerNum number(20) NOT NULL,
						CodeNum number(20) NOT NULL,
						Fee number(38,8) NOT NULL,
						CONSTRAINT resellerservice_ResellerServic PRIMARY KEY (ResellerServiceNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX resellerservice_ResellerNum ON resellerservice (ResellerNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX resellerservice_CodeNum ON resellerservice (CodeNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE registrationkey ADD IsResellerCustomer tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE registrationkey ADD IsResellerCustomer number(3)";
					Db.NonQ(command);
					command="UPDATE registrationkey SET IsResellerCustomer = 0 WHERE IsResellerCustomer IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE registrationkey MODIFY IsResellerCustomer NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE repeatcharge ADD CopyNoteToProc tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE repeatcharge ADD CopyNoteToProc number(3)";
					Db.NonQ(command);
					command="UPDATE repeatcharge SET CopyNoteToProc = 0 WHERE CopyNoteToProc IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE repeatcharge MODIFY CopyNoteToProc NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS xchargetransaction";
					Db.NonQ(command);
					command=@"CREATE TABLE xchargetransaction (
						XChargeTransactionNum bigint NOT NULL auto_increment PRIMARY KEY,
						TransType varchar(255) NOT NULL,
						Amount double NOT NULL,
						CCEntry varchar(255) NOT NULL,
						PatNum bigint NOT NULL,
						Result varchar(255) NOT NULL,
						ClerkID varchar(255) NOT NULL,
						ResultCode varchar(255) NOT NULL,
						Expiration varchar(255) NOT NULL,
						CCType varchar(255) NOT NULL,
						CreditCardNum varchar(255) NOT NULL,
						BatchNum varchar(255) NOT NULL,
						ItemNum varchar(255) NOT NULL,
						ApprCode varchar(255) NOT NULL,
						TransactionDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						INDEX(PatNum),
						INDEX(TransactionDateTime)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE xchargetransaction'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE xchargetransaction (
						XChargeTransactionNum number(20) NOT NULL,
						TransType varchar2(255),
						Amount number(38,8) NOT NULL,
						CCEntry varchar2(255),
						PatNum number(20) NOT NULL,
						Result varchar2(255),
						ClerkID varchar2(255),
						ResultCode varchar2(255),
						Expiration varchar2(255),
						CCType varchar2(255),
						CreditCardNum varchar2(255),
						BatchNum varchar2(255),
						ItemNum varchar2(255),
						ApprCode varchar2(255),
						TransactionDateTime date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT xchargetransaction_XChargeTran PRIMARY KEY (XChargeTransactionNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX xchargetransaction_PatNum ON xchargetransaction (PatNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX xchargetransaction_Transaction ON xchargetransaction (TransactionDateTime)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('CanadaODAMemberNumber','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'CanadaODAMemberNumber','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('CanadaODAMemberPass','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'CanadaODAMemberPass','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patient ADD SmokingSnoMed varchar(32) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patient ADD SmokingSnoMed varchar2(32)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE repeatcharge ADD CreatesClaim tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE repeatcharge ADD CreatesClaim number(3)";
					Db.NonQ(command);
					command="UPDATE repeatcharge SET CreatesClaim = 0 WHERE CreatesClaim IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE repeatcharge MODIFY CreatesClaim NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE repeatcharge ADD IsEnabled tinyint NOT NULL";
					Db.NonQ(command);
					command="UPDATE repeatcharge SET IsEnabled = 1";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE repeatcharge ADD IsEnabled number(3)";
					Db.NonQ(command);
					//command="UPDATE repeatcharge SET IsEnabled = 0 WHERE IsEnabled IS NULL";
					command="UPDATE repeatcharge SET IsEnabled = 1 WHERE IsEnabled IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE repeatcharge MODIFY IsEnabled NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE rxalert ADD IsHighSignificance tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE rxalert ADD IsHighSignificance number(3)";
					Db.NonQ(command);
					command="UPDATE rxalert SET IsHighSignificance = 0 WHERE IsHighSignificance IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE rxalert MODIFY IsHighSignificance NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('EhrRxAlertHighSeverity','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'EhrRxAlertHighSeverity','0')";
					Db.NonQ(command);
				}
				//Oracle compatible
				command="UPDATE patient SET SmokingSnoMed='449868002' WHERE SmokeStatus=5";//CurrentEveryDay
				Db.NonQ(command);
				command="UPDATE patient SET SmokingSnoMed='428041000124106' WHERE SmokeStatus=4";//CurrentSomeDay
				Db.NonQ(command);
				command="UPDATE patient SET SmokingSnoMed='8517006' WHERE SmokeStatus=3";//FormerSmoker
				Db.NonQ(command);
				command="UPDATE patient SET SmokingSnoMed='266919005' WHERE SmokeStatus=2";//NeverSmoked
				Db.NonQ(command);
				command="UPDATE patient SET SmokingSnoMed='77176002' WHERE SmokeStatus=1";//SmokerUnknownCurrent
				Db.NonQ(command);
				command="UPDATE patient SET SmokingSnoMed='266927001' WHERE SmokeStatus=0";//UnknownIfEver
				Db.NonQ(command);
				command="ALTER TABLE patient DROP COLUMN SmokeStatus";
				Db.NonQ(command);
				//Add ICD9Code to DiseaseDef and update eduresource and disease to use DiseaseDefNum instead of ICD9Num----------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE diseasedef ADD ICD9Code varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE diseasedef ADD ICD9Code varchar2(255)";
					Db.NonQ(command);
				}
				//command="SELECT DISTINCT Description,ICD9Code,icd9.ICD9Num "
				//  +"FROM icd9,eduresource,disease,reminderrule "
				//  +"WHERE icd9.ICD9Num=eduresource.ICD9Num "
				//  +"OR icd9.ICD9Num=disease.ICD9Num "
				//  +"OR (ReminderCriterion=6 AND icd9.ICD9Num=CriterionFK)";//6=ICD9
				//table=Db.GetTable(command);
				List<string> listDescription=new List<string>();
				List<string> listICD9Code=new List<string>();
				List<long> listICD9Num=new List<long>();
				command="SELECT DISTINCT icd9.Description,icd9.ICD9Code,icd9.ICD9Num "
					+"FROM icd9,eduresource "
					+"WHERE icd9.ICD9Num=eduresource.ICD9Num";
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					listDescription.Add(PIn.String(table.Rows[i]["Description"].ToString()));
					listICD9Code.Add(PIn.String(table.Rows[i]["ICD9Code"].ToString()));
					listICD9Num.Add(PIn.Long(table.Rows[i]["ICD9Num"].ToString()));
				}
				command="SELECT DISTINCT Description,ICD9Code,icd9.ICD9Num "
					+"FROM icd9,disease "
					+"WHERE icd9.ICD9Num=disease.ICD9Num ";//6=ICD9
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					if(listICD9Num.Contains(PIn.Long(table.Rows[i]["ICD9Num"].ToString()))) {
						continue;
					}
					listDescription.Add(PIn.String(table.Rows[i]["Description"].ToString()));
					listICD9Code.Add(PIn.String(table.Rows[i]["ICD9Code"].ToString()));
					listICD9Num.Add(PIn.Long(table.Rows[i]["ICD9Num"].ToString()));
				}
				command="SELECT DISTINCT Description,ICD9Code,icd9.ICD9Num "
					+"FROM icd9,reminderrule "
					+"WHERE (ReminderCriterion=6 AND icd9.ICD9Num=CriterionFK)";//6=ICD9
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					if(listICD9Num.Contains(PIn.Long(table.Rows[i]["ICD9Num"].ToString()))) {
						continue;
					}
					listDescription.Add(PIn.String(table.Rows[i]["Description"].ToString()));
					listICD9Code.Add(PIn.String(table.Rows[i]["ICD9Code"].ToString()));
					listICD9Num.Add(PIn.Long(table.Rows[i]["ICD9Num"].ToString()));
				}
				command="SELECT MAX(ItemOrder) FROM diseasedef";
				int itemOrderCur=PIn.Int(Db.GetScalar(command));
				//for(int i=0;i<table.Rows.Count;i++) {
				//  itemOrderCur++;
				//  if(DataConnection.DBtype==DatabaseType.MySql) {
				//    command="INSERT INTO diseasedef(DiseaseName,ItemOrder,ICD9Code) VALUES('"
				//      +POut.String(table.Rows[i]["Description"].ToString())+"',"+POut.Int(itemOrderCur)+",'"+POut.String(table.Rows[i]["ICD9Code"].ToString())+"')";
				//  }
				//  else {//oracle
				//    command="INSERT INTO diseasedef(DiseaseDefNum,DiseaseName,ItemOrder,ICD9Code) VALUES((SELECT MAX(DiseaseDefNum)+1 FROM diseasedef),'"
				//      +POut.String(table.Rows[i]["Description"].ToString())+"',"+POut.Int(itemOrderCur)+",'"+POut.String(table.Rows[i]["ICD9Code"].ToString())+"')";
				//  }
				//  long defNum=Db.NonQ(command,true);
				//  command="UPDATE eduresource SET DiseaseDefNum="+POut.Long(defNum)+" WHERE ICD9Num="+table.Rows[i]["ICD9Num"].ToString();
				//  Db.NonQ(command);
				//  command="UPDATE disease SET DiseaseDefNum="+POut.Long(defNum)+" WHERE ICD9Num="+table.Rows[i]["ICD9Num"].ToString();
				//  Db.NonQ(command);
				//  command="UPDATE reminderrule SET CriterionFK="+POut.Long(defNum)+" WHERE CriterionFK="+table.Rows[i]["ICD9Num"].ToString()+" AND ReminderCriterion=6";
				//  Db.NonQ(command);
				//}
				for(int i=0;i<listICD9Num.Count;i++) {
					itemOrderCur++;
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO diseasedef(DiseaseName,ItemOrder,ICD9Code) VALUES('"
							+POut.String(listDescription[i])+"',"+POut.Int(itemOrderCur)+",'"+POut.String(listICD9Code[i])+"')";
					}
					else {//oracle
						command="INSERT INTO diseasedef(DiseaseDefNum,DiseaseName,ItemOrder,ICD9Code) VALUES((SELECT COALESCE(MAX(DiseaseDefNum),0)+1 DiseaseDefNum FROM diseasedef),'"
							+POut.String(listDescription[i])+"',"+POut.Int(itemOrderCur)+",'"+POut.String(listICD9Code[i])+"')";
					}
					long defNum=Db.NonQ(command,true,"DiseaseDefNum","diseasedef");
					command="UPDATE eduresource SET DiseaseDefNum="+POut.Long(defNum)+" WHERE ICD9Num="+POut.Long(listICD9Num[i]);
					Db.NonQ(command);
					command="UPDATE disease SET DiseaseDefNum="+POut.Long(defNum)+" WHERE ICD9Num="+POut.Long(listICD9Num[i]);
					Db.NonQ(command);
					command="UPDATE reminderrule SET CriterionFK="+POut.Long(defNum)+" WHERE CriterionFK="+POut.Long(listICD9Num[i])+" AND ReminderCriterion=6";
					Db.NonQ(command);
				}
				command="ALTER TABLE eduresource DROP COLUMN ICD9Num";
				Db.NonQ(command);
				command="ALTER TABLE disease DROP COLUMN ICD9Num";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE diseasedef ADD SnomedCode varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE diseasedef ADD SnomedCode varchar2(255)";
					Db.NonQ(command);
				}
				//Update reminderrule.ReminderCriterion - set ICD9 (6) to Problem (0)------------------------------------------------------------------------------------
				command="UPDATE reminderrule SET ReminderCriterion=0 WHERE ReminderCriterion=6";
				Db.NonQ(command);
				//Update patientrace-------------------------------------------------------------------------------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('LanguagesIndicateNone','Declined to Specify')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'LanguagesIndicateNone','Declined to Specify')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS patientrace";
					Db.NonQ(command);
					command=@"CREATE TABLE patientrace (
						PatientRaceNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						Race tinyint NOT NULL,
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE patientrace'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE patientrace (
						PatientRaceNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						Race number(3) NOT NULL,
						CONSTRAINT patientrace_PatientRaceNum PRIMARY KEY (PatientRaceNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX patientrace_PatNum ON patientrace (PatNum)";
					Db.NonQ(command);
				}
				//Create Custom Language "DeclinedToSpecify" ----------------------------------------------------------------------------------------------------------
				command="SELECT ValueString FROM preference WHERE PrefName = 'LanguagesUsedByPatients'";
				string valueString=Db.GetScalar(command);
				command="UPDATE preference SET ValueString='"+(POut.String(valueString)+",Declined to Specify").Trim(',')+"'"//trim ,(comma) off
					+" WHERE PrefName='LanguagesUsedByPatients'";
				Db.NonQ(command);
				//update Race and Ethnicity for EHR.---------------------------------------------------------------------------------------------------------------------
				//Get a list of patients that have a race set.
				command="SELECT PatNum, Race FROM patient WHERE Race!=0";
				table=Db.GetTable(command);
				string maxPkStr="1";//Used for Orcale.  Oracle has to insert the first row manually setting the PK to 1.
				for(int i=0;i<table.Rows.Count;i++) {
					string patNum=table.Rows[i]["PatNum"].ToString();
					switch(PIn.Int(table.Rows[i]["Race"].ToString())) {//PatientRaceOld
						case 0://PatientRaceOld.Unknown
							//Do nothing.  No entry means "Unknown", the old default.
							continue;
						case 1://PatientRaceOld.Multiracial
							if(DataConnection.DBtype==DatabaseType.MySql) {
								command="INSERT INTO patientrace (PatNum,Race) VALUES ("+patNum+",7)";
							}
							else {//oracle
								command="INSERT INTO patientrace (PatientRaceNum,PatNum,Race) VALUES ("+maxPkStr+","+patNum+",7)";
							}
							break;
						case 2://PatientRaceOld.HispanicLatino
							if(DataConnection.DBtype==DatabaseType.MySql) {
								command="INSERT INTO patientrace (PatNum,Race) VALUES ("+patNum+",9)";
								Db.NonQ(command);
								command="INSERT INTO patientrace (PatNum,Race) VALUES ("+patNum+",6)";
							}
							else {//oracle
								command="INSERT INTO patientrace (PatientRaceNum,PatNum,Race) VALUES ("+maxPkStr+","+patNum+",9)";
								Db.NonQ(command);
								command="INSERT INTO patientrace (PatientRaceNum,PatNum,Race) VALUES ((SELECT MAX(PatientRaceNum+1) FROM patientrace),"+patNum+",6)";
							}
							break;
						case 3://PatientRaceOld.AfricanAmerican
							if(DataConnection.DBtype==DatabaseType.MySql) {
								command="INSERT INTO patientrace (PatNum,Race) VALUES ("+patNum+",1)";
							}
							else {//oracle
								command="INSERT INTO patientrace (PatientRaceNum,PatNum,Race) VALUES ("+maxPkStr+","+patNum+",1)";
							}
							break;
						case 4://PatientRaceOld.White
							if(DataConnection.DBtype==DatabaseType.MySql) {
								command="INSERT INTO patientrace (PatNum,Race) VALUES ("+patNum+",9)";
							}
							else {//oracle
								command="INSERT INTO patientrace (PatientRaceNum,PatNum,Race) VALUES ("+maxPkStr+","+patNum+",9)";
							}
							break;
						case 5://PatientRaceOld.HawaiiOrPacIsland
							if(DataConnection.DBtype==DatabaseType.MySql) {
								command="INSERT INTO patientrace (PatNum,Race) VALUES ("+patNum+",5)";
							}
							else {//oracle
								command="INSERT INTO patientrace (PatientRaceNum,PatNum,Race) VALUES ("+maxPkStr+","+patNum+",5)";
							}
							break;
						case 6://PatientRaceOld.AmericanIndian
							if(DataConnection.DBtype==DatabaseType.MySql) {
								command="INSERT INTO patientrace (PatNum,Race) VALUES ("+patNum+",2)";
							}
							else {//oracle
								command="INSERT INTO patientrace (PatientRaceNum,PatNum,Race) VALUES ("+maxPkStr+","+patNum+",2)";
							}
							break;
						case 7://PatientRaceOld.Asian
							if(DataConnection.DBtype==DatabaseType.MySql) {
								command="INSERT INTO patientrace (PatNum,Race) VALUES ("+patNum+",3)";
							}
							else {//oracle
								command="INSERT INTO patientrace (PatientRaceNum,PatNum,Race) VALUES ("+maxPkStr+","+patNum+",3)";
							}
							break;
						case 8://PatientRaceOld.Other
							if(DataConnection.DBtype==DatabaseType.MySql) {
								command="INSERT INTO patientrace (PatNum,Race) VALUES ("+patNum+",8)";
							}
							else {//oracle
								command="INSERT INTO patientrace (PatientRaceNum,PatNum,Race) VALUES ("+maxPkStr+","+patNum+",8)";
							}
							break;
						case 9://PatientRaceOld.Aboriginal
							if(DataConnection.DBtype==DatabaseType.MySql) {
								command="INSERT INTO patientrace (PatNum,Race) VALUES ("+patNum+",0)";
							}
							else {//oracle
								command="INSERT INTO patientrace (PatientRaceNum,PatNum,Race) VALUES ("+maxPkStr+","+patNum+",0)";
							}
							break;
						case 10://PatientRaceOld.BlackHispanic
							if(DataConnection.DBtype==DatabaseType.MySql) {
								command="INSERT INTO patientrace (PatNum,Race) VALUES ("+patNum+",1)";
								Db.NonQ(command);
								command="INSERT INTO patientrace (PatNum,Race) VALUES ("+patNum+",6)";
							}
							else {//oracle
								command="INSERT INTO patientrace (PatientRaceNum,PatNum,Race) VALUES ("+maxPkStr+","+patNum+",1)";
								Db.NonQ(command);
								command="INSERT INTO patientrace (PatientRaceNum,PatNum,Race) VALUES ((SELECT MAX(PatientRaceNum+1) FROM patientrace),"+patNum+",6)";
							}
							break;
						default:
							//should never happen, useful for debugging.
							continue;
					}//end switch
					Db.NonQ(command);
					if(DataConnection.DBtype==DatabaseType.Oracle && maxPkStr=="1") {
						//At least one row has been entered.  Set the pk string to the auto-increment SQL for Oracle.
						maxPkStr="(SELECT MAX(PatientRaceNum+1) FROM patientrace)";
					}
				}
				//Apex clearinghouse.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command=@"INSERT INTO clearinghouse(Description,ExportPath,Payors,Eformat,ISA05,SenderTin,ISA07,ISA08,ISA15,Password,ResponsePath,CommBridge,ClientProgram,
						LastBatchNumber,ModemPort,LoginID,SenderName,SenderTelephone,GS03,ISA02,ISA04,ISA16,SeparatorData,SeparatorSegment) 
						VALUES ('Apex','"+POut.String(@"C:\ONETOUCH\")+"','','5','ZZ','870578776','ZZ','99999','P','','','0','',0,0,'','Apex','8008409152','99999','','','','','')";
					Db.NonQ(command);
				}
				else {//oracle
					command=@"INSERT INTO clearinghouse(ClearinghouseNum,Description,ExportPath,Payors,Eformat,ISA05,SenderTin,ISA07,ISA08,ISA15,Password,ResponsePath,CommBridge,ClientProgram,
						LastBatchNumber,ModemPort,LoginID,SenderName,SenderTelephone,GS03,ISA02,ISA04,ISA16,SeparatorData,SeparatorSegment) 
						VALUES ((SELECT MAX(ClearinghouseNum+1) FROM clearinghouse),'Apex','"+POut.String(@"C:\ONETOUCH\")+"','','5','ZZ','870578776','ZZ','99999','P','','','0','',0,0,'','Apex','8008409152','99999','','','','','')";
					Db.NonQ(command);
				}
				//Insert Guru Bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"'Guru', "
				    +"'Guru from guru.waziweb.com', "
				    +"'0', "
				    +"'',"
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'Guru image path', "
				    +"'C:\')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"'"+POut.Long(programNum)+"', "
				    +"'2', "//ToolBarsAvail.ChartModule
				    +"'Guru')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'Guru', "
				    +"'Guru from guru.waziweb.com/', "
				    +"'0', "
				    +"'',"
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'Guru image path', "
				    +"'C:\')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'2', "//ToolBarsAvail.ChartModule
				    +"'Guru')";
					Db.NonQ(command);
				}//end Guru bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('NewCropPartnerName','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'NewCropPartnerName','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE emailaddress ADD SMTPserverIncoming varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE emailaddress ADD SMTPserverIncoming varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE emailaddress ADD ServerPortIncoming int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE emailaddress ADD ServerPortIncoming number(11)";
					Db.NonQ(command);
					command="UPDATE emailaddress SET ServerPortIncoming = 0 WHERE ServerPortIncoming IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE emailaddress MODIFY ServerPortIncoming NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '13.2.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_2_2();
		}

		///<summary>Oracle compatible: 07/11/2013</summary>
		private static void To13_2_2() {
			if(FromVersion<new Version("13.2.2.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.2.2");//No translation in convert script.
				string command;
				//Convert languages in the LanguagesUsedByPatients preference from ISO639-1 to ISO639-2 for languages which are not custom.
				command="SELECT ValueString FROM preference WHERE PrefName='LanguagesUsedByPatients'";
				string strLanguageList=PIn.String(Db.GetScalar(command));
				if(strLanguageList!="") {
					StringBuilder sb=new StringBuilder();
					string[] lanstring=strLanguageList.Split(',');
					for(int i=0;i<lanstring.Length;i++) {
						if(lanstring[i]=="") {
							continue;
						}
						if(sb.Length>0) {
							sb.Append(",");
						}
						try {
							sb.Append(CultureInfo.GetCultureInfo(lanstring[i]).ThreeLetterISOLanguageName);
						}
						catch {//custom language
							sb.Append(lanstring[i]);
						}
					}
					command="UPDATE preference SET ValueString='"+POut.String(sb.ToString())+"' WHERE PrefName='LanguagesUsedByPatients'";
					Db.NonQ(command);
				}
				//Convert languages in the patient.Langauge column from ISO639-1 to ISO639-2 for languages which are not custom.
				command="SELECT PatNum,Language FROM patient WHERE Language<>'' AND Language IS NOT NULL";
				DataTable tablePatLanguages=Db.GetTable(command);
				for(int i=0;i<tablePatLanguages.Rows.Count;i++) {
					string lang=PIn.String(tablePatLanguages.Rows[i]["Language"].ToString());
					try {
						lang=CultureInfo.GetCultureInfo(lang).ThreeLetterISOLanguageName;
						long patNum=PIn.Long(tablePatLanguages.Rows[i]["PatNum"].ToString());
						command="UPDATE patient SET Language='"+POut.String(lang)+"' WHERE PatNum="+POut.Long(patNum);
						Db.NonQ(command);
					}
					catch {//Custom language
						//Do not modify.
					}
				}
				command="UPDATE preference SET ValueString = '13.2.2.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_2_4();
		}

		private static void To13_2_4() {
			if(FromVersion<new Version("13.2.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.2.4");//No translation in convert script.
				//This fixes a bug in the conversion script above at lines 324 and 328
				string command;
				command="SELECT DiseaseDefNum,DiseaseName,ICD9Code,SnomedCode FROM diseasedef ORDER BY DiseaseDefNum ASC";
				DataTable table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {//compare each row (i)
					for(int j=i+1;j<table.Rows.Count;j++) {//with every row below it
						if(PIn.String(table.Rows[i]["DiseaseName"].ToString())!=PIn.String(table.Rows[j]["DiseaseName"].ToString())
							|| PIn.String(table.Rows[i]["ICD9Code"].ToString())!=PIn.String(table.Rows[j]["ICD9Code"].ToString())
							|| PIn.String(table.Rows[i]["SnomedCode"].ToString())!=PIn.String(table.Rows[j]["SnomedCode"].ToString())) 
						{
							continue;
						}
						//row i and row j are "identical".  Because DiseaseDefNum is ascending, we want to keep row j, not row i.
						//Always use POut when entering data into the database. Jordan ok'd omitting it here for readability. Do not use this as an example.
						//The queries below will probably not make any changes.  Just if they used this part of the program heavily after the 
						command="UPDATE eduresource SET DiseaseDefNum="+table.Rows[j]["DiseaseDefNum"].ToString()+" WHERE DiseaseDefNum="+table.Rows[i]["DiseaseDefNum"].ToString();
						Db.NonQ(command);
						command="UPDATE disease SET DiseaseDefNum="+table.Rows[j]["DiseaseDefNum"].ToString()+" WHERE DiseaseDefNum="+table.Rows[i]["DiseaseDefNum"].ToString();
						Db.NonQ(command);
						command="UPDATE reminderrule SET CriterionFK="+table.Rows[j]["DiseaseDefNum"].ToString()+" WHERE CriterionFK="+table.Rows[i]["DiseaseDefNum"].ToString()+" AND ReminderCriterion=6";
						Db.NonQ(command);
						command="DELETE FROM diseasedef WHERE DiseaseDefNum="+table.Rows[i]["DiseaseDefNum"].ToString();
						Db.NonQ(command);
					}
				}
				command="UPDATE preference SET ValueString = '13.2.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_2_7();
		}

		private static void To13_2_7() {
			if(FromVersion<new Version("13.2.7.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.2.7");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patient ADD Country varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patient ADD Country varchar2(255)";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '13.2.7.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_2_16();
		}

		private static void To13_2_16() {
			if(FromVersion<new Version("13.2.16.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.2.16");//No translation in convert script.
				string command;
				//Get the 1500 claim form primary key. The unique ID is OD9.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD9' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT * FROM (SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD9') WHERE RowNum<=1";
				}
				DataTable tableClaimFormNum=Db.GetTable(command);
				if(tableClaimFormNum.Rows.Count>0) {//The claim form should exist, but might not if foreign.
					long claimFormNum=PIn.Long(Db.GetScalar(command));
					//Change the form facility address from the pay to address to the treating address.  The pay to address still shows under the billing section of the 1500 claim form.
					command="UPDATE claimformitem SET FieldName='TreatingDentistAddress' WHERE claimformnum="+POut.Long(claimFormNum)+" AND FieldName='PayToDentistAddress' AND XPos<400";
					Db.NonQ(command);
					command="UPDATE claimformitem SET FieldName='TreatingDentistCity' WHERE claimformnum="+POut.Long(claimFormNum)+" AND FieldName='PayToDentistCity' AND XPos<470";
					Db.NonQ(command);
					command="UPDATE claimformitem SET FieldName='TreatingDentistST' WHERE claimformnum="+POut.Long(claimFormNum)+" AND FieldName='PayToDentistST' AND XPos<500";
					Db.NonQ(command);
					command="UPDATE claimformitem SET FieldName='TreatingDentistZip' WHERE claimformnum="+POut.Long(claimFormNum)+" AND FieldName='PayToDentistZip' AND XPos<520";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '13.2.16.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_2_22();
		}

		private static void To13_2_22() {
			if(FromVersion<new Version("13.2.22.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.2.22");//No translation in convert script.
				string command;
				//Moving codes to the Obsolete category that were deleted in CDT 2014.
				if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
					//Move depricated codes to the Obsolete procedure code category.
					//Make sure the procedure code category exists before moving the procedure codes.
					string procCatDescript="Obsolete";
					long defNum=0;
					command="SELECT DefNum FROM definition WHERE Category=11 AND ItemName='"+POut.String(procCatDescript)+"'";//11 is DefCat.ProcCodeCats
					DataTable dtDef=Db.GetTable(command);
					if(dtDef.Rows.Count==0) { //The procedure code category does not exist, add it
						command="SELECT COUNT(*) FROM definition WHERE Category=11";//11 is DefCat.ProcCodeCats
						string itemOrder=Db.GetCount(command);
						if(DataConnection.DBtype==DatabaseType.MySql) {
							command="INSERT INTO definition (Category,ItemName,ItemOrder) "
									+"VALUES (11"+",'"+POut.String(procCatDescript)+"',"+itemOrder+")";//11 is DefCat.ProcCodeCats
						}
						else {//oracle
							command="INSERT INTO definition (DefNum,Category,ItemName,ItemOrder) "
									+"VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),11,'"+POut.String(procCatDescript)+"',"+itemOrder+")";//11 is DefCat.ProcCodeCats
						}
						defNum=Db.NonQ(command,true,"DefNum","definition");
					}
					else { //The procedure code category already exists, get the existing defnum
						defNum=PIn.Long(dtDef.Rows[0]["DefNum"].ToString());
					}
					string[] cdtCodesDeleted=new string[] {
						"D0363","D3354","D5860","D5861"
					};
					for(int i=0;i<cdtCodesDeleted.Length;i++) {
						string procCode=cdtCodesDeleted[i];
						command="SELECT CodeNum FROM procedurecode WHERE ProcCode='"+POut.String(procCode)+"'";
						DataTable dtProcCode=Db.GetTable(command);
						if(dtProcCode.Rows.Count==0) { //The procedure code does not exist in this database.
							continue;//Do not try to move it.
						}
						long codeNum=PIn.Long(dtProcCode.Rows[0]["CodeNum"].ToString());
						command="UPDATE procedurecode SET ProcCat="+POut.Long(defNum)+" WHERE CodeNum="+POut.Long(codeNum);
						Db.NonQ(command);
					}
				}//end United States update
				command="UPDATE preference SET ValueString = '13.2.22.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_2_27();
		}

		private static void To13_2_27() {
			if(FromVersion<new Version("13.2.27.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.2.27");//No translation in convert script.
				string command;
				//Insert DentalStudio Bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"'DentalStudio', "
				    +"'DentalStudio from www.villasm.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\Program Files (x86)\DentalStudioPlus\AutoStartup.exe")+"',"
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'UserName (clear to use OD username)', "
				    +"'Admin')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'UserPassword', "
				    +"'12345678')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"'"+POut.Long(programNum)+"', "
				    +"'"+POut.Int(((int)ToolBarsAvail.ChartModule))+"', "
				    +"'DentalStudio')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'DentalStudio', "
				    +"'DentalStudio from www.villasm.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\Program Files (x86)\DentalStudioPlus\AutoStartup.exe")+"',"
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'UserName (clear to use OD username)', "
				    +"'Admin')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'UserPassword', "
				    +"'12345678')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'"+POut.Int(((int)ToolBarsAvail.ChartModule))+"', "
				    +"'DentalStudio')";
					Db.NonQ(command);
				}//end DentalStudio bridge
				command="UPDATE preference SET ValueString = '13.2.27.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_3_1();
		}

		private static void To13_3_1() {
			if(FromVersion<new Version("13.3.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.3.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailInboxComputerName','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'EmailInboxComputerName','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailInboxCheckInterval','5')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'EmailInboxCheckInterval','5')";
					Db.NonQ(command);
				}
				//Add Family Health table for EHR A.13 (Family Health History)
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS familyhealth";
					Db.NonQ(command);
					command=@"CREATE TABLE familyhealth (
						FamilyHealthNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						Relationship tinyint NOT NULL,
						DiseaseDefNum bigint NOT NULL,
						PersonName varchar(255) NOT NULL,
						INDEX(PatNum),
						INDEX(DiseaseDefNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE familyhealth'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE familyhealth (
						FamilyHealthNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						Relationship number(3) NOT NULL,
						DiseaseDefNum number(20) NOT NULL,
						PersonName varchar2(255),
						CONSTRAINT familyhealth_FamilyHealthNum PRIMARY KEY (FamilyHealthNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX familyhealth_PatNum ON familyhealth (PatNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX familyhealth_DiseaseDefNum ON familyhealth (DiseaseDefNum)";
					Db.NonQ(command);
				}
				//Add securityloghash table for EHR D.2
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS securityloghash";
					Db.NonQ(command);
					command=@"CREATE TABLE securityloghash (
						SecurityLogHashNum bigint NOT NULL auto_increment PRIMARY KEY,
						SecurityLogNum bigint NOT NULL,
						LogHash varchar(255) NOT NULL,
						INDEX(SecurityLogNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE securityloghash'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE securityloghash (
						SecurityLogHashNum number(20) NOT NULL,
						SecurityLogNum number(20) NOT NULL,
						LogHash varchar2(255),
						CONSTRAINT securityloghash_SecurityLogHas PRIMARY KEY (SecurityLogHashNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX securityloghash_SecurityLogNum ON securityloghash (SecurityLogNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE emailmessage CHANGE BodyText BodyText LONGTEXT NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					//Changing a column's datatype from VARCHAR2 to clob yields the following error in oracle:  ORA-22858: invalid alteration of datatype
					//The easiest way to get change the datatype from VARCHAR2 to clob is to create a temp column then rename it.
					command="ALTER TABLE emailmessage ADD (BodyTextClob clob NOT NULL)";
					Db.NonQ(command);
					command="UPDATE emailmessage SET BodyTextClob=BodyText";
					Db.NonQ(command);
					command="ALTER TABLE emailmessage DROP COLUMN BodyText";
					Db.NonQ(command);
					command="ALTER TABLE emailmessage RENAME COLUMN BodyTextClob TO BodyText";
					Db.NonQ(command);
				}
				//Electronic Dental Services (EDS) clearinghouse.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command=@"INSERT INTO clearinghouse(Description,ExportPath,Payors,Eformat,ISA05,SenderTin,ISA07,ISA08,ISA15,Password,ResponsePath,CommBridge,
					ClientProgram,LastBatchNumber,ModemPort,LoginID,SenderName,SenderTelephone,GS03,ISA02,ISA04,ISA16,SeparatorData,SeparatorSegment) 
					VALUES ('Electronic Dental Services','"+POut.String(@"C:\EDS\Claims\In\")+"','','1','ZZ','','ZZ','EDS','P','','','0','"+POut.String(@"C:\Program Files\EDS\edsbridge.exe")+"',0,0,'','','','EDS','','','','','')";
					Db.NonQ(command);
				}
				else {//oracle
					command=@"INSERT INTO clearinghouse (ClearinghouseNum,Description,ExportPath,Payors,Eformat,ISA05,SenderTin,ISA07,ISA08,ISA15,Password,ResponsePath,CommBridge,ClientProgram,
					LastBatchNumber,ModemPort,LoginID,SenderName,SenderTelephone,GS03,ISA02,ISA04,ISA16,SeparatorData,SeparatorSegment) 
					VALUES ((SELECT MAX(ClearinghouseNum+1) FROM clearinghouse),'Electronic Dental Services','"+POut.String(@"C:\EDS\Claims\In\")+"','','1','ZZ','','ZZ','EDS','P','','','0','"+POut.String(@"C:\Program Files\EDS\edsbridge.exe")+"',0,0,'','','','EDS','','','','','')";
					Db.NonQ(command);
				}
				//codesystem
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS codesystem";
					Db.NonQ(command);
					command=@"CREATE TABLE codesystem (
						CodeSystemNum bigint NOT NULL auto_increment PRIMARY KEY,
						CodeSystemName varchar(255) NOT NULL,
						VersionCur varchar(255) NOT NULL,
						VersionAvail varchar(255) NOT NULL,
						HL7OID varchar(255) NOT NULL,
						Note varchar(255) NOT NULL,
						INDEX(CodeSystemName)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE codesystem'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE codesystem (
						CodeSystemNum number(20) NOT NULL,
						CodeSystemName varchar2(255),
						VersionCur varchar2(255),
						VersionAvail varchar2(255),
						HL7OID varchar2(255),
						Note varchar2(255),
						CONSTRAINT codesystem_CodeSystemNum PRIMARY KEY (CodeSystemNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX codesystem_CodeSystemName ON codesystem (CodeSystemName)";
					Db.NonQ(command);
				}
				//No need for mysql/oracle split
				command=@"INSERT INTO codesystem (CodeSystemNum,CodeSystemName,HL7OID,VersionAvail,VersionCur,Note) VALUES (1,'AdministrativeSex','2.16.840.1.113883.18.2','HL7v2.5','HL7v2.5','')";
				Db.NonQ(command);
				command=@"INSERT INTO codesystem (CodeSystemNum,CodeSystemName,HL7OID) VALUES (2,'CDCREC','2.16.840.1.113883.6.238')";
				Db.NonQ(command);
				command=@"INSERT INTO codesystem (CodeSystemNum,CodeSystemName,HL7OID) VALUES (3,'CDT','2.16.840.1.113883.6.13')";
				Db.NonQ(command);
				command=@"INSERT INTO codesystem (CodeSystemNum,CodeSystemName,HL7OID) VALUES (4,'CPT','2.16.840.1.113883.6.12')";
				Db.NonQ(command);
				command=@"INSERT INTO codesystem (CodeSystemNum,CodeSystemName,HL7OID) VALUES (5,'CVX','2.16.840.1.113883.12.292')";
				Db.NonQ(command);
				command=@"INSERT INTO codesystem (CodeSystemNum,CodeSystemName,HL7OID) VALUES (6,'HCPCS','2.16.840.1.113883.6.285')";
				Db.NonQ(command);
				command=@"INSERT INTO codesystem (CodeSystemNum,CodeSystemName,HL7OID) VALUES (7,'ICD10CM','2.16.840.1.113883.6.90')";
				Db.NonQ(command);
				command=@"INSERT INTO codesystem (CodeSystemNum,CodeSystemName,HL7OID) VALUES (8,'ICD9CM','2.16.840.1.113883.6.103')";
				Db.NonQ(command);
				command=@"INSERT INTO codesystem (CodeSystemNum,CodeSystemName,HL7OID) VALUES (9,'LOINC','2.16.840.1.113883.6.1')";
				Db.NonQ(command);
				command=@"INSERT INTO codesystem (CodeSystemNum,CodeSystemName,HL7OID) VALUES (10,'RXNORM','2.16.840.1.113883.6.88')";
				Db.NonQ(command);
				command=@"INSERT INTO codesystem (CodeSystemNum,CodeSystemName,HL7OID) VALUES (11,'SNOMEDCT','2.16.840.1.113883.6.96')";
				Db.NonQ(command);
				command=@"INSERT INTO codesystem (CodeSystemNum,CodeSystemName,HL7OID) VALUES (12,'SOP','2.16.840.1.113883.3.221.5')";
				Db.NonQ(command);
#region Create Code Systems Tables
				//CDCREC (CDC Race and Ethnicity)-------------------------------------------------------------------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS cdcrec";
					Db.NonQ(command);
					command=@"CREATE TABLE cdcrec (
						CdcrecNum bigint NOT NULL auto_increment PRIMARY KEY,
						CdcrecCode varchar(255) NOT NULL,
						HeirarchicalCode varchar(255) NOT NULL,
						Description varchar(255) NOT NULL,
						INDEX(CdcrecCode)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE cdcrec'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE cdcrec (
						CdcrecNum number(20) NOT NULL,
						CdcrecCode varchar2(255),
						HeirarchicalCode varchar2(255),
						Description varchar2(255),
						CONSTRAINT cdcrec_CdcrecNum PRIMARY KEY (CdcrecNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX cdcrec_CdcrecCode ON cdcrec (CdcrecCode)";
					Db.NonQ(command);
				}
				//CDT ----------------------------------------------------------------------------------------------------------------------------------------------------
				//Not neccesary, stored in ProcCode table
				//CPT (Current Procedure Terminology)---------------------------------------------------------------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS cpt";
					Db.NonQ(command);
					command=@"CREATE TABLE cpt (
						CptNum bigint NOT NULL auto_increment PRIMARY KEY,
						CptCode varchar(255) NOT NULL,
						Description varchar(4000) NOT NULL,
						INDEX(CptCode)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE cpt'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE cpt (
						CptNum number(20) NOT NULL,
						CptCode varchar2(255),
						Description varchar2(4000),
						CONSTRAINT cpt_CptNum PRIMARY KEY (CptNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX cpt_CptCode ON cpt (CptCode)";
					Db.NonQ(command);
				}
				//CVX (Vaccine Administered)------------------------------------------------------------------------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS cvx";
					Db.NonQ(command);
					command=@"CREATE TABLE cvx (
						CvxNum bigint NOT NULL auto_increment PRIMARY KEY,
						CvxCode varchar(255) NOT NULL,
						Description varchar(255) NOT NULL,
						IsActive varchar(255) NOT NULL,
						INDEX(CvxCode)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE cvx'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE cvx (
						CvxNum number(20) NOT NULL,
						CvxCode varchar2(255),
						Description varchar2(255),
						IsActive varchar2(255),
						CONSTRAINT cvx_CvxNum PRIMARY KEY (CvxNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX cvx_CvxCode ON cvx (CvxCode)";
					Db.NonQ(command);
				}
				//HCPCS (Healhtcare Common Procedure Coding System)-------------------------------------------------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS hcpcs";
					Db.NonQ(command);
					command=@"CREATE TABLE hcpcs (
						HcpcsNum bigint NOT NULL auto_increment PRIMARY KEY,
						HcpcsCode varchar(255) NOT NULL,
						DescriptionShort varchar(255) NOT NULL,
						INDEX(HcpcsCode)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE hcpcs'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE hcpcs (
						HcpcsNum number(20) NOT NULL,
						HcpcsCode varchar2(255),
						DescriptionShort varchar2(255),
						CONSTRAINT hcpcs_HcpcsNum PRIMARY KEY (HcpcsNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX hcpcs_HcpcsCode ON hcpcs (HcpcsCode)";
					Db.NonQ(command);
				}
				//ICD10CM International Classification of Diseases, 10th Revision, Clinical Modification------------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS icd10";
					Db.NonQ(command);
					command=@"CREATE TABLE icd10 (
						Icd10Num bigint NOT NULL auto_increment PRIMARY KEY,
						Icd10Code varchar(255) NOT NULL,
						Description varchar(255) NOT NULL,
						IsCode varchar(255) NOT NULL,
						INDEX(Icd10Code)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE icd10'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE icd10 (
						Icd10Num number(20) NOT NULL,
						Icd10Code varchar2(255),
						Description varchar2(255),
						IsCode varchar2(255),
						CONSTRAINT icd10_Icd10Num PRIMARY KEY (Icd10Num)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX icd10_Icd10Code ON icd10 (Icd10Code)";
					Db.NonQ(command);
				}
				//ICD9CM International Classification of Diseases, 9th Revision, Clinical Modification--------------------------------------------------------------------
				//Already Exists.
				//LOINC (Logical Observation Identifier Names and Codes)--------------------------------------------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS loinc";
					Db.NonQ(command);
					command=@"CREATE TABLE loinc (
						LoincNum bigint NOT NULL auto_increment PRIMARY KEY,
						LoincCode varchar(255) NOT NULL,
						Component varchar(255) NOT NULL,
						PropertyObserved varchar(255) NOT NULL,
						TimeAspct varchar(255) NOT NULL,
						SystemMeasured varchar(255) NOT NULL,
						ScaleType varchar(255) NOT NULL,
						MethodType varchar(255) NOT NULL,
						StatusOfCode varchar(255) NOT NULL,
						NameShort varchar(255) NOT NULL,
						ClassType varchar(255) NOT NULL,
						UnitsRequired tinyint NOT NULL,
						OrderObs varchar(255) NOT NULL,
						HL7FieldSubfieldID varchar(255) NOT NULL,
						ExternalCopyrightNotice text NOT NULL,
						NameLongCommon varchar(255) NOT NULL,
						UnitsUCUM varchar(255) NOT NULL,
						RankCommonTests int NOT NULL,
						RankCommonOrders int NOT NULL,
						INDEX(LoincCode)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE loinc'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE loinc (
						LoincNum number(20) NOT NULL,
						LoincCode varchar2(255),
						Component varchar2(255),
						PropertyObserved varchar2(255),
						TimeAspct varchar2(255),
						SystemMeasured varchar2(255),
						ScaleType varchar2(255),
						MethodType varchar2(255),
						StatusOfCode varchar2(255),
						NameShort varchar2(255),
						ClassType varchar2(255) NOT NULL,
						UnitsRequired number(3) NOT NULL,
						OrderObs varchar2(255),
						HL7FieldSubfieldID varchar2(255),
						ExternalCopyrightNotice varchar2(4000),
						NameLongCommon varchar2(255),
						UnitsUCUM varchar2(255),
						RankCommonTests number(11) NOT NULL,
						RankCommonOrders number(11) NOT NULL,
						CONSTRAINT loinc_LoincNum PRIMARY KEY (LoincNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX loinc_LoincCode ON loinc (LoincCode)";
					Db.NonQ(command);
				}
				//RXNORM--------------------------------------------------------------------------------------------------------------------------------------------------
				//Already Exists.
				//SNOMEDCT (Systematic Nomencalture of Medicine Clinical Terms)-------------------------------------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS snomed";
					Db.NonQ(command);
					command=@"CREATE TABLE snomed (
						SnomedNum bigint NOT NULL auto_increment PRIMARY KEY,
						SnomedCode varchar(255) NOT NULL,
						Description varchar(255) NOT NULL,
						INDEX(SnomedCode)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE snomed'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE snomed (
						SnomedNum number(20) NOT NULL,
						SnomedCode varchar2(255),
						Description varchar2(255),
						CONSTRAINT snomed_SnomedNum PRIMARY KEY (SnomedNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX snomed_SnomedCode ON snomed (SnomedCode)";
					Db.NonQ(command);
				}
				//SOP (Source of Payment Typology)------------------------------------------------------------------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS sop";
					Db.NonQ(command);
					command=@"CREATE TABLE sop (
						SopNum bigint NOT NULL auto_increment PRIMARY KEY,
						SopCode varchar(255) NOT NULL,
						Description varchar(255) NOT NULL,
						INDEX(SopCode)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE sop'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE sop (
						SopNum number(20) NOT NULL,
						SopCode varchar2(255),
						Description varchar2(255),
						CONSTRAINT sop_SopNum PRIMARY KEY (SopNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX sop_SopCode ON sop (SopCode)";
					Db.NonQ(command);
				}
#endregion
				//Rename emailaddress.SMTPserverIncoming to emailaddress.Pop3ServerIncoming, but leave data type alone. CRUD generator cannot write this query. See pattern for convert database.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE emailaddress CHANGE SMTPserverIncoming Pop3ServerIncoming varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE emailaddress RENAME COLUMN SMTPserverIncoming TO Pop3ServerIncoming";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE medicationpat ADD MedDescript varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE medicationpat ADD MedDescript varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE medicationpat ADD RxCui bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE medicationpat ADD INDEX (RxCui)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE medicationpat ADD RxCui number(20)";
					Db.NonQ(command);
					command="UPDATE medicationpat SET RxCui = 0 WHERE RxCui IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE medicationpat MODIFY RxCui NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX medicationpat_RxCui ON medicationpat (RxCui)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="UPDATE medicationpat,medication SET medicationpat.RxCui=medication.RxCui WHERE medicationpat.MedicationNum=medication.MedicationNum";
					Db.NonQ(command);
				}
				else {//oracle
					command="UPDATE medicationpat SET medicationpat.RxCui=(SELECT medication.RxCui FROM medication WHERE medication.MedicationNum=medicationpat.MedicationNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE medicationpat ADD NewCropGuid varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE medicationpat ADD NewCropGuid varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE medicationpat ADD IsCpoe tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE medicationpat ADD IsCpoe number(3)";
					Db.NonQ(command);
					command="UPDATE medicationpat SET IsCpoe = 0 WHERE IsCpoe IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE medicationpat MODIFY IsCpoe NOT NULL";
					Db.NonQ(command);
				}
				//oracle compatible
				command="UPDATE medicationpat SET IsCpoe=1 "
						+"WHERE PatNote!='' AND DateStart > "+POut.Date((new DateTime(1880,1,1)));
				Db.NonQ(command);
				//Add additional EHR Measures to DB
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(16,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(17,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(18,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(19,-1,-1)";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),16,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),17,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),18,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),19,-1,-1)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('MeaningfulUseTwo','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'MeaningfulUseTwo','0')";
					Db.NonQ(command);
				}
				//Time Card Overhaul for differential pay----------------------------------------------------------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clockevent ADD Rate2Hours time NOT NULL";
					Db.NonQ(command);
					command="UPDATE clockevent SET rate2hours='-01:00:00'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clockevent ADD Rate2Hours varchar2(255)";
					Db.NonQ(command);
					command="UPDATE clockevent SET rate2hours='-01:00:00'";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clockevent ADD Rate2Auto time NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clockevent ADD Rate2Auto varchar2(255)";
					Db.NonQ(command);
				}
				command="ALTER TABLE timecardrule DROP COLUMN AmtDiff";
				Db.NonQ(command);
				command="ALTER TABLE clockevent DROP COLUMN AmountBonus";
				Db.NonQ(command);
				command="ALTER TABLE clockevent DROP COLUMN AmountBonusAuto";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehramendment";
					Db.NonQ(command);
					command=@"CREATE TABLE ehramendment (
						EhrAmendmentNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						IsAccepted tinyint NOT NULL,
						Description text NOT NULL,
						Source tinyint NOT NULL,
						SourceName text NOT NULL,
						FileName varchar(255) NOT NULL,
						RawBase64 longtext NOT NULL,
						DateTRequest datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						DateTAcceptDeny datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						DateTAppend datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehramendment'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehramendment (
						EhrAmendmentNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						IsAccepted number(3) NOT NULL,
						Description varchar2(2000),
						Source number(3) NOT NULL,
						SourceName varchar2(2000),
						FileName varchar2(255),
						RawBase64 clob,
						DateTRequest date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateTAcceptDeny date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateTAppend date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT ehramendment_EhrAmendmentNum PRIMARY KEY (EhrAmendmentNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehramendment_PatNum ON ehramendment (PatNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE popup ADD UserNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE popup ADD INDEX (UserNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE popup ADD UserNum number(20)";
					Db.NonQ(command);
					command="UPDATE popup SET UserNum = 0 WHERE UserNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE popup MODIFY UserNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX popup_UserNum ON popup (UserNum)";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE popup ADD DateTimeEntry datetime NOT NULL";
					Db.NonQ(command);
					command="UPDATE popup SET DateTimeEntry='0001-01-01 00:00:00'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE popup ADD DateTimeEntry date";
					Db.NonQ(command);
					command="UPDATE popup SET DateTimeEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateTimeEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE popup MODIFY DateTimeEntry NOT NULL";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE popup ADD IsArchived tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE popup ADD IsArchived number(3)";
					Db.NonQ(command);
					command="UPDATE popup SET IsArchived = 0 WHERE IsArchived IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE popup MODIFY IsArchived NOT NULL";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE popup ADD PopupNumArchive bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE popup ADD INDEX (PopupNumArchive)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE popup ADD PopupNumArchive number(20)";
					Db.NonQ(command);
					command="UPDATE popup SET PopupNumArchive = 0 WHERE PopupNumArchive IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE popup MODIFY PopupNumArchive NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX popup_PopupNumArchive ON popup (PopupNumArchive)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patientrace ADD CdcrecCode varchar(255) NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE patientrace ADD INDEX (CdcrecCode)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patientrace ADD CdcrecCode varchar2(255)";
					Db.NonQ(command);
					command="UPDATE patientrace SET CdcrecCode = '' WHERE CdcrecCode IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patientrace MODIFY CdcrecCode NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX patientrace_CdcrecCode ON patientrace (CdcrecCode)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vitalsign ADD WeightCode varchar(255) NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE vitalsign ADD INDEX (WeightCode)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vitalsign ADD WeightCode varchar2(255)";
					Db.NonQ(command);
					command=@"CREATE INDEX vitalsign_WeightCode ON vitalsign (WeightCode)";
					Db.NonQ(command);
				}
				//Add indexes for code systems------------------------------------------------------------------------------------------------------
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE diseasedef ADD INDEX (Icd9Code)";
						Db.NonQ(command);
						command="ALTER TABLE diseasedef ADD INDEX (SnomedCode)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX diseasedef_Icd9Code ON diseasedef (Icd9Code)";
						Db.NonQ(command);
						command=@"CREATE INDEX diseasedef_SnomedCode ON diseasedef (SnomedCode)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE icd9 ADD INDEX (Icd9Code)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX icd9_Icd9Code ON icd9 (Icd9Code)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE rxnorm ADD INDEX (RxCui)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX rxnorm_RxCui ON rxnorm (RxCui)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE insplan ADD SopCode varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE insplan ADD SopCode varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS payortype";
					Db.NonQ(command);
					command=@"CREATE TABLE payortype (
						PayorTypeNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						DateStart date NOT NULL DEFAULT '0001-01-01',
						SopCode varchar(255) NOT NULL,
						Note text NOT NULL,
						INDEX(PatNum),
						INDEX(SopCode)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE payortype'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE payortype (
						PayorTypeNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						DateStart date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						SopCode varchar2(255),
						Note varchar2(2000),
						CONSTRAINT payortype_PayorTypeNum PRIMARY KEY (PayorTypeNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX payortype_PatNum ON payortype (PatNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX payortype_SopCode ON payortype (SopCode)";
					Db.NonQ(command);
				}
				//oracle compatible
				command="UPDATE patientrace SET CdcrecCode='2054-5' WHERE Race=1";//AfricanAmerican
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='1002-5' WHERE Race=2";//AmericanIndian
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='2028-9' WHERE Race=3";//Asian
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='2076-8' WHERE Race=5";//HawaiiOrPacIsland
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='2135-2' WHERE Race=6";//Hispanic
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='2131-1' WHERE Race=8";//Other
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='2106-3' WHERE Race=9";//White
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='2186-5' WHERE Race=10";//NotHispanic
				Db.NonQ(command);
				//oracle compatible
				//We will insert another patientrace row specifying 'NotHispanic' if there is not a Hispanic entry or a DeclinedToSpecify entry but there is at least one other patientrace entry.  The absence of ethnicity was assumed NotHispanic in the past, now we are going to explicitly store that value.  enum=10, CdcrecCode='2186-5'
				command="SELECT DISTINCT PatNum FROM patientrace WHERE PatNum NOT IN(SELECT PatNum FROM patientrace WHERE Race IN(4,6))";//4=DeclinedToSpecify,6=Hispanic
				DataTable table=Db.GetTable(command);
				long patNum=0;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						patNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
						command="INSERT INTO patientrace (PatNum,Race,CdcrecCode) VALUES("+POut.Long(patNum)+",10,'2186-5')";
						Db.NonQ(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						patNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
						command="INSERT INTO patientrace (PatientRaceNum,PatNum,Race,CdcrecCode) "
							+"VALUES((SELECT MAX(PatientRaceNum)+1 FROM patientrace),"+POut.Long(patNum)+",10,'2186-5')";
						Db.NonQ(command);
					}
				}
				//intervention
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS intervention";
					Db.NonQ(command);
					command=@"CREATE TABLE intervention (
						InterventionNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						ProvNum bigint NOT NULL,
						CodeValue varchar(30) NOT NULL,
						CodeSystem varchar(30) NOT NULL,
						Note text NOT NULL,
						DateTimeEntry date NOT NULL DEFAULT '0001-01-01',
						CodeSet tinyint NOT NULL,
						INDEX(PatNum),
						INDEX(ProvNum),
						INDEX(CodeValue)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE intervention'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE intervention (
						InterventionNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						ProvNum number(20) NOT NULL,
						CodeValue varchar2(30),
						CodeSystem varchar2(30),
						Note varchar2(4000),
						CodeSet number(3) NOT NULL,
						DateTimeEntry date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT intervention_InterventionNum PRIMARY KEY (InterventionNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX intervention_PatNum ON intervention (PatNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX intervention_ProvNum ON intervention (ProvNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX intervention_CodeValue ON intervention (CodeValue)";
					Db.NonQ(command);
				}
				//ehrnotperformed
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrnotperformed";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrnotperformed (
						EhrNotPerformedNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						ProvNum bigint NOT NULL,
						CodeValue varchar(30) NOT NULL,
						CodeSystem varchar(30) NOT NULL,
						CodeValueReason varchar(30) NOT NULL,
						CodeSystemReason varchar(30) NOT NULL,
						Note text NOT NULL,
						DateEntry date NOT NULL DEFAULT '0001-01-01',
						INDEX(PatNum),
						INDEX(ProvNum),
						INDEX(CodeValue),
						INDEX(CodeValueReason)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrnotperformed'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrnotperformed (
						EhrNotPerformedNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						ProvNum number(20) NOT NULL,
						CodeValue varchar2(30),
						CodeSystem varchar2(30),
						CodeValueReason varchar2(30),
						CodeSystemReason varchar2(30),
						Note varchar2(4000),
						DateEntry date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT ehrnotperformed_EhrNotPerforme PRIMARY KEY (EhrNotPerformedNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrnotperformed_PatNum ON ehrnotperformed (PatNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrnotperformed_ProvNum ON ehrnotperformed (ProvNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrnotperformed_CodeValue ON ehrnotperformed (CodeValue)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrnotperformed_CodeValueReaso ON ehrnotperformed (CodeValueReason)";
					Db.NonQ(command);
				}
				//encounter
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS encounter";
					Db.NonQ(command);
					command=@"CREATE TABLE encounter (
						EncounterNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						ProvNum bigint NOT NULL,
						CodeValue varchar(30) NOT NULL,
						CodeSystem varchar(30) NOT NULL,
						Note text NOT NULL,
						DateEncounter date NOT NULL DEFAULT '0001-01-01',
						INDEX(PatNum),
						INDEX(ProvNum),
						INDEX(CodeValue)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE encounter'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE encounter (
						EncounterNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						ProvNum number(20) NOT NULL,
						CodeValue varchar2(30),
						CodeSystem varchar2(30),
						Note varchar2(4000),
						DateEncounter date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT encounter_EncounterNum PRIMARY KEY (EncounterNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX encounter_PatNum ON encounter (PatNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX encounter_ProvNum ON encounter (ProvNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX encounter_CodeValue ON encounter (CodeValue)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('NistTimeServerUrl','nist-time-server.eoni.com')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'NistTimeServerUrl','nist-time-server.eoni.com')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrsummaryccd ADD EmailAttachNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE ehrsummaryccd ADD INDEX (EmailAttachNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ehrsummaryccd ADD EmailAttachNum number(20)";
					Db.NonQ(command);
					command="UPDATE ehrsummaryccd SET EmailAttachNum = 0 WHERE EmailAttachNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE ehrsummaryccd MODIFY EmailAttachNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrsummaryccd_EmailAttachNum ON ehrsummaryccd (EmailAttachNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vitalsign ADD HeightExamCode varchar(30) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vitalsign ADD HeightExamCode varchar2(30)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vitalsign ADD WeightExamCode varchar(30) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vitalsign ADD WeightExamCode varchar2(30)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vitalsign ADD BMIExamCode varchar(30) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vitalsign ADD BMIExamCode varchar2(30)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vitalsign ADD EhrNotPerformedNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE vitalsign ADD INDEX (EhrNotPerformedNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vitalsign ADD EhrNotPerformedNum number(20)";
					Db.NonQ(command);
					command="UPDATE vitalsign SET EhrNotPerformedNum = 0 WHERE EhrNotPerformedNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vitalsign MODIFY EhrNotPerformedNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX vitalsign_EhrNotPerformedNum ON vitalsign (EhrNotPerformedNum)";
					Db.NonQ(command);
				}
				//Add exam codes to vital sign rows currently in the db using the most generic code from each set
				command="UPDATE vitalsign SET HeightExamCode='8302-2' WHERE Height!=0";//8302-2 is "Body height"
				Db.NonQ(command);
				command="UPDATE vitalsign SET WeightExamCode='29463-7' WHERE Weight!=0";//29463-7 is "Body weight"
				Db.NonQ(command);
				command="UPDATE vitalsign SET BMIExamCode='59574-4' WHERE Height!=0 AND Weight!=0";//59574-4 is "Body mass index (BMI) [Percentile]"
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//Update over/underweight code, only 1 LOINC code for overweight and 1 for underweight
					//Based on age before the start of the measurement period, which is age before any birthday in the year of DateTaken.  Different range for 18-64 and 65+.  Under 18 not classified under/over
					command="UPDATE vitalsign,patient SET WeightCode='238131007'/*Overweight*/ "
						+"WHERE patient.PatNum=vitalsign.PatNum AND Height!=0 AND Weight!=0 "
						+"AND Birthdate>'1880-01-01' AND ("
						+"(YEAR(DateTaken)-YEAR(Birthdate)-1>=65 AND (Weight*703)/(Height*Height)>=30) "
						+"OR "
						+"(YEAR(DateTaken)-YEAR(Birthdate)-1 BETWEEN 18 AND 64 AND (Weight*703)/(Height*Height)>=25))";
					Db.NonQ(command);
					command="UPDATE vitalsign,patient	SET WeightCode='248342006'/*Underweight*/ "
						+"WHERE patient.PatNum=vitalsign.PatNum	AND Height!=0 AND Weight!=0 "
						+"AND Birthdate>'1880-01-01' AND ("
						+"(YEAR(DateTaken)-YEAR(patient.Birthdate)-1>=65 AND (Weight*703)/(Height*Height)<23) "
						+"OR "
						+"(YEAR(DateTaken)-YEAR(patient.Birthdate)-1 BETWEEN 18 AND 64 AND (Weight*703)/(Height*Height)<18.5))";
					Db.NonQ(command);
				}
				else {//oracle
					//EHR not oracle compatible so the vital sign WeightCode will not be used, only for ehr reporting
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vitalsign ADD PregDiseaseNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE vitalsign ADD INDEX (PregDiseaseNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vitalsign ADD PregDiseaseNum number(20)";
					Db.NonQ(command);
					command="UPDATE vitalsign SET PregDiseaseNum = 0 WHERE PregDiseaseNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vitalsign MODIFY PregDiseaseNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX vitalsign_PregDiseaseNum ON vitalsign (PregDiseaseNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('CQMDefaultEncounterCodeValue','none')";//we cannot preset this to a SNOMEDCT code since the customer may not be US
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'CQMDefaultEncounterCodeValue','none')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('CQMDefaultEncounterCodeSystem','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'CQMDefaultEncounterCodeSystem','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PregnancyDefaultCodeValue','none')";//we cannot preset this to a SNOMEDCT code since the customer may not be US
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PregnancyDefaultCodeValue','none')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PregnancyDefaultCodeSystem','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PregnancyDefaultCodeSystem','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE diseasedef ADD Icd10Code varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE diseasedef ADD Icd10Code varchar2(255)";
					Db.NonQ(command);
				}
				//Add indexes for code systems------------------------------------------------------------------------------------------------------
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE diseasedef ADD INDEX (Icd10Code)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX diseasedef_Icd10Code ON diseasedef (Icd10Code)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				//Add indexes to speed up payroll------------------------------------------------------------------------------------------------------
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE clockevent ADD INDEX (TimeDisplayed1)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX clockevent_TimeDisplayed1 ON clockevent (TimeDisplayed1)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName) VALUES('ADPCompanyCode')";//No default value.
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ADPCompanyCode')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE employee ADD PayrollID varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE employee ADD PayrollID varchar2(255)";
					Db.NonQ(command);
				}
				//Add indexes to speed up customer management window------------------------------------------------------------------------------------
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE registrationkey ADD INDEX (PatNum)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX registrationkey_PatNum ON registrationkey (PatNum)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE repeatcharge ADD INDEX (PatNum)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX repeatcharge_PatNum ON repeatcharge (PatNum)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE allergydef CHANGE Snomed SnomedType tinyint";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE allergydef RENAME COLUMN Snomed TO SnomedType";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE allergydef ADD SnomedAllergyTo varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE allergydef ADD SnomedAllergyTo varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE allergy ADD SnomedReaction varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE allergy ADD SnomedReaction varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE dunning ADD EmailSubject varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE dunning ADD EmailSubject varchar2(255)";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE dunning ADD EmailBody mediumtext NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE dunning ADD EmailBody clob";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE statement ADD EmailSubject varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE statement ADD EmailSubject varchar2(255)";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE statement ADD EmailBody mediumtext NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE statement ADD EmailBody clob";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS maparea";
					Db.NonQ(command);
					command=@"CREATE TABLE maparea (
						MapAreaNum bigint NOT NULL auto_increment PRIMARY KEY,
						Extension int NOT NULL,
						XPos double NOT NULL,
						YPos double NOT NULL,
						Width double NOT NULL,
						Height double NOT NULL,
						Description varchar(255) NOT NULL,
						ItemType tinyint NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE maparea'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE maparea (
						MapAreaNum number(20) NOT NULL,
						Extension number(11) NOT NULL,
						XPos number(38,8) NOT NULL,
						YPos number(38,8) NOT NULL,
						Width number(38,8) NOT NULL,
						Height number(38,8) NOT NULL,
						Description varchar2(255),
						ItemType number(3) NOT NULL,
						CONSTRAINT maparea_MapAreaNum PRIMARY KEY (MapAreaNum)
						)";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '13.3.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_3_4();
		}

		private static void To13_3_4() {
			if(FromVersion<new Version("13.3.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.3.4");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE emailmessage ADD RecipientAddress varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE emailmessage ADD RecipientAddress varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS emailmessageuid";
					Db.NonQ(command);
					command=@"CREATE TABLE emailmessageuid (
						EmailMessageUidNum bigint NOT NULL auto_increment PRIMARY KEY,
						Uid text NOT NULL,
						RecipientAddress varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE emailmessageuid'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE emailmessageuid (
						EmailMessageUidNum number(20) NOT NULL,
						""Uid"" varchar2(4000),
						RecipientAddress varchar2(255),
						CONSTRAINT emailmessageuid_EmailMessageUi PRIMARY KEY (EmailMessageUidNum)
						)";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '13.3.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_3_5();
		}

		private static void To13_3_5() {
			if(FromVersion<new Version("13.3.5.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.3.5");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE erxlog ADD ProvNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE erxlog ADD INDEX (ProvNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE erxlog ADD ProvNum number(20)";
					Db.NonQ(command);
					command="UPDATE erxlog SET ProvNum = 0 WHERE ProvNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE erxlog MODIFY ProvNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX erxlog_ProvNum ON erxlog (ProvNum)";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '13.3.5.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_3_6();
		}

		///<summary>Oracle compatible: 12/26/2013</summary>
		private static void To13_3_6() {
			if(FromVersion<new Version("13.3.6.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.3.6");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE emailmessage ADD RawEmailIn longtext NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE emailmessage ADD RawEmailIn clob";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '13.3.6.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To13_3_7();
		}

		private static void To13_3_7() {
			if(FromVersion<new Version("13.3.7.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 13.3.7");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE emailmessageuid CHANGE Uid MsgId text";
					Db.NonQ(command);
				}
				else {//oracle
					command=@"ALTER TABLE emailmessageuid RENAME COLUMN ""Uid"" TO MsgId";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '13.3.7.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_1_1();
		}

		private static void To14_1_1() {
			if(FromVersion<new Version("14.1.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.1.1");//No translation in convert script.
				string command;
				//Added permission EhrShowCDS.     No one has this permission by default.  This is more like a user level preference than a permission.
				//Added permission EhrInfoButton.  No one has this permission by default.  This is more like a user level preference than a permission.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrtrigger";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrtrigger (
						EhrTriggerNum bigint NOT NULL auto_increment PRIMARY KEY,
						Description varchar(255) NOT NULL,
						ProblemSnomedList text NOT NULL,
						ProblemIcd9List text NOT NULL,
						ProblemIcd10List text NOT NULL,
						ProblemDefNumList text NOT NULL,
						MedicationNumList text NOT NULL,
						RxCuiList text NOT NULL,
						CvxList text NOT NULL,
						AllergyDefNumList text NOT NULL,
						DemographicsList text NOT NULL,
						LabLoincList text NOT NULL,
						VitalLoincList text NOT NULL,
						Instructions text NOT NULL,
						Bibliography text NOT NULL,
						Cardinality tinyint NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrtrigger'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrtrigger (
						EhrTriggerNum number(20) NOT NULL,
						Description varchar2(255),
						ProblemSnomedList varchar2(4000),
						ProblemIcd9List varchar2(4000),
						ProblemIcd10List varchar2(4000),
						ProblemDefNumList varchar2(4000),
						MedicationNumList varchar2(4000),
						RxCuiList varchar2(4000),
						CvxList varchar2(4000),
						AllergyDefNumList varchar2(4000),
						DemographicsList varchar2(4000),
						LabLoincList varchar2(4000),
						VitalLoincList varchar2(4000),
						Instructions varchar2(4000),
						Bibliography varchar2(4000),
						Cardinality number(3) NOT NULL,
						CONSTRAINT ehrtrigger_EhrTriggerNum PRIMARY KEY (EhrTriggerNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE provider ADD EmailAddressNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE provider ADD INDEX (EmailAddressNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE provider ADD EmailAddressNum number(20)";
					Db.NonQ(command);
					command="UPDATE provider SET EmailAddressNum = 0 WHERE EmailAddressNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE provider MODIFY EmailAddressNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX provider_EmailAddressNum ON provider (EmailAddressNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrmeasureevent ADD CodeValueEvent varchar(30) NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE ehrmeasureevent ADD INDEX (CodeValueEvent)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ehrmeasureevent ADD CodeValueEvent varchar2(30)";
					Db.NonQ(command);
					command="CREATE INDEX ehrmeasureevent_CodeValueEvent ON ehrmeasureevent (CodeValueEvent)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrmeasureevent ADD CodeSystemEvent varchar(30) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ehrmeasureevent ADD CodeSystemEvent varchar2(30)";
					Db.NonQ(command);
				}
				//oracle compatible
				command="UPDATE ehrmeasureevent SET CodeValueEvent='11366-2' WHERE EventType=8";//Set all TobaccoUseAssessed ehrmeasureevents to code for 'History of tobacco use Narrative'
				Db.NonQ(command);
				command="UPDATE ehrmeasureevent SET CodeSystemEvent='LOINC' WHERE EventType=8";//All TobaccoUseAssessed codes are LOINC codes
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrmeasureevent ADD CodeValueResult varchar(30) NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE ehrmeasureevent ADD INDEX (CodeValueResult)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ehrmeasureevent ADD CodeValueResult varchar2(30)";
					Db.NonQ(command);
					command="CREATE INDEX ehrmeasureevent_CodeValueResul ON ehrmeasureevent (CodeValueResult)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrmeasureevent ADD CodeSystemResult varchar(30) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ehrmeasureevent ADD CodeSystemResult varchar2(30)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE intervention CHANGE DateTimeEntry DateEntry date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE intervention RENAME COLUMN DateTimeEntry TO DateEntry";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrsummaryccd MODIFY ContentSummary longtext NOT NULL";
					Db.NonQ(command);
				}
				//oracle ContentSummary data type is already clob which can handle up to 4GB of data.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE disease ADD SnomedProblemType varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE disease ADD SnomedProblemType varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE disease ADD FunctionStatus tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE disease ADD FunctionStatus number(3)";
					Db.NonQ(command);
					command="UPDATE disease SET FunctionStatus = 0 WHERE FunctionStatus IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE disease MODIFY FunctionStatus NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrcareplan";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrcareplan (
						EhrCarePlanNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						SnomedEducation varchar(255) NOT NULL,
						Instructions varchar(255) NOT NULL,
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrcareplan'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrcareplan (
						EhrCarePlanNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						SnomedEducation varchar2(255),
						Instructions varchar2(255),
						CONSTRAINT ehrcareplan_EhrCarePlanNum PRIMARY KEY (EhrCarePlanNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrcareplan_PatNum ON ehrcareplan (PatNum)";
					Db.NonQ(command);
				}
				//Add UCUM table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ucum";
					Db.NonQ(command);
					command=@"CREATE TABLE ucum (
						UcumNum bigint NOT NULL auto_increment PRIMARY KEY,
						UcumCode varchar(255) NOT NULL,
						Description varchar(255) NOT NULL,
						IsInUse tinyint NOT NULL,
						INDEX(UcumCode)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ucum'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ucum (
						UcumNum number(20) NOT NULL,
						UcumCode varchar2(255),
						Description varchar2(255),
						IsInUse number(3) NOT NULL,
						CONSTRAINT ucum_UcumNum PRIMARY KEY (UcumNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ucum_UcumCode ON ucum (UcumCode)";
					Db.NonQ(command);
				}
				//Add UCUM to Code System Importer
				command=@"INSERT INTO codesystem (CodeSystemNum,CodeSystemName,HL7OID) VALUES (13,'UCUM','2.16.840.1.113883.6.8')";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrcareplan ADD DatePlanned date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ehrcareplan ADD DatePlanned date";
					Db.NonQ(command);
					command="UPDATE ehrcareplan SET DatePlanned = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DatePlanned IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE ehrcareplan MODIFY DatePlanned NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PatientPortalNotifyBody','Please go to this link and login using your credentials. [URL]')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PatientPortalNotifyBody','Please go to this link and login using your credentials. [URL]')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PatientPortalNotifySubject','You have a secure message waiting for you')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PatientPortalNotifySubject','You have a secure message waiting for you')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE emailmessage ADD ProvNumWebMail bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE emailmessage ADD INDEX (ProvNumWebMail)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE emailmessage ADD ProvNumWebMail number(20)";
					Db.NonQ(command);
					command="UPDATE emailmessage SET ProvNumWebMail = 0 WHERE ProvNumWebMail IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE emailmessage MODIFY ProvNumWebMail NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX emailmessage_ProvNumWebMail ON emailmessage (ProvNumWebMail)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE emailmessage ADD PatNumSubj bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE emailmessage ADD INDEX (PatNumSubj)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE emailmessage ADD PatNumSubj number(20)";
					Db.NonQ(command);
					command="UPDATE emailmessage SET PatNumSubj = 0 WHERE PatNumSubj IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE emailmessage MODIFY PatNumSubj NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX emailmessage_PatNumSubj ON emailmessage (PatNumSubj)";
					Db.NonQ(command);
				}
				//Added Table cdspermission
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS cdspermission";
					Db.NonQ(command);
					command=@"CREATE TABLE cdspermission (
						CDSPermissionNum bigint NOT NULL auto_increment PRIMARY KEY,
						UserNum bigint NOT NULL,
						SetupCDS tinyint NOT NULL,
						ShowCDS tinyint NOT NULL,
						ShowInfobutton tinyint NOT NULL,
						EditBibliography tinyint NOT NULL,
						ProblemCDS tinyint NOT NULL,
						MedicationCDS tinyint NOT NULL,
						AllergyCDS tinyint NOT NULL,
						DemographicCDS tinyint NOT NULL,
						LabTestCDS tinyint NOT NULL,
						VitalCDS tinyint NOT NULL,
						INDEX(UserNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE cdspermission'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE cdspermission (
						CDSPermissionNum number(20) NOT NULL,
						UserNum number(20) NOT NULL,
						SetupCDS number(3) NOT NULL,
						ShowCDS number(3) NOT NULL,
						ShowInfobutton number(3) NOT NULL,
						EditBibliography number(3) NOT NULL,
						ProblemCDS number(3) NOT NULL,
						MedicationCDS number(3) NOT NULL,
						AllergyCDS number(3) NOT NULL,
						DemographicCDS number(3) NOT NULL,
						LabTestCDS number(3) NOT NULL,
						VitalCDS number(3) NOT NULL,
						CONSTRAINT cdspermission_CDSPermissionNum PRIMARY KEY (CDSPermissionNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX cdspermission_UserNum ON cdspermission (UserNum)";
					Db.NonQ(command);
				}
				#region EHR Lab framework (never going to be used)
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrlab";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlab (
						EhrLabNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						EhrLabMessageNum bigint NOT NULL,
						OrderControlCode varchar(255) NOT NULL,
						PlacerOrderNum varchar(255) NOT NULL,
						PlacerOrderNamespace varchar(255) NOT NULL,
						PlacerOrderUniversalID varchar(255) NOT NULL,
						PlacerOrderUniversalIDType varchar(255) NOT NULL,
						FillerOrderNum varchar(255) NOT NULL,
						FillerOrderNamespace varchar(255) NOT NULL,
						FillerOrderUniversalID varchar(255) NOT NULL,
						FillerOrderUniversalIDType varchar(255) NOT NULL,
						PlacerGroupNum varchar(255) NOT NULL,
						PlacerGroupNamespace varchar(255) NOT NULL,
						PlacerGroupUniversalID varchar(255) NOT NULL,
						PlacerGroupUniversalIDType varchar(255) NOT NULL,
						OrderingProviderID varchar(255) NOT NULL,
						OrderingProviderLName varchar(255) NOT NULL,
						OrderingProviderFName varchar(255) NOT NULL,
						OrderingProviderMiddleNames varchar(255) NOT NULL,
						OrderingProviderSuffix varchar(255) NOT NULL,
						OrderingProviderPrefix varchar(255) NOT NULL,
						OrderingProviderAssigningAuthorityNamespaceID varchar(255) NOT NULL,
						OrderingProviderAssigningAuthorityUniversalID varchar(255) NOT NULL,
						OrderingProviderAssigningAuthorityIDType varchar(255) NOT NULL,
						OrderingProviderNameTypeCode varchar(255) NOT NULL,
						OrderingProviderIdentifierTypeCode varchar(255) NOT NULL,
						SetIdOBR bigint NOT NULL,
						UsiID varchar(255) NOT NULL,
						UsiText varchar(255) NOT NULL,
						UsiCodeSystemName varchar(255) NOT NULL,
						UsiIDAlt varchar(255) NOT NULL,
						UsiTextAlt varchar(255) NOT NULL,
						UsiCodeSystemNameAlt varchar(255) NOT NULL,
						UsiTextOriginal varchar(255) NOT NULL,
						ObservationDateTimeStart varchar(255) NOT NULL,
						ObservationDateTimeEnd varchar(255) NOT NULL,
						SpecimenActionCode varchar(255) NOT NULL,
						ResultDateTime varchar(255) NOT NULL,
						ResultStatus varchar(255) NOT NULL,
						ParentObservationID varchar(255) NOT NULL,
						ParentObservationText varchar(255) NOT NULL,
						ParentObservationCodeSystemName varchar(255) NOT NULL,
						ParentObservationIDAlt varchar(255) NOT NULL,
						ParentObservationTextAlt varchar(255) NOT NULL,
						ParentObservationCodeSystemNameAlt varchar(255) NOT NULL,
						ParentObservationTextOriginal varchar(255) NOT NULL,
						ParentObservationSubID varchar(255) NOT NULL,
						ParentPlacerOrderNum varchar(255) NOT NULL,
						ParentPlacerOrderNamespace varchar(255) NOT NULL,
						ParentPlacerOrderUniversalID varchar(255) NOT NULL,
						ParentPlacerOrderUniversalIDType varchar(255) NOT NULL,
						ParentFillerOrderNum varchar(255) NOT NULL,
						ParentFillerOrderNamespace varchar(255) NOT NULL,
						ParentFillerOrderUniversalID varchar(255) NOT NULL,
						ParentFillerOrderUniversalIDType varchar(255) NOT NULL,
						ListEhrLabResultsHandlingF tinyint NOT NULL,
						ListEhrLabResultsHandlingN tinyint NOT NULL,
						TQ1SetId bigint NOT NULL,
						TQ1DateTimeStart varchar(255) NOT NULL,
						TQ1DateTimeEnd varchar(255) NOT NULL,
						INDEX(PatNum),
						INDEX(EhrLabMessageNum),
						INDEX(SetIdOBR),
						INDEX(TQ1SetId)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					//EHR lab features have been hidden for Oracle users due to some column names exceeding the 30 character limit.
					//TODO: Fix the column names to be less than 30 characters in the future.
					//	This will require unhiding EHR lab features for Oracle users, altering the table types and columns in the both MySQL and Oracle.
					/*
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrlab'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlab (
						EhrLabNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						EhrLabMessageNum number(20) NOT NULL,
						OrderControlCode varchar2(255),
						PlacerOrderNum varchar2(255),
						PlacerOrderNamespace varchar2(255),
						PlacerOrderUniversalID varchar2(255),
						PlacerOrderUniversalIDType varchar2(255),
						FillerOrderNum varchar2(255),
						FillerOrderNamespace varchar2(255),
						FillerOrderUniversalID varchar2(255),
						FillerOrderUniversalIDType varchar2(255),
						PlacerGroupNum varchar2(255),
						PlacerGroupNamespace varchar2(255),
						PlacerGroupUniversalID varchar2(255),
						PlacerGroupUniversalIDType varchar2(255),
						OrderingProviderID varchar2(255),
						OrderingProviderLName varchar2(255),
						OrderingProviderFName varchar2(255),
						OrderingProviderMiddleNames varchar2(255),
						OrderingProviderSuffix varchar2(255),
						OrderingProviderPrefix varchar2(255),
						OrderingProviderAssigningAuthorityNamespaceID varchar2(255),
						OrderingProviderAssigningAuthorityUniversalID varchar2(255),
						OrderingProviderAssigningAuthorityIDType varchar2(255),
						OrderingProviderNameTypeCode varchar2(255),
						OrderingProviderIdentifierTypeCode varchar2(255),
						SetIdOBR number(20) NOT NULL,
						UsiID varchar2(255),
						UsiText varchar2(255),
						UsiCodeSystemName varchar2(255),
						UsiIDAlt varchar2(255),
						UsiTextAlt varchar2(255),
						UsiCodeSystemNameAlt varchar2(255),
						UsiTextOriginal varchar2(255),
						ObservationDateTimeStart varchar2(255),
						ObservationDateTimeEnd varchar2(255),
						SpecimenActionCode varchar2(255),
						ResultDateTime varchar2(255),
						ResultStatus varchar2(255),
						ParentObservationID varchar2(255),
						ParentObservationText varchar2(255),
						ParentObservationCodeSystemName varchar2(255),
						ParentObservationIDAlt varchar2(255),
						ParentObservationTextAlt varchar2(255),
						ParentObservationCodeSystemNameAlt varchar2(255),
						ParentObservationTextOriginal varchar2(255),
						ParentObservationSubID varchar2(255),
						ParentPlacerOrderNum varchar2(255),
						ParentPlacerOrderNamespace varchar2(255),
						ParentPlacerOrderUniversalID varchar2(255),
						ParentPlacerOrderUniversalIDType varchar2(255),
						ParentFillerOrderNum varchar2(255),
						ParentFillerOrderNamespace varchar2(255),
						ParentFillerOrderUniversalID varchar2(255),
						ParentFillerOrderUniversalIDType varchar2(255),
						ListEhrLabResultsHandlingF number(3) NOT NULL,
						ListEhrLabResultsHandlingN number(3) NOT NULL,
						TQ1SetId number(20) NOT NULL,
						TQ1DateTimeStart varchar2(255),
						TQ1DateTimeEnd varchar2(255),
						CONSTRAINT ehrlab_EhrLabNum PRIMARY KEY (EhrLabNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlab_PatNum ON ehrlab (PatNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlab_EhrLabMessageNum ON ehrlab (EhrLabMessageNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlab_SetIdOBR ON ehrlab (SetIdOBR)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlab_TQ1SetId ON ehrlab (TQ1SetId)";
					Db.NonQ(command);
					 */
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrlabresult";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabresult (
						EhrLabResultNum bigint NOT NULL auto_increment PRIMARY KEY,
						EhrLabNum bigint NOT NULL,
						SetIdOBX bigint NOT NULL,
						ValueType varchar(255) NOT NULL,
						ObservationIdentifierID varchar(255) NOT NULL,
						ObservationIdentifierText varchar(255) NOT NULL,
						ObservationIdentifierCodeSystemName varchar(255) NOT NULL,
						ObservationIdentifierIDAlt varchar(255) NOT NULL,
						ObservationIdentifierTextAlt varchar(255) NOT NULL,
						ObservationIdentifierCodeSystemNameAlt varchar(255) NOT NULL,
						ObservationIdentifierTextOriginal varchar(255) NOT NULL,
						ObservationIdentifierSub varchar(255) NOT NULL,
						ObservationValueCodedElementID varchar(255) NOT NULL,
						ObservationValueCodedElementText varchar(255) NOT NULL,
						ObservationValueCodedElementCodeSystemName varchar(255) NOT NULL,
						ObservationValueCodedElementIDAlt varchar(255) NOT NULL,
						ObservationValueCodedElementTextAlt varchar(255) NOT NULL,
						ObservationValueCodedElementCodeSystemNameAlt varchar(255) NOT NULL,
						ObservationValueCodedElementTextOriginal varchar(255) NOT NULL,
						ObservationValueDateTime varchar(255) NOT NULL,
						ObservationValueTime time NOT NULL DEFAULT '00:00:00',
						ObservationValueComparator varchar(255) NOT NULL,
						ObservationValueNumber1 double NOT NULL,
						ObservationValueSeparatorOrSuffix varchar(255) NOT NULL,
						ObservationValueNumber2 double NOT NULL,
						ObservationValueNumeric double NOT NULL,
						ObservationValueText varchar(255) NOT NULL,
						UnitsID varchar(255) NOT NULL,
						UnitsText varchar(255) NOT NULL,
						UnitsCodeSystemName varchar(255) NOT NULL,
						UnitsIDAlt varchar(255) NOT NULL,
						UnitsTextAlt varchar(255) NOT NULL,
						UnitsCodeSystemNameAlt varchar(255) NOT NULL,
						UnitsTextOriginal varchar(255) NOT NULL,
						referenceRange varchar(255) NOT NULL,
						AbnormalFlags varchar(255) NOT NULL,
						ObservationResultStatus varchar(255) NOT NULL,
						ObservationDateTime varchar(255) NOT NULL,
						AnalysisDateTime varchar(255) NOT NULL,
						PerformingOrganizationName varchar(255) NOT NULL,
						PerformingOrganizationNameAssigningAuthorityNamespaceId varchar(255) NOT NULL,
						PerformingOrganizationNameAssigningAuthorityUniversalId varchar(255) NOT NULL,
						PerformingOrganizationNameAssigningAuthorityUniversalIdType varchar(255) NOT NULL,
						PerformingOrganizationIdentifierTypeCode varchar(255) NOT NULL,
						PerformingOrganizationIdentifier varchar(255) NOT NULL,
						PerformingOrganizationAddressStreet varchar(255) NOT NULL,
						PerformingOrganizationAddressOtherDesignation varchar(255) NOT NULL,
						PerformingOrganizationAddressCity varchar(255) NOT NULL,
						PerformingOrganizationAddressStateOrProvince varchar(255) NOT NULL,
						PerformingOrganizationAddressZipOrPostalCode varchar(255) NOT NULL,
						PerformingOrganizationAddressCountryCode varchar(255) NOT NULL,
						PerformingOrganizationAddressAddressType varchar(255) NOT NULL,
						PerformingOrganizationAddressCountyOrParishCode varchar(255) NOT NULL,
						MedicalDirectorID varchar(255) NOT NULL,
						MedicalDirectorLName varchar(255) NOT NULL,
						MedicalDirectorFName varchar(255) NOT NULL,
						MedicalDirectorMiddleNames varchar(255) NOT NULL,
						MedicalDirectorSuffix varchar(255) NOT NULL,
						MedicalDirectorPrefix varchar(255) NOT NULL,
						MedicalDirectorAssigningAuthorityNamespaceID varchar(255) NOT NULL,
						MedicalDirectorAssigningAuthorityUniversalID varchar(255) NOT NULL,
						MedicalDirectorAssigningAuthorityIDType varchar(255) NOT NULL,
						MedicalDirectorNameTypeCode varchar(255) NOT NULL,
						MedicalDirectorIdentifierTypeCode varchar(255) NOT NULL,
						INDEX(EhrLabNum),
						INDEX(SetIdOBX)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					//EHR lab features have been hidden for Oracle users due to some column names exceeding the 30 character limit.
					//TODO: Fix the column names to be less than 30 characters in the future.
					//	This will require unhiding EHR lab features for Oracle users, altering the table types and columns in the both MySQL and Oracle.
					/*
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrlabresult'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabresult (
						EhrLabResultNum number(20) NOT NULL,
						EhrLabNum number(20) NOT NULL,
						SetIdOBX number(20) NOT NULL,
						ValueType varchar2(255),
						ObservationIdentifierID varchar2(255),
						ObservationIdentifierText varchar2(255),
						ObservationIdentifierCodeSystemName varchar2(255),
						ObservationIdentifierIDAlt varchar2(255),
						ObservationIdentifierTextAlt varchar2(255),
						ObservationIdentifierCodeSystemNameAlt varchar2(255),
						ObservationIdentifierTextOriginal varchar2(255),
						ObservationIdentifierSub varchar2(255),
						ObservationValueCodedElementID varchar2(255),
						ObservationValueCodedElementText varchar2(255),
						ObservationValueCodedElementCodeSystemName varchar2(255),
						ObservationValueCodedElementIDAlt varchar2(255),
						ObservationValueCodedElementTextAlt varchar2(255),
						ObservationValueCodedElementCodeSystemNameAlt varchar2(255),
						ObservationValueCodedElementTextOriginal varchar2(255),
						ObservationValueDateTime varchar2(255),
						ObservationValueTime date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						ObservationValueComparator varchar2(255),
						ObservationValueNumber1 number(38,8) NOT NULL,
						ObservationValueSeparatorOrSuffix varchar2(255),
						ObservationValueNumber2 number(38,8) NOT NULL,
						ObservationValueNumeric number(38,8) NOT NULL,
						ObservationValueText varchar2(255),
						UnitsID varchar2(255),
						UnitsText varchar2(255),
						UnitsCodeSystemName varchar2(255),
						UnitsIDAlt varchar2(255),
						UnitsTextAlt varchar2(255),
						UnitsCodeSystemNameAlt varchar2(255),
						UnitsTextOriginal varchar2(255),
						referenceRange varchar2(255),
						AbnormalFlags varchar2(255),
						ObservationResultStatus varchar2(255),
						ObservationDateTime varchar2(255),
						AnalysisDateTime varchar2(255),
						PerformingOrganizationName varchar2(255),
						PerformingOrganizationNameAssigningAuthorityNamespaceId varchar2(255),
						PerformingOrganizationNameAssigningAuthorityUniversalId varchar2(255),
						PerformingOrganizationNameAssigningAuthorityUniversalIdType varchar2(255),
						PerformingOrganizationIdentifierTypeCode varchar2(255),
						PerformingOrganizationIdentifier varchar2(255),
						PerformingOrganizationAddressStreet varchar2(255),
						PerformingOrganizationAddressOtherDesignation varchar2(255),
						PerformingOrganizationAddressCity varchar2(255),
						PerformingOrganizationAddressStateOrProvince varchar2(255),
						PerformingOrganizationAddressZipOrPostalCode varchar2(255),
						PerformingOrganizationAddressCountryCode varchar2(255),
						PerformingOrganizationAddressAddressType varchar2(255),
						PerformingOrganizationAddressCountyOrParishCode varchar2(255),
						MedicalDirectorID varchar2(255),
						MedicalDirectorLName varchar2(255),
						MedicalDirectorFName varchar2(255),
						MedicalDirectorMiddleNames varchar2(255),
						MedicalDirectorSuffix varchar2(255),
						MedicalDirectorPrefix varchar2(255),
						MedicalDirectorAssigningAuthorityNamespaceID varchar2(255),
						MedicalDirectorAssigningAuthorityUniversalID varchar2(255),
						MedicalDirectorAssigningAuthorityIDType varchar2(255),
						MedicalDirectorNameTypeCode varchar2(255),
						MedicalDirectorIdentifierTypeCode varchar2(255),
						CONSTRAINT ehrlabresult_EhrLabResultNum PRIMARY KEY (EhrLabResultNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlabresult_EhrLabNum ON ehrlabresult (EhrLabNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlabresult_SetIdOBX ON ehrlabresult (SetIdOBX)";
					Db.NonQ(command);
					 */
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrlab";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlab (
						EhrLabNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						OrderControlCode varchar(255) NOT NULL,
						PlacerOrderNum varchar(255) NOT NULL,
						PlacerOrderNamespace varchar(255) NOT NULL,
						PlacerOrderUniversalID varchar(255) NOT NULL,
						PlacerOrderUniversalIDType varchar(255) NOT NULL,
						FillerOrderNum varchar(255) NOT NULL,
						FillerOrderNamespace varchar(255) NOT NULL,
						FillerOrderUniversalID varchar(255) NOT NULL,
						FillerOrderUniversalIDType varchar(255) NOT NULL,
						PlacerGroupNum varchar(255) NOT NULL,
						PlacerGroupNamespace varchar(255) NOT NULL,
						PlacerGroupUniversalID varchar(255) NOT NULL,
						PlacerGroupUniversalIDType varchar(255) NOT NULL,
						OrderingProviderID varchar(255) NOT NULL,
						OrderingProviderLName varchar(255) NOT NULL,
						OrderingProviderFName varchar(255) NOT NULL,
						OrderingProviderMiddleNames varchar(255) NOT NULL,
						OrderingProviderSuffix varchar(255) NOT NULL,
						OrderingProviderPrefix varchar(255) NOT NULL,
						OrderingProviderAssigningAuthorityNamespaceID varchar(255) NOT NULL,
						OrderingProviderAssigningAuthorityUniversalID varchar(255) NOT NULL,
						OrderingProviderAssigningAuthorityIDType varchar(255) NOT NULL,
						OrderingProviderNameTypeCode varchar(255) NOT NULL,
						OrderingProviderIdentifierTypeCode varchar(255) NOT NULL,
						SetIdOBR bigint NOT NULL,
						UsiID varchar(255) NOT NULL,
						UsiText varchar(255) NOT NULL,
						UsiCodeSystemName varchar(255) NOT NULL,
						UsiIDAlt varchar(255) NOT NULL,
						UsiTextAlt varchar(255) NOT NULL,
						UsiCodeSystemNameAlt varchar(255) NOT NULL,
						UsiTextOriginal varchar(255) NOT NULL,
						ObservationDateTimeStart varchar(255) NOT NULL,
						ObservationDateTimeEnd varchar(255) NOT NULL,
						SpecimenActionCode varchar(255) NOT NULL,
						ResultDateTime varchar(255) NOT NULL,
						ResultStatus varchar(255) NOT NULL,
						ParentObservationID varchar(255) NOT NULL,
						ParentObservationText varchar(255) NOT NULL,
						ParentObservationCodeSystemName varchar(255) NOT NULL,
						ParentObservationIDAlt varchar(255) NOT NULL,
						ParentObservationTextAlt varchar(255) NOT NULL,
						ParentObservationCodeSystemNameAlt varchar(255) NOT NULL,
						ParentObservationTextOriginal varchar(255) NOT NULL,
						ParentObservationSubID varchar(255) NOT NULL,
						ParentPlacerOrderNum varchar(255) NOT NULL,
						ParentPlacerOrderNamespace varchar(255) NOT NULL,
						ParentPlacerOrderUniversalID varchar(255) NOT NULL,
						ParentPlacerOrderUniversalIDType varchar(255) NOT NULL,
						ParentFillerOrderNum varchar(255) NOT NULL,
						ParentFillerOrderNamespace varchar(255) NOT NULL,
						ParentFillerOrderUniversalID varchar(255) NOT NULL,
						ParentFillerOrderUniversalIDType varchar(255) NOT NULL,
						ListEhrLabResultsHandlingF tinyint NOT NULL,
						ListEhrLabResultsHandlingN tinyint NOT NULL,
						TQ1SetId bigint NOT NULL,
						TQ1DateTimeStart varchar(255) NOT NULL,
						TQ1DateTimeEnd varchar(255) NOT NULL,
						INDEX(PatNum),
						INDEX(SetIdOBR),
						INDEX(TQ1SetId)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					//EHR lab features have been hidden for Oracle users due to some column names exceeding the 30 character limit.
					//TODO: Fix the column names to be less than 30 characters in the future.
					//	This will require unhiding EHR lab features for Oracle users, altering the table types and columns in the both MySQL and Oracle.
					/*
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrlab'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlab (
						EhrLabNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						OrderControlCode varchar2(255),
						PlacerOrderNum varchar2(255),
						PlacerOrderNamespace varchar2(255),
						PlacerOrderUniversalID varchar2(255),
						PlacerOrderUniversalIDType varchar2(255),
						FillerOrderNum varchar2(255),
						FillerOrderNamespace varchar2(255),
						FillerOrderUniversalID varchar2(255),
						FillerOrderUniversalIDType varchar2(255),
						PlacerGroupNum varchar2(255),
						PlacerGroupNamespace varchar2(255),
						PlacerGroupUniversalID varchar2(255),
						PlacerGroupUniversalIDType varchar2(255),
						OrderingProviderID varchar2(255),
						OrderingProviderLName varchar2(255),
						OrderingProviderFName varchar2(255),
						OrderingProviderMiddleNames varchar2(255),
						OrderingProviderSuffix varchar2(255),
						OrderingProviderPrefix varchar2(255),
						OrderingProviderAssigningAuthorityNamespaceID varchar2(255),
						OrderingProviderAssigningAuthorityUniversalID varchar2(255),
						OrderingProviderAssigningAuthorityIDType varchar2(255),
						OrderingProviderNameTypeCode varchar2(255),
						OrderingProviderIdentifierTypeCode varchar2(255),
						SetIdOBR number(20) NOT NULL,
						UsiID varchar2(255),
						UsiText varchar2(255),
						UsiCodeSystemName varchar2(255),
						UsiIDAlt varchar2(255),
						UsiTextAlt varchar2(255),
						UsiCodeSystemNameAlt varchar2(255),
						UsiTextOriginal varchar2(255),
						ObservationDateTimeStart varchar2(255),
						ObservationDateTimeEnd varchar2(255),
						SpecimenActionCode varchar2(255),
						ResultDateTime varchar2(255),
						ResultStatus varchar2(255),
						ParentObservationID varchar2(255),
						ParentObservationText varchar2(255),
						ParentObservationCodeSystemName varchar2(255),
						ParentObservationIDAlt varchar2(255),
						ParentObservationTextAlt varchar2(255),
						ParentObservationCodeSystemNameAlt varchar2(255),
						ParentObservationTextOriginal varchar2(255),
						ParentObservationSubID varchar2(255),
						ParentPlacerOrderNum varchar2(255),
						ParentPlacerOrderNamespace varchar2(255),
						ParentPlacerOrderUniversalID varchar2(255),
						ParentPlacerOrderUniversalIDType varchar2(255),
						ParentFillerOrderNum varchar2(255),
						ParentFillerOrderNamespace varchar2(255),
						ParentFillerOrderUniversalID varchar2(255),
						ParentFillerOrderUniversalIDType varchar2(255),
						ListEhrLabResultsHandlingF number(3) NOT NULL,
						ListEhrLabResultsHandlingN number(3) NOT NULL,
						TQ1SetId number(20) NOT NULL,
						TQ1DateTimeStart varchar2(255),
						TQ1DateTimeEnd varchar2(255),
						CONSTRAINT ehrlab_EhrLabNum PRIMARY KEY (EhrLabNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlab_PatNum ON ehrlab (PatNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlab_SetIdOBR ON ehrlab (SetIdOBR)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlab_TQ1SetId ON ehrlab (TQ1SetId)";
					Db.NonQ(command);
					 */
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrlabclinicalinfo";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabclinicalinfo (
						EhrLabClinicalInfoNum bigint NOT NULL auto_increment PRIMARY KEY,
						EhrLabNum bigint NOT NULL,
						ClinicalInfoID varchar(255) NOT NULL,
						ClinicalInfoText varchar(255) NOT NULL,
						ClinicalInfoCodeSystemName varchar(255) NOT NULL,
						ClinicalInfoIDAlt varchar(255) NOT NULL,
						ClinicalInfoTextAlt varchar(255) NOT NULL,
						ClinicalInfoCodeSystemNameAlt varchar(255) NOT NULL,
						ClinicalInfoTextOriginal varchar(255) NOT NULL,
						INDEX(EhrLabNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrlabclinicalinfo'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabclinicalinfo (
						EhrLabClinicalInfoNum number(20) NOT NULL,
						EhrLabNum number(20) NOT NULL,
						ClinicalInfoID varchar2(255),
						ClinicalInfoText varchar2(255),
						ClinicalInfoCodeSystemName varchar2(255),
						ClinicalInfoIDAlt varchar2(255),
						ClinicalInfoTextAlt varchar2(255),
						ClinicalInfoCodeSystemNameAlt varchar2(255),
						ClinicalInfoTextOriginal varchar2(255),
						CONSTRAINT ehrlabclinicalinfo_EhrLabClini PRIMARY KEY (EhrLabClinicalInfoNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlabclinicalinfo_EhrLabNum ON ehrlabclinicalinfo (EhrLabNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrlabnote";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabnote (
						EhrLabNoteNum bigint NOT NULL auto_increment PRIMARY KEY,
						EhrLabNum bigint NOT NULL,
						EhrLabResultNum bigint NOT NULL,
						Comments text NOT NULL,
						INDEX(EhrLabNum),
						INDEX(EhrLabResultNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrlabnote'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabnote (
						EhrLabNoteNum number(20) NOT NULL,
						EhrLabNum number(20) NOT NULL,
						EhrLabResultNum number(20) NOT NULL,
						Comments clob,
						CONSTRAINT ehrlabnote_EhrLabNoteNum PRIMARY KEY (EhrLabNoteNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlabnote_EhrLabNum ON ehrlabnote (EhrLabNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlabnote_EhrLabResultNum ON ehrlabnote (EhrLabResultNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrlabresult";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabresult (
						EhrLabResultNum bigint NOT NULL auto_increment PRIMARY KEY,
						EhrLabNum bigint NOT NULL,
						SetIdOBX bigint NOT NULL,
						ValueType varchar(255) NOT NULL,
						ObservationIdentifierID varchar(255) NOT NULL,
						ObservationIdentifierText varchar(255) NOT NULL,
						ObservationIdentifierCodeSystemName varchar(255) NOT NULL,
						ObservationIdentifierIDAlt varchar(255) NOT NULL,
						ObservationIdentifierTextAlt varchar(255) NOT NULL,
						ObservationIdentifierCodeSystemNameAlt varchar(255) NOT NULL,
						ObservationIdentifierTextOriginal varchar(255) NOT NULL,
						ObservationIdentifierSub varchar(255) NOT NULL,
						ObservationValueCodedElementID varchar(255) NOT NULL,
						ObservationValueCodedElementText varchar(255) NOT NULL,
						ObservationValueCodedElementCodeSystemName varchar(255) NOT NULL,
						ObservationValueCodedElementIDAlt varchar(255) NOT NULL,
						ObservationValueCodedElementTextAlt varchar(255) NOT NULL,
						ObservationValueCodedElementCodeSystemNameAlt varchar(255) NOT NULL,
						ObservationValueCodedElementTextOriginal varchar(255) NOT NULL,
						ObservationValueDateTime varchar(255) NOT NULL,
						ObservationValueTime time NOT NULL DEFAULT '00:00:00',
						ObservationValueComparator varchar(255) NOT NULL,
						ObservationValueNumber1 double NOT NULL,
						ObservationValueSeparatorOrSuffix varchar(255) NOT NULL,
						ObservationValueNumber2 double NOT NULL,
						ObservationValueNumeric double NOT NULL,
						ObservationValueText varchar(255) NOT NULL,
						UnitsID varchar(255) NOT NULL,
						UnitsText varchar(255) NOT NULL,
						UnitsCodeSystemName varchar(255) NOT NULL,
						UnitsIDAlt varchar(255) NOT NULL,
						UnitsTextAlt varchar(255) NOT NULL,
						UnitsCodeSystemNameAlt varchar(255) NOT NULL,
						UnitsTextOriginal varchar(255) NOT NULL,
						referenceRange varchar(255) NOT NULL,
						AbnormalFlags varchar(255) NOT NULL,
						ObservationResultStatus varchar(255) NOT NULL,
						ObservationDateTime varchar(255) NOT NULL,
						AnalysisDateTime varchar(255) NOT NULL,
						PerformingOrganizationName varchar(255) NOT NULL,
						PerformingOrganizationNameAssigningAuthorityNamespaceId varchar(255) NOT NULL,
						PerformingOrganizationNameAssigningAuthorityUniversalId varchar(255) NOT NULL,
						PerformingOrganizationNameAssigningAuthorityUniversalIdType varchar(255) NOT NULL,
						PerformingOrganizationIdentifierTypeCode varchar(255) NOT NULL,
						PerformingOrganizationIdentifier varchar(255) NOT NULL,
						PerformingOrganizationAddressStreet varchar(255) NOT NULL,
						PerformingOrganizationAddressOtherDesignation varchar(255) NOT NULL,
						PerformingOrganizationAddressCity varchar(255) NOT NULL,
						PerformingOrganizationAddressStateOrProvince varchar(255) NOT NULL,
						PerformingOrganizationAddressZipOrPostalCode varchar(255) NOT NULL,
						PerformingOrganizationAddressCountryCode varchar(255) NOT NULL,
						PerformingOrganizationAddressAddressType varchar(255) NOT NULL,
						PerformingOrganizationAddressCountyOrParishCode varchar(255) NOT NULL,
						MedicalDirectorID varchar(255) NOT NULL,
						MedicalDirectorLName varchar(255) NOT NULL,
						MedicalDirectorFName varchar(255) NOT NULL,
						MedicalDirectorMiddleNames varchar(255) NOT NULL,
						MedicalDirectorSuffix varchar(255) NOT NULL,
						MedicalDirectorPrefix varchar(255) NOT NULL,
						MedicalDirectorAssigningAuthorityNamespaceID varchar(255) NOT NULL,
						MedicalDirectorAssigningAuthorityUniversalID varchar(255) NOT NULL,
						MedicalDirectorAssigningAuthorityIDType varchar(255) NOT NULL,
						MedicalDirectorNameTypeCode varchar(255) NOT NULL,
						MedicalDirectorIdentifierTypeCode varchar(255) NOT NULL,
						INDEX(EhrLabNum),
						INDEX(SetIdOBX)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					//EHR lab features have been hidden for Oracle users due to some column names exceeding the 30 character limit.
					//TODO: Fix the column names to be less than 30 characters in the future.
					//	This will require unhiding EHR lab features for Oracle users, altering the table types and columns in the both MySQL and Oracle.
					/*
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrlabresult'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabresult (
						EhrLabResultNum number(20) NOT NULL,
						EhrLabNum number(20) NOT NULL,
						SetIdOBX number(20) NOT NULL,
						ValueType varchar2(255),
						ObservationIdentifierID varchar2(255),
						ObservationIdentifierText varchar2(255),
						ObservationIdentifierCodeSystemName varchar2(255),
						ObservationIdentifierIDAlt varchar2(255),
						ObservationIdentifierTextAlt varchar2(255),
						ObservationIdentifierCodeSystemNameAlt varchar2(255),
						ObservationIdentifierTextOriginal varchar2(255),
						ObservationIdentifierSub varchar2(255),
						ObservationValueCodedElementID varchar2(255),
						ObservationValueCodedElementText varchar2(255),
						ObservationValueCodedElementCodeSystemName varchar2(255),
						ObservationValueCodedElementIDAlt varchar2(255),
						ObservationValueCodedElementTextAlt varchar2(255),
						ObservationValueCodedElementCodeSystemNameAlt varchar2(255),
						ObservationValueCodedElementTextOriginal varchar2(255),
						ObservationValueDateTime varchar2(255),
						ObservationValueTime date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						ObservationValueComparator varchar2(255),
						ObservationValueNumber1 number(38,8) NOT NULL,
						ObservationValueSeparatorOrSuffix varchar2(255),
						ObservationValueNumber2 number(38,8) NOT NULL,
						ObservationValueNumeric number(38,8) NOT NULL,
						ObservationValueText varchar2(255),
						UnitsID varchar2(255),
						UnitsText varchar2(255),
						UnitsCodeSystemName varchar2(255),
						UnitsIDAlt varchar2(255),
						UnitsTextAlt varchar2(255),
						UnitsCodeSystemNameAlt varchar2(255),
						UnitsTextOriginal varchar2(255),
						referenceRange varchar2(255),
						AbnormalFlags varchar2(255),
						ObservationResultStatus varchar2(255),
						ObservationDateTime varchar2(255),
						AnalysisDateTime varchar2(255),
						PerformingOrganizationName varchar2(255),
						PerformingOrganizationNameAssigningAuthorityNamespaceId varchar2(255),
						PerformingOrganizationNameAssigningAuthorityUniversalId varchar2(255),
						PerformingOrganizationNameAssigningAuthorityUniversalIdType varchar2(255),
						PerformingOrganizationIdentifierTypeCode varchar2(255),
						PerformingOrganizationIdentifier varchar2(255),
						PerformingOrganizationAddressStreet varchar2(255),
						PerformingOrganizationAddressOtherDesignation varchar2(255),
						PerformingOrganizationAddressCity varchar2(255),
						PerformingOrganizationAddressStateOrProvince varchar2(255),
						PerformingOrganizationAddressZipOrPostalCode varchar2(255),
						PerformingOrganizationAddressCountryCode varchar2(255),
						PerformingOrganizationAddressAddressType varchar2(255),
						PerformingOrganizationAddressCountyOrParishCode varchar2(255),
						MedicalDirectorID varchar2(255),
						MedicalDirectorLName varchar2(255),
						MedicalDirectorFName varchar2(255),
						MedicalDirectorMiddleNames varchar2(255),
						MedicalDirectorSuffix varchar2(255),
						MedicalDirectorPrefix varchar2(255),
						MedicalDirectorAssigningAuthorityNamespaceID varchar2(255),
						MedicalDirectorAssigningAuthorityUniversalID varchar2(255),
						MedicalDirectorAssigningAuthorityIDType varchar2(255),
						MedicalDirectorNameTypeCode varchar2(255),
						MedicalDirectorIdentifierTypeCode varchar2(255),
						CONSTRAINT ehrlabresult_EhrLabResultNum PRIMARY KEY (EhrLabResultNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlabresult_EhrLabNum ON ehrlabresult (EhrLabNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlabresult_SetIdOBX ON ehrlabresult (SetIdOBX)";
					Db.NonQ(command);
					 */
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrlabresultscopyto";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabresultscopyto (
						EhrLabResultsCopyToNum bigint NOT NULL auto_increment PRIMARY KEY,
						EhrLabNum bigint NOT NULL,
						CopyToID varchar(255) NOT NULL,
						CopyToLName varchar(255) NOT NULL,
						CopyToFName varchar(255) NOT NULL,
						CopyToMiddleNames varchar(255) NOT NULL,
						CopyToSuffix varchar(255) NOT NULL,
						CopyToPrefix varchar(255) NOT NULL,
						CopyToAssigningAuthorityNamespaceID varchar(255) NOT NULL,
						CopyToAssigningAuthorityUniversalID varchar(255) NOT NULL,
						CopyToAssigningAuthorityIDType varchar(255) NOT NULL,
						CopyToNameTypeCode varchar(255) NOT NULL,
						CopyToIdentifierTypeCode varchar(255) NOT NULL,
						INDEX(EhrLabNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					//EHR lab features have been hidden for Oracle users due to some column names exceeding the 30 character limit.
					//TODO: Fix the column names to be less than 30 characters in the future.
					//	This will require unhiding EHR lab features for Oracle users, altering the table types and columns in the both MySQL and Oracle.
					/*
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrlabresultscopyto'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabresultscopyto (
						EhrLabResultsCopyToNum number(20) NOT NULL,
						EhrLabNum number(20) NOT NULL,
						CopyToID varchar2(255),
						CopyToLName varchar2(255),
						CopyToFName varchar2(255),
						CopyToMiddleNames varchar2(255),
						CopyToSuffix varchar2(255),
						CopyToPrefix varchar2(255),
						CopyToAssigningAuthorityNamespaceID varchar2(255),
						CopyToAssigningAuthorityUniversalID varchar2(255),
						CopyToAssigningAuthorityIDType varchar2(255),
						CopyToNameTypeCode varchar2(255),
						CopyToIdentifierTypeCode varchar2(255),
						CONSTRAINT ehrlabresultscopyto_EhrLabResu PRIMARY KEY (EhrLabResultsCopyToNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlabresultscopyto_EhrLabNum ON ehrlabresultscopyto (EhrLabNum)";
					Db.NonQ(command);
					 */
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrlabspecimen";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabspecimen (
						EhrLabSpecimenNum bigint NOT NULL auto_increment PRIMARY KEY,
						EhrLabNum bigint NOT NULL,
						SetIdSPM bigint NOT NULL,
						SpecimenTypeID varchar(255) NOT NULL,
						SpecimenTypeText varchar(255) NOT NULL,
						SpecimenTypeCodeSystemName varchar(255) NOT NULL,
						SpecimenTypeIDAlt varchar(255) NOT NULL,
						SpecimenTypeTextAlt varchar(255) NOT NULL,
						SpecimenTypeCodeSystemNameAlt varchar(255) NOT NULL,
						SpecimenTypeTextOriginal varchar(255) NOT NULL,
						CollectionDateTimeStart varchar(255) NOT NULL,
						CollectionDateTimeEnd varchar(255) NOT NULL,
						INDEX(EhrLabNum),
						INDEX(SetIdSPM)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrlabspecimen'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabspecimen (
						EhrLabSpecimenNum number(20) NOT NULL,
						EhrLabNum number(20) NOT NULL,
						SetIdSPM number(20) NOT NULL,
						SpecimenTypeID varchar2(255),
						SpecimenTypeText varchar2(255),
						SpecimenTypeCodeSystemName varchar2(255),
						SpecimenTypeIDAlt varchar2(255),
						SpecimenTypeTextAlt varchar2(255),
						SpecimenTypeCodeSystemNameAlt varchar2(255),
						SpecimenTypeTextOriginal varchar2(255),
						CollectionDateTimeStart varchar2(255),
						CollectionDateTimeEnd varchar2(255),
						CONSTRAINT ehrlabspecimen_EhrLabSpecimenN PRIMARY KEY (EhrLabSpecimenNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlabspecimen_EhrLabNum ON ehrlabspecimen (EhrLabNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlabspecimen_SetIdSPM ON ehrlabspecimen (SetIdSPM)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrlabspecimencondition";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabspecimencondition (
						EhrLabSpecimenConditionNum bigint NOT NULL auto_increment PRIMARY KEY,
						EhrLabSpecimenNum bigint NOT NULL,
						SpecimenConditionID varchar(255) NOT NULL,
						SpecimenConditionText varchar(255) NOT NULL,
						SpecimenConditionCodeSystemName varchar(255) NOT NULL,
						SpecimenConditionIDAlt varchar(255) NOT NULL,
						SpecimenConditionTextAlt varchar(255) NOT NULL,
						SpecimenConditionCodeSystemNameAlt varchar(255) NOT NULL,
						SpecimenConditionTextOriginal varchar(255) NOT NULL,
						INDEX(EhrLabSpecimenNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					//EHR lab features have been hidden for Oracle users due to some column names exceeding the 30 character limit.
					//TODO: Fix the column names to be less than 30 characters in the future.
					//	This will require unhiding EHR lab features for Oracle users, altering the table types and columns in the both MySQL and Oracle.
					/*
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrlabspecimencondition'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabspecimencondition (
						EhrLabSpecimenConditionNum number(20) NOT NULL,
						EhrLabSpecimenNum number(20) NOT NULL,
						SpecimenConditionID varchar2(255),
						SpecimenConditionText varchar2(255),
						SpecimenConditionCodeSystemName varchar2(255),
						SpecimenConditionIDAlt varchar2(255),
						SpecimenConditionTextAlt varchar2(255),
						SpecimenConditionCodeSystemNameAlt varchar2(255),
						SpecimenConditionTextOriginal varchar2(255),
						CONSTRAINT ehrlabspecimencondition_EhrLab PRIMARY KEY (EhrLabSpecimenConditionNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlabspecimencondition_EhrLab ON ehrlabspecimencondition (EhrLabSpecimenNum)";
					Db.NonQ(command);
					 */
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrlabspecimenrejectreason";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabspecimenrejectreason (
						EhrLabSpecimenRejectReasonNum bigint NOT NULL auto_increment PRIMARY KEY,
						EhrLabSpecimenNum bigint NOT NULL,
						SpecimenRejectReasonID varchar(255) NOT NULL,
						SpecimenRejectReasonText varchar(255) NOT NULL,
						SpecimenRejectReasonCodeSystemName varchar(255) NOT NULL,
						SpecimenRejectReasonIDAlt varchar(255) NOT NULL,
						SpecimenRejectReasonTextAlt varchar(255) NOT NULL,
						SpecimenRejectReasonCodeSystemNameAlt varchar(255) NOT NULL,
						SpecimenRejectReasonTextOriginal varchar(255) NOT NULL,
						INDEX(EhrLabSpecimenNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					//EHR lab features have been hidden for Oracle users due to some column names exceeding the 30 character limit.
					//TODO: Fix the column names to be less than 30 characters in the future.
					//	This will require unhiding EHR lab features for Oracle users, altering the table types and columns in the both MySQL and Oracle.
					/*
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrlabspecimenrejectreason'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabspecimenrejectreason (
						EhrLabSpecimenRejectReasonNum number(20) NOT NULL,
						EhrLabSpecimenNum number(20) NOT NULL,
						SpecimenRejectReasonID varchar2(255),
						SpecimenRejectReasonText varchar2(255),
						SpecimenRejectReasonCodeSystemName varchar2(255),
						SpecimenRejectReasonIDAlt varchar2(255),
						SpecimenRejectReasonTextAlt varchar2(255),
						SpecimenRejectReasonCodeSystemNameAlt varchar2(255),
						SpecimenRejectReasonTextOriginal varchar2(255),
						CONSTRAINT ehrlabspecimenrejectreason_Ehr PRIMARY KEY (EhrLabSpecimenRejectReasonNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlabspecimenrejectreason_Ehr ON ehrlabspecimenrejectreason (EhrLabSpecimenNum)";
					Db.NonQ(command);
					 */
				}
				#endregion
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE guardian ADD IsGuardian tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE guardian ADD IsGuardian number(3)";
					Db.NonQ(command);
					command="UPDATE guardian SET IsGuardian = 0 WHERE IsGuardian IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE guardian MODIFY IsGuardian NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE guardian SET IsGuardian=1";//Works for both MySQL and Oracle.
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE allergydef ADD UniiCode varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE allergydef ADD UniiCode varchar2(255)";
					Db.NonQ(command);
				}
				//Oracle compatible.
				command="ALTER TABLE allergydef DROP COLUMN SnomedAllergyTo";
				Db.NonQ(command);
				//OID External
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS oidexternal";
					Db.NonQ(command);
					command=@"CREATE TABLE oidexternal (
						OIDExternalNum bigint NOT NULL auto_increment PRIMARY KEY,
						IDType varchar(255) NOT NULL,
						IDInternal bigint NOT NULL,
						IDExternal varchar(255) NOT NULL,
						rootExternal varchar(255) NOT NULL,
						INDEX(IDType,IDInternal),
						INDEX(rootExternal(62),IDExternal(62))
						) DEFAULT CHARSET=utf8";//Index is 1000/8=125/n where n is the number of columns to be indexed together. In this case the result is 62.5=62
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE oidexternal'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE oidexternal (
						OIDExternalNum number(20) NOT NULL,
						IDType varchar2(255),
						IDInternal number(20),
						IDExternal varchar2(255),
						rootExternal varchar2(255),
						CONSTRAINT oidexternal_OIDExternalNum PRIMARY KEY (OIDExternalNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX oidexternal_type_ID ON oidexternal (IDType, IDInternal)";
					Db.NonQ(command);
					command=@"CREATE INDEX oidexternal_root_extension ON oidexternal (rootExternal, IDExternal)";
					Db.NonQ(command);
				}
				//OID Internal
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS oidinternal";
					Db.NonQ(command);
					command=@"CREATE TABLE oidinternal (
						OIDInternalNum bigint NOT NULL auto_increment PRIMARY KEY,
						IDType varchar(255) NOT NULL,
						IDRoot varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE oidinternal'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE oidinternal (
						OIDInternalNum number(20) NOT NULL,
						IDType varchar2(255),
						IDRoot varchar2(255),
						CONSTRAINT oidinternal_OIDInternalNum PRIMARY KEY (OIDInternalNum)
						)";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vaccinepat ADD FilledCity varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vaccinepat ADD FilledCity varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vaccinepat ADD FilledST varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vaccinepat ADD FilledST varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vaccinepat ADD CompletionStatus tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vaccinepat ADD CompletionStatus number(3)";
					Db.NonQ(command);
					command="UPDATE vaccinepat SET CompletionStatus = 0 WHERE CompletionStatus IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat MODIFY CompletionStatus NOT NULL";
					Db.NonQ(command);
				}
				//MySQL and Oracle
				command="UPDATE vaccinepat SET CompletionStatus=CASE WHEN NotGiven=1 THEN 2 ELSE 0 END";//If was NotGiven then CompletionStatus=NotAdministered, otherwise CompletionStatus=Complete.
				Db.NonQ(command);
				//MySQL and Oracle
				command="ALTER TABLE vaccinepat DROP COLUMN NotGiven";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vaccinepat ADD AdministrationNoteCode tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vaccinepat ADD AdministrationNoteCode number(3)";
					Db.NonQ(command);
					command="UPDATE vaccinepat SET AdministrationNoteCode = 0 WHERE AdministrationNoteCode IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat MODIFY AdministrationNoteCode NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vaccinepat ADD UserNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat ADD INDEX (UserNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vaccinepat ADD UserNum number(20)";
					Db.NonQ(command);
					command="UPDATE vaccinepat SET UserNum = 0 WHERE UserNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat MODIFY UserNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX vaccinepat_UserNum ON vaccinepat (UserNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vaccinepat ADD ProvNumOrdering bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat ADD INDEX (ProvNumOrdering)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vaccinepat ADD ProvNumOrdering number(20)";
					Db.NonQ(command);
					command="UPDATE vaccinepat SET ProvNumOrdering = 0 WHERE ProvNumOrdering IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat MODIFY ProvNumOrdering NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX vaccinepat_ProvNumOrdering ON vaccinepat (ProvNumOrdering)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vaccinepat ADD ProvNumAdminister bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat ADD INDEX (ProvNumAdminister)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vaccinepat ADD ProvNumAdminister number(20)";
					Db.NonQ(command);
					command="UPDATE vaccinepat SET ProvNumAdminister = 0 WHERE ProvNumAdminister IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat MODIFY ProvNumAdminister NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX vaccinepat_ProvNumAdminister ON vaccinepat (ProvNumAdminister)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vaccinepat ADD DateExpire date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vaccinepat ADD DateExpire date";
					Db.NonQ(command);
					command="UPDATE vaccinepat SET DateExpire = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateExpire IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat MODIFY DateExpire NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vaccinepat ADD RefusalReason tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vaccinepat ADD RefusalReason number(3)";
					Db.NonQ(command);
					command="UPDATE vaccinepat SET RefusalReason = 0 WHERE RefusalReason IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat MODIFY RefusalReason NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vaccinepat ADD ActionCode tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vaccinepat ADD ActionCode number(3)";
					Db.NonQ(command);
					command="UPDATE vaccinepat SET ActionCode = 0 WHERE ActionCode IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat MODIFY ActionCode NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vaccinepat ADD AdministrationRoute tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vaccinepat ADD AdministrationRoute number(3)";
					Db.NonQ(command);
					command="UPDATE vaccinepat SET AdministrationRoute = 0 WHERE AdministrationRoute IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat MODIFY AdministrationRoute NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vaccinepat ADD AdministrationSite tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vaccinepat ADD AdministrationSite number(3)";
					Db.NonQ(command);
					command="UPDATE vaccinepat SET AdministrationSite = 0 WHERE AdministrationSite IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vaccinepat MODIFY AdministrationSite NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS vaccineobs";
					Db.NonQ(command);
					command=@"CREATE TABLE vaccineobs (
						VaccineObsNum bigint NOT NULL auto_increment PRIMARY KEY,
						VaccinePatNum bigint NOT NULL,
						ValType tinyint NOT NULL,
						IdentifyingCode tinyint NOT NULL,
						ValReported varchar(255) NOT NULL,
						ValCodeSystem tinyint NOT NULL,
						VaccineObsNumGroup bigint NOT NULL,
						UcumCode varchar(255) NOT NULL,
						DateObs date NOT NULL DEFAULT '0001-01-01',
						MethodCode varchar(255) NOT NULL,
						INDEX(VaccinePatNum),
						INDEX(VaccineObsNumGroup)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE vaccineobs'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE vaccineobs (
						VaccineObsNum number(20) NOT NULL,
						VaccinePatNum number(20) NOT NULL,
						ValType number(3) NOT NULL,
						IdentifyingCode number(3) NOT NULL,
						ValReported varchar2(255),
						ValCodeSystem number(3) NOT NULL,
						VaccineObsNumGroup number(20) NOT NULL,
						UcumCode varchar2(255),
						DateObs date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						MethodCode varchar2(255),
						CONSTRAINT vaccineobs_VaccineObsNum PRIMARY KEY (VaccineObsNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX vaccineobs_VaccinePatNum ON vaccineobs (VaccinePatNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX vaccineobs_VaccineObsNumGroup ON vaccineobs (VaccineObsNumGroup)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrmeasureevent ADD FKey bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE ehrmeasureevent ADD INDEX (FKey)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ehrmeasureevent ADD FKey number(20)";
					Db.NonQ(command);
					command="UPDATE ehrmeasureevent SET FKey = 0 WHERE FKey IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE ehrmeasureevent MODIFY FKey NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrmeasureevent_FKey ON ehrmeasureevent (FKey)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vitalsign ADD BMIPercentile int NOT NULL";
					Db.NonQ(command);
					command="UPDATE vitalsign SET BMIPercentile=-1";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vitalsign ADD BMIPercentile number(11)";
					Db.NonQ(command);
					command="UPDATE vitalsign SET BMIPercentile = -1 WHERE BMIPercentile IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vitalsign MODIFY BMIPercentile NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD SnomedBodySite varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD SnomedBodySite varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrpatient";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrpatient (
						PatNum bigint NOT NULL PRIMARY KEY,
						MotherMaidenFname varchar(255) NOT NULL,
						MotherMaidenLname varchar(255) NOT NULL,
						VacShareOk tinyint NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrpatient'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrpatient (
						PatNum number(20) NOT NULL,
						MotherMaidenFname varchar2(255),
						MotherMaidenLname varchar2(255),
						VacShareOk number(3) NOT NULL,
						CONSTRAINT ehrpatient_PatNum PRIMARY KEY (PatNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehraptobs";
					Db.NonQ(command);
					command=@"CREATE TABLE ehraptobs (
						EhrAptObsNum bigint NOT NULL auto_increment PRIMARY KEY,
						AptNum bigint NOT NULL,
						IdentifyingCode tinyint NOT NULL,
						ValType tinyint NOT NULL,
						ValReported varchar(255) NOT NULL,
						UcumCode varchar(255) NOT NULL,
						ValCodeSystem varchar(255) NOT NULL,
						INDEX(AptNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehraptobs'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehraptobs (
						EhrAptObsNum number(20) NOT NULL,
						AptNum number(20) NOT NULL,
						IdentifyingCode number(3) NOT NULL,
						ValType number(3) NOT NULL,
						ValReported varchar2(255),
						UcumCode varchar2(255),
						ValCodeSystem varchar2(255),
						CONSTRAINT ehraptobs_EhrAptObsNum PRIMARY KEY (EhrAptObsNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehraptobs_AptNum ON ehraptobs (AptNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patient ADD DateTimeDeceased datetime DEFAULT '0001-01-01' NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patient ADD DateTimeDeceased date";
					Db.NonQ(command);
					command="UPDATE patient SET DateTimeDeceased = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateTimeDeceased IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patient MODIFY DateTimeDeceased NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrlab ADD IsCpoe tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					//EHR lab features have been hidden for Oracle users due to some column names exceeding the 30 character limit.
					//TODO: Fix the column names to be less than 30 characters in the future.
					//	This will require unhiding EHR lab features for Oracle users, altering the table types and columns in the both MySQL and Oracle.
					/*
					command="ALTER TABLE ehrlab ADD IsCpoe number(3)";
					Db.NonQ(command);
					command="UPDATE ehrlab SET IsCpoe = 0 WHERE IsCpoe IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE ehrlab MODIFY IsCpoe NOT NULL";
					Db.NonQ(command);
					 */
				}
				//Add additional EHR Measures to DB
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(20,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(21,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(22,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(23,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(24,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(25,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(26,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(27,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(28,-1,-1)";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),20,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),21,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),22,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),23,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),24,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),25,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),26,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),27,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),28,-1,-1)";
					Db.NonQ(command);
				}
				//Split patientrace DeclinedToSpecify into DeclinedToSpecifyRace and DeclinedToSpecifyEthnicity.
				command="SELECT PatNum FROM patientrace WHERE Race=4";//DeclinedToSpecifyRace
				DataTable table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					string patNum=table.Rows[i]["PatNum"].ToString();
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO patientrace (PatNum,Race) VALUES ("+patNum+",11)";//DeclinedToSpecifyEthnicity
					}
					else {//oracle
						command="INSERT INTO patientrace (PatientRaceNum,PatNum,Race,CdcrecCode) VALUES ((SELECT MAX(PatientRaceNum+1) FROM patientrace),"+patNum+",11,'')";
					}
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ehrlabimage";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabimage (
						EhrLabImageNum bigint NOT NULL auto_increment PRIMARY KEY,
						EhrLabNum bigint NOT NULL,
						DocNum bigint NOT NULL,
						INDEX(EhrLabNum),
						INDEX(DocNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ehrlabimage'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ehrlabimage (
						EhrLabImageNum number(20) NOT NULL,
						EhrLabNum number(20) NOT NULL,
						DocNum number(20) NOT NULL,
						CONSTRAINT ehrlabimage_EhrLabImageNum PRIMARY KEY (EhrLabImageNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlabimage_EhrLabNum ON ehrlabimage (EhrLabNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX ehrlabimage_DocNum ON ehrlabimage (DocNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE apptfielddef CHANGE PickList PickList TEXT NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE apptfielddef MODIFY (PickList varchar2(4000) NOT NULL)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE refattach ADD ProvNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE refattach ADD INDEX (ProvNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE refattach ADD ProvNum number(20)";
					Db.NonQ(command);
					command="UPDATE refattach SET ProvNum = 0 WHERE ProvNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE refattach MODIFY ProvNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX refattach_ProvNum ON refattach (ProvNum)";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrlab ADD OriginalPIDSegment text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					//EHR lab features have been hidden for Oracle users due to some column names exceeding the 30 character limit.
					//TODO: Fix the column names to be less than 30 characters in the future.
					//	This will require unhiding EHR lab features for Oracle users, altering the table types and columns in the both MySQL and Oracle.
					/*
					command="ALTER TABLE ehrlab ADD OriginalPIDSegment varchar2(4000)";
					Db.NonQ(command);
					 */
				}
				//Added TimeCardADPExportIncludesName preference
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('TimeCardADPExportIncludesName','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'TimeCardADPExportIncludesName','0')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '14.1.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_1_3();
		}

		private static void To14_1_3() {
			if(FromVersion<new Version("14.1.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.1.3");//No translation in convert script.
				string command;
				//add programproperty to eClinicalWorks program link for changing the FT1 segments of the DFT messages
				//to place quadrants in the ToothNum component instead of the surface component
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ProgramNum FROM program WHERE ProgName='eClinicalWorks'";
					int programNum=PIn.Int(Db.GetScalar(command));
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
							+") VALUES("
							+"'"+POut.Long(programNum)+"', "
							+"'IsQuadAsToothNum', "
							+"'0')";//set to 0 (false) by default so behavior of existing customers will not change
					Db.NonQ(command);
				}
				else {//oracle
					//eCW will never use Oracle.
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE hl7def ADD IsQuadAsToothNum tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE hl7def ADD IsQuadAsToothNum number(3)";
					Db.NonQ(command);
					command="UPDATE hl7def SET IsQuadAsToothNum = 0 WHERE IsQuadAsToothNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE hl7def MODIFY IsQuadAsToothNum NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '14.1.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_1_8();
		}

		private static void To14_1_8() {
			if(FromVersion<new Version("14.1.8.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.1.8");//No translation in convert script.
				string command;
				//add programproperty to eClinicalWorks program link for changing the cookie creation for the LBSESSIONID
				//This is a fix for their version 10 so that the medical panel will work correctly
				command="SELECT ProgramNum FROM program WHERE ProgName='eClinicalWorks'";
				int programNum=PIn.Int(Db.GetScalar(command));
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
							+") VALUES("
							+"'"+POut.Long(programNum)+"', "
							+"'IsLBSessionIdExcluded', "
							+"'0')";//set to 0 (false) by default so behavior of existing customers will not change
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
							+") VALUES("
							+"(SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),"
							+"'"+POut.Long(programNum)+"', "
							+"'IsLBSessionIdExcluded', "
							+"'0')";//set to 0 (false) by default so behavior of existing customers will not change
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '14.1.8.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_2_1();
		}

		///<summary>Oracle compatible: 05/13/2014</summary>
		private static void To14_2_1() {
			if(FromVersion<new Version("14.2.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.2.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('CustListenerPort','25255')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'CustListenerPort','25255')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claimpayment ADD PayType bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimpayment ADD INDEX (PayType)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claimpayment ADD PayType number(20)";
					Db.NonQ(command);
					command="UPDATE claimpayment SET PayType = 0 WHERE PayType IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimpayment MODIFY PayType NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX claimpayment_PayType ON claimpayment (PayType)";
					Db.NonQ(command);
				}
				//Add program property for Tigerview enchancement
				command="SELECT ProgramNum FROM program WHERE ProgName='TigerView'";
				string progNum=Db.GetScalar(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+progNum+"', "
				    +"'TigerView EMR folder path', "
				    +"'')";
					Db.NonQ(command);
				}
				else {
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
						+"(SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),"
				    +"'"+progNum+"', "
				    +"'TigerView EMR folder path', "
				    +"'')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemValue) "
									+"VALUES (32"+",'"+POut.String("Check")+"','0','')";
				}
				else {//oracle
					command="INSERT INTO definition (DefNum,Category,ItemName,ItemOrder,ItemValue) "
									+"VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),32,'"+POut.String("Check")+"','0','')";
				}
				long defNum=Db.NonQ(command,true,"DefNum","definition");
				//At this point in time, all claimpayments in the database are assumed to be of pay type "Check".
				command="UPDATE claimpayment SET PayType = "+POut.Long(defNum)+ " WHERE PayType = 0";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemValue) "
									+"VALUES (32"+",'"+POut.String("EFT")+"','1','N')";
				}
				else {//oracle
					command="INSERT INTO definition (DefNum,Category,ItemName,ItemOrder,ItemValue) "
									+"VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),32,'"+POut.String("EFT")+"','1','N')";
				}
				Db.NonQ(command);
				//Insert VistaDent Bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"'VistaDent', "
				    +"'VistaDent from www.gactechnocenter.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\Program Files\GAC\VistaDent\VistaDent.exe")+"',"
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"'"+POut.Long(programNum)+"', "
				    +"'"+POut.Int(((int)ToolBarsAvail.ChartModule))+"', "
				    +"'VistaDent')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'VistaDent', "
				    +"'VistaDent from www.gactechnocenter.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\Program Files\GAC\VistaDent\VistaDent.exe")+"',"
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'"+POut.Int(((int)ToolBarsAvail.ChartModule))+"', "
				    +"'VistaDent')";
					Db.NonQ(command);
				}//end VistaDent bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD DiagnosticCode2 varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD DiagnosticCode2 varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD DiagnosticCode3 varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD DiagnosticCode3 varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD DiagnosticCode4 varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD DiagnosticCode4 varchar2(255)";
					Db.NonQ(command);
				}
				//Drop depricated Formulary table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS formulary";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE formulary'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
				}
				//Drop depricated FormularyMed table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS formularymed";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE formularymed'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
				}
				//Blue theme
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ColorTheme','1')";//On by default
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ColorTheme','1')";//On by default
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ShowFeaturePatientClone','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ShowFeaturePatientClone','0')";
					Db.NonQ(command);
				}
				//Add new SoftwareName preference for EHR report printing. Defaults to Open Dental Software.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('SoftwareName','Open Dental Software')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SoftwareName','Open Dental Software')";
					Db.NonQ(command);
				}
				//Add the 1500 claim form version 02/12 fields if the claim form does not already exist. The unique ID is OD12.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD12' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT * FROM (SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD12') WHERE RowNum<=1";
				}
				DataTable tableClaimFormNum=Db.GetTable(command);
				if(tableClaimFormNum.Rows.Count==0) {
					long claimFormNum=0;
					//The 1500 claim form version 02/12 does not exist, so safe to add.
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO claimform(Description,IsHidden,FontName,FontSize,UniqueID,PrintImages,OffsetX,OffsetY) "+
							"VALUES ('1500_02_12',0,'Arial',9,'OD12',0,0,0)";
						claimFormNum=Db.NonQ(command,true);
					}
					else {//oracle
						command="INSERT INTO claimform(ClaimFormNum,Description,IsHidden,FontName,FontSize,UniqueID,PrintImages,OffsetX,OffsetY) "+
							"VALUES ((SELECT COALESCE(MAX(ClaimFormNum),0)+1 ClaimFormNum FROM claimform),'1500_02_12',0,'Arial',9,'OD12',1,0,0)";
						claimFormNum=Db.NonQ(command,true,"ClaimFormNum","claimform");
					}
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'1500_02_12.gif','','','6','6','850','1170')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','AccidentST','','467','386','30','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentist','','531','984','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentist','','256','985','235','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistNPI','','260','1035','92','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistNPI','','531','1035','92','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistNumIsSSN','','189','951','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistNumIsTIN','','209','951','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistPh123','','680','968','40','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistPh456','','719','968','40','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistPh78910','','759','968','48','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','BillingDentistSSNorTIN','','39','949','131','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DiagnosisA','','46','651','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DiagnosisB','','176','651','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DiagnosisC','','306','651','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DiagnosisD','','436','651','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DiagnosisE','','46','668','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DiagnosisF','','176','668','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DiagnosisG','','306','668','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DiagnosisH','','436','668','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DiagnosisI','','46','684','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DiagnosisJ','','176','684','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DiagnosisK','','306','684','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','DiagnosisL','','436','683','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','EmployerName','','540','386','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','GroupNum','','531','319','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','ICDindicator','','437','636','20','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsAutoAccident','','368','386','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsGroupHealthPlan','','326','154','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsMedicaidClaim','','96','154','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsNotAutoAccident','','428','385','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsNotOccupational','','428','352','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsNotOtherAccident','','428','418','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsOccupational','','368','352','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','IsOtherAccident','','368','418','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsCarrierName','','36','450','245','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsExists','','537','451','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsGroupNum','','36','353','245','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsNotExists','','588','451','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','OtherInsSubscrLastFirst','','36','320','245','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Code','','274','752','55','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1CodeMod1','','340','752','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1CodeMod2','','375','752','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1CodeMod3','','405','752','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1CodeMod4','','435','752','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Date','MM    dd    yy','32','752','80','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Date','MM    dd     yy','122','752','80','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1DiagnosisPoint','','467','752','48','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1Fee','','598','752','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1PlaceNumericCode','','206','752','28','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1TreatProvNPI','','698','752','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P1UnitQty','','615','752','20','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Code','','274','786','55','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2CodeMod1','','340','786','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2CodeMod2','','375','786','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2CodeMod3','','405','786','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2CodeMod4','','435','786','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Date','MM    dd    yy','32','786','80','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Date','MM    dd     yy','122','786','80','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2DiagnosisPoint','','467','786','48','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2Fee','','598','786','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2PlaceNumericCode','','206','786','28','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2TreatProvNPI','','698','786','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P2UnitQty','','615','786','20','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Code','','274','818','55','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3CodeMod1','','340','818','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3CodeMod2','','375','818','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3CodeMod3','','405','818','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3CodeMod4','','435','818','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Date','MM    dd    yy','32','818','80','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Date','MM    dd     yy','121','818','80','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3DiagnosisPoint','','467','818','48','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3Fee','','598','818','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3PlaceNumericCode','','206','819','28','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3TreatProvNPI','','698','818','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P3UnitQty','','615','818','20','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Code','','274','852','55','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4CodeMod1','','340','852','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4CodeMod2','','375','852','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4CodeMod3','','405','852','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4CodeMod4','','435','852','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Date','MM    dd    yy','32','852','80','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Date','MM    dd     yy','122','852','80','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4DiagnosisPoint','','467','852','48','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4Fee','','598','852','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4PlaceNumericCode','','206','853','28','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4TreatProvNPI','','698','852','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P4UnitQty','','615','852','20','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Code','','274','885','55','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5CodeMod1','','340','885','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5CodeMod2','','375','885','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5CodeMod3','','405','885','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5CodeMod4','','435','885','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Date','MM    dd    yy','32','885','80','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Date','MM    dd     yy','122','885','80','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5DiagnosisPoint','','467','885','48','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5Fee','','598','885','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5PlaceNumericCode','','206','885','28','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5TreatProvNPI','','698','885','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P5UnitQty','','615','885','20','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Code','','274','919','55','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6CodeMod1','','340','919','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6CodeMod2','','375','919','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6CodeMod3','','405','919','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6CodeMod4','','435','919','26','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Date','MM    dd    yy','32','919','80','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Date','MM    dd     yy','122','919','80','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6DiagnosisPoint','','467','919','48','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6Fee','','598','919','70','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6PlaceNumericCode','','206','919','28','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6TreatProvNPI','','698','919','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','P6UnitQty','','615','919','20','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientAddress','','37','216','245','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientAssignment','','577','512','210','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientCity','','37','251','200','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientDOB','MM    dd    yyyy','333','187','95','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientIsFemale','','487','186','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientIsMale','','437','186','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientLastFirst','','37','184','245','13')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientPhone','','169','286','120','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientRelease','','78','512','240','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientReleaseDate','MM/dd/yyyy','386','512','113','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientST','','281','253','30','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PatientZip','','37','287','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PayToDentistAddress','','531','1000','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PayToDentistCity','','531','1016','139','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PayToDentistST','','671','1016','30','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PayToDentistZip','','701','1016','80','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsAddress','','419','86','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsAddress2','','419','100','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsCarrierName','','419','72','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsCarrierName','','527','416','245','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsCity','','419','114','140','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsST','','560','114','30','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriInsZip','','590','114','79','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','PriorAuthString','','528','685','282','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','RelatIsChild','','437','218','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','RelatIsOther','','487','218','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','RelatIsSelf','','347','218','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','RelatIsSpouse','','397','218','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrAddress','','530','219','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrCity','','530','253','200','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrDOB','MM    dd     yyyy','554','353','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrID','','530','153','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrIsFemale','','769','352','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrIsMale','','699','352','0','0')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrLastFirst','','530','186','250','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrPhone','','672','287','120','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrST','','760','253','50','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','SubscrZip','','532','286','100','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TotalFee','','620','951','75','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistAddress','','256','999','235','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistCity','','256','1013','132','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistSigDate','','169','1025','74','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistSignature','','27','1010','142','30')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistST','','388','1013','28','14')";
					Db.NonQ(command);
					command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
						+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','TreatingDentistZip','','416','1013','75','14')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrprovkey ADD YearValue int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ehrprovkey ADD YearValue number(11)";
					Db.NonQ(command);
					command="UPDATE ehrprovkey SET YearValue = 0 WHERE YearValue IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE ehrprovkey MODIFY YearValue NOT NULL";
					Db.NonQ(command);
				}
				command="ALTER TABLE provider DROP COLUMN EhrHasReportAccess";
				Db.NonQ(command);
				command="ALTER TABLE ehrprovkey DROP COLUMN HasReportAccess";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '14.2.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_2_18();
		}

		private static void To14_2_18() {
			if(FromVersion<new Version("14.2.18.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.2.18");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD11' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT * FROM (SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD11') WHERE RowNum<=1";
				}
				long claimFormNum=PIn.Long(Db.GetScalar(command));
				command="UPDATE claimformitem SET claimformitem.FieldName = 'P1DiagnosisPoint' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND claimformitem.FieldName = 'P1Diagnosis'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET claimformitem.FieldName = 'P2DiagnosisPoint' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND claimformitem.FieldName = 'P2Diagnosis'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET claimformitem.FieldName = 'P3DiagnosisPoint' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND claimformitem.FieldName = 'P3Diagnosis'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET claimformitem.FieldName = 'P4DiagnosisPoint' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND claimformitem.FieldName = 'P4Diagnosis'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET claimformitem.FieldName = 'P5DiagnosisPoint' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND claimformitem.FieldName = 'P5Diagnosis'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET claimformitem.FieldName = 'P6DiagnosisPoint' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND claimformitem.FieldName = 'P6Diagnosis'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET claimformitem.FieldName = 'P7DiagnosisPoint' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND claimformitem.FieldName = 'P7Diagnosis'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET claimformitem.FieldName = 'P8DiagnosisPoint' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND claimformitem.FieldName = 'P8Diagnosis'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET claimformitem.FieldName = 'P9DiagnosisPoint' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND claimformitem.FieldName = 'P9Diagnosis'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET claimformitem.FieldName = 'P10DiagnosisPoint' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND claimformitem.FieldName = 'P10Diagnosis'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET claimformitem.FieldName = 'DiagnosisA' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND claimformitem.FieldName = 'Diagnosis1'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET claimformitem.FieldName = 'DiagnosisB' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND claimformitem.FieldName = 'Diagnosis2'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET claimformitem.FieldName = 'DiagnosisC' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND claimformitem.FieldName = 'Diagnosis3'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET claimformitem.FieldName = 'DiagnosisD' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND claimformitem.FieldName = 'Diagnosis4'";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '14.2.18.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_2_20();
		}

		///<summary>Oracle compatible: 07/07/2014</summary>
		private static void To14_2_20() {
			if(FromVersion<new Version("14.2.20.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.2.20");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT CanadianNetworkNum FROM canadiannetwork WHERE Abbrev='TELUS B' LIMIT 1";
				}
				else {
					command="SELECT CanadianNetworkNum FROM canadiannetwork WHERE Abbrev='TELUS B' AND RowNum<=1";
				}
				long canadianNetworkNumTelusB=PIn.Long(Db.GetScalar(command));
				command="UPDATE carrier SET "+
					"CDAnetVersion='04',"+
					"CanadianSupportedTypes=2044,"+//Claims, Reversals, Predeterminations, COBs.
					"CanadianNetworkNum="+POut.Long((long)canadianNetworkNumTelusB)+" "+
					"WHERE IsCDA<>0 AND ElectID='000016'";
				Db.NonQ32(command);
				command="UPDATE preference SET ValueString = '14.2.20.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_2_21();
		}

		private static void To14_2_21() {
			if(FromVersion<new Version("14.2.21.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.2.21");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD ProvOrderOverride bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim ADD INDEX (ProvOrderOverride)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD ProvOrderOverride number(20)";
					Db.NonQ(command);
					command="UPDATE claim SET ProvOrderOverride = 0 WHERE ProvOrderOverride IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim MODIFY ProvOrderOverride NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX claim_ProvOrderOverride ON claim (ProvOrderOverride)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD ProvOrderOverride bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog ADD INDEX (ProvOrderOverride)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD ProvOrderOverride number(20)";
					Db.NonQ(command);
					command="UPDATE procedurelog SET ProvOrderOverride = 0 WHERE ProvOrderOverride IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog MODIFY ProvOrderOverride NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX procedurelog_ProvOrderOverride ON procedurelog (ProvOrderOverride)";
					Db.NonQ(command);
				}
				//Users started to get a (403) Forbidden error when trying to update.
				//Come to find out it was due to a redirect issue.  We're going to update the Uri to point to opendental.com instead of open-dent.com so that this doesn't happen again.
				command=@"UPDATE preference SET ValueString='http://www.opendental.com/updates/' WHERE PrefName='UpdateWebsitePath' AND ValueString='http://www.open-dent.com/updates/'";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '14.2.21.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_2_23();
		}

		private static void To14_2_23() {
			if(FromVersion<new Version("14.2.23.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.2.23");//No translation in convert script.
				string command;
				//We fixed the (403) Forbidden error by getting rid of the redirect, changing the A record for open-dent.com and pointing it to HQ.
				//Therefore, we now want users to be pointing to open-dent instead of opendental.  This simply undoes what happened at the end of the 14.2.21 method.
				command=@"UPDATE preference SET ValueString='http://www.open-dent.com/updates/' WHERE PrefName='UpdateWebsitePath' AND ValueString='http://www.opendental.com/updates/'";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '14.2.23.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_2_32();
		}

		private static void To14_2_32() {
			if(FromVersion<new Version("14.2.32.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.2.32");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('FamPhiAccess','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'FamPhiAccess','1')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '14.2.32.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_3_1();
		}

		private static void To14_3_1() {
			if(FromVersion<new Version("14.3.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.3.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO ehrprovkey (LName,FName,YearValue,ProvKey)"
					+" SELECT provider.LName,provider.FName,14,provider.EhrKey"
					+" FROM provider WHERE provider.EhrKey!=''";
					Db.NonQ(command);
				}
				command="ALTER TABLE provider DROP COLUMN EhrKey";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE provider ADD IsInstructor tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE provider ADD IsInstructor number(3)";
					Db.NonQ(command);
					command="UPDATE provider SET IsInstructor = 0 WHERE IsInstructor IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE provider MODIFY IsInstructor NOT NULL";
					Db.NonQ(command);
				}
				command="SELECT ValueString FROM preference WHERE PrefName = 'EasyHideDentalSchools'";
				string valueString=Db.GetScalar(command);
				if(valueString=="0") {//Works for Oracle as well.
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="SELECT provider.ProvNum FROM provider "
					+"INNER JOIN userod ON provider.ProvNum=userod.ProvNum "
					+"INNER JOIN usergroup ON userod.UserGroupNum=usergroup.UserGroupNum "
					+"INNER JOIN grouppermission ON grouppermission.UserGroupNum=usergroup.UserGroupNum "
					+"WHERE grouppermission.PermType=8";//Permission - Setup
						DataTable dt=Db.GetTable(command);
						StringBuilder sb=new StringBuilder();
						for(int i=0;i<dt.Rows.Count;i++) {
							sb.Append("UPDATE provider SET IsInstructor = 1 WHERE provider.ProvNum="+POut.Long((long)dt.Rows[i][0])+";");
						}
						try {
							Db.NonQ(sb.ToString());
						}
						catch(Exception ex) {
							ex.DoNothing();
							//In the rare case that the StringBuilder is too large for the MySQL connector (very rare) we don't want the convert script to fail.
							//Users can go manually set IsInstructor after the upgrade finishes.
						}
					}
					else {//oracle
						//Oracle does not allow calling multiple queries in one call. We are skipping adding this permission for Oracle users. They can still manually add this permission.
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('SecurityGroupForStudents','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SecurityGroupForStudents','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('SecurityGroupForInstructors','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SecurityGroupForInstructors','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS evaluation";
					Db.NonQ(command);
					command=@"CREATE TABLE evaluation (
						EvaluationNum bigint NOT NULL auto_increment PRIMARY KEY,
						InstructNum bigint NOT NULL,
						StudentNum bigint NOT NULL,
						SchoolCourseNum bigint NOT NULL,
						EvalTitle varchar(255) NOT NULL,
						DateEval date NOT NULL DEFAULT '0001-01-01',
						GradingScaleNum bigint NOT NULL,
						OverallGradeShowing varchar(255) NOT NULL,
						OverallGradeNumber float NOT NULL,
						Notes text NOT NULL,
						INDEX(InstructNum),
						INDEX(StudentNum),
						INDEX(SchoolCourseNum),
						INDEX(GradingScaleNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE evaluation'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE evaluation (
						EvaluationNum number(20) NOT NULL,
						InstructNum number(20) NOT NULL,
						StudentNum number(20) NOT NULL,
						SchoolCourseNum number(20) NOT NULL,
						EvalTitle varchar2(255),
						DateEval date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						GradingScaleNum number(20) NOT NULL,
						OverallGradeShowing varchar2(255),
						OverallGradeNumber number(38,8) NOT NULL,
						Notes varchar2(2000),
						CONSTRAINT evaluation_EvaluationNum PRIMARY KEY (EvaluationNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX evaluation_InstructNum ON evaluation (InstructNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX evaluation_StudentNum ON evaluation (StudentNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX evaluation_SchoolCourseNum ON evaluation (SchoolCourseNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX evaluation_GradingScaleNum ON evaluation (GradingScaleNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS evaluationcriterion";
					Db.NonQ(command);
					command=@"CREATE TABLE evaluationcriterion (
						EvaluationCriterionNum bigint NOT NULL auto_increment PRIMARY KEY,
						EvaluationNum bigint NOT NULL,
						CriterionDescript varchar(255) NOT NULL,
						IsCategoryName tinyint NOT NULL,
						GradingScaleNum bigint NOT NULL,
						GradeShowing varchar(255) NOT NULL,
						GradeNumber float NOT NULL,
						Notes text NOT NULL,
						ItemOrder int NOT NULL,
						INDEX(EvaluationNum),
						INDEX(GradingScaleNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE evaluationcriterion'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE evaluationcriterion (
						EvaluationCriterionNum number(20) NOT NULL,
						EvaluationNum number(20) NOT NULL,
						CriterionDescript varchar(255) NOT NULL,
						IsCategoryName number(3) NOT NULL,
						GradingScaleNum number(20) NOT NULL,
						GradeShowing varchar2(255),
						GradeNumber number(38,8) NOT NULL,
						Notes varchar2(2000),
						ItemOrder number(11) NOT NULL,
						CONSTRAINT evaluationcriterion_Evaluation PRIMARY KEY (EvaluationCriterionNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX evaluationcriterion_EvalNum ON evaluationcriterion (EvaluationNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX evaluationcriterion_GradingSca ON evaluationcriterion (GradingScaleNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS evaluationcriteriondef";
					Db.NonQ(command);
					command=@"CREATE TABLE evaluationcriteriondef (
						EvaluationCriterionDefNum bigint NOT NULL auto_increment PRIMARY KEY,
						EvaluationDefNum bigint NOT NULL,
						CriterionDescript varchar(255) NOT NULL,
						IsCategoryName tinyint NOT NULL,
						GradingScaleNum bigint NOT NULL,
						ItemOrder int NOT NULL,
						INDEX(EvaluationDefNum),
						INDEX(GradingScaleNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE evaluationcriteriondef'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE evaluationcriteriondef (
						EvaluationCriterionDefNum number(20) NOT NULL,
						EvaluationDefNum number(20) NOT NULL,
						CriterionDescript varchar(255) NOT NULL,
						IsCategoryName number(3) NOT NULL,
						GradingScaleNum number(20) NOT NULL,
						ItemOrder number(11) NOT NULL,
						CONSTRAINT evaluationcriteriondef_Evaluat PRIMARY KEY (EvaluationCriterionDefNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX evaluationcriteriondef_EvalDef ON evaluationcriteriondef (EvaluationDefNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX evaluationcriteriondef_Grading ON evaluationcriteriondef (GradingScaleNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS evaluationdef";
					Db.NonQ(command);
					command=@"CREATE TABLE evaluationdef (
						EvaluationDefNum bigint NOT NULL auto_increment PRIMARY KEY,
						SchoolCourseNum bigint NOT NULL,
						EvalTitle varchar(255) NOT NULL,
						GradingScaleNum bigint NOT NULL,
						INDEX(SchoolCourseNum),
						INDEX(GradingScaleNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE evaluationdef'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE evaluationdef (
						EvaluationDefNum number(20) NOT NULL,
						SchoolCourseNum number(20) NOT NULL,
						EvalTitle varchar2(255),
						GradingScaleNum number(20) NOT NULL,
						CONSTRAINT evaluationdef_EvaluationDefNum PRIMARY KEY (EvaluationDefNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX evaluationdef_SchoolCourseNum ON evaluationdef (SchoolCourseNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX evaluationdef_GradingScaleNum ON evaluationdef (GradingScaleNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS gradingscale";
					Db.NonQ(command);
					command=@"CREATE TABLE gradingscale (
						GradingScaleNum bigint NOT NULL auto_increment PRIMARY KEY,
						Description varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE gradingscale'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE gradingscale (
						GradingScaleNum number(20) NOT NULL,
						Description varchar2(255),
						CONSTRAINT gradingscale_GradingScaleNum PRIMARY KEY (GradingScaleNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS gradingscaleitem";
					Db.NonQ(command);
					command=@"CREATE TABLE gradingscaleitem (
						GradingScaleItemNum bigint NOT NULL auto_increment PRIMARY KEY,
						GradingScaleNum bigint NOT NULL,
						GradeShowing varchar(255) NOT NULL,
						GradeNumber float NOT NULL,
						Description varchar(255) NOT NULL,
						INDEX(GradingScaleNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE gradingscaleitem'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE gradingscaleitem (
						GradingScaleItemNum number(20) NOT NULL,
						GradingScaleNum number(20) NOT NULL,
						GradeShowing varchar2(255),
						GradeNumber number(38,8) NOT NULL,
						Description varchar2(255),
						CONSTRAINT gradingscaleitem_GradingScaleI PRIMARY KEY (GradingScaleItemNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX gradingscaleitem_GradingScaleN ON gradingscaleitem (GradingScaleNum)";
					Db.NonQ(command);
				}
				//Add OrthoChartEdit permission to everyone------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				DataTable table=Db.GetTable(command);
				long groupNum;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",79)";
						Db.NonQ(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",79)";
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE provider ADD EhrMuStage int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE provider ADD EhrMuStage number(3)";
					Db.NonQ(command);
					command="UPDATE provider SET EhrMuStage = 0 WHERE EhrMuStage IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE provider MODIFY EhrMuStage NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claimproc ADD PayPlanNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimproc ADD INDEX (PayPlanNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claimproc ADD PayPlanNum number(20)";
					Db.NonQ(command);
					command="UPDATE claimproc SET PayPlanNum = 0 WHERE PayPlanNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimproc MODIFY PayPlanNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX claimproc_PayPlanNum ON claimproc (PayPlanNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE etrans ADD TranSetId835 varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE etrans ADD TranSetId835 varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE evaluationcriteriondef ADD MaxPointsPoss float NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE evaluationcriteriondef ADD MaxPointsPoss number(38,8)";
					Db.NonQ(command);
					command="UPDATE evaluationcriteriondef SET MaxPointsPoss = 0 WHERE MaxPointsPoss IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE evaluationcriteriondef MODIFY MaxPointsPoss NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE gradingscale ADD ScaleType tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE gradingscale ADD ScaleType number(3)";
					Db.NonQ(command);
					command="UPDATE gradingscale SET ScaleType = 0 WHERE ScaleType IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE gradingscale MODIFY ScaleType NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE evaluationcriterion ADD MaxPointsPoss float NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE evaluationcriterion ADD MaxPointsPoss number(38,8)";
					Db.NonQ(command);
					command="UPDATE evaluationcriterion SET MaxPointsPoss = 0 WHERE MaxPointsPoss IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE evaluationcriterion MODIFY MaxPointsPoss NOT NULL";
					Db.NonQ(command);
				}
				//Insert HandyDentist bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"'HandyDentist', "
				    +"'HandyDentist from handycreate.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\HandyDentist\HandyDentist.exe")+"', "
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"'"+POut.Long(programNum)+"', "
				    +"'2', "//ToolBarsAvail.ChartModule
				    +"'HandyDentist')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'HandyDentist', "
				    +"'HandyDentist from handycreate.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\HandyDentist\HandyDentist.exe")+"', "
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'2', "//ToolBarsAvail.ChartModule
				    +"'HandyDentist')";
					Db.NonQ(command);
				}//end HandyDentist bridge
				//Insert XVWeb bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'XVWeb', "
						+"'XVWeb from www.apteryx.com/xvweb', "
						+"'0', "
						+"'', "
						+"'', "//leave blank if none
						+"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Enter desired URL address for XVWeb', "
						+"'https://demo2.apteryxweb.com/')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"'"+POut.Long(programNum)+"', "
						+"'2', "//ToolBarsAvail.ChartModule
						+"'XVWeb')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'XVWeb', "
						+"'XVWeb from www.apteryx.com/xvweb', "
						+"'0', "
						+"'', "
						+"'', "//leave blank if none
						+"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Enter desired URL address for XVWeb', "
						+"'https://demo2.apteryxweb.com/')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
						+"'"+POut.Long(programNum)+"', "
						+"'2', "//ToolBarsAvail.ChartModule
						+"'XVWeb')";
					Db.NonQ(command);
				}//end XVWeb bridge
				//Insert VixWinBase36 bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'VixWinBase36', "
						+"'VixWin(Base36) from www.gendexxray.com', "
						+"'0', "
						+"'"+POut.String(@"C:\VixWin\VixWin.exe")+"',"
						+"'', "
						+"'This VixWin bridge uses base 36 PatNums.')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+POut.Long(programNum)+", "
						+"'Image Path', "
						+"'')";//User will be required to set up image path before using bridge. If they try to use it they will get a warning message and the bridge will fail gracefully.
					Db.NonQ32(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+POut.Long(programNum)+", "
						+"'2', "//ToolBarsAvail.ChartModule
						+"'VixWinBase36')";
					Db.NonQ32(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'VixWinBase36', "
						+"'VixWin(Base36) from www.gendexxray.com', "
						+"'0', "
						+"'"+POut.String(@"C:\VixWin\VixWin.exe")+"',"
						+"'', "
						+"'This VixWin bridge uses base 36 PatNums.')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),"
						+POut.Long(programNum)+", "
						+"'Image Path', "
						+"'')";//User will be required to set up image path before using bridge. If they try to use it they will get a warning message and the bridge will fail gracefully.
					Db.NonQ32(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
						+POut.Long(programNum)+", "
						+"'2', "//ToolBarsAvail.ChartModule
						+"'VixWinBase36')";
					Db.NonQ32(command);
				}//end VixWinBase36 bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD Discount double NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD Discount number(38,8)";
					Db.NonQ(command);
					command="UPDATE procedurelog SET Discount = 0 WHERE Discount IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog MODIFY Discount NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('TreatPlanDiscountPercent','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'TreatPlanDiscountPercent','0')";
					Db.NonQ(command);
				}
				//Make sure the database has a "Discount" definition because we are adding a new feature that needs a default discount adjustment type.
				command="SELECT DefNum,IsHidden FROM definition WHERE Category=1 AND ItemName='Discount'";//1 - AdjTypes
				table=Db.GetTable(command);
				if(table.Rows.Count==0) {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO definition(Category,ItemName,ItemValue) VALUES(1,'Discount','-')";//1 - AdjTypes, ItemValue of '-' makes it a subtraction type.	
						long defNum=Db.NonQ(command);
						table.Rows.Add(defNum,0);
					}
					else {
						command="INSERT INTO definition(DefNum,Category,ItemOrder,ItemColor,ItemName,ItemValue,IsHidden) VALUES((SELECT MAX(DefNum)+1,1,SELECT MAX(ItemOrder)+1,0,'Discount','-',0)";//1 - AdjTypes, ItemValue of '-' makes it a subtraction type.	
						long defNum=Db.NonQ(command);
						table.Rows.Add(defNum,0);
					}
				}
				else {
					if(table.Rows[0]["IsHidden"].ToString()=="1") {
						command="UPDATE definition SET IsHidden=0 WHERE DefNum='"+table.Rows[0]["DefNum"]+"'";
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('TreatPlanDiscountAdjustmentType','"+table.Rows[0]["DefNum"]+"')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'TreatPlanDiscountAdjustmentType','"+table.Rows[0]["DefNum"]+"')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT usergroup.UserGroupNum FROM usergroup "
					+"INNER JOIN grouppermission ON grouppermission.UserGroupNum=usergroup.UserGroupNum "
					+"WHERE grouppermission.PermType=17";//Permission - AdjustmentCreate
					DataTable dts=Db.GetTable(command);
					StringBuilder sbs=new StringBuilder();
					for(int i=0;i<dts.Rows.Count;i++) {
						sbs.Append("INSERT INTO grouppermission(UserGroupNum,PermType) VALUES("+POut.Long((long)dts.Rows[i][0])+",82);");//Permission - TreatPlanDiscountEdit
					}
					try {
						Db.NonQ(sbs.ToString());
					}
					catch(Exception ex) {
						ex.DoNothing();
						//In the rare case that the StringBuilder is too large for the MySQL connector (very rare) we don't want the convert script to fail.
						//Users can go manually set IsInstructor after the upgrade finishes.
					}
				}
				else {//oracle
					//Oracle does not allow calling multiple queries in one call. We are skipping adding this permission for Oracle users. They can still manually add this permission.
				}
				//Insert AudaxCeph bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'AudaxCeph', "
						+"'AudaxCeph from www.audaxceph.com', "
						+"'0', "
						+"'"+POut.String(@"C:\AudaxCeph\AxCeph.exe")+"',"
						+"'', "
						+"'AudaxCeph needs to be running in the background for the bridge to work.')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+POut.Long(programNum)+", "
						+"'2', "//ToolBarsAvail.ChartModule
						+"'AudaxCeph')";
					Db.NonQ32(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'AudaxCeph', "
						+"'AudaxCeph from www.audaxceph.com', "
						+"'0', "
						+"'"+POut.String(@"C:\AudaxCeph\AxCeph.exe")+"',"
						+"'', "
						+"'AudaxCeph needs to be running in the background for the bridge to work.')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
						+POut.Long(programNum)+", "
						+"'2', "//ToolBarsAvail.ChartModule
						+"'AudaxCeph')";
					Db.NonQ32(command);
				}//end AudaxCeph bridge
				//Insert PandaPerio bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"'PandaPerio', "
				    +"'PandaPerio from www.pandaperio.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\Program Files (x86)\Panda Perio\Panda.exe")+"', "
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"'"+POut.Long(programNum)+"', "
				    +"'2', "//ToolBarsAvail.ChartModule
				    +"'PandaPerio')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'PandaPerio', "
				    +"'PandaPerio from www.pandaperio.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\Program Files (x86)\Panda Perio\Panda.exe")+"', "
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'2', "//ToolBarsAvail.ChartModule
				    +"'PandaPerio')";
					Db.NonQ(command);
				}//end PandaPerio bridge
				//Insert DemandForce bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"'DemandForce', "
				    +"'DemandForce from www.demandforce.com', "
				    +"'0', "
				    +"'"+POut.String(@"d3one.exe")+"', "
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter your DemandForce license key (required)', "
				    +"'')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"'"+POut.Long(programNum)+"', "
				    +"'7', "//ToolBarsAvail.ChartModule
				    +"'DemandForce')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'DemandForce', "
				    +"'DemandForce from www.demandforce.com', "
				    +"'0', "
				    +"'"+POut.String(@"d3one.exe")+"', "
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter your DemandForce license key (required)', "
				    +"'')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'7', "//ToolBarsAvail.ChartModule
				    +"'DemandForce')";
					Db.NonQ(command);
				}//end DemandForce bridge
				//Update any text based columns that are not CLOBs to allow NULL entries.  This is because Oracle treats empty strings as NULLs.
				if(DataConnection.DBtype==DatabaseType.Oracle) {
					command=@"SELECT TABLE_NAME, COLUMN_NAME 
						FROM USER_TAB_COLUMNS 
						WHERE NULLABLE='N' AND DATA_TYPE LIKE '%CHAR%'";
					table=Db.GetTable(command);
					for(int i=0;i<table.Rows.Count;i++) {
						command="ALTER TABLE "+table.Rows[i]["TABLE_NAME"].ToString()+" MODIFY("+table.Rows[i]["COLUMN_NAME"].ToString()+" NULL)";
						try {
							Db.NonQ(command);
						}
						catch {
							//This will only cause issues if the user tries to insert empty string into a NOT NULL text based column.
							//Therefore, I'd rather the failure happen within the program instead of here in the convert script.
						}
					}
				}
				//Added ReplicationUserQueryServer preference to stop CREATE TABLE or DROP TABLE user queries from being ran on any computer that is not the ReplicationUserQueryServer.
				//This is set in the Replication Setup Window.  Defaults to empty string.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ReplicationUserQueryServer','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ReplicationUserQueryServer','')";
					Db.NonQ(command);
				}
				//Insert iRYS NNTBridge bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"'iRYS', "
				    +"'iRYS from www.cefladental.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\NNT\NNTBridge.exe")+"', "
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"'"+POut.Long(programNum)+"', "
				    +"'2', "//ToolBarsAvail.ChartModule
				    +"'iRYS')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'iRYS', "
				    +"'iRYS from www.cefladental.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\NNT\NNTBridge.exe")+"', "
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'2', "//ToolBarsAvail.ChartModule
				    +"'iRYS')";
					Db.NonQ(command);
				}//end iRYS bridge
				//Insert visOra bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"'visOra', "
				    +"'visOra from www.visoraimaging.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\Program Files\Cieos\Cieos Workstation\Cieos.Workstation.Shell.exe")+"', "
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"'"+POut.Long(programNum)+"', "
				    +"'2', "//ToolBarsAvail.ChartModule
				    +"'visOra')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'visOra', "
				    +"'visOra from www.visoraimaging.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\Program Files\Cieos\Cieos Workstation\Cieos.Workstation.Shell.exe")+"', "
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'2', "//ToolBarsAvail.ChartModule
				    +"'visOra')";
					Db.NonQ(command);
				}//end visOra bridge
				//Insert Z-Image bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"'Z-Image', "
				    +"'Z-Image from www.visoraimaging.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\Program Files\Zuma\Zuma Workstation\Zuma.Workstation.Shell.exe")+"', "
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"'"+POut.Long(programNum)+"', "
				    +"'2', "//ToolBarsAvail.ChartModule
				    +"'Z-Image')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'Z-Image', "
				    +"'Z-Image from www.visoraimaging.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\Program Files\Zuma\Zuma Workstation\Zuma.Workstation.Shell.exe")+"', "
				    +"'', "
				    +"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'2', "//ToolBarsAvail.ChartModule
				    +"'Z-Image')";
					Db.NonQ(command);
				}//end Z-Image bridge
				//Add option to hide Rx buttons in Chart Module for eClinicalWorks
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ProgramNum FROM program WHERE ProgName='eClinicalWorks'";
					long programNum=PIn.Long(Db.GetScalar(command));
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
							+") VALUES("
							+"'"+POut.Long(programNum)+"', "
							+"'HideChartRxButtons', "
							+"'0')";//set to 0 (false) by default so behavior of existing customers will not change
					Db.NonQ(command);
				}
				else {//oracle
					//eCW will never use Oracle.
				}
				//Added AccountShowQuestionnaire preference to show Questionnaire button in account module.  This is set in the Show Features window.  Defaults to false.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('AccountShowQuestionnaire','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AccountShowQuestionnaire','0')";
					Db.NonQ(command);
				}
				//Added AccountShowTrojanExpressCollect preference to show TrojanCollect button in account module.  This is set in the Show Features window.  Defaults to false.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('AccountShowTrojanExpressCollect','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AccountShowTrojanExpressCollect','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE userod ADD InboxHidePopups tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE userod ADD InboxHidePopups number(3)";
					Db.NonQ(command);
					command="UPDATE userod SET InboxHidePopups = 0 WHERE InboxHidePopups IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE userod MODIFY InboxHidePopups NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ChartNonPatientWarn','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ChartNonPatientWarn','1')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('TreatPlanItemized','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'TreatPlanItemized','1')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS procbuttonquick";
					Db.NonQ(command);
					command=@"CREATE TABLE procbuttonquick (
						ProcButtonQuickNum bigint NOT NULL auto_increment PRIMARY KEY,
						Description varchar(255) NOT NULL,
						CodeValue varchar(255) NOT NULL,
						Surf varchar(255) NOT NULL,
						YPos int NOT NULL,
						ItemOrder int NOT NULL,
						IsLabel tinyint NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE procbuttonquick'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE procbuttonquick (
						ProcButtonQuickNum number(20) NOT NULL,
						Description varchar2(255),
						CodeValue varchar2(255),
						Surf varchar2(255),
						YPos number(11) NOT NULL,
						ItemOrder number(11) NOT NULL,
						IsLabel number(3) NOT NULL,
						CONSTRAINT procbuttonquick_ProcButtonQuic PRIMARY KEY (ProcButtonQuickNum)
						)";
					Db.NonQ(command);
				}
				//Fill ProckButton Quick with buttons to emulate current behavior with new ODButtonPanel
				//MySQL and Oracle compatible
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (1,'Post. Composite','','',0,0,1)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (2,'MO','D2392','MO',0,1,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (3,'MOD','D2393','MOD',1,1,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (4,'O','D2391','O',2,1,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (5,'DO','D2392','DO',3,1,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (7,'        ','','',4,1,1)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (6,'Seal','D1351','',5,1,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (8,'OL','D2392','OL',0,2,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (9,'OB','D2392','OB',1,2,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (10,'MODL','D2394','MODL',2,2,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (11,'MODB','D2394','MODB',3,2,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (12,'Ant. Composite','','',0,4,1)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (13,'DL','D2331','',0,5,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (14,'MDL','D2332','',1,5,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (15,'ML','D2331','',2,5,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (16,'Amalgam','','',0,7,1)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (17,'MO','D2150','MO',0,8,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (18,'MOD','D2160','MOD',1,8,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (19,'O','D2140','O',2,8,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (20,'DO','D2150','DO',3,8,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (21,'OL','D2150','OL',0,9,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (22,'OB','D2150','OB',1,9,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (23,'MODL','D2161','MODL',2,9,0)";
				Db.NonQ(command);
				command="INSERT INTO procbuttonquick (ProcButtonQuickNum, Description, CodeValue, Surf, ItemOrder, YPos, IsLabel) VALUES (24,'MODB','D2161','MODB',3,9,0)";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sheetdef ADD PageCount int NOT NULL";
					Db.NonQ(command);
					command="UPDATE sheetdef SET PageCount = 1";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sheetdef ADD PageCount number(11)";
					Db.NonQ(command);
					command="UPDATE sheetdef SET PageCount = 1 WHERE PageCount IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE sheetdef MODIFY PageCount NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE hl7defmessage ADD MessageStructure varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE hl7defmessage ADD MessageStructure varchar2(255)";
					Db.NonQ(command);
				}
				//Oracle compatible
				command="UPDATE hl7defmessage SET MessageStructure='ADT_A01' WHERE EventType='A04'";//All ADT's and ACK messages are event type A04 in the db
				Db.NonQ(command);
				command="UPDATE hl7defmessage SET MessageStructure='SIU_S12' WHERE EventType='S12'";//All SIU's are event type S12 in the db
				Db.NonQ(command);
				command="UPDATE hl7defmessage SET MessageStructure='DFT_P03' WHERE EventType='P03'";//All DFT's are event type P03 in the db
				Db.NonQ(command);
				command="UPDATE hl7defmessage SET MessageStructure='NotDefined' WHERE EventType='NotDefined' OR EventType=''";//Any messages with NotDefined or blank event type
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE appointment ADD AppointmentTypeNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE appointment ADD INDEX (AppointmentTypeNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE appointment ADD AppointmentTypeNum number(20)";
					Db.NonQ(command);
					command="UPDATE appointment SET AppointmentTypeNum = 0 WHERE AppointmentTypeNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE appointment MODIFY AppointmentTypeNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX appointment_AppointmentTypeNum ON appointment (AppointmentTypeNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS appointmenttype";
					Db.NonQ(command);
					command=@"CREATE TABLE appointmenttype (
						AppointmentTypeNum bigint NOT NULL auto_increment PRIMARY KEY,
						AppointmentTypeName varchar(255) NOT NULL,
						AppointmentTypeColor int NOT NULL,
						ItemOrder int NOT NULL,
						IsHidden tinyint NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE appointmenttype'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE appointmenttype (
						AppointmentTypeNum number(20) NOT NULL,
						AppointmentTypeName varchar2(255),
						AppointmentTypeColor number(11) NOT NULL,
						ItemOrder number(11) NOT NULL,
						IsHidden number(3) NOT NULL,
						CONSTRAINT appointmenttype_AppointmentTyp PRIMARY KEY (AppointmentTypeNum)
						)";
					Db.NonQ(command);
				}
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE etrans ADD INDEX (etransmessagetextnum)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX etrans_etransmessagetextnum ON etrans (etransmessagetextnum)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				command="UPDATE preference SET ValueString = '14.3.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_3_3();
		}

		private static void To14_3_3() {
			if(FromVersion<new Version("14.3.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.3.3");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sheet ADD IsMultiPage tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sheet ADD IsMultiPage number(3)";
					Db.NonQ(command);
					command="UPDATE sheet SET IsMultiPage = 0 WHERE IsMultiPage IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE sheet MODIFY IsMultiPage NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sheetdef ADD IsMultiPage tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sheetdef ADD IsMultiPage number(3)";
					Db.NonQ(command);
					command="UPDATE sheetdef SET IsMultiPage = 0 WHERE IsMultiPage IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE sheetdef MODIFY IsMultiPage NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sheetfield ADD INDEX (FieldType)";
					Db.NonQ(command);
				}
				else {//oracle
					command=@"CREATE INDEX sheetfield_FieldType ON sheetfield (FieldType)";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '14.3.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_3_4();
		}

		private static void To14_3_4() {
			if(FromVersion<new Version("14.3.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.3.4");//No translation in convert script.
				string command;
				//adding EmailNotifyAddressNum preference
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ValueString FROM preference WHERE PrefName='EmailDefaultAddressNum' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT * FROM (SELECT ValueString FROM preference WHERE PrefName='EmailDefaultAddressNum') WHERE RowNum<=1";
				}
				long emailDefaultAddressNum=PIn.Long(Db.GetScalar(command));
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailNotifyAddressNum','"+POut.Long(emailDefaultAddressNum)+"')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'EmailNotifyAddressNum','"+POut.Long(emailDefaultAddressNum)+"')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '14.3.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_3_9();
		}

		///<summary>Oracle compatible: 10/08/2014</summary>
		private static void To14_3_9() {
			if(FromVersion<new Version("14.3.9.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.3.9");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT COUNT(*) FROM preference WHERE PrefName='FamPhiAccess' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT COUNT(*) FROM (SELECT ValueString FROM preference WHERE PrefName='FamPhiAccess') WHERE RowNum<=1";
				}
				long hasFamPhiAccess=PIn.Long(Db.GetCount(command));
				if(hasFamPhiAccess==0) {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO preference(PrefName,ValueString) VALUES('FamPhiAccess','1')";
						Db.NonQ(command);
					}
					else {//oracle
						command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'FamPhiAccess','1')";
						Db.NonQ(command);
					}
				}
				command="UPDATE preference SET ValueString = '14.3.9.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_3_12();
		}

		///<summary>Oracle compatible: 10/10/2014</summary>
		private static void To14_3_12() {
			if(FromVersion<new Version("14.3.12.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.3.12");//No translation in convert script.
				string command;
				//Moving codes to the Obsolete category that were deleted in CDT 2015.
				if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
					//Move depricated codes to the Obsolete procedure code category.
					//Make sure the procedure code category exists before moving the procedure codes.
					string procCatDescript="Obsolete";
					long defNum=0;
					command="SELECT DefNum FROM definition WHERE Category=11 AND ItemName='"+POut.String(procCatDescript)+"'";//11 is DefCat.ProcCodeCats
					DataTable dtDef=Db.GetTable(command);
					if(dtDef.Rows.Count==0) { //The procedure code category does not exist, add it
						command="SELECT COUNT(*) FROM definition WHERE Category=11";//11 is DefCat.ProcCodeCats
						string itemOrder=Db.GetCount(command);
						if(DataConnection.DBtype==DatabaseType.MySql) {
							command="INSERT INTO definition (Category,ItemName,ItemOrder) "
									+"VALUES (11"+",'"+POut.String(procCatDescript)+"',"+itemOrder+")";//11 is DefCat.ProcCodeCats
						}
						else {//oracle
							command="INSERT INTO definition (DefNum,Category,ItemName,ItemOrder) "
									+"VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),11,'"+POut.String(procCatDescript)+"',"+itemOrder+")";//11 is DefCat.ProcCodeCats
						}
						defNum=Db.NonQ(command,true,"DefNum","definition");
					}
					else { //The procedure code category already exists, get the existing defnum
						defNum=PIn.Long(dtDef.Rows[0]["DefNum"].ToString());
					}
					string[] arrayCdtCodesDeleted=new string[] {
						"D6053","D6054","D6078","D6079","D6975"
					};
					for(int i=0;i<arrayCdtCodesDeleted.Length;i++) {
						string procCode=arrayCdtCodesDeleted[i];
						command="SELECT CodeNum FROM procedurecode WHERE ProcCode='"+POut.String(procCode)+"'";
						DataTable dtProcCode=Db.GetTable(command);
						if(dtProcCode.Rows.Count==0) { //The procedure code does not exist in this database.
							continue;//Do not try to move it.
						}
						long codeNum=PIn.Long(dtProcCode.Rows[0]["CodeNum"].ToString());
						command="UPDATE procedurecode SET ProcCat="+POut.Long(defNum)+" WHERE CodeNum="+POut.Long(codeNum);
						Db.NonQ(command);
					}
				}//end United States update
				command="UPDATE preference SET ValueString = '14.3.12.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_3_24();
		}

		///<summary></summary>
		private static void To14_3_24() {
			if(FromVersion<new Version("14.3.24.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.3.24");//No translation in convert script.
				string command="";
				//Bug fix for missing surfaces on quickbuttons. This should only affect quick buttons if they have not been edited by users.
				command=@"UPDATE procbuttonquick SET Surf='DL' WHERE ProcButtonQuickNum=13 AND Description='DL'AND CodeValue='D2331' AND Surf=''";
				Db.NonQ(command);
				command=@"UPDATE procbuttonquick SET Surf='MDL' WHERE ProcButtonQuickNum=13 AND Description='MDL'AND CodeValue='D2332' AND Surf=''";
				Db.NonQ(command);
				command=@"UPDATE procbuttonquick SET Surf='ML' WHERE ProcButtonQuickNum=13 AND Description='ML'AND CodeValue='D2331' AND Surf=''";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '14.3.24.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_3_30();
		}

		///<summary></summary>
		private static void To14_3_30() {
			if(FromVersion<new Version("14.3.30.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.3.30");//No translation in convert script.
				string command="";
				//Inline DBM to remove old signals.  This query is run regularly as of version 15.1.  This is here to tide user over until they update to version 15.1.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DELETE FROM signalod WHERE SigType = 1 AND SigDateTime < DATE_ADD(NOW(),INTERVAL -2 DAY)";//Itypes only older than 2 days
					Db.NonQ(command);
					command="DELETE FROM signalod WHERE SigType = 0 AND AckTime != '0001-01-01' AND SigDateTime < DATE_ADD(NOW(),INTERVAL -2 DAY)";//Only unacknowledged buttons older than 2 days
					Db.NonQ(command);
				}
				else {//oracle
					command="DELETE FROM signalod WHERE SigType = 1 AND SigDateTime < CURRENT_TIMESTAMP -2";//Itypes only older than 2 days
					Db.NonQ(command);
					command="DELETE FROM signalod WHERE SigType = 0 AND AckTime != TO_DATE('0001-01-01','YYYY-MM-DD') AND SigDateTime < CURRENT_TIMESTAMP -2";//Only unacknowledged buttons older than 2 days
					Db.NonQ(command);
				}
				//The ReplicationUserQueryServer preference used to store the "case insensitive computer name" of one singular computer.
				//When using replication, that one computer was designated as the ONLY computer that could run dangerous user queries.
				//Nathan qualified this as a bug because it was not good enough for one of our large customers.  We instead need to have the preference store the Repliction Server PK.
				//This way, ANY computer connected to the "report server" can run dangerous user queries.
				command="UPDATE preference SET ValueString = '0' WHERE PrefName = 'ReplicationUserQueryServer'";
				Db.NonQ(command);//Simply clear out the old computer name because there is no way we can guess which specific database is the "report server" based on a computer name.
				command="UPDATE preference SET ValueString = '14.3.30.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_3_35();
		}

		///<summary></summary>
		private static void To14_3_35() {
			if(FromVersion<new Version("14.3.35.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.3.35");//No translation in convert script.
				string command="";
				//AppointmentBubblesNoteLength also inserted into version 15.1.13
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('AppointmentBubblesNoteLength','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AppointmentBubblesNoteLength','0')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '14.3.35.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To14_3_37();
		}

		///<summary></summary>
		private static void To14_3_37() {
			if(FromVersion<new Version("14.3.37.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 14.3.37");//No translation in convert script.
				string command="";
				//InsPPOsecWriteoffs also inserted into version 15.1
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('InsPPOsecWriteoffs','0')";
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'InsPPOsecWriteoffs','0')";
				}
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '14.3.37.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_1_1();
		}

		private static void To15_1_1() {
			if(FromVersion<new Version("15.1.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.1.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS dispsupply";
					Db.NonQ(command);
					command=@"CREATE TABLE dispsupply (
						DispSupplyNum bigint NOT NULL auto_increment PRIMARY KEY,
						SupplyNum bigint NOT NULL,
						ProvNum bigint NOT NULL,
						DateDispensed date NOT NULL DEFAULT '0001-01-01',
						DispQuantity float NOT NULL,
						Note text NOT NULL,
						INDEX(SupplyNum),
						INDEX(ProvNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE dispsupply'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE dispsupply (
						DispSupplyNum number(20) NOT NULL,
						SupplyNum number(20) NOT NULL,
						ProvNum number(20) NOT NULL,
						DateDispensed date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DispQuantity number(38,8) NOT NULL,
						Note clob,
						CONSTRAINT dispsupply_DispSupplyNum PRIMARY KEY (DispSupplyNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX dispsupply_SupplyNum ON dispsupply (SupplyNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX dispsupply_ProvNum ON dispsupply (ProvNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE equipment ADD ProvNumCheckedOut bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE equipment ADD INDEX (ProvNumCheckedOut)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE equipment ADD ProvNumCheckedOut number(20)";
					Db.NonQ(command);
					command="UPDATE equipment SET ProvNumCheckedOut = 0 WHERE ProvNumCheckedOut IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE equipment MODIFY ProvNumCheckedOut NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX equipment_ProvNumCheckedOut ON equipment (ProvNumCheckedOut)";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE equipment ADD DateCheckedOut date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE equipment ADD DateCheckedOut date";
					Db.NonQ(command);
					command="UPDATE equipment SET DateCheckedOut = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateCheckedOut IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE equipment MODIFY DateCheckedOut NOT NULL";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE equipment ADD DateExpectedBack date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE equipment ADD DateExpectedBack date";
					Db.NonQ(command);
					command="UPDATE equipment SET DateExpectedBack = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateExpectedBack IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE equipment MODIFY DateExpectedBack NOT NULL";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE equipment ADD DispenseNote text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE equipment ADD DispenseNote clob";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE supply ADD BarCodeOrID varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE supply ADD BarCodeOrID varchar2(255)";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE supply ADD DispDefaultQuant float NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE supply ADD DispDefaultQuant number(38,8)";
					Db.NonQ(command);
					command="UPDATE supply SET DispDefaultQuant = 0 WHERE DispDefaultQuant IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE supply MODIFY DispDefaultQuant NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE supply ADD DispUnitsCount int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE supply ADD DispUnitsCount number(11)";
					Db.NonQ(command);
					command="UPDATE supply SET DispUnitsCount = 0 WHERE DispUnitsCount IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE supply MODIFY DispUnitsCount NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE supply ADD DispUnitDesc varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE supply ADD DispUnitDesc varchar2(255)";
					Db.NonQ(command);
				}
				//Add index to claimproc table to speed up ClaimProcs.AttachAllOutstandingToPayment() query.  Added because of slowness with AppleTree.
				//Using new multi-column naming pattern.
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE claimproc ADD INDEX indexCPNSIPA (ClaimPaymentNum, Status, InsPayAmt)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX claimproc_CPNSIPA ON claimproc (ClaimPaymentNum, Status, InsPayAmt)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE screen CHANGE Race RaceOld tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE screen RENAME COLUMN Race TO RaceOld";
					Db.NonQ(command);
				}
				//oracle compatible
				command="ALTER TABLE patient DROP COLUMN Race";
				Db.NonQ(command);
				//				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS wikilisthist";
					Db.NonQ(command);
					command=@"CREATE TABLE wikilisthist (
						WikiListHistNum bigint NOT NULL auto_increment PRIMARY KEY,
						UserNum bigint NOT NULL,
						ListName varchar(255) NOT NULL,
						ListHeaders text NOT NULL,
						ListContent mediumtext NOT NULL,
						DateTimeSaved datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						INDEX(UserNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE wikilisthist'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE wikilisthist (
						WikiListHistNum number(20) NOT NULL,
						UserNum number(20) NOT NULL,
						ListName varchar2(255),
						ListHeaders varchar2(4000),
						ListContent clob,
						DateTimeSaved date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT wikilisthist_WikiListHistNum PRIMARY KEY (WikiListHistNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX wikilisthist_UserNum ON wikilisthist (UserNum)";
					Db.NonQ(command);
				}
				//Insert SMARTDent bridge-----------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'SMARTDent', "
						+"'SMARTDent from www.raymedical.com', "
						+"'0', "
						+"'"+POut.String(@"C:\Ray\RayView\SMARTDent.exe")+"', "
						+"'', "
						+"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"'"+POut.Long(programNum)+"', "
						+"'2', "//ToolBarsAvail.ChartModule
						+"'SMARTDent')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'SMARTDent', "
						+"'SMARTDent from www.raymedical.com', "
						+"'0', "
						+"'"+POut.String(@"C:\Ray\RayView\SMARTDent.exe")+"', "
						+"'', "
						+"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
						+"'"+POut.Long(programNum)+"', "
						+"'2', "//ToolBarsAvail.ChartModule
						+"'SMARTDent')";
					Db.NonQ(command);
				}//end SMARTDent bridge
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("hl7msg","MsgText")) {//only add this index if it has not already been added
							//this could be slow on a very large hl7msg table, however most of the rows in a table will have empty MsgText fields due to old message text being deleted.
							//on a table with 870,000 rows, 145,000 filled with MsgText, with hl7msg.MYD file size of over 9 GB, this query took 10 minutes to run on my local PC
							//on our local test eCW server with 1690 rows, 500 with message text the query took 2 seconds.
							command="ALTER TABLE hl7msg ADD INDEX (MsgText(100))";
							Db.NonQ(command);
						}
					}
					else {//oracle
						//Cannot index a clob column in oracle.  Not likely that an oracle user will also be using HL7.
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ProgramNum FROM program WHERE ProgName='Xcharge' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT * FROM (SELECT ProgramNum FROM program WHERE ProgName='Xcharge') WHERE RowNum<=1";
				}
				long ProgramNum=PIn.Long(Db.GetScalar(command));
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT PropertyValue FROM programproperty WHERE ProgramNum="+POut.Long(ProgramNum)+" AND PropertyDesc='Password' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT * FROM (SELECT PropertyValue FROM programproperty WHERE ProgramNum="+POut.Long(ProgramNum)+" AND PropertyDesc='Password') WHERE RowNum<=1";
				}
				string pw=PIn.String(Db.GetScalar(command));
				command="UPDATE programproperty SET PropertyValue='"+Encrypt(pw)+"' WHERE ProgramNum="+POut.Long(ProgramNum)+" AND PropertyDesc='Password'";//Oracle doesn't have any rescrictions with this query.
				Db.NonQ(command);
				//Web Sched preferences-----------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedService','0')";//Service will be off by default
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WebSchedService','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedSubject','Dental Care Reminder for [NameF]')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WebSchedSubject','Dental Care Reminder for [NameF]')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedMessage','You or your family member is due for a regular dental check-up on [DueDate].  Please visit our online scheduler link below or call our office today at [OfficePhone] in order to schedule your appointment.\r\n[URL]')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WebSchedMessage','You or your family member is due for a regular dental check-up on [DueDate].  Please visit our online scheduler link below or call our office today at [OfficePhone] in order to schedule your appointment.\r\n[URL]')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedSubject2','Dental Care Reminder for [NameF]')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WebSchedSubject2','Dental Care Reminder for [NameF]')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedMessage2','You or your family member is due for a regular dental check-up on [DueDate].  Please visit our online scheduler link below or call our office today at [OfficePhone] in order to schedule your appointment.\r\n[URL]')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WebSchedMessage2','You or your family member is due for a regular dental check-up on [DueDate].  Please visit our online scheduler link below or call our office today at [OfficePhone] in order to schedule your appointment.\r\n[URL]')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedSubject3','Dental Care Reminder for [NameF]')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WebSchedSubject3','Dental Care Reminder for [NameF]')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedMessage3','You or your family member is due for a regular dental check-up on [DueDate].  Please visit our online scheduler link below or call our office today at [OfficePhone] in order to schedule your appointment.\r\n[URL]')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WebSchedMessage3','You or your family member is due for a regular dental check-up on [DueDate].  Please visit our online scheduler link below or call our office today at [OfficePhone] in order to schedule your appointment.\r\n[URL]')";
					Db.NonQ(command);
				}
				//End Web Sched preferences------
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						//This index was added to the primary key of the table in To6_1_1() when the table was created.
						//It is redundant to add an index to the primary key.
						command="ALTER TABLE anesthmedsgiven DROP INDEX AnestheticMedNum";
						Db.NonQ(command);
					}
					else {
						//table not added in oracle and oracle does not allow adding an index to the same column twice like MySQL, even if named differently
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						//The procedurelog table does not have the column ChartNum.
						//This index is incorrectly named and is on the PatNum column, which already has an index named indexPatNum.
						command="ALTER TABLE procedurelog DROP INDEX ChartNum";
						Db.NonQ(command);
					}
					else {
						//oracle does not allow adding an index to the same column twice like MySQL, even if named differently
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD IsDateProsthEst tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD IsDateProsthEst number(3)";
					Db.NonQ(command);
					command="UPDATE procedurelog SET IsDateProsthEst = 0 WHERE IsDateProsthEst IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog MODIFY IsDateProsthEst NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE referral ADD IsTrustedDirect tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE referral ADD IsTrustedDirect number(3)";
					Db.NonQ(command);
					command="UPDATE referral SET IsTrustedDirect = 0 WHERE IsTrustedDirect IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE referral MODIFY IsTrustedDirect NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ProblemListIsAlpabetical','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ProblemListIsAlpabetical','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD12'";
					DataTable Table=Db.GetTable(command);
					List<long> ListClaimFormNums=new List<long>();
					for(int i=0;i<Table.Rows.Count;i++) {
						ListClaimFormNums.Add(PIn.Long(Table.Rows[i]["ClaimFormNum"].ToString()));
					}
					for(int i=0;i<ListClaimFormNums.Count;i++) {
						command="UPDATE claimformitem SET width='250' WHERE ClaimFormNum='"+POut.Long(ListClaimFormNums[i])+"' AND FieldName='SubscrID'";
						Db.NonQ(command);
					}
				}
				else {//oracle
					command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD12'";
					DataTable Table=Db.GetTable(command);
					List<long> ListClaimFormNums=new List<long>();
					for(int i=0;i<Table.Rows.Count;i++) {
						ListClaimFormNums.Add(PIn.Long(Table.Rows[i]["ClaimFormNum"].ToString()));
					}
					for(int i=0;i<ListClaimFormNums.Count;i++) {
						command="UPDATE claimformitem SET width='250' WHERE ClaimFormNum='"+POut.Long(ListClaimFormNums[i])+"' AND FieldName='SubscrID'";
						Db.NonQ(command);
					}
				}
				//Add EmailSend permission to everyone------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT DISTINCT UserGroupNum FROM grouppermission";
					DataTable table=Db.GetTable(command);
					long groupNum;
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							 +"VALUES("+POut.Long(groupNum)+",85)";  //85: EmailSend
						Db.NonQ(command);
					}
				}
				else {//oracle
					command="SELECT DISTINCT UserGroupNum FROM grouppermission";
					DataTable table=Db.GetTable(command);
					long groupNum;
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",85)";  //85: EmailSend
						Db.NonQ(command);
					}
				}
				//Add WebmailSend permission to everyone------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT DISTINCT UserGroupNum FROM grouppermission";
					DataTable table=Db.GetTable(command);
					long groupNum;
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							 +"VALUES("+POut.Long(groupNum)+",86)";  //86: WebmailSend
						Db.NonQ(command);
					}
				}
				else {//oracle
					command="SELECT DISTINCT UserGroupNum FROM grouppermission";
					DataTable table=Db.GetTable(command);
					long groupNum;
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",86)";  //86: WebmailSend
						Db.NonQ(command);
					}
				}
				//START CPT column VersionIDs-------------------------------
				//Add new column to cpt table for keeping track of the years the cpt code existed in.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE cpt ADD VersionIDs varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE cpt ADD VersionIDs varchar2(255)";
					Db.NonQ(command);
				}
				//Add UserQueryAdmin permission to everyone-------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=19";//Anyone who currently has UserQuery access will initially have UserQueryAdmin access.
					DataTable table=Db.GetTable(command);
					long groupNum;
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							 +"VALUES("+POut.Long(groupNum)+",87)";  //87: UserQueryAdmin
						Db.NonQ(command);
					}
				}
				else {//oracle
					command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=19";//Anyone who currently has UserQuery access will initially have UserQueryAdmin access.
					DataTable table=Db.GetTable(command);
					long groupNum;
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",87)";  //87: UserQueryAdmin
						Db.NonQ(command);
					}
				}
				//Update every current cpt code to verion 2014.  Importing of 2015 codes was not implemented until this OD version or later.
				//oracle compatible
				command="UPDATE cpt SET VersionIDs='2014'";
				Db.NonQ(command);
				//END CPT column VersionIDs
				command="DELETE FROM grouppermission WHERE usergroupNum NOT IN (SELECT usergroupnum FROM usergroup)";//remove any orphaned grouppermissions; Oracle compatable
				Db.NonQ(command);
				//Add InsPlanChangeAssign permission to everyone------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				DataTable tableGroupPerm=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					long groupNum;
					for(int i=0;i<tableGroupPerm.Rows.Count;i++) {
						groupNum=PIn.Long(tableGroupPerm.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							 +"VALUES("+POut.Long(groupNum)+",88)";  //88: InsPlanChangeAssign
						Db.NonQ(command);
					}
				}
				else {//oracle
					long groupNum;
					for(int i=0;i<tableGroupPerm.Rows.Count;i++) {
						groupNum=PIn.Long(tableGroupPerm.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",88)";  //88: InsPlanChangeAssign
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sheetfield ADD TextAlign tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sheetfield ADD TextAlign number(3)";
					Db.NonQ(command);
					command="UPDATE sheetfield SET TextAlign = 0 WHERE TextAlign IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE sheetfield MODIFY TextAlign NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sheetfielddef ADD TextAlign tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sheetfielddef ADD TextAlign number(3)";
					Db.NonQ(command);
					command="UPDATE sheetfielddef SET TextAlign = 0 WHERE TextAlign IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE sheetfielddef MODIFY TextAlign NOT NULL";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sheetfielddef ADD IsPaymentOption tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sheetfielddef ADD IsPaymentOption number(3)";
					Db.NonQ(command);
					command="UPDATE sheetfielddef SET IsPaymentOption = 0 WHERE IsPaymentOption IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE sheetfielddef MODIFY IsPaymentOption NOT NULL";
					Db.NonQ(command);
				}
				string cBlack=POut.Int(System.Drawing.Color.Black.ToArgb());
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sheetfield ADD ItemColor int NOT NULL DEFAULT "+cBlack;
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sheetfield ADD ItemColor number(11)";
					Db.NonQ(command);
					command="UPDATE sheetfield SET ItemColor = "+cBlack+" WHERE ItemColor IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE sheetfield MODIFY ItemColor NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sheetfielddef ADD ItemColor int NOT NULL DEFAULT "+cBlack;
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sheetfielddef ADD ItemColor number(11)";
					Db.NonQ(command);
					command="UPDATE sheetfielddef SET ItemColor = "+cBlack+" WHERE ItemColor IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE sheetfielddef MODIFY ItemColor NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('StatementsUseSheets','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'StatementsUseSheets','0')";
					Db.NonQ(command);
				}
				//Insert Office bridge-----------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'Office', "
						+"'Office', "
						+"'0', "
						+"'"+POut.String("word.exe")+"', "
						+"'', "//leave blank if none
						+"'Verify the Path of file to open.')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Document folder', "
						+"'"+POut.String(@"C:\OpenDentImages\")+"')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'File extension', "
						+"'.doc')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"'"+POut.Long(programNum)+"', "
						+"'2', "//ToolBarsAvail.ChartModule
						+"'Office')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'Office', "
						+"'Office', "
						+"'0', "
						+"'"+POut.String("word.exe")+"', "
						+"'', "//leave blank if none
						+"'Verify the Path of file to open.')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Document folder', "
						+"'"+POut.String(@"C:\OpenDentImages\")+"')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'File extension', "
						+"'.doc')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
						+"'"+POut.Long(programNum)+"', "
						+"'2', "//ToolBarsAvail.ChartModule
						+"'Office')";
					Db.NonQ(command);
				}//end Office bridge
				//Inserting PriorityDefNum into task table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE task ADD PriorityDefNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE task ADD INDEX (PriorityDefNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE task ADD PriorityDefNum number(20)";
					Db.NonQ(command);
					command="UPDATE task SET PriorityDefNum = 0 WHERE PriorityDefNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE task MODIFY PriorityDefNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX task_PriorityDefNum ON task (PriorityDefNum)";
					Db.NonQ(command);
				}
				//Inserting new category for task PriorityDefNum defcat in definition table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor) "
						+"VALUES(33,0,'Normal','D',-1)";//Inserting definition with category 33 (TaskPriorities) with default of white (-1)
				}
				else {
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue,ItemColor) "
						+"VALUES((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),33,0,'Normal','D',-1)";//33 (TaskPriorities) with default of white (-1)
				}
				long defNum=Db.NonQ(command,true,"DefNum","definition");
				//Updating all tasks with white priority level
				command="UPDATE task SET PriorityDefNum="+POut.Long(defNum);
				Db.NonQ(command);
				//Add UserNameManualEntry to preference with a default value of '0' (so that it is disabled by default)
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('UserNameManualEntry','0')";
					Db.NonQ(command);
				}
				else{
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference), 'UserNameManualEntry','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE payplan ADD PaySchedule tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE payplan ADD PaySchedule number(3)";
					Db.NonQ(command);
					command="UPDATE payplan SET PaySchedule = 0 WHERE PaySchedule IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE payplan MODIFY PaySchedule NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE payplan ADD NumberOfPayments int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE payplan ADD NumberOfPayments number(11)";
					Db.NonQ(command);
					command="UPDATE payplan SET NumberOfPayments = 0 WHERE NumberOfPayments IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE payplan MODIFY NumberOfPayments NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE payplan ADD PayAmt double NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE payplan ADD PayAmt number(38,8)";
					Db.NonQ(command);
					command="UPDATE payplan SET PayAmt = 0 WHERE PayAmt IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE payplan MODIFY PayAmt NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE payplan ADD DownPayment double NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE payplan ADD DownPayment number(38,8)";
					Db.NonQ(command);
					command="UPDATE payplan SET DownPayment = 0 WHERE DownPayment IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE payplan MODIFY DownPayment NOT NULL";
					Db.NonQ(command);
				}
				//Add triple column index to procedurelog table for clinic filter enhancement, specifically patient selection.
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("procedurelog","PatNum,ProcStatus,ClinicNum")) {
							command="ALTER TABLE procedurelog ADD INDEX indexPNPSCN (PatNum,ProcStatus,ClinicNum)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX procedurelog_PNPSCN ON procedurelog (PatNum,ProcStatus,ClinicNum)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE commlog ADD IsWebSched tinyint NOT NULL";
					Db.NonQ(command);
					command="UPDATE commlog SET IsWebSched = 0";//Set all commlogs to not be web sched
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE commlog ADD IsWebSched number(3)";
					Db.NonQ(command);
					command="UPDATE commlog SET IsWebSched = 0 WHERE IsWebSched IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE commlog MODIFY IsWebSched NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE securitylog ADD LogSource tinyint NOT NULL";
					Db.NonQ(command);
					command="UPDATE securitylog SET LogSource = 0";//Set all securitylogs to none
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE securitylog ADD LogSource number(3)";
					Db.NonQ(command);
					command="UPDATE securitylog SET LogSource = 0 WHERE LogSource IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE securitylog MODIFY LogSource NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('SignalLastClearedDate','0001-01-01')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SignalLastClearedDate',TO_DATE('0001-01-01','YYYY-MM-DD'))";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE computerpref ADD ClinicNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE computerpref ADD INDEX (ClinicNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE computerpref ADD ClinicNum number(20)";
					Db.NonQ(command);
					command="UPDATE computerpref SET ClinicNum = 0 WHERE ClinicNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE computerpref MODIFY ClinicNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX computerpref_ClinicNum ON computerpref (ClinicNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE computerpref ADD ApptViewNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE computerpref ADD INDEX (ApptViewNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE computerpref ADD ApptViewNum number(20)";
					Db.NonQ(command);
					command="UPDATE computerpref SET ApptViewNum = 0 WHERE ApptViewNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE computerpref MODIFY ApptViewNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX computerpref_ApptViewNum ON computerpref (ApptViewNum)";
					Db.NonQ(command);
				}
				//convert RecentApptView (index in list of views for the recent view) to ApptViewNum (FK to the actual view)
				command="SELECT * FROM computerpref";
				DataTable tableCompPrefs=Db.GetTable(command);
				command="SELECT * FROM apptview ORDER BY ItemOrder";
				DataTable tableApptViews=Db.GetTable(command);
				for(int i=0;i<tableCompPrefs.Rows.Count;i++) {
					try {
						long compPrefNum=PIn.Long(tableCompPrefs.Rows[i]["ComputerPrefNum"].ToString());
						//The computer preference 'RecentApptView' column is stored as a byte that represents the selected index of the apptview within the appt view combo box.  It is 1 based.
						int apptViewIndex=PIn.Int(tableCompPrefs.Rows[i]["RecentApptView"].ToString());
						long apptViewNum=0;//Default to 'none' view.
						if(apptViewIndex > 0 && apptViewIndex<=tableApptViews.Rows.Count) {
							apptViewIndex--;//Subtract 1 from apptViewIndex because RecentApptView is 1 based and we need to treat it 0 based.  If it is already zero, let it go through.
							apptViewNum=PIn.Long(tableApptViews.Rows[apptViewIndex]["ApptViewNum"].ToString());//Get the apptview based on the index of the old RecentApptView computer preference.
						}
						command="UPDATE computerpref SET ApptViewNum="+POut.Long(apptViewNum)+" WHERE ComputerPrefNum="+POut.Long(compPrefNum);
						Db.NonQ(command);
					}
					catch(Exception) {
						//Don't fail the upgrade for failing to set a default appt view for this computer.
						//The worst that could happen is first user to log into this computer will see a default "none" view.
						//Keep trying to set defaults for subsequent computers.
					}
				}
				//after converting data in RecentApptView to ApptViewNum, drop the RecentApptView column
				command="ALTER TABLE computerpref DROP COLUMN RecentApptView";
				Db.NonQ(command);
				#region Duplicate Views for Clinics
				//Any apptviews with a ClinicNum set will need to have an apptviewitem for each clinic associated to that clinic.
				//For any views that contained an operatory and were "assigned to a clinic", they will no longer have access to that operatory.  This is expected behavior with our new clinic filtering.
				//Once all clinic apptviews are moved out of the 'Headquarters' view, we'll need to go back through the 'Headquarters' apptview list and fix the ItemOrders.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//Start by grabbing all apptviews with a clinic set.
					command="SELECT apptview.ApptViewNum,apptview.ClinicNum FROM apptview WHERE apptview.ClinicNum > 0";
					DataTable tableApptViewClinics=Db.GetTable(command);
					Dictionary<long,DataTable> dictOpsForClinics=new Dictionary<long,DataTable>();//Key = ClinicNum, Value= DataTable of OperatoryNums for the specified clinic.
					for(int i=0;i<tableApptViewClinics.Rows.Count;i++) {
						DataTable tableClinicOpNums=null;
						long clinicNum=PIn.Long(tableApptViewClinics.Rows[i]["ClinicNum"].ToString());
						//Now that we have a list of apptviews that will be moved into their own sub lists, we need to clear out any current operatories and replace them with ALL ops for that clinic.
						//Check to see if we've already gone to the database to retrieve all operatories for the clinic for this apptview
						if(dictOpsForClinics.ContainsKey(clinicNum)) {
							tableClinicOpNums=dictOpsForClinics[clinicNum];
						}
						else {
							command="SELECT OperatoryNum FROM operatory WHERE operatory.ClinicNum="+tableApptViewClinics.Rows[i]["ClinicNum"].ToString();
							tableClinicOpNums=Db.GetTable(command);
							dictOpsForClinics.Add(clinicNum,tableClinicOpNums);
						}
						if(tableClinicOpNums.Rows.Count==0) {
							//There is no such thing as an apptview with no operatory selected.  If there are no ops for this particular clinic, we must remove the clinic filter so that they can manually set it later.
							command="UPDATE apptview SET apptview.ClinicNum=0 WHERE apptview.ApptViewNum="+tableApptViewClinics.Rows[i]["ApptViewNum"].ToString();
							Db.NonQ(command);
							continue;
						}
						//Remove all current apptviewitems that are for 'ops'.  We have to remove them all because they might be for ops that are not associated to the apptview's clinic.
						command="DELETE FROM apptviewitem "
							+"WHERE apptviewitem.ApptViewNum="+tableApptViewClinics.Rows[i]["ApptViewNum"].ToString()+" "
							+"AND apptviewitem.OpNum > 0";
						Db.NonQ(command);
						//Add an 'op' apptviewitem for every single operatory that is associated to the apptview's clinic.
						command="";
						for(int j=0;j<tableClinicOpNums.Rows.Count;j++) {
							command+="INSERT INTO apptviewitem (ApptViewNum,OpNum) VALUES ("+tableApptViewClinics.Rows[i]["ApptViewNum"].ToString()+","+tableClinicOpNums.Rows[j]["OperatoryNum"].ToString()+");\r\n";
						}
						Db.NonQ(command);//Bulk inserts are not Oracle compatible with this current syntax.
					}
					//Update item orders for all apptviews split up into sub groups by clinic.
					command="SELECT ApptViewNum,ClinicNum FROM apptview ORDER BY ClinicNum,ItemOrder";
					DataTable tableClinicViewsOrdered=Db.GetTable(command);
					long clinicNumPrev=0;
					int itemOrderCur=0;
					for(int i=0;i<tableClinicViewsOrdered.Rows.Count;i++) {
						long clinicNumCur=PIn.Long(tableClinicViewsOrdered.Rows[i]["ClinicNum"].ToString());
						long apptViewNumCur=PIn.Long(tableClinicViewsOrdered.Rows[i]["ApptViewNum"].ToString());
						if(i==0 || clinicNumCur!=clinicNumPrev) {
							itemOrderCur=0;
							clinicNumPrev=clinicNumCur;
						}
						else if(clinicNumCur==clinicNumPrev) {
							itemOrderCur++;
						}
						command="UPDATE apptview SET ItemOrder="+POut.Int(itemOrderCur)+" WHERE ApptViewNum="+POut.Long(apptViewNumCur);
						Db.NonQ(command);
					}
				}
				else {//oracle
					//we won't try to duplicate the views for oracle, the user will have to create the views for each clinic manually
				}
				#endregion
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('TempFolderDateFirstCleaned',CURDATE())";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'TempFolderDateFirstCleaned',SYSDATE)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS userodapptview";
					Db.NonQ(command);
					command=@"CREATE TABLE userodapptview (
						UserodApptViewNum bigint NOT NULL auto_increment PRIMARY KEY,
						UserNum bigint NOT NULL,
						ClinicNum bigint NOT NULL,
						ApptViewNum bigint NOT NULL,
						INDEX(UserNum),
						INDEX(ClinicNum),
						INDEX(ApptViewNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE userodapptview'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE userodapptview (
						UserodApptViewNum number(20) NOT NULL,
						UserNum number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						ApptViewNum number(20) NOT NULL,
						CONSTRAINT userodapptview_UserodApptViewN PRIMARY KEY (UserodApptViewNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX userodapptview_UserNum ON userodapptview (UserNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX userodapptview_ClinicNum ON userodapptview (ClinicNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX userodapptview_ApptViewNum ON userodapptview (ApptViewNum)";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '15.1.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_1_3();
		}

		///<summary></summary>
		private static void To15_1_3() {
			if(FromVersion<new Version("15.1.3.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.1.3");//No translation in convert script.
				string command="";
				//We dropped RecentApptView in 15.1.1 but should not have because it is a column that is used by older versions prior to calling the 'update file copier' code which will cause UEs to occur.
				//Bringing the column back with deprecation comments for the database documentation in our manual.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE computerpref ADD RecentApptView tinyint unsigned NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE computerpref ADD RecentApptView number(3)";
					Db.NonQ(command);
					command="UPDATE computerpref SET RecentApptView = 0 WHERE RecentApptView IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE computerpref MODIFY RecentApptView NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '15.1.3.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_1_13();
		}

		///<summary>Oracle compatible: 02/25/2015</summary>
		private static void To15_1_13() {
			if(FromVersion<new Version("15.1.13.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.1.13");//No translation in convert script.
				string command="";
				//AppointmentBubblesNoteLength also inserted into version 14.3.35
				//This code was copied from the pattern used to insert FamPhiAccess pref in To14_3_9
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT COUNT(*) FROM preference WHERE PrefName='AppointmentBubblesNoteLength' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT COUNT(*) FROM (SELECT ValueString FROM preference WHERE PrefName='AppointmentBubblesNoteLength') WHERE RowNum<=1";
				}
				long hasAppointmentBubblesNoteLength=PIn.Long(Db.GetCount(command));
				if(hasAppointmentBubblesNoteLength==0) {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO preference(PrefName,ValueString) VALUES('AppointmentBubblesNoteLength','0')";
						Db.NonQ(command);
					}
					else {//oracle
						command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AppointmentBubblesNoteLength','0')";
						Db.NonQ(command);
					}
				}
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("sheetfield","FieldType")) {
							command="ALTER TABLE sheetfield ADD INDEX (FieldType)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX sheetfield_FieldType ON sheetfield (FieldType)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				command="UPDATE preference SET ValueString = '15.1.13.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_1_14();
		}

		///<summary>Oracle compatible: 03/02/2015</summary>
		private static void To15_1_14() {
			if(FromVersion<new Version("15.1.14.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.1.14");//No translation in convert script.
				string command="";
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE adjustment ADD INDEX indexProvNum (ProvNum)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX adjustment_ProvNum ON adjustment (ProvNum)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE claimproc ADD INDEX indexPNPD (ProvNum,ProcDate)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX claimproc_PNPD ON claimproc (ProvNum,ProcDate)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE claimproc ADD INDEX indexPNDCP (ProvNum,DateCP)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX claimproc_PNDCP ON claimproc (ProvNum,DateCP)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE procedurelog ADD INDEX indexPNPD (ProvNum,ProcDate)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX procedurelog_PNPD ON procedurelog (ProvNum,ProcDate)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE tasklist ADD INDEX indexParent (Parent)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX tasklist_Parent ON tasklist (Parent)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				command="UPDATE preference SET ValueString = '15.1.14.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_1_16();
		}

		///<summary>Oracle compatible: 03/18/2015</summary>
		private static void To15_1_16() {
			if(FromVersion<new Version("15.1.16.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.1.16");//No translation in convert script.
				string command="";
				command="SELECT * FROM preference WHERE PrefName='InsPPOsecWriteoffs'";
				DataTable tableCur=Db.GetTable(command);
				if(tableCur.Rows.Count==0) {//The InsPPOsecWriteoffs pref does not already exist
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO preference(PrefName,ValueString) VALUES('InsPPOsecWriteoffs','0')";
					}
					else {//oracle
						command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'InsPPOsecWriteoffs','0')";
					}
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '15.1.16.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_1_17();
		}

		///<summary>Oracle compatible: 03/24/2015</summary>
		private static void To15_1_17() {
			if(FromVersion<new Version("15.1.17.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.1.17");//No translation in convert script.
				string command="";
				//Insert Triana bridge-----------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"'Triana', "
				    +"'Triana from genorayamerica.com', "
				    +"'0', "
				    +"'"+POut.String(@"Triana.exe")+"', "
				    +"'', "//leave blank if none
				    +"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'Import.ini path', "
				    +"'"+POut.String(@"C:\Program Files\Triana\Import.ini")+"')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"'"+POut.Long(programNum)+"', "
				    +"'2', "//ToolBarsAvail.ChartModule
				    +"'Triana')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'Triana', "
				    +"'Triana from genorayamerica.com', "
				    +"'0', "
				    +"'"+POut.String(@"Triana.exe")+"', "
				    +"'', "//leave blank if none
				    +"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'Import.ini path', "
				    +"'"+POut.String(@"C:\Program Files\Triana\Import.ini")+"')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'2', "//ToolBarsAvail.ChartModule
				    +"'Triana')";
					Db.NonQ(command);
				}//end Triana bridge
				//Insert VixWinNumbered bridge-----------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"'VixWinNumbered', "
				    +"'VixWin(numbered) from www.gendexxray.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\VixWin\VixWin.exe")+"', "
				    +"'', "//leave blank if none
				    +"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'Image Path', "
				    +"'"+POut.String(@"X:\VXImages\")+"')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"'"+POut.Long(programNum)+"', "
				    +"'2', "//ToolBarsAvail.ChartModule
				    +"'VixWinNumbered')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'VixWinNumbered', "
				    +"'VixWin(numbered) from www.gendexxray.com', "
				    +"'0', "
				    +"'"+POut.String(@"C:\VixWin\VixWin.exe")+"', "
				    +"'', "//leave blank if none
				    +"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'Image Path', "
				    +"'"+POut.String(@"X:\VXImages\")+"')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
				    +"VALUES ("
				    +"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'2', "//ToolBarsAvail.ChartModule
				    +"'VixWinNumbered')";
					Db.NonQ(command);
				}//end VixWinNumbered bridge
				command="UPDATE preference SET ValueString = '15.1.17.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_1_20();
		}

		///<summary></summary>
		private static void To15_1_20() {
			if(FromVersion<new Version("15.1.20.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.1.20");//No translation in convert script.
				string command="";
				command="SELECT DefNum FROM definition WHERE Category='27' AND ItemValue='RECALL' ORDER BY ItemOrder";//commlog type for recall
				DataTable tableRecallTypes=Db.GetTable(command);
				DataTable tableCommlogs=new DataTable();//Will have no rows when there are no recall types set up.
				if(tableRecallTypes.Rows.Count>0) {
					command="SELECT PatNum,CommDateTime,Note FROM commlog where CommType='"+tableRecallTypes.Rows[0]["DefNum"].ToString()+"'";
					tableCommlogs=Db.GetTable(command);
				}
				for(int i=0;i<tableCommlogs.Rows.Count;i++) {//Make ehrmeasureevent for users who have been sending reminders from FormRecallList
					DateTime dateTimeComm=PIn.DateT(tableCommlogs.Rows[i]["CommDateTime"].ToString());
					long patNum=PIn.Long(tableCommlogs.Rows[i]["PatNum"].ToString());
					string note=PIn.String(tableCommlogs.Rows[i]["Note"].ToString());
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO ehrmeasureevent (DateTEvent,EventType,PatNum,MoreInfo) "
							+"VALUES( "
							+POut.DateT(dateTimeComm,true)+","//DateTEvent
							+"5,"//EventType ReminderSent
							+POut.Long(patNum)+","//PatNum
							+"'"+POut.String(note)+"'); ";//MoreInfo
					}
					else {
						//EHR is not Oracle compatable, so we don't worry about Oracle here.
					}
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '15.1.20.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_1_22();
		}

		///<summary></summary>
		private static void To15_1_22() {
			if(FromVersion<new Version("15.1.22.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.1.22");//No translation in convert script.
				string command="";
				//The next command is MySQL and Oracle compatible.  Used LTRIM() to remove the leading space after "NewCrop" is removed from description.
				command="UPDATE program SET ProgName='eRx', ProgDesc=LTRIM(REPLACE(ProgDesc,'NewCrop','')), Note=REPLACE(Note,'NewCrop','eRx') WHERE ProgName='NewCrop'";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '15.1.22.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_2_1();
		}

		///<summary></summary>
		private static void To15_2_1() {
			if(FromVersion<new Version("15.2.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.2.1");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('WaitingRoomFilterByView','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WaitingRoomFilterByView','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('BrokenApptCommLogWithProcedure','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'BrokenApptCommLogWithProcedure','0')";
					Db.NonQ(command);
				}
				command="SELECT CodeNum FROM procedurecode WHERE ProcCode='D9986'";//Broken appointment procedure for CDT 2015.  Oracle compatible.
				long codeNum=PIn.Long(Db.GetScalar(command));
				if(codeNum!=0) {//If foreign, the customer will probably not have D9986.  This is for USA only.
					if(DataConnection.DBtype==DatabaseType.MySql) {
						//Make a 'broken appointment' procedure for every adjustment in the database.
						command="SELECT ValueString FROM preference WHERE PrefName='BrokenAppointmentAdjustmentType'";
						long brokenAdjType=PIn.Long(Db.GetScalar(command));
						command="SELECT * FROM adjustment WHERE AdjType="+brokenAdjType;
						DataTable tableBrokenAdjustments=Db.GetTable(command);
						for(int i=0;i<tableBrokenAdjustments.Rows.Count;i++) {
							DateTime dateAdj=PIn.Date(tableBrokenAdjustments.Rows[i]["AdjDate"].ToString());
							command="INSERT INTO procedurelog ("
								+"PatNum,DateTP,ProcDate,DateEntryC,ProcFee,ProcStatus,ProvNum,ClinicNum,CodeNum,UnitQty,CodeMod1,CodeMod2,CodeMod3,CodeMod4,RevCode) VALUES("
								+tableBrokenAdjustments.Rows[i]["PatNum"].ToString()+","
								+POut.Date(dateAdj,true)+","//DateTP
								+POut.Date(dateAdj,true)+","//ProcDate
								+POut.Date(dateAdj,true)+","//DateEntryC
								+"0,"//ProcFee
								+"2,"//ProcStatus complete
								+tableBrokenAdjustments.Rows[i]["ProvNum"].ToString()+","
								+tableBrokenAdjustments.Rows[i]["ClinicNum"].ToString()+","
								+codeNum.ToString()+","//Code D9986
								+"1,"//UnitQty
								+"'','','','','')";//CodeMod1,CodeMod2,CodeMod3,CodeMod4,RevCode
							Db.NonQ(command);
						}
					}
					else {//Oracle
						//Not going to worry about Oracle automation for inserting procedures.
						//We would have to spell out every single column that does not allow null values and no one uses Oracle. -jsalmon
					}
					command="UPDATE procedurecode SET NoBillIns=1 WHERE CodeNum="+codeNum;//oracle compatible
					Db.NonQ(command);
				}
				#region medlab tables for LabCorp HL7 interface
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS medlab";
					Db.NonQ(command);
					command=@"CREATE TABLE medlab (
						MedLabNum bigint NOT NULL auto_increment PRIMARY KEY,
						SendingApp varchar(255) NOT NULL,
						SendingFacility varchar(255) NOT NULL,
						PatNum bigint NOT NULL,
						ProvNum bigint NOT NULL,
						PatIDLab varchar(255) NOT NULL,
						PatIDAlt varchar(255) NOT NULL,
						PatAge varchar(255) NOT NULL,
						PatAccountNum varchar(255) NOT NULL,
						PatFasting tinyint NOT NULL,
						SpecimenID varchar(255) NOT NULL,
						SpecimenIDFiller varchar(255) NOT NULL,
						ObsTestID varchar(255) NOT NULL,
						ObsTestDescript varchar(255) NOT NULL,
						ObsTestLoinc varchar(255) NOT NULL,
						ObsTestLoincText varchar(255) NOT NULL,
						DateTimeCollected datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						TotalVolume varchar(255) NOT NULL,
						ActionCode varchar(255) NOT NULL,
						ClinicalInfo varchar(255) NOT NULL,
						DateTimeEntered datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						OrderingProvNPI varchar(255) NOT NULL,
						OrderingProvLocalID varchar(255) NOT NULL,
						OrderingProvLName varchar(255) NOT NULL,
						OrderingProvFName varchar(255) NOT NULL,
						SpecimenIDAlt varchar(255) NOT NULL,
						DateTimeReported datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						ResultStatus varchar(255) NOT NULL,
						ParentObsID varchar(255) NOT NULL,
						ParentObsTestID varchar(255) NOT NULL,
						NotePat text NOT NULL,
						NoteLab text NOT NULL,
						FileName varchar(255) NOT NULL,
						OriginalPIDSegment text NOT NULL,
						INDEX(PatNum),
						INDEX(ProvNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE medlab'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE medlab (
						MedLabNum number(20) NOT NULL,
						SendingApp varchar2(255),
						SendingFacility varchar2(255),
						PatNum number(20) NOT NULL,
						ProvNum number(20) NOT NULL,
						PatIDLab varchar2(255),
						PatIDAlt varchar2(255),
						PatAge varchar2(255),
						PatAccountNum varchar2(255),
						PatFasting number(3) NOT NULL,
						SpecimenID varchar2(255),
						SpecimenIDFiller varchar2(255),
						ObsTestID varchar2(255),
						ObsTestDescript varchar2(255),
						ObsTestLoinc varchar2(255),
						ObsTestLoincText varchar2(255),
						DateTimeCollected date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						TotalVolume varchar2(255),
						ActionCode varchar2(255),
						ClinicalInfo varchar2(255),
						DateTimeEntered date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						OrderingProvNPI varchar2(255),
						OrderingProvLocalID varchar2(255),
						OrderingProvLName varchar2(255),
						OrderingProvFName varchar2(255),
						SpecimenIDAlt varchar2(255),
						DateTimeReported date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						ResultStatus varchar2(255),
						ParentObsID varchar2(255),
						ParentObsTestID varchar2(255),
						NotePat clob,
						NoteLab clob,
						FileName varchar2(255),
						OriginalPIDSegment varchar2(4000),
						CONSTRAINT medlab_MedLabNum PRIMARY KEY (MedLabNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX medlab_PatNum ON medlab (PatNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX medlab_ProvNum ON medlab (ProvNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS medlabresult";
					Db.NonQ(command);
					command=@"CREATE TABLE medlabresult (
						MedLabResultNum bigint NOT NULL auto_increment PRIMARY KEY,
						MedLabNum bigint NOT NULL,
						ObsID varchar(255) NOT NULL,
						ObsText varchar(255) NOT NULL,
						ObsLoinc varchar(255) NOT NULL,
						ObsLoincText varchar(255) NOT NULL,
						ObsIDSub varchar(255) NOT NULL,
						ObsValue text NOT NULL,
						ObsSubType varchar(255) NOT NULL,
						ObsUnits varchar(255) NOT NULL,
						ReferenceRange varchar(255) NOT NULL,
						AbnormalFlag varchar(255) NOT NULL,
						ResultStatus varchar(255) NOT NULL,
						DateTimeObs datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						FacilityID varchar(255) NOT NULL,
						DocNum bigint NOT NULL,
						Note text NOT NULL,
						INDEX(MedLabNum),
						INDEX(DocNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE medlabresult'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE medlabresult (
						MedLabResultNum number(20) NOT NULL,
						MedLabNum number(20) NOT NULL,
						ObsID varchar2(255),
						ObsText varchar2(255),
						ObsLoinc varchar2(255),
						ObsLoincText varchar2(255),
						ObsIDSub varchar2(255),
						ObsValue clob,
						ObsSubType varchar2(255),
						ObsUnits varchar2(255),
						ReferenceRange varchar2(255),
						AbnormalFlag varchar2(255),
						ResultStatus varchar2(255),
						DateTimeObs date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						FacilityID varchar2(255),
						DocNum number(20) NOT NULL,
						Note clob,
						CONSTRAINT medlabresult_MedLabResultNum PRIMARY KEY (MedLabResultNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX medlabresult_MedLabNum ON medlabresult (MedLabNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX medlabresult_DocNum ON medlabresult (DocNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS medlabspecimen";
					Db.NonQ(command);
					command=@"CREATE TABLE medlabspecimen (
						MedLabSpecimenNum bigint NOT NULL auto_increment PRIMARY KEY,
						MedLabNum bigint NOT NULL,
						SpecimenID varchar(255) NOT NULL,
						SpecimenDescript varchar(255) NOT NULL,
						DateTimeCollected datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						INDEX(MedLabNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE medlabspecimen'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE medlabspecimen (
						MedLabSpecimenNum number(20) NOT NULL,
						MedLabNum number(20) NOT NULL,
						SpecimenID varchar2(255),
						SpecimenDescript varchar2(255),
						DateTimeCollected date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT medlabspecimen_MedLabSpecimenN PRIMARY KEY (MedLabSpecimenNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX medlabspecimen_MedLabNum ON medlabspecimen (MedLabNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS medlabfacility";
					Db.NonQ(command);
					command=@"CREATE TABLE medlabfacility (
						MedLabFacilityNum bigint NOT NULL auto_increment PRIMARY KEY,
						FacilityName varchar(255) NOT NULL,
						Address varchar(255) NOT NULL,
						City varchar(255) NOT NULL,
						State varchar(255) NOT NULL,
						Zip varchar(255) NOT NULL,
						Phone varchar(255) NOT NULL,
						DirectorTitle varchar(255) NOT NULL,
						DirectorLName varchar(255) NOT NULL,
						DirectorFName varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE medlabfacility'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE medlabfacility (
						MedLabFacilityNum number(20) NOT NULL,
						FacilityName varchar2(255),
						Address varchar2(255),
						City varchar2(255),
						State varchar2(255),
						Zip varchar2(255),
						Phone varchar2(255),
						DirectorTitle varchar2(255),
						DirectorLName varchar2(255),
						DirectorFName varchar2(255),
						CONSTRAINT medlabfacility_MedLabFacilityN PRIMARY KEY (MedLabFacilityNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS medlabfacattach";
					Db.NonQ(command);
					command=@"CREATE TABLE medlabfacattach (
						MedLabFacAttachNum bigint NOT NULL auto_increment PRIMARY KEY,
						MedLabNum bigint NOT NULL,
						MedLabResultNum bigint NOT NULL,
						MedLabFacilityNum bigint NOT NULL,
						INDEX(MedLabNum),
						INDEX(MedLabResultNum),
						INDEX(MedLabFacilityNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE medlabfacattach'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE medlabfacattach (
						MedLabFacAttachNum number(20) NOT NULL,
						MedLabNum number(20) NOT NULL,
						MedLabResultNum number(20) NOT NULL,
						MedLabFacilityNum number(20) NOT NULL,
						CONSTRAINT medlabfacattach_MedLabFacAttac PRIMARY KEY (MedLabFacAttachNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX medlabfacattach_MedLabNum ON medlabfacattach (MedLabNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX medlabfacattach_MedLabResultNu ON medlabfacattach (MedLabResultNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX medlabfacattach_MedLabFacility ON medlabfacattach (MedLabFacilityNum)";
					Db.NonQ(command);
				}
				#endregion medlab tables for LabCorp HL7 interface
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD OrthoTotalM tinyint unsigned NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD OrthoTotalM number(3)";
					Db.NonQ(command);
					command="UPDATE claim SET OrthoTotalM = 0 WHERE OrthoTotalM IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim MODIFY OrthoTotalM NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('CentralManagerSecurityLock','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'CentralManagerSecurityLock','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE userod ADD UserNumCEMT bigint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE userod ADD UserNumCEMT number(20)";
					Db.NonQ(command);
					command="UPDATE userod SET UserNumCEMT = 0 WHERE UserNumCEMT IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE userod MODIFY UserNumCEMT NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE usergroup ADD UserGroupNumCEMT bigint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE usergroup ADD UserGroupNumCEMT number(20)";
					Db.NonQ(command);
					command="UPDATE usergroup SET UserGroupNumCEMT = 0 WHERE UserGroupNumCEMT IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE usergroup MODIFY UserGroupNumCEMT NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD DefaultProv bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE clinic ADD INDEX (DefaultProv)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD DefaultProv number(20)";
					Db.NonQ(command);
					command="UPDATE clinic SET DefaultProv = 0 WHERE DefaultProv IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clinic MODIFY DefaultProv NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX clinic_DefaultProv ON clinic (DefaultProv)";
					Db.NonQ(command);
				}
				#region ConnectionGroup tables for Central Enterprise Management Tool
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS connectiongroup";
					Db.NonQ(command);
					command=@"CREATE TABLE connectiongroup (
						ConnectionGroupNum bigint NOT NULL auto_increment PRIMARY KEY,
						Description varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE connectiongroup'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE connectiongroup (
						ConnectionGroupNum number(20) NOT NULL,
						Description varchar2(255),
						CONSTRAINT connectiongroup_ConnGroupNum PRIMARY KEY (ConnectionGroupNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS conngroupattach";
					Db.NonQ(command);
					command=@"CREATE TABLE conngroupattach (
						ConnGroupAttachNum bigint NOT NULL auto_increment PRIMARY KEY,
						ConnectionGroupNum bigint NOT NULL,
						CentralConnectionNum bigint NOT NULL,
						INDEX(ConnectionGroupNum),
						INDEX(CentralConnectionNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE conngroupattach'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE conngroupattach (
						ConnGroupAttachNum number(20) NOT NULL,
						ConnectionGroupNum number(20) NOT NULL,
						CentralConnectionNum number(20) NOT NULL,
						CONSTRAINT conngroupattach_ConnGroupAttac PRIMARY KEY (ConnGroupAttachNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX conngroupattach_CentralConnect ON conngroupattach (CentralConnectionNum)";
					Db.NonQ(command);
				}
				#endregion
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE hl7def ADD LabResultImageCat bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE hl7def ADD INDEX (LabResultImageCat)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE hl7def ADD LabResultImageCat number(20)";
					Db.NonQ(command);
					command="UPDATE hl7def SET LabResultImageCat = 0 WHERE LabResultImageCat IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE hl7def MODIFY LabResultImageCat NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX hl7def_LabResultImageCat ON hl7def (LabResultImageCat)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE hl7def ADD SftpUsername varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE hl7def ADD SftpUsername varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE hl7def ADD SftpPassword varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE hl7def ADD SftpPassword varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE hl7def ADD SftpInSocket varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE hl7def ADD SftpInSocket varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('HelpKey','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'HelpKey','')";
					Db.NonQ(command);
				}
				#region//==========================ADD SMS TABLES===========================
				//Add table eservicesignal
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS eservicesignal";
					Db.NonQ(command);
					command=@"CREATE TABLE eservicesignal (
						EServiceSignalNum bigint NOT NULL auto_increment PRIMARY KEY,
						ServiceCode int NOT NULL,
						ReasonCategory int NOT NULL,
						ReasonCode int NOT NULL,
						Severity tinyint NOT NULL,
						Description varchar(255) NOT NULL,
						SigDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						Tag varchar(255) NOT NULL,
						IsProcessed tinyint NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE eservicesignal'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE eservicesignal (
						EServiceSignalNum number(20) NOT NULL,
						ServiceCode number(11) NOT NULL,
						ReasonCategory number(11) NOT NULL,
						ReasonCode number(11) NOT NULL,
						Severity number(3) NOT NULL,
						Description varchar2(255),
						SigDateTime date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						Tag varchar2(255),
						IsProcessed number(3) NOT NULL,
						CONSTRAINT eservicesignal_EServiceSignalN PRIMARY KEY (EServiceSignalNum)
						)";
					Db.NonQ(command);
				}
				//Add SmsMo table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS smsmo";
					Db.NonQ(command);
					command=@"CREATE TABLE smsmo (
						SmsMONum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						ClinicNum bigint NOT NULL,
						CommlogNum bigint NOT NULL,
						MsgText text NOT NULL,
						DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						SmsPhone varchar(255) NOT NULL,
						MsgPart varchar(255) NOT NULL,
						MsgTotal varchar(255) NOT NULL,
						MsgRefID varchar(255) NOT NULL,
						INDEX(PatNum),
						INDEX(ClinicNum),
						INDEX(CommlogNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE smsmo'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE smsmo (
						SmsMONum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						CommlogNum number(20) NOT NULL,
						MsgText clob,
						DateTimeEntry date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						SmsPhone varchar2(255),
						MsgPart varchar2(255),
						MsgTotal varchar2(255),
						MsgRefID varchar2(255),
						CONSTRAINT smsmo_SmsMONum PRIMARY KEY (SmsMONum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX smsmo_PatNum ON smsmo (PatNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX smsmo_ClinicNum ON smsmo (ClinicNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX smsmo_CommlogNum ON smsmo (CommlogNum)";
					Db.NonQ(command);
				}
				//Add SmsMt table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS smsmt";
					Db.NonQ(command);
					command=@"CREATE TABLE smsmt (
						SmsMTNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						GuidMessage varchar(255) NOT NULL,
						GuidBatch varchar(255) NOT NULL,
						PhoneNumber varchar(255) NOT NULL,
						PhonePat varchar(255) NOT NULL,
						IsTimeSensitive tinyint NOT NULL,
						MsgType tinyint NOT NULL,
						MsgText text NOT NULL,
						Status tinyint NOT NULL,
						MsgParts int NOT NULL,
						MsgCost double NOT NULL,
						ClinicNum bigint NOT NULL,
						CustErrorText varchar(255) NOT NULL,
						DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						DateTimeTerminated datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						INDEX(PatNum),
						INDEX(ClinicNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE smsmt'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE smsmt (
						SmsMTNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						GuidMessage varchar2(255),
						GuidBatch varchar2(255),
						PhoneNumber varchar2(255),
						PhonePat varchar2(255),
						IsTimeSensitive number(3) NOT NULL,
						MsgType number(3) NOT NULL,
						MsgText clob,
						Status number(3) NOT NULL,
						MsgParts number(11) NOT NULL,
						MsgCost number(38,8) NOT NULL,
						ClinicNum number(20) NOT NULL,
						CustErrorText varchar2(255),
						DateTimeEntry date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateTimeTerminated date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT smsmt_SmsMTNum PRIMARY KEY (SmsMTNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX smsmt_PatNum ON smsmt (PatNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX smsmt_ClinicNum ON smsmt (ClinicNum)";
					Db.NonQ(command);
				}
				//add table smsvln
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS smsvln";
					Db.NonQ(command);
					command=@"CREATE TABLE smsvln (
						SmsVlnNum bigint NOT NULL auto_increment PRIMARY KEY,
						ClinicNum bigint NOT NULL,
						PhoneNumber varchar(255) NOT NULL,
						DateActive datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						DateInactive datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						InactiveCode varchar(255) NOT NULL,
						INDEX(ClinicNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE smsvln'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE smsvln (
						SmsVlnNum number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						PhoneNumber varchar2(255),
						DateActive date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateInactive date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						InactiveCode varchar2(255),
						CONSTRAINT smsvln_SmsVlnNum PRIMARY KEY (SmsVlnNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX smsvln_ClinicNum ON smsvln (ClinicNum)";
					Db.NonQ(command);
				}
				#endregion//========================END ADD SMS TABLES=========================
				//SMS contract fields
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('SmsContractDate','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SmsContractDate','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('SmsContractName','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SmsContractName','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD SmsContractDate datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD SmsContractDate date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD')";
					Db.NonQ(command);
					command="UPDATE clinic SET SmsContractDate = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE SmsContractDate IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clinic MODIFY SmsContractDate NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD SmsContractName varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD SmsContractName varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ProgramNum FROM program WHERE ProgName='Xcharge' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT ProgramNum FROM (SELECT ProgramNum FROM program WHERE ProgName='Xcharge') WHERE RowNum<=1";
				}
				long programNum=PIn.Long(Db.GetScalar(command));
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'XWebID', "
						+"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'AuthKey', "
						+"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'TerminalID', "
						+"'')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'XWebID', "
						+"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'AuthKey', "
						+"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'TerminalID', "
						+"'')";
					Db.NonQ(command);
				}//end X-Web properties.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('SignalInactiveMinutes','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SignalInactiveMinutes','0')";
					Db.NonQ(command);
				}
				//Add waiting room prefs
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('WaitingRoomAlertColor','-16777216')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WaitingRoomAlertColor','-16777216')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('WaitingRoomAlertTime','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WaitingRoomAlertTime','0')";
					Db.NonQ(command);
				}
				//Give all users with Setup permission the new EServices permission------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=8";//Setup
				DataTable table=Db.GetTable(command);
				long groupNum;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",91)";//EServices
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",91)";//EServices
						Db.NonQ32(command);
					}
				}
				//Listener Service monitoring.  As of right now the Patient Portal is the only important eService.  
				//Turn on Listener Service monitoring for offices using the patient portal by inserting an eService signal of status 'Critical'
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//Patients with OnlinePasswords will be the indicator that an office is or has attempted to use the patient portal.
					command="SELECT COUNT(*) FROM patient WHERE OnlinePassword!=''";
					int countPatPortals=PIn.Int(Db.GetCount(command));
					if(countPatPortals > 5) {//Check for more than 5 patient portal patients to avoid false positives.
						//Insert a 'Critical' signal into the eservicesignal table to trigger Listener Service monitoring.
						//Customers with active Listener Services will instantly have a 'Working' signal inserted and will not get notified of the service being down.
						//However, if the customer does not know that their service is down (not tech savy) then this will alert them to that fact (our goal).
						command="INSERT INTO eservicesignal (ServiceCode,ReasonCategory,ReasonCode,Severity,Description,SigDateTime,Tag,IsProcessed) VALUES("
						+"1,"//ListenerService
						+"0,"
						+"0,"
						+"5,"//Critical
						+"'Patient Portal users detected.  Listener Service status set to critical to trigger monitoring.',"
						+POut.DateT(DateTime.Now)+","
						+"'',"
						+"0)";
						Db.NonQ(command);
					}
				}
				else {//Oracle
					//eServices do not currently support Oracle.
				}
				command="UPDATE preference SET ValueString = '15.2.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_2_12();
		}

		///<summary></summary>
		private static void To15_2_12() {
			if(FromVersion<new Version("15.2.12.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.2.12");//No translation in convert script.
				string command="";
				//On by default to fall in line with our new patterns. This gives the user the option to turn off our new functionality for larger clinics.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ReportPandIhasClinicBreakdown','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ReportPandIhasClinicBreakdown','1')";
					Db.NonQ(command);
				}
				//Many customers have been complaining that they want the Send Claims window to validate their claims upon loading the window.
				//Per conversation with Nathan - default this setting to off because only a few large offices will need it.  We'll enhance the window later.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimsSendWindowValidatesOnLoad','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ClaimsSendWindowValidatesOnLoad','0')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '15.2.12.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_2_16();
		}

		///<summary></summary>
		private static void To15_2_16() {
			if(FromVersion<new Version("15.2.16.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.2.16");//No translation in convert script.
				string command="";
				//Customers were complaining that the Payment window splitting behavior has changed (Which it has, preferring the auto splitter)
				//This preference gives them the option of using the new hotness or to keep using the old and busted.  Defaulting on to use the new hotness.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PaymentsPromptForAutoSplit','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PaymentsPromptForAutoSplit','1')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '15.2.16.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_2_20();
		}

		///<summary></summary>
		private static void To15_2_20() {
			if(FromVersion<new Version("15.2.20.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.2.20");//No translation in convert script.
				string command="";
				command="UPDATE procedurecode SET NoBillIns = 1 WHERE ProcCode='D9986' OR ProcCode='D9987'";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '15.2.20.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_3_1();
		}

		///<summary></summary>
		private static void To15_3_1() {
			if(FromVersion<new Version("15.3.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.3.1");//No translation in convert script.
				string command="";
				//Column clinic.SmsContractName was never used.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic DROP COLUMN SmsContractName";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic DROP COLUMN SmsContractName";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD SmsMonthlyLimit double NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD SmsMonthlyLimit number(38,8)";
					Db.NonQ(command);
					command="UPDATE clinic SET SmsMonthlyLimit = 0 WHERE SmsMonthlyLimit IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clinic MODIFY SmsMonthlyLimit NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('SmsMonthlyLimit','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SmsMonthlyLimit','0')";
					Db.NonQ(command);
				}
				//Drop old sms tables with improper schema. They were never used.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS smsmt";
					Db.NonQ(command);
					command="DROP TABLE IF EXISTS smsmo";
					Db.NonQ(command);
					command="DROP TABLE IF EXISTS smsvln";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE smsmt'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE smsmo'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE smsvln'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
				}
				//Re-Add SMS Tables.  They were released in version 15.2 but were not used.  This is the updated schema.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS smsfrommobile";
					Db.NonQ(command);
					command=@"CREATE TABLE smsfrommobile (
						SmsFromMobileNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						ClinicNum bigint NOT NULL,
						CommlogNum bigint NOT NULL,
						MsgText text NOT NULL,
						DateTimeReceived datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						SmsPhoneNumber varchar(255) NOT NULL,
						MobilePhoneNumber varchar(255) NOT NULL,
						MsgPart int NOT NULL,
						MsgTotal int NOT NULL,
						MsgRefID varchar(255) NOT NULL,
						SmsStatus tinyint NOT NULL,
						Flags varchar(255) NOT NULL,
						IsHidden tinyint NOT NULL,
						INDEX(PatNum),
						INDEX(ClinicNum),
						INDEX(CommlogNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE smsfrommobile'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE smsfrommobile (
						SmsFromMobileNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						CommlogNum number(20) NOT NULL,
						MsgText clob,
						DateTimeReceived date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						SmsPhoneNumber varchar2(255),
						MobilePhoneNumber varchar2(255),
						MsgPart number(11) NOT NULL,
						MsgTotal number(11) NOT NULL,
						MsgRefID varchar2(255),
						SmsStatus number(3) NOT NULL,
						Flags varchar2(255),
						IsHidden number(3) NOT NULL,
						CONSTRAINT smsfrommobile_SmsFromMobileNum PRIMARY KEY (SmsFromMobileNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX smsfrommobile_PatNum ON smsfrommobile (PatNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX smsfrommobile_ClinicNum ON smsfrommobile (ClinicNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX smsfrommobile_CommlogNum ON smsfrommobile (CommlogNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS smsphone";
					Db.NonQ(command);
					command=@"CREATE TABLE smsphone (
						SmsPhoneNum bigint NOT NULL auto_increment PRIMARY KEY,
						ClinicNum bigint NOT NULL,
						PhoneNumber varchar(255) NOT NULL,
						DateTimeActive datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						DateTimeInactive datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						InactiveCode varchar(255) NOT NULL,
						CountryCode varchar(255) NOT NULL,
						INDEX(ClinicNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE smsphone'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE smsphone (
						SmsPhoneNum number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						PhoneNumber varchar2(255),
						DateTimeActive date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateTimeInactive date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						InactiveCode varchar2(255),
						CountryCode varchar2(255),
						CONSTRAINT smsphone_SmsPhoneNum PRIMARY KEY (SmsPhoneNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX smsphone_ClinicNum ON smsphone (ClinicNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS smstomobile";
					Db.NonQ(command);
					command=@"CREATE TABLE smstomobile (
						SmsToMobileNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						GuidMessage varchar(255) NOT NULL,
						GuidBatch varchar(255) NOT NULL,
						SmsPhoneNumber varchar(255) NOT NULL,
						MobilePhoneNumber varchar(255) NOT NULL,
						IsTimeSensitive tinyint NOT NULL,
						MsgType tinyint NOT NULL,
						MsgText text NOT NULL,
						SmsStatus tinyint NOT NULL,
						MsgParts int NOT NULL,
						MsgChargeUSD float NOT NULL,
						ClinicNum bigint NOT NULL,
						CustErrorText varchar(255) NOT NULL,
						DateTimeSent datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						DateTimeTerminated datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						IsHidden tinyint NOT NULL,
						INDEX(PatNum),
						INDEX(ClinicNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE smstomobile'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE smstomobile (
						SmsToMobileNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						GuidMessage varchar2(255),
						GuidBatch varchar2(255),
						SmsPhoneNumber varchar2(255),
						MobilePhoneNumber varchar2(255),
						IsTimeSensitive number(3) NOT NULL,
						MsgType number(3) NOT NULL,
						MsgText clob,
						SmsStatus number(3) NOT NULL,
						MsgParts number(11) NOT NULL,
						MsgChargeUSD number(38,8) NOT NULL,
						ClinicNum number(20) NOT NULL,
						CustErrorText varchar2(255),
						DateTimeSent date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateTimeTerminated date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						IsHidden number(3) NOT NULL,
						CONSTRAINT smstomobile_SmsToMobileNum PRIMARY KEY (SmsToMobileNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX smstomobile_PatNum ON smstomobile (PatNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX smstomobile_ClinicNum ON smstomobile (ClinicNum)";
					Db.NonQ(command);
				}
				//Creating connection group preference for CEMT
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ConnGroupCEMT','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ConnGroupCEMT','0')";
					Db.NonQ(command);
				}
				//Add permission to groups with existing permission------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=66";  //66 is TaskNoteEdit
				DataTable table=Db.GetTable(command);
				long groupNum;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",84)";  //84 is TaskEdit
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",84)";  //84 is TaskEdit
						Db.NonQ32(command);
					}
				}
				//Adding ApptTimeScrollStart column into apptview table.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE apptview ADD ApptTimeScrollStart time NOT NULL";
					Db.NonQ(command);
					command="UPDATE apptview SET ApptTimeScrollStart='08:00:00'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE apptview ADD ApptTimeScrollStart varchar2(255)";
					Db.NonQ(command);
					command="UPDATE apptview SET ApptTimeScrollStart='08:00:00'";
					Db.NonQ(command);
				}		
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('BillingElectBatchMax','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'BillingElectBatchMax','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ShowFeatureGoogleMaps','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ShowFeatureGoogleMaps','1')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE hl7def ADD HasLongDCodes tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE hl7def ADD HasLongDCodes number(3)";
					Db.NonQ(command);
					command="UPDATE hl7def SET HasLongDCodes = 0 WHERE HasLongDCodes IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE hl7def MODIFY HasLongDCodes NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ProgramVersionLastUpdated','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ProgramVersionLastUpdated','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(29,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(30,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(31,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(32,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(33,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(34,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(MeasureType,Numerator,Denominator) VALUES(35,-1,-1)";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),29,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),30,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),31,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),32,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),33,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),34,-1,-1)";
					Db.NonQ(command);
					command="INSERT INTO ehrmeasure(EhrMeasureNum,MeasureType,Numerator,Denominator) VALUES((SELECT MAX(EhrMeasureNum)+1 FROM ehrmeasure),35,-1,-1)";
					Db.NonQ(command);
				}
				command="SELECT COUNT(*) FROM ehrtrigger";
				int count=PIn.Int(Db.GetCount(command));
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO ehrmeasureevent (DateTEvent,EventType,PatNum,MoreInfo) VALUES("+POut.DateT(DateTime.Now)+",22,0,'Triggers currently enabled: "+count+"')";
					Db.NonQ(command);
				}
				else {
					//EHR is not Oracle compatable, so we don't worry about Oracle here.
				}
				//Associating Clinics to Fee Schedules
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE fee ADD ClinicNum bigint NOT NULL";//0 is default
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE fee ADD ClinicNum number(20)";
					Db.NonQ(command);
					command="UPDATE fee SET ClinicNum = 0 WHERE ClinicNum IS NULL";//0 is default
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE feesched ADD IsGlobal tinyint NOT NULL";//1 is default (true)
					Db.NonQ(command);
					command="UPDATE feesched SET IsGlobal = 1 WHERE IsGlobal = 0";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE feesched ADD IsGlobal number(3)";
					Db.NonQ(command);
					command="UPDATE feesched SET IsGlobal = 1 WHERE IsGlobal IS NULL";//1 is default (true)
					Db.NonQ(command);
				}
				//Creating TaskHist table, extends Task.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS taskhist";
					Db.NonQ(command);
					command=@"CREATE TABLE taskhist (
						TaskHistNum bigint NOT NULL auto_increment PRIMARY KEY,
						UserNumHist bigint NOT NULL,
						DateTStamp datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						IsNoteChange tinyint NOT NULL,
						TaskNum bigint NOT NULL,
						TaskListNum bigint NOT NULL,
						DateTask date NOT NULL DEFAULT '0001-01-01',
						KeyNum bigint NOT NULL,
						Descript text NOT NULL,
						TaskStatus tinyint NOT NULL,
						IsRepeating tinyint NOT NULL,
						DateType tinyint NOT NULL,
						FromNum bigint NOT NULL,
						ObjectType tinyint NOT NULL,
						DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						UserNum bigint NOT NULL,
						DateTimeFinished datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						PriorityDefNum bigint NOT NULL,
						INDEX(TaskNum)) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE taskhist'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE taskhist (
						TaskHistNum number(20) NOT NULL,
						UserNumHist number(20) NOT NULL,
						DateTStamp date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						IsNoteChange number(3) NOT NULL,
						TaskNum number(20) NOT NULL,
						TaskListNum number(20) NOT NULL,
						DateTask date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						KeyNum number(20) NOT NULL,
						Descript clob,
						TaskStatus number(3) NOT NULL,
						IsRepeating number(3) NOT NULL,
						DateType number(3) NOT NULL,
						FromNum number(20) NOT NULL,
						ObjectType number(3) NOT NULL,
						DateTimeEntry date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						UserNum number(20) NOT NULL,
						DateTimeFinished date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						PriorityDefNum number(20) NOT NULL,
						CONSTRAINT taskhist_TaskHistNum PRIMARY KEY (TaskHistNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX taskhist_TaskNum ON taskhist (TaskNum)";
					Db.NonQ(command);
				}
				//Add index for operatory------------------------------------------------------------------------------------------------------
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE appointment ADD INDEX (Op)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX appointment_Op ON appointment (Op)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				//Add index for operatory------------------------------------------------------------------------------------------------------
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE apptviewitem ADD INDEX (OpNum)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"CREATE INDEX apptviewitem_OpNum ON apptviewitem (OpNum)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD IsMedicalOnly tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD IsMedicalOnly number(3)";
					Db.NonQ(command);
					command="UPDATE clinic SET IsMedicalOnly = 0 WHERE IsMedicalOnly IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clinic MODIFY IsMedicalOnly NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PracticeIsMedicalOnly','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PracticeIsMedicalOnly','0')";
					Db.NonQ(command);
				}
				//Every time the payment window was opened, a tempfambal table would get created and then deleted after use.
				//We have had several complaints about orphaned tempfambal tables lingering in databases.
				//It is always okay to drop tempfambal tables in v15.3 because they are no longer used.
				//Therefore, this one time only, we're going to loop through all tempfambal tables and try to drop them.
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="SELECT DATABASE()";
						table=Db.GetTable(command);
						string databaseName=table.Rows[0][0].ToString();
						command="SHOW TABLES FROM "+databaseName+" LIKE 'tempfambal%'";
						table=Db.GetTable(command);
						for(int i=0;i<table.Rows.Count;i++) {
							command="DROP TABLE IF EXISTS "+databaseName+"."+table.Rows[i][0].ToString();
							Db.NonQ(command);
						}
					}
					else {//Oracle
						command="SELECT USER FROM dual";
						table=Db.GetTable(command);
						string ownerName=table.Rows[0][0].ToString();
						command="SELECT TABLE_NAME "
							+"FROM DBA_TABLES "
							+"WHERE TABLE_NAME LIKE 'TEMPFAMBAL%' "
							+"AND OWNER='"+ownerName+"'";
						table=Db.GetTable(command);
						for(int i=0;i<table.Rows.Count;i++) {
							command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE "+table.Rows[i][0].ToString()+"'; EXCEPTION WHEN OTHERS THEN NULL; END;";
							Db.NonQ(command);
						}
					}
				}
				catch(Exception e) {
					e.DoNothing();
					//Dropping the orphaned tempfambal tables failed.  This doesn't matter at all so just continue on with the script.
				}
				//Associating Providers to Fee Schedules
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE fee ADD ProvNum bigint NOT NULL";//0 is default
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE fee ADD ProvNum number(20)";
					Db.NonQ(command);
					command="UPDATE fee SET ProvNum = 0 WHERE ProvNum IS NULL";//0 is default
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES ('MedDefaultStopDays','7')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'MedDefaultStopDays','7')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('DxIcdVersion','9')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'DxIcdVersion','9')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD IcdVersion tinyint unsigned NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD IcdVersion number(3)";
					Db.NonQ(command);
					command="UPDATE procedurelog SET IcdVersion = 0 WHERE IcdVersion IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog MODIFY IcdVersion NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE procedurelog SET IcdVersion=9";//All ICD codes up this this point have been ICD-9 codes.
				Db.NonQ(command);
				//Get the ADA 2012 claim form primary key.  The unique ID is OD11.  There cannot be more than one such row.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD11' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT * FROM (SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD11') WHERE RowNum<=1";
				}
				DataTable tableClaimFormNum=Db.GetTable(command);
				if(tableClaimFormNum.Rows.Count>0) {//The claim form should exist, but might not if foreign.
					long claimFormNum=PIn.Long(tableClaimFormNum.Rows[0][0].ToString());
					//Change ICD version indicator from fixed text "B" (which indicates ICD-9 always),
					//to field ICDindicatorAB which indicates "AB" for ICD-10 and " B" for ICD-9.
					//The current fixed text field for ICD-9 indicator is at Y position 650 by default.  We want to allow the user to have moved the field
					//a little bit, but we need to limit the Y values affected so that we are sure we do not inadvertently affect another field (per pattern).
					command="UPDATE claimformitem SET FieldName='ICDindicatorAB',FormatString='',Width=24,XPos=XPos+12 "
						+"WHERE claimformnum="+POut.Long(claimFormNum)+" AND FieldName='FixedText' AND FormatString='B' AND YPos>=600 AND YPos<=700";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE document MODIFY DateCreated datetime NOT NULL";//times automatically updated in MySQL to 00:00:00.
					Db.NonQ(command);
				}
				else {//oracle
					//Oracle only has one date column data type which is 'date' and it already includes the time portion.
				}
				//Add ProviderFeeEdit to groups with SecurityAdmin ------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=24";
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",93)"; //93: ProviderFeeEdit
						Db.NonQ(command);
					}
				}	
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				    command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",93)";
				      Db.NonQ(command);
				  }
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patfielddef ADD ItemOrder int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patfielddef ADD ItemOrder number(11)";
					Db.NonQ(command);
					command="UPDATE patfielddef SET ItemOrder = 0 WHERE ItemOrder IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patfielddef MODIFY ItemOrder NOT NULL";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patfielddef ADD IsHidden tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patfielddef ADD IsHidden number(3)";
					Db.NonQ(command);
					command="UPDATE patfielddef SET IsHidden = 0 WHERE IsHidden IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patfielddef MODIFY IsHidden NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE creditcard ADD PayConnectToken varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE creditcard ADD PayConnectToken varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE creditcard ADD PayConnectTokenExp date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE creditcard ADD PayConnectTokenExp date";
					Db.NonQ(command);
					command="UPDATE creditcard SET PayConnectTokenExp = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE PayConnectTokenExp IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE creditcard MODIFY PayConnectTokenExp NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty(ProgramNum,PropertyDesc,PropertyValue) VALUES((SELECT ProgramNum FROM program WHERE "
						+"ProgName='eClinicalWorks'),'ProcRequireSignature',0)";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty(ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue) VALUES((SELECT MAX(ProgramPropertyNum)+1 "
						+"FROM programproperty),(SELECT ProgramNum FROM program WHERE ProgName='eClinicalWorks'),'ProcRequireSignature',0)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty(ProgramNum,PropertyDesc,PropertyValue) VALUES((SELECT ProgramNum FROM program WHERE "
						+"ProgName='eClinicalWorks'),'ProcNotesNoIncomplete',0)";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty(ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue) VALUES((SELECT MAX(ProgramPropertyNum)+1 "
						+"FROM programproperty),(SELECT ProgramNum FROM program WHERE ProgName='eClinicalWorks'),'ProcNotesNoIncomplete',0)";
					Db.NonQ(command);
				}
				//Add 4 new definitions for the Fee colors. Only the colors are editable. ------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) { //34 is DefCat.FeeColors
					command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemColor) VALUES (34,'Default',0,-16777216)"; //Black
					Db.NonQ(command);
					command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemColor) VALUES (34,'Provider',1,-15161839)"; //Greenish
					Db.NonQ(command);
					command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemColor) VALUES (34,'Clinic',2,-3176419)"; //Brownish
					Db.NonQ(command);
					command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemColor) VALUES (34,'Provider and Clinic',3,-16776961)"; //Blue
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO definition (DefNum,Category,ItemName,ItemOrder,ItemColor) VALUES ((SELECT MAX(DefNum)+1 FROM definition),34,'Default',0,-16777216)";
					Db.NonQ(command);
					command="INSERT INTO definition (DefNum,Category,ItemName,ItemOrder,ItemColor) VALUES ((SELECT MAX(DefNum)+1 FROM definition),34,'Provider',1,-15161839)";
					Db.NonQ(command);
					command="INSERT INTO definition (DefNum,Category,ItemName,ItemOrder,ItemColor) VALUES ((SELECT MAX(DefNum)+1 FROM definition),34,'Clinic',2,-3176419)";
					Db.NonQ(command);
					command="INSERT INTO definition (DefNum,Category,ItemName,ItemOrder,ItemColor) VALUES ((SELECT MAX(DefNum)+1 FROM definition),34,'Provider and Clinic',3,-16776961)";
					Db.NonQ(command);
				}
				if (DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patient ADD BillingCycleDay int NOT NULL DEFAULT 1";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patient ADD BillingCycleDay number(11) DEFAULT 1";
					Db.NonQ(command);
					command="UPDATE patient SET BillingCycleDay = 1 WHERE BillingCycleDay IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patient MODIFY BillingCycleDay NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES ('BillingUseBillingCycleDay','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'BillingUseBillingCycleDay','0')";
					Db.NonQ(command);
				}
				//A new Web Sched pref to keep track of how provider time slots are shown.  ValueString is an enum that is defaulted to 'FirstAvailable'.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES ('WebSchedProviderRule','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WebSchedProviderRule','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE operatory ADD IsWebSched tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE operatory ADD IsWebSched number(3)";
					Db.NonQ(command);
					command="UPDATE operatory SET IsWebSched = 0 WHERE IsWebSched IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE operatory MODIFY IsWebSched NOT NULL";
					Db.NonQ(command);
				}
				//Take the current logic for considering operatories as 'Web Sched' ops and set their 'IsWebSched' to true.
				//Going forward, our users will be able to manually dictate whether or not to include operatories for Web Sched.
				//Using two queries instead of one update statement because Oracle cannot use JOINs in an update statement.
				command=@"SELECT operatory.OperatoryNum 
					FROM operatory
					LEFT JOIN provider dentist ON operatory.ProvDentist=dentist.ProvNum
					LEFT JOIN provider hygienist ON operatory.ProvHygienist=hygienist.ProvNum
					WHERE (operatory.IsHidden!=1 AND (operatory.IsHygiene=1 OR (dentist.IsSecondary=1 OR hygienist.IsSecondary=1)))
					AND operatory.SetProspective=0";//Prospective operatories will be excluded from the Web Sched (convo with Nathan 01/08/2015)
				List<long> listOpNums=Db.GetListLong(command);
				if(listOpNums.Count>0) {
					command="UPDATE operatory SET IsWebSched=1 WHERE OperatoryNum IN ("+String.Join(",",listOpNums)+")";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE equipment ADD Status text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE equipment ADD Status varchar2(4000)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE supply ADD LevelOnHand float NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE supply ADD LevelOnHand number(38,8)";
					Db.NonQ(command);
					command="UPDATE supply SET LevelOnHand = 0 WHERE LevelOnHand IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE supply MODIFY LevelOnHand NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE creditcard ADD Procedures varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE creditcard ADD Procedures varchar2(255)";
					Db.NonQ(command);
				}
				//Insert Dental Intel Link
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"'DentalIntel', "
				    +"'Dental Intel from www.dentalintel.com', "
				    +"'0', "
				    +"'',"
				    +"'', "
				    +"'')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				    +") VALUES("
				    +"(SELECT MAX(ProgramNum)+1 FROM program),"
				    +"'DentalIntel', "
				    +"'Dental Intel from www.dentalintel.com', "
				    +"'0', "
				    +"'',"
				    +"'', "
				    +"'')";
					Db.NonQ(command);
				}//end Dental Intel Link
				//Preference used to point to WebServicesHQ. No UI, must be edited in the DB for now.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES ('WebServiceHQServerURL','http://www.patientviewer.com:49999/OpenDentalWebServiceHQ/WebServiceMainHQ.asmx')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WebServiceHQServerURL','http://www.patientviewer.com:49999/OpenDentalWebServiceHQ/WebServiceMainHQ.asmx')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE provider ADD ProvNumBillingOverride bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE provider ADD INDEX (ProvNumBillingOverride)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE provider ADD ProvNumBillingOverride number(20)";
					Db.NonQ(command);
					command="UPDATE provider SET ProvNumBillingOverride = 0 WHERE ProvNumBillingOverride IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE provider MODIFY ProvNumBillingOverride NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX provider_ProvNumBillingOverrid ON provider (ProvNumBillingOverride)";
					Db.NonQ(command);
				}
				//Adding Billing and PayTo address columns to Clinics -------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD BillingAddress varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD BillingAddress varchar2(255)";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD BillingAddress2 varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD BillingAddress2 varchar2(255)";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD BillingCity varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD BillingCity varchar2(255)";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD BillingState varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD BillingState varchar2(255)";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD BillingZip varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD BillingZip varchar2(255)";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD PayToAddress varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD PayToAddress varchar2(255)";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD PayToAddress2 varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD PayToAddress2 varchar2(255)";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD PayToCity varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD PayToCity varchar2(255)";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD PayToState varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD PayToState varchar2(255)";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD PayToZip varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD PayToZip varchar2(255)";
					Db.NonQ(command);
				}
				//Insert DentalTekSmartOfficePhone bridge-----------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'DentalTekSmartOfficePhone', "
						+"'DentalTekSmartOfficePhone from www.dentalsolutionsllc.com', "
						+"'0', "//Disabled by default.
						+"'"+POut.String(@"")+"', "//No Path needed
						+"'"+POut.String(@"")+"', "//No command line needed
						+"'No path or command line arguments needed.')";//Note
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"'"+POut.Long(programNum)+"', "
						+"'2', "//ToolBarsAvail.ChartModule
						+"'DentalTekSmartOfficePhone')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'DentalTekSmartOfficePhone', "
						+"'DentalTekSmartOfficePhone from www.dentalsolutionsllc.com', "
						+"'0', "//Disabled by default.
						+"'"+POut.String(@"")+"', "//No Path needed
						+"'"+POut.String(@"")+"', "//No command line needed
						+"'No path or command line arguments needed.')";//Note
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
						+"'"+POut.Long(programNum)+"', "
						+"'2', "//ToolBarsAvail.ChartModule
						+"'DentalTekSmartOfficePhone')";
					Db.NonQ(command);
				}//end DentalTekSmartOfficePhone bridge
				//Convert DentalSpecialties from an enum to a custom definition
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE provider MODIFY Specialty bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE referral MODIFY Specialty bigint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE provider MODIFY Specialty number(20)";
					Db.NonQ(command);
					command="ALTER TABLE referral MODIFY Specialty number(20)";
					Db.NonQ(command);
				}
				long[] defNums=new long[14];
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(35,0,'General','')";//35 is DefCat.ProviderSpecialties
					defNums[0]=Db.NonQ(command,true);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(35,1,'Hygienist','')";
					defNums[1]=Db.NonQ(command,true);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(35,2,'Endodontics','')";
					defNums[2]=Db.NonQ(command,true);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(35,3,'Pediatric','')";
					defNums[3]=Db.NonQ(command,true);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(35,4,'Perio','')";
					defNums[4]=Db.NonQ(command,true);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(35,5,'Prosth','')";
					defNums[5]=Db.NonQ(command,true);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(35,6,'Ortho','')";
					defNums[6]=Db.NonQ(command,true);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(35,7,'Denturist','')";
					defNums[7]=Db.NonQ(command,true);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(35,8,'Surgery','')";
					defNums[8]=Db.NonQ(command,true);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(35,9,'Assistant','')";
					defNums[9]=Db.NonQ(command,true);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(35,10,'LabTech','')";
					defNums[10]=Db.NonQ(command,true);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(35,11,'Pathology','')";
					defNums[11]=Db.NonQ(command,true);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(35,12,'PublicHealth','')";
					defNums[12]=Db.NonQ(command,true);
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES(35,13,'Radiology','')";
					defNums[13]=Db.NonQ(command,true);
				}
				else {//oracle
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),35,0,'General','')";
					defNums[0]=Db.NonQ(command,true,"DefNum","definition");
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),35,0,'Hygienist','')";
					defNums[1]=Db.NonQ(command,true,"DefNum","definition");
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),35,0,'Endodontics','')";
					defNums[2]=Db.NonQ(command,true,"DefNum","definition");
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),35,0,'Pediatric','')";
					defNums[3]=Db.NonQ(command,true,"DefNum","definition");
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),35,0,'Perio','')";
					defNums[4]=Db.NonQ(command,true,"DefNum","definition");
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),35,0,'Prosth','')";
					defNums[5]=Db.NonQ(command,true,"DefNum","definition");
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),35,0,'Ortho','')";
					defNums[6]=Db.NonQ(command,true,"DefNum","definition");
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),35,0,'Denturist','')";
					defNums[7]=Db.NonQ(command,true,"DefNum","definition");
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),35,0,'Surgery','')";
					defNums[8]=Db.NonQ(command,true,"DefNum","definition");
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),35,0,'Assistant','')";
					defNums[9]=Db.NonQ(command,true,"DefNum","definition");
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),35,0,'LabTech','')";
					defNums[10]=Db.NonQ(command,true,"DefNum","definition");
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),35,0,'Pathology','')";
					defNums[11]=Db.NonQ(command,true,"DefNum","definition");
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),35,0,'PublicHealth','')";
					defNums[12]=Db.NonQ(command,true,"DefNum","definition");
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) "
						+"VALUES((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),35,0,'Radiology','')";
					defNums[13]=Db.NonQ(command,true,"DefNum","definition");
				}
				//This is safe because the db is guaranteed to have more defs than 14
				for(int i=0;i<defNums.Length;i++) {
					command="UPDATE provider SET Specialty="+POut.Long(defNums[i])+" WHERE Specialty="+POut.Int(i);
					Db.NonQ(command);
					command="UPDATE referral SET Specialty="+POut.Long(defNums[i])+" WHERE Specialty="+POut.Int(i);
					Db.NonQ(command);
				}
				//add PatientMerge permissions for users that have setup permission ------------------------------------------------------
				command="SELECT UserGroupNum FROM grouppermission WHERE PermType=8";//POut.Int((int)Permissions.Setup)
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i][0].ToString());
						command="INSERT INTO grouppermission (NewerDate,UserGroupNum,PermType) "
						+"VALUES('0001-01-01',"+POut.Long(groupNum)+",94)";//POut.Int((int)Permissions.PatientMerge);
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i][0].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,NewerDate,UserGroupNum,PermType) "
						+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,TO_DATE('0001-01-01','YYYY-MM-DD'),"+POut.Long(groupNum)+",94)";//POut.Int((int)Permissions.PatientMerge)
						Db.NonQ32(command);
					}
				}
				//Insert Podium bridge-----------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'Podium', "
						+"'Podium from www.podium.co', "
						+"'0', "//Disabled by default.
						+"'"+POut.String(@"")+"', "
						+"'"+POut.String(@"")+"', "
						+"'No path or command line arguments needed.  The computer name or IP is the name or IP of the computer that will be sending Podium invitations. Get API Token and Location ID from Podium.')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Enter your computer name or IP (required)', "
						+"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Enter your API Token (required)', "
						+"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Enter your Location ID (required)', "
						+"'')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'Podium', "
						+"'Podium from www.podium.co', "
						+"'0', "//Disabled by default.
						+"'"+POut.String(@"")+"', "
						+"'"+POut.String(@"")+"', "
						+"'No path or command line arguments needed. The computer name or IP is the name or IP of the computer that will be sending Podium invitations. Get API Token and Location ID from Podium.')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Enter your computer name or IP (required)', "
						+"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Enter your API Token (required)', "
						+"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Enter your Location ID (required)', "
						+"'')";
					Db.NonQ(command);
				}//end Podium bridge
				//Drop and re-add the EServiceSignalTable. The content of the table does not matter at this point, 
				//this allows us to alter columns to clobs and also presrve column order.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS eservicesignal";
					Db.NonQ(command);
					command=@"CREATE TABLE eservicesignal (
						EServiceSignalNum bigint NOT NULL auto_increment PRIMARY KEY,
						ServiceCode int NOT NULL,
						ReasonCategory int NOT NULL,
						ReasonCode int NOT NULL,
						Severity tinyint NOT NULL,
						Description text NOT NULL,
						SigDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						Tag text NOT NULL,
						IsProcessed tinyint NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE eservicesignal'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE eservicesignal (
						EServiceSignalNum number(20) NOT NULL,
						ServiceCode number(11) NOT NULL,
						ReasonCategory number(11) NOT NULL,
						ReasonCode number(11) NOT NULL,
						Severity number(3) NOT NULL,
						Description clob,
						SigDateTime date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						Tag clob,
						IsProcessed number(3) NOT NULL,
						CONSTRAINT eservicesignal_EServiceSignalN PRIMARY KEY (EServiceSignalNum)
						)";
					Db.NonQ(command);
				}
				//Code identical to convert script from version 15.2.1
				//Listener Service monitoring.  As of right now the Patient Portal is the only important eService.  
				//Turn on Listener Service monitoring for offices using the patient portal by inserting an eService signal of status 'Critical'
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//Patients with OnlinePasswords will be the indicator that an office is or has attempted to use the patient portal.
					command="SELECT COUNT(*) FROM patient WHERE OnlinePassword!=''";
					int countPatPortals=PIn.Int(Db.GetCount(command));
					if(countPatPortals > 5) {//Check for more than 5 patient portal patients to avoid false positives.
						//Insert a 'Critical' signal into the eservicesignal table to trigger Listener Service monitoring.
						//Customers with active Listener Services will instantly have a 'Working' signal inserted and will not get notified of the service being down.
						//However, if the customer does not know that their service is down (not tech savy) then this will alert them to that fact (our goal).
						command="INSERT INTO eservicesignal (ServiceCode,ReasonCategory,ReasonCode,Severity,Description,SigDateTime,Tag,IsProcessed) VALUES("
						+"1,"//ListenerService
						+"0,"
						+"0,"
						+"5,"//Critical
						+"'Patient Portal users detected.  Listener Service status set to critical to trigger monitoring.',"
						+POut.DateT(DateTime.Now)+","
						+"'',"
						+"0)";
						Db.NonQ(command);
					}
				}
				else {//Oracle
					//eServices do not currently support Oracle.
				}
				command="UPDATE preference SET ValueString = '15.3.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_3_6();
		}

		private static void To15_3_6() {
			if(FromVersion<new Version("15.3.6.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.3.6");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE smsfrommobile ADD MatchCount int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE smsfrommobile ADD MatchCount number(11)";
					Db.NonQ(command);
					command="UPDATE smsfrommobile SET MatchCount = 0 WHERE MatchCount IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE smsfrommobile MODIFY MatchCount NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '15.3.6.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_3_10();
		}

		private static void To15_3_10() {
			if(FromVersion<new Version("15.3.10.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.3.10");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('DefaultCCProcs','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'DefaultCCProcs','')";
					Db.NonQ(command);
				}
				//PaymentsPromptForAutoSplit preference.  It was backported to 15.2, so we have to check for existence first.  Defaults to true.
				command="SELECT COUNT(*) FROM preference WHERE PrefName='PaymentsPromptForAutoSplit'";
				int results=PIn.Int(Db.GetCount(command));
				if(DataConnection.DBtype==DatabaseType.MySql) {
					if(results==0) {//Preference doesn't exist, insert it.
						command="INSERT INTO preference(PrefName,ValueString) VALUES('PaymentsPromptForAutoSplit','1')";
						Db.NonQ(command);
					}
				}
				else {//oracle
					if(results==0) {//Preference doesn't exist, insert it.
						command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PaymentsPromptForAutoSplit','1')";
						Db.NonQ(command);
					}
				}
				command="UPDATE preference SET ValueString = '15.3.10.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_3_13();
		}

		private static void To15_3_13() {
			if(FromVersion<new Version("15.3.13.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.3.13");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('InsWriteoffDescript','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'InsWriteoffDescript','')";
					Db.NonQ(command);
				}
				command="UPDATE procedurecode SET NoBillIns = 1 WHERE ProcCode='D9986' OR ProcCode='D9987'";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '15.3.13.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_3_16();
		}

		private static void To15_3_16() {
			if(FromVersion<new Version("15.3.16.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.3.16");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE creditcard CHANGE Procedures Procedures TEXT NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE creditcard MODIFY (Procedures varchar2(4000))";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '15.3.16.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_3_19();
		}

		private static void To15_3_19() {
			if(FromVersion<new Version("15.3.19.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.3.19");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('BrokenApptAdjustmentWithProcedure','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT MAX(PrefNum)+1 FROM preference),'BrokenApptAdjustmentWithProcedure','0')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '15.3.19.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_3_21();
		}

		private static void To15_3_21() {
			if(FromVersion<new Version("15.3.21.0")) {
				string command="SELECT COUNT(*) FROM medlab WHERE PatNum=0";
				string isReconciled="0";
				if(Db.GetCount(command)=="0") {
					isReconciled="1";
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('MedLabReconcileDone','"+isReconciled+"')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT MAX(PrefNum)+1 FROM preference),'MedLabReconcileDone','"+isReconciled+"')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '15.3.21.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_3_22();
		}

		private static void To15_3_22() {
			if(FromVersion<new Version("15.3.22.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.3.22");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD UseBillAddrOnClaims tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD UseBillAddrOnClaims number(3)";
					Db.NonQ(command);
					command="UPDATE clinic SET UseBillAddrOnClaims = 0 WHERE UseBillAddrOnClaims IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clinic MODIFY UseBillAddrOnClaims NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE clinic SET UseBillAddrOnClaims=CASE WHEN BillingAddress='' THEN 0 ELSE 1 END";//For backwards compatibility to version 15.3
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '15.3.22.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_4_1();
		}

		private static void To15_4_1() {
			if(FromVersion<new Version("15.4.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.4.1");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PatientSelectUseFNameForPreferred','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),"+
					"'PatientSelectUseFNameForPreferred','0')";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurecode ADD CanadaTimeUnits double NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurecode ADD CanadaTimeUnits number(38,8)";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits = 0 WHERE CanadaTimeUnits IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurecode MODIFY CanadaTimeUnits NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE procedurecode SET CanadaTimeUnits=1";//This is the correct value for most Canadian procedure codes.
				Db.NonQ(command);
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='02802'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=0.5 WHERE procedurecode.ProcCode='04507'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='04712'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='04713'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='04714'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='04722'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='04723'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='04724'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='04732'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='04733'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='04734'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='05102'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='05103'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='05104'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='05202'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='07022'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=0.5 WHERE procedurecode.ProcCode='07027'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='07032'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=0.5 WHERE procedurecode.ProcCode='07037'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='11102'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='11112'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='11113'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='11114'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=5 WHERE procedurecode.ProcCode='11115'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=6 WHERE procedurecode.ProcCode='11116'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=0.5 WHERE procedurecode.ProcCode='11117'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='13102'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='13103'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='13104'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='13212'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='13213'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='13214'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='13222'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='13223'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='13224'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='13232'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='13242'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='13302'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='13303'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='13304'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='13602'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='14312'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='14322'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='14402'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='14403'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='14622'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='14623'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='14732'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='14733'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='16102'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='16103'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='16202'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='16203'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='16512'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='16513'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='16514'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=0.5 WHERE procedurecode.ProcCode='16517'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='25782'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='25783'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='25784'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='29102'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='29103'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='29104'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='29302'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='29303'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='29304'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='39312'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='39313'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='41212'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='41213'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='41214'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='41222'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='41223'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='41224'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='41232'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='41233'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='41234'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='41302'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='42822'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='42823'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='42832'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='42833'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='42834'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='43312'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='43313'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='43314'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='43422'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='43423'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='43424'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=5 WHERE procedurecode.ProcCode='43425'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=6 WHERE procedurecode.ProcCode='43426'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=0.5 WHERE procedurecode.ProcCode='43427'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='43622'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='43623'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='43732'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='43733'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='43812'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='43813'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='49102'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='54202'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='66212'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='66213'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='66214'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='66222'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='66223'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='66224'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='66302'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='66303'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='66304'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='91112'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='91113'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='91122'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='91123'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='91212'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='91213'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='91232'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='91233'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='91234'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='92212'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='92213'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='92214'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=5 WHERE procedurecode.ProcCode='92215'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=6 WHERE procedurecode.ProcCode='92216'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=7 WHERE procedurecode.ProcCode='92217'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=8 WHERE procedurecode.ProcCode='92218'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='92222'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='92223'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='92224'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=5 WHERE procedurecode.ProcCode='92225'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=6 WHERE procedurecode.ProcCode='92226'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=7 WHERE procedurecode.ProcCode='92227'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=8 WHERE procedurecode.ProcCode='92228'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='92302'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='92303'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='92304'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=5 WHERE procedurecode.ProcCode='92305'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=6 WHERE procedurecode.ProcCode='92306'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=7 WHERE procedurecode.ProcCode='92307'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=8 WHERE procedurecode.ProcCode='92308'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='92322'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='92323'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='92324'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=5 WHERE procedurecode.ProcCode='92325'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=6 WHERE procedurecode.ProcCode='92326'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=7 WHERE procedurecode.ProcCode='92327'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=8 WHERE procedurecode.ProcCode='92328'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='92412'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='92413'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=4 WHERE procedurecode.ProcCode='92414'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=5 WHERE procedurecode.ProcCode='92415'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=6 WHERE procedurecode.ProcCode='92416'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=7 WHERE procedurecode.ProcCode='92417'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=8 WHERE procedurecode.ProcCode='92418'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='93112'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=2 WHERE procedurecode.ProcCode='97112'";
					Db.NonQ(command);
					command="UPDATE procedurecode SET CanadaTimeUnits=3 WHERE procedurecode.ProcCode='97113'";
					Db.NonQ(command);
				}
				//Add ConnectionStatus column to centralconnections -------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE centralconnection ADD ConnectionStatus varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE centralconnection ADD ConnectionStatus varchar2(255)";
					Db.NonQ(command);
				}
				//The ButtonImage must be a 22 x 22 image, and thus needs (width) x (height) x (depth) = 22 x 22 x 4 = 1936 bytes
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE program ADD ButtonImage text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE program ADD ButtonImage varchar2(4000)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('FinanceChargeAtLeast','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),"+
					"'FinanceChargeAtLeast','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('FinanceChargeOnlyIfOver','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),"+
					"'FinanceChargeOnlyIfOver','0')";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE etrans ADD CarrierNameRaw varchar(60) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE etrans ADD CarrierNameRaw varchar2(60)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE etrans ADD PatientNameRaw varchar(133) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE etrans ADD PatientNameRaw varchar2(133)";
					Db.NonQ(command);
				}
				//if(DataConnection.DBtype==DatabaseType.MySql) {
				//	command="INSERT INTO preference(PrefName,ValueString) VALUES('DefaultCCProcs','')";
				//	Db.NonQ(command);
				//}
				//else {//oracle
				//	command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'DefaultCCProcs','')";
				//	Db.NonQ(command);
				//}
				//Adding picklist column to DisplayFields.  Specifically for Ortho Chart.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE displayfield ADD PickList text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE displayfield ADD PickList clob";
					Db.NonQ(command);
				}
				//Insert the company logo for DentalTek for all users.
				command="UPDATE program "
				+"SET ButtonImage='iVBORw0KGgoAAAANSUhEUgAAABYAAAAWCAYAAADEtGw7AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAACxEA"
					+"AAsRAX9kX5EAAAAadEVYdFNvZnR3YXJlAFBhaW50Lk5FVCB2My41LjExR/NCNwAAAeFJREFUSEtjYAjb8RSI/1MZnx41GIYRBmfPvfb/zadfROEXH37+P3Xnw//+LQ/+G5Yf+88Uj"
					+"sfgksU3/pMDvv788z9n3jXqGwwCa068oL7Bn7//+e/RdoYyg//+/ff/959//3/8+vv/ydvv/xcefPrfvPoEOIx54naDMVaDv/z48//yo8//t5x99X/qjof/a1fe/l+06Mb/uKmXw"
					+"Ni99cx/86rj/6XS94MN4E/Y81+76Mj/zDlX/199/Pl/QPc5TINV8w791yw88p8rFmIrb/wecIz7dp79nz7r6n/RlH1gsfb19/6vOPr8/5Eb7/7fefEVHBQwEDXpIqbBICyRtv9/z"
					+"Yrb/y89/PT/779/UOX//5+79xHszfwF16Ei2AFWg+WyDoANRAcg81NnXvnPFbP7/2NguOIDbi2nMQ0uWog98h6+/g52LSit4gMfv/4GOw7DYFypogDofVAkXXr4GSqCHaw9+eI/S"
					+"8RO4gy+9ezrf5Hkff8Tpl4GBwku8Oz9j/8ahYdhhuI3GGRQIdC1rJE7/5+5+xEqigr+ANP1wWvv/hsBUw+SoagGg3LPvP1P4HjS9of/uYFhK51x4P/cfQhxEJ668xE4iEwrj//nj"
					+"NmFbiiqwVTGQ9Tg+UC8kbp4Ry8AAWMmQ44WLjkAAAAASUVORK5CYII=' "
				+"WHERE ProgName='DentalTekSmartOfficePhone'";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE securitylog ADD DefNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE securitylog ADD INDEX (DefNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE securitylog ADD DefNum number(20)";
					Db.NonQ(command);
					command="UPDATE securitylog SET DefNum = 0 WHERE DefNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE securitylog MODIFY DefNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX securitylog_DefNum ON securitylog (DefNum)";
					Db.NonQ(command);
				}
				//Insert Scanora bridge-----------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'Scanora', "
						+"'Scanora from www.soredex.com', "
						+"'0', "
						+"'"+POut.String(@"C:\Scanora\Scanora.exe")+"', "
						+"'', "//leave blank if none
						+"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNum)+"', "
				    +"'Import.ini path', "
				    +"'"+POut.String(@"C:\Scanora\Scanora.ini")+"')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"'"+POut.Long(programNum)+"', "
						+"'2', "//ToolBarsAvail.ChartModule
						+"'Scanora')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'Scanora', "
						+"'Scanora from www.soredex.com', "
						+"'0', "
						+"'"+POut.String(@"C:\Scanora\Scanora.exe")+"', "
						+"'', "//leave blank if none
						+"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
				    +"'"+POut.Long(programNum)+"', "
				    +"'Import.ini path', "
				    +"'"+POut.String(@"C:\Scanora\Scanora.ini")+"')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
						+"'"+POut.Long(programNum)+"', "
						+"'2', "//ToolBarsAvail.ChartModule
						+"'Scanora')";
					Db.NonQ(command);
				}//end Scanora bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('UseProviderColorsInChart','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'UseProviderColorsInChart','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptModuleDefaultToWeek','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptModuleDefaultToWeek','0')";
					Db.NonQ(command);
				}
				//Give all users with Appointment Edit permission the new Appointment Complete Edit permission------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=27";//AppointmentEdit
				DataTable table=Db.GetTable(command);
				long groupNum;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",96)";//AppointmentCompleteEdit
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",96)";//AppointmentCompleteEdit
						Db.NonQ32(command);
					}
				}
				command="SELECT ItemColor FROM definition WHERE Category=0 AND ItemName='Adjustment'";//Category 0 is account defs.
				string adjAccountItemColor=Db.GetScalar(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO definition(ItemName,Category,ItemOrder,ItemColor) "
						+"VALUES('Broken Appointment Procedure',0,10,"+POut.String(adjAccountItemColor)+")";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO definition(DefNum,ItemName,Category,ItemOrder,ItemColor) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),'Broken Appointment Procedure',0,10,"+POut.String(adjAccountItemColor)+")";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE emailmessage ADD CcAddress text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE emailmessage ADD CcAddress clob";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE emailmessage ADD BccAddress text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE emailmessage ADD BccAddress clob";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE emailtemplate ADD Description text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE emailtemplate ADD Description varchar2(255)";
					Db.NonQ(command);
				}
				command="UPDATE emailtemplate SET Description=emailtemplate.Subject";//Oracle compatible.
				Db.NonQ(command);
				command="UPDATE procedurecode SET NoBillIns = 1 WHERE ProcCode='D9986' OR ProcCode='D9987'";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty(ProgramNum,PropertyDesc,PropertyValue) VALUES((SELECT ProgramNum FROM program WHERE "
						+"ProgName='Xcharge'),'PromptSignature','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty(ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue) "
						+"VALUES((SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),"
						+"(SELECT ProgramNum FROM program WHERE ProgName='Xcharge'),'PromptSignature','1')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty(ProgramNum,PropertyDesc,PropertyValue) VALUES((SELECT ProgramNum FROM program WHERE "
						+"ProgName='Xcharge'),'PrintReceipt','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty(ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue) "
						+"VALUES((SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),"
						+"(SELECT ProgramNum FROM program WHERE ProgName='Xcharge'),'PrintReceipt','1')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimReportComputerName', '')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ClaimReportComputerName','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimReportReceiveInterval', '5')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES "
						+"((SELECT MAX(PrefNum)+1 FROM preference),'ClaimReportReceiveInterval','5')";
					Db.NonQ(command);
				}
				//Add ClinicNum to programproperty table for clinic specific PayConnect credentials
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE programproperty ADD ClinicNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE programproperty ADD INDEX (ClinicNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE programproperty ADD ClinicNum number(20)";
					Db.NonQ(command);
					command="UPDATE programproperty SET ClinicNum = 0 WHERE ClinicNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE programproperty MODIFY ClinicNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX programproperty_ClinicNum ON programproperty (ClinicNum)";
					Db.NonQ(command);
				}
				//Now add the three programproperty rows, Username, Password, and PaymentType, for each clinic in the database
				command="SELECT ClinicNum FROM clinic";
				List<long> listClinicNums=Db.GetListLong(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<listClinicNums.Count;i++) {
						command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
							+"(SELECT programproperty.ProgramNum,PropertyDesc,PropertyValue,ComputerName,"+listClinicNums[i]+" "
							+"FROM program INNER JOIN programproperty ON program.ProgramNum=programproperty.ProgramNum "
							+"WHERE program.ProgName='PayConnect' AND programproperty.ClinicNum=0)";
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<listClinicNums.Count;i++) {
						command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
							+"(SELECT (SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),programproperty.ProgramNum,PropertyDesc,PropertyValue,"
							+"ComputerName,"+listClinicNums[i]+" "
							+"FROM program INNER JOIN programproperty ON program.ProgramNum=programproperty.ProgramNum "
							+"WHERE program.ProgName='PayConnect' AND programproperty.ClinicNum=0)";
						Db.NonQ32(command);
					}
				}
				//Inserting new definition for task ClaimPaymentTracking defcat
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor) "
						+"VALUES(36,0,'Default','',0)";//Inserting definition with category 36 (ClaimPaymentTracking) and default value
					Db.NonQ(command);
				}
				else {
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue,ItemColor) "
						+"VALUES((SELECT MAX(DefNum)+1 FROM definition),36,0,'Default','',0)";//36 (ClaimPaymentTracking) and default value
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claimproc ADD ClaimPaymentTracking bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimproc ADD INDEX (ClaimPaymentTracking)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claimproc ADD ClaimPaymentTracking number(20)";
					Db.NonQ(command);
					command="UPDATE claimproc SET ClaimPaymentTracking = 0 WHERE ClaimPaymentTracking IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimproc MODIFY ClaimPaymentTracking NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX claimproc_ClaimPaymentTracking ON claimproc (ClaimPaymentTracking)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS emailautograph";
					Db.NonQ(command);
					command=@"CREATE TABLE emailautograph (
						EmailAutographNum bigint NOT NULL auto_increment PRIMARY KEY,
						Description text NOT NULL,
						EmailAddress varchar(255) NOT NULL,
						AutographText text NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE emailautograph'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE emailautograph (
						EmailAutographNum number(20) NOT NULL,
						Description clob,
						EmailAddress varchar2(255),
						AutographText clob,
						CONSTRAINT emailautograph_EmailAutographN PRIMARY KEY (EmailAutographNum)
						)";
					Db.NonQ(command);
				}
				command="SELECT ClearinghouseNum FROM clearinghouse WHERE CommBridge=11";//MercuryDE
				string mercuryClearinghouseNum=Db.GetScalar(command);
				//Check to see if either dental or medical default clearinghouses are set to MercuryDE
				command="SELECT ValueString FROM preference WHERE PrefName='ClearinghouseDefaultDent'";
				string defaultDentClearinghouseNum=Db.GetScalar(command);
				command="SELECT ValueString FROM preference WHERE PrefName='ClearinghouseDefaultMed'";
				string defaultMedClearinghouseNum=Db.GetScalar(command);
				//Clear out the default clearinghouses if either are set to MercuryDE because we are going to be hiding it.
				if(mercuryClearinghouseNum==defaultDentClearinghouseNum) {
					command="UPDATE preference SET ValueString='0' WHERE PrefName='ClearinghouseDefaultDent'";
					Db.NonQ(command);
				}
				if(mercuryClearinghouseNum==defaultMedClearinghouseNum) {
					command="UPDATE preference SET ValueString='0' WHERE PrefName='ClearinghouseDefaultMed'";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('NewCropDateLastAccessCheck','0001-01-01')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT MAX(PrefNum)+1 FROM preference),'NewCropDateLastAccessCheck','0001-01-01')";
					Db.NonQ(command);
				}
				bool isLegacyErx=false;
				command="SELECT ValueString FROM preference WHERE PrefName='NewCropAccountId'";
				DataTable tableAccountId=Db.GetTable(command);
				if(tableAccountId.Rows[0]["ValueString"].ToString()!="") {//NewCropAccountId is not blank.
					isLegacyErx=true;
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('NewCropIsLegacy','"+(isLegacyErx?"1":"0")+"')";
					Db.NonQ(command);
					command="INSERT INTO preference(PrefName,ValueString) VALUES('NewCropIsLexiData','"+(isLegacyErx?"0":"1")+"')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT MAX(PrefNum)+1 FROM preference),'NewCropIsLegacy','"+(isLegacyErx?"1":"0")+"')";
					Db.NonQ(command);
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT MAX(PrefNum)+1 FROM preference),'NewCropIsLexiData','"+(isLegacyErx?"0":"1")+"')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS providererx";
					Db.NonQ(command);
					command=@"CREATE TABLE providererx (
						ProviderErxNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						NationalProviderID varchar(255) NOT NULL,
						IsEnabled tinyint NOT NULL,
						IsIdentifyProofed tinyint NOT NULL,
						IsSentToHq tinyint NOT NULL,
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE providererx'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE providererx (
						ProviderErxNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						NationalProviderID varchar2(255),
						IsEnabled number(3) NOT NULL,
						IsIdentifyProofed number(3) NOT NULL,
						IsSentToHq number(3) NOT NULL,
						CONSTRAINT providererx_ProviderErxNum PRIMARY KEY (ProviderErxNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX providererx_PatNum ON providererx (PatNum)";
					Db.NonQ(command);
				}
				//Copy NPIs from the provider table to the providererx table if the customer has already been using eRx.
				//For other customers, records will be copied one-by-one as providers attempt to access eRx.
				if(isLegacyErx) {
					command="SELECT * FROM provider";
					DataTable tableProviders=Db.GetTable(command);
					//Copy provider NPIs from the provider table to the providererx table for providers who have likely accessed eRx in the past.
					List<string> listNpis=new List<string>();
					for(int i=0;i<tableProviders.Rows.Count;i++) {
						bool isNotPerson=PIn.Bool(tableProviders.Rows[i]["IsNotPerson"].ToString());
						if(isNotPerson) {
							continue;
						}
						string fname=PIn.String(tableProviders.Rows[i]["FName"].ToString()).Trim();
						if(fname=="") {
							continue;
						}
						if(Regex.Replace(fname,"[^A-Za-z\\-]*","")!=fname) {
							continue;
						}
						string lname=PIn.String(tableProviders.Rows[i]["LName"].ToString()).Trim();
						if(lname=="") {
							continue;
						}
						if(Regex.Replace(lname,"[^A-Za-z\\-]*","")!=lname) {
							continue;
						}
						string deaNum=PIn.String(tableProviders.Rows[i]["DEANum"].ToString());
						if(deaNum.ToLower()!="none" && !Regex.IsMatch(deaNum,"^[A-Za-z]{2}[0-9]{7}$")) {
							continue;
						}
						string npi=PIn.String(tableProviders.Rows[i]["NationalProvID"].ToString());
						npi=Regex.Replace(npi,"[^0-9]*","");//NPI with all non-numeric characters removed.
						if(npi.Length!=10) {
							continue;
						}
						string stateLicense=PIn.String(tableProviders.Rows[i]["StateLicense"].ToString());
						if(stateLicense=="") {
							continue;
						}
						if(!listNpis.Contains(npi)) {//Do not duplicate NPI in providererx table.
							listNpis.Add(npi);
						}
					}
					for(int i=0;i<listNpis.Count;i++) {
						string npi=listNpis[i];
						if(DataConnection.DBtype==DatabaseType.MySql) {
							command="INSERT INTO providererx (PatNum,NationalProviderID,IsEnabled,IsIdentifyProofed,IsSentToHq) VALUES "
								+"(0,'"+POut.String(npi)+"',1,0,0)";//All legacy providers are enabled by default.
							Db.NonQ(command);
						}
						else {//oracle
							command="INSERT INTO providererx (ProviderErxNum,PatNum,NationalProviderID,IsEnabled,IsIdentifyProofed,IsSentToHq) VALUES "
								+"((SELECT MAX(ProviderErxNum)+1 FROM providererx),0,'"+POut.String(npi)+"',1,0,0)";//All legacy providers are enabled by default.
							Db.NonQ(command);
						}
					}
				}
				//New State Abbreviations table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS stateabbr";
					Db.NonQ(command);
					command=@"CREATE TABLE stateabbr (
						StateAbbrNum bigint NOT NULL auto_increment PRIMARY KEY,
						Description varchar(50) NOT NULL,
						Abbr varchar(50) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE stateabbr'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE stateabbr (
						StateAbbrNum number(20) NOT NULL,
						Description varchar2(50),
						Abbr varchar2(50),
						CONSTRAINT stateabbr_StateAbbrNum PRIMARY KEY (StateAbbrNum)
						)";
					Db.NonQ(command);
				}
				if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
					//Insert all 50 US states and Washington DC
					List<string> listStateAbbrs=new List<string>() { "(1,'Alabama','AL')","(2,'Alaska','AK')","(3,'Arizona','AZ')","(4,'Arkansas','AR')"
						,"(5,'California','CA')","(6,'Colorado','CO')","(7,'Connecticut','CT')","(8,'Delaware','DE')","(9,'District of Columbia','DC')"
						,"(10,'Florida','FL')","(11,'Georgia','GA')","(12,'Hawaii','HI')","(13,'Idaho','ID')","(14,'Illinois','IL')","(15,'Indiana','IN')"
						,"(16,'Iowa','IA')","(17,'Kansas','KS')","(18,'Kentucky','KY')","(19,'Louisiana','LA')","(20,'Maine','ME')","(21,'Maryland','MD')"
						,"(22,'Massachusetts','MA')","(23,'Michigan','MI')","(24,'Minnesota','MN')","(25,'Mississippi','MS')","(26,'Missouri','MO')"
						,"(27,'Montana','MT')","(28,'Nebraska','NE')","(29,'Nevada','NV')","(30,'New Hampshire','NH')","(31,'New Jersey','NJ')"
						,"(32,'New Mexico','NM')","(33,'New York','NY')","(34,'North Carolina','NC')","(35,'North Dakota','ND')","(36,'Ohio','OH')"
						,"(37,'Oklahoma','OK')","(38,'Oregon','OR')","(39,'Pennsylvania','PA')","(40,'Rhode Island','RI')","(41,'South Carolina','SC')"
						,"(42,'South Dakota','SD')","(43,'Tennessee','TN')","(44,'Texas','TX')","(45,'Utah','UT')","(46,'Vermont','VT')","(47,'Virginia','VA')"
						,"(48,'Washington','WA')","(49,'West Virginia','WV')","(50,'Wisconsin','WI')","(51,'Wyoming','WY')"
					};
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO stateabbr (StateAbbrNum,Description,Abbr) VALUES"+string.Join(",",listStateAbbrs);
						Db.NonQ(command);
					}
					else {//Oracle
						//Oracle is not smart enough to insert multiple values in the same insert statement, so we'll make roughly 50 separate inserts.
						foreach(string stateAbbr in listStateAbbrs) {
							command="INSERT INTO stateabbr (StateAbbrNum,Description,Abbr) VALUES"+stateAbbr;
							Db.NonQ(command);
						}
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE program ADD FileTemplate text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE program ADD FileTemplate varchar2(4000)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE program ADD FilePath varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE program ADD FilePath varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE treatplan ADD DocNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE treatplan ADD INDEX (DocNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE treatplan ADD DocNum number(20)";
					Db.NonQ(command);
					command="UPDATE treatplan SET DocNum = 0 WHERE DocNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE treatplan MODIFY DocNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX treatplan_DocNum ON treatplan (DocNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('TreatPlanSaveSignedToPdf','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'TreatPlanSaveSignedToPdf','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS requiredfield";
					Db.NonQ(command);
					command=@"CREATE TABLE requiredfield (
						RequiredFieldNum bigint NOT NULL auto_increment PRIMARY KEY,
						FieldType tinyint NOT NULL,
						FieldName varchar(50) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE requiredfield'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE requiredfield (
						RequiredFieldNum number(20) NOT NULL,
						FieldType number(3) NOT NULL,
						FieldName varchar2(50),
						CONSTRAINT requiredfield_RequiredFieldNum PRIMARY KEY (RequiredFieldNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS requiredfieldcondition";
					Db.NonQ(command);
					command=@"CREATE TABLE requiredfieldcondition (
						RequiredFieldConditionNum bigint NOT NULL auto_increment PRIMARY KEY,
						RequiredFieldNum bigint NOT NULL,
						ConditionType varchar(50) NOT NULL,
						Operator tinyint NOT NULL,
						ConditionValue varchar(255) NOT NULL,
						ConditionRelationship tinyint NOT NULL,
						INDEX(RequiredFieldNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE requiredfieldcondition'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE requiredfieldcondition (
						RequiredFieldConditionNum number(20) NOT NULL,
						RequiredFieldNum number(20) NOT NULL,
						ConditionType varchar2(50),
						Operator number(3) NOT NULL,
						ConditionValue varchar2(255),
						ConditionRelationship number(3) NOT NULL,
						CONSTRAINT requiredfieldcondition_Require PRIMARY KEY (RequiredFieldConditionNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX requiredfieldcondition_Field ON requiredfieldcondition (RequiredFieldNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PriProvDefaultToSelectProv','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PriProvDefaultToSelectProv','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrpatient ADD MedicaidState varchar(50) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ehrpatient ADD MedicaidState varchar2(50)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE stateabbr ADD MedicaidIDLength int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE stateabbr ADD MedicaidIDLength number(11)";
					Db.NonQ(command);
					command="UPDATE stateabbr SET MedicaidIDLength = 0 WHERE MedicaidIDLength IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE stateabbr MODIFY MedicaidIDLength NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('EnforceMedicaidIDLength','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'EnforceMedicaidIDLength','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD IsCpoe tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD IsCpoe number(3)";
					Db.NonQ(command);
					command="UPDATE procedurelog SET IsCpoe = 0 WHERE IsCpoe IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog MODIFY IsCpoe NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurecode ADD IsRadiology tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurecode ADD IsRadiology number(3)";
					Db.NonQ(command);
					command="UPDATE procedurecode SET IsRadiology = 0 WHERE IsRadiology IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurecode MODIFY IsRadiology NOT NULL";
					Db.NonQ(command);
				}
				//Set D0210 - D0340 to "IsRadiology"
				for(int i=210;i<=340;i++) {
					command="UPDATE procedurecode SET IsRadiology=1 WHERE ProcCode='D0"+i.ToString()+"'";
					Db.NonQ(command);
				}
				//Set D0364 - D0386 to "IsRadiology"
				for(int i=364;i<=386;i++) {
					command="UPDATE procedurecode SET IsRadiology=1 WHERE ProcCode='D0"+i.ToString()+"'";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clearinghouse ADD ClinicNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE clearinghouse ADD INDEX (ClinicNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clearinghouse ADD ClinicNum number(20)";
					Db.NonQ(command);
					command="UPDATE clearinghouse SET ClinicNum = 0 WHERE ClinicNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clearinghouse MODIFY ClinicNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX clearinghouse_ClinicNum ON clearinghouse (ClinicNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clearinghouse ADD HqClearinghouseNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE clearinghouse ADD INDEX (HqClearinghouseNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clearinghouse ADD HqClearinghouseNum number(20)";
					Db.NonQ(command);
					command="UPDATE clearinghouse SET HqClearinghouseNum = 0 WHERE HqClearinghouseNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clearinghouse MODIFY HqClearinghouseNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX clearinghouse_HqClearinghouseN ON clearinghouse (HqClearinghouseNum)";
					Db.NonQ(command);
				}
				command="UPDATE clearinghouse SET HqClearinghouseNum=ClearinghouseNum";
				Db.NonQ(command);
				command="SELECT ProgramNum FROM program WHERE ProgName='DentalTekSmartOfficePhone'";
				long programNumCur=PIn.Long(Db.GetScalar(command));
				if(programNumCur!=0) {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				    +") VALUES("
				    +"'"+POut.Long(programNumCur)+"', "
				    +"'Enter your API Token', "
				    +"'')";
						Db.NonQ(command);
					}
					else {//oracle
						command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
							+") VALUES("
							+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
							+"'"+POut.Long(programNumCur)+"', "
							+"'Enter your API Token', "
							+"'', "
							+"0)";
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ImeCompositionCompatibility','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ImeCompositionCompatibility','0')";
					Db.NonQ(command);
				}
				//Add permission to groups with existing permission PatientMerge
				command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=94";//PatientMerge
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",99)";//ReferralMerge
						Db.NonQ(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",99)";//ReferralMerge
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('TimeCardShowSeconds','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'TimeCardShowSeconds','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS ebill";
					Db.NonQ(command);
					command=@"CREATE TABLE ebill (
						EbillNum bigint NOT NULL auto_increment PRIMARY KEY,
						ClinicNum bigint NOT NULL,
						ClientAcctNumber varchar(255) NOT NULL,
						ElectUserName varchar(255) NOT NULL,
						ElectPassword varchar(255) NOT NULL,
						INDEX(ClinicNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE ebill'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE ebill (
						EbillNum number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						ClientAcctNumber varchar2(255),
						ElectUserName varchar2(255),
						ElectPassword varchar2(255),
						CONSTRAINT ebill_EbillNum PRIMARY KEY (EbillNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX ebill_ClinicNum ON ebill (ClinicNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS claimsnapshot";
					Db.NonQ(command);
					command=@"CREATE TABLE claimsnapshot (
						ClaimSnapshotNum bigint NOT NULL auto_increment PRIMARY KEY,
						ProcNum bigint NOT NULL,
						ClaimType varchar(255) NOT NULL,
						Writeoff double NOT NULL,
						InsPayEst double NOT NULL,
						Fee double NOT NULL,
						DateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						INDEX(ProcNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE claimsnapshot'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE claimsnapshot (
						ClaimSnapshotNum number(20) NOT NULL,
						ProcNum number(20) NOT NULL,
						ClaimType varchar2(255),
						Writeoff number(38,8) NOT NULL,
						InsPayEst number(38,8) NOT NULL,
						Fee number(38,8) NOT NULL,
						DateTEntry date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT claimsnapshot_ClaimSnapshotNum PRIMARY KEY (ClaimSnapshotNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX claimsnapshot_ProcNum ON claimsnapshot (ProcNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimSnapshotEnabled','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ClaimSnapshotEnabled','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('TreatPlanUseSheets','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'TreatPlanUseSheets','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('RadiologyDateStartedUsing154',"+POut.DateT(DateTime.Now.Date)+")";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),"
						+"'RadiologyDateStartedUsing154',"+POut.DateT(DateTime.Now.Date)+")";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE provider ADD CustomID varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE provider ADD CustomID varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE provider ADD ProvStatus tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE provider ADD ProvStatus number(3)";
					Db.NonQ(command);
					command="UPDATE provider SET ProvStatus = 0 WHERE ProvStatus IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE provider MODIFY ProvStatus NOT NULL";
					Db.NonQ(command);
				}
				//Add provider merge permission to groups with existing permission SecurityAdmin
				command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=24";//SecurityAdmin
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",100)";//ProviderMerge
						Db.NonQ(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",100)";//ProviderMerge
						Db.NonQ(command);
					}
				}
				//Change IsWebSched column to "Source" to allow use of enums.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE commlog CHANGE IsWebSched CommSource tinyint";//Source is a keyword in MySQL
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE commlog RENAME COLUMN IsWebSched TO CommSource";//Source is a keyword in MySQL
					Db.NonQ(command);
				}
				//Add a FK to the program table which will be used in conjunction with the above enum (CommSource)
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE commlog ADD ProgramNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE commlog ADD INDEX (ProgramNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE commlog ADD ProgramNum number(20)";
					Db.NonQ(command);
					command="UPDATE commlog SET ProgramNum = 0 WHERE ProgramNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE commlog MODIFY ProgramNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX commlog_ProgramNum ON commlog (ProgramNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('AutomaticCommunicationTimeStart','01-01-0001 07:00:00')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AutomaticCommunicationTimeStart','01-01-0001 07:00:00')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('AutomaticCommunicationTimeEnd','01-01-0001 22:00:00')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AutomaticCommunicationTimeEnd','01-01-0001 22:00:00')";
					Db.NonQ(command);
				}
				//This index was added to greatly increase the speed of the select patient window when searching by chartnum
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patient ADD INDEX (ChartNumber)";
					Db.NonQ(command);
				}
				else {//oracle
					command=@"CREATE INDEX patient_ChartNumber ON patient (ChartNumber)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ebill ADD PracticeAddress tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ebill ADD PracticeAddress number(3)";
					Db.NonQ(command);
					command="UPDATE ebill SET PracticeAddress = 0 WHERE PracticeAddress IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE ebill MODIFY PracticeAddress NOT NULL";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ebill ADD RemitAddress tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ebill ADD RemitAddress number(3)";
					Db.NonQ(command);
					command="UPDATE ebill SET RemitAddress = 0 WHERE RemitAddress IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE ebill MODIFY RemitAddress NOT NULL";
					Db.NonQ(command);
				}
				//Create default Ebill object
				command="SELECT ValueString FROM preference WHERE PrefName = 'BillingElectUserName'";
				string billingElectUserName=Db.GetScalar(command);
				command="SELECT ValueString FROM preference WHERE PrefName = 'BillingElectPassword'";
				string billingElectPassword=Db.GetScalar(command);
				command="SELECT ValueString FROM preference WHERE PrefName = 'BillingElectClientAcctNumber'";
				string billingElectClientAcctNumber=Db.GetScalar(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO ebill (ClinicNum,ClientAcctNumber,ElectUserName,ElectPassword,PracticeAddress,RemitAddress) VALUES (0,'"
						+POut.String(billingElectClientAcctNumber)+"','"+POut.String(billingElectUserName)+"','"+POut.String(billingElectPassword)+"',"
						+POut.Int((int)EbillAddress.PracticePhysical)+","+POut.Int((int)EbillAddress.PracticeBilling)+")";
				}
				else {//oracle
					//We just added this table and there are no current entries, so we have to start with a PK of 1 instead of selecting MAX +1.
					command="INSERT INTO ebill (EbillNum,ClinicNum,ClientAcctNumber,ElectUserName,ElectPassword,PracticeAddress,RemitAddress) "
						+"VALUES (1,0,'"
						+POut.String(billingElectClientAcctNumber)+"','"+POut.String(billingElectUserName)+"','"+POut.String(billingElectPassword)+"',"
						+POut.Int((int)EbillAddress.PracticePhysical)+","+POut.Int((int)EbillAddress.PracticeBilling)+")";
				}
				Db.NonQ(command);
				//Add new computerpref PatSelectSearchMode
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE computerpref ADD PatSelectSearchMode tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE computerpref ADD PatSelectSearchMode number(3)";
					Db.NonQ(command);
					command="UPDATE computerpref SET PatSelectSearchMode = 0 WHERE PatSelectSearchMode IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE computerpref MODIFY PatSelectSearchMode NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD ShareOfCost double NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD ShareOfCost number(38,8)";
					Db.NonQ(command);
					command="UPDATE claim SET ShareOfCost = 0 WHERE ShareOfCost IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim MODIFY ShareOfCost NOT NULL";
					Db.NonQ(command);
				}
				//Add the X-Charge programproperty rows Username, Password, PaymentType, XWebID, AuthKey, TerminalID, PromptSignature, and PrintReceipt
				//for each clinic in the database.  Local path override will not be clinic specific, so not duplicated for each clinic.
				command="SELECT ClinicNum FROM clinic";
				List<long> listCNums=Db.GetListLong(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<listCNums.Count;i++) {
						command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
							+"(SELECT programproperty.ProgramNum,PropertyDesc,PropertyValue,ComputerName,"+listCNums[i]+" "
							+"FROM program INNER JOIN programproperty ON program.ProgramNum=programproperty.ProgramNum "
							+"WHERE program.ProgName='Xcharge' AND programproperty.ClinicNum=0 "//all existing programproperty rows for XCharge have ClinicNum=0
							+"AND PropertyDesc!='')";//Local path override will not be clinic specific, so don't duplicate for each clinic
						Db.NonQ32(command);
					}
				}
				else {//oracle
					for(int i=0;i<listCNums.Count;i++) {
						command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
							+"(SELECT (SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),programproperty.ProgramNum,PropertyDesc,PropertyValue,ComputerName,"
							+listCNums[i]+" FROM program INNER JOIN programproperty ON program.ProgramNum=programproperty.ProgramNum "
							+"WHERE program.ProgName='Xcharge' AND programproperty.ClinicNum=0 "//all existing programproperty rows for XCharge have ClinicNum=0
							+"AND PropertyDesc!='')";//Local path override will not be clinic specific, so don't duplicate for each clinic
						Db.NonQ32(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS apptcomm";
					Db.NonQ(command);
					command=@"CREATE TABLE apptcomm (
						ApptCommNum bigint NOT NULL auto_increment PRIMARY KEY,
						ApptNum bigint NOT NULL,
						ApptCommType tinyint NOT NULL,
						DateTimeSend datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						INDEX(ApptNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE apptcomm'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE apptcomm (
						ApptCommNum number(20) NOT NULL,
						ApptNum number(20) NOT NULL,
						ApptCommType number(3) NOT NULL,
						DateTimeSend date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT apptcomm_ApptCommNum PRIMARY KEY (ApptCommNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX apptcomm_ApptNum ON apptcomm (ApptNum)";
					Db.NonQ(command);
				}
				//Add ApptReminder preferences
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptReminderDayInterval','0')";
					Db.NonQ(command);
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptReminderDayMessage','0')";
					Db.NonQ(command);
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptReminderHourInterval','0')";
					Db.NonQ(command);
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptReminderHourMessage','0')";
					Db.NonQ(command);
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptReminderSendAll','0')";
					Db.NonQ(command);
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptReminderSendOrder','')";//This should be 0,1,2, fixed in version 15.4.7
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptReminderDayInterval','0')";
					Db.NonQ(command);
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptReminderDayMessage','0')";
					Db.NonQ(command);
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptReminderHourInterval','0')";
					Db.NonQ(command);
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptReminderHourMessage','0')";
					Db.NonQ(command);
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptReminderSendAll','0')";
					Db.NonQ(command);
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptReminderSendOrder','0,1,2')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedAutomaticSendSetting','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WebSchedAutomaticSendSetting','0')";
					Db.NonQ(command);
				} 
				//Add medication merge permission to groups with existing permission SecurityAdmin
				command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=24";//SecurityAdmin
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",102)";//MedicationMerge
						Db.NonQ(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",102)";//MedicationMerge
						Db.NonQ(command);
					}
				}
				//Set the to the language and region of the computer running the update.
				string languageAndRegion=CultureInfo.CurrentCulture.Name;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('LanguageAndRegion','"+POut.String(languageAndRegion)+"')";//default to blank, will be set when first connected.
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'LanguageAndRegion','"+POut.String(languageAndRegion)+"')";
					Db.NonQ(command);
				} 
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE computerpref ADD NoShowLanguage tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE computerpref ADD NoShowLanguage number(3)";
					Db.NonQ(command);
					command="UPDATE computerpref SET NoShowLanguage = 0 WHERE NoShowLanguage IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE computerpref MODIFY NoShowLanguage NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('EConnectorEnabled','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'EConnectorEnabled','0')";
					Db.NonQ(command);
				}
				//Add permission to groups with existing permission ProcComplCreate (23) ------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=23";
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
				   for(int i=0;i<table.Rows.Count;i++) {
				      groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				      command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				         +"VALUES("+POut.Long(groupNum)+",103)";//AccountProcsQuickAdd
				      Db.NonQ(command);
				   }
				}
				else {//oracle
				   for(int i=0;i<table.Rows.Count;i++) {
				      groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				      command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
				         +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",103)";//AccountProcsQuickAdd
				      Db.NonQ(command);
				   }
				}
				string valuestr=Db.GetScalar("SELECT ValueString FROM preference WHERE PrefName LIKE '"+POut.String(PrefName.ShowFeatureEhr.ToString())+"'");
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('AutomaticSummaryOfCareWebmail','"+valuestr+"')";
					Db.NonQ(command);
				}
				else {//oracle
						command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AutomaticSummaryOfCareWebmail','"+valuestr+"')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE treatplan ADD TPStatus tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE treatplan ADD TPStatus number(3)";
					Db.NonQ(command);
					command="UPDATE treatplan SET TPStatus = 0 WHERE TPStatus IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE treatplan MODIFY TPStatus NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS treatplanattach";
					Db.NonQ(command);
					command=@"CREATE TABLE treatplanattach (
						TreatPlanAttachNum bigint NOT NULL auto_increment PRIMARY KEY,
						TreatPlanNum bigint NOT NULL,
						ProcNum bigint NOT NULL,
						Priority bigint NOT NULL,
						INDEX(TreatPlanNum),
						INDEX(ProcNum),
						INDEX(Priority)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE treatplanattach'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE treatplanattach (
						TreatPlanAttachNum number(20) NOT NULL,
						TreatPlanNum number(20) NOT NULL,
						ProcNum number(20) NOT NULL,
						Priority number(20) NOT NULL,
						CONSTRAINT treatplanattach_TreatPlanAttac PRIMARY KEY (TreatPlanAttachNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX treatplanattach_TreatPlanNum ON treatplanattach (TreatPlanNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX treatplanattach_ProcNum ON treatplanattach (ProcNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX treatplanattach_Priority ON treatplanattach (Priority)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE chartview ADD IsTpCharting tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE chartview ADD IsTpCharting number(3)";
					Db.NonQ(command);
					command="UPDATE chartview SET IsTpCharting = 0 WHERE IsTpCharting IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE chartview MODIFY IsTpCharting NOT NULL";
					Db.NonQ(command);
				}
				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE screen ADD ScreenPatNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE screen ADD INDEX (ScreenPatNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE screen ADD ScreenPatNum number(20)";
					Db.NonQ(command);
					command="UPDATE screen SET ScreenPatNum = 0 WHERE ScreenPatNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE screen MODIFY ScreenPatNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX screen_ScreenPatNum ON screen (ScreenPatNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE screenpat ADD PatScreenPerm tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE screenpat ADD PatScreenPerm number(3)";
					Db.NonQ(command);
					command="UPDATE screenpat SET PatScreenPerm = 0 WHERE PatScreenPerm IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE screenpat MODIFY PatScreenPerm NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '15.4.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_4_4();
		}

		///<summary>Oracle compatible: 11/25/2015</summary>
		private static void To15_4_4() {
			if(FromVersion<new Version("15.4.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.4.4.0");//No translation in convert script.
				string command="";
				command="UPDATE clearinghouse SET Description='ITRANS' WHERE Description='CDAnet';";//oracle compatible
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '15.4.4.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_4_7();
		}

		private static void To15_4_7() {
			if(FromVersion<new Version("15.4.7.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.4.7.0");//No translation in convert script.
				string command="";
				command="UPDATE preference SET ValueString='0,1,2' WHERE Prefname='ApptReminderSendOrder'";
				Db.NonQ(command);
				command="SELECT ValueString FROM preference WHERE PrefName='ApptReminderHourMessage'";
				string result=Db.GetScalar(command);
				if(result=="0" || result=="") {//Set default text value for ApptReminderHourMessage
					string recallText="[nameFL],\r\nYou have an upcoming appointment on [apptDate] at [apptTime] with [practiceName] with [provName].\r\nThanks!";
					command="UPDATE preference SET ValueString='"+recallText+"' WHERE PrefName='ApptReminderHourMessage'";
					Db.NonQ(command);
				}
				command="SELECT ValueString FROM preference WHERE PrefName='ApptReminderDayMessage'";
				result=Db.GetScalar(command);
				if(result=="0" || result=="") {//Set default text value for ApptReminderDayMessage
					string recallText="[nameFL],\r\nYou have an upcoming appointment on [apptDate] at [apptTime] with [practiceName] with [provName].\r\nThanks!";
					command="UPDATE preference SET ValueString='"+recallText+"' WHERE PrefName='ApptReminderDayMessage'";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '15.4.7.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_4_19();
		}

		private static void To15_4_19() {
			if(FromVersion<new Version("15.4.19.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.4.19.0");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE rxpat ADD IsErxOld tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE rxpat ADD IsErxOld number(3)";
					Db.NonQ(command);
					command="UPDATE rxpat SET IsErxOld = 0 WHERE IsErxOld IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE rxpat MODIFY IsErxOld NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE rxpat SET IsErxOld=1 WHERE NewCropGuid!=''";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '15.4.19.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_4_24();
		}

		private static void To15_4_24() {
			if(FromVersion<new Version("15.4.24.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.4.24.0");//No translation in convert script.
				string command="";
				//Add referring provider information to the 1500_02_12 medical claim form.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD12' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT * FROM (SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD12') WHERE RowNum<=1";
				}
				long claimFormNum=PIn.Long(Db.GetScalar(command));
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','ReferringProvNPI','','349','587','166','14')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','ReferringProvNameFL','','69','587','226','14')";
				Db.NonQ(command);
				command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
					+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','FixedText','DN','33','587','24','14')";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '15.4.24.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_4_25();
		}

		private static void To15_4_25() {
			if(FromVersion<new Version("15.4.25.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.4.25.0");//No translation in convert script.
				string command="";
				command="UPDATE preference SET ValueString='' WHERE PrefName='EmailUsername' OR PrefName='EmailPassword'";//MySQL and Oracle compatible.
				Db.NonQ(command);
				command="UPDATE preference SET ValueString='15.4.25.0' WHERE PrefName='DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_4_26();
		}

		private static void To15_4_26() {
			if(FromVersion<new Version("15.4.26.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.4.26.0");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ReportPandIhasClinicInfo','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ReportPandIhasClinicInfo','0')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString='15.4.26.0' WHERE PrefName='DataBaseVersion'";
				Db.NonQ(command);
			}
			To15_4_29();
		}

		private static void To15_4_29() {
			if(FromVersion<new Version("15.4.29.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 15.4.29.0");
				string command = "";
				//Podium Advertising block----------------------------------------------------------------------
				command="SELECT ProgramNum from program WHERE ProgName='Podium'";
				List<long> programNums = Db.GetListLong(command);
				long programNum = 0;
				if(programNums.Count>0) {
					programNum=programNums[0];
				}
				if(DataConnection.DBtype==DatabaseType.MySql && programNum>0) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Disable Advertising', "
						+"'0')";
					Db.NonQ(command);
				}
				else if(programNum>0) {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Disable Advertising', "
						+"'0', "
						+"0)";
					Db.NonQ(command);
				}
				//DentalIntel Advertising block----------------------------------------------------------------------
				command="SELECT ProgramNum from program WHERE ProgName='DentalIntel'";
				programNums = Db.GetListLong(command);
				programNum = 0;
				if(programNums.Count>0) {
					programNum=programNums[0];
				}
				if(DataConnection.DBtype==DatabaseType.MySql && programNum>0) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Disable Advertising', "
						+"'0')";
					Db.NonQ(command);
				}
				else if(programNum>0) {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Disable Advertising', "
						+"'0', "
						+"0)";
					Db.NonQ(command);
				}
				//DentalTekSmartOfficePhone Advertising block----------------------------------------------------------------------
				command="SELECT ProgramNum from program WHERE ProgName='DentalTekSmartOfficePhone'";
				programNums = Db.GetListLong(command);
				programNum = 0;
				if(programNums.Count>0) {
					programNum=programNums[0];
				}
				if(DataConnection.DBtype==DatabaseType.MySql && programNum>0) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Disable Advertising', "
						+"'0')";
					Db.NonQ(command);
				}
				else if(programNum>0) {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Disable Advertising', "
						+"'0', "
						+"0)";
					Db.NonQ(command);
				}
				//CentralDataStorage Advertising block----------------------------------------------------------------------
				//Program link added to track the Disable Advertising property.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'CentralDataStorage', "
						+"'Cental Data Storage from centraldatastorage.com', "
						+"'0', "
						+"'',"
						+"'', "
						+"'')";
					programNum = Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Disable Advertising', "
						+"'0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'CentralDataStorage', "
						+"'Cental Data Storage from centraldatastorage.com', "
						+"'0', "
						+"'',"
						+"'', "
						+"'')";
					programNum = Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Disable Advertising', "
						+"'0', "
						+"0)";
					Db.NonQ(command);
				}//end CentralDataStorage bridge
				command="UPDATE preference SET ValueString = '15.4.29.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_1_1();
		}

		private static void To16_1_1() {
			if(FromVersion<new Version("16.1.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1");//No translation in convert script.
				string command="";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE emailattach ADD EmailTemplateNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE emailattach ADD INDEX (EmailTemplateNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE emailattach ADD EmailTemplateNum number(20)";
					Db.NonQ(command);
					command="UPDATE emailattach SET EmailTemplateNum = 0 WHERE EmailTemplateNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE emailattach MODIFY EmailTemplateNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX emailattach_EmailTemplateNum ON emailattach (EmailTemplateNum)";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - emailaddresses");//No translation in convert script.
				command="SELECT emailaddress.EmailAddressNum, emailaddress.EmailPassword FROM emailaddress";
				DataTable table=Db.GetTable(command);
				for(int i=0;i < table.Rows.Count;i++) {
					long emailAddressNum=PIn.Long(table.Rows[i][0].ToString());
					string emailPassEncrypted=POut.String(Encrypt(table.Rows[i][1].ToString()));
					command="UPDATE emailaddress SET EmailPassword = '"+emailPassEncrypted
						+"' WHERE EmailAddressNum = "+emailAddressNum;
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE emailaddress ADD UserNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE emailaddress ADD INDEX (UserNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE emailaddress ADD UserNum number(20)";
					Db.NonQ(command);
					command="UPDATE emailaddress SET UserNum = 0 WHERE UserNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE emailaddress MODIFY UserNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX emailaddress_UserNum ON emailaddress (UserNum)";
					Db.NonQ(command);
				}
				//Appointment time line color.  Default to old color of Color.Red = ARGB #FFFF0000.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('AppointmentTimeLineColor','-65536')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AppointmentTimeLineColor','-65536')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE vitalsign ADD Pulse int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE vitalsign ADD Pulse number(11)";
					Db.NonQ(command);
					command="UPDATE vitalsign SET Pulse = 0 WHERE Pulse IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE vitalsign MODIFY Pulse NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE apptview ADD IsScrollStartDynamic tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE apptview ADD IsScrollStartDynamic number(3)";
					Db.NonQ(command);
					command="UPDATE apptview SET IsScrollStartDynamic = 0 WHERE IsScrollStartDynamic IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE apptview MODIFY IsScrollStartDynamic NOT NULL";
					Db.NonQ(command);
				}
				//Perio chart skip missing teeth by default preference
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PerioSkipMissingTeeth','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PerioSkipMissingTeeth','1')";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - CDT 2016");//No translation in convert script.
				//Moving codes to the Obsolete category that were deleted in CDT 2016.
				if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
					//Move deprecated codes to the Obsolete procedure code category.
					//Make sure the procedure code category exists before moving the procedure codes.
					string procCatDescript="Obsolete";
					long defNum=0;
					command="SELECT DefNum FROM definition WHERE Category=11 AND ItemName='"+POut.String(procCatDescript)+"'";//11 is DefCat.ProcCodeCats
					DataTable dtDef=Db.GetTable(command);
					if(dtDef.Rows.Count==0) { //The procedure code category does not exist, add it
						command="SELECT COUNT(*) FROM definition WHERE Category=11";//11 is DefCat.ProcCodeCats
						int countCats=PIn.Int(Db.GetCount(command));
						if(DataConnection.DBtype==DatabaseType.MySql) {
							command="INSERT INTO definition (Category,ItemName,ItemOrder) "
									+"VALUES (11"+",'"+POut.String(procCatDescript)+"',"+POut.Int(countCats)+")";//11 is DefCat.ProcCodeCats
						}
						else {//oracle
							command="INSERT INTO definition (DefNum,Category,ItemName,ItemOrder) "
									+"VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),11,'"+POut.String(procCatDescript)+"',"+POut.Int(countCats)+")";//11 is DefCat.ProcCodeCats
						}
						defNum=Db.NonQ(command,true,"DefNum","definition");
					}
					else { //The procedure code category already exists, get the existing defnum
						defNum=PIn.Long(dtDef.Rows[0]["DefNum"].ToString());
					}
					string[] cdtCodesDeleted=new string[] {
						"D0260","D0421","D2970","D9220","D9221","D9241","D9242","D9931"
					};
					for(int i=0;i<cdtCodesDeleted.Length;i++) {
						string procCode=cdtCodesDeleted[i];
						command="SELECT CodeNum FROM procedurecode WHERE ProcCode='"+POut.String(procCode)+"'";
						DataTable dtProcCode=Db.GetTable(command);
						if(dtProcCode.Rows.Count==0) { //The procedure code does not exist in this database.
							continue;//Do not try to move it.
						}
						long codeNum=PIn.Long(dtProcCode.Rows[0]["CodeNum"].ToString());
						command="UPDATE procedurecode SET ProcCat="+POut.Long(defNum)+" WHERE CodeNum="+POut.Long(codeNum);
						Db.NonQ(command);
					}
				}//end United States CDT codes update
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS insverify";
					Db.NonQ(command);
					command=@"CREATE TABLE insverify (
						InsVerifyNum bigint NOT NULL auto_increment PRIMARY KEY,
						DateLastVerified date NOT NULL DEFAULT '0001-01-01',
						UserNum bigint NOT NULL,
						VerifyType tinyint NOT NULL,
						FKey bigint NOT NULL,
						DefNum bigint NOT NULL,
						Note text NOT NULL,
						INDEX(UserNum),
						INDEX(FKey),
						INDEX(DefNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE insverify'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE insverify (
						InsVerifyNum number(20) NOT NULL,
						DateLastVerified date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						UserNum number(20) NOT NULL,
						VerifyType number(3) NOT NULL,
						FKey number(20) NOT NULL,
						DefNum number(20) NOT NULL,
						Note clob,
						CONSTRAINT insverify_InsVerifyNum PRIMARY KEY (InsVerifyNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX insverify_UserNum ON insverify (UserNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX insverify_FKey ON insverify (FKey)";
					Db.NonQ(command);
					command=@"CREATE INDEX insverify_DefNum ON insverify (DefNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS insverifyhist";
					Db.NonQ(command);
					command=@"CREATE TABLE insverifyhist (
						InsVerifyHistNum bigint NOT NULL auto_increment PRIMARY KEY,
						InsVerifyNum bigint NOT NULL,
						DateLastVerified date NOT NULL DEFAULT '0001-01-01',
						UserNum bigint NOT NULL,
						VerifyType tinyint NOT NULL,
						FKey bigint NOT NULL,
						DefNum bigint NOT NULL,
						Note text NOT NULL,
						INDEX(InsVerifyNum),
						INDEX(UserNum),
						INDEX(FKey),
						INDEX(DefNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE insverifyhist'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE insverifyhist (
						InsVerifyHistNum number(20) NOT NULL,
						InsVerifyNum number(20) NOT NULL,
						DateLastVerified date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						UserNum number(20) NOT NULL,
						VerifyType number(3) NOT NULL,
						FKey number(20) NOT NULL,
						DefNum number(20) NOT NULL,
						Note clob,
						CONSTRAINT insverifyhist_InsVerifyHistNum PRIMARY KEY (InsVerifyHistNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX insverifyhist_InsVerifyNum ON insverifyhist (InsVerifyNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX insverifyhist_UserNum ON insverifyhist (UserNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX insverifyhist_FKey ON insverifyhist (FKey)";
					Db.NonQ(command);
					command=@"CREATE INDEX insverifyhist_DefNum ON insverifyhist (DefNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD Region bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE clinic ADD INDEX (Region)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD Region number(20)";
					Db.NonQ(command);
					command="UPDATE clinic SET Region = 0 WHERE Region IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clinic MODIFY Region NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX clinic_Region ON clinic (Region)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('InsVerifyBenefitEligibilityDays','90')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT MAX(PrefNum)+1 FROM preference),'InsVerifyBenefitEligibilityDays','90')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('InsVerifyPatientEnrollmentDays','30')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT MAX(PrefNum)+1 FROM preference),'InsVerifyPatientEnrollmentDays','30')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('InsVerifyAppointmentScheduledDays','7')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT MAX(PrefNum)+1 FROM preference),'InsVerifyAppointmentScheduledDays','7')";
					Db.NonQ(command);
				}
				long unverifiedDefNum=0;
				//Add 2 new definitions for the Insurance Verification Status definition category.
				if(DataConnection.DBtype==DatabaseType.MySql) { //38 is DefCat.InsuranceVerificationStatus
					command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemValue) VALUES (38,'Unverified',0,'')";
					unverifiedDefNum=Db.NonQ(command,true);
					command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemValue) VALUES (38,'Verified',1,'')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO definition (DefNum,Category,ItemName,ItemOrder,ItemValue) "
						+"VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),38,'Unverified',0,'')";
					unverifiedDefNum=Db.NonQ(command,true,"DefNum","definition");
					command="INSERT INTO definition (DefNum,Category,ItemName,ItemOrder,ItemValue) "
						+"VALUES ((SELECT MAX(DefNum)+1 FROM definition),38,'Verified',1,'')";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - insverify / insplans");//No translation in convert script.
				string insBenefitEnumValue="1";//VerifyTypes.InsuranceBenefit
				string patientEnrollmentEnumValue="2";//VerifyTypes.PatientEnrollment
				command="SELECT * FROM insplan";
				DataTable dtInsPlans=Db.GetTable(command);
				for(int i=0;i<dtInsPlans.Rows.Count;i++) {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO insverify (DateLastVerified,UserNum,VerifyType,FKey,DefNum) "
							+"VALUES('0001-01-01',0,"+insBenefitEnumValue+","+dtInsPlans.Rows[i]["PlanNum"]+","+unverifiedDefNum.ToString()+")";
						Db.NonQ(command);
					}
					else {//oracle
						command="INSERT INTO insverify (InsVerifyNum,DateLastVerified,UserNum,VerifyType,FKey,DefNum) "
							+"VALUES((SELECT COALESCE(MAX(InsVerifyNum),0)+1 FROM insverify),"
							+"TO_DATE('0001-01-01','YYYY-MM-DD'),0,"+insBenefitEnumValue+","+dtInsPlans.Rows[i]["PlanNum"]+","+unverifiedDefNum.ToString()+")";
						Db.NonQ(command);
					}
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - insverify / patplans");//No translation in convert script.
				command="SELECT * FROM patplan";
				DataTable dtPatPlans=Db.GetTable(command);
				for(int i=0;i<dtPatPlans.Rows.Count;i++) {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO insverify (DateLastVerified,UserNum,VerifyType,FKey,DefNum) "
							+"VALUES('0001-01-01',0,"+patientEnrollmentEnumValue+","+dtPatPlans.Rows[i]["PatPlanNum"]+","+unverifiedDefNum.ToString()+")";
						Db.NonQ(command);
					}
					else {//oracle
						command="INSERT INTO insverify (InsVerifyNum,DateLastVerified,UserNum,VerifyType,FKey,DefNum) "
							+"VALUES((SELECT COALESCE(MAX(InsVerifyNum),0)+1 FROM insverify),"
							+"TO_DATE('0001-01-01','YYYY-MM-DD'),0,"+patientEnrollmentEnumValue+","+dtPatPlans.Rows[i]["PatPlanNum"]+","+unverifiedDefNum.ToString()+")";
						Db.NonQ(command);
					}
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - perio");//No translation in convert script.
				//Perio exams treat implants as not missing
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PerioTreatImplantsAsNotMissing','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PerioTreatImplantsAsNotMissing','1')";
					Db.NonQ(command);
				}
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				table=Db.GetTable(command);
				long groupNum;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							 +"VALUES("+POut.Long(groupNum)+",104)";  //ClaimSend
						Db.NonQ(command);
					}
				}
				else {//oracle
					for(int i=0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",104)";  //ClaimSend
						Db.NonQ(command);
					}
				}
				//Insert CleaRay bridge-----------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					    +") VALUES("
					    +"'CleaRay', "
					    +"'CleaRay from www.clearaydental.com', "
					    +"'0', "
					    +"'"+POut.String(@"C:\Program Files\CLEARAY CR200\CR200 LAB\CR200 LAB-IS.exe")+"', "
					    +"'', "//leave blank if none
					    +"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					    +") VALUES("
					    +"'"+POut.Long(programNum)+"', "
					    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					    +"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					    +"VALUES ("
					    +"'"+POut.Long(programNum)+"', "
					    +"'2', "//ToolBarsAvail.ChartModule
					    +"'CleaRay')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				        +") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
								+"'CleaRay', "
				        +"'CleaRay from www.clearaydental.com', "
				        +"'0', "
				        +"'"+POut.String(@"C:\Program Files\CLEARAY CR200\CR200 LAB\CR200 LAB-IS.exe")+"', "
				        +"'', "//leave blank if none
				        +"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					    +") VALUES("
					    +"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
					    +"'"+POut.Long(programNum)+"', "
					    +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					    +"'0', "
					    +"0)";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
					    +"VALUES ("
					    +"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
					    +"'"+POut.Long(programNum)+"', "
					    +"'2', "//ToolBarsAvail.ChartModule
					    +"'CleaRay')";
					Db.NonQ(command);
				}//end CleaRay bridge				
				//Broken Appointment Procedure
				command="SELECT COUNT(CodeNum) FROM procedurecode WHERE procedurecode.ProcCode='D9986'";
				string countBrokenApptProcCode=Db.GetCount(command);
				bool hasBrokenApptProcCode=false;
				if(PIn.Int(countBrokenApptProcCode)>0) {
					hasBrokenApptProcCode=true;
				}
				//Several offices have upgraded past this version and were actively using the D9986 code (e.g. D9986 for one office was "whitening").
				//Therefore, we need to check the database first to see if the code is already in use.  
				//If the code is currently in use and does not match the correct description dictated by the ADA, we need to move that procedure to a "new"
				// procedure (and all other tables that are linked to the FK of said procedure) so that the D code tool can insert the "correct" D9986.
				if(hasBrokenApptProcCode) {
					command="SELECT Descript FROM procedurecode WHERE ProcCode='D9986'";
					string descript=Db.GetScalar(command);
					if(!descript.ToLower().Contains("missed appointment")) {//They have D9986 that isn't "missed appointment".  Move the code to a new entry.
						command="SELECT * FROM procedurecode WHERE procedurecode.ProcCode='D9986'";
						DataTable procInfo=Db.GetTable(command);
						long priKeyNew=0;
						if(DataConnection.DBtype==DatabaseType.Oracle) { 
							command="SELECT COALESCE(MAX(CodeNum),0)+1 FROM procedurecode";
							priKeyNew=PIn.Long(Db.GetScalar(command));
							command="INSERT INTO procedurecode VALUES("+POut.Long(priKeyNew)+",";//PK
						}
						else {
							command="INSERT INTO procedurecode (ProcCode,Descript,AbbrDesc,ProcTime,ProcCat,TreatArea,NoBillIns,IsProsth,DefaultNote,IsHygiene,GTypeNum,"
								+"AlternateCode1,MedicalCode,IsTaxed,PaintType,GraphicColor,LaymanTerm,IsCanadianLab,PreExisting,BaseUnits,SubstitutionCode,SubstOnlyIf,"
								+"DateTStamp,IsMultiVisit,DrugNDC,RevenueCodeDefault,ProvNumDefault,CanadaTimeUnits,IsRadiology) VALUES(";
						}						
						//If this duplicates a proc code, the user will call us and we will manually fix.
						command+="'"+POut.String(procInfo.Rows[0]["ProcCode"].ToString())+"T',"
							+"'"+POut.String(procInfo.Rows[0]["Descript"].ToString())+"',"
							+"'"+POut.String(procInfo.Rows[0]["AbbrDesc"].ToString())+"',"
							+"'"+POut.String(procInfo.Rows[0]["ProcTime"].ToString())+"',"
							+PIn.Long(procInfo.Rows[0]["ProcCat"].ToString()).ToString()+","
							+PIn.Long(procInfo.Rows[0]["TreatArea"].ToString()).ToString()+","
							+PIn.Long(procInfo.Rows[0]["NoBillIns"].ToString()).ToString()+","
							+PIn.Long(procInfo.Rows[0]["IsProsth"].ToString()).ToString()+","
							+"'"+POut.String(procInfo.Rows[0]["DefaultNote"].ToString())+"',"
							+PIn.Long(procInfo.Rows[0]["IsHygiene"].ToString()).ToString()+","
							+PIn.Long(procInfo.Rows[0]["GTypeNum"].ToString()).ToString()+","
							+"'"+POut.String(procInfo.Rows[0]["AlternateCode1"].ToString())+"',"
							+"'"+POut.String(procInfo.Rows[0]["MedicalCode"].ToString())+"',"
							+PIn.Long(procInfo.Rows[0]["IsTaxed"].ToString()).ToString()+","
							+PIn.Long(procInfo.Rows[0]["PaintType"].ToString()).ToString()+","
							+PIn.Long(procInfo.Rows[0]["GraphicColor"].ToString()).ToString()+","
							+"'"+POut.String(procInfo.Rows[0]["LaymanTerm"].ToString())+"',"
							+PIn.Long(procInfo.Rows[0]["IsCanadianLab"].ToString()).ToString()+","
							+PIn.Long(procInfo.Rows[0]["PreExisting"].ToString()).ToString()+","
							+PIn.Long(procInfo.Rows[0]["BaseUnits"].ToString()).ToString()+","
							+"'"+POut.String(procInfo.Rows[0]["SubstitutionCode"].ToString())+"',"
							+PIn.Long(procInfo.Rows[0]["SubstOnlyIf"].ToString()).ToString()+","
							+POut.DateT(DateTime.Now)+","
							+PIn.Long(procInfo.Rows[0]["IsMultiVisit"].ToString()).ToString()+","
							+"'"+POut.String(procInfo.Rows[0]["DrugNDC"].ToString())+"',"
							+"'"+POut.String(procInfo.Rows[0]["RevenueCodeDefault"].ToString())+"',"
							+PIn.Long(procInfo.Rows[0]["ProvNumDefault"].ToString()).ToString()+","
							+PIn.Double(procInfo.Rows[0]["CanadaTimeUnits"].ToString()).ToString()+","
							+PIn.Long(procInfo.Rows[0]["IsRadiology"].ToString()).ToString()+")";
						if(DataConnection.DBtype==DatabaseType.MySql) {
							priKeyNew=Db.NonQ(command,true);
						}
						else {//Oracle
							Db.NonQ(command);
						}
						long priKeyOld=PIn.Long(procInfo.Rows[0]["CodeNum"].ToString());
						//FIX FKEY ASSOCIATIONS
						//Autocodeitem
						command="UPDATE autocodeitem SET CodeNum="+POut.Long(priKeyNew)+" WHERE CodeNum="+POut.Long(priKeyOld);
						Db.NonQ(command);
						//Benefit
						command="UPDATE benefit SET CodeNum="+POut.Long(priKeyNew)+" WHERE CodeNum="+POut.Long(priKeyOld);
						Db.NonQ(command);
						//Fee
						command="UPDATE fee SET CodeNum="+POut.Long(priKeyNew)+" WHERE CodeNum="+POut.Long(priKeyOld);
						Db.NonQ(command);
						//Procbuttonitem
						command="UPDATE procbuttonitem SET CodeNum="+POut.Long(priKeyNew)+" WHERE CodeNum="+POut.Long(priKeyOld);
						Db.NonQ(command);
						//Procbuttonquick
						command="UPDATE procbuttonquick SET CodeValue="+POut.Long(priKeyNew)+" WHERE CodeValue="+POut.Long(priKeyOld);
						Db.NonQ(command);
						//Proccodenote
						command="UPDATE proccodenote SET CodeNum="+POut.Long(priKeyNew)+" WHERE CodeNum="+POut.Long(priKeyOld);
						Db.NonQ(command);
						//Procedurecode
						command="UPDATE procedurecode SET MedicalCode="+POut.Long(priKeyNew)+" WHERE MedicalCode="+POut.Long(priKeyOld);
						Db.NonQ(command);
						command="UPDATE procedurecode SET SubstitutionCode="+POut.Long(priKeyNew)+" WHERE SubstitutionCode="+POut.Long(priKeyOld);
						Db.NonQ(command);
						//Procedurelog
						command="UPDATE procedurelog SET MedicalCode="+POut.Long(priKeyNew)+" WHERE MedicalCode="+POut.Long(priKeyOld);
						Db.NonQ(command);
						command="UPDATE procedurelog SET CodeNum="+POut.Long(priKeyNew)+" WHERE CodeNum="+POut.Long(priKeyOld);
						Db.NonQ(command);
						//Recalltrigger
						command="UPDATE recalltrigger SET CodeNum="+POut.Long(priKeyNew)+" WHERE CodeNum="+POut.Long(priKeyOld);
						Db.NonQ(command);
						//Repeatcharge
						command="UPDATE repeatcharge SET ProcCode="+POut.Long(priKeyNew)+" WHERE ProcCode="+POut.Long(priKeyOld);
						Db.NonQ(command);
						//Toothgridcol
						command="UPDATE toothgridcol SET CodeNum="+POut.Long(priKeyNew)+" WHERE CodeNum="+POut.Long(priKeyOld);
						Db.NonQ(command);
						//Toothgriddef
						command="UPDATE toothgriddef SET CodeNum="+POut.Long(priKeyNew)+" WHERE CodeNum="+POut.Long(priKeyOld);
						Db.NonQ(command);
						//We need to set the hasBrokenApptProcCode to false so that the preference to utilize this code is OFF by default.
						hasBrokenApptProcCode=false;
						//Now that all references have been fixed, we need to REMOVE the incorrect D9986 code from the database so that it cannot be used again.
						command="DELETE FROM procedurecode WHERE procedurecode.ProcCode='D9986'";
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('BrokenApptProcedure','"+POut.Bool(hasBrokenApptProcCode)+"')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT MAX(PrefNum)+1 FROM preference),'BrokenApptProcedure','"+POut.Bool(hasBrokenApptProcCode)+"')";
					Db.NonQ(command);
				}
				//Rename Broken Appointment Adjustment pref
				command="UPDATE preference SET PrefName='BrokenApptAdjustment' WHERE PrefName='BrokenApptAdjustmentWithProcedure'";
				Db.NonQ(command);
				//Rename Broken Appointment Commlog Pref
				command="UPDATE preference SET PrefName='BrokenApptCommLog' WHERE PrefName='BrokenApptCommLogWithProcedure'";
				Db.NonQ(command);
				//Wrap columns when printing
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES ('ReportsWrapColumns','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'ReportsWrapColumns','0')";
					Db.NonQ(command);
				}
				//Screenings use sheets preference.  Defaults to being off so that this new functionality is opt in.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ScreeningsUseSheets','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ScreeningsUseSheets','0')";
					Db.NonQ(command);
				}
				//Change the appointment calendar to today's date when changing clinics
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('AppointmentClinicTimeReset','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AppointmentClinicTimeReset','0')";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - screenings");//No translation in convert script.
				//Screen Groups need the ability to be set up ahead of time prior to the actual screenings.
				//Therefore we need to move the information regarding the screen group from the screen table to the screengroup table,
				//since we used to save location and screener information on the screen instead of the screen group.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE screengroup ADD ProvName varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE screengroup ADD ProvName varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE screengroup ADD ProvNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE screengroup ADD INDEX (ProvNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE screengroup ADD ProvNum number(20)";
					Db.NonQ(command);
					command="UPDATE screengroup SET ProvNum = 0 WHERE ProvNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE screengroup MODIFY ProvNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX screengroup_ProvNum ON screengroup (ProvNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE screengroup ADD PlaceService tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE screengroup ADD PlaceService number(3)";
					Db.NonQ(command);
					command="UPDATE screengroup SET PlaceService = 0 WHERE PlaceService IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE screengroup MODIFY PlaceService NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE screengroup ADD County varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE screengroup ADD County varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE screengroup ADD GradeSchool varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE screengroup ADD GradeSchool varchar2(255)";
					Db.NonQ(command);
				}
				//Now that the screengroup table has columns for the related information we need to update any current screen groups via their screens.
				command=@"SELECT ScreenGroupNum,ProvName,ProvNum,PlaceService,County,GradeSchool 
					FROM screen
					GROUP BY ScreenGroupNum";//We want any random screen, since the screen for a screen group should have already been synced.
				if(DataConnection.DBtype!=DatabaseType.MySql) {
					command+=",ProvName,ProvNum,PlaceService,County,GradeSchool";
				}
				table=Db.GetTable(command);
				foreach(DataRow row in table.Rows) {
					command="UPDATE screengroup SET "
							+"ProvName='"			+row["ProvName"].ToString()+"'"
							+",ProvNum="			+row["ProvNum"].ToString()
							+",PlaceService="	+row["PlaceService"].ToString()
							+",County='"			+row["County"].ToString()+"'"
							+",GradeSchool='"	+row["GradeSchool"].ToString()+"' "
						+"WHERE ScreenGroupNum="+row["ScreenGroupNum"].ToString();
					Db.NonQ(command);
				}
				//Drop the duplicate columns from the screen table that are now stored in the screengroup table.
				command="ALTER TABLE screen DROP COLUMN ScreenDate";//This column already existed in the screengroup table.  Called SGDate.
				Db.NonQ(command);
				command="ALTER TABLE screen DROP COLUMN ProvName";
				Db.NonQ(command);
				command="ALTER TABLE screen DROP COLUMN ProvNum";
				Db.NonQ(command);
				command="ALTER TABLE screen DROP COLUMN PlaceService";
				Db.NonQ(command);
				command="ALTER TABLE screen DROP COLUMN County";
				Db.NonQ(command);
				command="ALTER TABLE screen DROP COLUMN GradeSchool";
				Db.NonQ(command);
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - proctp add abbr column");//No translation in convert script.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE proctp ADD ProcAbbr varchar(50) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE proctp ADD ProcAbbr varchar2(50)";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - proctp update abbr column");//No translation in convert script.
				//Update the new ProcAbbr column on the proctp column to match the AbbrDesc of the corresponding procedurecode.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="UPDATE proctp,procedurecode SET proctp.ProcAbbr=procedurecode.AbbrDesc WHERE proctp.ProcCode=procedurecode.ProcCode";
					Db.NonQ(command);
				}
				else {//Oracle
					//Oracle can't do the fast thing... so do the slow thing...
					//The following loops were taking FAR too long in MySQL.  E.g. db with ~1K procedure codes and ~6K proctps was taking over 5 minutes.
					//No one uses Oracle so who cares how long it takes.
					command="SELECT ProcCode,AbbrDesc FROM procedurecode GROUP BY ProcCode,AbbrDesc";
					DataTable dataTableProcCodes=Db.GetTable(command);
					foreach(DataRow row in dataTableProcCodes.Rows) {
						command="UPDATE proctp SET ProcAbbr='"+POut.String(row["AbbrDesc"].ToString())+"' "
							+"WHERE ProcCode='"+POut.String(row["ProcCode"].ToString())+"'";
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE screen ADD SheetNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE screen ADD INDEX (SheetNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE screen ADD SheetNum number(20)";
					Db.NonQ(command);
					command="UPDATE screen SET SheetNum = 0 WHERE SheetNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE screen MODIFY SheetNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX screen_SheetNum ON screen (SheetNum)";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - signalod");//No translation in convert script.
				//Form Level Signal Processing Phase I
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE signalod ADD FKey bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE signalod ADD INDEX (FKey)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE signalod ADD FKey number(20)";
					Db.NonQ(command);
					command="UPDATE signalod SET FKey = 0 WHERE FKey IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE signalod MODIFY FKey NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX signalod_FKey ON signalod (FKey)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE signalod ADD FKeyType varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE signalod ADD FKeyType varchar2(255)";
					Db.NonQ(command);
				}
				#region Security Timestamp Tables (grouped by table instead of by column)
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - adjustment security");//No translation in convert script.
				//adjustment table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE adjustment ADD SecUserNumEntry bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE adjustment ADD INDEX (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE adjustment ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE adjustment SET SecDateTEdit = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE adjustment ADD SecUserNumEntry number(20)";
					Db.NonQ(command);
					command="UPDATE adjustment SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE adjustment MODIFY SecUserNumEntry NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX adjustment_SecUserNumEntry ON adjustment (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE adjustment ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE adjustment SET SecDateTEdit = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - appointment security");//No translation in convert script.
				//appointment table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE appointment ADD SecUserNumEntry bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE appointment ADD INDEX (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE appointment ADD SecDateEntry date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE appointment ADD SecUserNumEntry number(20)";
					Db.NonQ(command);
					command="UPDATE appointment SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE appointment MODIFY SecUserNumEntry NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX appointment_SecUserNumEntry ON appointment (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE appointment ADD SecDateEntry date";
					Db.NonQ(command);
					command="UPDATE appointment SET SecDateEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE SecDateEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE appointment MODIFY SecDateEntry NOT NULL";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - carrier security");//No translation in convert script.
				//carrier table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE carrier ADD SecUserNumEntry bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE carrier ADD INDEX (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE carrier ADD SecDateEntry date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
					command="ALTER TABLE carrier ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE carrier SET SecDateTEdit = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE carrier ADD SecUserNumEntry number(20)";
					Db.NonQ(command);
					command="UPDATE carrier SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE carrier MODIFY SecUserNumEntry NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX carrier_SecUserNumEntry ON carrier (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE carrier ADD SecDateEntry date";
					Db.NonQ(command);
					command="UPDATE carrier SET SecDateEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE SecDateEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE carrier MODIFY SecDateEntry NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE carrier ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE carrier SET SecDateTEdit = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - claim security");//No translation in convert script.
				//claim table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD SecUserNumEntry bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim ADD INDEX (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE claim ADD SecDateEntry date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
					command="ALTER TABLE claim ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE claim SET SecDateTEdit = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD SecUserNumEntry number(20)";
					Db.NonQ(command);
					command="UPDATE claim SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim MODIFY SecUserNumEntry NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX claim_SecUserNumEntry ON claim (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE claim ADD SecDateEntry date";
					Db.NonQ(command);
					command="UPDATE claim SET SecDateEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE SecDateEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim MODIFY SecDateEntry NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE claim SET SecDateTEdit = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - claimpayment security");//No translation in convert script.
				//claimpayment table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claimpayment ADD SecUserNumEntry bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimpayment ADD INDEX (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE claimpayment ADD SecDateEntry date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
					command="ALTER TABLE claimpayment ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE claimpayment SET SecDateTEdit = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claimpayment ADD SecUserNumEntry number(20)";
					Db.NonQ(command);
					command="UPDATE claimpayment SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimpayment MODIFY SecUserNumEntry NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX claimpayment_SecUserNumEntry ON claimpayment (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE claimpayment ADD SecDateEntry date";
					Db.NonQ(command);
					command="UPDATE claimpayment SET SecDateEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE SecDateEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimpayment MODIFY SecDateEntry NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimpayment ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE claimpayment SET SecDateTEdit = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - claimproc security");//No translation in convert script.
				//claimproc table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claimproc ADD SecUserNumEntry bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimproc ADD INDEX (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE claimproc ADD SecDateEntry date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
					command="ALTER TABLE claimproc ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE claimproc SET SecDateTEdit = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claimproc ADD SecUserNumEntry number(20)";
					Db.NonQ(command);
					command="UPDATE claimproc SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimproc MODIFY SecUserNumEntry NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX claimproc_SecUserNumEntry ON claimproc (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE claimproc ADD SecDateEntry date";
					Db.NonQ(command);
					command="UPDATE claimproc SET SecDateEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE SecDateEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimproc MODIFY SecDateEntry NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimproc ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE claimproc SET SecDateTEdit = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - fee security");//No translation in convert script.
				//fee table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE fee ADD SecUserNumEntry bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE fee ADD INDEX (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE fee ADD SecDateEntry date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
					command="ALTER TABLE fee ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE fee SET SecDateTEdit = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE fee ADD SecUserNumEntry number(20)";
					Db.NonQ(command);
					command="UPDATE fee SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE fee MODIFY SecUserNumEntry NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX fee_SecUserNumEntry ON fee (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE fee ADD SecDateEntry date";
					Db.NonQ(command);
					command="UPDATE fee SET SecDateEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE SecDateEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE fee MODIFY SecDateEntry NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE fee ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE fee SET SecDateTEdit = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - feesched security");//No translation in convert script.
				//feesched table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE feesched ADD SecUserNumEntry bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE feesched ADD INDEX (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE feesched ADD SecDateEntry date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
					command="ALTER TABLE feesched ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE feesched SET SecDateTEdit = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE feesched ADD SecUserNumEntry number(20)";
					Db.NonQ(command);
					command="UPDATE feesched SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE feesched MODIFY SecUserNumEntry NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX feesched_SecUserNumEntry ON feesched (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE feesched ADD SecDateEntry date";
					Db.NonQ(command);
					command="UPDATE feesched SET SecDateEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE SecDateEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE feesched MODIFY SecDateEntry NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE feesched ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE feesched SET SecDateTEdit = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - insplan security");//No translation in convert script.
				//insplan table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE insplan ADD SecUserNumEntry bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE insplan ADD INDEX (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE insplan ADD SecDateEntry date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
					command="ALTER TABLE insplan ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE insplan SET SecDateTEdit = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE insplan ADD SecUserNumEntry number(20)";
					Db.NonQ(command);
					command="UPDATE insplan SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE insplan MODIFY SecUserNumEntry NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX insplan_SecUserNumEntry ON insplan (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE insplan ADD SecDateEntry date";
					Db.NonQ(command);
					command="UPDATE insplan SET SecDateEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE SecDateEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE insplan MODIFY SecDateEntry NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE insplan ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE insplan SET SecDateTEdit = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - inssub security");//No translation in convert script.
				//inssub table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE inssub ADD SecUserNumEntry bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE inssub ADD INDEX (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE inssub ADD SecDateEntry date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
					command="ALTER TABLE inssub ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE inssub SET SecDateTEdit = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE inssub ADD SecUserNumEntry number(20)";
					Db.NonQ(command);
					command="UPDATE inssub SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE inssub MODIFY SecUserNumEntry NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX inssub_SecUserNumEntry ON inssub (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE inssub ADD SecDateEntry date";
					Db.NonQ(command);
					command="UPDATE inssub SET SecDateEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE SecDateEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE inssub MODIFY SecDateEntry NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE inssub ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE inssub SET SecDateTEdit = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - patfield security");//No translation in convert script.
				//patfield table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patfield ADD SecUserNumEntry bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE patfield ADD INDEX (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE patfield ADD SecDateEntry date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
					command="ALTER TABLE patfield ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE patfield SET SecDateTEdit = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patfield ADD SecUserNumEntry number(20)";
					Db.NonQ(command);
					command="UPDATE patfield SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patfield MODIFY SecUserNumEntry NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX patfield_SecUserNumEntry ON patfield (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE patfield ADD SecDateEntry date";
					Db.NonQ(command);
					command="UPDATE patfield SET SecDateEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE SecDateEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patfield MODIFY SecDateEntry NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE patfield ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE patfield SET SecDateTEdit = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - patient security");//No translation in convert script.
				//patient table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patient ADD SecUserNumEntry bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE patient ADD INDEX (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE patient ADD SecDateEntry date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patient ADD SecUserNumEntry number(20)";
					Db.NonQ(command);
					command="UPDATE patient SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patient MODIFY SecUserNumEntry NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX patient_SecUserNumEntry ON patient (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE patient ADD SecDateEntry date";
					Db.NonQ(command);
					command="UPDATE patient SET SecDateEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE SecDateEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patient MODIFY SecDateEntry NOT NULL";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - payment security");//No translation in convert script.
				//payment table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE payment ADD SecUserNumEntry bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE payment ADD INDEX (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE payment ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE payment SET SecDateTEdit = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE payment ADD SecUserNumEntry number(20)";
					Db.NonQ(command);
					command="UPDATE payment SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE payment MODIFY SecUserNumEntry NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX payment_SecUserNumEntry ON payment (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE payment ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE payment SET SecDateTEdit = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - paysplit security");//No translation in convert script.
				//paysplit table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE paysplit ADD SecUserNumEntry bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE paysplit ADD INDEX (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE paysplit ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE paysplit SET SecDateTEdit = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE paysplit ADD SecUserNumEntry number(20)";
					Db.NonQ(command);
					command="UPDATE paysplit SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE paysplit MODIFY SecUserNumEntry NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX paysplit_SecUserNumEntry ON paysplit (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE paysplit ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE paysplit SET SecDateTEdit = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - procedurelog security");//No translation in convert script.
				//procedurelog table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD SecUserNumEntry bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog ADD INDEX (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog ADD SecDateEntry date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD SecUserNumEntry number(20)";
					Db.NonQ(command);
					command="UPDATE procedurelog SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog MODIFY SecUserNumEntry NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX procedurelog_SecUserNumEntry ON procedurelog (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog ADD SecDateEntry date";
					Db.NonQ(command);
					command="UPDATE procedurelog SET SecDateEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE SecDateEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog MODIFY SecDateEntry NOT NULL";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - proctp security");//No translation in convert script.
				//proctp table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE proctp ADD SecUserNumEntry bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE proctp ADD INDEX (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE proctp ADD SecDateEntry date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
					command="ALTER TABLE proctp ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE proctp SET SecDateTEdit = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE proctp ADD SecUserNumEntry number(20)";
					Db.NonQ(command);
					command="UPDATE proctp SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE proctp MODIFY SecUserNumEntry NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX proctp_SecUserNumEntry ON proctp (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE proctp ADD SecDateEntry date";
					Db.NonQ(command);
					command="UPDATE proctp SET SecDateEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE SecDateEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE proctp MODIFY SecDateEntry NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE proctp ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE proctp SET SecDateTEdit = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - treatplan security");//No translation in convert script.
				//treatplan table
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE treatplan ADD SecUserNumEntry bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE treatplan ADD INDEX (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE treatplan ADD SecDateEntry date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
					command="ALTER TABLE treatplan ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE treatplan SET SecDateTEdit = NOW()";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE treatplan ADD SecUserNumEntry number(20)";
					Db.NonQ(command);
					command="UPDATE treatplan SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE treatplan MODIFY SecUserNumEntry NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX treatplan_SecUserNumEntry ON treatplan (SecUserNumEntry)";
					Db.NonQ(command);
					command="ALTER TABLE treatplan ADD SecDateEntry date";
					Db.NonQ(command);
					command="UPDATE treatplan SET SecDateEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE SecDateEntry IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE treatplan MODIFY SecDateEntry NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE treatplan ADD SecDateTEdit timestamp";
					Db.NonQ(command);
					command="UPDATE treatplan SET SecDateTEdit = SYSTIMESTAMP";
					Db.NonQ(command);
				}
				#endregion Security Timestamp Tables (grouped by table instead of by column)
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - tasklists");//No translation in convert script.
				//Add TaskListCreate permission to everyone
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				table = Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					foreach(DataRow row in table.Rows) {
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							 +"VALUES("+POut.Long(PIn.Long(row["UserGroupNum"].ToString()))+",105)";  //TaskListCreate
						Db.NonQ(command);
					}
				}
				else {//oracle
					foreach(DataRow row in table.Rows) {
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) VALUES("
							 +"(SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(PIn.Long(row["UserGroupNum"].ToString()))+",105)";  //TaskListCreate
						Db.NonQ(command);
					}
				}
				//UserClinic linker table.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS userclinic";
					Db.NonQ(command);
					command=@"CREATE TABLE userclinic (
						UserClinicNum bigint NOT NULL auto_increment PRIMARY KEY,
						UserNum bigint NOT NULL,
						ClinicNum bigint NOT NULL,
						INDEX(UserNum),
						INDEX(ClinicNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE userclinic'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE userclinic (
						UserClinicNum number(20) NOT NULL,
						UserNum number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						CONSTRAINT userclinic_UserClinicNum PRIMARY KEY (UserClinicNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX userclinic_UserNum ON userclinic (UserNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX userclinic_ClinicNum ON userclinic (ClinicNum)";
					Db.NonQ(command);
				}
				command="SELECT UserNum,ClinicNum,ClinicIsRestricted FROM userod";//Get all users and their default clinics
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {//Insert a UserClinic entry for each user's default clinic.
					//If they're restricted enter their clinic into the table.  If they're not restricted we don't care.
					if(PIn.Bool(table.Rows[i]["ClinicIsRestricted"].ToString())) {
						if(DataConnection.DBtype==DatabaseType.MySql) {
							command="INSERT INTO userclinic (UserNum,ClinicNum) VALUES ("
								+table.Rows[i]["UserNum"].ToString()+","+table.Rows[i]["ClinicNum"].ToString()+")";
							Db.NonQ(command);
						}
						else {//oracle
							command="INSERT INTO userclinic (UserClinicNum,UserNum,ClinicNum) VALUES((SELECT MAX(UserClinicNum)+1 FROM userclinic),"
								+table.Rows[i]["UserNum"].ToString()+","+table.Rows[i]["ClinicNum"].ToString()+")";
							Db.NonQ(command);
						}
					}				
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('Ins834ImportPath','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'Ins834ImportPath','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('Ins834IsPatientCreate','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'Ins834IsPatientCreate','1')";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS dashboardlayout";
					Db.NonQ(command);
					command=@"CREATE TABLE dashboardlayout (
						DashboardLayoutNum bigint NOT NULL auto_increment PRIMARY KEY,
						UserNum bigint NOT NULL,
						UserGroupNum bigint NOT NULL,
						DashboardTabName varchar(255) NOT NULL,
						DashboardTabOrder int NOT NULL,
						DashboardRows int NOT NULL,
						DashboardColumns int NOT NULL,
						INDEX(UserNum),
						INDEX(UserGroupNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE dashboardlayout'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE dashboardlayout (
						DashboardLayoutNum number(20) NOT NULL,
						UserNum number(20) NOT NULL,
						UserGroupNum number(20) NOT NULL,
						DashboardTabName varchar2(255),
						DashboardTabOrder number(11) NOT NULL,
						DashboardRows number(11) NOT NULL,
						DashboardColumns number(11) NOT NULL,
						CONSTRAINT dashboardlayout_DashboardLayou PRIMARY KEY (DashboardLayoutNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX dashboardlayout_UserNum ON dashboardlayout (UserNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX dashboardlayout_UserGroupNum ON dashboardlayout (UserGroupNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS dashboardcell";
					Db.NonQ(command);
					command=@"CREATE TABLE dashboardcell (
						DashboardCellNum bigint NOT NULL auto_increment PRIMARY KEY,
						DashboardLayoutNum bigint NOT NULL,
						CellRow int NOT NULL,
						CellColumn int NOT NULL,
						CellType varchar(255) NOT NULL,
						CellSettings text NOT NULL,
						LastQueryTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						LastQueryData text NOT NULL,
						RefreshRateSeconds int NOT NULL,
						INDEX(DashboardLayoutNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE dashboardcell'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE dashboardcell (
						DashboardCellNum number(20) NOT NULL,
						DashboardLayoutNum number(20) NOT NULL,
						CellRow number(11) NOT NULL,
						CellColumn number(11) NOT NULL,
						CellType varchar2(255),
						CellSettings clob,
						LastQueryTime date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						LastQueryData clob,
						RefreshRateSeconds number(11) NOT NULL,
						CONSTRAINT dashboardcell_DashboardCellNum PRIMARY KEY (DashboardCellNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX dashboardcell_DashboardLayoutN ON dashboardcell (DashboardLayoutNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('RecurringChargesUsePriProv','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'RecurringChargesUsePriProv','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE statement ADD SuperFamily bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE statement ADD INDEX (SuperFamily)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE statement ADD SuperFamily number(20)";
					Db.NonQ(command);
					command="UPDATE statement SET SuperFamily = 0 WHERE SuperFamily IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE statement MODIFY SuperFamily NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX statement_SuperFamily ON statement (SuperFamily)";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - super billing");//No translation in convert script.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patient ADD HasSuperBilling tinyint NOT NULL";
					Db.NonQ(command);
					command="UPDATE patient SET HasSuperBilling=1 WHERE SuperFamily!=0 AND PatNum=Guarantor";//Include all superfamily member guarantors by default
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patient ADD HasSuperBilling number(3)";
					Db.NonQ(command);
					command="UPDATE patient SET HasSuperBilling = 0 WHERE HasSuperBilling IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patient MODIFY HasSuperBilling NOT NULL";
					Db.NonQ(command);
					command="UPDATE patient SET HasSuperBilling=1 WHERE SuperFamily!=0 AND PatNum=Guarantor";
					Db.NonQ(command);
				}
				//UserOdPref Table
        if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS userodpref";
					Db.NonQ(command);
					command=@"CREATE TABLE userodpref (
						UserOdPrefNum bigint NOT NULL auto_increment PRIMARY KEY,
						UserNum bigint NOT NULL,
						Fkey bigint NOT NULL,
						FkeyType tinyint NOT NULL,
						ValueString text NOT NULL,
						INDEX(UserNum),
						INDEX(Fkey)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE userodpref'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE userodpref (
						UserOdPrefNum number(20) NOT NULL,
						UserNum number(20) NOT NULL,
						Fkey number(20) NOT NULL,
						FkeyType number(3) NOT NULL,
						ValueString clob,
						CONSTRAINT userodpref_UserOdPrefNum PRIMARY KEY (UserOdPrefNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX userodpref_UserNum ON userodpref (UserNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX userodpref_Fkey ON userodpref (Fkey)";
					Db.NonQ(command);
				}
				//Update image categories to expand if original preference was set to expand.
				//Except the Statement folder will remain collapsed per Nathan.
				//Also update preference to new persistent option to encourage usage.
				command="SELECT ValueString FROM preference WHERE PrefName='ImagesModuleTreeIsCollapsed'";
				if(Db.GetScalar(command)=="0") {
					//Check for ItemValue 'S' incase someone renames their Statements image folder.  Category 18 is ImageCats according to Def.cs.
					command="UPDATE definition SET ItemValue=CONCAT(ItemValue,'E') WHERE Category=18 AND ItemValue NOT LIKE '%S%'";
					Db.NonQ(command);
					command="UPDATE preference SET ValueString=2 WHERE PrefName='ImagesModuleTreeIsCollapsed'";
					Db.NonQ(command);
				}
				else {//ImagesModuleTreeIsCollapsed=1
					//All folders are already flagged as collapsed by default.
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE dashboardlayout ADD DashboardGroupName varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE dashboardlayout ADD DashboardGroupName varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE wikipage ADD IsDraft tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE wikipage ADD IsDraft number(3)";
					Db.NonQ(command);
					command="UPDATE wikipage SET IsDraft = 0 WHERE IsDraft IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE wikipage MODIFY IsDraft NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PaymentsUsePatientClinic','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PaymentsUsePatientClinic','1')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ProgramAdditionalFeatures','0001-01-01 00:00:00')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ProgramAdditionalFeatures','0001-01-01 00:00:00')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString='' WHERE prefname='ProgramVersionLastUpdated'";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS updatehistory";
					Db.NonQ(command);
					command=@"CREATE TABLE updatehistory (
						UpdateHistoryNum bigint NOT NULL auto_increment PRIMARY KEY,
						DateTimeUpdated datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						ProgramVersion varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE updatehistory'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE updatehistory (
						UpdateHistoryNum number(20) NOT NULL,
						DateTimeUpdated date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						ProgramVersion varchar2(255),
						CONSTRAINT updatehistory_UpdateHistoryNum PRIMARY KEY (UpdateHistoryNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('LocalTimeOverridesServerTime','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'LocalTimeOverridesServerTime','0')";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.1 - insplan require verify");//No translation in convert script.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE insplan ADD HideFromVerifyList tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE insplan ADD HideFromVerifyList number(3)";
					Db.NonQ(command);
					command="UPDATE insplan SET HideFromVerifyList = 0 WHERE HideFromVerifyList IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE insplan MODIFY HideFromVerifyList NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE insverify ADD DateLastAssigned date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE insverify ADD DateLastAssigned date";
					Db.NonQ(command);
					command="UPDATE insverify SET DateLastAssigned = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateLastAssigned IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE insverify MODIFY DateLastAssigned NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE insverifyhist ADD DateLastAssigned date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE insverifyhist ADD DateLastAssigned date";
					Db.NonQ(command);
					command="UPDATE insverifyhist SET DateLastAssigned = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateLastAssigned IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE insverifyhist MODIFY DateLastAssigned NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptModuleAdjustmentsInProd','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptModuleAdjustmentsInProd','0')";
					Db.NonQ(command);
				}
				//Insert Romexis bridge-----------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
							+") VALUES("
							+"'Romexis', "
							+"'Romexis from www.planmeca.com', "
							+"'0', "
							+"'"+POut.String(@"C:\Program Files\Planmeca\Romexis")+"', "
							+"'', "//leave blank if none
							+"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
							+") VALUES("
							+"'"+POut.Long(programNum)+"', "
							+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
							+"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
							+"VALUES ("
							+"'"+POut.Long(programNum)+"', "
							+"'2', "//ToolBarsAvail.ChartModule
							+"'Romexis')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
							+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
							+"'Romexis', "
							+"'Romexis from www.planmeca.com', "
							+"'0', "
							+"'"+POut.String(@"C:\Program Files\Planmeca\Romexis")+"', "
							+"'', "//leave blank if none
							+"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
							+") VALUES("
							+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
							+"'"+POut.Long(programNum)+"', "
							+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
							+"'0', "
							+"0)";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
							+"VALUES ("
							+"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
							+"'"+POut.Long(programNum)+"', "
							+"'2', "//ToolBarsAvail.ChartModule
							+"'Romexis')";
					Db.NonQ(command);
				}//end Romexis bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ClinicTrackLast','Workstation')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ClinicTrackLast','Workstation')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SuperFamSortStrategy','2')";//2=PatNumAsc
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SuperFamSortStrategy','2')";//2=PatNumAsc
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('InsVerifyDefaultToCurrentUser','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'InsVerifyDefaultToCurrentUser','0')";
					Db.NonQ(command);
				}
				//People who used to be able to access the dashboard now get Graphical Report Setup permission.
				command="SELECT grouppermission.UserGroupNum FROM grouppermission WHERE PermType=59"; //Graphical Reports
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					for(int i = 0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",107)";//Graphical Report Setup
						Db.NonQ(command);
					}
				}
				else {//oracle
					for(int i = 0;i<table.Rows.Count;i++) {
						groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",107)";//Graphical Report Setup
						Db.NonQ(command);
					}
				}
				//Add automated Reminders email message preference (Also run in 15.4.32. okay to run again here.)
				//Convert old messages into new messages
				//Day Message======================================================================================
				command="SELECT ValueString FROM preference WHERE PrefName='ApptReminderDayMessage'";
				string reminderText = Db.GetScalar(command);
				if(reminderText=="[nameFL],\r\nYou have an upcoming appointment on[apptDate] at[apptTime] with[practiceName] with[provName].\r\nThanks!") {//old default mesage.
					reminderText=@"Appointment Reminder: [nameF] is scheduled for [apptTime] on [apptDate] at [clinicName]. Call [clinicPhone] if issue. No Reply";
				}
				reminderText=reminderText.Replace("[nameFL]","[nameF]");
				reminderText=reminderText.Replace("[nameL]","[nameF]");
				reminderText=reminderText.Replace("[nameFLnoPref]","[nameF]");
				command="UPDATE preference SET ValueString = '"+POut.String(reminderText)+"' WHERE PrefName = 'ApptReminderDayMessage'";
				Db.NonQ(command);
				//Hour message (deprecated, but convert it anyway)=================================================
				command="SELECT ValueString FROM preference WHERE PrefName='ApptReminderHourMessage'";
				reminderText = Db.GetScalar(command);
				if(reminderText=="[nameFL],\r\nYou have an upcoming appointment on[apptDate] at[apptTime] with[practiceName] with[provName].\r\nThanks!") {//old default mesage.
					reminderText=@"Appointment Reminder: [nameF] is scheduled for [apptTime] on [apptDate] at [clinicName]. Call [clinicPhone] if issue. No Reply";
				}
				reminderText=reminderText.Replace("[nameFL]","[nameF]");
				reminderText=reminderText.Replace("[nameL]","[nameF]");
				reminderText=reminderText.Replace("[nameFLnoPref]","[nameF]");
				command="UPDATE preference SET ValueString = '"+POut.String(reminderText)+"' WHERE PrefName = 'ApptReminderHourMessage'";
				Db.NonQ(command);
				//This pref was also added to the 15.4.32 conversion.
				command = "SELECT COUNT(*) FROM preference WHERE PrefName='ApptReminderEmailMessage'";
				if(Db.GetScalar(command)=="0") {//only add if not already exists.
					reminderText = "Dental appointment reminder from [clinicName]:\n"
						+"[nameF] is scheduled for an appointment at [apptTime] on [apptDate].\n"
						+"There is no need to reply if you are going to make this appointment, but if there is any issue please call [clinicPhone] as soon as possible.\n"
						+"Thanks!";
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptReminderEmailMessage','"+reminderText+"')";
						Db.NonQ(command);
					}
					else {//oracle
						command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptReminderEmailMessage','"+reminderText+"')";
						Db.NonQ(command);
					}
				}
				//New Statement Columns
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE statement ADD IsBalValid tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE statement ADD IsBalValid number(3)";
					Db.NonQ(command);
					command="UPDATE statement SET IsBalValid = 0 WHERE IsBalValid IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE statement MODIFY IsBalValid NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE statement ADD InsEst double NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE statement ADD InsEst number(38,8)";
					Db.NonQ(command);
					command="UPDATE statement SET InsEst = 0 WHERE InsEst IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE statement MODIFY InsEst NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE statement ADD BalTotal double NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE statement ADD BalTotal number(38,8)";
					Db.NonQ(command);
					command="UPDATE statement SET BalTotal = 0 WHERE BalTotal IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE statement MODIFY BalTotal NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE carrier ADD TIN varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE carrier ADD TIN varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurecode ADD DefaultClaimNote TEXT NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurecode ADD DefaultClaimNote clob";
					Db.NonQ(command);
				}
        command="SELECT ProgramNum FROM program WHERE ProgName='Podium'";
        string podiumProgramNum=Db.GetScalar(command);
				//Create a new program property for having Podium run via the eConnector instead of on the workstation(s).
				//Users may call in and complain about "multiple commlogs" being created in which case they need to upgrade to using the eConnector.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
							+") VALUES("
							+"'"+POut.String(podiumProgramNum)+"', "
							+"'Enter 0 to use Open Dental for sending review invitations, or 1 to use eConnector', "
							+"'0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
							+") VALUES("
							+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
							+"'"+POut.String(podiumProgramNum)+"', "
							+"'Enter 0 to use Open Dental for sending review invitations, or 1 to use eConnector', "
							+"'0', "
							+"0)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
							+") VALUES("
							+"'"+POut.String(podiumProgramNum)+"', "
							+"'Send after appointment completed (minutes)', "
							+"'5')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
							+") VALUES("
							+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
							+"'"+POut.String(podiumProgramNum)+"', "
							+"'Send after appointment completed (minutes)', "
							+"'5', "
							+"0)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
							+") VALUES("
							+"'"+POut.String(podiumProgramNum)+"', "
							+"'Send after appointment time arrived (minutes)', "
							+"'5')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
							+") VALUES("
							+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
							+"'"+POut.String(podiumProgramNum)+"', "
							+"'Send after appointment time arrived (minutes)', "
							+"'5', "
							+"0)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
							+") VALUES("
							+"'"+POut.String(podiumProgramNum)+"', "
							+"'Send after appointment time dismissed (minutes)', "
							+"'5')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
							+") VALUES("
							+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
							+"'"+POut.String(podiumProgramNum)+"', "
							+"'Send after appointment time dismissed (minutes)', "
							+"'5', "
							+"0)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
							+") VALUES("
							+"'"+POut.String(podiumProgramNum)+"', "
							+"'New patient trigger type', "
							+"'0')";//ReviewInvitationTrigger.AppointmentCompleted
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
							+") VALUES("
							+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
							+"'"+POut.String(podiumProgramNum)+"', "
							+"'New patient trigger type', "
							+"'0', "//ReviewInvitationTrigger.AppointmentCompleted
							+"0)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
							+") VALUES("
							+"'"+POut.String(podiumProgramNum)+"', "
							+"'Existing patient trigger type', "
							+"'1')";//ReviewInvitationTrigger.AppointmentTimeArrived
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
							+") VALUES("
							+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
							+"'"+POut.String(podiumProgramNum)+"', "
							+"'Existing patient trigger type', "
							+"'1', "//ReviewInvitationTrigger.AppointmentTimeArrived
							+"0)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE eduresource ADD SmokingSnoMed varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE eduresource ADD SmokingSnoMed varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE intervention ADD IsPatDeclined tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE intervention ADD IsPatDeclined number(3)";
					Db.NonQ(command);
					command="UPDATE intervention SET IsPatDeclined = 0 WHERE IsPatDeclined IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE intervention MODIFY IsPatDeclined NOT NULL";
					Db.NonQ(command);
				}

				command="UPDATE preference SET ValueString = '16.1.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_1_6();
		}

		private static void To16_1_6() {
			if(FromVersion<new Version("16.1.6.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.6.0");//No translation in convert script.
				string command = "";
				//Value for this pref should not be included in the convert script
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('JobManagerDefaultEmail','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'JobManagerDefaultEmail','')";
					Db.NonQ(command);
				}
				//Value for this pref should not be included in the convert script
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('JobManagerDefaultBillingMsg','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'JobManagerDefaultBillingMsg','')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString='16.1.6.0' WHERE PrefName='DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_1_13();
		}

		///<summary>Oracle compatible: 03/30/2016</summary>
		private static void To16_1_13() {
			if(FromVersion<new Version("16.1.13.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.13.0");//No translation in convert script.
				string command="";
				command="SELECT DefNum FROM definition WHERE ItemName='Unverified' AND Category=38";//38 = InsuranceVerificationStatus
				string unverifiedDefNum=Db.GetScalar(command);
				command="SELECT DefNum FROM definition WHERE ItemName='Verified' AND Category=38";
				string verifiedDefNum=Db.GetScalar(command);
				command="UPDATE insverify SET DefNum='0' WHERE DefNum="+unverifiedDefNum+" OR DefNum="+verifiedDefNum;
				Db.NonQ(command);
				command="UPDATE insverifyhist SET DefNum='0' WHERE DefNum="+unverifiedDefNum+" OR DefNum="+verifiedDefNum;
				Db.NonQ(command);
				command="DELETE FROM definition WHERE DefNum="+unverifiedDefNum;
				Db.NonQ(command);
				command="DELETE FROM definition WHERE DefNum="+verifiedDefNum;
				Db.NonQ(command);
				//Add 2 new definitions for the Insurance Verification Status definition category.
				if(DataConnection.DBtype==DatabaseType.MySql) { //38 is DefCat.InsuranceVerificationStatus
					command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemValue) VALUES (38,'Attempted',0,'')";
					Db.NonQ(command);
					command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemValue) VALUES (38,'See Notes',1,'')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO definition (DefNum,Category,ItemName,ItemOrder,ItemValue) "
						+"VALUES ((SELECT MAX(DefNum)+1 FROM definition),38,'Attempted',0,'')";
					Db.NonQ(command);
					command="INSERT INTO definition (DefNum,Category,ItemName,ItemOrder,ItemValue) "
						+"VALUES ((SELECT MAX(DefNum)+1 FROM definition),38,'See Notes',1,'')";
					Db.NonQ(command);
				}
				//Add InsPlanVerifyList permission to everyone------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				DataTable table=Db.GetTable(command);
				long groupNum;
				if(DataConnection.DBtype==DatabaseType.MySql) {
				   foreach(DataRow row in table.Rows) {
					  groupNum=PIn.Long(row["UserGroupNum"].ToString());
					  command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(groupNum)+",115)";//115 - InsPlanVerifyList
					  Db.NonQ(command);
				   }
				}
				else {//oracle
				   foreach(DataRow row in table.Rows) {
					  groupNum=PIn.Long(row["UserGroupNum"].ToString());
					  command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
						 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",115)";//115 - InsPlanVerifyList
					  Db.NonQ(command);
				   }
				}
				command="UPDATE preference SET ValueString='16.1.13.0' WHERE PrefName='DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_1_15();
		}

		private static void To16_1_15() {
			if(FromVersion<new Version("16.1.15.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.15.0");//No translation in convert script.
				string command = "";
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("commlog","PatNum,CommDateTime,CommType")) {
							command="ALTER TABLE commlog ADD INDEX indexPNCDateCType (PatNum,CommDateTime,CommType)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX commlog_PNCDateCType ON commlog (PatNum,CommDateTime,CommType)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				command="UPDATE preference SET ValueString='16.1.15.0' WHERE PrefName='DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_1_16();
		}

		///<summary>Oracle compatible: 04/25/2016</summary>
		private static void To16_1_16() {
			if(FromVersion<new Version("16.1.16.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.16.0");//No translation in convert script.
				string command="";
				command="UPDATE program SET ProgName='Dimaxis' WHERE ProgName='Planmeca'";//Renaming Planmeca to Dimaxis
				Db.NonQ(command);
				//Add program property for birthdate format for Dimaxis
				command=@"SELECT COUNT(*) FROM programproperty 
					INNER JOIN program ON program.ProgramNum=programproperty.ProgramNum
						AND program.ProgName='Dimaxis'
					WHERE programproperty.PropertyDesc='Birthdate format (usually dd/MM/yyyy or MM/dd/yyyy)'";
				if(Db.GetCount(command)=="0") {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command=@"INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue) 
							VALUES ((SELECT ProgramNum FROM program WHERE ProgName='Dimaxis'), 
							'Birthdate format (usually dd/MM/yyyy or MM/dd/yyyy)','dd/MM/yyyy')";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum) 
							VALUES ((SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),(SELECT ProgramNum FROM program WHERE ProgName='Dimaxis'),
							'Birthdate format (usually dd/MM/yyyy or MM/dd/yyyy)','dd/MM/yyyy',0)";
						Db.NonQ(command);
					}
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.16 - carrier index");//No translation in convert script.
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("carrier","CarrierNum,CarrierName")) {
							command="ALTER TABLE carrier ADD INDEX CarrierNumName (CarrierNum,CarrierName)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX carrier_CarrierNumName ON carrier (CarrierNum,CarrierName)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.16 - claim index");//No translation in convert script.
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("claim","PlanNum,ClaimStatus,ClaimType,PatNum,ClaimNum,DateService,ProvTreat,ClaimFee,ClinicNum")) {
							command="ALTER TABLE claim ADD INDEX indexOutClaimCovering (PlanNum,ClaimStatus,ClaimType,PatNum,ClaimNum,DateService,ProvTreat,ClaimFee,ClinicNum)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX claim_OutClaimCovering ON claim (PlanNum,ClaimStatus,ClaimType,PatNum,ClaimNum,DateService,ProvTreat,ClaimFee,ClinicNum)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.16 - claimproc index");//No translation in convert script.
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("claimproc","ClaimNum,ClaimPaymentNum,InsPayAmt")) {
							command="ALTER TABLE claimproc ADD INDEX indexOutClaimCovering (ClaimNum,ClaimPaymentNum,InsPayAmt)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX claimproc_OutClaimCovering ON claimproc (ClaimNum,ClaimPaymentNum,InsPayAmt)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.16 - insplan index");//No translation in convert script.
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("insplan","CarrierNum,PlanNum")) {
							command="ALTER TABLE insplan ADD INDEX CarrierNumPlanNum (CarrierNum,PlanNum)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX insplan_CarrierNumPlanNum ON insplan (CarrierNum,PlanNum)";
						Db.NonQ(command);
					}
				}
				catch(Exception ex) {
					ex.DoNothing();//Only an index. (Exception ex) required to catch thrown exception
				}
				command="UPDATE preference SET ValueString='16.1.16.0' WHERE PrefName='DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_1_20();
		}

		///<summary>This conversion script run in versions 15.4.50, and 16.1.20</summary>
		private static void To16_1_20() {
			if(FromVersion<new Version("16.1.20.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.1.20.0");//No translation in convert script.
				//Enable TP charting for all views if no views are enabled. Do nothing if mixed case. Oracle and MySQL compatible
				string command="SELECT COUNT(*) FROM chartview WHERE IsTpCharting=1";
				string result=Db.GetScalar(command);
				if(result=="0")	{
					command="UPDATE chartview SET IsTpCharting=1";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString='16.1.20.0' WHERE PrefName='DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_2_1();
		}

	}
}

