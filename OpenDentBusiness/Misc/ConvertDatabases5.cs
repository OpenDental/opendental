using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using DataConnectionBase;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace OpenDentBusiness {
	public partial class ConvertDatabases {
		#region B11013 Methods
		public const string PRESERVE_USERWEB="preserve_userweb";
		public const string PRESERVE_INVITE="preserve_patientportalinvite";
		public const string PRESERVE_EMAIL="preserve_emailmessage";
		public const string PREFNAME_B11013="B11013Status";
		[Flags]
		public enum B11013Status {
			NotStarted=0,
			ConvertDone=1,
			RedactedUploadDone=2,
			EmptyUserWebFixDone=4,
		}

		///<summary>Special method which fixes B11013 which was backported to versions 17.4 - 18.4 (head at the time) and should only ever be allowed to 
		///complete its operation once.  Safe to call from multiple versions as it will prevent subsequent runs with a pref it inserts.</summary>
		private static void FixB11013() {
			string command=$"SELECT COUNT(*) FROM preference WHERE PrefName='{PREFNAME_B11013}'";
			//The existence of the Pref is an indicator that this method has already executed before by a previously backported version.
			bool prefExists=PIn.Int(Db.GetCount(command)) > 0;
			if(prefExists) {
				return;//Only execute once.
			}
			#region Insert Pref (indicate to this method never to run again)
			//Default the ValueString column to 0 which indicates that no maintenance needs to be performed.
			Db.NonQ($"INSERT INTO preference(PrefName,ValueString) VALUES('{PREFNAME_B11013}','{(int)B11013Status.NotStarted}')");
			#endregion
			#region Check for Invites
			//The presence of patient portal invites will require maintenance.
			command="SELECT COUNT(*) FROM patientportalinvite";
			int countInvites=PIn.Int(Db.GetCount(command));
			if(countInvites<=0) {
				//The office has either never turned patient portal invites on or has turned it on and no invites were ever sent.
				return;//Nothing to do.
			}
			#endregion
			#region Create Backups (only patient portal invite related rows)
			Action<string,string> actionCreateTable=(tableName,selectQuery) => {
				Db.NonQ($"DROP TABLE IF EXISTS {tableName}");
				Db.NonQ($"CREATE TABLE {tableName} {selectQuery}");
			};
			//Preserve UserWeb.
			actionCreateTable(
				PRESERVE_USERWEB,
				"SELECT u.* FROM userweb u WHERE u.FKeyType=1 AND u.FKey IN(SELECT p.PatNum FROM patientportalinvite p)");//FKeyType 1 = PatientPortal
			//Preserve PatientPortalInvite.
			actionCreateTable(
				PRESERVE_INVITE,
				"SELECT * FROM patientportalinvite");
			//Preserve EmailMessage.
			actionCreateTable(
				PRESERVE_EMAIL,
				"SELECT e.* FROM emailmessage e WHERE e.EmailMessageNum IN(SELECT p.EmailMessageNum FROM patientportalinvite p)");
			#endregion
			#region Delete Duplicate UserWebs (only allow one per PatNum (B11069))
			Action<long> actionDeleteUserWeb=(userWebNum) => {
				Db.NonQ($"DELETE FROM userweb WHERE UserWebNum={POut.Long(userWebNum)}");
			};
			//Find any duplicate patient portal userweb entries that are associated to the same FKey (patient).
			command="SELECT UserWebNum,FKey,DateTimeLastLogin FROM userweb WHERE FKeyType=1 ORDER BY FKey, DateTimeLastLogin DESC, UserWebNum DESC";
			var listAllUserWebsBeforeDeleteDupePatNums=Db.GetTable(command).AsEnumerable()
				.Select(x => new {
					UserWebNum=PIn.Long(x["UserWebNum"].ToString()),
					PatNum=PIn.Long(x["FKey"].ToString()),//FK to patient.PatNum when FKeyType=1
					DateTimeLastLogin=PIn.DateT(x["DateTimeLastLogin"].ToString()),
				}).ToList();
			//Make PatNum groups.
			var listUserWebDuplicatesByPatNum=listAllUserWebsBeforeDeleteDupePatNums
				//Group each PatNum.
				.GroupBy(x => x.PatNum)
				//Only where a PatNum has more than 1 entry.
				.Where(x => x.Count()>1)
				//Last login on top for each group.
				.Select(x => x.OrderByDescending(y => y.DateTimeLastLogin).ThenByDescending(y => y.UserWebNum).ToList())
				.ToList();
			//Loop through and delete all but latest login per each PatNum group.
			foreach(var listDuplicatesForPatNum in listUserWebDuplicatesByPatNum) {
				//List is ordered by DateTimeLastLogin DESC so leave the most recent entry.
				var listDuplicatesToDelete=listDuplicatesForPatNum.Skip(1);
				//We know we had at least 2 entries so delete all but the most recent used.
				foreach(var deleteDuplicate in listDuplicatesToDelete) {
					actionDeleteUserWeb(deleteDuplicate.UserWebNum);
				}
			}
			#endregion
			#region Blank out UserName (for any userwebs associated to patient portal invites)
			command="SELECT u.UserWebNum,u.FKey FROM userweb u WHERE u.FKeyType=1 AND u.FKey IN(SELECT p.PatNum FROM patientportalinvite p)";
			var listAllUserWebsThatHaveInvites=Db.GetTable(command).AsEnumerable()
				.Select(x => new {
					UserWebNum=PIn.Long(x["UserWebNum"].ToString()),
					PatNum=PIn.Long(x["FKey"].ToString()),//FK to patient.PatNum when FKeyType=1
				}).ToList();
			//Loop through and blank out all UserNames for these UserWebs.
			foreach(var userWebThatHasInvites in listAllUserWebsThatHaveInvites) {
				Db.NonQ($"UPDATE userweb SET UserName='' WHERE UserWebNum={POut.Long(userWebThatHasInvites.UserWebNum)}");
			}
			#endregion
			#region Update Pref (alerts eConnector to continue the process)
			Db.NonQ($"UPDATE preference SET ValueString='{(int)B11013Status.ConvertDone}' WHERE PrefName='{PREFNAME_B11013}'");
			#endregion
		}
		#endregion

		private static void UpdateSheetFieldDefFieldNameForSheetType(string fieldNameOld,string fieldNameNew,params int[] arraySheetTypes) {
			if(arraySheetTypes==null || arraySheetTypes.Length < 1) {
				return;
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				string command="UPDATE sheetfielddef sfd,sheetdef sd "
					+"SET sfd.FieldName='"+POut.String(fieldNameNew)+"' "
					+"WHERE sfd.SheetDefNum=sd.SheetDefNum "
					+"AND sd.SheetType IN ("+string.Join(",",arraySheetTypes)+") "
					+"AND sfd.FieldName = '"+POut.String(fieldNameOld)+"'";
				Db.NonQ(command);
			}
			else {//Oracle
				//A correlated update is necessary in order to utilize more than one table in an update statement for Oracle.
				string command="UPDATE sheetfielddef sfd "
					+"SET sfd.FieldName='"+POut.String(fieldNameNew)+"' "
					+"WHERE sfd.FieldName='"+POut.String(fieldNameOld)+"' "
					+"AND EXISTS ("
						+"SELECT * FROM sheetdef sd "
						+"WHERE sfd.SheetDefNum=sd.SheetDefNum "
						+"AND sd.SheetType IN ("+string.Join(",",arraySheetTypes)+") "
					+")";
				Db.NonQ(command);
			}
		}

		///<summary>Canada only and for MySQL only.  Aborts silently on failure, because user can delete the duplicates manually.</summary>
		private static void CanadaDeleteDuplicatePreauthClaimprocs() {
			if(DataConnection.DBtype!=DatabaseType.MySql || !CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canada only.
				return;
			}
			//A bug in 17.4.50.0 caused Canadian preauths with labs to duplicate preauth lab claim procs when visiting the TP module.
			//Below we delete duplicate Canadian Lab Fee PreAuth claim procs, preserving one instance.
			//This will allow the preauths to maintain a single claim proc to display correctly.
			//All identified issue claim procs should be identical, so we pick the lowest numbered ClaimProcNum to preserve.
			try {
				//Get a list of duplicate Canadian labs for preauths.
				string command="SELECT claimproc.ClaimProcNum,claimproc.ProcNum,ClaimProc.ClaimNum,claimproc.PatNum "
					+"FROM claimproc "
					+"INNER JOIN procedurelog ON procedurelog.ProcNum=claimproc.ProcNum "
					+"AND procedurelog.ProcNumLab!=0 "//Canadian Lab Fees.
					+"WHERE claimproc.Status=2 "//Preauth.
					+"ORDER BY ProcNum,ClaimNum";
				DataTable table=Db.GetTable(command);
				List<long> listDuplicateCanadianClaimProcNums=new List<long>();
				for(int i=1;i<table.Rows.Count;i++) {
					ClaimProc claimProcPrev=new OpenDentBusiness.ClaimProc() {
						ProcNum=PIn.Long(table.Rows[i-1]["ProcNum"].ToString()),
						ClaimProcNum=PIn.Long(table.Rows[i-1]["ClaimProcNum"].ToString()),
						ClaimNum=PIn.Long(table.Rows[i-1]["ClaimNum"].ToString()),
						PatNum=PIn.Long(table.Rows[i-1]["PatNum"].ToString())
					};
					ClaimProc claimProcCur=new OpenDentBusiness.ClaimProc() {
						ProcNum=PIn.Long(table.Rows[i]["ProcNum"].ToString()),
						ClaimProcNum=PIn.Long(table.Rows[i]["ClaimProcNum"].ToString()),
						ClaimNum=PIn.Long(table.Rows[i]["ClaimNum"].ToString()),
						PatNum=PIn.Long(table.Rows[i]["PatNum"].ToString())
					};
					if(claimProcPrev.ProcNum==claimProcCur.ProcNum && claimProcPrev.ClaimNum==claimProcCur.ClaimNum) {
						listDuplicateCanadianClaimProcNums.Add(claimProcCur.ClaimProcNum);
					}
				}
				if(listDuplicateCanadianClaimProcNums.Count>0) {
					command="DELETE "
					+"FROM claimproc "
					+"WHERE claimproc.ClaimProcNum IN("+string.Join(",",listDuplicateCanadianClaimProcNums)+")";//Canadian lab preauth claim procs
					Db.NonQ(command);
				}
			}
			catch(Exception ex) {
				ex.DoNothing();//The user can fix the duplicates manually.
			}
		}

		///<summary>This is the start of our new ConvertDatabases pattern where engineers do not need to worry about versioning info.
		///From this point on, the only version that engineers need to know about is the version within the method name.</summary>
		private static void To17_1_1() {
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODDentalSealantMeasure',0,'FQHC Dental Sealant Measure',6,0)";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO displayreport(DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODDentalSealantMeasure',0,'FQHC Dental Sealant Measure',6,0)";
				Db.NonQ(command);
			}
			command="SELECT MAX(displayreport.ItemOrder) FROM displayreport WHERE displayreport.Category = 2"; //monthly
			long itemorder = Db.GetLong(command)+1; //get the next available ItemOrder for the Monthly Category to put this new report last.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODInsurancePayPlansPastDue',"+POut.Long(itemorder)+",'Ins Pay Plans Past Due',2,0)";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO displayreport(DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODInsurancePayPlansPastDue',"+POut.Long(itemorder)+",'Ins Pay Plans Past Due',2,0)";
				Db.NonQ(command);
			}
			//Add ReportDaily permission to groups with existing ReportProdInc permission------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=39";  //ReportProdInc
			DataTable table = Db.GetTable(command);
			long groupNum;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",133)";  //ReportDaily
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",133)";  //ReportDaily
					Db.NonQ(command);
				}
			}
			//Add ReportProdIncAllProviders permission to groups with existing ReportProdInc permission------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=39";  //ReportProdInc
			DataTable tableAllProv = Db.GetTable(command);
			long groupNumAllProv;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in tableAllProv.Rows) {
					groupNumAllProv=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNumAllProv)+",132)";  //ReportProdIncAllProviders
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in tableAllProv.Rows) {
					groupNumAllProv=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNumAllProv)+",132)";  //ReportProdIncAllProviders
					Db.NonQ(command);
				}
			}
			//Add ReportDailyAllProviders permission to groups with existing ReportProdInc permission------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=39";  //ReportProdInc
			DataTable tableAllDaily = Db.GetTable(command);
			long groupNumAllDaily;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in tableAllDaily.Rows) {
					groupNumAllDaily=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNumAllDaily)+",134)";  //ReportDailyAllProviders
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in tableAllDaily.Rows) {
					groupNumAllDaily=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNumAllDaily)+",134)";  //ReportDailyAllProviders
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('PasswordsWeakChangeToStrong','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'PasswordsWeakChangeToStrong','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimPaymentNoShowZeroDate',CURDATE())";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ClaimPaymentNoShowZeroDate',SYSDATE)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS patientlink";
				Db.NonQ(command);
				command=@"CREATE TABLE patientlink (
					PatientLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
					PatNumFrom bigint NOT NULL,
					PatNumTo bigint NOT NULL,
					LinkType tinyint NOT NULL,
					DateTimeLink datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					INDEX(PatNumFrom),
					INDEX(PatNumTo)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE patientlink'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE patientlink (
					PatientLinkNum number(20) NOT NULL,
					PatNumFrom number(20) NOT NULL,
					PatNumTo number(20) NOT NULL,
					LinkType number(3) NOT NULL,
					DateTimeLink date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
					CONSTRAINT patientlink_PatientLinkNum PRIMARY KEY (PatientLinkNum)
					)";
				Db.NonQ(command);
				command=@"CREATE INDEX patientlink_PatNumFrom ON patientlink (PatNumFrom)";
				Db.NonQ(command);
				command=@"CREATE INDEX patientlink_PatNumTo ON patientlink (PatNumTo)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('QuickBooksClassRefs','')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'QuickBooksClassRefs','')";
				Db.NonQ(command);
			}
			command="UPDATE program SET ProgDesc='Carestream Ortho/OMS from www.carestreamdental.com' WHERE ProgName='Carestream'";//Renaming Carestream TO Carestream Ortho/OMS
			Db.NonQ(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE clinic ADD ExternalID bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE clinic ADD INDEX (ExternalID)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE clinic ADD ExternalID number(20)";
				Db.NonQ(command);
				command="UPDATE clinic SET ExternalID = 0 WHERE ExternalID IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE clinic MODIFY ExternalID NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX clinic_ExternalID ON clinic (ExternalID)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('CommLogAutoSave','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'CommLogAutoSave','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE displayfield ADD DescriptionOverride varchar(255) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE displayfield ADD DescriptionOverride varchar2(255)";
				Db.NonQ(command);
			}
			//Get the 1500_02_12 form.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD12' LIMIT 1";
			}
			else {//oracle doesn't have LIMIT
				command="SELECT * FROM (SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD12') WHERE RowNum<=1";
			}
			long claimFormNum = PIn.Long(Db.GetScalar(command));
			command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
				+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','AcceptAssignmentY','','400','951','0','0')";
			Db.NonQ(command);
			command="INSERT INTO claimformitem (ClaimFormItemNum,ClaimFormNum,ImageFileName,FieldName,FormatString,XPos,YPos,Width,Height) "
				+"VALUES ("+GetClaimFormItemNum()+","+POut.Long(claimFormNum)+",'','AcceptAssignmentN','','450','951','0','0')";
			Db.NonQ(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE userod ADD DateTFail datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE userod ADD DateTFail date";
				Db.NonQ(command);
				command="UPDATE userod SET DateTFail = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateTFail IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE userod MODIFY DateTFail NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE userod ADD FailedAttempts tinyint unsigned NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE userod ADD FailedAttempts number(3)";
				Db.NonQ(command);
				command="UPDATE userod SET FailedAttempts = 0 WHERE FailedAttempts IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE userod MODIFY FailedAttempts NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE claim ADD DateSentOrig date NOT NULL DEFAULT '0001-01-01'";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE claim ADD DateSentOrig date";
				Db.NonQ(command);
				command="UPDATE claim SET DateSentOrig = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateSentOrig IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE claim MODIFY DateSentOrig NOT NULL";
				Db.NonQ(command);
			}
			//SheetDelete permission - Added to anybody that has SheetEdit
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=42";//42 - SheetEdit
			table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES("+POut.Long(groupNum)+",136)";//136 - SheetDelete
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
						 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",136)";//136 - SheetDelete
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ClaimPaymentBatchOnly','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ClaimPaymentBatchOnly','0')";
				Db.NonQ(command);
			}
			//UpdateCustomTracking permission - Added to anybody that has ClaimSentEdit
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=14";//14 - ClaimSentEdit
			table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES("+POut.Long(groupNum)+",137)";//137 - UpdateCustomTracking
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
						 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",137)";//137 - UpdateCustomTracking
					Db.NonQ(command);
				}
			}
			//Add GraphicsEdit permission to everyone------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(groupNum)+",138)";  //138 - GraphicsEdit
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
						 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",138)";  //138 - GraphicsEdit
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedRecallIgnoreBlockoutTypes','')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES "
					+"((SELECT MAX(PrefNum)+1 FROM preference),'WebSchedRecallIgnoreBlockoutTypes','')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedNewPatApptIgnoreBlockoutTypes','')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES "
					+"((SELECT MAX(PrefNum)+1 FROM preference),'WebSchedNewPatApptIgnoreBlockoutTypes','')";
				Db.NonQ(command);
			}
			string defaultApptMessage = "Your first dental appointment with us will include a comprehensive exam, xrays and a consultation with the dentist.  "
				+"The dentist will provide treatment options, recommend care and address any remaining questions.";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedNewPatApptMessage','"+defaultApptMessage+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES "
					+"((SELECT MAX(PrefNum)+1 FROM preference),'WebSchedNewPatApptMessage','"+defaultApptMessage+"')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('AgingServiceTimeDue','')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'AgingServiceTimeDue','')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE refattach CHANGE IsFrom RefType tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE refattach RENAME COLUMN IsFrom TO RefType";
				Db.NonQ(command);
			}
			//RAMQ clearinghouse.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command=@"INSERT INTO clearinghouse (Description,ExportPath,Payors,Eformat,ISA05,SenderTin,ISA07,ISA08,ISA15,Password,ResponsePath,
					CommBridge,ClientProgram,LastBatchNumber,ModemPort,LoginID,SenderName,SenderTelephone,GS03,ISA02,ISA04,ISA16,SeparatorData,SeparatorSegment)
					VALUES ('Ramq','"+POut.String(@"C:\Ramq\")+"','','7','','','','','','','','18','',0,0,'','','','','','','','','')";
				Db.NonQ(command);
			}
			else {//oracle
				command=@"INSERT INTO clearinghouse (ClearinghouseNum,Description,ExportPath,Payors,Eformat,ISA05,SenderTin,ISA07,ISA08,ISA15,Password,
					ResponsePath,CommBridge,ClientProgram,LastBatchNumber,ModemPort,LoginID,SenderName,SenderTelephone,GS03,ISA02,ISA04,ISA16,SeparatorData,
					SeparatorSegment,ClinicNum,HqClearinghouseNum) VALUES ((SELECT MAX(ClearinghouseNum+1) FROM clearinghouse),
					'Ramq','"+POut.String(@"C:\Ramq\")+"','','7','','','','','','','','18','',0,0,'','','','','','','','','',0,0)";
				Db.NonQ(command);
			}
			command="UPDATE clearinghouse SET HqClearinghouseNum=ClearinghouseNum WHERE HqClearinghouseNum=0";
			Db.NonQ(command);
			//REPORTING SERVER PREFERENCES
			//serverName
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ReportingServerCompName','')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES "
					+"((SELECT MAX(PrefNum)+1 FROM preference),'ReportingServerCompName','')";
				Db.NonQ(command);
			}
			//dbName
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ReportingServerDbName','')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES "
					+"((SELECT MAX(PrefNum)+1 FROM preference),'ReportingServerDbName','')";
				Db.NonQ(command);
			}
			//mysqlUser
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ReportingServerMySqlUser','')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES "
					+"((SELECT MAX(PrefNum)+1 FROM preference),'ReportingServerMySqlUser','')";
				Db.NonQ(command);
			}
			//mysqlPassHash
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ReportingServerMySqlPassHash','')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES "
					+"((SELECT MAX(PrefNum)+1 FROM preference),'ReportingServerMySqlPassHash','')";
				Db.NonQ(command);
			}
			//ReportingServer URI for middle tier
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ReportingServerURI','')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES "
					+"((SELECT MAX(PrefNum)+1 FROM preference),'ReportingServerURI','')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedAutomaticSendTextSetting','0')";//Do not send by default
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),"
					+"'WebSchedAutomaticSendTextSetting','0')";//Do not send by default
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('RecallStatusTexted','0')";//Blank by default
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),"
					+"'RecallStatusTexted','0')";//Do not send by default
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('RecallStatusEmailedTexted','0')";//Blank by default
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),"
					+"'RecallStatusEmailedTexted','0')";//Do not send by default
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS webschedrecall";
				Db.NonQ(command);
				command=@"CREATE TABLE webschedrecall (
						WebSchedRecallNum bigint NOT NULL auto_increment PRIMARY KEY,
						ClinicNum bigint NOT NULL,
						PatNum bigint NOT NULL,
						RecallNum bigint NOT NULL,
						DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						DateDue date NOT NULL DEFAULT '0001-01-01',
						ReminderCount int NOT NULL,
						PreferRecallMethod tinyint NOT NULL,
						DateTimeReminderSent datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						DateTimeSendFailed datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						EmailSendStatus tinyint NOT NULL,
						SmsSendStatus tinyint NOT NULL,
						PhonePat varchar(255) NOT NULL,
						EmailPat varchar(255) NOT NULL,
						MsgTextToMobileTemplate text NOT NULL,
						MsgTextToMobile text NOT NULL,
						EmailSubjTemplate text NOT NULL,
						EmailSubj text NOT NULL,
						EmailTextTemplate text NOT NULL,
						EmailText text NOT NULL,
						GuidMessageToMobile text NOT NULL,
						ShortGUIDSms varchar(255) NOT NULL,
						ShortGUIDEmail varchar(255) NOT NULL,
						ResponseDescript text NOT NULL,
						Source tinyint NOT NULL,
						INDEX(ClinicNum),
						INDEX(PatNum),
						INDEX(RecallNum),
						INDEX(DateTimeReminderSent)
						) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE webschedrecall'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE webschedrecall (
						WebSchedRecallNum number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						RecallNum number(20) NOT NULL,
						DateTimeEntry date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateDue date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						ReminderCount number(11) NOT NULL,
						PreferRecallMethod number(3) NOT NULL,
						DateTimeReminderSent date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateTimeSendFailed date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						EmailSendStatus number(3) NOT NULL,
						SmsSendStatus number(3) NOT NULL,
						PhonePat varchar2(255),
						EmailPat varchar2(255),
						MsgTextToMobileTemplate clob,
						MsgTextToMobile clob,
						EmailSubjTemplate clob,
						EmailSubj clob,
						EmailTextTemplate clob,
						EmailText clob,
						GuidMessageToMobile clob,
						ShortGUIDSms varchar2(255),
						ShortGUIDEmail varchar2(255),
						ResponseDescript clob,
						Source number(3) NOT NULL,
						CONSTRAINT webschedrecall_WebSchedRecallN PRIMARY KEY (WebSchedRecallNum)
						)";
				Db.NonQ(command);
				command=@"CREATE INDEX webschedrecall_ClinicNum ON webschedrecall (ClinicNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX webschedrecall_PatNum ON webschedrecall (PatNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX webschedrecall_RecallNum ON webschedrecall (RecallNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX webschedrecall_DateTimeRemind ON webschedrecall (DateTimeReminderSent)";
				Db.NonQ(command);
			}
			//ProcCode Override Preference
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ClaimPrintProcChartedDesc','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES "
					+"((SELECT MAX(PrefNum)+1 FROM preference),'ClaimPrintProcChartedDesc','0')";
				Db.NonQ(command);
			}
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=110";//InsPlanEdit
			table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",139)";//InsPlanOrthoEdit
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
						+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",139)";//InsPlanOrthoEdit
					Db.NonQ(command);
				}
			}
			//RecurringChargesUseTransDate 
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES ('RecurringChargesUseTransDate','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES "
					+"((SELECT MAX(PrefNum)+1 FROM preference),'RecurringChargesUseTransDate','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ProcFeeUpdatePrompt','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'ProcFeeUpdatePrompt','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS claimtracking";
				Db.NonQ(command);
				command=@"CREATE TABLE claimtracking (
					ClaimTrackingNum bigint NOT NULL auto_increment PRIMARY KEY,
					ClaimNum bigint NOT NULL,
					TrackingType varchar(255) NOT NULL,
					UserNum bigint NOT NULL,
					DateTimeEntry timestamp,
					Note text NOT NULL,
					TrackingDefNum bigint NOT NULL,
					INDEX(ClaimNum),
					INDEX(UserNum),
					INDEX(TrackingDefNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE claimtracking'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE claimtracking (
					ClaimTrackingNum number(20) NOT NULL,
					ClaimNum number(20) NOT NULL,
					TrackingType varchar2(255),
					UserNum number(20) NOT NULL,
					DateTimeEntry timestamp,
					Note clob,
					TrackingDefNum number(20) NOT NULL,
					CONSTRAINT claimtracking_ClaimTrackingNum PRIMARY KEY (ClaimTrackingNum)
					)";
				Db.NonQ(command);
				command=@"CREATE INDEX claimtracking_ClaimNum ON claimtracking (ClaimNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX claimtracking_UserNum ON claimtracking (UserNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX claimtracking_TrackingDefNum ON claimtracking (TrackingDefNum)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ClaimTrackingRequiresError','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES "
					+"((SELECT MAX(PrefNum)+1 FROM preference),'ClaimTrackingRequiresError','0')";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.1.1 - securitylog | DefNumError column");//No translation in convert script.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE securitylog ADD DefNumError bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE securitylog ADD INDEX (DefNumError)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE securitylog ADD DefNumError number(20)";
				Db.NonQ(command);
				command="UPDATE securitylog SET DefNumError = 0 WHERE DefNumError IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE securitylog MODIFY DefNumError NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX securitylog_DefNumError ON securitylog (DefNumError)";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.1.1");//No translation in convert script.
			#region Web Sched Automated Texting
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedAggregatedTextMessage',"
					+"'Dental checkups due: [FamilyListURLs].\r\nVisit links to schedule appointments or call [OfficePhone].')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),"
					+"'WebSchedAggregatedTextMessage','Dental checkups due: [FamilyListURLs].\r\nVisit links to schedule appointments or call [OfficePhone].')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedAggregatedEmailBody',"
					+"'These family members are due for a dental checkup: \r\n[FamilyListURLs]\r\nPlease visit the links above or "
					+"call our office today at [OfficePhone] to schedule your appointment.')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),"
					+"'WebSchedAggregatedEmailBody','These family members are due for a dental checkup: \r\n[FamilyListURLs]\r\nPlease visit the links above "
					+"or call our office today at [OfficePhone] to schedule your appointment.')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedAggregatedEmailSubject','Dental Care Reminder')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),"
					+"'WebSchedAggregatedEmailSubject','Dental Care Reminder')";
				Db.NonQ(command);
			}
			string textMessageTemplate="Dental checkup due for [NameF].\r\nVisit [URL] to schedule appointments or call [PracticePhone].";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedMessageText','"+textMessageTemplate+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),"
					+"'WebSchedMessageText','"+textMessageTemplate+".')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedMessageText2','"+textMessageTemplate+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),"
					+"'WebSchedMessageText2','"+textMessageTemplate+"')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedMessageText3','"+textMessageTemplate+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),"
					+"'WebSchedMessageText3','"+textMessageTemplate+"')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE alertitem ADD FormToOpen tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE alertitem ADD FormToOpen number(3)";
				Db.NonQ(command);
				command="UPDATE alertitem SET FormToOpen = 0 WHERE FormToOpen IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE alertitem MODIFY FormToOpen NOT NULL";
				Db.NonQ(command);
			}
			command="UPDATE alertitem SET FormToOpen = 2";//The only possible alert type that might exist in the database is for pending payments.
			Db.NonQ(command);
			//Create an alert for offices that are using web sched telling them that they can use texting for automated recall.
			bool isUsingWebSched=false;
			command="SELECT ValueString FROM preference WHERE PrefName='WebSchedService'";
			table=Db.GetTable(command);
			if(table.Rows.Count > 0 && PIn.Bool(table.Rows[0][0].ToString())) {
				isUsingWebSched=true;
			}
			if(isUsingWebSched) {
				string alertDescript=@"Web Sched can now automatically send text messages to remind patients to schedule their recall appointments online.
Go to eServices -> Web Sched to activate.";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO alertitem (Type,Actions,Severity,ClinicNum,Description,FormToOpen) VALUES("
						+"0,"//AlertType.Generic
						+"7,"//ActionType.Delete|ActionType.MarkAsRead|ActionType.OpenForm
						+"1,"//SeverityType.Low
						+"0,"//ClinicNum
						+"'"+alertDescript+"',"
						+"1)";//FormType.FormEServicesWebSchedRecall
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO alertitem (AlertItemNum,Type,Actions,Severity,ClinicNum,Description,FormToOpen) VALUES("+
						"(SELECT MAX(AlertItemNum)+1 FROM alertitem),"
						+"0,"//AlertType.Generic
						+"7,"//ActionType.Delete|ActionType.MarkAsRead|ActionType.OpenForm
						+"1,"//SeverityType.Low
						+"0,"//ClinicNum
						+"'"+alertDescript+"',"
						+"1)";//FormType.FormEServicesWebSchedRecall
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedSendThreadFrequency','7')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),"
					+"'WebSchedSendThreadFrequency','7')";
				Db.NonQ(command);
			}
			if(isUsingWebSched) {
				command=@"SELECT MAX(ItemOrder) MaxItemOrder
					FROM displayfield
					WHERE Category=4";//DisplayFieldCategory.RecallList
				int itemOrder=Db.GetInt(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO displayfield (InternalName,Description,ItemOrder,ColumnWidth,Category) VALUES("
						+"'WebSched',"
						+"'Web Sched',"
						+POut.Int(itemOrder+1)+","
						+"100,"
						+"4)";//DisplayFieldCategory.RecallList
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO displayfield (DisplayFieldNum,InternalName,Description,ItemOrder,ColumnWidth,Category) VALUES("+
						"(SELECT MAX(DisplayFieldNum)+1 FROM displayfield),"
						+"'WebSched',"
						+"'Web Sched',"
						+POut.Int(itemOrder+1)+","
						+"100,"
						+"4)";//DisplayFieldCategory.RecallList
					Db.NonQ(command);
				}
			}
			#endregion Web Sched Automated Texting
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.1.1 - Aging List | statement Index");//No translation in convert script.
			try {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					if(!LargeTableHelper.IndexExists("statement","SuperFamily,Mode_,DateSent")) {//add composite index if it doesn't already exist
						command="ALTER TABLE statement ADD INDEX SuperFamModeDateSent (SuperFamily,Mode_,DateSent)";
						Db.NonQ(command);
						if(LargeTableHelper.IndexExists("statement","SuperFamily")) {//drop redundant index once composite index is successfully added and only if it exists
							command="ALTER TABLE statement DROP INDEX SuperFamily";
							Db.NonQ(command);
						}
					}
				}
				else {//oracle
					command="CREATE INDEX statement_SFamModeDateSent ON statement (SuperFamily,Mode_,DateSent)";//add composite index
					Db.NonQ(command);
					command="DROP INDEX statement_SuperFamily";//drop redundant index once composite index is successfully added
					Db.NonQ(command);
				}
			}
			catch(Exception) { }//Only an index.
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.1.1");//No translation in convert script.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE alertitem ADD FKey bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE alertitem ADD INDEX (FKey)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE alertitem ADD FKey number(20)";
				Db.NonQ(command);
				command="UPDATE alertitem SET FKey = 0 WHERE FKey IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE alertitem MODIFY FKey NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX alertitem_FKey ON alertitem (FKey)";
				Db.NonQ(command);
			}
			//Force all users to TP Use Sheets.  For now 17.1 we're just updating pref, for 17.2 we'll remove the pref from code.
			command="UPDATE preference SET ValueString='1' WHERE PrefName='TreatPlanUseSheets'";
			Db.NonQ(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE clearinghouse ADD IsEraDownloadAllowed tinyint NOT NULL DEFAULT 1";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE clearinghouse ADD IsEraDownloadAllowed number(3) DEFAULT 1";
				Db.NonQ(command);
				command="UPDATE clearinghouse SET IsEraDownloadAllowed = 1 WHERE IsEraDownloadAllowed IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE clearinghouse MODIFY IsEraDownloadAllowed NOT NULL";
				Db.NonQ(command);
			}
			string alertDescriptStr=@"eServices can now be activated online.  Go to eServices -> Signup to activate.";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertitem (Type,Actions,Severity,ClinicNum,Description,FormToOpen) VALUES("
					+"0,"//AlertType.Generic
					+"7,"//ActionType.Delete|ActionType.MarkAsRead|ActionType.OpenForm
					+"1,"//SeverityType.Low
					+"0,"//ClinicNum
					+"'"+alertDescriptStr+"',"
					+"4)";//4 - FormType.FormEServicesSetup.
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertitem (AlertItemNum,Type,Actions,Severity,ClinicNum,Description,FormToOpen,FKey) VALUES("+
					"(SELECT COALESCE(MAX(AlertItemNum),0)+1 FROM alertitem),"
					+"0,"//AlertType.Generic
					+"7,"//ActionType.Delete|ActionType.MarkAsRead|ActionType.OpenForm
					+"1,"//SeverityType.Low
					+"0,"//ClinicNum
					+"'"+alertDescriptStr+"',"
					+"4"//4 - FormType.FormEServicesSetup.
					+",0)";
				Db.NonQ(command);
			}
		}

		private static void To17_1_3() {
			string command;
			//Add ProcProvChangesClaimProcWithClaim pref if doesn't exist.  Default to on.
			//This pref was backported to 16.4 so it might exist in DB already.
			command="SELECT COUNT(PrefName) FROM preference WHERE PrefName='ProcProvChangesClaimProcWithClaim'";
			if(PIn.Int(Db.GetCount(command))==0) {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ProcProvChangesClaimProcWithClaim','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
						+"'ProcProvChangesClaimProcWithClaim','1')";
					Db.NonQ(command);
				}
			}
		}

		private static void To17_1_7() {
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptConfirmExcludeERemind','')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
					+"'ApptConfirmExcludeERemind','')";
				Db.NonQ(command);
			}
		}

		private static void To17_1_8() {
			string command;
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.1.8 - Recall Sync | procedurelog composite index");
			try {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					if(!LargeTableHelper.IndexExists("procedurelog","PatNum,ProcStatus,CodeNum,ProcDate")) {//add composite index if it doesn't already exist
						command="ALTER TABLE procedurelog ADD INDEX PatStatusCodeDate (PatNum,ProcStatus,CodeNum,ProcDate)";
						Db.NonQ(command);
						if(LargeTableHelper.IndexExists("procedurelog","PatNum")) {//drop redundant index once composite index is successfully added and only if it exists
							command="ALTER TABLE procedurelog DROP INDEX indexPatNum";
							Db.NonQ(command);
						}
					}
				}
				else {//oracle
					command="CREATE INDEX procedurelog_PatStatusCodeDate ON procedurelog (PatNum,ProcStatus,CodeNum,ProcDate)";//add composite index
					Db.NonQ(command);
					try {
						command="DROP INDEX procedurelog_indexPatNum";//drop redundant index once composite index is successfully added
						Db.NonQ(command);
					}
					catch(Exception ex) {
						ex.DoNothing();
					}
				}
			}
			catch(Exception) { }//Only an index.
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.1.8 - Recall Sync | appointment composite index");
			try {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					if(!LargeTableHelper.IndexExists("appointment","AptStatus,AptDateTime")) {//add composite index if it doesn't already exist
						command="ALTER TABLE appointment ADD INDEX StatusDate (AptStatus,AptDateTime)";
						Db.NonQ(command);
						if(LargeTableHelper.IndexExists("appointment","AptStatus")) {//drop redundant index once composite index is successfully added and only if it exists
							command="ALTER TABLE appointment DROP INDEX AptStatus";
							Db.NonQ(command);
						}
					}
				}
				else {//oracle
					command="CREATE INDEX appointment_StatusDate ON appointment (AptStatus,AptDateTime)";//add composite index
					Db.NonQ(command);
					command="DROP INDEX appointment_AptStatus";//drop redundant index once composite index is successfully added
					Db.NonQ(command);
				}
			}
			catch(Exception) { }//Only an index.
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.1.8");//No translation in convert script.
		}

		private static void To17_1_10() {
			string command="SELECT ValueString FROM preference WHERE PrefName='RecallStatusTexted'";
			if(Db.GetScalar(command)=="0") {//They have not set the status yet.
				long defNum=0;
				//Check to see if they already have a 'Texted' status. If they do, use that definition, otherwise insert a new definition.
				command="SELECT DefNum FROM definition WHERE Category=13 "//DefCat.RecallUnschedStatus
					+"AND LOWER(ItemName)='texted' AND IsHidden=0";
				DataTable table=Db.GetTable(command);
				if(table.Rows.Count > 0) {
					defNum=PIn.Long(table.Rows[0][0].ToString());
				}
				if(defNum==0) {//We didn't find a definition named 'Texted'.
					//Insert new status for 'Texted'
					command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=13";
					string maxOrder=Db.GetScalar(command);
					if(maxOrder=="") {
						maxOrder="0";
					}
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO definition (Category, ItemOrder, ItemName) VALUES (13,"+maxOrder+",'Texted')";
						defNum=Db.NonQ(command,true);
					}
					else {//oracle
						command="INSERT INTO definition (DefNum,Category, ItemOrder, ItemName) VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),13,"+maxOrder
							+",'Texted')";
						defNum=Db.NonQ(command,true,"DefNum","definition");
					}
				}
				command="UPDATE preference SET ValueString='"+POut.Long(defNum)+"' WHERE PrefName='RecallStatusTexted'";
				Db.NonQ(command);
			}
			command="SELECT ValueString FROM preference WHERE PrefName='RecallStatusEmailedTexted'";
			if(Db.GetScalar(command)=="0") {//They have not set the status yet.
				//Insert new status for 'Texted/Emailed'
				long defNum;
				command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=13";
				string maxOrder=Db.GetScalar(command);
				if(maxOrder=="") {
					maxOrder="0";
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO definition (Category, ItemOrder, ItemName) VALUES (13,"+maxOrder+",'Texted/Emailed')";
					defNum=Db.NonQ(command,true);
				}
				else {//oracle
					command="INSERT INTO definition (DefNum,Category, ItemOrder, ItemName) VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),13,"+maxOrder
						+",'Texted/Emailed')";
					defNum=Db.NonQ(command,true,"DefNum","definition");
				}
				command="UPDATE preference SET ValueString='"+POut.Long(defNum)+"' WHERE PrefName='RecallStatusEmailedTexted'";
				Db.NonQ(command);
			}
		}

		private static void To17_1_16() {
			string command;
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.1.16 - Global Update Fees | patient FeeSched index");
			try {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					if(!LargeTableHelper.IndexExists("patient","FeeSched")) {//add index if it doesn't already exist
						command="ALTER TABLE patient ADD INDEX FeeSched (FeeSched)";
						Db.NonQ(command);
					}
				}
				else {//oracle
					command="CREATE INDEX patient_FeeSched ON patient (FeeSched)";
					Db.NonQ(command);
				}
			}
			catch(Exception) { }//Only an index.
		}

		private static void To17_1_17() {
			string command;
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.1.17 - Updating payment table");
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE payment ADD RecurringChargeDate date NOT NULL DEFAULT '0001-01-01'";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE payment ADD RecurringChargeDate date";
				Db.NonQ(command);
				command="UPDATE payment SET RecurringChargeDate = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE RecurringChargeDate IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE payment MODIFY RecurringChargeDate NOT NULL";
				Db.NonQ(command);
			}
		}

		private static void To17_1_18() {
			string command;
			//Insert PracticeByNumbers bridge-----------------------------------------------------------------
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'PracticeByNumbers', "
					+"'Practice by Numbers from www.practicenumbers.com', "
					+"'0', "
					+"'', "
					+"'', "
					+"'')";
				long programNum=Db.NonQ(command,true);
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
					+"'PracticeByNumbers', "
					+"'Practice by Numbers from www.practicenumbers.com', "
					+"'0', "
					+"'', "
					+"'', "
					+"'')";
				long programNum=Db.NonQ(command,true,"ProgramNum","program");
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+"'"+POut.Long(programNum)+"', "
					+"'Disable Advertising', "
					+"'0', "
					+"'0')";
				Db.NonQ(command);
			}//end PracticeByNumbers bridge
		}

		private static void To17_1_19() {
			string command;
			command="UPDATE claim SET DateSentOrig=DateSent WHERE DateSentOrig='0001-01-01' AND DateSent!='0001-01-01' AND ClaimStatus IN ('S','R')";
			Db.NonQ(command);
		}

		private static void To17_1_22() {
			string command;
			//Insert NewTom bridge-----------------------------------------------------------------
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					 +") VALUES("
					 +"'NewTomNNT', "
					 +"'NewTom NNT from www.newtom.it', "
					 +"'0', "
					 +"'"+POut.String(@"C:\NNT\NNTBridge.exe")+"', "
					 +"'"+POut.String(@"")+"', "//leave blank if none
					 +"'')";
				long programNum = Db.NonQ(command,true);
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
					 +"'NewTomNNT')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					 +") VALUES("
					+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
					 +"'NewTomNNT', "
					 +"'NewTom NNT from www.newtom.it', "
					 +"'0', "
					 +"'"+POut.String(@"C:\NNT\NNTBridge.exe")+"', "
					 +"'"+POut.String(@"")+"', "//leave blank if none
					 +"'')";
				long programNum = Db.NonQ(command,true,"ProgramNum","program");
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					 +") VALUES("
					 +"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					 +"'"+POut.Long(programNum)+"', "
					 +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					 +"'0', "
					 +"'0')";
				Db.NonQ(command);
				command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
					 +"VALUES ("
					 +"(SELECT COALESCE(MAX(ToolButItemNum),0)+1 FROM toolbutitem),"
					 +"'"+POut.Long(programNum)+"', "
					 +"'2', "//ToolBarsAvail.ChartModule
					 +"'NewTomNNT')";
				Db.NonQ(command);
			}//end NewTom bridge
			//Insert i-Dixel bridge-----------------------------------------------------------------
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					 +") VALUES("
					 +"'iDixel', "
					 +"'i-Dixel from http://www.jmoritaeurope.de/en/products/diagnostic-and-imaging-equipment/imaging-software/i-dixel/', "
					 +"'0', "
					 +"'"+POut.String(@"C:\Program Files\JMorita\ToIView\ToiViewLauncher.bat")+"', "
					 +"'"+POut.String(@"")+"', "//leave blank if none
					 +"'')";
				long programNum = Db.NonQ(command,true);
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
					 +"'i-Dixel')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					 +") VALUES("
					+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
					 +"'iDixel', "
					 +"'i-Dixel from http://www.jmoritaeurope.de/en/products/diagnostic-and-imaging-equipment/imaging-softwa', "
					 +"'0', "
					 +"'"+POut.String(@"C:\Program Files\JMorita\ToIView\ToiViewLauncher.bat")+"', "
					 +"'"+POut.String(@"")+"', "//leave blank if none
					 +"'')";
				long programNum = Db.NonQ(command,true,"ProgramNum","program");
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					 +") VALUES("
					 +"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					 +"'"+POut.Long(programNum)+"', "
					 +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					 +"'0', "
					 +"'0')";
				Db.NonQ(command);
				command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
					 +"VALUES ("
					 +"(SELECT COALESCE(MAX(ToolButItemNum),0)+1 FROM toolbutitem),"
					 +"'"+POut.Long(programNum)+"', "
					 +"'2', "//ToolBarsAvail.ChartModule
					 +"'i-Dixel')";
				Db.NonQ(command);
			}//end iDixel bridge
			//ADSTRA Imaging bridge-----------------------------------------------------------------
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					 +") VALUES("
					 +"'Adstra', "
					 +"'ADSTRA Imaging from http://adstra.com/adstra-dental-software/', "
					 +"'0', "
					 +"'"+POut.String(@"C:/Program Files/ADSTRA/adstradde.exe")+"', "
					 +"'"+POut.String(@"")+"', "//leave blank if none
					 +"'')";
				long programNum = Db.NonQ(command,true);
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
					 +"'ADSTRA')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					 +") VALUES("
					+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
					 +"'Adstra', "
					 +"'ADSTRA Imaging from http://adstra.com/adstra-dental-software/', "
					 +"'0', "
					 +"'"+POut.String(@"C:/Program Files/ADSTRA/adstradde.exe")+"', "
					 +"'"+POut.String(@"")+"', "//leave blank if none
					 +"'')";
				long programNum = Db.NonQ(command,true,"ProgramNum","program");
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+"'"+POut.Long(programNum)+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0', "
					+"'0')";
				Db.NonQ(command);
				command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
					 +"VALUES ("
					 +"(SELECT COALESCE(MAX(ToolButItemNum),0)+1 FROM toolbutitem),"
					 +"'"+POut.Long(programNum)+"', "
					 +"'2', "//ToolBarsAvail.ChartModule
					 +"'ADSTRA')";
				Db.NonQ(command);
			}//end ADSTRA Imaging bridge
			//Remove the . characters from the WebSchedAggregatedTextMessage preference. Clicking a link with a period will not get recognized. 
			command="UPDATE preference SET ValueString=REPLACE(ValueString,'[FamilyListURLs].','[FamilyListURLs]') WHERE PrefName='WebSchedAggregatedTextMessage'";
			Db.NonQ(command);
		}

		private static void To17_2_1() {
			string command;
			//For the next 3 database columns added, type is "text", because text is dynamically resized.
			//This means that if the column is empty then it will not take any extra space, other than header data.
			//Since these 3 database columns are HQ only, we do not want to bloat the table for our clients.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE repeatcharge ADD Npi text NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE repeatcharge ADD Npi clob";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE repeatcharge ADD ErxAccountId text NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE repeatcharge ADD ErxAccountId clob";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE repeatcharge ADD ProviderName text NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE repeatcharge ADD ProviderName clob";
				Db.NonQ(command);
			}
			DataTable table;
			command="SELECT ValueString FROM preference WHERE preference.PrefName = 'DistributorKey'";
			if(PIn.Bool(Db.GetScalar(command))) {
				command="SELECT * FROM repeatcharge WHERE repeatcharge.Note LIKE '%NPI%' OR Note LIKE '%ErxAccountId%'";
				table=Db.GetTable(command);
				foreach(DataRow row in table.Rows) {
					string strNoteOld=row["Note"].ToString();
					string strNPI="";
					string strAccountID="";
					string strNoteNew="";
					Match m=Regex.Match(strNoteOld,"^NPI=([0-9]{10})(  ErxAccountId=([0-9]+\\-[a-zA-Z0-9]{5}))?(\r\n)?",RegexOptions.IgnoreCase);
					if(m.Success) {
						strNPI=m.Result("$1");
						strAccountID=m.Result("$3");
						strNoteNew=strNoteOld.Substring(m.Length);//We use m.length so we can additionally skip the newline if present.
						command="UPDATE repeatcharge SET repeatcharge.Npi='"+POut.String(strNPI)+"',repeatcharge.ErxAccountId='"+POut.String(strAccountID)
							+"',repeatCharge.Note='"+POut.String(strNoteNew)+"' WHERE repeatcharge.RepeatChargeNum="+row["RepeatChargeNum"];
						Db.NonQ(command);
					}
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE providererx ADD IsEpcs tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE providererx ADD IsEpcs number(3)";
				Db.NonQ(command);
				command="UPDATE providererx SET IsEpcs = 0 WHERE IsEpcs IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE providererx MODIFY IsEpcs NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE tasksubscription ADD TaskNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE tasksubscription ADD INDEX (TaskNum)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE tasksubscription ADD TaskNum number(20)";
				Db.NonQ(command);
				command="UPDATE tasksubscription SET TaskNum = 0 WHERE TaskNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE tasksubscription MODIFY TaskNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX tasksubscription_TaskNum ON tasksubscription (TaskNum)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE claimtracking ADD TrackingErrorDefNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE claimtracking ADD INDEX (TrackingErrorDefNum)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE claimtracking ADD TrackingErrorDefNum number(20)";
				Db.NonQ(command);
				command="UPDATE claimtracking SET TrackingErrorDefNum = 0 WHERE TrackingErrorDefNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE claimtracking MODIFY TrackingErrorDefNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX claimtracking_TrackingErrorNum ON claimtracking (TrackingErrorDefNum)";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.2.1 - Claim Tracking");
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO claimtracking (ClaimNum,TrackingType,UserNum,DateTimeEntry,Note,TrackingDefNum,TrackingErrorDefNum) "
					+"SELECT FKey,'StatusHistory',UserNum,LogDateTime,LogText,DefNum,DefNumError "
					+"FROM securitylog "
					+"WHERE PermType=95";//PermType 95 is ClaimHistoryEdit
					Db.NonQ(command);
				}
				else {//oracle
				command="SELECT * FROM securitylog WHERE PermType=95";//PermType 95 is ClaimHistoryEdit
				DataTable tableCustomTrackingLogs=Db.GetTable(command);
				foreach(DataRow row in tableCustomTrackingLogs.Rows) {
					command="INSERT INTO claimtracking(ClaimTrackingNum,ClaimNum,TrackingType,UserNum,DateTimeEntry,Note,TrackingDefNum,TrackingErrorDefNum) "
						+"VALUES("
							+"(SELECT COALESCE(MAX(ClaimTrackingNum),0)+1 FROM claimtracking)"
							+","+POut.Long(PIn.Long(row["FKey"].ToString()))//ClaimNum
							+",'StatusHistory'"//TrackingType
							+","+POut.Long(PIn.Long(row["UserNum"].ToString()))//UserNum
							+","+POut.DateT(PIn.DateT(row["LogDateTime"].ToString()))//DateTimeEntry
							+",'"+POut.String(row["LogText"].ToString())+"'"//Note
							+","+POut.Long(PIn.Long(row["DefNum"].ToString()))//TrackingDefNum
							+","+POut.Long(PIn.Long(row["DefNumError"].ToString()))//TrackingErrorDefNum
						+")";
					Db.NonQ(command);
				}
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.2.1");//No translation in convert script.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE grouppermission ADD FKey bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE grouppermission ADD INDEX (FKey)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE grouppermission ADD FKey number(20)";
				Db.NonQ(command);
				command="UPDATE grouppermission SET FKey = 0 WHERE FKey IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE grouppermission MODIFY FKey NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX grouppermission_FKey ON grouppermission (FKey)";
				Db.NonQ(command);
			}
			DataTable userGroupTable=Db.GetTable("SELECT * FROM usergroup");
			DataTable reportTable=Db.GetTable("SELECT * FROM displayreport");
			foreach(DataRow row in userGroupTable.Rows) {
				//See if this UserGroup has ProdInc or DailyReport permissions
				string permissionReports=Db.GetCount("SELECT COUNT(*) FROM grouppermission WHERE PermType=22 AND UserGroupNum="+row["UserGroupNum"].ToString());
				if(permissionReports=="0") {
					continue;//This group doesn't have Reports permission.  Don't add any individual report permissions.
				}
				string permissionProdInc=Db.GetCount("SELECT COUNT(*) FROM grouppermission WHERE PermType=39 AND UserGroupNum="+row["UserGroupNum"].ToString());
				string permissionDailyReport=Db.GetCount("SELECT COUNT(*) FROM grouppermission WHERE PermType=133 AND UserGroupNum="+row["UserGroupNum"].ToString());
				foreach(DataRow reportRow in reportTable.Rows) {
					if(reportRow["IsHidden"].ToString()=="1") {
						continue;
					}
					if(permissionProdInc=="0" && (reportRow["InternalName"].ToString()=="ODToday" //They don't have ProdInc permission.  Don't add perms for those reports.
						|| reportRow["InternalName"].ToString()=="ODYesterday"
						|| reportRow["InternalName"].ToString()=="ODThisMonth"
						|| reportRow["InternalName"].ToString()=="ODLastMonth"
						|| reportRow["InternalName"].ToString()=="ODThisYear"
						|| reportRow["InternalName"].ToString()=="ODMoreOptions"
						|| reportRow["InternalName"].ToString()=="ODProviderPayrollSummary"
						|| reportRow["InternalName"].ToString()=="ODProviderPayrollDetailed"))
					{
						continue;
					}
					if(permissionDailyReport=="0" && (reportRow["InternalName"].ToString()=="ODAdjustments" //They don't have DailyReport permission.  Don't add perms for those reports.
						|| reportRow["InternalName"].ToString()=="ODPayments"
						|| reportRow["InternalName"].ToString()=="ODProcedures"
						|| reportRow["InternalName"].ToString()=="ODWriteoffs"))
					{
						continue;
					}
					//The report isn't hidden and it isn't restricted based on ProdInc or DailyReport perms.  Add a GroupPermission for it.
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) " //Insert Reports permission (22) for the particular report.
							+"VALUES('0001-01-01',0,"+row["UserGroupNum"].ToString()+",22,"+reportRow["DisplayReportNum"].ToString()+")";
						Db.NonQ(command);
					}
					else {//oracle
						command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType,FKey) " //Insert Reports permission (22) for the particular report.
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission)"
							+","+"TO_DATE('0001-01-01','YYYY-MM-DD'),0,"+row["UserGroupNum"].ToString()+",22,"+reportRow["DisplayReportNum"].ToString()+")";
						Db.NonQ(command);
					}
				}
			}
			//Insert OrthoCAD bridge-----------------------------------------------------------------
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'OrthoCAD', "
					+"'OrthoCAD from www.itero.com/', "
					+"'0', "
					+"'"+POut.String(@"C:\Program Files\Cadent\OrthoCAD\OrthoCAD.exe")+"', "
					+"'', "//leave blank if none
					+"'')";
				long programNum=Db.NonQ(command,true);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(programNum)+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
					+"'OrthoCAD', "
					+"'OrthoCAD from www.itero.com/', "
					+"'0', "
					+"'"+POut.String(@"C:\Program Files\Cadent\OrthoCAD\OrthoCAD.exe")+"', "
					+"'', "//leave blank if none
					+"'')";
				long programNum=Db.NonQ(command,true,"ProgramNum","program");
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
					+"'"+POut.Long(programNum)+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0', "
					+"'0')";
				Db.NonQ(command);
			}//end OrthoCAD bridge
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE signalod ADD RemoteRole tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE signalod ADD RemoteRole number(3)";
				Db.NonQ(command);
				command="UPDATE signalod SET RemoteRole = 0 WHERE RemoteRole IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE signalod MODIFY RemoteRole NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('PatInitBillingTypeFromPriInsPlan','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
					+"'PatInitBillingTypeFromPriInsPlan','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE insplan ADD BillingType bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE insplan ADD INDEX (BillingType)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE insplan ADD BillingType number(20)";
				Db.NonQ(command);
				command="UPDATE insplan SET BillingType = 0 WHERE BillingType IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE insplan MODIFY BillingType NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX insplan_BillingType ON insplan (BillingType)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('FormClickDelay','0.2')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'FormClickDelay','0.2')";
				Db.NonQ(command);
			}
			//Add InsPlanMerge permission for users that have InsPlanEdit permission------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=110 GROUP BY UserGroupNum";
			DataTable groupPermTable=Db.GetTable(command);
			long groupNum;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in groupPermTable.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",141)";//InsPlanMerge
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in groupPermTable.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",141,0)";//InsPlanMerge
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE carrier ADD CarrierGroupName varchar(255) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE carrier ADD CarrierGroupName varchar2(255)";
				Db.NonQ(command);
			}		
			//Invoice Payments Grid Show WriteOffs
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('InvoicePaymentsGridShowNetProd','1')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
					+"'InvoicePaymentsGridShowNetProd','1')";
				Db.NonQ(command);
			}
			//Add InsCarrierCombine permission to everyone------------------------------------------------------
			command="SELECT UserGroupNum FROM grouppermission GROUP BY UserGroupNum";
			groupPermTable=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in groupPermTable.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",142)";//InsCarrierCombine
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in groupPermTable.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",142,0)";//InsCarrierCombine
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE erxlog ADD UserNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE erxlog ADD INDEX (UserNum)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE erxlog ADD UserNum number(20)";
				Db.NonQ(command);
				command="UPDATE erxlog SET UserNum = 0 WHERE UserNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE erxlog MODIFY UserNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX erxlog_UserNum ON erxlog (UserNum)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE labcase ADD DateTStamp timestamp";
				Db.NonQ(command);
				command="UPDATE labcase SET DateTStamp = NOW()";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE labcase ADD DateTStamp timestamp";
				Db.NonQ(command);
				command="UPDATE labcase SET DateTStamp = SYSTIMESTAMP";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE etrans ADD UserNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE etrans ADD INDEX (UserNum)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE etrans ADD UserNum number(20)";
				Db.NonQ(command);
				command="UPDATE etrans SET UserNum = 0 WHERE UserNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE etrans MODIFY UserNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX etrans_UserNum ON etrans (UserNum)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE proccodenote ADD ProcStatus tinyint NOT NULL";
				Db.NonQ(command);
				command="UPDATE proccodenote SET ProcStatus = 2";// ProcStat.Complete
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE proccodenote ADD ProcStatus number(3)";
				Db.NonQ(command);
				command="UPDATE proccodenote SET ProcStatus = 2 WHERE ProcStatus IS NULL";// ProcStat.Complete
				Db.NonQ(command);
				command="ALTER TABLE proccodenote MODIFY ProcStatus NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE procedurecode ADD DefaultTPNote text NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE procedurecode ADD DefaultTPNote varchar2(4000)";
				Db.NonQ(command);
			}
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(groupNum)+",143)"; //PopupEdit
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
						 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",143,0)"; //PopupEdit
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimReportReceivedByService','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
					+"'ClaimReportReceivedByService','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('TitleBarClinicUseAbbr','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'TitleBarClinicUseAbbr','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS histappointment";
				Db.NonQ(command);
				command=@"CREATE TABLE histappointment (
						HistApptNum bigint NOT NULL auto_increment PRIMARY KEY,
						HistUserNum bigint NOT NULL,
						HistDateTStamp datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						HistApptAction tinyint NOT NULL,
						ApptSource tinyint NOT NULL,
						AptNum bigint NOT NULL,
						PatNum bigint NOT NULL,
						AptStatus tinyint NOT NULL,
						Pattern varchar(255) NOT NULL,
						Confirmed bigint NOT NULL,
						TimeLocked tinyint NOT NULL,
						Op bigint NOT NULL,
						Note varchar(255) NOT NULL,
						ProvNum bigint NOT NULL,
						ProvHyg bigint NOT NULL,
						AptDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						NextAptNum bigint NOT NULL,
						UnschedStatus bigint NOT NULL,
						IsNewPatient tinyint NOT NULL,
						ProcDescript varchar(255) NOT NULL,
						Assistant bigint NOT NULL,
						ClinicNum bigint NOT NULL,
						IsHygiene tinyint NOT NULL,
						DateTStamp timestamp,
						DateTimeArrived datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						DateTimeSeated datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						DateTimeDismissed datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						InsPlan1 bigint NOT NULL,
						InsPlan2 bigint NOT NULL,
						DateTimeAskedToArrive datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						ProcsColored varchar(255) NOT NULL,
						ColorOverride int NOT NULL,
						AppointmentTypeNum bigint NOT NULL,
						SecUserNumEntry bigint NOT NULL,
						SecDateEntry date NOT NULL DEFAULT '0001-01-01',
						INDEX(HistUserNum),
						INDEX(AptNum),
						INDEX(PatNum),
						INDEX(Confirmed),
						INDEX(Op),
						INDEX(ProvNum),
						INDEX(ProvHyg),
						INDEX(NextAptNum),
						INDEX(UnschedStatus),
						INDEX(Assistant),
						INDEX(ClinicNum),
						INDEX(InsPlan1),
						INDEX(InsPlan2),
						INDEX(AppointmentTypeNum),
						INDEX(SecUserNumEntry)
						) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE histappointment'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE histappointment (
						HistApptNum number(20) NOT NULL,
						HistUserNum number(20) NOT NULL,
						HistDateTStamp date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						HistApptAction number(3) NOT NULL,
						ApptSource number(3) NOT NULL,
						AptNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						AptStatus number(3) NOT NULL,
						Pattern varchar2(255),
						Confirmed number(20) NOT NULL,
						TimeLocked number(3) NOT NULL,
						Op number(20) NOT NULL,
						Note varchar2(255),
						ProvNum number(20) NOT NULL,
						ProvHyg number(20) NOT NULL,
						AptDateTime date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						NextAptNum number(20) NOT NULL,
						UnschedStatus number(20) NOT NULL,
						IsNewPatient number(3) NOT NULL,
						ProcDescript varchar2(255),
						Assistant number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						IsHygiene number(3) NOT NULL,
						DateTStamp timestamp,
						DateTimeArrived date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateTimeSeated date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateTimeDismissed date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						InsPlan1 number(20) NOT NULL,
						InsPlan2 number(20) NOT NULL,
						DateTimeAskedToArrive date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						ProcsColored varchar2(255),
						ColorOverride number(11) NOT NULL,
						AppointmentTypeNum number(20) NOT NULL,
						SecUserNumEntry number(20) NOT NULL,
						SecDateEntry date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT histappointment_HistApptNum PRIMARY KEY (HistApptNum)
						)";
				Db.NonQ(command);
				command=@"CREATE INDEX histappointment_HistUserNum ON histappointment (HistUserNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX histappointment_AptNum ON histappointment (AptNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX histappointment_PatNum ON histappointment (PatNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX histappointment_Confirmed ON histappointment (Confirmed)";
				Db.NonQ(command);
				command=@"CREATE INDEX histappointment_Op ON histappointment (Op)";
				Db.NonQ(command);
				command=@"CREATE INDEX histappointment_ProvNum ON histappointment (ProvNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX histappointment_ProvHyg ON histappointment (ProvHyg)";
				Db.NonQ(command);
				command=@"CREATE INDEX histappointment_NextAptNum ON histappointment (NextAptNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX histappointment_UnschedStatus ON histappointment (UnschedStatus)";
				Db.NonQ(command);
				command=@"CREATE INDEX histappointment_Assistant ON histappointment (Assistant)";
				Db.NonQ(command);
				command=@"CREATE INDEX histappointment_ClinicNum ON histappointment (ClinicNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX histappointment_InsPlan1 ON histappointment (InsPlan1)";
				Db.NonQ(command);
				command=@"CREATE INDEX histappointment_InsPlan2 ON histappointment (InsPlan2)";
				Db.NonQ(command);
				command=@"CREATE INDEX histappointment_AppointmentTyp ON histappointment (AppointmentTypeNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX histappointment_SecUserNumEntr ON histappointment (SecUserNumEntry)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS substitutionlink";
				Db.NonQ(command);
				command=@"CREATE TABLE substitutionlink (
					SubstitutionLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
					PlanNum bigint NOT NULL,
					CodeNum bigint NOT NULL,
					INDEX(PlanNum),
					INDEX(CodeNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE substitutionlink'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE substitutionlink (
					SubstitutionLinkNum number(20) NOT NULL,
					PlanNum number(20) NOT NULL,
					CodeNum number(20) NOT NULL,
					CONSTRAINT substitutionlink_SubstitutionL PRIMARY KEY (SubstitutionLinkNum)
					)";
				Db.NonQ(command);
				command=@"CREATE INDEX substitutionlink_PlanNum ON substitutionlink (PlanNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX substitutionlink_CodeNum ON substitutionlink (CodeNum)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE treatplan ADD TPType tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE treatplan ADD TPType number(3)";
				Db.NonQ(command);
				command="UPDATE treatplan SET TPType = 0 WHERE TPType IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE treatplan MODIFY TPType NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="UPDATE treatplan INNER JOIN patient ON treatplan.PatNum=patient.PatNum SET TPType=1 WHERE patient.DiscountPlanNum!=0";
				Db.NonQ(command);
			}
			else {//oracle
				//Joins are not supported within UPDATE statements so we have to use an "exists" instead.
				command=@"UPDATE treatplan SET treatplan.TPType=1 
					WHERE EXISTS (SELECT patient.PatNum FROM patient WHERE patient.DiscountPlanNum!=0 AND treatplan.PatNum=patient.PatNum)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS inseditlog";
				Db.NonQ(command);
				command=@"CREATE TABLE inseditlog (
						InsEditLogNum bigint NOT NULL auto_increment PRIMARY KEY,
						FKey bigint NOT NULL,
						LogType tinyint NOT NULL,
						FieldName varchar(255) NOT NULL,
						OldValue varchar(255) NOT NULL,
						NewValue varchar(255) NOT NULL,
						UserNum bigint NOT NULL,
						DateTStamp timestamp,
						ParentKey bigint NOT NULL,
						INDEX FKeyType (LogType,FKey),
						INDEX(UserNum),
						INDEX(ParentKey)
						) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE inseditlog'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE inseditlog (
						InsEditLogNum number(20) NOT NULL,
						FKey number(20) NOT NULL,
						LogType number(3) NOT NULL,
						FieldName varchar2(255),
						OldValue varchar2(255),
						NewValue varchar2(255),
						UserNum number(20) NOT NULL,
						DateTStamp timestamp,
						ParentKey number(20) NOT NULL,
						CONSTRAINT inseditlog_InsEditLogNum PRIMARY KEY (InsEditLogNum)
						)";
				Db.NonQ(command);
				command=@"CREATE INDEX inseditlog_FKeyType ON inseditlog (LogType,FKey)";
				Db.NonQ(command);
				command=@"CREATE INDEX inseditlog_UserNum ON inseditlog (UserNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX inseditlog_ParentKey ON inseditlog (ParentKey)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS stmtlink";
				Db.NonQ(command);
				command=@"CREATE TABLE stmtlink (
						StmtLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
						StatementNum bigint NOT NULL,
						StmtLinkType tinyint NOT NULL,
						FKey bigint NOT NULL,
						INDEX(StatementNum),
						INDEX FKeyAndType (StmtLinkType,FKey)
						) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE stmtlink'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE stmtlink (
						StmtLinkNum number(20) NOT NULL,
						StatementNum number(20) NOT NULL,
						StmtLinkType number(3) NOT NULL,
						FKey number(20) NOT NULL,
						CONSTRAINT stmtlink_StmtLinkNum PRIMARY KEY (StmtLinkNum)
						)";
				Db.NonQ(command);
				command=@"CREATE INDEX stmtlink_StatementNum ON stmtlink (StatementNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX stmtlink_FKey ON stmtlink (FKey)";
				Db.NonQ(command);
			}
			//Convert StmtProcAttach rows to stmtlink table
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO stmtlink (StatementNum,StmtLinkType,FKey) "
						+"SELECT StatementNum,1,ProcNum "
						+"FROM stmtprocattach ";
				Db.NonQ(command);
			}
			else {//oracle
				command="SELECT StatementNum,ProcNum FROM stmtprocattach";
				DataTable tableStmtLink=Db.GetTable(command);
				foreach(DataRow row in tableStmtLink.Rows) {
					command="INSERT INTO stmtink(StmtLinkNum,StatementNum,StmtLinkType,FKey) "
							+"VALUES("
									+"(SELECT COALESCE(MAX(StmtLinkNum),0)+1 FROM stmtlink)"
									+","+POut.Long(PIn.Long(row["StatementNum"].ToString()))
									+",1"//Type
									+","+POut.Long(PIn.Long(row["ProcNum"].ToString()))//AdjNum
							+")";
					Db.NonQ(command);
				}
			}
			//Convert StmtPaySplitAttach rows to stmtlink table
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO stmtlink (StatementNum,StmtLinkType,FKey) "
						+"SELECT StatementNum,2,PaySplitNum "
						+"FROM stmtpaysplitattach ";
				Db.NonQ(command);
			}
			else {//oracle
				command="SELECT StatementNum,PaySplitNum FROM stmtpaysplitattach";
				DataTable tableStmtLink = Db.GetTable(command);
				foreach(DataRow row in tableStmtLink.Rows) {
					command="INSERT INTO stmtink(StmtLinkNum,StatementNum,StmtLinkType,FKey) "
							+"VALUES("
									+"(SELECT COALESCE(MAX(StmtLinkNum),0)+1 FROM stmtlink)"
									+","+POut.Long(PIn.Long(row["StatementNum"].ToString()))
									+",2"//Type
									+","+POut.Long(PIn.Long(row["PaySplitNum"].ToString()))//PaySplitNum
							+")";
					Db.NonQ(command);
				}
			}
			//Convert StmtAdjAttach rows to stmtlink table
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO stmtlink (StatementNum,StmtLinkType,FKey) "
						+"SELECT StatementNum,3,AdjNum "
						+"FROM stmtadjattach ";
				Db.NonQ(command);
			}
			else {//oracle
				command="SELECT StatementNum,AdjNum FROM stmtadjattach";
				DataTable tableStmtLink = Db.GetTable(command);
				foreach(DataRow row in tableStmtLink.Rows) {
					command="INSERT INTO stmtink(StmtLinkNum,StatementNum,StmtLinkType,FKey) "
							+"VALUES("
									+"(SELECT COALESCE(MAX(StmtLinkNum),0)+1 FROM stmtlink)"
									+","+POut.Long(PIn.Long(row["StatementNum"].ToString()))
									+",3"//Type
									+","+POut.Long(PIn.Long(row["AdjNum"].ToString()))//AdjNum
							+")";
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS stmtprocattach";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE stmtprocattach'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS stmtpaysplitattach";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE stmtpaysplitattach'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS stmtadjattach";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE stmtadjattach'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
			}
			//We shouldn't have ADA hardcoded into our program.  Changing the description to Proc Code but preserving ADA Code for current USA customers.
			//The display fields Categories of 0 and 6 are None (default progress note grid) and ProcedureGroupNote
			//The following queries are Oracle compatible. 
			if(CultureInfo.CurrentCulture.Name=="en-US") {
				command="UPDATE displayfield SET Description='ADA Code' WHERE Description='' AND Category IN (0,6) AND InternalName = 'ADA Code'";
				Db.NonQ(command);
			}
			command="UPDATE displayfield SET InternalName='Proc Code' WHERE InternalName='ADA Code' AND Category IN (0,6)";
			Db.NonQ(command);
			//Add InsPlanPickListExisting permission to groups with existing permission InsPlanEdit------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=110";  //110 is InsPlanEdit
			table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(groupNum)+",144)"; //InsPlanPickListExisting
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
						 +"VALUES((SELECT COALESCE(MAX(GroupPermNum),0)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",144,0)"; //InsPlanPickListExisting
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) { //allow future debits, default to current behavior. 0 by default
				command="INSERT INTO preference(PrefName,ValueString) VALUES('AccountAllowFutureDebits',0)";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'AccountAllowFutureDebits',0)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE procedurecode ADD BypassGlobalLock tinyint NOT NULL";//Defaults to NeverBypass
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE procedurecode ADD BypassGlobalLock number(3)";
				Db.NonQ(command);
				command="UPDATE procedurecode SET BypassGlobalLock = 0 WHERE BypassGlobalLock IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE procedurecode MODIFY BypassGlobalLock NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE clinic ADD Specialty bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE clinic ADD INDEX (Specialty)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE clinic ADD Specialty number(20)";
				Db.NonQ(command);
				command="UPDATE clinic SET Specialty = 0 WHERE Specialty IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE clinic MODIFY Specialty NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX clinic_Specialty ON clinic (Specialty)";
				Db.NonQ(command);
			}
			//Create a new clone preference that will allow clones to be created into their own family and tied to the master via a super family.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('CloneCreateSuperFamily','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'CloneCreateSuperFamily','0')";
				Db.NonQ(command);
			}
			command="SELECT ValueString FROM preference WHERE PrefName='ShowFeaturePatientClone'";
			table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql && table.Rows.Count > 0 && PIn.Bool(table.Rows[0]["ValueString"].ToString())) {
				//Get every single potential clone in the database.
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.2.1 - Clones | Finding Potentials");
				//Our current clone system requires the following fields to be exactly the same: Guarantor,LName,FName, and Birthdate.
				command=@"SELECT Guarantor,LName,FName,Birthdate
					FROM patient
					WHERE PatStatus=0  -- PatientStatus.Patient
					AND YEAR(Birthdate) > 1880
					GROUP BY Guarantor,LName,FName,Birthdate
					HAVING COUNT(*) > 1";
				table=Db.GetTable(command);
				//Now that we have all potential clones from the database, go try and find the corresponding master.
				for(int i=0;i<table.Rows.Count;i++) {
					ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.2.1 - Clones | Considering "+(i+1)+" / "+table.Rows.Count);
					command="SELECT PatNum,LName,FName,Birthdate "
						+"FROM patient "
						+"WHERE PatStatus=0 "//PatientStatus.Patient
						+"AND LName='"+POut.String(table.Rows[i]["LName"].ToString())+"' "
						+"AND FName='"+POut.String(table.Rows[i]["FName"].ToString())+"' "
						+"AND Birthdate="+POut.Date(PIn.Date(table.Rows[i]["Birthdate"].ToString()));
					DataTable tableClones=Db.GetTable(command);
					//There needs to be at least two patients matching the exact name and birthdate in order to even consider them clones of each other.
					if(tableClones.Rows.Count < 2) {
						continue;
					}
					//At this point we know we have at least 2 patients, we need to find the master or original patient and then link the rest to them.
					//The master patient will have at least one lower case character within the last and first name fields.  Clones will have all caps.
					DataRow rowMaster=tableClones.Select().FirstOrDefault(x => x["LName"].ToString().Any(y => char.IsLower(y))
						&& x["FName"].ToString().Any(y => char.IsLower(y)));
					List<DataRow> listCloneRows=tableClones.Select().ToList().FindAll(x => x["LName"].ToString().All(y => char.IsUpper(y)
						&& x["FName"].ToString().All(z => char.IsUpper(z))));
					if(rowMaster==null || listCloneRows==null || listCloneRows.Count==0) {
						continue;//Either no master was found or no clones were found (this will happen for true duplicate patients).
					}
					//Now we can make patientlink entries that will associate the master or original patient with the corresponding clones.
					long patNumMaster=PIn.Long(rowMaster["PatNum"].ToString());
					foreach(DataRow rowClone in listCloneRows) {
						long patNumClone=PIn.Long(rowClone["PatNum"].ToString());
						if(patNumMaster==patNumClone) {
							continue;//Do not create a link between the master and themself.
						}
						command="INSERT INTO patientlink (PatNumFrom,PatNumTo,LinkType,DateTimeLink) "
							+"VALUES("+POut.Long(patNumMaster)+","
							+POut.Long(patNumClone)+","
							+"2,"//PatientLinkType.Clone
							+"NOW())";
						Db.NonQ(command);
					}
				}
				//Set the progress bar back to the way it was.
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.2.1");
			}
			//Conceal clearinghouse passwords.
			command="SELECT ClearinghouseNum,Password FROM clearinghouse WHERE Password !='' AND Password IS NOT NULL";//Null check for Oracle.
			table=Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				string concealPass="";
				CDT.Class1.ConcealClearinghouse(PIn.String(row["Password"].ToString()),out concealPass);
				command="UPDATE clearinghouse SET Password='"+POut.String(concealPass)+"' "
					+"WHERE ClearinghouseNum="+PIn.Long(row["ClearinghouseNum"].ToString());
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command=@"INSERT INTO histappointment (HistUserNum, HistApptAction, ApptSource, AptNum, PatNum, AptStatus, Pattern, Confirmed, TimeLocked, 
					Op, Note, ProvNum, ProvHyg, AptDateTime, NextAptNum, UnschedStatus, IsNewPatient, ProcDescript, Assistant, ClinicNum, IsHygiene, 
					DateTStamp, DateTimeArrived, DateTimeSeated, DateTimeDismissed, InsPlan1, InsPlan2, DateTimeAskedToArrive, ProcsColored, ColorOverride, 
					AppointmentTypeNum, SecUserNumEntry, SecDateEntry)
					SELECT 0,4/*Deleted*/,0/*No eService Type*/, AptNum, PatNum, AptStatus, Pattern, Confirmed, TimeLocked, Op, Note, ProvNum, 
					ProvHyg, AptDateTime, NextAptNum, UnschedStatus, IsNewPatient, ProcDescript, Assistant, ClinicNum, IsHygiene, DateTStamp, DateTimeArrived, 
					DateTimeSeated, DateTimeDismissed, InsPlan1, InsPlan2, DateTimeAskedToArrive, ProcsColored, ColorOverride, AppointmentTypeNum, 
					SecUserNumEntry, SecDateEntry
					FROM appointmentdeleted";
				Db.NonQ(command);
				command="DROP TABLE IF EXISTS appointmentdeleted";
				Db.NonQ(command);
			}
			else {//oracle
				command=@"INSERT INTO histappointment (HistApptNum, HistUserNum, HistApptAction, ApptSource, AptNum, PatNum, AptStatus, Pattern, 
					Confirmed, TimeLocked, Op, Note, ProvNum, ProvHyg, AptDateTime, NextAptNum, UnschedStatus, IsNewPatient, ProcDescript, Assistant, 
					ClinicNum, IsHygiene, DateTStamp, DateTimeArrived, DateTimeSeated, DateTimeDismissed, InsPlan1, InsPlan2, DateTimeAskedToArrive, 
					ProcsColored, ColorOverride, AppointmentTypeNum, SecUserNumEntry, SecDateEntry)
					SELECT (SELECT COALESCE(MAX(HistApptNum),0)+1 FROM histappointment), 0,4/*Deleted*/,0/*No eService Type*/, AptNum, 
					PatNum, AptStatus, Pattern, Confirmed, TimeLocked, Op, Note, ProvNum, ProvHyg, AptDateTime, NextAptNum, UnschedStatus, IsNewPatient, 
					ProcDescript, Assistant, ClinicNum, IsHygiene, DateTStamp, DateTimeArrived, DateTimeSeated, DateTimeDismissed, InsPlan1, InsPlan2, 
					DateTimeAskedToArrive, ProcsColored, ColorOverride, AppointmentTypeNum, SecUserNumEntry, SecDateEntry
					FROM appointmentdeleted";
				Db.NonQ(command);
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE appointmentdeleted'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE sheet ADD SheetDefNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE sheet ADD INDEX (SheetDefNum)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE sheet ADD SheetDefNum number(20)";
				Db.NonQ(command);
				command="UPDATE sheet SET SheetDefNum = 0 WHERE SheetDefNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE sheet MODIFY SheetDefNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX sheet_SheetDefNum ON sheet (SheetDefNum)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE sheetdef ADD BypassGlobalLock tinyint NOT NULL";//Defaults to NeverBypass
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE sheetdef ADD BypassGlobalLock number(3)";
				Db.NonQ(command);
				command="UPDATE sheetdef SET BypassGlobalLock = 0 WHERE BypassGlobalLock IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE sheetdef MODIFY BypassGlobalLock NOT NULL";
				Db.NonQ(command);
			}
			//Update Payment clinic settings
			command="UPDATE preference SET PrefName='PaymentClinicSetting' WHERE PrefName='PaymentsUsePatientClinic'";
			Db.NonQ(command);
			//Adding domainuserlogin
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE userod ADD DomainUser varchar(255) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE userod ADD DomainUser varchar2(255)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('DomainLoginEnabled','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'DomainLoginEnabled','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('DomainLoginPath','')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'DomainLoginPath','')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS smsblockphone";
				Db.NonQ(command);
				command=@"CREATE TABLE smsblockphone (
						SmsBlockPhoneNum bigint NOT NULL auto_increment PRIMARY KEY,
						BlockWirelessNumber varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE smsblockphone'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE smsblockphone (
						SmsBlockPhoneNum number(20) NOT NULL,
						BlockWirelessNumber varchar2(255),
						CONSTRAINT smsblockphone_SmsBlockPhoneNum PRIMARY KEY (SmsBlockPhoneNum)
						)";
				Db.NonQ(command);
			}
			//Insert Tigerview bridge program property-----------------------------------------------------------------
			command="SELECT ProgramNum FROM program WHERE ProgName = 'TigerView'";
			long tigerViewProgNum=PIn.Long(Db.GetScalar(command));
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(tigerViewProgNum)+"', "
					+"'Birthdate format (default MM/dd/yy)', "
					+"'MM/dd/yy')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+"'"+POut.Long(tigerViewProgNum)+"', "
					+"'Birthdate format (default MM/dd/yy)', "
					+"'MM/dd/yy', "
					+"'0')";
				Db.NonQ(command);
			}//End Tigerview
			//Giving OrthoChartEditUser permission to everybody that has OrthoChartEditFull permission -----------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=79";//OrthoChartEditFull permission
			table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(groupNum)+",145)"; //OrthoChartEditUser permission
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
						 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",145,0)"; //OrthoChartEditUser permission
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE orthochart ADD UserNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE orthochart ADD INDEX (UserNum)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE orthochart ADD UserNum number(20)";
				Db.NonQ(command);
				command="UPDATE orthochart SET UserNum = 0 WHERE UserNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE orthochart MODIFY UserNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX orthochart_UserNum ON orthochart (UserNum)";
				Db.NonQ(command);
			}
			//Giving ProcedureNoteUser permission to everybody that has ProcedureNoteFull permission -------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=53";//ProcedureNoteFull permission
			table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(groupNum)+",146)"; //ProcedureNoteUser permission
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
						 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",146,0)"; //ProcedureNoteUser permission
					Db.NonQ(command);
				}
			}
			//Giving GroupNoteEditSigned permission to everybody -------------------------------------------------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(groupNum)+",147)"; //GroupNoteEditSigned permission
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
						 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",147,0)"; //GroupNoteEditSigned permission
					Db.NonQ(command);
				}
			}
			#region Transworld Systems bridge
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note) "
					+"VALUES ("
					+"'Transworld',"
					+"'Transworld Systems Inc (TSI) from www.tsico.com',"
					+"'0',"
					+"'',"
					+"'',"//leave blank if none
					+"'No program path or arguments. Usernames, passwords, client IDs, and SFTP connection details are supplied by TSI.')";
				long programNum=Db.NonQ(command,true);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("
					+POut.Long(programNum)+","
					+"'SftpServerAddress',"
					+"'',"
					+"'',"
					+"0)";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("
					+POut.Long(programNum)+","
					+"'SftpServerPort',"
					+"'',"
					+"'',"
					+"0)";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("
					+POut.Long(programNum)+","
					+"'SftpUsername',"
					+"'',"
					+"'',"
					+"0)";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("
					+POut.Long(programNum)+","
					+"'SftpPassword',"
					+"'',"
					+"'',"
					+"0)";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("
					+POut.Long(programNum)+","
					+"'ClientIdAccelerator',"
					+"'',"
					+"'',"
					+"0)";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("
					+POut.Long(programNum)+","
					+"'ClientIdCollection',"
					+"'',"
					+"'',"
					+"0)";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("
					+POut.Long(programNum)+","
					+"'IsThankYouLetterEnabled',"
					+"'0',"
					+"'',"
					+"0)";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note) "
					+"VALUES ("
					+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
					+"'Transworld',"
					+"'Transworld Systems Inc (TSI) from www.tsico.com',"
					+"'0',"
					+"'',"
					+"'',"
					+"'No program path or arguments. Usernames, passwords, client IDs, and SFTP connection details are supplied by TSI.')";
				long programNum=Db.NonQ(command,true,"ProgramNum","program");
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+POut.Long(programNum)+","
					+"'SftpServerAddress',"
					+"'',"
					+"'',"
					+"0)";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+POut.Long(programNum)+","
					+"'SftpServerPort',"
					+"'',"
					+"'',"
					+"0)";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+POut.Long(programNum)+","
					+"'SftpUsername',"
					+"'',"
					+"'',"
					+"0)";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+POut.Long(programNum)+","
					+"'SftpPassword',"
					+"'',"
					+"'',"
					+"0)";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+POut.Long(programNum)+","
					+"'ClientIdAccelerator',"
					+"'',"
					+"'',"
					+"0)";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+POut.Long(programNum)+","
					+"'ClientIdCollection',"
					+"'',"
					+"'',"
					+"0)";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+POut.Long(programNum)+","
					+"'IsThankYouLetterEnabled',"
					+"'0',"
					+"'',"
					+"0)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS tsitranslog";
				Db.NonQ(command);
				command=@"CREATE TABLE tsitranslog (
					TsiTransLogNum bigint NOT NULL auto_increment PRIMARY KEY,
					PatNum bigint NOT NULL,
					UserNum bigint NOT NULL,
					TransType tinyint NOT NULL,
					TransDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					DemandType tinyint NOT NULL,
					ServiceCode tinyint NOT NULL,
					TransAmt double NOT NULL,
					AccountBalance double NOT NULL,
					FKeyType tinyint NOT NULL,
					FKey bigint NOT NULL,
					RawMsgText varchar(1000) NOT NULL,
					INDEX(PatNum),
					INDEX(UserNum),
					INDEX FKeyAndType (FKey,FKeyType)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE tsitranslog'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE tsitranslog (
					TsiTransLogNum number(20) NOT NULL,
					PatNum number(20) NOT NULL,
					UserNum number(20) NOT NULL,
					TransType number(3) NOT NULL,
					TransDateTime date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
					DemandType number(3) NOT NULL,
					ServiceCode number(3) NOT NULL,
					TransAmt number(38,8) NOT NULL,
					AccountBalance number(38,8) NOT NULL,
					FKeyType number(3) NOT NULL,
					FKey number(20) NOT NULL,
					RawMsgText varchar2(1000),
					CONSTRAINT tsitranslog_TsiTransLogNum PRIMARY KEY (TsiTransLogNum)
					)";
				Db.NonQ(command);
				command=@"CREATE INDEX tsitranslog_PatNum ON tsitranslog (PatNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX tsitranslog_UserNum ON tsitranslog (UserNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX tsitranslog_FKeyAndType ON tsitranslog (FKey,FKeyType)";
				Db.NonQ(command);
			}
			#endregion Transworld Systems bridge
		}//end function

		private static void To17_2_2() {
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE refattach ADD DateTStamp timestamp";
				Db.NonQ(command);
				command="UPDATE refattach SET DateTStamp = NOW()";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE refattach ADD DateTStamp timestamp";
				Db.NonQ(command);
				command="UPDATE refattach SET DateTStamp = SYSTIMESTAMP";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE referral ADD DateTStamp timestamp";
				Db.NonQ(command);
				command="UPDATE referral SET DateTStamp = NOW()";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE referral ADD DateTStamp timestamp";
				Db.NonQ(command);
				command="UPDATE referral SET DateTStamp = SYSTIMESTAMP";
				Db.NonQ(command);
			}
		}

		private static void To17_2_4() {
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedTextsPerBatch','25')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedTextsPerBatch','25')";
				Db.NonQ(command);
			}
		}

		private static void To17_2_5() {
			string command;
			//Tigerview bridge program property-----------------------------------------------------------------
			command="SELECT ProgramNum FROM program WHERE ProgName = 'TigerView'";
			long tigerViewProgNum=PIn.Long(Db.GetScalar(command));
			//There was a bug with an old 17.2.1 script where a new program property for TigerView would not associate to TigerView correctly.
			//Even though it isn't necessary, we should try and clean up after ourselves by deleting all properties with a -1 program num.
			command="DELETE FROM programproperty WHERE ProgramNum=-1";
			//The 17.2.1 script was corrected in 17.2.5 so we now want to see if the property exists first.
			command="SELECT COUNT(*) "+
				"FROM programproperty "+
				"WHERE ProgramNum="+POut.Long(tigerViewProgNum)+" "+
				"AND PropertyDesc='Birthdate format (default MM/dd/yy)' "+
				"AND PropertyValue='MM/dd/yy'";
			if(Db.GetScalar(command)=="0") {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(tigerViewProgNum)+"', "
						+"'Birthdate format (default MM/dd/yy)', "
						+"'MM/dd/yy')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
						+"'"+POut.Long(tigerViewProgNum)+"', "
						+"'Birthdate format (default MM/dd/yy)', "
						+"'MM/dd/yy', "
						+"'0')";
					Db.NonQ(command);
				}
			}//End Tigerview
		}

		private static void To17_2_6() {
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedNewPatAllowChildren','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedNewPatAllowChildren','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('UnschedDaysFuture','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'UnschedDaysFuture','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('UnschedDaysPast','365')";//Default ot 1 year.
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'UnschedDaysPast','365')";//Default ot 1 year.
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE clearinghouse ADD IsClaimExportAllowed tinyint NOT NULL DEFAULT 1";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE clearinghouse ADD IsClaimExportAllowed number(3) DEFAULT 1";
				Db.NonQ(command);
				command="UPDATE clearinghouse SET IsClaimExportAllowed = 0 WHERE IsClaimExportAllowed IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE clearinghouse MODIFY IsClaimExportAllowed NOT NULL";
				Db.NonQ(command);
			}
		}

		private static void To17_2_8() {
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS etrans835attach";
				Db.NonQ(command);
				command=@"CREATE TABLE etrans835attach (
						Etrans835AttachNum bigint NOT NULL auto_increment PRIMARY KEY,
						EtransNum bigint NOT NULL,
						ClaimNum bigint NOT NULL,
						ClpSegmentIndex int NOT NULL,
						INDEX(EtransNum),
						INDEX(ClaimNum)
						) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE etrans835attach'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE etrans835attach (
						Etrans835AttachNum number(20) NOT NULL,
						EtransNum number(20) NOT NULL,
						ClaimNum number(20) NOT NULL,
						ClpSegmentIndex number(11) NOT NULL,
						CONSTRAINT etrans835attach_Etrans835Attac PRIMARY KEY (Etrans835AttachNum)
						)";
				Db.NonQ(command);
				command=@"CREATE INDEX etrans835attach_EtransNum ON etrans835attach (EtransNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX etrans835attach_ClaimNum ON etrans835attach (ClaimNum)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('PlannedApptDaysFuture','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'PlannedApptDaysFuture','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('PlannedApptDaysPast','365')";//Default ot 1 year.
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'PlannedApptDaysPast','365')";//Default ot 1 year.
				Db.NonQ(command);
			}
		}

		private static void To17_2_13() {
			string command;
			command="SELECT COUNT(*) FROM preference WHERE PrefName='PlannedApptDaysFuture'";
			if(Db.GetScalar(command)=="0") {//Was suppose to be in 17.2.12 but was put in 17.2.8, so fixing in 17.2.13
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('PlannedApptDaysFuture','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'PlannedApptDaysFuture','0')";
					Db.NonQ(command);
				}
			}
			command="SELECT COUNT(*) FROM preference WHERE PrefName='PlannedApptDaysPast'";
			if(Db.GetScalar(command)=="0") {//Was suppose to be in 17.2.12 but was put in 17.2.8, so fixing in 17.2.13
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('PlannedApptDaysPast','365')";//Default ot 1 year.
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'PlannedApptDaysPast','365')";//Default ot 1 year.
					Db.NonQ(command);
				}
			}
		}

		private static void To17_2_23() {
			string command="";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE task ADD DateTimeOriginal datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE task ADD DateTimeOriginal date";
				Db.NonQ(command);
				command="UPDATE task SET DateTimeOriginal = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateTimeOriginal IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE task MODIFY DateTimeOriginal NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE taskhist ADD DateTimeOriginal datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE taskhist ADD DateTimeOriginal date";
				Db.NonQ(command);
				command="UPDATE taskhist SET DateTimeOriginal = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateTimeOriginal IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE taskhist MODIFY DateTimeOriginal NOT NULL";
				Db.NonQ(command);
			}
		}

		private static void To17_2_41() {
			string command="SELECT ValueString FROM preference WHERE PrefName='ShowFeaturePatientClone'";
			DataTable table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql && table.Rows.Count > 0 && PIn.Bool(table.Rows[0]["ValueString"].ToString())) {
				//Get every single potential clone in the database.
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.2.41 - Clones | Finding Potentials");
				//Our current clone system requires the following fields to be exactly the same: Guarantor,LName,FName, and Birthdate.
				//We have to join on the patientlink table because users could have manually associated patients to their clones / added new clones entirely.
				//The previous time that we tried to convert old clones into the new system did not account for non-alphabetic characters (symbols) in names.
				command=@"SELECT Guarantor,LName,FName,Birthdate
					FROM patient
					LEFT JOIN patientlink patientlinkfrom ON patient.PatNum=patientlinkfrom.PatNumFrom AND patientlinkfrom.LinkType=2
					LEFT JOIN patientlink patientlinkto ON patient.PatNum=patientlinkto.PatNumTo AND patientlinkto.LinkType=2
					WHERE PatStatus=0  -- PatientStatus.Patient
					AND YEAR(Birthdate) > 1880
					AND patientlinkfrom.PatNumFrom IS NULL
					AND patientlinkto.PatNumTo IS NULL
					GROUP BY Guarantor,LName,FName,Birthdate
					HAVING COUNT(*) > 1";
				table=Db.GetTable(command);
				//Now that we have all potential clones from the database, go try and find the corresponding master.
				for(int i=0;i<table.Rows.Count;i++) {
					ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.2.41 - Clones | Considering "+(i+1)+" / "+table.Rows.Count);
					command="SELECT PatNum,LName,FName,Birthdate "
						+"FROM patient "
						+"WHERE PatStatus=0 "//PatientStatus.Patient
						+"AND LName='"+POut.String(table.Rows[i]["LName"].ToString())+"' "
						+"AND FName='"+POut.String(table.Rows[i]["FName"].ToString())+"' "
						+"AND Birthdate="+POut.Date(PIn.Date(table.Rows[i]["Birthdate"].ToString()));
					DataTable tableClones=Db.GetTable(command);
					//There needs to be at least two patients matching the exact name and birthdate in order to even consider them clones of each other.
					if(tableClones.Rows.Count < 2) {
						continue;
					}
					//At this point we know we have at least 2 patients, we need to find the master or original patient and then link the rest to them.
					//The master patient will have at least one lower case character within the last and first name fields.  Clones will have all caps.
					//We have to ignore any non-alphabetic characters (like symbols and spaces).
					DataRow rowMaster=tableClones.Select().FirstOrDefault(x => x["LName"].ToString().Any(y => char.IsLower(y))
						&& x["FName"].ToString().Any(y => char.IsLower(y)));
					List<DataRow> listCloneRows=tableClones.Select().ToList().FindAll(x => x["LName"].ToString().Where(Char.IsLetter).All(y => char.IsUpper(y)
						&& x["FName"].ToString().Where(Char.IsLetter).All(z => char.IsUpper(z))));
					if(rowMaster==null || listCloneRows==null || listCloneRows.Count==0) {
						continue;//Either no master was found or no clones were found (this will happen for true duplicate patients).
					}
					//Now we can make patientlink entries that will associate the master or original patient with the corresponding clones.
					long patNumMaster=PIn.Long(rowMaster["PatNum"].ToString());
					foreach(DataRow rowClone in listCloneRows) {
						long patNumClone=PIn.Long(rowClone["PatNum"].ToString());
						if(patNumMaster==patNumClone) {
							continue;//Do not create a link between the master and themself.
						}
						command="INSERT INTO patientlink (PatNumFrom,PatNumTo,LinkType,DateTimeLink) "
							+"VALUES("+POut.Long(patNumMaster)+","
							+POut.Long(patNumClone)+","
							+"2,"//PatientLinkType.Clone
							+"NOW())";
						Db.NonQ(command);
					}
				}
				//Set the progress bar back to the way it was.
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.2.41");
			}
		}

		private static void To17_3_1() {
			string command;
			DataTable table;
			long groupNum;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE discountplan ADD IsHidden tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE discountplan ADD IsHidden number(3)";
				Db.NonQ(command);
				command="UPDATE discountplan SET IsHidden = 0 WHERE IsHidden IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE discountplan MODIFY IsHidden NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE carrier ADD ApptTextBackColor int NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE carrier ADD ApptTextBackColor number(11)";
				Db.NonQ(command);
				command="UPDATE carrier SET ApptTextBackColor = 0 WHERE ApptTextBackColor IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE carrier MODIFY ApptTextBackColor NOT NULL";
				Db.NonQ(command);
			}
			command="UPDATE carrier SET ApptTextBackColor="+System.Drawing.Color.Black.ToArgb();
			Db.NonQ(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE wikilistheaderwidth ADD PickList text NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE wikilistheaderwidth ADD PickList clob";
				Db.NonQ(command);
			}
			string templateText="[NameF], an opening has come up on [Date] at [Time] at [OfficeName]. Please call [OfficePhone] to reserve this slot.";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ASAPTextTemplate','"+templateText+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
					+"'ASAPTextTemplate','"+templateText+"')";
				Db.NonQ(command);
			}
			//Populate AlertSub table to encourage usage.
			//No harm in subscribing for all clinics.
			//4 for CallbackRequested
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command=@"INSERT INTO alertsub (UserNum,ClinicNum,Type) 
									SELECT userod.UserNum,0,4 
									FROM userod 
									UNION ALL 
									SELECT userod.UserNum,clinic.ClinicNum,4 
									FROM userod 
									INNER JOIN clinic";
				Db.NonQ(command);
			}
			else {//oracle
				command="SELECT UserNum FROM userod";
				DataTable tableUserods=Db.GetTable(command);
				//Always subscribe every user to the practice level alerts.
				foreach(DataRow rowUserod in tableUserods.Rows) {
					command="INSERT INTO alertsub (AlertSubNum,UserNum,ClinicNum,Type) "
						+"VALUES("
						+"(SELECT COALESCE(MAX(AlertSubNum),0)+1 FROM alertsub),"
						+rowUserod["UserNum"].ToString()+","
						+"0," //ClinicNum
						+"4)";//CallbackRequested
					Db.NonQ(command);
				}
				//Now subscribe every user to every clinic alert.
				command="SELECT ClinicNum FROM clinic";
				DataTable tableClinics=Db.GetTable(command);
				foreach(DataRow rowUserod in tableUserods.Rows) {
					foreach(DataRow rowClinic in tableClinics.Rows) {
						command="INSERT INTO alertsub (AlertSubNum,UserNum,ClinicNum,Type) "
							+"VALUES("
							+"(SELECT COALESCE(MAX(AlertSubNum),0)+1 FROM alertsub),"
							+rowUserod["UserNum"].ToString()+","
							+rowClinic["ClinicNum"].ToString()+","
							+"4)";//CallbackRequested
						Db.NonQ(command);
					}
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedNewPatVerifyInfo','1')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
					+"'WebSchedNewPatVerifyInfo','1')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE alertitem ADD ItemValue varchar(4000) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE alertitem ADD ItemValue varchar2(4000)";
				Db.NonQ(command);
			}
			command="SELECT MAX(displayreport.ItemOrder) FROM displayreport WHERE displayreport.Category = 1"; //Daily
			long itemorder = Db.GetLong(command)+1; //get the next available ItemOrder for the Daily Category to put this new report last.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODUnfinalizedInsPay',"+POut.Long(itemorder)
					+",'Unfinalized Insurance Payments',1,0)";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO displayreport(DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) "
					+"VALUES((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODUnfinalizedInsPay',"+POut.Long(itemorder)
					+",'Unfinalized Insurance Payments',1,0)";
				Db.NonQ(command);
			}
			//Add permission for this report to everybody with the Outstanding Claims Report permission.
			long reportNumODOutStanding=Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODOutstandingInsClaims'");
			long reportNumODUnfinalizedPay=Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODUnfinalizedInsPay'");
			//Get a table of UserGroupNum that have permissions to Reports and to the ODOutstandingInsClaims report.
			DataTable userGroupTable=Db.GetTable("SELECT UserGroupNum FROM grouppermission WHERE PermType=22 AND FKey="+POut.Long(reportNumODOutStanding));
			foreach(DataRow row in userGroupTable.Rows) {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//Insert Reports permission (22) for the daily ODUnfinalizedInsPay report.
					command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES('0001-01-01',0,"+POut.Long(PIn.Long(row["UserGroupNum"].ToString()))+",22,"+POut.Long(reportNumODUnfinalizedPay)+")";
					Db.NonQ(command);
				}
				else {//oracle
							//Insert Reports permission (22) for the ODUnfinalizedInsPay report.
					command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission)"
						+","+"TO_DATE('0001-01-01','YYYY-MM-DD'),0,"+POut.Long(PIn.Long(row["UserGroupNum"].ToString()))+",22,"
						+POut.Long(reportNumODUnfinalizedPay)+")";
					Db.NonQ(command);
				}
			}
			//Insert ActeonImagingSuite bridge-----------------------------------------------------------------
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'ActeonImagingSuite', "
					+"'AIS from www.acteongroup.com/', "
					+"'0', "
					+"'"+POut.String(@"C:\Program Files\Acteon\ActeonImagingSuite\AIS.exe")+"', "
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
					+"'Birthdate format (default yyyyMMdd)', "
					+"'yyyyMMdd')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
					+"'ActeonImagingSuite', "
					+"'AIS from www.acteongroup.com/', "
					+"'0', "
					+"'"+POut.String(@"C:\Program Files\Acteon\ActeonImagingSuite\AIS.exe")+"', "
					+"'', "//leave blank if none
					+"'')";
				long programNum=Db.NonQ(command,true,"ProgramNum","program");
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
					+"'"+POut.Long(programNum)+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0', "
					+"'0')";
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
					+"'"+POut.Long(programNum)+"', "
					+"'Birthdate format (default yyyyMMdd)', "
					+"'yyyyMMdd', "
					+"'0')";
				Db.NonQ(command);
			}//end ActeonImagingSuite bridge
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebFormsAutoFillNameAndBirthdate','1')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebFormsAutoFillNameAndBirthdate','1')";
				Db.NonQ(command);
			}
			//Web Sched ASAP ------------------------------------------------------------------------------------------------------------
			templateText="[NameF], an appointment opening has become available on [Date] at [Time] at [OfficeName]. Visit [AsapURL] to reserve it.";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedAsapTextTemplate','"+templateText+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
					+"'WebSchedAsapTextTemplate','"+templateText+"')";
				Db.NonQ(command);
			}
			templateText="[NameF], an appointment opening has become available on [Date] at [Time] at [OfficeName]. Visit [AsapURL] to attempt to make an "
				+"appointment for this new date and time.";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedAsapEmailTemplate','"+templateText+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
					+"'WebSchedAsapEmailTemplate','"+templateText+"')";
				Db.NonQ(command);
			}
			templateText="Dental Appointment Opening";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedAsapEmailSubj','"+templateText+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
					+"'WebSchedAsapEmailSubj','"+templateText+"')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedAsapEnabled','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
					+"'WebSchedAsapEnabled','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedAsapTextLimit','2')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
					+"'WebSchedAsapTextLimit','2')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS asapcomm";
				Db.NonQ(command);
				command=@"CREATE TABLE asapcomm (
						AsapCommNum bigint NOT NULL auto_increment PRIMARY KEY,
						FKey bigint NOT NULL,
						FKeyType tinyint NOT NULL,
						ScheduleNum bigint NOT NULL,
						PatNum bigint NOT NULL,
						ClinicNum bigint NOT NULL,
						ShortGUID varchar(255) NOT NULL,
						DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						DateTimeExpire datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						DateTimeSmsScheduled datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						SmsSendStatus tinyint NOT NULL,
						EmailSendStatus tinyint NOT NULL,
						DateTimeSmsSent datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						DateTimeEmailSent datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						EmailMessageNum bigint NOT NULL,
						ResponseStatus tinyint NOT NULL,
						DateTimeOrig datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						TemplateText text NOT NULL,
						TemplateEmail text NOT NULL,
						TemplateEmailSubj varchar(100) NOT NULL,
						Note text NOT NULL,
						GuidMessageToMobile text NOT NULL,
						INDEX(FKey),
						INDEX(ScheduleNum),
						INDEX(PatNum),
						INDEX(ClinicNum),
						INDEX(EmailMessageNum),
						INDEX(SmsSendStatus),
						INDEX(EmailSendStatus),
						INDEX(ShortGUID)
						) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE asapcomm'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE asapcomm (
						AsapCommNum number(20) NOT NULL,
						FKey number(20) NOT NULL,
						FKeyType number(3) NOT NULL,
						ScheduleNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						ShortGUID varchar2(255),
						DateTimeEntry date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateTimeExpire date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateTimeSmsScheduled date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						SmsSendStatus number(3) NOT NULL,
						EmailSendStatus number(3) NOT NULL,
						DateTimeSmsSent date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateTimeEmailSent date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						EmailMessageNum number(20) NOT NULL,
						ResponseStatus number(3) NOT NULL,
						DateTimeOrig date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						TemplateText clob,
						TemplateEmail clob,
						TemplateEmailSubj varchar2(100),
						Note clob,
						GuidMessageToMobile clob,
						CONSTRAINT asapcomm_AsapCommNum PRIMARY KEY (AsapCommNum)
						)";
				Db.NonQ(command);
				command=@"CREATE INDEX asapcomm_FKey ON asapcomm (FKey)";
				Db.NonQ(command);
				command=@"CREATE INDEX asapcomm_ScheduleNum ON asapcomm (ScheduleNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX asapcomm_PatNum ON asapcomm (PatNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX asapcomm_ClinicNum ON asapcomm (ClinicNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX asapcomm_EmailMessageNum ON asapcomm (EmailMessageNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX asapcomm_SmsSendStatus ON asapcomm (SmsSendStatus)";
				Db.NonQ(command);
				command=@"CREATE INDEX asapcomm_EmailSendStatus ON asapcomm (EmailSendStatus)";
				Db.NonQ(command);
				command=@"CREATE INDEX asapcomm_ShortGUID ON asapcomm (ShortGUID)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedSendASAPThreadFrequency','7')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
					+"'WebSchedSendASAPThreadFrequency','7')";
				Db.NonQ(command);
			}
			//End Web Sched ASAP
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('TreatFinderProcsAllGeneral','1')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
					+"'TreatFinderProcsAllGeneral','1')";
				Db.NonQ(command);
			}
			command="SELECT MAX(displayreport.ItemOrder) FROM displayreport WHERE displayreport.Category = 1"; //Daily
			itemorder = Db.GetLong(command)+1; //get the next available ItemOrder for the Daily Category to put this new report last.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODPatPortionUncollected',"+POut.Long(itemorder)
					+",'Patient Portion Uncollected',1,0)";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO displayreport(DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) "
					+"VALUES((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODPatPortionUncollected',"+POut.Long(itemorder)
					+",'Patient Portion Uncollected',1,0)";
				Db.NonQ(command);
			}
			//Add permission for this report to everybody with Payments report permission
			long reportPayments=Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODPayments'");
			long reportNumODPatPortionUncollected=Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODPatPortionUncollected'");
			//Get a table of UserGroupNum that have permissions to Reports and to the PaySheet report.
			userGroupTable=Db.GetTable("SELECT UserGroupNum FROM grouppermission WHERE PermType=22 AND FKey="+POut.Long(reportPayments));
			foreach(DataRow row in userGroupTable.Rows) {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//Insert Reports permission (22) for the daily ODPatPortionUncollected report.
					command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES('0001-01-01',0,"+POut.Long(PIn.Long(row["UserGroupNum"].ToString()))+",22,"+POut.Long(reportNumODPatPortionUncollected)+")";
					Db.NonQ(command);
				}
				else {//oracle
							//Insert Reports permission (22) for the ODPatPortionUncollected report.
					command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission)"
						+","+"TO_DATE('0001-01-01','YYYY-MM-DD'),0,"+POut.Long(PIn.Long(row["UserGroupNum"].ToString()))+",22,"
						+POut.Long(reportNumODPatPortionUncollected)+")";
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('EConnectorStatistics','')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
					+"'EConnectorStatistics','')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('PayPlanHideDueNow','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'PayPlanHideDueNow','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE alertitem CHANGE Description Description VARCHAR(2000) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE alertitem MODIFY (Description varchar2(2000))";
				Db.NonQ(command);
			}
			//Alert WebSchedNewPat
			string alertDescriptStr=@"The Web Sched New Pat feature now asks patients questions to verify patient information. 
If you do not want these questions to be asked, go to eServices -> Web Sched -> New Pat and uncheck the ""Verify Info"" column.
No Action Required in many cases, check your new patient Web Sched on your web site to see behavior. See online manual for details.";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				//create the alert item for clinic 0
				command="INSERT INTO alertitem (Type,Actions,Severity,ClinicNum,Description,FormToOpen) VALUES("
					+"5,"//AlertType.WebSchedNewPat
					+"7,"//ActionType.Delete|ActionType.MarkAsRead|ActionType.OpenForm
					+"1,"//SeverityType.Low
					+"0,"//ClinicNum
					+"'"+alertDescriptStr+"',"
					+"6)";//FormType.FormEServicesWebSchedNewPat.
				Db.NonQ(command);
				//Create alert items for all additional clinics
				command=@"INSERT INTO alertitem (Type,Actions,Severity,ClinicNum,Description,FormToOpen)
					SELECT 5,7,1,clinic.ClinicNum,'"+alertDescriptStr+@"',6
					FROM clinic WHERE clinic.IsHidden=0";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertitem (AlertItemNum,Type,Actions,Severity,ClinicNum,Description,FormToOpen,FKey) VALUES("+
					"(SELECT COALESCE(MAX(AlertItemNum),0)+1 FROM alertitem),"
					+"5,"//AlertType.WebSchedNewPat
					+"7,"//ActionType.Delete|ActionType.MarkAsRead|ActionType.OpenForm
					+"1,"//SeverityType.Low
					+"0,"//ClinicNum
					+"'"+alertDescriptStr+"',"
					+"6"//FormType.FormEServicesWebSchedNewPat
					+",0)";
				Db.NonQ(command);
				command="SELECT ClinicNum FROM clinic WHERE clinic.IsHidden=0";
				DataTable tableClinics=Db.GetTable(command);
				foreach(DataRow rowClinic in tableClinics.Rows) {
					command="INSERT INTO alertitem (AlertItemNum,Type,Actions,Severity,ClinicNum,Description,FormToOpen,FKey) "
						+"VALUES("
						+"(SELECT COALESCE(MAX(AlertItemNum),0)+1 FROM alertitem),"
						+"5,"//AlertType.WebSchedNewPat
						+"7,"//ActionType.Delete|ActionType.MarkAsRead|ActionType.OpenForm
						+"1,"//SeverityType.Low
						+rowClinic["ClinicNum"].ToString()+","
						+"'"+alertDescriptStr+"',"
						+"6"//FormType.FormEServicesWebSchedNewPat
						+",0)";
					Db.NonQ(command);
				}
			}
			//Populate AlertSub table to encourage usage.
			//No harm in subscribing for all clinics.
			//5 for WebSchedNewPat
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command=@"INSERT INTO alertsub (UserNum,ClinicNum,Type) 
									SELECT userod.UserNum,0,5
									FROM userod WHERE userod.IsHidden=0
									UNION ALL 
									SELECT userod.UserNum,clinic.ClinicNum,5
									FROM userod 
									INNER JOIN clinic WHERE clinic.IsHidden=0";
				Db.NonQ(command);
			}
			else {//oracle
				command="SELECT UserNum FROM userod WHERE userod.IsHidden=0";
				DataTable tableUserods=Db.GetTable(command);
				//Always subscribe every user to the practice level alerts.
				foreach(DataRow rowUserod in tableUserods.Rows) {
					command="INSERT INTO alertsub (AlertSubNum,UserNum,ClinicNum,Type) "
						+"VALUES("
						+"(SELECT COALESCE(MAX(AlertSubNum),0)+1 FROM alertsub),"
						+rowUserod["UserNum"].ToString()+","
						+"0," //ClinicNum
						+"5)";//WebSchedNewPat
					Db.NonQ(command);
				}
				//Now subscribe every user to every clinic alert.
				command="SELECT ClinicNum FROM clinic WHERE clinic.IsHidden=0";
				DataTable tableClinics=Db.GetTable(command);
				foreach(DataRow rowUserod in tableUserods.Rows) {
					foreach(DataRow rowClinic in tableClinics.Rows) {
						command="INSERT INTO alertsub (AlertSubNum,UserNum,ClinicNum,Type) "
							+"VALUES("
							+"(SELECT COALESCE(MAX(AlertSubNum),0)+1 FROM alertsub),"
							+rowUserod["UserNum"].ToString()+","
							+rowClinic["ClinicNum"].ToString()+","
							+"5)";//WebSchedNewPat
						Db.NonQ(command);
					}
				}
			}
			//Add WikiAdmin permission to groups with existing permission of SecurityAdmin------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=24";//SecurityAdmin
			table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",148)";//WikiAdmin
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",148,0)";//WikiAdmin
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE wikipage ADD IsLocked tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE wikipage ADD IsLocked number(3)";
				Db.NonQ(command);
				command="UPDATE wikipage SET IsLocked = 0 WHERE IsLocked IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE wikipage MODIFY IsLocked NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('AllowProcAdjFromClaim','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'AllowProcAdjFromClaim','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE userquery ADD IsReleased tinyint DEFAULT 0";
				Db.NonQ(command);
				command="UPDATE userquery SET IsReleased = 1";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE userquery ADD IsReleased number(3)";
				Db.NonQ(command);
				command="UPDATE userquery SET IsReleased = 1 WHERE IsReleased IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE userquery MODIFY IsReleased NOT NULL";
				Db.NonQ(command);
			}
			//Add PayPlanEdit permission to groups with existing AccountModule permission------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=3";  //AccountModule
			table = Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",149)";  //PayPlanEdit
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",149,0)";  //PayPlanEdit
					Db.NonQ(command);
				}
			}
			//Add CommandQuery permission to groups with existing UserQueryAdmin permission------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType = 87";  //UserQueryAdmin
			table = Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",151)";  //CommandQuery
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",151,0)";  //CommandQuery
					Db.NonQ(command);
				}
			}
			//Now change the UserQueryAdmin permission and give everyone with UserQuery permission UserQueryAdmin permission as well---------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType = 19";  //Get all with UserQuery permission
			table = Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				//if they do not have the UserQueryAdmin permission already...
				command="SELECT COUNT(*) FROM grouppermission WHERE grouppermission.PermType = 87 AND grouppermission.UserGroupNum = " + groupNum;
				int count = PIn.Int(Db.GetCount(command));
				if(count == 0) {
					//if the group does not have UserQueryAdmin
					if(DataConnection.DBtype==DatabaseType.MySql) {
						//insert it.
						command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES("+POut.Long(groupNum)+",87)"; //UserQueryAdmin
						Db.NonQ(command);
					}
					else {
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",87,0)";  //UserQueryAdmin
						Db.NonQ(command);
					}
				}
			}
			command = "SELECT COALESCE(MAX(displayreport.ItemOrder),0) FROM displayreport WHERE displayreport.Category = 4";//PublicHealth
			itemorder = Db.GetInt(command) + 1;
			command="UPDATE displayreport SET Category=4,ItemOrder="+itemorder+" WHERE displayreport.InternalName = 'ODDentalSealantMeasure'";
			Db.NonQ(command);
			//Add AptDateTimeOrig to confirmation request table------------------------------------------------------
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE confirmationrequest ADD AptDateTimeOrig datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE confirmationrequest ADD AptDateTimeOrig date";
				Db.NonQ(command);
				command="UPDATE confirmationrequest SET AptDateTimeOrig = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE AptDateTimeOrig IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE confirmationrequest MODIFY AptDateTimeOrig NOT NULL";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.3.1 - Changing userquery | QueryText to mediumtext");//No translation in convert script.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE userquery MODIFY QueryText mediumtext NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				//oracle QueryText data type is already clob which can handle up to 4GB of data.
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.3.1");//No translation in convert script.
			//Alert WebSchedNewPatApptCreated
			//Populate AlertSub table to encourage usage.
			//Subscribe all clinics, and subscribe all users for clinics they have access to
			//6 for WebSchedNewPatApptCreated
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command=@"INSERT INTO alertsub (UserNum,ClinicNum,Type) 
									SELECT userod.UserNum,userod.ClinicNum,6
									FROM userod WHERE userod.IsHidden=0
									UNION
									SELECT userod.UserNum,clinic.ClinicNum,6
									FROM userod 
									INNER JOIN userclinic ON userod.UserNum=userclinic.UserNum
									INNER JOIN clinic ON userclinic.ClinicNum=clinic.ClinicNum
									WHERE clinic.IsHidden=0
									UNION
									SELECT userod.UserNum,clinic.ClinicNum,6
									FROM userod
									LEFT JOIN clinic ON userod.ClinicNum=0
									WHERE clinic.IsHidden=0 AND userod.ClinicIsRestricted=0
									ORDER BY UserNum,ClinicNum";
				Db.NonQ(command);
			}
			else {//oracle
				command="SELECT UserNum FROM userod WHERE userod.IsHidden=0";
				DataTable tableUserods=Db.GetTable(command);
				//Always subscribe every user to the practice level alerts.
				foreach(DataRow rowUserod in tableUserods.Rows) {
					command="INSERT INTO alertsub (AlertSubNum,UserNum,ClinicNum,Type) "
						+"VALUES("
						+"(SELECT COALESCE(MAX(AlertSubNum),0)+1 FROM alertsub),"
						+rowUserod["UserNum"].ToString()+","
						+"0," //ClinicNum
						+"6)";//WebSchedNewPatApptCreated
					Db.NonQ(command);
				}
				//Now subscribe every user to every clinic alert they have access to
				foreach(DataRow rowUserod in tableUserods.Rows) {
					command=@"SELECT userod.ClinicNum FROM userod LEFT JOIN clinic on userod.ClinicNum=clinic.ClinicNum
										WHERE clinic.IsHidden=0
										UNION
										SELECT clinic.ClinicNum FROM userod LEFT JOIN clinic on userod.ClinicNum=0
										WHERE clinic.IsHidden=0 AND userod.ClinicIsRestricted=0
										UNION
										SELECT clinic.ClinicNum FROM clinic INNER JOIN userclinic ON clinic.ClinicNum=userclinic.ClinicNum 
										WHERE clinic.IsHidden=0 AND userclinic.UserNum="+POut.String(rowUserod["UserNum"].ToString());
					DataTable tableClinics=Db.GetTable(command);
					foreach(DataRow rowClinic in tableClinics.Rows) {
						command="INSERT INTO alertsub (AlertSubNum,UserNum,ClinicNum,Type) "
							+"VALUES("
							+"(SELECT COALESCE(MAX(AlertSubNum),0)+1 FROM alertsub),"
							+rowUserod["UserNum"].ToString()+","
							+rowClinic["ClinicNum"].ToString()+","
							+"6)";//WebSchedNewPatApptCreated
						Db.NonQ(command);
					}
				}
			}
			//Insert new ApptConfirmed status type for 'Created from Web Sched'
			command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=2";
			string maxOrder=Db.GetScalar(command);
			if(maxOrder=="") {
				maxOrder="0";
			}
			long defNum=0;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO definition (Category, ItemOrder, ItemName, ItemValue) VALUES (2,"+maxOrder+",'Created from Web Sched','WebSched')";
				defNum=Db.NonQ(command,true);
			}
			else {//oracle
				command="INSERT INTO definition (DefNum,Category, ItemOrder, ItemName, ItemValue) VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),2,"+maxOrder
					+",'Created from Web Sched','WebSched')";
				defNum=Db.NonQ(command,true,"DefNum","definition");
			}
			//Insert new preference WebSchedNewPatConfirmStatus
			if(DataConnection.DBtype==DatabaseType.MySql) {
				 command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedNewPatConfirmStatus','"+POut.Long(defNum)+"')";
				 Db.NonQ(command);
			}
			else {//oracle
				 command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedNewPatConfirmStatus','"+POut.Long(defNum)+"')";
				 Db.NonQ(command);
			}
			//Add the Web Sched Appointment Report to the displayreport table
			command="SELECT MAX(displayreport.ItemOrder) FROM displayreport WHERE displayreport.Category = 3"; //Lists
			itemorder = Db.GetLong(command)+1; //get the next available ItemOrder for the Daily Category to put this new report last.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command=@"INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden)
					VALUES('ODWebSchedAppointments',"+POut.Long(itemorder)
					+",'Web Sched Appointments',3,0)";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO displayreport(DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) "
					+"VALUES((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODWebSchedAppointments',"+POut.Long(itemorder)
					+",'Web Sched Appointments',3,0)";
				Db.NonQ(command);
			}
			//Add permission for this report to everybody with Appointments report permission
			command="SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODAppointments'";
			long reportApptsNum=Db.GetLong(command);
			command="SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODWebSchedAppointments'";
			long reportWebSchedApptsNum=Db.GetLong(command);
			//Get a table of UserGroupNum that have permissions to Reports and to the Appointments report.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command=@"INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) 
							SELECT '0001-01-01',0,grouppermission.UserGroupNum,22,"+POut.Long(reportWebSchedApptsNum)//22 is PermType.Reports
							+@" FROM grouppermission WHERE PermType=22 AND FKey="+POut.Long(reportApptsNum);
				Db.NonQ(command);
			}
			else {
				command="SELECT UserGroupNum FROM grouppermission WHERE PermType=22 AND FKey="+POut.Long(reportApptsNum);
				userGroupTable=Db.GetTable(command);
				foreach(DataRow row in userGroupTable.Rows) {
					command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission)"
						+","+"TO_DATE('0001-01-01','YYYY-MM-DD'),0,"+POut.Long(PIn.Long(row["UserGroupNum"].ToString()))+",22,"
						+POut.Long(reportWebSchedApptsNum)+")";
					Db.NonQ(command);
				}
			}
			//Preference that forces a user to set their password on next login-------------------------------------- 
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE userod ADD IsPasswordResetRequired tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE userod ADD IsPasswordResetRequired number(3)";
				Db.NonQ(command);
				command="UPDATE userod SET IsPasswordResetRequired = 0 WHERE IsPasswordResetRequired IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE userod MODIFY IsPasswordResetRequired NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS deflink";
				Db.NonQ(command);
				command=@"CREATE TABLE deflink (
						DefLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
						DefNum bigint NOT NULL,
						FKey bigint NOT NULL,
						LinkType tinyint NOT NULL,
						INDEX(DefNum),
						INDEX(FKey)
						) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE deflink'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE deflink (
						DefLinkNum number(20) NOT NULL,
						DefNum number(20) NOT NULL,
						FKey number(20) NOT NULL,
						LinkType number(3) NOT NULL,
						CONSTRAINT deflink_DefLinkNum PRIMARY KEY (DefLinkNum)
						)";
				Db.NonQ(command);
				command=@"CREATE INDEX deflink_DefNum ON deflink (DefNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX deflink_FKey ON deflink (FKey)";
				Db.NonQ(command);
			}
			//Move all clinic specialty to the new deflink table. 
			command="SELECT Specialty,ClinicNum FROM clinic WHERE Specialty>0";
			long clinicNum;
			table = Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					clinicNum=PIn.Long(row["ClinicNum"].ToString());
					defNum=PIn.Long(row["Specialty"].ToString());
					command="INSERT INTO deflink (DefNum,FKey,LinkType) "
							+"VALUES("+POut.Long(defNum)+","+POut.Long(clinicNum)+",0)";//DefLinkType.Clinic
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					clinicNum=PIn.Long(row["ClinicNum"].ToString());
					defNum=PIn.Long(row["Specialty"].ToString());
					command="INSERT INTO deflink (DefLinkNum,DefNum,FKey,LinkType) "
							+"VALUES((SELECT COALESCE(MAX(DefLinkNum),0)+1 FROM deflink),"+POut.Long(defNum)+","+POut.Long(clinicNum)+",0)"; //DefLinkType.Clinic
					Db.NonQ(command);
				}
			}
			//Assign specialties to every possible cloned patient
			command=@"
				SELECT patient.PatNum,patient.ClinicNum,deflink.DefNum
				FROM patient 
				INNER JOIN patientlink ON patient.PatNum=patientlink.PatNumTo AND patientlink.LinkType=2
				INNER JOIN deflink ON deflink.FKey=patient.ClinicNum AND deflink.LinkType=0";
			table=Db.GetTable(command);
			long patNum;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					patNum=PIn.Long(row["PatNum"].ToString());
					defNum=PIn.Long(row["DefNum"].ToString());
					command="INSERT INTO deflink (DefNum,FKey,LinkType) "
							+"VALUES("+POut.Long(defNum)+","+POut.Long(patNum)+",1)";//DefLinkType.Patient
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					patNum=PIn.Long(row["PatNum"].ToString());
					defNum=PIn.Long(row["DefNum"].ToString());
					command="INSERT INTO deflink (DefLinkNum,DefNum,FKey,LinkType) "
							+"VALUES((SELECT COALESCE(MAX(DefLinkNum),0)+1 FROM deflink),"+POut.Long(defNum)+","+POut.Long(patNum)+",1)"; //DefLinkType.Patient
					Db.NonQ(command);
				}
			}
			//remove specialty column from clinic. The specialty column was introduce in 17.2. Clinic specialties are now stored in the deflink table.
			command="ALTER TABLE clinic DROP COLUMN Specialty";//Oracle compatible
			Db.NonQ(command);
			//InsAgingReport ---------------------------------------------------------------------------------------------------------
			command="SELECT MAX(displayreport.ItemOrder) FROM displayreport WHERE displayreport.Category = 2"; //Monthly
			itemorder = Db.GetLong(command)+1; //get the next available ItemOrder for the Daily Category to put this new report last.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODInsAging',"+POut.Long(itemorder)
					+",'Insurance Aging Report',2,0)";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO displayreport(DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) "
					+"VALUES((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODInsAging',"+POut.Long(itemorder)
					+",'Insurance Aging Report',2,0)";
				Db.NonQ(command);
			}
			long reportNumInsAging=Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODInsAging'");
			//give usergroups with Aging of A/R reports permission access to this new report.
			long reportNumAging=Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODAgingAR'");
			command="SELECT UserGroupNum FROM grouppermission WHERE PermType=22 AND FKey="+POut.Long(reportNumAging);
			userGroupTable=Db.GetTable(command);
			foreach(DataRow row in userGroupTable.Rows) {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//Insert Reports permission (22) for the monthly ODInsAging report.
					command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES('0001-01-01',0,"+POut.Long(PIn.Long(row["UserGroupNum"].ToString()))+",22,"+POut.Long(reportNumInsAging)+")";
					Db.NonQ(command);
				}
				else {//oracle
					//Insert Reports permission (22) for the ODInsAging report.
					command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission)"
						+","+"TO_DATE('0001-01-01','YYYY-MM-DD'),0,"+POut.Long(PIn.Long(row["UserGroupNum"].ToString()))+",22,"
						+POut.Long(reportNumInsAging)+")";
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS alertcategory";
				Db.NonQ(command);
				command=@"CREATE TABLE alertcategory (
					AlertCategoryNum bigint NOT NULL auto_increment PRIMARY KEY,
					IsHQCategory tinyint NOT NULL,
					InternalName varchar(255) NOT NULL,
					Description varchar(255) NOT NULL
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE alertcategory'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE alertcategory (
					AlertCategoryNum number(20) NOT NULL,
					IsHQCategory number(3) NOT NULL,
					InternalName varchar2(255),
					Description varchar2(255),
					CONSTRAINT alertcategory_AlertCategoryNum PRIMARY KEY (AlertCategoryNum)
					)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE alertsub ADD AlertCategoryNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE alertsub ADD INDEX (AlertCategoryNum)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE alertsub ADD AlertCategoryNum number(20)";
				Db.NonQ(command);
				command="UPDATE alertsub SET AlertCategoryNum = 0 WHERE AlertCategoryNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE alertsub MODIFY AlertCategoryNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX alertsub_AlertCategoryNum ON alertsub (AlertCategoryNum)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS alertcategorylink";
				Db.NonQ(command);
				command=@"CREATE TABLE alertcategorylink (
					AlertCategoryLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
					AlertCategoryNum bigint NOT NULL,
					AlertType tinyint NOT NULL,
					INDEX(AlertCategoryNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE alertcategorylink'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE alertcategorylink (
					AlertCategoryLinkNum number(20) NOT NULL,
					AlertCategoryNum number(20) NOT NULL,
					AlertType number(3) NOT NULL,
					CONSTRAINT alertcategorylink_AlertCategor PRIMARY KEY (AlertCategoryLinkNum)
					)";
				Db.NonQ(command);
				command=@"CREATE INDEX alertcategorylink_ACN ON alertcategorylink (AlertCategoryNum)";
				Db.NonQ(command);
			}
			//Create inital AlertCategory
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategory (IsHQCategory,InternalName,Description) VALUES(1,'OdAllTypes','All')";
			}
			else {//oracle
				command="INSERT INTO alertcategory (AlertCategoryNum,IsHQCategory,InternalName,Description) VALUES((SELECT COALESCE(MAX(AlertCategoryNum),0)+1 FROM alertcategory),1,'OdAllTypes','All')";
			}
			string alertCategoryNum=POut.Long(Db.NonQ(command,true,"AlertCategoryNum","alertcategory"));
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",0),"+//Generic
				"("+alertCategoryNum+",1),"+//OnlinePaymentsPending
				"("+alertCategoryNum+",2),"+//VoiceMailMonitor
				"("+alertCategoryNum+",3),"+//RadiologyProcedures
				"("+alertCategoryNum+",4),"+//CallbackRequested
				"("+alertCategoryNum+",5)"; //WebSchedNewPat
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT COALESCE(MAX(AlertCategoryLinkNum),0)+1 FROM alertcategorylink),"+alertCategoryNum+",0)";//Generic
				Db.NonQ(command);
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT COALESCE(MAX(AlertCategoryLinkNum),0)+1 FROM alertcategorylink),"+alertCategoryNum+",1)";//OnlinePaymentsPending
				Db.NonQ(command);
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT COALESCE(MAX(AlertCategoryLinkNum),0)+1 FROM alertcategorylink),"+alertCategoryNum+",2)";//VoiceMailMonitor
				Db.NonQ(command);
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT COALESCE(MAX(AlertCategoryLinkNum),0)+1 FROM alertcategorylink),"+alertCategoryNum+",3)";//RadiologyProcedures
				Db.NonQ(command);
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT COALESCE(MAX(AlertCategoryLinkNum),0)+1 FROM alertcategorylink),"+alertCategoryNum+",4)";//CallbackRequested
				Db.NonQ(command);
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT COALESCE(MAX(AlertCategoryLinkNum),0)+1 FROM alertcategorylink),"+alertCategoryNum+",5)";//WebSchedNewPat
				Db.NonQ(command);
			}
			//Move old alert subs to use new alertCategory structure.
			//If no user nums are returned then customer does not use alerts so skip.
			command="SELECT UserNum from alertsub";
			List<long> listSubUserNums=Db.GetListLong(command).Distinct().ToList();
			if(listSubUserNums.Count>0) {
				//Populate AlertSub table to encourage usage.
				//No harm in subscribing for all clinics.
				//Associate all to new alertCategory.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO alertsub (UserNum,ClinicNum,AlertCategoryNum) "+
										"SELECT userod.UserNum,0,"+alertCategoryNum+" "+
										"FROM userod "+
										"WHERE userod.UserNum IN ( "+
										string.Join(",",listSubUserNums)+
										") "+
										"UNION ALL "+
										"SELECT userod.UserNum,clinic.ClinicNum,"+alertCategoryNum+" "+
										"FROM userod "+
										"INNER JOIN clinic ON clinic.IsHidden=0 "+
										"WHERE userod.UserNum IN ( "+
										string.Join(",",listSubUserNums)+
										") ";
					Db.NonQ(command);
				}
				else {//oracle
					command="SELECT UserNum FROM userod WHERE UserNum IN ("+string.Join(",",listSubUserNums)+")";
					DataTable tableUserods=Db.GetTable(command);
					//Always subscribe every user to the practice level alerts.
					foreach(DataRow rowUserod in tableUserods.Rows) {
						command="INSERT INTO alertsub (AlertSubNum,UserNum,ClinicNum,AlertCategoryNum,Type) "
							+"VALUES("
							+"(SELECT COALESCE(MAX(AlertSubNum),0)+1 FROM alertsub),"
							+rowUserod["UserNum"].ToString()+","
							+"0," //ClinicNum
							+alertCategoryNum+",0)";
						Db.NonQ(command);
					}
					//Now subscribe every user to every clinic alert.
					command="SELECT ClinicNum FROM clinic WHERE IsHidden=0";
					DataTable tableClinics=Db.GetTable(command);
					foreach(DataRow rowUserod in tableUserods.Rows) {
						foreach(DataRow rowClinic in tableClinics.Rows) {
							command="INSERT INTO alertsub (AlertSubNum,UserNum,ClinicNum,AlertCategoryNum,Type) "
								+"VALUES("
								+"(SELECT COALESCE(MAX(AlertSubNum),0)+1 FROM alertsub),"
								+rowUserod["UserNum"].ToString()+","
								+rowClinic["ClinicNum"].ToString()+","
								+alertCategoryNum+",0)";
							Db.NonQ(command);
						}
					}
				}
				//Remove all old alertSub rows
				command="DELETE FROM alertsub WHERE AlertCategoryNum=0";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE timecardrule ADD IsOvertimeExempt tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE timecardrule ADD IsOvertimeExempt number(3)";
				Db.NonQ(command);
				command="UPDATE timecardrule SET IsOvertimeExempt = 0 WHERE IsOvertimeExempt IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE timecardrule MODIFY IsOvertimeExempt NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ClaimProcAllowCreditsGreaterThanProcFee','0')";//0 - ClaimProcCreditsGreaterThanProcFee.Allow
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ClaimProcAllowCreditsGreaterThanProcFee','0')";//0 - ClaimProcCreditsGreaterThanProcFee.Allow
				Db.NonQ(command);
			}
			//Add columns to both inverify and insverifyhist
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE insverify ADD DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE insverify ADD DateTimeEntry date";
				Db.NonQ(command);
				command="UPDATE insverify SET DateTimeEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateTimeEntry IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE insverify MODIFY DateTimeEntry NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE insverify ADD HoursAvailableForVerification double NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE insverify ADD HoursAvailableForVerification number(38,8)";
				Db.NonQ(command);
				command="UPDATE insverify SET HoursAvailableForVerification = 0 WHERE HoursAvailableForVerification IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE insverify MODIFY HoursAvailableForVerification NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE insverifyhist ADD DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE insverifyhist ADD DateTimeEntry date";
				Db.NonQ(command);
				command="UPDATE insverifyhist SET DateTimeEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateTimeEntry IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE insverifyhist MODIFY DateTimeEntry NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE insverifyhist ADD HoursAvailableForVerification double NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE insverifyhist ADD HoursAvailableForVerification number(38,8)";
				Db.NonQ(command);
				command="UPDATE insverifyhist SET HoursAvailableForVerification = 0 WHERE HoursAvailableForVerification IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE insverifyhist MODIFY HoursAvailableForVerification NOT NULL";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.3.1 - Adding fee.ClinicNum index");//No translation in convert script.
			try {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					if(!LargeTableHelper.IndexExists("fee","ClinicNum")) {//add index if it doesn't already exist
						command="ALTER TABLE fee ADD INDEX ClinicNum (ClinicNum)";
						Db.NonQ(command);
					}
				}
				else {//oracle
					command="CREATE INDEX fee_ClinicNum ON fee (ClinicNum)";
					Db.NonQ(command);
				}
			}
			catch(Exception) { }//Only an index.
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.3.1");//No translation in convert script.
			//Add the new payconnect ForceRecurringCharge property---------------------------------------------------
			command="SELECT ProgramNum FROM program WHERE ProgName='PayConnect'";
			long progNum=PIn.Long(Db.GetScalar(command));
			command="SELECT ClinicNum FROM clinic";
			List<long> listClinicNums=Db.GetListLong(command);
			listClinicNums.Add(0);//Include a property for headquarters.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(long cNum in listClinicNums) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
						+"VALUES ("+POut.Long(progNum)+",'PayConnectForceRecurringCharge','0','',"+POut.Long(cNum)+")";
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(long cNum in listClinicNums) {
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
						+"VALUES ((SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
						+POut.Long(progNum)+",'PayConnectForceRecurringCharge','0','',"+POut.Long(cNum)+")";
					Db.NonQ(command);
				}
			}
			//Add the new xcharge ForceRecurringCharge property------------------------------------------------------
			command="SELECT ProgramNum FROM program WHERE ProgName='Xcharge'";
			progNum=PIn.Long(Db.GetScalar(command));
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(long cNum in listClinicNums) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
						+"VALUES ("+POut.Long(progNum)+",'XChargeForceRecurringCharge','0','',"+POut.Long(cNum)+")";
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(long cNum in listClinicNums) {
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
						+"VALUES ((SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
						+POut.Long(progNum)+",'XChargeForceRecurringCharge','0','',"+POut.Long(cNum)+")";
					Db.NonQ(command);
				}
			}
			//set claimsnapshot enabled for all customers starting in this version.
			command="UPDATE preference SET ValueString = 1 WHERE PrefName LIKE 'ClaimSnapshotEnabled'";
			Db.NonQ(command);
			//Add new BillingShowTransSinceBalZero preference
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES ('BillingShowTransSinceBalZero','0')";
				Db.NonQ(command);
			}
			else {
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'BillingShowTransSinceBalZero','0')";
				Db.NonQ(command);
			}
			//Set the blocked task list preferences for users who block either thier inbox or all popups currently---
			if(DataConnection.DBtype==DatabaseType.MySql) {
				//Add task list block preferences to database
				command="SELECT UserNum FROM userod WHERE DefaultHidePopups=1";
				List<long> listBlockDefault = Db.GetListLong(command);
				command="SELECT UserNum FROM userod WHERE InboxHidePopups=1";
				List<long> listBlockInbox = Db.GetListLong(command);
				//Go through the user that wish to block their inbox
				foreach(long userNum in listBlockInbox) {
					command="SELECT TaskListInBox FROM userod WHERE UserNum="+POut.Long(userNum);
					long inboxNum=Db.GetLong(command);
					command="INSERT INTO userodpref (UserNum,Fkey,FkeyType,ValueString) "
						+"VALUES ("+POut.Long(userNum)+","+POut.Long(inboxNum)+",'10', '1')";
					if(inboxNum==0) {
						continue;
					}
					Db.NonQ(command);
				}
				//Get all task lists
				command="SELECT TaskListNum,Parent FROM tasklist";
				table=Db.GetTable(command);
				Dictionary<long,List<long>> dictAllTaskLists=new Dictionary<long, List<long>>();
				//Build dictionary of all tasklists and their children.
				foreach(DataRow row in table.Rows) {
					long taskListNum=PIn.Long(row["TaskListNum"].ToString());
					List<long> listChildTaskLists=new List<long>();
					//Get all tasklistnum where row[parent] equals the current tasklistnum
					listChildTaskLists=table.Select().Where(x => x["Parent"].ToString()==taskListNum.ToString()).Select(x => PIn.Long(x["TaskListNum"].ToString())).ToList();
					dictAllTaskLists[taskListNum]=listChildTaskLists;
				}
				//Go through the users that wish to block all subscriptions
				foreach(long userNum in listBlockDefault) {
					command="SELECT TaskListNum from tasksubscription WHERE UserNum="+POut.Long(userNum);
					List<long> listTaskListsToBlock=Db.GetListLong(command);
					for(int i = 0;i<listTaskListsToBlock.Count;i++) {
						if(listTaskListsToBlock[i]==0) {
							continue;
						}
						if(dictAllTaskLists.ContainsKey(listTaskListsToBlock[i])) {
							listTaskListsToBlock.AddRange(dictAllTaskLists[listTaskListsToBlock[i]]);
						}
					}
					//Do a while loop and always just add the 0 index item.
					//After it's added, we remove all elements that match that number, giving us a new 0 index num.
					while(listTaskListsToBlock.Count>0) {
						command="INSERT INTO userodpref (UserNum,Fkey,FkeyType,ValueString) "
							+"VALUES ('"+POut.Long(userNum)+"','"+POut.Long(listTaskListsToBlock[0])+"','10', '1')"; //TaskListBlock; true;
						Db.NonQ(command);
						listTaskListsToBlock.RemoveAll(x => x==listTaskListsToBlock[0]);
					}
				}
			}
			//Create eServices AlertCategory----------------------------------------------------------------------------------------------------------------
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategory (IsHQCategory,InternalName,Description) VALUES(1,'eServices','eServices')";
			}
			else {//oracle
				command="INSERT INTO alertcategory (AlertCategoryNum,IsHQCategory,InternalName,Description) "
					+"VALUES((SELECT MAX(AlertCategoryNum)+1 FROM alertcategory),1,'eServices','eServices')";
			}
			alertCategoryNum=POut.Long(Db.NonQ(command,true,"AlertCategoryNum","alertcategory"));
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",4)";//CallbackRequested
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",4)";//CallbackRequested
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",5)";//WebSchedNewPat
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",5)";//WebSchedNewPat
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",6)";//WebSchedNewPatApptCreated
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",6)";//WebSchedNewPatApptCreated
				Db.NonQ(command);
			}
			//Subscribe all users for clinics they have access to and for headquarters if they are not restricted to a clinic.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command=@"INSERT INTO alertsub (UserNum,ClinicNum,AlertCategoryNum) 
									SELECT userod.UserNum,userod.ClinicNum,"+alertCategoryNum+@"
									FROM userod 
									WHERE userod.IsHidden=0
									UNION
									SELECT userod.UserNum,clinic.ClinicNum,"+alertCategoryNum+@"
									FROM userod 
									INNER JOIN userclinic ON userod.UserNum=userclinic.UserNum
									INNER JOIN clinic ON userclinic.ClinicNum=clinic.ClinicNum
									WHERE clinic.IsHidden=0
									AND userod.IsHidden=0
									UNION
									SELECT userod.UserNum,clinic.ClinicNum,"+alertCategoryNum+@"
									FROM userod
									LEFT JOIN clinic ON userod.ClinicNum=0
									WHERE clinic.IsHidden=0 AND userod.ClinicIsRestricted=0
									UNION
									SELECT userod.UserNum,0,"+alertCategoryNum+@"
									FROM userod
									WHERE userod.IsHidden=0 AND userod.ClinicIsRestricted=0";
				Db.NonQ(command);
			}
			else {//oracle
				command=@"SELECT userod.UserNum,userod.ClinicNum,"+alertCategoryNum+@"
									FROM userod 
									WHERE userod.IsHidden=0
									UNION
									SELECT userod.UserNum,clinic.ClinicNum,"+alertCategoryNum+@"
									FROM userod 
									INNER JOIN userclinic ON userod.UserNum=userclinic.UserNum
									INNER JOIN clinic ON userclinic.ClinicNum=clinic.ClinicNum
									WHERE clinic.IsHidden=0
									AND userod.IsHidden=0
									UNION
									SELECT userod.UserNum,clinic.ClinicNum,"+alertCategoryNum+@"
									FROM userod
									LEFT JOIN clinic ON userod.ClinicNum=0
									WHERE clinic.IsHidden=0 AND userod.ClinicIsRestricted=0
									UNION
									SELECT userod.UserNum,0,"+alertCategoryNum+@"
									FROM userod
									WHERE userod.IsHidden=0 AND userod.ClinicIsRestricted=0";
				DataTable tableUsersClinics=Db.GetTable(command);
				foreach(DataRow rowUserClinic in tableUsersClinics.Rows) {
					command="INSERT INTO alertsub (AlertSubNum,UserNum,ClinicNum,AlertCategoryNum,Type) "
						+"VALUES("
						+"(SELECT COALESCE(MAX(AlertSubNum),0)+1 FROM alertsub),"
						+rowUserClinic["UserNum"].ToString()+","
						+rowUserClinic["ClinicNum"].ToString()+","
						+alertCategoryNum+",0)";
					Db.NonQ(command);
				}
			}
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes' AND IsHQCategory=1";
			alertCategoryNum=Db.GetScalar(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",6)";//WebSchedNewPatApptCreated
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",6)";//WebSchedNewPatApptCreated
				Db.NonQ(command);
			}
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='eServices' AND IsHQCategory=1";
			alertCategoryNum=Db.GetScalar(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",7)";//NumberBarredFromTexting
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",7)";//NumberBarredFromTexting
				Db.NonQ(command);
			}
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes' AND IsHQCategory=1";
			alertCategoryNum=Db.GetScalar(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",7)";//NumberBarredFromTexting
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",7)";//NumberBarredFromTexting
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE insverifyhist ADD VerifyUserNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE insverifyhist ADD INDEX (VerifyUserNum)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE insverifyhist ADD VerifyUserNum number(20)";
				Db.NonQ(command);
				command="UPDATE insverifyhist SET VerifyUserNum = 0 WHERE VerifyUserNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE insverifyhist MODIFY VerifyUserNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX insverifyhist_VerifyUserNum ON insverifyhist (VerifyUserNum)";
				Db.NonQ(command);
			}
			//Add ReplicationSetup permission to groups with existing SecurityAdmin permission-------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=24";  //SecurityAdmin
			table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(groupNum)+",152)";  //ReplicationSetup
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
						 +"VALUES((SELECT COALESCE(MAX(GroupPermNum),0)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",152,0)";  //ReplicationSetup
					Db.NonQ(command);
				}
			}
			//ArManager preferences for filtering defaults
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ArManagerBillingTypes','')";//empty string means default is All billing types
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ArManagerExcludeBadAddresses','1')";//default to exclude if bad address (no zip)
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ArManagerExcludeIfUnsentProcs','0')";//default to include if unsent procs
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ArManagerExcludeInsPending','0')";//default to include if ins pending
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ArManagerLastTransTypes','')";//empty string means default is All trans types
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ArManagerSentAgeOfAccount','90')";//default to 90 days, user can change later
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ArManagerSentDaysSinceLastPay','90')";//default to 90 days since last payment
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ArManagerSentMinBal','25.00')";//default to min balance of $25
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ArManagerUnsentAgeOfAccount','90')";//default to 90 days, user can change later
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ArManagerUnsentDaysSinceLastPay','90')";//default to 90 days since last payment
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ArManagerUnsentMinBal','25.00')";//default to min balance of $25
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ArManagerBillingTypes','')";//empty string means default is All billing types
				Db.NonQ(command);
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ArManagerExcludeBadAddresses','1')";//default to exclude if bad address (no zip)
				Db.NonQ(command);
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ArManagerExcludeIfUnsentProcs','0')";//default to include if unsent procs
				Db.NonQ(command);
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ArManagerExcludeInsPending','0')";//default to include if ins pending
				Db.NonQ(command);
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ArManagerLastTransTypes','')";//empty string means default is All trans types
				Db.NonQ(command);
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ArManagerSentAgeOfAccount','90')";//default to 90 days, user can change later
				Db.NonQ(command);
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ArManagerSentDaysSinceLastPay','90')";//default to 90 days since last payment
				Db.NonQ(command);
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ArManagerSentMinBal','25.00')";//default to min balance of $25
				Db.NonQ(command);
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ArManagerUnsentAgeOfAccount','90')";//default to 90 days, user can change later
				Db.NonQ(command);
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ArManagerUnsentDaysSinceLastPay','90')";//default to 90 days since last payment
				Db.NonQ(command);
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ArManagerUnsentMinBal','25.00')";//default to min balance of $25
				Db.NonQ(command);
			}
			//Transworld program property ability to disable advertising
			command="SELECT ProgramNum FROM program WHERE ProgName='Transworld'";
			long tsiProgNum=Db.GetLong(command);
			command="SELECT DISTINCT ClinicNum FROM programproperty WHERE ProgramNum="+POut.Long(tsiProgNum);
			List<long> listProgClinicNums=Db.GetListLong(command);
			for(int i=0;i<listProgClinicNums.Count;i++) {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
						+"VALUES ("+POut.Long(tsiProgNum)+","
							+"'Disable Advertising',"
							+"'0',"
							+"'',"
							+POut.Long(listProgClinicNums[i])+")";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
						+"VALUES ((SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
							+POut.Long(tsiProgNum)+","
							+"'Disable Advertising',"
							+"'0',"
							+"'',"
							+POut.Long(listProgClinicNums[i])+")";
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE tsitranslog ADD ClientId varchar(25) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE tsitranslog ADD ClientId varchar2(25)";
				Db.NonQ(command);
			}
			//Reverted the below schema change as it causes major issues with large customers using Gallera (NADG).
			//ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.3.1 - Increase text to mediumtext");//No translation in convert script.
			//if(DataConnection.DBtype==DatabaseType.MySql) {
			//	command="ALTER TABLE task MODIFY Descript mediumtext NOT NULL";
			//	Db.NonQ(command);
			//	command="ALTER TABLE tasknote MODIFY Note mediumtext NOT NULL";
			//	Db.NonQ(command);
			//	command="ALTER TABLE commlog MODIFY Note mediumtext NOT NULL";
			//	Db.NonQ(command);
			//}
			//else {//oracle
			//	//oracle data type is already clob which can handle up to 4GB of data.
			//}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('RigorousAccounting','1')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'RigorousAccounting','1')";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.3.1");//No translation in convert script.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE sheetfield ADD IsLocked tinyint NOT NULL";
				Db.NonQ(command);
				command="UPDATE sheetfield SET IsLocked = 0 WHERE IsLocked IS NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE sheetfield ADD IsLocked number(3)";
				Db.NonQ(command);
				command="UPDATE sheetfield SET IsLocked = 0 WHERE IsLocked IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE sheetfield MODIFY IsLocked NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE sheetfielddef ADD IsLocked tinyint NOT NULL";
				Db.NonQ(command);
				command="UPDATE sheetfielddef SET IsLocked = 0 WHERE IsLocked IS NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE sheetfielddef ADD IsLocked number(3)";
				Db.NonQ(command);
				command="UPDATE sheetfielddef SET IsLocked = 0 WHERE IsLocked IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE sheetfielddef MODIFY IsLocked NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES ('ElectronicRxClinicUseSelected','0')";
				Db.NonQ(command);
			}
			else {
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ElectronicRxClinicUseSelected','0')";
				Db.NonQ(command);
			}
		}//End method To17_3_1

		private static void To17_3_6() {
			string command="";
			try {
				Db.NonQ("SELECT DateTimeOriginal FROM task"+(DataConnection.DBtype==DatabaseType.MySql ? " LIMIT 1" : ""));
			}
			catch {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE task ADD DateTimeOriginal datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE task ADD DateTimeOriginal date";
					Db.NonQ(command);
					command="UPDATE task SET DateTimeOriginal = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateTimeOriginal IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE task MODIFY DateTimeOriginal NOT NULL";
					Db.NonQ(command);
				}
			}
			try {
				Db.NonQ("SELECT DateTimeOriginal FROM taskhist"+(DataConnection.DBtype==DatabaseType.MySql ? " LIMIT 1" : ""));
			}
			catch {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE taskhist ADD DateTimeOriginal datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE taskhist ADD DateTimeOriginal date";
					Db.NonQ(command);
					command="UPDATE taskhist SET DateTimeOriginal = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateTimeOriginal IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE taskhist MODIFY DateTimeOriginal NOT NULL";
					Db.NonQ(command);
				}
			}
		}

		private static void To17_3_8() {
			string command;
			DataTable table;
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.3.8 - Adding claimproc.DateSuppReceived index");//No translation in convert script.
			try {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					if(!LargeTableHelper.IndexExists("claimproc","DateSuppReceived")) {//add index if it doesn't already exist
						command="ALTER TABLE claimproc ADD INDEX (DateSuppReceived)";
						Db.NonQ(command);
					}
				}
				else {//oracle
					command="CREATE INDEX claimproc_DateSuppReceived ON claimproc (DateSuppReceived)";
					Db.NonQ(command);
				}
			}
			catch(Exception) { }//Only an index.
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.3.8 - Adding procedurelog.DateComplete index");//No translation in convert script.
			try {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					if(!LargeTableHelper.IndexExists("procedurelog","DateComplete")) {//add index if it doesn't already exist
						command="ALTER TABLE procedurelog ADD INDEX (DateComplete)";
						Db.NonQ(command);
					}
				}
				else {//oracle
					command="CREATE INDEX procedurelog_DateComplete ON procedurelog (DateComplete)";
					Db.NonQ(command);
				}
			}
			catch(Exception) { }//Only an index.
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.3.8 - Adding patient.SecDateEntry index");//No translation in convert script.
			try {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					if(!LargeTableHelper.IndexExists("patient","SecDateEntry")) {//add index if it doesn't already exist
						command="ALTER TABLE patient ADD INDEX (SecDateEntry)";
						Db.NonQ(command);
					}
				}
				else {//oracle
					command="CREATE INDEX patient_SecDateEntry ON patient (SecDateEntry)";
					Db.NonQ(command);
				}
			}
			catch(Exception) { }//Only an index.
			//Remove duplicate task list blocks from the database
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="SELECT u2.UserOdPrefNum FROM userodpref u1 "
					+"INNER JOIN userodpref u2 ON u1.UserNum=u2.UserNum "
					+"AND u1.Fkey=u2.Fkey AND u1.FkeyType=u2.FkeyType AND u1.UserOdPrefNum!=u2.UserOdPrefNum "
					+"AND u1.ValueString=u2.ValueString "  //only deletes the preference when they match.
					+"WHERE u1.FkeyType=10" //OdUserPref fkey type for task list blocks.
					+" GROUP BY u1.UserNum, u1.Fkey";
			}
			else {//Oracle
				command="SELECT u2.UserOdPrefNum FROM userodpref u1 "
					+"INNER JOIN userodpref u2 ON u1.UserNum=u2.UserNum "
					+"AND u1.Fkey=u2.Fkey AND u1.FkeyType=u2.FkeyType AND u1.UserOdPrefNum!=u2.UserOdPrefNum "
					+"AND DBMS_LOB.SUBSTR(u1.ValueString,1000,1)=DBMS_LOB.SUBSTR(u2.ValueString,1000,1) "//only deletes the preference when they match.
					+"WHERE u1.FkeyType=10 "//OdUserPref fkey type for task list blocks.
					+"GROUP BY u1.UserNum,u1.Fkey,u2.UserOdPrefNum";
			}
			table=Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				command="DELETE FROM userodpref WHERE UserOdPrefNum="+row["UserOdPrefNum"].ToString();
				Db.NonQ(command);
			}
			//The following preference was incorrectly backported to the v17.3.8 method when v17.3.11 was the next version to be released.
			//This means that the following preference was available to users in versions 9, 10, and 11.
			//We are commenting this preference out as of v17.3.12 and moving it to a new convert method so that all users are guaranteed the preference.
			//E.g. HQ was already past v17.3.8 when this preference was added to the v17.3.8 method.
			//-------------------------------------------------------------------------------------------------------------------------------------------
			////Add preference RepeatingChargesBeginDateTime
			//if(DataConnection.DBtype==DatabaseType.MySql) {
			//	command="INSERT INTO preference(PrefName,ValueString) VALUES('RepeatingChargesBeginDateTime','')";
			//	Db.NonQ(command);
			//}
			//else {//oracle
			//	command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"+
			//		"'RepeatingChargesBeginDateTime','')";
			//	Db.NonQ(command);
			//}
			//-------------------------------------------------------------------------------------------------------------------------------------------
		}

		private static void To17_3_12() {
			string command="";
			//The following preference was incorrectly backported to the v17.3.8 method.
			//As a bug fix, we need to check and see if the preference already exists.  If it doesn't, add it.
			command="SELECT COUNT(*) FROM preference WHERE PrefName='RepeatingChargesBeginDateTime'";
			if(Db.GetCount(command)=="0") {
				//Add preference RepeatingChargesBeginDateTime
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('RepeatingChargesBeginDateTime','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"+
						"'RepeatingChargesBeginDateTime','')";
					Db.NonQ(command);
				}
			}
		}

		private static void To17_3_14() {
			string command;			
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('AutoSplitLogic','1')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'AutoSplitLogic','1')";
				Db.NonQ(command);
			}
			#region HQ Only
			//We are running this section of code for HQ only
			//This is very uncommon and normally manual queries should be run instead of doing a convert script.
			command="SELECT ValueString FROM preference WHERE PrefName='DockPhonePanelShow'";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count > 0 && PIn.Bool(table.Rows[0][0].ToString())) {
				//def.Category 45 is JobPriorities
				command="INSERT INTO definition(Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) VALUES('45','2','Medium High','MediumHigh','-32768','0')";
				long defNum=Db.NonQ(command,true);
				command="UPDATE job SET Priority = '"+POut.Long(defNum)+"' WHERE Priority='MediumHigh'";
				Db.NonQ(command);
				command="INSERT INTO definition(Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) VALUES('45','4','Low','Low','-4144960','0')";
				defNum=Db.NonQ(command,true);
				command="UPDATE job SET Priority = '"+POut.Long(defNum)+"' WHERE Priority='Low'";
				Db.NonQ(command);
				command="INSERT INTO definition(Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) VALUES('45','0','Urgent','Urgent','-65536','0')";
				defNum=Db.NonQ(command,true);
				command="UPDATE job SET Priority = '"+POut.Long(defNum)+"' WHERE Priority='Urgent'";
				Db.NonQ(command);
				command="INSERT INTO definition(Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) VALUES('45','3','Normal','Normal,JobDefault,DocumentationDefault','-1','0')";
				defNum=Db.NonQ(command,true);
				command="UPDATE job SET Priority = '"+POut.Long(defNum)+"' WHERE Priority='Normal' OR Priority='Medium'";
				Db.NonQ(command);
				command="INSERT INTO definition(Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) VALUES('45','1','High','High,BugDefault','-32640','0')";
				defNum=Db.NonQ(command,true);
				command="UPDATE job SET Priority = '"+POut.Long(defNum)+"' WHERE Priority='High'";
				Db.NonQ(command);
				command="INSERT INTO definition(Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) VALUES('45','5','On Hold','OnHold','-8355712','0')";
				defNum=Db.NonQ(command,true);
				command="UPDATE job SET Priority = '"+POut.Long(defNum)+"' WHERE Priority='OnHold'";
				Db.NonQ(command);
				//Convert the priority column, currently a string column, over to a bigint.
				command="ALTER TABLE job MODIFY Priority bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE jobreview ADD Hours varchar(255) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE jobquote ADD Hours varchar(255) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE jobquote ADD ApprovedAmount varchar(255) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE jobquote ADD IsCustomerApproved tinyint NOT NULL";
				Db.NonQ(command);
			}
			#endregion
		}

		private static void To17_3_17() {
			string command="SELECT ValueString FROM preference WHERE PrefName='WebSchedAutomaticSendTextSetting'";
			bool isAutoRecallTextingEnabled=(Db.GetScalar(command)=="4");//4 is WebSchedAutomaticSend.SendToText
			command="SELECT ValueString FROM preference WHERE PrefName='WebSchedTextsPerBatch'";
			int textsPerBatch=PIn.Int(Db.GetScalar(command));
			if(!isAutoRecallTextingEnabled && textsPerBatch==25) {
				//We only want to change the texts per batch if it is not currently enabled and they have the default number of 25.
				//25 texts every 10 minutes was causing offices to reach their limit of 675 texts per 24 hours.
				//2 was chosen so that we do not buy them another phone number. If we send 2 texts every 10 minutes, we would send 180 texts in 15 hours
				//which is the default automatic send window.
				command="UPDATE preference SET ValueString='2' WHERE PrefName='WebSchedTextsPerBatch'";
				Db.NonQ(command);
			}
		}

		private static void To17_3_19() {
			string command="SELECT ValueString FROM preference WHERE PrefName='DockPhonePanelShow'";
			//Only run if MySQL because HQ doesn't use Oracle.
			if(DataConnection.DBtype==DatabaseType.MySql && (Db.GetScalar(command)=="1")) {
				//Create an AlertCategoryLink for the new AsteriskServerMonitor AlertType
				command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes'";
				string alertCategoryNum=Db.GetScalar(command);
				if(!string.IsNullOrEmpty(alertCategoryNum)) {
					command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",10)";//AsteriskServerMonitor
					Db.NonQ(command);
				}
				command="INSERT INTO preference (PrefName,ValueString) VALUES('AsteriskServerHeartbeat','')";
				Db.NonQ(command);
			}
		}

		private static void To17_3_21() {
			string command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='eServices' AND IsHQCategory=1";
			string alertCategoryNum=Db.GetScalar(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",11)";//MultipleEConnectors
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",11)";//MultipleEConnectors
				Db.NonQ(command);
			}
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes' AND IsHQCategory=1";
			alertCategoryNum=Db.GetScalar(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",11)";//MultipleEConnectors
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",11)";//MultipleEConnectors
				Db.NonQ(command);
			}
		}

		private static void To17_3_22() {
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE inseditlog ADD Description varchar(255) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE inseditlog ADD Description varchar2(255)";
				Db.NonQ(command);
			}
			//Moving codes to the Obsolete category that were deleted in CDT 2018.
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
								+"VALUES ((SELECT MAX(DefNum)+1 FROM definition),11,'"+POut.String(procCatDescript)+"',"+POut.Int(countCats)+")";//11 is DefCat.ProcCodeCats
					}
					defNum=Db.NonQ(command,true);
				}
				else { //The procedure code category already exists, get the existing defnum
					defNum=PIn.Long(dtDef.Rows[0]["DefNum"].ToString());
				}
				string[] cdtCodesDeleted=new string[] {
					"D5510","D5610","D5620"
				};
				//Change the procedure codes' category to Obsolete.
				command="UPDATE procedurecode SET ProcCat="+POut.Long(defNum)
					+" WHERE ProcCode IN('"+string.Join("','",cdtCodesDeleted.Select(x => POut.String(x)))+"') ";
				Db.NonQ(command);
			}//end United States CDT codes update
		}

		private static void To17_3_23() {
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				string databaseName=Db.GetScalar("SELECT DATABASE()");
				command=@"SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS 
					WHERE TABLE_SCHEMA='"+POut.String(databaseName)+@"'
					AND TABLE_NAME='task' 
					AND COLUMN_NAME='Descript'";
				string dataType=Db.GetScalar(command);
				if(dataType.ToLower()!="text") {
					command="ALTER TABLE task MODIFY Descript text NOT NULL";
					Db.NonQ(command);
				}
				command=@"SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS 
					WHERE TABLE_SCHEMA='"+POut.String(databaseName)+@"'
					AND TABLE_NAME='tasknote' 
					AND COLUMN_NAME='Note'";
				dataType=Db.GetScalar(command);
				if(dataType.ToLower()!="text") {
					command="ALTER TABLE tasknote MODIFY Note text NOT NULL";
					Db.NonQ(command);
				}
				command=@"SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS 
					WHERE TABLE_SCHEMA='"+POut.String(databaseName)+@"'
					AND TABLE_NAME='commlog' 
					AND COLUMN_NAME='Note'";
				dataType=Db.GetScalar(command);
				if(dataType.ToLower()!="text") {
					command="ALTER TABLE commlog MODIFY Note text NOT NULL";
					Db.NonQ(command);
				}
			}
			else {
				//no changes necessary as the Oracle schema was unchanged
			}
		}

		private static void To17_3_28() {
			string command="SELECT ValueString FROM preference WHERE PrefName='ShowFeaturePatientClone'";
			DataTable table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql && table.Rows.Count > 0 && PIn.Bool(table.Rows[0]["ValueString"].ToString())) {
				//Get every single potential clone in the database.
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.3.28 - Clones | Finding Potentials");
				//Our current clone system requires the following fields to be exactly the same: Guarantor,LName,FName, and Birthdate.
				//We have to join on the patientlink table because users could have manually associated patients to their clones / added new clones entirely.
				//The previous time that we tried to convert old clones into the new system did not account for non-alphabetic characters (symbols) in names.
				command=@"SELECT Guarantor,LName,FName,Birthdate
					FROM patient
					LEFT JOIN patientlink patientlinkfrom ON patient.PatNum=patientlinkfrom.PatNumFrom AND patientlinkfrom.LinkType=2
					LEFT JOIN patientlink patientlinkto ON patient.PatNum=patientlinkto.PatNumTo AND patientlinkto.LinkType=2
					WHERE PatStatus=0  -- PatientStatus.Patient
					AND YEAR(Birthdate) > 1880
					AND patientlinkfrom.PatNumFrom IS NULL
					AND patientlinkto.PatNumTo IS NULL
					GROUP BY Guarantor,LName,FName,Birthdate
					HAVING COUNT(*) > 1";
				table=Db.GetTable(command);
				//Now that we have all potential clones from the database, go try and find the corresponding master.
				for(int i=0;i<table.Rows.Count;i++) {
					ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.3.28 - Clones | Considering "+(i+1)+" / "+table.Rows.Count);
					command="SELECT PatNum,LName,FName,Birthdate "
						+"FROM patient "
						+"WHERE PatStatus=0 "//PatientStatus.Patient
						+"AND LName='"+POut.String(table.Rows[i]["LName"].ToString())+"' "
						+"AND FName='"+POut.String(table.Rows[i]["FName"].ToString())+"' "
						+"AND Birthdate="+POut.Date(PIn.Date(table.Rows[i]["Birthdate"].ToString()));
					DataTable tableClones=Db.GetTable(command);
					//There needs to be at least two patients matching the exact name and birthdate in order to even consider them clones of each other.
					if(tableClones.Rows.Count < 2) {
						continue;
					}
					//At this point we know we have at least 2 patients, we need to find the master or original patient and then link the rest to them.
					//The master patient will have at least one lower case character within the last and first name fields.  Clones will have all caps.
					//We have to ignore any non-alphabetic characters (like symbols and spaces).
					DataRow rowMaster=tableClones.Select().FirstOrDefault(x => x["LName"].ToString().Any(y => char.IsLower(y))
						&& x["FName"].ToString().Any(y => char.IsLower(y)));
					List<DataRow> listCloneRows=tableClones.Select().ToList().FindAll(x => x["LName"].ToString().Where(Char.IsLetter).All(y => char.IsUpper(y)
						&& x["FName"].ToString().Where(Char.IsLetter).All(z => char.IsUpper(z))));
					if(rowMaster==null || listCloneRows==null || listCloneRows.Count==0) {
						continue;//Either no master was found or no clones were found (this will happen for true duplicate patients).
					}
					//Now we can make patientlink entries that will associate the master or original patient with the corresponding clones.
					long patNumMaster=PIn.Long(rowMaster["PatNum"].ToString());
					foreach(DataRow rowClone in listCloneRows) {
						long patNumClone=PIn.Long(rowClone["PatNum"].ToString());
						if(patNumMaster==patNumClone) {
							continue;//Do not create a link between the master and themself.
						}
						command="INSERT INTO patientlink (PatNumFrom,PatNumTo,LinkType,DateTimeLink) "
							+"VALUES("+POut.Long(patNumMaster)+","
							+POut.Long(patNumClone)+","
							+"2,"//PatientLinkType.Clone
							+"NOW())";
						Db.NonQ(command);
					}
				}
				//Set the progress bar back to the way it was.
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.3.28");
			}
		}

		private static void To17_3_32() {
			string command;
			command="SELECT COUNT(*) FROM ehrprovkey";
			bool hasEhrProvKeys=PIn.Bool(Db.GetScalar(command));//will be true if 1 or more rows exist in the ehrprovkey table, otherwise false
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('IsAlertRadiologyProcsEnabled','"+POut.Bool(hasEhrProvKeys)+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'IsAlertRadiologyProcsEnabled','"+POut.Bool(hasEhrProvKeys)+"')";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.3.32 - CPOE Radiology Procs Index");//No translation in convert script.
			try {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					if(!LargeTableHelper.IndexExists("procedurelog","AptNum,CodeNum,ProcStatus,IsCpoe,ProvNum")) {//add composite index if it doesn't already exist
						command="ALTER TABLE procedurelog ADD INDEX RadiologyProcs (AptNum,CodeNum,ProcStatus,IsCpoe,ProvNum)";
						Db.NonQ(command);
						if(LargeTableHelper.IndexExists("procedurelog","AptNum")) {//drop redundant index once composite index is successfully added and only if it exists
							command="ALTER TABLE procedurelog DROP INDEX indexAptNum";
							Db.NonQ(command);
						}
					}
				}
				else {//oracle
					command="CREATE INDEX procedurelog_RadiologyProcs ON procedurelog (AptNum,CodeNum,ProcStatus,IsCpoe,ProvNum)";//add composite index
					Db.NonQ(command);
					command="DROP INDEX procedurelog_indexAptNum";//drop redundant index once composite index is successfully added
					Db.NonQ(command);
				}
			}
			catch(Exception) { }//Only an index.
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.3.32");//No translation in convert script.
		}

		private static void To17_3_44() {
			string command;
			//add PaymentWindowDefaultHideSplits preference if it has not already been added.
			command = "SELECT * FROM preference WHERE preference.PrefName = 'PaymentWindowDefaultHideSplits'";
			if(Db.GetTable(command).Rows.Count == 0) {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('PaymentWindowDefaultHideSplits','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'PaymentWindowDefaultHideSplits','0')";
					Db.NonQ(command);
				}
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.3.44");//No translation in convert script.
		}

		private static void To17_3_50() {
			string command="SELECT * FROM program WHERE ProgName IN ('CentralDataStorage','DentalTekSmartOfficePhone','Podium','RapidCall','TransWorld','DentalIntel','PracticeByNumbers')";
			DataTable table=Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				//Change the "Disable Advertising" program property associated to program to the value of 0.
				command="UPDATE programproperty SET PropertyValue='0' WHERE (ProgramNum='"+row["ProgramNum"].ToString()+"' AND PropertyDesc='Disable Advertising')";
				Db.NonQ(command);
			}
			command="UPDATE preference SET ValueString='0001-01-01 00:00:00' WHERE PrefName='ProgramAdditionalFeatures'";//This was the default value when it was originally inserted.
			Db.NonQ(command);
		}

		private static void To17_3_59() {
			string command;
			DataTable table;
			//Delete all ortho age limitation proc code benefits and replace them with one category benefit
			command=@"SELECT procedurecode.CodeNum
				FROM covcat
				INNER JOIN covspan ON covspan.CovCatNum=covcat.CovCatNum
				INNER JOIN procedurecode ON procedurecode.ProcCode BETWEEN covspan.FromCode AND covspan.ToCode
				WHERE covcat.EbenefitCat=9";//9 is EbenefitCategory.Orthodontics
			List<long> listOrthoCodeNums=Db.GetListLong(command);
			command="SELECT CovCatNum FROM covcat WHERE EbenefitCat=9";//9 is EbenefitCategory.Orthodontics
			long orthoCovCatNum=Db.GetLong(command);
			table=new DataTable();
			if(listOrthoCodeNums.Count > 0) {
				command=@"SELECT MAX(benefit.Quantity) Quantity,benefit.PlanNum
					FROM benefit
					WHERE benefit.CodeNum IN("+string.Join(",",listOrthoCodeNums.Select(x => POut.Long(x)))+@")
					AND benefit.BenefitType=5"/*InsBenefitType.Limitations*/+@"
					AND benefit.MonetaryAmt=-1
					AND benefit.PatPlanNum=0
					AND benefit.Percent=-1
					AND benefit.QuantityQualifier=2"/*BenefitQuantity.AgeLimit*/+@"
					GROUP BY benefit.PlanNum";
				table=Db.GetTable(command);
				command=@"DELETE FROM benefit
					WHERE benefit.CodeNum IN("+string.Join(",",listOrthoCodeNums.Select(x => POut.Long(x)))+@")
					AND benefit.BenefitType=5"/*InsBenefitType.Limitations*/+@"
					AND benefit.MonetaryAmt=-1
					AND benefit.PatPlanNum=0
					AND benefit.Percent=-1
					AND benefit.QuantityQualifier=2"/*BenefitQuantity.AgeLimit*/;
				Db.NonQ(command);
			}
			foreach(DataRow row in table.Rows) {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command=@"INSERT INTO benefit(CodeNum,BenefitType,CovCatNum,PlanNum,QuantityQualifier,Quantity,Percent,MonetaryAmt,PatPlanNum) 
						VALUES("
						+"0,"
						+"5,"//InsBenefitType.Limitations
						+POut.Long(orthoCovCatNum)+","
						+POut.Long(PIn.Long(row["PlanNum"].ToString()))+","
						+"2,"//BenefitQuantity.AgeLimit
						+POut.Byte(PIn.Byte(row["Quantity"].ToString()))+","
						+"-1,"
						+"-1,"
						+"0)";
					Db.NonQ(command);
				}
			}
			try {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					if(!LargeTableHelper.IndexExists("adjustment","AdjDate,PatNum")) {
						command="ALTER TABLE adjustment ADD INDEX AdjDatePN (AdjDate, PatNum)";
						Db.NonQ(command);
						command="ALTER TABLE adjustment DROP INDEX AdjDate";//drop redundant index once composite index is successfully added
						Db.NonQ(command);
					}
				}
				else {//oracle
					command="CREATE INDEX adjustment_AdjDatePN ON adjustment (AdjDate, PatNum)";
					Db.NonQ(command);
					command="DROP INDEX adjustment_AdjDate";//drop redundant index once composite index is successfully added
					Db.NonQ(command);
				}
			}
			catch(Exception ex) {
				ex.DoNothing();//Just an index
			}
			//Add AgingReportShowAgePatPayplanPayments preference to make the age patient payments to payment plans checkbox visible.
			//For a specific customer, no UI, defaults to false, enable via query only.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('AgingReportShowAgePatPayplanPayments','0')";//false, unless enabled via query
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'AgingReportShowAgePatPayplanPayments','0')";//false, unless enabled via query
				Db.NonQ(command);
			}
		}

		private static void To17_3_63() {
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ApptConfirmAutoSignedUp','0')";//false by default, eConnector will change it if needed
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ApptConfirmAutoSignedUp','0')";//false by default, eConnector will change it if needed
				Db.NonQ(command);
			}
		}

		private static void To17_3_75() {
			string command;
			//This command was supposed to be run in To16_4_1, but the line Db.NonQ(command); was omitted after the command.
			command="UPDATE patientrace SET CdcrecCode='ASKU-ETHNICITY' WHERE Race=11";//DeclinedToSpecifyEthnicity		
			Db.NonQ(command);//Oracle compatible
		}

		private static void To17_4_1() {
			string command;
			DataTable table;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('AllowEmailCCReceipt','1')";//true by default
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'AllowEmailCCReceipt','1')";//true by default
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="SELECT alertcategorynum FROM alertcategory WHERE InternalName='OdAllTypes'";
				long alertCatNum=Db.GetLong(command);
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCatNum+",8)";//MySQL Max Connections Monitor
				Db.NonQ(command);
			}
			else {
				//Oracle not currently supported for the MySQL max connection monitor.
			}
			//Add AptNum to emailmessage
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.4.1 - Adding AptNum to emailmessage");//No translation in convert script.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE emailmessage ADD AptNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE emailmessage ADD INDEX (AptNum)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE emailmessage ADD AptNum number(20)";
				Db.NonQ(command);
				command="UPDATE emailmessage SET AptNum = 0 WHERE AptNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE emailmessage MODIFY AptNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX emailmessage_AptNum ON emailmessage (AptNum)";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.4.1");//No translation in convert script.
			//Rx and Rx Multi sheet defs have illogical field names.  They reference providers and yet have nothing to do with providers.
			//The following loop is going to update any custom Rx and Rx Multi sheet defs that the customer could have made in prior versions.
			int[] arraySheetTypes=new int[] { 5,20 };//Rx and Rx Multi
			for(int i=1;i<7;i++) {
				//prov.address(1-6).  Only applies to Rx sheetdef types.
				if(i==1) {
					UpdateSheetFieldDefFieldNameForSheetType("prov.address","clinic.address",arraySheetTypes);
					UpdateSheetFieldDefFieldNameForSheetType("prov.cityStateZip","clinic.cityStateZip",arraySheetTypes);
					UpdateSheetFieldDefFieldNameForSheetType("prov.phone","clinic.phone",arraySheetTypes);
				}
				else {
					UpdateSheetFieldDefFieldNameForSheetType("prov.address"+i,"clinic.address"+i,arraySheetTypes);
					UpdateSheetFieldDefFieldNameForSheetType("prov.cityStateZip"+i,"clinic.cityStateZip"+i,arraySheetTypes);
					UpdateSheetFieldDefFieldNameForSheetType("prov.phone"+i,"clinic.phone"+i,arraySheetTypes);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE dunning ADD ClinicNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE dunning ADD INDEX (ClinicNum)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE dunning ADD ClinicNum number(20)";
				Db.NonQ(command);
				command="UPDATE dunning SET ClinicNum = 0 WHERE ClinicNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE dunning MODIFY ClinicNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX dunning_ClinicNum ON dunning (ClinicNum)";
				Db.NonQ(command);
			}
			//Add WebSchedASAPApptCreated alert
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes' AND IsHQCategory=1";
			string alertCategoryNum=Db.GetScalar(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",9)";//WebSchedASAPApptCreated
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",9)";//WebSchedASAPApptCreated
				Db.NonQ(command);
			}
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='eServices' AND IsHQCategory=1";
			alertCategoryNum=Db.GetScalar(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",9)";//WebSchedASAPApptCreated
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",9)";//WebSchedASAPApptCreated
				Db.NonQ(command);
			}
			//Add PreAuthSentEdit permission to groups with existing ClaimSentEdit permission-------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=14";  //ClaimSentEdit
			long groupNum;
			table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(groupNum)+",153)";  //PreAuthSentEdit
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
						 +"VALUES((SELECT COALESCE(MAX(GroupPermNum),0)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",153,0)";  //PreAuthSentEdit
					Db.NonQ(command);
				}
			}
			//Add Priority Column to appointment table
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE appointment ADD Priority tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE appointment ADD Priority number(3)";
				Db.NonQ(command);
				command="UPDATE appointment SET Priority = 0 WHERE Priority IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE appointment MODIFY Priority NOT NULL";
				Db.NonQ(command);
			}
			//Add histappointment.Priority
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.4.1 - add histappointment.Priority");
			if(DataConnection.DBtype==DatabaseType.MySql) {
				LargeTableHelper.AlterLargeTable("histappointment","HistApptNum",new List<Tuple<string,string>> { Tuple.Create("Priority","tinyint NOT NULL") });
			}
			else {//oracle
				command="ALTER TABLE histappointment ADD Priority number(3)";
				Db.NonQ(command);
				command="UPDATE histappointment SET Priority = 0 WHERE Priority IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE histappointment MODIFY Priority NOT NULL";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.4.1");
			//Update appointment table to use new Priority column instead
			command=@"UPDATE appointment SET Priority=1 WHERE AptStatus=4"; //1- ASAP
			Db.NonQ(command);
			command=@"UPDATE appointment SET AptStatus=1 WHERE AptStatus=4"; //1 - Scheduled
			Db.NonQ(command);
			//Add Priority column to recall table
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE recall ADD Priority tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE recall ADD Priority number(3)";
				Db.NonQ(command);
				command="UPDATE recall SET Priority = 0 WHERE Priority IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE recall MODIFY Priority NOT NULL";
				Db.NonQ(command);
			}
			//Remove the Appt Status displayfield from category in case they had it enabled
			command="DELETE FROM displayfield WHERE InternalName='Appt Status' AND Category=9"; //9 - Appointment Bubble
			Db.NonQ(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('DisplayRenamedPatFields','1')";//true by default
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'DisplayRenamedPatFields','1')";//true by default
				Db.NonQ(command);
			}
			//Removes ability to store credit card number in the database
			command="UPDATE preference SET ValueString='0' WHERE PrefName='StoreCCnumbers'";
			Db.NonQ(command);
			//Insert CADI bridge-----------------------------------------------------------------
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'CADI', "
					+"'CADI from www.cadi.net', "
					+"'0', "
					+"'C:\\CADI\\CADI.exe', "
					+"'', "
					+"'"+POut.String(@"Example of image folder: C:\Mediadent\patients\")+"')"; //CADI bridging documents still use Mediadent as the path
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
					+"'Image Folder', "
					+"'"+POut.String(@"C:\Mediadent\patients\")+"')";
				Db.NonQ(command);
				command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"'"+POut.Long(programNum)+"', "
					+"'2', "//ToolBarsAvail.ChartModule
					+"'CADI')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramNum),0)+1 FROM program),"
					+"'CADI', "
					+"'CADI from www.cadi.net', "
					+"'0', "
					+"'C:\\CADI\\CADI.exe', "
					+"'', "//leave blank if none
					+"'"+POut.String(@"Example of image folder: C:\Mediadent\patients\")+"')"; //CADI bridging documents still use Mediadent as the path
				long programNum=Db.NonQ(command,true,"ProgramNum","program");
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+"'"+POut.Long(programNum)+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0', "
					+"'0')";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+"'"+POut.Long(programNum)+"', "
					+"'Image Folder', "
					+"'"+POut.String(@"C:\Mediadent\patients\")+"', "
					+"'0')";
				Db.NonQ(command);
				command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
					+"VALUES ("
					+"(SELECT COALESCE(MAX(ToolButItemNum),0)+1 FROM toolbutitem),"
					+"'"+POut.Long(programNum)+"', "
					+"'2', "//ToolBarsAvail.ChartModule
					+"'CADI')";
				Db.NonQ(command);
			}//End CADI bridge
			//add securitylog.DateTPrevious
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.4.1 - add securitylog.DateTPrevious");
			if(DataConnection.DBtype==DatabaseType.MySql) {
				LargeTableHelper.AlterLargeTable("securitylog","SecurityLogNum",new List<Tuple<string,string>> { Tuple.Create("DateTPrevious","datetime NOT NULL DEFAULT '0001-01-01 00:00:00'") });
			}
			else {//oracle
				command="ALTER TABLE securitylog ADD DateTPrevious date";
				Db.NonQ(command);
				command="UPDATE securitylog SET DateTPrevious = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateTPrevious IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE securitylog MODIFY DateTPrevious NOT NULL";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.4.1");
			command="SELECT ProgramNum FROM program WHERE ProgName = 'XVWeb'";
			long xvWebProgramNum=PIn.Long(Db.GetScalar(command));
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(xvWebProgramNum)+"', " //xvweb
					+"'Username', "
					+"'')";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(xvWebProgramNum)+"', " //xvweb
					+"'Password', "
					+"'')";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(xvWebProgramNum)+"', " //xvweb
					+"'ImageCategory', "
					+"'')";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(xvWebProgramNum)+"', " //xvweb
					+"'Save Images (yes or no)', "
					+"'no')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+"'"+POut.Long(xvWebProgramNum)+"', " //xvweb
					+"'Username', "
					+"'',0)";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+"'"+POut.Long(xvWebProgramNum)+"', " //xvweb
					+"'Password', "
					+"'',0)";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+"'"+POut.Long(xvWebProgramNum)+"', " //xvweb
					+"'ImageCategory', "
					+"'',0)";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+"'"+POut.Long(xvWebProgramNum)+"', " //xvweb
					+"'Save Images (yes or no)', "
					+"'no',0)";
				Db.NonQ(command);
			}
			//Insert HDX WILL bridge-----------------------------------------------------------------
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'HdxWill', "
					+"'HDX WILL from www.hdx.co.kr', "
					+"'0', "
					+"'"+POut.String(@"C:\HDX\WILL.exe")+"', "
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
					+"'System path to HDX WILL Argument ini file', "
					+"'"+POut.String(@"C:\HDX\Argument.ini")+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramNum),0)+1 FROM program),"
					+"'HdxWill', "
					+"'HDX WILL from www.hdx.co.kr', "
					+"'0', "
					+"'"+POut.String(@"C:\HDX\WILL.exe")+"', "
					+"'', "//leave blank if none
					+"'')";
				long programNum=Db.NonQ(command,true,"ProgramNum","program");
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+"'"+POut.Long(programNum)+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0', "
					+"'0')";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+"'"+POut.Long(programNum)+"', "
					+"'System path to HDX WILL Argument ini file', "
					+"'"+POut.String(@"C:\HDX\Argument.ini")+"', "
					+"'0')";
				Db.NonQ(command);
			}//End HDX WILL bridge
			//Adds a default processing method for payconnect, and sets it to use the web service
			command="SELECT ProgramNum FROM program WHERE ProgName='PayConnect'";
			long payConnectProgNum=Db.GetLong(command);
			command="SELECT ClinicNum FROM clinic";
			List<long> listClinicNums=Db.GetListLong(command);
			listClinicNums.Add(0);
			for(int i = 0;i<listClinicNums.Count;i++) {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
						+"VALUES ("+POut.Long(payConnectProgNum)+","
							+"'DefaultProcessingMethod',"
							+"'0',"
							+"'',"
							+POut.Long(listClinicNums[i])+")";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
						+"VALUES ((SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
							+POut.Long(payConnectProgNum)+","
							+"'DefaultProcessingMethod',"
							+"'0',"
							+"'',"
							+POut.Long(listClinicNums[i])+")";
					Db.NonQ(command);
				}
			}
			//add custom aging report -------------------------
			command="SELECT MAX(displayreport.ItemOrder) FROM displayreport WHERE displayreport.Category = 2"; //monthly
			long itemorder = Db.GetLong(command)+1; //get the next available ItemOrder for the Monthly Category to put this new report last.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODCustomAging',"+POut.Long(itemorder)+",'Custom Aging',2,0)";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO displayreport(DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODCustomAging',"+POut.Long(itemorder)+",'Custom Aging',2,0)";
				Db.NonQ(command);
			}
			//Add permission for this report to everybody with the Aging Report permission.
			long reportNumODAgingAR = Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODAgingAR'");
			long reportNumODCustomAging = Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODCustomAging'");
			//Get a table of UserGroupNum that have permissions to Reports and to the ODOutstandingInsClaims report.
			DataTable userGroupTable = Db.GetTable("SELECT UserGroupNum FROM grouppermission WHERE PermType=22 AND FKey="+POut.Long(reportNumODAgingAR));
			foreach(DataRow row in userGroupTable.Rows) {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//Insert Reports permission (22) for the Custom Aging report.
					command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES('0001-01-01',0,"+POut.Long(PIn.Long(row["UserGroupNum"].ToString()))+",22,"+POut.Long(reportNumODCustomAging)+")";
					Db.NonQ(command);
				}
				else {//oracle
							//Insert Reports permission (22) for the Custom Aging report.
					command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission)"
						+","+"TO_DATE('0001-01-01','YYYY-MM-DD'),0,"+POut.Long(PIn.Long(row["UserGroupNum"].ToString()))+",22,"
						+POut.Long(reportNumODCustomAging)+")";
					Db.NonQ(command);
				}
			}
			command="SELECT ProgramNum FROM program WHERE ProgName='eRx'";
			long eRxProgramNum=Db.GetLong(command);
			string eRxOptionDefaultVal="0";//We haven't been given production urls for DoseSpot, so for now it defaults to NewCrop.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(eRxProgramNum)+"', "
					+"'eRx Option', "
					+"'"+eRxOptionDefaultVal+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+"'"+POut.Long(eRxProgramNum)+"', "
					+"'eRx Option', "
					+"'"+eRxOptionDefaultVal+"', "
					+"'0')";
				Db.NonQ(command);
			}
			//Add 'MsgValue' column to signalod table
			if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE signalod ADD MsgValue varchar(255) NOT NULL";
					Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE signalod ADD MsgValue varchar2(255)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS usergroupattach";
				Db.NonQ(command);
				command=@"CREATE TABLE usergroupattach (
						UserGroupAttachNum bigint NOT NULL auto_increment PRIMARY KEY,
						UserNum bigint NOT NULL,
						UserGroupNum bigint NOT NULL,
						INDEX(UserGroupNum)
						) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE usergroupattach'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE usergroupattach (
						UserGroupAttachNum number(20) NOT NULL,
						UserNum number(20),
						UserGroupNum number(20) NOT NULL,
						CONSTRAINT usergroupattach_UserGroupAttac PRIMARY KEY (UserGroupAttachNum)
						)";
				Db.NonQ(command);
				command=@"CREATE INDEX usergroupattach_UserGroupNum ON usergroupattach (UserGroupNum)";
				Db.NonQ(command);
			}
			command="SELECT userod.UserNum, userod.UserGroupNum FROM userod";
			table=Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				string strUserNum = row["UserNum"].ToString();
				string strUserGroupNum = row["UserGroupNum"].ToString();
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO usergroupattach(UserNum, UserGroupNum) VALUES ("+strUserNum+","+strUserGroupNum+")";
				}
				else {
					command="INSERT INTO usergroupattach(UserGroupAttachNum, UserNum, UserGroupNum) VALUES "
						+"((SELECT COALESCE(MAX(UserGroupAttachNum),0)+1 FROM usergroupattach),"+strUserNum+","+strUserGroupNum+")";
				}
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('AllowFutureInsPayments','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'AllowFutureInsPayments','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('SendUnhandledExceptionsToHQ','1')";//True by default
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'SendUnhandledExceptionsToHQ','1')";//True by default
				Db.NonQ(command);
			}
			//Add preference ClaimIDPrefix
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimIdPrefix','[PatNum]/')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ClaimIdPrefix','[PatNum]/')";
				Db.NonQ(command);
			}
			//Add ClaimIdentifierOrig column to claim table
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE claim ADD ClaimIdentifierOrig varchar(255) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE claim ADD ClaimIdentifierOrig varchar2(255)";
				Db.NonQ(command);
			}
			//Set the ClaimIdentifierOrig for existing claims
			command="UPDATE claim SET claim.ClaimIdentifierOrig=claim.ClaimIdentifier";
			Db.NonQ(command);
			//Add ErxPharmacyInfo column to Rxpat table
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE rxpat ADD ErxPharmacyInfo varchar(255) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE rxpat ADD ErxPharmacyInfo varchar2(255)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE emailmessage ADD UserNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE emailmessage ADD INDEX (UserNum)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE emailmessage ADD UserNum number(20)";
				Db.NonQ(command);
				command="UPDATE emailmessage SET UserNum = 0 WHERE UserNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE emailmessage MODIFY UserNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX emailmessage_UserNum ON emailmessage (UserNum)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE provider ADD Birthdate date NOT NULL DEFAULT '0001-01-01'";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE provider ADD Birthdate date";
				Db.NonQ(command);
				command="UPDATE provider SET Birthdate = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE Birthdate IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE provider MODIFY Birthdate NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ShowPreferedReferrals','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ShowPreferedReferrals','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE referral ADD IsPreferred tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE referral ADD IsPreferred number(3)";
				Db.NonQ(command);
				command="UPDATE referral SET IsPreferred = 0 WHERE IsPreferred IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE referral MODIFY IsPreferred NOT NULL";
				Db.NonQ(command);
			}
			//Add provider.SchedNote
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE provider ADD SchedNote varchar(255) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE provider ADD SchedNote varchar2(255)";
				Db.NonQ(command);
			}
			//Add FeeSchedEdit permission (92) to for users who previously had the setup permission (8).
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=8";
			table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",92)"; 
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",92,0)";
					Db.NonQ(command);
				}
			}
			//We have reverted the tasksubscription feature.
			//if(DataConnection.DBtype==DatabaseType.MySql) {
			//	command="ALTER TABLE taskhist ADD TaskListNums text NOT NULL";
			//	Db.NonQ(command);
			//}
			//else {//oracle
			//	command="ALTER TABLE taskhist ADD TaskListNums clob";
			//	Db.NonQ(command);
			//}
			//command="UPDATE taskhist SET TaskListNums=TaskListNum";
			//Db.NonQ(command);
			//if(DataConnection.DBtype==DatabaseType.MySql) {
			//	command="INSERT INTO tasksubscription(UserNum,TaskListNum,TaskNum) "
			//		+"SELECT 0,TaskListNum,TaskNum FROM task";
			//	Db.NonQ(command);
			//}
			//else {//oracle
			//	command="SELECT TaskListNum,TaskNum FROM task";
			//	table=Db.GetTable(command);
			//	foreach(DataRow row in table.Rows) {
			//		long taskListNum=PIn.Long(row["TaskListNum"].ToString());
			//		long taskNum=PIn.Long(row["TaskNum"].ToString());
			//		command="INSERT INTO tasksubscription(TaskSubscriptionNum,UserNum,TaskListNum,TaskNum) "
			//			+"VALUES((SELECT COALESCE(MAX(TaskSubscriptionNum),0)+1 FROM tasksubscription),0,"+POut.Long(taskListNum)+","+POut.Long(taskNum)+")";
			//		Db.NonQ(command);
			//	}
			//}
			//try {
			//	if(DataConnection.DBtype==DatabaseType.MySql) {
			//		if(!IndexExists("taskancestor","TaskListNum,TaskNum")) {
			//			command="ALTER TABLE taskancestor ADD INDEX TaskListAndNum (TaskListNum,TaskNum)";
			//			Db.NonQ(command);
			//			if(IndexExists("taskancestor","TaskListNum")) {
			//				command="ALTER TABLE taskancestor DROP INDEX TaskListNum";
			//				Db.NonQ(command);
			//			}
			//		}
			//	}
			//	else {//oracle
			//		command="CREATE INDEX taskancestor_TaskListAndNum ON taskancestor (TaskListNum,TaskNum)";//add composite index
			//		Db.NonQ(command);
			//		command="DROP INDEX taskancestor_TaskListNum";//drop redundant index once composite index is successfully added
			//		Db.NonQ(command);
			//	}
			//}
			//catch(Exception) { }//Only an index.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ProcNoteConcurrencyMerge','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ProcNoteConcurrencyMerge','0')";
				Db.NonQ(command);
			}
			//add preference AppointmentWithoutProcsDefaultLength
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('AppointmentWithoutProcsDefaultLength','30')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'AppointmentWithoutProcsDefaultLength','30')";
				Db.NonQ(command);
			}
			command="UPDATE apptreminderrule SET TypeCur=0 WHERE TypeCur=2";//Change all ReminderFutureDay to just Reminder.
			Db.NonQ(command);

			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE creditcard ADD ExcludeProcSync tinyint NOT NULL";
				Db.NonQ(command);
				command="UPDATE creditcard SET ExcludeProcSync = 0 WHERE ExcludeProcSync IS NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE creditcard ADD ExcludeProcSync number(3)";
				Db.NonQ(command);
				command="UPDATE creditcard SET ExcludeProcSync = 0 WHERE ExcludeProcSync IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE creditcard MODIFY ExcludeProcSync NOT NULL";
				Db.NonQ(command);
			}
			//Adds the AddFamilyInheritsEmail preference
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('AddFamilyInheritsEmail','1')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'AddFamilyInheritsEmail','1')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE providererx ADD ErxType tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE providererx ADD ErxType number(3)";
				Db.NonQ(command);
				command="UPDATE providererx SET ErxType = 0 WHERE ErxType IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE providererx MODIFY ErxType NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('DoseSpotDateLastAccessCheck','')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'DoseSpotDateLastAccessCheck','')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE userweb ADD RequirePasswordChange tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE userweb ADD RequirePasswordChange number(3)";
				Db.NonQ(command);
				command="UPDATE userweb SET RequirePasswordChange = 0 WHERE RequirePasswordChange IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE userweb MODIFY RequirePasswordChange NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE clinic ADD SchedNote varchar(255) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE clinic ADD SchedNote varchar2(255)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('EraPrintOneClaimPerPage','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'EraPrintOneClaimPerPage','0')";
				Db.NonQ(command);
			}
			//convert carrier.CarrierGroupName from a string to long.It will now point to a definition. We need to create definitions to current existing CarrierGroupNames.
			command="SELECT CarrierGroupName FROM carrier GROUP BY CarrierGroupName";
			table=Db.GetTable(command);
			string carrierGroupNameCur;
			long defNum;
			int orderNum=0;
			foreach(DataRow row in table.Rows) {
				carrierGroupNameCur=PIn.String(row["CarrierGroupName"].ToString());
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO definition (Category, ItemOrder, ItemName) VALUES (46,"+orderNum+",'"+POut.String(carrierGroupNameCur)+"')";//46-CarrierGroupName
					defNum=Db.NonQ(command,true);
				}
				else {//oracle
					command="INSERT INTO definition (DefNum,Category, ItemOrder, ItemName) VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),46,"//46-CarrierGroupName
						+orderNum+",'"+POut.String(carrierGroupNameCur)+"')";
					defNum=Db.NonQ(command,true,"DefNum","definition");
				}
				//update CarrierGroupName with the new defnum
				command="UPDATE carrier SET CarrierGroupName='"+POut.Long(defNum)+"' "
				+"WHERE CarrierGroupName='"+POut.String(carrierGroupNameCur)+"'";
				Db.NonQ(command);
				orderNum++;
			}
			//Now change the data type of CarrierGroupName from a string to a long. 
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE carrier MODIFY CarrierGroupName bigint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE carrier MODIFY (CarrierGroupName number(20))";
				Db.NonQ(command);
				command="UPDATE carrier SET CarrierGroupName = 0 WHERE CarrierGroupName IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE carrier MODIFY CarrierGroupName NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS patientportalinvite";
				Db.NonQ(command);
				command=@"CREATE TABLE patientportalinvite (
						PatientPortalInviteNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						AptNum bigint NOT NULL,
						ClinicNum bigint NOT NULL,
						DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						TSPrior bigint NOT NULL,
						EmailSendStatus tinyint NOT NULL,
						EmailMessageNum bigint NOT NULL,
						TemplateEmail text NOT NULL,
						TemplateEmailSubj varchar(255) NOT NULL,
						Note text NOT NULL,
						INDEX(PatNum),
						INDEX(AptNum),
						INDEX(ClinicNum),
						INDEX(EmailMessageNum)
						) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE patientportalinvite'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE patientportalinvite (
						PatientPortalInviteNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						AptNum number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						DateTimeEntry date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						TSPrior number(20) NOT NULL,
						EmailSendStatus number(3) NOT NULL,
						EmailMessageNum number(20) NOT NULL,
						TemplateEmail clob,
						TemplateEmailSubj varchar2(255),
						Note clob,
						CONSTRAINT patientportalinvite_PatientPor PRIMARY KEY (PatientPortalInviteNum)
						)";
				Db.NonQ(command);
				command=@"CREATE INDEX patientportalinvite_PatNum ON patientportalinvite (PatNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX patientportalinvite_AptNum ON patientportalinvite (AptNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX patientportalinvite_ClinicNum ON patientportalinvite (ClinicNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX patientportalinvite_EmailMessa ON patientportalinvite (EmailMessageNum)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('PatientPortalSignedUp','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'PatientPortalSignedUp','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE paysplit CHANGE PrepaymentNum FSplitNum bigint(20) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE paysplit RENAME COLUMN PrepaymentNum TO FSplitNum";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE rxpat CHANGE NewCropGuid ErxGuid varchar(40) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE rxpat RENAME COLUMN NewCropGuid TO ErxGuid";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE medicationpat CHANGE NewCropGuid ErxGuid varchar(255) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE medicationpat RENAME COLUMN NewCropGuid TO ErxGuid";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedNewPatApptForcePhoneFormatting','1')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedNewPatApptForcePhoneFormatting','1')";
				Db.NonQ(command);
			}
			//Adding preferences for websched Verifys
			string VerifyEmailSubj="Appointment Scheduling Confirmation";
			string VerifyEmailBody="Hello [FName] [LName]. Your appointment has been scheduled on [ApptDate] [ApptTime] at [OfficeName], [OfficeAddress]. Please call [OfficePhone] if you have any questions about your appointment. We look forward to seeing you.";
			string VerifyText="Appointment scheduled for [FName] on [ApptDate] [ApptDate] at [OfficeName], [OfficeAddress]";
			// #1 VerifyNewPatEmailBody
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedVerifyNewPatEmailBody','"+VerifyEmailBody+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedVerifyNewPatEmailBody','"+VerifyEmailBody+"')";
				Db.NonQ(command);
			}
			// #2 VerifyNewPatEmailSubj
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedVerifyNewPatEmailSubj','"+VerifyEmailSubj+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedVerifyNewPatEmailSubj','"+VerifyEmailSubj+"')";
				Db.NonQ(command);
			}
			// #3 VerifyNewPatText
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedVerifyNewPatText','"+VerifyText+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedVerifyNewPatText','"+VerifyText+"')";
				Db.NonQ(command);
			}
			// #4 VerifyNewPatType
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedVerifyNewPatType','3')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedVerifyNewPatType','3')";
				Db.NonQ(command);
			}
			// #5 VerifyRecallEmailBody
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedVerifyRecallEmailBody','"+VerifyEmailBody+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedVerifyRecallEmailBody','"+VerifyEmailBody+"')";
				Db.NonQ(command);
			}
			// #6 VerifyRecallEmailSubj
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedVerifyRecallEmailSubj','"+VerifyEmailSubj+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedVerifyRecallEmailSubj','"+VerifyEmailSubj+"')";
				Db.NonQ(command);
			}
			// #7 VerifyRecallText
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedVerifyRecallText','"+VerifyText+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedVerifyRecallText','"+VerifyText+"')";
				Db.NonQ(command);
			}
			// #8 VerifyRecallType
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedVerifyRecallType','3')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedVerifyRecallType','3')";
				Db.NonQ(command);
			}
			// #9 VerifyASAPEmailBody
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedVerifyASAPEmailBody','"+VerifyEmailBody+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedVerifyASAPEmailBody','"+VerifyEmailBody+"')";
				Db.NonQ(command);
			}
			// #10 VerifyASAPEmailSubj
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedVerifyASAPEmailSubj','"+VerifyEmailSubj+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedVerifyASAPEmailSubj','"+VerifyEmailSubj+"')";
				Db.NonQ(command);
			}
			// #11 VerifyASAPText
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedVerifyASAPText','"+VerifyText+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedVerifyASAPText','"+VerifyText+"')";
				Db.NonQ(command);
			}
			// #12 VerifyASAPType
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedVerifyASAPType','3')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedVerifyASAPType','3')";
				Db.NonQ(command);
			}
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='eServices' AND IsHQCategory=1";
			alertCategoryNum=Db.GetScalar(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",12)";//EConnectorDown
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",12)";//EConnectorDown
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",13)";//EConnectorError
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",13)";//EConnectorError
				Db.NonQ(command);
			}
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes' AND IsHQCategory=1";
			alertCategoryNum=Db.GetScalar(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",12)";//EConnectorDown
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",12)";//EConnectorDown
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",13)";//EConnectorError
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",13)";//EConnectorError
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('PatientPortalInviteEnabled','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'PatientPortalInviteEnabled','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('TransworldDateTimeLastUpdated','')";
				Db.NonQ(command);
				command="INSERT INTO preference (PrefName,ValueString) VALUES('TransworldServiceSendFrequency','')";
				Db.NonQ(command);
				command="INSERT INTO preference (PrefName,ValueString) VALUES('TransworldServiceTimeDue','')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'TransworldDateTimeLastUpdated','')";
				Db.NonQ(command);
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'TransworldServiceSendFrequency','')";
				Db.NonQ(command);
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'TransworldServiceTimeDue','')";
				Db.NonQ(command);
			}
			//Add services selected program property for Transworld program link
			command="SELECT ProgramNum FROM program WHERE ProgName='Transworld'";
			long transworldProgNum=Db.GetLong(command);
			command="SELECT DISTINCT ClinicNum FROM programproperty WHERE ProgramNum="+POut.Long(transworldProgNum);
			listClinicNums=Db.GetListLong(command);
			for(int i=0;i<listClinicNums.Count;i++) {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
						+"VALUES ("+POut.Long(transworldProgNum)+","
						+"'SelectedServices',"
						+"'0,1,2',"//0=TsiDemandType.Accelerator,1=TsiDemandType.ProfitRecovery,2=TsiDemandType.Collection
						+"'',"
						+POut.Long(listClinicNums[i])+")";
					Db.NonQ(command);;
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
						+"VALUES ((SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
						+POut.Long(transworldProgNum)+","
						+"'SelectedServices',"
						+"'0,1,2',"//0=TsiDemandType.Accelerator,1=TsiDemandType.ProfitRecovery,2=TsiDemandType.Collection
						+"'',"
						+POut.Long(listClinicNums[i])+")";
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE tsitranslog ADD TransJson text NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE tsitranslog ADD TransJson clob";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.4.1 - tasksubscription index");
			try {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					if(!LargeTableHelper.IndexExists("tasksubscription","TaskListNum")) {
						command="ALTER TABLE tasksubscription ADD INDEX (TaskListNum)";
						Db.NonQ(command);
					}
				}
				else {//oracle
					command=@"CREATE INDEX tasksubscription_TaskListNum ON tasksubscription (TaskListNum)";
					Db.NonQ(command);
				}
			}
			catch(Exception ex) { ex.DoNothing(); }//Only an index. (Exception ex) required to catch thrown exception
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.4.1");
		}//End method To17_4_0

		private static void To17_4_10() {
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				//Check to see if the column that was added by the tasksubscription feature was added (meaning they need to revert those changes)
				command=@"SELECT COUNT(*) FROM information_schema.COLUMNS 
					WHERE TABLE_SCHEMA=DATABASE() 
					AND TABLE_NAME='taskhist' 
					AND COLUMN_NAME='TaskListNums'";
				if(Db.GetCount(command)!="0") {
					//Update task.tasklistnum from one of the tasksubscription rows.  MySQL will pick one for us at random if there are duplicates.
					command=@"UPDATE task INNER JOIN
						(SELECT TaskNum,TaskListNum 
							FROM tasksubscription 
							WHERE UserNum=0 
							GROUP BY TaskNum
						)tasksub ON task.TaskNum=tasksub.TaskNum 
						SET task.TaskListNum=tasksub.TaskListNum";
					Db.NonQ(command);
					//Delete tasksubscription rows that are not needed
					command="DELETE FROM tasksubscription WHERE UserNum=0";
					Db.NonQ(command);
					//Drop the added column
					command="ALTER TABLE taskhist DROP COLUMN TaskListNums";
					Db.NonQ(command);
					//Remove the multi-column index that was created, if it exists
					if(LargeTableHelper.IndexExists("taskancestor","TaskListNum,TaskNum")) {
						command="ALTER TABLE taskancestor DROP INDEX TaskListAndNum";
						Db.NonQ(command);
					}
					//Add the index back to taskancestor.TaskListNum
					if(!LargeTableHelper.IndexExists("taskancestor","TaskListNum")) {
						command="ALTER TABLE taskancestor ADD INDEX TaskListNum (TaskListNum)";
						Db.NonQ(command);
					}
				}
				else {
					//No need to to anything
				}
			}
			//No need to support oracle for this revert fix per Jason.
		}

		private static void To17_4_12() {
			string command="";
			//Add the verbose logging preference
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('HasVerboseLogging','')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'HasVerboseLogging','')";
				Db.NonQ(command);
			}
			command="SELECT ValueString FROM preference WHERE PrefName='NewCropAccountId'";
			string newCropAcctID=Db.GetScalar(command);
			if(newCropAcctID=="") {//If not using NewCrop, set the eRx option to DoseSpot.
				command="SELECT ProgramPropertyNum FROM programproperty WHERE PropertyDesc='eRx Option'";
				long erxOptionProgramPropertyNum=Db.GetLong(command);
				command="UPDATE programproperty SET PropertyValue=1 WHERE ProgramPropertyNum="+erxOptionProgramPropertyNum.ToString();
				Db.NonQ(command);
			}
		}

		private static void To17_4_15() {
			string command = "";
			//add PaymentWindowDefaultHideSplits preference if it does not already exist.
			command = "SELECT * FROM preference WHERE preference.PrefName = 'PaymentWindowDefaultHideSplits'";
			if(Db.GetTable(command).Rows.Count == 0) {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('PaymentWindowDefaultHideSplits','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'PaymentWindowDefaultHideSplits','0')";
					Db.NonQ(command);
				}
			}
		}

		private static void To17_4_21() {
			string command;
			command="SELECT ValueString FROM preference WHERE PrefName='ApptConfirmAutoSignedUp'";
			if(Db.GetTable(command).Rows.Count==0) {//This preference may have been inserted in 17.3.63.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('ApptConfirmAutoSignedUp','0')";//false by default, eConnector will change it if needed
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ApptConfirmAutoSignedUp','0')";//false by default, eConnector will change it if needed
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE providererx ADD UserId varchar(255) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE providererx ADD UserId varchar2(255)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS clinicerx";
				Db.NonQ(command);
				command=@"CREATE TABLE clinicerx (
					ClinicErxNum bigint NOT NULL auto_increment PRIMARY KEY,
					PatNum bigint NOT NULL,
					ClinicDesc varchar(255) NOT NULL,
					ClinicNum bigint NOT NULL,
					EnabledStatus tinyint NOT NULL,
					ClinicId varchar(255) NOT NULL,
					ClinicKey varchar(255) NOT NULL,
					INDEX(PatNum),
					INDEX(ClinicNum)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE clinicerx'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE clinicerx (
					ClinicErxNum number(20) NOT NULL,
					PatNum number(20) NOT NULL,
					ClinicDesc varchar2(255),
					ClinicNum number(20) NOT NULL,
					EnabledStatus number(3) NOT NULL,
					ClinicId varchar2(255),
					ClinicKey varchar2(255),
					CONSTRAINT clinicerx_ClinicErxNum PRIMARY KEY (ClinicErxNum)
					)";
				Db.NonQ(command);
				command=@"CREATE INDEX clinicerx_PatNum ON clinicerx (PatNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX clinicerx_ClinicNum ON clinicerx (ClinicNum)";
				Db.NonQ(command);
			}
		}

		private static void To17_4_22() {
			string command;
			DataTable table;
			//The convert script for deleting ortho benefits was included in 17.3.59 and we don't want to run the script again if it has already been run.
			bool hasUpdated17_3_59=false;
			command="SELECT ProgramVersion FROM updatehistory ORDER BY DateTimeUpdated DESC ";
			table=Db.GetTable(command);
			if(table.Rows.Count > 0) {
				Version vers=new Version(table.Rows[0]["ProgramVersion"].ToString());
				if(vers >= new Version(17,3,59,0) && vers < new Version(17,4,1,0)) {
					hasUpdated17_3_59=true;
				}
			}
			if(!hasUpdated17_3_59) {
				//Delete all ortho age limitation proc code benefits and replace them with one category benefit
				command=@"SELECT procedurecode.CodeNum
					FROM covcat
					INNER JOIN covspan ON covspan.CovCatNum=covcat.CovCatNum
					INNER JOIN procedurecode ON procedurecode.ProcCode BETWEEN covspan.FromCode AND covspan.ToCode
					WHERE covcat.EbenefitCat=9";//9 is EbenefitCategory.Orthodontics
				List<long> listOrthoCodeNums=Db.GetListLong(command);
				command="SELECT CovCatNum FROM covcat WHERE EbenefitCat=9";//9 is EbenefitCategory.Orthodontics
				long orthoCovCatNum=Db.GetLong(command);
				table=new DataTable();
				if(listOrthoCodeNums.Count > 0) {
					command=@"SELECT MAX(benefit.Quantity) Quantity,benefit.PlanNum
						FROM benefit
						WHERE benefit.CodeNum IN("+string.Join(",",listOrthoCodeNums.Select(x => POut.Long(x)))+@")
						AND benefit.BenefitType=5"/*InsBenefitType.Limitations*/+@"
						AND benefit.MonetaryAmt=-1
						AND benefit.PatPlanNum=0
						AND benefit.Percent=-1
						AND benefit.QuantityQualifier=2"/*BenefitQuantity.AgeLimit*/+@"
						GROUP BY benefit.PlanNum";
					table=Db.GetTable(command);
					command=@"DELETE FROM benefit
						WHERE benefit.CodeNum IN("+string.Join(",",listOrthoCodeNums.Select(x => POut.Long(x)))+@")
						AND benefit.BenefitType=5"/*InsBenefitType.Limitations*/+@"
						AND benefit.MonetaryAmt=-1
						AND benefit.PatPlanNum=0
						AND benefit.Percent=-1
						AND benefit.QuantityQualifier=2"/*BenefitQuantity.AgeLimit*/;
					Db.NonQ(command);
				}
				foreach(DataRow row in table.Rows) {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command=@"INSERT INTO benefit(CodeNum,BenefitType,CovCatNum,PlanNum,QuantityQualifier,Quantity,Percent,MonetaryAmt,PatPlanNum) 
							VALUES("
								+"0,"
								+"5,"//InsBenefitType.Limitations
								+POut.Long(orthoCovCatNum)+","
								+POut.Long(PIn.Long(row["PlanNum"].ToString()))+","
								+"2,"//BenefitQuantity.AgeLimit
								+POut.Byte(PIn.Byte(row["Quantity"].ToString()))+","
								+"-1,"
								+"-1,"
								+"0)";
						Db.NonQ(command);
					}
					else {//oracle
						command=@"INSERT INTO benefit(BenefitNum,CodeNum,BenefitType,CovCatNum,PlanNum,QuantityQualifier,Quantity,Percent,MonetaryAmt,PatPlanNum) 
						VALUES("
							+"(SELECT COALESCE(MAX(BenefitNum),0)+1 FROM benefit),"
							+"0,"
							+"5,"//InsBenefitType.Limitations
							+POut.Long(orthoCovCatNum)+","
							+POut.Long(PIn.Long(row["PlanNum"].ToString()))+","
							+"2,"//BenefitQuantity.AgeLimit
							+POut.Byte(PIn.Byte(row["Quantity"].ToString()))+","
							+"-1,"
							+"-1,"
							+"0)";
						Db.NonQ(command);
					}
				}
			}
			//Add TransworldPaidInFullBillingType preference for use by the OpenDentalService. No default, requires user interaction.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('TransworldPaidInFullBillingType','0')";//FK to definition.DefNum
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'TransworldPaidInFullBillingType','0')";//FK to definition.DefNum
				Db.NonQ(command);
			}
		}

		private static void To17_4_23() {
			DataTable table;
			string command="SELECT COUNT(*) FROM programproperty WHERE PropertyDesc='Disable Advertising HQ'";
			if(Db.GetCount(command)=="0") {
				command="SELECT * FROM program WHERE ProgName IN ('CentralDataStorage','DentalTekSmartOfficePhone','Podium','RapidCall','TransWorld','DentalIntel','PracticeByNumbers')";
				table=Db.GetTable(command);
				foreach(DataRow row in table.Rows) {
					//Change the "Disable Advertising" program property associated to program to the value of 0.
					command="UPDATE programproperty SET PropertyValue='0' WHERE (ProgramNum='"+row["ProgramNum"].ToString()+"' AND PropertyDesc='Disable Advertising')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString='0001-01-01 00:00:00' WHERE PrefName='ProgramAdditionalFeatures'";//This was the default value when it was originally inserted.
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE providererx ADD AccountId varchar(25) NOT NULL";//The account ids are generated by Open Dental and dont need to be 255 long
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE providererx ADD AccountId varchar2(25)";//The account ids are generated by Open Dental and dont need to be 255 long
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE clinicerx ADD AccountId varchar(25) NOT NULL";//The account ids are generated by Open Dental and dont need to be 255 long
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE clinicerx ADD AccountId varchar2(25)";//The account ids are generated by Open Dental and dont need to be 255 long
				Db.NonQ(command);
			}
		}

		private static void To17_4_24() {
			string command="";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE clinicerx ADD RegistrationKeyNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE clinicerx ADD INDEX (RegistrationKeyNum)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE clinicerx ADD RegistrationKeyNum number(20)";
				Db.NonQ(command);
				command="UPDATE clinicerx SET RegistrationKeyNum = 0 WHERE RegistrationKeyNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE clinicerx MODIFY RegistrationKeyNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX clinicerx_RegistrationKeyNum ON clinicerx (RegistrationKeyNum)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE providererx ADD RegistrationKeyNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE providererx ADD INDEX (RegistrationKeyNum)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE providererx ADD RegistrationKeyNum number(20)";
				Db.NonQ(command);
				command="UPDATE providererx SET RegistrationKeyNum = 0 WHERE RegistrationKeyNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE providererx MODIFY RegistrationKeyNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX providererx_RegistrationKeyNum ON providererx (RegistrationKeyNum)";
				Db.NonQ(command);
			}
		}
		
		private static void To17_4_28() {
			string command="";
			//Enable/disable ShowAllocateUnearnedPaymentPrompt when creating a claim with unearned unallocated income on the account
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ShowAllocateUnearnedPaymentPrompt','1')";//true by default, maintain current behavior
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ShowAllocateUnearnedPaymentPrompt','1')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE appointmenttype MODIFY CodeStr VARCHAR(4000) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE appointmenttype MODIFY CodeStr VARCHAR2(4000)";
				Db.NonQ(command);
			}
			//Create eRx AlertCategory----------------------------------------------------------------------------------------------------------------
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategory (IsHQCategory,InternalName,Description) VALUES(1,'eRx','eRx')";
			}
			else {//oracle
				command="INSERT INTO alertcategory (AlertCategoryNum,IsHQCategory,InternalName,Description) "
					+"VALUES((SELECT MAX(AlertCategoryNum)+1 FROM alertcategory),1,'eRx','eRx')";
			}
			string alertCategoryNum=POut.Long(Db.NonQ(command,true,"AlertCategoryNum","alertcategory"));
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",14)";//DoseSpotProviderRegistered
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",14)";//DoseSpotProviderRegistered
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",15)";//DoseSpotClinicRegistered
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",15)";//DoseSpotClinicRegistered
				Db.NonQ(command);
			}
		}

		private static void To17_4_30() {
			string command="";
			try {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					if(!LargeTableHelper.IndexExists("adjustment","AdjDate,PatNum")) {
						command="ALTER TABLE adjustment ADD INDEX AdjDatePN (AdjDate, PatNum)";
						Db.NonQ(command);
						command="ALTER TABLE adjustment DROP INDEX AdjDate";//drop redundant index once composite index is successfully added
						Db.NonQ(command);
					}
				}
				else {//oracle
					command="CREATE INDEX adjustment_AdjDatePN ON adjustment (AdjDate, PatNum)";
					Db.NonQ(command);
					command="DROP INDEX adjustment_AdjDate";//drop redundant index once composite index is successfully added
					Db.NonQ(command);
				}
			}
			catch(Exception ex) {
				ex.DoNothing();//Just an index
			}
			command="SELECT COUNT(*) FROM preference WHERE PrefName='AgingReportShowAgePatPayplanPayments'";
			if(Db.GetCount(command)=="0") {//could've been added in 17.3.59
				//Add AgingReportShowAgePatPayplanPayments preference to make the age patient payments to payment plans checkbox visible.
				//For a specific customer, no UI, defaults to false, enable via query only.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('AgingReportShowAgePatPayplanPayments','0')";//false, unless enabled via query
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'AgingReportShowAgePatPayplanPayments','0')";//false, unless enabled via query
					Db.NonQ(command);
				}
			}
		}

		private static void To17_4_32() {
			string command="";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('RxHasProc','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'RxHasProc','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE clinic ADD HasProcOnRx tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE clinic ADD HasProcOnRx number(3)";
				Db.NonQ(command);
				command="UPDATE clinic SET HasProcOnRx = 0 WHERE HasProcOnRx IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE clinic MODIFY HasProcOnRx NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE rxdef ADD IsProcRequired tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE rxdef ADD IsProcRequired number(3)";
				Db.NonQ(command);
				command="UPDATE rxdef SET IsProcRequired = 0 WHERE IsProcRequired IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE rxdef MODIFY IsProcRequired NOT NULL";
				Db.NonQ(command);
			}
			command="UPDATE rxdef SET IsProcRequired=IsControlled";
			Db.NonQ(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE rxpat ADD IsProcRequired tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE rxpat ADD IsProcRequired number(3)";
				Db.NonQ(command);
				command="UPDATE rxpat SET IsProcRequired = 0 WHERE IsProcRequired IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE rxpat MODIFY IsProcRequired NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE rxpat ADD ProcNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE rxpat ADD INDEX (ProcNum)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE rxpat ADD ProcNum number(20)";
				Db.NonQ(command);
				command="UPDATE rxpat SET ProcNum = 0 WHERE ProcNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE rxpat MODIFY ProcNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX rxpat_ProcNum ON rxpat (ProcNum)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE rxpat ADD DaysOfSupply int NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE rxpat ADD DaysOfSupply number(11)";
				Db.NonQ(command);
				command="UPDATE rxpat SET DaysOfSupply = 0 WHERE DaysOfSupply IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE rxpat MODIFY DaysOfSupply NOT NULL";
				Db.NonQ(command);
			}
		}

		private static void To17_4_33() {
			string command="";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('DoLimitTaskSignals','0')";//False by default
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'DoLimitTaskSignals','0')";//False by default
				Db.NonQ(command);
			}
		}

		private static void To17_4_34() {
			string command="";
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DELETE FROM taskunread WHERE taskunread.TaskNum IN "
				+"(SELECT taskunread2.TaskNum "
				+"FROM (SELECT * FROM taskunread) taskunread2 "
				+"INNER JOIN task ON task.TaskNum=taskunread2.TaskNum "
				+"WHERE task.TaskStatus=2) ";//TaskStatus 2 = Done
				Db.NonQ(command);
			}
			else {
				//Accompanied by a bug fix to prevent this situation from occurring. Not necessary to run for Oracle as we currently do not have any Oracle customers. If
				//we do start supporting Oracle there will be no need for them to run this.
			}
		}

		private static void To17_4_36() {
			string command="";
			command="SELECT ValueString FROM preference WHERE PrefName='RigorousAccounting'";
			int val=Db.GetInt(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('RigorousAdjustments','"+POut.Int(val)+"')";//Link but do not enforce
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'RigorousAdjustments','"+POut.Int(val)+"')";//Link but do not enforce
				Db.NonQ(command);
			}
			//Add new IsDeleted wikipage column
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE wikipage ADD IsDeleted tinyint NOT NULL DEFAULT 0";//false by default
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE wikipage ADD IsDeleted number(3)";
				Db.NonQ(command);
				command="UPDATE wikipage SET IsDeleted = 1 WHERE IsDeleted IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE wikipage MODIFY IsDeleted NOT NULL DEFAULT 0";//false by default
				Db.NonQ(command);
			}
			//Create wikipages that were once archived wikipagehist pages. 
			//Insert them into the wikipage table. 
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command=@"INSERT INTO wikipage (UserNum,PageTitle,PageContent,DateTimeSaved,IsDeleted) 
									SELECT wikipagehist.UserNum,wikipagehist.PageTitle,wikipagehist.PageContent,wikipagehist.DateTimeSaved,1  
									FROM wikipagehist 
									INNER JOIN (
										SELECT wikipagehist.PageTitle,MAX(wikipagehist.DateTimeSaved) DateTimeSaved
										FROM wikipagehist
										WHERE wikipagehist.IsDeleted=1
										GROUP BY wikipagehist.PageTitle
									)lastwikipagehist ON lastwikipagehist.PageTitle=wikipagehist.PageTitle AND lastwikipagehist.DateTimeSaved=wikipagehist.DateTimeSaved
									WHERE wikipagehist.IsDeleted=1
									AND wikipagehist.PageTitle NOT IN (SELECT wikipage.PageTitle FROM wikipage)";
				Db.NonQ(command);
			}
			else {//oracle
						//We currently have no customers in Oracle, which means no one is achiving wiki pages in oracle.
			}
			//Wikipage changes.
			//For updating WikiPages, it was decided on 02/01/2018 by Nathan, Josh, Allen, and Saul that updating in a small batch of 100 was acceptable,
			//due to the fact that at ODHQ we are able to successfully finish updating without running out of memory or taking several hours to complete.
			//Nathan is ok with the assumption that ODHQ would have the largest wiki/hist database tables and would be the worst offender of potential issues.
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.4.36 - Updating wikipage");//No translation in convert script.
			command="SELECT WikiPageNum,PageContent,PageTitle FROM wikipage WHERE IsDraft=0 ORDER BY PageTitle";
			DataTable tableWikiListAll=Db.GetTable(command);
			Dictionary<string,long> dictWikiPages=new Dictionary<string, long>();
			foreach(DataRow row in tableWikiListAll.Rows) {
				dictWikiPages[PIn.String(row["PageTitle"].ToString())]=PIn.Long(row["WikiPageNum"].ToString());
			}
			string wikiPageContent;
			long wikiPageNum;
			//Go through each of the wikipages.  This doesn't matter if the wikipage is deleted or not.
			foreach(DataRow row in tableWikiListAll.Rows) {
				wikiPageContent=PIn.String(row["PageContent"].ToString());
				wikiPageNum=PIn.Long(row["WikiPageNum"].ToString());
				MatchCollection matchCollection=Regex.Matches(wikiPageContent.ToString(),@"\[\[.+?\]\]");
				foreach(Match m in matchCollection) {
					string mVal=m.Value;
					if(mVal.Contains("[[img:")
						|| mVal.Contains("[[keywords:")
						|| mVal.Contains("[[file:")
						|| mVal.Contains("[[folder:")
						|| mVal.Contains("[[list:")
						|| mVal.Contains("[[color:")
						|| mVal.Contains("[[filecloud:")
						|| mVal.Contains("[[foldercloud:")
						|| mVal.Contains("[[font:"))
					{
						continue;
					}
					long pageNum;
					mVal=mVal.TrimStart('[').TrimEnd(']');
					if(dictWikiPages.TryGetValue(mVal,out pageNum)) {//page title exist, replace with WikiPageNum instead of PageTitle
						string replace="[["+pageNum.ToString()+"]]";
						command="UPDATE wikipage SET PageContent=REPLACE(PageContent,'[["+POut.String(mVal)+"]]', '[["+pageNum.ToString()+"]]') WHERE WikiPageNum="+POut.String(wikiPageNum.ToString())+";";
						Db.NonQ(command);
					}
					else {//page title does not exist. Replace with [[0]]
						command="UPDATE wikipage SET PageContent=REPLACE(PageContent,'[["+POut.String(mVal)+"]]', '[[0]]') WHERE WikiPageNum="+POut.String(wikiPageNum.ToString())+";";
						Db.NonQ(command);
					}
				}
			}
			//Go through IsDraft Wikipages.
			//Links to the active wikipages.
			command="SELECT WikiPageNum,PageContent,PageTitle FROM wikipage WHERE IsDraft=1 ORDER BY PageTitle";
			DataTable tableWikiListDrafts=Db.GetTable(command);
			foreach(DataRow row in tableWikiListDrafts.Rows) {
				wikiPageContent=PIn.String(row["PageContent"].ToString());
				wikiPageNum=PIn.Long(row["WikiPageNum"].ToString());
				MatchCollection matchCollection=Regex.Matches(wikiPageContent.ToString(),@"\[\[.+?\]\]");
				foreach(Match m in matchCollection) {
					string mVal=m.Value;
					if(mVal.Contains("[[img:")
						|| mVal.Contains("[[keywords:")
						|| mVal.Contains("[[file:")
						|| mVal.Contains("[[folder:")
						|| mVal.Contains("[[list:")
						|| mVal.Contains("[[color:")
						|| mVal.Contains("[[filecloud:")
						|| mVal.Contains("[[foldercloud:")
						|| mVal.Contains("[[font:"))
					{
						continue;
					}
					long pageNum;
					mVal=mVal.TrimStart('[').TrimEnd(']');
					if(dictWikiPages.TryGetValue(mVal,out pageNum)) {//page title exist, replace with WikiPageNum instead of PageTitle
						string replace="[["+pageNum.ToString()+"]]";
						command="UPDATE wikipage SET PageContent=REPLACE(PageContent,'[["+POut.String(mVal)+"]]', '[["+pageNum.ToString()+"]]') WHERE WikiPageNum="+POut.String(wikiPageNum.ToString())+";";
						Db.NonQ(command);
					}
					else {//page title does not exist. Replace with [[0]]
						command="UPDATE wikipage SET PageContent=REPLACE(PageContent,'[["+POut.String(mVal)+"]]', '[[0]]') WHERE WikiPageNum="+POut.String(wikiPageNum.ToString())+";";
						Db.NonQ(command);
					}
				}
			}
			//Wikipagehist changes.
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.4.36 - Updating wikipagehist");//No translation in convert script.
			long minWikiPageNum=0;
			long maxWikiPageNum=100;
			bool doWhile=true;
			StringBuilder sbCommand=new StringBuilder();
			while(doWhile) {
				command="SELECT WikiPageNum,PageContent FROM wikipagehist WHERE WikiPageNum>="+minWikiPageNum+" AND WikiPageNum<"+maxWikiPageNum+ " ORDER BY WikiPageNum";
				DataTable tableWikiPageHist=Db.GetTable(command);
				if(tableWikiPageHist.Rows.Count==0) {
					//jump out
					doWhile=false;
					continue;
				}
				string wikiPageHistContent;
				long wikiPageHistNum;
				bool didIncrementRange=false;
				foreach(DataRow row in tableWikiPageHist.Rows) {
					wikiPageHistContent=row["PageContent"].ToString();
					wikiPageHistNum=PIn.Long(row["WikiPageNum"].ToString());
					MatchCollection matchCollection=Regex.Matches(wikiPageHistContent,@"\[\[.+?\]\]");
					foreach(Match m in matchCollection) {
						string mVal=m.Value;
						if(mVal.Contains("[[img:")
							|| mVal.Contains("[[keywords:")
							|| mVal.Contains("[[file:")
							|| mVal.Contains("[[folder:")
							|| mVal.Contains("[[list:")
							|| mVal.Contains("[[color:")
							|| mVal.Contains("[[filecloud:")
							|| mVal.Contains("[[foldercloud:")
							|| mVal.Contains("[[font:"))
						{
							continue;
						}
						long pageNum;
						mVal=mVal.TrimStart('[').TrimEnd(']');
						if(dictWikiPages.TryGetValue(mVal,out pageNum)) {
							string replace="[["+pageNum.ToString()+"]]";
							sbCommand.AppendLine("UPDATE wikipagehist SET PageContent=REPLACE(PageContent,'[["+POut.String(mVal)+"]]', '[["+pageNum.ToString()+"]]') WHERE WikiPageNum="+POut.String(wikiPageHistNum.ToString())+";");
						}
						else {//page title does not exist. Replace with [[0]]
							sbCommand.AppendLine("UPDATE wikipagehist SET PageContent=REPLACE(PageContent,'[["+POut.String(mVal)+"]]', '[[0]]') WHERE WikiPageNum="+POut.String(wikiPageHistNum.ToString())+";");
						}
					}
					if(matchCollection.Count>0) {
						if(sbCommand.Length>250000) {
							minWikiPageNum=wikiPageHistNum+1;
							maxWikiPageNum=minWikiPageNum+100;
							didIncrementRange=true;
							break;
						}
					}
				}
				if(sbCommand.Length>0) {
					Db.NonQ(sbCommand.ToString());
					sbCommand.Clear();
				}
				if(!didIncrementRange) {
					minWikiPageNum=maxWikiPageNum;
					maxWikiPageNum+=100;
				}
			}
		}

		private static void To17_4_37() {
			//update DoseSpot alerts to be included in 'All' category
			string command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='ODAllTypes' AND IsHQCategory=1";
			string alertCategoryNum=Db.GetScalar(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",14)";//DoseSpotProviderRegistered
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",14)";//DoseSpotProviderRegistered
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",15)";//DoseSpotClinicRegistered
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",15)";//DoseSpotClinicRegistered
				Db.NonQ(command);
			}
		}

		private static void To17_4_39() {
			string command;
			DataTable table;
			//Get a table of CarrierGroupNames where the ItemName is blank. 
			//When we converted the data type of CarrierGroupNames in 17_4_1, we added a Defintion with a blank ItemName. Now we want to remove it
			command="SELECT DefNum FROM definition WHERE Category=46 ";//CarrierGroupNames
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command+="AND ItemName=''";
			}
			else {
				command+="AND ItemName IS NULL";
			}
			table=Db.GetTable(command);
			long defNum;
			foreach(DataRow row in table.Rows) {
				defNum=PIn.Long(row["DefNum"].ToString());
				command="UPDATE carrier SET CarrierGroupName=0 WHERE CarrierGroupName="+POut.Long(defNum);
				Db.NonQ(command);
				command="DELETE FROM definition WHERE DefNum="+POut.Long(defNum);
				Db.NonQ(command);
			}
			//Move ClaimHistoryEdit permission from Global Lock Date to permission specific date.  Perm already exists.
			//Check to see if the global lock date is set.
			command="SELECT ValueString FROM preference WHERE PrefName='SecurityLockDate'";
			table=Db.GetTable(command);
			DateTime dateSecurityLock = DateTime.MinValue;
			if(table.Rows.Count>0) {
				dateSecurityLock=PIn.Date(table.Rows[0][0].ToString());
			}
			//Check to see if the global lock date days is set.
			command="SELECT ValueString FROM preference WHERE PrefName='SecurityLockDays'";
			table=Db.GetTable(command);
			int securityLockDays = 0;
			if(table.Rows.Count>0) {
				securityLockDays=PIn.Int(table.Rows[0][0].ToString());
			}
			if(dateSecurityLock!=DateTime.MinValue || securityLockDays!=0) {//No DB changes needed if we'd just set the grouppermission to default value.
				//Check to see if the security lock doesn't include admins.
				command="SELECT ValueString FROM preference WHERE PrefName='SecurityLockIncludesAdmin'";
				table=Db.GetTable(command);
				List<long> listSecurityAdminUserGroupNums = new List<long>();
				if(table.Rows.Count>0 && !PIn.Bool(table.Rows[0][0].ToString())) {
					//Get usergroupnum for usergroups that have security admin permission, so we don't include them later.
					command="SELECT UserGroupNum FROM grouppermission WHERE PermType=24";  //SecurityAdmin
					try {
						listSecurityAdminUserGroupNums=Db.GetListLong(command);
					}
					catch(Exception ex) {
						ex.DoNothing();//We don't care, just leave as empty list.  They'll just have to remove the lock date from admins
					}
				}
				//Finally update database
				command="UPDATE grouppermission SET NewerDate="+POut.Date(dateSecurityLock)+", NewerDays="+POut.Int(securityLockDays)
					+" WHERE PermType=95"; //95 ClaimHistoryEdit
				if(listSecurityAdminUserGroupNums.Count>0) {//Exclude admins
					command+=" AND UserGroupNum NOT IN("+string.Join(",",listSecurityAdminUserGroupNums)+")";
				}
				Db.NonQ(command);
			}
			//END ClaimHistoryEdit permission changes
	 }

		private static void To17_4_40() {
			string command;
			DataTable table;
			//We are running this section of code for HQ only
			//This is very uncommon and normally manual queries should be run instead of doing a convert script.
			command="SELECT ValueString FROM preference WHERE PrefName='DockPhonePanelShow'";
			table=Db.GetTable(command);
			if(table.Rows.Count > 0 && PIn.Bool(table.Rows[0][0].ToString())) {
				command="ALTER TABLE job CHANGE Description Implementation text NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE job ADD Requirements text NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE joblog ADD RequirementsRTF text NOT NULL";
				Db.NonQ(command);
			}
		}

		private static void To17_4_42() {
			string command;
			//Drop ClaimIdentifierOrig column from claim table
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE claim DROP COLUMN ClaimIdentifierOrig";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE claim DROP COLUMN ClaimIdentifierOrig";
				Db.NonQ(command);
			}
			command="SELECT ValueString FROM preference WHERE PrefName='AdjustmentsForceAttachToProc'";
			int forceAttachToProc = Db.GetInt(command);
			command="SELECT ValueString FROM preference WHERE PrefName='RigorousAdjustments'";
			int rigorousAdjustments = Db.GetInt(command);
			//Maintain functionality of the now deprecated preference if necessary.
			if(forceAttachToProc==1 && rigorousAdjustments==2) {
				command="UPDATE preference SET ValueString=1 WHERE PrefName='RigorousAdjustments'";
				Db.NonQ(command);
			}
			//Add the appointment triggers to the list of confirmation statuses excluded from sending eConfirms and eReminders.		
			//Generate our list of defnums to add to the different Preference.ValueStrings
			List<string> listTriggerDefNums=new List<string>();
			command="SELECT ValueString FROM preference WHERE PrefName='AppointmentTimeArrivedTrigger'";
			listTriggerDefNums.Add(PIn.String(Db.GetScalar(command)));
			command="SELECT ValueString FROM preference WHERE PrefName='AppointmentTimeDismissedTrigger'";
			listTriggerDefNums.Add(PIn.String(Db.GetScalar(command)));
			command="SELECT ValueString FROM preference WHERE PrefName='AppointmentTimeSeatedTrigger'";
			listTriggerDefNums.Add(PIn.String(Db.GetScalar(command)));
			//Fetch our 3 different Pref.ValueStrings
			command="SELECT ValueString FROM preference WHERE PrefName='ApptConfirmExcludeEConfirm'";
			List<string> listEConfirm=PIn.String(Db.GetScalar(command)).Split(',')
					.Where(x => !string.IsNullOrWhiteSpace(x))
					.Union(listTriggerDefNums).ToList();
			command="SELECT ValueString FROM preference WHERE PrefName='ApptConfirmExcludeESend'";
			List<string> listESend=PIn.String(Db.GetScalar(command)).Split(',')
					.Where(x => !string.IsNullOrWhiteSpace(x))
					.Union(listTriggerDefNums).ToList();
			command="SELECT ValueString FROM preference WHERE PrefName='ApptConfirmExcludeERemind'";
			List<string> listERemind=PIn.String(Db.GetScalar(command)).Split(',')
					.Where(x => !string.IsNullOrWhiteSpace(x))
					.Union(listTriggerDefNums).ToList();
			//Update the lists back at the database. Same query works for MySQL and Oracle.
			command="UPDATE preference SET ValueString='"+string.Join(",",listEConfirm.Select(x => POut.String(x)))
				+"' WHERE PrefName='ApptConfirmExcludeEConfirm';";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString='"+string.Join(",",listESend.Select(x => POut.String(x)))
				+"' WHERE PrefName='ApptConfirmExcludeESend';";
			Db.NonQ(command);
			command="UPDATE preference SET ValueString='"+string.Join(",",listERemind.Select(x => POut.String(x)))
				+"' WHERE PrefName='ApptConfirmExcludeERemind';";
			Db.NonQ(command);
		}

		private static void To17_4_46() {
			string command;
			command="CREATE INDEX MsgBoxCompound ON emailmessage (MsgDateTime,SentOrReceived)";//MySQL and Oracle.  Speeds up loading email inbox/outbox.
			Db.NonQ(command);
		}
		
		private static void To17_4_51() {
			string command;
			//Worst case scenario index on SentOrReceived column.  This will give a 2 to 3 times speed increase compared to not using any index.
			command="CREATE INDEX SentOrReceived ON emailmessage (SentOrReceived)";//MySQL and Oracle.
			Db.NonQ(command);
			//Index to speed up email message window loading.
			command="CREATE INDEX MsgHistoricAddresses ON emailmessage (SentOrReceived,RecipientAddress(50),FromAddress(50))";//MySQL and Oracle.
			Db.NonQ(command);
		}

		private static void To17_4_52() {
			string command;
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.4.52 - alter histappointment.ProcsColored and Note");
			if(DataConnection.DBtype==DatabaseType.MySql) {
				LargeTableHelper.AlterLargeTable("histappointment","HistApptNum",
					new List<Tuple<string,string>> { Tuple.Create("ProcsColored","text NOT NULL"),Tuple.Create("Note","text NOT NULL") });//modify both column data types
			}
			else {//oracle
				command="ALTER TABLE histappointment MODIFY ProcsColored clob";
				Db.NonQ(command);
				command="ALTER TABLE histappointment MODIFY Note clob";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.4.52");
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE tsitranslog MODIFY TransJson mediumtext NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle datatype is already clob which goes up to 4GB
			}
		}

		private static void To17_4_53() {
			string command;
			try {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					if(LargeTableHelper.IndexExists("appointment","AptStatus,AptDateTime")) {
						command="DROP INDEX StatusDate ON appointment";
						Db.NonQ(command);
					}
				}
				else {//oracle
					command="DROP INDEX StatusDate ON appointment";
					Db.NonQ(command);
				}
				command="CREATE INDEX StatusDate ON appointment (AptStatus,AptDateTime,ClinicNum)";
				Db.NonQ(command);
			}
			catch(Exception ex) {
				ex.DoNothing();//Just an index
			}
			//Increase size of GroupNum column in InsPlan table from 20 to 25
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE insplan CHANGE GroupNum GroupNum VARCHAR(25)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE insplan MODIFY (GroupNum varchar2(25))";
				Db.NonQ(command);
			}
			//This command was supposed to be run in To16_4_1, but the line Db.NonQ(command); was omitted after the command.
			command="UPDATE patientrace SET CdcrecCode='ASKU-ETHNICITY' WHERE Race=11";//DeclinedToSpecifyEthnicity		
			Db.NonQ(command);//Oracle compatible
			if(DataConnection.DBtype==DatabaseType.MySql) {
				 command="INSERT INTO preference(PrefName,ValueString) VALUES('OutstandingInsReportDateFilterTab','0')";//Default to RpOutstandingIns.DateFilterTab.DaysOld
				 Db.NonQ(command);
			}
			else {//oracle
				 command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 "
					+"FROM preference),'OutstandingInsReportDateFilterTab','0')";//Default to RpOutstandingIns.DateFilterTab.DaysOld
				 Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="UPDATE preference SET ValueString=CURDATE() WHERE PrefName='ClaimPaymentNoShowZeroDate'";
				Db.NonQ(command);
			}
			else {//oracle
				command="UPDATE preference SET ValueString=SYSDATE WHERE PrefName='ClaimPaymentNoShowZeroDate'";
				Db.NonQ(command);
			}
		}

		private static void To17_4_55() {
			string command;
			//There was a slight mistake in the 17_1_1 convert script. It added a display field for the recall list. This was a problem if the office
			//was previously using the default display fields. This had the affect of "deleting" all their display fields except for the WebSched one.
			//To fix this, we will delete that one that was added so that the office will go back to using the default display fields.
			command=@"SELECT *
					FROM displayfield
					WHERE Category=4";//DisplayFieldCategory.RecallList
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==1 && PIn.String(table.Rows[0]["InternalName"].ToString())=="WebSched") {
				command="DELETE FROM displayfield WHERE DisplayFieldNum="+POut.Long(PIn.Long(table.Rows[0]["DisplayFieldNum"].ToString()));
				Db.NonQ(command);
			}
		}
		
		private static void To17_4_57() {
			string command;
			try {
				//In 17.4.53 and 17.4.54 DocNum was backported to version already released.
				command="SELECT DocNum FROM sheet";	
				Db.GetTable(command);//Check if column exists, if it does not then this will result in an exception.
			}
			catch(Exception ex) {
				ex.DoNothing();
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sheet ADD DocNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE sheet ADD INDEX (DocNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sheet ADD DocNum number(20)";
					Db.NonQ(command);
					command="UPDATE sheet SET DocNum = 0 WHERE DocNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE sheet MODIFY DocNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX sheet_DocNum ON sheet (DocNum)";
					Db.NonQ(command);
				}
			}
		}

		private static void To17_4_64() {
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE signalod MODIFY MsgValue text NOT NULL"; //Change from varchar[255] to text.
				Db.NonQ(command);
				//Oracle allows up to 4 gigs in CLOB, not conver needed.
			}
		}

		private static void To17_4_66() {
			//Rewritten in 17.4.68 to correctly delete the entire set of duplicate claimprocs. Helper function created.
			CanadaDeleteDuplicatePreauthClaimprocs();
		}

		private static void To17_4_68() {
			CanadaDeleteDuplicatePreauthClaimprocs();
		}

		private static void To17_4_73() {
			string command;
			command="UPDATE preference SET ValueString='1' WHERE PrefName='ClaimPaymentNoShowZeroDate'";
			Db.NonQ(command);
		}

		private static void To17_4_78() {
			string command;
			ODEvent.Fire(ODEventType.ConvertDatabases,
				"Upgrading database to version: 17.4.78 - Preserving ortho treatment month defaults");//No translation in convert script.
			//The below code is intended to preserve the value from PrefName.OrthoDefaultMonthsTreat for patients that have completed ortho placement procs.
			//Previously if someone were to change PrefName.OrthoDefaultMonthsTreat the patients that did not have overrides 
			//would then use the new pref value and lose their original Tx Months Total.
			command="SELECT preference.ValueString FROM preference WHERE preference.PrefName='OrthoDefaultMonthsTreat'";
			string orthoDefaultMonths=Db.GetScalar(command);
			//Mimics ProcedureCodes.GetOrthoBandingCodeNums()
			command="SELECT preference.ValueString FROM preference WHERE preference.PrefName='OrthoPlacementProcsList'";
			string orthoPlacementProcsPref=Db.GetScalar(command);
			List<long> listOrthoPlacementProcCodeNums=orthoPlacementProcsPref.Split(new char[] { ',' }).Select(x => PIn.Long(x)).ToList();
			//Get all procedure codes that start with D8 if the preference is not currently set (preserves old behavior).
			if(string.IsNullOrEmpty(orthoPlacementProcsPref)) {
				command="SELECT procedurecode.CodeNum FROM procedurecode WHERE procedurecode.ProcCode LIKE 'D8%'";
				listOrthoPlacementProcCodeNums=Db.GetListLong(command);
			}
			if(listOrthoPlacementProcCodeNums.Count > 0) {
				//Get all PatNums for patients that have at least one completed ortho placement procedure.
				command="SELECT DISTINCT procedurelog.PatNum "
					+"FROM procedurelog "
					+"WHERE ProcStatus=2 "/*2=Complete*/
					+"AND procedurelog.CodeNum IN ("+string.Join(",",listOrthoPlacementProcCodeNums)+") ";
				List<long> listPatNumsWithOrthoTreatments=Db.GetListLong(command);
				if(listPatNumsWithOrthoTreatments.Count > 0) {
					command="UPDATE patientnote SET patientnote.OrthoMonthsTreatOverride="+orthoDefaultMonths+" "
						+"WHERE patientnote.PatNum IN ("+string.Join(",",listPatNumsWithOrthoTreatments)+") "
						+"AND patientnote.OrthoMonthsTreatOverride=-1 ";
					Db.NonQ(command);
				}
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 17.4.78");//No translation in convert script.
			//Clinics turned on or off alert
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='eServices' AND IsHQCategory=1";
			string alertCategoryNum=Db.GetScalar(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",17)";//ClinicsChanged
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",17)";//ClinicsChanged
				Db.NonQ(command);
			}
			//Internal clinics turned on or off alert
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='eServices' AND IsHQCategory=1";
			alertCategoryNum=Db.GetScalar(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",18)";//ClinicsChangedInternal
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",18)";//ClinicsChangedInternal
				Db.NonQ(command);
			}
		}

		private static void To17_4_90() {
			//Add permission to groups with existing Setup permission------------------------------------------------------
			string command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=8";//Setup
			DataTable table=Db.GetTable(command);
			long groupNum;
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					+"VALUES("+POut.Long(groupNum)+",157)";//ProcCodeEdit
				Db.NonQ(command);
			}
		}

		private static void To17_4_95() {
			FixB11013();
		}

		private static void To18_1_1() {
			string command;
			DataTable table;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('AgingNegativeAdjsByAdjDate','0')";//false by default, maintian current behavior
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'AgingNegativeAdjsByAdjDate','0')";//false by default, maintian current behavior
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('BillingDefaultsSinglePatient','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'BillingDefaultsSinglePatient','0')";
				Db.NonQ(command);
			}
			//Add PlancCategory column to payplan
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE payplan ADD PlanCategory bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE payplan ADD INDEX (PlanCategory)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE payplan ADD PlanCategory number(20)";
				Db.NonQ(command);
				command="UPDATE payplan SET PlanCategory = 0 WHERE PlanCategory IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE payplan MODIFY PlanCategory NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX payplan_PlanCategory ON payplan (PlanCategory)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE userquery ADD IsPromptSetup tinyint NOT NULL DEFAULT 1";//true by default, maintain current behavior
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE userquery ADD IsPromptSetup number(3)";
				Db.NonQ(command);
				command="UPDATE userquery SET IsPromptSetup = 1 WHERE IsPromptSetup IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE userquery MODIFY IsPromptSetup NOT NULL DEFAULT 1";//true by default, maintain current behavior
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE displayreport ADD IsVisibleInSubMenu tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE displayreport ADD IsVisibleInSubMenu number(3)";
				Db.NonQ(command);
				command="UPDATE displayreport SET IsVisibleInSubMenu = 0 WHERE IsVisibleInSubMenu IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE displayreport MODIFY IsVisibleInSubMenu NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ClinicAllowPatientsAtHeadquarters','0')";//False by default
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ClinicAllowPatientsAtHeadquarters','0')";//False by default
				Db.NonQ(command);
			}
			//Add 'AddNewUser' permission to groups with existing permission 'SecurityAdmin'------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=24";  //Security Admin
			table=Db.GetTable(command);
			long groupNum;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",158)";  //AddNewUser
					Db.NonQ(command);
				}		
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",158,0)"; //AddNewUser
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('DefaultUserGroup','0')"; //default to 0
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'DefaultUserGroup','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE confirmationrequest ADD TSPrior bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE confirmationrequest ADD INDEX (TSPrior)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE confirmationrequest ADD TSPrior number(20)";
				Db.NonQ(command);
				command="UPDATE confirmationrequest SET TSPrior = 0 WHERE TSPrior IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE confirmationrequest MODIFY TSPrior NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX confirmationrequest_TSPrior ON confirmationrequest (TSPrior)";
				Db.NonQ(command);
			}
			//Insert a new definition for an HQ CommLog Type
			command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=27";
			string maxOrder=Db.GetScalar(command);
			if(maxOrder=="") {
				maxOrder="0";
			}
			long defNum=0;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO definition (Category, ItemOrder, ItemName, ItemValue) VALUES (27,"+maxOrder+",'ODHQ','ODHQ')";
				defNum=Db.NonQ(command,true);
			}
			else {//oracle
				command="INSERT INTO definition (DefNum,Category, ItemOrder, ItemName, ItemValue) VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),27,"+maxOrder
					+",'ODHQ','ODHQ')";
				defNum=Db.NonQ(command,true,"DefNum","definition");
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS databasemaintenance";
				Db.NonQ(command);
				command=@"CREATE TABLE databasemaintenance (
						DatabaseMaintenanceNum bigint NOT NULL auto_increment PRIMARY KEY,
						MethodName varchar(255) NOT NULL,
						IsHidden tinyint NOT NULL,
						IsOld tinyint NOT NULL,
						DateLastRun datetime NOT NULL DEFAULT '0001-01-01 00:00:00'
						) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE databasemaintenance'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE databasemaintenance (
						DatabaseMaintenanceNum number(20) NOT NULL,
						MethodName varchar2(255),
						IsHidden number(3) NOT NULL,
						IsOld number(3) NOT NULL,
						DateLastRun date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT databasemaintenance_DatabaseMa PRIMARY KEY (DatabaseMaintenanceNum)
						)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedRecallAllowProvSelection','1')";//true by default
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedRecallAllowProvSelection','1')";//true by default
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE provider ADD WebSchedDescript text NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE provider ADD WebSchedDescript clob";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE provider ADD WebSchedImageLocation varchar(255) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE provider ADD WebSchedImageLocation varchar2(255)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimReportReceiveTime','')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ClaimReportReceiveTime','')";
				Db.NonQ(command);
			}
			//check databasemaintenance for DatesNoZeros, insert if not there and set IsOld to True or update to set IsOld to true
			command="SELECT MethodName FROM databasemaintenance WHERE MethodName='DatesNoZeros'";
			string methodName=Db.GetScalar(command);
			if(methodName=="") {//didn't find row in table, insert
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO databasemaintenance (MethodName, IsOld) VALUES ('DatesNoZeros',1)";//true by default
				}
				else {//oracle
					command="INSERT INTO databasemaintenance (DatabaseMaintenanceNum, MethodName, IsOld) VALUES ((SELECT COALESCE(MAX(DatabaseMaintenanceNum),0)+1 DatabaseMaintenanceNum FROM databasemaintenance),'DatesNoZeros',1)";
				}
			}
			else {//found row, update IsOld
				command="UPDATE databasemaintenance SET IsOld = 1 WHERE MethodName = 'DatesNoZeros'";//true by default
			}
			Db.NonQ(command);
			//Add permission to groups with AccountModule permission by default------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=3";  //AccountModule
			table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(groupNum)+",159)";  //ViewClaim permission
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
						 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",159,0)";  //ViewClaim permission
					Db.NonQ(command);
				}
			}
			//Add RepeatChargeTool permission to everyone------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
			   foreach(DataRow row in table.Rows) {
			      groupNum=PIn.Long(row["UserGroupNum"].ToString());
			      command="INSERT INTO grouppermission (UserGroupNum,PermType) "
			         +"VALUES("+POut.Long(groupNum)+",160)";
			      Db.NonQ(command);
			   }
			}
			else {//oracle
			   foreach(DataRow row in table.Rows) {
			      groupNum=PIn.Long(row["UserGroupNum"].ToString());
			      command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
			         +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",160,0)";
			      Db.NonQ(command);
			   }
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Non-Canadian
					command="UPDATE clearinghouse SET IsEraDownloadAllowed=2 WHERE IsEraDownloadAllowed=1";//IsEraDownloadAllowed=true changes to IsEraDownloadAllowed=EraBehavior.DownloadAndReceive
					Db.NonQ(command);
					command="ALTER TABLE clearinghouse MODIFY COLUMN IsEraDownloadAllowed TINYINT NOT NULL DEFAULT 2";//Default to EraBehaviors.DownloadAndReceive for Non-Canadian
					Db.NonQ(command);
				}
			}
			else {//oracle
				if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Non-Canadian
					command="UPDATE clearinghouse SET IsEraDownloadAllowed=2 WHERE IsEraDownloadAllowed=1";//IsEraDownloadAllowed=true changes to IsEraDownloadAllowed=EraBehavior.DownloadAndReceive
					Db.NonQ(command);
					command="ALTER TABLE clearinghouse MODIFY IsEraDownloadAllowed DEFAULT 2 NOT NULL";//Default to EraBehaviors.DownloadAndReceive for Non-Canadian
					Db.NonQ(command);
				}
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.1.1 | statement columns");//No translation in convert script.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE statement ADD ShortGUID varchar(30) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE statement ADD INDEX (ShortGUID)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE statement ADD ShortGUID varchar2(30)";
				Db.NonQ(command);
				command=@"CREATE INDEX statement_ShortGUID ON statement (ShortGUID)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE statement ADD StatementShortURL varchar(50) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE statement ADD StatementShortURL varchar2(50)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE statement ADD StatementURL varchar(255) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE statement ADD StatementURL varchar2(255)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE statement ADD SmsSendStatus tinyint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE statement ADD SmsSendStatus number(3)";
				Db.NonQ(command);
				command="UPDATE statement SET SmsSendStatus = 0 WHERE SmsSendStatus IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE statement MODIFY SmsSendStatus NOT NULL";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.1.1");//No translation in convert script.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('BillingDefaultsModesToText','')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'BillingDefaultsModesToText','')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('BillingDefaultsSmsTemplate',"+
					"'A new statement from [OfficeName] is available. Visit [StatementShortURL] to view it.')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'BillingDefaultsSmsTemplate',"+
					"'A new statement from [OfficeName] is available. Visit [StatementShortURL] to view it.')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedNewPatAllowProvSelection','0')";//off by default
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedNewPatAllowProvSelection','0')";//off by default
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE proctp ADD FeeAllowed double NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE proctp ADD FeeAllowed number(38,8)";
				Db.NonQ(command);
				command="UPDATE proctp SET FeeAllowed = 0 WHERE FeeAllowed IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE proctp MODIFY FeeAllowed NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				if(!LargeTableHelper.IndexExists("clockevent","EmployeeNum")) {//add index if it doesn't already exist
					command="ALTER TABLE clockevent ADD INDEX (EmployeeNum)";
					Db.NonQ(command);
				}
			}
			else {//oracle
				command="CREATE INDEX clockevent_EmployeeNum ON clockevent (EmployeeNum)";//add index
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.1.1 - Updating journalentry");//No translation in convert script.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE journalentry ADD SecUserNumEntry bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE journalentry ADD INDEX (SecUserNumEntry)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE journalentry ADD SecUserNumEntry number(20)";
				Db.NonQ(command);
				command="UPDATE journalentry SET SecUserNumEntry = 0 WHERE SecUserNumEntry IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE journalentry MODIFY SecUserNumEntry NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX journalentry_SecUserNumEntry ON journalentry (SecUserNumEntry)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE journalentry ADD SecDateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE journalentry ADD SecDateTEntry date";
				Db.NonQ(command);
				command="UPDATE journalentry SET SecDateTEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE SecDateTEntry IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE journalentry MODIFY SecDateTEntry NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE journalentry ADD SecUserNumEdit bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE journalentry ADD INDEX (SecUserNumEdit)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE journalentry ADD SecUserNumEdit number(20)";
				Db.NonQ(command);
				command="UPDATE journalentry SET SecUserNumEdit = 0 WHERE SecUserNumEdit IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE journalentry MODIFY SecUserNumEdit NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX journalentry_SecUserNumEdit ON journalentry (SecUserNumEdit)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE journalentry ADD SecDateTEdit timestamp";
				Db.NonQ(command);
				command="UPDATE journalentry SET SecDateTEdit = NOW()";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE journalentry ADD SecDateTEdit timestamp";
				Db.NonQ(command);
				command="UPDATE journalentry SET SecDateTEdit = SYSTIMESTAMP";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.1.1 - Updating transaction");//No translation in convert script.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE transaction ADD SecUserNumEdit bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE transaction ADD INDEX (SecUserNumEdit)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE transaction ADD SecUserNumEdit number(20)";
				Db.NonQ(command);
				command="UPDATE transaction SET SecUserNumEdit = 0 WHERE SecUserNumEdit IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE transaction MODIFY SecUserNumEdit NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX transaction_SecUserNumEdit ON transaction (SecUserNumEdit)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE transaction ADD SecDateTEdit timestamp";
				Db.NonQ(command);
				command="UPDATE transaction SET SecDateTEdit = NOW()";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE transaction ADD SecDateTEdit timestamp";
				Db.NonQ(command);
				command="UPDATE transaction SET SecDateTEdit = SYSTIMESTAMP";
				Db.NonQ(command);
			}
			//Create the EntryLog table
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS entrylog";
				Db.NonQ(command);
				command=@"CREATE TABLE entrylog (
						EntryLogNum bigint NOT NULL auto_increment PRIMARY KEY,
						UserNum bigint NOT NULL,
						FKeyType tinyint NOT NULL,
						FKey bigint NOT NULL,
						LogSource tinyint NOT NULL,
						EntryDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						INDEX(UserNum),
						INDEX(FKey),
						INDEX(EntryDateTime)
						) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE entrylog'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE entrylog (
						EntryLogNum number(20) NOT NULL,
						UserNum number(20) NOT NULL,
						FKeyType number(3) NOT NULL,
						FKey number(20) NOT NULL,
						LogSource number(3) NOT NULL,
						EntryDateTime date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT entrylog_EntryLogNum PRIMARY KEY (EntryLogNum)
						)";
				Db.NonQ(command);
				command=@"CREATE INDEX entrylog_UserNum ON entrylog (UserNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX entrylog_FKey ON entrylog (FKey)";
				Db.NonQ(command);
				command=@"CREATE INDEX entrylog_EntryDateTime ON entrylog (EntryDateTime)";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.1.1");//No translation in convert script.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ShowPlannedAppointmentPrompt','1')";//true by default.
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ShowPlannedAppointmentPrompt','1')";//true by default.
				Db.NonQ(command);
			}
			//Add permission to groups with existing permission TreatPlanEdit ------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=38";  //TreatPlanEdit 
			table=Db.GetTable(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(groupNum)+",162)";  //TreatPlanSign
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
						 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",162,0)";  //TreatPlanSign
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE provider ADD HourlyProdGoalAmt double NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE provider ADD HourlyProdGoalAmt number(38,8)";
				Db.NonQ(command);
				command="UPDATE provider SET HourlyProdGoalAmt = 0 WHERE HourlyProdGoalAmt IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE provider MODIFY HourlyProdGoalAmt NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('PayPlanAdjType','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'PayPlanAdjType','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE payplancharge ADD SecDateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE payplancharge ADD SecDateTEntry date";
				Db.NonQ(command);
				command="UPDATE payplancharge SET SecDateTEntry = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE SecDateTEntry IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE payplancharge MODIFY SecDateTEntry NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE payplancharge ADD SecDateTEdit timestamp";
				Db.NonQ(command);
				command="UPDATE payplancharge SET SecDateTEdit = NOW()";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE payplancharge ADD SecDateTEdit timestamp";
				Db.NonQ(command);
				command="UPDATE payplancharge SET SecDateTEdit = SYSTIMESTAMP";
				Db.NonQ(command);
			}
			#region Web Sched New Pat Appt Appointment Types
			#region Preferences
			//Get all preferences that will help convert our Web Sched New Pat Appts over to forcefully using appointment types.
			command="SELECT ValueString FROM preference WHERE preference.PrefName = 'WebSchedNewPatApptTimePattern'";
			string prefWebSchedNewPatApptTimePattern=Db.GetScalar(command);
			command="SELECT ValueString FROM preference WHERE preference.PrefName = 'WebSchedNewPatApptProcs'";
			string prefWebSchedNewPatApptProcs=Db.GetScalar(command);
			command="SELECT ValueString FROM preference WHERE preference.PrefName = 'AppointmentTimeIncrement'";
			string prefAppointmentTimeIncrement=Db.GetScalar(command);
			//Convert the current time pattern into a five minute increment pattern because appointment types ALWAYS store the pattern in 5 min increments.
			StringBuilder patternConverted=new StringBuilder();
			for(int i=0;i<prefWebSchedNewPatApptTimePattern.Length;i++) {
				patternConverted.Append(prefWebSchedNewPatApptTimePattern.Substring(i,1));
				if(prefAppointmentTimeIncrement=="10") {
					patternConverted.Append(prefWebSchedNewPatApptTimePattern.Substring(i,1));
				}
				if(prefAppointmentTimeIncrement=="15") {
					patternConverted.Append(prefWebSchedNewPatApptTimePattern.Substring(i,1));
					patternConverted.Append(prefWebSchedNewPatApptTimePattern.Substring(i,1));
				}
			}
			if(patternConverted.ToString()=="") {
				if(prefAppointmentTimeIncrement=="15") {
					patternConverted.Append("///XXX///");
				}
				else {
					patternConverted.Append("//XX//");
				}
			}
			#endregion
			long appointmentTypeNum=0;
			long defNumApptTypeDefault=0;
			#region Default Appointment Type
			string defaultApptTypeName="WebSched New Patient Default";
			int attempts=0;
			//Make sure that the name of the new appointment type is unique.
			do {
				if(attempts > 0) {
					defaultApptTypeName=defaultApptTypeName+" "+POut.Int(attempts);
				}
				command="SELECT COUNT(*) FROM appointmenttype WHERE AppointmentTypeName = '"+POut.String(defaultApptTypeName)+"'";
				attempts++;
			} while(Db.GetCount(command)!="0");
			//Get the ItemOrder for the new default appointment type.  We want this appointment type to be at the end of the list.
			command="SELECT COALESCE(MAX(ItemOrder),-1)+1 FROM appointmenttype";
			int defaultApptTypeItemOrder=PIn.Int(Db.GetCount(command));
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO appointmenttype (AppointmentTypeName,AppointmentTypeColor,ItemOrder,IsHidden,Pattern,CodeStr) "
					+"VALUES("
					+"'"+POut.String(defaultApptTypeName)+"',"//AppointmentTypeName
					+"0,"//AppointmentTypeColor
					+POut.Int(defaultApptTypeItemOrder)+","//ItemOrder
					+"0,"//IsHidden
					+"'"+POut.String(patternConverted.ToString())+"',"//Pattern
					+"'"+POut.String(prefWebSchedNewPatApptProcs)+"')";//CodeStr
				appointmentTypeNum=Db.NonQ(command,true);
			}
			else {//oracle
				command="INSERT INTO appointmenttype (AppointmentTypeNum,AppointmentTypeName,AppointmentTypeColor,ItemOrder,IsHidden,Pattern,CodeStr) "
					+"VALUES("
					+"(SELECT COALESCE(MAX(AppointmentTypeNum),0)+1 FROM appointmenttype),"//AppointmentTypeNum
					+"'"+POut.String(defaultApptTypeName)+"',"//AppointmentTypeName
					+"0,"//AppointmentTypeColor
					+POut.Int(defaultApptTypeItemOrder)+","//ItemOrder
					+"0,"//IsHidden
					+"'"+POut.String(patternConverted.ToString())+"',"//Pattern
					+"'"+POut.String(prefWebSchedNewPatApptProcs)+"')";//CodeStr
				appointmentTypeNum=Db.NonQ(command,true);
			}
			#endregion
			#region Default Definition
			string defaultItemName="New Patient General";
			attempts=0;
			//Make sure that the name of the new appointment type is unique.
			do {
				if(attempts > 0) {
					defaultItemName=defaultItemName+" "+POut.Int(attempts);
				}
				command="SELECT COUNT(*) FROM definition "
					+"WHERE Category = 42 "//WebSchedNewPatApptTypes
					+"AND ItemName = '"+POut.String(defaultItemName)+"'";
				attempts++;
			} while(Db.GetCount(command)!="0");
			//We need to increment all ItemOrders for the WebSchedNewPatApptTypes PRIOR to inserting the definition.
			command="SELECT DefNum FROM definition WHERE Category = 42 ORDER BY ItemOrder";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				//ItemOrder is 0-based on the definition table so increment i by 1 in order to make room for an ItemOrder of 0.
				//Users cannot hide entries within the WebSchedNewPatApptTypes category so we don't have to worry about how they are treated.
				command="UPDATE definition SET ItemOrder = "+POut.Int(i+1)+" "
					+"WHERE DefNum = "+POut.String(table.Rows[i]["DefNum"].ToString());
				Db.NonQ(command);
			}
			//Insert the new definition with a hardcoded ItemOrder of 0 because we just incremented all existing defs in the category by 1.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
					+"VALUES("
					+"42,"//Category - WebSchedNewPatApptTypes
					+"0,"//ItemOrder
					+"'"+POut.String(defaultItemName)+"',"//ItemName
					+"'',"//ItemValue
					+"0,"//ItemColor
					+"0)";//IsHidden
				defNumApptTypeDefault=Db.NonQ(command,true);
			}
			else {//oracle
				command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
					+"VALUES("
					+"(SELECT COALESCE(MAX(DefNum),0)+1 FROM definition),"//DefNum
					+"42,"//Category - WebSchedNewPatApptTypes
					+"0,"//ItemOrder
					+"'"+POut.String(defaultItemName)+"',"//ItemName
					+"'',"//ItemValue
					+"0,"//ItemColor
					+"0)";//IsHidden
				defNumApptTypeDefault=Db.NonQ(command,true);
			}
			#endregion
			#region Link Definitions to Appointment Type
			//WebSchedNewPatApptTypes definitions are associated to one and only one appointment type and it is now a requirement.
			//Link all definitions to the new appointment type that was just created.
			command="SELECT DefNum FROM definition WHERE Category = 42 ORDER BY ItemOrder";
			List<long> listWSNPADefNums=Db.GetListLong(command);
			foreach(long defNumApptType in listWSNPADefNums) {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO deflink (DefNum,FKey,LinkType) "
						+"VALUES("
						+POut.Long(defNumApptType)+","//DefNum
						+POut.Long(appointmentTypeNum)+","//FKey
						+"2)";//LinkType.  2 - AppointmentType
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO deflink (DefLinkNum,DefNum,FKey,LinkType) "
						+"VALUES("
						+"(SELECT COALESCE(MAX(DefLinkNum),0)+1 FROM deflink),"//DefLinkNum
						+POut.Long(defNumApptType)+","//DefNum
						+POut.Long(appointmentTypeNum)+","//FKey
						+"2)";//LinkType.  2 - AppointmentType
					Db.NonQ(command);
				}
			}
			#endregion
			#region Link Operatories to All WebSchedNewPatApptTypes Definitions
			//Each operatory can support multiple appointment types being scheduled within them.
			//Therefore, make an entry in the deflink table for each operatory that is set up correctly for Web Sched New Pat Appt.
			command="SELECT OperatoryNum FROM operatory WHERE IsNewPatAppt=1";
			List<long> listWSNPAOpNums=Db.GetListLong(command);
			foreach(long operatoryNum in listWSNPAOpNums) {
				//Insert an deflink for every WebSchedNewPatApptTypes definition (preserves old behavior).
				foreach(long defNumApptType in listWSNPADefNums) {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO deflink (DefNum,FKey,LinkType) "
							+"VALUES("
							+POut.Long(defNumApptType)+","//DefNum
							+POut.Long(operatoryNum)+","//FKey
							+"3)";//LinkType.  3 - Operatory
						Db.NonQ(command);
					}
					else {//oracle
						command="INSERT INTO deflink (DefLinkNum,DefNum,FKey,LinkType) "
							+"VALUES("
							+"(SELECT COALESCE(MAX(DefLinkNum),0)+1 FROM deflink),"//DefLinkNum
							+POut.Long(defNumApptType)+","//DefNum
							+POut.Long(operatoryNum)+","//FKey
							+"2)";//LinkType.  3 - Operatory
						Db.NonQ(command);
					}
				}
			}
			#endregion
			#endregion
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('MiddleTierCacheFees','1')";//true by default.
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'MiddleTierCacheFees','1')";//true by default.
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('PatientCommunicationDateFormat','d')";//formats dates as "03/15/2018"
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'PatientCommunicationDateFormat','d')";//formats dates as "03/15/2018"
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.1.1 - add index on securitylog.UserNum");
			if(DataConnection.DBtype==DatabaseType.MySql) {
				if(!LargeTableHelper.IndexExists("securitylog","UserNum")) {
					LargeTableHelper.AlterLargeTable("securitylog","SecurityLogNum",null,new List<Tuple<string,string>> { Tuple.Create("UserNum","") });//no need to send index name
				}
			}
			else {//oracle
				try {
					command=@"CREATE INDEX securitylog_UserNum ON securitylog (UserNum)";
					Db.NonQ(command);
				}
				catch(Exception ex) {
					ex.DoNothing();//just an index, do nothing
				}
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.1.1");
			command="SELECT ValueString FROM preference WHERE PrefName='WebSchedNewPatConfirmStatus'";
			string prefVal=Db.GetScalar(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedRecallConfirmStatus','"+POut.String(prefVal)+"')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedRecallConfirmStatus','"+POut.String(prefVal)+"')";
				Db.NonQ(command);
			}
			//Add WebSchedRecallApptCreated alert
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes' AND IsHQCategory=1";
			string alertCategoryNum=Db.GetScalar(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",16)";//WebSchedRecallApptCreated
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",16)";//WebSchedRecallApptCreated
				Db.NonQ(command);
			}
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='eServices' AND IsHQCategory=1";
			alertCategoryNum=Db.GetScalar(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES ("+alertCategoryNum+",16)";//WebSchedRecallApptCreated
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO alertcategorylink (AlertCategoryLinkNum,AlertCategoryNum,AlertType) "+
					"VALUES ((SELECT MAX(AlertCategoryLinkNum)+1 FROM alertcategorylink),"+alertCategoryNum+",16)";//WebSchedRecallApptCreated
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('FutureTransDatesAllowed','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'FutureTransDatesAllowed','0')";
				Db.NonQ(command);
			}
			//Add ODDiscountPlan row to displayreport table
			command="SELECT MAX(displayreport.ItemOrder) FROM displayreport WHERE displayreport.Category = 3"; //List
			long itemorder=Db.GetLong(command)+1;//Get the next available ItemOrder for the list Category to put this new report last.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODDiscountPlan',"+POut.Long(itemorder)+",'Discount Plans',3,0)";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO displayreport(DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODDiscountPlan',"+POut.Long(itemorder)+",'Discount Plans',3,0)";
				Db.NonQ(command);
			}
			long reportNumDiscountPlan=Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODDiscountPlan'");
			//Give usergroups with permission for an existing report access to this new report.
			long reportNumInsPlans=Db.GetLong("SELECT DisplayReportNum FROM displayreport WHERE InternalName='ODInsurancePlans'");
			DataTable userGroupTable=Db.GetTable("SELECT UserGroupNum FROM grouppermission WHERE PermType=22 AND FKey="+POut.Long(reportNumInsPlans));
			foreach(DataRow row in userGroupTable.Rows) {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//Insert Reports permission (22) for the Discount Plan report.
					command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES('0001-01-01',0,"+POut.Long(PIn.Long(row["UserGroupNum"].ToString()))+",22,"+POut.Long(reportNumDiscountPlan)+")";
					Db.NonQ(command);
				}
				else {//oracle
					  //Insert Reports permission (22) for the Discount Plan report.
					command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
						+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission)"
						+","+"TO_DATE('0001-01-01','YYYY-MM-DD'),0,"+POut.Long(PIn.Long(row["UserGroupNum"].ToString()))+",22,"
						+POut.Long(reportNumDiscountPlan)+")";
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE apptreminderrule ADD DoNotSendWithin bigint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE apptreminderrule ADD DoNotSendWithin number(20)";
				Db.NonQ(command);
				command="UPDATE apptreminderrule SET DoNotSendWithin = 0 WHERE DoNotSendWithin IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE apptreminderrule MODIFY DoNotSendWithin NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE carrier ADD IsCoinsuranceInverted tinyint NOT NULL";//Default to false
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE carrier ADD IsCoinsuranceInverted number(3)";
				Db.NonQ(command);
				command="UPDATE carrier SET IsCoinsuranceInverted = 0 WHERE IsCoinsuranceInverted IS NULL";//Default to false
				Db.NonQ(command);
				command="ALTER TABLE carrier MODIFY IsCoinsuranceInverted NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE lettermerge ADD ImageFolder bigint NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE lettermerge ADD ImageFolder number(20)";
				Db.NonQ(command);
				command="UPDATE lettermerge SET ImageFolder = 0 WHERE ImageFolder IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE lettermerge MODIFY ImageFolder NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ApptEConfirm2ClickConfirmation','1')";//true by default, maintain current behavior
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ApptEConfirm2ClickConfirmation','1')";//true by default, maintain current behavior
				Db.NonQ(command);
			}
			//check databasemaintenance for CanadaCarriersCdaMissingInfo, insert if not there and set IsOld to True or update to set IsOld to true
			command="SELECT MethodName FROM databasemaintenance WHERE MethodName='CanadaCarriersCdaMissingInfo'";
			methodName=Db.GetScalar(command);
			if(methodName=="") {//didn't find row in table, insert
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO databasemaintenance (MethodName,IsOld) VALUES ('CanadaCarriersCdaMissingInfo',1)";
				}
				else {//oracle
					command="INSERT INTO databasemaintenance (DatabaseMaintenanceNum,MethodName,IsOld) VALUES ((SELECT COALESCE(MAX(DatabaseMaintenanceNum),0)+1 DatabaseMaintenanceNum FROM databasemaintenance),'CanadaCarriersCdaMissingInfo',1)";
				}
			}
			else {//found row, update IsOld
				command="UPDATE databasemaintenance SET IsOld = 1 WHERE MethodName = 'CanadaCarriersCdaMissingInfo'";
			}
			command="UPDATE clearinghouse SET ResponsePath='"+POut.String(@"C:\Program Files (x86)\CDA\ICD\")+"' WHERE Description='ITRANS'";
			Db.NonQ(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('ItransImportFields','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ItransImportFields','0')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedNewPatDoAuthEmail','0')";//off by default
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedNewPatDoAuthEmail','0')";//off by default
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedNewPatDoAuthText','0')";//off by default
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedNewPatDoAuthText','0')";//off by default
				Db.NonQ(command);
			}
			//Add providerclinic table
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS providerclinic";
				Db.NonQ(command);
				command=@"CREATE TABLE providerclinic (
						ProviderClinicNum bigint NOT NULL auto_increment PRIMARY KEY,
						ProvNum bigint NOT NULL,
						ClinicNum bigint NOT NULL,
						DEANum varchar(15) NOT NULL,
						INDEX(ProvNum),
						INDEX(ClinicNum)
						) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE providerclinic'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE providerclinic (
						ProviderClinicNum number(20) NOT NULL,
						ProvNum number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						DEANum varchar2(15),
						CONSTRAINT providerclinic_ProviderClinicN PRIMARY KEY (ProviderClinicNum)
						)";
				Db.NonQ(command);
				command=@"CREATE INDEX providerclinic_ProvNum ON providerclinic (ProvNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX providerclinic_ClinicNum ON providerclinic (ClinicNum)";
				Db.NonQ(command);
			}
			//Insert ProvNums and DEANums into providerclinic table
			command="SELECT ProvNum,DEANum FROM provider";
			DataTable tableProvs=Db.GetTable(command);
			foreach(DataRow row in tableProvs.Rows) {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO providerclinic (ProvNum,ClinicNum,DEANum) "
						+"VALUES ("+POut.String(row[0].ToString())+","//ProvNum
							+"'0',"//ClinicNum
							+"'"+POut.String(row[1].ToString())+"')";//DEANum
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO providerclinic (ProviderClinicNum,ProvNum,ClinicNum,DEANum) "
						+"VALUES ((SELECT COALESCE(MAX(ProviderClinicNum),0)+1 FROM providerclinic),"
							+POut.String(row[0].ToString())+","//ProvNum
							+"'0',"//ClinicNum
							+POut.String(row[1].ToString())+")";//DEANum
					Db.NonQ(command);
				}
			}
			//Add FKey to ClinicNum in userodpref
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE userodpref ADD ClinicNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE userodpref ADD INDEX (ClinicNum)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE userodpref ADD ClinicNum number(20)";
				Db.NonQ(command);
				command="UPDATE userodpref SET ClinicNum = 0 WHERE ClinicNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE userodpref MODIFY ClinicNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX userodpref_ClinicNum ON userodpref (ClinicNum)";
				Db.NonQ(command);
			}
			//Add WebSchedNewPatWebFormsURL redirect
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedNewPatWebFormsURL','')";//empty by default
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedNewPatWebFormsURL','')";//empty by default
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('AllowPrepayProvider','0')";
				Db.NonQ(command);
			}
			else {//oracle
				command=@"INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM 
					preference),'AllowPrepayProvider','0')";
				Db.NonQ(command);
			}
			//Add permission to groups with existing permission ProcComplEdit ------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=10"; //ProcComplEdit 
			List<long> listUserGroupNums=Db.GetListLong(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(long gNum in listUserGroupNums) {
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(gNum)+",163)";//ProcExistingEdit
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(long gNum in listUserGroupNums) {
					command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType,FKey) "
						 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(gNum)+",163,0)";//ProcExistingEdit
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ArchiveDate','0001-01-01')";//mindate by default.
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ArchiveDate','0001-01-01')";//mindate by default.
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ArchivePassHash','')";//blank by default.
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ArchivePassHash','')";//blank by default.
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ArchiveServerName','')";//blank by default.
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ArchiveServerName','')";//blank by default.
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ArchiveServerURI','')";//blank by default.
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ArchiveServerURI','')";//blank by default.
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ArchiveUserName','')";//blank by default.
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ArchiveUserName','')";//blank by default.
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ShowAutoDeposit','0')";//False by default
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ShowAutoDeposit','0')";//False by default
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE deposit ADD Batch varchar(25) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE deposit ADD Batch varchar2(25)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE deposit ADD DepositAccountNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE deposit ADD INDEX (DepositAccountNum)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE deposit ADD DepositAccountNum number(20)";
				Db.NonQ(command);
				command="UPDATE deposit SET DepositAccountNum = 0 WHERE DepositAccountNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE deposit MODIFY DepositAccountNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX deposit_DepositAccountNum ON deposit (DepositAccountNum)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="SELECT DefNum FROM definition WHERE definition.Category=40";//ClaimPaymentGroups
				DataTable tableClaimPayGroupDefs=Db.GetTable(command);
				//If there is only one claim payment group, hide it (important per Nathan to simplify window)
				if(tableClaimPayGroupDefs.Rows.Count==1) {
					command="UPDATE definition SET IsHidden=1 WHERE definition.DefNum="+tableClaimPayGroupDefs.Rows[0]["DefNum"].ToString();
					Db.NonQ(command);
				}
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE payplancharge ADD StatementNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE payplancharge ADD INDEX (StatementNum)";
					Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE payplancharge ADD StatementNum number(20)";
				Db.NonQ(command);
				command="UPDATE payplancharge SET StatementNum = 0 WHERE StatementNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE payplancharge MODIFY StatementNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX payplancharge_StatementNum ON payplancharge (StatementNum)";
				Db.NonQ(command);
			}
			//OrthoInsight3D bridge-----------------------------------------------------------------
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					 +") VALUES("
					 +"'OrthoInsight3d', "
					 +"'Ortho Insight 3d from http://www.motionview3d.com', "
					 +"'0', "
					 +"'"+POut.String(@"C:/Program Files/Ortho Insight 3D/OI3DFull.exe")+"', "
					 +"'"+POut.String(@"")+"', "//leave blank if none
					 +"'')";
				long programNum = Db.NonQ(command,true);
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
					 +"'OrthoInsight3d')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					 +") VALUES("
					+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
					 +"'OrthoInsight3d', "
					 +"'Ortho Insight 3d from http://www.motionview3d.com', "
					 +"'0', "
					 +"'"+POut.String(@"C:/Program Files/Ortho Insight 3D/OI3DFull.exe")+"', "
					 +"'"+POut.String(@"")+"', "//leave blank if none
					 +"'')";
				long programNum = Db.NonQ(command,true,"ProgramNum","program");
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+"'"+POut.Long(programNum)+"', "
					+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
					+"'0', "
					+"'0')";
				Db.NonQ(command);
				command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
					 +"VALUES ("
					 +"(SELECT COALESCE(MAX(ToolButItemNum),0)+1 FROM toolbutitem),"
					 +"'"+POut.Long(programNum)+"', "
					 +"'2', "//ToolBarsAvail.ChartModule
					 +"'OrthoInsight3d')";
				Db.NonQ(command);
			}//end OrthoInsight3D bridge
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE creditcard ADD PaySimpleToken varchar(255) NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE creditcard ADD PaySimpleToken varchar2(255)";
				Db.NonQ(command);
			}
			//Insert PaySimple bridge-----------------------------------------------------------------
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"'PaySimple', "
					+"'PaySimple from www.paysimple.com', "
					+"'0', "
					+"'', "
					+"'', "
					+"'')";
				long programNum=Db.NonQ(command,true);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(programNum)+"', "
					+"'PaySimple API User Name', "
					+"'')";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(programNum)+"', "
					+"'PaySimple API Key', "
					+"'')";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(programNum)+"', "
					+"'PaySimple Payment Type', "
					+"'0')";
				Db.NonQ(command);
			}
			else {//oracle	
				command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
					+"'PaySimple', "
					+"'PaySimple from www.paysimple.com', "
					+"'0', "
					+"'', "
					+"'', "
					+"'')";
				long programNum=Db.NonQ(command,true,"ProgramNum","program");
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+"'"+POut.Long(programNum)+"', "
					+"'PaySimple API User Name', "
					+"'', "
					+"'0')";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+"'"+POut.Long(programNum)+"', "
					+"'PaySimple API Key', "
					+"'', "
					+"'0')";
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+"'"+POut.Long(programNum)+"', "
					+"'PaySimple Payment Type', "
					+"'0', "
					+"'0')";
				Db.NonQ(command);
			}//end PaySimple bridge
			//Check databasemaintenance for ProcedurelogMultipleClaimProcForInsSub, insert if not there and set IsOld to true or update to set IsOld to true
			command="SELECT MethodName FROM databasemaintenance WHERE MethodName='ProcedurelogMultipleClaimProcForInsSub'";
			methodName=Db.GetScalar(command);
			if(methodName=="") {//didn't find row in table, insert
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO databasemaintenance (MethodName, IsOld) VALUES ('ProcedurelogMultipleClaimProcForInsSub',1)";//true by default
				}
				else {//oracle
					command="INSERT INTO databasemaintenance (DatabaseMaintenanceNum, MethodName, IsOld) VALUES ((SELECT COALESCE(MAX(DatabaseMaintenanceNum),0)+1 DatabaseMaintenanceNum FROM databasemaintenance),'ProcedurelogMultipleClaimProcForInsSub',1)";
				}
			}
			else {//found row, update IsOld
				command="UPDATE databasemaintenance SET IsOld = 1 WHERE MethodName = 'ProcedurelogMultipleClaimProcForInsSub'";//true by default
			}
			Db.NonQ(command);
			//Check databasemaintenance for InsSubNumMismatchPlanNum, insert if not there and set IsOld to true or update to set IsOld to true
			command="SELECT MethodName FROM databasemaintenance WHERE MethodName='InsSubNumMismatchPlanNum'";
			methodName=Db.GetScalar(command);
			if(methodName=="") {//didn't find row in table, insert
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO databasemaintenance (MethodName, IsOld) VALUES ('InsSubNumMismatchPlanNum',1)";//true by default
				}
				else {//oracle
					command="INSERT INTO databasemaintenance (DatabaseMaintenanceNum, MethodName, IsOld) VALUES ((SELECT COALESCE(MAX(DatabaseMaintenanceNum),0)+1 DatabaseMaintenanceNum FROM databasemaintenance),'InsSubNumMismatchPlanNum',1)";
				}
			}
			else {//found row, update IsOld
				command="UPDATE databasemaintenance SET IsOld = 1 WHERE MethodName = 'InsSubNumMismatchPlanNum'";//true by default
			}
			Db.NonQ(command);
			//Check databasemaintenance for ProcedurelogTpAttachedToClaim, insert if not there and set IsOld to true or update to set IsOld to true
			command="SELECT MethodName FROM databasemaintenance WHERE MethodName='ProcedurelogTpAttachedToClaim'";
			methodName=Db.GetScalar(command);
			if(methodName=="") {//didn't find row in table, insert
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO databasemaintenance (MethodName, IsOld) VALUES ('ProcedurelogTpAttachedToClaim',1)";//true by default
				}
				else {//oracle
					command="INSERT INTO databasemaintenance (DatabaseMaintenanceNum, MethodName, IsOld) VALUES ((SELECT COALESCE(MAX(DatabaseMaintenanceNum),0)+1 DatabaseMaintenanceNum FROM databasemaintenance),'ProcedurelogTpAttachedToClaim',1)";
				}
			}
			else {//found row, update IsOld
				command="UPDATE databasemaintenance SET IsOld = 1 WHERE MethodName = 'ProcedurelogTpAttachedToClaim'";//true by default
			}
			Db.NonQ(command);
			//Add clinic num to sheets
			if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sheet ADD ClinicNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE sheet ADD INDEX (ClinicNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sheet ADD ClinicNum number(20)";
					Db.NonQ(command);
					command="UPDATE sheet SET ClinicNum = 0 WHERE ClinicNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE sheet MODIFY ClinicNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX sheet_ClinicNum ON sheet (ClinicNum)";
					Db.NonQ(command);
				}
		}//End of 18_1_1() method

		private static void To18_1_5() {
			string command;
			if(DataConnection.DBtype==DatabaseType.MySql) {
				//May have already been converted in 17_4_64. This was a backport to stable originally.
				string databaseName=Db.GetScalar("SELECT DATABASE()");
				command=@"SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS 
					WHERE TABLE_SCHEMA='"+POut.String(databaseName)+@"'
					AND TABLE_NAME='signalod' 
					AND COLUMN_NAME='MsgValue'";
				string dataType=Db.GetScalar(command);
				if(dataType.ToLower()!="text") {
					command="ALTER TABLE signalod MODIFY MsgValue text NOT NULL"; //Change from varchar[255] to text.
					Db.NonQ(command);
				}
				//Oracle allows up to 4 gigs in CLOB, not conver needed.
			}
		}

		private static void To18_1_7() {
			//Rewritten in 18.1.9 to correctly delete the entire set of duplicate claimprocs. Helper function created.
			CanadaDeleteDuplicatePreauthClaimprocs();
		}

		private static void To18_1_9() {
			CanadaDeleteDuplicatePreauthClaimprocs();
		}

		private static void To18_1_14() {
			string command;
			command="SELECT ValueString FROM preference WHERE PrefName='ClaimPaymentNoShowZeroDate'";
			DataTable table=Db.GetTable(command);
			try {
				int.Parse(table.Rows[0][0].ToString());
				//If succeeds, then has already been converted to an int from a date in version 17.4.73.
			}
			catch(Exception ex) {
				ex.DoNothing();
				//If fails, then the preference needs to be converted to and int.
				command="UPDATE preference SET ValueString='1' WHERE PrefName='ClaimPaymentNoShowZeroDate'";
				Db.NonQ(command);
			}
		}

		private static void To18_1_16() {
			string command;
			//Remove 'ApptEConfirmStatusSent' from 'ApptConfirmExcludeESend' so that multiple eConfirmations can be sent for one appointment.
			command="SELECT ValueString FROM preference WHERE PrefName='ApptEConfirmStatusSent'";
			string defNumConfirmSent=Db.GetScalar(command);
			command="SELECT ValueString FROM preference WHERE PrefName='ApptConfirmExcludeESend'";
			List<string> listDefNums=Db.GetScalar(command).Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).ToList();
			listDefNums.RemoveAll(x => x==defNumConfirmSent);
			string defNumsExclude=string.Join(",",listDefNums);
			command="UPDATE preference SET ValueString='"+POut.String(defNumsExclude)+"' WHERE PrefName='ApptConfirmExcludeESend'";
			Db.NonQ(command);
		}

		private static void To18_1_19() {
			string command;
			//Insert iRYS bridge program property-----------------------------------------------------------------
			command="SELECT ProgramNum FROM program WHERE ProgName = 'iRYS'";
			long iRYSProgNum=PIn.Long(Db.GetScalar(command));
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
					+") VALUES("
					+"'"+POut.Long(iRYSProgNum)+"', "
					+"'Birthdate format (default dd,MM,yyyy)', "
					+"'dd,MM,yyyy')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
					+") VALUES("
					+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
					+"'"+POut.Long(iRYSProgNum)+"', "
					+"'Birthdate format (default dd,MM,yyyy)', "
					+"'dd,MM,yyyy', "
					+"'0')";
				Db.NonQ(command);
			}//End iRYS
		}

		private static void To18_1_21() {
			string command;
			//turn off single patient statements
			command="UPDATE preference SET ValueString = 0 WHERE PrefName LIKE 'BillingDefaultsSinglePatient'";
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,
				"Upgrading database to version: 18.1.21 - Preserving ortho treatment month defaults");//No translation in convert script.
			//The below code is intended to preserve the value from PrefName.OrthoDefaultMonthsTreat for patients that have completed ortho placement procs.
			//Previously if someone were to change PrefName.OrthoDefaultMonthsTreat the patients that did not have overrides 
			//would then use the new pref value and lose their original Tx Months Total.
			command="SELECT preference.ValueString FROM preference WHERE preference.PrefName='OrthoDefaultMonthsTreat'";
			string orthoDefaultMonths=Db.GetScalar(command);
			//Mimics ProcedureCodes.GetOrthoBandingCodeNums()
			command="SELECT preference.ValueString FROM preference WHERE preference.PrefName='OrthoPlacementProcsList'";
			string orthoPlacementProcsPref=Db.GetScalar(command);
			List<long> listOrthoPlacementProcCodeNums=orthoPlacementProcsPref.Split(new char[] { ',' }).Select(x => PIn.Long(x)).ToList();
			//Get all procedure codes that start with D8 if the preference is not currently set (preserves old behavior).
			if(string.IsNullOrEmpty(orthoPlacementProcsPref)) {
				command="SELECT procedurecode.CodeNum FROM procedurecode WHERE procedurecode.ProcCode LIKE 'D8%'";
				listOrthoPlacementProcCodeNums=Db.GetListLong(command);
			}
			if(listOrthoPlacementProcCodeNums.Count > 0) {
				//Get all PatNums for patients that have at least one completed ortho placement procedure.
				command="SELECT DISTINCT procedurelog.PatNum "
					+"FROM procedurelog "
					+"WHERE ProcStatus=2 "/*2=Complete*/
					+"AND procedurelog.CodeNum IN ("+string.Join(",",listOrthoPlacementProcCodeNums)+") ";
				List<long> listPatNumsWithOrthoTreatments=Db.GetListLong(command);
				if(listPatNumsWithOrthoTreatments.Count > 0) {
					command="UPDATE patientnote SET patientnote.OrthoMonthsTreatOverride="+orthoDefaultMonths+" "
						+"WHERE patientnote.PatNum IN ("+string.Join(",",listPatNumsWithOrthoTreatments)+") "
						+"AND patientnote.OrthoMonthsTreatOverride=-1 ";
					Db.NonQ(command);
				}
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.1.21");//No translation in convert script.
		}

		private static void To18_1_22() {
			string command;
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.1.22 - add index ClinicNumSchedType");
			//try {
				if(DataConnection.DBtype==DatabaseType.MySql) {
					//Add a composite index of the ClinicNum and SchedType columns and call the new index ClinicNumSchedType.
					if(!LargeTableHelper.IndexExists("schedule","ClinicNum,SchedType")) {
						LargeTableHelper.AlterLargeTable("schedule","ScheduleNum",null
							,new List<Tuple<string,string>> { Tuple.Create("ClinicNum,SchedType","ClinicNumSchedType") });
					}
					//Drop the redundant index once composite index is successfully added
					ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.1.22 - drop index ClinicNum");
					if(LargeTableHelper.IndexExists("schedule","ClinicNum")) {
						command="ALTER TABLE schedule DROP INDEX ClinicNum";
						Db.NonQ(command);
					}
				}
				else {//oracle
					//Add a composite index of the ClinicNum and SchedType columns and call the new index schedule_ClinicNumSchedType.
					command=@"CREATE INDEX schedule_ClinicNumSchedType ON schedule (ClinicNum,SchedType)";
					Db.NonQ(command);
					//Drop the redundant index once composite index is successfully added
					ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.1.22 - drop index ClinicNum");
					command="DROP INDEX schedule_ClinicNum";
					Db.NonQ(command);
				}
			//}
			//catch(Exception ex) {
			//	ex.DoNothing();//just an index, do nothing
			//}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.1.22");//No translation in convert script.
		}

		private static void To18_1_24() {
			string command;
			ODEvent.Fire(ODEventType.ConvertDatabases,
				"Upgrading database to version: 18.1.24 - Moving AuditTrailDeleteDuplicateApptCreate to Old Tab");//No translation in convert script.
			//check databasemaintenance for AuditTrailDeleteDuplicateApptCreate, insert if not there and set IsOld to True or update to set IsOld to true
			command="SELECT MethodName FROM databasemaintenance WHERE MethodName='AuditTrailDeleteDuplicateApptCreate'";
			string methodName=Db.GetScalar(command);
			if(methodName=="") {//didn't find row in table, insert
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO databasemaintenance (MethodName, IsOld) VALUES ('AuditTrailDeleteDuplicateApptCreate',1)";//true by default
				}
				else {//oracle
					command="INSERT INTO databasemaintenance (DatabaseMaintenanceNum, MethodName, IsOld) VALUES ((SELECT COALESCE(MAX(DatabaseMaintenanceNum),0)+1 DatabaseMaintenanceNum FROM databasemaintenance),'AuditTrailDeleteDuplicateApptCreate',1)";
				}
			}
			else {//found row, update IsOld
				command="UPDATE databasemaintenance SET IsOld = 1 WHERE MethodName = 'AuditTrailDeleteDuplicateApptCreate'";//true by default
			}
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.1.24");//No translation in convert script
		}

		private static void To18_1_25() {
			ODEvent.Fire(ODEventType.ConvertDatabases,
				"Upgrading database to version: 18.1.25");//No translation in convert script.
			string command=@"ALTER TABLE autonotecontrol 
					MODIFY ControlLabel VARCHAR(255)";
			Db.NonQ(command);
		}

		private static void To18_1_26() {
			ODEvent.Fire(ODEventType.ConvertDatabases,
				"Upgrading database to version: 18.1.26");//No translation in convert script.
			//We are running this section of code for HQ only
			//This is very uncommon and normally manual queries should be run instead of doing a convert script.
			string command="SELECT ValueString FROM preference WHERE PrefName='DockPhonePanelShow'";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count > 0 && PIn.Bool(table.Rows[0][0].ToString())) {
				//Convert the following int columns to bigint so that they can handle TimeSpans stored as Ticks.
				command="ALTER TABLE job CHANGE COLUMN HoursEstimate TimeEstimate bigint(20) NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE job CHANGE COLUMN HoursActual TimeActual bigint(20) NOT NULL";//Convert this even though it is deprecated
				Db.NonQ(command);
				//The following columns were storing hours as ints and need to be converted to TimeSpan.Ticks (1/10 MS per tick).
				command="UPDATE job SET job.TimeEstimate=ROUND(job.TimeEstimate*"+TimeSpan.TicksPerHour+",0)";
				Db.NonQ(command);
				command="UPDATE job SET job.TimeActual=ROUND(job.TimeActual*"+TimeSpan.TicksPerHour+",0)";
				Db.NonQ(command);
				DataTable tableReviews;
				//Pull all of the job reviews into memory so that we can convert the old string column into a TimeSpanLong column (using ticks).
				command="SELECT * FROM jobreview";
				tableReviews=Db.GetTable(command);
				command="ALTER TABLE jobreview CHANGE COLUMN Hours TimeReview bigint(20) NOT NULL";
				Db.NonQ(command);
				foreach(DataRow row in tableReviews.Rows) {
					long reviewNum=PIn.Long(row["JobReviewNum"].ToString());
					double hours=PIn.Double(row["Hours"].ToString());
					command="UPDATE jobreview SET jobreview.TimeReview=ROUND("+hours*TimeSpan.TicksPerHour+",0) WHERE jobreview.JobReviewNum="+reviewNum;
					Db.NonQ(command);
				}
				//Insert all the new TimeLog reviews for the deprecated TimeActual column
				command="INSERT INTO jobreview (JobNum,ReviewerNum,DateTStamp,Description,ReviewStatus,TimeReview) "
					+"SELECT job.JobNum"
						+",IF(job.UserNumEngineer != 0,job.UserNumEngineer,IF(job.UserNumExpert != 0,job.UserNumExpert,job.UserNumConcept))"
						+",NOW()"
						+",'Auto-Inserted Via Convert Script'"
						+",'TimeLog'"
						+",job.TimeActual "
					+"FROM job WHERE job.TimeActual != 0 ";
				Db.NonQ(command);
			}
		}

		private static void To18_1_29() {
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.1.29");//No translation in convert script.
			//check databasemaintenance for CanadaCarriersCdaMissingInfo, insert if not there and set IsOld to True or update to set IsOld to true
			string command="SELECT MethodName FROM databasemaintenance WHERE MethodName='CanadaCarriersCdaMissingInfo'";
			string methodName=Db.GetScalar(command);
			if(methodName=="") {//didn't find row in table, insert
				command="INSERT INTO databasemaintenance (MethodName,IsOld) VALUES ('CanadaCarriersCdaMissingInfo',1)";
			}
			else {//found row, update IsOld
				command="UPDATE databasemaintenance SET IsOld = 1 WHERE MethodName = 'CanadaCarriersCdaMissingInfo'";
			}
			Db.NonQ(command);
		}

		private static void To18_1_32() {
			string command="SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = 'opendentalarchive'";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==1) {
				//Check the preference table to double check that this database has made a valid archive before.
				command="SELECT ValueString FROM preference WHERE PrefName = 'ArchiveDate'";
				DateTime dateArchive=PIn.Date(Db.GetScalar(command));
				if(dateArchive.Year > 1880) {
					//This is most likely the database that had its data archived into the opendentalarchive database.
					//Rename the opendentalarchive database to '[currentdatabasename]_archive' so that multiple databases on the same server can archive.
					LargeTableHelper.RenameDatabase("opendentalarchive",(Db.GetScalar("SELECT DATABASE()")+"_archive"));
				}
			}
		}

		private static void To18_1_33() {
			//Per Brian: "I can’t think of any valid reason that anyone would prefer it to be blank given the recent changes.
			//I think more likely, everyone who set it to blank did so to get around issues that are no longer problems.
			//I think leaving some people blank will result in support calls where we have to go set a date.......
			//update anyone with blank to 2..."
			//A 1 in the database shows as a 2 in the UI.  A -1 in the database represents disabled (blank in UI).
			string command="UPDATE preference SET ValueString='1' WHERE PrefName='ClaimPaymentNoShowZeroDate' AND ValueString='-1'";
			Db.NonQ(command);
		}

		private static void To18_1_35() {
			string command;
			if(!LargeTableHelper.IndexExists("smstomobile","ClinicNum,DateTimeSent")) {
				command=@"ALTER TABLE smstomobile ADD INDEX ClinicDTSent (ClinicNum,DateTimeSent);";
				Db.NonQ(command);
				if(LargeTableHelper.IndexExists("smstomobile","ClinicNum")) {
					command=@"ALTER TABLE smstomobile DROP INDEX ClinicNum;";
					Db.NonQ(command);
				}
			}
			//Check if there is a user group with the ProcCodeEdit permission.
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=157";//ProcCodeEdit
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				//There should always be at least one group with this permission so we will assume that the v17.4 script was missed.
				//Add permission to groups with existing Setup permission------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=8";//Setup
				table=Db.GetTable(command);
				long groupNum;
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",157)";//ProcCodeEdit
					Db.NonQ(command);
				}
			}
		}

		private static void To18_1_39() {
			string command;
			//Insert birthdate format program property into XVWeb bridge
			command="SELECT ProgramNum FROM program WHERE ProgName='XVWeb'"; 
			long programNum=Db.GetLong(command); 
			if(programNum!=0) {
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
					 +") VALUES(" 
					 +"'"+POut.Long(programNum)+"', " 
					 +"'Birthdate format (default MM/dd/yyyy)', " 
					 +"'MM/dd/yyyy')"; 
				Db.NonQ(command);
			}
		}

		private static void To18_1_45() {
			string command;
			command="ALTER TABLE etrans ADD INDEX EtransTypeAndDate (Etype,DateTimeTrans)";
			Db.NonQ(command);
		}

		private static void To18_1_56() {
			string command;
			DataTable table;
			//There was a bug introduced in v18.1.55 that did not bubble up MySQL exceptions correctly.
			//Any user that executed the convert script may have missed the following blocks of code because they were written in such a way that they
			//were expecting a MySQL exception to be thrown based on the query that was written as an indicator to do some logic.
			//These blocks of code have been rewritten to instead utilize the tools that are given to us to safely check for such things
			//instead of expecting an exception to be thrown.  We should not using exceptions as "if / else" substitutes.
			#region To17_3_6 task.DateTimeOriginal Query
			command="SHOW COLUMNS FROM task WHERE FIELD='DateTimeOriginal'";
			table=Db.GetTable(command);
			if(table.Rows.Count==0) {//DateTimeOriginal column was not correctly added.
				command="ALTER TABLE task ADD DateTimeOriginal datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
			}
			#endregion
			#region To17_3_6 taskhist.DateTimeOriginal Query
			command="SHOW COLUMNS FROM taskhist WHERE FIELD='DateTimeOriginal'";
			table=Db.GetTable(command);
			if(table.Rows.Count==0) {//DateTimeOriginal column was not correctly added.
				command="ALTER TABLE taskhist ADD DateTimeOriginal datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
			}
			#endregion
			#region To17_4_57 sheet.DocNum Query 
			command="SHOW COLUMNS FROM sheet WHERE FIELD='DocNum'";
			table=Db.GetTable(command);
			if(table.Rows.Count==0) {//DocNum column was not correctly added.
				command="ALTER TABLE sheet ADD DocNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE sheet ADD INDEX (DocNum)";
				Db.NonQ(command);
			}
			#endregion
		}

		private static void To18_1_62() {
			FixB11013();
		}

		private static void To18_2_0() {
			string command;
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.2.1 - Adding wikipage PageContentPlanText");//No translation in convert script.
			if(DataConnection.DBtype==DatabaseType.MySql) { 
				command="ALTER TABLE wikipage ADD PageContentPlainText MEDIUMTEXT NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE wikipage ADD PageContentPlainText clob";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.2.1 - Converting WikiPage Content");//No translation in convert script.
			command="SELECT PageTitle,WikiPageNum,PageContent FROM wikipage WHERE IsDraft=0 ORDER BY PageTitle";
			DataTable tableWikiListAll=Db.GetTable(command);
			char currentLetter=' ';
			//Go through each of the wikipages and removing HTML markup from the PageContentPlainText column.
			foreach(DataRow row in tableWikiListAll.Rows) {
				string titleCur=PIn.String(row["PageTitle"].ToString()).ToUpper();
				if(titleCur.Length>0 && titleCur.First()!=currentLetter) {
					currentLetter=titleCur.First();
					ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.2.1 - Converting WikiPages starting with the letter '"+currentLetter+"'");//No translation in convert script.
				}
				string wikiPageContent=PIn.String(row["PageContent"].ToString());
				long wikiPageNum=PIn.Long(row["WikiPageNum"].ToString());
				StringBuilder strb=new StringBuilder(wikiPageContent.ToString());
				//The regex pattern below will match anything enclosed within "<" and ">". However for our wiki pages, we use "&" as an escape character
				//for "<" and ">", so we do not want to match "&<" or "&>". We know that this will not perfectly parse all HTML tags, but it is good enough
				//to use for searching.
				StringTools.RegReplace(strb,"(?<!&)<.+?(?<!&)>","");
				//We also want to remove links to other wiki pages.
				StringTools.RegReplace(strb,@"\[\[.+?\]\]","");
				command="UPDATE wikipage SET PageContentPlainText='"+POut.String(strb.ToString())+"' WHERE WikiPageNum="+POut.Long(wikiPageNum);
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.2.1 - Removing Duplicate Custom Dictionary Entries");//No translation in convert script.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				//cleans duplicate spell checks from database
				command=@"DELETE dc1 FROM dictcustom dc1 
					INNER JOIN dictcustom dc2 ON dc1.WordText = dc2.WordText
					WHERE dc1.DictCustomNum > dc2.DictCustomNum";
				Db.NonQ(command);
			}
			else {//oracle
				//cleans duplicate spell checks from database
				command=@"DELETE FROM dictcustom 
					WHERE DictCustomNum IN 
					(SELECT * FROM(SELECT dc1.DictCustomNum
					FROM dictcustom dc1 INNER JOIN dictcustom dc2 ON dc1.WordText = dc2.WordText 
					WHERE dc1.DictCustomNum > dc2.DictCustomNum) AS dc)";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.2.1 - Adding Usernum to SupplyOrder");//No translation in convert script.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE supplyorder ADD UserNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE supplyorder ADD INDEX (UserNum)";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE supplyorder ADD UserNum number(20)";
				Db.NonQ(command);
				command="UPDATE supplyorder SET UserNum = 0 WHERE UserNum IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE supplyorder MODIFY UserNum NOT NULL";
				Db.NonQ(command);
				command=@"CREATE INDEX supplyorder_UserNum ON supplyorder (UserNum)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE supplyorder ADD ShippingCharge double NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE supplyorder ADD ShippingCharge number(38,8)";
				Db.NonQ(command);
				command="UPDATE supplyorder SET ShippingCharge = 0 WHERE ShippingCharge IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE supplyorder MODIFY ShippingCharge NOT NULL";
				Db.NonQ(command);
			}
			ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 18.2.1");//No translation in convert script.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('RecurringChargesAutomatedEnabled','0')";//disabled by default
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'RecurringChargesAutomatedEnabled','0')";//disabled by default
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('RecurringChargesAutomatedTime','2018-01-01 09:00:00')";//9 AM
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'RecurringChargesAutomatedTime','2018-01-01 09:00:00')";//9 AM
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('RecurringChargesAutomatedLastDateTime','0001-01-01 00:00:00')";
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'RecurringChargesAutomatedLastDateTime','0001-01-01 00:00:00')";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS recurringcharge";
				Db.NonQ(command);
				command=@"CREATE TABLE recurringcharge (
						RecurringChargeNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						ClinicNum bigint NOT NULL,
						DateTimeCharge datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						ChargeStatus tinyint NOT NULL,
						FamBal double NOT NULL,
						PayPlanDue double NOT NULL,
						TotalDue double NOT NULL,
						RepeatAmt double NOT NULL,
						ChargeAmt double NOT NULL,
						UserNum bigint NOT NULL,
						PayNum bigint NOT NULL,
						CreditCardNum bigint NOT NULL,
						ErrorMsg text NOT NULL,
						INDEX(PatNum),
						INDEX(ClinicNum),
						INDEX(UserNum),
						INDEX(PayNum),
						INDEX(CreditCardNum)
						) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE recurringcharge'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE recurringcharge (
						RecurringChargeNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						DateTimeCharge date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						ChargeStatus number(3) NOT NULL,
						FamBal number(38,8) NOT NULL,
						PayPlanDue number(38,8) NOT NULL,
						TotalDue number(38,8) NOT NULL,
						RepeatAmt number(38,8) NOT NULL,
						ChargeAmt number(38,8) NOT NULL,
						UserNum number(20) NOT NULL,
						PayNum number(20) NOT NULL,
						CreditCardNum number(20) NOT NULL,
						ErrorMsg clob,
						CONSTRAINT recurringcharge_RecurringCharg PRIMARY KEY (RecurringChargeNum)
						)";
				Db.NonQ(command);
				command=@"CREATE INDEX recurringcharge_PatNum ON recurringcharge (PatNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX recurringcharge_ClinicNum ON recurringcharge (ClinicNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX recurringcharge_UserNum ON recurringcharge (UserNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX recurringcharge_PayNum ON recurringcharge (PayNum)";
				Db.NonQ(command);
				command=@"CREATE INDEX recurringcharge_CreditCardNum ON recurringcharge (CreditCardNum)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE timecardrule ADD MinClockInTime time NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE timecardrule ADD MinClockInTime date";
				Db.NonQ(command);
				command="UPDATE timecardrule SET MinClockInTime = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE MinClockInTime IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE timecardrule MODIFY MinClockInTime NOT NULL";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('AutoCommNumClinicsParallel','0')";//Default to use the number of cores in the machine
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'AutoCommNumClinicsParallel','0')";//Default to use the number of cores in the machine
				Db.NonQ(command);
			}
			//Add the new payconnect PreventSavingNewCC property---------------------------------------------------
			command="SELECT ProgramNum FROM program WHERE ProgName='PayConnect'";
			long progNum=PIn.Long(Db.GetScalar(command));
			command="SELECT ClinicNum FROM clinic";
			List<long> listClinicNums=Db.GetListLong(command);
			listClinicNums.Add(0);//Include a property for headquarters.
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(long cNum in listClinicNums) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
						+"VALUES ("+POut.Long(progNum)+",'PayConnectPreventSavingNewCC','0','',"+POut.Long(cNum)+")";
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(long cNum in listClinicNums) {
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
						+"VALUES ((SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
						+POut.Long(progNum)+",'PayConnectPreventSavingNewCC','0','',"+POut.Long(cNum)+")";
					Db.NonQ(command);
				}
			}
			//Add the new xcharge PreventSavingNewCC property------------------------------------------------------
			command="SELECT ProgramNum FROM program WHERE ProgName='Xcharge'";
			progNum=PIn.Long(Db.GetScalar(command));
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(long cNum in listClinicNums) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
						+"VALUES ("+POut.Long(progNum)+",'XChargePreventSavingNewCC','0','',"+POut.Long(cNum)+")";
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(long cNum in listClinicNums) {
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
						+"VALUES ((SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
						+POut.Long(progNum)+",'XChargePreventSavingNewCC','0','',"+POut.Long(cNum)+")";
					Db.NonQ(command);
				}
			}
			//Add the new PaySimple PreventSavingNewCC property------------------------------------------------------
			command="SELECT ProgramNum FROM program WHERE ProgName='PaySimple'";
			progNum=PIn.Long(Db.GetScalar(command));
			if(DataConnection.DBtype==DatabaseType.MySql) {
				foreach(long cNum in listClinicNums) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
						+"VALUES ("+POut.Long(progNum)+",'PaySimplePreventSavingNewCC','0','',"+POut.Long(cNum)+")";
					Db.NonQ(command);
				}
			}
			else {//oracle
				foreach(long cNum in listClinicNums) {
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
						+"VALUES ((SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
						+POut.Long(progNum)+",'PaySimplePreventSavingNewCC','0','',"+POut.Long(cNum)+")";
					Db.NonQ(command);
				}
			}
			command=@"UPDATE preference SET ValueString='https://opendentalsoft.com/WebHostSynch/SheetsSynch.asmx'
				WHERE PrefName='WebHostSynchServerURL' 
				AND ValueString='https://opendentalsoft.com/WebHostSynch/Sheets.asmx'";
			Db.NonQ(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ClockEventAllowBreak','1')";//Default to true.
				Db.NonQ(command);
			}
			else {//oracle
				command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ClockEventAllowBreak','1')";//Default to true.
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="DROP TABLE IF EXISTS commoptout";
				Db.NonQ(command);
				command=@"CREATE TABLE commoptout (
						CommOptOutNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						CommType tinyint NOT NULL,
						CommMode tinyint NOT NULL,
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			else {//oracle
				command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE commoptout'; EXCEPTION WHEN OTHERS THEN NULL; END;";
				Db.NonQ(command);
				command=@"CREATE TABLE commoptout (
						CommOptOutNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						CommType number(3) NOT NULL,
						CommMode number(3) NOT NULL,
						CONSTRAINT commoptout_CommOptOutNum PRIMARY KEY (CommOptOutNum)
						)";
				Db.NonQ(command);
				command=@"CREATE INDEX commoptout_PatNum ON commoptout (PatNum)";
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE smstomobile ADD MsgDiscountUSD float NOT NULL";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE smstomobile ADD MsgDiscountUSD number(38,8)";
				Db.NonQ(command);
				command="UPDATE smstomobile SET MsgDiscountUSD = 0 WHERE MsgDiscountUSD IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE smstomobile MODIFY MsgDiscountUSD NOT NULL";
				Db.NonQ(command);
			}
			command=@"UPDATE displayreport SET Description = 'Procedure Codes - Fee Schedules' 
					WHERE InternalName = 'ODProcedureCodes' AND Description = 'Procedure Codes'";
			Db.NonQ(command);
			command="ALTER TABLE insplan ADD HasPpoSubstWriteoffs tinyint NOT NULL";
			Db.NonQ(command);
			command="UPDATE insplan SET HasPpoSubstWriteoffs=1";//Retain past behavior
			Db.NonQ(command);
			#region F4487 - HTML Formatted Email
			command="ALTER TABLE emailmessage ADD IsHtml tinyint NOT NULL";
			Db.NonQ(command);
			//This is the same template that was used for the Wiki master page in ConvertDatabases2.
			string emailMasterText=POut.String(@"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
<style type=""text/css"">
<!--
*{
	border: 0px;
	margin: 0px;
}
body{
	margin:		3px;
	font-size:	9pt;
	font-family:	arial,sans-serif;
}
p{
	margin-top: 0px;
	margin-bottom: 0px;
}
h1, h1 a{
	font-size:20pt;
	color:#005677;
}
h2, h2 a {
	font-size:16pt;
	color:#005677;
}
h3, h3 a {
	font-size:12pt;
	color:#005677;
}
ul{
	list-style-position: inside;/*This puts the bullets inside the div rect instead of outside*/
}
ol{
	list-style-position: inside;
}
ul .ListItemContent{
	position: relative; left: -5px;/*Tightens up the spacing between bullets and text*/
}
ol .ListItemContent{
	position: relative; left: -1px;/*Tightens up the spacing between numbers and text*/
}
a{
	color:rgb(68,81,199);/*same blue color as wikipedia*/
	text-decoration:none;
}
a:hover{
	text-decoration:underline;
}
table, th, td, tr {
	border-collapse:collapse;
	border:1px solid #999999;
	padding:2px;
}
.PageNotExists, a.PageNotExists {
	border-bottom:1px dashed #000000;
}
a.PageNotExists:hover {
	border-bottom:1px solid #000000;
	text-decoration:none;
}
.keywords, a.keywords:hover {
	color:#000000;
	background-color:#eeeeee;
}
-->
</style>
</head>
@@@body@@@
</html>");
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailMasterTemplate','"+emailMasterText+"')";
			Db.NonQ(command);
			#endregion F4487 - HTML Formatted Email
			//Add new sheet.DateTSheetEdited column
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="ALTER TABLE sheet ADD DateTSheetEdited datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
			}
			else {//oracle
				command="ALTER TABLE sheet ADD DateTSheetEdited date";
				Db.NonQ(command);
				command="UPDATE sheet SET DateTSheetEdited = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateTSheetEdited IS NULL";
				Db.NonQ(command);
				command="ALTER TABLE sheet MODIFY DateTSheetEdited NOT NULL";
				Db.NonQ(command);
			}
			command="ALTER TABLE provider ADD DateTerm date NOT NULL DEFAULT '0001-01-01'";
			Db.NonQ(command);
			command="SELECT MethodName FROM databasemaintenance WHERE MethodName='ProcedurelogBaseUnitsZero'";
			string methodName=Db.GetScalar(command);
			if(methodName=="") {//didn't find row in table, insert
				command="INSERT INTO databasemaintenance (MethodName,IsOld) VALUES ('ProcedurelogBaseUnitsZero',1)";
			}
			else {//found row, update IsOld
				command="UPDATE databasemaintenance SET IsOld = 1 WHERE MethodName = 'ProcedurelogBaseUnitsZero'";
			}
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('ClaimZeroDollarProcBehavior','0')";//Default to ClaimZeroDollarProcBehavior.Allow
			Db.NonQ(command);
			//Add additional benefit limitation codes--------------------------------------------------------------
			command="INSERT INTO preference (PrefName,ValueString) VALUES('InsBenCancerScreeningCodes','D0431')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('InsBenProphyCodes','D1110,D1120')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('InsBenFlourideCodes','D1206,D1208')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('InsBenSealantCodes','D1351')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('InsBenCrownCodes','D2740,D2750,D2751,D2752,D2780,D2781,D2782,D2783,D2790,D2791,"
				+"D2792,D2794')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('InsBenSRPCodes','D4341,D4342')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('InsBenFullDebridementCodes','D4355')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('InsBenPerioMaintCodes','D4910')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('InsBenDenturesCodes','D5110,D5120,D5130,D5140,D5211,D5212,D5213,D5214,D5221,"
				+"D5222,D5223,D5224,D5225,D5226')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('InsBenImplantCodes','D6010')";
			Db.NonQ(command);
			//Default to true if US only.
			command="INSERT INTO preference (PrefName,ValueString) VALUES('EmailDisclaimerIsOn','"+(CultureInfo.CurrentCulture.Name.EndsWith("US") ? "1" : "0")+"')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('EmailDisclaimerTemplate',"+
				"'This email has been sent to you from:\r\n[PostalAddress].\r\n\r\nHow to unsubscribe:\r\nIf you no longer want to receive any email messages from us, simply reply to this email with the word \"unsubscribe\" in the subject line.')";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS dbmlog";
			Db.NonQ(command);
			command=@"CREATE TABLE dbmlog (
					DbmLogNum bigint NOT NULL auto_increment PRIMARY KEY,
					UserNum bigint NOT NULL,
					FKey bigint NOT NULL,
					FKeyType tinyint NOT NULL,
					ActionType tinyint NOT NULL,
					DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					MethodName varchar(255) NOT NULL,
					LogText text NOT NULL,
					INDEX(UserNum),
					INDEX FKeyAndType (FKey,FKeyType),
					INDEX(DateTimeEntry),
					INDEX(MethodName)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('TitleBarShowSpecialty','0')";//Default to false.
				Db.NonQ(command);
			}
			if(DataConnection.DBtype==DatabaseType.MySql) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('ApptSchedEnforceSpecialty','0')";//Default to 0-Don't Enforce.
				Db.NonQ(command);
			}
			command="ALTER TABLE clearinghouse ADD IsAttachmentSendAllowed TINYINT NOT NULL";
			Db.NonQ(command);
		}//End of 18_2_1() method

		private static void To18_2_4() {
			string command;
			//Insert birthdate format program property into XVWeb bridge
			command="SELECT ProgramNum FROM program WHERE ProgName='XVWeb'"; 
			long programNum=Db.GetLong(command); 
			command="SELECT ProgramPropertyNum FROM programproperty WHERE PropertyDesc='Birthdate format (default MM/dd/yyyy)' AND ProgramNum='"+POut.Long(programNum)+"'";
			long propertyNum=Db.GetLong(command); 
			if(propertyNum==0) {//ProgramProperty does not exist
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
					 +") VALUES(" 
					 +"'"+POut.Long(programNum)+"', " 
					 +"'Birthdate format (default MM/dd/yyyy)', " 
					 +"'MM/dd/yyyy')"; 
				Db.NonQ(command);
			}
		}
		
		private static void To18_2_9() {
			string command;
			command="ALTER TABLE rxpat MODIFY DaysOfSupply DOUBLE";
			Db.NonQ(command);
		}

		private static void To18_2_11() {
			string command;
			if(!LargeTableHelper.IndexExists("etrans","Etype,DateTimeTrans")) {
				command="ALTER TABLE etrans ADD INDEX EtransTypeAndDate (Etype,DateTimeTrans)";
				Db.NonQ(command);
			}
		}

		private static void To18_2_15() {
			string command;
			//This effectively makes everyone's future date for the unscheduled list 'blank' per Nathan's comment on Job 8917
			command="UPDATE preference SET ValueString = -1 WHERE PrefName = 'UnschedDaysFuture'";
			Db.NonQ(command);
		}

		private static void To18_2_20() {
			LargeTableHelper.AlterLargeTable("securitylog","SecurityLogNum",null,
				new List<Tuple<string,string>> { Tuple.Create("LogDateTime","") });//no need to send index name. only adds index if not exists
		}

		private static void To18_2_23() {
			string command;
			DataTable table;
			command="SELECT COUNT(*) FROM feesched WHERE FeeSchedType=3";//3=FixedBenefit FeeSchedType
			string numFixedBenefitFeeScheds=Db.GetCount(command);
			if(numFixedBenefitFeeScheds!="0") {
				//Default to true if FixedBenefit fees are already in use. Maintain current behavior for offices already using this feature.
				command="INSERT INTO preference (PrefName,ValueString) VALUES('FixedBenefitBlankLikeZero','1')";
			}
			else {
				//Default to false if FixedBenefit fees are not in use.
				command="INSERT INTO preference (PrefName,ValueString) VALUES('FixedBenefitBlankLikeZero','0')";
			}
			Db.NonQ(command);
			//There was a bug introduced in v18.1.55 that did not bubble up MySQL exceptions correctly.
			//There was a very small window for users to have updated to v18.1.55 and then updated AGAIN to an 18.2 version and they still need this code.
			//Any user that executed the convert script may have missed the following blocks of code because they were written in such a way that they
			//were expecting a MySQL exception to be thrown based on the query that was written as an indicator to do some logic.
			//These blocks of code have been rewritten to instead utilize the tools that are given to us to safely check for such things
			//instead of expecting an exception to be thrown.  We should not using exceptions as "if / else" substitutes.
			#region To17_3_6 task.DateTimeOriginal Query
			command="SHOW COLUMNS FROM task WHERE FIELD='DateTimeOriginal'";
			table=Db.GetTable(command);
			if(table.Rows.Count==0) {//DateTimeOriginal column was not correctly added.
				command="ALTER TABLE task ADD DateTimeOriginal datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
			}
			#endregion
			#region To17_3_6 taskhist.DateTimeOriginal Query
			command="SHOW COLUMNS FROM taskhist WHERE FIELD='DateTimeOriginal'";
			table=Db.GetTable(command);
			if(table.Rows.Count==0) {//DateTimeOriginal column was not correctly added.
				command="ALTER TABLE taskhist ADD DateTimeOriginal datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
				Db.NonQ(command);
			}
			#endregion
			#region To17_4_57 sheet.DocNum Query 
			command="SHOW COLUMNS FROM sheet WHERE FIELD='DocNum'";
			table=Db.GetTable(command);
			if(table.Rows.Count==0) {//DocNum column was not correctly added.
				command="ALTER TABLE sheet ADD DocNum bigint NOT NULL";
				Db.NonQ(command);
				command="ALTER TABLE sheet ADD INDEX (DocNum)";
				Db.NonQ(command);
			}
			#endregion
		}

		private static void To18_2_29() {
			string command;
			//Update current PandaPerio description to Panda Perio (simple)
			command="UPDATE program SET ProgDesc='Panda Perio (simple) from www.pandaperio.com' WHERE ProgName='PandaPerio'";
			Db.NonQ(command);
			//Insert PandaPeriodAdvanced bridge----------------------------------------------------------------- 
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				 +") VALUES("
				 +"'PandaPeriodAdvanced', "
				 +"'Panda Perio (advanced) from www.pandaperio.com', "
				 +"'0', "
				 +"'"+POut.String(@"C:\Program Files (x86)\Panda Perio\Panda.exe")+"', "
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
				 +"'PandaPeriodAdvanced')";
			Db.NonQ(command);
			//end PandaPeriodAdvanced bridge 
		}

		private static void To18_2_37() {
			string command;
			//Some of the default Web Sched Notify templates had '[ApptDate]' where they should have had '[ApptTime]'.
			foreach(string prefName in new List<string> { "WebSchedVerifyRecallText","WebSchedVerifyNewPatText","WebSchedVerifyASAPText" }) {
				command="SELECT ValueString FROM preference WHERE PrefName='"+POut.String(prefName)+"'";
				string curValue=Db.GetScalar(command);
				if(curValue=="Appointment scheduled for [FName] on [ApptDate] [ApptDate] at [OfficeName], [OfficeAddress]") {
					string newValue="Appointment scheduled for [FName] on [ApptDate] [ApptTime] at [OfficeName], [OfficeAddress]";
					command="UPDATE preference SET ValueString='"+POut.String(newValue)+"' WHERE PrefName='"+POut.String(prefName)+"'";
					Db.NonQ(command);
				}
			}
		}

		private static void To18_2_38() {
			string command;
			command="SELECT ProgramNum,Enabled FROM program WHERE ProgName='eRx'";
			DataTable table=Db.GetTable(command);
			if(table.Rows.Count>0 && PIn.Bool(table.Rows[0]["Enabled"].ToString())) {
				long programErx=PIn.Long(table.Rows[0]["ProgramNum"].ToString());
				command=$"SELECT PropertyValue FROM programproperty WHERE PropertyDesc='eRx Option' AND ProgramNum={programErx}";
				//0 is the enum value of Legacy, 1 is the enum value of DoseSpot
				bool isNewCrop=PIn.Int(Db.GetScalar(command))==0;
				if(isNewCrop) {//Only update rows if the office has eRx enabled and is using NewCrop.
					command="UPDATE provider SET IsErxEnabled=2 WHERE IsErxEnabled=1";
					Db.NonQ(command);
				}
			}
			//check databasemaintenance for TransactionsWithFutureDates, insert if not there and set IsOld to True or update to set IsOld to true
			command="SELECT MethodName FROM databasemaintenance WHERE MethodName='TransactionsWithFutureDates'";
			string methodName=Db.GetScalar(command);
			if(methodName=="") {//didn't find row in table, insert
				command="INSERT INTO databasemaintenance (MethodName,IsOld) VALUES ('TransactionsWithFutureDates',1)";
			}
			else {//found row, update IsOld
				command="UPDATE databasemaintenance SET IsOld = 1 WHERE MethodName = 'TransactionsWithFutureDates'";
			}
			Db.NonQ(command);
		}

		private static void To18_2_47() {
			string command;
			FixB11013();
			//Moving codes to the Obsolete category that were deleted in CDT 2019.
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
					command="INSERT INTO definition (Category,ItemName,ItemOrder) "
								+"VALUES (11"+",'"+POut.String(procCatDescript)+"',"+POut.Int(countCats)+")";//11 is DefCat.ProcCodeCats
					defNum=Db.NonQ(command,true);
				}
				else { //The procedure code category already exists, get the existing defnum
					defNum=PIn.Long(dtDef.Rows[0]["DefNum"].ToString());
				}
				string[] cdtCodesDeleted=new string[] {
					"D1515",
					"D1525",
					"D5281",
					"D9940"
				};
				//Change the procedure codes' category to Obsolete.
				command="UPDATE procedurecode SET ProcCat="+POut.Long(defNum)
					+" WHERE ProcCode IN('"+string.Join("','",cdtCodesDeleted.Select(x => POut.String(x)))+"') ";
				Db.NonQ(command);
			}//end United States CDT codes update
		}
		
		private static void To18_2_50() {
			string command;
			command=@"UPDATE preference SET ValueString='https://www.patientviewer.com:49997/OpenDentalWebServiceHQ/WebServiceMainHQ.asmx' 
				WHERE PrefName='WebServiceHQServerURL' AND ValueString='http://www.patientviewer.com:49999/OpenDentalWebServiceHQ/WebServiceMainHQ.asmx'";
			Db.NonQ(command);
		}

	}
}