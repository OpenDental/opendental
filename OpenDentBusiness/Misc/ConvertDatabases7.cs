using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CodeBase;
using static OpenDentBusiness.LargeTableHelper;//so you don't have to type the class name each time.

namespace OpenDentBusiness {
	public partial class ConvertDatabases {
		private static void To20_5_1() {
			string command;
			DataTable table;
			command="DROP TABLE IF EXISTS imagingdevice";
			Db.NonQ(command);
			command=@"CREATE TABLE imagingdevice (
				ImagingDeviceNum bigint NOT NULL auto_increment PRIMARY KEY,
				Description varchar(255) NOT NULL,
				ComputerName varchar(255) NOT NULL,
				DeviceType tinyint NOT NULL,
				TwainName varchar(255) NOT NULL,
				ItemOrder int NOT NULL,
				ShowTwainUI tinyint NOT NULL
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS eservicelog";
			Db.NonQ(command);
			command=@"CREATE TABLE eservicelog (
				EServiceLogNum bigint NOT NULL auto_increment PRIMARY KEY,
				EServiceCode tinyint NOT NULL,
				EServiceOperation tinyint NOT NULL,
				LogDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				AptNum bigint NOT NULL,
				PatNum bigint NOT NULL,
				PayNum bigint NOT NULL,
				SheetNum bigint NOT NULL,
				INDEX(EServiceCode),
				INDEX(EServiceOperation),
				INDEX(AptNum),
				INDEX(PatNum),
				INDEX(PayNum),
				INDEX(SheetNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="ALTER TABLE mountitemdef ADD RotateOnAcquire int NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mountitem ADD RotateOnAcquire int NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mountitemdef ADD ToothNumbers varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mountitem ADD ToothNumbers varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE wikilistheaderwidth ADD IsHidden tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE procedurecode ADD PaintText varchar(255) NOT NULL";
			Db.NonQ(command);
			command="UPDATE procedurecode SET PaintText='W' WHERE PaintType=15";//ToothPaintingType.Text
			Db.NonQ(command);
			command="SELECT COUNT(*) FROM preference WHERE PrefName='EnterpriseExactMatchPhone'";
			//This preference might have already been added in 20.2.49.
			if(Db.GetScalar(command)=="0") {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseExactMatchPhone','0')";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseExactMatchPhoneNumDigits','10')";
				Db.NonQ(command);
			}
			command="INSERT INTO alertcategory (IsHQCategory,InternalName,Description) VALUES(1,'SupplementalBackups','Supplemental Backups')";
			long alertCatNum=Db.NonQ(command, true);
			command=$@"UPDATE alertcategorylink SET AlertCategoryNum={POut.Long(alertCatNum)} WHERE AlertType=23";//23=SupplementalBackups
			Db.NonQ(command);
			command="SELECT UserNum,ClinicNum FROM alertsub WHERE AlertCategoryNum=1";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				command=$@"INSERT INTO alertsub (UserNum,ClinicNum,AlertCategoryNum) 
					VALUES(
						{POut.Long(PIn.Long(table.Rows[i]["UserNum"].ToString()))},
						{POut.Long(PIn.Long(table.Rows[i]["ClinicNum"].ToString()))},
						{POut.Long(alertCatNum)}
					)";
				Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('SameForFamilyCheckboxesUnchecked','0')";
			Db.NonQ(command);
			command="UPDATE procedurecode SET PaintType=17 WHERE ProcCode IN('D1510','D1516','D1517')";//17=SpaceMaintainer
			Db.NonQ(command);
			command="UPDATE procedurecode SET TreatArea=7 WHERE ProcCode IN('D1516','D1517')";//7=ToothRange for bilateral
			Db.NonQ(command);
			AlterTable("toothinitial","ToothInitialNum",new ColNameAndDef("DrawText","varchar(255) NOT NULL"));
			command="INSERT INTO preference(PrefName,ValueString) VALUES('IncomeTransfersTreatNegativeProductionAsIncome','1')";
			Db.NonQ(command);
			command="ALTER TABLE mount ADD ProvNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE document ADD ProvNum bigint NOT NULL";
			Db.NonQ(command);
			command="SELECT COUNT(*) FROM preference WHERE PrefName='EnterpriseAllowRefreshWhileTyping'";
			//This preference might have already been added in 20.3.41
			if(Db.GetScalar(command)=="0") {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseAllowRefreshWhileTyping','1')";
				Db.NonQ(command);
			}
			command="ALTER TABLE activeinstance ADD ConnectionType tinyint(4) NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('SaveDXCSOAPAsXML','0')";
			Db.NonQ(command);		
			command="ALTER TABLE clinic ADD TimeZone varchar(75) NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES ('WebFormsDownloadAlertFrequency','3600000')";//1 hour in ms
			Db.NonQ(command);
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes'";
			alertCatNum=Db.GetLong(command);
			command=$@"INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES({POut.Long(alertCatNum)},30)";//30=WebformsReady
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('AutoImportFolder','')";
			Db.NonQ(command);
			try {
				command="SELECT * FROM userod";
				table=Db.GetTable(command);
				command="SELECT ValueString FROM preference WHERE PrefName='DomainObjectGuid'";
				string domainObjectGuid=Db.GetScalar(command);
				for(int i=0;i<table.Rows.Count;i++) {
					if(!table.Rows[i]["DomainUser"].ToString().IsNullOrEmpty()) {
						string newDomainUser=domainObjectGuid+"\\"+table.Rows[i]["DomainUser"].ToString();
						command="UPDATE userod ";
						command+="SET DomainUser='"+POut.String(newDomainUser)+"' ";
						command+="WHERE UserNum="+POut.String(table.Rows[i]["UserNum"].ToString());
						Db.NonQ(command);
					}
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ShowIncomeTransferManager','1')";
			Db.NonQ(command);	
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimPayByTotalSplitsAuto','1')";
			Db.NonQ(command);
			command="SELECT valuestring FROM preference WHERE PrefName='SheetsDefaultStatement'";
			long sheetDefNumDefault=PIn.Long(Db.GetScalar(command));
			command=$@"INSERT INTO preference(PrefName,ValueString) VALUES('SheetsDefaultReceipt','{POut.Long(sheetDefNumDefault)}')";
			Db.NonQ(command);	
			command=$@"INSERT INTO preference(PrefName,ValueString) VALUES('SheetsDefaultInvoice','{POut.Long(sheetDefNumDefault)}')";
			Db.NonQ(command);	
			command=$@"INSERT INTO preference(PrefName,ValueString) VALUES('SheetsDefaultLimited','{POut.Long(sheetDefNumDefault)}')";
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				   +"VALUES("+POut.Long(groupNum)+",200)";//200 - FormAdded, Used for logging form creation in EClipboard.
				Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('SalesTaxDefaultProvider','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('SalesTaxDoAutomate','0')";
			Db.NonQ(command);
			command="ALTER TABLE program ADD CustErr varchar(255) NOT NULL";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS cert";
			Db.NonQ(command);
			command=@"CREATE TABLE cert (
				CertNum bigint NOT NULL auto_increment PRIMARY KEY,
				Description varchar(255) NOT NULL,
				WikiPageLink varchar(255) NOT NULL,
				ItemOrder int NOT NULL
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS certemployee";
			Db.NonQ(command);
			command=@"CREATE TABLE certemployee (
				CertEmployeeNum bigint NOT NULL auto_increment PRIMARY KEY,
				DateCompleted date NOT NULL DEFAULT '0001-01-01',
				Note varchar(255) NOT NULL,
				UserNum bigint NOT NULL,
				INDEX(UserNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS certlinkcategory";
			Db.NonQ(command);
			command=@"CREATE TABLE certlinkcategory (
				CertLinkCategoryNum bigint NOT NULL auto_increment PRIMARY KEY,
				CertNum bigint NOT NULL,
				CertCategoryNum bigint NOT NULL,
				INDEX(CertNum),
				INDEX(CertCategoryNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					+"VALUES("+POut.Long(groupNum)+",201)";//201 - Used to restrict access to Image Exporting when necessary.
				Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DefaultImageImportFolder','')";
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				+"VALUES("+POut.Long(groupNum)+",202)";//202 - Used to restrict access to Image Create.
				Db.NonQ(command);
			}
			command="DROP TABLE IF EXISTS statementprod";
			Db.NonQ(command);
			command=@"CREATE TABLE statementprod (
					StatementProdNum bigint NOT NULL auto_increment PRIMARY KEY,
					StatementNum bigint NOT NULL,
					FKey bigint NOT NULL,
					ProdType tinyint NOT NULL,
					LateChargeAdjNum bigint NOT NULL,
					INDEX(StatementNum),
					INDEX(FKey),
					INDEX(ProdType),
					INDEX(LateChargeAdjNum)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=1";//1 is AdjTypes
			int order=PIn.Int(Db.GetCount(command));
			command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemValue) "
				+"VALUES (1,'Late Charge',"+POut.Int(order)+",'+')";//1 is AdjTypes
			long defNum=Db.NonQ(command,true);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeAdjustmentType','"+POut.Long(defNum)+"')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeLastRunDate','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeExcludeAccountNoTil','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeExcludeBalancesLessThan','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargePercent','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeMin','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeMax','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeDateRangeStart','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeDateRangeEnd','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeDefaultBillingTypes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ShowFeatureLateCharges','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('RecurringChargesInactivateDeclinedCards','0')";
			Db.NonQ(command);//Unset by default (same effect as Off for now, for backwards compatibility).
			command="ALTER TABLE creditcard ADD IsRecurringActive tinyint NOT NULL";
			Db.NonQ(command);
			command="UPDATE creditcard SET IsRecurringActive=1";//Default to true for existing credit cards.
			Db.NonQ(command);
		}//End of 20_5_1() method

		private static void To20_5_2() {
			string command;
			DataTable table;
			command="DROP TABLE IF EXISTS discountplansub";
			Db.NonQ(command);
			command=@"CREATE TABLE discountplansub (
				DiscountSubNum bigint NOT NULL auto_increment PRIMARY KEY,
				DiscountPlanNum bigint NOT NULL,
				PatNum bigint NOT NULL,
				DateEffective date NOT NULL DEFAULT '0001-01-01',
				DateTerm date NOT NULL DEFAULT '0001-01-01',
				INDEX(DiscountPlanNum),
				INDEX(PatNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT PatNum,DiscountPlanNum FROM patient WHERE DiscountPlanNum>0";
			table=Db.GetTable(command);
			long patNum;
			for(int i=0;i<table.Rows.Count;i++) {
				patNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
				command=$@"INSERT INTO discountplansub (DiscountPlanNum,PatNum) 
					VALUES(
						{POut.Long(PIn.Long(table.Rows[i]["DiscountPlanNum"].ToString()))},
						{POut.Long(patNum)}
					)";
				Db.NonQ(command);
				//Optionally set the DiscountPlanNum to 0
				command="UPDATE patient ";
				command+="SET DiscountPlanNum=0 ";
				command+="WHERE PatNum="+POut.Long(patNum);
				Db.NonQ(command);
			}
			command="DELETE FROM grouppermission WHERE PermType=89";//Permission already existed, but not enforced. Refreshing this Permission from scratch.
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				+"VALUES("+POut.Long(groupNum)+",89)";//89 - Used to restrict access to Image Edit.
				Db.NonQ(command);
			}
			command="ALTER TABLE programproperty ADD IsMasked tinyint NOT NULL";
			Db.NonQ(command);
			command="SELECT ProgramPropertyNum FROM programproperty WHERE PropertyDesc LIKE '%password%'";
			List<long> listPasswordPropNums=Db.GetListLong(command);
			for(int i=0;i<listPasswordPropNums.Count;i++) {
				command=$"UPDATE programproperty SET IsMasked={POut.Bool(true)} WHERE ProgramPropertyNum={POut.Long(listPasswordPropNums[i])}";
				Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ConfirmPostcardFamMessage','We would like to confirm your appointments. [FamilyApptList].')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ConfirmEmailFamMessage','We would like to confirm your appointments. [FamilyApptList].')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ConfirmTextFamMessage','We would like to confirm your appointments. [FamilyApptList].')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ConfirmGroupByFamily','0')";
			Db.NonQ(command);
			command="ALTER TABLE orthochart ADD ProvNum bigint NOT NULL";
			Db.NonQ(command);
		}//End of 20_5_2() method

		private static void To20_5_3() {
			string command;
			DataTable table;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeExcludeExistingLateCharges','0')";
			Db.NonQ(command);
			command="ALTER TABLE statementprod ADD DocNum bigint NOT NULL,ADD INDEX (DocNum)";
			Db.NonQ(command);
			command="ALTER TABLE emailaddress ADD DownloadInbox tinyint NOT NULL";
			Db.NonQ(command);
		}//End of 20_5_3() method

		private static void To20_5_4() {
			string command;
			DataTable table;
			command=$"SELECT * FROM ebill WHERE ElectPassword!=''";
			table=Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				CDT.Class1.Encrypt(row["ElectPassword"].ToString(),out string encPW);
				long ebillNum=PIn.Long(row["EbillNum"].ToString());
				command=$"UPDATE ebill SET ElectPassword='{POut.String(encPW)}' WHERE EbillNum={POut.Long(ebillNum)}";
				Db.NonQ(command);
			}
			command="DROP TABLE IF EXISTS certemployee";
			Db.NonQ(command);
			command=@"CREATE TABLE certemployee (
				CertEmployeeNum bigint NOT NULL auto_increment PRIMARY KEY,
				CertNum bigint NOT NULL,
				EmployeeNum bigint NOT NULL,
				DateCompleted date NOT NULL DEFAULT '0001-01-01',
				Note varchar(255) NOT NULL,
				UserNum bigint NOT NULL,
				INDEX(UserNum),
				INDEX (CertNum),
				INDEX (EmployeeNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			long groupNum;
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				+"VALUES("+POut.Long(groupNum)+",203)";//203 - Permission to update Employee Certifications.
				Db.NonQ(command);
			}
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				+"VALUES("+POut.Long(groupNum)+",204)";//204 - Permission to set up Certifications.
				Db.NonQ(command);
			}
		}//End of 20_5_4() method

		private static void To20_5_5() {
			string command;
			DataTable table;
			command="ALTER TABLE cert ADD IsHidden tinyint NOT NULL";
			Db.NonQ(command);
			if(!ColumnExists(GetCurrentDatabase(),"IsSentToQuickBooksOnline","deposit")) {
				command="ALTER TABLE deposit ADD IsSentToQuickBooksOnline tinyint NOT NULL";
				Db.NonQ(command);
			}
		}//End of 20_5_5() method

		private static void To20_5_11() {
			string command="ALTER TABLE carecreditwebresponse ADD HasLogged tinyint NOT NULL";
			Db.NonQ(command);
			//Allen says this is what we want to do.
			command=@"SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=15";//SheetTypeEnum.Statement
			long sheetDefNumDefault=PIn.Long(Db.GetScalar(command));
			command=$@"UPDATE preference SET ValueString={POut.Long(sheetDefNumDefault)} WHERE PrefName='SheetsDefaultStatement'";
			Db.NonQ(command);	
			command=$@"UPDATE preference SET ValueString={POut.Long(sheetDefNumDefault)} WHERE PrefName='SheetsDefaultReceipt'";
			Db.NonQ(command);	
			command=$@"UPDATE preference SET ValueString={POut.Long(sheetDefNumDefault)} WHERE PrefName='SheetsDefaultInvoice'";
			Db.NonQ(command);	
			command=$@"UPDATE preference SET ValueString={POut.Long(sheetDefNumDefault)} WHERE PrefName='SheetsDefaultLimited'";
			Db.NonQ(command);
		}

		private static void To20_5_13() {
			string command;
			command="SELECT program.Path FROM program WHERE ProgName='DentalEye'";
			string programPath=Db.GetScalar(command);//only one path
			string newPath=programPath.Replace("DentalEye.exe","CmdLink.exe");//shouldn't launch from this exe anymore
			command="UPDATE program SET Path='"+POut.String(newPath)+"' WHERE ProgName='DentalEye'";
			Db.NonQ(command);
			string note="Please set the file path to open CmdLink.exe in order to send patient data to DentalEye. Ex: C:\\Program Files (x86)\\DentalEye\\CmdLink.exe.";
			command="UPDATE program SET Note='"+POut.String(note)+"' WHERE ProgName='DentalEye'";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('BillingElectIncludeAdjustDescript',1)";
			Db.NonQ(command);
			command="SELECT COUNT(*) FROM preference WHERE preference.PrefName='PdfLaunchWindow';";
			if(PIn.Int(Db.GetCount(command))==0) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('PdfLaunchWindow','0');";//false by default
				Db.NonQ(command);
			}
		}//End of 20_5_13() method

		private static void To20_5_17() {
			string command;
			command="ALTER TABLE cert ADD CertCategoryNum bigint NOT NULL";
			Db.NonQ(command);
			command="UPDATE cert SET CertCategoryNum=(SELECT CertCategoryNum FROM certlinkcategory WHERE cert.CertNum=certlinkcategory.CertNum)";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS certlinkcategory";
			Db.NonQ(command);
		}//End of 20_5_17() method

		private static void To20_5_32() {
			string command;
			//alertcategorylink.AlertCategoryNum for SBs may have been set incorrectly in 20.5.1
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='SupplementalBackups' AND IsHQCategory=1";
			long alertCatNum=Db.GetLong(command);
			command=$@"UPDATE alertcategorylink SET AlertCategoryNum={POut.Long(alertCatNum)} WHERE AlertType=23";//23=SupplementalBackups
			Db.NonQ(command);
			command="UPDATE alertitem SET ClinicNum=-1 WHERE Type=23 AND ClinicNum=0";//23=SupplementalBackups
			Db.NonQ(command);
		}//End of 20_5_32() method

		private static void To20_5_33() {
			string command;
			DoseSpotSelfReportedInvalidNote();
		}

		private static void To20_5_34() {
			string command;
			DataTable table;
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=149";//Currently has Payment Plan Edit permissions
			table=Db.GetTable(command);
			long groupNum;
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				   +"VALUES("+POut.Long(groupNum)+",208)";//208 - Will be given Payment Plan Charge Date Edit permissions
				Db.NonQ(command);
			}
		}//End of 20_5_34() method

		private static void To20_5_38() {
			string command;
			command="UPDATE program SET Path='"+POut.String(@"C:\Program Files\3Shape\Dental Desktop\Plugins\ThreeShape.PracticeManagementIntegration\DentalDesktopCmd.exe")+"' "
				+"WHERE Path='"+POut.String(@"C:\Program Files\3Shape\Dental Desktop\Plugins\ThreeShape.PMSIntegration\DentalDesktopCmd.exe")+"' "
				+"AND ProgName='"+POut.String("ThreeShape")+"'";
			Db.NonQ(command);
		}//End of 20_5_38() method

		private static void To20_5_41() {
			string command;
			command="SELECT program.Path FROM program WHERE ProgName='DentalEye'";
			string programPath=Db.GetScalar(command);//only one path
			if(programPath.Contains("DentalEye.exe\\CmdLink.exe")) {//this isn't a valid path, so fix it
				programPath=programPath.Replace("\\DentalEye.exe","");
				command="UPDATE program SET Path='"+POut.String(programPath)+"' WHERE ProgName='DentalEye'";
				Db.NonQ(command);
			}
		}//End of 20_5_41() method

		private static void To20_5_48() {
			string command;
			if(!IndexExists("claim","PatNum,ClaimStatus,ClaimType")) {
				command="ALTER TABLE claim ADD INDEX PatStatusType (PatNum,ClaimStatus,ClaimType)";
				List<string> listIndexNames=GetIndexNames("claim","PatNum");
				if(listIndexNames.Count>0) {
					command+=","+string.Join(",",listIndexNames.Select(x => $"DROP INDEX {x}"));
				}
				Db.NonQ(command);
			}
		}

		private static void To20_5_57() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraRefreshOnLoad','1')";//Default to true.
			Db.NonQ(command);
		}//End of 20_5_57() method

		private static void To20_5_61() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraStrictClaimMatching','0')"; //Default to false.
			Db.NonQ(command);
		}//End of 20_5_61() method

		private static void To20_5_65() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraShowStatusAndClinic','1')"; //Default to true.
			Db.NonQ(command);
		}//End of 20_5_65() method

		private static void To21_1_1() {
			string command;
			DataTable table;
			//Set the ExistingPat 2FA prefs pref to defautl vals for all clinics
			command="SELECT ClinicNum FROM clinic";
			List<long> listClinicNums=Db.GetListLong(command);
			if(listClinicNums.Count>0) {
				foreach(long clinicNum in listClinicNums) {
					command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'WebSchedExistingPatDoAuthEmail','1')"; //Default to true
					Db.NonQ(command);
					command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'WebSchedExistingPatDoAuthText','0')"; //Default to false
					Db.NonQ(command);
				}
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedExistingPatDoAuthEmail','1')"; //Default to true
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedExistingPatDoAuthText','0')"; //Default to false
			Db.NonQ(command);
			command="ALTER TABLE payplan ADD DynamicPayPlanTPOption tinyint NOT NULL";//will default to 0 'None' which should be ok since 1 will get set once TP added and OK clicked.
			Db.NonQ(command);
			if(!IndexExists("smsfrommobile","ClinicNum,SmsStatus,IsHidden")) {
				command="ALTER TABLE smsfrommobile ADD INDEX ClinicStatusHidden (ClinicNum,SmsStatus,IsHidden)";
				List<string> listIndexesToDrop=GetIndexNames("smsfrommobile","ClinicNum");
				if(!listIndexesToDrop.IsNullOrEmpty()) {
					command+=","+string.Join(",",listIndexesToDrop.Select(x => "DROP INDEX "+x));
				}
				Db.NonQ(command);
			}
			command="ALTER TABLE tsitranslog CHANGE DemandType ServiceType TINYINT NOT NULL";
			Db.NonQ(command);
			command="UPDATE program SET ProgDesc='Central Data Storage from centraldatastorage.com' "
				+"WHERE ProgName='CentralDataStorage' "
				+"AND ProgDesc='Cental Data Storage from centraldatastorage.com'";
			Db.NonQ(command);
			command="ALTER TABLE discountplan ADD PlanNote text NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE discountplansub ADD SubNote text NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailHostingUseNoReply','1')";//default to true.
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailSecureStatus','0')";//default to NotActivated or Enabled.
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('Ins834IsEmployerCreate','1')"; //Default to true
			Db.NonQ(command);
			command="ALTER TABLE document MODIFY COLUMN Note MEDIUMTEXT NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE apptreminderrule ADD TemplateFailureAutoReply text NOT NULL";
			Db.NonQ(command);
			command="UPDATE apptreminderrule SET TemplateFailureAutoReply='There was an error confirming your appointment with [OfficeName]."
				+" Please call [OfficePhone] to confirm.' WHERE TypeCur=1"; //1 - ConfirmationFutureDay
			Db.NonQ(command);
			command="ALTER TABLE discountplan ADD ExamFreqLimit int NOT NULL,ADD XrayFreqLimit int NOT NULL,ADD ProphyFreqLimit int NOT NULL,"
				+"ADD FluorideFreqLimit int NOT NULL,ADD PerioFreqLimit int NOT NULL,ADD LimitedExamFreqLimit int NOT NULL,ADD PAFreqLimit int NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanExamCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanXrayCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanProphyCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanFluorideCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanPerioCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanLimitedCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanPACodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('CloudAllowedIpAddresses','')";
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				 groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES("+POut.Long(groupNum)+",206)";
				 Db.NonQ(command);
			}
			command="TRUNCATE TABLE eservicelog";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog DROP COLUMN SheetNum";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog DROP COLUMN PayNum";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog DROP COLUMN AptNum";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog DROP COLUMN EServiceOperation";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog DROP COLUMN EServiceCode";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD EServiceType tinyint";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD EServiceAction smallint";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD KeyType smallint";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD LogGuid VARCHAR(16)";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD ClinicNum bigint";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD FKey bigint";
			Db.NonQ(command);
			if(!IndexExists("eservicelog","LogDateTime,ClinicNum")) {
				command="ALTER TABLE eservicelog ADD INDEX ClinicDateTime (LogDateTime,ClinicNum)";
				List<string> listIndexesToDrop=GetIndexNames("eservicelog","ClinicNum");
				if(!listIndexesToDrop.IsNullOrEmpty()) {
					command+=","+string.Join(",",listIndexesToDrop.Select(x => "DROP INDEX "+x));
				}
				Db.NonQ(command);
			}
			command="ALTER TABLE discountplan ADD AnnualMax double NOT NULL DEFAULT -1";
			Db.NonQ(command);
		}//End of 21_1_1() method

		private static void To21_1_3() {
			string command;
			DataTable table;
			//This was done way back in 16.4 but the SheetsDefaultTreatmentPlan preference did not get fully implemented.
			//This commit is to officially implement SheetsDefaultTreatmentPlan (along with clinic specific overrides for the pref).
			//Therefore, the current value in the preference table needs to be updated with the most recent SheetDefNum that would be currently used.
			//The following code mimics the behavior of SheetDefs.GetInternalOrCustom() which is being used for TP sheets at the time of this commit.
			//E.g. ListSheetDefs.OrderBy(x => x.Description).ThenBy(x => x.SheetDefNum).FirstOrDefault()
			command=@"SELECT SheetDefNum
				FROM sheetdef 
				WHERE SheetType=17
				ORDER BY Description,SheetDefNum
				LIMIT 1";
			table=Db.GetTable(command);//GetScalar won't work with this particular query because it may not return a row (no custom sheet def).
			long treatmentPlanSheetDefNum=0;
			if(table.Rows.Count > 0) {
				treatmentPlanSheetDefNum=PIn.Long(table.Rows[0]["SheetDefNum"].ToString());
			}
			command=$@"UPDATE preference SET ValueString='{POut.Long(treatmentPlanSheetDefNum)}' WHERE PrefName='SheetsDefaultTreatmentPlan'";
			Db.NonQ(command);
		}

		private static void To21_1_5() {
			string command;
			DataTable table;
			command="UPDATE alertitem SET ClinicNum=-1 WHERE Type=23 AND ClinicNum=0";//23=SupplementalBackups
			Db.NonQ(command);
		}

		private static void To21_1_6() {
			string command;
			DataTable table;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EraAutomationBehavior','1')";//1 - EraAutomationMode.ReviewAll by default.
			Db.NonQ(command);
			command="ALTER TABLE carrier ADD EraAutomationOverride tinyint NOT NULL";//0 - EraAutomationMode.UseGlobal by default.
			Db.NonQ(command);
			DoseSpotSelfReportedInvalidNote();
			command="SELECT preference.ValueString FROM preference WHERE preference.PrefName IN ('AppointmentTimeArrivedTrigger')";
			List<long> listDefNums=Db.GetListLong(command,hasExceptions:false).Where(x => x!=0).Distinct().ToList();
			string defNums=string.Join(",",listDefNums);
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('ApptConfirmByodEnabled','"+defNums+"')";
			Db.NonQ(command);
			command="SELECT preference.ValueString FROM preference WHERE preference.PrefName IN ('AppointmentTimeArrivedTrigger',"
				+"'AppointmentTimeSeatedTrigger','AppointmentTimeDismissedTrigger')";
			listDefNums=Db.GetListLong(command,hasExceptions:false).Where(x => x!=0).Distinct().ToList();
			defNums=string.Join(",",listDefNums);
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('ApptConfirmExcludeEclipboard','"+defNums+"')";
			Db.NonQ(command);
			//Set default to 'Legacy' to set the column value without updating the timestamp.
			command="ALTER TABLE emailmessage ADD MsgType varchar(255) NOT NULL DEFAULT 'Legacy',ADD FailReason varchar(255) NOT NULL";
			Db.NonQ(command);
			//Set default back to ''
			command="ALTER TABLE emailmessage MODIFY MsgType varchar(255) NOT NULL";
			Db.NonQ(command);
		}//End of 21_1_6() method

		private static void To21_1_9() {
			string command;
			DataTable table;
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=208";
			table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=149";//Currently has Payment Plan Edit permissions
				table=Db.GetTable(command);
				long groupNum;
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(groupNum)+",208)";//208 - Will be given Payment Plan Charge Date Edit permissions
					Db.NonQ(command);
				}
			}
		}//End of 21_1_9() method

		private static void To21_1_13() {
			string command;
			command="SELECT DISTINCT ClinicNum FROM clinicpref";
			List<long> listClinicNums=Db.GetListLong(command);
			for(int i=0;i<listClinicNums.Count;i++) {
				command=$@"SELECT ClinicPrefNum FROM clinicpref WHERE PrefName='WebSchedExistingPatDoAuthEmail' AND ClinicNum={POut.Long(listClinicNums[i])}";
				List<long> listClinicPrefNums=Db.GetListLong(command);
				if(listClinicPrefNums.Count>1) {
					//We will not delete the last one.
					for(int j=0;j<listClinicPrefNums.Count-1;j++) {
						command=@$"DELETE FROM clinicpref WHERE ClinicPrefNum={POut.Long(listClinicPrefNums[j])}";
						Db.NonQ(command);
					}
				}
				command=$@"SELECT ClinicPrefNum FROM clinicpref WHERE PrefName='WebSchedExistingPatDoAuthText' AND ClinicNum={POut.Long(listClinicNums[i])}";
				listClinicPrefNums=Db.GetListLong(command);
				if(listClinicPrefNums.Count>1) {
					//We will not delete the last one.
					for(int j=0;j<listClinicPrefNums.Count-1;j++) {
						command=@$"DELETE FROM clinicpref WHERE ClinicPrefNum={POut.Long(listClinicPrefNums[j])}";
						Db.NonQ(command);
					}
				}
			}
			command="UPDATE program SET Path='"+POut.String(@"C:\Program Files\3Shape\Dental Desktop\Plugins\ThreeShape.PracticeManagementIntegration\DentalDesktopCmd.exe")+"' "
				+"WHERE Path='"+POut.String(@"C:\Program Files\3Shape\Dental Desktop\Plugins\ThreeShape.PMSIntegration\DentalDesktopCmd.exe")+"' "
				+"AND ProgName='"+POut.String("ThreeShape")+"'";
			Db.NonQ(command);
		}//End of 21_1_13() method

		private static void To21_1_16() {
			string command;
			DataTable table;
			if(!LargeTableHelper.IndexExists("payment","PayType")) {
				command="ALTER TABLE payment ADD INDEX (PayType)";//FK to definition.DefNum with category 10
				Db.NonQ(command);
			}
			command="SELECT program.Path FROM program WHERE ProgName='DentalEye'";
			string programPath=Db.GetScalar(command);//only one path
			if(programPath.Contains("DentalEye.exe\\CmdLink.exe")) {//this isn't a valid path, so fix it
				programPath=programPath.Replace("\\DentalEye.exe","");
				command="UPDATE program SET Path='"+POut.String(programPath)+"' WHERE ProgName='DentalEye'";
				Db.NonQ(command);
			}
		}//End of 21_1_16() method

		private static void To21_1_22() {
			string command;
			if(!IndexExists("claim","PatNum,ClaimStatus,ClaimType")) {
				command="ALTER TABLE claim ADD INDEX PatStatusType (PatNum,ClaimStatus,ClaimType)";
				List<string> listIndexNames=GetIndexNames("claim","PatNum");
				if(listIndexNames.Count>0) {
					command+=","+string.Join(",",listIndexNames.Select(x => $"DROP INDEX {x}"));
				}
				Db.NonQ(command);
			}
		}//End of 21_1_22() method

		private static void To21_1_28() {
			//Update existing discountplans with all frequency limitations set to 0, to have unlimited frequency limitation.
			string command=$@"UPDATE discountplan
					SET ExamFreqLimit=-1,XrayFreqLimit=-1,ProphyFreqLimit=-1,FluorideFreqLimit=-1,PerioFreqLimit=-1,LimitedExamFreqLimit=-1,PAFreqLimit=-1
					WHERE ExamFreqLimit=0 AND XrayFreqLimit=0 AND ProphyFreqLimit=0 AND FluorideFreqLimit=0 AND PerioFreqLimit=0 AND LimitedExamFreqLimit=0 AND PAFreqLimit=0";
			Db.NonQ(command);
		}//End of 21_1_28() method

		private static void To21_1_31() {
			string command = "SELECT * FROM preference WHERE PrefName='EraRefreshOnLoad'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraRefreshOnLoad','1')";//Default to true.
				Db.NonQ(command);
			}
		}//End of 21_1_31() method

		private static void To21_1_35() {
			string command="SELECT * FROM preference WHERE PrefName='EraStrictClaimMatching'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraStrictClaimMatching','0')"; //Default to false.
				Db.NonQ(command);
			}
		}//End of 21_1_35() method

		private static void To21_1_37() {
			string command="SELECT * FROM preference WHERE PrefName='EraShowStatusAndClinic'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraShowStatusAndClinic','1')"; //Default to true.
				Db.NonQ(command);
			}
		}//End of 21_1_37() method
	}
}
