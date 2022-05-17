using CodeBase;
using DataConnectionBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OpenDentBusiness {
	public partial class ConvertDatabases {

		#region Helper Functions

		private static string Curdate() {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				return "SYSDATE";
			}
			return "CURDATE()";
		}   ///<summary>Helper for Oracle that will return equivalent of MySql NOW()</summary>\

		private static string Now() {
			if(DataConnection.DBtype==DatabaseType.Oracle) {
				//return "(SELECT TO_CHAR(SYSDATE,'YYYY-MM-DD HH24:MI:SS') FROM DUAL)";
				return "SYSDATE";
			}
			return "NOW()";
		}

		#endregion

		///<summary>Oracle compatible: 05/17/2016</summary>
		private static void To16_2_1() {
			if(FromVersion<new Version("16.2.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.2.1");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('PatientAllSuperFamilySync','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PatientAllSuperFamilySync','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE hl7def ADD IsProcApptEnforced tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE hl7def ADD IsProcApptEnforced number(3)";
					Db.NonQ(command);
					command="UPDATE hl7def SET IsProcApptEnforced = 0 WHERE IsProcApptEnforced IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE hl7def MODIFY IsProcApptEnforced NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS hl7procattach";
					Db.NonQ(command);
					command=@"CREATE TABLE hl7procattach (
						HL7ProcAttachNum bigint NOT NULL auto_increment PRIMARY KEY,
						HL7MsgNum bigint NOT NULL,
						ProcNum bigint NOT NULL,
						INDEX(HL7MsgNum),
						INDEX(ProcNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE hl7procattach'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE hl7procattach (
						HL7ProcAttachNum number(20) NOT NULL,
						HL7MsgNum number(20) NOT NULL,
						ProcNum number(20) NOT NULL,
						CONSTRAINT hl7procattach_HL7ProcAttachNum PRIMARY KEY (HL7ProcAttachNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX hl7procattach_HL7MsgNum ON hl7procattach (HL7MsgNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX hl7procattach_ProcNum ON hl7procattach (ProcNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE creditcard ADD CCSource tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE creditcard ADD CCSource number(3)";
					Db.NonQ(command);
					command="UPDATE creditcard SET CCSource = 0 WHERE CCSource IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE creditcard MODIFY CCSource NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE creditcard SET CCSource = 1 WHERE XChargeToken != ''";//CreditCardSource.XServer
				Db.NonQ(command);
				command="UPDATE creditcard SET CCSource = 3 WHERE PayConnectToken != ''";//CreditCardSource.PayConnect
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('TextingDefaultClinicNum','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'TextingDefaultClinicNum','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sheet ADD IsDeleted tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sheet ADD IsDeleted number(3)";
					Db.NonQ(command);
					command="UPDATE sheet SET IsDeleted = 0 WHERE IsDeleted IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE sheet MODIFY IsDeleted NOT NULL";
					Db.NonQ(command);
				}				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sheetfield ADD DateTimeSig datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sheetfield ADD DateTimeSig date";
					Db.NonQ(command);
					command="UPDATE sheetfield SET DateTimeSig = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateTimeSig IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE sheetfield MODIFY DateTimeSig NOT NULL";
					Db.NonQ(command);
				}
				//Insert RapidCall bridge-----------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'RapidCall', "
						+"'Rapid Call from www.dentaltek.com', "
						+"'0', "
						+"'"+POut.String(@"C:\DentalTek\CallTray\CallTray.exe")+"', "
						+"'"+POut.String(@"/DeepLink=RapidCall")+"', "//leave blank if none
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
						+"'RapidCall', "
						+"'Rapid Call from www.dentaltek.com', "
						+"'0', "
						+"'"+POut.String(@"C:\DentalTek\CallTray\CallTray.exe")+"', "
						+"'"+POut.String(@"/DeepLink=RapidCall")+"', "//leave blank if none
						+"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Disable Advertising', "
						+"'0', "
						+"'0')";
					Db.NonQ(command);
				}//end RapidCall bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE schedule ADD ClinicNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE schedule ADD INDEX (ClinicNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE schedule ADD ClinicNum number(20)";
					Db.NonQ(command);
					command="UPDATE schedule SET ClinicNum = 0 WHERE ClinicNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE schedule MODIFY ClinicNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX schedule_ClinicNum ON schedule (ClinicNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE apptview ADD IsApptBubblesDisabled tinyint NOT NULL";
					Db.NonQ(command);
					command="UPDATE apptview SET IsApptBubblesDisabled=(SELECT ValueString FROM preference WHERE PrefName='AppointmentBubblesDisabled')";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE apptview ADD IsApptBubblesDisabled number(3)";
					Db.NonQ(command);
					command="UPDATE apptview SET IsApptBubblesDisabled = 0 WHERE IsApptBubblesDisabled IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE apptview MODIFY IsApptBubblesDisabled NOT NULL";
					Db.NonQ(command);
					command="UPDATE apptview SET IsApptBubblesDisabled=(SELECT ValueString FROM preference WHERE PrefName='AppointmentBubblesDisabled')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE computerpref ADD NoShowDecimal tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE computerpref ADD NoShowDecimal number(3)";
					Db.NonQ(command);
					command="UPDATE computerpref SET NoShowDecimal = 0 WHERE NoShowDecimal IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE computerpref MODIFY NoShowDecimal NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE creditcard ADD ClinicNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE creditcard ADD INDEX (ClinicNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE creditcard ADD ClinicNum number(20)";
					Db.NonQ(command);
					command="UPDATE creditcard SET ClinicNum = 0 WHERE ClinicNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE creditcard MODIFY ClinicNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX creditcard_ClinicNum ON creditcard (ClinicNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE payment ADD PaymentSource tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE payment ADD PaymentSource number(3)";
					Db.NonQ(command);
					command="UPDATE payment SET PaymentSource = 0 WHERE PaymentSource IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE payment MODIFY PaymentSource NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE payment ADD ProcessStatus tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE payment ADD ProcessStatus number(3)";
					Db.NonQ(command);
					command="UPDATE payment SET ProcessStatus = 0 WHERE ProcessStatus IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE payment MODIFY ProcessStatus NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE dunning ADD DaysInAdvance int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE dunning ADD DaysInAdvance number(11)";
					Db.NonQ(command);
					command="UPDATE dunning SET DaysInAdvance = 0 WHERE DaysInAdvance IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE dunning MODIFY DaysInAdvance NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS displayreport";
					Db.NonQ(command);
					command=@"CREATE TABLE displayreport (
						DisplayReportNum bigint NOT NULL auto_increment PRIMARY KEY,
						InternalName varchar(255) NOT NULL,
						ItemOrder int NOT NULL,
						Description varchar(255) NOT NULL,
						Category tinyint NOT NULL,
						IsHidden tinyint NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE displayreport'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE displayreport (
						DisplayReportNum number(20) NOT NULL,
						InternalName varchar2(255),
						ItemOrder number(11) NOT NULL,
						Description varchar2(255),
						Category number(3) NOT NULL,
						IsHidden number(3) NOT NULL,
						CONSTRAINT displayreport_DisplayReportNum PRIMARY KEY (DisplayReportNum)
						)";
					Db.NonQ(command);
				}
				//default display reports.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command=@"
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODToday',0,'Today',0,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODYesterday',1,'Yesterday',0,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODThisMonth',2,'This Month',0,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODLastMonth',3,'Last Month',0,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODThisYear',4,'This Year',0,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODMoreOptions',5,'More Options',0,0);

					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODAdjustments',0,'Adjustments',1,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODPayments',1,'Payments',1,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODProcedures',2,'Procedures',1,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODWriteoffs',3,'Writeoffs',1,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODIncompleteProcNotes',4,'Incomplete Procedure Notes',1,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODRoutingSlips',5,'Routing Slips',1,0);

					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODAgingAR',0,'Aging of A/R',2,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODClaimsNotSent',1,'Claims Not Sent',2,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODCapitation',2,'Capitation Utilization',2,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODFinanceCharge',3,'Finance Charge Report',2,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODOutstandingInsClaims',4,'Outstanding Insurance Claims',2,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODProcsNotBilled',5,'Procedures Not Billed to Insurance',2,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODPPOWriteoffs',6,'PPO Writeoffs',2,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODPaymentPlans',7,'Payment Plans',2,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODReceivablesBreakdown',8,'Receivables Breakdown',2,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODUnearnedIncome',9,'Unearned Income',2,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODInsuranceOverpaid',10,'Insurance Overpaid',2,0);

					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODActivePatients',0,'Active Patients',3,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODAppointments',1,'Appointments',3,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODBirthdays',2,'Birthdays',3,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODBrokenAppointments',3,'BrokenAppointments',3,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODInsurancePlans',4,'Insurance Plans',3,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODNewPatients',5,'New Patients',3,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODPatientsRaw',6,'Patients - Raw',3,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODPatientNotes',7,'Patient Notes',3,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODPrescriptions',8,'Prescriptions',3,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODProcedureCodes',9,'Procedure Codes',3,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODReferralsRaw',10,'Referrals - Raw',3,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODReferralAnalysis',11,'Referral Analysis',3,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODReferredProcTracking',12,'Referred Proc Tracking',3,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODTreatmentFinder',13,'Treatment Finder',3,0);

					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODRawScreeningData',0,'Raw Screening Data',4,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODRawPopulationData',1,'Raw Population Data',4,0);

					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODEligibilityFile',0,'Eligibility File',5,0);
					INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODEncounterFile',1,'Encounter File',5,0);";
					Db.NonQ(command);
				}
				else {//oracle
					command=@"
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT COALESCE(MAX(DisplayReportNum), 0)+1 FROM displayreport),'ODToday',0,'Today',0,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODYesterday',1,'Yesterday',0,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODThisMonth',2,'This Month',0,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODLastMonth',3,'Last Month',0,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODThisYear',4,'This Year',0,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODMoreOptions',5,'More Options',0,0);

					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODAdjustments',0,'Adjustments',1,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODPayments',1,'Payments',1,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODProcedures',2,'Procedures',1,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODWriteoffs',3,'Writeoffs',1,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODIncompleteProcNotes',4,'Incomplete Procedure Notes',1,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODRoutingSlips',5,'Routing Slips',1,0);

					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODAgingAR',0,'Aging of A/R',2,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODClaimsNotSent',1,'Claims Not Sent',2,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODCapitation',2,'Capitation Utilization',2,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODFinanceCharge',3,'Finance Charge Report',2,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODOutstandingInsClaims',4,'Outstanding Insurance Claims',2,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODProcsNotBilled',5,'Procedures Not Billed to Insurance',2,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODPPOWriteoffs',6,'PPO Writeoffs',2,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODPaymentPlans',7,'Payment Plans',2,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODReceivablesBreakdown',8,'Receivables Breakdown',2,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODUnearnedIncome',9,'Unearned Income',2,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODInsuranceOverpaid',10,'Insurance Overpaid',2,0);

					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODActivePatients',0,'Active Patients',3,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODAppointments',1,'Appointments',3,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODBirthdays',2,'Birthdays',3,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODBrokenAppointments',3,'BrokenAppointments',3,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODInsurancePlans',4,'Insurance Plans',3,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODNewPatients',5,'New Patients',3,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODPatientsRaw',6,'Patients - Raw',3,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODPatientNotes',7,'Patient Notes',3,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODPrescriptions',8,'Prescriptions',3,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODProcedureCodes',9,'Procedure Codes',3,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODReferralsRaw',10,'Referrals - Raw',3,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODReferralAnalysis',11,'Referral Analysis',3,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODReferredProcTracking',12,'Referred Proc Tracking',3,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODTreatmentFinder',13,'Treatment Finder',3,0);

					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODRawScreeningData',0,'Raw Screening Data',4,0);
					INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES ((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODRawPopulationData',1,'Raw Population Data',4,0);

					INSERT INTO displayreport(DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODEligibilityFile',0,'Eligibility File',5,0);
					INSERT INTO displayreport(DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) VALUES((SELECT MAX(DisplayReportNum)+1 FROM displayreport),'ODEncounterFile',1,'Encounter File',5,0);";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {//Insert PaySplitUnearnedType definition if it doesn't already exist.
					command="SELECT COUNT(*) FROM definition WHERE Category=29 AND ItemName='Prepayment'";
					if(PIn.Int(Db.GetCount(command))==0) {//PaySplitUnearnedType definition doesn't already exist
						command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=29";
						string maxOrder=Db.GetScalar(command);
						if(maxOrder=="") {
							maxOrder="0";
						}
						command="INSERT INTO definition (Category, ItemOrder, ItemName) VALUES (29,"+maxOrder+",'Prepayment')";
						Db.NonQ(command);
					}
				}
				else {//oracle (Note for reviewer: I'm not at all sure this is oracle compatible)
					command="SELECT COUNT(*) FROM definition WHERE Category=29 AND ItemName='Prepayment'";
					if(PIn.Int(Db.GetCount(command))==0) {//PaySplitUnearnedType definition doesn't already exist
						command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=29";
						string maxOrder=Db.GetScalar(command);
						if(maxOrder=="") {
							maxOrder="0";
						}
						command="INSERT INTO definition (DefNum,Category, ItemOrder, ItemName) VALUES ((SELECT MAX(DefNum)+1 FROM definition),29,"+maxOrder+",'Prepayment')";
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT DefNum FROM definition WHERE Category=29 AND ItemName='Prepayment'";
					string defNum=Db.GetScalar(command);
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PrepaymentUnearnedType','"+defNum+"')";
					Db.NonQ(command);
				}
				else {//oracle
					command="SELECT DefNum FROM definition WHERE Category=29 AND ItemName='Prepayment'";
					string defNum=Db.GetScalar(command);
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PrepaymentUnearnedType','"+defNum+"')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE paysplit ADD PrepaymentNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE paysplit ADD INDEX (PrepaymentNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE paysplit ADD PrepaymentNum number(20)";
					Db.NonQ(command);
					command="UPDATE paysplit SET PrepaymentNum = 0 WHERE PrepaymentNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE paysplit MODIFY PrepaymentNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX paysplit_PrepaymentNum ON paysplit (PrepaymentNum)";
					Db.NonQ(command);
				}
				//Add InsPlanEdit permission to everyone------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				DataTable table=Db.GetTable(command);
				long groupNum;
				if(DataConnection.DBtype==DatabaseType.MySql) {
				   foreach(DataRow row in table.Rows) {
					  groupNum=PIn.Long(row["UserGroupNum"].ToString());
					  command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(groupNum)+",110)";//110 - InsPlanEdit
					  Db.NonQ(command);
				   }
				}
				else {//oracle
				   foreach(DataRow row in table.Rows) {
					  groupNum=PIn.Long(row["UserGroupNum"].ToString());
					  command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
						 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",110)";//110 - InsPlanEdit
					  Db.NonQ(command);
				   }
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {//Insert Canceled Appointment Procedure account color
					command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=0";//0 is AccountColor
					string maxOrder=Db.GetScalar(command);
					if(maxOrder=="") {
						maxOrder="0";
					}
					command="SELECT ItemColor FROM definition WHERE Category=0 AND ItemName='Broken Appointment Procedure'";
					string color=Db.GetScalar(command);
					if(color=="") {
						color="-16777031";//blue
					}
					command="INSERT INTO definition (Category, ItemOrder, ItemName, ItemColor) "
						+"VALUES (0,"+maxOrder+",'Canceled Appointment Procedure','"+color+"')";
					Db.NonQ(command);
				}
				else {//oracle 
					command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=0";//0 is AccountColor
					string maxOrder=Db.GetScalar(command);
					if(maxOrder=="") {
						maxOrder="0";
					}
					command="SELECT ItemColor FROM definition WHERE Category=0 AND ItemName='Broken Appointment Procedure'";
					string color=Db.GetScalar(command);
					if(color=="") {
						color="-16777031";//blue
					}
					command="INSERT INTO definition (DefNum, Category, ItemOrder, ItemName, ItemColor) "
						+"VALUES ((SELECT MAX(DefNum)+1 FROM definition),0,"+maxOrder+",'Canceled Appointment Procedure','"+color+"')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrmeasureevent ADD TobaccoCessationDesire tinyint unsigned NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ehrmeasureevent ADD TobaccoCessationDesire number(3)";
					Db.NonQ(command);
					command="UPDATE ehrmeasureevent SET TobaccoCessationDesire = 0 WHERE TobaccoCessationDesire IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE ehrmeasureevent MODIFY TobaccoCessationDesire NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrmeasureevent ADD DateStartTobacco date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ehrmeasureevent ADD DateStartTobacco date";
					Db.NonQ(command);
					command="UPDATE ehrmeasureevent SET DateStartTobacco = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateStartTobacco IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE ehrmeasureevent MODIFY DateStartTobacco NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD DateComplete date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD DateComplete date";
					Db.NonQ(command);
					command="UPDATE procedurelog SET DateComplete = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateComplete IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog MODIFY DateComplete NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claimproc ADD DateSuppReceived date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claimproc ADD DateSuppReceived date";
					Db.NonQ(command);
					command="UPDATE claimproc SET DateSuppReceived = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateSuppReceived IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimproc MODIFY DateSuppReceived NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('WikiDetectLinks','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WikiDetectLinks','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('WikiCreatePageFromLink','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WikiCreatePageFromLink','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('BadDebtAdjustmentTypes','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'BadDebtAdjustmentTypes','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE automation ADD AptStatus tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE automation ADD AptStatus number(3)";
					Db.NonQ(command);
					command="UPDATE automation SET AptStatus = 0 WHERE AptStatus IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE automation MODIFY AptStatus NOT NULL";
					Db.NonQ(command);
				}
				//Insert Carestream bridge-----------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'Carestream', "
						+"'Carestream from www.carestreamdental.com', "
						+"'0', "
						+"'"+POut.String(@"pwimage.exe")+"', "
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
						+"'Patient.ini path', "
						+"'"+POut.String(@"C:\Carestream\Patient.ini")+"')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"'"+POut.Long(programNum)+"', "
						+"'2', "//ToolBarsAvail.ChartModule
						+"'Carestream')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'Carestream', "
						+"'Carestream from www.carestreamdental.com', "
						+"'0', "
						+"'"+POut.String(@"pwimage.exe")+"', "
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
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Patient.ini path', "
						+"'"+POut.String(@"C:\Carestream\Patient.ini")+"', "
						+"'0')";
					Db.NonQ(command);
					command="INSERT INTO toolbutitem (ToolButItemNum,ProgramNum,ToolBar,ButtonText) "
						+"VALUES ("
						+"(SELECT MAX(ToolButItemNum)+1 FROM toolbutitem),"
						+"'"+POut.Long(programNum)+"', "
						+"'2', "//ToolBarsAvail.ChartModule
						+"'Carestream')";
					Db.NonQ(command);
				}//end Carestream bridge
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('CentralManagerSyncCode','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'CentralManagerSyncCode','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claimsnapshot ADD ClaimProcNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimsnapshot ADD INDEX (ClaimProcNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claimsnapshot ADD ClaimProcNum number(20)";
					Db.NonQ(command);
					command="UPDATE claimsnapshot SET ClaimProcNum = 0 WHERE ClaimProcNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimsnapshot MODIFY ClaimProcNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX claimsnapshot_ClaimProcNum ON claimsnapshot (ClaimProcNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claimsnapshot ADD SnapshotTrigger tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claimsnapshot ADD SnapshotTrigger number(3)";
					Db.NonQ(command);
					command="UPDATE claimsnapshot SET SnapshotTrigger = 0 WHERE SnapshotTrigger IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimsnapshot MODIFY SnapshotTrigger NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
				   command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimSnapshotRunTime','1881-01-01 23:30:00')";
				   Db.NonQ(command);
				}
				else {//oracle
				   command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ClaimSnapshotRunTime','1881-01-01 23:30:00')";
				   Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
				   command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimSnapshotTriggerType','ClaimCreate')";
				   Db.NonQ(command);
				}
				else {//oracle
				   command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ClaimSnapshotTriggerType','ClaimCreate')";
				   Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD ItemOrder int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD ItemOrder number(11)";
					Db.NonQ(command);
					command="UPDATE clinic SET ItemOrder = 0 WHERE ItemOrder IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clinic MODIFY ItemOrder NOT NULL";
					Db.NonQ(command);
				}
				command="SELECT * FROM clinic";
				DataTable clinics=Db.GetTable(command);
				for(int i=0;i<clinics.Rows.Count;i++) {
					command="UPDATE clinic SET ItemOrder="+POut.Int(i)+" WHERE ClinicNum="+clinics.Rows[i]["ClinicNum"].ToString();
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES ('ClinicListIsAlphabetical','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ClinicListIsAlphabetical','0')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString='1' WHERE PrefName='ClaimSnapshotEnabled'";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clockevent ADD ClinicNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE clockevent ADD INDEX (ClinicNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clockevent ADD ClinicNum number(20)";
					Db.NonQ(command);
					command="UPDATE clockevent SET ClinicNum = 0 WHERE ClinicNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clockevent MODIFY ClinicNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX clockevent_ClinicNum ON clockevent (ClinicNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE timeadjust ADD ClinicNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE timeadjust ADD INDEX (ClinicNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE timeadjust ADD ClinicNum number(20)";
					Db.NonQ(command);
					command="UPDATE timeadjust SET ClinicNum = 0 WHERE ClinicNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE timeadjust MODIFY ClinicNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX timeadjust_ClinicNum ON timeadjust (ClinicNum)";
					Db.NonQ(command);
				}
				//Users with ProcComplEdit permission will be granted ProcComplEditLimited permission with the same days/date restriction.
				command="SELECT NewerDate,NewerDays,UserGroupNum FROM grouppermission WHERE PermType=10"; //ProcComplEdit
				table=Db.GetTable(command);
				DateTime newerDate;
				int newerDays;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					foreach(DataRow row in table.Rows) {
						newerDate=PIn.Date(row["NewerDate"].ToString());
						newerDays=PIn.Int(row["NewerDays"].ToString());
						groupNum=PIn.Long(row["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType) "
							+"VALUES("+POut.Date(newerDate)+","+POut.Int(newerDays)+","+POut.Long(groupNum)+",117)";//117 - ProcComplEditLimited
						Db.NonQ(command);
					}
				}
				else {//oracle
					foreach(DataRow row in table.Rows) {
						newerDate=PIn.Date(row["NewerDate"].ToString());
						newerDays=PIn.Int(row["NewerDays"].ToString());
						groupNum=PIn.Long(row["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),"+POut.Date(newerDate)+","+POut.Int(newerDays)+","
							+POut.Long(groupNum)+",117)";//117 - ProcComplEditLimited
						Db.NonQ(command);
					}
				}
				//Add ClaimDelete permission for everyone
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					foreach(DataRow row in table.Rows) {
						groupNum=PIn.Long(row["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",118)";//118 - ClaimDelete
						Db.NonQ(command);
					}
				}
				else {//oracle
					foreach(DataRow row in table.Rows) {
						groupNum=PIn.Long(row["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission)"
							+",TO_DATE('0001-01-01','YYYY-MM-DD')"
							+",0"
							+","+POut.Long(groupNum)+",118)";//118 - ClaimDelete
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimProcsNotBilledToInsAutoGroup','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ClaimProcsNotBilledToInsAutoGroup','1')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS xwebresponse";
					Db.NonQ(command);
					command=@"CREATE TABLE xwebresponse (
						XWebResponseNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						ProvNum bigint NOT NULL,
						ClinicNum bigint NOT NULL,
						PaymentNum bigint NOT NULL,
						DateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						DateTUpdate datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						TransactionStatus tinyint NOT NULL,
						ResponseCode int NOT NULL,
						XWebResponseCode varchar(255) NOT NULL,
						ResponseDescription varchar(255) NOT NULL,
						OTK varchar(255) NOT NULL,
						HpfUrl text NOT NULL,
						HpfExpiration datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						TransactionID varchar(255) NOT NULL,
						TransactionType varchar(255) NOT NULL,
						Alias varchar(255) NOT NULL,
						CardType varchar(255) NOT NULL,
						CardBrand varchar(255) NOT NULL,
						CardBrandShort varchar(255) NOT NULL,
						MaskedAcctNum varchar(255) NOT NULL,
						Amount double NOT NULL,
						ApprovalCode varchar(255) NOT NULL,
						CardCodeResponse varchar(255) NOT NULL,
						ReceiptID int NOT NULL,
						ExpDate varchar(255) NOT NULL,
						EntryMethod varchar(255) NOT NULL,
						ProcessorResponse varchar(255) NOT NULL,
						BatchNum int NOT NULL,
						BatchAmount double NOT NULL,
						AccountExpirationDate date NOT NULL DEFAULT '0001-01-01',
						DebugError text NOT NULL,
						PayNote text NOT NULL,
						INDEX(PatNum),
						INDEX(ProvNum),
						INDEX(ClinicNum),
						INDEX(PaymentNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE xwebresponse'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE xwebresponse (
						XWebResponseNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						ProvNum number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						PaymentNum number(20) NOT NULL,
						DateTEntry date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateTUpdate date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						TransactionStatus number(3) NOT NULL,
						ResponseCode number(11) NOT NULL,
						XWebResponseCode varchar2(255),
						ResponseDescription varchar2(255),
						OTK varchar2(255),
						HpfUrl clob,
						HpfExpiration date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						TransactionID varchar2(255),
						TransactionType varchar2(255),
						Alias varchar2(255),
						CardType varchar2(255),
						CardBrand varchar2(255),
						CardBrandShort varchar2(255),
						MaskedAcctNum varchar2(255),
						Amount number(38,8) NOT NULL,
						ApprovalCode varchar2(255),
						CardCodeResponse varchar2(255),
						ReceiptID number(11) NOT NULL,
						ExpDate varchar2(255),
						EntryMethod varchar2(255),
						ProcessorResponse varchar2(255),
						BatchNum number(11) NOT NULL,
						BatchAmount number(38,8) NOT NULL,
						AccountExpirationDate date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DebugError clob,
						PayNote clob,
						CONSTRAINT xwebresponse_XWebResponseNum PRIMARY KEY (XWebResponseNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX xwebresponse_PatNum ON xwebresponse (PatNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX xwebresponse_ProvNum ON xwebresponse (ProvNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX xwebresponse_ClinicNum ON xwebresponse (ClinicNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX xwebresponse_PaymentNum ON xwebresponse (PaymentNum)";
					Db.NonQ(command);
				}
				//Add InsWriteOffEdit permission for everyone
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					foreach(DataRow row in table.Rows) {
						groupNum=PIn.Long(row["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",119)";//119 - InsWriteOffEdit
						Db.NonQ(command);
					}
				}
				else {//oracle
					foreach(DataRow row in table.Rows) {
						groupNum=PIn.Long(row["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType) "
							+"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission)"
							+",TO_DATE('0001-01-01','YYYY-MM-DD')"
							+",0"
							+","+POut.Long(groupNum)+",119)";//119 - InsWriteOffEdit
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
				   command="INSERT INTO preference(PrefName,ValueString) VALUES('InsVerifyExcludePatientClones','0')";
				   Db.NonQ(command);
				}
				else {//oracle
				   command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'InsVerifyExcludePatientClones','0')";
				   Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
				   command="INSERT INTO preference(PrefName,ValueString) VALUES('InsVerifyExcludePatVerify','0')";
				   Db.NonQ(command);
				}
				else {//oracle
				   command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'InsVerifyExcludePatVerify','0')";
				   Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD IsInsVerifyExcluded tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD IsInsVerifyExcluded number(3)";
					Db.NonQ(command);
					command="UPDATE clinic SET IsInsVerifyExcluded = 0 WHERE IsInsVerifyExcluded IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clinic MODIFY IsInsVerifyExcluded NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('PayPlansVersion','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PayPlansVersion','1')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE payplancharge ADD ChargeType tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE payplancharge ADD ChargeType number(3)";
					Db.NonQ(command);
					command="UPDATE payplancharge SET ChargeType = 0 WHERE ChargeType IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE payplancharge MODIFY ChargeType NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE payplancharge ADD ProcNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE payplancharge ADD INDEX (ProcNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE payplancharge ADD ProcNum number(20)";
					Db.NonQ(command);
					command="UPDATE payplancharge SET ProcNum = 0 WHERE ProcNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE payplancharge MODIFY ProcNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX payplancharge_ProcNum ON payplancharge (ProcNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('AccountShowCompletedPaymentPlans','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AccountShowCompletedPaymentPlans','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE payplan ADD IsClosed tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE payplan ADD IsClosed number(3)";
					Db.NonQ(command);
					command="UPDATE payplan SET IsClosed = 0 WHERE IsClosed IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE payplan MODIFY IsClosed NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) { //Oracle users will have to manually close payment plans.
					//Get all payment plans that have been paid off and need to be "closed".
					command="SELECT payplan.PayPlanNum,SUM(payplancharge.Principal) AS Princ,SUM(payplancharge.Interest) AS Interest,"
					+"COALESCE(ps.TotPayments,0) AS TotPay,COALESCE(cp.InsPayments,0) AS InsPay,"
					+"MAX(payplancharge.ChargeDate) AS LastDate "
					+"FROM payplan "
					+"LEFT JOIN payplancharge ON payplancharge.PayPlanNum=payplan.PayPlanNum "
					+"LEFT JOIN ("
						+"SELECT paysplit.PayPlanNum, SUM(paysplit.SplitAmt) AS TotPayments "
						+"FROM paysplit "
						+"GROUP BY paysplit.PayPlanNum "
					+")ps ON ps.PayPlanNum = payplan.PayPlanNum "
					+"LEFT JOIN ( "
						+"SELECT claimproc.PayPlanNum, SUM(claimproc.InsPayAmt) AS InsPayments "
						+"FROM claimproc "
						+"GROUP BY claimproc.PayPlanNum "
					+")cp ON cp.PayPlanNum = payplan.PayPlanNum "
					+"GROUP BY payplan.PayPlanNum "
					+"HAVING Princ+Interest <= (TotPay + InsPay) AND LastDate <="+Curdate();
					table=Db.GetTable(command);
					string payPlanNums="";
					for(int i = 0;i < table.Rows.Count;i++) {
						if(i!=0) {
							payPlanNums+=",";
						}
						payPlanNums+=table.Rows[i]["PayPlanNum"];
					}
					//Set all payment plans closed based on previous criteria.
					if(payPlanNums!="") {
						command="UPDATE payplan SET IsClosed=1 WHERE PayPlanNum IN ("+payPlanNums+")";
						Db.NonQ(command);
					}
				}
				//add a payplancharge credit for all current payment plans
				command="SELECT payplan.PayPlanNum,payplan.PatNum,payplan.Guarantor,payplan.PayPlanDate,payplan.PlanNum,payplan.CompletedAmt,"
					+"COALESCE(payplancharge.ProvNum,patient.PriProv) AS ProvNum,COALESCE(payplancharge.ClinicNum,patient.ClinicNum) AS ClinicNum "
					+"FROM payplan "
					+"LEFT JOIN payplancharge ON payplancharge.PayPlanNum=payplan.PayPlanNum "
					+"INNER JOIN patient on patient.PatNum=payplan.PatNum ";
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command+="GROUP BY payplan.PayPlanNum";
				}
				else { //oracle
					command+="GROUP BY payplan.PayPlanNum,payplan.PatNum,payplan.Guarantor,payplan.PayPlanDate,payplan.PlanNum,payplan.CompletedAmt"
						+",COALESCE(payplancharge.ProvNum,patient.PriProv)"
						+",COALESCE(payplancharge.ClinicNum,patient.ClinicNum)";
				}
				table=Db.GetTable(command);
				for(int i=0;i<table.Rows.Count;i++) {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO payplancharge (PayPlanNum,Guarantor,PatNum,ChargeDate,Principal,Note,ProvNum,ClinicNum,ChargeType) "
							+"VALUES ("
							+"'"+table.Rows[i]["PayPlanNum"].ToString()+"',"
							+"'"+table.Rows[i]["Guarantor"].ToString()+"',"
							+"'"+table.Rows[i]["PatNum"].ToString()+"',"
							+POut.Date(PIn.Date(table.Rows[i]["PayPlanDate"].ToString()))+","
							+"'"+table.Rows[i]["CompletedAmt"].ToString()+"',"
							+"'"+"Payment Plan Credit"+"',"
							+"'"+table.Rows[i]["ProvNum"].ToString()+"',"
							+"'"+table.Rows[i]["ClinicNum"].ToString()+"',"
							+"'1')";//Charge Type of Credit
					}
					else {//oracle
						command="INSERT INTO payplancharge (PayPlanChargeNum,PayPlanNum,Guarantor,PatNum,ChargeDate,Principal,Note,ProvNum,ClinicNum,ChargeType,ProcNum) "
							+"VALUES ("
							+"(SELECT COALESCE(MAX(PayPlanChargeNum),0)+1 FROM payplancharge)"+","
							+"'"+table.Rows[i]["PayPlanNum"].ToString()+"',"
							+"'"+table.Rows[i]["Guarantor"].ToString()+"',"
							+"'"+table.Rows[i]["PatNum"].ToString()+"',"
							+POut.Date(PIn.Date(table.Rows[i]["PayPlanDate"].ToString()))+","
							+"'"+table.Rows[i]["CompletedAmt"].ToString()+"',"
							+"'"+"Payment Plan Credit"+"',"
							+"'"+table.Rows[i]["ProvNum"].ToString()+"',"
							+"'"+table.Rows[i]["ClinicNum"].ToString()+"',"
							+"'1',"//Charge Type of Credit
							+"0)";
					}
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('PayPeriodIntervalSetting','0')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('PayPeriodPayAfterNumberOfDays','5')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('PayPeriodPayDateBeforeWeekend','1')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('PayPeriodPayDateExcludesWeekends','1')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('PayPeriodPayDay','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'PayPeriodIntervalSetting','0')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PayPeriodPayAfterNumberOfDays','5')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PayPeriodPayDateBeforeWeekend','1')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PayPeriodPayDateExcludesWeekends','1')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PayPeriodPayDay','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS orthocharttab";
					Db.NonQ(command);
					command=@"CREATE TABLE orthocharttab (
						OrthoChartTabNum bigint NOT NULL auto_increment PRIMARY KEY,
						TabName varchar(255) NOT NULL,
						ItemOrder int NOT NULL,
						IsHidden tinyint NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE orthocharttab'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE orthocharttab (
						OrthoChartTabNum number(20) NOT NULL,
						TabName varchar2(255),
						ItemOrder number(11) NOT NULL,
						IsHidden number(3) NOT NULL,
						CONSTRAINT orthocharttab_OrthoChartTabNum PRIMARY KEY (OrthoChartTabNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS orthocharttablink";
					Db.NonQ(command);
					command=@"CREATE TABLE orthocharttablink (
						OrthoChartTabLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
						ItemOrder int NOT NULL,
						OrthoChartTabNum bigint NOT NULL,
						DisplayFieldNum bigint NOT NULL,
						INDEX(OrthoChartTabNum),
						INDEX(DisplayFieldNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE orthocharttablink'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE orthocharttablink (
						OrthoChartTabLinkNum number(20) NOT NULL,
						ItemOrder number(11) NOT NULL,
						OrthoChartTabNum number(20) NOT NULL,
						DisplayFieldNum number(20) NOT NULL,
						CONSTRAINT orthocharttablink_TabLinkNum PRIMARY KEY (OrthoChartTabLinkNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX orthocharttablink_TabNum ON orthocharttablink (OrthoChartTabNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX orthocharttablink_DisplayField ON orthocharttablink (DisplayFieldNum)";
					Db.NonQ(command);
				}
				string tabNameDefault="Ortho Chart";
				if(!CultureInfo.CurrentCulture.Name.EndsWith("US")) {//Not United States
					command="SELECT Translation FROM languageforeign "
						+"WHERE ClassType='ContrChart' AND English='Ortho Chart' AND Culture='"+CultureInfo.CurrentCulture.Name+"'";
					DataTable tableTrans=Db.GetTable(command);
					if(tableTrans.Rows.Count > 0) {
						tabNameDefault=tableTrans.Rows[0][0].ToString();
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO orthocharttab(TabName,ItemOrder,IsHidden) VALUES ('"+POut.String(tabNameDefault)+"',0,0)";
				}
				else {//oracle
					command="INSERT INTO orthocharttab(OrthoChartTabNum,TabName,ItemOrder,IsHidden) "
						+"VALUES ((SELECT COALESCE(MAX(OrthoChartTabNum),0)+1 OrthoChartTabNum FROM orthocharttab),'"+POut.String(tabNameDefault)+"',0,0)";					
				}
				long orthoChartTabNum=Db.NonQ(command,true,"OrthoChartTabNum","orthocharttab");
				command="SELECT DisplayFieldNum FROM displayfield WHERE Category=8 ORDER BY ItemOrder";//Ortho Chart display fields
				table=Db.GetTable(command);
				//Move all existing display fields into the default ortho chart tab.
				for(int i=0;i<table.Rows.Count;i++) {
					long displayFieldNum=PIn.Long(table.Rows[i]["DisplayFieldNum"].ToString());
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO orthocharttablink (ItemOrder,OrthoChartTabNum,DisplayFieldNum) "
							+"VALUES ("+POut.Int(i)+","+POut.Long(orthoChartTabNum)+","+POut.Long(displayFieldNum)+")";
					}
					else {//oracle
						command="INSERT INTO orthocharttablink (OrthoChartTabLinkNum,ItemOrder,OrthoChartTabNum,DisplayFieldNum) "
							+"VALUES ((SELECT COALESCE(MAX(OrthoChartTabLinkNum),0)+1 FROM orthocharttablink),"
							+POut.Int(i)+","+POut.Long(orthoChartTabNum)+","+POut.Long(displayFieldNum)+")";
					}
					Db.NonQ(command);
				}
				command="SELECT ValueString from preference WHERE PrefName='ClearinghouseDefaultDent'";
				string value=Db.GetScalar(command);
				if(value=="0") {
					command="SELECT ValueString from preference WHERE PrefName='ClearinghouseDefaultMed'";
					value=Db.GetScalar(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
				   command="INSERT INTO preference(PrefName,ValueString) VALUES('ClearinghouseDefaultEligibility','"+value+"')";
				   Db.NonQ(command);
				}
				else {//oracle
				   command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ClearinghouseDefaultEligibility','"+value+"')";
				   Db.NonQ(command);
				}
				//Insert programproperty IsOnlinePaymentsEnabled for X-Charge per clinic to indicate whether to use for patient portal web payments
				command="SELECT ProgramNum FROM program WHERE ProgName='Xcharge'";
				long xChargeProgNum=PIn.Long(Db.GetScalar(command));
				command="SELECT DISTINCT ClinicNum FROM programproperty WHERE ProgramNum="+POut.Long(xChargeProgNum);
				List<long> listClinicNums=Db.GetListLong(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					foreach(long clinicNum in listClinicNums) {
						command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
							+"VALUES ("+POut.Long(xChargeProgNum)+",'IsOnlinePaymentsEnabled','0','',"+POut.Long(clinicNum)+")";
						Db.NonQ(command);
					}
				}
				else {//oracle
					foreach(long clinicNum in listClinicNums) {
						command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
							+"VALUES ((SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),"
							+POut.Long(xChargeProgNum)+",'IsOnlinePaymentsEnabled','0','',"+POut.Long(clinicNum)+")";
						Db.NonQ(command);
					}
				}
				//Insert programproperty IsOnlinePaymentsEnabled for PayConnect per clinic to indicate whether to use for patient portal web payments
				command="SELECT ProgramNum FROM program WHERE ProgName='PayConnect'";
				long payConnectProgNum=PIn.Long(Db.GetScalar(command));
				command="SELECT DISTINCT ClinicNum FROM programproperty WHERE ProgramNum="+POut.Long(payConnectProgNum);
				listClinicNums=Db.GetListLong(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					foreach(long clinicNum in listClinicNums) {
						command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
							+"VALUES ("+POut.Long(payConnectProgNum)+",'IsOnlinePaymentsEnabled','0','',"+POut.Long(clinicNum)+")";
						Db.NonQ(command);
					}
				}
				else {//oracle
					foreach(long clinicNum in listClinicNums) {
						command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
							+"VALUES ((SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),"
							+POut.Long(payConnectProgNum)+",'IsOnlinePaymentsEnabled','0','',"+POut.Long(clinicNum)+")";
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE site ADD Address varchar(100) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE site ADD Address varchar2(100)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE site ADD Address2 varchar(100) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE site ADD Address2 varchar2(100)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE site ADD City varchar(100) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE site ADD City varchar2(100)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE site ADD State varchar(100) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE site ADD State varchar2(100)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE site ADD Zip varchar(100) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE site ADD Zip varchar2(100)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE site ADD ProvNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE site ADD INDEX (ProvNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE site ADD ProvNum number(20)";
					Db.NonQ(command);
					command="UPDATE site SET ProvNum = 0 WHERE ProvNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE site MODIFY ProvNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX site_ProvNum ON site (ProvNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE site ADD PlaceService tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE site ADD PlaceService number(3)";
					Db.NonQ(command);
					command="UPDATE site SET PlaceService = 0 WHERE PlaceService IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE site MODIFY PlaceService NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE emailmessage ADD HideIn tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE emailmessage ADD HideIn number(3)";
					Db.NonQ(command);
					command="UPDATE emailmessage SET HideIn = 0 WHERE HideIn IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE emailmessage MODIFY HideIn NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE statement ADD StatementType varchar(50) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE statement ADD StatementType varchar2(50)";
					Db.NonQ(command);
				}
				//Default all statements in the db to NotSet.  This is for LimitedStatement, but in the future this will replace IsReceipt and IsInvoice.
				command="UPDATE statement SET StatementType='NotSet'";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS stmtadjattach";
					Db.NonQ(command);
					command=@"CREATE TABLE stmtadjattach (
						StmtAdjAttachNum bigint NOT NULL auto_increment PRIMARY KEY,
						StatementNum bigint NOT NULL,
						AdjNum bigint NOT NULL,
						INDEX(StatementNum,AdjNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE stmtadjattach'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE stmtadjattach (
						StmtAdjAttachNum number(20) NOT NULL,
						StatementNum number(20) NOT NULL,
						AdjNum number(20) NOT NULL,
						CONSTRAINT stmtadjattach_StmtAdjAttachNum PRIMARY KEY (StmtAdjAttachNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX stmtadjattach_StmtNumAdjNum ON stmtadjattach (StatementNum,AdjNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS stmtpaysplitattach";
					Db.NonQ(command);
					command=@"CREATE TABLE stmtpaysplitattach (
						StmtPaySplitAttachNum bigint NOT NULL auto_increment PRIMARY KEY,
						StatementNum bigint NOT NULL,
						PaySplitNum bigint NOT NULL,
						INDEX(StatementNum,PaySplitNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE stmtpaysplitattach'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE stmtpaysplitattach (
						StmtPaySplitAttachNum number(20) NOT NULL,
						StatementNum number(20) NOT NULL,
						PaySplitNum number(20) NOT NULL,
						CONSTRAINT stmtpaysplitattach_StmtPaySpli PRIMARY KEY (StmtPaySplitAttachNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX stmtpaysplitattach_SNumPSNum ON stmtpaysplitattach (StatementNum,PaySplitNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS stmtprocattach";
					Db.NonQ(command);
					command=@"CREATE TABLE stmtprocattach (
						StmtProcAttachNum bigint NOT NULL auto_increment PRIMARY KEY,
						StatementNum bigint NOT NULL,
						ProcNum bigint NOT NULL,
						INDEX(StatementNum,ProcNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE stmtprocattach'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE stmtprocattach (
						StmtProcAttachNum number(20) NOT NULL,
						StatementNum number(20) NOT NULL,
						ProcNum number(20) NOT NULL,
						CONSTRAINT stmtprocattach_StmtProcAttachN PRIMARY KEY (StmtProcAttachNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX stmtprocattach_SNumProcNum ON stmtprocattach (StatementNum,ProcNum)";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.2.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_2_7();
		}

		private static void To16_2_7() {
			if(FromVersion<new Version("16.2.7.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.2.7");//No translation in convert script.
				string command;
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("popup","PatNum")) {
							command="ALTER TABLE popup ADD INDEX (PatNum)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX popup_PatNum ON popup (PatNum)";
						Db.NonQ(command);
					}
				}
				catch(Exception) { }//Only an index.
				command="UPDATE preference SET ValueString = '16.2.7.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_2_10();
		}

		private static void To16_2_10() {
			if(FromVersion<new Version("16.2.10.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.2.10");//No translation in convert script.
				string command;
				command="SELECT MapAreaContainerNum FROM maparea";
				//Surround with a try catch because HQ manually added this column prior to upgrading to v16.2.10.
				//However, since this table is included for all customers, we need to make sure to add it for everyone.
				try {
					Db.NonQ(command);
				}
				catch(Exception) { //only run the convert script if MapAreaContainerNum does not exist.
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="ALTER TABLE maparea ADD MapAreaContainerNum bigint NOT NULL";
						Db.NonQ(command);
						command="ALTER TABLE maparea ADD INDEX (MapAreaContainerNum)";
						Db.NonQ(command);
					}
					else {//oracle
						command="ALTER TABLE maparea ADD MapAreaContainerNum number(20)";
						Db.NonQ(command);
						command="UPDATE maparea SET MapAreaContainerNum = 0 WHERE MapAreaContainerNum IS NULL";
						Db.NonQ(command);
						command="ALTER TABLE maparea MODIFY MapAreaContainerNum NOT NULL";
						Db.NonQ(command);
						command=@"CREATE INDEX maparea_MapAreaContainerNum ON maparea (MapAreaContainerNum)";
						Db.NonQ(command);
					}
					command="UPDATE maparea SET MapAreaContainerNum=1";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.2.10.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_2_11();
		}

		private static void To16_2_11() {
			if(FromVersion<new Version("16.2.11.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.2.11");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE screengroup ADD SheetDefNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE screengroup ADD INDEX (SheetDefNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE screengroup ADD SheetDefNum number(20)";
					Db.NonQ(command);
					command="UPDATE screengroup SET SheetDefNum = 0 WHERE SheetDefNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE screengroup MODIFY SheetDefNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX screengroup_SheetDefNum ON screengroup (SheetDefNum)";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.2.11.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_2_14();
		}

		private static void To16_2_14() {
			if(FromVersion<new Version("16.2.14.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.2.14");//No translation in convert script.
				string command;
				//add procedurelog.ClinicNum index to speed up selection and updating based on ClinicNum, for deleting a clinic
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("procedurelog","ClinicNum")) {
							command="ALTER TABLE procedurelog ADD INDEX (ClinicNum)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX procedurelog_ClinicNum ON procedurelog (ClinicNum)";
						Db.NonQ(command);
					}
				}
				catch(Exception) { }//Only an index.
				command="UPDATE preference SET ValueString = '16.2.14.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_2_19();
		}

		private static void To16_2_19() {
			if(FromVersion<new Version("16.2.19.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.2.19");//No translation in convert script.
				string command;
				//ADD URL preference for XCharge statment upload
				//Note, this is probably the wrong URL to use for uploading estatements, but it is the URL we have been using for years. It will now be user editable.
				//serverName="https://prelive.dentalxchange.com/dci/upload.svl";      //test URL for claims
				//serverName="https://claimconnect.dentalxchange.com/dci/upload.svl"; //live URL for claims
				//serverName="https://prelive.dentalxchange.com/dci/upload.svl";      //test URL for Stmts
				//serverName="https://billconnect.dentalxchange.com/dci/upload.svl";  //live URL for Stmts
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('BillingElectStmtUploadURL','https://claimconnect.dentalxchange.com/dci/upload.svl')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'BillingElectStmtUploadURL','https://claimconnect.dentalxchange.com/dci/upload.svl')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.2.19.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_2_20();
		}

		private static void To16_2_20() {
			if(FromVersion<new Version("16.2.20.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.2.20");//No translation in convert script.
				string command;
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("claimproc","indexAcctCov")) {
							//A covering index for the query that gets the procedures for the account module
							command="ALTER TABLE claimproc ADD INDEX indexAcctCov (ProcNum,PlanNum,Status,InsPayAmt,InsPayEst,WriteOff,NoBillIns)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX indexAcctCov ON claimproc (ProcNum,PlanNum,Status,InsPayAmt,InsPayEst,WriteOff,NoBillIns)";
						Db.NonQ(command);
					}
				}
				catch(Exception) {
					//Because we are using USE INDEX (indexAcctCov) in a query, we will fail the convert if the index cannot be added.
					throw new ApplicationException("Error adding indexAcctCov on table claimproc.");
				}
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("adjustment","indexPNAmt")) {
							command="ALTER TABLE adjustment ADD INDEX indexPNAmt (ProcNum,AdjAmt)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX adjustment_indexPNAmt ON adjustment (ProcNum,AdjAmt)";
						Db.NonQ(command);
					}
				}
				catch(Exception) { }//Only an index.
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("paysplit","indexPNAmt")) {
							command="ALTER TABLE paysplit ADD INDEX indexPNAmt (ProcNum,SplitAmt)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX paysplit_indexPNAmt ON paysplit (ProcNum,SplitAmt)";
						Db.NonQ(command);
					}
				}
				catch(Exception) { }//Only an index.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE automation ADD AppointmentTypeNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE automation ADD INDEX (AppointmentTypeNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE automation ADD AppointmentTypeNum number(20)";
					Db.NonQ(command);
					command="UPDATE automation SET AppointmentTypeNum = 0 WHERE AppointmentTypeNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE automation MODIFY AppointmentTypeNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX automation_AppointmentTypeNum ON automation (AppointmentTypeNum)";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.2.20.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_2_24();
		}

		private static void To16_2_24() {
			if(FromVersion<new Version("16.2.24.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.2.24");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					 command="INSERT INTO preference(PrefName,ValueString) VALUES('ShowProviderPayrollReport','0')";
					 Db.NonQ(command);
				}
				else {//oracle
					 command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ShowProviderPayrollReport','0')";
					 Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.2.24.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_2_26();
		}

		private static void To16_2_26() {
			if(FromVersion<new Version("16.2.26.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.2.26");//No translation in convert script.
				string command;
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("document","PatNum,DocCategory")) {
							command="ALTER TABLE document ADD INDEX PNDC(PatNum,DocCategory)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX document_PNDC ON document (PatNum,DocCategory)";
						Db.NonQ(command);
					}
				}
				catch(Exception) { }//Only an index.
				command="UPDATE preference SET ValueString = '16.2.26.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_2_29();
		}

		private static void To16_2_29() {
			if(FromVersion<new Version("16.2.29.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.2.29");//No translation in convert script.
				string command;
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("patient","HmPhone")) {
							command="ALTER TABLE patient ADD INDEX (HmPhone)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX patient_HmPhone ON patient (HmPhone)";
						Db.NonQ(command);
					}
				}
				catch(Exception) { }//Only an index
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("patient","WirelessPhone")) {
							command="ALTER TABLE patient ADD INDEX (WirelessPhone)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX patient_WirelessPhone ON patient (WirelessPhone)";
						Db.NonQ(command);
					}
				}
				catch(Exception) { }//Only an index.
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("patient","WkPhone")) {
							command="ALTER TABLE patient ADD INDEX (WkPhone)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX patient_WkPhone ON patient (WkPhone)";
						Db.NonQ(command);
					}
				}
				catch(Exception) { }//Only an index.
				command="UPDATE preference SET ValueString = '16.2.29.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_2_32();
		}

		private static void To16_2_32() {
			if(FromVersion<new Version("16.2.32.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.2.32");//No translation in convert script.
				string command;
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("insverify","VerifyType")) {
							command="ALTER TABLE insverify ADD INDEX (VerifyType)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX insverify_VerifyType ON insverify (VerifyType)";
						Db.NonQ(command);
					}
				}
				catch(Exception) { }//Only an index.
				command="UPDATE preference SET ValueString = '16.2.32.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_2_33();
		}

		private static void To16_2_33() {
			if(FromVersion<new Version("16.2.33.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.2.33");//No translation in convert script.
				string command;
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("adjustment","AdjDate")) {
							command="ALTER TABLE adjustment ADD INDEX(AdjDate)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command="CREATE INDEX adjustment_AdjDate ON adjustment (AdjDate)";
						Db.NonQ(command);
					}
				}
				catch(Exception) { }//Only an index.
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("claimpayment","CheckDate")) {
							command="ALTER TABLE claimpayment ADD INDEX (CheckDate)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX claimpayment_CheckDate ON claimpayment (CheckDate)";
						Db.NonQ(command);
					}
				}
				catch(Exception) { }//Only an index.
				command="UPDATE preference SET ValueString = '16.2.33.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_2_44();
		}

		private static void To16_2_44() {
			if(FromVersion<new Version("16.2.44.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.2.44");//No translation in convert script.
				string command;
				DataTable table=new DataTable();
				//set all payplan credits' Guarantors to be the same as their patnums so they show up in the correct account.
				command="UPDATE payplancharge	SET	payplancharge.Guarantor = payplancharge.PatNum WHERE payplancharge.ChargeType = 1"; //chargetype of credit
				Db.NonQ(command);
				//set all payplan debits' Guarantors to the correct patnum depending on whether the Guarantor is in the family or not.
				command="SELECT * FROM payplan"; //debits
				DataTable tablePayPlans=Db.GetTable(command);
				for(int i = 0;i < tablePayPlans.Rows.Count;i++) {
					ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.2.44 - Pay Plan "+(i+1)+" / "+tablePayPlans.Rows.Count);
					command="SELECT PatNum FROM patient WHERE patient.Guarantor = "+tablePayPlans.Rows[i]["Guarantor"];
					List<long> famPatNums=Db.GetListLong(command); //all patnums of the payplan's guarantor's family.
					if(famPatNums.Contains(PIn.Long(tablePayPlans.Rows[i]["PatNum"].ToString()))) {
						command="UPDATE payplancharge SET Guarantor = "+tablePayPlans.Rows[i]["PatNum"].ToString()
							+" WHERE payplancharge.PayPlanNum = "+tablePayPlans.Rows[i]["PayPlanNum"].ToString()+" AND payplancharge.ChargeType = 0"; //debits
						Db.NonQ(command);
						//All paysplits need to show up under this patients account regardless of who is associated to the payment.
						command="UPDATE paysplit SET PatNum = "+tablePayPlans.Rows[i]["PatNum"].ToString()
							+" WHERE paysplit.PayPlanNum = "+tablePayPlans.Rows[i]["PayPlanNum"].ToString(); //all paysplits for this payplan
						Db.NonQ(command);
					}
					else {
						command="UPDATE payplancharge SET Guarantor = "+tablePayPlans.Rows[i]["Guarantor"].ToString()
							+" WHERE payplancharge.PayPlanNum = "+tablePayPlans.Rows[i]["PayPlanNum"].ToString()+" AND payplancharge.ChargeType = 0"; //debits
						Db.NonQ(command);
						//All paysplits need to show up under the guarantor account regardless of who is associated to the payment.
						command="UPDATE paysplit SET PatNum = "+tablePayPlans.Rows[i]["Guarantor"].ToString()
							+" WHERE paysplit.PayPlanNum = "+tablePayPlans.Rows[i]["PayPlanNum"].ToString(); //all paysplits for this payplan
						Db.NonQ(command);
					}
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.2.44");//No translation in convert script.
				command="UPDATE preference SET ValueString = '16.2.44.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_2_56();
		}

		private static void To16_2_56() {
			if(FromVersion<new Version("16.2.56.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.2.56");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					 command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimMedProvTreatmentAsOrdering','1')";
					 Db.NonQ(command);
				}
				else {//oracle
					 command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ClaimMedProvTreatmentAsOrdering','1')";
					 Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.2.56.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_3_1();
		}

		private static void To16_3_1() {
			if(FromVersion<new Version("16.3.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.1");//No translation in convert script.
				string command;
				command="SELECT ProgramNum FROM program WHERE ProgName='Schick'";
				long schickProgramNum=PIn.Long(Db.GetScalar(command));
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(schickProgramNum)+"', "
						+"'Schick Version 4 or 5', "
						+"'5')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
						+"'"+POut.Long(schickProgramNum)+"', "
						+"'Schick Version 4 or 5', "
						+"'5', "
						+"'0')";
					Db.NonQ(command);
				}
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("periomeasure","PerioExamNum")) {
							command="ALTER TABLE periomeasure ADD INDEX (PerioExamNum)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX periomeasure_PerioExamNum ON periomeasure (PerioExamNum)";
						Db.NonQ(command);
					}
				}
				catch(Exception) { }//Only an index.
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("perioexam","PatNum")) {
							command="ALTER TABLE perioexam ADD INDEX (PatNum)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX perioexam_PatNum ON perioexam (PatNum)";
						Db.NonQ(command);
					}
				}
				catch(Exception) { }//Only an index.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE smsfrommobile ADD GuidMessage varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE smsfrommobile ADD GuidMessage varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE rxnorm MODIFY Description TEXT NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command=@"ALTER TABLE rxnorm MODIFY Description varchar2(4000)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD Abbr varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD Abbr varchar2(255)";
					Db.NonQ(command);
				}
				//Set each clinic's Abbr to be the clinic's Description by default
				command="UPDATE clinic SET Abbr=Description";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName, ValueString) VALUES ('UpdateDateTime','0001-01-01 00:00:00')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'UpdateDateTime','0001-01-01 00:00:00')";
					Db.NonQ(command);
				}
				bool hasTasksRepeating=false;
				command="SELECT COUNT(*) FROM task WHERE IsRepeating != 0";
				if(Db.GetCount(command)!="0") {
					hasTasksRepeating=true;
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('TasksUseRepeating','"+POut.Bool(hasTasksRepeating)+"')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'TasksUseRepeating','"+POut.Bool(hasTasksRepeating)+"')";
					Db.NonQ(command);
				}
				command="SELECT COUNT(*) FROM definition WHERE Category=33 AND ItemValue='R'";//33 for task priorities
				if(Db.GetCount(command)=="0") {//Definition for reminder priority does not exist yet.
					command="SELECT COALESCE(MAX(ItemOrder),0)+1 FROM definition WHERE Category=33";
					string maxOrder=Db.GetScalar(command);
					command="SELECT COALESCE(MAX(DefNum),0)+1 FROM definition";
					string reminderDefNum=Db.GetScalar(command);
					command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) "
						+"VALUES ("+reminderDefNum+",33,"+maxOrder+",'Reminder','R',-9031,'0')";//light orange
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE task ADD ReminderGroupId varchar(20) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE task ADD ReminderGroupId varchar2(20)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE task ADD ReminderType smallint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE task ADD ReminderType number(5)";
					Db.NonQ(command);
					command="UPDATE task SET ReminderType = 0 WHERE ReminderType IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE task MODIFY ReminderType NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE task ADD ReminderFrequency int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE task ADD ReminderFrequency number(11)";
					Db.NonQ(command);
					command="UPDATE task SET ReminderFrequency = 0 WHERE ReminderFrequency IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE task MODIFY ReminderFrequency NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE taskhist ADD ReminderGroupId varchar(20) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE taskhist ADD ReminderGroupId varchar2(20)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE taskhist ADD ReminderType smallint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE taskhist ADD ReminderType number(5)";
					Db.NonQ(command);
					command="UPDATE taskhist SET ReminderType = 0 WHERE ReminderType IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE taskhist MODIFY ReminderType NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE taskhist ADD ReminderFrequency int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE taskhist ADD ReminderFrequency number(11)";
					Db.NonQ(command);
					command="UPDATE taskhist SET ReminderFrequency = 0 WHERE ReminderFrequency IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE taskhist MODIFY ReminderFrequency NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PayPlansUseSheets','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PayPlansUseSheets','0')";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.1 - signals | Adding Def Links");
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sigbutdef ADD SigElementDefNumUser bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE sigbutdef ADD INDEX (SigElementDefNumUser)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sigbutdef ADD SigElementDefNumUser number(20)";
					Db.NonQ(command);
					command="UPDATE sigbutdef SET SigElementDefNumUser = 0 WHERE SigElementDefNumUser IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE sigbutdef MODIFY SigElementDefNumUser NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX sigbutdef_SigElementDefNumUser ON sigbutdef (SigElementDefNumUser)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sigbutdef ADD SigElementDefNumExtra bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE sigbutdef ADD INDEX (SigElementDefNumExtra)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sigbutdef ADD SigElementDefNumExtra number(20)";
					Db.NonQ(command);
					command="UPDATE sigbutdef SET SigElementDefNumExtra = 0 WHERE SigElementDefNumExtra IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE sigbutdef MODIFY SigElementDefNumExtra NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX sigbutdef_SigElementDefNumExtr ON sigbutdef (SigElementDefNumExtra)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE sigbutdef ADD SigElementDefNumMsg bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE sigbutdef ADD INDEX (SigElementDefNumMsg)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE sigbutdef ADD SigElementDefNumMsg number(20)";
					Db.NonQ(command);
					command="UPDATE sigbutdef SET SigElementDefNumMsg = 0 WHERE SigElementDefNumMsg IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE sigbutdef MODIFY SigElementDefNumMsg NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX sigbutdef_SigElementDefNumMsg ON sigbutdef (SigElementDefNumMsg)";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.1 - signals | Creating Message Table");
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS sigmessage";
					Db.NonQ(command);
					command=@"CREATE TABLE sigmessage (
						SigMessageNum bigint NOT NULL auto_increment PRIMARY KEY,
						ButtonText varchar(255) NOT NULL,
						ButtonIndex int NOT NULL,
						SynchIcon tinyint unsigned NOT NULL,
						FromUser varchar(255) NOT NULL,
						ToUser varchar(255) NOT NULL,
						MessageDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						AckDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						SigText varchar(255) NOT NULL,
						SigElementDefNumUser bigint NOT NULL,
						SigElementDefNumExtra bigint NOT NULL,
						SigElementDefNumMsg bigint NOT NULL,
						INDEX(SigElementDefNumUser),
						INDEX(SigElementDefNumExtra),
						INDEX(SigElementDefNumMsg)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE sigmessage'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE sigmessage (
						SigMessageNum number(20) NOT NULL,
						ButtonText varchar2(255),
						ButtonIndex number(11) NOT NULL,
						SynchIcon number(3) NOT NULL,
						FromUser varchar2(255),
						ToUser varchar2(255),
						MessageDateTime date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						AckDateTime date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						SigText varchar2(255),
						SigElementDefNumUser number(20) NOT NULL,
						SigElementDefNumExtra number(20) NOT NULL,
						SigElementDefNumMsg number(20) NOT NULL,
						CONSTRAINT sigmessage_SigMessageNum PRIMARY KEY (SigMessageNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX sigmessage_SigElementDefNumUse ON sigmessage (SigElementDefNumUser)";
					Db.NonQ(command);
					command=@"CREATE INDEX sigmessage_SigElementDefNumExt ON sigmessage (SigElementDefNumExtra)";
					Db.NonQ(command);
					command=@"CREATE INDEX sigmessage_SigElementDefNumMsg ON sigmessage (SigElementDefNumMsg)";
					Db.NonQ(command);
				}
				//Now that we have a table to convert signalods into we need to grab all the signals we care to convert.
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.1 - signals | Loading Messages");
				//Gets cleaned out roughly once a week so just do the whole thing?
				//Since we ONLY care about signals that are used with the messaging system, we can COMPLETELY ignore the ITypes column and do buttons only.
				command="SELECT * FROM signalod WHERE SigType=0";//0=Button.  Includes "text messages" (not to be confused with SMS).
				//SigType of 1 (the only other option) represents "Invalid" which is strictly used to let other workstations know about invalid cache.
				//Since we've kicked everyone out of Open Dental for this upgrade, we can ignore converting invalid types over to the new system.
				DataTable table=Db.GetTable(command);//Could take a long time...
				//Convert over the signalod table to the sigmessage table.
				int count=0;
				foreach(DataRow row in table.Rows) {
					count++;
					ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.1 - signals ("+count+"/"+table.Rows.Count+")");
					string sigElementDefNumUser="0";
					string sigElementDefNumExtra="0";
					string sigElementDefNumMsg="0";
					//We need to get all related sigelements in order to correctly bring over any message "text" that should show in the history.
					command="SELECT DISTINCT SigElementDefNum FROM sigelement WHERE SignalNum="+POut.String(row["SignalNum"].ToString());
					DataTable tableSigElements=Db.GetTable(command);
					foreach(DataRow rowSigElement in tableSigElements.Rows) {
						//Now we need to get all of the SigElementDefs related to this signal.
						command="SELECT SigElementType FROM sigelementdef "
							+"WHERE SigElementDefNum="+POut.String(rowSigElement["SigElementDefNum"].ToString());
						DataTable tableSigElementDef=Db.GetTable(command);
						if(tableSigElementDef.Rows.Count > 0) {
							switch(tableSigElementDef.Rows[0]["SigElementType"].ToString()) {
								case "0"://User
									sigElementDefNumUser=rowSigElement["SigElementDefNum"].ToString();
									break;
								case "1"://Extra
									sigElementDefNumExtra=rowSigElement["SigElementDefNum"].ToString();
									break;
								case "2"://Message
									sigElementDefNumMsg=rowSigElement["SigElementDefNum"].ToString();
									break;
							}
						}
						//SigText gets handled dynamically in regards to sigelementdefs.  Any SigText on the signalod will get handled below.
					}
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO sigmessage (FromUser,ToUser,MessageDateTime,AckDateTime,SigText"
								+",SigElementDefNumUser,SigElementDefNumExtra,SigElementDefNumMsg) "
							+"VALUES ('"+POut.String(row["FromUser"].ToString())+"'"
							+",'"+POut.String(row["ToUser"].ToString())+"'"
							+","+POut.DateT(PIn.Date(row["SigDateTime"].ToString()))
							+","+POut.DateT(PIn.Date(row["AckTime"].ToString()))
							+",'"+POut.String(row["SigText"].ToString())+"'"
							+","+POut.String(sigElementDefNumUser)
							+","+POut.String(sigElementDefNumExtra)
							+","+POut.String(sigElementDefNumMsg)+")";
						Db.NonQ(command);
					}
					else {
						command="INSERT INTO sigmessage (SigMessageNum,ButtonText,ButtonIndex,SynchIcon,FromUser,ToUser,MessageDateTime,AckDateTime,SigText"
								+",SigElementDefNumUser,SigElementDefNumExtra,SigElementDefNumMsg) "
							+"VALUES((SELECT COALESCE(MAX(SigMessageNum),0)+1 FROM sigmessage)"
							+",''"//ButtonText
							+",0"//ButtonIndex
							+",0"//SynchIcon
							+",'"+POut.String(row["FromUser"].ToString())+"'"
							+",'"+POut.String(row["ToUser"].ToString())+"'"
							+","+POut.DateT(PIn.Date(row["SigDateTime"].ToString()))
							+","+POut.DateT(PIn.Date(row["AckTime"].ToString()))
							+",'"+POut.String(row["SigText"].ToString())+"'"
							+","+POut.String(sigElementDefNumUser)
							+","+POut.String(sigElementDefNumExtra)
							+","+POut.String(sigElementDefNumMsg)+")";
						Db.NonQ(command);
					}
				}//end foreach message signal
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.1 - signals | Removing Old Messages");
				command="TRUNCATE TABLE signalod";//Truncate the signalod table because all the old data is now meaningless.
				Db.NonQ(command);
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.1 - signals | Altering Storage Method");
				//Instead of changing the datatype of the column which might not even be possible, we are going to simply drop the old and add the new.
				//Drop the old.
				command="ALTER TABLE signalod DROP COLUMN ITypes";
				Db.NonQ(command);
				//Add the new.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE signalod ADD IType tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE signalod ADD IType number(3)";
					Db.NonQ(command);
					command="UPDATE signalod SET IType = 0 WHERE IType IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE signalod MODIFY IType NOT NULL";
					Db.NonQ(command);
				}
				//Now to safely drop all the columns that are part of signalod which have been moved over to the sigmessage table.
				//DROP COLUMN statements are compatible with both MySQL and Oracle.
				command="ALTER TABLE signalod DROP COLUMN FromUser";
				Db.NonQ(command);
				command="ALTER TABLE signalod DROP COLUMN SigType";
				Db.NonQ(command);
				command="ALTER TABLE signalod DROP COLUMN SigText";
				Db.NonQ(command);
				command="ALTER TABLE signalod DROP COLUMN ToUser";
				Db.NonQ(command);
				command="ALTER TABLE signalod DROP COLUMN AckTime";
				Db.NonQ(command);
				command="ALTER TABLE signalod DROP COLUMN TaskNum";
				Db.NonQ(command);
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.1 - signals | Converting Buttons");
				//Convert all the sigbutdefelements over to the new 
				command=@"SELECT sigbutdef.SigButDefNum,sigelementdef.SigElementDefNum,sigelementdef.SigElementType FROM sigbutdefelement 
					LEFT JOIN sigbutdef ON sigbutdefelement.SigButDefNum=sigbutdef.SigButDefNum 
					LEFT JOIN sigelementdef ON sigbutdefelement.SigElementDefNum=sigelementdef.SigElementDefNum";
				table=Db.GetTable(command);
				//Loop through all sigbutdefelement entries and condense the potential multiple rows down into the one sigbutdef row (3 new columns).
				foreach(DataRow row in table.Rows) {
					//Validate that the data isn't null.
					if(string.IsNullOrEmpty(row["SigButDefNum"].ToString())
						|| string.IsNullOrEmpty(row["SigElementDefNum"].ToString())
						|| string.IsNullOrEmpty(row["SigElementType"].ToString())) 
					{
						continue;
					}
					command="UPDATE sigbutdef SET ";
					switch(row["SigElementType"].ToString()) {
						case "0"://User
							command+="SigElementDefNumUser=";
							break;
						case "1"://Extra
							command+="SigElementDefNumExtra=";
							break;
						case "2"://Message
							command+="SigElementDefNumMsg=";
							break;
					}
					command+=POut.String(row["SigElementDefNum"].ToString())+" "
						+"WHERE SigButDefNum="+POut.String(row["SigButDefNum"].ToString());
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.1 - signals | Cleaning Up");
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS sigelement";
					Db.NonQ(command);
					command="DROP TABLE IF EXISTS sigbutdefelement";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE sigelement'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE sigbutdefelement'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
				}
				#region FHIR
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.1 - FHIR");//No translation in convert script.
				long fhirProgNum;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,FilePath) "
						+"VALUES('FHIR','RESTful API following the HL7 FHIR standard',0,'')";
					fhirProgNum=Db.NonQ(command,true);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,FilePath) "
						+"VALUES((SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),'FHIR','RESTful API following the HL7 FHIR standard',0,'')";
					fhirProgNum=Db.NonQ(command,true,"ProgramNum","program");
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
							+"VALUES ("+POut.Long(fhirProgNum)+",'SubscriptionProcessingFrequency','','',0)";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
							+"VALUES ("+POut.Long(fhirProgNum)+",'SubscriptionsLastProcessed',NOW(),'',0)";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
							+"VALUES ((SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),"+POut.Long(fhirProgNum)+",'SubscriptionProcessingFrequency','',"
							+"'',0)";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
							+"VALUES ((SELECT MAX(ProgramPropertyNum)+1 FROM programproperty),"+POut.Long(fhirProgNum)+",'SubscriptionsLastProcessed',"
							+"SYSDATE,'',0)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS fhirsubscription";
					Db.NonQ(command);
					command=@"CREATE TABLE fhirsubscription (
						FHIRSubscriptionNum bigint NOT NULL auto_increment PRIMARY KEY,
						Criteria varchar(255) NOT NULL,
						Reason varchar(255) NOT NULL,
						SubStatus tinyint NOT NULL,
						ErrorNote text NOT NULL,
						ChannelType tinyint NOT NULL,
						ChannelEndpoint varchar(255) NOT NULL,
						ChannelPayLoad varchar(255) NOT NULL,
						ChannelHeader varchar(255) NOT NULL,
						DateEnd datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						APIKeyHash varchar(255) NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE fhirsubscription'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE fhirsubscription (
						FHIRSubscriptionNum number(20) NOT NULL,
						Criteria varchar2(255),
						Reason varchar2(255),
						SubStatus number(3) NOT NULL,
						ErrorNote clob,
						ChannelType number(3) NOT NULL,
						ChannelEndpoint varchar2(255),
						ChannelPayLoad varchar2(255),
						ChannelHeader varchar2(255),
						DateEnd date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						APIKeyHash varchar2(255),
						CONSTRAINT fhirsubscription_FHIRSubscript PRIMARY KEY (FHIRSubscriptionNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS fhircontactpoint";
					Db.NonQ(command);
					command=@"CREATE TABLE fhircontactpoint (
						FHIRContactPointNum bigint NOT NULL auto_increment PRIMARY KEY,
						FHIRSubscriptionNum bigint NOT NULL,
						ContactSystem tinyint NOT NULL,
						ContactValue varchar(255) NOT NULL,
						ContactUse tinyint NOT NULL,
						ItemOrder int NOT NULL,
						DateStart date NOT NULL DEFAULT '0001-01-01',
						DateEnd date NOT NULL DEFAULT '0001-01-01',
						INDEX(FHIRSubscriptionNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE fhircontactpoint'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE fhircontactpoint (
						FHIRContactPointNum number(20) NOT NULL,
						FHIRSubscriptionNum number(20) NOT NULL,
						ContactSystem number(3) NOT NULL,
						ContactValue varchar2(255),
						ContactUse number(3) NOT NULL,
						ItemOrder number(11) NOT NULL,
						DateStart date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateEnd date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT fhircontactpoint_FHIRContactPo PRIMARY KEY (FHIRContactPointNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX fhircontactpoint_FHIRSubscript ON fhircontactpoint (FHIRSubscriptionNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS appointmentdeleted";
					Db.NonQ(command);
					command=@"CREATE TABLE appointmentdeleted (
						AppointmentDeletedNum bigint NOT NULL auto_increment PRIMARY KEY,
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
						INDEX(AptNum),
						INDEX(PatNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE appointmentdeleted'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE appointmentdeleted (
						AppointmentDeletedNum number(20) NOT NULL,
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
						CONSTRAINT appointmentdeleted_Appointment PRIMARY KEY (AppointmentDeletedNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX appointmentdeleted_AptNum ON appointmentdeleted (AptNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX appointmentdeleted_PatNum ON appointmentdeleted (PatNum)";
					Db.NonQ(command);
				}
				#endregion FHIR
				//Put the progress bar back to the way it was OR the next developer can erase this comment and let the user know what they are doing.
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.1");//No translation in convert script.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					 command="INSERT INTO preference(PrefName,ValueString) VALUES('InsPpoAlwaysUseUcrFee','0')";
					 Db.NonQ(command);
				}
				else {//oracle
					 command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'InsPpoAlwaysUseUcrFee','0')";
					 Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE payplan ADD Signature text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE payplan ADD Signature varchar2(4000)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE payplan ADD SigIsTopaz tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE payplan ADD SigIsTopaz number(3)";
					Db.NonQ(command);
					command="UPDATE payplan SET SigIsTopaz = 0 WHERE SigIsTopaz IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE payplan MODIFY SigIsTopaz NOT NULL";
					Db.NonQ(command);
				}
				//Put the progress bar back to the way it was OR the next developer can erase this comment and let the user know what they are doing.
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.1");//No translation in convert script.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('ProcPromptForAutoNote','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ProcPromptForAutoNote','1')";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.1 - Aging | procedurelog Index");//No translation in convert script.
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("procedurelog","PatNum,ProcStatus,ProcFee,UnitQty,BaseUnits,ProcDate")) {
							command="ALTER TABLE procedurelog ADD INDEX indexAgingCovering (PatNum,ProcStatus,ProcFee,UnitQty,BaseUnits,ProcDate)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command="CREATE INDEX procedurelog_AgingCovering ON procedurelog (PatNum,ProcStatus,ProcFee,UnitQty,BaseUnits,ProcDate)";
						Db.NonQ(command);
					}
				}
				catch(Exception) { }//Only an index.
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.1 - Aging | claimproc Index");//No translation in convert script.
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("claimproc","Status,PatNum,DateCp,PayPlanNum,InsPayAmt,WriteOff,InsPayEst,ProcDate")) {
							command="ALTER TABLE claimproc ADD INDEX indexAgingCovering (Status,PatNum,DateCp,PayPlanNum,InsPayAmt,WriteOff,InsPayEst,ProcDate)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command="CREATE INDEX claimproc_AgingCovering ON claimproc (Status,PatNum,DateCp,PayPlanNum,InsPayAmt,WriteOff,InsPayEst,ProcDate)";
						Db.NonQ(command);
					}
				}
				catch(Exception) { }//Only an index.
				//Put the progress bar back to the way it was OR the next developer can erase this comment and let the user know what they are doing.
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.1");//No translation in convert script.
				//Add ApptConfirmStatusEdit permission to everyone
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				table=Db.GetTable(command);
				long groupNum;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					 foreach(DataRow row in table.Rows) {
							groupNum=PIn.Long(row["UserGroupNum"].ToString());
							command="INSERT INTO grouppermission (UserGroupNum,PermType) "
								 +"VALUES("+POut.Long(groupNum)+",120)";
							Db.NonQ(command);
					 }
				}
				else {//oracle
					 foreach(DataRow row in table.Rows) {
							groupNum=PIn.Long(row["UserGroupNum"].ToString());
							command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType) "
								 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),TO_DATE('0001-01-01','YYYY-MM-DD'),0,"+POut.Long(groupNum)+",120)";
							Db.NonQ(command);
					 }
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE repeatcharge ADD UsePrepay tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE repeatcharge ADD UsePrepay number(3)";
					Db.NonQ(command);
					command="UPDATE repeatcharge SET UsePrepay = 0 WHERE UsePrepay IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE repeatcharge MODIFY UsePrepay NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('InsTpChecksFrequency','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'InsTpChecksFrequency','1')";
					Db.NonQ(command);
				}
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.1");//No translation in convert script.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'Dropbox', "
						+"'Dropbox from www.dropbox.com', "
						+"'0', "
						+"'', "
						+"'', "//leave blank if none
						+"'')";
					long programNum=Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Dropbox AtoZ Path', "
						+"'/AtoZ')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'Dropbox API Token', "
						+"'')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'Dropbox', "
						+"'Dropbox from www.dropbox.com', "
						+"'0', "
						+"'', "
						+"'', "//leave blank if none
						+"'')";
					long programNum=Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Dropbox AtoZ Path', "
						+"'/AtoZ', "
						+"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'Dropbox API Token', "
						+"'', "
						+"'0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedNewPatApptProcs','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WebSchedNewPatApptProcs','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedNewPatApptTimePattern','/X/')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WebSchedNewPatApptTimePattern','/X/')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedNewPatApptSearchAfterDays','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'WebSchedNewPatApptSearchAfterDays','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE operatory ADD IsNewPatAppt tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE operatory ADD IsNewPatAppt number(3)";
					Db.NonQ(command);
					command="UPDATE operatory SET IsNewPatAppt = 0 WHERE IsNewPatAppt IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE operatory MODIFY IsNewPatAppt NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('AuditTrailEntriesDisplayed','500')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AuditTrailEntriesDisplayed','500')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('BillingElectCreatePDF','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'BillingElectCreatePDF','1')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('ProcCodeListSortOrder','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'ProcCodeListSortOrder','0')";
					Db.NonQ(command);
				}
        if(DataConnection.DBtype == DatabaseType.MySql) {
          command="ALTER TABLE computerpref ADD ComputerOS varchar(255) NOT NULL";
          Db.NonQ(command);
        }
        else {//oracle
          command="ALTER TABLE computerpref ADD ComputerOS varchar2(255)";
          Db.NonQ(command);
        }
        command="UPDATE computerpref SET ComputerOS = 'Undefined'";
        Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('ReportsShowHistory','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ReportsShowHistory','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE provider ADD IsHiddenReport tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE provider ADD IsHiddenReport number(3)";
					Db.NonQ(command);
					command="UPDATE provider SET IsHiddenReport = 0 WHERE IsHiddenReport IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE provider MODIFY IsHiddenReport NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE provider SET IsHiddenReport = 1 WHERE provider.IsHidden = 1";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE appointmenttype ADD Pattern varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE appointmenttype ADD Pattern varchar2(255)";
					Db.NonQ(command);
				}
				command="UPDATE appointmenttype SET Pattern=''";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE appointmenttype ADD CodeStr varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE appointmenttype ADD CodeStr varchar2(255)";
					Db.NonQ(command);
				}
				command="UPDATE appointmenttype SET CodeStr=''";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('AppointmentTypeShowPrompt','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AppointmentTypeShowPrompt','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('AppointmentTypeShowWarning','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AppointmentTypeShowWarning','1')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS userweb";
					Db.NonQ(command);
					command=@"CREATE TABLE userweb (
						UserWebNum bigint NOT NULL auto_increment PRIMARY KEY,
						FKey bigint NOT NULL,
						FKeyType tinyint NOT NULL,
						UserName varchar(255) NOT NULL,
						Password varchar(255) NOT NULL,
						PasswordResetCode varchar(255) NOT NULL,
						RequireUserNameChange tinyint NOT NULL,
						DateTimeLastLogin datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						INDEX(FKey)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE userweb'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE userweb (
						UserWebNum number(20) NOT NULL,
						FKey number(20) NOT NULL,
						FKeyType number(3) NOT NULL,
						UserName varchar2(255),
						Password varchar2(255),
						PasswordResetCode varchar2(255),
						RequireUserNameChange number(3) NOT NULL,
						DateTimeLastLogin date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						CONSTRAINT userweb_UserWebNum PRIMARY KEY (UserWebNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX userweb_FKey ON userweb (FKey)";
					Db.NonQ(command);
				}
				command="SELECT PatNum,FName,OnlinePassword FROM patient WHERE OnlinePassword!=''";
				DataTable dtPatPassword=Db.GetTable(command);
				foreach(DataRow row in dtPatPassword.Rows) {
					long patNumCur=PIn.Long(row["PatNum"].ToString());
					string fname=PIn.String(row["FName"].ToString());
					string passHashed=PIn.String(row["OnlinePassword"].ToString());
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO userweb (FKey,FKeyType,UserName,Password,RequireUserNameChange) "
							+"VALUES("+POut.Long(patNumCur)//FKey
							+",1"//UserWebFKeyType.PatientPortal
							+",'"+POut.String(fname)+POut.Long(patNumCur)+"'"//UserName
							+",'"+POut.String(passHashed)+"'"//Password
							+",'1')";//RequireUserNameChange
						Db.NonQ(command);
					}
					else {//oracle
						command="INSERT INTO userweb (UserWebNum,FKey,FKeyType,UserName,Password,PasswordResetCode,RequireUserNameChange,DateTimeLastLogin) "
							+"VALUES((SELECT COALESCE(MAX(UserWebNum),0)+1 FROM userweb)"//UserWebNum
							+","+POut.Long(patNumCur)//FKey
							+",1"//UserWebFKeyType.PatientPortal
							+",'"+POut.String(fname)+POut.Long(patNumCur)+"'"//UserName
							+",'"+POut.String(passHashed)+"'"//Password
							+",''"//PasswordResetCode
							+",1"//RequireUserNameChange
							+","+POut.DateT(DateTime.MinValue,true)+")";//DateTimeLastLogin
						Db.NonQ(command);
					}
				}
				command="ALTER TABLE patient DROP COLUMN OnlinePassword";//Oracle compatible
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claimpayment ADD PayGroup bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimpayment ADD INDEX (PayGroup)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claimpayment ADD PayGroup number(20)";
					Db.NonQ(command);
					command="UPDATE claimpayment SET PayGroup = 0 WHERE PayGroup IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimpayment MODIFY PayGroup NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX claimpayment_PayGroup ON claimpayment (PayGroup)";
					Db.NonQ(command);
				}
				//Insert the default claim payment group into the db.
				long insertID=0;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO definition (Category,ItemOrder,ItemName) VALUES(40,0,'Default')";
					insertID=Db.NonQ(command,true);
				}
				else {//oracle
					command="INSERT INTO definition(DefNum,Category,ItemOrder,ItemName) VALUES((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),40,0,'Default')";
					insertID=Db.NonQ(command,true,"DefNum","definition");
				}
				//Now set all of the current claimpayment groups to the default.
				command="UPDATE claimpayment SET PayGroup="+POut.Long(insertID);
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SuperFamNewPatAddIns','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SuperFamNewPatAddIns','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD MedLabAccountNum varchar(16) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD MedLabAccountNum varchar2(16)";
					Db.NonQ(command);
				}
				//Add Table ConfirmationRequest
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS confirmationrequest";
					Db.NonQ(command);
					command=@"CREATE TABLE confirmationrequest (
						ConfirmationRequestNum bigint NOT NULL auto_increment PRIMARY KEY,
						ClinicNum bigint NOT NULL,
						IsForSms tinyint NOT NULL,
						IsForEmail tinyint NOT NULL,
						PatNum bigint NOT NULL,
						ApptNum bigint NOT NULL,
						PhonePat varchar(255) NOT NULL,
						DateTimeConfirmExpire datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						SecondsFromEntryToExpire int NOT NULL,
						ShortGUID varchar(255) NOT NULL,
						ConfirmCode varchar(255) NOT NULL,
						MsgTextToMobileTemplate text NOT NULL,
						MsgTextToMobile text NOT NULL,
						EmailSubjTemplate text NOT NULL,
						EmailSubj text NOT NULL,
						EmailTextTemplate text NOT NULL,
						EmailText text NOT NULL,
						DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						DateTimeConfirmTransmit datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						DateTimeRSVP datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						RSVPStatus tinyint NOT NULL,
						ResponseDescript text NOT NULL,
						GuidMessageToMobile text NOT NULL,
						GuidMessageFromMobile text NOT NULL,
						INDEX(ClinicNum),
						INDEX(PatNum),
						INDEX(ApptNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE confirmationrequest'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE confirmationrequest (
						ConfirmationRequestNum number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						IsForSms number(3) NOT NULL,
						IsForEmail number(3) NOT NULL,
						PatNum number(20) NOT NULL,
						ApptNum number(20) NOT NULL,
						PhonePat varchar2(255),
						DateTimeConfirmExpire date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						SecondsFromEntryToExpire number(11) NOT NULL,
						ShortGUID varchar2(255),
						ConfirmCode varchar2(255),
						MsgTextToMobileTemplate clob,
						MsgTextToMobile clob,
						EmailSubjTemplate clob,
						EmailSubj clob,
						EmailTextTemplate clob,
						EmailText clob,
						DateTimeEntry date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateTimeConfirmTransmit date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateTimeRSVP date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						RSVPStatus number(3) NOT NULL,
						ResponseDescript clob,
						GuidMessageToMobile clob,
						GuidMessageFromMobile clob,
						CONSTRAINT confirmationrequest_Confirmati PRIMARY KEY (ConfirmationRequestNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX confirmationrequest_ClinicNum ON confirmationrequest (ClinicNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX confirmationrequest_PatNum ON confirmationrequest (PatNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX confirmationrequest_ApptNum ON confirmationrequest (ApptNum)";
					Db.NonQ(command);
				}
				//Add AuditTrail to groups with existing SecurityAdmin------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=24"; //SecurityAdmin
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					foreach(DataRow row in table.Rows) {
						groupNum=PIn.Long(row["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							 +"VALUES("+POut.Long(groupNum)+",122)"; //AuditTrail
						Db.NonQ(command);
					}
				}
				else {//oracle
					foreach(DataRow row in table.Rows) {
						groupNum=PIn.Long(row["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType) "
							 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),TO_DATE('0001-01-01','YYYY-MM-DD'),0,"+POut.Long(groupNum)+",122)";//AuditTrail
						Db.NonQ(command);
					}
				}
				//Treatment Plan Presenter Feature. By default, filled with the same usernum as SecUserNumEntry for the TreatPlan.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE treatplan ADD UserNumPresenter bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE treatplan ADD INDEX (UserNumPresenter)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE treatplan ADD UserNumPresenter number(20)";
					Db.NonQ(command);
					command="UPDATE treatplan SET UserNumPresenter = 0 WHERE UserNumPresenter IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE treatplan MODIFY UserNumPresenter NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX treatplan_UserNumPresenter ON treatplan (UserNumPresenter)";
					Db.NonQ(command);
				}
				//set defaults.
				command="UPDATE treatplan SET UserNumPresenter=SecUserNumEntry";//can be 0
				Db.NonQ(command);
				//TreatPlanPresenterEdit permission. 
				//Users with TreatPlanEdit(38) permission will be granted TreatPlanPresenterEdit(123) permission.
				command="SELECT UserGroupNum FROM grouppermission WHERE PermType=38"; //TreatPlanEdit
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					foreach(DataRow row in table.Rows) {
						groupNum=PIn.Long(row["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",123)";//123 - TreatPlanPresenterEdit
						Db.NonQ(command);
					}
				}
				else {//oracle
					foreach(DataRow row in table.Rows) {
						groupNum=PIn.Long(row["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType) "
							 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),TO_DATE('0001-01-01','YYYY-MM-DD'),0,"+POut.Long(groupNum)+",123)";//123 - TreatPlanPresenterEdit
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS apptcomm";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE apptcomm'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD IsConfirmEnabled tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD IsConfirmEnabled number(3)";
					Db.NonQ(command);
					command="UPDATE clinic SET IsConfirmEnabled = 0 WHERE IsConfirmEnabled IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clinic MODIFY IsConfirmEnabled NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD IsConfirmDefault tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD IsConfirmDefault number(3)";
					Db.NonQ(command);
					command="UPDATE clinic SET IsConfirmDefault = 0 WHERE IsConfirmDefault IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clinic MODIFY IsConfirmDefault NOT NULL";
					Db.NonQ(command);
				}
				//Add Table ApptReminderRule
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS apptreminderrule";
					Db.NonQ(command);
					command=@"CREATE TABLE apptreminderrule (
						ApptReminderRuleNum bigint NOT NULL auto_increment PRIMARY KEY,
						TypeCur tinyint NOT NULL,
						TSPrior bigint NOT NULL,
						SendOrder varchar(255) NOT NULL,
						IsSendAll tinyint NOT NULL,
						TemplateSMS text NOT NULL,
						TemplateEmailSubject text NOT NULL,
						TemplateEmail text NOT NULL,
						ClinicNum bigint NOT NULL,
						INDEX(TSPrior),
						INDEX(ClinicNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE apptreminderrule'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE apptreminderrule (
						ApptReminderRuleNum number(20) NOT NULL,
						TypeCur number(3) NOT NULL,
						TSPrior number(20) NOT NULL,
						SendOrder varchar2(255),
						IsSendAll number(3) NOT NULL,
						TemplateSMS clob,
						TemplateEmailSubject clob,
						TemplateEmail clob,
						ClinicNum number(20) NOT NULL,
						CONSTRAINT apptreminderrule_ApptReminderR PRIMARY KEY (ApptReminderRuleNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX apptreminderrule_TSPrior ON apptreminderrule (TSPrior)";
					Db.NonQ(command);
					command=@"CREATE INDEX apptreminderrule_ClinicNum ON apptreminderrule (ClinicNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS apptremindersent";
					Db.NonQ(command);
					command=@"CREATE TABLE apptremindersent (
						ApptReminderSentNum bigint NOT NULL auto_increment PRIMARY KEY,
						ApptNum bigint NOT NULL,
						ApptDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						DateTimeSent datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
						TSPrior bigint NOT NULL,
						ApptReminderRuleNum bigint NOT NULL,
						IsSmsSent tinyint NOT NULL,
						IsEmailSent tinyint NOT NULL,
						INDEX(ApptNum),
						INDEX(TSPrior),
						INDEX(ApptReminderRuleNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE apptremindersent'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE apptremindersent (
						ApptReminderSentNum number(20) NOT NULL,
						ApptNum number(20) NOT NULL,
						ApptDateTime date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						DateTimeSent date DEFAULT TO_DATE('0001-01-01','YYYY-MM-DD') NOT NULL,
						TSPrior number(20) NOT NULL,
						ApptReminderRuleNum number(20) NOT NULL,
						IsSmsSent number(3) NOT NULL,
						IsEmailSent number(3) NOT NULL,
						CONSTRAINT apptremindersent_ApptReminderS PRIMARY KEY (ApptReminderSentNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX apptremindersent_ApptNum ON apptremindersent (ApptNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX apptremindersent_TSPrior ON apptremindersent (TSPrior)";
					Db.NonQ(command);
					command=@"CREATE INDEX apptremindersent_ApptReminderR ON apptremindersent (ApptReminderRuleNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('ApptConfirmEnableForClinicZero','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptConfirmEnableForClinicZero','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('ApptConfirmAutoEnabled','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptConfirmAutoEnabled','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('ApptRemindAutoEnabled','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptRemindAutoEnabled','0')";
					Db.NonQ(command);
				}
				//Automated appointment Confirmation status, No default, requires user interaction
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('ApptEConfirmStatusSent','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'ApptEConfirmStatusSent','0')";
					Db.NonQ(command);
				}
				//Automated appointment Confirmation status, No default, requires user interaction
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('ApptEConfirmStatusAccepted','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'ApptEConfirmStatusAccepted','0')";
					Db.NonQ(command);
				}
				//Automated appointment Confirmation status, No default, requires user interaction
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('ApptEConfirmStatusDeclined','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'ApptEConfirmStatusDeclined','0')";
					Db.NonQ(command);
				}
				//Automated appointment Confirmation status, No default, requires user interaction
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('ApptEConfirmStatusSendFailed','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'ApptEConfirmStatusSendFailed','0')";
					Db.NonQ(command);
				}
				command="SELECT COUNT(*) FROM orthochart";
				string prefValue="0";
				if(Db.GetCount(command)!="0") {
					prefValue="1";
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('ApptModuleShowOrthoChartItem','"+prefValue+"')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptModuleShowOrthoChartItem','"+prefValue+"')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) { //21 is DefCat.MiscColors
					command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemColor) VALUES (21,'Family Module ICE',9,-70679)"; //Red
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO definition (DefNum,Category,ItemName,ItemOrder,ItemColor) VALUES ((SELECT MAX(DefNum)+1 FROM definition),21,'Family Module ICE',9,-70679)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('PaymentsPromptForPayType','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'PaymentsPromptForPayType','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE document ADD ExternalGUID varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE document ADD ExternalGUID varchar2(255)";
					Db.NonQ(command);
				}				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE document ADD ExternalSource varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE document ADD ExternalSource varchar2(255)";
					Db.NonQ(command);
				}
				//new standard report for treatment plan presenters
				command="SELECT COALESCE(MAX(ItemOrder),0)+1 FROM displayreport WHERE	Category = 2"; //monthly
				int itemOrd = PIn.Int(Db.GetScalar(command));
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODPresentedTreatmentProd',"+POut.Int(itemOrd)+",'Presented TreatPlan Production',2,0)";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) "
						+"VALUES ((SELECT COALESCE(MAX(DisplayReportNum), 0)+1 FROM displayreport),'ODPresentedTreatmentProd',"+POut.Int(itemOrd)+",'Presented TreatPlan Production',2,0)";
					Db.NonQ(command);
				}
				command="SELECT COALESCE(MAX(ItemOrder),0)+1 FROM displayreport WHERE	Category = 2"; //monthly
				itemOrd = PIn.Int(Db.GetScalar(command));
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) VALUES('ODTreatmentPresentationStats',"+POut.Int(itemOrd)+",'Treatment Presentation Statistics',2,0)";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) "
						+"VALUES ((SELECT COALESCE(MAX(DisplayReportNum), 0)+1 FROM displayreport),'ODTreatmentPresentationStats',"+POut.Int(itemOrd)+",'Treatment Presentation Statistics',2,0)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('AdjustmentsForceAttachToProc','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'AdjustmentsForceAttachToProc','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('BillingShowSendProgress','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'BillingShowSendProgress','1')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS patrestriction";
					Db.NonQ(command);
					command=@"CREATE TABLE patrestriction (
						PatRestrictionNum bigint NOT NULL auto_increment PRIMARY KEY,
						PatNum bigint NOT NULL,
						PatRestrictType tinyint NOT NULL,
						INDEX PatNumRestrictType (PatNum,PatRestrictType)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE patrestriction'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE patrestriction (
						PatRestrictionNum number(20) NOT NULL,
						PatNum number(20) NOT NULL,
						PatRestrictType number(3) NOT NULL,
						CONSTRAINT patrestriction_PatRestrictionN PRIMARY KEY (PatRestrictionNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX patrestriction_PatNumRestrictT ON patrestriction (PatNum,PatRestrictType)";
					Db.NonQ(command);
				}
				//Users with Providers(51) permission will be granted ProviderAlphabetize(124) permission.
				command="SELECT UserGroupNum FROM grouppermission WHERE PermType=51"; //Providers
				table=Db.GetTable(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					foreach(DataRow row in table.Rows) {
						groupNum=PIn.Long(row["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							+"VALUES("+POut.Long(groupNum)+",124)";//124 - ProviderAlphabetize
						Db.NonQ(command);
					}
				}
				else {//oracle
					foreach(DataRow row in table.Rows) {
						groupNum=PIn.Long(row["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType) "
							 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),TO_DATE('0001-01-01','YYYY-MM-DD'),0,"+POut.Long(groupNum)+",124)";//124 - ProviderAlphabetize
						Db.NonQ(command);
					}
				}
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("xwebresponse","DateTUpdate")) {
							command="ALTER TABLE xwebresponse ADD INDEX (DateTUpdate)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX xwebresponse_DateTUpdate ON xwebresponse (DateTUpdate)";
						Db.NonQ(command);
					}
				}
				catch(Exception) { }//Only an index.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) "
						+"VALUES('ODProviderPayrollSummary',6,'Provider Payroll Summary',0,1)";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) "
						+"VALUES ((SELECT COALESCE(MAX(DisplayReportNum),0)+1 FROM displayreport),'ODProviderPayrollSummary',6,'Provider Payroll Summary',0,1)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('ProviderPayrollAllowToday','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ProviderPayrollAllowToday','1')";
					Db.NonQ(command);
				}
				command="SELECT COALESCE(MAX(ItemOrder),0)+1 FROM definition WHERE Category=21";//21 is DefCat.MiscColors
				int itemOrder=PIn.Int(Db.GetScalar(command));
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemColor) "
						+"VALUES (21,'Family Module Pat Restrictions',"+itemOrder+",-70679)";//21 is DefCat.MiscColors
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO definition (DefNum,Category,ItemName,ItemOrder,ItemColor) "
						+"VALUES ((SELECT MAX(DefNum)+1 FROM definition),21,'Family Module Pat Restrictions',"+itemOrder+",-70679)";//21 is DefCat.MiscColors
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD IsNewPatApptExcluded tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD IsNewPatApptExcluded number(3)";
					Db.NonQ(command);
					command="UPDATE clinic SET IsNewPatApptExcluded = 0 WHERE IsNewPatApptExcluded IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clinic MODIFY IsNewPatApptExcluded NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('WebSchedNewPatApptEnabled','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'WebSchedNewPatApptEnabled','0')";
					Db.NonQ(command);
				}
				//alertitem table----------------------------------------------------------------------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS alertitem";
					Db.NonQ(command);
					command=@"CREATE TABLE alertitem (
						AlertItemNum bigint NOT NULL auto_increment PRIMARY KEY,
						ClinicNum bigint NOT NULL,
						Description varchar(255) NOT NULL,
						Type tinyint NOT NULL,
						Severity tinyint NOT NULL,
						Actions tinyint NOT NULL,
						INDEX(ClinicNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE alertitem'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE alertitem (
						AlertItemNum number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						Description varchar2(255),
						Type number(3) NOT NULL,
						Severity number(3) NOT NULL,
						Actions number(3) NOT NULL,
						CONSTRAINT alertitem_AlertItemNum PRIMARY KEY (AlertItemNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX alertitem_ClinicNum ON alertitem (ClinicNum)";
					Db.NonQ(command);
				}
				//alertread table----------------------------------------------------------------------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS alertread";
					Db.NonQ(command);
					command=@"CREATE TABLE alertread (
						AlertReadNum bigint NOT NULL auto_increment PRIMARY KEY,
						AlertItemNum bigint NOT NULL,
						UserNum bigint NOT NULL,
						INDEX(AlertItemNum),
						INDEX(UserNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE alertread'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE alertread (
						AlertReadNum number(20) NOT NULL,
						AlertItemNum number(20) NOT NULL,
						UserNum number(20) NOT NULL,
						CONSTRAINT alertread_AlertReadNum PRIMARY KEY (AlertReadNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX alertread_AlertItemNum ON alertread (AlertItemNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX alertread_UserNum ON alertread (UserNum)";
					Db.NonQ(command);
				}
				//alertsub table-----------------------------------------------------------------------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS alertsub";
					Db.NonQ(command);
					command=@"CREATE TABLE alertsub (
						AlertSubNum bigint NOT NULL auto_increment PRIMARY KEY,
						UserNum bigint NOT NULL,
						ClinicNum bigint NOT NULL,
						Type tinyint NOT NULL,
						INDEX(UserNum),
						INDEX(ClinicNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE alertsub'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE alertsub (
						AlertSubNum number(20) NOT NULL,
						UserNum number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						Type number(3) NOT NULL,
						CONSTRAINT alertsub_AlertSubNum PRIMARY KEY (AlertSubNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX alertsub_UserNum ON alertsub (UserNum)";
					Db.NonQ(command);
					command=@"CREATE INDEX alertsub_ClinicNum ON alertsub (ClinicNum)";
					Db.NonQ(command);
				}
				//Populate AlertSub table to encourage  usage.
				//No harm in subscribing for all clinics.
				//1 for OnlinePaymentsPending
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command=@"INSERT INTO alertsub (UserNum,ClinicNum,Type) 
										SELECT userod.UserNum,0,1 FROM userod 
										UNION ALL 
										SELECT userod.UserNum,clinic.ClinicNum,1 FROM userod INNER JOIN clinic";
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
							+"1)";//Type
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
								+"1)";//Type
							Db.NonQ(command);
						}
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) "
						+"VALUES('ODProviderPayrollDetailed',7,'Provider Payroll Detailed',0,1)";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) "
						+"VALUES ((SELECT COALESCE(MAX(DisplayReportNum),0)+1 FROM displayreport),'ODProviderPayrollDetailed',7,'Provider Payroll Detailed',0,1)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden) "
						+"VALUES('ODNetProdDetailDaily',6,'Net Production Detail Daily',1,1)";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO displayreport (DisplayReportNum,InternalName,ItemOrder,Description,Category,IsHidden) "
						+"VALUES ((SELECT COALESCE(MAX(DisplayReportNum),0)+1 FROM displayreport),'ODNetProdDetailDaily',6,'Net Production Detail Daily',1,1)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('NetProdDetailUseSnapshotToday','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'NetProdDetailUseSnapshotToday','1')";
					Db.NonQ(command);
				}
				command="SELECT ProgramNum FROM program WHERE ProgName='Podium'";
				long podiumProgramNum=PIn.Long(Db.GetScalar(command));
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(podiumProgramNum)+"', "
						+"'Show Commlogs In Chart', "
						+"'1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
						+"'"+POut.Long(podiumProgramNum)+"', "
						+"'Show Commlogs In Chart', "
						+"'1', "
						+"'0')";
					Db.NonQ(command);
				}
				//Item Order for ProcButtonItems
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procbuttonitem ADD ItemOrder bigint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procbuttonitem ADD ItemOrder number(20)";
					Db.NonQ(command);
					command="UPDATE procbuttonitem SET ItemOrder = 0 WHERE ItemOrder IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procbuttonitem MODIFY ItemOrder NOT NULL";
					Db.NonQ(command);
				}
				command="UPDATE procbuttonitem SET ItemOrder = procbuttonitemnum"; //Arbitrary ordering, but deterministic.
				Db.NonQ(command);
				//End ItemOrder for ProcButtonItems
				//Add ClaimProcReceivedEdit to groups with ClaimSentEdit permission------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=14";//ClaimSentEdit
				table=Db.GetTable(command);
				groupNum=0;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					 foreach(DataRow row in table.Rows) {
							groupNum=PIn.Long(row["UserGroupNum"].ToString());
							command="INSERT INTO grouppermission (UserGroupNum,PermType) "
								 +"VALUES("+POut.Long(groupNum)+",125)";//ClaimProcReceivedEdit
							Db.NonQ(command);
					 }
				}
				else {//oracle
					 foreach(DataRow row in table.Rows) {
							groupNum=PIn.Long(row["UserGroupNum"].ToString());
							command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
								 +"VALUES((SELECT COALESCE(MAX(GroupPermNum),0)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",125)";//ClaimProcReceivedEdit
						Db.NonQ(command);
					 }
				}
				//=================================Convert Appointment Reminder Hours interval to ApptreminderRule=================================
				command="SELECT ValueString FROM preference WHERE PrefName='ApptReminderHourInterval'";
				double apptReminderHourInterval=PIn.Double(Db.GetScalar(command));
				string TsHoursPriorTicks=POut.Long(TimeSpan.FromHours(apptReminderHourInterval).Ticks);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO apptreminderrule (TypeCur,TsPrior,SendOrder,IsSendAll,TemplateSMS,TemplateEmailSubject,TemplateEmail,ClinicNum) "+
						"SELECT 0 AS TypeCur, "+
						TsHoursPriorTicks+" AS TsPrior, "+
						"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderSendOrder') AS SendOrder, "+
						"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderSendAll') AS IsSendAll, "+
						"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderDayMessage') AS TemplateSMS, "+
						"'Appointment Reminder' AS TempalteEmailSubject, "+
						"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderEmailMessage') AS TemplateEmail, "+
						"0 AS ClinicNum ";//HQ
					if(apptReminderHourInterval>0) {
						command+="UNION ALL "+//No need to check if clinics are enabled
							"SELECT 0 AS TypeCur, "+
							TsHoursPriorTicks+" AS TsPrior, "+
							"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderSendOrder') AS SendOrder, "+
							"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderSendAll') AS IsSendAll, "+
							"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderDayMessage') AS TemplateSMS, "+
							"'Appointment Reminder' AS TempalteEmailSubject, "+
							"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderEmailMessage') AS TemplateEmail, "+
							"clinic.ClinicNum AS ClinicNum "+
							"FROM clinic";
					}
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO apptreminderrule (ApptReminderRuleNum,TypeCur,TsPrior,SendOrder,IsSendAll,TemplateSMS,TemplateEmailSubject,TemplateEmail,ClinicNum) "+
						"VALUES ("+
						"(SELECT COALESCE(MAX(ApptReminderRuleNum),0)+1 FROM apptreminderrule), "+
						"0, "+
						TsHoursPriorTicks+", "+
						"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderSendOrder'), "+
						"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderSendAll'), "+
						"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderDayMessage'), "+
						"'Appointment Reminder', "+
						"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderEmailMessage'), "+
						"0)";//HQ
					Db.NonQ(command);
					if(apptReminderHourInterval>0) {
						command="SELECT ClinicNum FROM clinic";
						table=Db.GetTable(command);
						foreach(DataRow row in table.Rows) {
							command="INSERT INTO apptreminderrule (ApptReminderRuleNum,TypeCur,TsPrior,SendOrder,IsSendAll,TemplateSMS,TemplateEmailSubject,TemplateEmail,ClinicNum) "+
								"VALUES ("+
								"(SELECT COALESCE(MAX(ApptReminderRuleNum),0)+1 FROM apptreminderrule), "+
								"0,"+
								TsHoursPriorTicks+", "+
								"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderSendOrder'), "+
								"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderSendAll'), "+
								"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderDayMessage'), "+
								"'Appointment Reminder', "+
								"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderEmailMessage'), "+
								row["ClinicNum"].ToString()+")";
							Db.NonQ(command);
						}
					}
				}
				//=================================Convert Appointment Reminder Days interval to ApptreminderRule=================================
				command="SELECT ValueString FROM preference WHERE PrefName='ApptReminderDayInterval'";
				double apptReminderDayInterval=PIn.Double(Db.GetScalar(command));
				string TsDaysPriorTicks=POut.Long(TimeSpan.FromDays(apptReminderDayInterval).Ticks);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO apptreminderrule (TypeCur,TsPrior,SendOrder,IsSendAll,TemplateSMS,TemplateEmailSubject,TemplateEmail,ClinicNum) "+
						"SELECT 0 AS TypeCur, "+
						TsDaysPriorTicks+" AS TsPrior, "+
						"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderSendOrder') AS SendOrder, "+
						"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderSendAll') AS IsSendAll, "+
						"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderDayMessage') AS TemplateSMS, "+
						"'Appointment Reminder' AS TempalteEmailSubject, "+
						"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderEmailMessage') AS TemplateEmail, "+
						"0 AS ClinicNum ";//HQ
					if(apptReminderDayInterval>0) {
						command+="UNION ALL "+//No need to check if clinics are enabled.
							"SELECT 0 AS TypeCur, "+
							TsDaysPriorTicks+" AS TsPrior, "+
							"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderSendOrder') AS SendOrder, "+
							"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderSendAll') AS IsSendAll, "+
							"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderDayMessage') AS TemplateSMS, "+
							"'Appointment Reminder' AS TempalteEmailSubject, "+
							"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderEmailMessage') AS TemplateEmail, "+
							"clinic.ClinicNum AS ClinicNum "+
							"FROM clinic";
					}
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO apptreminderrule (ApptReminderRuleNum,TypeCur,TsPrior,SendOrder,IsSendAll,TemplateSMS,TemplateEmailSubject,TemplateEmail,ClinicNum) "+
						"VALUES ("+
						"(SELECT COALESCE(MAX(ApptReminderRuleNum),0)+1 FROM apptreminderrule), "+
						"0, "+
						TsDaysPriorTicks+", "+
						"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderSendOrder'), "+
						"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderSendAll'), "+
						"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderDayMessage'), "+
						"'Appointment Reminder', "+
						"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderEmailMessage'), "+
						"0)";//HQ
					Db.NonQ(command);
					if(apptReminderDayInterval>0) {
						command="SELECT ClinicNum FROM clinic";
						table=Db.GetTable(command);
						foreach(DataRow row in table.Rows) {
							command="INSERT INTO apptreminderrule (ApptReminderRuleNum,TypeCur,TsPrior,SendOrder,IsSendAll,TemplateSMS,TemplateEmailSubject,TemplateEmail,ClinicNum) "+
								"VALUES ("+
								"(SELECT COALESCE(MAX(ApptReminderRuleNum),0)+1 FROM apptreminderrule), "+
								"0,"+
								TsDaysPriorTicks+", "+
								"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderSendOrder'), "+
								"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderSendAll'), "+
								"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderDayMessage'), "+
								"'Appointment Reminder', "+
								"(SELECT ValueString FROM preference WHERE PrefName = 'ApptReminderEmailMessage'), "+
								row["ClinicNum"].ToString()+")";
							Db.NonQ(command);
						}
					}
				}
				//RE-Enable reminders if they used to use the old system.
				if(apptReminderDayInterval>0 || apptReminderHourInterval>0) {
					command="UPDATE preference SET ValueString='1' WHERE PrefName='ApptConfirmEnableForClinicZero'";
					Db.NonQ(command);
					command="UPDATE preference SET ValueString='1' WHERE PrefName='ApptRemindAutoEnabled'";
					Db.NonQ(command);
					command="UPDATE clinic SET IsConfirmDefault=1, IsConfirmEnabled=1";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptSecondaryProviderConsiderOpOnly','0')";
					Db.NonQ(command);
				}
				else {//Oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ApptSecondaryProviderConsiderOpOnly','0')";
					Db.NonQ(command);
				}
				//Add new preference TaskSortApptDateTime.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('TaskSortApptDateTime','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
						+"'TaskSortApptDateTime','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ReportsIncompleteProcsNoNotes','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ReportsIncompleteProcsNoNotes','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ReportsIncompleteProcsUnsigned','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ReportsIncompleteProcsUnsigned','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('ProcEditRequireAutoCodes','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ProcEditRequireAutoCodes','0')";
					Db.NonQ(command);
				}
				//Treatment Plan Sorting Preference
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('TreatPlanSortByTooth','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'TreatPlanSortByTooth','1')";
					Db.NonQ(command);
				}
				command="SELECT COALESCE(MAX(CASE WHEN ItemName='econfirmsent' THEN DefNum ELSE 0 END),0) eConfirmSent,"
					+"COALESCE(MAX(CASE WHEN ItemName='econfirmcallback' THEN DefNum ELSE 0 END),0) eConfirmCallBack,"
					+"COALESCE(MAX(CASE WHEN ItemName='econfirmfailure' THEN DefNum ELSE 0 END),0) eConfirmFailure,"
					+"COALESCE(MAX(CASE WHEN ItemName='appointmentconfirmed' THEN DefNum ELSE 0 END),0) AppointmentConfirmed,"
					+"COALESCE(MAX(ItemOrder),0)+1 itemOrder "
					+"FROM definition "
					+"WHERE Category=2";//2=ApptConfirmed
				table=Db.GetTable(command);
				long eConfirmSentDefNum=PIn.Long(table.Rows[0]["eConfirmSent"].ToString());
				long eConfirmCallBackDefNum=PIn.Long(table.Rows[0]["eConfirmCallBack"].ToString());
				long eConfirmFailureDefNum=PIn.Long(table.Rows[0]["eConfirmFailure"].ToString());
				long eApptConfirmedDefNum=PIn.Long(table.Rows[0]["AppointmentConfirmed"].ToString());
				itemOrder=PIn.Int(table.Rows[0]["itemOrder"].ToString());
				if(eApptConfirmedDefNum==0) {
					command="SELECT MAX(COALESCE(DefNum,0)) AppointmentConfirmed "
						+"FROM definition "
						+"WHERE ItemName LIKE '%confirm%' AND ItemName NOT LIKE '%unconfirm%' "
						+"AND ItemName NOT IN ('econfirmsent','econfirmcallback','econfirmfailure','appointmentconfirmed') "
						+"AND Category=2";//2=ApptConfirmed
					eApptConfirmedDefNum=PIn.Long(Db.GetScalar(command));//can be 0; allowed.
				}
				#region Insert eConfirmSent DefNum and Set ApptEConfirmStatusSent Pref
				if(eConfirmSentDefNum==0) {//Insert new def.
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES (2,"+POut.Int(itemOrder++)
							+",'eConfirmSent','eConfirmSent')";
					}
					else {//oracle
						command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum from definition),2,"
							+POut.Long(itemOrder++)+",'eConfirmSent','eConfirmSent')";
					}
					eConfirmSentDefNum=Db.NonQ(command,true,"DefNum","definition");
				}
				Db.NonQ("UPDATE preference SET ValueString="+POut.Long(eConfirmSentDefNum)+" WHERE PrefName='ApptEConfirmStatusSent'");
				#endregion Insert eConfirmSent DefNum and Set ApptEConfirmStatusSent Pref
				#region Insert eConfirmCallBack DefNum and Set ApptEConfirmStatusDeclined Pref
				if(eConfirmCallBackDefNum==0) {//Insert new def.
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES (2,"+POut.Int(itemOrder++)
							+",'eConfirmCallBack','eConfirmCallBack')";
					}
					else {//oracle
						command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum from definition),2,"
							+POut.Long(itemOrder++)+",'eConfirmCallBack','eConfirmCallBack')";
					}
					eConfirmCallBackDefNum=Db.NonQ(command,true,"DefNum","definition");
				}
				Db.NonQ("UPDATE preference SET ValueString = "+POut.Long(eConfirmCallBackDefNum)+" WHERE PrefName = 'ApptEConfirmStatusDeclined'");
				#endregion Insert eConfirmCallBack DefNum and Set ApptEConfirmStatusDeclined Pref
				#region Insert eConfirmFailure DefNum and Set ApptEConfirmStatusSendFailed Pref
				if(eConfirmFailureDefNum==0) {//Insert new def.
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES (2,"+POut.Int(itemOrder++)
							+",'eConfirmFailure','eConfirmFailure')";
					}
					else {//oracle
						command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum from definition),2,"
							+POut.Long(itemOrder++)+",'eConfirmFailure','eConfirmFailure')";
					}
					eConfirmFailureDefNum=Db.NonQ(command,true,"DefNum","definition");
				}
				Db.NonQ("UPDATE preference SET ValueString = "+POut.Long(eConfirmFailureDefNum)+" WHERE PrefName = 'ApptEConfirmStatusSendFailed'");
				#endregion Insert eConfirmFailure DefNum and Set ApptEConfirmStatusSendFailed Pref
				#region Insert eApptConfirmed DefNum and Set ApptEConfirmStatusAccepted Pref
				if(eApptConfirmedDefNum==0) {//Insert new def.
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue) VALUES (2,"+POut.Int(itemOrder++)
							+",'AppointmentConfirmed','AppointmentConfirmed')";
					}
					else {//oracle
						command="INSERT INTO definition (DefNum,Category,ItemOrder,ItemName,ItemValue) VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum from definition),2,"
							+POut.Long(itemOrder++)+",'AppointmentConfirmed','AppointmentConfirmed')";
					}
					eApptConfirmedDefNum=Db.NonQ(command,true,"DefNum","definition");
				}
				Db.NonQ("UPDATE preference SET ValueString="+POut.Long(eApptConfirmedDefNum)+" WHERE PrefName='ApptEConfirmStatusAccepted'");
				#endregion Insert eApptConfirmed DefNum and Set ApptEConfirmStatusAccepted Pref
				command="UPDATE preference SET ValueString = '16.3.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_3_4();
		}
		
		private static void To16_3_4() {
			if(FromVersion<new Version("16.3.4.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.4.0");//No translation in convert script.
				string command;
				//==============================ICE Fields==============================
				//This field was introduced in 16.3.1 and was re-engineered in 16.3.4 A very short lived field, 
				//not expected to have very much data. This script will only apply to about a dozen customers 
				//that actually used version 16.3.1.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patientnote ADD ICEName varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patientnote ADD ICEName varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patientnote ADD ICEPhone varchar(30) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patientnote ADD ICEPhone varchar2(30)";
					Db.NonQ(command);
				}
				command="SELECT patnum FROM patientnote";
				List<long> listPatNoteNums = Db.GetListLong(command);
				command="SELECT * FROM patfield WHERE FieldName='In Case of Emergency(ICE)'";
				DataTable tablePatFields = Db.GetTable(command);
				foreach(DataRow row in tablePatFields.Select()) {
					long patNum = PIn.Long(row["PatNum"].ToString());
					string[] ICEValues = row["FieldValue"].ToString().Split(new[] { "\r\n" },StringSplitOptions.None);
					if(ICEValues.Length<3) {
						Array.Resize<string>(ref ICEValues,3);//should never happen.
					}
					string ICEName = ICEValues[0];
					if(!string.IsNullOrWhiteSpace(ICEValues[2])) {
						ICEName+="; "+ICEValues[2];//add note onto end of name field.
					}
					string ICEPhone = ICEValues[1];
					if(listPatNoteNums.Contains(patNum)) {
						command="UPDATE patientnote SET ICEName='"+POut.String(ICEName)+"', ICEPhone='"+POut.String(ICEPhone)+"' WHERE PatNum="+POut.Long(patNum);
					}
					else {
						command="INSERT INTO patientnote (PatNum,FamFinancial,ApptPhone,Medical,Service,MedicalComp,Treatment,ICEName,ICEPhone) "
							+"VALUES ("+POut.Long(patNum)+",'','','','','','','"+POut.String(ICEName)+"','"+POut.String(ICEPhone)+"')";//blank values because the columns are nullable in DB
					}
					Db.NonQ(command);
				}//end foreach
				command="DELETE FROM patfield WHERE FieldName='In Case of Emergency(ICE)'";
				Db.NonQ(command);
				//==============================ICE Fields END==========================
				command="UPDATE preference SET ValueString='16.3.4.0' WHERE PrefName='DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_3_14();
		}

		private static void To16_3_14() {
			if(FromVersion<new Version("16.3.14.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.14");//No translation in convert script.
				string command;
				command="SELECT COUNT(*) FROM preference WHERE PrefName='ClaimMedProvTreatmentAsOrdering'";
				if(Db.GetCount(command)=="0") {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						 command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimMedProvTreatmentAsOrdering','1')";
						 Db.NonQ(command);
					}
					else {//oracle
						 command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
							+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ClaimMedProvTreatmentAsOrdering','1')";
						 Db.NonQ(command);
					}
				}
				command="UPDATE preference SET ValueString = '16.3.14.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_3_19();
		}

		private static void To16_3_19() {
			if(FromVersion<new Version("16.3.19.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.19");//No translation in convert script.
				string command;
				//Moving codes to the Obsolete category that were deleted in CDT 2017.
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
						"D0290",//Deleted in 2017
						"D0260","D0421","D2970","D9220","D9221","D9241","D9242","D9931"//Deleted in 2016. A bug in our D-Codes Tools caused them to be added after
																																					 //they were supposed to be deleted.
					};
					//Change the procedure codes' category to Obsolete.
					command="UPDATE procedurecode SET ProcCat="+POut.Long(defNum)
						+" WHERE ProcCode IN('"+string.Join("','",cdtCodesDeleted.Select(x => POut.String(x)))+"') ";
					Db.NonQ(command);
				}//end United States CDT codes update
				command="UPDATE preference SET ValueString = '16.3.19.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_3_20();
		}

		private static void To16_3_20() {
			if(FromVersion<new Version("16.3.20.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.20");//No translation in convert script.
				string command;
				long prefValueString=PIn.Long(Db.GetScalar("SELECT ValueString FROM preference WHERE PrefName='ClaimReportReceiveInterval'"));
				if(prefValueString==5) {//Update to new default.
					command="UPDATE preference SET ValueString='30' WHERE PrefName='ClaimReportReceiveInterval'";
					Db.NonQ(command);
				}
				else if(prefValueString<5) {
					command="UPDATE preference SET ValueString='5' WHERE PrefName='ClaimReportReceiveInterval'";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimReportReceiveLastDateTime','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) "
					+"VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ClaimReportReceiveLastDateTime','')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.3.20.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_3_22();
		}

		private static void To16_3_22() {
			if(FromVersion<new Version("16.3.22.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.22");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claim ADD OrderingReferralNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim ADD INDEX (OrderingReferralNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claim ADD OrderingReferralNum number(20)";
					Db.NonQ(command);
					command="UPDATE claim SET OrderingReferralNum = 0 WHERE OrderingReferralNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claim MODIFY OrderingReferralNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX claim_OrderingReferralNum ON claim (OrderingReferralNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE procedurelog ADD OrderingReferralNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog ADD INDEX (OrderingReferralNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE procedurelog ADD OrderingReferralNum number(20)";
					Db.NonQ(command);
					command="UPDATE procedurelog SET OrderingReferralNum = 0 WHERE OrderingReferralNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE procedurelog MODIFY OrderingReferralNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX procedurelog_OrderingReferralN ON procedurelog (OrderingReferralNum)";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.3.22.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_3_25();
		}

		private static void To16_3_25() {
			if(FromVersion<new Version("16.3.25.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.25");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS famaging";
					Db.NonQ(command);
					command=@"CREATE TABLE famaging (
						PatNum bigint NOT NULL auto_increment PRIMARY KEY,
						Bal_0_30 double NOT NULL,
						Bal_31_60 double NOT NULL,
						Bal_61_90 double NOT NULL,
						BalOver90 double NOT NULL,
						InsEst double NOT NULL,
						BalTotal double NOT NULL,
						PayPlanDue double NOT NULL
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE famaging'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE famaging (
						PatNum number(20) NOT NULL,
						Bal_0_30 number(38,8) NOT NULL,
						Bal_31_60 number(38,8) NOT NULL,
						Bal_61_90 number(38,8) NOT NULL,
						BalOver90 number(38,8) NOT NULL,
						InsEst number(38,8) NOT NULL,
						BalTotal number(38,8) NOT NULL,
						PayPlanDue number(38,8) NOT NULL,
						CONSTRAINT famaging_PatNum PRIMARY KEY (PatNum)
						)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('AgingIsEnterprise','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'AgingIsEnterprise','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('AgingBeginDateTime','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'AgingBeginDateTime','')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.3.25.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_3_29();
		}

		private static void To16_3_29() {
			if(FromVersion<new Version("16.3.29.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.29");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ScheduleProvEmpSelectAll','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ScheduleProvEmpSelectAll','1')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('BillingElectStmtOutputPathPos','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'BillingElectStmtOutputPathPos','')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('BillingElectStmtOutputPathClaimX','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'BillingElectStmtOutputPathClaimX','')";
					Db.NonQ(command);
				}
				string edsOutputPath=@"C:\\EDS\\STATEMENTS";
				if(Db.GetScalar("SELECT ValueString FROM preference WHERE PrefName='BillingUseElectronic'")=="4") {
					edsOutputPath=Db.GetScalar("SELECT ValueString FROM preference WHERE PrefName='BillingElectStmtUploadURL'");
					Db.NonQ("UPDATE preference SET ValueString='https://claimconnect.dentalxchange.com/dci/upload.svl' WHERE PrefName='BillingElectStmtUploadURL'");
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('BillingElectStmtOutputPathEds','"+edsOutputPath+"')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'BillingElectStmtOutputPathEds','"+edsOutputPath+"')";
					Db.NonQ(command);
				}
				#region Enhance apptreminderrule for aggregate sending and convert eCR rules from multiple ReminderSameDay to 1 ReminderSameDay and 1 ReminderFutureDay				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE apptreminderrule ADD TemplateSMSAggShared text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE apptreminderrule ADD TemplateSMSAggShared clob";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE apptreminderrule ADD TemplateSMSAggPerAppt text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE apptreminderrule ADD TemplateSMSAggPerAppt clob";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE apptreminderrule ADD TemplateEmailSubjAggShared text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE apptreminderrule ADD TemplateEmailSubjAggShared clob";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE apptreminderrule ADD TemplateEmailAggShared text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE apptreminderrule ADD TemplateEmailAggShared clob";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE apptreminderrule ADD TemplateEmailAggPerAppt text NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE apptreminderrule ADD TemplateEmailAggPerAppt clob";
					Db.NonQ(command);
				}
				//All db commands are Oracle compatible.
				command=@"UPDATE apptreminderrule SET 
					TemplateSMSAggShared='Appointment Reminder:\n[Appts]\nIf you have questions call[ClinicPhone].',
					TemplateSMSAggPerAppt='[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName].',
					TemplateEmailSubjAggShared='Appointment Reminder',
					TemplateEmailAggShared='Appointment Reminder:\r\n[Appts]\r\nIf you have questions call [ClinicPhone].',
					TemplateEmailAggPerAppt='[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName].'
					WHERE TypeCur=0";
				Db.NonQ(command);
				command=@"UPDATE apptreminderrule SET 
					TemplateSMSAggShared='Appointment Confirmation:\n[Appts]\nIf you have questions call [ClinicPhone].',
					TemplateSMSAggPerAppt='[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName].',
					TemplateEmailSubjAggShared='Appointment Confirmation',
					TemplateEmailAggShared='Appointment Confirmation:\r\n[Appts]\r\nGoto [ConfirmURL] for confirmation options, or call [ClinicPhone].',
					TemplateEmailAggPerAppt='[NameF] is scheduled for [ApptTime] on [ApptDate] at [ClinicName].'
					WHERE TypeCur=1";
				Db.NonQ(command);
				command="SELECT ApptReminderRuleNum, TypeCur, TSPrior, ClinicNum FROM apptreminderrule WHERE TypeCur=0";
				var listARRs = Db.GetTable(command).Select().Select(x => new {
					ApptReminderRuleNum = PIn.Long(x[0].ToString()),
					TypeCur = PIn.Int(x[1].ToString()),
					TSPrior = TimeSpan.FromTicks(PIn.Long(x[2].ToString())),
					ClinicNum = PIn.Long(x[3].ToString())
				}).GroupBy(x => x.ClinicNum)
				.ToDictionary(x => x.Key,x => x.ToList());
				foreach(var kvp in listARRs) {
					//No Rules fo Clinic
					if(kvp.Value.Count==0) {
						continue;
					}
					//Single reminder for clinic (Simple)
					if(kvp.Value.Count==1) {
						if(kvp.Value[0].TSPrior.TotalDays>=1) {
							int days = (int)Math.Round(kvp.Value[0].TSPrior.TotalDays);
							command="UPDATE apptreminderrule SET TypeCur=2, TSPrior="+POut.Long(TimeSpan.FromDays(days).Ticks)
								+" WHERE ApptReminderRuleNum="+POut.Long(kvp.Value[0].ApptReminderRuleNum);
						}
						else {
							int hours = (int)Math.Round(kvp.Value[0].TSPrior.TotalHours);
							if(hours==0&&kvp.Value[0].TSPrior!=TimeSpan.Zero) {
								//this only happens if they have a rule setup for less than 30 minutes and greater than zero; because of rounding.
								hours=1;
							}
							command="UPDATE apptreminderrule SET TSPrior="+POut.Long(TimeSpan.FromHours(hours).Ticks)
								+" WHERE ApptReminderRuleNum="+POut.Long(kvp.Value[0].ApptReminderRuleNum);
						}
						Db.NonQ(command);
					}
					//At least 2 rules for this clinic.
					if(kvp.Value.Count>1) {
						var listCur = kvp.Value.OrderBy(x => x.TSPrior).ToList();
						int hours = (int)Math.Round(listCur[0].TSPrior.TotalHours);
						if(hours==0&&kvp.Value[0].TSPrior!=TimeSpan.Zero) {
							//this only happens if they have a rule setup for less than 30 minutes and greater than zero; because of rounding.
							hours=1;
						}
						if(hours>=24) {
							hours=1;
						}
						command="UPDATE apptreminderrule SET TSPrior="+POut.Long(TimeSpan.FromHours(hours).Ticks)
							+" WHERE ApptReminderRuleNum="+POut.Long(listCur[0].ApptReminderRuleNum);
						Db.NonQ(command);
						int days = (int)Math.Round(listCur[1].TSPrior.TotalDays);
						if(days==0&&listCur[1].TSPrior!=TimeSpan.Zero) {
							//this only happens if they have a rule setup for less than 30 minutes and greater than zero; because of rounding.
							days=1;
						}
						command="UPDATE apptreminderrule SET TypeCur=2, TSPrior="+POut.Long(TimeSpan.FromDays(days).Ticks)
							+" WHERE ApptReminderRuleNum="+POut.Long(listCur[1].ApptReminderRuleNum);
						Db.NonQ(command);
					}
				}
				#endregion				
				//The default value of IsErxEnabled is false for new providers.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE provider ADD IsErxEnabled tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE provider ADD IsErxEnabled number(3)";
					Db.NonQ(command);
					command="UPDATE provider SET IsErxEnabled = 0 WHERE IsErxEnabled IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE provider MODIFY IsErxEnabled NOT NULL";
					Db.NonQ(command);
				}
				//For existing providers, the value of IsErxEnabled is true if the eRx bridge is enabled and a user has clicked through to eRx at least once.
				bool isErxEnabled=true;
				command="SELECT Enabled FROM program WHERE ProgName='eRx'";
				if(Db.GetScalar(command)=="0") {//The eRx bridge is currently disabled.
					isErxEnabled=false;
				}
				else {//The eRx bridge is currently enabled.
					command="SELECT ValueString FROM preference WHERE PrefName='NewCropAccountId'";
					if(Db.GetScalar(command)=="") {//Nobody has clicked through to eRx.
						isErxEnabled=false;
					}
				}
				if(isErxEnabled) {
					command="UPDATE provider SET IsErxEnabled=1";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.3.29.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_3_32();
		}
	
		private static void To16_3_32() {
			if(FromVersion<new Version("16.3.32.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.29");//No translation in convert script.
				string command;
				command="SELECT ProgramNum FROM program WHERE ProgName='Owandy'";
				long owandyProgramNum=PIn.Long(Db.GetScalar(command));
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(owandyProgramNum)+"', "
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
						+"'"+POut.Long(owandyProgramNum)+"', "
						+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
						+"'5', "
						+"'0')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.3.32.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_3_34();
		}

		private static void To16_3_34() {
			if(FromVersion<new Version("16.3.34.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.34");//No translation in convert script.
				string command;				
				command=@"UPDATE apptreminderrule SET 
					TemplateSMSAggShared='Appointment Confirmation:\n[Appts]\nGoto [ConfirmURL] for confirmation options, or call [ClinicPhone].'
					WHERE TypeCur=1";
				Db.NonQ(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE confirmationrequest ADD ShortGuidEmail varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE confirmationrequest ADD ShortGuidEmail varchar2(255)";
					Db.NonQ(command);
				}
				//Add ApptConfirmExcludeEConfirm preference ------------------------------------------------------------------
				long sentNum = Db.GetLong("SELECT ValueString FROM preference WHERE PrefName='ApptEConfirmStatusSent'");
				long acceptedNum = Db.GetLong("SELECT ValueString FROM preference WHERE PrefName='ApptEConfirmStatusAccepted'");
				long declinedNum = Db.GetLong("SELECT ValueString FROM preference WHERE PrefName='ApptEConfirmStatusDeclined'");
				long failedNum = Db.GetLong("SELECT ValueString FROM preference WHERE PrefName='ApptEConfirmStatusSendFailed'");
				long[] excludeSendNums = new[] { sentNum,acceptedNum,declinedNum,failedNum };
				long[] excludeConfirmNums = new[] { acceptedNum,declinedNum,failedNum };
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptConfirmExcludeEConfirm','"+string.Join(",",excludeConfirmNums)+"')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
						+"'ApptConfirmExcludeEConfirm','"+string.Join(",",excludeConfirmNums)+"')";
					Db.NonQ(command);
				}
				//Add ApptConfirmExcludeESend preference ---------------------------------------------------------------------
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptConfirmExcludeESend','"+string.Join(",",excludeSendNums)+"')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
						+"'ApptConfirmExcludeESend','"+string.Join(",",excludeSendNums)+"')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.3.34.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_3_45();
		}

		private static void To16_3_45() {
			if(FromVersion<new Version("16.3.45.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.3.45");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('QuickBooksClassRefsEnabled','0')";//off by default
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'QuickBooksClassRefsEnabled','0')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.3.45.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_4_1();
		}

		private static void To16_4_1() {
			if(FromVersion<new Version("16.4.1.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.4.1");//No translation in convert script.
				string command;
				command="SELECT MIN(DefNum) FROM definition WHERE Category='1' AND ItemName='Sales Tax' AND ItemValue='+'";
				long defNum=PIn.Long(Db.GetScalar(command));
				if(defNum==0) {//Sales Tax adjustment definition doesn't exist.  Create one for them.
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO definition (Category,ItemName,ItemValue) VALUES ('1','Sales Tax','+')";
						defNum=Db.NonQ(command,true);
					}
					else {//oracle
						command="INSERT INTO definition (DefNum,Category,ItemName,ItemValue) VALUES ((SELECT COALESCE(MAX(DefNum),0)+1 DefNum FROM definition),'1','Sales Tax','+')";
						defNum=Db.NonQ(command,true,"DefNum","definition");
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('SalesTaxAdjustmentType','"+defNum+"')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'SalesTaxAdjustmentType','"+defNum+"')";
					Db.NonQ(command);
				}
				//Allow claimprocs to be created for backdated completed procedures preference
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('ClaimProcsAllowedToBackdate','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ClaimProcsAllowedToBackdate','0')";
					Db.NonQ(command);
				}
				//Remove all the \r\n characters from the wikipages so it works with the new ODcodeBox control.
				command="UPDATE wikipage SET PageContent = REPLACE(PageContent, '\r\n', '\n')";
				Db.NonQ(command);
				command="UPDATE wikipagehist SET PageContent = REPLACE(PageContent, '\r\n', '\n')";
				Db.NonQ(command);
				//FieldDefLink feature for OrthoChart (and other areas)
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS fielddeflink";
					Db.NonQ(command);
					command=@"CREATE TABLE fielddeflink (
						FieldDefLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
						FieldDefNum bigint NOT NULL,
						FieldDefType tinyint NOT NULL,
						FieldLocation tinyint NOT NULL,
						INDEX(FieldDefNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE fielddeflink'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE fielddeflink (
						FieldDefLinkNum number(20) NOT NULL,
						FieldDefNum number(20) NOT NULL,
						FieldDefType number(3) NOT NULL,
						FieldLocation number(3) NOT NULL,
						CONSTRAINT fielddeflink_FieldDefLinkNum PRIMARY KEY (FieldDefLinkNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX fielddeflink_FieldDefNum ON fielddeflink (FieldDefNum)";
					Db.NonQ(command);
				}
				//Require special character when PasswordsMustBeStrong is enabled.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					 command="INSERT INTO preference(PrefName,ValueString) VALUES('PasswordsStrongIncludeSpecial','0')";
					 Db.NonQ(command);
				}
				else {//oracle
					 command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'PasswordsStrongIncludeSpecial','0')";
					 Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE xwebresponse ADD CCSource tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE xwebresponse ADD CCSource number(3)";
					Db.NonQ(command);
					command="UPDATE xwebresponse SET CCSource = 0 WHERE CCSource IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE xwebresponse MODIFY CCSource NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE clinic ADD IsHidden tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE clinic ADD IsHidden number(3)";
					Db.NonQ(command);
					command="UPDATE clinic SET IsHidden = 0 WHERE IsHidden IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE clinic MODIFY IsHidden NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('ApptsCheckFrequency','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'ApptsCheckFrequency','0')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE recalltype ADD AppendToSpecial tinyint NOT NULL";
					Db.NonQ(command);
					//Set the "AppendToSpecial" boolean to true for all recall types by default.  This preserves old behavior.
					command="UPDATE recalltype SET AppendToSpecial = 1";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE recalltype ADD AppendToSpecial number(3)";
					Db.NonQ(command);
					//Set the "AppendToSpecial" boolean to true for all recall types by default.  This preserves old behavior.
					command="UPDATE recalltype SET AppendToSpecial = 1";
					Db.NonQ(command);
					command="ALTER TABLE recalltype MODIFY AppendToSpecial NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('PayPlansExcludePastActivity','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'PayPlansExcludePastActivity','0')";
					Db.NonQ(command);
				}
				//Add MobileWeb permission to everyone------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				DataTable table=Db.GetTable(command);
				long groupNum;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					foreach(DataRow row in table.Rows) {
						groupNum=PIn.Long(row["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (UserGroupNum,PermType) "
							 +"VALUES("+POut.Long(groupNum)+",127)";//MobileWeb 
						Db.NonQ(command);
					}
				}
				else {//oracle
					foreach(DataRow row in table.Rows) {
						groupNum=PIn.Long(row["UserGroupNum"].ToString());
						command="INSERT INTO grouppermission (GroupPermNum,NewerDays,UserGroupNum,PermType) "
							 +"VALUES((SELECT MAX(GroupPermNum)+1 FROM grouppermission),0,"+POut.Long(groupNum)+",127)";//MobileWeb
						Db.NonQ(command);
					}
				}
				//Saving new tasks as draft on force close of OD
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE task ADD IsDraft tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE task ADD IsDraft number(3)";
					Db.NonQ(command);
					command="UPDATE task SET IsDraft = 0 WHERE IsDraft IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE task MODIFY IsDraft NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE taskhist ADD IsDraft tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE taskhist ADD IsDraft number(3)";
					Db.NonQ(command);
					command="UPDATE taskhist SET IsDraft = 0 WHERE IsDraft IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE taskhist MODIFY IsDraft NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE tasknote ADD IsDraft tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE tasknote ADD IsDraft number(3)";
					Db.NonQ(command);
					command="UPDATE tasknote SET IsDraft = 0 WHERE IsDraft IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE tasknote MODIFY IsDraft NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE terminalactive ADD SessionId int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE terminalactive ADD SessionId number(11)";
					Db.NonQ(command);
					command="UPDATE terminalactive SET SessionId = 0 WHERE SessionId IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE terminalactive MODIFY SessionId NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE terminalactive ADD ProcessId int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE terminalactive ADD ProcessId number(11)";
					Db.NonQ(command);
					command="UPDATE terminalactive SET ProcessId = 0 WHERE ProcessId IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE terminalactive MODIFY ProcessId NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE terminalactive ADD SessionName varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE terminalactive ADD SessionName varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptModuleProductionUsesOps','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),'ApptModuleProductionUsesOps','0')";
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE perioexam ADD DateTMeasureEdit datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE perioexam ADD DateTMeasureEdit date";
					Db.NonQ(command);
					command="UPDATE perioexam SET DateTMeasureEdit = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateTMeasureEdit IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE perioexam MODIFY DateTMeasureEdit NOT NULL";
					Db.NonQ(command);
				}
				//Insert programproperty TerminalProcessingEnabled for PayConnect for each clinic to indicate whether terminal processing is allowed
				command="SELECT ProgramNum FROM program WHERE ProgName='PayConnect'";
				long progNum=PIn.Long(Db.GetScalar(command));
				command="SELECT ClinicNum FROM clinic";
				List<long> listClinicNums=Db.GetListLong(command);
				listClinicNums.Add(0);//Include a property for headquarters.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					foreach(long clinicNum in listClinicNums) {
						command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
							+"VALUES ("+POut.Long(progNum)+",'TerminalProcessingEnabled','0','',"+POut.Long(clinicNum)+")";
						Db.NonQ(command);
					}
				}
				else {//oracle
					foreach(long clinicNum in listClinicNums) {
						command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
							+"VALUES ((SELECT COALESCE(MAX(ProgramPropertyNum),0)+1 FROM programproperty),"
							+POut.Long(progNum)+",'TerminalProcessingEnabled','0','',"+POut.Long(clinicNum)+")";
						Db.NonQ(command);
					}
				}
				command="UPDATE preference SET PrefName='InsChecksFrequency' WHERE PrefName='InsTpChecksFrequency'";
				Db.NonQ(command);			
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrpatient ADD SexualOrientation varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ehrpatient ADD SexualOrientation varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrpatient ADD GenderIdentity varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ehrpatient ADD GenderIdentity varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrpatient ADD SexualOrientationNote varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ehrpatient ADD SexualOrientationNote varchar2(255)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE ehrpatient ADD GenderIdentityNote varchar(255) NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE ehrpatient ADD GenderIdentityNote varchar2(255)";
					Db.NonQ(command);
				}
				//Make sure the CdcrecCode is set for each patientrace entry.
				command="UPDATE patientrace SET CdcrecCode='2076-8' WHERE Race=0";//Aboriginal to NATIVE HAWAIIAN OR OTHER PACIFIC ISLANDER
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='2054-5' WHERE Race=1";//AfricanAmerican
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='1002-5' WHERE Race=2";//AmericanIndian
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='2028-9' WHERE Race=3";//Asian
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='ASKU-RACE' WHERE Race=4";//DeclinedToSpecifyRace
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='2076-8' WHERE Race=5";//HawaiiOrPacIsland
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='2135-2' WHERE Race=6";//Hispanic
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='MULTI-RACE' WHERE Race=7";//Multiracial 
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='2131-1' WHERE Race=8";//Other
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='2106-3' WHERE Race=9";//White
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='2186-5' WHERE Race=10";//NotHispanic
				Db.NonQ(command);
				command="UPDATE patientrace SET CdcrecCode='ASKU-ETHNICITY' WHERE Race=11";//DeclinedToSpecifyEthnicity				
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptsRequireProc','1')";//True by default per Nathan.
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
						+"'ApptsRequireProc','1')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"'SFTP', "
						+"'', "
						+"'0', "//Disabled
						+"'"+POut.String(@"")+"', "
						+"'', "//leave blank if none
						+"'')";
					long programNum = Db.NonQ(command,true);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'FTP User Name', "
						+"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'FTP Password', "
						+"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'FTP Hostname', "
						+"'')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
						+") VALUES("
						+"'"+POut.Long(programNum)+"', "
						+"'FTP AtoZ Path', "
						+"'AtoZ/')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO program (ProgramNum,ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
						+") VALUES("
						+"(SELECT COALESCE(MAX(ProgramNum),0)+1 ProgramNum FROM program),"
						+"'SFTP', "
						+"'', "
						+"'0', "//Disabled
						+"'"+POut.String(@"")+"', "
						+"'', "//leave blank if none
						+"'')";
					long programNum = Db.NonQ(command,true,"ProgramNum","program");
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'FTP User Name', "
						+"'', "
						+"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'FTP Password', "
						+"'', "
						+"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'FTP Hostname', "
						+"'', "
						+"'0')";
					Db.NonQ(command);
					command="INSERT INTO programproperty (ProgramPropertyNum,ProgramNum,PropertyDesc,PropertyValue,ClinicNum"
						+") VALUES("
						+"(SELECT MAX(ProgramPropertyNum+1) FROM programproperty),"
						+"'"+POut.Long(programNum)+"', "
						+"'FTP AtoZ Path', "
						+"'AtoZ/', "
						+"'0')";
					Db.NonQ(command);
				}
				command="SELECT ValueString FROM preference WHERE PrefName = 'ClaimSnapshotTriggerType'";
				if(Db.GetScalar(command).ToLower()=="econnector") {
					command="UPDATE preference SET ValueString = 'Service' WHERE PrefName = 'ClaimSnapshotTriggerType'";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					 command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptAllowFutureComplete','0')";
					 Db.NonQ(command);
				}
				else {//oracle
					 command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptAllowFutureComplete','0')";
					 Db.NonQ(command);
				}
				//Add PatPriProvEdit (PermType=129) permission to everyone
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				List<long> listLongs=Db.GetListLong(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					foreach(long userGroupNum in listLongs) {
						command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES ("+POut.Long(userGroupNum)+",129)";
						Db.NonQ(command);
					}
				}
				else {//oracle
					foreach(long userGroupNum in listLongs) {
						command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType) "
							+"VALUES ((SELECT MAX(GroupPermNum)+1 FROM grouppermission),TO_DATE('0001-01-01','YYYY-MM-DD'),0,"+POut.Long(userGroupNum)+",129)";
						Db.NonQ(command);
					}
				}
				#region Ortho Features
				#region Insert Ortho Auto Proc
				//If everything goes well, a new D8670.auto procedurecode will be inserted into the database.
				//If the customer is using Oracle or any of the if-checks fail, then a procedurecode won't get inserted into the DB.
				//This just means that the user will have to manually create a new procedurecode if they want to have a distinct Auto Ortho Proc
				//and link that procedure to the preference in the Ortho Setup window.
				long orthoAutoProcCodeNum=0;
				command="SELECT procedurecode.ProcCat FROM procedurecode WHERE procedurecode.ProcCode='D8670'";
				long procCatNum=PIn.Long(Db.GetScalar(command));
				if(DataConnection.DBtype==DatabaseType.MySql && procCatNum > 0) {
					command="SELECT procedurecode.CodeNum FROM procedurecode WHERE procedurecode.ProcCode='D8670.auto'";
					orthoAutoProcCodeNum=PIn.Long(Db.GetScalar(command));
					//Automatically create a D8670.auto code if one does not already exist.
					if(orthoAutoProcCodeNum==0) {
						command="INSERT INTO procedurecode (ProcCode, Descript, AbbrDesc, ProcTime, ProcCat, DateTStamp) "
							+"VALUES ('D8670.auto', 'Default auto ortho procedure code', 'AutoOrthoProc', '/',"+POut.Long(procCatNum)+","+Now()+")";
						orthoAutoProcCodeNum=Db.NonQ(command,true);
					}
				}
				#endregion
				#region preferences
				//OrthoAutoProcCodeNum
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('OrthoAutoProcCodeNum',"+POut.Long(orthoAutoProcCodeNum)+")";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'OrthoAutoProcCodeNum',"+POut.Long(orthoAutoProcCodeNum)+")";
					Db.NonQ(command);
				}
				//OrthoCaseInfoInOrthoChart
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('OrthoCaseInfoInOrthoChart','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'OrthoCaseInfoInOrthoChart','0')";
					Db.NonQ(command);
				}
				//OrthoClaimMarkAsOrtho
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('OrthoClaimMarkAsOrtho','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'OrthoClaimMarkAsOrtho','0')";
					Db.NonQ(command);
				}
				//OrthoClaimUseDatePlacement
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('OrthoClaimUseDatePlacement','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'OrthoClaimUseDatePlacement','0')";
					Db.NonQ(command);
				}
				//OrthoDefaultMonthsTreat
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('OrthoDefaultMonthsTreat','24')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'OrthoDefaultMonthsTreat','24')";
					Db.NonQ(command);
				}
				//OrthoEnabled
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('OrthoEnabled','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'OrthoEnabled','0')";
					Db.NonQ(command);
				}
				//OrthoInsPayConsolidated
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('OrthoInsPayConsolidated','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'OrthoInsPayConsolidated','0')";
					Db.NonQ(command);
				}
				#endregion
				#region insplan
				//insplan.OrthoType
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE insplan ADD OrthoType tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE insplan ADD OrthoType number(3)";
					Db.NonQ(command);
					command="UPDATE insplan SET OrthoType = 0 WHERE OrthoType IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE insplan MODIFY OrthoType NOT NULL";
					Db.NonQ(command);
				}
				//insplan.OrthoAutoProcFreq
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE insplan ADD OrthoAutoProcFreq tinyint NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE insplan ADD OrthoAutoProcFreq number(3)";
					Db.NonQ(command);
					command="UPDATE insplan SET OrthoAutoProcFreq = 0 WHERE OrthoAutoProcFreq IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE insplan MODIFY OrthoAutoProcFreq NOT NULL";
					Db.NonQ(command);
				}
				//insplan.OrthoAutoProcCodeNumOverride
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE insplan ADD OrthoAutoProcCodeNumOverride bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE insplan ADD INDEX (OrthoAutoProcCodeNumOverride)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE insplan ADD OrthoAutoProcCodeNumOverride number(20)";
					Db.NonQ(command);
					command="UPDATE insplan SET OrthoAutoProcCodeNumOverride = 0 WHERE OrthoAutoProcCodeNumOverride IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE insplan MODIFY OrthoAutoProcCodeNumOverride NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX insplan_OrthoAutoProcCodeNumOv ON insplan (OrthoAutoProcCodeNumOverride)";
					Db.NonQ(command);
				}
				//insplan.OrthoAutoFeeBilled
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE insplan ADD OrthoAutoFeeBilled double NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE insplan ADD OrthoAutoFeeBilled number(38,8)";
					Db.NonQ(command);
					command="UPDATE insplan SET OrthoAutoFeeBilled = 0 WHERE OrthoAutoFeeBilled IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE insplan MODIFY OrthoAutoFeeBilled NOT NULL";
					Db.NonQ(command);
				}
				//insplan.OrthoAutoClaimDaysWait
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE insplan ADD OrthoAutoClaimDaysWait int NOT NULL";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE insplan ADD OrthoAutoClaimDaysWait number(11)";
					Db.NonQ(command);
					command="UPDATE insplan SET OrthoAutoClaimDaysWait = 0 WHERE OrthoAutoClaimDaysWait IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE insplan MODIFY OrthoAutoClaimDaysWait NOT NULL";
					Db.NonQ(command);
				}
				#endregion
				#region patplan
				//patplan.OrthoAutoFeeBilledOverride
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patplan ADD OrthoAutoFeeBilledOverride double NOT NULL DEFAULT -1";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patplan ADD OrthoAutoFeeBilledOverride number(38,8)";
					Db.NonQ(command);
					command="UPDATE patplan SET OrthoAutoFeeBilledOverride = -1 WHERE OrthoAutoFeeBilledOverride IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patplan MODIFY OrthoAutoFeeBilledOverride NOT NULL";
					Db.NonQ(command);
				}
				//patplan.OrthoAutoNextClaimDate
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patplan ADD OrthoAutoNextClaimDate date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patplan ADD OrthoAutoNextClaimDate date";
					Db.NonQ(command);
					command="UPDATE patplan SET OrthoAutoNextClaimDate = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE OrthoAutoNextClaimDate IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patplan MODIFY OrthoAutoNextClaimDate NOT NULL";
					Db.NonQ(command);
				}
				#endregion
				#region patientnote
				//patientnote.OrthoMonthsTreatOverride
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patientnote ADD OrthoMonthsTreatOverride int NOT NULL DEFAULT -1";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patientnote ADD OrthoMonthsTreatOverride number(11)";
					Db.NonQ(command);
					command="UPDATE patientnote SET OrthoMonthsTreatOverride = -1 WHERE OrthoMonthsTreatOverride IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patientnote MODIFY OrthoMonthsTreatOverride NOT NULL";
					Db.NonQ(command);
				}
				#endregion
				#endregion
				if(DataConnection.DBtype==DatabaseType.MySql) {
					 command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptAllowEmptyComplete','1')";//Enabled by default
					 Db.NonQ(command);
				}
				else {//oracle
					 command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'ApptAllowEmptyComplete','1')";//Enabled by default
					 Db.NonQ(command);
				}
				//Add ReferralEdit permission to everyone. (Previously no restriction to editing.)------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				listLongs = Db.GetListLong(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					foreach(long userGroupNum in listLongs) {
						command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES ("+POut.Long(userGroupNum)+",130)";
						Db.NonQ(command);
					}
				}
				else {//oracle
					foreach(long userGroupNum in listLongs) {
						command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType) "
							+"VALUES ((SELECT MAX(GroupPermNum)+1 FROM grouppermission),TO_DATE('0001-01-01','YYYY-MM-DD'),0,"+POut.Long(userGroupNum)+",130)";
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patient ADD PatNumCloneFrom bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE patient ADD INDEX (PatNumCloneFrom)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patient ADD PatNumCloneFrom number(20)";
					Db.NonQ(command);
					command="UPDATE patient SET PatNumCloneFrom = 0 WHERE PatNumCloneFrom IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patient MODIFY PatNumCloneFrom NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX patient_PatNumCloneFrom ON patient (PatNumCloneFrom)";
					Db.NonQ(command);
				}
				//Add PatientBillingEdit permission to everyone------------------------------------------------------
				command="SELECT DISTINCT UserGroupNum FROM grouppermission";
				listLongs = Db.GetListLong(command);
				if(DataConnection.DBtype==DatabaseType.MySql) {
					foreach(long userGroupNum in listLongs) {
						command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES ("+POut.Long(userGroupNum)+",131)";//131=PatientBillingEdit 
						Db.NonQ(command);
					}
				}
				else {//oracle
					foreach(long userGroupNum in listLongs) {
						command="INSERT INTO grouppermission (GroupPermNum,NewerDate,NewerDays,UserGroupNum,PermType) "
							+"VALUES ((SELECT MAX(GroupPermNum)+1 FROM grouppermission),TO_DATE('0001-01-01','YYYY-MM-DD'),0,"+POut.Long(userGroupNum)+",131)";//131=PatientBillingEdit 
						Db.NonQ(command);
					}
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('InsVerifyFutureDateBenefitYear','0')";//Disabled by default
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'InsVerifyFutureDateBenefitYear','0')";//Disabled by default
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('InsVerifyDaysFromPastDueAppt','1')";//1 day by default
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'InsVerifyDaysFromPastDueAppt','1')";//1 day by default
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE claimproc ADD DateInsFinalized date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
					command=@"UPDATE claimproc 
									SET DateInsFinalized=(SELECT COALESCE(MIN(claimpayment.CheckDate),'0001-01-01') FROM claimpayment WHERE claimpayment.ClaimPaymentNum=claimproc.ClaimPaymentNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE claimproc ADD DateInsFinalized date";
					Db.NonQ(command);
					command="UPDATE claimproc SET DateInsFinalized = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateInsFinalized IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE claimproc MODIFY DateInsFinalized NOT NULL";
					Db.NonQ(command);
					command=@"UPDATE claimproc 
									SET DateInsFinalized=(SELECT COALESCE(MIN(claimpayment.CheckDate),TO_DATE('0001-01-01','YYYY-MM-DD')) FROM claimpayment WHERE claimpayment.ClaimPaymentNum=claimproc.ClaimPaymentNum)";
					Db.NonQ(command);
				}
				//Create Sheet preferences
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=6";
				long SheetsDefaultConsent=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=14";
				long SheetsDefaultDepositSlip=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=13";
				long SheetsDefaultExamSheet=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=12";
				long SheetsDefaultLabSlip=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=4";
				long SheetsDefaultLabelAppointment=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=1";
				long SheetsDefaultLabelCarrier=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=0";
				long SheetsDefaultLabelPatient=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=2";
				long SheetsDefaultLabelReferral=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=11";
				long SheetsDefaultMedicalHistory=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=16";
				long SheetsDefaultMedLabResults=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=9";
				long SheetsDefaultPatientForm=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=7";
				long SheetsDefaultPatientLetter=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=19";
				long SheetsDefaultPaymentPlan=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=8";
				long SheetsDefaultReferralLetter=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=3";
				long SheetsDefaultReferralSlip=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=10";
				long SheetsDefaultRoutingSlip=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=5";
				long SheetsDefaultRx=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=20";
				long SheetsDefaultRxMulti=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=18";
				long SheetsDefaultScreening=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=15";
				long SheetsDefaultStatement=PIn.Long(Db.GetScalar(command));
				command="SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=17";
				long SheetsDefaultTreatmentPlan=PIn.Long(Db.GetScalar(command));
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultConsent','"+POut.Long(SheetsDefaultConsent)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultDepositSlip','"+POut.Long(SheetsDefaultDepositSlip)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultExamSheet','"+POut.Long(SheetsDefaultExamSheet)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultLabSlip','"+POut.Long(SheetsDefaultLabSlip)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultLabelAppointment','"+POut.Long(SheetsDefaultLabelAppointment)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultLabelCarrier','"+POut.Long(SheetsDefaultLabelCarrier)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultLabelPatient','"+POut.Long(SheetsDefaultLabelPatient)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultLabelReferral','"+POut.Long(SheetsDefaultLabelReferral)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultMedicalHistory','"+POut.Long(SheetsDefaultMedicalHistory)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultMedLabResults','"+POut.Long(SheetsDefaultMedLabResults)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultPatientForm','"+POut.Long(SheetsDefaultPatientForm)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultPatientLetter','"+POut.Long(SheetsDefaultPatientLetter)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultPaymentPlan','"+POut.Long(SheetsDefaultPaymentPlan)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultReferralLetter','"+POut.Long(SheetsDefaultReferralLetter)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultReferralSlip','"+POut.Long(SheetsDefaultReferralSlip)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultRoutingSlip','"+POut.Long(SheetsDefaultRoutingSlip)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultRx','"+POut.Long(SheetsDefaultRx)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultRxMulti','"+POut.Long(SheetsDefaultRxMulti)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultScreening','"+POut.Long(SheetsDefaultScreening)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultStatement','"+POut.Long(SheetsDefaultStatement)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefName,ValueString) VALUES('SheetsDefaultTreatmentPlan','"+POut.Long(SheetsDefaultTreatmentPlan)+"')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultConsent','"+POut.Long(SheetsDefaultConsent)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultDepositSlip','"+POut.Long(SheetsDefaultDepositSlip)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultExamSheet','"+POut.Long(SheetsDefaultExamSheet)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultLabSlip','"+POut.Long(SheetsDefaultLabSlip)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultLabelAppointment','"+POut.Long(SheetsDefaultLabelAppointment)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultLabelCarrier','"+POut.Long(SheetsDefaultLabelCarrier)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultLabelPatient','"+POut.Long(SheetsDefaultLabelPatient)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultLabelReferral','"+POut.Long(SheetsDefaultLabelReferral)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultMedicalHistory','"+POut.Long(SheetsDefaultMedicalHistory)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultMedLabResults','"+POut.Long(SheetsDefaultMedLabResults)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultPatientForm','"+POut.Long(SheetsDefaultPatientForm)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultPatientLetter','"+POut.Long(SheetsDefaultPatientLetter)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultPaymentPlan','"+POut.Long(SheetsDefaultPaymentPlan)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultReferralLetter','"+POut.Long(SheetsDefaultReferralLetter)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultReferralSlip','"+POut.Long(SheetsDefaultReferralSlip)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultRoutingSlip','"+POut.Long(SheetsDefaultRoutingSlip)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultRx','"+POut.Long(SheetsDefaultRx)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultRxMulti','"+POut.Long(SheetsDefaultRxMulti)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultScreening','"+POut.Long(SheetsDefaultScreening)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultStatement','"+POut.Long(SheetsDefaultStatement)+"')";
					Db.NonQ(command);
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SheetsDefaultTreatmentPlan','"+POut.Long(SheetsDefaultTreatmentPlan)+"')";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS discountplan";
					Db.NonQ(command);
					command=@"CREATE TABLE discountplan (
						DiscountPlanNum bigint NOT NULL auto_increment PRIMARY KEY,
						Description varchar(255) NOT NULL,
						FeeSchedNum bigint NOT NULL,
						DefNum bigint NOT NULL,
						INDEX(FeeSchednum),
						INDEX(DefNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE discountplan'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE discountplan (
						DiscountPlanNum number(20) NOT NULL,
						Description varchar2(255),
						FeeSchedNum number(20) NOT NULL,
						DefNum number(20) NOT NULL,
						CONSTRAINT discountplan_DiscountPlanNum PRIMARY KEY (DiscountPlanNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX discountplan_FeeSchedNum ON discountplan (FeeSchednum)";
					Db.NonQ(command);
					command=@"CREATE INDEX discountplan_DefNum ON discountplan (DefNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patient ADD DiscountPlanNum bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE patient ADD INDEX (DiscountPlanNum)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patient ADD DiscountPlanNum number(20)";
					Db.NonQ(command);
					command="UPDATE patient SET DiscountPlanNum = 0 WHERE DiscountPlanNum IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patient MODIFY DiscountPlanNum NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX patient_DiscountPlanNum ON patient (DiscountPlanNum)";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('SignatureAllowDigital','1')";//Enabled by default
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT MAX(PrefNum)+1 FROM preference),'SignatureAllowDigital','1')";//Enabled by default
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE autonote ADD Category bigint NOT NULL";
					Db.NonQ(command);
					command="ALTER TABLE autonote ADD INDEX (Category)";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE autonote ADD Category number(20)";
					Db.NonQ(command);
					command="UPDATE autonote SET Category = 0 WHERE Category IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE autonote MODIFY Category NOT NULL";
					Db.NonQ(command);
					command=@"CREATE INDEX autonote_Category ON autonote (Category)";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.4.1.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_4_7();
		}

		private static void To16_4_7() {
			if(FromVersion<new Version("16.4.7.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.4.7");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="DROP TABLE IF EXISTS clinicpref";
					Db.NonQ(command);
					command=@"CREATE TABLE clinicpref (
						ClinicPrefNum bigint NOT NULL auto_increment PRIMARY KEY,
						ClinicNum bigint NOT NULL,
						PrefName varchar(255) NOT NULL,
						ValueString text NOT NULL,
						INDEX(ClinicNum)
						) DEFAULT CHARSET=utf8";
					Db.NonQ(command);
				}
				else {//oracle
					command="BEGIN EXECUTE IMMEDIATE 'DROP TABLE clinicpref'; EXCEPTION WHEN OTHERS THEN NULL; END;";
					Db.NonQ(command);
					command=@"CREATE TABLE clinicpref (
						ClinicPrefNum number(20) NOT NULL,
						ClinicNum number(20) NOT NULL,
						PrefName varchar2(255),
						ValueString varchar2(4000),
						CONSTRAINT clinicpref_ClinicPrefNum PRIMARY KEY (ClinicPrefNum)
						)";
					Db.NonQ(command);
					command=@"CREATE INDEX clinicpref_ClinicNum ON clinicpref (ClinicNum)";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.4.7.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_4_9();
		}

		private static void To16_4_9() {
			if(FromVersion<new Version("16.4.9.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.4.9");//No translation in convert script.
				string command;
				//Remove all the \r\n characters from the setup wikipage incase people have manually fixed their setup pages.
				command="UPDATE wikipage SET PageContent = REPLACE(PageContent, '\r\n', '\n') WHERE PageTitle='_Master' AND IsDraft=0";
				Db.NonQ(command);
				//Now revert all /n back to /r/n so that the wiki setup textbox correctly shows newline characters
				command="UPDATE wikipage SET PageContent = REPLACE(PageContent, '\n', '\r\n') WHERE PageTitle='_Master' AND IsDraft=0";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '16.4.9.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_4_11();
		}

		private static void To16_4_11() {
			if(FromVersion<new Version("16.4.11.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.4.11");//No translation in convert script.
				string command;
				command="ALTER TABLE task DROP COLUMN IsDraft";
				Db.NonQ(command);
				command="ALTER TABLE tasknote DROP COLUMN IsDraft";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '16.4.11.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_4_13();
		}

		private static void To16_4_13() {
			if(FromVersion<new Version("16.4.13.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.4.13");//No translation in convert script.
				string command;
				command="ALTER TABLE taskhist DROP COLUMN IsDraft";
				Db.NonQ(command);
				command="SELECT ProgramNum FROM program WHERE ProgName='Podium'";
				long podiumProgNum=PIn.Long(Db.GetScalar(command));
				command="UPDATE programproperty SET PropertyDesc = 'Enter 0 to use Open Dental for sending review invitations, or 1 to use the Service' "
					+"WHERE ProgramNum="+podiumProgNum+" "
					+"AND PropertyDesc = 'Enter 0 to use Open Dental for sending review invitations, or 1 to use eConnector'";
				Db.NonQ(command);
				command="UPDATE preference SET ValueString = '16.4.13.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_4_15();
		}

		private static void To16_4_15() {
			if(FromVersion<new Version("16.4.15.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.4.15");//No translation in convert script.
				string command="SELECT COUNT(*) FROM preference WHERE PrefName = 'QuickBooksClassRefsEnabled'";
				if(Db.GetCount(command)=="0") {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						command="INSERT INTO preference (PrefName,ValueString) VALUES('QuickBooksClassRefsEnabled','0')";//off by default
						Db.NonQ(command);
					}
					else {//oracle
						command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'QuickBooksClassRefsEnabled','0')";
						Db.NonQ(command);
					}
				}
				//Frequency limitation convert script previously here. Removed in this version and added to To16_4_19().
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.4.15 - Adding index claimproc.DateCP");//No translation in convert script.
				try {
					if(DataConnection.DBtype==DatabaseType.MySql) {
						if(!LargeTableHelper.IndexExists("claimproc","DateCP")) {
							command="ALTER TABLE claimproc ADD INDEX (DateCP)";
							Db.NonQ(command);
						}
					}
					else {//oracle
						command=@"CREATE INDEX claimproc_DateCP ON claimproc (DateCP)";
						Db.NonQ(command);
					}
				}
				catch(Exception) { }//Only an index.
				command="UPDATE preference SET ValueString = '16.4.15.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_4_19();
		}

		private static void To16_4_19() {
			if(FromVersion<new Version("16.4.19.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.4.19");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT COUNT(*) FROM task WHERE IsRepeating != 0 OR YEAR(DateTask) > 1880";
				}
				else {
					command="SELECT COUNT(*) FROM task WHERE IsRepeating != 0 OR CAST(TO_CHAR(DateTask,'YYYY') AS NUMBER) > 1880";
				}
				if(Db.GetCount(command)!="0") {//Turn repeating tasks back on because it may have been turned off while still in use.
					command="UPDATE preference SET ValueString='1' WHERE PrefName='TasksUseRepeating'";
					Db.NonQ(command);
				}
				#region Revert 16.4.15 Benefit Frequency Changes
				if(FromVersion>=new Version("16.4.15.0")) {
					//Only run this script to repair changes made if user is updating from 16.4.15.
					//It is unlikely but possible that this will "fix" not only the rows that were created in error but also some user defined rows.
					//This is a known and accepted risk per discussion with NS on 3 Feb 2017.
					command="SELECT ProcCode,CodeNum FROM procedurecode "
						+"WHERE ProcCode IN ('D0270','D0272','D0273','D0210','D0120','D0145','D0150','D0180')";
					Dictionary<string,long> dictProcs=Db.GetTable(command).Select()
						.ToDictionary(x => PIn.String(x["ProcCode"].ToString()),x => PIn.Long(x["CodeNum"].ToString()));
					string whereClause="WHERE BenefitType=5 "//Limitation
						+"AND MonetaryAmt=-1 "
						+"AND PatPlanNum=0 "
						+"AND Percent=-1 "
						+"AND QuantityQualifier IN (1,4,5) ";//NumberOfServices,Years,Months
					#region Revert BW Frequency Changes
					if(new[] { "D0270","D0272","D0273" }.All(x => dictProcs.ContainsKey(x))) {
						ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.4.19 - Reconcile bitewing frequency limitations");//No translation in convert script.
						command="DELETE FROM benefit "+whereClause+"AND TimePeriod=0 AND CoverageLevel=0 "
							+"AND CodeNum IN ("+dictProcs["D0270"]+","+dictProcs["D0272"]+","+dictProcs["D0273"]+")";
						Db.NonQ(command);
					}
					#endregion Revert BW Frequency Changes
					#region Revert Pano/FMX Frequency Changes
					if(dictProcs.ContainsKey("D0210")) {
						ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.4.19 - Reconcile Pano/FMX frequency limitations");//No translation in convert script.
						command="DELETE FROM benefit "+whereClause+"AND TimePeriod=0 AND CoverageLevel=0 "
							+"AND CodeNum="+dictProcs["D0210"];
						Db.NonQ(command);
					}
					#endregion Revert Pano/FMX Frequency Changes
					#region Revert Exam Frequency Changes
					if(new[] { "D0120","D0145","D0150","D0180" }.All(x => dictProcs.ContainsKey(x))) {
						ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.4.19 - Reconcile exam frequency limitations");//No translation in convert script.
						command="DELETE FROM benefit "+whereClause+"AND TimePeriod=0 AND CoverageLevel=0 "
							+"AND CodeNum IN ("+dictProcs["D0145"]+","+dictProcs["D0150"]+","+dictProcs["D0180"]+")";
						Db.NonQ(command);
						command="UPDATE benefit SET CodeNum=0 "
							+whereClause
							+"AND CovCatNum IN (SELECT CovCatNum FROM covcat WHERE EbenefitCat=12) "//12=RoutinePreventive
							+"AND CodeNum="+dictProcs["D0120"];
						Db.NonQ(command);
					}
					#endregion Revert Exam Frequency Changes
				}
				#endregion Revert 16.4.15 Benefit Frequency Changes
				//Insert new insurance benefit frequency preferences.  These prefs will default to D codes, even for foreign users.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('InsBenBWCodes','D0272,D0274')";//2BW,4BW
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
						+" VALUES((SELECT MAX(PrefNum)+1 FROM preference),'InsBenBWCodes','D0272,D0274')";//2BW,4BW
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('InsBenPanoCodes','D0210,D0330')";//FMX,Pano
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT MAX(PrefNum)+1 FROM preference),'InsBenPanoCodes','D0210,D0330')";//FMX,Pano
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('InsBenExamCodes','D0120,D0150')";//PerEx,CmpEx
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) "
						+"VALUES((SELECT MAX(PrefNum)+1 FROM preference),'InsBenExamCodes','D0120,D0150')";//PerEx,CmpEx
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.4.19.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_4_23();
		}

		private static void To16_4_23() {
			if(FromVersion<new Version("16.4.23.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.4.23");//No translation in convert script.
				string command;
				//Get the Canadian claim form.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD6' LIMIT 1";
				}
				else {//oracle doesn't have LIMIT
					command="SELECT * FROM (SELECT ClaimFormNum FROM claimform WHERE UniqueID='OD6') WHERE RowNum<=1";
				}
				long claimFormNum=PIn.Long(Db.GetScalar(command));
				command="UPDATE claimformitem SET FieldName='P1ToothNumOrArea' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND FieldName='P1ToothNumber'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='P2ToothNumOrArea' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND FieldName='P2ToothNumber'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='P3ToothNumOrArea' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND FieldName='P3ToothNumber'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='P4ToothNumOrArea' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND FieldName='P4ToothNumber'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='P5ToothNumOrArea' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND FieldName='P5ToothNumber'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='P6ToothNumOrArea' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND FieldName='P6ToothNumber'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='P7ToothNumOrArea' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND FieldName='P7ToothNumber'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='P8ToothNumOrArea' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND FieldName='P8ToothNumber'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='P9ToothNumOrArea' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND FieldName='P9ToothNumber'";
				Db.NonQ(command);
				command="UPDATE claimformitem SET FieldName='P10ToothNumOrArea' WHERE ClaimFormNum="+POut.Long(claimFormNum)+" AND FieldName='P10ToothNumber'";
				Db.NonQ(command);				
				command="UPDATE preference SET ValueString = '16.4.23.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_4_28();
		}

		private static void To16_4_28() {
			if(FromVersion<new Version("16.4.28.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.4.28");//No translation in convert script.
				string command;
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="ALTER TABLE patientnote ADD DateOrthoPlacementOverride date NOT NULL DEFAULT '0001-01-01'";
					Db.NonQ(command);
				}
				else {//oracle
					command="ALTER TABLE patientnote ADD DateOrthoPlacementOverride date";
					Db.NonQ(command);
					command="UPDATE patientnote SET DateOrthoPlacementOverride = TO_DATE('0001-01-01','YYYY-MM-DD') WHERE DateOrthoPlacementOverride IS NULL";
					Db.NonQ(command);
					command="ALTER TABLE patientnote MODIFY DateOrthoPlacementOverride NOT NULL";
					Db.NonQ(command);
				}
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('OrthoPlacementProcsList','')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES ((SELECT MAX(PrefNum)+1 FROM preference),'OrthoPlacementProcsList','')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.4.28.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_4_29();
		}

		private static void To16_4_29() {
			if(FromVersion<new Version("16.4.29.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.4.29");//No translation in convert script
				string command;
				//DatabaseMaintenanceDisableOptimize  
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES('DatabaseMaintenanceDisableOptimize','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES "
						+"((SELECT MAX(PrefNum)+1 FROM preference),'DatabaseMaintenanceDisableOptimize','0')";
					Db.NonQ(command);
				}
				//DatabaseMaintenanceSkipCheckTable   
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference (PrefName,ValueString) VALUES ('DatabaseMaintenanceSkipCheckTable','0')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference (PrefNum,PrefName,ValueString) VALUES "
						+"((SELECT MAX(PrefNum)+1 FROM preference),'DatabaseMaintenanceSkipCheckTable','0')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.4.29.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			To16_4_32();
		}

		private static void To16_4_32() {
			if(FromVersion<new Version("16.4.32.0")) {
				ODEvent.Fire(ODEventType.ConvertDatabases,"Upgrading database to version: 16.4.32");//No translation in convert script
				string command;
				//Add ProcProvChangesClaimProcWithClaim pref if doesn't exist.  Default to on.
				if(DataConnection.DBtype==DatabaseType.MySql) {
					command="INSERT INTO preference(PrefName,ValueString) VALUES('ProcProvChangesClaimProcWithClaim','1')";
					Db.NonQ(command);
				}
				else {//oracle
					command="INSERT INTO preference(PrefNum,PrefName,ValueString) VALUES((SELECT COALESCE(MAX(PrefNum),0)+1 FROM preference),"
						+"'ProcProvChangesClaimProcWithClaim','1')";
					Db.NonQ(command);
				}
				command="UPDATE preference SET ValueString = '16.4.32.0' WHERE PrefName = 'DataBaseVersion'";
				Db.NonQ(command);
			}
			//To16_4_X();
		}

		//This is where the daisy chain update old pattern ends.  The new pattern begins with version 17.1 and is located in ConvertDatabases5.cs


	}
}
